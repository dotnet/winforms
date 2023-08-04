﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using static Interop.UiaCore;

namespace System.Windows.Forms.Tests;

public class PrintPreviewControl_PrintPreviewControlAccessibleObjectTests
{
    [WinFormsFact]
    public void PrintPreviewControlAccessibleObject_Ctor_Default()
    {
        using PrintPreviewControl control = new();
        PrintPreviewControl.PrintPreviewControlAccessibleObject accessibleObject = new(control);

        Assert.Equal(control, accessibleObject.Owner);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData((int)UIA.NamePropertyId, "TestName")]
    [InlineData((int)UIA.AutomationIdPropertyId, "PrintPreviewControl1")]
    public void PrintPreviewControlAccessibleObject_GetPropertyValue_Invoke_ReturnsExpected(int propertyID, object expected)
    {
        using PrintPreviewControl control = new()
        {
            Name = "PrintPreviewControl1",
            AccessibleName = "TestName"
        };

        Assert.False(control.IsHandleCreated);
        var accessibleObject = new PrintPreviewControl.PrintPreviewControlAccessibleObject(control);
        object value = accessibleObject.GetPropertyValue((UIA)propertyID);

        Assert.Equal(expected, value);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void PrintPreviewControlAccessibleObject_GetPropertyValue_Focused_ReturnsExpected()
    {
        using PrintPreviewControl control = new();

        var accessibleObject = new PrintPreviewControl.PrintPreviewControlAccessibleObject(control);
        control.CreateControl();
        PInvoke.SetFocus(control);
        bool value = (bool)accessibleObject.GetPropertyValue(UIA.HasKeyboardFocusPropertyId);

        Assert.True(value);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void PrintPreviewControlAccessibleObject_GetPropertyValue_Unfocused_ReturnsExpected()
    {
        using PrintPreviewControl control = new();

        Assert.False(control.IsHandleCreated);
        var accessibleObject = new PrintPreviewControl.PrintPreviewControlAccessibleObject(control);
        bool value = (bool)accessibleObject.GetPropertyValue(UIA.HasKeyboardFocusPropertyId);

        Assert.False(value);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void PrintPreviewControlAccessibleObject_BoundingRectangle_NoHandle_ReturnsExpected()
    {
        using PrintPreviewControl control = new();

        AccessibleObject accessibleObject = new PrintPreviewControl.PrintPreviewControlAccessibleObject(control);

        Assert.Equal(Rectangle.Empty, accessibleObject.BoundingRectangle);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void PrintPreviewControlAccessibleObject_BoundingRectangle_ReturnsExpected()
    {
        using PrintPreviewControl control = new();

        using Panel panel = new();
        panel.Controls.Add(control);
        panel.CreateControl();

        Rectangle controlBounds = panel.RectangleToScreen(control.Bounds);

        AccessibleObject accessibleObject = new PrintPreviewControl.PrintPreviewControlAccessibleObject(control);
        Rectangle accessibleObjectBoundingRectangle = accessibleObject.BoundingRectangle;

        Assert.Equal(controlBounds, accessibleObjectBoundingRectangle);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void PrintPreviewControlAccessibleObject_Bounds_NoHandle_ReturnsExpected()
    {
        using PrintPreviewControl control = new();

        AccessibleObject accessibleObject = new PrintPreviewControl.PrintPreviewControlAccessibleObject(control);

        Assert.Equal(Rectangle.Empty, accessibleObject.Bounds);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void PrintPreviewControlAccessibleObject_Bounds_ReturnsExpected()
    {
        using PrintPreviewControl control = new();
        control.CreateControl();

        Rectangle controlClientRectangle = control.RectangleToScreen(control.ClientRectangle);

        AccessibleObject accessibleObject = new PrintPreviewControl.PrintPreviewControlAccessibleObject(control);

        Assert.Equal(controlClientRectangle, accessibleObject.Bounds);
        Assert.True(control.IsHandleCreated);
    }
}
