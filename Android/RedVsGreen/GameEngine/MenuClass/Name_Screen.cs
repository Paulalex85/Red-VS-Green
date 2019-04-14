using System;
using GameStateManagement;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input.Touch;
using System.IO.IsolatedStorage;
using System.Threading;

namespace RedVsGreen
{
	public class Name_Screen : GameScreen
	{
		private enum Statut_Bouton {
			Normal,
			Attente_Verification,
			Attente_Confirmation,
			Deja_Pris,
			Envoie_Confirmation
		}

		private Statut_Bouton _statut_bouton = Statut_Bouton.Normal;

		int width, height, marge;
		SpriteFont font_1, font_texte, font_keyboard;
		Police_Size_Manage font_manage;
		Texture2D CaseLettre, effacer;
		RoundedRectangle rectangle = new RoundedRectangle();
		Vector2 texte_taille, position_texte, bouton_taille;
		Color color_fond = new Color(250,248,239), color_bouton = new Color(143,122,102),color_case_vide = new Color(205,192,180),color_texte = new Color(119,110,101),
		color_green = new Color(71,164,71),
		color_red = new Color(246,94,59);
		int nombre_max_name = 12;

		Vector2 _quit_position;
		Texture2D _quit_texture;

		Keyboard keyboard;
		string _name = "";
		bool _error_nbr_letters = false;
		bool _option = false;
		bool _error_trop_de_letters = false;
		bool isMaj = true;
		Vector2 position_bouton_1;
		Bouton bouton_1;
		Rectangle r1;
		Echange_Server_Class server = new Echange_Server_Class ();
		LoadingSprite _loading;
		No_Connection_POPUP _no_co;

		//TEXTE 
		Languages langue = new Languages();
		string _name_string , _name_restrict_string , _name_nbr_limit , _name_less_15;
		string check_string, wait_string, wait_creation_string,deja_pris_string, tap_confirm_string;

		public Name_Screen (bool option)
		{
			EnabledGestures = GestureType.Tap;
			_option = option;
		}

		public override void LoadContent ()
		{
			width = ScreenManager.GraphicsDevice.Viewport.Width;
			height = ScreenManager.GraphicsDevice.Viewport.Height;

			font_manage = new Police_Size_Manage (height, width, this);
			font_1 = font_manage.Get_Bold_Font ();
			font_texte = font_manage.Get_Regular_Font ();
			font_keyboard = font_manage.Get_Keyboard_Font ();

			_name_string = langue.getString (29);
			_name_restrict_string = langue.getString (30) + nombre_max_name.ToString () + " " + langue.getString (31);
			_name_nbr_limit = langue.getString (32);
			_name_less_15 = langue.getString (33) + nombre_max_name.ToString () + " " + langue.getString (31);
			check_string = langue.getString (28);
			wait_string = langue.getString (35);
			wait_creation_string = langue.getString (37);
			deja_pris_string = langue.getString (34);
			tap_confirm_string = langue.getString (36);

			_no_co = new No_Connection_POPUP (this);

			effacer = ScreenManager.Game.Content.Load<Texture2D> ("clear");

			texte_taille = new Vector2 ((float)(width * 0.9), (float)(height * 0.08));
			keyboard = new Keyboard ((int)(height * 0.5), font_keyboard, effacer, width, height, font_manage._scale);

			position_texte = new Vector2 ((float)(width /2 - texte_taille.X /2), (float)(height * 0.2));

			CaseLettre = rectangle.Texture_Rounded_Rectangle (ScreenManager.GraphicsDevice, (int)texte_taille.X, (int)texte_taille.Y, Color.White,(int)(texte_taille.Y * 0.1), (int)(texte_taille.Y * 0.1));

			bouton_taille = new Vector2 ((float)(width * 0.8), (float)(height * 0.1));

			position_bouton_1 = new Vector2 ((float)(width * 0.1), (float)(height * 0.3));

			r1 = new Rectangle ((int)(position_bouton_1.X), (int)(position_bouton_1.Y), (int)(bouton_taille.X), (int)(bouton_taille.Y));
			marge = (int)(r1.Height * 0.1);

			bouton_1 = new Bouton (this, r1, font_texte, check_string, marge, 0, Color.White, color_bouton, font_manage._scale);

			_quit_texture = ScreenManager.Game.Content.Load<Texture2D> ("cancel");
			_quit_position = new Vector2((float)(width - (_quit_texture.Width*font_manage._scale) - width*0.02),(float)( height*0.01 )); 

			base.LoadContent ();
		}

