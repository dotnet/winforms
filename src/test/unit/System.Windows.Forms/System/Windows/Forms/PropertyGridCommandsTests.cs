// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Windows.Forms.PropertyGridInternal;

namespace System.Windows.Forms.Tests;

public class PropertyGridCommandsTests
{
    [Fact]
    public void PropertyGridCommand_CommandID_HasExpectedValues()
    {
        Guid expectedGuid = new("5a51cf82-7619-4a5d-b054-47f438425aa7");

        PropertyGridCommands.Reset.Guid.Should().Be(expectedGuid);
        PropertyGridCommands.Reset.ID.Should().Be(0x3000);

        PropertyGridCommands.Description.Guid.Should().Be(expectedGuid);
        PropertyGridCommands.Description.ID.Should().Be(0x3001);

        PropertyGridCommands.Hide.Guid.Should().Be(expectedGuid);
        PropertyGridCommands.Hide.ID.Should().Be(0x3002);

        PropertyGridCommands.Commands.Guid.Should().Be(expectedGuid);
        PropertyGridCommands.Commands.ID.Should().Be(0x3010);
    }

    [Fact]
    public void WfcMenuGroup_HasExpectedGuid()
    {
        PropertyGridCommands propertyGridCommands = new();
        Guid tests = propertyGridCommands.TestAccessor().Dynamic.wfcMenuGroup;

        tests.Should().Be(new Guid("a72bd644-1979-4cbc-a620-ea4112198a66"));
    }

    [Fact]
    public void WfcMenuCommand_HasExpectedGuid()
    {
        PropertyGridCommands propertyGridCommands = new();
        Guid tests = propertyGridCommands.TestAccessor().Dynamic.wfcMenuCommand;

        tests.Should().Be(new Guid("5a51cf82-7619-4a5d-b054-47f438425aa7"));
    }
}
