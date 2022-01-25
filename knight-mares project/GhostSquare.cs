using Android.App;
using Android.Content;
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
    public class GhostSquare : Square
    {
        public GhostSquare(float x, float y, float w, float h, int i, int j, Context context, int size) : base(x, y, w, h, i, j, context, size)
        {
            this.bitmap = MainActivity.snowtree;
            this.isBitmapResized = false;

            if (i == 0 || i == size - 1)
            {
                if (j == 0 || j == size - 1) // corner : 2
                {
                    numOfPossibleMoves = 2;
                }
                else if (j == 1 || j == size - 2) // around corner : 3
                {
                    numOfPossibleMoves = 3;
                }
                else
                {
                    numOfPossibleMoves = 4; // middle row
                }
            }
            else if (i == 1 || i == size - 2)
            {
                if (j == 0 || j == size - 1) // around corner : 3
                {
                    numOfPossibleMoves = 3;
                }
                else if (j == 1 || j == size - 2) // around corner : 4
                {
                    numOfPossibleMoves = 4;
                }
                else
                {
                    numOfPossibleMoves = 6; // middle row
                }
            }
            else // middle rows
            {
                if (j == 0 || j == size - 1) // edges
                {
                    numOfPossibleMoves = 4;
                }
                else if (j == 1 || j == size - 2) // left and right
                {
                    numOfPossibleMoves = 6;
                }
                else
                {
                    numOfPossibleMoves = 8; // center row
                }
            }


            public int GetNumPossibleMoves()
            {
                return numOfPossibleMoves;
            }
        }
}