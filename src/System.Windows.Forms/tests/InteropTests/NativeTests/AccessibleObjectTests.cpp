// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#include "testhelpers.h"
#include <cassert>
#include <UIAutomation.h>
#include <atlbase.h>
using namespace ATL;

#define COR_E_NOTSUPPORTED 0x80131515

TEST const WCHAR* WINAPI Test_IAccessibleExConvertReturnedElement(IUnknown* pUnknown)
{
    return RunTest([&](std::wstringstream& output)
    {
        HRESULT hr;
        CComPtr<IAccessibleEx> pAccessibleEx;
        CComPtr<IRawElementProviderSimple> pIn;

        hr = pUnknown->QueryInterface(IID_IAccessibleEx, (void**)&pAccessibleEx);
        assertEqualHr(S_OK, hr);
        hr = pUnknown->QueryInterface(IID_IRawElementProviderSimple, (void**)&pIn);
        assertEqualHr(S_OK, hr);

        CComPtr<IAccessibleEx> result;
        hr = pAccessibleEx->ConvertReturnedElement(pIn, &result);
        assertEqualHr(E_NOTIMPL, hr);
        assertNull(result.p);

        // Negative tests.
        hr = pAccessibleEx->ConvertReturnedElement(NULL, &result);
        assertEqualHr(E_NOTIMPL, hr);

        hr = pAccessibleEx->ConvertReturnedElement(pIn, NULL);
        assertEqualHr(E_POINTER, hr);
        assertNull(result.p);

        hr = pAccessibleEx->ConvertReturnedElement(NULL, NULL);
        assertEqualHr(E_POINTER, hr);

        return S_OK;
    });
}

TEST const WCHAR* WINAPI Test_IAccessibleExGetIAccessiblePair(IUnknown* pUnknown)
{
    return RunTest([&](std::wstringstream& output)
    {
        HRESULT hr;
        CComPtr<IAccessibleEx> pAccessibleEx;

        hr = pUnknown->QueryInterface(IID_IAccessibleEx, (void**)&pAccessibleEx);
        assertEqualHr(S_OK, hr);

        long idChild = 1;
        CComPtr<IAccessible> result;
        hr = pAccessibleEx->GetIAccessiblePair(&result, &idChild);
        assertEqualHr(S_OK, hr);
        assertNotNull(result.p);
        assertEqualInt(0, idChild);

        // Negative tests.
        idChild = 1;
        hr = pAccessibleEx->GetIAccessiblePair(NULL, &idChild);

        assertEqualHr(E_POINTER, hr);
        assertEqualInt(1, idChild);

        result = NULL;
        hr = pAccessibleEx->GetIAccessiblePair(&result, NULL);
        assertEqualHr(E_POINTER, hr);
        assertNull(result.p);

        hr = pAccessibleEx->GetIAccessiblePair(NULL, NULL);
        assertEqualHr(E_POINTER, hr);

        return S_OK;
    });
}

TEST const WCHAR* WINAPI Test_IAccessibleExGetRuntimeId(IUnknown* pUnknown, int* expected)
{
    return RunTest([&](std::wstringstream& output)
    {
        HRESULT hr;
        CComPtr<IAccessibleEx> pAccessibleEx;

        hr = pUnknown->QueryInterface(IID_IAccessibleEx, (void**)&pAccessibleEx);
        assertEqualHr(S_OK, hr);

        SAFEARRAY *result;
        hr = pAccessibleEx->GetRuntimeId(&result);
        assertEqualHr(COR_E_NOTSUPPORTED, hr);
        SafeArrayDestroy(result);

        // Negative tests.
        hr = pAccessibleEx->GetRuntimeId(NULL);
        assertEqualHr(E_POINTER, hr);

        return S_OK;
    });
}

TEST const WCHAR* WINAPI Test_IAccessibleExGetObjectForChild(IUnknown* pUnknown, long idChild)
{
    return RunTest([&](std::wstringstream& output)
    {
        HRESULT hr;
        CComPtr<IAccessibleEx> pAccessibleEx;

        hr = pUnknown->QueryInterface(IID_IAccessibleEx, (void**)&pAccessibleEx);
        assertEqualHr(S_OK, hr);

        long idChild = 1;
        CComPtr<IAccessibleEx> result;
        hr = pAccessibleEx->GetObjectForChild(idChild, &result);
        assertEqualHr(S_OK, hr);
        assertNull(result.p);

        // Negative tests.
        hr = pAccessibleEx->GetObjectForChild(idChild, NULL);
        assertEqualHr(E_POINTER, hr);

        return S_OK;
    });
}

TEST const WCHAR* WINAPI Test_IServiceProviderQueryService(IUnknown* pUnknown)
{
    return RunTest([&](std::wstringstream& output)
    {
        HRESULT hr;
        CComPtr<IServiceProvider> pServiceProvider;

        hr = pUnknown->QueryInterface(IID_IServiceProvider, (void**)&pServiceProvider);
        assertEqualHr(S_OK, hr);

        GUID service = { 0 };
        GUID riid = { 0 };
        void* pObject = NULL;

        hr = pServiceProvider->QueryService(service, riid, &pObject);
        assertEqualHr(E_NOINTERFACE, hr);

        service = IID_IAccessibleEx;
        riid = { 0 };
        hr = pServiceProvider->QueryService(service, riid, &pObject);
        assertEqualHr(E_NOINTERFACE, hr);

        service = { 0 };
        riid = IID_IAccessibleEx;
        hr = pServiceProvider->QueryService(service, riid, &pObject);
        assertEqualHr(E_NOINTERFACE, hr);

        service = IID_IAccessibleEx;
        riid = IID_IAccessibleEx;
        hr = pServiceProvider->QueryService(service, riid, &pObject);
        assertEqualHr(E_NOINTERFACE, hr);

        // Negative tests.
        hr = pServiceProvider->QueryService(service, riid, NULL);
        assertEqualHr(E_POINTER, hr);

        return S_OK;
    });
}

TEST const WCHAR* WINAPI Test_IRawElementProviderSimpleHostRawElementProvider(IUnknown* pUnknown, BOOL expected)
{
    return RunTest([&](std::wstringstream& output)
    {
        HRESULT hr;
        CComPtr<IRawElementProviderSimple> pRawElementProviderSimple;

        hr = pUnknown->QueryInterface(IID_IRawElementProviderSimple, (void**)&pRawElementProviderSimple);
        assertEqualHr(S_OK, hr);

        CComPtr<IRawElementProviderSimple> result;
        hr = pRawElementProviderSimple->get_HostRawElementProvider(&result);
        assertEqualHr(S_OK, hr);
        assertEqualBool(expected, result != NULL);

        // Negative tests.
        hr = pRawElementProviderSimple->get_HostRawElementProvider(NULL);
        assertEqualHr(E_POINTER, hr);

        return S_OK;
    });
}

TEST const WCHAR* WINAPI Test_IRawElementProviderSimpleProviderOptions(IUnknown* pUnknown, ProviderOptions expected)
{
    return RunTest([&](std::wstringstream& output)
    {
        HRESULT hr;
        CComPtr<IRawElementProviderSimple> pRawElementProviderSimple;

        hr = pUnknown->QueryInterface(IID_IRawElementProviderSimple, (void**)&pRawElementProviderSimple);
        assertEqualHr(S_OK, hr);

        ProviderOptions result;
        hr = pRawElementProviderSimple->get_ProviderOptions(&result);
        assertEqualHr(S_OK, hr);
        assertEqualInt(expected, result);

        // Negative tests.
        hr = pRawElementProviderSimple->get_ProviderOptions(NULL);
        assertEqualHr(E_POINTER, hr);

        return S_OK;
    });
}

TEST const WCHAR* WINAPI Test_IRawElementProviderSimpleGetPatternProvider(IUnknown* pUnknown, PATTERNID patternId, BOOL expected)
{
    return RunTest([&](std::wstringstream& output)
    {
        HRESULT hr;
        CComPtr<IRawElementProviderSimple> pRawElementProviderSimple;

        hr = pUnknown->QueryInterface(IID_IRawElementProviderSimple, (void**)&pRawElementProviderSimple);
        assertEqualHr(S_OK, hr);

        CComPtr<IUnknown> pResult;
        hr = pRawElementProviderSimple->GetPatternProvider(patternId, &pResult);
        assertEqualHr(S_OK, hr);
        if (expected)
        {
            assert(pResult == pUnknown);
        }
        else
        {
            assertNull(pResult.p);
        }

        // Negative tests.
        hr = pRawElementProviderSimple->GetPatternProvider(UIA_DockPatternId, NULL);
        assertEqualHr(E_POINTER, hr);

        return S_OK;
    });
}

