// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms.Tests;

public partial class ControlTests
{
    [WinFormsFact]
    public void Control_AccessibilityNotifyClients_WithHandle_RaisesWinEvent()
    {
        using NoClientNotificationsScope scope = new(enable: false);
        using SubControl control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);

        using WinEventListener listener = new(AccessibleEvents.Focus);
        control.AccessibilityNotifyClients(AccessibleEvents.Focus, 2);

        WinEventListener.WinEvent winEvent = Assert.Single(listener.EventsOf(control));
        Assert.Equal((uint)AccessibleEvents.Focus, winEvent.Event);
        Assert.Equal((int)OBJECT_IDENTIFIER.OBJID_CLIENT, winEvent.ObjectId);

        // MSAA child ids are 1-based.
        Assert.Equal(3, winEvent.ChildId);
    }

    [WinFormsFact]
    public void Control_AccessibilityNotifyClients_ObjectId_WithHandle_RaisesWinEvent()
    {
        using NoClientNotificationsScope scope = new(enable: false);
        using SubControl control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);

        using WinEventListener listener = new(AccessibleEvents.SystemMenuStart);
        control.AccessibilityNotifyClients(AccessibleEvents.SystemMenuStart, (int)OBJECT_IDENTIFIER.OBJID_MENU, -1);

        WinEventListener.WinEvent winEvent = Assert.Single(listener.EventsOf(control));
        Assert.Equal((int)OBJECT_IDENTIFIER.OBJID_MENU, winEvent.ObjectId);
        Assert.Equal(0, winEvent.ChildId);
    }

    [WinFormsFact]
    public void Control_AccessibilityNotifyClients_NoClientNotifications_DoesNotRaiseWinEvent()
    {
        using NoClientNotificationsScope scope = new(enable: true);
        using SubControl control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);

        using WinEventListener listener = new(AccessibleEvents.Focus);
        control.AccessibilityNotifyClients(AccessibleEvents.Focus, 2);

        Assert.Empty(listener.EventsOf(control));
    }

    /// <summary>
    ///  Listens, on the current thread, to the WinEvents of one kind raised by this process.
    ///  The hook is in-context, so the callback runs synchronously inside NotifyWinEvent.
    /// </summary>
    private sealed unsafe class WinEventListener : IDisposable
    {
        public readonly record struct WinEvent(uint Event, HWND Hwnd, int ObjectId, int ChildId);

        // The hook is installed for the current thread only, and WinForms tests do not run in parallel.
        private static List<WinEvent>? s_events;

        private readonly HWINEVENTHOOK _hook;

        public WinEventListener(AccessibleEvents accEvent)
        {
            s_events = [];
            _hook = PInvoke.SetWinEventHook(
                (uint)accEvent,
                (uint)accEvent,
                PInvoke.GetModuleHandle((PCWSTR)null),
                &WinEventProc,
                (uint)Environment.ProcessId,
                PInvokeCore.GetCurrentThreadId(),
                PInvoke.WINEVENT_INCONTEXT);

            Assert.False(_hook.IsNull);
        }

        public IEnumerable<WinEvent> EventsOf(Control control)
        {
            HWND hwnd = control.HWND;
            return s_events!.Where(e => e.Hwnd == hwnd);
        }

        [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
        private static void WinEventProc(
            HWINEVENTHOOK hook,
            uint @event,
            HWND hwnd,
            int idObject,
            int idChild,
            uint idEventThread,
            uint dwmsEventTime) => s_events?.Add(new(@event, hwnd, idObject, idChild));

        public void Dispose()
        {
            PInvoke.UnhookWinEvent(_hook);
            s_events = null;
        }
    }
}
