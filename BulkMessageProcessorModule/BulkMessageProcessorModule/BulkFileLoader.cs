using BulkMessageProcessorModule.DataAccess.CommandBuilders;
using BulkMessageProcessorModule.DataAccess.DataAccessors;
using BulkMessageProcessorModule.DataAccess.DataObjects;
using BulkMessageProcessorModule.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkMessageProcessorModule
{
    public class BulkFileLoader : ILoader
    {
        private SourceFile _sourceFile;
        private IDataAccessor<SourceFile> _sourceFileDataAccessor;

        public BulkFileLoader() : this(null) { }

        public BulkFileLoader(SourceFile sourceFile) : this(sourceFile, null) { }

        public BulkFileLoader(SourceFile sourceFile, IDataAccessor<SourceFile> dataAccessor)
        {
            _sourceFile = sourceFile ?? new SourceFile();
            _sourceFileDataAccessor = dataAccessor ?? new SourceFileDataAccessor();
        }

        public bool Load()
        {
            _sourceFile.SourceId = 1;
            _sourceFile.Filename = "FileName2.csv";
            _sourceFile.NumRows = 3;
            
            var result = _sourceFileDataAccessor.Insert(_sourceFile);
            return result;
        }
    }
}