TEST const WCHAR* WINAPI Test_IRawElementProviderSimpleGetPropertyValue(IUnknown* pUnknown, PATTERNID patternId, VARIANT* result)
{
    return RunTest([&](std::wstringstream& output)
    {
        HRESULT hr;
        CComPtr<IRawElementProviderSimple> pRawElementProviderSimple;

        hr = pUnknown->QueryInterface(IID_IRawElementProviderSimple, (void**)&pRawElementProviderSimple);
        assertEqualHr(S_OK, hr);

        hr = pRawElementProviderSimple->GetPropertyValue(patternId, result);
        assertEqualHr(S_OK, hr);

        // Negative tests.
        hr = pRawElementProviderSimple->GetPropertyValue(UIA_DockPatternId, NULL);
        assertEqualHr(E_POINTER, hr);

        return S_OK;
    });
}

TEST const WCHAR* WINAPI Test_IRawElementProviderFragmentGetBoundingRectangle(IUnknown* pUnknown, UiaRect expected)
{
    return RunTest([&](std::wstringstream& output)
    {
        HRESULT hr;
        CComPtr<IRawElementProviderFragment> pRawElementProviderFragment;

        hr = pUnknown->QueryInterface(IID_IRawElementProviderFragment, (void**)&pRawElementProviderFragment);
        assertEqualHr(S_OK, hr);

        UiaRect result;
        hr = pRawElementProviderFragment->get_BoundingRectangle(&result);
        assertEqualHr(S_OK, hr);
        assertEqualDouble(expected.left, result.left);
        assertEqualDouble(expected.top, result.top);
        assertEqualDouble(expected.width, result.width);
        assertEqualDouble(expected.height, result.height);

        // Negative tests.
        hr = pRawElementProviderFragment->get_BoundingRectangle(NULL);
        assertEqualHr(E_POINTER, hr);

        return S_OK;
    });
}

TEST const WCHAR* WINAPI Test_IRawElementProviderFragmentGetFragmentRoot(IUnknown* pUnknown)
{
    return RunTest([&](std::wstringstream& output)
    {
        HRESULT hr;
        CComPtr<IRawElementProviderFragment> pRawElementProviderFragment;

        hr = pUnknown->QueryInterface(IID_IRawElementProviderFragment, (void**)&pRawElementProviderFragment);
        assertEqualHr(S_OK, hr);

        CComPtr<IRawElementProviderFragmentRoot> result;
        hr = pRawElementProviderFragment->get_FragmentRoot(&result);
        assertEqualHr(S_OK, hr);
        assertNull(result.p);

        // Negative tests.
        hr = pRawElementProviderFragment->get_FragmentRoot(NULL);
        assertEqualHr(E_POINTER, hr);

        return S_OK;
    });
}

TEST const WCHAR* WINAPI Test_IRawElementProviderFragmentGetEmbeddedFragmentRoots(IUnknown* pUnknown)
{
    return RunTest([&](std::wstringstream& output)
    {
        HRESULT hr;
        CComPtr<IRawElementProviderFragment> pRawElementProviderFragment;

        hr = pUnknown->QueryInterface(IID_IRawElementProviderFragment, (void**)&pRawElementProviderFragment);
        assertEqualHr(S_OK, hr);

        SAFEARRAY *result = (SAFEARRAY*)(long)0xdeadbeef;
        hr = pRawElementProviderFragment->GetEmbeddedFragmentRoots(&result);
        assertEqualHr(S_OK, hr);
        assertNull(result);
        SafeArrayDestroy(result);

        // Negative tests.
        hr = pRawElementProviderFragment->GetEmbeddedFragmentRoots(NULL);
        assertEqualHr(E_POINTER, hr);

        return S_OK;
    });
}

TEST const WCHAR* WINAPI Test_IRawElementProviderFragmentGetRuntimeId(IUnknown* pUnknown, int* expected)
{
    return RunTest([&](std::wstringstream& output)
    {
        HRESULT hr;
        CComPtr<IRawElementProviderFragment> pRawElementProviderFragment;

        hr = pUnknown->QueryInterface(IID_IRawElementProviderFragment, (void**)&pRawElementProviderFragment);
        assertEqualHr(S_OK, hr);
        SAFEARRAY *result;
        hr = pRawElementProviderFragment->GetRuntimeId(&result);
        assertEqualHr(S_OK, hr);

        // Negative tests.
        hr = pRawElementProviderFragment->GetRuntimeId(NULL);
        assertEqualHr(E_POINTER, hr);

        return S_OK;
    });
}

TEST const WCHAR* WINAPI Test_IRawElementProviderFragmentNavigate(IUnknown* pUnknown, NavigateDirection direction)
{
    return RunTest([&](std::wstringstream& output)
    {
        HRESULT hr;
        CComPtr<IRawElementProviderFragment> pRawElementProviderFragment;

        hr = pUnknown->QueryInterface(IID_IRawElementProviderFragment, (void**)&pRawElementProviderFragment);
        assertEqualHr(S_OK, hr);

        CComPtr<IRawElementProviderFragment> result;
        hr = pRawElementProviderFragment->Navigate(direction, &result);
        assertEqualHr(S_OK, hr);
        assertNull(result.p);

        // Negative tests.
        hr = pRawElementProviderFragment->Navigate(NavigateDirection_Parent, NULL);
        assertEqualHr(E_POINTER, hr);

        return S_OK;
    });
}

TEST const WCHAR* WINAPI Test_IRawElementProviderFragmentSetFocus(IUnknown* pUnknown)
{
    return RunTest([&](std::wstringstream& output)
    {
        HRESULT hr;
        CComPtr<IRawElementProviderFragment> pRawElementProviderFragment;

        hr = pUnknown->QueryInterface(IID_IRawElementProviderFragment, (void**)&pRawElementProviderFragment);
        assertEqualHr(S_OK, hr);

        hr = pRawElementProviderFragment->SetFocus();
        assertEqualHr(S_OK, hr);

        return S_OK;
    });
}

TEST const WCHAR* WINAPI Test_IRawElementProviderFragmentRootElementProviderFromPoint(IUnknown* pUnknown, double x, double y)
{
    return RunTest([&](std::wstringstream& output)
    {
        HRESULT hr;
        CComPtr<IRawElementProviderFragmentRoot> pRawElementProviderFragment;
        CComPtr<IRawElementProviderFragmentRoot> pRawElementProviderFragmentRoot;

        hr = pUnknown->QueryInterface(IID_IRawElementProviderFragment, (void**)&pRawElementProviderFragment);
        assertEqualHr(S_OK, hr);

        hr = pUnknown->QueryInterface(IID_IRawElementProviderFragmentRoot, (void**)&pRawElementProviderFragmentRoot);
        assertEqualHr(S_OK, hr);

        CComPtr<IRawElementProviderFragment> result;
        hr = pRawElementProviderFragmentRoot->ElementProviderFromPoint(x, y, &result);
        assertEqualHr(S_OK, hr);
        assertNotNull(result.p);

        // Negative tests.
        hr = pRawElementProviderFragmentRoot->ElementProviderFromPoint(0, 0, NULL);
        assertEqualHr(E_POINTER, hr);

        return S_OK;
    });
}

TEST const WCHAR* WINAPI Test_IRawElementProviderFragmentRootGetFocus(IUnknown* pUnknown)
{
    return RunTest([&](std::wstringstream& output)
    {
        HRESULT hr;
        CComPtr<IRawElementProviderFragmentRoot> pRawElementProviderFragmentRoot;

        hr = pUnknown->QueryInterface(IID_IRawElementProviderFragmentRoot, (void**)&pRawElementProviderFragmentRoot);
        assertEqualHr(S_OK, hr);

        CComPtr<IRawElementProviderFragment> result;
        hr = pRawElementProviderFragmentRoot->GetFocus(&result);
        assertEqualHr(S_OK, hr);
        assertNull(result.p);

        // Negative tests.
        hr = pRawElementProviderFragmentRoot->GetFocus(NULL);
        assertEqualHr(E_POINTER, hr);

        return S_OK;
    });
}

TEST const WCHAR* WINAPI Test_IInvokeProviderInvoke(IUnknown* pUnknown)
{
    return RunTest([&](std::wstringstream& output)
    {
        HRESULT hr;
        CComPtr<IInvokeProvider> pInvokeProvider;

        hr = pUnknown->QueryInterface(IID_IInvokeProvider, (void**)&pInvokeProvider);
        assertEqualHr(S_OK, hr);

        hr = pInvokeProvider->Invoke();
        assertEqualHr(S_OK, hr);

        return S_OK;
    });
}

