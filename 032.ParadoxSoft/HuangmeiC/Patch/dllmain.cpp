#include <Windows.h>
#include <immintrin.h>
#include "Ntdll/ntdll.h"
#include "Ntdll/WindowsInternal.h"
#include "Common/stringhelper.h"
#include "Common/path.h"
#include "Common/util.h"
#include "ExtendUtils.h"
#include <unordered_set>

#ifdef _M_X64
#pragma comment(lib,"./Ntdll/ntdll_x64.lib")
#else
#pragma comment(lib,"./Ntdll/ntdll_x86.lib")
#endif

using tNtCreateFile = decltype(&NtCreateFile);
using tNtQueryDirectoryFile = decltype(&NtQueryDirectoryFile);
using tNtQueryDirectoryFileEx = NTSTATUS (NTAPI*)(_In_ HANDLE FileHandle, _In_opt_ HANDLE Event, _In_opt_ PIO_APC_ROUTINE ApcRoutine, _In_opt_ PVOID ApcContext, _Out_ PIO_STATUS_BLOCK IoStatusBlock,_Out_ PVOID FileInformation,_In_ ULONG Length,_In_ FILE_INFORMATION_CLASS FileInformationClass,_In_ ULONG QueryFlags,_In_opt_ PUNICODE_STRING FileName);

static tNtCreateFile g_NtCreateFile = nullptr;
static tNtQueryDirectoryFile g_NtQueryDirectoryFile = nullptr;
static tNtQueryDirectoryFileEx g_NtQueryDirectoryFileEx = nullptr;

static std::wstring g_ExeFileName;
static std::wstring g_PatchFileName;
static std::wstring g_ExeDirectory;
static std::wstring g_LibDirectory;

NTSTATUS NTAPI Hook_NtCreateFile(PHANDLE FileHandle, ACCESS_MASK DesiredAccess, POBJECT_ATTRIBUTES ObjectAttributes, PIO_STATUS_BLOCK IoStatusBlock, PLARGE_INTEGER AllocationSize, ULONG FileAttributes, ULONG ShareAccess, ULONG CreateDisposition, ULONG CreateOptions, PVOID EaBuffer, ULONG EaLength);
NTSTATUS NTAPI Hook_NtQueryDirectoryFile(HANDLE FileHandle, HANDLE Event, PIO_APC_ROUTINE ApcRoutine, PVOID ApcContext, PIO_STATUS_BLOCK IoStatusBlock, PVOID FileInformation, ULONG Length, FILE_INFORMATION_CLASS FileInformationClass, BOOLEAN ReturnSingleEntry, PUNICODE_STRING FileName, BOOLEAN RestartScan);
NTSTATUS NTAPI Hook_NtQueryDirectoryFileEx(HANDLE FileHandle, HANDLE Event, PIO_APC_ROUTINE ApcRoutine, PVOID ApcContext, PIO_STATUS_BLOCK IoStatusBlock, PVOID FileInformation, ULONG Length, FILE_INFORMATION_CLASS FileInformationClass, ULONG QueryFlags, PUNICODE_STRING FileName);
NTSTATUS NTAPI Proc_PatchDirectoryFileData(HANDLE FileHandle, PIO_STATUS_BLOCK IoStatusBlock, PVOID FileInformation, FILE_INFORMATION_CLASS FileInformationClass);
bool Proc_RemoveLastEntry(PIO_STATUS_BLOCK IoStatusBlock, PVOID FileInformation, FILE_INFORMATION_CLASS FileInformationClass);

