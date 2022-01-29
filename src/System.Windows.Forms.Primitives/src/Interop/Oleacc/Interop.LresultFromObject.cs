// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class Oleacc
    {
        [DllImport(Libraries.Oleacc, ExactSpelling = true)]
        public static extern nint LresultFromObject(in Guid refiid, nint wParam, IntPtr pAcc);

        public static nint LresultFromObject(in Guid refiid, nint wParam, HandleRef pAcc)
        {
            nint result = LresultFromObject(in refiid, wParam, pAcc.Handle);
            GC.KeepAlive(pAcc.Wrapper);
            return result;
        }
    }
}
