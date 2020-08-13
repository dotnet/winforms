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
        [Guid("3127CA40-446E-11CE-8135-00AA004BB851")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public unsafe interface IErrorLog
        {
            [PreserveSig]
            HRESULT AddError(
                [MarshalAs(UnmanagedType.LPWStr)] string pszPropName,
                EXCEPINFO* pExcepInfo);
        }
    }
}
