using System;
using GameStateManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Input;
using System.IO.IsolatedStorage;

namespace RedVsGreen
{
	public class SelectionScreen : GameScreen
	{
		private enum Annimation{
			Attente,
			Phase_1,
			Phase_2,
			Phase_3
		}

		Annimation annim_statut = Annimation.Attente;

		SpriteFont font;
		Police_Size_Manage font_manage;
		int width, height, largeur_barre_black;
		Rectangle red, green, black;

		Color color_green = new Color(71,164,71),
		color_red = new Color(246,94,59),
		color_fond = new Color(250,248,239);

		string side_final ="";
		string red_string = "Red", green_string = "Green", side_string ;
		bool _option = false;
		bool _annim_red = false, _annim_green = false;

		float _alpha_block = 1f, _alpha_texte = 1f;
		Languages langue = new Languages();

		Compteur_Time _timer_annimation;
		bool _blbl = false;

		public SelectionScreen (bool option)
		{
			_option = option;
			EnabledGestures = GestureType.Tap;
		}

		public override void LoadContent ()
		{
			width = ScreenManager.GraphicsDevice.Viewport.Width;
			height = ScreenManager.GraphicsDevice.Viewport.Height;

			font_manage = new Police_Size_Manage (height, width, this);
			side_string = langue.getString (45);

			font = font_manage.Get_Bold_Gros_Font ();

			largeur_barre_black = 4;

			black = new Rectangle (width / 2 - largeur_barre_black / 2, 0, largeur_barre_black, height);
			red = new Rectangle (0, 0, width / 2 - (black.Width / 2), height);
			green = new Rectangle (width / 2, 0, width / 2, height);
			base.LoadContent ();
		}

		public override void UnloadContent ()
		{
			base.UnloadContent ();
		}

		public override void HandleInput (InputState input)
		{
			foreach (GestureSample gesture in input.Gestures) {
				if (gesture.GestureType == GestureType.Tap) {
					if (gesture.Position.X > red.X &&
						gesture.Position.X < red.X + red.Width &&
						gesture.Position.Y > red.Y &&
						gesture.Position.Y < red.Y + red.Height) {
						_annim_red = true;
						side_final = red_string;
					}
					if (gesture.Position.X > green.X &&
						gesture.Position.X < green.X + green.Width &&
						gesture.Position.Y > green.Y &&
						gesture.Position.Y < green.Y + green.Height) {
						_annim_green = true;
						side_final = green_string;
					}
				}
			}
			base.HandleInput (input);
		}

		private void Side_Choose()
		{
			if (_annim_green) {
				IsolatedStorageSettings.ApplicationSettings ["side"] = "0";
			} else {
				IsolatedStorageSettings.ApplicationSettings ["side"] = "1";
			}
			this.ExitScreen ();
		}

