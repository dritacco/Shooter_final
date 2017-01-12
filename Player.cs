using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;


namespace Shooter
{
    class Player
    {
        //imagen del player
        //public Texture2D PlayerTexture;
        //animacion del player
        public Animation PlayerAnimation;

        //Posicion  del jugador
        public Vector2 Position;
        //Estado del jugador
        public bool Active;
        //Cantidad de puntos de vida que el jugador tiene
        public int Health;

        /* se comento porque se mejoro la animacion
        //Tamaño del ancho de la nave
        public int Width
        {
            get { return PlayerTexture.Width; }
        }
        //Tamaño del alto de la nave
        public int Height
        {
            get { return PlayerTexture.Height; }
        }
        */

        //Tamaño del ancho de la nave
        public int Width
        {
            get { return PlayerAnimation.FrameWidth; }
        }
        //Tamaño del alto de la nave
        public int Height
        {
            get { return PlayerAnimation.FrameHeight; }
        }

        /* Se comento por que se agrego una version con animacion
         * 
        public void  Initialize(Texture2D texture, Vector2  position)
        {
            // Inicializamos las configuraciones del jugador del jugador
            PlayerTexture = texture;

            Position = position;
            //Activo el jugador
            Active = true;
            //Energia del jugador
            Health = 100;
        }
        */

        public void Initialize(Animation animation, Vector2 position)
        {
            PlayerAnimation = animation;
            // posicion del jugador
            Position = position;
            //Activo el jugador
            Active = true;
            //Energia del jugador
            Health = 20;
        }

        public void update(GameTime gameTime)
        {
            PlayerAnimation.Position = Position;
            PlayerAnimation.Update(gameTime);

            if (this.Health <= 0)
            {
                this.Position.Y = -1000;
                Active = false;
            }

        }

        /* Se comenta porque se agrego animacion
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(PlayerTexture, Position, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

        }
        */

        public void Draw(SpriteBatch spriteBatch)
        {
            if(this.Active)
            {
                PlayerAnimation.Draw(spriteBatch);
            }
        }

        public void updateHealth(int damageTaken)
        {
            this.Health -= damageTaken;
        }
        
    }
}