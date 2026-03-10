// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System;

internal static class NativeIntExtensions
{
    extension(nint)
    {
        // These analyzers aren't correct in this case.
#pragma warning disable IDE0049 // Simplify Names
#pragma warning disable SA1121 // Use built-in type alias
        /// <summary>Represents the smallest possible value of a native sized signed integer.</summary>
        public static nint MinValue => IntPtr.Size == 4
            ? unchecked((nint)(Int32.MinValue))
            : unchecked((nint)(Int64.MinValue));

        /// <summary>Represents the largest possible value of a native sized signed integer.</summary>
        public static nint MaxValue => IntPtr.Size == 4
            ? unchecked((nint)(Int32.MaxValue))
            : unchecked((nint)(Int64.MaxValue));
#pragma warning restore SA1121
#pragma warning restore IDE0049
    }
}
