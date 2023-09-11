
#include <Windows.h>
#include <string>
#include "Il2CppAPI.h"

#include "path.h"
#include "file.h"
#include "util.h"

static Il2CppDomain* g_Il2CppDomain = nullptr;
static Il2CppThread* g_Il2CppThread = nullptr;

__declspec(noinline)
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

__declspec(noinline)
Il2CppObject* Encoding_GetUTF8()
{
    Il2CppClass* encodingClass = Il2CppClassFromName(Il2CppGetCorlib(), "System.Text", "Encoding");
    const PropertyInfo* utf8EncoderProp = Il2CppClassGetPropertyFromName(encodingClass, "UTF8");
    const MethodInfo* utf8EncoderPropGet = Il2CppPropertyGetGetMethod((PropertyInfo*)utf8EncoderProp);

    //Encoding.UTF8
    return ((Il2CppObject * (*)(void))utf8EncoderPropGet->methodPointer)();
}

__declspec(noinline)
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
                        return swObject;
                    }
                }
            }
        }
    }
    return nullptr;
}

__declspec(noinline)
void TextWriter_Dispose(Il2CppObject* writer)
{
    Il2CppClass* textWriterClass = Il2CppClassFromName(Il2CppGetCorlib(), "System.IO", "TextWriter");
    const MethodInfo* textWriterDispose = Il2CppClassGetMethodFromName(textWriterClass, "Dispose", 0);

    //TextWriter.Dispose()
    ((void (*)(Il2CppObject*, const MethodInfo*))textWriterDispose->methodPointer)(writer, nullptr);
}

__declspec(noinline)
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

__declspec(noinline)
void JsonTextWriter_Close(Il2CppObject* jsonWriter)
{
    Il2CppImage* newtonImage = Il2CppAssaemblyGetImage(GetAssemblyByName("Newtonsoft.Json"));
    Il2CppClass* jsonTextWriterClass = Il2CppClassFromName(newtonImage, "Newtonsoft.Json", "JsonTextWriter");

    const MethodInfo* jsonTextWriterClose = Il2CppClassGetMethodFromName(jsonTextWriterClass, "Close", 0);

    //JsonTextWriter.Close()
    ((void (*)(Il2CppObject*, const MethodInfo*))jsonTextWriterClose->methodPointer)(jsonWriter, nullptr);
}

__declspec(noinline)
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

__declspec(noinline)
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

__declspec(noinline)
void SaveScriptJSON(Il2CppObject* ast, Il2CppObject* jsonWriter)
{
    Il2CppImage* mainImage = Il2CppAssaemblyGetImage(GetAssemblyByName("Assembly-CSharp"));
    Il2CppClass* projectHelperClass = Il2CppClassFromName(mainImage, "NVLMaker", "ProjectHelper");

    const MethodInfo* saveScriptJSONMethod = Il2CppClassGetMethodFromName(projectHelperClass, "SaveScriptJSON", 2);

    //SaveScriptJSON(SpritAST ast ,JsonWriter w)
    ((void (*)(Il2CppObject*, Il2CppObject*, const MethodInfo*))saveScriptJSONMethod->methodPointer)(ast, jsonWriter, nullptr);
}


__declspec(dllexport)
void WINAPI DumpScript(const wchar_t* fullpath) 
{
    std::wstring inPutPath(fullpath);
    std::wstring outPutPath = Path::ChangeExtension(inPutPath, L".json");
    File::Delete(outPutPath);

    //动态调用转化为json格式
    Il2CppArray* fileBuffer = File_ReadAllBytes(inPutPath);
    Il2CppObject* ast = LoadScriptBinary(fileBuffer);

    Il2CppObject* sw = StreamWriter_Create(outPutPath);
    Il2CppObject* jsonsw = JsonTextWriter_Create(sw);

    SaveScriptJSON(ast, jsonsw);

    JsonTextWriter_Close(jsonsw);
    TextWriter_Dispose(sw);

    Util::WriteDebugMessage(L"Dump Success ---> %s", Path::GetFileName(inPutPath).c_str());
}

__declspec(dllexport)
void WINAPI Initialize()
{
    while (!GetModuleHandleW(L"GameAssembly.dll"))
    {
        Sleep(1000);
    }

    //初始化Il2Cpp API
    if (Il2CppInitialize()) 
    {
        Util::WriteDebugMessage(L"Il2Cpp Initialized Success");

        g_Il2CppDomain = Il2CppDomainGet();
        g_Il2CppThread = Il2CppThreadAttach(g_Il2CppDomain);
    }
    else
    {
        Util::WriteDebugMessage(L"Il2Cpp Initialized Failed");
    }
}

__declspec(dllexport)
void WINAPI UnInitialize()
{
    if (g_Il2CppThread) 
    {
        Il2CppThreadDettach(g_Il2CppThread);

        g_Il2CppDomain = nullptr;
        g_Il2CppThread = nullptr;
    }
}

BOOL APIENTRY DllMain(HMODULE hModule, DWORD ul_reason_for_call, LPVOID lpReserved)
{
    UNREFERENCED_PARAMETER(lpReserved);
    switch (ul_reason_for_call)
    {
        case DLL_PROCESS_ATTACH:
        {
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
            break;
        }
    }
    return TRUE;
}
