using System;
using Facebook;
using Android.Content;
using GameStateManagement;
using Xamarin.Auth;
using Android.App;

namespace RedVsGreen
{
	public class Facebook_Class : Activity
	{
		private const string AppId = "1432676373681912";
		private const string ExtendedPermissions = "user_about_me";

		FacebookClient fb;
		string accessToken;
		bool isLoggedIn;

		public Facebook_Class ()
		{
		}

		public void Login()
		{

			//StartActivity (typeof(Connect_FB));
			/*var webAuth = new Intent (this, typeof (Connect_FB));
			webAuth.PutExtra ("AppId", AppId);
			webAuth.PutExtra ("ExtendedPermissions", ExtendedPermissions);
			StartActivityForResult (webAuth, 0);*/
		}
	}
}

