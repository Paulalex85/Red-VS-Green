using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using GameStateManagement;

namespace RedVsGreen
{
	public class Divers_Method
	{
		public Divers_Method ()
		{
		}

		public string[] Text_In_Button_Center(String text, SpriteFont font, Vector2 box, int marge_vertical, float scale)
		{
			String line = String.Empty;
			String[] ligne_array_final;
			String[] wordArray;
			if (!text.Contains (" ")) {
				return ligne_array_final = new string[1]{ text };
			} else {
				wordArray = text.Split (' ');
			}

			int i = 0;
			int k = 1;

			//Calcul du nombre de ligne
			int taille_height = (int)(font.MeasureString("GORGE").Y * scale);
			int nbr_ligne = (int)(box.Y - (marge_vertical*2)) / taille_height;


			// Taille total de la ligne
			int taille_total_string = (int)(font.MeasureString (text).X * scale);
			int taille_par_ligne = taille_total_string / nbr_ligne;

			if (nbr_ligne == 0|| taille_total_string < box.X) {
				nbr_ligne = 1;
			}
			ligne_array_final = new string[nbr_ligne];

			if (nbr_ligne == 1) {
				ligne_array_final [0] = text;
				return ligne_array_final;
			}

			foreach (String word in wordArray)
			{
				if (font.MeasureString (line + word).X * scale > taille_par_ligne && (nbr_ligne - 1) != i) {
					ligne_array_final [i] = line;
					line = String.Empty;
					i++;
				} 

				line = line + word + ' ';
				if (wordArray.Length == k) {

				}

				if ((nbr_ligne - 1) == i && wordArray.Length == k) {
					ligne_array_final [i] = line;
					line = String.Empty;
					i++;
				}
				k++;
			}
			return ligne_array_final;
		}

		public String parseText(String text, SpriteFont font, Rectangle textBox, float scale)
		{
			String line = String.Empty;
			String returnString = String.Empty;
			String[] wordArray = text.Split (' ');

			foreach (String word in wordArray)
			{
				if (font.MeasureString(line + word).Length() *scale > textBox.Width)
				{
					returnString = returnString + line + '\n';
					line = String.Empty;
				}

				line = line + word + ' ';
			}

			return returnString + line;
		}
	}
}

