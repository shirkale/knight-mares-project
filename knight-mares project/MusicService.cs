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
    public class MusicService : Service
    {
        MediaPlayer mp; // media player which plays the music
        MusicPlayerBroadcastReciever musicPlayerBroadcast; // broadcast reciever, is registered with the media player an plays the music according to the user

        public static bool musicInit = true; // checking if music has been initialized
        public override void OnCreate()
        {
            base.OnCreate();

            musicInit = true;

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


            // thread which stops the service if music is stopped for a long time, user left the app
            Thread threadStopService = new Thread(RunUntilMusicStopped);
            threadStopService.Start();

            return base.OnStartCommand(intent, flags, startId);
        }

        private void RunUntilMusicStopped()
        {
            while (musicInit) ;
            StopSelf();
        }

        public override void OnDestroy()
        {
            UnregisterReceiver(musicPlayerBroadcast);
            musicInit = false;
            base.OnDestroy();
        }

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }
    }
}