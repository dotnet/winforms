// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

internal partial class Interop
{
    internal unsafe partial class WinFormsComWrappers
    {
        private class ShellItemWrapper : Shell32.IShellItem, IDisposable
        {
            private IntPtr _wrappedInstance;

            public ShellItemWrapper(IntPtr wrappedInstance)
            {
                ArgumentNullException.ThrowIfNull(wrappedInstance);

                _wrappedInstance = wrappedInstance;
            }

            public void Dispose()
            {
                Marshal.Release(_wrappedInstance);
                _wrappedInstance = IntPtr.Zero;
            }

            HRESULT Shell32.IShellItem.BindToHandler(IntPtr pbc, ref Guid bhid, ref Guid riid, out IntPtr ppv)
            {
                int retVal;
                fixed (Guid* bhid_local = &bhid)
                fixed (Guid* riid_local = &riid)
                fixed (IntPtr* ppv_local = &ppv)
                {
                    retVal = ((delegate* unmanaged<IntPtr, IntPtr, Guid*, Guid*, IntPtr*, int>)(*(*(void***)_wrappedInstance + 3)))
                        (_wrappedInstance, pbc, bhid_local, riid_local, ppv_local);
                }

                return (HRESULT)retVal;
            }

            HRESULT Shell32.IShellItem.GetParent(out Shell32.IShellItem ppsi)
            {
                IntPtr ppsi_local;
                int retVal;
                retVal = ((delegate* unmanaged<IntPtr, IntPtr*, int>)(*(*(void***)_wrappedInstance + 4)))
                    (_wrappedInstance, &ppsi_local);
                ppsi = ppsi_local == IntPtr.Zero ? null! : (Shell32.IShellItem)WinFormsComWrappers.Instance.GetOrCreateObjectForComInstance(ppsi_local, CreateObjectFlags.Unwrap);
                return (HRESULT)retVal;
            }

            HRESULT Shell32.IShellItem.GetDisplayName(Shell32.SIGDN sigdnName, out string ppszName)
            {
                IntPtr ppszName_local;
                int retVal;
                retVal = ((delegate* unmanaged<IntPtr, int, IntPtr*, int>)(*(*(void***)_wrappedInstance + 5)))
                    (_wrappedInstance, (int)sigdnName, &ppszName_local);
                ppszName = Marshal.PtrToStringUni(ppszName_local)!;
                Marshal.FreeCoTaskMem(ppszName_local);
                return (HRESULT)retVal;
            }

            HRESULT Shell32.IShellItem.GetAttributes(Shell32.SFGAOF sfgaoMask, out Shell32.SFGAOF psfgaoAttribs)
            {
                Shell32.SFGAOF psfgaoAttribs_local;
                int retVal;
                retVal = ((delegate* unmanaged<IntPtr, Shell32.SFGAOF, Shell32.SFGAOF*, int>)(*(*(void***)_wrappedInstance + 6)))
                    (_wrappedInstance, sfgaoMask, &psfgaoAttribs_local);
                psfgaoAttribs = psfgaoAttribs_local;
                return (HRESULT)retVal;
            }

            HRESULT Shell32.IShellItem.Compare(Shell32.IShellItem psi, uint hint, out int piOrder)
            {
                int result;
                IntPtr ppv_local;
                if (psi == null)
                {
                    ppv_local = IntPtr.Zero;
                }
                else
                {
                    var pUnk_local = WinFormsComWrappers.Instance.GetOrCreateComInterfaceForObject(psi, CreateComInterfaceFlags.None);
                    var local_psi_IID = IID.IShellItem;
                    result = Marshal.QueryInterface(pUnk_local, ref local_psi_IID, out ppv_local);
                    Marshal.Release(pUnk_local);
                    if (result != 0)
                    {
                        Marshal.ThrowExceptionForHR(result);
                    }
                }

                int retVal;
                fixed (int* piOrder_local = &piOrder)
                {
                    retVal = ((delegate* unmanaged<IntPtr, IntPtr, uint, int*, int>)(*(*(void***)_wrappedInstance + 7)))
                        (_wrappedInstance, ppv_local, hint, piOrder_local);
                }

                return (HRESULT)retVal;
            }
        }
    }
}
