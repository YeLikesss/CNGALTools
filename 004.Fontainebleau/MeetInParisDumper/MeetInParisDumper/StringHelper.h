﻿
#include <Windows.h>
#include "BaseType.h"
namespace Strings 
{
	/// <summary>
	/// 获取字符串长度 (请保证字符串\0结束)
	/// </summary>
	/// <param name="str">待测字符串</param>
	/// <returns>字符串长度</returns>
	SInteger StringLengthW(const WCHAR* str);

	/// <summary>
	/// 连接字符串 (请务必保证目标内存可以容纳待连接字符串)
	/// 目标字符串与待连接字符串必须为\0结束
	/// </summary>
	/// <param name="destinationStr">目标字符串</param>
	/// <param name="sourceStr">待连接字符串</param>
	void StringConcatW(WCHAR* destinationStr, const WCHAR* sourceStr);

	/// <summary>
	/// 复制字符串 (请务必保证目标内存可以容纳待复制字符串)
	/// 待复制字符串结尾必须为\0结束
	/// </summary>
	/// <param name="destinationStr">目标字符串</param>
	/// <param name="sourceStr">待复制字符串</param>
	/// <param name="isFixEnd">是否\0填充</param>
	void StringCopyW(WCHAR* destinationStr, const WCHAR* sourceStr);

	/// <summary>
	/// 复制字符串 (请务必保证目标内存可以容纳待复制字符串)
	/// 待复制字符串结尾必须为\0结束
	/// </summary>
	/// <param name="destinationStr">目标字符串</param>
	/// <param name="sourceStr">待字符串</param>
	/// <param name="copyLength">需复制字符长度(不包括\0)</param>
	/// <param name="isFixEnd">是否\0填充</param>
	void StringCopyW(WCHAR* destinationStr, const WCHAR* sourceStr, SInteger copyLength, BOOL isFixEnd);

	/// <summary>
	/// 比较字符串
	/// (两字符串必须\0结束)
	/// </summary>
	/// <param name="str1">待比较字符串1</param>
	/// <param name="str2">待比较字符串2</param>
	/// <param name="ignoreCase">TRUE为忽略大小写 FALSE不忽略 </param>
	/// <returns>TRUE为匹配 FALSE为不匹配</returns>
	BOOL StringCompareW(const WCHAR* str1, const WCHAR* str2, BOOL ignoreCase);

	/// <summary>
	/// 比较字符串  检查大小写
	/// (两字符串必须\0结束)
	/// </summary>
	/// <param name="str1">待比较字符串1</param>
	/// <param name="str2">待比较字符串2</param>
	/// <returns>TRUE为匹配 FALSE为不匹配</returns>
	BOOL StringCompareW(const WCHAR* str1, const WCHAR* str2);

	/// <summary>
	/// 比较字符串
	/// (可无需\0结束)
	/// </summary>
	/// <param name="str1">待比较字符串1</param>
	/// <param name="str2">待比较字符串2</param>
	/// <param name="compareLength">比较字符长度(此长度不包括\0)</param>
	/// <returns>TRUE为匹配 FALSE为不匹配</returns>
	BOOL StringCompareW(const WCHAR* str1, const WCHAR* str2, SInteger compareLength);

	/// <summary>
	/// 比较字符串  检查大小写
	/// (可无需\0结束)
	/// </summary>
	/// <param name="str1">待比较字符串1</param>
	/// <param name="str2">待比较字符串2</param>
	/// <param name="compareLength">比较字符长度(此长度不包括\0)</param>
	/// <param name="ignoreCase">TRUE为忽略大小写 FALSE不忽略 </param>
	/// <returns>TRUE为匹配 FALSE为不匹配</returns>
	BOOL StringCompareW(const WCHAR* str1, const WCHAR* str2, SInteger compareLength, BOOL ignoreCase);

	/// <summary>
	/// 小写转大写
	/// (字符串必须\0结束)
	/// </summary>
	/// <param name="destinationStr">待转字符串</param>
	void StringToUpperW(WCHAR* destinationStr);

	/// <summary>
	/// 小写转大写
	/// (可无需\0结束)
	/// </summary>
	/// <param name="destinationStr">待转字符串</param>
	/// <param name="strLength">字符串长度(此长度不包括\0)</param>
	void StringToUpperW(WCHAR* destinationStr, SInteger strLength);

	/// <summary>
	/// 大写转小写
	/// (字符串必须\0结束)
	/// </summary>
	/// <param name="destinationStr">待转字符串</param>
	void StringToLowerW(WCHAR* destinationStr);

	/// <summary>
	/// 大写转小写
	/// (可无需\0结束)
	/// </summary>
	/// <param name="destinationStr">待转字符串</param>
	/// <param name="strLength">字符串长度(此长度不包括\0)</param>
	void StringToLowerW(WCHAR* destinationStr, SInteger strLength);
}