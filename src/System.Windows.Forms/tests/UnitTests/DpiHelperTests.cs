// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Windows.Forms.TestUtilities;

namespace System.Windows.Forms.Tests;

// NB: doesn't require thread affinity
public class DpiHelperTests
{
    [Theory]
    [IntegerData<int>]
    public void DpiHelper_LogicalToDeviceUnits(int value)
    {
        var expected = Math.Round(value * (DpiHelper.DeviceDpi / DpiHelper.LogicalDpi));

        Assert.Equal(expected, DpiHelper.LogicalToDeviceUnits(value));
    }

    [Theory]
    [CommonMemberData(typeof(CommonTestHelper), nameof(CommonTestHelper.GetSizeTheoryData))]
    public void DpiHelper_LogicalToDeviceUnitsSize(Size value)
    {
        var expected = new Size((int)Math.Round(value.Width * (DpiHelper.DeviceDpi / DpiHelper.LogicalDpi)),
                                 (int)Math.Round(value.Height * (DpiHelper.DeviceDpi / DpiHelper.LogicalDpi)));

        Assert.Equal(expected, DpiHelper.LogicalToDeviceUnits(value));
    }
}
