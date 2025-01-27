// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Globalization;
using System.IO;

namespace System.Drawing;

public partial class ImageConverter : TypeConverter
{
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type? sourceType)
    {
        return sourceType == typeof(byte[]) || sourceType == typeof(Icon);
    }

    public override bool CanConvertTo(ITypeDescriptorContext? context, [NotNullWhen(true)] Type? destinationType)
    {
        return destinationType == typeof(byte[]) || destinationType == typeof(string);
    }

    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
        if (value is Icon icon)
        {
            return icon.ToBitmap();
        }

        if (value is byte[] bytes)
        {
            // Try to get memory stream for images with ole header.
            MemoryStream memStream = GetBitmapStream(bytes) ?? new MemoryStream(bytes);
            return Image.FromStream(memStream);
        }
        else
        {
            return base.ConvertFrom(context, culture, value);
        }
    }

    public override unsafe object ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
    {
        if (destinationType == typeof(string))
        {
            if (value is null)
            {
                return SR.none;
            }
            else if (value is Image)
            {
                return value.ToString()!;
            }
        }
        else if (destinationType == typeof(byte[]))
        {
            if (value is null)
            {
                return Array.Empty<byte>();
            }
            else if (value is Image image)
            {
                using MemoryStream ms = new();
                image.Save(ms);

                return ms.ToArray();
            }
        }

        throw GetConvertFromException(value);
    }

    [RequiresUnreferencedCode("The Type of value cannot be statically discovered. The public parameterless constructor or the 'Default' static field may be trimmed from the Attribute's Type.")]
    public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext? context, object? value, Attribute[]? attributes)
    {
        return TypeDescriptor.GetProperties(typeof(Image), attributes);
    }

    public override bool GetPropertiesSupported(ITypeDescriptorContext? context) => true;

    private static unsafe MemoryStream? GetBitmapStream(ReadOnlySpan<byte> rawData)
    {
        // Why we try to get out the bitmap stream from the Access (Jet) storage format isn't 100% clear. It might
        // have something to do with Visual Basic 3.0's support for Jet, but that has not been confirmed. The same
        // OLEOBJHEADER code is in VB6 (but not WFC). Now that the data is clearly described here, fast, and
        // thoroughly checked, it's probably best to just leave it in place.

        // Try to verify the incoming data to the level that Windows / Office would.

        SpanReader<byte> reader = new(rawData);
        if (!reader.TryRead(out OLEOBJHEADER header))
        {
            return null;
        }

        // Validate the OLEOBJHEADER
        if (header.typ != OLEOBJHEADER.OLEOBJID
            || header.oot != OleObjectType.OT_EMBEDDED
            || header.ibName != sizeof(OLEOBJHEADER)
            || header.ibClass != sizeof(OLEOBJHEADER) + header.cchName
            || header.cbHdr != sizeof(OLEOBJHEADER) + header.cchClass + header.cchName
            || header.ptSize != new Point(-1, -1)
            || !reader.TryRead(header.cchName, out ReadOnlySpan<byte> nameSpan)
            || !reader.TryRead(header.cchClass, out ReadOnlySpan<byte> classSpan))
        {
            return null;
        }

        // Unknown if this would ever be anything else in practice.
        Debug.Assert(nameSpan.SequenceEqual("Bitmap Image\0"u8));
        Debug.Assert(classSpan.SequenceEqual("Paint.Picture\0"u8));

        // At this point we're at the start of the OLE 1.0 data.

        // [MS-OLEDS]: Object Linking and Embedding (OLE)
        // 2.2.5 EmbeddedObject
        // https://learn.microsoft.com/openspecs/windows_protocols/ms-oleds/3395d95d-97f0-49ff-b792-28d331f254f1

        // Read and validate the ObjectHeader (version is the first uint)
        if (!reader.TryRead(out uint _)
            || !reader.TryRead(out FMTID format)
            || format != FMTID.FMTID_EMBED
            || !reader.TryRead(out int classLength)
            || !reader.TryRead(classLength, out ReadOnlySpan<byte> className)
            // MSPaint, which was originally PC Paintbrush
            || !className.SequenceEqual("PBrush\0"u8)
            || !reader.TryRead(out int topicLength)
            || topicLength != 0
            || !reader.TryRead(out int itemNameLength)
            || itemNameLength != 0)
        {
            return null;
        }

        // Read the EmbeddedObjectData
        if (!reader.TryRead(out int dataLength) || !reader.TryRead(dataLength, out var data))
        {
            return null;
        }

        // We could avoid a copy here with some effort, but this whole code path seems to be extremely rare.
        return new MemoryStream(data.ToArray());
    }
}
