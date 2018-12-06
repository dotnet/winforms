// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using System.Drawing;
using WinForms.Common.Tests;

namespace System.Windows.Forms.Tests
{
    public class DpiHelperTests
    {

        /// <summary>
        /// Data for the LogicalToDeviceUnits test
        /// </summary>
        public static TheoryData<int> LogicalToDeviceUnitsData =>
            CommonTestHelper.GetIntTheoryData();

        [Theory]
        [MemberData(nameof(LogicalToDeviceUnitsData))]
        public void DpiHelper_LogicalToDeviceUnits(int value)
        {
            var expected = Math.Round(value * (DpiHelper.DeviceDpi / DpiHelper.LogicalDpi));

            Assert.Equal(expected, DpiHelper.LogicalToDeviceUnits(value));
        }

        /// <summary>
        /// Data for the LogicalToDeviceUnitsSize test
        /// </summary>
        public static TheoryData<Size> LogicalToDeviceUnitsSizeData =>
            TestHelper.GetSizeTheoryData();

        [Theory]
        [MemberData(nameof(LogicalToDeviceUnitsSizeData))]
        public void DpiHelper_LogicalToDeviceUnitsSize(Size value)
        {
            var expected = new Size ((int)Math.Round(value.Width * (DpiHelper.DeviceDpi / DpiHelper.LogicalDpi)),
                                     (int)Math.Round(value.Height * (DpiHelper.DeviceDpi / DpiHelper.LogicalDpi)));

            Assert.Equal(expected, DpiHelper.LogicalToDeviceUnits(value));
        }
    }
}
