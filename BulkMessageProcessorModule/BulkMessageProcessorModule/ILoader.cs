using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkMessageProcessorModule
{
    public interface ILoader
    {
        bool Load();
    }
}
