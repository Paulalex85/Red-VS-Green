using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameStateManagement;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO.IsolatedStorage;

namespace GameStateManagement
{
    class PhoneMenuScreen : GameScreen
    {
        List<Button> menuButtons = new List<Button>();
        string menuTitle;
        Texture2D logoTitle;

        string theme = (string)IsolatedStorageSettings.ApplicationSettings["theme"];


        /// <summary>
        /// Gets the list of buttons, so derived classes can add or change the menu contents.
        /// </summary>
        protected IList<Button> MenuButtons
        {
            get { return menuButtons; }
        }

        /// <summary>
        /// Creates the PhoneMenuScreen with a particular title.
        /// </summary>
        /// <param name="title">The title of the screen</param>
        public PhoneMenuScreen(string title)
        {
            menuTitle = title;


            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            // We need tap gestures to hit the buttons
            EnabledGestures = GestureType.Tap;
        }

        public PhoneMenuScreen()
        {

            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            // We need tap gestures to hit the buttons
            EnabledGestures = GestureType.Tap;
        }

        public override void LoadContent()
        {
            float y = 140f;
            float center = 480 / 2;
            for (int i = 0; i < MenuButtons.Count; i++)
            {
                Button b = MenuButtons[i];
                if (b.Position == Vector2.Zero)
                {
                    b.Position = new Vector2(center - b.Size.X / 2, y);
                    y += b.Size.Y * 1.5f;
                }
            }

            base.LoadContent();
        }


        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            // Update opacity of the buttons
            foreach (Button b in menuButtons)
            {
                b.Alpha = TransitionAlpha;
            }

            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        /// <summary>
        /// Handler for when the user has cancelled the menu.
        /// </summary>
        protected virtual void OnCancel(PlayerIndex playerIndex)
        {
            ExitScreen();
        }

        public override void HandleInput(InputState input)
        {
            // Test for the menuCancel action
            PlayerIndex player;
            if (input.IsNewButtonPress(Buttons.Back, ControllingPlayer, out player))
            {
                OnCancel(player);
            }

            // Read in our gestures
            foreach (GestureSample gesture in input.Gestures)
            {
                // If we have a tap
                if (gesture.GestureType == GestureType.Tap)
                {
                    // Test the tap against the buttons until one of the buttons handles the tap
                    foreach (Button b in menuButtons)
                    {
                        if (b.HandleTap(gesture.Position))
                            break;
                    }
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice graphics = ScreenManager.GraphicsDevice;
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            SpriteFont font = ScreenManager.Font;


            if (menuTitle == null)
            {
                logoTitle = ScreenManager.LogoTexture;
            }

            spriteBatch.Begin();

            // Draw all of the buttons
            foreach (Button b in menuButtons)
                b.Draw(this);

            // Make the menu slide into place during transitions, using a
            // power curve to make things look more interesting (this makes
            // the movement slow down as it nears the end).
            float transitionOffset = (float)Math.Pow(TransitionPosition, 2);

            // Draw the menu title centered on the screen
            Vector2 titlePosition = new Vector2(graphics.Viewport.Width / 2, 50);
            Vector2 logoPosition = Vector2.Zero;
            Vector2 titleOrigin = Vector2.Zero;
            if (menuTitle == null)
            {
                titleOrigin = new Vector2(logoTitle.Width / 2, logoTitle.Height / 2);
                logoPosition = titlePosition - titleOrigin;
            }
            else
            {
                titleOrigin = font.MeasureString(menuTitle) / 2;
            }
            Color titleColor = new Color(0, 0, 0) * TransitionAlpha;
            if (theme == "classic")
            {
                titleColor = new Color(255, 255, 255) * TransitionAlpha;
            }
            else if (theme == "tuiles")
            {
                titleColor = new Color(0, 0, 0) * TransitionAlpha;
            }

            float titleScale = 1.25f;

            titlePosition.Y -= transitionOffset * 100;

            if (menuTitle == null)
            {
                spriteBatch.Draw(logoTitle, logoPosition, Color.White * TransitionAlpha);
            }
            else
            {
                spriteBatch.DrawString(font, menuTitle, titlePosition, titleColor, 0,
                                       titleOrigin, titleScale, SpriteEffects.None, 0);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
