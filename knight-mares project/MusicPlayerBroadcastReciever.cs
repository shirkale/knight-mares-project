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
        AudioManager am;
        Thread t;
        public MusicPlayerBroadcastReciever()
        {
        }
        public MusicPlayerBroadcastReciever(MediaPlayer mp)
        {
            this.mp = mp;
            mp.Looping = true;

            

            mp.SetVolume(1, 1);

            t = new Thread(Run);
        }

        private void Run()
        {
            for (int i = 0; i < 100; i++)
            {
                Thread.Sleep(1000);
            }
        }

        public override void OnReceive(Context context, Intent intent)
        {
            //Toast.MakeText(context, "Received intent!", ToastLength.Short).Show();
            int action = intent.GetIntExtra("action", 0);
            if(action == 1)
            {
                mp.Start();
                mp.SetVolume((float)0.3, (float)0.3);
                MyService.musicStopped = false;
            }
            else if(action == 0)
            {
                mp.Pause();
                MyService.musicStopped = true;
            }
        }

    }
}