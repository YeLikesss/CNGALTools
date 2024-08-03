using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace EngineCoreStatic
{
    /// <summary>
    /// 图像压缩算法
    /// </summary>
    public enum HACImageCompress : uint
    {
        Unknow = 0u,
        /// <summary>
        /// 未压缩
        /// </summary>
        NoCompress = 1u,
        /// <summary>
        /// RLE压缩算法
        /// </summary>
        RLE = 2u,
        /// <summary>
        /// Bzip2压缩算法
        /// </summary>
        Bzip2 = 3u,
        /// <summary>
        /// RYC压缩算法
        /// </summary>
        RYC = 4u,
    }

    /// <summary>
    /// 图像结构
    /// </summary>
    public class HACImageEntry
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name = string.Empty;
        /// <summary>
        /// 数据
        /// </summary>
        public byte[] Data = Array.Empty<byte>();

        /// <summary>
        /// 子项
        /// </summary>
        public List<HACImageEntry> SubEntries = new();

        /// <summary>
        /// 转化为Bool
        /// </summary>
        public bool AsBool()
        {
            return this.Data[0] != 0;
        }

        /// <summary>
        /// 转为UInt8
        /// </summary>
        public byte AsUInt8()
        {
            return this.Data[0];
        }

        /// <summary>
        /// 转为Int8
        /// </summary>
        public sbyte AsInt8()
        {
            return (sbyte)this.AsUInt8();
        }

        /// <summary>
        /// 转为UInt16
        /// </summary>
        public ushort AsUInt16()
        {
            return BitConverter.ToUInt16(this.Data);
        }

        /// <summary>
        /// 转为Int16
        /// </summary>
        public short AsInt16()
        {
            return (short)this.AsUInt16();
        }

        /// <summary>
        /// 转为UInt32
        /// </summary>
        public uint AsUInt32()
        {
            return BitConverter.ToUInt32(this.Data);
        }

        /// <summary>
        /// 转为Int32
        /// </summary>
        public int AsInt32()
        {
            return (int)this.AsUInt32();
        }

        /// <summary>
        /// 转为UInt64
        /// </summary>
        public ulong AsUInt64()
        {
            return BitConverter.ToUInt64(this.Data);
        }

        /// <summary>
        /// 转为Int64
        /// </summary>
        public long AsInt64()
        {
            return (long)this.AsUInt64();
        }

        /// <summary>
        /// 转为32位浮点
        /// </summary>
        public float AsSingle()
        {
            return BitConverter.ToSingle(this.Data);
        }

        /// <summary>
        /// 转为64位浮点
        /// </summary>
        public double AsDouble()
        {
            return BitConverter.ToDouble(this.Data);
        }

        /// <summary>
        /// 转为字符串
        /// </summary>
        public string AsString()
        {
            using MemoryStream ms = new(this.Data, false);
            return HACStreamExtend.ReadString(ms);
        }

        /// <summary>
        /// 转为字节数组
        /// </summary>
        /// <param name="dest">目标</param>
        /// <param name="length">需要长度</param>
        public void AsArray(in Span<byte> dest, int length)
        {
            this.Data.AsSpan()[..length].CopyTo(dest);
        }

        /// <summary>
        /// 寻找子节点
        /// </summary>
        /// <param name="name">节点名称</param>
        /// <returns>节点对象</returns>
        public HACImageEntry? Find(string name)
        {
            return this.SubEntries.FirstOrDefault(e => e.Name == name);
        }

        /// <summary>
        /// 结构解析
        /// </summary>
        /// <param name="stream">输入流</param>
        /// <returns>节点对象</returns>
        public static HACImageEntry ParseEntry(Stream stream)
        {
            HACImageEntry entry = new();
            HACImageEntry.ParseEntry(stream, entry);
            return entry;
        }

        /// <summary>
        /// 结构解析
        /// </summary>
        /// <param name="stream">输入流</param>
        /// <param name="node">当前节点</param>
        public static void ParseEntry(Stream stream, HACImageEntry node)
        {
            using BinaryReader br = new(stream, Encoding.Unicode, true);

            node.Name = HACStreamExtend.ReadString(stream);     //解析名称
            node.Data = HACStreamExtend.ReadBytes(stream);      //解析数据

            //解析子项
            int subCount = br.ReadInt32();

            List<HACImageEntry> subItems = node.SubEntries;
            subItems.Capacity = subCount;

            for (int i = 0; i < subCount; ++i)
            {
                HACImageEntry entry = new();
                HACImageEntry.ParseEntry(stream, entry);

                subItems.Add(entry);
            }
        }
    }

    /// <summary>
    /// Tex图像解码
    /// </summary>
    public class HACTexImageDecoder
    {
        /// <summary>
        /// Tex图像信息
        /// </summary>
        private class TexImageInfo
        {
            /// <summary>
            /// 宽度
            /// </summary>
            public int Width;
            /// <summary>
            /// 高度
            /// </summary>
            public int Height;
            /// <summary>
            /// 通道
            /// </summary>
            public int Channel;

            /// <summary>
            /// OpenGL格式
            /// </summary>
            public int GLFormat;

            /// <summary>
            /// 压缩算法
            /// </summary>
            public HACImageCompress CompressType;

            /// <summary>
            /// Alpha通道过滤
            /// </summary>
            public bool EnableAlphaFilter;
            /// <summary>
            /// Alpha过滤像素
            /// </summary>
            public uint AlphaFilterPixel;

            /// <summary>
            /// 像素数据
            /// </summary>
            public byte[] Data = Array.Empty<byte>();
        }

        private readonly TexImageInfo mImageInfo = new();        //图像信息
        private readonly string mFileName = string.Empty;        //文件名

        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName => this.mFileName;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="stream">输入流</param>
        /// <param name="fileName">文件名</param>
        public HACTexImageDecoder(Stream stream, string fileName)
        {
            this.mFileName = fileName;
            this.Initialize(stream);
        }

        /// <summary>
        /// 提取PNG
        /// </summary>
        /// <param name="outputDirectory">输出目录</param>
        /// <returns>图像流</returns>
        public void ExtractToPNG(string outputDirectory)
        {
            string path = Path.Combine(outputDirectory, this.mFileName) + ".png";
            using Stream stream = this.ExtractImage(ImageFormat.Png);
            using FileStream fs = File.Create(path);
            stream.CopyTo(fs);
        }

        /// <summary>
        /// 提取图像
        /// </summary>
        /// <param name="imgFormat">图像格式</param>
        /// <returns>图像流</returns>
        private unsafe Stream ExtractImage(ImageFormat imgFormat)
        {
            Stream returnStream = new MemoryStream();

            TexImageInfo info = this.mImageInfo;
            using Stream rawStream = this.ExtractRAW();
            if(rawStream.Length > 0L)
            {
                using Bitmap bitmap = ImageConverter.OpenGLToGDI32bpp(rawStream, info.Width, info.Height, info.Channel);
                bitmap.Save(returnStream, imgFormat);
            }
            returnStream.Seek(0L, SeekOrigin.Begin);
            return returnStream;
        }

        /// <summary>
        /// 提取图像原数据
        /// </summary>
        /// <returns>图像原始信息</returns>
        private Stream ExtractRAW()
        {
            TexImageInfo info = this.mImageInfo;

            int width = info.Width;
            int height = info.Height;
            int channel = info.Channel;

            HACImageCompress compress = info.CompressType;
            bool enableAlphaFilter = info.EnableAlphaFilter;
            uint alphaFilterPixel = info.AlphaFilterPixel;

            byte[] data = info.Data;

            //输出流
            Stream outStream = new MemoryStream
            {
                Position = 0L
            };

            if (width != 0 && height != 0 && channel != 0)
            {
                using MemoryStream dataStream = new(data, false);
                using MemoryStream orgPixelStream = new(width * height * channel);
                bool isVaild = true;

                //解压缩
                switch (compress)
                {
                    case HACImageCompress.Unknow:
                    {
                        Console.WriteLine("未知的压缩算法: {0}", this.mFileName);
                        isVaild = false;
                        break;
                    }
                    case HACImageCompress.NoCompress:
                    {
                        dataStream.CopyTo(orgPixelStream);
                        break;
                    }
                    case HACImageCompress.RLE:
                    {
                        HACDecompressor.DecompressRLE(dataStream, orgPixelStream, data.LongLength, width, height, channel);
                        break;
                    }
                    case HACImageCompress.Bzip2:
                    {
                        HACDecompressor.DecompressBzip2(dataStream, orgPixelStream);
                        break;
                    }
                }
                orgPixelStream.Seek(0L, SeekOrigin.Begin);

                //解压成功
                if (isVaild)
                {
                    //Alpha像素过滤
                    if (enableAlphaFilter)
                    {
                        if (channel == 1)
                        {
                            ++channel;
                            //增加Alpha通道 1Byte->2Byte
                            outStream.SetLength(width * height * channel);

                            while (orgPixelStream.Position < orgPixelStream.Length)
                            {
                                byte pixel = (byte)orgPixelStream.ReadByte();

                                outStream.WriteByte(pixel);
                                if (pixel == (byte)alphaFilterPixel)
                                {
                                    outStream.WriteByte(0x00);
                                }
                                else
                                {
                                    outStream.WriteByte(0xFF);
                                }
                            }
                        }
                        else if (channel == 3)
                        {
                            ++channel;
                            //增加Alpha通道 3Byte->4Byte
                            outStream.SetLength(width * height * channel);

                            Span<byte> pixel = stackalloc byte[4];
                            Span<byte> bgr = pixel[..3];
                            
                            while (orgPixelStream.Position < orgPixelStream.Length)
                            {
                                orgPixelStream.Read(bgr);

                                pixel[3] = 0x00;
                                if (MemoryMarshal.Read<uint>(pixel) != alphaFilterPixel)
                                {
                                    pixel[3] = 0xFF;
                                }
                                outStream.Write(pixel);
                            }
                        }
                        else
                        {
                            orgPixelStream.CopyTo(outStream);
                        }
                    }
                    else
                    {
                        orgPixelStream.CopyTo(outStream);
                    }
                }
            }

            outStream.Seek(0L, SeekOrigin.Begin);
            return outStream;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="stream">输入流</param>
        private void Initialize(Stream stream)
        {
            HACImageEntry obj = HACImageEntry.ParseEntry(stream);

            TexImageInfo info = this.mImageInfo;
            //宽度
            {
                if (obj.Find("宽") is HACImageEntry item)
                {
                    info.Width = item.AsInt32();
                }
            }

            //高度
            {
                if (obj.Find("高") is HACImageEntry item)
                {
                    info.Height = item.AsInt32();
                }
            }

            //通道
            {
                if (obj.Find("色彩") is HACImageEntry item)
                {
                    info.Channel = item.AsUInt8();
                }
            }

            //opengl格式
            {
                if (obj.Find("推荐格式") is HACImageEntry item)
                {
                    info.GLFormat = item.AsInt32();
                }
            }

            //压缩算法
            {
                HACImageCompress compress = HACImageCompress.Unknow;

                if (obj.Find("压缩算法") is HACImageEntry item)
                {
                    string compressStr = item.AsString();
                    if (string.IsNullOrEmpty(compressStr))
                    {
                        compress = HACImageCompress.NoCompress;
                    }
                    else
                    {
                        switch (compressStr)
                        {
                            case "RLE":
                            {
                                compress = HACImageCompress.RLE;
                                break;
                            }
                            case "Bzip2":
                            {
                                compress = HACImageCompress.Bzip2;
                                break;
                            }
                            default:
                            {
                                Debugger.Break();
                                break;
                            }
                        }
                    }
                }

                info.CompressType = compress;
            }

            //透明度过滤器
            {
                if (obj.Find("透明色") is HACImageEntry item)
                {
                    Span<byte> colorBytes = stackalloc byte[4];
                    colorBytes.Clear();

                    item.AsArray(colorBytes, 3);

                    info.AlphaFilterPixel = MemoryMarshal.Read<uint>(colorBytes);
                    info.EnableAlphaFilter = true;
                }
            }

            //像素数据
            {
                if (obj.Find("象素数据") is HACImageEntry item)
                {
                    info.Data = item.Data;
                }
            }
        }
    }


    public class HACHgpImageDecoder
    {
        /// <summary>
        /// Hgp图像信息
        /// </summary>
        private class HgpImageInfo
        {
            /// <summary>
            /// 层数
            /// </summary>
            public int LayerCount;
            /// <summary>
            /// 宽度
            /// </summary>
            public int Width;
            /// <summary>
            /// 高度
            /// </summary>
            public int Height;
            /// <summary>
            /// 图层
            /// </summary>
            public List<HgpLayerInfo> Layers = new();
        }

        /// <summary>
        /// Hgp图层信息
        /// </summary>
        private class HgpLayerInfo
        {
            /// <summary>
            /// 名称
            /// </summary>
            public string Name = string.Empty;
            /// <summary>
            /// 宽度
            /// </summary>
            public int Width;
            /// <summary>
            /// 高度
            /// </summary>
            public int Height;
            /// <summary>
            /// X偏移
            /// </summary>
            public int OffsetX;
            /// <summary>
            /// Y偏移
            /// </summary>
            public int OffsetY;
            /// <summary>
            /// 通道数
            /// </summary>
            public int Channel;
            /// <summary>
            /// 不透明度
            /// </summary>
            public int Opacity;
            /// <summary>
            /// 压缩方式
            /// </summary>
            public HACImageCompress CompressType;
            /// <summary>
            /// 源混合模式
            /// </summary>
            public OGLRenderEmulator.BlendType SrcBlendMode;
            /// <summary>
            /// 目标混合模式
            /// </summary>
            public OGLRenderEmulator.BlendType DestBlendMode;
            /// <summary>
            /// OpenGL格式
            /// </summary>
            public int GLFormat;
            /// <summary>
            /// 可见性
            /// </summary>
            public bool Visible;
            /// <summary>
            /// 原数据
            /// </summary>
            public byte[] Data = Array.Empty<byte>();
        }

        private readonly HgpImageInfo mImageInfo = new();        //图像信息
        private readonly string mFileName = string.Empty;        //文件名

        /// <summary>
        /// 获取文件名
        /// </summary>
        public string FileName => this.mFileName;

        /// <summary>
        /// 提取图像
        /// </summary>
        /// <param name="outputDirectory">目标文件夹</param>
        public void ExtractPNG(string outputDirectory)
        {
            string baseDir = Path.Combine(outputDirectory, this.mFileName + ".ext");

            List<HgpLayerInfo> layers = this.mImageInfo.Layers;
            for(int i = 0; i < layers.Count; ++i)
            {
                HgpLayerInfo layer = layers[i];

                byte[] compressData = layer.Data;
                int width = layer.Width;
                int height = layer.Height;
                int channel = layer.Channel;
                int opacity = layer.Opacity;
                int offsetX = layer.OffsetX;
                int offsetY = layer.OffsetY;
                HACImageCompress compress = layer.CompressType;

                if (width != 0 && height != 0 && channel != 0)
                {
                    using MemoryStream compressMs = new(compressData, false);
                    using MemoryStream pixelMs = new(width * height * channel);
                    bool isVaild = true;

                    //解压
                    switch (compress)
                    {
                        case HACImageCompress.Unknow:
                        {
                            Console.WriteLine("未知的压缩算法: {0}", this.mFileName);
                            isVaild = false;
                            break;
                        }
                        case HACImageCompress.NoCompress:
                        {
                            compressMs.CopyTo(pixelMs);
                            break;
                        }
                        case HACImageCompress.RLE:
                        {
                            HACDecompressor.DecompressRLE(compressMs, pixelMs, compressData.LongLength, width, height, channel);
                            break;
                        }
                        case HACImageCompress.Bzip2:
                        {
                            HACDecompressor.DecompressBzip2(compressMs, pixelMs);
                            break;
                        }
                        case HACImageCompress.RYC:
                        {
                            HACDecompressor.DecompressRyc(compressMs, pixelMs, compressData.LongLength, width, height, channel);
                            break;
                        }
                    }
                    pixelMs.Seek(0L, SeekOrigin.Begin);

                    //解压成功
                    if (isVaild)
                    {
                        //输出散装图片
                        {
                            string layerPath = Path.Combine(baseDir, layer.Name + ".png");

                            {
                                if(Path.GetDirectoryName(layerPath) is string dir && !Directory.Exists(dir))
                                {
                                    Directory.CreateDirectory(dir);
                                }
                            }

                            using FileStream layerFs = File.Create(layerPath);

                            using Bitmap bitmap = ImageConverter.OpenGLToGDI32bpp(pixelMs, width, height, channel);
                            ImageProcess.ChangeAlpha(bitmap, (byte)opacity);

                            bitmap.Save(layerFs, ImageFormat.Png);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="stream">输入流</param>
        /// <param name="fileName">文件名</param>
        public HACHgpImageDecoder(Stream stream, string fileName)
        {
            this.mFileName = fileName;
            this.Initialize(stream);
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="stream">输入流</param>
        private void Initialize(Stream stream)
        {
            HACImageEntry obj = HACImageEntry.ParseEntry(stream);

            HgpImageInfo info = this.mImageInfo;
            //层数
            {
                if(obj.Find("层次") is HACImageEntry item)
                {
                    info.LayerCount = item.AsInt32();
                }
            }

            //宽度
            {
                if (obj.Find("宽") is HACImageEntry item)
                {
                    info.Width = item.AsInt32();
                }
            }

            //高度
            {
                if (obj.Find("高") is HACImageEntry item)
                {
                    info.Height = item.AsInt32();
                }
            }

            List<HgpLayerInfo> layers = info.Layers;
            layers.Capacity = info.LayerCount;
            for(int i = 0; i < info.LayerCount; ++i)
            {
                HgpLayerInfo layer = new();

                if (obj.Find($"第{i}层") is HACImageEntry layerEntry)
                {
                    //名称
                    {
                        if (layerEntry.Find("名称") is HACImageEntry item)
                        {
                            layer.Name = item.AsString();
                        }
                    }

                    //宽度
                    {
                        if (layerEntry.Find("宽") is HACImageEntry item)
                        {
                            layer.Width = item.AsInt32();
                        }
                    }

                    //高度
                    {
                        if (layerEntry.Find("高") is HACImageEntry item)
                        {
                            layer.Height = item.AsInt32();
                        }
                    }

                    //X
                    {
                        if (layerEntry.Find("左") is HACImageEntry item)
                        {
                            layer.OffsetX = item.AsInt32();
                        }
                    }

                    //Y
                    {
                        if (layerEntry.Find("上") is HACImageEntry item)
                        {
                            layer.OffsetY = item.AsInt32();
                        }
                    }

                    //通道数
                    {
                        if (layerEntry.Find("色彩") is HACImageEntry item)
                        {
                            layer.Channel = item.AsUInt8();
                        }
                    }

                    //不透明度
                    {
                        if (layerEntry.Find("透明度") is HACImageEntry item)
                        {
                            layer.Opacity = Math.Min((int)item.AsInt16(), 0xFF);
                        }
                    }

                    //可见性
                    {
                        if (layerEntry.Find("显示") is HACImageEntry item)
                        {
                            layer.Visible = item.AsBool();
                        }
                    }

                    //混合方式
                    {
                        OGLRenderEmulator.BlendType src = OGLRenderEmulator.BlendType.Unknow;
                        OGLRenderEmulator.BlendType dest = OGLRenderEmulator.BlendType.Unknow;
                        if (layerEntry.Find("混合方式") is HACImageEntry item)
                        {
                            byte[] modeBytes = new byte[4];
                            item.AsArray(modeBytes, modeBytes.Length);

                            string mode = Encoding.ASCII.GetString(modeBytes);

                            switch (mode)
                            {
                                case "dark":
                                {
                                    src = OGLRenderEmulator.BlendType.Zero;
                                    dest = OGLRenderEmulator.BlendType.OneMinusSrcColor;
                                    break;
                                }
                                case "lite":
                                case "hLit":
                                case "sLit":
                                case "vLit":
                                case "lLit":
                                case "pLit":
                                {
                                    src = OGLRenderEmulator.BlendType.SrcAlpha;
                                    dest = OGLRenderEmulator.BlendType.One;
                                    break;
                                }
                                case "norm":
                                {
                                    src = OGLRenderEmulator.BlendType.SrcAlpha;
                                    dest = OGLRenderEmulator.BlendType.OneMinusSrcAlpha;
                                    break;
                                }
                                default:
                                {
                                    Debugger.Break();
                                    src = OGLRenderEmulator.BlendType.SrcAlpha;
                                    dest = OGLRenderEmulator.BlendType.OneMinusSrcAlpha;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            src = OGLRenderEmulator.BlendType.SrcAlpha;
                            dest = OGLRenderEmulator.BlendType.OneMinusSrcAlpha;
                        }

                        layer.SrcBlendMode = src;
                        layer.DestBlendMode = dest;
                    }

                    //压缩算法
                    {
                        HACImageCompress compress = HACImageCompress.Unknow;

                        if (layerEntry.Find("压缩算法") is HACImageEntry item)
                        {
                            string compressStr = item.AsString();
                            if (string.IsNullOrEmpty(compressStr))
                            {
                                compress = HACImageCompress.NoCompress;
                            }
                            else
                            {
                                switch (compressStr)
                                {
                                    case "RLE":
                                    {
                                        compress = HACImageCompress.RLE;
                                        break;
                                    }
                                    case "Bzip2":
                                    {
                                        compress = HACImageCompress.Bzip2;
                                        break;
                                    }
                                    case "RYC":
                                    {
                                        compress = HACImageCompress.RYC;
                                        break;
                                    }
                                    default:
                                    {
                                        Debugger.Break();
                                        break;
                                    }
                                }
                            }
                        }

                        layer.CompressType = compress;
                    }

                    //opengl格式
                    {
                        if (layerEntry.Find("推荐格式") is HACImageEntry item)
                        {
                            layer.GLFormat = item.AsInt32();
                        }
                    }

                    //像素数据
                    {
                        if (layerEntry.Find("象素数据") is HACImageEntry item)
                        {
                            layer.Data = item.Data;
                        }
                    }
                }
                
                layers.Add(layer);
            }
        }
    }
}
