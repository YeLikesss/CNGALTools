#include <Windows.h>
#include "detours.h"
#include "ntdll.h"

extern "C" __declspec(dllexport) void Dummy() { }

#define EncryptDll_CheckThreadStartAddress_RVA 0x68CB8      //加密DLL报错线程地址RVA
#define Xbundler_SteamApi_FileSize 0x40F90      //壳VFS内steam_api.dll模块文件大小
#define Xbundler_SteamApi_FileSize_MemoryAlign 0x41000     //壳VFS内steam_api.dll模块文件大小(对齐)
#define Xbundler_SteamApi_ImageSize 0x42000     //壳VFS内steam_api.dll模块内存大小
#define Xbundler_SteamApi_Kernel32ImportName_RVA 0x3B8BE     //壳VFS内steam_api.dll导入kernel32.dll的名称RVA
#define Xbundler_SteamApi_Advapi32ImportName_RVA 0x3B8FE     //壳VFS内steam_api.dll导入Advapi32.dll的名称RVA

#define tNtAllocateVirtualMemory decltype(&NtAllocateVirtualMemory)
#define tNtCreateThreadEx decltype(&NtCreateThreadEx)

static tNtAllocateVirtualMemory g_orgNtAllocateVirtualMemory = nullptr;    //原NtAllocateVirtualMemory地址
static tNtCreateThreadEx g_orgNtCreateThreadEx = nullptr;      //原NtCreateThreadEx地址

static PVOID g_Xbundler_SteamApi_ImageBase = nullptr;      //壳VFS内steam_api.dll基地址
static PVOID g_encryptDll_ImageBase = nullptr;      //加密dll基地址
static PVOID g_CheckThreadFunc = nullptr;      //检查线程开始地址


void InlineHook(PVOID* OriginalFunction, PVOID DetourFunction)
{
    DetourUpdateThread(GetCurrentThread());
    DetourTransactionBegin();
    DetourAttach(OriginalFunction, DetourFunction);
    DetourTransactionCommit();
}

void UnInlineHook(PVOID* OriginalFunction, PVOID DetourFunction)
{
    DetourUpdateThread(GetCurrentThread());
    DetourTransactionBegin();
    DetourDetach(OriginalFunction, DetourFunction);
    DetourTransactionCommit();
}

NTSTATUS NTAPI HookNtAllocateVirtualMemory(HANDLE ProcessHandle, PVOID* BaseAddress, ULONG_PTR ZeroBits, PSIZE_T RegionSize, ULONG AllocationType, ULONG Protect);
NTSTATUS NTAPI HookNtCreateThreadEx(PHANDLE ThreadHandle, ACCESS_MASK DesiredAccess, POBJECT_ATTRIBUTES ObjectAttributes, HANDLE ProcessHandle, PUSER_THREAD_START_ROUTINE StartRoutine, PVOID Argument, ULONG CreateFlags, SIZE_T ZeroBits, SIZE_T StackSize, SIZE_T MaximumStackSize, PPS_ATTRIBUTE_LIST AttributeList);

NTSTATUS NTAPI HookNtAllocateVirtualMemory(HANDLE ProcessHandle, PVOID* BaseAddress, ULONG_PTR ZeroBits, PSIZE_T RegionSize, ULONG AllocationType, ULONG Protect)
{
    BOOL isSteam = *RegionSize == Xbundler_SteamApi_FileSize;

    NTSTATUS status = g_orgNtAllocateVirtualMemory(ProcessHandle, BaseAddress, ZeroBits, RegionSize, AllocationType, Protect);

    if (status == STATUS_SUCCESS && (SIZE_T)ProcessHandle == MAXSIZE_T)
    {
        if (g_Xbundler_SteamApi_ImageBase)
        {
            PBYTE memBase = *(PBYTE*)BaseAddress;     //当前内存基地址
            PBYTE steamApiImageBase = (PBYTE)g_Xbundler_SteamApi_ImageBase;

            //判断范围    判断最后一次提交内存
            if (memBase >= steamApiImageBase && (memBase + *RegionSize) == (steamApiImageBase + Xbundler_SteamApi_ImageSize))
            {
                //修改导入Dll名称 --- kernel32.dll ---> steam_32.dll   advapi32.dll ---> steam_32.dll
                char steamEmuNameA[13]{ 's','t','e','a','m','_','3','2','.','d','l','l','\0' };
                memcpy(steamApiImageBase + Xbundler_SteamApi_Kernel32ImportName_RVA, steamEmuNameA, sizeof(steamEmuNameA));
                memcpy(steamApiImageBase + Xbundler_SteamApi_Advapi32ImportName_RVA, steamEmuNameA, sizeof(steamEmuNameA));

                //解除内存分配函数Hook
                UnInlineHook((PVOID*)&g_orgNtAllocateVirtualMemory, HookNtAllocateVirtualMemory);
                //Hook线程创建函数
                InlineHook((PVOID*)&g_orgNtCreateThreadEx, HookNtCreateThreadEx);
            }
        }
        else
        {
            //获取steam_api.dll基地址
            //判断大小   内存保留  内存读写
            if (*RegionSize == Xbundler_SteamApi_ImageSize && AllocationType == MEM_RESERVE && Protect == PAGE_READWRITE)
            {
                g_Xbundler_SteamApi_ImageBase = *BaseAddress;
            }
        }
    }
    return status;
}

