using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using NJINSimulator.Common.Models;
using static System.Net.Mime.MediaTypeNames;
using NLog;
using PROBESimulator.IO.Swagger;
using UFOSimulator.Processors;
using PROBESimulator.Common.Contracts;

namespace UFOSimulator.IO.Controllers
{
    public class TPSController : ApiController
    {
        private readonly Logger _logger = LogManager.GetLogger("tpsLogger");

        public TPSController() { }

        [SwaggerConsumes("application/xml", "application/json")]
        [SwaggerProduces("application/xml", "application/json")]
        [SwaggerResponse(HttpStatusCode.OK, "Response to Valid Request", typeof(NJINTPS))]
        [SwaggerResponse(HttpStatusCode.BadRequest, "Response to Bad Request", typeof(Error))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, "Response to Internal Server Error", typeof(Error))]
        public IHttpActionResult Post(NJINTPS request)
        {
            try
            {
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
                _logger.Trace($"=====> Handling TPS Request | Timestamp: {request.TimeStamp:yyyy-MM-dd HH:mm:ss.fff} | IBCID: {request.IBCID}");
                UFOProcessorNJINTPS.Instance.Process(request);
                _logger.Trace($"<===== Handled");

                // For now, simply log the result.
                /*
                var toLog = Serialization.Serialize(request);
                if (toLog == null)
                {
                    var msg = "TPSController: Unable to serialize the request";
                    _logger.Error(msg);
                    throw new Exception(msg);
                }

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
