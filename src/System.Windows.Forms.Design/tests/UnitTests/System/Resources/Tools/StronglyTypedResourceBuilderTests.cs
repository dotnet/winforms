// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.CodeDom.Compiler;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using Microsoft.CSharp;
using Xunit;

namespace System.Resources.Tools.Tests;

public partial class StronglyTypedResourceBuilderTests
{
    // https://docs.microsoft.com/dotnet/core/extensions/work-with-resx-files-programmatically

    private static readonly CodeDomProvider s_cSharpProvider = new CSharpCodeProvider();

    private static string CreateResx(string data = null) =>
        $"""
        <root>
            <resheader name="resmimetype">
                <value>text/microsoft-resx</value>
            </resheader>
            <resheader name="version">
                <value>2.0</value>
            </resheader>
            <resheader name="reader">
                <value>System.Resources.ResXResourceReader</value>
            </resheader>
            <resheader name="writer">
                <value>System.Resources.ResXResourceWriter</value>
            </resheader>
            {data ?? string.Empty}
        </root>
        """;

    [Fact]
    public void StronglyTypedResourceBuilder_Create_NullBaseName_ThrowsArgumentNull()
    {
        Hashtable resources = new();
        Assert.Throws<ArgumentNullException>(
            "baseName",
            () => StronglyTypedResourceBuilder.Create(
                resources,
                baseName: null,
                string.Empty,
                s_cSharpProvider,
                internalClass: false,
                out _));

        Assert.Throws<ArgumentNullException>(
            "baseName",
            () => StronglyTypedResourceBuilder.Create(
                resources,
                baseName: null,
                string.Empty,
                string.Empty,
                s_cSharpProvider,
                internalClass: false,
                out _));

        using var temp = TempFile.Create(CreateResx());
        Assert.Throws<ArgumentNullException>(
            "baseName",
            () => StronglyTypedResourceBuilder.Create(
                temp.Path,
                baseName: null,
                string.Empty,
                s_cSharpProvider,
                internalClass: false,
                out _));

        Assert.Throws<ArgumentNullException>(
            "baseName",
            () => StronglyTypedResourceBuilder.Create(
                temp.Path,
                baseName: null,
                string.Empty,
                string.Empty,
                s_cSharpProvider,
                internalClass: false,
                out _));
    }

    [Fact]
    public void StronglyTypedResourceBuilder_Create_NullCodeProvider_ThrowsArgumentNull()
    {
        Hashtable resources = new();
        Assert.Throws<ArgumentNullException>(
            "codeProvider",
            () => StronglyTypedResourceBuilder.Create(
                resources,
                string.Empty,
                string.Empty,
                codeProvider: null,
                internalClass: false,
                out _));

        Assert.Throws<ArgumentNullException>(
            "codeProvider",
            () => StronglyTypedResourceBuilder.Create(
                resources,
                string.Empty,
                string.Empty,
                string.Empty,
                codeProvider: null,
                internalClass: false,
                out _));

        using var temp = TempFile.Create(CreateResx());
        Assert.Throws<ArgumentNullException>(
            "codeProvider",
            () => StronglyTypedResourceBuilder.Create(
                temp.Path,
                string.Empty,
                string.Empty,
                codeProvider: null,
                internalClass: false,
                out _));

        Assert.Throws<ArgumentNullException>(
            "codeProvider",
            () => StronglyTypedResourceBuilder.Create(
                temp.Path,
                string.Empty,
                string.Empty,
                string.Empty,
                codeProvider: null,
                internalClass: false,
                out _));
    }

    [Fact]
    public void StronglyTypedResourceBuilder_Create_NullResourceList_ThrowsArgumentNull()
    {
        Hashtable resources = new();
        Assert.Throws<ArgumentNullException>(
            "resourceList",
            () => StronglyTypedResourceBuilder.Create(
                resourceList: null,
                string.Empty,
                string.Empty,
                s_cSharpProvider,
                internalClass: false,
                out _));

        Assert.Throws<ArgumentNullException>(
            "resourceList",
            () => StronglyTypedResourceBuilder.Create(
                resourceList: null,
                string.Empty,
                string.Empty,
                string.Empty,
                s_cSharpProvider,
                internalClass: false,
                out _));
    }

