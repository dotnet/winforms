// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
    // Desktop Framework uses GUID 458AB8A2-A1EA-4d7b-8EBE-DEE5D3D9442C
    // If COM interop with Desktop Framework is ever required a second interface
    // must be declared as ComImport and implemented on all relevant classes.
    [Guid("6608AC34-D51E-4CCF-BD1F-E994D128648B")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [ComVisible(true)]
    public interface IWin32Window
    {
        /// <summary>
        ///  Gets the handle to the window represented by the implementor.
        /// </summary>
        IntPtr Handle { get; }
    }
}
