// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;

internal partial class Interop
{
    internal static partial class Richedit
    {
        [StructLayout(LayoutKind.Sequential, Pack = RichEditPack)]
        public struct EDITSTREAM
        {
            public UIntPtr dwCookie;
            public uint dwError;
            public IntPtr pfnCallback;
        }
    }
}
