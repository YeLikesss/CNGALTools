#pragma once

#include "Il2Cpp.h"

namespace TSKTContainer
{
	/// <summary>
	/// 可序列化顺序字典
	/// </summary>
	/// <typeparam name="K">键</typeparam>
	/// <typeparam name="V">值</typeparam>
	template<class K, class V>
	class SerializableOrderedDictionary : public Il2CppObject
	{
	public:
		SerializableOrderedDictionary() = delete;
		SerializableOrderedDictionary(const SerializableOrderedDictionary&) = delete;
		SerializableOrderedDictionary(SerializableOrderedDictionary&&) = delete;
		SerializableOrderedDictionary& operator=(const SerializableOrderedDictionary&) = delete;
		SerializableOrderedDictionary& operator=(SerializableOrderedDictionary&&) = delete;
		~SerializableOrderedDictionary() = delete;
	public:
		void __cdecl Add(K key, V value, const MethodInfo* method);
		bool __cdecl TryGetValue(K key, V* result, const MethodInfo* method);
		void __cdecl Clear(const MethodInfo* method);

	public:
		using tAdd = decltype(&SerializableOrderedDictionary::Add);
		using tTryGetValue = decltype(&SerializableOrderedDictionary::TryGetValue);
		using tClear = decltype(&SerializableOrderedDictionary::Clear);
	};
}