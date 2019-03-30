using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace particles.monogame
{
    internal class FpsMetricRenderer
    {
        private readonly SpriteFont _font;
        private readonly Vector2 _pos;

        public FpsMetricRenderer(SpriteFont font, Vector2 pos)
        {
            _font = font;
            _pos = pos;
        }

        public void Draw(SpriteBatch spriteBatch, FpsMetric metric)
        {
            var fps = metric.Fps();
            spriteBatch.DrawString(_font, $"fps: {fps:0.##}", _pos, Color.Red);
        }
    }
}