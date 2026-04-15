#pragma once

template<unsigned __int32 Size>
class InlineStringW
{
public:
    wchar_t String[Size];

    InlineStringW() = delete;
    constexpr __forceinline InlineStringW(const wchar_t* s) : String{}
    {
        for (unsigned __int32 i = 0; i < Size; ++i)
        {
            this->String[i] = s[i];
        }
    }
};

#define InlineUnicodeString(name, s) constexpr InlineStringW<sizeof(L###s) / sizeof(wchar_t)> name = InlineStringW<sizeof(L###s) / sizeof(wchar_t)>(L###s)
