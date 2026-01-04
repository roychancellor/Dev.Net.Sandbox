using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkMessageProcessorModule.Providers
{
    public class ApplicationData : IApplicationData
    {
        public string NJINExpressConnection { get; set; }
        public string SourceFilePath { get; set; }
        public string ProcSourceFileInsert { get; set; }
    }
}
