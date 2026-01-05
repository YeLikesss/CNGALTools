using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.Json;

namespace EngineCore
{
    /// <summary>
    /// 序列化
    /// </summary>
    public class YuriSerializer
    {
        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns>成功:对象 失败:null</returns>
        public static object? Deserialize(Stream stream)
        {
            object? obj;
            try
            {
                BinaryFormatter binaryFormatter = new();
                obj = binaryFormatter.Deserialize(stream);
            }
            catch (Exception ex)
            {
                obj = null;
            }
            return obj;
        }
    }
}
