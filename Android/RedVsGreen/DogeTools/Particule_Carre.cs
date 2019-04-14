using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using GameStateManagement;
using Microsoft.Xna.Framework.Graphics;

namespace RedVsGreen.DogeTools
{
    class Particule_Carre
    {
        Texture2D texture;

        Timer _temps_particule = new Timer(500f);
        Vector2 _position;
        int _taille;
        int epaisseur_trait;
        Color _couleur;
        float _rotation = 0;

        Vector2 _origin;

        public Particule_Carre(int rang, Vector2 position, Color color,float rotation, GameScreen screen)
        {
            _position = position;
            _couleur = color;
            _rotation = rotation;
            Initialization(rang, screen);
        }

        private void Initialization(int rang, GameScreen screen)
        {
            int _distance = 0;

            switch (rang)
            {
                case 1: _distance = 10; epaisseur_trait = 3; break;
                case 2: _distance = 20; epaisseur_trait = 2; break;
                case 3: _distance = 40; epaisseur_trait = 1; break;
            }

            _taille = 125 + (_distance * 2);
            _position.X = _position.X - _distance;
            _position.Y = _position.Y - _distance;

            _origin = new Vector2(_taille / 2, _taille / 2);

            Initialize_Texture(_couleur, screen);
        }

        public void Initialize_Texture(Color color, GameScreen screen)
        {
            // SET CARRE
            texture = new Texture2D(screen.ScreenManager.GraphicsDevice, _taille, _taille, false, SurfaceFormat.Color);
            Color[] c = new Color[_taille * _taille];
            for (int i = 0; i < c.Length; ++i)
            {
                if (i < (epaisseur_trait * _taille) || i > (_taille * _taille) - (epaisseur_trait * _taille) || epaisseur_trait > (i % _taille) || _taille - epaisseur_trait <= (i % _taille))
                {
                    c[i] = _couleur;
                }
                else
                {
                    c[i] = _couleur * 0;
                }
            }
            texture.SetData(c);

        }

        public bool VerificationTempsParticule(float timer)
        {
            if (_temps_particule.IncreaseTimer(timer))
            {
                return true;
            }
            return false;
        }

        private Vector2 RotatePoint(Vector2 pointToRotate, Vector2 centerPoint, double _angleInRadians)
        {
            double angleInRadians = _angleInRadians;
            double cosTheta = Math.Cos(angleInRadians);
            double sinTheta = Math.Sin(angleInRadians);
            return new Vector2
            {
                X =
                    (int)
                    (cosTheta * (pointToRotate.X - centerPoint.X) -
                    sinTheta * (pointToRotate.Y - centerPoint.Y) + centerPoint.X),
                Y =
                    (int)
                    (sinTheta * (pointToRotate.X - centerPoint.X) +
                    cosTheta * (pointToRotate.Y - centerPoint.Y) + centerPoint.Y)
            };
        }

        public void DrawCarre(GameScreen screen)
        {
            SpriteBatch spriteBatch = screen.ScreenManager.SpriteBatch;
            SpriteFont font = screen.ScreenManager.Font;

            spriteBatch.Draw(texture, _position + _origin, null, Color.White, _rotation, _origin, 1, SpriteEffects.None, 0);
        }
    }
}
