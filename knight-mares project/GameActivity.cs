﻿using Android.App;
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
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here

            SetContentView(Resource.Layout.board_view);

            // initializing widgets

            this.flGame = (FrameLayout)FindViewById(Resource.Id.flBoard);
            //this.tvTime = (TextView)FindViewById(Resource.Id.tvTime);

            // initializing board according to level selection
            int levelSelected = Intent.GetIntExtra("level", 8);
            int boardSize = levelSelected;

            difficulty = 5;

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
            /*won = false;
            time = 0;
            ThreadStart timerstart = new ThreadStart(TimerFunc);
            Thread timer = new Thread(timerstart);
            timer.Start();*/
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
            won = true;
            AlertDialog.Builder builder = new AlertDialog.Builder(this);
            builder.SetTitle("You Win!!! Hooray!!!");
            builder.SetMessage("You have accumulated " + difficulty + " points during the game\nGo back to main menu!");
            builder.SetCancelable(false);
            builder.SetPositiveButton("take me there!", OkAction);
            AlertDialog dialog = builder.Create();
            dialog.Show();
        }

        private void OkAction(object sender, DialogClickEventArgs e)
        {
            Intent i = new Intent();
            i.PutExtra("score", difficulty);
            SetResult(Result.Ok, i);
            Finish();
        }


    }
}