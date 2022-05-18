// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using System.Windows.Forms;
using Accessibility;

internal partial class Interop
{
    internal unsafe partial class WinFormsComWrappers
    {
        internal class StandardAccessibleWrapper : IAccessible, Ole32.IOleWindow, Oleaut32.IEnumVariant
        {
            private IntPtr _accessibleInstance;
            private IntPtr _enumVariantComObject;
            private IntPtr _oleWindowComObject;

            public StandardAccessibleWrapper(IntPtr wrappedInstance)
            {
                _accessibleInstance = wrappedInstance.OrThrowIfZero();

                Guid enumVariantIID = IID.IEnumVariant;
                var hr = (HRESULT)Marshal.QueryInterface(wrappedInstance, ref enumVariantIID, out IntPtr enumVariantComObject);
                hr.ThrowIfFailed();
                _enumVariantComObject = enumVariantComObject;

                Guid oleWindowIID = IID.IOleWindow;
                hr = (HRESULT)Marshal.QueryInterface(wrappedInstance, ref oleWindowIID, out IntPtr oleWindowComObject);
                hr.ThrowIfFailed();
                _oleWindowComObject = oleWindowComObject;
            }

            ~StandardAccessibleWrapper()
            {
                Marshal.Release(_accessibleInstance);
                _accessibleInstance = IntPtr.Zero;
                Marshal.Release(_enumVariantComObject);
                _enumVariantComObject = IntPtr.Zero;
                Marshal.Release(_oleWindowComObject);
                _oleWindowComObject = IntPtr.Zero;
            }

            HRESULT Ole32.IOleWindow.GetWindow(IntPtr* phwnd)
            {
                return ((delegate* unmanaged<IntPtr, IntPtr*, HRESULT>)(*(*(void***)_oleWindowComObject + 3)))
                    (_oleWindowComObject, phwnd);
            }

            HRESULT Ole32.IOleWindow.ContextSensitiveHelp(BOOL fEnterMode)
            {
                return ((delegate* unmanaged<IntPtr, BOOL, HRESULT>)(*(*(void***)_oleWindowComObject + 4)))
                    (_oleWindowComObject, fEnterMode);
            }

            /// <summary>
            ///  Return the parent object
            /// </summary>
            object? IAccessible.accParent
            {
                get
                {
                    IntPtr value = IntPtr.Zero;
                    ((delegate* unmanaged<IntPtr, IntPtr*, HRESULT>)(*(*(void***)_accessibleInstance + 7)))
                        (_accessibleInstance, &value).ThrowIfFailed();
                    return value == IntPtr.Zero ? null : Marshal.GetObjectForIUnknown(value);
                }
            }

            /// <summary>
            ///  Return the number of children
            /// </summary>
            int IAccessible.accChildCount
            {
                get
                {
                    int result;
                    ((delegate* unmanaged<IntPtr, int*, HRESULT>)(*(*(void***)_accessibleInstance + 8)))
                        (_accessibleInstance, &result).ThrowIfFailed();
                    return result;
                }
            }

            /// <summary>
            ///  Returns a child Accessible object
            /// </summary>
            object? IAccessible.get_accChild(object childID)
            {
                IntPtr value = IntPtr.Zero;
                using Oleaut32.VARIANT childIdVar = new Oleaut32.VARIANT();
                Marshal.GetNativeVariantForObject(childID, (IntPtr)(void*)&childIdVar);
                ((delegate* unmanaged<IntPtr, Oleaut32.VARIANT, IntPtr *, HRESULT>)(*(*(void***)_accessibleInstance + 9)))
                    (_accessibleInstance, childIdVar, &value).ThrowIfFailed();
                return value == IntPtr.Zero ? null : Marshal.GetObjectForIUnknown(value);
            }

            /// <summary>
            ///  Return the object or child name
            /// </summary>
            string? IAccessible.get_accName(object childID)
            {
                IntPtr value = IntPtr.Zero;
                try
                {
                    using Oleaut32.VARIANT childIdVar = new Oleaut32.VARIANT();
                    Marshal.GetNativeVariantForObject(childID, (IntPtr)(void*)&childIdVar);
                    ((delegate* unmanaged<IntPtr, Oleaut32.VARIANT, IntPtr*, HRESULT>)(*(*(void***)_accessibleInstance + 10)))
                        (_accessibleInstance, childIdVar, &value).ThrowIfFailed();
                    if (value == IntPtr.Zero)
                    {
                        return null;
                    }

                    return Marshal.PtrToStringBSTR(value);
                }
                finally
                {
                    Marshal.FreeBSTR(value);
                }
            }

