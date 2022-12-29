using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;

namespace Extractor.ZixRenpy8V1.Crypto
{
    public class Crypto128
    {
        /// <summary>
        /// 解密环境变量
        /// </summary>
        private class DecryptContext
        {
            /// <summary>
            /// Key长度
            /// </summary>
            public int KeyLength { get; private set; }
            /// <summary>
            /// 存放生成的Key表
            /// </summary>
            public byte[] DecryptTable { get; private set; }
            /// <summary>
            /// 轮解密次数
            /// </summary>
            public int DecryptRound { get; private set; }
            /// <summary>
            /// 表块起始点(块大小)
            /// </summary>
            public int StartBlock { get; private set; }

            /// <summary>
            /// 块大小
            /// </summary>
            public int BlockSize => 4;

            private IKeyInformation mKeyInformation;

            /// <summary>
            /// 解密
            /// </summary>
            /// <param name="data">16字节数据</param>
            /// <returns></returns>
            public bool Decrypt(Span<byte> data)
            {
                int dataLen = data.Length;
                if (dataLen == 16)
                {
                    int round = this.DecryptRound;
                    //使用表解密
                    {
                        Span<byte> decryptKeyInTable = this.DecryptTable.AsSpan().Slice((round - 1) * 16, 16);
                        for(int i = 0; i < 16; ++i)
                        {
                            data[i] ^= decryptKeyInTable[i];
                        }
                    }
                    round -= 2;     //轮数-2

                    while (round > 0)
                    {
                        //变换一次
                        this.Transform16Bytes(data);

                        //取一次S盒7
                        {
                            for (int i = 0; i < 16; ++i)
                            {
                                data[i] = this.GetSubstitutionBox(this.mKeyInformation.SubstitutionBox7, data[i]);
                            }
                        }

                        //使用表解密
                        {
                            Span<byte> decryptKeyInTable = this.DecryptTable.AsSpan().Slice(round * 16, 16);
                            for (int i = 0; i < 16; ++i)
                            {
                                data[i] ^= decryptKeyInTable[i];
                            }
                        }

                        //变换一轮
                        for(int i = 0; i < 4; ++i)
                        {
                            this.Transform4Bytes(data.Slice(i * 4, 4));
                        }
                        --round;
                    }

                    //变换一次
                    this.Transform16Bytes(data);

                    //取一次S盒7
                    {
                        for (int i = 0; i < 16; ++i)
                        {
                            data[i] = this.GetSubstitutionBox(this.mKeyInformation.SubstitutionBox7, data[i]);
                        }
                    }

                    //使用表解密
                    {
                        Span<byte> decryptKeyInTable = this.DecryptTable.AsSpan().Slice(0, 16);
                        for (int i = 0; i < 16; ++i)
                        {
                            data[i] ^= decryptKeyInTable[i];
                        }
                    }
                    return true;
                }
                return false;
            }

            /// <summary>
            /// 根据索引获取S盒的值
            /// </summary>
            /// <param name="substitutionBox">s盒表</param>
            /// <param name="index">索引</param>
            /// <returns></returns>
            private byte GetSubstitutionBox(byte[] substitutionBox, byte index)
            {
                return substitutionBox[index];
            }
            /// <summary>
            /// 根据索引和选择子获取S盒的值
            /// </summary>
            /// <param name="substitutionBox">s盒表</param>
            /// <param name="index">索引</param>
            /// <param name="selector">选择子</param>
            /// <returns></returns>
            private byte GetSubstitutionBox(byte index, byte selector)
            {
                switch (selector)
                {
                    case 0x02:
                        return this.GetSubstitutionBox(this.mKeyInformation.SubstitutionBox6, index);
                    case 0x03:
                        return this.GetSubstitutionBox(this.mKeyInformation.SubstitutionBox5, index);
                    case 0x09:
                        return this.GetSubstitutionBox(this.mKeyInformation.SubstitutionBox4, index);
                    case 0x0B:
                        return this.GetSubstitutionBox(this.mKeyInformation.SubstitutionBox3, index);
                    case 0x0D:
                        return this.GetSubstitutionBox(this.mKeyInformation.SubstitutionBox2, index);
                    case 0x0E:
                        return this.GetSubstitutionBox(this.mKeyInformation.SubstitutionBox1, index);
                    default:
                        return index;
                }
            }

