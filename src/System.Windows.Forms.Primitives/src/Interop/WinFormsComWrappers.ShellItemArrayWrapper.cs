// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using System.Windows.Forms;

internal partial class Interop
{
    internal unsafe partial class WinFormsComWrappers
    {
        internal class ShellItemArrayWrapper : Shell32.IShellItemArray, IDisposable
        {
            private IntPtr _wrappedInstance;

            public ShellItemArrayWrapper(IntPtr wrappedInstance)
            {
                _wrappedInstance = wrappedInstance.OrThrowIfZero();
            }

            internal IntPtr Instance => _wrappedInstance;

            public void Dispose()
            {
                Marshal.Release(_wrappedInstance);
                _wrappedInstance = IntPtr.Zero;
            }

            HRESULT Shell32.IShellItemArray.BindToHandler(IntPtr pbc, ref Guid bhid, ref Guid riid, out IntPtr ppv)
            {
                fixed (Guid* bhid_local = &bhid)
                fixed (Guid* riid_local = &riid)
                fixed (IntPtr* ppv_local = &ppv)
                {
                    return ((delegate* unmanaged<IntPtr, IntPtr, Guid*, Guid*, IntPtr*, HRESULT>)(*(*(void***)_wrappedInstance + 3)))
                        (_wrappedInstance, pbc, bhid_local, riid_local, ppv_local);
                }
            }

            HRESULT Shell32.IShellItemArray.GetPropertyStore(GETPROPERTYSTOREFLAGS flags, ref Guid riid, out IntPtr ppv)
            {
                fixed (Guid* riid_local = &riid)
                fixed (IntPtr* ppv_local = &ppv)
                {
                    return ((delegate* unmanaged<IntPtr, int, Guid*, IntPtr*, HRESULT>)(*(*(void***)_wrappedInstance + 4)))
                        (_wrappedInstance, (int)flags, riid_local, ppv_local);
                }
            }

            HRESULT Shell32.IShellItemArray.GetPropertyDescriptionList(ref Shell32.PROPERTYKEY keyType, ref Guid riid, out IntPtr ppv)
            {
                fixed (Shell32.PROPERTYKEY* keyType_local = &keyType)
                fixed (Guid* riid_local = &riid)
                fixed (IntPtr* ppv_local = &ppv)
                {
                    return ((delegate* unmanaged<IntPtr, Shell32.PROPERTYKEY*, Guid*, IntPtr*, HRESULT>)(*(*(void***)_wrappedInstance + 5)))
                        (_wrappedInstance, keyType_local, riid_local, ppv_local);
                }
            }

            HRESULT Shell32.IShellItemArray.GetAttributes(Shell32.SIATTRIBFLAGS dwAttribFlags, uint sfgaoMask, out uint psfgaoAttribs)
            {
                fixed (uint* psfgaoAttribs_local = &psfgaoAttribs)
                {
                    return ((delegate* unmanaged<IntPtr, int, uint, uint*, HRESULT>)(*(*(void***)_wrappedInstance + 6)))
                        (_wrappedInstance, (int)dwAttribFlags, sfgaoMask, psfgaoAttribs_local);
                }
            }

            public void GetCount(out uint pdwNumItems)
            {
                fixed (uint* pdwNumItems_local = &pdwNumItems)
                {
                    ((delegate* unmanaged<IntPtr, uint*, HRESULT>)(*(*(void***)_wrappedInstance + 7)))
                        (_wrappedInstance, pdwNumItems_local).ThrowIfFailed();
                }
            }

            public void GetItemAt(uint dwIndex, out Shell32.IShellItem ppsi)
            {
                IntPtr ppsi_local;
                ((delegate* unmanaged<IntPtr, uint, IntPtr*, HRESULT>)(*(*(void***)_wrappedInstance + 8)))
                    (_wrappedInstance, dwIndex, &ppsi_local).ThrowIfFailed();
                ppsi = (Shell32.IShellItem)WinFormsComWrappers.Instance.GetOrCreateObjectForComInstance(ppsi_local, CreateObjectFlags.Unwrap);
            }

            HRESULT Shell32.IShellItemArray.EnumItems(out IntPtr ppenumShellItems)
            {
                fixed (IntPtr* ppenumShellItems_local = &ppenumShellItems)
                {
                    return ((delegate* unmanaged<IntPtr, IntPtr*, HRESULT>)(*(*(void***)_wrappedInstance + 9)))
                        (_wrappedInstance, ppenumShellItems_local);
                }
            }
        }
    }
}
