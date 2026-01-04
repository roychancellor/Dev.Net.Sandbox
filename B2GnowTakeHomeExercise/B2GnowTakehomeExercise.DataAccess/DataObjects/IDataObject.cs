using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace B2GnowTakehomeExercise.DataAccess.DataObjects
{
    public interface IDataObject
    {
        long ID { get; }
        bool IsProcessable();
    }
}
