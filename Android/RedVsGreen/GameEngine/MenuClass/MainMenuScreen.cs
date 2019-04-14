using System;
using GameStateManagement;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using System.Json;
using System.Threading.Tasks;
using Android.Content;
using System.IO.IsolatedStorage;

namespace RedVsGreen
{
	class MainMenuScreen : GameScreen
	{
		//ANNIMATION SORTIE
		private enum TypeEcranAnnimation {
			Jouer,
			Quitter,
			Option,
			Rien
		}

		private TypeEcranAnnimation Sortie_ecran;

		//DIVERS MERDES
		string jouer, quitter, red_string = "RED", vs_string = "VS", green_string = "GREEN",
		win_string = "Win", elo_string = "Elo", option_string = "Options", rank_string;
		Languages langue = new Languages();
		string _mail = "hello@redvsgreen.fr";
		Rectangle r;
		Color color_fond = new Color(250,248,239), color_bouton = new Color(143,122,102), color_texte = new Color(119,110,101), color_green = new Color(71,164,71), color_red = new Color(246,94,59);
		Bouton bouton_1, bouton_2, bouton_3;
		Vector2 position_2, position_3, bouton_taille, position_1;
		Echange_Server_Class server = new Echange_Server_Class();
		TransitionClass transition = new TransitionClass();

		string red_win = "0", green_win= "0", player_win= "0", player_game= "0",player_rank= "0",player_elo= "0", player_name= "X", side ="", id ="";
		bool _name_check = false, _envoie_name = false;
		bool _draw_check = false, _need_draw_check = false;

		SpriteFont font_bouton, font_titre, font_mega_titre, font_tiny;
		Police_Size_Manage font_manage;

		int width, height, position_width_info_joueur;

		public MainMenuScreen ()
		{
			EnabledGestures = GestureType.Tap;
		}

		public override void LoadContent ()
		{
			Sortie_ecran = TypeEcranAnnimation.Rien;
			width = ScreenManager.GraphicsDevice.Viewport.Width;
			height = ScreenManager.GraphicsDevice.Viewport.Height;
			bouton_taille = new Vector2 ((float)(width * 0.4), (float)(height * 0.1));

			jouer = langue.getString (20);
			quitter= langue.getString (21);
			rank_string= langue.getString (22);

			position_width_info_joueur = (int)(width * 0.55);
			position_1 = new Vector2((float)(width * 0.05), (float)(height * 0.45));
			position_2 = new Vector2 ((float)(width * 0.05), (float)(height * 0.6));
			position_3 = new Vector2 ((float)(width * 0.05), (float)(height * 0.75));

			font_manage = new Police_Size_Manage (height, width, this);
			font_bouton = font_manage.Get_Regular_Font ();
			font_titre = font_manage.Get_Bold_Font ();
			font_mega_titre = font_manage.Get_Bold_Gros2_Font ();
			font_tiny = font_manage.Get_Regular_Petit_Font ();

			r = new Rectangle (0, 0, width, height);
			Rectangle r1, r2, r3;

			r1 = new Rectangle ((int)(position_1.X), (int)(position_1.Y), (int)(bouton_taille.X), (int)(bouton_taille.Y));
			r2 = new Rectangle ((int)(position_2.X), (int)(position_2.Y), (int)(bouton_taille.X), (int)(bouton_taille.Y));
			r3 = new Rectangle ((int)(position_3.X), (int)(position_3.Y), (int)(bouton_taille.X), (int)(bouton_taille.Y));

			int marge = (int)(r1.Height * 0.1);

			bouton_1 = new Bouton (this, r1, font_bouton, jouer, marge, 0, Color.White, color_bouton, font_manage._scale);
			bouton_2 = new Bouton (this, r2, font_bouton, option_string, marge, 0, Color.White, color_bouton, font_manage._scale);
			bouton_3 = new Bouton (this, r3, font_bouton, quitter, marge, 0, Color.White, color_bouton, font_manage._scale);

			id = (string)IsolatedStorageSettings.ApplicationSettings ["id"];
			player_name = (string)IsolatedStorageSettings.ApplicationSettings ["name"];
			side = (string)IsolatedStorageSettings.ApplicationSettings ["side"];

			if (IsolatedStorageSettings.ApplicationSettings ["red_win"] != null) {
				red_win = (string)IsolatedStorageSettings.ApplicationSettings ["red_win"];
				green_win = (string)IsolatedStorageSettings.ApplicationSettings ["green_win"];
				player_elo = (string)IsolatedStorageSettings.ApplicationSettings ["elo"];
				player_game = (string)IsolatedStorageSettings.ApplicationSettings ["player_game"];
				player_rank = (string)IsolatedStorageSettings.ApplicationSettings ["rank"];
				player_win = (string)IsolatedStorageSettings.ApplicationSettings ["player_win"];
			}

			if (id != null) {
				server.Main_Menu_Info (id, player_name);
			} else if (!_name_check) {
				server.Check_Name (player_name);
			}

			base.LoadContent ();
		}

