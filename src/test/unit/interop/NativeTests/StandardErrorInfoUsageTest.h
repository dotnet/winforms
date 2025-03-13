// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#pragma once

#include "ComHelpers.h"
#include <Contract.h>
#include "DispatchImpl.h"
#include <vector>

class StandardErrorInfoUsageTest : public DispatchImpl, public IBasicTest, public ISupportErrorInfo
{
public:
    StandardErrorInfoUsageTest()
        : DispatchImpl(IID_IBasicTest, static_cast<IBasicTest*>(this))
    {
    }

public: // IBasicTest
    
    virtual HRESULT STDMETHODCALLTYPE get_Int_Property( 
        /* [retval][out] */ int *ret);
    
    virtual HRESULT STDMETHODCALLTYPE put_Int_Property( 
        /* [in] */ int val);

public: // ISupportErrorInfo
    virtual HRESULT STDMETHODCALLTYPE InterfaceSupportsErrorInfo(
        /* [in] */ __RPC__in REFIID riid);

public: // IDispatch
    DEFINE_DISPATCH();
    
public: // IUnknown
    STDMETHOD(QueryInterface)(
        /* [in] */ REFIID riid,
        /* [iid_is][out] */ _COM_Outptr_ void __RPC_FAR *__RPC_FAR *ppvObject)
    {
        return DoQueryInterface(riid, ppvObject,
            static_cast<IDispatch *>(this),
            static_cast<IBasicTest *>(this),
            static_cast<ISupportErrorInfo *>(this));
    }

    DEFINE_REF_COUNTING();

private:
    int _int;
};
