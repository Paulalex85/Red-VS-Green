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
	public class RechercheAdversaireClass : GameScreen
	{
		int width, height;
		Rectangle r;
		Color color_fond = new Color(250,248,239), color_bouton = new Color(143,122,102), color_texte = new Color(119,110,101);
		Police_Size_Manage font_manage;
		SpriteFont font;
		string string_1, bouton_1_string, bouton_2_string;
		Vector2 _position_texte, _position_loading;
		Bouton _bouton_1, _bouton_2;
		LoadingSprite _loading;
		bool is_dans_salle_attente = false;
		Compteur_Time _timer_ping_partie = new Compteur_Time(2000f);
		string _code, _id;
		No_Connection_POPUP _no_co;
		TransitionClass transition = new TransitionClass();
		Languages langue = new Languages();

		//ANNIMATION SORTIE
		private enum TypeEcranAnnimation {
			Quitter,
			Rien
		}

		Adversaire_Found_POPUP popup;
		Echange_Server_Class server;


		public RechercheAdversaireClass ()
		{
			EnabledGestures = GestureType.Tap;
		}

		public RechercheAdversaireClass(Echange_Server_Class _server)
		{
			server = _server;
		}

		public override void LoadContent ()
		{
			width = ScreenManager.GraphicsDevice.Viewport.Width;
			height = ScreenManager.GraphicsDevice.Viewport.Height;
			r = new Rectangle (0, 0, width, height);

			font_manage = new Police_Size_Manage (height, width, this);
			font = font_manage.Get_Regular_Font ();

			string_1 = langue.getString (43);
			bouton_1_string = langue.getString (44);
			bouton_2_string = langue.getString (21);

			_no_co = new No_Connection_POPUP (this);

			_code = (string)IsolatedStorageSettings.ApplicationSettings ["code"];
			_id = (string)IsolatedStorageSettings.ApplicationSettings ["id"];

			int _loading_height = (int)(width * 0.05);

			_position_texte = new Vector2 ((float)(width /2 - font.MeasureString (string_1).X*font_manage._scale / 2), (float)(height * 0.3));
			_position_loading = new Vector2 ((float)(width *0.6 + font.MeasureString (string_1).X*font_manage._scale / 2), (float)(height * 0.3 + font.MeasureString (string_1).Y*font_manage._scale / 2));

			Vector2 bouton_taille = new Vector2 ((float)(width * 0.5), (float)(height * 0.15));
			Vector2 position_bouton_1 = new Vector2 ((float)(width /2 - bouton_taille.X /2), (float)(height * 0.45));

			Rectangle r1 = new Rectangle ((int)(position_bouton_1.X), (int)(position_bouton_1.Y), (int)(bouton_taille.X), (int)(bouton_taille.Y));
			Rectangle r2 = new Rectangle ((int)(position_bouton_1.X), (int)(height * 0.65), (int)(bouton_taille.X), (int)(bouton_taille.Y));
			int marge = (int)(r1.Height * 0.1);
			_bouton_1 = new Bouton (this, r1, font, bouton_1_string, marge, 0, Color.White, color_bouton, font_manage._scale);
			_bouton_2 = new Bouton (this, r2, font, bouton_2_string, marge, 0, Color.White, color_bouton, font_manage._scale);

			_loading = new LoadingSprite (this, _loading_height, _position_loading, color_texte);

			if (server == null) {
				server = new Echange_Server_Class ();
				server.Ajout_Joueur_Liste_Attente (_id, _code);
			} else {
				if (!server.Echange_en_cours) {
					server.Verifier_Partie_trouver (_id, _code);
				}
			}
			popup = new Adversaire_Found_POPUP (this, server);

			base.LoadContent ();
		}

		public override void UnloadContent ()
		{
			base.UnloadContent ();
		}

		public override void HandleInput (InputState input)
		{
			if (_no_co._statut != No_Connection_POPUP.Statut_Popup.Wait) {
				_no_co.Input (input);
				if (_no_co._statut == No_Connection_POPUP.Statut_Popup.Option_1) {
					_no_co._statut = No_Connection_POPUP.Statut_Popup.Wait;
					server.CloseConnection ();
					server = new Echange_Server_Class ();
					server.Ajout_Joueur_Liste_Attente (_id, _code);
				} else if (_no_co._statut == No_Connection_POPUP.Statut_Popup.Option_2) {
					server.CloseConnection ();
					this.ExitScreen ();
					ScreenManager.AddScreen (new MainMenuScreen ());
				}
			} else if (popup.is_active) {
				popup.Input (input);
			} else {
				foreach (GestureSample gesture in input.Gestures) {
					if (gesture.GestureType == GestureType.Tap) {
						if (_bouton_1.Input (gesture.Position)) {
							this.ExitScreen ();
							ScreenManager.AddScreen (new PlayClass (server, is_dans_salle_attente)); 
						}
						if (_bouton_2.Input (gesture.Position)) {
							server.CloseConnection ();
							this.ExitScreen ();
							ScreenManager.AddScreen (new MainMenuScreen ()); 
						}
					}
				}
			}
			base.HandleInput (input);
		}

		public override void Update (GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
		{
			if (GamePad.GetState (PlayerIndex.One).Buttons.Back == ButtonState.Pressed) {
				this.ExitScreen ();
				ScreenManager.AddScreen (new MainMenuScreen ());
			}

			float timer = (float)gameTime.ElapsedGameTime.TotalMilliseconds;
			transition.Update_Transition (timer);

			_no_co.Update (timer, server);

			if (!popup.is_active) {
				_loading.Update(timer);
			} else {
				GestionFin_Popup ();
				popup.Update (timer);
			}

			if (!popup.is_active) {
				if (server.Info_Attente_Check ()) {
					string blbl = server.Recuperer_Info ();
					string[] data = blbl.Split (' ');

					if (!is_dans_salle_attente) {
						if (data [0] == "true") {
							is_dans_salle_attente = true;
							server.Verifier_Partie_trouver (_id, _code);
						} else {
							server.Ajout_Joueur_Liste_Attente (_id, _code);
						}
					} else {
						if (data [0] == "true") {
							Adversaire_Found (data [1]);
						}
					}
				}
				if (is_dans_salle_attente) {
					if (!server.Echange_en_cours && _timer_ping_partie.IncreaseTimer (timer)) {
						server.Verifier_Partie_trouver (_id, _code);
					}
				}
			}



			base.Update (gameTime, otherScreenHasFocus, coveredByOtherScreen);
		}

		private void GestionFin_Popup()
		{
			if (popup.statut == Adversaire_Found_POPUP.Statut_Pop_Up.Partie_found) {
				popup.statut = Adversaire_Found_POPUP.Statut_Pop_Up.Attente;
				server.CloseConnection ();
				this.ExitScreen ();
				ScreenManager.AddScreen(new PlayClass(popup.data_game, true));

			} else if (popup.statut == Adversaire_Found_POPUP.Statut_Pop_Up.Retour_Menu || popup.statut == Adversaire_Found_POPUP.Statut_Pop_Up.Refuse) {
				popup.statut = Adversaire_Found_POPUP.Statut_Pop_Up.Attente;
				server.CloseConnection ();
				this.ExitScreen ();
				ScreenManager.AddScreen (new MainMenuScreen ());
			}
			else if (popup.statut == Adversaire_Found_POPUP.Statut_Pop_Up.Retour_liste_attente) {
				popup.statut = Adversaire_Found_POPUP.Statut_Pop_Up.Attente;
				popup = new Adversaire_Found_POPUP (this, server);
				server.Ajout_Joueur_Liste_Attente (_id, _code);
			}
		}

		private void Adversaire_Found(string _id_partie)
		{
			is_dans_salle_attente = false;
			popup.is_active = true;
			popup._id_partie_found = _id_partie;
		}

		public override void Draw (GameTime gameTime)
		{
			ScreenManager.SpriteBatch.Begin ();

			//FOND
			ScreenManager.SpriteBatch.Draw (ScreenManager.BlankTexture, r, color_fond);
			ScreenManager.SpriteBatch.DrawString (font, string_1, _position_texte, color_texte * transition._transition_alpha, 0f, Vector2.Zero, font_manage._scale, SpriteEffects.None, 1f);
			_bouton_1.Draw (transition._transition_alpha);
			_bouton_2.Draw (transition._transition_alpha);

			if (popup.is_active) {
				popup.Draw ();
			} else {
				_loading.Draw (transition._transition_alpha);

			}

			_no_co.Draw ();

			ScreenManager.SpriteBatch.End ();
			base.Draw (gameTime);
		}

		static double DegreesToRadians(double angleInDegrees)
		{
			return angleInDegrees * (Math.PI / 180);
		}
	}
}

