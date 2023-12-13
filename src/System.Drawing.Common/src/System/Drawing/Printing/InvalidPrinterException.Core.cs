// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.Serialization;

namespace System.Drawing.Printing;

[Runtime.CompilerServices.TypeForwardedFrom(AssemblyRef.SystemDrawing)]
public partial class InvalidPrinterException
{
#if NET8_0_OR_GREATER
    [Obsolete(DiagnosticId = "SYSLIB0051")]
#endif
    protected InvalidPrinterException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
        // Ignoring not serializable input
    }

#if NET8_0_OR_GREATER
    [Obsolete(DiagnosticId = "SYSLIB0051")]
#endif
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
        info.AddValue("settings", null);
    }
}
