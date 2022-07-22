using System;
using System.Buffers;
using System.Runtime.InteropServices;

namespace Extractor
{
    public class Crypto128
    {
        /// <summary>
        /// 解密信息
        /// </summary>
        public struct DecryptInfo
        {
            /// <summary>
            /// 解密长度
            /// </summary>
            public int DecryptLength;
            /// <summary>
            /// 存放生成的Key表
            /// </summary>
            public byte[] DecryptTable;
            /// <summary>
            /// Key表长度
            /// </summary>
            public int DecryptTableLength;
            /// <summary>
            /// 轮解密次数
            /// </summary>
            public int DecryptRound;
            /// <summary>
            /// 表块起始点(块大小)
            /// </summary>
            public int StartBlock;
        }

        public bool IsInitialized { get; set; }

        private byte[] mKey;
        private DecryptInfo mDecryptInfo;

        private byte[] mSubstitutionBox1;
        private byte[] mSubstitutionBox2;
        private byte[] mSubstitutionBox3;
        private byte[] mSubstitutionBox4;
        private byte[] mSubstitutionBox5;
        private byte[] mSubstitutionBox6;
        private byte[] mSubstitutionBox7;
        private byte[] mSubstitutionBox8;

        private byte[] mXorVector;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="key">解密Key</param>
        /// <param name="xorVector">异或向量</param>
        public Crypto128(byte[] key, byte[] xorVector)
        {
            this.mKey = new byte[16];
            key.CopyTo(this.mKey, 0);
            this.mXorVector = xorVector;
        }

        /// <summary>
        /// 初始化Key表与解密S盒
        /// </summary>
        /// <param name="substitutionBox1">S盒1</param>
        /// <param name="substitutionBox2">S盒2</param>
        /// <param name="substitutionBox3">S盒3</param>
        /// <param name="substitutionBox4">S盒4</param>
        /// <param name="substitutionBox5">S盒5</param>
        /// <param name="substitutionBox6">S盒6</param>
        /// <param name="substitutionBox7">S盒7</param>
        /// <param name="substitutionBox8">S盒8</param>
        public void Initialize(byte[] substitutionBox1, byte[] substitutionBox2, byte[] substitutionBox3,
                               byte[] substitutionBox4, byte[] substitutionBox5, byte[] substitutionBox6,
                               byte[] substitutionBox7, byte[] substitutionBox8)
        {

            this.mSubstitutionBox1 = substitutionBox1;
            this.mSubstitutionBox2 = substitutionBox2;
            this.mSubstitutionBox3 = substitutionBox3;
            this.mSubstitutionBox4 = substitutionBox4;
            this.mSubstitutionBox5 = substitutionBox5;
            this.mSubstitutionBox6 = substitutionBox6;
            this.mSubstitutionBox7 = substitutionBox7;
            this.mSubstitutionBox8 = substitutionBox8;

            //Key初始化(解密)
            for (int index = 0; index < 16; index++)
            {
                this.mKey[index] ^= (byte)(39 - index);
            }

            //解密信息初始化
            this.mDecryptInfo = new();
            this.mDecryptInfo.DecryptLength = 16;   //解密长度
            this.mDecryptInfo.StartBlock = this.mDecryptInfo.DecryptLength / 4;     //设置当前块位置  (一个块为4字节)
            this.mDecryptInfo.DecryptRound = this.mDecryptInfo.StartBlock + 7;   //设置解密轮数
            this.mDecryptInfo.DecryptTableLength = this.mDecryptInfo.DecryptRound * 16; //设置解密表长度

            this.mDecryptInfo.DecryptTable = new byte[this.mDecryptInfo.DecryptTableLength];

            //复制key进解密表
            this.mKey.CopyTo(this.mDecryptInfo.DecryptTable, 0);

            //解密信息初始化完成生成key表
            this.CreateKeyTable();

            this.IsInitialized = true;
        }

        /// <summary>
        /// 生成Key表
        /// </summary>
        /// <remarks>使用S盒8</remarks>
        private void CreateKeyTable()
        {
            int blockIndex = this.mDecryptInfo.StartBlock; //当前块位置
            int blockSize = this.mDecryptInfo.StartBlock;  //块大小
            int maxBlockIndex = this.mDecryptInfo.DecryptRound * 4;     //最大块大小

            //最后生成的key
            Span<byte> lastKeyBytes = stackalloc byte[4];
            //循环生成
            while (maxBlockIndex > blockIndex)
            {
                uint lastKey = BitConverter.ToUInt32(this.mDecryptInfo.DecryptTable, (blockIndex - 1) * 4); //获取上一次最后4字节作为key

                AssemblyEmulator.ROR(ref lastKey, 8);   //循环右移

                BitConverter.TryWriteBytes(lastKeyBytes, lastKey);      //回写栈缓存(最后一次key)

                //查表取S盒
                for (int index = 0; index < 4; index++)
                {
                    lastKeyBytes[index] = this.mSubstitutionBox8[lastKeyBytes[index]];
                }

                //异或向量
                lastKeyBytes[0] ^= this.mXorVector[blockIndex / 4 - 1];

                //每4块生成key表
                for (int blockLoop = 0; blockLoop < blockSize; blockLoop++)
                {
                    //生成Key表(4*4字节)
                    for (int index = 0; index < 4; index++)
                    {
                        this.mDecryptInfo.DecryptTable[blockIndex * 4 + index] = (byte)(lastKeyBytes[index] ^ this.mDecryptInfo.DecryptTable[(blockIndex - 4) * 4 + index]);
                    }

                    blockIndex++;       //块索引自增
                    //检查块是否超过最大数量
                    if (maxBlockIndex <= blockIndex)
                    {
                        break;
                    }

                    lastKey = BitConverter.ToUInt32(this.mDecryptInfo.DecryptTable, (blockIndex - 1) * 4); //获取上一次最后4字节作为key
                    BitConverter.TryWriteBytes(lastKeyBytes, lastKey);  //回写栈缓存(最后一次key)
                }
            }
        }

