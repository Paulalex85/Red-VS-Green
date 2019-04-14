using System;
using GameStateManagement;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using System.Collections.Generic;
using System.Threading;

namespace RedVsGreen
{
	public class Plateau_Intro
	{
		//GENERAL ET CONSTANTES
		GameScreen _screen;
		const int NOMBRE_PLATEAU_LARGEUR = 8;
		const int NOMBRE_PLATEAU_HAUTEUR = 6;
		int LARGEUR_PLATEAU;
		int _height_debut;
		Vector2 position_plateau;
		Random rand = new Random();
		char[] _type_plateau;
		int _index_selected = -1;
		bool _multi = false;
		string _plateau_debut;

		public bool _coup_jouer_annimation = false;
		public bool _test_contamination_needed = false;
		public bool _coup_jouer = false; 
		public int _coup_jouer_index = -1;
		public CaseClass.Surbrillance_Type _type_coup_jouer = CaseClass.Surbrillance_Type.Rien;
		public int _origin_saut_coup_jouer = -1;
		public bool _tour_changement = false;

		PlayClass.Couleurs _couleur_joueur;

		//ARRAY
		public CaseClass[] array_case_class;
		List<int> list_index_selected = new List<int>();
		CaseClass.Type_Case _case_joueur;

		//COULEURS
		Color color_fond = new Color(187,173,160), 
		color_case_vide = new Color(205,192,180),
		color_green = new Color(71,164,71),
		color_red = new Color(246,94,59);

		//TEXTURE
		Texture2D plateau_fond, case_vide, case_verte, case_rouge, case_surbrillance_rouge, case_surbrillance_vert;
		RoundedRectangle RoundedRectangle_Class = new RoundedRectangle();
		float _taille_bordure;

		public Plateau_Intro (GameScreen screen, int height_debut)
		{
			_height_debut = height_debut;
			_screen = screen;
			_couleur_joueur = PlayClass.Couleurs.Red;
			_multi = false;
			LoadContent ();
		}

		private void LoadContent ()
		{
			//PLATEAU DIMENSION
			array_case_class = new CaseClass[NOMBRE_PLATEAU_LARGEUR * NOMBRE_PLATEAU_HAUTEUR];
			LARGEUR_PLATEAU = (int)(_screen.ScreenManager.GraphicsDevice.Viewport.Width * 0.9);

			_taille_bordure = Taille_Bordure ();
			int Largueur_Case = Taille_Case ();


			//INITIALISATION TEXTURE
			plateau_fond = RoundedRectangle_Class.Texture_Rounded_Rectangle (_screen.ScreenManager.GraphicsDevice, LARGEUR_PLATEAU, (int)(LARGEUR_PLATEAU * 0.75), color_fond, (int)(LARGEUR_PLATEAU * 0.005), (int)(LARGEUR_PLATEAU * 0.03));
			case_vide = RoundedRectangle_Class.Texture_Rounded_Rectangle (_screen.ScreenManager.GraphicsDevice, Largueur_Case, Largueur_Case, color_case_vide, (int)(LARGEUR_PLATEAU * 0.005), (int)(LARGEUR_PLATEAU * 0.03));
			case_verte = RoundedRectangle_Class.Texture_Rounded_Rectangle (_screen.ScreenManager.GraphicsDevice, Largueur_Case, Largueur_Case, color_green, (int)(LARGEUR_PLATEAU * 0.005), (int)(LARGEUR_PLATEAU * 0.03));
			case_rouge = RoundedRectangle_Class.Texture_Rounded_Rectangle (_screen.ScreenManager.GraphicsDevice, Largueur_Case, Largueur_Case, color_red, (int)(LARGEUR_PLATEAU * 0.005), (int)(LARGEUR_PLATEAU * 0.03));

			//INITIALISATION TEXTURE SURBRILLANCE SAUT

			case_surbrillance_vert = RoundedRectangle_Class.Texture_Rounded_Rectangle_Saut_Cercle (_screen.ScreenManager.GraphicsDevice, Largueur_Case, Largueur_Case, color_green, color_case_vide, (int)(LARGEUR_PLATEAU * 0.005), (int)(LARGEUR_PLATEAU * 0.03));
			case_surbrillance_rouge = RoundedRectangle_Class.Texture_Rounded_Rectangle_Saut_Cercle (_screen.ScreenManager.GraphicsDevice, Largueur_Case, Largueur_Case, color_red, color_case_vide, (int)(LARGEUR_PLATEAU * 0.005), (int)(LARGEUR_PLATEAU * 0.03));

			position_plateau = new Vector2 ((float)(_screen.ScreenManager.GraphicsDevice.Viewport.Width * 0.05), _height_debut);

			if (!_multi) {
				_plateau_debut = Return_Type_Plateau ();
			}

			// SET COLOR JOUEUR
			if (_couleur_joueur == PlayClass.Couleurs.Green) {
				_case_joueur = CaseClass.Type_Case.Green;
			} else {
				_case_joueur = CaseClass.Type_Case.Red;
			}

			_type_plateau = new char[NOMBRE_PLATEAU_LARGEUR * NOMBRE_PLATEAU_HAUTEUR];
			for (int i = 0; i < array_case_class.Length; i++) {
				_type_plateau [i] = _plateau_debut [i];
				CaseClass.Type_Case type_case = CaseClass.Type_Case.Vide;

				switch (_type_plateau [i]) {
				case '0':
					type_case = CaseClass.Type_Case.Vide;
					break;
				case '1':
					type_case = CaseClass.Type_Case.Obstacle;
					break;
				case '3':
					type_case = CaseClass.Type_Case.Red;
					break;
				case '4':
					type_case = CaseClass.Type_Case.Green;
					break;
				default : 
					type_case = CaseClass.Type_Case.Vide;
					break;
				}
				array_case_class [i] = new CaseClass (_screen, type_case, Position_Par_Index (i, NOMBRE_PLATEAU_LARGEUR));
				if (array_case_class [i]._type_case == CaseClass.Type_Case.Green || array_case_class [i]._type_case == CaseClass.Type_Case.Red) {
					array_case_class [i]._scale_annimation = 1;
				}
			}
		}

