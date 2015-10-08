using System;
using System.Xml;
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
using Snakes_and_Ladders;
using Snakes_and_Ladders.Graphics;
using Snakes_and_Ladders.Input;
using MySql.Data.MySqlClient;


namespace Snakes_and_Ladders
{

    public class Engine : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        ContentManager content;

        //Viewport
        Viewport V;
        

        //Board of the game (fixed for now but can be changed later)
        Image board;
        Texture2D boardTexture;
        int size_of_board = 64;
        int[] snakes_array;
        int[] ladder_array;

        //Mouse Cursor
        Texture2D cursorTexture;

        //Music
        //Sound Effects
        Song gamePlayMusic;
        SoundEffect panelSound;
        SoundEffect tokenMoveSound;
        SoundEffect snakeBiteSound;
        SoundEffect ladderClimbSound;
        SoundEffect winSound;
        SoundEffect loseSound;
        int flagPanelSound = 1;

        //Token (to be animated)
        Image[] token;
        Texture2D tokenTexture;
        
        //Pictures of Objects ..retrieved from database
        public int PanelSize 
        {
            get { return panelSize; }
            set { panelSize=value;}
        }

        
        int panelSize = 4;
        Image[] panel;
        int[] selected;//the sequence of those textures which are to be shown
        string[] objectName;//name of the image file in objects directory
        int[] sound;//the no. of sounds of those objects that are to be shown
        Texture2D[] tex ;

        Vector2 startPosOfPanel = new Vector2(650, 20);
        Texture2D panelTexture;

        //Desired Scale of each object of the panel
        Vector2 desiredScale = new Vector2(140, 140);
        int reduceDesiredScale = 40;

        //Token Movement variables
        Vector2 startPosition;
        Vector2 currentPosition;
        
        int moveDirection=1;
        int stepCount=1;
        Vector2 sideError;
        int flag = 1;
        int flagTurnCpu = 0;
        
        float horizontalBlockLenth=100,verticalBlockLenth=84;



        //Board Specific
        int horizontalBlocks=5;
        int verticalBlocks=6;
        int endNumber = 30;
        int lengthOfBoard = 600;
        
        //projectile animation       
        int horizontalSteps = 8;
        int verticalSteps = 6;
        float moveInX ;
        float moveInY ;


        //cpu token texture
        Texture2D cpuTexture;
        
        //cpu turn texture
        Texture2D cpuTurnTexture;
        Image cpu;
        //player image
        Texture2D playerTexture;
        Image player;


        //fields for update method
        int turn = 0;// Player's turn=0, CPUs turn=1

        //UI elements        
        string gameString = "Hello World!!";
        SpriteFont font;
        
        //Time keeping
        double t1, t2=0, t3 = 1500;

        //border on mouse over
        Image border;
        Texture2D borderTexture;
        int flagDisplayBorder = 0;

