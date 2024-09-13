// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;
using Windows.Win32.UI.Shell;

namespace Windows.Win32;

internal partial class PInvoke
{
    // Copies from CsWin32, which won't generate this as SHSTOCKICONINFO isn't technically safe for AnyCPU
    // (see SHSTOCKICONINFO.cs for more details).

    /// <summary>Retrieves information about system-defined Shell icons.</summary>
    ///  <param name="siid">
    ///   <para>Type: <b><a href="https://learn.microsoft.com/windows/desktop/api/shellapi/ne-shellapi-shstockiconid">
    ///   SHSTOCKICONID</a></b> One of the values from the
    ///   <a href="https://learn.microsoft.com/windows/desktop/api/shellapi/ne-shellapi-shstockiconid">
    ///   SHSTOCKICONID</a> enumeration that specifies which icon should be retrieved.
    ///   </para>
    ///   <para>
    ///    <see href="https://learn.microsoft.com/windows/win32/api/shellapi/nf-shellapi-shgetstockiconinfo#parameters">
    ///     Read more on https://learn.microsoft.com.
    ///    </see>
    ///   </para>
    ///  </param>
    ///   <param name="uFlags">
    ///   <para>
    ///    Type: <b>UINT</b> A combination of zero or more of the following flags that specify which information is requested.
    ///   </para>
    ///   <para>
    ///    <see href="https://learn.microsoft.com/windows/win32/api/shellapi/nf-shellapi-shgetstockiconinfo#parameters">
    ///     Read more on https://learn.microsoft.com.
    ///    </see>
    ///   </para>
    ///  </param>
    ///  <param name="psii">
    ///   <para>Type: <b><a href="https://learn.microsoft.com/windows/desktop/api/shellapi/ns-shellapi-shstockiconinfo">
    ///    SHSTOCKICONINFO</a>*</b> A pointer to a
    ///    <a href="https://learn.microsoft.com/windows/desktop/api/shellapi/ns-shellapi-shstockiconinfo">
    ///    SHSTOCKICONINFO</a> structure. When this function is called, the <b>cbSize</b> member of this structure
    ///    needs to be set to the size of the <b>SHSTOCKICONINFO</b> structure. When this function returns,
    ///    contains a pointer to a <b>SHSTOCKICONINFO</b> structure that contains the requested information.
    ///  </para>
    ///  <para>
    ///   <see href="https://learn.microsoft.com/windows/win32/api/shellapi/nf-shellapi-shgetstockiconinfo#parameters">
    ///    Read more on https://learn.microsoft.com.
    ///   </see>
    ///  </para>
    /// </param>
    /// <returns>
    ///  <para>
    ///   Type: <b>HRESULT</b> If this function succeeds, it returns <b>S_OK</b>. Otherwise, it returns an <b>HRESULT</b> error code.
    ///  </para>
    /// </returns>
    /// <remarks>
    ///  <para>
    ///   If this function returns an icon handle in the <b>hIcon</b> member of the
    ///   <a href="https://learn.microsoft.com/windows/desktop/api/shellapi/ns-shellapi-shstockiconinfo">SHSTOCKICONINFO</a>
    ///   structure pointed to by <i>psii</i>, you are responsible for freeing the icon with
    ///   <a href="https://learn.microsoft.com/windows/desktop/api/winuser/nf-winuser-destroyicon">DestroyIcon</a>
    ///   when you no longer need it.
    ///  </para>
    /// </remarks>
    [DllImport(Libraries.Shell32, ExactSpelling = true)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    public static extern unsafe HRESULT SHGetStockIconInfo(SHSTOCKICONID siid, SHGSI_FLAGS uFlags, SHSTOCKICONINFO* psii);
}
