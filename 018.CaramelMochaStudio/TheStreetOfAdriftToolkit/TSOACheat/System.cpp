#include "System.h"
#include <string.h>

namespace System
{
	/********************Object*************************/
	const char* Object::NameSpace = "System";
	const char* Object::ClassName = "Object";

	static void* g_Object_GetType = nullptr;
	Il2CppObject* Object::GetType(Il2CppObject* obj)
	{
		if (!g_Object_GetType)
		{
			const Il2CppImage* image = il2cpp_get_corlib();
			Il2CppClass* cls = il2cpp_class_from_name(image, NameSpace, ClassName);
			const MethodInfo* method = il2cpp_class_get_method_from_name(cls, "GetType", 0);

			g_Object_GetType = method->methodPointer;
		}
		return ((Il2CppObject * (*)(Il2CppObject*, const MethodInfo*))g_Object_GetType)(obj, nullptr);
	}
	/*=================================================*/

	/********************String*************************/
	const char* String::NameSpace = "System";
	const char* String::ClassName = "String";

	bool String::IsEmpty(const Il2CppString* s)
	{
		if (s)
		{
			return s->length == 0u;
		}
		return false;
	}
	bool String::Equals(const Il2CppString* s1, const Il2CppString* s2)
	{
		if (s1 == s2)
		{
			return true;
		}
		if (s1 && s2 && s1->length == s2->length)
		{
			return !memcmp(s1->chars, s2->chars, s1->length * sizeof(Il2CppChar));
		}
		return false;
	}
	/*======================================================*/
}