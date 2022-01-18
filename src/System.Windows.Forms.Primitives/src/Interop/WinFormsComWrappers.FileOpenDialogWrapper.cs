// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using System.Windows.Forms;

internal partial class Interop
{
    internal unsafe partial class WinFormsComWrappers
    {
        private class FileOpenDialogWrapper : Shell32.IFileOpenDialog, IDisposable
        {
            private IntPtr _wrappedInstance;

            public FileOpenDialogWrapper(IntPtr wrappedInstance)
            {
                wrappedInstance.OrThrowIfZero();

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

            void Shell32.IFileDialog.GetFolder(out Shell32.IShellItem? ppsi)
            {
                ((Shell32.IFileOpenDialog)this).GetFolder(out ppsi);
            }

            HRESULT Shell32.IFileDialog.GetCurrentSelection(out Shell32.IShellItem? ppsi)
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

            void Shell32.IFileDialog.GetResult(out Shell32.IShellItem? ppsi)
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
                HRESULT retVal;
                retVal = ((delegate* unmanaged<IntPtr, IntPtr, HRESULT>)(*(*(void***)_wrappedInstance + 3)))
                    (_wrappedInstance, parent);
                return retVal;
            }

            HRESULT Shell32.IFileOpenDialog.SetFileTypes(uint cFileTypes, Shell32.COMDLG_FILTERSPEC[] rgFilterSpec)
            {
                HRESULT retVal;
                fixed (Shell32.COMDLG_FILTERSPEC* rgFilterSpec_local = rgFilterSpec)
                {
                    retVal = ((delegate* unmanaged<IntPtr, uint, Shell32.COMDLG_FILTERSPEC*, HRESULT>)(*(*(void***)_wrappedInstance + 4)))
                        (_wrappedInstance, cFileTypes, rgFilterSpec_local);
                }

                return retVal;
            }

            HRESULT Shell32.IFileOpenDialog.SetFileTypeIndex(uint iFileType)
            {
                HRESULT retVal;
                retVal = ((delegate* unmanaged<IntPtr, uint, HRESULT>)(*(*(void***)_wrappedInstance + 5)))
                    (_wrappedInstance, iFileType);
                return retVal;
            }

            void Shell32.IFileOpenDialog.GetFileTypeIndex(out uint piFileType)
            {
                HRESULT result;
                fixed (uint* piFileType_local = &piFileType)
                {
                    result = ((delegate* unmanaged<IntPtr, uint*, HRESULT>)(*(*(void***)_wrappedInstance + 6)))
                        (_wrappedInstance, piFileType_local);
                }

                if (result.Failed())
                {
                    Marshal.ThrowExceptionForHR((int)result);
                }
            }

            void Shell32.IFileOpenDialog.Advise(Shell32.IFileDialogEvents pfde, out uint pdwCookie)
            {
                HRESULT result;
                IntPtr pfde_local = WinFormsComWrappers.Instance.GetComPointer(pfde, IID.IFileDialogEvents);
                fixed (uint* pdwCookie_local = &pdwCookie)
                {
                    result = ((delegate* unmanaged<IntPtr, IntPtr, uint*, HRESULT>)(*(*(void***)_wrappedInstance + 7)))
                        (_wrappedInstance, pfde_local, pdwCookie_local);
                }

                if (result.Failed())
                {
                    Marshal.ThrowExceptionForHR((int)result);
                }
            }

            void Shell32.IFileOpenDialog.Unadvise(uint dwCookie)
            {
                HRESULT result = ((delegate* unmanaged<IntPtr, uint, HRESULT>)(*(*(void***)_wrappedInstance + 8)))
                    (_wrappedInstance, dwCookie);
                if (result.Failed())
                {
                    Marshal.ThrowExceptionForHR((int)result);
                }
            }

            void Shell32.IFileOpenDialog.SetOptions(Shell32.FOS fos)
            {
                HRESULT result = ((delegate* unmanaged<IntPtr, Shell32.FOS, HRESULT>)(*(*(void***)_wrappedInstance + 9)))
                    (_wrappedInstance, fos);
                if (result.Failed())
                {
                    Marshal.ThrowExceptionForHR((int)result);
                }
            }

            void Shell32.IFileOpenDialog.GetOptions(out Shell32.FOS pfos)
            {
                Shell32.FOS pfos_local;
                HRESULT result = ((delegate* unmanaged<IntPtr, Shell32.FOS*, HRESULT>)(*(*(void***)_wrappedInstance + 10)))
                    (_wrappedInstance, &pfos_local);
                if (result.Failed())
                {
                    Marshal.ThrowExceptionForHR((int)result);
                }

                pfos = pfos_local;
            }

