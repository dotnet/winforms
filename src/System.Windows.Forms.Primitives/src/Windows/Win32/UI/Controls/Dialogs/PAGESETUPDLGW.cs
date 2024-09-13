// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace Windows.Win32.UI.Controls.Dialogs;

/// <summary>
///  Contains information the PageSetupDlg function uses to initialize the Page Setup dialog box. After the user closes
///  the dialog box, the system returns information about the user-defined page parameters in this structure.
/// </summary>
/// <remarks>
///  <para>
///   <see href="https://learn.microsoft.com/windows/win32/api/commdlg/ns-commdlg-pagesetupdlgw#">
///    Read more on https://learn.microsoft.com.
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
internal partial struct PAGESETUPDLGW
{
    /// <summary>
    ///  <para>Type: <b>DWORD</b> The size, in bytes, of this structure.</para>
    ///  <para>
    ///   <see href="https://learn.microsoft.com/windows/win32/api/commdlg/ns-commdlg-pagesetupdlgw#members">
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
    ///   <see href="https://learn.microsoft.com/windows/win32/api/commdlg/ns-commdlg-pagesetupdlgw#members">
    ///    Read more on https://learn.microsoft.com.
    ///   </see>
    ///  </para>
    /// </summary>
    public HWND hwndOwner;

    /// <summary>
    ///  <para>
    ///   Type: <b>HGLOBAL</b> A handle to a global memory object that contains a
    ///   <a href="https://learn.microsoft.com/windows/win32/api/wingdi/ns-wingdi-devmodea">
    ///   DEVMODE</a> structure. On input, if a handle is specified, the values in the corresponding <b>DEVMODE</b>
    ///   structure are used to initialize the controls in the dialog box. On output, the dialog box sets <b>hDevMode</b>
    ///   to a global memory handle to a <b>DEVMODE</b> structure that contains values specifying the user's selections.
    ///   If the user's selections are not available, the dialog box sets <b>hDevMode</b> to <b>NULL</b>.
    ///  </para>
    ///  <para>
    ///   <see href="https://learn.microsoft.com/windows/win32/api/commdlg/ns-commdlg-pagesetupdlgw#members">
    ///    Read more on https://learn.microsoft.com.
    ///   </see>
    ///  </para>
    /// </summary>
    public HGLOBAL hDevMode;

    /// <summary>
    ///  <para>
    ///   Type: <b>HGLOBAL</b> A handle to a global memory object that contains a
    ///   <a href="https://learn.microsoft.com/windows/desktop/api/commdlg/ns-commdlg-devnames">
    ///   DEVNAMES</a> structure. This structure contains three strings that specify the driver name,
    ///   the printer name, and the output port name. On input, if a handle is specified,
    ///   the strings in the corresponding <b>DEVNAMES</b> structure are used to initialize controls in the dialog box.
    ///   On output, the dialog box sets <b>hDevNames</b> to a global memory handle to a <b>DEVNAMES</b>
    ///   structure that contains strings specifying the user's selections. If the user's selections are not available,
    ///   the dialog box sets <b>hDevNames</b> to <b>NULL</b>.
    ///  </para>
    ///  <para>
    ///   <see href="https://learn.microsoft.com/windows/win32/api/commdlg/ns-commdlg-pagesetupdlgw#members">
    ///    Read more on https://learn.microsoft.com.
    ///   </see>
    ///  </para>
    /// </summary>
    public HGLOBAL hDevNames;

    /// <summary>Type: <b>DWORD</b></summary>
    public PAGESETUPDLG_FLAGS Flags;

