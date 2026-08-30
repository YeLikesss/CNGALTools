
#include <Windows.h>
#include "detours.h"
#include "ntdll.h"
#include "file.h"
#include "log.h"
#include "path.h"
#include "util.h"
#include <regex>

extern "C" __declspec(dllexport) void Dummy() { Sleep(30000); }


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

#define EnableDumper 0          //设置为1启用文件Dump

#define MainExe_CreateWindowExW_IAT_RVA 0x24D34C        //主程序CreateWindowExW函数 IAT RVA
#define MainExe_SetWindowTextW_IAT_RVA 0x24D384         //主程序SetWindowTextW函数 IAT RVA

#define Xbundler_SteamApi_ImageSize 0x42000     //壳VFS内steam_api.dll模块大小
#define Xbundler_SteamApi_Kernel32ImportName_RVA 0x3B80A     //壳VFS内steam_api.dll导入kernel32.dll的名称RVA
#define Xbundler_SteamApi_Advapi32ImportName_RVA 0x3B84A     //壳VFS内steam_api.dll导入Advapi32.dll的名称RVA

#define tNtAllocateVirtualMemory decltype(&NtAllocateVirtualMemory)
#define tNtCreateThreadEx decltype(&NtCreateThreadEx)

static std::wstring g_AppDirectory;         //游戏文件夹路径
static Log::Logger g_Logger;
static std::vector<std::wstring> g_regexExcludeRules;

static tNtAllocateVirtualMemory g_orgNtAllocateVirtualMemory = NULL;    //原NtAllocateVirtualMemory地址
static tNtCreateThreadEx g_orgNtCreateThreadEx = NULL;      //原NtCreateThreadEx地址

static PVOID g_Xbundler_SteamApi_ImageBase = NULL;      //壳VFS内steam_api.dll基地址

BOOL WINAPI ExtractFile(const wchar_t* fileName);

static PVOID g_MainExeFopenFunc = NULL;
static PVOID g_MainExeWFopenFunc = NULL;
static PVOID g_MainExeFSeekFunc = NULL;
static PVOID g_MainExeFTellFunc = NULL;
static PVOID g_MainExeFReadFunc = NULL;
static PVOID g_MainExeFCloseFunc = NULL;

__declspec(noinline)
void* __stdcall File_OpenA(const char* fileName, const char* mode)
{
    return ((void* (__cdecl*)(const char*, const char*))g_MainExeFopenFunc)(fileName, mode);
}

__declspec(noinline)
void* __stdcall File_OpenW(const wchar_t* fileName, const wchar_t* mode) 
{
    return ((void* (__cdecl*)(const wchar_t*, const wchar_t*))g_MainExeWFopenFunc)(fileName, mode);
}

__declspec(noinline)
int __stdcall File_Seek(void* stream, long offset, int origin) 
{
    return ((int (__cdecl*)(void*, long, int))g_MainExeFSeekFunc)(stream, offset, origin);
}

__declspec(noinline)
long __stdcall File_Position(void* stream) 
{
    return ((long (__cdecl*)(void*))g_MainExeFTellFunc)(stream);
}

__declspec(noinline)
size_t __stdcall File_Read(void* buffer, size_t size, size_t count, void* stream)
{
    return ((size_t(__cdecl*)(void*, size_t, size_t, void*))g_MainExeFReadFunc)(buffer, size, count, stream);
}

__declspec(noinline)
int __stdcall File_Close(void* stream)
{
    return ((int (__cdecl*)(void*))g_MainExeFCloseFunc)(stream);
}


long __stdcall File_GetSize(void* stream)
{
    File_Seek(stream, 0, SEEK_END);
    long size = File_Position(stream);
    File_Seek(stream, 0, SEEK_SET);
    return size;
}