TEST const WCHAR* WINAPI Test_IValueProviderGetIsReadOnly(IUnknown* pUnknown, BOOL expected)
{
    return RunTest([&](std::wstringstream& output)
    {
        HRESULT hr;
        CComPtr<IValueProvider> pRangeValueProvider;

        hr = pUnknown->QueryInterface(IID_IValueProvider, (void**)&pRangeValueProvider);
        assertEqualHr(S_OK, hr);

        BOOL result;
        hr = pRangeValueProvider->get_IsReadOnly(&result);
        assertEqualHr(S_OK, hr);
        assertEqualInt(expected, result);

        // Negative tests.
        hr = pRangeValueProvider->get_IsReadOnly(NULL);
        assertEqualHr(E_POINTER, hr);

        return S_OK;
    });
}

TEST const WCHAR* WINAPI Test_IValueProviderGetValue(IUnknown* pUnknown, LPCWSTR expected)
{
    return RunTest([&](std::wstringstream& output)
    {
        HRESULT hr;
        CComPtr<IValueProvider> pRangeValueProvider;

        hr = pUnknown->QueryInterface(IID_IValueProvider, (void**)&pRangeValueProvider);
        assertEqualHr(S_OK, hr);

        BSTR result;
        hr = pRangeValueProvider->get_Value(&result);
        assertEqualHr(S_OK, hr);
        assertEqualWString(expected, result);
        SysFreeString(result);

        // Negative tests.
        hr = pRangeValueProvider->get_Value(NULL);
        assertEqualHr(E_POINTER, hr);

        return S_OK;
    });
}

TEST const WCHAR* WINAPI Test_IValueProviderSetValue(IUnknown* pUnknown, LPCWSTR value, LPCWSTR expected)
{
    return RunTest([&](std::wstringstream& output)
    {
        HRESULT hr;
        CComPtr<IValueProvider> pRangeValueProvider;

        hr = pUnknown->QueryInterface(IID_IValueProvider, (void**)&pRangeValueProvider);
        assertEqualHr(S_OK, hr);

        hr = pRangeValueProvider->SetValue(value);
        assertEqualHr(S_OK, hr);

        BSTR result;
        hr = pRangeValueProvider->get_Value(&result);
        assertEqualHr(S_OK, hr);
        assertEqualWString(expected, result);
        SysFreeString(result);

        // Negative tests
        hr = pRangeValueProvider->SetValue(NULL);
        assertEqualHr(S_OK, hr);

        return S_OK;
    });
}
TEST const WCHAR* WINAPI Test_IRangeValueProviderGetIsReadOnly(IUnknown* pUnknown, BOOL expected)
{
    return RunTest([&](std::wstringstream& output)
    {
        HRESULT hr;
        CComPtr<IRangeValueProvider> pRangeValueProvider;

        hr = pUnknown->QueryInterface(IID_IRangeValueProvider, (void**)&pRangeValueProvider);
        assertEqualHr(S_OK, hr);

        BOOL result;
        hr = pRangeValueProvider->get_IsReadOnly(&result);
        assertEqualHr(S_OK, hr);
        assertEqualInt(expected, result);

        // Negative tests.
        hr = pRangeValueProvider->get_IsReadOnly(NULL);
        assertEqualHr(E_POINTER, hr);

        return S_OK;
    });
}

TEST const WCHAR* WINAPI Test_IRangeValueProviderGetLargeChange(IUnknown* pUnknown, double expected)
{
    return RunTest([&](std::wstringstream& output)
    {
        HRESULT hr;
        CComPtr<IRangeValueProvider> pRangeValueProvider;

        hr = pUnknown->QueryInterface(IID_IRangeValueProvider, (void**)&pRangeValueProvider);
        assertEqualHr(S_OK, hr);

        double result;
        hr = pRangeValueProvider->get_LargeChange(&result);
        assertEqualHr(S_OK, hr);
        assertEqualDouble(expected, result);

        // Negative tests.
        hr = pRangeValueProvider->get_LargeChange(NULL);
        assertEqualHr(E_POINTER, hr);

        return S_OK;
    });
}

TEST const WCHAR* WINAPI Test_IRangeValueProviderGetMaximum(IUnknown* pUnknown, double expected)
{
    return RunTest([&](std::wstringstream& output)
    {
        HRESULT hr;
        CComPtr<IRangeValueProvider> pRangeValueProvider;

        hr = pUnknown->QueryInterface(IID_IRangeValueProvider, (void**)&pRangeValueProvider);
        assertEqualHr(S_OK, hr);

        double result;
        hr = pRangeValueProvider->get_Maximum(&result);
        assertEqualHr(S_OK, hr);
        assertEqualDouble(expected, result);

        // Negative tests.
        hr = pRangeValueProvider->get_Maximum(NULL);
        assertEqualHr(E_POINTER, hr);

        return S_OK;
    });
}

TEST const WCHAR* WINAPI Test_IRangeValueProviderGetMinimum(IUnknown* pUnknown, double expected)
{
    return RunTest([&](std::wstringstream& output)
    {
        HRESULT hr;
        CComPtr<IRangeValueProvider> pRangeValueProvider;

        hr = pUnknown->QueryInterface(IID_IRangeValueProvider, (void**)&pRangeValueProvider);
        assertEqualHr(S_OK, hr);

        double result;
        hr = pRangeValueProvider->get_Minimum(&result);
        assertEqualHr(S_OK, hr);
        assertEqualDouble(expected, result);

        // Negative tests.
        hr = pRangeValueProvider->get_Minimum(NULL);
        assertEqualHr(E_POINTER, hr);

        return S_OK;
    });
}

TEST const WCHAR* WINAPI Test_IRangeValueProviderGetSmallChange(IUnknown* pUnknown, double expected)
{
    return RunTest([&](std::wstringstream& output)
    {
        HRESULT hr;
        CComPtr<IRangeValueProvider> pRangeValueProvider;

        hr = pUnknown->QueryInterface(IID_IRangeValueProvider, (void**)&pRangeValueProvider);
        assertEqualHr(S_OK, hr);

        double result;
        hr = pRangeValueProvider->get_SmallChange(&result);
        assertEqualHr(S_OK, hr);
        assertEqualDouble(expected, result);

        // Negative tests.
        hr = pRangeValueProvider->get_SmallChange(NULL);
        assertEqualHr(E_POINTER, hr);

        return S_OK;
    });
}

TEST const WCHAR* WINAPI Test_IRangeValueProviderGetValue(IUnknown* pUnknown, double expected)
{
    return RunTest([&](std::wstringstream& output)
    {
        HRESULT hr;
        CComPtr<IRangeValueProvider> pRangeValueProvider;

        hr = pUnknown->QueryInterface(IID_IRangeValueProvider, (void**)&pRangeValueProvider);
        assertEqualHr(S_OK, hr);

        double result;
        hr = pRangeValueProvider->get_Value(&result);
        assertEqualHr(S_OK, hr);
        assertEqualDouble(expected, result);

        // Negative tests.
        hr = pRangeValueProvider->get_Value(NULL);
        assertEqualHr(E_POINTER, hr);

        return S_OK;
    });
}

TEST const WCHAR* WINAPI Test_IRangeValueProviderSetValue(IUnknown* pUnknown, double value, double expected)
{
    return RunTest([&](std::wstringstream& output)
    {
        HRESULT hr;
        CComPtr<IRangeValueProvider> pRangeValueProvider;

        hr = pUnknown->QueryInterface(IID_IRangeValueProvider, (void**)&pRangeValueProvider);
        assertEqualHr(S_OK, hr);

        hr = pRangeValueProvider->SetValue(value);
        assertEqualHr(S_OK, hr);

        double result;
        hr = pRangeValueProvider->get_Value(&result);
        assertEqualHr(S_OK, hr);
        assertEqualDouble(expected, result);

        return S_OK;
    });
}

TEST const WCHAR* WINAPI Test_IExpandCollapseProviderGetExpandCollapseState(IUnknown* pUnknown, ExpandCollapseState expected)
{
    return RunTest([&](std::wstringstream& output)
    {
        HRESULT hr;
        CComPtr<IExpandCollapseProvider> pExpandCollapseProvider;

        hr = pUnknown->QueryInterface(IID_IExpandCollapseProvider, (void**)&pExpandCollapseProvider);
        assertEqualHr(S_OK, hr);

        ExpandCollapseState result;
        hr = pExpandCollapseProvider->get_ExpandCollapseState(&result);
        assertEqualHr(S_OK, hr);
        assertEqualInt(expected, result);

        // Negative tests.
        hr = pExpandCollapseProvider->get_ExpandCollapseState(NULL);
        assertEqualHr(E_POINTER, hr);

        return S_OK;
    });
}

