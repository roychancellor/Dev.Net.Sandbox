using MultiStateQueryPOC.Models.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace MultiStateQueryPOC.Models.Response
{
    [DataContract(Namespace = "")]
    public class DR
    {
        public DR()
        {
            Header = new Header();
        }

        [DataMember]
        [Required]
        public Header Header { get; set; }
        [DataMember]
        [Required]
        public string ResponseBody { get; set; }

        public override string ToString()
        {
            return $"{{{nameof(Header)}={Header}, {nameof(ResponseBody)}={ResponseBody}}}";
        }
    }
}