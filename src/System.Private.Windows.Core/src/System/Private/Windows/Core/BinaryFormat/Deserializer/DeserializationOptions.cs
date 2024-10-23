// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;

namespace System.Private.Windows.Core.BinaryFormat;

internal sealed class DeserializationOptions
{
    /// <summary>
    ///  How exactly assembly names need to match for deserialization.
    /// </summary>
#pragma warning disable SYSLIB0050 // Type or member is obsolete
    public FormatterAssemblyStyle AssemblyMatching { get; set; } = FormatterAssemblyStyle.Simple;
#pragma warning restore SYSLIB0050 // Type or member is obsolete

    /// <summary>
    ///  Type name binder.
    /// </summary>
    public SerializationBinder? Binder { get; set; }

    /// <summary>
    ///  Optional type <see cref="ISerializationSurrogate"/> provider.
    /// </summary>
#pragma warning disable SYSLIB0050 // Type or member is obsolete
    public ISurrogateSelector? SurrogateSelector { get; set; }
#pragma warning restore SYSLIB0050 // Type or member is obsolete

    /// <summary>
    ///  Streaming context.
    /// </summary>
#pragma warning disable SYSLIB0050 // Type or member is obsolete
    public StreamingContext StreamingContext { get; set; } = new(StreamingContextStates.All);
#pragma warning restore SYSLIB0050 // Type or member is obsolete
}
