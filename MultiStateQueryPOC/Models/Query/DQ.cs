using Models.Common;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Models.Query
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