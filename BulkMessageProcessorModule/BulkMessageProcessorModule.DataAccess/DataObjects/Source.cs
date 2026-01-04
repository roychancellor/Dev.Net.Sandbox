using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkMessageProcessorModule.DataAccess.DataObjects
{
    public class Source : IDataObject
    {
        public long ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }


        public bool IsProcessable()
        {
            return true;
        }
    }
}
