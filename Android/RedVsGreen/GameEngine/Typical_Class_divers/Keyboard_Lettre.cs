using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameStateManagement;
using Microsoft.Devices;
using System.IO.IsolatedStorage;

namespace RedVsGreen
{
    class Keyboard_Lettre
    {
        public Keyboard.TypeLettre _type;
        public Texture2D _icone;
        public string _lettre = "";
        public Vector2 _position = Vector2.Zero;
		public Vector2 size;
		private Color _color = new Color (100, 100, 100);
        private Color _color_when_tap = new Color(0, 174, 196);
        public Color _textColor = Color.White;
        public event EventHandler<EventArgs> Tapped;
        SpriteFont font;
		private float scale;

        //Timer tap
        public bool _tap_bool = false;
        public float _tap_timer_max_anim = 70f;
        public float _tap_timer = 0f;
		float Alpha = 1f;

		public Keyboard_Lettre(SpriteFont _font,string lettre, Vector2 position,Keyboard.TypeLettre type, int width, int height, float _scale )
        {
			size = new Vector2 ((float)(width * 0.08), (float)(height * 0.075));

            _type = type;
            font = _font;
            _lettre = lettre;
            _position = position;
			scale = _scale;
        }

		public Keyboard_Lettre(Vector2 _size,Vector2 position, Texture2D icone, Keyboard.TypeLettre type, float _scale)
        {
            _type = type;
            size = _size;
            _position = position;
            _icone = icone;
			scale = _scale;
        }

		public Keyboard_Lettre(Vector2 _size,Vector2 position, SpriteFont _font,string lettre, Keyboard.TypeLettre type, float _scale)
		{
			_type = type;
			size = _size;
			scale = _scale;
			_position = position;
			font = _font;
			_lettre = lettre;
		}

        protected virtual void OnTapped()
        {
            if (Tapped != null)
                Tapped(this, EventArgs.Empty);

            _tap_bool = true;
            _tap_timer = 0f;

        }

		public void Timer_Update(float timer)
		{
			if (_tap_bool) {
				_tap_timer += timer;
				if (_tap_timer > _tap_timer_max_anim) {
					_tap_bool = false;
				}
			}
		}

        public bool HandleTap(Vector2 tap)
        {
            if (tap.X >= _position.X &&
                tap.Y >= _position.Y &&
                tap.X <= _position.X + size.X &&
                tap.Y <= _position.Y + size.Y)
            {
                OnTapped();
                return true;
            }

            return false;
        }

		public void DrawLettre(GameScreen screen)
        {
            // Grab some common items from the ScreenManager
            SpriteBatch spriteBatch = screen.ScreenManager.SpriteBatch;
            Texture2D blank = screen.ScreenManager.BlankTexture;

            // Compute the button's rectangle
            Rectangle r = new Rectangle(
                (int)_position.X,
                (int)_position.Y,
                (int)size.X,
                (int)size.Y);

            // Fill the button
            if (_tap_bool)
            {
                spriteBatch.Draw(blank, r, _color_when_tap);
            }
            else
            {
                spriteBatch.Draw(blank, r, _color * Alpha);
            }

            if (_lettre != "")
            {
                // Draw the text centered in the button
				Vector2 textSize = font.MeasureString(_lettre)*scale;
                Vector2 textPosition = new Vector2(r.Center.X, r.Center.Y) - textSize / 2f;
                textPosition.X = (int)textPosition.X;
                textPosition.Y = (int)textPosition.Y;
				spriteBatch.DrawString(font, _lettre, textPosition, _textColor * Alpha, 0f, Vector2.Zero, scale, SpriteEffects.None, 1f);
            }
            else
            {
				Vector2 textureSize = new Vector2(_icone.Width * scale, _icone.Height * scale);
                Vector2 texturePosition = new Vector2(r.Center.X, r.Center.Y) - textureSize / 2;
                texturePosition.X = (int)texturePosition.X;
                texturePosition.Y = (int)texturePosition.Y;
				spriteBatch.Draw (_icone, texturePosition, new Rectangle (0,0,(int)(_icone.Width), (int)(_icone.Height)), Color.White * Alpha, 0, new Vector2 (0, 0), scale, SpriteEffects.None, 0f);
            }
        }
    }
}
