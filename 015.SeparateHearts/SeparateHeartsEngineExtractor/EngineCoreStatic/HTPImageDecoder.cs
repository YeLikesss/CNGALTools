using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace EngineCoreStatic
{
    /// <summary>
    /// HTP图像解码
    /// </summary>
    public class HTPImageDecoder
    {
        /// <summary>
        /// HTP图层
        /// </summary>
        private class HTPLayer
        {
            /// <summary>
            /// X偏移
            /// </summary>
            public int OffsetX;
            /// <summary>
            /// Y偏移
            /// </summary>
            public int OffsetY;
            /// <summary>
            /// 宽度
            /// </summary>
            public int Width;
            /// <summary>
            /// 高度
            /// </summary>
            public int Height;
            /// <summary>
            /// 通道数
            /// </summary>
            public int Channel;
            /// <summary>
            /// 不透明度
            /// </summary>
            public int Opacity;
            /// <summary>
            /// 可见性
            /// </summary>
            public bool Visible;
            /// <summary>
            /// 源混合模式
            /// </summary>
            public OGLRenderEmulator.BlendType SrcBlendMode;
            /// <summary>
            /// 目标混合模式
            /// </summary>
            public OGLRenderEmulator.BlendType DestBlendMode;
            /// <summary>
            /// 名称
            /// </summary>
            public string Name = string.Empty;
            /// <summary>
            /// 切片表
            /// </summary>
            public byte[] TileTable = Array.Empty<byte>();
        }

        /// <summary>
        /// 切片信息
        /// </summary>
        private class HTLTile
        {
            /// <summary>
            /// 切片宽度
            /// </summary>
            public int Width;
            /// <summary>
            /// 切片高度
            /// </summary>
            public int Height;
            /// <summary>
            /// 切片大小
            /// </summary>
            public int Size;
            /// <summary>
            /// 切片数量
            /// </summary>
            public int Count;
            /// <summary>
            /// 切片偏移
            /// </summary>
            public long Offset;
        }

        private readonly string mFileName;                      //文件名
        private readonly List<HTPLayer> mLayers = new();        //图层
        private int mWidth;                                     //画布宽度
        private int mHeight;                                    //画布高度

        /// <summary>
        /// 提取图像
        /// </summary>
        /// <param name="outputDirectory">输出文件夹路径</param>
        /// <param name="tileBindingStream">绑定对应的切片流</param>
        public void ExtractToPNG(string outputDirectory, Stream tileBindingStream)
        {
            List<HTPLayer> layers = this.mLayers;
            if (layers.Any())
            {
                using BinaryReader br = new(tileBindingStream, Encoding.Unicode, true);
                if (br.ReadUInt64() == 0x000000001A4C5448)
                {
                    HTLTile tileInfo = new();
                    {
                        int w, h, size, count;
                        long pos;
                        w = br.ReadInt32();
                        h = br.ReadInt32();
                        size = w * h;
                        pos = tileBindingStream.Position;
                        count = (int)((tileBindingStream.Length - pos) / size);

                        tileInfo.Width = w;
                        tileInfo.Height = h;
                        tileInfo.Size = size;
                        tileInfo.Count = count;
                        tileInfo.Offset = pos;
                    }

                    string baseDir = Path.Combine(outputDirectory, this.mFileName + ".ext");

                    //提取图层
                    for(int i = 0; i < layers.Count; ++i)
                    {
                        HTPLayer layer = layers[i];

                        string layerPath = Path.Combine(baseDir, layer.Name + ".png");
                        {
                            if (Path.GetDirectoryName(layerPath) is string dir && !Directory.Exists(dir))
                            {
                                Directory.CreateDirectory(dir);
                            }
                        }

                        byte[] rawData = HTPImageDecoder.ExtractRAW(layer, tileInfo, tileBindingStream);
                        using MemoryStream pixelMs = new(rawData, false);

                        using FileStream layerFs = File.Create(layerPath);

                        using Bitmap bitmap = ImageConverter.OpenGLToGDI32bpp(pixelMs, layer.Width, layer.Height, layer.Channel);
                        ImageProcess.ChangeAlpha(bitmap, (byte)layer.Opacity);

                        bitmap.Save(layerFs, ImageFormat.Png);
                    }
                }
            }
        }

        /// <summary>
        /// 提取像素数据
        /// </summary>
        /// <param name="layerInfo">图层信息</param>
        /// <param name="tileInfo">切片信息</param>
        /// <param name="tileStream">切片流</param>
        /// <returns>像素数据</returns>
        private static byte[] ExtractRAW(HTPLayer layerInfo, HTLTile tileInfo, Stream tileStream)
        {
            /*
             * union
             * {
             *      unsigned __int8 Color;    <255
             *      int TileIndex;            >255
             * }
             * 
             * 切片矩阵按通道顺序
             * 切片内均为同一个通道
             *
             *  R, R        G, G         B, B
             *  R, R        G, G         B, B
             * 
             * 
             */

            int width = layerInfo.Width;
            int height = layerInfo.Height;
            int channel = layerInfo.Channel;

            int tileWidth = tileInfo.Width;
            int tileHeight = tileInfo.Height;
            int tileSize = tileInfo.Size;
            int tileCount = tileInfo.Count;
            long tileOffset = tileInfo.Offset;

            Span<int> table = MemoryMarshal.Cast<byte, int>(layerInfo.TileTable);

            int tableIndex = 0;
            int stride = width * channel;

            byte[] tileData = new byte[tileSize];
            byte[] rawData = new byte[width * height * channel];

            for(int y = 0; y < height;)
            {
                int procH = Math.Min(height - y, tileHeight);       //块处理高度
                for(int x = 0; x < width;)
                {
                    int procW = Math.Min(width - x, tileWidth);     //块处理宽度

                    //按通道填充
                    for(int c = 0; c < channel; ++c)
                    {
                        int tableValue = table[tableIndex];
                        if (tableValue >= 256)
                        {
                            //按切片块拷贝
                            int tileIndex = tableValue - 256;
                            if (tileIndex < tileCount)
                            {
                                tileStream.Position = tileIndex * tileSize + tileOffset;
                                if (tileStream.Read(tileData) != tileSize)
                                {
                                    Debugger.Break();
                                }

                                for (int ty = 0; ty < procH; ++ty)
                                {
                                    for (int tx = 0; tx < procW; ++tx)
                                    {
                                        int idx = (y + ty) * stride + (x + tx) * channel + c;
                                        int inTileIdx = ty * tileHeight + tx;
                                        rawData[idx] = tileData[inTileIdx];
                                    }
                                }
                            }
                        }
                        else
                        {
                            //按切片块赋值
                            for (int ty = 0; ty < procH; ++ty)
                            {
                                for (int tx = 0; tx < procW; ++tx)
                                {
                                    int idx = (y + ty) * stride + (x + tx) * channel + c;
                                    rawData[idx] = (byte)tableValue;
                                }
                            }
                        }
                        ++tableIndex;
                    }
                    x += procW;
                }
                y += procH;
            }

            return rawData;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="stream">输入流</param>
        /// <param name="fileName">文件名</param>
        public HTPImageDecoder(Stream stream,string fileName)
        {
            this.mFileName = fileName;
            this.Intialize(stream);
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="stream">输入流</param>
        private void Intialize(Stream stream)
        {
            using BinaryReader br = new(stream, Encoding.Unicode, true);
            if (br.ReadUInt64() != 0x000000001A505448ul)
            {
                return;
            }

            this.mWidth = br.ReadInt32();
            this.mHeight = br.ReadInt32();

            int layerCount = br.ReadInt32();
            if (layerCount == 0)
            {
                return;
            }

            List<HTPLayer> layers = this.mLayers;
            layers.Capacity = layerCount;

            //获取图层信息
            for (int i = 0; i < layerCount; ++i)
            {
                int metaDataSize = br.ReadInt32();
                long metaDataPosition = stream.Position;

                HTPLayer layer = new();

                //基本信息
                layer.Name = HACStreamExtend.ReadString(stream);
                layer.OffsetX = br.ReadInt32();
                layer.OffsetY = br.ReadInt32();
                layer.Width = br.ReadInt32();
                layer.Height = br.ReadInt32();
                layer.Channel = br.ReadByte();
                layer.Opacity = br.ReadByte();
                layer.Visible = br.ReadByte() != 0;

                //渲染模式
                string mode = string.Empty;
                {
                    byte[] bytes = br.ReadBytes(4);
                    mode = Encoding.ASCII.GetString(bytes);
                }

                OGLRenderEmulator.BlendType src, dest;
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
                    default:
                    {
                        src = OGLRenderEmulator.BlendType.SrcAlpha;
                        dest = OGLRenderEmulator.BlendType.OneMinusSrcAlpha;
                        break;
                    }
                }
                layer.SrcBlendMode = src;
                layer.DestBlendMode = dest;

                //切片表
                stream.Position = metaDataPosition + metaDataSize;

                int tableLength = br.ReadInt32();
                tableLength &= ~3;

                byte[] table = new byte[tableLength];
                if (stream.Read(table) != tableLength)
                {
                    Debugger.Break();
                }
                layer.TileTable = table;

                layers.Add(layer);
            }
        }
    }
}