            /// <summary>
            /// 变换一次数据 (16字节)
            /// </summary>
            /// <param name="data">数据</param>
            private void Transform16Bytes(Span<byte> data)
            {
                //暂存解密结果
                Span<byte> temp = stackalloc byte[16];

                //解密
                for (int index = 0; index < 16; index++)
                {
                    temp[index] = data[(4 * (16 - index) + index) % 16];
                }
                //回写覆盖原数据
                temp.CopyTo(data);
            }

            /// <summary>
            /// 变换一次数据 (4字节)
            /// </summary>
            /// <param name="data">数据</param>
            private unsafe void Transform4Bytes(Span<byte> data)
            {
                //暂存解密结果
                Span<byte> temp = stackalloc byte[4];

                uint order = 0x090D0B0E;

                Span<byte> orderPtr = new(&order, 4);

                //解密
                for (int index = 0; index < 4; index++)
                {
                    for(int i = 0; i < 4; ++i)
                    {
                        temp[index] ^= GetSubstitutionBox(data[i], orderPtr[i]);
                    }

                    order = BitOperations.RotateLeft(order, 8);
                }
                //回写覆盖原数据
                temp.CopyTo(data);
            }

            /// <summary>
            /// 解密一次key
            /// </summary>
            /// <param name="key">key</param>
            private void DecryptKey(Span<byte> key)
            {
                for (int index = 0; index < 16; ++index)
                {
                    key[index] ^= (byte)(0x7E - index);
                }

            }


            /// <summary>
            /// 生成表
            /// </summary>
            private void TableGenerator()
            {
                int blockStart = this.StartBlock;       //起始块索引
                int blockIndex = this.StartBlock;       //当前块索引
                int maxBlockIndex = this.DecryptRound * 4;      //最大块索引

                Span<byte> tablePtr = this.DecryptTable.AsSpan();
                Span<uint> tablePtrPack4 = MemoryMarshal.Cast<byte, uint>(tablePtr);

                //临时key
                uint key;
                //循环生成
                while (maxBlockIndex > blockIndex)
                {
                    key = tablePtrPack4[blockIndex - 1];

                    if (blockIndex % blockStart != 0)
                    {
                        if(blockIndex % blockStart == 4 && blockStart > 6)
                        {
                            //取S盒8异或
                            unsafe
                            {
                                Span<byte> keyBytePtr = new(&key, 4);
                                for (int i = 0; i < 4; ++i)
                                {
                                    keyBytePtr[i] = this.GetSubstitutionBox(this.mKeyInformation.SubstitutionBox8, keyBytePtr[i]);
                                }
                            }
                        }
                    }
                    else
                    {
                        key = BitOperations.RotateRight(key, 8);
                        //取S盒8异或
                        unsafe
                        {
                            Span<byte> keyBytePtr = new(&key, 4);
                            for (int i = 0; i < 4; ++i)
                            {
                                keyBytePtr[i] = this.GetSubstitutionBox(this.mKeyInformation.SubstitutionBox8, keyBytePtr[i]);
                            }
                            keyBytePtr[0] ^= this.mKeyInformation.SubKey[blockIndex / blockStart - 1];
                        }
                    }

                    tablePtrPack4[blockIndex] = tablePtrPack4[blockIndex - blockStart] ^ key;
                    ++blockIndex;
                }
            }



