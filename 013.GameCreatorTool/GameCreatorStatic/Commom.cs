using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameCreatorStatic
{
    /// <summary>
    /// 提取器版本
    /// </summary>
    public enum GCExtractorVersion : uint
    {
        V1,
    }

    /// <summary>
    /// 提取器接口
    /// </summary>
    public interface IGCExtractor
    {
        /// <summary>
        /// 提取器版本
        /// </summary>
        public GCExtractorVersion ExtractorVersion { get; }
        /// <summary>
        /// 提取资源
        /// </summary>
        /// <param name="gameDirectory">游戏目录</param>
        /// <param name="msgcallback">信息回调</param>
        public void Extract(string gameDirectory, IProgress<string>? msgcallback = null);
    }
}
