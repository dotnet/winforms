// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using Xunit;
using System.Windows.Forms.Primitives.Tests.Interop.Mocks;
using static Interop.Ole32;
using static Interop.User32;

namespace System.Windows.Forms.Primitives.Tests.Interop.Ole32
{
    [Collection("Sequential")]
    public class IPictureTests
    {
        [StaFact]
        public void GetIPictureFromCursor()
        {
            using MockCursor arrow = new MockCursor(CursorResourceId.IDC_ARROW);

            IPicture picture = MockAxHost.GetIPictureFromCursor(arrow.Handle);
            Assert.NotNull(picture);
            Assert.Equal(PICTYPE.ICON, (PICTYPE)picture.Type);

            Assert.Equal(arrow.Size.Height, GdiHelper.HimetricToPixelY(picture.Height));
            Assert.Equal(arrow.Size.Width, GdiHelper.HimetricToPixelX(picture.Width));
        }

        [StaFact]
        public void GetIPictureFromImage()
        {
            using MockCursor arrow = new MockCursor(CursorResourceId.IDC_ARROW);
            using Icon icon = Icon.FromHandle(arrow.Handle);
            using Bitmap bitmap = icon.ToBitmap();
            IPicture picture = MockAxHost.GetIPictureFromPicture(bitmap);
            Assert.NotNull(picture);
            Assert.Equal(PICTYPE.BITMAP, (PICTYPE)picture.Type);

            Assert.Equal(bitmap.Size.Height, GdiHelper.HimetricToPixelY(picture.Height));
            Assert.Equal(bitmap.Size.Width, GdiHelper.HimetricToPixelX(picture.Width));
        }

        [StaFact]
        public void GetIPictureDispFromImage()
        {
            using Icon icon = SystemIcons.Question;
            using Bitmap bitmap = icon.ToBitmap();
            IPictureDisp picture = MockAxHost.GetIPictureDispFromPicture(bitmap);
            Assert.NotNull(picture);
            Assert.Equal(PICTYPE.BITMAP, (PICTYPE)picture.Type);

            Assert.Equal(bitmap.Size.Height, GdiHelper.HimetricToPixelY(picture.Height));
            Assert.Equal(bitmap.Size.Width, GdiHelper.HimetricToPixelX(picture.Width));
        }

        [StaFact]
        public void GetPictureFromIPicture()
        {
            using Icon icon = SystemIcons.Exclamation;
            using Bitmap bitmap = icon.ToBitmap();
            IPicture picture = MockAxHost.GetIPictureFromPicture(bitmap);
            Assert.NotNull(picture);
            using Image image = MockAxHost.GetPictureFromIPicture(picture)!;
            Assert.NotNull(image);
            Assert.Equal(bitmap.Size, image.Size);
        }

        [StaFact]
        public void GetPictureFromIPictureDisp()
        {
            using Bitmap bitmap = new Bitmap(100, 200);
            IPictureDisp picture = MockAxHost.GetIPictureDispFromPicture(bitmap);
            Assert.NotNull(picture);
            using Image image = MockAxHost.GetPictureFromIPictureDisp(picture)!;
            Assert.NotNull(image);
            Assert.Equal(bitmap.Size, image.Size);
        }
    }
}
