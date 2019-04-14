using System;
using GameStateManagement;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace RedVsGreen
{
	public class Type_Partie_Screen : GameScreen
	{
		int width, height;
		Vector2 position_bouton_1, position_bouton_2, bouton_taille;
		SpriteFont font_bouton, font_titre,font_texte;
		Police_Size_Manage font_manage;
		Color color_fond = new Color(250,248,239), color_bouton = new Color(143,122,102), color_texte = new Color(119,110,101);
		Texture2D back;
		Vector2 position_back;
		Divers_Method blbl = new Divers_Method ();
		Bouton bouton_1, bouton_2;
		Rectangle r1, r2;
		bool _tapped_multi = false, _tapped_bot = false;
		TransitionClass transition = new TransitionClass();
		Languages langue = new Languages();
		string info_bot, game_bot_string, multi_string, caca;
		float _scale;


		public Type_Partie_Screen ()
		{
			EnabledGestures = GestureType.Tap;
		}

		public override void LoadContent ()
		{
			width = ScreenManager.GraphicsDevice.Viewport.Width;
			height = ScreenManager.GraphicsDevice.Viewport.Height;

			info_bot = langue.getString (52);
			game_bot_string = langue.getString (54);
			multi_string = langue.getString (55);
			caca = langue.getString (53);

			font_manage = new Police_Size_Manage (height, width, this);
			font_bouton = font_manage.Get_Regular_Font ();
			font_titre = font_manage.Get_Bold_Font ();
			font_texte = font_manage.Get_Regular_Petit_Font ();

			back = ScreenManager.Game.Content.Load<Texture2D> ("back");
			position_back = new Vector2 ((float)(width * 0.05), (float)(height * 0.05));

			bouton_taille = new Vector2 ((float)(width * 0.5), (float)(height * 0.15));

			position_bouton_1 = new Vector2 ((float)(width /2  - bouton_taille.X /2 ), (float)(height * 0.3));
			position_bouton_2 = new Vector2 ((float)(width /2  - bouton_taille.X /2 ), (float)(height * 0.6));

			r1 = new Rectangle ((int)(position_bouton_1.X), (int)(position_bouton_1.Y), (int)(bouton_taille.X), (int)(bouton_taille.Y));
			r2 = new Rectangle ((int)(position_bouton_2.X), (int)(position_bouton_2.Y), (int)(bouton_taille.X), (int)(bouton_taille.Y));
			int marge = (int)(r1.Height * 0.1);

			bouton_1 = new Bouton (this, r1, font_bouton, game_bot_string, marge, 0, Color.White, color_bouton, font_manage._scale);
			bouton_2 = new Bouton (this, r2, font_bouton, multi_string, marge, 0, Color.White, color_bouton, font_manage._scale);

			_scale = Calcul_Scale_Texture ();

			base.LoadContent ();
		}

		public override void HandleInput (InputState input)
		{
			if (_tapped_bot) {
				Partie_Vs_Bot ();
			} else if (_tapped_multi) {
				Partie_Multi ();
			}

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
						_tapped_bot = true;
					}
				}
				if (gesture.GestureType == GestureType.Tap) {
					if (bouton_2.Input (gesture.Position)) {
						_tapped_multi = true;
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

		void Partie_Vs_Bot()
		{
			this.ExitScreen ();
			ScreenManager.AddScreen (new PlayClass ());
		}

		void Partie_Multi()
		{
			this.ExitScreen ();
			ScreenManager.AddScreen (new RechercheAdversaireClass ());
		}

		void Quitter()
		{
			this.ExitScreen ();
			ScreenManager.AddScreen (new MainMenuScreen ());
		}

		private float Calcul_Scale_Texture()
		{
			double scale = 1.0000;
			int base_width = 480, base_height = 800;
			float difference_width = (float)(width - base_width);
			float difference_height = (float)(height - base_height);
			if (difference_height > difference_width) {
				scale = (double)width / base_width;
			} else {
				scale = (double)height / base_height;
			}
			return (float)(scale);
		}

		public override void Draw (Microsoft.Xna.Framework.GameTime gameTime)
		{
			ScreenManager.SpriteBatch.Begin ();

			ScreenManager.SpriteBatch.Draw (ScreenManager.BlankTexture, new Rectangle(0,0,width,height), color_fond);

			ScreenManager.SpriteBatch.DrawString (font_titre, caca, new Vector2 ((float)(width / 2 - font_titre.MeasureString (caca).X*font_manage._scale / 2), (float)(height * 0.15)), color_texte* transition._transition_alpha, 0f, Vector2.Zero, font_manage._scale, SpriteEffects.None, 1f);
			ScreenManager.SpriteBatch.Draw (back, position_back, new Rectangle (0,0,(int)(back.Width), (int)(back.Height)), Color.White * transition._transition_alpha, 0f, new Vector2 (0, 0), _scale, SpriteEffects.None, 1f);

			bouton_1.Draw (transition._transition_alpha);
			bouton_2.Draw (transition._transition_alpha);

			ScreenManager.SpriteBatch.DrawString (font_texte, info_bot, new Vector2 ((float)(width / 2 - font_texte.MeasureString (info_bot).X*font_manage._scale / 2), (float)(position_bouton_1.Y + bouton_taille.Y + bouton_taille.Y * 0.1)), color_texte* transition._transition_alpha, 0f, Vector2.Zero, font_manage._scale, SpriteEffects.None, 1f);

			ScreenManager.SpriteBatch.End ();

			base.Draw (gameTime);
		}

	}
}