		public void Reset_Plateau()
		{
			for (int i = 0; i < _plateau_debut.Length; i++) {
				CaseClass.Type_Case type_case = CaseClass.Type_Case.Vide;

				switch (_plateau_debut [i]) {
				case '0':
					type_case = CaseClass.Type_Case.Vide;
					break;
				case '1':
					type_case = CaseClass.Type_Case.Obstacle;
					break;
				case '3':
					type_case = CaseClass.Type_Case.Red;
					break;
				case '4':
					type_case = CaseClass.Type_Case.Green;
					break;
				default : 
					type_case = CaseClass.Type_Case.Vide;
					break;
				}
				array_case_class [i] = new CaseClass (_screen, type_case, Position_Par_Index (i, NOMBRE_PLATEAU_LARGEUR));
				if (array_case_class [i]._type_case == CaseClass.Type_Case.Green || array_case_class [i]._type_case == CaseClass.Type_Case.Red) {
					array_case_class [i]._scale_annimation = 1;
				}
			}
		}

		public void Reset_Plateau(string plateau)
		{
			for (int i = 0; i < plateau.Length; i++) {
				CaseClass.Type_Case type_case = CaseClass.Type_Case.Vide;

				switch (plateau [i]) {
				case '0':
					type_case = CaseClass.Type_Case.Vide;
					break;
				case '1':
					type_case = CaseClass.Type_Case.Obstacle;
					break;
				case '3':
					type_case = CaseClass.Type_Case.Red;
					break;
				case '4':
					type_case = CaseClass.Type_Case.Green;
					break;
				default : 
					type_case = CaseClass.Type_Case.Vide;
					break;
				}
				array_case_class [i] = new CaseClass (_screen, type_case, Position_Par_Index (i, NOMBRE_PLATEAU_LARGEUR));
				if (array_case_class [i]._type_case == CaseClass.Type_Case.Green || array_case_class [i]._type_case == CaseClass.Type_Case.Red) {
					array_case_class [i]._scale_annimation = 1;
				}
			}
		}

