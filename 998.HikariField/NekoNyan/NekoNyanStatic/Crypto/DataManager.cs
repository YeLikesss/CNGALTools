using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace NekoNyanStatic.Crypto
{
    /// <summary>
    /// 数据库
    /// </summary>
    public class DataManager
    {
        /// <summary>
        /// 游戏信息
        /// </summary>
        public static Dictionary<string, CryptoVersion> GameInformations { get; } = new(16)
         {
            { "Ao no Kanata no Four Rhythm PE", CryptoVersion.V10 },
            { "Ao no Kanata no Four Rhythm Extra 1", CryptoVersion.V10 },
            { "Kinkoi: Golden Loveriche", CryptoVersion.V10},
            { "Ao no Kanata no Four Rhythm Extra 2", CryptoVersion.V11},
            { "Clover Days", CryptoVersion.V12 },
        };
    }
}
