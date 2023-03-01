#include <windows.h>
#include "detours.h"
#include "path.h"
#include "util.h"
#include "file.h"
#include "stringhelper.h"


int wmain(int argc, wchar_t* argv[])
{
	if (argc < 2)
	{
		printf("Usage:\n");
		printf("  Loader <path>\n\n");
		return 1;
	}

	std::wstring gamePath = Path::GetFullPath(argv[1]);
	std::wstring gameDirPath = Path::GetDirectoryName(gamePath);

	std::wstring commandLine;

	for (int i = 2; i < argc; i++)
	{
		std::wstring_view v(argv[i]);

		if (v.find(L' ') != std::wstring::npos)
		{
			commandLine += L'\"';
			commandLine += argv[i];
			commandLine += L'\"';
		}
		else
		{
			commandLine += argv[i];
		}

		commandLine += L' ';
	}

	std::string dllPath = Util::GetAppDirectoryA() + "\\ScriptDumper.dll";

	STARTUPINFO startupInfo{};
	PROCESS_INFORMATION processInfo{};

	startupInfo.cb = sizeof(startupInfo);

	if (DetourCreateProcessWithDllW(gamePath.c_str(), const_cast<std::wstring::pointer>(commandLine.c_str()),
		NULL, NULL, FALSE, 0, NULL, gameDirPath.c_str(), &startupInfo, &processInfo, dllPath.c_str(), NULL) == FALSE)
	{
		auto msg = Util::GetLastErrorMessageA();
		printf("CreateProcess failed : %s\n", msg.c_str());
		return 1;
	}

	printf("Inject Dll Success\n");
	WaitForSingleObject(processInfo.hProcess, INFINITE);

	CloseHandle(processInfo.hThread);
	CloseHandle(processInfo.hProcess);

	return 0;
}

