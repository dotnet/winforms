// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Reflection.Metadata;
using BinaryFormatUtilities = System.Private.Windows.Ole.BinaryFormatUtilities<System.Private.Windows.Nrbf.CoreNrbfSerializer>;

namespace System.Private.Windows.Ole.Tests;

public sealed partial class BinaryFormatUtilitiesTests : BinaryFormatUtilitesTestsBase
{
    protected override void WriteObjectToStream(MemoryStream stream, object data, string format)
    {
        BinaryFormatUtilities.WriteObjectToStream(stream, data, format);
    }

    protected override bool TryReadObjectFromStream<T>(
        MemoryStream stream,
        bool untypedRequest,
        string format,
        Func<TypeName, Type>? resolver,
        [NotNullWhen(true)] out T? @object) where T : default
    {
        DataRequest request = new(format) { Resolver = resolver, TypedRequest = !untypedRequest };
        return BinaryFormatUtilities.TryReadObjectFromStream(stream, in request, out @object);
    }

    // Primitive types as defined by the NRBF spec.
    // https://learn.microsoft.com/dotnet/api/system.formats.nrbf.primitivetyperecord
    public static TheoryData<object> PrimitiveObjects_TheoryData =>
    [
        (byte)1,
        (sbyte)2,
        (short)3,
        (ushort)4,
        5,
        (uint)6,
        (long)7,
        (ulong)8,
        (float)9.0,
        10.0,
        'a',
        true,
        "string",
        DateTime.Now,
        TimeSpan.FromHours(1),
        decimal.MaxValue
    ];

    public static TheoryData<object> KnownObjects_TheoryData =>
    [
        -(nint)11,
        (nuint)12,
        new PointF(1, 2),
        new RectangleF(1, 2, 3, 4),
        new Point(-1, int.MaxValue),
        new Rectangle(-1, int.MinValue, 10, 13),
        new Size(int.MaxValue, int.MinValue),
        new SizeF(float.MaxValue, float.MinValue),
        Color.Red
    ];

    public static TheoryData<IList> PrimitiveListObjects_TheoryData =>
    [
        new List<bool> { false, true },
        new List<char> { char.MinValue, char.MaxValue },
        new List<byte> { byte.MinValue, byte.MaxValue },
        new List<sbyte> { sbyte.MinValue, sbyte.MaxValue },
        new List<short> { short.MinValue, short.MaxValue },
        new List<ushort> { ushort.MinValue, ushort.MaxValue },
        new List<int> { int.MinValue, int.MaxValue },
        new List<uint> { uint.MinValue, uint.MaxValue },
        new List<long> { long.MinValue, long.MaxValue },
        new List<ulong> { ulong.MinValue, ulong.MaxValue },
        new List<float> { float.MinValue, float.MaxValue },
        new List<double> { double.MinValue, double.MaxValue },
        new List<decimal> { decimal.MinValue, decimal.MaxValue },
        new List<DateTime> { DateTime.MinValue, DateTime.MaxValue },
        new List<TimeSpan> { TimeSpan.MinValue, TimeSpan.MaxValue },
        new List<string> { "a", "b", "c" }
    ];

    public static TheoryData<Array> PrimitiveArrayObjects_TheoryData =>
    [
        new bool[] { false, true },
        new char[] { char.MinValue, char.MaxValue },
        new byte[] { byte.MinValue, byte.MaxValue },
        new sbyte[] { sbyte.MinValue, sbyte.MaxValue },
        new short[] { short.MinValue, short.MaxValue },
        new ushort[] { ushort.MinValue, ushort.MaxValue },
        new int[] { int.MinValue, int.MaxValue },
        new uint[] { uint.MinValue, uint.MaxValue },
        new long[] { long.MinValue, long.MaxValue },
        new ulong[] { ulong.MinValue, ulong.MaxValue },
        new float[] { float.MinValue, float.MaxValue },
        new double[] { double.MinValue, double.MaxValue },
        new decimal[] { decimal.MinValue, decimal.MaxValue },
        new DateTime[] { DateTime.MinValue, DateTime.MaxValue },
        new TimeSpan[] { TimeSpan.MinValue, TimeSpan.MaxValue },
        new string[] { "a", "b", "c" }
    ];

    public static TheoryData<ArrayList> PrimitiveArrayListObjects_TheoryData =>
    [
        [null],
        [null, "something"],
        [false, true],
        [char.MinValue, char.MaxValue],
        [byte.MinValue, byte.MaxValue],
        [sbyte.MinValue, sbyte.MaxValue],
        [short.MinValue, short.MaxValue],
        [ushort.MinValue, ushort.MaxValue],
        [int.MinValue, int.MaxValue],
        [uint.MinValue, uint.MaxValue],
        [long.MinValue, long.MaxValue],
        [ulong.MinValue, ulong.MaxValue],
        [float.MinValue, float.MaxValue],
        [double.MinValue, double.MaxValue],
        [decimal.MinValue, decimal.MaxValue],
        [DateTime.MinValue, DateTime.MaxValue],
        [TimeSpan.MinValue, TimeSpan.MaxValue],
        ["a", "b", "c"]
    ];

