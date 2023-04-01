#include "util.h"
#include "path.h"
#include "log.h"
#include "pe.h"
#include "stringhelper.h"
#include "file.h"
#include "encoding.h"
#include "detours.h"
#include <regex>
#include <vector>

#pragma warning ( push )
#pragma warning ( disable : 4100 4201 4457 )
#include "tp_stub.h"
#pragma warning ( pop )


static HMODULE g_hEXE;
static HMODULE g_hDLL;

static std::wstring g_exePath;
static std::wstring g_dllPath;
static std::wstring g_currentDirPath;

static std::wstring g_krVFSCurrentDirPath;

static Log::Logger g_logger;
static Log::Logger g_nvlFullPathLogger;
static Log::Logger g_nvlRelativePathLogger;
static Log::Logger g_nvlAutoPathLogger;

//获取TJSString对象字符串
const tjs_char* TJSStringGetPtr(tTJSString* s)
{
	if (!s)
		return L"";

	tTJSVariantString_S* v = *(tTJSVariantString_S**)s;

	if (!v)
		return L"";

	if (v->LongString)
		return v->LongString;

	return v->ShortString;
}


class tTJSBinaryStream
{
public:
	virtual tjs_uint64 TJS_INTF_METHOD Seek(tjs_int64 offset, tjs_int whence) = 0;
	virtual tjs_uint TJS_INTF_METHOD Read(void* buffer, tjs_uint read_size) = 0;
	// virtual tjs_uint TJS_INTF_METHOD Write(const void* buffer, tjs_uint write_size) = 0;
	// virtual void TJS_INTF_METHOD SetEndOfStorage() = 0;
	// virtual tjs_uint64 TJS_INTF_METHOD GetSize() = 0;
	// virtual ~tTJSBinaryStream() { }
};

tjs_uint64 TJSBinaryStream_GetLength(tTJSBinaryStream* stream)
{
	tjs_uint64 size;

	size = stream->Seek(0, TJS_BS_SEEK_END);
	stream->Seek(0, TJS_BS_SEEK_SET);

	return size;
}


static std::wstring g_outputPath;

static std::vector<std::wstring> g_regexIncludeRules;
static std::vector<std::wstring> g_regexExcludeRules;

//检查游戏资源封包路径 并提取文件名
std::wstring MatchPath(const std::wstring& path)
{
	std::wstring newPath;

	if (path.find(L':') != std::string::npos)
	{
		for (auto& rule : g_regexIncludeRules)
		{
			std::wregex expr(rule, std::regex_constants::icase);
			std::wsmatch result;

			if (std::regex_match(path, result, expr))
			{
				if (result.size() > 1)
					newPath = result[1].str();
				else
					newPath = result[0].str();
			}
		}
	}
	else
	{
		newPath = path;
	}
	return newPath;
}

//检查游戏资源封包路径
BOOL MatchPathNVL(const wchar_t* path, std::vector<std::wstring>& regex)
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



//斜杠转反斜杠  大写转小写
void FixPath(std::wstring& path)
{
	for (size_t i = 0; i < path.length(); i++)
	{
		if (path[i] == L'\\')
		{
			path[i] = L'/';
		}

		if (path[i] >= L'A' && path[i] <= L'Z')
		{
			path[i] |= 0x20;
		}
	}
}

//将文件夹路径转化封包格式
std::wstring GetXP3VFSCurrentDirectoryPath(const std::wstring& currentDirPath)
{
	wchar_t diskVol[2]{ 0 };
	diskVol[0] = currentDirPath.c_str()[0];       //获取盘符

	std::wstring xp3CurrentDir(L"file://./");
	xp3CurrentDir += diskVol;         //添加盘符
	xp3CurrentDir += &currentDirPath.c_str()[2];        //添加文件夹路径

	FixPath(xp3CurrentDir);

	return xp3CurrentDir;
}

