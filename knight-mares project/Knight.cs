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
            this.curSquare.SetImageVisability(false);
            //this.curSquare.StepOn(MainActivity.flag);
        }

        public void Draw(Canvas canvas)
        {
            int padding = (int)(this.curSquare.GetW() * 0.01); // padding for the knight figure in the tiles

            int x = (int)this.curSquare.GetX() + padding;
            int y = (int)this.curSquare.GetY() + padding;

            if(!isKnightResized)  // resizing the bitmap
            {
                int w = (int)this.curSquare.GetW() - 2 * padding;
                int h = (int)this.curSquare.GetH() - 2 * padding;

                this.bitmap = Bitmap.CreateScaledBitmap(this.bitmap, w, h, true);

                this.isKnightResized = true;
            }


            canvas.DrawBitmap(this.bitmap, x, y, null);

        }

        public void moveToSquare(Square s)
            // moves the knight to the square inputted
        {
            this.curSquare.SetImageVisability(true);
            this.curSquare = s;
            this.curSquare.SetImageVisability(false);
        }

        public Square GetCurrentSquare()
        {
            return this.curSquare;
        }

        internal void SetCurrentSquare(Square square)
        {
            this.curSquare = square;
        }
    }
}