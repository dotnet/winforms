// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using static Interop;

namespace System.Windows.Forms.PropertyGridInternal
{
    internal partial class PropertyGridView
    {
        internal partial class MouseHook
        {
            private readonly PropertyGridView _gridView;
            private readonly Control _control;
            private readonly IMouseHookClient _client;

            private uint _thisProcessId;
            private GCHandle _mouseHookRoot;
            private IntPtr _mouseHookHandle = IntPtr.Zero;
            private bool _hookDisable;

            private bool _processing;

            public MouseHook(Control control, IMouseHookClient client, PropertyGridView gridView)
            {
                _control = control;
                _gridView = gridView;
                _client = client;
#if DEBUG
                _callingStack = Environment.StackTrace;
#endif
            }

#if DEBUG
            private readonly string _callingStack;
            ~MouseHook()
            {
                if (_mouseHookHandle != IntPtr.Zero)
                {
                    throw new InvalidOperationException($"Finalizing an active mouse hook. This will crash the process. Calling stack: {_callingStack}");
                }
            }
#endif

            public bool DisableMouseHook
            {
                set
                {
                    _hookDisable = value;
                    if (value)
                    {
                        UnhookMouse();
                    }
                }
            }

            public virtual bool HookMouseDown
            {
                get
                {
                    GC.KeepAlive(this);
                    return _mouseHookHandle != IntPtr.Zero;
                }
                set
                {
                    if (value && !_hookDisable)
                    {
                        HookMouse();
                    }
                    else
                    {
                        UnhookMouse();
                    }
                }
            }

            public void Dispose() => UnhookMouse();

            /// <summary>
            ///  Sets up the needed windows hooks to catch messages.
            /// </summary>
            private void HookMouse()
            {
                GC.KeepAlive(this);

                // Locking 'this' here is ok since this is an internal class.
                lock (this)
                {
                    if (_mouseHookHandle != IntPtr.Zero)
                    {
                        return;
                    }

                    if (_thisProcessId == 0)
                    {
                        User32.GetWindowThreadProcessId(_control, out _thisProcessId);
                    }

                    var hook = new User32.HOOKPROC(new MouseHookObject(this).Callback);
                    _mouseHookRoot = GCHandle.Alloc(hook);
                    _mouseHookHandle = User32.SetWindowsHookExW(
                        User32.WH.MOUSE,
                        hook,
                        IntPtr.Zero,
                        Kernel32.GetCurrentThreadId());
                    Debug.Assert(_mouseHookHandle != IntPtr.Zero, "Failed to install mouse hook");
                    Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, "DropDownHolder:HookMouse()");
                }
            }

            /// <summary>
            ///  HookProc used for catch mouse messages.
            /// </summary>
            private unsafe nint MouseHookProc(User32.HC nCode, nint wparam, nint lparam)
            {
                if (nCode == User32.HC.ACTION)
                {
                    var mhs = (User32.MOUSEHOOKSTRUCT*)lparam;
                    if (mhs is not null)
                    {
                        switch ((User32.WM)wparam)
                        {
                            case User32.WM.LBUTTONDOWN:
                            case User32.WM.MBUTTONDOWN:
                            case User32.WM.RBUTTONDOWN:
                            case User32.WM.NCLBUTTONDOWN:
                            case User32.WM.NCMBUTTONDOWN:
                            case User32.WM.NCRBUTTONDOWN:
                            case User32.WM.MOUSEACTIVATE:
                                if (ProcessMouseDown(mhs->hWnd))
                                {
                                    return 1;
                                }

                                break;
                        }
                    }
                }

                return User32.CallNextHookEx(_mouseHookHandle, nCode, wparam, lparam);
            }

            /// <summary>
            ///  Removes the windowshook that was installed.
            /// </summary>
            private void UnhookMouse()
            {
                GC.KeepAlive(this);

                // Locking 'this' here is ok since this is an internal class.
                lock (this)
                {
                    if (_mouseHookHandle != IntPtr.Zero)
                    {
                        User32.UnhookWindowsHookEx(new HandleRef(this, _mouseHookHandle));
                        _mouseHookRoot.Free();
                        _mouseHookHandle = IntPtr.Zero;
                        Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, "DropDownHolder:UnhookMouse()");
                    }
                }
            }

            private bool ProcessMouseDown(IntPtr hwnd)
            {
                // If we put up the "invalid" message box, it appears this method is getting called reentrantly
                // when it shouldn't be. This prevents us from recursing.
                if (_processing)
                {
                    return false;
                }

                IntPtr handle = _control.HandleInternal;

                // If it is us or one of our children just process as normal.
                if (hwnd != handle
                    && FromHandle(hwnd) is Control targetControl
                    && !_control.Contains(targetControl))
                {
                    Debug.Assert(_thisProcessId != 0, "Didn't get our process id!");

                    // Make sure the window is in our process.
                    User32.GetWindowThreadProcessId(hwnd, out uint pid);

                    // If this isn't our process, unhook the mouse.
                    if (pid != _thisProcessId)
                    {
                        HookMouseDown = false;
                        return false;
                    }

                    // If this a sibling control (e.g. the drop down or buttons), just forward the message and skip the commit
                    bool needCommit = targetControl is null || !_gridView.IsSiblingControl(_control, targetControl);

                    try
                    {
                        _processing = true;

                        if (needCommit && _client.OnClickHooked())
                        {
                            return true; // there was an error, so eat the mouse
                        }
                    }
                    finally
                    {
                        _processing = false;
                    }

                    // Cancel our hook at this point.
                    HookMouseDown = false;
                }

                return false;
            }
        }
    }
}
