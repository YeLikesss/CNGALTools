#include "NtdllExtend.h"

static void* g_LdrRegisterDllNotification = nullptr;
NTSTATUS NTAPI NtdllExtend::LdrRegisterDllNotification(ULONG Flags, PLDR_DLL_NOTIFICATION_FUNCTION NotificationFunction, PVOID Context, PVOID* Cookie)
{
    if (!g_LdrRegisterDllNotification)
    {
        g_LdrRegisterDllNotification = ::GetProcAddress(::GetModuleHandleW(L"ntdll"), "LdrRegisterDllNotification");
    }
    return ((decltype(&NtdllExtend::LdrRegisterDllNotification))g_LdrRegisterDllNotification)(Flags, NotificationFunction, Context, Cookie);
}

static void* g_LdrUnregisterDllNotification = nullptr;
NTSTATUS NTAPI NtdllExtend::LdrUnregisterDllNotification(PVOID Cookie)
{
    if (!g_LdrUnregisterDllNotification)
    {
        g_LdrUnregisterDllNotification = ::GetProcAddress(::GetModuleHandleW(L"ntdll"), "LdrUnregisterDllNotification");
    }
    return ((decltype(&NtdllExtend::LdrUnregisterDllNotification))g_LdrUnregisterDllNotification)(Cookie);
}
