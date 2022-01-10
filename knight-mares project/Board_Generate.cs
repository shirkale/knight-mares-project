using Android.Content;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Threading;

namespace knight_mares_project
{
    public class Board_Generate : View
    {
        protected Context context;
        protected Square[,] squares; // board 
        protected int size; // size * size = board size
        protected Knight player; // knight piece that moves on the board

        protected bool firstDraw; // helps create the matrix
        protected bool firstKnight; // helps initiailize the knight
        protected Paint borders; // paintbrush for borders


        protected int checkWin; // when checkWin reaches 0, it means all the walkable squares are walked on

        private int difficulty; // maximum number of moves the computer simulates

        protected readonly int[] xMove = { -1, 1, 2, 2, 1, -1, -2, -2};
        protected readonly int[] yMove = { 2, 2, 1, -1, -2, -2, -1, 1};

        public EventHandler winEvent;
        public EventHandler updateEvent;

        protected Stack<Square> moves; // stack into which we insert squares the player goes on

        protected Square starter; // saves the starter square

        public Board_Generate(Context context, int size, int difficulty) : base(context)
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

            this.moves = new Stack<Square>();
        }

        public void GoBack() // undo button
        {
            if(this.moves.Count != 0) // if stack isn't empty - if the player has moved
            {
                if (this.player.GetCurrentSquare() is MultipleStepSquare mssCur)
                    mssCur.SetWalkedOver(false);
                else
                    checkWin++; // checkwin goes up - player still needs to step on the square
                this.player.GetCurrentSquare().UnstepOn();
                this.player.GetCurrentSquare().ResizeBitmap(false);
                this.player.moveToSquare((Square)this.moves.Pop());
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

                    if(this.player.GetCurrentSquare() is MultipleStepSquare mss) // if first is Mult, turn it back to square
                    {
                        Square newSquare = (Square)mss;
                        this.player.SetCurrentSquare(newSquare);
                    }
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
                    if (this.squares[i, j] is MultipleStepSquare mss)
                        mss.UnstepOnFinal();
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
                    this.moves.Push(this.player.GetCurrentSquare()); // pushes move into the stack for later undos
                    this.player.moveToSquare(newSquare);

                    Bitmap newBitmap = MainActivity.snowtree;
                    if (!(newSquare is MultipleStepSquare))
                    {
                        checkWin--;
                        newBitmap = MainActivity.flag;
                    }
                    newSquare.StepOn(newBitmap);
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
                    if(nextSquare is MultipleStepSquare nextSquareMult)
                    {
                        if (k == steps - 1)
                        {
                            steps--;
                            checkWin--;
                            break;
                        }
                        else
                        {
                            nextSquareMult.ResizeBitmap(false);
                            this.player.moveToSquare(nextSquareMult);
                            nextSquareMult.UnstepOn();

                            this.checkWin--;
                        }
                        
                    }
                    else
                    {
                        chanceForMultSquare = CalculateChance();
                        if (chanceForMultSquare && k != steps - 1)
                        {
                            i = nextSquare.GetI();
                            j = nextSquare.GetJ();
                            squares[i, j] = new MultipleStepSquare(nextSquare);
                            MultipleStepSquare nextSquareMult2 = (MultipleStepSquare)squares[i, j];
                            // for each square we turn into a multiplestepsquare, we have to take the checkwin down by 1, because multstep squares aren't a necessity for winning
                            nextSquareMult2.ResizeBitmap(false);
                            this.player.moveToSquare(nextSquareMult2);
                            nextSquareMult2.UnstepOn();
                            this.checkWin--;
                        }
                        else
                        {
                            nextSquare.ResizeBitmap(false);
                            this.player.moveToSquare(nextSquare);
                            nextSquare.UnstepOn();
                        }

                    }
                    
                }
                else
                {
                    if (k < steps / 2)
                    {
                        firstDraw = true;
                        InitializeBoard(canvas);
                        GenerateRandomMap(canvas);
                    }
                    else
                        checkWin -= (steps - k);
                    break;
                }
            }
        }

        private bool CalculateChance() // calculates chance for multstepsquares
        {
            Random random = new Random();
            return random.Next(100) < 10;
        }

        protected void InitializeKnight()
        {
            Square starter = PickRandomStarter();
            this.player = new Knight(starter, this.context);
            starter.ResizeBitmap(false);
            this.firstKnight = false;
        }

        // picks a random next position according to if the player has walked on the square before
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

        protected bool EdgeCheck(int newX, int newY)
        {
            return (!(newX < 0 || newX >= this.size || newY < 0 || newY >= this.size));
        }

        protected bool AllTrue(bool[] boolArray)
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

        public void InitializeBoard(Canvas canvas) // initializes board and player on first run, draws all squares
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
                        if (this is Board_Knight_s_Tour)
                            this.squares[i, j].UnstepOn();
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