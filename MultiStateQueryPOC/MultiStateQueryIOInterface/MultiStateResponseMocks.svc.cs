using Models.Query;
using Models.Response;
using MultiStateQuery.Logic;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace MultiStateQueryIOInterface
{
    public class MultiStateResponseMocks : IMultiStateResponseMocks
    {
        private static Logger _log = LogManager.GetCurrentClassLogger();
        public string Process(string XMLQueryBody)
        {
            _log.Trace($"====> Process: Received query message:\n{XMLQueryBody}");
            var toReturn = Logic.ProcessDQString(XMLQueryBody);
            _log.Trace($"<==== Process: Returning response message\n{toReturn}");
            return toReturn;
        }

        public DR ProcessDQ(DQ XMLQueryBody)
        {
            _log.Trace($"Received DQ:\n{XMLQueryBody}");
            return Logic.ProcessDQ(XMLQueryBody);
        }
    }
}
