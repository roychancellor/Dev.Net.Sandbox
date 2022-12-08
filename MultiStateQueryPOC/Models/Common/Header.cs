using Models.Query;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using System.Xml.Serialization;

namespace Models.Common
{
    [DataContract(Namespace = "")]
    public class Header
    {
        public Header() { }

        [DataMember] [Required] public string SendORI { get; set; }
        [DataMember] [Required] public string DestORI { get; set; }
        [DataMember] public string ControlField { get; set; }
        [DataMember] [Required] public string MessageKey { get; set; }

        public override string ToString()
        {
            return $"{{{nameof(SendORI)}={SendORI}, {nameof(DestORI)}={DestORI}, {nameof(ControlField)}={ControlField}, {nameof(MessageKey)}={MessageKey}}}";
        }
    }
}