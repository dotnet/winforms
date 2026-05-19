// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32.System.Com;

internal partial struct IEnumSTATDATA : IComIID
{
    readonly ref readonly Guid IComIID.Guid => ref Unsafe.AsRef(in IID_Guid);
}
