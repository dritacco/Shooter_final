using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shooter
{
    class Animation
    {
        //la imagen representa una coleccion de imagenes para representar la animacion
        Texture2D spriteStrip;

        public float scale;
        //tiempo de actualizacion del ultimo frame
        int elapsedTime;
        //tiempo que mostramos un frame hasta el proximo
        int frameTime;
        //el nro de frames que se mostraran
        int frameCount;
        //indice del frame que se esta mostrando
        int currentFrame;
        //el color del frame que sera mostrado
        Color color;
        //el area de la imagen que se quiere mostrar
        Rectangle sourceRect = new Rectangle();
        //el area donde queremos mostrar la imagen en el juego
        Rectangle destinationRect = new Rectangle();
        //tamaño del frame
        public int FrameWidth;
        public int FrameHeight;
        //estado de la animacion
        public bool Active;
        //determina si la animacion va a seguir corriendo o si se va a desactivar luego de una ejecucion
        public bool Looping;
        //tamaño de un frame dado
        public Vector2 Position;

        public void Initialize(Texture2D texture, Vector2 position, int frameWidth, int frameHeight, int frameCount, 
                                int frametime, Color color, float scale, bool looping)
        {
            this.color = color;
            this.FrameWidth = frameWidth;
            this.FrameHeight = frameHeight;
            this.frameCount = frameCount;
            this.frameTime = frametime;
            this.scale = scale;

            Looping = looping;
            Position = position;
            spriteStrip = texture;
            //setear el tiempo en cero
            elapsedTime = 0;
            currentFrame = 0;

            //animacion activa por defecto
            Active = true;

        }

        public void Update(GameTime gameTime)
        {
            //no actualizar el juego si no esta activo
            if (Active == false) return;
            //actualizar el tiempo
            elapsedTime += (int)gameTime.ElapsedGameTime.TotalMilliseconds;

            if (elapsedTime > frameTime)
            {
                currentFrame++;

            }

            if (currentFrame == frameCount)
            {
                currentFrame = 0;
                if (Looping == false)
                {
                    Active = false;
                }
                //reseteo el tiempo pasado en  cero 
                elapsedTime = 0;
            }

            sourceRect = new Rectangle(currentFrame * FrameWidth, 0, FrameWidth, FrameHeight);
            destinationRect = new Rectangle((int)Position.X - (int)(FrameWidth * scale) / 2,
                (int)Position.Y - (int)(FrameHeight * scale) / 2,
                (int)(FrameWidth * scale),
                (int)(FrameHeight * scale));

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (Active)
            {
                spriteBatch.Draw(spriteStrip, destinationRect, sourceRect, color);
            }
        }

    }
}
