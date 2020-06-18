// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using Accessibility;

internal static partial class Interop
{
    internal static partial class UiaCore
    {
        [ComImport]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [Guid("F8B80ADA-2C44-48D0-89BE-5FF23C9CD875")]
        public unsafe interface IAccessibleEx
        {
            /// <summary>
            ///  Returns the IAccessibleEx for specified child. Returns
            ///  S_OK/NULL if this implementation does not use child ids,
            ///  or does not have an IAccessibleEx for the specified child,
            ///  or already represents a child element.
            ///  idChild must be normalized; ie. client must have previously
            ///  used get_accChild to check whether it actually has its own
            ///  IAccessible. Only idChild values that do not have a corresponding
            ///  IAccessible can be used here.
            /// </summary>
            IAccessibleEx? GetObjectForChild(int idChild);

            /// <summary>
            ///  Returns an IAccessible and idChild pair for this IAccessibleEx.
            ///  Implementation must return fully normalized idChild values: ie.
            ///  it is not required to call get_accChild on the resulting pair.
            ///
            ///  For IAccessible implementations that do not use child ids, this
            ///  just returns the corresponding IAccessible and CHILDID_SELF.
            /// </summary>
            [PreserveSig]
            HRESULT GetIAccessiblePair(
                [MarshalAs(UnmanagedType.Interface)] out object? ppAcc,
                int* pidChild);

            [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_I4)]
            int[]? GetRuntimeId();

            /// <summary>
            ///  Some wrapper-based implementations (notably UIABridge) can't reasonably wrap all
            ///  IRawElementProviderSimple elements returned as property values or patterns, so
            ///  these elements won't QI to IAccessibleEx. Where this is the case, the original
            ///  IAccessibleEx that the property was retreived from must implement this method
            ///  so that the client can get an IAccessibleEx.
            ///
            ///  Usage for a client is as follows:
            ///  When an IRawElementProviderSimple is obtained as a property value,
            ///  - first try to QI to IAccessibleEx
            ///  - if that fails, call this method on the source IAccessibleEx
            /// </summary>
            [PreserveSig]
            HRESULT ConvertReturnedElement(
                IRawElementProviderSimple pIn,
                out IAccessibleEx? ppRetValOut);
        }
    }
}
