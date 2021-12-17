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
using System.Threading;

namespace knight_mares_project
{
    [Service]
    public class MyService : Service
    {
        int counter;
        MyHandler myhandler;
        public override void OnCreate()
        {
            base.OnCreate();
            myhandler = new MyHandler(this);
        }
        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            counter = intent.GetIntExtra("counter", 3);
            Toast.MakeText(this, "service started " + counter, ToastLength.Long).Show();
            Thread t = new Thread(Run);
            t.Start();
            return base.OnStartCommand(intent, flags, startId);
        }
        public override void OnDestroy()
        {
            base.OnDestroy();
            Toast.MakeText(this, "Service Stoped", ToastLength.Long).Show();
            counter = 0;
        }
        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        private void Run()
        {
            while (counter > 0)
            {
                Thread.Sleep(3000);
                Message mes = new Message();
                mes.Arg1 = counter;
                myhandler.SendMessage(mes);
                counter--;
            }
            StopSelf();
        }
    }
}