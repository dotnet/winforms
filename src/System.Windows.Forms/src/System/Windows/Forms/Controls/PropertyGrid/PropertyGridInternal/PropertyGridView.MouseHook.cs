// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;

namespace System.Windows.Forms.PropertyGridInternal;

internal partial class PropertyGridView
{
    internal partial class MouseHook
    {
        private readonly PropertyGridView _gridView;
        private readonly Control _control;
        private readonly IMouseHookClient _client;

        private uint _thisProcessId;
        private HOOKPROC? _callBack;
        private HHOOK _mouseHookHandle;
        private bool _hookDisable;

        private bool _processing;

        private readonly Lock _lock = new();

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
            if (!_mouseHookHandle.IsNull)
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
            get => !_mouseHookHandle.IsNull;
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
        private unsafe void HookMouse()
        {
            lock (_lock)
            {
                if (!_mouseHookHandle.IsNull)
                {
                    return;
                }

                if (_thisProcessId == 0)
                {
                    PInvoke.GetWindowThreadProcessId(_control, out _thisProcessId);
                }

                _callBack = MouseHookProc;
                IntPtr hook = Marshal.GetFunctionPointerForDelegate(_callBack);
                _mouseHookHandle = PInvoke.SetWindowsHookEx(
                    WINDOWS_HOOK_ID.WH_MOUSE,
                    (delegate* unmanaged[Stdcall]<int, WPARAM, LPARAM, LRESULT>)hook,
                    HINSTANCE.Null,
                    PInvokeCore.GetCurrentThreadId());

                Debug.Assert(!_mouseHookHandle.IsNull, "Failed to install mouse hook");
            }
        }

        /// <summary>
        ///  HookProc used for catch mouse messages.
        /// </summary>
        private unsafe LRESULT MouseHookProc(int nCode, WPARAM wparam, LPARAM lparam)
        {
            if (nCode == PInvoke.HC_ACTION)
            {
                var mhs = (MOUSEHOOKSTRUCT*)(nint)lparam;
                if (mhs is not null)
                {
                    switch ((uint)wparam)
                    {
                        case PInvokeCore.WM_LBUTTONDOWN:
                        case PInvokeCore.WM_MBUTTONDOWN:
                        case PInvokeCore.WM_RBUTTONDOWN:
                        case PInvokeCore.WM_NCLBUTTONDOWN:
                        case PInvokeCore.WM_NCMBUTTONDOWN:
                        case PInvokeCore.WM_NCRBUTTONDOWN:
                        case PInvokeCore.WM_MOUSEACTIVATE:
                            if (ProcessMouseDown(mhs->hwnd))
                            {
                                return (LRESULT)1;
                            }

                            break;
                    }
                }
            }

            return PInvoke.CallNextHookEx(_mouseHookHandle, nCode, wparam, lparam);
        }

        /// <summary>
        ///  Removes the windowshook that was installed.
        /// </summary>
        private void UnhookMouse()
        {
            lock (_lock)
            {
                if (!_mouseHookHandle.IsNull)
                {
                    PInvoke.UnhookWindowsHookEx(_mouseHookHandle);
                    _mouseHookHandle = default;
                }
            }
        }

        private bool ProcessMouseDown(HWND hwnd)
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
                PInvoke.GetWindowThreadProcessId(hwnd, out uint pid);

                // If this isn't our process, unhook the mouse.
                if (pid != _thisProcessId)
                {
                    HookMouseDown = false;
                    return false;
                }

                // If this a sibling control (e.g. the drop down or buttons), just forward the message and skip the commit
                bool needCommit = targetControl is null || !IsSiblingControl(_control, targetControl);

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
