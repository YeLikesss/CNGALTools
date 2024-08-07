﻿#include "StringHelper.h"


namespace Strings 
{
	SInteger StringLengthW(const WCHAR* str)
	{	
		//设置当前字符指针
		const WCHAR* nowWChar = str;

		//遍历到\0结束
		while (*nowWChar != 0)
		{
			nowWChar += 1;		//向后移动一个WChar 实际Pointer += 2
		}

		return nowWChar - str;
	}

	void StringConcatW(WCHAR* destinationStr, const WCHAR* sourceStr) 
	{
		SInteger length = StringLengthW(destinationStr);		//获得字符串长度
		StringCopyW(destinationStr + length, sourceStr);    //从目标字符串\0处开始复制
	}

	void StringCopyW(WCHAR* destinationStr, const WCHAR* sourceStr)
	{
		//设置当前字符指针
		WCHAR* nowDestWChar = destinationStr;
		const WCHAR* nowSourceWChar = sourceStr;

		//遍历原字符串到\0结束
		while (*nowSourceWChar != 0) 
		{
			*nowDestWChar = *nowSourceWChar;	//复制到目标字符串

			//两个向后移动一个WChar 实际Pointer += 2
			nowDestWChar += 1;
			nowSourceWChar += 1;
		}
		*nowDestWChar = 0;		//目标字符串补\0
	}

	void StringCopyW(WCHAR* destinationStr, const WCHAR* sourceStr, SInteger copyLength, BOOL isFixEnd)
	{
		SInteger mCopyCount = copyLength;

		//复制长度为0 不进行复制
		if (mCopyCount == 0)
		{
			return;
		}

		//设置当前字符指针
		WCHAR* nowDestWChar = destinationStr;
		const WCHAR* nowSourceWChar = sourceStr;

		//循环复制次数   原字符串\0结束检查
		while (mCopyCount != 0 && *nowSourceWChar != 0)
		{
			//复制到目标字符串
			*nowDestWChar = *nowSourceWChar;	

			//向后移动一个WChar 实际Pointer += 2
			nowDestWChar += 1;
			nowSourceWChar += 1;

			mCopyCount -= 1;	//复制次数-1
		}

		//是否\0填充
		if (isFixEnd) 
		{
			*nowDestWChar = 0;		//目标字符串补\0
		}
	}

	BOOL StringCompareW(const WCHAR* str1, const WCHAR* str2, BOOL ignoreCase) 
	{
		if (ignoreCase) 
		{
			//忽略大小写
			//获取字符串长度
			SInteger strLength = StringLengthW(str1);

			//判断长度是否相等
			if (strLength == StringLengthW(str2))
			{
				//申请新内存 字符串长度+1(补零\0)
				WCHAR* tempStr1 = (WCHAR*)HeapAlloc(GetProcessHeap(), 0, (strLength + 1) << 1);
				WCHAR* tempStr2 = (WCHAR*)HeapAlloc(GetProcessHeap(), 0, (strLength + 1) << 1);

				//复制到新的缓冲区
				StringCopyW(tempStr1, str1);
				StringCopyW(tempStr2, str2);

				//转小写
				StringToLowerW(tempStr1);
				StringToLowerW(tempStr2);

				//比较
				BOOL result = StringCompareW(tempStr1, tempStr2);

				//释放内存
				HeapFree(GetProcessHeap(), 0, tempStr1);
				HeapFree(GetProcessHeap(), 0, tempStr2);

				return result;
			}
			else
			{
				return FALSE;	//长度不一致则字符串不相等
			}
		}
		else
		{
			//不忽略大小写
			return StringCompareW(str1, str2);
		}
	}