#define SW_CreateInstanceFunc_RVA 0x3F130   //初始化环境
#define SW_UnknowFunc_RVA 0x3F3C0      //未知函数
#define SW_CrashFunc_RVA 0x613E0       //报错函数

/// <summary>
/// Syawase线程信息
/// </summary>
struct SWThreadEntry
{
    PVOID Function;
    PVOID Args;
};

//检查线程
NTSTATUS NTAPI CheckThreadHook(LPVOID threadParameter)
{
    SWThreadEntry* swEntry = (SWThreadEntry*)threadParameter;
    SIZE_T rva = (PBYTE)swEntry->Function - (PBYTE)g_encryptDll_ImageBase;

    if (rva == SW_CreateInstanceFunc_RVA)
    {
        ((void(*)(void*))g_CheckThreadFunc)(threadParameter);
    }

    //if (rva == SW_UnknowFunc_RVA)
    //{
    //    ((void(*)(void*))g_CheckThreadFunc)(threadParameter);
    //}

    return STATUS_SUCCESS;
}


NTSTATUS NTAPI HookNtCreateThreadEx
(
    PHANDLE ThreadHandle,
    ACCESS_MASK DesiredAccess,
    POBJECT_ATTRIBUTES ObjectAttributes,
    HANDLE ProcessHandle,
    PUSER_THREAD_START_ROUTINE StartRoutine,
    PVOID Argument,
    ULONG CreateFlags,
    SIZE_T ZeroBits,
    SIZE_T StackSize,
    SIZE_T MaximumStackSize,
    PPS_ATTRIBUTE_LIST AttributeList
)
{
    if ((SIZE_T)ProcessHandle == MAXSIZE_T)
    {
        if (!g_encryptDll_ImageBase)
        {
            PVOID encryptDllImagebase = GetModuleHandleW(L"HappyLiveShowup.dll");
            g_encryptDll_ImageBase = encryptDllImagebase;
            g_CheckThreadFunc = (PBYTE)encryptDllImagebase + EncryptDll_CheckThreadStartAddress_RVA;
        }

        //检测创建线程是否为检测函数
        if (g_CheckThreadFunc == StartRoutine) 
        {
            StartRoutine = CheckThreadHook;       //替换目标地址
        }
    }
    return g_orgNtCreateThreadEx(ThreadHandle, DesiredAccess, ObjectAttributes, ProcessHandle, StartRoutine, Argument, CreateFlags, ZeroBits, StackSize, MaximumStackSize, AttributeList);
}


void StartUp()
{
    HMODULE hNtdll = GetModuleHandleW(L"ntdll.dll");
    g_orgNtAllocateVirtualMemory = (tNtAllocateVirtualMemory)GetProcAddress(hNtdll, "NtAllocateVirtualMemory");
    g_orgNtCreateThreadEx = (tNtCreateThreadEx)GetProcAddress(hNtdll, "NtCreateThreadEx");

    InlineHook((PVOID*)&g_orgNtAllocateVirtualMemory, HookNtAllocateVirtualMemory);

    //InlineHook((PVOID*)&g_orgNtCreateThreadEx, HookNtCreateThreadEx);
}


BOOL APIENTRY DllMain(HMODULE hModule, DWORD ul_reason_for_call, LPVOID lpReserved)
{
    UNREFERENCED_PARAMETER(lpReserved);
    switch (ul_reason_for_call)
    {
        case DLL_PROCESS_ATTACH:
        {
        StartUp();
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


