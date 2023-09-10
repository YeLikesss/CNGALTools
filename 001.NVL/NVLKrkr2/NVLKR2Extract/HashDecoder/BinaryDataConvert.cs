using System;
using System.Collections.Generic;
using System.Buffers;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Utils.Binary
{
    /// <summary>
    /// 数组转换相关
    /// </summary>
    public class BinaryDataConvert
    {
        /// <summary>
        /// 16进制转字符串
        /// </summary>
        /// <param name="data">数据流</param>
        /// <returns></returns>
        public static string HexToString(Span<byte> data)
        {
            string hexMap = "0123456789ABCDEF";

            char[] tempS = ArrayPool<char>.Shared.Rent(data.Length * 2);
            for(int i = 0; i < data.Length; ++i)
            {
                tempS[2 * i + 0] = hexMap[(data[i] & 0xF0) >> 4];
                tempS[2 * i + 1] = hexMap[(data[i] & 0x0F) >> 0];
            }
            string s = new (tempS, 0, data.Length * 2);
            ArrayPool<char>.Shared.Return(tempS);

            return s;
        }

        /// <summary>
        /// 字符串转16进制流
        /// </summary>
        /// <param name="hexString">字符串</param>
        /// <returns></returns>
        public static byte[] StringToHexBytes(string hexString)
        {
            byte[] buffer = new byte[hexString.Length / 2];
            for (int i = 0; i < buffer.Length; ++i)
            {
                buffer[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }
            return buffer;
        }

        /// <summary>
        /// 获取结构体的字节数据
        /// </summary>
        /// <typeparam name="T">结构体类型</typeparam>
        /// <param name="structure">结构体对象</param>
        /// <returns></returns>
        public static byte[] GetStructureBytes<T>(T structure)
        {
            int size = Marshal.SizeOf(structure);           
            byte[] data = new byte[size];                      
            IntPtr unmanagedMemory = Marshal.AllocHGlobal(size);        
            Marshal.StructureToPtr(structure, unmanagedMemory, true);       
            Marshal.Copy(unmanagedMemory, data, 0, size);              
            Marshal.FreeHGlobal(unmanagedMemory);                
            return data;
        }

        /// <summary>
        /// 获取结构体的字节数据
        /// </summary>
        /// <typeparam name="T">结构体类型</typeparam>
        /// <param name="structure">结构体对象</param>
        /// <param name="retData">返回值数据指针</param>
        /// <returns></returns>
        unsafe public static void GetStructureBytes<T>(T structure, Span<byte> retData)
        {
            int size = Marshal.SizeOf(structure);
            IntPtr unmanagedMemory = Marshal.AllocHGlobal(size);
            
            Marshal.StructureToPtr(structure, unmanagedMemory, true);

            Span<byte> memPtr = new(unmanagedMemory.ToPointer(), size);
            memPtr.CopyTo(retData);
            
            Marshal.FreeHGlobal(unmanagedMemory);
        }
    }
}
