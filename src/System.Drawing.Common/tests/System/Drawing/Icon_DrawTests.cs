// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32;
using Windows.Win32.Graphics.Gdi;

namespace System.Drawing.Tests;

public class Icon_DrawTests
{
    private const string TestIcon = "48x48_multiple_entries_4bit.ico";

    public static IEnumerable<object[]> Draw_TestData()
    {
        // Empty rectangle
        yield return new object[] { Rectangle.Empty };
        // Normal rectangle at origin
        yield return new object[] { new Rectangle(0, 0, 32, 32) };
        // Normal rectangle with offset
        yield return new object[] { new Rectangle(10, 20, 64, 64) };
        // Rectangle smaller than icon
        yield return new object[] { new Rectangle(5, 5, 16, 16) };
        // Oversized rectangle
        yield return new object[] { new Rectangle(0, 0, 200, 200) };
    }

    [Theory]
    [MemberData(nameof(Draw_TestData))]
    public void Graphics_DrawIcon_Invoke_Success(Rectangle targetRect)
    {
        using Icon icon = new(Helpers.GetTestBitmapPath(TestIcon));
        using Bitmap image = new(100, 100);
        using Graphics graphics = Graphics.FromImage(image);
        graphics.DrawIcon(icon, targetRect);
    }

    [Theory]
    [MemberData(nameof(Draw_TestData))]
    public void Graphics_DrawIconUnstretched_Invoke_Success(Rectangle targetRect)
    {
        using Icon icon = new(Helpers.GetTestBitmapPath(TestIcon));
        using Bitmap image = new(100, 100);
        using Graphics graphics = Graphics.FromImage(image);
        graphics.DrawIconUnstretched(icon, targetRect);
    }

    [Fact]
    public void Graphics_DrawIcon_IntInt_Success()
    {
        using Icon icon = new(Helpers.GetTestBitmapPath(TestIcon));
        using Bitmap image = new(100, 100);
        using Graphics graphics = Graphics.FromImage(image);
        graphics.DrawIcon(icon, 10, 20);
    }

    [Fact]
    public void Graphics_DrawIcon_NullIcon_ThrowsArgumentNullException()
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        AssertExtensions.Throws<ArgumentNullException>("icon", () => graphics.DrawIcon(null, 0, 0));
        AssertExtensions.Throws<ArgumentNullException>("icon", () => graphics.DrawIcon(null, Rectangle.Empty));
        AssertExtensions.Throws<ArgumentNullException>("icon", () => graphics.DrawIconUnstretched(null, Rectangle.Empty));
    }

    // --- Group B: Direct internal Icon.Draw / DrawUnstretched on screen DC (exercises the GDI helper) ---

    [Theory]
    [MemberData(nameof(Draw_TestData))]
    public void Icon_Draw_InvokeOnScreenDC_DoesNotThrow(Rectangle targetRect)
    {
        using Icon icon = new(Helpers.GetTestBitmapPath(TestIcon));
        using GetDcScope hdc = new(PInvokeCore.GetDesktopWindow());
        using Graphics graphics = Graphics.FromHdcInternal(hdc);
        icon.Draw(graphics, targetRect);
    }

    [Theory]
    [MemberData(nameof(Draw_TestData))]
    public void Icon_DrawUnstretched_InvokeOnScreenDC_DoesNotThrow(Rectangle targetRect)
    {
        using Icon icon = new(Helpers.GetTestBitmapPath(TestIcon));
        using GetDcScope hdc = new(PInvokeCore.GetDesktopWindow());
        using Graphics graphics = Graphics.FromHdcInternal(hdc);
        icon.DrawUnstretched(graphics, targetRect);
    }

    [Fact]
    public void Icon_Draw_InvokeOnScreenDC_NotBlank()
    {
        using Icon icon = new(Helpers.GetTestBitmapPath("32x32_one_entry_4bit.ico"));
        using Bitmap bitmap = new(100, 100);
        using Graphics graphics = Graphics.FromImage(bitmap);

        // Draw the icon at an offset within the bitmap.
        icon.Draw(graphics, new Rectangle(10, 20, 32, 32));

        // Verify that drawing produced non-blank pixels.
        Helpers.VerifyBitmapNotBlank(bitmap);
    }

    // --- Group C: Transform offset ---

    [Fact]
    public void Icon_Draw_TranslateTransform_DoesNotThrow()
    {
        using Icon icon = new(Helpers.GetTestBitmapPath(TestIcon));
        using Bitmap image = new(100, 100);
        using Graphics graphics = Graphics.FromImage(image);

        // Non-integer translation to exercise the offset calculation.
        graphics.TranslateTransform(5.4f, 7.6f);
        icon.Draw(graphics, new Rectangle(0, 0, 32, 32));
    }

    [Fact]
    public void Icon_DrawUnstretched_TranslateTransform_DoesNotThrow()
    {
        using Icon icon = new(Helpers.GetTestBitmapPath(TestIcon));
        using Bitmap image = new(100, 100);
        using Graphics graphics = Graphics.FromImage(image);

        graphics.TranslateTransform(5.4f, 7.6f);
        icon.DrawUnstretched(graphics, new Rectangle(0, 0, 32, 32));
    }

    // --- Group D: Empty target defaults to native icon size ---

    [Fact]
    public void Icon_Draw_EmptyTarget_UsesNativeSize()
    {
        using Icon icon = new(Helpers.GetTestBitmapPath(TestIcon));
        using Bitmap image = new(100, 100);
        using Graphics graphics = Graphics.FromImage(image);

        // Empty rectangle should be treated as the native icon size (32x32).
        icon.Draw(graphics, Rectangle.Empty);
        Helpers.VerifyBitmapNotBlank(image);
    }

    [Fact]
    public void Icon_DrawUnstretched_EmptyTarget_UsesNativeSize()
    {
        using Icon icon = new(Helpers.GetTestBitmapPath(TestIcon));
        using Bitmap image = new(100, 100);
        using Graphics graphics = Graphics.FromImage(image);

        icon.DrawUnstretched(graphics, Rectangle.Empty);
        Helpers.VerifyBitmapNotBlank(image);
    }
}
