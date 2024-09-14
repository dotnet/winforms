// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32.UI.Controls.Dialogs;

/// <summary>
///  Contains information that the ChooseFont function uses to initialize the Font dialog box. After the user closes the
///  dialog box, the system returns information about the user's selection in this structure.</summary>
/// <remarks>
///  <para>
///   <see href="https://learn.microsoft.com/windows/win32/api/commdlg/ns-commdlg-choosefontw#">
///    Read more on https://learn.microsoft.com.
///   </see>
///  </para>
/// </remarks>
internal partial struct CHOOSEFONTW
{
    /// <summary>
    ///  <para>Type: <b>DWORD</b> The length of the structure, in bytes.</para>
    ///  <para>
    ///   <see href="https://learn.microsoft.com/windows/win32/api/commdlg/ns-commdlg-choosefontw#members">
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
    ///   <see href="https://learn.microsoft.com/windows/win32/api/commdlg/ns-commdlg-choosefontw#members">
    ///    Read more on https://learn.microsoft.com.
    ///   </see>
    ///  </para>
    /// </summary>
    public HWND hwndOwner;

    /// <summary>
    ///  <para>
    ///   Type: <b>HDC</b> This member is ignored by the
    ///   <a href="https://learn.microsoft.com/previous-versions/windows/desktop/legacy/ms646914(v=vs.85)">
    ///   ChooseFont</a> function. <b>Windows Vista and Windows XP/2000:  </b>A handle to the device context or
    ///   information context of the printer whose fonts will be listed in the dialog box.
    ///   This member is used only if the <b>Flags</b> member specifies the <b>CF_PRINTERFONTS</b> or <b>CF_BOTH</b>
    ///   flag; otherwise, this member is ignored.
    ///  </para>
    ///  <para>
    ///   <see href="https://learn.microsoft.com/windows/win32/api/commdlg/ns-commdlg-choosefontw#members">
    ///    Read more on https://learn.microsoft.com.
    ///   </see>
    ///  </para>
    /// </summary>
    public HDC hDC;

    /// <summary>
    ///  <para>
    ///   Type: <b>LPLOGFONT</b> A pointer to a
    ///   <a href="https://learn.microsoft.com/windows/desktop/api/wingdi/ns-wingdi-logfonta">
    ///   LOGFONT</a> structure. If you set the <b>CF_INITTOLOGFONTSTRUCT</b> flag in the <b>Flags</b> member and
    ///   initialize the other members, the
    ///   <a href="https://learn.microsoft.com/previous-versions/windows/desktop/legacy/ms646914(v=vs.85)">
    ///   ChooseFont</a> function initializes the dialog box with a font that matches the <b>LOGFONT</b> members.
    ///   If the user clicks the <b>OK</b> button, <b>ChooseFont</b> sets the members of the <b>LOGFONT</b>
    ///   structure based on the user's selections.
    ///  </para>
    ///  <para>
    ///   <see href="https://learn.microsoft.com/windows/win32/api/commdlg/ns-commdlg-choosefontw#members">
    ///    Read more on https://learn.microsoft.com.
    ///   </see>
    ///  </para>
    /// </summary>
    public unsafe LOGFONTW* lpLogFont;

    /// <summary>
    ///  <para>
    ///   Type: <b>INT</b> The size of the selected font, in units of 1/10 of a point. The
    ///   <a href="https://learn.microsoft.com/previous-versions/windows/desktop/legacy/ms646914(v=vs.85)">
    ///   ChooseFont</a> function sets this value after the user closes the dialog box.
    ///  </para>
    ///  <para>
    ///   <see href="https://learn.microsoft.com/windows/win32/api/commdlg/ns-commdlg-choosefontw#members">
    ///    Read more on https://learn.microsoft.com.
    ///   </see>
    ///  </para>
    /// </summary>
    public int iPointSize;

    /// <summary>Type: <b>DWORD</b></summary>
    public CHOOSEFONT_FLAGS Flags;

    /// <summary>
    ///  <para>
    ///   Type: <b><a href="https://learn.microsoft.com/windows/desktop/gdi/colorref">
    ///   COLORREF</a></b> If the <b>CF_EFFECTS</b> flag is set, <b>rgbColors</b> specifies the initial text color.
    ///   When <a href="https://learn.microsoft.com/previous-versions/windows/desktop/legacy/ms646914(v=vs.85)">
    ///   ChooseFont</a> returns successfully, this member contains the RGB value of the text color that the user selected.
    ///   To create a <a href="https://learn.microsoft.com/windows/desktop/gdi/colorref">COLORREF</a> color value,
    ///   use the <a href="https://learn.microsoft.com/windows/desktop/api/wingdi/nf-wingdi-rgb">RGB</a> macro.
    ///  </para>
    ///  <para>
    ///   <see href="https://learn.microsoft.com/windows/win32/api/commdlg/ns-commdlg-choosefontw#members">
    ///    Read more on https://learn.microsoft.com.
    ///   </see>
    ///  </para>
    /// </summary>
    public COLORREF rgbColors;

    /// <summary>
    ///  <para>
    ///   Type: <b>LPARAM</b> Application-defined data that the system passes to the hook procedure identified by the
    ///   <b>lpfnHook</b> member. When the system sends the
    ///   <a href="https://learn.microsoft.com/windows/desktop/dlgbox/wm-initdialog">WM_INITDIALOG</a> message to
    ///   the hook procedure, the message's <i>lParam</i> parameter is a pointer to the
    ///   <a href="https://learn.microsoft.com/previous-versions/windows/desktop/legacy/ms646914(v=vs.85)">
    ///   CHOOSEFONT</a> structure specified when the dialog was created. The hook procedure can use this pointer
    ///   to get the <b>lCustData</b> value.
    ///  </para>
    ///  <para>
    ///   <see href="https://learn.microsoft.com/windows/win32/api/commdlg/ns-commdlg-choosefontw#members">
    ///    Read more on https://learn.microsoft.com.
    ///   </see>
    ///  </para>
    /// </summary>
    public LPARAM lCustData;

