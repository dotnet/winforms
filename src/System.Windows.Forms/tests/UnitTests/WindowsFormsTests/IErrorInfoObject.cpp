// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#include "ocidl.h"
#include <sstream>
#include "ComHelpers.h"
#include <Contract.h>

#include "RawErrorInfoUsageTest.h"
#include "StandardErrorInfoUsageTest.h"

extern "C" __declspec(dllexport) HRESULT WINAPI Create_Raw_IErrorInfo_UsageObject(_Out_ LPVOID FAR * ppDispatchPtr)
{
    IClassFactory* classFactory;
    HRESULT hr;
    RETURN_IF_FAILED(ClassFactoryBasic<RawErrorInfoUsageTest>::Create(IID_IClassFactory, (LPVOID*)&classFactory));
    return classFactory->CreateInstance(nullptr, IID_IBasicTest, ppDispatchPtr);
}

extern "C" __declspec(dllexport) HRESULT WINAPI Create_Standard_IErrorInfo_UsageObject(_Out_ LPVOID FAR * ppDispatchPtr)
{
    IClassFactory* classFactory;
    HRESULT hr;
    RETURN_IF_FAILED(ClassFactoryBasic<StandardErrorInfoUsageTest>::Create(IID_IClassFactory, (LPVOID*)&classFactory));
    return classFactory->CreateInstance(nullptr, IID_IBasicTest, ppDispatchPtr);
}
