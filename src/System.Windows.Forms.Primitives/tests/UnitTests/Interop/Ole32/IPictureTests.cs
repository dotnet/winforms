﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Windows.Forms.Primitives.Tests.Interop.Mocks;
using Windows.Win32.System.Com;
using Windows.Win32.System.Ole;
using Xunit;

namespace System.Windows.Forms.Primitives.Tests.Interop.Ole32
{
    [Collection("Sequential")]
    public class IPictureTests
    {
        [StaFact]
        public void GetIPictureFromCursor()
        {
            using MockCursor arrow = new MockCursor(PInvoke.IDC_ARROW);

            IPicture.Interface picture = MockAxHost.GetIPictureFromCursor(arrow.Handle);
            Assert.NotNull(picture);
            picture.get_Type(out short type).ThrowOnFailure();
            Assert.Equal((short)PICTYPE.PICTYPE_ICON, type);

            picture.get_Height(out int height).ThrowOnFailure();
            Assert.Equal(arrow.Size.Height, GdiHelper.HimetricToPixelY(height));
            picture.get_Width(out int width).ThrowOnFailure();
            Assert.Equal(arrow.Size.Width, GdiHelper.HimetricToPixelX(width));
        }

        [StaFact]
        public unsafe void GetIPictureFromImage()
        {
            using MockCursor arrow = new MockCursor(PInvoke.IDC_ARROW);
            using Icon icon = Icon.FromHandle(arrow.Handle);
            using Bitmap bitmap = icon.ToBitmap();
            IPicture.Interface picture = MockAxHost.GetIPictureFromPicture(bitmap);
            Assert.NotNull(picture);
            picture.get_Type(out short type).ThrowOnFailure();
            Assert.Equal((short)PICTYPE.PICTYPE_BITMAP, type);

            picture.get_Height(out int height).ThrowOnFailure();
            Assert.Equal(bitmap.Size.Height, GdiHelper.HimetricToPixelY(height));
            picture.get_Width(out int width).ThrowOnFailure();
            Assert.Equal(bitmap.Size.Width, GdiHelper.HimetricToPixelX(width));
        }

        [StaFact]
        public unsafe void GetIPictureDispFromImage()
        {
            using Icon icon = SystemIcons.Question;
            using Bitmap bitmap = icon.ToBitmap();
            using var picture = ComHelpers.GetComScope<IDispatch>(
                MockAxHost.GetIPictureDispFromPicture(bitmap),
                out bool result);
            Assert.True(result);
            using VARIANT variant = new();
            ComHelpers.GetDispatchProperty(picture, PInvoke.DISPID_PICT_TYPE, &variant).ThrowOnFailure();
            Assert.Equal(PICTYPE.PICTYPE_BITMAP, (PICTYPE)variant.data.iVal);

            ComHelpers.GetDispatchProperty(picture, PInvoke.DISPID_PICT_HEIGHT, &variant).ThrowOnFailure();
            Assert.Equal(bitmap.Size.Height, GdiHelper.HimetricToPixelY((int)variant.data.uintVal));

            ComHelpers.GetDispatchProperty(picture, PInvoke.DISPID_PICT_WIDTH, &variant).ThrowOnFailure();
            Assert.Equal(bitmap.Size.Width, GdiHelper.HimetricToPixelX((int)variant.data.uintVal));
        }

        [StaFact]
        public void GetPictureFromIPicture()
        {
            using Icon icon = SystemIcons.Exclamation;
            using Bitmap bitmap = icon.ToBitmap();
            IPicture.Interface picture = MockAxHost.GetIPictureFromPicture(bitmap);
            Assert.NotNull(picture);
            using Image image = MockAxHost.GetPictureFromIPicture(picture)!;
            Assert.NotNull(image);
            Assert.Equal(bitmap.Size, image.Size);
        }

        [StaFact]
        public void GetPictureFromIPictureDisp()
        {
            using Bitmap bitmap = new Bitmap(100, 200);
            IPictureDisp.Interface picture = MockAxHost.GetIPictureDispFromPicture(bitmap);
            Assert.NotNull(picture);
            using Image image = MockAxHost.GetPictureFromIPictureDisp(picture)!;
            Assert.NotNull(image);
            Assert.Equal(bitmap.Size, image.Size);
        }
    }
}
