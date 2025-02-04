// log.h

#pragma once

#include <cstdio>

namespace Log
{
	class Logger
	{
	public:
		Logger();
		Logger(const wchar_t* lpFileName);
		~Logger();

		Logger(const Logger&) = delete;
		Logger& operator=(const Logger&) = delete;

		void Open(const wchar_t* lpFileName);
		void Close();
		void Flush();

		void WriteAnsi(int iCodePage, const char* lpFormat, ...);
		void WriteLineAnsi(int iCodePage, const char* lpFormat, ...);
		void Write(const wchar_t* lpFormat, ...);
		void WriteLine(const wchar_t* lpFormat, ...);
		void WriteUnicode(const wchar_t* lpFormat, ...);
		void WriteData(void* data, unsigned int size);


	private:
		FILE* m_pOutput;
	};
}
