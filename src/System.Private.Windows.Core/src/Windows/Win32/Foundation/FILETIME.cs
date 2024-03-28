// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32.Foundation;

internal partial struct FILETIME
{
    public FILETIME(DateTime date)
    {
        long ft = date.ToFileTime();
        dwLowDateTime = (uint)(ft & 0xFFFFFFFF);
        dwHighDateTime = (uint)(ft >> 32);
    }

    public readonly DateTime ToDateTime() => DateTime.FromFileTime(((long)dwHighDateTime << 32) + dwLowDateTime);
}
