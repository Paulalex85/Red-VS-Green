using System;
using System.Json;
using System.Threading.Tasks;
using Android.App;
using Android.Widget;
using Android.OS;
using Xamarin.Auth;
using GameStateManagement;

namespace RedVsGreen
{
	public class Connect_Facebook : GameScreen
	{
		public Connect_Facebook()
		{
			LoginToFacebook ();
		}

		void LoginToFacebook ()
		{
			var auth = new OAuth2Authenticator (
				clientId: "1432676373681912",
				scope: "",
				authorizeUrl: new Uri ("https://m.facebook.com/dialog/oauth/"),
				redirectUrl: new Uri ("http://www.facebook.com/connect/login_success.html"));

			auth.AllowCancel = true;

			// If authorization succeeds or is canceled, .Completed will be fired.
			auth.Completed += (s, ee) => {
				if (!ee.IsAuthenticated) {
					Quitter();
					return;
				}

				// Now that we're logged in, make a OAuth2 request to get the user's info.
				var request = new OAuth2Request ("GET", new Uri ("https://graph.facebook.com/me"), null, ee.Account);
				request.GetResponseAsync().ContinueWith (t => {
					if (t.IsFaulted) {
						Quitter();
					} else if (t.IsCanceled)
					{
						Quitter();
					}
					else {
						Quitter();
						/*var obj = JsonValue.Parse (t.Result.GetResponseText());
						builder.SetMessage ("Name: " + obj["name"]);*/
					}
				}, UIScheduler);
			};

			var intent = auth.GetUI (Game1.Activity.ApplicationContext);
			Game1.Activity.StartActivity(intent);
		}

		private void Quitter()
		{
			/*for (int i = 0; i < ScreenManager.GetScreens ().Length; i++) {
				ScreenManager.RemoveScreen (ScreenManager.GetScreens () [i]);
			}
			ScreenManager.AddScreen (new MainMenuScreen (TransitionClass.Sens_Transition.Left));*/
		}

		private static readonly TaskScheduler UIScheduler = TaskScheduler.FromCurrentSynchronizationContext();
	}
}

