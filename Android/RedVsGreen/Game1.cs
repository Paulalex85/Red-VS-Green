#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Input;
using GameStateManagement;
using System.IO.IsolatedStorage;

#endregion
namespace RedVsGreen
{
	/// <summary>
	/// This is the main type for your game
	/// </summary>
	public class Game1 : Game
	{
		GraphicsDeviceManager graphics;
		ScreenManager screenManager;

		public Game1 ()
		{
			graphics = new GraphicsDeviceManager (this);
			Content.RootDirectory = "Content";	            
			graphics.IsFullScreen = true;	

			graphics.SupportedOrientations = DisplayOrientation.Portrait;

			screenManager = new ScreenManager(this);
			Components.Add(screenManager);

			AfterSplashScreen ();
		}

		private void AfterSplashScreen()
		{
			string region = System.Globalization.RegionInfo.CurrentRegion.Name;
			string[] blbl = region.Split ('-');
			IsolatedStorageSettings.ApplicationSettings ["lang"] = blbl [0];

			if (!IsolatedStorageSettings.ApplicationSettings.Contains("name"))
			{
				IsolatedStorageSettings.ApplicationSettings["name"] = "";
			}

			if ((string)IsolatedStorageSettings.ApplicationSettings ["name"] == "") {
				screenManager.AddScreen (new SelectionScreen (false));
			} else {
				screenManager.AddScreen (new MainMenuScreen ());
			}
		}

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize ()
		{
			base.Initialize ();
				
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent ()
		{
		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update (GameTime gameTime)
		{

			base.Update (gameTime);
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw (GameTime gameTime)
		{
			graphics.GraphicsDevice.Clear (Color.CornflowerBlue);
		
            
			base.Draw (gameTime);
		}
	}
}

