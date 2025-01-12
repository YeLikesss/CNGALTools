using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace AsicxArt
{
	/// <summary>
	/// SQLite3
	/// </summary>
    internal class SQLite3
    {
		/// <summary>
		/// Sqlite3函数执行结果
		/// </summary>
		public enum Result : uint
		{
			OK,
			Error,
			Internal,
			Perm,
			Abort,
			Busy,
			Locked,
			NoMem,
			ReadOnly,
			Interrupt,
			IOError,
			Corrupt,
			NotFound,
			Full,
			CannotOpen,
			LockErr,
			Empty,
			SchemaChngd,
			TooBig,
			Constraint,
			Mismatch,
			Misuse,
			NotImplementedLFS,
			AccessDenied,
			Format,
			Range,
			NonDBFile,
			Notice,
			Warning,
			Row = 100,
			Done
		}

		/// <summary>
		/// SQLite额外的错误码
		/// </summary>
		public enum ExtendedResult : uint
		{
			IOErrorRead = 266,
			IOErrorShortRead = 522,
			IOErrorWrite = 778,
			IOErrorFsync = 1034,
			IOErrorDirFSync = 1290,
			IOErrorTruncate = 1546,
			IOErrorFStat = 1802,
			IOErrorUnlock = 2058,
			IOErrorRdlock = 2314,
			IOErrorDelete = 2570,
			IOErrorBlocked = 2826,
			IOErrorNoMem = 3082,
			IOErrorAccess = 3338,
			IOErrorCheckReservedLock = 3594,
			IOErrorLock = 3850,
			IOErrorClose = 4106,
			IOErrorDirClose = 4362,
			IOErrorSHMOpen = 4618,
			IOErrorSHMSize = 4874,
			IOErrorSHMLock = 5130,
			IOErrorSHMMap = 5386,
			IOErrorSeek = 5642,
			IOErrorDeleteNoEnt = 5898,
			IOErrorMMap = 6154,
			LockedSharedcache = 262,
			BusyRecovery = 261,
			CannottOpenNoTempDir = 270,
			CannotOpenIsDir = 526,
			CannotOpenFullPath = 782,
			CorruptVTab = 267,
			ReadonlyRecovery = 264,
			ReadonlyCannotLock = 520,
			ReadonlyRollback = 776,
			AbortRollback = 516,
			ConstraintCheck = 275,
			ConstraintCommitHook = 531,
			ConstraintForeignKey = 787,
			ConstraintFunction = 1043,
			ConstraintNotNull = 1299,
			ConstraintPrimaryKey = 1555,
			ConstraintTrigger = 1811,
			ConstraintUnique = 2067,
			ConstraintVTab = 2323,
			NoticeRecoverWAL = 283,
			NoticeRecoverRollback = 539
		}

		/// <summary>
		/// 配置选项
		/// </summary>
		public enum ThreadOption
		{
			/// <summary>
			/// 单线程
			/// </summary>
			SingleThread = 1,
			/// <summary>
			/// 多线程
			/// </summary>
			MultiThread,
			/// <summary>
			/// 串行
			/// </summary>
			Serialized
		}

		/// <summary>
		/// 数据类型
		/// </summary>
		public enum ColType
		{
			/// <summary>
			/// 整数
			/// </summary>
			Integer = 1,
			/// <summary>
			/// 浮点
			/// </summary>
			Float,
			/// <summary>
			/// 文本
			/// </summary>
			Text,
			/// <summary>
			/// 二进制字节
			/// </summary>
			Blob,
			Null
		}

		/// <summary>
		/// 数据库打开选项
		/// </summary>
		[Flags]
		public enum SQLiteOpenFlags : uint
		{
			/// <summary>
			/// 只读
			/// </summary>
			ReadOnly = 0x00000001,
			/// <summary>
			/// 读写
			/// </summary>
			ReadWrite = 0x00000002,
			/// <summary>
			/// 创建
			/// </summary>
			Create = 0x00000004,
			/// <summary>
			/// 多线程模式
			/// </summary>
			NoMutex = 0x00008000,
			/// <summary>
			/// 串行模式
			/// </summary>
			FullMutex = 0x00010000,
			/// <summary>
			/// 共享缓存模式
			/// </summary>
			SharedCache = 0x00020000,
			/// <summary>
			/// 非共享缓存模式
			/// </summary>
			PrivateCache = 0x00040000,
			ProtectionComplete = 0x00100000,
			ProtectionCompleteUnlessOpen = 0x00200000,
			ProtectionCompleteUntilFirstUserAuthentication = 0x00300000,
			ProtectionNone = 0x00400000
		}

		/// <summary>
		/// 打开数据库 UTF-8版
		/// </summary>
		/// <param name="fileName">数据库路径</param>
		/// <param name="hDB">数据库对象</param>
		/// <returns></returns>
		[DllImport("sqlite3.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "sqlite3_open")]
		public static extern SQLite3.Result OpenA([MarshalAs(UnmanagedType.LPUTF8Str)]string fileName, out IntPtr hDB);

		/// <summary>
		/// 打开数据库 UTF-8版
		/// </summary>
		/// <param name="fileName">数据库路径</param>
		/// <param name="hDB">数据库句柄</param>
		/// <param name="flags">打开标志</param>
		/// <param name="vfsModuleName">虚拟文件模块名</param>
		/// <returns></returns>
		[DllImport("sqlite3.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "sqlite3_open_v2")]
		public static extern SQLite3.Result OpenA([MarshalAs(UnmanagedType.LPUTF8Str)] string fileName, out IntPtr hDB, SQLiteOpenFlags flags, [MarshalAs(UnmanagedType.LPUTF8Str)] string? vfsModuleName);

		/// <summary>
		/// 打开数据库 Unicode版
		/// </summary>
		/// <param name="fileName">数据库路径</param>
		/// <param name="hDB">数据库句柄</param>
		/// <returns></returns>
		[DllImport("sqlite3.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "sqlite3_open16", CharSet = CharSet.Unicode)]
		public static extern SQLite3.Result OpenW(string fileName, out IntPtr hDB);

		/// <summary>
		/// 设置数据库加载扩展
		/// </summary>
		/// <param name="hDB">数据库句柄</param>
		/// <param name="enable">启用状态</param>
		/// <returns></returns>
		[DllImport("sqlite3.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "sqlite3_enable_load_extension")]
		public static extern SQLite3.Result EnableLoadExtension(IntPtr hDB, bool enable);

		/// <summary>
		/// 关闭数据库
		/// </summary>
		/// <param name="hDB">数据库句柄</param>
		/// <returns></returns>
		[DllImport("sqlite3.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "sqlite3_close")]
		public static extern SQLite3.Result Close(IntPtr hDB);

		/// <summary>
		/// 初始化SQLite
		/// </summary>
		/// <returns></returns>
		[DllImport("sqlite3.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "sqlite3_initialize")]
		public static extern SQLite3.Result Initialize();

		/// <summary>
		/// 关闭SQLite
		/// </summary>
		/// <returns></returns>
		[DllImport("sqlite3.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "sqlite3_shutdown")]
		public static extern SQLite3.Result Shutdown();

		/// <summary>
		/// 设置SQLite线程模式
		/// </summary>
		/// <param name="option">线程模式</param>
		/// <returns></returns>
		[DllImport("sqlite3.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "sqlite3_config")]
		public static extern SQLite3.Result SetThreadMode(SQLite3.ThreadOption option);

		/// <summary>
		/// 设置数据库超时时间
		/// </summary>
		/// <param name="hDB">数据库句柄</param>
		/// <param name="milliseconds">超时时间(毫秒)</param>
		/// <returns></returns>
		[DllImport("sqlite3.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "sqlite3_busy_timeout")]
		public static extern SQLite3.Result SetBusyTimeout(IntPtr hDB, int milliseconds);

		/// <summary>
		/// 获得修改次数
		/// </summary>
		/// <param name="hDB">数据库句柄</param>
		/// <returns></returns>
		[DllImport("sqlite3.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "sqlite3_changes")]
		public static extern int GetChanges(IntPtr hDB);

		/// <summary>
		/// 查询准备
		/// </summary>
		/// <param name="hDB">数据库句柄</param>
		/// <param name="sql">sql语句</param>
		/// <param name="numBytes">sql语句字节数</param>
		/// <param name="statementPtr">二进制执行流指针</param>
		/// <param name="tailPtr">sql未使用的语句</param>
		/// <returns></returns>
		[DllImport("sqlite3.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "sqlite3_prepare_v2")]
		private static extern SQLite3.Result Prepare2A(IntPtr hDB, [MarshalAs(UnmanagedType.LPUTF8Str)] string sql, int numBytes, out IntPtr statementPtr, IntPtr tailPtr);

		/// <summary>
		/// 查询准备
		/// </summary>
		/// <param name="hDB">数据库句柄</param>
		/// <param name="query">sql语句</param>
		/// <returns>二进制执行流指针</returns>
		public static IntPtr Prepare2(IntPtr hDB, string query)
		{
			IntPtr statementPtr;
			SQLite3.Result result = SQLite3.Prepare2A(hDB, query, Encoding.UTF8.GetByteCount(query), out statementPtr, IntPtr.Zero);
			if (result != SQLite3.Result.OK)
			{
				throw SQLiteException.New(result, SQLite3.GetErrorMessage(hDB));
			}
			return statementPtr;
		}

		/// <summary>
		/// 执行准备好二进制sql语句
		/// </summary>
		/// <param name="statementPtr">二进制执行流指针</param>
		/// <returns></returns>
		[DllImport("sqlite3.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "sqlite3_step")]
		public static extern SQLite3.Result Step(IntPtr statementPtr);

		/// <summary>
		/// 重置二进制sql语句到初始状态
		/// </summary>
		/// <param name="statementPtr">二进制执行流指针</param>
		/// <returns></returns>
		[DllImport("sqlite3.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "sqlite3_reset")]
		public static extern SQLite3.Result Reset(IntPtr statementPtr);

		/// <summary>
		/// 释放二进制sql执行语句
		/// </summary>
		/// <param name="statementPtr">二进制执行流指针</param>
		/// <returns></returns>
		[DllImport("sqlite3.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "sqlite3_finalize")]
		public static extern SQLite3.Result Finalize(IntPtr statementPtr);

		/// <summary>
		/// 获取最后一条插入操作的RowID
		/// </summary>
		/// <param name="hDB">数据库句柄</param>
		/// <returns></returns>
		[DllImport("sqlite3.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "sqlite3_last_insert_rowid")]
		public static extern long GetLastInsertRowid(IntPtr hDB);

		/// <summary>
		/// 获取错误信息
		/// </summary>
		/// <param name="hDB">数据库句柄</param>
		/// <returns></returns>
		[DllImport("sqlite3.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "sqlite3_errmsg16")]
		private static extern IntPtr GetErrorMessageW(IntPtr hDB);

		/// <summary>
		/// 获取错误信息
		/// </summary>
		/// <param name="hDB">数据库句柄</param>
		/// <returns></returns>
		public static string GetErrorMessage(IntPtr hDB)
		{
			return SQLite3.GetErrorMessageW(hDB).AsUnicodeString();
		}

		/// <summary>
		/// 获取参数索引
		/// </summary>
		/// <param name="statementPtr">二进制执行语句指针</param>
		/// <param name="name">名称</param>
		/// <returns></returns>
		[DllImport("sqlite3.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "sqlite3_bind_parameter_index")]
		public static extern int GetBindParameterIndex(IntPtr statementPtr, [MarshalAs(UnmanagedType.LPStr)] string name);

		/// <summary>
		/// 绑定空值
		/// </summary>
		/// <param name="statementPtr">二进制执行语句指针</param>
		/// <param name="index">索引</param>
		/// <returns></returns>
		[DllImport("sqlite3.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "sqlite3_bind_null")]
		public static extern int BindNull(IntPtr statementPtr, int index);

		/// <summary>
		/// 绑定32位整数类型
		/// </summary>
		/// <param name="statementPtr">二进制执行语句指针</param>
		/// <param name="index">索引</param>
		/// <param name="value">待绑定的值</param>
		/// <returns></returns>
		[DllImport("sqlite3.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "sqlite3_bind_int")]
		public static extern int BindInt(IntPtr statementPtr, int index, int value);

		/// <summary>
		/// 绑定64位整数类型
		/// </summary>
		/// <param name="statementPtr">二进制执行语句指针</param>
		/// <param name="index">索引</param>
		/// <param name="value">待绑定的值</param>
		/// <returns></returns>
		[DllImport("sqlite3.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "sqlite3_bind_int64")]
		public static extern int BindInt64(IntPtr statementPtr, int index, long value);

		/// <summary>
		/// 绑定64位浮点(双精度)类型
		/// </summary>
		/// <param name="statementPtr">二进制执行语句指针</param>
		/// <param name="index">索引</param>
		/// <param name="value">待绑定的值</param>
		/// <returns></returns>
		[DllImport("sqlite3.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "sqlite3_bind_double")]
		public static extern int BindDouble(IntPtr statementPtr, int index, double value);

		/// <summary>
		/// 绑定字符串类型(Unicode)
		/// </summary>
		/// <param name="statementPtr">二进制执行语句指针</param>
		/// <param name="index">索引</param>
		/// <param name="value">待绑定的值</param>
		/// <param name="bufferLength">字符串内存长度</param>
		/// <param name="freeFuncPtr">释放字符串的函数指针</param>
		/// <returns></returns>
		[DllImport("sqlite3.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode, EntryPoint = "sqlite3_bind_text16")]
		public static extern int BindTextW(IntPtr statementPtr, int index, string value, int bufferLength, IntPtr freeFuncPtr);

		/// <summary>
		/// 绑定二进制流类型
		/// </summary>
		/// <param name="statementPtr">二进制执行语句指针</param>
		/// <param name="index">索引</param>
		/// <param name="value">待绑定的值</param>
		/// <param name="bufferLength">二进制流内存长度</param>
		/// <param name="freeFuncPtr">释放内存的函数指针</param>
		/// <returns></returns>
		[DllImport("sqlite3.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "sqlite3_bind_blob")]
		public static extern int BindBlob(IntPtr statementPtr, int index, byte[] value, int bufferLength, IntPtr freeFuncPtr);

		/// <summary>
		/// 获取列的数量
		/// </summary>
		/// <param name="statementPtr">二进制执行语句指针</param>
		/// <returns></returns>
		[DllImport("sqlite3.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "sqlite3_column_count")]
		public static extern int GetColumnCount(IntPtr statementPtr);

		/// <summary>
		/// 获取指定列的名称指针 (UTF-8)
		/// </summary>
		/// <param name="statementPtr">二进制执行语句指针</param>
		/// <param name="index">列序号</param>
		/// <returns></returns>
		[DllImport("sqlite3.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "sqlite3_column_name")]
		private static extern IntPtr GetColumnNameA(IntPtr statementPtr, int index);

		/// <summary>
		/// 获取指定列的名称指针 (Unicode)
		/// </summary>
		/// <param name="statementPtr">二进制执行语句指针</param>
		/// <param name="index">列序号</param>
		/// <returns></returns>
		[DllImport("sqlite3.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "sqlite3_column_name16")]
		private static extern IntPtr GetColumnNameW(IntPtr statementPtr, int index);

		/// <summary>
		/// 获取指定列的名称
		/// </summary>
		/// <param name="statementPtr">二进制执行语句指针</param>
		/// <param name="index">列序号</param>
		/// <returns></returns>
		public static string GetColumnName(IntPtr statementPtr, int index)
		{
			return SQLite3.GetColumnNameW(statementPtr, index).AsUnicodeString();
		}

		/// <summary>
		/// 获取指定列的数据类型
		/// </summary>
		/// <param name="statementPtr">二进制执行语句指针</param>
		/// <param name="index">列序号</param>
		/// <returns></returns>
		[DllImport("sqlite3.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "sqlite3_column_type")]
		public static extern SQLite3.ColType GetColumnType(IntPtr statementPtr, int index);

		/// <summary>
		/// 获取指定列的32位整数型字段
		/// </summary>
		/// <param name="statementPtr">二进制执行语句指针</param>
		/// <param name="index">列序号</param>
		/// <returns></returns>
		[DllImport("sqlite3.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "sqlite3_column_int")]
		public static extern int GetColumnInt(IntPtr statementPtr, int index);

		/// <summary>
		/// 获取指定列的64位整数型字段
		/// </summary>
		/// <param name="statementPtr">二进制执行语句指针</param>
		/// <param name="index">列序号</param>
		/// <returns></returns>
		[DllImport("sqlite3.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "sqlite3_column_int64")]
		public static extern long GetColumnInt64(IntPtr statementPtr, int index);

		/// <summary>
		/// 获取指定列的64位双精度浮点型字段
		/// </summary>
		/// <param name="statementPtr">二进制执行语句指针</param>
		/// <param name="index">列序号</param>
		/// <returns></returns>
		[DllImport("sqlite3.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "sqlite3_column_double")]
		public static extern double GetColumnDouble(IntPtr statementPtr, int index);

		/// <summary>
		/// 获取指定列的文本字段指针 (UTF-8)
		/// </summary>
		/// <param name="statementPtr">二进制执行语句指针</param>
		/// <param name="index">列序号</param>
		/// <returns></returns>
		[DllImport("sqlite3.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "sqlite3_column_text")]
		private static extern IntPtr GetColumnTextA(IntPtr statementPtr, int index);

		/// <summary>
		/// 获取指定列的文本字段指针 (Unicode)
		/// </summary>
		/// <param name="statementPtr">二进制执行语句指针</param>
		/// <param name="index">列序号</param>
		/// <returns></returns>
		[DllImport("sqlite3.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "sqlite3_column_text16")]
		private static extern IntPtr GetColumnTextW(IntPtr statementPtr, int index);

		/// <summary>
		/// 获取指定列的文本数据
		/// </summary>
		/// <param name="statementPtr">二进制执行语句指针</param>
		/// <param name="index">列序号</param>
		public static string GetColumnString(IntPtr statementPtr, int index)
		{
			return SQLite3.GetColumnTextW(statementPtr, index).AsUnicodeString();
		}

		/// <summary>
		/// 获取指定列的二进制数组指针
		/// </summary>
		/// <param name="statementPtr">二进制执行语句指针</param>
		/// <param name="index">列序号</param>
		/// <returns></returns>
		[DllImport("sqlite3.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "sqlite3_column_blob")]
		private static extern IntPtr GetColumnBlob(IntPtr statementPtr, int index);

		/// <summary>
		/// 获取指定列的数据流长度 (文本字节/二进制字节数组)
		/// </summary>
		/// <param name="statementPtr">二进制执行语句指针</param>
		/// <param name="index">列序号</param>
		/// <returns></returns>
		[DllImport("sqlite3.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "sqlite3_column_bytes")]
		private static extern int GetColumnBytes(IntPtr statementPtr, int index);

		/// <summary>
		/// 获取指定列的二进制数组
		/// </summary>
		/// <param name="statementPtr">二进制执行语句指针</param>
		/// <param name="index">列序号</param>
		/// <returns></returns>
		public static byte[] GetColumnByteArray(IntPtr statementPtr, int index)
		{
			int length = SQLite3.GetColumnBytes(statementPtr, index);		//获取长度
			byte[] buffer = new byte[length];
			if (length > 0)
			{
				Marshal.Copy(SQLite3.GetColumnBlob(statementPtr, index), buffer, 0, length);  //复制到非托管内存
			}
			return buffer;
		}

		/// <summary>
		/// 获取额外的错误码
		/// </summary>
		/// <param name="hDB">数据库句柄</param>
		/// <returns></returns>
		[DllImport("sqlite3.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "sqlite3_extended_errcode")]
		public static extern SQLite3.ExtendedResult GetExtendedErrorCode(IntPtr hDB);

		/// <summary>
		/// 获取SQLite版本号
		/// </summary>
		/// <returns></returns>
		[DllImport("sqlite3.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "sqlite3_libversion_number")]
		public static extern int GetLibVersionNumber();

		/// <summary>
		/// 数据库认证密钥
		/// </summary>
		/// <param name="hDB">数据库句柄</param>
		/// <param name="key"></param>
		/// <param name="length">key的长度</param>
		/// <returns></returns>
		[DllImport("sqlite3.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "sqlite3_key")]
		public static extern int SetAuthenticationKey(IntPtr hDB, byte[] key, int length);

		/// <summary>
		/// 重置数据库认证密钥
		/// </summary>
		/// <param name="hDB">数据库句柄</param>
		/// <param name="key"></param>
		/// <param name="length">key的长度</param>
		/// <returns></returns>
		[DllImport("sqlite3.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "sqlite3_rekey")]
		public static extern int ReAuthenticationKey(IntPtr hDB, byte[] key, int length);

	}

	/// <summary>
	/// SQLite异常
	/// </summary>
	internal class SQLiteException : Exception
	{
		protected SQLiteException(SQLite3.Result result, string message) : base(message)
		{
			this.Result = result;
		}

		/// <summary>
		/// 获取SQLite错误码
		/// </summary>
		public SQLite3.Result Result { get; private set; }
		/// <summary>
		/// 实例化SQLite异常对象(静态版本)
		/// </summary>
		/// <param name="result">错误码</param>
		/// <param name="message">错误信息</param>
		/// <returns></returns>
		public static SQLiteException New(SQLite3.Result result, string message)
		{
			return new SQLiteException(result, message);
		}
	}
}
