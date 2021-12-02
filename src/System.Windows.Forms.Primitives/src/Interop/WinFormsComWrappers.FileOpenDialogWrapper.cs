// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

internal partial class Interop
{
    internal unsafe partial class WinFormsComWrappers
    {
        private class FileOpenDialogWrapper : Shell32.IFileOpenDialog, IDisposable
        {
            private IntPtr _wrappedInstance;

            public FileOpenDialogWrapper(IntPtr wrappedInstance)
            {
                ArgumentNullException.ThrowIfNull(wrappedInstance);

                _wrappedInstance = wrappedInstance;
            }

            public void Dispose()
            {
                Marshal.Release(_wrappedInstance);
                _wrappedInstance = IntPtr.Zero;
            }

            HRESULT Shell32.IFileDialog.Show(IntPtr parent)
            {
                return ((Shell32.IFileOpenDialog)this).Show(parent);
            }

            HRESULT Shell32.IFileDialog.SetFileTypes(uint cFileTypes, Shell32.COMDLG_FILTERSPEC[] rgFilterSpec)
            {
                return ((Shell32.IFileOpenDialog)this).SetFileTypes(cFileTypes, rgFilterSpec);
            }

            HRESULT Shell32.IFileDialog.SetFileTypeIndex(uint iFileType)
            {
                return ((Shell32.IFileOpenDialog)this).SetFileTypeIndex(iFileType);
            }

            void Shell32.IFileDialog.GetFileTypeIndex(out uint piFileType)
            {
                ((Shell32.IFileOpenDialog)this).GetFileTypeIndex(out piFileType);
            }

            void Shell32.IFileDialog.Advise(Shell32.IFileDialogEvents pfde, out uint pdwCookie)
            {
                ((Shell32.IFileOpenDialog)this).Advise(pfde, out pdwCookie);
            }

            void Shell32.IFileDialog.Unadvise(uint dwCookie)
            {
                ((Shell32.IFileOpenDialog)this).Unadvise(dwCookie);
            }

            void Shell32.IFileDialog.SetOptions(Shell32.FOS fos)
            {
                ((Shell32.IFileOpenDialog)this).SetOptions(fos);
            }

            void Shell32.IFileDialog.GetOptions(out Shell32.FOS pfos)
            {
                ((Shell32.IFileOpenDialog)this).GetOptions(out pfos);
            }

            void Shell32.IFileDialog.SetDefaultFolder(Shell32.IShellItem psi)
            {
                ((Shell32.IFileOpenDialog)this).SetDefaultFolder(psi);
            }

            void Shell32.IFileDialog.SetFolder(Shell32.IShellItem psi)
            {
                ((Shell32.IFileOpenDialog)this).SetFolder(psi);
            }

            void Shell32.IFileDialog.GetFolder(out Shell32.IShellItem ppsi)
            {
                ((Shell32.IFileOpenDialog)this).GetFolder(out ppsi);
            }

            HRESULT Shell32.IFileDialog.GetCurrentSelection(out Shell32.IShellItem ppsi)
            {
                return ((Shell32.IFileOpenDialog)this).GetCurrentSelection(out ppsi);
            }

            void Shell32.IFileDialog.SetFileName(string pszName)
            {
                ((Shell32.IFileOpenDialog)this).SetFileName(pszName);
            }

            void Shell32.IFileDialog.GetFileName(out string pszName)
            {
                ((Shell32.IFileOpenDialog)this).GetFileName(out pszName);
            }

            void Shell32.IFileDialog.SetTitle(string pszTitle)
            {
                ((Shell32.IFileOpenDialog)this).SetTitle(pszTitle);
            }

            HRESULT Shell32.IFileDialog.SetOkButtonLabel(string pszText)
            {
                return ((Shell32.IFileOpenDialog)this).SetOkButtonLabel(pszText);
            }

