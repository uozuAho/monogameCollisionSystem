using System.Diagnostics;

namespace particles.monogame
{
    internal class FpsMetric
    {
        private readonly long[] _frameTimes = new long[100];
        private readonly Stopwatch _stopwatch;
        private int _idx = 0;

        public FpsMetric()
        {
            _stopwatch = Stopwatch.StartNew();
        }

        public void OnFrame()
        {
            _frameTimes[_idx++] = _stopwatch.ElapsedMilliseconds;
            if (_idx == _frameTimes.Length)
                _idx = 0;
        }

        public double Fps()
        {
            var latestIdx = _idx - 1;
            if (latestIdx < 0) latestIdx = _frameTimes.Length - 1;

            var totalTimeMs = _frameTimes[latestIdx] - _frameTimes[_idx];

            if (totalTimeMs < double.Epsilon) return 0;

            return (_frameTimes.Length * 1000.0) / totalTimeMs;
        }
    }
}