using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkMessageProcessorModule.DataAccess.DataObjects
{
    public class SubjectMessageKeyFields : IDataObject
    {
        public long ID { get; }
        public Guid SubjectId { get; set; }
        public long MessageKeyFieldsId { get; set; }
        public string Value { get; set; }

        public bool IsProcessable()
        {
            return true;
        }
    }
}
