using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApiWithJSONAndXML.Logic;
using WebApiWithJSONAndXML.Models;
using WebApiWithJSONAndXML.Swagger;

namespace WebApiWithJSONAndXML.Controllers
{
    public class ValidateSchemaController : ApiController
    {
        private ValidationResponse _toReturn;
        private ValidationLogic _logic;

        public ValidateSchemaController() : this(null, null) { }

        public ValidateSchemaController(ValidationResponse toReturn, ValidationLogic logic)
        {
            _toReturn = toReturn ?? new ValidationResponse();
            _logic = logic ?? new ValidationLogic();
        }

        [SwaggerConsumes("application/xml", "application/json")]
        [SwaggerProduces("application/xml", "application/json")]
        [SwaggerResponse(HttpStatusCode.OK, "Response to Valid Request", typeof(ValidationResponse))]
        [SwaggerResponse(HttpStatusCode.BadRequest, "Response to Bad Request", typeof(Error))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, "Response to Internal Server Error", typeof(Error))]
        public IHttpActionResult Post(ValidationRequest request)
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

                var validationFindings = _logic.ValidateMessageBody(request);

                var reportCardText = _logic.BuildReportCard(request, validationFindings);

                _toReturn.IsValid = true;
                _toReturn.ReportCardText = reportCardText;
                _toReturn.ValidationFindings = validationFindings;
                _toReturn.ValidationRequest = request;

                return Ok(_toReturn);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
