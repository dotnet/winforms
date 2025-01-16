// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.ComponentModel;
using System.Drawing;
using System.Formats.Nrbf;
using System.Reflection.Metadata;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json;
using System.Windows.Forms.BinaryFormat;
using System.Windows.Forms.Nrbf;
using static System.Windows.Forms.TestUtilities.DataObjectTestHelpers;

namespace System.Private.Windows.Core.BinaryFormat.Tests;

public class WinFormsBinaryFormattedObjectTests
{
    private static readonly Attribute[] s_visible = [DesignerSerializationVisibilityAttribute.Visible];

    [Fact]
    public void BinaryFormattedObject_NonJsonData_RemainsSerialized()
    {
        SimpleTestData testData = new() { X = 1, Y = 1 };
        SerializationRecord format = testData.SerializeAndDecode();
        ITypeResolver resolver = new DataObject.Composition.Binder(typeof(SimpleTestData), resolver: null, legacyMode: false);
        format.TryGetObjectFromJson<SimpleTestData>(resolver, out _).Should().BeFalse();
    }

    [Fact]
    public void BinaryFormattedObject_JsonData_RoundTrip()
    {
        SimpleTestData testData = new() { X = 1, Y = 1 };

        JsonData<SimpleTestData> json = new()
        {
            JsonBytes = JsonSerializer.SerializeToUtf8Bytes(testData)
        };

        using MemoryStream stream = new();
        WinFormsBinaryFormatWriter.WriteJsonData(stream, json);

        stream.Position = 0;
        SerializationRecord binary = NrbfDecoder.Decode(stream);
        binary.TypeName.AssemblyName!.FullName.Should().Be(IJsonData.CustomAssemblyName);
        ITypeResolver resolver = new DataObject.Composition.Binder(typeof(SimpleTestData), resolver: null, legacyMode: false);
        binary.TryGetObjectFromJson<int>(resolver, out _).Should().BeTrue();
        binary.TryGetObjectFromJson<SimpleTestData>(resolver, out object? result).Should().BeTrue();
        SimpleTestData deserialized = result.Should().BeOfType<SimpleTestData>().Which;
        deserialized.Should().BeEquivalentTo(testData);
    }

    [Fact]
    public void BinaryFormattedObject_Deserialize_FromStream_WithBinaryFormatter()
    {
        SimpleTestData testData = new() { X = 1, Y = 1 };
        JsonData<SimpleTestData> data = new()
        {
            JsonBytes = JsonSerializer.SerializeToUtf8Bytes(testData)
        };

        using MemoryStream stream = new();
        WinFormsBinaryFormatWriter.WriteJsonData(stream, data);
        stream.Position = 0;

        using BinaryFormatterScope scope = new(enable: true);
#pragma warning disable SYSLIB0011 // Type or member is obsolete
        BinaryFormatter binaryFormatter = new() { Binder = new JsonDataTestDataBinder() };
#pragma warning restore SYSLIB0011
        SimpleTestData deserialized = binaryFormatter.Deserialize(stream).Should().BeOfType<SimpleTestData>().Which;
        deserialized.Should().BeEquivalentTo(testData);
    }

    [Serializable]
    private struct ReplicatedJsonData : IObjectReference
    {
        public byte[] JsonBytes { get; set; }

        public string InnerTypeAssemblyQualifiedName { get; set; }

        public readonly object GetRealObject(StreamingContext context)
        {
            object? result = null;
            if (TypeName.TryParse(InnerTypeAssemblyQualifiedName, out TypeName? innerTypeName)
                && innerTypeName.Matches(typeof(SimpleTestData).ToTypeName()))
            {
                result = JsonSerializer.Deserialize<SimpleTestData>(JsonBytes);
            }

            return result ?? throw new InvalidOperationException();
        }
    }

    private class JsonDataTestDataBinder : SerializationBinder
    {
        public override Type? BindToType(string assemblyName, string typeName)
        {
            if (assemblyName == "System.Private.Windows.VirtualJson"
                && typeName == "System.Private.Windows.JsonData")
            {
                return typeof(ReplicatedJsonData);
            }

            throw new InvalidOperationException();
        }
    }

