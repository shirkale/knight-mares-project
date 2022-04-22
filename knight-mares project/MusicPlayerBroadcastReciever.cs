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
        Thread threadToCountServiceShutdown; // when service is turned on and music is shut down, the timer starts and shuts down the service at the end
        public MusicPlayerBroadcastReciever() { }
        public MusicPlayerBroadcastReciever(MediaPlayer mp)
        {
            this.mp = mp;
            mp.Looping = true;
        }

        private void CountDownForServiceShutDown()
        {
            for (int i = 0; i < 180; i++)
            {
                Thread.Sleep(1000);
            }

            MusicService.musicInit = false;
        }

        public override void OnReceive(Context context, Intent intent)
        {
            int action = intent.GetIntExtra("action", 0);
            if(action == 1)
            {
                mp.Start();

                if(threadToCountServiceShutdown != null && threadToCountServiceShutdown.IsAlive)
                    threadToCountServiceShutdown.Abort();
            }
            else if(action == 0)
            {
                mp.Pause();

                threadToCountServiceShutdown = new Thread(CountDownForServiceShutDown);
                threadToCountServiceShutdown.Start();
            }
        }

    }
}