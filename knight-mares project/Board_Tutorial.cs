using Android.Content;
using Android.Graphics;
using Android.Widget;
using System;
using System.Threading;

namespace knight_mares_project
{
    public class Board_Tutorial : Board
    {
        TextView tvMessage; // textview that shows the instructions
        int delay;
        int _delaystarter;
        int DelayStarter
        {
            set { delay = value; _delaystarter = value; }
            get { return _delaystarter; }
        }

        int phase; // the phase the tutorial is in. starts at 0

        public Board_Tutorial(Context context, int size, TextView tvMessage) : base(context, size)
        {
            this.size = size;
            this.checkWin = 1;
            this.tvMessage = tvMessage;
            tvMessage.Text = "This is how a knight moves on a chessboard.\nClick the ghost to move";
            this.phase = 0;
            this.DelayStarter = 5;

        }

        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);
            InitializeBoard(canvas);

            if(phase == 0)
            {
                if (firstDraw)
                {
                    InitializeBoard(canvas);
                    tvMessage.Text = "This is how a knight moves on a chessboard.";
                    starter = squares[3, 3];
                    InitializeKnight();

                    Invalidate();
                    firstDraw = false;
                }
                else if(delay > 0)
                {
                    if(delay != DelayStarter)
                        Thread.Sleep(1000);

                    for (int i = 0; i < 8; i++)
                    {
                        Square cur = squares[starter.GetI() + xMove[i], starter.GetJ() + yMove[i]];
                        (cur as GhostSquare).SetBitmap(MainActivity.flag);
                    }

                    delay--;
                    Invalidate();
                }
                else
                {
                    DelayStarter = 5;
                    for (int i = 0; i < 8; i++)
                    {
                        Square cur = squares[starter.GetI() + xMove[i], starter.GetJ() + yMove[i]];
                        (cur as GhostSquare).SetBitmap(MainActivity.snowtree);
                    }
                    phase++;
                    firstDraw = true;
                    Invalidate();
                }
            }
            if (phase == 1)
            {
                if (firstDraw)
                {
                    tvMessage.Text += "\nClick the ghost to move";

                    // path
                    squares[5, 2].UnstepOn();

                    firstDraw = false;
                    Invalidate();
                }
                if(checkWin == 0)
                {
                    ResetBoardNextPhase("Great job! You defeated the ghost!");
                }
            }
            else if(phase == 2)
            {
                if(firstDraw)
                {
                    GoOverAll(true);
                    tvMessage.Text = "Now try with two ghosts";
                    starter = squares[6, 0];
                    player.MoveToSquare(starter);
                    checkWin = 2;

                    // path
                    squares[4, 1].UnstepOn();
                    squares[5, 3].UnstepOn();

                    firstDraw = false;
                    Invalidate();
                }
                if(checkWin == 0)
                {
                    ResetBoardNextPhase("Awesome!");
                }
            }
            else if(phase == 3)
            {
                if(firstDraw)
                {
                    GoOverAll(true);
                    tvMessage.Text = "Try This:\nNotice how you can't go back :)\nIf you get stuck, undo using the orange arrow";
                    starter = squares[2, 1];
                    starter.StepOn();
                    player.MoveToSquare(starter);
                    checkWin = 8;

                    // path
                    squares[4, 2].UnstepOn();
                    squares[2, 3].UnstepOn();
                    squares[3, 1].UnstepOn();
                    squares[4, 3].UnstepOn();
                    squares[6, 4].UnstepOn();
                    squares[5, 6].UnstepOn();
                    squares[3, 7].UnstepOn();
                    squares[1, 6].UnstepOn();

                    firstDraw = false;
                    Invalidate();
                }
                if(checkWin == 0)
                {
                    ResetBoardNextPhase("You're doing great!");
                }
            }
            else if(phase == 4)
            {
                if (firstDraw)
                {
                    GoOverAll(true);
                    tvMessage.Text = "On these special squares you can step multiple times\n(it's the one with the number 7 on it)";
                    starter = squares[5, 5];
                    starter.StepOn();
                    player.MoveToSquare(starter);
                    checkWin = 7;

                    // path
                    squares[4, 6].UnstepOn();
                    squares[2, 6].UnstepOn();
                    squares[1, 5].UnstepOn();
                    squares[1, 3].UnstepOn();
                    squares[2, 2].UnstepOn();
                    squares[4, 2].UnstepOn();
                    squares[5, 3].UnstepOn();

                    squares[3, 4] = new MultipleStepSquare(squares[3,4]);

                    for (int i = 0; i < 7; i++)
                        squares[3, 4].UnstepOn();

                    (squares[3, 4] as MultipleStepSquare).UnstepOnFinal();

                    firstDraw = false;
                    Invalidate();
                }
                if (checkWin == 0)
                {
                    ResetBoardNextPhase("Amazing!");
                }
            }
            else if(phase == 5)
            {
                if (firstDraw)
                {
                    GoOverAll(false);
                    tvMessage.Text = "Knight's tour fills the board! Try to solve it!\nIn knight's tour mode the code does it for you ;)\nClick the blue home button below to go back to main menu";
                    starter = squares[0, 0];
                    starter.StepOn();
                    player.MoveToSquare(starter);
                    checkWin = 63;

                    firstDraw = false;
                    Invalidate();
                }
                if (checkWin == 0)
                {
                    ResetBoardNextPhase("Tutorial is Complete!\nSending you to main menu <3");
                    winEvent.Invoke(this, EventArgs.Empty);
                }
            }
        }

        private void ResetBoardNextPhase(string message)
        {
            tvMessage.Text = message;
            if (delay != 0)
            {
                Thread.Sleep(500);
                delay--;
                Invalidate();
            }
            else
            {
                delay = 5;
                phase++;
                firstDraw = true;
                moves.Clear();
                Invalidate();
            }
        }


        public override void GoBack() // undo button
        {
            if (this.moves.Count != 0) // if stack isn't empty - if the player has moved
            {
                if (this.player.GetCurrentSquare() is MultipleStepSquare)
                    (this.player.GetCurrentSquare() as MultipleStepSquare).SetWalkedOn(false);
                else
                {
                    (this.player.GetCurrentSquare() as GhostSquare).ResizeBitmap(false);
                    checkWin++; // checkwin goes up - player still needs to step on the square
                }

                this.player.GetCurrentSquare().UnstepOn();
                this.player.MoveToSquare(this.moves.Pop());
            }
        }


    }
}