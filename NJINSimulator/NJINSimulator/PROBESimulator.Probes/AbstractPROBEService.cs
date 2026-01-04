using Apache.NMS;
using NJINSimulator.Common.ActiveMQ;
using NJINSimulator.Common.Models;
using NJINSimulator.Common.Serialization;
using NLog;
using PROBESimulator.Common.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PROBESimulator.Services
{
    public abstract class AbstractPROBEService
    {
        static Logger _logger = LogManager.GetCurrentClassLogger();
        
        public PROBEServiceContracts ServiceName { get; set; }
        public string ReceiveFromSource { get; set; }
        public string ProduceToDestination { get; set; }
        public IMessageProducer Producer { get; set; }
        public IMessageConsumer Consumer { get; set; }
        public int ProcessTimeMilliseconds { get; set; }

        public virtual bool Init()
        {
            _logger.Trace($"{ServiceName}: Entering Init");
            if (ReceiveFromSource == null || ProduceToDestination == null)
            {
                return false;
            }

            var amq = AMQServices.Instance;
            if (amq.Connection == null || amq.Session == null)
            {
                AMQServices.Init();
            }

            Producer = amq.Producer(ProduceToDestination);
            if (Producer == null)
            {
                throw new Exception($"During Init for {ServiceName}: Producer is NULL");
            }
            Consumer = amq.Consumer(ReceiveFromSource);
            if (Consumer == null)
            {
                throw new Exception($"During Init for {ServiceName}: Consumer is NULL");
            }
            Consumer.Listener += Handle;

            _logger.Trace($"{ServiceName}: Exiting Init");
            return true;
        }
        
        public virtual void Handle(object message)
        {
            _logger.Trace($"{ServiceName}: Entering Handle");
            if (message == null) return;
            
            try
            {
                var twToDeser = (message as ITextMessage).Text;
                var TW = Serialization.Deserialize<TransactionalWrapper>(twToDeser, true);

                TW.ProcessResults.Add(new ProcessResult { Result = ServiceName.ToString() + " SUCCESS" });

                // HERE IS WHERE THE PROBE SERVICE DOES ITS WORK
                Thread.Sleep(ProcessTimeMilliseconds);

                var toSend = Serialization.Serialize(TW);
                _logger.Trace($"{ServiceName}: Sending to {ProduceToDestination}");
                var sendResult = AMQServices.Instance.Send(Producer, toSend);
            }
            catch (Exception ex)
            {
                _logger.Error($"{ServiceName}: Exception caught: {ex.Message}");
                return;
            }
            _logger.Trace($"{ServiceName}: Exiting Handle");
            return;
        }
    }
}
