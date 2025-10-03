#pragma once

#include "Il2Cpp.h"
namespace BulletHellCheat
{
	/// <summary>
	/// 弹幕游戏管理器
	/// </summary>
	class BulletHellManager : public Il2CppObject
	{
	public:
		BulletHellManager() = delete;
		BulletHellManager(const BulletHellManager&) = delete;
		BulletHellManager(BulletHellManager&&) = delete;
		BulletHellManager& operator=(const BulletHellManager&) = delete;
		BulletHellManager& operator=(BulletHellManager&&) = delete;
		~BulletHellManager() = delete;

	public:
		/// <summary>
		/// 获取类
		/// </summary>
		static Il2CppClass* Class();
		/// <summary>
		/// 补丁(静态)
		/// </summary>
		static void Patch();
	};
}

