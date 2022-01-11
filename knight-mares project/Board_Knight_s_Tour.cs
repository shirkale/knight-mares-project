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
        bool won;
        public Board_Knight_s_Tour(Context context, int size) : base(context, size, 0)
        {
            this.checkWin = size * size - 1;
            this.solution = new List<Square>();
            won = false;
        }
        protected override void OnDraw(Canvas canvas)
        {
            if (checkWin > 0)
            {
                InitializeBoard(canvas);
                if (firstKnight)
                    InitializeKnight();

                if (firstDraw)
                {
                    firstDraw = false;
                    this.player.GetCurrentSquare().StepOn(MainActivity.flag);
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

        private void FindTour(Square curSquare)
        {
            if (won)
            { }
            else
            {
                if (solution.Count == size * size)
                {
                    Console.WriteLine("win");
                    won = true;
                }
                else if (Stuck(curSquare))
                {
                    solution[solution.Count - 1].UnstepOn();
                    Console.WriteLine("stuck " + curSquare.GetI() + " " + curSquare.GetJ());
                    solution.Remove(solution[solution.Count - 1]);
                }
                else
                {
                    Console.WriteLine("not stuck " + curSquare.GetI() + " " + curSquare.GetJ());
                    for (int k = 0; k < 8; k++)
                    {
                        int newX = curSquare.GetI() + xMove[k];
                        int newY = curSquare.GetJ() + yMove[k];
                        if (EdgeCheck(newX, newY) && !squares[newX, newY].IsWalkedOver())
                        {
                            Square newSquare = this.squares[newX, newY];
                            newSquare.StepOn(MainActivity.flag);
                            solution.Add(newSquare);
                            Console.WriteLine(newSquare.GetI() + ", " + newSquare.GetJ());
                            FindTour(newSquare);
                        }
                    }
                    if(!won)
                    {
                        Console.WriteLine("going back from " + curSquare.GetI() + " " + curSquare.GetJ());
                        solution[solution.Count - 1].UnstepOn();
                        solution.RemoveAt(solution.Count - 1);
                    }
                }
            }
        }

        private bool Stuck(Square curSquare)
        {
            for (int i = 0; i < 8; i++)
            {
                int newX = curSquare.GetI() + xMove[i];
                int newY = curSquare.GetJ() + yMove[i];
                
                if (EdgeCheck(newX, newY) && !squares[newX, newY].IsWalkedOver())
                {
                    return false;
                }
            }
            return true;
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