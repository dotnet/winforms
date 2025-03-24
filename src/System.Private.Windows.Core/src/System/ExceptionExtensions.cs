// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Runtime.Serialization;

namespace System;

internal static class ExceptionExtensions
{
    /// <summary>
    ///  Returns <see langword="true"/> if the exception is an exception that isn't recoverable and/or a likely
    ///  bug in our implementation.
    /// </summary>
    internal static bool IsCriticalException(this Exception ex)
        => ex is NullReferenceException
            or StackOverflowException
            or OutOfMemoryException
            or ThreadAbortException
            or IndexOutOfRangeException
            or AccessViolationException;

    /// <summary>
    ///  Converts the given exception to a <see cref="SerializationException"/> if needed, nesting the original exception
    ///  and assigning the original stack trace.
    /// </summary>
    internal static SerializationException ConvertToSerializationException(this Exception ex)
    {
        if (ex is TargetInvocationException targetException)
        {
            ex = targetException.InnerException ?? ex;
        }

        return ex is SerializationException serializationException
            ? serializationException
            : (SerializationException)ExceptionDispatchInfo.SetRemoteStackTrace(
                new SerializationException(ex.Message, ex),
                ex.StackTrace ?? string.Empty);
    }
}
