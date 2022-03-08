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
    public class Board_Knight_s_Tour : Board_Generate
    {
        List<Square> solution; // list into which a path will be inserted
        int count; // for debugging delete when done
        int index; // for running the simulation solution comp calculates

        public static bool solve = false;

        public EventHandler pauseBgMusic;

        List<List<int>> allPaths;
        public Board_Knight_s_Tour(Context context, int size) : base(context, size, 0)
        {
            this.checkWin = size * size;
            this.solution = new List<Square>();
            count = 0;
            index = 0;

            allPaths = FileHelper.DeSerializeNow<List<List<int>>>(); // will be null if file not initialized
        }

        protected override void OnDraw(Canvas canvas)
        {
            if (checkWin > 0)
            {
                InitializeBoard(canvas);
                if (firstKnight)
                    InitializeKnight();

                this.player.Draw(canvas);

                if (firstDraw)
                {
                    firstDraw = false;
                    this.player.GetCurrentSquare().StepOn();
                    SolveTour();

                    UnstepAll();
                    this.player.moveToSquare(solution[0]);
                    solution[0].StepOn();
                }

                if(solve)
                {
                    if (index < solution.Count)
                    {
                        if (index == 0)
                        {
                            UnstepAll();
                            pauseBgMusic.Invoke(this, EventArgs.Empty);
                        }
                        this.player.moveToSquare(solution[index]);
                        Thread.Sleep(TimeSpan.FromMilliseconds(700-index*10));
                        this.player.GetCurrentSquare().StepOn();
                        index++;
                        Invalidate();
                    }
                    if(index == solution.Count)
                    {
                        winEvent.Invoke(this, EventArgs.Empty);
                        index++;
                        this.player.mpMove.PlaybackParams.SetPitch(Knight.defaultMpPitch);
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
            GoBackAll();
            if (allPaths == null) // initializing file one time only
            {
                List<List<int>> writeToFile = new List<List<int>>();
                for (int k = 0; k < size; k++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        UnstepAll();
                        this.solution = new List<Square>();
                        this.count = 0;
                        Console.WriteLine("-----------------------------" + squares[k, j].GetI() + " " + squares[k, j].GetJ() + "---------------------------------");
                        squares[k, j].StepOn();
                        FindTour(squares[k, j]);
                        writeToFile.Add(FileHelper.IntFromSquarePath(this.solution));
                    }
                }
                FileHelper.SerializeNow(writeToFile);
            }
            allPaths = FileHelper.DeSerializeNow<List<List<int>>>(); // reading all paths from file computed beforehand

            int starterPath = this.starter.GetI() * size + this.starter.GetJ(); // enumerates the starter path with one int instead of two
            List<int> intPath = allPaths[starterPath];
            List<Square> testList = FileHelper.SquarePathFromInt(intPath, squares);
            //FindTour(this.starter);

            Console.WriteLine("solution count: " + solution.Count);

            for (int i = 0; i < this.solution.Count; i++)
            {
                Console.WriteLine(this.solution[i].GetI() + ", " + this.solution[i].GetJ());
                // check if correct
                //if (this.solution[i] != testList[i])
                //{
                //    Console.WriteLine("==================WRONG==================");
                //}
            }

            this.solution = testList;
        }

        private bool FindTour(Square curSquare)
        {
            if (count == 0)
                solution.Add(curSquare);
            count++;
            Console.WriteLine(count);
            if (solution.Count == this.checkWin)
                return true;
            else
            {
                Console.WriteLine("trying " + curSquare.GetI() + " " + curSquare.GetJ());

                List<Square> toSort = new List<Square>();

                for (int i = 0; i < 8; i++)
                {
                    int newX = curSquare.GetI() + xMove[i];
                    int newY = curSquare.GetJ() + yMove[i];
                    if (EdgeCheck(newX, newY) && !squares[newX, newY].IsWalkedOver())
                    {
                        GhostSquareForKnightsTour squareToSort = (GhostSquareForKnightsTour)this.squares[newX, newY];
                        if (toSort.Count == 0)
                            toSort.Add(squareToSort);
                        else if (squareToSort.GetNumPossibleMoves() <= 3)
                        {
                            toSort.Insert(0, squareToSort);
                            Console.WriteLine("****going down****");
                            squareToSort.StepOn();
                            solution.Add(squareToSort);
                            if (FindTour(squareToSort))
                                return true;
                            else
                                toSort.RemoveAt(0);
                        }
                        else if (squareToSort.GetNumPossibleMoves() == 8)
                        {
                            toSort.Add(squareToSort);
                        }
                        else
                        {
                            bool added = false;
                            for (int j = 0; j < toSort.Count; j++)
                            {
                                GhostSquareForKnightsTour testsquare = (GhostSquareForKnightsTour)toSort[j];
                                if (testsquare.GetNumPossibleMoves() >= squareToSort.GetNumPossibleMoves())
                                {
                                    toSort.Insert(j, squareToSort);
                                    added = true;
                                    j = toSort.Count;
                                }
                            }
                            if(!added)
                            {
                                toSort.Add(squareToSort);
                            }
                        }
                    }
                }

                if (toSort.Count == 0) // stuck, no way to go
                {
                    solution[solution.Count - 1].UnstepOn();
                    Console.WriteLine("stuck " + solution[solution.Count - 1].GetI() + " " + solution[solution.Count - 1].GetJ());
                    solution.RemoveAt(solution.Count - 1);
                    return false;
                }

                foreach(Square newSquare in toSort)
                {
                    Console.WriteLine("****going down****");
                    newSquare.StepOn();
                    solution.Add(newSquare);
                    if (FindTour(newSquare))
                        return true;
                }
                solution[solution.Count - 1].UnstepOn();
                solution.RemoveAt(solution.Count - 1);
                return false; // couldn't find knight's tour
            }
        }


        private void GoBackAll()
        {
            while (this.moves.Count > 0)
            {
                this.GoBack();
            }
        }

        private void UnstepAll()
        {
            for (int i = 0; i < this.size; i++)
            {
                for (int j = 0; j < this.size; j++)
                {
                    squares[i, j].UnstepOn();
                }
            }
        }
    }
}