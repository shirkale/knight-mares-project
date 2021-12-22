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
    [Service]
    public class MyService : Service
    {
        static MediaPlayer mp;
        public override void OnCreate()
        {
            base.OnCreate();
        }
        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            Thread t = new Thread(Run);
            t.Start();
            return base.OnStartCommand(intent, flags, startId);
        }
        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        private void Run()
        {
            if(!MainActivity.hasMusicStarted || mp == null)
            {
                mp = MediaPlayer.Create(this, Resource.Raw.music);
                MainActivity.hasMusicStarted = true;
            }
            mp.Start();
        }

        public static void ResumeMusic()
        {
            if(mp == null)
                mp.Start();
        }

        public static void PauseMusic()
        {
            mp.Pause();
        }
        public static void StopMusic()
        {
            mp.Stop();
            MainActivity.hasMusicStarted = false;
        }

    }
}