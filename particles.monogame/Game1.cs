using System.Collections.Generic;
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
//            _metricFont = Content.Load<SpriteFont>("metric");
//            _updateMetricRenderer = new TimerMetricRenderer(_metricFont, "update", new Vector2(0, 0));
//            _drawMetricRenderer = new TimerMetricRenderer(_metricFont, "draw", new Vector2(0, 20));
//            _fpsMetricRenderer = new FpsMetricRenderer(_metricFont, new Vector2(0, 40));
            _collisionSystemRenderer = new CollisionSystemRenderer(_collisionSystem, particleTexture);
        }

        protected override void Update(GameTime gameTime)
        {
//            _updateMetric.Begin();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            _collisionSystem.Update(gameTime.TotalGameTime.TotalSeconds);

            base.Update(gameTime);

//            _updateMetric.End();
        }

        protected override void Draw(GameTime gameTime)
        {
//            _drawMetric.Begin();
//            _fpsMetric.OnFrame();

            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            _collisionSystemRenderer.Draw(spriteBatch);
//            _updateMetricRenderer.Draw(spriteBatch, _updateMetric);
//            _drawMetricRenderer.Draw(spriteBatch, _drawMetric);
//            _fpsMetricRenderer.Draw(spriteBatch, _fpsMetric);
            spriteBatch.End();

            base.Draw(gameTime);

            _drawMetric.End();
        }
    }
}
