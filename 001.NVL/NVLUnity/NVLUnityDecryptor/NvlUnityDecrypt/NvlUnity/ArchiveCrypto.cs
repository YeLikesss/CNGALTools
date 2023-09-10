using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NvlUnity.V1;

namespace NvlUnity
{
    public abstract class ArchiveDecryptorBase
    {
        /// <summary>
        /// 导出文件夹
        /// </summary>
        public string OutPutDirectory { get; protected set; }

        /// <summary>
        /// 提取文件
        /// </summary>
        /// <param name="filePath">文件全路径</param>
        public abstract bool Extract(string filePath);

        /// <summary>
        /// 创建解密器
        /// </summary>
        /// <param name="outPutDir">导出目录</param>
        /// <param name="title">游戏标题</param>
        /// <returns></returns>
        public static ArchiveDecryptorBase Create(string outPutDir, string title)
        {
            if (DataManagerV1.SGameInformation.ContainsKey(title))
            {
                return ArchiveDecryptorV1.CreateInstance(outPutDir, title);
            }
            else
            {
                return null;
            }
        }
    }



    public class ArchiveDecryptorV1 : ArchiveDecryptorBase
    {

        private NVLFilterV1 mFilter;

        /// <summary>
        /// V1加密
        /// </summary>
        /// <param name="title"></param>
        private ArchiveDecryptorV1()
        {
        }


        public override bool Extract(string filePath)
        {
            if (File.Exists(filePath))
            {
                string outPutPath = Path.Combine(this.OutPutDirectory, Path.ChangeExtension(Path.GetFileName(filePath), ".asset"));
                {
                    string dir = Path.GetDirectoryName(outPutPath);
                    //创建文件夹
                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }
                }

                using FileStream outFs = new(outPutPath, FileMode.Create, FileAccess.ReadWrite);
                using FileStream inFs = File.OpenRead(filePath);
                outFs.Position = 0;
                inFs.Position = 0;

                long offset = 0;       //资源偏移

                Span<byte> buffer = stackalloc byte[4096];

                //解密
                while (inFs.Position < inFs.Length)
                {
                    int readLen = inFs.Read(buffer);

                    Span<byte> data = buffer[0..readLen];
                    this.mFilter.Decrypt(data, offset);
                    outFs.Write(data);

                    offset += readLen;
                }

                outFs.Flush();

                Console.WriteLine(string.Format("Decrypt Success ---> {0}", Path.GetFileName(filePath)));

                return true;
            }
            return false;
        }


        /// <summary>
        /// 创建V1版加密
        /// </summary>
        /// <param name="outPutDir">导出文件夹</param>
        /// <param name="title">游戏名</param>
        /// <returns></returns>
        public static ArchiveDecryptorV1 CreateInstance(string outPutDir, string title)
        {
            if(DataManagerV1.SGameInformation.TryGetValue(title, out NVLUnityV1 keyV1))
            {
                return new()
                {
                    OutPutDirectory = outPutDir,
                    mFilter = new(keyV1)
                };
            }
            else
            {
                return null;
            }
        }
    }


    public class DataManager
    {
        /// <summary>
        /// 单一实例
        /// </summary>
        public static readonly DataManager Instance = new();

        /// <summary>
        /// 获取游戏标题
        /// </summary>
        public List<string> GameTitles { get; private set; }

        private void Initialize()
        {
            //标题
            {
                List<string> titles = new(32);
                titles.AddRange(DataManagerV1.SGameInformation.Keys);
                this.GameTitles = titles;
            }
        }


        private DataManager()
        {
            this.Initialize();
        }
    }
}
