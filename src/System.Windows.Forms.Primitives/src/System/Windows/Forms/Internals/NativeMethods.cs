// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

internal static class NativeMethods
{
    public delegate int ListViewCompareCallback(IntPtr lParam1, IntPtr lParam2, IntPtr lParamSort);
}
