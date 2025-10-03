#include "BulletHellCheat.h"
#include <Windows.h>

namespace BulletHellCheat
{
	//****************BulletHellManager*****************//
	Il2CppClass* BulletHellManager::Class()
	{
		const Il2CppImage* image = Il2CppUtils::GetImageByName("Assembly-CSharp");
		return il2cpp_class_from_name(image, "", "BulletHellManager");
	}
	void BulletHellManager::Patch()
	{
		Il2CppClass* cls = BulletHellManager::Class();

		//静态构造函数处理
		{
			const MethodInfo* cctor_Method = il2cpp_class_get_method_from_name(cls, ".cctor", 0);
			void* cctorFunc = cctor_Method->methodPointer;

			//调用一次
			((void(*)(const MethodInfo * methodInfo))cctorFunc)(nullptr);

			//屏蔽
			DWORD oldProtect = 0u;
			::VirtualProtect(cctorFunc, 4, PAGE_EXECUTE_READWRITE, &oldProtect);
			*(unsigned __int32*)cctorFunc = 0x909090C3u;
			::VirtualProtect(cctorFunc, 4, oldProtect, &oldProtect);
			::FlushInstructionCache((HANDLE)-1, cctorFunc, 4);
		}

		//修改敌人最大血量
		{
			FieldInfo* MaxEnemyHP_Field = il2cpp_class_get_field_from_name(cls, "MaxEnemyHP");

			__int32 maxHP = 20;
			il2cpp_field_static_set_value(MaxEnemyHP_Field, &maxHP);
		}
	}
	//*=================================================*//
}