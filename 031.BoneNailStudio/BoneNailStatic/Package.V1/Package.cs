using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BoneNailStatic.Package.V1
{
    /// <summary>
    /// 资源信息
    /// </summary>
    public class YooAsset
    {
        public string Address = string.Empty;
        public string AssetPath = string.Empty;
        public string AssetGUID = string.Empty;
        public string[] AssetTags = Array.Empty<string>();
        public int BundleID;

        /// <summary>
        /// 是否存在Tag
        /// </summary>
        /// <param name="tags">tag数组</param>
        /// <returns>True存在 False不存在</returns>
        public bool ContainTags(string[] tags)
        {
            foreach(string t in tags)
            {
                if (this.AssetTags.Contains(t))
                {
                    return true;
                }
            }
            return false;
        }
    }

    /// <summary>
    /// 封包信息
    /// </summary>
    public class YooBundle
    {
        public string BundleName = string.Empty;
        public uint UnityCRC;
        public string FileHash = string.Empty;
        public string FileCRC = string.Empty;
        public long FileSize;
        public bool Encrypted;
        public string[] Tags = Array.Empty<string>();
        public int[] DependIDs = Array.Empty<int>();

        private string mPackageName = string.Empty;     //ceod
        private string mBuildPipeline = string.Empty;   //ceoe
        private string mOutputName = string.Empty;      //ceof
        private string mExtension = string.Empty;       //ceog

        public string PackageName => this.mPackageName;
        public string BuildPipeline => this.mBuildPipeline;
        public string OutputName => this.mOutputName;
        public string Extension => this.mExtension;

        /// <summary>
        /// 设置描述信息
        /// </summary>
        /// <param name="manifest">描述信息</param>
        public void SetManifestInfo(YooManifest manifest)
        {
            this.mPackageName = manifest.PackageName;
            this.mBuildPipeline = manifest.BuildPipeline;

            string name = this.BundleName;
            string ext = Path.GetExtension(name);

            switch (manifest.OutputNameStyle)
            {
                case 0:
                {
                    name = $"{this.FileHash}{ext}";

                    break;
                }
                case 1:
                {
                    break;
                }
                case 2:
                {
                    int idx = name.LastIndexOf('.');
                    name = name.Remove(idx);
                    name = $"{name}_{this.FileHash}{ext}";

                    break;
                }
                default:
                {
                    throw new NotImplementedException("非法的名称类型");
                }
            }

            this.mOutputName = name;
            this.mExtension = ext;
        }
    }

    /// <summary>
    /// 描述信息
    /// </summary>
    public class YooManifest
    {
        public string FileVersion = string.Empty;
        public bool EnableAddressable;
        public bool LocationToLower;
        public bool IncludeAssetGUID;
        public int OutputNameStyle;
        public string BuildPipeline = string.Empty;
        public string PackageName = string.Empty;
        public string PackageVersion = string.Empty;

        public List<YooAsset> AssetList = new();
        public List<YooBundle> BundleList = new();

        public Dictionary<string, YooBundle> BundleDic1 = new();
        public Dictionary<string, YooBundle> BundleDic2 = new();
        public Dictionary<string, YooBundle> BundleDic3 = new();

        public Dictionary<string, YooAsset> AssetDic = new();
        public Dictionary<string, string> AssetPathMapping1 = new();
        public Dictionary<string, string> AssetPathMapping2 = new();


        /// <summary>
        /// 提取资源
        /// </summary>
        /// <param name="directory">资源目录</param>
        /// <param name="crypto">加密类</param>
        public void Extract(string directory, Crypto? crypto = null)
        {
            for (int i = 0; i < this.BundleList.Count; ++i)
            {
                YooBundle bundle = this.BundleList[i];
                string inPath = Path.Combine(directory, bundle.OutputName);
                string outPath = Path.Combine(directory, "Static_Extract", bundle.BundleName);
                if (!File.Exists(inPath))
                {
                    continue;
                }
                {
                    string dir = Path.GetDirectoryName(outPath)!;
                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }
                }

                if (crypto is not null)
                {
                    byte[] key = crypto.GenerateKey(bundle.BundleName);
                    byte[] iv = crypto.GenerateIV(bundle.BundleName);

                    using FileStream inFs = File.OpenRead(inPath);
                    using FileStream outFs = File.Create(outPath);
                    AesCtr.Decrypt(inFs, outFs, inFs.Length, key, iv[0..16]);
                    outFs.Flush();
                }
                else
                {
                    File.Copy(inPath, outPath);
                }
                Console.WriteLine($"[提取成功] {bundle.OutputName}");
            }
        }
    }
}