//获取资源相对路径
const wchar_t* GetRelativePath(const wchar_t* path)
{
	if (MatchPathNVL(path,g_regexIncludeRules)) 
	{
		int length = 0;

		const wchar_t* newPath = path;

		//获取字符串长度 字符串移到最后
		while (*newPath != '\0')
		{
			length++;
			newPath++;
		}

		//小于两个字符
		if (length < 2)
		{
			return newPath;
		}

		//扫描封包分割符号
		while (length != 0)
		{
			if (*newPath == '>')
			{
				return newPath + 1;
			}
			newPath--;
			length--;
		}

		return newPath;
	}
	else if(!MatchPathNVL(path, g_regexExcludeRules))
	{
		int pathLen = lstrlenW(path);
		int dirLen = g_krVFSCurrentDirPath.length();

		Sleep(1);

		if (dirLen >= pathLen)
		{
			return path;
		}

		if(!memcmp(path, g_krVFSCurrentDirPath.c_str(), dirLen * sizeof(wchar_t)))
		{
			return &path[dirLen + 1];
		}
		else
		{
			return path;
		}
	}
	else
	{
		return L"";
	}

}

//创建文件夹
void FullCreateDirectoryW(const std::wstring& dirPath)
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

//提取资源
void ExtractFile(tTJSBinaryStream* stream, std::wstring& extractPath)
{
	FixPath(extractPath);   //修复路径

	if (StringHelper::StartsWith(extractPath, L".\\"))
	{
		extractPath = extractPath.substr(2);
	}

	std::wstring outputPath = g_outputPath + extractPath;

	//创建文件夹

	std::wstring outputDir = Path::GetDirectoryName(outputPath);

	if (!outputDir.empty())
	{
		FullCreateDirectoryW(outputDir.c_str());
	}

	// 导出文件

	size_t size = (size_t)TJSBinaryStream_GetLength(stream);

	if (size > 0)
	{
		std::vector<uint8_t> buffer;

		bool success = false;

		buffer.resize(size);		//调整动态数组容器大小

		stream->Seek(0, TJS_BS_SEEK_SET);

		if (stream->Read(buffer.data(), size) == size)  //读取KR资源流
		{
			success = true;
		}

		if (success && !buffer.empty())
		{
			g_logger.WriteLine(L"Extract \"%s\"", extractPath.c_str());

			if (File::WriteAllBytes(outputPath, buffer.data(), buffer.size()) == false)
			{
				g_logger.WriteLine(L"Failed to write \"%s\"", outputPath.c_str());
			}
		}

		stream->Seek(0, TJS_BS_SEEK_SET);
	}
	else
	{
		File::WriteAllBytes(outputPath, NULL, 0);
	}
}

//处理资源流
void ProcessStream(tTJSBinaryStream* stream, ttstr* name, tjs_uint32 flags)
{
	if (stream && flags == TJS_BS_READ)
	{
		try
		{
			const tjs_char* psz = TJSStringGetPtr(name);	//获取资源名

			std::wstring path(psz);
			std::wstring extractPath = MatchPath(path);		//提取文件名

			if (!extractPath.empty())
			{
				g_logger.WriteLine(L"Included \"%s\"", psz);

				ExtractFile(stream, extractPath);		//提取资源
			}
			else
			{
				g_logger.WriteLine(L"Excluded \"%s\"", psz);
			}
		}
		catch (const std::exception& e)
		{
			g_logger.WriteLineAnsi(CP_ACP, "Exception : %s", e.what());
		}
	}
}


// 特征码
// 55 8B EC 81 C4 64 FF FF FF 53 56 57 89 85 70 FF FF FF C7 85 7C FF FF FF ?? ?? ?? ?? 89 65 80 B8 ?? ?? ?? ?? 89 85 78 FF FF FF 8B DA 66 C7 45 84 00 00 33 D2 89 55 90 64 8B 0D 00 00 00 00 89 8D 74 FF FF FF 8D 85 74 FF FF FF 64 A3 00 00 00 00 66 C7 45 84 0C 00 8B D3 8B 85 70 FF FF FF E8 ?? ?? ?? ?? 8B 95 74 FF FF FF 64 89 15 00 00 00 00
// NVLKRKR2 2.32.1.426 (BCB)
//
#define NVLKR2_TVPCREATEMSTREAM_SIGNATURE "\x55\x8B\xEC\x81\xC4\x64\xFF\xFF\xFF\x53\x56\x57\x89\x85\x70\xFF\xFF\xFF\xC7\x85\x7C\xFF\xFF\xFF\x2A\x2A\x2A\x2A\x89\x65\x80\xB8\x2A\x2A\x2A\x2A\x89\x85\x78\xFF\xFF\xFF\x8B\xDA\x66\xC7\x45\x84\x00\x00\x33\xD2\x89\x55\x90\x64\x8B\x0D\x00\x00\x00\x00\x89\x8D\x74\xFF\xFF\xFF\x8D\x85\x74\xFF\xFF\xFF\x64\xA3\x00\x00\x00\x00\x66\xC7\x45\x84\x0C\x00\x8B\xD3\x8B\x85\x70\xFF\xFF\xFF\xE8\x2A\x2A\x2A\x2A\x8B\x95\x74\xFF\xFF\xFF\x64\x89\x15\x00\x00\x00\x00"
#define NVLKR2_TVPCREATEMSTREAM_SIGNATURE_LENGTH ( sizeof(NVLKR2_TVPCREATEMSTREAM_SIGNATURE) -1 )


