// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using static Interop;

namespace System.Windows.Forms.Tests
{
    public class LabelAccessibleObjectTests
    {
        [WinFormsTheory]
        [InlineData((int)UiaCore.UIA.NamePropertyId, "Address")]
        [InlineData((int)UiaCore.UIA.AutomationIdPropertyId, "Label1")]
        public void LabelAccessibleObject_GetPropertyValue_Invoke_ReturnsExpected(int propertyID, object expected)
        {
            using Label label = new()
            {
                Text = "Some test label text",
                Name = "Label1",
                AccessibleName = "Address"
            };

            Label.LabelAccessibleObject accessibilityObject = (Label.LabelAccessibleObject)label.AccessibilityObject;

            object value = accessibilityObject.GetPropertyValue((UiaCore.UIA)propertyID);
            Assert.Equal(expected, value);
            Assert.False(label.IsHandleCreated);
        }

        [WinFormsFact]
        public void LabelAccessibleObject_IsPatternSupported_LegacyIAccessible_ReturnsTrue()
        {
            using var label = new Label();
            label.Name = "Label1";
            label.Text = "Some test label text";
            var labelAccessibleObject = new Label.LabelAccessibleObject(label);

            Assert.False(label.IsHandleCreated);

            bool supportsLegacyIAccessiblePatternId = labelAccessibleObject.IsPatternSupported(Interop.UiaCore.UIA.LegacyIAccessiblePatternId);
            Assert.True(supportsLegacyIAccessiblePatternId);
        }

        [WinFormsFact]
        public void LabelAccessibleObject_LegacyIAccessible_Role_ReturnsExpected()
        {
            using var label = new Label();
            label.Name = "Label1";
            label.Text = "Some test label text";
            label.AccessibleRole = AccessibleRole.Link;
            var labelAccessibleObject = new Label.LabelAccessibleObject(label);

            Assert.False(label.IsHandleCreated);
            Assert.Equal(AccessibleRole.Link, labelAccessibleObject.Role);
        }

        [WinFormsFact]
        public void LabelAccessibleObject_Role_IsStaticText_ByDefault()
        {
            using Label label = new Label();
            // AccessibleRole is not set = Default

            AccessibleRole actual = label.AccessibilityObject.Role;

            Assert.Equal(AccessibleRole.StaticText, actual);
            Assert.False(label.IsHandleCreated);
        }

        [WinFormsFact]
        public void LabelAccessibleObject_LegacyIAccessible_Description_ReturnsExpected()
        {
            string testAccDescription = "Test description";
            using var label = new Label();
            label.Name = "Label1";
            label.Text = "Some test label text";
            label.AccessibleDescription = testAccDescription;
            var labelAccessibleObject = new Label.LabelAccessibleObject(label);

            Assert.False(label.IsHandleCreated);
            Assert.Equal(testAccDescription, labelAccessibleObject.Description);
        }

        [WinFormsFact]
        public void LabelAccessibleObject_ControlType_IsText_IfAccessibleRoleIsDefault()
        {
            using Label label = new Label();
            // AccessibleRole is not set = Default

            object actual = label.AccessibilityObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);

            Assert.Equal(UiaCore.UIA.TextControlTypeId, actual);
            Assert.False(label.IsHandleCreated);
        }

        public static IEnumerable<object[]> LabelAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData()
        {
            Array roles = Enum.GetValues(typeof(AccessibleRole));

            foreach (AccessibleRole role in roles)
            {
                if (role == AccessibleRole.Default)
                {
                    continue; // The test checks custom roles
                }

                yield return new object[] { role };
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(LabelAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData))]
        public void LabelAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole(AccessibleRole role)
        {
            using Label label = new Label();
            label.AccessibleRole = role;

            object actual = label.AccessibilityObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);
            UiaCore.UIA expected = AccessibleRoleControlTypeMap.GetControlType(role);

            Assert.Equal(expected, actual);
            Assert.False(label.IsHandleCreated);
        }

        [WinFormsFact]
        public void LabelAccessibleObject_TextChanged_AutomationPropertyChanged_Raised()
        {
            const string newText = "New text";
            using var control = new LabelWithCustomAccessibleObject(
                (propertyId, value) => propertyId == UiaCore.UIA.NamePropertyId && ReferenceEquals(value, newText))
            {
                Text = "Text"
            };

            var accessibilityObject = control.AccessibilityObject as ControlAccessibleObjectWithNotificationCounter;
            Assert.NotNull(accessibilityObject);
            Assert.True(control.IsAccessibilityObjectCreated);
            Assert.Equal(0, accessibilityObject.RaiseAutomationNotificationCallCount);

            control.Text = newText;

            Assert.Equal(1, accessibilityObject.RaiseAutomationNotificationCallCount);
        }

        private class LabelWithCustomAccessibleObject : Label
        {
            private readonly Func<UiaCore.UIA, object, bool> _checkRaisedEvent;

            public LabelWithCustomAccessibleObject(Func<UiaCore.UIA, object, bool> checkRaisedEvent)
            {
                _checkRaisedEvent = checkRaisedEvent;
            }

            protected override AccessibleObject CreateAccessibilityInstance() => new ControlAccessibleObjectWithNotificationCounter(this, _checkRaisedEvent);
        }

        private class ControlAccessibleObjectWithNotificationCounter : Control.ControlAccessibleObject
        {
            private readonly Func<UiaCore.UIA, object, bool> _checkRaisedEvent;

            public ControlAccessibleObjectWithNotificationCounter(Control ownerControl, Func<UiaCore.UIA, object, bool> checkRaisedEvent) : base(ownerControl)
            {
                _checkRaisedEvent = checkRaisedEvent;
            }

            internal int RaiseAutomationNotificationCallCount { get; private set; }

            internal override bool RaiseAutomationPropertyChangedEvent(UiaCore.UIA propertyId, object oldValue, object newValue)
            {
                if (_checkRaisedEvent(propertyId, newValue))
                {
                    RaiseAutomationNotificationCallCount++;
                }

                return base.RaiseAutomationPropertyChangedEvent(propertyId, oldValue, newValue);
            }
        }
    }
}