    [Fact]
    public void StronglyTypedResourceBuilder_Create_NullResxFile_ThrowsArgumentNull()
    {
        Assert.Throws<ArgumentNullException>(
            "resxFile",
            () => StronglyTypedResourceBuilder.Create(
                resxFile: null,
                string.Empty,
                string.Empty,
                s_cSharpProvider,
                internalClass: false,
                out _));

        Assert.Throws<ArgumentNullException>(
            "resxFile",
            () => StronglyTypedResourceBuilder.Create(
                resxFile: null,
                string.Empty,
                string.Empty,
                string.Empty,
                s_cSharpProvider,
                internalClass: false,
                out _));
    }

    [Fact]
    public static void StronglyTypedResourceBuilder_Create_MismatchedResxDataNode_Throws()
    {
        Hashtable values = new()
        {
            { "TestName", new ResXDataNode("WrongName", "TestValue") }
        };

        Assert.Throws<ArgumentException>(() => StronglyTypedResourceBuilder.Create(
            resourceList: values,
            baseName: "Resources",
            generatedCodeNamespace: "Namespace",
            s_cSharpProvider,
            internalClass: false,
            out _));
    }

    [Fact]
    public static void StronglyTypedResourceBuilder_Create_EmptyResx()
    {
        using var temp = TempFile.Create(CreateResx());
        var compileUnit = StronglyTypedResourceBuilder.Create(
            resxFile: temp.Path,
            baseName: "Resources",
            generatedCodeNamespace: null,
            s_cSharpProvider,
            internalClass: false,
            out _);

        Type type = CodeDomCompileHelper.CompileClass(compileUnit, "Resources");
        Assert.NotNull(type);
    }

    [Fact]
    public static void StronglyTypedResourceBuilder_Create_StringResource_FromFile()
    {
        const string data = """
            <data name="TestName" xml:space="preserve">
              <value>TestValue</value>
            </data>
            """;

        using var temp = TempFile.Create(CreateResx(data));
        var compileUnit = StronglyTypedResourceBuilder.Create(
            resxFile: temp.Path,
            baseName: "Resources",
            generatedCodeNamespace: "Namespace",
            s_cSharpProvider,
            internalClass: false,
            out _);

        MemoryStream resourceStream = new();
        using ResourceWriter resourceWriter = new(resourceStream);
        resourceWriter.AddResource("TestName", "TestValue");
        resourceWriter.Generate();
        resourceStream.Position = 0;

        Type type = CodeDomCompileHelper.CompileClass(compileUnit, "Resources", "Namespace", resourceStream);
        Assert.NotNull(type);
        var nameProperty = type.GetProperty("TestName");
        Assert.NotNull(nameProperty);
        Assert.Equal("TestValue", (string)nameProperty.GetValue(obj: null));
    }

    [Fact]
    public static void StronglyTypedResourceBuilder_Create_StringResource_FromResxWriterFile()
    {
        using var temp = TempFile.Create();
        using (ResXResourceWriter writer = new(temp.Path))
        {
            writer.AddResource("TestName", "TestValue");
            writer.Generate();
        }

        var compileUnit = StronglyTypedResourceBuilder.Create(
            resxFile: temp.Path,
            baseName: "Resources",
            generatedCodeNamespace: "Namespace",
            s_cSharpProvider,
            internalClass: false,
            out _);

        MemoryStream resourceStream = new();
        using ResourceWriter resourceWriter = new(resourceStream);
        resourceWriter.AddResource("TestName", "TestValue");
        resourceWriter.Generate();
        resourceStream.Position = 0;

        Type type = CodeDomCompileHelper.CompileClass(compileUnit, "Resources", "Namespace", resourceStream);
        Assert.NotNull(type);
        var nameProperty = type.GetProperty("TestName");
        Assert.NotNull(nameProperty);
        Assert.Equal("TestValue", (string)nameProperty.GetValue(obj: null));
    }

