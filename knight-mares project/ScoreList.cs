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
    public class ScoreList
    {
        public List<int> listOfScores;

        public ScoreList() { }
        public ScoreList(List<int> l)
        {
            this.listOfScores = l;
        }
        

    }
}