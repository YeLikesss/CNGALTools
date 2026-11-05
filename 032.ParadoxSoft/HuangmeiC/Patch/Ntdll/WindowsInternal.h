#pragma once
#include <windows.h>

namespace WindowsInternal
{
	typedef struct _FILE_BASIC_INFORMATION
	{
		LARGE_INTEGER CreationTime;
		LARGE_INTEGER LastAccessTime;
		LARGE_INTEGER LastWriteTime;
		LARGE_INTEGER ChangeTime;
		ULONG         FileAttributes;
	} FILE_BASIC_INFORMATION, * PFILE_BASIC_INFORMATION;

	typedef struct _FILE_STANDARD_INFORMATION
	{
		LARGE_INTEGER AllocationSize;
		LARGE_INTEGER EndOfFile;
		ULONG         NumberOfLinks;
		BOOLEAN       DeletePending;
		BOOLEAN       Directory;
	} FILE_STANDARD_INFORMATION, * PFILE_STANDARD_INFORMATION;

	typedef struct _FILE_INTERNAL_INFORMATION
	{
		LARGE_INTEGER IndexNumber;
	} FILE_INTERNAL_INFORMATION, * PFILE_INTERNAL_INFORMATION;

	typedef struct _FILE_EA_INFORMATION
	{
		ULONG EaSize;
	} FILE_EA_INFORMATION, * PFILE_EA_INFORMATION;

	typedef struct _FILE_ACCESS_INFORMATION
	{
		ACCESS_MASK AccessFlags;
	} FILE_ACCESS_INFORMATION, * PFILE_ACCESS_INFORMATION;

	typedef struct _FILE_POSITION_INFORMATION
	{
		LARGE_INTEGER CurrentByteOffset;
	} FILE_POSITION_INFORMATION, * PFILE_POSITION_INFORMATION;

	typedef struct _FILE_MODE_INFORMATION
	{
		ULONG Mode;
	} FILE_MODE_INFORMATION, * PFILE_MODE_INFORMATION;

	typedef struct _FILE_ALIGNMENT_INFORMATION
	{
		ULONG AlignmentRequirement;
	} FILE_ALIGNMENT_INFORMATION, * PFILE_ALIGNMENT_INFORMATION;

	typedef struct _FILE_NAME_INFORMATION
	{
		ULONG FileNameLength;
		WCHAR FileName[1];
	} FILE_NAME_INFORMATION, * PFILE_NAME_INFORMATION;

	typedef struct _FILE_ALL_INFORMATION
	{
		FILE_BASIC_INFORMATION     BasicInformation;
		FILE_STANDARD_INFORMATION  StandardInformation;
		FILE_INTERNAL_INFORMATION  InternalInformation;
		FILE_EA_INFORMATION        EaInformation;
		FILE_ACCESS_INFORMATION    AccessInformation;
		FILE_POSITION_INFORMATION  PositionInformation;
		FILE_MODE_INFORMATION      ModeInformation;
		FILE_ALIGNMENT_INFORMATION AlignmentInformation;
		FILE_NAME_INFORMATION      NameInformation;
	} FILE_ALL_INFORMATION, * PFILE_ALL_INFORMATION;

	typedef struct _FILE_FULL_DIR_INFORMATION
	{
		ULONG         NextEntryOffset;
		ULONG         FileIndex;
		LARGE_INTEGER CreationTime;
		LARGE_INTEGER LastAccessTime;
		LARGE_INTEGER LastWriteTime;
		LARGE_INTEGER ChangeTime;
		LARGE_INTEGER EndOfFile;
		LARGE_INTEGER AllocationSize;
		ULONG         FileAttributes;
		ULONG         FileNameLength;
		ULONG         EaSize;
		WCHAR         FileName[1];
	} FILE_FULL_DIR_INFORMATION, * PFILE_FULL_DIR_INFORMATION;

	typedef struct _FILE_BOTH_DIR_INFORMATION
	{
		ULONG         NextEntryOffset;
		ULONG         FileIndex;
		LARGE_INTEGER CreationTime;
		LARGE_INTEGER LastAccessTime;
		LARGE_INTEGER LastWriteTime;
		LARGE_INTEGER ChangeTime;
		LARGE_INTEGER EndOfFile;
		LARGE_INTEGER AllocationSize;
		ULONG         FileAttributes;
		ULONG         FileNameLength;
		ULONG         EaSize;
		CCHAR         ShortNameLength;
		WCHAR         ShortName[12];
		WCHAR         FileName[1];
	} FILE_BOTH_DIR_INFORMATION, * PFILE_BOTH_DIR_INFORMATION;

	enum SL_QUERY_DIRECTORY_MASK : unsigned __int32
	{
		SL_RESTART_SCAN = 0x00000001u,
		SL_RETURN_SINGLE_ENTRY = 0x00000002u,
		SL_INDEX_SPECIFIED = 0x00000004u,
		SL_RETURN_ON_DISK_ENTRIES_ONLY = 0x00000008u,
		SL_NO_CURSOR_UPDATE_QUERY = 0x00000010u,
	};
}

