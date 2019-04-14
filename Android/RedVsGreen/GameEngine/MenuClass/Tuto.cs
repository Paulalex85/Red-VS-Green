using System;
using GameStateManagement;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input.Touch;
using System.IO.IsolatedStorage;

namespace RedVsGreen
{
	public class Tuto: GameScreen
	{
		private enum Statut_Annimation_Page
		{
			Page_1,
			Page_2,
			Page_3,
			Page_4
		}

		private enum Phase_Annimation {
			Phase_1,
			Phase_2
		}

		private Phase_Annimation _phase_anim = Phase_Annimation.Phase_1;
		private Statut_Annimation_Page _statut_tuto = Statut_Annimation_Page.Page_1;

		SpriteFont font_bouton, font_titre;
		Police_Size_Manage font_manage;
		int width, height;
		Rectangle blbl;
		Plateau_Intro plateau;
		TransitionClass transition;
		Compteur_Time time = new Compteur_Time (1000f);

		Color color_fond = new Color(250,248,239), color_bouton = new Color(143,122,102), color_texte = new Color(119,110,101), color_green = new Color(71,164,71), color_red = new Color(246,94,59);

		Languages langue = new Languages();
		string how_to_play_string , tap_continue_string, tuto_1,tuto_2,tuto_3,tuto_4;
		string texte_1,texte_2,texte_3,texte_4;

		public Tuto ()
		{
			EnabledGestures = GestureType.Tap;
		}

		public override void LoadContent ()
		{
			width = ScreenManager.GraphicsDevice.Viewport.Width;
			height = ScreenManager.GraphicsDevice.Viewport.Height;

			how_to_play_string = langue.getString (46);
			tap_continue_string = langue.getString (47);
			tuto_1 = langue.getString (48);
			tuto_2 = langue.getString (49);
			tuto_3 = langue.getString (50);
			tuto_4 = langue.getString (51);

			transition = new TransitionClass ();
			transition._transition_alpha = 1f;
			plateau = new Plateau_Intro (this, (int)(height * 0.35));

			font_manage = new Police_Size_Manage (height, width, this);
			font_bouton = font_manage.Get_Regular_Font ();
			font_titre = font_manage.Get_Bold_Font ();

			blbl = new Rectangle ((int)(width * 0.1), (int)(height * 0.125), (int)(width * 0.9), (int)(height * 0.3));
			Divers_Method method = new Divers_Method ();
			texte_1 = method.parseText (tuto_1, font_bouton, blbl, font_manage._scale);
			texte_2 = method.parseText (tuto_2, font_bouton, blbl, font_manage._scale);
			texte_3 = method.parseText (tuto_3, font_bouton, blbl, font_manage._scale);
			texte_4 = method.parseText (tuto_4, font_bouton, blbl, font_manage._scale);

			base.LoadContent ();
		}

		public override void HandleInput (InputState input)
		{
			foreach (GestureSample gesture in input.Gestures) {
				if (gesture.GestureType == GestureType.Tap) {
					if (gesture.Position.X > 0 &&
					    gesture.Position.Y > 0 &&
					    gesture.Position.X < width &&
					    gesture.Position.Y < height) {

						switch (_statut_tuto) {
						case Statut_Annimation_Page.Page_1:
							time = new Compteur_Time (2000f);
							_statut_tuto = Statut_Annimation_Page.Page_2;
							_phase_anim = Phase_Annimation.Phase_1;
							plateau.Changement_Statut_Case_To_Selected (0);
							plateau.Changement_Statut_Selected_To_Validation (1, 0, CaseClass.Type_Case.Red);
							break;
						case Statut_Annimation_Page.Page_2:
							_statut_tuto = Statut_Annimation_Page.Page_3;
							_phase_anim = Phase_Annimation.Phase_1;
							plateau.Reset_Plateau ("333333333333333300000000444444444444444444444444");
							plateau.Changement_Statut_Case_To_Selected (11);
							plateau.Changement_Statut_Selected_To_Validation (19, 11, CaseClass.Type_Case.Red);
							break;
						case Statut_Annimation_Page.Page_3:
							_statut_tuto = Statut_Annimation_Page.Page_4;
							_phase_anim = Phase_Annimation.Phase_1;
							break;
						case Statut_Annimation_Page.Page_4:
							this.ExitScreen ();
							ScreenManager.AddScreen (new MainMenuScreen ());
							break;
						default:
							this.ExitScreen ();
							ScreenManager.AddScreen (new MainMenuScreen ());
							break;
						}
					}
				}
			}
			base.HandleInput (input);
		}

