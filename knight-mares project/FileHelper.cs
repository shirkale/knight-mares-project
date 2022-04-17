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
using System.Reflection;

namespace knight_mares_project
{
    public class FileHelper // TO BE DELETED!!!!!!!!!!!
    {
        public static string filePath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "test1.bin");


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


        public static void SerializeNow<T>(T objectToInsert)
        {
            Stream s = System.IO.File.Open(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            BinaryFormatter b = new BinaryFormatter();
            b.Serialize(s, objectToInsert);
            //Stream s2 = System.IO.File.Open("/Downloads/testBin.bin", FileMode.OpenOrCreate, FileAccess.ReadWrite);
            //b.Serialize(s2, objectToInsert);
            s.Close();

        }
        public static T DeSerializeNow<T>()
        {
            try
            {
                Stream s = System.IO.File.Open(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                BinaryFormatter b = new BinaryFormatter();
                T result = (T)b.Deserialize(s);
                s.Close();
                return result;
            }
            catch
            {
                return default;
            }
        }

        public static List<Square> SquarePathFromInt(List<int> source, Square[,] squareMatrix)
        {
            if (source != null)
            {
                List<Square> newList = new List<Square>();
                for (int k = 0; k < source.Count - 1; k += 2)
                {
                    int i = k;
                    int j = k + 1;
                    newList.Add(squareMatrix[source[i], source[j]]);
                }
                return newList;
            }
            return null;
        }
        public static List<int> IntFromSquarePath(List<Square> source)
        {
            List<int> newList = new List<int>();
            foreach(Square s in source)
            {
                newList.Add(s.GetI());
                newList.Add(s.GetJ());
            }
            return newList;
        }
    }
}