        public Engine()
        {
            graphics = new GraphicsDeviceManager(this);
            this.content = new Microsoft.Xna.Framework.Content.ContentManager(this.Services);
            Content.RootDirectory = "Content";
            //viewport
            
            /*this.V = graphics.GraphicsDevice.Viewport;
            V.Height = 800;
            V.Width = 800;
            V.X = 100;*/
            //graphics.GraphicsDevice.Viewport = V;
            graphics.PreferredBackBufferHeight = 650;
            graphics.PreferredBackBufferWidth = 1100;
            //array of snakes positions
            snakes_array = new int[size_of_board];
            //array of ladder positions
            ladder_array = new int[size_of_board];

            selected = new int[panelSize];
            objectName = new string[panelSize];
            sound = new int[panelSize];
            tex = new Texture2D[panelSize];

            Content.RootDirectory = "Content";
            token = new Image[2];
            for (int i = 0; i < token.Length; i++)
            {
                token[i] = new Image();
            }
            border = new Image();
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
            
            board = new Image();
            
            board.Initialize(new Vector2(0,0), Color.White, new Vector2(600/680f,600/794f), new Rectangle(0,0,680,794),1);
            //Board Specific Details
            sideError = new Vector2(0, 0);
            startPosition = new Vector2(board.Position.X + 75 + sideError.X, board.Position.Y + 490 + sideError.Y);
            currentPosition = startPosition;

            //initialize ladder array
            ladder_array[7] = 19;
            ladder_array[14] = 25;
            ladder_array[23] = 29;
            //initialize snakes array
            snakes_array[10] = 3;
            snakes_array[24] = 9;
            snakes_array[28] = 12;

            //Player
            token[0].Initialize(startPosition, Color.White,new Vector2(40/140f,40/137f), new Rectangle((int)startPosition.X, (int)startPosition.Y, 140, 137), 0);
            Snakes_and_Ladders.Input.Mouse.Initialize();


            //Panel
            panel = new Image[panelSize];
            for (int i = 0; i < panelSize; i++)
            {
                panel[i] = new Image();
            }   
            


            //Select the sequence randomly of the objects to be shown
            selectObjects();
            //get the actual objects
            decider();
                    
            //Cpu
            
            token[1].Initialize(new Vector2(startPosition.X+10,startPosition.Y),Color.Red,new Vector2(40/140f,40/137f), new Rectangle((int)startPosition.X+10, (int)startPosition.Y, 140, 137), 0);

            moveInX = (horizontalBlockLenth / horizontalSteps);
            moveInY = (horizontalBlockLenth / verticalSteps);


            //cpu image
            cpu = new Image();
            //player image
            player = new Image();
                        
           

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

            //board
            this.boardTexture = this.content.Load<Microsoft.Xna.Framework.Graphics.Texture2D>("Content\\Sprite\\board4");
            board.LoadGraphicsContent(boardTexture, spriteBatch);

            //Mouse cursor
            this.cursorTexture = this.content.Load<Microsoft.Xna.Framework.Graphics.Texture2D>("Content\\Sprite\\cursor");
            Snakes_and_Ladders.Input.Mouse.LoadGraphicsContent(spriteBatch,cursorTexture);

            //Token
            this.tokenTexture = this.content.Load<Microsoft.Xna.Framework.Graphics.Texture2D>("Content\\Sprite\\goti");
            token[0].LoadGraphicsContent(tokenTexture, spriteBatch);
           
            //Panel objects
            for (int i = 0; i < panelSize; i++)
            {
                this.tex[i]=this.content.Load<Texture2D>(string.Concat("Content\\Objects\\", objectName[i].Substring(0, objectName[i].Length - 4)));
                panel[i].LoadGraphicsContent(tex[i], spriteBatch);
            }
            //Initialize the panel objects
            for (int i = 0; i < panelSize; i++)
            {
                panel[i].Initialize(new Vector2(startPosOfPanel.X, startPosOfPanel.Y + i * 160), 
                    Color.White, 
                    new Vector2(desiredScale.X / tex[i].Width, desiredScale.Y / tex[i].Height),
                    new Rectangle((int)startPosOfPanel.X, (int)startPosOfPanel.Y + i * 160,tex[i].Width,tex[i].Height),
                    0);
            }

            //CPU
            cpuTexture = this.content.Load<Texture2D>("Content\\Sprite\\goti");
            token[1].LoadGraphicsContent(cpuTexture, spriteBatch);

            //font
            font = Content.Load<SpriteFont>("gameFont");

            //cpu turn texture
            cpuTurnTexture = this.content.Load<Texture2D>("Content\\Sprite\\cpu");
            cpu.LoadGraphicsContent(cpuTurnTexture, spriteBatch);

            //player turn texture
            playerTexture = this.content.Load<Texture2D>("Content\\Sprite\\player");
            player.LoadGraphicsContent(playerTexture, spriteBatch);

            //initialize cpu and player images
            
            cpu.Initialize(new Vector2(850, 350), Color.White,
                new Vector2((desiredScale.X - reduceDesiredScale)/ cpuTurnTexture.Width, (desiredScale.Y - reduceDesiredScale)/ cpuTurnTexture.Height),
                new Rectangle(800, 50, cpuTurnTexture.Width, cpuTurnTexture.Height),
                0);
            player.Initialize(new Vector2(850, 150), Color.White,
                new Vector2((desiredScale.X - reduceDesiredScale)/ playerTexture.Width, (desiredScale.Y - reduceDesiredScale) / playerTexture.Height ),
                new Rectangle(800, 50, playerTexture.Width, playerTexture.Height),
                0);

            //border
            borderTexture = this.content.Load<Texture2D>("Content\\Sprite\\border");
            border.LoadGraphicsContent(borderTexture, spriteBatch);
            border.Initialize(Vector2.Zero,
                Color.DarkRed,
                Vector2.One,
                new Rectangle(0, 0, borderTexture.Width, borderTexture.Height),
                0);
            border.active = 0;


            //Background Music
            gamePlayMusic = content.Load<Song>("Content\\Sound\\Music");
            playMusic(gamePlayMusic);

            //SoundEffects
            panelSound = content.Load<SoundEffect>("Content\\Sound\\beep8");
            winSound = content.Load<SoundEffect>("Content\\Sound\\applause");
            //loseSound = content.Load<SoundEffect>("Content\\Sound\\loseSound");
        }

