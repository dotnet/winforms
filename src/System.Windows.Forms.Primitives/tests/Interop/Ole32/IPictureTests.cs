// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using Xunit;
using static Interop.Ole32;

namespace System.Windows.Forms.Primitives.Tests.Interop.Ole32
{
    public class IPictureTests : IClassFixture<ThreadExceptionFixture>
    {
        [Fact]
        public void GetIPictureFromCursor()
        {
            Cursor arrow = Cursors.Arrow;
            IPicture picture = AxHostAccess.GetIPictureFromCursor(arrow);
            Assert.NotNull(picture);
            Assert.Equal(PICTYPE.ICON, (PICTYPE)picture.Type);

            Assert.Equal(arrow.Size.Height, GdiHelper.HimetricToPixelY(picture.Height));
            Assert.Equal(arrow.Size.Width, GdiHelper.HimetricToPixelX(picture.Width));
        }

        [Fact]
        public void GetIPictureFromImage()
        {
            using Icon icon = Icon.FromHandle(Cursors.Arrow.Handle);
            using Bitmap bitmap = icon.ToBitmap();
            IPicture picture = AxHostAccess.GetIPictureFromPicture(bitmap);
            Assert.NotNull(picture);
            Assert.Equal(PICTYPE.BITMAP, (PICTYPE)picture.Type);

            Assert.Equal(bitmap.Size.Height, GdiHelper.HimetricToPixelY(picture.Height));
            Assert.Equal(bitmap.Size.Width, GdiHelper.HimetricToPixelX(picture.Width));
        }

        [Fact]
        public void GetIPictureDispFromImage()
        {
            using Icon icon = SystemIcons.Question;
            using Bitmap bitmap = icon.ToBitmap();
            IPictureDisp picture = AxHostAccess.GetIPictureDispFromPicture(bitmap);
            Assert.NotNull(picture);
            Assert.Equal(PICTYPE.BITMAP, (PICTYPE)picture.Type);

            Assert.Equal(bitmap.Size.Height, GdiHelper.HimetricToPixelY(picture.Height));
            Assert.Equal(bitmap.Size.Width, GdiHelper.HimetricToPixelX(picture.Width));
        }

        [Fact]
        public void GetPictureFromIPicture()
        {
            using Icon icon = SystemIcons.Exclamation;
            using Bitmap bitmap = icon.ToBitmap();
            IPicture picture = AxHostAccess.GetIPictureFromPicture(bitmap);
            Assert.NotNull(picture);
            using Image image = AxHostAccess.GetPictureFromIPicture(picture);
            Assert.NotNull(image);
            Assert.Equal(bitmap.Size, image.Size);
        }

        [Fact]
        public void GetPictureFromIPictureDisp()
        {
            using Bitmap bitmap = new Bitmap(100, 200);
            IPictureDisp picture = AxHostAccess.GetIPictureDispFromPicture(bitmap);
            Assert.NotNull(picture);
            using Image image = AxHostAccess.GetPictureFromIPictureDisp(picture);
            Assert.NotNull(image);
            Assert.Equal(bitmap.Size, image.Size);
        }

        internal class AxHostAccess : AxHost
        {
            private AxHostAccess() : base(string.Empty) { }

            internal new static IPicture GetIPictureFromCursor(Cursor cursor)
                => (IPicture)AxHost.GetIPictureFromCursor(cursor);

            internal new static IPicture GetIPictureFromPicture(Image image)
                => (IPicture)AxHost.GetIPictureFromPicture(image);

            internal new static IPictureDisp GetIPictureDispFromPicture(Image image)
                => (IPictureDisp)AxHost.GetIPictureDispFromPicture(image);

            internal static Image GetPictureFromIPicture(IPicture picture)
                => GetPictureFromIPicture((object)picture);

            internal static Image GetPictureFromIPictureDisp(IPictureDisp picture)
                => GetPictureFromIPictureDisp((object)picture);
        }
    }
}
