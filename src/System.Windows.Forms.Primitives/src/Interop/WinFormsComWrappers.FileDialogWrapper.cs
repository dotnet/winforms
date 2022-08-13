// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using System.Windows.Forms;

internal partial class Interop
{
    internal unsafe partial class WinFormsComWrappers
    {
        internal abstract class FileDialogWrapper : Shell32.IFileDialog, IDisposable
        {
            protected IntPtr _wrappedInstance;

            public FileDialogWrapper(IntPtr wrappedInstance)
            {
                _wrappedInstance = wrappedInstance.OrThrowIfZero();
            }

            internal IntPtr Instance => _wrappedInstance;

            public void Dispose()
            {
                Marshal.Release(_wrappedInstance);
                _wrappedInstance = IntPtr.Zero;
            }

            public HRESULT Show(IntPtr parent)
            {
                return ((delegate* unmanaged<IntPtr, IntPtr, HRESULT>)(*(*(void***)_wrappedInstance + 3)))
                    (_wrappedInstance, parent);
            }

            HRESULT Shell32.IFileDialog.SetFileTypes(uint cFileTypes, Shell32.COMDLG_FILTERSPEC[] rgFilterSpec)
            {
                ref Shell32.COMDLG_FILTERSPEC rgFilterSpecRef = ref rgFilterSpec == null
                    ? ref *(Shell32.COMDLG_FILTERSPEC*)0
                    : ref MemoryMarshal.GetArrayDataReference(rgFilterSpec);
                fixed (Shell32.COMDLG_FILTERSPEC* rgFilterSpec_local = &rgFilterSpecRef)
                {
                    return ((delegate* unmanaged<IntPtr, uint, Shell32.COMDLG_FILTERSPEC*, HRESULT>)(*(*(void***)_wrappedInstance + 4)))
                        (_wrappedInstance, cFileTypes, rgFilterSpec_local);
                }
            }

            HRESULT Shell32.IFileDialog.SetFileTypeIndex(uint iFileType)
            {
                return ((delegate* unmanaged<IntPtr, uint, HRESULT>)(*(*(void***)_wrappedInstance + 5)))
                    (_wrappedInstance, iFileType);
            }

            public void GetFileTypeIndex(out uint piFileType)
            {
                fixed (uint* piFileType_local = &piFileType)
                {
                    ((delegate* unmanaged<IntPtr, uint*, HRESULT>)(*(*(void***)_wrappedInstance + 6)))
                        (_wrappedInstance, piFileType_local).ThrowIfFailed();
                }
            }

            void Shell32.IFileDialog.Advise(Shell32.IFileDialogEvents pfde, out uint pdwCookie)
            {
                IntPtr pfde_local = WinFormsComWrappers.Instance.GetComPointer(pfde, IID.IFileDialogEvents);
                fixed (uint* pdwCookie_local = &pdwCookie)
                {
                    ((delegate* unmanaged<IntPtr, IntPtr, uint*, HRESULT>)(*(*(void***)_wrappedInstance + 7)))
                        (_wrappedInstance, pfde_local, pdwCookie_local).ThrowIfFailed();
                }
            }

            void Shell32.IFileDialog.Unadvise(uint dwCookie)
            {
                ((delegate* unmanaged<IntPtr, uint, HRESULT>)(*(*(void***)_wrappedInstance + 8)))
                    (_wrappedInstance, dwCookie).ThrowIfFailed();
            }

            void Shell32.IFileDialog.SetOptions(Shell32.FOS fos)
            {
                ((delegate* unmanaged<IntPtr, Shell32.FOS, HRESULT>)(*(*(void***)_wrappedInstance + 9)))
                    (_wrappedInstance, fos).ThrowIfFailed();
            }

            void Shell32.IFileDialog.GetOptions(out Shell32.FOS pfos)
            {
                Shell32.FOS pfos_local;
                ((delegate* unmanaged<IntPtr, Shell32.FOS*, HRESULT>)(*(*(void***)_wrappedInstance + 10)))
                    (_wrappedInstance, &pfos_local).ThrowIfFailed();
                pfos = pfos_local;
            }

            void Shell32.IFileDialog.SetDefaultFolder(Shell32.IShellItem psi)
            {
                IntPtr psi_local = WinFormsComWrappers.Instance.GetComPointer(psi, IID.IShellItem);
                ((delegate* unmanaged<IntPtr, IntPtr, HRESULT>)(*(*(void***)_wrappedInstance + 11)))
                    (_wrappedInstance, psi_local).ThrowIfFailed();
            }

            void Shell32.IFileDialog.SetFolder(Shell32.IShellItem psi)
            {
                IntPtr psi_local = WinFormsComWrappers.Instance.GetComPointer(psi, IID.IShellItem);
                ((delegate* unmanaged<IntPtr, IntPtr, HRESULT>)(*(*(void***)_wrappedInstance + 12)))
                    (_wrappedInstance, psi_local).ThrowIfFailed();
            }

            void Shell32.IFileDialog.GetFolder(out Shell32.IShellItem? ppsi)
            {
                IntPtr ppsi_local;
                ((delegate* unmanaged<IntPtr, IntPtr*, HRESULT>)(*(*(void***)_wrappedInstance + 13)))
                    (_wrappedInstance, &ppsi_local).ThrowIfFailed();
                ppsi = ppsi_local == IntPtr.Zero ? null : (Shell32.IShellItem)WinFormsComWrappers.Instance.GetOrCreateObjectForComInstance(ppsi_local, CreateObjectFlags.Unwrap);
            }

