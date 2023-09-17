using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using AonatsuLineStatic;

namespace ConsoleExecute
{
    internal class Program
    {
        static void Main(string[] args)
        {
            List<Tuple<string, PacArchive.EntryMode, bool>> pckInfos = new()
            {
                { new("SPM.pac", PacArchive.EntryMode.EightByteMode, false) },
                { new("Stand_c.pac", PacArchive.EntryMode.TenByteMode, true) },
                { new("Stand.pac", PacArchive.EntryMode.TenByteMode, false) },
                { new("Thumbnail.pac", PacArchive.EntryMode.TenByteMode, false) },
                { new("Visual.pac", PacArchive.EntryMode.TenByteMode, false) },
                { new("Voice.pac", PacArchive.EntryMode.TenByteMode, false) },
                { new("Update\\SystemUpdate1.pac", PacArchive.EntryMode.TenByteMode, false) },
                { new("Update\\ThumbnailUpdate1.pac", PacArchive.EntryMode.TenByteMode, false) },
                { new("Update\\VisualUpdate1.pac", PacArchive.EntryMode.TenByteMode, false) },
                { new("Update\\VoiceUpdate1.pac", PacArchive.EntryMode.TenByteMode, false) }
            };



            string gameDirectory = @"D:\#DownLoad\Aonatsu_Line";        //此处填写你的游戏目录

            string extractdirectory = Path.Combine(gameDirectory, "Extract_Static");

            foreach(var pck in CollectionsMarshal.AsSpan(pckInfos))
            {
                PacArchive arc = new(Path.Combine(gameDirectory, pck.Item1), pck.Item2, pck.Item3);
                if (arc.Extract(extractdirectory))
                {
                    Console.WriteLine("{0}    解包成功", pck.Item1);
                }
                else
                {
                    Console.WriteLine("{0}    解包失败", pck.Item1);
                }
            }
        }
    }
}