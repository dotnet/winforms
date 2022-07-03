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
        internal static class IOleWindowVtbl
        {
            public static IntPtr Create(IntPtr fpQueryInterface, IntPtr fpAddRef, IntPtr fpRelease)
            {
                IntPtr* vtblRaw = (IntPtr*)RuntimeHelpers.AllocateTypeAssociatedMemory(typeof(IEnumStringVtbl), IntPtr.Size * 5);
                vtblRaw[0] = fpQueryInterface;
                vtblRaw[1] = fpAddRef;
                vtblRaw[2] = fpRelease;
                vtblRaw[3] = (IntPtr)(delegate* unmanaged<IntPtr, IntPtr*, HRESULT>)&GetWindow;
                vtblRaw[4] = (IntPtr)(delegate* unmanaged<IntPtr, BOOL, HRESULT>)&ContextSensitiveHelp;

                return (IntPtr)vtblRaw;
            }

            [UnmanagedCallersOnly]
            private static HRESULT GetWindow(IntPtr thisPtr, IntPtr* phwnd)
            {
                if (phwnd is null)
                {
                    return HRESULT.E_INVALIDARG;
                }

                try
                {
                    var instance = ComInterfaceDispatch.GetInstance<Ole32.IOleWindow>((ComInterfaceDispatch*)thisPtr);
                    var result = instance.GetWindow(phwnd);
                    return result;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    return (HRESULT)ex.HResult;
                }
            }

            [UnmanagedCallersOnly]
            private static HRESULT ContextSensitiveHelp(IntPtr thisPtr, BOOL fEnterMode)
            {
                try
                {
                    var instance = ComInterfaceDispatch.GetInstance<Ole32.IOleWindow>((ComInterfaceDispatch*)thisPtr);
                    return instance.ContextSensitiveHelp(fEnterMode);
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
