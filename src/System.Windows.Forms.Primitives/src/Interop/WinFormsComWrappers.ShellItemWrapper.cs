// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using System.Windows.Forms;

internal partial class Interop
{
    internal unsafe partial class WinFormsComWrappers
    {
        private class ShellItemWrapper : Shell32.IShellItem, IDisposable
        {
            private IntPtr _wrappedInstance;

            public ShellItemWrapper(IntPtr wrappedInstance)
            {
                _wrappedInstance = wrappedInstance.OrThrowIfZero();
            }

            internal IntPtr Instance => _wrappedInstance;

            public void Dispose()
            {
                Marshal.Release(_wrappedInstance);
                _wrappedInstance = IntPtr.Zero;
            }

            HRESULT Shell32.IShellItem.BindToHandler(IntPtr pbc, ref Guid bhid, ref Guid riid, out IntPtr ppv)
            {
                HRESULT retVal;
                fixed (Guid* bhid_local = &bhid)
                fixed (Guid* riid_local = &riid)
                fixed (IntPtr* ppv_local = &ppv)
                {
                    retVal = ((delegate* unmanaged<IntPtr, IntPtr, Guid*, Guid*, IntPtr*, HRESULT>)(*(*(void***)_wrappedInstance + 3)))
                        (_wrappedInstance, pbc, bhid_local, riid_local, ppv_local);
                }

                return retVal;
            }

            HRESULT Shell32.IShellItem.GetParent(out Shell32.IShellItem? ppsi)
            {
                IntPtr ppsi_local;
                HRESULT retVal;
                retVal = ((delegate* unmanaged<IntPtr, IntPtr*, HRESULT>)(*(*(void***)_wrappedInstance + 4)))
                    (_wrappedInstance, &ppsi_local);
                ppsi = ppsi_local == IntPtr.Zero ? null : (Shell32.IShellItem)WinFormsComWrappers.Instance.GetOrCreateObjectForComInstance(ppsi_local, CreateObjectFlags.Unwrap);
                return retVal;
            }

            HRESULT Shell32.IShellItem.GetDisplayName(SIGDN sigdnName, out string? ppszName)
            {
                IntPtr ppszName_local;
                HRESULT retVal;
                retVal = ((delegate* unmanaged<IntPtr, int, IntPtr*, HRESULT>)(*(*(void***)_wrappedInstance + 5)))
                    (_wrappedInstance, (int)sigdnName, &ppszName_local);
                ppszName = Marshal.PtrToStringUni(ppszName_local);
                Marshal.FreeCoTaskMem(ppszName_local);
                return retVal;
            }

            HRESULT Shell32.IShellItem.GetAttributes(Shell32.SFGAOF sfgaoMask, out Shell32.SFGAOF psfgaoAttribs)
            {
                Shell32.SFGAOF psfgaoAttribs_local;
                HRESULT retVal;
                retVal = ((delegate* unmanaged<IntPtr, Shell32.SFGAOF, Shell32.SFGAOF*, HRESULT>)(*(*(void***)_wrappedInstance + 6)))
                    (_wrappedInstance, sfgaoMask, &psfgaoAttribs_local);
                psfgaoAttribs = psfgaoAttribs_local;
                return retVal;
            }

            HRESULT Shell32.IShellItem.Compare(Shell32.IShellItem psi, uint hint, out int piOrder)
            {
                HRESULT retVal;
                IntPtr ppv_local = WinFormsComWrappers.Instance.GetComPointer(psi, IID.IShellItem);
                fixed (int* piOrder_local = &piOrder)
                {
                    retVal = ((delegate* unmanaged<IntPtr, IntPtr, uint, int*, HRESULT>)(*(*(void***)_wrappedInstance + 7)))
                        (_wrappedInstance, ppv_local, hint, piOrder_local);
                }

                return retVal;
            }
        }
    }
}
