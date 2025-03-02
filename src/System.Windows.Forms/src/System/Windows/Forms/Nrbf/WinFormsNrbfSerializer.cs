// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Formats.Nrbf;
using System.Private.Windows.Nrbf;
using System.Reflection.Metadata;
using System.Windows.Forms.BinaryFormat;

namespace System.Windows.Forms.Nrbf;

/// <summary>
///  Provides binary format serialization services for Windows Forms.
/// </summary>
internal sealed partial class WinFormsNrbfSerializer : INrbfSerializer
{
    private static Dictionary<TypeName, Type>? s_knownTypes;

    // Do not allow construction of this type.
    private WinFormsNrbfSerializer() { }

    public static bool TryBindToType(TypeName typeName, [NotNullWhen(true)] out Type? type)
    {
        if (CoreNrbfSerializer.TryBindToType(typeName, out type))
        {
            return true;
        }

        s_knownTypes ??= new(3, TypeNameComparer.FullNameAndAssemblyNameMatch)
        {
            { Types.ToTypeName($"{typeof(ImageListStreamer).FullName}, System.Windows.Forms"), typeof(ImageListStreamer) },
            { Types.ToTypeName($"{Types.BitmapType}, System.Drawing"), typeof(Bitmap) },
            { Types.ToTypeName($"{Types.BitmapType}, System.Drawing.Common"), typeof(Bitmap) }
        };

        return s_knownTypes.TryGetValue(typeName, out type);
    }

    public static bool TryGetObject(SerializationRecord record, [NotNullWhen(true)] out object? value) =>
        CoreNrbfSerializer.TryGetObject(record, out value)
        || TryGetBitmap(record, out value)
        || TryGetImageListStreamer(record, out value);

    public static bool TryWriteObject(Stream stream, object value) =>
        CoreNrbfSerializer.TryWriteObject(stream, value)
        || WinFormsBinaryFormatWriter.TryWriteObject(stream, value);

    public static bool IsFullySupportedType(Type type) => CoreNrbfSerializer.IsFullySupportedType(type)
        // If users want to include ImageListStreamer in their custom types, they should explicitly resolve it.
        || type == typeof(Bitmap);
}
