// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using System.Text;

internal static partial class Interop
{
    internal static partial class Shlwapi
    {
        [DllImport(Libraries.Shlwapi, ExactSpelling = true, CharSet = CharSet.Unicode)]
#pragma warning disable CA1838 // Avoid 'StringBuilder' parameters for P/Invokes
        public static extern HRESULT SHLoadIndirectString(string pszSource, StringBuilder pszOutBuf, uint cchOutBuf, IntPtr ppvReserved);
#pragma warning restore CA1838 // Avoid 'StringBuilder' parameters for P/Invokes
    }
}
