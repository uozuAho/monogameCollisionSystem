using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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
        private const double HZ = 0.5; // number of redraw events per clock tick

        private MinPQ<Event> _pq; // the priority queue
        private Particle[] particles; // the array of particles
        private TimeSpan _lastEventTime = TimeSpan.Zero;
        private Texture2D _particleTexture;

        /**
         * Initializes a system with the specified collection of particles.
         * The individual particles will be mutated during the simulation.
         *
         * @param  particles the array of particles
         */
        public CollisionSystem(Particle[] particles)
        {
            this.particles = particles.Select(p => p.Clone()).ToArray(); // defensive copy
        }

        public void Init(Texture2D particleTexture)
        {
            _particleTexture = particleTexture;
            
            // initialize PQ with collision events
            _pq = new MinPQ<Event>(new EventComparer());
            foreach (var p in particles)
            {
                predict(p);
            }
        }
        
        public void Update(GameTime now)
        {
            // todo: this assumes particle time is in seconds (looks like it)
            // handle all events up to current time
            while (_pq.Peek().time <= now.ElapsedGameTime)
            {
                var event_ = _pq.Pop();
                if (!event_.isValid()) continue;

                var a = event_.a;
                var b = event_.b;

                // update all particles to current time
                foreach (var p in particles)
                {
                    p.move((event_.time - _lastEventTime).TotalSeconds);
                }

                _lastEventTime = event_.time;

                if (a != null && b != null) a.bounceOff(b);
                else if (a != null) a.bounceOffVerticalWall();
                else if (b != null) b.bounceOffHorizontalWall();
    
                // update the priority queue with new collisions involving a or b
                predict(a);
                predict(b);
                
//                Console.WriteLine(now.ElapsedGameTime.TotalSeconds);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var particle in particles)
            {
                particle.Draw(spriteBatch, _particleTexture);
            }
        }

        // updates priority queue with all new events for particle a
        private void predict(Particle a)
        {
            if (a == null) return;

            // particle-particle collisions
            foreach (var p in particles)
            {
                var dt = a.timeToHit(p);
                _pq.Push(new Event(_lastEventTime + dt, a, p));
            }

            // particle-wall collisions
            var dtX = a.timeToHitVerticalWall();
            var dtY = a.timeToHitHorizontalWall();
            _pq.Push(new Event(_lastEventTime + dtX, a, null));
            _pq.Push(new Event(_lastEventTime + dtY, null, a));
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
            public readonly TimeSpan time; // time that event is scheduled to occur
            public readonly Particle a, b; // particles involved in event, possibly null
            public readonly int countA, countB; // collision counts at event creation

            // create a new event to occur at time t involving a and b
            public Event(TimeSpan t, Particle a, Particle b)
            {
                this.time = t;
                this.a = a;
                this.b = b;
                if (a != null) countA = a.count;
                else countA = -1;
                if (b != null) countB = b.count;
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
                if (a != null && a.count != countA) return false;
                if (b != null && b.count != countB) return false;
                return true;
            }
        }
    }
}