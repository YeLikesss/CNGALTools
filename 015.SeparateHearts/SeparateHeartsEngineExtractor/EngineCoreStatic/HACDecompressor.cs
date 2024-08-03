using System;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using ICSharpCode.SharpZipLib.BZip2;

namespace EngineCoreStatic
{
    /// <summary>
    /// HAC解压缩算法
    /// </summary>
    public class HACDecompressor
    {
        /// <summary>
        /// Lzma解压
        /// </summary>
        /// <param name="inStream">输入流</param>
        /// <param name="outStream">输出流</param>
        /// <param name="inLength">输入长度</param>
        /// <param name="outLength">输出长度</param>
        public static unsafe void DecompressLzma(Stream inStream, Stream outStream, long inLength, long outLength)
        {
            long inSize = inLength - 9;

            int propLen = 0;
            byte[] properties = new byte[5];

            inStream.Seek(-9L, SeekOrigin.End);
            inStream.Read(new Span<byte>(&propLen, sizeof(int)));
            inStream.Read(properties);

            inStream.Seek(0L,SeekOrigin.Begin);

            SevenZip.Compression.LZMA.Decoder decoder = new();
            decoder.SetDecoderProperties(properties);
            decoder.Code(inStream, outStream, inSize, outLength, null);
        }

        /// <summary>
        /// RLE解压图像
        /// </summary>
        /// <param name="inStream">输入流</param>
        /// <param name="outStream">输出流</param>
        /// <param name="inLength">输入长度</param>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        /// <param name="channel">通道数</param>
        /// <returns>解压后长度</returns>
        public static long DecompressRLE(Stream inStream, Stream outStream, long inLength, int width, int height, int channel)
        {
            int pixelCount = width * height;

            using MemoryStream bufStream = new(pixelCount * channel);
            using BinaryReader bufReader = new(bufStream, Encoding.Unicode, true);

            long outLength = HACDecompressor.DecompressRLE(inStream, bufStream, inLength);

            //[RRRR..GGGG..BBBB..AAAA]摆列转为[RGBA..RGBA]排列
            bufStream.Seek(0L, SeekOrigin.Begin);
            byte[] outBuf = new byte[pixelCount * channel];
            for(int c = 0; c < channel; ++c)
            {
                for(int i = 0; i < pixelCount; ++i)
                {
                    outBuf[i * channel + c] = bufReader.ReadByte();
                }
            }
            outStream.Write(outBuf);

            return outLength;
        }

        /// <summary>
        /// RLE解压
        /// </summary>
        /// <param name="inStream">输入流</param>
        /// <param name="outStream">输出流</param>
        /// <param name="inLength">输入长度</param>
        /// <returns>解压后长度</returns>
        public static unsafe long DecompressRLE(Stream inStream, Stream outStream, long inLength)
        {
            long inStartPos = inStream.Position;
            long outStartPos = outStream.Position;

            using BinaryReader inBr = new(inStream, Encoding.Unicode, true);
            using BinaryWriter outBw = new(outStream, Encoding.Unicode, true);

            while (inStream.Position < inStartPos + inLength)
            {
                int selector = inBr.ReadInt16();
                if (selector > 0)
                {
                    //重复字节
                    byte repeatByte = inBr.ReadByte();
                    
                    for(int i = 0; i < selector; ++i)
                    {
                        outBw.Write(repeatByte);
                    }
                }
                else
                {
                    //无法压缩的数据
                    int count = -selector;
                    for(int i = 0; i < count; ++i)
                    {
                        byte data = inBr.ReadByte();
                        outBw.Write(data);
                    }
                }
            }

            return outStream.Position - outStartPos;
        }

        /// <summary>
        /// Bzip2解压
        /// </summary>
        /// <param name="inStream">输入流</param>
        /// <param name="outStream">输出流</param>
        /// <returns>解压后长度</returns>
        public static long DecompressBzip2(Stream inStream, Stream outStream)
        {
            long outStartPos = outStream.Position;

            using BZip2InputStream bzip2Stream = new(inStream);
            bzip2Stream.CopyTo(outStream);

            return outStream.Position - outStartPos;
        }

