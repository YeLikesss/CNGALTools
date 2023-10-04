using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightVNStatic
{
    /// <summary>
    /// 封包接口
    /// </summary>
    public interface IPackage
    {
        /// <summary>
        /// 解包
        /// </summary>
        public bool Extract();
    }
}
