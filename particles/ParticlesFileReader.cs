using System.Collections.Generic;
using System.IO;

namespace particles
{
    public static class ParticlesFileReader
    {
        public static IEnumerable<Particle> FromFile(string path)
        {
            using (var file = File.OpenText(path))
            {
                var firstLine = file.ReadLine();
                int numParticles = int.Parse(firstLine);
                
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
                    yield return new Particle(rx, ry, vx, vy, radius, mass);
                }
            }
        }
    }
}