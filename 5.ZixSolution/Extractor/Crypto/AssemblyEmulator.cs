using System;
using System.Collections.Generic;


namespace Extractor
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
        public static void ROL(ref uint reg32, uint shiftCount)
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
        public static void ROR(ref uint reg32, uint shiftCount)
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
    }
}
