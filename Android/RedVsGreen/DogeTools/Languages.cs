using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using GameStateManagement;
using Microsoft.Xna.Framework.Content;
using System.IO.IsolatedStorage;

namespace RedVsGreen
{
	public class Languages
    {
		public string lang;
        public Languages()
        {
			lang = (string)IsolatedStorageSettings.ApplicationSettings ["lang"];
        }

		public string getString(int id)
		{
			if (lang == "FR") {

				switch (id) {
				case 1:
					return "Victoire";
				case 2:
					return "Défaite";
				case 3:
					return "Match Nul";
				case 4:
					return "Nouveau Elo";
				case 5:
					return "Nouveau Rang";
				case 6:
					return "Recommencer";
				case 7:
					return "Menu";
				case 8:
					return "Ordinateur";
				case 9:
					return "Attente Match";
				case 10:
					return "Oui";
				case 11:
					return "Non";
				case 12:
					return "Quitter La Partie ?";
				case 13:
					return "Match Trouvé";
				case 14:
					return "Accepter";
				case 15:
					return "Refuser";
				case 16:
					return "Attente Adversaire";
				case 17:
					return "Temps écoulé";
				case 18:
					return "Adversaire Refuse";
				case 19:
					return "Retour";
				case 20:
					return "Jouer";
				case 21:
					return "Quitter";
				case 22:
					return "Rang";
				case 23:
					return "Quel est votre nom ?";
				case 24:
					return "Entre 4 Et ";
				case 25:
					return "Lettres";
				case 26:
					return "Mauvais nombre de lettres";
				case 27:
					return "Moins de ";
				case 28:
					return "Check";
				case 29:
					return "Quel est votre nom ?";
				case 30:
					return "Entre 4 Et ";
				case 31:
					return "Lettres";
				case 32:
					return "Mauvais nombre de lettres";
				case 33:
					return "Moins de ";
				case 34:
					return "Deja pris";
				case 35:
					return "Patientez";
				case 36:
					return "Tap Pour Confirmer";
				case 37:
					return "Creation En Cours";
				case 38:
					return "Pas De Connection";
				case 39:
					return "Changer Camp";
				case 40:
					return "Changer Nom";
				case 41:
					return "Aide";
				case 42:
					return "Options";
				case 43:
					return "Recherche Adversaire";
				case 44:
					return "Partie Vs IA en attendant";
				case 45:
					return "Quel est votre camp ?";
				case 46:
					return "Comment Jouer ?";
				case 47:
					return "Tap Pour Continuer";
				case 48:
					return "Selectionner un carré de votre couleur";
				case 49:
					return "Ensuite, choisissez où vous voulez le faire evoluer ou déplacer";
				case 50:
					return "Si votre coup est proche du camp ennemi vous les capturez";
				case 51:
					return "A la fin, le camp avec le plus grand nombre de points gagne. En Multi-Joueur, vous gagner ou perder des points elo qui vont déterminer votre rang.";
				case 52:
					return "Le rang ne sera pas affecté";
				case 53:
					return "Choisissez un mode de jeu";
				case 54:
					return "Partie VS IA";
				case 55:
					return "Multi-Joueur";
				default:
					return "";

				}
			} else {
				switch (id) {
				case 1:
					return "You Win";
				case 2:
					return "You Loose";
				case 3:
					return "Draw";
				case 4:
					return "New Elo";
				case 5:
					return "New Rank";
				case 6:
					return "Again";
				case 7:
					return "Menu";
				case 8:
					return "Computer";
				case 9:
					return "Wait Match";
				case 10:
					return "Yes";
				case 11:
					return "No";
				case 12:
					return "Quit The Game ?";
				case 13:
					return "Match Found";
				case 14:
					return "Accept";
				case 15:
					return "Refuse";
				case 16:
					return "Wait The Opponent";
				case 17:
					return "Time Out";
				case 18:
					return "Opponent Refuse";
				case 19:
					return "Back";
				case 20:
					return "Play";
				case 21:
					return "Quit";
				case 22:
					return "Rank";
				case 23:
					return "What is your name ?";
				case 24:
					return "Between 4 And ";
				case 25:
					return "Letters";
				case 26:
					return "Wrong Numbers Of Letters";
				case 27:
					return "Less Than";
				case 28:
					return "Check";
				case 29:
					return "What is your name ?";
				case 30:
					return "Between 4 And ";
				case 31:
					return "Letters";
				case 32:
					return "Wrong Numbers Of Letters";
				case 33:
					return "Less Than ";
				case 34:
					return "Already Taken";
				case 35:
					return "Wait";
				case 36:
					return "Tap For Confirm";
				case 37:
					return "Wait Creation";
				case 38:
					return "No Connection";
				case 39:
					return "Change side";
				case 40:
					return "Change name";
				case 41:
					return "Help";
				case 42:
					return "Options";
				case 43:
					return "Search Opponent";
				case 44:
					return "Start game vs IA meanwhile";
				case 45:
					return "What is your side ?";
				case 46:
					return "How To Play ?";
				case 47:
					return "Tap To Continue";
				case 48:
					return "Select a square of your color";
				case 49:
					return "Next, select where you want to evolve or move your side";
				case 50:
					return "If you move a square close to the other side you take them";
				case 51:
					return "At the end, the side with the higest number of square win. In multiplayer, you win or lose elo points, who can determine your rank.";
				case 52:
					return "Rank Will Not Be Affected";
				case 53:
					return "Select Game Mode";
				case 54:
					return "Game VS BOT";
				case 55:
					return "MultiPlayer";
				default:
					return "";

				}
			}
		}
    }
}
