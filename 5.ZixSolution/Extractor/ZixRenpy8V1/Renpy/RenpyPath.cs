using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Extractor.ZixRenpy8V1.Renpy
{
    public class RenpyPath
    {
        private string mPath;

        /// <summary>
        /// 获取资源路径
        /// </summary>
        /// <returns></returns>
        public string GetArchivePath()
        {
            return Path.Combine(this.mPath, "game");
        }

        /// <summary>
        /// 获取py模块路径
        /// </summary>
        /// <returns></returns>
        public string GetModulePath()
        {
            return Path.Combine(this.mPath, "renpy");
        }


        /// <summary>
        /// 获取所有资源文件全路径
        /// </summary>
        /// <returns></returns>
        public string[] GetAllArchiveFilesFullPath()
        {
            return Directory.GetFiles(this.GetArchivePath(), "*.rpa", SearchOption.AllDirectories);
        }

        /// <summary>
        /// 获取所有模块文件全路径 (加密的)
        /// </summary>
        /// <returns></returns>
        public string[] GetAllModuleFilesFullPath()
        {
            return Directory.GetFiles(this.GetModulePath(), "*.pye", SearchOption.AllDirectories);
        }

        /// <summary>
        /// 获取提取目录
        /// </summary>
        /// <returns></returns>
        public string GetExtractPath()
        {
            return Path.Combine(this.mPath, "Static_Extract");
        }

        /// <summary>
        /// 获取相对路径
        /// </summary>
        /// <returns></returns>
        public string GetRelativePath(string path)
        {
            if (this.mPath == path.Substring(0, this.mPath.Length))
            {
                ReadOnlySpan<char> str = path.AsSpan().Slice(this.mPath.Length);

                int pos = 0;
                for(int i = 0; i < str.Length; ++i)
                {
                    if (str[i]=='\\'|| str[i] == '/')
                    {
                        pos++;
                    }
                    else
                    {
                        break;
                    }
                }

                return str.Slice(pos).ToString();
            }
            else
            {
                return path;
            }
        }

        /// <summary>
        /// 恢复扩展名
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public string FixExtension(string path)
        {
            return Path.ChangeExtension(path, ".pyc");
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="gamePath">游戏路径</param>
        public RenpyPath(string gamePath)
        {
            this.mPath = gamePath;
        }
    }
}
