// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Windows.Forms.ComponentModel.Com2Interop;
using Windows.Win32.System.Com;
using Windows.Win32.System.Ole;
using Xunit;
using static Interop;

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

        private unsafe class TestIPicture : IPictureMock
        {
            private nint _handle;
            private PICTYPE _type;

            public TestIPicture(nint handle, PICTYPE type = PICTYPE.PICTYPE_NONE)
            {
                _handle = handle;
                _type = type;
            }

            public override HRESULT get_Handle(uint* pHandle)
            {
                *pHandle = PARAM.ToUInt(_handle);
                return HRESULT.S_OK;
            }

            public override HRESULT get_Type(short* pType)
            {
                *pType = (short)_type;
                return HRESULT.S_OK;
            }
        }

        [Fact]
        public void ConvertNativeToManaged_NullHandle()
        {
            TestIPicture nullIPicture = new(0);
            Assert.Null(Instance.ConvertNativeToManaged(nullIPicture, null));
        }

        [Fact]
        public unsafe void ConvertNativeToManaged_Icon()
        {
            Icon errorIcon = SystemIcons.Error;
            nint handle = errorIcon.Handle;
            TestIPicture iconIPicture = new(handle, PICTYPE.PICTYPE_ICON);

            using Icon icon = (Icon)Instance.ConvertNativeToManaged(iconIPicture, null);

            Assert.Equal(icon.Height, errorIcon.Height);
            Assert.Equal(icon.Width, errorIcon.Width);
            Assert.Equal(typeof(Icon), Instance.ManagedType);

            // We should get the cached object if the handle didn't change
            Assert.Same(icon, (Icon)Instance.ConvertNativeToManaged(iconIPicture, null));
        }

        [Fact]
        public void ConvertNativeToManaged_Bitmap()
        {
            Icon errorIcon = SystemIcons.Error;
            using Bitmap errorBitmap = errorIcon.ToBitmap();
            nint hBitmap = errorBitmap.GetHbitmap();
            TestIPicture bitmapIPicture = new(hBitmap, PICTYPE.PICTYPE_BITMAP);
            try
            {
                using Bitmap bitmap = (Bitmap)Instance.ConvertNativeToManaged(bitmapIPicture, pd: null);

                Assert.Equal(bitmap.Height, errorIcon.Height);
                Assert.Equal(bitmap.Width, errorIcon.Width);
                Assert.Equal(typeof(Bitmap), Instance.ManagedType);

                // We should get the cached object if the handle didn't change
                Assert.Same(bitmap, (Bitmap)Instance.ConvertNativeToManaged(bitmapIPicture, pd: null));
            }
            finally
            {
                PInvoke.DeleteObject((HGDIOBJ)hBitmap);
            }
        }

        [Fact]
        public void ConvertNativeToManaged_UnsupportedPICTYPE()
        {
            // The converter asserts, but doesn't throw. Suppress asserts so
            // that we can validate it returns null as expected.
            using (new NoAssertContext())
            {
                Assert.Null(Instance.ConvertNativeToManaged(new TestIPicture(1, PICTYPE.PICTYPE_METAFILE), null));
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
        public unsafe void ConvertManagedToNative_Icon()
        {
            bool cancelSet = true;
            Icon exclamationIcon = SystemIcons.Exclamation;

            IPicture.Interface picture = (IPicture.Interface)Instance.ConvertManagedToNative(exclamationIcon, null, ref cancelSet);

            Assert.False(cancelSet);
            picture.get_Type(out short type).ThrowOnFailure();
            picture.get_Height(out int height).ThrowOnFailure();
            picture.get_Width(out int width).ThrowOnFailure();
            Assert.Equal((short)PICTYPE.PICTYPE_ICON, type);
            Assert.Equal(exclamationIcon.Height, GdiHelper.HimetricToPixelY(height));
            Assert.Equal(exclamationIcon.Width, GdiHelper.HimetricToPixelX(width));

            // Should get the cached value
            Assert.Same(picture, (IPicture.Interface)Instance.ConvertManagedToNative(exclamationIcon, null, ref cancelSet));

            // And we should also round trip to the same value
            Assert.Same(exclamationIcon, Instance.ConvertNativeToManaged(picture, null));
        }

        [Fact]
        public unsafe void ConvertManagedToNative_Bitmap()
        {
            bool cancelSet = true;
            using Bitmap bitmap = new Bitmap(42, 70);

            IPicture.Interface picture = (IPicture.Interface)Instance.ConvertManagedToNative(bitmap, null, ref cancelSet);

            Assert.False(cancelSet);
            picture.get_Type(out short type).ThrowOnFailure();
            picture.get_Height(out int height).ThrowOnFailure();
            picture.get_Width(out int width).ThrowOnFailure();
            Assert.Equal((short)PICTYPE.PICTYPE_BITMAP, type);
            Assert.Equal(bitmap.Height, GdiHelper.HimetricToPixelY(height));
            Assert.Equal(bitmap.Width, GdiHelper.HimetricToPixelX(width));

            // Should get the cached value
            Assert.Same(picture, (IPicture.Interface)Instance.ConvertManagedToNative(bitmap, null, ref cancelSet));

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

        private unsafe class IPictureMock : IPicture.Interface
        {
            public virtual HRESULT get_Handle(uint* pHandle) => HRESULT.S_OK;

            public virtual HRESULT get_hPal(uint* phPal) => HRESULT.S_OK;

            public virtual HRESULT get_Type(short* pType) => HRESULT.S_OK;

            public virtual HRESULT get_Width(int* pWidth) => HRESULT.S_OK;

            public virtual HRESULT get_Height(int* pHeight) => HRESULT.S_OK;

            public virtual HRESULT Render(HDC hDC, int x, int y, int cx, int cy, int xSrc, int ySrc, int cxSrc, int cySrc, RECT* pRcWBounds)
                => HRESULT.S_OK;

            public virtual HRESULT set_hPal(uint hPal) => HRESULT.S_OK;

            public virtual HRESULT get_CurDC(HDC* phDC) => HRESULT.S_OK;

            public virtual HRESULT SelectPicture(HDC hDCIn, HDC* phDCOut, uint* phBmpOut) => HRESULT.S_OK;

            public virtual HRESULT get_KeepOriginalFormat(BOOL* pKeep) => HRESULT.S_OK;

            public virtual HRESULT put_KeepOriginalFormat(BOOL keep) => HRESULT.S_OK;

            public virtual HRESULT PictureChanged() => HRESULT.S_OK;

            public virtual HRESULT SaveAsFile(IStream* pStream, BOOL fSaveMemCopy, int* pCbSize) => HRESULT.S_OK;

            public virtual HRESULT get_Attributes(uint* pDwAttr) => HRESULT.S_OK;
        }
    }
}
