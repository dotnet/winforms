// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using Xunit;

namespace System.Windows.Forms.Tests
{
    // NB: doesn't require thread affinity
    public class PrintPreviewControlTests : IClassFixture<ThreadExceptionFixture>
    {
        private const int emptyColorArgb = 0;
        private const int blueColorArgb = -16776961;
        private const int greenColorArgb = -16744448;
        private const int controlDarkColorArgb = -6250336;
        private const int appWorkSpaceNoHcColorArgb = -5526613;
        private const int appWorkSpaceHcColorArgb = -1;

        [Theory]
        [InlineData(emptyColorArgb, false, appWorkSpaceNoHcColorArgb)]
        [InlineData(emptyColorArgb, true, controlDarkColorArgb)]
        [InlineData(blueColorArgb, false, blueColorArgb)]
        [InlineData(greenColorArgb, true, greenColorArgb)]
        public void ShowPrintPreviewControl_BackColorIsCorrect(int customBackColorArgb, bool isHighContrast, int expectedBackColorArgb)
        {
            var control = new PrintPreviewControl();

            if (customBackColorArgb != emptyColorArgb)
            {
                control.BackColor = Color.FromArgb(customBackColorArgb);
            }

            int actualBackColorArgb = control.TestAccessor().Dynamic.GetBackColor(isHighContrast).ToArgb();
            Assert.Equal(expectedBackColorArgb, actualBackColorArgb);

            // Default AppWorkSpace color in HC theme does not allow to follow HC standards.
            if (isHighContrast)
            {
                Assert.True(!appWorkSpaceHcColorArgb.Equals(actualBackColorArgb));
            }
        }
    }
}