    public static TheoryData<Hashtable> PrimitiveTypeHashtables_TheoryData =>
    [
        new Hashtable { { "bool", true } },
        new Hashtable { { "char", 'a' } },
        new Hashtable { { "byte", (byte)1 } },
        new Hashtable { { "sbyte", (sbyte)2 } },
        new Hashtable { { "short", (short)3 } },
        new Hashtable { { "ushort", (ushort)4 } },
        new Hashtable { { "int", 5 } },
        new Hashtable { { "uint", (uint)6 } },
        new Hashtable { { "long", (long)7 } },
        new Hashtable { { "ulong", (ulong)8 } },
        new Hashtable { { "float", 9.0f } },
        new Hashtable { { "double", 10.0 } },
        new Hashtable { { "decimal", (decimal)11 } },
        new Hashtable { { "DateTime", DateTime.Now } },
        new Hashtable { { "TimeSpan", TimeSpan.FromHours(1) } },
        new Hashtable { { "string", "test" } }
    ];

    public static TheoryData<NotSupportedException> NotSupportedException_TestData =>
    [
        new(),
        new("Error message"),
        new(null)
    ];

    public static TheoryData<IList> Lists_UnsupportedTestData =>
    [
        new List<object>(),
        new List<nint>(),
        new List<(int, int)>(),
        new List<nint> { nint.MinValue, nint.MaxValue },
        new List<nuint> { nuint.MinValue, nuint.MaxValue }
    ];

    [Theory]
    [MemberData(nameof(PrimitiveObjects_TheoryData))]
    [MemberData(nameof(KnownObjects_TheoryData))]
    public void RoundTrip_Simple(object value)
    {
        RoundTripObject(value, out object? result).Should().BeTrue();
        result.Should().Be(value);
    }

    [Theory]
    [MemberData(nameof(PrimitiveObjects_TheoryData))]
    [MemberData(nameof(KnownObjects_TheoryData))]
    public void RoundTrip_PredefinedFormat_Simple(object value)
    {
        RoundTripObject_PredefinedFormat(value, out object? result).Should().BeTrue();
        result.Should().Be(value);
    }

    [Theory]
    [MemberData(nameof(NotSupportedException_TestData))]
    public void RoundTrip_NotSupportedException(NotSupportedException value)
    {
        RoundTripObject(value, out object? result).Should().BeTrue();
        result.Should().BeEquivalentTo(value);
    }

    [Theory]
    [MemberData(nameof(NotSupportedException_TestData))]
    public void RoundTrip_PredefinedFormat_NotSupportedException(NotSupportedException value)
    {
        RoundTripObject_PredefinedFormat(value, out object? result).Should().BeTrue();
        result.Should().BeEquivalentTo(value);
    }

    [Fact]
    public void RoundTrip_NotSupportedException_DataLoss()
    {
        NotSupportedException value = new("Error message", new ArgumentException());
        RoundTripObject(value, out object? result).Should().BeTrue();
        result.Should().BeEquivalentTo(new NotSupportedException("Error message", innerException: null));
    }

    [Fact]
    public void RoundTrip_PredefinedFormat_NotSupportedException_DataLoss()
    {
        NotSupportedException value = new("Error message", new ArgumentException());
        RoundTripObject_PredefinedFormat(value, out object? result).Should().BeTrue();
        result.Should().BeEquivalentTo(new NotSupportedException("Error message", innerException: null));
    }

    [Theory]
    [MemberData(nameof(PrimitiveListObjects_TheoryData))]
    public void RoundTrip_PrimitiveList(IList value)
    {
        RoundTripObject(value, out object? result).Should().BeTrue();
        result.Should().BeEquivalentTo(value);
    }

    [Theory]
    [MemberData(nameof(PrimitiveListObjects_TheoryData))]
    public void RoundTrip_PredefinedFormat_PrimitiveList(IList value)
    {
        RoundTripObject_PredefinedFormat(value, out object? result).Should().BeTrue();
        result.Should().BeEquivalentTo(value);
    }

    [Theory]
    [MemberData(nameof(PrimitiveArrayObjects_TheoryData))]
    public void TryReadObjectFromStream_Primitives_BaseResolver(Array value)
    {
        using MemoryStream stream = new();
        BinaryFormatUtilities.WriteObjectToStream(stream, value, "test");
        stream.Position = 0;

        DataRequest request = new()
        {
            Format = "test",
            TypedRequest = true,
            Resolver = (TypeName typeName) => Type.GetType(typeName.FullName)
        };

        BinaryFormatUtilities.TryReadObjectFromStream(stream, in request, out Array? result).Should().BeTrue();
        result.Should().BeEquivalentTo(value);
    }

    [Theory]
    [EnumData<DataType>]
    public void TryReadObjectFromStream_IntArray_TypedToBaseFails(DataType dataType)
    {
        int[] array = [];
        using MemoryStream stream = CreateStream(dataType, array);

        DataRequest request = new()
        {
            Format = "test",
            TypedRequest = true
        };

        Action action = () => BinaryFormatUtilities.TryReadObjectFromStream(stream, in request, out Array? _);
        action.Should().Throw<NotSupportedException>();

        action = () => BinaryFormatUtilities.TryReadObjectFromStream(stream, in request, out IEnumerable? _);
        action.Should().Throw<NotSupportedException>();

        action = () => BinaryFormatUtilities.TryReadObjectFromStream(stream, in request, out object? result3);
        action.Should().Throw<NotSupportedException>();
    }

