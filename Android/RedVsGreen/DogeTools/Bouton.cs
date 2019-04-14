using System;
using GameStateManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RedVsGreen
{
	public class Bouton
	{
		GameScreen _screen;
		Divers_Method _divers = new Divers_Method ();
		RoundedRectangle rectangle = new RoundedRectangle ();
		Rectangle _textBox;
		SpriteFont _font;
		string _texte;
		int _marge, _padding;
		Color _color_texte, _color_box;
		Texture2D bouton;
		float _alpha = 1f;
		float _scale = 1f;
		public bool _bouton_tapped = false;

		string[] text_format;

		public Bouton (GameScreen screen, Rectangle textBox, SpriteFont font, string texte, int marge, int padding_texte, Color color_texte, Color color_box, float scale, float transparence)
		{
			_screen = screen;
			_textBox = textBox;
			_font = font;
			_texte = texte;
			_marge = marge;
			_padding = padding_texte;
			_color_box = color_box;
			_color_texte = color_texte;
			_alpha = transparence;
			_scale = scale;
			Initialize ();
		}

		public Bouton (GameScreen screen, Rectangle textBox, SpriteFont font, string texte, int marge, int padding_texte, Color color_texte, Color color_box, float scale)
		{
			_screen = screen;
			_textBox = textBox;
			_font = font;
			_texte = texte;
			_marge = marge;
			_padding = padding_texte;
			_color_box = color_box;
			_color_texte = color_texte;
			_scale = scale;
			Initialize ();
		}

		private void Initialize()
		{
			int arrondi = (int)((_textBox.Height + _textBox.Width) / 2 * 0.05);
			bouton = rectangle.Texture_Rounded_Rectangle (_screen.ScreenManager.GraphicsDevice, _textBox.Width, _textBox.Height, _color_box,arrondi,arrondi);
			text_format = _divers.Text_In_Button_Center (_texte, _font, new Vector2(_textBox.Width,_textBox.Height), _marge, _scale);
		}

		public void Change_color(Color color)
		{
			int arrondi = (int)((_textBox.Height + _textBox.Width) / 2 * 0.05);
			bouton = rectangle.Texture_Rounded_Rectangle (_screen.ScreenManager.GraphicsDevice, _textBox.Width, _textBox.Height, color, arrondi, arrondi);
			text_format = _divers.Text_In_Button_Center (_texte, _font, new Vector2(_textBox.Width,_textBox.Height), _marge, _scale);
		}

		public bool Input(Vector2 tap)
		{
			if (tap.X >= _textBox.X &&
				tap.Y >= _textBox.Y &&
				tap.X <= _textBox.X + _textBox.Width &&
				tap.Y <= _textBox.Y + _textBox.Height)
			{
				_bouton_tapped = true;
				return true;
			}

			return false;
		}

		public void Draw(float transition_alpha)
		{
			if (!_bouton_tapped) {
				_screen.ScreenManager.SpriteBatch.Draw (bouton, new Vector2 (_textBox.X, _textBox.Y), _color_box * transition_alpha);
			} else {
				_screen.ScreenManager.SpriteBatch.Draw (bouton, new Vector2 (_textBox.X, _textBox.Y), _color_box*0.8f * transition_alpha);
			}

			int nbr_ligne = text_format.Length;
			for (int i = 0; i < nbr_ligne; i++) {
				Vector2 position;
				int taille_height = (int)(_font.MeasureString("GORGE").Y * _scale);

				/*if (nbr_ligne == text_format.Length) {
					position = new Vector2 (_textBox.X + _textBox.Width / 2 - _font.MeasureString (text_format [i]).X / 2, _textBox.Y + _marge + ((_font.MeasureString (text_format [i]).Y + _padding) * i));
				} else */

				if (nbr_ligne % 2 != 0) {
					int milieu = (int)(text_format.Length / 2);
					if (i < milieu) {
						position = new Vector2 (_textBox.X + _textBox.Width / 2 - (_font.MeasureString (text_format [i].ToUpper()).X * _scale) / 2, _textBox.Y + _textBox.Height / 2 - (_font.MeasureString (text_format [milieu].ToUpper()).Y * _scale) / 2 - (((_font.MeasureString (text_format [i]).Y* _scale) + _padding) * (milieu - i)));
					} else if (i == milieu) {
						position = new Vector2 (_textBox.X + _textBox.Width / 2 - (_font.MeasureString (text_format [i].ToUpper()).X * _scale) / 2, _textBox.Y + _textBox.Height / 2 - ((_font.MeasureString (text_format [i].ToUpper()).Y * _scale) + _padding) / 2);
					} else {
						position = new Vector2 (_textBox.X + _textBox.Width / 2 - (_font.MeasureString (text_format [i].ToUpper()).X * _scale)/ 2, _textBox.Y + _textBox.Height / 2  + (_font.MeasureString (text_format [milieu].ToUpper()).Y * _scale) / 2 + (((_font.MeasureString (text_format [i]).Y * _scale) + _padding) * (i-milieu)));
					}

				} else {
					int milieu = (int)(text_format.Length / 2);
					if (i < milieu) {
						position = new Vector2 (_textBox.X + _textBox.Width / 2 - (_font.MeasureString (text_format [i].ToUpper()).X * _scale) / 2, _textBox.Y + _textBox.Height / 2 - (((_font.MeasureString (text_format [i].ToUpper()).Y * _scale)+ _padding) * (milieu - i)));
					} else {
						position = new Vector2 (_textBox.X + _textBox.Width / 2 - (_font.MeasureString (text_format [i].ToUpper()).X * _scale) / 2, _textBox.Y + _textBox.Height / 2 + (((_font.MeasureString (text_format [i].ToUpper()).Y * _scale)+ _padding) * (milieu - i)));
					}
				}
				_screen.ScreenManager.SpriteBatch.DrawString (_font, text_format [i].ToUpper (), position, _color_texte * _alpha * transition_alpha, 0f, Vector2.Zero, _scale, SpriteEffects.None, 1f);
			}
		}

		public void Draw()
		{
			if (!_bouton_tapped) {
				_screen.ScreenManager.SpriteBatch.Draw (bouton, new Vector2 (_textBox.X, _textBox.Y), _color_box);
			} else {
				_screen.ScreenManager.SpriteBatch.Draw (bouton, new Vector2 (_textBox.X, _textBox.Y), _color_box*0.8f);
			}

			int nbr_ligne = text_format.Length;
			for (int i = 0; i < nbr_ligne; i++) {
				Vector2 position;
				int taille_height = (int)(_font.MeasureString("GORGE").Y * _scale);

				/*if (nbr_ligne == text_format.Length) {
					position = new Vector2 (_textBox.X + _textBox.Width / 2 - _font.MeasureString (text_format [i]).X / 2, _textBox.Y + _marge + ((_font.MeasureString (text_format [i]).Y + _padding) * i));
				} else */

				if (nbr_ligne % 2 != 0) {
					int milieu = (int)(text_format.Length / 2);
					if (i < milieu) {
						position = new Vector2 (_textBox.X + _textBox.Width / 2 - (_font.MeasureString (text_format [i].ToUpper()).X * _scale) / 2, _textBox.Y + _textBox.Height / 2 - (_font.MeasureString (text_format [milieu].ToUpper()).Y * _scale)/ 2 - (((_font.MeasureString (text_format [i]).Y * _scale) + _padding) * (milieu - i)));
					} else if (i == milieu) {
						position = new Vector2 (_textBox.X + _textBox.Width / 2 - ( _font.MeasureString (text_format [i].ToUpper()).X * _scale) / 2, _textBox.Y + _textBox.Height / 2 - ((_font.MeasureString (text_format [i].ToUpper()).Y * _scale) + _padding) / 2);
					} else {
						position = new Vector2 (_textBox.X + _textBox.Width / 2 - (_font.MeasureString (text_format [i].ToUpper()).X * _scale ) / 2, _textBox.Y + _textBox.Height / 2  + (_font.MeasureString (text_format [milieu].ToUpper()).Y * _scale)/ 2 + (((_font.MeasureString (text_format [i]).Y * _scale)+ _padding) * (milieu - i)));
					}

				} else {
					int milieu = (int)(text_format.Length / 2);
					if (i < milieu) {
						position = new Vector2 (_textBox.X + _textBox.Width / 2 - (_font.MeasureString (text_format [i].ToUpper()).X * _scale) / 2, _textBox.Y + _textBox.Height / 2 - (((_font.MeasureString (text_format [i].ToUpper()).Y * _scale) + _padding) * (milieu - i)));
					} else {
						position = new Vector2 (_textBox.X + _textBox.Width / 2 - (_font.MeasureString (text_format [i].ToUpper()).X * _scale) / 2, _textBox.Y + _textBox.Height / 2 + (((_font.MeasureString (text_format [i].ToUpper()).Y * _scale) + _padding) * (milieu - i)));
					}
				}
				_screen.ScreenManager.SpriteBatch.DrawString (_font, text_format [i].ToUpper (), position, _color_texte * _alpha, 0f, Vector2.Zero, _scale, SpriteEffects.None, 1f);
			}
		}
	}
}