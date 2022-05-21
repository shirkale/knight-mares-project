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
        MusicPlayerBroadcastReceiver musicPlayerBroadcast; // broadcast reciever, is registered with the media player an plays the music according to the user
        public override void OnCreate()
        {
            base.OnCreate();
        }
        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            mp = MediaPlayer.Create(this, Resource.Raw.music);
            musicPlayerBroadcast = new MusicPlayerBroadcastReceiver(mp);

            IntentFilter intentFilter = new IntentFilter("music");
            RegisterReceiver(musicPlayerBroadcast, intentFilter);

            if (!MainActivity.muted)
            {
                Intent i = new Intent("music");
                i.PutExtra("action", 1);
                SendBroadcast(i);
            }


            return base.OnStartCommand(intent, flags, startId);
        }

        public override void OnDestroy()
        {
            UnregisterReceiver(musicPlayerBroadcast);
            base.OnDestroy();
        }

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }
    }
}