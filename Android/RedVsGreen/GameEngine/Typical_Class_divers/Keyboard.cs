using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameStateManagement;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Content;
using System.IO.IsolatedStorage;

namespace RedVsGreen
{
    class Keyboard :GameScreen
    {
        public enum TypeLettre
        {
            Lettre,
            Effacer,
            Maj_Min
        }

        public List<Keyboard_Lettre> liste_lettre = new List<Keyboard_Lettre>();
		int _position_Y, width, height;
        SpriteFont _font_keyboard;
        Texture2D clear;
		float _texture_scale;

        public bool _tapped = false;
        public string _letter_tapped = "";
		public TypeLettre _type_tapped;
		string lang;

		public Keyboard(int position_Y, SpriteFont font, Texture2D _clear, int _width , int _heigth, float scale)
        {
            clear = _clear;
            _font_keyboard = font;
            _position_Y = position_Y;
			width = _width;
			height = _heigth;
			_texture_scale = scale;
            Lettre_Initialize_MAJ();
        }

		public void Input_Keyboard(InputState input)
		{
			foreach (GestureSample gesture in input.Gestures) {
				if (gesture.GestureType == GestureType.Tap) {
					foreach (Keyboard_Lettre caca in liste_lettre) {
						if (caca.HandleTap (gesture.Position)) {
							_letter_tapped = caca._lettre;
							_tapped = true;
							_type_tapped = caca._type;
							break;
						}
					}
				}
			}
		}

		public void Reboot_Variable()
		{
			_tapped = false;
			_letter_tapped = "";
		}

		public void Lettre_Initialize_MAJ()
        {
			lang = (string)IsolatedStorageSettings.ApplicationSettings ["lang"];

			int base_ligne_2 = 10;
			int base_ligne_3 = 20;
			int base_ligne_4 = 30;
			List<string> list_clavier_en = new List<string> {"0","1","2","3","4","5","6","7","8","9", "Q", "W", "E", "R", "T", "Y", "U", "I", "O", "P", "A", "S", "D", "F", "G", "H", "J", "K", "L", "M", "Z", "X", "C", "V", "B", "N" };
			List<string> list_clavier_fr = new List<string> {"0","1","2","3","4","5","6","7","8","9", "A", "Z", "E", "R", "T", "Y", "U", "I", "O", "P", "Q", "S", "D", "F", "G", "H", "J", "K", "L", "M", "W", "X", "C", "V", "B", "N" };
			List<string> list_clavier;
			if (lang == "FR") {
				list_clavier = list_clavier_fr.GetRange (0, list_clavier_fr.Count);
			} else {
				list_clavier = list_clavier_en.GetRange (0, list_clavier_fr.Count);
			}

			for (int i = 0; i < base_ligne_2; i++)
            {
				liste_lettre.Add(new Keyboard_Lettre(_font_keyboard, list_clavier[i], new Vector2((float)((width * 0.01) + ((width * 0.1) * i)), (float)(_position_Y + (height * 0.02))),TypeLettre.Lettre,width, height, _texture_scale));
            }
			for (int i = base_ligne_2; i < base_ligne_3; i++)
            {
				liste_lettre.Add(new Keyboard_Lettre(_font_keyboard, list_clavier[i], new Vector2((float)((width * 0.01) + ((width * 0.1) * (i - base_ligne_2) )), (float)(_position_Y + (height * 0.11))), TypeLettre.Lettre,width, height, _texture_scale));
            }

			for (int i = base_ligne_3; i < base_ligne_4; i++)
			{
				liste_lettre.Add(new Keyboard_Lettre(_font_keyboard, list_clavier[i], new Vector2((float)((width * 0.01) + ((width * 0.1) * (i - base_ligne_3) )), (float)(_position_Y + (height * 0.20))), TypeLettre.Lettre,width, height, _texture_scale));
			}

			for (int i = base_ligne_4; i < base_ligne_4+6; i++)
            {
				liste_lettre.Add(new Keyboard_Lettre(_font_keyboard, list_clavier[i], new Vector2((float)((width * 0.21) + ((width * 0.1) * (i - base_ligne_4))), (float)(_position_Y + (height * 0.29))), TypeLettre.Lettre,width, height, _texture_scale));
            }

			liste_lettre.Add(new Keyboard_Lettre(new Vector2((float)(width * 0.17), (float)(height * 0.075)), new Vector2((float)(width * 0.01), _position_Y + (float)(height * 0.29)),_font_keyboard, "abc",TypeLettre.Maj_Min,_texture_scale));
			liste_lettre.Add(new Keyboard_Lettre(new Vector2((float)(width * 0.17), (float)(height * 0.075)), new Vector2((float)(width - (width *0.18)), _position_Y + (float)(height * 0.29)),clear,TypeLettre.Effacer, _texture_scale));
        }

		public void Changer_en_MIN()
		{
			List<string> list_clavier_en = new List<string> {"0","1","2","3","4","5","6","7","8","9", "q", "w", "e", "r", "t", "y", "u", "i", "o", "p", "a", "s", "d", "f", "g", "h", "j", "k", "l", "m", "z", "x", "c", "v", "b", "n" };
			List<string> list_clavier_fr = new List<string> {"0","1","2","3","4","5","6","7","8","9", "a", "z", "e", "r", "t", "y", "u", "i", "o", "p", "q", "s", "d", "f", "g", "h", "j", "k", "l", "m", "w", "x", "c", "v", "b", "n" };
			List<string> list_clavier;
			if (lang == "FR") {
				list_clavier = list_clavier_fr.GetRange (0, list_clavier_fr.Count);
			} else {
				list_clavier = list_clavier_en.GetRange (0, list_clavier_fr.Count);
			}

			for (int i = 0; i < list_clavier.Count; i++) {
				liste_lettre [i]._lettre = list_clavier [i];
			}
			liste_lettre [list_clavier.Count]._lettre = "ABC";
		}

		public void Changer_en_MAJ()
		{
			List<string> list_clavier_en = new List<string> {"0","1","2","3","4","5","6","7","8","9", "Q", "W", "E", "R", "T", "Y", "U", "I", "O", "P", "A", "S", "D", "F", "G", "H", "J", "K", "L", "M", "Z", "X", "C", "V", "B", "N" };
			List<string> list_clavier_fr = new List<string> {"0","1","2","3","4","5","6","7","8","9", "A", "Z", "E", "R", "T", "Y", "U", "I", "O", "P", "Q", "S", "D", "F", "G", "H", "J", "K", "L", "M", "W", "X", "C", "V", "B", "N" };
			List<string> list_clavier;
			if (lang == "FR") {
				list_clavier = list_clavier_fr.GetRange (0, list_clavier_fr.Count);
			} else {
				list_clavier = list_clavier_en.GetRange (0, list_clavier_fr.Count);
			}

			for (int i = 0; i < list_clavier.Count; i++) {
				liste_lettre [i]._lettre = list_clavier [i];
			}
			liste_lettre [list_clavier.Count]._lettre = "abc";
		}

		public void Draw_Keyboard (GameScreen screen)
		{
			screen.ScreenManager.SpriteBatch.Draw (screen.ScreenManager.BlankTexture, new Rectangle (0, _position_Y, width, height - _position_Y), Color.Black);

			foreach (Keyboard_Lettre caca in liste_lettre) {
				caca.DrawLettre (screen);
			}			
		}

		public void Update_Timer( float timer)
		{
			foreach (Keyboard_Lettre caca in liste_lettre) {
				caca.Timer_Update (timer);
			}
		}
    }
}