    [Theory]
    [EnumData<DataType>]
    public void TryReadObjectFromStream_IntArray_UntypedSucceeds(DataType dataType)
    {
        int[] array = [];
        using MemoryStream stream = CreateStream(dataType, array);

        DataRequest request = new()
        {
            Format = "test",
            TypedRequest = false
        };

        BinaryFormatUtilities.TryReadObjectFromStream(stream, in request, out object? result).Should().BeTrue();
        result.Should().BeEquivalentTo(array);
    }

    [Theory]
    [EnumData<DataType>]
    public void TryReadObjectFromStream_IntArray_TypedToBaseSucceeds(DataType dataType)
    {
        int[] array = [];
        using MemoryStream stream = CreateStream(dataType, array);

        DataRequest request = new()
        {
            Format = "test",
            TypedRequest = true,
            Resolver = (TypeName typeName) => typeof(int[]).Matches(typeName, TypeNameComparison.TypeFullName)
                ? typeof(int[])
                : throw new NotSupportedException()
        };

        BinaryFormatUtilities.TryReadObjectFromStream(stream, in request, out Array? result).Should().BeTrue();
        result.Should().BeEquivalentTo(array);

        BinaryFormatUtilities.TryReadObjectFromStream(stream, in request, out IEnumerable? result2).Should().BeTrue();
        result2.Should().BeEquivalentTo(array);

        BinaryFormatUtilities.TryReadObjectFromStream(stream, in request, out object? result3).Should().BeTrue();
        result3.Should().BeEquivalentTo(array);
    }

    [Theory]
    [MemberData(nameof(PrimitiveArrayListObjects_TheoryData))]
    public void RoundTrip_PrimitiveArrayList(ArrayList value)
    {
        RoundTripObject(value, out object? result).Should().BeTrue();
        result.Should().BeEquivalentTo(value);
    }

    [Theory]
    [MemberData(nameof(PrimitiveArrayListObjects_TheoryData))]
    public void RoundTrip_PredefinedFormat_PrimitiveArrayList(ArrayList value)
    {
        RoundTripObject_PredefinedFormat(value, out object? result).Should().BeTrue();
        result.Should().BeEquivalentTo(value);
    }

    [Theory]
    [MemberData(nameof(PrimitiveTypeHashtables_TheoryData))]
    public void RoundTrip_PrimitiveHashtable(Hashtable value)
    {
        RoundTripObject(value, out object? result).Should().BeTrue();
        result.Should().BeEquivalentTo(value);
    }

    [Theory]
    [MemberData(nameof(PrimitiveTypeHashtables_TheoryData))]
    public void RoundTrip_PredefinedFormat_PrimitiveHashtable(Hashtable value)
    {
        RoundTripObject_PredefinedFormat(value, out object? result).Should().BeTrue();
        result.Should().BeEquivalentTo(value);
    }

    [Theory]
    [MemberData(nameof(Lists_UnsupportedTestData))]
    public void RoundTrip_Unsupported(IList value)
    {
        Action writer = () => WriteObjectToStream(value);
        Action reader = () => TryReadObjectFromStream(out object? _);

        writer.Should().Throw<NotSupportedException>();

        using (NrbfSerializerInClipboardDragDropScope nrbfScope = new(enable: false))
        {
            using (BinaryFormatterScope scope = new(enable: true))
            {
                writer.Should().Throw<NotSupportedException>();

                using BinaryFormatterInClipboardDragDropScope clipboardDragDropScope = new(enable: true);
                WriteObjectToStream(value);
                TryReadObjectFromStream(out object? result).Should().BeTrue();
                result.Should().BeEquivalentTo(value);
            }

            reader.Should().Throw<NotSupportedException>();
        }

        // Binary format deserializers in Clipboard/DragDrop scenarios are not opted in.
        reader.Should().Throw<NotSupportedException>();
    }

    [Theory]
    [MemberData(nameof(Lists_UnsupportedTestData))]
    public void RoundTrip_PredefinedFormat_Unsupported(IList value)
    {
        Action writer = () => WriteObjectToStream(value, restrictSerialization: true);
        writer.Should().Throw<NotSupportedException>();

        using ClipboardBinaryFormatterFullCompatScope scope = new();
        writer.Should().Throw<RestrictedTypeDeserializationException>();
    }

    [Fact]
    public void RoundTrip_OffsetArray()
    {
        Array value = Array.CreateInstance(typeof(uint), lengths: [2, 3], lowerBounds: [1, 2]);
        value.SetValue(101u, 1, 2);
        value.SetValue(102u, 1, 3);
        value.SetValue(103u, 1, 4);
        value.SetValue(201u, 2, 2);
        value.SetValue(202u, 2, 3);
        value.SetValue(203u, 2, 4);

        // Can read offset array with the BinaryFormatter.
        using ClipboardBinaryFormatterFullCompatScope scope = new();
        RoundTripObject(value, out object? result).Should().BeTrue();
        result.Should().NotBeNull();
        Assert.Equal(value, result);
    }

