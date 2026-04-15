
#include <Windows.h>
#include "Common/directory.h"
#include "Common/encoding.h"
#include "Common/file.h"
#include "Common/path.h"
#include "Common/stringhelper.h"
#include "Common/util.h"
#include "KrkrPlugin/tp_stub.h"


extern "C" HRESULT __stdcall V2Link(iTVPFunctionExporter * exporter)
{
    TVPInitImportStub(exporter);
    return S_OK;
}

extern "C" HRESULT __stdcall V2Unlink()
{
    return S_OK;
}

class tTVPArchive;
using tTVPOpenArchiveFunc = tTVPArchive* (__fastcall*)(const ttstr&);

/// <summary>
/// 封包类
/// </summary>
class tTVPArchive
{
private:
    tjs_uint mRefCount;
public:
    tTVPArchive() = delete;
    tTVPArchive(const tTVPArchive&) = delete;
    tTVPArchive(tTVPArchive&&) = delete;
    tTVPArchive& operator=(const tTVPArchive&) = delete;
    tTVPArchive& operator=(tTVPArchive&&) = delete;

    void AddRef()
    {
        this->mRefCount++;
    }
    void Release()
    {
        if (this->mRefCount == 1u)
        {
            delete this;    // this->~tTVPArchive(true)
        }
        else
        {
            this->mRefCount--;
        }
    }

    virtual ~tTVPArchive() = 0;
    virtual tjs_uint GetCount() = 0;
    virtual ttstr GetName(tjs_uint idx) = 0;
    virtual tTJSBinaryStream* CreateStreamByIndex(tjs_uint idx) = 0;
};

/// <summary>
/// krkr Stream类
/// </summary>
class tTJSBinaryStream
{
public:
    tTJSBinaryStream() = delete;
    tTJSBinaryStream(const tTJSBinaryStream&) = delete;
    tTJSBinaryStream(tTJSBinaryStream&&) = delete;
    tTJSBinaryStream& operator=(const tTJSBinaryStream&) = delete;
    tTJSBinaryStream& operator=(tTJSBinaryStream&&) = delete;

    void Destruct()
    {
        delete this;    // this->~tTJSBinaryStream(true)
    }

    virtual tjs_uint64 TJS_INTF_METHOD Seek(tjs_int64 offset, tjs_int whence) = 0;
    virtual tjs_uint TJS_INTF_METHOD Read(void* buffer, tjs_uint read_size) = 0;
    virtual tjs_uint TJS_INTF_METHOD Write(const void* buffer, tjs_uint write_size) = 0;
    virtual void TJS_INTF_METHOD SetEndOfStorage() = 0;
    virtual tjs_uint64 TJS_INTF_METHOD GetSize() = 0;
    virtual ~tTJSBinaryStream() = 0;
};

/// <summary>
/// 打开封包函数
/// </summary>
static tTVPOpenArchiveFunc g_OpenXP3Archive = nullptr;

/// <summary>
/// 枚举文件
/// </summary>
void EnumerateFilesW(std::vector<std::wstring>& subFiles, const std::wstring& directory)
{
    WIN32_FIND_DATAW findFileData{ };
    HANDLE hFindFile;

    std::wstring scanPath = Path::Combine(directory, L"*");

    hFindFile = ::FindFirstFileW(scanPath.c_str(), &findFileData);
    if (hFindFile == INVALID_HANDLE_VALUE)
    {
        return;
    }

    do
    {
        if (!::lstrcmpW(findFileData.cFileName, L".") || !::lstrcmpW(findFileData.cFileName, L".."))
        {
            continue;
        }

        std::wstring fullName = Path::Combine(directory, findFileData.cFileName);
        if (findFileData.dwFileAttributes & FILE_ATTRIBUTE_DIRECTORY)
        {
            EnumerateFilesW(subFiles, fullName);
        }
        else
        {
            subFiles.push_back(fullName);
        }
    } 
    while (::FindNextFileW(hFindFile,&findFileData));

    ::FindClose(hFindFile);
}

