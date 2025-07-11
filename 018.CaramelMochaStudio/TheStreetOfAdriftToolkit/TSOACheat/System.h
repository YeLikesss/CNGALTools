#pragma once

#include "Il2Cpp.h"
namespace System
{
	/// <summary>
	/// C#对象类
	/// </summary>
	class Object
	{
	public:
		Object() = delete;
		Object(const Object&) = delete;
		Object(Object&&) = delete;
		Object& operator=(const Object&) = delete;
		Object& operator=(Object&&) = delete;
		~Object() = delete;

	private:
		static const char* NameSpace;
		static const char* ClassName;

	public:
		/// <summary>
		/// 获取类型
		/// <para>[C#]object.GetType()</para>
		/// </summary>
		static Il2CppObject* GetType(Il2CppObject* obj);
	};

	/// <summary>
	/// 字符串
	/// </summary>
	class String
	{
	public:
		String() = delete;
		String(const String&) = delete;
		String(String&&) = delete;
		String& operator=(const String&) = delete;
		String& operator=(String&&) = delete;
		~String() = delete;

	private:
		static const char* NameSpace;
		static const char* ClassName;

	public:
		/// <summary>
		/// 字符串是否为空
		/// </summary>
		/// <param name="s">字符串对象</param>
		static bool IsEmpty(const Il2CppString* s);
		/// <summary>
		/// 比较字符串是否相等
		/// </summary>
		/// <param name="s1">字符串1</param>
		/// <param name="s2">字符串2</param>
		static bool Equals(const Il2CppString* s1, const Il2CppString* s2);
	};
}