// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32.UI.WindowsAndMessaging;

internal unsafe partial struct ICONINFO : IDisposable
{
    public void Dispose()
    {
        if (!hbmMask.IsNull)
        {
            PInvokeCore.DeleteObject((HGDIOBJ)hbmMask.Value);
            hbmMask = default;
        }

        if (!hbmColor.IsNull)
        {
            PInvokeCore.DeleteObject((HGDIOBJ)hbmColor.Value);
            hbmColor = default;
        }
    }
}
