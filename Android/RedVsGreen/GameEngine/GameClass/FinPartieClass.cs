using System;
using GameStateManagement;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using System.Collections.Generic;
using System.IO.IsolatedStorage;

namespace RedVsGreen
{
	public class FinPartieClass
	{
		public enum Statut_Fin{
			Attente,
			Menu,
			Rejouer
		}

		public Statut_Fin _statut = Statut_Fin.Attente;

		SpriteFont font_bold, font_regular;
		Police_Size_Manage font_manage;
		string string_win, string_loose, string_draw,string_elo,string_rank , string_rejouer , string_quit;
		Languages langue = new Languages ();
		bool _win, _draw, _multi;
		string _info_fin_partie, _new_elo, _new_rank, _old_elo, _old_rank, _name,_point_1, _point_2;
		string _difference_elo, _difference_rank;
		GameScreen _screen;
		int width, height;
		Color color_font = new Color(143,122,102);
		Bouton bouton_1, bouton_2;
		Rectangle r1, r2;
		Vector2 position_play, position_quit, bouton_taille, position_point_1, position_point_2;
		bool bool_1=false, bool_2= false, bool_draw = false;

		public FinPartieClass (GameScreen screen, bool win, bool draw, string point_1, string point_2)
		{
			_screen = screen;
			_win = win;
			_draw = draw;
			_multi = false;
			_point_1 = point_1;
			_point_2 = point_2;
			LoadContent ();
		}

		public FinPartieClass (GameScreen screen, string info_fin_partie)
		{
			_screen = screen;
			_multi = true;
			_info_fin_partie = info_fin_partie;
			LoadContent ();
		}

		private void LoadContent()
		{
			width = _screen.ScreenManager.GraphicsDevice.Viewport.Width;
			height = _screen.ScreenManager.GraphicsDevice.Viewport.Height;
			bouton_taille = new Vector2 ((float)(width * 0.4), (float)(height * 0.1));

			string_win = langue.getString (1);
			string_loose = langue.getString (2);
			string_draw = langue.getString (3);
			string_elo = langue.getString (4);
			string_rank = langue.getString (5);
			string_rejouer = langue.getString (6);
			string_quit = langue.getString (7);

			font_manage = new Police_Size_Manage (height, width, _screen);
			font_bold = font_manage.Get_Bold_Font ();
			font_regular = font_manage.Get_Regular_Font ();

			position_play = new Vector2 ((float)(width * 0.05), (float)(height * 0.7));
			position_quit = new Vector2 ((float)(width * 0.55), (float)(height * 0.7));

			r1 = new Rectangle ((int)(position_play.X), (int)(position_play.Y), (int)(bouton_taille.X), (int)(bouton_taille.Y));
			r2 = new Rectangle ((int)(position_quit.X), (int)(position_quit.Y), (int)(bouton_taille.X), (int)(bouton_taille.Y));
			int marge = (int)(r1.Height * 0.1);

			bouton_1 = new Bouton (_screen, r1, font_regular, string_rejouer, marge, 0, Color.White, color_font, font_manage._scale);
			bouton_2 = new Bouton (_screen, r2, font_regular, string_quit, marge, 0, Color.White, color_font, font_manage._scale);


			if (_multi) {
				string _id = (string)IsolatedStorageSettings.ApplicationSettings ["id"];
				string[] data = _info_fin_partie.Split ('_');
				bool _is_joueur_1 = false;
				if (data [3] == _id) {
					_is_joueur_1 = true;
				}
				if ((_is_joueur_1 && data [2] == "1") || (!_is_joueur_1 && data [2] == "3")) {
					_win = true;
					_draw = false;
				} else if (data [2] == "2") {
					_draw = true;
					_win = false;
				} else {
					_draw = false;
					_win = false;
				}

				if (_is_joueur_1) {
					_point_1 = data [4];
					_point_2 = data [5];
				} else {
					_point_1 = data [5];
					_point_2 = data [4];
				}
				position_point_1 = new Vector2 ((float)(width * 0.25 - font_bold.MeasureString (_point_1).X  *font_manage._scale / 2), (float)(height * 0.15));
				position_point_2 = new Vector2 ((float)(width * 0.75 - font_bold.MeasureString (_point_2).X *font_manage._scale / 2), (float)(height * 0.15));

				_old_elo = (string)IsolatedStorageSettings.ApplicationSettings ["elo"];
				_old_rank = (string)IsolatedStorageSettings.ApplicationSettings ["rank"];
				_name = (string)IsolatedStorageSettings.ApplicationSettings ["name"];
				_new_elo = data [0];
				_new_rank = data [1];

				int _diff_elo = int.Parse (_new_elo) - int.Parse (_old_elo);
				int _diff_rank = int.Parse (_new_rank) - int.Parse (_old_rank);
				if (_diff_elo > 0) {
					_difference_elo = "+" + _diff_elo.ToString ();
				} else {
					_difference_elo = _diff_elo.ToString ();
				}

				if (_diff_rank > 0) {
					_difference_rank = (_diff_rank - _diff_rank * 2).ToString ();
				} else {
					_difference_rank = "+" + (-_diff_rank).ToString ();
				}
				IsolatedStorageSettings.ApplicationSettings ["elo"] = _new_elo;
				IsolatedStorageSettings.ApplicationSettings ["rank"] = _new_rank;
			} else {
				_name = (string)IsolatedStorageSettings.ApplicationSettings ["name"];
				_new_elo = (string)IsolatedStorageSettings.ApplicationSettings ["elo"];
				_new_rank = (string)IsolatedStorageSettings.ApplicationSettings ["rank"];

				if (_new_elo == null) {
					_new_elo = "";
				}
				if (_new_rank == null) {
					_new_rank = "";
				}
			}
		}

