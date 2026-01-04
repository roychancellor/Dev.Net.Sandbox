using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkMessageProcessorModule.DataAccess.DataObjects
{
    public class SourceFileControl : IDataObject
    {
        public long ID { get; set; }
        public DateTime DateStarted { get; set; }
        public DateTime DateFinished { get; set; }
        public long SubjectsInserted { get; set; }
        public long SubjectMKEFieldsInserted { get; set; }

        public bool IsProcessable()
        {
            return true;
        }
    }
}
