using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;


namespace Shooter
{
    class Animation
    {
        Texture2D spriteStrip;
        float scale;
        int frameCount;
        public int FrameWidth;
        public int FrameHeight;
        int currentFrame;
        float elapsedTime;
        float frameTime;
        Rectangle sourceRectangle = new Rectangle();
        Rectangle destRectangle = new Rectangle();
        Color frameColor;
        public bool Active;
        public bool Looping;
        public Vector2 Position;

        public void Initialize(Texture2D texture, Vector2 position, int frameHeight, int frameWidth, int frameCount, float frameTime, Color color, float scale,bool looping)
        {
            this.spriteStrip = texture;
            this.Position = position;
            this.FrameHeight = frameHeight;
            this.FrameWidth = frameWidth;
            this.frameCount = frameCount;
            this.frameTime = frameTime;
            this.frameColor = color;
            this.scale = scale;
            this.Looping = looping;
            this.elapsedTime = 0;
            this.currentFrame = 1;
            this.Active = true;


        }

        public void Update(GameTime gametime)
        {
            if (!Active)
            {
                return;
            }
            elapsedTime += (float)gametime.ElapsedGameTime.TotalMilliseconds;
            if (elapsedTime>=frameTime)
            {
                if (currentFrame==frameCount)
                {
                    currentFrame = 1;
                    if (Looping==false)
                    {
                        Active = false;
                    }
                }

                sourceRectangle = new Rectangle(currentFrame * FrameWidth, 0, FrameWidth, FrameHeight);
                destRectangle = new Rectangle((int)Position.X, (int)Position.Y, FrameWidth, FrameHeight);
                currentFrame++;
                elapsedTime = 0;
            }
            
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(spriteStrip, destRectangle, sourceRectangle, frameColor,0.0f,Vector2.Zero,SpriteEffects.None,0.1f);
        }

    }
}
