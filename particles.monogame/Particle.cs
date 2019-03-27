using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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

        private static readonly TimeSpan INFINITY = TimeSpan.MaxValue;

        public double posX { get; private set; }
        public double posY { get; private set; }
        private double vx, vy; // velocity
        private readonly double radius; // radius
        private readonly double mass; // mass
        private readonly Color color; // color

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
            this.color = Color.Red;
        }

        public Particle Clone()
        {
            return new Particle(posX, posY, vx, vy, radius, mass);
        }

        /**
         * Initializes a particle with a random position and velocity.
         * The position is uniform in the unit box; the velocity in
         * either direction is chosen uniformly at random.
         */
//        public Particle()
//        {
//        rx     = StdRandom.uniform(0.0, 1.0);
//        ry     = StdRandom.uniform(0.0, 1.0);
//        vx     = StdRandom.uniform(-0.005, 0.005);
//        vy     = StdRandom.uniform(-0.005, 0.005);
//        radius = 0.02;
//        mass   = 0.5;
//        color  = Color.BLACK;
//        }

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

        public void Draw(SpriteBatch spriteBatch, Texture2D texture2D)
        {
            spriteBatch.Draw(texture2D, new Rectangle((int)posX, (int)posY, 20, 20), Color.Red);
        }

        /**
         * Returns the amount of time for this particle to collide with the specified
         * particle, assuming no intervening collisions.
         *
         * @param  that the other particle
         * @return the amount of time for this particle to collide with the specified
         *         particle, assuming no intervening collisions; 
         *         {@code Double.POSITIVE_INFINITY} if the particles will not collide
         */
        public TimeSpan timeToHit(Particle that)
        {
            if (this == that) return INFINITY;
            double dx = that.posX - this.posX;
            double dy = that.posY - this.posY;
            double dvx = that.vx - this.vx;
            double dvy = that.vy - this.vy;
            double dvdr = dx * dvx + dy * dvy;
            if (dvdr > 0) return INFINITY;
            double dvdv = dvx * dvx + dvy * dvy;
            if (dvdv == 0) return INFINITY;
            double drdr = dx * dx + dy * dy;
            double sigma = this.radius + that.radius;
            double d = (dvdr * dvdr) - dvdv * (drdr - sigma * sigma);
            // if (drdr < sigma*sigma) StdOut.println("overlapping particles");
            if (d < 0) return INFINITY;
            return TimeSpan.FromSeconds(-(dvdr + Math.Sqrt(d)) / dvdv);
        }

        /**
         * Returns the amount of time for this particle to collide with a vertical
         * wall, assuming no interening collisions.
         *
         * @return the amount of time for this particle to collide with a vertical wall,
         *         assuming no interening collisions; 
         *         {@code Double.POSITIVE_INFINITY} if the particle will not collide
         *         with a vertical wall
         */
        public TimeSpan timeToHitVerticalWall()
        {
            if (vx > 0) return TimeSpan.FromSeconds((1.0 - posX - radius) / vx);
            else if (vx < 0) return TimeSpan.FromSeconds((radius - posX) / vx);
            else return INFINITY;
        }

        /**
         * Returns the amount of time for this particle to collide with a horizontal
         * wall, assuming no interening collisions.
         *
         * @return the amount of time for this particle to collide with a horizontal wall,
         *         assuming no interening collisions; 
         *         {@code Double.POSITIVE_INFINITY} if the particle will not collide
         *         with a horizontal wall
         */
        public TimeSpan timeToHitHorizontalWall()
        {
            if (vy > 0) return TimeSpan.FromSeconds((1.0 - posY - radius) / vy);
            else if (vy < 0) return TimeSpan.FromSeconds((radius - posY) / vy);
            else return INFINITY;
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

        /**
         * Returns the kinetic energy of this particle.
         * The kinetic energy is given by the formula 1/2 <em>m</em> <em>v</em><sup>2</sup>,
         * where <em>m</em> is the mass of this particle and <em>v</em> is its velocity.
         *
         * @return the kinetic energy of this particle
         */
        public double kineticEnergy()
        {
            return 0.5 * mass * (vx * vx + vy * vy);
        }
    }
}