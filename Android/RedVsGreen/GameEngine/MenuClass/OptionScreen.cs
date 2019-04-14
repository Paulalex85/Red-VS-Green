using System;
using GameStateManagement;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace RedVsGreen
{
	public class OptionScreen : GameScreen
	{
		int width, height;
		Vector2 position_bouton_1, position_bouton_2, position_bouton_3, bouton_taille;
		SpriteFont font_bouton, font_titre;
		Police_Size_Manage font_manage;
		Color color_fond = new Color(250,248,239), color_bouton = new Color(143,122,102), color_texte = new Color(119,110,101);
		Texture2D back;
		float _scale = 1f;
		Vector2 position_back;
		Divers_Method blbl = new Divers_Method ();
		Bouton bouton_1, bouton_2, bouton_3;
		Rectangle r1, r2, r3;
		TransitionClass transition = new TransitionClass();

		string caca, side_string, name_string, help_string;
		Languages langue = new Languages ();

		public OptionScreen ()
		{
			EnabledGestures = GestureType.Tap;
		}

		public override void LoadContent ()
		{
			width = ScreenManager.GraphicsDevice.Viewport.Width;
			height = ScreenManager.GraphicsDevice.Viewport.Height;

			caca = langue.getString (42);
			side_string = langue.getString (39);
			name_string = langue.getString (40);
			help_string = langue.getString (41);

			font_manage = new Police_Size_Manage (height, width, this);
			font_bouton = font_manage.Get_Regular_Font ();
			font_titre = font_manage.Get_Bold_Font();
			_scale = font_manage._scale;

			back = ScreenManager.Game.Content.Load<Texture2D> ("back");
			position_back = new Vector2 ((float)(width * 0.05), (float)(height * 0.05));

			bouton_taille = new Vector2 ((float)(width * 0.5), (float)(height * 0.15));

			position_bouton_1 = new Vector2 ((float)(width * 0.5 - bouton_taille.X /2), (float)(height * 0.3));
			position_bouton_2 = new Vector2 ((float)(width * 0.5 - bouton_taille.X /2), (float)(height * 0.5));
			position_bouton_3 = new Vector2 ((float)(width * 0.5 - bouton_taille.X /2), (float)(height * 0.7));

			r1 = new Rectangle ((int)(position_bouton_1.X), (int)(position_bouton_1.Y), (int)(bouton_taille.X), (int)(bouton_taille.Y));
			r2 = new Rectangle ((int)(position_bouton_2.X), (int)(position_bouton_2.Y), (int)(bouton_taille.X), (int)(bouton_taille.Y));
			r3 = new Rectangle ((int)(position_bouton_3.X), (int)(position_bouton_3.Y), (int)(bouton_taille.X), (int)(bouton_taille.Y));
			int marge = (int)(r1.Height * 0.1);

			bouton_1 = new Bouton (this, r1, font_bouton, side_string, marge, 0, Color.White, color_bouton, _scale);
			bouton_2 = new Bouton (this, r2, font_bouton, name_string, marge, 0, Color.White, color_bouton, _scale);
			bouton_3 = new Bouton (this, r3, font_bouton, help_string, marge, 0, Color.White, color_bouton, _scale);

			base.LoadContent ();
		}

		public override void HandleInput (InputState input)
		{
			foreach (GestureSample gesture in input.Gestures) {
				if (gesture.GestureType == GestureType.Tap) {
					if (gesture.Position.X > position_back.X &&
						gesture.Position.X < position_back.X + (back.Width * _scale) &&
						gesture.Position.Y > position_back.Y &&
						gesture.Position.Y < position_back.Y + (back.Height * _scale)) {
						Quitter ();
					}
				}
				if (gesture.GestureType == GestureType.Tap) {
					if (bouton_1.Input (gesture.Position)) {
						Change_Side ();
					}
				}
				if (gesture.GestureType == GestureType.Tap) {
					if (bouton_2.Input (gesture.Position)) {
						Change_Name ();
					}
				}
				if (gesture.GestureType == GestureType.Tap) {
					if (bouton_3.Input (gesture.Position)) {
						Show_Help ();
					}
				}
			}
			base.HandleInput (input);
		}

		public override void Update (Microsoft.Xna.Framework.GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
		{
			base.Update (gameTime, otherScreenHasFocus, coveredByOtherScreen);

			float timer = (float)gameTime.ElapsedGameTime.TotalMilliseconds;

			transition.Update_Transition (timer);

		}

		void Show_Help()
		{
			this.ExitScreen ();
			ScreenManager.AddScreen (new Tuto ());
		}

		void Change_Side()
		{
			this.ExitScreen ();
			ScreenManager.AddScreen (new SelectionScreen (true));
		}

		void Change_Name()
		{
			this.ExitScreen ();
			ScreenManager.AddScreen (new Name_Screen(true));
		}

		void Quitter()
		{
			this.ExitScreen ();
			ScreenManager.AddScreen (new MainMenuScreen ());
		}

		public override void Draw (Microsoft.Xna.Framework.GameTime gameTime)
		{
			ScreenManager.SpriteBatch.Begin ();

			ScreenManager.SpriteBatch.Draw (ScreenManager.BlankTexture, new Rectangle (0, 0, width, height), color_fond);

			ScreenManager.SpriteBatch.DrawString (font_titre, caca, new Vector2 ((float)(width / 2 - font_titre.MeasureString (caca).X*font_manage._scale / 2), (float)(height * 0.15)), color_texte * transition._transition_alpha, 0, Vector2.Zero, _scale, SpriteEffects.None, 1f);

			ScreenManager.SpriteBatch.Draw (back, position_back, null, Color.White * transition._transition_alpha, 0f, new Vector2 (0, 0), _scale, SpriteEffects.None, 1f);

			bouton_1.Draw (transition._transition_alpha);
			bouton_2.Draw (transition._transition_alpha);
			bouton_3.Draw (transition._transition_alpha);

			ScreenManager.SpriteBatch.End ();

			base.Draw (gameTime);
		}
	}
}

