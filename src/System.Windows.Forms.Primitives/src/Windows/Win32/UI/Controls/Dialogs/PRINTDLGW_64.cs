// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32.UI.Controls.Dialogs;

/// <summary>
///  Contains information that the PrintDlg function uses to initialize the Print Dialog Box. After the user closes the
///  dialog box, the system uses this structure to return information about the user's selections.
/// </summary>
/// <remarks>
///  <para>
///   <see href="https://learn.microsoft.com/windows/win32/api/commdlg/ns-commdlg-printdlgw#">
///    Read more on https://learn.microsoft.com.
///   </see>
///  </para>
/// </remarks>
/// <devdoc>
///  Manually copied from a 64 bit project CsWin32 generated wrapper. We can't directly use CsWin32 for this as it
///  isn't compatible with AnyCPU. The packing on 32 bit is 1 byte and unfortunately in this case the type doesn't
///  align the same way it normally would.
/// </devdoc>
internal partial struct PRINTDLGW_64
{
    /// <summary>
    ///  <para>
    ///   Type: <b>DWORD</b> The structure size, in bytes.
    ///  </para>
    ///  <para>
    ///   <see href="https://learn.microsoft.com/windows/win32/api/commdlg/ns-commdlg-printdlgw#members">
    ///    Read more on https://learn.microsoft.com.
    ///   </see>
    ///  </para>
    /// </summary>
    public uint lStructSize;

    /// <summary>
    ///  <para>
    ///   Type: <b>HWND</b> A handle to the window that owns the dialog box. This member can be any valid window handle,
    ///   or it can be <b>NULL</b> if the dialog box has no owner.
    ///  </para>
    ///  <para>
    ///   <see href="https://learn.microsoft.com/windows/win32/api/commdlg/ns-commdlg-printdlgw#members">
    ///    Read more on https://learn.microsoft.com.
    ///   </see>
    ///  </para>
    /// </summary>
    public HWND hwndOwner;

    /// <summary>
    ///  <para>
    ///   Type: <b>HGLOBAL</b> A handle to a movable global memory object that contains a
    ///   <a href="https://learn.microsoft.com/windows/win32/api/wingdi/ns-wingdi-devmodea">DEVMODE</a> structure.
    ///   If <b>hDevMode</b> is not <b>NULL</b> on input, you must allocate a movable block of memory for the
    ///   <b>DEVMODE</b> structure and initialize its members.
    ///   The <a href="https://learn.microsoft.com/previous-versions/windows/desktop/legacy/ms646940(v=vs.85)">
    ///   PrintDlg</a> function uses the input data to initialize the controls in the dialog box.
    ///   When <b>PrintDlg</b> returns, the <b>DEVMODE</b> members indicate the user's input.
    ///   If <b>hDevMode</b> is <b>NULL</b> on input,
    ///   <a href="https://learn.microsoft.com/previous-versions/windows/desktop/legacy/ms646940(v=vs.85)">
    ///   PrintDlg</a> allocates memory for the
    ///   <a href="https://learn.microsoft.com/windows/win32/api/wingdi/ns-wingdi-devmodea">
    ///   DEVMODE</a> structure, initializes its members to indicate the user's input, and returns a handle
    ///   that identifies it. If the device driver for the specified printer does not support extended device modes,
    ///   <b>hDevMode</b> is <b>NULL</b> when
    ///   <a href="https://learn.microsoft.com/previous-versions/windows/desktop/legacy/ms646940(v=vs.85)">
    ///   PrintDlg</a> returns. If the device name (specified by the <b>dmDeviceName</b> member of the
    ///   <a href="https://learn.microsoft.com/windows/win32/api/wingdi/ns-wingdi-devmodea">
    ///   DEVMODE</a> structure) does not appear in the [devices] section of WIN.INI,
    ///   <a href="https://learn.microsoft.com/previous-versions/windows/desktop/legacy/ms646940(v=vs.85)">
    ///   PrintDlg</a> returns an error. For more information about the <b>hDevMode</b> and <b>hDevNames</b> members,
    ///   see the Remarks section at the end of this topic.
    ///  </para>
    ///  <para>
    ///   <see href="https://learn.microsoft.com/windows/win32/api/commdlg/ns-commdlg-printdlgw#members">
    ///    Read more on https://learn.microsoft.com.
    ///   </see>
    ///  </para>
    /// </summary>
    public HGLOBAL hDevMode;

