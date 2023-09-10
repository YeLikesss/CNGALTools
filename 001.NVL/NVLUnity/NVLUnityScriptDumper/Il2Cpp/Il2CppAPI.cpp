#include "Il2CppAPI.h"
#include <Windows.h>

void* g_Il2CppSetMemoryCallbacksPtr = nullptr;
void* g_Il2CppGetCorlibPtr = nullptr;
void* g_Il2CppResolveInternalCallPtr = nullptr;
void* g_Il2CppAllocPtr = nullptr;
void* g_Il2CppFreePtr = nullptr;

//array
void* g_Il2CppArrayClassGetPtr = nullptr;
void* g_Il2CppArrayLengthPtr = nullptr;
void* g_Il2CppArrayGetByteLengthPtr = nullptr;
void* g_Il2CppArrayNewPtr = nullptr;
void* g_Il2CppArrayNewSpecificPtr = nullptr;
void* g_Il2CppArrayNewFullPtr = nullptr;
void* g_Il2CppBoundedArrayClassGetPtr = nullptr;
void* g_Il2CppArrayElementSizePtr = nullptr;

//assembly
void* g_Il2CppAssaemblyGetImagePtr = nullptr;

//class
void* g_Il2CppClassFromNamePtr = nullptr;
void* g_Il2CppClassGetFieldsPtr = nullptr;
void* g_Il2CppClassGetPropertiesPtr = nullptr;
void* g_Il2CppClassGetPropertyFromNamePtr = nullptr;
void* g_Il2CppClassGetFieldFromNamePtr = nullptr;
void* g_Il2CppClassGetMethodsPtr = nullptr;
void* g_Il2CppClassGetMethodFromNamePtr = nullptr;

//domain
void* g_Il2CppDomainGetPtr = nullptr;
void* g_Il2CppDomainAssemblyOpenPtr = nullptr;
void* g_Il2CppDomainGetAssemblies = nullptr;

//property
void* g_Il2CppPropertyGetFlagsPtr = nullptr;
void* g_Il2CppPropertyGetGetMethodPtr = nullptr;
void* g_Il2CppPropertyGetSetMethodPtr = nullptr;
void* g_Il2CppPropertyGetNamePtr = nullptr;
void* g_Il2CppPropertyGetParentPtr = nullptr;

//object
void* g_Il2CppObjectNewPtr = nullptr;

//string
void* g_Il2CppStringNewUTF16Ptr = nullptr;

// thread
void* g_Il2CppThreadCurrentPtr = nullptr;
void* g_Il2CppThreadAttachPtr = nullptr;
void* g_Il2CppThreadDetachPtr = nullptr;
void* g_Il2CppThreadGetAllAttachedThreadsPtr = nullptr;
void* g_Il2CppIsVmThreadPtr = nullptr;



