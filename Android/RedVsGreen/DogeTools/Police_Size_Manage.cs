using System;
using Microsoft.Xna.Framework.Graphics;
using GameStateManagement;

namespace RedVsGreen
{
	public class Police_Size_Manage
	{
		int _width,_height, _pixel;
		public float _scale;
		SpriteFont regular_font_petit, regular_font, bold_font, keyboard_font, bold_30, bold_35;
		GameScreen _screen;

		public Police_Size_Manage (int height, int width, GameScreen screen)
		{
			_width = width;
			_height = height;
			_screen = screen;

			_pixel = height * width;

			LoadContent ();
		}

		private void LoadContent()
		{
			_scale = Calcul_Scale_Texture ();

			/*if (_pixel > (1100 * 2000)) {
				bold_30 = _screen.ScreenManager.Game.Content.Load<SpriteFont> ("45_b"); //90
				bold_35 = _screen.ScreenManager.Game.Content.Load<SpriteFont> ("b_52"); //105
				keyboard_font = _screen.ScreenManager.Game.Content.Load<SpriteFont> ("keyboard_35"); // Segoe UI Mono 70 spacing :2 BOLD
				regular_font_petit = _screen.ScreenManager.Game.Content.Load<SpriteFont> ("24"); //50
				regular_font = _screen.ScreenManager.Game.Content.Load<SpriteFont> ("30");//60
				bold_font = _screen.ScreenManager.Game.Content.Load<SpriteFont> ("b_35");//70
			} else if (_pixel > (800 * 1300)) {
				bold_30 = _screen.ScreenManager.Game.Content.Load<SpriteFont> ("60_b"); //60
				bold_35 = _screen.ScreenManager.Game.Content.Load<SpriteFont> ("70_b"); //70
				keyboard_font = _screen.ScreenManager.Game.Content.Load<SpriteFont> ("keyboard_46"); // Segoe UI Mono 46 spacing 2 BOLD
				regular_font_petit = _screen.ScreenManager.Game.Content.Load<SpriteFont> ("30"); //30
				regular_font = _screen.ScreenManager.Game.Content.Load<SpriteFont> ("45");//45
				bold_font = _screen.ScreenManager.Game.Content.Load<SpriteFont> ("b_52");//52
			} else if (_pixel > (500 * 800)) {
				bold_30 = _screen.ScreenManager.Game.Content.Load<SpriteFont> ("45_b"); //45
				bold_35 = _screen.ScreenManager.Game.Content.Load<SpriteFont> ("b_52"); //52
				keyboard_font = _screen.ScreenManager.Game.Content.Load<SpriteFont> ("keyboard_35"); // Segoe UI Mono 35 spacing :2 BOLD
				regular_font_petit = _screen.ScreenManager.Game.Content.Load<SpriteFont> ("24"); //24
				regular_font = _screen.ScreenManager.Game.Content.Load<SpriteFont> ("30");//30
				bold_font = _screen.ScreenManager.Game.Content.Load<SpriteFont> ("b_35");//35
			} else if (_pixel > (400 * 500)) {*/
				bold_30 = _screen.ScreenManager.Game.Content.Load<SpriteFont> ("b_30"); //30
				bold_35 = _screen.ScreenManager.Game.Content.Load<SpriteFont> ("b_35"); //35
				keyboard_font = _screen.ScreenManager.Game.Content.Load<SpriteFont> ("Lettre_keyboard"); // Segoe UI Mono 23 spacing :2 BOLD
				regular_font_petit = _screen.ScreenManager.Game.Content.Load<SpriteFont> ("regular_font_16"); //15
				regular_font = _screen.ScreenManager.Game.Content.Load<SpriteFont> ("regular_font");//20
				bold_font = _screen.ScreenManager.Game.Content.Load<SpriteFont> ("bold_font");//26
			/*} else {
			}*/
		}

		private float Calcul_Scale_Texture()
		{
			double scale = 1.0000;
			int base_width = 480, base_height = 800;
			float difference_width = (float)(_width - base_width);
			float difference_height = (float)(_height - base_height);
			if (difference_height > difference_width) {
				scale = (double)_width / base_width;
			} else {
				scale = (double)_height / base_height;
			}
			return (float)(scale);
		}

		public SpriteFont Get_Regular_Font()
		{
			return regular_font;
		}

		public SpriteFont Get_Regular_Petit_Font()
		{
			return regular_font_petit;
		}

		public SpriteFont Get_Keyboard_Font()
		{
			return keyboard_font;
		}

		public SpriteFont Get_Bold_Font()
		{
			return bold_font;
		}

		public SpriteFont Get_Bold_Gros_Font()
		{
			return bold_30;
		}

		public SpriteFont Get_Bold_Gros2_Font()
		{
			return bold_35;
		}
	}
}

