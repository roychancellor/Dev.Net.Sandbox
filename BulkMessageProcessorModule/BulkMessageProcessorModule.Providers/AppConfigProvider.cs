using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkMessageProcessorModule.Providers
{
    public class AppConfigProvider : IConfigProvider
    {
        public ApplicationData AppData { get; set; }

        public AppConfigProvider()
        {
            AppData = new ApplicationData();
            Refresh();
        }

        public ApplicationData Get()
        {
            return AppData;
        }
        
        public void Refresh()
        {
            var strConnectionString = ConfigurationManager.ConnectionStrings[nameof(AppData.NJINExpressConnection)].ConnectionString;
            var strSourceFilePath = ConfigurationManager.AppSettings[nameof(AppData.SourceFilePath)];
            var strProcSourceFileInsert = ConfigurationManager.AppSettings[nameof(AppData.ProcSourceFileInsert)];

            if (!string.IsNullOrEmpty(strConnectionString))
            {
                AppData.NJINExpressConnection = strConnectionString;
            }
            if (!string.IsNullOrEmpty(strSourceFilePath))
            {
                AppData.SourceFilePath = strSourceFilePath;
            }
            if (!string.IsNullOrEmpty(strProcSourceFileInsert))
            {
                AppData.ProcSourceFileInsert = strProcSourceFileInsert;
            }
        }
    }
}
