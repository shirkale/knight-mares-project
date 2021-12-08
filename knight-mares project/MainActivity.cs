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

namespace knight_mares_project
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        public static string snowtree;
        public static string cuteGhost; // ghost picture
        public static string cuteGhostPurp; // ghost picture purp
        public static string cuteGhostBlue;
        public static string flag;


        Button btnStart;
        // Button btnLvl0, btnLvl1, btnLvl2;
        TextView tvTitle, tvWinMessage;
        ISharedPreferences spHighScore;

        //Dialog chooseLevel;
        Dialog difficultyDialog;

        //difficulty chosen in dialog and sent to gameactivity
        int difficulty;

        BroadcastBattery broadcastBattery;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            // initializing widgets

            btnStart = (Button)FindViewById(Resource.Id.btnStart);
            //btnStart.SetBackgroundColor(Color.ParseColor("#171747"));

            tvTitle = (TextView)FindViewById(Resource.Id.tvTitle);
            tvWinMessage = (TextView)FindViewById(Resource.Id.tvTitle);

            // click functions

            btnStart.Click += BtnStart_Click; // button click that starts the game

            // high score code

            spHighScore = this.GetSharedPreferences("details", FileCreationMode.Private);


            snowtree = Helper.BitmapToBase64(BitmapFactory.DecodeResource(Resources, Resource.Drawable.snowtreesmol)); // tree picture
            cuteGhost = Helper.BitmapToBase64(BitmapFactory.DecodeResource(Resources, Resource.Drawable.cutearmsupghostsmol)); // ghost picture
            cuteGhostPurp = Helper.BitmapToBase64(BitmapFactory.DecodeResource(Resources, Resource.Drawable.cutearmsupghostsmolpurp)); // ghost picture purp
            cuteGhostBlue = Helper.BitmapToBase64(BitmapFactory.DecodeResource(Resources, Resource.Drawable.cutearmsupghostsmolblue));
            flag = Helper.BitmapToBase64(BitmapFactory.DecodeResource(Resources, Resource.Drawable.little_red_flag));

            this.difficulty = 15;

            broadcastBattery = new BroadcastBattery();
        }

        protected override void OnResume()
        {
            base.OnResume();

        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menuDif, menu);
            MenuBuilder m = (MenuBuilder)menu;
            m.SetOptionalIconsVisible(true);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            base.OnOptionsItemSelected(item);
            if(item.ItemId == Resource.Id.difficulty)
            {
                DifficultyDialog();
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
        }

        private void BtnSubmit_Click(object sender, System.EventArgs e)
        {
            SeekBar sb = (SeekBar)difficultyDialog.FindViewById(Resource.Id.sbDifficulty);
            sb.Max = 30;
            sb.Min = 3;
            sb.ProgressChanged += Sb_ProgressChanged;
            difficultyDialog.Dismiss();
        }

        private void Sb_ProgressChanged(object sender, SeekBar.ProgressChangedEventArgs e)
        {
            this.difficulty = e.Progress;

        }

        private void BtnStart_Click(object sender, System.EventArgs e)
        {
            /* different level dialog
            // creating dialog for level selection
            chooseLevel = new Dialog(this);
            chooseLevel.SetContentView(Resource.Layout.choose_level_view);
            chooseLevel.SetCancelable(false);

            // assigning buttons

            btnLvl0 = (Button)chooseLevel.FindViewById(Resource.Id.btnLvl0);
            btnLvl1 = (Button)chooseLevel.FindViewById(Resource.Id.btnLvl1);
            btnLvl2 = (Button)chooseLevel.FindViewById(Resource.Id.btnLvl2);

            chooseLevel.Show();

            // click events for dialog buttons (level selection)

            btnLvl0.Click += BtnLvl_Click;
            btnLvl1.Click += BtnLvl_Click;
            btnLvl2.Click += BtnLvl_Click;
            */
            Intent i = new Intent(this, typeof(GameActivity));
            StartActivityForResult(i, this.difficulty);
        }


        private void BtnLvl_Click(object sender, System.EventArgs e)
        {
            // hide level selection dialog
            //if (chooseLevel != null)
                //chooseLevel.Hide();

            // getting level from button which was clicked
            Button b = (Button)sender;
            string lvlStr = b.Tag.ToString();
            int lvl = int.Parse(lvlStr);

            // creating intent
            Intent i = new Intent(this, typeof(MainActivity));

            // sending the lvl to the next screen in order to build the correct board
            i.PutExtra("level", lvl);

            // starting activity
            StartActivityForResult(i, lvl);
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (resultCode == Result.Ok)
            {
                int timecompleted = data.GetIntExtra("time", 0);
                string str = CheckHighScoreInLevel(requestCode, timecompleted);

                tvTitle.Visibility = ViewStates.Gone;

                tvWinMessage.TextSize = 20;
                tvWinMessage.Gravity = GravityFlags.Center;
                tvWinMessage.Text = str;
                tvWinMessage.Visibility = ViewStates.Visible;
                btnStart.Text = "play again?";
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

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}