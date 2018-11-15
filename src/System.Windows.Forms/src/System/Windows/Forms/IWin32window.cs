// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.Windows.Forms {

    using System.Diagnostics;
    using System;

    /// <include file='doc\IWin32window.uex' path='docs/doc[@for="IWin32Window"]/*' />
    /// <devdoc>
    ///    <para>Provides an interface to expose Win32 HWND handles.</para>
    /// </devdoc>
    [System.Runtime.InteropServices.Guid("458AB8A2-A1EA-4d7b-8EBE-DEE5D3D9442C"), System.Runtime.InteropServices.InterfaceTypeAttribute(System.Runtime.InteropServices.ComInterfaceType.InterfaceIsIUnknown)]
    [System.Runtime.InteropServices.ComVisible(true)]
    public interface IWin32Window {
    
        /// <include file='doc\IWin32window.uex' path='docs/doc[@for="IWin32Window.Handle"]/*' />
        /// <devdoc>
        ///    <para>Gets the handle to the window represented by the implementor.</para>
        /// </devdoc>
        IntPtr Handle { get; }
    }
}
