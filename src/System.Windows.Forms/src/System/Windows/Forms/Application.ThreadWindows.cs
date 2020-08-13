// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.ComponentModel;
using System.Diagnostics;
using static Interop;

namespace System.Windows.Forms
{
    public sealed partial class Application
    {
        /// <summary>
        ///  This class enables or disables all windows in the current thread.  We use this to
        ///  disable other windows on the thread when a modal dialog is to be shown.  It can also
        ///  be used to dispose all windows in a thread, which we do before returning from a message
        ///  loop.
        /// </summary>
        private sealed class ThreadWindows
        {
            private IntPtr[] _windows;
            private int _windowCount;
            private IntPtr _activeHwnd;
            private IntPtr _focusedHwnd;
            internal ThreadWindows _previousThreadWindows;
            private readonly bool _onlyWinForms = true;

            internal ThreadWindows(bool onlyWinForms)
            {
                _windows = new IntPtr[16];
                _onlyWinForms = onlyWinForms;
                User32.EnumThreadWindows(
                    Kernel32.GetCurrentThreadId(),
                    Callback);
            }

            private BOOL Callback(IntPtr hWnd)
            {
                // We only do visible and enabled windows.  Also, we only do top level windows.
                // Finally, we only include windows that are DNA windows, since other MSO components
                // will be responsible for disabling their own windows.
                if (User32.IsWindowVisible(hWnd).IsTrue() && User32.IsWindowEnabled(hWnd).IsTrue())
                {
                    bool add = true;

                    if (_onlyWinForms)
                    {
                        Control c = Control.FromHandle(hWnd);
                        if (c is null)
                        {
                            add = false;
                        }
                    }

                    if (add)
                    {
                        if (_windowCount == _windows.Length)
                        {
                            IntPtr[] newWindows = new IntPtr[_windowCount * 2];
                            Array.Copy(_windows, 0, newWindows, 0, _windowCount);
                            _windows = newWindows;
                        }
                        _windows[_windowCount++] = hWnd;
                    }
                }

                return BOOL.TRUE;
            }

            // Disposes all top-level Controls on this thread
            internal void Dispose()
            {
                for (int i = 0; i < _windowCount; i++)
                {
                    IntPtr hWnd = _windows[i];
                    if (User32.IsWindow(hWnd).IsTrue())
                    {
                        Control c = Control.FromHandle(hWnd);
                        if (c != null)
                        {
                            c.Dispose();
                        }
                    }
                }
            }

            // Enables/disables all top-level Controls on this thread
            internal void Enable(bool state)
            {
                if (!_onlyWinForms && !state)
                {
                    _activeHwnd = User32.GetActiveWindow();
                    Control activatingControl = ThreadContext.FromCurrent().ActivatingControl;
                    if (activatingControl != null)
                    {
                        _focusedHwnd = activatingControl.Handle;
                    }
                    else
                    {
                        _focusedHwnd = User32.GetFocus();
                    }
                }

                for (int i = 0; i < _windowCount; i++)
                {
                    IntPtr hWnd = _windows[i];
                    Debug.WriteLineIf(CompModSwitches.MSOComponentManager.TraceInfo, "ComponentManager : Changing enabled on window: " + hWnd.ToString() + " : " + state.ToString());
                    if (User32.IsWindow(hWnd).IsTrue())
                    {
                        User32.EnableWindow(hWnd, state.ToBOOL());
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
                    if (_activeHwnd != IntPtr.Zero && User32.IsWindow(_activeHwnd).IsTrue())
                    {
                        User32.SetActiveWindow(_activeHwnd);
                    }

                    if (_focusedHwnd != IntPtr.Zero && User32.IsWindow(_focusedHwnd).IsTrue())
                    {
                        User32.SetFocus(_focusedHwnd);
                    }
                }
            }
        }
    }
}
