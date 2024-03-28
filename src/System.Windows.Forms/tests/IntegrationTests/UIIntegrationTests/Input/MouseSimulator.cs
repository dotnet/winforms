// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Runtime.InteropServices;
using Windows.Win32.UI.Input.KeyboardAndMouse;

namespace System.Windows.Forms.UITests.Input;

internal class MouseSimulator
{
    private readonly InputSimulator _inputSimulator;

    public MouseSimulator(InputSimulator inputSimulator)
    {
        _inputSimulator = inputSimulator;
    }

    public KeyboardSimulator Keyboard => _inputSimulator.Keyboard;

    internal MouseSimulator LeftButtonDown()
    {
        return ButtonDown(MouseButtons.Left);
    }

    internal MouseSimulator LeftButtonUp()
    {
        return ButtonUp(MouseButtons.Left);
    }

    internal MouseSimulator LeftButtonClick()
    {
        return ButtonClick(MouseButtons.Left);
    }

    internal MouseSimulator LeftButtonDoubleClick()
    {
        return ButtonDoubleClick(MouseButtons.Left);
    }

    internal MouseSimulator MiddleButtonDown()
    {
        return ButtonDown(MouseButtons.Middle);
    }

    internal MouseSimulator MiddleButtonUp()
    {
        return ButtonUp(MouseButtons.Middle);
    }

    internal MouseSimulator MiddleButtonClick()
    {
        return ButtonClick(MouseButtons.Middle);
    }

    internal MouseSimulator MiddleButtonDoubleClick()
    {
        return ButtonDoubleClick(MouseButtons.Middle);
    }

    internal MouseSimulator RightButtonDown()
    {
        return ButtonDown(MouseButtons.Right);
    }

    internal MouseSimulator RightButtonUp()
    {
        return ButtonUp(MouseButtons.Right);
    }

    internal MouseSimulator RightButtonClick()
    {
        return ButtonClick(MouseButtons.Right);
    }

    internal MouseSimulator RightButtonDoubleClick()
    {
        return ButtonDoubleClick(MouseButtons.Right);
    }

    private MouseSimulator ButtonDown(MouseButtons button)
    {
        Span<INPUT> inputs =
        [
            InputBuilder.MouseButtonDown(button),
        ];

        PInvoke.SendInput(inputs, Marshal.SizeOf<INPUT>());
        return this;
    }

    private MouseSimulator ButtonUp(MouseButtons button)
    {
        Span<INPUT> inputs =
        [
            InputBuilder.MouseButtonUp(button),
        ];

        PInvoke.SendInput(inputs, Marshal.SizeOf<INPUT>());
        return this;
    }

    private MouseSimulator ButtonClick(MouseButtons button)
    {
        Span<INPUT> inputs =
        [
            InputBuilder.MouseButtonDown(button),
            InputBuilder.MouseButtonUp(button),
        ];

        PInvoke.SendInput(inputs, Marshal.SizeOf<INPUT>());
        return this;
    }

    private MouseSimulator ButtonDoubleClick(MouseButtons button)
    {
        Span<INPUT> inputs =
        [
            InputBuilder.MouseButtonDown(button),
            InputBuilder.MouseButtonUp(button),
            InputBuilder.MouseButtonDown(button),
            InputBuilder.MouseButtonUp(button),
        ];

        PInvoke.SendInput(inputs, Marshal.SizeOf<INPUT>());
        return this;
    }

    internal MouseSimulator MoveMouseBy(int x, int y, bool bypassOperatingSystemSettings = true)
    {
        if (bypassOperatingSystemSettings)
        {
            // Redirect to MoveMouseTo, since the implementation of MoveMouseBy is subject to the OS's settings for
            // mouse speed and acceleration.
            Assert.True(PInvoke.GetPhysicalCursorPos(out var point));
            var virtualPoint = ControlTestBase.ToVirtualPoint(point + new Size(x, y));
            return MoveMouseTo(virtualPoint.X, virtualPoint.Y);
        }

        Span<INPUT> inputs =
        [
            InputBuilder.RelativeMouseMovement(x, y),
        ];

        PInvoke.SendInput(inputs, Marshal.SizeOf<INPUT>());
        return this;
    }

    internal MouseSimulator MoveMouseTo(double absoluteX, double absoluteY)
    {
        Span<INPUT> inputs =
        [
            InputBuilder.AbsoluteMouseMovement((int)absoluteX, (int)absoluteY),
        ];

        PInvoke.SendInput(inputs, Marshal.SizeOf<INPUT>());
        return this;
    }

    internal MouseSimulator MoveMouseToPositionOnVirtualDesktop(double absoluteX, double absoluteY)
    {
        Span<INPUT> inputs =
        [
            InputBuilder.AbsoluteMouseMovementOnVirtualDesktop((int)absoluteX, (int)absoluteY),
        ];

        PInvoke.SendInput(inputs, Marshal.SizeOf<INPUT>());
        return this;
    }

    internal MouseSimulator Sleep(TimeSpan time)
    {
        Thread.Sleep(time);
        return this;
    }
}
