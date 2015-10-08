using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;


namespace mygame
{
    class Engine : Microsoft.Xna.Framework.Game
    {
        #region Fields
        Microsoft.Xna.Framework.GraphicsDeviceManager graphics;
        Microsoft.Xna.Framework.Content.ContentManager content;

        //for texture 2D which is basically very simple and easy to draw
        Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch;
        Microsoft.Xna.Framework.Graphics.Texture2D myTexture;
        Microsoft.Xna.Framework.Graphics.Texture2D myTextureTable;
        Microsoft.Xna.Framework.Graphics.Texture2D arrowTexture;
        
        Microsoft.Xna.Framework.Vector2 spritePosition;
        Song mysong;
        int Max_Obj = Program.Max_Obj;

        public mygame.Graphics.Image[] box; //= new mygame.Graphics.Image();
        mygame.Graphics.Image arrow;
        int boxesKept = 0;
        Vector2 keepAt;

        float  t1=0,t2=0,t3=0;//time of the game .to check if any sprites needs to given effects
        float checkTime = 5;//time after which the sprite is given effect
        int arrowDisplayFlag = 0;
        int autoKeepFlag = 0;
        int j = 0;
        int bigBox = 0;
        float mover = 0;
    
        int fl = 0;//for checking movement of box
        //for 3D models,relatively difficult to draw. there are meshes in the model and each mesh has to be drawn separately.
        //for this we need a matrix which has all the meshes and then we can iterate over it to draw the model.
        //spriteBatch means collection of simple 2D images.and for a model everything needs to be specified ->
        //the cameraPosition, the position of the model, the aspect ratio(i will write more in this regard), 
        //the meshes are copied to boneTransforms matrix in the draw function

        //Instantiation
        //sphere ->LoadGraphicsContent()
        //boneTransforms->LoadGraphicsContent()
        //spherePosition->Initialize()
        //cameraPosition->Initialize()
        //aspectRation->Initialize()



        Microsoft.Xna.Framework.Graphics.Model sphere;
        Microsoft.Xna.Framework.Matrix[] boneTransforms;
        Microsoft.Xna.Framework.Vector3 spherePosition;
        float sphereRotation;
        Microsoft.Xna.Framework.Vector3 cameraPosition;
        float aspectRatio;
        //SpriteFont font;

        float x1 = 0f;
        float y1 = 0f;
        float x0 = 0f, y = 0f;
        int state = 0;
        float randomNo;
        float m, c;
        float translateSpeed = 0.5f;

        float xLimitPositive = 10f;
        float yLimitPositive = 10f;
        float xLimitNegative = -10f;
        float yLimitNegative = -10f;
        int decrease;
        #endregion
        #region Main Methods (constructor,LoadGraphicsConstent,UnloadGraphicsConent,Initialize,Draw,Update)

        public Engine()
        {
            this.graphics = new Microsoft.Xna.Framework.GraphicsDeviceManager(this);
            /*the first this is used because the "graphics" variable is outside the scope of method Engine() ,So we need to
            get the variable which will be present in the object of this class...So we use "this" keyword to  get it.
            The 2nd this is the parameter passed to the constructor of class GraphicsDeviceManager which takes as its argument
            the game to which the graphics device manager should be associated with. We know that it will be associated to the 
             object of this class...SO we use "this" again.
             */

            this.content = new Microsoft.Xna.Framework.Content.ContentManager(this.Services);
            //this.spriteBatch = new Microsoft.Xna.Framework.Graphics.SpriteBatch(this.graphics.GraphicsDevice);
            //this.box = new mygame.Graphics.Image[Max_Obj];
            //this.Initialize();


        }

