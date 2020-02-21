// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class Ole32
    {
        [ComImport]
        [Guid("BEF6E002-A874-101A-8BBA-00AA00300CAB")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public unsafe interface IFont
        {
            string Name { get; set; }
            long Size { get; set; }
            BOOL Bold { get; set; }
            BOOL Italic { get; set; }
            BOOL Underline { get; set; }
            BOOL Strikethrough { get; set; }
            short Weight { get; set; }
            short Charset { get; set; }
            IntPtr hFont { get; }

            [PreserveSig]
            HRESULT Clone(
                out IFont ppfont);

            [PreserveSig]
            HRESULT IsEqual(
                IFont pfontOther);

            [PreserveSig]
            HRESULT SetRatio(
                int cyLogical,
                int cyHimetric);

            [PreserveSig]
            HRESULT QueryTextMetrics(
                Gdi32.TEXTMETRICW* pTM);

            [PreserveSig]
            HRESULT AddRefHfont(
                IntPtr hFont);

            [PreserveSig]
            HRESULT ReleaseHfont(
                IntPtr hFont);

            [PreserveSig]
            HRESULT SetHdc(
                IntPtr hDC);
        }
    }
}
