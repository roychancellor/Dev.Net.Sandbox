using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NJINSimulator.Common.Models
{
    public enum NJINServiceNames
    {
        IOInterface,
        RequestManager,
        PipelineOrchestratorIn,
        Validator,
        RouterDestInfo,
        RouterRoute,
        ImageProcessor,
        Conversion,
        PipelineOrchestratorOut,
        SenderManager,
        ErrorProcessor,
        ArchiveTrans,
        ArchiveDelConf,
        UNKNOWN,
    }
}