	BOOL StringCompareW(const WCHAR* str1, const WCHAR* str2, SInteger compareLength, BOOL ignoreCase) 
	{
		if (ignoreCase)
		{
			//忽略大小写
			//申请新内存 字符串长度+1(补零\0)
			WCHAR* tempStr1 = (WCHAR*)HeapAlloc(GetProcessHeap(), 0, (compareLength + 1) << 1);
			WCHAR* tempStr2 = (WCHAR*)HeapAlloc(GetProcessHeap(), 0, (compareLength + 1) << 1);

			//复制到新的缓冲区
			StringCopyW(tempStr1, str1, compareLength, TRUE);
			StringCopyW(tempStr2, str2, compareLength, TRUE);

			//转小写
			StringToLowerW(tempStr1);
			StringToLowerW(tempStr2);

			//比较
			BOOL result = StringCompareW(tempStr1, tempStr2, compareLength);

			//释放内存
			HeapFree(GetProcessHeap(), 0, tempStr1);
			HeapFree(GetProcessHeap(), 0, tempStr2);

			return result;
		}
		else
		{
			//不忽略大小写
			return StringCompareW(str1, str2, compareLength);
		}
	}

	BOOL StringCompareW(const WCHAR* str1, const WCHAR* str2)
	{
		//获取字符串长度
		SInteger strLength = StringLengthW(str1);

		//字符串长度相等则进行扫描匹配
		if (strLength == StringLengthW(str2))
		{
			for (SInteger index = 0; index < strLength; index++) 
			{
				if (str1[index] != str2[index])
				{
					return FALSE;		//字符串内容不相等
				}
			}
			return TRUE;	//扫描完毕 字符串内容一致
		}
		else
		{
			return FALSE;		//长度不一致则字符串不相等
		}
	}

	BOOL StringCompareW(const WCHAR* str1, const WCHAR* str2, SInteger compareLength) 
	{
		//限定长度
		for (SInteger index = 0; index < compareLength; index++)
		{
			//检查\0结束位 字符是否相等
			if (str1[index] == 0 || str2[index] == 0 || str1[index] != str2[index])
			{
				return FALSE;
			}
		}
		return TRUE;
	}


	void StringToUpperW(WCHAR* destinationStr) 
	{
		//设置当前字符位置
		WCHAR* nowDestWChar = destinationStr;

		//判断\0结束
		while (*nowDestWChar != 0)
		{
			//0x0061-0x007A  小写a-z
			if (*nowDestWChar >= 0x0061 && *nowDestWChar <= 0x007A)
			{
				*((BYTE*)nowDestWChar) &= 0xDF;  //and 0xDF  sub 0x20 小写转大写
			}
			//向后移动一个WChar 实际Pointer += 2
			nowDestWChar += 1;
		}
	}

	void StringToUpperW(WCHAR* destinationStr, SInteger strLength) 
	{
		//限定长度
		for (SInteger index = 0; index < strLength; index++) 
		{
			//检查\0结束位
			if (destinationStr[index] == 0) 
			{
				break;
			}
			//0x0061-0x007A  小写a-z
			if (destinationStr[index] >= 0x0061 && destinationStr[index] <= 0x007A)
			{
				*(BYTE*)(destinationStr + index) &= 0xDF;  //and 0xDF  sub 0x20 小写转大写
			}
		}
	}

	void StringToLowerW(WCHAR* destinationStr) 
	{
		//设置当前字符位置
		WCHAR* nowDestWChar = destinationStr;

		//判断\0结束
		while (*nowDestWChar != 0)
		{
			//0x0041-0x005A  大写A-Z
			if (*nowDestWChar >= 0x0041 && *nowDestWChar <= 0x005A)
			{
				*((BYTE*)nowDestWChar) |= 0x20;  //or 0x20  add 0x20 大写转小写
			}
			//向后移动一个WChar 实际Pointer += 2
			nowDestWChar += 1;
		}
	}

	void StringToLowerW(WCHAR* destinationStr, SInteger strLength) 
	{
		//限定长度
		for (SInteger index = 0; index < strLength; index++)
		{
			//检查\0结束位
			if (destinationStr[index] == 0)
			{
				break;
			}
			//0x0041-0x005A  大写A-Z
			if (destinationStr[index] >= 0x0041 && destinationStr[index] <= 0x005A)
			{
				*(BYTE*)(destinationStr + index) |= 0x20;  //or 0x20  add 0x20 大写转小写
			}
		}
	}


}
