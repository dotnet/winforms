// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;
using static System.Windows.Forms.Form;

namespace System.Windows.Forms.Tests.AccessibleObjects;

public class Form_FormAccessibleObjectTests
{
    [WinFormsFact]
    public void FormAccessibleObject_Ctor_Default()
    {
        using Form form = new();
        FormAccessibleObject accessibleObject = new(form);

        Assert.Equal(form, accessibleObject.Owner);
        Assert.False(form.IsHandleCreated);
    }

    [WinFormsFact]
    public void FormAccessibleObject_ControlType_IsWindow_IfAccessibleRoleIsDefault()
    {
        using Form form = new();
        // AccessibleRole is not set = Default

        AccessibleObject accessibleObject = form.AccessibilityObject;
        VARIANT actual = accessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId);

        Assert.Equal(AccessibleRole.Client, accessibleObject.Role);
        Assert.Equal(UIA_CONTROLTYPE_ID.UIA_WindowControlTypeId, (UIA_CONTROLTYPE_ID)(int)actual);
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
            actualValue = (bool?)accessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_IsDialogPropertyId);
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
            actualValue = (bool?)accessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_IsDialogPropertyId);
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
            if (role is AccessibleRole.Default or AccessibleRole.Client)
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
        using Form form = new();
        form.AccessibleRole = role;

        VARIANT actual = form.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId);
        UIA_CONTROLTYPE_ID expected = AccessibleRoleControlTypeMap.GetControlType(role);

        Assert.Equal(expected, (UIA_CONTROLTYPE_ID)(int)actual);
        Assert.False(form.IsHandleCreated);
    }

    [WinFormsFact]
    public void FormAccessibleObject_Role_IsClient_ByDefault()
    {
        using Form form = new();
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

        internal override bool RaiseAutomationEvent(UIA_EVENT_ID eventId)
        {
            if (eventId == UIA_EVENT_ID.UIA_AutomationFocusChangedEventId)
            {
                RaiseAutomationFocusEventCallsCount++;
            }

            return base.RaiseAutomationEvent(eventId);
        }
    }
}
