// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Windows.Forms;

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
///         cfFormat = (short)DataFormats.GetFormat("yourDataFormat").Id,
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
///         || types.GetRawValue("<InnerTypeAssemblyQualifiedName>k__BackingField") is not string innerTypeAssemblyQualifiedName
///         || !System.Reflection.Metadata.TypeName.TryParse(innerTypeAssemblyQualifiedName.ToCharArray(), out System.Reflection.Metadata.TypeName? innerTypeName))
///     {
///         // This is supposed to be JsonData, but somehow the data is corrupt.
///         throw new InvalidOperationException();
///     }
///
///     // TODO: Additional checking on 'innerTypeName' to ensure it is expected type.
///
///     // This should return the original data that was JSON serialized.
///     var result = System.Text.Json.JsonSerializer.Deserialize(byteData.GetArray(), innerType);
///     // TODO: Process the payload as needed.
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
///  // This definition must live in an assembly named System.Private.Windows.VirtualJson and referenced in order to work as expected.
///  namespace System.Private.Windows;
///  [Serializable]
///  struct JsonData : IObjectReference
///  {
///     public byte[] JsonBytes { get; set; }
///
///     public string InnerTypeAssemblyQualifiedName { get; set; }
///
///     public object GetRealObject(StreamingContext context)
///     {
///         // TODO: Additional checking on InnerTypeAssemblyQualifiedName to ensure it is expected type.
///         return JsonSerializer.Deserialize(JsonBytes, typeof(ExpectedType)) ?? throw new InvalidOperationException();
///     }
///  }
///  ]]>
/// </example>
[Serializable]
internal struct JsonData<T> : IJsonData
{
    public byte[] JsonBytes { get; set; }

    public readonly string InnerTypeAssemblyQualifiedName => typeof(T).ToTypeName().AssemblyQualifiedName;

    public readonly object Deserialize()
    {
        object? result;
        try
        {
            result = JsonSerializer.Deserialize<T>(JsonBytes);
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
    const string CustomAssemblyName = "System.Private.Windows.VirtualJson";

    byte[] JsonBytes { get; set; }

    /// <summary>
    ///  The assembly qualified name of the T in <see cref="JsonData{T}"/>. This name should
    ///  have any <see cref="TypeForwardedFromAttribute"/> names taken into account.
    /// </summary>
    string InnerTypeAssemblyQualifiedName { get; }

    /// <summary>
    ///  Deserializes the data stored in the JsonData. This is a convenience method
    ///  to deserialize the data when we are not dealing with a binary formatted record.
    /// </summary>
    object Deserialize();
}
