using System;
using System.IO;
using SoraPlayerStatic;

namespace ConsoleExecute
{
    class Program
    {

        static void Main(string[] args)
        {
            string[] packs = Directory.GetFiles("E:\\夏花的轨迹——That Summer Of Eternal Eden-体验版β", "*.soa");

            foreach(var packPath in packs)
            {
                Archive archive = new(packPath);
                archive.Extract();
            }
        }
    }
}
