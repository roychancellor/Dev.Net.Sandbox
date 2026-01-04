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
    public sealed class NJINServicesSingleton
    {
        private static readonly NJINServicesSingleton _instance = new NJINServicesSingleton();
        private Dictionary<NJINServiceNames, AbstractNJINService> _njinServices;

        /*
         * This singleton manages the NJIN services for the NJINSimulator Console App that contains all
         * of the NJIN services except the IO Interface, which lives in a different WebApi project and
         * has its own singleton.
        */

        static NJINServicesSingleton()
        {
        }
        private NJINServicesSingleton()
        {
            _njinServices = new Dictionary<NJINServiceNames, AbstractNJINService>();
            Configure();
        }

        public Dictionary<NJINServiceNames, AbstractNJINService> NjinServices
        {
            get
            {
                if (_njinServices.Count == 0)
                {
                    Configure();
                }
                return _njinServices;
            }
        }

        public static NJINServicesSingleton Instance
        {
            get
            {
                return _instance;
            }
        }

        private bool Configure()
        {
            try
            {
                var njinConfig = NJINSimulatorAppConfigProvider.Instance.NjinConfig;
                _njinServices.AddOrUpdate(NJINServiceNames.RequestManager, new NJINServiceQueueToQueue(NJINServiceNames.RequestManager, njinConfig.SafeRetrieve(NJINServiceNames.RequestManager)));
                _njinServices.AddOrUpdate(NJINServiceNames.PipelineOrchestratorIn, new NJINServiceQueueToQueue(NJINServiceNames.PipelineOrchestratorIn, njinConfig.SafeRetrieve(NJINServiceNames.PipelineOrchestratorIn)));
                _njinServices.AddOrUpdate(NJINServiceNames.Validator, new NJINServiceQueueToQueue(NJINServiceNames.Validator, njinConfig.SafeRetrieve(NJINServiceNames.Validator)));
                _njinServices.AddOrUpdate(NJINServiceNames.RouterDestInfo, new NJINServiceQueueToQueue(NJINServiceNames.RouterDestInfo, njinConfig.SafeRetrieve(NJINServiceNames.RouterDestInfo)));
                _njinServices.AddOrUpdate(NJINServiceNames.RouterRoute, new NJINServiceQueueToQueue(NJINServiceNames.RouterRoute, njinConfig.SafeRetrieve(NJINServiceNames.RouterRoute)));
                _njinServices.AddOrUpdate(NJINServiceNames.ImageProcessor, new NJINServiceQueueToQueue(NJINServiceNames.ImageProcessor, njinConfig.SafeRetrieve(NJINServiceNames.ImageProcessor)));
                _njinServices.AddOrUpdate(NJINServiceNames.Conversion, new NJINServiceQueueToQueue(NJINServiceNames.Conversion, njinConfig.SafeRetrieve(NJINServiceNames.Conversion)));
                _njinServices.AddOrUpdate(NJINServiceNames.PipelineOrchestratorOut, new NJINServicePipelineOrchestratorOut(NJINServiceNames.PipelineOrchestratorOut, njinConfig.SafeRetrieve(NJINServiceNames.PipelineOrchestratorOut)));
                _njinServices.AddOrUpdate(NJINServiceNames.SenderManager, new NJINServiceSenderManager(NJINServiceNames.SenderManager, njinConfig.SafeRetrieve(NJINServiceNames.SenderManager)));
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public string PrintServices()
        {
            if (_njinServices == null) return null;

            StringBuilder sb = new StringBuilder();
            foreach (var k in _njinServices.Keys)
            {
                sb.AppendLine($"{_njinServices.SafeRetrieve(k)}");
            }
            return sb.ToString();
        }
    }
}
