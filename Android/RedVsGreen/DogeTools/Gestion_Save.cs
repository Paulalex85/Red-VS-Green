using System;
using System.IO.IsolatedStorage;

namespace RedVsGreen
{
	public class Gestion_Save
	{
		const int NBR_PARAMETRE = 1;
		string param = (string)IsolatedStorageSettings.ApplicationSettings ["save"];
		public string[] param_actualise = new string[NBR_PARAMETRE];
		public Gestion_Save ()
		{
			Recuperation_Parametre ();
		}

		private void Recuperation_Parametre()
		{
			if (param != null && param != "") {
				string[] caca = param.Split (new char[] { ' ' });
				param_actualise = caca;
			}
		}

		public void Save_Parametre()
		{
			string caca = "";
			for (int i = 0; i < NBR_PARAMETRE; i++) {
				caca += param_actualise [i] + " ";
			}
			IsolatedStorageSettings.ApplicationSettings ["save"] = caca;
		}
	}
}