            HRESULT Shell32.IFileDialog.SetFileNameLabel(string pszLabel)
            {
                return ((Shell32.IFileOpenDialog)this).SetFileNameLabel(pszLabel);
            }

            void Shell32.IFileDialog.GetResult(out Shell32.IShellItem ppsi)
            {
                ((Shell32.IFileOpenDialog)this).GetResult(out ppsi);
            }

            HRESULT Shell32.IFileDialog.AddPlace(Shell32.IShellItem psi, Shell32.FDAP fdap)
            {
                return ((Shell32.IFileOpenDialog)this).AddPlace(psi, fdap);
            }

            void Shell32.IFileDialog.SetDefaultExtension(string pszDefaultExtension)
            {
                ((Shell32.IFileOpenDialog)this).SetDefaultExtension(pszDefaultExtension);
            }

            void Shell32.IFileDialog.Close(int hr)
            {
                ((Shell32.IFileOpenDialog)this).Close(hr);
            }

            void Shell32.IFileDialog.SetClientGuid(ref Guid guid)
            {
                ((Shell32.IFileOpenDialog)this).SetClientGuid(ref guid);
            }

            HRESULT Shell32.IFileDialog.ClearClientData()
            {
                return ((Shell32.IFileOpenDialog)this).ClearClientData();
            }

            HRESULT Shell32.IFileDialog.SetFilter(IntPtr pFilter)
            {
                return ((Shell32.IFileOpenDialog)this).SetFilter(pFilter);
            }

            HRESULT Shell32.IFileOpenDialog.Show(IntPtr parent)
            {
                int retVal;
                retVal = ((delegate* unmanaged<IntPtr, IntPtr, int>)(*(*(void***)_wrappedInstance + 3)))
                    (_wrappedInstance, parent);
                return (HRESULT)retVal;
            }

            HRESULT Shell32.IFileOpenDialog.SetFileTypes(uint cFileTypes, Shell32.COMDLG_FILTERSPEC[] rgFilterSpec)
            {
                int retVal;
                fixed (Shell32.COMDLG_FILTERSPEC* rgFilterSpec_local = &rgFilterSpec)
                retVal = ((delegate* unmanaged<IntPtr, uint, Shell32.COMDLG_FILTERSPEC*, int>)(*(*(void***)_wrappedInstance + 4)))
                    (_wrappedInstance, cFileTypes, rgFilterSpec_local);
                return (HRESULT)retVal;
            }

            HRESULT Shell32.IFileOpenDialog.SetFileTypeIndex(uint iFileType)
            {
                int retVal;
                retVal = ((delegate* unmanaged<IntPtr, uint, int>)(*(*(void***)_wrappedInstance + 5)))
                    (_wrappedInstance, iFileType);
                return (HRESULT)retVal;
            }

            void Shell32.IFileOpenDialog.GetFileTypeIndex(out uint piFileType)
            {
                int result;
                fixed (uint* piFileType_local = &piFileType)
                result = ((delegate* unmanaged<IntPtr, uint*, int>)(*(*(void***)_wrappedInstance + 6)))
                    (_wrappedInstance, piFileType_local);
                if (result != 0)
                {
                    Marshal.ThrowExceptionForHR(result);
                }
            }

            void Shell32.IFileOpenDialog.Advise(Shell32.IFileDialogEvents pfde, out uint pdwCookie)
            {
                int result;
                IntPtr pfde_local;
                if (pfde == null)
                {
                    pfde_local = IntPtr.Zero;
                }
                else
                {
                    var pUnk_local = WinFormsComWrappers.Instance.GetOrCreateComInterfaceForObject(pfde, CreateComInterfaceFlags.None);
                    var local_pfde_IID = IID.IFileDialogEvents;
                    result = Marshal.QueryInterface(pUnk_local, ref local_pfde_IID, out pfde_local);
                    Marshal.Release(pUnk_local);
                    if (result != 0)
                    {
                        Marshal.ThrowExceptionForHR(result);
                    }
                }

                fixed (uint* pdwCookie_local = &pdwCookie)
                result = ((delegate* unmanaged<IntPtr, IntPtr, uint*, int>)(*(*(void***)_wrappedInstance + 7)))
                    (_wrappedInstance, pfde_local, pdwCookie_local);
                if (result != 0)
                {
                    Marshal.ThrowExceptionForHR(result);
                }
            }

