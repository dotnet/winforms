// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;
using Windows.Win32.UI.Controls.RichEdit;
internal partial class Interop
{
    internal static partial class Richedit
    {
        [StructLayout(LayoutKind.Sequential, Pack = RichEditPack)]
        public struct ENPROTECTED
        {
            public NMHDR nmhdr;
            public int msg;
            public nuint wParam;
            public nint lParam;
            public CHARRANGE chrg;
        }
    }
}