//创建文件夹
void __stdcall FullCreateDirectoryW(const std::wstring& dirPath)
{
    //判断文件夹是否存在
    DWORD fileAttr = GetFileAttributesW(dirPath.c_str());
    if ((int)fileAttr == -1 || (fileAttr & FILE_ATTRIBUTE_DIRECTORY) == 0)
    {
        //逐级创建文件夹
        if (!CreateDirectoryW(dirPath.c_str(), NULL))
        {
            FullCreateDirectoryW(Path::GetDirectoryName(dirPath));
            CreateDirectoryW(dirPath.c_str(), NULL);
        }
    }
}

BOOL __stdcall CheckFileExist(const std::wstring& filePath)
{
    //判断文件是否存在
    DWORD fileAttr = GetFileAttributesW(filePath.c_str());
    if ((int)fileAttr == -1 || (fileAttr & FILE_ATTRIBUTE_DIRECTORY))
    {
        return FALSE;
    }
    return TRUE;
}

void __stdcall NormalizeWindowsPath(std::wstring& path)
{
    for (size_t i = 0; i < path.length(); i++)
    {
        if (path[i] == L'/')
        {
            path[i] = L'\\';
        }
    }
}

//检查游戏资源封包路径
__declspec(noinline)
BOOL __stdcall MatchPath(const wchar_t* path, std::vector<std::wstring>& regex)
{
    BOOL match = FALSE;

    for (auto& rule : regex)
    {
        std::wregex expr(rule, std::regex_constants::icase);
        if (std::regex_match(path, expr))
        {
            match = TRUE;
            break;
        }
    }
    return match;
}

__declspec(noinline)
void* __cdecl HookWFopen(const wchar_t* fileName, const wchar_t* mode)
{
    void* hFile = File_OpenW(fileName, mode);
    if (hFile) 
    {
        //rb模式
        if (!lstrcmpiW(mode, L"rb") && !MatchPath(fileName, g_regexExcludeRules))
        {
            std::wstring outPath = g_AppDirectory + L"\\Dumper_Output\\" + fileName;
            NormalizeWindowsPath(outPath);
            if (!CheckFileExist(outPath))
            {
                FullCreateDirectoryW(Path::GetDirectoryName(outPath));

                HANDLE hOutFile = CreateFileW(outPath.c_str(), GENERIC_READ | GENERIC_WRITE, FILE_SHARE_READ, NULL, CREATE_ALWAYS, FILE_ATTRIBUTE_NORMAL, NULL);
                if (hOutFile != INVALID_HANDLE_VALUE)
                {
                    long size = File_GetSize(hFile);
                    {
                        LARGE_INTEGER pos{ 0 };
                        SetFilePointerEx(hOutFile, pos, NULL, SEEK_SET);
                    }
                    
                    byte buffer[64];
                    int bufferSize = sizeof(buffer);

                    while (size > 0)
                    {
                        size_t readSize = File_Read(buffer, 1, bufferSize, hFile);
                        DWORD writeBytes;
                        WriteFile(hOutFile, buffer, readSize, &writeBytes, NULL);

                        size -= readSize;
                    }

                    File_Seek(hFile, 0, SEEK_SET);

                    FlushFileBuffers(hOutFile);
                    CloseHandle(hOutFile);

                    g_Logger.WriteLine(L"Dump Success  %s", fileName);
                }
                else
                {
                    g_Logger.WriteLine(L"CreateFileError  %s", fileName);
                }
            }
            else
            {
                g_Logger.WriteLine(L"File Is Dumped  %s", fileName);
            }
        }
        else
        {
            g_Logger.WriteLine(L"Exclude  %s", fileName);
        }
    }
    return hFile;
}

void InitializeFileStream() 
{
    PBYTE mainBase = (PBYTE)GetModuleHandleW(NULL);
    g_MainExeFopenFunc = mainBase + 0x21367B;
    g_MainExeWFopenFunc = mainBase + 0x213664;
    g_MainExeFSeekFunc = mainBase + 0x20C233;
    g_MainExeFTellFunc = mainBase + 0x214998;
    g_MainExeFReadFunc = mainBase + 0x208B57;
    g_MainExeFCloseFunc = mainBase + 0x20816D;
}