NTSTATUS NTAPI Hook_NtCreateFile(PHANDLE FileHandle, ACCESS_MASK DesiredAccess, POBJECT_ATTRIBUTES ObjectAttributes, PIO_STATUS_BLOCK IoStatusBlock, PLARGE_INTEGER AllocationSize, ULONG FileAttributes, ULONG ShareAccess, ULONG CreateDisposition, ULONG CreateOptions, PVOID EaBuffer, ULONG EaLength)
{
    if (CreateDisposition == FILE_OPEN)
    {
        if (PUNICODE_STRING oldPath = ObjectAttributes->ObjectName)
        {
            bool detectHashCheck = false;
            std::wstring filepath = std::wstring(oldPath->Buffer, oldPath->Length / 2u);
            {
                // 主程序.exe steam_api64.dll
                std::wstring s1 = StringHelper::ToLower(filepath);
                std::wstring s2 = StringHelper::ToLower(g_ExeFileName);
                if (StringHelper::EndsWith(s1, s2) || StringHelper::EndsWith(s1, L"steam_api64.dll"))
                {
                    //修改后缀
                    wchar_t* ext = &filepath.data()[filepath.length() - 4u];
                    ext[0] = L'.';
                    ext[1] = L'b';
                    ext[2] = L'a';
                    ext[3] = L'k';
                    detectHashCheck = true;
                }
            }

            if (detectHashCheck)
            {
                UNICODE_STRING newPath;
                ::RtlInitUnicodeString(&newPath, filepath.data());

                ObjectAttributes->ObjectName = &newPath;
                NTSTATUS status = g_NtCreateFile(FileHandle, DesiredAccess, ObjectAttributes, IoStatusBlock, AllocationSize, FileAttributes, ShareAccess, CreateDisposition, CreateOptions, EaBuffer, EaLength);
                ObjectAttributes->ObjectName = oldPath;
                
                return status;
            }
        }
    }
    return g_NtCreateFile(FileHandle, DesiredAccess, ObjectAttributes, IoStatusBlock, AllocationSize, FileAttributes, ShareAccess, CreateDisposition, CreateOptions, EaBuffer, EaLength);
}

