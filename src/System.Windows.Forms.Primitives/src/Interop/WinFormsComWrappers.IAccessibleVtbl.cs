// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Accessibility;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

internal partial class Interop
{
    internal unsafe partial class WinFormsComWrappers
    {
        internal static class IAccessibleVtbl
        {
            public static IntPtr Create(IntPtr fpQueryInterface, IntPtr fpAddRef, IntPtr fpRelease)
            {
                IntPtr* vtblRaw = (IntPtr*)RuntimeHelpers.AllocateTypeAssociatedMemory(typeof(IAccessibleVtbl), IntPtr.Size * 23);
                vtblRaw[0] = fpQueryInterface;
                vtblRaw[1] = fpAddRef;
                vtblRaw[2] = fpRelease;

                vtblRaw[3] = IntPtr.Zero;
                vtblRaw[4] = IntPtr.Zero;
                vtblRaw[5] = IntPtr.Zero;
                vtblRaw[6] = IntPtr.Zero;

                vtblRaw[7] = (IntPtr)(delegate* unmanaged<IntPtr, IntPtr*, HRESULT>)&get_accParent;
                vtblRaw[8] = (IntPtr)(delegate* unmanaged<IntPtr, int*, HRESULT>)&get_accChildCount;
                vtblRaw[9] = (IntPtr)(delegate* unmanaged<IntPtr, Oleaut32.VARIANT, IntPtr*, HRESULT>)&get_accChild;
                vtblRaw[10] = (IntPtr)(delegate* unmanaged<IntPtr, Oleaut32.VARIANT, IntPtr*, HRESULT>)&get_accName;
                vtblRaw[11] = (IntPtr)(delegate* unmanaged<IntPtr, Oleaut32.VARIANT, IntPtr*, HRESULT>)&get_accValue;
                vtblRaw[12] = (IntPtr)(delegate* unmanaged<IntPtr, Oleaut32.VARIANT, IntPtr*, HRESULT>)&get_accDescription;
                vtblRaw[13] = (IntPtr)(delegate* unmanaged<IntPtr, Oleaut32.VARIANT, IntPtr*, HRESULT>)&get_accRole;
                vtblRaw[14] = (IntPtr)(delegate* unmanaged<IntPtr, Oleaut32.VARIANT, IntPtr*, HRESULT>)&get_accState;
                vtblRaw[15] = (IntPtr)(delegate* unmanaged<IntPtr, Oleaut32.VARIANT, IntPtr*, HRESULT>)&get_accHelp;
                vtblRaw[16] = (IntPtr)(delegate* unmanaged<IntPtr, IntPtr*, Oleaut32.VARIANT, int*, HRESULT>)&get_accHelpTopic;
                vtblRaw[17] = (IntPtr)(delegate* unmanaged<IntPtr, Oleaut32.VARIANT, IntPtr*, HRESULT>)&get_accKeyboardShortcut;
                vtblRaw[18] = (IntPtr)(delegate* unmanaged<IntPtr, IntPtr*, HRESULT>)&get_accFocus;
                vtblRaw[19] = (IntPtr)(delegate* unmanaged<IntPtr, IntPtr*, HRESULT>)&get_accSelection;
                vtblRaw[20] = (IntPtr)(delegate* unmanaged<IntPtr, int, Oleaut32.VARIANT, HRESULT>)&accSelect;
                vtblRaw[21] = (IntPtr)(delegate* unmanaged<IntPtr, int*, int*, int*, int*, Oleaut32.VARIANT, HRESULT >)&accLocation;
                vtblRaw[22] = (IntPtr)(delegate* unmanaged<IntPtr, int, Oleaut32.VARIANT, IntPtr*, HRESULT>)&accNavigate;
                vtblRaw[23] = (IntPtr)(delegate* unmanaged<IntPtr, int, int, IntPtr*, HRESULT>)&accHitTest;
                vtblRaw[24] = (IntPtr)(delegate* unmanaged<IntPtr, Oleaut32.VARIANT, HRESULT>)&accDoDefaultAction;
                vtblRaw[25] = (IntPtr)(delegate* unmanaged<IntPtr, Oleaut32.VARIANT, IntPtr, HRESULT>)&set_accName;
                vtblRaw[26] = (IntPtr)(delegate* unmanaged<IntPtr, Oleaut32.VARIANT, IntPtr, HRESULT>)&set_accValue;

                return (IntPtr)vtblRaw;
            }

