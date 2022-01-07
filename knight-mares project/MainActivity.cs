using Android.App;
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

        Button btnStart, btnLeader;
        TextView tvTitle, tvWinMessage, tvDisplayDifficulty;
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

        // animation
        ImageView ivPumpkin;
        Animation pumpkinAnimation;

        // sound

        public static bool muted = false;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            this.difficulty = 15;

            // initializing widgets

            btnStart = (Button)FindViewById(Resource.Id.btnStart);
            btnLeader = (Button)FindViewById(Resource.Id.btnLeader);
            tvDisplayDifficulty = (TextView)FindViewById(Resource.Id.tvDisplayDifficulty);
            tvTitle = (TextView)FindViewById(Resource.Id.tvTitle);
            tvWinMessage = (TextView)FindViewById(Resource.Id.tvTitle);

            tvLbSmall = (TextView)FindViewById(Resource.Id.tv1);
            tvLb = (TextView)FindViewById(Resource.Id.tv2);
            tvLbBig = (TextView)FindViewById(Resource.Id.tv3);
            llLeaderBoard = (LinearLayout)FindViewById(Resource.Id.linLeader);

            ivPumpkin = (ImageView)FindViewById(Resource.Id.ivPumpkin);

            // high score code

            spHighScore = this.GetSharedPreferences("details", FileCreationMode.Private);


            // click functions

            btnStart.Click += BtnStart_Click; // button click that starts the game

            btnLeader.Click += BtnLeader_Click;
            btnLeader.CallOnClick();
            btnLeader.CallOnClick();


            

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
        }


        private void TvLbBig_Click(object sender, EventArgs e)
        {
            if (difficulty != 29)
            {
                difficulty++;
                BtnLeader_Click(sender, e);
                BtnLeader_Click(sender, e);
            }

        }

        private void Tv1_Click(object sender, EventArgs e)
        {
            if (difficulty != 4)
            {
                difficulty--;
                BtnLeader_Click(sender, e);
                BtnLeader_Click(sender, e);
            }
        }

        private void Animation_AnimationEnd(object sender, Animation.AnimationEndEventArgs e)
        {
            ivPumpkin.StartAnimation(pumpkinAnimation);
        }

        private void BtnLeader_Click(object sender, EventArgs e)
        {
            if(llLeaderBoard.Visibility == ViewStates.Visible)
                llLeaderBoard.Visibility = ViewStates.Invisible;
            else
            {
                llLeaderBoard.Visibility = ViewStates.Visible;
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
            }
            
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


            llLeaderBoard.Visibility = ViewStates.Invisible;
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

        }

        private void BtnStart_Click(object sender, EventArgs e)
        {
            Intent i = new Intent(this, typeof(GameActivity));
            i.PutExtra("level", difficulty);
            StartActivityForResult(i, this.difficulty);
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (resultCode == Result.Ok)
            {
                int timecompleted = data.GetIntExtra("time", 0);
                string str = CheckHighScoreInLevel(requestCode, timecompleted);

                tvTitle.Visibility = ViewStates.Invisible;

                tvWinMessage.TextSize = 20;
                tvWinMessage.Gravity = GravityFlags.Center;
                tvWinMessage.Text = str;
                tvWinMessage.Visibility = ViewStates.Visible;
                btnStart.Text = "play again?";

                llLeaderBoard.Visibility = ViewStates.Invisible;

            }
        }

        private string CheckHighScoreInLevel(int requestCode, int time) // updates high score and returns string to display
        {
            string str = "You completed the level in " + time + " seconds.";

            string level = "level" + requestCode;

            if (time <= spHighScore.GetInt(level, time)) // checking high score in this level
            {
                str += "\nYou currently have the fastest time for this level!\nCONGRATS!";
                var editor = spHighScore.Edit();
                editor.PutInt(level, time);
                editor.Commit();
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