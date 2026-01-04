using DotNetCoreWebApi.Logic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using NLog;

namespace DotNetCoreWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MotorcycleController(IRequestLogic requestLogic) : ControllerBase
    {
        private static readonly NLog.ILogger _reqLog = LogManager.GetLogger("RequestLogger");

        [HttpPost]
        public async Task<IActionResult> InboundRequest()
        {
            _reqLog.Info("=====> REQUEST RECEIVED");

            if (requestLogic == null)
            {
                _reqLog.Error("The injected RequestLogic object is null. Returning 500.");
                return StatusCode(500, "Internal server error: Unable to process request");
            }

            // Handle the request
            _reqLog.Trace("Calling request handler");
            HandleResponse handleResponse;
            try
            {
                handleResponse = await requestLogic.HandleRequest();
            }
            catch (BadRequestException brex)
            {
                _reqLog.Error($"HandleRequest threw an exception. Returning 400.\n{brex}");
                return BadRequest(brex.Message);
            }

            // Process the request using the URL in the request XML
            _reqLog.Trace("Calling request processor");
            string aPIResponseBody;
            try
            {
                aPIResponseBody = await requestLogic.ProcessRequest(handleResponse);
            }
            catch (Exception suex)
            {
                _reqLog.Error($"HandleRequest threw an exception. Returning 400.\n{suex}");
                return StatusCode(503, suex.Message);
            }
            
            _reqLog.Info("<===== RESPONSE SENT");

            return Ok(aPIResponseBody);
        }
    }
}
