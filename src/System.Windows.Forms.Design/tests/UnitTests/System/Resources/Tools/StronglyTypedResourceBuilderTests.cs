// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Text;
using System.Windows.Forms.TestUtilities;
using AxWMPLib;
using Microsoft.CSharp;

namespace System.Resources.Tools.Tests;

public partial class StronglyTypedResourceBuilderTests
{
    // https://docs.microsoft.com/dotnet/core/extensions/work-with-resx-files-programmatically

    private static readonly CodeDomProvider s_cSharpProvider = new CSharpCodeProvider();
    private const string TypeAssembly = "mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
    private const string TxtFileEncoding = "Windows-1252";

    [Fact]
    public void StronglyTypedResourceBuilder_Create_NullBaseName_ThrowsArgumentNull()
    {
        Hashtable resources = [];
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

        using var temp = TempFile.Create(ResxHelper.CreateResx());
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
        Hashtable resources = [];
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

        using var temp = TempFile.Create(ResxHelper.CreateResx());
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
        Hashtable resources = [];
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
        using var temp = TempFile.Create(ResxHelper.CreateResx());
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

        using var temp = TempFile.Create(ResxHelper.CreateResx(data));
        var compileUnit = StronglyTypedResourceBuilder.Create(
            resxFile: temp.Path,
            baseName: "Resources",
            generatedCodeNamespace: "Namespace",
            s_cSharpProvider,
            internalClass: false,
            out _);

        using MemoryStream resourceStream = new();
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

        using MemoryStream resourceStream = new();
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

        using MemoryStream resourceStream = new();
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
        const string data = $"""
            <data name="Image1" type="System.Resources.ResXFileRef, System.Windows.Forms">
                <value>Resources\Image1.png;System.Byte[], {TypeAssembly}</value>
            </data>
            """;

        using var temp = TempFile.Create(ResxHelper.CreateResx(data));

        var compileUnit = StronglyTypedResourceBuilder.Create(
            resxFile: temp.Path,
            baseName: "Resources",
            generatedCodeNamespace: "Namespace",
            s_cSharpProvider,
            internalClass: false,
            out _);

        using ResXResourceReader reader = new(temp.Path);
        var imagePropertyInfo = CompileAndGetPropertyInfo(reader.GetEnumerator(), compileUnit, "Image1");
        using Bitmap expected = (Bitmap)Image.FromFile(@"Resources\Image1.png");
        ValidateResultBitmap(imagePropertyInfo, expected, TypeDescriptor.GetConverter(typeof(Bitmap)));
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
        var imagePropertyInfo = CompileAndGetPropertyInfo(reader.GetEnumerator(), compileUnit, "Image1");
        ValidateResultBitmap(imagePropertyInfo, bitmap, converter);
    }

    [Fact]
    public static void StronglyTypedResourceBuilder_Create_BitmapResource_FromFileRef()
    {
        ResXFileRef fileRef = new(@"Resources\Image1.png", $"System.Byte[], {TypeAssembly}");
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

        using MemoryStream resxStream = new();
        using ResXResourceWriter resxWriter = new(resxStream);
        resxWriter.AddResource(new ResXDataNode("Image1", fileRef));
        resxWriter.Generate();
        resxStream.Position = 0;
        using ResXResourceReader reader = new(resxStream);
        var imagePropertyInfo = CompileAndGetPropertyInfo(reader.GetEnumerator(), compileUnit, "Image1");
        using Bitmap expected = (Bitmap)Image.FromFile(@"Resources\Image1.png");
        ValidateResultBitmap(imagePropertyInfo, expected, TypeDescriptor.GetConverter(typeof(Bitmap)));
    }

