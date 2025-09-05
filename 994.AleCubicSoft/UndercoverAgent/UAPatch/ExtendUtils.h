#pragma once

#include <windows.h>
#include "detours.h"

namespace HookUtils
{
	class InlineHook
	{
	public:
		InlineHook() = delete;
		InlineHook(const InlineHook&) = delete;
		InlineHook(InlineHook&&) = delete;
		InlineHook& operator=(const InlineHook&) = delete;
		InlineHook& operator=(InlineHook&&) = delete;
		~InlineHook() = delete;


		template<class T>
		static void Hook(T& OriginalFunction, T DetourFunction)
		{
			DetourUpdateThread(GetCurrentThread());
			DetourTransactionBegin();
			DetourAttach(&(PVOID&)OriginalFunction, (PVOID&)DetourFunction);
			DetourTransactionCommit();
		}

		template<class T>
		static void UnHook(T& OriginalFunction, T DetourFunction)
		{
			DetourUpdateThread(GetCurrentThread());
			DetourTransactionBegin();
			DetourDetach(&(PVOID&)OriginalFunction, (PVOID&)DetourFunction);
			DetourTransactionCommit();
		}
	};
}



