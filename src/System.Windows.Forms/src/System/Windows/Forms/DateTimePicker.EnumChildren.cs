// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    public partial class DateTimePicker
    {
        private sealed class EnumChildren
        {
            public HWND hwndFound;

            public BOOL enumChildren(HWND hwnd)
            {
                hwndFound = hwnd;
                return true;
            }
        }
    }
}
