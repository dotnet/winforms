// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#include "ocidl.h"
#include <sstream>
#include "ComHelpers.h"
#include <Contract.h>

#include "RawErrorInfoUsageTest.h"
#include "StandardErrorInfoUsageTest.h"

STDAPI DllGetClassObject(_In_ REFCLSID rclsid, _In_ REFIID riid, _Out_ LPVOID FAR* ppv)
{
    if (rclsid == __uuidof(RawErrorInfoUsageTest))
        return ClassFactoryBasic<RawErrorInfoUsageTest>::Create(riid, ppv);

    if (rclsid == __uuidof(StandardErrorInfoUsageTest))
        return ClassFactoryBasic<StandardErrorInfoUsageTest>::Create(riid, ppv);

    return CLASS_E_CLASSNOTAVAILABLE;
}
