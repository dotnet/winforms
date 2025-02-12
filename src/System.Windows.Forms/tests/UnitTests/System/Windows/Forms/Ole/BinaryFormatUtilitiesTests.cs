// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Private.Windows.Ole;
using System.Private.Windows.Ole.Tests;
using System.Reflection.Metadata;
using Utilities = System.Private.Windows.Ole.BinaryFormatUtilities<System.Windows.Forms.Nrbf.WinFormsNrbfSerializer>;

namespace System.Windows.Forms.Ole;

public sealed class BinaryFormatUtilitiesTests : BinaryFormatUtilitesTestsBase
{
    protected override bool TryReadObjectFromStream<T>(
        MemoryStream stream,
        bool untypedRequest,
        string format,
        Func<TypeName, Type>? resolver,
        [NotNullWhen(true)] out T? @object) where T : default
    {
        DataRequest request = new(format) { Resolver = resolver, TypedRequest = untypedRequest };
        return Utilities.TryReadObjectFromStream(stream, in request, out @object);
    }

    protected override void WriteObjectToStream(MemoryStream stream, object data, string format)
    {
        Utilities.WriteObjectToStream(stream, data, format);
    }

    [Fact]
    public void RoundTrip_ImageList()
    {
        using ImageList sourceList = new();
        using Bitmap image = new(10, 10);
        sourceList.Images.Add(image);
        using ImageListStreamer value = sourceList.ImageStream!;

        RoundTripObject(value, out ImageListStreamer? result).Should().BeTrue();
        result.Should().BeOfType<ImageListStreamer>();
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

        RoundTripObject_RestrictedFormat(value, out ImageListStreamer? result).Should().BeTrue();
        using ImageList newList = new();
        newList.ImageStream = result;
        newList.Images.Count.Should().Be(1);
    }

    [Fact]
    public void RoundTrip_Bitmap()
    {
        using Bitmap value = new(10, 10);
        RoundTripObject(value, out Bitmap? result).Should().BeTrue();
        result.Should().BeOfType<Bitmap>().Which.Size.Should().Be(value.Size);
    }

    [Fact]
    public void RoundTrip_RestrictedFormat_Bitmap()
    {
        using Bitmap value = new(10, 10);
        RoundTripObject_RestrictedFormat(value, out Bitmap? result).Should().BeTrue();
        result.Should().BeOfType<Bitmap>().Which.Size.Should().Be(value.Size);
    }

    [Fact]
    public void RoundTripOfType_AsUnmatchingType_Simple()
    {
        List<int> value = [1, 2, 3];
        RoundTripOfType(value, out Control? result).Should().BeFalse();
        result.Should().BeNull();
    }

    [Fact]
    public void RoundTripOfType_RestrictedFormat_AsUnmatchingType_Simple()
    {
        Rectangle value = new(1, 1, 2, 2);

        // We are setting up an invalid content scenario, Rectangle type can't be read as a restricted format,
        // but in this case requested type will not match the payload type.
        WriteObjectToStream(value);
        Stream.Position = 0;
        ReadRestrictedObjectFromStream(NotSupportedResolver, out Control? result).Should().BeFalse();
        result.Should().BeNull();

        using ClipboardBinaryFormatterFullCompatScope scope = new();
        Stream.Position = 0;
        ReadRestrictedObjectFromStream(NotSupportedResolver, out Control? result2).Should().BeFalse();
        result2.Should().BeNull();
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

        using ClipboardBinaryFormatterFullCompatScope scope = new();
        RoundTripOfType(value, FontResolver, out Font? result).Should().BeTrue();
        result.Should().BeOfType<Font>().Which.Should().Be(value);
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

        DataRequest request = new("test")
        {
            TypedRequest = true
        };

        // Default deserialization with the NRBF deserializer.
        using (BinaryFormatterInClipboardDragDropScope binaryFormatScope = new(enable: true))
        {
            // GetData case.
            stream.Position = 0;

            Action getData = () => Utilities.TryReadObjectFromStream<object>(
               stream,
               in request,
               out _);

            getData.Should().Throw<NotSupportedException>();

            Action tryGetData = () => TryGetData(stream);
            tryGetData.Should().Throw<NotSupportedException>();
        }

        // Deserialize using the binary formatter.
        using ClipboardBinaryFormatterFullCompatScope scope = new();

        // GetData case.
        stream.Position = 0;
        Utilities.TryReadObjectFromStream(
           stream,
           in request,
           out Font? result).Should().BeTrue();

        result.Should().NotBeNull();
        result!.Name.Should().Be("Microsoft Sans Serif");
        result.Size.Should().Be(10);

        TryGetData(stream);

        static void TryGetData(MemoryStream stream)
        {
            // TryGetData<Font> case.
            stream.Position = 0;

            DataRequest request = new("test")
            {
                Resolver = FontResolver,
            };

            Utilities.TryReadObjectFromStream(
                stream,
                in request,
                out Font? result).Should().BeTrue();

            result.Should().NotBeNull();
            result!.Name.Should().Be("Microsoft Sans Serif");
            result.Size.Should().Be(10);
        }
    }
}
