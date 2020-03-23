// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Diagnostics;
using System.Runtime.InteropServices;
using static Interop;

namespace System.Windows.Forms
{
    public static partial class ToolStripManager
    {
        internal partial class ModalMenuFilter
        {
            private class HostedWindowsFormsMessageHook
            {
                private IntPtr _messageHookHandle;
                private bool _isHooked;
                private User32.HOOKPROC _hookProc;

                public HostedWindowsFormsMessageHook()
                {
#if DEBUG
                    try
                    {
                        _callingStack = Environment.StackTrace;
                    }
                    catch (Security.SecurityException)
                    {
                    }
#endif
                }

#if DEBUG
                private readonly string _callingStack;

                ~HostedWindowsFormsMessageHook()
                {
                    Debug.Assert(_messageHookHandle == IntPtr.Zero, "Finalizing an active mouse hook.  This will crash the process.  Calling stack: " + _callingStack);
                }
#endif

                public bool HookMessages
                {
                    get => _messageHookHandle != IntPtr.Zero;
                    set
                    {
                        if (value)
                        {
                            InstallMessageHook();
                        }
                        else
                        {
                            UninstallMessageHook();
                        }
                    }
                }

                private void InstallMessageHook()
                {
                    lock (this)
                    {
                        if (_messageHookHandle != IntPtr.Zero)
                        {
                            return;
                        }

                        _hookProc = new User32.HOOKPROC(MessageHookProc);
                        _messageHookHandle = User32.SetWindowsHookExW(
                            User32.WH.GETMESSAGE,
                            _hookProc,
                            IntPtr.Zero,
                            Kernel32.GetCurrentThreadId());

                        if (_messageHookHandle != IntPtr.Zero)
                        {
                            _isHooked = true;
                        }

                        Debug.Assert(_messageHookHandle != IntPtr.Zero, "Failed to install mouse hook");
                    }
                }

                private unsafe IntPtr MessageHookProc(User32.HC nCode, IntPtr wparam, IntPtr lparam)
                {
                    if (nCode == User32.HC.ACTION)
                    {
                        if (_isHooked && (User32.PM)wparam == User32.PM.REMOVE)
                        {
                            // only process messages we've pulled off the queue
                            User32.MSG* msg = (User32.MSG*)lparam;
                            if (msg != null)
                            {
                                // call pretranslate on the message - this should execute
                                // the message filters and preprocess message.
                                if (Application.ThreadContext.FromCurrent().PreTranslateMessage(ref *msg))
                                {
                                    msg->message = User32.WM.NULL;
                                }
                            }
                        }
                    }

                    return User32.CallNextHookEx(new HandleRef(this, _messageHookHandle), nCode, wparam, lparam);
                }

                private void UninstallMessageHook()
                {
                    lock (this)
                    {
                        if (_messageHookHandle != IntPtr.Zero)
                        {
                            User32.UnhookWindowsHookEx(new HandleRef(this, _messageHookHandle));
                            _hookProc = null;
                            _messageHookHandle = IntPtr.Zero;
                            _isHooked = false;
                        }
                    }
                }
            }
        }
    }
}
