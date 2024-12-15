// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.Collections;
using System.Drawing;
using System.Reflection.Metadata;
using System.Runtime.Serialization;
using Utilities = System.Windows.Forms.DataObject.Composition.BinaryFormatUtilities;

namespace System.Windows.Forms.Tests;

public partial class BinaryFormatUtilitiesTests : IDisposable
{
    private readonly MemoryStream _stream;

    public BinaryFormatUtilitiesTests() => _stream = new();

    public void Dispose() => _stream.Dispose();

    private void WriteObjectToStream(object value, bool restrictSerialization = false) =>
        Utilities.WriteObjectToStream(_stream, value, restrictSerialization);

    private object? ReadObjectFromStream()
    {
        _stream.Position = 0;
        return Utilities.ReadObjectFromStream<object>(_stream, resolver: null, legacyMode: true);
    }

    private object? ReadRestrictedObjectFromStream()
    {
        _stream.Position = 0;
        return Utilities.ReadRestrictedObjectFromStream<object>(_stream, resolver: null, legacyMode: true);
    }

    private object? ReadObjectFromStream<T>(bool restrictDeserialization, Func<TypeName, Type>? resolver)
    {
        _stream.Position = 0;
        return restrictDeserialization
            ? Utilities.ReadRestrictedObjectFromStream<T>(_stream, resolver, legacyMode: false)
            : Utilities.ReadObjectFromStream<T>(_stream, resolver, legacyMode: false);
    }

    private object? ReadObjectFromStream<T>(Func<TypeName, Type>? resolver)
    {
        _stream.Position = 0;
        return Utilities.ReadObjectFromStream<T>(_stream, resolver, legacyMode: false);
    }

    private object? ReadRestrictedObjectFromStream<T>(Func<TypeName, Type>? resolver)
    {
        _stream.Position = 0;
        return Utilities.ReadRestrictedObjectFromStream<T>(_stream, resolver, legacyMode: false);
    }

    private object? RoundTripObject(object value)
    {
        // This is equivalent to SetData/GetData methods with unbounded formats,
        // and works with the BinaryFormat AppContext switches.
        WriteObjectToStream(value);
        return ReadObjectFromStream();
    }

    private object? RoundTripObject_RestrictedFormat(object value)
    {
        // This is equivalent to SetData/GetData methods using registered OLE formats, resolves only the known types
        WriteObjectToStream(value, restrictSerialization: true);
        return ReadRestrictedObjectFromStream();
    }

    private object? RoundTripOfType<T>(object value)
    {
        // This is equivalent to SetData/TryGetData<T> methods using unbounded OLE formats,
        // and works with the BinaryFormat AppContext switches.
        WriteObjectToStream(value);
        return ReadObjectFromStream<T>(NotSupportedResolver);
    }

    private object? RoundTripOfType_RestrictedFormat<T>(object value)
    {
        // This is equivalent to SetData/TryGetData<T> methods using OLE formats. Deserialization is restricted
        // to known types.
        WriteObjectToStream(value, restrictSerialization: true);
        return ReadRestrictedObjectFromStream<T>(NotSupportedResolver);
    }

    private object? RoundTripOfType<T>(object value, Func<TypeName, Type>? resolver)
    {
        // This is equivalent to SetData/TryGetData<T> methods using unbounded formats,
        // serialization is restricted by the resolver and BinaryFormat AppContext switches.
        WriteObjectToStream(value);
        return ReadObjectFromStream<T>(resolver);
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
    public void RoundTrip_Simple(object value) =>
        RoundTripObject(value).Should().Be(value);

    [Theory]
    [MemberData(nameof(PrimitiveObjects_TheoryData))]
    [MemberData(nameof(KnownObjects_TheoryData))]
    public void RoundTrip_RestrictedFormat_Simple(object value) =>
        RoundTripObject_RestrictedFormat(value).Should().Be(value);

    [Theory]
    [MemberData(nameof(NotSupportedException_TestData))]
    public void RoundTrip_NotSupportedException(NotSupportedException value) =>
        RoundTripObject(value).Should().BeEquivalentTo(value);

