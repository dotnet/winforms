// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32.UI.Controls.Dialogs;

/// <summary>
///  The <see cref="CHOOSECOLORW"/> structure (commdlg.h) contains information the ChooseColor function uses to
///  initialize the Color dialog box.
/// </summary>
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
internal unsafe partial struct CHOOSECOLORW
{
    /// <summary>
    ///  <para>
    ///   Type: <b>DWORD</b> The length, in bytes, of the structure.
    ///  </para>
    ///  <para>
    ///   <see href="https://learn.microsoft.com/windows/win32/api/commdlg/ns-commdlg-choosecolorw#members">
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
    ///   <see href="https://learn.microsoft.com/windows/win32/api/commdlg/ns-commdlg-choosecolorw#members">
    ///    Read more on https://learn.microsoft.com.
    ///   </see>
    ///  </para>
    /// </summary>
    public HWND hwndOwner;

    /// <summary>
    ///  <para>
    ///   Type: <b>HWND</b> If the <b>CC_ENABLETEMPLATEHANDLE</b> flag is set in the <b>Flags</b> member,
    ///   <b>hInstance</b> is a handle to a memory object containing a dialog box template.
    ///   If the <b>CC_ENABLETEMPLATE</b> flag is set, <b>hInstance</b> is a handle to a module that contains
    ///   a dialog box template named by the <b>lpTemplateName</b> member. If neither <b>CC_ENABLETEMPLATEHANDLE</b>
    ///   nor <b>CC_ENABLETEMPLATE</b> is set, this member is ignored.
    ///  </para>
    ///  <para>
    ///   <see href="https://learn.microsoft.com/windows/win32/api/commdlg/ns-commdlg-choosecolorw#members">
    ///    Read more on https://learn.microsoft.com.
    ///   </see>
    ///  </para>
    /// </summary>
    public HWND hInstance;

    /// <summary>
    ///  <para>
    ///   Type: <b><a href="https://learn.microsoft.com/windows/desktop/gdi/colorref">COLORREF</a></b>
    ///   If the <b>CC_RGBINIT</b> flag is set, <b>rgbResult</b> specifies the color initially selected when the
    ///   dialog box is created. If the specified color value is not among the available colors, the system selects
    ///   the nearest solid color available. If <b>rgbResult</b> is zero or <b>CC_RGBINIT</b> is not set,
    ///   the initially selected color is black. If the user clicks the <b>OK</b> button, <b>rgbResult</b>
    ///   specifies the user's color selection. To create a
    ///   <a href="https://learn.microsoft.com/windows/desktop/gdi/colorref">COLORREF</a> color value,
    ///   use the <a href="https://learn.microsoft.com/windows/desktop/api/wingdi/nf-wingdi-rgb">RGB</a> macro.
    ///  </para>
    ///  <para>
    ///   <see href="https://learn.microsoft.com/windows/win32/api/commdlg/ns-commdlg-choosecolorw#members">
    ///    Read more on https://learn.microsoft.com.
    ///   </see>
    ///  </para>
    /// </summary>
    public COLORREF rgbResult;

    /// <summary>
    ///  <para>
    ///   Type: <b><a href="https://learn.microsoft.com/windows/desktop/gdi/colorref">COLORREF</a>*</b> A pointer
    ///   to an array of 16 values that contain red, green, blue (RGB) values for the custom color boxes in the
    ///   dialog box. If the user modifies these colors, the system updates the array with the new RGB values.
    ///   To preserve new custom colors between calls to the
    ///   <a href="https://learn.microsoft.com/previous-versions/windows/desktop/legacy/ms646912(v=vs.85)">
    ///   ChooseColor</a> function, you should allocate static memory for the array. To create a
    ///   <a href="https://learn.microsoft.com/windows/desktop/gdi/colorref">COLORREF</a> color value,
    ///   use the <a href="https://learn.microsoft.com/windows/desktop/api/wingdi/nf-wingdi-rgb">RGB</a> macro.
    ///  </para>
    ///  <para>
    ///   <see href="https://learn.microsoft.com/windows/win32/api/commdlg/ns-commdlg-choosecolorw#members">
    ///    Read more on https://learn.microsoft.com.
    ///   </see>
    ///  </para>
    /// </summary>
    public unsafe COLORREF* lpCustColors;

    /// <summary>
    ///  <para>
    ///   Type: <b>DWORD</b> A set of bit flags that you can use to initialize the <b>Color</b> dialog box.
    ///   When the dialog box returns, it sets these flags to indicate the user's input. This member can be a
    ///   combination of the following flags.
    ///  </para>
    ///  <para>This doc was truncated.</para>
    ///  <para>
    ///   <see href="https://learn.microsoft.com/windows/win32/api/commdlg/ns-commdlg-choosecolorw#members">
    ///    Read more on https://learn.microsoft.com.
    ///   </see>
    ///  </para>
    /// </summary>
    public CHOOSECOLOR_FLAGS Flags;

    /// <summary>
    ///  <para>
    ///   Type: <b>LPARAM</b> Application-defined data that the system passes to the hook procedure identified by
    ///   the <b>lpfnHook</b> member. When the system sends the
    ///   <a href="https://learn.microsoft.com/windows/desktop/dlgbox/wm-initdialog">WM_INITDIALOG</a>
    ///   message to the hook procedure, the message's <i>lParam</i> parameter is a pointer to the <b>CHOOSECOLOR</b>
    ///   structure specified when the dialog was created. The hook procedure can use this pointer to get the
    ///   <b>lCustData</b> value.
    ///  </para>
    ///  <para>
    ///   <see href="https://learn.microsoft.com/windows/win32/api/commdlg/ns-commdlg-choosecolorw#members">
    ///    Read more on https://learn.microsoft.com.
    ///   </see>
    ///  </para>
    /// </summary>
    public LPARAM lCustData;

    /// <summary>
    ///  <para>
    ///   Type: <b>LPCCHOOKPROC</b> A pointer to a
    ///   <a href="https://learn.microsoft.com/windows/desktop/api/commdlg/nc-commdlg-lpcchookproc">CCHookProc</a>
    ///   hook procedure that can process messages intended for the dialog box. This member is ignored unless the
    ///   <b>CC_ENABLEHOOK</b> flag is set in the <b>Flags</b> member.
    ///  </para>
    ///  <para>
    ///   <see href="https://learn.microsoft.com/windows/win32/api/commdlg/ns-commdlg-choosecolorw#members">
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
    ///   MAKEINTRESOURCE</a> macro. This member is ignored unless the <b>CC_ENABLETEMPLATE</b>
    ///   flag is set in the <b>Flags</b> member.
    ///  </para>
    ///  <para>
    ///   <see href="https://learn.microsoft.com/windows/win32/api/commdlg/ns-commdlg-choosecolorw#members">
    ///    Read more on https://learn.microsoft.com.
    ///   </see>
    ///  </para>
    /// </summary>
    public PCWSTR lpTemplateName;
}
