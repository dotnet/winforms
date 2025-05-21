// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel.Design;
using System.Windows.Forms.PropertyGridInternal;

namespace System.Windows.Forms.Tests;

public class PropertyGridCommandsTests
{
    [WinFormsTheory]
    [InlineData(0x3000)]
    [InlineData(0x3001)]
    [InlineData(0x3002)]
    [InlineData(0x3010)]
    public void PropertyGridCommand_CommandID_HasExpectedValues(int expectedId)
    {
        Guid expectedGuid = new Guid("5a51cf82-7619-4a5d-b054-47f438425aa7");
        CommandID command = expectedId switch
        {
            0x3000 => PropertyGridCommands.Reset,
            0x3001 => PropertyGridCommands.Description,
            0x3002 => PropertyGridCommands.Hide,
            0x3010 => PropertyGridCommands.Commands,
            _ => throw new ArgumentOutOfRangeException(nameof(expectedId))
        };

        command.Should().NotBeNull();
        command.Guid.Should().Be(expectedGuid);
        command.ID.Should().Be(expectedId);
    }

    private class PropertyGridCommandsAccessor : PropertyGridCommands
    {
        public static Guid WfcMenuGroup => wfcMenuGroup;
        public static Guid WfcMenuCommand => wfcMenuCommand;
    }

    [WinFormsFact]
    public void WfcMenuGroup_HasExpectedGuid()
    {
        PropertyGridCommandsAccessor.WfcMenuGroup
            .Should().Be(new Guid("a72bd644-1979-4cbc-a620-ea4112198a66"));
    }

    [WinFormsFact]
    public void WfcMenuCommand_HasExpectedGuid()
    {
        PropertyGridCommandsAccessor.WfcMenuCommand
            .Should().Be(new Guid("5a51cf82-7619-4a5d-b054-47f438425aa7"));
    }
}
