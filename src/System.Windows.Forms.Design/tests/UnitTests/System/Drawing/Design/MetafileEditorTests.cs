// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms.TestUtilities;

namespace System.Drawing.Design.Tests;

public class MetafileEditorTests
{
    [Fact]
    public void MetafileEditor_Ctor_Default()
    {
        MetafileEditor editor = new();
        Assert.False(editor.IsDropDownResizable);
    }

    [Theory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetITypeDescriptorContextTestData))]
    public void MetafileEditor_GetEditStyle_Invoke_ReturnsModal(ITypeDescriptorContext context)
    {
        MetafileEditor editor = new();
        Assert.Equal(UITypeEditorEditStyle.Modal, editor.GetEditStyle(context));
    }

    [Fact]
    public void MetafileEditor_GetExtensions_InvokeDefault_ReturnsExpected()
    {
        SubMetafileEditor editor = new();
        string[] extensions = editor.GetExtensions();
        Assert.Equal(new string[] { "emf", "wmf" }, extensions);
        Assert.NotSame(extensions, editor.GetExtensions());
    }

    [Fact]
    public void MetafileEditor_GetFileDialogDescription_Invoke_ReturnsExpected()
    {
        SubMetafileEditor editor = new();
        Assert.Equal("Metafiles", editor.GetFileDialogDescription());
    }

    [Theory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetITypeDescriptorContextTestData))]
    public void MetafileEditor_GetPaintValueSupported_Invoke_ReturnsTrue(ITypeDescriptorContext context)
    {
        MetafileEditor editor = new();
        Assert.True(editor.GetPaintValueSupported(context));
    }

    [Fact]
    public void MetafileEditor_LoadFromStream_BitmapStream_ThrowsExternalException()
    {
        SubMetafileEditor editor = new();
        using MemoryStream stream = new();
        using Bitmap image = new(10, 10);
        image.Save(stream, ImageFormat.Bmp);
        stream.Position = 0;
        Assert.Throws<ExternalException>(() => editor.LoadFromStream(stream));
    }

    [Fact]
    public void MetafileEditor_LoadFromStream_MetafileStream_ReturnsExpected()
    {
        SubMetafileEditor editor = new();
        using Stream stream = File.OpenRead("Resources/telescope_01.wmf");
        Metafile result = Assert.IsType<Metafile>(editor.LoadFromStream(stream));
        Assert.Equal(new Size(3096, 4127), result.Size);
    }

    [Fact]
    public void MetafileEditor_LoadFromStream_NullStream_ThrowsArgumentNullException()
    {
        SubMetafileEditor editor = new();
        Assert.Throws<ArgumentNullException>("stream", () => editor.LoadFromStream(null));
    }

    private class SubMetafileEditor : MetafileEditor
    {
        public new string[] GetExtensions() => base.GetExtensions();

        public new string GetFileDialogDescription() => base.GetFileDialogDescription();

        public new Image LoadFromStream(Stream stream) => base.LoadFromStream(stream);
    }

    private class CustomGetImageExtendersEditor : MetafileEditor
    {
        public new string[] GetExtensions() => base.GetExtensions();

        protected override Type[] GetImageExtenders() => [typeof(CustomGetExtensionsEditor)];
    }

    private class CustomGetExtensionsEditor : ImageEditor
    {
        protected override string[] GetExtensions() => ["CustomGetExtensionsEditor"];
    }
}
