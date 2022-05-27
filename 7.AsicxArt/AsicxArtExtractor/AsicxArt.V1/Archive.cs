using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace AsicxArt.V1
{
    public class Archive
    {
        /// <summary>
        /// 提取图像资源
        /// </summary>
        /// <param name="dirPath">资源文件夹路径</param>
        /// <param name="key">数据库key</param>
        public static void ExtractImage(string dirPath, byte[] key)
        {
            //资源数据库
            List<string> dbNameList = new List<string>()
            {
                "\\rsinfo.db"
            };

            //资源表
            List<string> tableNameList = new List<string>()
            {
                "RsBGInfo",
                "RsCGInfo"
            };

            //遍历数据库
            foreach(string dbName in dbNameList)
            {
                string dbPath = dirPath + dbName;
                //检查文件存在
                if (File.Exists(dbPath))
                {
                    //打开数据库
                    IntPtr hDB = SQLite3Command.OpenDBWithKey(dbPath, SQLite3.SQLiteOpenFlags.ReadOnly, key);
                    //遍历表
                    foreach(string tableName in tableNameList)
                    {
                        int rowCount = SQLite3Command.GetTableItemCount(hDB, tableName);  //获得表项

                        if (rowCount == -1)
                        {
                            continue;
                        }

                        //导出目录
                        string outDirPath = dirPath + "\\Extract\\" + Path.GetFileNameWithoutExtension(dbPath) + "\\";
                        //检查文件夹是否存在
                        if (Directory.Exists(outDirPath) == false)
                        {
                            Directory.CreateDirectory(outDirPath);
                        }

                        //遍历id
                        for(int id = 0; id < rowCount; id++)
                        {
                            //准备sql执行语句
                            string sql = string.Format("select * from {0} where id={1}", tableName, id.ToString());
                            IntPtr statementPtr;
                            statementPtr = SQLite3.Prepare2(hDB, sql);
                            //执行
                            SQLite3.Step(statementPtr);

                            string fileName = SQLite3.GetColumnString(statementPtr, 1);  //name=1
                            byte[] buffer = SQLite3.GetColumnByteArray(statementPtr, 4);   //value=4

                            //释放
                            SQLite3.Finalize(statementPtr);
                            //回写
                            File.WriteAllBytes(outDirPath + fileName + ".png", buffer);
                        }

                    } 
                }
            }

        }

        /// <summary>
        /// 提取L2D资源
        /// </summary>
        /// <param name="dirPath">资源文件夹路径</param>
        /// <param name="key">数据库key</param>
        public static void ExtractL2DImage(string dirPath, byte[] key)
        {
            //资源数据库
            List<string> dbNameList = new List<string>()
            {
                "\\rsinfo2.db"
            };

            //资源表
            List<string> tableNameList = new List<string>()
            {
                "RsLive2DInfo"
            };

            //遍历数据库
            foreach (string dbName in dbNameList)
            {
                string dbPath = dirPath + dbName;
                //检查文件存在
                if (File.Exists(dbPath))
                {
                    //打开数据库
                    IntPtr hDB = SQLite3Command.OpenDBWithKey(dbPath, SQLite3.SQLiteOpenFlags.ReadOnly, key);
                    //遍历表
                    foreach (string tableName in tableNameList)
                    {
                        int rowCount = SQLite3Command.GetTableItemCount(hDB, tableName);  //获得表项

                        //找不到表
                        if (rowCount == -1)
                        {
                            continue;
                        }

                        //导出目录
                        string outDirPath = dirPath + "\\Extract\\" + Path.GetFileNameWithoutExtension(dbPath) + "\\";

                        //遍历id
                        for (int id = 0; id < rowCount; id++)
                        {
                            //准备sql执行语句
                            string sql = string.Format("select * from {0} where id={1}", tableName, id.ToString());
                            IntPtr statementPtr;
                            statementPtr = SQLite3.Prepare2(hDB, sql);
                            //执行
                            SQLite3.Step(statementPtr);

                            string pathName = SQLite3.GetColumnString(statementPtr, 1);  //path=1
                            string textureName = SQLite3.GetColumnString(statementPtr, 2); //texture=2
                            byte[] buffer = SQLite3.GetColumnByteArray(statementPtr, 5);   //value=4

                            //释放
                            SQLite3.Finalize(statementPtr);

                            //导出路径
                            string outPath = outDirPath + pathName + textureName + ".png";
                            if (Directory.Exists(Path.GetDirectoryName(outPath)) == false)
                            {
                                Directory.CreateDirectory(Path.GetDirectoryName(outPath));
                            }

                            //回写
                            File.WriteAllBytes(outPath, buffer);
                        }

                    }
                }
            }
        }
    }
}
