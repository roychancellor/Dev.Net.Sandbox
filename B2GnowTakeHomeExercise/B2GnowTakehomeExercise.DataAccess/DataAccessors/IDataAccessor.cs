using B2GnowTakehomeExercise.DataAccess.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace B2GnowTakehomeExercise.DataAccess.DataAccessors
{
    public interface IDataAccessor<T> where T : IDataObject
    {
        InsertResult Insert(T toInsert);
        IEnumerable<T> GetAll();
        T GetById(int id);
    }
}
