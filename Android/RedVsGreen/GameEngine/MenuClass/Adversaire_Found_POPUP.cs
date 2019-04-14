using System;
using GameStateManagement;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.IO.IsolatedStorage;
using System.Collections.Generic;

namespace RedVsGreen
{
	public class Adversaire_Found_POPUP
	{
		public enum Statut_Pop_Up
		{
			Partie_found,
			Attente,
			Accept,
			Refuse,
			Retour_liste_attente,
			Retour_Menu
		}

		public Statut_Pop_Up statut = Statut_Pop_Up.Attente;

		Police_Size_Manage font_manage;
		SpriteFont font_bold, font_regular, font_gros;
		GameScreen _screen;
		int width, height, marge;
		Vector2 position_bouton_1, position_bouton_2, bouton_taille, _position_texte, _position_loading;
		Bouton bouton_1, bouton_2;
		string info, string_accept  , string_refuse  , string_wait_adversaire  , string_time  , string_adversaire_refuse  , string_menu , string_retour_file_attente;
		Languages langue = new Languages ();
		public bool is_active = false;
		Color color_fond = new Color(250,248,239), color_bouton = new Color(143,122,102), color_texte = new Color(119,110,101);
		Rectangle r1,r2;
		Compteur_Time _timer = new Compteur_Time (5999f);
		public string _id_partie_found = "";
		string _id, _code;
		public Echange_Server_Class _server;
		bool _out_of_time = false, _adversaire_refuse = false;
		LoadingSprite _loading;
		public List<string> data_game = new List<string> ();
		bool option_1 = false, option_2 = false, option_3 = false, option_4 = false;
		Compteur_Time _timer_ping = new Compteur_Time(1000f);
		bool _ping_partie = false;

		public Adversaire_Found_POPUP (GameScreen screen, Echange_Server_Class server)
		{
			_screen = screen;
			_server = server;
			LoadContent ();
		}

		private void LoadContent()
		{
			width = _screen.ScreenManager.GraphicsDevice.Viewport.Width;
			height = _screen.ScreenManager.GraphicsDevice.Viewport.Height;
			bouton_taille = new Vector2 ((float)(width * 0.4), (float)(height * 0.1));

			info = langue.getString (13);
			string_accept  = langue.getString (14); 
			string_refuse  = langue.getString (15);
			string_wait_adversaire  = langue.getString (16);
			string_time  = langue.getString (17);
			string_adversaire_refuse  = langue.getString (18);
			string_menu = langue.getString (7);
			string_retour_file_attente = langue.getString (19);

			font_manage = new Police_Size_Manage (height, width, _screen);
			font_bold = font_manage.Get_Bold_Font ();
			font_regular = font_manage.Get_Regular_Font ();
			font_gros = font_manage.Get_Bold_Gros_Font ();

			_id = (string)IsolatedStorageSettings.ApplicationSettings ["id"];
			_code = (string)IsolatedStorageSettings.ApplicationSettings ["code"];


			bouton_taille = new Vector2 ((float)(width * 0.4), (float)(height * 0.1));

			position_bouton_1 = new Vector2 ((float)(width * 0.05), (float)(height * 0.65));
			position_bouton_2 = new Vector2 ((float)(width * 0.55), (float)(height * 0.65));

			r1 = new Rectangle ((int)(position_bouton_1.X), (int)(position_bouton_1.Y), (int)(bouton_taille.X), (int)(bouton_taille.Y));
			r2 = new Rectangle ((int)(position_bouton_2.X), (int)(position_bouton_2.Y), (int)(bouton_taille.X), (int)(bouton_taille.Y));
			marge = (int)(r1.Height * 0.1);

			bouton_1 = new Bouton (_screen, r1, font_regular, string_accept, marge, 0, Color.White, color_bouton, font_manage._scale);
			bouton_2 = new Bouton (_screen, r2, font_regular, string_refuse, marge, 0, Color.White, color_bouton, font_manage._scale);

			int _loading_height = (int)(width * 0.05);
			_position_texte = new Vector2 ((float)(width /2 - font_bold.MeasureString (string_wait_adversaire).X*font_manage._scale / 2), (float)(height * 0.3));
			_position_loading = new Vector2 ((float)(width *0.6 + font_regular.MeasureString (string_wait_adversaire).X *font_manage._scale/ 2), (float)(height * 0.3 + font_regular.MeasureString (string_wait_adversaire).Y*font_manage._scale / 2));

			_loading = new LoadingSprite (_screen, _loading_height, _position_loading, color_texte);
		}

		public void Input(InputState input)
		{
			if (option_1) {
				_server.Client_Valid_Or_Refuse_Game (_id, _id_partie_found, _code, "1");
				option_1 = false;
				bouton_1._bouton_tapped = false;
				statut = Statut_Pop_Up.Accept;
			} else if (option_2) {
				_server.Client_Valid_Or_Refuse_Game (_id, _id_partie_found, _code, "0");
				option_2 = false;
				bouton_2._bouton_tapped = false;
				statut = Statut_Pop_Up.Refuse;
				Reset_Bouton_Phase_2 ();
			}else if (option_3) {
				statut = Statut_Pop_Up.Retour_liste_attente;
				option_3 = false;
				_ping_partie = false;
				bouton_1._bouton_tapped = false;
			}else if (option_4) {
				statut = Statut_Pop_Up.Retour_Menu;
				option_4 = false;
				bouton_2._bouton_tapped = false;
			}


			if (!_server.Echange_en_cours) {
				if (!_out_of_time && !_adversaire_refuse) {
					foreach (GestureSample gesture in input.Gestures) {
						if (gesture.GestureType == GestureType.Tap) {
							if (bouton_1.Input (gesture.Position)) {
								option_1 = true;
							}
							if (bouton_2.Input (gesture.Position)) {
								option_2 = true;
							}
						}
					}
				} else {
					foreach (GestureSample gesture in input.Gestures) {
						if (gesture.GestureType == GestureType.Tap) {
							if (bouton_1.Input (gesture.Position)) {
								option_3 = true;
							}
							if (bouton_2.Input (gesture.Position)) {
								option_4 = true;
							}
						}
					}
				}
			}
		}

