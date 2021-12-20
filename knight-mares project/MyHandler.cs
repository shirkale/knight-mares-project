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
using Android.Media;

namespace knight_mares_project
{
    public class MyHandler : Handler
    {
        Context context;
        MediaPlayer mp;
        public MyHandler(Context context)
        {
            this.context = context;
        }

        public override void HandleMessage(Message msg)
        {
            mp = MediaPlayer.Create(context, Resource.Raw.music);
            mp.Start();
        }
    }
}