TEST const WCHAR* WINAPI Test_IExpandCollapseProviderCollapse(IUnknown* pUnknown, ExpandCollapseState expected)
{
    return RunTest([&](std::wstringstream& output)
    {
        HRESULT hr;
        CComPtr<IExpandCollapseProvider> pExpandCollapseProvider;

        hr = pUnknown->QueryInterface(IID_IExpandCollapseProvider, (void**)&pExpandCollapseProvider);
        assertEqualHr(S_OK, hr);

        hr = pExpandCollapseProvider->Collapse();
        assertEqualHr(S_OK, hr);

        ExpandCollapseState result;
        hr = pExpandCollapseProvider->get_ExpandCollapseState(&result);
        assertEqualHr(S_OK, hr);
        assertEqualInt(expected, result);

        // Call again.
        hr = pExpandCollapseProvider->Collapse();
        assertEqualHr(S_OK, hr);

        hr = pExpandCollapseProvider->get_ExpandCollapseState(&result);
        assertEqualHr(S_OK, hr);
        assertEqualInt(expected, result);

        return S_OK;
    });
}

TEST const WCHAR* WINAPI Test_IExpandCollapseProviderExpand(IUnknown* pUnknown, ExpandCollapseState expected)
{
    return RunTest([&](std::wstringstream& output)
    {
        HRESULT hr;
        CComPtr<IExpandCollapseProvider> pExpandCollapseProvider;

        hr = pUnknown->QueryInterface(IID_IExpandCollapseProvider, (void**)&pExpandCollapseProvider);
        assertEqualHr(S_OK, hr);

        hr = pExpandCollapseProvider->Expand();
        assertEqualHr(S_OK, hr);

        ExpandCollapseState result;
        hr = pExpandCollapseProvider->get_ExpandCollapseState(&result);
        assertEqualHr(S_OK, hr);
        assertEqualInt(expected, result);

        // Call again.
        hr = pExpandCollapseProvider->Expand();
        assertEqualHr(S_OK, hr);

        hr = pExpandCollapseProvider->get_ExpandCollapseState(&result);
        assertEqualHr(S_OK, hr);
        assertEqualInt(expected, result);

        return S_OK;
    });
}

TEST const WCHAR* WINAPI Test_IToggleProviderGetToggleState(IUnknown* pUnknown, ToggleState expected)
{
    return RunTest([&](std::wstringstream& output)
    {
        HRESULT hr;
        CComPtr<IToggleProvider> pToggleProvider;

        hr = pUnknown->QueryInterface(IID_IToggleProvider, (void**)&pToggleProvider);
        assertEqualHr(S_OK, hr);

        ToggleState result;
        hr = pToggleProvider->get_ToggleState(&result);
        assertEqualHr(S_OK, hr);
        assertEqualInt(expected, result);

        // Negative tests.
        hr = pToggleProvider->get_ToggleState(NULL);
        assertEqualHr(E_POINTER, hr);

        return S_OK;
    });
}

TEST const WCHAR* WINAPI Test_IToggleProviderToggle(IUnknown* pUnknown, ToggleState expected)
{
    return RunTest([&](std::wstringstream& output)
    {
        HRESULT hr;
        CComPtr<IToggleProvider> pToggleProvider;

        hr = pUnknown->QueryInterface(IID_IToggleProvider, (void**)&pToggleProvider);
        assertEqualHr(S_OK, hr);

        hr = pToggleProvider->Toggle();
        assertEqualHr(S_OK, hr);

        ToggleState result;
        hr = pToggleProvider->get_ToggleState(&result);
        assertEqualHr(S_OK, hr);
        assertEqualInt(expected, result);

        // Call again.
        hr = pToggleProvider->Toggle();
        assertEqualHr(S_OK, hr);

        hr = pToggleProvider->get_ToggleState(&result);
        assertEqualHr(S_OK, hr);
        assertEqualInt(expected, result);

        return S_OK;
    });
}

TEST const WCHAR* WINAPI Test_ITableProviderGetRowOrColumnMajor(IUnknown* pUnknown, RowOrColumnMajor expected)
{
    return RunTest([&](std::wstringstream& output)
    {
        HRESULT hr;
        CComPtr<ITableProvider> pTableProvider;

        hr = pUnknown->QueryInterface(IID_ITableProvider, (void**)&pTableProvider);
        assertEqualHr(S_OK, hr);

        RowOrColumnMajor result;
        hr = pTableProvider->get_RowOrColumnMajor(&result);
        assertEqualHr(S_OK, hr);
        assertEqualInt(expected, result);

        // Negative tests.
        hr = pTableProvider->get_RowOrColumnMajor(NULL);
        assertEqualHr(E_POINTER, hr);

        return S_OK;
    });
}

TEST const WCHAR* WINAPI Test_ITableProviderGetColumnHeaders(IUnknown* pUnknown)
{
    return RunTest([&](std::wstringstream& output)
    {
        HRESULT hr;
        CComPtr<ITableProvider> pTableProvider;

        hr = pUnknown->QueryInterface(IID_ITableProvider, (void**)&pTableProvider);
        assertEqualHr(S_OK, hr);

        SAFEARRAY *result;
        hr = pTableProvider->GetColumnHeaders(&result);
        assertEqualHr(S_OK, hr);
        assertNull(result);

        // Negative tests.
        hr = pTableProvider->GetColumnHeaders(NULL);
        assertEqualHr(E_POINTER, hr);

        return S_OK;
    });
}

TEST const WCHAR* WINAPI Test_ITableProviderGetRowHeaders(IUnknown* pUnknown)
{
    return RunTest([&](std::wstringstream& output)
    {
        HRESULT hr;
        CComPtr<ITableProvider> pTableProvider;

        hr = pUnknown->QueryInterface(IID_ITableProvider, (void**)&pTableProvider);
        assertEqualHr(S_OK, hr);

        SAFEARRAY *result;
        hr = pTableProvider->GetRowHeaders(&result);
        assertEqualHr(S_OK, hr);
        assertNull(result);

        // Negative tests.
        hr = pTableProvider->GetRowHeaders(NULL);
        assertEqualHr(E_POINTER, hr);

        return S_OK;
    });
}

TEST const WCHAR* WINAPI Test_ITableItemProviderGetColumnHeaderItems(IUnknown* pUnknown)
{
    return RunTest([&](std::wstringstream& output)
    {
        HRESULT hr;
        CComPtr<ITableItemProvider> pTableItemProvider;

        hr = pUnknown->QueryInterface(IID_ITableItemProvider, (void**)&pTableItemProvider);
        assertEqualHr(S_OK, hr);

        SAFEARRAY *result;
        hr = pTableItemProvider->GetColumnHeaderItems(&result);
        assertEqualHr(S_OK, hr);
        assertNull(result);

        // Negative tests.
        hr = pTableItemProvider->GetColumnHeaderItems(NULL);
        assertEqualHr(E_POINTER, hr);

        return S_OK;
    });
}

TEST const WCHAR* WINAPI Test_ITableItemProviderGetRowHeaderItems(IUnknown* pUnknown)
{
    return RunTest([&](std::wstringstream& output)
    {
        HRESULT hr;
        CComPtr<ITableItemProvider> pTableItemProvider;

        hr = pUnknown->QueryInterface(IID_ITableItemProvider, (void**)&pTableItemProvider);
        assertEqualHr(S_OK, hr);

        SAFEARRAY *result;
        hr = pTableItemProvider->GetRowHeaderItems(&result);
        assertEqualHr(S_OK, hr);
        assertNull(result);

        // Negative tests.
        hr = pTableItemProvider->GetRowHeaderItems(NULL);
        assertEqualHr(E_POINTER, hr);

        return S_OK;
    });
}

TEST const WCHAR* WINAPI Test_IGridProviderGetColumnCount(IUnknown* pUnknown, int expected)
{
    return RunTest([&](std::wstringstream& output)
    {
        HRESULT hr;
        CComPtr<IGridProvider> pGridProvider;

        hr = pUnknown->QueryInterface(IID_IGridProvider, (void**)&pGridProvider);
        assertEqualHr(S_OK, hr);

        int result;
        hr = pGridProvider->get_ColumnCount(&result);
        assertEqualHr(S_OK, hr);
        assertEqualInt(expected, result);

        // Negative tests.
        hr = pGridProvider->get_ColumnCount(NULL);
        assertEqualHr(E_POINTER, hr);

        return S_OK;
    });
}

