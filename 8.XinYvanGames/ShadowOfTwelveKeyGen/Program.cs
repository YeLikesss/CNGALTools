using System;
using System.IO;
using System.Text;

namespace ShadowOfTwelveKeyGen
{
    class Program
    {
        static void Main(string[] args)
        {
            //code.txt在游戏目录 ./ShiErKe_Data/StreamingAssets/ 下
            KeyGen(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "code.txt"));
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

            codeSR.Close();
            keySW.Flush();
            keySW.Close();
        }


    }
}
