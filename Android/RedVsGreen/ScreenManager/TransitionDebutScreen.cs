using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameStateManagement;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace GameStateManagement
{
    class TransitionDebutScreen :GameScreen
    {
        float _timerbl = 1000f;
        float _timerblbl = 1000f;
        bool blbl;

        public TransitionDebutScreen()
        {
            blbl = true;
        }

        public void UpdateAlpha(float timer)
        {
            if (blbl)
            {
                _timerbl -= timer;
                if (_timerbl < 0f)
                {
                    blbl = false;
                }
            }
        }

        public void DrawAlpha(GameScreen gameScreen)
        {
            SpriteBatch _spriteBatch = gameScreen.ScreenManager.SpriteBatch;
            Texture2D blankTexture = gameScreen.ScreenManager.BlankTexture;
            Viewport viewport = gameScreen.ScreenManager.GraphicsDevice.Viewport;
            Rectangle r = new Rectangle(0, 0, viewport.Width, viewport.Height);

            if (blbl)
            {
                _spriteBatch.Draw(blankTexture, r, Color.Black * (_timerbl / _timerblbl));
            }
        }
    }
}
