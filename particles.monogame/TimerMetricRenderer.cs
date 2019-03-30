using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace particles.monogame
{
    internal class TimerMetricRenderer
    {
        private readonly SpriteFont _font;
        private readonly string _label;
        private readonly Vector2 _pos;

        public TimerMetricRenderer(SpriteFont font, string label, Vector2 pos)
        {
            _font = font;
            _label = label;
            _pos = pos;
        }

        public void Draw(SpriteBatch spriteBatch, TimerMetric metric)
        {
            var (min, max, avg) = metric.MinMaxAvg();
            spriteBatch.DrawString(_font, $"{_label}: {min}, {max}, {avg}", _pos, Color.Red);
        }
    }
}