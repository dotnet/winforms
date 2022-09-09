// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

internal partial class Interop
{
    internal unsafe partial class WinFormsComWrappers
    {
        internal sealed class FileSaveDialogWrapper : FileDialogWrapper, Shell32.IFileSaveDialog
        {
            public FileSaveDialogWrapper(IntPtr wrappedInstance)
                : base(wrappedInstance)
            {
            }

            HRESULT Shell32.IFileSaveDialog.Show(IntPtr parent)
            {
                return ((Shell32.IFileDialog)this).Show(parent);
            }

            HRESULT Shell32.IFileSaveDialog.SetFileTypes(uint cFileTypes, Shell32.COMDLG_FILTERSPEC[] rgFilterSpec)
            {
                return ((Shell32.IFileDialog)this).SetFileTypes(cFileTypes, rgFilterSpec);
            }

            HRESULT Shell32.IFileSaveDialog.SetFileTypeIndex(uint iFileType)
            {
                return ((Shell32.IFileDialog)this).SetFileTypeIndex(iFileType);
            }

            void Shell32.IFileSaveDialog.GetFileTypeIndex(out uint piFileType)
            {
                ((Shell32.IFileDialog)this).GetFileTypeIndex(out piFileType);
            }

            void Shell32.IFileSaveDialog.Advise(Shell32.IFileDialogEvents pfde, out uint pdwCookie)
            {
                ((Shell32.IFileDialog)this).Advise(pfde, out pdwCookie);
            }

            void Shell32.IFileSaveDialog.Unadvise(uint dwCookie)
            {
                ((Shell32.IFileDialog)this).Unadvise(dwCookie);
            }

            void Shell32.IFileSaveDialog.SetOptions(Shell32.FOS fos)
            {
                ((Shell32.IFileDialog)this).SetOptions(fos);
            }

            void Shell32.IFileSaveDialog.GetOptions(out Shell32.FOS pfos)
            {
                ((Shell32.IFileDialog)this).GetOptions(out pfos);
            }

            void Shell32.IFileSaveDialog.SetDefaultFolder(Shell32.IShellItem psi)
            {
                ((Shell32.IFileDialog)this).SetDefaultFolder(psi);
            }

            void Shell32.IFileSaveDialog.SetFolder(Shell32.IShellItem psi)
            {
                ((Shell32.IFileDialog)this).SetFolder(psi);
            }

            void Shell32.IFileSaveDialog.GetFolder(out Shell32.IShellItem? ppsi)
            {
                ((Shell32.IFileDialog)this).GetFolder(out ppsi);
            }

            HRESULT Shell32.IFileSaveDialog.GetCurrentSelection(out Shell32.IShellItem? ppsi)
            {
                return ((Shell32.IFileDialog)this).GetCurrentSelection(out ppsi);
            }

            void Shell32.IFileSaveDialog.SetFileName(string pszName)
            {
                ((Shell32.IFileDialog)this).SetFileName(pszName);
            }

            void Shell32.IFileSaveDialog.GetFileName(out string? pszName)
            {
                ((Shell32.IFileDialog)this).GetFileName(out pszName);
            }

            void Shell32.IFileSaveDialog.SetTitle(string pszTitle)
            {
                ((Shell32.IFileDialog)this).SetTitle(pszTitle);
            }

            HRESULT Shell32.IFileSaveDialog.SetOkButtonLabel(string pszText)
            {
                return ((Shell32.IFileDialog)this).SetOkButtonLabel(pszText);
            }

            HRESULT Shell32.IFileSaveDialog.SetFileNameLabel(string pszLabel)
            {
                return ((Shell32.IFileDialog)this).SetFileNameLabel(pszLabel);
            }

            void Shell32.IFileSaveDialog.GetResult(out Shell32.IShellItem? ppsi)
            {
                ((Shell32.IFileDialog)this).GetResult(out ppsi);
            }

            HRESULT Shell32.IFileSaveDialog.AddPlace(Shell32.IShellItem psi, FDAP fdap)
            {
                return ((Shell32.IFileDialog)this).AddPlace(psi, fdap);
            }

            void Shell32.IFileSaveDialog.SetDefaultExtension(string pszDefaultExtension)
            {
                ((Shell32.IFileDialog)this).SetDefaultExtension(pszDefaultExtension);
            }

            void Shell32.IFileSaveDialog.Close(int hr)
            {
                ((Shell32.IFileDialog)this).Close(hr);
            }

            void Shell32.IFileSaveDialog.SetClientGuid(ref Guid guid)
            {
                ((Shell32.IFileDialog)this).SetClientGuid(ref guid);
            }

            HRESULT Shell32.IFileSaveDialog.ClearClientData()
            {
                return ((Shell32.IFileDialog)this).ClearClientData();
            }

            HRESULT Shell32.IFileSaveDialog.SetFilter(IntPtr pFilter)
            {
                return ((Shell32.IFileDialog)this).SetFilter(pFilter);
            }

            HRESULT Shell32.IFileSaveDialog.SetSaveAsItem(Shell32.IShellItem psi)
            {
                IntPtr psi_local = WinFormsComWrappers.Instance.GetComPointer(psi, IID.IShellItem);
                return ((delegate* unmanaged<IntPtr, IntPtr, HRESULT>)(*(*(void***)_wrappedInstance + 27)))
                    (_wrappedInstance, psi_local);
            }

            HRESULT Shell32.IFileSaveDialog.SetProperties(IntPtr pStore)
            {
                return ((delegate* unmanaged<IntPtr, IntPtr, HRESULT>)(*(*(void***)_wrappedInstance + 28)))
                    (_wrappedInstance, pStore);
            }

            HRESULT Shell32.IFileSaveDialog.SetCollectedProperties(IntPtr pList, BOOL fAppendDefault)
            {
                return ((delegate* unmanaged<IntPtr, IntPtr, BOOL, HRESULT>)(*(*(void***)_wrappedInstance + 29)))
                    (_wrappedInstance, pList, fAppendDefault);
            }

            HRESULT Shell32.IFileSaveDialog.GetProperties(out IntPtr ppStore)
            {
                fixed (IntPtr* ppStore_local = &ppStore)
                {
                    return ((delegate* unmanaged<IntPtr, IntPtr*, HRESULT>)(*(*(void***)_wrappedInstance + 30)))
                        (_wrappedInstance, ppStore_local);
                }
            }

            HRESULT Shell32.IFileSaveDialog.ApplyProperties(Shell32.IShellItem psi, IntPtr pStore, ref IntPtr hwnd, IntPtr pSink)
            {
                IntPtr psi_local = WinFormsComWrappers.Instance.GetComPointer(psi, IID.IShellItem);
                fixed (IntPtr* hwnd_local = &hwnd)
                {
                    return ((delegate* unmanaged<IntPtr, IntPtr, IntPtr, IntPtr*, IntPtr, HRESULT>)(*(*(void***)_wrappedInstance + 31)))
                        (_wrappedInstance, psi_local, pStore, hwnd_local, pSink);
                }
            }
        }
    }
}
