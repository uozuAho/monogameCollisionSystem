using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace particles.monogame
{
    public class CollisionSystemRenderer
    {
        private readonly CollisionSystem _system;
        private readonly Texture2D _particleTexture;

        public CollisionSystemRenderer(CollisionSystem system, Texture2D particleTexture)
        {
            _system = system;
            _particleTexture = particleTexture;
        }
        
        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var particle in _system.particles)
            {
                DrawParticle(spriteBatch, particle);
            }
        }

        private void DrawParticle(SpriteBatch spriteBatch, Particle particle)
        {
            spriteBatch.Draw(_particleTexture, new Rectangle((int)particle.posX, (int)particle.posY, 20, 20), Color.Red);
        }
    }
}