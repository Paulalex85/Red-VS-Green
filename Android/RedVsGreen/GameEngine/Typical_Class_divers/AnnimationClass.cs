using System;
using GameStateManagement;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using System.Collections.Generic;

namespace RedVsGreen
{
	public class AnnimationClass
	{
		GameScreen _screen;
		RoundedRectangle rect = new RoundedRectangle ();
		int width, height;

		// CHANGEMENT TOUR 
		float _alpha_annim = 0f;
		bool _changement_tour_couleur_bool = false;
		bool _tour_joueur = false;
		PlayClass.Couleurs _couleur_joueur;
		Compteur_Time _changement_tour_couleur_timer = new Compteur_Time(300f);
		Texture2D fond_rouge, fond_vert;
		Vector2 _position_gauche, _position_droite;

		public AnnimationClass (GameScreen screen, PlayClass.Couleurs couleur_joueur, Vector2 position_gauche, Vector2 position_droite)
		{
			_screen = screen;
			_couleur_joueur = couleur_joueur;
			_position_droite = position_droite;
			_position_gauche = position_gauche;
			LoadContent ();
		}

		private void LoadContent()
		{
			width =  _screen.ScreenManager.GraphicsDevice.Viewport.Width;
			height = _screen.ScreenManager.GraphicsDevice.Viewport.Height;

			fond_rouge = rect.Texture_Rounded_Rectangle (_screen.ScreenManager.GraphicsDevice, (int)(width*0.4),(int)(height*0.12),new Color(246,94,59),(int)(width*0.01),(int)(height*0.01));
			fond_vert = rect.Texture_Rounded_Rectangle (_screen.ScreenManager.GraphicsDevice, (int)(width*0.4),(int)(height*0.12),new Color(71,164,71),(int)(width*0.01),(int)(height*0.01));
		}

		public void Changement_Tour(bool tour_joueur)
		{
			_tour_joueur = tour_joueur;
			_changement_tour_couleur_bool = true;
		}

		public void Update_Annim(float timer)
		{
			if (_changement_tour_couleur_bool) {
				_alpha_annim = _changement_tour_couleur_timer._timer / _changement_tour_couleur_timer._timer_max;
				if (_changement_tour_couleur_timer.IncreaseTimer (timer)) {
					_changement_tour_couleur_bool = false;
					_alpha_annim = 1;
				}
			}
		}

		public void Draw_Annim()
		{
			if (fond_vert != null) {
				if (_tour_joueur) {
					if (_couleur_joueur == PlayClass.Couleurs.Green) {
						_screen.ScreenManager.SpriteBatch.Draw (fond_vert, _position_droite, Color.White * _alpha_annim*0.5f);
					} else {
						_screen.ScreenManager.SpriteBatch.Draw (fond_rouge, _position_gauche, Color.White * _alpha_annim*0.5f);
					}
				} else {
					if (_couleur_joueur == PlayClass.Couleurs.Green) {
						_screen.ScreenManager.SpriteBatch.Draw (fond_rouge, _position_gauche, Color.White * _alpha_annim*0.5f);
					} else {
						_screen.ScreenManager.SpriteBatch.Draw (fond_vert, _position_droite, Color.White * _alpha_annim*0.5f);
					}
				}
			}
		}

		public void Draw_Annim(bool is_joueur_1)
		{
			Texture2D texture;
			Vector2 position;

			if (fond_vert != null) {
				if (_tour_joueur) {
					if (is_joueur_1) {
						position = _position_gauche;
					} else {
						position = _position_droite;
					}

					if (_couleur_joueur == PlayClass.Couleurs.Green) {
						texture = fond_vert;
					} else {
						texture = fond_rouge;
					}
				} else {
					if (is_joueur_1) {
						position = _position_droite;
					} else {
						position = _position_gauche;
					}

					if (_couleur_joueur == PlayClass.Couleurs.Green) {
						texture = fond_rouge;
					} else {
						texture = fond_vert;
					}
				}

				_screen.ScreenManager.SpriteBatch.Draw (texture, position, Color.White * _alpha_annim*0.5f);
			}
		}
	}
}