		public override void HandleInput (InputState input)
		{
			if (_no_co._statut != No_Connection_POPUP.Statut_Popup.Wait) {
				_no_co.Input (input);
				if (_no_co._statut == No_Connection_POPUP.Statut_Popup.Option_1) {
					_no_co._statut = No_Connection_POPUP.Statut_Popup.Wait;
					Statut_Normal ();
					_name = "";
					keyboard.Changer_en_MAJ ();
					server = new Echange_Server_Class ();
				} else if (_no_co._statut == No_Connection_POPUP.Statut_Popup.Option_2) {
					No_Name ();
				}
			} else {
				keyboard.Input_Keyboard (input);

				foreach (GestureSample gesture in input.Gestures) {
					if (gesture.GestureType == GestureType.Tap) {
						if (_statut_bouton == Statut_Bouton.Normal && bouton_1.Input (gesture.Position)) {
							Check ();
						} else if (_statut_bouton == Statut_Bouton.Attente_Confirmation && bouton_1.Input (gesture.Position)) {
							Statut_Envoie ();
						}

						else if (gesture.Position.X > _quit_position.X &&
							gesture.Position.X < _quit_position.X + (_quit_texture.Width * font_manage._scale) &&
							gesture.Position.Y > _quit_position.Y &&
							gesture.Position.Y < _quit_position.Y + (_quit_texture.Height * font_manage._scale)) {
							No_Name ();
						}
					}
				}
			}

			base.HandleInput (input);
		}

		private void No_Name()
		{
			if (_option) {
				this.ExitScreen ();
				ScreenManager.AddScreen (new MainMenuScreen ());
			} else {
				Random _rand = new Random ();
				int blblb = _rand.Next (1, 1000000);
				if (langue.lang == "FR") {
					IsolatedStorageSettings.ApplicationSettings ["name"] = "Joueur" + blblb.ToString ();
				} else {
					IsolatedStorageSettings.ApplicationSettings ["name"] = "Guest" + blblb.ToString ();
				}
				this.ExitScreen ();
				ScreenManager.AddScreen (new Tuto ());
			}
		}

