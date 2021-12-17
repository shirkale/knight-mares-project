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

namespace knight_mares_project
{
    public class MyHandler : Handler
    {
        Context context;
        //TextView tvResult;

        public MyHandler(Context context)
        {
            this.context = context;
        }

        public override void HandleMessage(Message msg)
        {
            Toast.MakeText(context, "" + msg.Arg1, ToastLength.Long).Show();
        }
    }
}