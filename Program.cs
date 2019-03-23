using System;
using System.IO;
using Microsoft.Xna.Framework;

namespace particles
{
    public static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Console.WriteLine(string.Join(",", args));
            
            var collisionSystem = InitCollisionSystem(args);
            
            using (var game = new Game1(collisionSystem))
                game.Run();
        }

        private static CollisionSystem InitCollisionSystem(string[] args)
        {
            var particles = GetParticlesFromArgs(args);
            
            return new CollisionSystem(particles);
        }

        private static Particle[] GetParticlesFromArgs(string[] args)
        {
            Particle[] particles;

            var path = args[0];

            using (var file = File.OpenText(path))
            {
                var firstLine = file.ReadLine();
                int numParticles = int.Parse(firstLine);
                particles = new Particle[numParticles];
                
                for (int i = 0; i < numParticles; i++)
                {
                    var line = file.ReadLine();
                    var values = line.Trim().Split(' ');
                    double rx = double.Parse(values[0]);
                    double ry = double.Parse(values[1]);
                    double vx = double.Parse(values[2]);
                    double vy = double.Parse(values[3]);
                    double radius = double.Parse(values[4]);
                    double mass = double.Parse(values[5]);
                    int r = int.Parse(values[6]);
                    int g = int.Parse(values[7]);
                    int b = int.Parse(values[8]);
                    Color color = new Color(r, g, b);
                    particles[i] = new Particle(rx, ry, vx, vy, radius, mass, color);
                }
            }

            return particles;
        }
    }
}
