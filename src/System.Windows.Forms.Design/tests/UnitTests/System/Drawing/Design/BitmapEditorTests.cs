// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing.Imaging;
using System.Windows.Forms.TestUtilities;

namespace System.Drawing.Design.Tests;

public class BitmapEditorTests
{
    [Fact]
    public void BitmapEditor_Ctor_Default()
    {
        BitmapEditor editor = new();
        Assert.False(editor.IsDropDownResizable);
    }

    [Fact]
    public void BitmapEditor_BitmapExtensions_Get_ReturnsExpected()
    {
        List<string> extensions = SubBitmapEditor.BitmapExtensions;
        Assert.Equal(new string[] { "bmp", "gif", "jpg", "jpeg", "png", "ico" }, extensions);
        Assert.Same(extensions, SubBitmapEditor.BitmapExtensions);
    }

    [Theory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetITypeDescriptorContextTestData))]
    public void BitmapEditor_GetEditStyle_Invoke_ReturnsModal(ITypeDescriptorContext context)
    {
        BitmapEditor editor = new();
        Assert.Equal(UITypeEditorEditStyle.Modal, editor.GetEditStyle(context));
    }

    [Fact]
    public void BitmapEditor_GetExtensions_InvokeDefault_ReturnsExpected()
    {
        SubBitmapEditor editor = new();
        string[] extensions = editor.GetExtensions();
        Assert.Equal(new string[] { "bmp", "gif", "jpg", "jpeg", "png", "ico" }, extensions);
        Assert.NotSame(extensions, editor.GetExtensions());
    }

    [Fact]
    public void BitmapEditor_GetExtensions_InvokeCustomExtenders_ReturnsExpected()
    {
        CustomGetImageExtendersEditor editor = new();
        string[] extensions = editor.GetExtensions();
        Assert.Equal(new string[] { "bmp", "gif", "jpg", "jpeg", "png", "ico" }, extensions);
        Assert.NotSame(extensions, editor.GetExtensions());
    }

    [Fact]
    public void BitmapEditor_GetFileDialogDescription_Invoke_ReturnsExpected()
    {
        SubBitmapEditor editor = new();
        Assert.Equal("Bitmap files", editor.GetFileDialogDescription());
    }

    [Theory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetITypeDescriptorContextTestData))]
    public void BitmapEditor_GetPaintValueSupported_Invoke_ReturnsTrue(ITypeDescriptorContext context)
    {
        BitmapEditor editor = new();
        Assert.True(editor.GetPaintValueSupported(context));
    }

    [Fact]
    public void BitmapEditor_LoadFromStream_BitmapStream_ReturnsExpected()
    {
        SubBitmapEditor editor = new();
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
    public void BitmapEditor_LoadFromStream_MetafileStream_ReturnsExpected()
    {
        SubBitmapEditor editor = new();
        using Stream stream = File.OpenRead("Resources/telescope_01.wmf");
        Bitmap result = Assert.IsType<Bitmap>(editor.LoadFromStream(stream));
        Assert.Equal(new Size(490, 654), result.Size);
    }

    [Fact]
    public void BitmapEditor_LoadFromStream_NullStream_ThrowsArgumentNullException()
    {
        SubBitmapEditor editor = new();
        Assert.Throws<ArgumentNullException>("stream", () => editor.LoadFromStream(null));
    }

    private class SubBitmapEditor : BitmapEditor
    {
#pragma warning disable IDE1006 // Naming Styles
        public static new List<string> BitmapExtensions = BitmapEditor.BitmapExtensions;
#pragma warning restore IDE1006

        public new string[] GetExtensions() => base.GetExtensions();

        public new string GetFileDialogDescription() => base.GetFileDialogDescription();

        public new Image LoadFromStream(Stream stream) => base.LoadFromStream(stream);
    }

    private class CustomGetImageExtendersEditor : BitmapEditor
    {
        public new string[] GetExtensions() => base.GetExtensions();

        protected override Type[] GetImageExtenders() => [typeof(CustomGetExtensionsEditor)];
    }

    private class CustomGetExtensionsEditor : ImageEditor
    {
        protected override string[] GetExtensions() => ["CustomGetExtensionsEditor"];
    }
}
