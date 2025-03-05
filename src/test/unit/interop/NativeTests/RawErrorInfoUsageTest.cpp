// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#include "RawErrorInfoUsageTest.h"
#include <cmath>

HRESULT STDMETHODCALLTYPE RawErrorInfoUsageTest::get_Int_Property(
    /* [retval][out] */ int *ret)
{
    *ret = _int;
    return S_OK;
}

HRESULT STDMETHODCALLTYPE RawErrorInfoUsageTest::put_Int_Property(
    /* [in] */ int val)
{
    return DISP_E_MEMBERNOTFOUND;
}

HRESULT STDMETHODCALLTYPE RawErrorInfoUsageTest::InterfaceSupportsErrorInfo(
    /* [in] */ __RPC__in REFIID riid)
{
    // This is hack, in order to not implement IDispatch.
    // By default implementations wrap any error during invoke into DISP_E_EXCEPTION
    // and consume IErrorInfo. Some implementation behave differently, so this is emulation
    // of that behavior.
    ComSmartPtr<ICreateErrorInfo> cei;
    if (SUCCEEDED(::CreateErrorInfo(&cei)))
    {
        if (SUCCEEDED(cei->SetGUID(IID_IBasicTest)))
        {
            if (SUCCEEDED(cei->SetDescription(L"Error From RawErrorInfoUsageTest")))
            {
                ComSmartPtr<IErrorInfo> errorInfo;
                if (SUCCEEDED(cei->QueryInterface(IID_IErrorInfo, (void**)&errorInfo)))
                {
                    ::SetErrorInfo(0, errorInfo);
                }
            }
        }
    }

    return S_OK;
}
