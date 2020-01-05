// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal partial class Interop
{
    internal static partial class VSSDK
    {
        [ComImport]
        [Guid("33C0C1D8-33CF-11d3-BFF2-00C04F990235")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public unsafe interface IProvidePropertyBuilder
        {
            [PreserveSig]
            HRESULT MapPropertyToBuilder(
                Ole32.DispatchID dispid,
                CTLBLDTYPE* pdwCtlBldType,
                [In, Out, MarshalAs(UnmanagedType.LPArray)] string[] pbstrGuidBldr,
                BOOL* builderAvailable);

            [PreserveSig]
            HRESULT ExecuteBuilder(
                Ole32.DispatchID dispid,
                [MarshalAs(UnmanagedType.BStr)] string bstrGuidBldr,
                [MarshalAs(UnmanagedType.Interface)] object pdispApp,
                IntPtr hwndBldrOwner,
                [In, Out, MarshalAs(UnmanagedType.Struct)] ref object pvarValue,
                BOOL* actionCommitted);
        }
    }
}