            /// <summary>
            ///  Return the object or child value
            /// </summary>
            string? IAccessible.get_accValue(object childID)
            {
                IntPtr value = IntPtr.Zero;
                try
                {
                    using Oleaut32.VARIANT childIdVar = new Oleaut32.VARIANT();
                    Marshal.GetNativeVariantForObject(childID, (IntPtr)(void*)&childIdVar);
                    ((delegate* unmanaged<IntPtr, Oleaut32.VARIANT, IntPtr*, HRESULT>)(*(*(void***)_accessibleInstance + 11)))
                        (_accessibleInstance, childIdVar, &value).ThrowIfFailed();
                    if (value == IntPtr.Zero)
                    {
                        return null;
                    }

                    return Marshal.PtrToStringBSTR(value);
                }
                finally
                {
                    Marshal.FreeBSTR(value);
                }
            }

            /// <summary>
            ///  Return the object or child description
            /// </summary>
            string? IAccessible.get_accDescription(object childID)
            {
                IntPtr value = IntPtr.Zero;
                try
                {
                    using Oleaut32.VARIANT childIdVar = new Oleaut32.VARIANT();
                    Marshal.GetNativeVariantForObject(childID, (IntPtr)(void*)&childIdVar);
                    ((delegate* unmanaged<IntPtr, Oleaut32.VARIANT, IntPtr*, HRESULT>)(*(*(void***)_accessibleInstance + 12)))
                        (_accessibleInstance, childIdVar, &value).ThrowIfFailed();
                    if (value == IntPtr.Zero)
                    {
                        return null;
                    }

                    return Marshal.PtrToStringBSTR(value);
                }
                finally
                {
                    Marshal.FreeBSTR(value);
                }
            }

            /// <summary>
            ///  The role property describes an object's purpose in terms of its
            ///  relationship with sibling or child objects.
            /// </summary>
            object? IAccessible.get_accRole(object childID)
            {
                using Oleaut32.VARIANT value = new Oleaut32.VARIANT();
                using Oleaut32.VARIANT childIdVar = new Oleaut32.VARIANT();
                Marshal.GetNativeVariantForObject(childID, (IntPtr)(void*)&childIdVar);
                ((delegate* unmanaged<IntPtr, Oleaut32.VARIANT, Oleaut32.VARIANT*, HRESULT>)(*(*(void***)_accessibleInstance + 13)))
                    (_accessibleInstance, childIdVar, &value).ThrowIfFailed();
                var result = value.ToObject();
                return result;
            }

            /// <summary>
            ///  Return the object or child state
            /// </summary>
            object? IAccessible.get_accState(object childID)
            {
                using Oleaut32.VARIANT value = new Oleaut32.VARIANT();
                using Oleaut32.VARIANT childIdVar = new Oleaut32.VARIANT();
                Marshal.GetNativeVariantForObject(childID, (IntPtr)(void*)&childIdVar);
                ((delegate* unmanaged<IntPtr, Oleaut32.VARIANT, Oleaut32.VARIANT*, HRESULT>)(*(*(void***)_accessibleInstance + 14)))
                    (_accessibleInstance, childIdVar, &value).ThrowIfFailed();
                var result = value.ToObject();
                return result;
            }

            /// <summary>
            ///  Return help for this accessible object.
            /// </summary>
            string? IAccessible.get_accHelp(object childID)
            {
                IntPtr value = IntPtr.Zero;
                try
                {
                    using Oleaut32.VARIANT childIdVar = new Oleaut32.VARIANT();
                    Marshal.GetNativeVariantForObject(childID, (IntPtr)(void*)&childIdVar);
                    ((delegate* unmanaged<IntPtr, Oleaut32.VARIANT, IntPtr*, HRESULT>)(*(*(void***)_accessibleInstance + 15)))
                        (_accessibleInstance, childIdVar, &value).ThrowIfFailed();
                    if (value == IntPtr.Zero)
                    {
                        return null;
                    }

                    return Marshal.PtrToStringBSTR(value);
                }
                finally
                {
                    Marshal.FreeBSTR(value);
                }
            }

            /// <summary>
            ///  Return the object or child help topic
            /// </summary>
            int IAccessible.get_accHelpTopic(out string? pszHelpFile, object childID)
            {
                IntPtr value = IntPtr.Zero;
                try
                {
                    int result;
                    using Oleaut32.VARIANT childIdVar = new Oleaut32.VARIANT();
                    Marshal.GetNativeVariantForObject(childID, (IntPtr)(void*)&childIdVar);
                    ((delegate* unmanaged<IntPtr, IntPtr*, Oleaut32.VARIANT, int*, HRESULT>)(*(*(void***)_accessibleInstance + 16)))
                        (_accessibleInstance, &value, childIdVar, &result).ThrowIfFailed();
                    if (value == IntPtr.Zero)
                    {
                        pszHelpFile = null;
                        return result;
                    }

                    pszHelpFile = Marshal.PtrToStringBSTR(value);
                    return result;
                }
                finally
                {
                    Marshal.FreeBSTR(value);
                }
            }

