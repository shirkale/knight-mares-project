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
        int count;

        public Board_Knight_s_Tour(Context context, int size) : base(context, size, 0)
        {
            this.checkWin = size * size - 1;
            this.solution = new List<Square>();
            count = 0;
        }
        protected override void OnDraw(Canvas canvas)
        {
            if (checkWin > 0)
            {
                InitializeBoard(canvas);
                //if (firstKnight)
                //    InitializeKnight();

                if (firstDraw)
                {
                    firstDraw = false;
                    //this.player.GetCurrentSquare().StepOn(MainActivity.flag);
                    this.starter = this.squares[0, 3];
                    this.starter.StepOn(MainActivity.flag);
                    SolveTour();
                }
            }
            else
            {
                winEvent.Invoke(this, EventArgs.Empty);
            }
        }

        public void SolveTour()
        {
            UnstepOnAll();
            solution.Add(starter);
            FindTour(this.starter);
            //for (int i = 1; i < this.solution.Count; i++)
            //{
            //    this.player.moveToSquare(this.solution[i]);
            //    Thread.Sleep(1000);
            //}

            Console.WriteLine("solution count: " + solution.Count);

            for (int i = 0; i < this.solution.Count; i++)
            {
                Console.WriteLine(this.solution[i].GetI() + ", " + this.solution[i].GetJ());
            }
        }

        private bool FindTour(Square curSquare)
        {
            count++;
            Console.WriteLine(count);
            if (solution.Count == size * size)
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
                        Square squareToSort = this.squares[newX, newY];
                        if (toSort.Count == 0)
                            toSort.Add(squareToSort);
                        else if (squareToSort.GetNumPossibleMoves() <= 3)
                        {
                            toSort.Insert(0, squareToSort);
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
                                if (toSort[j].GetNumPossibleMoves() >= squareToSort.GetNumPossibleMoves())
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

                for (int k = 0; k < toSort.Count; k++)
                {
                    Console.WriteLine("****going down****");
                    Square newSquare = toSort[k];
                    newSquare.StepOn(MainActivity.flag);
                    solution.Add(newSquare);
                    if (FindTour(newSquare))
                        return true;
                    //else
                    //{
                    //    solution[solution.Count - 1].UnstepOn();
                    //    solution.RemoveAt(solution.Count - 1);
                    //}
                }
                solution[solution.Count - 1].UnstepOn();
                solution.RemoveAt(solution.Count - 1);
                return false; // couldn't find knight's tour
            }
        }


        private void UnstepOnAll()
        {
            while (this.moves.Count > 0)
            {
                this.GoBack();
            }
        }
    }
}