// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Windows.Forms.ComponentModel.Com2Interop;
using Moq;
using Xunit;
using static Interop;
using static Interop.Ole32;

namespace System.Windows.Forms.Tests.ComponentModel.Com2Interop
{
    // NB: doesn't require thread affinity
    public class COM2PictureConverterTests : IClassFixture<ThreadExceptionFixture>
    {
        private static Com2PictureConverter Instance { get; } = new Com2PictureConverter(new Com2PropertyDescriptor(
            default,
            "Foo",
            default,
            default,
            default,
            default,
            default));

        [Fact]
        public void ConvertNativeToManaged_Null()
        {
            Assert.Null(Instance.ConvertNativeToManaged(null, null));
        }

        [Fact]
        public void ConvertNativeToManaged_NullHandle()
        {
            var mock = new Mock<IPicture>(MockBehavior.Strict);
            mock.Setup(m => m.Handle).Returns(0);
            Assert.Null(Instance.ConvertNativeToManaged(mock.Object, null));
        }

        [Fact]
        public void ConvertNativeToManaged_Icon()
        {
            Icon errorIcon = SystemIcons.Error;

            var mock = new Mock<IPicture>(MockBehavior.Strict);
            mock.Setup(m => m.Handle).Returns((int)errorIcon.Handle);
            mock.Setup(m => m.Type).Returns((short)PICTYPE.ICON);

            using Icon icon = (Icon)Instance.ConvertNativeToManaged(mock.Object, null);

            Assert.Equal(icon.Height, errorIcon.Height);
            Assert.Equal(icon.Width, errorIcon.Width);
            Assert.Equal(typeof(Icon), Instance.ManagedType);

            // We should get the cached object if the handle didn't change
            Assert.Same(icon, (Icon)Instance.ConvertNativeToManaged(mock.Object, null));
        }

        [Fact]
        public void ConvertNativeToManaged_Bitmap()
        {
            Icon errorIcon = SystemIcons.Error;
            using Bitmap errorBitmap = errorIcon.ToBitmap();
            IntPtr hBitmap = errorBitmap.GetHbitmap();
            try
            {
                var mock = new Mock<IPicture>(MockBehavior.Strict);
                mock.Setup(m => m.Handle).Returns((int)hBitmap);
                mock.Setup(m => m.Type).Returns((short)PICTYPE.BITMAP);

                using Bitmap bitmap = (Bitmap)Instance.ConvertNativeToManaged(mock.Object, null);

                Assert.Equal(bitmap.Height, errorIcon.Height);
                Assert.Equal(bitmap.Width, errorIcon.Width);
                Assert.Equal(typeof(Bitmap), Instance.ManagedType);

                // We should get the cached object if the handle didn't change
                Assert.Same(bitmap, (Bitmap)Instance.ConvertNativeToManaged(mock.Object, null));
            }
            finally
            {
                Gdi32.DeleteObject(hBitmap);
            }
        }

        [Fact]
        public void ConvertNativeToManaged_UnsupportedPICTYPE()
        {
            var mock = new Mock<IPicture>(MockBehavior.Strict);
            mock.Setup(m => m.Handle).Returns(1);
            mock.Setup(m => m.Type).Returns((short)PICTYPE.METAFILE);

            // The converter asserts, but doesn't throw. Suppress asserts so
            // that we can validate it returns null as expected.
            using (new NoAssertContext())
            {
                Assert.Null(Instance.ConvertNativeToManaged(mock.Object, null));
            }
        }

        [Fact]
        public void ConvertManagedToNative_NullObject()
        {
            bool cancelSet = true;
            Assert.Null(Instance.ConvertManagedToNative(null, null, ref cancelSet));
            Assert.False(cancelSet);
        }

        [Fact]
        public void ConvertManagedToNative_Icon()
        {
            bool cancelSet = true;
            Icon exclamationIcon = SystemIcons.Exclamation;

            IPicture picture = (IPicture)Instance.ConvertManagedToNative(exclamationIcon, null, ref cancelSet);

            Assert.False(cancelSet);
            Assert.Equal((short)PICTYPE.ICON, picture.Type);
            Assert.Equal(exclamationIcon.Height, GdiHelper.HimetricToPixelY(picture.Height));
            Assert.Equal(exclamationIcon.Width, GdiHelper.HimetricToPixelX(picture.Width));

            // Should get the cached value
            Assert.Same(picture, (IPicture)Instance.ConvertManagedToNative(exclamationIcon, null, ref cancelSet));

            // And we should also round trip to the same value
            Assert.Same(exclamationIcon, Instance.ConvertNativeToManaged(picture, null));
        }

        [Fact]
        public void ConvertManagedToNative_Bitmap()
        {
            bool cancelSet = true;
            using Bitmap bitmap = new Bitmap(42, 70);

            IPicture picture = (IPicture)Instance.ConvertManagedToNative(bitmap, null, ref cancelSet);

            Assert.False(cancelSet);
            Assert.Equal((short)PICTYPE.BITMAP, picture.Type);
            Assert.Equal(bitmap.Height, GdiHelper.HimetricToPixelY(picture.Height));
            Assert.Equal(bitmap.Width, GdiHelper.HimetricToPixelX(picture.Width));

            // Should get the cached value
            Assert.Same(picture, (IPicture)Instance.ConvertManagedToNative(bitmap, null, ref cancelSet));

            // And we should also round trip to the same value
            Assert.Same(bitmap, Instance.ConvertNativeToManaged(picture, null));
        }

        [Fact]
        public void ConvertManagedToNative_UnknownObjectType()
        {
            // The converter asserts, but doesn't throw. Suppress asserts so
            // that we can validate it returns null as expected.
            using (new NoAssertContext())
            {
                bool cancelSet = true;
                Assert.Null(Instance.ConvertManagedToNative(new object(), null, ref cancelSet));
                Assert.False(cancelSet);
            }
        }
    }
}
