using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace EngineCoreStatic
{
    /// <summary>
    /// 图像转换器
    /// </summary>
    public class ImageConverter
    {
        /// <summary>
        /// 转换OpenGL像素到GDI像素 (32bpp ARGB) 
        /// </summary>
        /// <param name="data">像素数据</param>
        /// <param name="width">宽</param>
        /// <param name="height">高</param>
        /// <param name="channel">通道数</param>
        /// <returns>图像对象</returns>
        public static Bitmap OpenGLToGDI32bpp(byte[] data, int width, int height, int channel)
        {
            using MemoryStream ms = new(data, false);
            return ImageConverter.OpenGLToGDI32bpp(ms, width, height, channel);
        }

        /// <summary>
        /// 转换OpenGL像素到GDI像素 (32bpp ARGB)
        /// </summary>
        /// <param name="stream">输入流</param>
        /// <param name="width">宽</param>
        /// <param name="height">高</param>
        /// <param name="channel">通道数</param>
        /// <returns>图像对象</returns>
        public unsafe static Bitmap OpenGLToGDI32bpp(Stream stream, int width, int height, int channel)
        {
            using BinaryReader br = new(stream, Encoding.Unicode, true);

            int w = width;
            int h = height;
            int c = channel;

            Bitmap bitmap = new(w, h, PixelFormat.Format32bppArgb);
            BitmapData bmpData = bitmap.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            Span<byte> bmpPtr = new(bmpData.Scan0.ToPointer(), w * h * 4);

            switch (c)
            {
                case 1:
                {
                    //OGL(Gray) -> GDI(BGRA)    1 Channel   GL_LUMINANCE
                    for (int i = 0; i < bmpPtr.Length; i += 4)
                    {
                        byte gary = br.ReadByte();

                        bmpPtr[i + 0] = gary;
                        bmpPtr[i + 1] = gary;
                        bmpPtr[i + 2] = gary;
                        bmpPtr[i + 3] = 0xFF;
                    }
                    break;
                }
                case 2:
                {
                    //OGL(GrayA) -> GDI(BGRA)   2 Channel   GL_LUMINANCE_ALPHA
                    for (int i = 0; i < bmpPtr.Length; i += 4)
                    {
                        byte gary = br.ReadByte();
                        byte a = br.ReadByte();

                        bmpPtr[i + 0] = gary;
                        bmpPtr[i + 1] = gary;
                        bmpPtr[i + 2] = gary;
                        bmpPtr[i + 3] = a;
                    }
                    break;
                }
                case 3:
                {
                    //OGL(RGB) -> GDI(BGRA)      3 Channel  GL_RGB
                    for (int i = 0; i < bmpPtr.Length; i += 4)
                    {
                        byte r = br.ReadByte();
                        byte g = br.ReadByte();
                        byte b = br.ReadByte();

                        bmpPtr[i + 0] = b;
                        bmpPtr[i + 1] = g;
                        bmpPtr[i + 2] = r;
                        bmpPtr[i + 3] = 0xFF;
                    }
                    break;
                }
                case 4:
                {
                    //OGL(RGBA) -> GDI(BGRA)    4 Channel   GL_RGBA
                    for (int i = 0; i < bmpPtr.Length; i += 4)
                    {
                        byte r = br.ReadByte();
                        byte g = br.ReadByte();
                        byte b = br.ReadByte();
                        byte a = br.ReadByte();

                        bmpPtr[i + 0] = b;
                        bmpPtr[i + 1] = g;
                        bmpPtr[i + 2] = r;
                        bmpPtr[i + 3] = a;
                    }
                    break;
                }
                case 5:
                {
                    //OGL(A) -> GDI(BGRA)    1 Channel   GL_ALPHA
                    for (int i = 0; i < bmpPtr.Length; i += 4)
                    {
                        byte alpha = br.ReadByte();

                        bmpPtr[i + 0] = alpha;
                        bmpPtr[i + 1] = alpha;
                        bmpPtr[i + 2] = alpha;
                        bmpPtr[i + 3] = 0xFF;
                    }
                    break;
                }
                default:
                {
#if DEBUG
                    Debugger.Break();
#endif
                    break;
                }
            }
            bitmap.UnlockBits(bmpData);
            return bitmap;
        }
    }

    /// <summary>
    /// 图像处理
    /// </summary>
    public class ImageProcess
    {
        /// <summary>
        /// 修改Alpha通道
        /// </summary>
        /// <param name="bitmap">图像</param>
        /// <param name="opacity">不透明度</param>
        public unsafe static void ChangeAlpha(Bitmap bitmap, byte opacity)
        {
            int w = bitmap.Width;
            int h = bitmap.Height;

            BitmapData bmpData = bitmap.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            Span<byte> bmpPtr = new(bmpData.Scan0.ToPointer(), w * h * 4);

            for (int i = 0; i < bmpPtr.Length; i += 4)
            {
                byte alpha = (byte)(bmpPtr[i + 3] * opacity / 0xFF);
                bmpPtr[i + 3] = alpha;
            }

            bitmap.UnlockBits(bmpData);
        }
    }

    /// <summary>
    /// OGL渲染器
    /// </summary>
    public class OGLRenderEmulator
    {
        /// <summary>
        /// 混合模式
        /// </summary>
        public enum BlendType
        {
            Unknow = -1,
            Zero = 0,
            One = 1,
            SrcColor = 0x300,
            OneMinusSrcColor = 0x301,
            SrcAlpha = 0x302,
            OneMinusSrcAlpha = 0x303,
        }
    }
}
