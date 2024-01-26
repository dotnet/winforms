// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms.BinaryFormat.Tests;

public class WinFormsBinaryFormattedObjectTests
{
    private static readonly Attribute[] s_visible = [DesignerSerializationVisibilityAttribute.Visible];

    [Fact]
    public void BinaryFormattedObject_ImageListStreamer_FromBinaryFormatter()
    {
        using ImageList sourceList = new();
        using Bitmap image = new(10, 10);
        sourceList.Images.Add(image);
        using ImageListStreamer stream = sourceList.ImageStream!;

        BinaryFormattedObject binary = stream.SerializeAndParse();
        binary.RecordCount.Should().Be(5);
        binary[1].Should().BeOfType<BinaryLibrary>().Which
            .LibraryName.Should().Be(typeof(WinFormsBinaryFormatWriter).Assembly.FullName);
        binary[2].Should().BeOfType<ClassWithMembersAndTypes>().Which.Name.Should().Be(typeof(ImageListStreamer).FullName);
        binary[3].Should().BeOfType<ArraySinglePrimitive<byte>>();

        binary.TryGetImageListStreamer(out object? result).Should().BeTrue();
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
        BinaryFormattedObject binary = new(memoryStream, leaveOpen: true);

        binary.TryGetImageListStreamer(out object? result).Should().BeTrue();
        using ImageListStreamer deserialized = result.Should().BeOfType<ImageListStreamer>().Which;
        using ImageList newList = new();
        newList.ImageStream = deserialized;
        newList.Images.Count.Should().Be(1);
        Bitmap newImage = (Bitmap)newList.Images[0];
        newImage.Size.Should().Be(sourceList.Images[0].Size);
    }

    [Theory]
    [MemberData(nameof(BinaryFormattedObjects_TestData))]
    public void BinaryFormattedObjects_SuccessfullyParse(object value)
    {
        // Check that we can parse types that would hit the BinaryFormatter for property serialization.
        using (value as IDisposable)
        {
            var format = value.SerializeAndParse();
        }
    }

    public static TheoryData<object> BinaryFormattedObjects_TestData => new()
    {
        new PointF(),
        new PointF[] { default },
        new RectangleF(),
        new RectangleF[] { default },
        new DateTime[] { default },
        new ImageListStreamer(new ImageList()),
        new ListViewGroup(),
        new ListViewItem(),
        new OwnerDrawPropertyBag(),
        new TreeNode(),
        new ListViewItem.ListViewSubItem()
    };

    [WinFormsTheory]
    [MemberData(nameof(Control_DesignerVisibleProperties_TestData))]
    public void Control_BinaryFormatted_DesignerVisibleProperties(object value, string[] properties)
    {
        // Check WinForms types for properties that can hit the BinaryFormatter

        using (value as IDisposable)
        {
            var propertyDescriptors = TypeDescriptor.GetProperties(value, s_visible);

            List<string> binaryFormattedProperties = new();
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
