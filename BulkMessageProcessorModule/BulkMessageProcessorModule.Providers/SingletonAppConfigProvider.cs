using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkMessageProcessorModule.Providers
{
    public sealed class SingletonAppConfigProvider
    {
        private static readonly IConfigProvider _instance = new AppConfigProvider();
        private static IConfigProvider _injectedInstance;

        static SingletonAppConfigProvider()
        {
        }
        private SingletonAppConfigProvider()
        {
        }
        public static IConfigProvider Instance
        {
            get
            {
                if (_injectedInstance != null)
                {
                    return (AppConfigProvider)_injectedInstance;
                }
                return _instance;
            }
        }

        public static void SetInstance(IConfigProvider toSet)
        {
            _injectedInstance = toSet;
        }
    }
}
