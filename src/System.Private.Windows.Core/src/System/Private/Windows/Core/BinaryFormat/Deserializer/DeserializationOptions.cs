// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.Serialization;

namespace System.Private.Windows.Core.BinaryFormat;

internal sealed class DeserializationOptions
{
    /// <summary>
    ///  How exactly assembly names need to match for deserialization.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   Aligned with <see href="https://learn.microsoft.com/dotnet/api/system.runtime.serialization.formatters.formatterassemblystyle">
    ///   FormatterAssemblyStyle</see> behavior.
    ///  </para>
    /// </remarks>
    public bool SimpleAssemblyMatching { get; set; } = true;

    /// <summary>
    ///  Type name binder.
    /// </summary>
    public ITypeResolver? TypeResolver { get; set; }

    /// <summary>
    ///  Optional type <see cref="ISerializationSurrogate"/> provider.
    /// </summary>
#pragma warning disable SYSLIB0050 // Type or member is obsolete
    public ISurrogateSelector? SurrogateSelector { get; set; }
#pragma warning restore SYSLIB0050

    /// <summary>
    ///  Streaming context.
    /// </summary>
#pragma warning disable SYSLIB0050 // Type or member is obsolete
    public StreamingContext StreamingContext { get; set; } = new(StreamingContextStates.All);
#pragma warning restore SYSLIB0050
}
