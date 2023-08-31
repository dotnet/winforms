// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.CompilerServices;

namespace System.Windows.Forms;

internal static class ArgumentValidation
{
    internal static T OrThrowIfNull<T>([NotNull] this T? argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
    {
        ArgumentNullException.ThrowIfNull(argument, paramName);
        return argument;
    }

    internal static T OrThrowIfNullWithMessage<T>([NotNull] this T? argument, string message, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
    {
        if (argument is null)
        {
            throw new ArgumentNullException(paramName, message);
        }

        return argument;
    }

    internal static IntPtr OrThrowIfZero(this IntPtr argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
    {
        if (argument == IntPtr.Zero)
        {
            throw new ArgumentNullException(paramName);
        }

        return argument;
    }

    internal static string OrThrowIfNullOrEmpty([NotNull] this string? argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
    {
        ThrowIfNullOrEmpty(argument, paramName);
        return argument;
    }

    internal static void ThrowIfNullOrEmpty([NotNull] this string? argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
    {
        if (string.IsNullOrEmpty(argument))
        {
            throw new ArgumentNullException(paramName);
        }
    }

    internal static void ThrowIfNullOrEmptyWithMessage([NotNull] this string? argument, string message, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
    {
        if (string.IsNullOrEmpty(argument))
        {
            throw new ArgumentNullException(paramName, message);
        }
    }

    internal static void ThrowIfNull(HDC argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
    {
        if (argument.IsNull)
        {
            throw new ArgumentNullException(paramName);
        }
    }
}
