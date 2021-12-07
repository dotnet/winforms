// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using static Interop;

namespace System.Windows.Forms
{
    internal class ListViewLabelEditNativeWindow : NativeWindow
    {
        private const uint TextSelectionChanged = 0x8014;

        private ListView _owner;

        private AccessibleObject? _accessibilityObject;

        private User32.WINEVENTPROC? _winEventProc;

        private nint _valueChangeHook;
        private nint _textSelectionChangedHook;

        private bool _winEventHooksInstalled;

        public ListViewLabelEditNativeWindow(ListView owner)
        {
            _owner = owner.OrThrowIfNull();
        }

        public AccessibleObject AccessibilityObject => _accessibilityObject ??= new ListViewLabelEditAccessibleObject(_owner, this);

        public override void ReleaseHandle()
        {
            if (_winEventHooksInstalled)
            {
                User32.UnhookWinEvent(_valueChangeHook);
                User32.UnhookWinEvent(_textSelectionChangedHook);

                _winEventHooksInstalled = false;
            }

            if (_accessibilityObject is not null)
            {
                // When a window that previously returned providers has been destroyed,
                // you should notify UI Automation by calling the UiaReturnRawElementProvider
                // as follows: UiaReturnRawElementProvider(hwnd, 0, 0, NULL). This call tells
                // UI Automation that it can safely remove all map entries that refer to the specified window.
                UiaCore.UiaReturnRawElementProvider(Handle, 0, 0, null);

                if (OsVersion.IsWindows8OrGreater)
                {
                    UiaCore.UiaDisconnectProvider(AccessibilityObject);
                }
            }

            base.ReleaseHandle();
        }

        protected override void OnHandleChange()
        {
            base.OnHandleChange();

            if (Handle == IntPtr.Zero)
            {
                return;
            }

            // Install winevent hooks only in the case when the parent listview already has an accessible object created.
            // If we won't install hooks at the label edit startup then Narrator won't announce the text pattern for it.
            // If we will just check for UiaClientsAreListening then we will have hooks installed even if Narrator isn't currently run.
            if (!_owner.IsAccessibilityObjectCreated)
            {
                return;
            }

            InstallWinEventHooks();
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

        private void InstallWinEventHooks()
        {
            if (UiaCore.UiaClientsAreListening().IsFalse())
            {
                return;
            }

            _winEventProc = new User32.WINEVENTPROC(WinEventProc);
            _valueChangeHook = User32.SetWinEventHook((uint)AccessibleEvents.ValueChange, _winEventProc);
            _textSelectionChangedHook = User32.SetWinEventHook(TextSelectionChanged, _winEventProc);

            _winEventHooksInstalled = true;
        }

        private AccessibleObject RequestAccessibilityObject()
        {
            AccessibleObject result = AccessibilityObject;

            // Accessibility object was likely requested by some accessibility tool (because of WM_GETOBJECT message).
            // The tool that requested the object may be Narrator in which case we may need to install winevent hooks to
            // produce the automation events related to the text pattern.
            if (!_winEventHooksInstalled)
            {
                InstallWinEventHooks();
            }

            return result;
        }

        private void WinEventProc(nint hWinEventHook, uint eventId, nint hwnd, int idObject, int idChild, uint idEventThread, uint dwmsEventTime)
        {
            if (hwnd != Handle || idObject != User32.OBJID.CLIENT)
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
            if (m.LParamInternal == NativeMethods.UiaRootObjectId)
            {
                // If the requested object identifier is UiaRootObjectId,
                // we should return an UI Automation provider using the UiaReturnRawElementProvider function.
                m.ResultInternal = UiaCore.UiaReturnRawElementProvider(
                    this,
                    m.WParamInternal,
                    m.LParamInternal,
                    RequestAccessibilityObject());

                return;
            }

            if ((int)m.LParamInternal != User32.OBJID.CLIENT)
            {
                DefWndProc(ref m);

                return;
            }

            // https://docs.microsoft.com/windows/win32/winauto/how-to-handle-wm-getobject
            // Get an Lresult for the accessibility Object for this control.
            try
            {
                // Obtain the Lresult.
                IntPtr pUnknown = Marshal.GetIUnknownForObject(RequestAccessibilityObject());

                try
                {
                    m.ResultInternal = Oleacc.LresultFromObject(ref IID.IAccessible, m.WParamInternal, new HandleRef(this, pUnknown));
                }
                finally
                {
                    Marshal.Release(pUnknown);
                }
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(SR.RichControlLresult, e);
            }
        }
    }
}
