using System;
using System.IO;

namespace Rename
{
    class Program
    {
        static void Main(string[] args)
        {

            DirectoryInfo ResDir = new("E:\\花都之恋\\Resources\\Extract\\PNG");

            FileInfo[] ResFiles = ResDir.GetFiles();

            foreach(FileInfo resFile in ResFiles)
            {
                string fileNameNoExtension = Path.GetFileNameWithoutExtension(resFile.FullName);
                if (Path.GetExtension(fileNameNoExtension) == ".pvr")
                {
                    string filename = resFile.FullName.Replace(".pvr.png", ".png", StringComparison.OrdinalIgnoreCase);
                    resFile.MoveTo(filename);
                }
            }

        }
    }
}
