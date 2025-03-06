// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Formats.Nrbf;
using System.Private.Windows.Nrbf;

namespace System.Windows.Forms.Nrbf;

internal sealed partial class WinFormsNrbfSerializer
{
    /// <summary>
    ///  Tries to get a record as a binary formatted <see cref="ImageListStreamer"/>.
    /// </summary>
    private static bool TryGetImageListStreamer(
        SerializationRecord record,
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
    ///  Tries to get a record as a binary formatted <see cref="Bitmap"/>.
    /// </summary>
    private static bool TryGetBitmap(SerializationRecord record, out object? bitmap)
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
}
