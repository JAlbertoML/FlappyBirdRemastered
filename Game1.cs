using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using FlappyBirdRemastered.Classes;

namespace FlappyBirdRemastered
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private SpriteFont scoreFont;
        private SpriteFont textFont;
        private GameController gameController;
        private MouseState mouseState;
        private Items items;
        private int gameStateTmp;
        private Scene scene;
        private Bird bird;
        private int indexFrame;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            graphics.PreferredBackBufferWidth = 1080;
            graphics.PreferredBackBufferHeight = 650;
            graphics.ApplyChanges();
            GameController.graphics = graphics;
            Booster.graphics = graphics;
            Scene.graphics = graphics;
            Items.graphics = graphics;
            Enemy.graphics = graphics;
            Bird.graphics = graphics;
            Pipe.graphics = graphics;
            gameController = new GameController();
            scene = new Scene();
            items = new Items();
            bird = new Bird();
            indexFrame = 0;
            mouseState = Mouse.GetState();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            scene.BackgroundTexture = Content.Load<Texture2D>("SceneImages/background");
            scene.FloorTexture = Content.Load<Texture2D>("SceneImages/floor");
            bird.Texture2D[0] = Content.Load<Texture2D>("Character/bird1");
            bird.Texture2D[1] = Content.Load<Texture2D>("Character/bird2");
            bird.Texture2D[2] = Content.Load<Texture2D>("Character/bird3");
            Pipe.topPipeTexture = Content.Load<Texture2D>("Obstacles/toppipe");
            Pipe.bottomPipeTexture = Content.Load<Texture2D>("Obstacles/bottompipe");
            Enemy.texture2D = Content.Load<Texture2D>("Obstacles/enemy");
            GiftBooster.texture2D = Content.Load<Texture2D>("Boosters/gift");
            NoPointsBooster.texture2D = Content.Load<Texture2D>("Boosters/nopoints");
            FireBall.texture2D = Content.Load<Texture2D>("Boosters/fireball");
            Items.clickTexture = Content.Load<Texture2D>("Items/click");
            Items.instructionsTexture = Content.Load<Texture2D>("Items/instructions");
            Items.pauseTexture = Content.Load<Texture2D>("Items/pause");
            Items.loseTexture = Content.Load<Texture2D>("Items/lose");
            BigBirdBooster.texture2D = Content.Load<Texture2D>("Boosters/bigbird");

            // Load sounds
            Bird.wingSound = Content.Load<SoundEffect>("SoundEffects/wing");
            GameController.hitSound = Content.Load<SoundEffect>("SoundEffects/hit");
            GameController.dieSound = Content.Load<SoundEffect>("SoundEffects/die");
            GameController.pointSound = Content.Load<SoundEffect>("SoundEffects/point");
            GameController.bulletSound = Content.Load<SoundEffect>("SoundEffects/bullet");
            GameController.pauseSound = Content.Load<SoundEffect>("SoundEffects/pause");
            GameController.fireballSound = Content.Load<SoundEffect>("SoundEffects/fireball");
            GameController.hitbulletSound = Content.Load<SoundEffect>("SoundEffects/hitbullet");
            GameController.hoohooSound = Content.Load<SoundEffect>("SoundEffects/hoohoo");
            GameController.boosterSound = Content.Load<SoundEffect>("SoundEffects/booster");
            GameController.breakSound = Content.Load<SoundEffect>("SoundEffects/break");

            // Load Fonts
            scoreFont = Content.Load<SpriteFont>("Fonts/score");
            textFont = Content.Load<SpriteFont>("Fonts/text");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            switch (gameController.GameState)
            {
                case GameController.INIT_STATE:
                    gameController.ArrayBoosters.Clear();
                    // Inicializar 
                    gameController.ArrayPipes.Clear();
                    bird.SetInitPosition();
                    gameController.Score = 0;
                    gameController.ArrayBoosters.Clear();
                    gameController.ArrayEnemies.Clear();
                    bird.ArrayFireBalls.Clear();

                    scene.Move(gameController.GameState);

                    // Mover alas
                    indexFrame = gameController.GetWingsBirdFrame(gameTime, bird);

                    // Elevar pajaro en clic
                    if (mouseState.LeftButton == ButtonState.Released && Mouse.GetState().LeftButton == ButtonState.Pressed)
                    {
                        gameController.GenerateInitBoosters();
                        gameController.GameState = GameController.SELECT_BOOSTER_STATE;
                    }
                    mouseState = Mouse.GetState();
                    break;

                case GameController.PLAYING_STATE:

                    bird.SetNormalSize();

                    scene.Move(gameController.GameState);

                    gameController.AddPipes();

                    gameController.MovePipes();

                    gameController.EnemyAppear();

                    gameController.MoveEnemies();

                    gameController.BirdShoot(bird);

                    gameController.VerifyFireBallEnemyImpact(bird.ArrayFireBalls);

                    gameController.MoveFireBalls(bird.ArrayFireBalls);


                    // Mover alas
                    indexFrame = gameController.GetWingsBirdFrame(gameTime, bird);

                    gameController.RaiseBirdOnClick(bird);

                    gameController.LoseForImpactPipe(bird);

                    gameController.LoseForImpactFloor(bird);

                    gameController.LoseForImpactEnemy(bird);

                    gameController.IncreaseScore(bird);

                    if (gameController.IsTimeToBigBird())
                    {
                        gameController.ArrayBoosters.Add(new BigBirdBooster());
                    }

                    foreach (Booster booster in gameController.ArrayBoosters.ToArray())
                    {
                        booster.Move();
                        if(booster.Rectangle.Right <= 0)
                        {
                            gameController.ArrayBoosters.Remove(booster);
                            gameController.NumberOfBigBirdBoosters = 0;
                        }
                    }

                    foreach (Booster booster in gameController.ArrayBoosters.ToArray())
                    {
                        if (bird.Rectangle.Intersects(booster.Rectangle))
                        {
                            gameController.ArrayBoosters.Remove(booster);
                            GameController.boosterSound.Play();
                            gameController.GameState = GameController.DESTROYING_STATE;
                        }
                    }

                    // Pause game
                    if (mouseState.RightButton == ButtonState.Released && Mouse.GetState().RightButton == ButtonState.Pressed)
                    {
                        gameStateTmp = gameController.GameState;
                        gameController.GameState = GameController.PAUSE_STATE;
                        GameController.pauseSound.Play();
                    }
                    mouseState = Mouse.GetState();

                    break;

                case GameController.LOSE_STATE:
                    gameController.MoveEnemies();
                    gameController.GetDaownBirdAfterLose(bird);
                    // Ir al init state despues de perder
                    if (mouseState.LeftButton == ButtonState.Released && Mouse.GetState().LeftButton == ButtonState.Pressed)
                    {
                        gameController.GameState = GameController.INIT_STATE;
                    }
                    mouseState = Mouse.GetState();
                    gameController.SetBestScore();
                    break;

                case GameController.GETTING_GIFT_STATE:
                    gameController.MoveBoosters();

                    bird.SetInitPosition();

                    scene.Move(gameController.GameState);

                    gameController.AddPipes();

                    gameController.MovePipes();

                    // Mover alas
                    indexFrame = gameController.GetWingsBirdFrame(gameTime, bird);

                    gameController.IncreaseScore(bird);

                    // Ir al transition state despues de pasar por los tubos
                    if (gameController.Score == 8)
                    {
                        gameController.GameState = GameController.TRANSITION_STATE;
                    }

                    // Pause game
                    if (mouseState.RightButton == ButtonState.Released && Mouse.GetState().RightButton == ButtonState.Pressed)
                    {
                        gameStateTmp = gameController.GameState;
                        gameController.GameState = GameController.PAUSE_STATE;
                        GameController.pauseSound.Play();
                    }
                    mouseState = Mouse.GetState();

                    break;

                case GameController.TRANSITION_STATE:

                    scene.Move(gameController.GameState);

                    gameController.MovePipes();

                    // Mover alas
                    indexFrame = gameController.GetWingsBirdFrame(gameTime, bird);

                    gameController.IncreaseScore(bird);

                    if (gameController.ArrayPipes.ToArray().Length == 0)
                    {
                        gameController.GameState = GameController.PLAYING_STATE;
                    }

                    // Pause game
                    if (mouseState.RightButton == ButtonState.Released && Mouse.GetState().RightButton == ButtonState.Pressed)
                    {
                        gameStateTmp = gameController.GameState;
                        gameController.GameState = GameController.PAUSE_STATE;
                        GameController.pauseSound.Play();
                    }
                    mouseState = Mouse.GetState();

                    break;

                case GameController.SELECT_BOOSTER_STATE:
                    scene.Move(gameController.GameState);
                    // Mover alas
                    indexFrame = gameController.GetWingsBirdFrame(gameTime, bird);
                    gameController.RaiseBirdOnClick(bird);
                    gameController.MoveBoosters();
                    gameController.LoseForImpactFloor(bird);

                    // Select booster
                    foreach (Booster booster in gameController.ArrayBoosters.ToArray())
                    {
                        if (bird.Rectangle.Intersects(booster.Rectangle))
                        {
                            gameController.ArrayBoosters.Remove(booster);
                            GameController.boosterSound.Play();
                            if (booster.Type == Booster.GIFT_BOOSTER)
                            {
                                gameController.GameState = GameController.GETTING_GIFT_STATE;
                            }
                            else if (booster.Type == Booster.NO_POINTS_BOOSTER)
                            {
                                gameController.GameState = GameController.NO_POINT_STATE;
                            }
                            break;
                        }
                    }

                    // Iniciar juego si no se eligió booster
                    if (gameController.ArrayBoosters.ToArray().Length == 0)
                    {
                        gameController.GameState = GameController.PLAYING_STATE;
                    }

                    // Pause game
                    if (mouseState.RightButton == ButtonState.Released && Mouse.GetState().RightButton == ButtonState.Pressed)
                    {
                        gameStateTmp = gameController.GameState;
                        gameController.GameState = GameController.PAUSE_STATE;
                        GameController.pauseSound.Play();
                    }
                    mouseState = Mouse.GetState();

                    break;

                case GameController.PAUSE_STATE:
                    // Unpause game
                    if (mouseState.LeftButton == ButtonState.Released && Mouse.GetState().LeftButton == ButtonState.Pressed)
                    {
                        gameController.GameState = gameStateTmp;
                        GameController.pauseSound.Play();
                    }
                    mouseState = Mouse.GetState();
                    break;

                case GameController.NO_POINT_STATE:
                    gameController.MoveBoosters();

                    scene.Move(gameController.GameState);

                    gameController.AddPipes();

                    gameController.MovePipes();

                    gameController.EnemyAppear();

                    gameController.MoveEnemies();

                    gameController.BirdShoot(bird);

                    gameController.VerifyFireBallEnemyImpact(bird.ArrayFireBalls);

                    gameController.MoveFireBalls(bird.ArrayFireBalls);


                    // Mover alas
                    indexFrame = gameController.GetWingsBirdFrame(gameTime, bird);

                    gameController.RaiseBirdOnClick(bird);

                    gameController.LoseForImpactPipe(bird);

                    gameController.LoseForImpactFloor(bird);

                    gameController.LoseForImpactEnemy(bird);

                    if (gameController.GetNumberOfPipesCrossed(bird) >= NoPointsBooster.penalty)
                    {
                        gameController.PipesCrossed = 0;
                        gameController.GameState = GameController.PLAYING_STATE;
                        NoPointsBooster.DefinePenalty();
                    }

                    // Pause game
                    if (mouseState.RightButton == ButtonState.Released && Mouse.GetState().RightButton == ButtonState.Pressed)
                    {
                        gameStateTmp = gameController.GameState;
                        gameController.GameState = GameController.PAUSE_STATE;
                        GameController.pauseSound.Play();
                    }
                    mouseState = Mouse.GetState();
                    break;

                case GameController.DESTROYING_STATE:
                    bird.SetInitPosition();

                    bird.SetBigSize();

                    scene.Move(gameController.GameState);

                    gameController.AddPipes();

                    gameController.MovePipes();

                    gameController.DestoyPipe(bird);

                    // Mover alas
                    indexFrame = gameController.GetWingsBirdFrame(gameTime, bird);

                    // Ir al transition state despues de pasar por los tubos
                    if (gameController.PipesDestroyed >= 10)
                    {
                        gameController.PipesDestroyed = 0;
                        gameController.GameState = GameController.LEAVE_DESTROYER_STATE;
                    }

                    // Pause game
                    if (mouseState.RightButton == ButtonState.Released && Mouse.GetState().RightButton == ButtonState.Pressed)
                    {
                        gameStateTmp = gameController.GameState;
                        gameController.GameState = GameController.PAUSE_STATE;
                        GameController.pauseSound.Play();
                    }
                    mouseState = Mouse.GetState();
                    break;

                case GameController.LEAVE_DESTROYER_STATE:

                    bird.SetInitPosition();

                    bird.SetBigSize();

                    scene.Move(gameController.GameState);

                    gameController.MovePipes();

                    gameController.DestoyPipe(bird);

                    // Mover alas
                    indexFrame = gameController.GetWingsBirdFrame(gameTime, bird);

                    // Ir al transition state despues de pasar por los tubos
                    if (gameController.ArrayPipes.ToArray().Length <= 0)
                    {
                        gameController.NumberOfBigBirdBoosters = 0;
                        gameController.GameState = GameController.PLAYING_STATE;
                    }

                    // Pause game
                    if (mouseState.RightButton == ButtonState.Released && Mouse.GetState().RightButton == ButtonState.Pressed)
                    {
                        gameStateTmp = gameController.GameState;
                        gameController.GameState = GameController.PAUSE_STATE;
                        GameController.pauseSound.Play();
                    }
                    mouseState = Mouse.GetState();

                    break;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();
            // Dibujar Fondo
            spriteBatch.Draw(scene.BackgroundTexture, scene.BackgroundRectangle, Color.White);
            spriteBatch.Draw(scene.BackgroundTexture, scene.BackgroundRectangle2, Color.White);

            // Dibujar tubos
            foreach (Pipe pipe in gameController.ArrayPipes)
            {
                spriteBatch.Draw(Pipe.topPipeTexture, pipe.TopPipeRectangle, Color.White);
                spriteBatch.Draw(Pipe.bottomPipeTexture, pipe.BottomPipeRectangle, Color.White);
            }

            // Dibujar suelo
            spriteBatch.Draw(scene.FloorTexture, scene.FloorRectangle, Color.White);
            spriteBatch.Draw(scene.FloorTexture, scene.FloorRectangle2, Color.White);

            // Dibujar boosters
            foreach (Booster booster in gameController.ArrayBoosters)
            {
                if (booster.Type == Booster.GIFT_BOOSTER)
                {
                    spriteBatch.Draw(GiftBooster.texture2D, booster.Rectangle, Color.White);
                }
                else if (booster.Type == Booster.NO_POINTS_BOOSTER)
                {
                    spriteBatch.Draw(NoPointsBooster.texture2D, booster.Rectangle, Color.White);
                }
                else if(booster.Type == Booster.BIG_BIRD_BOOSTER)
                {
                    spriteBatch.Draw(BigBirdBooster.texture2D, booster.Rectangle, Color.White);
                }
            }

            // Dibujar enemigos
            foreach (Enemy enemy in gameController.ArrayEnemies)
            {
                spriteBatch.Draw(Enemy.texture2D, enemy.Rectangle, Color.White);
            }

            // Dibujar pajaro
            spriteBatch.Draw(bird.Texture2D[indexFrame], bird.Rectangle, Color.White);

            // Dibujar items del inicio
            if(gameController.GameState == GameController.INIT_STATE)
            {
                spriteBatch.Draw(Items.clickTexture, items.ClickRectangle, Color.White);
                spriteBatch.Draw(Items.instructionsTexture, items.InstructionsRectangle, Color.White);
            }

            // Dibujar items pause
            if(gameController.GameState == GameController.PAUSE_STATE)
            {
                spriteBatch.Draw(Items.pauseTexture, items.PauseRectangle, Color.White);
            }

            // Texto si agarró booster de castigo
            if(gameController.GameState == GameController.NO_POINT_STATE)
            {
                spriteBatch.DrawString(textFont, "TUS PRIMEROS " + NoPointsBooster.penalty + " PUNTOS NO VALEN", new Vector2((graphics.PreferredBackBufferWidth / 2) - (textFont.MeasureString("TUS PRIMEROS PUNTOS NO VALEN").X / 2), 80), Color.White);
            }

            // Texto si agarró booster de regalo
            if (gameController.GameState == GameController.GETTING_GIFT_STATE)
            {
                spriteBatch.DrawString(textFont, "PUNTOS SIN ESFUERZO", new Vector2((graphics.PreferredBackBufferWidth / 2) - (textFont.MeasureString("PUNTOS SIN ESFUERZO").X / 2), 80), Color.White);
            }

            // Dibujar bolas de fuego
            foreach (FireBall fireBall in bird.ArrayFireBalls)
            {
                spriteBatch.Draw(FireBall.texture2D, fireBall.Rectangle, Color.White);
            }

            // Dibujar items lose
            if (gameController.GameState == GameController.LOSE_STATE)
            {
                spriteBatch.Draw(Items.loseTexture, items.LoseRectangle, Color.White);
                spriteBatch.DrawString(scoreFont, gameController.Score.ToString(), new Vector2((graphics.PreferredBackBufferWidth / 2) - (scoreFont.MeasureString(gameController.Score.ToString()).X / 2), 260), Color.GhostWhite);
                spriteBatch.DrawString(scoreFont, gameController.BestScore.ToString(), new Vector2((graphics.PreferredBackBufferWidth / 2) - (scoreFont.MeasureString(gameController.BestScore.ToString()).X / 2), 370), Color.GhostWhite);
            }

            // Dibujar score
            spriteBatch.DrawString(scoreFont, gameController.Score.ToString(), new Vector2((graphics.PreferredBackBufferWidth / 2) - (scoreFont.MeasureString(gameController.Score.ToString()).X / 2), 10), Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
