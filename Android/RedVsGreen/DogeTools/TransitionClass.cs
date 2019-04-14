using System;

namespace RedVsGreen
{
	public class TransitionClass
	{
		public enum Statut_Transition {
			On,
			None
		}

		private Statut_Transition _statut;
		public float _timer_max = 1000f;
		Compteur_Time time;

		public float _transition_alpha = 0f;

		public TransitionClass ()
		{
			_statut = Statut_Transition.On;
			time = new Compteur_Time(_timer_max);
		}

		/*public float Position_Transition(float Position_Initial_Width)
		{
			if (_statut == Statut_Transition.On) {
				return this.Position_Transition_ON (Position_Initial_Width);
			} else if (_statut == Statut_Transition.Off) {
				return this.Position_Transition_OFF (Position_Initial_Width);
			} else {
				return Position_Initial_Width;
			}
		}

		private float Position_Transition_ON(float Position_Initial_Width)
		{
			if (_sens == Sens_Transition.Left) {
				return (Position_Initial_Width + _width_ecran) - (_width_ecran * time._timer / time._timer_max);
			} 
			else {
				return (Position_Initial_Width - _width_ecran) + (_width_ecran * time._timer / time._timer_max);
			}
		}

		private float Position_Transition_OFF(float Position_Initial_Width)
		{
			if (_sens == Sens_Transition.Left) {
				return (Position_Initial_Width) - (_width_ecran * time._timer / time._timer_max);
			} 
			else {
				return (Position_Initial_Width) + (_width_ecran * time._timer / time._timer_max);
			}
		}*/

		/*public void Transition_Fade_Out()
		{
			_statut = Statut_Transition.Off;
		}

		public bool Is_End()
		{
			if (_statut == Statut_Transition.End) {
				return true;
			}
			return false;
		}*/

		public void Update_Transition(float gameTime)
		{
			Update_Alpha ();
			if (_statut == Statut_Transition.On) {
				if (time.IncreaseTimer (gameTime)) {
					_statut = Statut_Transition.None;
				}
			}			
		}

		private void Update_Alpha()
		{
			if (_statut == Statut_Transition.On) {
				_transition_alpha = ((time._timer - time._timer_max / 2) / (time._timer_max / 2));
			}
			else {
				_transition_alpha = 1;
			}

			/*if (_statut == Statut_Transition.On) {
				_transition_alpha = time._timer / time._timer_max;
			} else if (_statut == Statut_Transition.Off) {
				_transition_alpha = 1 - (time._timer / time._timer_max);
			} else if (_statut == Statut_Transition.End) {
				_transition_alpha = 0f;
			} 
			else {
				_transition_alpha = 1;
			}*/


			/*if (_statut == Statut_Transition.On) {
				_transition_alpha = ((time._timer - time._timer_max / 2) / (time._timer_max / 2));
			} else if (_statut == Statut_Transition.Off) {
				if (time._timer > time._timer_max / 2) {
					_transition_alpha = 0f;
				} else {
					_transition_alpha = 1 - ((time._timer *2) / time._timer_max);
				}
			} else if (_statut == Statut_Transition.End) {
				_transition_alpha = 0f;
			} else {
				_transition_alpha = 1;
			}*/

		}

		/*public void Lancer_Annimation_OFF(Sens_Transition sens)
		{
			_sens = sens;
			_statut = Statut_Transition.Off;
		}*/
	}
}

