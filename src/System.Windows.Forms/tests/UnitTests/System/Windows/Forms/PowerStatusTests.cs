// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Windows.Forms.Tests
{
    public class PowerStatusTests
    {
        [Fact]
        public void PowerStatus_BatteryChargeStatus_Get_ReturnsExpected()
        {
            PowerStatus status = SystemInformation.PowerStatus;
            Assert.True(Enum.IsDefined(typeof(BatteryChargeStatus), status.BatteryChargeStatus));
        }

        [Fact]
        public void PowerStatus_BatteryFullLifetime_Get_ReturnsExpected()
        {
            PowerStatus status = SystemInformation.PowerStatus;
            Assert.True(status.BatteryFullLifetime >= -1);
        }

        [Fact]
        public void PowerStatus_BatteryLifePercent_Get_ReturnsExpected()
        {
            PowerStatus status = SystemInformation.PowerStatus;
            float value = status.BatteryLifePercent;
            Assert.True((value >= 0 && value <= 100) || value == 255);
        }

        [Fact]
        public void PowerStatus_BatteryLifeRemaining_Get_ReturnsExpected()
        {
            PowerStatus status = SystemInformation.PowerStatus;
            Assert.True(status.BatteryLifeRemaining >= -1);
        }

        [Fact]
        public void PowerStatus_PowerLineStatus_Get_ReturnsExpected()
        {
            PowerStatus status = SystemInformation.PowerStatus;
            Assert.True(Enum.IsDefined(typeof(PowerLineStatus), status.PowerLineStatus));
        }
    }
}
