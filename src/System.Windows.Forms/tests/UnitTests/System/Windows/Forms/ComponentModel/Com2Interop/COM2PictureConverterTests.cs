// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Windows.Forms.ComponentModel.Com2Interop;
using Windows.Win32.System.Com;
using Windows.Win32.System.Ole;
using Windows.Win32.System.Variant;

namespace System.Windows.Forms.Tests.ComponentModel.Com2Interop;

// NB: doesn't require thread affinity
public unsafe class COM2PictureConverterTests
{
    private static Com2PictureConverter Instance { get; } = new(new Com2PropertyDescriptor(
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
        Assert.Null(Instance.ConvertNativeToManaged(default, null));
    }

    private unsafe class TestIPicture : IPictureMock
    {
        private readonly nint _handle;
        private readonly PICTYPE _type;

        public TestIPicture(nint handle, PICTYPE type = PICTYPE.PICTYPE_NONE)
        {
            _handle = handle;
            _type = type;
        }

        public override unsafe HRESULT get_Handle(OLE_HANDLE* pHandle)
        {
            if (pHandle is null)
            {
                return HRESULT.E_POINTER;
            }

            *pHandle = new((uint)(int)_handle);
            return HRESULT.S_OK;
        }

        public override unsafe HRESULT get_Type(PICTYPE* pType)
        {
            if (pType is null)
            {
                return HRESULT.E_POINTER;
            }

            *pType = _type;
            return HRESULT.S_OK;
        }
    }

    [Fact]
    public void ConvertNativeToManaged_NullHandle()
    {
        TestIPicture nullIPicture = new(0);
        using var unknown = ComHelpers.GetComScope<IUnknown>(nullIPicture);
        Assert.Null(Instance.ConvertNativeToManaged((VARIANT)unknown.Value, null));
    }

    [Fact]
    public unsafe void ConvertNativeToManaged_Icon()
    {
        Icon errorIcon = SystemIcons.Error;
        nint handle = errorIcon.Handle;
        TestIPicture iconIPicture = new(handle, PICTYPE.PICTYPE_ICON);
        using var unknown = ComHelpers.GetComScope<IUnknown>(iconIPicture);

        using Icon icon = (Icon)Instance.ConvertNativeToManaged((VARIANT)unknown.Value, null);

        Assert.Equal(icon.Height, errorIcon.Height);
        Assert.Equal(icon.Width, errorIcon.Width);
        Assert.Equal(typeof(Icon), Instance.ManagedType);

        // We should get the cached object if the handle didn't change
        Assert.Same(icon, (Icon)Instance.ConvertNativeToManaged((VARIANT)unknown.Value, null));
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
            using var unknown = ComHelpers.GetComScope<IUnknown>(bitmapIPicture);
            using Bitmap bitmap = (Bitmap)Instance.ConvertNativeToManaged((VARIANT)unknown.Value, property: null);

            Assert.Equal(bitmap.Height, errorIcon.Height);
            Assert.Equal(bitmap.Width, errorIcon.Width);
            Assert.Equal(typeof(Bitmap), Instance.ManagedType);

            // We should get the cached object if the handle didn't change
            Assert.Same(bitmap, (Bitmap)Instance.ConvertNativeToManaged((VARIANT)unknown.Value, property: null));
        }
        finally
        {
            PInvokeCore.DeleteObject((HGDIOBJ)hBitmap);
        }
    }

    [Fact]
    public void ConvertNativeToManaged_UnsupportedPICTYPE()
    {
        // The converter asserts, but doesn't throw. Suppress asserts so that we can validate it returns null as expected.
        using (new NoAssertContext())
        {
            using var unknown = ComHelpers.GetComScope<IUnknown>(new TestIPicture(1, PICTYPE.PICTYPE_METAFILE));
            Assert.Null(Instance.ConvertNativeToManaged((VARIANT)unknown.Value, null));
        }
    }

    [Fact]
    public void ConvertManagedToNative_NullObject()
    {
        bool cancelSet = true;
        Assert.True(Instance.ConvertManagedToNative(null, null, ref cancelSet).IsEmpty);
        Assert.False(cancelSet);
    }

