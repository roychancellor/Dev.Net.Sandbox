using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PROBESimulator.Processors
{
    public class MovingAverage
    {
        public int MovingAverageSize { get; set; }

        private Queue<double> _queue;
        public Queue<double> Queue { get => _queue; }
        
        private double _runningSum;
        public double RunningSum { get => _runningSum; }

        public MovingAverage() : this(0, null) { }
        public MovingAverage(int movingAverageSize) : this(movingAverageSize, null) { }
        public MovingAverage(int movingAverageSize, Queue<double> queue)
        {
            MovingAverageSize = movingAverageSize == 0 ? 10 : movingAverageSize;
            _queue = queue ?? new Queue<double>();
            _runningSum = 0.0;
        }

        public double ComputeMovingAverage(double toAdd)
        {
            if (_queue == null) return double.MinValue;

            var firstIn = 0.0;
            if (_queue.Count >= MovingAverageSize)
            {
                firstIn = _queue.Dequeue();
            }
            _queue.Enqueue(toAdd);
            _runningSum += (toAdd - firstIn);

            return _runningSum / MovingAverageSize;
        }

        public void Reset()
        {
            _runningSum = 0.0;
            _queue = new Queue<double>();
        }
    }
}
