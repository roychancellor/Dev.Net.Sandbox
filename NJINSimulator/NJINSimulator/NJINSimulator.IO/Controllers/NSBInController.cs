using NJINSimulator.Common.Logic;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using NJINSimulator.Common.Models;
using NJINSimulator.IO.Swagger;
using NLog;
using NJINSimulator.Common.Serialization;

namespace NJINSimulator.IO.Controllers
{
    public class NSBInController : ApiController
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public NSBInController()
        {
            _logger.Trace("In NSBInController constructor");
        }

        [SwaggerConsumes("application/xml", "application/json")]
        [SwaggerProduces("application/xml", "application/json")]
        [SwaggerResponse(HttpStatusCode.OK, "Response to Valid Request", typeof(TransactionalWrapper))]
        [SwaggerResponse(HttpStatusCode.BadRequest, "Response to Bad Request", typeof(Error))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, "Response to Internal Server Error", typeof(Error))]
        public IHttpActionResult Post(TransactionalWrapper request)
        {
            try
            {
                _logger.Trace("NSBInController: Request received.");
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

                // For now, simply log the result.
                var toLog = Serialization.Serialize(request);
                if (toLog == null)
                {
                    var msg = "NSBInController: Unable to serialize the TW";
                    _logger.Error(msg);
                    throw new Exception(msg);
                }

                if (_logger.IsTraceEnabled) _logger.Trace($"<<< NSB.IN | IBCID: {request.InboundCorrelationID} | S: {request.StartDateTime} | F: {request.FinishDateTime} | ET, ms: {request.ElapsedTimeMilliseconds}");
                if (!_logger.IsTraceEnabled) _logger.Info($"<==== NSB.IN | IBCID: {request.InboundCorrelationID}");
                _logger.Trace($"Request:\n{toLog}");

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
