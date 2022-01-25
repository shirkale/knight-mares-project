﻿using Android.App;
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

        public Board_Knight_s_Tour(Context context, int size) : base(context, size, 0)
        {
            this.checkWin = size * size;
            this.solution = new List<Square>();
            count = 0;
            index = 1;
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
                    this.player.GetCurrentSquare().StepOn(MainActivity.flag);
                    SolveTour();

                    UnstepAll();
                    this.player.moveToSquare(solution[0]);
                    solution[0].StepOn(MainActivity.flag);
                }

                if(index < solution.Count)
                {
                    this.player.moveToSquare(solution[index]);
                    this.player.GetCurrentSquare().StepOn(MainActivity.flag);
                    index++;
                    Thread.Sleep(500);
                    Invalidate();
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
            solution.Add(starter);
            FindTour(this.starter);

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
                        Square squareToSort = this.squares[newX, newY];
                        if (toSort.Count == 0)
                            toSort.Add(squareToSort);
                        else if (squareToSort.GetNumPossibleMoves() <= 3)
                        {
                            toSort.Insert(0, squareToSort);
                            Console.WriteLine("****going down****");
                            squareToSort.StepOn(MainActivity.flag);
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

                foreach(Square newSquare in toSort)
                {
                    Console.WriteLine("****going down****");
                    newSquare.StepOn(MainActivity.flag);
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