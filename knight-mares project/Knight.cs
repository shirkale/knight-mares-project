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
using Android.Media;
using Android.Media.Audiofx;

namespace knight_mares_project
{
    public class Knight
    {
        private Square curSquare; // the square the knight is currently on

        private Bitmap bitmap; // picture of the horse
        private bool isKnightResized; // knight bitmap needs to be resized once only

        public MediaPlayer mpMove; // sound of knight movement

        private Context context; // context in order to create music player

        public Knight(Square starter, Context context)
        {
            this.curSquare = starter;

            this.bitmap = MainActivity.knight;
            this.bitmap = this.bitmap.Copy(Bitmap.Config.Argb8888, true); // turning the bitmap mutable

            this.isKnightResized = false;
            this.curSquare.SetImageVisability(false);

            this.context = context;

            mpMove = MediaPlayer.Create(context, Resource.Raw.move2);
            mpMove.SetVolume(1, 1);

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

        public bool CanMakeJump(Square s)
        {
            if (s.IsWalkedOver())
                return false;
            if (this.curSquare.GetI() + 2 == s.GetI() || this.curSquare.GetI() - 2 == s.GetI())
            {
                if (this.curSquare.GetJ() + 1 == s.GetJ() || this.curSquare.GetJ() - 1 == s.GetJ())
                    return true;
            }
            else if (this.curSquare.GetJ() + 2 == s.GetJ() || this.curSquare.GetJ() - 2 == s.GetJ())
            {
                if (this.curSquare.GetI() + 1 == s.GetI() || this.curSquare.GetI() - 1 == s.GetI())
                    return true;
            }
            return false;
        }


        public void MoveToSquare(Square s)
            // moves the knight to the square inputted
        {
            if(this.mpMove == null)
            {
                mpMove = MediaPlayer.Create(context, Resource.Raw.move2);
            }
            this.mpMove.Start();
                
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