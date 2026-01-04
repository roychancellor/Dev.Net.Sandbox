using BulkMessageProcessorModule.DataAccess.CommandBuilders;
using BulkMessageProcessorModule.DataAccess.DataObjects;
using BulkMessageProcessorModule.DataAccess.DbManager;
using BulkMessageProcessorModule.Providers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Linq;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Configuration;
using System.Text;
using System.Threading.Tasks;

namespace BulkMessageProcessorModule.DataAccess.DataAccessors
{
    public class SourceFileDataAccessor : IDataAccessor<SourceFile>
    {
        private ApplicationData _appData;
        private ICommandBuilder _insertCommandBuilder;

        public SourceFileDataAccessor() : this(null) { }

        public SourceFileDataAccessor(ICommandBuilder insertCommandBuilder)
        {
            _appData = SingletonAppConfigProvider.Instance.Get();
            _insertCommandBuilder = insertCommandBuilder ?? new SourceFileInsertCommandBuilder();
        }

        public IQueryable<SourceFile> GetAll()
        {
            throw new NotImplementedException();
        }

        public SourceFile GetById(long id)
        {
            throw new NotImplementedException();
        }

        public bool Insert(SourceFile dataObject)
        {
            if (dataObject == null)
            {
                return default;
            }

            var sourceFile = dataObject as SourceFile;
            if (!sourceFile.IsProcessable())
            {
                return default;
            }

            try
            {
                var sqlMgr = SingletonSqlServerDbManager.Instance;
                var conn = sqlMgr.Connection(_appData.NJINExpressConnection);
                var cmd = _insertCommandBuilder.Build(conn, _appData.ProcSourceFileInsert, sourceFile);
                if (cmd == null)
                {
                    return default;
                }

                var isSuccessful = sqlMgr.ExecuteScalar(conn, cmd, out object result);
                var sourceFileId = (long)result;
                if (!isSuccessful || sourceFileId < 0)
                {
                    return default;
                }
                sourceFile.ID = sourceFileId;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return true;
        }
    }
}