            void Shell32.IFileOpenDialog.Unadvise(uint dwCookie)
            {
                int result = ((delegate* unmanaged<IntPtr, uint, int>)(*(*(void***)_wrappedInstance + 8)))
                    (_wrappedInstance, dwCookie);
                if (result != 0)
                {
                    Marshal.ThrowExceptionForHR(result);
                }
            }

            void Shell32.IFileOpenDialog.SetOptions(Shell32.FOS fos)
            {
                int result = ((delegate* unmanaged<IntPtr, Shell32.FOS, int>)(*(*(void***)_wrappedInstance + 9)))
                    (_wrappedInstance, fos);
                if (result != 0)
                {
                    Marshal.ThrowExceptionForHR(result);
                }
            }

            void Shell32.IFileOpenDialog.GetOptions(out Shell32.FOS pfos)
            {
                Shell32.FOS pfos_local;
                int result = ((delegate* unmanaged<IntPtr, Shell32.FOS*, int>)(*(*(void***)_wrappedInstance + 10)))
                    (_wrappedInstance, &pfos_local);
                if (result != 0)
                {
                    Marshal.ThrowExceptionForHR(result);
                }

                pfos = pfos_local;
            }

            void Shell32.IFileOpenDialog.SetDefaultFolder(Shell32.IShellItem psi)
            {
                int result;
                IntPtr psi_local;
                if (psi == null)
                {
                    psi_local = IntPtr.Zero;
                }
                else
                {
                    var pUnk_local = WinFormsComWrappers.Instance.GetOrCreateComInterfaceForObject(psi, CreateComInterfaceFlags.None);
                    var local_psi_IID = IID.IShellItem;
                    result = Marshal.QueryInterface(pUnk_local, ref local_psi_IID, out psi_local);
                    Marshal.Release(pUnk_local);
                    if (result != 0)
                    {
                        Marshal.ThrowExceptionForHR(result);
                    }
                }

                result = ((delegate* unmanaged<IntPtr, IntPtr, int>)(*(*(void***)_wrappedInstance + 11)))
                    (_wrappedInstance, psi_local);
                if (result != 0)
                {
                    Marshal.ThrowExceptionForHR(result);
                }
            }

            void Shell32.IFileOpenDialog.SetFolder(Shell32.IShellItem psi)
            {
                int result;
                IntPtr psi_local;
                if (psi == null)
                {
                    psi_local = IntPtr.Zero;
                }
                else
                {
                    var pUnk_local = WinFormsComWrappers.Instance.GetOrCreateComInterfaceForObject(psi, CreateComInterfaceFlags.None);
                    var local_psi_IID = IID.IShellItem;
                    result = Marshal.QueryInterface(pUnk_local, ref local_psi_IID, out psi_local);
                    Marshal.Release(pUnk_local);
                    if (result != 0)
                    {
                        Marshal.ThrowExceptionForHR(result);
                    }
                }

                result = ((delegate* unmanaged<IntPtr, IntPtr, int>)(*(*(void***)_wrappedInstance + 12)))
                    (_wrappedInstance, psi_local);
                if (result != 0)
                {
                    Marshal.ThrowExceptionForHR(result);
                }
            }

            void Shell32.IFileOpenDialog.GetFolder(out Shell32.IShellItem ppsi)
            {
                int result;
                IntPtr ppsi_local;
                result = ((delegate* unmanaged<IntPtr, IntPtr*, int>)(*(*(void***)_wrappedInstance + 13)))
                    (_wrappedInstance, &ppsi_local);
                if (result != 0)
                {
                    Marshal.ThrowExceptionForHR(result);
                }

                ppsi = ppsi_local == IntPtr.Zero ? null! : (Shell32.IShellItem)WinFormsComWrappers.Instance.GetOrCreateObjectForComInstance(ppsi_local, CreateObjectFlags.Unwrap);
            }

