using System;
using Microsoft.Xna.Framework;
using GameStateManagement;
using Microsoft.Xna.Framework.Graphics;

namespace RedVsGreen
{
	public class LoadingSprite
	{
		Texture2D loading_rectangle;
		RoundedRectangle rect = new RoundedRectangle();
		const int NOMBRE_BARRE_LOADING = 8;
		float[] array_loading;
		int index_loading = 0;
		Compteur_Time _timer_loading = new Compteur_Time(150f);
		Compteur_Time _temps_recherche = new Compteur_Time(1000f);

		public int _taille;
		public Vector2 _position;
		Color _color;
		GameScreen _screen;

		public LoadingSprite (GameScreen screen,int taille, Vector2 position, Color color)
		{
			_taille = taille;
			_position = position;
			_color = color;
			_screen = screen;
			LoadContent ();
		}

		private void LoadContent()
		{
			loading_rectangle = rect.Texture_Rounded_Rectangle (_screen.ScreenManager.GraphicsDevice, (int)(_taille*0.4), (int)(_taille*0.1), _color, 0, 0);
			array_loading = new float[NOMBRE_BARRE_LOADING];
			Set_Array_Loading (index_loading);
		}

		public void Update(float timer)
		{
			if(_timer_loading.IncreaseTimer(timer))
			{
				if (index_loading == NOMBRE_BARRE_LOADING - 1) {
					index_loading = 0;
				} else {
					index_loading++;
				}
				Set_Array_Loading (index_loading);
			}
		}

		private void Set_Array_Loading(int index)
		{
			for (int i = 0; i < NOMBRE_BARRE_LOADING; i++) {
				if (i == index) {
					array_loading [i] = 1;
				} else if (index == 0 && i == NOMBRE_BARRE_LOADING - 1) {
					array_loading [i] = 0.75f;
				} else if (index - 1 == i) {
					array_loading [i] = 0.75f;
				} else if (index == 1 && i == NOMBRE_BARRE_LOADING - 1 || index == 0 && i == NOMBRE_BARRE_LOADING - 2) {
					array_loading [i] = 0.5f;
				} else if (index - 2 == i) {
					array_loading [i] = 0.5f;
				} else {
					array_loading [i] = 0.25f;
				}
			}
		}

		public void Draw(float _alpha)
		{
			Vector2 origin = new Vector2 (loading_rectangle.Width / 2, loading_rectangle.Height / 2);


			for (int i = 0; i < 8; i++) {
				Vector2 position_modificateur = new Vector2 (0, 0);
				switch (i) {
				case 0:
					position_modificateur = new Vector2 (loading_rectangle.Width, 0);
					break;
				case 1:
					position_modificateur = new Vector2 ((float)(loading_rectangle.Width * 0.75), (float)(loading_rectangle.Width * 0.75));
					break;
				case 2:
					position_modificateur = new Vector2 (0, loading_rectangle.Width);
					break;
				case 3:
					position_modificateur = new Vector2 ((float)(-loading_rectangle.Width * 0.75), (float)(loading_rectangle.Width * 0.75));
					break;
				case 4:
					position_modificateur = new Vector2 (-loading_rectangle.Width, 0);
					break;
				case 5:
					position_modificateur = new Vector2 ((float)(-loading_rectangle.Width * 0.75), (float)(-loading_rectangle.Width * 0.75));
					break;
				case 6:
					position_modificateur = new Vector2 (0, -loading_rectangle.Width);
					break;
				case 7:
					position_modificateur = new Vector2 ((float)(loading_rectangle.Width * 0.75), (float)(-loading_rectangle.Width * 0.75));
					break;
				}
				_screen.ScreenManager.SpriteBatch.Draw (loading_rectangle, new Rectangle ((int)(_position.X + position_modificateur.X), (int)(_position.Y + position_modificateur.Y), loading_rectangle.Width, loading_rectangle.Height), null, Color.White * array_loading [i] * _alpha, (float)(DegreesToRadians (i * 45)), origin, SpriteEffects.None, 1);

			}
		}

		static double DegreesToRadians(double angleInDegrees)
		{
			return angleInDegrees * (Math.PI / 180);

		}
	}
}

