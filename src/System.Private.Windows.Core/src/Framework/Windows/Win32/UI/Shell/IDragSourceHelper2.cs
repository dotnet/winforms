// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32.UI.Shell;

internal partial struct IDragSourceHelper2 : IComIID
{
    public readonly ref readonly Guid Guid => ref Unsafe.AsRef(in IID_Guid);
}
