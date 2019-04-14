using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Microsoft.Xna.Framework;
using Android.Gms.Ads;

namespace RedVsGreen
{
	[Activity (Label = "RedVsGreen", 
		MainLauncher = true,
		Icon = "@drawable/icon",
		Theme = "@style/Theme.Splash",
		AlwaysRetainTaskState = true,
		LaunchMode = Android.Content.PM.LaunchMode.SingleInstance,
		ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation |
		Android.Content.PM.ConfigChanges.KeyboardHidden |
		Android.Content.PM.ConfigChanges.Keyboard)]
	public class Activity1 : AndroidGameActivity
	{
		private const string TEST_DEVICE_ID = "YOUR_DEVICE_ID";
		private AdView adView;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Create our OpenGL view, and display it
			Game1.Activity = this;
			var g = new Game1 ();
			//SetContentView (g.Window);
			createAds(g.Window);
			g.Run ();
		}

		private void createAds(AndroidGameWindow window)
		{
			var frameLayout = new FrameLayout(this);
			var linearLayout = new LinearLayout(this);

			linearLayout.Orientation = Orientation.Horizontal;
			linearLayout.SetGravity(Android.Views.GravityFlags.Right | Android.Views.GravityFlags.Bottom);

			frameLayout.AddView(window);

			adView = new AdView(this);
			adView.AdUnitId = AD_UNIT_ID;
			adView.AdSize = AdSize.Banner;

			linearLayout.AddView(adView);
			frameLayout.AddView(linearLayout);
			SetContentView(frameLayout);

			try
			{
				// Initiate a generic request.
				var adRequest = new AdRequest.Builder()
					.AddTestDevice(AdRequest.DeviceIdEmulator)
					.AddTestDevice(TEST_DEVICE_ID)
					.Build();

				// Load the adView with the ad request.
				adView.LoadAd(adRequest);
			}
			catch (Exception ex)
			{
				// your error logging goes here
			}
		}

	}
}


