// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using AxWMPLib;
using Microsoft.CSharp;
using Xunit;

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

        using var temp = TempFile.Create(CreateResx(data));

        var compileUnit = StronglyTypedResourceBuilder.Create(
            resxFile: temp.Path,
            baseName: "Resources",
            generatedCodeNamespace: "Namespace",
            s_cSharpProvider,
            internalClass: false,
            out _);

        using ResXResourceReader reader = new(temp.Path);
        var imagePropertyInfo = GetPropertyInfo(reader.GetEnumerator(), compileUnit, "Image1");
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
        var imagePropertyInfo = GetPropertyInfo(reader.GetEnumerator(), compileUnit, "Image1");
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
        var imagePropertyInfo = GetPropertyInfo(reader.GetEnumerator(), compileUnit, "Image1");
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

        using var temp = TempFile.Create(CreateResx(data));

        var compileUnit = StronglyTypedResourceBuilder.Create(
            resxFile: temp.Path,
            baseName: "Resources",
            generatedCodeNamespace: "Namespace",
            s_cSharpProvider,
            internalClass: false,
            out _);

        using ResXResourceReader reader = new(temp.Path);
        var iconPropertyInfo = GetPropertyInfo(reader.GetEnumerator(), compileUnit, "Icon1");
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
        var iconPropertyInfo = GetPropertyInfo(reader.GetEnumerator(), compileUnit, "Icon1");
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
        var iconPropertyInfo = GetPropertyInfo(reader.GetEnumerator(), compileUnit, "Icon1");
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
        ValidateResultTxtFileContent(GetPropertyInfo(reader.GetEnumerator(), compileUnit, "TextFile1"));
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
        ValidateResultTxtFileContent(GetPropertyInfo(reader.GetEnumerator(), compileUnit, "TextFile1"));
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
        ValidateResultAudio(GetPropertyInfo(reader.GetEnumerator(), compileUnit, "Audio1"));
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
        ValidateResultAudio(GetPropertyInfo(reader.GetEnumerator(), compileUnit, "Audio1"));
    }

    [WinFormsFact]
    public static void StronglyTypedResourceBuilder_Create_OcxState_FromMemory()
    {
        TypeConverter converter = TypeDescriptor.GetConverter(typeof(AxHost.State));
        Assert.Equal(typeof(TypeConverter), converter.GetType());

        TypeDescriptionProvider parentProvider = TypeDescriptor.GetProvider(typeof(AxHost.State));
        TypeDescriptionProvider newProvider = new AxHostStateTypeDescriptionProvider(parentProvider);
        try
        {
            TypeDescriptor.AddProvider(newProvider, typeof(AxHost.State));
            TypeConverter newConverter = TypeDescriptor.GetConverter(typeof(AxHost.State));
            Assert.Equal(typeof(AxHost.StateConverter), newConverter.GetType());

            using Form form = new();
            using AxWindowsMediaPlayer mediaPlayer = new();
            ((ISupportInitialize)mediaPlayer).BeginInit();
            form.Controls.Add(mediaPlayer);
            ((ISupportInitialize)mediaPlayer).EndInit();

            string expectedUrl = $"{Path.GetTempPath()}testurl1";
            mediaPlayer.URL = expectedUrl;

            ResXDataNode node = new("MediaPlayer1", newConverter.ConvertTo(mediaPlayer.OcxState, typeof(byte[])));
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
            var mediaPlayerPropertyInfo = GetPropertyInfo(reader.GetEnumerator(), compileUnit, "MediaPlayer1");
            byte[] resourceByte = Assert.IsType<byte[]>(mediaPlayerPropertyInfo.GetValue(obj: null));
            AxHost.State state = Assert.IsType<AxHost.State>(newConverter.ConvertFrom(resourceByte));

            string changedUrl = $"{Path.GetTempPath()}testurl2";
            mediaPlayer.URL = changedUrl;
            Assert.Equal(changedUrl, mediaPlayer.URL);

            mediaPlayer.OcxState = state;
            Assert.Equal(expectedUrl, mediaPlayer.URL);
        }
        finally
        {
            TypeDescriptor.RemoveProvider(newProvider, typeof(AxHost.State));
        }
    }

    public class AxHostStateTypeDescriptionProvider : TypeDescriptionProvider
    {
        public AxHostStateTypeDescriptionProvider(TypeDescriptionProvider parent) : base(parent)
        {
        }

        public override ICustomTypeDescriptor GetTypeDescriptor(
            [DynamicallyAccessedMembers((DynamicallyAccessedMemberTypes)(-1))] Type objectType,
            object instance) => new TypeConverterProvider(base.GetTypeDescriptor(objectType, instance));

        private class TypeConverterProvider : CustomTypeDescriptor
        {
            private static TypeConverter s_converter = new AxHost.StateConverter();
            public TypeConverterProvider(ICustomTypeDescriptor parent) : base(parent) { }
            public override TypeConverter GetConverter() => s_converter;
        }
    }

    private static PropertyInfo GetPropertyInfo(
        IDictionaryEnumerator enumerator,
        CodeCompileUnit compileUnit,
        string propertyName)
    {
        using MemoryStream resourceStream = new();
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
        var contents = new byte[resourceAudio.Length];
        int pos = (int)(resourceAudio.Position = 0);
        while (pos < resourceAudio.Length)
        {
            pos += resourceAudio.Read(contents, pos, (int)(resourceAudio.Length - pos));
        }

        Assert.Equal("HELLO", Encoding.UTF8.GetString(contents));
    }
}
