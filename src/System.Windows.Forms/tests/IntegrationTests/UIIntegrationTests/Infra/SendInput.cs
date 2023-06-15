// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
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
            foreach (var key in keys)
            {
                switch (key)
                {
                    case string str:
                        var text = str.Replace("\r\n", "\r").Replace("\n", "\r");
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

                                inputSimulator.Keyboard.TextEntry(text.Substring(index, nextIndex - index));
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
        if (actions is null)
        {
            throw new ArgumentNullException(nameof(actions));
        }

        IHandle<HWND> handle = window;
        if (!TrySetForegroundWindow(handle.Handle))
        {
            throw new InvalidOperationException("Failed to set the foreground window.");
        }

        // Ensure the window is 'Active' as it may not have been achieved by 'SetForegroundWindow'
        PInvoke.SetActiveWindow(window);

        // Give the window the keyboard focus as it may not have been achieved by 'SetActiveWindow'
        PInvoke.SetFocus(window);

        await Task.Run(() => actions(new InputSimulator()));

        await _waitForIdleAsync();
    }

    private static HWND GetForegroundWindow()
    {
        var startTime = DateTime.Now;

        // Attempt to get the foreground window in a loop, as the NativeMethods function can return IntPtr.Zero
        // in certain circumstances, such as when a window is losing activation.
        HWND foregroundWindow;
        do
        {
            foregroundWindow = PInvoke.GetForegroundWindow();
        }
        while (foregroundWindow == IntPtr.Zero
            && DateTime.Now - startTime < TimeSpan.FromMilliseconds(500));

        return foregroundWindow;
    }

    private static bool TrySetForegroundWindow(HWND window)
    {
        var activeWindow = PInvoke.GetLastActivePopup(window);
        activeWindow = PInvoke.IsWindowVisible(activeWindow) ? activeWindow : window;
        // 'fUnknown: true' indicates that the window is being switched to using the Alt/Ctl+Tab key sequence
        PInvoke.SwitchToThisWindow(activeWindow, fUnknown: true);

        if (!PInvoke.SetForegroundWindow(activeWindow))
        {
            if (!PInvoke.AllocConsole())
            {
                Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
            }

            try
            {
                var consoleWindow = PInvoke.GetConsoleWindow();
                if (consoleWindow == IntPtr.Zero)
                {
                    throw new InvalidOperationException("Failed to obtain the console window.");
                }

                if (!PInvoke.SetWindowPos(consoleWindow, hWndInsertAfter: (HWND)0, 0, 0, 0, 0, SET_WINDOW_POS_FLAGS.SWP_NOZORDER))
                {
                    Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
                }
            }
            finally
            {
                if (!PInvoke.FreeConsole())
                {
                    Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
                }
            }

            if (!PInvoke.SetForegroundWindow(activeWindow))
            {
                return false;
            }
        }

        return true;
    }

    private static void SetForegroundWindow(Form window)
    {
        // Make the window a top-most window so it will appear above any existing top-most windows
        PInvoke.SetWindowPos(window, HWND.HWND_TOPMOST, 0, 0, 0, 0, SET_WINDOW_POS_FLAGS.SWP_NOSIZE | SET_WINDOW_POS_FLAGS.SWP_NOMOVE);

        // Move the window into the foreground as it may not have been achieved by the 'SetWindowPos' call
        if (!PInvoke.SetForegroundWindow(window))
        {
            string windowTitle = PInvoke.GetWindowText(window);
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
