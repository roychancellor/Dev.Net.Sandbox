using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IOSimulator.NSB.OUT.Models
{
    public class TestConfiguration
    {
        public int NumberOfTests { get; set; }
        public int DelayBetweenTestsMS { get; set; }
        public bool VerboseTW { get; set; }
        public string InboundCorrelationIdBase { get; set; }
    }
}
