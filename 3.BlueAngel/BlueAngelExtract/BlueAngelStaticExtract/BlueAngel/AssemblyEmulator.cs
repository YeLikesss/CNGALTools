

namespace BlueAngel
{
    /// <summary>
    /// 汇编仿真
    /// </summary>
    public class AssemblyEmulator
    {
        /// <summary>
        /// 循环左移
        /// </summary>
        /// <param name="reg32">Dword数据</param>
        /// <param name="shiftCount">位移数量</param>
        public static void ROL(ref uint reg32,uint shiftCount)
        {
            uint mShiftCount = shiftCount % 32;      //有效位移次数
            uint mask = 0x00000000;         //掩码用于取位移出来的值
            uint bitSet = 0x80000000;       //位设置
            for (int loop = 0; loop < mShiftCount; loop++)
            {
                mask |= bitSet;
                bitSet >>= 1;
            }
            uint shiftOut = reg32 & mask;  //获取位移出来的数据
            reg32 <<= (int)mShiftCount;     //位移操作数
            shiftOut >>= (32 - (int)mShiftCount);        //将位移出数据移动到右侧
            reg32 |= shiftOut;          //合并数据
        }

        /// <summary>
        /// 循环右移
        /// </summary>
        /// <param name="reg32">Dword数据</param>
        /// <param name="shiftCount">位移次数</param>
        public static void ROR(ref uint reg32,uint shiftCount)
        {
            uint mShiftCount = shiftCount % 32;      //有效位移次数
            uint mask = 0x00000000;         //掩码用于取位移出来的值
            uint bitSet = 0x00000001;       //位设置
            for (int loop = 0; loop < mShiftCount; loop++)
            {
                mask |= bitSet;
                bitSet <<= 1;
            }
            uint shiftOut = reg32 & mask;  //获取位移出来的数据
            reg32 >>= (int)mShiftCount;     //位移操作数
            shiftOut <<= (32 - (int)mShiftCount);        //将位移出数据移动到左侧
            reg32 |= shiftOut;          //合并数据
        }
        /// <summary>
        /// 双精度右移
        /// </summary>
        /// <param name="reg32Dest">目标操作数</param>
        /// <param name="reg32Sour">源操作数</param>
        /// <param name="shiftCount">位移次数</param>
        /// <remarks>shrd reg32,reg32,cl/imm8</remarks>
        public static void SHRD(ref uint reg32Dest,uint reg32Sour,byte shiftCount)
        {
            shiftCount %= 32;           //有效位移次数
            reg32Dest >>= shiftCount;
            //去源操作数位移掩码
            uint mask = 0;
            uint bitSet = 0x00000001;
            for (int shiftLoop = 0; shiftLoop < shiftCount; shiftLoop++)
            {
                mask |= bitSet;
                bitSet <<= 1;
            }
            //取源操作数位移内容
            uint sourceShiftData = reg32Sour & mask;
            //将源操作数位移数据复制到目标操作数
            sourceShiftData <<= (32 - shiftCount);
            reg32Dest |= sourceShiftData;
        }
        /// <summary>
        /// 双精度左移
        /// </summary>
        /// <param name="reg32Dest">目标操作数</param>
        /// <param name="reg32Sour">源操作数</param>
        /// <param name="shiftCount">位移次数</param>
        /// <remarks>shld reg32,reg32,cl/imm8</remarks>
        public static void SHLD(ref uint reg32Dest, uint reg32Sour, byte shiftCount)
        {
            shiftCount %= 32;           //有效位移次数
            reg32Dest <<= shiftCount;
            //获取源操作数位移数据掩码
            uint mask = 0;
            uint bitSet = 0x80000000;
            for (int shiftLoop = 0; shiftLoop < shiftCount; shiftLoop++)
            {
                mask |= bitSet;
                bitSet >>= 1;
            }
            //获取源操作数数据
            uint sourceShiftData = reg32Sour & mask;
            //将源操作数位移数据复制到目标操作数
            sourceShiftData >>= (32 - shiftCount);
            reg32Dest |= sourceShiftData;
        }
    }
}
