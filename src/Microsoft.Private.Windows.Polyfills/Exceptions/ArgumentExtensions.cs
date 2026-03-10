// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.Private.Windows.Polyfills.Resources;

namespace System;

internal static class ArgumentExtensions
{
    extension(ArgumentException)
    {
        // .NET Framework analyzer is confused by the use of [NotNull] on the parameter of these methods, and thinks
        // that the parameter must be non-null when exiting the method, which is not the case as these methods throw
        // when the parameter is null. Suppress the warning for these methods.

#pragma warning disable CS8777 // Parameter must have a non-null value when exiting.

        /// <summary>Throws an exception if <paramref name="argument"/> is null or empty.</summary>
        /// <param name="argument">The string argument to validate as non-null and non-empty.</param>
        /// <param name="paramName">The name of the parameter with which <paramref name="argument"/> corresponds.</param>
        /// <exception cref="ArgumentNullException"><paramref name="argument"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="argument"/> is empty.</exception>
        public static void ThrowIfNullOrEmpty([NotNull] string? argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
        {
            if (string.IsNullOrEmpty(argument))
            {
                ThrowNullOrEmptyException(argument, paramName);
            }
        }

        /// <summary>Throws an exception if <paramref name="argument"/> is null, empty, or consists only of white-space characters.</summary>
        /// <param name="argument">The string argument to validate.</param>
        /// <param name="paramName">The name of the parameter with which <paramref name="argument"/> corresponds.</param>
        /// <exception cref="ArgumentNullException"><paramref name="argument"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="argument"/> is empty or consists only of white-space characters.</exception>
        public static void ThrowIfNullOrWhiteSpace([NotNull] string? argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
        {
            if (string.IsNullOrWhiteSpace(argument))
            {
                ThrowNullOrWhiteSpaceException(argument, paramName);
            }
        }

#pragma warning restore CS8777 // Parameter must have a non-null value when exiting.
    }

#pragma warning disable IDE0051 // Remove unused private members
    // .NET Framework analyzers don't understand that these methods are only called from the extension methods above,
    // so it thinks it's unused. Suppress that warning since these methods are actually used.

    [DoesNotReturn]
    private static void ThrowNullOrEmptyException(string? argument, string? paramName)
    {
        ArgumentNullException.ThrowIfNull(argument, paramName);
        throw new ArgumentException(SRF.Argument_EmptyString, paramName);
    }

    [DoesNotReturn]
    private static void ThrowNullOrWhiteSpaceException(string? argument, string? paramName)
    {
        ArgumentNullException.ThrowIfNull(argument, paramName);
        throw new ArgumentException(SRF.Argument_EmptyOrWhiteSpaceString, paramName);
    }
#pragma warning restore IDE0051
}
