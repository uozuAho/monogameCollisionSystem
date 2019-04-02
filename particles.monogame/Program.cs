using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace particles.monogame
{
    public static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            var particles = args.Length > 0
                ? LoadParticlesFromFile(args[0])
                : GenerateParticles(1000);

            var collisionSystem = new CollisionSystem(particles);
            
            using (var game = new Game1(collisionSystem))
                game.Run();
        }

        private static IEnumerable<Particle> LoadParticlesFromFile(string path)
        {
            return ParticlesFileReader.FromFile(path);
        }

        private static IEnumerable<Particle> GenerateParticles(int numParticles)
        {
            var rng = new Random();
            
            for (var i = 0; i < numParticles; i++)
            {
                yield return new Particle(
                    rng.NextDouble(),
                    rng.NextDouble(),
                    rng.NextDouble() * 0.5,
                    rng.NextDouble() * 0.5,
                    .0005,
                    1
                );
            }
        }
    }
}
