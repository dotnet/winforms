// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class Ole32
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public class OLEVERB
        {
            public OLEIVERB lVerb;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string? lpszVerbName;
            public User32.MF fuFlags;
            public OLEVERBATTRIB grfAttribs;
        }
    }
}