bool Il2CppInitialize() 
{
	HMODULE il2CppExcutableBase = GetModuleHandleW(L"GameAssembly.dll");
	if (!il2CppExcutableBase) 
	{
		return false;
	}

	g_Il2CppSetMemoryCallbacksPtr = GetProcAddress(il2CppExcutableBase, "il2cpp_set_memory_callbacks");
	g_Il2CppGetCorlibPtr = GetProcAddress(il2CppExcutableBase, "il2cpp_get_corlib");
	g_Il2CppResolveInternalCallPtr = GetProcAddress(il2CppExcutableBase, "il2cpp_resolve_icall");
	g_Il2CppAllocPtr = GetProcAddress(il2CppExcutableBase, "il2cpp_alloc");
	g_Il2CppFreePtr = GetProcAddress(il2CppExcutableBase, "il2cpp_free");

	//array
	g_Il2CppArrayClassGetPtr = GetProcAddress(il2CppExcutableBase, "il2cpp_array_class_get");
	g_Il2CppArrayLengthPtr = GetProcAddress(il2CppExcutableBase, "il2cpp_array_length");
	g_Il2CppArrayGetByteLengthPtr = GetProcAddress(il2CppExcutableBase, "il2cpp_array_get_byte_length");
	g_Il2CppArrayNewPtr = GetProcAddress(il2CppExcutableBase, "il2cpp_array_new");
	g_Il2CppArrayNewSpecificPtr = GetProcAddress(il2CppExcutableBase, "il2cpp_array_new_specific");
	g_Il2CppArrayNewFullPtr = GetProcAddress(il2CppExcutableBase, "il2cpp_array_new_full");
	g_Il2CppBoundedArrayClassGetPtr = GetProcAddress(il2CppExcutableBase, "il2cpp_bounded_array_class_get");
	g_Il2CppArrayElementSizePtr = GetProcAddress(il2CppExcutableBase, "il2cpp_array_element_size");

	//assembly
	g_Il2CppAssaemblyGetImagePtr = GetProcAddress(il2CppExcutableBase, "il2cpp_assembly_get_image");

	//class
	g_Il2CppClassFromNamePtr = GetProcAddress(il2CppExcutableBase, "il2cpp_class_from_name");
	g_Il2CppClassGetFieldsPtr = GetProcAddress(il2CppExcutableBase, "il2cpp_class_get_fields");
	g_Il2CppClassGetPropertiesPtr = GetProcAddress(il2CppExcutableBase, "il2cpp_class_get_properties");
	g_Il2CppClassGetPropertyFromNamePtr = GetProcAddress(il2CppExcutableBase, "il2cpp_class_get_property_from_name");
	g_Il2CppClassGetFieldFromNamePtr = GetProcAddress(il2CppExcutableBase, "il2cpp_class_get_field_from_name");
	g_Il2CppClassGetMethodsPtr = GetProcAddress(il2CppExcutableBase, "il2cpp_class_get_methods");
	g_Il2CppClassGetMethodFromNamePtr = GetProcAddress(il2CppExcutableBase, "il2cpp_class_get_method_from_name");

	//domain
	g_Il2CppDomainGetPtr = GetProcAddress(il2CppExcutableBase, "il2cpp_domain_get");
	g_Il2CppDomainAssemblyOpenPtr = GetProcAddress(il2CppExcutableBase, "il2cpp_domain_assembly_open");
	g_Il2CppDomainGetAssemblies = GetProcAddress(il2CppExcutableBase, "il2cpp_domain_get_assemblies");

	//property
	g_Il2CppPropertyGetFlagsPtr = GetProcAddress(il2CppExcutableBase, "il2cpp_property_get_flags");
	g_Il2CppPropertyGetGetMethodPtr = GetProcAddress(il2CppExcutableBase, "il2cpp_property_get_get_method");
	g_Il2CppPropertyGetSetMethodPtr = GetProcAddress(il2CppExcutableBase, "il2cpp_property_get_set_method");
	g_Il2CppPropertyGetNamePtr = GetProcAddress(il2CppExcutableBase, "il2cpp_property_get_name");
	g_Il2CppPropertyGetParentPtr = GetProcAddress(il2CppExcutableBase, "il2cpp_property_get_parent");

	//object
	g_Il2CppObjectNewPtr = GetProcAddress(il2CppExcutableBase, "il2cpp_object_new");
	
	//string
	g_Il2CppStringNewUTF16Ptr = GetProcAddress(il2CppExcutableBase, "il2cpp_string_new_utf16");

	//thread
	g_Il2CppThreadCurrentPtr = GetProcAddress(il2CppExcutableBase, "il2cpp_thread_current");
	g_Il2CppThreadAttachPtr = GetProcAddress(il2CppExcutableBase, "il2cpp_thread_attach");
	g_Il2CppThreadDetachPtr = GetProcAddress(il2CppExcutableBase, "il2cpp_thread_detach");
	g_Il2CppThreadGetAllAttachedThreadsPtr = GetProcAddress(il2CppExcutableBase, "il2cpp_thread_get_all_attached_threads");
	g_Il2CppIsVmThreadPtr = GetProcAddress(il2CppExcutableBase, "il2cpp_is_vm_thread");

	return true;
}

void
Il2CppSetMemoryCallbacks(Il2CppMemoryCallbacks* callbacks) 
{
	((void(*)(Il2CppMemoryCallbacks*))g_Il2CppSetMemoryCallbacksPtr)(callbacks);
}

