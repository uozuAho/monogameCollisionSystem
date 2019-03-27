using NUnit.Framework;

namespace particles.test
{
    public class ParticleTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ShouldMoveAtXSpeed()
        {
            const int xSpeed = 1;
            var particle = new Particle(0, 0, xSpeed, 0, .1, .1);
            
            particle.move(1);
            
            Assert.AreEqual(1, particle.posX);
        }
    }
}