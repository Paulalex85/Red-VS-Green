using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameStateManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameStateManagement
{
    class TransitionScreen : GameScreen
    {
        #region Fields

        bool transitionDone;
        bool transition;

        float _timer = 0f;
        float _timer_max = 500f;

        GameScreen[] screensToLoad;
        GameScreen screenToErrase;

        #endregion

        #region Initialization


        /// <summary>
        /// The constructor is private: loading screens should
        /// be activated via the static Load method instead.
        /// </summary>
        private TransitionScreen(ScreenManager screenManager,GameScreen gamescreen, bool loadingIsSlow,
                              GameScreen[] screensToLoad)
        {
            this.screenToErrase = gamescreen;
            this.screensToLoad = screensToLoad;
            transitionDone = false;
            transition = true;

            TransitionOnTime = TimeSpan.FromSeconds(0.5);
        }


        /// <summary>
        /// Activates the loading screen.
        /// </summary>
        public static void Load(ScreenManager screenManager, GameScreen gamescreen, bool loadingIsSlow,
                                PlayerIndex? controllingPlayer,
                                params GameScreen[] screensToLoad)
        {


            // Create and activate the loading screen.
            TransitionScreen loadingScreen = new TransitionScreen(screenManager,
                                                            gamescreen,
                                                            loadingIsSlow,
                                                            screensToLoad);

            screenManager.AddScreen(loadingScreen);
        }


        #endregion

        #region Update and Draw


        /// <summary>
        /// Updates the loading screen.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            float time = (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            if (transitionDone)
            {
                FinAnimation(time);
            }
            else
            {
                DebutAnimation(time);
            }
        }

        private void DebutAnimation(float time)
        {
            if (_timer < _timer_max)
            {
                _timer += time;
            }
            else if (_timer > _timer_max)
            {

                foreach (GameScreen screen in screensToLoad)
                {
                    if (screen != null)
                    {
                        ScreenManager.AddScreen(screen);
                    }
                }
                // Tell all the current screens to transition off.
                screenToErrase.ExitScreen();

                transitionDone = true;
            }
        }

        private void FinAnimation(float time)
        {
            if (_timer < 0f)
            {
                
                transition = false;
                ScreenManager.RemoveScreen(this);

                // Once the load has finished, we use ResetElapsedTime to tell
                // the  game timing mechanism that we have just finished a very
                // long frame, and that it should not try to catch up.
                ScreenManager.Game.ResetElapsedTime();
            }
            else
            {
                _timer -= time;
            }
        }


        /// <summary>
        /// Draws the loading screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            // The gameplay screen takes a while to load, so we display a loading
            // message while that is going on, but the menus load very quickly, and
            // it would look silly if we flashed this up for just a fraction of a
            // second while returning from the game to the menus. This parameter
            // tells us how long the loading is going to take, so we know whether
            // to bother drawing the message.
            if (transition)
            {
                SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
                SpriteFont font = ScreenManager.Font;
                Texture2D blank = ScreenManager.BlankTexture;

                // Center the text in the viewport.
                Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
                Rectangle r = new Rectangle(0,0,viewport.Width,viewport.Height);

                Color color = Color.Black * (_timer /_timer_max);

                // Draw the text.
                spriteBatch.Begin();
                spriteBatch.Draw(blank, r, color);
                spriteBatch.End();
            }
        }


        #endregion
    }
}
