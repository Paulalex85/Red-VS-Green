using System;
using System.Collections.Generic;

namespace RedVsGreen
{
	public class IAClass
	{
		Random rand = new Random();
		int difficulté = 2; // 1 easy 2 medium 3 hard

		public IAClass ()
		{
		}

		public List<int> index_coup_jouer(CaseClass[] plateau, int NOMBRE_PLATEAU_LARGEUR, CaseClass.Type_Case couleur_joueur)
		{
			int index_coup_jouer = -1;
			int _type_coup_jouer = -1;
			int _origin_coup_jouer_saut = -1;

			CaseClass.Type_Case type_case_adversaire;
			List<int> list_index_possible_en_evolution = new List<int> ();
			List<int> list_index_possible_en_saut = new List<int> ();
			List<int> list_index_possible_en_saut_origin = new List<int> ();
			//set case adversaire
			if (couleur_joueur == CaseClass.Type_Case.Green) {
				type_case_adversaire = CaseClass.Type_Case.Red;
			} else {
				type_case_adversaire = CaseClass.Type_Case.Green;
			}

			//fais la liste des cases possibles
			for (int k = 0; k < plateau.Length; k++) {
				if (plateau [k]._type_case == type_case_adversaire) {

					int i = 0;
					List<int> list_index_selected = new List<int> ();
					list_index_selected.Add (k - 1);
					list_index_selected.Add (k - 2);
					list_index_selected.Add (k + 1);
					list_index_selected.Add (k + 2);
					list_index_selected.Add (k - NOMBRE_PLATEAU_LARGEUR);
					list_index_selected.Add (k - NOMBRE_PLATEAU_LARGEUR - 1);
					list_index_selected.Add (k - NOMBRE_PLATEAU_LARGEUR + 1);
					list_index_selected.Add (k - (NOMBRE_PLATEAU_LARGEUR * 2));
					list_index_selected.Add (k + NOMBRE_PLATEAU_LARGEUR);
					list_index_selected.Add (k + NOMBRE_PLATEAU_LARGEUR + 1);
					list_index_selected.Add (k + NOMBRE_PLATEAU_LARGEUR - 1);
					list_index_selected.Add (k + (NOMBRE_PLATEAU_LARGEUR * 2));

					foreach(int caca in list_index_selected)
					{
						if (Verification_Case_Vide_Et_Possible (list_index_selected [i], k, NOMBRE_PLATEAU_LARGEUR, plateau)) {
							if (list_index_selected [i] == k - 2 ||
							    list_index_selected [i] == k + 2 ||
							    list_index_selected [i] == k + (NOMBRE_PLATEAU_LARGEUR * 2) ||
							    list_index_selected [i] == k - (NOMBRE_PLATEAU_LARGEUR * 2)) {
								if (!list_index_possible_en_saut.Contains (list_index_selected [i])) {
									list_index_possible_en_saut.Add (list_index_selected [i]);
									list_index_possible_en_saut_origin.Add (k);
								}
							} else {
								if (!list_index_possible_en_evolution.Contains (list_index_selected [i])) {
									list_index_possible_en_evolution.Add (list_index_selected [i]);
								}
							}
						}
						i++;
					}
				}
			}

			List<int> foutre = new List<int> ();
			//TODO : AMELIORER CETTE IA DEG
			//CALCUL DISTANCE POUR PLUS OPTIMISE ET NON RANDOM COMME UN PD 
			if (difficulté == 1) {
				if (list_index_possible_en_evolution.Count > 0) {
					int blbl = rand.Next (0, list_index_possible_en_evolution.Count);
					index_coup_jouer = list_index_possible_en_evolution [blbl];
					_type_coup_jouer = 1;
				} else {
					int blbl = rand.Next (0, list_index_possible_en_saut.Count);
					index_coup_jouer = list_index_possible_en_saut [blbl];
					_type_coup_jouer = 2;
					_origin_coup_jouer_saut = list_index_possible_en_saut_origin [blbl];
				}

				foutre.Add (_type_coup_jouer);
				foutre.Add (index_coup_jouer);
				foutre.Add (_origin_coup_jouer_saut);
			} else {

				int[,] list_de_possibilite = new int[list_index_possible_en_evolution.Count, 2];
				int[,] list_de_possibilite_saut = new int[list_index_possible_en_saut.Count, 2];
				for (int i = 0; i < list_index_possible_en_evolution.Count; i++) {
					int blbl = Contamination_index (list_index_possible_en_evolution [i], NOMBRE_PLATEAU_LARGEUR, plateau, couleur_joueur);
					list_de_possibilite [i,0] = list_index_possible_en_evolution [i];
					list_de_possibilite [i,1] = blbl;
				}
				for (int i = 0; i < list_index_possible_en_saut.Count; i++) {
					int blbl = Contamination_index (list_index_possible_en_saut [i], NOMBRE_PLATEAU_LARGEUR, plateau, couleur_joueur);
					list_de_possibilite_saut [i,0] = list_index_possible_en_saut [i];
					list_de_possibilite_saut [i,1] = blbl;
				}

				int max_conta_evol = 0, max_conta_saut = 0 , index_evol=0,index_saut=0, origin_index_saut=0;

				for (int i = 0; i < list_de_possibilite.Length / 2; i++) {
					if (list_de_possibilite [i,1] > max_conta_evol) {
						max_conta_evol = list_de_possibilite [i,1];
						index_evol = list_de_possibilite [i,0];
					}
				}

				for (int i = 0; i < list_de_possibilite_saut.Length / 2; i++) {
					if (list_de_possibilite_saut [i,1] > max_conta_saut) {
						max_conta_saut = list_de_possibilite_saut [i,1];
						index_saut = list_de_possibilite_saut [i,0];
						origin_index_saut = list_index_possible_en_saut_origin [i];
					}
				}

				if (max_conta_evol == 0 && max_conta_saut == 0) {
					if (list_index_possible_en_evolution.Count > 0) {
						int blbl = rand.Next (0, list_index_possible_en_evolution.Count);
						index_coup_jouer = list_index_possible_en_evolution [blbl];
						_type_coup_jouer = 1;
					} else {
						int blbl = rand.Next (0, list_index_possible_en_saut.Count);
						index_coup_jouer = list_index_possible_en_saut [blbl];
						_type_coup_jouer = 2;
						_origin_coup_jouer_saut = list_index_possible_en_saut_origin [blbl];
					}

					foutre.Add (_type_coup_jouer);
					foutre.Add (index_coup_jouer);
					foutre.Add (_origin_coup_jouer_saut);
				} else {
					if (max_conta_saut > max_conta_evol) {
						foutre.Add (2);
						foutre.Add (index_saut);
						foutre.Add (origin_index_saut);
					} else {
						foutre.Add (1);
						foutre.Add (index_evol);
						foutre.Add (0);
					}
				}

			}

			return foutre;
		}

