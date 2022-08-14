using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace BlueAngel.V1
{

    /// <summary>
    /// 资源加解密
    /// </summary>
    public class ArchiveCrypto
    {
        /// <summary>
        /// 初始化生成S盒
        /// </summary>
        /// <param name="SBox32_1">32位S盒1  数组长度0x100</param>
        /// <param name="SBox32_2">32位S盒2  数组长度0x100</param>
        /// <param name="SBox32_3">32位S盒3  数组长度0x100</param>
        /// <param name="SBox32_4">32位S盒4  数组长度0x100</param>
        /// <param name="SBox32_5">32位S盒5  数组长度0x100</param>
        /// <param name="SBox32_6">32位S盒6  数组长度0x100</param>
        /// <param name="SBox32_7">32位S盒7  数组长度0x100</param>
        /// <param name="SBox32_8">32位S盒8  数组长度0x100</param>
        /// <param name="SBox32_9">32位S盒9  数组长度0x100</param>
        /// <param name="SBox8_1">8位S盒1  数组长度0x100<</param>
        /// <param name="SBox8_2">8位S盒2  数组长度0x100<</param>
        /// <remarks>《亿万年的星光》SBox32_1为VA:0x00A00CC0 SBox32_2为VA:0x00A011C0 SBox32_3为VA:0x00A01250 SBox32_4为VA:0x00A01650 SBox32_5为VA:0x00A01A50 SBox32_6为VA:0x00A01E50 SBox32_7为VA:0x00A02250 SBox32_8为VA:0x00A02650 SBox32_9为VA:0x00A02A50 SBox8_1为VA:0x00A00BC0 SBox8_2为VA:0x00A010C0</remarks>
        public static void SubstitutionBoxInitialize(out List<uint> SBox32_1, out List<uint> SBox32_2, 
                                                     out List<uint> SBox32_3, out List<uint> SBox32_4,
                                                     out List<uint> SBox32_5, out List<uint> SBox32_6,
                                                     out List<uint> SBox32_7, out List<uint> SBox32_8,
                                                     out List<uint> SBox32_9, 
                                                     out List<byte> SBox8_1, out List<byte> SBox8_2)
        {
            //初始化S盒空间
            SBox32_1 = new List<uint>();
            SBox32_2 = new List<uint>();
            SBox32_3 = new List<uint>();
            SBox32_4 = new List<uint>();
            SBox32_5 = new List<uint>();
            SBox32_6 = new List<uint>();
            SBox32_7 = new List<uint>();
            SBox32_8 = new List<uint>();
            SBox32_9 = new List<uint>();

            SBox8_1 = new List<byte>();
            SBox8_2 = new List<byte>();

            for(int loop = 0; loop < 256; loop++)
            {
                SBox32_1.Add(0);
                SBox32_2.Add(0);
                SBox32_3.Add(0);
                SBox32_4.Add(0);
                SBox32_5.Add(0);
                SBox32_6.Add(0);
                SBox32_7.Add(0);
                SBox32_8.Add(0);
                SBox32_9.Add(0);

                SBox8_1.Add(0);
                SBox8_2.Add(0);
            }


            //生成栈空间 lea esp,dword ptr ss:[esp-0x808]
            List<uint> Stack = new List<uint>();
            for(int loop = 0; loop < 0x202; loop++)
            {
                Stack.Add(0);
            }
            int EBP = Stack.Count;

            Stack[EBP - 0x202] = 1;

            uint roundVar = 1;

            //栈生成因子  第一轮循环
            for(uint loop = 0; loop < 256; loop++)
            {
                Stack[EBP + (int)loop - 0x201] = roundVar;
                Stack[EBP + (int)roundVar - 0x101] = loop;

                //and al,0x80
                //movzx ecx,al
                uint ecx = roundVar & 0x00000080;
                //neg ecx
                //sbb ecx
                ecx = ecx == 0 ? 0 : 0xFFFFFFFF;

                ecx &= 0x0000001B;
                ecx ^= (roundVar * 2) ^ (roundVar);
                roundVar = ecx & 0x000000FF;
            }

            roundVar = 1;
            //静态表生成1 第二轮循环
            for(int loop = 0; loop < 10; loop++)
            {
                SBox32_2[loop] = roundVar;

                //and al,0x80
                //movzx ecx,al
                uint ecx = roundVar & 0x00000080;
                //neg ecx
                //sbb ecx
                ecx = ecx == 0 ? 0 : 0xFFFFFFFF;

                ecx &= 0x0000001B;
                ecx ^= (roundVar * 2);
                roundVar = ecx & 0x000000FF;
            }

            SBox8_1[0x00] = 0x63;
            SBox8_2[0x63] = 0x00;

            roundVar = 1;
            //生成两个8字节S盒 第三轮循环
            for (int loop = 1; loop < 256; loop++)
            {
                uint ebx = Stack[EBP - 0x102 - (int)(((Stack[EBP + loop - 0x101]) << 2) / 4)];

                uint ecx = ebx;
                // sar ecx,0x7
                if ((ecx & 0x80000000) == 0x80000000)
                {
                    uint mask = 0xFE000000;
                    ecx >>= 7;
                    ecx |= mask;
                }
                else
                {
                    ecx >>= 7;
                }
                //lea eax, dword ptr ds:[ebx+ebx]
                //or ecx, eax
                ecx |= (ebx * 2);

                uint eax,edx;

                eax = ecx & 0x000000FF;
                ebx ^= eax;
                ecx = (eax >> 7) | (eax * 2);

                eax = ecx & 0x000000FF;
                ebx ^= eax;
                ecx = (eax >> 7) | (eax * 2);

                edx = ecx & 0x000000FF;
                ecx = ((edx >> 7) | (edx * 2)) ^ 0x00000063;

                eax = (ecx & 0x000000FF) ^ edx;
                ebx ^= eax;

                SBox8_1[loop] = (byte)(ebx & 0x000000FF);
                SBox8_2[(int)(ebx & 0x000000FF)] = (byte)(loop & 0x000000FF);
            }
            Stack[EBP - 0x202] = 256;

            //生成8个32位长度为256的S盒数组
            for(int loop = 0; loop < 256; loop++)
            {
                //使用第一张1字节S盒作为Seed
                uint mSB8_1 = SBox8_1.ElementAt(loop);

                uint keySB1 = mSB8_1;
                uint tempSB1;

                keySB1 &= 0x00000080;
                keySB1 = keySB1 == 0 ? 0 : 0xFFFFFFFF;
                keySB1 &= 0x0000001B;
                keySB1 ^= (mSB8_1 * 2);
                tempSB1 = keySB1 & 0x000000FF;
                keySB1 = tempSB1;

                keySB1 ^= mSB8_1;
                keySB1 <<= 8;
                keySB1 ^= mSB8_1;
                keySB1 <<= 8;
                keySB1 ^= mSB8_1;
                keySB1 <<= 8;
                keySB1 ^= tempSB1;

                SBox32_8[loop] = keySB1;
                AssemblyEmulator.ROL(ref keySB1, 8);
                SBox32_7[loop] = keySB1;
                AssemblyEmulator.ROL(ref keySB1, 8);
                SBox32_4[loop] = keySB1;
                AssemblyEmulator.ROL(ref keySB1, 8);
                SBox32_3[loop] = keySB1;

                //使用第二张S盒作为Seed
                uint mSB8_2 = SBox8_2.ElementAt(loop);
                if (mSB8_2 != 0)
                {
                    uint tempSB2_0 = Stack[EBP - 0x201 + ((int)(Stack[EBP - 0x101 + (int)mSB8_2] + Stack[EBP - 0xF3]) % 0x000000FF)];
                    uint tempSB2_1 = Stack[EBP - 0x201 + ((int)(Stack[EBP - 0x101 + (int)mSB8_2] + Stack[EBP - 0xF8]) % 0x000000FF)];
                    uint tempSB2_2 = Stack[EBP - 0x201 + ((int)(Stack[EBP - 0x101 + (int)mSB8_2] + Stack[EBP - 0xF4]) % 0x000000FF)];
                    uint tempSB2_3 = Stack[EBP - 0x201 + ((int)(Stack[EBP - 0x101 + (int)mSB8_2] + Stack[EBP - 0xF6]) % 0x000000FF)];

                    uint keySB2 = tempSB2_3;
                    keySB2 <<= 8;
                    keySB2 ^= tempSB2_2;
                    keySB2 <<= 8;
                    keySB2 ^= tempSB2_1;
                    keySB2 <<= 8;
                    keySB2 ^= tempSB2_0;

                    SBox32_6[loop] = keySB2;
                    AssemblyEmulator.ROL(ref keySB2, 8);
                    SBox32_5[loop] = keySB2;
                    AssemblyEmulator.ROL(ref keySB2, 8);
                    SBox32_9[loop] = keySB2;
                    AssemblyEmulator.ROL(ref keySB2, 8);
                    SBox32_1[loop] = keySB2;

                    Stack[EBP - 0x202] = tempSB2_0;
                }
                else
                {
                    Stack[EBP - 0x202] = 0;
                    SBox32_6[loop] = 0;
                    SBox32_5[loop] = 0;
                    SBox32_9[loop] = 0;
                    SBox32_1[loop] = 0;
                }
            }
            //4字节S盒2与4字节S盒3有重合
            for(int loop = 36; loop < 256; loop++)
            {
                SBox32_2[loop] = SBox32_3.ElementAt(loop - 36);
            }
        }
        /// <summary>
        /// 资源解密
        /// </summary>
        /// <param name="data">资源数据</param>
        /// <param name="length">解密长度</param>
        /// <param name="round">key加密轮数</param>
        /// <param name="key16Bytes">16字节原key</param>
        /// <param name="key256Bytes">256字节key表</param>
        /// <param name="key32Table1">4字节S盒表1</param>
        /// <param name="key32Table2">4字节S盒表2</param>
        /// <param name="key32Table3">4字节S盒表3</param>
        /// <param name="key32Table4">4字节S盒表4</param>
        /// <param name="key8Table1">1字节S盒表1</param>
        /// <remarks>《亿万年的星光》 S盒32_1为VA:0x00A01250  S盒32_2为VA:0x00A01650  S盒32_3为VA:0x00A02250  S盒32_4为VA:0x00A02650  S盒8_1为VA:0x00A0BC0</remarks>
        public static bool DecryptArchive(byte[] data,int length, int round,
                                          byte[] key16Bytes,
                                          List<uint> key256Bytes,
                                          List<uint> key32Table1, List<uint> key32Table2,
                                          List<uint> key32Table3, List<uint> key32Table4,
                                          List<byte> key8Table1)
        {
            if (data == null || data.Length <= 0|| length <= 0 || key16Bytes.Length != 16)
            {
                return false;       //数据有效性检查
            }

            int decryptLength = Math.Min(data.Length, length);      //取解密长度
            int dataPointer = 0;

            //循环解密
            while (true)
            {
                //转化key为Dword型
                List<uint> uintKey16Group = new List<uint>();
                for(int index = 0; index < key16Bytes.Length; index += 4)
                {
                    uintKey16Group.Add(BitConverter.ToUInt32(key16Bytes, index));
                }

                //获取解密key
                List<byte> decryptKey = CreateDecryptKey16(uintKey16Group, round, key256Bytes, 
                                                           key32Table1, key32Table2, key32Table3, key32Table4, 
                                                           key8Table1);
                if (decryptKey == null)
                {
                    return false;
                }

                //循环解密数据
                foreach(byte KeyByte in decryptKey)
                {
                    data[dataPointer] ^= KeyByte;
                    dataPointer++;      //数据指针自增
                    if (dataPointer >= decryptLength)
                    {
                        return true;
                    }
                }

                //原key从后向前自增
                for(int index = key16Bytes.Length - 1; index >= 0; index--)
                {
                    key16Bytes[index]++;
                    if (key16Bytes[index] != 0)
                    {
                        break;
                    }
                }

            }
        }
        /// <summary>
        /// 生成16字节key
        /// </summary>
        /// <param name="key16bytes">原16字节key</param>
        /// <param name="round">解密次数</param>
        /// <param name="key256bytes">256字节解密表</param>
        /// <param name="key32Table1">4字节S盒表1</param>
        /// <param name="key32Table2">4字节S盒表2</param>
        /// <param name="key32Table3">4字节S盒表3</param>
        /// <param name="key32Table4">4字节S盒表4</param>
        /// <param name="key8Table1">1字节S盒表1</param>
        /// <returns>最终解密key</returns>
        public static List<byte> CreateDecryptKey16(List<uint> key16bytes, int round, List<uint> key256bytes,
                                                    List<uint> key32Table1, List<uint> key32Table2,
                                                    List<uint> key32Table3, List<uint> key32Table4,
                                                    List<byte> key8Table1)
        {
            if (key16bytes.Count != 4 || round <= 0 || round > 15 || key256bytes.Count != 64)
            {
                return null;        //key长度检查
            }

            int Key256Index = 0;

            //第一轮解密
            Parallel.For(0, key16bytes.Count, index =>
            {
                key16bytes[index] ^= key256bytes[Key256Index + index];
            });

            Key256Index += 4;   //第一轮解密完毕
            round--;

            //循环轮解密
            while (round > 0)
            {
                List<uint> tempKey16 = new List<uint>() { 0, 0, 0, 0 };     //存放临时生成的key
                Parallel.For(0, key16bytes.Count, index =>
                {

                    //获取key表索引
                    int tableIndex4 = (int)((key16bytes.ElementAt((index + 0) % key16bytes.Count) & 0x000000FF) >> 0x00);
                    int tableIndex3 = (int)((key16bytes.ElementAt((index + 1) % key16bytes.Count) & 0x0000FF00) >> 0x08);
                    int tableIndex2 = (int)((key16bytes.ElementAt((index + 2) % key16bytes.Count) & 0x00FF0000) >> 0x10);
                    int tableIndex1 = (int)((key16bytes.ElementAt((index + 3) % key16bytes.Count) & 0xFF000000) >> 0x18);
                    //使用key表解密
                    tempKey16[index] = key32Table4.ElementAt(tableIndex4) ^
                                       key32Table3.ElementAt(tableIndex3) ^
                                       key32Table2.ElementAt(tableIndex2) ^
                                       key32Table1.ElementAt(tableIndex1) ^
                                       key256bytes.ElementAt(Key256Index + index);
                });

                key16bytes = tempKey16;
                Key256Index += 4;      //一次轮解密完毕
                round--;
            }

            //最后一轮解密
            List<uint> tempLastKey16 = new List<uint>() { 0, 0, 0, 0 };

            Parallel.For(0, key16bytes.Count, index =>
            {
                //获取key表索引
                int tableIndex4 = (int)((key16bytes.ElementAt((index + 0) % key16bytes.Count) & 0x000000FF) >> 0x00);
                int tableIndex3 = (int)((key16bytes.ElementAt((index + 1) % key16bytes.Count) & 0x0000FF00) >> 0x08);
                int tableIndex2 = (int)((key16bytes.ElementAt((index + 2) % key16bytes.Count) & 0x00FF0000) >> 0x10);
                int tableIndex1 = (int)((key16bytes.ElementAt((index + 3) % key16bytes.Count) & 0xFF000000) >> 0x18);

                tempLastKey16[index] = (((uint)key8Table1.ElementAt(tableIndex4)) << 0x00) ^
                                       (((uint)key8Table1.ElementAt(tableIndex3)) << 0x08) ^
                                       (((uint)key8Table1.ElementAt(tableIndex2)) << 0x10) ^
                                       (((uint)key8Table1.ElementAt(tableIndex1)) << 0x18) ^
                                       key256bytes.ElementAt(Key256Index + index);
            });
            Key256Index += 4;      //最后一轮解密完毕

            //获取解密key数组
            List<byte> decryptKeyBytes = new List<byte>();
            tempLastKey16.ForEach(temp =>
            {
                decryptKeyBytes.AddRange(BitConverter.GetBytes(temp));
            });

            return decryptKeyBytes;
        }

        /// <summary>
        /// 生成初始解密Key
        /// </summary>
        /// <param name="length">资源长度</param>
        /// <returns></returns>
        public static byte[] CreateOriginalKey16(uint length)
        {
            List<byte> seedBytes;

            //使用资源长度生成Hash种子
            uint seed = length ^ 0xBFAF8EFD;
            seedBytes = Encoding.UTF8.GetBytes(seed.ToString("X8").ToUpper()).ToList();
            seedBytes.Add(0x00);

            //生成第一轮Hash表
            HashTable hashTable = AllocHashTable(0x20);
            CreateHashTable(ref hashTable, seedBytes);
            CreateHash(ref hashTable, false);

            //获取第一轮生成作为Seed
            seedBytes = hashTable.Hash.ToList().GetRange(0, 0x20);

            //生成第二轮Hash表
            hashTable = AllocHashTable(0x10);
            CreateHashTable(ref hashTable, seedBytes);
            CreateHash(ref hashTable, false);

            //返回第二轮生成作为Key
            return hashTable.Hash.ToList().GetRange(0, 0x10).ToArray();
        }

        /// <summary>
        /// 生成256解密表一部分Key(32字节)
        /// </summary>
        /// <param name="length">资源长度</param>
        /// <returns></returns>
        public static byte[] CreateDecryptKey32(uint length)
        {
            List<byte> seedBytes;

            //使用资源长度生成Hash种子
            uint seed = length ^ 0xBFAF8EFD;
            seedBytes = Encoding.UTF8.GetBytes(seed.ToString("X2").ToUpper()).ToList();
            seedBytes.Add(0x00);

            //生成第一轮Hash表
            HashTable hashTable = AllocHashTable(0x20);
            CreateHashTable(ref hashTable, seedBytes);
            CreateHash(ref hashTable, false);

            //获取第一轮生成作为Seed
            seedBytes = hashTable.Hash.ToList().GetRange(0, 0x20);

            //生成最终Hash表
            hashTable = AllocHashTable(0x20);

            //长度与0x10取最小值
            uint round = Math.Min(length, 0x10);
            //sbb edi,edi   and edi,eax
            round = (round & 0x10) + 0x10;
            //cmp dword ptr ss:[esp+14],0x20   cmova edi,edx
            round = length > 0x20 ? 0x40 : round;
            //cmp dword ptr ss:[esp+10],edx   cmova edi,ecx
            round = length > 0x40 ? 0x80 : round;

            //循环迭代Hash表
            for(uint loop = 0; loop < round; loop++)
            {
                CreateHashTable(ref hashTable, seedBytes);
            }
            CreateHash(ref hashTable, false);

            return hashTable.Hash.ToList().GetRange(0, 0x20).ToArray();
        }
        /// <summary>
        /// 生成256字节解密表
        /// </summary>
        /// <param name="length">资源长度</param>
        /// <param name="round">轮解密次数</param>
        /// <param name="substitutionBox8">8字节S盒表</param>
        /// <param name="substitutionBox32">32字节S盒表</param>
        /// <remarks>《亿万年的星光》 8字节S盒为VA:0x00A00BC0   32字节S盒为VA:0x00A011C0</remarks>
        public static List<uint> CreateDecryptTable256(uint length ,int round ,List<byte> substitutionBox8 ,List<uint> substitutionBox32)
        {
            List<uint> KeyTable256 = new List<uint>();

            //生成32字节key
            byte[] key32 = CreateDecryptKey32(length);
            //添加32字节key
            for(int index = 0; index < key32.Length; index += 4)
            {
                KeyTable256.Add(BitConverter.ToUInt32(key32, index));
            }

            //填充至256字节
            while (true)
            {
                KeyTable256.Add(0);
                if (KeyTable256.Count == 64)
                {
                    break;
                }
            }

            //判断轮解密次数
            if (round == 10)
            {
                int pointer = 0;
                uint key = KeyTable256.ElementAt(pointer);      //mov ecx, dword ptr ds:[edx]
                pointer += 4;           //lea edi,dword ptr ds:[edx+0x10]
                for(int loop = 0; loop < 0x28; loop += 4)
                {
                    uint index = KeyTable256.ElementAt(pointer - 1);        //mov edx,dword ptr ds:[edi-0x4]
                    uint tempKey = (uint)(substitutionBox8.ElementAt((int)((index & 0x000000FF) >> 0x00)) << 0x18) ^
                                   (uint)(substitutionBox8.ElementAt((int)((index & 0xFF000000) >> 0x18)) << 0x10) ^
                                   (uint)(substitutionBox8.ElementAt((int)((index & 0x00FF0000) >> 0x10)) << 0x08) ^
                                   (uint)(substitutionBox8.ElementAt((int)((index & 0x0000FF00) >> 0x08)) << 0x00);
                    tempKey ^= substitutionBox32.ElementAt(loop / 4);
                    tempKey ^= key;

                    key = tempKey;

                    KeyTable256[pointer + 0] = tempKey;
                    KeyTable256[pointer + 1] = KeyTable256.ElementAt(pointer + 0) ^ KeyTable256.ElementAt(pointer - 3);
                    KeyTable256[pointer + 2] = KeyTable256.ElementAt(pointer + 1) ^ KeyTable256.ElementAt(pointer - 2);
                    KeyTable256[pointer + 3] = KeyTable256.ElementAt(pointer + 2) ^ KeyTable256.ElementAt(pointer - 1);

                    pointer += 4;
                }
            }
            else if (round == 12)
            {
                int pointer = 0;
                uint key = KeyTable256.ElementAt(pointer);      //mov ecx, dword ptr ds:[edx]
                pointer += 6;           //lea edi,dword ptr ds:[edx+0x18]

                for(int loop = 0; loop < 0x20; loop += 4)
                {
                    uint index = KeyTable256.ElementAt(pointer - 1);        //mov edx,dword ptr ds:[edi-0x4]
                    uint tempKey = (uint)(substitutionBox8.ElementAt((int)((index & 0x000000FF) >> 0x00)) << 0x18) ^
                                   (uint)(substitutionBox8.ElementAt((int)((index & 0xFF000000) >> 0x18)) << 0x10) ^
                                   (uint)(substitutionBox8.ElementAt((int)((index & 0x00FF0000) >> 0x10)) << 0x08) ^
                                   (uint)(substitutionBox8.ElementAt((int)((index & 0x0000FF00) >> 0x08)) << 0x00);
                    tempKey ^= substitutionBox32.ElementAt(loop / 4);
                    tempKey ^= key;

                    key = tempKey;

                    KeyTable256[pointer + 0] = tempKey;
                    KeyTable256[pointer + 1] = KeyTable256.ElementAt(pointer + 0) ^ KeyTable256.ElementAt(pointer - 5);
                    KeyTable256[pointer + 2] = KeyTable256.ElementAt(pointer + 1) ^ KeyTable256.ElementAt(pointer - 4);
                    KeyTable256[pointer + 3] = KeyTable256.ElementAt(pointer + 2) ^ KeyTable256.ElementAt(pointer - 3);
                    KeyTable256[pointer + 4] = KeyTable256.ElementAt(pointer + 3) ^ KeyTable256.ElementAt(pointer - 2);
                    KeyTable256[pointer + 5] = KeyTable256.ElementAt(pointer + 4) ^ KeyTable256.ElementAt(pointer - 1);

                    pointer += 6;
                }

            }
            else if (round == 14)
            {
                int pointer = 0;
                uint key = KeyTable256.ElementAt(pointer);      //mov ecx, dword ptr ds:[edx]
                pointer += 8;           //add edx,0x20
                for (int loop = 0; loop < 0x1C; loop += 4)
                {
                    uint index = KeyTable256.ElementAt(pointer - 1);        //mov ebx,dword ptr ds:[edx-0x4]
                    uint tempKey = (uint)(substitutionBox8.ElementAt((int)((index & 0x000000FF) >> 0x00)) << 0x18) ^
                                   (uint)(substitutionBox8.ElementAt((int)((index & 0xFF000000) >> 0x18)) << 0x10) ^
                                   (uint)(substitutionBox8.ElementAt((int)((index & 0x00FF0000) >> 0x10)) << 0x08) ^
                                   (uint)(substitutionBox8.ElementAt((int)((index & 0x0000FF00) >> 0x08)) << 0x00);
                    tempKey ^= substitutionBox32.ElementAt(loop / 4);
                    tempKey ^= key;

                    key = tempKey;

                    KeyTable256[pointer + 0] = tempKey;
                    KeyTable256[pointer + 1] = KeyTable256.ElementAt(pointer + 0) ^ KeyTable256.ElementAt(pointer - 7);
                    KeyTable256[pointer + 2] = KeyTable256.ElementAt(pointer + 1) ^ KeyTable256.ElementAt(pointer - 6);
                    KeyTable256[pointer + 3] = KeyTable256.ElementAt(pointer + 2) ^ KeyTable256.ElementAt(pointer - 5);


                    index = KeyTable256.ElementAt(pointer + 3);
                    tempKey = (uint)(substitutionBox8.ElementAt((int)((index & 0xFF000000) >> 0x18)) << 0x18) ^
                              (uint)(substitutionBox8.ElementAt((int)((index & 0x00FF0000) >> 0x10)) << 0x10) ^
                              (uint)(substitutionBox8.ElementAt((int)((index & 0x0000FF00) >> 0x08)) << 0x08) ^
                              (uint)(substitutionBox8.ElementAt((int)((index & 0x000000FF) >> 0x00)) << 0x00);

                    KeyTable256[pointer + 4] = KeyTable256.ElementAt(pointer - 4) ^ tempKey;
                    KeyTable256[pointer + 5] = KeyTable256.ElementAt(pointer + 4) ^ KeyTable256.ElementAt(pointer - 3);
                    KeyTable256[pointer + 6] = KeyTable256.ElementAt(pointer + 5) ^ KeyTable256.ElementAt(pointer - 2);
                    KeyTable256[pointer + 7] = KeyTable256.ElementAt(pointer + 6) ^ KeyTable256.ElementAt(pointer - 1);

                    pointer += 8;
                }
            }
            return KeyTable256;
        }
        /// <summary>
        /// 初始化哈希表
        /// </summary>
        /// <param name="needLength">需要获取的长度</param>
        /// <returns>哈希表结构</returns>
        public static HashTable AllocHashTable(int needLength)
        {
            //生成Hash表
            HashTable hashTable = new HashTable
            {
                NeedLength = needLength,
                MaxSeedLength = (0x64 - needLength) << 1,
                Hash = new byte[0xC8]
            };
            return hashTable;
        }

        /// <summary>
        /// 生成Hash表
        /// </summary>
        /// <param name="hashTable">哈希表对象</param>
        /// <param name="seed">种子数组</param>
        /// <returns></returns>
        public static void CreateHashTable(ref HashTable hashTable ,List<byte> seed)
        {
            //生成种子到Hash表
            int seedPointer = 0;         //seed指针
            int hashTablePointer = hashTable.ActualSeedLength;   //hash指针
            while (seedPointer < seed.Count)
            {
                hashTable.Hash[hashTablePointer] ^= seed.ElementAt(seedPointer);

                //判断种子长度是否超过最大长度
                hashTablePointer++;
                if (hashTablePointer >= hashTable.MaxSeedLength)
                {
                    CreateHash(ref hashTable, true);        //生成一轮Hash
                    hashTablePointer = 0;
                }
                seedPointer++;
            }
            hashTable.ActualSeedLength = hashTablePointer;
        }

        /// <summary>
        /// 获取Unicode字符串Hash
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static byte[] UnicodeStringHash(string text)
        {
            List<byte> seedBytes;
            seedBytes = Encoding.Unicode.GetBytes(text).ToList();
            //计算Hash
            HashTable hashTable = AllocHashTable(0x10);
            CreateHashTable(ref hashTable, seedBytes);
            CreateHash(ref hashTable, false);

            return hashTable.Hash.ToList().GetRange(0, 0x10).ToArray();
        }

        /// <summary>
        /// 生成Hash
        /// </summary>
        /// <param name="hashTable">Hash表</param>
        /// <param name="IsCreateHashTableCall">是否来自表生成函数调用</param>
        public static void CreateHash(ref HashTable hashTable,bool IsCreateHashTableCall)
        {
            if(IsCreateHashTableCall == false)
            {
                //mov eax,dword ptr ds:[esi+0xC8]
                //xor byte ptr ds:[eax+esi], 0x06
                hashTable.Hash[hashTable.ActualSeedLength] ^= 0x06;
                //mov eax, dword ptr ds:[esi+0xCC]
                //xor byte ptr ds:[eax+esi-0x1], 0x80
                hashTable.Hash[hashTable.MaxSeedLength - 1] ^= 0x80;

            }


            List<uint> Stack = new List<uint>()
            {
                0x00000000,     //[ebp-0x1D4]

                0x00000005, 0x00000000, 0x00000000, 0x00000000,      //[ebp-0x1D0]
                0x00000000, 0x00000000, 0x00000000, 0x00000000,      //[ebp-0x1C0]

                0x0000000A, 0x00000007, 0x0000000B, 0x00000011,      //[ebp-0x1B0]
                0x00000012, 0x00000003, 0x00000005, 0x00000010,      //[ebp-0x1A0]
                0x00000008, 0x00000015, 0x00000018, 0x00000004,      //[ebp-0x190]
                0x0000000F, 0x00000017, 0x00000013, 0x0000000D,      //[ebp-0x180]
                0x0000000C, 0x00000002, 0x00000014, 0x0000000E,      //[ebp-0x170]
                0x00000016, 0x00000009, 0x00000006, 0x00000001,      //[ebp-0x160]
                0x00000001, 0x00000003, 0x00000006, 0x0000000A,      //[ebp-0x150]
                0x0000000F, 0x00000015, 0x0000001C, 0x00000024,      //[ebp-0x140]
                0x0000002D, 0x00000037, 0x00000002, 0x0000000E,      //[ebp-0x130]
                0x0000001B, 0x00000029, 0x00000038, 0x00000008,      //[ebp-0x120]
                0x00000019, 0x0000002B, 0x0000003E, 0x00000012,      //[ebp-0x110]
                0x00000027, 0x0000003D, 0x00000014, 0x0000002C,      //[ebp-0x100]

                0x00000001, 0x00000000, 0x00008082, 0x00000000,      //[ebp-0xF0]   
                0x0000808A, 0x80000000, 0x80008000, 0x80000000,      //[ebp-0xE0]
                0x0000808B, 0x00000000, 0x80000001, 0x00000000,      //[ebp-0xD0]
                0x80008081, 0x80000000, 0x00008009, 0x80000000,      //[ebp-0xC0]
                0x0000008A, 0x00000000, 0x00000088, 0x00000000,      //[ebp-0xB0]
                0x80008009, 0x00000000, 0x8000000A, 0x00000000,      //[ebp-0xA0]
                0x8000808B, 0x00000000, 0x0000008B, 0x80000000,      //[ebp-0x90]
                0x00008089, 0x80000000, 0x00008003, 0x80000000,      //[ebp-0x80]
                0x00008002, 0x80000000, 0x00000080, 0x80000000,      //[ebp-0x70]
                0x0000800A, 0x00000000, 0x8000000A, 0x80000000,      //[ebp-0x60]
                0x80008081, 0x80000000, 0x00008080, 0x80000000,      //[ebp-0x50]
                0x80000001, 0x00000000, 0x80008008, 0x80000000,      //[ebp-0x40]

                0x00000000, 0x00000000, 0x00000000, 0x00000000,      //[ebp-0x30]

                0x00000000, 0x00000000, 0x00000000, 0x00000000,      //[ebp-0x20]
                0x00000000, 0x00000000, 0x00000000, 0x00000000,      //[ebp-0x10]
                //[ebp-4]  栈保护
                0x00000000      //[ebp]     原ebp
            };

            int EBP = Stack.Count - 1;      //栈帧

            int Pointer = 0x00000078;       //hash表hash字节索引

            //第一层循环
            //[ebp-0x1C8]
            for(int loopCount = 0; loopCount < 0x00000018; loopCount++)
            {
                //第一轮
                for(int loop = 0; loop < 5; loop++)
                {
                    uint temp1 = BitConverter.ToUInt32(hashTable.Hash, Pointer - 0x78) ^
                                 BitConverter.ToUInt32(hashTable.Hash, Pointer - 0x50) ^
                                 BitConverter.ToUInt32(hashTable.Hash, Pointer - 0x28) ^
                                 BitConverter.ToUInt32(hashTable.Hash, Pointer + 0x28) ^
                                 BitConverter.ToUInt32(hashTable.Hash, Pointer);

                    uint temp2 = BitConverter.ToUInt32(hashTable.Hash, Pointer - 0x74) ^
                                 BitConverter.ToUInt32(hashTable.Hash, Pointer - 0x4C) ^
                                 BitConverter.ToUInt32(hashTable.Hash, Pointer - 0x24) ^
                                 BitConverter.ToUInt32(hashTable.Hash, Pointer + 0x2C) ^
                                 BitConverter.ToUInt32(hashTable.Hash, Pointer + 0x04);

                    Pointer += 0x08;

                    Stack[EBP + loop * 2 - 0x0C] = temp1;         //[ebp+edi*0x8-0x30]
                    Stack[EBP + loop * 2 - 0x0B] = temp2;         //[ebp+edi*0x8-0x2C]
                }


                uint round2 = 4;     //[ebp-0x1B4]
                Pointer = 0;

                //第二轮
                //[ebp-0x1B8]
                for (int loop = 5; loop > 0; loop--)
                {
                    uint edx = (uint)((int)(round2-3) % 5);  //edx

                    uint ecx = Stack[EBP + (int)edx * 2 - 0x0C];       //mov ecx,dword ptr ss:[ebp+edx*0x8-0x30]
                    uint eax = Stack[EBP + (int)edx * 2 - 0x0B];       //mov eax,dword ptr ss:[ebp+edx*0x8-0x2C]

                    edx = eax;   //mov edx,eax

                    //shld eax,ecx,0x1;
                    eax <<= 1;
                    eax |= ((ecx & 0x80000000) >> 31);

                    edx >>= 31;      //shr edx,0x1F
                    ecx <<= 1;       //add ecx,ecx
                    edx |= ecx;      //or edx,ecx
                    ecx = eax;       //sub ecx,ecx  or ecx,eax

                    Stack[EBP - 0x71] = edx;       //mov dword ptr ss:[ebp-0x1C4],edx
                    Stack[EBP - 0x6F] = ecx;       //mov dword ptr ss:[ebp-0x1BC],ecx

                    edx = (uint)((int)round2 % 5);

                    ecx = Stack[EBP - 0x71] ^ Stack[EBP + (int)(edx * 2) - 0x0C];
                    eax = Stack[EBP - 0x6F] ^ Stack[EBP + (int)(edx * 2) - 0x0B];

                    Stack[EBP - 0x6F] = eax;       //mov dword ptr ss:[ebp-0x1BC],eax

                    
                    //循环2-1
                    edx = 5;
                    int mPointer = Pointer;
                    while (edx != 0)
                    {
                        uint temp;

                        temp = BitConverter.ToUInt32(hashTable.Hash, mPointer) ^ ecx;
                        Array.Copy(BitConverter.GetBytes(temp), 0, hashTable.Hash, mPointer, 4);

                        temp = BitConverter.ToUInt32(hashTable.Hash, mPointer + 0x04) ^ eax;
                        Array.Copy(BitConverter.GetBytes(temp), 0, hashTable.Hash, mPointer + 0x04, 4);

                        mPointer += 0x28;
                        edx--;
                    }

                    Pointer += 0x08;
                    round2 ++;        
                }

                Pointer = 0;            //mov esi,dword ptr ss:[ebp-0x1C0]
                Stack[EBP - 0x6F] = 0;  //mov dword ptr ss:[ebp-0x1BC],edx
                //mov edi,dword ptr ds:[esi+0x8]
                //mov eax,dword ptr ds:[esi+0xC]
                //mov dword ptr ss:[ebp-0x1B8],edi
                //mov dword ptr ss:[ebp-0x1B4],eax
                Stack[EBP - 0x6E] = BitConverter.ToUInt32(hashTable.Hash, Pointer + 0x08);
                Stack[EBP - 0x6D] = BitConverter.ToUInt32(hashTable.Hash, Pointer + 0x0C);

                uint ediLoop3 = Stack[EBP - 0x6E];
                //第三轮
                //[ebp-0x1BC]
                for (int loop = 0; loop < 0x60; loop += 0x04)
                {
                    uint temp;
                    uint esi;
                    uint ebx = Stack[EBP + loop / 4 - 0x6C];        //mov ebx,dword ptr ss:[ebp+edx-0x1B0]
                    
                    //mov ecx,0x40    sub ecx,dword ptr ss:[ebp+edx-0x150]
                    uint ecx = 0x40 - Stack[EBP + loop / 4 - 0x54]; 
                    uint edx = Stack[EBP - 0x6D];                   //mov edx,dword ptr ss:[ebp-0x1B4]
                    //mov eax,dword ptr ds:[esi+ebx*0x8]
                    //mov dword ptr ss:[ebp-0x1C4],eax
                    //mov eax,dword ptr ds:[esi+ebx*0x8+0x4]
                    //mov dword ptr ss:[ebp-0x1CC],eax
                    Stack[EBP - 0x71] = BitConverter.ToUInt32(hashTable.Hash, (int)(ebx * 8)); 
                    Stack[EBP - 0x73] = BitConverter.ToUInt32(hashTable.Hash, (int)(ebx * 8 + 0x04));

                    uint eax = ediLoop3;
                    CreateHashSubFunc1(ref eax, ref ecx, ref edx);

                    esi = eax;
                    ediLoop3 = edx;

                    eax = Stack[EBP - 0x6E];
                    edx = Stack[EBP - 0x6D];
                    ecx = Stack[EBP + loop / 4 - 0x54];
                    CreateHashSubFunc2(ref eax, ref ecx, ref edx);

                    temp = esi | eax;
                    Array.Copy(BitConverter.GetBytes(temp), 0, hashTable.Hash, ebx * 8, 4);


                    //mov eax,dword ptr ss:[ebp-0x1CC]
                    //mov dword ptr ss:[ebp-0x1B4],eax
                    Stack[EBP - 0x6D] = Stack[EBP - 0x73];

                    temp = ediLoop3 | edx;
                    Array.Copy(BitConverter.GetBytes(temp), 0, hashTable.Hash, ebx * 8 + 0x04, 4);
                    
                    //mov edi,dword ptr ss:[ebp-0x1C4]
                    //mov dword ptr ss:[ebp-0x1B8],edi 
                    ediLoop3 = Stack[EBP - 0x71];
                    Stack[EBP - 0x6E] = ediLoop3;
                }
                Stack[EBP - 0x6F] = 0x60;

                Pointer = 0;
                //第四轮
                //[ebp-0x1B4]
                for(int loop = 5; loop > 0; loop--)
                {
                    //mov ecx,0xA
                    //lea edi,dword ptr ss:[ebp-0x30]
                    //rep movsd 
                    byte[] movTemp = new byte[0xA*4];
                    Array.Copy(hashTable.Hash, Pointer, movTemp, 0, 0xA*4);
                    for(int rep = 0; rep < 0xA; rep++)
                    {
                        Stack[EBP - 0x0C + rep] = BitConverter.ToUInt32(movTemp, rep * 4);
                    }

                    uint ediLoop41 = 2;
                    //4-1轮
                    //[ebp-0x1B8]
                    for(int loop41 = 5; loop41 > 0; loop41--)
                    {
                        uint eax = ediLoop41 - 1;

                        //mov esi,dword ptr ss:[ebp+edx*0x8-0x30]
                        //mov ecx,dword ptr ss:[ebp+edx*0x8-0x2C]
                        //not ecx
                        //not esi
                        uint esi = ~Stack[EBP + (int)(eax % 5 * 2) - 0x0C];
                        uint ecx = ~Stack[EBP + (int)(eax % 5 * 2) - 0x0B];

                        eax = ediLoop41;
                        esi &= Stack[EBP + (int)(eax % Stack[EBP - 0x74] * 2) - 0x0C];
                        ecx &= Stack[EBP + (int)(eax % Stack[EBP - 0x74] * 2) - 0x0B];

                        uint temp;
                        temp = BitConverter.ToUInt32(hashTable.Hash, Pointer) ^ esi;
                        Array.Copy(BitConverter.GetBytes(temp), 0, hashTable.Hash, Pointer, 4);

                        temp = BitConverter.ToUInt32(hashTable.Hash, Pointer + 0x04) ^ ecx;
                        Array.Copy(BitConverter.GetBytes(temp), 0, hashTable.Hash, Pointer + 0x04, 4);

                        Pointer += 0x08;
                        ediLoop41++;

                    }
                }

                Pointer = 0;

                uint tempEnd;
                tempEnd = Stack[EBP + loopCount * 2 - 0x3C] ^ BitConverter.ToUInt32(hashTable.Hash, Pointer);
                Array.Copy(BitConverter.GetBytes(tempEnd), 0, hashTable.Hash, Pointer, 4);

                tempEnd = Stack[EBP + loopCount * 2 - 0x3B] ^ BitConverter.ToUInt32(hashTable.Hash, Pointer + 0x04);
                Array.Copy(BitConverter.GetBytes(tempEnd), 0, hashTable.Hash, Pointer + 0x04, 4);

                Pointer = 0x00000078;
            }

        }
        /// <summary>
        /// 生成Hash子函数1
        /// </summary>
        private static void CreateHashSubFunc1(ref uint eax,ref uint ecx,ref uint edx)
        {
            if ((ecx & 0x000000FF) >= 0x00000040)
            {
                eax = 0;
                edx = 0;
            }
            else if ((ecx & 0x000000FF) >= 0x00000020)
            {
                eax = edx;
                edx = 0;
                ecx &= 0xFFFFFF1F;   //and cl,0x1F
                int shiftCount = (int)(ecx & 0x000000FF);
                eax >>= shiftCount;   //shr eax,cl
            }
            else
            {
                int shiftCount = (int)(ecx & 0x000000FF);
                //shrd eax,edx,cl
                AssemblyEmulator.SHRD(ref eax, edx, (byte)(uint)shiftCount);

                edx >>= shiftCount;  //shr edx,cl
            }
        }
        /// <summary>
        /// 生成Hash子函数2
        /// </summary>
        private static void CreateHashSubFunc2(ref uint eax, ref uint ecx, ref uint edx)
        {
            if ((ecx & 0x000000FF) >= 0x00000040)
            {
                eax = 0;
                edx = 0;
            }
            else if ((ecx & 0x000000FF) >= 0x00000020)
            {
                edx = eax;
                eax = 0;
                ecx &= 0xFFFFFF1F;   //and cl,0x1F
                int shiftCount = (int)(ecx & 0x000000FF);
                edx <<= shiftCount;   //shl edx,cl
            }
            else
            {
                int shiftCount = (int)(ecx & 0x000000FF);
                //shld edx,eax,cl
                AssemblyEmulator.SHLD(ref edx, eax, (byte)(uint)shiftCount);

                eax <<= shiftCount;  //shl eax,cl
            }
        }

        
    }

    /// <summary>
    /// Hash表
    /// </summary>
    [StructLayout(LayoutKind.Sequential,Pack = 4)]
    public struct HashTable
    {
        /// <summary>
        /// Hash(192字节)  0x00
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray,SizeConst = 0xC8)]
        public byte[] Hash;
        /// <summary>
        /// 实际种子长度    0xC8
        /// </summary>
        public int ActualSeedLength;
        /// <summary>
        /// 最大种子长度    0xCC
        /// </summary>
        public int MaxSeedLength;
        /// <summary>
        /// 需要获取的长度  0xD0
        /// </summary>
        public int NeedLength;
    }
}
