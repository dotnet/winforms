// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.CodeDom.Compiler;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using Microsoft.CSharp;
using Xunit;
using System.CodeDom;
using System.Reflection;

namespace System.Resources.Tools.Tests;

public partial class StronglyTypedResourceBuilderTests
{
    // https://docs.microsoft.com/dotnet/core/extensions/work-with-resx-files-programmatically

    private static readonly CodeDomProvider s_cSharpProvider = new CSharpCodeProvider();
    private const string TypeAssembly = "mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
    private const string TxtFileEncoding = "Windows-1252";

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

    private static PropertyInfo getPropertyInfo(IDictionaryEnumerator enumerator,
        CodeCompileUnit compileUnit, string propertyName)
    {
        MemoryStream resourceStream = new();
        using ResourceWriter resourceWriter = new(resourceStream);
        while (enumerator.MoveNext())
        {
            resourceWriter.AddResource((string)enumerator.Key, enumerator.Value);
        }

        resourceWriter.Generate();
        resourceStream.Position = 0;

        Type type = CodeDomCompileHelper.CompileClass(compileUnit, "Resources", "Namespace", resourceStream);
        Assert.NotNull(type);
        var propertyInfo = type.GetProperty(propertyName);
        Assert.NotNull(propertyInfo);
        return propertyInfo;
    }

    private static void validateResultBitmap(PropertyInfo imagePropertyInfo, Bitmap expected, TypeConverter converter)
    {
        byte[] resourceBytes = Assert.IsType<byte[]>(imagePropertyInfo.GetValue(obj: null));
        using Bitmap resourceBitmap = Assert.IsType<Bitmap>(converter.ConvertFrom(resourceBytes));
        Assert.Equal(expected.Size, resourceBitmap.Size);
        Assert.Equal(expected.GetPixel(0, 0), resourceBitmap.GetPixel(0, 0));
    }

    [Fact]
    public static void StronglyTypedResourceBuilder_Create_BitmapResource_FromFile()
    {
        const string data = $"""
            <data name="Image1" type="System.Resources.ResXFileRef, System.Windows.Forms">
                <value>Resources\Image1.png;System.Byte[], {TypeAssembly}</value>
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

        using ResXResourceReader reader = new(temp.Path);
        var imagePropertyInfo = getPropertyInfo(reader.GetEnumerator(), compileUnit, "Image1");
        using Bitmap expected = (Bitmap)Image.FromFile(@"Resources\Image1.png");
        validateResultBitmap(imagePropertyInfo, expected, TypeDescriptor.GetConverter(typeof(Bitmap)));
    }

    [Fact]
    public static void StronglyTypedResourceBuilder_Create_BitmapResource_FromMemory()
    {
        using Bitmap bitmap = new(10, 10);
        bitmap.SetPixel(0, 0, Color.Red);
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

        using ResXResourceReader reader = new(temp.Path);
        var imagePropertyInfo = getPropertyInfo(reader.GetEnumerator(), compileUnit, "Image1");
        validateResultBitmap(imagePropertyInfo, bitmap, converter);
    }

    [Fact]
    public static void StronglyTypedResourceBuilder_Create_BitmapResource_FromFileRef()
    {
        ResXFileRef fileRef = new(@"Resources\Image1.png",
            $"System.Byte[], {TypeAssembly}");
        Hashtable values = new()
        {
            { "Image1", new ResXDataNode("Image1", fileRef) }
        };

        var compileUnit = StronglyTypedResourceBuilder.Create(
            resourceList: values,
            baseName: "Resources",
            generatedCodeNamespace: "Namespace",
            s_cSharpProvider,
            internalClass: false,
            out _);

        MemoryStream resxStream = new();
        using ResXResourceWriter resxWriter = new(resxStream);
        resxWriter.AddResource(new ResXDataNode("Image1", fileRef));
        resxWriter.Generate();
        resxStream.Position = 0;
        using ResXResourceReader reader = new(resxStream);
        var imagePropertyInfo = getPropertyInfo(reader.GetEnumerator(), compileUnit, "Image1");
        using Bitmap expected = (Bitmap)Image.FromFile(@"Resources\Image1.png");
        validateResultBitmap(imagePropertyInfo, expected, TypeDescriptor.GetConverter(typeof(Bitmap)));
    }

    private static void validateResultIcon(PropertyInfo iconPropertyInfo, Icon expected, TypeConverter converter)
    {
        byte[] resourceByte = Assert.IsType<byte[]>(iconPropertyInfo.GetValue(obj: null));
        using Icon resourceIcon = Assert.IsType<Icon>(converter.ConvertFrom(resourceByte));
        Assert.Equal(expected.Size, resourceIcon.Size);
    }

[Fact]
    public static void StronglyTypedResourceBuilder_Create_IconResource_FromFile()
    {
        const string data = $"""
            <data name="Icon1" type="System.Resources.ResXFileRef, System.Windows.Forms">
              <value>Resources\Icon1.ico;System.Byte[], {TypeAssembly}</value>
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

        using ResXResourceReader reader = new(temp.Path);
        var iconPropertyInfo = getPropertyInfo(reader.GetEnumerator(), compileUnit, "Icon1");
        using Icon expected = new(@"Resources\Icon1.ico");
        validateResultIcon(iconPropertyInfo, expected, TypeDescriptor.GetConverter(typeof(Icon)));
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

        using ResXResourceReader reader = new(temp.Path);
        var iconPropertyInfo = getPropertyInfo(reader.GetEnumerator(), compileUnit, "Icon1");
        validateResultIcon(iconPropertyInfo, icon, converter);
    }

