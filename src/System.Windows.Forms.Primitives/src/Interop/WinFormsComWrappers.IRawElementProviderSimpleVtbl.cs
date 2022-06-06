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
        internal static class IRawElementProviderSimpleVtbl
        {
            public static IntPtr Create(IntPtr fpQueryInterface, IntPtr fpAddRef, IntPtr fpRelease)
            {
                IntPtr* vtblRaw = (IntPtr*)RuntimeHelpers.AllocateTypeAssociatedMemory(typeof(IRawElementProviderSimpleVtbl), IntPtr.Size * 7);
                vtblRaw[0] = fpQueryInterface;
                vtblRaw[1] = fpAddRef;
                vtblRaw[2] = fpRelease;
                vtblRaw[3] = (IntPtr)(delegate* unmanaged<IntPtr, UiaCore.ProviderOptions*, HRESULT>)&get_ProviderOptions;
                vtblRaw[4] = (IntPtr)(delegate* unmanaged<IntPtr, UiaCore.UIA, IntPtr*, HRESULT>)&GetPatternProvider;
                vtblRaw[5] = (IntPtr)(delegate* unmanaged<IntPtr, UiaCore.UIA, IntPtr*, HRESULT>)&GetPropertyValue;
                vtblRaw[6] = (IntPtr)(delegate* unmanaged<IntPtr, IntPtr*, HRESULT>)&get_HostRawElementProvider;

                return (IntPtr)vtblRaw;
            }

            [UnmanagedCallersOnly]
            private static HRESULT get_ProviderOptions(IntPtr thisPtr, UiaCore.ProviderOptions* pRetVal)
            {
                if (pRetVal is null)
                {
                    return HRESULT.E_INVALIDARG;
                }

                var instance = ComInterfaceDispatch.GetInstance<UiaCore.IRawElementProviderSimple>((ComInterfaceDispatch*)thisPtr);
                try
                {
                    *pRetVal = instance.ProviderOptions;
                    return HRESULT.S_OK;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    return (HRESULT)ex.HResult;
                }
            }

            [UnmanagedCallersOnly]
            private static HRESULT GetPropertyValue(IntPtr thisPtr, UiaCore.UIA patternId, IntPtr* pRetVal)
            {
                if (pRetVal is null)
                {
                    return HRESULT.E_INVALIDARG;
                }

                var instance = ComInterfaceDispatch.GetInstance<UiaCore.IRawElementProviderSimple>((ComInterfaceDispatch*)thisPtr);
                try
                {
                    object? result = instance.GetPropertyValue(patternId);
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
            private static HRESULT GetPatternProvider(IntPtr thisPtr, UiaCore.UIA patternId, IntPtr* pRetVal)
            {
                if (pRetVal is null)
                {
                    return HRESULT.E_INVALIDARG;
                }

                var instance = ComInterfaceDispatch.GetInstance<UiaCore.IRawElementProviderSimple>((ComInterfaceDispatch*)thisPtr);
                try
                {
                    object? result = instance.GetPatternProvider(patternId);
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
            private static HRESULT get_HostRawElementProvider(IntPtr thisPtr, IntPtr* pRetVal)
            {
                if (pRetVal is null)
                {
                    return HRESULT.E_INVALIDARG;
                }

                var instance = ComInterfaceDispatch.GetInstance<UiaCore.IRawElementProviderSimple>((ComInterfaceDispatch*)thisPtr);
                try
                {
                    UiaCore.IRawElementProviderSimple? result = instance.HostRawElementProvider;
                    if (result is null)
                    {
                        *pRetVal = IntPtr.Zero;
                    }
                    else
                    {
                        if (result is RawElementProviderSimpleWrapper wrapper)
                        {
                            *pRetVal = wrapper.Instance;
                            Marshal.AddRef(wrapper.Instance);
                        }
                        else
                        {
                            *pRetVal = WinFormsComWrappers.Instance.GetOrCreateComInterfaceForObject(result, CreateComInterfaceFlags.None);
                        }
                    }

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
