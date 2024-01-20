// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Security;

namespace System.Drawing;

internal static class ClientUtils
{
    // ExecutionEngineException is obsolete and shouldn't be used (to catch, throw or reference) anymore.
    // Pragma added to prevent converting the "type is obsolete" warning into build error.
#pragma warning disable 618
    public static bool IsCriticalException(Exception ex)
    {
        return ex is NullReferenceException
                or StackOverflowException
                or OutOfMemoryException
                or Threading.ThreadAbortException
                or ExecutionEngineException
                or IndexOutOfRangeException
                or AccessViolationException;
    }
#pragma warning restore 618

    public static bool IsSecurityOrCriticalException(Exception ex)
    {
        return (ex is SecurityException) || IsCriticalException(ex);
    }
}
