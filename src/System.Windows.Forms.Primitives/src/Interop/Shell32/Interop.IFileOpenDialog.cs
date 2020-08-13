// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class Shell32
    {
#pragma warning disable CS0108
        [ComImport]
        [Guid("d57c7288-d4ad-4768-be02-9d969532d960")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IFileOpenDialog : IFileDialog
        {
            [PreserveSig]
            HRESULT Show(
                IntPtr parent);

            [PreserveSig]
            HRESULT SetFileTypes(
                uint cFileTypes,
                [MarshalAs(UnmanagedType.LPArray)] COMDLG_FILTERSPEC[] rgFilterSpec);

            [PreserveSig]
            HRESULT SetFileTypeIndex(
                uint iFileType);

            void GetFileTypeIndex(
                out uint piFileType);

            void Advise(
                IFileDialogEvents pfde,
                out uint pdwCookie);

            void Unadvise(
                uint dwCookie);

            void SetOptions(
                FOS fos);

            void GetOptions(
                out FOS pfos);

            void SetDefaultFolder(
                IShellItem psi);

            void SetFolder(
                IShellItem psi);

            void GetFolder(
                out IShellItem ppsi);

            [PreserveSig]
            HRESULT GetCurrentSelection(
                out IShellItem ppsi);

            void SetFileName(
                [MarshalAs(UnmanagedType.LPWStr)] string pszName);

            void GetFileName(
                [MarshalAs(UnmanagedType.LPWStr)] out string pszName);

            void SetTitle(
                [MarshalAs(UnmanagedType.LPWStr)] string pszTitle);

            [PreserveSig]
            HRESULT SetOkButtonLabel(
                [MarshalAs(UnmanagedType.LPWStr)] string pszText);

            [PreserveSig]
            HRESULT SetFileNameLabel(
                [MarshalAs(UnmanagedType.LPWStr)] string pszLabel);

            void GetResult(
                out IShellItem ppsi);

            [PreserveSig]
            HRESULT AddPlace(
                IShellItem psi,
                FDAP fdap);

            void SetDefaultExtension(
                [MarshalAs(UnmanagedType.LPWStr)] string pszDefaultExtension);

            void Close(
                [MarshalAs(UnmanagedType.Error)] int hr);

            void SetClientGuid(
                ref Guid guid);

            [PreserveSig]
            HRESULT ClearClientData();

            [PreserveSig]
            HRESULT SetFilter(
                IntPtr pFilter);

            void GetResults(
                out IShellItemArray ppenum);

            [PreserveSig]
            HRESULT GetSelectedItems(
                out IShellItemArray ppsai);
        }
#pragma warning restore CS0108
    }
}
