using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BKEngine.V21
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
        /// <param name="listdecryptkey">文件表解密Key</param>
        /// <param name="filekey">文件解密key</param>
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
        public static void ListAnalysis(byte[] metalistdata,uint listcount,out List<BZip2CompressedResources> compressedreslist,out List<NormalResources> normalreslist)
        {
            uint index = 0;                 //数据索引
            uint count = 0;                //表项计数
            BZip2CompressedResources compressedRes;
            NormalResources normalRes;
            List<BZip2CompressedResources> mCompressedReslist = new List<BZip2CompressedResources>();
            List<NormalResources> mNormalReslist = new List<NormalResources>();

            while (count < listcount)
            {
                uint type = BitConverter.ToUInt32(metalistdata, (int)(index + 0xC));
                if (type == 0)
                {   //当列表项为普通资源时
                    normalRes.FileNameHash = BitConverter.ToUInt32(metalistdata, (int)index);
                    normalRes.FileOffset = BitConverter.ToUInt32(metalistdata, (int)(index + 0x4));
                    normalRes.FileSize = BitConverter.ToUInt32(metalistdata, (int)(index + 0x8));
                    normalRes.ResourcesType = 0;
                    mNormalReslist.Add(normalRes);        //添加数组
                    index += 0x10;
                }
                else if (type == 1)
                {   //当列表项为压缩资源时
                    compressedRes.FileNameHash = BitConverter.ToUInt32(metalistdata, (int)index);
                    compressedRes.FileOffset = BitConverter.ToUInt32(metalistdata, (int)(index + 0x4));
                    compressedRes.UncompressedSize = BitConverter.ToUInt32(metalistdata, (int)(index + 0x8));
                    compressedRes.ResourcesType = 1;
                    compressedRes.FileSize = BitConverter.ToUInt32(metalistdata, (int)(index + 0x10));
                    mCompressedReslist.Add(compressedRes);  //添加数组
                    index += 0x14;
                }
                count++;            //表项计数自增
            }
            compressedreslist = mCompressedReslist;     //解析得资源表项
            normalreslist = mNormalReslist;
        }
    }
}
