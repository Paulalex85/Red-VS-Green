using System;
using Android.App;
using Android.OS;
using Xamarin.Auth;
using System.Threading.Tasks;

namespace RedVsGreen
{
	[Activity (Label="Facebook")]
	public class Connect_Facebook_Activity : Activity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			var auth = new OAuth2Authenticator (
				clientId: "1432676373681912",
				scope: "",
				authorizeUrl: new Uri ("https://m.facebook.com/dialog/oauth/"),
				redirectUrl: new Uri ("http://www.facebook.com/connect/login_success.html"));

			auth.AllowCancel = true;

			// If authorization succeeds or is canceled, .Completed will be fired.
			auth.Completed += (s, ee) => {
				if (!ee.IsAuthenticated) {
					return;
				}

				// Now that we're logged in, make a OAuth2 request to get the user's info.
				var request = new OAuth2Request ("GET", new Uri ("https://graph.facebook.com/me"), null, ee.Account);
				request.GetResponseAsync().ContinueWith (t => {
					if (t.IsFaulted) {
					} else if (t.IsCanceled)
					{
					}
					else {
						/*var obj = JsonValue.Parse (t.Result.GetResponseText());
						builder.SetMessage ("Name: " + obj["name"]);*/
					}
				}, UIScheduler);
			};

			var intent = auth.GetUI (this.ApplicationContext);
			this.StartActivity (intent);
		}

		private static readonly TaskScheduler UIScheduler = TaskScheduler.FromCurrentSynchronizationContext();
	}
}