            HRESULT Shell32.IFileOpenDialog.GetCurrentSelection(out Shell32.IShellItem ppsi)
            {
                IntPtr ppsi_local;
                int retVal;
                retVal = ((delegate* unmanaged<IntPtr, IntPtr*, int>)(*(*(void***)_wrappedInstance + 14)))
                    (_wrappedInstance, &ppsi_local);
                ppsi = ppsi_local == IntPtr.Zero ? null! : (Shell32.IShellItem)WinFormsComWrappers.Instance.GetOrCreateObjectForComInstance(ppsi_local, CreateObjectFlags.Unwrap);
                return (HRESULT)retVal;
            }

            void Shell32.IFileOpenDialog.SetFileName(string pszName)
            {
                int result;
                fixed (char* pszName_local = pszName)
                result = ((delegate* unmanaged<IntPtr, char*, int>)(*(*(void***)_wrappedInstance + 15)))
                    (_wrappedInstance, pszName_local);
                if (result != 0)
                {
                    Marshal.ThrowExceptionForHR(result);
                }
            }

            void Shell32.IFileOpenDialog.GetFileName(out string pszName)
            {
                int result;
                IntPtr pszName_local;
                result = ((delegate* unmanaged<IntPtr, IntPtr*, int>)(*(*(void***)_wrappedInstance + 16)))
                    (_wrappedInstance, &pszName_local);
                if (result != 0)
                {
                    Marshal.ThrowExceptionForHR(result);
                }

                pszName = Marshal.PtrToStringUni(pszName_local)!;
            }

            void Shell32.IFileOpenDialog.SetTitle(string pszTitle)
            {
                int result;
                fixed (char* pszTitle_local = pszTitle)
                result = ((delegate* unmanaged<IntPtr, char*, int>)(*(*(void***)_wrappedInstance + 17)))
                    (_wrappedInstance, pszTitle_local);
                if (result != 0)
                {
                    Marshal.ThrowExceptionForHR(result);
                }
            }

            HRESULT Shell32.IFileOpenDialog.SetOkButtonLabel(string pszText)
            {
                int retVal;
                fixed (char* pszText_local = pszText)
                retVal = ((delegate* unmanaged<IntPtr, char*, int>)(*(*(void***)_wrappedInstance + 18)))
                    (_wrappedInstance, pszText_local);
                return (HRESULT)retVal;
            }

            HRESULT Shell32.IFileOpenDialog.SetFileNameLabel(string pszLabel)
            {
                int retVal;
                fixed (char* pszLabel_local = pszLabel)
                retVal = ((delegate* unmanaged<IntPtr, char*, int>)(*(*(void***)_wrappedInstance + 19)))
                    (_wrappedInstance, pszLabel_local);
                return (HRESULT)retVal;
            }

            void Shell32.IFileOpenDialog.GetResult(out Shell32.IShellItem ppsi)
            {
                int result;
                IntPtr ppsi_local;
                result = ((delegate* unmanaged<IntPtr, IntPtr*, int>)(*(*(void***)_wrappedInstance + 20)))
                    (_wrappedInstance, &ppsi_local);
                if (result != 0)
                {
                    Marshal.ThrowExceptionForHR(result);
                }

                ppsi = ppsi_local == IntPtr.Zero ? null! : (Shell32.IShellItem)WinFormsComWrappers.Instance.GetOrCreateObjectForComInstance(ppsi_local, CreateObjectFlags.Unwrap);
            }