        /// <summary>
        /// 解密16字节1
        /// </summary>
        /// <param name="data">数据</param>
        public bool Decrypt16Bytes_1(Span<byte> data)
        {
            //检查数据有效性
            if (data == null || data.Length != 16)
            {
                return false;       
            }

            //暂存解密结果
            Span<byte> temp = stackalloc byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};

            //解密
            for (int index = 0; index < 16; index++)
            {
                temp[index] = data[(4 * (16 - index) + index) % 16];
            }
            //回写覆盖原数据
            temp.CopyTo(data);

            return true;
        }
        /// <summary>
        /// 解密8字节1
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="order">序号</param>
        /// <returns>表数据</returns>
        /// <remarks>使用S盒1-6</remarks>
        public byte Decrypt1Byte_1(byte data, int order)
        {
            //跳转取key
            switch (order)
            {
                case 0: 
                case 1:
                    return data;
                case 2:
                    return this.mSubstitutionBox6[data];
                case 3:
                    return this.mSubstitutionBox5[data];
                case 4:
                case 5:
                case 6: 
                case 7:
                case 8:
                    return data;
                case 9:
                    return this.mSubstitutionBox4[data];
                case 10:
                    return data;
                case 11:
                    return this.mSubstitutionBox3[data];
                case 12:
                    return data;
                case 13:
                    return this.mSubstitutionBox2[data];
                case 14:
                    return this.mSubstitutionBox1[data];
                default:
                    return data;
            }
        }
        /// <summary>
        /// 加密4字节数据
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="offset">偏移</param>
        public void Decrypt4Bytes_1(Span<byte> data, int offset)
        {
            //暂存解密结果
            Span<byte> temp8 = stackalloc byte[4] { 0, 0, 0, 0 };
            //跳转表序号
            Span<int> orderList = stackalloc int[4] { 0xE, 0xB, 0xD, 0x9 };

            //待写入目标地址   4字节一组
            Span<byte> destData = data.Slice(offset, 4);

            //解密
            for(int index = 0; index < 4; index++)
            {
                temp8[index] = (byte)(this.Decrypt1Byte_1(destData[0], orderList[(4 - index + 0) % 4]) ^
                                      this.Decrypt1Byte_1(destData[1], orderList[(4 - index + 1) % 4]) ^
                                      this.Decrypt1Byte_1(destData[2], orderList[(4 - index + 2) % 4]) ^
                                      this.Decrypt1Byte_1(destData[3], orderList[(4 - index + 3) % 4]));
            }
            //回写覆盖原数据
            temp8.CopyTo(destData);
        }
        /// <summary>
        /// 解密16字节数据
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns></returns>
        /// <remarks>使用S盒7</remarks>
        public bool Decrypt16BytesData_1(Span<byte> data)
        {
            //数据检查
            if (data == null || data.Length != 16)
            {
                return false;
            }

            //获取轮解密次数
            int round = this.mDecryptInfo.DecryptRound;
            //获取解密表
            byte[] key = this.mDecryptInfo.DecryptTable;


            //第一轮解密
            for(int index = 0; index < 16; index++)
            {
                data[index] ^= key[(round - 1) * 16 + index];
            }

            //第二轮解密
            round -= 2;
            while (round >= 0)
            {
                //16字节解密1
                this.Decrypt16Bytes_1(data);
                //取S盒表
                for(int index = 0; index < 16; index++)
                {
                    data[index] = this.mSubstitutionBox7[data[index]];
                }
                //2-1轮解密
                for(int index = 0; index < 16; index++)
                {
                    data[index] ^= key[round * 16 + index];
                }
                
                //最后一次解密不执行4*4字节解密操作
                if (round == 0)
                {
                    break;
                }

                //2-2解密4*4字节解密
                for(int index = 0; index < 16; index += 4)
                {
                    this.Decrypt4Bytes_1(data, index);
                }
                round--;    //轮解密循环-1
            }
            return true;
        }

    }
}
