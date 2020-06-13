// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Xunit;

namespace System.Windows.Forms.Design.Tests
{
    public class ImageListImageEditorTests : IClassFixture<ThreadExceptionFixture>
    {
        [Fact]
        public void ImageListImageEditor_LoadImageFromStream_BitmapStream_ReturnsExpected()
        {
            var editor = new ImageListImageEditor();
            var editor_LoadImageFromStream = editor.TestAccessor().CreateDelegate<Func<Stream, bool, ImageListImage>>("LoadImageFromStream");

            using var stream = new MemoryStream();
            using var image = new Bitmap(10, 10);
            image.Save(stream, ImageFormat.Bmp);
            stream.Position = 0;

            var result = Assert.IsType<ImageListImage>(editor_LoadImageFromStream(stream, false));
            var resultImage = Assert.IsType<Bitmap>(result.Image);
            Assert.Equal(new Size(10, 10), result.Size);
            Assert.Equal(new Size(10, 10), resultImage.Size);

            using var resultStream = new MemoryStream();
            result.Image.Save(resultStream, ImageFormat.Bmp);
            Assert.Equal(stream.Length, resultStream.Length);
        }
    }
}