    [Fact]
    public static void StronglyTypedResourceBuilder_Create_IconResource_FromFile()
    {
        const string data = $"""
            <data name="Icon1" type="System.Resources.ResXFileRef, System.Windows.Forms">
              <value>Resources\Icon1.ico;System.Byte[], {TypeAssembly}</value>
            </data>
            """;

        using var temp = TempFile.Create(ResxHelper.CreateResx(data));

        var compileUnit = StronglyTypedResourceBuilder.Create(
            resxFile: temp.Path,
            baseName: "Resources",
            generatedCodeNamespace: "Namespace",
            s_cSharpProvider,
            internalClass: false,
            out _);

        using ResXResourceReader reader = new(temp.Path);
        var iconPropertyInfo = CompileAndGetPropertyInfo(reader.GetEnumerator(), compileUnit, "Icon1");
        using Icon expected = new(@"Resources\Icon1.ico");
        ValidateResultIcon(iconPropertyInfo, expected, TypeDescriptor.GetConverter(typeof(Icon)));
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
        var iconPropertyInfo = CompileAndGetPropertyInfo(reader.GetEnumerator(), compileUnit, "Icon1");
        ValidateResultIcon(iconPropertyInfo, icon, converter);
    }

    [Fact]
    public static void StronglyTypedResourceBuilder_Create_IconResource_FromFileRef()
    {
        ResXFileRef fileRef = new(@"Resources\Icon1.ico", $"System.Byte[], {TypeAssembly}");
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

        using MemoryStream resxStream = new();
        using ResXResourceWriter resxWriter = new(resxStream);
        resxWriter.AddResource(new ResXDataNode("Icon1", fileRef));
        resxWriter.Generate();
        resxStream.Position = 0;
        using ResXResourceReader reader = new(resxStream);
        var iconPropertyInfo = CompileAndGetPropertyInfo(reader.GetEnumerator(), compileUnit, "Icon1");
        using Icon expected = new(@"Resources\Icon1.ico");
        ValidateResultIcon(iconPropertyInfo, expected, TypeDescriptor.GetConverter(typeof(Icon)));
    }

    [Fact]
    public static void StronglyTypedResourceBuilder_Create_TxtFileResource_FromFile()
    {
        const string data = $"""
            <data name="TextFile1" type="System.Resources.ResXFileRef, System.Windows.Forms">
              <value>Resources\TextFile1.txt;System.String, {TypeAssembly};{TxtFileEncoding}</value>
            </data>
            """;

        using var temp = TempFile.Create(ResxHelper.CreateResx(data));
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        var compileUnit = StronglyTypedResourceBuilder.Create(
            resxFile: temp.Path,
            baseName: "Resources",
            generatedCodeNamespace: "Namespace",
            s_cSharpProvider,
            internalClass: false,
            out _);

        using ResXResourceReader reader = new(temp.Path);
        ValidateResultTxtFileContent(CompileAndGetPropertyInfo(reader.GetEnumerator(), compileUnit, "TextFile1"));
    }

    [Fact]
    public static void StronglyTypedResourceBuilder_Create_TxtFileResource_FromFileRef()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        ResXFileRef fileRef = new(
            @"Resources\TextFile1.txt",
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

        using MemoryStream resxStream = new();
        using ResXResourceWriter resxWriter = new(resxStream);
        resxWriter.AddResource(new ResXDataNode("TextFile1", fileRef));
        resxWriter.Generate();
        resxStream.Position = 0;
        using ResXResourceReader reader = new(resxStream);
        ValidateResultTxtFileContent(CompileAndGetPropertyInfo(reader.GetEnumerator(), compileUnit, "TextFile1"));
    }

    [Fact]
    public static void StronglyTypedResourceBuilder_Create_AudioResource_FromFile()
    {
        const string data = $"""
            <data name="Audio1" type="System.Resources.ResXFileRef, System.Windows.Forms">
              <value>Resources\Audio1.wav;System.IO.MemoryStream, {TypeAssembly}</value>
            </data>
            """;

        using var temp = TempFile.Create(ResxHelper.CreateResx(data));

        var compileUnit = StronglyTypedResourceBuilder.Create(
            resxFile: temp.Path,
            baseName: "Resources",
            generatedCodeNamespace: "Namespace",
            s_cSharpProvider,
            internalClass: false,
            out _);

        using ResXResourceReader reader = new(temp.Path);
        ValidateResultAudio(CompileAndGetPropertyInfo(reader.GetEnumerator(), compileUnit, "Audio1"));
    }

