// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Moq;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class KeyboardTooltipStateMachineTests
    {
        [Fact]
        public void HookToolTip()
        {
            using (ToolTip toolTip = new ToolTip())
            {
                var mock = new Mock<IKeyboardToolTip>(MockBehavior.Strict);
                IKeyboardToolTip keyboardToolTip = mock.Object;

                // Validate we don't get OnHooked if AllowsToolTip is false
                mock.Setup(m => m.AllowsToolTip()).Returns(false);
                mock.Setup(m => m.OnHooked(toolTip));
                KeyboardToolTipStateMachine.Instance.Hook(keyboardToolTip, toolTip);
                mock.Verify(m => m.AllowsToolTip());

                mock.Reset();

                // Now validate we get OnHooked if AllowsToolTip is true
                mock.Setup(m => m.AllowsToolTip()).Returns(true);
                mock.Setup(m => m.OnHooked(toolTip));
                KeyboardToolTipStateMachine.Instance.Hook(keyboardToolTip, toolTip);
                mock.Verify(m => m.AllowsToolTip());
                mock.Verify(m => m.OnHooked(toolTip), Times.Once);

                mock.Reset();

                // Validate we don't get OnUnhooked if AllowsToolTip is false
                mock.Setup(m => m.AllowsToolTip()).Returns(false);
                mock.Setup(m => m.OnUnhooked(toolTip));
                KeyboardToolTipStateMachine.Instance.Unhook(keyboardToolTip, toolTip);
                mock.Verify(m => m.AllowsToolTip());

                mock.Reset();

                // Finally validate we get OnUnhooked if AllowsToolTip is true
                mock.Setup(m => m.AllowsToolTip()).Returns(true);
                mock.Setup(m => m.OnUnhooked(toolTip));
                KeyboardToolTipStateMachine.Instance.Unhook(keyboardToolTip, toolTip);
                mock.Verify(m => m.AllowsToolTip());
                mock.Verify(m => m.OnUnhooked(toolTip), Times.Once);
            }
        }
    }
}
