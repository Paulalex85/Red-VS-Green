using System;
using GameStateManagement;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using System.Collections.Generic;
using System.Threading;
using System.IO.IsolatedStorage;

namespace RedVsGreen
{
	public class PlayClass : GameScreen
	{
		public enum Statut_Partie {
			En_Attente,
			Selection,
			Tour_Adversaire,
			Fin_Partie_Win,
			Fin_Partie_Loose,
			Fin_Partie_Draw
		}

		//GENERAL
		bool _multi = false;
		PlateauClass plateau;
		int width, height;
		Random rand = new Random();
		SpriteFont font, font_bold, font_petit;
		Police_Size_Manage font_manage;
		Statut_Partie _statut;
		FinPartieClass _fin_partie;
		AnnimationClass _annim;
		string _side;
		IntroClass _intro;
		No_Connection_POPUP _no_co;

		//POSITION
		float debut_info_Y, debut_plateau_Y, debut_info2_Y;
		Vector2 annimation_gauche_position, annimation_droite_position;

		//QUITTER
		Vector2 _quit_position;
		Texture2D _quit_texture;
		Quit_Game_POPUP _quitter;
		float _scale = 1f;

		//TEXTE
		string computer_string, wait_match_string;
		Languages langue = new Languages();

		//SERVER
		Echange_Server_Class server = new Echange_Server_Class();
		Compteur_Time _timer_check_server_adversaire_play = new Compteur_Time (2000f);
		Compteur_Time _timer_recuperer_info_partie = new Compteur_Time (1000f);
		Compteur_Time _timer_recuperer_temps_partie = new Compteur_Time (5000f);
		bool _recuperer_info_partie = false;
		bool _partie_en_attente = false;
		bool _is_joueur_1 = false;
		bool _ajout_file_d_attente = false;
		string id_joueur_1;
		string _id, _code, _elo, _rank, _name, _id_partie, _id_adversaire, _nom_adversaire, _elo_adversaire, _rank_adversaire, _plateau_debut, info_fin_partie, temps_coup;
		int _statut_partie_next_turn = 0;

		//ATTENTE MATCH VARIABLES
		bool _en_attente_match = false;
		bool _est_dans_salle_attente = false;
		bool _temps_ecoulee_multi = false;
		LoadingSprite _loading;
		Adversaire_Found_POPUP popup_attente_match;
		Compteur_Time _timer_ping_server_attente_match = new Compteur_Time (2000f);

		//ANNIMATION SORTIE
		private enum TypeEcranAnnimation {
			Quitter,
			Rien
		}

		TransitionClass transition = new TransitionClass ();

		//FOND
		Color color_fond = new Color(250,248,239) , font_color = new Color(143,122,102);
		Rectangle r;
		int position_apres_plateau_Y;

		//VARIABLES PARTIE
		public enum Couleurs {
			Red,
			Green
		}

		public Couleurs _couleur_joueur;
		public Couleurs _couleur_adversaire;
		public bool _selection = false;
		const float _timer_tour_float = 20700f;
		Compteur_Time _timer_tour = new Compteur_Time (_timer_tour_float);
		int nombre_point_red = 0, nombre_point_green = 0;

		bool _ping_partie_en_attente = false;
		Compteur_Time _timer_ping_attente_match = new Compteur_Time (2000f);

		public PlayClass()
		{
			EnabledGestures = GestureType.Tap;
			_multi = false;
		}

		public PlayClass (List<string> info_partie, bool multi)
		{
			EnabledGestures = GestureType.Tap;
			_multi = multi;
			_partie_en_attente = true;

			_id_partie = info_partie [0];
			_id_adversaire = info_partie [1];
			_nom_adversaire = info_partie [2];
			_rank_adversaire = info_partie [3];
			_elo_adversaire = info_partie [4];
			_plateau_debut = info_partie [5];
			id_joueur_1 = info_partie [6];

		}

		public PlayClass (Echange_Server_Class Server, bool is_salle_attente)
		{
			EnabledGestures = GestureType.Tap;
			_multi = false;
			server = Server;
			_en_attente_match = true;
			_est_dans_salle_attente = is_salle_attente;
		}

