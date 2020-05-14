// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal partial class Interop
{
    internal static partial class Oleaut32
    {
        [ComImport]
        [Guid("55272A00-42CB-11CE-8135-00AA004BB851")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IPropertyBag
        {
            [PreserveSig]
            HRESULT Read(
                [MarshalAs(UnmanagedType.LPWStr)] string pszPropName,
                ref object pVar,
                IErrorLog pErrorLog);

            [PreserveSig]
            HRESULT Write(
                [MarshalAs(UnmanagedType.LPWStr)] string pszPropName,
                ref object pVar);
        }
    }
}
