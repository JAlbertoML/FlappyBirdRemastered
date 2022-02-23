using System;
using System.Collections;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System.IO;
using Microsoft.Xna.Framework.Audio;

namespace FlappyBirdRemastered.Classes
{
    public class GameController
    {
        public const int INIT_STATE = 0;
        public const int PLAYING_STATE = 1;
        public const int LOSE_STATE = 2;
        public const int GETTING_GIFT_STATE = 3;
        public const int TRANSITION_STATE = 4;
        public const int SELECT_BOOSTER_STATE = 5;
        public const int PAUSE_STATE = 6;
        public const int NO_POINT_STATE = 7;
        public const int DESTROYING_STATE = 8;
        public const int LEAVE_DESTROYER_STATE = 9;

        public static GraphicsDeviceManager graphics;
        private int gameState;
        private int indexFrame = 0;
        private int horizontalDistanceCounter = 0;
        private KeyboardState keyboardState;
        public static SoundEffect hitSound;
        public static SoundEffect dieSound;
        public static SoundEffect pointSound;
        public static SoundEffect bulletSound;
        public static SoundEffect pauseSound;
        public static SoundEffect fireballSound;
        public static SoundEffect hitbulletSound;
        public static SoundEffect hoohooSound;
        public static SoundEffect boosterSound;
        public static SoundEffect breakSound;
        private ArrayList arrayPipes;
        private ArrayList arrayBoosters;
        private MouseState mouseState;
        private int upDistance;
        private int score;
        private int bestScore;
        private bool point;
        private int enemyDelay;
        private ArrayList arrayEnemies;
        private int pipesCrossed;
        private string path_tmp;
        private string path;
        private int numberOfBigBirdBoosters;
        private int pipesDestroyed;

        public GameController()
        {
            path_tmp = @"%USERPROFILE%\\flappyBirdScore.txt";
            path = Environment.ExpandEnvironmentVariables(path_tmp);
            arrayBoosters = new ArrayList();
            arrayEnemies = new ArrayList();
            gameState = 0;
            pipesCrossed = 0;
            score = 0;
            bestScore = ReadScore();
            point = true;
            upDistance = 0;
            pipesDestroyed = 0;
            numberOfBigBirdBoosters = 0;
            mouseState = new MouseState();
            arrayPipes = new ArrayList();
        }

        public int GetWingsBirdFrame(GameTime gameTime, Bird bird)
        {
            if ((int)gameTime.TotalGameTime.TotalMilliseconds % 20 == 0)
            {
                indexFrame++;
                if (indexFrame == bird.Texture2D.Length)
                {
                    indexFrame = 0;
                }
            }
            return indexFrame;
        }

        public void MovePipes()
        {
            foreach (Pipe pipe in arrayPipes.ToArray())
            {
                //Moverlos
                pipe.Move(gameState);

                // Quitarlos
                if (pipe.TopPipeRectangle.Right < 0)
                {
                    arrayPipes.Remove(pipe);
                }
            }
        }

        public void AddPipes()
        {
            horizontalDistanceCounter++;
            if (horizontalDistanceCounter >= Pipe.horizontalDistanceBetween)
            {
                arrayPipes.Add(new Pipe(gameState));
                horizontalDistanceCounter = 0;
            }
        }

        public void RaiseBirdOnClick(Bird bird)
        {
            if (mouseState.LeftButton == ButtonState.Released && Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                bird.IsUp = true;
                Bird.wingSound.Play();
            }
            mouseState = Mouse.GetState();
            if (upDistance < 10 && bird.IsUp)
            {
                upDistance++;
                bird.GoUp();
            }
            else
            {
                upDistance = 0;
                bird.IsUp = false;
                bird.GoDown();
            }
        }

        public void GetDaownBirdAfterLose(Bird bird)
        {
            if (!bird.IsOnFloor())
            {
                bird.GoDown();
            }
        }