    [Fact]
    public void RoundTripOfType_Unsupported()
    {
        // Not a known type, while 'List<object>' is resolved by default, 'object' requires a custom resolver.
        List<object> value = ["text"];
        using (ClipboardBinaryFormatterFullCompatScope scope = new())
        {
            WriteObjectToStream(value);

            ReadAndValidate();

            using NrbfSerializerInClipboardDragDropScope nrbfScope = new(enable: true);
            ReadAndValidate();
        }

        Action read = () => TryReadObjectFromStream<List<object>>(ObjectListResolver, out _);
        read.Should().Throw<NotSupportedException>();

        void ReadAndValidate()
        {
            TryReadObjectFromStream(ObjectListResolver, out List<object>? result).Should().BeTrue();
            result.Should().BeOfType<List<object>>();
            result!.Count.Should().Be(1);
            result[0].Should().Be("text");
        }

        static Type ObjectListResolver(TypeName typeName)
        {
            (string name, Type type)[] allowedTypes =
            [
                ("System.Object", typeof(object)),
                ("System.Collections.Generic.List`1[[System.Object, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]", typeof(List<object>))
            ];

            string fullName = typeName.FullName;
            foreach (var (name, type) in allowedTypes)
            {
                // Namespace-qualified type name.
                if (name == fullName)
                {
                    return type;
                }
            }

            throw new NotSupportedException($"Can't resolve {typeName.AssemblyQualifiedName}");
        }
    }

    [Theory]
    [EnumData<DataType>]
    public void TryReadObjectFromStream_TypeDoesNotMatch(DataType dataType)
    {
        List<int> value = [1, 2, 3];
        using MemoryStream stream = CreateStream(dataType, value);

        // Typed with no resolver
        DataRequest request = new()
        {
            Format = "test",
            TypedRequest = true,
        };

        Action action = () => BinaryFormatUtilities.TryReadObjectFromStream(stream, ref request, out BaseClass? _);
        action.Should().Throw<NotSupportedException>();
    }

    [Theory]
    [EnumData<DataType>]
    public void TryReadObjectFromStream_Class_DerivedAsBase(DataType dataType)
    {
        using BinaryFormatterScope scope = new(enable: dataType == DataType.BinaryFormat);
        using BinaryFormatterInClipboardDragDropScope clipboardScope = new(enable: dataType == DataType.BinaryFormat);

        using MemoryStream stream = CreateStream(dataType, new DerivedClass());

        // Typed with no resolver
        DataRequest request = new()
        {
            Format = "test",
            TypedRequest = true,
        };

        Action action = () => BinaryFormatUtilities.TryReadObjectFromStream(stream, ref request, out BaseClass? result);
        action.Should().Throw<NotSupportedException>();

        // Typed with resolver
        request = new()
        {
            Format = "test",
            TypedRequest = true,
            Resolver = (TypeName typeName) => typeName.FullName == typeof(DerivedClass).FullName
                ? typeof(DerivedClass)
                : throw new NotSupportedException()
        };

        BinaryFormatUtilities.TryReadObjectFromStream(stream, ref request, out BaseClass? result).Should().BeTrue();
        result.Should().NotBeNull();
    }

    [Serializable]
    public class BaseClass { }

    [Serializable]
    public class DerivedClass : BaseClass { }

    [Theory]
    [EnumData<DataType>]
    public void TryReadObjectFromStream_Struct_AsNullable(DataType dataType)
    {
        using BinaryFormatterScope scope = new(enable: dataType == DataType.BinaryFormat);
        using BinaryFormatterInClipboardDragDropScope clipboardScope = new(enable: dataType == DataType.BinaryFormat);

        using MemoryStream stream = CreateStream(dataType, default(MyStruct));

        // Typed with no resolver
        DataRequest request = new()
        {
            Format = "test",
            TypedRequest = true,
        };

        Action action = () => BinaryFormatUtilities.TryReadObjectFromStream(stream, ref request, out MyStruct? result);
        action.Should().Throw<NotSupportedException>();

        // Typed with resolver
        request = new()
        {
            Format = "test",
            TypedRequest = true,
            Resolver = (TypeName typeName) => typeName.FullName == typeof(MyStruct).FullName
                ? typeof(MyStruct?)
                : throw new NotSupportedException()
        };

        BinaryFormatUtilities.TryReadObjectFromStream(stream, ref request, out MyStruct? result).Should().BeTrue();
        result.Should().NotBeNull();
    }

    [Serializable]
    public struct MyStruct { }

