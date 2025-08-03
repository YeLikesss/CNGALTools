
using System;
using System.Collections;
using System.IO;
using System.Text;
using ICSharpCode.SharpZipLib.Zip;

namespace GameCreatorStatic
{
    /// <summary>
    /// Zip类
    /// </summary>
    public class ZipStorage
    {
        /// <summary>
        /// 文件解压
        /// </summary>
        /// <param name="stream">文件流</param>
        /// <param name="encoding">编码</param>
        /// <param name="password">密码</param>
        /// <param name="outputDiretory">输出目录</param>
        /// <param name="error">错误信息</param>
        /// <returns>True解压成功 False解压失败</returns>
        public static bool Decompress(Stream stream, Encoding encoding, string password, string outputDiretory, out string error)
        {
            try
            {
                using ZipFile zip = new(stream, true, StringCodec.FromEncoding(encoding));
                if (!string.IsNullOrEmpty(password))
                {
                    zip.Password = password;
                }

                //解压文件
                foreach (ZipEntry entry in zip)
                {
                    string outPath = Path.Combine(outputDiretory, entry.Name);
                    {
                        string outDir = Path.GetDirectoryName(outPath)!;
                        if (!Directory.Exists(outDir))
                        {
                            Directory.CreateDirectory(outDir);
                        }
                    }

                    using FileStream outFs = File.Create(outPath);
                    using Stream inStream = zip.GetInputStream(entry.ZipFileIndex);

                    inStream.CopyTo(outFs);
                    outFs.Flush();
                }

                error = string.Empty;
                return true;
            }
            catch(Exception e)
            {
                error = e.Message;
                return false;
            }
        }
    }
}
