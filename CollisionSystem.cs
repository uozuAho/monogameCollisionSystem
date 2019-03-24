using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

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

        private MinPQ<Event> pq; // the priority queue
        private double t = 0.0; // simulation clock time
        private Particle[] particles; // the array of particles

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

        // updates priority queue with all new events for particle a
        private void predict(Particle a, double limit)
        {
            if (a == null) return;

            // particle-particle collisions
            for (int i = 0; i < particles.Length; i++)
            {
                double dt = a.timeToHit(particles[i]);
                if (t + dt <= limit)
                    pq.Push(new Event(t + dt, a, particles[i]));
            }

            // particle-wall collisions
            double dtX = a.timeToHitVerticalWall();
            double dtY = a.timeToHitHorizontalWall();
            if (t + dtX <= limit) pq.Push(new Event(t + dtX, a, null));
            if (t + dtY <= limit) pq.Push(new Event(t + dtY, null, a));
        }

        // redraw all particles
        private void redraw(double limit)
        {
//        StdDraw.clear();
//        for (int i = 0; i < particles.length; i++) {
//            particles[i].draw();
//        }
//        StdDraw.show();
//        StdDraw.pause(20);
//        if (t < limit) {
//            pq.insert(new Event(t + 1.0 / HZ, null, null));
//        }
        }

        private class EventComparer : IComparer<Event>
        {
            public int Compare(Event x, Event y)
            {
                return x.CompareTo(y);
            }
        }

        /**
         * Simulates the system of particles for the specified amount of time.
         *
         * - predict next collision for all particles, push collisions to queue
         * - push redraw to queue
         * - while queue is not empty
         *     - pop from queue
         *     - update all particles to current time
         *     - recalculate next collisions for event popped off queue, push to queue
         *
         * 
         * @param  limit the amount of time
         */
        public void simulate(double limit)
        {

            // initialize PQ with collision events and redraw event
            pq = new MinPQ<Event>(new EventComparer());
            for (int i = 0; i < particles.Length; i++)
            {
                predict(particles[i], limit);
            }

            pq.Push(new Event(0, null, null)); // redraw event


            // the main event-driven simulation loop
            while (!pq.IsEmpty)
            {

                // get impending event, discard if invalidated
                Event e = pq.Pop();
                if (!e.isValid()) continue;
                Particle a = e.a;
                Particle b = e.b;

                // physical collision, so update positions, and then simulation clock
                for (int i = 0; i < particles.Length; i++)
                    particles[i].move(e.time - t);
                t = e.time;

                // process event
                if (a != null && b != null) a.bounceOff(b); // particle-particle collision
                else if (a != null && b == null) a.bounceOffVerticalWall(); // particle-wall collision
                else if (a == null && b != null) b.bounceOffHorizontalWall(); // particle-wall collision
                else if (a == null && b == null) redraw(limit); // redraw event

                // update the priority queue with new collisions involving a or b
                predict(a, limit);
                predict(b, limit);
            }
        }

        /// <summary>
        /// An event during a particle collision simulation. Each event contains
        /// the time at which it will occur (assuming no supervening actions)
        /// and the particles a and b involved.
        /// </summary>
        /// <remarks>
        ///  - a and b both null:      redraw event
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

        /**
         * Unit tests the {@code CollisionSystem} data type.
         * Reads in the particle collision system from a standard input
         * (or generates {@code N} random particles if a command-line integer
         * is specified); simulates the system.
         *
         * @param args the command-line arguments
         */
        public static void main(String[] args)
        {

//        StdDraw.setCanvasSize(600, 600);

            // enable double buffering
//        StdDraw.enableDoubleBuffering();

            // create collision system and simulate
//        CollisionSystem system = new CollisionSystem(particles);
//        system.simulate(10000);
        }

    }
}