            HRESULT Shell32.IFileOpenDialog.AddPlace(Shell32.IShellItem psi, Shell32.FDAP fdap)
            {
                int result;
                IntPtr psi_local;
                if (psi == null)
                {
                    psi_local = IntPtr.Zero;
                }
                else
                {
                    var pUnk_local = WinFormsComWrappers.Instance.GetOrCreateComInterfaceForObject(psi, CreateComInterfaceFlags.None);
                    var local_psi_IID = IID.IShellItem;
                    result = Marshal.QueryInterface(pUnk_local, ref local_psi_IID, out psi_local);
                    Marshal.Release(pUnk_local);
                    if (result != 0)
                    {
                        Marshal.ThrowExceptionForHR(result);
                    }
                }

                int retVal;
                retVal = ((delegate* unmanaged<IntPtr, IntPtr, Shell32.FDAP, int>)(*(*(void***)_wrappedInstance + 21)))
                    (_wrappedInstance, psi_local, fdap);
                return (HRESULT)retVal;
            }

            void Shell32.IFileOpenDialog.SetDefaultExtension(string pszDefaultExtension)
            {
                int result;
                fixed (char* pszDefaultExtension_local = pszDefaultExtension)
                result = ((delegate* unmanaged<IntPtr, char*, int>)(*(*(void***)_wrappedInstance + 22)))
                    (_wrappedInstance, pszDefaultExtension_local);
                if (result != 0)
                {
                    Marshal.ThrowExceptionForHR(result);
                }
            }

            void Shell32.IFileOpenDialog.Close(int hr)
            {
                int result = ((delegate* unmanaged<IntPtr, int, int>)(*(*(void***)_wrappedInstance + 23)))
                    (_wrappedInstance, hr);
                if (result != 0)
                {
                    Marshal.ThrowExceptionForHR(result);
                }
            }

            void Shell32.IFileOpenDialog.SetClientGuid(ref Guid guid)
            {
                int result;
                fixed (Guid* guid_local = &guid)
                result = ((delegate* unmanaged<IntPtr, Guid*, int>)(*(*(void***)_wrappedInstance + 24)))
                    (_wrappedInstance, guid_local);
                if (result != 0)
                {
                    Marshal.ThrowExceptionForHR(result);
                }
            }

            HRESULT Shell32.IFileOpenDialog.ClearClientData()
            {
                int retVal;
                retVal = ((delegate* unmanaged<IntPtr, int>)(*(*(void***)_wrappedInstance + 25)))
                    (_wrappedInstance);
                return (HRESULT)retVal;
            }

            HRESULT Shell32.IFileOpenDialog.SetFilter(IntPtr pFilter)
            {
                int retVal;
                retVal = ((delegate* unmanaged<IntPtr, IntPtr, int>)(*(*(void***)_wrappedInstance + 26)))
                    (_wrappedInstance, pFilter);
                return (HRESULT)retVal;
            }

            void Shell32.IFileOpenDialog.GetResults(out Shell32.IShellItemArray ppenum)
            {
                int result;
                IntPtr ppenum_local;
                result = ((delegate* unmanaged<IntPtr, IntPtr*, int>)(*(*(void***)_wrappedInstance + 27)))
                    (_wrappedInstance, &ppenum_local);
                if (result != 0)
                {
                    Marshal.ThrowExceptionForHR(result);
                }

                ppenum = ppenum_local == IntPtr.Zero ? null! : (Shell32.IShellItemArray)Marshal.GetObjectForIUnknown(ppenum_local);
            }

            HRESULT Shell32.IFileOpenDialog.GetSelectedItems(out Shell32.IShellItemArray ppsai)
            {
                IntPtr ppsai_local;
                int retVal;
                retVal = ((delegate* unmanaged<IntPtr, IntPtr*, int>)(*(*(void***)_wrappedInstance + 28)))
                    (_wrappedInstance, &ppsai_local);
                ppsai = ppsai_local == IntPtr.Zero ? null! : (Shell32.IShellItemArray)Marshal.GetObjectForIUnknown(ppsai_local);
                return (HRESULT)retVal;
            }
        }
    }
}
