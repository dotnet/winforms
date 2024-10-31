// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Windows.Forms.UITests.Input;
using Windows.Win32.UI.Input.KeyboardAndMouse;
using Windows.Win32.UI.WindowsAndMessaging;

namespace System.Windows.Forms.UITests;

public class SendInput
{
    private readonly Func<Task> _waitForIdleAsync;

    public SendInput(Func<Task> waitForIdleAsync)
    {
        _waitForIdleAsync = waitForIdleAsync;
    }

    internal async Task SendAsync(Form window, params object[] keys)
    {
        await SendAsync(window, inputSimulator =>
        {
            foreach (object key in keys)
            {
                switch (key)
                {
                    case string str:
                        string text = str.Replace("\r\n", "\r").Replace("\n", "\r");
                        int index = 0;
                        while (index < text.Length)
                        {
                            if (text[index] == '\r')
                            {
                                inputSimulator.Keyboard.KeyPress(VIRTUAL_KEY.VK_RETURN);
                                index++;
                            }
                            else
                            {
                                int nextIndex = text.IndexOf('\r', index);
                                if (nextIndex == -1)
                                {
                                    nextIndex = text.Length;
                                }

                                inputSimulator.Keyboard.TextEntry(text[index..nextIndex]);
                                index = nextIndex;
                            }
                        }

                        break;

                    case char c:
                        inputSimulator.Keyboard.TextEntry(c);
                        break;

                    case VIRTUAL_KEY virtualKeyCode:
                        inputSimulator.Keyboard.KeyPress(virtualKeyCode);
                        break;

                    case null:
                        throw new ArgumentNullException(nameof(keys));

                    default:
                        throw new ArgumentException($"Unexpected type encountered: {key.GetType()}", nameof(keys));
                }
            }
        });
    }

    internal async Task SendAsync(Form window, Action<InputSimulator> actions)
    {
        ArgumentNullException.ThrowIfNull(actions);

        SetForegroundWindow(window);
        await Task.Run(() => actions(new InputSimulator()));

        await _waitForIdleAsync();
    }

    private static void SetForegroundWindow(Form window)
    {
        // Make the window a top-most window so it will appear above any existing top-most windows
        PInvoke.SetWindowPos(window, HWND.HWND_TOPMOST, 0, 0, 0, 0, SET_WINDOW_POS_FLAGS.SWP_NOSIZE | SET_WINDOW_POS_FLAGS.SWP_NOMOVE);

        // Move the window into the foreground as it may not have been achieved by the 'SetWindowPos' call
        if (!PInvoke.SetForegroundWindow(window))
        {
            string windowTitle = PInvokeCore.GetWindowText(window);
            if (PInvoke.GetWindowThreadProcessId(window, out uint processId) == 0 || processId != Environment.ProcessId)
            {
                string message = $"ForegroundWindow doesn't belong the test process! The current window HWND: {window}, title:{windowTitle}.";
                throw new InvalidOperationException(message);
            }
        }

        // Ensure the window is 'Active' as it may not have been achieved by 'SetForegroundWindow'
        PInvoke.SetActiveWindow(window);

        // Give the window the keyboard focus as it may not have been achieved by 'SetActiveWindow'
        PInvoke.SetFocus(window);

        // Remove the 'Top-Most' qualification from the window
        PInvoke.SetWindowPos(window, HWND.HWND_NOTOPMOST, 0, 0, 0, 0, SET_WINDOW_POS_FLAGS.SWP_NOSIZE | SET_WINDOW_POS_FLAGS.SWP_NOMOVE);
    }
}
