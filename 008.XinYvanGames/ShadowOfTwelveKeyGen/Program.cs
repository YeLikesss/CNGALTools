using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace ShadowOfTwelveKeyGen
{
    class Program
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
                DefaultExt = ".txt",
                Filter = "验证文档(*.txt)|*.txt|所有文件(*.*)|*.*",
                Multiselect = false,
                RestoreDirectory = true,
                ShowHelp = false,
                Title = "十二刻度月计时 - 选择验证Key文档",
            };
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                KeyGen(ofd.FileName);
                Console.WriteLine("====== 十二刻度月计时 KeyGen ======");
                Console.Read();
            }
        }

        private static void KeyGen(string codePath)
        {
            string table = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            int tableLength = table.Length;

            using StreamReader codeSR = new(codePath, Encoding.Default);
            using StreamWriter keySW = new(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Key.txt"), false, Encoding.Unicode);

            while (!codeSR.EndOfStream)
            {
                string code = codeSR.ReadLine();
                string key = string.Empty;

                for(int i = 0; i < code.Length; ++i)
                {
                    int tableIndex = table.IndexOf(code[i]);

                    tableIndex -= 13;
                    if (tableIndex < 0)
                    {
                        tableIndex += tableLength;
                    }
                    key += table[tableIndex];
                }
                keySW.WriteLine(key);
            }

            keySW.Flush();
            keySW.Close();
        }
    }
}
