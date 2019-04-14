using System;
using GameStateManagement;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace RedVsGreen
{
	public class No_Connection_POPUP
	{
		public enum Statut_Popup
		{
			Wait,
			Active,
			Option_1,
			Option_2
		}

		public Statut_Popup _statut = Statut_Popup.Wait;
		Police_Size_Manage font_manage;
		SpriteFont font_bold, font_regular;
		GameScreen _screen;
		int width, height, marge;
		Vector2 position_bouton_1, position_bouton_2, bouton_taille;
		Bouton bouton_1, bouton_2;
		Color color_bouton = new Color(143,122,102), color_texte = new Color(119,110,101);
		Rectangle r1,r2;
		bool bool_1=false, bool_2= false;
		Languages langue = new Languages();

		string option_1_string , option_2_string , info ;

		public No_Connection_POPUP (GameScreen screen)
		{
			_screen = screen;
			LoadContent ();
		}

		private void LoadContent()
		{
			width = _screen.ScreenManager.GraphicsDevice.Viewport.Width;
			height = _screen.ScreenManager.GraphicsDevice.Viewport.Height;
			bouton_taille = new Vector2 ((float)(width * 0.4), (float)(height * 0.1));

			option_1_string = langue.getString (19);
			option_2_string = langue.getString (7);
			info = langue.getString (38);

			font_manage = new Police_Size_Manage (height, width, _screen);
			font_bold = font_manage.Get_Bold_Font ();
			font_regular = font_manage.Get_Regular_Font ();

			bouton_taille = new Vector2 ((float)(width * 0.4), (float)(height * 0.1));

			position_bouton_1 = new Vector2 ((float)(width * 0.05), (float)(height * 0.7));
			position_bouton_2 = new Vector2 ((float)(width * 0.55), (float)(height * 0.7));

			r1 = new Rectangle ((int)(position_bouton_1.X), (int)(position_bouton_1.Y), (int)(bouton_taille.X), (int)(bouton_taille.Y));
			r2 = new Rectangle ((int)(position_bouton_2.X), (int)(position_bouton_2.Y), (int)(bouton_taille.X), (int)(bouton_taille.Y));
			marge = (int)(r1.Height * 0.1);

			bouton_1 = new Bouton (_screen, r1, font_regular, option_1_string, marge, 0, Color.White, color_bouton, font_manage._scale);
			bouton_2 = new Bouton (_screen, r2, font_regular, option_2_string, marge, 0, Color.White, color_bouton, font_manage._scale);
		}

		public void Input(InputState input)
		{
			if (bool_1) {
				_statut = Statut_Popup.Option_1;
				bool_1 = false;
				bouton_1._bouton_tapped = false;
			} else if (bool_2) {
				_statut = Statut_Popup.Option_2;
				bool_2 = false;
				bouton_2._bouton_tapped = false;
			}

			foreach (GestureSample gesture in input.Gestures) {
				if (gesture.GestureType == GestureType.Tap) {
					if (bouton_1.Input (gesture.Position)) {
						bool_1 = true;
					}
					if (bouton_2.Input (gesture.Position)) {
						bool_2 = true;
					}
				}
			}
		}		

		public void Update(float timer,Echange_Server_Class server)
		{
			if (_statut == No_Connection_POPUP.Statut_Popup.Wait) {
				if (server.Test_Connection (timer)) {
					_statut = No_Connection_POPUP.Statut_Popup.Active;
				}
			}
		}

		public void Draw ()
		{
			if (_statut != Statut_Popup.Wait) {
				Rectangle r = new Rectangle (0, 0, width, height);
				_screen.ScreenManager.SpriteBatch.Draw (_screen.ScreenManager.BlankTexture, r, Color.White * (float)0.9);

				_screen.ScreenManager.SpriteBatch.DrawString (font_bold, info, new Vector2 ((float)(width / 2 - font_bold.MeasureString (info).X*font_manage._scale / 2), (float)(height * 0.2)), color_texte, 0f, Vector2.Zero, font_manage._scale, SpriteEffects.None, 1f);

				bouton_1.Draw ();
				bouton_2.Draw ();
			}
		}
	}
}

