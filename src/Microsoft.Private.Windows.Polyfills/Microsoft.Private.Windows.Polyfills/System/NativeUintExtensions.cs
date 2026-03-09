// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System;

internal static class NativeUIntExtensions
{
    extension(nuint)
    {
        // These analyzers aren't correct in this case.
#pragma warning disable IDE0049 // Simplify Names
#pragma warning disable SA1121 // Use built-in type alias
        /// <summary>Represents the smallest possible value of a native sized unsigned integer.</summary>
        public static nuint MinValue => UIntPtr.Size == 4
            ? unchecked((nuint)(UInt32.MinValue))
            : unchecked((nuint)(UInt64.MinValue));

        /// <summary>Represents the largest possible value of a native sized unsigned integer.</summary>
        public static nuint MaxValue => UIntPtr.Size == 4
            ? unchecked((nuint)(UInt32.MaxValue))
            : unchecked((nuint)(UInt64.MaxValue));
#pragma warning restore SA1121
#pragma warning restore IDE0049
    }
}