        protected override void Initialize()
        {

           // LoadGraphicsContent(true);
           // MediaPlayer.Play(mysong);
            box = new mygame.Graphics.Image[4];
            for (int i = 0; i < Max_Obj; i++)
            {
                box[i] = new mygame.Graphics.Image();
            }
            Mouse.Initialize();
            this.cameraPosition = new Microsoft.Xna.Framework.Vector3(0.0f, 0.0f, 30.0f);
            this.aspectRatio = (float)graphics.GraphicsDevice.Viewport.Width / (float)graphics.GraphicsDevice.Viewport.Height;
            this.aspectRatio = 1.0f;
            this.sphereRotation = 0.0f;
            this.arrow=new mygame.Graphics.Image();
            //Initializing the Position Color and Scale of the boxes.
            //Fixing 4 positions and giving them random colors and scales
            int j = 100;
            Color c1 = new Color(Color.Red, 255);
            Color c2 = new Color(Color.White, 255);
            Color c3 = new Color(Color.Green, 255);

            Color c4 = new Color(Color.Blue, 255);

            Rectangle rect=new Rectangle(0,0,220,220);
            for (int i = 0; i < Max_Obj; i++)
            {
                randomNo = ((float)GenRandom() * 10) % 2;
                randomNo = randomNo < 1 ? 1 : randomNo;

                if (i == 0)
                    box[i].Initialize(new Vector2(100, 500), c1, new Vector2((float)0.3), rect);
                else if (i == 1)
                    box[i].Initialize(new Vector2(250, 500), c2, new Vector2((float)0.48),rect);
                else if (i == 2)
                    box[i].Initialize(new Vector2(400, 500), c3, new Vector2((float)0.25),rect);
                else if (i == 3)
                    box[i].Initialize(new Vector2(550, 500), c4, new Vector2((float)0.4),rect);


            }

            keepAt = new Vector2(300,400-120);
            randomNo = (float)GenRandom();
            y1 = (float)randomNo;
            x1 = 1f;
            m = y1 / x1;
            base.Initialize();
        }

        protected override void LoadGraphicsContent(bool loadAllContent)
        {
            //this.font = this.content.Load<SpriteFont>("Content\\font\\gamefont");

            if (loadAllContent)
            {
                this.sphere = this.content.Load<Microsoft.Xna.Framework.Graphics.Model>("Content\\Models\\newSphere");
                this.boneTransforms = new Matrix[sphere.Bones.Count];

                this.myTexture = this.content.Load<Microsoft.Xna.Framework.Graphics.Texture2D>("Content\\textures\\wooden texture");
                this.spriteBatch = new Microsoft.Xna.Framework.Graphics.SpriteBatch(this.graphics.GraphicsDevice);
                for (int i = 0; i < Max_Obj; i++)
                {
                    this.box[i].LoadGraphicsContent(myTexture, spriteBatch);
                }

                this.myTexture = this.content.Load<Microsoft.Xna.Framework.Graphics.Texture2D>("Content\\textures\\cursor");
                Mouse.LoadGraphicsContent(spriteBatch, myTexture);
                
                this.myTextureTable = this.content.Load<Microsoft.Xna.Framework.Graphics.Texture2D>("Content\\textures\\horizontal");
                //this.myTextureTable = this.content.Load<Microsoft.Xna.Framework.Graphics.Texture2D>("Content\\textures\\table");
                this.arrowTexture = this.content.Load<Microsoft.Xna.Framework.Graphics.Texture2D>("Content\\textures\\arrow");
                this.arrow.LoadGraphicsContent(this.arrowTexture, spriteBatch);

                this.mysong = this.content.Load<Song>("Content\\Sounds\\12 Wake Up");


            }
        }

        protected override void UnloadGraphicsContent(bool unloadAllContent)
        {
            if (unloadAllContent)
            {
                this.content.Unload();
            }
        }


        protected override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            this.graphics.GraphicsDevice.Clear(Microsoft.Xna.Framework.Graphics.Color.White);

            //this.DrawModel();
            //this.DrawString();

            #region inner Draw()
            /*
            this.sphere.CopyAbsoluteBoneTransformsTo(boneTransforms);

            foreach (ModelMesh mesh in this.sphere.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = boneTransforms[mesh.ParentBone.Index]*Matrix.CreateRotationX(sphereMotion)*Matrix.CreateRotationY(sphereMotion)*Matrix.CreateRotationZ(sphereMotion) *Matrix.CreateTranslation(spherePosition.X,spherePosition.Y,spherePosition.Z) ;
                    effect.View = Matrix.CreateLookAt(cameraPosition, Vector3.Zero, Vector3.Down);
                    effect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(50.0f), aspectRatio, 1.0f, 10000f);
                    effect.EnableDefaultLighting();

                
                }
                mesh.Draw();
                
            }
            */
            #endregion


            #region 2D Draw()

            this.spriteBatch.Begin(Microsoft.Xna.Framework.Graphics.SpriteBlendMode.AlphaBlend);
            
