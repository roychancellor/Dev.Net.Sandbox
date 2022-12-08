using MultiStateQueryPOC.DataAccess;
using MultiStateQueryPOC.Models.Common;
using MultiStateQueryPOC.Models.Query;
using MultiStateQueryPOC.Models.Response;
using MultiStateQueryPOC.Utilities;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MultiStateQueryPOC.Controllers
{
    public class QueryController : ApiController
    {
        private Logger _log = LogManager.GetCurrentClassLogger();

        // This controller will accept POST requests with query payloads matching the varous query payload class types
        
        public HttpResponseMessage PostDQReturnDR(DQ dQ)
        {
            string errMsg = string.Empty;
            
            // Check the ModelState
            if (!ModelState.IsValid)
            {
                _log.Error($"The model state is invalid\n{ModelState}");
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState);
            }

            // Validate the Query
            if (dQ == null || dQ.Header == null || dQ.Header.DestORI == null || dQ.Header.MessageKey == null)
            {
                errMsg = dQ == null ? "dQ is null" : dQ.Header == null ? $"dQ.Header is null" : $"dQ.Header.DestORI is null: {dQ}";
                _log.Error(errMsg);
                return Request.CreateResponse(HttpStatusCode.BadRequest, errMsg);
            }
            _log.Info($"The inbound deserialized DQ object is\n{dQ}");

            // Build the response mock entity
            var queryStateCode = dQ.Header.DestORI.ToUpper();
            var mke = dQ.Header.MessageKey.ToUpper();
            Mock stateMockEntity = null;

            var responseMKE = Utilities.Utilities.QueryToResponseMessageKey(mke);

            DR toReturn;
            using (var ctx = new QueryContext())
            {
                // Find the mock that matches the state/response message key combination
                var mocksMessageKey = ctx.Mocks.Where(m => m.MessageKey.Equals(responseMKE)).ToList();
                _log.Trace($"mocksMessageKey:\n{Utilities.Utilities.ListToString(mocksMessageKey)}");
                var states = ctx.States.Where(st => st.StateCode.Equals(queryStateCode)).ToList();
                _log.Trace($"states:\n{Utilities.Utilities.ListToString(states)}");
                var stateMockEntities = (from st in states join mmk in mocksMessageKey on st.MockID equals mmk.MockID select mmk).ToList();
                _log.Trace($"stateMockEntities:\n{Utilities.Utilities.ListToString(stateMockEntities)}");
                stateMockEntity = stateMockEntities.First(sme => sme.MessageKey.Equals(responseMKE));
                _log.Trace($"stateMockEntity:\n{stateMockEntity}");
                if (stateMockEntity == null)
                {
                    errMsg = $"Mock not found for State {queryStateCode} and MKE {responseMKE} combination";
                    _log.Error(errMsg);
                    return Request.CreateResponse(HttpStatusCode.NotFound, errMsg);
                }

                // Build the response from the stateMockEntity
                var stateSendORI = states.First(st => st.MockID == stateMockEntity.MockID).SendORI;
                toReturn = new DR
                {
                    Header = new Header
                    { 
                        ControlField = dQ.Header.ControlField,
                        DestORI = dQ.Header.SendORI,
                        SendORI = stateSendORI
                    },
                    ResponseBody = stateMockEntity.MockBody,
                };
            }

            _log.Trace($"Returning the DR:\n{toReturn}");
            return Request.CreateResponse(HttpStatusCode.OK, toReturn, Configuration.Formatters.XmlFormatter);
        }
    }
}
