using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EngineCore
{
    /// <summary>
    /// 脚本
    /// </summary>
    public class YuriScenario
    {
        private List<string> mLines = new();
        private string mName = string.Empty;

        /// <summary>
        /// 提取脚本
        /// </summary>
        /// <param name="directory">输出目录</param>
        /// <param name="progressCallBack">回调信息</param>
        public void Extract(string directory, IProgress<string>? progressCallBack = null)
        {
            string extractDirectory = Path.Combine(directory, "Static_Extract");
            string extractPath = Path.Combine(extractDirectory, this.mName + ".txt");
            if (!Directory.Exists(extractDirectory))
            {
                Directory.CreateDirectory(extractDirectory);
            }

            using FileStream outFs = File.Create(extractPath);
            using StreamWriter sw = new(outFs, Encoding.Unicode);
            foreach(string s in this.mLines)
            {
                sw.WriteLine(s);
            }
            sw.Flush();

            progressCallBack?.Report($"脚本提取成功: {this.mName}");
        }

        /// <summary>
        /// 打开脚本
        /// </summary>
        /// <param name="filepath">脚本路径</param>
        /// <param name="gameInfo">游戏信息</param>
        /// <param name="msg">错误信息</param>
        /// <returns>成功:脚本对象 失败:null</returns>
        public static YuriScenario? Open(string filepath, YuriGameInformation gameInfo, out string msg)
        {
            if (!File.Exists(filepath))
            {
                msg = "脚本文件路径不存在";
                return null;
            }

            using StreamReader sr = new(filepath);

            //脚本文件头
            {
                bool hdrVaild = false;
                if(sr.ReadLine() is string header)
                {
                    string[] headers = header.Split('?');
                    hdrVaild = headers.Length != 0;
                }
                if (!hdrVaild)
                {
                    msg = "脚本文件头数据错误";
                    return null;
                }
            }

            YuriScenario scenario = new() 
            { 
                mName = Path.GetFileNameWithoutExtension(filepath)
            };

            //解密脚本
            while (!sr.EndOfStream)
            {
                string s = sr.ReadLine()!;
                if (!string.IsNullOrEmpty(s) && s != ">>>YuriEOF")
                {
                    scenario.mLines.Add(YuriCrypto.DecryptString(s, gameInfo));
                }
            }

            msg = string.Empty;
            return scenario;
        }
    }
}
