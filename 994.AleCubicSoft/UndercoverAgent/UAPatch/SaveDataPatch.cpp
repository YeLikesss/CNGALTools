#include "SaveDataPatch.h"
#include "TSKTContainer.h"

namespace SaveDataPatch
{
	//*****************SaveData*********************//
	Il2CppClass* SaveData::Class()
	{
		const Il2CppImage* image = Il2CppUtils::GetImageByName("Assembly-CSharp");
		return il2cpp_class_from_name(image, "TSKT", "SaveData");
	}

	void SaveData::Patch()
	{
		Il2CppClass* cls = SaveData::Class();

		//立绘鉴赏解锁
		{
			FieldInfo* damageArmorUnlocked_Field = il2cpp_class_get_field_from_name(cls, "damageArmorUnlocked");
			
			DressDamageLevel level = DressDamageLevel::Nude;
			il2cpp_field_set_value(this, damageArmorUnlocked_Field, &level);
		}

		//CG鉴赏解锁
		{
			FieldInfo* memoryExecutedCountMap_Field = il2cpp_class_get_field_from_name(cls, "memoryExecutedCountMap");

			Il2CppObject* memoryExecutedCountMap = il2cpp_field_get_value_object(memoryExecutedCountMap_Field, this);

			//获取字典泛型实例类
			Il2CppClass* cls_SerializableOrderedDictionary_int_int = il2cpp_object_get_class(memoryExecutedCountMap);
			const MethodInfo* method_Add = il2cpp_class_get_method_from_name(cls_SerializableOrderedDictionary_int_int, "Add", 2);
			const MethodInfo* method_Clear = il2cpp_class_get_method_from_name(cls_SerializableOrderedDictionary_int_int, "Clear", 0);

			TSKTContainer::SerializableOrderedDictionary<__int32, __int32>* dict = (TSKTContainer::SerializableOrderedDictionary<__int32, __int32>*)memoryExecutedCountMap;

			TSKTContainer::SerializableOrderedDictionary<__int32, __int32>::tAdd add_func = nullptr;
			TSKTContainer::SerializableOrderedDictionary<__int32, __int32>::tClear clear_func = nullptr;
			*((void**)&add_func) = method_Add->methodPointer;
			*((void**)&clear_func) = method_Clear->methodPointer;

			//清空字典
			(dict->*clear_func)(method_Clear);

			//设置关卡CG解锁 关卡1-6 + 结局 共7个
			for (__int32 i = 0; i < 7; ++i)
			{
				(dict->*add_func)(i, 1, method_Add);
			}
		}

		//关卡解锁
		{
			FieldInfo* floorCleareds_Field = il2cpp_class_get_field_from_name(cls, "floorCleareds");

			Il2CppObject* array = il2cpp_field_get_value_object(floorCleareds_Field, this);
			Il2CppExtend::Il2CppArrayT<bool>* floorCleareds = (Il2CppExtend::Il2CppArrayT<bool>*)array;

			for (size_t i = 0u; i < floorCleareds->GetCount(); ++i)
			{
				floorCleareds->GetItemsPointer()[i] = true;
			}
		}
	}

	//*===============================================*//
}
