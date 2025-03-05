// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Private.Windows.Ole;
using System.Private.Windows.Ole.Tests;
using System.Reflection.Metadata;
using BinaryFormatUtilities = System.Private.Windows.Ole.BinaryFormatUtilities<System.Windows.Forms.Nrbf.WinFormsNrbfSerializer>;

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
        DataRequest request = new(format) { Resolver = resolver, TypedRequest = !untypedRequest };
        return BinaryFormatUtilities.TryReadObjectFromStream(stream, in request, out @object);
    }

    protected override void WriteObjectToStream(MemoryStream stream, object data, string format)
    {
        BinaryFormatUtilities.WriteObjectToStream(stream, data, format);
    }

    [Fact]
    public void RoundTrip_ImageList()
    {
        using ImageList sourceList = new();
        using Bitmap image = new(10, 10);
        sourceList.Images.Add(image);
        using ImageListStreamer value = sourceList.ImageStream!;

        using MemoryStream stream = CreateStream(DataType.BinaryFormat, value);

        DataRequest request = new()
        {
            Format = "test",
            TypedRequest = true
        };

        BinaryFormatUtilities.TryReadObjectFromStream(stream, in request, out ImageListStreamer? result).Should().BeTrue();
        using ImageList newList = new();
        newList.ImageStream = result;
        newList.Images.Count.Should().Be(1);
    }

    [Fact]
    public void RoundTrip_Bitmap()
    {
        using Bitmap value = new(10, 10);
        using MemoryStream stream = CreateStream(DataType.BinaryFormat, value);

        DataRequest request = new()
        {
            Format = "test",
            TypedRequest = true
        };

        BinaryFormatUtilities.TryReadObjectFromStream(stream, in request, out Bitmap? result).Should().BeTrue();
        result.Should().BeOfType<Bitmap>().Which.Size.Should().Be(value.Size);
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
        string fontData =
            "AAEAAAD/////AQAAAAAAAAAMAgAAAFFTeXN0ZW0uRHJhd2luZywgVmVyc2lvbj00LjAuMC4wLCBDdWx0dXJl"
            + "PW5ldXRyYWwsIFB1YmxpY0tleVRva2VuPWIwM2Y1ZjdmMTFkNTBhM2EFAQAAABNTeXN0ZW0uRHJhd2luZy5G"
            + "b250BAAAAAROYW1lBFNpemUFU3R5bGUEVW5pdAEABAQLGFN5c3RlbS5EcmF3aW5nLkZvbnRTdHlsZQIAAAAb"
            + "U3lzdGVtLkRyYXdpbmcuR3JhcGhpY3NVbml0AgAAAAIAAAAGAwAAABRNaWNyb3NvZnQgU2FucyBTZXJpZgAA"
            + "IEEF/P///xhTeXN0ZW0uRHJhd2luZy5Gb250U3R5bGUBAAAAB3ZhbHVlX18ACAIAAAAAAAAABfv///8bU3lz"
            + "dGVtLkRyYXdpbmcuR3JhcGhpY3NVbml0AQAAAAd2YWx1ZV9fAAgCAAAAAwAAAAs=";

        byte[] bytes = Convert.FromBase64String(fontData);
        using MemoryStream stream = new(bytes);
        stream.Position = 0;

        DataRequest request = new()
        {
            Format = "test",
            TypedRequest = false
        };

        // Untyped BinaryFormatter disabled
        Action action = () => BinaryFormatUtilities.TryReadObjectFromStream(
            stream,
            in request,
            out object? _);

        action.Should().Throw<NotSupportedException>();

        // Untyped BinaryFormatter enabled
        using ClipboardBinaryFormatterFullCompatScope scope = new();

        BinaryFormatUtilities.TryReadObjectFromStream(
           stream,
           in request,
           out object? result).Should().BeTrue();

        result.Should().NotBeNull();
        Font? font = result.Should().BeOfType<Font>().Subject;
        font.Name.Should().Be("Microsoft Sans Serif");
        font.Size.Should().Be(10);

        // Typed with resolver
        request = new()
        {
            Format = "test",
            Resolver = FontResolver,
            TypedRequest = true
        };

        BinaryFormatUtilities.TryReadObjectFromStream(
            stream,
            in request,
            out font).Should().BeTrue();

        font.Should().NotBeNull();
        font!.Name.Should().Be("Microsoft Sans Serif");
        font.Size.Should().Be(10);
    }
}
