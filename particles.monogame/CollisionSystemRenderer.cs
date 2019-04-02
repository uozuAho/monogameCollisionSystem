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
            foreach (var particle in _system.Particles)
            {
                DrawParticle(spriteBatch, particle);
            }
        }

        private void DrawParticle(SpriteBatch spriteBatch, Particle particle)
        {
            var width = spriteBatch.GraphicsDevice.Viewport.Width;
            var height = spriteBatch.GraphicsDevice.Viewport.Height;
            var box = new Rectangle((int) (particle.posX * width), (int) (particle.posY * height), 2, 2);
            spriteBatch.Draw(_particleTexture, box, Color.Red);
        }
    }
}