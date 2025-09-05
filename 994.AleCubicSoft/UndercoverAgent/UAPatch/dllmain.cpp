#include "NtdllExtend.h"
#include "ExtendUtils.h"
#include "SaveDataPatch.h"

#pragma comment(linker, "/MERGE:\".detourd=.data\"")
#pragma comment(linker, "/MERGE:\".detourc=.rdata\"")

static bool g_Initialized = false;
static void* g_DllLoadNotificationHandle = nullptr;

void Initialize(PVOID dllBase);
DWORD WINAPI Process(PVOID dllBase);
VOID CALLBACK DllLoadNotification(ULONG NotificationReason, const NtdllExtend::LDR_DLL_NOTIFICATION_DATA* NotificationData, PVOID Context);

static SaveDataPatch::SaveData::tSaveData_Instance_Get g_SaveData_GetInstance_Func = nullptr;
SaveDataPatch::SaveData* Hook_SaveData_GetInstance(const MethodInfo*);

SaveDataPatch::SaveData* Hook_SaveData_GetInstance(const MethodInfo* method)
{
    HookUtils::InlineHook::UnHook(g_SaveData_GetInstance_Func, Hook_SaveData_GetInstance);

    SaveDataPatch::SaveData* savedata = g_SaveData_GetInstance_Func(method);

    //存档打补丁  一键解锁鉴赏模式
    savedata->Patch();

    return savedata;
}

DWORD WINAPI Process(PVOID dllBase)
{
    while (!il2cpp_is_vm_thread(nullptr))
    {
        ::Sleep(1u);
    }

    Il2CppDomain* domain = il2cpp_domain_get();
    Il2CppThread* thread = il2cpp_thread_attach(domain);

    //Hook 存档单例获取函数
    {
        Il2CppClass* cls = SaveDataPatch::SaveData::Class();

        const PropertyInfo* prop = il2cpp_class_get_property_from_name(cls, "Instance");
        const MethodInfo* method_get = il2cpp_property_get_get_method((PropertyInfo*)prop);

        g_SaveData_GetInstance_Func = (SaveDataPatch::SaveData::tSaveData_Instance_Get)method_get->methodPointer;

        HookUtils::InlineHook::Hook(g_SaveData_GetInstance_Func, Hook_SaveData_GetInstance);
    }

    il2cpp_thread_detach(thread);

    return 0u;
}

void Initialize(PVOID dllBase)
{
    IL2CppInitialize(dllBase);
    if (HANDLE hThread = ::CreateThread(nullptr, 0u, Process, dllBase, 0u, nullptr))
    {
        ::CloseHandle(hThread);
    }
}

VOID CALLBACK DllLoadNotification(ULONG NotificationReason, const NtdllExtend::LDR_DLL_NOTIFICATION_DATA* NotificationData, PVOID Context)
{
    bool* initialized = (bool*)Context;
    if (!*initialized)
    {
        if (NotificationReason == NtdllExtend::LDR_DLL_NOTIFICATION_REASON_LOADED)
        {
            const NtdllExtend::LDR_DLL_LOADED_NOTIFICATION_DATA* loaded = &NotificationData->Loaded;
            if (!::lstrcmpiW(loaded->BaseDllName->Buffer, L"GameAssembly.dll"))
            {
                Initialize(loaded->DllBase);
                *initialized = true;
            }
        }
    }
}

BOOL APIENTRY DllMain(HMODULE hModule, DWORD ul_reason_for_call, LPVOID lpReserved)
{
    UNREFERENCED_PARAMETER(lpReserved);
    switch (ul_reason_for_call)
    {
        case DLL_PROCESS_ATTACH:
        {
            NTSTATUS status = NtdllExtend::LdrRegisterDllNotification(0u, DllLoadNotification, &g_Initialized, &g_DllLoadNotificationHandle);
            if (!NT_SUCCESS(status))
            {
                return FALSE;
            }
            break;
        }
        case DLL_THREAD_ATTACH:
        {
            break;
        }
        case DLL_THREAD_DETACH:
        {
            break;
        }
        case DLL_PROCESS_DETACH:
        {
            if (g_DllLoadNotificationHandle)
            {
                NtdllExtend::LdrUnregisterDllNotification(g_DllLoadNotificationHandle);
            }
            break;
        }
    }
    return TRUE;
}


extern "C" __declspec(dllexport) void Dummy()
{
}
