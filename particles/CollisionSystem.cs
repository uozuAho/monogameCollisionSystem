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
        private readonly MinPQ<Event> _pq;
        public Particle[] Particles { get; }
        private double _lastUpdateTime = 0;

        /**
         * Initializes a system with the specified collection of particles.
         * The individual particles will be mutated during the simulation.
         *
         * @param  particles the array of particles
         */
        public CollisionSystem(IEnumerable<Particle> particles)
        {
            Particles = particles.ToArray();
            _pq = new MinPQ<Event>(new EventComparer());
            PredictAllParticles();
        }

        private void PredictAllParticles()
        {
            foreach (var p in Particles)
            {
                predict(p);
            }
        }
        
        public void Update(double nowSeconds)
        {
            if (_pq.Peek().time > nowSeconds)
            {
                UpdateAllParticles(nowSeconds);
            }

            // handle all events up to current time
            while (_pq.Peek().time <= nowSeconds)
            {
                var event_ = _pq.Pop();
                if (!event_.isValid()) continue;

                var a = event_.a;
                var b = event_.b;

                UpdateAllParticles(event_.time);

                if (a != null && b != null) a.bounceOff(b);
                else if (a != null) a.bounceOffVerticalWall();
                else if (b != null) b.bounceOffHorizontalWall();
    
                // update the priority queue with new collisions involving a or b
                predict(a);
                predict(b);
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

        // updates priority queue with all new events for particle a
        private void predict(Particle a)
        {
            if (a == null) return;

            // particle-particle collisions
            foreach (var p in Particles)
            {
                var dt = a.timeToHit(p);
                _pq.Push(new Event(_lastUpdateTime + dt, a, p));
            }

            // particle-wall collisions
            var dtX = a.timeToHitVerticalWall();
            var dtY = a.timeToHitHorizontalWall();
            _pq.Push(new Event(_lastUpdateTime + dtX, a, null));
            _pq.Push(new Event(_lastUpdateTime + dtY, null, a));
        }

        private class EventComparer : IComparer<Event>
        {
            public int Compare(Event x, Event y)
            {
                return x.CompareTo(y);
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
        private class Event : IComparable<Event>
        {
            public readonly double time; // time that event is scheduled to occur
            public readonly Particle a, b; // particles involved in event, possibly null
            public readonly int countA, countB; // collision counts at event creation

            // create a new event to occur at time t involving a and b
            public Event(double t, Particle a, Particle b)
            {
                time = t;
                this.a = a;
                this.b = b;
                if (a != null) countA = a.NumCollisions;
                else countA = -1;
                if (b != null) countB = b.NumCollisions;
                else countB = -1;
            }

            // compare times when two events will occur
            public int CompareTo(Event that)
            {
                return time < that.time ? -1 : time > that.time ? 1 : 0;
            }

            // has any collision occurred between when event was created and now?
            public bool isValid()
            {
                if (a != null && a.NumCollisions != countA) return false;
                if (b != null && b.NumCollisions != countB) return false;
                return true;
            }
        }
    }
}