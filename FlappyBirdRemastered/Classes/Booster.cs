using System;
using Microsoft.Xna.Framework;

namespace FlappyBirdRemastered.Desktop.Classes
{
    public class Booster
    {
        public const int GIFT_BOOSTER = 0;
        public const int NO_POINTS_BOOSTER = 1;
        public const int BIG_BIRD_BOOSTER = 2;
        public static GraphicsDeviceManager graphics;

        protected Rectangle rectangle;
        protected int width;
        protected int height;
        protected int type;

        public Booster()
        {
            width = 75;
            height = 75;
        }

        public void Move()
        {
            rectangle.X -= 10;
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

        public int Width
        {
            get
            {
                return width;
            }
            set
            {
                this.width = value;
            }
        }

        public int Height
        {
            get
            {
                return height;
            }
            set
            {
                this.height = value;
            }
        }

        public int Type
        {
            get
            {
                return type;
            }
            set
            {
                this.type = value;
            }
        }
    }
}
