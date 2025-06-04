// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Com;

namespace Windows.Win32.UI.Controls.Dialogs;

/// <summary>
///  Contains information that the PrintDlgEx function uses to initialize the Print property sheet. After the user
///  closes the property sheet, the system uses this structure to return information about the user's selections.
/// </summary>
/// <remarks>
///  <para>
///   <see href="https://learn.microsoft.com/windows/win32/api/commdlg/ns-commdlg-printdlgexw#">
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
internal partial struct PRINTDLGEXW
{
    /// <summary>
    ///  <para>Type: <b>DWORD</b> The structure size, in bytes.</para>
    ///  <para>
    ///   <see href="https://learn.microsoft.com/windows/win32/api/commdlg/ns-commdlg-printdlgexw#members">
    ///    Read more on https://learn.microsoft.com.
    ///   </see>
    ///  </para>
    /// </summary>
    public uint lStructSize;

    /// <summary>
    ///  <para>
    ///   Type: <b>HWND</b> A handle to the window that owns the property sheet. This member must be a valid
    ///   window handle; it cannot be <b>NULL</b>.
    ///  </para>
    ///  <para>
    ///   <see href="https://learn.microsoft.com/windows/win32/api/commdlg/ns-commdlg-printdlgexw#members">
    ///    Read more on https://learn.microsoft.com.
    ///   </see>
    ///  </para>
    /// </summary>
    public HWND hwndOwner;

    /// <summary>
    ///  <para>
    ///   Type: <b>HGLOBAL</b> A handle to a movable global memory object that contains a
    ///   <a href="https://learn.microsoft.com/windows/win32/api/wingdi/ns-wingdi-devmodew">DEVMODE</a> structure.
    ///   If <b>hDevMode</b> is not <b>NULL</b> on input, you must allocate a movable block of memory for the
    ///   <b>DEVMODE</b> structure and initialize its members. The
    ///   <a href="https://learn.microsoft.com/previous-versions/windows/desktop/legacy/ms646942(v=vs.85)">
    ///   PrintDlgEx</a> function uses the input data to initialize the controls in the property sheet.
    ///   When <b>PrintDlgEx</b> returns, the <b>DEVMODE</b> members indicate the user's input.
    ///   If <b>hDevMode</b> is <b>NULL</b> on input,
    ///   <a href="https://learn.microsoft.com/previous-versions/windows/desktop/legacy/ms646942(v=vs.85)">
    ///   PrintDlgEx</a> allocates memory for the
    ///   <a href="https://learn.microsoft.com/windows/win32/api/wingdi/ns-wingdi-devmodea">
    ///   DEVMODE</a> structure, initializes its members to indicate the user's input, and returns a handle that
    ///   identifies it. For more information about the <b>hDevMode</b> and <b>hDevNames</b> members,
    ///   see the Remarks section at the end of this topic.
    ///  </para>
    ///  <para>
    ///   <see href="https://learn.microsoft.com/windows/win32/api/commdlg/ns-commdlg-printdlgexw#members">
    ///    Read more on https://learn.microsoft.com.
    ///   </see>
    ///  </para>
    /// </summary>
    public HGLOBAL hDevMode;

    /// <summary>
    ///  <para>
    ///   Type: <b>HGLOBAL</b> A handle to a movable global memory object that contains a
    ///   <a href="https://learn.microsoft.com/windows/desktop/api/commdlg/ns-commdlg-devnames">DEVNAMES</a> structure.
    ///   If <b>hDevNames</b> is not <b>NULL</b> on input, you must allocate a movable block of memory
    ///   for the <b>DEVNAMES</b> structure and initialize its members. The
    ///   <a href="https://learn.microsoft.com/previous-versions/windows/desktop/legacy/ms646942(v=vs.85)">PrintDlgEx</a>
    ///   function uses the input data to initialize the controls in the property sheet. When <b>PrintDlgEx</b> returns,
    ///   the <b>DEVNAMES</b> members contain information for the printer chosen by the user. You can use this
    ///   information to create a device context or an information context. The <b>hDevNames</b> member can be
    ///   <b>NULL</b>, in which case,
    ///   <a href="https://learn.microsoft.com/previous-versions/windows/desktop/legacy/ms646942(v=vs.85)">
    ///   PrintDlgEx</a> allocates memory for the
    ///   <a href="https://learn.microsoft.com/windows/desktop/api/commdlg/ns-commdlg-devnames">DEVNAMES</a>
    ///   structure, initializes its members to indicate the user's input, and returns a handle that identifies it.
    ///   For more information about the <b>hDevMode</b> and <b>hDevNames</b> members, see the Remarks section at the
    ///   end of this topic.
    ///  </para>
    ///  <para>
    ///   <see href="https://learn.microsoft.com/windows/win32/api/commdlg/ns-commdlg-printdlgexw#members">
    ///    Read more on https://learn.microsoft.com.
    ///   </see>
    ///  </para>
    /// </summary>
    public HGLOBAL hDevNames;

