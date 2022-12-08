using MultiStateQueryPOC.Models.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace MultiStateQueryPOC.Models.Query
{
    [DataContract(Namespace = "")]
    public class DQ
    {
        public DQ()
        {
            Header = new Header();
            InquiryData = new DQInquiryData();
        }

        [DataMember]
        [Required]
        public Header Header { get; set; }
        [DataMember]
        [Required]
        public DQInquiryData InquiryData { get; set; }

        public override string ToString()
        {
            return $"{{{nameof(Header)}={Header}, {nameof(InquiryData)}={InquiryData}}}";
        }
    }
}