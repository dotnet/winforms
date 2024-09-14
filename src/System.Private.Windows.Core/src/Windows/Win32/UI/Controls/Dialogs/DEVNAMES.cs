// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32.UI.Controls.Dialogs;

/// <summary>
///  Contains strings that identify the driver, device, and output port names for a printer.
/// </summary>
/// <remarks>
///  <para>
///   <see href="https://learn.microsoft.com/windows/win32/api/commdlg/ns-commdlg-devnames">
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
internal partial struct DEVNAMES
{
    /// <summary>
    ///  <para>
    ///   Type: <b>WORD</b> The offset, in characters, from the beginning of this structure to a null-terminated string
    ///   that contains the file name (without the extension) of the device driver. On input, this string is used to
    ///   determine the printer to display initially in the dialog box.
    ///  </para>
    ///  <para>
    ///   <see href="https://learn.microsoft.com/windows/win32/api/commdlg/ns-commdlg-devnames#members">
    ///    Read more on https://learn.microsoft.com.
    ///   </see>
    ///  </para>
    /// </summary>
    public ushort wDriverOffset;

    /// <summary>
    ///  <para>
    ///   Type: <b>WORD</b> The offset, in characters, from the beginning of this structure to the null-terminated
    ///   string that contains the name of the device.
    ///  </para>
    ///  <para>
    ///   <see href="https://learn.microsoft.com/windows/win32/api/commdlg/ns-commdlg-devnames#members">
    ///    Read more on https://learn.microsoft.com.
    ///   </see>
    ///  </para>
    /// </summary>
    public ushort wDeviceOffset;

    /// <summary>
    ///  <para>
    ///   Type: <b>WORD</b> The offset, in characters, from the beginning of this structure to the null-terminated
    ///   string that contains the device name for the physical output medium (output port).
    ///  </para>
    ///  <para>
    ///   <see href="https://learn.microsoft.com/windows/win32/api/commdlg/ns-commdlg-devnames#members">
    ///    Read more on https://learn.microsoft.com.
    ///   </see>
    ///  </para>
    /// </summary>
    public ushort wOutputOffset;

    /// <summary>
    ///  <para>
    ///   Type: <b>WORD</b> Indicates whether the strings contained in the <b>DEVNAMES</b> structure identify the
    ///   default printer. This string is used to verify that the default printer has not changed since the last
    ///   print operation. If any of the strings do not match, a warning message is displayed informing the user
    ///   that the document may need to be reformatted. On output, the <b>wDefault</b> member is changed only if
    ///   the <b>Print Setup</b> dialog box was displayed and the user chose the <b>OK</b> button.
    ///   The <b>DN_DEFAULTPRN</b> flag is used if the default printer was selected. If a specific printer is selected,
    ///   the flag is not used. All other flags in this member are reserved for internal use by the dialog box
    ///   procedure for the <b>Print</b> property sheet or <b>Print</b> dialog box.
    ///  </para>
    ///  <para>
    ///   <see href="https://learn.microsoft.com/windows/win32/api/commdlg/ns-commdlg-devnames#members">
    ///    Read more on https://learn.microsoft.com.
    ///   </see>
    ///  </para>
    /// </summary>
    public ushort wDefault;
}