		public override void Update (GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
		{
			if (GamePad.GetState (PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
				ScreenManager.Game.Exit ();

			float timer = (float)gameTime.ElapsedGameTime.TotalMilliseconds;

			Annim (timer);

			if (!_blbl) {
				if (!_option) {
					ScreenManager.AddScreen (new Name_Screen(false));
				} else {
					ScreenManager.AddScreen (new MainMenuScreen());
				}

				ScreenManager.ReverseScreen ();
				_blbl = true;
			}
			base.Update (gameTime, otherScreenHasFocus, coveredByOtherScreen);
		}

		void Annim(float timer)
		{
			if (_annim_red || _annim_green) {
				if (annim_statut == Annimation.Attente) {
					annim_statut = Annimation.Phase_1;
					_timer_annimation = new Compteur_Time (700f);
				} else if (annim_statut == Annimation.Phase_1) {
					if (_timer_annimation.IncreaseTimer (timer)) {
						if (_annim_red) {
							red = new Rectangle (0, 0, width, height);
							green = new Rectangle (0, 0, 0, 0);
							black = new Rectangle (0, 0, 0, 0);
						} else {
							green = new Rectangle (0, 0, width, height);
							red = new Rectangle (0, 0, 0, 0);
							black = new Rectangle (0, 0, 0, 0);
						}
						annim_statut = Annimation.Phase_2;
						_timer_annimation = new Compteur_Time (500f);
					} else {
						if (_annim_red) {
							red.Width = (int)((width / 2) + (_timer_annimation._timer / _timer_annimation._timer_max * (width / 2)));
						} else {
							green.Width = (int)((width / 2) + (_timer_annimation._timer / _timer_annimation._timer_max * (width / 2)));
							green.X = (int)((width / 2) - (_timer_annimation._timer / _timer_annimation._timer_max * (width / 2)));
						}

						_alpha_texte = 1 - (_timer_annimation._timer / (_timer_annimation._timer_max / 2));
					}
				}
				else if (annim_statut == Annimation.Phase_2) {
					if (_timer_annimation.IncreaseTimer (timer)) {
						annim_statut = Annimation.Phase_3;
						_timer_annimation = new Compteur_Time (1500f);
					} else {
						_alpha_texte = _timer_annimation._timer * _timer_annimation._timer_max;
					}
				}
				else if (annim_statut == Annimation.Phase_3) {
					if (_timer_annimation.IncreaseTimer (timer)) {
						_timer_annimation = new Compteur_Time (1000f);
						Side_Choose ();
					}
				}
			}
		}

		public override void Draw (GameTime gameTime)
		{
			Texture2D blank = ScreenManager.BlankTexture;

			ScreenManager.SpriteBatch.Begin ();

				ScreenManager.SpriteBatch.Draw (blank, new Rectangle (0, 0, width, height), color_fond);
			

			if (annim_statut == Annimation.Attente) {
				ScreenManager.SpriteBatch.Draw (blank, red, color_red);
				ScreenManager.SpriteBatch.Draw (blank, green, color_green);
				ScreenManager.SpriteBatch.Draw (blank, black, Color.Black);
				ScreenManager.SpriteBatch.DrawString (font, side_string, new Vector2 ((float)(width / 2 - font.MeasureString (side_string).X * font_manage._scale / 2), (float)(height * 0.2)), Color.White, 0f, Vector2.Zero, font_manage._scale, SpriteEffects.None, 1f);
				ScreenManager.SpriteBatch.DrawString (font, red_string, new Vector2 ((float)(width * 0.25 - font.MeasureString (red_string).X * font_manage._scale / 2), (float)(height * 0.4)), Color.White, 0f, Vector2.Zero, font_manage._scale, SpriteEffects.None, 1f);
				ScreenManager.SpriteBatch.DrawString (font, green_string, new Vector2 ((float)(width * 0.75 - font.MeasureString (green_string).X * font_manage._scale / 2), (float)(height * 0.4)), Color.White, 0f, Vector2.Zero, font_manage._scale, SpriteEffects.None, 1f);
			} else if (annim_statut == Annimation.Phase_1) {
				if (_annim_red) {
					ScreenManager.SpriteBatch.Draw (blank, green, color_green);
					ScreenManager.SpriteBatch.Draw (blank, black, Color.Black);
					ScreenManager.SpriteBatch.Draw (blank, red, color_red);
				} else {
					ScreenManager.SpriteBatch.Draw (blank, red, color_red);
					ScreenManager.SpriteBatch.Draw (blank, black, Color.Black);
					ScreenManager.SpriteBatch.Draw (blank, green, color_green);
				}

				ScreenManager.SpriteBatch.DrawString (font, side_string, new Vector2 ((float)(width / 2 - font.MeasureString (side_string).X * font_manage._scale / 2), (float)(height * 0.2)), Color.White * _alpha_texte, 0f, Vector2.Zero, font_manage._scale, SpriteEffects.None, 1f);
				ScreenManager.SpriteBatch.DrawString (font, red_string, new Vector2 ((float)(width * 0.25 - font.MeasureString (red_string).X * font_manage._scale / 2), (float)(height * 0.4)), Color.White * _alpha_texte, 0f, Vector2.Zero, font_manage._scale, SpriteEffects.None, 1f);
				ScreenManager.SpriteBatch.DrawString (font, green_string, new Vector2 ((float)(width * 0.75 - font.MeasureString (green_string).X * font_manage._scale / 2), (float)(height * 0.4)), Color.White * _alpha_texte, 0f, Vector2.Zero, font_manage._scale, SpriteEffects.None, 1f);
			} else {
				ScreenManager.SpriteBatch.Draw (blank, green, color_green * _alpha_block);
				ScreenManager.SpriteBatch.Draw (blank, black, Color.Black * _alpha_block);
				ScreenManager.SpriteBatch.Draw (blank, red, color_red * _alpha_block);

				ScreenManager.SpriteBatch.DrawString (font, side_final, new Vector2 ((float)(width / 2 - font.MeasureString (side_final).X * font_manage._scale / 2), (float)(height / 2 - font.MeasureString (side_final).Y * font_manage._scale / 2)), Color.White * _alpha_texte, 0f, Vector2.Zero, font_manage._scale, SpriteEffects.None, 1f);
			}

			ScreenManager.SpriteBatch.End ();

			base.Draw (gameTime);
		}
	}
}

