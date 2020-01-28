﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class PowerStatusTests : IClassFixture<ThreadExceptionFixture>
    {
        [Fact]
        public void PowerStatus_BatteryChargeStatus_Get_ReturnsExpected()
        {
            PowerStatus status = SystemInformation.PowerStatus;

            // try a valid combination
            // an edge-case can surface on laptops that are not permanently connected to power
            // e.g. a charing laptop in a high performance mode would be HIGH | CHARGING
            Assert.True(EnumIsDefined(status.BatteryChargeStatus));

            // try an invalid (as of time of writing) combination
            Assert.False(EnumIsDefined((BatteryChargeStatus)67));

            bool EnumIsDefined(BatteryChargeStatus value)
            {
                // BatteryChargeStatus.Unknown == 1111_1111, OR'ing anything with it won't change the result
                // and thus won't catch invalid combinations, so we need to exclude if from further consideration

                if (value == BatteryChargeStatus.Unknown)
                {
                    return true;
                }

                var values = Enum.GetValues(typeof(BatteryChargeStatus))
                    .OfType<BatteryChargeStatus>()
                    .Where(v => v != BatteryChargeStatus.Unknown)
                    .Aggregate((e1, e2) => (e1 | e2));

                return (values & value) == value;
            }
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
