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
using System.Threading;

namespace knight_mares_project
{
    [Activity(Label = "GameActivity")]
    public class GameActivity : Activity
    {
        FrameLayout flGame;
        Board game;

        TextView tvTime;

        int difficulty;

        int time;
        bool won;

        ImageButton btnBack;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here

            SetContentView(Resource.Layout.board_view);

            // initializing widgets

            this.flGame = (FrameLayout)FindViewById(Resource.Id.flBoard);
            this.btnBack = (ImageButton)FindViewById(Resource.Id.btnReturn);

            this.tvTime = (TextView)FindViewById(Resource.Id.tvTime);

            // initializing board according to level selection
            int levelSelected = Intent.GetIntExtra("level", 8);
            int boardSize = levelSelected;

            difficulty = 10;

            this.game = new Board(this, boardSize, difficulty);

            // adding the board to the framelayout
            this.flGame.AddView(this.game);


            // events
            this.game.winEvent += WinDialog;

            this.game.updateEvent += (s, args) =>
            {
                // UpdateScore();
            };

            // creating timer
            won = false;
            time = 0;
            ThreadStart timerstart = new ThreadStart(TimerFunc);
            Thread timer = new Thread(timerstart);
            timer.Start();

            btnBack.Click += BtnBack_Click;
        }

        private void BtnBack_Click(object sender, EventArgs e)
        {
            this.game.GoBack();
            this.game.Invalidate();
        }

        private void TimerFunc()
        {
            this.tvTime = (TextView)FindViewById(Resource.Id.tvTime);
            while (!won)
            {
                Thread.Sleep(1000);
                time++;
                RunOnUiThread(() => {this.tvTime.Text = "Time: " + time;} ); // runs the change in view on the main thread
            }
        }


        public void WinDialog(object s, EventArgs args)
        {
            string msg = string.Format("You have defeated all the ghosts in difficulty {0} in {1} seconds!", difficulty, time);
            won = true;
            AlertDialog.Builder builder = new AlertDialog.Builder(this);
            builder.SetTitle("You Win!!! Hooray!!!");
            builder.SetMessage(msg);
            builder.SetCancelable(false);
            builder.SetPositiveButton("take me there!", OkAction);
            AlertDialog dialog = builder.Create();
            dialog.Show();
        }

        private void OkAction(object sender, DialogClickEventArgs e)
        {
            Intent i = new Intent();
            i.PutExtra("time", this.time);
            SetResult(Result.Ok, i);
            Finish();
        }


    }
}