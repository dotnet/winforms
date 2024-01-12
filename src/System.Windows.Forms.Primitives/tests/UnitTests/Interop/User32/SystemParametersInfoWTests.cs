// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms.Primitives.Tests.Interop.User32;

public class SystemParametersInfoWTests
{
    [Fact]
    public void SystemParametersInfoW_check_fonts()
    {
        NONCLIENTMETRICSW data = default;

        bool result = PInvokeCore.SystemParametersInfo(ref data);
        Assert.True(result);

        Font captionFont = Font.FromLogFont(data.lfCaptionFont);
        AreEqual(SystemFonts.CaptionFont!, captionFont);

        Font menuFont = Font.FromLogFont(data.lfMenuFont);
        AreEqual(SystemFonts.CaptionFont!, menuFont);

        Font messageFont = Font.FromLogFont(data.lfMessageFont);
        AreEqual(SystemFonts.MessageBoxFont!, messageFont);

        Font smCaptionFont = Font.FromLogFont(data.lfSmCaptionFont);
        AreEqual(SystemFonts.SmallCaptionFont!, smCaptionFont);

        Font statusFont = Font.FromLogFont(data.lfStatusFont);
        AreEqual(SystemFonts.StatusFont!, statusFont);

        static void AreEqual(Font expected, Font actual)
        {
            Assert.Equal(expected.Name, actual.Name);
            Assert.Equal(expected.SizeInPoints, actual.SizeInPoints);
            Assert.Equal(expected.GdiCharSet, actual.GdiCharSet);
            Assert.Equal(expected.Style, actual.Style);
        }
    }
}
