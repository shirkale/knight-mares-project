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
        MediaPlayer mp;
        MusicPlayerBroadcastReciever musicPlayerBroadcast;
        public static bool musicStopped = false;
        public override void OnCreate()
        {
            base.OnCreate();
            mp = MediaPlayer.Create(this, Resource.Raw.music);
            musicPlayerBroadcast = new MusicPlayerBroadcastReciever(mp);
            IntentFilter intentFilter = new IntentFilter("music");
            RegisterReceiver(musicPlayerBroadcast, intentFilter);
        }
        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            Intent i = new Intent("music");
            i.PutExtra("action", 1);
            SendBroadcast(i);
            Thread t = new Thread(Run);
            t.Start();


            return base.OnStartCommand(intent, flags, startId);
        }

        private void Run()
        {
            while (true)
            {
                if (musicStopped)
                {

                }
            }
        }

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }
    }
}