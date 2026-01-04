using BulkMessageProcessorModule.DataAccess.CommandBuilders;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkMessageProcessorModule.DataAccess.DataObjects
{
    public class SourceFile : IDataObject
    {
        public long ID { get; set; }
        public long SourceId { get; set; }
        public string Filename { get; set; }
        public long NumRows { get; set; }

        public SourceFile() { }

        public bool IsProcessable()
        {
            if (string.IsNullOrEmpty(Filename) || NumRows <= 0 || SourceId <= 0)
            {
                return false;
            }
            return true;
        }
    }
}
