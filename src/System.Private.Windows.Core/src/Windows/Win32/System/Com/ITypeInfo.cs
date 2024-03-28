// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32.System.Com;

internal unsafe partial struct ITypeInfo
{
    /// <inheritdoc cref="GetIDsOfNames(PWSTR*, uint, int*)"/>
    public HRESULT GetIDOfName(string name, out int memberId)
    {
        int id = 0;

        fixed (char* n = name)
        {
            HRESULT result = GetIDsOfNames((PWSTR*)&n, 1, &id);
            memberId = id;
            return result;
        }
    }
}
