// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using static Interop.Oleaut32;

internal partial class Interop
{
    internal static partial class Ole32
    {
        [DllImport(Libraries.Ole32)]
        public static extern unsafe HRESULT PropVariantClear(VARIANT* pvarg);

        public static unsafe HRESULT PropVariantClear(ref VARIANT varg)
        {
            fixed (VARIANT* pvarg = &varg)
            {
                return PropVariantClear(pvarg);
            }
        }
    }
}
