using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using IrregularsStatic;

namespace ConsoleTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //设置你的游戏文件夹
            string gameDir = "D:\\Galgame Reverse\\MOBIUS BAND";

            List<string> resDirs = new(3)
            {
                "settings",
                "data",
                //"localization",               //demo版有加密 正式版没加密
            };

            MobiusBand mobiusBand = new();

            /*
             * 
             * 如果你需要破解游戏过校验
             * 解密settings_Template.ini
             * HashVerification = 1 改为 0
             * 加密回封替换原文件
             * 
             */


            //加密回封
            if (false)
            {
                string dynamicSettingFile = Path.Combine(gameDir, "Static_Pack\\settings\\settings_Dynamic.ini");
                string templateSettingFile = Path.Combine(gameDir, "Static_Pack\\settings\\settings_Template.ini");

                string outDynamicSettingFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings_Dynamic.ini");
                string outTemplateSettingFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings_Template.ini");

                {
                    using Stream encStream = mobiusBand.CreateEncryptStream(dynamicSettingFile);
                    using FileStream outFs = File.Create(outDynamicSettingFile);
                    encStream.CopyTo(outFs);
                    outFs.Flush();
                }

                {
                    using Stream encStream = mobiusBand.CreateEncryptStream(templateSettingFile);
                    using FileStream outFs = File.Create(outTemplateSettingFile);
                    encStream.CopyTo(outFs);
                    outFs.Flush();
                }
                return;
            }

            //解密
            foreach(string resdir in resDirs)
            {
                List<string> resFiles = PathUtil.EnumerateFullName(Path.Combine(gameDir, resdir));

                foreach(string resFilePath in resFiles)
                {
                    using Stream stream = mobiusBand.CreateDecryptStream(resFilePath);
                    if (stream != Stream.Null)
                    {
                        string outPath = Path.Combine(gameDir, "Static_Extract", resFilePath[(gameDir.Length + 1)..]);
                        {
                            string dir = Path.GetDirectoryName(outPath);
                            if (!Directory.Exists(dir))
                            {
                                Directory.CreateDirectory(dir);
                            }

                            using FileStream outFs = new(outPath, FileMode.Create, FileAccess.ReadWrite);
                            stream.CopyTo(outFs);
                            outFs.Flush();
                        }
                    }
                }
            }
        }
    }
}