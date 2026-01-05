using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace EngineCore
{
    /// <summary>
    /// 游戏信息
    /// </summary>
    public abstract class YuriGameInformation
    {
        /// <summary>
        /// 加密Key(字符串)
        /// </summary>
        public virtual string StringKey { get; } = "yurayuri";
        /// <summary>
        /// 加密Key
        /// </summary>
        public virtual byte[] Key { get; } = Encoding.UTF8.GetBytes("yurayuri");
        /// <summary>
        /// 解密IV
        /// </summary>
        public virtual byte[] IV { get; } = new byte[] { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };

        private readonly static List<YuriGameInformation> sTitles = new(8);
        /// <summary>
        /// 游戏信息
        /// </summary>
        public static ReadOnlyCollection<YuriGameInformation> Titles => YuriGameInformation.sTitles.AsReadOnly();

        /// <summary>
        /// 静态构造函数
        /// </summary>
        static YuriGameInformation()
        {
            List<YuriGameInformation> titles = YuriGameInformation.sTitles;
            titles.Add(new MisrepresentLove());
        }

        public override string ToString()
        {
            return string.Empty;
        }
    }

    /// <summary>
    /// 《线转》
    /// </summary>
    public class MisrepresentLove : YuriGameInformation
    {
        public override string ToString()
        {
            return "线转 MisrepresentLove";
        }
    }
}
