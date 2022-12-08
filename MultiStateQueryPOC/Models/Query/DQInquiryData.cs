using System.Runtime.Serialization;

namespace Models.Query
{
    [DataContract(Namespace = "")]
    public class DQInquiryData
    {
        public DQInquiryData()
        {

        }
        
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string DOB { get; set; }
        [DataMember]
        public string Sex { get; set; }
        [DataMember]
        public string OLN { get; set; }
        [DataMember]
        public string OLS { get; set; }

        public override string ToString()
        {
            return $"{{{nameof(Name)}={Name}, {nameof(DOB)}={DOB}, {nameof(Sex)}={Sex}, {nameof(OLN)}={OLN}, {nameof(OLS)}={OLS}}}";
        }
    }
}