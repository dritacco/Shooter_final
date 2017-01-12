using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;


namespace Shooter
{
    class ParallaxingBackground
    {
        //La imagen representa el fondo 
        Texture2D texture;

        //pisiciones del fondo
        Vector2[] positions;
                
        //velocidad de movimiento del fondo
        int speed;

        int bgHeight;
        int bgWidth;

        public void Initialize(ContentManager content, String texturePath, int screenWidth, int screenHeight, int speed)
        {
            bgHeight = screenHeight;
            bgWidth = screenWidth;

            //carga el fondo que estaremos usando
            texture = content.Load<Texture2D>(texturePath);

            //ver la velocidad del fondo
            this.speed = speed;

            positions = new Vector2[screenWidth / texture.Width + 2];

            for (int i = 0; i < positions.Length; i++)
            {
                positions[i] = new Vector2(i * texture.Width, 0);
            }


        }

        public void Update(GameTime gameTime)
        {
            if (positions != null)
            { 
            //actualizar la posicion  del fondo
            for (int i = 0; i < positions.Length; i++)
            {
                //actualizamos la posicion de la pantalla agregandole velocidad
                positions[i].X += speed;

                if (speed <= 0)
                {
                   if (positions[i].X <= -texture.Width)
                    {
                        positions[i].X = texture.Width * (positions.Length - 1);

                    }
                }
                else
                {
                    if (positions[i].X >= -texture.Width * (positions.Length - 1))
                    {
                        positions[i].X = -texture.Width;

                    }
                }
            }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (positions != null) { 
            for (int i = 0; i < positions.Length; i++)
            {
                Rectangle rectBg = new Rectangle((int)positions[i].X, (int)positions[i].Y, bgWidth, bgHeight);
                spriteBatch.Draw(texture, rectBg, Color.White);
            }
            }
        }

    }
}
