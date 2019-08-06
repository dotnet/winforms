// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class Gdi32
    {
        [DllImport(ExternDll.Gdi32, ExactSpelling = true)]
        public static extern ObjectType GetObjectType(IntPtr h);

        public static ObjectType GetObjectType(HandleRef h)
        {
            ObjectType result = GetObjectType(h.Handle);
            GC.KeepAlive(h.Wrapper);
            return result;
        }
    }
}
