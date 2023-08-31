// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.CodeDom;
using System.Runtime.Serialization;

namespace System.ComponentModel.Design.Serialization;

/// <summary>
///  The exception that is thrown when the code dom serializer experiences an error.
/// </summary>
public class CodeDomSerializerException : SystemException
{
    public CodeDomSerializerException(string? message, CodeLinePragma? linePragma) : base(message)
    {
        LinePragma = linePragma;
    }

    public CodeDomSerializerException(Exception? ex, CodeLinePragma? linePragma) : base(ex?.Message, ex)
    {
        LinePragma = linePragma;
    }

    public CodeDomSerializerException(string? message, IDesignerSerializationManager manager) : base(message)
    {
        ArgumentNullException.ThrowIfNull(manager);
    }

    public CodeDomSerializerException(Exception? ex, IDesignerSerializationManager manager) : base(ex?.Message, ex)
    {
        ArgumentNullException.ThrowIfNull(manager);
    }

    /// <summary>
    ///  Gets the line pragma object that is related to this error.
    /// </summary>
    public CodeLinePragma? LinePragma { get; }

    [Obsolete(DiagnosticId = "SYSLIB0051")]
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        throw new PlatformNotSupportedException();
    }
}
