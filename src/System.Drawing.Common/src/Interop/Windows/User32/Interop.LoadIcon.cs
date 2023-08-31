// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;
#if NET7_0_OR_GREATER
using System.Runtime.InteropServices.Marshalling;
#endif

internal static partial class Interop
{
    internal static partial class User32
    {
#if NET7_0_OR_GREATER
        [LibraryImport(Libraries.User32, EntryPoint = "LoadIconW", SetLastError = true)]
        internal static partial IntPtr LoadIcon(
            [MarshalUsing(typeof(HandleRefMarshaller))]
#else
        [DllImport(Libraries.User32, SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern IntPtr LoadIcon(
#endif
            HandleRef hInst, IntPtr iconId);
    }
}
