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
using System.IO;
using Android.Content.Res;
using System.Runtime.Serialization.Formatters.Binary;

namespace knight_mares_project
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true, Icon = "@drawable/homebutton4", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class MainActivity : AppCompatActivity
    {
        public static Bitmap snowtree;
        public static Bitmap cuteGhost; // ghost picture
        public static Bitmap cuteGhostPurp; // ghost picture purp
        public static Bitmap cuteGhostBlue;
        public static Bitmap flag;
        public static Bitmap knight;

        public static Intent musicIntent;

        public static List<List<int>> knightTourPaths = null;

        Button btnStart;
        TextView tvTitle, tvDisplayDifficulty, tvHighScore, tvWorldScore;

        ISharedPreferences spHighScore;

        private void SetTvTitleText(string value)
        {
            tvTitle.StartAnimation(changeTitleFadeOutAnimation);

            tvTitle.Text = value;
            if (value.Contains('\n'))
            {
                tvTitle.TextSize = 20;
            }
            else
            {
                tvTitle.TextSize = 30;
            }
            tvTitle.StartAnimation(changeTitleFadeInAnimation);

        }

        //Dialog chooseLevel;
        Dialog difficultyDialog;
        SeekBar sb;
        TextView tvDifficultyInDialog;


        //difficulty chosen in dialog and sent to gameactivity
        int _difficulty;
        private int difficulty
        {
            get
            {
                return _difficulty;
            }

            set
            {
                //#3
                _difficulty = value;
                OnDifChanged();
            }
        }

        protected void OnDifChanged()
        {
            if(tvDisplayDifficulty != null)
                tvDisplayDifficulty.Text = "DIFFICULTY: " + difficulty;
        }


        // leaderboard
        LinearLayout llLeaderBoard;
        TextView tvLbSmall, tvLb, tvLbBig;

        // world leaderboard
        LinearLayout llWorldBoard;
        TextView tvWbSmall, tvWb, tvWbBig;

        // animation
        ImageView ivGoblet;
        ImageView ivPumpkin;
        Animation pumpkinAnimation;
        Animation fromHomeAnimation;
        Animation toLeaderAnimation;
        Animation toHomeAnimation;
        Animation fromLeaderAnimation;
        Animation changeTitleFadeOutAnimation;
        Animation changeTitleFadeInAnimation;

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
            SetContentView(Resource.Layout.activity_main_save);


            // initializing widgets

            btnStart = (Button)FindViewById(Resource.Id.btnStart);
            tvHighScore = (TextView)FindViewById(Resource.Id.tvHighScore);
            tvWorldScore = (TextView)FindViewById(Resource.Id.tvWorldScore);

            tvDisplayDifficulty = (TextView)FindViewById(Resource.Id.tvDisplayDifficulty);
            tvTitle = (TextView)FindViewById(Resource.Id.tvTitle);

            tvLbSmall = (TextView)FindViewById(Resource.Id.tv1);
            tvLb = (TextView)FindViewById(Resource.Id.tv2);
            tvLbBig = (TextView)FindViewById(Resource.Id.tv3);
            llLeaderBoard = (LinearLayout)FindViewById(Resource.Id.linLeader);

            tvWbSmall = (TextView)FindViewById(Resource.Id.tv1w);
            tvWb = (TextView)FindViewById(Resource.Id.tv2w);
            tvWbBig = (TextView)FindViewById(Resource.Id.tv3w);
            llWorldBoard = (LinearLayout)FindViewById(Resource.Id.linWorld);

            ivPumpkin = (ImageView)FindViewById(Resource.Id.ivPumpkin);
            ivGoblet = (ImageView)FindViewById(Resource.Id.ivGoblet);

            bottomnv = FindViewById<BottomNavigationView>(Resource.Id.bottom_navigation);


            // high score code

            spHighScore = this.GetSharedPreferences("details", FileCreationMode.Private);

            this.difficulty = 15;





            snowtree = BitmapFactory.DecodeResource(Resources, Resource.Drawable.snowtreesmol); // tree picture
            cuteGhost = BitmapFactory.DecodeResource(Resources, Resource.Drawable.cutearmsupghostsmol); // ghost picture
            cuteGhostPurp = BitmapFactory.DecodeResource(Resources, Resource.Drawable.cutearmsupghostsmolpurp); // ghost picture purp
            cuteGhostBlue = BitmapFactory.DecodeResource(Resources, Resource.Drawable.cutearmsupghostsmolblue);
            flag = BitmapFactory.DecodeResource(Resources, Resource.Drawable.little_red_flag);
            if(knight == null)
                knight = BitmapFactory.DecodeResource(Resources, Resource.Drawable.knightpic);



            // music player

            musicIntent = new Intent(this, typeof(MusicService));
            StartService(musicIntent);

            // animation init

            pumpkinAnimation = AnimationUtils.LoadAnimation(this, Resource.Animation.pumpkinAnimation);
            fromHomeAnimation = AnimationUtils.LoadAnimation(this, Resource.Animation.fromHome);
            toLeaderAnimation = AnimationUtils.LoadAnimation(this, Resource.Animation.toLeader);
            toHomeAnimation = AnimationUtils.LoadAnimation(this, Resource.Animation.toHome);
            fromLeaderAnimation = AnimationUtils.LoadAnimation(this, Resource.Animation.fromLeader);
            changeTitleFadeOutAnimation = AnimationUtils.LoadAnimation(this, Resource.Animation.changeTitleFadeOut);
            changeTitleFadeInAnimation = AnimationUtils.LoadAnimation(this, Resource.Animation.changeTitleFadeIn);
            ivPumpkin.StartAnimation(pumpkinAnimation);
            ivPumpkin.Animation.AnimationEnd += Animation_AnimationEnd;

            // typgame init

            if(typegame == null)
                typegame = TypeGame.Generate;



            //initialization of firebase database

            //List<int> l = new List<int>();
            //for (int i = 0; i < 30; i++)
            //{
            //    l.Add(-1);
            //}
            //ScoreList s = new ScoreList(l);
            //FirebaseHelper.Add(s);

            GetDatabase();

            // click functions

            btnStart.Click += BtnStart_Click; // button click that starts the game
            bottomnv.NavigationItemSelected += Bottomnv_NavigationItemSelected;

            tvLbSmall.Click += TvLbSmall_Click;
            tvLbBig.Click += TvLbBig_Click;

            tvWbSmall.Click += TvLbSmall_Click;
            tvWbBig.Click += TvLbBig_Click;


            // read path file
            if (knightTourPaths == null)
            {
                const int maxReadSize = 256 * 1024;
                byte[] content;
                AssetManager assets = this.Assets;
                using (BinaryReader br = new BinaryReader(assets.Open("listofpaths.bin")))
                {
                    content = br.ReadBytes(maxReadSize);
                }
                BinaryFormatter bf = new BinaryFormatter();
                using (MemoryStream ms = new MemoryStream(content))
                {
                    object obj = bf.Deserialize(ms);
                    knightTourPaths = (List<List<int>>)obj;
                }
            }
        }


        // show leaderboard views
        public void ShowLeader()
        {
            // scale animation to hide widgets in home
            btnStart.StartAnimation(fromHomeAnimation);
            tvDisplayDifficulty.StartAnimation(fromHomeAnimation);


            // visibility gone in home
            btnStart.Visibility = ViewStates.Gone;
            tvDisplayDifficulty.Visibility = ViewStates.Gone;


            ivPumpkin.ClearAnimation();
            ivPumpkin.StartAnimation(fromHomeAnimation);
            ivPumpkin.Animation.AnimationEnd -= Animation_AnimationEnd;
            ivPumpkin.Visibility = ViewStates.Gone;


            SetTvTitleText("Leaderboards"); // fade out fade in

            // visibility visable in leaderboard
            llLeaderBoard.Visibility = ViewStates.Visible;
            llWorldBoard.Visibility = ViewStates.Visible;
            ivGoblet.Visibility = ViewStates.Visible;
            tvHighScore.Visibility = ViewStates.Visible;
            tvWorldScore.Visibility = ViewStates.Visible;

            // scale animation to show widgets in leaderboard
            llLeaderBoard.StartAnimation(toLeaderAnimation);
            llWorldBoard.StartAnimation(toLeaderAnimation);
            ivGoblet.StartAnimation(toLeaderAnimation);
            tvHighScore.StartAnimation(toLeaderAnimation);
            tvWorldScore.StartAnimation(toLeaderAnimation);

            SetLeaderboardText();
        }

        // show home views
        public void ShowHome(bool animate)
        {
            // visibility visable in home
            tvDisplayDifficulty.Visibility = ViewStates.Visible;
            btnStart.Visibility = ViewStates.Visible;
            ivPumpkin.Visibility = ViewStates.Visible;

            // scale animation to show widgets in home
            if (animate)
            {
                tvDisplayDifficulty.StartAnimation(toHomeAnimation);
                btnStart.StartAnimation(toHomeAnimation);
                ivPumpkin.StartAnimation(toHomeAnimation);
            }
            else
            {
                ivPumpkin.StartAnimation(pumpkinAnimation);
            }
            ivPumpkin.Animation.AnimationEnd += Animation_AnimationEnd;

            SetTvTitleText("Knight-Mares"); // fade out fade in

            // visibility gone in leaderboard
            llLeaderBoard.Visibility = ViewStates.Gone;
            llWorldBoard.Visibility = ViewStates.Gone;
            ivGoblet.Visibility = ViewStates.Gone;
            tvHighScore.Visibility = ViewStates.Gone;
            tvWorldScore.Visibility = ViewStates.Gone;

            // scale animation to show widgets in leaderboard
            if (animate)
            {
                llLeaderBoard.StartAnimation(fromLeaderAnimation);
                llWorldBoard.StartAnimation(fromLeaderAnimation);
                ivGoblet.StartAnimation(fromLeaderAnimation);
                tvHighScore.StartAnimation(fromLeaderAnimation);
                tvWorldScore.StartAnimation(fromLeaderAnimation);
            }
        }

        // bottom nav menu
        private void Bottomnv_NavigationItemSelected(object sender, BottomNavigationView.NavigationItemSelectedEventArgs e)
        {
            if (btnStart.Animation == null || !btnStart.Animation.HasStarted || btnStart.Animation.HasEnded)
            {
                if (e.Item.ItemId == Resource.Id.itemLeader)
                {
                    if (btnStart.Visibility == ViewStates.Visible)
                    {
                        ShowLeader();
                        btnStart.Clickable = false;
                        tvWbBig.Clickable = true;
                        tvWbSmall.Clickable = true;
                    }
                }
                else if (e.Item.ItemId == Resource.Id.itemhome)
                {
                    if (btnStart.Visibility == ViewStates.Gone)
                    {
                        ShowHome(true);
                        btnStart.Clickable = true;
                        tvWbBig.Clickable = false;
                        tvWbSmall.Clickable = true;
                    }
                }
            }
        }

        // get database from firebase
        public async Task<List<int>> GetDatabase()
        {
            List<ScoreList> getScore = await FirebaseHelper.GetAll();
            if (getScore == null)
                return null;
            ScoreList s = getScore[0];
            worldRecord = s.listOfScores;
            return worldRecord;
        }


        // leaderboard clicks
        private void TvLbBig_Click(object sender, EventArgs e)
        {
            if (difficulty != 30 && llLeaderBoard.Visibility != ViewStates.Gone)
            {
                difficulty++;
                SetLeaderboardText();
            }

        }

        private void TvLbSmall_Click(object sender, EventArgs e)
        {
            if (difficulty != 3 && llLeaderBoard.Visibility != ViewStates.Gone)
            {
                difficulty--;
                SetLeaderboardText();
            }
        }

        // loop animation
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

            try
            {
                s1 = "" + (level - 1) + '\n';
                if (worldRecord[level - 4] == -1)
                    s1 += "---";
                else
                    s1 += "" + worldRecord[level - 4] + " seconds";

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

                tvWbSmall.Text = s1;
                tvWb.Text = s2;
                tvWbBig.Text = s3;
            }
            catch
            {
                tvWbSmall.Text = "error";
                tvWb.Text = "error";
                tvWbBig.Text = "error";
            }
            
        }

        protected override void OnResume()
        {
            base.OnResume();
            if (!muted)
            {
                if (!MusicService.musicInit)
                {
                    musicIntent = new Intent(this, typeof(MusicService));
                    StartService(musicIntent);
                }
                else
                {
                    SendAction(1);
                }
            }
            else
            {
                if (!MusicService.musicInit)
                {
                    musicIntent = new Intent(this, typeof(MusicService));
                    StartService(musicIntent);
                }
                else
                {
                    SendAction(0);
                }
            }
        }

        protected override void OnPause()
        {
            if (!MusicService.musicInit)
            {
                Intent musicIntent = new Intent(this, typeof(MusicService));
                StartService(musicIntent);
            }
            SendAction(0);
            base.OnPause();
        }



        // menu code
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            if (menu is MenuBuilder) (menu as MenuBuilder).SetOptionalIconsVisible(true);


            MenuInflater.Inflate(Resource.Menu.camera_and_mute_menu, menu);
            return base.OnCreateOptionsMenu(menu);
        }
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            base.OnOptionsItemSelected(item);
            if (item.ItemId == Resource.Id.mute)
            {
                if (muted)
                {
                    if(!MusicService.musicInit)
                    {
                        musicIntent = new Intent(this, typeof(MusicService));
                        StartService(musicIntent);
                    }
                    else
                    {
                        SendAction(1);
                    }
                    item.SetIcon(Resource.Drawable.mute);
                }
                else
                {
                    if (!MusicService.musicInit)
                    {
                        musicIntent = new Intent(this, typeof(MusicService));
                        StartService(musicIntent);
                    }
                    SendAction(0);
                    item.SetIcon(Resource.Drawable.sound);
                }
                muted = !muted;
            }
            else if(item.ItemId == Resource.Id.changePic)
            {
                Intent i = new Intent(Android.Provider.MediaStore.ActionImageCapture);
                StartActivityForResult(i, 100);
            }
            return true;
        }

        // dialog opened on select gamemode and difficulty in menu
        public void DifficultyDialog()
        {
            this.difficultyDialog = new Dialog(this);
            difficultyDialog.SetContentView(Resource.Layout.difficulty_dialog2);
            difficultyDialog.SetTitle("Difficulty");
            difficultyDialog.SetCancelable(true);

            Button btnSubmit = (Button)difficultyDialog.FindViewById(Resource.Id.btnSubmit);
            RadioButton rbPath = (RadioButton)difficultyDialog.FindViewById(Resource.Id.rbPath);
            RadioButton rbTour = (RadioButton)difficultyDialog.FindViewById(Resource.Id.rbTour);
            RadioButton rbTutorial = (RadioButton)difficultyDialog.FindViewById(Resource.Id.rbTutorial);
            RadioGroup rgMode = (RadioGroup)difficultyDialog.FindViewById(Resource.Id.rgGamemode);
            sb = (SeekBar)difficultyDialog.FindViewById(Resource.Id.sbDifficulty);
            tvDifficultyInDialog = (TextView)difficultyDialog.FindViewById(Resource.Id.displayDifficulty);

            sb.Max = 30;
            sb.Min = 3;
            sb.Progress = this.difficulty;
            tvDifficultyInDialog.Text = "" + this.difficulty;


            rgMode.CheckedChange += RgMode_CheckedChange;
            switch(typegame)
            {
                case TypeGame.Generate:
                    rbPath.Checked = true;
                    sb.Visibility = ViewStates.Visible;
                    tvDifficultyInDialog.Visibility = ViewStates.Visible;
                    break;
                case TypeGame.Tour:
                    rbTour.Checked = true;
                    sb.Visibility = ViewStates.Invisible;
                    tvDifficultyInDialog.Visibility = ViewStates.Invisible;
                    break;
                case TypeGame.Tutorial:
                    rbTutorial.Checked = true;
                    sb.Visibility = ViewStates.Invisible;
                    tvDifficultyInDialog.Visibility = ViewStates.Invisible;
                    break;
                default:
                    rbPath.Checked = true;
                    sb.Visibility = ViewStates.Visible;
                    tvDifficultyInDialog.Visibility = ViewStates.Visible;
                    break;
            }

            btnSubmit.Click += BtnSubmit_Click;
            sb.ProgressChanged += Sb_ProgressChanged;

            difficultyDialog.Show();
        }

        private void ChangeMode(ViewStates difVisibility, TypeGame curTypeGame)
        {
            sb.Visibility = difVisibility;
            tvDifficultyInDialog.Visibility = difVisibility;
            tvDisplayDifficulty.Visibility = difVisibility;
            this.typegame = curTypeGame;
        }

        private void RgMode_CheckedChange(object sender, RadioGroup.CheckedChangeEventArgs e)
        {
            switch(e.CheckedId)
            {
                case Resource.Id.rbPath:
                    ChangeMode(ViewStates.Visible, TypeGame.Generate);
                    break;
                case Resource.Id.rbTour:
                    ChangeMode(ViewStates.Invisible, TypeGame.Tour);
                    break;
                case Resource.Id.rbTutorial:
                    ChangeMode(ViewStates.Invisible, TypeGame.Tutorial);
                    break;
                default:
                    ChangeMode(ViewStates.Visible, TypeGame.Generate);
                    break;
            }
        }

        // submit difficulty in dialog
        private void BtnSubmit_Click(object sender, EventArgs e)
        {
            difficultyDialog.Dismiss();
            if (difficulty < 3)
                difficulty = 3;
            else if (difficulty > 30)
                difficulty = 30;

            if (this.typegame == null)
                this.typegame = TypeGame.Generate;

            Intent i = new Intent(this, typeof(GameActivity));
            i.PutExtra("level", difficulty);
            i.PutExtra("type", (int)this.typegame);
            StartActivityForResult(i, this.difficulty);
        }
        // seek bar in dialog
        private void Sb_ProgressChanged(object sender, SeekBar.ProgressChangedEventArgs e)
        {
            this.difficulty = e.Progress;
            tvDifficultyInDialog.Text = "" + this.difficulty;
        }
        // start generate
        private void BtnStart_Click(object sender, EventArgs e)
        {
            if (btnStart.Visibility != ViewStates.Gone)
            {
                DifficultyDialog();
            }
        }

        // comes back from game
        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (resultCode == Result.Ok)
            {
                ShowHome(false);
                if (requestCode == 0)
                {
                    SetTvTitleText("KNIGHTS TOUR COMPLETE");
                    Board_Knight_s_Tour.solve = false;
                    this.typegame = TypeGame.Tour;
                }
                else if(requestCode == 100)
                {
                    knight = (Bitmap)data.Extras.Get("data");
                }
                else if(requestCode == 101)
                {
                    SetTvTitleText("Knight-Mares");
                    this.typegame = TypeGame.Tutorial;
                }
                else
                {
                    int timecompleted = data.GetIntExtra("time", 0);
                    SetTvTitleText(CheckHighScoreInLevel(requestCode, timecompleted));
                    this.typegame = TypeGame.Generate;
                }


                tvTitle.Gravity = GravityFlags.Center;
                tvTitle.Visibility = ViewStates.Visible;
            }
        }
        // returns win string according to high scores
        private string CheckHighScoreInLevel(int requestCode, int time) // updates high score and returns string to display
        {
            string str = ":::Knight-Mares:::\nYou completed the level in " + time + " seconds.";

            string level = "level" + requestCode;

            if (time <= spHighScore.GetInt(level, time)) // checking high score in this level
            {
                str += "\nYou currently played your fastest time for this level!\nCONGRATS!";
                var editor = spHighScore.Edit();
                editor.PutInt(level, time);
                editor.Commit();
            }
            if (worldRecord == null)
            {
                GetDatabase();
            }
            try
            {
                if (worldRecord[requestCode - 3] == -1 || time <= worldRecord[requestCode - 3]) // checking high score from world records
                {
                    str += "\nWorld Record!!!!";
                    worldRecord[requestCode - 3] = time;
                    FirebaseHelper.Update(new ScoreList(worldRecord));
                }
            }
            catch
            {
                str += "\ncouldn't find database";
            }
                
            return str;
        }

        public void SendAction(int action) // 1 to turn on music, 0 to turn off
        {
            Intent i = new Intent("music");
            i.PutExtra("action", action);
            SendBroadcast(i);
        }

        public void ResumeMusic()
        {
            if(muted)
            {
                SendAction(1);
            }
        }

        public void PauseMusic()
        {
            if (!MusicService.musicInit)
            {
                musicIntent = new Intent(this, typeof(MusicService));
                StartService(musicIntent);
            }
            SendAction(0);
        }


        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}