using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BKEngine
{
    /// <summary>
    /// BKE版本
    /// </summary>
    public enum BKEngineVersion : int
    {
        V20,
        V21,
        V40,
        Unknow,
    }

    public abstract class BKARCFileBase : IDisposable
    {
        
        protected Stream mStream;
        /// <summary>
        /// 获取流是否已释放
        /// </summary>
        public bool IsDispose => this.mStream is null;

        /// <summary>
        /// 封包名称
        /// </summary>
        public string PackageName { get; protected set; }

        /// <summary>
        /// 检查头
        /// </summary>
        /// <returns></returns>
        public abstract bool CheckHeader();

        /// <summary>
        /// 解析文件表
        /// </summary>
        public abstract void ParseEntry();

        /// <summary>
        /// 提取资源
        /// </summary>
        /// <param name="outputDirectory"></param>
        public abstract void Extract(string outputDirectory);

        /// <summary>
        /// 释放
        /// </summary>
        public virtual void Dispose()
        {
            this.mStream?.Dispose();
            this.mStream = null;
        }

        /// <summary>
        /// 创建封包实例
        /// </summary>
        /// <param name="packageName"></param>
        /// <param name="stream"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        public static BKARCFileBase CreateInstance(string packageName, Stream stream, BKEngineVersion version)
        {
            BKARCFileBase bkarc = version switch
            {
                BKEngineVersion.V20 => new BKARCFileV20(),
                BKEngineVersion.V21 => new BKARCFileV21(),
                BKEngineVersion.V40 => new BKARCFileV40(),
                _ => null
            };

            if (bkarc != null)
            {
                bkarc.PackageName = packageName;
                bkarc.mStream = stream;
                if (bkarc.CheckHeader())
                {
                    bkarc.ParseEntry();
                    return bkarc;
                }
                else
                {
                    bkarc.Dispose();
                }
            }
            return null;
        }
    }
}
