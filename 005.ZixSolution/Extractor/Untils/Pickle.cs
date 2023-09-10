using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Razorvine.Pickle.Objects;
using Razorvine.Pickle;

namespace Extractor.Untils
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
            Unpickler unpickler = new();
            object result = unpickler.loads(data);
            return result;
        }
    }
}
