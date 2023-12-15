// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;

namespace Windows.Win32.UI.Controls.Dialogs;

/// <inheritdoc cref="PRINTDLGW_64"/>
/// <devdoc>
///  Unfortunately the packing on 32 bit doesn't align all members with the default packing.
/// </devdoc>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
internal partial struct PRINTDLGW_32
{
    /// <inheritdoc cref="PRINTDLGW_64.lStructSize"/>
    public uint lStructSize;
    /// <inheritdoc cref="PRINTDLGW_64.hwndOwner"/>
    public HWND hwndOwner;
    /// <inheritdoc cref="PRINTDLGW_64.hDevMode"/>
    public HGLOBAL hDevMode;
    /// <inheritdoc cref="PRINTDLGW_64.hDevNames"/>
    public HGLOBAL hDevNames;
    /// <inheritdoc cref="PRINTDLGW_64.hDC"/>
    public HDC hDC;
    /// <inheritdoc cref="PRINTDLGW_64.Flags"/>
    public PRINTDLGEX_FLAGS Flags;
    /// <inheritdoc cref="PRINTDLGW_64.nFromPage"/>
    public ushort nFromPage;
    /// <inheritdoc cref="PRINTDLGW_64.nToPage"/>
    public ushort nToPage;
    /// <inheritdoc cref="PRINTDLGW_64.nMinPage"/>
    public ushort nMinPage;
    /// <inheritdoc cref="PRINTDLGW_64.nMaxPage"/>
    public ushort nMaxPage;
    /// <inheritdoc cref="PRINTDLGW_64.nCopies"/>
    public ushort nCopies;
    /// <inheritdoc cref="PRINTDLGW_64.hInstance"/>
    public HINSTANCE hInstance;
    /// <inheritdoc cref="PRINTDLGW_64.lCustData"/>
    public LPARAM lCustData;
    /// <inheritdoc cref="PRINTDLGW_64.lpfnPrintHook"/>
    public unsafe delegate* unmanaged[Stdcall]<HWND, uint, WPARAM, LPARAM, nuint> lpfnPrintHook;
    /// <inheritdoc cref="PRINTDLGW_64.lpfnSetupHook"/>
    public unsafe delegate* unmanaged[Stdcall]<HWND, uint, WPARAM, LPARAM, nuint> lpfnSetupHook;
    /// <inheritdoc cref="PRINTDLGW_64.lpPrintTemplateName"/>
    public PCWSTR lpPrintTemplateName;
    /// <inheritdoc cref="PRINTDLGW_64.lpSetupTemplateName"/>
    public PCWSTR lpSetupTemplateName;
    /// <inheritdoc cref="PRINTDLGW_64.hPrintTemplate"/>
    public HGLOBAL hPrintTemplate;
    /// <inheritdoc cref="PRINTDLGW_64.hSetupTemplate"/>
    public HGLOBAL hSetupTemplate;
}
