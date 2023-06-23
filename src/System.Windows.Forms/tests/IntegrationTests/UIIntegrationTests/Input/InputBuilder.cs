// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Windows.Win32.UI.Input.KeyboardAndMouse;

namespace System.Windows.Forms.UITests.Input;

internal static class InputBuilder
{
    public static INPUT KeyDown(VIRTUAL_KEY keyCode)
    {
        uint scanCode = PInvoke.MapVirtualKey((uint)keyCode, MAP_VIRTUAL_KEY_TYPE.MAPVK_VK_TO_VSC_EX);

        return new INPUT
        {
            type = INPUT_TYPE.INPUT_KEYBOARD,
            Anonymous =
            {
                ki = new KEYBDINPUT
                {
                    wVk = keyCode,
                    wScan = (ushort)(scanCode & 0xFF),
                    dwFlags = (scanCode & 0xFF00) == 0xE000 ? KEYBD_EVENT_FLAGS.KEYEVENTF_EXTENDEDKEY : 0,
                    time = 0,
                    dwExtraInfo = 0,
                },
            },
        };
    }

    public static INPUT KeyUp(VIRTUAL_KEY keyCode)
    {
        var input = KeyDown(keyCode);
        input.Anonymous.ki.dwFlags |= KEYBD_EVENT_FLAGS.KEYEVENTF_KEYUP;
        return input;
    }

    public static INPUT CharacterDown(char character)
    {
        return new INPUT
        {
            type = INPUT_TYPE.INPUT_KEYBOARD,
            Anonymous =
            {
                ki = new KEYBDINPUT
                {
                    wVk = 0,
                    wScan = character,
                    dwFlags = KEYBD_EVENT_FLAGS.KEYEVENTF_UNICODE,
                    time = 0,
                    dwExtraInfo = 0,
                },
            },
        };
    }

    public static INPUT CharacterUp(char character)
    {
        var input = CharacterDown(character);
        input.Anonymous.ki.dwFlags |= KEYBD_EVENT_FLAGS.KEYEVENTF_KEYUP;
        return input;
    }

    public static INPUT MouseButtonDown(MouseButtons button)
    {
        return new INPUT
        {
            type = INPUT_TYPE.INPUT_MOUSE,
            Anonymous =
            {
                mi = new MOUSEINPUT
                {
                    dwFlags =
                        button switch
                        {
                            MouseButtons.Left => MOUSE_EVENT_FLAGS.MOUSEEVENTF_LEFTDOWN,
                            MouseButtons.Middle => MOUSE_EVENT_FLAGS.MOUSEEVENTF_MIDDLEDOWN,
                            MouseButtons.Right => MOUSE_EVENT_FLAGS.MOUSEEVENTF_RIGHTDOWN,
                            _ => throw new ArgumentException("Unexpected button", nameof(button)),
                        },
                }
            },
        };
    }

    public static INPUT MouseButtonUp(MouseButtons button)
    {
        return new INPUT
        {
            type = INPUT_TYPE.INPUT_MOUSE,
            Anonymous =
            {
                mi = new MOUSEINPUT
                {
                    dwFlags =
                        button switch
                        {
                            MouseButtons.Left => MOUSE_EVENT_FLAGS.MOUSEEVENTF_LEFTUP,
                            MouseButtons.Middle => MOUSE_EVENT_FLAGS.MOUSEEVENTF_MIDDLEUP,
                            MouseButtons.Right => MOUSE_EVENT_FLAGS.MOUSEEVENTF_RIGHTUP,
                            _ => throw new ArgumentException("Unexpected button", nameof(button)),
                        },
                }
            },
        };
    }

    public static INPUT RelativeMouseMovement(int x, int y)
    {
        return new INPUT
        {
            type = INPUT_TYPE.INPUT_MOUSE,
            Anonymous =
            {
                mi = new MOUSEINPUT
                {
                    dx = x,
                    dy = y,
                    dwFlags = MOUSE_EVENT_FLAGS.MOUSEEVENTF_MOVE | MOUSE_EVENT_FLAGS.MOUSEEVENTF_MOVE_NOCOALESCE,
                }
            },
        };
    }

    public static INPUT AbsoluteMouseMovement(int x, int y)
    {
        return new INPUT
        {
            type = INPUT_TYPE.INPUT_MOUSE,
            Anonymous =
            {
                mi = new MOUSEINPUT
                {
                    dx = x,
                    dy = y,
                    dwFlags = MOUSE_EVENT_FLAGS.MOUSEEVENTF_MOVE | MOUSE_EVENT_FLAGS.MOUSEEVENTF_MOVE_NOCOALESCE | MOUSE_EVENT_FLAGS.MOUSEEVENTF_ABSOLUTE,
                }
            },
        };
    }

    public static INPUT AbsoluteMouseMovementOnVirtualDesktop(int x, int y)
    {
        return new INPUT
        {
            type = INPUT_TYPE.INPUT_MOUSE,
            Anonymous =
            {
                mi = new MOUSEINPUT
                {
                    dx = x,
                    dy = y,
                    dwFlags = MOUSE_EVENT_FLAGS.MOUSEEVENTF_MOVE | MOUSE_EVENT_FLAGS.MOUSEEVENTF_MOVE_NOCOALESCE | MOUSE_EVENT_FLAGS.MOUSEEVENTF_ABSOLUTE | MOUSE_EVENT_FLAGS.MOUSEEVENTF_VIRTUALDESK,
                }
            },
        };
    }
}
