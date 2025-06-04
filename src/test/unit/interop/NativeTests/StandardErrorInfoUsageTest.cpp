// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#include "StandardErrorInfoUsageTest.h"
#include <cmath>

HRESULT STDMETHODCALLTYPE StandardErrorInfoUsageTest::get_Int_Property(
    /* [retval][out] */ int *ret)
{
    *ret = _int;
    return S_OK;
}

HRESULT STDMETHODCALLTYPE StandardErrorInfoUsageTest::put_Int_Property(
    /* [in] */ int val)
{
    ComSmartPtr<ICreateErrorInfo> cei;
    if (SUCCEEDED(::CreateErrorInfo(&cei)))
    {
        if (SUCCEEDED(cei->SetGUID(IID_IBasicTest)))
        {
            if (SUCCEEDED(cei->SetDescription(L"Error From StandardErrorInfoUsageTest")))
            {
                ComSmartPtr<IErrorInfo> errorInfo;
                if (SUCCEEDED(cei->QueryInterface(IID_IErrorInfo, (void**)&errorInfo)))
                {
                    ::SetErrorInfo(0, errorInfo);
                }
            }
        }
    }

    return DISP_E_MEMBERNOTFOUND;
}

HRESULT STDMETHODCALLTYPE StandardErrorInfoUsageTest::InterfaceSupportsErrorInfo(
    /* [in] */ __RPC__in REFIID riid)
{
    return S_OK;
}
