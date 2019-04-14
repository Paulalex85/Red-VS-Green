using System;
using GameStateManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RedVsGreen
{
	public class BackgroundScreen : GameScreen
	{

		Rectangle r;
		Color color_fond = new Color(250,248,239);

		public BackgroundScreen ()
		{
		}

		public override void LoadContent ()
		{
			r = new Rectangle (0, 0, ScreenManager.GraphicsDevice.Viewport.Width, ScreenManager.GraphicsDevice.Viewport.Height);

			base.LoadContent ();
		}

		public override void Update (GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
		{
			base.Update (gameTime, otherScreenHasFocus, coveredByOtherScreen);
		}

		public override void Draw (GameTime gameTime)
		{
			ScreenManager.SpriteBatch.Begin ();

			ScreenManager.SpriteBatch.Draw (ScreenManager.BlankTexture, r, color_fond);

			ScreenManager.SpriteBatch.End ();

			base.Draw (gameTime);
		}
	}
}

