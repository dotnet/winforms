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
        [Guid("00000113-0000-0000-C000-000000000046")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public unsafe interface IOleInPlaceObject
        {
            [PreserveSig]
            HRESULT GetWindow(
                IntPtr* phwnd);

            [PreserveSig]
            HRESULT ContextSensitiveHelp(
                BOOL fEnterMode);

            [PreserveSig]
            HRESULT InPlaceDeactivate();

            [PreserveSig]
            HRESULT UIDeactivate();

            [PreserveSig]
            HRESULT SetObjectRects(
                RECT* lprcPosRect,
                RECT* lprcClipRect);

            [PreserveSig]
            HRESULT ReactivateAndUndo();
        }
    }
}
