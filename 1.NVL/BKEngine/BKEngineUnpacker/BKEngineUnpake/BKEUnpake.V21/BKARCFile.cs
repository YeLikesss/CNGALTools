using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.IO;
using BKEUnpake.V20;

namespace BKEUnpake.V21
{
    /// <summary>
    /// 文件处理
    /// </summary>
    public class BKARCFile
    {
        /// <summary>
        /// 封包合法性标志
        /// </summary>
        private bool isVaild = false;
        /// <summary>
        /// 文件表信息
        /// </summary>
        private FilePackageListInfo listInfo;
        /// <summary>
        /// 错误信息
        /// </summary>
        private string errorMessage = null;
        /// <summary>
        /// 输出路径
        /// </summary>
        private string outDirectory = null;
        /// <summary>
        /// 文件流
        /// </summary>
        private FileStream fileStream = null;
        /// <summary>
        /// 文件listkey
        /// </summary>
        private uint filekey = 0;
        /// <summary>
        /// 压缩资源文件表
        /// </summary>
        private List<BZip2CompressedResources> mCompressedResourceslist;
        /// <summary>
        /// 普通资源文件表
        /// </summary>
        private List<NormalResources> mNormalResourceslist;

        /// <summary>
        /// 获取封包合法性
        /// </summary>
        public bool IsVaild => this.isVaild;
        /// <summary>
        /// 获取错误信息
        /// </summary>
        public string ErrorMessage => this.errorMessage;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="filepath">文件路径</param>
        public BKARCFile(string filepath)
        {
            this.FileAnalysis(filepath);            //文件特征分析
        }
        /// <summary>
        /// 文件解析
        /// </summary>
        /// <param name="filepath">文件路径</param>
        private void FileAnalysis(string filepath)
        {
            FileInfo fileInfo = new FileInfo(filepath);
            if (File.Exists(filepath)==false)
            {   //文件不存在
                this.isVaild = false;
                this.errorMessage = "文件不存在！";
                return;
            }
            try
            {
                this.fileStream = new FileStream(filepath, FileMode.Open, FileAccess.Read);       //创建文件流
                byte[] header = new byte[6];                                 //文件头
                fileStream.Read(header, 0, header.Length);                  //读取文件头6个字节
                for(int index=0;index<header.Length;index++)
                {   //循环检查文件头
                    if (header[index] != FileSignature.Header[index])
                    {
                        this.isVaild = false;               //检查失败
                        this.errorMessage = "请选择正确的bkarc封包";
                        return;
                    }
                }
                this.isVaild = true;            //文件格式正确
                string noExtensionFilename = fileInfo.Name.Replace(fileInfo.Extension,"");     //获取无扩展文件名
                this.outDirectory = fileInfo.DirectoryName+"/Extract/"+noExtensionFilename+"/";     //设置导出路径
            }
            catch (Exception ex)
            {
                this.isVaild = false;
                this.errorMessage = "读取文件失败\n"+ex.Message;
                return;
            }
        }
        /// <summary>
        /// 解密文件
        /// </summary>
        /// <returns>True为解密成功 False为解密失败</returns>
        public bool DecryptFile()
        {
            if (this.fileStream == null)
            {   //文件为空则返回失败
                this.errorMessage = "文件对象不存在";
                return false;
            }
            if (this.isVaild == false)
            {   //文件非法
                this.errorMessage = "文件不是有效bkarc文件";
                return false;
            }

            try
            {
                byte[] listinfo = new byte[12];
                this.fileStream.Seek(this.fileStream.Length - listinfo.Length, SeekOrigin.Begin);       //设置文件流位置
                this.fileStream.Read(listinfo, 0, listinfo.Length);   //读取尾部12字节
                BKARCList.FileListInfoAnalysis(listinfo, out this.listInfo);        //解析列表信息结构

                byte[] listfiledata = new byte[this.listInfo.ListDataSize];
                //读取文件中的列表数据
                this.fileStream.Seek(this.fileStream.Length - listinfo.Length - listfiledata.Length, SeekOrigin.Begin);     //设置列表数据起始位置
                this.fileStream.Read(listfiledata, 0, listfiledata.Length);

                byte[] list = BKARCList.DecryptList(listfiledata, this.listInfo.ListDecryptKey, out this.filekey);     //解密列表


                if (list == null)
                {
                    this.errorMessage = "数据解密解压失败";
                    return false;
                }


                //解析文件表
                BKARCList.ListAnalysis(list, this.listInfo.ListCount, out this.mCompressedResourceslist, out this.mNormalResourceslist);                

                if (this.mCompressedResourceslist != null || this.mCompressedResourceslist.Count > 0)
                {
                    //压缩资源检查
                    //读取压缩资源文件
                    List<List<byte>> compresseddata = FileIOManager.ReadCompressedResources(this.fileStream, this.mCompressedResourceslist, out this.errorMessage);        
                    if (compresseddata == null)
                    {
                        return false;
                    }
                    FileFix.CompressedResourcesFix(compresseddata);             //修复文件头
                    List<List<byte>> unzipdata = BZip2Helper.DecompressData(compresseddata);        //解压文件
                    if (unzipdata == null || unzipdata.Count <= 0)
                    {
                        this.errorMessage = "文件解压失败或无压缩资源";
                    }
                    else
                    {
                        //检查格式并导出文件
                        if (this.CheckAndOutput(unzipdata,this.mCompressedResourceslist) == false)             
                        {
                            Debug.WriteLine("文件导出失败");
                            return false;
                        }
                    }

                    
                }

                if (this.mNormalResourceslist != null || this.mNormalResourceslist.Count > 0)
                {
                    //普通资源检查
                    //读取普通资源
                    List<List<byte>> normalres = FileIOManager.ReadNormalResources(this.fileStream, this.mNormalResourceslist, out this.errorMessage); 
                    for (int norresindex = 0; norresindex < normalres.Count; norresindex++)
                    {   
                        //循环解密文件
                        normalres[norresindex]= DecryptHelper.DecryptFile(normalres.ElementAt(norresindex).ToArray(), 
                                                                          (int)this.filekey, 
                                                                          this.mNormalResourceslist.ElementAt(norresindex).FileSize, 
                                                                         this.mNormalResourceslist.ElementAt(norresindex).FileOffset)
                                                                         .ToList();
                    }

                    //检查格式并导出文件
                    if (this.CheckAndOutput(normalres,this.mNormalResourceslist)==false)               
                    {
                        Debug.WriteLine("文件导出失败");
                        return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                this.errorMessage = "解密文件失败\n" + ex.Message;
                return false;
            }
        }
        /// <summary>
        /// 检查文件格式然后导出到硬盘(压缩资源版本)
        /// </summary>
        /// <param name="data">字节流数组</param>
        /// <param name="ziplist">压缩数据表</param>
        /// <returns>True为导出成功 Flase为导出失败</returns>
        private bool CheckAndOutput(List<List<byte>> data, List<BZip2CompressedResources> ziplist)
        {
            for (int dataindex = 0; dataindex < data.Count; dataindex++)
            {
                //设置文件名
                string filename = ziplist.ElementAt(dataindex).FileNameHash.ToString("x8").ToUpper();
                byte[] buffer = new byte[16];                               //定义16字节文件
                data.ElementAt(dataindex).CopyTo(0, buffer, 0, buffer.Length);      //读取文件头

                foreach (KeyValuePair<string, string> valuePair in FormatCheck.GetFileFormat)
                {   
                    //遍历文件格式
                    List<byte> magic = Encoding.UTF8.GetBytes(valuePair.Key).ToList();  //获取头标志特征码
                    if (FormatCheck.FileCheck(buffer, magic.ToArray()))
                    {
                        filename += valuePair.Value;          //加上文件扩展名
                        break;
                    }
                }

                //非资源文件
                if (filename.Split('.').Length <= 1)
                {
                    filename += ".bkpsr";
                }

                //写入数据到硬盘
                if (FileIOManager.WriteFile(data.ElementAt(dataindex).ToArray(), this.outDirectory + filename, out this.errorMessage) == false)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 检查文件格式然后导出到硬盘(普通资源版本)
        /// </summary>
        /// <param name="data">字节流数组</param>
        /// <param name="reslist">普通数据表</param>
        /// <returns>True为导出成功 Flase为导出失败</returns>
        private bool CheckAndOutput(List<List<byte>> data, List<NormalResources> reslist)
        {
            for (int dataindex = 0; dataindex < data.Count; dataindex++)
            {
                //设置文件名
                string filename = reslist.ElementAt(dataindex).FileNameHash.ToString("x8").ToUpper();
                byte[] buffer = new byte[16];                               //定义16字节文件
                data.ElementAt(dataindex).CopyTo(0, buffer, 0, buffer.Length);      //读取文件头

                foreach (KeyValuePair<string, string> valuePair in FormatCheck.GetFileFormat)
                {
                    //遍历文件格式
                    List<byte> magic = Encoding.UTF8.GetBytes(valuePair.Key).ToList();  //获取头标志特征码
                    if (FormatCheck.FileCheck(buffer, magic.ToArray()))
                    {
                        filename += valuePair.Value;          //加上文件扩展名
                        break;
                    }
                }

                //非资源文件
                if (filename.Split('.').Length <= 1)
                {
                    filename += ".bkpsr";
                }

                //写入数据到硬盘
                if (FileIOManager.WriteFile(data.ElementAt(dataindex).ToArray(), this.outDirectory + filename, out this.errorMessage) == false)
                {
                    return false;
                }
            }
            return true;
        }


        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            try 
            {
                this.fileStream?.Close();                //释放资源
                this.fileStream?.Dispose();
            }
            catch { }
            this.fileStream = null;
           
            this.errorMessage = null;
            this.mCompressedResourceslist = null;
            this.mNormalResourceslist = null;
            this.outDirectory = null;
        }
    }
}
