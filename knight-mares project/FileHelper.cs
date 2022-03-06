using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Java.IO;
using System.IO;

namespace knight_mares_project
{
    public class FileHelper
    {
        public static string filePath = "test.bin";
        public static void WriteToBinaryFile<T>(T objectToWrite, bool append = true)
        {
            using (Stream stream = Context.OpenFileOutput(filePath, append ? FileCreationMode.Append : FileCreationMode.Create))
            {
                var binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(stream, objectToWrite);
                stream.Close();
            }
        }

        public static T ReadFromBinaryFile<T>()
        {
            try
            {
                using (Stream stream = OpenFileInput(filePath))
                {
                    var binaryFormatter = new BinaryFormatter();
                    return (T)binaryFormatter.Deserialize(stream);
                }
            }
            catch
            {
                return default(T);
            }
        }

    }
}