		public string Return_Type_Plateau()
		{
			return "300000000000000000000000000000000000000000000004";
		}

		private Vector2 Position_Par_Index(int index, int largeur)
		{
			int hauteur_index = (int)(index / largeur);
			int largeur_index = (int)(index - (hauteur_index * largeur));
			float position_largeur = (float)(position_plateau.X + (_taille_bordure *1.5) + ((case_vide.Width + _taille_bordure) * largeur_index));
			float position_hauteur = (float)(position_plateau.Y + (_taille_bordure*1.5) + ((case_vide.Height + _taille_bordure) * hauteur_index));
			return new Vector2 (position_largeur, position_hauteur);
		}

		private float Taille_Bordure()
		{
			return (float)(LARGEUR_PLATEAU / 9 / 10);
		}

		private int Taille_Case()
		{
			return (int)((LARGEUR_PLATEAU - (_taille_bordure * 10))/8);
		}

		public bool HandleInput_Attente (InputState input,PlayClass.Statut_Partie statut)
		{
			if (statut == PlayClass.Statut_Partie.En_Attente) {
				for (int i = 0; i < array_case_class.Length; i++) {
					if (array_case_class [i].HandledInput (input, case_vide) && array_case_class [i]._type_case == _case_joueur) {
						//RENVOIE SI UNE CASE EST POSSIBLE AU MINIMUM
						if (Changement_Statut_Case_To_Selected (i)) {
							return true;
						}
					}
				}
			}
			return false;
		}

		public bool HandleInput_Selection (InputState input,PlayClass.Statut_Partie statut)
		{
			if (statut == PlayClass.Statut_Partie.Selection) {
				for (int i = 0; i < list_index_selected.Count; i++) {
					if (array_case_class [list_index_selected [i]].HandledInput (input, case_vide)) {
						if (Changement_Statut_Selected_To_Validation (list_index_selected [i], _index_selected, _case_joueur)) {
							_coup_jouer_annimation = true;
							return true;
						}
					}
				}

			}
			return false;
		}

		public bool Changement_Statut_Case_To_Selected( int index_selected)
		{
			int i = 0;
			List<int> bon_a_garder = new List<int> ();
			list_index_selected.Add (index_selected - 1);
			list_index_selected.Add (index_selected - 2);
			list_index_selected.Add (index_selected + 1);
			list_index_selected.Add (index_selected + 2);
			list_index_selected.Add (index_selected - NOMBRE_PLATEAU_LARGEUR);
			list_index_selected.Add (index_selected - NOMBRE_PLATEAU_LARGEUR - 1);
			list_index_selected.Add (index_selected - NOMBRE_PLATEAU_LARGEUR + 1);
			list_index_selected.Add (index_selected - (NOMBRE_PLATEAU_LARGEUR * 2));
			list_index_selected.Add (index_selected + NOMBRE_PLATEAU_LARGEUR);
			list_index_selected.Add (index_selected + NOMBRE_PLATEAU_LARGEUR + 1);
			list_index_selected.Add (index_selected + NOMBRE_PLATEAU_LARGEUR - 1);
			list_index_selected.Add (index_selected + (NOMBRE_PLATEAU_LARGEUR * 2));

			foreach(int caca in list_index_selected)
			{
				if (Verification_Case_Vide_Et_Possible (list_index_selected [i], index_selected)) {
					bon_a_garder.Add (caca);
					if (list_index_selected [i] == index_selected - 2 ||
						list_index_selected [i] == index_selected + 2 ||
						list_index_selected [i] == index_selected + (NOMBRE_PLATEAU_LARGEUR * 2) ||
						list_index_selected [i] == index_selected - (NOMBRE_PLATEAU_LARGEUR * 2)) {
						array_case_class [list_index_selected [i]]._type_surbrillance = CaseClass.Surbrillance_Type.Saut;
					} else {
						array_case_class [list_index_selected [i]]._type_surbrillance = CaseClass.Surbrillance_Type.Evolution;
					}
				}
				i++;
			}
			list_index_selected = new List<int> ();
			list_index_selected.AddRange (bon_a_garder);

			if (list_index_selected.Count > 0) {
				_index_selected = index_selected;
				return true;
			} else {
				list_index_selected = new List<int> ();
				_index_selected = -1;
			}
			return false;
		}