__declspec(noinline)
BOOL WINAPI ExtractFile(const wchar_t* fileName) 
{
    BOOL status = FALSE;

    std::wstring outPath = g_AppDirectory + L"\\Extract_Output\\" + fileName;
    NormalizeWindowsPath(outPath);

    FullCreateDirectoryW(Path::GetDirectoryName(outPath));

    void* hFile = File_OpenW(fileName, L"rb");
    if (hFile)
    {
        HANDLE hOutFile = CreateFileW(outPath.c_str(), GENERIC_READ | GENERIC_WRITE, FILE_SHARE_READ, NULL, CREATE_ALWAYS, FILE_ATTRIBUTE_NORMAL, NULL);
        if (hOutFile != INVALID_HANDLE_VALUE)
        {
            long size = File_GetSize(hFile);
            {
                LARGE_INTEGER pos{ 0 };
                SetFilePointerEx(hOutFile, pos, NULL, SEEK_SET);
            }

            byte buffer[64];

            while (size > 0)
            {
                size_t readSize = File_Read(buffer, 1, sizeof(buffer), hFile);
                DWORD writeBytes;
                WriteFile(hOutFile, buffer, readSize, &writeBytes, NULL);

                size -= readSize;
            }

            FlushFileBuffers(hOutFile);
            CloseHandle(hOutFile);
            File_Seek(hFile, 0, SEEK_SET);

            status = TRUE;
            g_Logger.WriteLine(L"Dump Success  %s", fileName);
        }
        else
        {
            g_Logger.WriteLine(L"CreateFileError  %s", fileName);
        }
            
        File_Close(hFile);
    }
    else
    {
        g_Logger.WriteLine(L"File Not Found  %s", fileName);
    }
    return status;
}



NTSTATUS NTAPI HookNtAllocateVirtualMemory(HANDLE ProcessHandle, PVOID* BaseAddress, ULONG_PTR ZeroBits, PSIZE_T RegionSize, ULONG AllocationType, ULONG Protect);
NTSTATUS NTAPI HookNtCreateThreadEx(PHANDLE ThreadHandle, ACCESS_MASK DesiredAccess, POBJECT_ATTRIBUTES ObjectAttributes, HANDLE ProcessHandle, PUSER_THREAD_START_ROUTINE StartRoutine, PVOID Argument, ULONG CreateFlags, SIZE_T ZeroBits, SIZE_T StackSize, SIZE_T MaximumStackSize, PPS_ATTRIBUTE_LIST AttributeList);
BOOL NTAPI BypassThreadDetector(SIZE_T threadEPRva);

NTSTATUS NTAPI HookNtAllocateVirtualMemory(HANDLE ProcessHandle, PVOID* BaseAddress, ULONG_PTR ZeroBits, PSIZE_T RegionSize, ULONG AllocationType, ULONG Protect) 
{
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
            //判断大小   内存保留  保护类型
            if (*RegionSize == Xbundler_SteamApi_ImageSize && AllocationType == MEM_RESERVE && Protect == PAGE_READWRITE)
            {
                g_Xbundler_SteamApi_ImageBase = *BaseAddress;
            }
        }
    }
    return status;
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
        if (PVOID encryptDllImagebase = GetModuleHandleW(L"hamidashi.dll"))
        {
            //检测创建线程是否为 定时检测函数
            if (BypassThreadDetector((SIZE_T)StartRoutine - (SIZE_T)encryptDllImagebase))
            {
                StartRoutine = (PUSER_THREAD_START_ROUTINE)Dummy;       //替换目标地址
            }
        }
    }
    return g_orgNtCreateThreadEx(ThreadHandle, DesiredAccess, ObjectAttributes, ProcessHandle, StartRoutine, Argument, CreateFlags, ZeroBits, StackSize, MaximumStackSize, AttributeList);
}

