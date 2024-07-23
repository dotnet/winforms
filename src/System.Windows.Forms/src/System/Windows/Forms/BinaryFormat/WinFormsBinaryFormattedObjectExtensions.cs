// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Private.Windows.Core.BinaryFormat;
using System.Runtime.CompilerServices;

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
    ///  Try to get a supported object.
    /// </summary>
    public static bool TryGetObject(this BinaryFormattedObject format, [NotNullWhen(true)] out object? value) =>
        format.TryGetFrameworkObject(out value)
        || format.TryGetBitmap(out value)
        || format.TryGetImageListStreamer(out value);

    /// <summary>
    ///  Try to get a supported object including those that don't have a <see cref="TypeConverter"/>.
    /// </summary>
    public static bool TryGetClipboardObject(this BinaryFormattedObject format, [NotNullWhen(true)] out object? value) =>
        format.TryGetObject(out value)
        || format.TryGetDrawingPrimitivesObject(out value);

    /// <summary>
    ///  Verify if this binary formatted object contains an object of type <typeparamref name="T"/>.
    /// </summary>
    public static bool Contains<T>(this BinaryFormattedObject format)
    {
        switch (format.RootRecord)
        {
            case ClassRecord record:
                {
                    var (recordTypeName, recordAssemblyName) = format.GetNamesFromClassRecord(record);

                    Type type = typeof(T);
                    var (requestedTypeName, requestedAssemblyName) = GetNamesFromTypeSimpleNullable(type);

                    // Name of the root record could be a .NET Framework or .NET type, first compare to T as the .NET type,
                    // then if T has a TypeForwardedFrom attribute, compare get assembly name type was forwarded from.
                    if (string.Equals(requestedTypeName, recordTypeName, StringComparison.Ordinal))
                    {
                        if (string.Equals(requestedAssemblyName, recordAssemblyName, StringComparison.Ordinal))
                        {
                            return true;
                        }

                        if (TryGetForwardedFromName(type, out string? name) && string.Equals(recordAssemblyName, name, StringComparison.Ordinal))
                        {
                            return true;
                        }
                    }

                    Type recordType = format.TypeResolver.GetType(record.Name, record.LibraryId);
                    return recordType.IsAssignableTo(typeof(T));
                }

            case BinaryObjectString:
                // We should return a string if the user is requesting an object.
                return typeof(string).IsAssignableTo(typeof(T));

            case ArraySingleString:
                return typeof(string[]).IsAssignableTo(typeof(T));

            case ArraySingleObject:
                return typeof(object[]).IsAssignableTo(typeof(T));

            case IBinaryArray binaryArray:
                {
                    if (binaryArray is not ArrayRecord arrayRecord)
                    {
                        throw new NotSupportedException($"A root {nameof(IBinaryArray)} is not an {nameof(ArrayRecord)}.");
                    }

                    Type requestedType = typeof(T);
                    if (!requestedType.IsArray)
                    {
                        return typeof(T) == typeof(object) || typeof(T) == typeof(Array);
                    }

                    return format.IsBinaryArrayOfType<T>(binaryArray);
                }

            case IPrimitiveTypeRecord primitiveArray:
                {
                    if (primitiveArray is not ArrayRecord)
                    {
                        throw new NotSupportedException($"A root {nameof(IPrimitiveTypeRecord)} is not an {nameof(ArrayRecord)}.");
                    }

                    Type requestedType = typeof(T);
                    if (!requestedType.IsArray)
                    {
                        return typeof(T) == typeof(object) || typeof(T) == typeof(Array);
                    }

                    if (requestedType.GetArrayRank() != 1)
                    {
                        return false;
                    }

                    return primitiveArray.PrimitiveType.GetPrimitiveTypeType() == requestedType.GetElementType();
                }

            default:
                throw new NotSupportedException($"RootRecord should not be of type {format.RootRecord.GetType().Name}.");
        }
    }

    private static (string typeName, string assemblyName) GetNamesFromClassRecord(this BinaryFormattedObject format, ClassRecord record)
    {
        string typeName = record.Name;
        string assemblyName = (record.LibraryId.IsNull
            ? TypeInfo.MscorlibAssemblyName
            : ((BinaryLibrary)format[record.LibraryId]).LibraryName) ?? throw new NotSupportedException("A class Record does not reference a valid library.");

        assemblyName = assemblyName.Split(',')[0].Trim();
        return (record.Name, assemblyName);
    }

    private static (string typeName, string assemblyName) GetNamesFromTypeSimpleNullable(Type type)
    {
        type = Formatter.NullableUnwrap(type);
        return (type.FullName!, type.Assembly.FullName!.Split(',')[0].Trim());
    }

    private static bool IsBinaryArrayOfType<T>(this BinaryFormattedObject format, IBinaryArray binaryArray)
    {
        BinaryArrayType arrayType = binaryArray.ArrayType;
        if (arrayType is not (BinaryArrayType.Single or BinaryArrayType.Jagged or BinaryArrayType.Rectangular))
        {
            // This can't happen after the stream had been read successfully.
            new NotSupportedException("Only arrays with zero offsets are supported.");
        }

        Type requestedType = typeof(T);
        if (!requestedType.IsArray || requestedType.GetArrayRank() != binaryArray.Rank)
        {
            return false;
        }

        object? info = binaryArray.ElementTypeInfo;
        Type? requestedElementType = requestedType.GetElementType();
        if (requestedElementType is null)
        {
            return false;
        }

        var (requestedTypeName, requestedAssemblyName) = GetNamesFromTypeSimpleNullable(requestedElementType);
        ArrayRecord arrayRecord = (ArrayRecord)binaryArray;
        switch (arrayRecord.ElementType)
        {
            case BinaryType.SystemClass:
                {
                    if (info is Type recordElementType)
                    {
                        return recordElementType.IsAssignableTo(requestedElementType);
                    }

                    string elementTypeName = (string)info!;
                    if (string.Equals(requestedTypeName, elementTypeName, StringComparison.Ordinal))
                    {
                        // TanyaSo: test DayOfWeek
                        if (string.Equals(requestedAssemblyName, TypeInfo.MscorlibAssemblyName, StringComparison.Ordinal))
                        {
                            return true;
                        }

                        if (TryGetForwardedFromName(requestedElementType, out string? name)
                            && string.Equals(TypeInfo.MscorlibAssemblyName, name, StringComparison.Ordinal))
                        {
                            return true;
                        }
                    }

                    Type recordType = format.TypeResolver.GetType(elementTypeName, Id.Null);
                    recordType = Formatter.NullableUnwrap(recordType);
                    return recordType.IsAssignableTo(requestedElementType);
                }

            case BinaryType.Class:
                {
                    if (info is Type recordElementType)
                    {
                        return recordElementType.IsAssignableTo(requestedElementType);
                    }

                    string elementTypeName;
                    string elementAssemblyName;
                    ClassTypeInfo classType = (ClassTypeInfo)info!;
                    elementTypeName = classType.TypeName;
                    elementAssemblyName = ((BinaryLibrary)format[classType.LibraryId]).LibraryName.Split(',')[0].Trim();

                    if (string.Equals(requestedTypeName, elementTypeName, StringComparison.Ordinal))
                    {
                        if (string.Equals(requestedAssemblyName, elementAssemblyName, StringComparison.Ordinal))
                        {
                            return true;
                        }

                        if (TryGetForwardedFromName(requestedElementType, out string? name) && string.Equals(elementAssemblyName, name, StringComparison.Ordinal))
                        {
                            return true;
                        }
                    }

                    recordElementType = format.TypeResolver.GetType(elementTypeName, Id.Null);
                    return recordElementType.IsAssignableTo(requestedElementType);
                }

            case BinaryType.String:
                return requestedElementType == typeof(string);

            case BinaryType.PrimitiveArray:
                while (requestedElementType?.IsArray is true)
                {
                    // Jagged array is an array of arrays.
                    requestedElementType = requestedElementType.GetElementType();
                }

                return ((PrimitiveType)info!).GetPrimitiveTypeType().IsAssignableTo(requestedElementType);

            case BinaryType.Primitive:
                return ((PrimitiveType)info!).GetPrimitiveTypeType().IsAssignableTo(requestedElementType);

            default:
                throw new NotSupportedException($"Unexpected BinaryArray type: {arrayRecord.ElementType}");
        }
    }

    internal static bool TryGetForwardedFromName(this Type type, [NotNullWhen(true)] out string? name)
    {
        name = null;
        // Special case types like arrays.
        Type attributedType = type;
        while (attributedType.HasElementType)
        {
            attributedType = attributedType.GetElementType()!;
        }

        attributedType = Formatter.NullableUnwrap(attributedType);

        object[] a = attributedType.GetCustomAttributes(typeof(TypeForwardedFromAttribute), inherit: false);

        foreach (Attribute first in attributedType.GetCustomAttributes(typeof(TypeForwardedFromAttribute), inherit: false))
        {
            name = ((TypeForwardedFromAttribute)first).AssemblyFullName.Split(',')[0].Trim();
            return true;
        }

        return false;
    }
}