    [StaFact]
    public unsafe void ConvertManagedToNative_Icon()
    {
        bool cancelSet = true;
        Icon exclamationIcon = SystemIcons.Exclamation;

        using VARIANT native = Instance.ConvertManagedToNative(exclamationIcon, null, ref cancelSet);
        using ComScope<IPicture> picture = ComScope<IPicture>.QueryFrom((IUnknown*)native);

        Assert.False(cancelSet);
        picture.Value->get_Height(out int height).ThrowOnFailure();
        picture.Value->get_Width(out int width).ThrowOnFailure();
        picture.Value->get_Type(out PICTYPE type).ThrowOnFailure();
        Assert.Equal(PICTYPE.PICTYPE_ICON, type);
        Assert.Equal(exclamationIcon.Height, GdiHelper.HimetricToPixelY(height));
        Assert.Equal(exclamationIcon.Width, GdiHelper.HimetricToPixelX(width));

        // And we should also round trip to the same value
        Assert.Same(exclamationIcon, Instance.ConvertNativeToManaged((VARIANT)picture.AsUnknown, null));
    }

    [StaFact]
    public unsafe void ConvertManagedToNative_Bitmap()
    {
        bool cancelSet = true;
        using Bitmap bitmap = new(42, 70);

        using VARIANT native = Instance.ConvertManagedToNative(bitmap, null, ref cancelSet);
        using ComScope<IPicture> picture = ComScope<IPicture>.QueryFrom((IUnknown*)native);

        Assert.False(cancelSet);
        picture.Value->get_Height(out int height).ThrowOnFailure();
        picture.Value->get_Width(out int width).ThrowOnFailure();
        picture.Value->get_Type(out PICTYPE type).ThrowOnFailure();
        Assert.Equal(PICTYPE.PICTYPE_BITMAP, type);
        Assert.Equal(bitmap.Height, GdiHelper.HimetricToPixelY(height));
        Assert.Equal(bitmap.Width, GdiHelper.HimetricToPixelX(width));

        // And we should also round trip to the same value
        Assert.Same(bitmap, Instance.ConvertNativeToManaged((VARIANT)picture.AsUnknown, null));
    }

    [Fact]
    public void ConvertManagedToNative_UnknownObjectType()
    {
        // The converter asserts, but doesn't throw. Suppress asserts so
        // that we can validate it returns null as expected.
        using (new NoAssertContext())
        {
            bool cancelSet = true;
            Assert.True(Instance.ConvertManagedToNative(new object(), null, ref cancelSet).IsEmpty);
            Assert.False(cancelSet);
        }
    }

    private unsafe class IPictureMock : IPicture.Interface, IManagedWrapper<IPicture>
    {
        public virtual HRESULT get_Handle(OLE_HANDLE* pHandle) => HRESULT.S_OK;
        public virtual HRESULT get_Type(PICTYPE* pType) => HRESULT.S_OK;
        public HRESULT get_Width(int* pWidth) => HRESULT.S_OK;
        public HRESULT get_Height(int* pHeight) => HRESULT.S_OK;
        public HRESULT get_CurDC(HDC* phDC) => HRESULT.S_OK;
        public HRESULT get_KeepOriginalFormat(BOOL* pKeep) => HRESULT.S_OK;
        public HRESULT put_KeepOriginalFormat(BOOL keep) => HRESULT.S_OK;
        public HRESULT get_Attributes(uint* pDwAttr) => HRESULT.S_OK;
        public HRESULT get_hPal(OLE_HANDLE* phPal) => HRESULT.S_OK;
        public HRESULT Render(HDC hDC, int x, int y, int cx, int cy, int xSrc, int ySrc, int cxSrc, int cySrc, RECT* pRcWBounds) => HRESULT.S_OK;
        public HRESULT set_hPal(OLE_HANDLE hPal) => HRESULT.S_OK;
        public HRESULT SelectPicture(HDC hDCIn, HDC* phDCOut, OLE_HANDLE* phBmpOut) => HRESULT.S_OK;
        public HRESULT PictureChanged() => HRESULT.S_OK;
        public HRESULT SaveAsFile(IStream* pStream, BOOL fSaveMemCopy, int* pCbSize) => HRESULT.S_OK;
    }
}
