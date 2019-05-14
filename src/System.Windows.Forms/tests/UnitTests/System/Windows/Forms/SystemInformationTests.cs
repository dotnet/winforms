// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Windows.Forms.Tests
{
    public class SystemInformationTests
    {
        private const int LogicalDpi = 96;

        [Fact]
        public void SystemInformation_PowerStatus_Get_ReturnsExpected()
        {
            PowerStatus status = SystemInformation.PowerStatus;
            Assert.NotNull(status);
            Assert.Same(status, SystemInformation.PowerStatus);
        }

        [Fact]
        public void SystemInformation_VerticalScrollBarArrowHeight_LogicalDpi_ReturnsVerticalScrollBarArrowHeight()
        {
            Assert.Equal(SystemInformation.VerticalScrollBarArrowHeight, SystemInformation.VerticalScrollBarArrowHeightForDpi(LogicalDpi));
        }

        [Fact]
        public void SystemInformation_GetHorizontalScrollBarArrowWidthForDpi_LogicalDpi_HorizontalScrollBarArrowWidth()
        {
            Assert.Equal(SystemInformation.HorizontalScrollBarArrowWidth, SystemInformation.GetHorizontalScrollBarArrowWidthForDpi(LogicalDpi));
        }
    }
}