    /// <summary>
    ///  <para>
    ///   Type: <b>HGLOBAL</b> A handle to a movable global memory object that contains a
    ///   <a href="https://learn.microsoft.com/windows/desktop/api/commdlg/ns-commdlg-devnames">
    ///   DEVNAMES</a> structure. If <b>hDevNames</b> is not <b>NULL</b> on input, you must allocate
    ///   a movable block of memory for the <b>DEVNAMES</b> structure and initialize its members.
    ///   The <a href="https://learn.microsoft.com/previous-versions/windows/desktop/legacy/ms646940(v=vs.85)">
    ///   PrintDlg</a> function uses the input data to initialize the controls in the dialog box. When <b>PrintDlg</b>
    ///   returns, the <b>DEVNAMES</b> members contain information for the printer chosen by the user.
    ///   You can use this information to create a device context or an information context. The <b>hDevNames</b>
    ///   member can be <b>NULL</b>, in which case,
    ///   <a href="https://learn.microsoft.com/previous-versions/windows/desktop/legacy/ms646940(v=vs.85)">
    ///   PrintDlg</a> allocates memory for the
    ///   <a href="https://learn.microsoft.com/windows/desktop/api/commdlg/ns-commdlg-devnames">
    ///   DEVNAMES</a> structure, initializes its members to indicate the user's input,
    ///   and returns a handle that identifies it. For more information about the <b>hDevMode</b>
    ///   and <b>hDevNames</b> members, see the Remarks section at the end of this topic.
    ///  </para>
    ///  <para>
    ///   <see href="https://learn.microsoft.com/windows/win32/api/commdlg/ns-commdlg-printdlgw#members">
    ///    Read more on https://learn.microsoft.com.
    ///   </see>
    ///  </para>
    /// </summary>
    public HGLOBAL hDevNames;

    /// <summary>
    ///  <para>
    ///   Type: <b>HDC</b> A handle to a device context or an information context, depending on whether the <b>Flags</b>
    ///   member specifies the <b>PD_RETURNDC</b> or <b>PC_RETURNIC</b> flag.
    ///   If neither flag is specified, the value of this member is undefined. If both flags are specified,
    ///   <b>PD_RETURNDC</b> has priority.
    ///  </para>
    ///  <para>
    ///   <see href="https://learn.microsoft.com/windows/win32/api/commdlg/ns-commdlg-printdlgw#members">
    ///    Read more on https://learn.microsoft.com.
    ///   </see>
    ///  </para>
    /// </summary>
    public HDC hDC;

    /// <summary>Type: <b>DWORD</b></summary>
    public PRINTDLGEX_FLAGS Flags;

    /// <summary>
    ///  <para>
    ///   Type: <b>WORD</b> The initial value for the starting page edit control.
    ///   When <a href="https://learn.microsoft.com/previous-versions/windows/desktop/legacy/ms646940(v=vs.85)">
    ///   PrintDlg</a> returns, <b>nFromPage</b> is the starting page specified by the user. If the <b>Pages</b>
    ///   radio button is selected when the user clicks the <b>Okay</b> button, <b>PrintDlg</b> sets the
    ///   <b>PD_PAGENUMS</b> flag and does not return until the user enters a starting page value that is within
    ///   the minimum to maximum page range. If the input value for either <b>nFromPage</b> or <b>nToPage</b>
    ///   is outside the minimum/maximum range,
    ///   <a href="https://learn.microsoft.com/previous-versions/windows/desktop/legacy/ms646940(v=vs.85)">
    ///   PrintDlg</a> returns an error only if the <b>PD_PAGENUMS</b> flag is specified; otherwise,
    ///   it displays the dialog box but changes the out-of-range value to the minimum or maximum value.
    ///  </para>
    ///  <para>
    ///   <see href="https://learn.microsoft.com/windows/win32/api/commdlg/ns-commdlg-printdlgw#members">
    ///    Read more on https://learn.microsoft.com.
    ///   </see>
    ///  </para>
    /// </summary>
    public ushort nFromPage;

    /// <summary>
    ///  <para>
    ///   Type: <b>WORD</b> The initial value for the ending page edit control.
    ///   When <a href="https://learn.microsoft.com/previous-versions/windows/desktop/legacy/ms646940(v=vs.85)">
    ///   PrintDlg</a> returns, <b>nToPage</b> is the ending page specified by the user. If the <b>Pages</b>
    ///   radio button is selected when the use clicks the <b>Okay</b> button, <b>PrintDlg</b> sets the
    ///   <b>PD_PAGENUMS</b> flag and does not return until the user enters an ending page value that is within
    ///   the minimum to maximum page range.
    ///  </para>
    ///  <para>
    ///   <see href="https://learn.microsoft.com/windows/win32/api/commdlg/ns-commdlg-printdlgw#members">
    ///    Read more on https://learn.microsoft.com.
    ///   </see>
    ///  </para>
    /// </summary>
    public ushort nToPage;

