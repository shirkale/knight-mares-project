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
using System.Threading;

namespace knight_mares_project
{
    [BroadcastReceiver]
    public class MusicPlayerBroadcastReceiver : BroadcastReceiver
    {
        MediaPlayer mp;
        public MusicPlayerBroadcastReceiver() { }
        public MusicPlayerBroadcastReceiver(MediaPlayer mp)
        {
            this.mp = mp;
            mp.Looping = true;
        }

        public override void OnReceive(Context context, Intent intent)
        {
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