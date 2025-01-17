using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using EngineCore;

namespace AonatsuLine
{
    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            List<(string, PacArchive.EntryMode, bool)> pckInfos = new()
            {
                { ("SPM.pac", PacArchive.EntryMode.EightByteMode, false) },
                { ("Stand_c.pac", PacArchive.EntryMode.TenByteMode, true) },
                { ("Stand.pac", PacArchive.EntryMode.TenByteMode, false) },
                { ("Thumbnail.pac", PacArchive.EntryMode.TenByteMode, false) },
                { ("Visual.pac", PacArchive.EntryMode.TenByteMode, false) },
                { ("Voice.pac", PacArchive.EntryMode.TenByteMode, false) },

                { ("Update\\SystemUpdate1.pac", PacArchive.EntryMode.TenByteMode, false) },
                { ("Update\\ThumbnailUpdate1.pac", PacArchive.EntryMode.TenByteMode, false) },
                { ("Update\\VisualUpdate1.pac", PacArchive.EntryMode.TenByteMode, false) },
                { ("Update\\VoiceUpdate1.pac", PacArchive.EntryMode.TenByteMode, false) }
            };

            using FolderBrowserDialog fbd = new()
            {
                Description = "アオナツライン [官中版] - 请选择游戏文件夹",
                ShowNewFolderButton = false,
                AutoUpgradeEnabled = true,
                UseDescriptionForTitle = true
            };
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                string gameDirectory = fbd.SelectedPath;
                string extractDirectory = Path.Combine(gameDirectory, "Extract_Static");

                foreach (var pck in pckInfos)
                {
                    PacArchive arc = new(Path.Combine(gameDirectory, pck.Item1), pck.Item1, pck.Item2, pck.Item3);
                    if (arc.Extract(extractDirectory))
                    {
                        Console.WriteLine("{0} 解包成功", pck.Item1);
                    }
                    else
                    {
                        Console.WriteLine("{0} 解包失败", pck.Item1);
                    }
                }
                Console.WriteLine("\n===== アオナツライン [官中版] - 提取完成 =====");
                Console.Read();
            }
        }
    }
}