BOOL NTAPI BypassThreadDetector(SIZE_T threadEPRva)
{
    SIZE_T encDllOrgCreateWindowExW_RVA = 0;    //加密DLL存放原CreateWindowExW函数地址的RVA
    SIZE_T encDllOrgSetWindowTextW_RVA = 0;     //加密DLL存放原SetWindowTextW函数地址的RVA

    if (0x26903 == threadEPRva)   //V102  Steam 2022.10.1
    {
        g_Logger.WriteLine(L"Hamidashi V102 Hit");

        encDllOrgCreateWindowExW_RVA = 0x42C28;
        encDllOrgSetWindowTextW_RVA = 0x42C38;
    }
    else if(0x27A03 == threadEPRva)    //V105  Steam 2022.10.11
    {
        g_Logger.WriteLine(L"Hamidashi V105 Hit");

        encDllOrgCreateWindowExW_RVA = 0x43C70;
        encDllOrgSetWindowTextW_RVA = 0x43C80;
    }
    else
    {
        return FALSE;
    }

    PVOID mainExeImageBase = GetModuleHandleW(NULL);
    PVOID encryptDllImagebase = GetModuleHandleW(L"hamidashi.dll");
    DWORD oldProtect = 0;

    if (encDllOrgCreateWindowExW_RVA) 
    {
    }

    //还原SetWindowTextW Hook
    if (encDllOrgSetWindowTextW_RVA)
    {
        PVOID mainExe_SetWindowTextW_IAT_Va = (PVOID)((SIZE_T)mainExeImageBase + MainExe_SetWindowTextW_IAT_RVA);

        VirtualProtect(mainExe_SetWindowTextW_IAT_Va, 4, PAGE_READWRITE, &oldProtect);
        *(SIZE_T*)mainExe_SetWindowTextW_IAT_Va = *(SIZE_T*)((SIZE_T)encryptDllImagebase + encDllOrgSetWindowTextW_RVA);
        VirtualProtect(mainExe_SetWindowTextW_IAT_Va, 4, oldProtect, &oldProtect);
    }

    return TRUE;
}


void StartUp() 
{
    HMODULE hNtdll = GetModuleHandleW(L"ntdll.dll");
    g_orgNtAllocateVirtualMemory = (tNtAllocateVirtualMemory)GetProcAddress(hNtdll, "NtAllocateVirtualMemory");
    g_orgNtCreateThreadEx = (tNtCreateThreadEx)GetProcAddress(hNtdll, "NtCreateThreadEx");

    InlineHook((PVOID*)&g_orgNtAllocateVirtualMemory, HookNtAllocateVirtualMemory);

    //初始化log与各种路径
    std::wstring appDir = Util::GetAppDirectoryW();
    std::wstring logPath = appDir + L"\\HamidashiCreative.log";

    File::Delete(logPath);
    g_Logger.Open(logPath.c_str());
    g_Logger.WriteLine(L"Game Directory Path  %s", appDir.c_str());

    g_AppDirectory = std::move(appDir);

    //初始化文件读写
    InitializeFileStream();

    //Dumper Hook
#if EnableDumper
    InlineHook(&g_MainExeWFopenFunc, HookWFopen);
    g_Logger.WriteLine(L"Dumper Enable");
#else
    g_Logger.WriteLine(L"Dumper Disable");
#endif 

    //dump筛选器
    g_regexExcludeRules.clear();
    g_regexExcludeRules.push_back(L"^[a-zA-Z]:([\\\\/].+)+$");
    g_regexExcludeRules.push_back(L"^saveg\\.dat$");
    g_regexExcludeRules.push_back(L"^system\\.dat$");
    g_regexExcludeRules.push_back(L"^save[0-9]{4}\\.dat$");

}

void ShutDown() 
{
    g_Logger.Close();
    UnInlineHook(&g_MainExeWFopenFunc, HookWFopen);
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
            ShutDown();
            break;
        }
    }
    return TRUE;
}

