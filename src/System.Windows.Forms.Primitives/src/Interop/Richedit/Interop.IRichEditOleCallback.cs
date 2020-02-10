﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

internal partial class Interop
{
    internal static partial class Richedit
    {
        [ComImport]
        [Guid("00020D03-0000-0000-C000-000000000046")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public unsafe interface IRichEditOleCallback
        {
            [PreserveSig]
            HRESULT GetNewStorage(out Ole32.IStorage ret);

            [PreserveSig]
            HRESULT GetInPlaceContext(IntPtr lplpFrame, IntPtr lplpDoc, IntPtr lpFrameInfo);

            [PreserveSig]
            HRESULT ShowContainerUI(BOOL fShow);

            [PreserveSig]
            HRESULT QueryInsertObject(ref Guid lpclsid, IntPtr lpstg, int cp);

            [PreserveSig]
            HRESULT DeleteObject(IntPtr lpoleobj);

            [PreserveSig]
            HRESULT QueryAcceptData(IDataObject lpdataobj, /* CLIPFORMAT* */ IntPtr lpcfFormat, RECO reco, BOOL fReally, IntPtr hMetaPict);

            [PreserveSig]
            HRESULT ContextSensitiveHelp(BOOL fEnterMode);

            [PreserveSig]
            HRESULT GetClipboardData(ref CHARRANGE lpchrg, RECO reco, IntPtr lplpdataobj);

            [PreserveSig]
            HRESULT GetDragDropEffect(
                BOOL fDrag,
                User32.MK grfKeyState,
                Ole32.DROPEFFECT* pdwEffect);

            [PreserveSig]
            HRESULT GetContextMenu(short seltype, IntPtr lpoleobj, ref CHARRANGE lpchrg, out IntPtr hmenu);
        }
    }
}