    /// <summary>
    ///  <para>
    ///   Type: <b>WORD</b> The minimum value for the page range specified in the <b>From</b> and <b>To</b> page edit
    ///   controls. If <b>nMinPage</b> equals <b>nMaxPage</b>, the <b>Pages</b> radio button and the starting and
    ///   ending page edit controls are disabled.
    ///  </para>
    ///  <para>
    ///   <see href="https://learn.microsoft.com/windows/win32/api/commdlg/ns-commdlg-printdlgw#members">
    ///    Read more on https://learn.microsoft.com.
    ///   </see>
    ///  </para>
    /// </summary>
    public ushort nMinPage;

    /// <summary>
    ///  <para>
    ///   Type: <b>WORD</b> The maximum value for the page range specified in the <b>From</b> and <b>To</b> page edit controls.
    ///  </para>
    ///  <para>
    ///   <see href="https://learn.microsoft.com/windows/win32/api/commdlg/ns-commdlg-printdlgw#members">
    ///    Read more on https://learn.microsoft.com.
    ///   </see>
    ///  </para>
    /// </summary>
    public ushort nMaxPage;

    /// <summary>
    ///  <para>
    ///   Type: <b>WORD</b> The initial number of copies for the <b>Copies</b> edit control if <b>hDevMode</b>
    ///   is <b>NULL</b>; otherwise, the <b>dmCopies</b> member of the
    ///   <a href="https://learn.microsoft.com/windows/win32/api/wingdi/ns-wingdi-devmodea">
    ///   DEVMODE</a> structure contains the initial value.
    ///   When <a href="https://learn.microsoft.com/previous-versions/windows/desktop/legacy/ms646940(v=vs.85)">
    ///   PrintDlg</a> returns, <b>nCopies</b> contains the actual number of copies to print.
    ///   This value depends on whether the application or the printer driver is responsible for printing multiple copies.
    ///   If the <b>PD_USEDEVMODECOPIESANDCOLLATE</b> flag is set in the <b>Flags</b> member, <b>nCopies</b>
    ///   is always 1 on return, and the printer driver is responsible for printing multiple copies.
    ///   If the flag is not set, the application is responsible for printing the number of copies specified by
    ///   <b>nCopies</b>. For more information, see the description of the <b>PD_USEDEVMODECOPIESANDCOLLATE</b> flag.
    ///  </para>
    ///  <para>
    ///   <see href="https://learn.microsoft.com/windows/win32/api/commdlg/ns-commdlg-printdlgw#members">
    ///    Read more on https://learn.microsoft.com.
    ///   </see>
    ///  </para>
    /// </summary>
    public ushort nCopies;

    /// <summary>
    ///  <para>
    ///   Type: <b>HINSTANCE</b> If the <b>PD_ENABLEPRINTTEMPLATE</b> or <b>PD_ENABLESETUPTEMPLATE</b> flag is set in the
    ///   <b>Flags</b> member, <b>hInstance</b> is a handle to the application or module instance that contains
    ///   the dialog box template named by the <b>lpPrintTemplateName</b> or <b>lpSetupTemplateName</b> member.
    ///  </para>
    ///  <para>
    ///   <see href="https://learn.microsoft.com/windows/win32/api/commdlg/ns-commdlg-printdlgw#members">
    ///    Read more on https://learn.microsoft.com.
    ///   </see>
    ///  </para>
    /// </summary>
    public HINSTANCE hInstance;

    /// <summary>
    ///  <para>
    ///   Type: <b>LPARAM</b> Application-defined data that the system passes to the hook procedure identified by the
    ///   <b>lpfnPrintHook</b> or <b>lpfnSetupHook</b> member. When the system sends the
    ///   <a href="https://learn.microsoft.com/windows/desktop/dlgbox/wm-initdialog">WM_INITDIALOG</a> message
    ///   to the hook procedure, the message's <i>lParam</i> parameter is a pointer to the <b>PRINTDLG</b>
    ///   structure specified when the dialog was created. The hook procedure can use this pointer to get the
    ///   <b>lCustData</b> value.
    ///  </para>
    ///  <para>
    ///   <see href="https://learn.microsoft.com/windows/win32/api/commdlg/ns-commdlg-printdlgw#members">
    ///    Read more on https://learn.microsoft.com.
    ///   </see>
    ///  </para>
    /// </summary>
    public LPARAM lCustData;

