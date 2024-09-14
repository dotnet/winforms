// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32.UI.Controls.Dialogs;

/// <summary>
///  Represents a range of pages in a print job. A print job can have more than one page range. This information is
///  supplied in the <see cref="PRINTDLGEXW"/> structure when calling the <see cref="PInvokeCore.PrintDlgEx"/> function.
/// </summary>
/// <remarks>
///  <para>
///   <see href="https://learn.microsoft.com/windows/win32/api/commdlg/ns-commdlg-printpagerange">
///    Learn more about this API from https://learn.microsoft.com.
///   </see>
///  </para>
/// </remarks>
/// <devdoc>
///  Manually copied from a 64 bit project CsWin32 generated wrapper. We can't directly use CsWin32 for this as it
///  technically isn't compatible with AnyCPU. For our usages this works fine on both 32 bit and 64 bit.
///
///  This is defined with single byte packing on 32 bit, but there are no gaps as everything naturally packs with no
///  gaps on 32 bit. Issues would arise if this was contained in another native struct where it wouldn't start 32 bit
///  aligned due to the single byte packing.
///
///  https://github.com/microsoft/CsWin32/issues/882
/// </devdoc>
internal partial struct PRINTPAGERANGE
{
    /// <summary>
    ///  <para>Type: <b>DWORD</b> The first page of the range.</para>
    ///  <para>
    ///   <see href="https://learn.microsoft.com/windows/win32/api/commdlg/ns-commdlg-printpagerange#members">
    ///    Read more on https://learn.microsoft.com.
    ///   </see>
    ///  </para>
    /// </summary>
    public uint nFromPage;

    /// <summary>
    ///  <para>Type: <b>DWORD</b> The last page of the range.</para>
    ///  <para>
    ///   <see href="https://learn.microsoft.com/windows/win32/api/commdlg/ns-commdlg-printpagerange#members">
    ///    Read more on https://learn.microsoft.com.
    ///   </see>
    ///  </para>
    /// </summary>
    public uint nToPage;
}
