using System;

namespace particles.console
{
    class Program
    {
        static void Main(string[] args)
        {
            var path = args[0];
            var particles = ParticlesFileReader.FromFile(path);
            var collisionSystem = new CollisionSystem(particles);

            for (var millis = 0; ; millis += 500)
            {
                collisionSystem.Update(TimeSpan.FromMilliseconds(millis));
            }
        }
    }
}
