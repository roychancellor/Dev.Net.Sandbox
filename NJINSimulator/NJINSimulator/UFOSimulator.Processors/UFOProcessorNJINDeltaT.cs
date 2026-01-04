using NJINSimulator.Common.Utilities;
using NLog;
using PROBESimulator.Common.Contracts;
using PROBESimulator.Processors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace UFOSimulator.Processors
{
    public sealed class UFOProcessorNJINDeltaT : IUFOProcessorDeltaT
    {
        // The DeltaTController will call a Process method in this class that takes in a NJINDeltaT object
        // and computes a rolling average of the DeltaT and logs it to a file.
        private static readonly Logger _logger = LogManager.GetLogger("deltaTLogger");
        private static readonly UFOProcessorNJINDeltaT _instance = new UFOProcessorNJINDeltaT();
        private Dictionary<string, DateTime> _inTimestamps;
        private MovingAverage _movingAverage;

        static UFOProcessorNJINDeltaT() { }
        private UFOProcessorNJINDeltaT()
        {
            Initialize();
        }

        public static UFOProcessorNJINDeltaT Instance
        {
            get
            {
                return _instance;
            }
        }

        public int Current
        {
            // This will give the most current moving average of DeltaT.
            get
            {
                var current = -1;
                return current;
            }
        }

        public void Initialize()
        {
            _inTimestamps = new Dictionary<string, DateTime>();
            _movingAverage = new MovingAverage(10);
        }

        public void Process(IPROBEDataDeltaT toProcess)
        {
            if (toProcess == null || toProcess.IBCID.IsNullOrEmpty() || toProcess.Direction.IsNullOrEmpty())
            {
                _logger.Warn($"UFOProcessorNJINDeltaT: Received null/empty request data.");
                return;
            }
            // Determine the direction. IN --> add to dictionary; OUT --> Search for matching IN IBCID and, if found, compute Delta T
            var ibcid = toProcess.IBCID;
            if (toProcess.Direction.ToUpper().Equals("IN"))
            {
                if (_inTimestamps.ContainsKey(ibcid))
                {
                    _logger.Warn($"Duplicate IBCID | IN found in dictionary for: '{ibcid}'");
                    return;
                }
                _inTimestamps.Add(ibcid, toProcess.TimeStamp);
            }
            if (toProcess.Direction.ToUpper().Equals("OUT"))
            {
                if (!_inTimestamps.ContainsKey(ibcid))
                {
                    _logger.Warn($"No matching IBCID | IN found in dictionary for: '{ibcid}'");
                    return;
                }
                var deltaT = (toProcess.TimeStamp - _inTimestamps[ibcid]).Duration().TotalSeconds;

                // Get the moving average of Delta T
                var movingAverage = _movingAverage.ComputeMovingAverage(deltaT);
                
                // In practice, this would be going to a database so the UFO could apply business logic for reporting / alerting.
                // For this simulator, just log what would be going to the database.
                _logger.Info($"UFO DeltaT Processor: {ibcid} | DT, s: {deltaT:#.###} | MA DT, s: {movingAverage:#.###}");

                // Remove the IBCID from the dictionary since the transaction is complete.
                _inTimestamps.Remove(ibcid);
            }
        }
        public void Reset()
        {
            _inTimestamps = new Dictionary<string, DateTime>();
        }
    }
}