		private void Reset_Bouton_Phase_2()
		{
			bouton_1 = new Bouton (_screen, r1, font_regular, string_retour_file_attente, marge, 0, Color.White, color_bouton, font_manage._scale);
			bouton_2 = new Bouton (_screen, r2, font_regular, string_menu, marge, 0, Color.White, color_bouton, font_manage._scale);
		}

		public void Update(float timer)
		{
			if (!_out_of_time && !_adversaire_refuse && !_ping_partie) {
				if (_timer.IncreaseTimer (timer)) {
					_server.Client_Valid_Or_Refuse_Game (_id, _id_partie_found, _code, "0");
					_out_of_time = true;
					_ping_partie = false;
					Reset_Bouton_Phase_2 ();
				}
			}

			if (_server.Echange_en_cours) {
				_loading.Update (timer);
			}
			if (_server.Info_Attente_Check ()) {
				string blbl = _server.Recuperer_Info ();
				string[] data = blbl.Split (' ');
				if (data [0] == "ok") {
					statut = Statut_Pop_Up.Refuse;
				} else if (data [0] == "ERREUR" || data [0] == "4") {
					_out_of_time = true;
					_ping_partie = false;
					Reset_Bouton_Phase_2 ();
				}
				else if (data [0] == "3") {
					_adversaire_refuse = true;
					_ping_partie = false;
					Reset_Bouton_Phase_2 ();
				}
				else if (data [0] == "2") {
					statut = Statut_Pop_Up.Partie_found;
					data_game.Add (data [1]);
					data_game.Add (data [2]);
					data_game.Add (data [3]);
					data_game.Add (data [4]);
					data_game.Add (data [5]);
					data_game.Add (data [6]);
					data_game.Add (data [7]);
				}
				else if (data [0] == "1") {
					_ping_partie = true;
				}
			}

			if (_ping_partie && _timer_ping.IncreaseTimer (timer)) {
				_server.Client_Valid_Or_Refuse_Game (_id, _id_partie_found, _code, "1");
			}
		}

		public void Draw()
		{
			Rectangle r = new Rectangle (0, 0, width, height);
			_screen.ScreenManager.SpriteBatch.Draw (_screen.ScreenManager.BlankTexture, r, Color.White * (float)0.9);
			if (statut != Statut_Pop_Up.Accept && !_out_of_time && !_adversaire_refuse) {
				_screen.ScreenManager.SpriteBatch.DrawString (font_bold, info, new Vector2 ((float)(width / 2 - font_bold.MeasureString (info).X*font_manage._scale / 2), (float)(height * 0.1)), color_texte, 0f, Vector2.Zero, font_manage._scale, SpriteEffects.None, 1f);
			} else if (!_out_of_time && !_adversaire_refuse) {
				_screen.ScreenManager.SpriteBatch.DrawString (font_bold, string_wait_adversaire, new Vector2 ((float)(width / 2 - font_bold.MeasureString (string_wait_adversaire).X*font_manage._scale / 2), (float)(height * 0.1)), color_texte, 0f, Vector2.Zero, font_manage._scale, SpriteEffects.None, 1f);
			} else if (_out_of_time) {
				_screen.ScreenManager.SpriteBatch.DrawString (font_bold, string_time, new Vector2 ((float)(width / 2 - font_bold.MeasureString (string_time).X*font_manage._scale / 2), (float)(height * 0.1)), color_texte, 0f, Vector2.Zero, font_manage._scale, SpriteEffects.None, 1f);
			} else if (_adversaire_refuse) {
				_screen.ScreenManager.SpriteBatch.DrawString (font_bold, string_adversaire_refuse, new Vector2 ((float)(width / 2 - font_bold.MeasureString (string_adversaire_refuse).X*font_manage._scale / 2), (float)(height * 0.1)), color_texte, 0f, Vector2.Zero, font_manage._scale, SpriteEffects.None, 1f);
			}

			bouton_1.Draw ();
			bouton_2.Draw ();

			if (_server.Echange_en_cours) {
				_loading.Draw (1f);
			}

			//DRAW COMPTE A REBOURS
			string rebours_string = "";
			rebours_string = (_timer._timer_max - _timer._timer).ToString () [0].ToString ();

			if ((_timer._timer_max - _timer._timer) < 1000f) {
				rebours_string = "0";
			}
			if (!_out_of_time && !_adversaire_refuse) {
				_screen.ScreenManager.SpriteBatch.DrawString (font_gros, rebours_string, new Vector2 ((float)(width / 2 - font_bold.MeasureString (rebours_string).X*font_manage._scale / 2), (float)(height * 0.5)), color_texte, 0f, Vector2.Zero, font_manage._scale, SpriteEffects.None, 1f);
			}
		}
	}
}