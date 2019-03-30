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

        private readonly BinaryMinHeap<CollisionEvent> _eventHeap;
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
            var preFillArraySize = Particles.Length * Particles.Length * 100;
            _eventHeap = BinaryMinHeap<CollisionEvent>.CreateAndPreSize(
                new EventTimeComparer(), preFillArraySize);
            _collisionEventSource = new CollisionEventSource(preFillArraySize);
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
            if (_eventHeap.PeekMin().Time > nowSeconds)
                MoveAllParticles(nowSeconds);

            ProcessEvents(nowSeconds);
        }

        private void ProcessEvents(double nowSeconds)
        {
            while (_eventHeap.PeekMin().Time <= nowSeconds)
            {
                var event_ = _eventHeap.RemoveMin();
                if (!event_.IsValid())
                {
                    _collisionEventSource.Reclaim(event_);
                    continue;
                }

                var a = event_.A;
                var b = event_.B;

                MoveAllParticles(event_.Time);

                if (a != null && b != null) a.bounceOff(b);
                else if (a != null) a.bounceOffVerticalWall();
                else if (b != null) b.bounceOffHorizontalWall();

                EnqueueCollisionTimes(a);
                EnqueueCollisionTimes(b);

                _collisionEventSource.Reclaim(event_);
            }
        }

        private void MoveAllParticles(double nowSeconds)
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
            for (var i = 0; i < Particles.Length; i++)
            {
                var p = Particles[i];
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
            var discardedEvent = _eventHeap.Add(event_);
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