using PROBESimulator.Common.Contracts;
using PROBESimulator.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PROBESimulator.Probes
{
    public sealed class PROBEServicesSingleton
    {
        private static readonly Dictionary<PROBEServiceContracts, AbstractPROBEService> _probeServices = new Dictionary<PROBEServiceContracts, AbstractPROBEService>();

        static PROBEServicesSingleton()
        {
            Configure();
        }
        private PROBEServicesSingleton()
        {
        }

        public static Dictionary<PROBEServiceContracts, AbstractPROBEService> ProbeServices
        {
            get
            {
                if (_probeServices.Count == 0)
                {
                    Configure();
                }
                return _probeServices;
            }
        }

        public static bool Configure()
        {
            // TODO: Get all the information from the PROBESimulator.Application App.config file
            _probeServices.Add(PROBEServiceContracts.NJINTPS, new PROBEServiceNJINTPS(PROBEServiceContracts.NJINTPS, "Consumer.PROBE_TPS.VirtualTopic.IO_OUT", "http://localhost:9200/ufosimulator/io/api/v1/TPS", 0));
            _probeServices.Add(PROBEServiceContracts.NJINDeltaTIn, new PROBEServiceNJINDeltaT(PROBEServiceContracts.NJINDeltaTIn, "Consumer.PROBE_DELTA_T_IN.VirtualTopic.IO_OUT", "http://localhost:9200/ufosimulator/io/api/v1/DeltaT", 0));
            _probeServices.Add(PROBEServiceContracts.NJINDeltaTOut, new PROBEServiceNJINDeltaT(PROBEServiceContracts.NJINDeltaTOut, "Consumer.PROBE_DELTA_T_OUT.VirtualTopic.PO_OUT", "http://localhost:9200/ufosimulator/io/api/v1/DeltaT", 0));
            return true;
        }
    }
}