            void Shell32.IFileOpenDialog.SetDefaultFolder(Shell32.IShellItem psi)
            {
                HRESULT result;
                IntPtr psi_local = WinFormsComWrappers.Instance.GetComPointer(psi, IID.IShellItem);
                result = ((delegate* unmanaged<IntPtr, IntPtr, HRESULT>)(*(*(void***)_wrappedInstance + 11)))
                    (_wrappedInstance, psi_local);
                if (result.Failed())
                {
                    Marshal.ThrowExceptionForHR((int)result);
                }
            }

            void Shell32.IFileOpenDialog.SetFolder(Shell32.IShellItem psi)
            {
                HRESULT result;
                IntPtr psi_local = WinFormsComWrappers.Instance.GetComPointer(psi, IID.IShellItem);
                result = ((delegate* unmanaged<IntPtr, IntPtr, HRESULT>)(*(*(void***)_wrappedInstance + 12)))
                    (_wrappedInstance, psi_local);
                if (result.Failed())
                {
                    Marshal.ThrowExceptionForHR((int)result);
                }
            }

            void Shell32.IFileOpenDialog.GetFolder(out Shell32.IShellItem? ppsi)
            {
                HRESULT result;
                IntPtr ppsi_local;
                result = ((delegate* unmanaged<IntPtr, IntPtr*, HRESULT>)(*(*(void***)_wrappedInstance + 13)))
                    (_wrappedInstance, &ppsi_local);
                if (result.Failed())
                {
                    Marshal.ThrowExceptionForHR((int)result);
                }

                ppsi = ppsi_local == IntPtr.Zero ? null : (Shell32.IShellItem)WinFormsComWrappers.Instance.GetOrCreateObjectForComInstance(ppsi_local, CreateObjectFlags.Unwrap);
            }

            HRESULT Shell32.IFileOpenDialog.GetCurrentSelection(out Shell32.IShellItem? ppsi)
            {
                IntPtr ppsi_local;
                HRESULT retVal;
                retVal = ((delegate* unmanaged<IntPtr, IntPtr*, HRESULT>)(*(*(void***)_wrappedInstance + 14)))
                    (_wrappedInstance, &ppsi_local);
                ppsi = ppsi_local == IntPtr.Zero ? null : (Shell32.IShellItem)WinFormsComWrappers.Instance.GetOrCreateObjectForComInstance(ppsi_local, CreateObjectFlags.Unwrap);
                return retVal;
            }

            void Shell32.IFileOpenDialog.SetFileName(string pszName)
            {
                HRESULT result;
                fixed (char* pszName_local = pszName)
                {
                    result = ((delegate* unmanaged<IntPtr, char*, HRESULT>)(*(*(void***)_wrappedInstance + 15)))
                        (_wrappedInstance, pszName_local);
                }

                if (result.Failed())
                {
                    Marshal.ThrowExceptionForHR((int)result);
                }
            }

            void Shell32.IFileOpenDialog.GetFileName(out string pszName)
            {
                HRESULT result;
                IntPtr pszName_local;
                result = ((delegate* unmanaged<IntPtr, IntPtr*, HRESULT>)(*(*(void***)_wrappedInstance + 16)))
                    (_wrappedInstance, &pszName_local);
                if (result.Failed())
                {
                    Marshal.ThrowExceptionForHR((int)result);
                }

                pszName = Marshal.PtrToStringUni(pszName_local)!;
                Marshal.FreeCoTaskMem(ppszName_local);
            }

            void Shell32.IFileOpenDialog.SetTitle(string pszTitle)
            {
                HRESULT result;
                fixed (char* pszTitle_local = pszTitle)
                {
                    result = ((delegate* unmanaged<IntPtr, char*, HRESULT>)(*(*(void***)_wrappedInstance + 17)))
                        (_wrappedInstance, pszTitle_local);
                }

                if (result.Failed())
                {
                    Marshal.ThrowExceptionForHR((int)result);
                }
            }

            HRESULT Shell32.IFileOpenDialog.SetOkButtonLabel(string pszText)
            {
                HRESULT retVal;
                fixed (char* pszText_local = pszText)
                {
                    retVal = ((delegate* unmanaged<IntPtr, char*, HRESULT>)(*(*(void***)_wrappedInstance + 18)))
                        (_wrappedInstance, pszText_local);
                }

                return retVal;
            }

            HRESULT Shell32.IFileOpenDialog.SetFileNameLabel(string pszLabel)
            {
                HRESULT retVal;
                fixed (char* pszLabel_local = pszLabel)
                {
                    retVal = ((delegate* unmanaged<IntPtr, char*, HRESULT>)(*(*(void***)_wrappedInstance + 19)))
                        (_wrappedInstance, pszLabel_local);
                }

                return retVal;
            }

