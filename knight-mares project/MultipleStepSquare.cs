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
    public class MultipleStepSquare : Square
    {
        private int steps; // the amount of steps you can step on the square
        private Paint numP; // paint for the number on the multstepsquare
        private char endChar; // x to put at end of path - when can't be stepped on
        public MultipleStepSquare(float x, float y, float w, float h, int i, int j, Context context, int size) : base(x, y, w, h, i, j, context)
        {
            this.steps = 0;
            this.numP = new Paint();
            this.numP.Color = Color.YellowGreen;
            this.numP.TextSize = 30;
            this.endChar = 'x';
        }

        public MultipleStepSquare(Square s) : base(s)
        {
            this.steps = 0;
            this.numP = new Paint();
            this.numP.Color = Color.YellowGreen;
            this.numP.TextSize = 30;
        }

        public override void StepOn()
        {
            if (this.steps == 0)
            {
                base.StepOn();
            }
            else
            {
                steps--;
                this.visibility = false;
                if (this.steps == 0)
                    base.StepOn();
            }
        }

        public override void UnstepOn()
        {
            this.steps++;
        }

        public void UnstepOnFinal()
        {
            this.walkedOver = false;
            this.visibility = true;
        }

        public override void Draw(Canvas canvas)
        {
            base.Draw(canvas);
            if(visibility)
            {
                if(this.steps == 0)
                    canvas.DrawText("" + this.endChar, this.x + this.w / 2, this.y + this.h / 2, this.numP);
                else
                    canvas.DrawText("" + this.steps, this.x + this.w / 2, this.y + this.h / 2, this.numP);
            }
        }


        public int GetSteps()
        {
            return this.steps;
        }
    }
}