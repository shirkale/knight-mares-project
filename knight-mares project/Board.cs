using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace knight_mares_project
{
    public abstract class Board : View
    {
        protected Context context;
        protected Square[,] squares; // board 
        public int size; // size * size = board size
        protected Knight player; // knight piece that moves on the board

        protected bool firstDraw; // helps create the matrix
        protected bool firstKnight; // helps initiailize the knight
        protected Paint borders; // paintbrush for borders


        protected int checkWin; // when checkWin reaches 0, it means all the walkable squares are walked on

        protected readonly int[] xMove = { -1, 1, 2, 2, 1, -1, -2, -2 };
        protected readonly int[] yMove = { 2, 2, 1, -1, -2, -2, -1, 1 };

        public EventHandler winEvent; // event which when triggered in GameActivity shows winning dialog

        protected Stack<Square> moves; // stack into which we insert squares the player goes on

        protected Square starter; // saves the starter square

        public Board(Context context, int size) : base(context)
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

            this.moves = new Stack<Square>();
        }

        protected override void OnDraw(Canvas canvas) { }

        public abstract void GoBack();
        public override bool OnTouchEvent(MotionEvent e)
        {
            if (e.Action == MotionEventActions.Down && !Board_Knight_s_Tour.solve)
            {
                Square newSquare = FindClickedSquare((int)e.GetX(), (int)e.GetY()); // find the square that the user clicked on
                if (this.player.GetCurrentSquare().CanMakeJump(newSquare)) // if the player can make the jump it will jump and step on the clicked square
                {
                    this.moves.Push(this.player.GetCurrentSquare()); // pushes move into the stack for later undos
                    this.player.MoveToSquare(newSquare);

                    if (!(newSquare is MultipleStepSquare))
                        checkWin--;

                    newSquare.StepOn();
                    Invalidate();
                }
                //Toast.MakeText(this.context, "" + this.checkWin, ToastLength.Short).Show();
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

        protected virtual void InitializeKnight()
        {
            this.player = new Knight(this.starter, this.context);
            (starter as GhostSquare).ResizeBitmap(false);
            this.firstKnight = false;
            starter.StepOn();
        }

        protected void PickRandomStarter() // picking random square for starter position
        {
            Random random = new Random();
            int i = random.Next(0, size);
            int j = random.Next(0, size);

            this.starter = (GhostSquare)squares[i, j];
        }

        protected bool EdgeCheck(int newX, int newY) // returns true if the x and y are on the board
        {
            return (!(newX < 0 || newX >= this.size || newY < 0 || newY >= this.size));
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
                        this.squares[i, j] = new GhostSquare(x, y, w, h, i, j, this.context, size);
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

        protected void GoOverAll(bool step) // for loop that goes over all squares on board, if step is true the squares are stepped on, if not squares are unsteped on
        {
            for (int i = 0; i < this.size; i++)
            {
                for (int j = 0; j < this.size; j++)
                {
                    if (step)
                    {
                        squares[i, j].StepOn();
                        if(squares[i, j] is GhostSquare) { (squares[i, j] as GhostSquare).SetBitmap(MainActivity.snowtree); } // there is need for them to be trees to reset, not all flags.
                    }
                    else
                        squares[i, j].UnstepOn();
                }
            }
        }
    }
    }