    [Fact]
    public static void StronglyTypedResourceBuilder_Create_AudioResource_FromFileRef()
    {
        ResXFileRef fileRef = new(@"Resources\Audio1.wav", $"System.IO.MemoryStream, {TypeAssembly}");
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

        using MemoryStream resxStream = new();
        using ResXResourceWriter resxWriter = new(resxStream);
        resxWriter.AddResource(new ResXDataNode("Audio1", fileRef));
        resxWriter.Generate();
        resxStream.Position = 0;
        using ResXResourceReader reader = new(resxStream);
        ValidateResultAudio(CompileAndGetPropertyInfo(reader.GetEnumerator(), compileUnit, "Audio1"));
    }

    [Fact]
    public static void StronglyTypedResourceBuilder_VerifyResourceName_ValidName()
    {
        string key = "MyResource";
        string result = StronglyTypedResourceBuilder.VerifyResourceName(key, s_cSharpProvider);
        Assert.Equal(key, result);
    }

    [Fact]
    public static void StronglyTypedResourceBuilder_VerifyResourceName_InvalidName()
    {
        string key = "Invalid Resource?";
        string result = StronglyTypedResourceBuilder.VerifyResourceName(key, s_cSharpProvider);
        Assert.Equal("Invalid_Resource_", result);
    }

    [Fact]
    public static void StronglyTypedResourceBuilder_VerifyResourceName_NameWithSpaces()
    {
        string key = "Resource Name";
        string result = StronglyTypedResourceBuilder.VerifyResourceName(key, s_cSharpProvider);
        Assert.Equal("Resource_Name", result);
    }

    [Fact]
    public static void StronglyTypedResourceBuilder_VerifyResourceName_NameStartWithNumber()
    {
        string key = "1.name";
        string result = StronglyTypedResourceBuilder.VerifyResourceName(key, s_cSharpProvider);
        Assert.Equal("_1_name", result);
    }

    [Theory]
    [MemberData(nameof(ResourceName_TestData))]
    public static void StronglyTypedResourceBuilder_VerifyResourceName_NameWithSpecialCharacters(string resourceName, string expectedResult)
    {
        string result = StronglyTypedResourceBuilder.VerifyResourceName(resourceName, s_cSharpProvider);
        Assert.Equal(expectedResult, result);
    }

    [WinFormsFact]
    public static void StronglyTypedResourceBuilder_Create_AxHost_FromMemory_SerializeWith_StateConverter()
    {
        // AxHost.StateConverter is not properly registered as a converter for AxHost.State.
        // Temporarily register StateConverter as State's converter and test serialization.
        TypeConverter converter = TypeDescriptor.GetConverter(typeof(AxHost.State));
        Assert.Equal(typeof(TypeConverter), converter.GetType());
        using var scope = CustomConverter.RegisterConverter(typeof(AxHost.State), new AxHost.StateConverter());
        converter = TypeDescriptor.GetConverter(typeof(AxHost.State));
        Assert.Equal(typeof(AxHost.StateConverter), converter.GetType());

        using Form form = new();
        using AxWindowsMediaPlayer mediaPlayer = new();
        ((ISupportInitialize)mediaPlayer).BeginInit();
        form.Controls.Add(mediaPlayer);
        ((ISupportInitialize)mediaPlayer).EndInit();

        string expectedUrl = $"{Path.GetTempPath()}testurl1";
        mediaPlayer.URL = expectedUrl;

        ResXDataNode node = new("MediaPlayer1", converter.ConvertTo(mediaPlayer.OcxState, typeof(byte[])));
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
        var mediaPlayerPropertyInfo = CompileAndGetPropertyInfo(reader.GetEnumerator(), compileUnit, "MediaPlayer1");
        byte[] resourceByte = Assert.IsType<byte[]>(mediaPlayerPropertyInfo.GetValue(obj: null));
        AxHost.State state = Assert.IsType<AxHost.State>(converter.ConvertFrom(resourceByte));

        string changedUrl = $"{Path.GetTempPath()}testurl2";
        mediaPlayer.URL = changedUrl;
        Assert.Equal(changedUrl, mediaPlayer.URL);

        mediaPlayer.OcxState = state;
        Assert.Equal(expectedUrl, mediaPlayer.URL);
    }

