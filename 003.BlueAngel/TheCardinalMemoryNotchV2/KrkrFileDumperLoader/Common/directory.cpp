
#include <Windows.h>
#include "directory.h"
#include "path.h"

namespace Directory
{
	bool Exists(const std::string& dirPath)
	{
		DWORD fileAttr = GetFileAttributesA(dirPath.c_str());
		if (fileAttr == INVALID_FILE_ATTRIBUTES || (fileAttr & FILE_ATTRIBUTE_DIRECTORY) == 0)
		{
			return false;
		}
		return true;
	}

	bool Exists(const std::wstring& dirPath)
	{
		DWORD fileAttr = GetFileAttributesW(dirPath.c_str());
		if (fileAttr == INVALID_FILE_ATTRIBUTES || (fileAttr & FILE_ATTRIBUTE_DIRECTORY) == 0)
		{
			return false;
		}
		return true;
	}

	void Create(const std::string& dirPath)
	{
		if (!Directory::Exists(dirPath))
		{
			if (!CreateDirectoryA(dirPath.c_str(), NULL))
			{
				Directory::Create(Path::GetDirectoryName(dirPath));
				CreateDirectoryA(dirPath.c_str(), NULL);
			}
		}
	}


	void Create(const std::wstring& dirPath)
	{
		if (!Directory::Exists(dirPath))
		{
			if (!CreateDirectoryW(dirPath.c_str(), NULL))
			{
				Directory::Create(Path::GetDirectoryName(dirPath));
				CreateDirectoryW(dirPath.c_str(), NULL);
			}
		}
	}
}