
#include"BaseType.h"
#include"Path.h"
#include"StringHelper.h"
#include<Windows.h>
#include <shlobj.h>

typedef BOOLEAN(* IsCheckBuffer)(PBYTE FileBuffer, UInteger FileLength);
typedef UInteger(* DecodeBuffer)(PBYTE FileBuffer, UInteger FileLength, PBYTE* ReturnBufferPointer);


UInteger IsCCZBufferRVA = 0x00448490;       //判断CCZ资源函数RVA
UInteger InflateCCZBufferZipUtilsRVA = 0x00447E60;      //Zlib压缩资源解码函数RVA

UInteger IsGzipBufferRVA = 0x00447C50;     //判断Gzip资源函数RVA  
UInteger InflateGzipMemoryRVA = 0x004476B0;  //Gzip压缩资源解码函数RVA

const WCHAR* CrtLibraryName = L"ucrtbased.dll";     //CRT库名称
typedef void(* FreeMemory)(PVOID Buffer);


void WINAPI Dumper(PVOID ThreadParam) 
{
    //获取解密函数
    DecodeBuffer DecryptFunc = (DecodeBuffer)((UInteger)GetModuleHandleW(L"libcocos2d.dll") + InflateCCZBufferZipUtilsRVA);
    //获取程序释放内存函数
    FreeMemory Free = (FreeMemory)GetProcAddress(GetModuleHandleW(CrtLibraryName), "free");

    WCHAR* ExtractFolder = (WCHAR*)HeapAlloc(GetProcessHeap(), HEAP_ZERO_MEMORY, 0x1000);   //导出路径
    WCHAR* ResFolder = (WCHAR*)HeapAlloc(GetProcessHeap(), HEAP_ZERO_MEMORY, 0x1000);       //资源路径
    WCHAR* ScanFolder = (WCHAR*)HeapAlloc(GetProcessHeap(), HEAP_ZERO_MEMORY, 0x1000);      //遍历路径
    GetModuleFileNameW(NULL, ResFolder, 0x800);         //获取主程序路径
    Path::GetDirectoryPathW(ResFolder, TRUE);           //获取主程序文件夹
    Strings::StringConcatW(ResFolder, L"Resources\\");    //设置资源路径
    Strings::StringCopyW(ExtractFolder, ResFolder);     //设置提取目标路径
    Strings::StringConcatW(ExtractFolder, L"Extract\\");

    SHCreateDirectory(NULL, ExtractFolder);     //创建文件夹

    WCHAR* FilePath = (WCHAR*)HeapAlloc(GetProcessHeap(), HEAP_ZERO_MEMORY, 0x1000);    //资源文件路径
    Strings::StringCopyW(FilePath, ResFolder);
    WCHAR* ExtractFilePath = (WCHAR*)HeapAlloc(GetProcessHeap(), HEAP_ZERO_MEMORY, 0x1000);    //提取文件路径
    Strings::StringCopyW(ExtractFilePath, ExtractFolder);


    WIN32_FIND_DATAW findFileData;
    HANDLE hListFile;

    Strings::StringCopyW(ScanFolder, ResFolder);     //设置遍历路径
    Strings::StringConcatW(ScanFolder, L"*.ccz");        //新增筛选后缀

    //开始遍历
    hListFile = FindFirstFileW(ScanFolder, &findFileData);
    if (hListFile == INVALID_HANDLE_VALUE)
    {
        MessageBoxW(NULL, L"文件遍历失败", L"Error", MB_OK);
        HeapFree(GetProcessHeap(), 0, ExtractFolder);
        HeapFree(GetProcessHeap(), 0, ResFolder);
        HeapFree(GetProcessHeap(), 0, ScanFolder);
        HeapFree(GetProcessHeap(), 0, FilePath);
        HeapFree(GetProcessHeap(), 0, ExtractFilePath);
        return;

    }

    //遍历文件
    do 
    {
        if (Strings::StringCompareW(findFileData.cFileName, L".") || Strings::StringCompareW(findFileData.cFileName, L"..")) 
        {
            continue;
        }
        if (findFileData.dwFileAttributes & FILE_ATTRIBUTE_DIRECTORY) 
        {
            continue;
        }

        OutputDebugStringW(findFileData.cFileName);

        Path::GetDirectoryPathW(FilePath, TRUE);    //设定读取路径
        Strings::StringConcatW(FilePath, findFileData.cFileName);
        
        HANDLE fileHandle = CreateFileW(FilePath, GENERIC_READ, FILE_SHARE_READ | FILE_SHARE_WRITE, NULL, OPEN_ALWAYS, FILE_ATTRIBUTE_NORMAL, NULL);
        if (fileHandle == INVALID_HANDLE_VALUE) 
        {
            OutputDebugStringW(L"文件打开失败");
            continue;
        }

        //获取文件大小
        UInteger fileSize= GetFileSize(fileHandle, NULL);

        //设置流位置
        SetFilePointer(fileHandle, 0, NULL, FILE_BEGIN);

        //文件缓存
        PBYTE fileData = (PBYTE)HeapAlloc(GetProcessHeap(), 0, fileSize);

        //读取文件
        UInteger returnFileNumberToRead = 0;
        ReadFile(fileHandle, fileData, fileSize, &returnFileNumberToRead, NULL);

        //关闭封包流
        CloseHandle(fileHandle);

        //原始文件
        PBYTE originalFileData;
        //原始大小
        UInteger originalFileSize;

        //解密
        originalFileSize = DecryptFunc(fileData, fileSize, &originalFileData);

        //释放原文件资源
        HeapFree(GetProcessHeap(), 0, fileData);

        if ((SInteger)originalFileSize == -1) 
        {
            OutputDebugStringW(L"文件解密失败");
            continue;
        }

        Path::GetDirectoryPathW(ExtractFilePath, TRUE);    //设定提取路径
        Strings::StringConcatW(ExtractFilePath, findFileData.cFileName);

        //去掉ccz后缀
        ExtractFilePath[Strings::StringLengthW(ExtractFilePath) - 4] = '\0';

        //创建写入流
        HANDLE extractFileHandle = CreateFileW(ExtractFilePath, GENERIC_WRITE, FILE_SHARE_READ | FILE_SHARE_WRITE, NULL, CREATE_ALWAYS, FILE_ATTRIBUTE_NORMAL, NULL);
        if (extractFileHandle == INVALID_HANDLE_VALUE)
        {
            OutputDebugStringW(Path::GetFileNameW(ExtractFilePath));
            OutputDebugStringW(L"文件创建失败");
            continue;
        }

        //设置流位置
        SetFilePointer(extractFileHandle, 0, NULL, FILE_BEGIN);

        //写入文件
        UInteger returnFileNumberToWrite = 0;
        WriteFile(extractFileHandle, originalFileData, originalFileSize, &returnFileNumberToWrite, NULL);

        //关闭文件流
        CloseHandle(extractFileHandle);

        //释放内存
        Free(originalFileData);

        OutputDebugStringW(L"提取成功");

    } while (FindNextFileW(hListFile, &findFileData));

    //释放资源
    HeapFree(GetProcessHeap(), 0, ExtractFolder);
    HeapFree(GetProcessHeap(), 0, ResFolder);
    HeapFree(GetProcessHeap(), 0, ScanFolder);
    HeapFree(GetProcessHeap(), 0, FilePath);
    HeapFree(GetProcessHeap(), 0, ExtractFilePath);

    return;
}




BOOL APIENTRY DllMain( HMODULE hModule, DWORD  ul_reason_for_call,LPVOID lpReserved)
{
    switch (ul_reason_for_call)
    {
    case DLL_PROCESS_ATTACH:
        CreateThread(NULL, 0, (LPTHREAD_START_ROUTINE)Dumper, NULL, 0, NULL);
        break;
    case DLL_THREAD_ATTACH:
    case DLL_THREAD_DETACH:
    case DLL_PROCESS_DETACH:
        break;
    }
    return TRUE;
}