    // Utilizes ResourceWriter to save the resources and gets the specified
    // PropertyInfo.
    private static PropertyInfo CompileAndGetPropertyInfo(
        IDictionaryEnumerator resources,
        CodeCompileUnit compileUnit,
        string propertyName)
    {
        using MemoryStream resourceStream = new();
        using ResourceWriter resourceWriter = new(resourceStream);
        while (resources.MoveNext())
        {
            resourceWriter.AddResource((string)resources.Key, resources.Value);
        }

        resourceWriter.Generate();
        resourceStream.Position = 0;

        Type type = CodeDomCompileHelper.CompileClass(compileUnit, "Resources", "Namespace", resourceStream);
        Assert.NotNull(type);
        var propertyInfo = type.GetProperty(propertyName);
        Assert.NotNull(propertyInfo);
        return propertyInfo;
    }

    private static void ValidateResultBitmap(PropertyInfo imagePropertyInfo, Bitmap expected, TypeConverter converter)
    {
        byte[] resourceBytes = Assert.IsType<byte[]>(imagePropertyInfo.GetValue(obj: null));
        using Bitmap resourceBitmap = Assert.IsType<Bitmap>(converter.ConvertFrom(resourceBytes));
        Assert.Equal(expected.Size, resourceBitmap.Size);
        Assert.Equal(expected.GetPixel(0, 0), resourceBitmap.GetPixel(0, 0));
    }

    private static void ValidateResultIcon(PropertyInfo iconPropertyInfo, Icon expected, TypeConverter converter)
    {
        byte[] resourceByte = Assert.IsType<byte[]>(iconPropertyInfo.GetValue(obj: null));
        using Icon resourceIcon = Assert.IsType<Icon>(converter.ConvertFrom(resourceByte));
        Assert.Equal(expected.Size, resourceIcon.Size);
    }

    private static void ValidateResultTxtFileContent(PropertyInfo txtFilePropertyInfo)
    {
        string resourceTxtFileContents = Assert.IsType<string>(txtFilePropertyInfo.GetValue(obj: null));
        Assert.Equal("hello test\r\n!", resourceTxtFileContents);
    }

    private static void ValidateResultAudio(PropertyInfo audioPropertyInfo)
    {
        using UnmanagedMemoryStream resourceAudio =
            Assert.IsType<UnmanagedMemoryStream>(audioPropertyInfo.GetValue(obj: null));
        byte[] contents = new byte[resourceAudio.Length];
        int pos = (int)(resourceAudio.Position = 0);
        while (pos < resourceAudio.Length)
        {
            pos += resourceAudio.Read(contents, pos, (int)(resourceAudio.Length - pos));
        }

        Assert.Equal("HELLO", Encoding.UTF8.GetString(contents));
    }

    public static IEnumerable<object[]> ResourceName_TestData()
    {
        yield return new object[] { "Image#Jpeg", "Image_Jpeg" };
        yield return new object[] { "Generate &method", "Generate__method" };
        yield return new object[] { "'%s' in this scope", "__s__in_this_scope" };
        yield return new object[] { "{ ... }", "_______" };
    }
}
