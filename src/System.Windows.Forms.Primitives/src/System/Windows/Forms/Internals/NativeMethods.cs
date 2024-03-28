// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

internal static class NativeMethods
{
    public delegate int ListViewCompareCallback(IntPtr lParam1, IntPtr lParam2, IntPtr lParamSort);

    public static class ActiveX
    {
        public const int ALIGN_MIN = 0x0;
        public const int ALIGN_NO_CHANGE = 0x0;
        public const int ALIGN_TOP = 0x1;
        public const int ALIGN_BOTTOM = 0x2;
        public const int ALIGN_LEFT = 0x3;
        public const int ALIGN_RIGHT = 0x4;
        public const int ALIGN_MAX = 0x4;
    }
}
