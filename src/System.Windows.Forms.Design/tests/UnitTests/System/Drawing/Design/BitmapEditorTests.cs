// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Imaging;
using System.IO;
using Moq;
using WinForms.Common.Tests;
using Xunit;

namespace System.Drawing.Design.Tests
{
    public class BitmapEditorTests : IClassFixture<ThreadExceptionFixture>
    {
        [Fact]
        public void BitmapEditor_Ctor_Default()
        {
            var editor = new BitmapEditor();
            Assert.False(editor.IsDropDownResizable);
        }

        [Fact]
        public void BitmapEditor_BitmapExtensions_Get_ReturnsExpected()
        {
            var editor = new SubBitmapEditor();
            List<string> extensions = SubBitmapEditor.BitmapExtensions;
            Assert.Equal(new string[] { "bmp", "gif", "jpg", "jpeg", "png", "ico" }, extensions);
            Assert.Same(extensions, SubBitmapEditor.BitmapExtensions);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetITypeDescriptorContextTestData))]
        public void BitmapEditor_GetEditStyle_Invoke_ReturnsModal(ITypeDescriptorContext context)
        {
            var editor = new BitmapEditor();
            Assert.Equal(UITypeEditorEditStyle.Modal, editor.GetEditStyle(context));
        }

        [Fact]
        public void BitmapEditor_GetExtensions_InvokeDefault_ReturnsExpected()
        {
            var editor = new SubBitmapEditor();
            string[] extensions = editor.GetExtensions();
            Assert.Equal(new string[] { "bmp", "gif", "jpg", "jpeg", "png", "ico" }, extensions);
            Assert.NotSame(extensions, editor.GetExtensions());
        }

        [Fact]
        public void BitmapEditor_GetExtensions_InvokeCustomExtenders_ReturnsExpected()
        {
            var editor = new CustomGetImageExtendersEditor();
            string[] extensions = editor.GetExtensions();
            Assert.Equal(new string[] { "bmp", "gif", "jpg", "jpeg", "png", "ico" }, extensions);
            Assert.NotSame(extensions, editor.GetExtensions());
        }

        [Fact]
        public void BitmapEditor_GetFileDialogDescription_Invoke_ReturnsExpected()
        {
            var editor = new SubBitmapEditor();
            Assert.Equal("Bitmap files", editor.GetFileDialogDescription());
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetITypeDescriptorContextTestData))]
        public void BitmapEditor_GetPaintValueSupported_Invoke_ReturnsTrue(ITypeDescriptorContext context)
        {
            var editor = new BitmapEditor();
            Assert.True(editor.GetPaintValueSupported(context));
        }

        [Fact]
        public void BitmapEditor_LoadFromStream_BitmapStream_ReturnsExpected()
        {
            var editor = new SubBitmapEditor();
            using (MemoryStream stream = new MemoryStream())
            using (var image = new Bitmap(10, 10))
            {
                image.Save(stream, ImageFormat.Bmp);
                stream.Position = 0;
                Bitmap result = Assert.IsType<Bitmap>(editor.LoadFromStream(stream));
                Assert.Equal(new Size(10, 10), result.Size);

                using var resultStream = new MemoryStream();
                result.Save(resultStream, ImageFormat.Bmp);
                Assert.Equal(stream.Length, resultStream.Length);
            }
        }

        [Fact]
        public void BitmapEditor_LoadFromStream_MetafileStream_ReturnsExpected()
        {
            var editor = new SubBitmapEditor();
            using (Stream stream = File.OpenRead("Resources/telescope_01.wmf"))
            {
                Bitmap result = Assert.IsType<Bitmap>(editor.LoadFromStream(stream));
                Assert.Equal(new Size(490, 654), result.Size);
            }
        }

        [Fact]
        public void BitmapEditor_LoadFromStream_NullStream_ThrowsArgumentNullException()
        {
            var editor = new SubBitmapEditor();
            Assert.Throws<ArgumentNullException>("stream", () => editor.LoadFromStream(null));
        }

        private class SubBitmapEditor : BitmapEditor
        {
            public static new List<string> BitmapExtensions = BitmapEditor.BitmapExtensions;

            public new string[] GetExtensions() => base.GetExtensions();

            public new string GetFileDialogDescription() => base.GetFileDialogDescription();

            public new Image LoadFromStream(Stream stream) => base.LoadFromStream(stream);
        }

        private class CustomGetImageExtendersEditor : BitmapEditor
        {
            public new string[] GetExtensions() => base.GetExtensions();

            protected override Type[] GetImageExtenders() => new Type[] { typeof(CustomGetExtensionsEditor) };
        }

        private class CustomGetExtensionsEditor : ImageEditor
        {
            protected override string[] GetExtensions() => new string[] { "CustomGetExtensionsEditor" };
        }
    }
}