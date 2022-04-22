using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Threading;

namespace knight_mares_project
{
    public class Board_Generate : Board
    {
        private int difficulty; // maximum number of moves the computer simulates

        static int delayCount = 10; // delays winevent so player can see board
        static bool isStarterMms = false;


        public Board_Generate(Context context, int size, int difficulty) : base(context, size)
        {
            this.checkWin = difficulty;
            this.difficulty = difficulty;
        }

        public override void GoBack() // undo button
        {
            if(this.moves.Count != 0) // if stack isn't empty - if the player has moved
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

        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);
            if(checkWin > 0)
            {
                InitializeBoard(canvas);
                if (firstKnight)
                {
                    PickRandomStarter();
                    InitializeKnight();
                }

                if (firstDraw)
                {
                    firstDraw = false;
                    GenerateRandomMap(canvas); // generates map
                    FinalUnStepOnMultSquares(); // makes all multstepsquares stepable, if 1 turn into ghostsquare

                    this.player.MoveToSquare(this.starter);
                    if(isStarterMms)
                        this.starter.StepOn();
                    delayCount = 10;
                }
            }
            else
            {
                if(delayCount > 0)
                {
                    InitializeBoard(canvas);
                    delayCount--;
                    Thread.Sleep(50);
                    Invalidate();
                }
                else
                {
                    winEvent.Invoke(this, EventArgs.Empty);
                }
            }
        }


        private void FinalUnStepOnMultSquares()
        {
            for (int i = 0; i < this.size; i++)
            {
                for (int j = 0; j < this.size; j++)
                {
                    if (this.squares[i, j] is MultipleStepSquare mss)
                    {
                        if(mss.GetSteps() == 1)
                        {
                            this.squares[i, j] = new GhostSquare(this.squares[i, j]);
                            this.squares[i, j].UnstepOn();
                            this.squares[i, j].SetImageVisability(true);
                            checkWin++;
                            isStarterMms = this.squares[i, j] == starter;
                        }
                        else
                        {
                            mss.UnstepOnFinal();
                        }

                    }
                }
            }
        }

        private void GenerateRandomMap(Canvas canvas) // generates map by going backwards and unsteping squares
        {
            int steps = difficulty;

            bool chanceForMultSquare;
            int i, j;

            for (int k = 0; k < steps; k++)
            {
                Square nextSquare = PickRandomNextOpenPosition();
                if(nextSquare != null) // if knight is not stuck
                {
                    if(nextSquare is MultipleStepSquare) // if the generated square is mult
                    {
                        this.checkWin--;
                    }
                    else
                    {
                        chanceForMultSquare = CalculateChance();
                        if (chanceForMultSquare)
                        {
                            i = nextSquare.GetI();
                            j = nextSquare.GetJ();
                            squares[i, j] = new MultipleStepSquare(nextSquare);
                            nextSquare = squares[i, j];
                            // for each square we turn into a multiplestepsquare, we have to take the checkwin down by 1, because multstep squares aren't a necessity for winning
                            this.checkWin--;
                        }
                    }
                    this.player.MoveToSquare(nextSquare);
                    nextSquare.UnstepOn();
                }
                else
                {
                    if (k < steps / 2)
                    {
                        firstDraw = true;
                        checkWin = difficulty;
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
            return random.Next(100) < 20;
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

        protected bool AllTrue(bool[] boolArray)
        {
            for (int i = 0; i < boolArray.Length; i++)
            {
                if (boolArray[i] == false)
                    return false;
            }
            return true;
        }

    }
}