using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiWithJSONAndXML.Models;

namespace WebApiWithJSONAndXML.Logic
{
    public interface IValidationLogic
    {
        string BuildReportCard(ValidationRequest request, List<ValidationFinding> validationFindings = null);
        List<ValidationFinding> ValidateMessageBody(ValidationRequest request);
    }
}
