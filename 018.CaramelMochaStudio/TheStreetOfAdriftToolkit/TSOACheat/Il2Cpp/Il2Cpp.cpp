#include "Il2Cpp.h"
#include <string.h>

namespace Il2CppExtend
{
}

namespace Il2CppUtils
{
	const Il2CppAssembly* GetAssemblyByName(const char* name)
	{
		Il2CppDomain* domain = il2cpp_domain_get();
		size_t size = 0u;
		const Il2CppAssembly** assemblies = il2cpp_domain_get_assemblies(domain, &size);
		for (size_t i = 0u; i < size; ++i)
		{
			if (!strcmp(assemblies[i]->aname.name, name))
			{
				return assemblies[i];
			}
		}
		return nullptr;
	}

	const Il2CppImage* GetImageByName(const char* name)
	{
		if (const Il2CppAssembly* assembly = GetAssemblyByName(name))
		{
			return il2cpp_assembly_get_image(assembly);
		}
		return nullptr;
	}
}