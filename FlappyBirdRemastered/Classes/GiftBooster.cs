using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace FlappyBirdRemastered.Desktop.Classes
{
    public class GiftBooster : Booster
    {
        public static Texture2D texture2D;

        public GiftBooster()
        {
            type = GIFT_BOOSTER;
            rectangle = new Rectangle(graphics.PreferredBackBufferWidth, (4 * (graphics.PreferredBackBufferHeight / 10)) - (height / 2), width, height);
        }
    }
}
