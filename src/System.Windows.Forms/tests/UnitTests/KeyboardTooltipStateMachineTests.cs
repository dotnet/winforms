// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Moq;

namespace System.Windows.Forms.Tests;

public class KeyboardTooltipStateMachineTests
{
    [WinFormsFact]
    public void HookToolTip()
    {
        using ToolTip toolTip = new();
        Mock<IKeyboardToolTip> mock = new(MockBehavior.Strict);
        IKeyboardToolTip keyboardToolTip = mock.Object;

        // Validate we don't get OnHooked if AllowsToolTip is false
        mock.Setup(m => m.AllowsToolTip()).Returns(false);
        mock.Setup(m => m.OnHooked(toolTip));
        KeyboardToolTipStateMachine.Instance.Hook(keyboardToolTip, toolTip);
        mock.Verify(m => m.AllowsToolTip());

        mock.Reset();

        // Now validate we get OnHooked if AllowsToolTip is true.
        mock.Setup(m => m.AllowsToolTip()).Returns(true);
        mock.Setup(m => m.OnHooked(toolTip));
        KeyboardToolTipStateMachine.Instance.Hook(keyboardToolTip, toolTip);
        mock.Verify(m => m.AllowsToolTip());
        mock.Verify(m => m.OnHooked(toolTip), Times.Once);

        mock.Reset();

        // Validate we don't get OnUnhooked if AllowsToolTip is false.
        mock.Setup(m => m.AllowsToolTip()).Returns(false);
        mock.Setup(m => m.OnUnhooked(toolTip));
        KeyboardToolTipStateMachine.Instance.Unhook(keyboardToolTip, toolTip);
        mock.Verify(m => m.AllowsToolTip());

        mock.Reset();

        // Finally validate we get OnUnhooked if AllowsToolTip is true.
        mock.Setup(m => m.AllowsToolTip()).Returns(true);
        mock.Setup(m => m.OnUnhooked(toolTip));
        KeyboardToolTipStateMachine.Instance.Unhook(keyboardToolTip, toolTip);
        mock.Verify(m => m.AllowsToolTip());
        mock.Verify(m => m.OnUnhooked(toolTip), Times.Once);
    }

    [WinFormsTheory]
    [InlineData(Keys.ControlKey)]
    [InlineData(Keys.Escape)]
    [InlineData(Keys.ControlKey | Keys.ShiftKey | Keys.F10)]
    public void KeyboardTooltipStateMachine_DismissalKeyUp_NonPersistent_NotDismissed(Keys keys)
    {
        using TestControl control = new();
        using ToolTip toolTip = new();

        control.CreateControl();
        _ = toolTip.Handle;

        toolTip.SetToolTip(control, "Non-persistent");
        toolTip.AutoPopDelay = 888;

        // Simulate that the toolTip is shown.
        KeyboardToolTipStateMachine instance = KeyboardToolTipStateMachine.Instance;
        instance.TestAccessor().Dynamic._currentTool = control;
        instance.TestAccessor().Dynamic._currentState = KeyboardToolTipStateMachine.SmState.Shown;

        control.SimulateKeyUp(keys);

        IKeyboardToolTip currentTool = instance.TestAccessor().Dynamic._currentTool;
        string currentState = instance.TestAccessor().Dynamic._currentState.ToString();

        Assert.Equal(control, currentTool);
        Assert.Equal("Shown", currentState);
    }

    [WinFormsTheory]
    [InlineData(Keys.ControlKey, true)]
    [InlineData(Keys.Escape, true)]
    [InlineData(Keys.ControlKey | Keys.ShiftKey | Keys.F10, true)]
    [InlineData(Keys.ControlKey, false)]
    [InlineData(Keys.Escape, false)]
    [InlineData(Keys.ControlKey | Keys.ShiftKey | Keys.F10, false)]
    public void KeyboardTooltipStateMachine_DismissalKeyUp(Keys keys, bool isPersistent)
    {
        using TestControl control = new();
        using ToolTip toolTip = new();

        control.CreateControl();
        _ = toolTip.Handle;

        toolTip.SetToolTip(control, "test");
        toolTip.IsPersistent = isPersistent;

        // Simulate that the toolTip is shown.
        KeyboardToolTipStateMachine instance = KeyboardToolTipStateMachine.Instance;
        instance.TestAccessor().Dynamic._currentTool = control;
        instance.TestAccessor().Dynamic._currentState = KeyboardToolTipStateMachine.SmState.Shown;

        control.SimulateKeyUp(keys);

        IKeyboardToolTip currentTool = instance.TestAccessor().Dynamic._currentTool;
        string currentState = instance.TestAccessor().Dynamic._currentState.ToString();

        Assert.Equal(isPersistent && OsVersion.IsWindows11_OrGreater() ? "Hidden" : "Shown", currentState);
    }

    private class TestControl : Control
    {
        public void SimulateKeyUp(Keys keys) => base.OnKeyUp(new KeyEventArgs(keys));
    }
}
