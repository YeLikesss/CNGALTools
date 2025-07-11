#include "NtdllExtend.h"
#include "ExtendUtils.h"
#include "GameDotCheat.h"

#pragma comment(linker, "/MERGE:\".detourd=.data\"")
#pragma comment(linker, "/MERGE:\".detourc=.rdata\"")

static bool g_Initialized = false;
static void* g_DllLoadNotificationHandle = nullptr;

void Initialize(PVOID dllBase);
DWORD WINAPI Process(PVOID dllBase);
VOID CALLBACK DllLoadNotification(ULONG NotificationReason, const NtdllExtend::LDR_DLL_NOTIFICATION_DATA* NotificationData, PVOID Context);

static GameDotCheat::GameDotConnectUI::tGameDotConnectUI_Awake g_tGameDotConnectUI_Awake_Func = nullptr;
void Hook_GameDotConnectUI_Awake(GameDotCheat::GameDotConnectUI* obj, const MethodInfo* method);

void Hook_GameDotConnectUI_Awake(GameDotCheat::GameDotConnectUI* obj, const MethodInfo* method)
{
    HookUtils::InlineHook::UnHook(g_tGameDotConnectUI_Awake_Func, Hook_GameDotConnectUI_Awake);

#ifdef _DEBUG
    obj->Print();
#endif
    obj->Patch();

    g_tGameDotConnectUI_Awake_Func(obj, nullptr);
}


DWORD WINAPI Process(PVOID dllBase)
{
    while (!il2cpp_is_vm_thread(nullptr))
    {
        ::Sleep(1u);
    }

    Il2CppDomain* domain = il2cpp_domain_get();
    Il2CppThread* thread = il2cpp_thread_attach(domain);

    //连线关卡
    {
        Il2CppClass* cls = GameDotCheat::GameDotConnectUI::Class();
        const MethodInfo* method_awake = il2cpp_class_get_method_from_name(cls, "Awake", 0);

        g_tGameDotConnectUI_Awake_Func = (GameDotCheat::GameDotConnectUI::tGameDotConnectUI_Awake)method_awake->methodPointer;
        HookUtils::InlineHook::Hook(g_tGameDotConnectUI_Awake_Func, Hook_GameDotConnectUI_Awake);
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