TEST const WCHAR* WINAPI Test_IGridProviderGetRowCount(IUnknown* pUnknown, int expected)
{
    return RunTest([&](std::wstringstream& output)
    {
        HRESULT hr;
        CComPtr<IGridProvider> pGridProvider;

        hr = pUnknown->QueryInterface(IID_IGridProvider, (void**)&pGridProvider);
        assertEqualHr(S_OK, hr);

        int result;
        hr = pGridProvider->get_RowCount(&result);
        assertEqualHr(S_OK, hr);
        assertEqualInt(expected, result);

        // Negative tests.
        hr = pGridProvider->get_RowCount(NULL);
        assertEqualHr(E_POINTER, hr);

        return S_OK;
    });
}

TEST const WCHAR* WINAPI Test_IGridProviderGetItem(IUnknown* pUnknown, int row, int column)
{
    return RunTest([&](std::wstringstream& output)
    {
        HRESULT hr;
        CComPtr<IGridProvider> pIGridProvider;

        hr = pUnknown->QueryInterface(IID_IGridProvider, (void**)&pIGridProvider);
        assertEqualHr(S_OK, hr);

        CComPtr<IRawElementProviderSimple> result;
        hr = pIGridProvider->GetItem(row, column, &result);
        assertEqualHr(S_OK, hr);
        assertNull(result.p);

        // Negative tests.
        hr = pIGridProvider->GetItem(row, column, NULL);
        assertEqualHr(E_POINTER, hr);

        return S_OK;
    });
}

TEST const WCHAR* WINAPI Test_IGridItemProviderGetContainingGrid(IUnknown* pUnknown)
{
    return RunTest([&](std::wstringstream& output)
    {
        HRESULT hr;
        CComPtr<IGridItemProvider> pGridItemProvider;

        hr = pUnknown->QueryInterface(IID_IGridItemProvider, (void**)&pGridItemProvider);
        assertEqualHr(S_OK, hr);

        CComPtr<IRawElementProviderSimple> result;
        hr = pGridItemProvider->get_ContainingGrid(&result);
        assertEqualHr(S_OK, hr);
        assertNull(result.p);

        // Negative tests.
        hr = pGridItemProvider->get_ContainingGrid(NULL);
        assertEqualHr(E_POINTER, hr);

        return S_OK;
    });
}

TEST const WCHAR* WINAPI Test_IGridItemProviderGetColumn(IUnknown* pUnknown, int expected)
{
    return RunTest([&](std::wstringstream& output)
    {
        HRESULT hr;
        CComPtr<IGridItemProvider> pGridItemProvider;

        hr = pUnknown->QueryInterface(IID_IGridItemProvider, (void**)&pGridItemProvider);
        assertEqualHr(S_OK, hr);

        int result;
        hr = pGridItemProvider->get_Column(&result);
        assertEqualHr(S_OK, hr);
        assertEqualInt(expected, result);

        // Negative tests.
        hr = pGridItemProvider->get_Column(NULL);
        assertEqualHr(E_POINTER, hr);

        return S_OK;
    });
}

TEST const WCHAR* WINAPI Test_IGridItemProviderGetColumnSpan(IUnknown* pUnknown, int expected)
{
    return RunTest([&](std::wstringstream& output)
    {
        HRESULT hr;
        CComPtr<IGridItemProvider> pGridItemProvider;

        hr = pUnknown->QueryInterface(IID_IGridItemProvider, (void**)&pGridItemProvider);
        assertEqualHr(S_OK, hr);

        int result;
        hr = pGridItemProvider->get_ColumnSpan(&result);
        assertEqualHr(S_OK, hr);
        assertEqualInt(expected, result);

        // Negative tests.
        hr = pGridItemProvider->get_ColumnSpan(NULL);
        assertEqualHr(E_POINTER, hr);

        return S_OK;
    });
}

TEST const WCHAR* WINAPI Test_IGridItemProviderGetRow(IUnknown* pUnknown, int expected)
{
    return RunTest([&](std::wstringstream& output)
    {
        HRESULT hr;
        CComPtr<IGridItemProvider> pGridItemProvider;

        hr = pUnknown->QueryInterface(IID_IGridItemProvider, (void**)&pGridItemProvider);
        assertEqualHr(S_OK, hr);

        int result;
        hr = pGridItemProvider->get_Row(&result);
        assertEqualHr(S_OK, hr);
        assertEqualInt(expected, result);

        // Negative tests.
        hr = pGridItemProvider->get_Row(NULL);
        assertEqualHr(E_POINTER, hr);

        return S_OK;
    });
}

TEST const WCHAR* WINAPI Test_IGridItemProviderGetRowSpan(IUnknown* pUnknown, int expected)
{
    return RunTest([&](std::wstringstream& output)
    {
        HRESULT hr;
        CComPtr<IGridItemProvider> pGridItemProvider;

        hr = pUnknown->QueryInterface(IID_IGridItemProvider, (void**)&pGridItemProvider);
        assertEqualHr(S_OK, hr);

        int result;
        hr = pGridItemProvider->get_RowSpan(&result);
        assertEqualHr(S_OK, hr);
        assertEqualInt(expected, result);

        // Negative tests.
        hr = pGridItemProvider->get_RowSpan(NULL);
        assertEqualHr(E_POINTER, hr);

        return S_OK;
    });
}

TEST const WCHAR* WINAPI Test_IEnumVARIANTClone(IUnknown* pUnknown)
{
    return RunTest([&](std::wstringstream& output)
    {
        HRESULT hr;
        CComPtr<IEnumVARIANT> pEnumVariant;

        hr = pUnknown->QueryInterface(IID_IEnumVARIANT, (void**)&pEnumVariant);
        assertEqualHr(S_OK, hr);

        CComPtr<IEnumVARIANT> result;
        hr = pEnumVariant->Clone(&result);
        assertEqualHr(S_OK, hr);
        assertNotNull(result.p);
        assert(result.p != pEnumVariant.p);

        VARIANT var;
        ULONG celtFetched = 2;
        hr = result->Next(1, &var, &celtFetched);
        assertEqualHr(S_FALSE, hr);
        assertEqualInt(0, celtFetched);

        // Negative tests.
        hr = pEnumVariant->Clone(NULL);
        assertEqualHr(E_POINTER, hr);

        return S_OK;
    });
}

TEST const WCHAR* WINAPI Test_IEnumVARIANTNextReset(IUnknown* pUnknown)
{
    return RunTest([&](std::wstringstream& output)
    {
        HRESULT hr;
        CComPtr<IEnumVARIANT> pEnumVariant1;
        CComPtr<IEnumVARIANT> pEnumVariant2;
        CComPtr<IEnumVARIANT> pEnumVariant3;
        CComPtr<IEnumVARIANT> pEnumVariant4;

        hr = pUnknown->QueryInterface(IID_IEnumVARIANT, (void**)&pEnumVariant1);
        assertEqualHr(S_OK, hr);
        hr = pUnknown->QueryInterface(IID_IEnumVARIANT, (void**)&pEnumVariant2);
        assertEqualHr(S_OK, hr);
        hr = pUnknown->QueryInterface(IID_IEnumVARIANT, (void**)&pEnumVariant3);
        assertEqualHr(S_OK, hr);
        hr = pUnknown->QueryInterface(IID_IEnumVARIANT, (void**)&pEnumVariant4);
        assertEqualHr(S_OK, hr);

        // Fetch nothing.
        VARIANT var;
        ULONG celtFetched = 2;
        hr = pEnumVariant1->Next(0, &var, &celtFetched);
        assertEqualHr(S_OK, hr);
        assertEqualInt(0, celtFetched);

        // Fetch one.
        for (int i = 0; i < 2; i++)
        {
            hr = pEnumVariant1->Next(1, &var, &celtFetched);
            assertEqualHr(S_FALSE, hr);
            assertEqualInt(0, celtFetched);

            // Fetch another.
            hr = pEnumVariant1->Next(2, &var, &celtFetched);
            assertEqualHr(S_FALSE, hr);
            assertEqualInt(0, celtFetched);

            // Fetch another.
            hr = pEnumVariant1->Next(1, &var, &celtFetched);
            assertEqualHr(S_FALSE, hr);
            assertEqualInt(0, celtFetched);

            // Fetch another.
            hr = pEnumVariant1->Reset();
            assertEqualHr(S_OK, hr);
        }

        // Fetch more than one.
        hr = pEnumVariant2->Next(2, &var, &celtFetched);
        assertEqualHr(S_FALSE, hr);
        assertEqualInt(0, celtFetched);

        // Fetch without celt.
        hr = pEnumVariant2->Next(1, &var, NULL);
#if false
        assertEqualHr(E_POINTER, hr);
#else
        assertEqualHr(S_OK, hr);
#endif

        // Negative tests.
        hr = pEnumVariant4->Next(1, NULL, &celtFetched);
        assertEqualHr(S_FALSE, hr);

        return S_OK;
    });
}

