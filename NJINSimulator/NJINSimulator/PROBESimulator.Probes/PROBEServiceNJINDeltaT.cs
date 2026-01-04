using Apache.NMS;
using NJINSimulator.Common.ActiveMQ;
using NJINSimulator.Common.HttpServices;
using NJINSimulator.Common.Models;
using NJINSimulator.Common.Serialization;
using NJINSimulator.Common.Utilities;
using NLog;
using PROBESimulator.Common.Contracts;
using PROBESimulator.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PROBESimulator.Probes
{
    public class PROBEServiceNJINDeltaT : AbstractPROBEService
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        public PROBEServiceNJINDeltaT(PROBEServiceContracts serviceName, string receiveFromQueue, string produceToQueue, int processTimeMilliseconds)
        {
            ServiceName = serviceName;
            ReceiveFromSource = receiveFromQueue.IsNullOrEmpty() ? "NULL_RECEIVE" : receiveFromQueue;
            ProduceToDestination = produceToQueue.IsNullOrEmpty() ? "DEADLETTER" : produceToQueue;
            ProcessTimeMilliseconds = processTimeMilliseconds == 0 ? 100 : processTimeMilliseconds;
            Init();
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

            Producer = null; // because the PROBE Delta T service sends POST requests rather than publishing messages to a queue

            _logger.Trace($"{ServiceName}: Exiting Init");
            return true;
        }

        public override void Handle(object message)
        {
            _logger.Trace($"{ServiceName}: Entering Handle");
            if (message == null) return;

            try
            {
                var twToDeser = (message as ITextMessage).Text;
                _logger.Trace($"{ServiceName}: received message is:\n{twToDeser}");
                var TW = Serialization.Deserialize<TransactionalWrapper>(twToDeser, true);

                // Determine whether the message is coming into the IO Interface or leaving through Sender Manager
                var hasIOInterface = TW.ProcessResults.Any(pr => pr.Result.Contains("IOInterface"));
                var hasSenderManager = TW.ProcessResults.Any(pr => pr.Result.Contains("PipelineOrchestratorOut"));
                var direction = "UNKNOWN";
                if (hasIOInterface)
                {
                    direction = "IN";
                    if (hasSenderManager)
                    {
                        direction = "OUT";
                    }
                }

                var tps = new NJINDeltaT
                {
                    IBCID = TW.InboundCorrelationID,
                    TimeStamp = DateTime.Now,
                    Direction = direction,
                };
                var toSend = Serialization.Serialize(tps);
                if (_logger.IsTraceEnabled) _logger.Trace($"{ServiceName}: Sending POST to '{ProduceToDestination}' with body: {toSend}");
                if (_logger.IsInfoEnabled) _logger.Info($"{ServiceName}: POST to UFO | {direction} | {TW.InboundCorrelationID}");
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
            _logger.Trace($"{ServiceName}: Exiting Handle");
            return;
        }
    }
}
