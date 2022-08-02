// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using Xunit;
using static System.Windows.Forms.Form;
using static Interop;

namespace System.Windows.Forms.Tests
{
    public class Form_FormAccessibleObjectTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void FormAccessibleObject_Ctor_Default()
        {
            using Form form = new Form();
            FormAccessibleObject accessibleObject = new FormAccessibleObject(form);

            Assert.Equal(form, accessibleObject.Owner);
            Assert.False(form.IsHandleCreated);
        }

        [WinFormsFact]
        public void FormAccessibleObject_ControlType_IsWindow_IfAccessibleRoleIsDefault()
        {
            using Form form = new Form();
            // AccessibleRole is not set = Default

            AccessibleObject accessibleObject = form.AccessibilityObject;
            object actual = accessibleObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);

            Assert.Equal(AccessibleRole.Client, accessibleObject.Role);
            Assert.Equal(UiaCore.UIA.WindowControlTypeId, actual);
            Assert.False(form.IsHandleCreated);
        }

        [WinFormsFact]
        public void FormAccessibleObject_ControlType_IsDialog_True()
        {
            using Form form = new();

            bool? actualValue = null;
            form.Load += (_, _) =>
            {
                AccessibleObject accessibleObject = form.AccessibilityObject;
                actualValue = (bool?)accessibleObject.GetPropertyValue(UiaCore.UIA.IsDialogPropertyId);
                form.Close();
            };
            form.ShowDialog();

            Assert.True(actualValue);
            Assert.False(form.IsHandleCreated);
        }

        [WinFormsFact]
        public void FormAccessibleObject_ControlType_IsDialog_False()
        {
            using Form form = new();

            bool? actualValue = null;
            form.Load += (_, _) =>
            {
                AccessibleObject accessibleObject = form.AccessibilityObject;
                actualValue = (bool?)accessibleObject.GetPropertyValue(UiaCore.UIA.IsDialogPropertyId);
                form.Close();
            };
            form.Show();

            Assert.False(actualValue);
            Assert.False(form.IsHandleCreated);
        }

        public static IEnumerable<object[]> FormAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData()
        {
            Array roles = Enum.GetValues(typeof(AccessibleRole));

            foreach (AccessibleRole role in roles)
            {
                if (role == AccessibleRole.Default || role == AccessibleRole.Client)
                {
                    continue; // The test checks custom roles. "Client" is the default role and it has special handling.
                }

                yield return new object[] { role };
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(FormAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData))]
        public void FormAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole(AccessibleRole role)
        {
            using Form form = new Form();
            form.AccessibleRole = role;

            object actual = form.AccessibilityObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);
            UiaCore.UIA expected = AccessibleRoleControlTypeMap.GetControlType(role);

            Assert.Equal(expected, actual);
            Assert.False(form.IsHandleCreated);
        }

        [WinFormsFact]
        public void FormAccessibleObject_Role_IsClient_ByDefault()
        {
            using Form form = new Form();
            // AccessibleRole is not set = Default

            AccessibleRole actual = form.AccessibilityObject.Role;

            Assert.Equal(AccessibleRole.Client, actual);
            Assert.False(form.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData("null")] // The form is empty
        [InlineData("invisible")]  // The form has invisible control (looks empty)
        [InlineData("disabled")] // The form has disabled control
        public void FormAccessibleObject_RaiseFocusEvent_WhenFormGetsFocus_WithoutActiveControl(string controlCase)
        {
            using Form form = new FocusEventsCounterForm();
            using Control control = controlCase switch
            {
                "null" => null,
                "invisible" => new Button() { Visible = false },
                "disabled" => new Button() { Enabled = false },
                _ => null
            };
            form.Controls.Add(control);
            form.CreateControl(true);
            var accessibleObject = (FocusEventsCounterFormAccessibleObject)form.AccessibilityObject;

            Assert.NotNull(accessibleObject);
            Assert.Equal(0, accessibleObject.RaiseAutomationFocusEventCallsCount);
            Assert.True(form.IsHandleCreated);

            form.Visible = true;
            form.Focus();

            Assert.Null(form.ActiveControl);
            Assert.Equal(1, accessibleObject.RaiseAutomationFocusEventCallsCount);
        }

        [WinFormsFact]
        public void FormAccessibleObject_RaiseFocusEvent_WhenFormGetsFocus_WithActiveControl()
        {
            using Form form = new FocusEventsCounterForm();
            using Button control = new();
            form.Controls.Add(control);
            form.CreateControl(true);
            var accessibleObject = (FocusEventsCounterFormAccessibleObject)form.AccessibilityObject;

            Assert.NotNull(accessibleObject);
            Assert.Equal(0, accessibleObject.RaiseAutomationFocusEventCallsCount);
            Assert.True(form.IsHandleCreated);

            form.Visible = true;
            control.Visible = true;
            form.Focus();

            Assert.NotNull(form.ActiveControl);

            // The child control gets the focus changed event instead of the form.
            // Native control does it itself, so a screen reader should focus on the inner control.
            Assert.Equal(0, accessibleObject.RaiseAutomationFocusEventCallsCount);
        }

        [WinFormsTheory]
        [InlineData(true)]
        [InlineData(false)]
        public void FormAccessibleObject_BoundingRectangle_ReturnsExpected_ForRootForm(bool createControl)
        {
            using Form form = new();

            if (createControl)
            {
                form.CreateControl(true);
            }

            Rectangle actual = form.AccessibilityObject.BoundingRectangle;
            Rectangle expected = createControl ? form.Bounds : Rectangle.Empty;

            Assert.Equal(expected, actual);
            Assert.Equal(createControl, form.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true)]
        [InlineData(false)]
        public void FormAccessibleObject_BoundingRectangle_ReturnsExpected_ForEmbeddedForm(bool createControl)
        {
            using Form form = new();
            using Form embeddedForm = new() { TopLevel = false };
            form.Controls.Add(embeddedForm);

            if (createControl)
            {
                form.CreateControl(true);
            }

            Rectangle actual = form.AccessibilityObject.BoundingRectangle;

            Assert.Equal(createControl, actual.Location != Point.Empty);
            Assert.Equal(createControl, form.IsHandleCreated);
        }

        private class FocusEventsCounterForm : Form
        {
            protected override AccessibleObject CreateAccessibilityInstance()
                => new FocusEventsCounterFormAccessibleObject(this);
        }

        private class FocusEventsCounterFormAccessibleObject : FormAccessibleObject
        {
            public FocusEventsCounterFormAccessibleObject(Form owner) : base(owner)
            {
                RaiseAutomationFocusEventCallsCount = 0;
            }

            public int RaiseAutomationFocusEventCallsCount { get; private set; }

            internal override bool RaiseAutomationEvent(UiaCore.UIA eventId)
            {
                if (eventId == UiaCore.UIA.AutomationFocusChangedEventId)
                {
                    RaiseAutomationFocusEventCallsCount++;
                }

                return base.RaiseAutomationEvent(eventId);
            }
        }
    }
}