            [UnmanagedCallersOnly]
            private static HRESULT get_accParent(IntPtr thisPtr, IntPtr* pRetVal)
            {
                if (pRetVal is null)
                {
                    return HRESULT.E_INVALIDARG;
                }

                var instance = ComInterfaceDispatch.GetInstance<IAccessible>((ComInterfaceDispatch*)thisPtr);
                try
                {
                    object result = instance.accParent;
                    *pRetVal = result == null ? IntPtr.Zero : Marshal.GetIDispatchForObject(result);
                    return HRESULT.S_OK;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    return (HRESULT)ex.HResult;
                }
            }

            [UnmanagedCallersOnly]
            private static HRESULT get_accChildCount(IntPtr thisPtr, int* pcountChildren)
            {
                if (pcountChildren is null)
                {
                    return HRESULT.E_INVALIDARG;
                }

                var instance = ComInterfaceDispatch.GetInstance<IAccessible>((ComInterfaceDispatch*)thisPtr);
                try
                {
                    *pcountChildren = instance.accChildCount;
                    return HRESULT.S_OK;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    return (HRESULT)ex.HResult;
                }
            }

            [UnmanagedCallersOnly]
            private static HRESULT get_accChild(IntPtr thisPtr, Oleaut32.VARIANT varChild, IntPtr* ppdispChild)
            {
                if (ppdispChild is null)
                {
                    return HRESULT.E_INVALIDARG;
                }

                var instance = ComInterfaceDispatch.GetInstance<IAccessible>((ComInterfaceDispatch*)thisPtr);
                try
                {
                    object result = instance.accChild[varChild.ToObject()];
                    *ppdispChild = result == null ? IntPtr.Zero : Marshal.GetIDispatchForObject(result);
                    return HRESULT.S_OK;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    return (HRESULT)ex.HResult;
                }
            }

            [UnmanagedCallersOnly]
            private static HRESULT get_accName(IntPtr thisPtr, Oleaut32.VARIANT varChild, IntPtr* pszName)
            {
                if (pszName is null)
                {
                    return HRESULT.E_INVALIDARG;
                }

                var instance = ComInterfaceDispatch.GetInstance<IAccessible>((ComInterfaceDispatch*)thisPtr);
                try
                {
                    string result = instance.accName[varChild.ToObject()];
                    *pszName = Marshal.StringToBSTR(result);
                    return HRESULT.S_OK;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    return (HRESULT)ex.HResult;
                }
            }

            [UnmanagedCallersOnly]
            private static HRESULT get_accValue(IntPtr thisPtr, Oleaut32.VARIANT varChild, IntPtr* pszValue)
            {
                if (pszValue is null)
                {
                    return HRESULT.E_INVALIDARG;
                }

                var instance = ComInterfaceDispatch.GetInstance<IAccessible>((ComInterfaceDispatch*)thisPtr);
                try
                {
                    string result = instance.accValue[varChild.ToObject()];
                    *pszValue = Marshal.StringToBSTR(result);
                    return HRESULT.S_OK;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    return (HRESULT)ex.HResult;
                }
            }

            [UnmanagedCallersOnly]
            private static HRESULT get_accDescription(IntPtr thisPtr, Oleaut32.VARIANT varChild, IntPtr* pszDescription)
            {
                if (pszDescription is null)
                {
                    return HRESULT.E_INVALIDARG;
                }

                var instance = ComInterfaceDispatch.GetInstance<IAccessible>((ComInterfaceDispatch*)thisPtr);
                try
                {
                    string result = instance.accDescription[varChild.ToObject()];
                    *pszDescription = Marshal.StringToBSTR(result);
                    return HRESULT.S_OK;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    return (HRESULT)ex.HResult;
                }
            }

