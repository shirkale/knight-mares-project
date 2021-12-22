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

namespace knight_mares_project
{
    [BroadcastReceiver(Enabled = true)]
    [IntentFilter(new[] { Intent.ActionBatteryChanged })]
    public class BroadcastBattery : BroadcastReceiver
    {
        TextView tv;
        public BroadcastBattery()
        {
        }
        public BroadcastBattery(TextView tv)
        {
            this.tv = tv;
        }
        public override void OnReceive(Context context, Intent intent)
        {
            int battery = intent.GetIntExtra("level", 0);
            Toast.MakeText(context, battery, ToastLength.Short).Show();
            if (battery < 15)
            {
                Toast.MakeText(context, "Your battery is getting low, how about saving the game just in case?", ToastLength.Short).Show();
            }

        }
    }
}