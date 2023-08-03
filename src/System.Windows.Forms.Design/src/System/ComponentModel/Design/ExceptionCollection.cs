// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.Runtime.Serialization;

namespace System.ComponentModel.Design;

public sealed class ExceptionCollection : Exception
{
    private readonly List<Exception>? _exceptions;

    public ExceptionCollection(ArrayList? exceptions)
    {
        if (exceptions is null)
        {
            return;
        }

        if (exceptions.ToArray().Any(e => e is not Exception))
        {
            throw new ArgumentException(string.Format(SR.ExceptionCollectionInvalidArgument, nameof(Exception)), nameof(exceptions));
        }

        _exceptions = exceptions?.Cast<Exception>().ToList();
    }

    internal ExceptionCollection(List<Exception>? exceptions)
    {
        _exceptions = exceptions;
    }

    public ArrayList? Exceptions => _exceptions is null ? null : new ArrayList(_exceptions);

    [Obsolete(DiagnosticId = "SYSLIB0051")]
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        throw new PlatformNotSupportedException();
    }
}
