// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

namespace System.Windows.Forms;

public class NumericUpDownAccelerationTests
{
    [WinFormsFact]
    public void NumericUpDownAcceleration_SecondsProperty_WorksAsExpected()
    {
        NumericUpDownAcceleration acceleration = new(5, 1.0m);

        acceleration.Seconds.Should().Be(5);

        acceleration.Seconds = 10;
        acceleration.Seconds.Should().Be(10);

        Action act = () => acceleration.Seconds = -1;
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [WinFormsFact]
    public void NumericUpDownAcceleration_IncrementProperty_WorksAsExpected()
    {
        NumericUpDownAcceleration acceleration = new(5, 1.0m);

        acceleration.Increment.Should().Be(1.0m);

        acceleration.Increment = 2.0m;
        acceleration.Increment.Should().Be(2.0m);

        Action act = () => acceleration.Increment = -1.0m;
        act.Should().Throw<ArgumentOutOfRangeException>();
    }
}
