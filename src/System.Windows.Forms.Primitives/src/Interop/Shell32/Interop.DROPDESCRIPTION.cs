// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using Windows.Win32;

internal partial class Interop
{
    internal static partial class Shell32
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public unsafe struct DROPDESCRIPTION
        {
            public DROPIMAGETYPE type;
            private fixed char _szMessage[PInvoke.MAX_PATH];
            private fixed char _szInsert[PInvoke.MAX_PATH];

            private Span<char> szMessage
            {
                get { fixed (char* c = _szMessage) { return new Span<char>(c, PInvoke.MAX_PATH); } }
            }

            private Span<char> szInsert
            {
                get { fixed (char* c = _szInsert) { return new Span<char>(c, PInvoke.MAX_PATH); } }
            }

            public ReadOnlySpan<char> Message
            {
                get => szMessage.SliceAtFirstNull();
                set => SpanHelpers.CopyAndTerminate(value, szMessage);
            }

            public ReadOnlySpan<char> Insert
            {
                get => szInsert.SliceAtFirstNull();
                set => SpanHelpers.CopyAndTerminate(value, szInsert);
            }
        }
    }
}
