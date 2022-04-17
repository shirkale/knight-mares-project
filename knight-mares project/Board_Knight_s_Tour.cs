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
using System.Threading;

namespace knight_mares_project
{
    public class Board_Knight_s_Tour : Board
    {
        List<Square> solution; // list into which a path will be inserted
        int index; // for running the simulation solution comp calculates

        public static bool solve = false;

        public EventHandler pauseBgMusic; // pause music during solve

        public Board_Knight_s_Tour(Context context, int size) : base(context, size)
        {
            this.checkWin = size * size - 1;
            this.solution = new List<Square>();
            count = 0;
            index = 0;
        }

        public override void GoBack()
        {
            if (this.moves.Count != 0) // if stack isn't empty - if the player has moved
            {
                this.player.GetCurrentSquare().UnstepOn();
                this.player.moveToSquare(this.moves.Pop());
            }
        }

        protected override void OnDraw(Canvas canvas)
        {
            if (checkWin > 0) // when checkWin is 0 player wins, goes over all squares
            {
                InitializeBoard(canvas);
                if (firstKnight)
                {
                    PickRandomStarter();
                    InitializeKnight();
                }

                this.player.Draw(canvas);

                if (firstDraw)
                {
                    firstDraw = false;
                    this.player.GetCurrentSquare().StepOn();
                    SolveTour();

                    GoOverAll(false);
                    this.player.moveToSquare(this.solution[0]);
                    this.player.GetCurrentSquare().StepOn();
                }

                if(solve) // when solve button is pressed the knight moves on the board according to the solution
                {
                    if (index < solution.Count)
                    {
                        if (index == 0) // initialize solve
                        {
                            GoOverAll(false);
                            this.player.moveToSquare(solution[index]);
                            this.player.GetCurrentSquare().StepOn();
                            pauseBgMusic.Invoke(this, EventArgs.Empty); // pause background music, hear steps
                        }
                        else
                        {
                            this.player.moveToSquare(solution[index]);
                            Thread.Sleep(TimeSpan.FromMilliseconds(700 - index * 10));
                            this.player.GetCurrentSquare().StepOn();
                        }
                        index++;
                        Invalidate();
                    }
                    if(index == solution.Count)
                    {
                        index++;
                        winEvent.Invoke(this, EventArgs.Empty);
                    }
                }

            }
            else
            {
                winEvent.Invoke(this, EventArgs.Empty);
            }
        }

        public void SolveTour()
        {
            GoBackAll(); // backtracking any moves the player has made

            int starterPath = this.starter.GetI() * size + this.starter.GetJ(); // enumerates the starter path with one int instead of two
            List<int> intPath = MainActivity.knightTourPaths[starterPath]; // path of ints, each pair represents 1 square
            this.solution = SquarePathFromInt(intPath, squares);

        }
        public static List<Square> SquarePathFromInt(List<int> source, Square[,] squareMatrix)
        {
            if (source != null)
            {
                List<Square> newList = new List<Square>();
                for (int k = 0; k < source.Count - 1; k += 2)
                {
                    int i = k;
                    int j = k + 1;
                    newList.Add(squareMatrix[source[i], source[j]]);
                }
                return newList;
            }
            return null;
        }

        private void GoBackAll()
        {
            while (this.moves.Count > 0)
            {
                this.GoBack();
            }
        }
    }
}