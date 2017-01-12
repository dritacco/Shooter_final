using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Shooter;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace Shooter
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        //agregamos clases para el sonido del laser
        static SoundEffect laserSound;
        static SoundEffectInstance laserSoundInstance;

        static SoundEffect explosionSound;
        static SoundEffectInstance explosionSoundInstance;

        private Song gameMusic;

        //jugador
        Player player;

        //para determinar que tecla está siendo presionada
        KeyboardState currentKeyboardState;
        KeyboardState previousKeyboardState;

        //Velocidad de movimiento del jugador
        float playerMovespeed;

        Texture2D mainBackground;
        Rectangle rectBackground;
        protected float scale = 1f;

        ParallaxingBackground bgLayer1;
        ParallaxingBackground bgLayer2;

        //Enemigos
        Texture2D enemyTexture;
        List<Enemy> enemies;
        //velocidad en  que aparecen los enemigos
        TimeSpan enemySpawnTime;
        TimeSpan previousSpawnTime;
        //generador random
        Random random;

        //Laser
        Texture2D laserTexture;
        //manejar que tan rapido se puede usar el laser
        TimeSpan laserSpawnTime;
        TimeSpan previousLaserSpawnTime;
        //variable para controlar el tiempo entre disparos
        List<Laser> laserBeams;

        //textura la animacion de la explosion
        Texture2D explosionTexture;
        //variable para las explisiones
        List<Explosion> explosions;
        Explosion playerExplosion;

       private SpriteFont fuente;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            //inicia la lista de enem igos
            enemies = new List<Enemy>();
            previousSpawnTime = TimeSpan.Zero;
            //para determinar que tan rapido aparecen  los enemigos
            enemySpawnTime = TimeSpan.FromSeconds(1.0f);
            //inicializo el generador random
            random = new Random();

            //Inicia la clase Player
            player = new Player();
            //velocidad del jugador
            playerMovespeed = 6.1f;

            //iniciar el laser
            laserBeams = new List<Laser>();
            //para determinar cuando puedo hacer un nuevo disparo.
            //calculo los segundos que pasaron para un nuevo disparo...0.3
            const float SECONDS_IN_MINUTE = 60f;
            const float RATE_OF_FIRE = 200f;
            laserSpawnTime = TimeSpan.FromSeconds(SECONDS_IN_MINUTE / RATE_OF_FIRE);
            previousLaserSpawnTime = TimeSpan.Zero;

            //iniciar las explosiones
            explosions = new List<Explosion>();

            //Inicia los fondos
            bgLayer1 = new ParallaxingBackground();
            bgLayer2 = new ParallaxingBackground();
            rectBackground = new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here

            /* Se comenta por que se cambia por una version con animacion
            //Cargo los recursos de player
            Vector2 playerPosition = new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X, GraphicsDevice.Viewport.TitleSafeArea.Y +
                GraphicsDevice.Viewport.TitleSafeArea.Height / 2);
            */

            //player.Initialize(Content.Load<Texture2D>("Graphics/Player.png"), playerPosition); --SIN ANIMACION

            //cargo los recursos para el Player con animacion
            Animation playerAnimation = new Animation();
            Texture2D playerTexture = Content.Load<Texture2D>("Graphics/shipAnimation.png");
            playerAnimation.Initialize(playerTexture, Vector2.Zero, 115, 69, 8, 30, Color.White, 1f, true);

            Vector2 playerPosition = new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X,
                 GraphicsDevice.Viewport.TitleSafeArea.Y + GraphicsDevice.Viewport.TitleSafeArea.Height / 2);
                 player.Initialize(playerAnimation, playerPosition);

            //cargo los enemigos
            enemyTexture = Content.Load<Texture2D>("Graphics/mineAnimation.png");

            //cargo el laser
            laserTexture = Content.Load<Texture2D>("Graphics/laser.png");
            //cargo el sonido del laser
            laserSound = Content.Load<SoundEffect>("Sounds/laserFire");
            laserSoundInstance = laserSound.CreateInstance();

            //cargo la explosion
            explosionSound = Content.Load<SoundEffect>("Sound/Explosion");
            explosionSoundInstance = explosionSound.CreateInstance();
            
            //cargar la musica
            gameMusic = Content.Load<Song>("Sounds/gameMusic");

            //para empezara  reproducir la musica
            MediaPlayer.Play(gameMusic);

            //parar de reproducir la musica
            //MedialPlayer.Stop();
        
            //cargo la explosion
            explosionTexture = Content.Load<Texture2D>("Graphics/explosion.png");

            //Cargo los fondos y seteo la velocidad
            bgLayer1.Initialize(Content, "Graphics/bgLayer1.png", 
                GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, - 3);
            bgLayer1.Initialize(Content, "Graphics/bgLayer2.png",
                GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, - 4);

            mainBackground = Content.Load<Texture2D>("Graphics/mainbackground.png");

            fuente = Content.Load<SpriteFont>("fuente");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            laserSoundInstance.Dispose();

            explosionSoundInstance.Dispose();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            //GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || -- PROBAR SI SE PUEDE SACAR

            // TODO: Add your update logic here
            //Guardar el estado anterior de la tecla para determinar que se apreto antes
            previousKeyboardState = currentKeyboardState;

            //leer el estado del teclado y almacenarlas
            currentKeyboardState = Keyboard.GetState();

            //Actualizar el jugador
            UpdatePlayer(gameTime);

            //Actualizar las colisiones
            UpdateCollision();

            //actualizar los disparos
            for (var i = 0; i < laserBeams.Count; i++)
            {
                laserBeams[i].Update(gameTime);
                //para eliminarlos cuando pasan  la pantalla o chocan al enemigo
                if (!laserBeams[i].Active || laserBeams[i].Position.X > GraphicsDevice.Viewport.Width)
                {
                    laserBeams.Remove(laserBeams[i]);
                }
            }

            //actualizar las explosiones
            UpdateExplosions(gameTime);

            updatePlayerExplosion(gameTime);

            //actualiza el fondo
            bgLayer1.Update(gameTime);
            bgLayer2.Update(gameTime);

            //actualizo los enemigos
            UpdateEnemies(gameTime);

            base.Update(gameTime);
        }

        //metodo para controlar los movimientos del jugador
        private void UpdatePlayer(GameTime gameTime)
        {
            if(this.player.Active)
            {
                player.update(gameTime);

                //disparo del laser
                if (currentKeyboardState.IsKeyDown(Keys.Space))
                {
                    FireLaser(gameTime);
                }

                //Movimiento hacia la izquierda del jugador
                if (currentKeyboardState.IsKeyDown(Keys.Left))
                {
                    player.Position.X -= playerMovespeed  / 2;
                }
                //Movimiento hacia la derecha del jugador
                if (currentKeyboardState.IsKeyDown(Keys.Right))
                {
                    player.Position.X += playerMovespeed;
                }
                //Movimiento hacia abajo del jugador
                if (currentKeyboardState.IsKeyDown(Keys.Down))
                {
                    player.Position.Y += playerMovespeed;
                }
                //Movimiento hacia arriba del jugador
                if (currentKeyboardState.IsKeyDown(Keys.Up))
                {
                    player.Position.Y -= playerMovespeed;
                }

                //para asegurarnos de que el jugador no se salga de la pantalla
                player.Position.X = MathHelper.Clamp(player.Position.X, player.Width * player.PlayerAnimation.scale / 2, 
                    GraphicsDevice.Viewport.Width - player.Width * player.PlayerAnimation.scale / 2);
                player.Position.Y = MathHelper.Clamp(player.Position.Y, player.Height * player.PlayerAnimation.scale / 2, 
                    GraphicsDevice.Viewport.Height - player.Height * player.PlayerAnimation.scale / 2);
            }
        }

        protected void FireLaser(GameTime gameTime)
        {
            //manejar el alcance de disparo de nuestro laser
            if (gameTime.TotalGameTime - previousLaserSpawnTime > laserSpawnTime)
            {
                previousLaserSpawnTime = gameTime.TotalGameTime;
                //agregar laser a la lista
                AddLaser();
                //reproducir el sonido del laser
                laserSoundInstance.Play();
            }
        }

        protected void AddLaser()
        {
            Animation laserAnimation = new Animation();
            //inicializo la animacion del laser
            laserAnimation.Initialize(laserTexture, player.Position, 46, 16, 1, 30, Color.White, 1f, true);

            Laser laser = new Laser();

            //tomo la posicion inicial del laser
            var laserPostion = player.Position;

            //ajuste de posicion para coincidir con la boca del cañon de la nave
            laserPostion.Y += 37;
            laserPostion.X += 70;

            //inicia la animacion del laser
            laser.Initialize(laserAnimation, laserPostion);
            laserBeams.Add(laser);
            /* todo: add code to create a laser. */
             laserSoundInstance.Play();
        }

        private void UpdateCollision()
        {
            //usamos dos rectanguso para determinar si dos objetos se pisan
            Rectangle rectangleP;
            Rectangle rectangleE;

            //rectangulo del laser
            Rectangle laserRectangle;

            //solo creamos el rectangulo una vez para el jugador
            rectangleP = new Rectangle((int)player.Position.X, (int)player.Position.Y, player.Width, player.Height);

            //hacer la colision entre el jugador y los enemigos
            foreach (var enemy in enemies)
            {
                rectangleE = new Rectangle((int)enemy.Position.X, (int)enemy.Position.Y,
                                           enemy.Width, enemy.Height);

                //determinar si los objetos colisionan entre  si
                if (rectangleP.Intersects(rectangleE))
                {
                    //le bajo la energia al player
                    player.updateHealth(enemy.Damage);
                    //si el enemigo chocha al jugador, explzita el enemigo
                    enemy.isDead = true;
                    //agrego la explosion del enemigo
                    AddExplosion(enemy.Position);

                    //sonido explosion
                    var explosion = explosionSound.CreateInstance();
                    explosionSound.Play();

                }

                //Si la energia del jugador es menor o igual a cero, pierde el jugador
                if (player.Health <= 0)
                {
                    //agrego la explosion del enemigo
                    //AddExplosion(player.Position);
                    addPlayerExplosion();
                }

                enemies.ForEach(e =>
                {
                    //crear un rectangulo para el enemigo
                    rectangleE = new Rectangle((int)e.Position.X, (int)e.Position.Y, e.Width, e.Height);
                    laserBeams.ForEach(lb =>
                    {
                        laserRectangle = new Rectangle((int)lb.Position.X, (int)lb.Position.Y, lb.Width, lb.Height);
                        if (laserRectangle.Intersects(rectangleE))
                        {
                            //sonido de la explosion
                            var explosion = explosionSound.CreateInstance();
                            explosionSound.Play();
                            //mostrar la explosion donde esta el enemigo
                            AddExplosion(e.Position);
                            //mata al enemigo
                            e.isDead = true;
                            //myGame.Stage.EnemiesKilled++;
                            //elimina el disparo
                            lb.Active = false;
                            //sumar puntaje
                            //myGame.Score += e.Value;
                        }
                    });
                });
            }
        }
    /// <summary>
    /// This is called when the game should draw itself.
    /// </summary>
    /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            //Empezar a dibujar
            spriteBatch.Begin();
            
            rectBackground = GraphicsDevice.Viewport.TitleSafeArea;
            //dibuja el fondo principal
            spriteBatch.Draw(mainBackground, rectBackground, Color.White);

            //dibuja los fondos en movimiento
            bgLayer1.Draw(spriteBatch);
            bgLayer2.Draw(spriteBatch);

            //dibujar  los enemigos
            foreach (var e in enemies)
            {
                e.Draw(spriteBatch);
            }

            //dibujar a player
            player.Draw(spriteBatch);

            //dibujar laser
            foreach (var l in laserBeams)
            {
                l.Draw(spriteBatch);
            }

            //dibujar las explisiones
            foreach (var exp in explosions)
            {
                exp.Draw(spriteBatch);
            }

            if(this.playerExplosion != null)
            {
                if(this.playerExplosion.Active)
                {
                    this.playerExplosion.Draw(spriteBatch);
                } else
                {
                    spriteBatch.DrawString(this.fuente, "GAME OVER", new Vector2(230, 200), Color.Black);
                }
            }

            //parar de dibujar
            spriteBatch.End();

            base.Draw(gameTime);
        }
        
        //metodo para agregar un nuevo enemigo al juego
        private void AddEnemy()
        {
            //crea la animacion del objeto
            Animation enemyAnimation = new Animation();
            //inicializa los valores de la animacion
            enemyAnimation.Initialize(enemyTexture, Vector2.Zero, 47, 61, 8, 30, Color.White, 1f, true);
            //generador random de la ubicacion de los enemigos
            Vector2 position = new Vector2(GraphicsDevice.Viewport.Width +enemyTexture.Width / 2,
                                           random.Next(100, GraphicsDevice.Viewport.Height -100));
            //crea un enemigo
            Enemy enemy = new Enemy();
            //iniciaiza el enemigo
            enemy.Initialize(enemyAnimation, position);
            //agrega el enemigo a la lista de enemigos activos
            enemies.Add(enemy);

        }

        //metodo para actualizar  a los enemigos
        private void UpdateEnemies(GameTime gameTime)
        {
            if (gameTime.TotalGameTime - previousSpawnTime > enemySpawnTime)
            {
                previousSpawnTime = gameTime.TotalGameTime;
                //agrego enemigo
                AddEnemy();

            }
                //actualizo los enemigos
                for (int i = enemies.Count - 1; i >= 0; i--)
                {
                    enemies[i].Update(gameTime);
                    if (enemies[i].Active == false)
                    {
                        enemies.RemoveAt(i);
                    }
                }
        }

        //metodo para iniciar e insertar nuevas instancias de las explosiones
        protected void AddExplosion(Vector2 position)
        {
            Animation explosionAnimation = new Animation();
            explosionAnimation.Initialize(explosionTexture, position, 134, 134, 12, 30, Color.White, 1.0f, true);

            Explosion explosion = new Explosion();
            explosion.Initialize(explosionAnimation, position);
            explosions.Add(explosion);
        }

        //metodo para iniciar e insertar nuevas instancias de las explosiones
        protected void addPlayerExplosion()
        {
            if (this.playerExplosion == null)
            {
                Animation explosionAnimation = new Animation();
                explosionAnimation.Initialize(explosionTexture, this.player.Position, 134, 134, 12, 30, Color.White, 1.0f, true);

                Explosion explosion = new Explosion();
                explosion.Initialize(explosionAnimation, this.player.Position);
                this.playerExplosion = explosion;
            }
            
        }

        //chequear si las explisiones siguen activas para saber si paso la animacion completa, entonces la removemos
        private void UpdateExplosions(GameTime gameTime)
        {
            for (var e = 0; e < explosions.Count; e++)
            {
                explosions[e].Update(gameTime);
                if (!explosions[e].Active)
                {
                    explosions.Remove(explosions[e]);
                }
            }
        }

        //chequear si las explisiones siguen activas para saber si paso la animacion completa, entonces la removemos
        private void updatePlayerExplosion(GameTime gameTime)
        {
            if(this.playerExplosion != null)
            {
                this.playerExplosion.Update(gameTime);
            }
        }

    }
}
