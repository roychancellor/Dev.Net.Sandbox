using Apache.NMS;
using NJINSimulator.Common.ActiveMQ;
using NJINSimulator.Common.Config;
using NJINSimulator.Common.Models;
using NJINSimulator.Common.Serialization;
using NJINSimulator.Common.Utilities;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NJINSimulator.Modules
{
    public class NJINServicePipelineOrchestratorOut : AbstractNJINService
    {
        public NJINServicePipelineOrchestratorOut(NJINServiceNames serviceName, string receiveFromQueue, string produceToQueue, int processTimeMilliseconds)
        {
            ServiceName = serviceName;
            ReceiveFromSource = receiveFromQueue.IsNullOrEmpty() ? "NULL_RECEIVE" : receiveFromQueue;
            ProduceToDestination = produceToQueue.IsNullOrEmpty() ? "DEADLETTER" : produceToQueue;
            ProcessTimeMilliseconds = processTimeMilliseconds == 0 ? 100 : processTimeMilliseconds;
            Init();
        }
        public NJINServicePipelineOrchestratorOut(NJINServiceNames serviceName, NJINServiceConfig nJINServiceConfig)
        {
            ServiceName = serviceName;
            nJINServiceConfig = nJINServiceConfig ?? new NJINServiceConfig();
            ReceiveFromSource = nJINServiceConfig.Source ?? "NULL_RECEIVE";
            ProduceToDestination = nJINServiceConfig.Destination ?? "DEADLETTER";
            ProcessTimeMilliseconds = nJINServiceConfig.ProcessingTime;
            Init();
            _logger.Trace($"{ServiceName}: Leaving constructor: S: {ReceiveFromSource} | D: {ProduceToDestination} | Delay: {ProcessTimeMilliseconds}");
        }

        public override bool Init()
        {
            _logger.Trace($"{ServiceName}: Entering Init");
            if (ReceiveFromSource == null || ProduceToDestination == null)
            {
                return false;
            }

            var amq = AMQServices.Instance;
            if (amq.Connection == null || amq.Session == null)
            {
                _logger.Warn($"{ServiceName}: Found the AMQ connection and/or session null. Attempting to call Init().");
                AMQServices.Init();
            }

            _logger.Trace($"{ServiceName}: Getting Producer.");
            Producer = amq.Producer(ProduceToDestination, true); // because PO ends by publishing to a TOPIC, pass in 'true' for the optional isTopic parameter
            if (Producer == null)
            {
                throw new Exception($"During Init for {ServiceName}: Producer is NULL");
            }
            _logger.Trace($"{ServiceName}: Getting Consumer.");
            Consumer = amq.Consumer(ReceiveFromSource);
            if (Consumer == null)
            {
                throw new Exception($"During Init for {ServiceName}: Consumer is NULL");
            }
            Consumer.Listener += Handle;

            _logger.Trace($"{ServiceName}: Exiting Init");
            return true;
        }

        public override void Handle(object message)
        {
            _logger.Debug($">>> {ServiceName}: Entering Handle");
            if (message == null) return;

            try
            {
                var twToDeser = (message as ITextMessage).Text;
                var TW = Serialization.Deserialize<TransactionalWrapper>(twToDeser, true);
                if (TW == null)
                {
                    throw new Exception($"{ServiceName}: TW is null");
                }
                TW.ProcessResults.Add(new ProcessResult { Result = ServiceName.ToString() + " SUCCESS" });

                // This simulates the NJIN service doing actual work
                Thread.Sleep(ProcessTimeMilliseconds);

                var toSend = Serialization.Serialize(TW);
                if (Producer == null)
                {
                    throw new Exception($"Handle: Producer is NULL. Why????");
                }
                _logger.Trace($"{ServiceName}: Calling AMQ Send.");
                var sendSuccess = AMQServices.Instance.Send(Producer, toSend);
                if (!sendSuccess)
                {
                    throw new Exception($"{ServiceName}: Unable to place message on the queue {ProduceToDestination}");
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"{ServiceName}: Exception caught: {ex.Message}");
                throw ex;
            }
            _logger.Debug($"<<< {ServiceName}: Exiting Handle");
            return;
        }
    }
}
