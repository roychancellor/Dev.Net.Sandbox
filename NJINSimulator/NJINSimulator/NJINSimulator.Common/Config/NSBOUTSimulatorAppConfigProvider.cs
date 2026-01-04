using NJINSimulator.Common.Models;
using NJINSimulator.Common.Utilities;
using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NJINSimulator.Common.Config
{
    public sealed class NSBOUTSimulatorAppConfigProvider : IConfigProvider
    {
        private static readonly NSBOUTSimulatorAppConfigProvider _instance = new NSBOUTSimulatorAppConfigProvider();
        private readonly NSBOUTConfig _nsbOutConfig;

        static NSBOUTSimulatorAppConfigProvider()
        {
        }

        private NSBOUTSimulatorAppConfigProvider()
        {
            _nsbOutConfig = new NSBOUTConfig();
            Refresh();
        }

        public NSBOUTConfig NsbOutConfig
        {
            get
            {
                if (_nsbOutConfig == null)
                {
                    Refresh();
                }
                return _nsbOutConfig;
            }
        }

        public static NSBOUTSimulatorAppConfigProvider Instance
        {
            get
            {
                return _instance;
            }
        }

        public void Refresh()
        {
            _nsbOutConfig.IOInterfaceURL = Parse(ConfigurationManager.AppSettings[nameof(NSBOUTConfig.IOInterfaceURL)]);
            _nsbOutConfig.TestConfigPath = Parse(ConfigurationManager.AppSettings[nameof(NSBOUTConfig.TestConfigPath)]);
        }

        private string Parse(string toParse)
        {
            return toParse.IsNullOrEmpty() ? null : toParse;
        }

        public string PrintConfig()
        {
            return$"{nameof(NSBOUTConfig.IOInterfaceURL)}: {_nsbOutConfig?.IOInterfaceURL} | {nameof(NSBOUTConfig.TestConfigPath)}: {_nsbOutConfig?.TestConfigPath}";
        }
    }
}