		public bool Verification_possibilite_jouer(CaseClass.Type_Case couleur_verification)
		{
			List<int> caca = new List<int> ();
			for (int k = 0; k < array_case_class.Length; k++) {
				if (array_case_class [k]._type_case == couleur_verification) {

					caca.Add (k - 1);
					caca.Add (k - 2);
					caca.Add (k + 1);
					caca.Add (k + 2);
					caca.Add (k - NOMBRE_PLATEAU_LARGEUR);
					caca.Add (k - NOMBRE_PLATEAU_LARGEUR - 1);
					caca.Add (k - NOMBRE_PLATEAU_LARGEUR + 1);
					caca.Add (k - (NOMBRE_PLATEAU_LARGEUR * 2));
					caca.Add (k + NOMBRE_PLATEAU_LARGEUR);
					caca.Add (k + NOMBRE_PLATEAU_LARGEUR + 1);
					caca.Add (k + NOMBRE_PLATEAU_LARGEUR - 1);
					caca.Add (k + (NOMBRE_PLATEAU_LARGEUR * 2));

					for (int i = 0; i < caca.Count; i++) {
						if (Verification_Case_Vide_Et_Possible (caca [i], k)) {
							return true;
						}
					}
					caca = new List<int> ();
				}
			}
			return false;
		}



		private bool Verification_Case_Vide_Et_Possible(int index, int index_origin)
		{
			if (index >= 0 && index < (NOMBRE_PLATEAU_LARGEUR * NOMBRE_PLATEAU_LARGEUR)) {
				if (array_case_class [index]._type_case == CaseClass.Type_Case.Vide) {

					if (index_origin % NOMBRE_PLATEAU_LARGEUR == 0 && (index == index_origin + NOMBRE_PLATEAU_LARGEUR - 1 || index == index_origin - NOMBRE_PLATEAU_LARGEUR - 1)) {
						return false;
					}

					int modificateur_vertical = 0;
					if (index == index_origin - NOMBRE_PLATEAU_LARGEUR - 1 || index == index_origin - NOMBRE_PLATEAU_LARGEUR + 1 || index == index_origin - NOMBRE_PLATEAU_LARGEUR) {
						modificateur_vertical = 1;
					} else if (index == index_origin + NOMBRE_PLATEAU_LARGEUR - 1 || index == index_origin + NOMBRE_PLATEAU_LARGEUR + 1 || index == index_origin + NOMBRE_PLATEAU_LARGEUR) {
						modificateur_vertical = -1;
					} else if (index == index_origin - (NOMBRE_PLATEAU_LARGEUR * 2)) {
						modificateur_vertical = 2;
					} else if (index == index_origin + (NOMBRE_PLATEAU_LARGEUR * 2)) {
						modificateur_vertical = -2;
					}
					int index_base_horizontal = index + (NOMBRE_PLATEAU_LARGEUR * modificateur_vertical);
					int base_horizontal = index_base_horizontal / NOMBRE_PLATEAU_LARGEUR;
					int index_horizontal = index_origin / NOMBRE_PLATEAU_LARGEUR;
					if (base_horizontal == index_horizontal) {
						return true;
					}
				}
			}
			return false;
		}

		public void Changement_Statut_Selected_To_Nothing()
		{
			Reset_Surbrillance ();
			_index_selected = -1;
		}

		public bool Changement_Statut_Selected_To_Validation(int index_selected, int index_origin, CaseClass.Type_Case type_case_joueur)
		{
			if (array_case_class [index_selected]._type_surbrillance == CaseClass.Surbrillance_Type.Evolution) {
				_coup_jouer_index = index_selected;
				_type_coup_jouer = CaseClass.Surbrillance_Type.Evolution;
				Reset_Surbrillance ();
				_index_selected = -1;
				Gestion_Coup_Jouer (_couleur_joueur);
				return true;
			} else if (array_case_class [index_selected]._type_surbrillance == CaseClass.Surbrillance_Type.Saut) {

				_coup_jouer_index = index_selected;
				_type_coup_jouer = CaseClass.Surbrillance_Type.Saut;
				_origin_saut_coup_jouer = index_origin;
				Reset_Surbrillance ();
				_index_selected = -1;
				Gestion_Coup_Jouer (_couleur_joueur);
				return true;
			}
			return false;
		}

