// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms.Internal;
using Xunit;
using static Interop;
using static Interop.Ole32;

namespace System.Windows.Forms.Tests.InteropTests
{
    public class IPictureTests
    {
        [Fact]
        public void GetIPictureFromCursor()
        {
            Cursor arrow = Cursors.Arrow;
            IPicture picture = AxHostAccess.GetIPictureFromCursor(arrow);
            Assert.NotNull(picture);
            Assert.Equal(PICTYPE.ICON, (PICTYPE)picture.Type);

            Assert.Equal(arrow.Size.Height, HimetricToPixelY(picture.Height));
            Assert.Equal(arrow.Size.Width, HimetricToPixelX(picture.Width));
        }

        [Fact]
        public void GetIPictureFromImage()
        {
            using Icon icon = Icon.FromHandle(Cursors.Arrow.Handle);
            using Bitmap bitmap = icon.ToBitmap();
            IPicture picture = AxHostAccess.GetIPictureFromPicture(bitmap);
            Assert.NotNull(picture);
            Assert.Equal(PICTYPE.BITMAP, (PICTYPE)picture.Type);

            Assert.Equal(bitmap.Size.Height, HimetricToPixelY(picture.Height));
            Assert.Equal(bitmap.Size.Width, HimetricToPixelX(picture.Width));
        }

        [Fact]
        public void GetIPictureDispFromImage()
        {
            using Icon icon = SystemIcons.Question;
            using Bitmap bitmap = icon.ToBitmap();
            IPictureDisp picture = AxHostAccess.GetIPictureDispFromPicture(bitmap);
            Assert.NotNull(picture);
            Assert.Equal(PICTYPE.BITMAP, (PICTYPE)picture.Type);

            Assert.Equal(bitmap.Size.Height, HimetricToPixelY(picture.Height));
            Assert.Equal(bitmap.Size.Width, HimetricToPixelX(picture.Width));
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

        static IPictureTests()
        {
            using ScreenDC dc = ScreenDC.Create();
            s_logPixelsX = Gdi32.GetDeviceCaps(dc, Gdi32.DeviceCapability.LOGPIXELSX);
            s_logPixelsY = Gdi32.GetDeviceCaps(dc, Gdi32.DeviceCapability.LOGPIXELSY);
        }

        private static long s_logPixelsX;
        private static long s_logPixelsY;
        // private const int TwipsPerInch = 1440;
        private const int HiMetricPerInch = 2540;

        private static int HimetricToPixelX(int himetric)
            => (int)((s_logPixelsX * himetric) / HiMetricPerInch);
        private static int HimetricToPixelY(int himetric)
            => (int)((s_logPixelsY * himetric) / HiMetricPerInch);


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
