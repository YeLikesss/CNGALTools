
#include <Windows.h>
#include <immintrin.h>
#include "Ntdll/ntdll.h"
#include "inlinestring.h"
#include "Common/path.h"
#include "Common/util.h"
#include "Common/directory.h"

#ifdef M_X64
#pragma comment(lib,"./Ntdll/ntdll_x64.lib")
#else
#pragma comment(lib,"./Ntdll/ntdll_x86.lib")
#endif

DWORD WINAPI Playload32(LPVOID param);

__declspec(naked)
DWORD WINAPI Playload32Start(LPVOID param)
{
    __asm
    {
        jmp Playload32
    }
}
/// <summary>
/// Shellcode代码 (需要Release编译)
/// </summary>
__declspec(noinline)
DWORD WINAPI Playload32(LPVOID param)
{
    InlineUnicodeString(dllname, KrkrFileDumper.dll);

    // 32位不支持 rdfsbase
    // TEB._NT_TIB.Self (0x18)
    TEB* teb32 = (TEB*)__readfsdword(0x18u);
    PEB* peb32 = teb32->ProcessEnvironmentBlock;
    PVOID exeBase = peb32->ImageBaseAddress;

    //Safengine壳模拟的 FF15 IAT Call LoadLibraryW
    PVOID const se_Emu_LoadLibraryW = (PVOID)((SIZE_T)exeBase + 0x980188u);
    const wchar_t* const name = dllname.String;
    __asm 
    {
        push name
        call se_Emu_LoadLibraryW
        nop     //SE壳FF15转E8 返回地址为 EIP + 6
    }
    return 0u;
}
__declspec(naked)
void Playload32End()
{
    __asm 
    {
        _emit 0x0F
        _emit 0x1F
        _emit 0x00
        ret
    }
}

struct ProcessWindow
{
    DWORD ProcessID;
    HWND MainWindow;
};
BOOL CALLBACK EnumWindowsCallback(HWND hwnd, LPARAM lParam)
{
    ProcessWindow* param = (ProcessWindow*)lParam;

    DWORD pid = 0u;
    ::GetWindowThreadProcessId(hwnd, &pid);
    if (pid == param->ProcessID && ::IsWindowVisible(hwnd))
    {
        param->MainWindow = hwnd;
        return FALSE;
    }
    return TRUE;
}
HWND GetMainWindowByProcessID(DWORD processID)
{
    ProcessWindow param{ processID, nullptr };
    ::EnumWindows(EnumWindowsCallback, (LPARAM)&param);
    return param.MainWindow;
}


int APIENTRY wWinMain(_In_ HINSTANCE hInstance, _In_opt_ HINSTANCE hPrevInstance, _In_ LPWSTR lpCmdLine, _In_ int nCmdShow)
{
    UNREFERENCED_PARAMETER(hPrevInstance);
    UNREFERENCED_PARAMETER(lpCmdLine);

    //::Sleep(10000);

    std::wstring loaderFullPath = Util::GetAppPathW();
    std::wstring loaderCurrentDirectory = Path::GetDirectoryName(loaderFullPath);
    std::wstring krkrExeFullPath;
    std::wstring krkrExeDirectory;
    {
        int argc = 0;
        LPWSTR* argv = ::CommandLineToArgvW(lpCmdLine, &argc);
        if (argc)
        {
            wchar_t* arg = argv[0];

            krkrExeFullPath = std::wstring(arg);
            krkrExeDirectory = Path::GetDirectoryName(krkrExeFullPath);
        }
        ::LocalFree(argv);
    }

    STARTUPINFOW si{ };
    si.cb = sizeof(si);
    PROCESS_INFORMATION pi{ };
    if (::CreateProcessW(krkrExeFullPath.c_str(), nullptr, nullptr, nullptr, FALSE, 0u, nullptr, krkrExeDirectory.c_str(), &si, &pi))
    {
        HWND krkrHwnd = nullptr;
        while (!krkrHwnd)
        {
            krkrHwnd = GetMainWindowByProcessID(pi.dwProcessId);
            ::Sleep(1000);
        }
        ::Sleep(1000);

        if (LPVOID remoteMem = ::VirtualAllocEx(pi.hProcess, nullptr, 0x1000u, MEM_RESERVE | MEM_COMMIT, PAGE_EXECUTE_READWRITE))
        {
#ifndef _DEBUG
            const SIZE_T payloadSize = (SIZE_T)Playload32End - (SIZE_T)Playload32Start;
            if (::WriteProcessMemory(pi.hProcess, remoteMem, Playload32Start, payloadSize, nullptr))
            {
                ::FlushInstructionCache(pi.hProcess, remoteMem, 0x1000u);
                if (HANDLE hRemoteThread = ::CreateRemoteThread(pi.hProcess, nullptr, 0u, (LPTHREAD_START_ROUTINE)remoteMem, nullptr, 0u, nullptr))
                {
                    ::WaitForSingleObject(hRemoteThread, INFINITE);
                    ::CloseHandle(hRemoteThread);
                }
            }
            else
            {
                ::MessageBoxW(nullptr, L"目标进程写入内存失败", L"错误", MB_OK);
            }
#else
            ::MessageBoxW(nullptr, L"加载器Debug编译模式不支持Shellcode注入", L"错误", MB_OK);
#endif
            ::VirtualFreeEx(pi.hProcess, remoteMem, 0u, MEM_RELEASE);
        }
        else
        {
            ::MessageBoxW(nullptr, L"目标进程分配内存失败", L"错误", MB_OK);
        }

        ::CloseHandle(pi.hThread);
        ::CloseHandle(pi.hProcess);
    }
    else
    {
        ::MessageBoxW(nullptr, L"创建进程失败", L"错误", MB_OK);
    }

    return 0;
}

