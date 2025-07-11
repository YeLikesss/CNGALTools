#include "UnityEngine.h"
#include <string.h>
namespace UnityEngine
{
	/********************Object*************************/
	const char* Object::NameSpace = "UnityEngine";
	const char* Object::ClassName = "Object";

	static void* g_Object_GetName = nullptr;
	Il2CppString* Object::GetName(Il2CppObject* obj)
	{
		if (!g_Object_GetName)
		{
			const Il2CppImage* image = Il2CppUtils::GetImageByName(ModuleName);
			Il2CppClass* cls = il2cpp_class_from_name(image, NameSpace, ClassName);
			const PropertyInfo* prop = il2cpp_class_get_property_from_name(cls, "name");
			const MethodInfo* method_get = il2cpp_property_get_get_method((PropertyInfo*)prop);

			g_Object_GetName = method_get->methodPointer;
		}
		return ((Il2CppString * (*)(Il2CppObject*, const MethodInfo*))g_Object_GetName)(obj, nullptr);
	}

	static void* g_Object_Destroy = nullptr;
	void Object::Destroy(Il2CppObject* obj)
	{
		if (!g_Object_Destroy)
		{
			const Il2CppImage* image = Il2CppUtils::GetImageByName(ModuleName);
			Il2CppClass* cls = il2cpp_class_from_name(image, NameSpace, ClassName);
			const MethodInfo* method = il2cpp_class_get_method_from_name(cls, "Destroy", 1);

			g_Object_Destroy = method->methodPointer;
		}
		return ((void(*)(Il2CppObject*, const MethodInfo*))g_Object_Destroy)(obj, nullptr);
	}

	static void* g_Object_DestroyImmediate = nullptr;
	void Object::DestroyImmediate(Il2CppObject* obj)
	{
		if (!g_Object_DestroyImmediate)
		{
			const Il2CppImage* image = Il2CppUtils::GetImageByName(ModuleName);
			Il2CppClass* cls = il2cpp_class_from_name(image, NameSpace, ClassName);
			const MethodInfo* method = il2cpp_class_get_method_from_name(cls, "DestroyImmediate", 1);

			g_Object_DestroyImmediate = method->methodPointer;
		}
		return ((void(*)(Il2CppObject*, const MethodInfo*))g_Object_DestroyImmediate)(obj, nullptr);
	}
	/*======================================================*/

	/********************GameObject*************************/
	const char* GameObject::NameSpace = "UnityEngine";
	const char* GameObject::ClassName = "GameObject";

	const Il2CppType* GameObject::GetNativeType()
	{
		const Il2CppImage* image = Il2CppUtils::GetImageByName(ModuleName);
		Il2CppClass* cls = il2cpp_class_from_name(image, NameSpace, ClassName);
		return il2cpp_class_get_type(cls);
	}

	static void* g_GameObject_GetComponentsInternal = nullptr;
	Il2CppArray* GameObject::GetComponentsInternal(Il2CppObject* gameObj, const Il2CppObject* type, bool useSearchTypeAsArrayReturnType, bool recursive, bool includeInactive, bool reverse, void* resultList)
	{
		if (!g_GameObject_GetComponentsInternal)
		{
			const Il2CppImage* image = Il2CppUtils::GetImageByName(ModuleName);
			Il2CppClass* cls = il2cpp_class_from_name(image, NameSpace, ClassName);
			const MethodInfo* method = il2cpp_class_get_method_from_name(cls, "GetComponentsInternal", 6);

			g_GameObject_GetComponentsInternal = method->methodPointer;
		}
		return ((Il2CppArray * (*)(Il2CppObject*, const Il2CppObject*, bool, bool, bool, bool, void*, const MethodInfo*))g_GameObject_GetComponentsInternal)
				(gameObj, type, useSearchTypeAsArrayReturnType, recursive, includeInactive, reverse, resultList, nullptr);
	}
	Il2CppArray* GameObject::GetComponentsInChildren(Il2CppObject* gameObj, const Il2CppObject* type, bool includeInactive)
	{
		return GameObject::GetComponentsInternal(gameObj, type, true, true, includeInactive, false, nullptr);
	}
	Il2CppArray* GameObject::GetComponentsInParent(Il2CppObject* gameObj, const Il2CppObject* type, bool includeInactive)
	{
		return GameObject::GetComponentsInternal(gameObj, type, true, true, includeInactive, true, nullptr);
	}
	/*======================================================*/

