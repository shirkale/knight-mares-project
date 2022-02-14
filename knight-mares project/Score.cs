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
using static Android.Resource;

namespace knight_mares_project
{
    public class Score
    {
        public List<Integer> l;

        public Score() { }
        public Score(List<Integer> l)
        {
            this.l = l;
        }
        

    }
}