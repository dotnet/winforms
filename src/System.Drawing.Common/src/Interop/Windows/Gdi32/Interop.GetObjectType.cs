// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class Gdi32
    {
#if NET7_0_OR_GREATER
        [LibraryImport(Libraries.Gdi32)]
        public static partial ObjectType GetObjectType(
#else
        [DllImport(Libraries.Gdi32, ExactSpelling = true)]
        public static extern ObjectType GetObjectType(
#endif
            IntPtr h);
    }
}