//原函数指针
PVOID pfnKrkr2BcbFastCallTVPCreateStreamProc = NULL;

//MSVC cdcel转Borland C++ fastcall
_declspec(naked)
tTJSBinaryStream* Krkr2BcbFastCallTVPCreateStreamCallback(ttstr* name, tjs_uint32 flags)
{
	_asm
	{
		mov edx, flags
		mov eax, name
		call pfnKrkr2BcbFastCallTVPCreateStreamProc
		ret
	}
}

//Hook
tTJSBinaryStream* Krkr2BcbFastCallTVPCreateStream(ttstr* name, tjs_uint32 flags)
{
	tTJSBinaryStream* stream = Krkr2BcbFastCallTVPCreateStreamCallback(name, flags);
	ProcessStream(stream, name, flags);
	return stream;
}
//Borland C++ fastcall转MSVC cdcel
_declspec(naked)
void Krkr2BcbFastCallTVPCreateStreamDetour()
{
	_asm
	{
		push edx
		push eax
		call Krkr2BcbFastCallTVPCreateStream
		add esp, 8
		ret
	}
}

//Nvl文件名完整路径Hook

//特征码
#define NVLKR2_FindInTable_SIGNATURE "\x55\x8B\xEC\x83\xC4\xC8\x33\xD2\x53\x56\x57\x8B\xD8\xC7\x45\xD8\x2A\x2A\x2A\x2A\x89\x65\xDC\xB8\x2A\x2A\x2A\x2A\x89\x45\xD4\x66\xC7\x45\xE0\x00\x00\x89\x55\xEC\x64\x8B\x0D\x00\x00\x00\x00\x89\x4D\xD0\x8D\x45\xD0\x64\xA3\x00\x00\x00\x00\x66\xC7\x45\xE0\x0C\x00\xBA\x2A\x2A\x2A\x2A\x8D\x45\xFC\xE8\x2A\x2A\x2A\x2A\xFF\x45\xEC\x66\xC7\x45\xE0\x18\x00\x0F\xB7\x15\x2A\x2A\x2A\x2A\x52\x83\x3B\x00\x74\x1A\x8B\x03\x85\xC0\x75\x04\x33\xC9\xEB\x16\x83\x78\x04\x00\x74\x05\x8B\x48\x04\xEB\x0B\x8D\x48\x08\xEB\x06\x8B\x0D\x2A\x2A\x2A\x2A\x51"
#define NVLKR2_FindInTable_SIGNATURE_LENGTH ( sizeof(NVLKR2_FindInTable_SIGNATURE) -1 )

//原函数指针
PVOID pfnKrkr2BcbFastCallFindInTable = NULL;

//MSVC cdcel转Borland C++ fastcall
_declspec(naked)
BOOLEAN Krkr2BcbFastCallFindInTableCallback(ttstr* name)
{
	_asm
	{
		mov eax, name
		call pfnKrkr2BcbFastCallFindInTable
		ret
	}
}

BOOLEAN HookKrkr2BcbFastCallFindInTable(ttstr* name)
{
	//判断返回值al寄存器
	BOOLEAN isExist = Krkr2BcbFastCallFindInTableCallback(name);		

	if (isExist)
	{
		//封包路径存在
		const wchar_t* namePtr = TJSStringGetPtr(name);
		g_nvlFullPathLogger.Write(L"%s\n", namePtr);

		const wchar_t* relativeNamePtr = GetRelativePath(namePtr);
		g_nvlRelativePathLogger.Write(L"%s\n", relativeNamePtr);		//带文件夹相对路径
		g_nvlRelativePathLogger.Write(L"%s\n", Path::GetFileName(relativeNamePtr).c_str());		//仅文件名
	}
	return isExist;
}