    /// <summary>
    ///  <para>
    ///   Type: <b>HDC</b> A handle to a device context or an information context, depending on whether the <b>Flags</b>
    ///   member specifies the <b>PD_RETURNDC</b> or <b>PC_RETURNIC</b> flag. If neither flag is specified, the value
    ///   of this member is undefined. If both flags are specified, <b>PD_RETURNDC</b> has priority.
    ///  </para>
    ///  <para>
    ///   <see href="https://learn.microsoft.com/windows/win32/api/commdlg/ns-commdlg-printdlgexw#members">
    ///    Read more on https://learn.microsoft.com.
    ///   </see>
    ///  </para>
    /// </summary>
    public HDC hDC;

    /// <summary>Type: <b>DWORD</b></summary>
    public PRINTDLGEX_FLAGS Flags;

    /// <summary>Type: <b>DWORD</b></summary>
    public uint Flags2;

    /// <summary>
    ///  <para>
    ///   Type: <b>DWORD</b> A set of bit flags that can exclude items from the printer driver property pages in the
    ///   <b>Print</b> property sheet. This value is used only if the <b>PD_EXCLUSIONFLAGS</b> flag is set in the
    ///   <b>Flags</b> member. Exclusion flags should be used only if the item to be excluded will be included on
    ///   either the <b>General</b> page or on an application-defined page in the <b>Print</b> property sheet.
    ///   This member can specify the following flag.
    ///  </para>
    ///  <para>
    ///   <see href="https://learn.microsoft.com/windows/win32/api/commdlg/ns-commdlg-printdlgexw#members">
    ///    Read more on https://learn.microsoft.com.
    ///   </see>
    ///  </para>
    /// </summary>
    public uint ExclusionFlags;

    /// <summary>
    ///  <para>
    ///   Type: <b>DWORD</b> On input, set this member to the initial number of page ranges specified in the
    ///   <b>lpPageRanges</b> array. When the
    ///   <a href="https://learn.microsoft.com/previous-versions/windows/desktop/legacy/ms646942(v=vs.85)">
    ///   PrintDlgEx</a> function returns, <b>nPageRanges</b> indicates the number of user-specified page ranges stored
    ///   in the <b>lpPageRanges</b> array. If the <b>PD_NOPAGENUMS</b> flag is specified, this value is not valid.
    ///  </para>
    ///  <para>
    ///   <see href="https://learn.microsoft.com/windows/win32/api/commdlg/ns-commdlg-printdlgexw#members">
    ///    Read more on https://learn.microsoft.com.
    ///   </see>
    ///  </para>
    /// </summary>
    public uint nPageRanges;

    /// <summary>
    ///  <para>
    ///   Type: <b>DWORD</b> The size, in array elements, of the <b>lpPageRanges</b> buffer. This value indicates
    ///   the maximum number of page ranges that can be stored in the array. If the <b>PD_NOPAGENUMS</b> flag
    ///   is specified, this value is not valid. If the <b>PD_NOPAGENUMS</b> flag is not specified, this
    ///   value must be greater than zero.
    ///  </para>
    ///  <para>
    ///   <see href="https://learn.microsoft.com/windows/win32/api/commdlg/ns-commdlg-printdlgexw#members">
    ///    Read more on https://learn.microsoft.com.
    ///   </see>
    ///  </para>
    /// </summary>
    public uint nMaxPageRanges;

    /// <summary>
    ///  <para>
    ///   Type: <b>LPPRINTPAGERANGE</b> Pointer to a buffer containing an array of
    ///   <a href="https://learn.microsoft.com/windows/desktop/api/commdlg/ns-commdlg-printpagerange">PRINTPAGERANGE</a>
    ///   structures. On input, the array contains the initial page ranges to display in the <b>Pages</b> edit control.
    ///   When the <a href="https://learn.microsoft.com/previous-versions/windows/desktop/legacy/ms646942(v=vs.85)">
    ///   PrintDlgEx</a> function returns, the array contains the page ranges specified by the user. If the
    ///   <b>PD_NOPAGENUMS</b> flag is specified, this value is not valid. If the <b>PD_NOPAGENUMS</b> flag is not
    ///   specified, <b>lpPageRanges</b> must be non-<b>NULL</b>.
    ///  </para>
    ///  <para>
    ///   <see href="https://learn.microsoft.com/windows/win32/api/commdlg/ns-commdlg-printdlgexw#members">
    ///    Read more on https://learn.microsoft.com.
    ///   </see>
    ///  </para>
    /// </summary>
    public unsafe PRINTPAGERANGE* lpPageRanges;