    /// <summary>
    ///  <para>
    ///   Type: <b><a href="https://learn.microsoft.com/windows/win32/api/windef/ns-windef-point">
    ///   POINT</a></b> The dimensions of the paper selected by the user.
    ///   The <b>PSD_INTHOUSANDTHSOFINCHES</b> or <b>PSD_INHUNDREDTHSOFMILLIMETERS</b> flag indicates
    ///   the units of measurement.
    ///  </para>
    ///  <para>
    ///   <see href="https://learn.microsoft.com/windows/win32/api/commdlg/ns-commdlg-pagesetupdlgw#members">
    ///    Read more on https://learn.microsoft.com.
    ///   </see>
    ///  </para>
    /// </summary>
    public Point ptPaperSize;

    /// <summary>
    ///  <para>
    ///   Type: <b><a href="https://learn.microsoft.com/windows/desktop/api/windef/ns-windef-rect">
    ///   RECT</a></b> The minimum allowable widths for the left, top, right, and bottom margins.
    ///   The system ignores this member if the <b>PSD_MINMARGINS</b> flag is not set.
    ///   These values must be less than or equal to the values specified in the <b>rtMargin</b> member.
    ///   The <b>PSD_INTHOUSANDTHSOFINCHES</b> or <b>PSD_INHUNDREDTHSOFMILLIMETERS</b>
    ///   flag indicates the units of measurement.
    ///  </para>
    ///  <para>
    ///   <see href="https://learn.microsoft.com/windows/win32/api/commdlg/ns-commdlg-pagesetupdlgw#members">
    ///    Read more on https://learn.microsoft.com.
    ///   </see>
    ///  </para>
    /// </summary>
    public RECT rtMinMargin;

    /// <summary>
    ///  <para>
    ///   Type: <b><a href="https://learn.microsoft.com/windows/desktop/api/windef/ns-windef-rect">
    ///   RECT</a></b> The widths of the left, top, right, and bottom margins. If you set the <b>PSD_MARGINS</b> flag,
    ///   <b>rtMargin</b> specifies the initial margin values.
    ///   When <a href="https://learn.microsoft.com/previous-versions/windows/desktop/legacy/ms646937(v=vs.85)">
    ///   PageSetupDlg</a> returns, <b>rtMargin</b> contains the margin widths selected by the user.
    ///   The <b>PSD_INHUNDREDTHSOFMILLIMETERS</b> or <b>PSD_INTHOUSANDTHSOFINCHES</b>
    ///   flag indicates the units of measurement.
    ///  </para>
    ///  <para>
    ///   <see href="https://learn.microsoft.com/windows/win32/api/commdlg/ns-commdlg-pagesetupdlgw#members">
    ///    Read more on https://learn.microsoft.com.
    ///   </see>
    ///  </para>
    /// </summary>
    public RECT rtMargin;

    /// <summary>
    ///  <para>
    ///   Type: <b>HINSTANCE</b> If the <b>PSD_ENABLEPAGESETUPTEMPLATE</b> flag is set in the <b>Flags</b> member,
    ///   <b>hInstance</b> is a handle to the application or module instance that contains the dialog box template
    ///   named by the <b>lpPageSetupTemplateName</b> member.
    ///  </para>
    ///  <para>
    ///   <see href="https://learn.microsoft.com/windows/win32/api/commdlg/ns-commdlg-pagesetupdlgw#members">
    ///    Read more on https://learn.microsoft.com.
    ///   </see>
    ///  </para>
    /// </summary>
    public HINSTANCE hInstance;

    /// <summary>
    ///  <para>
    ///   Type: <b>LPARAM</b> Application-defined data that the system passes to the hook procedure identified by the
    ///   <b>lpfnPageSetupHook</b> member. When the system sends the
    ///   <a href="https://learn.microsoft.com/windows/desktop/dlgbox/wm-initdialog">WM_INITDIALOG</a>
    ///   message to the hook procedure, the message's <i>lParam</i> parameter is a pointer to the <b>PAGESETUPDLG</b>
    ///   structure specified when the dialog was created. The hook procedure can use this pointer to get the
    ///   <b>lCustData</b> value.
    ///  </para>
    ///  <para>
    ///   <see href="https://learn.microsoft.com/windows/win32/api/commdlg/ns-commdlg-pagesetupdlgw#members">
    ///    Read more on https://learn.microsoft.com.
    ///   </see>
    ///  </para>
    /// </summary>
    public LPARAM lCustData;

