using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace BoneNailStatic.Package.V1
{
    /// <summary>
    /// 序列化器
    /// </summary>
    public class Serializer
    {
        /// <summary>
        /// 反序列化文件信息
        /// </summary>
        /// <param name="data">二进制数据</param>
        /// <returns>信息对象 反序列化失败:null</returns>
        public static YooManifest? DeserializeManifest(byte[] data)
        {
            using YooReader reader = new(data);
            YooManifest manifest = new();

            //1.解析头部
            {
                if (reader.ReadUInt32() != 0x00594F4Fu)
                {
                    return null;
                }
                string ver = reader.ReadString();
                if (ver != "2.0.0")
                {
                    return null;
                }

                manifest.FileVersion = ver;
                manifest.EnableAddressable = reader.ReadBoolean();
                manifest.LocationToLower= reader.ReadBoolean();
                manifest.IncludeAssetGUID = reader.ReadBoolean();
                manifest.OutputNameStyle = reader.ReadInt32();
                manifest.BuildPipeline = reader.ReadString();
                manifest.PackageName = reader.ReadString();
                manifest.PackageVersion = reader.ReadString();

                if(manifest.EnableAddressable && manifest.LocationToLower)
                {
                    return null;
                }
            }

            //2.解析资源
            {
                int assetCount = reader.ReadInt32();

                List<YooAsset> assetList = manifest.AssetList;
                Dictionary<string, YooAsset> assetDic = manifest.AssetDic;
                Dictionary<string, string> assetMap1 = manifest.AssetPathMapping1;
                Dictionary<string, string> assetMap2 = manifest.AssetPathMapping2;

                assetList.EnsureCapacity(assetCount);
                assetDic.EnsureCapacity(assetCount);
                assetMap1.EnsureCapacity(manifest.EnableAddressable ? 3 * assetCount : 2 * assetCount);
                if (manifest.IncludeAssetGUID)
                {
                    assetMap2.EnsureCapacity(assetCount);
                }

                for (int i = 0; i < assetCount; ++i)
                {
                    YooAsset asset = new();

                    asset.Address = reader.ReadString();
                    asset.AssetPath = reader.ReadString();
                    asset.AssetGUID = reader.ReadString();
                    asset.AssetTags = reader.ReadStringArray();
                    asset.BundleID = reader.ReadInt32();

                    assetList.Add(asset);
                    assetDic.Add(asset.AssetPath, asset);

                    //map1
                    {
                        string path = asset.AssetPath;

                        string name = manifest.LocationToLower ? path.ToLower() : path;
                        string nameNoExt = Path.ChangeExtension(name, null);

                        assetMap1.Add(name, path);
                        assetMap1.TryAdd(nameNoExt, path);
                        if (manifest.EnableAddressable)
                        {
                            string addr = asset.Address;
                            if (!string.IsNullOrEmpty(addr))
                            {
                                assetMap1.Add(addr, path);
                            }
                        }
                    }

                    //map2
                    {
                        if (manifest.IncludeAssetGUID)
                        {
                            assetMap2.Add(asset.AssetGUID, asset.AssetPath);
                        }
                    }
                }
            }

            //3.解析封包
            {
                int bundleCount = reader.ReadInt32();

                List<YooBundle> bundleList = manifest.BundleList;
                Dictionary<string, YooBundle> bundleDic1 = manifest.BundleDic1;
                Dictionary<string, YooBundle> bundleDic2 = manifest.BundleDic2;
                Dictionary<string, YooBundle> bundleDic3 = manifest.BundleDic3;

                bundleList.EnsureCapacity(bundleCount);
                bundleDic1.EnsureCapacity(bundleCount);
                bundleDic2.EnsureCapacity(bundleCount);
                bundleDic3.EnsureCapacity(bundleCount);

                for(int i = 0; i < bundleCount; ++i)
                {
                    YooBundle bundle = new();

                    bundle.BundleName = reader.ReadString();
                    bundle.UnityCRC = reader.ReadUInt32();
                    bundle.FileHash = reader.ReadString();
                    bundle.FileCRC = reader.ReadString();
                    bundle.FileSize = reader.ReadInt64();
                    bundle.Encrypted = reader.ReadBoolean();
                    bundle.Tags = reader.ReadStringArray();
                    bundle.DependIDs = reader.ReadInt32Array();

                    bundle.SetManifestInfo(manifest);

                    bundleList.Add(bundle);
                    bundleDic1.Add(bundle.BundleName, bundle);
                    bundleDic2.Add(bundle.OutputName, bundle);
                    bundleDic3.Add(bundle.FileHash, bundle);
                }
            }

            return manifest;
        }
    }


    /// <summary>
    /// 二进制读取器
    /// </summary>
    public class YooReader : BinaryReader
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="data">数据流</param>
        public YooReader(byte[] data) : base(new MemoryStream(data, false))
        {
        }

        /// <summary>
        /// 是否存在数据
        /// </summary>
        public bool HasData => this.BaseStream.Length != 0L;

        /// <summary>
        /// 当前数据位置
        /// </summary>
        public long Position => this.BaseStream.Position;

        public override bool ReadBoolean()
        {
            return this.ReadByte() == 0x01;
        }
        public override string ReadString()
        {
            int len = this.ReadUInt16();
            if (len == 0)
            {
                return string.Empty;
            }
            return Encoding.UTF8.GetString(this.ReadBytes(len));
        }

        /// <summary>
        /// 读取Int32数组
        /// </summary>
        public int[] ReadInt32Array()
        {
            int len = this.ReadUInt16();
            if (len == 0)
            {
                return Array.Empty<int>();
            }

            int[] dat = new int[len];
            Span<byte> ptr = MemoryMarshal.AsBytes(dat.AsSpan());
            this.Read(ptr);

            return dat;
        }

        /// <summary>
        /// 读取Int64数组
        /// </summary>
        public long[] ReadInt64Array()
        {
            int len = this.ReadUInt16();
            if (len == 0)
            {
                return Array.Empty<long>();
            }

            long[] dat = new long[len];
            Span<byte> ptr = MemoryMarshal.AsBytes(dat.AsSpan());
            this.Read(ptr);

            return dat;
        }

        /// <summary>
        /// 读取字符串数组
        /// </summary>
        public string[] ReadStringArray()
        {
            int len = this.ReadUInt16();
            if (len == 0)
            {
                return Array.Empty<string>();
            }

            string[] dat = new string[len];
            for(int i = 0; i < len; ++i)
            {
                dat[i] = this.ReadString();
            }

            return dat;
        }
    }
}
