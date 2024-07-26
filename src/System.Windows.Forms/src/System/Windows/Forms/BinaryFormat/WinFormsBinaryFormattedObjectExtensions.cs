// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Private.Windows;
using System.Private.Windows.Core.BinaryFormat;
using System.Reflection.Metadata;
using System.Text.Json;
using MemberReference = System.Private.Windows.Core.BinaryFormat.MemberReference;

namespace System.Windows.Forms.BinaryFormat;

internal static class WinFormsBinaryFormattedObjectExtensions
{
    /// <summary>
    ///  Tries to get this object as a binary formatted <see cref="ImageListStreamer"/>.
    /// </summary>
    public static bool TryGetImageListStreamer(
        this BinaryFormattedObject format,
        out object? imageListStreamer)
    {
        return BinaryFormattedObjectExtensions.TryGet(Get, format, out imageListStreamer);

        static bool Get(BinaryFormattedObject format, [NotNullWhen(true)] out object? imageListStreamer)
        {
            imageListStreamer = null;

            if (format.RootRecord is not ClassWithMembersAndTypes types
                || types.ClassInfo.Name != typeof(ImageListStreamer).FullName
                || format[3] is not ArraySinglePrimitive<byte> data)
            {
                return false;
            }

            Debug.Assert(data.ArrayObjects is byte[]);
            imageListStreamer = new ImageListStreamer((byte[])data.ArrayObjects);
            return true;
        }
    }

    /// <summary>
    ///  Tries to get this object as a binary formatted <see cref="Bitmap"/>.
    /// </summary>
    public static bool TryGetBitmap(this BinaryFormattedObject format, out object? bitmap)
    {
        bitmap = null;

        if (format.RootRecord is not ClassWithMembersAndTypes types
            || types.ClassInfo.Name != typeof(Bitmap).FullName
            || format[3] is not ArraySinglePrimitive<byte> data)
        {
            return false;
        }

        Debug.Assert(data.ArrayObjects is byte[]);
        bitmap = new Bitmap(new MemoryStream((byte[])data.ArrayObjects));
        return true;
    }

    /// <summary>
    ///  Tries to deserialize this object if it was serialized as JSON.
    /// </summary>
    public static bool TryGetObjectFromJson(this BinaryFormattedObject format, out object? @object)
    {
        @object = null;

        if (format[2] is not BinaryLibrary library || library.LibraryName != IJsonData.CustomAssemblyName)
        {
            // The data was not serialized as JSON.
            return false;
        }

        if (format.RootRecord is not ClassWithMembersAndTypes types
            || types["<JsonBytes>k__BackingField"] is not MemberReference reference
            || format[reference] is not ArraySinglePrimitive<byte> byteData
            || !TypeName.TryParse(types.ClassInfo.Name, out TypeName? result)
            || Type.GetType(result.GetGenericArguments()[0].AssemblyQualifiedName) is not Type genericType)
        {
            // This is supposed to be JsonData, but somehow the binary formatted data is corrupt.
            throw new InvalidOperationException();
        }

        // TODO: We should get the type from the Func<TypeName, Type> that will be passed down instead of using Type.GetType()
        @object = JsonSerializer.Deserialize((byte[])byteData.ArrayObjects, genericType);

        return true;
    }

    /// <summary>
    ///  Try to get a supported object.
    /// </summary>
    public static bool TryGetObject(this BinaryFormattedObject format, [NotNullWhen(true)] out object? value) =>
        format.TryGetFrameworkObject(out value)
        || format.TryGetBitmap(out value)
        || format.TryGetImageListStreamer(out value)
        || format.TryGetObjectFromJson(out value);
}
