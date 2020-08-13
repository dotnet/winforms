// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class Oleacc
    {
        [DllImport(Libraries.Oleacc, ExactSpelling = true)]
        public static extern IntPtr LresultFromObject(ref Guid refiid, IntPtr wParam, IntPtr pAcc);

        public static IntPtr LresultFromObject(ref Guid refiid, IntPtr wParam, HandleRef pAcc)
        {
            IntPtr result = LresultFromObject(ref refiid, wParam, pAcc.Handle);
            GC.KeepAlive(pAcc.Wrapper);
            return result;
        }
    }
}
