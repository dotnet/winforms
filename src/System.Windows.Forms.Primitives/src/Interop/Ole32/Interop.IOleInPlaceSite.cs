// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Drawing;
using System.Runtime.InteropServices;

internal partial class Interop
{
    internal static partial class Ole32
    {
        [ComImport]
        [Guid("00000119-0000-0000-C000-000000000046")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public unsafe interface IOleInPlaceSite
        {
            [PreserveSig]
            HRESULT GetWindow(
                IntPtr* phwnd);

            [PreserveSig]
            HRESULT ContextSensitiveHelp(
                BOOL fEnterMode);

            [PreserveSig]
            HRESULT CanInPlaceActivate();

            [PreserveSig]
            HRESULT OnInPlaceActivate();

            [PreserveSig]
            HRESULT OnUIActivate();

            [PreserveSig]
            HRESULT GetWindowContext(
                out IOleInPlaceFrame ppFrame,
                out IOleInPlaceUIWindow ppDoc,
                RECT* lprcPosRect,
                RECT* lprcClipRect,
                OLEINPLACEFRAMEINFO* lpFrameInfo);

            [PreserveSig]
            HRESULT Scroll(
                Size scrollExtant);

            [PreserveSig]
            HRESULT OnUIDeactivate(
                BOOL fUndoable);

            [PreserveSig]
            HRESULT OnInPlaceDeactivate();

            [PreserveSig]
            HRESULT DiscardUndoState();

            [PreserveSig]
            HRESULT DeactivateAndUndo();

            [PreserveSig]
            HRESULT OnPosRectChange(
                RECT* lprcPosRect);
        }
    }
}
