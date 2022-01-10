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
    public class Board_Knight_s_Tour : Board_Generate
    {
        List<Square> solution;
        public Board_Knight_s_Tour(Context context, int size) : base(context, size, 0)
        {
            this.checkWin = size * size - 1;
            this.solution = new List<Square>();
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
            FindTour(this.starter);
        }

        private List<Square> FindTour(Square curSquare)
        {
            if (checkWin == 0)
                return this.solution;
            else if (Stuck(curSquare))
            {
                solution.Remove(solution[solution.Count]);
                return this.solution;
            }
            else
            {
                int i = curSquare.GetI();
                int j = curSquare.GetJ();
                for(int k = 0; k < 8; k++)
                {
                    Square newSquare = this.squares[i + xMove[k], j + yMove[k]];
                    solution.Add(newSquare);
                    FindTour(newSquare);
                }
                return this.solution;
            }
        }

        private bool Stuck(Square curSquare)
        {
            for (int i = 0; i < 8; i++)
            {
                int newX = curSquare.GetI() + xMove[i];
                int newY = curSquare.GetJ() + xMove[i];
                
                if (EdgeCheck(newX, newY) && squares[newX, newY].IsWalkedOver())
                {
                    return true;
                }
            }
            return false;
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