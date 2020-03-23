// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using static Interop.User32;

internal partial class Interop
{
    internal partial class Comdlg32
    {
        // Any change in PRINTDLG_32, should also be in PRINTDLG and PRINTDLG_64
        // x86 requires EXPLICIT packing of 1.

        public unsafe interface PRINTDLGW
        {
            uint lStructSize { get; set; }

            IntPtr hwndOwner { get; set; }
            IntPtr hDevMode { get; set; }
            IntPtr hDevNames { get; set; }
            IntPtr hDC { get; set; }

            PD Flags { get; set; }

            ushort nFromPage { get; set; }
            ushort nToPage { get; set; }
            ushort nMinPage { get; set; }
            ushort nMaxPage { get; set; }
            ushort nCopies { get; set; }

            IntPtr hInstance { get; set; }
            IntPtr lCustData { get; set; }

            IntPtr lpfnPrintHook { get; set; }
            IntPtr lpfnSetupHook { get; set; }

            char* lpPrintTemplateName { get; set; }
            char* lpSetupTemplateName { get; set; }

            IntPtr hPrintTemplate { get; set; }
            IntPtr hSetupTemplate { get; set; }
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1)]
        public unsafe struct PRINTDLGW_32 : PRINTDLGW
        {
            public uint _lStructSize;
            public IntPtr _hwndOwner;
            public IntPtr _hDevMode;
            public IntPtr _hDevNames;
            public IntPtr _hDC;
            public PD _flags;
            public ushort _nFromPage;
            public ushort _nToPage;
            public ushort _nMinPage;
            public ushort _nMaxPage;
            public ushort _nCopies;
            public IntPtr _hInstance;
            public IntPtr _lCustData;
            public IntPtr _lpfnPrintHook;
            public IntPtr _lpfnSetupHook;
            public char* _lpPrintTemplateName;
            public char* _lpSetupTemplateName;
            public IntPtr _hPrintTemplate;
            public IntPtr _hSetupTemplate;

            public uint lStructSize { get => _lStructSize; set => _lStructSize = value; }
            public IntPtr hwndOwner { get => _hwndOwner; set => _hwndOwner = value; }
            public IntPtr hDevMode { get => _hDevMode; set => _hDevMode = value; }
            public IntPtr hDevNames { get => _hDevNames; set => _hDevNames = value; }
            public IntPtr hDC { get => _hDC; set => _hDC = value; }
            public PD Flags { get => _flags; set => _flags = value; }
            public ushort nFromPage { get => _nFromPage; set => _nFromPage = value; }
            public ushort nToPage { get => _nToPage; set => _nToPage = value; }
            public ushort nMinPage { get => _nMinPage; set => _nMinPage = value; }
            public ushort nMaxPage { get => _nMaxPage; set => _nMaxPage = value; }
            public ushort nCopies { get => _nCopies; set => _nCopies = value; }
            public IntPtr hInstance { get => _hInstance; set => _hInstance = value; }
            public IntPtr lCustData { get => _lCustData; set => _lCustData = value; }
            public IntPtr lpfnPrintHook { get => _lpfnPrintHook; set => _lpfnPrintHook = value; }
            public IntPtr lpfnSetupHook { get => _lpfnSetupHook; set => _lpfnSetupHook = value; }
            public char* lpPrintTemplateName { get => _lpPrintTemplateName; set => _lpPrintTemplateName = value; }
            public char* lpSetupTemplateName { get => _lpSetupTemplateName; set => _lpSetupTemplateName = value; }
            public IntPtr hPrintTemplate { get => _hPrintTemplate; set => _hPrintTemplate = value; }
            public IntPtr hSetupTemplate { get => _hSetupTemplate; set => _hSetupTemplate = value; }
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public unsafe struct PRINTDLGW_64 : PRINTDLGW
        {
            public uint _lStructSize;
            public IntPtr _hwndOwner;
            public IntPtr _hDevMode;
            public IntPtr _hDevNames;
            public IntPtr _hDC;
            public PD _flags;
            public ushort _nFromPage;
            public ushort _nToPage;
            public ushort _nMinPage;
            public ushort _nMaxPage;
            public ushort _nCopies;
            public IntPtr _hInstance;
            public IntPtr _lCustData;
            public IntPtr _lpfnPrintHook;
            public IntPtr _lpfnSetupHook;
            public char* _lpPrintTemplateName;
            public char* _lpSetupTemplateName;
            public IntPtr _hPrintTemplate;
            public IntPtr _hSetupTemplate;

            public uint lStructSize { get => _lStructSize; set => _lStructSize = value; }
            public IntPtr hwndOwner { get => _hwndOwner; set => _hwndOwner = value; }
            public IntPtr hDevMode { get => _hDevMode; set => _hDevMode = value; }
            public IntPtr hDevNames { get => _hDevNames; set => _hDevNames = value; }
            public IntPtr hDC { get => _hDC; set => _hDC = value; }
            public PD Flags { get => _flags; set => _flags = value; }
            public ushort nFromPage { get => _nFromPage; set => _nFromPage = value; }
            public ushort nToPage { get => _nToPage; set => _nToPage = value; }
            public ushort nMinPage { get => _nMinPage; set => _nMinPage = value; }
            public ushort nMaxPage { get => _nMaxPage; set => _nMaxPage = value; }
            public ushort nCopies { get => _nCopies; set => _nCopies = value; }
            public IntPtr hInstance { get => _hInstance; set => _hInstance = value; }
            public IntPtr lCustData { get => _lCustData; set => _lCustData = value; }
            public IntPtr lpfnPrintHook { get => _lpfnPrintHook; set => _lpfnPrintHook = value; }
            public IntPtr lpfnSetupHook { get => _lpfnSetupHook; set => _lpfnSetupHook = value; }
            public char* lpPrintTemplateName { get => _lpPrintTemplateName; set => _lpPrintTemplateName = value; }
            public char* lpSetupTemplateName { get => _lpSetupTemplateName; set => _lpSetupTemplateName = value; }
            public IntPtr hPrintTemplate { get => _hPrintTemplate; set => _hPrintTemplate = value; }
            public IntPtr hSetupTemplate { get => _hSetupTemplate; set => _hSetupTemplate = value; }
        }
    }
}
