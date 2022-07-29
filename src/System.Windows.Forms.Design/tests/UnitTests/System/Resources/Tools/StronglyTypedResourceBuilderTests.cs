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
        var converter = TypeDescriptor.GetConverter(typeof(Bitmap));
        Bitmap bitmap = (Bitmap)Image.FromFile(@"Resources\Image1.png");

        Type type = CodeDomCompileHelper.CompileClass(compileUnit, "Resources", "Namespace", resourceStream);
        Assert.NotNull(type);
        var imageProperty = type.GetProperty("Image1");
        Assert.NotNull(imageProperty);
        byte[] resourceBytes = Assert.IsType<byte[]>(imageProperty.GetValue(obj: null));
        Bitmap resourceBitmap = Assert.IsType<Bitmap>(converter.ConvertFrom(resourceBytes));
        Assert.Equal(bitmap.Size, resourceBitmap.Size);
        Assert.Equal(bitmap.GetPixel(0, 0), resourceBitmap.GetPixel(0, 0));
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
        Bitmap resourceBitmap = Assert.IsType<Bitmap>(converter.ConvertFrom(resourceBytes));
        Assert.Equal(bitmap.GetPixel(0, 0), resourceBitmap.GetPixel(0, 0));
        Assert.Equal(bitmap.Size, resourceBitmap.Size);
    }

    [Fact]
    public static void StronglyTypedResourceBuilder_Create_BitmapResource_FromFileRef()
    {
        ResXFileRef fileRef = new(@"Resources\Image1.png",
            "System.Byte[], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
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
        ResXResourceWriter resxWriter = new(resxStream);
        resxWriter.AddResource(new ResXDataNode("Image1", fileRef));
        resxWriter.Generate();
        resxStream.Position = 0;
        
        MemoryStream resourceStream = new();
        using ResXResourceReader reader = new(resxStream);
        var enumerator = reader.GetEnumerator();
        using ResourceWriter resourceWriter = new(resourceStream);

        while (enumerator.MoveNext())
        {
            resourceWriter.AddResource((string)enumerator.Key, enumerator.Value);
        }

        resourceWriter.Generate();
        resourceStream.Position = 0;
        var converter = TypeDescriptor.GetConverter(typeof(Image));
        Bitmap bitmap = (Bitmap)Image.FromFile(@"Resources\Image1.png");

        Type type = CodeDomCompileHelper.CompileClass(compileUnit, "Resources", "Namespace", resourceStream);
        Assert.NotNull(type);
        var imageProperty = type.GetProperty("Image1");
        Assert.NotNull(imageProperty);
        byte[] resourceBytes = Assert.IsType<byte[]>(imageProperty.GetValue(obj: null));
        Bitmap resouceBitmap = Assert.IsType<Bitmap>(converter.ConvertFrom(resourceBytes));
        Assert.Equal(bitmap.GetPixel(0, 0), resouceBitmap.GetPixel(0, 0));
        Assert.Equal(bitmap.Size, resouceBitmap.Size);
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
        var converter = TypeDescriptor.GetConverter(typeof(Icon));
        Icon icon = new(@"Resources\Icon1.ico");

        Type type = CodeDomCompileHelper.CompileClass(compileUnit, "Resources", "Namespace", resourceStream);
        Assert.NotNull(type);
        var iconProperty = type.GetProperty("Icon1");
        Assert.NotNull(iconProperty);
        byte[] resourceByte = Assert.IsType<byte[]>(iconProperty.GetValue(obj: null));
        Icon resourceIcon = Assert.IsType<Icon>(converter.ConvertFrom(resourceByte));
        Assert.Equal(icon.Size, resourceIcon.Size);
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
        Icon resourceIcon = Assert.IsType<Icon>(converter.ConvertFrom(resourceBytes));
        Assert.Equal(icon.Size, resourceIcon.Size);
    }

    [Fact]
    public static void StronglyTypedResourceBuilder_Create_IconResource_FromFileRef()
    {
        ResXFileRef fileRef = new(@"Resources\Icon1.ico",
            "System.Byte[], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
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
        ResXResourceWriter resxWriter = new(resxStream);
        resxWriter.AddResource(new ResXDataNode("Icon1", fileRef));
        resxWriter.Generate();
        resxStream.Position = 0;

        MemoryStream resourceStream = new();
        using ResXResourceReader reader = new(resxStream);
        var enumerator = reader.GetEnumerator();
        using ResourceWriter resourceWriter = new(resourceStream);

        while (enumerator.MoveNext())
        {
            resourceWriter.AddResource((string)enumerator.Key, enumerator.Value);
        }

        resourceWriter.Generate();
        resourceStream.Position = 0;
        var converter = TypeDescriptor.GetConverter(typeof(Icon));
        Icon icon = new(@"Resources\Icon1.ico");

        Type type = CodeDomCompileHelper.CompileClass(compileUnit, "Resources", "Namespace", resourceStream);
        Assert.NotNull(type);
        var iconProperty = type.GetProperty("Icon1");
        Assert.NotNull(iconProperty);
        byte[] resourceBytes = Assert.IsType<byte[]>(iconProperty.GetValue(obj: null));
        Icon resourceIcon = Assert.IsType<Icon>(converter.ConvertFrom(resourceBytes));
        Assert.Equal(icon.Size, resourceIcon.Size);
    }

    [Fact]
    public static void StronglyTypedResourceBuilder_Create_TxtFileResource_FromFile()
    {
        const string data = """
            <data name="TextFile1" type="System.Resources.ResXFileRef, System.Windows.Forms">
              <value>Resources\TextFile1.txt;System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089;Windows-1252</value>
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
        var txtFileProperty = type.GetProperty("TextFile1");
        Assert.NotNull(txtFileProperty);
        string resourceTxtFileContents = Assert.IsType<string>(txtFileProperty.GetValue(obj: null));
        Assert.Equal("hello test\r\n!", resourceTxtFileContents);
    }

    [Fact]
    public static void StronglyTypedResourceBuilder_Create_TxtFileResource_FromFileRef()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        ResXFileRef fileRef = new(@"Resources\TextFile1.txt",
            "System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089",
            Encoding.GetEncoding("Windows-1252"));
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
        ResXResourceWriter resxWriter = new(resxStream);
        resxWriter.AddResource(new ResXDataNode("TextFile1", fileRef));
        resxWriter.Generate();
        resxStream.Position = 0;

        MemoryStream resourceStream = new();
        using ResXResourceReader reader = new(resxStream);
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
        var txtFileProperty = type.GetProperty("TextFile1");
        Assert.NotNull(txtFileProperty);
        string resourceTxtFileContents = Assert.IsType<string>(txtFileProperty.GetValue(obj: null));
        Assert.Equal("hello test\r\n!", resourceTxtFileContents);
    }

    [Fact]
    public static void StronglyTypedResourceBuilder_Create_AudioResource_FromFile()
    {
        const string data = """
            <data name="Audio1" type="System.Resources.ResXFileRef, System.Windows.Forms">
              <value>Resources\Audio1.wav;System.IO.MemoryStream, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</value>
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
        var audioProperty = type.GetProperty("Audio1");
        Assert.NotNull(audioProperty);
        UnmanagedMemoryStream resourceUms = Assert.IsType<UnmanagedMemoryStream>(audioProperty.GetValue(obj: null));
        var contents = new byte[resourceUms.Length];
        int pos = (int)(resourceUms.Position = 0);
        while (pos < resourceUms.Length)
        {
            pos += resourceUms.Read(contents, pos, (int)(resourceUms.Length - pos));
        }

        Assert.Equal("HELLO", Encoding.UTF8.GetString(contents));
    }

    [Fact]
    public static void StronglyTypedResourceBuilder_Create_AudioResource_FromFileRef()
    {
        ResXFileRef fileRef = new(@"Resources\Audio1.wav",
            "System.IO.MemoryStream, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
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
        ResXResourceWriter resxWriter = new(resxStream);
        resxWriter.AddResource(new ResXDataNode("Audio1", fileRef));
        resxWriter.Generate();
        resxStream.Position = 0;

        MemoryStream resourceStream = new();
        using ResXResourceReader reader = new(resxStream);
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
        var audioProperty = type.GetProperty("Audio1");
        Assert.NotNull(audioProperty);
        UnmanagedMemoryStream resourceUms = Assert.IsType<UnmanagedMemoryStream>(audioProperty.GetValue(obj: null));
        var contents = new byte[resourceUms.Length];
        int pos = (int)(resourceUms.Position = 0);
        while (pos < resourceUms.Length)
        {
            pos += resourceUms.Read(contents, pos, (int)(resourceUms.Length - pos));
        }

        Assert.Equal("HELLO", Encoding.UTF8.GetString(contents));
    }
}
