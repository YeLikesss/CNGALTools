#pragma once
#include "ntdll.h"

namespace NtdllExtend
{
    struct LDR_DLL_UNLOADED_NOTIFICATION_DATA
    {
        ULONG Flags;                    //Reserved.
        PUNICODE_STRING FullDllName;    //The full path name of the DLL module.
        PUNICODE_STRING BaseDllName;    //The base file name of the DLL module.
        PVOID DllBase;                  //A pointer to the base address for the DLL in memory.
        ULONG SizeOfImage;              //The size of the DLL image, in bytes.
    };

    struct LDR_DLL_LOADED_NOTIFICATION_DATA
    {
        ULONG Flags;                    //Reserved.
        PUNICODE_STRING FullDllName;    //The full path name of the DLL module.
        PUNICODE_STRING BaseDllName;    //The base file name of the DLL module.
        PVOID DllBase;                  //A pointer to the base address for the DLL in memory.
        ULONG SizeOfImage;              //The size of the DLL image, in bytes.
    };

    union LDR_DLL_NOTIFICATION_DATA
    {
        LDR_DLL_LOADED_NOTIFICATION_DATA Loaded;
        LDR_DLL_UNLOADED_NOTIFICATION_DATA Unloaded;
    };

    using PLDR_DLL_NOTIFICATION_FUNCTION = VOID(CALLBACK*)(ULONG NotificationReason, const LDR_DLL_NOTIFICATION_DATA* NotificationData, PVOID Context);

    constexpr ULONG LDR_DLL_NOTIFICATION_REASON_LOADED = 1u;
    constexpr ULONG LDR_DLL_NOTIFICATION_REASON_UNLOADED = 2u;

    NTSTATUS NTAPI LdrRegisterDllNotification(ULONG Flags, PLDR_DLL_NOTIFICATION_FUNCTION NotificationFunction, PVOID Context, PVOID* Cookie);
    NTSTATUS NTAPI LdrUnregisterDllNotification(PVOID Cookie);
}


