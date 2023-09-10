using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;

namespace Snowing
{
    /// <summary>
    /// 图像资源相关
    /// </summary>
    public class TextureArchive
    {
        /// <summary>
        /// 图像资源格式
        /// </summary>
        public enum Format:byte
        {
            /// <summary>
            /// PNG图像
            /// </summary>
            PNG=0,
            /// <summary>
            /// PNG图像 多一个重复的Red通道
            /// </summary>
            PNGR8=1,
            /// <summary>
            /// DDS图像
            /// </summary>
            DDS=2
        }
        /// <summary>
        /// 文件头
        /// </summary>
        [StructLayout(LayoutKind.Explicit,Pack =1)]
        public struct Header
        {
            /// <summary>
            /// 格式
            /// </summary>
            [FieldOffset(0)]
            public Format Format;
            /// <summary>
            /// 图像宽度
            /// </summary>
            [FieldOffset(1)]
            public ushort Width;
            /// <summary>
            /// 图像高度
            /// </summary>
            [FieldOffset(3)]
            public ushort Heigth;
        }
        /// <summary>
        /// 转化图像文件
        /// </summary>
        /// <param name="dataInfo"></param>
        /// <param name="key">AES128Key</param>
        /// <param name="iv">AES128IV</param>
        /// <returns></returns>
        public static Archive.DataInfo ConvertTexture(Archive.DataInfo dataInfo,byte[] key,byte[] iv)
        {
            //获取头
            Header header= StructureConvert.GetStructure<Header>(dataInfo.Data);

            //获取数据
            byte[] decryptData=new byte[dataInfo.Data.Length];
            dataInfo.Data.CopyTo(decryptData, 0);

            //检查数据是否加密
            if(((header.Format==Format.PNG|| header.Format == Format.PNGR8 || header.Format == Format.DDS)&&(header.Width%4==0&&header.Heigth%4==0))==false)
            {
                //解密数据
                decryptData = AesHelper.AesDecrypt128(dataInfo.Data, key, iv);
                //重新获取头
                header = StructureConvert.GetStructure<Header>(decryptData);
            }

            Archive.DataInfo newDataInfo = new();
            //转化为图像
            switch (header.Format)
            {
                case Format.PNG:
                    //设置数据起始点
                    int indexPNG = decryptData.Length - (header.Width * header.Heigth*4);
                    //创建图片
                    Bitmap bitmapPNG = new(header.Width, header.Heigth, PixelFormat.Format32bppPArgb);
                    //设置像素
                    for (int y = 0; y < header.Heigth; y++)
                    {
                        for(int x = 0; x < header.Width; x++)
                        {
                            //4byte转1个像素
                            Color colorPNG = Color.FromArgb(decryptData[indexPNG + 3], decryptData[indexPNG], decryptData[indexPNG + 1], decryptData[indexPNG + 2]);
                            bitmapPNG.SetPixel(x, y, colorPNG);
                            indexPNG += 4;
                        }
                    }

                    //保存为png格式
                    Image imagePNG = bitmapPNG;
                    MemoryStream msPNG = new();
                    imagePNG.Save(msPNG, ImageFormat.Png);

                    //保存数据并设置路径
                    newDataInfo.Data = msPNG.ToArray();
                    newDataInfo.FileName = Path.ChangeExtension(dataInfo.FileName, ".png");

                    break;

                case Format.PNGR8:
                    //设置数据起始点
                    int indexPNGR8 = decryptData.Length - (header.Width * header.Heigth*5);
                    //创建图片
                    Bitmap bitmapPNGR8 = new(header.Width, header.Heigth, PixelFormat.Format32bppPArgb);
                    //设置像素
                    for (int y = 0; y < header.Heigth; y++)
                    {
                        for (int x = 0; x < header.Width; x++)
                        {
                            //4byte转1个像素
                            Color color = Color.FromArgb(decryptData[indexPNGR8 + 3], decryptData[indexPNGR8], decryptData[indexPNGR8 + 1], decryptData[indexPNGR8 + 2]);
                            bitmapPNGR8.SetPixel(x, y, color);
                            indexPNGR8 += 5;
                        }
                    }

                    //保存为png格式
                    Image imagePNGR8 = bitmapPNGR8;
                    MemoryStream msPNGR8 = new();
                    imagePNGR8.Save(msPNGR8, ImageFormat.Png);

                    //保存数据并设置路径
                    newDataInfo.Data = msPNGR8.ToArray();
                    newDataInfo.FileName = Path.ChangeExtension(dataInfo.FileName, ".png");

                    break;

                case Format.DDS:

                    //寻找DDS头
                    int indexDDS = MemorySearch.IndexOfFirst(decryptData, 0, new byte[] { 0x44, 0x44, 0x53, 0x20 });

                    //移除原资源头
                    List<byte> bufferDDS = decryptData.ToList();
                    bufferDDS.RemoveRange(0, indexDDS);

                    //保存数据并设置路径
                    newDataInfo.Data = bufferDDS.ToArray();
                    newDataInfo.FileName = Path.ChangeExtension(dataInfo.FileName, ".dds");

                    break;
            }
            return newDataInfo;
        }
    }
}