NTSTATUS NTAPI Hook_NtQueryDirectoryFile(HANDLE FileHandle, HANDLE Event, PIO_APC_ROUTINE ApcRoutine, PVOID ApcContext, PIO_STATUS_BLOCK IoStatusBlock, PVOID FileInformation, ULONG Length, FILE_INFORMATION_CLASS FileInformationClass, BOOLEAN ReturnSingleEntry, PUNICODE_STRING FileName, BOOLEAN RestartScan)
{
    NTSTATUS status = g_NtQueryDirectoryFile(FileHandle, Event, ApcRoutine, ApcContext, IoStatusBlock, FileInformation, Length, FileInformationClass, ReturnSingleEntry, FileName, RestartScan);

    //Windows Internal 缓存溢出移除最后一项
    if (status == STATUS_BUFFER_OVERFLOW && !ReturnSingleEntry)
    {
        if (Proc_RemoveLastEntry(IoStatusBlock, FileInformation, FileInformationClass))
        {
            status = STATUS_SUCCESS;
        }
    }

    if (!NT_SUCCESS(status))
    {
        return status;
    }
    return Proc_PatchDirectoryFileData(FileHandle, IoStatusBlock, FileInformation, FileInformationClass);
}
NTSTATUS NTAPI Hook_NtQueryDirectoryFileEx(HANDLE FileHandle, HANDLE Event, PIO_APC_ROUTINE ApcRoutine, PVOID ApcContext, PIO_STATUS_BLOCK IoStatusBlock, PVOID FileInformation, ULONG Length, FILE_INFORMATION_CLASS FileInformationClass, ULONG QueryFlags, PUNICODE_STRING FileName)
{
    NTSTATUS status = g_NtQueryDirectoryFileEx(FileHandle, Event, ApcRoutine, ApcContext, IoStatusBlock, FileInformation, Length, FileInformationClass, QueryFlags, FileName);
    
    //Windows Internal 缓存溢出移除最后一项
    if (status == STATUS_BUFFER_OVERFLOW && !(QueryFlags & WindowsInternal::SL_QUERY_DIRECTORY_MASK::SL_RETURN_SINGLE_ENTRY))
    {
        if (Proc_RemoveLastEntry(IoStatusBlock, FileInformation, FileInformationClass))
        {
            status = STATUS_SUCCESS;
        }
    }

    if (!NT_SUCCESS(status))
    {
        return status;
    }
    return Proc_PatchDirectoryFileData(FileHandle, IoStatusBlock, FileInformation, FileInformationClass);
}
NTSTATUS NTAPI Proc_PatchDirectoryFileData(HANDLE FileHandle, PIO_STATUS_BLOCK IoStatusBlock, PVOID FileInformation, FILE_INFORMATION_CLASS FileInformationClass)
{
    // FindFirstFile / FindNextFile API
    if (FileInformationClass == FILE_INFORMATION_CLASS::FileFullDirectoryInformation || FileInformationClass == FILE_INFORMATION_CLASS::FileBothDirectoryInformation)
    {
        std::wstring path;
        {
            NTSTATUS status;
            IO_STATUS_BLOCK ioStatus;
            FILE_STANDARD_INFORMATION standInfo{};
            status = ::NtQueryInformationFile(FileHandle, &ioStatus, &standInfo, sizeof(standInfo), FILE_INFORMATION_CLASS::FileStandardInformation);
            if (NT_SUCCESS(status) && standInfo.Directory)
            {
                DWORD len = ::GetFinalPathNameByHandleW(FileHandle, nullptr, 0u, FILE_NAME_NORMALIZED);
                if (len > 4u)
                {
                    wchar_t* temp = new wchar_t[len];
                    ::GetFinalPathNameByHandleW(FileHandle, temp, len, FILE_NAME_NORMALIZED);
                    path.reserve(len);
                    path.append(&temp[4u]);
                    delete[] temp;
                }
            }
        }

        std::unordered_set<std::wstring> filters;
        if (!path.empty())
        {
            if (path == g_ExeDirectory)
            {
                // 过滤补丁dll 重定向文件
                filters.insert(Path::ChangeExtension(g_ExeFileName, L".bak"));
                filters.insert(g_PatchFileName);
            }
            else if (path == g_LibDirectory)
            {
                // 过滤SteamEmu配置 重定向文件
                filters.emplace(L"steam_api64.bak");
                filters.emplace(L"steam_settings");
            }
        }

        //过滤文件
        if (!filters.empty())
        {
            if (ULONG_PTR maxLen = IoStatusBlock->Information)
            {
                {
                    unsigned __int8* data = (unsigned __int8*)FileInformation;
                    ULONG nextOffset = 0UL;
                    ULONG fileNameLen = 0UL;
                    WCHAR* filename = nullptr;

                    ULONG_PTR prevPos = 0UL, curPos = 0UL;
                    do
                    {
                        //获取文件信息
                        switch (FileInformationClass)
                        {
                            case FILE_INFORMATION_CLASS::FileFullDirectoryInformation:
                            {
                                WindowsInternal::FILE_FULL_DIR_INFORMATION* info = (WindowsInternal::FILE_FULL_DIR_INFORMATION*)&data[curPos];
                                nextOffset = info->NextEntryOffset;
                                fileNameLen = info->FileNameLength / sizeof(WCHAR);
                                filename = info->FileName;
                                break;
                            }
                            case FILE_INFORMATION_CLASS::FileBothDirectoryInformation:
                            {
                                WindowsInternal::FILE_BOTH_DIR_INFORMATION* info = (WindowsInternal::FILE_BOTH_DIR_INFORMATION*)&data[curPos];
                                nextOffset = info->NextEntryOffset;
                                fileNameLen = info->FileNameLength / sizeof(WCHAR);
                                filename = info->FileName;
                                break;
                            }
                        }

                        std::wstring name = std::wstring(filename, fileNameLen);
                        if (filters.contains(name))
                        {
                            if (nextOffset)
                            {
                                //中间项  前移覆盖
                                ExtendUtils::Memory::Copy(&data[curPos], &data[curPos + nextOffset], maxLen - (curPos + nextOffset));
                                maxLen -= nextOffset;
                            }
                            else
                            {
                                //最后一项
                                maxLen = curPos;
                                
                                //上一项清零
                                switch (FileInformationClass)
                                {
                                    case FILE_INFORMATION_CLASS::FileFullDirectoryInformation:
                                    {
                                        WindowsInternal::FILE_FULL_DIR_INFORMATION* info = (WindowsInternal::FILE_FULL_DIR_INFORMATION*)&data[prevPos];
                                        info->NextEntryOffset = 0UL;
                                        break;
                                    }
                                    case FILE_INFORMATION_CLASS::FileBothDirectoryInformation:
                                    {
                                        WindowsInternal::FILE_BOTH_DIR_INFORMATION* info = (WindowsInternal::FILE_BOTH_DIR_INFORMATION*)&data[prevPos];
                                        info->NextEntryOffset = 0UL;
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            //查找下一项
                            prevPos = curPos;
                            curPos += nextOffset;
                        }
                    } 
                    while (nextOffset);
                }
                
                //返回值长度修正
                if (maxLen)
                {
                    IoStatusBlock->Information = maxLen;
                    return STATUS_SUCCESS;
                }
                else
                {
                    IoStatusBlock->Status = STATUS_NO_MORE_FILES;
                    IoStatusBlock->Information = 0UL;
                    return STATUS_NO_MORE_FILES;
                }
            }
        }
    }
    return STATUS_SUCCESS;
}
bool Proc_RemoveLastEntry(PIO_STATUS_BLOCK IoStatusBlock, PVOID FileInformation, FILE_INFORMATION_CLASS FileInformationClass)
{
    if (ULONG_PTR maxLen = IoStatusBlock->Information)
    {
        //含有NextOffsetEntry字段
        //FileDirectoryInformation --- FILE_DIRECTORY_INFORMATION
        //FileFullDirectoryInformation --- FILE_FULL_DIR_INFORMATION
        //FileBothDirectoryInformation --- FILE_BOTH_DIR_INFORMATION
        if (FileInformationClass < 4u)
        {
            unsigned __int8* prev = nullptr;
            unsigned __int8* curr = (unsigned __int8*)FileInformation;

            while (ULONG nextOffset = *(ULONG*)curr)
            {
                prev = curr;
                curr += nextOffset;
            }
            if (prev)
            {
                *(ULONG*)prev = 0UL;
            }

            return true;
        }
    }
    return false;
}

/// <summary>
/// 初始化
/// </summary>
void Initialize(HMODULE dllModule)
{
    std::wstring exePath = Util::GetAppPathW();
    std::wstring exeDir = Path::GetDirectoryName(exePath);
    std::wstring libDir = Path::Combine(exeDir, L"lib\\py3-windows-x86_64");
    std::wstring steamPath = Path::Combine(libDir, L"steam_api64.dll");

    ::LoadLibraryW(steamPath.c_str());

    g_ExeFileName = Path::GetFileName(exePath);
    g_PatchFileName = Path::GetFileName(Util::GetModulePathW(dllModule));
    g_ExeDirectory = std::move(exeDir);
    g_LibDirectory = std::move(libDir);
    {
        if (HMODULE hNtdll = ::GetModuleHandleW(L"ntdll.dll"))
        {
            g_NtQueryDirectoryFile = (tNtQueryDirectoryFile)::GetProcAddress(hNtdll, "NtQueryDirectoryFile");
            g_NtQueryDirectoryFileEx = (tNtQueryDirectoryFileEx)::GetProcAddress(hNtdll, "NtQueryDirectoryFileEx");
            g_NtCreateFile = (tNtCreateFile)::GetProcAddress(hNtdll, "NtCreateFile");

            if (g_NtQueryDirectoryFile)
            {
                ExtendUtils::InlineHook::Hook(g_NtQueryDirectoryFile, Hook_NtQueryDirectoryFile);
            }
            if (g_NtQueryDirectoryFileEx)
            {
                ExtendUtils::InlineHook::Hook(g_NtQueryDirectoryFileEx, Hook_NtQueryDirectoryFileEx);
            }
            if (g_NtCreateFile)
            {
                ExtendUtils::InlineHook::Hook(g_NtCreateFile, Hook_NtCreateFile);
            }
        }
    }
}


BOOL APIENTRY DllMain(HMODULE hModule, DWORD ul_reason_for_call, LPVOID lpReserved)
{
    UNREFERENCED_PARAMETER(lpReserved);
    switch (ul_reason_for_call)
    {
        case DLL_PROCESS_ATTACH:
        {
            Initialize(hModule);
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

