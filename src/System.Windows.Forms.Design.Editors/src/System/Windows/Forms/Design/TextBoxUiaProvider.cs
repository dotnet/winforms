// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static System.Windows.Forms.Design.UnsafeNativeMethods;

namespace System.Windows.Forms.Design
{
    internal class TextBoxUiaProvider : IRawElementProviderSimple
    {
        private TextBox _owningTextBox;
        private AccessibleObject _defaultAccessibilityObject;
        private bool notificationEventAvailable = true;

        public TextBoxUiaProvider(TextBox owningTextBox, AccessibleObject accessibilityObject)
        {
            _owningTextBox = owningTextBox;
            _defaultAccessibilityObject = accessibilityObject;
        }

        public IRawElementProviderSimple HostRawElementProvider
        {
            get
            {
                IRawElementProviderSimple provider;
                UnsafeNativeMethods.UiaHostProviderFromHwnd(new Runtime.InteropServices.HandleRef(this, _owningTextBox.Handle), out provider);
                return provider;
            }
        }

        public ProviderOptions ProviderOptions
        {
            get
            {
                return ProviderOptions.ServerSideProvider;
            }
        }

        public object GetPatternProvider(int patternId)
        {
            return null;
        }

        public object GetPropertyValue(int propertyId)
        {
            switch (propertyId)
            {
                case NativeMethods.UIA_ControlTypePropertyId:
                    return NativeMethods.UIA_EditControlTypeId;
                case NativeMethods.UIA_NamePropertyId:
                    return _defaultAccessibilityObject.Name;
                case NativeMethods.UIA_AccessKeyPropertyId:
                    return _defaultAccessibilityObject.KeyboardShortcut ?? string.Empty;
                case NativeMethods.UIA_HasKeyboardFocusPropertyId:
                    return (_defaultAccessibilityObject.State & AccessibleStates.Focused) == AccessibleStates.Focused;
                case NativeMethods.UIA_IsKeyboardFocusablePropertyId:
                    return (_defaultAccessibilityObject.State & AccessibleStates.Focusable) == AccessibleStates.Focusable;
                case NativeMethods.UIA_IsEnabledPropertyId:
                    return _owningTextBox.Enabled;
                case NativeMethods.UIA_HelpTextPropertyId:
                    return _defaultAccessibilityObject.Help ?? string.Empty;
                case NativeMethods.UIA_IsPasswordPropertyId:
                    return false;
                case NativeMethods.UIA_IsOffscreenPropertyId:
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
            if (UnsafeNativeMethods.UiaClientsAreListening())
            {
                int result = UnsafeNativeMethods.UiaRaiseAutomationEvent(this, eventId);
                return result == NativeMethods.S_OK;
            }

            return false;
        }

        public bool RaiseAutomationNotification(
            NativeMethods.AutomationNotificationKind notificationKind,
            NativeMethods.AutomationNotificationProcessing notificationProcessing,
            string notificationText)
        {
            if (!notificationEventAvailable)
            {
                return false;
            }

            if (!UnsafeNativeMethods.UiaClientsAreListening())
            {
                return false;
            }

            int result = NativeMethods.S_FALSE;
            try
            {
                // The activityId can be any string. It cannot be null. It isn’t used currently.
                result = UnsafeNativeMethods.UiaRaiseNotificationEvent(
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
