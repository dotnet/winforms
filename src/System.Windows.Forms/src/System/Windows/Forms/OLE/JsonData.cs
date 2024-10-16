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
///   If this type needs to be deserialized from stream in these instances, a workaround would be to create an assembly with the name <see cref="IJsonData.CustomAssemblyName"/>
///   and replicate this type. Then, manually retrieve the serialized stream and use the <see cref="Formats.Nrbf.NrbfDecoder"/> to decode the stream and rehydrate the serialized type.
///   Alternatively, but not recommended, BinaryFormatter can also be used to deserialize the stream if this type is not available.
///  </para>
/// </remarks>
/// <example>
/// <![CDATA[
///  [Serializable]
///  struct ReplicatedJsonData<T> : IObjectReference
///  {
///     public byte[] JsonBytes { get; set; }
///
///     // For deserializing with BinaryFormatter only. This interface is not needed if using NrbfDecoder to help deserialize.
///     public readonly object GetRealObject(StreamingContext context) =>
///         JsonSerializer.Deserialize(JsonBytes, typeof(T)) ?? throw new InvalidOperationException();
///  }
///
///  // For deserializing with BinaryFormatter only.
///  public Type ResolveType(TypeName typeName)
///  {
///     // The assembly name for JsonData<T> should always be "System.Private.Windows.VirtualJson"
///     if (name.AssemblyName == "System.Private.Windows.VirtualJson")
///     {
///         // TODO: Additional checking for the generic type to block unwanted types if needed.
///         return typeof(ReplicatedJsonData<T>);
///     }
///
///     // TODO: Rejection behavior
///  }
///
///
///  void DeserializeJsonData(DataObject dataObject)
///  {
///     // Manually retrieve serialized stream.
///     System.Runtime.InteropServices.ComTypes.FORMATETC formatetc = new()
///     {
///         cfFormat = (short) DataFormats.GetFormat("testFormat").Id,
///         dwAspect = System.Runtime.InteropServices.ComTypes.DVASPECT.DVASPECT_CONTENT,
///         lindex = -1,
///         tymed = System.Runtime.InteropServices.ComTypes.TYMED.TYMED_HGLOBAL
///     };
///
///     dataObject.GetData(ref formatetc, out System.Runtime.InteropServices.ComTypes.STGMEDIUM medium);
///     HGLOBAL hglobal = (HGLOBAL)medium.unionmember;
///     Stream stream;
///     try
///     {
///         void* buffer = GlobalLock(hglobal);
///         int size = GlobalSize(hglobal);
///         byte[] bytes = new byte[size];
///         Marshal.Copy((nint)buffer, bytes, 0, size);
///         // this comes from DataObject.Composition.s_serializedObjectID
///         int index = 16;
///         stream = new MemoryStream(bytes, index, bytes.Length - index);
///     }
///     finally
///     {
///         GlobalUnlock(hglobal);
///     }
///
///     // Use Nrbf to decode stream and rehydrate data.
///     System.Formats.Nrbf.SerializationRecord record = NrbfDecoder.Decode(stream);
///     if (record.TypeName.AssemblyName.FullName != "System.Private.Windows.VirtualJson")
///     {
///         // The data was not serialized as JSON.
///         return false;
///     }
///
///     if (record is not System.Formats.Nrbf.ClassRecord types
///         || !types.HasMember("<JsonBytes>k__BackingField")
///         || types.GetRawValue("<JsonBytes>k__BackingField") is not System.Formats.Nrbf.SZArrayRecord<byte> byteData
///         || !TypeName.TryParse(types.TypeName.FullName, out TypeName? result)
///         || Type.GetType(result.GetGenericArguments()[0].AssemblyQualifiedName) is not Type genericType)
///     {
///         // This is supposed to be JsonData, but somehow the data is corrupt.
///         throw new InvalidOperationException();
///     }
///
///     // This should return the original data that was JSON serialized.
///     System.Text.Json.JsonSerializer.Deserialize(byteData.GetArray(), genericType);
///
///      // OR
///
///      // This should return the original data that was JSON serialized.
///      dataObject.TryGetData<T>(format, ResolveType, out T data);
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
