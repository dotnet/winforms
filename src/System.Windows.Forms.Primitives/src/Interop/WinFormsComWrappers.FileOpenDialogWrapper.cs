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
                if (wrappedInstance == IntPtr.Zero)
                {
                    throw new ArgumentNullException(nameof(wrappedInstance));
                }

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
                System.Span<COMDLG_FILTERSPEC_native> local_1_arr = stackalloc COMDLG_FILTERSPEC_native[rgFilterSpec.Length == 0 ? 1 : rgFilterSpec.Length];
                for (int local_1_cnt = 0; local_1_cnt < rgFilterSpec.Length; local_1_cnt++)
                {
                    var arrayItem = rgFilterSpec[local_1_cnt];
                    COMDLG_FILTERSPEC_native local_1_0 = default;
                    var local_1_0_pszName = arrayItem.pszName;
                    var local_1_0_0 = Marshal.StringToCoTaskMemUni(local_1_0_pszName);
                    local_1_0.pszName = local_1_0_0;
                    var local_1_0_pszSpec = arrayItem.pszSpec;
                    var local_1_0_1 = Marshal.StringToCoTaskMemUni(local_1_0_pszSpec);
                    local_1_0.pszSpec = local_1_0_1;
                    local_1_arr[local_1_cnt] = local_1_0;
                }

                fixed (COMDLG_FILTERSPEC_native* local_1 = local_1_arr)
                retVal = ((delegate* unmanaged<IntPtr, uint, COMDLG_FILTERSPEC_native*, int>)(*(*(void***)_wrappedInstance + 4)))
                    (_wrappedInstance, cFileTypes, local_1);
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
                fixed (uint* local_0 = &piFileType)
                result = ((delegate* unmanaged<IntPtr, uint*, int>)(*(*(void***)_wrappedInstance + 6)))
                    (_wrappedInstance, local_0);
                if (result != 0)
                {
                    Marshal.ThrowExceptionForHR(result);
                }
            }

            void Shell32.IFileOpenDialog.Advise(Shell32.IFileDialogEvents pfde, out uint pdwCookie)
            {
                int result;
                IntPtr local_0;
                if (pfde == null)
                {
                    local_0 = IntPtr.Zero;
                }
                else
                {
                    var local_0_unk = WinFormsComWrappers.Instance.GetOrCreateComInterfaceForObject(pfde, CreateComInterfaceFlags.None);
                    var local_pfde_IID = IID.IFileDialogEvents;
                    result = Marshal.QueryInterface(local_0_unk, ref local_pfde_IID, out local_0);
                    if (result != 0)
                    {
                        Marshal.ThrowExceptionForHR(result);
                    }
                }

                fixed (uint* local_1 = &pdwCookie)
                result = ((delegate* unmanaged<IntPtr, IntPtr, uint*, int>)(*(*(void***)_wrappedInstance + 7)))
                    (_wrappedInstance, local_0, local_1);
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
                int result = ((delegate* unmanaged<IntPtr, int, int>)(*(*(void***)_wrappedInstance + 9)))
                    (_wrappedInstance, (int)fos);
                if (result != 0)
                {
                    Marshal.ThrowExceptionForHR(result);
                }
            }

            void Shell32.IFileOpenDialog.GetOptions(out Shell32.FOS pfos)
            {
                int local_0;
                int result = ((delegate* unmanaged<IntPtr, int*, int>)(*(*(void***)_wrappedInstance + 10)))
                    (_wrappedInstance, &local_0);
                if (result != 0)
                {
                    Marshal.ThrowExceptionForHR(result);
                }

                pfos = (Shell32.FOS)local_0;
            }

            void Shell32.IFileOpenDialog.SetDefaultFolder(Shell32.IShellItem psi)
            {
                int result;
                IntPtr local_0;
                if (psi == null)
                {
                    local_0 = IntPtr.Zero;
                }
                else
                {
                    var local_0_unk = WinFormsComWrappers.Instance.GetOrCreateComInterfaceForObject(psi, CreateComInterfaceFlags.None);
                    var local_psi_IID = IID.IShellItem;
                    result = Marshal.QueryInterface(local_0_unk, ref local_psi_IID, out local_0);
                    if (result != 0)
                    {
                        Marshal.ThrowExceptionForHR(result);
                    }
                }

                result = ((delegate* unmanaged<IntPtr, IntPtr, int>)(*(*(void***)_wrappedInstance + 11)))
                    (_wrappedInstance, local_0);
                if (result != 0)
                {
                    Marshal.ThrowExceptionForHR(result);
                }
            }

            void Shell32.IFileOpenDialog.SetFolder(Shell32.IShellItem psi)
            {
                int result;
                IntPtr local_0;
                if (psi == null)
                {
                    local_0 = IntPtr.Zero;
                }
                else
                {
                    var local_0_unk = WinFormsComWrappers.Instance.GetOrCreateComInterfaceForObject(psi, CreateComInterfaceFlags.None);
                    var local_psi_IID = IID.IShellItem;
                    result = Marshal.QueryInterface(local_0_unk, ref local_psi_IID, out local_0);
                    if (result != 0)
                    {
                        Marshal.ThrowExceptionForHR(result);
                    }
                }

                result = ((delegate* unmanaged<IntPtr, IntPtr, int>)(*(*(void***)_wrappedInstance + 12)))
                    (_wrappedInstance, local_0);
                if (result != 0)
                {
                    Marshal.ThrowExceptionForHR(result);
                }
            }

            void Shell32.IFileOpenDialog.GetFolder(out Shell32.IShellItem ppsi)
            {
                int result;
                IntPtr local_0;
                result = ((delegate* unmanaged<IntPtr, IntPtr*, int>)(*(*(void***)_wrappedInstance + 13)))
                    (_wrappedInstance, &local_0);
                if (result != 0)
                {
                    Marshal.ThrowExceptionForHR(result);
                }

                ppsi = local_0 == IntPtr.Zero ? null! : (Shell32.IShellItem)WinFormsComWrappers.Instance.GetOrCreateObjectForComInstance(local_0, CreateObjectFlags.Unwrap);
            }

            HRESULT Shell32.IFileOpenDialog.GetCurrentSelection(out Shell32.IShellItem ppsi)
            {
                IntPtr local_0;
                int retVal;
                retVal = ((delegate* unmanaged<IntPtr, IntPtr*, int>)(*(*(void***)_wrappedInstance + 14)))
                    (_wrappedInstance, &local_0);
                ppsi = local_0 == IntPtr.Zero ? null! : (Shell32.IShellItem)WinFormsComWrappers.Instance.GetOrCreateObjectForComInstance(local_0, CreateObjectFlags.Unwrap);
                return (HRESULT)retVal;
            }

            void Shell32.IFileOpenDialog.SetFileName(string pszName)
            {
                int result;
                var local_0 = Marshal.StringToCoTaskMemUni(pszName);
                result = ((delegate* unmanaged<IntPtr, IntPtr, int>)(*(*(void***)_wrappedInstance + 15)))
                    (_wrappedInstance, local_0);
                if (result != 0)
                {
                    Marshal.ThrowExceptionForHR(result);
                }

                Marshal.FreeCoTaskMem(local_0);
            }

            void Shell32.IFileOpenDialog.GetFileName(out string pszName)
            {
                int result;
                IntPtr local_0;
                result = ((delegate* unmanaged<IntPtr, IntPtr*, int>)(*(*(void***)_wrappedInstance + 16)))
                    (_wrappedInstance, &local_0);
                if (result != 0)
                {
                    Marshal.ThrowExceptionForHR(result);
                }

                pszName = Marshal.PtrToStringUni(local_0)!;
            }

            void Shell32.IFileOpenDialog.SetTitle(string pszTitle)
            {
                int result;
                var local_0 = Marshal.StringToCoTaskMemUni(pszTitle);
                result = ((delegate* unmanaged<IntPtr, IntPtr, int>)(*(*(void***)_wrappedInstance + 17)))
                    (_wrappedInstance, local_0);
                if (result != 0)
                {
                    Marshal.ThrowExceptionForHR(result);
                }

                Marshal.FreeCoTaskMem(local_0);
            }

            HRESULT Shell32.IFileOpenDialog.SetOkButtonLabel(string pszText)
            {
                var local_0 = Marshal.StringToCoTaskMemUni(pszText);
                int retVal;
                retVal = ((delegate* unmanaged<IntPtr, IntPtr, int>)(*(*(void***)_wrappedInstance + 18)))
                    (_wrappedInstance, local_0);
                Marshal.FreeCoTaskMem(local_0);
                return (HRESULT)retVal;
            }

            HRESULT Shell32.IFileOpenDialog.SetFileNameLabel(string pszLabel)
            {
                var local_0 = Marshal.StringToCoTaskMemUni(pszLabel);
                int retVal;
                retVal = ((delegate* unmanaged<IntPtr, IntPtr, int>)(*(*(void***)_wrappedInstance + 19)))
                    (_wrappedInstance, local_0);
                Marshal.FreeCoTaskMem(local_0);
                return (HRESULT)retVal;
            }

            void Shell32.IFileOpenDialog.GetResult(out Shell32.IShellItem ppsi)
            {
                int result;
                IntPtr local_0;
                result = ((delegate* unmanaged<IntPtr, IntPtr*, int>)(*(*(void***)_wrappedInstance + 20)))
                    (_wrappedInstance, &local_0);
                if (result != 0)
                {
                    Marshal.ThrowExceptionForHR(result);
                }

                ppsi = local_0 == IntPtr.Zero ? null! : (Shell32.IShellItem)WinFormsComWrappers.Instance.GetOrCreateObjectForComInstance(local_0, CreateObjectFlags.Unwrap);
            }

            HRESULT Shell32.IFileOpenDialog.AddPlace(Shell32.IShellItem psi, Shell32.FDAP fdap)
            {
                int result;
                IntPtr local_0;
                if (psi == null)
                {
                    local_0 = IntPtr.Zero;
                }
                else
                {
                    var local_0_unk = WinFormsComWrappers.Instance.GetOrCreateComInterfaceForObject(psi, CreateComInterfaceFlags.None);
                    var local_psi_IID = IID.IShellItem;
                    result = Marshal.QueryInterface(local_0_unk, ref local_psi_IID, out local_0);
                    if (result != 0)
                    {
                        Marshal.ThrowExceptionForHR(result);
                    }
                }

                int retVal;
                retVal = ((delegate* unmanaged<IntPtr, IntPtr, int, int>)(*(*(void***)_wrappedInstance + 21)))
                    (_wrappedInstance, local_0, (int)fdap);
                return (HRESULT)retVal;
            }

            void Shell32.IFileOpenDialog.SetDefaultExtension(string pszDefaultExtension)
            {
                var local_0 = Marshal.StringToCoTaskMemUni(pszDefaultExtension);
                int result = ((delegate* unmanaged<IntPtr, IntPtr, int>)(*(*(void***)_wrappedInstance + 22)))
                    (_wrappedInstance, local_0);
                if (result != 0)
                {
                    Marshal.ThrowExceptionForHR(result);
                }

                Marshal.FreeCoTaskMem(local_0);
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
                fixed (Guid* local_0 = &guid)
                result = ((delegate* unmanaged<IntPtr, Guid*, int>)(*(*(void***)_wrappedInstance + 24)))
                    (_wrappedInstance, local_0);
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
                IntPtr local_0;
                result = ((delegate* unmanaged<IntPtr, IntPtr*, int>)(*(*(void***)_wrappedInstance + 27)))
                    (_wrappedInstance, &local_0);
                if (result != 0)
                {
                    Marshal.ThrowExceptionForHR(result);
                }

                ppenum = local_0 == IntPtr.Zero ? null! : (Shell32.IShellItemArray)Marshal.GetObjectForIUnknown(local_0);
            }

            HRESULT Shell32.IFileOpenDialog.GetSelectedItems(out Shell32.IShellItemArray ppsai)
            {
                IntPtr local_0;
                int retVal;
                retVal = ((delegate* unmanaged<IntPtr, IntPtr*, int>)(*(*(void***)_wrappedInstance + 28)))
                    (_wrappedInstance, &local_0);
                ppsai = local_0 == IntPtr.Zero ? null! : (Shell32.IShellItemArray)Marshal.GetObjectForIUnknown(local_0);
                return (HRESULT)retVal;
            }
        }
    }
}
