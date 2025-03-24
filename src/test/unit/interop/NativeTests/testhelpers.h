// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#pragma once

#include <assert.h>
#include <stdio.h>
#include <sstream>
#include <type_traits>
#include <Windows.h>

#define TEST extern "C" __declspec(dllexport)

template<typename T, typename = std::enable_if_t<std::is_floating_point<T>::value>>
static inline bool is_essentially_equal_to(const T& a, const T& b)
{
    if (std::isnan(a))
    {
        return std::isnan(b);
    }
    else if (std::isnan(b))
    {
        return false;
    }

    return std::abs(a - b) <= std::numeric_limits<T>::epsilon() * std::fmin(std::abs(a), std::abs(b));
}

template< typename... Args >
std::wstring format(const wchar_t* format, Args... args)
{
    int length = std::swprintf(nullptr, 0, format, args...);
    // If this fails, let the program crash.
    wchar_t* buf = new wchar_t[length + 1];
    // cpp/non-constant-format
    std::swprintf(buf, length + 1, format, args...); // CodeQL [SM01734] : This is a test code and the format string is trusted.

    std::wstring str(buf);
    delete[] buf;
    return str;
}

static void printAssertionFailure(std::wstringstream& output, const char* file, const char* function, int line = __LINE__)
{
    output << format(L"Assertion failure: file %hs in %hs, line %d", file, function, line) << std::endl;
}

#define printLine() printAssertionFailure(output, __FILE__, __FUNCTION__, __LINE__);

#define assertEqualInt(expected, actual) \
if ((expected) != (actual)) \
{ \
    printLine(); \
    output << format(L"Expected: %d\n", (int)((expected))); \
    output << format(L"Actual:   %d\n", (int)((actual))); \
    return E_FAIL; \
}

#define assertEqualBool(expected, actual) \
if ((BOOL)(expected) != (BOOL)(actual)) \
{ \
    printLine(); \
    if ((expected)) \
    { \
        output << format(L"Expected: TRUE\n"); \
        output << format(L"Actual: FALSE\n"); \
    } \
    else \
    { \
        output << format(L"Expected: FALSE\n"); \
        output << format(L"Actual: TRUE\n"); \
    } \
    return E_FAIL; \
}

#define assertEqualFloat(expected, actual) \
if (!is_essentially_equal_to((float)((expected)), (float)((actual)))) \
{ \
    printLine(); \
    output << format(L"Expected: %f\n", (float)((expected))); \
    output << format(L"Actual:   %f\n", (float)((actual))); \
    return E_FAIL; \
}

#define assertEqualDouble(expected, actual) \
if (!is_essentially_equal_to((double)((expected)), (double)((actual)))) \
{ \
    printLine(); \
    output << format(L"Expected: %f\n", (double)((expected))); \
    output << format(L"Actual:   %f\n", (double)((actual))); \
    return E_FAIL; \
}

#define assertEqualHr(expected, actual) \
if ((int)((expected)) != (int)((actual))) \
{ \
    printLine(); \
    output << format(L"Expected: 0x%08X\n", (int)((expected))); \
    output << format(L"Actual:   0x%08X\n", (int)((actual))); \
    return E_FAIL; \
}

#define assertEqualWString(expected, actual) \
if (!(expected)) \
{ \
    if ((actual)) \
    { \
        output << format(L"Expected: NULL\n"); \
        output << format(L"Actual:   %s\n", (const wchar_t*)((actual))); \
        return E_FAIL; \
    } \
} \
else if (!(actual)) \
{ \
    output << format(L"Expected:   %s\n", (const wchar_t*)((expected))); \
    output << format(L"Actual: NULL\n"); \
    return E_FAIL; \
} \
else if (wcscmp((const wchar_t*)(expected), (const wchar_t*)(actual)) != 0) \
{ \
    printLine(); \
    output << format(L"Expected: %s\n", (const wchar_t*)((expected))); \
    output << format(L"Actual:   %s\n", (const wchar_t*)((actual))); \
    return E_FAIL; \
}

#define assertNull(actual) \
if ((actual)) \
{ \
    printLine(); \
    output << format(L"Actual: %p\n", ((actual))); \
    return E_FAIL; \
}

#define assertNotNull(actual) \
if (!(actual) || (void*)((actual)) == (void*)(long)0xdeadbeef) \
{ \
    printLine(); \
    output << format(L"Actual: %p\n", ((actual))); \
    return E_FAIL; \
}

template<typename T>
static const WCHAR* RunTest(T method)
{
    std::wstringstream output;
    HRESULT hr = method(output);

    if (hr == S_OK)
    {
        output << L"Success";
    }
    return Duplicate(output.str().c_str());
}

// Need to allocate this as CoTaskMemAlloc to be interpreted by interop marshallers.
static const WCHAR* Duplicate(const WCHAR* source)
{
    size_t length = wcslen(source) + 1;
    WCHAR* clone = (WCHAR*)CoTaskMemAlloc(length * sizeof(WCHAR));
    wcscpy_s(clone, length, source);
    return clone;
}
