using Android.App;
using Android.Content;
using Android.Media;
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
    [BroadcastReceiver]
    public class MusicPlayerBroadcastReciever : BroadcastReceiver
    {
        MediaPlayer mp;
        public MusicPlayerBroadcastReciever()
        {
        }
        public MusicPlayerBroadcastReciever(MediaPlayer mp)
        {
            this.mp = mp;
        }
        public override void OnReceive(Context context, Intent intent)
        {
            Toast.MakeText(context, "Received intent!", ToastLength.Short).Show();
            int action = intent.GetIntExtra("action", 0);
            if(action == 1)
            {
                mp.Start();
            }
            else if(action == 0)
            {
                mp.Pause();
            }
        }
    }
}