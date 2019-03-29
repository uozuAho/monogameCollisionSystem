using System.Collections.Generic;

namespace particles
{
    internal class CollisionEventSource
    {
        private readonly List<CollisionEvent> _availableEvents = new List<CollisionEvent>();

        public CollisionEvent NewEvent(double timeSeconds, Particle a, Particle b)
        {
            if (_availableEvents.Count == 0) return new CollisionEvent(timeSeconds, a, b);

            var lastIdx = _availableEvents.Count - 1;
            var event_ = _availableEvents[lastIdx];
            _availableEvents.RemoveAt(lastIdx);

            event_.Init(timeSeconds, a, b);
            return event_;
        }

        public void Reclaim(CollisionEvent e)
        {
            _availableEvents.Add(e);
        }
    }
}