		public override void LoadContent ()
		{
			width = ScreenManager.GraphicsDevice.Viewport.Width;
			height = ScreenManager.GraphicsDevice.Viewport.Height;
			r = new Rectangle (0, 0, width, height);

			computer_string= langue.getString (8);
			wait_match_string = langue.getString (9);

			font_manage = new Police_Size_Manage (height, width, this);
			font = font_manage.Get_Regular_Font ();
			font_bold = font_manage.Get_Bold_Font ();
			font_petit = font_manage.Get_Regular_Petit_Font ();

			debut_info_Y = (float)(height * 0.08);
			debut_plateau_Y = (float)(height * 0.22);
			debut_info2_Y = (float)(width * 0.92 + debut_plateau_Y);

			_quit_texture = ScreenManager.Game.Content.Load<Texture2D> ("cancel");

			_no_co = new No_Connection_POPUP (this);
			_quitter = new Quit_Game_POPUP (this);
			_scale = Calcul_Scale_Texture ();
			_quit_position = new Vector2((float)(width - (_quit_texture.Width*_scale) - width*0.02),(float)( height*0.01 )); 
			position_apres_plateau_Y = (int)(debut_info2_Y);
			_side = (string)IsolatedStorageSettings.ApplicationSettings ["side"];

			//INITIALIZE INFOS JOUEURS	
			_elo = (string)IsolatedStorageSettings.ApplicationSettings ["elo"];
			_rank = (string)IsolatedStorageSettings.ApplicationSettings ["rank"];
			_name = (string)IsolatedStorageSettings.ApplicationSettings ["name"];
			_id = (string)IsolatedStorageSettings.ApplicationSettings ["id"];
			_code = (string)IsolatedStorageSettings.ApplicationSettings ["code"];

			if (_rank == null) {
				_rank = "";
			} 
			if (_elo == null) {
				_elo = "";
			}

			if (id_joueur_1 != null && id_joueur_1 == _id) {
				_is_joueur_1 = true;
			}

			if (_side == "1") { // RED
				_couleur_joueur = Couleurs.Red;
				_couleur_adversaire = Couleurs.Green;
			} else {
				_couleur_joueur = Couleurs.Green;
				_couleur_adversaire = Couleurs.Red;
			}

			annimation_droite_position = new Vector2((float)(width*0.97 - width*0.4),(float)(debut_info_Y));
			annimation_gauche_position = new Vector2((float)(width * 0.03), (float)(debut_info_Y));
			_annim = new AnnimationClass (this, _couleur_joueur, annimation_gauche_position, annimation_droite_position);
			//INITIALISATION TOUR
			if (!_multi) {
				plateau = new PlateauClass (this, (int)(debut_plateau_Y), _couleur_joueur, _side);

				int k = rand.Next (0, 2);
				if (k == 0) {
					_statut = Statut_Partie.En_Attente;
					_annim.Changement_Tour (true);
				} else {
					_statut = Statut_Partie.Tour_Adversaire;
					Tour_Adversaire (_couleur_adversaire);
				}
			} else {
				plateau = new PlateauClass (this, (int)(debut_plateau_Y), _couleur_joueur,_is_joueur_1, _side, _plateau_debut);
			}

			if (_multi) {

				if (_side == "0") {
					string[] data = new string[] { _nom_adversaire, _rank_adversaire, _elo_adversaire, _name, _rank, _elo };
					_intro = new IntroClass (this, data);
				} else {
					string[] data = new string[] { _name, _rank, _elo, _nom_adversaire, _rank_adversaire, _elo_adversaire };
					_intro = new IntroClass (this, data);
				}

			} else {
				if (_side == "0") {
					string[] data = new string[] { computer_string, "", "", _name, _rank, _elo };
					_intro = new IntroClass (this, data);
				} else {
					string[] data = new string[] { _name, _rank, _elo, computer_string, "", "" };
					_intro = new IntroClass (this, data);
				}
			}

			if (!_multi) {
				_nom_adversaire = computer_string;
				_rank_adversaire = "";
				_elo_adversaire = "";
				if (_en_attente_match) {
					_loading = new LoadingSprite (this, (int)(font_petit.MeasureString ("GORGE").Y * font_manage._scale / 2), new Vector2 ((float)(width * 0.4), (float)(height * 0.02)), font_color);
					popup_attente_match = new Adversaire_Found_POPUP (this, server);
				}
			}

			Texte_Variable_Point_Classement ();


			if (_partie_en_attente) {
				server.Joueur_Pret_Partie (_id, _id_partie, _code);
			}

			nombre_point_red = plateau.Nombre_cases_Red ();
			nombre_point_green = plateau.Nombre_cases_Green ();
			base.LoadContent ();
		}

		private void Texte_Variable_Point_Classement()
		{
			if (langue.lang == "FR") {
				if (_rank_adversaire != "") {
					if (_rank_adversaire == "1") {
						_rank_adversaire = _rank_adversaire + "er";
					}else {
						_rank_adversaire = _rank_adversaire + "ème";
					}
				}
				if (_rank != "") {
					if (_rank == "1") {
						_rank = _rank + "er";
					} else {
						_rank = _rank + "ème";
					}
				}

				if (_elo != "") {
					_elo = _elo + "pts";
				}
				if (_elo_adversaire != "") {
					_elo_adversaire = _elo_adversaire + "pts";
				}
			} else {
				if (_rank_adversaire != "") {
					if (_rank_adversaire == "1") {
						_rank_adversaire = _rank_adversaire + "st";
					} else if (_rank_adversaire == "2") {
						_rank_adversaire = _rank_adversaire + "nd";
					} else {
						_rank_adversaire = _rank_adversaire + "th";
					}
				}
				if (_rank != "") {
					if (_rank == "1") {
						_rank = _rank + "st";
					} else if (_rank == "2") {
						_rank = _rank + "nd";
					} else {
						_rank = _rank + "th";
					}
				}

				if (_elo != "") {
					_elo = _elo + "pts";
				}
				if (_elo_adversaire != "") {
					_elo_adversaire = _elo_adversaire + "pts";
				}
			}
		}