    /// <summary>
    ///  <para>
    ///   Type: <b>LPPRINTHOOKPROC</b> A pointer to a
    ///   <a href="https://learn.microsoft.com/windows/desktop/api/commdlg/nc-commdlg-lpprinthookproc">
    ///   PrintHookProc</a> hook procedure that can process messages intended for the <b>Print</b> dialog box.
    ///   This member is ignored unless the <b>PD_ENABLEPRINTHOOK</b> flag is set in the <b>Flags</b> member.
    ///  </para>
    ///  <para>
    ///   <see href="https://learn.microsoft.com/windows/win32/api/commdlg/ns-commdlg-printdlgw#members">
    ///    Read more on https://learn.microsoft.com.
    ///   </see>
    ///  </para>
    /// </summary>
    public unsafe delegate* unmanaged[Stdcall]<HWND, uint, WPARAM, LPARAM, nuint> lpfnPrintHook;

    /// <summary>
    ///  <para>
    ///   Type: <b>LPSETUPHOOKPROC</b> A pointer to a
    ///   <a href="https://learn.microsoft.com/windows/desktop/api/commdlg/nc-commdlg-lpsetuphookproc">
    ///   SetupHookProc</a> hook procedure that can process messages intended for the <b>Print Setup</b> dialog box.
    ///   This member is ignored unless the <b>PD_ENABLESETUPHOOK</b> flag is set in the <b>Flags</b> member.
    ///  </para>
    ///  <para>
    ///   <see href="https://learn.microsoft.com/windows/win32/api/commdlg/ns-commdlg-printdlgw#members">
    ///    Read more on https://learn.microsoft.com.
    ///   </see>
    ///  </para>
    /// </summary>
    public unsafe delegate* unmanaged[Stdcall]<HWND, uint, WPARAM, LPARAM, nuint> lpfnSetupHook;

    /// <summary>
    ///  <para>
    ///   Type: <b>LPCTSTR</b> The name of the dialog box template resource in the module identified by the
    ///   <b>hInstance</b> member. This template replaces the default <b>Print</b> dialog box template.
    ///   This member is ignored unless the <b>PD_ENABLEPRINTTEMPLATE</b> flag is set in the <b>Flags</b> member.
    ///  </para>
    ///  <para>
    ///   <see href="https://learn.microsoft.com/windows/win32/api/commdlg/ns-commdlg-printdlgw#members">
    ///    Read more on https://learn.microsoft.com.
    ///   </see>
    ///  </para>
    /// </summary>
    public PCWSTR lpPrintTemplateName;

    /// <summary>
    ///  <para>
    ///   Type: <b>LPCTSTR</b> The name of the dialog box template resource in the module identified by the
    ///   <b>hInstance</b> member. This template replaces the default <b>Print Setup</b> dialog box template.
    ///   This member is ignored unless the <b>PD_ENABLESETUPTEMPLATE</b> flag is set in the <b>Flags</b> member.
    ///  </para>
    ///  <para>
    ///   <see href="https://learn.microsoft.com/windows/win32/api/commdlg/ns-commdlg-printdlgw#members">
    ///    Read more on https://learn.microsoft.com.
    ///   </see>
    ///  </para>
    /// </summary>
    public PCWSTR lpSetupTemplateName;

    /// <summary>
    ///  <para>
    ///   Type: <b>HGLOBAL</b> If the <b>PD_ENABLEPRINTTEMPLATEHANDLE</b> flag is set in the <b>Flags</b> member,
    ///   <b>hPrintTemplate</b> is a handle to a memory object containing a dialog box template.
    ///   This template replaces the default <b>Print</b> dialog box template.
    ///  </para>
    ///  <para>
    ///   <see href="https://learn.microsoft.com/windows/win32/api/commdlg/ns-commdlg-printdlgw#members">
    ///    Read more on https://learn.microsoft.com.
    ///   </see>
    ///  </para>
    /// </summary>
    public HGLOBAL hPrintTemplate;

    /// <summary>
    ///  <para>
    ///   Type: <b>HGLOBAL</b> If the <b>PD_ENABLESETUPTEMPLATEHANDLE</b> flag is set in the <b>Flags</b> member,
    ///   <b>hSetupTemplate</b> is a handle to a memory object containing a dialog box template.
    ///   This template replaces the default <b>Print Setup</b> dialog box template.
    ///  </para>
    ///  <para>
    ///   <see href="https://learn.microsoft.com/windows/win32/api/commdlg/ns-commdlg-printdlgw#members">
    ///    Read more on https://learn.microsoft.com.
    ///   </see>
    ///  </para>
    /// </summary>
    public HGLOBAL hSetupTemplate;
}