    [Fact]
    public static void StronglyTypedResourceBuilder_Create_StringResource_FromResxDataNode()
    {
        Hashtable values = new()
        {
            { "TestName", new ResXDataNode("TestName", "TestValue") }
        };

        var compileUnit = StronglyTypedResourceBuilder.Create(
            resourceList: values,
            baseName: "Resources",
            generatedCodeNamespace: "Namespace",
            s_cSharpProvider,
            internalClass: false,
            out _);

        MemoryStream resourceStream = new();
        using ResourceWriter resourceWriter = new(resourceStream);
        resourceWriter.AddResource("TestName", "TestValue");
        resourceWriter.Generate();
        resourceStream.Position = 0;

        Type type = CodeDomCompileHelper.CompileClass(compileUnit, "Resources", "Namespace", resourceStream);
        Assert.NotNull(type);
        var nameProperty = type.GetProperty("TestName");
        Assert.NotNull(nameProperty);
        Assert.Equal("TestValue", (string)nameProperty.GetValue(obj: null));
    }

    [Fact]
    public static void StronglyTypedResourceBuilder_Create_BitmapResource_FromFile()
    {
        const string data = """
            <data name="Image1" type="System.Resources.ResXFileRef, System.Windows.Forms">
                <value>Resources\Image1.png;System.Byte[], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</value>
            </data>
            """;

        using var temp = TempFile.Create(CreateResx(data));

        var compileUnit = StronglyTypedResourceBuilder.Create(
            resxFile: temp.Path,
            baseName: "Resources",
            generatedCodeNamespace: "Namespace",
            s_cSharpProvider,
            internalClass: false,
            out _);

        MemoryStream resourceStream = new();
        using ResXResourceReader reader = new(temp.Path);
        var enumerator = reader.GetEnumerator();
        using ResourceWriter resourceWriter = new(resourceStream);

        while (enumerator.MoveNext())
        {
            resourceWriter.AddResource((string)enumerator.Key, enumerator.Value);
        }

        resourceWriter.Generate();
        resourceStream.Position = 0;

        Type type = CodeDomCompileHelper.CompileClass(compileUnit, "Resources", "Namespace", resourceStream);
        Assert.NotNull(type);
        var imageProperty = type.GetProperty("Image1");
        Assert.NotNull(imageProperty);
        byte[] resourceBytes = Assert.IsType<byte[]>(imageProperty.GetValue(obj: null));

        var converter = TypeDescriptor.GetConverter(typeof(Bitmap));
        var result = converter.ConvertFrom(resourceBytes);
    }

    [Fact]
    public static void StronglyTypedResourceBuilder_Create_BitmapResource_FromMemory()
    {
        using Bitmap bitmap = new(10, 10);
        var converter = TypeDescriptor.GetConverter(bitmap);

        // StronglyTypedResourceBuilder can't handle embedded byte data, need to investigate further.
        // (Doing it this way uses the Image TypeConverter to serialize the bitmap as a UUEncoded byte array.)
        //
        // using var temp = TempFile.Create();
        // using (ResXResourceWriter writer = new(temp.Path))
        // {
        //     writer.AddResource("Image1", converter.ConvertTo(bitmap, typeof(byte[])));
        //     writer.Generate();
        // }

        const string data = """
            <data name="Image1" type="System.Resources.ResXFileRef, System.Windows.Forms">
                <value>Resources\Image1.png;System.Byte[], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</value>
            </data>
            """;

        using var temp = TempFile.Create(CreateResx(data));

        var compileUnit = StronglyTypedResourceBuilder.Create(
            resxFile: temp.Path,
            baseName: "Resources",
            generatedCodeNamespace: "Namespace",
            s_cSharpProvider,
            internalClass: false,
            out _);

        MemoryStream resourceStream = new();
        using ResourceWriter resourceWriter = new(resourceStream);
        resourceWriter.AddResource("Image1", converter.ConvertTo(bitmap, typeof(byte[])));

        resourceWriter.Generate();

        resourceStream.Position = 0;

        Type type = CodeDomCompileHelper.CompileClass(compileUnit, "Resources", "Namespace", resourceStream);
        Assert.NotNull(type);
        var imageProperty = type.GetProperty("Image1");
        Assert.NotNull(imageProperty);
        byte[] resourceBytes = Assert.IsType<byte[]>(imageProperty.GetValue(obj: null));
        Bitmap result = Assert.IsType<Bitmap>(converter.ConvertFrom(resourceBytes));
        Assert.Equal(result.Size, bitmap.Size);
    }
}