            [UnmanagedCallersOnly]
            private static HRESULT get_accRole(IntPtr thisPtr, Oleaut32.VARIANT varChild, IntPtr* pvarRole)
            {
                if (pvarRole is null)
                {
                    return HRESULT.E_INVALIDARG;
                }

                var instance = ComInterfaceDispatch.GetInstance<IAccessible>((ComInterfaceDispatch*)thisPtr);
                try
                {
                    object result = instance.accRole[varChild.ToObject()];
                    Marshal.GetNativeVariantForObject(result, (IntPtr)pvarRole);
                    return HRESULT.S_OK;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    return (HRESULT)ex.HResult;
                }
            }

            [UnmanagedCallersOnly]
            private static HRESULT get_accState(IntPtr thisPtr, Oleaut32.VARIANT varChild, IntPtr* pvarState)
            {
                if (pvarState is null)
                {
                    return HRESULT.E_INVALIDARG;
                }

                var instance = ComInterfaceDispatch.GetInstance<IAccessible>((ComInterfaceDispatch*)thisPtr);
                try
                {
                    object result = instance.accState[varChild.ToObject()];
                    Marshal.GetNativeVariantForObject(result, (IntPtr)pvarState);
                    return HRESULT.S_OK;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    return (HRESULT)ex.HResult;
                }
            }

            [UnmanagedCallersOnly]
            private static HRESULT get_accHelp(IntPtr thisPtr, Oleaut32.VARIANT varChild, IntPtr* pszHelp)
            {
                if (pszHelp is null)
                {
                    return HRESULT.E_INVALIDARG;
                }

                var instance = ComInterfaceDispatch.GetInstance<IAccessible>((ComInterfaceDispatch*)thisPtr);
                try
                {
                    string result = instance.accHelp[varChild.ToObject()];
                    *pszHelp = Marshal.StringToBSTR(result);
                    return HRESULT.S_OK;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    return (HRESULT)ex.HResult;
                }
            }

            [UnmanagedCallersOnly]
            private static HRESULT get_accHelpTopic(IntPtr thisPtr, IntPtr* pszHelpFile, Oleaut32.VARIANT varChild, int* pidTopic)
            {
                if (pidTopic is null || pszHelpFile is null)
                {
                    return HRESULT.E_INVALIDARG;
                }

                var instance = ComInterfaceDispatch.GetInstance<IAccessible>((ComInterfaceDispatch*)thisPtr);
                try
                {
                    *pidTopic = instance.accHelpTopic[out string helpFile, varChild.ToObject()];
                    *pszHelpFile = Marshal.StringToBSTR(helpFile);
                    return HRESULT.S_OK;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    return (HRESULT)ex.HResult;
                }
            }

            [UnmanagedCallersOnly]
            private static HRESULT get_accKeyboardShortcut(IntPtr thisPtr, Oleaut32.VARIANT varChild, IntPtr* pszKeyboardShortcut)
            {
                if (pszKeyboardShortcut is null)
                {
                    return HRESULT.E_INVALIDARG;
                }

                var instance = ComInterfaceDispatch.GetInstance<IAccessible>((ComInterfaceDispatch*)thisPtr);
                try
                {
                    string result = instance.accKeyboardShortcut[varChild.ToObject()];
                    *pszKeyboardShortcut = Marshal.StringToBSTR(result);
                    return HRESULT.S_OK;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    return (HRESULT)ex.HResult;
                }
            }

            [UnmanagedCallersOnly]
            private static HRESULT get_accFocus(IntPtr thisPtr, IntPtr* pvarChild)
            {
                if (pvarChild is null)
                {
                    return HRESULT.E_INVALIDARG;
                }

                var instance = ComInterfaceDispatch.GetInstance<IAccessible>((ComInterfaceDispatch*)thisPtr);
                try
                {
                    object result = instance.accFocus;
                    Marshal.GetNativeVariantForObject(result, (IntPtr)pvarChild);
                    return HRESULT.S_OK;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    return (HRESULT)ex.HResult;
                }
            }

            [UnmanagedCallersOnly]
            private static HRESULT get_accSelection(IntPtr thisPtr, IntPtr* pvarChildren)
            {
                if (pvarChildren is null)
                {
                    return HRESULT.E_INVALIDARG;
                }

                var instance = ComInterfaceDispatch.GetInstance<IAccessible>((ComInterfaceDispatch*)thisPtr);
                try
                {
                    object result = instance.accSelection;
                    Marshal.GetNativeVariantForObject(result, (IntPtr)pvarChildren);
                    return HRESULT.S_OK;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    return (HRESULT)ex.HResult;
                }
            }

