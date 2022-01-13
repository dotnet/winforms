// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using static Interop;

namespace System.Windows.Forms
{
    public partial class SendKeys
    {
        /// <summary>
        ///  This class is our callback for the journaling hook we install.
        /// </summary>
        private class SendKeysHookProc
        {
            // There appears to be a timing issue where setting and removing and then setting these hooks via
            // SetWindowsHookEx / UnhookWindowsHookEx can cause messages to be left in the queue and sent after the
            // re-hookup happens. This puts us in a bad state as we get an HC_SKIP before an HC_GETNEXT. So in that
            // case, we just ignore the HC_SKIP calls until we get an HC_GETNEXT. We also sleep a bit in the Unhook.

            private bool _gotNextEvent;

            public unsafe virtual nint Callback(User32.HC nCode, nint wparam, nint lparam)
            {
                User32.EVENTMSG* eventmsg = (User32.EVENTMSG*)lparam;

                if (User32.GetAsyncKeyState((int)Keys.Pause) != 0)
                {
                    s_stopHook = true;
                }

                switch (nCode)
                {
                    case User32.HC.SKIP:
                        if (_gotNextEvent)
                        {
                            if (s_events.Count > 0)
                            {
                                s_events.Dequeue();
                            }

                            s_stopHook = s_events.Count == 0;
                            break;
                        }

                        break;
                    case User32.HC.GETNEXT:
                        _gotNextEvent = true;

                        Debug.Assert(
                            s_events.Count > 0 && !s_stopHook,
                            "HC_GETNEXT when queue is empty!");

                        SKEvent @event = s_events.Peek();
                        eventmsg->message = @event.WM;
                        eventmsg->paramL = @event.ParamL;
                        eventmsg->paramH = @event.ParamH;
                        eventmsg->hwnd = @event.HWND;
                        eventmsg->time = Kernel32.GetTickCount();
                        break;
                    default:
                        if (nCode < 0)
                        {
                            User32.CallNextHookEx(s_hhook, nCode, wparam, lparam);
                        }

                        break;
                }

                if (s_stopHook)
                {
                    UninstallJournalingHook();
                    _gotNextEvent = false;
                }

                return 0;
            }
        }
    }
}