		public void Input(InputState input)
		{
			if (bool_draw) {
				if (bool_1) {
					_statut = Statut_Fin.Rejouer;
					bool_1 = false;
				} else if (bool_2) {
					_statut = Statut_Fin.Menu;
					bool_2 = false;
				}
				bool_draw = false;
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

		public void Update()
		{
		}

		public void Draw()
		{
			Rectangle r = new Rectangle (0, 0, width, height);
			_screen.ScreenManager.SpriteBatch.Draw (_screen.ScreenManager.BlankTexture, r, Color.White * (float)0.9 );

			if (_win) {
				_screen.ScreenManager.SpriteBatch.DrawString (font_bold, string_win, new Vector2 ((float)(width / 2 - font_bold.MeasureString (string_win).X *font_manage._scale/ 2), (float)(height * 0.1)), color_font, 0f, Vector2.Zero, font_manage._scale, SpriteEffects.None, 1f);
			} else if (_draw) {
				_screen.ScreenManager.SpriteBatch.DrawString (font_bold, string_draw, new Vector2 ((float)(width / 2 - font_bold.MeasureString (string_draw).X*font_manage._scale / 2), (float)(height * 0.1)), color_font, 0f, Vector2.Zero, font_manage._scale, SpriteEffects.None, 1f);
			} else {
				_screen.ScreenManager.SpriteBatch.DrawString (font_bold, string_loose, new Vector2 ((float)(width / 2 - font_bold.MeasureString (string_loose).X*font_manage._scale / 2), (float)(height * 0.1)), color_font, 0f, Vector2.Zero, font_manage._scale, SpriteEffects.None, 1f);
			}

			_screen.ScreenManager.SpriteBatch.DrawString (font_regular, _point_1 + " - " + _point_2, new Vector2 ((float)(width / 2 - font_regular.MeasureString (_point_1 + " - " + _point_2).X*font_manage._scale / 2), (float)(height * 0.15)), color_font, 0f, Vector2.Zero, font_manage._scale, SpriteEffects.None, 1f);

			_screen.ScreenManager.SpriteBatch.DrawString (font_bold, _name, new Vector2 ((float)(width * 0.1), (float)(height * 0.3)), color_font, 0f, Vector2.Zero, font_manage._scale, SpriteEffects.None, 1f);
			_screen.ScreenManager.SpriteBatch.DrawString (font_bold, string_elo, new Vector2 ((float)(width * 0.1), (float)(height * 0.4)), color_font, 0f, Vector2.Zero, font_manage._scale, SpriteEffects.None, 1f);
			_screen.ScreenManager.SpriteBatch.DrawString (font_bold, string_rank, new Vector2 ((float)(width * 0.1), (float)(height * 0.5)), color_font, 0f, Vector2.Zero, font_manage._scale, SpriteEffects.None, 1f);

			_screen.ScreenManager.SpriteBatch.DrawString (font_regular, _new_elo, new Vector2 ((float)(width * 0.7 - font_regular.MeasureString(_new_elo).X*font_manage._scale), (float)(height * 0.41 )), color_font, 0f, Vector2.Zero, font_manage._scale, SpriteEffects.None, 1f);
			_screen.ScreenManager.SpriteBatch.DrawString (font_regular, _new_rank, new Vector2 ((float)(width * 0.7- font_regular.MeasureString(_new_rank).X*font_manage._scale), (float)(height * 0.51)), color_font, 0f, Vector2.Zero, font_manage._scale, SpriteEffects.None, 1f);
			if (_multi) {
				_screen.ScreenManager.SpriteBatch.DrawString (font_regular, _difference_elo, new Vector2 ((float)(width * 0.9 - font_regular.MeasureString (_difference_elo).X*font_manage._scale), (float)(height * 0.41)), color_font, 0f, Vector2.Zero, font_manage._scale, SpriteEffects.None, 1f);
				_screen.ScreenManager.SpriteBatch.DrawString (font_regular, _difference_rank, new Vector2 ((float)(width * 0.9 - font_regular.MeasureString (_difference_rank).X*font_manage._scale), (float)(height * 0.51)), color_font, 0f, Vector2.Zero, font_manage._scale, SpriteEffects.None, 1f);
			}
			Draw_Bouton ();
			if (bool_1 || bool_2) {
				bool_draw = true;
			}
		}

		private void Draw_Bouton()
		{
			bouton_1.Draw ();
			bouton_2.Draw ();
		}
	}
}