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
    class MultipleStepSquare : Square
    {
        private int steps; // the amount of steps you can step on the square
        public MultipleStepSquare(int steps, float x, float y, float w, float h, int i, int j, Context context) : base(x, y, w, h, i, j, context)
        {
            this.steps = steps;
            this.p = new Paint();
            this.p.Color = Color.YellowGreen;
            this.p.TextSize = 30;
        }

        public override void StepOn(string bitmap)
        {
            if (this.steps == 0)
            {
                base.StepOn(bitmap);
            }
            else
            {
                steps--;
                this.visibility = false;
            }
        }

        public override void Draw(Canvas canvas)
        {
            if(this.steps == 0 || !visibility)
            {
                base.Draw(canvas);
            }
            else
            {
                canvas.DrawText("" + this.steps, this.x, this.y, this.p);
            }
        }
    }
}