            [UnmanagedCallersOnly]
            private static HRESULT accSelect(IntPtr thisPtr, int flagsSelect, Oleaut32.VARIANT varChild)
            {
                var instance = ComInterfaceDispatch.GetInstance<IAccessible>((ComInterfaceDispatch*)thisPtr);
                try
                {
                    instance.accSelect(flagsSelect, varChild.ToObject());
                    return HRESULT.S_OK;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    return (HRESULT)ex.HResult;
                }
            }

            [UnmanagedCallersOnly]
            private static HRESULT accLocation(IntPtr thisPtr, int* pxLeft, int* pyTop, int* pcxWidth, int* pcyHeight, Oleaut32.VARIANT varChild)
            {
                if (pxLeft is null || pyTop is null || pcxWidth is null || pcyHeight is null)
                {
                    return HRESULT.E_INVALIDARG;
                }

                var instance = ComInterfaceDispatch.GetInstance<IAccessible>((ComInterfaceDispatch*)thisPtr);
                try
                {
                    instance.accLocation(out *pxLeft, out *pyTop, out *pcxWidth, out *pcyHeight, varChild.ToObject());
                    return HRESULT.S_OK;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    return (HRESULT)ex.HResult;
                }
            }

            [UnmanagedCallersOnly]
            private static HRESULT accNavigate(IntPtr thisPtr, int navDir, Oleaut32.VARIANT varStart, IntPtr* pvarEndUpAt)
            {
                if (pvarEndUpAt is null)
                {
                    return HRESULT.E_INVALIDARG;
                }

                var instance = ComInterfaceDispatch.GetInstance<IAccessible>((ComInterfaceDispatch*)thisPtr);
                try
                {
                    object result = instance.accNavigate(navDir, varStart.ToObject());
                    Marshal.GetNativeVariantForObject(result, (IntPtr)pvarEndUpAt);
                    return HRESULT.S_OK;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    return (HRESULT)ex.HResult;
                }
            }

            [UnmanagedCallersOnly]
            private static HRESULT accHitTest(IntPtr thisPtr, int xLeft, int yTop, IntPtr* pvarChild)
            {
                if (pvarChild is null)
                {
                    return HRESULT.E_INVALIDARG;
                }

                var instance = ComInterfaceDispatch.GetInstance<IAccessible>((ComInterfaceDispatch*)thisPtr);
                try
                {
                    object result = instance.accHitTest(xLeft, yTop);
                    Marshal.GetNativeVariantForObject(result, (IntPtr)pvarChild);
                    return HRESULT.S_OK;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    return (HRESULT)ex.HResult;
                }
            }

            [UnmanagedCallersOnly]
            private static HRESULT accDoDefaultAction(IntPtr thisPtr, Oleaut32.VARIANT varChild)
            {
                var instance = ComInterfaceDispatch.GetInstance<IAccessible>((ComInterfaceDispatch*)thisPtr);
                try
                {
                    instance.accDoDefaultAction(varChild.ToObject());
                    return HRESULT.S_OK;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    return (HRESULT)ex.HResult;
                }
            }

            [UnmanagedCallersOnly]
            private static HRESULT set_accName(IntPtr thisPtr, Oleaut32.VARIANT varChild, IntPtr szName)
            {
                var instance = ComInterfaceDispatch.GetInstance<IAccessible>((ComInterfaceDispatch*)thisPtr);
                try
                {
                    string? name = szName == IntPtr.Zero ? null : Marshal.PtrToStringBSTR(szName);
                    instance.accName[varChild.ToObject()] = name;
                    return HRESULT.S_OK;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    return (HRESULT)ex.HResult;
                }
            }

            [UnmanagedCallersOnly]
            private static HRESULT set_accValue(IntPtr thisPtr, Oleaut32.VARIANT varChild, IntPtr szValue)
            {
                var instance = ComInterfaceDispatch.GetInstance<IAccessible>((ComInterfaceDispatch*)thisPtr);
                try
                {
                    string? value = szValue == IntPtr.Zero ? null : Marshal.PtrToStringBSTR(szValue);
                    instance.accValue[varChild.ToObject()] = value;
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
