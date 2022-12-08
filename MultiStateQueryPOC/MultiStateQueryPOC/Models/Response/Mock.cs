using MultiStateQueryPOC.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MultiStateQueryPOC.Models.Response
{
    public class Mock
    {
        public int MockID { get; set; }
        public string MessageKey { get; set; }
        public string HeaderType { get; set; }
        public string PayloadType { get; set; }
        public string MockBody { get; set; }

        public override string ToString()
        {
            return $"{nameof(MockID)}: {MockID}, {nameof(MessageKey)}: {MessageKey}, {nameof(HeaderType)}: {HeaderType}, {nameof(PayloadType)}: {PayloadType}, {nameof(MockBody)}: {MockBody}";
        }
    }
}