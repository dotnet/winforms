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
        Bitmap result = Assert.IsType<Bitmap>(converter.ConvertFrom(resourceBytes));
        Assert.Equal(new(800, 600), result.Size);
    }

    [Fact]
    public static void StronglyTypedResourceBuilder_Create_BitmapResource_FromMemory()
    {
        using Bitmap bitmap = new(10, 10);
        var converter = TypeDescriptor.GetConverter(bitmap);

        ResXDataNode node = new("Image1", converter.ConvertTo(bitmap, typeof(byte[])));
        using var temp = TempFile.Create();
        using (ResXResourceWriter resxWriter = new(temp.Path))
        {
            resxWriter.AddResource(node);
            resxWriter.Generate();
        }

        var compileUnit = StronglyTypedResourceBuilder.Create(
            resxFile: temp.Path,
            baseName: "Resources",
            generatedCodeNamespace: "Namespace",
            s_cSharpProvider,
            internalClass: false,
            out _);

        MemoryStream resourceStream = new();
        using ResXResourceReader resXReader = new(temp.Path);
        using ResourceWriter resourceWriter = new(resourceStream);
        var enumerator = resXReader.GetEnumerator();
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
        Bitmap result = Assert.IsType<Bitmap>(converter.ConvertFrom(resourceBytes));
        Assert.Equal(bitmap.Size, result.Size);
    }

    [Fact]
    public static void StronglyTypedResourceBuilder_Create_IconResource_FromFile()
    {
        const string data = """
            <data name="Icon1" type="System.Resources.ResXFileRef, System.Windows.Forms">
              <value>Resources\Icon1.ico;System.Byte[], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</value>
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
        var iconProperty = type.GetProperty("Icon1");
        Assert.NotNull(iconProperty);
        byte[] resourceByte = Assert.IsType<byte[]>(iconProperty.GetValue(obj: null));

        var converter = TypeDescriptor.GetConverter(typeof(Icon));
        Icon result = Assert.IsType<Icon>(converter.ConvertFrom(resourceByte));
        Assert.Equal(new(32, 32), result.Size);
    }

    [Fact]
    public static void StronglyTypedResourceBuilder_Create_IconResource_FromMemory()
    {
        using Icon icon = new(SystemIcons.Exclamation, 16, 16);
        var converter = TypeDescriptor.GetConverter(icon);

        ResXDataNode node = new("Icon1", converter.ConvertTo(icon, typeof(byte[])));
        using var temp = TempFile.Create();
        using (ResXResourceWriter resxWriter = new(temp.Path))
        {
            resxWriter.AddResource(node);
            resxWriter.Generate();
        }

        var compileUnit = StronglyTypedResourceBuilder.Create(
            resxFile: temp.Path,
            baseName: "Resources",
            generatedCodeNamespace: "Namespace",
            s_cSharpProvider,
            internalClass: false,
            out _);

        MemoryStream resourceStream = new();
        using ResXResourceReader resXReader = new(temp.Path);
        using ResourceWriter resourceWriter = new(resourceStream);
        var enumerator = resXReader.GetEnumerator();
        while (enumerator.MoveNext())
        {
            resourceWriter.AddResource((string)enumerator.Key, enumerator.Value);
        }

        resourceWriter.Generate();

        resourceStream.Position = 0;

        Type type = CodeDomCompileHelper.CompileClass(compileUnit, "Resources", "Namespace", resourceStream);
        Assert.NotNull(type);
        var imageProperty = type.GetProperty("Icon1");
        Assert.NotNull(imageProperty);
        byte[] resourceBytes = Assert.IsType<byte[]>(imageProperty.GetValue(obj: null));
        Icon result = Assert.IsType<Icon>(converter.ConvertFrom(resourceBytes));
        Assert.Equal(icon.Size, result.Size);
    }
}