//Borland C++ fastcall转MSVC cdcel
_declspec(naked)
void Krkr2BcbFastCallFindInTableDetour()
{
	_asm
	{
		push eax
		call HookKrkr2BcbFastCallFindInTable
		add esp, 4
		ret
	}
}

//TVPAddAutoPath

//特征码
#define NVLKR2_AddAutoPath_SIGNATURE "\x55\x8B\xEC\x81\xC4\x50\xFF\xFF\xFF\x33\xD2\x53\x56\x57\x8B\xD8\xC7\x45\xDC\x2A\x2A\x2A\x2A\x89\x65\xE0\xB8\x2A\x2A\x2A\x2A\x89\x45\xD8\x66\xC7\x45\xE4\x00\x00\x89\x55\xF0\x64\x8B\x0D\x00\x00\x00\x00\x89\x4D\xD4\x8D\x45\xD4\x64\xA3\x00\x00\x00\x00\x66\xC7\x45\xE4\x0C\x00\xBA\x2A\x2A\x2A\x2A\x8D\x45\xFC\xE8\x2A\x2A\x2A\x2A\xFF\x45\xF0\x66\xC7\x45\xE4\x18\x00\x83\x3B\x00"
#define NVLKR2_AddAutoPath_SIGNATURE_LENGTH ( sizeof(NVLKR2_AddAutoPath_SIGNATURE) -1 )

//原函数指针
PVOID pfnKrkr2BcbFastCallAddAutoPath = NULL;

void HookKrkr2BcbFastCallAddAutoPath(ttstr* path) 
{
	const wchar_t* str = TJSStringGetPtr(path);
	g_nvlAutoPathLogger.Write(L"%s\n", GetRelativePath(str));
}

//Borland C++ fastcall转MSVC cdcel
_declspec(naked)
void Krkr2BcbFastCallFindAddAutoPathDetour()
{
	_asm 
	{
		push eax
		call pfnKrkr2BcbFastCallAddAutoPath
		call HookKrkr2BcbFastCallAddAutoPath
		add esp,4
		ret
	}
}

//设定
void LoadConfiguration()
{
	g_outputPath.clear();
	g_regexIncludeRules.clear();
	g_regexExcludeRules.clear();

	//资源导出路径为游戏路径+"/Extract"
	wchar_t gameDir[MAX_PATH];
	GetCurrentDirectoryW(256, gameDir);
	std::wstring outPutPath = std::wstring(gameDir);
	outPutPath += L"\\Extract\\";
	g_outputPath = std::move(outPutPath);

	//路径筛选器
	g_regexIncludeRules.push_back(L"file://\\./.+?\\.xp3>(.+?\\..+$)");

	g_regexExcludeRules.push_back(L"file://\\./.+?\\.xp3(>)?");
}

