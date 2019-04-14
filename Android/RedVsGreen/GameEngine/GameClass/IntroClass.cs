using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using GameStateManagement;

namespace RedVsGreen
{
	public class IntroClass
	{
		public enum Statut_intro {
			Start, //1s
			Red, //2s
			Green, //2s
			Fade_Out_Player, //1s
			Finish_Fade_Out, //1s
			End
		}

		public Statut_intro _statut = Statut_intro.Start;
		GameScreen _screen;

		Police_Size_Manage font_manage;
		Languages langue= new Languages();
		SpriteFont font_bold, font_35;
		Color color_green = new Color(71,164,71), color_red = new Color(246,94,59);
		int width, height;
		Texture2D red_t, green_t;
		Rectangle _r;
		RoundedRectangle rectangle = new RoundedRectangle ();

		float _timeur = 0f;
		string[] _data = new string[6];
		float _position_destination_X_red, _position_destination_X_green;
		float _alpha = 1f, _alpha_texte = 1f;
		Compteur_Time _timer_fade_out;
		Compteur_Time _timer_fade_out_fond;

		Info_Intro _red_name, _red_rank, _red_elo, _green_name, _green_rank, _green_elo;

		//TIMERS 
		float _temps_red = 500f;
		float _temps_green = 1500f;
		float _temps_Fade_Out_Player = 3500f;
		float _temps_Finish_Fade_Out = 4500f;
		float _temps_End = 5000f;

		public IntroClass (GameScreen screen, string[] data)
		{
			_screen = screen;
			Array.Copy (data, _data, 6);
			LoadContent ();
		}

		private void LoadContent()
		{
			width = _screen.ScreenManager.GraphicsDevice.Viewport.Width;
			height = _screen.ScreenManager.GraphicsDevice.Viewport.Height;

			_r = new Rectangle (0, 0, (int)(width / 2), height);
			red_t = rectangle.Texture_Rounded_Rectangle (_screen.ScreenManager.GraphicsDevice, (int)(width*0.75), (int)(height*2), color_red, 0, 0);
			green_t = rectangle.Texture_Rounded_Rectangle (_screen.ScreenManager.GraphicsDevice, (int)(width*.75), (int)_r.Height, color_green, 0, 0);

			font_manage = new Police_Size_Manage (height, width, _screen);
			font_bold = font_manage.Get_Regular_Font ();
			font_35 = font_manage.Get_Bold_Gros2_Font ();


			_timer_fade_out = new Compteur_Time (_temps_Finish_Fade_Out - _temps_Fade_Out_Player);
			_timer_fade_out_fond = new Compteur_Time (_temps_End - _temps_Finish_Fade_Out);

			bool _add_deja_fait = false;
			if (_data [2] != null && _data [2] != "") {
				string fin_string = _data [2].Substring (_data [2].Length - 3, 3);
				if (fin_string == "pts") {
					_add_deja_fait = true;
				}
			} else if(_data [5] != null&& _data [5] != "")  {
				string fin_string = _data [5].Substring (_data [5].Length - 3, 3);
				if (fin_string == "pts") {
					_add_deja_fait = true;
				}
			}

			if (!_add_deja_fait) {
				if (langue.lang == "FR") {
					if (_data [1] != "") {
						if (_data [1] == "1") {
							_data [1] = _data [1] + "er";
						} else {
							_data [1] = _data [1] + "ème";
						}
					}

					if (_data [4] != "") {
						if (_data [4] == "1") {
							_data [4] = _data [4] + "er";
						} else {
							_data [4] = _data [4] + "ème";
						}
					}
				} else {
					if (_data [1] != "") {
						if (_data [1] == "1") {
							_data [1] = _data [1] + "st";
						} else if (_data [1] == "2") {
							_data [1] = _data [1] + "nd";
						} else {
							_data [1] = _data [1] + "th";
						}
					}

					if (_data [4] != "") {
						if (_data [4] == "1") {
							_data [4] = _data [4] + "st";
						} else if (_data [4] == "2") {
							_data [4] = _data [4] + "nd";
						} else {
							_data [4] = _data [4] + "th";
						}
					}
				}

				if (_data [2] != "") {
					_data [2] = _data [2] + "pts";
				}
				if (_data [5] != "") {
					_data [5] = _data [5] + "pts";
				}
			}

			_red_name = new Info_Intro(_data[0], new Vector2((float)(0 - font_bold.MeasureString(_data[0]).X*font_manage._scale), (float)(height * 0.2)), true, _screen, font_bold);
			_red_rank = new Info_Intro(_data[1], new Vector2((float)(0 - font_bold.MeasureString(_data[1]).X*font_manage._scale), (float)(height * 0.25)), true, _screen, font_bold);
			_red_elo = new Info_Intro(_data[2], new Vector2((float)(0 - font_bold.MeasureString(_data[2]).X*font_manage._scale), (float)(height * 0.3)), true, _screen, font_bold);

			_green_name = new Info_Intro(_data[3], new Vector2((float)(width), (float)(height * 0.7)), false, _screen, font_bold);
			_green_rank = new Info_Intro(_data[4], new Vector2((float)(width), (float)(height * 0.75)), false, _screen, font_bold);
			_green_elo = new Info_Intro(_data[5], new Vector2((float)(width), (float)(height * 0.8)), false, _screen, font_bold);

			_position_destination_X_red = (float)(width * 0.1);
			_position_destination_X_green = (float)(width * 0.65);

		}