            this.spriteBatch.Draw(this.myTextureTable, new Vector2(300, 405), new Rectangle(0, 0, 500, 300), Microsoft.Xna.Framework.Graphics.Color.White,
                0, new Vector2(150,150), Vector2.One, SpriteEffects.None, 0.5f);
            if (arrowDisplayFlag == 1)
            {
                arrow.Draw(gameTime);
            }

            for (int i = 0; i < Max_Obj; i++)
            {
                box[i].Draw(gameTime);

            }
            


            //this.spriteBatch.Draw(this.myTexture, new Vector2(200,400),new Rectangle(0,0,400,10), Microsoft.Xna.Framework.Graphics.Color.White,
               // 0,Vector2.Zero,Vector2.One,SpriteEffects.None,0.5f);
            this.spriteBatch.End();

            this.spriteBatch.Begin(Microsoft.Xna.Framework.Graphics.SpriteBlendMode.AlphaBlend);
            Mouse.Draw(gameTime);
            this.spriteBatch.End();



            base.Draw(gameTime);


            #endregion
        }

        protected override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {

            #region 2D Update

            Mouse.Update(gameTime);

            #endregion

            #region Checking Time of Idleness
            int xl = 150, xu = 450;
            t1 += (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            //if (t1 - t2 >= checkTime)
            //{
                
                float findMaxScale=0;
                
                for (int i = 0; i < Max_Obj; i++)
                {
                    if (box[i].scale.X > findMaxScale && box[i].flag==0)
                    {
                        bigBox = i;
                        findMaxScale = box[i].scale.X;
                    }
                }

            if (t1 - t2 >= checkTime)
            {
                //initialising the arrow and setting its flag for drawing it 
                arrow.Initialize(new Vector2(box[bigBox].Position.X + box[bigBox].origin.X * box[bigBox].scale.X + 0.3f * 200, box[bigBox].Position.Y), Color.White, new Vector2(0.3f, 0.3f), new Rectangle(0, 0, 200, 200));
                

                arrowDisplayFlag = 1;
                if (box[bigBox].flag == 1)
                {
                    arrowDisplayFlag = 0;

                }
                /*else
                {
                    arrow.Initialize(new Vector2(box[bigBox].Position.X + arrow.scale.X * arrow.SourceRectangle.Width, box[bigBox].Position.Y), Color.White, new Vector2(0.3f, 0.3f), new Rectangle(0, 0, 200, 200));
                    arrowDisplayFlag = 1;
                }*/
                
                
                t2 = t1;
            }
            mover = (t1*15)%8;
            if (arrowDisplayFlag==1)
            {
                arrow.Position = new Vector2(box[bigBox].Position.X + box[bigBox].origin.X * box[bigBox].scale.X + 0.3f*100 - mover, box[bigBox].Position.Y);
            }

            if (t1 - t3 >= checkTime * 2 && box[bigBox].flag==0)
            {
                autoKeepFlag = 1;
                j = bigBox;
            }
            #endregion


            
            

            //mousestate=Microsoft.Xna.Framework.Input.Mouse.GetState();
            if (Mouse.mouseState.LeftButton == ButtonState.Pressed || autoKeepFlag==1)
            {
                double min = 1000;//= new Vector2(10000);
                arrowDisplayFlag = 0;
                t2 = t1;
                t3 = t1;
                if ((Mouse.mouseState.Y > 400 ||( Mouse.mouseState.X<xl || Mouse.mouseState.X>xu )) || fl==1 || autoKeepFlag==1)
                {
                    if (autoKeepFlag == 0)
                    {

                        for (int i = 0; i < Program.Max_Obj; i++)
                        {
                            if (box[i].flag == 0)
                            {


                                double dis = System.Math.Sqrt(System.Math.Pow(MathHelper.Distance(Mouse.mouseState.X, box[i].Position.X), 2) + System.Math.Pow(MathHelper.Distance(Mouse.mouseState.Y, box[i].Position.Y), 2));
                                if (min > dis)
                                {
                                    min = dis;
                                    j = i;
                                }
                            }
                        }


                        //Update Position
                        if(box[j].flag==0)
                            box[j].Position = new Vector2(Mouse.mouseState.X, Mouse.mouseState.Y);



                        if (Mouse.mouseState.Y <= 410)
                            fl = 1;
                        else if (Mouse.mouseState.Y > 400)
                            fl = 0;
                    }
                    //if (Mouse.mouseState.LeftButton == ButtonState.Released)
                    {

                        if ((box[j].Position.Y <= 400 && box[j].Position.X >= xl && box[j].Position.X <= xu) || autoKeepFlag==1)
                        {
                            if (j == bigBox && box[j].flag==0)
                            {
                                arrowDisplayFlag = 0;
                                keepAt.Y = keepAt.Y - box[j].origin.Y * box[j].scale.Y;
                                //keepAt.X = box[j].Position.X;
                                box[j].Position = new Vector2(keepAt.X, keepAt.Y);
                                keepAt.Y = keepAt.Y - box[j].origin.Y * box[j].scale.Y;
                                box[j].flag = 1;
                                fl = 0;
                                autoKeepFlag = 0;
                            }
                        }

                    }

                    /*if (MathHelper.Distance(box[j].Position.X, 200) <= 50 && MathHelper.Distance(box[j].Position.Y, 200) <= 50)
                    {
                        box[j].Position = new Vector2(Mouse.mouseState.X, Mouse.mouseState.Y);
                        box[j].flag = 1;
                        fl = 0;
                        
                    }*/
                }
            }

            


            float time;
                sphereRotation += (float)gameTime.ElapsedGameTime.TotalSeconds;


                
                x1 = spherePosition.X;
                y1 = spherePosition.Y;

               
                

                
                if(state==0 && (spherePosition.X>= xLimitPositive || spherePosition.X<=xLimitNegative ||spherePosition.Y>=yLimitPositive||spherePosition.Y<=yLimitNegative))
                {
                        state=1;
                }

                //starting from the origin
                #region state=0
                if (state==0)
                {    
                    spherePosition.X += translateSpeed;
                    spherePosition.Y = m * spherePosition.X;

                }
                #endregion

                //new trajectory
                #region state=1
                if (state == 1)
                {
                    //slope has to be changed but only once
                    if (x1 >= xLimitPositive || x1 <= xLimitNegative || y1 >= yLimitPositive || y1 <= yLimitNegative)
                    {
                        m = -m;
                        c = y1 - m * x1;
                        x0=x1;
                        
                    }

                    //move the sphere on the tajectory ie a st line until it reaches a limit

                    if(x1 >= xLimitPositive)
                    {
                        decrease = 1;
                        
                    }

                    else if (x1 <= xLimitNegative)
                    {
                        decrease = 0;
                        
                    }

                    else if (y1 >= yLimitPositive)
                    {
                        decrease = 4;
                    }

                    else if (y1 <= yLimitNegative)
                    {
                        decrease=3;
                        
                    }

                    if(decrease==0)
                    {
                        spherePosition.X += translateSpeed;
                        spherePosition.Y = m * spherePosition.X + c;
                    }

                    else if(decrease==1)
                    {
                        spherePosition.X -= translateSpeed;
                        spherePosition.Y = m * spherePosition.X + c;
                    }

                    else if (decrease==3)
                    {
                        spherePosition.Y += translateSpeed;
                        spherePosition.X = (spherePosition.Y - c) / m;
                    }
	
                    
                    else if (decrease==4)
                    {
                        spherePosition.Y -= translateSpeed;
                        spherePosition.X = (spherePosition.Y - c) / m;
                    }
	

                }
                #endregion
           
            base.Update(gameTime);
        }

        #endregion


        #region Extra Methods For 3D Functions
        private void DrawModel()
        {
            this.sphere.CopyAbsoluteBoneTransformsTo(boneTransforms);

            foreach (ModelMesh mesh in this.sphere.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = boneTransforms[mesh.ParentBone.Index] * Matrix.CreateTranslation(spherePosition) * Matrix.CreateRotationZ(MathHelper.ToRadians(sphereRotation));
                    effect.View = Matrix.CreateLookAt(cameraPosition, Vector3.Zero, Vector3.Up);
                    effect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(50.0f), aspectRatio, 1.0f, 10000f);
                    effect.EnableDefaultLighting();


                }
                mesh.Draw();

            }
        }
        private double GenRandom()
        {

            Random random;
            random = new Random();
            return random.NextDouble();
        }
        private void DrawString()
        {
            //this.spriteBatch.DrawString( font, "hii" , Vector2.Zero, Color.White);
        }
        #endregion


    }

}

        