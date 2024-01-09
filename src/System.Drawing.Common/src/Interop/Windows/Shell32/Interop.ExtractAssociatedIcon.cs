// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;
#if NET7_0_OR_GREATER
using System.Runtime.InteropServices.Marshalling;
#endif

internal static partial class Interop
{
    internal static partial class Shell32
    {
#if NET7_0_OR_GREATER
        [LibraryImport(Libraries.Shell32, EntryPoint = "ExtractAssociatedIconW")]
        internal static unsafe partial IntPtr ExtractAssociatedIcon(
            [MarshalUsing(typeof(HandleRefMarshaller))]
#else
        [DllImport(Libraries.Shell32, CharSet = CharSet.Unicode)]
        internal static extern unsafe IntPtr ExtractAssociatedIcon(
#endif
            HandleRef hInst,
            char* iconPath,
            ref int index);
    }
}
