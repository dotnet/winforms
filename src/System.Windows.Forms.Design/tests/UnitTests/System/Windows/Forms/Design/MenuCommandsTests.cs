// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel.Design;
using Xunit;

namespace System.Windows.Forms.Design.Tests
{
    public class MenuCommandsTests
    {
        [Fact]
        public void MenuCommands_Ctor_Default()
        {
            // Make sure it doesn't throw.
            new MenuCommands();
        }

        public static IEnumerable<object[]> Commands_TestData()
        {
            yield return new object[] { MenuCommands.ComponentTrayMenu, new CommandID(new Guid("74d21312-2aee-11d1-8bfb-00a0c90f26f7"), 1286) };
            yield return new object[] { MenuCommands.ContainerMenu, new CommandID(new Guid("74d21312-2aee-11d1-8bfb-00a0c90f26f7"), 1281) };
            yield return new object[] { MenuCommands.DesignerProperties, new CommandID(new Guid("74d21313-2aee-11d1-8bfb-00a0c90f26f7"), 4097) };
            yield return new object[] { MenuCommands.EditLabel, new CommandID(new Guid("5efc7975-14bc-11cf-9b2b-00aa00573819"), 338) };
            yield return new object[] { MenuCommands.KeyCancel, new CommandID(new Guid("1496a755-94de-11d0-8c3f-00c04fc2aae2"), 103) };
            yield return new object[] { MenuCommands.KeyDefaultAction, new CommandID(new Guid("1496a755-94de-11d0-8c3f-00c04fc2aae2"), 3) };
            yield return new object[] { MenuCommands.KeyEnd, new CommandID(new Guid("1496a755-94de-11d0-8c3f-00c04fc2aae2"), 17) };
            yield return new object[] { MenuCommands.KeyHome, new CommandID(new Guid("1496a755-94de-11d0-8c3f-00c04fc2aae2"), 15) };
            yield return new object[] { MenuCommands.KeyInvokeSmartTag, new CommandID(new Guid("1496a755-94de-11d0-8c3f-00c04fc2aae2"), 147) };
            yield return new object[] { MenuCommands.KeyMoveDown, new CommandID(new Guid("1496a755-94de-11d0-8c3f-00c04fc2aae2"), 13) };
            yield return new object[] { MenuCommands.KeyMoveLeft, new CommandID(new Guid("1496a755-94de-11d0-8c3f-00c04fc2aae2"), 7) };
            yield return new object[] { MenuCommands.KeyMoveRight, new CommandID(new Guid("1496a755-94de-11d0-8c3f-00c04fc2aae2"), 9) };
            yield return new object[] { MenuCommands.KeyMoveUp, new CommandID(new Guid("1496a755-94de-11d0-8c3f-00c04fc2aae2"), 11) };
            yield return new object[] { MenuCommands.KeyNudgeDown, new CommandID(new Guid("1496a755-94de-11d0-8c3f-00c04fc2aae2"), 1225) };
            yield return new object[] { MenuCommands.KeyNudgeHeightDecrease, new CommandID(new Guid("1496a755-94de-11d0-8c3f-00c04fc2aae2"), 1229) };
            yield return new object[] { MenuCommands.KeyNudgeHeightIncrease, new CommandID(new Guid("1496a755-94de-11d0-8c3f-00c04fc2aae2"), 1228) };
            yield return new object[] { MenuCommands.KeyNudgeLeft, new CommandID(new Guid("1496a755-94de-11d0-8c3f-00c04fc2aae2"), 1224) };
            yield return new object[] { MenuCommands.KeyNudgeRight, new CommandID(new Guid("1496a755-94de-11d0-8c3f-00c04fc2aae2"), 1226) };
            yield return new object[] { MenuCommands.KeyNudgeUp, new CommandID(new Guid("1496a755-94de-11d0-8c3f-00c04fc2aae2"), 1227) };
            yield return new object[] { MenuCommands.KeyNudgeWidthDecrease, new CommandID(new Guid("1496a755-94de-11d0-8c3f-00c04fc2aae2"), 1230) };
            yield return new object[] { MenuCommands.KeyNudgeWidthDecrease, new CommandID(new Guid("1496a755-94de-11d0-8c3f-00c04fc2aae2"), 1230) };
            yield return new object[] { MenuCommands.KeyNudgeHeightIncrease, new CommandID(new Guid("1496a755-94de-11d0-8c3f-00c04fc2aae2"), 1228) };
            yield return new object[] { MenuCommands.KeyReverseCancel, new CommandID(new Guid("74d21313-2aee-11d1-8bfb-00a0c90f26f7"), 16385) };
            yield return new object[] { MenuCommands.KeySelectNext, new CommandID(new Guid("1496a755-94de-11d0-8c3f-00c04fc2aae2"), 4) };
            yield return new object[] { MenuCommands.KeySelectPrevious, new CommandID(new Guid("1496a755-94de-11d0-8c3f-00c04fc2aae2"), 5) };
            yield return new object[] { MenuCommands.KeyShiftEnd, new CommandID(new Guid("1496a755-94de-11d0-8c3f-00c04fc2aae2"), 18) };
            yield return new object[] { MenuCommands.KeyShiftHome, new CommandID(new Guid("1496a755-94de-11d0-8c3f-00c04fc2aae2"), 16) };
            yield return new object[] { MenuCommands.KeySizeHeightDecrease, new CommandID(new Guid("1496a755-94de-11d0-8c3f-00c04fc2aae2"), 14) };
            yield return new object[] { MenuCommands.KeySizeHeightIncrease, new CommandID(new Guid("1496a755-94de-11d0-8c3f-00c04fc2aae2"), 12) };
            yield return new object[] { MenuCommands.KeySizeWidthDecrease, new CommandID(new Guid("1496a755-94de-11d0-8c3f-00c04fc2aae2"), 8) };
            yield return new object[] { MenuCommands.KeySizeWidthIncrease, new CommandID(new Guid("1496a755-94de-11d0-8c3f-00c04fc2aae2"), 10) };
            yield return new object[] { MenuCommands.KeyTabOrderSelect, new CommandID(new Guid("74d21313-2aee-11d1-8bfb-00a0c90f26f7"), 16405) };
            yield return new object[] { MenuCommands.SelectionMenu, new CommandID(new Guid("74d21312-2aee-11d1-8bfb-00a0c90f26f7"), 1280) };
            yield return new object[] { MenuCommands.SetStatusRectangle, new CommandID(new Guid("74d21313-2aee-11d1-8bfb-00a0c90f26f7"), 16388) };
            yield return new object[] { MenuCommands.SetStatusText, new CommandID(new Guid("74d21313-2aee-11d1-8bfb-00a0c90f26f7"), 16387) };
            yield return new object[] { MenuCommands.TraySelectionMenu, new CommandID(new Guid("74d21312-2aee-11d1-8bfb-00a0c90f26f7"), 1283) };
        }

        [Theory]
        [MemberData(nameof(Commands_TestData))]
        public void MenuCommands_Commands_Get_ReturnsExected(CommandID command, CommandID expected)
        {
            Assert.Equal(expected, command);
        }
    }
}
