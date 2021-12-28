// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop;

namespace System.Windows.Forms
{
    public partial class DateTimePicker
    {
        private sealed class EnumChildren
        {
            public IntPtr hwndFound = IntPtr.Zero;

            public BOOL enumChildren(IntPtr hwnd)
            {
                hwndFound = hwnd;
                return BOOL.TRUE;
            }
        }
    }
}
