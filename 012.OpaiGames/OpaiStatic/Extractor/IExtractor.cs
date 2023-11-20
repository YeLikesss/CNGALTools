using System;

namespace OpaiStatic.Extractor
{
    /// <summary>
    /// 解包接口
    /// </summary>
    public interface IExtractor
    {
        /// <summary>
        /// 解包
        /// </summary>
        /// <param name="rootPath">游戏根目录</param>
        public void Extract(string rootPath);
    }
}
