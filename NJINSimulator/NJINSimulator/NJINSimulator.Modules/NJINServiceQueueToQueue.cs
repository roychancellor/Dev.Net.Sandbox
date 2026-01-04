using NJINSimulator.Common.Config;
using NJINSimulator.Common.Models;
using NJINSimulator.Common.Utilities;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NJINSimulator.Modules
{
    public class NJINServiceQueueToQueue : AbstractNJINService
    {
        public NJINServiceQueueToQueue(NJINServiceNames serviceName, string receiveFromQueue, string produceToQueue, int processTimeMilliseconds)
        {
            ServiceName = serviceName;
            ReceiveFromSource = receiveFromQueue.IsNullOrEmpty() ? "NULL_RECEIVE" : receiveFromQueue;
            ProduceToDestination = produceToQueue.IsNullOrEmpty() ? "DEADLETTER" : produceToQueue;
            ProcessTimeMilliseconds = processTimeMilliseconds == 0 ? 100 : processTimeMilliseconds;
            Init();
        }
        public NJINServiceQueueToQueue(NJINServiceNames serviceName, NJINServiceConfig nJINServiceConfig)
        {
            ServiceName = serviceName;
            nJINServiceConfig = nJINServiceConfig ?? new NJINServiceConfig();
            ReceiveFromSource = nJINServiceConfig.Source ?? "NULL_RECEIVE";
            ProduceToDestination = nJINServiceConfig.Destination ?? "DEADLETTER";
            ProcessTimeMilliseconds = nJINServiceConfig.ProcessingTime;
            Init();
            _logger.Trace($"{ServiceName}: Leaving constructor: S: {ReceiveFromSource} | D: {ProduceToDestination} | Delay: {ProcessTimeMilliseconds}");
        }
    }
}
