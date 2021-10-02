// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;

namespace System.Windows.Forms.Design
{
    internal static class ArgumentValidation
    {
        internal static T OrThrowIfNull<T>(this T argument, [CallerArgumentExpression("argument")] string? paramName = null)
        {
            ArgumentNullException.ThrowIfNull(argument, paramName);
            return argument;
        }
    }
}