    [Theory]
    [EnumData<DataType>]
    public void TryReadObjectFromStream_Int_AsNullableInt(DataType dataType)
    {
        using MemoryStream stream = CreateStream(dataType, 101);

        // No resolver does not succeed
        DataRequest request = new()
        {
            Format = "test",
            TypedRequest = true,
        };

        Action action = () => BinaryFormatUtilities.TryReadObjectFromStream(stream, ref request, out int? result).Should().BeTrue();
        action.Should().Throw<NotSupportedException>();

        stream.Position.Should().Be(0);

        // If we say no, we mean no.
        request = new()
        {
            Format = "test",
            TypedRequest = true,
            Resolver = NotSupportedResolver
        };

        // Auto rebinding from int to int? adds signficant logic complexity with little real value.
        action = () => BinaryFormatUtilities.TryReadObjectFromStream(stream, ref request, out int? result);
        action.Should().Throw<NotSupportedException>();

        stream.Position.Should().Be(0);

        // Let's redirect.
        request = new()
        {
            Format = "test",
            TypedRequest = true,
            Resolver = (TypeName typeName) => typeName.FullName == typeof(int).FullName
                ? typeof(int?)
                : throw new NotSupportedException()
        };

        BinaryFormatUtilities.TryReadObjectFromStream(stream, ref request, out int? result).Should().BeTrue();
        result.Should().Be(101);

        request = new()
        {
            Format = "test",
            TypedRequest = false
        };
    }

    [Theory]
    [EnumData<DataType>]
    public void TryReadObjectFromStream_Int_AsFloat(DataType dataType)
    {
        using MemoryStream stream = CreateStream(dataType, 101);

        // If we say no, we mean no.
        DataRequest request = new()
        {
            Format = "test",
            TypedRequest = true,
            Resolver = NotSupportedResolver
        };

        Action action = () => BinaryFormatUtilities.TryReadObjectFromStream(stream, ref request, out float result);
        action.Should().Throw<NotSupportedException>();

        stream.Position.Should().Be(0);

        // Let's redirect.
        request = new()
        {
            Format = "test",
            TypedRequest = true,
            Resolver = (TypeName typeName) => typeName.FullName == typeof(int).FullName
                ? typeof(float)
                : throw new NotSupportedException()
        };

        action = () =>
        {
            BinaryFormatUtilities.TryReadObjectFromStream(stream, ref request, out float result).Should().BeTrue();
            result.Should().Be(101);
        };

        if (dataType == DataType.BinaryFormat)
        {
            action.Should().Throw<InvalidCastException>();
        }
        else
        {
            // This is one of the side-effects of using JSON. The data is very basic. A 101 value can be converted to
            // float without a problem.
            action.Should().NotThrow();
        }
    }

    [Fact]
    public void RoundTripOfType_PredefinedFormat_intNullableArray_NotSupportedResolver()
    {
        int?[] value = [101, null, 303];

        using ClipboardBinaryFormatterFullCompatScope scope = new();
        WriteObjectToStream(value);
        Action read = () => ReadPredefinedObjectFromStream<int?[]>(NotSupportedResolver, out _);

        // nullable struct requires a custom resolver.
        // PredefinedTypeDeserializationException
        read.Should().Throw<Exception>();
    }

    [Fact]
    public void RoundTripOfType_intNullableArray_NotSupportedResolver()
    {
        int?[] value = [101, null, 303];

        using ClipboardBinaryFormatterFullCompatScope scope = new();
        WriteObjectToStream(value);

        Action action = () => TryReadObjectFromStream<int?[]>(NotSupportedResolver, out _);
        action.Should().Throw<NotSupportedException>();
    }

    [Theory]
    [BoolData]
    public void RoundTripOfType_OffsetArray_NotSupportedResolver(bool restrictDeserialization)
    {
        Array value = Array.CreateInstance(typeof(uint), lengths: [2, 3], lowerBounds: [1, 2]);
        value.SetValue(101u, 1, 2);
        value.SetValue(102u, 1, 3);
        value.SetValue(103u, 1, 4);
        value.SetValue(201u, 2, 2);
        value.SetValue(202u, 2, 3);
        value.SetValue(203u, 2, 4);

        using ClipboardBinaryFormatterFullCompatScope scope = new();
        WriteObjectToStream(value);

        if (restrictDeserialization)
        {
            Action action = () => ReadObjectFromStream<uint[,]>(restrictDeserialization, NotSupportedResolver, out _);
            action.Should().Throw<RestrictedTypeDeserializationException>();
        }
        else
        {
            ReadObjectFromStream<uint[,]>(restrictDeserialization, NotSupportedResolver, out var result).Should().BeTrue();

            // FluentAssertions cannot validate non-zero lower bounds.
            Assert.Equal(result, value);
        }
    }

    [Fact]
    public void RoundTripOfType_intNullableArray_CustomResolver()
    {
        int?[] value = [101, null, 303];

        using ClipboardBinaryFormatterFullCompatScope scope = new();
        RoundTripOfType(value, NullableIntArrayResolver, out int?[]? result).Should().BeTrue();
        result.Should().BeEquivalentTo(value);
    }

    private static Type NullableIntArrayResolver(TypeName typeName)
    {
        (string name, Type type)[] allowedTypes =
        [
            ("System.Nullable`1[[System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]][]", typeof(int?[])),
            ("System.Nullable`1[[System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]", typeof(int?))
        ];

        string fullName = typeName.FullName;
        foreach (var (name, type) in allowedTypes)
        {
            // Namespace-qualified type name.
            if (name == fullName)
            {
                return type;
            }
        }

        throw new NotSupportedException($"Can't resolve {typeName.AssemblyQualifiedName}");
    }

