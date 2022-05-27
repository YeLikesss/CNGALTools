using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsicxArt.V1
{
    /// <summary>
    /// 吸血鬼旋律
    /// </summary>
    public class VampiresMelody
    {
        private static string mStringKey = "000000c200000050000000ab000000a0000000b5000000f900000046000000ce000000ff0000009c000000900000003e000000040000000b0000000e0000006d";

        public static byte[] Key
        {
            get
            {
                return Encoding.UTF8.GetBytes(mStringKey);
            }
        }
    }
    /// <summary>
    /// 茸茸便利店
    /// </summary>
    public class FluffyStore
    {
        private static string mStringKey = "000000b4000000fe000000fa000000ea000000830000000200000034000000fe000000b3000000110000003f0000001e000000580000007f0000008e000000c9";

        public static byte[] Key
        {
            get
            {
                return Encoding.UTF8.GetBytes(mStringKey);
            }
        }
    }
}