    /// <summary>
    ///  <para>
    ///   Type: <b>LPPAGESETUPHOOK</b> A pointer to a
    ///   <a href="https://learn.microsoft.com/windows/desktop/api/commdlg/nc-commdlg-lppagesetuphook">PageSetupHook</a>
    ///   hook procedure that can process messages intended for the dialog box. This member is ignored unless the
    ///   <b>PSD_ENABLEPAGESETUPHOOK</b> flag is set in the <b>Flags</b> member.
    ///  </para>
    ///  <para>
    ///   <see href="https://learn.microsoft.com/windows/win32/api/commdlg/ns-commdlg-pagesetupdlgw#members">
    ///    Read more on https://learn.microsoft.com.
    ///   </see>
    ///  </para>
    /// </summary>
    public unsafe delegate* unmanaged[Stdcall]<HWND, uint, WPARAM, LPARAM, nuint> lpfnPageSetupHook;

    /// <summary>
    ///  <para>
    ///   Type: <b>LPPAGEPAINTHOOK</b> A pointer to a
    ///   <a href="https://learn.microsoft.com/windows/desktop/api/commdlg/nc-commdlg-lppagepainthook">PagePaintHook</a>
    ///   hook procedure that receives <b>WM_PSD_*</b> messages from the dialog box whenever the sample page is redrawn.
    ///   By processing the messages, the hook procedure can customize the appearance of the sample page. This member
    ///   is ignored unless the <b>PSD_ENABLEPAGEPAINTHOOK</b> flag is set in the <b>Flags</b> member.
    ///  </para>
    ///  <para>
    ///   <see href="https://learn.microsoft.com/windows/win32/api/commdlg/ns-commdlg-pagesetupdlgw#members">
    ///    Read more on https://learn.microsoft.com.
    ///   </see>
    ///  </para>
    /// </summary>
    public unsafe delegate* unmanaged[Stdcall]<HWND, uint, WPARAM, LPARAM, nuint> lpfnPagePaintHook;

    /// <summary>
    ///  <para>
    ///   Type: <b>LPCTSTR</b> The name of the dialog box template resource in the module identified by the
    ///   <b>hInstance</b> member. This template is substituted for the standard dialog box template.
    ///   For numbered dialog box resources, <b>lpPageSetupTemplateName</b> can be a value returned by the
    ///   <a href="https://learn.microsoft.com/windows/desktop/api/winuser/nf-winuser-makeintresourcea">
    ///   MAKEINTRESOURCE</a> macro. This member is ignored unless the <b>PSD_ENABLEPAGESETUPTEMPLATE</b> flag is set
    ///   in the <b>Flags</b> member.
    ///  </para>
    ///  <para>
    ///   <see href="https://learn.microsoft.com/windows/win32/api/commdlg/ns-commdlg-pagesetupdlgw#members">
    ///    Read more on https://learn.microsoft.com.
    ///   </see>
    ///  </para>
    /// </summary>
    public PCWSTR lpPageSetupTemplateName;

    /// <summary>
    ///  <para>
    ///   Type: <b>HGLOBAL</b> If the <b>PSD_ENABLEPAGESETUPTEMPLATEHANDLE</b> flag is set in the <b>Flags</b> member,
    ///   <b>hPageSetupTemplate</b> is a handle to a memory object containing a dialog box template.
    ///  </para>
    ///  <para>
    ///   <see href="https://learn.microsoft.com/windows/win32/api/commdlg/ns-commdlg-pagesetupdlgw#members">
    ///    Read more on https://learn.microsoft.com.
    ///   </see>
    ///  </para>
    /// </summary>
    public HGLOBAL hPageSetupTemplate;
}
