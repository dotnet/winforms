// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

internal partial class Interop
{
    internal static partial class Ole32
    {
        [ComImport]
        [Guid("00000127-0000-0000-C000-000000000046")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public unsafe interface IViewObject2 /* : IViewObject */
        {
            [PreserveSig]
            HRESULT Draw(
                DVASPECT dwDrawAspect,
                int lindex,
                IntPtr pvAspect,
                Ole32.DVTARGETDEVICE* ptd,
                IntPtr hdcTargetDev,
                IntPtr hdcDraw,
                RECT* lprcBounds,
                RECT* lprcWBounds,
                IntPtr pfnContinue,
                uint dwContinue);

            [PreserveSig]
            HRESULT GetColorSet(
                DVASPECT dwDrawAspect,
                int lindex,
                IntPtr pvAspect,
                Ole32.DVTARGETDEVICE* ptd,
                IntPtr hicTargetDev,
                Gdi32.LOGPALETTE* ppColorSet);

            [PreserveSig]
            HRESULT Freeze(
                DVASPECT dwDrawAspect,
                int lindex,
                IntPtr pvAspect,
                uint* pdwFreeze);

            [PreserveSig]
            HRESULT Unfreeze(
                int dwFreeze);

            [PreserveSig]
            HRESULT SetAdvise(
                DVASPECT aspects,
                ADVF advf,
                IAdviseSink pAdvSink);

            [PreserveSig]
            HRESULT GetAdvise(
                DVASPECT* pAspects,
                ADVF* pAdvf,
                [Out, MarshalAs(UnmanagedType.LPArray)] IAdviseSink[] ppAdvSink);

            [PreserveSig]
            HRESULT GetExtent(
                DVASPECT dwDrawAspect,
                int lindex,
                Ole32.DVTARGETDEVICE* ptd,
                Size* lpsizel);
        }
    }
}
