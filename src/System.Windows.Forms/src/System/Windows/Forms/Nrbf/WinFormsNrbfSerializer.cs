// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

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

    // These types are read from and written to serialized stream manually, accessing record field by field.
    // Thus they are re-hydrated with no formatters and are safe. The default resolver should recognize them
    // to resolve primitive types or fields of the specified type T.
    private static readonly Type[] s_intrinsicTypes =
    [
        // Common WinForms types.
        typeof(ImageListStreamer),
        typeof(Drawing.Bitmap),
    ];

    // Do not allow construction of this type.
    private WinFormsNrbfSerializer() { }

    public static bool TryBindToType(TypeName typeName, [NotNullWhen(true)] out Type? type)
    {
        if (CoreNrbfSerializer.TryBindToType(typeName, out type))
        {
            return true;
        }

        if (s_knownTypes is null)
        {
            s_knownTypes = new(s_intrinsicTypes.Length, TypeNameComparer.Default);
            foreach (Type intrinsic in s_intrinsicTypes)
            {
                s_knownTypes.Add(intrinsic.ToTypeName(), intrinsic);
            }
        }

        return s_knownTypes.TryGetValue(typeName, out type);

        throw new NotImplementedException();
    }

    public static bool TryGetObject(SerializationRecord record, [NotNullWhen(true)] out object? value) =>
        CoreNrbfSerializer.TryGetObject(record, out value)
        || TryGetBitmap(record, out value)
        || TryGetImageListStreamer(record, out value);

    public static bool TryWriteObject(Stream stream, object value) =>
        CoreNrbfSerializer.TryWriteObject(stream, value)
        || WinFormsBinaryFormatWriter.TryWriteObject(stream, value);

    public static bool IsSupportedType<T>() => CoreNrbfSerializer.IsSupportedType<T>()
        || typeof(T) == typeof(Drawing.Bitmap)
        || typeof(T) == typeof(ImageListStreamer);
}