void InstallHooks() 
{
	PVOID base = PE::GetModuleBase(g_hEXE);
	DWORD size = PE::GetModuleSize(g_hEXE);

	g_logger.WriteLine(L"Image Base = 0x%p", base);
	g_logger.WriteLine(L"Image Size = 0x%08X", size);

	PVOID pfnTVPCreateStream = NULL;

	//NVLKR2搜索TVPCreateStream函数
	pfnTVPCreateStream = PE::SearchPattern(base, size, NVLKR2_TVPCREATEMSTREAM_SIGNATURE, NVLKR2_TVPCREATEMSTREAM_SIGNATURE_LENGTH);
	if (pfnTVPCreateStream)
	{
		pfnKrkr2BcbFastCallTVPCreateStreamProc = pfnTVPCreateStream;

		DetourUpdateThread(GetCurrentThread());
		DetourTransactionBegin();
		DetourAttach(&pfnKrkr2BcbFastCallTVPCreateStreamProc, Krkr2BcbFastCallTVPCreateStreamDetour);
		DetourTransactionCommit();

		g_logger.WriteLine(L"TVPCreateStream Address = 0x%p", pfnTVPCreateStream);
		g_logger.WriteLine(L"TVPCreateStream Hooks Installed");
	}

	//搜索FindInTable游戏遍历封包函数
	pfnKrkr2BcbFastCallFindInTable = PE::SearchPattern(base, size, NVLKR2_FindInTable_SIGNATURE, NVLKR2_FindInTable_SIGNATURE_LENGTH);
	if (pfnKrkr2BcbFastCallFindInTable)
	{
		g_logger.WriteLine(L"FindInTable Address = 0x%p", pfnKrkr2BcbFastCallFindInTable);

		DetourUpdateThread(GetCurrentThread());
		DetourTransactionBegin();
		DetourAttach(&pfnKrkr2BcbFastCallFindInTable, Krkr2BcbFastCallFindInTableDetour);
		DetourTransactionCommit();

		g_logger.WriteLine(L"FindInTable Hooks Installed");

		//打开log  记录资源路径 (用于GARbro导入)
		g_nvlFullPathLogger.Open((Path::GetDirectoryName(g_dllPath) + L"\\FullPath.lst").c_str());
		g_nvlRelativePathLogger.Open((Path::GetDirectoryName(g_dllPath) + L"\\RelativePath.lst").c_str());
	}

	//搜索TVPAddAutoPath函数
	pfnKrkr2BcbFastCallAddAutoPath = PE::SearchPattern(base, size, NVLKR2_AddAutoPath_SIGNATURE, NVLKR2_AddAutoPath_SIGNATURE_LENGTH);
	if (pfnKrkr2BcbFastCallAddAutoPath) 
	{
		g_logger.WriteLine(L"TVPAddAutoPath Address = 0x%p", pfnKrkr2BcbFastCallAddAutoPath);

		DetourUpdateThread(GetCurrentThread());
		DetourTransactionBegin();
		DetourAttach(&pfnKrkr2BcbFastCallAddAutoPath, Krkr2BcbFastCallFindAddAutoPathDetour);
		DetourTransactionCommit();

		g_logger.WriteLine(L"TVPAddAutoPath Hooks Installed");

		g_nvlAutoPathLogger.Open((Path::GetDirectoryName(g_dllPath) + L"\\AutoPath.lst").c_str());
	}

}

//启动
void OnStartup()
{
	std::wstring exePath = Util::GetModulePathW(g_hEXE);
	std::wstring dllPath = Util::GetModulePathW(g_hDLL);
	std::wstring logPath = Path::ChangeExtension(dllPath, L"log");
	std::wstring currentDir = Path::GetDirectoryName(exePath);

	Util::WriteDebugMessage(L"[KrkrDump] EXE Path = \"%s\"", exePath.c_str());
	Util::WriteDebugMessage(L"[KrkrDump] DLL Path = \"%s\"", dllPath.c_str());
	Util::WriteDebugMessage(L"[KrkrDump] Log Path = \"%s\"", logPath.c_str());

	// !!!
	File::Delete(logPath);

	g_logger.Open(logPath.c_str());

	g_logger.WriteLine(L"Startup");

	g_logger.WriteLine(L"Game Executable Path = \"%s\"", exePath.c_str());

	g_exePath = std::move(exePath);
	g_dllPath = std::move(dllPath);
	g_currentDirPath = std::move(currentDir);

	g_krVFSCurrentDirPath = std::move(GetXP3VFSCurrentDirectoryPath(g_currentDirPath));

	// Started

	try
	{
		LoadConfiguration();		//加载设定
		g_logger.WriteLine(L"Configuration loaded");
	}
	catch (const std::exception&)
	{
		g_logger.WriteLine(L"Failed to load configuration");
	}

	try
	{
		InstallHooks();
	}
	catch (const std::exception&)
	{
		g_logger.WriteLine(L"Failed to install hooks");
	}
}

//关闭日志文档
void OnShutdown()
{
	g_logger.WriteLine(L"Shutdown");
	g_logger.Close();

	g_nvlFullPathLogger.Close();
	g_nvlRelativePathLogger.Close();
	g_nvlAutoPathLogger.Close();
}

extern "C" __declspec(dllexport) void Dummy() {  }

BOOL APIENTRY DllMain(HMODULE hModule, DWORD ul_reason_for_call, LPVOID lpReserved)
{
	UNREFERENCED_PARAMETER(lpReserved);

	switch (ul_reason_for_call)
	{
		case DLL_PROCESS_ATTACH:
		{
			g_hEXE = GetModuleHandleW(NULL);
			g_hDLL = hModule;
			OnStartup();
			break;
		}
		case DLL_THREAD_ATTACH:
			break;
		case DLL_THREAD_DETACH:
			break;
		case DLL_PROCESS_DETACH:
		{
			OnShutdown();
			break;
		}
	}
	return TRUE;
}