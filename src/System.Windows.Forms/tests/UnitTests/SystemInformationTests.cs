// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using System.Windows.Forms;

namespace System.Windows.Forms.Tests
{
    public class SystemInformationTests
    {
        private const int LogicalDpi = 96;

        [Fact]
        public void SystemInformation_CompareDeviceToLogicalValues()
        {
            Assert.Equal(SystemInformation.VerticalScrollBarArrowHeight, SystemInformation.VerticalScrollBarArrowHeightForDpi(LogicalDpi));
            Assert.Equal(SystemInformation.HorizontalScrollBarArrowWidth, SystemInformation.GetHorizontalScrollBarArrowWidthForDpi(LogicalDpi));
        }
    }
}
