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
        public Particle[] Particles { get; }

        private readonly MinPQ<CollisionEvent> _eventQueue;
        private readonly CollisionEventSource _collisionEventSource;
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
            var maxNumEvents = Particles.Length * Particles.Length * 10;
            _eventQueue = new MinPQ<CollisionEvent>(new EventTimeComparer(), maxNumEvents);
            _collisionEventSource = new CollisionEventSource(maxNumEvents);
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
            if (_eventQueue.Peek().Time > nowSeconds)
            {
                UpdateAllParticles(nowSeconds);
            }

            // handle all events up to current time
            while (_eventQueue.Peek().Time <= nowSeconds)
            {
                var event_ = _eventQueue.Pop();
                if (!event_.IsValid())
                {
                    _collisionEventSource.Reclaim(event_);
                    continue;
                }

                var a = event_.A;
                var b = event_.B;

                UpdateAllParticles(event_.Time);

                if (a != null && b != null) a.bounceOff(b);
                else if (a != null) a.bounceOffVerticalWall();
                else if (b != null) b.bounceOffHorizontalWall();
    
                EnqueueCollisionTimes(a);
                EnqueueCollisionTimes(b);

                _collisionEventSource.Reclaim(event_);
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
                Enqueue(_collisionEventSource.NewEvent(_lastUpdateTime + dt, a, p));
            }

            // particle-wall collisions
            var dtX = a.timeToHitVerticalWall();
            var dtY = a.timeToHitHorizontalWall();
            Enqueue(_collisionEventSource.NewEvent(_lastUpdateTime + dtX, a, null));
            Enqueue(_collisionEventSource.NewEvent(_lastUpdateTime + dtY, null, a));
        }

        private void Enqueue(CollisionEvent event_)
        {
            var discardedEvent = _eventQueue.Push(event_);
            if (discardedEvent != null)
                _collisionEventSource.Reclaim(discardedEvent);
        }

        private class EventTimeComparer : IComparer<CollisionEvent>
        {
            public int Compare(CollisionEvent x, CollisionEvent y)
            {
                return x.Time < y.Time ? -1 : x.Time > y.Time ? 1 : 0;
            }
        }
    }
}