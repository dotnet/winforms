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
///   There may be instances where this type is not available in different versions, e.g. .NET 8, .NET Framework.
///   If this type needs to be deserialized from stream in these instances, a workaround would be to create an assembly with the name <see cref="IJsonData.CustomAssemblyName"/>
///   and replicate this type. Then, manually retrieve the serialized stream and use the <see cref="Formats.Nrbf.NrbfDecoder"/> to decode the stream and rehydrate the serialized type.
///   Alternatively, but not recommended, BinaryFormatter can also be used to deserialize the stream if this type is not available.
///  </para>
/// </remarks>
/// <example>
/// <![CDATA[
///  // Recommended: deserialize using NrbfDecoder.
///  void DeserializeJsonData(DataObject dataObject)
///  {
///     // Manually retrieve serialized stream.
///     System.Runtime.InteropServices.ComTypes.FORMATETC formatetc = new()
///     {
///         cfFormat = (short) DataFormats.GetFormat("yourDataFormat").Id,
///         dwAspect = System.Runtime.InteropServices.ComTypes.DVASPECT.DVASPECT_CONTENT,
///         lindex = -1,
///         tymed = System.Runtime.InteropServices.ComTypes.TYMED.TYMED_HGLOBAL
///     };
///
///     System.Runtime.InteropServices.ComTypes.IDataObject castedDataObject = (System.Runtime.InteropServices.ComTypes.IDataObject)dataObject;
///     castedDataObject.GetData(ref formatetc, out System.Runtime.InteropServices.ComTypes.STGMEDIUM medium);
///     IntPtr hglobal = medium.unionmember;
///     Stream stream;
///     try
///     {
///         IntPtr buffer = GlobalLock(hglobal);
///         int size = GlobalSize(hglobal);
///         byte[] bytes = new byte[size];
///         Marshal.Copy(buffer, bytes, 0, size);
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
///     System.Formats.Nrbf.SerializationRecord record = System.Formats.Nrbf.NrbfDecoder.Decode(stream);
///     if (record.TypeName.AssemblyName.FullName != "System.Private.Windows.VirtualJson")
///     {
///         // The data was not serialized as JSON.
///         return;
///     }
///
///     if (record is not System.Formats.Nrbf.ClassRecord types
///         || types.GetRawValue("<JsonBytes>k__BackingField") is not System.Formats.Nrbf.SZArrayRecord<byte> byteData
///         || !System.Reflection.Metadata.TypeName.TryParse(types.TypeName.FullName.ToCharArray(), out System.Reflection.Metadata.TypeName? result))
///     {
///         // This is supposed to be JsonData, but somehow the data is corrupt.
///         throw new InvalidOperationException();
///     }
///
///     System.Reflection.Metadata.TypeName genericTypeName = result.GetGenericArguments().Single();
///     // TODO: Additional checking on generic type to block unwanted types if needed.
///
///     // This should return the original data that was JSON serialized.
///     System.Text.Json.JsonSerializer.Deserialize(byteData.GetArray(), genericType);
///  }
///
///  [DllImport("kernel32.dll")]
///  static extern int GlobalSize(IntPtr hMem);
///
///  [DllImport("kernel32.dll")]
///  static extern IntPtr GlobalLock(IntPtr hMem);
///
///  [DllImport("kernel32.dll")]
///  static extern int GlobalUnlock(IntPtr hMem);
///
///  // OR
///  // Not recommended: deserialize using BinaryFormatter.
///
///  // This definition must live in an assembly named System.Private.Windows.VirtualJson in order to work as expected.
///  namespace System.Private.Windows;
///  [Serializable]
///  struct JsonData<T> : IObjectReference
///  {
///     public byte[] JsonBytes { get; set; }
///
///     public object GetRealObject(StreamingContext context)
///     {
///         // TODO: Additional checking on generic type to block unwanted types if needed.
///         return JsonSerializer.Deserialize(JsonBytes, typeof(T)) ?? throw new InvalidOperationException();
///     }
///  }
///  ]]>
/// </example>
[Serializable]
internal struct JsonData<T> : IJsonData
{
    public byte[] JsonBytes { get; set; }

    public readonly string TypeFullName => $"{typeof(JsonData<T>).FullName}";

    public readonly object Deserialize()
    {
        object? result = null;
        try
        {
            result = JsonSerializer.Deserialize(JsonBytes, typeof(T));
        }
        catch (Exception ex)
        {
            result = new NotSupportedException(ex.Message);
        }

        return result ?? throw new InvalidOperationException();
    }
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
