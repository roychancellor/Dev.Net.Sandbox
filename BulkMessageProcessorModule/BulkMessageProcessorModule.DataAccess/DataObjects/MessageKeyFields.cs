using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkMessageProcessorModule.DataAccess.DataObjects
{
    public class MessageKeyFields : IDataObject
    {
        public long ID { get; set; }
        public long MessageKeyId { get; set; }
        public string Field { get; set; }

        public bool IsProcessable()
        {
            return true;
        }
    }
}