            void Shell32.IFileOpenDialog.GetResult(out Shell32.IShellItem? ppsi)
            {
                HRESULT result;
                IntPtr ppsi_local;
                result = ((delegate* unmanaged<IntPtr, IntPtr*, HRESULT>)(*(*(void***)_wrappedInstance + 20)))
                    (_wrappedInstance, &ppsi_local);
                if (result.Failed())
                {
                    Marshal.ThrowExceptionForHR((int)result);
                }

                ppsi = ppsi_local == IntPtr.Zero ? null : (Shell32.IShellItem)WinFormsComWrappers.Instance.GetOrCreateObjectForComInstance(ppsi_local, CreateObjectFlags.Unwrap);
            }

            HRESULT Shell32.IFileOpenDialog.AddPlace(Shell32.IShellItem psi, Shell32.FDAP fdap)
            {
                HRESULT retVal;
                IntPtr psi_local = WinFormsComWrappers.Instance.GetComPointer(psi, IID.IShellItem);
                retVal = ((delegate* unmanaged<IntPtr, IntPtr, Shell32.FDAP, HRESULT>)(*(*(void***)_wrappedInstance + 21)))
                    (_wrappedInstance, psi_local, fdap);
                return retVal;
            }

            void Shell32.IFileOpenDialog.SetDefaultExtension(string pszDefaultExtension)
            {
                HRESULT result;
                fixed (char* pszDefaultExtension_local = pszDefaultExtension)
                {
                    result = ((delegate* unmanaged<IntPtr, char*, HRESULT>)(*(*(void***)_wrappedInstance + 22)))
                        (_wrappedInstance, pszDefaultExtension_local);
                }

                if (result.Failed())
                {
                    Marshal.ThrowExceptionForHR((int)result);
                }
            }

            void Shell32.IFileOpenDialog.Close(int hr)
            {
                HRESULT result = ((delegate* unmanaged<IntPtr, int, HRESULT>)(*(*(void***)_wrappedInstance + 23)))
                    (_wrappedInstance, hr);
                if (result.Failed())
                {
                    Marshal.ThrowExceptionForHR((int)result);
                }
            }

            void Shell32.IFileOpenDialog.SetClientGuid(ref Guid guid)
            {
                HRESULT result;
                fixed (Guid* guid_local = &guid)
                {
                    result = ((delegate* unmanaged<IntPtr, Guid*, HRESULT>)(*(*(void***)_wrappedInstance + 24)))
                        (_wrappedInstance, guid_local);
                }

                if (result.Failed())
                {
                    Marshal.ThrowExceptionForHR((int)result);
                }
            }

            HRESULT Shell32.IFileOpenDialog.ClearClientData()
            {
                HRESULT retVal;
                retVal = ((delegate* unmanaged<IntPtr, HRESULT>)(*(*(void***)_wrappedInstance + 25)))
                    (_wrappedInstance);
                return retVal;
            }

            HRESULT Shell32.IFileOpenDialog.SetFilter(IntPtr pFilter)
            {
                HRESULT retVal;
                retVal = ((delegate* unmanaged<IntPtr, IntPtr, HRESULT>)(*(*(void***)_wrappedInstance + 26)))
                    (_wrappedInstance, pFilter);
                return retVal;
            }

            void Shell32.IFileOpenDialog.GetResults(out Shell32.IShellItemArray? ppenum)
            {
                HRESULT result;
                IntPtr ppenum_local;
                result = ((delegate* unmanaged<IntPtr, IntPtr*, HRESULT>)(*(*(void***)_wrappedInstance + 27)))
                    (_wrappedInstance, &ppenum_local);
                if (result.Failed())
                {
                    Marshal.ThrowExceptionForHR((int)result);
                }

                ppenum = ppenum_local == IntPtr.Zero ? null : (Shell32.IShellItemArray)Marshal.GetObjectForIUnknown(ppenum_local);
            }

            HRESULT Shell32.IFileOpenDialog.GetSelectedItems(out Shell32.IShellItemArray? ppsai)
            {
                IntPtr ppsai_local;
                HRESULT retVal;
                retVal = ((delegate* unmanaged<IntPtr, IntPtr*, HRESULT>)(*(*(void***)_wrappedInstance + 28)))
                    (_wrappedInstance, &ppsai_local);
                ppsai = ppsai_local == IntPtr.Zero ? null : (Shell32.IShellItemArray)Marshal.GetObjectForIUnknown(ppsai_local);
                return retVal;
            }
        }
    }
}
