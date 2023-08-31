// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System;

internal static class IComparerHelpers
{
    public static bool CompareReturnIfNull<T>([NotNullWhen(false)] T x, [NotNullWhen(false)] T y, [NotNullWhen(true)] out int? compareReturnValue)
    {
        if (x is not null && y is not null)
        {
            compareReturnValue = null;
            return false;
        }

        if (x is null && y is null)
        {
            compareReturnValue = 0;
            return true;
        }

        if (x is null)
        {
            compareReturnValue = -1;
            return true;
        }

        // y is null.
        compareReturnValue = 1;
        return true;
    }
}