            HRESULT Shell32.IFileDialog.GetCurrentSelection(out Shell32.IShellItem? ppsi)
            {
                IntPtr ppsi_local;
                HRESULT result = ((delegate* unmanaged<IntPtr, IntPtr*, HRESULT>)(*(*(void***)_wrappedInstance + 14)))
                    (_wrappedInstance, &ppsi_local);
                ppsi = ppsi_local == IntPtr.Zero ? null : (Shell32.IShellItem)WinFormsComWrappers.Instance.GetOrCreateObjectForComInstance(ppsi_local, CreateObjectFlags.Unwrap);
                return result;
            }

            void Shell32.IFileDialog.SetFileName(string pszName)
            {
                ArgumentNullException.ThrowIfNull(pszName);
                fixed (char* pszName_local = pszName)
                {
                    ((delegate* unmanaged<IntPtr, char*, HRESULT>)(*(*(void***)_wrappedInstance + 15)))
                        (_wrappedInstance, pszName_local).ThrowIfFailed();
                }
            }

            void Shell32.IFileDialog.GetFileName(out string? pszName)
            {
                IntPtr pszName_local;
                ((delegate* unmanaged<IntPtr, IntPtr*, HRESULT>)(*(*(void***)_wrappedInstance + 16)))
                    (_wrappedInstance, &pszName_local).ThrowIfFailed();
                pszName = Marshal.PtrToStringUni(pszName_local);
                Marshal.FreeCoTaskMem(pszName_local);
            }

            void Shell32.IFileDialog.SetTitle(string pszTitle)
            {
                ArgumentNullException.ThrowIfNull(pszTitle);
                fixed (char* pszTitle_local = pszTitle)
                {
                    ((delegate* unmanaged<IntPtr, char*, HRESULT>)(*(*(void***)_wrappedInstance + 17)))
                        (_wrappedInstance, pszTitle_local).ThrowIfFailed();
                }
            }

            HRESULT Shell32.IFileDialog.SetOkButtonLabel(string pszText)
            {
                ArgumentNullException.ThrowIfNull(pszText);
                fixed (char* pszText_local = pszText)
                {
                    return ((delegate* unmanaged<IntPtr, char*, HRESULT>)(*(*(void***)_wrappedInstance + 18)))
                        (_wrappedInstance, pszText_local);
                }
            }

            HRESULT Shell32.IFileDialog.SetFileNameLabel(string pszLabel)
            {
                ArgumentNullException.ThrowIfNull(pszLabel);
                fixed (char* pszLabel_local = pszLabel)
                {
                    return ((delegate* unmanaged<IntPtr, char*, HRESULT>)(*(*(void***)_wrappedInstance + 19)))
                        (_wrappedInstance, pszLabel_local);
                }
            }

            public void GetResult(out Shell32.IShellItem? ppsi)
            {
                IntPtr ppsi_local;
                ((delegate* unmanaged<IntPtr, IntPtr*, HRESULT>)(*(*(void***)_wrappedInstance + 20)))
                    (_wrappedInstance, &ppsi_local).ThrowIfFailed();
                ppsi = ppsi_local == IntPtr.Zero ? null : (Shell32.IShellItem)WinFormsComWrappers.Instance.GetOrCreateObjectForComInstance(ppsi_local, CreateObjectFlags.Unwrap);
            }

            HRESULT Shell32.IFileDialog.AddPlace(Shell32.IShellItem psi, FDAP fdap)
            {
                IntPtr psi_local = WinFormsComWrappers.Instance.GetComPointer(psi, IID.IShellItem);
                return ((delegate* unmanaged<IntPtr, IntPtr, FDAP, HRESULT>)(*(*(void***)_wrappedInstance + 21)))
                    (_wrappedInstance, psi_local, fdap);
            }

            void Shell32.IFileDialog.SetDefaultExtension(string pszDefaultExtension)
            {
                ArgumentNullException.ThrowIfNull(pszDefaultExtension);
                fixed (char* pszDefaultExtension_local = pszDefaultExtension)
                {
                    ((delegate* unmanaged<IntPtr, char*, HRESULT>)(*(*(void***)_wrappedInstance + 22)))
                        (_wrappedInstance, pszDefaultExtension_local).ThrowIfFailed();
                }
            }

            void Shell32.IFileDialog.Close(int hr)
            {
                ((delegate* unmanaged<IntPtr, int, HRESULT>)(*(*(void***)_wrappedInstance + 23)))
                    (_wrappedInstance, hr).ThrowIfFailed();
            }

            void Shell32.IFileDialog.SetClientGuid(ref Guid guid)
            {
                fixed (Guid* guid_local = &guid)
                {
                    ((delegate* unmanaged<IntPtr, Guid*, HRESULT>)(*(*(void***)_wrappedInstance + 24)))
                        (_wrappedInstance, guid_local).ThrowIfFailed();
                }
            }

            HRESULT Shell32.IFileDialog.ClearClientData()
            {
                return ((delegate* unmanaged<IntPtr, HRESULT>)(*(*(void***)_wrappedInstance + 25)))
                    (_wrappedInstance);
            }

            HRESULT Shell32.IFileDialog.SetFilter(IntPtr pFilter)
            {
                return ((delegate* unmanaged<IntPtr, IntPtr, HRESULT>)(*(*(void***)_wrappedInstance + 26)))
                    (_wrappedInstance, pFilter);
            }
        }
    }
}
