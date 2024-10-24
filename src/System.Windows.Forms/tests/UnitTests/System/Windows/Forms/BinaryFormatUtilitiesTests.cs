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

    private object? ReadObjectFromStream(bool restrictDeserialization = false)
    {
        _stream.Position = 0;
        return Utilities.ReadObjectFromStream<object>(
            _stream,
            resolver: null,
            restrictDeserialization,
            legacyMode: true);
    }

    private object? ReadObjectFromStream<T>(Func<TypeName, Type> resolver, bool restrictDeserialization = false)
    {
        _stream.Position = 0;
        return Utilities.ReadObjectFromStream<T>(_stream, resolver, restrictDeserialization, legacyMode: false);
    }

    private object? RoundTripObject(object value)
    {
        // This is equivalent to SetData/GetData methods with unbounded formats, and works with the BF AppCompat switches.
        WriteObjectToStream(value);
        return ReadObjectFromStream();
    }

    private object? RoundTripObject_RestrictedFormat(object value)
    {
        // This is equivalent to SetData/GetData methods using registered OLE formats and thus the BitmapBinder,
        // and works with the BF AppCompat switches.
        WriteObjectToStream(value, restrictSerialization: true);
        return ReadObjectFromStream(restrictDeserialization: true);
    }

    private object? RoundTripOfType<T>(object value)
    {
        // This is equivalent to SetData/TryGetData<T> methods using unbounded OLE formats,
        // and works with the BinaryFormatter AppCompat switches.
        WriteObjectToStream(value);
        return ReadObjectFromStream<T>(DataObject.NotSupportedResolver);
    }

    private object? RoundTripOfType_RestrictedFormat<T>(object value)
    {
        // This is equivalent to SetData/TryGetData<T> methods using OLE formats. Deserialization is restricted by
        // BitmapBinder and the BF AppCompat switches.
        WriteObjectToStream(value, restrictSerialization: true);
        return ReadObjectFromStream<T>(DataObject.NotSupportedResolver, restrictDeserialization: true);
    }

    private object? RoundTripOfType<T>(object value, Func<TypeName, Type> resolver)
    {
        // This is equivalent to SetData/TryGetData<T> methods using unbounded formats,
        // serialization is restricted by the resolver and BF AppCompat switches.
        WriteObjectToStream(value);
        return ReadObjectFromStream<T>(resolver);
    }

    // Primitive types as defined by the NRBF spec.
    // https://learn.microsoft.com/dotnet/api/system.formats.nrbf.primitivetyperecord
    public static TheoryData<object> PrimitiveObjects_TheoryData => new()
    {
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
    };

    public static TheoryData<object> KnownObjects_TheoryData => new()
    {
        -(nint)11,
        (nuint)12,
        new PointF(1, 2),
        new RectangleF(1, 2, 3, 4),
        new Point(-1, int.MaxValue),
        new Rectangle(-1, int.MinValue, 10, 13),
        new Size(int.MaxValue, int.MinValue),
        new SizeF(float.MaxValue, float.MinValue),
        Color.Red
    };

    public static TheoryData<IList> PrimitiveListObjects_TheoryData => new()
    {
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
    };

    public static TheoryData<Array> PrimitiveArrayObjects_TheoryData => new()
    {
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
    };

    public static TheoryData<ArrayList> PrimitiveArrayListObjects_TheoryData => new()
    {
        new ArrayList { null },
        new ArrayList { null, "something" },
        new ArrayList { false, true },
        new ArrayList { char.MinValue, char.MaxValue },
        new ArrayList { byte.MinValue, byte.MaxValue },
        new ArrayList { sbyte.MinValue, sbyte.MaxValue },
        new ArrayList { short.MinValue, short.MaxValue },
        new ArrayList { ushort.MinValue, ushort.MaxValue },
        new ArrayList { int.MinValue, int.MaxValue },
        new ArrayList { uint.MinValue, uint.MaxValue },
        new ArrayList { long.MinValue, long.MaxValue },
        new ArrayList { ulong.MinValue, ulong.MaxValue },
        new ArrayList { float.MinValue, float.MaxValue },
        new ArrayList { double.MinValue, double.MaxValue },
        new ArrayList { decimal.MinValue, decimal.MaxValue },
        new ArrayList { DateTime.MinValue, DateTime.MaxValue },
        new ArrayList { TimeSpan.MinValue, TimeSpan.MaxValue },
        new ArrayList { "a", "b", "c" }
    };

    public static TheoryData<Hashtable> PrimitiveTypeHashtables_TheoryData => new()
    {
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
    };

    public static TheoryData<NotSupportedException> NotSupportedException_TestData => new()
    {
        new NotSupportedException(),
        new NotSupportedException("Error message"),
        new NotSupportedException(null)
    };

    public static TheoryData<IList> Lists_UnsupportedTestData => new()
    {
        new List<object>(),
        new List<nint>(),
        new List<(int, int)>(),
        new List<nint> { nint.MinValue, nint.MaxValue },
        new List<nuint> { nuint.MinValue, nuint.MaxValue }
    };

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
        RoundTripObject(value).Should().BeOfType<Bitmap>().Subject.Size.Should().Be(value.Size);
    }

    [Fact]
    public void RoundTrip_RestrictedFormat_Bitmap()
    {
        using Bitmap value = new(10, 10);
        RoundTripObject_RestrictedFormat(value).Should().BeOfType<Bitmap>().Subject.Size.Should().Be(value.Size);
    }

    [Theory]
    [MemberData(nameof(Lists_UnsupportedTestData))]
    public void RoundTrip_Unsupported(IList value)
    {
        ((Action)(() => WriteObjectToStream(value))).Should().Throw<NotSupportedException>();

        using (BinaryFormatterScope scope = new(enable: true))
        using (BinaryFormatterInClipboardScope clipboardScope = new(enable: true))
        {
            WriteObjectToStream(value);
            ReadObjectFromStream().Should().BeEquivalentTo(value);
        }

        // Doesn't attempt to access BinaryFormatter.
        ReadObjectFromStream().Should().BeNull();
    }

    [Theory]
    [MemberData(nameof(Lists_UnsupportedTestData))]
    public void RoundTrip_RestrictedFormat_Unsupported(IList value)
    {
        ((Action)(() => WriteObjectToStream(value, restrictSerialization: true))).Should().Throw<NotSupportedException>();

        using BinaryFormatterScope scope = new(enable: true);
        using BinaryFormatterInClipboardScope clipboardScope = new(enable: true);
        ((Action)(() => WriteObjectToStream(value, restrictSerialization: true))).Should().Throw<SerializationException>();
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

        using BinaryFormatterScope scope = new(enable: true);
        using BinaryFormatterInClipboardScope clipboardScope = new(enable: true);
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
        List<object> value = new() { "text" };
        using (BinaryFormatterScope scope = new(enable: true))
        using (BinaryFormatterInClipboardScope clipboardScope = new(enable: true))
        {
            WriteObjectToStream(value);

            var result = ReadObjectFromStream<List<object>>(ObjectResolver).Should().BeOfType<List<object>>().Subject;
            result.Count.Should().Be(1);
            result[0].Should().Be("text");
        }

        ReadObjectFromStream<List<object>>(ObjectResolver).Should().BeNull();
    }

    private static Type ObjectResolver(TypeName typeName)
    {
        if (typeof(object).FullName! == typeName.FullName)
        {
            return typeof(object);
        }

        throw new NotSupportedException($"Can't resolve {typeName.FullName}");
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

        ReadObjectFromStream<Control>(DataObject.NotSupportedResolver, restrictDeserialization: true).Should().BeNull();

        using BinaryFormatterScope scope = new(enable: true);
        using BinaryFormatterInClipboardScope clipboardScope = new(enable: true);
        ReadObjectFromStream<Control>(DataObject.NotSupportedResolver, restrictDeserialization: true).Should().BeNull();
    }

    [Fact]
    public void RoundTripOfType_intNullable() =>
        RoundTripOfType<int?>(101, DataObject.NotSupportedResolver).Should().Be(101);

    [Fact]
    public void RoundTripOfType_RestrictedFormat_intNullable() =>
        RoundTripOfType_RestrictedFormat<int?>(101).Should().Be(101);

    [Fact]
    public void RoundTripOfType_intNullableArray_DefaultResolver()
    {
        int?[] value = [101, null, 303];

        using BinaryFormatterScope scope = new(enable: true);
        using BinaryFormatterInClipboardScope clipboardScope = new(enable: true);
        ((Action)(() => RoundTripOfType<int?[]>(value))).Should().Throw<SerializationException>();
    }

    [Fact]
    public void RoundTripOfType_RestrictedFormat_intNullableArray_DefaultResolver()
    {
        int?[] value = [101, null, 303];

        using BinaryFormatterScope scope = new(enable: true);
        using BinaryFormatterInClipboardScope clipboardScope = new(enable: true);
        ((Action)(() => RoundTripOfType_RestrictedFormat<int?[]>(value))).Should().Throw<SerializationException>();
    }

    [Fact]
    public void RoundTripOfType_OffsetArray_DefaultResolver()
    {
        Array value = Array.CreateInstance(typeof(uint), lengths: [2, 3], lowerBounds: [1, 2]);
        value.SetValue(101u, 1, 2);
        value.SetValue(102u, 1, 3);
        value.SetValue(103u, 1, 4);
        value.SetValue(201u, 2, 2);
        value.SetValue(202u, 2, 3);
        value.SetValue(203u, 2, 4);

        using BinaryFormatterScope scope = new(enable: true);
        using BinaryFormatterInClipboardScope clipboardScope = new(enable: true);
        ((Action)(() => RoundTripOfType<uint[,]>(value))).Should().Throw<NotSupportedException>();
    }

    [Fact]
    public void RoundTripOfType_intNullableArray_CustomResolver()
    {
        int?[] value = [101, null, 303];

        using BinaryFormatterScope scope = new(enable: true);
        using BinaryFormatterInClipboardScope clipboardScope = new(enable: true);
        RoundTripOfType<int?[]>(value, NullableIntArrayResolver).Should().BeEquivalentTo(value);
    }

    private static Type NullableIntArrayResolver(TypeName typeName)
    {
        (string name, Type type)[] allowedTypes =
        [
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

        throw new NotSupportedException($"Can't resolve {fullName}");
    }

    [Fact]
    public void RoundTripOfType_TestData_TestDataResolver()
    {
        TestData value = new(new(10, 10), 2);

        using BinaryFormatterScope scope = new(enable: true);
        using BinaryFormatterInClipboardScope clipboardScope = new(enable: true);

        var result = RoundTripOfType<TestDataBase>(value, TestDataResolver).Should().BeOfType<TestData>().Subject;

        result.Equals(value, value.Bitmap.Size);
    }

    [Fact]
    public void RoundTripOfType_TestData_InvalidResolver()
    {
        TestData value = new(new(10, 10), 2);

        using BinaryFormatterScope scope = new(enable: true);
        using BinaryFormatterInClipboardScope clipboardScope = new(enable: true);

        WriteObjectToStream(value);

        // Resolver that returns a null is blocked in our SerializationBinder wrapper.
        ((Action)(() => ReadObjectFromStream<TestData>(InvalidResolver))).Should().Throw<SerializationException>();
    }

    private static Type InvalidResolver(TypeName typeName) => null!;

    [Fact]
    public void RoundTripOfType_Font_FontResolver()
    {
        using Font value = new("Microsoft Sans Serif", emSize: 10);

        using BinaryFormatterScope scope = new(enable: true);
        using BinaryFormatterInClipboardScope clipboardScope = new(enable: true);

        using Font result = RoundTripOfType<Font>(value, FontResolver).Should().BeOfType<Font>().Subject;
        result.Should().Be(value);
    }

    private static Type FontResolver(TypeName typeName)
    {
        (string name, Type type)[] allowedTypes =
        [
            (typeof(FontStyle).FullName!, typeof(FontStyle)),
            (typeof(FontFamily).FullName!, typeof(FontFamily)),
            (typeof(GraphicsUnit).FullName!, typeof(GraphicsUnit)),
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

        throw new NotSupportedException($"Can't resolve {fullName}");
    }

    [Fact]
    public void RoundTripOfType_FlatData_DefaultResolver()
    {
        TestDataBase.InnerData value = new("simple class");

        using BinaryFormatterScope scope = new(enable: true);
        using BinaryFormatterInClipboardScope clipboardScope = new(enable: true);

        var result = RoundTripOfType<TestDataBase.InnerData>(value).Should().BeOfType<TestDataBase.InnerData>().Subject;

        result.Text.Should().Be("simple class");
    }

    [Serializable]
    private class TestDataBase
    {
        public TestDataBase(Bitmap bitmap)
        {
            Bitmap = bitmap;
            Inner = new InnerData("inner");
        }

        public Bitmap Bitmap;
        public InnerData? Inner;

        [Serializable]
        internal class InnerData
        {
            public InnerData(string text)
            {
                Text = text;
            }

            public string Text;
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

        // BinaryFormatter resolves primitive types or arrays of primitive types with no callback to the resolver.
        public int? Count;
        public DateTime? Today = DateTime.Now;
        public float[] FloatArray = [1.0f, 2.0f, 3.0f];
        public TimeSpan[] TimeSpanArray = [TimeSpan.FromHours(1)];

        // Common WinForms types are resolved using the binder based on the provided resolver.
        public NotSupportedException Exception = new();
        public Point Point = new(1, 2);
        public Rectangle Rectangle = new(1, 2, 3, 4);
        public Size? Size = new(1, 2);
        public SizeF SizeF = new(1, 2);
        public Color Color = Color.Red;
        public PointF PointF = new(1, 2);
        public RectangleF RectangleF = new(1, 2, 3, 4);
        public ImageListStreamer ImageList = new(new ImageList());

        public List<byte> Bytes = new() { 1 };
        public List<sbyte> Sbytes = new() { 1 };
        public List<short> Shorts = new() { 1 };
        public List<ushort> Ushorts = new() { 1 };
        public List<int> Ints = new() { 1, 2, 3 };
        public List<uint> Uints = new() { 1, 2, 3 };
        public List<long> Longs = new() { 1, 2, 3 };
        public List<ulong> Ulongs = new() { 1, 2, 3 };
        public List<float> Floats = new() { 1.0f, 2.0f, 3.0f };
        public List<double> Doubles = new() { 1.0, 2.0, 3.0 };
        public List<decimal> Decimals = new() { 1.0m, 2.0m, 3.0m };
        public List<DateTime> DateTimes = new() { DateTime.Now };
        // System.Runtime.Serialization.SerializationException : Invalid BinaryFormatter stream.
        // System.NotSupportedException : Can't resolve System.Collections.Generic.List`1[[System.TimeSpan, System.Private.CoreLib, Version=10.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e]]
        // Even though when serialized as a root record, TimeSpan is normalized to the framework assembly.
        // public List<TimeSpan> TimeSpans = new() { TimeSpan.FromHours(1) };
        public List<string> Strings = new() { "a", "b", "c" };

        public void Equals(TestData other, Size bitmapSize)
        {
            Bitmap.Size.Should().Be(bitmapSize);
            Inner.Should().BeEquivalentTo(other.Inner);
            Count.Should().Be(other.Count);
            Today.Should().Be(other.Today);
            FloatArray.Should().BeEquivalentTo(other.FloatArray);
            TimeSpanArray.Should().BeEquivalentTo(other.TimeSpanArray);
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
            DateTimes.Should().BeEquivalentTo(other.DateTimes);
            // TimeSpans.Should().BeEquivalentTo(other.TimeSpans);
            Strings.Should().BeEquivalentTo(other.Strings);
        }
    }

    private const float Delta = 0.0003f;

    private static Type TestDataResolver(TypeName typeName)
    {
        (string name, Type type)[] allowedTypes =
        [
            (typeof(TestData).FullName!, typeof(TestData)),
            (typeof(TestDataBase.InnerData).FullName!, typeof(TestDataBase.InnerData)),
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

        throw new NotSupportedException($"Can't resolve {fullName}");
    }

     [Fact]
    public void ReadFontSerializedOnNet481()
    {
        // This string was generated on net481.
        // Clipboard.SetData("TestData", new Font("Arial", 12));
        // And the resulting stream was saved as a string
        // string text = Convert.ToBase64String(stream.ToArray());
        string arielFont =
            "AAEAAAD/////AQAAAAAAAAAMAgAAAFFTeXN0ZW0uRHJhd2luZywgVmVyc2lvbj00LjAuMC4wLCBDdWx0dXJl"
            + "PW5ldXRyYWwsIFB1YmxpY0tleVRva2VuPWIwM2Y1ZjdmMTFkNTBhM2EFAQAAABNTeXN0ZW0uRHJhd2luZy5G"
            + "b250BAAAAAROYW1lBFNpemUFU3R5bGUEVW5pdAEABAQLGFN5c3RlbS5EcmF3aW5nLkZvbnRTdHlsZQIAAAAb"
            + "U3lzdGVtLkRyYXdpbmcuR3JhcGhpY3NVbml0AgAAAAIAAAAGAwAAABRNaWNyb3NvZnQgU2FucyBTZXJpZgAA"
            + "IEEF/P///xhTeXN0ZW0uRHJhd2luZy5Gb250U3R5bGUBAAAAB3ZhbHVlX18ACAIAAAAAAAAABfv///8bU3lz"
            + "dGVtLkRyYXdpbmcuR3JhcGhpY3NVbml0AQAAAAd2YWx1ZV9fAAgCAAAAAwAAAAs=";

        byte[] bytes = Convert.FromBase64String(arielFont);
        using MemoryStream stream = new MemoryStream(bytes);
        var result =  Utilities.ReadObjectFromStream<object>(
            stream,
            resolver: null,
            restrictDeserialization : false,
            legacyMode: true);
    }
}
