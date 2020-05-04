// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

internal static partial class Interop
{
    internal static partial class Shell32
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public unsafe struct NOTIFYICONDATAW
        {
            public uint cbSize;
            public IntPtr hWnd;
            public uint uID;
            public NIF uFlags;
            public uint uCallbackMessage;
            public IntPtr hIcon;
            public fixed char _szTip[128];
            public uint dwState;
            public uint dwStateMask;
            public fixed char _szInfo[256];
            public uint uTimeoutOrVersion;
            public fixed char _szInfoTitle[64];
            public NIIF dwInfoFlags;

            private Span<char> szTip
            {
                get { fixed (char* c = _szTip) { return new Span<char>(c, 128); } }
            }

            public ReadOnlySpan<char> Tip
            {
                get => szTip.SliceAtFirstNull();
                set => SpanHelpers.CopyAndTerminate(value, szTip);
            }

            private Span<char> szInfo
            {
                get { fixed (char* c = _szInfo) { return new Span<char>(c, 256); } }
            }

            public ReadOnlySpan<char> Info
            {
                get => szInfo.SliceAtFirstNull();
                set => SpanHelpers.CopyAndTerminate(value, szInfo);
            }

            private Span<char> szInfoTitle
            {
                get { fixed (char* c = _szInfoTitle) { return new Span<char>(c, 64); } }
            }

            public ReadOnlySpan<char> InfoTitle
            {
                get => szInfoTitle.SliceAtFirstNull();
                set => SpanHelpers.CopyAndTerminate(value, szInfoTitle);
            }
        }
    }
}
