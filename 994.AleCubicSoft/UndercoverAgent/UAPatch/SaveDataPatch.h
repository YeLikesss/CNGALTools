#pragma once

#include "Il2Cpp.h"

namespace SaveDataPatch
{
	class SaveData;

	/// <summary>
	/// 衣服破损级别
	/// </summary>
	enum DressDamageLevel
	{
		None,
		Little,
		NakedBreast,
		Underwear,
		Nude,
	};

	/// <summary>
	/// 存档类
	/// </summary>
	class SaveData : public Il2CppObject
	{
	public:
		SaveData() = delete;
		SaveData(const SaveData&) = delete;
		SaveData(SaveData&&) = delete;
		SaveData& operator=(const SaveData&) = delete;
		SaveData& operator=(SaveData&&) = delete;
		~SaveData() = delete;
	public:
		/// <summary>
		/// 获取唯一实例静态方法类型
		/// </summary>
		using tSaveData_Instance_Get = SaveData* (*)(const MethodInfo* method);

	public:
		/// <summary>
		/// 获取类
		/// </summary>
		static Il2CppClass* Class();

		/// <summary>
		/// 补丁存档
		/// </summary>
		void Patch();
	};


}