const Il2CppImage*
Il2CppGetCorlib()
{
	return ((const Il2CppImage* (*)())g_Il2CppGetCorlibPtr)();
}

Il2CppMethodPointer
Il2CppResolveInternalCall(const char* name)
{
	return ((Il2CppMethodPointer (*)(const char*))g_Il2CppResolveInternalCallPtr)(name);
}

void*
Il2CppAlloc(size_t size)
{
	return ((void* (*)(size_t))g_Il2CppAllocPtr)(size);
}

void
Il2CppFree(void* ptr)
{
	((void (*)(void*))g_Il2CppFreePtr)(ptr);
}

//array
Il2CppClass*
Il2CppArrayClassGet(Il2CppClass* element_class, uint32_t rank)
{
	return ((Il2CppClass* (*)(Il2CppClass*, uint32_t))g_Il2CppArrayClassGetPtr)(element_class, rank);
}

uint32_t
Il2CppArrayLength(Il2CppArray* array)
{
	return ((uint32_t (*)(Il2CppArray*))g_Il2CppArrayLengthPtr)(array);
}

uint32_t
Il2CppArrayGetByteLength(Il2CppArray* array) 
{
	return ((uint32_t (*)(Il2CppArray*))g_Il2CppArrayGetByteLengthPtr)(array);
}

Il2CppArray*
Il2CppArrayNew(Il2CppClass* elementTypeInfo, il2cpp_array_size_t length)
{
	return ((Il2CppArray* (*)(Il2CppClass*, il2cpp_array_size_t))g_Il2CppArrayNewPtr)(elementTypeInfo, length);
}

Il2CppArray*
Il2CppArrayNewSpecific(Il2CppClass* arrayTypeInfo, il2cpp_array_size_t length)
{
	return ((Il2CppArray* (*)(Il2CppClass*, il2cpp_array_size_t))g_Il2CppArrayNewSpecificPtr)(arrayTypeInfo, length);
}

Il2CppArray*
Il2CppArrayNewFull(Il2CppClass* array_class, il2cpp_array_size_t* lengths, il2cpp_array_size_t* lower_bounds)
{
	return ((Il2CppArray* (*)(Il2CppClass*, il2cpp_array_size_t*, il2cpp_array_size_t*))g_Il2CppArrayNewFullPtr)(array_class, lengths, lower_bounds);
}

Il2CppClass*
Il2CppBoundedArrayClassGet(Il2CppClass* element_class, uint32_t rank, bool bounded)
{
	return ((Il2CppClass* (*)(Il2CppClass*, uint32_t, bool))g_Il2CppBoundedArrayClassGetPtr)(element_class, rank, bounded);
}

int
Il2CppArrayElementSize(const Il2CppClass* array_class)
{
	return ((int (*)(const Il2CppClass*))g_Il2CppArrayElementSizePtr)(array_class);
}

//class
Il2CppClass*
Il2CppClassFromName(const Il2CppImage* image, const char* namespaze, const char* name)
{
	return ((Il2CppClass* (*)(const Il2CppImage*, const char*, const char*))g_Il2CppClassFromNamePtr)(image, namespaze, name);
}

FieldInfo*
Il2CppClassGetFields(Il2CppClass* klass, void** iter) 
{
	return ((FieldInfo* (*)(Il2CppClass*, void**))g_Il2CppClassGetFieldsPtr)(klass, iter);
}

const PropertyInfo*
Il2CppClassGetProperties(Il2CppClass* klass, void** iter) 
{
	return ((const PropertyInfo* (*)(Il2CppClass*, void**))g_Il2CppClassGetPropertiesPtr)(klass, iter);
}

const PropertyInfo*
Il2CppClassGetPropertyFromName(Il2CppClass* klass, const char* name) 
{
	return ((const PropertyInfo* (*)(Il2CppClass*, const char*))g_Il2CppClassGetPropertyFromNamePtr)(klass, name);
}

FieldInfo*
Il2CppClassGetFieldFromName(Il2CppClass* klass, const char* name) 
{
	return ((FieldInfo* (*)(Il2CppClass*, const char*))g_Il2CppClassGetFieldFromNamePtr)(klass, name);
}