    [Fact]
    public static void StronglyTypedResourceBuilder_Create_IconResource_FromFileRef()
    {
        ResXFileRef fileRef = new(@"Resources\Icon1.ico",
            $"System.Byte[], {TypeAssembly}");
        Hashtable values = new()
        {
            { "Icon1", new ResXDataNode("Icon1", fileRef) }
        };

        var compileUnit = StronglyTypedResourceBuilder.Create(
            resourceList: values,
            baseName: "Resources",
            generatedCodeNamespace: "Namespace",
            s_cSharpProvider,
            internalClass: false,
            out _);

        MemoryStream resxStream = new();
        using ResXResourceWriter resxWriter = new(resxStream);
        resxWriter.AddResource(new ResXDataNode("Icon1", fileRef));
        resxWriter.Generate();
        resxStream.Position = 0;
        using ResXResourceReader reader = new(resxStream);
        var iconPropertyInfo = getPropertyInfo(reader.GetEnumerator(), compileUnit, "Icon1");
        using Icon expected = new(@"Resources\Icon1.ico");
        validateResultIcon(iconPropertyInfo, expected, TypeDescriptor.GetConverter(typeof(Icon)));
    }

    private static void validateResultTxtFileContent(PropertyInfo txtFilePropertyInfo)
    {
        string resourceTxtFileContents = Assert.IsType<string>(txtFilePropertyInfo.GetValue(obj: null));
        Assert.Equal("hello test\r\n!", resourceTxtFileContents);
    }

    [Fact]
    public static void StronglyTypedResourceBuilder_Create_TxtFileResource_FromFile()
    {
        const string data = $"""
            <data name="TextFile1" type="System.Resources.ResXFileRef, System.Windows.Forms">
              <value>Resources\TextFile1.txt;System.String, {TypeAssembly};{TxtFileEncoding}</value>
            </data>
            """;

        using var temp = TempFile.Create(CreateResx(data));
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        var compileUnit = StronglyTypedResourceBuilder.Create(
            resxFile: temp.Path,
            baseName: "Resources",
            generatedCodeNamespace: "Namespace",
            s_cSharpProvider,
            internalClass: false,
            out _);

        using ResXResourceReader reader = new(temp.Path);
        validateResultTxtFileContent(getPropertyInfo(reader.GetEnumerator(), compileUnit, "TextFile1"));
    }

    [Fact]
    public static void StronglyTypedResourceBuilder_Create_TxtFileResource_FromFileRef()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        ResXFileRef fileRef = new(@"Resources\TextFile1.txt",
            $"System.String, {TypeAssembly}",
            Encoding.GetEncoding(TxtFileEncoding));
        Hashtable values = new()
        {
            { "TextFile1", new ResXDataNode("TextFile1", fileRef) }
        };

        var compileUnit = StronglyTypedResourceBuilder.Create(
            resourceList: values,
            baseName: "Resources",
            generatedCodeNamespace: "Namespace",
            s_cSharpProvider,
            internalClass: false,
            out _);

        MemoryStream resxStream = new();
        using ResXResourceWriter resxWriter = new(resxStream);
        resxWriter.AddResource(new ResXDataNode("TextFile1", fileRef));
        resxWriter.Generate();
        resxStream.Position = 0;
        using ResXResourceReader reader = new(resxStream);
        validateResultTxtFileContent(getPropertyInfo(reader.GetEnumerator(), compileUnit, "TextFile1"));
    }

    private static void validateResultAudio(PropertyInfo audioPropertyInfo)
    {
        using UnmanagedMemoryStream resourceAudio =
            Assert.IsType<UnmanagedMemoryStream>(audioPropertyInfo.GetValue(obj: null));
        var contents = new byte[resourceAudio.Length];
        int pos = (int)(resourceAudio.Position = 0);
        while (pos < resourceAudio.Length)
        {
            pos += resourceAudio.Read(contents, pos, (int)(resourceAudio.Length - pos));
        }

        Assert.Equal("HELLO", Encoding.UTF8.GetString(contents));
    }

    [Fact]
    public static void StronglyTypedResourceBuilder_Create_AudioResource_FromFile()
    {
        const string data = $"""
            <data name="Audio1" type="System.Resources.ResXFileRef, System.Windows.Forms">
              <value>Resources\Audio1.wav;System.IO.MemoryStream, {TypeAssembly}</value>
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

        using ResXResourceReader reader = new(temp.Path);
        validateResultAudio(getPropertyInfo(reader.GetEnumerator(), compileUnit, "Audio1"));
    }

    [Fact]
    public static void StronglyTypedResourceBuilder_Create_AudioResource_FromFileRef()
    {
        ResXFileRef fileRef = new(@"Resources\Audio1.wav",
            $"System.IO.MemoryStream, {TypeAssembly}");
        Hashtable values = new()
        {
            { "Audio1", new ResXDataNode("Audio1", fileRef) }
        };

        var compileUnit = StronglyTypedResourceBuilder.Create(
            resourceList: values,
            baseName: "Resources",
            generatedCodeNamespace: "Namespace",
            s_cSharpProvider,
            internalClass: false,
            out _);

        MemoryStream resxStream = new();
        using ResXResourceWriter resxWriter = new(resxStream);
        resxWriter.AddResource(new ResXDataNode("Audio1", fileRef));
        resxWriter.Generate();
        resxStream.Position = 0;
        using ResXResourceReader reader = new(resxStream);
        validateResultAudio(getPropertyInfo(reader.GetEnumerator(), compileUnit, "Audio1"));
    }
}
