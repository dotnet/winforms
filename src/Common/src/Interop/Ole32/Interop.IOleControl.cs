// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal partial class Interop
{
    internal static partial class Ole32
    {
        [ComImport]
        [Guid("B196B288-BAB4-101A-B69C-00AA00341D07")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public unsafe interface IOleControl
        {
            [PreserveSig]
            HRESULT GetControlInfo(
                CONTROLINFO* pCI);

            [PreserveSig]
            HRESULT OnMnemonic(
                User32.MSG* pMsg);

            [PreserveSig]
            HRESULT OnAmbientPropertyChange(
                Ole32.DispatchID dispID);

            [PreserveSig]
            HRESULT FreezeEvents(
                BOOL bFreeze);
        }
    }
}
