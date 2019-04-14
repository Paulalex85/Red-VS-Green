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
	public class PlateauClass
	{
		//GENERAL ET CONSTANTES
		GameScreen _screen;
		const int NOMBRE_PLATEAU_LARGEUR = 8;
		int LARGEUR_PLATEAU;
		int _height_debut;
		Vector2 position_plateau;
		Random rand = new Random();
		char[] _type_plateau;
		int _index_selected = -1;
		bool _multi = false, _is_joueur_1 = false;
		string _side;
		string _plateau_debut;

		public bool _coup_jouer_annimation = false;
		public bool _test_contamination_needed = false;
		public bool _coup_jouer = false; 
		public int _coup_jouer_index = -1;
		public CaseClass.Surbrillance_Type _type_coup_jouer = CaseClass.Surbrillance_Type.Rien;
		public int _origin_saut_coup_jouer = -1;
		public bool _tour_changement = false;

		PlayClass.Couleurs _couleur_joueur;
		PlayClass.Couleurs _couleur_adversaire;

		//GENERAL MULTI
		string[] index_conta;
		string _code_plateau_coup_jouer = "";

		//ARRAY
		public CaseClass[] array_case_class;
		List<int> list_index_selected = new List<int>();
		CaseClass.Type_Case _case_joueur;
		CaseClass.Type_Case _case_adversaire;

		//COULEURS
		Color color_fond = new Color(187,173,160), 
		color_case_vide = new Color(205,192,180),
		color_green = new Color(71,164,71),
		color_red = new Color(246,94,59);

		//TEXTURE
		Texture2D plateau_fond, case_vide, case_verte, case_rouge, case_surbrillance_rouge, case_surbrillance_vert;
		RoundedRectangle RoundedRectangle_Class = new RoundedRectangle();
		float _taille_bordure;

		//IA
		private Thread ia_thread;
		IAClass _ia = new IAClass();

		public PlateauClass (GameScreen screen, int height_debut,PlayClass.Couleurs couleur_joueur, string side)
		{
			_height_debut = height_debut;
			_screen = screen;
			_couleur_joueur = couleur_joueur;
			_multi = false;
			_side = side;
			LoadContent ();
		}

		public PlateauClass (GameScreen screen, int height_debut,PlayClass.Couleurs couleur_joueur, bool is_joueur_1, string side, string plateau)
		{
			_height_debut = height_debut;
			_screen = screen;
			_couleur_joueur = couleur_joueur;
			_multi = true;
			_side = side;
			_plateau_debut = plateau;
			_is_joueur_1 = is_joueur_1;
			LoadContent ();
		}

		private void LoadContent ()
		{
			//PLATEAU DIMENSION
			array_case_class = new CaseClass[NOMBRE_PLATEAU_LARGEUR * NOMBRE_PLATEAU_LARGEUR];
			LARGEUR_PLATEAU = (int)(_screen.ScreenManager.GraphicsDevice.Viewport.Width * 0.9);

			_taille_bordure = Taille_Bordure ();
			int Largueur_Case = Taille_Case ();
			

			//INITIALISATION TEXTURE
			plateau_fond = RoundedRectangle_Class.Texture_Rounded_Rectangle (_screen.ScreenManager.GraphicsDevice, LARGEUR_PLATEAU, LARGEUR_PLATEAU, color_fond, (int)(LARGEUR_PLATEAU * 0.005), (int)(LARGEUR_PLATEAU * 0.03));
			case_vide = RoundedRectangle_Class.Texture_Rounded_Rectangle (_screen.ScreenManager.GraphicsDevice, Largueur_Case, Largueur_Case, color_case_vide, (int)(LARGEUR_PLATEAU * 0.005), (int)(LARGEUR_PLATEAU * 0.03));
			case_verte = RoundedRectangle_Class.Texture_Rounded_Rectangle (_screen.ScreenManager.GraphicsDevice, Largueur_Case, Largueur_Case, color_green, (int)(LARGEUR_PLATEAU * 0.005), (int)(LARGEUR_PLATEAU * 0.03));
			case_rouge = RoundedRectangle_Class.Texture_Rounded_Rectangle (_screen.ScreenManager.GraphicsDevice, Largueur_Case, Largueur_Case, color_red, (int)(LARGEUR_PLATEAU * 0.005), (int)(LARGEUR_PLATEAU * 0.03));

			//INITIALISATION TEXTURE SURBRILLANCE SAUT

			case_surbrillance_vert = RoundedRectangle_Class.Texture_Rounded_Rectangle_Saut_Cercle (_screen.ScreenManager.GraphicsDevice,Largueur_Case, Largueur_Case, color_green, color_case_vide, (int)(LARGEUR_PLATEAU * 0.005), (int)(LARGEUR_PLATEAU * 0.03));
			case_surbrillance_rouge = RoundedRectangle_Class.Texture_Rounded_Rectangle_Saut_Cercle (_screen.ScreenManager.GraphicsDevice, Largueur_Case, Largueur_Case, color_red, color_case_vide, (int)(LARGEUR_PLATEAU * 0.005), (int)(LARGEUR_PLATEAU * 0.03));

			position_plateau = new Vector2 ((float)(_screen.ScreenManager.GraphicsDevice.Viewport.Width * 0.05), _height_debut);

			if(!_multi)
			{
				_plateau_debut = Return_Type_Plateau ();
			}

			// SET COLOR JOUEUR
			if (_couleur_joueur == PlayClass.Couleurs.Green) {
				_case_joueur = CaseClass.Type_Case.Green;
				_case_adversaire = CaseClass.Type_Case.Red;
				_couleur_adversaire = PlayClass.Couleurs.Red;
			} else {
				_case_joueur = CaseClass.Type_Case.Red;
				_case_adversaire = CaseClass.Type_Case.Green;
				_couleur_adversaire = PlayClass.Couleurs.Green;
			}

			//INITIALISATION ARRAY
			if (!_multi) {
				_type_plateau = new char[NOMBRE_PLATEAU_LARGEUR * NOMBRE_PLATEAU_LARGEUR];
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
			} else {
				_type_plateau = new char[NOMBRE_PLATEAU_LARGEUR * NOMBRE_PLATEAU_LARGEUR];
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
						if (_is_joueur_1) {
							type_case = _case_joueur;
						} else {
							type_case = _case_adversaire;
						}
						break;
					case '4':
						if (_is_joueur_1) {
							type_case = _case_adversaire;
						} else {
							type_case = _case_joueur;
						}
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
		}

		public void Reset_Plateau(string plateau)
		{
			if (!_multi) {
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
			} else {
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
						if (_is_joueur_1) {
							type_case = _case_joueur;
						} else {
							type_case = _case_adversaire;
						}
						break;
					case '4':
						if (_is_joueur_1) {
							type_case = _case_adversaire;
						} else {
							type_case = _case_joueur;
						}
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
		}

		public string Return_Type_Plateau()
		{
			//if (!_multi) {
			int nombre_de_plateau_possible = 9;
			int k = rand.Next (0, nombre_de_plateau_possible);
			switch (k) {
			case 0:
				return "3000000000000000000000000000000000000000000000000000000000000004";
			case 1: 
				return "3000000000000000000000000001100000011000000000000000000000000004";
			case 2: 
				return "3300000031111100010000100101001001001010010000100011111400000044";
			case 3: 
				return "3101100001100100010000100100001001000010010000100010011000011014";
			case 4: 
				return "0000000110000010010101000013100000141000010101001000001000000001";
			case 5: 
				return "1110011111000011100000013000000430000004100000011100001111100111";
			case 6: 
				return "3100100010100100010100100010100110010100010010100010010100010014";
			case 7: 
				return "3001400000010000000100030041111111111400300010000000100000041003";
			case 8: 
				return "1400000141000010001001000001100000011000001001000100001310000031";
			default :
				return "3000000000000000000000000000000000000000000000000000000000000004";
			}
		}

		private Vector2 Position_Par_Index(int index, int largeur)
		{
			int hauteur_index = (int)(index / largeur);
			int largeur_index = (int)(index - (hauteur_index * largeur));
			float position_largeur = (float)(position_plateau.X + (_taille_bordure*1.5) + ((case_vide.Width + _taille_bordure) * largeur_index));
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

		public string HandleInput_Selection_Multi(InputState input,PlayClass.Statut_Partie statut)
		{
			if (statut == PlayClass.Statut_Partie.Selection) {
				for (int i = 0; i < list_index_selected.Count; i++) {
					if (array_case_class [list_index_selected [i]].HandledInput (input, case_vide)) {

						int index_destination = list_index_selected [i];

						if (array_case_class [index_destination]._type_surbrillance == CaseClass.Surbrillance_Type.Evolution) {
							Reset_Surbrillance ();
							string data_return = "0_" + _index_selected + "_" + index_destination;
							_index_selected = -1;
							return data_return;
						} else if (array_case_class [index_destination]._type_surbrillance == CaseClass.Surbrillance_Type.Saut) {
							Reset_Surbrillance ();
							string data_return = "1_" + _index_selected + "_" + index_destination;
							_index_selected = -1;
							return data_return;
						} else {
							return "";
						}
					}
				}

			}
			return "";
		}

		public void Reponse_server_multi_coup_jouer(string code_plateau_joueur_coup,string new_plateau,string detail_coup )
		{
			string[] data_coup = detail_coup.Split ('_');
			//COUP
			string type_coup = data_coup [1];
			string index_origin = data_coup [2];
			string index_destination = data_coup [3];
			//CONTAMINATION
			string contamination = data_coup [5];
			index_conta = contamination.Split ('?');
			PlayClass.Couleurs _couleur_coup_jouer;
			if (code_plateau_joueur_coup == "3") {
				if (_is_joueur_1) {
					_couleur_coup_jouer = _couleur_joueur;
				} else {
					_couleur_coup_jouer = _couleur_adversaire;
				}
			} else {
				if (_is_joueur_1) {
					_couleur_coup_jouer = _couleur_adversaire;
				} else {
					_couleur_coup_jouer = _couleur_joueur;
				}
			}
			_code_plateau_coup_jouer = code_plateau_joueur_coup;
			_coup_jouer_index = int.Parse( index_destination);
			if (type_coup == "0") {
				_type_coup_jouer = CaseClass.Surbrillance_Type.Evolution;
			} else if (type_coup == "1") {
				_type_coup_jouer = CaseClass.Surbrillance_Type.Saut;
				_origin_saut_coup_jouer = int.Parse( index_origin);
			}
			Gestion_Coup_Jouer (_couleur_coup_jouer);
		}

		public string Plateau_To_String()
		{
			string plateau = "";
			for (int i = 0; i < 64; i++) {
				string blbl = "";
				switch (array_case_class [i]._type_case) {
				case CaseClass.Type_Case.Vide:

					blbl = "0";
					break;
				case CaseClass.Type_Case.Obstacle:
					blbl = "1";
					break;
				case CaseClass.Type_Case.Red:
					if (_couleur_joueur == PlayClass.Couleurs.Red) {
						if (_is_joueur_1) {
							blbl = "3";
						} else {
							blbl = "4";
						}
					} else {
						if (_is_joueur_1) {
							blbl = "4";
						} else {
							blbl = "3";
						}
					}
					break;
				case CaseClass.Type_Case.Green:
					if (_couleur_joueur == PlayClass.Couleurs.Red) {
						if (_is_joueur_1) {
							blbl = "4";
						} else {
							blbl = "3";
						}
					} else {
						if (_is_joueur_1) {
							blbl = "3";
						} else {
							blbl = "4";
						}
					}
					break;
				default : 
					blbl = "0";
					break;
				}

				plateau = plateau + blbl;
			}
			return plateau;
		}

		private bool Changement_Statut_Case_To_Selected( int index_selected)
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

		private bool Changement_Statut_Selected_To_Validation(int index_selected, int index_origin, CaseClass.Type_Case type_case_joueur)
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

		private void Contamination_multi(string[] index_contamination, string type_case_index_jouer)
		{
			for (int i = 0; i < index_contamination.Length; i++) {
				if (index_contamination [i] != "") {
					int index = int.Parse (index_contamination [i]);
					if (int.Parse (index_contamination [i]) >= 0) {

						switch (type_case_index_jouer) {
						case "0":
							array_case_class [index]._type_case = CaseClass.Type_Case.Vide;
							break;
						case "1":
							array_case_class [index]._type_case = CaseClass.Type_Case.Obstacle;
							break;
						case "3":
							if (_is_joueur_1) {
								array_case_class [index]._type_case = _case_joueur;
							} else {
								array_case_class [index]._type_case = _case_adversaire;
							}
							break;
						case "4":
							if (_is_joueur_1) {
								array_case_class [index]._type_case = _case_adversaire;
							} else {
								array_case_class [index]._type_case = _case_joueur;
							}
							break;
						default : 
							array_case_class [index]._type_case = CaseClass.Type_Case.Vide;
							break;
						}
						array_case_class [index]._type_annim = CaseClass.Type_Annimation.Both;
					}
				}
			}
			index_conta = null;
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


			if (!_multi) {
				_test_contamination_needed = true;
				_coup_jouer_annimation = true;
			} else {
				_coup_jouer_annimation = true;
				if (index_conta != null || index_conta[0] != "-1" || index_conta[0] != "") {
					_test_contamination_needed = true;
				} else {
					_test_contamination_needed = false;
				}
			}

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
				if (_test_contamination_needed && !_multi) {
					_coup_jouer_annimation = true;
					Contamination (_coup_jouer_index);
					_test_contamination_needed = false;
				} else if (_test_contamination_needed && _multi) {
					_coup_jouer_annimation = true;
					Contamination_multi (index_conta, _code_plateau_coup_jouer);
					_test_contamination_needed = false;
				} else {
					_coup_jouer = false;
					_coup_jouer_annimation = false;
					_test_contamination_needed = false;
					_coup_jouer_index = -1;
					_origin_saut_coup_jouer = -1;
					_type_coup_jouer = CaseClass.Surbrillance_Type.Rien;
					_tour_changement = true;
					if (_multi) {
						_code_plateau_coup_jouer = "";
					}
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

		public void Tour_Adversaire_IA()
		{
			if (ia_thread == null) {
				ia_thread = new Thread (Gestion_Tour_Adversaire);
				ia_thread.Start ();
			}
		}


		public void Remplissage_plateau_fin_partie( CaseClass.Type_Case type_case_a_remplir)
		{
			for (int i = 0; i < array_case_class.Length; i++) {
				if (array_case_class [i]._type_case == CaseClass.Type_Case.Vide) {
					array_case_class [i]._type_case = type_case_a_remplir;
					array_case_class [i]._scale_annimation = 1f;
				}
			}
		}

		private void Gestion_Tour_Adversaire()
		{
			List<int> caca = new List<int> ();
			caca = _ia.index_coup_jouer (array_case_class, NOMBRE_PLATEAU_LARGEUR, _case_joueur);

			if (caca [0] == 1) {
				_type_coup_jouer = CaseClass.Surbrillance_Type.Evolution;
			} else {
				_type_coup_jouer = CaseClass.Surbrillance_Type.Saut;
			}
			_coup_jouer_index = caca [1];
			_origin_saut_coup_jouer = caca [2];
			Gestion_Coup_Jouer (_couleur_adversaire);
			ia_thread = null;
		}

		public int Nombre_cases_Red()
		{
			int caca = 0;
			for (int i = 0; i < array_case_class.Length; i++) {
				if (array_case_class [i]._type_case == CaseClass.Type_Case.Red) {
					caca++;
				}
			}
			return caca;
		}

		public int Nombre_cases_Green()
		{
			int caca = 0;
			for (int i = 0; i < array_case_class.Length; i++) {
				if (array_case_class [i]._type_case == CaseClass.Type_Case.Green) {
					caca++;
				}
			}
			return caca;
		}

		public void Draw (TransitionClass transition)
		{
			_screen.ScreenManager.SpriteBatch.Draw (plateau_fond, new Vector2 (position_plateau.X, position_plateau.Y), Color.White * transition._transition_alpha);
			Draw_Case_Plateau (transition);
		}

		private void Draw_Case_Plateau (TransitionClass transition)
		{
			for (int i = 0; i < array_case_class.Length; i++) {
				array_case_class [i].Draw (transition,_side,new List<Texture2D> { case_vide, case_rouge, case_verte, case_surbrillance_rouge, case_surbrillance_vert });
			}
		}
	}
}