    [Fact]
    public void BinaryFormattedObject_Bitmap_FromBinaryFormatter()
    {
        using Bitmap bitmap = new(10, 10);
        SerializationRecord rootRecord = bitmap.SerializeAndDecode();
        ClassRecord root = rootRecord.Should().BeAssignableTo<ClassRecord>().Subject;
        root.TypeNameMatches(typeof(Bitmap)).Should().BeTrue();
        root.TypeName.FullName.Should().Be(typeof(Bitmap).FullName);
        root.TypeName.AssemblyName!.FullName.Should().Be(AssemblyRef.SystemDrawing);
        ArrayRecord arrayRecord = root.GetArrayRecord("Data")!;
        arrayRecord.Should().BeAssignableTo<SZArrayRecord<byte>>();
        rootRecord.TryGetBitmap(out object? result).Should().BeTrue();
        using Bitmap deserialized = result.Should().BeOfType<Bitmap>().Which;
        deserialized.Size.Should().Be(bitmap.Size);
    }

    [Fact]
    public void BinaryFormattedObject_Bitmap_RoundTrip()
    {
        using Bitmap bitmap = new(10, 10);
        using MemoryStream stream = new();
        WinFormsBinaryFormatWriter.WriteBitmap(stream, bitmap);

        stream.Position = 0;
        SerializationRecord rootRecord = NrbfDecoder.Decode(stream);

        rootRecord.TryGetBitmap(out object? result).Should().BeTrue();
        using Bitmap deserialized = result.Should().BeOfType<Bitmap>().Which;
        deserialized.Size.Should().Be(bitmap.Size);
    }

    [Fact]
    public void BinaryFormattedObject_Bitmap_FromWinFormsBinaryFormatWriter()
    {
        using Bitmap bitmap = new(10, 10);
        using MemoryStream stream = new();
        WinFormsBinaryFormatWriter.WriteBitmap(stream, bitmap);

        stream.Position = 0;

        using BinaryFormatterScope formatterScope = new(enable: true);
#pragma warning disable SYSLIB0011 // Type or member is obsolete
        // cs/binary-formatter-without-binder
        BinaryFormatter binaryFormat = new(); // CodeQL [SM04191] This is a test deserialization process is performed on trusted data and the types are controlled and validated.
#pragma warning restore SYSLIB0011

        // cs/dangerous-binary-deserialization
        using Bitmap deserialized = binaryFormat.Deserialize(stream).Should().BeOfType<Bitmap>().Which; // CodeQL [SM03722] : Testing legacy feature. This is a safe use of BinaryFormatter because the data is trusted and the types are controlled and validated.
        deserialized.Size.Should().Be(bitmap.Size);
    }

    [Fact]
    public void BinaryFormattedObject_ImageListStreamer_FromBinaryFormatter()
    {
        using ImageList sourceList = new();
        using Bitmap image = new(10, 10);
        sourceList.Images.Add(image);
        using ImageListStreamer stream = sourceList.ImageStream!;

        SerializationRecord rootRecord = stream.SerializeAndDecode();
        ClassRecord root = rootRecord.Should().BeAssignableTo<ClassRecord>().Subject;
        root.TypeName.FullName.Should().Be(typeof(ImageListStreamer).FullName);
        root.TypeName.AssemblyName!.FullName.Should().Be(typeof(WinFormsBinaryFormatWriter).Assembly.FullName);
        root.GetArrayRecord("Data")!.Should().BeAssignableTo<SZArrayRecord<byte>>();

        rootRecord.TryGetImageListStreamer(out object? result).Should().BeTrue();
        using ImageListStreamer deserialized = result.Should().BeOfType<ImageListStreamer>().Which;
        using ImageList newList = new();
        newList.ImageStream = deserialized;
        newList.Images.Count.Should().Be(1);
        Bitmap newImage = (Bitmap)newList.Images[0];
        newImage.Size.Should().Be(sourceList.Images[0].Size);
    }

    [Fact]
    public void BinaryFormattedObject_ImageListStreamer_RoundTrip()
    {
        using ImageList sourceList = new();
        using Bitmap image = new(10, 10);
        sourceList.Images.Add(image);
        using ImageListStreamer stream = sourceList.ImageStream!;

        using MemoryStream memoryStream = new();
        WinFormsBinaryFormatWriter.WriteImageListStreamer(memoryStream, stream);
        memoryStream.Position = 0;
        SerializationRecord rootRecord = NrbfDecoder.Decode(memoryStream);

        rootRecord.TryGetImageListStreamer(out object? result).Should().BeTrue();
        using ImageListStreamer deserialized = result.Should().BeOfType<ImageListStreamer>().Which;
        using ImageList newList = new();
        newList.ImageStream = deserialized;
        newList.Images.Count.Should().Be(1);
        Bitmap newImage = (Bitmap)newList.Images[0];
        newImage.Size.Should().Be(sourceList.Images[0].Size);
    }

