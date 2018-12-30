// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Drawing;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class ToolStripItemImageRenderEventArgsTests
    {
        public static IEnumerable<object[]> Ctor_Graphics_ToolStripItem_Rectangle_TestData()
        {
            var image = new Bitmap(10, 10);
            Graphics graphics = Graphics.FromImage(image);

            yield return new object[] { null, new ToolStripButton(), Rectangle.Empty };
            yield return new object[] { graphics, new ToolStripButton(), new Rectangle(1, 2, 3, 4) };
            yield return new object[] { graphics, new ToolStripButton() { Image = image }, new Rectangle(1, 2, 3, 4) };
            yield return new object[]
            {
                graphics, new ToolStripButton
                {
                    RightToLeft = RightToLeft.Yes,
                    Image = image
                }, new Rectangle(1, 2, 3, 4)
            };
                yield return new object[]
            {
                graphics, new ToolStripButton
                {
                    RightToLeftAutoMirrorImage = true,
                    Image = image
                }, new Rectangle(1, 2, 3, 4)
            };
            yield return new object[]
            {
                graphics, new ToolStripButton
                {
                    RightToLeftAutoMirrorImage = true,
                    RightToLeft = RightToLeft.Yes,
                    Image = image
                }, new Rectangle(1, 2, 3, 4)
            };
        }

        [Theory]
        [MemberData(nameof(Ctor_Graphics_ToolStripItem_Rectangle_TestData))]
        public void Ctor_Graphics_ToolStripItem_Rectangle(Graphics g, ToolStripItem item, Rectangle imageRectangle)
        {
            var e = new ToolStripItemImageRenderEventArgs(g, item, imageRectangle);
            Assert.Equal(g, e.Graphics);
            Assert.Equal(item, e.Item);
            Assert.Equal(imageRectangle, e.ImageRectangle);
            if (item.Image == null)
            {
                Assert.Null(e.Image);
            }
            else
            {
                Assert.NotNull(e.Image);
            }
        }

        public static IEnumerable<object[]> Ctor_Graphics_ToolStripItem_Image_Rectangle_TestData()
        {
            var image = new Bitmap(10, 10);
            var otherImage = new Bitmap(10, 10);
            Graphics graphics = Graphics.FromImage(image);

            yield return new object[] { null, null, null, Rectangle.Empty };
            yield return new object[] { graphics, new ToolStripButton(), otherImage, new Rectangle(1, 2, 3, 4) };
            yield return new object[] { graphics, new ToolStripButton() { Image = image }, otherImage, new Rectangle(1, 2, 3, 4) };

        }

        [Theory]
        [MemberData(nameof(Ctor_Graphics_ToolStripItem_Image_Rectangle_TestData))]
        public void Ctor_Graphics_ToolStripItem_Image_Rectangle(Graphics g, ToolStripItem item, Image image, Rectangle imageRectangle)
        {
            var e = new ToolStripItemImageRenderEventArgs(g, item, image, imageRectangle);
            Assert.Equal(g, e.Graphics);
            Assert.Equal(item, e.Item);
            Assert.Equal(image, e.Image);
            Assert.Equal(imageRectangle, e.ImageRectangle);
        }

        [Fact]
        public void Ctor_NullItem_ThrowsNullReferenceException()
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                Assert.Throws<NullReferenceException>(() => new ToolStripItemImageRenderEventArgs(graphics, null, new Rectangle(1, 2, 3, 4)));
            }
        }
    }
}
