
#include <Windows.h>
#include <string>
#include "Il2CppAPI.h"

#include "path.h"
#include "util.h"

static Il2CppDomain* g_Il2CppDomain = nullptr;
static Il2CppThread* g_Il2CppThread = nullptr;

static HMODULE g_DllBase = nullptr;
static std::wstring g_DllPath;
static std::wstring g_DllDirectory;

static std::wstring g_GamePath;
static std::wstring g_GameDirectory;

const Il2CppAssembly* GetAssemblyByName(const char* name)
{
    size_t count = 0;
    const Il2CppAssembly** assemblies = Il2CppDomainGetAssemblies(g_Il2CppDomain, &count);

    for (size_t i = 0; i < count; ++i)
    {
        if (strcmp(assemblies[i]->aname.name, name) == 0)
        {
            return assemblies[i];
        }
    }
    return nullptr;
}

Il2CppObject* Encoding_GetUTF8()
{
    Il2CppClass* encodingClass = Il2CppClassFromName(Il2CppGetCorlib(), "System.Text", "Encoding");
    const PropertyInfo* utf8EncoderProp = Il2CppClassGetPropertyFromName(encodingClass, "UTF8");
    const MethodInfo* utf8EncoderPropGet = Il2CppPropertyGetGetMethod((PropertyInfo*)utf8EncoderProp);

    //Encoding.UTF8
    return ((Il2CppObject * (*)(void))utf8EncoderPropGet->methodPointer)();
}

Il2CppObject* StreamWriter_Create(std::wstring& fullpath)
{
    Il2CppClass* streamWriterClass = Il2CppClassFromName(Il2CppGetCorlib(), "System.IO", "StreamWriter");

    Il2CppObject* swObject = Il2CppObjectNew(streamWriterClass);
    Il2CppString* p = Il2CppStringNewUTF16((const Il2CppChar*)fullpath.c_str(), fullpath.length());
    Il2CppObject* enc = Encoding_GetUTF8();

    void* iter = nullptr;
    while (const MethodInfo* method = Il2CppClassGetMethods(streamWriterClass, &iter))
    {
        if (strcmp(method->name, ".ctor") == 0 && method->parameters_count == 3)
        {
            const ParameterInfo* parameters = method->parameters;

            if (strcmp(parameters->name, "path") == 0) 
            {
                parameters++;

                if (strcmp(parameters->name, "append") == 0)
                {
                    parameters++;

                    if (strcmp(parameters->name, "encoding") == 0)
                    {
                        //new StreamWriter(string path, bool append, Encoding encoding)
                        ((void (*)(Il2CppObject*, Il2CppString*, bool, Il2CppObject*, const MethodInfo*))method->methodPointer)(swObject, p, false, enc, nullptr);
                    }
                }
            }
        }
    }
    return swObject;
}

void TextWriter_Dispose(Il2CppObject* writer)
{
    Il2CppClass* textWriterClass = Il2CppClassFromName(Il2CppGetCorlib(), "System.IO", "TextWriter");
    const MethodInfo* textWriterDispose = Il2CppClassGetMethodFromName(textWriterClass, "Dispose", 0);

    //TextWriter.Dispose()
    ((void (*)(Il2CppObject*, const MethodInfo*))textWriterDispose->methodPointer)(writer, nullptr);
}


Il2CppObject* JsonTextWriter_Create(Il2CppObject* textWriter)
{
    Il2CppImage* newtonImage = Il2CppAssaemblyGetImage(GetAssemblyByName("Newtonsoft.Json"));
    Il2CppClass* jsonTextWriterClass = Il2CppClassFromName(newtonImage, "Newtonsoft.Json", "JsonTextWriter");

    Il2CppObject* jtwObject = Il2CppObjectNew(jsonTextWriterClass);
    const MethodInfo* jsonTextWriterCtor = Il2CppClassGetMethodFromName(jsonTextWriterClass, ".ctor", 1);

    //new JsonTextWriter(TextWriter w)
    ((void (*)(Il2CppObject*, Il2CppObject*, const MethodInfo*))jsonTextWriterCtor->methodPointer)(jtwObject, textWriter, nullptr);

    return jtwObject;
}

void JsonTextWriter_Close(Il2CppObject* jsonWriter)
{
    Il2CppImage* newtonImage = Il2CppAssaemblyGetImage(GetAssemblyByName("Newtonsoft.Json"));
    Il2CppClass* jsonTextWriterClass = Il2CppClassFromName(newtonImage, "Newtonsoft.Json", "JsonTextWriter");

    const MethodInfo* jsonTextWriterClose = Il2CppClassGetMethodFromName(jsonTextWriterClass, "Close", 0);

    //JsonTextWriter.Close()
    ((void (*)(Il2CppObject*, const MethodInfo*))jsonTextWriterClose->methodPointer)(jsonWriter, nullptr);
}

Il2CppArray* File_ReadAllBytes(std::wstring& fullpath)
{
    HANDLE hFile = CreateFileW(fullpath.c_str(), GENERIC_READ, FILE_SHARE_READ, NULL, OPEN_EXISTING, FILE_ATTRIBUTE_ARCHIVE, NULL);
    if (hFile != INVALID_HANDLE_VALUE)
    {
        SetFilePointer(hFile, 0, NULL, FILE_BEGIN);
        DWORD fileSize = GetFileSize(hFile, NULL);

        Il2CppClass* byteClass = Il2CppClassFromName(Il2CppGetCorlib(), "System", "Byte");
        Il2CppArray* buffer = Il2CppArrayNew(byteClass, fileSize);

        DWORD fileRead = 0;
        ReadFile(hFile, buffer->vector, fileSize, &fileRead, NULL);

        CloseHandle(hFile);

        return buffer;
    }
    return nullptr;
}

