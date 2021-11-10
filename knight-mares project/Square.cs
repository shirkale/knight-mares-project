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
    public class Square
    {
        private float x, y, w, h;
        private bool walkedOver;

        private string bitmap;
        private bool isBitmapResized; // if false on draw, the bitmap will be resized
        private int i, j; // square id in matrix

        private bool visibility; // if the knight is on the square, visibility will be false

        private Paint p; // paint is used when knight stands on square, hiding the image with visibility

        private Context context;
        public Square(float x, float y, float w, float h, string bitmap, int i, int j, Context context)
        {
            this.x = x;
            this.y = y;
            this.w = w;
            this.h = h;
            this.bitmap = bitmap;
            this.walkedOver = false;

            this.i = i;
            this.j = j;

            isBitmapResized = false;

            this.context = context;
            visibility = true;

            this.p = new Paint();
            this.p.Color = Color.ParseColor("#014760");
        }

        public void Draw(Canvas canvas)
        {
            if (!isBitmapResized)  // resizing the bitmap
            {
                Bitmap toDraw = Bitmap.CreateScaledBitmap(Helper.Base64ToBitmap(this.bitmap), (int)this.w, (int)this.h, true);
                this.bitmap = Helper.BitmapToBase64(toDraw);
                this.isBitmapResized = true;
            }
            if (visibility)
                canvas.DrawBitmap(Helper.Base64ToBitmap(this.bitmap), this.x, this.y, null);
            else
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

        public void StepOn() // makes knight unable to step on square
        {
            this.walkedOver = true;
            this.bitmap = Helper.BitmapToBase64(BitmapFactory.DecodeResource(this.context.Resources, Resource.Drawable.little_red_flag));
            this.isBitmapResized = false;
        }

        public void UnstepOn() // makes knight able to step on square
        {
            this.walkedOver = false;
            this.bitmap = Helper.BitmapToBase64(BitmapFactory.DecodeResource(this.context.Resources, Resource.Drawable.cutearmsupghostsmol));
            //this.p.Color = Color.White;
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