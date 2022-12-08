using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Serialization;

namespace WebApiWithJSONAndXML.Models
{
    public class ValidationResponse
    {
        [XmlElement(Order = 1)]
        [JsonProperty(Order = 1)]
        public ValidationRequest ValidationRequest { get; set; }

        [XmlElement(Order = 2)]
        [JsonProperty(Order = 2)]
        public bool IsValid { get; set; }

        [XmlElement(Order = 3)]
        [JsonProperty(Order = 3)]
        public List<ValidationFinding> ValidationFindings { get; set; }

        [XmlElement(Order = 4)]
        [JsonProperty(Order = 4)]
        public string ReportCardText { get; set; }
    }
}