// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using static Interop;

namespace System.Windows.Forms
{
    public partial class Screen
    {
        private class MonitorEnumCallback
        {
            public ArrayList screens = new ArrayList();

            public virtual BOOL Callback(IntPtr monitor, Gdi32.HDC hdc, IntPtr lprcMonitor, IntPtr lparam)
            {
                screens.Add(new Screen(monitor, hdc));
                return BOOL.TRUE;
            }
        }
    }
}