        /// <summary>
        /// Ryc解压
        /// </summary>
        /// <param name="inStream">输入流</param>
        /// <param name="outStream">输出流</param>
        /// <param name="inLength">输入长度</param>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        /// <param name="channel">通道数</param>
        /// <returns>解压后长度</returns>
        public static long DecompressRyc(Stream inStream, Stream outStream, long inLength, int width, int height, int channel)
        {
            if(channel < 3)
            {
                return HACDecompressor.DecompressRLE(inStream, outStream, inLength);
            }

            if(channel == 3)
            {
                using MemoryStream rycStream = new();

                //RLE -> RYC
                HACDecompressor.DecompressRLE(inStream, rycStream, inLength);
                rycStream.Seek(0L, SeekOrigin.Begin);

                //RYC-> OGL(RGB) 3 Channel
                HACRycCompressor.Decompress(rycStream, outStream, width, height);
            }
            else if(channel == 4)
            {
                using BinaryReader inBr = new(inStream, Encoding.Unicode, true);

                using MemoryStream rgbStream = new(width * height * 3);
                using MemoryStream alphaStream = new(width * height);

                //RLE -> RYC -> OGL(RGB) 3 Channel
                {
                    using MemoryStream rycStream = new();

                    long inStartPos = inStream.Position;
                    uint segmLength = inBr.ReadUInt32();

                    //RLE -> RYC
                    HACDecompressor.DecompressRLE(inStream, rycStream, segmLength);
                    rycStream.Seek(0L, SeekOrigin.Begin);

                    //RYC-> OGL(RGB) 3 Channel
                    HACRycCompressor.Decompress(rycStream, rgbStream, width, height);
                    rgbStream.Seek(0L, SeekOrigin.Begin);

                    inStream.Seek(inStartPos + segmLength + 4, SeekOrigin.Begin);
                }

                //RLE -> OGL(A) 1 Channel
                {
                    uint segmLength = inBr.ReadUInt32();

                    HACDecompressor.DecompressRLE(inStream, alphaStream, segmLength);
                    alphaStream.Seek(0L, SeekOrigin.Begin);
                }

                //通道混合
                {
                    Span<byte> rgba = stackalloc byte[4];
                    Span<byte> rgb = rgba[..3];
                    Span<byte> a = rgba[3..4];

                    int pixelCount = width * height;
                    for (int i = 0; i < pixelCount; ++i)
                    {
                        rgbStream.Read(rgb);
                        alphaStream.Read(a);

                        outStream.Write(rgba);
                    }
                }
            }
            return width * height * channel;
        }
    }

    /// <summary>
    /// Ryc压缩
    /// </summary>
    public class HACRycCompressor
    {
        /// <summary>
        /// 解压
        /// </summary>
        /// <param name="inStream">输入流</param>
        /// <param name="outStream">输出流</param>
        /// <param name="width">输入长度</param>
        /// <param name="height">高度</param>
        public static void Decompress(Stream inStream, Stream outStream, int width, int height)
        {
            /*
             * 压缩数据长度 1.5x width*height
             * 解压数据长度 3x width*height
             * 
             * 压缩节点 4(2x2 矩阵)+1+1   4字节+2选择子
             *  
             *  byte , byte
             *  byte , byte
             *  
             * 解压节点 12(2x2 矩阵)   12字节 4个rgb像素
             * 
             *  rgb , rgb
             *  rgb , rgb
             * 
             */

            int alignW = width % 2 == 0 ? width : width + 1;
            int alignH = height % 2 == 0 ? height : height + 1;
            int compressDataCount = alignW * alignH;
            int selectDataCount = compressDataCount / 4;

            byte[] compressData = new byte[compressDataCount];
            byte[] selectData1 = new byte[selectDataCount];
            byte[] selectData2 = new byte[selectDataCount];

            inStream.Read(compressData);
            inStream.Read(selectData1);
            inStream.Read(selectData2);

            int pixelCount = width * height;
            if (pixelCount != 0)
            {
                byte[] outPixelData = new byte[pixelCount * 3];

                Span<int> t1 = MemoryMarshal.Cast<byte, int>(HACRycCompressor.Table_1);
                Span<int> t3 = MemoryMarshal.Cast<byte, int>(HACRycCompressor.Table_3);
                Span<int> t5 = MemoryMarshal.Cast<byte, int>(HACRycCompressor.Table_5);

                Span<byte> t4 = HACRycCompressor.Table_4;

                Span<byte> g1 = HACRycCompressor.Table_G1;
                Span<byte> g2 = HACRycCompressor.Table_G2;

                //2x2 矩阵压缩数据 -> 2x2 矩阵rgb像素
                int nodeIndex = 0;
                for (int matrixY = 0; matrixY < alignH; matrixY += 2)
                {
                    for (int matrixX = 0; matrixX < alignW; matrixX += 2)
                    {
                        byte selector1 = selectData1[nodeIndex];
                        byte selector2 = selectData2[nodeIndex];

                        int tableOffset = t5[selector1] + t1[selector2];

                        Span<byte> segm1 = g1.Slice(0x100 * selector1, 0x100);
                        Span<byte> segm2 = g2.Slice(0x100 * selector2, 0x100);

                        for (int y = 0; y < 2; ++y)
                        {
                            for (int x = 0; x < 2; ++x)
                            {
                                int offsetY = matrixY + y;
                                int offsetX = matrixX + x;

                                byte v = compressData[offsetY * alignW + offsetX];

                                if (offsetX < width && offsetY < height)
                                {
                                    int position = (offsetY * width + offsetX) * 3;

                                    outPixelData[position + 0] = segm2[v];
                                    outPixelData[position + 1] = t4[t3[v] - tableOffset + 0x180];
                                    outPixelData[position + 2] = segm1[v];
                                }
                            }
                        }

                        ++nodeIndex;
                    }
                }

                outStream.Write(outPixelData);
            }
        }


