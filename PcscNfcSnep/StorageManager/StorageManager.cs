using System;
using System.IO;

namespace IOStorage
{
    public class StorageManager
    {
        static public void FileWrite(string fileType, string contents)
        {
            File.WriteAllText(fileType, contents);
        }

        static bool TryFileMove(string source, string destination)
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

        static bool TryFileDelete(string source)
        {
            if(!File.Exists(source))
            {
                File.Delete(source);
                return true;
            }

            return false;
        }

        static bool PathCombine(string path, out string pathCombine)
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