		public static double Degrees2Radians (double degrees) {
			return Math.PI / 180 * degrees;
		}

		public void Update(float timer)
		{
			_timeur += timer;
			if (_timeur > _temps_End) {
				_statut = Statut_intro.End;
			} else if (_timeur > _temps_Finish_Fade_Out) {
				_statut = Statut_intro.Finish_Fade_Out;
			} else if (_timeur > _temps_Fade_Out_Player) {
				_statut = Statut_intro.Fade_Out_Player;
			} else if (_timeur > _temps_green) {
				_statut = Statut_intro.Green;
			} else if (_timeur > _temps_red) {
				_statut = Statut_intro.Red;
			} else {
				_statut = Statut_intro.Start;
			}


			if (_statut == Statut_intro.Red) {
				if (_timeur > _temps_red && _red_name._statut_info == Info_Intro.Statut_info.Out) {
					_red_name._statut_info = Info_Intro.Statut_info.Fade_In;
				}
				if (_timeur > _temps_red + 200f && _red_rank._statut_info == Info_Intro.Statut_info.Out) {
					_red_rank._statut_info = Info_Intro.Statut_info.Fade_In;
				}
				if (_timeur > _temps_red + 400f && _red_elo._statut_info == Info_Intro.Statut_info.Out) {
					_red_elo._statut_info = Info_Intro.Statut_info.Fade_In;
				}
			} else if (_statut == Statut_intro.Green) {
				if (_timeur > _temps_green && _green_name._statut_info == Info_Intro.Statut_info.Out) {
					_green_name._statut_info = Info_Intro.Statut_info.Fade_In;
				}
				if (_timeur > _temps_green + 200f && _green_rank._statut_info == Info_Intro.Statut_info.Out) {
					_green_rank._statut_info = Info_Intro.Statut_info.Fade_In;
				}
				if (_timeur > _temps_green + 400f && _green_elo._statut_info == Info_Intro.Statut_info.Out) {
					_green_elo._statut_info = Info_Intro.Statut_info.Fade_In;
				}
			} else if (_statut == Statut_intro.Fade_Out_Player) {
				_alpha_texte = 1 - (_timer_fade_out._timer / (_timer_fade_out._timer_max/2));
				if (_timer_fade_out.IncreaseTimer (timer)) {
					_alpha_texte = 0f;
				}
				_green_elo._alpha = _alpha_texte;
				_green_name._alpha = _alpha_texte;
				_green_rank._alpha = _alpha_texte;
				_red_elo._alpha = _alpha_texte;
				_red_name._alpha = _alpha_texte;
				_red_rank._alpha = _alpha_texte;

				_position_destination_X_red = _position_destination_X_red + ((width - _position_destination_X_red) * (_timer_fade_out._timer / _timer_fade_out._timer_max ));
				_position_destination_X_green = (float)( _position_destination_X_green - ((width*0.4  + _position_destination_X_green) * (_timer_fade_out._timer / _timer_fade_out._timer_max )));

			} else if (_statut == Statut_intro.Finish_Fade_Out) {
				_alpha = 1 - (_timer_fade_out_fond._timer / _timer_fade_out_fond._timer_max);
				if (_timer_fade_out_fond.IncreaseTimer (timer)) {
					_alpha = 0f;
				}
			}

			if (_statut == Statut_intro.Green || _statut == Statut_intro.Red) {
				float _temps_max = _temps_Fade_Out_Player - _temps_red;
				int _distance_X_total = (int)(width * 0.2);
				float _to_add = _distance_X_total * (timer / _temps_max);
				_position_destination_X_green -= _to_add;
				_position_destination_X_red += _to_add;
			}

			_green_elo._position_destination_X = _position_destination_X_green;
			_green_name._position_destination_X = _position_destination_X_green;
			_green_rank._position_destination_X = _position_destination_X_green;

			_red_elo._position_destination_X = _position_destination_X_red;
			_red_name._position_destination_X = _position_destination_X_red;
			_red_rank._position_destination_X = _position_destination_X_red;

			_green_elo.Update (timer);
			_green_name.Update (timer);
			_green_rank.Update (timer);
			_red_elo.Update (timer);
			_red_name.Update (timer);
			_red_rank.Update (timer);

		}

