using Android.App;
using Android.Content;
using Android.OS;

namespace knight_mares_project
{
    internal class MyHandler : Handler
    {
        Context context;
        Dialog dialog;
        public MyHandler(Context context, Dialog d)
        {
            this.context = context;
            this.dialog = d;
        }
        public override void HandleMessage(Message msg)
        {
            if(msg.Arg1 == 1)
            {
                this.dialog.Show();
            }
            else
            {
                this.dialog.Dismiss();
            }
        }
    }
}