            /// <summary>
            /// 初始化加密上下文
            /// </summary>
            private void InitializeContext()
            {
                byte[] mKey = new byte[this.mKeyInformation.MainKey.Length];
                this.mKeyInformation.MainKey.CopyTo(mKey.AsSpan());
                this.DecryptKey(mKey);      //解密key

                this.KeyLength = mKey.Length;
                this.StartBlock = this.KeyLength / this.BlockSize;
                this.DecryptRound = this.StartBlock + 7;
                int tableLen = this.DecryptRound * 16;
                this.DecryptTable = new byte[tableLen];

                mKey.CopyTo(this.DecryptTable.AsSpan().Slice(0, mKey.Length));      //复制初始Key到加密表

                this.TableGenerator();
            }

            public DecryptContext(IKeyInformation keyInformation)
            {
                this.mKeyInformation = keyInformation;
                this.InitializeContext();
            }
        }

        private DecryptContext mDecryptContext;

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="path">文件路径(全路径)</param>
        /// <param name="extractpath">导出路径(全路径)</param>
        /// <returns></returns>
        public bool Decrypt(string path,string extractpath)
        {
            byte[] buffer = File.ReadAllBytes(path);

            //16字节对齐
            if (buffer.Length % 16 != 0)
            {
                return false;
            }

            Span<byte> data = buffer.AsSpan();
            int dataLen = data.Length;
            //每16字节解密
            for(int pos = 0; pos < dataLen; pos += 16)
            {
                this.mDecryptContext.Decrypt(data.Slice(pos, 16));
            }

            //移除对齐部分
            int alignSize = data[dataLen - 1];
            dataLen -= alignSize;
            data.Slice(0, dataLen);

            {
                string dir = Path.GetDirectoryName(extractpath);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
            }

            using FileStream mFs = new(extractpath, FileMode.Create, FileAccess.ReadWrite, FileShare.None);
            mFs.Write(data);
            mFs.Flush();

            return true;
        }

        /// <summary>
        /// 初始化加密状态
        /// </summary>
        private void InitializeContext(IKeyInformation keyInformation)
        {
            this.mDecryptContext = new(keyInformation);
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="keyInformation">游戏key信息</param>
        public Crypto128(IKeyInformation keyInformation)
        {
            this.InitializeContext(keyInformation);
        }
    }

    public interface IKeyInformation
    {
        /// <summary>
        /// S盒1  256字节
        /// <para>RVA 0x6120</para>
        /// </summary>
        public byte[] SubstitutionBox1 { get; }
        /// <summary>
        /// S盒2  256字节
        /// <para>RVA 0x6220</para>
        /// </summary>
        public byte[] SubstitutionBox2 { get; }
        /// <summary>
        /// S盒3  256字节
        /// <para>RVA 0x6320</para>
        /// </summary>
        public byte[] SubstitutionBox3 { get; }
        /// <summary>
        /// S盒4  256字节
        /// <para>RVA 0x6420</para>
        /// </summary>
        public byte[] SubstitutionBox4 { get; }
        /// <summary>
        /// S盒5  256字节
        /// <para>RVA 0x6520</para>
        /// </summary>
        public byte[] SubstitutionBox5 { get; }
        /// <summary>
        /// S盒6  256字节
        /// <para>RVA 0x6620</para>
        /// </summary>
        public byte[] SubstitutionBox6 { get; }
        /// <summary>
        /// S盒7  256字节
        /// <para>RVA 0x6720</para>
        /// </summary>
        public byte[] SubstitutionBox7 { get; }
        /// <summary>
        /// S盒8  256字节
        /// <para>RVA 0x6820</para>
        /// </summary>
        public byte[] SubstitutionBox8 { get; }

        /// <summary>
        /// 副Key
        /// <para>RVA 0x6920</para>
        /// </summary>
        public byte[] SubKey { get; }
        /// <summary>
        /// 主Key
        /// <para>RVA 0x5030</para>
        /// </summary>
        public byte[] MainKey { get; }
    }


}
