using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using GameStateManagement;
using Microsoft.Xna.Framework;

namespace RedVsGreen.DogeTools
{
    public class Particule_Mini
    {
        Texture2D texture;

        Vector2 _position;
        Vector2 _direction;
        Vector2 _origin;
        int _taille = 2;
        float _vitesse = 1;

        Color _couleur;
        float _rotation = 0;

        GameScreen _screen;

        Random rand = new Random();

        public Particule_Mini(GameScreen screen)
        {
            _screen = screen;
            Initialization(screen);
        }

        public Particule_Mini(GameScreen screen, Vector2 position)
        {
            _screen = screen;
            _position = position;
            InitializationPetitCarre();
        }

        public void InitializationPetitCarre()
        {
            _position.X = _position.X + 15;
            _position.Y = _position.Y + 15;


            int a = rand.Next(0, 2);
            int b = rand.Next(0,2);
            _direction.X = (float)rand.NextDouble();
            _direction.Y = (float)rand.NextDouble();

            if (a == 0)
            {
                _direction.X = -_direction.X;
            }
            if (b == 0)
            {
                _direction.Y = -_direction.Y;
            }


            _origin = new Vector2(_taille / 2, _taille / 2);
            _couleur = RandomColor();

            Initialize_Texture(_couleur, _screen);
        }

        public void Initialization(GameScreen screen)
        {
            int g = rand.Next(0,4);
            switch (g)
            {
                case 0: _position = new Vector2(rand.Next(0, 480), 0);
                    _direction = new Vector2(0, 1); break;
                case 1: _position = new Vector2(rand.Next(0, 480), 800);
                    _direction = new Vector2(0, -1); break;
                case 2: _position = new Vector2(0, rand.Next(0, 800));
                    _direction = new Vector2(1, 0); break;
                case 3: _position = new Vector2(480, rand.Next(0, 800));
                    _direction = new Vector2(-1, 0); break;
            }
            _origin = new Vector2(_taille / 2, _taille / 2);
            _couleur = RandomColor();

            Initialize_Texture(_couleur, screen);
        }

        private Color RandomColor()
        {
            List<Color> _color_annimation = new List<Color>() { new Color(255, 45, 255), new Color(45, 255, 255), new Color(255, 255, 45) };

            int k = rand.Next(0, _color_annimation.Count);
            return _color_annimation[k];
        }

        public void Initialize_Texture(Color color, GameScreen screen)
        {
            // SET CARRE
            texture = new Texture2D(screen.ScreenManager.GraphicsDevice, _taille, _taille, false, SurfaceFormat.Color);
            Color[] c = new Color[_taille * _taille];
            for (int i = 0; i < c.Length; ++i)
            {
                c[i] = _couleur;

            }
            texture.SetData(c);
        }

        public bool VerificationLimite()
        {
            if (_position.Y > 800 || _position.Y < 0 || _position.X < 0 || _position.X > 480)
            {
                return true;
            }
            return false;
        }

        public void UpdatePositionParticule()
        {
            _position.X = _position.X += (_direction.X * _vitesse);
            _position.Y = _position.Y += (_direction.Y * _vitesse);
        }

        public void DrawParticule(GameScreen screen)
        {
            SpriteBatch spriteBatch = screen.ScreenManager.SpriteBatch;
            SpriteFont font = screen.ScreenManager.Font;

            spriteBatch.Draw(texture, _position + _origin, null, Color.White, _rotation, _origin, 1, SpriteEffects.None, 0);
        }
    }
}