TEST const WCHAR* WINAPI Test_IEnumVARIANTSkip(IUnknown* pUnknown)
{
    return RunTest([&](std::wstringstream& output)
    {
        HRESULT hr;
        CComPtr<IEnumVARIANT> pEnumVariant1;
        CComPtr<IEnumVARIANT> pEnumVariant2;
        CComPtr<IEnumVARIANT> pEnumVariant3;
        VARIANT var;
        ULONG celtFetched = 2;

        hr = pUnknown->QueryInterface(IID_IEnumVARIANT, (void**)&pEnumVariant1);
        assertEqualHr(S_OK, hr);
        hr = pUnknown->QueryInterface(IID_IEnumVARIANT, (void**)&pEnumVariant2);
        assertEqualHr(S_OK, hr);
        hr = pUnknown->QueryInterface(IID_IEnumVARIANT, (void**)&pEnumVariant3);
        assertEqualHr(S_OK, hr);

        // Skip nothing.
        hr = pEnumVariant1->Skip(0);
        assertEqualHr(S_OK, hr);

        // Fetch.
        hr = pEnumVariant1->Next(1, &var, &celtFetched);
        assertEqualHr(S_FALSE, hr);
        assertEqualInt(0, celtFetched);

        // Skip one.
        hr = pEnumVariant2->Skip(1);
        assertEqualHr(S_OK, hr);

        hr = pEnumVariant1->Next(1, &var, &celtFetched);
        assertEqualHr(S_FALSE, hr);
        assertEqualInt(0, celtFetched);

        // Skip again.
        hr = pEnumVariant1->Skip(1);
        assertEqualHr(S_OK, hr);

        hr = pEnumVariant1->Next(1, &var, &celtFetched);
        assertEqualHr(S_FALSE, hr);
        assertEqualInt(0, celtFetched);

        // Skip multiple.
        hr = pEnumVariant2->Skip(2);
        assertEqualHr(S_OK, hr);

        hr = pEnumVariant2->Next(1, &var, &celtFetched);
        assertEqualHr(S_FALSE, hr);
        assertEqualInt(0, celtFetched);

        return S_OK;
    });
}

TEST const WCHAR* WINAPI Test_IOleWindowContextSensitiveHelp(IUnknown* pUnknown, BOOL fEnterMode, HRESULT expectedHr)
{
    return RunTest([&](std::wstringstream& output)
    {
        HRESULT hr;
        CComPtr<IOleWindow> pOleWindow;

        hr = pUnknown->QueryInterface(IID_IOleWindow, (void**)&pOleWindow);
        assertEqualHr(S_OK, hr);

        hr = pOleWindow->ContextSensitiveHelp(fEnterMode);
        assertEqualHr(expectedHr, hr);

        return S_OK;
    });
}

TEST const WCHAR* WINAPI Test_IOleWindowGetWindow(IUnknown* pUnknown, HWND expected, HRESULT expectedHr)
{
    return RunTest([&](std::wstringstream& output)
    {
        HRESULT hr;
        CComPtr<IOleWindow> pOleWindow;

        hr = pUnknown->QueryInterface(IID_IOleWindow, (void**)&pOleWindow);
        assertEqualHr(S_OK, hr);

        HWND result = (HWND)(long)0xdeadbeef;
        hr = pOleWindow->GetWindow(&result);
        assertEqualHr(expectedHr, hr);
        assert(expected == result);

        // Negative tests.
        hr = pOleWindow->GetWindow(NULL);
        assertEqualHr(E_POINTER, hr);

        return S_OK;
    });
}

TEST const WCHAR* WINAPI Test_ILegacyIAccessibleProviderGetChildId(IUnknown* pUnknown, int expected)
{
    return RunTest([&](std::wstringstream& output)
    {
        HRESULT hr;
        CComPtr<ILegacyIAccessibleProvider> pLegacyIAccessibleProvider;

        hr = pUnknown->QueryInterface(IID_ILegacyIAccessibleProvider, (void**)&pLegacyIAccessibleProvider);
        assertEqualHr(S_OK, hr);

        int result;
        hr = pLegacyIAccessibleProvider->get_ChildId(&result);
        assertEqualHr(S_OK, hr);
        assertEqualInt(expected, result);

        // Negative tests.
        hr = pLegacyIAccessibleProvider->get_ChildId(NULL);
        assertEqualHr(E_POINTER, hr);

        return S_OK;
    });
}

TEST const WCHAR* WINAPI Test_ILegacyIAccessibleProviderGetDefaultAction(IUnknown* pUnknown, LPCWSTR expected)
{
    return RunTest([&](std::wstringstream& output)
    {
        HRESULT hr;
        CComPtr<ILegacyIAccessibleProvider> pLegacyIAccessibleProvider;

        hr = pUnknown->QueryInterface(IID_ILegacyIAccessibleProvider, (void**)&pLegacyIAccessibleProvider);
        assertEqualHr(S_OK, hr);

        BSTR result;
        hr = pLegacyIAccessibleProvider->get_DefaultAction(&result);
        assertEqualHr(S_OK, hr);
        assertEqualWString(expected, result);
        SysFreeString(result);

        // Negative tests.
        hr = pLegacyIAccessibleProvider->get_DefaultAction(NULL);
        assertEqualHr(E_POINTER, hr);

        return S_OK;
    });
}

TEST const WCHAR* WINAPI Test_ILegacyIAccessibleProviderGetDescription(IUnknown* pUnknown, LPCWSTR expected)
{
    return RunTest([&](std::wstringstream& output)
    {
        HRESULT hr;
        CComPtr<ILegacyIAccessibleProvider> pLegacyIAccessibleProvider;

        hr = pUnknown->QueryInterface(IID_ILegacyIAccessibleProvider, (void**)&pLegacyIAccessibleProvider);
        assertEqualHr(S_OK, hr);

        BSTR result;
        hr = pLegacyIAccessibleProvider->get_Description(&result);
        assertEqualHr(S_OK, hr);
        assertEqualWString(expected, result);
        SysFreeString(result);

        // Negative tests.
        hr = pLegacyIAccessibleProvider->get_Description(NULL);
        assertEqualHr(E_POINTER, hr);

        return S_OK;
    });
}

TEST const WCHAR* WINAPI Test_ILegacyIAccessibleProviderGetHelp(IUnknown* pUnknown, LPCWSTR expected)
{
    return RunTest([&](std::wstringstream& output)
    {
        HRESULT hr;
        CComPtr<ILegacyIAccessibleProvider> pLegacyIAccessibleProvider;

        hr = pUnknown->QueryInterface(IID_ILegacyIAccessibleProvider, (void**)&pLegacyIAccessibleProvider);
        assertEqualHr(S_OK, hr);

        BSTR result;
        hr = pLegacyIAccessibleProvider->get_Help(&result);
        assertEqualHr(S_OK, hr);
        assertEqualWString(expected, result);
        SysFreeString(result);

        // Negative tests.
        hr = pLegacyIAccessibleProvider->get_Help(NULL);
        assertEqualHr(E_POINTER, hr);

        return S_OK;
    });
}

TEST const WCHAR* WINAPI Test_ILegacyIAccessibleProviderGetKeyboardShortcut(IUnknown* pUnknown, LPCWSTR expected)
{
    return RunTest([&](std::wstringstream& output)
    {
        HRESULT hr;
        CComPtr<ILegacyIAccessibleProvider> pLegacyIAccessibleProvider;

        hr = pUnknown->QueryInterface(IID_ILegacyIAccessibleProvider, (void**)&pLegacyIAccessibleProvider);
        assertEqualHr(S_OK, hr);

        BSTR result;
        hr = pLegacyIAccessibleProvider->get_KeyboardShortcut(&result);
        assertEqualHr(S_OK, hr);
        assertEqualWString(expected, result);
        SysFreeString(result);

        // Negative tests.
        hr = pLegacyIAccessibleProvider->get_KeyboardShortcut(NULL);
        assertEqualHr(E_POINTER, hr);

        return S_OK;
    });
}

