// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Formats.Nrbf;
using System.Private.Windows;
using System.Reflection.Metadata;
using System.Text.Json;

namespace System.Windows.Forms.Nrbf;

internal static class WinFormsSerializationRecordExtensions
{
    /// <summary>
    ///  Tries to get this object as a binary formatted <see cref="ImageListStreamer"/>.
    /// </summary>
    public static bool TryGetImageListStreamer(
        this SerializationRecord record,
        out object? imageListStreamer)
    {
        return SerializationRecordExtensions.TryGet(Get, record, out imageListStreamer);

        static bool Get(SerializationRecord record, [NotNullWhen(true)] out object? imageListStreamer)
        {
            imageListStreamer = null;

            if (record is not ClassRecord types
                || !types.TypeNameMatches(typeof(ImageListStreamer))
                || !types.HasMember("Data")
                || types.GetRawValue("Data") is not SZArrayRecord<byte> data)
            {
                return false;
            }

            imageListStreamer = new ImageListStreamer(data.GetArray());
            return true;
        }
    }

    /// <summary>
    ///  Tries to get this object as a binary formatted <see cref="Bitmap"/>.
    /// </summary>
    public static bool TryGetBitmap(this SerializationRecord record, out object? bitmap)
    {
        bitmap = null;

        if (record is not ClassRecord types
            || !types.TypeNameMatches(typeof(Bitmap))
            || !types.HasMember("Data")
            || types.GetRawValue("Data") is not SZArrayRecord<byte> data)
        {
            return false;
        }

        bitmap = new Bitmap(new MemoryStream(data.GetArray()));
        return true;
    }

    /// <summary>
    ///  Tries to deserialize this object if it was serialized as JSON.
    /// </summary>
    public static bool TryGetObjectFromJson(this SerializationRecord record, out object? @object)
    {
        @object = null;

        if (record.TypeName.AssemblyName?.FullName != IJsonData.CustomAssemblyName)
        {
            // The data was not serialized as JSON.
            return false;
        }

        if (record is not ClassRecord types
            || types.GetRawValue("<JsonBytes>k__BackingField") is not SZArrayRecord<byte> byteData
            || types.GetRawValue("<OriginalAssemblyQualifiedTypeName>k__BackingField") is not string typeData
            || !TypeName.TryParse(typeData, out TypeName? result)
            || Type.GetType(result.AssemblyQualifiedName) is not Type originalType)
        {
            // This is supposed to be JsonData, but somehow the binary formatted data is corrupt.
            throw new InvalidOperationException();
        }

        // TODO: If the full name of the type user is asking for doesn't match the type that is saved, return false.

        // TODO: We should get the type from the Func<TypeName, Type> that will be passed down instead of using Type.GetType()
        @object = JsonSerializer.Deserialize(byteData.GetArray(), originalType);

        return true;
    }

    /// <summary>
    ///  Try to get a supported object. This supports common types used in WinForms that do not have type converters.
    /// </summary>
    public static bool TryGetResXObject(this SerializationRecord record, [NotNullWhen(true)] out object? value) =>
        record.TryGetFrameworkObject(out value)
        || record.TryGetBitmap(out value)
        || record.TryGetImageListStreamer(out value);

    public static bool TryGetCommonObject(this SerializationRecord record, [NotNullWhen(true)] out object? value) =>
        record.TryGetResXObject(out value)
        || record.TryGetDrawingPrimitivesObject(out value)
        || record.TryGetObjectFromJson(out value);
}