		private void Reset_Surbrillance()
		{
			for (int i = 0; i < array_case_class.Length; i++) {
				array_case_class [i]._type_surbrillance = CaseClass.Surbrillance_Type.Rien;
			}
			list_index_selected = new List<int> ();
		}

		private void Contamination(int index_jouer)
		{
			CaseClass.Type_Case type_case_index_jouer = array_case_class [index_jouer]._type_case;
			CaseClass.Type_Case type_case_autre;
			if (type_case_index_jouer == CaseClass.Type_Case.Green) {
				type_case_autre = CaseClass.Type_Case.Red;
			} else {
				type_case_autre = CaseClass.Type_Case.Green;
			}
			List<int> caca = new List<int> ();
			caca.Add (index_jouer - 1);
			caca.Add (index_jouer + 1);
			caca.Add (index_jouer - NOMBRE_PLATEAU_LARGEUR);
			caca.Add (index_jouer - NOMBRE_PLATEAU_LARGEUR - 1);
			caca.Add (index_jouer - NOMBRE_PLATEAU_LARGEUR + 1);
			caca.Add (index_jouer + NOMBRE_PLATEAU_LARGEUR);
			caca.Add (index_jouer + NOMBRE_PLATEAU_LARGEUR + 1);
			caca.Add (index_jouer + NOMBRE_PLATEAU_LARGEUR - 1);

			for (int i = 0; i < caca.Count; i++) {
				if (Verification_Case_Possible (caca [i], index_jouer) && array_case_class [caca [i]]._type_case == type_case_autre) {
					array_case_class [caca [i]]._type_case = type_case_index_jouer;
					array_case_class [caca [i]]._type_annim = CaseClass.Type_Annimation.Both;
				}
			}
		}

		private bool Verification_Case_Possible(int index, int index_origin)
		{
			if (index >= 0 && index < (NOMBRE_PLATEAU_LARGEUR * NOMBRE_PLATEAU_LARGEUR)) {

				if (index_origin % NOMBRE_PLATEAU_LARGEUR == 0 && (index == index_origin + NOMBRE_PLATEAU_LARGEUR - 1 || index == index_origin - NOMBRE_PLATEAU_LARGEUR - 1)) {
					return false;
				}

				int modificateur_vertical = 0;
				if (index == index_origin - NOMBRE_PLATEAU_LARGEUR - 1 || index == index_origin - NOMBRE_PLATEAU_LARGEUR + 1 || index == index_origin - NOMBRE_PLATEAU_LARGEUR) {
					modificateur_vertical = 1;
				} else if (index == index_origin + NOMBRE_PLATEAU_LARGEUR - 1 || index == index_origin + NOMBRE_PLATEAU_LARGEUR + 1 || index == index_origin + NOMBRE_PLATEAU_LARGEUR) {
					modificateur_vertical = -1;
				} else if (index == index_origin - (NOMBRE_PLATEAU_LARGEUR * 2)) {
					modificateur_vertical = 2;
				} else if (index == index_origin + (NOMBRE_PLATEAU_LARGEUR * 2)) {
					modificateur_vertical = -2;
				}
				int index_base_horizontal = index + (NOMBRE_PLATEAU_LARGEUR * modificateur_vertical);
				int base_horizontal = index_base_horizontal / NOMBRE_PLATEAU_LARGEUR;
				int index_horizontal = index_origin / NOMBRE_PLATEAU_LARGEUR;
				if (base_horizontal == index_horizontal) {
					return true;
				}
			}
			return false;
		}

