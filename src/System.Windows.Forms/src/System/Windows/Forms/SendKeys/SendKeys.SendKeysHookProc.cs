// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.Windows.Forms;

public partial class SendKeys
{
    /// <summary>
    ///  This class is our callback for the journaling hook we install.
    /// </summary>
    private static class SendKeysHookProc
    {
        // There appears to be a timing issue where setting and removing and then setting these hooks via
        // SetWindowsHookEx / UnhookWindowsHookEx can cause messages to be left in the queue and sent after the
        // re-hookup happens. This puts us in a bad state as we get an HC_SKIP before an HC_GETNEXT. So in that
        // case, we just ignore the HC_SKIP calls until we get an HC_GETNEXT. We also sleep a bit in the Unhook.

        private static bool s_gotNextEvent;

#pragma warning disable CS3016 // Arrays as attribute arguments is not CLS-compliant
        [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
#pragma warning restore CS3016
        public static unsafe LRESULT Callback(int nCode, WPARAM wparam, LPARAM lparam)
        {
            EVENTMSG* eventmsg = (EVENTMSG*)(nint)lparam;

            if (PInvoke.GetAsyncKeyState((int)Keys.Pause) != 0)
            {
                s_stopHook = true;
            }

            switch ((uint)nCode)
            {
                case PInvoke.HC_SKIP:
                    if (s_gotNextEvent)
                    {
                        if (s_events.Count > 0)
                        {
                            s_events.Dequeue();
                        }

                        s_stopHook = s_events.Count == 0;
                        break;
                    }

                    break;
                case PInvoke.HC_GETNEXT:
                    s_gotNextEvent = true;

                    Debug.Assert(
                        s_events.Count > 0 && !s_stopHook,
                        "HC_GETNEXT when queue is empty!");

                    SKEvent @event = s_events.Peek();
                    eventmsg->message = (uint)@event.WM;
                    eventmsg->paramL = @event.ParamL;
                    eventmsg->paramH = @event.ParamH;
                    eventmsg->hwnd = @event.HWND;
                    eventmsg->time = PInvoke.GetTickCount();
                    break;
                default:
                    if (nCode < 0)
                    {
                        PInvoke.CallNextHookEx(s_hhook, nCode, wparam, lparam);
                    }

                    break;
            }

            if (s_stopHook)
            {
                UninstallJournalingHook();
                s_gotNextEvent = false;
            }

            return (LRESULT)0;
        }
    }
}
