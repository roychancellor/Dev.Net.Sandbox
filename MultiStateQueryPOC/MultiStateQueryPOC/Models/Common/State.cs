using MultiStateQueryPOC.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MultiStateQueryPOC.Models.Common
{
    public class State
    {
        public int StateID { get; set; }
        public int MockID { get; set; }
        public string StateCode { get; set; }
        public string SendHeaderType { get; set; }
        public string SendPayloadType { get; set; }
        public string SendPayloadCHRIType { get; set; }
        public string SendORI { get; set; }
        public int ResponseDelayMilliseconds { get; set; }

        public virtual Mock Mock { get; set; }

        public override string ToString()
        {
            return $"{nameof(StateID)}: {StateID}, {nameof(MockID)}: {MockID}, {nameof(StateCode)}: {StateCode}, {nameof(SendHeaderType)}: {SendHeaderType}, {nameof(SendPayloadType)}: {SendPayloadType}, {nameof(SendPayloadCHRIType)}: {SendPayloadCHRIType}, {nameof(ResponseDelayMilliseconds)}: {ResponseDelayMilliseconds}, {nameof(SendORI)}: {SendORI}";
        }
    }
}