using Swashbuckle.Swagger.Annotations;
using System;
using System.Linq;
using System.Net;
using System.Web.Http;
using NJINSimulator.Common.Models;
using NJINSimulator.IO.Swagger;
using NJINSimulator.Modules;
using static System.Net.Mime.MediaTypeNames;
using NLog;
using NJINSimulator.Common.Utilities;

namespace NJINSimulator.IO.Controllers
{
    public class IOInterfaceController : ApiController
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public IOInterfaceController()
        {
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

                request.StartDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                if (_logger.IsTraceEnabled) _logger.Trace($">>> IOInterface | IBCID: {request.InboundCorrelationID} | S: {request.StartDateTime}");
                if (!_logger.IsTraceEnabled) _logger.Info($"====> IOInterface | IBCID: {request.InboundCorrelationID}");
                _logger.Trace($">>> Getting the njinService from NJINServicesIOSingleton.Instance.NjinServices[NJINServiceNames.IOInterface]");
                var njinService = NJINServicesIOSingleton.Instance.NjinServices.SafeRetrieve(NJINServiceNames.IOInterface);
                _logger.Trace($"<<< {njinService?.ServiceName}: S: {njinService?.ReceiveFromSource} | D: {njinService?.ProduceToDestination} | Delay: {njinService?.ProcessTimeMilliseconds}");
                njinService.Handle(request);

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
