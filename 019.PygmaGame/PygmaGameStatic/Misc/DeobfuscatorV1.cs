using System;
using System.Text;

namespace PygmaGameStatic.Misc
{
    /// <summary>
    /// 去混淆V1
    /// </summary>
    public class DeobfuscatorV1
    {
        /// <summary>
        /// 获取renpy运行时脚本
        /// </summary>
        public static string TransformRuntimeScript_1(string s)
        {
            byte[] bytes = Convert.FromBase64String(s);

            bytes = Zlib.Decompress(bytes);
            bytes = Zlib.Decompress(bytes);

            return Encoding.UTF8.GetString(bytes, 0, bytes.Length);
        }
    }
}
