// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using WindowsInput;
using WindowsInput.Native;
using static Interop;

namespace System.Windows.Forms.UITests
{
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
                                    inputSimulator.Keyboard.KeyPress(VirtualKeyCode.RETURN);
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

                        case VirtualKeyCode virtualKeyCode:
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

            var foregroundWindow = IntPtr.Zero;

            try
            {
                var foreground = GetForegroundWindow();
                SetForegroundWindow(window.Handle);

                await Task.Run(() => actions(new InputSimulator()));
            }
            finally
            {
                if (foregroundWindow != IntPtr.Zero)
                {
                    SetForegroundWindow(foregroundWindow);
                }
            }

            await _waitForIdleAsync();
        }

        private static IntPtr GetForegroundWindow()
        {
            var startTime = DateTime.Now;

            // Attempt to get the foreground window in a loop, as the NativeMethods function can return IntPtr.Zero
            // in certain circumstances, such as when a window is losing activation.
            IntPtr foregroundWindow;
            do
            {
                foregroundWindow = User32.GetForegroundWindow();
            }
            while (foregroundWindow == IntPtr.Zero
                && DateTime.Now - startTime < TimeSpan.FromMilliseconds(500));

            return foregroundWindow;
        }

        private static void SetForegroundWindow(IntPtr window)
        {
            // Make the window a top-most window so it will appear above any existing top-most windows
            User32.SetWindowPos(window, User32.HWND_TOPMOST, 0, 0, 0, 0, User32.SWP.NOSIZE | User32.SWP.NOMOVE);

            // Move the window into the foreground as it may not have been achieved by the 'SetWindowPos' call
            if (User32.SetForegroundWindow(window).IsFalse())
            {
                string windowTitle = User32.GetWindowText(window);
                if (User32.GetWindowThreadProcessId(window, out uint processId) == 0 || processId != Environment.ProcessId)
                {
                    string message = $"ForegroundWindow doesn't belong the test process! The current window HWND: {window}, title:{windowTitle}.";
                    throw new InvalidOperationException(message);
                }
            }

            // Ensure the window is 'Active' as it may not have been achieved by 'SetForegroundWindow'
            User32.SetActiveWindow(new HandleRef(null, window));

            // Give the window the keyboard focus as it may not have been achieved by 'SetActiveWindow'
            User32.SetFocus(new HandleRef(null, window));

            // Remove the 'Top-Most' qualification from the window
            User32.SetWindowPos(window, User32.HWND_NOTOPMOST, 0, 0, 0, 0, User32.SWP.NOSIZE | User32.SWP.NOMOVE);
        }
    }
}
