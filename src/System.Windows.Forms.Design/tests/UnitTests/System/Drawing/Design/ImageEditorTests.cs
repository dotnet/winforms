// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing.Imaging;
using System.Windows.Forms.TestUtilities;

namespace System.Drawing.Design.Tests;

public class ImageEditorTests
{
    [Fact]
    public void ImageEditor_Ctor_Default()
    {
        ImageEditor editor = new();
        Assert.False(editor.IsDropDownResizable);
    }

    public static IEnumerable<object[]> CreateExtensionsString_TestData()
    {
        yield return new object[] { null, ",", null };
        yield return new object[] { Array.Empty<string>(), ",", null };
        yield return new object[] { new string[] { "a", "b", "c" }, ",", "*.a,*.b,*.c" };
        yield return new object[] { new string[] { "a", "b", "c" }, "", "*.a*.b*.c" };
        yield return new object[] { new string[] { "a", "b", "c" }, null, "*.a*.b*.c" };
        yield return new object[] { new string[] { null, null, null }, ",", "" };
        yield return new object[] { new string[] { string.Empty, string.Empty, string.Empty }, ",", "" };
    }

    [Theory]
    [MemberData(nameof(CreateExtensionsString_TestData))]
    public void ImageEditor_CreateExtensionsString_Invoke_ReturnsExpected(string[] extensions, string sep, string expected)
    {
        Assert.Equal(expected, SubImageEditor.CreateExtensionsString(extensions, sep));
    }

    [Fact]
    public void ImageEditor_CreateFilterEntry_Invoke_CallsGetExtensionsOnce()
    {
        CustomGetImageExtendersEditor editor = new()
        {
            GetImageExtendersResult = [typeof(PublicImageEditor), typeof(PrivateImageEditor)]
        };
        Assert.Equal("CustomGetImageExtendersEditor(*.PublicImageEditor,*.PrivateImageEditor)|*.PublicImageEditor;*.PrivateImageEditor", SubImageEditor.CreateFilterEntry(editor));
        Assert.Equal(1, editor.GetImageExtendersCallCount);
    }

