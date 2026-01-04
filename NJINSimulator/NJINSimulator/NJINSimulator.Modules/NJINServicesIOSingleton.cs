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
    public sealed class NJINServicesIOSingleton
    {
        private static readonly NJINServicesIOSingleton _instance = new NJINServicesIOSingleton();
        private Dictionary<NJINServiceNames, AbstractNJINService> _njinServices;

        /*
         * This singleton holds only the NJIN IO Interface service which operates as part of a WebApi project
         * and is distinct from the NJIN services that perform the other functionality by listening to queues
         * in the NJINSimulator Console App.
        */

        static NJINServicesIOSingleton()
        {
        }
        private NJINServicesIOSingleton()
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

        public static NJINServicesIOSingleton Instance
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
                _njinServices.AddOrUpdate(NJINServiceNames.IOInterface, new NJINServiceIOInterface(NJINServiceNames.IOInterface, njinConfig.SafeRetrieve(NJINServiceNames.IOInterface)));
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
