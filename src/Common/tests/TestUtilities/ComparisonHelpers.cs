﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Numerics;

namespace System;

public static class ComparisonHelpers
{
    public static bool EqualsInteger<T>(T x, T y, T variance)
        where T : struct, IBinaryInteger<T>
    {
        if (variance < T.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(variance));
        }

        return T.Abs(x > y ? x - y : y - x) <= variance;
    }

    public static bool EqualsFloating<T>(T x, T y, T variance)
        where T : struct, IFloatingPoint<T>
    {
        if (variance < T.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(variance));
        }

        if (T.IsNaN(x))
        {
            return T.IsNaN(y);
        }
        else if (T.IsNaN(y))
        {
            return false;
        }

        if (T.IsNegativeInfinity(x))
        {
            return T.IsNegativeInfinity(y);
        }
        else if (T.IsNegativeInfinity(y))
        {
            return false;
        }

        if (T.IsPositiveInfinity(x))
        {
            return T.IsPositiveInfinity(y);
        }
        else if (T.IsPositiveInfinity(y))
        {
            return false;
        }

        if (IsNegativeZero(x))
        {
            if (IsNegativeZero(y))
            {
                return true;
            }

            if (IsPositiveZero(variance) || IsNegativeZero(variance))
            {
                return false;
            }

            // When the variance is not +-0.0, then we are handling a case where
            // the actual result is expected to not be exactly -0.0 on some platforms
            // and we should fallback to checking if it is within the allowed variance instead.
        }
        else if (IsNegativeZero(y))
        {
            if (IsPositiveZero(variance) || IsNegativeZero(variance))
            {
                return false;
            }

            // When the variance is not +-0.0, then we are handling a case where
            // the actual result is expected to not be exactly -0.0 on some platforms
            // and we should fallback to checking if it is within the allowed variance instead.
        }

        if (IsPositiveZero(x))
        {
            if (IsPositiveZero(y))
            {
                return true;
            }

            if (IsPositiveZero(variance) || IsNegativeZero(variance))
            {
                return false;
            }

            // When the variance is not +-0.0, then we are handling a case where
            // the actual result is expected to not be exactly +0.0 on some platforms
            // and we should fallback to checking if it is within the allowed variance instead.
        }
        else if (IsPositiveZero(y))
        {
            if (IsPositiveZero(variance) || IsNegativeZero(variance))
            {
                return false;
            }

            // When the variance is not +-0.0, then we are handling a case where
            // the actual result is expected to not be exactly +0.0 on some platforms
            // and we should fallback to checking if it is within the allowed variance instead.
        }

        return T.Abs(x > y ? x - y : y - x) <= variance;

        static unsafe bool IsNegativeZero(T value) => T.IsZero(value) && T.IsNegative(value);

        static unsafe bool IsPositiveZero(T value) => T.IsZero(value) && T.IsPositive(value);
    }
}
