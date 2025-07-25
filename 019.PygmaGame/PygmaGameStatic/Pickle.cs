using System;
using Razorvine.Pickle;

namespace PygmaGameStatic
{
    public class Pickle
    {
        /// <summary>
        /// 反序列化Python的Pickle格式
        /// </summary>
        /// <param name="data">序列化数据</param>
        /// <returns></returns>
        public static object Decode(byte[] data)
        {
            using Unpickler unpickler = new();
            object result = unpickler.loads(data);
            return result;
        }
    }
}
