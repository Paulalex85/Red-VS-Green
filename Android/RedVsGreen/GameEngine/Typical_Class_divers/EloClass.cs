using System;

namespace RedVsGreen
{
	public class EloClass
	{
		public EloClass ()
		{
		}

		public float NouveauElo_enPoint(float EloActuel, float EloAdversaire, int statut_partie)
		{
			// statut_partie -> 1win 2 draw 3 loose

			float W = 0.0f;
			float p_D = 0.0f;
			int K = 0;

			//INI VARIABLE
			if (statut_partie == 1) {
				W = 1;
			} else if (statut_partie == 2) {
				W = 0.5f;
			} else {
				W = 0;
			}

			if (EloActuel > 2400 || EloAdversaire > 2400) {
				K = 16;
			} else if (EloActuel < 2100 || EloAdversaire < 2100) {
				K = 32;
			} else {
				K = 24;
			}

			float difference_elo = EloActuel - EloAdversaire;
			p_D = (float)(1 / (1 + Math.Pow (10, (-difference_elo / 400))));

			float caca = K * (W - p_D);
			return caca;

		}
	}
}

