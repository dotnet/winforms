// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

public sealed partial class Application
{
    /// <summary>
    ///  This class enables or disables all windows in the current thread. We use this to disable other windows on the
    ///  thread when a modal dialog is to be shown. It can also be used to dispose all windows in a thread, which we do
    ///  before returning from a message loop.
    /// </summary>
    private sealed class ThreadWindows
    {
        private readonly List<HWND> _windows;
        private HWND _activeHwnd;
        private HWND _focusedHwnd;
        internal ThreadWindows? _previousThreadWindows;
        private readonly bool _onlyWinForms = true;

        internal ThreadWindows(bool onlyWinForms)
        {
            _windows = new List<HWND>(16);
            _onlyWinForms = onlyWinForms;
            PInvokeCore.EnumCurrentThreadWindows(Callback);
        }

        private BOOL Callback(HWND hwnd)
        {
            // We only do visible and enabled windows. Also, we only do top level windows.
            // Finally, we only include windows that are DNA windows, since other MSO components
            // will be responsible for disabling their own windows.
            if (PInvoke.IsWindowVisible(hwnd) && PInvoke.IsWindowEnabled(hwnd))
            {
                if (!_onlyWinForms || Control.FromHandle(hwnd) is not null)
                {
                    _windows.Add(hwnd);
                }
            }

            return true;
        }

        // Disposes all top-level Controls on this thread
        internal void Dispose()
        {
            foreach (HWND hwnd in _windows)
            {
                if (PInvoke.IsWindow(hwnd))
                {
                    Control.FromHandle(hwnd)?.Dispose();
                }
            }
        }

        // Enables/disables all top-level Controls on this thread
        internal void Enable(bool enable)
        {
            if (!_onlyWinForms && !enable)
            {
                _activeHwnd = PInvoke.GetActiveWindow();
                Control? activatingControl = ThreadContext.FromCurrent().ActivatingControl;
                _focusedHwnd = activatingControl is not null ? activatingControl.HWND : PInvoke.GetFocus();
            }

            foreach (HWND hwnd in _windows)
            {
                if (PInvoke.IsWindow(hwnd))
                {
                    PInvoke.EnableWindow(hwnd, enable);
                }
            }

            // OpenFileDialog is not returning the focus the way other dialogs do.
            // Important that we re-activate the old window when we are closing
            // our modal dialog.
            //
            // edit mode forever with Excel application
            // But, DON'T change other people's state when we're simply
            // responding to external MSOCM events about modality. When we are,
            // we are created with a TRUE for onlyWinForms.
            if (!_onlyWinForms && enable)
            {
                if (!_activeHwnd.IsNull && PInvoke.IsWindow(_activeHwnd))
                {
                    PInvoke.SetActiveWindow(_activeHwnd);
                }

                if (!_focusedHwnd.IsNull && PInvoke.IsWindow(_focusedHwnd))
                {
                    PInvoke.SetFocus(_focusedHwnd);
                }
            }
        }
    }
}
