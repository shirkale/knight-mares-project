﻿using Android.App;
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
    [Activity(Label = "GameActivity", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class GameActivity : Activity
    {
        FrameLayout flGame;
        Board game;

        TextView tvTime;

        int difficulty;

        int time, result; // time counts the time, result saves the time when win:avoiding mistakenly counted seconds
        Thread timer;
        bool won;

        ImageButton btnBack;
        ImageButton btnHome;
        Button btnSolve;

        //Animations

        Animation animTurnUndo;

        // tutorial
        TextView tvTutorialInstructions;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here

            SetContentView(Resource.Layout.board_view);

            // initializing widgets

            this.flGame = (FrameLayout)FindViewById(Resource.Id.flBoard);
            this.btnBack = (ImageButton)FindViewById(Resource.Id.btnReturn);
            this.btnHome = (ImageButton)FindViewById(Resource.Id.btnBackToMainActivity);
            this.btnSolve = (Button)FindViewById(Resource.Id.btnSolve);

            this.tvTime = (TextView)FindViewById(Resource.Id.tvTime);
            this.tvTutorialInstructions = (TextView)FindViewById(Resource.Id.tvTutorialInstructions);

            // initializing board according to level selection

            difficulty = Intent.GetIntExtra("level", 3);
            TypeGame type = (TypeGame)Intent.GetIntExtra("type", 0);
            int size = 8;

            switch(type)
            {
                case TypeGame.Generate:
                    this.game = new Board_Generate(this, size, difficulty);
                    this.btnSolve.Visibility = ViewStates.Gone;
                    this.tvTutorialInstructions.Visibility = ViewStates.Gone;
                    break;
                case TypeGame.Tour:
                    this.game = new Board_Knight_s_Tour(this, size);
                    this.btnSolve.Visibility = ViewStates.Visible;
                    this.tvTutorialInstructions.Visibility = ViewStates.Gone;
                    break;
                case TypeGame.Tutorial:
                    this.game = new Board_Tutorial(this, size, tvTutorialInstructions);
                    this.btnSolve.Visibility = ViewStates.Gone;
                    this.tvTutorialInstructions.Visibility = ViewStates.Visible;
                    break;
                default:
                    this.game = new Board_Generate(this, size, difficulty);
                    break;
            }
            

            // adding the board to the framelayout
            this.flGame.AddView(this.game);


            // events
            this.game.winEvent += WinDialog;
            if (this.game is Board_Knight_s_Tour)
            {
                Board_Knight_s_Tour tour = (Board_Knight_s_Tour)this.game;
                tour.pauseBgMusic += PauseMusic;
            }

            // creating timer
            won = false;
            time = 0;
            ThreadStart timerstart = new ThreadStart(TimerFunc);
            timer = new Thread(timerstart);
            timer.Start();

            // initializing animations

            animTurnUndo = AnimationUtils.LoadAnimation(this, Resource.Animation.undobuttonturn);

            btnBack.Click += BtnBack_Click;
            btnHome.Click += BtnHome_Click;
            this.btnSolve.Click += BtnSolve_Click;

        }


        private void BtnSolve_Click(object sender, EventArgs e)
        {
            Board_Knight_s_Tour.solve = true;
            timer.Abort();
            tvTime.Visibility = ViewStates.Gone;
            btnBack.Visibility = ViewStates.Gone;
            btnSolve.Visibility = ViewStates.Gone;
        }

        private void BtnHome_Click(object sender, EventArgs e)
        {
            Board_Knight_s_Tour.solve = false;
            if(timer != null && timer.IsAlive)
                timer.Abort();
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
            string msg = "";
            this.result = time;
            if (this.game is Board_Knight_s_Tour)
                msg = "The Knight's Tour is complete!";
            else if (this.game is Board_Generate)
                msg = string.Format("You have defeated all the ghosts in difficulty {0} in {1} seconds!", difficulty, result);
            else
                msg = "Tutorial Complete!";
            won = true;
            AlertDialog.Builder builder = new AlertDialog.Builder(this);
            builder.SetTitle("Hooray!!!");
            builder.SetMessage(msg);
            builder.SetCancelable(false);
            builder.SetPositiveButton("back to main menu", OkAction);
            AlertDialog dialog = builder.Create();
            dialog.Show();
        }

        private void OkAction(object sender, DialogClickEventArgs e)
        {
            Intent i = new Intent();
            i.PutExtra("time", this.result);
            SetResult(Result.Ok, i);
            Board_Knight_s_Tour.solve = false;
            Finish();
        }

        protected override void OnResume()
        {
            base.OnResume();
            ResumeMusic();
        }

        protected override void OnPause()
        {
            PauseMusic(this, EventArgs.Empty);
            base.OnPause();
        }


        public void ResumeMusic()
        {
            if (!MainActivity.muted)
                SendAction(1);
            else
                PauseMusic(this, EventArgs.Empty);
        }

        public void PauseMusic(object sender, EventArgs eventArgs)
        {
            SendAction(0);
        }

        public void SendAction(int action) // 1 to turn on music, 0 to turn off
        {
            Intent i = new Intent("music");
            i.PutExtra("action", action);
            SendBroadcast(i);
        }
    }
}