/// <summary>
/// 转换路径为Windows路径
/// </summary>
void NormalizeWindowsPath(std::wstring& s)
{
    const size_t length = s.length();
    for (size_t i = 0u; i < length; ++i)
    {
        if (s[i] == L'/')
        {
            s[i] = L'\\';
        }
    }
}

/// <summary>
/// 提取文件
/// </summary>
void ExtractFiles()
{
    std::wstring currentDirectory = Util::GetAppDirectoryW();
    std::vector<std::wstring> subFiles = std::vector<std::wstring>();

    EnumerateFilesW(subFiles, currentDirectory);

    std::wstring dumpDirectory = Path::Combine(currentDirectory, L"File_Dumper");
    for (const std::wstring& file : subFiles)
    {
        if (StringHelper::EndsWith(file, L".xp3"))
        {
            std::wstring relativeName = file.substr(currentDirectory.length() + 1u);
            std::wstring relativeDirectory = Path::ChangeExtension(relativeName, L"");
            std::wstring extractDirectory = Path::Combine(dumpDirectory, relativeDirectory);
            
            if (tTVPArchive* arc = g_OpenXP3Archive(tTJSString(file.c_str())))
            {
                const tjs_uint count = arc->GetCount();
                Util::WriteDebugMessage(L"[封包] %s 文件个数: %u", relativeName.c_str(), count);

                for (tjs_uint idx = 0u; idx < count; ++idx)
                {
                    std::wstring name = std::wstring(arc->GetName(idx).c_str());
                    Util::WriteDebugMessage(L"[提取] %s", name.c_str());
                    
                    if (tTJSBinaryStream* stream = arc->CreateStreamByIndex(idx))
                    {
                        std::wstring extractPath = Path::Combine(extractDirectory, name);

                        //krkr路径转化为Windows路径
                        NormalizeWindowsPath(extractPath);
                        {
                            std::wstring dir = Path::GetDirectoryName(extractPath);
                            Directory::Create(dir);
                        }

                        HANDLE hFile = ::CreateFileW(extractPath.c_str(), GENERIC_READ | GENERIC_WRITE, 0u, nullptr, CREATE_ALWAYS, FILE_ATTRIBUTE_NORMAL, nullptr);
                        if (hFile != INVALID_HANDLE_VALUE)
                        {
                            unsigned __int64 pos = 0ui64;
                            unsigned __int64 len = stream->GetSize();

                            unsigned __int8 buf[4096] { };
                            while (pos < len)
                            {
                                unsigned __int32 read = stream->Read(buf, sizeof(buf));
                                DWORD writen = 0u;

                                ::WriteFile(hFile, buf, read, &writen, nullptr);

                                pos += read;
                            }

                            ::FlushFileBuffers(hFile);
                            ::CloseHandle(hFile);
                        }

                        stream->Destruct();
                    }
                }
                Util::WriteDebugMessage(L"==================================");

                arc->Release();
            }
        }
    }
}

void Initialize()
{
    if (size_t mainModuleBase = (size_t)::GetModuleHandleW(nullptr))
    {
        g_OpenXP3Archive = (tTVPOpenArchiveFunc)(mainModuleBase + 0x7AEE9u);
        V2Link((iTVPFunctionExporter*)(mainModuleBase + 0x633098u));
    }
}


BOOL APIENTRY DllMain(HMODULE hModule, DWORD  ul_reason_for_call, LPVOID lpReserved)
{
    UNREFERENCED_PARAMETER(lpReserved);
    switch (ul_reason_for_call)
    {
        case DLL_PROCESS_ATTACH:
        {
            Initialize();
            ExtractFiles();
            break;
        }
        case DLL_THREAD_ATTACH:
        case DLL_THREAD_DETACH:
        case DLL_PROCESS_DETACH:
            break;
    }
    return TRUE;
}

