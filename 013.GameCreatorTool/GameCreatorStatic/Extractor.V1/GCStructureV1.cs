using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GameCreatorStatic.Extractor.V1
{
    /// <summary>
    /// 加密标志V1
    /// </summary>
    [Flags]
    public enum GCEntryptionFlagV1 : uint
    {
        /// <summary>
        /// 无加密
        /// </summary>
        None = 0x00000000u,
        /// <summary>
        /// 图像加密
        /// </summary>
        Image = 0x00000001u,
        /// <summary>
        /// 文本加密
        /// </summary>
        Text = 0x00000002u,
        /// <summary>
        /// 音频加密
        /// </summary>
        Audio = 0x00000004u,
        /// <summary>
        /// 视频加密
        /// </summary>
        Video = 0x00000008u,
    }

    /// <summary>
    /// 游戏解包V1
    /// </summary>
    public abstract class GCExtractorV1 : IGCExtractor
    {
        public GCExtractorVersion ExtractorVersion => GCExtractorVersion.V1;

        /// <summary>
        /// 游戏名称
        /// </summary>
        public abstract string Title { get; }
        /// <summary>
        /// 加密标志
        /// </summary>
        public abstract GCEntryptionFlagV1 EntryptionFlag { get; }
        /// <summary>
        /// GC版本
        /// </summary>
        public virtual string Version { get; } = string.Empty;
        /// <summary>
        /// 文本加密key
        /// </summary>
        public virtual string TextKey { get; } = string.Empty;
        /// <summary>
        /// 音频加密key
        /// </summary>
        public virtual string AudioKey { get; } = string.Empty;
        /// <summary>
        /// 视频加密key
        /// </summary>
        public virtual string VideoKey { get; } = string.Empty;

        /// <summary>
        /// 图像文件夹
        /// </summary>
        public virtual string[] ImageFolders { get; } = new string[]
        {
            "asset\\image",
        };
        /// <summary>
        /// 图像扩展名
        /// </summary>
        public virtual HashSet<string> ImageExtensions { get; } = new()
        {
            ".png",
            ".jpeg",
            ".jpg",
            ".gif",
        };


        /// <summary>
        /// 文本文件夹
        /// </summary>
        public virtual string[] TextFolders { get; } = new string[]
        {
            "asset\\json",
        };
        /// <summary>
        /// 文本扩展名
        /// </summary>
        public virtual HashSet<string> TextExtensions { get; } = new()
        {
            ".json",
        };
        /// <summary>
        /// 特殊文本
        /// <para>Dictionary[相对路径, 文件key]</para>
        /// </summary>
        public virtual Dictionary<string, string> TextSpecial { get; } = new()
        {
            { "asset\\json\\startup.json", "gc_zip" },
        };


        /// <summary>
        /// 音频文件夹
        /// </summary>
        public virtual string[] AudioFolders { get; } = new string[]
        {
            "asset\\audio",
        };
        /// <summary>
        /// 音频扩展名
        /// </summary>
        public virtual HashSet<string> AudioExtensions { get; } = new()
        {
            ".ogg",
            ".mp3",
        };

        /// <summary>
        /// 视频文件夹
        /// </summary>
        public virtual string[] VideoFolders { get; } = new string[]
        {
            "asset\\video",
        };
        /// <summary>
        /// 视频扩展名
        /// </summary>
        public virtual HashSet<string> VideoExtensions { get; } = new()
        {
            ".mp4",
        };


        /// <summary>
        /// Zip包编码
        /// </summary>
        public virtual Encoding ZipEncoding { get; } = Encoding.UTF8;


        public override string ToString()
        {
            return this.Title;
        }

        protected string mGameDirectory = string.Empty;             //游戏文件夹
        protected IProgress<string>? mMessageCallBack = null;       //信息回调

        /// <summary>
        /// 提取图像
        /// </summary>
        protected void ExtractImage()
        {
            if (this.EntryptionFlag.HasFlag(GCEntryptionFlagV1.Image))
            {
                string gameRootDirectory = this.mGameDirectory;
                IProgress<string>? msgCB = this.mMessageCallBack;

                foreach (string encryptFolder in this.ImageFolders)
                {
                    string resourceDirectory = Path.Combine(gameRootDirectory, encryptFolder);
                    if (Directory.Exists(resourceDirectory))
                    {
                        foreach (string inPath in Directory.EnumerateFiles(resourceDirectory, "*.*", SearchOption.AllDirectories))
                        {
                            string relativePath = inPath[(gameRootDirectory.Length + 1)..];
                            string outPath = Path.Combine(gameRootDirectory, "Extract_Static", relativePath);
                            {
                                string outDir = Path.GetDirectoryName(outPath)!;
                                if (!Directory.Exists(outDir))
                                {
                                    Directory.CreateDirectory(outDir);
                                }
                            }

                            //检查后缀解密
                            string ext = Path.GetExtension(inPath).ToLower();
                            if (this.ImageExtensions.Contains(ext))
                            {
                                using FileStream outFs = File.Create(outPath);
                                byte[] buf = File.ReadAllBytes(inPath);

                                long length = buf.LongLength;
                                long fakeBytePosition = (length - 1L) / 2L;
                                
                                buf[1] ^= buf[2];
                                buf[2] ^= buf[1];
                                buf[1] ^= buf[2];

                                outFs.Write(buf, 0, (int)fakeBytePosition);
                                outFs.Write(buf, (int)(fakeBytePosition + 1L), (int)(length - fakeBytePosition - 1L));
                                outFs.Flush();

                                msgCB?.Report($"[图像]解密: {relativePath}");
                            }
                            else
                            {
                                File.Copy(inPath, outPath, true);
                                msgCB?.Report($"[图像]仅拷贝: {relativePath}");
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 提取特殊文本
        /// </summary>
        protected void ExtractTextSpecial()
        {
            string gameRootDirectory = this.mGameDirectory;
            IProgress<string>? msgCB = this.mMessageCallBack;
            Encoding encoding = this.ZipEncoding;

            foreach (KeyValuePair<string, string> special in this.TextSpecial)
            {
                string relativePath = special.Key;
                string inPath = Path.Combine(gameRootDirectory, relativePath);
                if(File.Exists(inPath))
                {
                    string outPath = Path.Combine(gameRootDirectory, "Extract_Static", relativePath);
                    {
                        string outDir = Path.GetDirectoryName(outPath)!;
                        if (!Directory.Exists(outDir))
                        {
                            Directory.CreateDirectory(outDir);
                        }
                    }

                    //解密文本
                    using FileStream inFs = File.OpenRead(inPath);
                    if(ZipStorage.Decompress(inFs, encoding, special.Value, Path.GetDirectoryName(outPath)!, out string error))
                    {
                        msgCB?.Report($"[文本]解密: {relativePath}");
                    }
                    else
                    {
                        File.Copy(inPath, outPath, true);
                        msgCB?.Report($"[文本]仅拷贝: {relativePath} 解密错误: {error}");
                    }
                }
            }
        }

        /// <summary>
        /// 提取文本
        /// </summary>
        protected void ExtractText()
        {
            //特殊文本不受加密Flag影响
            this.ExtractTextSpecial();

            //解密普通文本
            if (this.EntryptionFlag.HasFlag(GCEntryptionFlagV1.Text))
            {
                string gameRootDirectory = this.mGameDirectory;
                IProgress<string>? msgCB = this.mMessageCallBack;
                Encoding encoding = this.ZipEncoding;
                string key = this.TextKey;

                foreach (string encryptFolder in this.TextFolders)
                {
                    string resourceDirectory = Path.Combine(gameRootDirectory, encryptFolder);
                    if (Directory.Exists(resourceDirectory))
                    {
                        foreach (string inPath in Directory.EnumerateFiles(resourceDirectory, "*.*", SearchOption.AllDirectories))
                        {
                            string relativePath = inPath[(gameRootDirectory.Length + 1)..];

                            //跳过特殊文本
                            if (this.TextSpecial.ContainsKey(relativePath))
                            {
                                continue;
                            }

                            string outPath = Path.Combine(gameRootDirectory, "Extract_Static", relativePath);
                            {
                                string outDir = Path.GetDirectoryName(outPath)!;
                                if (!Directory.Exists(outDir))
                                {
                                    Directory.CreateDirectory(outDir);
                                }
                            }

                            //检查后缀解密
                            string ext = Path.GetExtension(inPath).ToLower();
                            if (this.TextExtensions.Contains(ext))
                            {
                                using FileStream inFs = File.OpenRead(inPath);
                                if (ZipStorage.Decompress(inFs, encoding, key, Path.GetDirectoryName(outPath)!, out string error))
                                {
                                    msgCB?.Report($"[文本]解密: {relativePath}");
                                }
                                else
                                {
                                    File.Copy(inPath, outPath, true);
                                    msgCB?.Report($"[文本]仅拷贝: {relativePath} 解密错误: {error}");
                                }
                            }
                            else
                            {
                                File.Copy(inPath, outPath, true);
                                msgCB?.Report($"[文本]仅拷贝: {relativePath}");
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 提取音频
        /// </summary>
        protected void ExtractAudio()
        {
            if (this.EntryptionFlag.HasFlag(GCEntryptionFlagV1.Audio))
            {
                string gameRootDirectory = this.mGameDirectory;
                IProgress<string>? msgCB = this.mMessageCallBack;
                Encoding encoding = this.ZipEncoding;
                string key = this.AudioKey;

                foreach (string encryptFolder in this.AudioFolders)
                {
                    string resourceDirectory = Path.Combine(gameRootDirectory, encryptFolder);
                    if (Directory.Exists(resourceDirectory))
                    {
                        foreach (string inPath in Directory.EnumerateFiles(resourceDirectory, "*.*", SearchOption.AllDirectories))
                        {
                            string relativePath = inPath[(gameRootDirectory.Length + 1)..];
                            string outPath = Path.Combine(gameRootDirectory, "Extract_Static", relativePath);
                            {
                                string outDir = Path.GetDirectoryName(outPath)!;
                                if (!Directory.Exists(outDir))
                                {
                                    Directory.CreateDirectory(outDir);
                                }
                            }

                            //检查后缀解密
                            string ext = Path.GetExtension(inPath).ToLower();
                            if (this.AudioExtensions.Contains(ext))
                            {
                                using FileStream inFs = File.OpenRead(inPath);
                                if (ZipStorage.Decompress(inFs, encoding, key, Path.GetDirectoryName(outPath)!, out string error))
                                {
                                    msgCB?.Report($"[音频]解密: {relativePath}");
                                }
                                else
                                {
                                    File.Copy(inPath, outPath, true);
                                    msgCB?.Report($"[音频]仅拷贝: {relativePath} 解密错误: {error}");
                                }
                            }
                            else
                            {
                                File.Copy(inPath, outPath, true);
                                msgCB?.Report($"[音频]仅拷贝: {relativePath}");
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 提取视频
        /// </summary>
        protected void ExtractVideo()
        {
            //NotImpl
        }

        /// <summary>
        /// 提取资源
        /// </summary>
        /// <param name="gameDirectory">游戏路径</param>
        /// <param name="msgcallback">信息回调</param>
        public void Extract(string gameDirectory, IProgress<string>? msgcallback = null)
        {
            if (Directory.Exists(gameDirectory))
            {
                this.mGameDirectory = gameDirectory;
                this.mMessageCallBack = msgcallback;

                this.ExtractImage();
                this.ExtractText();
                this.ExtractAudio();
                this.ExtractVideo();
            }
            else
            {
                msgcallback?.Report($"游戏路径不存在: {gameDirectory}");
            }
        }
    }
}
