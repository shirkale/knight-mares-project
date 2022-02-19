﻿using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using AndroidX.AppCompat.App;
using Android.Graphics;
using Android.Views;
using System;
using AndroidX.AppCompat.View.Menu;
using Android.Views.Animations;
using Android.Media;
using System.Collections.Generic;
using static Android.Resource;
using Animation = Android.Views.Animations.Animation;
using System.Threading.Tasks;
using Android.Support.Design.Widget;

namespace knight_mares_project
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true, Icon = "@drawable/homebutton4")]
    public class MainActivity : AppCompatActivity
    {
        public static Bitmap snowtree;
        public static Bitmap cuteGhost; // ghost picture
        public static Bitmap cuteGhostPurp; // ghost picture purp
        public static Bitmap cuteGhostBlue;
        public static Bitmap flag;

        public static Intent musicIntent;

        Button btnStart, btnTour;
        TextView tvTitle, tvDisplayDifficulty;
        ISharedPreferences spHighScore;

        //Dialog chooseLevel;
        Dialog difficultyDialog;
        SeekBar sb;
        TextView tvDifficultyInDialog;

        //difficulty chosen in dialog and sent to gameactivity
        int difficulty;


        // leaderboard
        LinearLayout llLeaderBoard;
        TextView tvLbSmall, tvLb, tvLbBig;

        // world leaderboard
        LinearLayout wlLeaderboard;
        TextView tvWLbSmall, tvWLb, tvWLbBig;

        // animation
        ImageView ivGoblet;
        ImageView ivPumpkin;
        Animation pumpkinAnimation;

        // sound

        public static bool muted = false;

        // TypeGame

        private TypeGame typegame;

        List<int> worldRecord; // list into which world record is put from firebase

        // bottom menu
        BottomNavigationView bottomnv;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            this.difficulty = 15;

            // initializing widgets

            btnStart = (Button)FindViewById(Resource.Id.btnStart);
            btnTour = (Button)FindViewById(Resource.Id.btnKnightsTour);
            tvDisplayDifficulty = (TextView)FindViewById(Resource.Id.tvDisplayDifficulty);
            tvTitle = (TextView)FindViewById(Resource.Id.tvTitle);

            tvLbSmall = (TextView)FindViewById(Resource.Id.tv1);
            tvLb = (TextView)FindViewById(Resource.Id.tv2);
            tvLbBig = (TextView)FindViewById(Resource.Id.tv3);
            llLeaderBoard = (LinearLayout)FindViewById(Resource.Id.linLeader);

            tvWLbSmall = (TextView)FindViewById(Resource.Id.tv1w);
            tvWLb = (TextView)FindViewById(Resource.Id.tv2w);
            tvWLbBig = (TextView)FindViewById(Resource.Id.tv3w);
            wlLeaderboard = (LinearLayout)FindViewById(Resource.Id.linWorld);

            ivPumpkin = (ImageView)FindViewById(Resource.Id.ivPumpkin);
            ivGoblet = (ImageView)FindViewById(Resource.Id.ivGoblet);

            bottomnv = FindViewById<BottomNavigationView>(Resource.Id.bottom_navigation);


            // high score code

            spHighScore = this.GetSharedPreferences("details", FileCreationMode.Private);


            

            

            snowtree = BitmapFactory.DecodeResource(Resources, Resource.Drawable.snowtreesmol); // tree picture
            cuteGhost = BitmapFactory.DecodeResource(Resources, Resource.Drawable.cutearmsupghostsmol); // ghost picture
            cuteGhostPurp = BitmapFactory.DecodeResource(Resources, Resource.Drawable.cutearmsupghostsmolpurp); // ghost picture purp
            cuteGhostBlue = BitmapFactory.DecodeResource(Resources, Resource.Drawable.cutearmsupghostsmolblue);
            flag = BitmapFactory.DecodeResource(Resources, Resource.Drawable.little_red_flag);

            tvDisplayDifficulty.Text = "DIFFICULTY: " + this.difficulty;



            // music player

            musicIntent = new Intent(this, typeof(MyService));
            StartService(musicIntent);

            // animation init

            pumpkinAnimation = AnimationUtils.LoadAnimation(this, Resource.Animation.pumpkinAnimation);
            ivPumpkin.StartAnimation(pumpkinAnimation);
            ivPumpkin.Animation.AnimationEnd += Animation_AnimationEnd;

            tvLbSmall.Click += Tv1_Click;
            tvLbBig.Click += TvLbBig_Click;

            // typgame init

            typegame = TypeGame.Generate;



            // initialization of firebase database

            //List<int> l = new List<int>();
            //for (int i = 0; i < 30; i++)
            //{
            //    l.Add(-1);
            //}
            //Score s = new Score(l);
            //FirebaseHelper.Add(s);
            
            GetDatabase();

            // click functions

            btnStart.Click += BtnStart_Click; // button click that starts the game

            btnTour.Click += BtnTour_Click;
            bottomnv.NavigationItemSelected += Bottomnv_NavigationItemSelected;

        }

        private void Bottomnv_NavigationItemSelected(object sender, BottomNavigationView.NavigationItemSelectedEventArgs e)
        {
            if(e.Item.ItemId == Resource.Id.itemLeader)
            {
                tvDisplayDifficulty.Visibility = ViewStates.Gone;
                btnStart.Visibility = ViewStates.Gone;
                btnTour.Visibility = ViewStates.Gone;
                ivPumpkin.ClearAnimation();
                ivPumpkin.Visibility = ViewStates.Gone;

                tvTitle.Text = "Leaderboards";
                llLeaderBoard.Visibility = ViewStates.Visible;
                wlLeaderboard.Visibility = ViewStates.Visible;
                ivGoblet.Visibility = ViewStates.Visible;


                SetLeaderboardText();
            }
            else if (e.Item.ItemId == Resource.Id.itemhome)
            {
                tvDisplayDifficulty.Visibility = ViewStates.Visible;
                btnStart.Visibility = ViewStates.Visible;
                btnTour.Visibility = ViewStates.Visible;
                ivPumpkin.Visibility = ViewStates.Visible;
                ivPumpkin.StartAnimation(pumpkinAnimation);
                ivPumpkin.Animation.AnimationEnd += Animation_AnimationEnd;

                tvTitle.Text = "Kight-Mares";
                llLeaderBoard.Visibility = ViewStates.Gone;
                wlLeaderboard.Visibility = ViewStates.Gone;
                ivGoblet.Visibility = ViewStates.Gone;
            }
        }

        public async Task<List<int>> GetDatabase()
        {
            List<Score> getScore = await FirebaseHelper.GetAll();
            Score s = getScore[0];
            worldRecord = s.l;
            return worldRecord;
        }

        private void BtnTour_Click(object sender, EventArgs e)
        {
            this.typegame = TypeGame.Tour;
            Intent i = new Intent(this, typeof(GameActivity));
            i.PutExtra("type", (char)this.typegame);
            StartActivityForResult(i, 0);
        }

        private void TvLbBig_Click(object sender, EventArgs e)
        {
            if (difficulty != 29)
            {
                difficulty++;
                SetLeaderboardText();
            }

        }

        private void Tv1_Click(object sender, EventArgs e)
        {
            if (difficulty != 4)
            {
                difficulty--;
                SetLeaderboardText();
            }
        }

        private void Animation_AnimationEnd(object sender, Animation.AnimationEndEventArgs e)
        {
            if(ivPumpkin.Visibility == ViewStates.Visible)
                ivPumpkin.StartAnimation(pumpkinAnimation);
        }

        private async void SetLeaderboardText() // display leaderboards
        {
            // personal records
            string s1 = "", s2 = "", s3 = "";
            int level = difficulty;

            if (difficulty == 3)
                level = 4;
            if (difficulty == 30)
                level = 29;

            s1 = "" + (level - 1) + '\n';
            if (spHighScore.GetInt("level" + (level - 1), 0) == 0)
                s1 += "---";
            else
                s1 += "" + spHighScore.GetInt("level" + (level - 1), 0) + " seconds";

            s2 = "" + (level) + '\n';
            if (spHighScore.GetInt("level" + (level), 0) == 0)
                s2 += "---";
            else
                s2 += "" + spHighScore.GetInt("level" + (level), 0) + " seconds";

            s3 = "" + (level + 1) + '\n';
            if (spHighScore.GetInt("level" + (level + 1), 0) == 0)
                s3 += "---";
            else
                s3 += "" + spHighScore.GetInt("level" + (level + 1), 0) + " seconds";

            tvLbSmall.Text = s1;
            tvLb.Text = s2;
            tvLbBig.Text = s3;

            // world records

            s1 = "";
            s2 = "";
            s3 = "";

            if (difficulty == 3)
                level = 4;
            if (difficulty == 30)
                level = 29;

            if(worldRecord == null)
                await GetDatabase();

            s1 = "" + (level - 1) + '\n';
            if (worldRecord[level-4] == -1)
                s1 += "---";
            else
                s1 += "" + worldRecord[level-4] + " seconds";

            s2 = "" + (level) + '\n';
            if (worldRecord[level - 3] == -1)
                s2 += "---";
            else
                s2 += "" + worldRecord[level - 3] + " seconds";

            s3 = "" + (level + 1) + '\n';
            if (worldRecord[level - 2] == -1)
                s3 += "---";
            else
                s3 += "" + worldRecord[level - 2] + " seconds";

            tvWLbSmall.Text = s1;
            tvWLb.Text = s2;
            tvWLbBig.Text = s3;
            
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



        // menu code
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuBuilder m = (MenuBuilder)menu;
            m.SetOptionalIconsVisible(true);
            MenuInflater.Inflate(Resource.Menu.menuDif, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            base.OnOptionsItemSelected(item);
            if (item.ItemId == Resource.Id.difficulty)
            {
                DifficultyDialog();
            }
            else if (item.ItemId == Resource.Id.mute)
            {
                if (muted)
                { 
                    muted = !muted;
                    ResumeMusic();
                    item.SetIcon(Resource.Drawable.mute);
                }
                else
                {
                    muted = !muted;
                    PauseMusic();
                    item.SetIcon(Resource.Drawable.sound);
                }
            }
            return true;
        }

        public void DifficultyDialog()
        {
            this.difficultyDialog = new Dialog(this);
            difficultyDialog.SetContentView(Resource.Layout.difficulty_dialog);
            difficultyDialog.SetTitle("Difficulty");
            difficultyDialog.SetCancelable(true);
            Button btnSubmit = (Button)difficultyDialog.FindViewById(Resource.Id.btnSubmit);
            difficultyDialog.Show();
            btnSubmit.Click += BtnSubmit_Click;


            sb = (SeekBar)difficultyDialog.FindViewById(Resource.Id.sbDifficulty);
            sb.Max = 30;
            sb.Min = 3;
            sb.Progress = this.difficulty;
            tvDifficultyInDialog = (TextView)difficultyDialog.FindViewById(Resource.Id.displayDifficulty);
            tvDifficultyInDialog.Text = "" + this.difficulty;
            sb.ProgressChanged += Sb_ProgressChanged;

        }

        private void BtnSubmit_Click(object sender, EventArgs e)
        {
            difficultyDialog.Dismiss();
        }

        private void Sb_ProgressChanged(object sender, SeekBar.ProgressChangedEventArgs e)
        {
            this.difficulty = e.Progress;
            tvDifficultyInDialog.Text = "" + this.difficulty;
            tvDisplayDifficulty.Text = "DIFFICULTY: " + this.difficulty;
            SetLeaderboardText();
        }

        private void BtnStart_Click(object sender, EventArgs e)
        {
            this.typegame = TypeGame.Generate;
            Intent i = new Intent(this, typeof(GameActivity));
            i.PutExtra("level", difficulty);
            i.PutExtra("type", (char)this.typegame);
            StartActivityForResult(i, this.difficulty);
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (resultCode == Result.Ok)
            {
                if (requestCode == 0)
                {
                    tvTitle.Text = "KNIGHTS TOUR COMPLETE";
                    Board_Knight_s_Tour.solve = false;
                }
                else
                {
                    int timecompleted = data.GetIntExtra("time", 0);
                    tvTitle.Text = CheckHighScoreInLevel(requestCode, timecompleted);
                }


                tvTitle.Gravity = GravityFlags.Center;
                tvTitle.Visibility = ViewStates.Visible;

                llLeaderBoard.Visibility = ViewStates.Invisible;
                wlLeaderboard.Visibility = ViewStates.Invisible;
                
            }
        }

        private string CheckHighScoreInLevel(int requestCode, int time) // updates high score and returns string to display
        {
            string str = "You completed the level in " + time + " seconds.";

            string level = "level" + requestCode;

            if (time <= spHighScore.GetInt(level, time)) // checking high score in this level
            {
                str += "\nYou currently played your fastest time for this level!\nCONGRATS!";
                var editor = spHighScore.Edit();
                editor.PutInt(level, time);
                editor.Commit();
            }

            if(worldRecord[requestCode - 3] == -1 || time <= worldRecord[requestCode - 3]) // checking high score from world records
            {
                str += "\nWorld Record!!!!";
                worldRecord[requestCode - 3] = time;
                FirebaseHelper.Update(new Score(worldRecord));
            }
            return str;
        }


        public void ResumeMusic() // move to mainactivity
        {
            if(!muted)
            {
                if (MyService.musicStopped)
                {
                    MyService.musicStopped = false;
                    musicIntent = new Intent(this, typeof(MyService));
                    StartService(musicIntent);
                }
                else
                {
                    Intent i = new Intent("music");
                    i.PutExtra("action", 1); // 1 to turn on
                    SendBroadcast(i);
                }
            }
        }

        public void PauseMusic() // move to main
        {
            Intent i = new Intent("music");
            i.PutExtra("action", 0); // 0 to turn on
            SendBroadcast(i);
        }


        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}