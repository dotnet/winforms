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
        internal static class IRawElementProviderFragmentRootVtbl
        {
            public static IntPtr Create(IntPtr fpQueryInterface, IntPtr fpAddRef, IntPtr fpRelease)
            {
                IntPtr* vtblRaw = (IntPtr*)RuntimeHelpers.AllocateTypeAssociatedMemory(typeof(IRawElementProviderFragmentRootVtbl), IntPtr.Size * 5);
                vtblRaw[0] = fpQueryInterface;
                vtblRaw[1] = fpAddRef;
                vtblRaw[2] = fpRelease;
                vtblRaw[3] = (IntPtr)(delegate* unmanaged<IntPtr, double, double, IntPtr*, HRESULT>)&ElementProviderFromPoint;
                vtblRaw[4] = (IntPtr)(delegate* unmanaged<IntPtr, IntPtr*, HRESULT>)&GetFocus;

                return (IntPtr)vtblRaw;
            }

            [UnmanagedCallersOnly]
            private static HRESULT ElementProviderFromPoint(IntPtr thisPtr, double x, double y, IntPtr* pRetVal)
            {
                if (pRetVal is null)
                {
                    return HRESULT.E_INVALIDARG;
                }

                var instance = ComInterfaceDispatch.GetInstance<UiaCore.IRawElementProviderFragmentRoot>((ComInterfaceDispatch*)thisPtr);
                try
                {
                    var result = instance.ElementProviderFromPoint(x, y);
                    *pRetVal = result is null ? IntPtr.Zero : Marshal.GetIUnknownForObject(result);
                    return HRESULT.S_OK;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    return (HRESULT)ex.HResult;
                }
            }

            [UnmanagedCallersOnly]
            private static HRESULT GetFocus(IntPtr thisPtr, IntPtr* pRetVal)
            {
                if (pRetVal == null)
                {
                    return HRESULT.E_INVALIDARG;
                }

                var instance = ComInterfaceDispatch.GetInstance<UiaCore.IRawElementProviderFragmentRoot>((ComInterfaceDispatch*)thisPtr);
                try
                {
                    var result = instance.GetFocus();
                    *pRetVal = result is null ? IntPtr.Zero : Marshal.GetIUnknownForObject(result);
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
