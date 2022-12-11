using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace BKEngine.V21
{
    public class Encrypt
    {
        /// <summary>
        /// 加密文件名
        /// </summary>
        /// <param name="filename">文件名</param>
        public static uint EncryptFileName(string filename)
        {
            byte[] filenamekey = Encoding.UTF8.GetBytes(filename);    
            uint encryptedkey = DecryptHelper.constKey1;   
            uint seed =  DecryptHelper.constKey2;             
            uint uintfilename;
            foreach (byte temp in filenamekey)
            {
                if (temp == 0)
                {   
                    //字符串结束null判断
                    break;
                }
                uintfilename = Convert.ToUInt32(temp);
                uintfilename = uintfilename ^ encryptedkey;
                encryptedkey = (uint)((int)uintfilename * (int)seed); 
            }
            return encryptedkey;
        }
    }
}