const MethodInfo*
Il2CppClassGetMethods(Il2CppClass* klass, void** iter) 
{
	return ((const MethodInfo* (*)(Il2CppClass*, void**))g_Il2CppClassGetMethodsPtr)(klass, iter);
}

const MethodInfo*
Il2CppClassGetMethodFromName(Il2CppClass* klass, const char* name, int argsCount) 
{
	return ((const MethodInfo* (*)(Il2CppClass*, const char*, int))g_Il2CppClassGetMethodFromNamePtr)(klass, name, argsCount);
}

//assembly
Il2CppImage*
Il2CppAssaemblyGetImage(const Il2CppAssembly* assembly)
{
	return ((Il2CppImage* (*)(const Il2CppAssembly*))g_Il2CppAssaemblyGetImagePtr)(assembly);
}

//domain
Il2CppDomain*
Il2CppDomainGet()
{
	return ((Il2CppDomain* (*)())g_Il2CppDomainGetPtr)();
}

const Il2CppAssembly*
Il2CppDomainAssemblyOpen(Il2CppDomain* domain, const char* name)
{
	return ((const Il2CppAssembly* (*)(Il2CppDomain*, const char*))g_Il2CppDomainAssemblyOpenPtr)(domain, name);
}

const Il2CppAssembly**
Il2CppDomainGetAssemblies(const Il2CppDomain* domain, size_t* size)
{
	return ((const Il2CppAssembly** (*)(const Il2CppDomain*, size_t*))g_Il2CppDomainGetAssemblies)(domain, size);
}

//property
uint32_t
Il2CppPropertyGetFlags(PropertyInfo* prop)
{
	return ((uint32_t (*)(PropertyInfo*))g_Il2CppPropertyGetFlagsPtr)(prop);
}

const MethodInfo*
Il2CppPropertyGetGetMethod(PropertyInfo* prop)
{
	return ((const MethodInfo* (*)(PropertyInfo*))g_Il2CppPropertyGetGetMethodPtr)(prop);
}

const MethodInfo*
Il2CppPropertyGetSetMethod(PropertyInfo* prop)
{
	return ((const MethodInfo* (*)(PropertyInfo*))g_Il2CppPropertyGetSetMethodPtr)(prop);
}

const char*
Il2CppPropertyGetName(PropertyInfo* prop)
{
	return ((const char* (*)(PropertyInfo*))g_Il2CppPropertyGetNamePtr)(prop);
}

Il2CppClass*
Il2CppPropertyGetParent(PropertyInfo* prop)
{
	return ((Il2CppClass * (*)(PropertyInfo*))g_Il2CppPropertyGetParentPtr)(prop);
}

//object
Il2CppObject*
Il2CppObjectNew(const Il2CppClass* klass) 
{
	return ((Il2CppObject* (*)(const Il2CppClass*))g_Il2CppObjectNewPtr)(klass);
}

//string
Il2CppString*
Il2CppStringNewUTF16(const Il2CppChar* text, int32_t len)
{
	return ((Il2CppString* (*)(const Il2CppChar*, int32_t))g_Il2CppStringNewUTF16Ptr)(text, len);
}

// thread
Il2CppThread*
Il2CppThreadCurrent() 
{
	return ((Il2CppThread* (*)())g_Il2CppThreadCurrentPtr)();
}

Il2CppThread*
Il2CppThreadAttach(Il2CppDomain* domain) 
{
	return ((Il2CppThread* (*)(Il2CppDomain*))g_Il2CppThreadAttachPtr)(domain);
}

void
Il2CppThreadDettach(Il2CppThread* thread) 
{
	((void (*)(Il2CppThread*))g_Il2CppThreadDetachPtr)(thread);
}

Il2CppThread**
Il2CppThreadGetAllAttachedThreads(size_t* size) 
{
	return ((Il2CppThread** (*)(size_t*))g_Il2CppThreadGetAllAttachedThreadsPtr)(size);
}

bool
Il2CppIsVmThread(Il2CppThread* thread) 
{
	return ((bool (*)(Il2CppThread*))g_Il2CppIsVmThreadPtr)(thread);
}