        void playMusic(Song song)
        {
            try
            {
                MediaPlayer.Play(song);
                MediaPlayer.IsRepeating = true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            this.content.Unload();
            
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            Snakes_and_Ladders.Input.Mouse.Update(gameTime);
            /*
             * Summary of Update function
             * 
             * If it is player's turn
             * update the panel of sprites if the game is still running
             * Intersection of Sprites - panel sprites and mouse sprite
             * move the goti with the sound of the clicked sprite
             * check for 4 conditions
             *      1 snake is encountered.
             *      2 ladder is encountered.             *      
             *      3 finish is encountered.
             *      4 nothing is encountered.
             *      For 1 and 2 move the sprite to its actual position, for 3 ,end the game
             * change the turn to CPUs turn.
             * 
             * Else if its CPUs turn
             * generate a random number between 1 and 6 inclusive.
             * move the goti with the random number.
             * check the same three conditions as mentioned above.
             * change the turn to player's turn
             
             */
            t1 = gameTime.TotalGameTime.TotalMilliseconds;

            if (turn==0)
            {
                //players turn

                //increase size of players sprite
                player.scale.X = (float)desiredScale.X / playerTexture.Width;
                player.scale.Y = (float)desiredScale.Y / playerTexture.Height;

                //decrease size of cpu sprite
                cpu.scale.X = (float)(desiredScale.X - reduceDesiredScale)/ cpuTurnTexture.Width;
                cpu.scale.Y = (float)(desiredScale.Y - reduceDesiredScale)/ cpuTurnTexture.Height;



                int check=checkIntersection();
                //Console.WriteLine(check);
                int gameControl = 0;

                //border over panel objects
                if (check>0)
                {                   
                    border.active = 1;
                    border.Position = new Vector2(panel[check - 1].Position.X - 10, panel[check - 1].Position.Y - 10);
                    border.scale = new Vector2((desiredScale.X + 20) / borderTexture.Width, 
                        (desiredScale.Y + 20) / borderTexture.Height);
                    if (flagPanelSound>0)
                    {
                        //panelSound.Play();
                        flagPanelSound = 0;
                    }
                }
                else
                {
                    border.active = 0;
                    flagPanelSound = 1;
                }

                if (check>0 && Input.Mouse.mouseState.LeftButton == ButtonState.Pressed && flag==1)
                {
                    
                    int moveBy = sound[check - 1];                    
                    gameControl=moveToken(moveBy,turn,gameTime);
                    checkBoardConstraints(gameControl,turn, gameTime);//also move if necessary                                        
                    //turn = 1;//change to cpus turn
                    flag = 0;

                    Console.WriteLine("Player:" + token[0].currentNumber+" POsition:" + token[0].Position.X +"'"+token[0].Position.Y+" moved by:" +moveBy);
                }
                       
                else if (Input.Mouse.mouseState.LeftButton==ButtonState.Released && flag==0)
                {
                    t2 += gameTime.ElapsedGameTime.Milliseconds;
                    if (t2 > t3)
                    {
                        t2 = 0;
                        flag = 1;
                        turn = 1;
                        selectObjects();
                        decider();
                        for (int i = 0; i < panelSize; i++)
                        {
                            this.tex[i] = this.content.Load<Texture2D>(string.Concat("Content\\Objects\\", objectName[i].Substring(0, objectName[i].Length - 4)));
                            panel[i].Update(this.tex[i], new Vector2(desiredScale.X / tex[i].Width, desiredScale.Y / tex[i].Height));
                        }
                    }

                }
                
            }
            else if (turn==1)
            {
                //cpus turn

                //increase size of cpu sprite
                cpu.scale.X = (float)(desiredScale.X ) / cpuTurnTexture.Width;
                cpu.scale.Y = (float)(desiredScale.Y ) / cpuTurnTexture.Height;
       
                //decrease size of player sprite
                player.scale.X = (float)(desiredScale.X - reduceDesiredScale) / playerTexture.Width;
                player.scale.Y = (float)(desiredScale.Y - reduceDesiredScale) / playerTexture.Height;


                /*if (flagTurnCpu==1)
                {
                    Random r = new Random();
                    int moveBy = r.Next(1, 6);
                    int gameControl = moveToken(moveBy, turn, gameTime);
                    checkBoardConstraints(gameControl, turn, gameTime);
                    flagTurnCpu = 0;
                }*/
                t2 += gameTime.ElapsedGameTime.Milliseconds;
                if (t2 > t3)
                {
                    Random r = new Random();
                    int moveBy = r.Next(1, 6);
                    int gameControl = moveToken(moveBy, turn, gameTime);
                    checkBoardConstraints(gameControl, turn, gameTime);
                    flagTurnCpu = 0;
                    turn = 0;
                    flagTurnCpu = 1;
                    t2 = 0;
                    
                }
                //Console.WriteLine("CPU:" + token[1].currentNumber+",MoveBy:"+ moveBy);
            }
            else if (turn == -1)
            {
               /* Game has ended
                * check who has won
               */
                if (token[0].currentNumber>token[1].currentNumber)
                {
                    gameString = "You Win!!";
                    winSound.Play();
                }
                else
                {
                    gameString = "CPU Wins!!";
                }
                turn = -2;
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Green);
            this.spriteBatch.Begin(SpriteBlendMode.None);
            this.board.Draw(gameTime);
            this.spriteBatch.End();

            //draw the border if it is active
            if (border.active == 1)
            {
                spriteBatch.Begin();
                border.Draw(gameTime);
                spriteBatch.End();
            }

            
            this.spriteBatch.Begin();
            for (int i = 0; i < panelSize; i++)
            {
                this.panel[i].Draw(gameTime);
            }
            this.spriteBatch.End();
            

            //cpu token
            this.spriteBatch.Begin();
            this.token[1].Draw(gameTime);
            this.spriteBatch.End();


            //players token
            this.spriteBatch.Begin();
            this.token[0].Draw(gameTime);
            this.spriteBatch.End();


            //turn sprites of player and cpu
            spriteBatch.Begin();
            this.player.Draw(gameTime);
            this.cpu.Draw(gameTime);
            spriteBatch.End();

           
            //draw mouse cursor
            this.spriteBatch.Begin(Microsoft.Xna.Framework.Graphics.SpriteBlendMode.AlphaBlend);
            Snakes_and_Ladders.Input.Mouse.Draw(gameTime);
            this.spriteBatch.End();

            

            if (turn == -2)
            {
                //draw the string
                spriteBatch.Begin();
                spriteBatch.DrawString(font, gameString,
                    new Vector2(GraphicsDevice.Viewport.Width / 2 - 100, GraphicsDevice.Viewport.Height / 2), Color.BlueViolet);
                spriteBatch.End();
            }
            //position of player and cpu
            spriteBatch.Begin();
            spriteBatch.DrawString(font,Convert.ToString(token[0].currentNumber),
                new Vector2(1000,150), Color.Yellow);
            spriteBatch.DrawString(font, Convert.ToString(token[1].currentNumber),
                new Vector2(1000, 350), Color.Red);
            
            spriteBatch.End();

            base.Draw(gameTime);
        }
        void snakeBite(int moveBy, int turn)
        {
            currentPosition = token[turn].Position;
            for (int i = 0; i < moveBy; i++)
            {
                token[turn].currentNumber--;
                token[turn].currentRow = getCurrentRow(token[turn].currentNumber);



                if (token[turn].lastRow - token[turn].currentRow == 1)
                {
                    currentPosition.Y += verticalBlockLenth;
                }
                else if (token[turn].currentRow % 2 == 0)
                {
                    currentPosition.X -= horizontalBlockLenth;
                }
                else if (token[turn].currentRow % 2 != 0)
                {
                    currentPosition.X += horizontalBlockLenth;
                }

                token[turn].lastRow = token[turn].currentRow;
            }
            token[turn].Position = currentPosition;
            currentPosition = Vector2.Zero;
        }

        private void checkBoardConstraints(int control,int turn, GameTime gTime)
        {
            if (control == 1)//snake is encountered
            {

                int moveBy = token[turn].currentNumber - snakes_array[token[turn].currentNumber];
                snakeBite(moveBy,turn);
            }
            else if (control == 2)//ladder
            {
                int moveBy = ladder_array[token[turn].currentNumber] - token[turn].currentNumber;
                moveToken(moveBy,turn,gTime);
            }
            else if (control == 3)//end
            {
                this.turn = -1;
            }
            /*else if (control == 4)//nothing
            {
                turn = 1;
            }*/
            
        }

        private void projectile(int direction, int turn, GameTime gTime)
        {
            double checkTime = 150, currentTime , previousTime = gTime.TotalGameTime.TotalMilliseconds;
            int horizontalSteps = 8;
            int verticalSteps = 6;
            int t = turn;
            float moveInX = (horizontalBlockLenth / horizontalSteps) * direction;
            float moveInY = (horizontalBlockLenth / verticalSteps);
            for (int i = 0; i < horizontalSteps ;)
            {                
                turn = -1;
                Update(gTime);
                currentTime = gTime.TotalGameTime.TotalMilliseconds - previousTime;
                if (checkTime < currentTime)
                {
                    if (i<horizontalSteps/2)                    
                        token[turn].Position = new Vector2(token[turn].Position.X + moveInX, token[turn].Position.Y + moveInY);
                    else
                        token[turn].Position = new Vector2(token[turn].Position.X + moveInX, token[turn].Position.Y - moveInY);
                    token[turn].Draw(gTime);
                    i++;
                }
            }
            turn = t;
            
        }

        private int moveToken(int moveBy, int turn , GameTime gTime)
        {           
            currentPosition = token[turn].Position;
            for (int i = 0; i < moveBy; i++)
            {
                token[turn].currentNumber++;
                token[turn].currentRow = getCurrentRow(token[turn].currentNumber);

                if (token[turn].currentRow - token[turn].lastRow == 1)
                {
                    currentPosition.Y -= verticalBlockLenth;
                }
                else if (token[turn].currentRow % 2 == 0)
                {
                    currentPosition.X += horizontalBlockLenth;                    
                }
                else if (token[turn].currentRow % 2 != 0)
                {
                    currentPosition.X -= horizontalBlockLenth;                    
                }

                token[turn].Position = currentPosition;
                token[turn].lastRow = token[turn].currentRow;
            }
            currentPosition = Vector2.Zero;

            if (snakes_array[token[turn].currentNumber] > 0)
            {
                return 1;
            }
            else if (ladder_array[token[turn].currentNumber] > 0)
            {
                return 2;
            }
            else if (token[turn].currentNumber >= endNumber)
            {
                return 3;
            }
            else
                return 4;


        }
        
        int getCurrentRow(int num)
        {
            int a = num / horizontalBlocks;
            int b = num % horizontalBlocks;
            if (b==0)
            {
                return a - 1;
            }
            else if (b!=0)
            {
                return a;
            }


            return 0;
        }


        //checks intersection between mouse and  a panel object
        private int checkIntersection()
        {
            //rectangle1 = new Rectangle((int)lasers[i].position.X, (int)lasers[i].position.Y, lasers[i].width, lasers[i].height);                
            Rectangle recMouse = new Rectangle(Input.Mouse.mouseState.X,Input.Mouse.mouseState.Y,cursorTexture.Width,cursorTexture.Height);            
            

            Rectangle[] recPanel = new Rectangle[panelSize];
            for (int i = 0; i < panelSize; i++)
			{
                recPanel[i] = new Rectangle((int)panel[i].Position.X, (int)panel[i].Position.Y, (int)desiredScale.X, (int)desiredScale.Y);
			}

            for (int i = 0; i < panelSize; i++)
            {
                if (recMouse.Intersects(recPanel[i])==true)
                {
                    return i+1;
                }
                
            }
            return 0;
        }
    
    

        // gets a new panel objects. sequence can be found in selected array in unique and sorted  order 
        void selectObjects()
        {            
            Random r = new Random();
            int count=0;
            Selector s = new Selector();
            MySqlDataReader reader = s.fun("select count(*) from picture");
            while (reader.Read())
                count = Convert.ToInt32(reader[0].ToString());
            s.conn.Close();
            for (int i = 0; i < panelSize; i++)
            {
                //get random numbers to corr to the object to select and make sure they are unique
                selected[i] = r.Next(0,count);

                for (int j = 0; j < i; j++)
                {
                    if (selected[j]==selected[i])
                    {
                        i -= 1;
                        continue;
                    }
                }
            }
           
            Array.Sort(selected);
        }
        // according to the selected array sequence this function gets the path and sound of the object.
        void decider()
        {
            Selector s = new Selector();
            MySqlDataReader r = s.fun("select * from picture,english_detail where picture.id=english_detail.pid");
            int count = 0;
            int i=0;
            while (r.Read())
            {
                if (selected.Contains<int>(count))
                {
                    objectName[i] = string.Copy(r["path"].ToString());
                    sound[i] = Convert.ToInt32( r["sound"].ToString());                    
                    i++;
                }
                count++;
            }
            s.conn.Close();
        }
    }
}