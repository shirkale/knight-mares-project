using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Graphics;

namespace knight_mares_project
{
    public class Knight
    {
        private Square curSquare; // the square the knight is currently on

        private Bitmap bitmap; // picture of the horse
        private Context context;
        private bool isKnightResized; // knight bitmap needs to be resized once only

        public Knight(Square starter, Context context)
        {
            this.curSquare = starter;
            this.context = context;

            this.bitmap = BitmapFactory.DecodeResource(this.context.Resources, Resource.Drawable.knightpic);
            this.bitmap = this.bitmap.Copy(Bitmap.Config.Argb8888, true); // turning the bitmap mutable

            this.isKnightResized = false;
        }

        public void Draw(Canvas canvas)
        {
            int paddingRight = (int)(this.curSquare.GetW() * 0.2); // padding for the knight figure in the tiles
            int paddingTop = (int)(this.curSquare.GetH() * 0.2);

            int x = (int)this.curSquare.GetX() + paddingRight;
            int y = (int)this.curSquare.GetY() + paddingTop;

            if(!isKnightResized)  // resizing the bitmap
            {
                int w = (int)this.curSquare.GetW() - paddingRight;
                int h = (int)this.curSquare.GetH() - paddingTop;

                this.bitmap = Bitmap.CreateScaledBitmap(this.bitmap, w, h, true);

                this.isKnightResized = true;
            }

            canvas.DrawBitmap(this.bitmap, x, y, null);

        }

        public void moveToSquare(Square s) // moves the knight to the square inputted
        {
            this.curSquare = s;
            this.curSquare.StepOn();
        }

        public Square GetCurrentSquare()
        {
            return this.curSquare;
        }

    }
}