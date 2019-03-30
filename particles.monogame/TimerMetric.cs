using System.Diagnostics;

namespace particles.monogame
{
    internal class TimerMetric
    {
        private readonly long[] _measurements = new long[100];
        private int _idx = 0;
        private long _beginMs = 0;
        private readonly Stopwatch _stopwatch;

        public TimerMetric()
        {
            _stopwatch = Stopwatch.StartNew();
        }

        public void Begin()
        {
            _beginMs = _stopwatch.ElapsedMilliseconds;
        }

        public void End()
        {
            _measurements[_idx++] = _stopwatch.ElapsedMilliseconds - _beginMs;
            if (_idx == _measurements.Length)
                _idx = 0;
        }

        public (long, long, double) MinMaxAvg()
        {
            var min = long.MaxValue;
            var max = long.MinValue;
            var sum = 0.0;

            for (var i = 0; i < _measurements.Length; i++)
            {
                var value = _measurements[i];
                if (value < min) min = value;
                if (value > max) max = value;
                sum += value;
            }

            return (min, max, sum / _measurements.Length);
        }
    }
}