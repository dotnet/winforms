// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
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
        internal static partial IntPtr CreateDIBSection(
            [MarshalUsing(typeof(HandleRefMarshaller))]
#else
        [DllImport(Libraries.Gdi32, SetLastError = true, ExactSpelling = true)]
        internal static extern IntPtr CreateDIBSection(
#endif
            HandleRef hdc,
            ref BITMAPINFO_FLAT bmi,
            int iUsage,
            ref IntPtr ppvBits,
            IntPtr hSection,
            int dwOffset);
    }
}