            /// <summary>
            ///  Return the object or child keyboard shortcut
            /// </summary>
            string? IAccessible.get_accKeyboardShortcut(object childID)
            {
                IntPtr value = IntPtr.Zero;
                try
                {
                    using Oleaut32.VARIANT childIdVar = new Oleaut32.VARIANT();
                    Marshal.GetNativeVariantForObject(childID, (IntPtr)(void*)&childIdVar);
                    ((delegate* unmanaged<IntPtr, Oleaut32.VARIANT, IntPtr*, HRESULT>)(*(*(void***)_accessibleInstance + 17)))
                        (_accessibleInstance, childIdVar, &value).ThrowIfFailed();
                    if (value == IntPtr.Zero)
                    {
                        return null;
                    }

                    return Marshal.PtrToStringBSTR(value);
                }
                finally
                {
                    Marshal.FreeBSTR(value);
                }
            }

            /// <summary>
            ///  Return the object or child focus
            /// </summary>
            object? IAccessible.accFocus
            {
                get
                {
                    using Oleaut32.VARIANT value = new Oleaut32.VARIANT();
                    ((delegate* unmanaged<IntPtr, Oleaut32.VARIANT*, HRESULT>)(*(*(void***)_accessibleInstance + 18)))
                        (_accessibleInstance, &value).ThrowIfFailed();
                    return value.ToObject();
                }
            }

            /// <summary>
            ///  Return the object or child selection
            /// </summary>
            object? IAccessible.accSelection
            {
                get
                {
                    using Oleaut32.VARIANT value = new Oleaut32.VARIANT();
                    ((delegate* unmanaged<IntPtr, Oleaut32.VARIANT*, HRESULT>)(*(*(void***)_accessibleInstance + 19)))
                        (_accessibleInstance, &value).ThrowIfFailed();
                    return value.ToObject();
                }
            }

            /// <summary>
            ///  Return the default action
            /// </summary>
            string? IAccessible.get_accDefaultAction(object childID)
            {
                IntPtr value = IntPtr.Zero;
                try
                {
                    using Oleaut32.VARIANT childIdVar = new Oleaut32.VARIANT();
                    Marshal.GetNativeVariantForObject(childID, (IntPtr)(void*)&childIdVar);
                    ((delegate* unmanaged<IntPtr, Oleaut32.VARIANT, IntPtr*, HRESULT>)(*(*(void***)_accessibleInstance + 20)))
                        (_accessibleInstance, childIdVar, &value).ThrowIfFailed();
                    if (value == IntPtr.Zero)
                    {
                        return null;
                    }

                    return Marshal.PtrToStringBSTR(value);
                }
                finally
                {
                    Marshal.FreeBSTR(value);
                }
            }

            /// <summary>
            ///  Select an accessible object.
            /// </summary>
            void IAccessible.accSelect(int flagsSelect, object childID)
            {
                using Oleaut32.VARIANT childIdVar = new Oleaut32.VARIANT();
                Marshal.GetNativeVariantForObject(childID, (IntPtr)(void*)&childIdVar);
                ((delegate* unmanaged<IntPtr, int, Oleaut32.VARIANT, HRESULT>)(*(*(void***)_accessibleInstance + 21)))
                    (_accessibleInstance, flagsSelect, childIdVar).ThrowIfFailed();
            }

            /// <summary>
            ///  The location of the Accessible object
            /// </summary>
            void IAccessible.accLocation(
                out int pxLeft,
                out int pyTop,
                out int pcxWidth,
                out int pcyHeight,
                object childID)
            {
                using Oleaut32.VARIANT childIdVar = new Oleaut32.VARIANT();
                Marshal.GetNativeVariantForObject(childID, (IntPtr)(void*)&childIdVar);
                fixed (int* pxLeftPtr = &pxLeft)
                fixed (int* pyTopPtr = &pyTop)
                fixed (int* pcxWidthPtr = &pcxWidth)
                fixed (int* pcyHeightPtr = &pcyHeight)
                {
                    ((delegate* unmanaged<IntPtr, int*, int*, int*, int*, Oleaut32.VARIANT, HRESULT>)(*(*(void***)_accessibleInstance + 22)))
                        (_accessibleInstance, pxLeftPtr, pyTopPtr, pcxWidthPtr, pcyHeightPtr, childIdVar).ThrowIfFailed();
                }
            }

