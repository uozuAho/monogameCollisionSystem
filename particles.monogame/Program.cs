using System;
using System.IO;
using System.Linq;

namespace particles.monogame
{
    public static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Console.WriteLine(string.Join(",", args));
            
            var collisionSystem = InitCollisionSystem(args);
            
            Console.WriteLine("starting game");
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
            var path = args[0];
            return ParticlesFileReader.FromFile(path).ToArray();
        }
    }
}
