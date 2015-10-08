using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Snakes_and_Ladders;
//using Microsoft.Xna.Framework;



namespace Snakes_and_Ladders.Graphics
{
    class Image
    {
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        public SpriteEffects Effects
        {
            get { return effects; }
            set { effects = value; }
        }

        public Rectangle SourceRectangle
        {
            get { return sourceRectangle; }
            set { sourceRectangle = value; }
        }
        SpriteBatch spriteBatch;
        Texture2D texture;
        Vector2 position;
        Vector2 originalPosition;
        Rectangle sourceRectangle;
        Color color;
        float rotation;
        public Vector2 origin;
        public Vector2 scale;
        SpriteEffects effects;
        float layer;
        public int flag = 0;//if the image is fixed flag=1 or it can move flag=0
        private int rectangleCheck = 1;//if the complete rectangle is to be drawn rectangleChack =0
        public int currentNumber=1;
        public int currentRow = 0, lastRow = 0;
        public int active = 1;

        public void LoadGraphicsContent(Texture2D mytexture, SpriteBatch spriteBatch)
        {
            this.spriteBatch = spriteBatch;
            this.texture = mytexture;
        }

        public void Update(Texture2D mytexture, Vector2 scale)
        {
            this.texture = mytexture;
            this.scale = scale;
        }

        public void Initialize(Vector2 iniPosition, Color iniColor, Vector2 iniScale, Rectangle rectangle, int rectangleCheck)
        {
            this.position = iniPosition;
            this.originalPosition = iniPosition;
            this.rotation = 0.0f;
            this.scale = iniScale;
            this.sourceRectangle = rectangle;
            this.effects = SpriteEffects.None;
            this.color = iniColor;
            this.layer = 0.5f;
            this.origin.X =  (this.sourceRectangle.Width) / 2;
            this.origin.Y =  (this.sourceRectangle.Height) / 2;
            this.rectangleCheck = rectangleCheck;
        }

        public void Draw(GameTime gametime)
        {
            if (this.rectangleCheck==0)
            {
             this.spriteBatch.Draw(this.texture, this.position, null, this.color, this.rotation, Vector2.Zero, this.scale, this.effects, this.layer);
               
            }
            else
                this.spriteBatch.Draw(this.texture, this.position, this.sourceRectangle, this.color, this.rotation, Vector2.Zero, this.scale, this.effects, this.layer);

        }


    }
}
