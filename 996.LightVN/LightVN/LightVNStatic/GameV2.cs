using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightVNStatic
{
    /// <summary>
    /// 游戏信息
    /// </summary>
    public interface IGameInfoV2
    {
        /// <summary>
        /// 文件表文件名
        /// </summary>
        public string FileListFileName { get; }
    }

    /// <summary>
    /// プトリカ1st.cut
    /// </summary>
    public class PutrikaFirst : CryptoFilterV2, IGameInfoV2
    {
        public sealed override byte[] Key { get; } = new byte[]
        {
            0x64, 0x36, 0x63, 0x35, 0x66, 0x4B, 0x49, 0x33, 0x47, 0x67, 0x42, 0x57, 0x70, 0x5A, 0x46, 0x33,
            0x54, 0x7A, 0x36, 0x69, 0x61, 0x33, 0x6B, 0x46, 0x30, 0x00
        };

        public string FileListFileName { get; } = "Data\\_\\0.mcdat";

        public override string ToString()
        {
            return "プトリカ1st.cut";
        }
    }
}
