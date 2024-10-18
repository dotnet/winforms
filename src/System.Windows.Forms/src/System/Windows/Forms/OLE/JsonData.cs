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
///  struct ReplicatedJsonData : IObjectReference
///  {
///     public byte[] JsonBytes { get; set; }
///
///     public string OriginalAssemblyQualifiedTypeName { get; set; }
///
///     // For deserializing with BinaryFormatter only. This interface is not needed if using NrbfDecoder to help deserialize.
///     public readonly object GetRealObject(StreamingContext context)
///     {
///         // TODO: Additional checking on OriginalAssemblyQualifiedTypeName to block unwanted types if needed.
///         return JsonSerializer.Deserialize(JsonBytes, Type.GetType(OriginalAssemblyQualifiedTypeName)) ?? throw new InvalidOperationException();
///     }
///  }
///
///  // For deserializing with BinaryFormatter only.
///  class ReplicatedJsonDataBinder : SerializationBinder
///  {
///     public override Type? BindToType(string assemblyName, string typeName)
///     {
///         // The assembly name for JsonData should always be "System.Private.Windows.VirtualJson"
///         if (assemblyName == "System.Private.Windows.VirtualJson"
///             && TypeName.TryParse(typeName, out TypeName? name)
///             && name.Name == "JsonData")
///         {
///             return typeof(ReplicatedJsonData);
///         }
///
///         // TODO: Rejection behavior
///     }
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
///     // Use Nrbf to decode stream and rehydrate data. (recommended)
///     System.Formats.Nrbf.SerializationRecord record = NrbfDecoder.Decode(stream);
///     if (record.TypeName.AssemblyName.FullName != "System.Private.Windows.VirtualJson")
///     {
///         // The data was not serialized as JSON.
///         return false;
///     }
///
///     if (record is not System.Formats.Nrbf.ClassRecord types
///         || types.GetRawValue("<JsonBytes>k__BackingField") is not SZArrayRecord<byte> byteData
///         || types.GetRawValue("<OriginalAssemblyQualifiedTypeName>k__BackingField") is not string typeData
///         || !TypeName.TryParse(typeData, out TypeName? result)
///         || Type.GetType(result.AssemblyQualifiedName) is not Type originalType)
///     {
///         // This is supposed to be JsonData, but somehow the data is corrupt.
///         throw new InvalidOperationException();
///     }
///
///     // This should return the original data that was JSON serialized.
///     System.Text.Json.JsonSerializer.Deserialize(byteData.GetArray(), originalType);
///
///      // OR
///      // Use BinaryFormatter to rehydrate the data.
///
///      // This should return the original data that was JSON serialized.
///      BinaryFormatter binaryFormatter = new() { Binder = new ReplicatedJsonDataBinder() };
///      binaryFormatter.Deserialize(stream);
///  }
///  ]]>
/// </example>
[Serializable]
internal struct JsonData : IJsonData
{
    public byte[] JsonBytes { get; set; }

    public string OriginalAssemblyQualifiedTypeName { get; set; }

    public readonly object Deserialize() => JsonSerializer.Deserialize(JsonBytes, Type.GetType(OriginalAssemblyQualifiedTypeName)!) ?? throw new InvalidOperationException();
}

/// <summary>
///  Represents an object that contains JSON serialized data. This interface is used to
///  identify a <see cref="JsonData"/> without needing to have the generic type information.
/// </summary>
internal interface IJsonData
{
    // We use a custom assembly name to allow versions where JsonData<T> doesn't exist to still be able rehydrate JSON serialized data.
    public const string CustomAssemblyName = "System.Private.Windows.VirtualJson";

    byte[] JsonBytes { get; set; }

    public string OriginalAssemblyQualifiedTypeName { get; set; }

    object Deserialize();
}