        public void LoseForImpactPipe(Bird bird)
        {
            foreach (Pipe pipe in arrayPipes.ToArray())
            {
                if (bird.Rectangle.Intersects(pipe.TopPipeRectangle) || bird.Rectangle.Intersects(pipe.BottomPipeRectangle))
                {
                    hitSound.Play();
                    dieSound.Play();
                    gameState = LOSE_STATE;
                }
            }
        }

        public void DestoyPipe(Bird bird)
        {
            foreach (Pipe pipe in arrayPipes.ToArray())
            {
                if (pipe.Destroyed)
                {
                    arrayPipes.Remove(pipe);
                    score += 3;
                    breakSound.Play();
                    pipesDestroyed++;
                } 
                else if (bird.Rectangle.Right >= pipe.TopPipeRectangle.X)
                {
                    pipe.Destroy();
                }
            }
        }

        public void LoseForImpactFloor(Bird bird)
        {
            if (bird.IsOnFloor())
            {
                hitSound.Play();
                gameState = LOSE_STATE;
            }
        }

        public void IncreaseScore(Bird bird)
        {
            foreach (Pipe pipe in arrayPipes.ToArray())
            {
                if (pipe.State == Pipe.FRONT_STATE)
                {
                    if (point && bird.Rectangle.X >= pipe.TopPipeRectangle.X && bird.Rectangle.Y > pipe.TopPipeRectangle.Y && bird.Rectangle.Y < pipe.BottomPipeRectangle.Y)
                    {
                        pointSound.Play();
                        score++;
                        point = false;
                    }

                    if (bird.Rectangle.X >= pipe.TopPipeRectangle.Right)
                    {
                        point = true;
                        pipe.State = Pipe.BACK_STATE;
                    }
                }
            }
        }

        public int GetNumberOfPipesCrossed(Bird bird)
        {
            foreach (Pipe pipe in arrayPipes.ToArray())
            {
                if (pipe.State == Pipe.FRONT_STATE)
                {
                    if (point && bird.Rectangle.X >= pipe.TopPipeRectangle.X && bird.Rectangle.Y > pipe.TopPipeRectangle.Y && bird.Rectangle.Y < pipe.BottomPipeRectangle.Y)
                    {
                        pipesCrossed++;
                        point = false;
                    }

                    if (bird.Rectangle.X >= pipe.TopPipeRectangle.Right)
                    {
                        point = true;
                        pipe.State = Pipe.BACK_STATE;
                    }
                }
            }
            return pipesCrossed;
        }

        public void MoveBoosters()
        {
            foreach (Booster booster in arrayBoosters.ToArray())
            {
                booster.Move();
                if(booster.Rectangle.Right <= 0)
                {
                    arrayBoosters.Remove(booster);
                }
            }
        }

        public void SetBestScore()
        {
            if(score > bestScore)
            {
                bestScore = score;
                SaveScore(bestScore);
            }
        }

        private int ReadScore()
        {
            // Open the file to read from.
            int sc = 0; 
            try
            {
                using (StreamReader sr = File.OpenText(path))
                {
                    string s = "";
                    while ((s = sr.ReadLine()) != null)
                    {
                        sc = Int32.Parse(s);
                    }
                }
            }
            catch(FileNotFoundException e)
            {
                Console.Write(e.StackTrace);
                sc = 0;
            }
            return sc;
        }

        private void SaveScore(int scoreToSave)
        {
            // Create a file to write to.
            using (StreamWriter sw = File.CreateText(path))
            {
                sw.WriteLine(scoreToSave);
            }
        }

        public void GenerateInitBoosters()
        {
            arrayBoosters.Add(new NoPointsBooster(NoPointsBooster.UP_POSITION));
            arrayBoosters.Add(new GiftBooster());
            arrayBoosters.Add(new NoPointsBooster(NoPointsBooster.DOWN_POSITION));
        }

        public void EnemyAppear()
        {
            if(score >= 103)
            {
                if (enemyDelay >= Enemy.Delay)
                {
                    arrayEnemies.Add(new Enemy());
                    bulletSound.Play();
                    enemyDelay = 0;
                    Enemy.DefineDelay();
                }
                enemyDelay++;
            }
        }

