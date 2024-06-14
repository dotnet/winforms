// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Text.Json;

namespace System.Private.Windows;

/// <summary>
///  Wrapper which contains JSON serialized data along with the JSON data's original type information
///  to be deserialized later.
/// </summary>
/// <remarks>
///  <para>
///   There may be instances where this type is not available in different versions, e.g. .NET 8.
///   If this type needs to be deserialized from stream in these instances, a workaround would be to replicate this type and implement
///   <see cref="Runtime.Serialization.IObjectReference"/>. Then, use the deserializer to deserialize the JSON data with a binder that will
///   reroute JsonData's custom assembly name to the replicated type.
///  </para>
/// </remarks>
/// <example>
/// <![CDATA[
///  [Serializable]
///  struct ReplicatedJsonData<T> : IObjectReference
///  {
///     public byte[] JsonBytes { get; set; }
///
///     public readonly object GetRealObject(StreamingContext context) =>
///         JsonSerializer.Deserialize(JsonBytes, typeof(T)) ?? throw new InvalidOperationException();
///  }
///
///  class ReplicatedJsonDataBinder : SerializationBinder
///  {
///     public override Type? BindToType(string assemblyName, string typeName)
///     {
///         // The assembly name for JsonData<T> should always be "System.Private.Windows.VirtualJson"
///         if (assemblyName == "System.Private.Windows.VirtualJson" && TypeName.TryParse(typeName, out TypeName? name))
///         {
///             // TODO: Additional checking for the generic type to block unwanted types if needed.
///             return typeof(ReplicatedJsonData<T>);
///         }
///
///         // TODO: Rejection behavior
///     }
///  }
///
///  void DeserializeJsonData(Stream stream)
///  {
///      BinaryFormattedObject binary = new(stream, new BinaryFormattedObject.Options { Binder = new ReplicatedJsonDataBinder() });
///      // This should return the original data that was JSON serialized.
///      object? deserialized = binary.Deserialize();
///  }
///  ]]>
/// </example>
[Serializable]
internal struct JsonData<T> : IJsonData
{
    public byte[] JsonBytes { get; set; }

    public readonly string TypeFullName => $"{typeof(JsonData<T>).FullName}";

    public readonly object Deserialize() => JsonSerializer.Deserialize(JsonBytes, typeof(T)) ?? throw new InvalidOperationException();
}

/// <summary>
///  Represents an object that contains JSON serialized data. This interface is used to
///  identify a <see cref="JsonData{T}"/> without needing to have the generic type information.
/// </summary>
internal interface IJsonData
{
    // We use a custom assembly name to allow versions where JsonData<T> doesn't exist to still be able rehydrate JSON serialized data.
    public const string CustomAssemblyName = "System.Private.Windows.VirtualJson";

    byte[] JsonBytes { get; set; }

    string TypeFullName { get; }

    object Deserialize();
}