Il2CppObject* LoadScriptBinary(Il2CppArray* buffer)
{
    Il2CppImage* mainImage = Il2CppAssaemblyGetImage(GetAssemblyByName("Assembly-CSharp"));
    Il2CppClass* projectHelperClass = Il2CppClassFromName(mainImage, "NVLMaker", "ProjectHelper");

    void* iter = nullptr;
    while (const MethodInfo* method = Il2CppClassGetMethods(projectHelperClass, &iter))
    {
        if (strcmp(method->name, "LoadScriptBinary") == 0 && method->parameters_count == 1)
        {
            if (strcmp(method->parameters->name, "bytes") == 0)
            {
                //LoadScriptBinary(byte[] bytes)
                return ((Il2CppObject * (*)(Il2CppArray*, const MethodInfo*))method->methodPointer)(buffer, nullptr);
            }
        }
    }
    return nullptr;
}

void SaveScriptJSON(Il2CppObject* ast, Il2CppObject* jsonWriter)
{
    Il2CppImage* mainImage = Il2CppAssaemblyGetImage(GetAssemblyByName("Assembly-CSharp"));
    Il2CppClass* projectHelperClass = Il2CppClassFromName(mainImage, "NVLMaker", "ProjectHelper");

    const MethodInfo* saveScriptJSONMethod = Il2CppClassGetMethodFromName(projectHelperClass, "SaveScriptJSON", 2);

    //SaveScriptJSON(SpritAST ast ,JsonWriter w)
    ((void (*)(Il2CppObject*, Il2CppObject*, const MethodInfo*))saveScriptJSONMethod->methodPointer)(ast, jsonWriter, nullptr);
}



void ProcessScript()
{
    WIN32_FIND_DATAW findFileData;
    HANDLE hListFile;

    std::wstring dirPath = g_DllDirectory + L"\\Script";
    std::wstring scanPath = dirPath + L"\\*.compiled.bytes";

    //查找第一个文件
    hListFile = FindFirstFileW(scanPath.c_str(), &findFileData);
    if(hListFile != INVALID_HANDLE_VALUE)
    {
        //遍历提取json格式的脚本ast
        do 
        {
            if (findFileData.dwFileAttributes & FILE_ATTRIBUTE_ARCHIVE) 
            {
                std::wstring inPutPath = dirPath + L"\\" + findFileData.cFileName;
                std::wstring outPutPath = Path::ChangeExtension(inPutPath, L".json");

                //动态调用转化为json格式
                Il2CppArray* fileBuffer = File_ReadAllBytes(inPutPath);
                Il2CppObject* ast = LoadScriptBinary(fileBuffer);

                Il2CppObject* sw = StreamWriter_Create(outPutPath);
                Il2CppObject* jsonsw = JsonTextWriter_Create(sw);

                SaveScriptJSON(ast, jsonsw);

                JsonTextWriter_Close(jsonsw);
                TextWriter_Dispose(sw);

                Util::WriteDebugMessage(L"Dump Success ---> %s", findFileData.cFileName);
            }
        } 
        while (FindNextFileW(hListFile, &findFileData));
        FindClose(hListFile);

        Util::WriteDebugMessage(L"Script Bytecode Dump Completed");
    }
    else
    {
        Util::WriteDebugMessage(L"Script File Can not Find");
    }
}


void Initialize()
{
    {
        std::wstring dllpath = Util::GetModulePathW(g_DllBase);
        std::wstring gamepath = Util::GetModulePathW(NULL);

        g_DllDirectory = std::move(Path::GetDirectoryName(dllpath));
        g_DllPath = std::move(dllpath);

        g_GameDirectory = std::move(Path::GetDirectoryName(gamepath));
        g_GamePath = std::move(gamepath);
    }

    while (!GetModuleHandleW(L"GameAssembly.dll"))
    {
        Sleep(1000);
    }

    //初始化Il2Cpp API
    if (Il2CppInitialize()) 
    {
        Util::WriteDebugMessage(L"Il2Cpp Initialized Success");

        Sleep(5000);  //等待游戏il2cpp代码初始化完毕

        g_Il2CppDomain = Il2CppDomainGet();
        g_Il2CppThread = Il2CppThreadAttach(g_Il2CppDomain);

        ProcessScript();

        Il2CppThreadDettach(g_Il2CppThread);

        g_Il2CppDomain = nullptr;
        g_Il2CppThread = nullptr;
    }
    else
    {
        Util::WriteDebugMessage(L"Il2Cpp Initialized Failed");
    }
}



BOOL APIENTRY DllMain(HMODULE hModule, DWORD ul_reason_for_call, LPVOID lpReserved)
{
    UNREFERENCED_PARAMETER(lpReserved);
    switch (ul_reason_for_call)
    {
    case DLL_PROCESS_ATTACH:
    {
        g_DllBase = hModule;
        CreateThread(NULL, 0, (LPTHREAD_START_ROUTINE)Initialize, NULL, 0, NULL);
        break;
    }
    case DLL_THREAD_ATTACH:
    case DLL_THREAD_DETACH:
    case DLL_PROCESS_DETACH:
        break;
    }
    return TRUE;
}

extern "C" __declspec(dllexport) void Dummy() { }