    /// <summary>
    ///  <para>
    ///   Type: <b>DWORD</b> The minimum value for the page ranges specified in the <b>Pages</b> edit control.
    ///   If the <b>PD_NOPAGENUMS</b> flag is specified, this value is not valid.
    ///  </para>
    ///  <para>
    ///   <see href="https://learn.microsoft.com/windows/win32/api/commdlg/ns-commdlg-printdlgexw#members">
    ///    Read more on https://learn.microsoft.com.
    ///   </see>
    ///  </para>
    /// </summary>
    public uint nMinPage;

    /// <summary>
    ///  <para>
    ///   Type: <b>DWORD</b> The maximum value for the page ranges specified in the <b>Pages</b> edit control.
    ///   If the <b>PD_NOPAGENUMS</b> flag is specified, this value is not valid.
    ///  </para>
    ///  <para>
    ///   <see href="https://learn.microsoft.com/windows/win32/api/commdlg/ns-commdlg-printdlgexw#members">
    ///    Read more on https://learn.microsoft.com.
    ///   </see>
    ///  </para>
    /// </summary>
    public uint nMaxPage;

    /// <summary>
    ///  <para>
    ///   Type: <b>DWORD</b> Contains the initial number of copies for the <b>Copies</b> edit control if <b>hDevMode</b>
    ///   is <b>NULL</b>; otherwise, the <b>dmCopies</b> member of the
    ///   <a href="https://learn.microsoft.com/windows/win32/api/wingdi/ns-wingdi-devmodea">DEVMODE</a> structure
    ///   contains the initial value.
    ///   When <a href="https://learn.microsoft.com/previous-versions/windows/desktop/legacy/ms646942(v=vs.85)">
    ///   PrintDlgEx</a> returns, <b>nCopies</b> contains the actual number of copies the application must print.
    ///   This value depends on whether the application or the printer driver is responsible for
    ///   printing multiple copies. If the <b>PD_USEDEVMODECOPIESANDCOLLATE</b> flag is set in the <b>Flags</b>
    ///   member, <b>nCopies</b> is always 1 on return, and the printer driver is responsible for printing
    ///   multiple copies. If the flag is not set, the application is responsible for printing the number of copies
    ///   specified by <b>nCopies</b>. For more information, see the description of the
    ///   <b>PD_USEDEVMODECOPIESANDCOLLATE</b> flag.
    ///  </para>
    ///  <para>
    ///   <see href="https://learn.microsoft.com/windows/win32/api/commdlg/ns-commdlg-printdlgexw#members">
    ///    Read more on https://learn.microsoft.com.
    ///   </see>
    ///  </para>
    /// </summary>
    public uint nCopies;

    /// <summary>
    ///  <para>
    ///   Type: <b>HINSTANCE</b> If the <b>PD_ENABLEPRINTTEMPLATE</b> flag is set in the <b>Flags</b> member,
    ///   <b>hInstance</b> is a handle to the application or module instance that contains the dialog box
    ///   template named by the <b>lpPrintTemplateName</b> member. If the <b>PD_ENABLEPRINTTEMPLATEHANDLE</b> flag
    ///   is set in the <b>Flags</b> member, <b>hInstance</b> is a handle to a memory object containing a
    ///   dialog box template. If neither of the template flags is set in the <b>Flags</b> member,
    ///   <b>hInstance</b> should be <b>NULL</b>.
    ///  </para>
    ///  <para>
    ///   <see href="https://learn.microsoft.com/windows/win32/api/commdlg/ns-commdlg-printdlgexw#members">
    ///    Read more on https://learn.microsoft.com.
    ///   </see>
    ///  </para>
    /// </summary>
    public HINSTANCE hInstance;

    /// <summary>
    ///  <para>
    ///   Type: <b>LPCTSTR</b> The name of the dialog box template resource in the module identified by the
    ///   <b>hInstance</b> member. This template replaces the default dialog box template in the lower portion
    ///   of the <b>General</b> page. The default template contains controls similar to those of the
    ///   <b>Print</b> dialog box. This member is ignored unless the PD_ENABLEPRINTTEMPLATE flag is set in the
    ///   <b>Flags</b> member.
    ///  </para>
    ///  <para>
    ///   <see href="https://learn.microsoft.com/windows/win32/api/commdlg/ns-commdlg-printdlgexw#members">
    ///    Read more on https://learn.microsoft.com.
    ///   </see>
    ///  </para>
    /// </summary>
    public PCWSTR lpPrintTemplateName;

