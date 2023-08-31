// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

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
        private HWND[] _windows;
        private int _windowCount;
        private HWND _activeHwnd;
        private HWND _focusedHwnd;
        internal ThreadWindows? _previousThreadWindows;
        private readonly bool _onlyWinForms = true;

        internal ThreadWindows(bool onlyWinForms)
        {
            _windows = new HWND[16];
            _onlyWinForms = onlyWinForms;
            PInvoke.EnumThreadWindows(
                PInvoke.GetCurrentThreadId(),
                Callback);
        }

        private BOOL Callback(HWND hWnd)
        {
            // We only do visible and enabled windows.  Also, we only do top level windows.
            // Finally, we only include windows that are DNA windows, since other MSO components
            // will be responsible for disabling their own windows.
            if (PInvoke.IsWindowVisible(hWnd) && PInvoke.IsWindowEnabled(hWnd))
            {
                bool add = true;

                if (_onlyWinForms)
                {
                    Control? c = Control.FromHandle(hWnd);
                    if (c is null)
                    {
                        add = false;
                    }
                }

                if (add)
                {
                    if (_windowCount == _windows.Length)
                    {
                        HWND[] newWindows = new HWND[_windowCount * 2];
                        Array.Copy(_windows, 0, newWindows, 0, _windowCount);
                        _windows = newWindows;
                    }

                    _windows[_windowCount++] = hWnd;
                }
            }

            return true;
        }

        // Disposes all top-level Controls on this thread
        internal void Dispose()
        {
            for (int i = 0; i < _windowCount; i++)
            {
                HWND hWnd = _windows[i];
                if (PInvoke.IsWindow(hWnd))
                {
                    Control? c = Control.FromHandle(hWnd);
                    c?.Dispose();
                }
            }
        }

        // Enables/disables all top-level Controls on this thread
        internal void Enable(bool state)
        {
            if (!_onlyWinForms && !state)
            {
                _activeHwnd = PInvoke.GetActiveWindow();
                Control? activatingControl = ThreadContext.FromCurrent().ActivatingControl;
                if (activatingControl is not null)
                {
                    _focusedHwnd = activatingControl.HWND;
                }
                else
                {
                    _focusedHwnd = PInvoke.GetFocus();
                }
            }

            for (int i = 0; i < _windowCount; i++)
            {
                HWND hWnd = _windows[i];
                Debug.WriteLineIf(
                    CompModSwitches.MSOComponentManager.TraceInfo,
                    $"ComponentManager : Changing enabled on window: {hWnd} : {state}");

                if (PInvoke.IsWindow(hWnd))
                {
                    PInvoke.EnableWindow(hWnd, state);
                }
            }

            // OpenFileDialog is not returning the focus the way other dialogs do.
            // Important that we re-activate the old window when we are closing
            // our modal dialog.
            //
            // edit mode forever with Excel application
            // But, DON'T change other people's state when we're simply
            // responding to external MSOCM events about modality.  When we are,
            // we are created with a TRUE for onlyWinForms.
            if (!_onlyWinForms && state)
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
