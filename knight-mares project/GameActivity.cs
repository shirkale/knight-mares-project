using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Views.Animations;
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

        int time, result; // time counts the time, result saves the time when win:avoiding mistakenly counted seconds
        bool won;

        ImageButton btnBack;
        ImageButton btnHome;

        //Animations

        Animation animTurnUndo;


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here

            SetContentView(Resource.Layout.board_view);

            // initializing widgets

            this.flGame = (FrameLayout)FindViewById(Resource.Id.flBoard);
            this.btnBack = (ImageButton)FindViewById(Resource.Id.btnReturn);
            this.btnHome = (ImageButton)FindViewById(Resource.Id.btnBackToMainActivity);

            this.tvTime = (TextView)FindViewById(Resource.Id.tvTime);

            // initializing board according to level selection

            difficulty = Intent.GetIntExtra("level", 3);

            this.game = new Board(this, 8, difficulty);

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

            // initializing animations

            animTurnUndo = AnimationUtils.LoadAnimation(this, Resource.Animation.undobuttonturn);

            btnBack.Click += BtnBack_Click;
            btnHome.Click += BtnHome_Click;

        }

        private void BtnHome_Click(object sender, EventArgs e)
        {
            Finish();
        }

        private void BtnBack_Click(object sender, EventArgs e)
        {
            Thread animateBackButton = new Thread(new ThreadStart(AnimateBackButton));
            animateBackButton.Start();
            this.game.GoBack();
            this.game.Invalidate();
            
        }

        private void AnimateBackButton()
        {
            RunOnUiThread(() => { this.btnBack.StartAnimation(animTurnUndo); }); 
        }

        private void TimerFunc()
        {
            this.tvTime = (TextView)FindViewById(Resource.Id.tvTime);
            while (!won)
            {
                Thread.Sleep(999);
                time++;
                RunOnUiThread(() => {this.tvTime.Text = "Time: " + time;} ); // runs the change in view on the main thread
            }
        }


        public void WinDialog(object s, EventArgs args)
        {
            this.result = time;
            string msg = string.Format("You have defeated all the ghosts in difficulty {0} in {1} seconds!", difficulty, result);
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
            i.PutExtra("time", this.result);
            SetResult(Result.Ok, i);
            Finish();
        }

        protected override void OnResume()
        {
            base.OnResume();
            ResumeMusic();
        }

        protected override void OnPause()
        {
            PauseMusic();
            base.OnPause();
        }

        public void ResumeMusic() // move to mainactivity
        { 
            if(!MainActivity.muted)
            {
                Intent i = new Intent("music");
                i.PutExtra("action", 1); // 1 to turn on
                SendBroadcast(i);
            }
        }

        public void PauseMusic() // move to main
        {
            Intent i = new Intent("music");
            i.PutExtra("action", 0); // 0 to turn on
            SendBroadcast(i);
        }

    }
}