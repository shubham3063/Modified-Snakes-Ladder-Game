using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Snakes_and_Ladders.Graphics;


namespace Snakes_and_Ladders.Input
{
    static class Mouse
    {
        #region (cursor , stateOfMouse)

        public static Image cursor;
        public static int a;
        public static MouseState mouseState;

        #endregion

        #region Main Methods (Constructor, Initialize, LoadGraphicsContent, Update, Draw)

        static Mouse()
        {
            cursor = new Image();
        }


        public static void LoadGraphicsContent(SpriteBatch spriteBatch, Texture2D texture)
        {
            cursor.LoadGraphicsContent(texture, spriteBatch);
        }

        public static void Initialize()
        {
            cursor.Initialize(new Vector2(100, 100), Color.White, Vector2.One, new Rectangle(0, 0, 30, 26),1);
        }

        public static void Draw(GameTime gametime)
        {
            cursor.Draw(gametime);

        }

        public static void Update(GameTime gametime)
        {

            mouseState = Microsoft.Xna.Framework.Input.Mouse.GetState();
            //Console.WriteLine("(" + mouseState.X + "," + mouseState.Y + ")");
            cursor.Position = new Vector2(mouseState.X, mouseState.Y);
        }
        #endregion
    }
}
