using System;
using System.Text;

namespace GameCreatorStatic.Extractor.V1
{
    public class GCCryptoV1
    {
        /// <summary>
        /// 变换密码
        /// </summary>
        /// <param name="password">密码</param>
        /// <returns>新密码</returns>
        public static string TransformPasswordV1(string password)
        {
            string[] pws = password.Split('|');
            StringBuilder sb = new(1024);
            foreach(string s in pws)
            {
                sb.Append((char)(s[0] - 1));
            }
            return sb.ToString();
        }
    }
}