		public void Draw()
		{
			//DRAW RECTANGLE
			_screen.ScreenManager.SpriteBatch.Draw (red_t, new Rectangle((int)(width),(int)(0 - width),width, height*2), null, Color.White* _alpha,(float)( Degrees2Radians(45)), new Vector2 (0, 0), SpriteEffects.None, 1f); 
			_screen.ScreenManager.SpriteBatch.Draw (green_t, new Rectangle((int)(width),(int)(height*0.245),width, height), null, Color.White * _alpha, (float)( Degrees2Radians(45)), new Vector2 (0, 0), SpriteEffects.None, 1f); 


			_screen.ScreenManager.SpriteBatch.DrawString (font_35, "VS", new Vector2 ((float)(width *0.45), (float)(height / 2)), Color.White * _alpha, 0f, Vector2.Zero, font_manage._scale, SpriteEffects.None, 1f);

			_green_elo.Draw (_alpha_texte,font_manage._scale);
			_green_name.Draw (_alpha_texte,font_manage._scale);
			_green_rank.Draw (_alpha_texte,font_manage._scale);
			_red_elo.Draw (_alpha_texte,font_manage._scale);
			_red_name.Draw (_alpha_texte,font_manage._scale);
			_red_rank.Draw (_alpha_texte,font_manage._scale);
		}
	}

	public class Info_Intro
	{
		public string _name = "";
		public float _alpha = 0f;
		public Vector2 _position;
		public float _position_destination_X;
		public bool _is_red;

		Compteur_Time _timer;

		float _temps_annimation_in = 300f;

		GameScreen _screen;
		SpriteFont _font;

		public enum Statut_info{
			Out,
			Fade_In,
			Reste_mother_fucker
		}

		public Statut_info _statut_info = Statut_info.Out;

		public Info_Intro(string name, Vector2 position, bool is_red, GameScreen screen, SpriteFont font)
		{
			_name = name;
			_position = position;
			_is_red = is_red;
			_font = font;
			_screen = screen;

			_timer = new Compteur_Time (_temps_annimation_in);
		}

		public void Update(float timer)
		{
			if (_statut_info == Statut_info.Fade_In) {
				if (_timer.IncreaseTimer (timer)) {
					_alpha = 1f;
					_position.X = _position_destination_X;
					_statut_info = Statut_info.Reste_mother_fucker;
				}

				_alpha = _timer._timer / _timer._timer_max;

				if (_is_red) {
					_position.X = _position.X + ((_position_destination_X - _position.X) * (_timer._timer / _timer._timer_max)); 
				} else {
					_position.X = _position.X - ((_position.X - _position_destination_X) * (_timer._timer / _timer._timer_max)); 
				}
			} else if (_statut_info == Statut_info.Reste_mother_fucker) {
				_position.X = _position_destination_X;
			}
		}

		public void Draw(float alpha,float scale)
		{
			_screen.ScreenManager.SpriteBatch.DrawString (_font, _name, _position, Color.White * alpha, 0f, Vector2.Zero, scale, SpriteEffects.None, 1f);
		}
	}
}