    [Theory]
    [MemberData(nameof(NotSupportedException_TestData))]
    public void RoundTrip_RestrictedFormat_NotSupportedException(NotSupportedException value) =>
        RoundTripObject_RestrictedFormat(value).Should().BeEquivalentTo(value);

    [Fact]
    public void RoundTrip_NotSupportedException_DataLoss()
    {
        NotSupportedException value = new("Error message", new ArgumentException());
        RoundTripObject(value).Should().BeEquivalentTo(new NotSupportedException("Error message", innerException: null));
    }

    [Fact]
    public void RoundTrip_RestrictedFormat_NotSupportedException_DataLoss()
    {
        NotSupportedException value = new("Error message", new ArgumentException());
        RoundTripObject_RestrictedFormat(value).Should().BeEquivalentTo(new NotSupportedException("Error message", innerException: null));
    }

    [Theory]
    [MemberData(nameof(PrimitiveListObjects_TheoryData))]
    public void RoundTrip_PrimitiveList(IList value) =>
        RoundTripObject(value).Should().BeEquivalentTo(value);

    [Theory]
    [MemberData(nameof(PrimitiveListObjects_TheoryData))]
    public void RoundTrip_RestrictedFormat_PrimitiveList(IList value) =>
        RoundTripObject_RestrictedFormat(value).Should().BeEquivalentTo(value);

    [Theory]
    [MemberData(nameof(PrimitiveArrayObjects_TheoryData))]
    public void RoundTrip_PrimitiveArray(Array value) =>
        RoundTripObject(value).Should().BeEquivalentTo(value);

    [Theory]
    [MemberData(nameof(PrimitiveArrayListObjects_TheoryData))]
    public void RoundTrip_PrimitiveArrayList(ArrayList value) =>
        RoundTripObject(value).Should().BeEquivalentTo(value);

    [Theory]
    [MemberData(nameof(PrimitiveArrayListObjects_TheoryData))]
    public void RoundTrip_RestrictedFormat_PrimitiveArrayList(ArrayList value) =>
        RoundTripObject_RestrictedFormat(value).Should().BeEquivalentTo(value);

    [Theory]
    [MemberData(nameof(PrimitiveTypeHashtables_TheoryData))]
    public void RoundTrip_PrimitiveHashtable(Hashtable value) =>
        RoundTripObject(value).Should().BeEquivalentTo(value);

    [Theory]
    [MemberData(nameof(PrimitiveTypeHashtables_TheoryData))]
    public void RoundTrip_RestrictedFormat_PrimitiveHashtable(Hashtable value) =>
        RoundTripObject_RestrictedFormat(value).Should().BeEquivalentTo(value);

    [Fact]
    public void RoundTrip_ImageList()
    {
        using ImageList sourceList = new();
        using Bitmap image = new(10, 10);
        sourceList.Images.Add(image);
        using ImageListStreamer value = sourceList.ImageStream!;

        var result = RoundTripObject(value).Should().BeOfType<ImageListStreamer>().Which;

        using ImageList newList = new();
        newList.ImageStream = result;
        newList.Images.Count.Should().Be(1);
    }

    [Fact]
    public void RoundTrip_RestrictedFormat_ImageList()
    {
        using ImageList sourceList = new();
        using Bitmap image = new(10, 10);
        sourceList.Images.Add(image);
        using ImageListStreamer value = sourceList.ImageStream!;

        var result = RoundTripObject_RestrictedFormat(value).Should().BeOfType<ImageListStreamer>().Which;

        using ImageList newList = new();
        newList.ImageStream = result;
        newList.Images.Count.Should().Be(1);
    }

    [Fact]
    public void RoundTrip_Bitmap()
    {
        using Bitmap value = new(10, 10);
        RoundTripObject(value).Should().BeOfType<Bitmap>().Which.Size.Should().Be(value.Size);
    }

    [Fact]
    public void RoundTrip_RestrictedFormat_Bitmap()
    {
        using Bitmap value = new(10, 10);
        RoundTripObject_RestrictedFormat(value).Should().BeOfType<Bitmap>().Which.Size.Should().Be(value.Size);
    }

