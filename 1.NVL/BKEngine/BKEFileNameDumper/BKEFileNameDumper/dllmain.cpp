#include <Windows.h>
#include <string>
#include "log.h"
#include "stringhelper.h"
#include "util.h"
#include "path.h"
#include "file.h"
#include "pe.h"

//仅仅《十二色的季节》使用了Hash
#define BKARC_GetFileEntry_Vptr_RVA 0x29633C
struct StdWStringMSVC2013
{
    union
    {
        wchar_t* LongString;
        wchar_t ShortString[8];
    } String;
    int StringLength;
    int MaxStringLength;

    const wchar_t* ConstString()
    {
        if (this->MaxStringLength >= 8)
        {
            return this->String.LongString;
        }
        else
        {
            return this->String.ShortString;
        }
    }
    int Length()
    {
        return this->StringLength;
    }
};
typedef void* (_fastcall* tBKARC_GetFileEntry)(void*, void*, StdWStringMSVC2013*);


static tBKARC_GetFileEntry g_BKARC_GetFileEntry_FuncPtr = NULL;
static Log::Logger g_FileNameDumper;

//Hook 封包查找函数
__declspec(noinline)
void* __fastcall HookBKARCGetFileEntry(void* thisObj, void* unusedEdx,StdWStringMSVC2013* string)
{
    void* entry = g_BKARC_GetFileEntry_FuncPtr(thisObj, NULL, string);
    //文件存在
    if (entry) 
    {
        g_FileNameDumper.Write(L"%s\n", string->ConstString());
    }
    return entry;
}

void InstallHook() 
{
    PVOID gameBase = GetModuleHandleW(NULL);
    PVOID bkarc_GetFileEntry_Vptr_VA = (BYTE*)gameBase + BKARC_GetFileEntry_Vptr_RVA;

    //保存原函数指针
    g_BKARC_GetFileEntry_FuncPtr = *(tBKARC_GetFileEntry*)bkarc_GetFileEntry_Vptr_VA;

    //虚表Hook
    PVOID HookBKARC_GetFileEntry_FuncPtr = HookBKARCGetFileEntry;
    PE::WriteMemory(bkarc_GetFileEntry_Vptr_VA, &HookBKARC_GetFileEntry_FuncPtr, sizeof(PVOID));
}

void Initialize() 
{
    std::wstring gameDirectory = Util::GetAppDirectoryW();
    std::wstring fileNameOutPath = gameDirectory + L"\\FileName.lst";

    File::Delete(fileNameOutPath);

    g_FileNameDumper.Open(fileNameOutPath.c_str());

    InstallHook();
}

void ShutDown() 
{
    g_FileNameDumper.Close();
}

BOOL APIENTRY DllMain(HMODULE hModule, DWORD ul_reason_for_call, LPVOID lpReserved)
{
    switch (ul_reason_for_call)
    {
        case DLL_PROCESS_ATTACH:
        {
            Initialize();
            break;
        }
        case DLL_THREAD_ATTACH:
        case DLL_THREAD_DETACH:
            break;
        case DLL_PROCESS_DETACH:
        {
            ShutDown();
            break;
        }
    }
    return TRUE;
}


extern "C" __declspec(dllexport) void Dummy() {}
