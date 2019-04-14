using System;
using Android.App;
using Android.OS;
using Java.IO;
using Microsoft.Xna.Framework.Graphics;

namespace RedVsGreen
{
	public class ReadAsset : Activity
	{
		public Texture2D blankTexture;
		public ReadAsset()
		{
		}

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			//blankTexture = Assets.Open ("blank.xnb");
		}
	}
}

