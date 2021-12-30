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
    public class MusicPlayerBroadcastReciever : BroadcastReceiver
    {
        MediaPlayer mp;
        Thread t;
        public MusicPlayerBroadcastReciever()
        {
        }
        public MusicPlayerBroadcastReciever(MediaPlayer mp)
        {
            this.mp = mp;
            mp.Looping = true;
        }

        private void CountDownTilMusicStopped()
        {
            for (int i = 0; i < 10; i++)
            {
                Thread.Sleep(1000);
            }

            MyService.musicStopped = true;
        }

        public override void OnReceive(Context context, Intent intent)
        {
            //Toast.MakeText(context, "Received intent!", ToastLength.Short).Show();
            int action = intent.GetIntExtra("action", 0);
            if(action == 1)
            {
                mp.Start();
                mp.SetVolume((float)0.4, (float)0.4);

                if(t!=null && t.IsAlive)
                    t.Abort();
            }
            else if(action == 0)
            {
                mp.Pause();

                t = new Thread(CountDownTilMusicStopped);
                t.Start();
            }
        }

    }
}