TEST const WCHAR* WINAPI Test_ILegacyIAccessibleProviderGetName(IUnknown* pUnknown, LPCWSTR expected)
{
    return RunTest([&](std::wstringstream& output)
    {
        HRESULT hr;
        CComPtr<ILegacyIAccessibleProvider> pLegacyIAccessibleProvider;

        hr = pUnknown->QueryInterface(IID_ILegacyIAccessibleProvider, (void**)&pLegacyIAccessibleProvider);
        assertEqualHr(S_OK, hr);

        BSTR result;
        hr = pLegacyIAccessibleProvider->get_Name(&result);
        assertEqualHr(S_OK, hr);
        assertEqualWString(expected, result);
        SysFreeString(result);

        // Negative tests.
        hr = pLegacyIAccessibleProvider->get_Name(NULL);
        assertEqualHr(E_POINTER, hr);

        return S_OK;
    });
}

TEST const WCHAR* WINAPI Test_ILegacyIAccessibleProviderGetRole(IUnknown* pUnknown, DWORD expected)
{
    return RunTest([&](std::wstringstream& output)
    {
        HRESULT hr;
        CComPtr<ILegacyIAccessibleProvider> pLegacyIAccessibleProvider;

        hr = pUnknown->QueryInterface(IID_ILegacyIAccessibleProvider, (void**)&pLegacyIAccessibleProvider);
        assertEqualHr(S_OK, hr);

        DWORD result;
        hr = pLegacyIAccessibleProvider->get_Role(&result);
        assertEqualHr(S_OK, hr);
        assertEqualInt(expected, result);

        // Negative tests.
        hr = pLegacyIAccessibleProvider->get_Role(NULL);
        assertEqualHr(E_POINTER, hr);

        return S_OK;
    });
}

TEST const WCHAR* WINAPI Test_ILegacyIAccessibleProviderGetState(IUnknown* pUnknown, DWORD expected)
{
    return RunTest([&](std::wstringstream& output)
    {
        HRESULT hr;
        CComPtr<ILegacyIAccessibleProvider> pLegacyIAccessibleProvider;

        hr = pUnknown->QueryInterface(IID_ILegacyIAccessibleProvider, (void**)&pLegacyIAccessibleProvider);
        assertEqualHr(S_OK, hr);

        DWORD result;
        hr = pLegacyIAccessibleProvider->get_State(&result);
        assertEqualHr(S_OK, hr);
        assertEqualInt(expected, result);

        // Negative tests.
        hr = pLegacyIAccessibleProvider->get_State(NULL);
        assertEqualHr(E_POINTER, hr);

        return S_OK;
    });
}

TEST const WCHAR* WINAPI Test_ILegacyIAccessibleProviderGetValue(IUnknown* pUnknown, LPCWSTR expected)
{
    return RunTest([&](std::wstringstream& output)
    {
        HRESULT hr;
        CComPtr<ILegacyIAccessibleProvider> pLegacyIAccessibleProvider;

        hr = pUnknown->QueryInterface(IID_ILegacyIAccessibleProvider, (void**)&pLegacyIAccessibleProvider);
        assertEqualHr(S_OK, hr);

        BSTR result;
        hr = pLegacyIAccessibleProvider->get_Value(&result);
        assertEqualHr(S_OK, hr);
        assertEqualWString(expected, result);
        SysFreeString(result);

        // Negative tests.
        hr = pLegacyIAccessibleProvider->get_Value(NULL);
        assertEqualHr(E_POINTER, hr);

        return S_OK;
    });
}

TEST const WCHAR* WINAPI Test_ILegacyIAccessibleProviderDoDefaultAction(IUnknown* pUnknown)
{
    return RunTest([&](std::wstringstream& output)
    {
        HRESULT hr;
        CComPtr<ILegacyIAccessibleProvider> pLegacyIAccessibleProvider;

        hr = pUnknown->QueryInterface(IID_ILegacyIAccessibleProvider, (void**)&pLegacyIAccessibleProvider);
        assertEqualHr(S_OK, hr);

        hr = pLegacyIAccessibleProvider->DoDefaultAction();
        assertEqualHr(S_OK, hr);

        return S_OK;
    });
}

TEST const WCHAR* WINAPI Test_ILegacyIAccessibleProviderGetIAccessible(IUnknown* pUnknown)
{
    return RunTest([&](std::wstringstream& output)
    {
        HRESULT hr;
        CComPtr<ILegacyIAccessibleProvider> pLegacyIAccessibleProvider;

        hr = pUnknown->QueryInterface(IID_ILegacyIAccessibleProvider, (void**)&pLegacyIAccessibleProvider);
        assertEqualHr(S_OK, hr);

        CComPtr<IAccessible> result;
        hr = pLegacyIAccessibleProvider->GetIAccessible(&result);
        assertEqualHr(S_OK, hr);
        assertNotNull(result.p);

        // Negative tests
        hr = pLegacyIAccessibleProvider->GetIAccessible(NULL);
        assertEqualHr(E_POINTER, hr);

        return S_OK;
    });
}

TEST const WCHAR* WINAPI Test_ILegacyIAccessibleProviderGetSelection(IUnknown* pUnknown, BOOL hasAnything)
{
    return RunTest([&](std::wstringstream& output)
    {
        HRESULT hr;
        CComPtr<ILegacyIAccessibleProvider> pLegacyIAccessibleProvider;

        hr = pUnknown->QueryInterface(IID_ILegacyIAccessibleProvider, (void**)&pLegacyIAccessibleProvider);
        assertEqualHr(S_OK, hr);

#if false
        SAFEARRAY *result = (SAFEARRAY*)(long)0xdeadbeef;
        hr = pLegacyIAccessibleProvider->GetSelection(&result);
        assertEqualHr(S_OK, hr);
        assertNotNull(result);
        assertEqualInt(1, result->cbElements);
        assertEqualInt(hasAnything, static_cast<IRawElementProviderSimple*>(result->pvData) != NULL);
        SafeArrayDestroy(result);
#endif

        // Negative tests
        hr = pLegacyIAccessibleProvider->GetSelection(NULL);
        assertEqualHr(E_POINTER, hr);

        return S_OK;
    });
}

TEST const WCHAR* WINAPI Test_ILegacyIAccessibleProviderSelect(IUnknown *pUnknown, long flagsSelect)
{
    return RunTest([&](std::wstringstream& output)
    {
        HRESULT hr;
        CComPtr<ILegacyIAccessibleProvider> pLegacyIAccessibleProvider;

        hr = pUnknown->QueryInterface(IID_ILegacyIAccessibleProvider, (void**)&pLegacyIAccessibleProvider);
        assertEqualHr(S_OK, hr);

        // Negative tests
        hr = pLegacyIAccessibleProvider->Select(flagsSelect);
        assertEqualHr(S_OK, hr);

#if false
        SAFEARRAY *result = (SAFEARRAY*)(long)0xdeadbeef;
        hr = pLegacyIAccessibleProvider->GetSelection(&result);
        assertEqualHr(S_OK, hr);
        assertNotNull(result);
        assertEqualInt(1, result->cbElements);
        assertNull(*static_cast<IRawElementProviderSimple*>(result->pvData));
        SafeArrayDestroy(result);
#endif

        return S_OK;
    });
}

TEST const WCHAR* WINAPI Test_ILegacyIAccessibleProviderSetValue(IUnknown* pUnknown, LPCWSTR value, LPCWSTR expected)
{
    return RunTest([&](std::wstringstream& output)
    {
        HRESULT hr;
        CComPtr<ILegacyIAccessibleProvider> pLegacyIAccessibleProvider;

        hr = pUnknown->QueryInterface(IID_ILegacyIAccessibleProvider, (void**)&pLegacyIAccessibleProvider);
        assertEqualHr(S_OK, hr);

        hr = pLegacyIAccessibleProvider->SetValue(value);
        assertEqualHr(S_OK, hr);

        BSTR result;
        hr = pLegacyIAccessibleProvider->get_Value(&result);
        assertEqualHr(S_OK, hr);
        assertEqualWString(expected, result);
        SysFreeString(result);

        // Negative tests
        hr = pLegacyIAccessibleProvider->SetValue(NULL);
        assertEqualHr(S_OK, hr);

        return S_OK;
    });
}

