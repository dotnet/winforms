// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.Runtime.Serialization.BinaryFormat;

internal sealed class RectangularOrCustomOffsetArrayRecord : ArrayRecord
{
    private AssemblyNameInfo? _elementTypeLibraryName;

    private RectangularOrCustomOffsetArrayRecord(Type elementType, ArrayInfo arrayInfo,
        MemberTypeInfo memberTypeInfo, int[] lengths, int[] offsets, RecordMap recordMap) : base(arrayInfo)
    {
        ElementType = elementType;
        MemberTypeInfo = memberTypeInfo;
        Lengths = lengths;
        Offsets = offsets;
        RecordMap = recordMap;
        ElementTypeName = memberTypeInfo.GetElementTypeName();
        Values = new();
    }

    public override RecordType RecordType => RecordType.BinaryArray;

    public override TypeName ElementTypeName { get; }

    public override AssemblyNameInfo ElementTypeLibraryName => _elementTypeLibraryName ??= MemberTypeInfo.GetElementLibraryName(RecordMap);

    private Type ElementType { get; }

    private MemberTypeInfo MemberTypeInfo { get; }

    private int[] Lengths { get; }

    private int[] Offsets { get; }

    private RecordMap RecordMap { get; }

    // This is the only array type that may have more elements than Array.MaxLength,
    // that is why we use Linked instead of regular List here.
    // TODO: verify my assumptions as I doubt them myself
    private LinkedList<object> Values { get; }

    internal override bool IsElementType(Type typeElement)
        => MemberTypeInfo.IsElementType(typeElement, RecordMap);

    private protected override Array Deserialize(Type arrayType, bool allowNulls, int maxLength)
    {
        if (Length > maxLength)
        {
            ThrowHelper.ThrowMaxArrayLength(maxLength, Length);
        }

        Array result = Array.CreateInstance(ElementType, Lengths, Offsets);

#if !NET8_0_OR_GREATER
        int[] indices = new int[Offsets.Length];
        Offsets.CopyTo(indices, 0); // respect custom offsets

        foreach (object value in Values)
        {
            result.SetValue(GetActualValue(value), indices);

            int dimension = indices.Length - 1;
            while (dimension >= 0)
            {
                indices[dimension]++;
                if (indices[dimension] < Offsets[dimension] + Lengths[dimension])
                {
                    break;
                }
                indices[dimension] = Offsets[dimension];
                dimension--;
            }

            if (dimension < 0)
            {
                break;
            }
        }

        return result;
#else
        // I took this idea from Array.CoreCLR that maps an array of int indices into
        // an internal flat index.
        // Yes, I know I'll most likely burn in hell for doing that.
        if (ElementType.IsValueType)
        {
            if (ElementType == typeof(bool)) CopyTo<bool>(Values, result);
            else if (ElementType == typeof(byte)) CopyTo<byte>(Values, result);
            else if (ElementType == typeof(sbyte)) CopyTo<sbyte>(Values, result);
            else if (ElementType == typeof(short)) CopyTo<short>(Values, result);
            else if (ElementType == typeof(ushort)) CopyTo<ushort>(Values, result);
            else if (ElementType == typeof(char)) CopyTo<char>(Values, result);
            else if (ElementType == typeof(int)) CopyTo<int>(Values, result);
            else if (ElementType == typeof(float)) CopyTo<float>(Values, result);
            else if (ElementType == typeof(long)) CopyTo<long>(Values, result);
            else if (ElementType == typeof(ulong)) CopyTo<ulong>(Values, result);
            else if (ElementType == typeof(double)) CopyTo<double>(Values, result);
            else if (ElementType == typeof(TimeSpan)) CopyTo<TimeSpan>(Values, result);
            else if (ElementType == typeof(DateTime)) CopyTo<DateTime>(Values, result);
            else if (ElementType == typeof(decimal)) CopyTo<decimal>(Values, result);
        }
        else
        {
            ref byte arrayDataRef = ref MemoryMarshal.GetArrayDataReference(result);
            ref object elementRef = ref Unsafe.As<byte, object>(ref arrayDataRef);
            nuint flattenedIndex = 0;
            foreach (object value in Values)
            {
                ref object offsetElementRef = ref Unsafe.Add(ref elementRef, flattenedIndex);
                offsetElementRef = GetActualValue(value)!;
                flattenedIndex++;
            }
        }

        return result;

        static void CopyTo<T>(LinkedList<object> list, Array array) where T : unmanaged
        {
            ref byte arrayDataRef = ref MemoryMarshal.GetArrayDataReference(array);
            ref T elementRef = ref Unsafe.As<byte, T>(ref arrayDataRef);
            nuint flattenedIndex = 0;
            foreach (object value in list)
            {
                ref T targetIndex = ref Unsafe.Add(ref elementRef, flattenedIndex);
                targetIndex = (T)GetActualValue(value)!;
                flattenedIndex++;
            }
        }
#endif
    }

    private protected override void AddValue(object value) => Values.AddLast(value);

    internal override (AllowedRecordTypes allowed, PrimitiveType primitiveType) GetAllowedRecordType()
    {
        (AllowedRecordTypes allowed, PrimitiveType primitiveType) = MemberTypeInfo.GetNextAllowedRecordType(0);

        if (allowed != AllowedRecordTypes.None)
        {
            // It's an array, it can also contain multiple nulls
            return (allowed | AllowedRecordTypes.Nulls, primitiveType);
        }

        return (allowed, primitiveType);
    }

    internal static RectangularOrCustomOffsetArrayRecord Create(ArrayInfo arrayInfo,
        MemberTypeInfo memberTypeInfo, int[] lengths, int[] offsets, RecordMap recordMap)
    {
        return memberTypeInfo.Infos[0].BinaryType switch
        {
            BinaryType.Primitive => new(MapPrimitive((PrimitiveType)memberTypeInfo.Infos[0].AdditionalInfo!), arrayInfo, memberTypeInfo, lengths, offsets, recordMap),
            BinaryType.String => new(typeof(string), arrayInfo, memberTypeInfo, lengths, offsets, recordMap),
            BinaryType.Object => new(typeof(object), arrayInfo, memberTypeInfo, lengths, offsets, recordMap),
            BinaryType.SystemClass or BinaryType.Class => new(typeof(ClassRecord), arrayInfo, memberTypeInfo, lengths, offsets, recordMap),
            _ => throw ThrowHelper.InvalidBinaryType(memberTypeInfo.Infos[0].BinaryType),
        };
    }

    private static Type MapPrimitive(PrimitiveType primitiveType)
        => primitiveType switch
        {
            PrimitiveType.Boolean => typeof(bool),
            PrimitiveType.Byte => typeof(byte),
            PrimitiveType.Char => typeof(char),
            PrimitiveType.Decimal => typeof(decimal),
            PrimitiveType.Double => typeof(double),
            PrimitiveType.Int16 => typeof(short),
            PrimitiveType.Int32 => typeof(int),
            PrimitiveType.Int64 => typeof(long),
            PrimitiveType.SByte => typeof(sbyte),
            PrimitiveType.Single => typeof(float),
            PrimitiveType.TimeSpan => typeof(TimeSpan),
            PrimitiveType.DateTime => typeof(DateTime),
            PrimitiveType.UInt16 => typeof(ushort),
            PrimitiveType.UInt32 => typeof(uint),
            PrimitiveType.UInt64 => typeof(ulong),
            _ => throw ThrowHelper.InvalidPrimitiveType(primitiveType),
        };

    private static object? GetActualValue(object value)
        => value is SerializationRecord serializationRecord
            ? serializationRecord.GetValue()
            : value; // it must be a primitive type
}
