using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using AndroidX.AppCompat.App;
using Android.Graphics;
using Android.Views;

namespace knight_mares_project
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {

        Button btnStart;
        // Button btnLvl0, btnLvl1, btnLvl2;
        TextView tvTitle, tvWinMessage;
        ISharedPreferences spHighScore;

        //Dialog chooseLevel;
        Dialog difficultyDialog;
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

            // creating menu


        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menuDif, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            base.OnOptionsItemSelected(item);
            if(item.ItemId == Resource.Id.difficulty)
            {
                LoginDialog();
            }
            return true;
        }

        public void LoginDialog()
        {
            this.difficultyDialog = new Dialog(this);
            difficultyDialog.SetContentView(Resource.Layout.difficulty_dialog);
            difficultyDialog.SetTitle("Login");
            difficultyDialog.SetCancelable(true);
            Button btnSubmit = (Button)difficultyDialog.FindViewById(Resource.Id.btnSubmit);
            difficultyDialog.Show();
            btnSubmit.Click += BtnSubmit_Click;
        }

        private void BtnSubmit_Click(object sender, System.EventArgs e)
        {
            difficultyDialog.Dismiss();
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
            int lvl = 8;
            StartActivityForResult(i, lvl);
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
                int curScore = data.GetIntExtra("score", 0);
                string str = CheckHighScoreInLevel(requestCode, curScore);

                tvTitle.Visibility = Android.Views.ViewStates.Gone;

                tvWinMessage.Text = str;
                tvWinMessage.Visibility = Android.Views.ViewStates.Visible;
                btnStart.Text = "play again?";
            }
        }

        private string CheckHighScoreInLevel(int requestCode, int score) // updates high score and returns string to display
        {
            string str = "Level " + (requestCode + 1) + "\nYou made " + score + " moves.";

            string level = "level" + requestCode;

            if (score <= spHighScore.GetInt(level, score)) // checking high score in this level
            {
                str += "\n\nYou currently have the lowest amount of moves for this level!\nCONGRATS!";
                var editor = spHighScore.Edit();
                editor.PutInt(level, score);
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