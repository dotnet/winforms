// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

internal partial class Interop
{
    internal static partial class Ole32
    {
        [ComImport]
        [Guid("0000010d-0000-0000-C000-000000000046")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public unsafe interface IViewObject
        {
            [PreserveSig]
            HRESULT Draw(
                DVASPECT dwDrawAspect,
                int lindex,
                IntPtr pvAspect,
                DVTARGETDEVICE* ptd,
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
                DVTARGETDEVICE* ptd,
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
                uint dwFreeze);

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
        }
    }
}
