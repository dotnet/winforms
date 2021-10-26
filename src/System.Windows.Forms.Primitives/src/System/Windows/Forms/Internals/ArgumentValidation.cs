// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;

namespace System.Windows.Forms
{
    internal static class ArgumentValidation
    {
        internal static T OrThrowIfNull<T>(this T? argument, [CallerArgumentExpression("argument")] string? paramName = null)
        {
            ArgumentNullException.ThrowIfNull(argument, paramName);
            return argument;
        }

        internal static T OrThrowIfNullWithMessage<T>(this T? argument, string message, [CallerArgumentExpression("argument")] string? paramName = null)
        {
            if (argument == null)
            {
                throw new ArgumentNullException(paramName, message);
            }

            return argument!;
        }

        internal static IntPtr OrThrowIfZero(this IntPtr argument, [CallerArgumentExpression("argument")] string? paramName = null)
        {
            if (argument == IntPtr.Zero)
            {
                throw new ArgumentNullException(paramName);
            }

            return argument;
        }

        internal static string OrThrowIfNullOrEmpty(this string? argument, [CallerArgumentExpression("argument")] string? paramName = null)
        {
            ThrowIfNullOrEmpty(argument, paramName);
            return argument!;
        }

        internal static void ThrowIfNullOrEmpty(this string? argument, [CallerArgumentExpression("argument")] string? paramName = null)
        {
            if (string.IsNullOrEmpty(argument))
            {
                throw new ArgumentNullException(paramName);
            }
        }

        internal static void ThrowIfNullOrEmptyWithMessage(this string? argument, string message, [CallerArgumentExpression("argument")] string? paramName = null)
        {
            if (string.IsNullOrEmpty(argument))
            {
                throw new ArgumentNullException(paramName, message);
            }
        }

        internal static void ThrowIfNull(Interop.Gdi32.HDC argument, [CallerArgumentExpression("argument")] string? paramName = null)
        {
            if (argument.IsNull)
            {
                throw new ArgumentNullException(paramName);
            }
        }
    }
}
