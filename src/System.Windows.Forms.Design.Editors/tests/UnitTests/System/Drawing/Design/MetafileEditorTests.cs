// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using Moq;
using Xunit;

namespace System.Drawing.Design.Tests
{
    public class MetafileEditorTests
    {
        [Fact]
        public void MetafileEditor_GetExtensions_InvokeDefault_ReturnsExpected()
        {
            var editor = new SubMetafileEditor();
            string[] extensions = editor.GetExtensions();
            Assert.Equal(new string[] { "emf", "wmf" }, extensions);
            Assert.NotSame(extensions, editor.GetExtensions());
        }

        [Fact]
        public void MetafileEditor_GetFileDialogDescription_Invoke_ReturnsExpected()
        {
            var editor = new SubMetafileEditor();
            Assert.Equal("Metafiles", editor.GetFileDialogDescription());
        }

        [Fact]
        public void MetafileEditor_LoadFromStream_BitmapStream_ThrowsExternalException()
        {
            var editor = new SubMetafileEditor();
            using (MemoryStream stream = new MemoryStream())
            using (var image = new Bitmap(10, 10))
            {
                image.Save(stream, ImageFormat.Bmp);
                stream.Position = 0;
                Assert.Throws<ExternalException>(() => editor.LoadFromStream(stream));
            }
        }

        [Fact]
        public void MetafileEditor_LoadFromStream_MetafileStream_ReturnsExpected()
        {
            var editor = new SubMetafileEditor();
            using (Stream stream = File.OpenRead("Resources/telescope_01.wmf"))
            {
                Metafile result = Assert.IsType<Metafile>(editor.LoadFromStream(stream));
                Assert.Equal(new Size(3096, 4127), result.Size);
            }
        }

        [Fact]
        public void MetafileEditor_LoadFromStream_NullStream_ThrowsArgumentNullException()
        {
            var editor = new SubMetafileEditor();
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

            protected override Type[] GetImageExtenders() => new Type[] { typeof(CustomGetExtensionsEditor) };
        }

        private class CustomGetExtensionsEditor : ImageEditor
        {
            protected override string[] GetExtensions() => new string[] { "CustomGetExtensionsEditor" };
        }
    }
}