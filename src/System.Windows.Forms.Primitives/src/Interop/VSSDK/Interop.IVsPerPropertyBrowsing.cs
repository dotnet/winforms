// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal partial class Interop
{
    internal static partial class VSSDK
    {
        [ComImport]
        [Guid("0FF510A3-5FA5-49F1-8CCC-190D71083F3E")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public unsafe interface IVsPerPropertyBrowsing
        {
            /// <summary>
            ///  Hides the property at the given dispid from the properties window
            ///  implmentors should can return E_NOTIMPL to show all properties that
            ///  are otherwise browsable.
            /// </summary>
            [PreserveSig]
            HRESULT HideProperty(
                Ole32.DispatchID dispid,
                BOOL* pfHide);

            /// <summary>
            ///  Will have the "+" expandable glyph next to them and can be expanded or collapsed by the user
            ///  Returning a non-S_OK return code or false for pfDisplay will suppress this feature
            /// </summary>
            [PreserveSig]
            HRESULT DisplayChildProperties(
                Ole32.DispatchID dispid,
                BOOL* pfDisplay);

            /// <summary>
            ///  Retrieves the localized name and description for a property.
            ///  returning a non-S_OK return code will display the default values
            /// </summary>
            [PreserveSig]
            HRESULT GetLocalizedPropertyInfo(
                Ole32.DispatchID dispid,
                Kernel32.LCID localeID,
                [Out, MarshalAs(UnmanagedType.LPArray)] string[] pbstrLocalizedName,
                [Out, MarshalAs(UnmanagedType.LPArray)] string[] pbstrLocalizeDescription);

            /// <summary>
            ///  Determines if the given (usually current) value for a property is the default.  If it is not default,
            ///  the property will be shown as bold in the browser to indcate that it has been modified from the default.
            /// </summary>
            [PreserveSig]
            HRESULT HasDefaultValue(
                Ole32.DispatchID dispid,
                BOOL* fDefault);

            /// <summary>
            ///  Determines if a property should be made read only.  This only applies to properties that are writeable,
            /// </summary>
            [PreserveSig]
            HRESULT IsPropertyReadOnly(
                Ole32.DispatchID dispid,
                BOOL* fReadOnly);

            /// <summary>
            ///  Returns the classname for this object. The class name is the non-bolded text
            ///  that appears in the properties window selection combo.  If this method returns
            ///  a non-S_OK return code, the default will be used. The default is the name
            ///  string from a call to ITypeInfo::GetDocumentation(MEMID_NIL, ...);
            /// </summary>
            [PreserveSig]
            HRESULT GetClassName(
                ref string pbstrClassName);

            /// <summary>
            ///  Checks whether the given property can be reset to some default value.
            ///  If return value is non-S_OK or *pfCanReset is
            /// </summary>
            [PreserveSig]
            HRESULT CanResetPropertyValue(
                Ole32.DispatchID dispid,
                BOOL* pfCanReset);

            /// <summary>
            ///  If the return value is S_OK, the property's value will then be refreshed to the
            ///  new default values.
            /// </summary>
            [PreserveSig]
            HRESULT ResetPropertyValue(
                Ole32.DispatchID dispid);
        }
    }
}