    [Theory]
    [MemberData(nameof(BinaryFormattedObjects_TestData))]
    public void NrbfDecoder_SuccessfullyDecode(object value)
    {
        // Check that we can parse types that would hit the BinaryFormatter for property serialization.
        using (value as IDisposable)
        {
            var format = value.SerializeAndDecode();
        }
    }

    public static TheoryData<object> BinaryFormattedObjects_TestData =>
    [
        default(PointF),
        new PointF[] { default },
        default(RectangleF),
        new RectangleF[] { default },
        new DateTime[] { default },
        new ImageListStreamer(new ImageList()),
        new ListViewGroup(),
        new ListViewItem(),
        new OwnerDrawPropertyBag(),
        new TreeNode(),
        new ListViewItem.ListViewSubItem()
    ];

    [WinFormsTheory]
    [MemberData(nameof(Control_DesignerVisibleProperties_TestData))]
    public void Control_BinaryFormatted_DesignerVisibleProperties(object value, string[] properties)
    {
        // Check WinForms types for properties that can hit the BinaryFormatter

        using (value as IDisposable)
        {
            var propertyDescriptors = TypeDescriptor.GetProperties(value, s_visible);

            List<string> binaryFormattedProperties = [];
            foreach (PropertyDescriptor property in propertyDescriptors)
            {
                Type propertyType = property.PropertyType;
                if (propertyType.IsBinaryFormatted())
                {
                    binaryFormattedProperties.Add($"{property.Name}: {propertyType.Name}");
                }
            }

            Assert.Equal(properties, binaryFormattedProperties);
        }
    }

    public static TheoryData<object, string[]> Control_DesignerVisibleProperties_TestData => new()
    {
        { new Control(), new string[] { "DataContext: Object", "Tag: Object" } },
        { new Form(), new string[] { "DataContext: Object", "Tag: Object" } },
        { new Button(), new string[] { "DataContext: Object", "Tag: Object" } },
        { new CheckBox(), new string[] { "DataContext: Object", "Tag: Object" } },
        { new RadioButton(), new string[] { "DataContext: Object", "Tag: Object" } },
        { new DataGridView(), new string[] { "DataSource: Object", "DataContext: Object", "Tag: Object" } },
        { new DateTimePicker(), new string[] { "DataContext: Object", "Tag: Object" } },
        { new GroupBox(), new string[] { "DataContext: Object", "Tag: Object" } },
        { new Label(), new string[] { "DataContext: Object", "Tag: Object" } },
        { new ComboBox(), new string[] { "DataSource: Object", "DataContext: Object", "Tag: Object" } },
        { new ListBox(), new string[] { "DataSource: Object", "DataContext: Object", "Tag: Object" } },
        { new ListView(), new string[] { "DataContext: Object", "Tag: Object" } },
        {
            new MonthCalendar(), new string[]
            {
                "AnnuallyBoldedDates: DateTime[]",
                "BoldedDates: DateTime[]",
                "MonthlyBoldedDates: DateTime[]",
                "DataContext: Object",
                "Tag: Object"
            }
        },
        { new PictureBox(), new string[] { "DataContext: Object", "Tag: Object" } },
        { new PrintPreviewControl(), new string[] { "DataContext: Object", "Tag: Object" } },
        { new ProgressBar(), new string[] { "DataContext: Object", "Tag: Object" } },
        { new ScrollableControl(), new string[] { "DataContext: Object", "Tag: Object" } },
        { new HScrollBar(), new string[] { "DataContext: Object", "Tag: Object" } },
        { new VScrollBar(), new string[] { "DataContext: Object", "Tag: Object" } },
        { new Splitter(), new string[] { "DataContext: Object", "Tag: Object" } },
        { new TabControl(), new string[] { "DataContext: Object", "Tag: Object" } },
        { new TextBox(), new string[] { "DataContext: Object", "Tag: Object" } },
        { new RichTextBox(), new string[] { "DataContext: Object", "Tag: Object" } },
        { new MaskedTextBox(), new string[] { "DataContext: Object", "Tag: Object" } },
        { new ToolStrip(), new string[] { "DataContext: Object", "Tag: Object" } },
        { new TrackBar(), new string[] { "DataContext: Object", "Tag: Object" } },
        { new WebBrowser(), new string[] { "DataContext: Object", "Tag: Object" } },
    };
}
