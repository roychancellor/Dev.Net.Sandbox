using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using System.Xml.Serialization;

namespace WebApiWithJSONAndXML.Models
{
    public class ValidationRequest
    {
        public ValidationRequest()
        {

        }

        [Required]
        [MinLength(2)]
        [MaxLength(3)]
        [RegularExpression("[A-Z]+", ErrorMessage = "Message Key may include letters A-Z only")]
        public string MessageKey { get; set; }

        [Required]
        public string MessageBody { get; set; }

        [Required]
        [MinLength(4, ErrorMessage = "Minimum length for Header Format is 4 characters, A-Z and 0-9")]
        [MaxLength(8, ErrorMessage = "Maximum length for Header Format is 8 characters, A-Z and 0-9")]
        [RegularExpression("[A-Z0-9]+", ErrorMessage = "Header Format only allows A-Z and 0-9")]
        public string HeaderFormat { get; set; }

        [Required]
        [MinLength(4, ErrorMessage = "Minimum length for Body Format is 4 characters, A-Z and 0-9")]
        [MaxLength(8, ErrorMessage = "Maximum length for Body Format is 8 characters, A-Z and 0-9")]
        [RegularExpression("[A-Z0-9]+", ErrorMessage = "Body Format only allows A-Z and 0-9")]
        public string BodyFormat { get; set; }

        [Required]
        public string InboundCorrelationID { get; set; }
    }
}