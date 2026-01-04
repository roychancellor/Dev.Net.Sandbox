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
    public sealed class NJINSimulatorAppConfigProvider : IConfigProvider
    {
        private static readonly NJINSimulatorAppConfigProvider _instance = new NJINSimulatorAppConfigProvider();
        private readonly Dictionary<NJINServiceNames, NJINServiceConfig> _njinConfig;

        static NJINSimulatorAppConfigProvider()
        {
        }

        private NJINSimulatorAppConfigProvider()
        {
            _njinConfig = new Dictionary<NJINServiceNames, NJINServiceConfig>();
            Refresh();
        }

        public Dictionary<NJINServiceNames, NJINServiceConfig> NjinConfig
        {
            get
            {
                if (_njinConfig.Count == 0)
                {
                    Refresh();
                }
                return _njinConfig;
            }
        }

        public static NJINSimulatorAppConfigProvider Instance
        {
            get
            {
                return _instance;
            }
        }

        public void Refresh()
        {
            _njinConfig.AddOrUpdate(NJINServiceNames.IOInterface, Parse(ConfigurationManager.AppSettings[NJINServiceNames.IOInterface.ToString()]));
            _njinConfig.AddOrUpdate(NJINServiceNames.RequestManager, Parse(ConfigurationManager.AppSettings[NJINServiceNames.RequestManager.ToString()]));
            _njinConfig.AddOrUpdate(NJINServiceNames.PipelineOrchestratorIn, Parse(ConfigurationManager.AppSettings[NJINServiceNames.PipelineOrchestratorIn.ToString()]));
            _njinConfig.AddOrUpdate(NJINServiceNames.Validator, Parse(ConfigurationManager.AppSettings[NJINServiceNames.Validator.ToString()]));
            _njinConfig.AddOrUpdate(NJINServiceNames.RouterDestInfo, Parse(ConfigurationManager.AppSettings[NJINServiceNames.RouterDestInfo.ToString()]));
            _njinConfig.AddOrUpdate(NJINServiceNames.RouterRoute, Parse(ConfigurationManager.AppSettings[NJINServiceNames.RouterRoute.ToString()]));
            _njinConfig.AddOrUpdate(NJINServiceNames.ImageProcessor, Parse(ConfigurationManager.AppSettings[NJINServiceNames.ImageProcessor.ToString()]));
            _njinConfig.AddOrUpdate(NJINServiceNames.Conversion, Parse(ConfigurationManager.AppSettings[NJINServiceNames.Conversion.ToString()]));
            _njinConfig.AddOrUpdate(NJINServiceNames.PipelineOrchestratorOut, Parse(ConfigurationManager.AppSettings[NJINServiceNames.PipelineOrchestratorOut.ToString()]));
            _njinConfig.AddOrUpdate(NJINServiceNames.SenderManager, Parse(ConfigurationManager.AppSettings[NJINServiceNames.SenderManager.ToString()]));
        }

        private NJINServiceConfig Parse(string toParse)
        {
            if (toParse.IsNullOrEmpty()) return null;

            var parts = toParse.Split('|');
            if (parts.Length != 3) return null;

            var procTimeParsed = int.TryParse(parts[2], out int procTime);
            var toReturn = new NJINServiceConfig
            {
                Source = parts[0],
                Destination = parts[1],
                ProcessingTime = procTimeParsed ? procTime : 50,
            };
            return toReturn;
        }

        public string PrintConfig()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var key in _njinConfig.Keys)
            {
                sb.AppendLine($"{key}: S: {_njinConfig?.SafeRetrieve(key)?.Source} | D: {_njinConfig?.SafeRetrieve(key)?.Destination} | Delay: {_njinConfig?.SafeRetrieve(key)?.ProcessingTime}");
            }
            return sb.ToString();
        }
    }
}