        public void MoveEnemies()
        {
            foreach (Enemy enemy in arrayEnemies.ToArray())
            {
                if(enemy.IsDestroyed)
                {
                    enemy.GoDown();
                }
                else
                {
                    enemy.Move();
                }
                //Eliminar al enemigo cuando salga de la pantalla
                if (enemy.Rectangle.Right <= 0 || enemy.Rectangle.Y >= graphics.PreferredBackBufferHeight)
                {
                    arrayEnemies.Remove(enemy);
                }
            }
        }

        public void BirdShoot(Bird bird)
        {
                keyboardState = Keyboard.GetState();
                if (keyboardState.IsKeyDown(Keys.Space) && bird.DelayShoot >= 10)
                {
                    bird.Shoot();
                    fireballSound.Play();
                    bird.DelayShoot = 0;
                }
                bird.DelayShoot++;

        }

        public void MoveFireBalls(ArrayList arrayFireBalls)
        {
            foreach (FireBall fireBall in arrayFireBalls.ToArray())
            {
                fireBall.Move();
                if (fireBall.Rectangle.X >= graphics.PreferredBackBufferWidth)
                    arrayFireBalls.Remove(fireBall);
            }
        }

        public void VerifyFireBallEnemyImpact(ArrayList arrayFireBall)
        {
            foreach (Enemy enemy in arrayEnemies.ToArray())
            {
                foreach (FireBall fireBall in arrayFireBall.ToArray())
                {
                    if (fireBall.Rectangle.Intersects(enemy.Rectangle))
                    {
                        if(enemy.NumberImpacts >= 2)
                        {
                            arrayFireBall.Remove(fireBall);
                            score += 3;
                            hoohooSound.Play();
                            enemy.IsDestroyed = true;
                        }
                        else
                        {
                            arrayFireBall.Remove(fireBall);
                            enemy.NumberImpacts++;
                            hitbulletSound.Play();
                        }
                    }
                }
            }
        }

        public void LoseForImpactEnemy(Bird bird)
        {
            foreach (Enemy enemy in arrayEnemies)
            {
                if (bird.Rectangle.Intersects(enemy.Rectangle))
                {
                    gameState = LOSE_STATE;
                    dieSound.Play();
                }
            }
        }

        public bool IsTimeToBigBird()
        {
            if(score >= 50 && score % 50 == 0 && numberOfBigBirdBoosters < 1)
            {
                numberOfBigBirdBoosters = 1;
                return true;
            }
            else
            {
                return false;
            }
        }

        public int GameState
        {
            get
            {
                return gameState;
            }
            set
            {
                this.gameState = value;
            }
        }

        public int Score
        {
            get
            {
                return score;
            }
            set
            {
                this.score = value;
            }
        }

        public int BestScore
        {
            get
            {
                return bestScore;
            }
            set
            {
                this.bestScore = value;
            }
        }

        public ArrayList ArrayPipes
        {
            get
            {
                return arrayPipes;
            }
            set
            {
                this.arrayPipes = value;
            }
        }

        public ArrayList ArrayBoosters
        {
            get
            {
                return arrayBoosters;
            }
            set
            {
                this.arrayBoosters = value;
            }
        }

        public ArrayList ArrayEnemies
        {
            get
            {
                return arrayEnemies;
            }
            set
            {
                this.arrayEnemies = value;
            }
        }

        public int PipesCrossed
        {
            get
            {
                return pipesCrossed;
            }
            set
            {
                this.pipesCrossed = value;
            }
        }

        public int PipesDestroyed
        {
            get
            {
                return pipesDestroyed;
            }
            set
            {
                this.pipesDestroyed = value;
            }
        }

        public int NumberOfBigBirdBoosters
        {
            get
            {
                return numberOfBigBirdBoosters;
            }
            set
            {
                this.numberOfBigBirdBoosters = value;
            }
        }
    }
}
