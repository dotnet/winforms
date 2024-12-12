// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Windows.Forms.Primitives.Tests.Interop.Mocks;
using Windows.Win32.Graphics.GdiPlus;
using Windows.Win32.System.Com;
using Windows.Win32.System.Ole;
using Windows.Win32.System.Variant;

namespace System.Windows.Forms.Primitives.Tests.Interop.Ole32;

[Collection("Sequential")]
public unsafe class IPictureTests
{
    [StaFact]
    public void GetIPictureFromCursor()
    {
        using MockCursor arrow = new(PInvoke.IDC_ARROW);

        using var picture = Icon.FromHandle(arrow.Handle).CreateIPicture(copy: true);
        Assert.False(picture.IsNull);
        picture.Value->get_Type(out PICTYPE type);
        Assert.Equal(PICTYPE.PICTYPE_ICON, type);

        picture.Value->get_Height(out int height);
        Assert.Equal(arrow.Size.Height, GdiHelper.HimetricToPixelY(height));
        picture.Value->get_Width(out int width);
        Assert.Equal(arrow.Size.Width, GdiHelper.HimetricToPixelX(width));
    }

    [StaFact]
    public void GetIPictureFromImage()
    {
        using MockCursor arrow = new(PInvoke.IDC_ARROW);
        using Icon icon = Icon.FromHandle(arrow.Handle);
        using Bitmap bitmap = icon.ToBitmap();
        using var picture = bitmap.CreateIPicture();
        Assert.False(picture.IsNull);
        picture.Value->get_Type(out PICTYPE type);
        Assert.Equal(PICTYPE.PICTYPE_BITMAP, type);

        picture.Value->get_Height(out int height);
        Assert.Equal(bitmap.Size.Height, GdiHelper.HimetricToPixelY(height));
        picture.Value->get_Width(out int width);
        Assert.Equal(bitmap.Size.Width, GdiHelper.HimetricToPixelX(width));
    }

    [StaFact]
    public void GetIPictureDispFromImage()
    {
        using Icon icon = SystemIcons.Question;
        using Bitmap bitmap = icon.ToBitmap();
        using var picture = bitmap.CreateIPictureDisp();
        Assert.False(picture.IsNull);
        using VARIANT variant = default;

        IDispatch* dispatch = (IDispatch*)picture.Value;
        dispatch->TryGetProperty(PInvokeCore.DISPID_PICT_TYPE, &variant).ThrowOnFailure();
        Assert.Equal(PICTYPE.PICTYPE_BITMAP, (PICTYPE)variant.data.iVal);

        dispatch->TryGetProperty(PInvokeCore.DISPID_PICT_HEIGHT, &variant).ThrowOnFailure();
        Assert.Equal(bitmap.Size.Height, GdiHelper.HimetricToPixelY((int)variant.data.uintVal));

        dispatch->TryGetProperty(PInvokeCore.DISPID_PICT_WIDTH, &variant).ThrowOnFailure();
        Assert.Equal(bitmap.Size.Width, GdiHelper.HimetricToPixelX((int)variant.data.uintVal));
    }

    [StaFact]
    public void GetPictureFromIPicture()
    {
        using Icon icon = SystemIcons.Exclamation;
        using Bitmap bitmap = icon.ToBitmap();
        using var picture = bitmap.CreateIPicture();
        Assert.False(picture.IsNull);
        using Image? image = ImageExtensions.ToImage(picture);
        Assert.NotNull(image);
        Assert.Equal(bitmap.Size, image.Size);
    }

    [StaFact]
    public void GetPictureFromIPictureDisp()
    {
        using Bitmap bitmap = new(100, 200);
        using var picture = bitmap.CreateIPictureDisp();
        Assert.False(picture.IsNull);
        using Image? image = ImageExtensions.ToImage(picture);
        Assert.NotNull(image);
        Assert.Equal(bitmap.Size, image.Size);
    }
}
