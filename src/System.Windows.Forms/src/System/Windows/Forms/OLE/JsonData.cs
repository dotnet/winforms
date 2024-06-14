// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.Serialization;
using System.Text.Json;

namespace System.Private.Windows;

/// <summary>
///  Wrapper which contains JSON serialized data along with the JSON data's original type information
///  to be deserialized later.
/// </summary>
[Serializable]
#pragma warning disable SYSLIB0050 // Type or member is obsolete
internal struct JsonData<T> : IObjectReference, IJsonData
#pragma warning restore SYSLIB0050
{
    public byte[] JsonBytes { get; set; }

    public readonly string TypeFullName => $"{typeof(JsonData<T>).FullName}";

    public readonly object GetRealObject(StreamingContext context) =>
        JsonSerializer.Deserialize(JsonBytes, typeof(T)) ?? throw new InvalidOperationException();
}

/// <summary>
///  Represents an object that contains JSON serialized data. This interface is used to
///  identify a <see cref="JsonData{T}"/> without needing to have the generic type information.
/// </summary>
internal interface IJsonData
{
    byte[] JsonBytes { get; set; }

    string TypeFullName { get; }
}