    /// <summary>
    ///  <para>
    ///   Type: <b>LPCFHOOKPROC</b> A pointer to a
    ///   <a href="https://learn.microsoft.com/windows/desktop/api/commdlg/nc-commdlg-lpcfhookproc">
    ///   CFHookProc</a> hook procedure that can process messages intended for the dialog box. This member is ignored
    ///   unless the <b>CF_ENABLEHOOK</b> flag is set in the <b>Flags</b> member.
    ///  </para>
    ///  <para>
    ///   <see href="https://learn.microsoft.com/windows/win32/api/commdlg/ns-commdlg-choosefontw#members">
    ///    Read more on https://learn.microsoft.com.
    ///   </see>
    ///  </para>
    /// </summary>
    public unsafe delegate* unmanaged[Stdcall]<HWND, uint, WPARAM, LPARAM, nuint> lpfnHook;

    /// <summary>
    ///  <para>
    ///   Type: <b>LPCTSTR</b> The name of the dialog box template resource in the module identified by the
    ///   <b>hInstance</b> member. This template is substituted for the standard dialog box template.
    ///   For numbered dialog box resources, <b>lpTemplateName</b> can be a value returned by the
    ///   <a href="https://learn.microsoft.com/windows/desktop/api/winuser/nf-winuser-makeintresourcea">
    ///   MAKEINTRESOURCE</a> macro. This member is ignored unless the <b>CF_ENABLETEMPLATE</b> flag is set in the
    ///   <b>Flags</b> member.
    ///  </para>
    ///  <para>
    ///   <see href="https://learn.microsoft.com/windows/win32/api/commdlg/ns-commdlg-choosefontw#members">
    ///    Read more on https://learn.microsoft.com.
    ///   </see>
    ///  </para>
    /// </summary>
    public PCWSTR lpTemplateName;

    /// <summary>
    ///  <para>
    ///   Type: <b>HINSTANCE</b> If the <b>CF_ENABLETEMPLATEHANDLE</b> flag is set in the <b>Flags</b> member,
    ///   <b>hInstance</b> is a handle to a memory object containing a dialog box template. If the
    ///   <b>CF_ENABLETEMPLATE</b> flag is set, <b>hInstance</b> is a handle to a module that contains a dialog box
    ///   template named by the <b>lpTemplateName</b> member. If neither <b>CF_ENABLETEMPLATEHANDLE</b> nor
    ///   <b>CF_ENABLETEMPLATE</b> is set, this member is ignored.
    ///  </para>
    ///  <para>
    ///   <see href="https://learn.microsoft.com/windows/win32/api/commdlg/ns-commdlg-choosefontw#members">
    ///    Read more on https://learn.microsoft.com.
    ///   </see>
    ///  </para>
    /// </summary>
    public HINSTANCE hInstance;

    /// <summary>
    ///  <para>
    ///   Type: <b>LPTSTR</b> The style data. If the <b>CF_USESTYLE</b> flag is specified,
    ///   <a href="https://learn.microsoft.com/previous-versions/windows/desktop/legacy/ms646914(v=vs.85)">ChooseFont</a>
    ///   uses the data in this buffer to initialize the <b>Font Style</b> combo box. When the user closes the
    ///   dialog box, <b>ChooseFont</b> copies the string in the <b>Font Style</b> combo box into this buffer.
    ///  </para>
    ///  <para>
    ///   <see href="https://learn.microsoft.com/windows/win32/api/commdlg/ns-commdlg-choosefontw#members">
    ///    Read more on https://learn.microsoft.com.
    ///  </see>
    ///  </para>
    /// </summary>
    public PWSTR lpszStyle;

    /// <summary>Type: <b>WORD</b></summary>
    public CHOOSEFONT_FONT_TYPE nFontType;

    public ushort ___MISSING_ALIGNMENT__;

    /// <summary>
    ///  <para>
    ///   Type: <b>INT</b> The minimum point size a user can select.
    ///   <a href="https://learn.microsoft.com/previous-versions/windows/desktop/legacy/ms646914(v=vs.85)">
    ///   ChooseFont</a> recognizes this member only if the <b>CF_LIMITSIZE</b> flag is specified.
    ///  </para>
    ///  <para>
    ///   <see href="https://learn.microsoft.com/windows/win32/api/commdlg/ns-commdlg-choosefontw#members">
    ///    Read more on https://learn.microsoft.com.
    ///   </see>
    ///  </para>
    /// </summary>
    public int nSizeMin;

    /// <summary>
    ///  <para>
    ///   Type: <b>INT</b> The maximum point size a user can select.
    ///   <a href="https://learn.microsoft.com/previous-versions/windows/desktop/legacy/ms646914(v=vs.85)">
    ///   ChooseFont</a> recognizes this member only if the <b>CF_LIMITSIZE</b> flag is specified.
    ///  </para>
    ///  <para>
    ///   <see href="https://learn.microsoft.com/windows/win32/api/commdlg/ns-commdlg-choosefontw#members">
    ///    Read more on https://learn.microsoft.com.
    ///   </see>
    ///  </para>
    /// </summary>
    public int nSizeMax;
}