TEST const WCHAR* WINAPI Test_ISelectionProviderGetSelection(IUnknown* pUnknown, BOOL hasAnything)
{
    return RunTest([&](std::wstringstream& output)
    {
        HRESULT hr;
        CComPtr<ISelectionProvider> pSelectionProvider;

        hr = pUnknown->QueryInterface(IID_ISelectionProvider, (void**)&pSelectionProvider);
        assertEqualHr(S_OK, hr);

        SAFEARRAY *result = (SAFEARRAY*)(long)0xdeadbeef;
        hr = pSelectionProvider->GetSelection(&result);
        assertEqualHr(S_OK, hr);
        assertNull(result);
        SafeArrayDestroy(result);

        // Negative tests
        hr = pSelectionProvider->GetSelection(NULL);
        assertEqualHr(E_POINTER, hr);

        return S_OK;
    });
}

TEST const WCHAR* WINAPI Test_ISelectionProviderGetCanSelectMultiple(IUnknown* pUnknown, BOOL expected)
{
    return RunTest([&](std::wstringstream& output)
    {
        HRESULT hr;
        CComPtr<ISelectionProvider> pRangeSelectionProvider;

        hr = pUnknown->QueryInterface(IID_ISelectionProvider, (void**)&pRangeSelectionProvider);
        assertEqualHr(S_OK, hr);

        BOOL result;
        hr = pRangeSelectionProvider->get_CanSelectMultiple(&result);
        assertEqualHr(S_OK, hr);
        assertEqualInt(expected, result);

        // Negative tests.
        hr = pRangeSelectionProvider->get_CanSelectMultiple(NULL);
        assertEqualHr(E_POINTER, hr);

        return S_OK;
    });
}

TEST const WCHAR* WINAPI Test_ISelectionProviderGetIsSelectionRequired(IUnknown* pUnknown, BOOL expected)
{
    return RunTest([&](std::wstringstream& output)
    {
        HRESULT hr;
        CComPtr<ISelectionProvider> pRangeSelectionProvider;

        hr = pUnknown->QueryInterface(IID_ISelectionProvider, (void**)&pRangeSelectionProvider);
        assertEqualHr(S_OK, hr);

        BOOL result;
        hr = pRangeSelectionProvider->get_IsSelectionRequired(&result);
        assertEqualHr(S_OK, hr);
        assertEqualInt(expected, result);

        // Negative tests.
        hr = pRangeSelectionProvider->get_IsSelectionRequired(NULL);
        assertEqualHr(E_POINTER, hr);

        return S_OK;
    });
}

TEST const WCHAR* WINAPI Test_ISelectionItemProviderGetIsSelected(IUnknown* pUnknown, BOOL expected)
{
    return RunTest([&](std::wstringstream& output)
    {
        HRESULT hr;
        CComPtr<ISelectionItemProvider> pSelectionItemProvider;

        hr = pUnknown->QueryInterface(IID_ISelectionProvider, (void**)&pSelectionItemProvider);
        assertEqualHr(S_OK, hr);

        // Fails: The active test run was aborted. Reason: Test host process crashed : Fatal error. Internal CLR error.
#if false
        BOOL result;
        hr = pSelectionItemProvider->get_IsSelected(&result);
        assertEqualHr(S_OK, hr);
        assertEqualInt(expected, result);

        // Negative tests.
        hr = pSelectionItemProvider->get_IsSelected(NULL);
        assertEqualHr(S_OK, hr);
#endif

        return S_OK;
    });
}

TEST const WCHAR* WINAPI Test_ISelectionItemProviderGetSelectionContainer(IUnknown* pUnknown, BOOL expected)
{
    return RunTest([&](std::wstringstream& output)
    {
        HRESULT hr;
        CComPtr<ISelectionItemProvider> pSelectionItemProvider;

        hr = pUnknown->QueryInterface(IID_ISelectionProvider, (void**)&pSelectionItemProvider);
        assertEqualHr(S_OK, hr);

        // Fails: The active test run was aborted. Reason: Test host process crashed : Fatal error. Internal CLR error.
#if false
        CComPtr<IRawElementProviderSimple> result;
        hr = pSelectionItemProvider->get_SelectionContainer(&result);
        assertEqualHr(S_OK, hr);
        assertNull(result.p);

        // Negative tests.
        hr = pSelectionItemProvider->get_SelectionContainer(NULL);
        assertEqualHr(S_OK, hr);
#endif

        return S_OK;
    });
}

TEST const WCHAR* WINAPI Test_ISelectionItemProviderAddToSelection(IUnknown* pUnknown)
{
    return RunTest([&](std::wstringstream& output)
    {
        HRESULT hr;
        CComPtr<ISelectionItemProvider> pSelectionItemProvider;

        hr = pUnknown->QueryInterface(IID_ISelectionItemProvider, (void**)&pSelectionItemProvider);
        assertEqualHr(S_OK, hr);

        hr = pSelectionItemProvider->AddToSelection();
        assertEqualHr(S_OK, hr);

        BOOL result;
        hr = pSelectionItemProvider->get_IsSelected(&result);
        assertEqualHr(S_OK, hr);
        assertEqualInt(FALSE, result);

        // Call again.
        hr = pSelectionItemProvider->AddToSelection();
        assertEqualHr(S_OK, hr);

        hr = pSelectionItemProvider->get_IsSelected(&result);
        assertEqualHr(S_OK, hr);
        assertEqualInt(FALSE, result);

        return S_OK;
    });
}

TEST const WCHAR* WINAPI Test_ISelectionItemProviderRemoveFromSelection(IUnknown* pUnknown)
{
    return RunTest([&](std::wstringstream& output)
    {
        HRESULT hr;
        CComPtr<ISelectionItemProvider> pSelectionItemProvider;

        hr = pUnknown->QueryInterface(IID_ISelectionItemProvider, (void**)&pSelectionItemProvider);
        assertEqualHr(S_OK, hr);

        hr = pSelectionItemProvider->RemoveFromSelection();
        assertEqualHr(S_OK, hr);

        BOOL result;
        hr = pSelectionItemProvider->get_IsSelected(&result);
        assertEqualHr(S_OK, hr);
        assertEqualInt(FALSE, result);

        // Add.
        hr = pSelectionItemProvider->AddToSelection();
        assertEqualHr(S_OK, hr);

        hr = pSelectionItemProvider->RemoveFromSelection();
        assertEqualHr(S_OK, hr);

        hr = pSelectionItemProvider->get_IsSelected(&result);
        assertEqualHr(S_OK, hr);
        assertEqualInt(FALSE, result);

        return S_OK;
    });
}

TEST const WCHAR* WINAPI Test_ISelectionItemProviderSelect(IUnknown* pUnknown)
{
    return RunTest([&](std::wstringstream& output)
    {
        HRESULT hr;
        CComPtr<ISelectionItemProvider> pSelectionItemProvider;

        hr = pUnknown->QueryInterface(IID_ISelectionItemProvider, (void**)&pSelectionItemProvider);
        assertEqualHr(S_OK, hr);

        hr = pSelectionItemProvider->Select();
        assertEqualHr(S_OK, hr);

        BOOL result;
        hr = pSelectionItemProvider->get_IsSelected(&result);
        assertEqualHr(S_OK, hr);
        assertEqualInt(FALSE, result);

        return S_OK;
    });
}

TEST const WCHAR* WINAPI Test_IRawElementProviderHwndOverrideGetOverrideProviderForHwnd(IUnknown* pUnknown)
{
    return RunTest([&](std::wstringstream& output)
    {
        HRESULT hr;
        CComPtr<IRawElementProviderHwndOverride> pRawElementProviderHwndOverride;

        // Fails with E_NOINTERFACE.
        hr = pUnknown->QueryInterface(IID_IRawElementProviderHwndOverride, (void**)&pRawElementProviderHwndOverride);
#if false
        assertEqualHr(S_OK, hr);

        CComPtr<IRawElementProviderSimple> result;
        hr = pRawElementProviderHwndOverride->GetOverrideProviderForHwnd(NULL, &result);
        assertEqualHr(S_OK, hr);
        assertNull(result.p);

        // Negative tests.
        hr = pRawElementProviderHwndOverride->GetOverrideProviderForHwnd(NULL, NULL);
        assertEqualHr(S_OK, hr);
#endif

        return S_OK;
    });
}

TEST const WCHAR* WINAPI Test_IScrollItemProviderScrollIntoView(IUnknown* pUnknown)
{
    return RunTest([&](std::wstringstream& output)
    {
        HRESULT hr;
        CComPtr<IScrollItemProvider> pScrollItemProvider;

        hr = pUnknown->QueryInterface(IID_IScrollItemProvider, (void**)&pScrollItemProvider);
        assertEqualHr(S_OK, hr);

        hr = pScrollItemProvider->ScrollIntoView();
        assertEqualHr(S_OK, hr);

        return S_OK;
    });
}