		void JouerEntrySelected()
		{
			server.CloseConnection ();
			this.ExitScreen();
			ScreenManager.AddScreen (new Type_Partie_Screen());
		}

		void OptionsSelected()
		{
			server.CloseConnection ();
			this.ExitScreen();
			ScreenManager.AddScreen (new OptionScreen());
		}

		/*void ConnectionFacebook()
		{
			var auth = new OAuth2Authenticator (
				clientId: "1432676373681912",
				scope: "",
				authorizeUrl: new Uri ("https://m.facebook.com/dialog/oauth/"),
				redirectUrl: new Uri ("http://www.facebook.com/connect/login_success.html"));

			auth.AllowCancel = true;

			// If authorization succeeds or is canceled, .Completed will be fired.
			auth.Completed += (s, ee) => {
				if (!ee.IsAuthenticated) {
					return;
				}

				// Now that we're logged in, make a OAuth2 request to get the user's info.
				var request = new OAuth2Request ("GET", new Uri ("https://graph.facebook.com/me"), null, ee.Account);
				request.GetResponseAsync().ContinueWith (t => {
					if (t.IsFaulted) {
					} else if (t.IsCanceled)
					{
					}
					else {
						/*var obj = JsonValue.Parse (t.Result.GetResponseText());
						builder.SetMessage ("Name: " + obj["name"]);
					}
				}, UIScheduler);
			};

			var intent = auth.GetUI (Game1.Activity.ApplicationContext);
			Game1.Activity.StartActivity(intent);
		}

		void ConnectionFacebook()
		{
			const string AppId = "1432676373681912";
			const string ExtendedPermissions = "user_about_me";

			FacebookClient fb;
			string accessToken;
			bool isLoggedIn;
			string lastMessageId;

			var webAuth = new Intent (Game1.Activity.ApplicationContext, typeof (Connect_FB));
			webAuth.PutExtra ("AppId", AppId);
			webAuth.PutExtra ("ExtendedPermissions", ExtendedPermissions);
			Activity1 activity = Game1.Activity
			Game1.Activity.StartActivityForResult(webAuth, 0);
		}*/

		//private static readonly TaskScheduler UIScheduler = TaskScheduler.FromCurrentSynchronizationContext();

		void Quitter()
		{
			server.CloseConnection ();
			ScreenManager.Game.Exit ();
		}

		public override void HandleInput (InputState input)
		{
			foreach (GestureSample gesture in input.Gestures) {
				if (gesture.GestureType == GestureType.Tap) {
					if (bouton_1.Input (gesture.Position)) {
						Sortie_ecran = TypeEcranAnnimation.Jouer;
						_need_draw_check = true;
					}
					if (bouton_2.Input (gesture.Position)) {
						Sortie_ecran = TypeEcranAnnimation.Option;
						_need_draw_check = true;
					}
					if (bouton_3.Input (gesture.Position)) {
						Sortie_ecran = TypeEcranAnnimation.Quitter;
						_need_draw_check = true;
					}
				}
			}
			base.HandleInput (input);
		}