    [Fact]
    public void ImageEditor_CreateFilterEntry_NullE_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>("e", () => SubImageEditor.CreateFilterEntry(null));
    }

    [Theory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetEditValueInvalidProviderTestData))]
    public void ImageEditor_EditValue_InvalidProvider_ReturnsValue(IServiceProvider provider, object value)
    {
        ImageEditor editor = new();
        Assert.Same(value, editor.EditValue(null, provider, value));
    }

    [Theory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetITypeDescriptorContextTestData))]
    public void ImageEditor_GetEditStyle_Invoke_ReturnsModal(ITypeDescriptorContext context)
    {
        ImageEditor editor = new();
        Assert.Equal(UITypeEditorEditStyle.Modal, editor.GetEditStyle(context));
    }

    [Fact]
    public void ImageEditor_GetExtensions_InvokeDefault_ReturnsExpected()
    {
        SubImageEditor editor = new();
        string[] extensions = editor.GetExtensions();
        Assert.Equal(new string[] { "bmp", "gif", "jpg", "jpeg", "png", "ico", "emf", "wmf" }, extensions);
        Assert.NotSame(extensions, editor.GetExtensions());
    }

    [Fact]
    public void ImageEditor_GetExtensions_InvokeCustom_CallsGetImageExtendersOnce()
    {
        CustomGetImageExtendersEditor editor = new()
        {
            GetImageExtendersResult = [typeof(PublicImageEditor), typeof(PrivateImageEditor), typeof(ImageEditor), typeof(NullExtensionsImageEditor)]
        };
        Assert.Equal(new string[] { "PublicImageEditor", "PrivateImageEditor" }, editor.GetExtensions());
        Assert.Equal(1, editor.GetImageExtendersCallCount);
    }

    [Fact]
    public void ImageEditor_GetExtensions_InvokeInvalid_ReturnsExpected()
    {
        CustomGetImageExtendersEditor editor = new()
        {
            GetImageExtendersResult = [typeof(object), null]
        };
        Assert.Empty(editor.GetExtensions());
        Assert.Equal(1, editor.GetImageExtendersCallCount);
    }

    [Fact]
    public void ImageEditor_GetFileDialogDescription_Invoke_ReturnsExpected()
    {
        SubImageEditor editor = new();
        Assert.Equal("All image files", editor.GetFileDialogDescription());
    }

    [Fact]
    public void ImageEditor_GetImageExtenders_Invoke_ReturnsExpected()
    {
        SubImageEditor editor = new();
        Type[] extenders = editor.GetImageExtenders();
        Assert.Equal([typeof(BitmapEditor), typeof(MetafileEditor)], extenders);
        Assert.Same(extenders, editor.GetImageExtenders());
    }

    [Theory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetITypeDescriptorContextTestData))]
    public void ImageEditor_GetPaintValueSupported_Invoke_ReturnsTrue(ITypeDescriptorContext context)
    {
        ImageEditor editor = new();
        Assert.True(editor.GetPaintValueSupported(context));
    }

    [Fact]
    public void ImageEditor_LoadFromStream_BitmapStream_ReturnsExpected()
    {
        SubImageEditor editor = new();
        using MemoryStream stream = new();
        using Bitmap image = new(10, 10);
        image.Save(stream, ImageFormat.Bmp);
        stream.Position = 0;
        Bitmap result = Assert.IsType<Bitmap>(editor.LoadFromStream(stream));
        Assert.Equal(new Size(10, 10), result.Size);

        using MemoryStream resultStream = new();
        result.Save(resultStream, ImageFormat.Bmp);
        Assert.Equal(stream.Length, resultStream.Length);
    }

    [Fact]
    public void ImageEditor_LoadFromStream_MetafileStream_ThrowsArgumentException()
    {
        SubImageEditor editor = new();
        using Stream stream = File.OpenRead("Resources/telescope_01.wmf");
        Assert.Throws<ArgumentException>(() => editor.LoadFromStream(stream));
    }

    [Fact]
    public void ImageEditor_LoadFromStream_NullStream_ThrowsArgumentNullException()
    {
        SubImageEditor editor = new();
        Assert.Throws<ArgumentNullException>("stream", () => editor.LoadFromStream(null));
    }

    [Fact]
    public void ImageEditor_PaintValue_Invoke_Success()
    {
        ImageEditor editor = new();
        using Bitmap image = new(10, 10);
        using Bitmap otherImage = new(3, 2);
        using Graphics graphics = Graphics.FromImage(image);
        otherImage.SetPixel(0, 0, Color.Red);
        otherImage.SetPixel(1, 0, Color.Red);
        otherImage.SetPixel(2, 0, Color.Red);
        otherImage.SetPixel(0, 1, Color.Red);
        otherImage.SetPixel(1, 1, Color.Red);
        otherImage.SetPixel(2, 1, Color.Red);

        PaintValueEventArgs e = new(null, otherImage, graphics, new Rectangle(1, 2, 3, 4));
        editor.PaintValue(e);
    }

    public static IEnumerable<object[]> PaintValue_InvalidArgsValue_TestData()
    {
        yield return new object[] { null };
        yield return new object[] { new() };
    }

    [Theory]
    [MemberData(nameof(PaintValue_InvalidArgsValue_TestData))]
    public void ImageEditor_PaintValue_InvalidArgsValue_Nop(object value)
    {
        ImageEditor editor = new();
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        PaintValueEventArgs e = new(null, value, graphics, new Rectangle(1, 2, 3, 4));
        editor.PaintValue(e);
    }

    [Fact]
    public void ImageEditor_PaintValue_NullE_Nop()
    {
        ImageEditor editor = new();
        editor.PaintValue(null);
    }

    private class SubImageEditor : ImageEditor
    {
        public static new string CreateExtensionsString(string[] extensions, string sep)
        {
            return ImageEditor.CreateExtensionsString(extensions, sep);
        }

        public static new string CreateFilterEntry(ImageEditor e)
        {
            return ImageEditor.CreateFilterEntry(e);
        }

        public new string[] GetExtensions() => base.GetExtensions();

        public new string GetFileDialogDescription() => base.GetFileDialogDescription();

        public new Type[] GetImageExtenders() => base.GetImageExtenders();

        public new Image LoadFromStream(Stream stream) => base.LoadFromStream(stream);
    }

    private class CustomGetImageExtendersEditor : ImageEditor
    {
        public int GetImageExtendersCallCount { get; set; }

        public Type[] GetImageExtendersResult { get; set; }

        public new string[] GetExtensions() => base.GetExtensions();

        protected override string GetFileDialogDescription() => "CustomGetImageExtendersEditor";

        protected override Type[] GetImageExtenders()
        {
            GetImageExtendersCallCount++;
            return GetImageExtendersResult;
        }
    }

    private class PublicImageEditor : ImageEditor
    {
        public PublicImageEditor()
        {
        }

        protected override string[] GetExtensions() => ["PublicImageEditor"];
    }

    private class PrivateImageEditor : ImageEditor
    {
        private PrivateImageEditor()
        {
        }

        protected override string[] GetExtensions() => ["PrivateImageEditor"];
    }

    private class NullExtensionsImageEditor : ImageEditor
    {
        public NullExtensionsImageEditor()
        {
        }

        protected override string[] GetExtensions() => null;
    }
}
