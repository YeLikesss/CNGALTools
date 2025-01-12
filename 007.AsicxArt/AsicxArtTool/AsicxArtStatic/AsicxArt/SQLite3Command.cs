using System;
using System.Collections.Generic;
using System.Text;

namespace AsicxArt
{
    internal class SQLite3Command
    {
        /// <summary>
        /// 获取表中项数
        /// </summary>
        /// <param name="hDB">数据库句柄</param>
        /// <param name="tableName">表名称</param>
        /// <returns>项数</returns>
        public static int GetTableItemCount(IntPtr hDB, string tableName)
        {
            string sql = string.Format("select count(*) from {0}", tableName);

            IntPtr statementPtr = SQLite3.Prepare2(hDB, sql);       //准备
            try
            {
                //执行数据库指令
                while (SQLite3.Step(statementPtr) == SQLite3.Result.Row)
                {
                    return SQLite3.GetColumnInt(statementPtr, 0);       //获取行数
                }
                return -1;
            }
            catch
            {
                return -1;
            }
            finally
            {
                SQLite3.Finalize(statementPtr);     //释放
            }
        }

        /// <summary>
        /// 打开加密数据库
        /// </summary>
        /// <param name="dbPath">数据库路径</param>
        /// <param name="flags">打开选项</param>
        /// <param name="key"></param>
        /// <returns>数据库句柄</returns>
        public static IntPtr OpenDBWithKey(string dbPath, SQLite3.SQLiteOpenFlags flags, byte[] key)
        {
            IntPtr hDB = new(-1);
            //打开数据库
            if(SQLite3.OpenA(dbPath, out hDB, SQLite3.SQLiteOpenFlags.ReadWrite, null) == SQLite3.Result.OK)
            {
                SQLite3.SetAuthenticationKey(hDB, key, key.Length);  //设置key
            }
            return hDB;
        }
    }
}
