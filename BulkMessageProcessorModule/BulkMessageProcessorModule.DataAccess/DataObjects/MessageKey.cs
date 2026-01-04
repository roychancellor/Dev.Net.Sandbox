using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkMessageProcessorModule.DataAccess.DataObjects
{
    public class MessageKey : IDataObject
    {
        public long ID { get; }
        public string MKE { get; set; }

        public bool IsProcessable()
        {
            return true;
        }
    }
}
