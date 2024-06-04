using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Imaging;
using System.Buffers;

namespace ConsoleExecute
{
    /// <summary>
    /// 图像头
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 0x0E)]
    public struct ImageHeader
    {
        [FieldOffset(0x00)]
        public ulong Signature;
        [FieldOffset(0x08)]
        public byte Reserve1;
        [FieldOffset(0x09)]
        public byte Reserve2;
        /// <summary>
        /// 宽
        /// </summary>
        [FieldOffset(0x0A)]
        public ushort Width;
        /// <summary>
        /// 高
        /// </summary>
        [FieldOffset(0x0C)]
        public ushort Height;

        /// <summary>
        /// 检查头
        /// </summary>
        public bool IsVaild => this.Signature == 0x0049414B474E4159ul;
    }

    /// <summary>
    /// 图像解码器
    /// </summary>
    public class ImageDecoder
    {
        /// <summary>
        /// 加载图像
        /// </summary>
        /// <param name="path">全路径</param>
        /// <returns>图像对象</returns>
        public unsafe static Bitmap? Load(string path)
        {
            if (File.Exists(path))
            {
                using FileStream fs = File.OpenRead(path);
                if(fs.Length > 0x30EL)
                {
                    using BinaryReader br = new(fs);
                    ImageHeader header = StreamExtend.Read<ImageHeader>(fs);
                    if (header.IsVaild)
                    {
                        int w = header.Width;
                        int h = header.Height;
                        int pixelCount = w * h;

                        Bitmap bitmap = new(w, h, PixelFormat.Format8bppIndexed);
                        ColorPalette palette = bitmap.Palette;

                        for (int i = 0; i < 256; ++i)
                        {
                            byte r = br.ReadByte();
                            byte g = br.ReadByte();
                            byte b = br.ReadByte();
                            byte a = 0xFF;

                            palette.Entries[i] = Color.FromArgb(a, r, g, b);
                        }
                        bitmap.Palette = palette;

                        byte[] pixelData = new byte[pixelCount];
                        fs.Read(pixelData);

                        BitmapData bmpData = bitmap.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);
                        IntPtr ptr = bmpData.Scan0;
                        Marshal.Copy(pixelData, 0, ptr, pixelCount);

                        bitmap.UnlockBits(bmpData);

                        return bitmap;
                    }
                }
            }
            return null;
        }

    }
}