    [Fact]
    public void RoundTripOfType_TestData_TestDataResolver()
    {
        TestData value = new(2);

        using ClipboardBinaryFormatterFullCompatScope scope = new();
        RoundTripOfType(value, TestDataResolver, out TestDataBase? result).Should().BeTrue();
        result.Should().BeOfType<TestData>().Subject.Equals(value);

        static Type TestDataResolver(TypeName typeName)
        {
            (string name, Type type)[] allowedTypes =
            [
                (typeof(TestData).FullName!, typeof(TestData)),
                (typeof(TestDataBase.InnerData).FullName!, typeof(TestDataBase.InnerData)),
                (typeof(NotSupportedException).FullName!, typeof(NotSupportedException)),
                (typeof(int?).FullName!, typeof(int?)),
                (typeof(DateTime?).FullName!, typeof(DateTime?)),
                ("System.Nullable`1[[System.Decimal, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]", typeof(decimal?)),
                ("System.Collections.Generic.List`1[[System.Nullable`1[[System.Decimal, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]", typeof(List<decimal?>)),
                ("System.Collections.Generic.List`1[[System.TimeSpan, System.Private.CoreLib, Version=10.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e]]", typeof(List<TimeSpan>))
            ];

            string fullName = typeName.FullName;
            foreach (var (name, type) in allowedTypes)
            {
                // Namespace-qualified type name.
                if (name == fullName)
                {
                    return type;
                }
            }

            // To get implicit binding for supported types, return null.
            return null!;
        }
    }

    [Fact]
    public void RoundTripOfType_TestData_InvalidResolver()
    {
        TestData value = new(2);

        using ClipboardBinaryFormatterFullCompatScope scope = new();
        WriteObjectToStream(value);

        // Resolver that returns a null is blocked in our SerializationBinder wrapper.
        Action read = () => TryReadObjectFromStream<TestData>(InvalidResolver, out _);

        read.Should().Throw<NotSupportedException>();

        static Type InvalidResolver(TypeName typeName) => null!;
    }

    [Fact]
    public void RoundTripOfType_FlatData_NoResolver()
    {
        TestDataBase.InnerData value = new("simple class");

        using ClipboardBinaryFormatterFullCompatScope scope = new();

        Action action = () => RoundTripOfType<TestDataBase.InnerData>(value, resolver: null, out var result);
        action.Should().Throw<NotSupportedException>();
    }

    [Fact]
    public void RoundTripOfType_FlatData_NrbfDeserializer_NoResolver()
    {
        TestDataBase.InnerData value = new("simple class");

        using BinaryFormatterScope scope = new(enable: true);
        using BinaryFormatterInClipboardDragDropScope clipboardScope = new(enable: true);

        Action action = () => RoundTripOfType<TestDataBase.InnerData>(value, resolver: null, out var result);
        action.Should().Throw<NotSupportedException>();
    }

    [Theory]
    [EnumData<DataType>]
    public void TryReadObjectFromStream_MyClass(DataType dataType)
    {
        using BinaryFormatterScope scope = new(enable: dataType == DataType.BinaryFormat);
        using BinaryFormatterInClipboardDragDropScope clipboardScope = new(enable: dataType == DataType.BinaryFormat);

        MyClass1 value = new(value: 1);
        using MemoryStream stream = CreateStream(dataType, value);

        DataRequest request = new()
        {
            Format = "test",
            TypedRequest = false
        };

        // Untyped requests must be object
        BinaryFormatUtilities.TryReadObjectFromStream(stream, in request, out object? result).Should().BeTrue();
        result.Should().BeEquivalentTo(value);

        request = new()
        {
            Format = "test",
            TypedRequest = true
        };

        Action action = () => BinaryFormatUtilities.TryReadObjectFromStream(stream, in request, out MyClass1? result);

        if (dataType == DataType.BinaryFormat)
        {
            action.Should().Throw<NotSupportedException>();
        }
        else
        {
            action.Should().NotThrow();
        }

        request = new()
        {
            Format = "test",
            TypedRequest = true,
            Resolver = (TypeName typeName) =>
            {
                if (typeof(MyClass1).Matches(typeName))
                {
                    return typeof(MyClass1);
                }

                if (typeof(MyClass2).Matches(typeName))
                {
                    return typeof(MyClass2);
                }

                // Let implicit handling work for other types.
                return null;
            }
        };

        BinaryFormatUtilities.TryReadObjectFromStream(stream, in request, out MyClass1? result2).Should().BeTrue();
        result2.Should().BeEquivalentTo(value);
    }

    [Theory]
    [BoolData]
    public void Sample_TryGetData_NoResolver_UseBinaryFormatter(bool restrictDeserialization)
    {
        MyClass1 value = new(value: 1);

        using ClipboardBinaryFormatterFullCompatScope scope = new();
        WriteObjectToStream(value);

        // PredefinedTypeDeserializationException will be swallowed up the call stack, when reading HGLOBAL.
        // Fails to resolve MyClass2 or both MyClass1 and MyClass2 in the case of Predefined formats.
        Action read = () => ReadObjectFromStream<MyClass1>(restrictDeserialization, resolver: null, out _);
        read.Should().Throw<Exception>();
    }

