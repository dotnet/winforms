// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;
#if NET7_0_OR_GREATER
using System.Runtime.InteropServices.Marshalling;
#endif

internal static partial class Interop
{
    internal static partial class Gdi32
    {
#if NET7_0_OR_GREATER
        [LibraryImport(Libraries.Gdi32, SetLastError = true)]
        internal static partial int AbortDoc(
            [MarshalUsing(typeof(HandleRefMarshaller))]
#else
        [DllImport(Libraries.Gdi32, SetLastError = true, ExactSpelling = true, CharSet = CharSet.Auto)]
        internal static extern int AbortDoc(
#endif
            HandleRef hDC);
    }
}
