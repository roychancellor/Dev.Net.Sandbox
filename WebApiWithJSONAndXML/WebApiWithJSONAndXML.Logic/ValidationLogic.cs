using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiWithJSONAndXML.Models;

namespace WebApiWithJSONAndXML.Logic
{
    public class ValidationLogic : IValidationLogic
    {
        public string BuildReportCard(ValidationRequest request, List<ValidationFinding> validationFindings = null)
        {
            var findings = validationFindings ?? new List<ValidationFinding>();
            
            try
            {
                if (string.IsNullOrEmpty(request.MessageKey) || string.IsNullOrEmpty(request.MessageBody))
                {
                    throw new Exception("Error building the report card: The input MKE and/or message was null or empty.");
                }
                
                StringBuilder sb = new StringBuilder();

                sb.AppendLine("\n====== REQUEST ======");
                sb.AppendLine($"Inbound Correlation ID: {request.InboundCorrelationID}");
                sb.AppendLine($"Message Key: {request.MessageKey}");
                sb.AppendLine($"Message Body: {request.MessageBody}");
                sb.AppendLine($"Header Format: {request.HeaderFormat}");
                sb.AppendLine($"Body Format: {request.BodyFormat}");
                
                sb.AppendLine($"\n====== REPORT CARD ======");
                if (findings.Count == 0)
                {
                    sb.AppendLine("The message is VALID to the schema.");
                }
                else
                {
                    foreach (var f in findings)
                    {
                        sb.AppendLine($"Severity: {f.Severity} (Frequency: {f.Frequency}) | Message: {f.Message}");
                    }
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<ValidationFinding> ValidateMessageBody(ValidationRequest request)
        {
            if (request == null || request.MessageBody.IsNullOrEmpty())
            {
                return null;
            }
            
            var toReturn = new List<ValidationFinding>();
            if (request.MessageBody.Contains("INVALID"))
            {
                toReturn.Add(new ValidationFinding { Severity = "Error", Frequency = 1, Message = "The XML is not valid for this reason" });
                toReturn.Add(new ValidationFinding { Severity = "Error", Frequency = 3, Message = "The XML is not valid for another reason" });
                toReturn.Add(new ValidationFinding { Severity = "Warning", Frequency = 2, Message = "The XML is not valid for yet another reason" });
            }
            return toReturn;
        }
    }
}
