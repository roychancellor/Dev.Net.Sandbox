using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using System.Xml.Serialization;

namespace NJINSimulator.Common.Models
{
    [Serializable, DataContract] // The DataContract attribute helps Swagger not display k_BackingField
    public class TransactionalWrapper
    {
        public TransactionalWrapper()
        {
            ProcessResults = ProcessResults ?? new List<ProcessResult>();
        }

        [Required]
        [XmlElement(Order = 1)]
        [DataMember]
        public bool Verbose { get; set; }

        [Required]
        [XmlElement(Order = 2)]
        [DataMember]
        public string InboundCorrelationID { get; set; }

        [XmlArray(Order = 3)]
        [DataMember]
        public List<ProcessResult> ProcessResults { get; set; }

        [XmlElement(Order = 4)]
        [DataMember]
        public string StartDateTime { get; set; }

        [XmlElement(Order = 5)]
        [DataMember]
        public string FinishDateTime { get; set; }

        [XmlElement(Order = 6)]
        [DataMember]
        public int ElapsedTimeMilliseconds { get; set; }
    }

    [Serializable, DataContract]
    public class ProcessResult
    {
        [XmlText]
        [DataMember]
        public string Result { get; set; }
    }
}