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

            Interop.HRESULT Interop.Shell32.IShellItem.BindToHandler(IntPtr pbc, ref Guid bhid, ref Guid riid, out IntPtr ppv)
            {
                int retVal;
                fixed (Guid* local_1 = &bhid)
                fixed (Guid* local_2 = &riid)
                fixed (IntPtr* local_3 = &ppv)
                retVal = ((delegate* unmanaged<System.IntPtr, IntPtr, Guid*, Guid*, IntPtr*, int>)(*(*(void***)_wrappedInstance + 3)))
                    (_wrappedInstance, pbc, local_1, local_2, local_3);
                return (Interop.HRESULT)retVal;
            }

            Interop.HRESULT Interop.Shell32.IShellItem.GetParent(out Interop.Shell32.IShellItem ppsi)
            {
                System.IntPtr local_0;
                int retVal;
                retVal = ((delegate* unmanaged<System.IntPtr, System.IntPtr*, int>)(*(*(void***)_wrappedInstance + 4)))
                    (_wrappedInstance, &local_0);
                ppsi = local_0 == System.IntPtr.Zero ? null! : (Interop.Shell32.IShellItem)WinFormsComWrappers.Instance.GetOrCreateObjectForComInstance(local_0, CreateObjectFlags.Unwrap);
                return (Interop.HRESULT)retVal;
            }

            Interop.HRESULT Interop.Shell32.IShellItem.GetDisplayName(Interop.Shell32.SIGDN sigdnName, out string ppszName)
            {
                System.IntPtr local_1;
                int retVal;
                retVal = ((delegate* unmanaged<System.IntPtr, int, System.IntPtr*, int>)(*(*(void***)_wrappedInstance + 5)))
                    (_wrappedInstance, (int)sigdnName, &local_1);
                ppszName = Marshal.PtrToStringUni(local_1)!;
                return (Interop.HRESULT)retVal;
            }

            Interop.HRESULT Interop.Shell32.IShellItem.GetAttributes(Interop.Shell32.SFGAOF sfgaoMask, out Interop.Shell32.SFGAOF psfgaoAttribs)
            {
                int local_1;
                int retVal;
                retVal = ((delegate* unmanaged<System.IntPtr, int, int*, int>)(*(*(void***)_wrappedInstance + 6)))
                    (_wrappedInstance, (int)sfgaoMask, &local_1);
                psfgaoAttribs = (Interop.Shell32.SFGAOF)local_1;
                return (Interop.HRESULT)retVal;
            }

            Interop.HRESULT Interop.Shell32.IShellItem.Compare(Interop.Shell32.IShellItem psi, uint hint, out int piOrder)
            {
                int result;
                System.IntPtr local_0;
                if (psi == null)
                {
                    local_0 = System.IntPtr.Zero;
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
                fixed (int* local_2 = &piOrder)
                retVal = ((delegate* unmanaged<System.IntPtr, System.IntPtr, uint, int*, int>)(*(*(void***)_wrappedInstance + 7)))
                    (_wrappedInstance, local_0, hint, local_2);
                return (Interop.HRESULT)retVal;
            }
        }
    }
}
