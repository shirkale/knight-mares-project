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
using Java.IO;
using System.IO;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace knight_mares_project
{
    public class FileHelper
    {
        public static string filePath;
        //public void WriteToBinaryFile<T>(T objectToWrite, bool append = true)
        //{
        //    using (Stream stream = OpenFileOutput(filePath, FileCreationMode.Private))
        //    {
        //        var binaryFormatter = new BinaryFormatter();
        //        binaryFormatter.Serialize(stream, objectToWrite);
        //        stream.Close();
        //    }
        //}

        //public T ReadFromBinaryFile<T>()
        //{
        //    try
        //    {
        //        using (Stream stream = OpenFileInput(filePath))
        //        {
        //            var binaryFormatter = new BinaryFormatter();
        //            return (T)binaryFormatter.Deserialize(stream);
        //        }
        //    }
        //    catch
        //    {
        //        return default(T);
        //    }
        //}


        public static void SerializeNow(List<Square> lSquare)
        {
            Stream s = System.IO.File.Open(filePath, FileMode.Append);
            BinaryFormatter b = new BinaryFormatter();
            b.Serialize(s, lSquare);
            s.Close();
        }
        public static T DeSerializeNow<T>()
        {
            Stream s = System.IO.File.Open(filePath, FileMode.Append);
            BinaryFormatter b = new BinaryFormatter();
            T result = (T)b.Deserialize(s);
            s.Close();
            return result;
        }

    }
}