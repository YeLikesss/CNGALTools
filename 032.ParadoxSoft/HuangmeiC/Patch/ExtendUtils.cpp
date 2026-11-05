#include "ExtendUtils.h"
#include <immintrin.h>

namespace ExtendUtils
{
	void Memory::Copy(void* dst, const void* src, uintptr_t size)
	{
		__movsb((PBYTE)dst, (const BYTE*)src, size);
	}

}