		public override void Update (Microsoft.Xna.Framework.GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
		{
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
			{
				this.ExitScreen ();
				ScreenManager.AddScreen(new Name_Screen(true));
			}

			float timer = (float)gameTime.ElapsedGameTime.TotalMilliseconds;

			if (server.Test_Connection (timer)) {
				server.CloseConnection ();
			}

			Execution_Screen ();
			transition.Update_Transition (timer);


			if (server.Info_Attente_Check ()) {
				string data = server.Recuperer_Info ();
				string[] data_array = data.Split (' ');


				if (id != null) {
					if (data_array [0] == "false") {
						id = null;
						Random _rand = new Random ();
						int blblb = _rand.Next (1, 1000000);
						if (langue.lang == "FR") {
							IsolatedStorageSettings.ApplicationSettings ["name"] = "Joueur" + blblb.ToString ();
							player_name = "Joueur" + blblb.ToString ();
						} else {
							IsolatedStorageSettings.ApplicationSettings ["name"] = "Guest" + blblb.ToString ();
							player_name = "Guest" + blblb.ToString ();
						}
						server.Check_Name (player_name);
					} else {
						for (int i = 0; i < data_array.Length; i++) {
							if (data_array [i] == null) {
								data_array [i] = "0";
							}
						}

						red_win = data_array [0];
						green_win = data_array [1];
						player_game = data_array [2];
						player_win = data_array [3];
						player_elo = data_array [4];
						player_rank = data_array [5];

						IsolatedStorageSettings.ApplicationSettings ["red_win"] = red_win;
						IsolatedStorageSettings.ApplicationSettings ["green_win"] = green_win;
						IsolatedStorageSettings.ApplicationSettings ["elo"] = player_elo;
						IsolatedStorageSettings.ApplicationSettings ["player_game"] = player_game;
						IsolatedStorageSettings.ApplicationSettings ["rank"] = player_rank;
						IsolatedStorageSettings.ApplicationSettings ["player_win"] = player_win;

						//ADRESS PORT SERVER
						if (data_array.Length > 6) {
							IsolatedStorageSettings.ApplicationSettings ["adresse_ip"] = data_array [6];
							IsolatedStorageSettings.ApplicationSettings ["port"] = data_array [7];
						}
					}
				} else if (!_name_check) {
					if (data_array [0] == "false") {
						_name_check = true;
						server.Nouveau_Joueur (player_name, side);
					} else {
						Random _rand = new Random ();
						int blblb = _rand.Next (1, 1000000);
						if (langue.lang == "FR") {
							IsolatedStorageSettings.ApplicationSettings ["name"] = "Joueur" + blblb.ToString ();
							player_name = "Joueur" + blblb.ToString ();
						} else {
							IsolatedStorageSettings.ApplicationSettings ["name"] = "Guest" + blblb.ToString ();
							player_name = "Guest" + blblb.ToString ();
						}
						server.Check_Name (player_name);
					}
				} else if (_name_check && !_envoie_name) {
					string code = "0"; //ASSIGNE 
					id = data_array [0].ToString ();
					code = data_array [1].ToString ();
					IsolatedStorageSettings.ApplicationSettings ["id"] = id;
					IsolatedStorageSettings.ApplicationSettings ["code"] = code;
				}
			}

			base.Update (gameTime, otherScreenHasFocus, coveredByOtherScreen);
		}

		private void Execution_Screen()
		{
			if (Sortie_ecran != TypeEcranAnnimation.Rien) {
				if (_draw_check) {
					if (Sortie_ecran == TypeEcranAnnimation.Jouer) {
						JouerEntrySelected ();
						Sortie_ecran = TypeEcranAnnimation.Rien;
					} else if (Sortie_ecran == TypeEcranAnnimation.Quitter) {
						Quitter ();
						Sortie_ecran = TypeEcranAnnimation.Rien;
					} else if (Sortie_ecran == TypeEcranAnnimation.Option) {
						OptionsSelected ();
						Sortie_ecran = TypeEcranAnnimation.Rien;
					}
				}
			}
		}

		public override void Draw (GameTime gameTime)
		{
			ScreenManager.SpriteBatch.Begin ();

			ScreenManager.SpriteBatch.Draw (ScreenManager.BlankTexture, r, color_fond);

			bouton_1.Draw (transition._transition_alpha);
			bouton_2.Draw (transition._transition_alpha);
			bouton_3.Draw (transition._transition_alpha);

			Draw_Interface ();
			Draw_Stats_Perso ();

			ScreenManager.SpriteBatch.DrawString(font_tiny, _mail, new Vector2(position_width_info_joueur, (float)(height * 0.8)), color_texte * transition._transition_alpha, 0f, Vector2.Zero, font_manage._scale, SpriteEffects.None, 1f);

			ScreenManager.SpriteBatch.End ();
			if (_need_draw_check) {
				_draw_check = true;
			}

			base.Draw (gameTime);
		}

		private void Draw_Interface()
		{
			//AFFICHE TITRE RED VS GREEN
			ScreenManager.SpriteBatch.DrawString(font_mega_titre, red_string, new Vector2((float)(width* 0.25 - font_mega_titre.MeasureString(red_string).X*font_manage._scale / 2 ),(float)( height * 0.1)), color_red * transition._transition_alpha, 0f, Vector2.Zero, font_manage._scale, SpriteEffects.None, 1f);
			ScreenManager.SpriteBatch.DrawString(font_mega_titre, vs_string, new Vector2((float)(width * 0.45 - font_mega_titre.MeasureString(vs_string).X*font_manage._scale / 2), (float)(height * 0.1)), color_texte * transition._transition_alpha, 0f, Vector2.Zero, font_manage._scale, SpriteEffects.None, 1f);
			ScreenManager.SpriteBatch.DrawString(font_mega_titre, green_string, new Vector2((float)(width * 0.7 - font_mega_titre.MeasureString(green_string).X*font_manage._scale / 2), (float)(height * 0.1)), color_green * transition._transition_alpha, 0f, Vector2.Zero, font_manage._scale, SpriteEffects.None, 1f);

			//AFFICHE WIN RATE
			ScreenManager.SpriteBatch.DrawString (font_bouton, win_string, new Vector2 ((float)(width*0.02), (float)(height * 0.3)), color_red * transition._transition_alpha, 0f, Vector2.Zero, font_manage._scale, SpriteEffects.None, 1f);
			ScreenManager.SpriteBatch.DrawString(font_bouton, red_win, new Vector2((float)(width * 0.05 + font_bouton.MeasureString(win_string).X*font_manage._scale), (float)(height * 0.3)), color_red * transition._transition_alpha, 0f, Vector2.Zero, font_manage._scale, SpriteEffects.None, 1f);

			ScreenManager.SpriteBatch.DrawString (font_bouton, win_string, new Vector2 ((float)(width*0.98 - font_bouton.MeasureString(win_string).X*font_manage._scale), (float)(height * 0.3)), color_green * transition._transition_alpha, 0f, Vector2.Zero, font_manage._scale, SpriteEffects.None, 1f);
			ScreenManager.SpriteBatch.DrawString(font_bouton, green_win, new Vector2((float)(width * 0.95 - font_bouton.MeasureString(win_string + green_win).X*font_manage._scale), (float)(height * 0.3)), color_green * transition._transition_alpha, 0f, Vector2.Zero, font_manage._scale, SpriteEffects.None, 1f);
		}

		private void Draw_Stats_Perso()
		{
			//DRAW NOM PLAYER
			if (font_bouton.MeasureString (player_name).X * font_manage._scale > width - position_width_info_joueur) {
				ScreenManager.SpriteBatch.DrawString (font_tiny, player_name, new Vector2 (position_width_info_joueur, (float)(height * 0.425)), color_texte * transition._transition_alpha, 0f, Vector2.Zero, font_manage._scale, SpriteEffects.None, 1f);
			} else {
				ScreenManager.SpriteBatch.DrawString (font_bouton, player_name, new Vector2 (position_width_info_joueur, (float)(height * 0.425)), color_texte * transition._transition_alpha, 0f, Vector2.Zero, font_manage._scale, SpriteEffects.None, 1f);
			}
				//DRAW INTITULE
			ScreenManager.SpriteBatch.DrawString (font_bouton, jouer, new Vector2 (position_width_info_joueur, (float)(height * 0.5)), color_texte * transition._transition_alpha, 0f, Vector2.Zero, font_manage._scale, SpriteEffects.None, 1f);
			ScreenManager.SpriteBatch.DrawString(font_bouton, win_string, new Vector2(position_width_info_joueur, (float)(height * 0.575)), color_texte * transition._transition_alpha, 0f, Vector2.Zero, font_manage._scale, SpriteEffects.None, 1f);
			ScreenManager.SpriteBatch.DrawString(font_bouton, elo_string, new Vector2(position_width_info_joueur, (float)(height * 0.65)), color_texte * transition._transition_alpha, 0f, Vector2.Zero, font_manage._scale, SpriteEffects.None, 1f);
			ScreenManager.SpriteBatch.DrawString(font_bouton, rank_string, new Vector2(position_width_info_joueur, (float)(height * 0.725)), color_texte * transition._transition_alpha, 0f, Vector2.Zero, font_manage._scale, SpriteEffects.None, 1f);
			//DRAW SCORE
			ScreenManager.SpriteBatch.DrawString(font_bouton, player_game, new Vector2((float)(width * 0.95 - font_bouton.MeasureString(player_game).X*font_manage._scale), (float)(height * 0.5)), color_texte * transition._transition_alpha, 0f, Vector2.Zero, font_manage._scale, SpriteEffects.None, 1f);
			ScreenManager.SpriteBatch.DrawString(font_bouton, player_win, new Vector2((float)(width * 0.95 - font_bouton.MeasureString(player_win).X*font_manage._scale), (float)(height * 0.575)), color_texte * transition._transition_alpha, 0f, Vector2.Zero, font_manage._scale, SpriteEffects.None, 1f);
			ScreenManager.SpriteBatch.DrawString(font_bouton, player_elo, new Vector2((float)(width * 0.95 - font_bouton.MeasureString(player_elo).X*font_manage._scale), (float)(height * 0.65)), color_texte * transition._transition_alpha, 0f, Vector2.Zero, font_manage._scale, SpriteEffects.None, 1f);
			ScreenManager.SpriteBatch.DrawString(font_bouton, player_rank, new Vector2((float)(width * 0.95 - font_bouton.MeasureString(player_rank).X*font_manage._scale), (float)(height * 0.725)), color_texte * transition._transition_alpha, 0f, Vector2.Zero, font_manage._scale, SpriteEffects.None, 1f);
		}
	}
}