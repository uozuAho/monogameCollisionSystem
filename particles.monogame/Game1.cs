using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace particles.monogame
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        private SpriteFont _metricFont;
        private readonly CollisionSystem _collisionSystem;
        private CollisionSystemRenderer _collisionSystemRenderer;
        private readonly TimerMetric _updateMetric = new TimerMetric();
        private readonly TimerMetric _drawMetric = new TimerMetric();
        private readonly FpsMetric _fpsMetric = new FpsMetric();
        private TimerMetricRenderer _updateMetricRenderer;
        private TimerMetricRenderer _drawMetricRenderer;
        private FpsMetricRenderer _fpsMetricRenderer;

        public Game1(CollisionSystem collisionSystem)
        {
            _collisionSystem = collisionSystem;
            
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            var particleTexture = Content.Load<Texture2D>("dot_20x20");
            _metricFont = Content.Load<SpriteFont>("metric");
            _updateMetricRenderer = new TimerMetricRenderer(_metricFont, "update", new Vector2(0, 0));
            _drawMetricRenderer = new TimerMetricRenderer(_metricFont, "draw", new Vector2(0, 20));
            _fpsMetricRenderer = new FpsMetricRenderer(_metricFont, new Vector2(0, 40));
            _collisionSystemRenderer = new CollisionSystemRenderer(_collisionSystem, particleTexture);
        }

        protected override void Update(GameTime gameTime)
        {
            _updateMetric.Begin();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            _collisionSystem.Update(gameTime.TotalGameTime.TotalSeconds);

            base.Update(gameTime);

            _updateMetric.End();
        }

        protected override void Draw(GameTime gameTime)
        {
            _drawMetric.Begin();
            _fpsMetric.OnFrame();

            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            _collisionSystemRenderer.Draw(spriteBatch);
            _updateMetricRenderer.Draw(spriteBatch, _updateMetric);
            _drawMetricRenderer.Draw(spriteBatch, _drawMetric);
            _fpsMetricRenderer.Draw(spriteBatch, _fpsMetric);
            spriteBatch.End();

            base.Draw(gameTime);

            _drawMetric.End();
        }
    }

    internal class FpsMetric
    {
        private readonly long[] _frameTimes = new long[100];
        private readonly Stopwatch _stopwatch;
        private int _idx = 0;

        public FpsMetric()
        {
            _stopwatch = Stopwatch.StartNew();
        }

        public void OnFrame()
        {
            _frameTimes[_idx++] = _stopwatch.ElapsedMilliseconds;
            if (_idx == _frameTimes.Length)
                _idx = 0;
        }

        public double Fps()
        {
            var latestIdx = _idx - 1;
            if (latestIdx < 0) latestIdx = _frameTimes.Length - 1;

            var totalTimeMs = _frameTimes[latestIdx] - _frameTimes[_idx];

            if (totalTimeMs < double.Epsilon) return 0;

            return (_frameTimes.Length * 1000.0) / totalTimeMs;
        }
    }

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

    internal class TimerMetric
    {
        private readonly long[] _measurements = new long[100];
        private int _idx = 0;
        private long _beginMs = 0;
        private readonly Stopwatch _stopwatch;

        public TimerMetric()
        {
            _stopwatch = Stopwatch.StartNew();
        }

        public void Begin()
        {
            _beginMs = _stopwatch.ElapsedMilliseconds;
        }

        public void End()
        {
            _measurements[_idx++] = _stopwatch.ElapsedMilliseconds - _beginMs;
            if (_idx == _measurements.Length)
                _idx = 0;
        }

        public (long, long, double) MinMaxAvg()
        {
            var min = long.MaxValue;
            var max = long.MinValue;
            var sum = 0.0;

            for (var i = 0; i < _measurements.Length; i++)
            {
                var value = _measurements[i];
                if (value < min) min = value;
                if (value > max) max = value;
                sum += value;
            }

            return (min, max, sum / _measurements.Length);
        }
    }

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
