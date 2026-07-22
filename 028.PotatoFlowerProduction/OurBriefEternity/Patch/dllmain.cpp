#include <Windows.h>
#include <immintrin.h>
#include "Ntdll/ntdll.h"

#ifdef _M_X64
#pragma comment(lib,"./Ntdll/ntdll_x64.lib")
#else
#pragma comment(lib,"./Ntdll/ntdll_x86.lib")
#endif

void PatchExecutablePath();

__declspec(noinline)
void PatchExecutablePath()
{
    TEB* teb64 = (TEB*)_readgsbase_u64();
    PEB* peb64 = teb64->ProcessEnvironmentBlock;

    PVOID exeImgBs = peb64->ImageBaseAddress;

    LIST_ENTRY* const root = &peb64->Ldr->InLoadOrderModuleList;
    LIST_ENTRY* curNode = root->Flink;
    while (root != curNode)
    {
        LDR_DATA_TABLE_ENTRY* moduleEntry = (LDR_DATA_TABLE_ENTRY*)curNode;
        if (moduleEntry->DllBase == exeImgBs)
        {
            UNICODE_STRING* bsName = &moduleEntry->BaseDllName;
            PWSTR ext = bsName->Buffer + (bsName->Length / sizeof(WCHAR)) - 3u;

            ext[0] = L'b';
            ext[1] = L'a';
            ext[2] = L'k';

            break;
        }
        curNode = curNode->Flink;
    }
}

BOOL APIENTRY DllMain(HMODULE hModule, DWORD ul_reason_for_call, LPVOID lpReserved)
{
    UNREFERENCED_PARAMETER(lpReserved);
    switch (ul_reason_for_call)
    {
        case DLL_PROCESS_ATTACH:
        {
            PatchExecutablePath();
            ::DisableThreadLibraryCalls(hModule);
            break;
        }
        case DLL_THREAD_ATTACH:
        case DLL_THREAD_DETACH:
        {
            break;
        }
        case DLL_PROCESS_DETACH:
        {
            break;
        }
    }
    return TRUE;
}

extern "C" __declspec(dllexport)
void CreateObject()
{
}
