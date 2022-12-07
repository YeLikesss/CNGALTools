using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;

namespace Snowing
{
    public class StructureConvert
    {
        /// <summary>
        /// 将字节流数据转化为结构体类型
        /// </summary>
        /// <typeparam name="T">结构体类型</typeparam>
        /// <param name="data">字节流数据</param>
        /// <param name="offset">起始偏移</param>
        /// <returns>指定类型结构体数据</returns>
        public static T GetStructure<T>(byte[] data, int offset = 0)
        {
            int size = Marshal.SizeOf(typeof(T));                       //获取结构体大小
            IntPtr unmanagedMemory = Marshal.AllocHGlobal(size);       //申请与结构体大小相同的内存
            if (size > data.Length)
            {
                //结构体大小大于要转化的数据长度
                size = Math.Min(data.Length, size);         //取最小
            }
            Marshal.Copy(data, offset, unmanagedMemory, size);          //数据复制到非托管内存中
            var structure = (T)Marshal.PtrToStructure(unmanagedMemory, typeof(T));       //指定数据转型为结构体
            Marshal.FreeHGlobal(unmanagedMemory);                   //释放内存
            return structure;
        }
    }
}
