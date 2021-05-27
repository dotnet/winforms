// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using static Interop;

namespace System.Windows.Forms.Design.Behavior
{
    public sealed partial class BehaviorService
    {
        private partial class AdornerWindow
        {
            /// <summary>
            ///  This class knows how to hook all the messages to a given process/thread.
            ///
            ///  On any mouse clicks, it asks the designer what to do with the message, that is to eat it or propagate
            ///  it to the control it was meant for.   This allows us to synchronously process mouse messages when
            ///  the AdornerWindow itself may be pumping messages.
            /// </summary>
            private class MouseHook
            {
                private AdornerWindow _currentAdornerWindow;
                private uint _thisProcessID;
                private GCHandle _mouseHookRoot;
                private IntPtr _mouseHookHandle = IntPtr.Zero;
                private bool _processingMessage;

                private bool _isHooked; //VSWHIDBEY # 474112
                private int _lastLButtonDownTimeStamp;

                public MouseHook()
                {
#if DEBUG
                    _callingStack = Environment.StackTrace;
#endif
                    HookMouse();
                }

#if DEBUG
                readonly string _callingStack;
                ~MouseHook()
                {
                    Debug.Assert(
                        _mouseHookHandle == IntPtr.Zero,
                        "Finalizing an active mouse hook.  This will crash the process.  Calling stack: " + _callingStack);
                }
#endif

                public void Dispose()
                {
                    UnhookMouse();
                }

                private void HookMouse()
                {
                    Debug.Assert(s_adornerWindowList.Count > 0, "No AdornerWindow available to create the mouse hook");
                    lock (this)
                    {
                        if (_mouseHookHandle != IntPtr.Zero || s_adornerWindowList.Count == 0)
                        {
                            return;
                        }

                        if (_thisProcessID == 0)
                        {
                            AdornerWindow adornerWindow = s_adornerWindowList[0];
                            User32.GetWindowThreadProcessId(adornerWindow, out _thisProcessID);
                        }

                        var hook = new User32.HOOKPROC(MouseHookProc);
                        _mouseHookRoot = GCHandle.Alloc(hook);
                        _mouseHookHandle = User32.SetWindowsHookExW(
                            User32.WH.MOUSE,
                            hook,
                            IntPtr.Zero,
                            Kernel32.GetCurrentThreadId());

                        if (_mouseHookHandle != IntPtr.Zero)
                        {
                            _isHooked = true;
                        }

                        Debug.Assert(_mouseHookHandle != IntPtr.Zero, "Failed to install mouse hook");
                    }
                }

                private unsafe IntPtr MouseHookProc(User32.HC nCode, IntPtr wparam, IntPtr lparam)
                {
                    if (_isHooked && nCode == User32.HC.ACTION && lparam != IntPtr.Zero)
                    {
                        User32.MOUSEHOOKSTRUCT* mhs = (User32.MOUSEHOOKSTRUCT*)lparam;

                        try
                        {
                            if (ProcessMouseMessage(mhs->hWnd, unchecked((int)(long)wparam), mhs->pt.X, mhs->pt.Y))
                            {
                                return (IntPtr)1;
                            }
                        }
                        catch (Exception ex)
                        {
                            _currentAdornerWindow.Capture = false;

                            if (ex != CheckoutException.Canceled)
                            {
                                _currentAdornerWindow._behaviorService.ShowError(ex);
                            }

                            if (ClientUtils.IsCriticalException(ex))
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
                    return User32.CallNextHookEx(new HandleRef(this, _mouseHookHandle), nCode, wparam, lparam);
                }

                private void UnhookMouse()
                {
                    lock (this)
                    {
                        if (_mouseHookHandle != IntPtr.Zero)
                        {
                            User32.UnhookWindowsHookEx(new HandleRef(this, _mouseHookHandle));
                            _mouseHookRoot.Free();
                            _mouseHookHandle = IntPtr.Zero;
                            _isHooked = false;
                        }
                    }
                }

                private bool ProcessMouseMessage(IntPtr hWnd, int msg, int x, int y)
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
                        IntPtr handle = adornerWindow.DesignerFrame.Handle;

                        // If it's us or one of our children, just process as normal
                        if (adornerWindow.ProcessingDrag
                            || (hWnd != handle && User32.IsChild(new HandleRef(this, handle), hWnd).IsTrue()))
                        {
                            Debug.Assert(_thisProcessID != 0, "Didn't get our process id!");

                            // Make sure the window is in our process
                            User32.GetWindowThreadProcessId(hWnd, out uint pid);

                            // If this isn't our process, bail
                            if (pid != _thisProcessID)
                            {
                                return false;
                            }

                            try
                            {
                                _processingMessage = true;
                                var pt = new Point(x, y);
                                User32.MapWindowPoints(IntPtr.Zero, adornerWindow.Handle, ref pt, 1);
                                Message m = Message.Create(hWnd, msg, (IntPtr)0, PARAM.FromLowHigh(pt.Y, pt.X));

                                // No one knows why we get an extra click here from VS. As a workaround, we check the TimeStamp and discard it.
                                if (m.Msg == (int)User32.WM.LBUTTONDOWN)
                                {
                                    _lastLButtonDownTimeStamp = User32.GetMessageTime();
                                }
                                else if (m.Msg == (int)User32.WM.LBUTTONDBLCLK)
                                {
                                    int lButtonDoubleClickTimeStamp = User32.GetMessageTime();
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
}
