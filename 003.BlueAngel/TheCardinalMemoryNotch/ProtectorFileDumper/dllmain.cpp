#include "directory.h"
#include "encoding.h"
#include "file.h"
#include "path.h"
#include "stringhelper.h"
#include "util.h"

using tFindFirstFileA = decltype(&::FindFirstFileA);
using tFindNextFileA = decltype(&::FindNextFileA);
using tFindClose = decltype(&::FindClose);

using tCreateFileA = decltype(&::CreateFileA);
using tGetFileSize = decltype(&::GetFileSize);
using tReadFile = decltype(&::ReadFile);
using tSetFilePointer = decltype(&::SetFilePointer);
using tCloseHandle = decltype(&::CloseHandle);

static tFindFirstFileA g_VFS_FindFirstFileA = nullptr;
static tFindNextFileA g_VFS_FindNextFileA = nullptr;
static tFindClose g_VFS_FindClose = nullptr;

static tCreateFileA g_VFS_CreateFileA = nullptr;
static tGetFileSize g_VFS_GetFileSize = nullptr;
static tReadFile g_VFS_ReadFile = nullptr;
static tSetFilePointer g_VFS_SetFilePointer = nullptr;
static tCloseHandle g_VFS_CloseHandle = nullptr;

/// <summary>
/// 枚举文件
/// </summary>
/// <param name="subFiles">返回值: 当前目录所有文件路径</param>
/// <param name="directory">参数: 目标文件夹路径</param>
void EnumerateFilesA(std::vector<std::string>& subFiles, const std::string& directory)
{
    WIN32_FIND_DATAA findFileData { };
    HANDLE hFindFile;

    std::string scanPath = Path::Combine(directory, "*");

    hFindFile = g_VFS_FindFirstFileA(scanPath.c_str(), &findFileData);
    if (hFindFile == INVALID_HANDLE_VALUE)
    {
        return;
    }

    do
    {
        //过滤本级与上级目录
        if (!::lstrcmpA(findFileData.cFileName, ".") || !::lstrcmpA(findFileData.cFileName, ".."))
        {
            continue;
        }

        //获取全路径
        std::string fullName = Path::Combine(directory, findFileData.cFileName);

        //如果是文件夹则递归遍历函数
        if (findFileData.dwFileAttributes & FILE_ATTRIBUTE_DIRECTORY)
        {
            EnumerateFilesA(subFiles, fullName);
        }
        else
        {
            subFiles.push_back(fullName);
        }
    } 
    while (g_VFS_FindNextFileA(hFindFile, &findFileData));

    g_VFS_FindClose(hFindFile);
}

/// <summary>
/// 提取文件
/// </summary>
void ExtractFiles()
{
    std::string currentDirectory = Util::GetAppDirectoryA();
    std::vector<std::string> subFiles = std::vector<std::string>();

    EnumerateFilesA(subFiles, currentDirectory);

    //dump目录
    std::string dumpDirectory = Path::Combine(currentDirectory, "File_Dumper");
    Directory::Create(dumpDirectory);

    for (const std::string& file : subFiles)
    {
        Util::WriteDebugMessage("[Packman VFS] %s", file.c_str());

        //提取封包
        if (StringHelper::EndsWith(file, ".xp3"))
        {
            HANDLE hFile = g_VFS_CreateFileA(file.c_str(), GENERIC_READ, FILE_SHARE_READ, nullptr, OPEN_EXISTING, FILE_ATTRIBUTE_ARCHIVE, nullptr);
            if (hFile != INVALID_HANDLE_VALUE)
            {
                ULARGE_INTEGER size { };
                size.LowPart = g_VFS_GetFileSize(hFile, &size.HighPart);

                std::string fileName = Path::GetFileName(file);
                std::string outPath = Path::Combine(dumpDirectory, Path::ChangeExtension(fileName, ".spk"));

                //dump输出
                HANDLE hDumpFile = ::CreateFileA(outPath.c_str(), GENERIC_READ | GENERIC_WRITE, 0u, nullptr, CREATE_ALWAYS, FILE_ATTRIBUTE_ARCHIVE, nullptr);
                if (hDumpFile != INVALID_HANDLE_VALUE)
                {
                    unsigned __int64 position = 0ui64;
                    unsigned __int8 buf[4096]{};
                    while (position < size.QuadPart)
                    {
                        DWORD readLen = 0u;
                        if (g_VFS_ReadFile(hFile, buf, sizeof(buf), &readLen, nullptr))
                        {
                            DWORD writen = 0u;
                            ::WriteFile(hDumpFile, buf, readLen, &writen, nullptr);
                        }
                        position += readLen;
                    }
                    ::FlushFileBuffers(hDumpFile);
                    ::CloseHandle(hDumpFile);

                    Util::WriteDebugMessage("文件名: %s 文件大小: %016x", fileName.c_str(), size.QuadPart);
                }
                
                g_VFS_CloseHandle(hFile);
            }
        }
    }
}

/// <summary>
/// 初始化
/// </summary>
void Initialize()
{
    if (size_t mainModuleBase = (size_t)::GetModuleHandleW(nullptr))
    {
        //绯色的记忆之痕V2.01 Packman IAT
        g_VFS_FindFirstFileA = *(tFindFirstFileA*)(mainModuleBase + 0x3D43CCu);
        g_VFS_FindNextFileA = *(tFindNextFileA*)(mainModuleBase + 0x3D43D0u);
        g_VFS_FindClose = *(tFindClose*)(mainModuleBase + 0x3D43C8u);

        g_VFS_CreateFileA = *(tCreateFileA*)(mainModuleBase + 0x3D4388u);
        g_VFS_GetFileSize = *(tGetFileSize*)(mainModuleBase + 0x3D4424u);
        g_VFS_ReadFile = *(tReadFile*)(mainModuleBase + 0x3D4538u);
        g_VFS_SetFilePointer = *(tSetFilePointer*)(mainModuleBase + 0x3D4564u);
        g_VFS_CloseHandle = *(tCloseHandle*)(mainModuleBase + 0x3D4378u);
    }
}

BOOL APIENTRY DllMain(HMODULE hModule, DWORD ul_reason_for_call, LPVOID lpReserved)
{
    UNREFERENCED_PARAMETER(hModule);
    UNREFERENCED_PARAMETER(lpReserved);
    switch (ul_reason_for_call)
    {
        case DLL_PROCESS_ATTACH:
        {
            Initialize();
            ExtractFiles();
        }
        case DLL_THREAD_ATTACH:
        case DLL_THREAD_DETACH:
        case DLL_PROCESS_DETACH:
            break;
    }
    return TRUE;
}

extern "C" __declspec(dllexport) void Dummy()
{ 
}