    [Theory]
    [BoolData]
    public void Sample_TryGetData_NoResolver_UseNrbfDeserializer(bool restrictDeserialization)
    {
        MyClass1 value = new(value: 1);

        using BinaryFormatterScope scope = new(enable: true);
        using BinaryFormatterInClipboardDragDropScope clipboardScope = new(enable: true);
        WriteObjectToStream(value);

        Action read = () => ReadObjectFromStream<MyClass1>(restrictDeserialization, resolver: null, out _);
        read.Should().Throw<Exception>();
    }

    [Fact]
    public void Sample_TryGetData_UseBinaryFormatter()
    {
        MyClass1 value = new(value: 1);

        using ClipboardBinaryFormatterFullCompatScope scope = new();
        WriteObjectToStream(value);

        ReadObjectFromStream<MyClass1>(restrictDeserialization: false, MyClass1.MyExactMatchResolver, out var result).Should().BeTrue();
        result.Should().BeEquivalentTo(value);
    }

    [Fact]
    public void Sample_TryGetData_PredefinedFormat_UseBinaryFormatter()
    {
        MyClass1 value = new(value: 1);

        using ClipboardBinaryFormatterFullCompatScope scope = new();
        WriteObjectToStream(value);

        Action read = () => ReadObjectFromStream<MyClass1>(restrictDeserialization: true, MyClass1.MyExactMatchResolver, out _);
        read.Should().Throw<Exception>();
    }

    [Fact]
    public void Sample_TryGetData_PredefinedFormat_UseNrbfDeserializer()
    {
        MyClass1 value = new(value: 1);

        using BinaryFormatterScope scope = new(enable: true);
        using BinaryFormatterInClipboardDragDropScope clipboardScope = new(enable: true);
        WriteObjectToStream(value);

        Action read = () => ReadObjectFromStream<MyClass1>(restrictDeserialization: true, MyClass1.MyExactMatchResolver, out _);
        read.Should().Throw<Exception>();
    }

    [Fact]
    public void Sample_TryGetData_UseNrbfDeserializer()
    {
        MyClass1 value = new(value: 1);

        using BinaryFormatterScope scope = new(enable: true);
        using BinaryFormatterInClipboardDragDropScope clipboardScope = new(enable: true);
        WriteObjectToStream(value);

        ReadObjectFromStream<MyClass1>(restrictDeserialization: false, MyClass1.MyExactMatchResolver, out var result).Should().BeTrue();
        result.Should().BeEquivalentTo(value);
    }

    [Serializable]
    private class TestDataBase
    {
        public TestDataBase()
        {
            Inner = new("inner");
        }

        public InnerData? Inner;

        [Serializable]
        internal class InnerData
        {
            public InnerData(string text)
            {
                Text = text;
                Location = new Point(1, 2);
            }

            public string Text;
            public Point Location;
        }
    }

    [Serializable]
    private class TestData : TestDataBase
    {
        public TestData(int count)
        {
            Count = count;
        }

        private const float Delta = 0.0003f;

        // BinaryFormatter resolves primitive types or arrays of primitive types with no resolver.
        public int? Count;
        public DateTime? Today = DateTime.Now;

        public byte[] ByteArray = [8, 9];
        public sbyte[] SbyteArray = [8, 9];
        public short[] ShortArray = [8, 9];
        public ushort[] UshortArray = [8, 9];
        public int[] IntArray = [8, 9];
        public uint[] UintArray = [8, 9];
        public long[] LongArray = [8, 9];
        public ulong[] UlongArray = [8, 9];
        public float[] FloatArray = [1.0f, 2.0f, 3.0f];
        public double[] DoubleArray = [1.0, 2.0, 3.0];
        public char[] CharArray = ['a', 'b', 'c'];
        public bool[] BoolArray = [true, false];
        public string[] StringArray = ["a", "b", "c"];
        public decimal[] DecimalArray = [1.0m, 2.0m, 3.0m];
        public TimeSpan[] TimeSpanArray = [TimeSpan.FromHours(1)];
        public DateTime[] DateTimeArray = [DateTime.Now];

        // Common WinForms types are resolved using the intrinsic binder.
        public NotSupportedException Exception = new();
        public Point Point = new(1, 2);
        public Rectangle Rectangle = new(1, 2, 3, 4);
        public Size? Size = new(1, 2);
        public SizeF SizeF = new(1, 2);
        public Color Color = Color.Red;
        public PointF PointF = new(1, 2);
        public RectangleF RectangleF = new(1, 2, 3, 4);

