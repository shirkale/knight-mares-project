using Android.Content;
using Android.Graphics;
using Android.Views;
using System;
using System.Collections;
using System.Threading;

namespace knight_mares_project
{
    public class Board : View
    {
        private Context context;
        private Square[,] squares;
        private int size; // size * size = board size
        private Knight player; // knight piece that moves on the board

        private bool firstDraw; // helps create the matrix
        private bool firstKnight; // helps initiailize the knight
        private Paint borders; // paintbrush for borders


        private int checkWin; // when checkWin reaches 0, it means all the squares are walked on
        
        private int difficulty; // maximum number of moves the computer simulates

        private readonly int[] xMove = { -1, 1, 2, 2, 1, -1, -2, -2}; 
        private readonly int[] yMove = { 2, 2, 1, -1, -2, -2, -1, 1};

        public EventHandler winEvent;
        public EventHandler updateEvent;

        private Stack moves; // stack into which we insert squares the player goes on

        public Board(Context context, int size, int difficulty) : base(context)
        {
            this.context = context;
            this.size = size;
            this.squares = new Square[size, size];
            this.player = null;
            
            firstDraw = true;
            firstKnight = true;

            this.borders = new Paint();
            this.borders.Color = Color.ParseColor("#BFA380");
            this.borders.SetStyle(Paint.Style.Stroke);
            this.borders.StrokeWidth = 10;
            this.borders.Alpha = 60;

            // this.checkWin = this.squares.Length - 1;

            this.difficulty = difficulty;

            this.moves = new Stack();

        }

        public void GoBack()
        {
            if(this.moves.Count != 0)
            {
                this.player.moveToSquare((Square)this.moves.Pop());
            }
        }

        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);
            InitializeBoard(canvas);
            if(firstKnight)
                InitializeKnight();
            this.player.Draw(canvas);
            Thread.Sleep(300);

            winEvent.Invoke(this, EventArgs.Empty);
            checkWin = 0;
            //this.player.moveToSquare(PickRandomStarter());
            if(checkWin != 0)
                Invalidate();
            //GenerateRandomMap(canvas, this.difficulty);
        }

        public override bool OnTouchEvent(MotionEvent e)
        {
            // make the winevent invoke here
            return true;
        }

        private void GenerateRandomMap(Canvas canvas, int steps) // generates map by going backwards and unsteping squares
        {
            InitializeBoard(canvas);
            if (firstKnight)
            {
                InitializeKnight();
            }
            this.player.GetCurrentSquare().UnstepOn();

            for (int i = 0; i < steps; i++)
            {
                Square nextSquare = PickRandomNextOpenPosition();
                if(nextSquare != null)
                {
                    this.player.moveToSquare(nextSquare);
                    nextSquare.UnstepOn();
                }
                else
                {
                    GenerateRandomMap(canvas, steps);
                    break;
                }
            }
        }

        private void InitializeKnight()
        {
            Square starter = PickRandomStarter();
            this.player = new Knight(starter, this.context);
            this.firstKnight = false;
            this.moves.Push(starter);
        }

        private Square PickRandomNextOpenPosition()
        {
            int i = this.player.GetCurrentSquare().GetI();
            int j = this.player.GetCurrentSquare().GetJ();

            Random random = new Random();
            int n;

            bool[] tried = new bool[8];

            for (int k = 0; k < tried.Length; k++)
                tried[k] = false;


            int newX, newY;

            while (!AllTrue(tried))
            { 
                n = random.Next(0, tried.Length);

                newX = i + xMove[n];
                newY = j + yMove[n];

                if (tried[n] == false && EdgeCheck(newX, newY))
                {
                    if (this.player.GetCurrentSquare().CanMakeJump(squares[newX, newY]))
                        return squares[newX, newY];
                }
                else
                    tried[n] = true;
            }
            return null;
        }

        private bool EdgeCheck(int newX, int newY)
        {
            return (!(newX < 0 || newX >= this.size || newY < 0 || newY >= this.size));

        }

        private bool AllTrue(bool[] boolArray)
        {
            for (int i = 0; i < boolArray.Length; i++)
            {
                if (boolArray[i] == false)
                    return false;
            }
            return true;
        }

        private Square PickRandomStarter()
        {
            // picking random square for starter position
            Random random = new Random();
            int i = random.Next(0, size);
            int j = random.Next(0, size);

            return squares[i, j];
        }

        private void InitializeBoard(Canvas canvas) // initializes board and player on first run, draws all squares
        {
            int x = 0;
            int y = 0;
            int w = canvas.Width / this.size;
            int h = canvas.Width / this.size;

            string snowtree = Helper.BitmapToBase64(BitmapFactory.DecodeResource(this.context.Resources, Resource.Drawable.snowtreesmol)); // tree picture
            string cuteGhost = Helper.BitmapToBase64(BitmapFactory.DecodeResource(this.context.Resources, Resource.Drawable.cutearmsupghostsmol)); // ghost picture
            string cuteGhostPurp = Helper.BitmapToBase64(BitmapFactory.DecodeResource(this.context.Resources, Resource.Drawable.cutearmsupghostsmolpurp)); // ghost picture purp
            string cuteGhostBlue = Helper.BitmapToBase64(BitmapFactory.DecodeResource(this.context.Resources, Resource.Drawable.cutearmsupghostsmolblue)); // ghost picture
            string bitmap;

            for (int i = 0; i < this.size; i++)
            {
                for (int j = 0; j < this.size; j++)
                {
                    if (j % 4 == 0)
                        bitmap = snowtree;
                    else if (j % 4 == 1)
                        bitmap = cuteGhostPurp;
                    else if (j % 4 == 2)
                        bitmap = cuteGhostBlue;
                    else
                        bitmap = cuteGhost; // showing all bitmaps for screens

                    if (this.firstDraw) // if it's the first time, the function will initialize the squares along with drawing them
                    {
                        this.squares[i, j] = new Square(x, y, w, h, bitmap, i, j, this.context);
                        //this.squares[i, j].StepOn(); // step on all, so later they can be unstepped
                    }

                    this.squares[i, j].Draw(canvas); // draw cur square
                    canvas.DrawRect(x, y, x + w, y + h, this.borders); // draw in borders

                    x = x + w;
                }
                y = y + h;
                x = 0;
            }

            if (this.firstDraw)
            {
                this.firstDraw = false;
            }
            else
                this.player.Draw(canvas);
        }

    }
        
}