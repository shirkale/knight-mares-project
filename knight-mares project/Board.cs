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

            this.checkWin = difficulty-1;

            this.difficulty = difficulty;

            this.moves = new Stack();

        }


        public void GoBack()
        {
            if(this.moves.Count != 0)
            {
                this.player.GetCurrentSquare().UnstepOn();
                this.player.GetCurrentSquare().BitmapResized(false);
                this.player.moveToSquare((Square)this.moves.Pop());
                checkWin++;
            }
        }

        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);
            if(checkWin > 0)
            {
                InitializeBoard(canvas);
                if (firstKnight)
                    InitializeKnight();

                if (firstDraw)
                {
                    firstDraw = false;
                    GenerateRandomMap(canvas);
                    FinalUnStepOnMultSquares();
                    this.player.SetCurrentSquare((Square)this.player.GetCurrentSquare());
                    this.player.GetCurrentSquare().StepOn(MainActivity.flag);
                }
            }
            else
            {
                winEvent.Invoke(this, EventArgs.Empty);
            }
        }

        private void FinalUnStepOnMultSquares()
        {
            for (int i = 0; i < this.size; i++)
            {
                for (int j = 0; j < this.size; j++)
                {
                    if (this.squares[i, j] is MultipleStepSquare)
                    {
                        MultipleStepSquare mss = (MultipleStepSquare)this.squares[i, j];
                        mss.UnstepOnFinal();
                    }
                }
            }
        }

        public override bool OnTouchEvent(MotionEvent e)
        {
            if (e.Action == MotionEventActions.Down)
            {
                Square newSquare = FindClickedSquare((int)e.GetX(), (int)e.GetY()); // find the square that the user clicked on
                if (this.player.GetCurrentSquare().CanMakeJump(newSquare)) // if the player can make the jump it will jump and step on the clicked square
                {
                    this.moves.Push(this.player.GetCurrentSquare());
                    checkWin--;
                    this.player.moveToSquare(newSquare);
                    if(newSquare is MultipleStepSquare)
                    {
                        MultipleStepSquare mss = (MultipleStepSquare)newSquare;
                        mss.StepOn(MainActivity.flag);
                    }
                    else
                        newSquare.StepOn(MainActivity.flag);
                    Invalidate();
                }
            }
            return false;
        }

        // getting a square with x and y coordinates
        public Square FindClickedSquare(int x, int y)
        {
            for (int i = 0; i < this.size; i++)
            {
                for (int j = 0; j < this.size; j++)
                {
                    if (squares[i, j].IsXandYInSquare(x, y))
                    {
                        return squares[i, j];
                    }
                }
            }
            return squares[0, 0];
        }

        private void GenerateRandomMap(Canvas canvas) // generates map by going backwards and unsteping squares
        {
            int steps = difficulty;

            bool chanceForMultSquare;
            int i, j;

            for (int k = 0; k < steps; k++)
            {
                Square nextSquare = PickRandomNextOpenPosition();
                if(nextSquare != null)
                {
                    chanceForMultSquare = CalculateChance();
                    if (chanceForMultSquare)
                    {
                        i = nextSquare.GetI();
                        j = nextSquare.GetJ();
                        squares[i, j] = new MultipleStepSquare(nextSquare);
                        MultipleStepSquare nextSquareMult = (MultipleStepSquare)squares[i, j];
                        nextSquareMult.BitmapResized(false);
                        this.player.moveToSquare(nextSquareMult);
                        nextSquareMult.UnstepOn();
                    }
                    else
                    {
                        nextSquare.BitmapResized(false);
                        this.player.moveToSquare(nextSquare);
                        nextSquare.UnstepOn();
                    }

                }
                else
                {
                    if(k < steps/2)
                        GenerateRandomMap(canvas);
                    break;
                }
            }


        }

        private bool CalculateChance()
        {
            Random random = new Random();
            return random.Next(10000) < 10000;
        }

        private void InitializeKnight()
        {
            Square starter = PickRandomStarter();
            this.player = new Knight(starter, this.context);
            starter.BitmapResized(false);
            this.firstKnight = false;
        }

        private Square PickRandomNextOpenPosition()
        {
            int i = this.player.GetCurrentSquare().GetI();
            int j = this.player.GetCurrentSquare().GetJ();

            Random random = new Random();
            int n;

            bool[] tried = new bool[8];

            for (int k = 0; k < tried.Length; k++) // initializing tried array with false
                tried[k] = false;


            int newX, newY;

            while (!AllTrue(tried)) // if tried array is all true, then there isn't a way to get to a new square. 
            { 
                n = random.Next(0, tried.Length);

                newX = i + xMove[n];
                newY = j + yMove[n];

                if (tried[n] == false && EdgeCheck(newX, newY) && squares[newX, newY].IsWalkedOver())
                {
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

        private Square PickRandomStarter() // picking random square for starter position
        {
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


            for (int i = 0; i < this.size; i++)
            {
                for (int j = 0; j < this.size; j++)
                {
                    if (this.firstDraw) // if it's the first time, the function will initialize the squares along with drawing them
                    {
                        this.squares[i, j] = new Square(x, y, w, h, i, j, this.context);
                        this.squares[i, j].StepOn(MainActivity.snowtree); // step on all, so later they can be unstepped
                    }

                    this.squares[i, j].Draw(canvas); // draw cur square
                    canvas.DrawRect(x, y, x + w, y + h, this.borders); // draw in borders

                    x = x + w;
                }
                y = y + h;
                x = 0;
            }

            if (!this.firstDraw)
                this.player.Draw(canvas);

            Invalidate();
        }

    }
        
}