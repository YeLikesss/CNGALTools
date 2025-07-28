using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace RPAArchive
{
    /// <summary>
    /// 封包版本
    /// </summary>
    public enum RenpyRPAVersion
    {
        /// <summary>
        /// 未知
        /// </summary>
        Unknow,
        /// <summary>
        /// V3
        /// </summary>
        RPAv3,
    }

    /// <summary>
    /// 封包类
    /// </summary>
    public abstract class RenpyRPA
    {
        protected readonly string mFilePath;
        protected readonly string mFileName;
        protected readonly string mName;

        /// <summary>
        /// 文件路径
        /// </summary>
        public string FilePath => this.mFilePath;
        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName => this.mFileName;
        /// <summary>
        /// 封包名
        /// </summary>
        public string Name => this.mName;
        /// <summary>
        /// 封包版本
        /// </summary>
        public abstract RenpyRPAVersion Version { get; }
        /// <summary>
        /// 文件个数
        /// </summary>
        public abstract int Count { get; }
        /// <summary>
        /// 提取资源
        /// </summary>
        /// <param name="progressCallBack">进度回调</param>
        /// <returns>True提取成功 False提取是把你</returns>
        public abstract bool Extract(IProgress<string>? progressCallBack = null);

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="filePath">封包路径</param>
        public RenpyRPA(string filePath)
        {
            this.mFilePath = filePath;
            this.mFileName = Path.GetFileName(filePath);
            this.mName = Path.GetFileNameWithoutExtension(filePath);
        }
    }

    /// <summary>
    /// RPA工厂
    /// </summary>
    public static class RenpyRPAFactory
    {
        /// <summary>
        /// 创建RPA封包对象
        /// </summary>
        /// <param name="path">封包路径</param>
        /// <param name="lastError">错误信息</param>
        /// <returns>RPA封包对象</returns>
        public static RenpyRPA? Create(string path, out string lastError)
        {
            if (!File.Exists(path))
            {
                lastError = "文件不存在";
                return null;
            }

            RenpyRPA? rpa;
            try
            {
                //RPAv3
                rpa = RenpyRPAv3.Create(path);
                if (rpa is null)
                {
                    lastError = "文件格式不支持";
                    return null;
                }
            }
            catch(Exception e)
            {
                lastError = e.Message;
                return null;
            }
            lastError = string.Empty;
            return rpa;
        }
    }
}
