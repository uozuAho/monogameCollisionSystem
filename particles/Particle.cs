using System;

namespace particles
{

/******************************************************************************
 *
 * Copied and adapted from https://algs4.cs.princeton.edu/61event/Particle.java.html
 * 
 *  A particle moving in the unit box with a given position, velocity,
 *  radius, and mass.
 *
 ******************************************************************************/

/**
 *  The {@code Particle} class represents a particle moving in the unit box,
 *  with a given position, velocity, radius, and mass. Methods are provided
 *  for moving the particle and for predicting and resolvling elastic
 *  collisions with vertical walls, horizontal walls, and other particles.
 *  This data type is mutable because the position and velocity change.
 *  <p>
 *  For additional documentation, 
 *  see <a href="https://algs4.cs.princeton.edu/61event">Section 6.1</a> of 
 *  <i>Algorithms, 4th Edition</i> by Robert Sedgewick and Kevin Wayne. 
 *
 *  @author Robert Sedgewick
 *  @author Kevin Wayne
 */
    public class Particle
    {
        public int NumCollisions { get; private set; } = 0;

        public double posX { get; private set; }
        public double posY { get; private set; }
        private double vx, vy; // velocity
        private readonly double radius; // radius
        private readonly double mass; // mass

        /**
         * Initializes a particle with the specified position, velocity, radius, mass, and color.
         *
         * @param  rx <em>x</em>-coordinate of position
         * @param  ry <em>y</em>-coordinate of position
         * @param  vx <em>x</em>-coordinate of velocity
         * @param  vy <em>y</em>-coordinate of velocity
         * @param  radius the radius
         * @param  mass the mass
         * @param  color the color
         */
        public Particle(double posX, double posY, double vx, double vy, double radius, double mass)
        {
            this.vx = vx;
            this.vy = vy;
            this.posX = posX;
            this.posY = posY;
            this.radius = radius;
            this.mass = mass;
        }

        /**
         * Moves this particle in a straight line (based on its velocity)
         * for the specified amount of time.
         *
         * @param  dt the amount of time
         */
        public void move(double dt)
        {
            posX += vx * dt;
            posY += vy * dt;
        }

        /// <summary>
        /// Time before colliding with given particle, in seconds.
        /// If no collision, double.PositiveInfinity
        /// </summary>
        public double timeToHit(Particle that)
        {
            if (this == that) return double.PositiveInfinity;
            double dx = that.posX - this.posX;
            double dy = that.posY - this.posY;
            double dvx = that.vx - this.vx;
            double dvy = that.vy - this.vy;
            double dvdr = dx * dvx + dy * dvy;
            if (dvdr > 0) return double.PositiveInfinity;
            double dvdv = dvx * dvx + dvy * dvy;
            if (dvdv == 0) return double.PositiveInfinity;
            double drdr = dx * dx + dy * dy;
            double sigma = this.radius + that.radius;
            double d = (dvdr * dvdr) - dvdv * (drdr - sigma * sigma);
            // if (drdr < sigma*sigma) StdOut.println("overlapping particles");
            if (d < 0) return double.PositiveInfinity;
            return -(dvdr + Math.Sqrt(d)) / dvdv;
        }

        public double timeToHitVerticalWall()
        {
            if (vx > 0) return (1.0 - posX - radius) / vx;
            if (vx < 0) return (radius - posX) / vx;
            
            return double.PositiveInfinity;
        }

        public double timeToHitHorizontalWall()
        {
            if (vy > 0) return (1.0 - posY - radius) / vy;
            if (vy < 0) return (radius - posY) / vy;
            
            return double.PositiveInfinity;
        }

        /**
         * Updates the velocities of this particle and the specified particle according
         * to the laws of elastic collision. Assumes that the particles are colliding
         * at this instant.
         *
         * @param  that the other particle
         */
        public void bounceOff(Particle that)
        {
            double dx = that.posX - this.posX;
            double dy = that.posY - this.posY;
            double dvx = that.vx - this.vx;
            double dvy = that.vy - this.vy;
            double dvdr = dx * dvx + dy * dvy; // dv dot dr
            double dist = this.radius + that.radius; // distance between particle centers at collison

            // magnitude of normal force
            double magnitude = 2 * this.mass * that.mass * dvdr / ((this.mass + that.mass) * dist);

            // normal force, and in x and y directions
            double fx = magnitude * dx / dist;
            double fy = magnitude * dy / dist;

            // update velocities according to normal force
            this.vx += fx / this.mass;
            this.vy += fy / this.mass;
            that.vx -= fx / that.mass;
            that.vy -= fy / that.mass;

            // update collision counts
            this.NumCollisions++;
            that.NumCollisions++;
        }

        /**
         * Updates the velocity of this particle upon collision with a vertical
         * wall (by reflecting the velocity in the <em>x</em>-direction).
         * Assumes that the particle is colliding with a vertical wall at this instant.
         */
        public void bounceOffVerticalWall()
        {
            vx = -vx;
            NumCollisions++;
        }

        /**
         * Updates the velocity of this particle upon collision with a horizontal
         * wall (by reflecting the velocity in the <em>y</em>-direction).
         * Assumes that the particle is colliding with a horizontal wall at this instant.
         */
        public void bounceOffHorizontalWall()
        {
            vy = -vy;
            NumCollisions++;
        }
    }
}