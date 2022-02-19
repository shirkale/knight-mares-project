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

        public static async Task<List<Score>> GetAll()
        {
            var respone = await client.Child(database).OnceAsync<Score>();
            return (respone).Select(item => new Score
            {
                l = item.Object.l,

            }).ToList();
        }

        public static async Task Add(List<Score> l)
        {
            await client
                .Child(database)
                .PostAsync(l);
        }

        public static async Task<Score> Get(int dif)
        {
            var allPersons = await GetAll();
            await client
              .Child(database)
              .OnceAsync<Score>();
            return allPersons.Where(a => true).FirstOrDefault();
        }

        public static async Task Update(Score state)
        {
            var toUpdatePerson = (await client
              .Child(database)
              .OnceAsync<Score>()).Where(a => true).FirstOrDefault();

            await client
              .Child(database)
              .Child(toUpdatePerson.Key)
              .PutAsync(state);
        }
    }
}