            /// <summary>
            ///  Navigate to another accessible object.
            /// </summary>
            object? IAccessible.accNavigate(int navDir, object childID)
            {
                using Oleaut32.VARIANT value = new Oleaut32.VARIANT();
                using Oleaut32.VARIANT childIdVar = new Oleaut32.VARIANT();
                Marshal.GetNativeVariantForObject(childID, (IntPtr)(void*)&childIdVar);
                ((delegate* unmanaged<IntPtr, int, Oleaut32.VARIANT, Oleaut32.VARIANT*, HRESULT>)(*(*(void***)_accessibleInstance + 23)))
                    (_accessibleInstance, navDir, childIdVar, &value).ThrowIfFailed();
                return value.ToObject();
            }

            /// <summary>
            ///  Perform a hit test
            /// </summary>
            object? IAccessible.accHitTest(int xLeft, int yTop)
            {
                using Oleaut32.VARIANT value = new Oleaut32.VARIANT();
                ((delegate* unmanaged<IntPtr, int, int, Oleaut32.VARIANT*, HRESULT>)(*(*(void***)_accessibleInstance + 24)))
                    (_accessibleInstance, xLeft, yTop, &value).ThrowIfFailed();
                return value.ToObject();
            }

            /// <summary>
            ///  Perform the default action
            /// </summary>
            void IAccessible.accDoDefaultAction(object childID)
            {
                using Oleaut32.VARIANT childIdVar = new Oleaut32.VARIANT();
                Marshal.GetNativeVariantForObject(childID, (IntPtr)(void*)&childIdVar);
                ((delegate* unmanaged<IntPtr, Oleaut32.VARIANT, HRESULT>)(*(*(void***)_accessibleInstance + 25)))
                    (_accessibleInstance, childIdVar).ThrowIfFailed();
            }

            /// <summary>
            ///  Set the object or child name
            /// </summary>
            void IAccessible.set_accName(object childID, string newName)
            {
                using Oleaut32.VARIANT childIdVar = new Oleaut32.VARIANT();
                Marshal.GetNativeVariantForObject(childID, (IntPtr)(void*)&childIdVar);
                var newNamePtr = Marshal.StringToBSTR(newName);
                ((delegate* unmanaged<IntPtr, Oleaut32.VARIANT, IntPtr, HRESULT>)(*(*(void***)_accessibleInstance + 26)))
                    (_accessibleInstance, childIdVar, newNamePtr).ThrowIfFailed();
            }

            /// <summary>
            ///  Set the object or child value
            /// </summary>
            void IAccessible.set_accValue(object childID, string newValue)
            {
                using Oleaut32.VARIANT childIdVar = new Oleaut32.VARIANT();
                Marshal.GetNativeVariantForObject(childID, (IntPtr)(void*)&childIdVar);
                var newValuePtr = Marshal.StringToBSTR(newValue);
                ((delegate* unmanaged<IntPtr, Oleaut32.VARIANT, IntPtr, HRESULT>)(*(*(void***)_accessibleInstance + 27)))
                    (_accessibleInstance, childIdVar, newValuePtr).ThrowIfFailed();
            }

            HRESULT Oleaut32.IEnumVariant.Next(uint celt, IntPtr rgVar, uint* pCeltFetched)
            {
                return ((delegate* unmanaged<IntPtr, uint, IntPtr, uint*, HRESULT>)(*(*(void***)_enumVariantComObject + 3)))
                    (_enumVariantComObject, celt, rgVar, pCeltFetched);
            }

            HRESULT Oleaut32.IEnumVariant.Skip(uint celt)
            {
                return ((delegate* unmanaged<IntPtr, uint, HRESULT>)(*(*(void***)_enumVariantComObject + 4)))
                    (_enumVariantComObject, celt);
            }

            HRESULT Oleaut32.IEnumVariant.Reset()
            {
                return ((delegate* unmanaged<IntPtr, HRESULT>)(*(*(void***)_enumVariantComObject + 5)))
                    (_enumVariantComObject);
            }

            HRESULT Oleaut32.IEnumVariant.Clone(Oleaut32.IEnumVariant[]? ppEnum)
            {
                if (ppEnum == null || ppEnum.Length == 0)
                {
                    return HRESULT.E_POINTER;
                }

                IntPtr resultPtr;
                var result = ((delegate* unmanaged<IntPtr, IntPtr*, HRESULT>)(*(*(void***)_enumVariantComObject + 6)))
                    (_enumVariantComObject, &resultPtr);
                if (result.Failed())
                {
                    ppEnum = null;
                    return result;
                }

                ppEnum[0] = (Oleaut32.IEnumVariant)WinFormsComWrappers.Instance
                    .GetOrCreateObjectForComInstance(resultPtr, CreateObjectFlags.Unwrap);
                return result;
            }
        }
    }
}
