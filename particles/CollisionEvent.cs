using System.Resources;

namespace particles
{
    /// <summary>
    /// An event during a particle collision simulation. Each event contains
    /// the time at which it will occur (assuming no supervening actions)
    /// and the particles a and b involved.
    /// </summary>
    /// <remarks>
    ///  - a null, b not null:     collision with vertical wall
    ///  - a not null, b null:     collision with horizontal wall
    ///  - a and b both not null:  binary collision between a and b
    /// </remarks>
    internal class CollisionEvent
    {
        public double Time { get; private set; }
        public Particle A { get; private set; }
        public Particle B { get; private set; }
        private int _countA; // collision counts at event creation
        private int _countB; // collision counts at event creation

        public CollisionEvent(double timeSeconds, Particle a, Particle b)
        {
            Init(timeSeconds, a, b);
        }

        public void Init(double timeSeconds, Particle a, Particle b)
        {
            Time = timeSeconds;
            A = a;
            B = b;
            if (a != null) _countA = a.NumCollisions;
            else _countA = -1;
            if (b != null) _countB = b.NumCollisions;
            else _countB = -1;
        }

        /// <summary>
        /// False if either particle in the event has collided since this event was created
        /// </summary>
        public bool IsValid()
        {
            if (A != null && A.NumCollisions != _countA) return false;
            if (B != null && B.NumCollisions != _countB) return false;
            return true;
        }
    }
}
