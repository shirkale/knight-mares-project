using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
//using System.Drawing;
using System.Linq;
using System.Text;
using Color = Android.Graphics.Color;
//using Android.Graphics;

namespace knight_mares_project
{
    [Serializable]
    public abstract class Square
    {
        protected float x, y, w, h;
        protected bool walkedOver;

        protected int i, j; // square id in matrix

        protected bool visibility; // if the knight is on the square, visibility will be false

        protected Paint p; // paint is used when knight stands on square, hiding the image with visibility

        protected Context context;


        public Square(float x, float y, float w, float h, int i, int j, Context context)
        {
            this.x = x;
            this.y = y;
            this.w = w;
            this.h = h;
            this.walkedOver = true; // for gen board all squares need to be stepped on init. 

            this.i = i;
            this.j = j;

            this.context = context;
            visibility = true;

            this.p = new Paint(); 
            this.p.Color = Color.ParseColor("#F07E1A");
            this.p.Alpha = 60;


            // https://www.sciencedirect.com/science/article/pii/S0166218X04003488
            // https://bradfieldcs.com/algos/graphs/knights-tour/

        }


        public Square(Square s)
        {
            this.x = s.x;
            this.y = s.y;
            this.w = s.w;
            this.h = s.h;
            this.walkedOver = s.walkedOver;

            this.i = s.i;
            this.j = s.j;


            this.context = s.context;
            this.visibility = s.visibility;
            this.p = s.p;
        }

        public virtual void Draw(Canvas canvas)
        {
            if(!visibility)
                canvas.DrawRect(this.x, this.y, this.x + this.w, this.y + this.h, this.p);
        }

        public bool IsXandYInSquare(float otherX, float otherY)
        {
            return otherX >= this.x && otherX <= this.x + this.w && otherY >= this.y && otherY <= this.y + this.h;
        }

        public bool CanMakeJump(Square s)
        {
            if (s.IsWalkedOver())
                return false;
            if (this.i + 2 == s.i || this.i - 2 == s.i)
            {
                if (this.j + 1 == s.j || this.j - 1 == s.j)
                    return true;
            }
            else if (this.j + 2 == s.j || this.j - 2 == s.j)
            {
                if (this.i + 1 == s.i || this.i - 1 == s.i)
                    return true;
            }
            return false;
        }

        internal void SetImageVisability(bool visibility)
        {
            this.visibility = visibility;
        }

        public float GetX()
        {
            return this.x;
        }
        public float GetY()
        {
            return this.y;
        }
        public float GetW()
        {
            return this.w;
        }
        public float GetH()
        {
            return this.h;
        }
        public bool IsWalkedOver()
        {
            return this.walkedOver;
        }

        public virtual void StepOn() // makes knight unable to step on square
        {
            this.walkedOver = true;
        }

        public virtual void UnstepOn() // makes knight able to step on square
        {
            this.walkedOver = false;
        }

        public int GetI()
        {
            return i;
        }
        public int GetJ()
        {
            return j;
        }

    }
}