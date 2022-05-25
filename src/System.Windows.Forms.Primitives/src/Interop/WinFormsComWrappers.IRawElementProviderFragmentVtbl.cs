// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

internal partial class Interop
{
    internal unsafe partial class WinFormsComWrappers
    {
        internal static class IRawElementProviderFragmentVtbl
        {
            public static IntPtr Create(IntPtr fpQueryInterface, IntPtr fpAddRef, IntPtr fpRelease)
            {
                IntPtr* vtblRaw = (IntPtr*)RuntimeHelpers.AllocateTypeAssociatedMemory(typeof(IRawElementProviderFragmentVtbl), IntPtr.Size * 9);
                vtblRaw[0] = fpQueryInterface;
                vtblRaw[1] = fpAddRef;
                vtblRaw[2] = fpRelease;
                vtblRaw[3] = (IntPtr)(delegate* unmanaged<IntPtr, UiaCore.NavigateDirection, IntPtr*, HRESULT>)&Navigate;
                vtblRaw[4] = (IntPtr)(delegate* unmanaged<IntPtr, IntPtr*, HRESULT>)&GetRuntimeId;
                vtblRaw[5] = (IntPtr)(delegate* unmanaged<IntPtr, UiaCore.UiaRect*, HRESULT>)&get_BoundingRectangle;
                vtblRaw[6] = (IntPtr)(delegate* unmanaged<IntPtr, IntPtr*, HRESULT>)&GetEmbeddedFragmentRoots;
                vtblRaw[7] = (IntPtr)(delegate* unmanaged<IntPtr, HRESULT>)&SetFocus;
                vtblRaw[8] = (IntPtr)(delegate* unmanaged<IntPtr, IntPtr*, HRESULT>)&FragmentRoot;

                return (IntPtr)vtblRaw;
            }

            [UnmanagedCallersOnly]
            private static HRESULT Navigate(IntPtr thisPtr, UiaCore.NavigateDirection direction, IntPtr* resultPtr)
            {
                var instance = ComInterfaceDispatch.GetInstance<UiaCore.IRawElementProviderFragment>((ComInterfaceDispatch*)thisPtr);
                try
                {
                    var result = instance.Navigate(direction);
                    *resultPtr = result is null ? IntPtr.Zero : Marshal.GetIUnknownForObject(result);
                    return HRESULT.S_OK;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    return (HRESULT)ex.HResult;
                }
            }

            [UnmanagedCallersOnly]
            private static HRESULT get_BoundingRectangle(IntPtr thisPtr, UiaCore.UiaRect* resultPtr)
            {
                var instance = ComInterfaceDispatch.GetInstance<UiaCore.IRawElementProviderFragment>((ComInterfaceDispatch*)thisPtr);
                try
                {
                    *resultPtr = instance.BoundingRectangle;
                    return HRESULT.S_OK;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    return (HRESULT)ex.HResult;
                }
            }

            [UnmanagedCallersOnly]
            private static HRESULT GetEmbeddedFragmentRoots(IntPtr thisPtr, IntPtr* resultPtr)
            {
                var instance = ComInterfaceDispatch.GetInstance<UiaCore.IRawElementProviderFragment>((ComInterfaceDispatch*)thisPtr);
                try
                {
                    var objects = instance.GetEmbeddedFragmentRoots();
                    if (objects == null)
                    {
                        *resultPtr = IntPtr.Zero;
                        return HRESULT.S_OK;
                    }

                    return HRESULT.E_NOTIMPL;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    return (HRESULT)ex.HResult;
                }
            }

            [UnmanagedCallersOnly]
            private static unsafe HRESULT GetRuntimeId(IntPtr thisPtr, IntPtr* resultPtr)
            {
                var instance = ComInterfaceDispatch.GetInstance<UiaCore.IRawElementProviderFragment>((ComInterfaceDispatch*)thisPtr);
                try
                {
                    var result = instance.GetRuntimeId();
                    if (result == null)
                    {
                        *resultPtr = IntPtr.Zero;
                        return HRESULT.S_OK;
                    }

                    var array = Oleaut32.SafeArrayCreateVector(Ole32.VARENUM.I4, 0, (uint)result.Length);
                    fixed (int* pResult = result)
                    {
                        for (var i = 0; i < result.Length; i++)
                        {
                            Oleaut32.SafeArrayPutElement(array, in i, (IntPtr)(pResult + i));
                        }
                    }

                    *resultPtr = (IntPtr)array;
                    return HRESULT.S_OK;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    return (HRESULT)ex.HResult;
                }
            }

            [UnmanagedCallersOnly]
            private static HRESULT SetFocus(IntPtr thisPtr)
            {
                var instance = ComInterfaceDispatch.GetInstance<UiaCore.IRawElementProviderFragment>((ComInterfaceDispatch*)thisPtr);
                try
                {
                    instance.SetFocus();
                    return HRESULT.S_OK;
                }
                catch (Exception ex)
                {
                    return (HRESULT)ex.HResult;
                }
            }

            [UnmanagedCallersOnly]
            private static HRESULT FragmentRoot(IntPtr thisPtr, IntPtr* resultPtr)
            {
                var instance = ComInterfaceDispatch.GetInstance<UiaCore.IRawElementProviderFragment>((ComInterfaceDispatch*)thisPtr);
                try
                {
                    var result = instance.FragmentRoot;
                    *resultPtr = result is null ? IntPtr.Zero : Marshal.GetIUnknownForObject(result);
                    return HRESULT.S_OK;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    return (HRESULT)ex.HResult;
                }
            }
        }
    }
}
