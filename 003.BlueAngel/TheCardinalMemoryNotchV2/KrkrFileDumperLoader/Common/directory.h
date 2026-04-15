#pragma once

#include <string>
namespace Directory
{
	bool Exists(const std::string& dirPath);
	bool Exists(const std::wstring& dirPath);
	void Create(const std::string& dirPath);
	void Create(const std::wstring& dirPath);
}
