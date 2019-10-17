// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Imaging;
using Xunit;

namespace System.Windows.Forms.Design.Editors.Tests
{
    public class EnsureEditorsTests
    {
        [Theory]
        [InlineData(typeof(Array), typeof(ArrayEditor))]
        [InlineData(typeof(IList), typeof(CollectionEditor))]
        [InlineData(typeof(ICollection), typeof(CollectionEditor))]
        [InlineData(typeof(byte[]), typeof(ArrayEditor))]
        [InlineData(typeof(string[]), typeof(StringArrayEditor))]
        [InlineData(typeof(Bitmap), typeof(BitmapEditor))]
        [InlineData(typeof(Color), typeof(ColorEditor))]
        [InlineData(typeof(Font), typeof(FontEditor))]
        [InlineData(typeof(Image), typeof(ImageEditor))]
        [InlineData(typeof(Metafile), typeof(MetafileEditor))]

        [InlineData(typeof(AnchorStyles), typeof(AnchorEditor))]
        [InlineData(typeof(ToolStripStatusLabelBorderSides), typeof(BorderSidesEditor))]
        [InlineData(typeof(DockStyle), typeof(DockEditor))]
        [InlineData(typeof(ImageList.ImageCollection), typeof(ImageCollectionEditor))]
        [InlineData(typeof(ImageListImage), typeof(ImageListImageEditor))]
        [InlineData(typeof(Keys), typeof(ShortcutKeysEditor))]
        public void EnsureUITypeEditorForType(Type type, Type expectedEditorType)
        {
            var editor = TypeDescriptor.GetEditor(type, typeof(UITypeEditor));
            Assert.NotNull(editor);

            Assert.Equal(expectedEditorType, editor.GetType());
        }

        [Theory]
        [InlineData(typeof(ButtonBase), "Text", typeof(MultilineStringEditor))]
        public void EnsureUITypeEditorForProperty(Type type, string propertyName, Type expectedEditorType)
        {
            var properties = TypeDescriptor.GetProperties(type);
            Assert.NotNull(properties);
            Assert.NotEmpty(properties);

            var propertyDescriptor = properties.Find(propertyName, true);
            Assert.NotNull(propertyDescriptor);

            var editor = propertyDescriptor.GetEditor(typeof(UITypeEditor));
            Assert.NotNull(editor);

            Assert.Equal(expectedEditorType, editor.GetType());
        }
    }
}
