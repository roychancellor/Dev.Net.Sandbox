using Apache.NMS;
using NJINSimulator.Common.ActiveMQ;
using NJINSimulator.Common.Config;
using NJINSimulator.Common.HttpServices;
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
    public class NJINServiceSenderManager : AbstractNJINService
    {
        public NJINServiceSenderManager(NJINServiceNames serviceName, string receiveFromQueue, string produceToQueue, int processTimeMilliseconds)
        {
            ServiceName = serviceName;
            ReceiveFromSource = receiveFromQueue.IsNullOrEmpty() ? "Consumer.SNDMGR_IN.VirtualTopic.PO_OUT" : receiveFromQueue;
            ProduceToDestination = produceToQueue.IsNullOrEmpty() ? "NULL_PUBLISH" : produceToQueue;
            ProcessTimeMilliseconds = processTimeMilliseconds == 0 ? 100 : processTimeMilliseconds;
            Init();
        }
        public NJINServiceSenderManager(NJINServiceNames serviceName, NJINServiceConfig nJINServiceConfig)
        {
            ServiceName = serviceName;
            nJINServiceConfig = nJINServiceConfig ?? new NJINServiceConfig();
            ReceiveFromSource = nJINServiceConfig.Source ?? "Consumer.SNDMGR_IN.VirtualTopic.PO_OUT";
            ProduceToDestination = nJINServiceConfig.Destination ?? "NULL_PUBLISH";
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
                AMQServices.Init();
            }

            Consumer = amq.Consumer(ReceiveFromSource);
            if (Consumer == null)
            {
                throw new Exception($"During Init for {ServiceName}: Consumer is NULL");
            }
            Consumer.Listener += Handle;

            Producer = null; // because the Sender Manager is a web service which sends POST requests, not publishes messages to a queue

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
                TW.ProcessResults.Add(new ProcessResult { Result = ServiceName.ToString() + " SUCCESS" });
                _logger.Trace($"{ServiceName}: TW is deserialized");
                
                // This simulates the NJIN service doing actual work
                Thread.Sleep(ProcessTimeMilliseconds);

                var startTime = DateTime.Parse(TW.StartDateTime);
                var finishTime = DateTime.Now;
                var elapsedTime = finishTime.Subtract(startTime).TotalMilliseconds;
                TW.FinishDateTime = finishTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
                TW.ElapsedTimeMilliseconds = (int)elapsedTime;

                var toSend = Serialization.Serialize(TW);
                if (_logger.IsTraceEnabled) _logger.Trace($"{ServiceName}: Sending POST to '{ProduceToDestination}' with body: {toSend}");
                if (!_logger.IsTraceEnabled) _logger.Info($"{ServiceName}: Sending POST to '{ProduceToDestination}'");
                var sendSuccess = HttpServices.Send(ProduceToDestination, toSend).Result;
                _logger.Trace($"{ServiceName}: POST sent with result sendSuccess = {sendSuccess}");
                if (!sendSuccess)
                {
                    throw new Exception($"{ServiceName}: Unable to POST message to endpoint {ProduceToDestination}");
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
