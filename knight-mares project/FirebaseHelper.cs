using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Firebase.Database;
using Firebase.Database.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace knight_mares_project
{
    public class FirebaseHelper
    {
        public static FirebaseClient client = new FirebaseClient("https://database-37bc4-default-rtdb.europe-west1.firebasedatabase.app/");
        private static string database = "dbWorldRecords";

        public static async Task<List<ScoreList>> GetAll()
        {
            var respone = await client.Child(database).OnceAsync<ScoreList>();
            return (respone).Select(item => new ScoreList
            {
                listOfScores = item.Object.listOfScores,

            }).ToList();
        }

        public static async Task Add(ScoreList l)
        {
            await client
                .Child(database)
                .PostAsync(l);
        }

        public static async Task<ScoreList> Get(int dif)
        {
            var allPersons = await GetAll();
            await client
              .Child(database)
              .OnceAsync<ScoreList>();
            return allPersons.Where(a => true).FirstOrDefault();
        }

        public static async Task Update(ScoreList state)
        {
            var toUpdatePerson = (await client
              .Child(database)
              .OnceAsync<ScoreList>()).Where(a => true).FirstOrDefault();

            await client
              .Child(database)
              .Child(toUpdatePerson.Key)
              .PutAsync(state);
        }
    }
}