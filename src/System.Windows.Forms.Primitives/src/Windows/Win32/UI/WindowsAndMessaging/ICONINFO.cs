﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Windows.Win32.UI.WindowsAndMessaging
{
    internal partial struct ICONINFO : IDisposable
    {
        public void Dispose()
        {
            if (!hbmMask.IsNull)
            {
                PInvoke.DeleteObject((HGDIOBJ)hbmMask.Value);
                hbmMask = default;
            }

            if (!hbmColor.IsNull)
            {
                PInvoke.DeleteObject((HGDIOBJ)hbmColor.Value);
                hbmColor = default;
            }
        }
    }
}
