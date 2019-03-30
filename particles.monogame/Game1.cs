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

            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            _collisionSystemRenderer.Draw(spriteBatch);
            spriteBatch.DrawString(_metricFont, "hello!", new Vector2(100, 100), Color.Black);
            spriteBatch.End();

            base.Draw(gameTime);

            _drawMetric.End();
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
}