    [Theory]
    [MemberData(nameof(Lists_UnsupportedTestData))]
    public void RoundTrip_Unsupported(IList value)
    {
        Action writer = () => WriteObjectToStream(value);
        Action reader = () => ReadObjectFromStream();

        writer.Should().Throw<NotSupportedException>();

        using (NrbfSerializerInClipboardDragDropScope nrbfScope = new(enable: false))
        {
            using (BinaryFormatterScope scope = new(enable: true))
            {
                writer.Should().Throw<NotSupportedException>();

                using BinaryFormatterInClipboardDragDropScope clipboardDragDropScope = new(enable: true);
                WriteObjectToStream(value);
                ReadObjectFromStream().Should().BeEquivalentTo(value);
            }

            reader.Should().Throw<NotSupportedException>();
        }

        // Binary format deserializers in Clipboard/DragDrop scenarios are not opted in.
        reader.Should().Throw<NotSupportedException>();
    }

    [Theory]
    [MemberData(nameof(Lists_UnsupportedTestData))]
    public void RoundTrip_RestrictedFormat_Unsupported(IList value)
    {
        Action writer = () => WriteObjectToStream(value, restrictSerialization: true);
        writer.Should().Throw<SerializationException>();

        using BinaryFormatterFullCompatScope scope = new();
        writer.Should().Throw<SerializationException>();
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
        using BinaryFormatterFullCompatScope scope = new();
        var result = RoundTripObject(value).Should().BeOfType<uint[,]>().Subject;

        result.Rank.Should().Be(2);
        result.GetLength(0).Should().Be(2);
        result.GetLength(1).Should().Be(3);
        result.GetLowerBound(0).Should().Be(1);
        result.GetLowerBound(1).Should().Be(2);
        result.GetValue(1, 2).Should().Be(101u);
        result.GetValue(1, 3).Should().Be(102u);
        result.GetValue(1, 4).Should().Be(103u);
        result.GetValue(2, 2).Should().Be(201u);
        result.GetValue(2, 3).Should().Be(202u);
        result.GetValue(2, 4).Should().Be(203u);
    }

