// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop;

namespace System.Windows.Forms
{
    internal class ListViewLabelEditNativeWindow : NativeWindow
    {
        private const uint TextSelectionChanged = 0x8014;

        private readonly ListView _owningListView;
        private AccessibleObject? _accessibilityObject;
        private User32.WINEVENTPROC? _winEventProcCallback;
        private nint _valueChangeHook;
        private nint _textSelectionChangedHook;
        private bool _winEventHooksInstalled;

        public ListViewLabelEditNativeWindow(ListView owningListView)
        {
            _owningListView = owningListView.OrThrowIfNull();
        }

        public AccessibleObject AccessibilityObject =>
            _accessibilityObject ??= new ListViewLabelEditAccessibleObject(_owningListView, this);

        private void InstallWinEventHooks()
        {
            if (!UiaCore.UiaClientsAreListening())
            {
                return;
            }

            _winEventProcCallback = new User32.WINEVENTPROC(WinEventProcCallback);
            _valueChangeHook = User32.SetWinEventHook((uint)AccessibleEvents.ValueChange, _winEventProcCallback);
            _textSelectionChangedHook = User32.SetWinEventHook(TextSelectionChanged, _winEventProcCallback);

            _winEventHooksInstalled = true;
        }

        private bool IsAccessibilityObjectCreated => _accessibilityObject is not null;

        public bool IsHandleCreated => Handle != IntPtr.Zero;

        protected override void OnHandleChange()
        {
            base.OnHandleChange();

            if (!IsHandleCreated)
            {
                return;
            }

            // Install winevent hooks *only* when the parent ListView has an existing accessible object.
            // If we don't install hooks at the label edit startup then assistive tech (e.g. Narrator) won't announce the text pattern for it.
            // By invoking UiaClientsAreListening we will have hooks installed even if the assistive tech isn't currently run.
            if (!_owningListView.IsAccessibilityObjectCreated)
            {
                return;
            }

            InstallWinEventHooks();
        }

        public override void ReleaseHandle()
        {
            if (_winEventHooksInstalled)
            {
                User32.UnhookWinEvent(_valueChangeHook);
                User32.UnhookWinEvent(_textSelectionChangedHook);

                _winEventHooksInstalled = false;
            }

            if (IsHandleCreated)
            {
                // When a window that previously returned providers has been destroyed,
                // you should notify UI Automation by calling the UiaReturnRawElementProvider
                // as follows: UiaReturnRawElementProvider(hwnd, 0, 0, NULL). This call tells
                // UI Automation that it can safely remove all map entries that refer to the specified window.
                UiaCore.UiaReturnRawElementProvider(Handle, wParam: 0, lParam: 0, el: null);
            }

            if (OsVersion.IsWindows8OrGreater())
            {
                UiaCore.UiaDisconnectProvider(_accessibilityObject);
            }

            _accessibilityObject = null;
            base.ReleaseHandle();
        }

        private void WinEventProcCallback(nint hWinEventHook, uint eventId, nint hwnd, int idObject, int idChild, uint idEventThread, uint dwmsEventTime)
        {
            if (hwnd != Handle || idObject != User32.OBJID.CLIENT || !IsAccessibilityObjectCreated)
            {
                return;
            }

            switch (eventId)
            {
                case (uint)AccessibleEvents.ValueChange:
                    AccessibilityObject.RaiseAutomationEvent(UiaCore.UIA.Text_TextChangedEventId);
                    break;
                case TextSelectionChanged:
                    AccessibilityObject.RaiseAutomationEvent(UiaCore.UIA.Text_TextSelectionChangedEventId);
                    break;
            }
        }

        private void WmGetObject(ref Message m)
        {
            AccessibleObject EnsureWinEventHooksInstalledAndGetAccessibilityObject()
            {
                AccessibleObject accessibilityObject = AccessibilityObject;

                // Accessibility object was likely requested by an assistive tech (which sent WM_GETOBJECT message).
                // We may need to install winevent hooks to produce the automation events related to the text pattern.
                if (!_winEventHooksInstalled)
                {
                    InstallWinEventHooks();
                }

                return accessibilityObject;
            }

            if (m.LParamInternal == NativeMethods.UiaRootObjectId)
            {
                // If the requested object identifier is UiaRootObjectId,
                // we should return an UI Automation provider using the UiaReturnRawElementProvider function.
                m.ResultInternal = (LRESULT)UiaCore.UiaReturnRawElementProvider(
                    this,
                    m.WParamInternal,
                    m.LParamInternal,
                    EnsureWinEventHooksInstalledAndGetAccessibilityObject());

                return;
            }

            if ((int)m.LParamInternal != User32.OBJID.CLIENT)
            {
                DefWndProc(ref m);

                return;
            }

            try
            {
                m.ResultInternal = EnsureWinEventHooksInstalledAndGetAccessibilityObject().GetLRESULT(m.WParamInternal);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(SR.RichControlLresult, ex);
            }
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.MsgInternal)
            {
                case User32.WM.GETOBJECT:
                    WmGetObject(ref m);
                    return;
                default:
                    base.WndProc(ref m);
                    return;
            }
        }
    }
}
