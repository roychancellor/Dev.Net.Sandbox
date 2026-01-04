using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using NJINSimulator.Common.Models;
using NLog;
using NJINSimulator.Common.Serialization;
using PROBESimulator.IO.Swagger;
using PROBESimulator.Common.Contracts;
using UFOSimulator.Processors;

namespace UFOSimulator.IO.Controllers
{
    public class DeltaTController : ApiController
    {
        private static readonly Logger _logger = LogManager.GetLogger("deltaTLogger");

        public DeltaTController()
        {
            _logger.Trace("In DeltaTController constructor");
        }

        [SwaggerConsumes("application/xml", "application/json")]
        [SwaggerProduces("application/xml", "application/json")]
        [SwaggerResponse(HttpStatusCode.OK, "Response to Valid Request", typeof(NJINDeltaT))]
        [SwaggerResponse(HttpStatusCode.BadRequest, "Response to Bad Request", typeof(Error))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, "Response to Internal Server Error", typeof(Error))]
        public IHttpActionResult Post(NJINDeltaT request)
        {
            try
            {
                _logger.Trace("DeltaTController: Request received.");
                if (request == null)
                {
                    return BadRequest("Request is NULL. Check the structure of the request.");
                }
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                                .SelectMany(x => x.Errors)
                                .Select(e => e.ErrorMessage)
                                .ToList();
                    return BadRequest(string.Join(" | ", errors));
                }

                // Call the TPS handler
                _logger.Trace($"=====> Handling DeltaT Request | Timestamp: {request.TimeStamp:yyyy-MM-dd HH:mm:ss.fff} | IBCID: {request.IBCID}");
                UFOProcessorNJINDeltaT.Instance.Process(request);
                _logger.Trace($"<===== Handled");

                // For now, simply log the result.
                /*
                var toLog = Serialization.Serialize(request);
                if (toLog == null)
                {
                    var msg = "DeltaTController: Unable to serialize the TW";
                    _logger.Error(msg);
                    throw new Exception(msg);
                }

                _logger.Info($"UFO DeltaT Controller: DIRECTION: {request.Direction} | IBCID: {request.IBCID} | TIME STAMP: {request.TimeStamp:yyyy-MM-dd HH:mm:ss.fff}");
                _logger.Trace($"Request:\n{toLog}");
                */

                if (request.Verbose)
                {
                    return Ok(request);
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
