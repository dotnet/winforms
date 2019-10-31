// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

namespace System.Windows.Forms.Design
{
    internal class TextBoxUiaProvider : Interop.UiaCore.IRawElementProviderSimple
    {
        private TextBox _owningTextBox;
        private AccessibleObject _defaultAccessibilityObject;
        private bool notificationEventAvailable = true;

        public TextBoxUiaProvider(TextBox owningTextBox, AccessibleObject accessibilityObject)
        {
            _owningTextBox = owningTextBox;
            _defaultAccessibilityObject = accessibilityObject;
        }

        public Interop.UiaCore.IRawElementProviderSimple HostRawElementProvider
        {
            get
            {
                Interop.UiaCore.IRawElementProviderSimple provider;
                Interop.UiaCore.UiaHostProviderFromHwnd(new Runtime.InteropServices.HandleRef(this, _owningTextBox.Handle), out provider);
                return provider;
            }
        }

        public Interop.UiaCore.ProviderOptions ProviderOptions
        {
            get
            {
                return Interop.UiaCore.ProviderOptions.ServerSideProvider;
            }
        }

        [return: MarshalAs(UnmanagedType.IUnknown)]
        public object GetPatternProvider(Interop.UiaCore.UIA patternId) => null;

        public object GetPropertyValue(Interop.UiaCore.UIA propertyId)
        {
            switch (propertyId)
            {
                case Interop.UiaCore.UIA.ControlTypePropertyId:
                    return Interop.UiaCore.UIA.EditControlTypeId;
                case Interop.UiaCore.UIA.NamePropertyId:
                    return _defaultAccessibilityObject.Name;
                case Interop.UiaCore.UIA.AccessKeyPropertyId:
                    return _defaultAccessibilityObject.KeyboardShortcut ?? string.Empty;
                case Interop.UiaCore.UIA.HasKeyboardFocusPropertyId:
                    return (_defaultAccessibilityObject.State & AccessibleStates.Focused) == AccessibleStates.Focused;
                case Interop.UiaCore.UIA.IsKeyboardFocusablePropertyId:
                    return (_defaultAccessibilityObject.State & AccessibleStates.Focusable) == AccessibleStates.Focusable;
                case Interop.UiaCore.UIA.IsEnabledPropertyId:
                    return _owningTextBox.Enabled;
                case Interop.UiaCore.UIA.HelpTextPropertyId:
                    return _defaultAccessibilityObject.Help ?? string.Empty;
                case Interop.UiaCore.UIA.IsPasswordPropertyId:
                    return false;
                case Interop.UiaCore.UIA.IsOffscreenPropertyId:
                    return (_defaultAccessibilityObject.State & AccessibleStates.Offscreen) == AccessibleStates.Offscreen;
            }

            return null;
        }

        public int[] GetRuntimeId()
        {
            var runtimeId = new int[2];
            runtimeId[0] = 0x2a;
            runtimeId[1] = (int)(long)_owningTextBox.Handle;
            return runtimeId;
        }

        public bool RaiseAutomationEvent(int eventId)
        {
            if (Interop.UiaCore.UiaClientsAreListening() == Interop.BOOL.TRUE)
            {
                Interop.HRESULT result = Interop.UiaCore.UiaRaiseAutomationEvent(this, (Interop.UiaCore.UIA)eventId);
                return result == Interop.HRESULT.S_OK;
            }

            return false;
        }

        public bool RaiseAutomationNotification(
            Interop.UiaCore.AutomationNotificationKind notificationKind,
            Interop.UiaCore.AutomationNotificationProcessing notificationProcessing,
            string notificationText)
        {
            if (!notificationEventAvailable)
            {
                return false;
            }

            if (Interop.UiaCore.UiaClientsAreListening() == Interop.BOOL.FALSE)
            {
                return false;
            }

            Interop.HRESULT result = Interop.HRESULT.S_FALSE;
            try
            {
                // The activityId can be any string. It cannot be null. It isn’t used currently.
                result = Interop.UiaCore.UiaRaiseNotificationEvent(
                this,
                notificationKind,
                notificationProcessing,
                notificationText,
                String.Empty);
            }
            catch (EntryPointNotFoundException)
            {
                // The UIA Notification event is not available, so don't attempt to raise it again.
                notificationEventAvailable = false;
            }

            return result == NativeMethods.S_OK;
        }
    }
}