		private void Gestion_Coup_Jouer(PlayClass.Couleurs _tour_de_jouer)
		{
			_coup_jouer = true;

			CaseClass.Type_Case caca;
			if (_tour_de_jouer == PlayClass.Couleurs.Green) {
				caca = CaseClass.Type_Case.Green;
			} else {
				caca = CaseClass.Type_Case.Red;
			}

			if (_type_coup_jouer == CaseClass.Surbrillance_Type.Evolution) {
				array_case_class [_coup_jouer_index]._type_case = caca;
				array_case_class [_coup_jouer_index]._type_annim = CaseClass.Type_Annimation.On;
			} else if (_type_coup_jouer == CaseClass.Surbrillance_Type.Saut) {
				array_case_class [_origin_saut_coup_jouer]._type_annim = CaseClass.Type_Annimation.Off;
				array_case_class [_coup_jouer_index]._type_case = caca;
				array_case_class [_coup_jouer_index]._type_annim = CaseClass.Type_Annimation.On;
			}

			_test_contamination_needed = true;
			_coup_jouer_annimation = true;
		}

		public void Update_Annimation (float timer, PlayClass.Couleurs _tour_de_jouer)
		{
			if (_coup_jouer_annimation) {
				foreach (CaseClass caca in array_case_class) {
					if (caca._type_annim == CaseClass.Type_Annimation.On) {
						caca._scale_annimation = caca._timer_annimation._timer / caca._timer_annimation._timer_max;
						if (caca._timer_annimation.IncreaseTimer (timer)) {
							caca._scale_annimation = 1;
							caca._type_annim = CaseClass.Type_Annimation.None;
						}
					} else if (caca._type_annim == CaseClass.Type_Annimation.Off) {
						caca._scale_annimation = 1 - caca._timer_annimation._timer / caca._timer_annimation._timer_max;
						if (caca._timer_annimation.IncreaseTimer (timer)) {
							caca._scale_annimation = 0;
							caca._type_annim = CaseClass.Type_Annimation.None;
							caca._type_case = CaseClass.Type_Case.Vide;
						}
					} else if (caca._type_annim == CaseClass.Type_Annimation.Both) {
						float half = caca._timer_annimation._timer_max / 2;

						if (caca._timer_annimation._timer < half) {
							caca._scale_annimation = 1 - caca._timer_annimation._timer / half;
						} else {
							caca._half_done_annimation_both = true;
							caca._scale_annimation = (caca._timer_annimation._timer - half) / half;
						}

						if (caca._timer_annimation.IncreaseTimer (timer)) {
							caca._scale_annimation = 1;
							caca._half_done_annimation_both = false;
							caca._type_annim = CaseClass.Type_Annimation.None;
						}
					}
				}
			}


			if (_coup_jouer && Fin_Tour_Annimation (array_case_class)) {
				if (_test_contamination_needed) {
					_coup_jouer_annimation = true;
					Contamination (_coup_jouer_index);
					_test_contamination_needed = false;
				}else {
					_coup_jouer = false;
					_coup_jouer_annimation = false;
					_test_contamination_needed = false;
					_coup_jouer_index = -1;
					_origin_saut_coup_jouer = -1;
					_type_coup_jouer = CaseClass.Surbrillance_Type.Rien;
					_tour_changement = true;

				}
			}
		}

		private bool Fin_Tour_Annimation(CaseClass[] plateau)
		{
			bool blbl = true;
			if (_coup_jouer_annimation) {
				foreach (CaseClass caca in plateau) {
					if (caca._type_annim != CaseClass.Type_Annimation.None) {
						blbl = false;
						break;
					}
				}
				if (blbl) {
					_coup_jouer_annimation = false;
				}
				return blbl;
			}
			return false;
		}

		public void Draw (TransitionClass transition)
		{
			_screen.ScreenManager.SpriteBatch.Draw (plateau_fond, new Vector2 (position_plateau.X, position_plateau.Y), Color.White * transition._transition_alpha);
			Draw_Case_Plateau (transition);
		}

		private void Draw_Case_Plateau (TransitionClass transition)
		{
			for (int i = 0; i < array_case_class.Length; i++) {
				array_case_class [i].Draw (transition,"1",new List<Texture2D> { case_vide, case_rouge, case_verte, case_surbrillance_rouge, case_surbrillance_vert });
			}
		}
	}
}

