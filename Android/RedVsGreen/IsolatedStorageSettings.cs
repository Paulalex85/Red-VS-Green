using System;
using System.Net;
using System.Windows;
using Android.App;
using Android.Content;

namespace System.IO.IsolatedStorage
{
	public class IsolatedStorageSettings
	{
		static IsolatedStorageSettings appSettings = null;
		public static IsolatedStorageSettings ApplicationSettings
		{
			get
			{
				if (appSettings == null)
					appSettings = new IsolatedStorageSettings();
				return appSettings;
			}
		}

		// Returns:
		//     The value associated with the specified key. If the specified key is not
		//     found, a get operation throws a System.Collections.Generic.KeyNotFoundException,
		//     and a set operation creates a new element that has the specified key.
		public object this[string key]
		{
			get
			{
				// Load
				var prefs = Application.Context.GetSharedPreferences("MyApp", FileCreationMode.Private);
				return prefs.GetString(key, null);
			}
			set
			{
				Add(key, value);
			}
		}

		public void Add(string key, object value)
		{
			var prefs = Application.Context.GetSharedPreferences("MyApp", FileCreationMode.Private);
			var prefEditor = prefs.Edit();
			prefEditor.PutString(key, Convert.ToString(value));
			prefEditor.Commit();
		}

		public bool Contains(string key)
		{
			var prefs = Application.Context.GetSharedPreferences("MyApp", FileCreationMode.Private);
			return prefs.Contains(key);
		}

		public void Save()
		{
		}
	}
}