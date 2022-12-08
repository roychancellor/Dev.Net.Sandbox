using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace LPREventReadTestSubmitter
{
    [XmlRoot(ElementName = "EventRead")]
    public class TestEventRead
    {
        [XmlIgnore]
        public int TestId { get; set; }
        public string SubmitterId { get; set; }
        public string ScannedLPN { get; set; }
        public string EventId { get; set; }

        public override string ToString()
        {
            return $"{nameof(SubmitterId)}: {SubmitterId}\n{nameof(ScannedLPN)}: {ScannedLPN}\n{nameof(EventId)}: {EventId}";
        }
    }
}
