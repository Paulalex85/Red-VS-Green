using System;
using GameStateManagement;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using System.Collections.Generic;

namespace RedVsGreen
{
	public class CaseClass
	{
		public enum Type_Case {
			Obstacle,
			Vide,
			Red,
			Green
		}

		public enum Surbrillance_Type {
			Evolution,
			Saut,
			Rien
		}

		public enum Type_Annimation {
			None,
			On,
			Off,
			Both
		}

		public Type_Annimation _type_annim = Type_Annimation.None;
		GameScreen _screen;
		public Type_Case _type_case;
		public Surbrillance_Type _type_surbrillance = Surbrillance_Type.Rien;
		private Vector2 _position;
		RoundedRectangle rect = new RoundedRectangle();
		public float _scale_annimation = 0f;
		public Compteur_Time _timer_annimation = new Compteur_Time (500f);
		public bool _half_done_annimation_both = false;


		public CaseClass (GameScreen screen,Type_Case type_case, Vector2 position)
		{
			_screen = screen;
			_type_case = type_case;
			_position = position;
		}

		private void LoadContent()
		{
		}

		public bool HandledInput(InputState input, Texture2D case_texture)
		{
			foreach (GestureSample gesture in input.Gestures) {
				if (gesture.GestureType == GestureType.Tap) {
					if (gesture.Position.X > _position.X &&
						gesture.Position.X < _position.X + case_texture.Width &&
						gesture.Position.Y > _position.Y &&
						gesture.Position.Y < _position.Y + case_texture.Height) {
						return true;
					}
				}
			}
			return false;
		}

		public void Update()
		{
		}

		public void Draw(TransitionClass transition,string side,List<Texture2D> texture)
		{
			//DRAW CASE
				Vector2 origin = new Vector2 (texture [0].Width / 2, texture [0].Height / 2);
			if (_type_case == Type_Case.Vide) {
				_screen.ScreenManager.SpriteBatch.Draw (texture [0], new Vector2 (_position.X, _position.Y), Color.White * transition._transition_alpha);
			} else if (_type_case == Type_Case.Red) {
				_screen.ScreenManager.SpriteBatch.Draw (texture [0], new Vector2 (_position.X, _position.Y), Color.White * transition._transition_alpha);
				if (_type_annim == Type_Annimation.Both && !_half_done_annimation_both) {
					_screen.ScreenManager.SpriteBatch.Draw (texture [2], new Vector2 (_position.X + origin.X, _position.Y + origin.Y), null, Color.White * transition._transition_alpha, 0f, origin, _scale_annimation, SpriteEffects.None, 0f);

				} else {
					_screen.ScreenManager.SpriteBatch.Draw (texture [1], new Vector2 (_position.X + origin.X, _position.Y + origin.Y), null, Color.White * transition._transition_alpha, 0f, origin, _scale_annimation, SpriteEffects.None, 0f);

				}
			} else if (_type_case == Type_Case.Green) {
				_screen.ScreenManager.SpriteBatch.Draw (texture [0], new Vector2 (_position.X, _position.Y), Color.White * transition._transition_alpha);
				if (_type_annim == Type_Annimation.Both && !_half_done_annimation_both) {
					_screen.ScreenManager.SpriteBatch.Draw (texture [1], new Vector2 (_position.X + origin.X, _position.Y + origin.Y), null, Color.White * transition._transition_alpha, 0f, origin, _scale_annimation, SpriteEffects.None, 0f);

				} else {
					_screen.ScreenManager.SpriteBatch.Draw (texture [2], new Vector2 (_position.X + origin.X, _position.Y + origin.Y), null, Color.White * transition._transition_alpha, 0f, origin, _scale_annimation, SpriteEffects.None, 0f);

				}
			}

			//DRAW SURBRILLANCE
			Texture2D texture_surbrillance_couleur_saut;
			Texture2D texture_surbrillance_couleur;
			if (side == "1") {//RED
				texture_surbrillance_couleur = texture [1];
				texture_surbrillance_couleur_saut = texture [3];
			} else {
				texture_surbrillance_couleur = texture [2];
				texture_surbrillance_couleur_saut = texture [4];
			}

			if (_type_surbrillance == Surbrillance_Type.Evolution) {
				_screen.ScreenManager.SpriteBatch.Draw (texture_surbrillance_couleur, new Vector2 (_position.X, _position.Y), Color.White * (float)(transition._transition_alpha * 0.5));
			} else if (_type_surbrillance == Surbrillance_Type.Saut) {
				_screen.ScreenManager.SpriteBatch.Draw (texture_surbrillance_couleur_saut, new Vector2 (_position.X, _position.Y), Color.White * (float)(transition._transition_alpha * 0.5));
			}
		}
	}
}

