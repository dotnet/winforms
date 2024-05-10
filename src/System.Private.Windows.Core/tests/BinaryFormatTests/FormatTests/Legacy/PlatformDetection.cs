// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System;

public static partial class PlatformDetectionMe
{
    // Drawing is not supported on non windows platforms in .NET 7.0+. It also isn't supported on Windows Nano Server and Core skus.
    public static bool IsDrawingSupported => IsWindows;

    private static volatile Tuple<bool>? s_lazyNonZeroLowerBoundArraySupported;

    public static bool IsNonZeroLowerBoundArraySupported
    {
        get
        {
            if (s_lazyNonZeroLowerBoundArraySupported is null)
            {
                bool nonZeroLowerBoundArraysSupported = false;
                try
                {
                    Array.CreateInstance(typeof(int), [5], [5]);
                    nonZeroLowerBoundArraysSupported = true;
                }
                catch (PlatformNotSupportedException)
                {
                }

                s_lazyNonZeroLowerBoundArraySupported = Tuple.Create(nonZeroLowerBoundArraysSupported);
            }

            return s_lazyNonZeroLowerBoundArraySupported.Item1;
        }
    }
}
