using System;
using System.Collections.Generic;
using System.Linq;

namespace particles
{
 /******************************************************************************
  *
  * copied and adapted from https://algs4.cs.princeton.edu/61event/CollisionSystem.java
  * 
 *  Creates n random particles and simulates their motion according
 *  to the laws of elastic collisions.
 *
 ******************************************************************************/

/**
 *  The {@code CollisionSystem} class represents a collection of particles
 *  moving in the unit box, according to the laws of elastic collision.
 *  This event-based simulation relies on a priority queue.
 *  <p>
 *  For additional documentation, 
 *  see <a href="https://algs4.cs.princeton.edu/61event">Section 6.1</a> of 
 *  <i>Algorithms, 4th Edition</i> by Robert Sedgewick and Kevin Wayne. 
 *
 *  @author Robert Sedgewick
 *  @author Kevin Wayne
 */

    public class CollisionSystem
    {
        private readonly MinPQ<CollisionEvent> _pq;
        public Particle[] Particles { get; }
        private double _lastUpdateTime;

        /**
         * Initializes a system with the specified collection of particles.
         * The individual particles will be mutated during the simulation.
         *
         * @param  particles the array of particles
         */
        public CollisionSystem(IEnumerable<Particle> particles)
        {
            Particles = particles.ToArray();
            _pq = new MinPQ<CollisionEvent>(new EventTimeComparer());
            PredictAllParticles();
        }

        private void PredictAllParticles()
        {
            foreach (var p in Particles)
            {
                EnqueueCollisionTimes(p);
            }
        }
        
        public void Update(double nowSeconds)
        {
            if (_pq.Peek().Time > nowSeconds)
            {
                UpdateAllParticles(nowSeconds);
            }

            // handle all events up to current time
            while (_pq.Peek().Time <= nowSeconds)
            {
                var event_ = _pq.Pop();
                if (!event_.IsValid()) continue;

                var a = event_.A;
                var b = event_.B;

                UpdateAllParticles(event_.Time);

                if (a != null && b != null) a.bounceOff(b);
                else if (a != null) a.bounceOffVerticalWall();
                else if (b != null) b.bounceOffHorizontalWall();
    
                EnqueueCollisionTimes(a);
                EnqueueCollisionTimes(b);
            }
        }

        private void UpdateAllParticles(double nowSeconds)
        {
            foreach (var p in Particles)
            {
                p.move(nowSeconds - _lastUpdateTime);
            }

            _lastUpdateTime = nowSeconds;
        }

        private void EnqueueCollisionTimes(Particle a)
        {
            if (a == null) return;

            // particle-particle collisions
            foreach (var p in Particles)
            {
                var dt = a.timeToHit(p);
                _pq.Push(new CollisionEvent(_lastUpdateTime + dt, a, p));
            }

            // particle-wall collisions
            var dtX = a.timeToHitVerticalWall();
            var dtY = a.timeToHitHorizontalWall();
            _pq.Push(new CollisionEvent(_lastUpdateTime + dtX, a, null));
            _pq.Push(new CollisionEvent(_lastUpdateTime + dtY, null, a));
        }

        private class EventTimeComparer : IComparer<CollisionEvent>
        {
            public int Compare(CollisionEvent x, CollisionEvent y)
            {
                return x.Time < y.Time ? -1 : x.Time > y.Time ? 1 : 0;
            }
        }

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
        private class CollisionEvent
        {
            public readonly double Time;
            public readonly Particle A, B;
            private readonly int _countA; // collision counts at event creation
            private readonly int _countB; // collision counts at event creation

            public CollisionEvent(double timeSeconds, Particle a, Particle b)
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
}