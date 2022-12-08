using Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Models.Query
{
    [DataContract(Namespace = "")]
    public class NLETS
    {
        [DataMember] public Header Header { get; set; }
        public InquiryData InquiryData { get; set; }
    }
}
