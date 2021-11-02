using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace knight_mares_project
{
    class Helper
    {
        public static string dbname = "dbTest3";
        public Helper()
        {
        }
        public static string Path()
        {
            string path = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), Helper.dbname);
            return path;
        }
        public static string BitmapToBase64(Bitmap bitmap)
        {
            string str = "";
            using (var stream = new MemoryStream())
            {
                bitmap.Compress(Bitmap.CompressFormat.Png, 0, stream);
                var bytes = stream.ToArray();
                str = Convert.ToBase64String(bytes);
            }
            return str;
        }

        public static Bitmap Base64ToBitmap(string base64String)
        {
            //if (base64String == null)
            //    return BitmapFactory.DecodeResource(Resources, Resource.Drawable.woman);
            byte[] imageAsBytes = Base64.Decode(base64String, Base64Flags.Default);
            return BitmapFactory.DecodeByteArray(imageAsBytes, 0, imageAsBytes.Length);
        }
    }
}