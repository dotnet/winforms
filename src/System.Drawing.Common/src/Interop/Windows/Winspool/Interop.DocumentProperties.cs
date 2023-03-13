// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;
#if NET7_0_OR_GREATER
using System.Runtime.InteropServices.Marshalling;
#endif

internal static partial class Interop
{
    internal static partial class Winspool
    {
#if NET7_0_OR_GREATER
        [LibraryImport(Libraries.Winspool, EntryPoint = "DocumentPropertiesW", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
        internal static partial int DocumentProperties(
            [MarshalUsing(typeof(HandleRefMarshaller))]
#else
        [DllImport(Libraries.Winspool, SetLastError = true, CharSet = CharSet.Auto, BestFitMapping = false)]
        internal static extern int DocumentProperties(
#endif
            HandleRef hwnd,
#if NET7_0_OR_GREATER
            [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
            HandleRef hPrinter,
            string pDeviceName,
            IntPtr /*DEVMODE*/ pDevModeOutput,
#if NET7_0_OR_GREATER
            [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
            HandleRef /*DEVMODE*/ pDevModeInput,
            int fMode);

#if NET7_0_OR_GREATER
        [LibraryImport(Libraries.Winspool, EntryPoint = "DocumentPropertiesW", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
        internal static partial int DocumentProperties(
            [MarshalUsing(typeof(HandleRefMarshaller))]
#else
        [DllImport(Libraries.Winspool, SetLastError = true, CharSet = CharSet.Auto, BestFitMapping = false)]
        internal static extern int DocumentProperties(
#endif
            HandleRef hwnd,
#if NET7_0_OR_GREATER
            [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
            HandleRef hPrinter,
            string pDeviceName,
            IntPtr /*DEVMODE*/ pDevModeOutput,
            IntPtr /*DEVMODE*/ pDevModeInput,
            int fMode);
    }
}
