using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using PROBESimulator.Common.Contracts;
using PROBESimulator.Processors;

namespace UFOSimulator.Processors
{
    public sealed class UFOProcessorNJINTPS : IUFOProcessorTPS
    {
        // The TPSController will call a Process method in this class that takes in a NJINTPS object
        // and computes a rolling average of the TPS and logs it to a file.
        private static readonly UFOProcessorNJINTPS _instance = new UFOProcessorNJINTPS();
        private static int _latestFinishedSecond;
        private static readonly Logger _logger = LogManager.GetLogger("tpsLogger");
        private Dictionary<string, Dictionary<int, int>> _tpsReport;
        private MovingAverage _movingAverage;
        private double _latestMovingAverage;
        private double _latestTPS;
        private DateTime _latestTimeStamp;

        static UFOProcessorNJINTPS() { }
        private UFOProcessorNJINTPS()
        {
            Initialize();
        }

        public static UFOProcessorNJINTPS Instance
        {
            get
            {
                return _instance;
            }
        }

        public Dictionary<string, Dictionary<int, int>> TPSReport
        {
            get
            {
                return _tpsReport;
            }
        }

        public int Current
        {
            get
            {
                return (int)_latestTPS;
                /*
                var now = DateTime.Now;
                var date = now.ToString("yyyyMMdd");
                var ticks = _latestFinishedSecond;
                var current = -1;
                if (_tpsReport != null && _tpsReport.ContainsKey(date) && _tpsReport[date].ContainsKey(ticks))
                {
                    current = _tpsReport[date][ticks];
                }
                return current;
                */
            }
        }

        public void Initialize()
        {
            _latestFinishedSecond = 0;
            _tpsReport = new Dictionary<string, Dictionary<int, int>>();
            _movingAverage = new MovingAverage();
            _latestMovingAverage = 0.0;
            _latestTPS = 0.0;
            _latestTimeStamp = DateTime.Now;
        }

        public void Process(IPROBEData toProcess)
        {
            var thisTimeStamp = toProcess.TimeStamp;
            var deltaT = (thisTimeStamp - _latestTimeStamp).TotalSeconds;
            _latestTimeStamp = thisTimeStamp;
            _latestTPS = 1.0 / deltaT;
            _latestMovingAverage = _movingAverage.ComputeMovingAverage(_latestTPS);

            /*
            var thisDate = toProcess.TimeStamp.ToString("yyyyMMdd");
            var thisSecond = (int)toProcess.TimeStamp.TimeOfDay.TotalSeconds;
            if (_tpsReport.ContainsKey(thisDate))
            {
                // Still on the current day
                if (_tpsReport[thisDate].ContainsKey(thisSecond))
                {
                    // Still in the current second, so increment the transaction count for this second
                    _tpsReport[thisDate][thisSecond]++;
                }
                else
                {
                    // This is a new second

                    // Set the latest finished second to the previous second that has an entry in the dictionary
                    var keys = _tpsReport[thisDate].Keys;
                    _latestFinishedSecond = keys.Count > 0 ? keys.Max() : -1;

                    // Compute the latest moving average of TPS
                    _latestMovingAverage = _movingAverage.ComputeMovingAverage(Current);

                    // Enter the first transaction of the new second
                    _tpsReport[thisDate].Add(thisSecond, 1);
                }

            }
            else
            {
                // A new day has arrived, so start a new dictionary
                _tpsReport.Add(thisDate, new Dictionary<int, int>());
            }
            */

            // In practice, this would be going to a database so the UFO could apply business logic for reporting / alerting.
            // For this simulator, just log what would be going to the database.
            _logger.Info($"UFO TPS Processor: {toProcess.IBCID} | TPS: {_latestTPS:#.##} | MA TPS: {_latestMovingAverage:#.###}");
        }

        public void Reset()
        {
            _latestFinishedSecond = 0;
            _tpsReport = new Dictionary<string, Dictionary<int, int>>();
        }
    }
}
