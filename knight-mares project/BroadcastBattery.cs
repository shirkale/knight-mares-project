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
        public BroadcastBattery()
        {
        }
        public override void OnReceive(Context context, Intent intent)
        {
            int battery = intent.GetIntExtra("level", 0);

            if(battery < 15)
            {
                Toast.MakeText(context, "Your battery is getting low, how about saving the game just in case?", ToastLength.Short).Show();
            }
            else
            {
                Toast.MakeText(context, "battery good", ToastLength.Short).Show();
            }

        }
    }
}