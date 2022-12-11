using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BKEngine.V21;

namespace BKEngine.V20
{
    /// <summary>
    /// 文件资源表
    /// </summary>
    public class BKARCList
    {
        /// <summary>
        /// 解析文件中列表信息结构
        /// </summary>
        /// <param name="filelistinfodata">文件中列表数据</param>
        /// <param name="listInfo"></param>
        public static void FileListInfoAnalysis(byte[] filelistinfodata,out FilePackageListInfo listInfo)
        {
            listInfo.ListDataSize = BitConverter.ToUInt32(filelistinfodata, 0);
            listInfo.ListCount = BitConverter.ToUInt32(filelistinfodata, 4);
            listInfo.ListDecryptKey = BitConverter.ToUInt32(filelistinfodata, 8);
        }

        /// <summary>
        /// 解密列表数据
        /// </summary>
        /// <param name="listdata">文件中列表原数据</param>
        /// <param name="listdecryptkey">文件表解密key</param>
        /// <param name="filekey">文件key</param>
        /// <returns>解密后的文件列表压缩数据</returns>
        public static byte[] DecryptList(byte[] listdata,uint listdecryptkey,out uint filekey)
        {
            //解密列表且计算得到文件key
            filekey = DecryptHelper.DecryptList(listdata, (uint)listdata.Length, listdecryptkey);
            return BZip2Helper.DecompressData(listdata);            //解压数据并返回
        }

        /// <summary>
        /// 文件表解析
        /// </summary>
        /// <param name="metalistdata">文件表原数据</param>
        /// <param name="listcount">列表项项数</param>
        /// <param name="compressedreslist">压缩资源表数组</param>
        /// <param name="normalreslist">普通资源表数组</param>
        public static void ListAnalysis(byte[] metalistdata, uint listcount, out List<BZip2CompressedResources> compressedreslist, out List<NormalResources> normalreslist)
        {
            uint count = 0;                //表项计数
            BZip2CompressedResources compressedRes;
            NormalResources normalRes;
            List<BZip2CompressedResources> mCompressedReslist = new List<BZip2CompressedResources>();
            List<NormalResources> mNormalReslist = new List<NormalResources>();

            //初始化列表指针
            uint listPointer = 0;

            while (count < listcount)
            {
                //储存字符串长度
                uint utf8strLength;
                //获取文件名
                string fileName = StructureConvert.GetUTF8String(metalistdata, listPointer,out utf8strLength);

                //列表指针移到字符串结束符之后
                listPointer += utf8strLength+1;

                uint type = BitConverter.ToUInt32(metalistdata, (int)(listPointer + 0x8));
                if (type == 0)

                {   //当列表项为普通资源时
                    normalRes.FileName = fileName;
                    normalRes.FileOffset = BitConverter.ToUInt32(metalistdata, (int)(listPointer));
                    normalRes.FileSize = BitConverter.ToUInt32(metalistdata, (int)(listPointer + 0x4));
                    normalRes.ResourcesType = 0;
                    mNormalReslist.Add(normalRes);        //添加数组
                    listPointer += 0x0C;
                }
                else if (type == 1)

                {   //当列表项为压缩资源时
                    compressedRes.FileName= fileName;
                    compressedRes.FileOffset = BitConverter.ToUInt32(metalistdata, (int)(listPointer));
                    compressedRes.UncompressedSize = BitConverter.ToUInt32(metalistdata, (int)(listPointer + 0x4));
                    compressedRes.ResourcesType = 1;
                    compressedRes.FileSize = BitConverter.ToUInt32(metalistdata, (int)(listPointer + 0xC));
                    mCompressedReslist.Add(compressedRes);  //添加数组
                    listPointer += 0x10;
                }
                count++;            //表项计数自增
            }
            compressedreslist = mCompressedReslist;     //解析得资源表项
            normalreslist = mNormalReslist;
        }
    }
}
