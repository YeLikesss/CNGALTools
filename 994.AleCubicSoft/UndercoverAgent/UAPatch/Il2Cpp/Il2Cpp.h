#pragma once
#include "Il2CppAPI.h"

namespace Il2CppExtend
{
	/*****************Il2CppArrayT********************/

	/// <summary>
	/// Il2CppArray 泛型
	/// </summary>
	/// <typeparam name="T"></typeparam>
	template<class T>
	class Il2CppArrayT : public Il2CppArray
	{
	private:
		T mValue[1];		//0x20
	public:
		/// <summary>
		/// 获取数组长度
		/// </summary>
		size_t GetCount() const;
		/// <summary>
		/// 获取数组头指针
		/// </summary>
		T* GetItemsPointer();
	};

	template<class T>
	size_t Il2CppArrayT<T>::GetCount() const
	{
		return this->max_length;
	}

	template<class T>
	T* Il2CppArrayT<T>::GetItemsPointer()
	{
		return this->mValue;
	}
}


namespace Il2CppUtils
{
	/// <summary>
	/// 获取模块对象
	/// </summary>
	/// <param name="name">模块名</param>
	const Il2CppAssembly* GetAssemblyByName(const char* name);

	/// <summary>
	/// 获取模块映像
	/// </summary>
	/// <param name="name">模块名</param>
	const Il2CppImage* GetImageByName(const char* name);
}