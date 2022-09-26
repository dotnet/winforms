﻿// Licensed to the .NET Foundation under one or more agreements.
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
                private HHOOK _messageHookHandle;
                private bool _isHooked;
                private HOOKPROC _callBack;

                public HostedWindowsFormsMessageHook()
                {
#if DEBUG
                    _callingStack = Environment.StackTrace;
#endif
                }

#if DEBUG
                private readonly string _callingStack;

                ~HostedWindowsFormsMessageHook()
                {
                    Debug.Assert(
                        _messageHookHandle == IntPtr.Zero,
                        $"Finalizing an active mouse hook. This will crash the process. Calling stack: {_callingStack}");
                }
#endif

                public bool HookMessages
                {
                    get => !_messageHookHandle.IsNull;
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

                private unsafe void InstallMessageHook()
                {
                    lock (this)
                    {
                        if (!_messageHookHandle.IsNull)
                        {
                            return;
                        }

                        _callBack = MessageHookProc;
                        var hook = Marshal.GetFunctionPointerForDelegate(_callBack);
                        _messageHookHandle = PInvoke.SetWindowsHookEx(
                            WINDOWS_HOOK_ID.WH_GETMESSAGE,
                            (delegate* unmanaged[Stdcall]<int, WPARAM, LPARAM, LRESULT>)hook,
                            (HINSTANCE)0,
                            PInvoke.GetCurrentThreadId());

                        if (_messageHookHandle != IntPtr.Zero)
                        {
                            _isHooked = true;
                        }

                        Debug.Assert(_messageHookHandle != IntPtr.Zero, "Failed to install mouse hook");
                    }
                }

                private unsafe LRESULT MessageHookProc(int nCode, WPARAM wparam, LPARAM lparam)
                {
                    if (nCode == PInvoke.HC_ACTION && _isHooked
                        && (PEEK_MESSAGE_REMOVE_TYPE)(nuint)wparam == PEEK_MESSAGE_REMOVE_TYPE.PM_REMOVE)
                    {
                        // Only process messages we've pulled off the queue.
                        MSG* msg = (MSG*)(nint)lparam;
                        if (msg is not null)
                        {
                            // Call pretranslate on the message to execute the message filters and preprocess message.
                            if (Application.ThreadContext.FromCurrent().PreTranslateMessage(ref *msg))
                            {
                                msg->message = (uint)User32.WM.NULL;
                            }
                        }
                    }

                    return PInvoke.CallNextHookEx(_messageHookHandle, nCode, wparam, lparam);
                }

                private void UninstallMessageHook()
                {
                    lock (this)
                    {
                        if (!_messageHookHandle.IsNull)
                        {
                            PInvoke.UnhookWindowsHookEx(_messageHookHandle);
                            _messageHookHandle = default;
                            _isHooked = false;
                        }
                    }
                }
            }
        }
    }
}
