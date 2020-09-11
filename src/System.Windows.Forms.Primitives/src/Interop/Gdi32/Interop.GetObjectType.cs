// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class Gdi32
    {
        // This does set last error but *only* in one error case that doesn't add any real value.
        [DllImport(Libraries.Gdi32, ExactSpelling = true)]
        public static extern OBJ GetObjectType(HGDIOBJ h);

        public static OBJ GetObjectType(HandleRef h)
        {
            OBJ result = GetObjectType((HGDIOBJ)h.Handle);
            GC.KeepAlive(h.Wrapper);
            return result;
        }
    }
}
