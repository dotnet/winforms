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
        internal static class ILegacyIAccessibleProviderVtbl
        {
            public static IntPtr Create(IntPtr fpQueryInterface, IntPtr fpAddRef, IntPtr fpRelease)
            {
                IntPtr* vtblRaw = (IntPtr*)RuntimeHelpers.AllocateTypeAssociatedMemory(typeof(ILegacyIAccessibleProviderVtbl), IntPtr.Size * 17);
                vtblRaw[0] = fpQueryInterface;
                vtblRaw[1] = fpAddRef;
                vtblRaw[2] = fpRelease;
                vtblRaw[3] = (IntPtr)(delegate* unmanaged<IntPtr, int, HRESULT>)&Select;
                vtblRaw[4] = (IntPtr)(delegate* unmanaged<IntPtr, HRESULT>)&DoDefaultAction;
                vtblRaw[5] = (IntPtr)(delegate* unmanaged<IntPtr, IntPtr, HRESULT>)&SetValue;
                vtblRaw[6] = (IntPtr)(delegate* unmanaged<IntPtr, IntPtr*, HRESULT>)&GetIAccessible;
                vtblRaw[7] = (IntPtr)(delegate* unmanaged<IntPtr, int*, HRESULT>)&get_ChildId;
                vtblRaw[8] = (IntPtr)(delegate* unmanaged<IntPtr, IntPtr*, HRESULT>)&get_Name;
                vtblRaw[9] = (IntPtr)(delegate* unmanaged<IntPtr, IntPtr*, HRESULT>)&get_Value;
                vtblRaw[10] = (IntPtr)(delegate* unmanaged<IntPtr, IntPtr*, HRESULT>)&get_Description;
                vtblRaw[11] = (IntPtr)(delegate* unmanaged<IntPtr, uint*, HRESULT>)&get_Role;
                vtblRaw[12] = (IntPtr)(delegate* unmanaged<IntPtr, uint*, HRESULT>)&get_State;
                vtblRaw[13] = (IntPtr)(delegate* unmanaged<IntPtr, IntPtr*, HRESULT>)&get_Help;
                vtblRaw[14] = (IntPtr)(delegate* unmanaged<IntPtr, IntPtr*, HRESULT>)&get_KeyboardShortcut;
                vtblRaw[15] = (IntPtr)(delegate* unmanaged<IntPtr, IntPtr*, HRESULT>)&GetSelection;
                vtblRaw[16] = (IntPtr)(delegate* unmanaged<IntPtr, IntPtr*, HRESULT>)&get_DefaultAction;

                return (IntPtr)vtblRaw;
            }

            [UnmanagedCallersOnly]
            private static HRESULT Select(IntPtr thisPtr, int flagsSelect)
            {
                var instance = ComInterfaceDispatch.GetInstance<UiaCore.ILegacyIAccessibleProvider>((ComInterfaceDispatch*)thisPtr);
                try
                {
                    instance.Select(flagsSelect);
                    return HRESULT.S_OK;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    return (HRESULT)ex.HResult;
                }
            }

            [UnmanagedCallersOnly]
            private static HRESULT DoDefaultAction(IntPtr thisPtr)
            {
                var instance = ComInterfaceDispatch.GetInstance<UiaCore.ILegacyIAccessibleProvider>((ComInterfaceDispatch*)thisPtr);
                try
                {
                    instance.DoDefaultAction();
                    return HRESULT.S_OK;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    return (HRESULT)ex.HResult;
                }
            }

            [UnmanagedCallersOnly]
            private static HRESULT SetValue(IntPtr thisPtr, IntPtr szValue)
            {
                var instance = ComInterfaceDispatch.GetInstance<UiaCore.ILegacyIAccessibleProvider>((ComInterfaceDispatch*)thisPtr);
                try
                {
                    var value = Marshal.PtrToStringUni(szValue);
                    instance.SetValue(value!);
                    return HRESULT.S_OK;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    return (HRESULT)ex.HResult;
                }
            }

            [UnmanagedCallersOnly]
            private static HRESULT GetIAccessible(IntPtr thisPtr, IntPtr* pRetVal)
            {
                if (pRetVal is null)
                {
                    return HRESULT.E_INVALIDARG;
                }

                var instance = ComInterfaceDispatch.GetInstance<UiaCore.ILegacyIAccessibleProvider>((ComInterfaceDispatch*)thisPtr);
                try
                {
                    var result = instance.GetIAccessible();
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
            private static HRESULT get_ChildId(IntPtr thisPtr, int* pRetVal)
            {
                if (pRetVal is null)
                {
                    return HRESULT.E_INVALIDARG;
                }

                var instance = ComInterfaceDispatch.GetInstance<UiaCore.ILegacyIAccessibleProvider>((ComInterfaceDispatch*)thisPtr);
                try
                {
                    *pRetVal = instance.ChildId;
                    return HRESULT.S_OK;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    return (HRESULT)ex.HResult;
                }
            }

            [UnmanagedCallersOnly]
            private static HRESULT get_Name(IntPtr thisPtr, IntPtr* pRetVal)
            {
                if (pRetVal is null)
                {
                    return HRESULT.E_INVALIDARG;
                }

                var instance = ComInterfaceDispatch.GetInstance<UiaCore.ILegacyIAccessibleProvider>((ComInterfaceDispatch*)thisPtr);
                try
                {
                    var result = instance.Name;
                    *pRetVal = Marshal.StringToCoTaskMemUni(result);
                    return HRESULT.S_OK;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    return (HRESULT)ex.HResult;
                }
            }

            [UnmanagedCallersOnly]
            private static HRESULT get_Value(IntPtr thisPtr, IntPtr* pRetVal)
            {
                if (pRetVal is null)
                {
                    return HRESULT.E_INVALIDARG;
                }

                var instance = ComInterfaceDispatch.GetInstance<UiaCore.ILegacyIAccessibleProvider>((ComInterfaceDispatch*)thisPtr);
                try
                {
                    var result = instance.Value;
                    *pRetVal = Marshal.StringToCoTaskMemUni(result);
                    return HRESULT.S_OK;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    return (HRESULT)ex.HResult;
                }
            }

            [UnmanagedCallersOnly]
            private static HRESULT get_Description(IntPtr thisPtr, IntPtr* pRetVal)
            {
                if (pRetVal is null)
                {
                    return HRESULT.E_INVALIDARG;
                }

                var instance = ComInterfaceDispatch.GetInstance<UiaCore.ILegacyIAccessibleProvider>((ComInterfaceDispatch*)thisPtr);
                try
                {
                    var result = instance.Description;
                    *pRetVal = Marshal.StringToCoTaskMemUni(result);
                    return HRESULT.S_OK;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    return (HRESULT)ex.HResult;
                }
            }

            [UnmanagedCallersOnly]
            private static HRESULT get_Role(IntPtr thisPtr, uint* pRetVal)
            {
                if (pRetVal is null)
                {
                    return HRESULT.E_INVALIDARG;
                }

                var instance = ComInterfaceDispatch.GetInstance<UiaCore.ILegacyIAccessibleProvider>((ComInterfaceDispatch*)thisPtr);
                try
                {
                    *pRetVal = instance.Role;
                    return HRESULT.S_OK;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    return (HRESULT)ex.HResult;
                }
            }

            [UnmanagedCallersOnly]
            private static HRESULT get_State(IntPtr thisPtr, uint* pRetVal)
            {
                if (pRetVal is null)
                {
                    return HRESULT.E_INVALIDARG;
                }

                var instance = ComInterfaceDispatch.GetInstance<UiaCore.ILegacyIAccessibleProvider>((ComInterfaceDispatch*)thisPtr);
                try
                {
                    *pRetVal = instance.State;
                    return HRESULT.S_OK;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    return (HRESULT)ex.HResult;
                }
            }

            [UnmanagedCallersOnly]
            private static HRESULT get_Help(IntPtr thisPtr, IntPtr* pRetVal)
            {
                if (pRetVal is null)
                {
                    return HRESULT.E_INVALIDARG;
                }

                var instance = ComInterfaceDispatch.GetInstance<UiaCore.ILegacyIAccessibleProvider>((ComInterfaceDispatch*)thisPtr);
                try
                {
                    var result = instance.Help;
                    *pRetVal = Marshal.StringToCoTaskMemUni(result);
                    return HRESULT.S_OK;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    return (HRESULT)ex.HResult;
                }
            }

            [UnmanagedCallersOnly]
            private static HRESULT get_KeyboardShortcut(IntPtr thisPtr, IntPtr* pRetVal)
            {
                if (pRetVal is null)
                {
                    return HRESULT.E_INVALIDARG;
                }

                var instance = ComInterfaceDispatch.GetInstance<UiaCore.ILegacyIAccessibleProvider>((ComInterfaceDispatch*)thisPtr);
                try
                {
                    var result = instance.KeyboardShortcut;
                    *pRetVal = Marshal.StringToCoTaskMemUni(result);
                    return HRESULT.S_OK;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    return (HRESULT)ex.HResult;
                }
            }

            [UnmanagedCallersOnly]
            private static HRESULT GetSelection(IntPtr thisPtr, IntPtr* pRetVal)
            {
                if (pRetVal is null)
                {
                    return HRESULT.E_INVALIDARG;
                }

                var instance = ComInterfaceDispatch.GetInstance<UiaCore.ILegacyIAccessibleProvider>((ComInterfaceDispatch*)thisPtr);
                try
                {
                    var result = instance.GetSelection();
                    Oleaut32.SAFEARRAY* array = Oleaut32.SafeArrayCreateVector(Ole32.VARENUM.UNKNOWN, 0, (uint)result.Length);
                    for (int i = 0; i < result.Length; i++)
                    {
                        Oleaut32.SafeArrayPutElement(array, in i, Instance.GetComPointer(result[i], IID.IRawElementProviderSimple));
                    }

                    *pRetVal = (IntPtr)array;
                    return HRESULT.S_OK;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    return (HRESULT)ex.HResult;
                }
            }

            [UnmanagedCallersOnly]
            private static HRESULT get_DefaultAction(IntPtr thisPtr, IntPtr* pRetVal)
            {
                if (pRetVal is null)
                {
                    return HRESULT.E_INVALIDARG;
                }

                var instance = ComInterfaceDispatch.GetInstance<UiaCore.ILegacyIAccessibleProvider>((ComInterfaceDispatch*)thisPtr);
                try
                {
                    var result = instance.DefaultAction;
                    *pRetVal = Marshal.StringToCoTaskMemUni(result);
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
