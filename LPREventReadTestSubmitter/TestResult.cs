using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LPREventReadTestSubmitter
{
    public class TestResult
    {
        public string TestId { get; set; }
        public string HttpReturnCode { get; set; }

        public override string ToString()
        {
            return $"{nameof(TestId)}: {TestId}\t{nameof(HttpReturnCode)}: {HttpReturnCode}";
        }
    }
}
