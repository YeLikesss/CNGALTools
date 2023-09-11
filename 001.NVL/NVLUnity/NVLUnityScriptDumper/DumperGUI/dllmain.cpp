#include <Windows.h>
#include "resource.h"

#include <Shlwapi.h>
#pragma comment(lib,"shlwapi.lib")

#include<vector>
#include<string>

typedef void (WINAPI* tDumpFunc)(const wchar_t* fullpath);
typedef void (WINAPI* tDumperLink)();

static PVOID g_DumperCoreImageBase = NULL;
static tDumpFunc g_DumpScriptFunc = NULL;
static tDumperLink g_Link = NULL;
static tDumperLink g_Unlink = NULL;


/// <summary>
/// 主窗体消息循环
/// </summary>
/// <param name="hwnd">窗口句柄</param>
/// <param name="msg">消息</param>
/// <param name="wParam"></param>
/// <param name="lParam"></param>
INT_PTR CALLBACK DumperDialogWindProc(HWND hwnd, UINT msg, WPARAM wParam, LPARAM lParam)
{
    switch (msg)
    {
        case WM_DROPFILES:
        {
            HDROP hDrop = (HDROP)wParam;
            UINT count = DragQueryFileW(hDrop, 0xFFFFFFFF, NULL, 0);
            wchar_t fullName[1024];

            std::vector<std::wstring>* fullPaths = new std::vector<std::wstring>();

            for (UINT i = 0; i < count; ++i)
            {
                if (UINT strLen = DragQueryFileW(hDrop, i, fullName, 1024))
                {
                    DWORD attr = GetFileAttributesW(fullName);
                    if (attr != INVALID_FILE_ATTRIBUTES && (attr & FILE_ATTRIBUTE_ARCHIVE) == FILE_ATTRIBUTE_ARCHIVE)
                    {
                        g_DumpScriptFunc(fullName);
                    }
                }
            }

            DragFinish(hDrop);

            return TRUE;
        }
        case WM_CLOSE:
        {
            DestroyWindow(hwnd);
            return TRUE;
        }
        case WM_DESTROY:
        {
            PostQuitMessage(0);
            return TRUE;
        }
    }
    return FALSE;
}


/// <summary>
/// 窗口代码
/// </summary>
/// <param name="hInstance">模块基地址</param>
DWORD WINAPI WinDumperEntry(LPVOID hInstance)
{
    g_Link();

    HWND hwnd = CreateDialogParamW((HINSTANCE)hInstance, MAKEINTRESOURCEW(IDD_MainForm), NULL, DumperDialogWindProc, 0);
    ShowWindow(hwnd, SW_NORMAL);

    DWORD result = 0;

    MSG msg{ 0 };
    while (BOOL ret = GetMessageW(&msg, NULL, 0, 0))
    {
        if (ret == -1)
        {
            result = -1;
            break;
        }
        else
        {
            TranslateMessage(&msg);
            DispatchMessageW(&msg);
        }
    }

    g_Unlink();
    return result;;
}


BOOL APIENTRY DllMain(HMODULE hModule, DWORD  ul_reason_for_call, LPVOID lpReserved)
{
    switch (ul_reason_for_call)
    {
        case DLL_PROCESS_ATTACH:
        {
            wchar_t moduleFullPath[1024];
            DWORD strLen = GetModuleFileNameW(hModule, moduleFullPath, 1024);
            wchar_t* dllName = PathFindFileNameW(moduleFullPath);
            //ScriptDumper.dll
            {
                dllName[0] = L'S';
                dllName[1] = L'c';
                dllName[2] = L'r';
                dllName[3] = L'i';
                dllName[4] = L'p';
                dllName[5] = L't';
                dllName[6] = L'D';
                dllName[7] = L'u';
                dllName[8] = L'm';
                dllName[9] = L'p';
                dllName[10] = L'e';
                dllName[11] = L'r';
                dllName[12] = L'.';
                dllName[13] = L'd';
                dllName[14] = L'l';
                dllName[15] = L'l';
                dllName[16] = L'\0';
            }

            if (HMODULE coreBase = LoadLibraryW(moduleFullPath)) 
            {
                g_DumperCoreImageBase = coreBase;
                g_DumpScriptFunc = (tDumpFunc)GetProcAddress(coreBase, "DumpScript");
                g_Link = (tDumperLink)GetProcAddress(coreBase, "Initialize");
                g_Unlink = (tDumperLink)GetProcAddress(coreBase, "UnInitialize");

                if (HANDLE hThread = CreateThread(NULL, 0, WinDumperEntry, hModule, 0, NULL)) 
                {
                    CloseHandle(hThread);
                }
            }
            else
            {
                MessageBoxW(NULL, L"ScriptDumper.dll加载失败", L"错误", MB_OK);
            }
        }
        case DLL_THREAD_ATTACH: 
        { 
            break;
        }
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

extern "C" __declspec(dllexport) void Dummy() {}