		private void Nouvelle_Partie()
		{
			nombre_point_red = 0;
			nombre_point_green = 0;
			_timer_tour = new Compteur_Time (_timer_tour_float);
			_selection = false;

			_annim = new AnnimationClass(this, _couleur_joueur, annimation_gauche_position, annimation_droite_position);
			plateau = new PlateauClass (this, (int)(debut_plateau_Y), _couleur_joueur, _side);
			int k = rand.Next (0, 2);
			if (k == 0) {
				_statut = Statut_Partie.En_Attente;
				_annim.Changement_Tour (true);
			} else {
				_statut = Statut_Partie.Tour_Adversaire;
				Tour_Adversaire (_couleur_adversaire);
			}

			_fin_partie._statut= FinPartieClass.Statut_Fin.Attente;

			if (_side == "0") {
				string[] data = new string[] { computer_string, "", "", _name, _rank, _elo };
				_intro = new IntroClass (this, data);
			} else {
				string[] data = new string[] { _name, _rank, _elo, computer_string, "", "" };
				_intro = new IntroClass (this, data);
			}

			nombre_point_red = plateau.Nombre_cases_Red ();
			nombre_point_green = plateau.Nombre_cases_Green ();

			if (!_multi) {
				_nom_adversaire = computer_string;
				_rank_adversaire = "";
				_elo_adversaire = "";
			}
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

		public override void UnloadContent ()
		{
			base.UnloadContent ();
		}

		public override void HandleInput (InputState input)
		{
			if (_multi || _en_attente_match) {
				if (_no_co._statut != No_Connection_POPUP.Statut_Popup.Wait) {
					_no_co.Input (input);
					if (_no_co._statut == No_Connection_POPUP.Statut_Popup.Option_1) {
						_no_co._statut = No_Connection_POPUP.Statut_Popup.Wait;
						server.CloseConnection ();
						server = new Echange_Server_Class ();
						if (_multi) {
							server.Obtenir_info_partie_en_cours (_id_partie, _id);
						} else if (popup_attente_match.is_active) {
							popup_attente_match.is_active = false;
							_en_attente_match = false;
						} else {
							_en_attente_match = false;
						}
					} else if (_no_co._statut == No_Connection_POPUP.Statut_Popup.Option_2) {
						server.CloseConnection ();
						this.ExitScreen ();
						ScreenManager.AddScreen (new MainMenuScreen ());
					}
				}
			}

			if ((popup_attente_match == null || popup_attente_match.is_active == false) && _quitter._statut == Quit_Game_POPUP.Statut_Popup.Wait && _no_co._statut == No_Connection_POPUP.Statut_Popup.Wait) {
				if (_statut == Statut_Partie.En_Attente) {
					if (plateau.HandleInput_Attente (input, _statut)) {
						_statut = Statut_Partie.Selection;
					}
				} else if (_statut == Statut_Partie.Selection) {
					if (_multi) {
						string data = plateau.HandleInput_Selection_Multi (input, _statut);
						if (data != "") {
							server.Annulation_Transfert ();
							server.Joueur_Jouer_Coup (_id, _id_partie, data, _code);
						} else if (Tap_Ecran (input)) {
							plateau.Changement_Statut_Selected_To_Nothing ();
							_statut = Statut_Partie.En_Attente;
						}
					} else {
						if (plateau.HandleInput_Selection (input, _statut)) {
						} else if (Tap_Ecran (input)) {
							plateau.Changement_Statut_Selected_To_Nothing ();
							_statut = Statut_Partie.En_Attente;
						}
					}
				} else if (_statut == Statut_Partie.Tour_Adversaire) {
				} else if (_statut == Statut_Partie.Fin_Partie_Loose || _statut == Statut_Partie.Fin_Partie_Win || _statut == Statut_Partie.Fin_Partie_Draw) {
					_fin_partie.Input (input);

					if (_fin_partie._statut == FinPartieClass.Statut_Fin.Menu) {
						server.CloseConnection ();
						this.ExitScreen ();
						ScreenManager.AddScreen (new MainMenuScreen ());
					} else if (_fin_partie._statut == FinPartieClass.Statut_Fin.Rejouer && _multi) {
						server.CloseConnection ();
						this.ExitScreen ();
						ScreenManager.AddScreen (new RechercheAdversaireClass ());
					} else if (_fin_partie._statut == FinPartieClass.Statut_Fin.Rejouer && !_multi) {
						Nouvelle_Partie ();
					}
				}
			}
			//ATTENTE MATCH
			else if (_en_attente_match && popup_attente_match.is_active) {
				popup_attente_match.Input (input);
			}

			//QUITTER BUTTON
			if (_quitter._statut == Quit_Game_POPUP.Statut_Popup.Wait) {
				Abandonner (input, _quit_position, _quit_texture);
			} else {
				_quitter.Input (input);
			}

			base.HandleInput (input);
		}

		private void Abandonner(InputState input, Vector2 position_abandon, Texture2D texture_abandon)
		{
			foreach (GestureSample gesture in input.Gestures) {
				if (gesture.GestureType == GestureType.Tap) {
					if (gesture.Position.X > position_abandon.X &&
						gesture.Position.X < position_abandon.X + (texture_abandon.Width * _scale) &&
						gesture.Position.Y > position_abandon.Y &&
						gesture.Position.Y < position_abandon.Y + (texture_abandon.Height * _scale)) {
						_quitter._statut = Quit_Game_POPUP.Statut_Popup.Active;
					}
				}
			}
		}

		private bool Tap_Ecran(InputState input)
		{
			foreach (GestureSample gesture in input.Gestures) {
				if (gesture.GestureType == GestureType.Tap) {
					if (gesture.Position.X > 0 &&
						gesture.Position.X < width &&
						gesture.Position.Y > 0 &&
						gesture.Position.Y < height) {
						return true;
					}
				}
			}
			return false;
		}

		public override void Update (GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
		{
			if (GamePad.GetState (PlayerIndex.One).Buttons.Back == ButtonState.Pressed) {
				Quitter ();
			}

			float timer = (float)gameTime.ElapsedGameTime.TotalMilliseconds;

			_no_co.Update (timer, server);
			if (_no_co._statut == No_Connection_POPUP.Statut_Popup.Wait) {
				//INTRO
				if (_intro._statut != IntroClass.Statut_intro.End) {
					_intro.Update (timer);
				} else {
					transition.Update_Transition (timer);
					Gestion_Temps_Tour_Update (timer);
					_annim.Update_Annim (timer);

					Couleurs tour_de_jouer = Couleurs.Green;

					if (_statut == Statut_Partie.Tour_Adversaire && _couleur_adversaire == Couleurs.Red) {
						tour_de_jouer = Couleurs.Red;
					} else if ((_statut == Statut_Partie.Selection || _statut == Statut_Partie.En_Attente) && _couleur_joueur == Couleurs.Red) {
						tour_de_jouer = Couleurs.Red;
					}
					plateau.Update_Annimation (timer, tour_de_jouer);
					Gestion_Changement_Tour ();

					if (_ping_partie_en_attente) {
						if (!server.Echange_en_cours) {
							if (_timer_ping_attente_match.IncreaseTimer (timer)) {
								server.Joueur_Pret_Partie (_id, _id_partie, _code);
							}
						}
					}
					if (_recuperer_info_partie) {
						if (!server.Echange_en_cours) {
							if (_timer_recuperer_info_partie.IncreaseTimer (timer)) {
								server.Obtenir_info_partie_en_cours (_id_partie, _id);
								_recuperer_info_partie = false;
							}
						}
					}

					if (_multi && (_statut == Statut_Partie.En_Attente || _statut == Statut_Partie.Selection)) {
						if (_timer_recuperer_temps_partie.IncreaseTimer (timer)) {
							server.Recuperer_Temps_Partie (_id_partie,_id);
						}
					}
				}

				//ATTENTE MATCH UPDATE
				if (_en_attente_match) {
					if (popup_attente_match.is_active) {
						popup_attente_match.Update (timer);

						if (popup_attente_match.statut == Adversaire_Found_POPUP.Statut_Pop_Up.Partie_found) {
							popup_attente_match.statut = Adversaire_Found_POPUP.Statut_Pop_Up.Attente;
							server.CloseConnection ();
							this.ExitScreen ();
							ScreenManager.AddScreen (new PlayClass (popup_attente_match.data_game, true));
						} else if (popup_attente_match.statut == Adversaire_Found_POPUP.Statut_Pop_Up.Refuse) {
							popup_attente_match.is_active = false;
							_en_attente_match = false;
						} else if (popup_attente_match.statut == Adversaire_Found_POPUP.Statut_Pop_Up.Retour_liste_attente) {
							popup_attente_match = new Adversaire_Found_POPUP (this, server);
							server.Ajout_Joueur_Liste_Attente (_id, _code);
							_ajout_file_d_attente = true;
						} else if (popup_attente_match.statut == Adversaire_Found_POPUP.Statut_Pop_Up.Retour_Menu) {
							popup_attente_match.is_active = false;
							this.ExitScreen ();
							ScreenManager.AddScreen (new MainMenuScreen ());
						}
					} else {
						_loading.Update (timer);
						if (!server.Echange_en_cours && _timer_ping_server_attente_match.IncreaseTimer (timer)) {
							server.Verifier_Partie_trouver (_id, _code);
						}
						if (server.Info_Attente_Check ()) {
							string blbl = server.Recuperer_Info ();
							string[] data = blbl.Split (' ');
							if (_ajout_file_d_attente) {
								if (data [0] == "true") {
									_ajout_file_d_attente = false;
									server.Verifier_Partie_trouver (_id, _code);
								} else {
									server.Ajout_Joueur_Liste_Attente (_id, _code);
								}
							} else {
								if (data [0] == "true") {
									popup_attente_match.is_active = true;
									popup_attente_match._id_partie_found = data [1];
								}
							}
						}
					}
				}

				//QUITTER SCREEN
				if (_quitter._statut == Quit_Game_POPUP.Statut_Popup.Option_1) {
					if (_multi) {
						_quitter._multi_quit_partie = true;
						server.Annulation_Transfert ();
						server.Abandonner_Partie (_id, _id_partie, _code);
					} else if (_en_attente_match) {
						this.ExitScreen ();
						ScreenManager.AddScreen (new RechercheAdversaireClass (server));
					} else {
						this.ExitScreen ();
						ScreenManager.AddScreen (new MainMenuScreen ());
					}

					_quitter._statut = Quit_Game_POPUP.Statut_Popup.Wait;
				} else if (_quitter._statut == Quit_Game_POPUP.Statut_Popup.Option_2) {
					_quitter._statut = Quit_Game_POPUP.Statut_Popup.Wait;
				}

				if (_quitter._multi_quit_partie) {
					if (server.Info_Attente_Check ()) {
						string blbl = server.Recuperer_Info ();
						string[] data = blbl.Split (' ');
						if (data [0] == "true") {
							_quitter._statut = Quit_Game_POPUP.Statut_Popup.Wait;
							server.CloseConnection ();
							this.ExitScreen ();
							ScreenManager.AddScreen (new MainMenuScreen ());
						}
					}
				}

				if (_multi && _quitter._statut == Quit_Game_POPUP.Statut_Popup.Wait) {
					if (server.Info_Attente_Check ()) {
						string blbl = server.Recuperer_Info ();
						string[] data = blbl.Split (' ');
						string[] caca = new string[data.Length - 1];

						Array.Copy (data, 1, caca, 0, data.Length - 1);

						switch (int.Parse (data [0])) {
						case 5:
							MULTI_Partie_En_Attente (caca);
							break;

						case 6:
							MULTI_Coup_Joueur_Confirmation (caca);
							break;

						case 7:
							MULTI_Recuperation_Coup_Adversaire (caca);
							break;

						case 10:
							MULTI_Recuperation_Info_Partie (caca);
							break;

						case 12:
							MULTI_Recuperation_Temps_Partie (caca);
							break;
						}
					} else if (_statut == Statut_Partie.Tour_Adversaire && !_temps_ecoulee_multi) {
						if (_timer_check_server_adversaire_play.IncreaseTimer (timer)) {
							server.Verifier_Adversaire_Jouer (_id, _id_partie, _code);
						}
					}
				}
			}

			base.Update (gameTime, otherScreenHasFocus, coveredByOtherScreen);
		}

		private void MULTI_Recuperation_Temps_Partie(string[] array_data)
		{
			Reset_Temps_Multi_Ecoulee (int.Parse (array_data [0]));
		}

		private void MULTI_Partie_En_Attente(string[] array_data)
		{
			if (array_data [0] == "true") {
				_partie_en_attente = false;
				_ping_partie_en_attente = false;
				if ((array_data [1] == "3" && _is_joueur_1) || (array_data [1] == "4" && !_is_joueur_1)) {
					_statut = Statut_Partie.En_Attente;
					_annim.Changement_Tour (true);
					Reset_Temps_Multi (array_data [2], true);
				} else {
					_statut = Statut_Partie.Tour_Adversaire;
					_annim.Changement_Tour (false);
					Reset_Temps_Multi (array_data [2], true);
				}
			} else {
				_ping_partie_en_attente = true;
			}
		}

		private void MULTI_Recuperation_Coup_Adversaire(string[] data)
		{
			if (data [0] == "false") {
				Verification_Plateau (data [1]);
				Reset_Temps_Multi_Ecoulee (int.Parse (data [2]));
			} else {

				_statut_partie_next_turn = int.Parse (data [3]);
				string code_plateau = "";
				if (_is_joueur_1) {
					code_plateau = "4";
				} else {
					code_plateau = "3";
				}

				if (data [4] != "-1" && _statut_partie_next_turn == 2) {
					info_fin_partie = data [4];
				}

				string[] caca = data [2].Split ('_');
				temps_coup = caca [4];

				plateau.Reponse_server_multi_coup_jouer (code_plateau, data [1], data [2]);

			}
		}

		private void MULTI_Coup_Joueur_Confirmation(string[] array_data)
		{
			if (array_data [0] == "true") {
				_statut_partie_next_turn = int.Parse (array_data [1]);

				string code_plateau = "";
				string[] liche = array_data [4].Split ('_');

				if (_is_joueur_1) {
					code_plateau = "3";
				} else {
					code_plateau = "4";
				}

				if (array_data [2] != "-1" && array_data [1] == "2") {
					info_fin_partie = array_data [2];
				}

				temps_coup = liche [4];

				plateau.Reponse_server_multi_coup_jouer (code_plateau, array_data [3], array_data [4]);
			}
		}

		private void MULTI_Recuperation_Info_Partie (string[] array_data)
		{
			if (_statut == Statut_Partie.En_Attente || _statut == Statut_Partie.Selection) {
				if (_is_joueur_1 && array_data [2] == "3" || !_is_joueur_1 && array_data [2] == "4") {
					_recuperer_info_partie = true;
				}
			}
			else if (_statut == Statut_Partie.Tour_Adversaire) {
				if (_is_joueur_1 && array_data [2] == "4" || !_is_joueur_1 && array_data [2] == "3") {
					_recuperer_info_partie = true;
				}
			}


			if (array_data [0] == "true") {
				if (!_recuperer_info_partie) {
					_temps_ecoulee_multi = false;
					if (array_data [2] == "2") {
						Gestion_Fin_Partie (array_data [5]);
					} else if ((array_data [2] == "3" && _is_joueur_1) || (array_data [2] == "4" && !_is_joueur_1)) {
						plateau.Reset_Plateau (array_data [3]);
						Logic_Tour_Changement (_couleur_joueur, array_data [4], true);
						_statut = Statut_Partie.En_Attente;
					} else {
						plateau.Reset_Plateau (array_data [3]);
						Logic_Tour_Changement (_couleur_adversaire, array_data [4], true);
						_statut = Statut_Partie.Tour_Adversaire;
					}
				}
			}
		}
		

		private void Verification_Plateau(string plateau_server)
		{
			if (plateau_server != plateau.Plateau_To_String ()) {
				plateau.Reset_Plateau (plateau_server);
			}
		}

		private void Reset_Temps_Multi_Ecoulee(int temps)
		{
			_timer_tour._timer = _timer_tour._timer_max - (temps * 1000);
		}

		private void Gestion_Fin_Partie()
		{
			if (_statut == Statut_Partie.Fin_Partie_Loose || _statut == Statut_Partie.Fin_Partie_Win || _statut == Statut_Partie.Fin_Partie_Draw) {
				if (_statut == Statut_Partie.Fin_Partie_Loose) {
					_fin_partie = new FinPartieClass (this, false, false, nombre_point_red.ToString(), nombre_point_green.ToString());
				} else if (_statut == Statut_Partie.Fin_Partie_Draw) {
					_fin_partie = new FinPartieClass (this, false, true, nombre_point_red.ToString(), nombre_point_green.ToString());
				} else {
					_fin_partie = new FinPartieClass (this, true, false, nombre_point_red.ToString(), nombre_point_green.ToString());
				}
			}
		}

		private void Gestion_Fin_Partie(string info_fin_partie)
		{
			string[] data = info_fin_partie.Split ('_');
			bool is_joueur_1 = false;
			if (data [3] == _id) {
				is_joueur_1 = true;
			}

			if (data [2] == "1") { //WIN
				if (is_joueur_1) {
					_statut = Statut_Partie.Fin_Partie_Win;
					_fin_partie = new FinPartieClass (this, info_fin_partie);
				} else {
					_statut = Statut_Partie.Fin_Partie_Loose;
					_fin_partie = new FinPartieClass (this, info_fin_partie);
				}
			} else if (data [2] == "3") { // LOOSE
				if (is_joueur_1) {
					_statut = Statut_Partie.Fin_Partie_Loose;
					_fin_partie = new FinPartieClass (this, info_fin_partie);
				} else {
					_statut = Statut_Partie.Fin_Partie_Win;
					_fin_partie = new FinPartieClass (this, info_fin_partie);
				}
			} else {
				_statut = Statut_Partie.Fin_Partie_Draw;
				_fin_partie = new FinPartieClass (this, info_fin_partie);
			}
		}

		private void Gestion_Changement_Tour()
		{
			if (plateau._tour_changement) {
				if (!_multi) {
					if (_statut == Statut_Partie.Selection) {
						Logic_Tour_Changement (_couleur_adversaire);
						if (_statut != Statut_Partie.Fin_Partie_Loose && _statut != Statut_Partie.Fin_Partie_Win && _statut != Statut_Partie.Fin_Partie_Draw) {
							_statut = Statut_Partie.Tour_Adversaire;
						} else if (_statut == Statut_Partie.Fin_Partie_Loose || _statut == Statut_Partie.Fin_Partie_Win || _statut == Statut_Partie.Fin_Partie_Draw) {
							Gestion_Fin_Partie ();
						}
					} else if (_statut == Statut_Partie.Tour_Adversaire) {
						Logic_Tour_Changement (_couleur_joueur);
						if (_statut != Statut_Partie.Fin_Partie_Loose && _statut != Statut_Partie.Fin_Partie_Win && _statut != Statut_Partie.Fin_Partie_Draw) {
							_statut = Statut_Partie.En_Attente;
						} else if (_statut == Statut_Partie.Fin_Partie_Loose || _statut == Statut_Partie.Fin_Partie_Win || _statut == Statut_Partie.Fin_Partie_Draw) {
							Gestion_Fin_Partie ();
						}
					}
				} else {
					if (_statut_partie_next_turn == 2) {
						Gestion_Fin_Partie (info_fin_partie);
					} else if ((_statut_partie_next_turn == 3 && _is_joueur_1) || (_statut_partie_next_turn == 4 && !_is_joueur_1)) {
						Logic_Tour_Changement (_couleur_joueur, temps_coup, false);
						_statut = Statut_Partie.En_Attente;
					} else {
						Logic_Tour_Changement (_couleur_adversaire, temps_coup, false);
						_statut = Statut_Partie.Tour_Adversaire;
					}
				}

				plateau._tour_changement = false;
			}
		}

		private void Gestion_Temps_Tour_Update(float timer)
		{
			if (_statut != Statut_Partie.Fin_Partie_Loose && _statut != Statut_Partie.Fin_Partie_Win && _statut != Statut_Partie.Fin_Partie_Draw) {
				if (!plateau._coup_jouer) {
					if (!_multi) {
						if (_timer_tour.IncreaseTimer (timer)) {
							if (_statut == Statut_Partie.En_Attente || _statut == Statut_Partie.Selection) {
								_statut = Statut_Partie.Tour_Adversaire;
								Logic_Tour_Changement (_couleur_adversaire);
							} else {
								_statut = Statut_Partie.En_Attente;
								Logic_Tour_Changement (_couleur_joueur);
							}
						}
					} else {
						if (!_temps_ecoulee_multi) {
							if (_timer_tour.IncreaseTimer (timer)) {
								_temps_ecoulee_multi = true;
								server.Obtenir_info_partie_en_cours (_id_partie, _id);
							}
						}
					}
				}
			}
		}

		private void Logic_Tour_Changement(Couleurs tour_de_jouer)
		{
			Reset_Temps ();
			Tour_Adversaire (tour_de_jouer);
			nombre_point_red = plateau.Nombre_cases_Red ();
			nombre_point_green = plateau.Nombre_cases_Green ();
		}

		private void Logic_Tour_Changement(Couleurs tour_de_jouer, string temps, bool _temps_ecoulee)
		{
			Reset_Temps_Multi (temps, _temps_ecoulee);
			Tour_Adversaire (tour_de_jouer);
			nombre_point_red = plateau.Nombre_cases_Red ();
			nombre_point_green = plateau.Nombre_cases_Green ();
		}

		private void Reset_Temps_Multi(string temps_coup,bool _temps_play_before)
		{
			if (!_temps_play_before) {// temps_coup = temps du dernier coup 
				string[] data = temps_coup.Split (':');
				int coup_min = int.Parse (data [0]);
				int coup_sec = int.Parse (data [1]);

				int actual_min = DateTime.Now.Minute;
				int actual_sec = DateTime.Now.Second;

				int coup_sec_total = coup_min * 60 + coup_sec;
				int actual_sec_total = actual_min * 60 + actual_sec;

				if (actual_sec_total >= coup_sec_total) {
					_timer_tour._timer = (actual_sec_total - coup_sec_total);
				} else {
					int blblbl = 3600 - coup_sec_total + actual_sec_total;
					if (blblbl < 21) {
						_timer_tour._timer = blblbl;
					} else {
						Reset_Temps ();
					}
				}

				temps_coup = "";
			} else {//temps_coup = next_play_before_this_time

				string[] data = temps_coup.Split (':');
				int coup_min = int.Parse (data [0]);
				int coup_sec = int.Parse (data [1]);

				int actual_min = DateTime.Now.Minute;
				int actual_sec = DateTime.Now.Second;

				int coup_sec_total = coup_min * 60 + coup_sec;
				int actual_sec_total = actual_min * 60 + actual_sec;

				if (actual_sec_total <= coup_sec_total) {//TODO
					_timer_tour._timer = (coup_sec_total - actual_sec_total);
				} else {
					int blblbl = 3600 - actual_sec_total + coup_sec_total;
					if (blblbl < 21) {
						_timer_tour._timer = blblbl;
					} else {
						Reset_Temps ();
					}
				}

				temps_coup = "";
			}
		}



		private void Tour_Adversaire(Couleurs tour_de_jouer)
		{
			bool tour_du_joueur = false;
			CaseClass.Type_Case type_case;
			CaseClass.Type_Case type_case_autre;

			if (tour_de_jouer == _couleur_joueur) {
				tour_du_joueur = true;
			}
			if (tour_de_jouer == Couleurs.Green) {
				type_case = CaseClass.Type_Case.Green;
				type_case_autre = CaseClass.Type_Case.Red;
			} else {
				type_case = CaseClass.Type_Case.Red;
				type_case_autre = CaseClass.Type_Case.Green;
			}
			_annim.Changement_Tour (tour_du_joueur);

			if (!_multi) {
				if (!plateau.Verification_possibilite_jouer (type_case)) {
					plateau.Remplissage_plateau_fin_partie (type_case_autre);

					nombre_point_red = plateau.Nombre_cases_Red ();
					nombre_point_green = plateau.Nombre_cases_Green ();

					if (nombre_point_red == nombre_point_green) {
						_statut = Statut_Partie.Fin_Partie_Draw;
					} else {
						if ((_couleur_joueur == Couleurs.Green && nombre_point_green > nombre_point_red) || (_couleur_joueur == Couleurs.Red && nombre_point_green < nombre_point_red)) {
							_statut = Statut_Partie.Fin_Partie_Win;
						} else {
							_statut = Statut_Partie.Fin_Partie_Loose;
						}
					}

				} else if (!tour_du_joueur) {
					plateau.Tour_Adversaire_IA ();
				}
			}
		}

		private void Reset_Temps()
		{
			_timer_tour = new Compteur_Time (_timer_tour_float);
		}

		private void Quitter()
		{
			this.ExitScreen ();
			ScreenManager.AddScreen (new MainMenuScreen ());
		}

		public override void Draw (GameTime gameTime)
		{
			ScreenManager.SpriteBatch.Begin ();

			//FOND
			ScreenManager.SpriteBatch.Draw (ScreenManager.BlankTexture, r, color_fond);

			//PLATEAU
			plateau.Draw (transition);

			if (_multi) {
				_annim.Draw_Annim (_is_joueur_1);
			} else {
				_annim.Draw_Annim ();
			}

			Draw_Interface ();
			ScreenManager.SpriteBatch.Draw (_quit_texture, _quit_position, new Rectangle (0,0,(int)(_quit_texture.Width), (int)(_quit_texture.Height)), Color.White, 0f, new Vector2 (0, 0), _scale, SpriteEffects.None, 1f);


			if (_statut == Statut_Partie.Fin_Partie_Loose || _statut == Statut_Partie.Fin_Partie_Win || _statut == Statut_Partie.Fin_Partie_Draw) {
				_fin_partie.Draw ();
			}

			if (_en_attente_match) {
				_loading.Draw (1f);
				if (popup_attente_match.is_active) {
					popup_attente_match.Draw ();
				}
				ScreenManager.SpriteBatch.DrawString (font_petit, wait_match_string, new Vector2 ((float)(_loading._position.X + _loading._taille *1.5),(float)( _loading._position.Y - _loading._taille)), font_color, 0f, Vector2.Zero, font_manage._scale, SpriteEffects.None, 1f);
			}

			//INTRO
			if (_intro._statut != IntroClass.Statut_intro.End) {
				_intro.Draw ();
			}

			_no_co.Draw ();
			_quitter.Draw ();

			ScreenManager.SpriteBatch.End ();

			base.Draw (gameTime);
		}

		private void Draw_Interface()
		{
			float position_left_X = (float)(width * 0.05);
			float position_right_X = (float)(width * 0.95);

			float position_1Y = (float)(debut_info_Y);
			float position_2Y = (float)(debut_info_Y + height * 0.04);
			float position_3Y = (float)(debut_info_Y + height * 0.075);

			SpriteFont _name_font = font;
			SpriteFont _name_adversaire_font = font;

			if (font.MeasureString (_name).X * font_manage._scale > width * 0.4) {
				_name_font = font_petit;
			}
			if (!_en_attente_match && font.MeasureString (_nom_adversaire).X *font_manage._scale > width * 0.4) {
				_name_adversaire_font = font_petit;
			}

			if (_multi) {
				if (_is_joueur_1) {
					ScreenManager.SpriteBatch.DrawString (_name_font, _name, new Vector2 (position_left_X, position_1Y), font_color, 0f, Vector2.Zero, font_manage._scale, SpriteEffects.None, 1f);
					ScreenManager.SpriteBatch.DrawString (font, _rank, new Vector2 (position_left_X, position_2Y), font_color, 0f, Vector2.Zero, font_manage._scale, SpriteEffects.None, 1f);
					ScreenManager.SpriteBatch.DrawString (font, _elo, new Vector2 (position_left_X, position_3Y), font_color, 0f, Vector2.Zero, font_manage._scale, SpriteEffects.None, 1f);

					//ADVERSAIRE
					ScreenManager.SpriteBatch.DrawString (_name_adversaire_font, _nom_adversaire, new Vector2 (position_right_X - _name_adversaire_font.MeasureString (_nom_adversaire).X *font_manage._scale, position_1Y), font_color, 0f, Vector2.Zero, font_manage._scale, SpriteEffects.None, 1f);
					ScreenManager.SpriteBatch.DrawString (font, _rank_adversaire, new Vector2 (position_right_X - font.MeasureString (_rank_adversaire).X*font_manage._scale, position_2Y), font_color, 0f, Vector2.Zero, font_manage._scale, SpriteEffects.None, 1f);
					ScreenManager.SpriteBatch.DrawString (font, _elo_adversaire, new Vector2 (position_right_X - font.MeasureString (_elo_adversaire).X*font_manage._scale, position_3Y), font_color, 0f, Vector2.Zero, font_manage._scale, SpriteEffects.None, 1f);

					//POINTS
					if (_couleur_joueur == Couleurs.Red) {
						ScreenManager.SpriteBatch.DrawString (font_bold, nombre_point_red.ToString (), new Vector2 (width / 4 - font_bold.MeasureString (nombre_point_red.ToString ()).X*font_manage._scale / 2, (float)position_apres_plateau_Y), font_color, 0f, Vector2.Zero, font_manage._scale, SpriteEffects.None, 1f);
						ScreenManager.SpriteBatch.DrawString (font_bold, nombre_point_green.ToString (), new Vector2 (width * 3 / 4 - font_bold.MeasureString (nombre_point_green.ToString ()).X*font_manage._scale / 2, (float)position_apres_plateau_Y), font_color, 0f, Vector2.Zero, font_manage._scale, SpriteEffects.None, 1f);
					} else {
						ScreenManager.SpriteBatch.DrawString (font_bold, nombre_point_red.ToString (), new Vector2 (width * 3 / 4 - font_bold.MeasureString (nombre_point_red.ToString ()).X*font_manage._scale / 2, (float)position_apres_plateau_Y), font_color, 0f, Vector2.Zero, font_manage._scale, SpriteEffects.None, 1f);
						ScreenManager.SpriteBatch.DrawString (font_bold, nombre_point_green.ToString (), new Vector2 (width / 4 - font_bold.MeasureString (nombre_point_green.ToString ()).X*font_manage._scale / 2, (float)position_apres_plateau_Y), font_color, 0f, Vector2.Zero, font_manage._scale, SpriteEffects.None, 1f);
					}
				} else {
					ScreenManager.SpriteBatch.DrawString (_name_adversaire_font, _nom_adversaire, new Vector2 (position_left_X, position_1Y), font_color, 0f, Vector2.Zero, font_manage._scale, SpriteEffects.None, 1f);
					ScreenManager.SpriteBatch.DrawString (font, _rank_adversaire, new Vector2 (position_left_X, position_2Y), font_color, 0f, Vector2.Zero, font_manage._scale, SpriteEffects.None, 1f);
					ScreenManager.SpriteBatch.DrawString (font, _elo_adversaire, new Vector2 (position_left_X, position_3Y), font_color, 0f, Vector2.Zero, font_manage._scale, SpriteEffects.None, 1f);

					//ADVERSAIRE
					ScreenManager.SpriteBatch.DrawString (_name_font, _name, new Vector2 (position_right_X - _name_font.MeasureString (_name).X*font_manage._scale, position_1Y), font_color, 0f, Vector2.Zero, font_manage._scale, SpriteEffects.None, 1f);
					ScreenManager.SpriteBatch.DrawString (font, _rank, new Vector2 (position_right_X - font.MeasureString (_rank).X*font_manage._scale, position_2Y), font_color, 0f, Vector2.Zero, font_manage._scale, SpriteEffects.None, 1f);
					ScreenManager.SpriteBatch.DrawString (font, _elo, new Vector2 (position_right_X - font.MeasureString (_elo).X*font_manage._scale, position_3Y), font_color, 0f, Vector2.Zero, font_manage._scale, SpriteEffects.None, 1f);

					//POINTS
					if (_couleur_joueur == Couleurs.Red) {
						ScreenManager.SpriteBatch.DrawString (font_bold, nombre_point_red.ToString (), new Vector2 (width * 3 / 4 - font_bold.MeasureString (nombre_point_red.ToString ()).X*font_manage._scale / 2, (float)position_apres_plateau_Y), font_color, 0f, Vector2.Zero, font_manage._scale, SpriteEffects.None, 1f);
						ScreenManager.SpriteBatch.DrawString (font_bold, nombre_point_green.ToString (), new Vector2 (width / 4 - font_bold.MeasureString (nombre_point_green.ToString ()).X*font_manage._scale / 2, (float)position_apres_plateau_Y), font_color, 0f, Vector2.Zero, font_manage._scale, SpriteEffects.None, 1f);
					} else {
						ScreenManager.SpriteBatch.DrawString (font_bold, nombre_point_red.ToString (), new Vector2 (width / 4 - font_bold.MeasureString (nombre_point_red.ToString ()).X*font_manage._scale / 2, (float)position_apres_plateau_Y), font_color, 0f, Vector2.Zero, font_manage._scale, SpriteEffects.None, 1f);
						ScreenManager.SpriteBatch.DrawString (font_bold, nombre_point_green.ToString (), new Vector2 (width * 3 / 4 - font_bold.MeasureString (nombre_point_green.ToString ()).X*font_manage._scale / 2, (float)position_apres_plateau_Y), font_color, 0f, Vector2.Zero, font_manage._scale, SpriteEffects.None, 1f);
					}
				}
			} else {
				if (_couleur_joueur == Couleurs.Red) {

					ScreenManager.SpriteBatch.DrawString (_name_font, _name, new Vector2 (position_left_X, position_1Y), font_color, 0f, Vector2.Zero, font_manage._scale, SpriteEffects.None, 1f);
					ScreenManager.SpriteBatch.DrawString (font, _rank, new Vector2 (position_left_X, position_2Y), font_color, 0f, Vector2.Zero, font_manage._scale, SpriteEffects.None, 1f);
					ScreenManager.SpriteBatch.DrawString (font, _elo, new Vector2 (position_left_X, position_3Y), font_color, 0f, Vector2.Zero, font_manage._scale, SpriteEffects.None, 1f);

					//ADVERSAIRE
					ScreenManager.SpriteBatch.DrawString (_name_adversaire_font, computer_string, new Vector2 (position_right_X - _name_adversaire_font.MeasureString (computer_string).X*font_manage._scale, position_1Y), font_color, 0f, Vector2.Zero, font_manage._scale, SpriteEffects.None, 1f);
					ScreenManager.SpriteBatch.DrawString (font, "", new Vector2 (position_right_X - font.MeasureString ("").X*font_manage._scale, position_2Y), font_color, 0f, Vector2.Zero, font_manage._scale, SpriteEffects.None, 1f);
					ScreenManager.SpriteBatch.DrawString (font, "", new Vector2 (position_right_X - font.MeasureString ("").X*font_manage._scale, position_3Y), font_color, 0f, Vector2.Zero, font_manage._scale, SpriteEffects.None, 1f);

				} else {
					ScreenManager.SpriteBatch.DrawString (_name_adversaire_font, computer_string, new Vector2 (position_left_X, position_1Y), font_color, 0f, Vector2.Zero, font_manage._scale, SpriteEffects.None, 1f);
					ScreenManager.SpriteBatch.DrawString (font, "", new Vector2 (position_left_X, position_2Y), font_color, 0f, Vector2.Zero, font_manage._scale, SpriteEffects.None, 1f);
					ScreenManager.SpriteBatch.DrawString (font, "", new Vector2 (position_left_X, position_3Y), font_color, 0f, Vector2.Zero, font_manage._scale, SpriteEffects.None, 1f);

					//ADVERSAIRE
					ScreenManager.SpriteBatch.DrawString (_name_font, _name, new Vector2 (position_right_X - _name_font.MeasureString (_name).X*font_manage._scale, position_1Y), font_color, 0f, Vector2.Zero, font_manage._scale, SpriteEffects.None, 1f);
					ScreenManager.SpriteBatch.DrawString (font, _rank, new Vector2 (position_right_X - font.MeasureString (_rank).X*font_manage._scale, position_2Y), font_color, 0f, Vector2.Zero, font_manage._scale, SpriteEffects.None, 1f);
					ScreenManager.SpriteBatch.DrawString (font, _elo, new Vector2 (position_right_X - font.MeasureString (_elo).X*font_manage._scale, position_3Y), font_color, 0f, Vector2.Zero, font_manage._scale, SpriteEffects.None, 1f);
				}
			
				//POINTS
				ScreenManager.SpriteBatch.DrawString (font_bold, nombre_point_red.ToString (), new Vector2 (width / 4 - font_bold.MeasureString (nombre_point_red.ToString ()).X*font_manage._scale / 2, (float)position_apres_plateau_Y), font_color, 0f, Vector2.Zero, font_manage._scale, SpriteEffects.None, 1f);
				ScreenManager.SpriteBatch.DrawString (font_bold, nombre_point_green.ToString (), new Vector2 (width * 3 / 4 - font_bold.MeasureString (nombre_point_green.ToString ()).X*font_manage._scale / 2, (float)position_apres_plateau_Y), font_color, 0f, Vector2.Zero, font_manage._scale, SpriteEffects.None, 1f);
			}

			//TEMPS
			string temps_a_afficher = Secondes (_timer_tour_float - _timer_tour._timer);
			ScreenManager.SpriteBatch.DrawString (font_bold, temps_a_afficher, new Vector2 (width / 2 - font_bold.MeasureString (temps_a_afficher).X*font_manage._scale / 2, (float)position_apres_plateau_Y), font_color, 0f, Vector2.Zero, font_manage._scale, SpriteEffects.None, 1f);
		}

		private string Secondes(float temps)
		{
			double caca = TimeSpan.FromMilliseconds(temps).TotalSeconds;
			int blbl = (int)caca;
			if (caca < 60)
			{
				if (caca < 10)
				{
					return "0" + blbl;
				}
				else if (blbl == 0)
				{
					return "00";
				}
				else
				{
					return blbl.ToString();
				}
			}
			else
			{
				int k = (int)caca / 60;
				blbl = (int)caca - (60 * k);
				if (caca < 10)
				{
					return "0" + blbl;
				}
				else if (blbl == 0)
				{
					return "00";
				}
				else
				{
					return blbl.ToString();
				}
			}
		}
	}
}

