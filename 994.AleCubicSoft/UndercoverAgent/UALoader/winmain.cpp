#include "path.h"
#include "util.h"
#include "file.h"
#include "detours.h"

#pragma comment(linker, "/MERGE:\".detourd=.data\"")
#pragma comment(linker, "/MERGE:\".detourc=.rdata\"")

#ifdef _UNICODE
#if defined _M_IX86
#pragma comment(linker,"/manifestdependency:\"type='win32' name='Microsoft.Windows.Common-Controls' version='6.0.0.0' processorArchitecture='x86' publicKeyToken='6595b64144ccf1df' language='*'\"")
#elif defined _M_X64
#pragma comment(linker,"/manifestdependency:\"type='win32' name='Microsoft.Windows.Common-Controls' version='6.0.0.0' processorArchitecture='amd64' publicKeyToken='6595b64144ccf1df' language='*'\"")
#else
#pragma comment(linker,"/manifestdependency:\"type='win32' name='Microsoft.Windows.Common-Controls' version='6.0.0.0' processorArchitecture='*' publicKeyToken='6595b64144ccf1df' language='*'\"")
#endif
#endif

int APIENTRY wWinMain(HINSTANCE hInstance, HINSTANCE hPrevInstance, LPWSTR lpCmdLine, int nCmdShow)
{
    UNREFERENCED_PARAMETER(hPrevInstance);
    UNREFERENCED_PARAMETER(lpCmdLine);

    std::wstring currentDirectory = Util::GetAppDirectoryW();
    std::wstring targetExePath = Path::Combine(currentDirectory, L"UndercoverAgent.exe");
    std::string dllPath = Path::Combine(Util::GetAppDirectoryA(), "UAPatch.dll");

    if (Path::Exists(targetExePath))
    {
        if (Path::Exists(dllPath))
        {
            DWORD createFlags = 0u;

#ifdef _DEBUG
            if (::MessageBoxW(nullptr, L"是否以挂起模式启动进程[CREATE_SUSPENDED]\r\n使用调试器附加并恢复线程", L"调试", MB_YESNO) == IDYES)
            {
                createFlags = CREATE_SUSPENDED;
            }
#endif
            STARTUPINFOW si{ };
            si.cb = sizeof(si);
            PROCESS_INFORMATION pi{ };

            if (DetourCreateProcessWithDllW(targetExePath.c_str(), nullptr, nullptr, nullptr, FALSE, createFlags, nullptr, currentDirectory.c_str(), &si, &pi, dllPath.c_str(), nullptr))
            {
                ::CloseHandle(pi.hThread);
                ::CloseHandle(pi.hProcess);
            }
            else
            {
                ::MessageBoxW(nullptr, L"创建进程错误", L"错误", MB_OK);
            }
        }
        else
        {
            ::MessageBoxW(nullptr, L"UAPatch.dll不存在", L"错误", MB_OK);
        }
    }
    else
    {
        ::MessageBoxW(nullptr, L"游戏主程序UndercoverAgent.exe不存在", L"错误", MB_OK);
    }

    return 0;
}


