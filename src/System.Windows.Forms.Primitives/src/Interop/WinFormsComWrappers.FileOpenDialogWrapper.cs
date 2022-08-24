// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

internal partial class Interop
{
    internal unsafe partial class WinFormsComWrappers
    {
        internal sealed class FileOpenDialogWrapper : FileDialogWrapper, Shell32.IFileOpenDialog
        {
            public FileOpenDialogWrapper(IntPtr wrappedInstance)
                : base(wrappedInstance)
            {
            }

            HRESULT Shell32.IFileOpenDialog.Show(IntPtr parent)
            {
                return this.Show(parent);
            }

            HRESULT Shell32.IFileOpenDialog.SetFileTypes(uint cFileTypes, Shell32.COMDLG_FILTERSPEC[] rgFilterSpec)
            {
                return ((Shell32.IFileDialog)this).SetFileTypes(cFileTypes, rgFilterSpec);
            }

            HRESULT Shell32.IFileOpenDialog.SetFileTypeIndex(uint iFileType)
            {
                return ((Shell32.IFileDialog)this).SetFileTypeIndex(iFileType);
            }

            void Shell32.IFileOpenDialog.GetFileTypeIndex(out uint piFileType)
            {
                ((Shell32.IFileDialog)this).GetFileTypeIndex(out piFileType);
            }

            void Shell32.IFileOpenDialog.Advise(Shell32.IFileDialogEvents pfde, out uint pdwCookie)
            {
                ((Shell32.IFileDialog)this).Advise(pfde, out pdwCookie);
            }

            void Shell32.IFileOpenDialog.Unadvise(uint dwCookie)
            {
                ((Shell32.IFileDialog)this).Unadvise(dwCookie);
            }

            void Shell32.IFileOpenDialog.SetOptions(Shell32.FOS fos)
            {
                ((Shell32.IFileDialog)this).SetOptions(fos);
            }

            void Shell32.IFileOpenDialog.GetOptions(out Shell32.FOS pfos)
            {
                ((Shell32.IFileDialog)this).GetOptions(out pfos);
            }

            void Shell32.IFileOpenDialog.SetDefaultFolder(Shell32.IShellItem psi)
            {
                ((Shell32.IFileDialog)this).SetDefaultFolder(psi);
            }

            void Shell32.IFileOpenDialog.SetFolder(Shell32.IShellItem psi)
            {
                ((Shell32.IFileDialog)this).SetFolder(psi);
            }

            void Shell32.IFileOpenDialog.GetFolder(out Shell32.IShellItem? ppsi)
            {
                ((Shell32.IFileDialog)this).GetFolder(out ppsi);
            }

            HRESULT Shell32.IFileOpenDialog.GetCurrentSelection(out Shell32.IShellItem? ppsi)
            {
                return ((Shell32.IFileDialog)this).GetCurrentSelection(out ppsi);
            }

            void Shell32.IFileOpenDialog.SetFileName(string pszName)
            {
                ((Shell32.IFileDialog)this).SetFileName(pszName);
            }

            void Shell32.IFileOpenDialog.GetFileName(out string? pszName)
            {
                ((Shell32.IFileDialog)this).GetFileName(out pszName);
            }

            void Shell32.IFileOpenDialog.SetTitle(string pszTitle)
            {
                ((Shell32.IFileDialog)this).SetTitle(pszTitle);
            }

            HRESULT Shell32.IFileOpenDialog.SetOkButtonLabel(string pszText)
            {
                return ((Shell32.IFileDialog)this).SetOkButtonLabel(pszText);
            }

            HRESULT Shell32.IFileOpenDialog.SetFileNameLabel(string pszLabel)
            {
                return ((Shell32.IFileDialog)this).SetFileNameLabel(pszLabel);
            }

            void Shell32.IFileOpenDialog.GetResult(out Shell32.IShellItem? ppsi)
            {
                ((Shell32.IFileDialog)this).GetResult(out ppsi);
            }

            HRESULT Shell32.IFileOpenDialog.AddPlace(Shell32.IShellItem psi, FDAP fdap)
            {
                return ((Shell32.IFileDialog)this).AddPlace(psi, fdap);
            }

            void Shell32.IFileOpenDialog.SetDefaultExtension(string pszDefaultExtension)
            {
                ((Shell32.IFileDialog)this).SetDefaultExtension(pszDefaultExtension);
            }

            void Shell32.IFileOpenDialog.Close(int hr)
            {
                ((Shell32.IFileDialog)this).Close(hr);
            }

            void Shell32.IFileOpenDialog.SetClientGuid(ref Guid guid)
            {
                ((Shell32.IFileDialog)this).SetClientGuid(ref guid);
            }

            HRESULT Shell32.IFileOpenDialog.ClearClientData()
            {
                return ((Shell32.IFileDialog)this).ClearClientData();
            }

            HRESULT Shell32.IFileOpenDialog.SetFilter(IntPtr pFilter)
            {
                return ((Shell32.IFileDialog)this).SetFilter(pFilter);
            }

            void Shell32.IFileOpenDialog.GetResults(out Shell32.IShellItemArray? ppenum)
            {
                GetResults(out Interop.WinFormsComWrappers.ShellItemArrayWrapper? wrapper);
                ppenum = wrapper;
            }

            public void GetResults(out Interop.WinFormsComWrappers.ShellItemArrayWrapper? ppenum)
            {
                IntPtr ppenum_local;
                ((delegate* unmanaged<IntPtr, IntPtr*, HRESULT>)(*(*(void***)_wrappedInstance + 27)))
                    (_wrappedInstance, &ppenum_local).ThrowOnFailure();
                ppenum = ppenum_local == IntPtr.Zero ? null : (Interop.WinFormsComWrappers.ShellItemArrayWrapper)WinFormsComWrappers.Instance.GetOrCreateObjectForComInstance(ppenum_local, CreateObjectFlags.UniqueInstance);
            }

            HRESULT Shell32.IFileOpenDialog.GetSelectedItems(out Shell32.IShellItemArray? ppsai)
            {
                IntPtr ppsai_local;
                HRESULT result = ((delegate* unmanaged<IntPtr, IntPtr*, HRESULT>)(*(*(void***)_wrappedInstance + 28)))
                    (_wrappedInstance, &ppsai_local);
                ppsai = ppsai_local == IntPtr.Zero ? null : (Shell32.IShellItemArray)WinFormsComWrappers.Instance.GetOrCreateObjectForComInstance(ppsai_local, CreateObjectFlags.Unwrap);
                return result;
            }

            public void AddText(uint dwIDCtl, string pszText)
            {
                var targetInterface = IID.IFileDialogCustomize;
                var result = Marshal.QueryInterface(_wrappedInstance, ref targetInterface, out var thisPtr);
                if (result != 0)
                {
                    throw new InvalidCastException();
                }

                IntPtr pszText_local = IntPtr.Zero;
                try
                {
                    pszText_local = Marshal.StringToCoTaskMemUni(pszText);
                    ((delegate* unmanaged<IntPtr, uint, IntPtr, HRESULT>)(*(*(void***)thisPtr + 11 /* IFileDialogCustomize.AddText */)))
                        (thisPtr, dwIDCtl, pszText_local).ThrowOnFailure();
                }
                finally
                {
                    Marshal.FreeCoTaskMem(pszText_local);
                    Marshal.Release(thisPtr);
                }
            }
        }
    }
}