    /// <summary>
    ///  <para>
    ///   Type: <b>LPUNKNOWN</b> A pointer to an application-defined callback object. The object should contain the
    ///   <a href="https://learn.microsoft.com/windows/desktop/api/commdlg/nn-commdlg-iprintdialogcallback">
    ///   IPrintDialogCallback</a> class to receive messages for the child dialog box in the lower portion of the
    ///   <b>General</b> page. The callback object should also contain the
    ///   <a href="https://learn.microsoft.com/windows/desktop/api/ocidl/nn-ocidl-iobjectwithsite">
    ///   IObjectWithSite</a> class to receive a pointer to the
    ///   <a href="https://learn.microsoft.com/windows/desktop/api/commdlg/nn-commdlg-iprintdialogservices">
    ///   IPrintDialogServices</a> interface.
    ///   The <a href="https://learn.microsoft.com/previous-versions/windows/desktop/legacy/ms646942(v=vs.85)">
    ///   PrintDlgEx</a> function calls
    ///   <a href="https://learn.microsoft.com/windows/desktop/api/unknwn/nf-unknwn-iunknown-queryinterface(q)">
    ///   IUnknown::QueryInterface</a> on the callback object for both <b>IID_IPrintDialogCallback</b> and
    ///   <b>IID_IObjectWithSite</b> to determine which interfaces are supported. If you do not want to retrieve any
    ///   of the callback information, set <b>lpCallback</b> to <b>NULL</b>.
    ///  </para>
    ///  <para>
    ///   <see href="https://learn.microsoft.com/windows/win32/api/commdlg/ns-commdlg-printdlgexw#members">
    ///    Read more on https://learn.microsoft.com.
    ///   </see>
    ///  </para>
    /// </summary>
    public unsafe IUnknown* lpCallback;

    /// <summary>
    ///  <para>Type: <b>DWORD</b> The number of property page handles in the <b>lphPropertyPages</b> array.
    ///  </para>
    ///  <para>
    ///   <see href="https://learn.microsoft.com/windows/win32/api/commdlg/ns-commdlg-printdlgexw#members">
    ///    Read more on https://learn.microsoft.com.
    ///   </see>
    ///  </para>
    /// </summary>
    public uint nPropertyPages;

    /// <summary>
    ///  <para>
    ///   Type: <b>HPROPSHEETPAGE*</b> Contains an array of property page handles to add to the <b>Print</b>
    ///   property sheet. The additional property pages follow the <b>General</b> page. Use the
    ///   <a href="https://learn.microsoft.com/windows/desktop/api/prsht/nf-prsht-createpropertysheetpagea">
    ///   CreatePropertySheetPage</a> function to create these additional pages. When the
    ///   <a href="https://learn.microsoft.com/previous-versions/windows/desktop/legacy/ms646942(v=vs.85)">
    ///   PrintDlgEx</a> function returns, all the <b>HPROPSHEETPAGE</b> handles in the <b>lphPropertyPages</b>
    ///   array have been destroyed. If <b>nPropertyPages</b> is zero, <b>lphPropertyPages</b> should be <b>NULL</b>.
    ///  </para>
    ///  <para>
    ///   <see href="https://learn.microsoft.com/windows/win32/api/commdlg/ns-commdlg-printdlgexw#members">
    ///    Read more on https://learn.microsoft.com.
    ///   </see>
    ///  </para>
    /// </summary>
    public unsafe HPROPSHEETPAGE* lphPropertyPages;

    /// <summary>
    ///  <para>
    ///   Type: <b>DWORD</b> The property page that is initially displayed. To display the <b>General</b> page, specify
    ///   <b>START_PAGE_GENERAL</b>. Otherwise, specify the zero-based index of a property page in the array specified
    ///   in the <b>lphPropertyPages</b> member. For consistency, it is recommended that the property sheet always be
    ///   started on the <b>General</b> page.
    ///  </para>
    ///  <para>
    ///   <see href="https://learn.microsoft.com/windows/win32/api/commdlg/ns-commdlg-printdlgexw#members">
    ///    Read more on https://learn.microsoft.com.
    ///   </see>
    ///  </para>
    /// </summary>
    public uint nStartPage;

    /// <summary>Type: <b>DWORD</b></summary>
    public uint dwResultAction;
}