		public override void Update (GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
		{
			float timer = (float)gameTime.ElapsedGameTime.TotalMilliseconds;

			plateau.Update_Annimation (timer, PlayClass.Couleurs.Red);

			if (_statut_tuto == Statut_Annimation_Page.Page_1) {
				if (_phase_anim == Phase_Annimation.Phase_1) {
					if (time.IncreaseTimer (timer)) {
						plateau.Changement_Statut_Case_To_Selected (0);
						_phase_anim = Phase_Annimation.Phase_2;
					}
				} else if (_phase_anim == Phase_Annimation.Phase_2) {
					if (time.IncreaseTimer (timer)) {
						plateau.Changement_Statut_Selected_To_Nothing ();
						_phase_anim = Phase_Annimation.Phase_1;
					}
				}
			} else if (_statut_tuto == Statut_Annimation_Page.Page_2) {
				if (_phase_anim == Phase_Annimation.Phase_1) {
					if (time.IncreaseTimer (timer)) {
						plateau.Reset_Plateau ();
						plateau.Changement_Statut_Case_To_Selected (0);
						_phase_anim = Phase_Annimation.Phase_2;
						plateau.Changement_Statut_Selected_To_Validation (2, 0, CaseClass.Type_Case.Red);
					}
				} else if (_phase_anim == Phase_Annimation.Phase_2) {
					if (time.IncreaseTimer (timer)) {
						plateau.Reset_Plateau ();
						plateau.Changement_Statut_Case_To_Selected (0);
						_phase_anim = Phase_Annimation.Phase_1;
						plateau.Changement_Statut_Selected_To_Validation (1, 0, CaseClass.Type_Case.Red);
					}
				}
			} else if (_statut_tuto == Statut_Annimation_Page.Page_3) {
				if (time.IncreaseTimer (timer)) {
					plateau.Reset_Plateau ("333333333333333300000000444444444444444444444444");
					plateau.Changement_Statut_Case_To_Selected (11);
					plateau.Changement_Statut_Selected_To_Validation (19, 11, CaseClass.Type_Case.Red);
				}
			}


			base.Update (gameTime, otherScreenHasFocus, coveredByOtherScreen);
		}

		public override void Draw (GameTime gameTime)
		{
			ScreenManager.SpriteBatch.Begin ();
			Rectangle r = new Rectangle (0, 0, width, height);
			ScreenManager.SpriteBatch.Draw (ScreenManager.BlankTexture, r, color_fond);

			ScreenManager.SpriteBatch.DrawString (font_titre, how_to_play_string, new Vector2 ((float)(width / 2 - font_titre.MeasureString (how_to_play_string).X*font_manage._scale / 2), (float)(height * 0.05)), color_texte, 0f, Vector2.Zero, font_manage._scale, SpriteEffects.None, 1f);
			ScreenManager.SpriteBatch.DrawString (font_titre, tap_continue_string, new Vector2 ((float)(width / 2 - font_titre.MeasureString (tap_continue_string).X *font_manage._scale/ 2), (float)(height * 0.8)), color_texte, 0f, Vector2.Zero, font_manage._scale, SpriteEffects.None, 1f);

			if (_statut_tuto == Statut_Annimation_Page.Page_1) {
				ScreenManager.SpriteBatch.DrawString (font_bouton, texte_1, new Vector2 (blbl.X, blbl.Y), color_texte, 0f, Vector2.Zero, font_manage._scale, SpriteEffects.None, 1f);
				plateau.Draw (transition);
			} else if (_statut_tuto == Statut_Annimation_Page.Page_2) {
				ScreenManager.SpriteBatch.DrawString (font_bouton, texte_2, new Vector2 (blbl.X, blbl.Y), color_texte, 0f, Vector2.Zero, font_manage._scale, SpriteEffects.None, 1f);
				plateau.Draw (transition);
			} else if (_statut_tuto == Statut_Annimation_Page.Page_3) {
				ScreenManager.SpriteBatch.DrawString (font_bouton, texte_3, new Vector2 (blbl.X, blbl.Y), color_texte, 0f, Vector2.Zero, font_manage._scale, SpriteEffects.None, 1f);
				plateau.Draw (transition);
			} else if (_statut_tuto == Statut_Annimation_Page.Page_4) {
				ScreenManager.SpriteBatch.DrawString (font_bouton, texte_4, new Vector2 (blbl.X, blbl.Y), color_texte, 0f, Vector2.Zero, font_manage._scale, SpriteEffects.None, 1f);
			}

			ScreenManager.SpriteBatch.End ();

			base.Draw (gameTime);
		}
	}
}