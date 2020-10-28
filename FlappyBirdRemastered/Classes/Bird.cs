using System;
using System.Collections;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
namespace FlappyBirdRemastered.Desktop.Classes
{
    public class Bird
    {
        private Texture2D[] texture2D;
        private Rectangle rectangle;
        public static SoundEffect wingSound;
        private readonly int initX;
        private int delayShoot;
        private readonly int initY;
        private readonly int initWidth;
        private readonly int initHeight;
        private bool isUp;
        private ArrayList arrayFireBalls;
        public static GraphicsDeviceManager graphics;

        public Bird()
        {
            texture2D = new Texture2D[3];
            arrayFireBalls = new ArrayList();
            initWidth = 60;
            initHeight = 45;
            delayShoot = 0;
            isUp = false;
            initX = (graphics.PreferredBackBufferWidth / 4) - (initWidth / 2);
            initY = (4 * (graphics.PreferredBackBufferHeight / 10)) - (initHeight / 2);
            rectangle = new Rectangle(initX, initY, initWidth, initHeight);
        }

        public void SetNormalSize()
        {
            rectangle.Width = initWidth;
            rectangle.Height = initHeight;
        }

        public void SetBigSize()
        {
            rectangle.Y = 50;
            rectangle.Width = 10 * initWidth;
            rectangle.Height = 10 * initHeight;
        }

        public void SetInitPosition()
        {
            rectangle.X = initX;
            rectangle.Y = initY;
        }

        public bool IsOnFloor()
        {
            bool isOnFloor = false || rectangle.Y > 485;
            return isOnFloor;
        }

        public void GoUp()
        {
            rectangle.Y-=10;
        }

        public void GoDown()
        {
            rectangle.Y+=5;
        }

        public void Shoot()
        {
            arrayFireBalls.Add(new FireBall(rectangle.Right, rectangle.Y + (2 * (rectangle.Width / 3)) - (FireBall.height / 2)));
        }

        public Texture2D[] Texture2D
        {
            get
            {
                return texture2D;
            }
            set
            {
                this.texture2D = value;
            }
        }

        public Rectangle Rectangle
        {
            get
            {
                return rectangle;
            }
            set
            {
                this.rectangle = value;
            }
        }

        public bool IsUp
        {
            get
            {
                return isUp;
            }
            set
            {
                this.isUp = value;
            }
        }

        public int DelayShoot
        {
            get
            {
                return delayShoot;
            }
            set
            {
                this.delayShoot = value;
            }
        }

        public ArrayList ArrayFireBalls
        {
            get
            {
                return arrayFireBalls;
            }
        }
    }
}