		public override void Update (Microsoft.Xna.Framework.GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
		{
			float timer = (float)gameTime.ElapsedGameTime.TotalMilliseconds;

			_no_co.Update (timer, server);

			//KEYBOARD
			if (_statut_bouton == Statut_Bouton.Normal || _statut_bouton == Statut_Bouton.Attente_Confirmation || _statut_bouton == Statut_Bouton.Deja_Pris) {
				if (keyboard._tapped) {
					if (keyboard._type_tapped == Keyboard.TypeLettre.Lettre) {
						if (_name.Length < nombre_max_name) {
							if (_name.Length == 0) {
								keyboard.Changer_en_MIN ();
								isMaj = false;
							}
							_name += keyboard._letter_tapped;
							_error_nbr_letters = false;
							if (_statut_bouton == Statut_Bouton.Deja_Pris || _statut_bouton == Statut_Bouton.Attente_Confirmation) {
								Statut_Normal ();
							}
						} else {
							_error_trop_de_letters = true;
						}
					} else if (keyboard._type_tapped == Keyboard.TypeLettre.Effacer) {
						if (_name.Length > 0) {
							_name = _name.Remove (_name.Length - 1, 1);
							if (_statut_bouton == Statut_Bouton.Attente_Confirmation || _statut_bouton == Statut_Bouton.Deja_Pris) {
								Statut_Normal ();
							}
						}
					} else if (keyboard._type_tapped == Keyboard.TypeLettre.Maj_Min) {
						Inverser_MAJ ();
					}

					keyboard.Reboot_Variable ();
				}
			}

			if (server.Info_Attente_Check ()) {
				if (_statut_bouton == Statut_Bouton.Attente_Verification) {
					string data = server.Recuperer_Info ();
					bool result = false;

					if (data == "true") {
						result = true;
					} else if (data == "false") {
						result = false;
					}

					if (result) {
						Statut_Already_Take ();
					} else {
						Statut_Bon ();
					}

				} else if (_statut_bouton == Statut_Bouton.Envoie_Confirmation) {
					string data = server.Recuperer_Info ();
					string[] data_array = data.Split (' ');

					string id = "0", code = "0"; //ASSIGNE 
					id = data_array [0].ToString ();
					code = data_array [1].ToString ();
					IsolatedStorageSettings.ApplicationSettings ["id"] = id;
					IsolatedStorageSettings.ApplicationSettings ["code"] = code;

					Valider ();
				}
			}

			if (_name.Length < nombre_max_name) {
				_error_trop_de_letters = false;
			}

			if (_loading != null & _statut_bouton == Statut_Bouton.Attente_Verification || _statut_bouton == Statut_Bouton.Envoie_Confirmation) {
				_loading.Update (timer);
			}

			keyboard.Update_Timer (timer);

			base.Update (gameTime, otherScreenHasFocus, coveredByOtherScreen);
		}

		private void Statut_Normal()
		{
			bouton_1 = new Bouton (this, r1, font_texte, check_string, marge, 0, Color.White, color_bouton, font_manage._scale);
			_statut_bouton = Statut_Bouton.Normal;
		}

		private void Statut_Already_Take()
		{
			bouton_1 = new Bouton (this, r1, font_texte,deja_pris_string , marge, 0, Color.White, color_red, font_manage._scale);
			_statut_bouton = Statut_Bouton.Deja_Pris;
		}

		private void Statut_Bon()
		{
			bouton_1 = new Bouton (this, r1, font_texte, tap_confirm_string, marge, 0, Color.White, color_green, font_manage._scale);
			_statut_bouton = Statut_Bouton.Attente_Confirmation;
		}
		private void Statut_Loading()
		{
			string caca = wait_string;
			bouton_1 = new Bouton (this, r1, font_texte, caca, marge, 0, Color.White, color_case_vide, font_manage._scale);
			_statut_bouton = Statut_Bouton.Attente_Verification;
			int height_loading = (int)(r1.Height / 2);
			_loading = new LoadingSprite(this, height_loading, new Vector2( r1.X + (r1.Width/2 - font_texte.MeasureString(caca).X*font_manage._scale / 2 - height_loading) ,r1.Y + r1.Height / 2),Color.White);

			server.Check_Name (_name);
		}

		private void Statut_Envoie()
		{
			string caca = wait_creation_string;
			bouton_1 = new Bouton (this, r1, font_texte, caca, marge, 0, Color.White, color_green, font_manage._scale * 0.8f);
			_statut_bouton = Statut_Bouton.Envoie_Confirmation;
			int height_loading = (int)(r1.Height / 4);
			_loading = new LoadingSprite (this, height_loading, new Vector2 ((float)(r1.X + (r1.Width *0.1)), r1.Y + r1.Height / 2), Color.White);

			string side = (string)IsolatedStorageSettings.ApplicationSettings ["side"];
			server.Nouveau_Joueur (_name, side);
		}

		private void Inverser_MAJ()
		{
			if (isMaj) {
				keyboard.Changer_en_MIN ();
				isMaj = false;
			} else {
				keyboard.Changer_en_MAJ ();
				isMaj = true;
			}
		}

		private void Check()
		{
			if (_name.Length < 4 || _name.Length > nombre_max_name) {
				_error_nbr_letters = true;
			} else {
				Statut_Loading ();
			}
		}

		private void Valider()
		{
			IsolatedStorageSettings.ApplicationSettings ["name"] = _name;
			server.CloseConnection ();
			this.ExitScreen ();
			if (_option) {
				ScreenManager.AddScreen (new MainMenuScreen ());
			} else {
				ScreenManager.AddScreen (new Tuto ());
			}
		}

		public override void Draw (GameTime gameTime)
		{
			ScreenManager.SpriteBatch.Begin ();

			ScreenManager.SpriteBatch.Draw (ScreenManager.BlankTexture, new Rectangle (0, 0, width, height), color_fond);


			bouton_1.Draw ();
			keyboard.Draw_Keyboard (this);
			ScreenManager.SpriteBatch.Draw (CaseLettre, position_texte, Color.White);
			ScreenManager.SpriteBatch.DrawString (font_texte, _name, new Vector2 ((float)(position_texte.X + (width * 0.05)), (float)(position_texte.Y + (CaseLettre.Height / 2) - font_texte.MeasureString (_name).Y*font_manage._scale / 2)), Color.Black, 0f, Vector2.Zero, font_manage._scale, SpriteEffects.None, 1f);
			ScreenManager.SpriteBatch.DrawString (font_1,_name_string, new Vector2 ((float)(width / 2 - font_1.MeasureString (_name_string).X*font_manage._scale / 2), (float)(height * 0.1)), color_bouton, 0f, Vector2.Zero, font_manage._scale, SpriteEffects.None, 1f);


			if (_error_nbr_letters) {
				ScreenManager.SpriteBatch.DrawString (font_1, _name_nbr_limit, new Vector2 ((float)(width / 2 - font_1.MeasureString (_name_nbr_limit).X*font_manage._scale / 2), (float)(height * 0.425)), color_red, 0f, Vector2.Zero, font_manage._scale, SpriteEffects.None, 1f);
			} else if (_error_trop_de_letters) {
				ScreenManager.SpriteBatch.DrawString (font_1, _name_less_15, new Vector2 ((float)((width / 2) - (font_1.MeasureString (_name_less_15).X*font_manage._scale / 2)), (float)(height * 0.425)), color_red, 0f, Vector2.Zero, font_manage._scale, SpriteEffects.None, 1f);
			} else {
				ScreenManager.SpriteBatch.DrawString (font_1, _name_restrict_string, new Vector2 ((float)(width / 2 - font_1.MeasureString (_name_restrict_string).X*font_manage._scale / 2), (float)(height * 0.425)), color_bouton, 0f, Vector2.Zero, font_manage._scale, SpriteEffects.None, 1f);
			}

			if (_loading != null & _statut_bouton == Statut_Bouton.Attente_Verification || _statut_bouton == Statut_Bouton.Envoie_Confirmation) {
				_loading.Draw (1f);
			}

			ScreenManager.SpriteBatch.Draw (_quit_texture, _quit_position, new Rectangle (0,0,(int)(_quit_texture.Width), (int)(_quit_texture.Height)), Color.White, 0f, new Vector2 (0, 0), font_manage._scale, SpriteEffects.None, 1f);

			_no_co.Draw ();


			ScreenManager.SpriteBatch.End ();

			base.Draw (gameTime);
		}
	}
}