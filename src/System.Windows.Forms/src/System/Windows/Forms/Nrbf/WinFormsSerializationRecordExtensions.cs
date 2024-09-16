// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Formats.Nrbf;

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
    ///  Try to get a supported object. This supports common types used in WinForms that do not have type converters.
    /// </summary>
    public static bool TryGetResXObject(this SerializationRecord record, [NotNullWhen(true)] out object? value) =>
        record.TryGetFrameworkObject(out value)
        || record.TryGetBitmap(out value)
        || record.TryGetImageListStreamer(out value);

    public static bool TryGetCommonObject(this SerializationRecord record, [NotNullWhen(true)] out object? value) =>
        record.TryGetResXObject(out value)
        || record.TryGetDrawingPrimitivesObject(out value);
}