	/********************Component*************************/
	const char* Component::NameSpace = "UnityEngine";
	const char* Component::ClassName = "Component";

	Il2CppObject* Component::New()
	{
		const Il2CppImage* image = Il2CppUtils::GetImageByName(ModuleName);
		Il2CppClass* cls = il2cpp_class_from_name(image, NameSpace, ClassName);
		return il2cpp_object_new(cls);
	}
	const Il2CppType* Component::GetNativeType()
	{
		const Il2CppImage* image = Il2CppUtils::GetImageByName(ModuleName);
		Il2CppClass* cls = il2cpp_class_from_name(image, NameSpace, ClassName);
		return il2cpp_class_get_type(cls);
	}

	static void* g_Component_GetGameObject = nullptr;
	Il2CppObject* Component::GetGameObject(Il2CppObject* component)
	{
		if (!g_Component_GetGameObject)
		{
			const Il2CppImage* image = Il2CppUtils::GetImageByName(ModuleName);
			Il2CppClass* cls = il2cpp_class_from_name(image, NameSpace, ClassName);
			const PropertyInfo* prop = il2cpp_class_get_property_from_name(cls, "gameObject");
			const MethodInfo* method_get = il2cpp_property_get_get_method((PropertyInfo*)prop);

			g_Component_GetGameObject = method_get->methodPointer;
		}
		return ((Il2CppObject * (*)(Il2CppObject*, const MethodInfo*))g_Component_GetGameObject)(component, nullptr);
	}

	static void* g_Component_GetTransform = nullptr;
	Il2CppObject* Component::GetTransform(Il2CppObject* component)
	{
		if (!g_Component_GetTransform)
		{
			const Il2CppImage* image = Il2CppUtils::GetImageByName(ModuleName);
			Il2CppClass* cls = il2cpp_class_from_name(image, NameSpace, ClassName);
			const PropertyInfo* prop = il2cpp_class_get_property_from_name(cls, "transform");
			const MethodInfo* method_get = il2cpp_property_get_get_method((PropertyInfo*)prop);

			g_Component_GetTransform = method_get->methodPointer;
		}
		return ((Il2CppObject * (*)(Il2CppObject*, const MethodInfo*))g_Component_GetTransform)(component, nullptr);
	}
	/*====================================================*/


	/********************Transform*************************/
	const char* Transform::NameSpace = "UnityEngine";
	const char* Transform::ClassName = "Transform";

	static void* g_Transform_GetParent = nullptr;
	Il2CppObject* Transform::GetParent(Il2CppObject* transform)
	{
		if (!g_Transform_GetParent)
		{
			const Il2CppImage* image = Il2CppUtils::GetImageByName(ModuleName);
			Il2CppClass* cls = il2cpp_class_from_name(image, NameSpace, ClassName);
			const PropertyInfo* prop = il2cpp_class_get_property_from_name(cls, "parent");
			const MethodInfo* method_get = il2cpp_property_get_get_method((PropertyInfo*)prop);

			g_Transform_GetParent = method_get->methodPointer;
		}
		return ((Il2CppObject * (*)(Il2CppObject*, const MethodInfo*))g_Transform_GetParent)(transform, nullptr);
	}

	static void* g_Transform_SetParent = nullptr;
	void Transform::SetParent(Il2CppObject* transform, Il2CppObject* parent)
	{
		if (!g_Transform_SetParent)
		{
			const Il2CppImage* image = Il2CppUtils::GetImageByName(ModuleName);
			Il2CppClass* cls = il2cpp_class_from_name(image, NameSpace, ClassName);
			const PropertyInfo* prop = il2cpp_class_get_property_from_name(cls, "parent");
			const MethodInfo* method_set = il2cpp_property_get_set_method((PropertyInfo*)prop);

			g_Transform_SetParent = method_set->methodPointer;
		}
		return ((void(*)(Il2CppObject*, Il2CppObject*, const MethodInfo*))g_Transform_SetParent)(transform, parent, nullptr);
	}

	/*====================================================*/
}