        public List<byte> Bytes = [1];
        public List<sbyte> Sbytes = [1];
        public List<short> Shorts = [1];
        public List<ushort> Ushorts = [1];
        public List<int> Ints = [1, 2, 3];
        public List<uint> Uints = [1, 2, 3];
        public List<long> Longs = [1, 2, 3];
        public List<ulong> Ulongs = [1, 2, 3];
        public List<float> Floats = [1.0f, 2.0f, 3.0f];
        public List<double> Doubles = [1.0, 2.0, 3.0];
        public List<decimal> Decimals = [1.0m, 2.0m, 3.0m];
        public List<decimal?> NullableDecimals = [null, 2.0m, 3.0m];
        public List<DateTime> DateTimes = [DateTime.Now];
        public List<TimeSpan> TimeSpans = [TimeSpan.FromHours(1)];
        public List<string> Strings = ["a", "b", "c"];

        public void Equals(TestData other)
        {
            Inner.Should().BeEquivalentTo(other.Inner);
            Count.Should().Be(other.Count);
            Today.Should().Be(other.Today);

            ByteArray.Should().BeEquivalentTo(other.ByteArray);
            SbyteArray.Should().BeEquivalentTo(other.SbyteArray);
            ShortArray.Should().BeEquivalentTo(other.ShortArray);
            UshortArray.Should().BeEquivalentTo(other.UshortArray);
            IntArray.Should().BeEquivalentTo(other.IntArray);
            UintArray.Should().BeEquivalentTo(other.UintArray);
            LongArray.Should().BeEquivalentTo(other.LongArray);
            UlongArray.Should().BeEquivalentTo(other.UlongArray);
            FloatArray.Should().BeEquivalentTo(other.FloatArray);
            DoubleArray.Should().BeEquivalentTo(other.DoubleArray);
            CharArray.Should().BeEquivalentTo(other.CharArray);
            BoolArray.Should().BeEquivalentTo(other.BoolArray);
            StringArray.Should().BeEquivalentTo(other.StringArray);
            DecimalArray.Should().BeEquivalentTo(other.DecimalArray);
            TimeSpanArray.Should().BeEquivalentTo(other.TimeSpanArray);
            DateTimeArray.Should().BeEquivalentTo(other.DateTimeArray);

            Exception.Should().BeEquivalentTo(other.Exception);
            Point.Should().Be(other.Point);
            Rectangle.Should().Be(other.Rectangle);
            Size.Should().Be(other.Size);
            SizeF.Should().Be(other.SizeF);
            Color.Should().Be(other.Color);
            PointF.Should().BeApproximately(other.PointF, Delta);
            RectangleF.Should().BeApproximately(other.RectangleF, Delta);
            Bytes.Should().BeEquivalentTo(other.Bytes);
            Sbytes.Should().BeEquivalentTo(other.Sbytes);
            Shorts.Should().BeEquivalentTo(other.Shorts);
            Ushorts.Should().BeEquivalentTo(other.Ushorts);
            Ints.Should().BeEquivalentTo(other.Ints);
            Uints.Should().BeEquivalentTo(other.Uints);
            Longs.Should().BeEquivalentTo(other.Longs);
            Ulongs.Should().BeEquivalentTo(other.Ulongs);
            Floats.Should().BeEquivalentTo(other.Floats);
            Doubles.Should().BeEquivalentTo(other.Doubles);
            Decimals.Should().BeEquivalentTo(other.Decimals);
            NullableDecimals.Should().BeEquivalentTo(other.NullableDecimals);
            DateTimes.Should().BeEquivalentTo(other.DateTimes);
            // TimeSpans.Should().BeEquivalentTo(other.TimeSpans);
            Strings.Should().BeEquivalentTo(other.Strings);
        }
    }

    [Serializable]
    private class MyClass1
    {
        public MyClass1(int value)
        {
            Value = value;
            MyClass2 = new();
        }

        public int Value { get; set; }
        public MyClass2 MyClass2 { get; set; }

        internal static Type MyExactMatchResolver(TypeName typeName)
        {
            // The preferred approach is to resolve types at build time to avoid assembly loading at runtime.
            (Type type, TypeName typeName)[] allowedTypes =
            [
                (typeof(MyClass1), TypeName.Parse(typeof(MyClass1).AssemblyQualifiedName)),
                (typeof(MyClass2), TypeName.Parse(typeof(MyClass2).AssemblyQualifiedName))
            ];

            foreach (var (type, name) in allowedTypes)
            {
                // Namespace-qualified type name, using case-sensitive comparison for C#.
                if (name.FullName != typeName.FullName)
                {
                    continue;
                }

                AssemblyNameInfo? info1 = typeName.AssemblyName;
                AssemblyNameInfo? info2 = name.AssemblyName;

                if (info1 is null && info2 is null)
                {
                    return type;
                }

                if (info1 is null || info2 is null)
                {
                    continue;
                }

                // Full assembly name comparison, case sensitive.
                if (info1.Name == info2.Name
                     && info1.Version == info2.Version
                     && ((info1.CultureName ?? string.Empty) == info2.CultureName)
                     && info1.PublicKeyOrToken.AsSpan().SequenceEqual(info2.PublicKeyOrToken.AsSpan()))
                {
                    return type;
                }
            }

            // Allow implicit binding for supported types by returning null.
            return null!;
        }
    }

    [Serializable]
    public class MyClass2
    {
        public MyClass2()
        {
            Point = new(1, 2);
        }

        public Point Point { get; set; } = new(1, 2);
    }
}