    [Fact]
    public void RoundTripOfType_Unsupported()
    {
        // Not a known type, while 'List<object>' is resolved by default, 'object' requires a custom resolver.
        List<object> value = ["text"];
        using (BinaryFormatterFullCompatScope scope = new())
        {
            WriteObjectToStream(value);

            ReadAndValidate();

            using NrbfSerializerInClipboardDragDropScope nrbfScope = new(enable: true);
            ReadAndValidate();
        }

        Action read = () => ReadObjectFromStream<List<object>>(ObjectListResolver);
        read.Should().Throw<NotSupportedException>();

        void ReadAndValidate()
        {
            var result = ReadObjectFromStream<List<object>>(ObjectListResolver)
                .Should().BeOfType<List<object>>().Subject;
            result.Count.Should().Be(1);
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

    [Fact]
    public void RoundTripOfType_AsUnmatchingType_Simple()
    {
        List<int> value = [1, 2, 3];
        RoundTripOfType<Control>(value).Should().BeNull();
    }

    [Fact]
    public void RoundTripOfType_RestrictedFormat_AsUnmatchingType_Simple()
    {
        Rectangle value = new(1, 1, 2, 2);
        // We are setting up an invalid content scenario, Rectangle type can't be read as a restricted format,
        // but in this case requested type will not match the payload type.
        WriteObjectToStream(value);

        ReadRestrictedObjectFromStream<Control>(NotSupportedResolver).Should().BeNull();

        using BinaryFormatterFullCompatScope scope = new();
        ReadRestrictedObjectFromStream<Control>(NotSupportedResolver).Should().BeNull();
    }

    [Fact]
    public void RoundTripOfType_intNullable() =>
        RoundTripOfType<int?>(101, NotSupportedResolver).Should().Be(101);

    [Fact]
    public void RoundTripOfType_RestrictedFormat_intNullable() =>
        RoundTripOfType_RestrictedFormat<int?>(101).Should().Be(101);

    [Fact]
    public void RoundTripOfType_RestrictedFormat_intNullableArray_NotSupportedResolver()
    {
        int?[] value = [101, null, 303];

        using BinaryFormatterFullCompatScope scope = new();
        WriteObjectToStream(value);
        Action read = () => ReadRestrictedObjectFromStream<int?[]>(NotSupportedResolver);

        // nullable struct requires a custom resolver.
        // RestrictedTypeDeserializationException
        read.Should().Throw<Exception>();
    }

    [Fact]
    public void RoundTripOfType_intNullableArray_NotSupportedResolver()
    {
        int?[] value = [101, null, 303];

        using BinaryFormatterFullCompatScope scope = new();
        WriteObjectToStream(value);
        Action read = () => ReadObjectFromStream<int?[]>(NotSupportedResolver);

        // nullable struct requires a custom resolver.
        // This is either NotSupportedException of RestrictedTypeDeserializationException, depending on format.
        read.Should().Throw<Exception>();
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

        using BinaryFormatterFullCompatScope scope = new();
        WriteObjectToStream(value);
        Action read = () => ReadObjectFromStream<uint[,]>(restrictDeserialization, NotSupportedResolver);

        read.Should().Throw<Exception>();
    }

    [Fact]
    public void RoundTripOfType_intNullableArray_CustomResolver()
    {
        int?[] value = [101, null, 303];

        using BinaryFormatterFullCompatScope scope = new();
        RoundTripOfType<int?[]>(value, NullableIntArrayResolver).Should().BeEquivalentTo(value);
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
        TestData value = new(new(10, 10), 2);

        using BinaryFormatterFullCompatScope scope = new();
        var result = RoundTripOfType<TestDataBase>(value, TestDataResolver).Should().BeOfType<TestData>().Subject;

        result.Equals(value, value.Bitmap.Size);

        static Type TestDataResolver(TypeName typeName)
        {
            (string name, Type type)[] allowedTypes =
            [
                (typeof(TestData).FullName!, typeof(TestData)),
                (typeof(TestDataBase.InnerData).FullName!, typeof(TestDataBase.InnerData)),
                ("System.Nullable`1[[System.Decimal, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]", typeof(decimal?)),
                ("System.Collections.Generic.List`1[[System.Nullable`1[[System.Decimal, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]", typeof(List<decimal?>))
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

    [Fact]
    public void RoundTripOfType_TestData_InvalidResolver()
    {
        TestData value = new(new(10, 10), 2);

        using BinaryFormatterFullCompatScope scope = new();
        WriteObjectToStream(value);

        // Resolver that returns a null is blocked in our SerializationBinder wrapper.
        Action read = () => ReadObjectFromStream<TestData>(InvalidResolver);

        read.Should().Throw<SerializationException>();

        static Type InvalidResolver(TypeName typeName) => null!;
    }

    private static Type FontResolver(TypeName typeName)
    {
        (string? name, Type type)[] allowedTypes =
        [
            (typeof(Font).FullName, typeof(Font)),
            (typeof(FontStyle).FullName, typeof(FontStyle)),
            (typeof(FontFamily).FullName, typeof(FontFamily)),
            (typeof(GraphicsUnit).FullName, typeof(GraphicsUnit)),
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
    public void RoundTripOfType_Font_FontResolver()
    {
        using Font value = new("Microsoft Sans Serif", emSize: 10);

        using BinaryFormatterFullCompatScope scope = new();

        using Font result = RoundTripOfType<Font>(value, FontResolver).Should().BeOfType<Font>().Subject;
        result.Should().Be(value);
    }

    [Fact]
    public void ReadFontSerializedOnNet481()
    {
        // This string was generated on net481.
        // Clipboard.SetData("TestData", new Font("Microsoft Sans Serif", 10));
        // And the resulting stream was saved as a string
        // string text = Convert.ToBase64String(stream.ToArray());
        string font =
            "AAEAAAD/////AQAAAAAAAAAMAgAAAFFTeXN0ZW0uRHJhd2luZywgVmVyc2lvbj00LjAuMC4wLCBDdWx0dXJl"
            + "PW5ldXRyYWwsIFB1YmxpY0tleVRva2VuPWIwM2Y1ZjdmMTFkNTBhM2EFAQAAABNTeXN0ZW0uRHJhd2luZy5G"
            + "b250BAAAAAROYW1lBFNpemUFU3R5bGUEVW5pdAEABAQLGFN5c3RlbS5EcmF3aW5nLkZvbnRTdHlsZQIAAAAb"
            + "U3lzdGVtLkRyYXdpbmcuR3JhcGhpY3NVbml0AgAAAAIAAAAGAwAAABRNaWNyb3NvZnQgU2FucyBTZXJpZgAA"
            + "IEEF/P///xhTeXN0ZW0uRHJhd2luZy5Gb250U3R5bGUBAAAAB3ZhbHVlX18ACAIAAAAAAAAABfv///8bU3lz"
            + "dGVtLkRyYXdpbmcuR3JhcGhpY3NVbml0AQAAAAd2YWx1ZV9fAAgCAAAAAwAAAAs=";

        byte[] bytes = Convert.FromBase64String(font);
        using MemoryStream stream = new(bytes);

        // Default deserialization with the NRBF deserializer.
        using (BinaryFormatterInClipboardDragDropScope binaryFormatScope = new(enable: true))
        {
            // GetData case.
            stream.Position = 0;
            Action getData = () => Utilities.ReadObjectFromStream<object>(
               stream,
               resolver: null,
               legacyMode: true);

            getData.Should().Throw<NotSupportedException>();

            TryGetData(stream);
        }

        // Deserialize using the binary formatter.
        using BinaryFormatterFullCompatScope scope = new();
        // GetData case.
        stream.Position = 0;
        var result = Utilities.ReadObjectFromStream<object>(
           stream,
           resolver: null,
           legacyMode: true).Should().BeOfType<Font>().Subject;

        result.Name.Should().Be("Microsoft Sans Serif");
        result.Size.Should().Be(10);

        TryGetData(stream);

        static void TryGetData(MemoryStream stream)
        {
            // TryGetData<Font> case.
            stream.Position = 0;
            var result = Utilities.ReadObjectFromStream<Font>(
                stream,
                resolver: FontResolver,
                legacyMode: false).Should().BeOfType<Font>().Subject;

            result.Name.Should().Be("Microsoft Sans Serif");
            result.Size.Should().Be(10);
        }
    }

    [Fact]
    public void RoundTripOfType_FlatData_NoResolver()
    {
        TestDataBase.InnerData value = new("simple class");

        using BinaryFormatterFullCompatScope scope = new();

        RoundTripOfType<TestDataBase.InnerData>(value, resolver: null)
            .Should().BeOfType<TestDataBase.InnerData>().Which.Should().BeEquivalentTo(value);
    }

    [Fact]
    public void RoundTripOfType_FlatData_NrbfDeserializer_NoResolver()
    {
        TestDataBase.InnerData value = new("simple class");

        using BinaryFormatterScope scope = new(enable: true);
        using BinaryFormatterInClipboardDragDropScope clipboardScope = new(enable: true);

        RoundTripOfType<TestDataBase.InnerData>(value, resolver: null)
            .Should().BeOfType<TestDataBase.InnerData>().Which.Should().BeEquivalentTo(value);
    }

    [Fact]
    public void Sample_GetData_UseBinaryFormatter()
    {
        MyClass1 value = new(value: 1);

        using BinaryFormatterFullCompatScope scope = new();
        WriteObjectToStream(value);

        // legacyMode == true follows the GetData path.
        _stream.Position = 0;
        Utilities.ReadObjectFromStream<MyClass1>(_stream, resolver: null, legacyMode: true)
            .Should().BeEquivalentTo(value);
    }

    [Fact]
    public void Sample_GetData_UseNrbfDeserialize()
    {
        MyClass1 value = new(value: 1);

        using BinaryFormatterScope scope = new(enable: true);
        using BinaryFormatterInClipboardDragDropScope clipboardScope = new(enable: true);
        WriteObjectToStream(value);

        // This works because GetData falls back to the BinaryFormatter deserializer, NRBF deserializer fails because it requires a resolver.
        _stream.Position = 0;
        Utilities.ReadObjectFromStream<MyClass1>(_stream, resolver: null, legacyMode: true)
            .Should().BeEquivalentTo(value);
    }

    [Theory]
    [BoolData]
    public void Sample_TryGetData_NoResolver_UseBinaryFormatter(bool restrictDeserialization)
    {
        MyClass1 value = new(value: 1);

        using BinaryFormatterFullCompatScope scope = new();
        WriteObjectToStream(value);

        // RestrictedTypeDeserializationException will be swallowed up the call stack, when reading HGLOBAL.
        // Fails to resolve MyClass2 or both MyClass1 and MyClass2 in the case of restricted formats.
        Action read = () => ReadObjectFromStream<MyClass1>(restrictDeserialization, resolver: null);
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

        Action read = () => ReadObjectFromStream<MyClass1>(restrictDeserialization, resolver: null);
        read.Should().Throw<Exception>();
    }

    [Fact]
    public void Sample_TryGetData_UseBinaryFormatter()
    {
        MyClass1 value = new(value: 1);

        using BinaryFormatterFullCompatScope scope = new();
        WriteObjectToStream(value);

        ReadObjectFromStream<MyClass1>(restrictDeserialization: false, MyClass1.MyExactMatchResolver)
            .Should().BeEquivalentTo(value);
    }

    [Fact]
    public void Sample_TryGetData_RestrictedFormat_UseBinaryFormatter()
    {
        MyClass1 value = new(value: 1);

        using BinaryFormatterFullCompatScope scope = new();
        WriteObjectToStream(value);

        Action read = () => ReadObjectFromStream<MyClass1>(restrictDeserialization: true, MyClass1.MyExactMatchResolver);
        read.Should().Throw<Exception>();
    }

    [Fact]
    public void Sample_TryGetData_RestrictedFormat_UseNrbfDeserializer()
    {
        MyClass1 value = new(value: 1);

        using BinaryFormatterScope scope = new(enable: true);
        using BinaryFormatterInClipboardDragDropScope clipboardScope = new(enable: true);
        WriteObjectToStream(value);

        Action read = () => ReadObjectFromStream<MyClass1>(restrictDeserialization: true, MyClass1.MyExactMatchResolver);
        read.Should().Throw<Exception>();
    }

    [Fact]
    public void Sample_TryGetData_UseNrbfDeserializer()
    {
        MyClass1 value = new(value: 1);

        using BinaryFormatterScope scope = new(enable: true);
        using BinaryFormatterInClipboardDragDropScope clipboardScope = new(enable: true);
        WriteObjectToStream(value);

        ReadObjectFromStream<MyClass1>(restrictDeserialization: false, MyClass1.MyExactMatchResolver)
            .Should().BeEquivalentTo(value);
    }

    private static Type NotSupportedResolver(TypeName typeName) =>
        throw new NotSupportedException($"Can't resolve {typeName.AssemblyQualifiedName}");

    [Serializable]
    private class TestDataBase
    {
        public TestDataBase(Bitmap bitmap)
        {
            Bitmap = bitmap;
            Inner = new("inner");
        }

        public Bitmap Bitmap;
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
        public TestData(Bitmap bitmap, int count)
            : base(bitmap)
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
        public ImageListStreamer ImageList = new(new ImageList());

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
        // System.Runtime.Serialization.SerializationException : Invalid BinaryFormatter stream.
        // System.NotSupportedException : Can't resolve System.Collections.Generic.List`1[[System.TimeSpan, System.Private.CoreLib, Version=10.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e]]
        // Even though when serialized as a root record, TimeSpan is normalized to the framework assembly.
        // public List<TimeSpan> TimeSpans = new() { TimeSpan.FromHours(1) };
        public List<string> Strings = ["a", "b", "c"];

        public void Equals(TestData other, Size bitmapSize)
        {
            Bitmap.Size.Should().Be(bitmapSize);
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
            using ImageList newList = new();
            newList.ImageStream = ImageList;
            newList.Images.Count.Should().Be(0);
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

            throw new NotSupportedException($"Can't resolve {typeName.AssemblyQualifiedName}");
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
