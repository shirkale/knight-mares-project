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
    public class GhostSquare : Square
    {
        protected Bitmap bitmap;
        protected bool isBitmapResized; // if false on draw, the bitmap will be resized

        public GhostSquare(float x, float y, float w, float h, int i, int j, Context context, int size) : base(x, y, w, h, i, j, context)
        {
            this.bitmap = MainActivity.snowtree;
            this.isBitmapResized = false;
        }

        public GhostSquare(Square s) : base(s)
        {
            this.bitmap = MainActivity.snowtree;
            this.isBitmapResized = false;
        }

        public override void Draw(Canvas canvas)
        {
            base.Draw(canvas);

            if(visibility)  // when the knight isn't on the square
            {
                if (!isBitmapResized)  // resizing the bitmap
                {
                    Bitmap toDraw = Bitmap.CreateScaledBitmap(this.bitmap, (int)this.w, (int)this.h, true);
                    this.bitmap = toDraw;
                    this.isBitmapResized = true;
                }
                canvas.DrawBitmap(this.bitmap, this.x, this.y, null);
            }
        }

        public void ResizeBitmap(bool resize)
        {
            this.isBitmapResized = resize;
        }

        public override void StepOn()
        {
            base.StepOn();
            this.bitmap = MainActivity.flag;
            this.isBitmapResized = false;
        }

        public override void UnstepOn()
        {
            base.UnstepOn();

            Random random = new Random();
            int ghostCount = random.Next(3);

            if(ghostCount % 3 == 0)
                this.bitmap = MainActivity.cuteGhost;
            else if(ghostCount % 3 == 1)
                this.bitmap = MainActivity.cuteGhostBlue;
            else
                this.bitmap = MainActivity.cuteGhostPurp;
            this.isBitmapResized = false;
        }

        public void SetBitmap(Bitmap bitmap)
        {
            isBitmapResized = false;
            this.bitmap = bitmap;
        }
    }

}