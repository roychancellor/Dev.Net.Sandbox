using BulkMessageProcessorModule.DataAccess.DataObjects;
using BulkMessageProcessorModule.DataAccess.DbManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkMessageProcessorModule.DataAccess.DataAccessors
{
    public interface IDataAccessor<T> where T : IDataObject
    {
        bool Insert(T toInsert);
        IQueryable<T> GetAll();
        T GetById(long id);
    }
}
