using BlueAngel.StarlightofAeons;
using BlueAngel.V1;
using System;
using System.Windows.Forms;

namespace StarlightofAeonsExtractor
{
    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            using OpenFileDialog ofd = new()
            {
                AddExtension = true,
                AutoUpgradeEnabled = true,
                CheckFileExists = true,
                CheckPathExists = true,
                DefaultExt = ".xp3",
                Filter = "XP3封包(*.xp3)|*.xp3|所有文件(*.*)|*.*",
                Multiselect = false,
                RestoreDirectory = true,
                ShowHelp = false,
                Title = "亿万年的星光 - 选择封包",
            };

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string filepath = ofd.FileName;
                Archive archive = new(filepath);
                ArchiveCrypto.SubstitutionBoxInitialize(out archive.mTableKey32_1, out archive.mTableKey32_2, out archive.mTableKey32_3,
                                                        out archive.mTableKey32_4, out archive.mTableKey32_5, out archive.mTableKey32_6,
                                                        out archive.mTableKey32_7, out archive.mTableKey32_8, out archive.mTableKey32_9,
                                                        out archive.mTableKey8_1, out archive.mTableKey8_2);
                archive.Extract();
                Console.WriteLine("\n\n======== 亿万年的星光 ---- 提取成功 ========");
                Console.Read();
            }

        }
    }
}