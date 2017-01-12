using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shooter
{
    class Enemy
    {
        //representacion de la animacion del enemigo
        public Animation EnemyAnimation;

        public Vector2 Position;
        //estado de la clase enemiga
        public bool Active;
        //daño que los enemigos infringen 
        public int Damage;
        //puntaje que da el enemigo
        public int Value;

        public bool isDead;

        //tamaño
        public int Width
        {
            get { return EnemyAnimation.FrameWidth; }
        }

        public int Height
        {
            get { return EnemyAnimation.FrameHeight; }
        }
        //velocidad del enemigo
        float enemyMoveSpeed;

        public void Initialize(Animation animation, Vector2 position)
        {
            //cargo la textura de la nave enemiga
            EnemyAnimation = animation;

            //seteo la posicion
            Position = position;

            Active = true;

            //daño que produce el enemigo
            Damage = 10;
            //seteo velocidad del enemigo
            enemyMoveSpeed = 8f;
            //puntaje que me da cada enemigo muerto
            Value = 100;
        }

        public void Update(GameTime gameTime)
        {
            Position.X -= enemyMoveSpeed;
            EnemyAnimation.Position = Position;
            EnemyAnimation.Update(gameTime);

            if (Position.X < -Width || this.isDead)
            {
                Active = false;
            }

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            //dibujo al enemigo
            EnemyAnimation.Draw(spriteBatch);
        }
    }
}