		private int Contamination_index(int index,int NOMBRE_PLATEAU_LARGEUR,CaseClass[] plateau,CaseClass.Type_Case couleur_adversaire )
		{
			int jaimelabite = 0;

			List<int> caca = new List<int> ();
			caca.Add (index - 1);
			caca.Add (index + 1);
			caca.Add (index - NOMBRE_PLATEAU_LARGEUR);
			caca.Add (index - NOMBRE_PLATEAU_LARGEUR - 1);
			caca.Add (index - NOMBRE_PLATEAU_LARGEUR + 1);
			caca.Add (index + NOMBRE_PLATEAU_LARGEUR);
			caca.Add (index + NOMBRE_PLATEAU_LARGEUR + 1);
			caca.Add (index + NOMBRE_PLATEAU_LARGEUR - 1);

			for (int i = 0; i < caca.Count; i++) {
				if (Verification_Case_Possible(caca[i],index, NOMBRE_PLATEAU_LARGEUR) && plateau [caca [i]]._type_case == couleur_adversaire) {
					jaimelabite++;
				}
			}
			return jaimelabite;
		}

		private bool Verification_Case_Possible(int index, int index_origin, int NOMBRE_PLATEAU_LARGEUR)
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

		private static bool Verification_Case_Vide_Et_Possible(int index, int index_origin, int NOMBRE_PLATEAU_LARGEUR ,CaseClass[] plateau)
		{
			if (index >= 0 && index < (NOMBRE_PLATEAU_LARGEUR * NOMBRE_PLATEAU_LARGEUR)) {
				if (plateau [index]._type_case == CaseClass.Type_Case.Vide) {

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
	}
}

