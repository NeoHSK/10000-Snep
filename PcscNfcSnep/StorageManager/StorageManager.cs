using System;
using System.IO;

namespace IOStorage
{
    public class StorageManager
    {
#if false
        static readonly string mDir = "D:\\NIPRO_NFC";
        static readonly string relPath = "RESULT";
        readonly string fullPath = mDir + Path.DirectorySeparatorChar + relPath;

        static readonly string mDir2 = "D:\\NIPRO_NFC\\";
        static readonly string relPath2 = "RESULT";
        readonly string fullPath2 = mDir2 + Path.DirectorySeparatorChar + relPath2;

        string[] paths = {"C:\\NIPRO_P2P","RESULT","20210308" };
#endif
        static public void FileWrite(string fileType, string contents)
        {
            File.WriteAllText(fileType, contents);
        }

        static public bool TryFileMove(string source, string destination)
        {
            if(!File.Exists(source))
            {
                return false;
            }

            try
            {
                File.Move(source, destination);
                return true;
            }
            catch (Exception e)
            {
                if(e is DirectoryNotFoundException)
                {
                    Console.WriteLine(e.Message);
                }
                else if (e is IOException)
                {
                    Console.WriteLine(e.Message);
                }
                else
                {
                    Console.WriteLine(e.Message);
                }

                return false;
            }
        }

        static public bool TryFileDelete(string source)
        {
            if(!File.Exists(source))
            {
                File.Delete(source);
                return true;
            }

            return false;
        }

        static  bool PathCombine(string path, out string pathCombine)
        {
            try
            {
                pathCombine = Path.Combine(path);

                return true;
            }
            catch (Exception)
            {
                pathCombine = "";

                return false;
            }
        }
    }
}
