// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Reflection.Metadata;

namespace System.Private.Windows.Ole;

/// <summary>
///  Encapsulates state of a data request.
/// </summary>
internal readonly struct DataRequest
{
    [SetsRequiredMembers]
    public DataRequest(string format)
    {
        Format = format;
        TypedRequest = true;
    }

    public required string Format { get; init; }

    public bool AutoConvert { get; init; } = true;

    /// <summary>
    ///  Provides the list of custom allowed types that user considers safe to deserialize from the payload.
    ///  Resolver should recognize the closure of all non-primitive and not known types in the payload,
    ///  such as field types and types in the inheritance hierarchy and the code to match these types to the
    ///  <see cref="TypeName"/>s read from the deserialized stream.
    /// </summary>
    public Func<TypeName, Type?>? Resolver { get; init; } = null;

    /// <summary>
    ///  <see langword="false"/> if the user had not requested any specific type, i.e. the call originates from
    ///  <see cref="IDataObjectInternal.GetData(string)"/> API family, that returns an <see cref="object"/>. <see langword="true"/>
    ///  if the user had requested a specific type by calling <see cref="IDataObjectInternal.TryGetData{T}(out T)"/> API family.
    /// </summary>
    public required bool TypedRequest { get; init; }
}
