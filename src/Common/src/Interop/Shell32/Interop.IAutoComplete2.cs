// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

internal static partial class Interop
{
    internal static partial class Shell32
    {
        [ComImport]
        [Guid("EAC04BC0-3791-11d2-BB95-0060977B464C")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public unsafe interface IAutoComplete2
        {
            [PreserveSig]
            HRESULT Init(
                IntPtr hwndEdit,
                IEnumString punkACL,
                [MarshalAs(UnmanagedType.LPWStr)] string pwszRegKeyPath,
                [MarshalAs(UnmanagedType.LPWStr)] string pwszQuickComplete);

            [PreserveSig]
            HRESULT Enable(
                BOOL fEnable);

            [PreserveSig]
            HRESULT SetOptions(
                AUTOCOMPLETEOPTIONS dwFlag);

            [PreserveSig]
            HRESULT GetOptions(
                AUTOCOMPLETEOPTIONS* pdwFlag);
        }
    }
}
