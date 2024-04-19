using System.Collections.Generic;
using System.IO;
using System;
using EngineCore;

namespace MakingLoverFHD
{
    internal class Program
    {
        static void Main(string[] args)
        {
            List<Tuple<string, PacArchive.EntryMode, bool>> pckInfos = new()
            {
                { new("Visual.pac", PacArchive.EntryMode.TenByteMode, false) },
                { new("Visual_tw.pac", PacArchive.EntryMode.TenByteMode, false) },
                { new("Stand_c.pac", PacArchive.EntryMode.TenByteMode, true) },
                { new("Stand.pac", PacArchive.EntryMode.TenByteMode, false) },
                { new("Thumbnail.pac", PacArchive.EntryMode.TenByteMode, false) },
                { new("Thumbnail_tw.pac", PacArchive.EntryMode.TenByteMode, false) },
                { new("Voice.pac", PacArchive.EntryMode.TenByteMode, false) },

                { new("Update\\VisualUpdate1.pac", PacArchive.EntryMode.TenByteMode, false) },
                { new("Update\\StandUpdate1.pac", PacArchive.EntryMode.TenByteMode, false) },
                { new("Update\\VoiceUpdate1.pac", PacArchive.EntryMode.TenByteMode, false) },
                { new("Update\\ThumbnailUpdate1.pac", PacArchive.EntryMode.TenByteMode, false) },
                { new("Update\\SystemUpdate1.pac", PacArchive.EntryMode.TenByteMode, false) },

                { new("FD\\Visual.pac", PacArchive.EntryMode.TenByteMode, false) },
                { new("FD\\Visual_tw.pac", PacArchive.EntryMode.TenByteMode, false) },
                { new("FD\\Stand_c.pac", PacArchive.EntryMode.TenByteMode, true) },
                { new("FD\\Stand.pac", PacArchive.EntryMode.TenByteMode, false) },
                { new("FD\\Thumbnail.pac", PacArchive.EntryMode.TenByteMode, false) },
                { new("FD\\Thumbnail_tw.pac", PacArchive.EntryMode.TenByteMode, false) },
                { new("FD\\Voice.pac", PacArchive.EntryMode.TenByteMode, false) },

                { new("FD\\Update\\VisualUpdate1.pac", PacArchive.EntryMode.TenByteMode, false) },
                { new("FD\\Update\\StandUpdate1.pac", PacArchive.EntryMode.TenByteMode, false) },
                { new("FD\\Update\\VoiceUpdate1.pac", PacArchive.EntryMode.TenByteMode, false) },
                { new("FD\\Update\\ThumbnailUpdate1.pac", PacArchive.EntryMode.TenByteMode, false) },
                { new("FD\\Update\\SystemUpdate1.pac", PacArchive.EntryMode.TenByteMode, false) },

            };

            string gameDirectory = "D:\\Galgame Reverse\\Making_Lovers_FHD_R18";        //此处填写你的游戏目录

            string extractdirectory = Path.Combine(gameDirectory, "Extract_Static");
            foreach (var pck in pckInfos)
            {
                PacArchive arc = new(Path.Combine(gameDirectory, pck.Item1), pck.Item1, pck.Item2, pck.Item3);
                if (arc.Extract(extractdirectory))
                {
                    Console.WriteLine("{0}  解包成功", pck.Item1);
                }
                else
                {
                    Console.WriteLine("{0}  解包失败", pck.Item1);
                }
            }

            Console.WriteLine("\n========请按任意键退出========");
            Console.Read();
        }
    }
}