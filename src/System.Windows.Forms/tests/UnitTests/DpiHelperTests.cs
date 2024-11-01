// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms.Tests;

// NB: doesn't require thread affinity
public class DpiHelperTests
{
    [Theory]
    [IntegerData<int>]
    public void DpiHelper_LogicalToDeviceUnits(int value)
    {
        int expected = (int)Math.Round(value * (ScaleHelper.InitialSystemDpi / (double)ScaleHelper.OneHundredPercentLogicalDpi));

        Assert.Equal(expected, ScaleHelper.ScaleToDpi(value, ScaleHelper.InitialSystemDpi));
    }

    [Theory]
    [CommonMemberData(typeof(CommonTestHelper), nameof(CommonTestHelper.GetSizeTheoryData))]
    public void DpiHelper_LogicalToDeviceUnitsSize(Size value)
    {
        Size expected = new(
            (int)Math.Round(value.Width * (ScaleHelper.InitialSystemDpi / (double)ScaleHelper.OneHundredPercentLogicalDpi)),
            (int)Math.Round(value.Height * (ScaleHelper.InitialSystemDpi / (double)ScaleHelper.OneHundredPercentLogicalDpi)));

        Assert.Equal(expected, ScaleHelper.ScaleToDpi(value, ScaleHelper.InitialSystemDpi));
    }
}
