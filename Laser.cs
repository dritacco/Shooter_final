using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shooter
{
    class Laser
    {
        //animacion del laser
        public Animation LaserAnimation;

        //velocidad en que el laser viaja
        float laserMoveSpeed = 30f;

        //posicion del laser
        public Vector2 Position;

        //daño que genera el laser
        //int damage = 10;

        //Setear  la variable para activar el laser
        public bool Active;

        //largo del laser
        //int Range;

        //el ancho del laser
        public int Width
        {
            get { return LaserAnimation.FrameWidth; }
        }

        //la altura del laser
        public int Height
        {
            get { return LaserAnimation.FrameHeight; }
        }

        public void Initialize(Animation animation, Vector2 position)
        {
            LaserAnimation = animation;
            Position = position;
            Active = true;
        }

        public void Update(GameTime gameTime)
        {
            Position.X += laserMoveSpeed;
            LaserAnimation.Position = Position;
            LaserAnimation.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            LaserAnimation.Draw(spriteBatch);
        }

    }
}
