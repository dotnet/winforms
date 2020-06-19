// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class Shell32
    {
        /// <remarks>
        ///  Some of these callbacks are cancelable - returning S_FALSE means that the dialog should
        ///  not proceed (e.g. with closing, changing folder); to support this, we need to use the
        ///  PreserveSig attribute to enable us to return the proper HRESULT
        /// </remarks>
        [ComImport]
        [Guid("973510DB-7D7F-452B-8975-74A85828D354")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public unsafe interface IFileDialogEvents
        {
            [PreserveSig]
            HRESULT OnFileOk(
                IFileDialog pfd);

            [PreserveSig]
            HRESULT OnFolderChanging(
                IFileDialog pfd,
                IShellItem psiFolder);

            [PreserveSig]
            HRESULT OnFolderChange(
                IFileDialog pfd);

            [PreserveSig]
            HRESULT OnSelectionChange(
                IFileDialog pfd);

            [PreserveSig]
            HRESULT OnShareViolation(
                IFileDialog pfd,
                IShellItem psi,
                FDESVR* pResponse);

            [PreserveSig]
            HRESULT OnTypeChange(
                IFileDialog pfd);

            [PreserveSig]
            HRESULT OnOverwrite(
                IFileDialog pfd,
                IShellItem psi,
                FDEOR* pResponse);
        }
    }
}
