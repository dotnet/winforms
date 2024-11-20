// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel.Design;
using System.Drawing;
using System.Runtime.InteropServices;

namespace System.Windows.Forms.Design.Behavior;

public sealed partial class BehaviorService
{
    private partial class AdornerWindow
    {
        /// <summary>
        ///  <para>
        ///   This class knows how to hook all the messages to a given process/thread.
        ///  </para>
        ///  <para>
        ///   On any mouse clicks, it asks the designer what to do with the message, that is to eat it or propagate
        ///   it to the control it was meant for. This allows us to synchronously process mouse messages when
        ///   the AdornerWindow itself may be pumping messages.
        ///  </para>
        /// </summary>
        private class MouseHook
        {
            private AdornerWindow? _currentAdornerWindow;
            private uint _thisProcessID;
            private HOOKPROC? _callBack;
            private HHOOK _mouseHookHandle;
            private bool _processingMessage;

            private bool _isHooked;
            private int _lastLButtonDownTimeStamp;
            private readonly Lock _lock = new();

            public MouseHook()
            {
#if DEBUG
                _callingStack = Environment.StackTrace;
#endif
                HookMouse();
            }

#if DEBUG
            private readonly string _callingStack;
#pragma warning disable CA1821 // Remove empty Finalizers
            ~MouseHook()
#pragma warning restore CA1821
            {
                Debug.Assert(
                    _mouseHookHandle == 0,
                    $"Finalizing an active mouse hook. This will crash the process. Calling stack: {_callingStack}");
            }
#endif

            public void Dispose()
            {
                UnhookMouse();
            }

            private unsafe void HookMouse()
            {
                Debug.Assert(s_adornerWindowList.Count > 0, "No AdornerWindow available to create the mouse hook");
                lock (_lock)
                {
                    if (_mouseHookHandle != 0 || s_adornerWindowList.Count == 0)
                    {
                        return;
                    }

                    if (_thisProcessID == 0)
                    {
                        AdornerWindow adornerWindow = s_adornerWindowList[0];
                        PInvoke.GetWindowThreadProcessId(adornerWindow, out _thisProcessID);
                    }

                    _callBack = MouseHookProc;
                    IntPtr hook = Marshal.GetFunctionPointerForDelegate(_callBack);
                    _mouseHookHandle = PInvoke.SetWindowsHookEx(
                        WINDOWS_HOOK_ID.WH_MOUSE,
                        (delegate* unmanaged[Stdcall]<int, WPARAM, LPARAM, LRESULT>)hook,
                        HINSTANCE.Null,
                        PInvokeCore.GetCurrentThreadId());

                    _isHooked = _mouseHookHandle != 0;

                    Debug.Assert(_isHooked, "Failed to install mouse hook.");
                }
            }

            private unsafe LRESULT MouseHookProc(int nCode, WPARAM wparam, LPARAM lparam)
            {
                if (_isHooked && nCode == PInvoke.HC_ACTION && lparam != 0)
                {
                    MOUSEHOOKSTRUCT* mhs = (MOUSEHOOKSTRUCT*)(nint)lparam;

                    try
                    {
                        if (ProcessMouseMessage(mhs->hwnd, (MessageId)(nuint)wparam, mhs->pt.X, mhs->pt.Y))
                        {
                            return (LRESULT)1;
                        }
                    }
                    catch (Exception ex)
                    {
                        _currentAdornerWindow!.Capture = false;

                        if (ex != CheckoutException.Canceled)
                        {
                            _currentAdornerWindow._behaviorService.ShowError(ex);
                        }

                        if (ex.IsCriticalException())
                        {
                            throw;
                        }
                    }
                    finally
                    {
                        _currentAdornerWindow = null;
                    }
                }

                Debug.Assert(_isHooked, "How did we get here when we are disposed?");
                return PInvoke.CallNextHookEx(_mouseHookHandle, nCode, wparam, lparam);
            }

            private void UnhookMouse()
            {
                lock (_lock)
                {
                    if (_mouseHookHandle != 0)
                    {
                        if (!PInvoke.UnhookWindowsHookEx(_mouseHookHandle))
                        {
                            Debug.Fail("Failed to remove mouse hook.");
                        }

                        _mouseHookHandle = default;
                        _isHooked = false;
                    }
                }
            }

            private bool ProcessMouseMessage(HWND hwnd, MessageId msg, int x, int y)
            {
                if (_processingMessage)
                {
                    return false;
                }

                foreach (AdornerWindow adornerWindow in s_adornerWindowList)
                {
                    if (!adornerWindow.DesignerFrameValid)
                    {
                        continue;
                    }

                    _currentAdornerWindow = adornerWindow;

                    // If it's us or one of our children, just process as normal
                    if (adornerWindow.ProcessingDrag
                        || (hwnd != adornerWindow.DesignerFrame.Handle && PInvoke.IsChild(adornerWindow.DesignerFrame, hwnd)))
                    {
                        Debug.Assert(_thisProcessID != 0, "Didn't get our process id!");

                        // Make sure the window is in our process
                        PInvoke.GetWindowThreadProcessId(hwnd, out uint pid);

                        // If this isn't our process, bail
                        if (pid != _thisProcessID)
                        {
                            return false;
                        }

                        try
                        {
                            _processingMessage = true;
                            Point pt = adornerWindow.PointToClient(new Point(x, y));
                            Message m = Message.Create(hwnd, msg, 0u, PARAM.FromLowHigh(pt.Y, pt.X));

                            // No one knows why we get an extra click here from VS. As a workaround, we check the TimeStamp and discard it.
                            if (m.Msg == (int)PInvokeCore.WM_LBUTTONDOWN)
                            {
                                _lastLButtonDownTimeStamp = PInvoke.GetMessageTime();
                            }
                            else if (m.Msg == (int)PInvokeCore.WM_LBUTTONDBLCLK)
                            {
                                int lButtonDoubleClickTimeStamp = PInvoke.GetMessageTime();
                                if (lButtonDoubleClickTimeStamp == _lastLButtonDownTimeStamp)
                                {
                                    return true;
                                }
                            }

                            if (!adornerWindow.WndProcProxy(ref m, pt.X, pt.Y))
                            {
                                // We did the work, stop the message propagation
                                return true;
                            }
                        }
                        finally
                        {
                            _processingMessage = false;
                        }

                        // No need to enumerate the other adorner windows since only one can be focused at a time.
                        break;
                    }
                }

                return false;
            }
        }
    }
}