        /// <summary>
        /// RVA 0x10AC8 长度0x10000
        /// </summary>
        public static byte[] Table_G1 { get; private set; } = new byte[0x10000];

        /// <summary>
        /// RVA 0x20AC8 长度0x400
        /// </summary>
        public static byte[] Table_1 { get; private set; } = new byte[0x400];
        /// <summary>
        /// RVA 0x20EC8 长度0x400
        /// </summary>
        public static byte[] Table_2 { get; private set; } = new byte[0x400];
        /// <summary>
        /// RVA 0x212C8 长度0x400
        /// </summary>
        public static byte[] Table_3 { get; private set; } = new byte[0x400];

        /// <summary>
        /// RVA 0x216C8 长度0x10000
        /// </summary>
        public static byte[] Table_G2 { get; private set; } = new byte[0x10000];

        /// <summary>
        /// RVA 0x316C8 长度0x400
        /// </summary>
        public static byte[] Table_4 { get; private set; } = new byte[0x400];

        /// <summary>
        /// RVA 0x31AC8 长度0x400
        /// </summary>
        public static byte[] Table_5 { get; private set; } = new byte[0x400];
        /// <summary>
        /// RVA 0x31EC8 长度0x400
        /// </summary>
        public static byte[] Table_6 { get; private set; } = new byte[0x400];

        /// <summary>
        /// 静态构造
        /// </summary>
        static HACRycCompressor()
        {
            {
                int v1 = -26496;
                int v2 = -65920;
                int v3 = -4768;
                int v5 = -12672;
                int v6 = -52224;

                Span<int> t1 = MemoryMarshal.Cast<byte, int>(HACRycCompressor.Table_1);
                Span<int> t2 = MemoryMarshal.Cast<byte, int>(HACRycCompressor.Table_2);
                Span<int> t3 = MemoryMarshal.Cast<byte, int>(HACRycCompressor.Table_3);
                Span<int> t5 = MemoryMarshal.Cast<byte, int>(HACRycCompressor.Table_5);
                Span<int> t6 = MemoryMarshal.Cast<byte, int>(HACRycCompressor.Table_6);

                int index = 0;
                do
                {
                    t1[index] = v1 >> 8;
                    t2[index] = v2 >> 8;
                    t3[index] = v3 >> 8;
                    t5[index] = v5 >> 8;
                    t6[index] = v6 >> 8;

                    v1 += 208;
                    v2 += 516;
                    v3 += 298;
                    v5 += 100;
                    v6 += 409;

                    ++index;
                }
                while (v3 < 71520);
            }

            {
                Span<byte> t4 = HACRycCompressor.Table_4;

                t4[0..0x180].Fill(0x00);

                for(int i = 0x180; i < 0x280; ++i)
                {
                    t4[i] = (byte)(i - 0x180);
                }

                t4[0x280..].Fill(0xFF);
            }

            {
                int index = 0;

                byte[] g1 = HACRycCompressor.Table_G1;
                byte[] g2 = HACRycCompressor.Table_G2;

                Span<int> t2 = MemoryMarshal.Cast<byte, int>(HACRycCompressor.Table_2);
                Span<int> t3 = MemoryMarshal.Cast<byte, int>(HACRycCompressor.Table_3);
                Span<int> t6 = MemoryMarshal.Cast<byte, int>(HACRycCompressor.Table_6);

                Span<byte> t4 = HACRycCompressor.Table_4;

                for (int i = 0; i < 256; ++i)
                {
                    int v1 = t2[i];
                    int v2 = t6[i];

                    for(int j = 0; j < 256; ++j)
                    {
                        int selector = t3[j];

                        g1[index] = t4[v1 + 0x180 + selector];
                        g2[index] = t4[v2 + 0x180 + selector];

                        ++index;
                    }
                }
            }
        }
    }
}
