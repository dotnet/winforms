// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms.Tests;

// NB: doesn't require thread affinity
public class PrintPreviewControlTests
{
    private const int EmptyColorArgb = 0;
    private const int BlueColorArgb = -16776961;
    private const int GreenColorArgb = -16744448;
    private const int ControlDarkColorArgb = -6250336;
    private const int AppWorkSpaceNoHcColorArgb = -5526613;
    private const int AppWorkSpaceHcColorArgb = -1;

    [Theory]
    [InlineData(EmptyColorArgb, false, AppWorkSpaceNoHcColorArgb)]
    [InlineData(EmptyColorArgb, true, ControlDarkColorArgb)]
    [InlineData(BlueColorArgb, false, BlueColorArgb)]
    [InlineData(GreenColorArgb, true, GreenColorArgb)]
    public void ShowPrintPreviewControl_BackColorIsCorrect(int customBackColorArgb, bool isHighContrast, int expectedBackColorArgb)
    {
        PrintPreviewControl control = new();

        if (customBackColorArgb != EmptyColorArgb)
        {
            control.BackColor = Color.FromArgb(customBackColorArgb);
        }

        int actualBackColorArgb = control.TestAccessor().Dynamic.GetBackColor(isHighContrast).ToArgb();
        Assert.Equal(expectedBackColorArgb, actualBackColorArgb);

        // Default AppWorkSpace color in HC theme does not allow to follow HC standards.
        if (isHighContrast)
        {
            Assert.True(!AppWorkSpaceHcColorArgb.Equals(actualBackColorArgb));
        }
    }
}
