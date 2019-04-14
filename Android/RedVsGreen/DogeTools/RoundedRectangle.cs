using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace RedVsGreen
{
	public class RoundedRectangle
	{
		public RoundedRectangle ()
		{
		}

		public Texture2D Texture_Rounded_Rectangle(GraphicsDevice graphics, int width, int height, Color background_color, int borderThickness, int borderRadius)
		{
			Texture2D texture = new Texture2D(graphics, width, height, false, SurfaceFormat.Color);
			Color[] color = new Color[width * height];

			for (int x = 0; x < texture.Width; x++)
			{
				for (int y = 0; y < texture.Height; y++)
				{
					color [y * width + x] = ColorBorder (x, y, width, height, borderThickness, borderRadius, background_color);
				}
			}

			texture.SetData<Color>(color);
			return texture;
		}

		public Texture2D Texture_Rounded_Rectangle_Saut_Cercle(GraphicsDevice graphics, int width, int height, Color background_color, Color vide_color, int borderThickness, int borderRadius)
		{
			const float POURCENTAGE_TAILLE_VIDE = 0.7f;
			float bordure = (1 - POURCENTAGE_TAILLE_VIDE) / 2; 

			Texture2D texture = new Texture2D(graphics, width, height, false, SurfaceFormat.Color);
			Color[] color = new Color[width * height];

			for (int x = 0; x < texture.Width; x++)
			{
				for (int y = 0; y < texture.Height; y++)
				{
					color [y * width + x] = ColorBorder (x, y, width, height, borderThickness, borderRadius, background_color);

					if (x >= width * bordure && x <= width * (POURCENTAGE_TAILLE_VIDE + bordure) && y >= height * bordure && y <= height * (POURCENTAGE_TAILLE_VIDE + bordure)) {
						color [y * width + x] = ColorBorder ((int)(x- (bordure*width)), (int)(y- (bordure*height)), (int)(width * POURCENTAGE_TAILLE_VIDE), (int)(height * POURCENTAGE_TAILLE_VIDE), borderThickness, borderRadius, vide_color);
					}
				}
			}

			texture.SetData<Color>(color);
			return texture;
		}

		private Color ColorBorder(int x, int y, int width, int height, int borderThickness, int borderRadius, Color initialColor)
		{
			Rectangle internalRectangle = new Rectangle((borderThickness + borderRadius), (borderThickness + borderRadius), width - 2 * (borderThickness + borderRadius), height - 2 * (borderThickness + borderRadius));

			if (internalRectangle.Contains(x, y)) return initialColor;

			Vector2 origin = Vector2.Zero;
			Vector2 point = new Vector2(x, y);

			if (x < borderThickness + borderRadius)
			{
				if (y < borderRadius + borderThickness)
					origin = new Vector2(borderRadius + borderThickness, borderRadius + borderThickness);
				else if (y > height - (borderRadius + borderThickness))
					origin = new Vector2(borderRadius + borderThickness, height - (borderRadius + borderThickness));
				else
					origin = new Vector2(borderRadius + borderThickness, y);
			}
			else if (x > width - (borderRadius + borderThickness))
			{
				if (y < borderRadius + borderThickness)
					origin = new Vector2(width - (borderRadius + borderThickness), borderRadius + borderThickness);
				else if (y > height - (borderRadius + borderThickness))
					origin = new Vector2(width - (borderRadius + borderThickness), height - (borderRadius + borderThickness));
				else
					origin = new Vector2(width - (borderRadius + borderThickness), y);
			}
			else
			{
				if (y < borderRadius + borderThickness)
					origin = new Vector2(x, borderRadius + borderThickness);
				else if (y > height - (borderRadius + borderThickness))
					origin = new Vector2(x, height - (borderRadius + borderThickness));
			}

			if (!origin.Equals(Vector2.Zero))
			{
				float distance = Vector2.Distance(point, origin);

				if (distance > borderRadius + borderThickness + 1)
				{
					return Color.Transparent;
				}
			}

			return initialColor;
		}
	}
}

