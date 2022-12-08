using Models.Common;
using Models.Query;
using Models.Response;
using MultiStateQueryMocks.Utilities;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiStateQuery.Logic
{
    public class Logic
    {
        private static Logger _log = LogManager.GetCurrentClassLogger();

        public static string ProcessDQString(string toProcess)
        {
            _log.Trace($"Received the string: {toProcess}");
            var header = GetHeader(toProcess);
            if (header == null)
            {
                _log.Error($"Error processing the header. Returning null string.");
                return null;
            }
            string toReturn = GetMockResponsePayload(header);
            _log.Trace($"Returning the payload:\n{toReturn}");
            return toReturn;
        }
        
        public static DR ProcessDQ(DQ toProcess)
        {
            DR toReturn = new DR();
            return toReturn;
        }

        private static Header GetHeader(string queryPayload)
        {
            var toReturn = Utilities.Deserialize<NLETS>(queryPayload);
            if (toReturn == null)
            {
                _log.Error($"Deserialization of payload failed.");
                return null;
            }
            if (!ValidateHeader(toReturn?.Header))
            {
                _log.Error($"Header is invalid - one or more required fields (SendORI, DestORI, and MKE) is null or empty.");
                return null;
            }
            return toReturn.Header;
        }

        private static bool ValidateHeader(Header toValidate)
        {
            if (toValidate == null)
            {
                return false;
            }
            return !string.IsNullOrEmpty(toValidate.SendORI) &&
                   !string.IsNullOrEmpty(toValidate.DestORI) &&
                   !string.IsNullOrEmpty(toValidate.MessageKey);
        }

        private static string GetMockResponsePayload(Header header)
        {
            // Determine the type of query and where it is destined
            // Retrieve the string response
            // string response = _repository.GetMock(parameters...);
            // Populate the string with header data
            string toReturn = $"Send ORI = {header.DestORI}0000000, DestORI = {header.SendORI}, Control Field = {header.ControlField}, MKE = {header.MessageKey}";
            // Return the string
            return toReturn;
        }
    }
}
