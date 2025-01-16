// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms.Layout;
using System.Windows.Forms.TestUtilities;
using Moq;
using Windows.Win32.System.Ole;

namespace System.Windows.Forms.Tests;

public partial class ControlTests
{
    [WinFormsTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void Control_AccessibilityObject_Get_ReturnsExpected(bool createControl)
    {
        using Control control = new();
        if (createControl)
        {
            control.CreateControl();
        }

        Assert.Equal(createControl, control.IsHandleCreated);
        Control.ControlAccessibleObject accessibleObject = Assert.IsType<Control.ControlAccessibleObject>(control.AccessibilityObject);
        Assert.Same(accessibleObject, control.AccessibilityObject);
        Assert.Equal(createControl, control.IsHandleCreated);
        Assert.Same(control, accessibleObject.Owner);
    }

    [WinFormsFact]
    public void Control_AccessibilityObject_GetWithHandle_ReturnsExpected()
    {
        using Control control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        Control.ControlAccessibleObject accessibleObject = Assert.IsType<Control.ControlAccessibleObject>(control.AccessibilityObject);
        Assert.Same(accessibleObject, control.AccessibilityObject);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Same(control, accessibleObject.Owner);
    }

    public static TheoryData<AccessibleObject, AccessibleObject> AccessibilityObject_CustomCreateAccessibilityInstance_TestData()
    {
        AccessibleObject accessibleObject = new();
        return new TheoryData<AccessibleObject, AccessibleObject>
        {
            { accessibleObject, accessibleObject }
        };
    }

    [WinFormsTheory]
    [MemberData(nameof(AccessibilityObject_CustomCreateAccessibilityInstance_TestData))]
    public void Control_AccessibilityObject_GetCustomCreateAccessibilityInstance_ReturnsExpected(AccessibleObject result, AccessibleObject expected)
    {
        using CustomCreateAccessibilityInstanceControl control = new()
        {
            CreateAccessibilityResult = result
        };

        Assert.Same(expected, control.AccessibilityObject);
        Assert.Same(control.AccessibilityObject, control.AccessibilityObject);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void Control_AccessibilityObject_GetCustomCreateAccessibilityInstance_WithRealControlAccessibleObject_ReturnsExpected()
    {
        using Control control1 = new();
        var controlAccessibleObject = new Control.ControlAccessibleObject(control1);

        using (new NoAssertContext())
        {
            using CustomCreateAccessibilityInstanceControl control = new()
            {
                CreateAccessibilityResult = controlAccessibleObject
            };
            Assert.Same(controlAccessibleObject, control.AccessibilityObject);
            Assert.Same(control.AccessibilityObject, control.AccessibilityObject);
            Assert.False(control.IsHandleCreated);
        }
    }

    private class CustomCreateAccessibilityInstanceControl : Control
    {
        public AccessibleObject CreateAccessibilityResult { get; set; }

        protected override AccessibleObject CreateAccessibilityInstance() => CreateAccessibilityResult;
    }

    [WinFormsTheory]
    [StringWithNullData]
    public void Control_AccessibleDefaultActionDescription_Set_GetReturnsExpected(string value)
    {
        using Control control = new()
        {
            AccessibleDefaultActionDescription = value
        };
        Assert.Equal(value, control.AccessibleDefaultActionDescription);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.AccessibleDefaultActionDescription = value;
        Assert.Equal(value, control.AccessibleDefaultActionDescription);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [StringWithNullData]
    public void Control_AccessibleDescription_Set_GetReturnsExpected(string value)
    {
        using Control control = new()
        {
            AccessibleDescription = value
        };
        Assert.Equal(value, control.AccessibleDescription);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.AccessibleDescription = value;
        Assert.Equal(value, control.AccessibleDescription);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [StringWithNullData]
    public void Control_AccessibleName_Set_GetReturnsExpected(string value)
    {
        using Control control = new()
        {
            AccessibleName = value
        };
        Assert.Equal(value, control.AccessibleName);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.AccessibleName = value;
        Assert.Equal(value, control.AccessibleName);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [EnumData<AccessibleRole>]
    public void Control_AccessibleRole_Set_GetReturnsExpected(AccessibleRole value)
    {
        using Control control = new()
        {
            AccessibleRole = value
        };
        Assert.Equal(value, control.AccessibleRole);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.AccessibleRole = value;
        Assert.Equal(value, control.AccessibleRole);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InvalidEnumData<AccessibleRole>]
    public void Control_AccessibleRole_SetInvalidValue_ThrowsInvalidEnumArgumentException(AccessibleRole value)
    {
        using Control control = new();
        Assert.Throws<InvalidEnumArgumentException>("value", () => control.AccessibleRole = value);
    }

    [WinFormsTheory]
    [BoolData]
    public void Control_AllowDrop_Set_GetReturnsExpected(bool value)
    {
        using Control control = new()
        {
            AllowDrop = value
        };
        Assert.Equal(value, control.AllowDrop);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.AllowDrop = value;
        Assert.Equal(value, control.AllowDrop);
        Assert.False(control.IsHandleCreated);

        // Set different.
        control.AllowDrop = value;
        Assert.Equal(value, control.AllowDrop);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [BoolData]
    public void Control_AllowDrop_SetWithHandle_GetReturnsExpected(bool value)
    {
        using Control control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.AllowDrop = value;
        Assert.Equal(value, control.AllowDrop);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.AllowDrop = value;
        Assert.Equal(value, control.AllowDrop);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set different.
        control.AllowDrop = value;
        Assert.Equal(value, control.AllowDrop);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [BoolData]
    public void Control_AllowDrop_SetWithHandleAlreadyRegistered_GetReturnsExpected(bool value)
    {
        using Control control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        DropTargetMock dropTarget = new();
        Assert.Equal(ApartmentState.STA, Application.OleRequired());
        Assert.Equal(HRESULT.S_OK, PInvoke.RegisterDragDrop(control, dropTarget));

        control.AllowDrop = value;
        Assert.Equal(value, control.AllowDrop);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.AllowDrop = value;
        Assert.Equal(value, control.AllowDrop);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set different.
        control.AllowDrop = value;
        Assert.Equal(value, control.AllowDrop);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [Fact] // non-UI thread
    public void Control_AllowDrop_SetWithHandleNonSTAThread_ThrowsInvalidOperationException()
    {
        using Control control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        Assert.Throws<InvalidOperationException>(() => control.AllowDrop = true);
        Assert.False(control.AllowDrop);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Can set to false.
        control.AllowDrop = false;
        Assert.False(control.AllowDrop);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> Anchor_Set_TestData()
    {
        yield return new object[] { AnchorStyles.Top, AnchorStyles.Top };
        yield return new object[] { AnchorStyles.Top | AnchorStyles.Bottom, AnchorStyles.Top | AnchorStyles.Bottom };
        yield return new object[] { AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left, AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left };
        yield return new object[] { AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right, AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right };
        yield return new object[] { AnchorStyles.Top | AnchorStyles.Left, AnchorStyles.Top | AnchorStyles.Left };
        yield return new object[] { AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right, AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
        yield return new object[] { AnchorStyles.Top | AnchorStyles.Right, AnchorStyles.Top | AnchorStyles.Right };
        yield return new object[] { AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right, AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right };

        yield return new object[] { AnchorStyles.Bottom, AnchorStyles.Bottom };
        yield return new object[] { AnchorStyles.Bottom | AnchorStyles.Left, AnchorStyles.Bottom | AnchorStyles.Left };
        yield return new object[] { AnchorStyles.Bottom | AnchorStyles.Right, AnchorStyles.Bottom | AnchorStyles.Right };
        yield return new object[] { AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right, AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right };

        yield return new object[] { AnchorStyles.Left, AnchorStyles.Left };
        yield return new object[] { AnchorStyles.Left | AnchorStyles.Right, AnchorStyles.Left | AnchorStyles.Right };

        yield return new object[] { AnchorStyles.Right, AnchorStyles.Right };

        yield return new object[] { AnchorStyles.None, AnchorStyles.None };
        yield return new object[] { (AnchorStyles)(-1), AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right };
        yield return new object[] { (AnchorStyles)int.MaxValue, AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right };
    }

    [WinFormsTheory]
    [MemberData(nameof(Anchor_Set_TestData))]
    public void Control_Anchor_Set_GetReturnsExpected(AnchorStyles value, AnchorStyles expected)
    {
        using Control control = new();
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;

        control.Anchor = value;
        Assert.Equal(expected, control.Anchor);
        Assert.Equal(DockStyle.None, control.Dock);
        Assert.Equal(0, layoutCallCount);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.Anchor = value;
        Assert.Equal(expected, control.Anchor);
        Assert.Equal(DockStyle.None, control.Dock);
        Assert.Equal(0, layoutCallCount);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(Anchor_Set_TestData))]
    public void Control_Anchor_SetWithOldValue_GetReturnsExpected(AnchorStyles value, AnchorStyles expected)
    {
        using Control control = new()
        {
            Anchor = AnchorStyles.Left
        };
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;

        control.Anchor = value;
        Assert.Equal(expected, control.Anchor);
        Assert.Equal(DockStyle.None, control.Dock);
        Assert.Equal(0, layoutCallCount);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.Anchor = value;
        Assert.Equal(expected, control.Anchor);
        Assert.Equal(DockStyle.None, control.Dock);
        Assert.Equal(0, layoutCallCount);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> Anchor_SetWithParent_TestData()
    {
        yield return new object[] { AnchorStyles.Top, AnchorStyles.Top, 1, 1 };
        yield return new object[] { AnchorStyles.Top | AnchorStyles.Bottom, AnchorStyles.Top | AnchorStyles.Bottom, 1, 1 };
        yield return new object[] { AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left, AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left, 1, 1 };
        yield return new object[] { AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right, AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right, 1, 1 };
        yield return new object[] { AnchorStyles.Top | AnchorStyles.Left, AnchorStyles.Top | AnchorStyles.Left, 0, 0 };
        yield return new object[] { AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right, AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right, 1, 1 };
        yield return new object[] { AnchorStyles.Top | AnchorStyles.Right, AnchorStyles.Top | AnchorStyles.Right, 1, 1 };
        yield return new object[] { AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right, AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right, 1, 1 };

        yield return new object[] { AnchorStyles.Bottom, AnchorStyles.Bottom, 1, 1 };
        yield return new object[] { AnchorStyles.Bottom | AnchorStyles.Left, AnchorStyles.Bottom | AnchorStyles.Left, 1, 1 };
        yield return new object[] { AnchorStyles.Bottom | AnchorStyles.Right, AnchorStyles.Bottom | AnchorStyles.Right, 1, 1 };
        yield return new object[] { AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right, AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right, 1, 1 };

        yield return new object[] { AnchorStyles.Left, AnchorStyles.Left, 1, 1 };
        yield return new object[] { AnchorStyles.Left | AnchorStyles.Right, AnchorStyles.Left | AnchorStyles.Right, 1, 1 };

        yield return new object[] { AnchorStyles.Right, AnchorStyles.Right, 1, 1 };

        yield return new object[] { AnchorStyles.None, AnchorStyles.None, 1, 1 };
        yield return new object[] { (AnchorStyles)(-1), AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right, 1, 2 };
        yield return new object[] { (AnchorStyles)int.MaxValue, AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right, 1, 2 };
    }

    [WinFormsTheory]
    [MemberData(nameof(Anchor_SetWithParent_TestData))]
    public void Control_Anchor_SetWithParent_GetReturnsExpected(AnchorStyles value, AnchorStyles expected, int expectedParentLayoutCallCount1, int expectedParentLayoutCallCount2)
    {
        using Control parent = new();
        using Control control = new()
        {
            Parent = parent
        };
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(parent, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("Anchor", e.AffectedProperty);
            parentLayoutCallCount++;
        }

        parent.Layout += parentHandler;

        try
        {
            control.Anchor = value;
            Assert.Equal(expected, control.Anchor);
            Assert.Equal(DockStyle.None, control.Dock);
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(expectedParentLayoutCallCount1, parentLayoutCallCount);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.Anchor = value;
            Assert.Equal(expected, control.Anchor);
            Assert.Equal(DockStyle.None, control.Dock);
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(expectedParentLayoutCallCount2, parentLayoutCallCount);
            Assert.False(control.IsHandleCreated);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(Anchor_SetWithParent_TestData))]
    public void Control_Anchor_SetWithGrandparent_GetReturnsExpected(AnchorStyles value, AnchorStyles expected, int expectedParentLayoutCallCount1, int expectedParentLayoutCallCount2)
    {
        using Control grandparent = new();
        using Control parent = new()
        {
            Parent = grandparent
        };
        using Control control = new()
        {
            Parent = parent
        };
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(parent, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("Anchor", e.AffectedProperty);
            parentLayoutCallCount++;
        }

        int grandparentLayoutCallCount = 0;
        void grandparentHandler(object sender, LayoutEventArgs e)
        {
            grandparentLayoutCallCount++;
        }

        parent.Layout += parentHandler;
        grandparent.Layout += grandparentHandler;

        try
        {
            control.Anchor = value;
            Assert.Equal(expected, control.Anchor);
            Assert.Equal(DockStyle.None, control.Dock);
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(expectedParentLayoutCallCount1, parentLayoutCallCount);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.Anchor = value;
            Assert.Equal(expected, control.Anchor);
            Assert.Equal(DockStyle.None, control.Dock);
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(expectedParentLayoutCallCount2, parentLayoutCallCount);
            Assert.False(control.IsHandleCreated);
        }
        finally
        {
            parent.Layout -= parentHandler;
            grandparent.Layout -= parentHandler;
        }
    }

    public static IEnumerable<object[]> Anchor_SetWithOldValueWithParent_TestData()
    {
        foreach (AnchorStyles oldValue in new AnchorStyles[] { AnchorStyles.Left, AnchorStyles.Top, AnchorStyles.Right, AnchorStyles.Left })
        {
            yield return new object[] { oldValue, AnchorStyles.Top | AnchorStyles.Bottom, AnchorStyles.Top | AnchorStyles.Bottom, 1, 1 };
            yield return new object[] { oldValue, AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left, AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left, 1, 1 };
            yield return new object[] { oldValue, AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right, AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right, 1, 1 };
            yield return new object[] { oldValue, AnchorStyles.Top | AnchorStyles.Left, AnchorStyles.Top | AnchorStyles.Left, 1, 1 };
            yield return new object[] { oldValue, AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right, AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right, 1, 1 };
            yield return new object[] { oldValue, AnchorStyles.Top | AnchorStyles.Right, AnchorStyles.Top | AnchorStyles.Right, 1, 1 };
            yield return new object[] { oldValue, AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right, AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right, 1, 1 };

            yield return new object[] { oldValue, AnchorStyles.Bottom | AnchorStyles.Left, AnchorStyles.Bottom | AnchorStyles.Left, 1, 1 };
            yield return new object[] { oldValue, AnchorStyles.Bottom | AnchorStyles.Right, AnchorStyles.Bottom | AnchorStyles.Right, 1, 1 };
            yield return new object[] { oldValue, AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right, AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right, 1, 1 };

            yield return new object[] { oldValue, AnchorStyles.Left | AnchorStyles.Right, AnchorStyles.Left | AnchorStyles.Right, 1, 1 };

            yield return new object[] { oldValue, AnchorStyles.None, AnchorStyles.None, 1, 1 };
            yield return new object[] { oldValue, (AnchorStyles)(-1), AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right, 1, 2 };
            yield return new object[] { oldValue, (AnchorStyles)int.MaxValue, AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right, 1, 2 };
        }

        yield return new object[] { AnchorStyles.Top, AnchorStyles.Top, AnchorStyles.Top, 0, 0 };
        yield return new object[] { AnchorStyles.Bottom, AnchorStyles.Top, AnchorStyles.Top, 1, 1 };
        yield return new object[] { AnchorStyles.Left, AnchorStyles.Top, AnchorStyles.Top, 1, 1 };
        yield return new object[] { AnchorStyles.Right, AnchorStyles.Top, AnchorStyles.Top, 1, 1 };

        yield return new object[] { AnchorStyles.Top, AnchorStyles.Bottom, AnchorStyles.Bottom, 1, 1 };
        yield return new object[] { AnchorStyles.Bottom, AnchorStyles.Bottom, AnchorStyles.Bottom, 0, 0 };
        yield return new object[] { AnchorStyles.Left, AnchorStyles.Bottom, AnchorStyles.Bottom, 1, 1 };
        yield return new object[] { AnchorStyles.Right, AnchorStyles.Bottom, AnchorStyles.Bottom, 1, 1 };

        yield return new object[] { AnchorStyles.Top, AnchorStyles.Left, AnchorStyles.Left, 1, 1 };
        yield return new object[] { AnchorStyles.Bottom, AnchorStyles.Left, AnchorStyles.Left, 1, 1 };
        yield return new object[] { AnchorStyles.Left, AnchorStyles.Left, AnchorStyles.Left, 0, 0 };
        yield return new object[] { AnchorStyles.Right, AnchorStyles.Left, AnchorStyles.Left, 1, 1 };

        yield return new object[] { AnchorStyles.Top, AnchorStyles.Right, AnchorStyles.Right, 1, 1 };
        yield return new object[] { AnchorStyles.Bottom, AnchorStyles.Right, AnchorStyles.Right, 1, 1 };
        yield return new object[] { AnchorStyles.Left, AnchorStyles.Right, AnchorStyles.Right, 1, 1 };
        yield return new object[] { AnchorStyles.Right, AnchorStyles.Right, AnchorStyles.Right, 0, 0 };
    }

    [WinFormsTheory]
    [MemberData(nameof(Anchor_SetWithOldValueWithParent_TestData))]
    public void Control_Anchor_SetWithOldValueWithParent_GetReturnsExpected(AnchorStyles oldValue, AnchorStyles value, AnchorStyles expected, int expectedParentLayoutCallCount1, int expectedParentLayoutCallCount2)
    {
        using Control parent = new();
        using Control control = new()
        {
            Anchor = oldValue,
            Parent = parent
        };
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(parent, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("Anchor", e.AffectedProperty);
            parentLayoutCallCount++;
        }

        parent.Layout += parentHandler;

        try
        {
            control.Anchor = value;
            Assert.Equal(expected, control.Anchor);
            Assert.Equal(DockStyle.None, control.Dock);
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(expectedParentLayoutCallCount1, parentLayoutCallCount);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.Anchor = value;
            Assert.Equal(expected, control.Anchor);
            Assert.Equal(DockStyle.None, control.Dock);
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(expectedParentLayoutCallCount2, parentLayoutCallCount);
            Assert.False(control.IsHandleCreated);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    public static IEnumerable<object[]> Anchor_SetWithOldValueWithGrandparent_TestData()
    {
        foreach (AnchorStyles oldValue in new AnchorStyles[] { AnchorStyles.Left, AnchorStyles.Top, AnchorStyles.Right, AnchorStyles.Left })
        {
            yield return new object[] { oldValue, AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right, AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right, 1, 1, 0, 0 };
            yield return new object[] { oldValue, AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right, AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right, 1, 1, 0, 0 };
            yield return new object[] { oldValue, AnchorStyles.Top | AnchorStyles.Right, AnchorStyles.Top | AnchorStyles.Right, 1, 1, 0, 0 };

            yield return new object[] { oldValue, AnchorStyles.Bottom | AnchorStyles.Right, AnchorStyles.Bottom | AnchorStyles.Right, 1, 1, 0, 0 };
            yield return new object[] { oldValue, AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right, AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right, 1, 1, 0, 0 };

            yield return new object[] { oldValue, AnchorStyles.Left | AnchorStyles.Right, AnchorStyles.Left | AnchorStyles.Right, 1, 1, 0, 0 };

            yield return new object[] { oldValue, (AnchorStyles)(-1), AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right, 1, 2, 0, 0 };
            yield return new object[] { oldValue, (AnchorStyles)int.MaxValue, AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right, 1, 2, 0, 0 };
        }

        yield return new object[] { AnchorStyles.Top, AnchorStyles.Top | AnchorStyles.Bottom, AnchorStyles.Top | AnchorStyles.Bottom, 1, 1, 0, 0 };
        yield return new object[] { AnchorStyles.Bottom, AnchorStyles.Top | AnchorStyles.Bottom, AnchorStyles.Top | AnchorStyles.Bottom, 1, 1, 0, 0 };
        yield return new object[] { AnchorStyles.Left, AnchorStyles.Top | AnchorStyles.Bottom, AnchorStyles.Top | AnchorStyles.Bottom, 1, 1, 0, 0 };
        yield return new object[] { AnchorStyles.Right, AnchorStyles.Top | AnchorStyles.Bottom, AnchorStyles.Top | AnchorStyles.Bottom, 1, 1, 1, 1 };

        yield return new object[] { AnchorStyles.Top, AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left, AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left, 1, 1, 0, 0 };
        yield return new object[] { AnchorStyles.Bottom, AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left, AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left, 1, 1, 0, 0 };
        yield return new object[] { AnchorStyles.Left, AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left, AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left, 1, 1, 0, 0 };
        yield return new object[] { AnchorStyles.Right, AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left, AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left, 1, 1, 1, 1 };

        yield return new object[] { AnchorStyles.Top, AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left, AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left, 1, 1, 0, 0 };
        yield return new object[] { AnchorStyles.Bottom, AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left, AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left, 1, 1, 0, 0 };
        yield return new object[] { AnchorStyles.Left, AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left, AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left, 1, 1, 0, 0 };
        yield return new object[] { AnchorStyles.Right, AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left, AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left, 1, 1, 1, 1 };

        yield return new object[] { AnchorStyles.Top, AnchorStyles.Top | AnchorStyles.Left, AnchorStyles.Top | AnchorStyles.Left, 1, 1, 0, 0 };
        yield return new object[] { AnchorStyles.Bottom, AnchorStyles.Top | AnchorStyles.Left, AnchorStyles.Top | AnchorStyles.Left, 1, 1, 1, 1 };
        yield return new object[] { AnchorStyles.Left, AnchorStyles.Top | AnchorStyles.Left, AnchorStyles.Top | AnchorStyles.Left, 1, 1, 0, 0 };
        yield return new object[] { AnchorStyles.Bottom, AnchorStyles.Top | AnchorStyles.Left, AnchorStyles.Top | AnchorStyles.Left, 1, 1, 1, 1 };

        yield return new object[] { AnchorStyles.Top, AnchorStyles.Bottom | AnchorStyles.Left, AnchorStyles.Bottom | AnchorStyles.Left, 1, 1, 0, 0 };
        yield return new object[] { AnchorStyles.Bottom, AnchorStyles.Bottom | AnchorStyles.Left, AnchorStyles.Bottom | AnchorStyles.Left, 1, 1, 0, 0 };
        yield return new object[] { AnchorStyles.Left, AnchorStyles.Bottom | AnchorStyles.Left, AnchorStyles.Bottom | AnchorStyles.Left, 1, 1, 0, 0 };
        yield return new object[] { AnchorStyles.Right, AnchorStyles.Bottom | AnchorStyles.Left, AnchorStyles.Bottom | AnchorStyles.Left, 1, 1, 1, 1 };

        yield return new object[] { AnchorStyles.Top, AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right, AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right, 1, 1, 0, 0 };
        yield return new object[] { AnchorStyles.Bottom, AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right, AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right, 1, 1, 0, 0 };
        yield return new object[] { AnchorStyles.Left, AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right, AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right, 1, 1, 0, 0 };
        yield return new object[] { AnchorStyles.Right, AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right, AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right, 1, 1, 0, 0 };

        yield return new object[] { AnchorStyles.Top, AnchorStyles.None, AnchorStyles.None, 1, 1, 0, 0 };
        yield return new object[] { AnchorStyles.Bottom, AnchorStyles.None, AnchorStyles.None, 1, 1, 1, 1 };
        yield return new object[] { AnchorStyles.Left, AnchorStyles.None, AnchorStyles.None, 1, 1, 0, 0 };
        yield return new object[] { AnchorStyles.Right, AnchorStyles.None, AnchorStyles.None, 1, 1, 1, 1 };

        yield return new object[] { AnchorStyles.Top, AnchorStyles.Top, AnchorStyles.Top, 0, 0, 0, 0 };
        yield return new object[] { AnchorStyles.Bottom, AnchorStyles.Top, AnchorStyles.Top, 1, 1, 1, 1 };
        yield return new object[] { AnchorStyles.Left, AnchorStyles.Top, AnchorStyles.Top, 1, 1, 0, 0 };
        yield return new object[] { AnchorStyles.Right, AnchorStyles.Top, AnchorStyles.Top, 1, 1, 1, 1 };

        yield return new object[] { AnchorStyles.Top, AnchorStyles.Bottom, AnchorStyles.Bottom, 1, 1, 0, 0 };
        yield return new object[] { AnchorStyles.Bottom, AnchorStyles.Bottom, AnchorStyles.Bottom, 0, 0, 0, 0 };
        yield return new object[] { AnchorStyles.Left, AnchorStyles.Bottom, AnchorStyles.Bottom, 1, 1, 0, 0 };
        yield return new object[] { AnchorStyles.Right, AnchorStyles.Bottom, AnchorStyles.Bottom, 1, 1, 1, 1 };

        yield return new object[] { AnchorStyles.Top, AnchorStyles.Left, AnchorStyles.Left, 1, 1, 0, 0 };
        yield return new object[] { AnchorStyles.Bottom, AnchorStyles.Left, AnchorStyles.Left, 1, 1, 1, 1 };
        yield return new object[] { AnchorStyles.Left, AnchorStyles.Left, AnchorStyles.Left, 0, 0, 0, 0 };
        yield return new object[] { AnchorStyles.Right, AnchorStyles.Left, AnchorStyles.Left, 1, 1, 1, 1 };

        yield return new object[] { AnchorStyles.Top, AnchorStyles.Right, AnchorStyles.Right, 1, 1, 0, 0 };
        yield return new object[] { AnchorStyles.Bottom, AnchorStyles.Right, AnchorStyles.Right, 1, 1, 1, 1 };
        yield return new object[] { AnchorStyles.Left, AnchorStyles.Right, AnchorStyles.Right, 1, 1, 0, 0 };
        yield return new object[] { AnchorStyles.Right, AnchorStyles.Right, AnchorStyles.Right, 0, 0, 0, 0 };
    }

    [WinFormsTheory]
    [MemberData(nameof(Anchor_SetWithOldValueWithGrandparent_TestData))]
    public void Control_Anchor_SetWithOldValueWithGrandparent_GetReturnsExpected(AnchorStyles oldValue, AnchorStyles value, AnchorStyles expected, int expectedParentLayoutCallCount1, int expectedParentLayoutCallCount2, int expectedGrandparentLayoutCallCount1, int expectedGrandparentLayoutCallCount2)
    {
        using Control grandparent = new();
        using Control parent = new()
        {
            Parent = grandparent
        };
        using Control control = new()
        {
            Anchor = oldValue,
            Parent = parent
        };
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(parent, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("Anchor", e.AffectedProperty);
            parentLayoutCallCount++;
        }

        int grandparentLayoutCallCount = 0;
        void grandparentHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(grandparent, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("Anchor", e.AffectedProperty);
            grandparentLayoutCallCount++;
        }

        parent.Layout += parentHandler;
        grandparent.Layout += grandparentHandler;

        try
        {
            control.Anchor = value;
            Assert.Equal(expected, control.Anchor);
            Assert.Equal(DockStyle.None, control.Dock);
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(expectedParentLayoutCallCount1, parentLayoutCallCount);
            Assert.Equal(expectedGrandparentLayoutCallCount1, grandparentLayoutCallCount);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.Anchor = value;
            Assert.Equal(expected, control.Anchor);
            Assert.Equal(DockStyle.None, control.Dock);
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(expectedParentLayoutCallCount2, parentLayoutCallCount);
            Assert.Equal(expectedGrandparentLayoutCallCount2, grandparentLayoutCallCount);
            Assert.False(control.IsHandleCreated);
        }
        finally
        {
            parent.Layout -= parentHandler;
            grandparent.Layout -= grandparentHandler;
        }
    }

    public static IEnumerable<object[]> Anchor_SetWithDock_TestData()
    {
        foreach (DockStyle dock in Enum.GetValues(typeof(DockStyle)))
        {
            yield return new object[] { dock, AnchorStyles.Top, AnchorStyles.Top, DockStyle.None };
            yield return new object[] { dock, AnchorStyles.Top | AnchorStyles.Bottom, AnchorStyles.Top | AnchorStyles.Bottom, DockStyle.None };
            yield return new object[] { dock, AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left, AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left, DockStyle.None };
            yield return new object[] { dock, AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right, AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right, DockStyle.None };
            yield return new object[] { dock, AnchorStyles.Top | AnchorStyles.Left, AnchorStyles.Top | AnchorStyles.Left, dock };
            yield return new object[] { dock, AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right, AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right, DockStyle.None };
            yield return new object[] { dock, AnchorStyles.Top | AnchorStyles.Right, AnchorStyles.Top | AnchorStyles.Right, DockStyle.None };
            yield return new object[] { dock, AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right, AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right, DockStyle.None };

            yield return new object[] { dock, AnchorStyles.Bottom, AnchorStyles.Bottom, DockStyle.None };
            yield return new object[] { dock, AnchorStyles.Bottom | AnchorStyles.Left, AnchorStyles.Bottom | AnchorStyles.Left, DockStyle.None };
            yield return new object[] { dock, AnchorStyles.Bottom | AnchorStyles.Right, AnchorStyles.Bottom | AnchorStyles.Right, DockStyle.None };
            yield return new object[] { dock, AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right, AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right, DockStyle.None };

            yield return new object[] { dock, AnchorStyles.Left, AnchorStyles.Left, DockStyle.None };
            yield return new object[] { dock, AnchorStyles.Left | AnchorStyles.Right, AnchorStyles.Left | AnchorStyles.Right, DockStyle.None };

            yield return new object[] { dock, AnchorStyles.Right, AnchorStyles.Right, DockStyle.None };

            yield return new object[] { dock, AnchorStyles.None, AnchorStyles.None, DockStyle.None };
            yield return new object[] { dock, (AnchorStyles)(-1), AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right, DockStyle.None };
            yield return new object[] { dock, (AnchorStyles)int.MaxValue, AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right, DockStyle.None };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(Anchor_SetWithDock_TestData))]
    public void Control_Anchor_SetWithDock_GetReturnsExpected(DockStyle dock, AnchorStyles value, AnchorStyles expectedAnchor, DockStyle expectedDock)
    {
        using Control control = new()
        {
            Dock = dock
        };
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;

        control.Anchor = value;
        Assert.Equal(expectedAnchor, control.Anchor);
        Assert.Equal(expectedDock, control.Dock);
        Assert.Equal(0, layoutCallCount);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.Anchor = value;
        Assert.Equal(expectedAnchor, control.Anchor);
        Assert.Equal(expectedDock, control.Dock);
        Assert.Equal(0, layoutCallCount);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> Anchor_SetWithDockWithParent_TestData()
    {
        foreach (DockStyle dock in new DockStyle[] { DockStyle.Bottom, DockStyle.Fill, DockStyle.Left, DockStyle.Right, DockStyle.Top })
        {
            yield return new object[] { dock, AnchorStyles.Top, AnchorStyles.Top, DockStyle.None, 1 };
            yield return new object[] { dock, AnchorStyles.Top | AnchorStyles.Bottom, AnchorStyles.Top | AnchorStyles.Bottom, DockStyle.None, 1 };
            yield return new object[] { dock, AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left, AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left, DockStyle.None, 1 };
            yield return new object[] { dock, AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right, AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right, DockStyle.None, 1 };
            yield return new object[] { dock, AnchorStyles.Top | AnchorStyles.Left, AnchorStyles.Top | AnchorStyles.Left, dock, 0 };
            yield return new object[] { dock, AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right, AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right, DockStyle.None, 1 };
            yield return new object[] { dock, AnchorStyles.Top | AnchorStyles.Right, AnchorStyles.Top | AnchorStyles.Right, DockStyle.None, 1 };
            yield return new object[] { dock, AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right, AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right, DockStyle.None, 1 };

            yield return new object[] { dock, AnchorStyles.Bottom, AnchorStyles.Bottom, DockStyle.None, 1 };
            yield return new object[] { dock, AnchorStyles.Bottom | AnchorStyles.Left, AnchorStyles.Bottom | AnchorStyles.Left, DockStyle.None, 1 };
            yield return new object[] { dock, AnchorStyles.Bottom | AnchorStyles.Right, AnchorStyles.Bottom | AnchorStyles.Right, DockStyle.None, 1 };
            yield return new object[] { dock, AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right, AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right, DockStyle.None, 1 };

            yield return new object[] { dock, AnchorStyles.Left, AnchorStyles.Left, DockStyle.None, 1 };
            yield return new object[] { dock, AnchorStyles.Left | AnchorStyles.Right, AnchorStyles.Left | AnchorStyles.Right, DockStyle.None, 1 };

            yield return new object[] { dock, AnchorStyles.Right, AnchorStyles.Right, DockStyle.None, 1 };

            yield return new object[] { dock, AnchorStyles.None, AnchorStyles.None, DockStyle.None, 1 };
            yield return new object[] { dock, (AnchorStyles)(-1), AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right, DockStyle.None, 1 };
            yield return new object[] { dock, (AnchorStyles)int.MaxValue, AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right, DockStyle.None, 1 };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(Anchor_SetWithDockWithParent_TestData))]
    public void Control_Anchor_SetWithDockWithParent_GetReturnsExpected(DockStyle dock, AnchorStyles value, AnchorStyles expectedAnchor, DockStyle expectedDock, int expectedParentLayoutCallCount)
    {
        using Control parent = new();
        using Control control = new()
        {
            Parent = parent,
            Dock = dock
        };
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e)
        {
            if (e.AffectedProperty == "Dock")
            {
                Assert.Same(parent, sender);
                Assert.Same(control, e.AffectedControl);
                parentLayoutCallCount++;
            }
        }

        parent.Layout += parentHandler;

        try
        {
            control.Anchor = value;
            Assert.Equal(expectedAnchor, control.Anchor);
            Assert.Equal(expectedDock, control.Dock);
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.Anchor = value;
            Assert.Equal(expectedAnchor, control.Anchor);
            Assert.Equal(expectedDock, control.Dock);
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
            Assert.False(control.IsHandleCreated);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(Anchor_Set_TestData))]
    public void Control_Anchor_SetWithHandle_GetReturnsExpected(AnchorStyles value, AnchorStyles expected)
    {
        using Control control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;

        control.Anchor = value;
        Assert.Equal(expected, control.Anchor);
        Assert.Equal(DockStyle.None, control.Dock);
        Assert.Equal(0, layoutCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.Anchor = value;
        Assert.Equal(expected, control.Anchor);
        Assert.Equal(DockStyle.None, control.Dock);
        Assert.Equal(0, layoutCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelper), nameof(CommonTestHelper.GetPointTheoryData))]
    public void Control_AutoScrollOffset_Set_GetReturnsExpected(Point value)
    {
        using Control control = new()
        {
            AutoScrollOffset = value
        };
        Assert.Equal(value, control.AutoScrollOffset);

        // Set same.
        control.AutoScrollOffset = value;
        Assert.Equal(value, control.AutoScrollOffset);
    }

    [WinFormsTheory]
    [BoolData]
    public void Control_AutoSize_Set_GetReturnsExpected(bool value)
    {
        using Control control = new();
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;

        control.AutoSize = value;
        Assert.Equal(value, control.AutoSize);
        Assert.False(control.IsHandleCreated);
        Assert.Equal(0, layoutCallCount);

        // Set same.
        control.AutoSize = value;
        Assert.Equal(value, control.AutoSize);
        Assert.False(control.IsHandleCreated);
        Assert.Equal(0, layoutCallCount);

        // Set different.
        control.AutoSize = !value;
        Assert.Equal(!value, control.AutoSize);
        Assert.False(control.IsHandleCreated);
        Assert.Equal(0, layoutCallCount);
    }

    [WinFormsTheory]
    [InlineData(true, 1)]
    [InlineData(false, 0)]
    public void Control_AutoSize_SetWithParent_GetReturnsExpected(bool value, int expectedLayoutCallCount)
    {
        using Control parent = new();
        using Control control = new()
        {
            Parent = parent
        };
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(parent, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("AutoSize", e.AffectedProperty);
            parentLayoutCallCount++;
        }

        parent.Layout += parentHandler;

        try
        {
            control.AutoSize = value;
            Assert.Equal(value, control.AutoSize);
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(expectedLayoutCallCount, parentLayoutCallCount);
            Assert.False(control.IsHandleCreated);
            Assert.False(parent.IsHandleCreated);

            // Set same.
            control.AutoSize = value;
            Assert.Equal(value, control.AutoSize);
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(expectedLayoutCallCount, parentLayoutCallCount);
            Assert.False(control.IsHandleCreated);
            Assert.False(parent.IsHandleCreated);

            // Set different.
            control.AutoSize = !value;
            Assert.Equal(!value, control.AutoSize);
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(expectedLayoutCallCount + 1, parentLayoutCallCount);
            Assert.False(control.IsHandleCreated);
            Assert.False(parent.IsHandleCreated);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    [WinFormsTheory]
    [InlineData(true, 1)]
    [InlineData(false, 0)]
    public void Control_AutoSize_SetWithParentWithCustomLayoutEngine_GetReturnsExpected(bool value, int expectedLayoutCallCount)
    {
        using CustomLayoutEngineControl parent = new();
        using Control control = new()
        {
            Parent = parent
        };
        Mock<LayoutEngine> mockLayoutEngine = new(MockBehavior.Strict);
        mockLayoutEngine
            .Setup(e => e.Layout(parent, It.IsAny<LayoutEventArgs>()))
            .Returns(false)
            .Verifiable();
        mockLayoutEngine
            .Setup(e => e.InitLayout(control, BoundsSpecified.None));
        parent.SetLayoutEngine(mockLayoutEngine.Object);
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(parent, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("AutoSize", e.AffectedProperty);
            parentLayoutCallCount++;
        }

        parent.Layout += parentHandler;

        try
        {
            control.AutoSize = value;
            Assert.Equal(value, control.AutoSize);
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(expectedLayoutCallCount, parentLayoutCallCount);
            mockLayoutEngine.Verify(e => e.Layout(parent, It.IsAny<LayoutEventArgs>()), Times.Exactly(expectedLayoutCallCount));
            Assert.False(control.IsHandleCreated);
            Assert.False(parent.IsHandleCreated);

            // Set same.
            control.AutoSize = value;
            Assert.Equal(value, control.AutoSize);
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(expectedLayoutCallCount, parentLayoutCallCount);
            mockLayoutEngine.Verify(e => e.Layout(parent, It.IsAny<LayoutEventArgs>()), Times.Exactly(expectedLayoutCallCount));
            Assert.False(control.IsHandleCreated);
            Assert.False(parent.IsHandleCreated);

            // Set different.
            control.AutoSize = !value;
            Assert.Equal(!value, control.AutoSize);
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(expectedLayoutCallCount + 1, parentLayoutCallCount);
            mockLayoutEngine.Verify(e => e.Layout(parent, It.IsAny<LayoutEventArgs>()), Times.Exactly(expectedLayoutCallCount + 1));
            Assert.False(control.IsHandleCreated);
            Assert.False(parent.IsHandleCreated);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    [WinFormsTheory]
    [BoolData]
    public void Control_AutoSize_SetWithHandle_GetReturnsExpected(bool value)
    {
        using Control control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;

        control.AutoSize = value;
        Assert.Equal(value, control.AutoSize);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(0, layoutCallCount);

        // Set same.
        control.AutoSize = value;
        Assert.Equal(value, control.AutoSize);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(0, layoutCallCount);

        // Set different.
        control.AutoSize = !value;
        Assert.Equal(!value, control.AutoSize);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.Equal(0, layoutCallCount);
    }

    [WinFormsTheory]
    [InlineData(true, 1)]
    [InlineData(false, 0)]
    public void Control_AutoSize_SetWithParentWithHandle_GetReturnsExpected(bool value, int expectedLayoutCallCount)
    {
        using Control parent = new();
        using Control control = new()
        {
            Parent = parent
        };
        Assert.NotEqual(IntPtr.Zero, parent.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
        int parentInvalidatedCallCount = 0;
        parent.Invalidated += (sender, e) => parentInvalidatedCallCount++;
        int parentStyleChangedCallCount = 0;
        parent.StyleChanged += (sender, e) => parentStyleChangedCallCount++;
        int parentCreatedCallCount = 0;
        parent.HandleCreated += (sender, e) => parentCreatedCallCount++;
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(parent, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("AutoSize", e.AffectedProperty);
            parentLayoutCallCount++;
        }

        parent.Layout += parentHandler;

        try
        {
            control.AutoSize = value;
            Assert.Equal(value, control.AutoSize);
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(expectedLayoutCallCount, parentLayoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.True(parent.IsHandleCreated);
            Assert.Equal(0, parentInvalidatedCallCount);
            Assert.Equal(0, parentStyleChangedCallCount);
            Assert.Equal(0, parentCreatedCallCount);

            // Set same.
            control.AutoSize = value;
            Assert.Equal(value, control.AutoSize);
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(expectedLayoutCallCount, parentLayoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.True(parent.IsHandleCreated);
            Assert.Equal(0, parentInvalidatedCallCount);
            Assert.Equal(0, parentStyleChangedCallCount);
            Assert.Equal(0, parentCreatedCallCount);

            // Set different.
            control.AutoSize = !value;
            Assert.Equal(!value, control.AutoSize);
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(expectedLayoutCallCount + 1, parentLayoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.True(parent.IsHandleCreated);
            Assert.Equal(0, parentInvalidatedCallCount);
            Assert.Equal(0, parentStyleChangedCallCount);
            Assert.Equal(0, parentCreatedCallCount);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    [WinFormsFact]
    public void Control_AutoSize_SetWithHandler_CallsAutoSizeChanged()
    {
        using Control control = new()
        {
            AutoSize = true
        };
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        control.AutoSizeChanged += handler;

        // Set different.
        control.AutoSize = false;
        Assert.False(control.AutoSize);
        Assert.Equal(1, callCount);

        // Set same.
        control.AutoSize = false;
        Assert.False(control.AutoSize);
        Assert.Equal(1, callCount);

        // Set different.
        control.AutoSize = true;
        Assert.True(control.AutoSize);
        Assert.Equal(2, callCount);

        // Remove handler.
        control.AutoSizeChanged -= handler;
        control.AutoSize = false;
        Assert.False(control.AutoSize);
        Assert.Equal(2, callCount);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetBackColorTheoryData))]
    public void Control_BackColor_Set_GetReturnsExpected(Color value, Color expected)
    {
        using Control control = new()
        {
            BackColor = value
        };
        Assert.Equal(expected, control.BackColor);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.BackColor = value;
        Assert.Equal(expected, control.BackColor);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetBackColorTheoryData))]
    public void Control_BackColor_SetWithCustomOldValue_GetReturnsExpected(Color value, Color expected)
    {
        using Control control = new()
        {
            BackColor = Color.YellowGreen
        };

        control.BackColor = value;
        Assert.Equal(expected, control.BackColor);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.BackColor = value;
        Assert.Equal(expected, control.BackColor);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> BackColor_SetTransparent_TestData()
    {
        yield return new object[] { Color.Red, Color.Red };
        yield return new object[] { Color.FromArgb(254, 1, 2, 3), Color.FromArgb(254, 1, 2, 3) };
        yield return new object[] { Color.Empty, Control.DefaultBackColor };
    }

    [WinFormsTheory]
    [MemberData(nameof(BackColor_SetTransparent_TestData))]
    public void Control_BackColor_SetTransparent_GetReturnsExpected(Color value, Color expected)
    {
        using SubControl control = new();
        control.SetStyle(ControlStyles.SupportsTransparentBackColor, true);

        control.BackColor = value;
        Assert.Equal(expected, control.BackColor);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.BackColor = value;
        Assert.Equal(expected, control.BackColor);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetBackColorTheoryData))]
    public void Control_BackColor_SetWithChildren_GetReturnsExpected(Color value, Color expected)
    {
        using Control child1 = new();
        using Control child2 = new();
        using Control control = new();
        control.Controls.Add(child1);
        control.Controls.Add(child2);

        control.BackColor = value;
        Assert.Equal(expected, control.BackColor);
        Assert.Equal(expected, child1.BackColor);
        Assert.Equal(expected, child2.BackColor);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.BackColor = value;
        Assert.Equal(expected, control.BackColor);
        Assert.Equal(expected, child1.BackColor);
        Assert.Equal(expected, child2.BackColor);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetBackColorTheoryData))]
    public void Control_BackColor_SetWithChildrenWithColor_GetReturnsExpected(Color value, Color expected)
    {
        using Control child1 = new()
        {
            BackColor = Color.Yellow
        };
        using Control child2 = new()
        {
            BackColor = Color.YellowGreen
        };
        using Control control = new();
        control.Controls.Add(child1);
        control.Controls.Add(child2);

        control.BackColor = value;
        Assert.Equal(expected, control.BackColor);
        Assert.Equal(Color.Yellow, child1.BackColor);
        Assert.Equal(Color.YellowGreen, child2.BackColor);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.BackColor = value;
        Assert.Equal(expected, control.BackColor);
        Assert.Equal(Color.Yellow, child1.BackColor);
        Assert.Equal(Color.YellowGreen, child2.BackColor);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> BackColor_SetWithHandle_TestData()
    {
        yield return new object[] { Color.Red, Color.Red, 1 };
        yield return new object[] { Color.Empty, Control.DefaultBackColor, 0 };
    }

    [WinFormsTheory]
    [MemberData(nameof(BackColor_SetWithHandle_TestData))]
    public void Control_BackColor_SetWithHandle_GetReturnsExpected(Color value, Color expected, int expectedInvalidatedCallCount)
    {
        using Control control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.BackColor = value;
        Assert.Equal(expected, control.BackColor);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.BackColor = value;
        Assert.Equal(expected, control.BackColor);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void Control_BackColor_SetWithHandler_CallsBackColorChanged()
    {
        using Control control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        control.BackColorChanged += handler;

        // Set different.
        control.BackColor = Color.Red;
        Assert.Equal(Color.Red, control.BackColor);
        Assert.Equal(1, callCount);

        // Set same.
        control.BackColor = Color.Red;
        Assert.Equal(Color.Red, control.BackColor);
        Assert.Equal(1, callCount);

        // Set different.
        control.BackColor = Color.Empty;
        Assert.Equal(Control.DefaultBackColor, control.BackColor);
        Assert.Equal(2, callCount);

        // Remove handler.
        control.BackColorChanged -= handler;
        control.BackColor = Color.Red;
        Assert.Equal(Color.Red, control.BackColor);
        Assert.Equal(2, callCount);
    }

    [WinFormsFact]
    public void Control_BackColor_SetWithHandlerInDisposing_DoesNotCallBackColorChanged()
    {
        using Control control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);

        int callCount = 0;
        control.BackColorChanged += (sender, e) => callCount++;
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;

        int disposedCallCount = 0;
        control.Disposed += (sender, e) =>
        {
            control.BackColor = Color.Red;
            Assert.Equal(Color.Red, control.BackColor);
            Assert.Equal(0, callCount);
            Assert.Equal(0, invalidatedCallCount);
            disposedCallCount++;
        };

        control.Dispose();
        Assert.Equal(1, disposedCallCount);
    }

    [WinFormsFact]
    public void Control_BackColor_SetWithChildrenWithHandler_CallsBackColorChanged()
    {
        using Control child1 = new();
        using Control child2 = new();
        using Control control = new();
        control.Controls.Add(child1);
        control.Controls.Add(child2);

        int callCount = 0;
        int childCallCount1 = 0;
        int childCallCount2 = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(callCount, childCallCount1);
            Assert.Equal(childCallCount1, childCallCount2);
            callCount++;
        };
        EventHandler childHandler1 = (sender, e) =>
        {
            Assert.Same(child1, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(callCount - 1, childCallCount1);
            Assert.Equal(childCallCount1, childCallCount2);
            childCallCount1++;
        };
        EventHandler childHandler2 = (sender, e) =>
        {
            Assert.Same(child2, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(callCount, childCallCount1);
            Assert.Equal(childCallCount1 - 1, childCallCount2);
            childCallCount2++;
        };
        control.BackColorChanged += handler;
        child1.BackColorChanged += childHandler1;
        child2.BackColorChanged += childHandler2;

        // Set different.
        control.BackColor = Color.Red;
        Assert.Equal(Color.Red, control.BackColor);
        Assert.Equal(Color.Red, child1.BackColor);
        Assert.Equal(Color.Red, child2.BackColor);
        Assert.Equal(1, callCount);
        Assert.Equal(1, childCallCount1);
        Assert.Equal(1, childCallCount2);

        // Set same.
        control.BackColor = Color.Red;
        Assert.Equal(Color.Red, control.BackColor);
        Assert.Equal(Color.Red, child1.BackColor);
        Assert.Equal(Color.Red, child2.BackColor);
        Assert.Equal(1, callCount);
        Assert.Equal(1, childCallCount1);
        Assert.Equal(1, childCallCount2);

        // Set different.
        control.BackColor = Color.Empty;
        Assert.Equal(Control.DefaultBackColor, control.BackColor);
        Assert.Equal(Control.DefaultBackColor, child1.BackColor);
        Assert.Equal(Control.DefaultBackColor, child2.BackColor);
        Assert.Equal(2, callCount);
        Assert.Equal(2, childCallCount1);
        Assert.Equal(2, childCallCount2);

        // Remove handler.
        control.BackColorChanged -= handler;
        child1.BackColorChanged -= childHandler1;
        child2.BackColorChanged -= childHandler2;
        control.BackColor = Color.Red;
        Assert.Equal(Color.Red, control.BackColor);
        Assert.Equal(Color.Red, child1.BackColor);
        Assert.Equal(Color.Red, child2.BackColor);
        Assert.Equal(2, callCount);
        Assert.Equal(2, childCallCount1);
        Assert.Equal(2, childCallCount2);
    }

    [WinFormsFact]
    public void Control_BackColor_SetWithChildrenWithBackColorWithHandler_CallsBackColorChanged()
    {
        using Control child1 = new()
        {
            BackColor = Color.Yellow
        };
        using Control child2 = new()
        {
            BackColor = Color.YellowGreen
        };
        using Control control = new();
        control.Controls.Add(child1);
        control.Controls.Add(child2);

        int callCount = 0;
        int childCallCount1 = 0;
        int childCallCount2 = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        EventHandler childHandler1 = (sender, e) =>
        {
            Assert.Same(child1, sender);
            Assert.Same(EventArgs.Empty, e);
            childCallCount1++;
        };
        EventHandler childHandler2 = (sender, e) =>
        {
            Assert.Same(child2, sender);
            Assert.Same(EventArgs.Empty, e);
            childCallCount2++;
        };
        control.BackColorChanged += handler;
        child1.BackColorChanged += childHandler1;
        child2.BackColorChanged += childHandler2;

        // Set different.
        control.BackColor = Color.Red;
        Assert.Equal(Color.Red, control.BackColor);
        Assert.Equal(Color.Yellow, child1.BackColor);
        Assert.Equal(Color.YellowGreen, child2.BackColor);
        Assert.Equal(1, callCount);
        Assert.Equal(0, childCallCount1);
        Assert.Equal(0, childCallCount2);

        // Set same.
        control.BackColor = Color.Red;
        Assert.Equal(Color.Red, control.BackColor);
        Assert.Equal(Color.Yellow, child1.BackColor);
        Assert.Equal(Color.YellowGreen, child2.BackColor);
        Assert.Equal(1, callCount);
        Assert.Equal(0, childCallCount1);
        Assert.Equal(0, childCallCount2);

        // Set different.
        control.BackColor = Color.Empty;
        Assert.Equal(Control.DefaultBackColor, control.BackColor);
        Assert.Equal(Color.Yellow, child1.BackColor);
        Assert.Equal(Color.YellowGreen, child2.BackColor);
        Assert.Equal(2, callCount);
        Assert.Equal(0, childCallCount1);
        Assert.Equal(0, childCallCount2);

        // Remove handler.
        control.BackColorChanged -= handler;
        child1.BackColorChanged -= childHandler1;
        child2.BackColorChanged -= childHandler2;
        control.BackColor = Color.Red;
        Assert.Equal(Color.Red, control.BackColor);
        Assert.Equal(Color.Yellow, child1.BackColor);
        Assert.Equal(Color.YellowGreen, child2.BackColor);
        Assert.Equal(2, callCount);
        Assert.Equal(0, childCallCount1);
        Assert.Equal(0, childCallCount2);
    }

    [WinFormsFact]
    public void Control_BackColor_SetTransparent_ThrowsArgmentException()
    {
        using Control control = new();
        Assert.Throws<ArgumentException>(() => control.BackColor = Color.FromArgb(254, 1, 2, 3));
    }

    [WinFormsFact]
    public void Control_BackColor_ResetValue_Success()
    {
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(Control))[nameof(Control.BackColor)];
        using Control control = new();
        Assert.False(property.CanResetValue(control));

        control.BackColor = Color.Red;
        Assert.Equal(Color.Red, control.BackColor);
        Assert.True(property.CanResetValue(control));

        property.ResetValue(control);
        Assert.Equal(Control.DefaultBackColor, control.BackColor);
        Assert.False(property.CanResetValue(control));
    }

    [WinFormsFact]
    public void Control_BackColor_ShouldSerializeValue_Success()
    {
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(Control))[nameof(Control.BackColor)];
        using Control control = new();
        Assert.False(property.ShouldSerializeValue(control));

        control.BackColor = Color.Red;
        Assert.Equal(Color.Red, control.BackColor);
        Assert.True(property.ShouldSerializeValue(control));

        property.ResetValue(control);
        Assert.Equal(Control.DefaultBackColor, control.BackColor);
        Assert.False(property.ShouldSerializeValue(control));
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetImageTheoryData))]
    public void Control_BackgroundImage_Set_GetReturnsExpected(Image value)
    {
        using Control control = new()
        {
            BackgroundImage = value
        };
        Assert.Same(value, control.BackgroundImage);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.BackgroundImage = value;
        Assert.Same(value, control.BackgroundImage);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> BackgroundImage_SetWithHandle_TestData()
    {
        yield return new object[] { new Bitmap(10, 10), 1 };
        yield return new object[] { null, 0 };
    }

    [WinFormsTheory]
    [MemberData(nameof(BackgroundImage_SetWithHandle_TestData))]
    public void Control_BackgroundImage_SetWithHandle_GetReturnsExpected(Image value, int expectedInvalidatedCallCount)
    {
        using Control control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.BackgroundImage = value;
        Assert.Same(value, control.BackgroundImage);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.BackgroundImage = value;
        Assert.Same(value, control.BackgroundImage);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetImageTheoryData))]
    public void Control_BackgroundImage_SetWithChildren_GetReturnsExpected(Image value)
    {
        using Control child1 = new();
        using Control child2 = new();
        using Control control = new();
        control.Controls.Add(child1);
        control.Controls.Add(child2);

        control.BackgroundImage = value;
        Assert.Same(value, control.BackgroundImage);
        Assert.Null(child1.BackgroundImage);
        Assert.Null(child2.BackgroundImage);

        // Set same.
        control.BackgroundImage = value;
        Assert.Same(value, control.BackgroundImage);
        Assert.Null(child1.BackgroundImage);
        Assert.Null(child2.BackgroundImage);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetImageTheoryData))]
    public void Control_BackgroundImage_SetWithChildrenWithBackgroundImage_GetReturnsExpected(Image value)
    {
        using Bitmap image1 = new(10, 10);
        using Bitmap image2 = new(10, 10);
        using Control child1 = new()
        {
            BackgroundImage = image1
        };
        using Control child2 = new()
        {
            BackgroundImage = image2
        };
        using Control control = new();
        control.Controls.Add(child1);
        control.Controls.Add(child2);

        control.BackgroundImage = value;
        Assert.Same(value, control.BackgroundImage);
        Assert.Same(image1, child1.BackgroundImage);
        Assert.Same(image2, child2.BackgroundImage);

        // Set same.
        control.BackgroundImage = value;
        Assert.Same(value, control.BackgroundImage);
        Assert.Same(image1, child1.BackgroundImage);
        Assert.Same(image2, child2.BackgroundImage);
    }

    [WinFormsFact]
    public void Control_BackgroundImage_SetWithHandler_CallsBackgroundImageChanged()
    {
        using Control control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        control.BackgroundImageChanged += handler;

        // Set different.
        Bitmap image1 = new(10, 10);
        control.BackgroundImage = image1;
        Assert.Same(image1, control.BackgroundImage);
        Assert.Equal(1, callCount);

        // Set same.
        control.BackgroundImage = image1;
        Assert.Same(image1, control.BackgroundImage);
        Assert.Equal(1, callCount);

        // Set different.
        Bitmap image2 = new(10, 10);
        control.BackgroundImage = image2;
        Assert.Same(image2, control.BackgroundImage);
        Assert.Equal(2, callCount);

        // Set null.
        control.BackgroundImage = null;
        Assert.Null(control.BackgroundImage);
        Assert.Equal(3, callCount);

        // Remove handler.
        control.BackgroundImageChanged -= handler;
        control.BackgroundImage = image1;
        Assert.Same(image1, control.BackgroundImage);
        Assert.Equal(3, callCount);
    }

    [WinFormsFact]
    public void Control_BackgroundImage_SetWithHandlerInDisposing_DoesNotCallBackgroundImageChanged()
    {
        using Control control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);

        int callCount = 0;
        control.BackgroundImageChanged += (sender, e) => callCount++;
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;

        int disposedCallCount = 0;
        control.Disposed += (sender, e) =>
        {
            Bitmap value = new(10, 10);
            control.BackgroundImage = value;
            Assert.Same(value, control.BackgroundImage);
            Assert.Equal(0, callCount);
            Assert.Equal(0, invalidatedCallCount);
            disposedCallCount++;
        };

        control.Dispose();
        Assert.Equal(1, disposedCallCount);
    }

    [WinFormsFact]
    public void Control_BackgroundImage_SetWithChildrenWithHandler_CallsBackgroundImageChanged()
    {
        using Control child1 = new();
        using Control child2 = new();
        using Control control = new();
        control.Controls.Add(child1);
        control.Controls.Add(child2);

        int callCount = 0;
        int child1CallCount = 0;
        int child2CallCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        EventHandler childHandler1 = (sender, e) =>
        {
            Assert.Same(child1, sender);
            Assert.Same(EventArgs.Empty, e);
            child1CallCount++;
        };
        EventHandler childHandler2 = (sender, e) =>
        {
            Assert.Same(child2, sender);
            Assert.Same(EventArgs.Empty, e);
            child2CallCount++;
        };
        control.BackgroundImageChanged += handler;
        child1.BackgroundImageChanged += childHandler1;
        child2.BackgroundImageChanged += childHandler2;

        // Set different.
        Bitmap image1 = new(10, 10);
        control.BackgroundImage = image1;
        Assert.Same(image1, control.BackgroundImage);
        Assert.Null(child1.BackgroundImage);
        Assert.Null(child2.BackgroundImage);
        Assert.Equal(1, callCount);
        Assert.Equal(1, child1CallCount);
        Assert.Equal(1, child2CallCount);

        // Set same.
        control.BackgroundImage = image1;
        Assert.Same(image1, control.BackgroundImage);
        Assert.Null(child1.BackgroundImage);
        Assert.Null(child2.BackgroundImage);
        Assert.Equal(1, callCount);
        Assert.Equal(1, child1CallCount);
        Assert.Equal(1, child2CallCount);

        // Set different.
        Bitmap image2 = new(10, 10);
        control.BackgroundImage = image2;
        Assert.Same(image2, control.BackgroundImage);
        Assert.Null(child1.BackgroundImage);
        Assert.Null(child2.BackgroundImage);
        Assert.Equal(2, callCount);
        Assert.Equal(2, child1CallCount);
        Assert.Equal(2, child2CallCount);

        // Set null.
        control.BackgroundImage = null;
        Assert.Null(control.BackgroundImage);
        Assert.Null(child1.BackgroundImage);
        Assert.Null(child2.BackgroundImage);
        Assert.Equal(3, callCount);
        Assert.Equal(3, child1CallCount);
        Assert.Equal(3, child2CallCount);

        // Remove handler.
        control.BackgroundImageChanged -= handler;
        child1.BackgroundImageChanged -= childHandler1;
        child2.BackgroundImageChanged -= childHandler2;
        control.BackgroundImage = image1;
        Assert.Same(image1, control.BackgroundImage);
        Assert.Null(child1.BackgroundImage);
        Assert.Null(child2.BackgroundImage);
        Assert.Equal(3, callCount);
        Assert.Equal(3, child1CallCount);
        Assert.Equal(3, child2CallCount);
    }

    [WinFormsFact]
    public void Control_BackgroundImage_SetWithChildrenWithBackgroundImageWithHandler_CallsBackgroundImageChanged()
    {
        using Bitmap childBackgroundImage1 = new(10, 10);
        using Bitmap childBackgroundImage2 = new(10, 10);
        using Control child1 = new()
        {
            BackgroundImage = childBackgroundImage1
        };
        using Control child2 = new()
        {
            BackgroundImage = childBackgroundImage2
        };
        using Control control = new();
        control.Controls.Add(child1);
        control.Controls.Add(child2);

        int callCount = 0;
        int child1CallCount = 0;
        int child2CallCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        EventHandler childHandler1 = (sender, e) =>
        {
            Assert.Same(child1, sender);
            Assert.Same(EventArgs.Empty, e);
            child1CallCount++;
        };
        EventHandler childHandler2 = (sender, e) =>
        {
            Assert.Same(child2, sender);
            Assert.Same(EventArgs.Empty, e);
            child2CallCount++;
        };
        control.BackgroundImageChanged += handler;
        child1.BackgroundImageChanged += childHandler1;
        child2.BackgroundImageChanged += childHandler2;

        // Set different.
        Bitmap image1 = new(10, 10);
        control.BackgroundImage = image1;
        Assert.Same(image1, control.BackgroundImage);
        Assert.Same(childBackgroundImage1, child1.BackgroundImage);
        Assert.Same(childBackgroundImage2, child2.BackgroundImage);
        Assert.Equal(1, callCount);
        Assert.Equal(1, child1CallCount);
        Assert.Equal(1, child2CallCount);

        // Set same.
        control.BackgroundImage = image1;
        Assert.Same(image1, control.BackgroundImage);
        Assert.Same(childBackgroundImage1, child1.BackgroundImage);
        Assert.Same(childBackgroundImage2, child2.BackgroundImage);
        Assert.Equal(1, callCount);
        Assert.Equal(1, child1CallCount);
        Assert.Equal(1, child2CallCount);

        // Set different.
        Bitmap image2 = new(10, 10);
        control.BackgroundImage = image2;
        Assert.Same(image2, control.BackgroundImage);
        Assert.Same(childBackgroundImage1, child1.BackgroundImage);
        Assert.Same(childBackgroundImage2, child2.BackgroundImage);
        Assert.Equal(2, callCount);
        Assert.Equal(2, child1CallCount);
        Assert.Equal(2, child2CallCount);

        // Set null.
        control.BackgroundImage = null;
        Assert.Null(control.BackgroundImage);
        Assert.Same(childBackgroundImage1, child1.BackgroundImage);
        Assert.Same(childBackgroundImage2, child2.BackgroundImage);
        Assert.Equal(3, callCount);
        Assert.Equal(3, child1CallCount);
        Assert.Equal(3, child2CallCount);

        // Remove handler.
        control.BackgroundImageChanged -= handler;
        child1.BackgroundImageChanged -= childHandler1;
        child2.BackgroundImageChanged -= childHandler2;
        control.BackgroundImage = image1;
        Assert.Same(image1, control.BackgroundImage);
        Assert.Same(childBackgroundImage1, child1.BackgroundImage);
        Assert.Same(childBackgroundImage2, child2.BackgroundImage);
        Assert.Equal(3, callCount);
        Assert.Equal(3, child1CallCount);
        Assert.Equal(3, child2CallCount);
    }

    [WinFormsTheory]
    [EnumData<ImageLayout>]
    public void Control_BackgroundImageLayout_Set_GetReturnsExpected(ImageLayout value)
    {
        using SubControl control = new()
        {
            BackgroundImageLayout = value
        };
        Assert.Equal(value, control.BackgroundImageLayout);
        Assert.False(control.DoubleBuffered);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.BackgroundImageLayout = value;
        Assert.Equal(value, control.BackgroundImageLayout);
        Assert.False(control.DoubleBuffered);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(ImageLayout.Center, 1)]
    [InlineData(ImageLayout.None, 1)]
    [InlineData(ImageLayout.Stretch, 1)]
    [InlineData(ImageLayout.Tile, 0)]
    [InlineData(ImageLayout.Zoom, 1)]
    public void Control_BackgroundImageLayout_SetWithHandle_GetReturnsExpected(ImageLayout value, int expectedInvalidatedCallCount)
    {
        using SubControl control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.BackgroundImageLayout = value;
        Assert.Equal(value, control.BackgroundImageLayout);
        Assert.False(control.DoubleBuffered);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.BackgroundImageLayout = value;
        Assert.Equal(value, control.BackgroundImageLayout);
        Assert.False(control.DoubleBuffered);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> BackgroundImageLayout_SetWithBackgroundImage_TestData()
    {
        yield return new object[] { new Bitmap(10, 10, PixelFormat.Format32bppArgb), ImageLayout.None, false };
        yield return new object[] { new Bitmap(10, 10, PixelFormat.Format32bppArgb), ImageLayout.Tile, false };
        yield return new object[] { new Bitmap(10, 10, PixelFormat.Format32bppArgb), ImageLayout.Center, true };
        yield return new object[] { new Bitmap(10, 10, PixelFormat.Format32bppArgb), ImageLayout.Stretch, true };
        yield return new object[] { new Bitmap(10, 10, PixelFormat.Format32bppArgb), ImageLayout.Zoom, true };

        yield return new object[] { new Bitmap(10, 10, PixelFormat.Format32bppRgb), ImageLayout.None, false };
        yield return new object[] { new Bitmap(10, 10, PixelFormat.Format32bppRgb), ImageLayout.Tile, false };
        yield return new object[] { new Bitmap(10, 10, PixelFormat.Format32bppRgb), ImageLayout.Center, false };
        yield return new object[] { new Bitmap(10, 10, PixelFormat.Format32bppRgb), ImageLayout.Stretch, false };
        yield return new object[] { new Bitmap(10, 10, PixelFormat.Format32bppRgb), ImageLayout.Zoom, false };
    }

    [WinFormsTheory]
    [MemberData(nameof(BackgroundImageLayout_SetWithBackgroundImage_TestData))]
    public void Control_BackgroundImageLayout_SetWithBackgroundImage_GetReturnsExpected(Image backgroundImage, ImageLayout value, bool expectedDoubleBuffered)
    {
        using SubControl control = new()
        {
            BackgroundImage = backgroundImage,
            BackgroundImageLayout = value
        };
        Assert.Equal(value, control.BackgroundImageLayout);
        Assert.Equal(expectedDoubleBuffered, control.DoubleBuffered);

        // Set same.
        control.BackgroundImageLayout = value;
        Assert.Equal(value, control.BackgroundImageLayout);
        Assert.Equal(expectedDoubleBuffered, control.DoubleBuffered);
    }

    [WinFormsFact]
    public void Control_BackgroundImageLayout_SetWithHandler_CallsBackgroundImageLayoutChanged()
    {
        using Control control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        control.BackgroundImageLayoutChanged += handler;

        // Set different.
        control.BackgroundImageLayout = ImageLayout.Center;
        Assert.Equal(ImageLayout.Center, control.BackgroundImageLayout);
        Assert.Equal(1, callCount);

        // Set same.
        control.BackgroundImageLayout = ImageLayout.Center;
        Assert.Equal(ImageLayout.Center, control.BackgroundImageLayout);
        Assert.Equal(1, callCount);

        // Set different.
        control.BackgroundImageLayout = ImageLayout.Stretch;
        Assert.Equal(ImageLayout.Stretch, control.BackgroundImageLayout);
        Assert.Equal(2, callCount);

        // Remove handler.
        control.BackgroundImageLayoutChanged -= handler;
        control.BackgroundImageLayout = ImageLayout.Center;
        Assert.Equal(ImageLayout.Center, control.BackgroundImageLayout);
        Assert.Equal(2, callCount);
    }

    [WinFormsFact]
    public void Control_BackgroundImageLayout_SetWithHandlerInDisposing_DoesNotCallBackgroundImageLayoutChanged()
    {
        using Control control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);

        int callCount = 0;
        control.BackgroundImageLayoutChanged += (sender, e) => callCount++;
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;

        int disposedCallCount = 0;
        control.Disposed += (sender, e) =>
        {
            control.BackgroundImageLayout = ImageLayout.Center;
            Assert.Equal(ImageLayout.Center, control.BackgroundImageLayout);
            Assert.Equal(0, callCount);
            Assert.Equal(0, invalidatedCallCount);
            disposedCallCount++;
        };

        control.Dispose();
        Assert.Equal(1, disposedCallCount);
    }

    [WinFormsTheory]
    [InvalidEnumData<ImageLayout>]
    public void Control_BackgroundImageLayout_SetInvalid_ThrowsInvalidEnumArgumentException(ImageLayout value)
    {
        using Control control = new();
        Assert.Throws<InvalidEnumArgumentException>("value", () => control.BackgroundImageLayout = value);
    }

    [WinFormsFact]
    public void Control_BindingContext_GetWithParent_ReturnsExpected()
    {
        BindingContext bindingContext = [];
        using Control parent = new()
        {
            BindingContext = bindingContext
        };
        using Control control = new()
        {
            Parent = parent
        };
        Assert.Same(bindingContext, control.BindingContext);
    }

    [WinFormsFact]
    public void Control_BindingContext_GetWithParentCantAccessProperties_ReturnsExpected()
    {
        BindingContext bindingContext = [];
        using SubAxHost parent = new("00000000-0000-0000-0000-000000000000")
        {
            BindingContext = bindingContext
        };
        using Control control = new()
        {
            Parent = parent
        };
        Assert.Null(control.BindingContext);
    }

    public static IEnumerable<object[]> BindingContext_Set_TestData()
    {
        yield return new object[] { null };
        yield return new object[] { new BindingContext() };
    }

    [WinFormsTheory]
    [MemberData(nameof(BindingContext_Set_TestData))]
    public void Control_BindingContext_Set_GetReturnsExpected(BindingContext value)
    {
        using Control control = new()
        {
            BindingContext = value
        };
        Assert.Same(value, control.BindingContext);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.BindingContext = value;
        Assert.Same(value, control.BindingContext);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(BindingContext_Set_TestData))]
    public void Control_BindingContext_SetWithNonNullBindingContext_GetReturnsExpected(BindingContext value)
    {
        using Control control = new()
        {
            BindingContext = []
        };

        control.BindingContext = value;
        Assert.Same(value, control.BindingContext);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.BindingContext = value;
        Assert.Same(value, control.BindingContext);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void Control_BindingContext_SetWithHandler_CallsBindingContextChanged()
    {
        using Control control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        control.BindingContextChanged += handler;

        // Set different.
        BindingContext context1 = [];
        control.BindingContext = context1;
        Assert.Same(context1, control.BindingContext);
        Assert.Equal(1, callCount);

        // Set same.
        control.BindingContext = context1;
        Assert.Same(context1, control.BindingContext);
        Assert.Equal(1, callCount);

        // Set different.
        BindingContext context2 = [];
        control.BindingContext = context2;
        Assert.Same(context2, control.BindingContext);
        Assert.Equal(2, callCount);

        // Set null.
        control.BindingContext = null;
        Assert.Null(control.BindingContext);
        Assert.Equal(3, callCount);

        // Remove handler.
        control.BindingContextChanged -= handler;
        control.BindingContext = context1;
        Assert.Same(context1, control.BindingContext);
        Assert.Equal(3, callCount);
    }

    [WinFormsFact]
    public void Control_BindingContext_SetWithChildrenWithHandler_CallsBindingContextChanged()
    {
        using Control child1 = new();
        using Control child2 = new();
        using Control control = new();
        control.Controls.Add(child1);
        control.Controls.Add(child2);

        int callCount = 0;
        int childCallCount1 = 0;
        int childCallCount2 = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        EventHandler childHandler1 = (sender, e) =>
        {
            Assert.Same(child1, sender);
            Assert.Same(EventArgs.Empty, e);
            childCallCount1++;
        };
        EventHandler childHandler2 = (sender, e) =>
        {
            Assert.Same(child2, sender);
            Assert.Same(EventArgs.Empty, e);
            childCallCount2++;
        };
        control.BindingContextChanged += handler;
        child1.BindingContextChanged += childHandler1;
        child2.BindingContextChanged += childHandler2;

        // Set different.
        BindingContext context1 = [];
        control.BindingContext = context1;
        Assert.Same(context1, control.BindingContext);
        Assert.Same(context1, child1.BindingContext);
        Assert.Same(context1, child2.BindingContext);
        Assert.Equal(1, callCount);
        Assert.Equal(1, childCallCount1);
        Assert.Equal(1, childCallCount2);

        // Set same.
        control.BindingContext = context1;
        Assert.Same(context1, control.BindingContext);
        Assert.Same(context1, child1.BindingContext);
        Assert.Same(context1, child2.BindingContext);
        Assert.Equal(1, callCount);
        Assert.Equal(1, childCallCount1);
        Assert.Equal(1, childCallCount2);

        // Set different.
        BindingContext context2 = [];
        control.BindingContext = context2;
        Assert.Same(context2, control.BindingContext);
        Assert.Same(context2, child1.BindingContext);
        Assert.Same(context2, child2.BindingContext);
        Assert.Equal(2, callCount);
        Assert.Equal(2, childCallCount1);
        Assert.Equal(2, childCallCount2);

        // Set null.
        control.BindingContext = null;
        Assert.Null(control.BindingContext);
        Assert.Null(child1.BindingContext);
        Assert.Null(child2.BindingContext);
        Assert.Equal(3, callCount);
        Assert.Equal(3, childCallCount1);
        Assert.Equal(3, childCallCount2);

        // Remove handler.
        control.BindingContextChanged -= handler;
        child1.BindingContextChanged -= childHandler1;
        child2.BindingContextChanged -= childHandler2;
        control.BindingContext = context1;
        Assert.Same(context1, control.BindingContext);
        Assert.Same(context1, child1.BindingContext);
        Assert.Same(context1, child2.BindingContext);
        Assert.Equal(3, callCount);
        Assert.Equal(3, childCallCount1);
        Assert.Equal(3, childCallCount2);
    }

    [WinFormsFact]
    public void Control_BindingContext_SetWithChildrenWithBindingContextWithHandler_CallsBindingContextChanged()
    {
        BindingContext childContext1 = [];
        BindingContext childContext2 = [];
        using Control child1 = new()
        {
            BindingContext = childContext1
        };
        using Control child2 = new()
        {
            BindingContext = childContext2
        };
        using Control control = new();
        control.Controls.Add(child1);
        control.Controls.Add(child2);

        int callCount = 0;
        int childCallCount1 = 0;
        int childCallCount2 = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        EventHandler childHandler1 = (sender, e) =>
        {
            Assert.Same(child1, sender);
            Assert.Same(EventArgs.Empty, e);
            childCallCount1++;
        };
        EventHandler childHandler2 = (sender, e) =>
        {
            Assert.Same(child2, sender);
            Assert.Same(EventArgs.Empty, e);
            childCallCount2++;
        };
        control.BindingContextChanged += handler;
        child1.BindingContextChanged += childHandler1;
        child2.BindingContextChanged += childHandler2;

        // Set different.
        BindingContext context1 = [];
        control.BindingContext = context1;
        Assert.Same(context1, control.BindingContext);
        Assert.Same(childContext1, child1.BindingContext);
        Assert.Same(childContext2, child2.BindingContext);
        Assert.Equal(1, callCount);
        Assert.Equal(0, childCallCount1);
        Assert.Equal(0, childCallCount2);

        // Set same.
        control.BindingContext = context1;
        Assert.Same(context1, control.BindingContext);
        Assert.Same(childContext1, child1.BindingContext);
        Assert.Same(childContext2, child2.BindingContext);
        Assert.Equal(1, callCount);
        Assert.Equal(0, childCallCount1);
        Assert.Equal(0, childCallCount2);

        // Set different.
        BindingContext context2 = [];
        control.BindingContext = context2;
        Assert.Same(context2, control.BindingContext);
        Assert.Same(childContext1, child1.BindingContext);
        Assert.Same(childContext2, child2.BindingContext);
        Assert.Equal(2, callCount);
        Assert.Equal(0, childCallCount1);
        Assert.Equal(0, childCallCount2);

        // Set null.
        control.BindingContext = null;
        Assert.Null(control.BindingContext);
        Assert.Same(childContext1, child1.BindingContext);
        Assert.Same(childContext2, child2.BindingContext);
        Assert.Equal(3, callCount);
        Assert.Equal(0, childCallCount1);
        Assert.Equal(0, childCallCount2);

        // Remove handler.
        control.BindingContextChanged -= handler;
        child1.BindingContextChanged -= childHandler1;
        child2.BindingContextChanged -= childHandler2;
        control.BindingContext = context1;
        Assert.Same(context1, control.BindingContext);
        Assert.Same(childContext1, child1.BindingContext);
        Assert.Same(childContext2, child2.BindingContext);
        Assert.Equal(3, callCount);
        Assert.Equal(0, childCallCount1);
        Assert.Equal(0, childCallCount2);
    }

    [WinFormsTheory]
    [MemberData(nameof(SetBounds_Int_Int_Int_Int_TestData))]
    public void Control_Bounds_Set_GetReturnsExpected(int x, int y, int width, int height, int expectedLayoutCallCount)
    {
        using SubControl control = new();
        int layoutCallCount = 0;
        int resizeCallCount = 0;
        int sizeChangedCallCount = 0;
        int clientSizeChangedCallCount = 0;
        control.Layout += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("Bounds", e.AffectedProperty);
            Assert.Equal(resizeCallCount, layoutCallCount);
            Assert.Equal(sizeChangedCallCount, layoutCallCount);
            Assert.Equal(clientSizeChangedCallCount, layoutCallCount);
            layoutCallCount++;
        };
        control.Resize += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(layoutCallCount - 1, resizeCallCount);
            Assert.Equal(sizeChangedCallCount, resizeCallCount);
            Assert.Equal(clientSizeChangedCallCount, resizeCallCount);
            resizeCallCount++;
        };
        control.SizeChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(resizeCallCount - 1, sizeChangedCallCount);
            Assert.Equal(layoutCallCount - 1, sizeChangedCallCount);
            Assert.Equal(clientSizeChangedCallCount, sizeChangedCallCount);
            sizeChangedCallCount++;
        };
        control.ClientSizeChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(resizeCallCount - 1, clientSizeChangedCallCount);
            Assert.Equal(layoutCallCount - 1, clientSizeChangedCallCount);
            Assert.Equal(sizeChangedCallCount - 1, clientSizeChangedCallCount);
            clientSizeChangedCallCount++;
        };

        control.Bounds = new Rectangle(x, y, width, height);
        Assert.Equal(new Size(width, height), control.ClientSize);
        Assert.Equal(new Rectangle(0, 0, width, height), control.ClientRectangle);
        Assert.Equal(new Rectangle(0, 0, width, height), control.DisplayRectangle);
        Assert.Equal(new Size(width, height), control.Size);
        Assert.Equal(x, control.Left);
        Assert.Equal(x + width, control.Right);
        Assert.Equal(y, control.Top);
        Assert.Equal(y + height, control.Bottom);
        Assert.Equal(width, control.Width);
        Assert.Equal(height, control.Height);
        Assert.Equal(new Rectangle(x, y, width, height), control.Bounds);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.Equal(expectedLayoutCallCount, resizeCallCount);
        Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
        Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
        Assert.False(control.IsHandleCreated);

        // Call again.
        control.Bounds = new Rectangle(x, y, width, height);
        Assert.Equal(new Size(width, height), control.ClientSize);
        Assert.Equal(new Rectangle(0, 0, width, height), control.ClientRectangle);
        Assert.Equal(new Rectangle(0, 0, width, height), control.DisplayRectangle);
        Assert.Equal(new Size(width, height), control.Size);
        Assert.Equal(x, control.Left);
        Assert.Equal(x + width, control.Right);
        Assert.Equal(y, control.Top);
        Assert.Equal(y + height, control.Bottom);
        Assert.Equal(width, control.Width);
        Assert.Equal(height, control.Height);
        Assert.Equal(new Rectangle(x, y, width, height), control.Bounds);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.Equal(expectedLayoutCallCount, resizeCallCount);
        Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
        Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(SetBounds_Int_Int_Int_Int_WithConstrainedSize_TestData))]
    public void Control_Bounds_SetWithConstrainedSize_GetReturnsExpected(Size minimumSize, Size maximumSize, int x, int y, int width, int height, int expectedWidth, int expectedHeight, int expectedLayoutCallCount)
    {
        using SubControl control = new()
        {
            MinimumSize = minimumSize,
            MaximumSize = maximumSize
        };
        int layoutCallCount = 0;
        int resizeCallCount = 0;
        int sizeChangedCallCount = 0;
        int clientSizeChangedCallCount = 0;
        control.Layout += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("Bounds", e.AffectedProperty);
            Assert.Equal(resizeCallCount, layoutCallCount);
            Assert.Equal(sizeChangedCallCount, layoutCallCount);
            Assert.Equal(clientSizeChangedCallCount, layoutCallCount);
            layoutCallCount++;
        };
        control.Resize += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(layoutCallCount - 1, resizeCallCount);
            Assert.Equal(sizeChangedCallCount, resizeCallCount);
            Assert.Equal(clientSizeChangedCallCount, resizeCallCount);
            resizeCallCount++;
        };
        control.SizeChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(resizeCallCount - 1, sizeChangedCallCount);
            Assert.Equal(layoutCallCount - 1, sizeChangedCallCount);
            Assert.Equal(clientSizeChangedCallCount, sizeChangedCallCount);
            sizeChangedCallCount++;
        };
        control.ClientSizeChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(resizeCallCount - 1, clientSizeChangedCallCount);
            Assert.Equal(layoutCallCount - 1, clientSizeChangedCallCount);
            Assert.Equal(sizeChangedCallCount - 1, clientSizeChangedCallCount);
            clientSizeChangedCallCount++;
        };

        control.Bounds = new Rectangle(x, y, width, height);
        Assert.Equal(new Size(expectedWidth, expectedHeight), control.ClientSize);
        Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.ClientRectangle);
        Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.DisplayRectangle);
        Assert.Equal(new Size(expectedWidth, expectedHeight), control.Size);
        Assert.Equal(x, control.Left);
        Assert.Equal(x + expectedWidth, control.Right);
        Assert.Equal(y, control.Top);
        Assert.Equal(y + expectedHeight, control.Bottom);
        Assert.Equal(expectedWidth, control.Width);
        Assert.Equal(expectedHeight, control.Height);
        Assert.Equal(new Rectangle(x, y, expectedWidth, expectedHeight), control.Bounds);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.Equal(expectedLayoutCallCount, resizeCallCount);
        Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
        Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
        Assert.False(control.IsHandleCreated);

        // Call again.
        control.Bounds = new Rectangle(x, y, width, height);
        Assert.Equal(new Size(expectedWidth, expectedHeight), control.ClientSize);
        Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.ClientRectangle);
        Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.DisplayRectangle);
        Assert.Equal(new Size(expectedWidth, expectedHeight), control.Size);
        Assert.Equal(x, control.Left);
        Assert.Equal(x + expectedWidth, control.Right);
        Assert.Equal(y, control.Top);
        Assert.Equal(y + expectedHeight, control.Bottom);
        Assert.Equal(expectedWidth, control.Width);
        Assert.Equal(expectedHeight, control.Height);
        Assert.Equal(new Rectangle(x, y, expectedWidth, expectedHeight), control.Bounds);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.Equal(expectedLayoutCallCount, resizeCallCount);
        Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
        Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(SetBounds_Int_Int_Int_Int_WithCustomStyle_TestData))]
    public void Control_Bounds_SetWithCustomStyle_GetReturnsExpected(int x, int y, int width, int height, int expectedClientWidth, int expectedClientHeight, int expectedLayoutCallCount)
    {
        using BorderedControl control = new();
        int layoutCallCount = 0;
        int resizeCallCount = 0;
        int sizeChangedCallCount = 0;
        int clientSizeChangedCallCount = 0;
        control.Layout += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("Bounds", e.AffectedProperty);
            Assert.Equal(resizeCallCount, layoutCallCount);
            Assert.Equal(sizeChangedCallCount, layoutCallCount);
            Assert.Equal(clientSizeChangedCallCount, layoutCallCount);
            layoutCallCount++;
        };
        control.Resize += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(layoutCallCount - 1, resizeCallCount);
            Assert.Equal(sizeChangedCallCount, resizeCallCount);
            Assert.Equal(clientSizeChangedCallCount, resizeCallCount);
            resizeCallCount++;
        };
        control.SizeChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(resizeCallCount - 1, sizeChangedCallCount);
            Assert.Equal(layoutCallCount - 1, sizeChangedCallCount);
            Assert.Equal(clientSizeChangedCallCount, sizeChangedCallCount);
            sizeChangedCallCount++;
        };
        control.ClientSizeChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(resizeCallCount - 1, clientSizeChangedCallCount);
            Assert.Equal(layoutCallCount - 1, clientSizeChangedCallCount);
            Assert.Equal(sizeChangedCallCount - 1, clientSizeChangedCallCount);
            clientSizeChangedCallCount++;
        };

        control.Bounds = new Rectangle(x, y, width, height);
        Assert.Equal(new Size(expectedClientWidth, expectedClientHeight), control.ClientSize);
        Assert.Equal(new Rectangle(0, 0, expectedClientWidth, expectedClientHeight), control.ClientRectangle);
        Assert.Equal(new Rectangle(0, 0, expectedClientWidth, expectedClientHeight), control.DisplayRectangle);
        Assert.Equal(new Size(width, height), control.Size);
        Assert.Equal(x, control.Left);
        Assert.Equal(x + width, control.Right);
        Assert.Equal(y, control.Top);
        Assert.Equal(y + height, control.Bottom);
        Assert.Equal(width, control.Width);
        Assert.Equal(height, control.Height);
        Assert.Equal(new Rectangle(x, y, width, height), control.Bounds);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.Equal(expectedLayoutCallCount, resizeCallCount);
        Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
        Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
        Assert.False(control.IsHandleCreated);

        // Call again.
        control.Bounds = new Rectangle(x, y, width, height);
        Assert.Equal(new Size(expectedClientWidth, expectedClientHeight), control.ClientSize);
        Assert.Equal(new Rectangle(0, 0, expectedClientWidth, expectedClientHeight), control.ClientRectangle);
        Assert.Equal(new Rectangle(0, 0, expectedClientWidth, expectedClientHeight), control.DisplayRectangle);
        Assert.Equal(new Size(width, height), control.Size);
        Assert.Equal(x, control.Left);
        Assert.Equal(x + width, control.Right);
        Assert.Equal(y, control.Top);
        Assert.Equal(y + height, control.Bottom);
        Assert.Equal(width, control.Width);
        Assert.Equal(height, control.Height);
        Assert.Equal(new Rectangle(x, y, width, height), control.Bounds);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.Equal(expectedLayoutCallCount, resizeCallCount);
        Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
        Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(SetBounds_Int_Int_Int_Int_WithParent_TestData))]
    public void Control_Bounds_SetWithParent_GetReturnsExpected(int x, int y, int width, int height, int expectedLayoutCallCount, int expectedParentLayoutCallCount)
    {
        using Control parent = new();
        using SubControl control = new()
        {
            Parent = parent
        };
        int layoutCallCount = 0;
        int resizeCallCount = 0;
        int sizeChangedCallCount = 0;
        int clientSizeChangedCallCount = 0;
        int parentLayoutCallCount = 0;
        control.Layout += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("Bounds", e.AffectedProperty);
            Assert.Equal(resizeCallCount, layoutCallCount);
            Assert.Equal(sizeChangedCallCount, layoutCallCount);
            Assert.Equal(clientSizeChangedCallCount, layoutCallCount);
            Assert.Equal(parentLayoutCallCount, layoutCallCount);
            layoutCallCount++;
        };
        control.Resize += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(layoutCallCount - 1, resizeCallCount);
            Assert.Equal(sizeChangedCallCount, resizeCallCount);
            Assert.Equal(clientSizeChangedCallCount, resizeCallCount);
            Assert.Equal(parentLayoutCallCount, resizeCallCount);
            resizeCallCount++;
        };
        control.SizeChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(resizeCallCount - 1, sizeChangedCallCount);
            Assert.Equal(layoutCallCount - 1, sizeChangedCallCount);
            Assert.Equal(clientSizeChangedCallCount, sizeChangedCallCount);
            Assert.Equal(parentLayoutCallCount, sizeChangedCallCount);
            sizeChangedCallCount++;
        };
        control.ClientSizeChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(resizeCallCount - 1, clientSizeChangedCallCount);
            Assert.Equal(layoutCallCount - 1, clientSizeChangedCallCount);
            Assert.Equal(sizeChangedCallCount - 1, clientSizeChangedCallCount);
            Assert.Equal(parentLayoutCallCount, clientSizeChangedCallCount);
            clientSizeChangedCallCount++;
        };
        void parentHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(parent, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("Bounds", e.AffectedProperty);
            parentLayoutCallCount++;
        }

        parent.Layout += parentHandler;

        try
        {
            control.Bounds = new Rectangle(x, y, width, height);
            Assert.Equal(new Size(width, height), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, width, height), control.ClientRectangle);
            Assert.Equal(new Rectangle(0, 0, width, height), control.DisplayRectangle);
            Assert.Equal(new Size(width, height), control.Size);
            Assert.Equal(x, control.Left);
            Assert.Equal(x + width, control.Right);
            Assert.Equal(y, control.Top);
            Assert.Equal(y + height, control.Bottom);
            Assert.Equal(width, control.Width);
            Assert.Equal(height, control.Height);
            Assert.Equal(new Rectangle(x, y, width, height), control.Bounds);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.Equal(expectedLayoutCallCount, resizeCallCount);
            Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
            Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
            Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
            Assert.False(control.IsHandleCreated);

            // Call again.
            control.Bounds = new Rectangle(x, y, width, height);
            Assert.Equal(new Size(width, height), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, width, height), control.ClientRectangle);
            Assert.Equal(new Rectangle(0, 0, width, height), control.DisplayRectangle);
            Assert.Equal(new Size(width, height), control.Size);
            Assert.Equal(x, control.Left);
            Assert.Equal(x + width, control.Right);
            Assert.Equal(y, control.Top);
            Assert.Equal(y + height, control.Bottom);
            Assert.Equal(width, control.Width);
            Assert.Equal(height, control.Height);
            Assert.Equal(new Rectangle(x, y, width, height), control.Bounds);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.Equal(expectedLayoutCallCount, resizeCallCount);
            Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
            Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
            Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
            Assert.False(control.IsHandleCreated);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(SetBounds_Int_Int_Int_Int_WithHandle_TestData))]
    public void Control_Bounds_SetWithHandle_GetReturnsExpected(bool resizeRedraw, int x, int y, int width, int height, int expectedWidth, int expectedHeight, int expectedLayoutCallCount, int expectedInvalidatedCallCount)
    {
        using SubControl control = new();
        control.SetStyle(ControlStyles.ResizeRedraw, resizeRedraw);
        int layoutCallCount = 0;
        int resizeCallCount = 0;
        int sizeChangedCallCount = 0;
        int clientSizeChangedCallCount = 0;
        control.Layout += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("Bounds", e.AffectedProperty);
            Assert.Equal(resizeCallCount, layoutCallCount);
            Assert.Equal(sizeChangedCallCount, layoutCallCount);
            Assert.Equal(clientSizeChangedCallCount, layoutCallCount);
            layoutCallCount++;
        };
        control.Resize += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(layoutCallCount - 1, resizeCallCount);
            Assert.Equal(sizeChangedCallCount, resizeCallCount);
            Assert.Equal(clientSizeChangedCallCount, resizeCallCount);
            resizeCallCount++;
        };
        control.SizeChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(resizeCallCount - 1, sizeChangedCallCount);
            Assert.Equal(layoutCallCount - 1, sizeChangedCallCount);
            Assert.Equal(clientSizeChangedCallCount, sizeChangedCallCount);
            sizeChangedCallCount++;
        };
        control.ClientSizeChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(resizeCallCount - 1, clientSizeChangedCallCount);
            Assert.Equal(layoutCallCount - 1, clientSizeChangedCallCount);
            Assert.Equal(sizeChangedCallCount - 1, clientSizeChangedCallCount);
            clientSizeChangedCallCount++;
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.Bounds = new Rectangle(x, y, width, height);
        Assert.Equal(new Size(expectedWidth, expectedHeight), control.ClientSize);
        Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.ClientRectangle);
        Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.DisplayRectangle);
        Assert.Equal(new Size(expectedWidth, expectedHeight), control.Size);
        Assert.Equal(x, control.Left);
        Assert.Equal(x + expectedWidth, control.Right);
        Assert.Equal(y, control.Top);
        Assert.Equal(y + expectedHeight, control.Bottom);
        Assert.Equal(expectedWidth, control.Width);
        Assert.Equal(expectedHeight, control.Height);
        Assert.Equal(new Rectangle(x, y, expectedWidth, expectedHeight), control.Bounds);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.Equal(expectedLayoutCallCount, resizeCallCount);
        Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
        Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Call again.
        control.Bounds = new Rectangle(x, y, width, height);
        Assert.Equal(new Size(expectedWidth, expectedHeight), control.ClientSize);
        Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.ClientRectangle);
        Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.DisplayRectangle);
        Assert.Equal(new Size(expectedWidth, expectedHeight), control.Size);
        Assert.Equal(x, control.Left);
        Assert.Equal(x + expectedWidth, control.Right);
        Assert.Equal(y, control.Top);
        Assert.Equal(y + expectedHeight, control.Bottom);
        Assert.Equal(expectedWidth, control.Width);
        Assert.Equal(expectedHeight, control.Height);
        Assert.Equal(new Rectangle(x, y, expectedWidth, expectedHeight), control.Bounds);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.Equal(expectedLayoutCallCount, resizeCallCount);
        Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
        Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [MemberData(nameof(SetBounds_Int_Int_Int_Int_WithParentWithHandle_TestData))]
    public void Control_Bounds_SetWithParentWithHandle_GetReturnsExpected(bool resizeRedraw, int x, int y, int width, int height, int expectedWidth, int expectedHeight, int expectedLayoutCallCount, int expectedInvalidatedCallCount, int expectedParentLayoutCallCount1, int expectedParentLayoutCallCount2)
    {
        using Control parent = new();
        using SubControl control = new()
        {
            Parent = parent
        };
        control.SetStyle(ControlStyles.ResizeRedraw, resizeRedraw);
        int layoutCallCount = 0;
        int resizeCallCount = 0;
        int sizeChangedCallCount = 0;
        int clientSizeChangedCallCount = 0;
        int parentLayoutCallCount = 0;
        control.Layout += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("Bounds", e.AffectedProperty);
            Assert.Equal(resizeCallCount, layoutCallCount);
            Assert.Equal(sizeChangedCallCount, layoutCallCount);
            Assert.Equal(clientSizeChangedCallCount, layoutCallCount);
            Assert.Equal(parentLayoutCallCount, layoutCallCount);
            layoutCallCount++;
        };
        control.Resize += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(layoutCallCount - 1, resizeCallCount);
            Assert.Equal(sizeChangedCallCount, resizeCallCount);
            Assert.Equal(clientSizeChangedCallCount, resizeCallCount);
            Assert.Equal(parentLayoutCallCount, resizeCallCount);
            resizeCallCount++;
        };
        control.SizeChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(resizeCallCount - 1, sizeChangedCallCount);
            Assert.Equal(layoutCallCount - 1, sizeChangedCallCount);
            Assert.Equal(clientSizeChangedCallCount, sizeChangedCallCount);
            Assert.Equal(parentLayoutCallCount, sizeChangedCallCount);
            sizeChangedCallCount++;
        };
        control.ClientSizeChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(resizeCallCount - 1, clientSizeChangedCallCount);
            Assert.Equal(layoutCallCount - 1, clientSizeChangedCallCount);
            Assert.Equal(sizeChangedCallCount - 1, clientSizeChangedCallCount);
            Assert.Equal(parentLayoutCallCount, clientSizeChangedCallCount);
            clientSizeChangedCallCount++;
        };
        void parentHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(parent, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("Bounds", e.AffectedProperty);
            parentLayoutCallCount++;
        }

        parent.Layout += parentHandler;
        Assert.NotEqual(IntPtr.Zero, parent.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
        int parentInvalidatedCallCount = 0;
        parent.Invalidated += (sender, e) => parentInvalidatedCallCount++;
        int parentStyleChangedCallCount = 0;
        parent.StyleChanged += (sender, e) => parentStyleChangedCallCount++;
        int parentCreatedCallCount = 0;
        parent.HandleCreated += (sender, e) => parentCreatedCallCount++;

        try
        {
            control.Bounds = new Rectangle(x, y, width, height);
            Assert.Equal(new Size(expectedWidth, expectedHeight), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.ClientRectangle);
            Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.DisplayRectangle);
            Assert.Equal(new Size(expectedWidth, expectedHeight), control.Size);
            Assert.Equal(x, control.Left);
            Assert.Equal(x + expectedWidth, control.Right);
            Assert.Equal(y, control.Top);
            Assert.Equal(y + expectedHeight, control.Bottom);
            Assert.Equal(expectedWidth, control.Width);
            Assert.Equal(expectedHeight, control.Height);
            Assert.Equal(new Rectangle(x, y, expectedWidth, expectedHeight), control.Bounds);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.Equal(expectedParentLayoutCallCount1, parentLayoutCallCount);
            Assert.Equal(expectedLayoutCallCount, resizeCallCount);
            Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
            Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.True(parent.IsHandleCreated);
            Assert.Equal(0, parentInvalidatedCallCount);
            Assert.Equal(0, parentStyleChangedCallCount);
            Assert.Equal(0, parentCreatedCallCount);

            // Call again.
            control.Bounds = new Rectangle(x, y, width, height);
            Assert.Equal(new Size(expectedWidth, expectedHeight), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.ClientRectangle);
            Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.DisplayRectangle);
            Assert.Equal(new Size(expectedWidth, expectedHeight), control.Size);
            Assert.Equal(x, control.Left);
            Assert.Equal(x + expectedWidth, control.Right);
            Assert.Equal(y, control.Top);
            Assert.Equal(y + expectedHeight, control.Bottom);
            Assert.Equal(expectedWidth, control.Width);
            Assert.Equal(expectedHeight, control.Height);
            Assert.Equal(new Rectangle(x, y, expectedWidth, expectedHeight), control.Bounds);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.Equal(expectedParentLayoutCallCount2, parentLayoutCallCount);
            Assert.Equal(expectedLayoutCallCount, resizeCallCount);
            Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
            Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.True(parent.IsHandleCreated);
            Assert.Equal(0, parentInvalidatedCallCount);
            Assert.Equal(0, parentStyleChangedCallCount);
            Assert.Equal(0, parentCreatedCallCount);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    [WinFormsTheory]
    [InlineData(true, true)]
    [InlineData(true, false)]
    [InlineData(false, true)]
    [InlineData(false, false)]
    public void Control_CanFocus_GetWithHandle_ReturnsExpected(bool enabled, bool visible)
    {
        using Control control = new()
        {
            Enabled = enabled,
            Visible = visible
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        Assert.False(control.CanFocus);
    }

    [WinFormsTheory]
    [InlineData(true, true, true)]
    [InlineData(true, false, false)]
    [InlineData(false, true, false)]
    [InlineData(false, false, false)]
    public void Control_CanSelect_Get_ReturnsExpected(bool enabled, bool visible, bool expected)
    {
        using Control control = new()
        {
            Enabled = enabled,
            Visible = visible
        };
        Assert.Equal(expected, control.CanSelect);
    }

    [WinFormsTheory]
    [InlineData(true, true, true)]
    [InlineData(true, false, false)]
    [InlineData(false, true, false)]
    [InlineData(false, false, false)]
    public void Control_CanSelect_GetWithParent_ReturnsExpected(bool enabled, bool visible, bool expected)
    {
        using Control parent = new()
        {
            Enabled = enabled,
            Visible = visible
        };
        using Control control = new()
        {
            Parent = parent
        };
        Assert.Equal(expected, control.CanSelect);
    }

    [WinFormsTheory]
    [InlineData(true, true, true)]
    [InlineData(true, false, false)]
    [InlineData(false, true, false)]
    [InlineData(false, false, false)]
    public void Control_CanSelect_GetWithHandle_ReturnsExpected(bool enabled, bool visible, bool expected)
    {
        using Control control = new()
        {
            Enabled = enabled,
            Visible = visible
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        Assert.Equal(expected, control.CanSelect);
    }

    [WinFormsTheory]
    [InlineData(true, true, true)]
    [InlineData(true, false, false)]
    [InlineData(false, true, false)]
    [InlineData(false, false, false)]
    public void Control_CanSelect_GetWithParentWithHandle_ReturnsExpected(bool enabled, bool visible, bool expected)
    {
        using Control parent = new()
        {
            Enabled = enabled,
            Visible = visible
        };
        using Control control = new()
        {
            Parent = parent
        };
        Assert.NotEqual(IntPtr.Zero, parent.Handle);
        Assert.Equal(expected, control.CanSelect);
    }

    [WinFormsTheory]
    [BoolData]
    public void Control_Capture_Set_GetReturnsExpected(bool value)
    {
        using Control control = new()
        {
            Capture = value
        };
        Assert.Equal(value, control.Capture);
        Assert.Equal(value, control.IsHandleCreated);

        // Set same.
        control.Capture = value;
        Assert.Equal(value, control.Capture);
        Assert.Equal(value, control.IsHandleCreated);

        // Set different.
        control.Capture = !value;
        Assert.Equal(!value, control.Capture);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [BoolData]
    public void Control_Capture_SetWithHandle_GetReturnsExpected(bool value)
    {
        using Control control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.Capture = value;
        Assert.Equal(value, control.Capture);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.Capture = value;
        Assert.Equal(value, control.Capture);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set different.
        control.Capture = !value;
        Assert.Equal(!value, control.Capture);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [BoolData]
    public void Control_CausesValidation_Set_GetReturnsExpected(bool value)
    {
        using Control control = new()
        {
            CausesValidation = value
        };
        Assert.Equal(value, control.CausesValidation);
        Assert.False(control.IsHandleCreated);

        // Set same
        control.CausesValidation = value;
        Assert.Equal(value, control.CausesValidation);
        Assert.False(control.IsHandleCreated);

        // Set different
        control.CausesValidation = !value;
        Assert.Equal(!value, control.CausesValidation);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [BoolData]
    public void Control_CausesValidation_SetWithHandle_GetReturnsExpected(bool value)
    {
        using Control control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.CausesValidation = value;
        Assert.Equal(value, control.CausesValidation);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same
        control.CausesValidation = value;
        Assert.Equal(value, control.CausesValidation);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set different
        control.CausesValidation = !value;
        Assert.Equal(!value, control.CausesValidation);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void Control_CausesValidation_SetWithHandler_CallsCausesValidationChanged()
    {
        using Control control = new()
        {
            CausesValidation = true
        };
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        control.CausesValidationChanged += handler;

        // Set different.
        control.CausesValidation = false;
        Assert.False(control.CausesValidation);
        Assert.Equal(1, callCount);

        // Set same.
        control.CausesValidation = false;
        Assert.False(control.CausesValidation);
        Assert.Equal(1, callCount);

        // Set different.
        control.CausesValidation = true;
        Assert.True(control.CausesValidation);
        Assert.Equal(2, callCount);

        // Remove handler.
        control.CausesValidationChanged -= handler;
        control.CausesValidation = false;
        Assert.False(control.CausesValidation);
        Assert.Equal(2, callCount);
    }

    public static IEnumerable<object[]> ClientSize_Get_TestData()
    {
        yield return new object[] { new Control(), Size.Empty };
        yield return new object[] { new NonZeroWidthBorderedControl(), Size.Empty };
        yield return new object[] { new NonZeroHeightBorderedControl(), Size.Empty };
        yield return new object[] { new NonZeroWidthNonZeroHeightBorderedControl(), new Size(6, 16) };
    }

    [WinFormsTheory]
    [MemberData(nameof(ClientSize_Get_TestData))]
    public void Control_ClientSize_Get_ReturnsExpected(Control control, Size expected)
    {
        Assert.Equal(expected, control.ClientSize);
    }

    private class NonZeroWidthBorderedControl : BorderedControl
    {
        protected override Size DefaultSize => new(10, 0);
    }

    private class NonZeroHeightBorderedControl : BorderedControl
    {
        protected override Size DefaultSize => new(0, 10);
    }

    private class NonZeroWidthNonZeroHeightBorderedControl : BorderedControl
    {
        protected override Size DefaultSize => new(10, 20);
    }

    public static IEnumerable<object[]> ClientSize_Set_TestData()
    {
        yield return new object[] { new Size(10, 20), 1 };
        yield return new object[] { new Size(1, 2), 1 };
        yield return new object[] { new Size(0, 0), 0 };
        yield return new object[] { new Size(-1, -2), 1 };
        yield return new object[] { new Size(-1, 2), 1 };
        yield return new object[] { new Size(1, -2), 1 };
    }

    [WinFormsTheory]
    [MemberData(nameof(ClientSize_Set_TestData))]
    public void Control_ClientSize_Set_GetReturnsExpected(Size value, int expectedLayoutCallCount)
    {
        using Control control = new();
        int layoutCallCount = 0;
        control.Layout += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("Bounds", e.AffectedProperty);
            layoutCallCount++;
        };

        control.ClientSize = value;
        Assert.Equal(value, control.ClientSize);
        Assert.Equal(new Rectangle(Point.Empty, value), control.ClientRectangle);
        Assert.Equal(new Rectangle(Point.Empty, value), control.DisplayRectangle);
        Assert.Equal(value, control.Size);
        Assert.Equal(new Rectangle(Point.Empty, value), control.Bounds);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.ClientSize = value;
        Assert.Equal(value, control.ClientSize);
        Assert.Equal(new Rectangle(Point.Empty, value), control.ClientRectangle);
        Assert.Equal(new Rectangle(Point.Empty, value), control.DisplayRectangle);
        Assert.Equal(value, control.Size);
        Assert.Equal(new Rectangle(Point.Empty, value), control.Bounds);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> ClientSize_SetWithCustomStyle_TestData()
    {
        yield return new object[] { new Size(10, 20), new Size(14, 24) };
        yield return new object[] { new Size(1, 2), new Size(5, 6) };
        yield return new object[] { new Size(0, 0), new Size(4, 4) };
        yield return new object[] { new Size(-1, -2), new Size(3, 2) };
        yield return new object[] { new Size(-1, 2), new Size(3, 6) };
        yield return new object[] { new Size(1, -2), new Size(5, 2) };
    }

    [WinFormsTheory]
    [MemberData(nameof(ClientSize_SetWithCustomStyle_TestData))]
    public void Control_ClientSize_SetWithCustomStyle_GetReturnsExpected(Size value, Size expectedSize)
    {
        using BorderedControl control = new();
        int layoutCallCount = 0;
        control.Layout += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("Bounds", e.AffectedProperty);
            layoutCallCount++;
        };

        control.ClientSize = value;
        Assert.Equal(value, control.ClientSize);
        Assert.Equal(new Rectangle(Point.Empty, value), control.ClientRectangle);
        Assert.Equal(new Rectangle(Point.Empty, value), control.DisplayRectangle);
        Assert.Equal(expectedSize, control.Size);
        Assert.Equal(0, control.Left);
        Assert.Equal(expectedSize.Width, control.Right);
        Assert.Equal(0, control.Top);
        Assert.Equal(expectedSize.Height, control.Bottom);
        Assert.Equal(expectedSize.Width, control.Width);
        Assert.Equal(expectedSize.Height, control.Height);
        Assert.Equal(new Rectangle(Point.Empty, expectedSize), control.Bounds);
        Assert.Equal(1, layoutCallCount);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.ClientSize = value;
        Assert.Equal(value, control.ClientSize);
        Assert.Equal(new Rectangle(Point.Empty, value), control.ClientRectangle);
        Assert.Equal(new Rectangle(Point.Empty, value), control.DisplayRectangle);
        Assert.Equal(expectedSize, control.Size);
        Assert.Equal(0, control.Left);
        Assert.Equal(expectedSize.Width, control.Right);
        Assert.Equal(0, control.Top);
        Assert.Equal(expectedSize.Height, control.Bottom);
        Assert.Equal(expectedSize.Width, control.Width);
        Assert.Equal(expectedSize.Height, control.Height);
        Assert.Equal(new Rectangle(Point.Empty, expectedSize), control.Bounds);
        Assert.Equal(1, layoutCallCount);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> ClientSize_SetWithHandle_TestData()
    {
        yield return new object[] { true, new Size(1, 2), new Size(1, 2), 1, 1 };
        yield return new object[] { true, new Size(0, 0), new Size(0, 0), 0, 0 };
        yield return new object[] { true, new Size(-1, -2), new Size(0, 0), 0, 0 };
        yield return new object[] { true, new Size(-1, 2), new Size(0, 2), 1, 1 };
        yield return new object[] { true, new Size(1, -2), new Size(1, 0), 1, 1 };

        yield return new object[] { false, new Size(1, 2), new Size(1, 2), 1, 0 };
        yield return new object[] { false, new Size(0, 0), new Size(0, 0), 0, 0 };
        yield return new object[] { false, new Size(-1, -2), new Size(0, 0), 0, 0 };
        yield return new object[] { false, new Size(-1, 2), new Size(0, 2), 1, 0 };
        yield return new object[] { false, new Size(1, -2), new Size(1, 0), 1, 0 };
    }

    [WinFormsTheory]
    [MemberData(nameof(ClientSize_SetWithHandle_TestData))]
    public void Control_ClientSize_SetWithHandle_GetReturnsExpected(bool resizeRedraw, Size value, Size expectedSize, int expectedLayoutCallCount, int expectedInvalidatedCallCount)
    {
        using SubControl control = new();
        control.SetStyle(ControlStyles.ResizeRedraw, resizeRedraw);
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.ClientSize = value;
        Assert.Equal(value, control.ClientSize);
        Assert.Equal(new Rectangle(Point.Empty, value), control.ClientRectangle);
        Assert.Equal(new Rectangle(Point.Empty, value), control.DisplayRectangle);
        Assert.Equal(expectedSize, control.Size);
        Assert.Equal(0, control.Left);
        Assert.Equal(expectedSize.Width, control.Right);
        Assert.Equal(0, control.Top);
        Assert.Equal(expectedSize.Height, control.Bottom);
        Assert.Equal(expectedSize.Width, control.Width);
        Assert.Equal(expectedSize.Height, control.Height);
        Assert.Equal(new Rectangle(Point.Empty, expectedSize), control.Bounds);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.ClientSize = value;
        Assert.Equal(value, control.ClientSize);
        Assert.Equal(new Rectangle(Point.Empty, value), control.ClientRectangle);
        Assert.Equal(new Rectangle(Point.Empty, value), control.DisplayRectangle);
        Assert.Equal(expectedSize, control.Size);
        Assert.Equal(0, control.Left);
        Assert.Equal(expectedSize.Width, control.Right);
        Assert.Equal(0, control.Top);
        Assert.Equal(expectedSize.Height, control.Bottom);
        Assert.Equal(expectedSize.Width, control.Width);
        Assert.Equal(expectedSize.Height, control.Height);
        Assert.Equal(new Rectangle(Point.Empty, expectedSize), control.Bounds);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void Control_ClientSize_SetWithHandler_CallsClientSizeChanged()
    {
        using Control control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        int sizeChangedCallCount = 0;
        EventHandler sizeChangedHandler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            sizeChangedCallCount++;
        };
        control.ClientSizeChanged += handler;
        control.SizeChanged += sizeChangedHandler;

        control.ClientSize = new Size(10, 10);
        Assert.Equal(new Size(10, 10), control.ClientSize);
        Assert.Equal(2, callCount);
        Assert.Equal(1, sizeChangedCallCount);

        // Set same.
        control.ClientSize = new Size(10, 10);
        Assert.Equal(new Size(10, 10), control.ClientSize);
        Assert.Equal(3, callCount);
        Assert.Equal(1, sizeChangedCallCount);

        // Set different.
        control.ClientSize = new Size(11, 11);
        Assert.Equal(new Size(11, 11), control.ClientSize);
        Assert.Equal(5, callCount);
        Assert.Equal(2, sizeChangedCallCount);

        // Remove handler.
        control.ClientSizeChanged -= handler;
        control.SizeChanged -= sizeChangedHandler;
        control.ClientSize = new Size(10, 10);
        Assert.Equal(new Size(10, 10), control.ClientSize);
        Assert.Equal(5, callCount);
        Assert.Equal(2, sizeChangedCallCount);
    }

    public static IEnumerable<object[]> ContextMenuStrip_Set_TestData()
    {
        yield return new object[] { null };
        yield return new object[] { new ContextMenuStrip() };
    }

    [WinFormsTheory]
    [MemberData(nameof(ContextMenuStrip_Set_TestData))]
    public void Control_ContextMenuStrip_Set_GetReturnsExpected(ContextMenuStrip value)
    {
        using Control control = new()
        {
            ContextMenuStrip = value
        };
        Assert.Same(value, control.ContextMenuStrip);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.ContextMenuStrip = value;
        Assert.Same(value, control.ContextMenuStrip);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(ContextMenuStrip_Set_TestData))]
    public void Control_ContextMenuStrip_SetWithCustomOldValue_GetReturnsExpected(ContextMenuStrip value)
    {
        ContextMenuStrip oldValue = new();
        using Control control = new()
        {
            ContextMenuStrip = oldValue
        };
        control.ContextMenuStrip = value;
        Assert.Same(value, control.ContextMenuStrip);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.ContextMenuStrip = value;
        Assert.Same(value, control.ContextMenuStrip);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void Control_ContextMenuStrip_SetDisposeNew_RemovesContextMenuStrip()
    {
        using ContextMenuStrip menu = new();
        using Control control = new()
        {
            ContextMenuStrip = menu
        };
        Assert.Same(menu, control.ContextMenuStrip);

        menu.Dispose();
        Assert.Null(control.ContextMenuStrip);
    }

    [WinFormsFact]
    public void Control_ContextMenuStrip_SetDisposeOld_RemovesContextMenuStrip()
    {
        using ContextMenuStrip menu1 = new();
        using ContextMenuStrip menu2 = new();
        using Control control = new()
        {
            ContextMenuStrip = menu1
        };
        Assert.Same(menu1, control.ContextMenuStrip);

        control.ContextMenuStrip = menu2;
        Assert.Same(menu2, control.ContextMenuStrip);

        menu1.Dispose();
        Assert.Same(menu2, control.ContextMenuStrip);
    }

    [WinFormsFact]
    public void Control_ContextMenuStrip_SetWithHandler_CallsContextMenuStripChanged()
    {
        using Control control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        control.ContextMenuStripChanged += handler;

        // Set different.
        using ContextMenuStrip menu1 = new();
        control.ContextMenuStrip = menu1;
        Assert.Same(menu1, control.ContextMenuStrip);
        Assert.Equal(1, callCount);

        // Set same.
        control.ContextMenuStrip = menu1;
        Assert.Same(menu1, control.ContextMenuStrip);
        Assert.Equal(1, callCount);

        // Set different.
        using ContextMenuStrip menu2 = new();
        control.ContextMenuStrip = menu2;
        Assert.Same(menu2, control.ContextMenuStrip);
        Assert.Equal(2, callCount);

        // Set null.
        control.ContextMenuStrip = null;
        Assert.Null(control.ContextMenuStrip);
        Assert.Equal(3, callCount);

        // Remove handler.
        control.ContextMenuStripChanged -= handler;
        control.ContextMenuStrip = menu1;
        Assert.Same(menu1, control.ContextMenuStrip);
        Assert.Equal(3, callCount);
    }

    public static IEnumerable<object[]> Controls_CustomCreateControlsInstance_TestData()
    {
        yield return new object[] { null };
        yield return new object[] { new Control.ControlCollection(new Control()) };
    }

    [WinFormsTheory]
    [MemberData(nameof(Controls_CustomCreateControlsInstance_TestData))]
    public void Control_Controls_GetCustomCreateControlsInstance_ReturnsExpected(Control.ControlCollection result)
    {
        using CustomCreateControlsInstanceControl control = new()
        {
            CreateControlsResult = result
        };
        Assert.Same(result, control.Controls);
        Assert.Same(control.Controls, control.Controls);
        Assert.False(control.IsHandleCreated);
    }

    private class CustomCreateControlsInstanceControl : Control
    {
        public ControlCollection CreateControlsResult { get; set; }

        protected override ControlCollection CreateControlsInstance() => CreateControlsResult;
    }

    [WinFormsFact]
    public void Control_Controls_ResetValue_Success()
    {
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(Control))[nameof(Control.Controls)];
        using Control control = new();
        Assert.False(property.CanResetValue(control));

        Assert.Empty(control.Controls);
        Assert.False(property.CanResetValue(control));

        using Control child = new();
        control.Controls.Add(child);
        Assert.Same(child, Assert.Single(control.Controls));
        Assert.False(property.CanResetValue(control));

        property.ResetValue(control);
        Assert.Same(child, Assert.Single(control.Controls));
        Assert.False(property.CanResetValue(control));
    }

    [WinFormsFact]
    public void Control_Controls_ShouldSerializeValue_Success()
    {
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(Control))[nameof(Control.Controls)];
        using Control control = new();
        Assert.True(property.ShouldSerializeValue(control));

        Assert.Empty(control.Controls);
        Assert.True(property.ShouldSerializeValue(control));

        using Control child = new();
        control.Controls.Add(child);
        Assert.Same(child, Assert.Single(control.Controls));
        Assert.True(property.ShouldSerializeValue(control));

        property.ResetValue(control);
        Assert.Same(child, Assert.Single(control.Controls));
        Assert.True(property.ShouldSerializeValue(control));
    }

    [WinFormsFact]
    public void Control_Cursor_GetUseWaitCursor_ReturnsExpected()
    {
        using Cursor cursor = new(1);
        using Control control = new()
        {
            UseWaitCursor = true
        };
        Assert.Same(Cursors.WaitCursor, control.Cursor);

        // Set custom.
        control.Cursor = cursor;
        Assert.Same(Cursors.WaitCursor, control.Cursor);
    }

    [WinFormsFact]
    public void Control_Cursor_GetWithParent_ReturnsExpected()
    {
        using Cursor cursor1 = new(1);
        using Cursor cursor2 = new(2);
        using Control parent = new()
        {
            Cursor = cursor1
        };
        using Control control = new()
        {
            Parent = parent
        };
        Assert.Same(cursor1, control.Cursor);

        // Set custom.
        control.Cursor = cursor2;
        Assert.Same(cursor2, control.Cursor);
    }

    [WinFormsFact]
    public void Control_Cursor_GetWithParentCantAccessProperties_ReturnsExpected()
    {
        using Cursor cursor1 = new(1);
        using Cursor cursor2 = new(2);
        using SubAxHost parent = new("00000000-0000-0000-0000-000000000000")
        {
            Cursor = cursor1
        };
        using Control control = new()
        {
            Parent = parent
        };
        Assert.Same(cursor1, control.Cursor);

        // Set custom.
        control.Cursor = cursor2;
        Assert.Same(cursor2, control.Cursor);
    }

    [WinFormsFact]
    public void Control_Cursor_GetWithDefaultCursor_ReturnsExpected()
    {
        using Cursor cursor = new(1);
        using CustomDefaultCursorControl control = new();
        Assert.Same(control.DefaultCursorResult, control.Cursor);

        // Set custom.
        control.Cursor = cursor;
        Assert.Same(cursor, control.Cursor);
    }

    [WinFormsFact]
    public void Control_Cursor_GetWithDefaultCursorWithParent_ReturnsExpected()
    {
        using Cursor cursor1 = new(1);
        using Cursor cursor2 = new(2);
        using Control parent = new()
        {
            Cursor = cursor1
        };
        using CustomDefaultCursorControl control = new()
        {
            Parent = parent
        };
        Assert.Same(control.DefaultCursorResult, control.Cursor);

        // Set custom.
        control.Cursor = cursor2;
        Assert.Same(cursor2, control.Cursor);
    }

    private class CustomDefaultCursorControl : Control
    {
        public Cursor DefaultCursorResult { get; } = new(1);

        protected override Cursor DefaultCursor => DefaultCursorResult;
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetCursorTheoryData))]
    public void Control_Cursor_Set_GetReturnsExpected(Cursor value)
    {
        using Control control = new()
        {
            Cursor = value
        };
        Assert.Same(value ?? Cursors.Default, control.Cursor);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.Cursor = value;
        Assert.Same(value ?? Cursors.Default, control.Cursor);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetCursorTheoryData))]
    public void Control_Cursor_SetWithHandle_GetReturnsExpected(Cursor value)
    {
        using Control control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.Cursor = value;
        Assert.Same(value ?? Cursors.Default, control.Cursor);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.Cursor = value;
        Assert.Same(value ?? Cursors.Default, control.Cursor);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetCursorTheoryData))]
    public void Control_Cursor_SetWithChildren_GetReturnsExpected(Cursor value)
    {
        using Control child1 = new();
        using Control child2 = new();
        using Control control = new();
        control.Controls.Add(child1);
        control.Controls.Add(child2);

        control.Cursor = value;
        Assert.Same(value ?? Cursors.Default, control.Cursor);
        Assert.Same(value ?? Cursors.Default, child1.Cursor);
        Assert.Same(value ?? Cursors.Default, child2.Cursor);

        // Set same.
        control.Cursor = value;
        Assert.Same(value ?? Cursors.Default, control.Cursor);
        Assert.Same(value ?? Cursors.Default, child1.Cursor);
        Assert.Same(value ?? Cursors.Default, child2.Cursor);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetCursorTheoryData))]
    public void Control_Cursor_SetWithChildrenWithCursor_GetReturnsExpected(Cursor value)
    {
        Cursor cursor1 = new(1);
        Cursor cursor2 = new(1);
        using Control child1 = new()
        {
            Cursor = cursor1
        };
        using Control child2 = new()
        {
            Cursor = cursor2
        };
        using Control control = new();
        control.Controls.Add(child1);
        control.Controls.Add(child2);

        control.Cursor = value;
        Assert.Same(value ?? Cursors.Default, control.Cursor);
        Assert.Same(cursor1, child1.Cursor);
        Assert.Same(cursor2, child2.Cursor);

        // Set same.
        control.Cursor = value;
        Assert.Same(value ?? Cursors.Default, control.Cursor);
        Assert.Same(cursor1, child1.Cursor);
        Assert.Same(cursor2, child2.Cursor);
    }

    [WinFormsFact]
    public void Control_Cursor_SetWithHandler_CallsCursorChanged()
    {
        using Control control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        control.CursorChanged += handler;

        // Set different.
        using Cursor cursor1 = new(1);
        control.Cursor = cursor1;
        Assert.Same(cursor1, control.Cursor);
        Assert.Equal(1, callCount);

        // Set same.
        control.Cursor = cursor1;
        Assert.Same(cursor1, control.Cursor);
        Assert.Equal(1, callCount);

        // Set different.
        using Cursor cursor2 = new(2);
        control.Cursor = cursor2;
        Assert.Same(cursor2, control.Cursor);
        Assert.Equal(2, callCount);

        // Set null.
        control.Cursor = null;
        Assert.Same(Cursors.Default, control.Cursor);
        Assert.Equal(3, callCount);

        // Remove handler.
        control.CursorChanged -= handler;
        control.Cursor = cursor1;
        Assert.Same(cursor1, control.Cursor);
        Assert.Equal(3, callCount);
    }

    [WinFormsFact]
    public void Control_Cursor_SetWithChildrenWithHandler_CallsCursorChanged()
    {
        using Control child1 = new();
        using Control child2 = new();
        using Control control = new();
        control.Controls.Add(child1);
        control.Controls.Add(child2);

        int callCount = 0;
        int child1CallCount = 0;
        int child2CallCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        EventHandler childHandler1 = (sender, e) =>
        {
            Assert.Same(child1, sender);
            Assert.Same(EventArgs.Empty, e);
            child1CallCount++;
        };
        EventHandler childHandler2 = (sender, e) =>
        {
            Assert.Same(child2, sender);
            Assert.Same(EventArgs.Empty, e);
            child2CallCount++;
        };
        control.CursorChanged += handler;
        child1.CursorChanged += childHandler1;
        child2.CursorChanged += childHandler2;

        // Set different.
        using Cursor cursor1 = new(1);
        control.Cursor = cursor1;
        Assert.Same(cursor1, control.Cursor);
        Assert.Same(cursor1, child1.Cursor);
        Assert.Same(cursor1, child2.Cursor);
        Assert.Equal(1, callCount);
        Assert.Equal(1, child1CallCount);
        Assert.Equal(1, child2CallCount);

        // Set same.
        control.Cursor = cursor1;
        Assert.Same(cursor1, control.Cursor);
        Assert.Same(cursor1, child1.Cursor);
        Assert.Same(cursor1, child2.Cursor);
        Assert.Equal(1, callCount);
        Assert.Equal(1, child1CallCount);
        Assert.Equal(1, child2CallCount);

        // Set different.
        using Cursor cursor2 = new(2);
        control.Cursor = cursor2;
        Assert.Same(cursor2, control.Cursor);
        Assert.Same(cursor2, child1.Cursor);
        Assert.Same(cursor2, child2.Cursor);
        Assert.Equal(2, callCount);
        Assert.Equal(2, child1CallCount);
        Assert.Equal(2, child2CallCount);

        // Set null.
        control.Cursor = null;
        Assert.Same(Cursors.Default, control.Cursor);
        Assert.Same(Cursors.Default, child1.Cursor);
        Assert.Same(Cursors.Default, child2.Cursor);
        Assert.Equal(3, callCount);
        Assert.Equal(3, child1CallCount);
        Assert.Equal(3, child2CallCount);

        // Remove handler.
        control.CursorChanged -= handler;
        child1.CursorChanged -= childHandler1;
        child2.CursorChanged -= childHandler2;
        control.Cursor = cursor1;
        Assert.Same(cursor1, control.Cursor);
        Assert.Same(cursor1, child1.Cursor);
        Assert.Same(cursor1, child2.Cursor);
        Assert.Equal(3, callCount);
        Assert.Equal(3, child1CallCount);
        Assert.Equal(3, child2CallCount);
    }

    [WinFormsFact]
    public void Control_Cursor_SetWithChildrenWithCursorWithHandler_CallsCursorChanged()
    {
        using Cursor childCursor1 = new(1);
        using Cursor childCursor2 = new(2);
        using Control child1 = new()
        {
            Cursor = childCursor1
        };
        using Control child2 = new()
        {
            Cursor = childCursor2
        };
        using Control control = new();
        control.Controls.Add(child1);
        control.Controls.Add(child2);

        int callCount = 0;
        int child1CallCount = 0;
        int child2CallCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        EventHandler childHandler1 = (sender, e) =>
        {
            Assert.Same(child1, sender);
            Assert.Same(EventArgs.Empty, e);
            child1CallCount++;
        };
        EventHandler childHandler2 = (sender, e) =>
        {
            Assert.Same(child2, sender);
            Assert.Same(EventArgs.Empty, e);
            child2CallCount++;
        };
        control.CursorChanged += handler;
        child1.CursorChanged += childHandler1;
        child2.CursorChanged += childHandler2;

        // Set different.
        using Cursor cursor1 = new(3);
        control.Cursor = cursor1;
        Assert.Same(cursor1, control.Cursor);
        Assert.Same(childCursor1, child1.Cursor);
        Assert.Same(childCursor2, child2.Cursor);
        Assert.Equal(1, callCount);
        Assert.Equal(0, child1CallCount);
        Assert.Equal(0, child2CallCount);

        // Set same.
        control.Cursor = cursor1;
        Assert.Same(cursor1, control.Cursor);
        Assert.Same(childCursor1, child1.Cursor);
        Assert.Same(childCursor2, child2.Cursor);
        Assert.Equal(1, callCount);
        Assert.Equal(0, child1CallCount);
        Assert.Equal(0, child2CallCount);

        // Set different.
        using Cursor cursor2 = new(4);
        control.Cursor = cursor2;
        Assert.Same(cursor2, control.Cursor);
        Assert.Same(childCursor1, child1.Cursor);
        Assert.Same(childCursor2, child2.Cursor);
        Assert.Equal(2, callCount);
        Assert.Equal(0, child1CallCount);
        Assert.Equal(0, child2CallCount);

        // Set null.
        control.Cursor = null;
        Assert.Same(Cursors.Default, control.Cursor);
        Assert.Same(childCursor1, child1.Cursor);
        Assert.Same(childCursor2, child2.Cursor);
        Assert.Equal(3, callCount);
        Assert.Equal(0, child1CallCount);
        Assert.Equal(0, child2CallCount);

        // Remove handler.
        control.CursorChanged -= handler;
        child1.CursorChanged -= childHandler1;
        child2.CursorChanged -= childHandler2;
        control.Cursor = cursor1;
        Assert.Same(cursor1, control.Cursor);
        Assert.Same(childCursor1, child1.Cursor);
        Assert.Same(childCursor2, child2.Cursor);
        Assert.Equal(3, callCount);
        Assert.Equal(0, child1CallCount);
        Assert.Equal(0, child2CallCount);
    }

    [WinFormsTheory]
    [EnumData<DockStyle>]
    public void Control_Dock_Set_GetReturnsExpected(DockStyle value)
    {
        using Control control = new();
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;

        control.Dock = value;
        Assert.Equal(value, control.Dock);
        Assert.Equal(AnchorStyles.Top | AnchorStyles.Left, control.Anchor);
        Assert.Equal(0, layoutCallCount);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.Dock = value;
        Assert.Equal(value, control.Dock);
        Assert.Equal(AnchorStyles.Top | AnchorStyles.Left, control.Anchor);
        Assert.Equal(0, layoutCallCount);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [EnumData<DockStyle>]
    public void Control_Dock_SetWithOldValue_GetReturnsExpected(DockStyle value)
    {
        using Control control = new()
        {
            Dock = DockStyle.Top
        };
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;

        control.Dock = value;
        Assert.Equal(value, control.Dock);
        Assert.Equal(AnchorStyles.Top | AnchorStyles.Left, control.Anchor);
        Assert.Equal(0, layoutCallCount);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.Dock = value;
        Assert.Equal(value, control.Dock);
        Assert.Equal(AnchorStyles.Top | AnchorStyles.Left, control.Anchor);
        Assert.Equal(0, layoutCallCount);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> Dock_SetWithParent_TestData()
    {
        yield return new object[] { DockStyle.Bottom, 1 };
        yield return new object[] { DockStyle.Fill, 1 };
        yield return new object[] { DockStyle.Left, 1 };
        yield return new object[] { DockStyle.None, 0 };
        yield return new object[] { DockStyle.Right, 1 };
        yield return new object[] { DockStyle.Top, 1 };
    }

    [WinFormsTheory]
    [MemberData(nameof(Dock_SetWithParent_TestData))]
    public void Control_Dock_SetWithParent_GetReturnsExpected(DockStyle value, int expectedParentLayoutCallCount)
    {
        using Control parent = new();
        using Control control = new()
        {
            Parent = parent
        };
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(parent, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("Dock", e.AffectedProperty);
            parentLayoutCallCount++;
        }

        parent.Layout += parentHandler;

        try
        {
            control.Dock = value;
            Assert.Equal(value, control.Dock);
            Assert.Equal(AnchorStyles.Top | AnchorStyles.Left, control.Anchor);
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.Dock = value;
            Assert.Equal(value, control.Dock);
            Assert.Equal(AnchorStyles.Top | AnchorStyles.Left, control.Anchor);
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
            Assert.False(control.IsHandleCreated);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    public static IEnumerable<object[]> Control_Dock_SetWithOldValueWithParent_TestData()
    {
        yield return new object[] { DockStyle.Bottom, DockStyle.Bottom, 0 };
        yield return new object[] { DockStyle.Fill, DockStyle.Bottom, 1 };
        yield return new object[] { DockStyle.Left, DockStyle.Bottom, 1 };
        yield return new object[] { DockStyle.None, DockStyle.Bottom, 1 };
        yield return new object[] { DockStyle.Right, DockStyle.Bottom, 1 };
        yield return new object[] { DockStyle.Top, DockStyle.Bottom, 1 };

        yield return new object[] { DockStyle.Bottom, DockStyle.Fill, 1 };
        yield return new object[] { DockStyle.Fill, DockStyle.Fill, 0 };
        yield return new object[] { DockStyle.Left, DockStyle.Fill, 1 };
        yield return new object[] { DockStyle.None, DockStyle.Fill, 1 };
        yield return new object[] { DockStyle.Right, DockStyle.Fill, 1 };
        yield return new object[] { DockStyle.Top, DockStyle.Fill, 1 };

        yield return new object[] { DockStyle.Bottom, DockStyle.Left, 1 };
        yield return new object[] { DockStyle.Fill, DockStyle.Left, 1 };
        yield return new object[] { DockStyle.Left, DockStyle.Left, 0 };
        yield return new object[] { DockStyle.None, DockStyle.Left, 1 };
        yield return new object[] { DockStyle.Right, DockStyle.Left, 1 };
        yield return new object[] { DockStyle.Top, DockStyle.Left, 1 };

        yield return new object[] { DockStyle.Bottom, DockStyle.None, 1 };
        yield return new object[] { DockStyle.Fill, DockStyle.None, 1 };
        yield return new object[] { DockStyle.Left, DockStyle.None, 1 };
        yield return new object[] { DockStyle.None, DockStyle.None, 0 };
        yield return new object[] { DockStyle.Right, DockStyle.None, 1 };
        yield return new object[] { DockStyle.Top, DockStyle.None, 1 };

        yield return new object[] { DockStyle.Bottom, DockStyle.Right, 1 };
        yield return new object[] { DockStyle.Fill, DockStyle.Right, 1 };
        yield return new object[] { DockStyle.Left, DockStyle.Right, 1 };
        yield return new object[] { DockStyle.None, DockStyle.Right, 1 };
        yield return new object[] { DockStyle.Right, DockStyle.Right, 0 };
        yield return new object[] { DockStyle.Top, DockStyle.Right, 1 };

        yield return new object[] { DockStyle.Bottom, DockStyle.Top, 1 };
        yield return new object[] { DockStyle.Fill, DockStyle.Top, 1 };
        yield return new object[] { DockStyle.Left, DockStyle.Top, 1 };
        yield return new object[] { DockStyle.None, DockStyle.Top, 1 };
        yield return new object[] { DockStyle.Right, DockStyle.Top, 1 };
        yield return new object[] { DockStyle.Top, DockStyle.Top, 0 };
    }

    [WinFormsTheory]
    [MemberData(nameof(Control_Dock_SetWithOldValueWithParent_TestData))]
    public void Control_Dock_SetWithOldValueWithParent_GetReturnsExpected(DockStyle oldValue, DockStyle value, int expectedParentLayoutCallCount)
    {
        using Control parent = new();
        using Control control = new()
        {
            Dock = oldValue,
            Parent = parent
        };
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(parent, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("Dock", e.AffectedProperty);
            parentLayoutCallCount++;
        }

        parent.Layout += parentHandler;

        try
        {
            control.Dock = value;
            Assert.Equal(value, control.Dock);
            Assert.Equal(AnchorStyles.Top | AnchorStyles.Left, control.Anchor);
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.Dock = value;
            Assert.Equal(value, control.Dock);
            Assert.Equal(AnchorStyles.Top | AnchorStyles.Left, control.Anchor);
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
            Assert.False(control.IsHandleCreated);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    public static IEnumerable<object[]> Dock_SetWithAnchor_TestData()
    {
        foreach (AnchorStyles anchor in Enum.GetValues(typeof(AnchorStyles)))
        {
            foreach (DockStyle value in Enum.GetValues(typeof(DockStyle)))
            {
                yield return new object[] { anchor, value, value == DockStyle.None ? anchor : AnchorStyles.Top | AnchorStyles.Left };
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(Dock_SetWithAnchor_TestData))]
    public void Control_Dock_SetWithAnchor_GetReturnsExpected(AnchorStyles anchor, DockStyle value, AnchorStyles expectedAnchor)
    {
        using Control control = new()
        {
            Anchor = anchor
        };

        control.Dock = value;
        Assert.Equal(value, control.Dock);
        Assert.Equal(expectedAnchor, control.Anchor);

        // Set same.
        control.Dock = value;
        Assert.Equal(value, control.Dock);
        Assert.Equal(expectedAnchor, control.Anchor);
    }

    [WinFormsTheory]
    [EnumData<DockStyle>]
    public void Control_Dock_SetWithHandle_GetReturnsExpected(DockStyle value)
    {
        using Control control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;

        control.Dock = value;
        Assert.Equal(value, control.Dock);
        Assert.Equal(AnchorStyles.Top | AnchorStyles.Left, control.Anchor);
        Assert.Equal(0, layoutCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.Dock = value;
        Assert.Equal(value, control.Dock);
        Assert.Equal(AnchorStyles.Top | AnchorStyles.Left, control.Anchor);
        Assert.Equal(0, layoutCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void Control_Dock_SetWithHandler_CallsDockChanged()
    {
        using Control control = new()
        {
            Dock = DockStyle.None
        };
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        control.DockChanged += handler;

        // Set different.
        control.Dock = DockStyle.Top;
        Assert.Equal(DockStyle.Top, control.Dock);
        Assert.Equal(1, callCount);

        // Set same.
        control.Dock = DockStyle.Top;
        Assert.Equal(DockStyle.Top, control.Dock);
        Assert.Equal(1, callCount);

        // Set different.
        control.Dock = DockStyle.Left;
        Assert.Equal(DockStyle.Left, control.Dock);
        Assert.Equal(2, callCount);

        // Remove handler.
        control.DockChanged -= handler;
        control.Dock = DockStyle.Top;
        Assert.Equal(DockStyle.Top, control.Dock);
        Assert.Equal(2, callCount);
    }

    [WinFormsTheory]
    [InvalidEnumData<DockStyle>]
    public void Control_Dock_SetInvalid_ThrowsInvalidEnumArgumentException(DockStyle value)
    {
        using Control control = new();
        Assert.Throws<InvalidEnumArgumentException>("value", () => control.Dock = value);
    }

    [WinFormsTheory]
    [BoolData]
    public void Control_DoubleBuffered_Get_ReturnsExpected(bool value)
    {
        using SubControl control = new();
        control.SetStyle(ControlStyles.OptimizedDoubleBuffer, value);
        Assert.Equal(value, control.DoubleBuffered);
    }

    [WinFormsTheory]
    [BoolData]
    public void Control_DoubleBuffered_Set_GetReturnsExpected(bool value)
    {
        using SubControl control = new()
        {
            DoubleBuffered = value
        };
        Assert.Equal(value, control.DoubleBuffered);
        Assert.Equal(value, control.GetStyle(ControlStyles.OptimizedDoubleBuffer));
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.DoubleBuffered = value;
        Assert.Equal(value, control.DoubleBuffered);
        Assert.Equal(value, control.GetStyle(ControlStyles.OptimizedDoubleBuffer));
        Assert.False(control.IsHandleCreated);

        // Set different.
        control.DoubleBuffered = !value;
        Assert.Equal(!value, control.DoubleBuffered);
        Assert.Equal(!value, control.GetStyle(ControlStyles.OptimizedDoubleBuffer));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [BoolData]
    public void Control_DoubleBuffered_SetWithHandle_GetReturnsExpected(bool value)
    {
        using SubControl control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.DoubleBuffered = value;
        Assert.Equal(value, control.DoubleBuffered);
        Assert.Equal(value, control.GetStyle(ControlStyles.OptimizedDoubleBuffer));
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.DoubleBuffered = value;
        Assert.Equal(value, control.DoubleBuffered);
        Assert.Equal(value, control.GetStyle(ControlStyles.OptimizedDoubleBuffer));
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set different.
        control.DoubleBuffered = !value;
        Assert.Equal(!value, control.DoubleBuffered);
        Assert.Equal(!value, control.GetStyle(ControlStyles.OptimizedDoubleBuffer));
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [BoolData]
    public void Control_Enabled_Set_GetReturnsExpected(bool value)
    {
        using Control control = new()
        {
            Enabled = value
        };
        Assert.Equal(value, control.Enabled);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.Enabled = value;
        Assert.Equal(value, control.Enabled);
        Assert.False(control.IsHandleCreated);

        // Set different.
        control.Enabled = !value;
        Assert.Equal(!value, control.Enabled);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true, false, 1, 2)]
    [InlineData(true, true, 0, 1)]
    [InlineData(false, false, 0, 0)]
    [InlineData(false, true, 0, 0)]
    public void Control_Enabled_SetWithHandle_GetReturnsExpected(bool userPaint, bool value, int expectedInvalidateCallCount1, int expectedInvalidateCallCount2)
    {
        using SubControl control = new();
        control.SetStyle(ControlStyles.UserPaint, userPaint);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
        Assert.Equal(userPaint, control.GetStyle(ControlStyles.UserPaint));

        control.Enabled = value;
        Assert.Equal(value, control.Enabled);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidateCallCount1, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.Enabled = value;
        Assert.Equal(value, control.Enabled);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidateCallCount1, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set different.
        control.Enabled = !value;
        Assert.Equal(!value, control.Enabled);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidateCallCount2, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void Control_Enabled_SetWithHandler_CallsEnabledChanged()
    {
        using Control control = new()
        {
            Enabled = true
        };
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        control.EnabledChanged += handler;

        // Set different.
        control.Enabled = false;
        Assert.False(control.Enabled);
        Assert.Equal(1, callCount);

        // Set same.
        control.Enabled = false;
        Assert.False(control.Enabled);
        Assert.Equal(1, callCount);

        // Set different.
        control.Enabled = true;
        Assert.True(control.Enabled);
        Assert.Equal(2, callCount);

        // Remove handler.
        control.EnabledChanged -= handler;
        control.Enabled = false;
        Assert.False(control.Enabled);
        Assert.Equal(2, callCount);
    }

    [WinFormsFact]
    public void Control_Enabled_SetWithChildrenWithHandler_CallsEnabledChanged()
    {
        using Control child1 = new();
        using Control child2 = new();
        using Control control = new()
        {
            Enabled = true
        };
        control.Controls.Add(child1);
        control.Controls.Add(child2);

        int callCount = 0;
        int childCallCount1 = 0;
        int childCallCount2 = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        EventHandler childHandler1 = (sender, e) =>
        {
            Assert.Same(child1, sender);
            Assert.Same(EventArgs.Empty, e);
            childCallCount1++;
        };
        EventHandler childHandler2 = (sender, e) =>
        {
            Assert.Same(child2, sender);
            Assert.Same(EventArgs.Empty, e);
            childCallCount2++;
        };
        control.EnabledChanged += handler;
        child1.EnabledChanged += childHandler1;
        child2.EnabledChanged += childHandler2;

        // Set different.
        control.Enabled = false;
        Assert.False(control.Enabled);
        Assert.False(child1.Enabled);
        Assert.False(child2.Enabled);
        Assert.Equal(1, callCount);
        Assert.Equal(1, childCallCount1);
        Assert.Equal(1, childCallCount2);

        // Set same.
        control.Enabled = false;
        Assert.False(control.Enabled);
        Assert.False(child1.Enabled);
        Assert.False(child2.Enabled);
        Assert.Equal(1, callCount);
        Assert.Equal(1, childCallCount1);
        Assert.Equal(1, childCallCount2);

        // Set different.
        control.Enabled = true;
        Assert.True(control.Enabled);
        Assert.True(child1.Enabled);
        Assert.True(child2.Enabled);
        Assert.Equal(2, callCount);
        Assert.Equal(2, childCallCount1);
        Assert.Equal(2, childCallCount2);

        // Remove handler.
        control.EnabledChanged -= handler;
        child1.EnabledChanged -= childHandler1;
        child2.EnabledChanged -= childHandler2;
        control.Enabled = false;
        Assert.False(control.Enabled);
        Assert.False(child1.Enabled);
        Assert.False(child2.Enabled);
        Assert.Equal(2, callCount);
        Assert.Equal(2, childCallCount1);
        Assert.Equal(2, childCallCount2);
    }

    [WinFormsFact]
    public void Control_Enabled_SetWithChildrenDisabledWithHandler_CallsEnabledChanged()
    {
        using Control child1 = new()
        {
            Enabled = false
        };
        using Control child2 = new()
        {
            Enabled = false
        };
        using Control control = new()
        {
            Enabled = true
        };
        control.Controls.Add(child1);
        control.Controls.Add(child2);

        int callCount = 0;
        int childCallCount1 = 0;
        int childCallCount2 = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        EventHandler childHandler1 = (sender, e) =>
        {
            Assert.Same(child1, sender);
            Assert.Same(EventArgs.Empty, e);
            childCallCount1++;
        };
        EventHandler childHandler2 = (sender, e) =>
        {
            Assert.Same(child2, sender);
            Assert.Same(EventArgs.Empty, e);
            childCallCount2++;
        };
        control.EnabledChanged += handler;
        child1.EnabledChanged += childHandler1;
        child2.EnabledChanged += childHandler2;

        // Set different.
        control.Enabled = false;
        Assert.False(control.Enabled);
        Assert.False(child1.Enabled);
        Assert.False(child2.Enabled);
        Assert.Equal(1, callCount);
        Assert.Equal(0, childCallCount1);
        Assert.Equal(0, childCallCount2);

        // Set same.
        control.Enabled = false;
        Assert.False(control.Enabled);
        Assert.False(child1.Enabled);
        Assert.False(child2.Enabled);
        Assert.Equal(1, callCount);
        Assert.Equal(0, childCallCount1);
        Assert.Equal(0, childCallCount2);

        // Set different.
        control.Enabled = true;
        Assert.True(control.Enabled);
        Assert.False(child1.Enabled);
        Assert.False(child2.Enabled);
        Assert.Equal(2, callCount);
        Assert.Equal(0, childCallCount1);
        Assert.Equal(0, childCallCount2);

        // Remove handler.
        control.EnabledChanged -= handler;
        child1.EnabledChanged -= childHandler1;
        child2.EnabledChanged -= childHandler2;
        control.Enabled = false;
        Assert.False(control.Enabled);
        Assert.False(child1.Enabled);
        Assert.False(child2.Enabled);
        Assert.Equal(2, callCount);
        Assert.Equal(0, childCallCount1);
        Assert.Equal(0, childCallCount2);
    }

    [WinFormsFact]
    public void Control_Enabled_ResetValue_Success()
    {
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(Control))[nameof(Control.Enabled)];
        using Control control = new();
        Assert.False(property.CanResetValue(control));

        control.Enabled = false;
        Assert.False(control.Enabled);
        Assert.True(property.CanResetValue(control));

        property.ResetValue(control);
        Assert.True(control.Enabled);
        Assert.False(property.CanResetValue(control));
    }

    [WinFormsFact]
    public void Control_Enabled_ShouldSerializeValue_Success()
    {
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(Control))[nameof(Control.Enabled)];
        using Control control = new();
        Assert.False(property.ShouldSerializeValue(control));

        control.Enabled = false;
        Assert.False(control.Enabled);
        Assert.True(property.ShouldSerializeValue(control));

        property.ResetValue(control);
        Assert.True(control.Enabled);
        Assert.False(property.ShouldSerializeValue(control));
    }

    [WinFormsFact]
    public void Control_Font_GetWithParent_ReturnsExpected()
    {
        using Font font1 = new("Arial", 8.25f);
        using Font font2 = new("Arial", 8.5f);
        using Control parent = new()
        {
            Font = font1
        };
        using Control control = new()
        {
            Parent = parent
        };
        Assert.Same(font1, control.Font);

        // Set custom.
        control.Font = font2;
        Assert.Same(font2, control.Font);
    }

    [WinFormsFact]
    public void Control_Font_GetWithParentCantAccessProperties_ReturnsExpected()
    {
        using Font font1 = new("Arial", 8.25f);
        using Font font2 = new("Arial", 8.5f);
        using SubAxHost parent = new("00000000-0000-0000-0000-000000000000")
        {
            Font = font1
        };
        using Control control = new()
        {
            Parent = parent
        };
        Assert.Equal(Control.DefaultFont, control.Font);

        // Set custom.
        control.Font = font2;
        Assert.Same(font2, control.Font);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetFontTheoryData))]
    public void Control_Font_Set_GetReturnsExpected(Font value)
    {
        using SubControl control = new()
        {
            Font = value
        };
        Assert.Equal(value ?? Control.DefaultFont, control.Font);
        Assert.Equal(control.Font.Height, control.FontHeight);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.Font = value;
        Assert.Equal(value ?? Control.DefaultFont, control.Font);
        Assert.Equal(control.Font.Height, control.FontHeight);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> Font_SetWithFontHeight_TestData()
    {
        Font font = new("Arial", 8.25f);
        yield return new object[] { null, 10 };
        yield return new object[] { font, font.Height };
    }

    [WinFormsTheory]
    [MemberData(nameof(Font_SetWithFontHeight_TestData))]
    public void Control_Font_SetWithFontHeight_GetReturnsExpected(Font value, int expectedFontHeight)
    {
        using SubControl control = new()
        {
            FontHeight = 10,
            Font = value
        };
        Assert.Equal(value ?? Control.DefaultFont, control.Font);
        Assert.Equal(expectedFontHeight, control.FontHeight);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.Font = value;
        Assert.Equal(value ?? Control.DefaultFont, control.Font);
        Assert.Equal(expectedFontHeight, control.FontHeight);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> Font_SetNonNullOldValueWithFontHeight_TestData()
    {
        Font font = new("Arial", 8.25f);
        yield return new object[] { null, Control.DefaultFont.Height };
        yield return new object[] { font, font.Height };
    }

    [WinFormsTheory]
    [MemberData(nameof(Font_SetNonNullOldValueWithFontHeight_TestData))]
    public void Control_Font_SetNonNullOldValueWithFontHeight_GetReturnsExpected(Font value, int expectedFontHeight)
    {
        using SubControl control = new()
        {
            FontHeight = 10,
            Font = new Font("Arial", 1)
        };

        control.Font = value;
        Assert.Equal(value ?? Control.DefaultFont, control.Font);
        Assert.Equal(expectedFontHeight, control.FontHeight);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.Font = value;
        Assert.Equal(value ?? Control.DefaultFont, control.Font);
        Assert.Equal(expectedFontHeight, control.FontHeight);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetFontTheoryData))]
    public void Control_Font_SetWithNonNullOldValue_GetReturnsExpected(Font value)
    {
        using SubControl control = new()
        {
            Font = new Font("Arial", 1)
        };

        control.Font = value;
        Assert.Equal(value ?? Control.DefaultFont, control.Font);
        Assert.Equal(control.Font.Height, control.FontHeight);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.Font = value;
        Assert.Equal(value ?? Control.DefaultFont, control.Font);
        Assert.Equal(control.Font.Height, control.FontHeight);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> Font_SetWithHandle_TestData()
    {
        foreach (bool userPaint in new bool[] { true, false })
        {
            yield return new object[] { userPaint, new Font("Arial", 8.25f), 1 };
            yield return new object[] { userPaint, null, 0 };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(Font_SetWithHandle_TestData))]
    public void Control_Font_SetWithHandle_GetReturnsExpected(bool userPaint, Font value, int expectedInvalidatedCallCount)
    {
        using SubControl control = new();
        control.SetStyle(ControlStyles.UserPaint, userPaint);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        Assert.Equal(userPaint, control.GetStyle(ControlStyles.UserPaint));
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        // Set different.
        control.Font = value;
        Assert.Equal(value ?? Control.DefaultFont, control.Font);
        Assert.Equal(control.Font.Height, control.FontHeight);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.Font = value;
        Assert.Equal(value ?? Control.DefaultFont, control.Font);
        Assert.Equal(control.Font.Height, control.FontHeight);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> Font_SetWithNonNullOldValueWithHandle_TestData()
    {
        foreach (bool userPaint in new bool[] { true, false })
        {
            yield return new object[] { userPaint, new Font("Arial", 8.25f) };
            yield return new object[] { userPaint, null };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(Font_SetWithNonNullOldValueWithHandle_TestData))]
    public void Control_Font_SetWithNonNullOldValueWithHandle_GetReturnsExpected(bool userPaint, Font value)
    {
        using SubControl control = new()
        {
            Font = new Font("Arial", 1)
        };
        control.SetStyle(ControlStyles.UserPaint, userPaint);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        Assert.Equal(userPaint, control.GetStyle(ControlStyles.UserPaint));
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        // Set different.
        control.Font = value;
        Assert.Equal(value ?? Control.DefaultFont, control.Font);
        Assert.Equal(control.Font.Height, control.FontHeight);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(1, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.Font = value;
        Assert.Equal(value ?? Control.DefaultFont, control.Font);
        Assert.Equal(control.Font.Height, control.FontHeight);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(1, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void Control_Font_SetWithHandler_CallsFontChanged()
    {
        using Control control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        control.FontChanged += handler;

        // Set different.
        using Font font1 = new("Arial", 8.25f);
        control.Font = font1;
        Assert.Same(font1, control.Font);
        Assert.Equal(1, callCount);

        // Set same.
        control.Font = font1;
        Assert.Same(font1, control.Font);
        Assert.Equal(1, callCount);

        // Set different.
        using var font2 = SystemFonts.DialogFont;
        control.Font = font2;
        Assert.Same(font2, control.Font);
        Assert.Equal(2, callCount);

        // Set null.
        control.Font = null;
        Assert.Equal(Control.DefaultFont, control.Font);
        Assert.Equal(3, callCount);

        // Remove handler.
        control.FontChanged -= handler;
        control.Font = font1;
        Assert.Same(font1, control.Font);
        Assert.Equal(3, callCount);
    }

    [WinFormsFact]
    public void Control_Font_SetWithChildrenWithHandler_CallsFontChanged()
    {
        using Control child1 = new();
        using Control child2 = new();
        using Control control = new();
        control.Controls.Add(child1);
        control.Controls.Add(child2);

        int callCount = 0;
        int childCallCount1 = 0;
        int childCallCount2 = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        EventHandler childHandler1 = (sender, e) =>
        {
            Assert.Same(child1, sender);
            Assert.Same(EventArgs.Empty, e);
            childCallCount1++;
        };
        EventHandler childHandler2 = (sender, e) =>
        {
            Assert.Same(child2, sender);
            Assert.Same(EventArgs.Empty, e);
            childCallCount2++;
        };
        control.FontChanged += handler;
        child1.FontChanged += childHandler1;
        child2.FontChanged += childHandler2;

        // Set different.
        using Font font1 = new("Arial", 8.25f);
        control.Font = font1;
        Assert.Same(font1, control.Font);
        Assert.Same(font1, child1.Font);
        Assert.Same(font1, child2.Font);
        Assert.Equal(1, callCount);
        Assert.Equal(1, childCallCount1);
        Assert.Equal(1, childCallCount2);

        // Set same.
        control.Font = font1;
        Assert.Same(font1, control.Font);
        Assert.Same(font1, child1.Font);
        Assert.Same(font1, child2.Font);
        Assert.Equal(1, callCount);
        Assert.Equal(1, childCallCount1);
        Assert.Equal(1, childCallCount2);

        // Set different.
        using var font2 = SystemFonts.DialogFont;
        control.Font = font2;
        Assert.Same(font2, control.Font);
        Assert.Same(font2, child1.Font);
        Assert.Same(font2, child2.Font);
        Assert.Equal(2, callCount);
        Assert.Equal(2, childCallCount1);
        Assert.Equal(2, childCallCount2);

        // Set null.
        control.Font = null;
        Assert.Equal(Control.DefaultFont, control.Font);
        Assert.Equal(Control.DefaultFont, child1.Font);
        Assert.Equal(Control.DefaultFont, child2.Font);
        Assert.Equal(3, callCount);
        Assert.Equal(3, childCallCount1);
        Assert.Equal(3, childCallCount2);

        // Remove handler.
        control.FontChanged -= handler;
        child1.FontChanged -= childHandler1;
        child2.FontChanged -= childHandler2;
        control.Font = font1;
        Assert.Same(font1, control.Font);
        Assert.Same(font1, child1.Font);
        Assert.Same(font1, child2.Font);
        Assert.Equal(3, callCount);
        Assert.Equal(3, childCallCount1);
        Assert.Equal(3, childCallCount2);
    }

    [WinFormsFact]
    public void Control_Font_SetWithChildrenWithFontWithHandler_CallsFontChanged()
    {
        using Font childFont1 = new("Arial", 1);
        using Font childFont2 = new("Arial", 1);
        using Control child1 = new()
        {
            Font = childFont1
        };
        using Control child2 = new()
        {
            Font = childFont2
        };
        using Control control = new();
        control.Controls.Add(child1);
        control.Controls.Add(child2);

        int callCount = 0;
        int childCallCount1 = 0;
        int childCallCount2 = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        EventHandler childHandler1 = (sender, e) =>
        {
            Assert.Same(child1, sender);
            Assert.Same(EventArgs.Empty, e);
            childCallCount1++;
        };
        EventHandler childHandler2 = (sender, e) =>
        {
            Assert.Same(child2, sender);
            Assert.Same(EventArgs.Empty, e);
            childCallCount2++;
        };
        control.FontChanged += handler;
        child1.FontChanged += childHandler1;
        child2.FontChanged += childHandler2;

        // Set different.
        using Font font1 = new("Arial", 8.25f);
        control.Font = font1;
        Assert.Same(font1, control.Font);
        Assert.Same(childFont1, child1.Font);
        Assert.Same(childFont2, child2.Font);
        Assert.Equal(1, callCount);
        Assert.Equal(0, childCallCount1);
        Assert.Equal(0, childCallCount2);

        // Set same.
        control.Font = font1;
        Assert.Same(font1, control.Font);
        Assert.Same(childFont1, child1.Font);
        Assert.Same(childFont2, child2.Font);
        Assert.Equal(1, callCount);
        Assert.Equal(0, childCallCount1);
        Assert.Equal(0, childCallCount2);

        // Set different.
        using var font2 = SystemFonts.DialogFont;
        control.Font = font2;
        Assert.Same(font2, control.Font);
        Assert.Same(childFont1, child1.Font);
        Assert.Same(childFont2, child2.Font);
        Assert.Equal(2, callCount);
        Assert.Equal(0, childCallCount1);
        Assert.Equal(0, childCallCount2);

        // Set null.
        control.Font = null;
        Assert.Equal(Control.DefaultFont, control.Font);
        Assert.Same(childFont1, child1.Font);
        Assert.Same(childFont2, child2.Font);
        Assert.Equal(3, callCount);
        Assert.Equal(0, childCallCount1);
        Assert.Equal(0, childCallCount2);

        // Remove handler.
        control.FontChanged -= handler;
        child1.FontChanged -= childHandler1;
        child2.FontChanged -= childHandler2;
        control.Font = font1;
        Assert.Same(font1, control.Font);
        Assert.Same(childFont1, child1.Font);
        Assert.Same(childFont2, child2.Font);
        Assert.Equal(3, callCount);
        Assert.Equal(0, childCallCount1);
        Assert.Equal(0, childCallCount2);
    }

    [WinFormsFact]
    public void Control_Font_ResetValue_Success()
    {
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(Control))[nameof(Control.Font)];
        using Control control = new();
        Assert.False(property.CanResetValue(control));

        using Font font = new("Arial", 8.25f);
        control.Font = font;
        Assert.Same(font, control.Font);
        Assert.True(property.CanResetValue(control));

        control.Font = null;
        Assert.Equal(Control.DefaultFont, control.Font);
        Assert.False(property.CanResetValue(control));

        control.Font = font;
        Assert.Same(font, control.Font);
        Assert.True(property.CanResetValue(control));

        property.ResetValue(control);
        Assert.Equal(Control.DefaultFont, control.Font);
        Assert.False(property.CanResetValue(control));
    }

    [WinFormsFact]
    public void Control_Font_ShouldSerializeValue_Success()
    {
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(Control))[nameof(Control.Font)];
        using Control control = new();
        Assert.False(property.ShouldSerializeValue(control));

        using Font font = new("Arial", 8.25f);
        control.Font = font;
        Assert.Same(font, control.Font);
        Assert.True(property.ShouldSerializeValue(control));

        control.Font = null;
        Assert.Equal(Control.DefaultFont, control.Font);
        Assert.False(property.ShouldSerializeValue(control));

        control.Font = font;
        Assert.Same(font, control.Font);
        Assert.True(property.ShouldSerializeValue(control));

        property.ResetValue(control);
        Assert.Equal(Control.DefaultFont, control.Font);
        Assert.False(property.ShouldSerializeValue(control));
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetForeColorTheoryData))]
    public void Control_ForeColor_Set_GetReturnsExpected(Color value, Color expected)
    {
        using Control control = new()
        {
            ForeColor = value
        };
        Assert.Equal(expected, control.ForeColor);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.ForeColor = value;
        Assert.Equal(expected, control.ForeColor);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetForeColorTheoryData))]
    public void Control_ForeColor_SetWithCustomOldValue_GetReturnsExpected(Color value, Color expected)
    {
        using Control control = new()
        {
            ForeColor = Color.YellowGreen
        };

        control.ForeColor = value;
        Assert.Equal(expected, control.ForeColor);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.ForeColor = value;
        Assert.Equal(expected, control.ForeColor);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetForeColorTheoryData))]
    public void Control_ForeColor_SetWithChildren_GetReturnsExpected(Color value, Color expected)
    {
        using Control child1 = new();
        using Control child2 = new();
        using Control control = new();
        control.Controls.Add(child1);
        control.Controls.Add(child2);

        control.ForeColor = value;
        Assert.Equal(expected, control.ForeColor);
        Assert.Equal(expected, child1.ForeColor);
        Assert.Equal(expected, child2.ForeColor);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.ForeColor = value;
        Assert.Equal(expected, control.ForeColor);
        Assert.Equal(expected, child1.ForeColor);
        Assert.Equal(expected, child2.ForeColor);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetForeColorTheoryData))]
    public void Control_ForeColor_SetWithChildrenWithColor_GetReturnsExpected(Color value, Color expected)
    {
        using Control child1 = new()
        {
            ForeColor = Color.Yellow
        };
        using Control child2 = new()
        {
            ForeColor = Color.YellowGreen
        };
        using Control control = new();
        control.Controls.Add(child1);
        control.Controls.Add(child2);

        control.ForeColor = value;
        Assert.Equal(expected, control.ForeColor);
        Assert.Equal(Color.Yellow, child1.ForeColor);
        Assert.Equal(Color.YellowGreen, child2.ForeColor);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.ForeColor = value;
        Assert.Equal(expected, control.ForeColor);
        Assert.Equal(Color.Yellow, child1.ForeColor);
        Assert.Equal(Color.YellowGreen, child2.ForeColor);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> ForeColor_SetWithHandle_TestData()
    {
        yield return new object[] { Color.Red, Color.Red, 1 };
        yield return new object[] { Color.FromArgb(254, 1, 2, 3), Color.FromArgb(254, 1, 2, 3), 1 };
        yield return new object[] { Color.Empty, Control.DefaultForeColor, 0 };
    }

    [WinFormsTheory]
    [MemberData(nameof(ForeColor_SetWithHandle_TestData))]
    public void Control_ForeColor_SetWithHandle_GetReturnsExpected(Color value, Color expected, int expectedInvalidatedCallCount)
    {
        using Control control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.ForeColor = value;
        Assert.Equal(expected, control.ForeColor);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.ForeColor = value;
        Assert.Equal(expected, control.ForeColor);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void Control_ForeColor_SetWithHandler_CallsForeColorChanged()
    {
        using Control control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        control.ForeColorChanged += handler;

        // Set different.
        control.ForeColor = Color.Red;
        Assert.Equal(Color.Red, control.ForeColor);
        Assert.Equal(1, callCount);

        // Set same.
        control.ForeColor = Color.Red;
        Assert.Equal(Color.Red, control.ForeColor);
        Assert.Equal(1, callCount);

        // Set different.
        control.ForeColor = Color.Empty;
        Assert.Equal(Control.DefaultForeColor, control.ForeColor);
        Assert.Equal(2, callCount);

        // Remove handler.
        control.ForeColorChanged -= handler;
        control.ForeColor = Color.Red;
        Assert.Equal(Color.Red, control.ForeColor);
        Assert.Equal(2, callCount);
    }

    [WinFormsFact]
    public void Control_ForeColor_SetWithHandlerInDisposing_DoesNotCallForeColorChanged()
    {
        using Control control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);

        int callCount = 0;
        control.ForeColorChanged += (sender, e) => callCount++;
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;

        int disposedCallCount = 0;
        control.Disposed += (sender, e) =>
        {
            control.ForeColor = Color.Red;
            Assert.Equal(Color.Red, control.ForeColor);
            Assert.Equal(0, callCount);
            Assert.Equal(0, invalidatedCallCount);
            disposedCallCount++;
        };

        control.Dispose();
        Assert.Equal(1, disposedCallCount);
    }

    [WinFormsFact]
    public void Control_ForeColor_SetWithChildrenWithHandler_CallsForeColorChanged()
    {
        using Control child1 = new();
        using Control child2 = new();
        using Control control = new();
        control.Controls.Add(child1);
        control.Controls.Add(child2);

        int callCount = 0;
        int childCallCount1 = 0;
        int childCallCount2 = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(callCount, childCallCount1);
            Assert.Equal(childCallCount1, childCallCount2);
            callCount++;
        };
        EventHandler childHandler1 = (sender, e) =>
        {
            Assert.Same(child1, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(callCount - 1, childCallCount1);
            Assert.Equal(childCallCount1, childCallCount2);
            childCallCount1++;
        };
        EventHandler childHandler2 = (sender, e) =>
        {
            Assert.Same(child2, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(callCount, childCallCount1);
            Assert.Equal(childCallCount1 - 1, childCallCount2);
            childCallCount2++;
        };
        control.ForeColorChanged += handler;
        child1.ForeColorChanged += childHandler1;
        child2.ForeColorChanged += childHandler2;

        // Set different.
        control.ForeColor = Color.Red;
        Assert.Equal(Color.Red, control.ForeColor);
        Assert.Equal(Color.Red, child1.ForeColor);
        Assert.Equal(Color.Red, child2.ForeColor);
        Assert.Equal(1, callCount);
        Assert.Equal(1, childCallCount1);
        Assert.Equal(1, childCallCount2);

        // Set same.
        control.ForeColor = Color.Red;
        Assert.Equal(Color.Red, control.ForeColor);
        Assert.Equal(Color.Red, child1.ForeColor);
        Assert.Equal(Color.Red, child2.ForeColor);
        Assert.Equal(1, callCount);
        Assert.Equal(1, childCallCount1);
        Assert.Equal(1, childCallCount2);

        // Set different.
        control.ForeColor = Color.Empty;
        Assert.Equal(Control.DefaultForeColor, control.ForeColor);
        Assert.Equal(Control.DefaultForeColor, child1.ForeColor);
        Assert.Equal(Control.DefaultForeColor, child2.ForeColor);
        Assert.Equal(2, callCount);
        Assert.Equal(2, childCallCount1);
        Assert.Equal(2, childCallCount2);

        // Remove handler.
        control.ForeColorChanged -= handler;
        child1.ForeColorChanged -= childHandler1;
        child2.ForeColorChanged -= childHandler2;
        control.ForeColor = Color.Red;
        Assert.Equal(Color.Red, control.ForeColor);
        Assert.Equal(Color.Red, child1.ForeColor);
        Assert.Equal(Color.Red, child2.ForeColor);
        Assert.Equal(2, callCount);
        Assert.Equal(2, childCallCount1);
        Assert.Equal(2, childCallCount2);
    }

    [WinFormsFact]
    public void Control_ForeColor_SetWithChildrenWithForeColorWithHandler_CallsForeColorChanged()
    {
        using Control child1 = new()
        {
            ForeColor = Color.Yellow
        };
        using Control child2 = new()
        {
            ForeColor = Color.YellowGreen
        };
        using Control control = new();
        control.Controls.Add(child1);
        control.Controls.Add(child2);

        int callCount = 0;
        int childCallCount1 = 0;
        int childCallCount2 = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        EventHandler childHandler1 = (sender, e) =>
        {
            Assert.Same(child1, sender);
            Assert.Same(EventArgs.Empty, e);
            childCallCount1++;
        };
        EventHandler childHandler2 = (sender, e) =>
        {
            Assert.Same(child2, sender);
            Assert.Same(EventArgs.Empty, e);
            childCallCount2++;
        };
        control.ForeColorChanged += handler;
        child1.ForeColorChanged += childHandler1;
        child2.ForeColorChanged += childHandler2;

        // Set different.
        control.ForeColor = Color.Red;
        Assert.Equal(Color.Red, control.ForeColor);
        Assert.Equal(Color.Yellow, child1.ForeColor);
        Assert.Equal(Color.YellowGreen, child2.ForeColor);
        Assert.Equal(1, callCount);
        Assert.Equal(0, childCallCount1);
        Assert.Equal(0, childCallCount2);

        // Set same.
        control.ForeColor = Color.Red;
        Assert.Equal(Color.Red, control.ForeColor);
        Assert.Equal(Color.Yellow, child1.ForeColor);
        Assert.Equal(Color.YellowGreen, child2.ForeColor);
        Assert.Equal(1, callCount);
        Assert.Equal(0, childCallCount1);
        Assert.Equal(0, childCallCount2);

        // Set different.
        control.ForeColor = Color.Empty;
        Assert.Equal(Control.DefaultForeColor, control.ForeColor);
        Assert.Equal(Color.Yellow, child1.ForeColor);
        Assert.Equal(Color.YellowGreen, child2.ForeColor);
        Assert.Equal(2, callCount);
        Assert.Equal(0, childCallCount1);
        Assert.Equal(0, childCallCount2);

        // Remove handler.
        control.ForeColorChanged -= handler;
        child1.ForeColorChanged -= childHandler1;
        child2.ForeColorChanged -= childHandler2;
        control.ForeColor = Color.Red;
        Assert.Equal(Color.Red, control.ForeColor);
        Assert.Equal(Color.Yellow, child1.ForeColor);
        Assert.Equal(Color.YellowGreen, child2.ForeColor);
        Assert.Equal(2, callCount);
        Assert.Equal(0, childCallCount1);
        Assert.Equal(0, childCallCount2);
    }

    [WinFormsFact]
    public void Control_ForeColor_ResetValue_Success()
    {
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(Control))[nameof(Control.ForeColor)];
        using Control control = new();
        Assert.False(property.CanResetValue(control));

        control.ForeColor = Color.Red;
        Assert.Equal(Color.Red, control.ForeColor);
        Assert.True(property.CanResetValue(control));

        property.ResetValue(control);
        Assert.Equal(Control.DefaultForeColor, control.ForeColor);
        Assert.False(property.CanResetValue(control));
    }

    [WinFormsFact]
    public void Control_ForeColor_ShouldSerializeValue_Success()
    {
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(Control))[nameof(Control.ForeColor)];
        using Control control = new();
        Assert.False(property.ShouldSerializeValue(control));

        control.ForeColor = Color.Red;
        Assert.Equal(Color.Red, control.ForeColor);
        Assert.True(property.ShouldSerializeValue(control));

        property.ResetValue(control);
        Assert.Equal(Control.DefaultForeColor, control.ForeColor);
        Assert.False(property.ShouldSerializeValue(control));
    }

    [WinFormsFact]
    public void Control_GetHandle()
    {
        using Control cont = new();

        IntPtr intptr = cont.Handle;

        Assert.NotEqual(IntPtr.Zero, intptr);
    }

    [WinFormsTheory]
    [InlineData(-4, 1)]
    [InlineData(0, 0)]
    [InlineData(2, 1)]
    [InlineData(40, 1)]
    public void Control_Height_Set_GetReturnsExpected(int value, int expectedLayoutCallCount)
    {
        using SubControl control = new();
        int layoutCallCount = 0;
        int resizeCallCount = 0;
        int sizeChangedCallCount = 0;
        int clientSizeChangedCallCount = 0;
        control.Layout += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("Bounds", e.AffectedProperty);
            Assert.Equal(resizeCallCount, layoutCallCount);
            Assert.Equal(sizeChangedCallCount, layoutCallCount);
            Assert.Equal(clientSizeChangedCallCount, layoutCallCount);
            layoutCallCount++;
        };
        control.Resize += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(layoutCallCount - 1, resizeCallCount);
            Assert.Equal(sizeChangedCallCount, resizeCallCount);
            Assert.Equal(clientSizeChangedCallCount, resizeCallCount);
            resizeCallCount++;
        };
        control.SizeChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(resizeCallCount - 1, sizeChangedCallCount);
            Assert.Equal(layoutCallCount - 1, sizeChangedCallCount);
            Assert.Equal(clientSizeChangedCallCount, sizeChangedCallCount);
            sizeChangedCallCount++;
        };
        control.ClientSizeChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(resizeCallCount - 1, clientSizeChangedCallCount);
            Assert.Equal(layoutCallCount - 1, clientSizeChangedCallCount);
            Assert.Equal(sizeChangedCallCount - 1, clientSizeChangedCallCount);
            clientSizeChangedCallCount++;
        };

        control.Height = value;
        Assert.Equal(new Size(0, value), control.ClientSize);
        Assert.Equal(new Rectangle(0, 0, 0, value), control.ClientRectangle);
        Assert.Equal(new Rectangle(0, 0, 0, value), control.DisplayRectangle);
        Assert.Equal(new Size(0, value), control.Size);
        Assert.Equal(0, control.Left);
        Assert.Equal(0, control.Right);
        Assert.Equal(0, control.Top);
        Assert.Equal(value, control.Bottom);
        Assert.Equal(0, control.Width);
        Assert.Equal(value, control.Height);
        Assert.Equal(new Rectangle(0, 0, 0, value), control.Bounds);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.Equal(expectedLayoutCallCount, resizeCallCount);
        Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
        Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
        Assert.False(control.IsHandleCreated);

        // Call again.
        control.Height = value;
        Assert.Equal(new Size(0, value), control.ClientSize);
        Assert.Equal(new Rectangle(0, 0, 0, value), control.ClientRectangle);
        Assert.Equal(new Rectangle(0, 0, 0, value), control.DisplayRectangle);
        Assert.Equal(new Size(0, value), control.Size);
        Assert.Equal(0, control.Left);
        Assert.Equal(0, control.Right);
        Assert.Equal(0, control.Top);
        Assert.Equal(value, control.Bottom);
        Assert.Equal(0, control.Width);
        Assert.Equal(value, control.Height);
        Assert.Equal(new Rectangle(0, 0, 0, value), control.Bounds);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.Equal(expectedLayoutCallCount, resizeCallCount);
        Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
        Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> Height_Set_WithConstrainedSize_TestData()
    {
        yield return new object[] { Size.Empty, Size.Empty, 40, 0, 40, 1 };
        yield return new object[] { new Size(10, 20), Size.Empty, 40, 10, 40, 1 };
        yield return new object[] { new Size(30, 40), Size.Empty, 40, 30, 40, 0 };
        yield return new object[] { new Size(31, 40), Size.Empty, 40, 31, 40, 0 };
        yield return new object[] { new Size(30, 41), Size.Empty, 40, 30, 41, 0 };
        yield return new object[] { new Size(40, 50), Size.Empty, 40, 40, 50, 0 };
        yield return new object[] { Size.Empty, new Size(20, 10), 40, 0, 10, 1 };
        yield return new object[] { Size.Empty, new Size(30, 40), 40, 0, 40, 1 };
        yield return new object[] { Size.Empty, new Size(31, 40), 40, 0, 40, 1 };
        yield return new object[] { Size.Empty, new Size(30, 41), 40, 0, 40, 1 };
        yield return new object[] { Size.Empty, new Size(40, 50), 40, 0, 40, 1 };
        yield return new object[] { new Size(10, 20), new Size(40, 50), 40, 10, 40, 1 };
        yield return new object[] { new Size(10, 20), new Size(20, 30), 40, 10, 30, 1 };
        yield return new object[] { new Size(30, 40), new Size(20, 30), 40, 30, 40, 0 };
        yield return new object[] { new Size(30, 40), new Size(40, 50), 40, 30, 40, 0 };
        yield return new object[] { new Size(40, 50), new Size(20, 30), 40, 40, 50, 0 };
        yield return new object[] { new Size(40, 50), new Size(40, 50), 40, 40, 50, 0 };
    }

    [WinFormsTheory]
    [MemberData(nameof(Height_Set_WithConstrainedSize_TestData))]
    public void Control_Height_SetWithConstrainedSize_GetReturnsExpected(Size minimumSize, Size maximumSize, int value, int expectedWidth, int expectedHeight, int expectedLayoutCallCount)
    {
        using SubControl control = new()
        {
            MinimumSize = minimumSize,
            MaximumSize = maximumSize
        };
        int layoutCallCount = 0;
        int resizeCallCount = 0;
        int sizeChangedCallCount = 0;
        int clientSizeChangedCallCount = 0;
        control.Layout += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("Bounds", e.AffectedProperty);
            Assert.Equal(resizeCallCount, layoutCallCount);
            Assert.Equal(sizeChangedCallCount, layoutCallCount);
            Assert.Equal(clientSizeChangedCallCount, layoutCallCount);
            layoutCallCount++;
        };
        control.Resize += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(layoutCallCount - 1, resizeCallCount);
            Assert.Equal(sizeChangedCallCount, resizeCallCount);
            Assert.Equal(clientSizeChangedCallCount, resizeCallCount);
            resizeCallCount++;
        };
        control.SizeChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(resizeCallCount - 1, sizeChangedCallCount);
            Assert.Equal(layoutCallCount - 1, sizeChangedCallCount);
            Assert.Equal(clientSizeChangedCallCount, sizeChangedCallCount);
            sizeChangedCallCount++;
        };
        control.ClientSizeChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(resizeCallCount - 1, clientSizeChangedCallCount);
            Assert.Equal(layoutCallCount - 1, clientSizeChangedCallCount);
            Assert.Equal(sizeChangedCallCount - 1, clientSizeChangedCallCount);
            clientSizeChangedCallCount++;
        };

        control.Height = value;
        Assert.Equal(new Size(expectedWidth, expectedHeight), control.ClientSize);
        Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.ClientRectangle);
        Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.DisplayRectangle);
        Assert.Equal(new Size(expectedWidth, expectedHeight), control.Size);
        Assert.Equal(0, control.Left);
        Assert.Equal(expectedWidth, control.Right);
        Assert.Equal(0, control.Top);
        Assert.Equal(expectedHeight, control.Bottom);
        Assert.Equal(expectedWidth, control.Width);
        Assert.Equal(expectedHeight, control.Height);
        Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.Bounds);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.Equal(expectedLayoutCallCount, resizeCallCount);
        Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
        Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
        Assert.False(control.IsHandleCreated);

        // Call again.
        control.Height = value;
        Assert.Equal(new Size(expectedWidth, expectedHeight), control.ClientSize);
        Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.ClientRectangle);
        Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.DisplayRectangle);
        Assert.Equal(new Size(expectedWidth, expectedHeight), control.Size);
        Assert.Equal(0, control.Left);
        Assert.Equal(expectedWidth, control.Right);
        Assert.Equal(0, control.Top);
        Assert.Equal(expectedHeight, control.Bottom);
        Assert.Equal(expectedWidth, control.Width);
        Assert.Equal(expectedHeight, control.Height);
        Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.Bounds);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.Equal(expectedLayoutCallCount, resizeCallCount);
        Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
        Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(-4, -4, -8, 1)]
    [InlineData(0, 0, 0, 0)]
    [InlineData(2, -4, -2, 1)]
    [InlineData(40, -4, 36, 1)]
    public void Control_Height_SetWithCustomStyle_GetReturnsExpected(int value, int expectedClientWidth, int expectedClientHeight, int expectedLayoutCallCount)
    {
        using BorderedControl control = new();
        int layoutCallCount = 0;
        int resizeCallCount = 0;
        int sizeChangedCallCount = 0;
        int clientSizeChangedCallCount = 0;
        control.Layout += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("Bounds", e.AffectedProperty);
            Assert.Equal(resizeCallCount, layoutCallCount);
            Assert.Equal(sizeChangedCallCount, layoutCallCount);
            Assert.Equal(clientSizeChangedCallCount, layoutCallCount);
            layoutCallCount++;
        };
        control.Resize += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(layoutCallCount - 1, resizeCallCount);
            Assert.Equal(sizeChangedCallCount, resizeCallCount);
            Assert.Equal(clientSizeChangedCallCount, resizeCallCount);
            resizeCallCount++;
        };
        control.SizeChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(resizeCallCount - 1, sizeChangedCallCount);
            Assert.Equal(layoutCallCount - 1, sizeChangedCallCount);
            Assert.Equal(clientSizeChangedCallCount, sizeChangedCallCount);
            sizeChangedCallCount++;
        };
        control.ClientSizeChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(resizeCallCount - 1, clientSizeChangedCallCount);
            Assert.Equal(layoutCallCount - 1, clientSizeChangedCallCount);
            Assert.Equal(sizeChangedCallCount - 1, clientSizeChangedCallCount);
            clientSizeChangedCallCount++;
        };

        control.Height = value;
        Assert.Equal(new Size(expectedClientWidth, expectedClientHeight), control.ClientSize);
        Assert.Equal(new Rectangle(0, 0, expectedClientWidth, expectedClientHeight), control.ClientRectangle);
        Assert.Equal(new Rectangle(0, 0, expectedClientWidth, expectedClientHeight), control.DisplayRectangle);
        Assert.Equal(new Size(0, value), control.Size);
        Assert.Equal(0, control.Left);
        Assert.Equal(0, control.Right);
        Assert.Equal(0, control.Top);
        Assert.Equal(value, control.Bottom);
        Assert.Equal(0, control.Width);
        Assert.Equal(value, control.Height);
        Assert.Equal(new Rectangle(0, 0, 0, value), control.Bounds);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.Equal(expectedLayoutCallCount, resizeCallCount);
        Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
        Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
        Assert.False(control.IsHandleCreated);

        // Call again.
        control.Height = value;
        Assert.Equal(new Size(expectedClientWidth, expectedClientHeight), control.ClientSize);
        Assert.Equal(new Rectangle(0, 0, expectedClientWidth, expectedClientHeight), control.ClientRectangle);
        Assert.Equal(new Rectangle(0, 0, expectedClientWidth, expectedClientHeight), control.DisplayRectangle);
        Assert.Equal(new Size(0, value), control.Size);
        Assert.Equal(0, control.Left);
        Assert.Equal(0, control.Right);
        Assert.Equal(0, control.Top);
        Assert.Equal(value, control.Bottom);
        Assert.Equal(0, control.Width);
        Assert.Equal(value, control.Height);
        Assert.Equal(new Rectangle(0, 0, 0, value), control.Bounds);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.Equal(expectedLayoutCallCount, resizeCallCount);
        Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
        Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(-4, 1, 2)]
    [InlineData(0, 0, 0)]
    [InlineData(2, 1, 2)]
    [InlineData(40, 1, 2)]
    public void Control_Height_SetWithParent_GetReturnsExpected(int value, int expectedLayoutCallCount, int expectedParentLayoutCallCount)
    {
        using Control parent = new();
        using SubControl control = new()
        {
            Parent = parent
        };
        int layoutCallCount = 0;
        int resizeCallCount = 0;
        int sizeChangedCallCount = 0;
        int clientSizeChangedCallCount = 0;
        int parentLayoutCallCount = 0;
        control.Layout += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("Bounds", e.AffectedProperty);
            Assert.Equal(resizeCallCount, layoutCallCount);
            Assert.Equal(sizeChangedCallCount, layoutCallCount);
            Assert.Equal(clientSizeChangedCallCount, layoutCallCount);
            Assert.Equal(parentLayoutCallCount, layoutCallCount);
            layoutCallCount++;
        };
        control.Resize += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(layoutCallCount - 1, resizeCallCount);
            Assert.Equal(sizeChangedCallCount, resizeCallCount);
            Assert.Equal(clientSizeChangedCallCount, resizeCallCount);
            Assert.Equal(parentLayoutCallCount, resizeCallCount);
            resizeCallCount++;
        };
        control.SizeChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(resizeCallCount - 1, sizeChangedCallCount);
            Assert.Equal(layoutCallCount - 1, sizeChangedCallCount);
            Assert.Equal(clientSizeChangedCallCount, sizeChangedCallCount);
            Assert.Equal(parentLayoutCallCount, sizeChangedCallCount);
            sizeChangedCallCount++;
        };
        control.ClientSizeChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(resizeCallCount - 1, clientSizeChangedCallCount);
            Assert.Equal(layoutCallCount - 1, clientSizeChangedCallCount);
            Assert.Equal(sizeChangedCallCount - 1, clientSizeChangedCallCount);
            Assert.Equal(parentLayoutCallCount, clientSizeChangedCallCount);
            clientSizeChangedCallCount++;
        };
        void parentHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(parent, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("Bounds", e.AffectedProperty);
            parentLayoutCallCount++;
        }

        parent.Layout += parentHandler;

        try
        {
            control.Height = value;
            Assert.Equal(new Size(0, value), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, 0, value), control.ClientRectangle);
            Assert.Equal(new Rectangle(0, 0, 0, value), control.DisplayRectangle);
            Assert.Equal(new Size(0, value), control.Size);
            Assert.Equal(0, control.Left);
            Assert.Equal(0, control.Right);
            Assert.Equal(0, control.Top);
            Assert.Equal(value, control.Bottom);
            Assert.Equal(0, control.Width);
            Assert.Equal(value, control.Height);
            Assert.Equal(new Rectangle(0, 0, 0, value), control.Bounds);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.Equal(expectedLayoutCallCount, resizeCallCount);
            Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
            Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
            Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
            Assert.False(control.IsHandleCreated);

            // Call again.
            control.Height = value;
            Assert.Equal(new Size(0, value), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, 0, value), control.ClientRectangle);
            Assert.Equal(new Rectangle(0, 0, 0, value), control.DisplayRectangle);
            Assert.Equal(new Size(0, value), control.Size);
            Assert.Equal(0, control.Left);
            Assert.Equal(0, control.Right);
            Assert.Equal(0, control.Top);
            Assert.Equal(value, control.Bottom);
            Assert.Equal(0, control.Width);
            Assert.Equal(value, control.Height);
            Assert.Equal(new Rectangle(0, 0, 0, value), control.Bounds);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.Equal(expectedLayoutCallCount, resizeCallCount);
            Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
            Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
            Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
            Assert.False(control.IsHandleCreated);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    public static IEnumerable<object[]> Height_SetWithHandle_TestData()
    {
        yield return new object[] { true, -4, 0, 0, 0 };
        yield return new object[] { true, 0, 0, 0, 0 };
        yield return new object[] { true, 2, 2, 1, 1 };
        yield return new object[] { true, 40, 40, 1, 1 };
        yield return new object[] { false, -4, 0, 0, 0 };
        yield return new object[] { false, 0, 0, 0, 0 };
        yield return new object[] { false, 2, 2, 1, 0 };
        yield return new object[] { false, 40, 40, 1, 0 };
    }

    [WinFormsTheory]
    [MemberData(nameof(Height_SetWithHandle_TestData))]
    public void Control_Height_SetWithHandle_GetReturnsExpected(bool resizeRedraw, int value, int expectedHeight, int expectedLayoutCallCount, int expectedInvalidatedCallCount)
    {
        using SubControl control = new();
        control.SetStyle(ControlStyles.ResizeRedraw, resizeRedraw);
        int layoutCallCount = 0;
        int resizeCallCount = 0;
        int sizeChangedCallCount = 0;
        int clientSizeChangedCallCount = 0;
        control.Layout += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("Bounds", e.AffectedProperty);
            Assert.Equal(resizeCallCount, layoutCallCount);
            Assert.Equal(sizeChangedCallCount, layoutCallCount);
            Assert.Equal(clientSizeChangedCallCount, layoutCallCount);
            layoutCallCount++;
        };
        control.Resize += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(layoutCallCount - 1, resizeCallCount);
            Assert.Equal(sizeChangedCallCount, resizeCallCount);
            Assert.Equal(clientSizeChangedCallCount, resizeCallCount);
            resizeCallCount++;
        };
        control.SizeChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(resizeCallCount - 1, sizeChangedCallCount);
            Assert.Equal(layoutCallCount - 1, sizeChangedCallCount);
            Assert.Equal(clientSizeChangedCallCount, sizeChangedCallCount);
            sizeChangedCallCount++;
        };
        control.ClientSizeChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(resizeCallCount - 1, clientSizeChangedCallCount);
            Assert.Equal(layoutCallCount - 1, clientSizeChangedCallCount);
            Assert.Equal(sizeChangedCallCount - 1, clientSizeChangedCallCount);
            clientSizeChangedCallCount++;
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.Height = value;
        Assert.Equal(new Size(0, expectedHeight), control.ClientSize);
        Assert.Equal(new Rectangle(0, 0, 0, expectedHeight), control.ClientRectangle);
        Assert.Equal(new Rectangle(0, 0, 0, expectedHeight), control.DisplayRectangle);
        Assert.Equal(new Size(0, expectedHeight), control.Size);
        Assert.Equal(0, control.Left);
        Assert.Equal(0, control.Right);
        Assert.Equal(0, control.Top);
        Assert.Equal(expectedHeight, control.Bottom);
        Assert.Equal(0, control.Width);
        Assert.Equal(expectedHeight, control.Height);
        Assert.Equal(new Rectangle(0, 0, 0, expectedHeight), control.Bounds);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.Equal(expectedLayoutCallCount, resizeCallCount);
        Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
        Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Call again.
        control.Height = value;
        Assert.Equal(new Size(0, expectedHeight), control.ClientSize);
        Assert.Equal(new Rectangle(0, 0, 0, expectedHeight), control.ClientRectangle);
        Assert.Equal(new Rectangle(0, 0, 0, expectedHeight), control.DisplayRectangle);
        Assert.Equal(new Size(0, expectedHeight), control.Size);
        Assert.Equal(0, control.Left);
        Assert.Equal(0, control.Right);
        Assert.Equal(0, control.Top);
        Assert.Equal(expectedHeight, control.Bottom);
        Assert.Equal(0, control.Width);
        Assert.Equal(expectedHeight, control.Height);
        Assert.Equal(new Rectangle(0, 0, 0, expectedHeight), control.Bounds);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.Equal(expectedLayoutCallCount, resizeCallCount);
        Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
        Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> Height_SetWithParentWithHandle_TestData()
    {
        yield return new object[] { true, -4, 0, 0, 0, 1, 2 };
        yield return new object[] { true, 0, 0, 0, 0, 0, 0 };
        yield return new object[] { true, 2, 2, 1, 1, 2, 2 };
        yield return new object[] { true, 40, 40, 1, 1, 2, 2 };
        yield return new object[] { false, -4, 0, 0, 0, 1, 2 };
        yield return new object[] { false, 0, 0, 0, 0, 0, 0 };
        yield return new object[] { false, 2, 2, 1, 0, 2, 2 };
        yield return new object[] { false, 40, 40, 1, 0, 2, 2 };
    }

    [WinFormsTheory]
    [MemberData(nameof(Height_SetWithParentWithHandle_TestData))]
    public void Control_Height_SetWithParentWithHandle_GetReturnsExpected(bool resizeRedraw, int value, int expectedHeight, int expectedLayoutCallCount, int expectedInvalidatedCallCount, int expectedParentLayoutCallCount1, int expectedParentLayoutCallCount2)
    {
        using Control parent = new();
        using SubControl control = new()
        {
            Parent = parent
        };
        control.SetStyle(ControlStyles.ResizeRedraw, resizeRedraw);
        int layoutCallCount = 0;
        int resizeCallCount = 0;
        int sizeChangedCallCount = 0;
        int clientSizeChangedCallCount = 0;
        int parentLayoutCallCount = 0;
        control.Layout += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("Bounds", e.AffectedProperty);
            Assert.Equal(resizeCallCount, layoutCallCount);
            Assert.Equal(sizeChangedCallCount, layoutCallCount);
            Assert.Equal(clientSizeChangedCallCount, layoutCallCount);
            Assert.Equal(parentLayoutCallCount, layoutCallCount);
            layoutCallCount++;
        };
        control.Resize += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(layoutCallCount - 1, resizeCallCount);
            Assert.Equal(sizeChangedCallCount, resizeCallCount);
            Assert.Equal(clientSizeChangedCallCount, resizeCallCount);
            Assert.Equal(parentLayoutCallCount, resizeCallCount);
            resizeCallCount++;
        };
        control.SizeChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(resizeCallCount - 1, sizeChangedCallCount);
            Assert.Equal(layoutCallCount - 1, sizeChangedCallCount);
            Assert.Equal(clientSizeChangedCallCount, sizeChangedCallCount);
            Assert.Equal(parentLayoutCallCount, sizeChangedCallCount);
            sizeChangedCallCount++;
        };
        control.ClientSizeChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(resizeCallCount - 1, clientSizeChangedCallCount);
            Assert.Equal(layoutCallCount - 1, clientSizeChangedCallCount);
            Assert.Equal(sizeChangedCallCount - 1, clientSizeChangedCallCount);
            Assert.Equal(parentLayoutCallCount, clientSizeChangedCallCount);
            clientSizeChangedCallCount++;
        };
        void parentHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(parent, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("Bounds", e.AffectedProperty);
            parentLayoutCallCount++;
        }

        parent.Layout += parentHandler;
        Assert.NotEqual(IntPtr.Zero, parent.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
        int parentInvalidatedCallCount = 0;
        parent.Invalidated += (sender, e) => parentInvalidatedCallCount++;
        int parentStyleChangedCallCount = 0;
        parent.StyleChanged += (sender, e) => parentStyleChangedCallCount++;
        int parentCreatedCallCount = 0;
        parent.HandleCreated += (sender, e) => parentCreatedCallCount++;

        try
        {
            control.Height = value;
            Assert.Equal(new Size(0, expectedHeight), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, 0, expectedHeight), control.ClientRectangle);
            Assert.Equal(new Rectangle(0, 0, 0, expectedHeight), control.DisplayRectangle);
            Assert.Equal(new Size(0, expectedHeight), control.Size);
            Assert.Equal(0, control.Left);
            Assert.Equal(0, control.Right);
            Assert.Equal(0, control.Top);
            Assert.Equal(expectedHeight, control.Bottom);
            Assert.Equal(0, control.Width);
            Assert.Equal(expectedHeight, control.Height);
            Assert.Equal(new Rectangle(0, 0, 0, expectedHeight), control.Bounds);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.Equal(expectedParentLayoutCallCount1, parentLayoutCallCount);
            Assert.Equal(expectedLayoutCallCount, resizeCallCount);
            Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
            Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.True(parent.IsHandleCreated);
            Assert.Equal(0, parentInvalidatedCallCount);
            Assert.Equal(0, parentStyleChangedCallCount);
            Assert.Equal(0, parentCreatedCallCount);

            // Call again.
            control.Height = value;
            Assert.Equal(new Size(0, expectedHeight), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, 0, expectedHeight), control.ClientRectangle);
            Assert.Equal(new Rectangle(0, 0, 0, expectedHeight), control.DisplayRectangle);
            Assert.Equal(new Size(0, expectedHeight), control.Size);
            Assert.Equal(0, control.Left);
            Assert.Equal(0, control.Right);
            Assert.Equal(0, control.Top);
            Assert.Equal(expectedHeight, control.Bottom);
            Assert.Equal(0, control.Width);
            Assert.Equal(expectedHeight, control.Height);
            Assert.Equal(new Rectangle(0, 0, 0, expectedHeight), control.Bounds);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.Equal(expectedParentLayoutCallCount2, parentLayoutCallCount);
            Assert.Equal(expectedLayoutCallCount, resizeCallCount);
            Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
            Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.True(parent.IsHandleCreated);
            Assert.Equal(0, parentInvalidatedCallCount);
            Assert.Equal(0, parentStyleChangedCallCount);
            Assert.Equal(0, parentCreatedCallCount);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    public static IEnumerable<object[]> ImeMode_Set_TestData()
    {
        yield return new object[] { ImeMode.Inherit, ImeMode.NoControl };
        yield return new object[] { ImeMode.NoControl, ImeMode.NoControl };
        yield return new object[] { ImeMode.On, ImeMode.On };
        yield return new object[] { ImeMode.Off, ImeMode.Off };
        yield return new object[] { ImeMode.Disable, ImeMode.Disable };
        yield return new object[] { ImeMode.Hiragana, ImeMode.Hiragana };
        yield return new object[] { ImeMode.Katakana, ImeMode.Katakana };
        yield return new object[] { ImeMode.KatakanaHalf, ImeMode.KatakanaHalf };
        yield return new object[] { ImeMode.AlphaFull, ImeMode.AlphaFull };
        yield return new object[] { ImeMode.Alpha, ImeMode.Alpha };
        yield return new object[] { ImeMode.HangulFull, ImeMode.HangulFull };
        yield return new object[] { ImeMode.Hangul, ImeMode.Hangul };
        yield return new object[] { ImeMode.Close, ImeMode.Close };
        yield return new object[] { ImeMode.OnHalf, ImeMode.On };
    }

    [WinFormsTheory]
    [MemberData(nameof(ImeMode_Set_TestData))]
    public void Control_ImeMode_Set_GetReturnsExpected(ImeMode value, ImeMode expected)
    {
        using Control control = new()
        {
            ImeMode = value
        };
        Assert.Equal(expected, control.ImeMode);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.ImeMode = value;
        Assert.Equal(expected, control.ImeMode);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(ImeMode_Set_TestData))]
    public void Control_ImeMode_SetWithHandle_GetReturnsExpected(ImeMode value, ImeMode expected)
    {
        using Control control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.ImeMode = value;
        Assert.Equal(expected, control.ImeMode);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.ImeMode = value;
        Assert.Equal(expected, control.ImeMode);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void Control_ImeMode_SetWithHandler_CallsImeModeChanged()
    {
        using Control control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        control.ImeModeChanged += handler;

        // Set different.
        control.ImeMode = ImeMode.On;
        Assert.Equal(ImeMode.On, control.ImeMode);
        Assert.Equal(1, callCount);

        // Set same.
        control.ImeMode = ImeMode.On;
        Assert.Equal(ImeMode.On, control.ImeMode);
        Assert.Equal(1, callCount);

        // Set different.
        control.ImeMode = ImeMode.Off;
        Assert.Equal(ImeMode.Off, control.ImeMode);
        Assert.Equal(2, callCount);

        // Remove handler.
        control.ImeModeChanged -= handler;
        control.ImeMode = ImeMode.Off;
        Assert.Equal(ImeMode.Off, control.ImeMode);
        Assert.Equal(2, callCount);
    }

    [WinFormsTheory]
    [InvalidEnumData<ImeMode>]
    public void Control_ImeMode_SetInvalid_ThrowsInvalidEnumArgumentException(ImeMode value)
    {
        using Control control = new();
        Assert.Throws<InvalidEnumArgumentException>("value", () => control.ImeMode = value);
    }

    public static IEnumerable<object[]> ImeModeBase_Set_TestData()
    {
        yield return new object[] { ImeMode.Inherit, ImeMode.NoControl };
        yield return new object[] { ImeMode.NoControl, ImeMode.NoControl };
        yield return new object[] { ImeMode.On, ImeMode.On };
        yield return new object[] { ImeMode.Off, ImeMode.Off };
        yield return new object[] { ImeMode.Disable, ImeMode.Disable };
        yield return new object[] { ImeMode.Hiragana, ImeMode.Hiragana };
        yield return new object[] { ImeMode.Katakana, ImeMode.Katakana };
        yield return new object[] { ImeMode.KatakanaHalf, ImeMode.KatakanaHalf };
        yield return new object[] { ImeMode.AlphaFull, ImeMode.AlphaFull };
        yield return new object[] { ImeMode.Alpha, ImeMode.Alpha };
        yield return new object[] { ImeMode.HangulFull, ImeMode.HangulFull };
        yield return new object[] { ImeMode.Hangul, ImeMode.Hangul };
        yield return new object[] { ImeMode.Close, ImeMode.Close };
        yield return new object[] { ImeMode.OnHalf, ImeMode.OnHalf };
    }

    [WinFormsTheory]
    [MemberData(nameof(ImeModeBase_Set_TestData))]
    public void Control_ImeModeBase_Set_GetReturnsExpected(ImeMode value, ImeMode expected)
    {
        using SubControl control = new()
        {
            ImeModeBase = value
        };
        Assert.Equal(expected, control.ImeModeBase);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.ImeModeBase = value;
        Assert.Equal(expected, control.ImeModeBase);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(ImeModeBase_Set_TestData))]
    public void Control_ImeModeBase_SetWithHandle_GetReturnsExpected(ImeMode value, ImeMode expected)
    {
        using SubControl control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.ImeModeBase = value;
        Assert.Equal(expected, control.ImeModeBase);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.ImeModeBase = value;
        Assert.Equal(expected, control.ImeModeBase);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void Control_ImeModeBase_SetWithHandler_CallsImeModeChanged()
    {
        using SubControl control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        control.ImeModeChanged += handler;

        // Set different.
        control.ImeModeBase = ImeMode.On;
        Assert.Equal(ImeMode.On, control.ImeModeBase);
        Assert.Equal(1, callCount);

        // Set same.
        control.ImeModeBase = ImeMode.On;
        Assert.Equal(ImeMode.On, control.ImeModeBase);
        Assert.Equal(1, callCount);

        // Set different.
        control.ImeModeBase = ImeMode.Off;
        Assert.Equal(ImeMode.Off, control.ImeModeBase);
        Assert.Equal(2, callCount);

        // Remove handler.
        control.ImeModeChanged -= handler;
        control.ImeModeBase = ImeMode.Off;
        Assert.Equal(ImeMode.Off, control.ImeModeBase);
        Assert.Equal(2, callCount);
    }

    [WinFormsTheory]
    [InvalidEnumData<ImeMode>]
    public void Control_ImeModeBase_SetInvalid_ThrowsInvalidEnumArgumentException(ImeMode value)
    {
        using SubControl control = new();
        Assert.Throws<InvalidEnumArgumentException>("value", () => control.ImeModeBase = value);
    }

    [WinFormsFact]
    public async Task Control_InvokeRequired_Get_ReturnsExpected()
    {
        using Control control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        Assert.False(control.InvokeRequired);

        await Task.Run(() =>
        {
            Assert.True(control.InvokeRequired);
        });
    }

    [WinFormsFact]
    public async Task Control_InvokeRequired_GetWithHandle_ReturnsExpected()
    {
        using Control control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        Assert.False(control.InvokeRequired);

        await Task.Run(() =>
        {
            Assert.True(control.InvokeRequired);
        });
    }

    [WinFormsTheory]
    [BoolData]
    public void Control_IsAccessible_Set_GetReturnsExpected(bool value)
    {
        using Control control = new()
        {
            IsAccessible = value
        };
        Assert.Equal(value, control.IsAccessible);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.IsAccessible = value;
        Assert.Equal(value, control.IsAccessible);
        Assert.False(control.IsHandleCreated);

        // Set different.
        control.IsAccessible = !value;
        Assert.Equal(!value, control.IsAccessible);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void Control_IsMirrored_Get_ReturnsExpected()
    {
        using Control control = new();
        Assert.False(control.IsMirrored);

        // Call again to test caching behavior.
        Assert.False(control.IsMirrored);
    }

    [WinFormsFact]
    public void Control_IsMirrored_GetLayoutRtl_ReturnsExpected()
    {
        using RightToLeftControl control = new();
        Assert.True(control.IsMirrored);

        // Call again to test caching behavior.
        Assert.True(control.IsMirrored);
    }

    [WinFormsFact]
    public void Control_IsMirrored_GetLayoutRtlWithHandle_ReturnsExpected()
    {
        using RightToLeftControl control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        Assert.True(control.IsMirrored);

        // Call again to test caching behavior.
        Assert.True(control.IsMirrored);
    }

    [WinFormsFact]
    public void Control_IsMirrored_GetWithHandle_ReturnsExpected()
    {
        using Control control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        Assert.False(control.IsMirrored);

        // Call again to test caching behavior.
        Assert.False(control.IsMirrored);
    }

    private class RightToLeftControl : Control
    {
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= (int)WINDOW_EX_STYLE.WS_EX_LAYOUTRTL;
                return cp;
            }
        }
    }

    [WinFormsTheory]
    [InlineData(0, 0)]
    [InlineData(-1, 1)]
    [InlineData(1, 1)]
    public void Control_Left_Set_GetReturnsExpected(int value, int expectedLocationChangedCallCount)
    {
        using SubControl control = new();
        int moveCallCount = 0;
        int locationChangedCallCount = 0;
        int layoutCallCount = 0;
        int resizeCallCount = 0;
        int sizeChangedCallCount = 0;
        int clientSizeChangedCallCount = 0;
        control.Move += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(locationChangedCallCount, moveCallCount);
            moveCallCount++;
        };
        control.LocationChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(moveCallCount - 1, locationChangedCallCount);
            locationChangedCallCount++;
        };
        control.Layout += (sender, e) => layoutCallCount++;
        control.Resize += (sender, e) => resizeCallCount++;
        control.SizeChanged += (sender, e) => sizeChangedCallCount++;
        control.ClientSizeChanged += (sender, e) => clientSizeChangedCallCount++;

        control.Left = value;
        Assert.Equal(new Size(0, 0), control.ClientSize);
        Assert.Equal(new Rectangle(0, 0, 0, 0), control.ClientRectangle);
        Assert.Equal(new Rectangle(0, 0, 0, 0), control.DisplayRectangle);
        Assert.Equal(new Size(0, 0), control.Size);
        Assert.Equal(value, control.Left);
        Assert.Equal(value, control.Right);
        Assert.Equal(new Point(value, 0), control.Location);
        Assert.Equal(0, control.Top);
        Assert.Equal(0, control.Bottom);
        Assert.Equal(0, control.Width);
        Assert.Equal(0, control.Height);
        Assert.Equal(new Rectangle(value, 0, 0, 0), control.Bounds);
        Assert.Equal(expectedLocationChangedCallCount, moveCallCount);
        Assert.Equal(expectedLocationChangedCallCount, locationChangedCallCount);
        Assert.Equal(0, layoutCallCount);
        Assert.Equal(0, resizeCallCount);
        Assert.Equal(0, sizeChangedCallCount);
        Assert.Equal(0, clientSizeChangedCallCount);
        Assert.False(control.IsHandleCreated);

        // Call again.
        control.Left = value;
        Assert.Equal(new Size(0, 0), control.ClientSize);
        Assert.Equal(new Rectangle(0, 0, 0, 0), control.ClientRectangle);
        Assert.Equal(new Rectangle(0, 0, 0, 0), control.DisplayRectangle);
        Assert.Equal(new Size(0, 0), control.Size);
        Assert.Equal(value, control.Left);
        Assert.Equal(value, control.Right);
        Assert.Equal(new Point(value, 0), control.Location);
        Assert.Equal(0, control.Top);
        Assert.Equal(0, control.Bottom);
        Assert.Equal(0, control.Width);
        Assert.Equal(0, control.Height);
        Assert.Equal(new Rectangle(value, 0, 0, 0), control.Bounds);
        Assert.Equal(expectedLocationChangedCallCount, moveCallCount);
        Assert.Equal(expectedLocationChangedCallCount, locationChangedCallCount);
        Assert.Equal(0, layoutCallCount);
        Assert.Equal(0, resizeCallCount);
        Assert.Equal(0, sizeChangedCallCount);
        Assert.Equal(0, clientSizeChangedCallCount);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(0, 0)]
    [InlineData(-1, 1)]
    [InlineData(1, 1)]
    public void Control_Left_SetWithParent_GetReturnsExpected(int value, int expectedLocationChangedCallCount)
    {
        using Control parent = new();
        using SubControl control = new()
        {
            Parent = parent
        };
        int moveCallCount = 0;
        int locationChangedCallCount = 0;
        int layoutCallCount = 0;
        int resizeCallCount = 0;
        int sizeChangedCallCount = 0;
        int clientSizeChangedCallCount = 0;
        int parentLayoutCallCount = 0;
        control.Move += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(locationChangedCallCount, moveCallCount);
            moveCallCount++;
        };
        control.LocationChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(moveCallCount - 1, locationChangedCallCount);
            locationChangedCallCount++;
        };
        control.Layout += (sender, e) => layoutCallCount++;
        control.Resize += (sender, e) => resizeCallCount++;
        control.SizeChanged += (sender, e) => sizeChangedCallCount++;
        control.ClientSizeChanged += (sender, e) => clientSizeChangedCallCount++;
        void parentHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(parent, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("Bounds", e.AffectedProperty);
            parentLayoutCallCount++;
        }

        parent.Layout += parentHandler;

        try
        {
            control.Left = value;
            Assert.Equal(new Size(0, 0), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, 0, 0), control.ClientRectangle);
            Assert.Equal(new Rectangle(0, 0, 0, 0), control.DisplayRectangle);
            Assert.Equal(new Size(0, 0), control.Size);
            Assert.Equal(value, control.Left);
            Assert.Equal(new Point(value, 0), control.Location);
            Assert.Equal(0, control.Top);
            Assert.Equal(0, control.Bottom);
            Assert.Equal(0, control.Width);
            Assert.Equal(0, control.Height);
            Assert.Equal(new Rectangle(value, 0, 0, 0), control.Bounds);
            Assert.Equal(expectedLocationChangedCallCount, moveCallCount);
            Assert.Equal(expectedLocationChangedCallCount, locationChangedCallCount);
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(0, resizeCallCount);
            Assert.Equal(0, sizeChangedCallCount);
            Assert.Equal(0, clientSizeChangedCallCount);
            Assert.Equal(expectedLocationChangedCallCount, parentLayoutCallCount);
            Assert.False(control.IsHandleCreated);

            // Call again.
            control.Left = value;
            Assert.Equal(new Size(0, 0), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, 0, 0), control.ClientRectangle);
            Assert.Equal(new Rectangle(0, 0, 0, 0), control.DisplayRectangle);
            Assert.Equal(new Size(0, 0), control.Size);
            Assert.Equal(value, control.Left);
            Assert.Equal(new Point(value, 0), control.Location);
            Assert.Equal(0, control.Top);
            Assert.Equal(0, control.Bottom);
            Assert.Equal(0, control.Width);
            Assert.Equal(0, control.Height);
            Assert.Equal(new Rectangle(value, 0, 0, 0), control.Bounds);
            Assert.Equal(expectedLocationChangedCallCount, moveCallCount);
            Assert.Equal(expectedLocationChangedCallCount, locationChangedCallCount);
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(0, resizeCallCount);
            Assert.Equal(0, sizeChangedCallCount);
            Assert.Equal(0, clientSizeChangedCallCount);
            Assert.Equal(expectedLocationChangedCallCount, parentLayoutCallCount);
            Assert.False(control.IsHandleCreated);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    public static IEnumerable<object[]> Left_SetWithHandle_TestData()
    {
        foreach (bool resizeRedraw in new bool[] { true, false })
        {
            yield return new object[] { resizeRedraw, 0, 0 };
            yield return new object[] { resizeRedraw, -1, 1 };
            yield return new object[] { resizeRedraw, 1, 1 };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(Left_SetWithHandle_TestData))]
    public void Control_Left_SetWithHandle_GetReturnsExpected(bool resizeRedraw, int value, int expectedLocationChangedCallCount)
    {
        using SubControl control = new();
        control.SetStyle(ControlStyles.ResizeRedraw, resizeRedraw);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        int moveCallCount = 0;
        int locationChangedCallCount = 0;
        int layoutCallCount = 0;
        int resizeCallCount = 0;
        int sizeChangedCallCount = 0;
        int clientSizeChangedCallCount = 0;
        control.Move += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(locationChangedCallCount, moveCallCount);
            moveCallCount++;
        };
        control.LocationChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(moveCallCount - 1, locationChangedCallCount);
            locationChangedCallCount++;
        };
        control.Layout += (sender, e) => layoutCallCount++;
        control.Resize += (sender, e) => resizeCallCount++;
        control.SizeChanged += (sender, e) => sizeChangedCallCount++;
        control.ClientSizeChanged += (sender, e) => clientSizeChangedCallCount++;

        control.Left = value;
        Assert.Equal(new Size(0, 0), control.ClientSize);
        Assert.Equal(new Rectangle(0, 0, 0, 0), control.ClientRectangle);
        Assert.Equal(new Rectangle(0, 0, 0, 0), control.DisplayRectangle);
        Assert.Equal(new Size(0, 0), control.Size);
        Assert.Equal(value, control.Left);
        Assert.Equal(value, control.Right);
        Assert.Equal(new Point(value, 0), control.Location);
        Assert.Equal(0, control.Top);
        Assert.Equal(0, control.Bottom);
        Assert.Equal(0, control.Width);
        Assert.Equal(0, control.Height);
        Assert.Equal(new Rectangle(value, 0, 0, 0), control.Bounds);
        Assert.Equal(expectedLocationChangedCallCount, moveCallCount);
        Assert.Equal(expectedLocationChangedCallCount, locationChangedCallCount);
        Assert.Equal(0, layoutCallCount);
        Assert.Equal(0, resizeCallCount);
        Assert.Equal(0, sizeChangedCallCount);
        Assert.Equal(0, clientSizeChangedCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Call again.
        control.Left = value;
        Assert.Equal(new Size(0, 0), control.ClientSize);
        Assert.Equal(new Rectangle(0, 0, 0, 0), control.ClientRectangle);
        Assert.Equal(new Rectangle(0, 0, 0, 0), control.DisplayRectangle);
        Assert.Equal(new Size(0, 0), control.Size);
        Assert.Equal(value, control.Left);
        Assert.Equal(value, control.Right);
        Assert.Equal(new Point(value, 0), control.Location);
        Assert.Equal(0, control.Top);
        Assert.Equal(0, control.Bottom);
        Assert.Equal(0, control.Width);
        Assert.Equal(0, control.Height);
        Assert.Equal(new Rectangle(value, 0, 0, 0), control.Bounds);
        Assert.Equal(expectedLocationChangedCallCount, moveCallCount);
        Assert.Equal(expectedLocationChangedCallCount, locationChangedCallCount);
        Assert.Equal(0, layoutCallCount);
        Assert.Equal(0, resizeCallCount);
        Assert.Equal(0, sizeChangedCallCount);
        Assert.Equal(0, clientSizeChangedCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [MemberData(nameof(Left_SetWithHandle_TestData))]
    public void Control_Left_SetWithParentWithHandle_GetReturnsExpected(bool resizeRedraw, int value, int expectedLocationChangedCallCount)
    {
        using Control parent = new();
        using SubControl control = new()
        {
            Parent = parent
        };
        control.SetStyle(ControlStyles.ResizeRedraw, resizeRedraw);
        Assert.NotEqual(IntPtr.Zero, parent.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
        int parentInvalidatedCallCount = 0;
        parent.Invalidated += (sender, e) => parentInvalidatedCallCount++;
        int parentStyleChangedCallCount = 0;
        parent.StyleChanged += (sender, e) => parentStyleChangedCallCount++;
        int parentCreatedCallCount = 0;
        parent.HandleCreated += (sender, e) => parentCreatedCallCount++;

        int moveCallCount = 0;
        int locationChangedCallCount = 0;
        int layoutCallCount = 0;
        int resizeCallCount = 0;
        int sizeChangedCallCount = 0;
        int clientSizeChangedCallCount = 0;
        int parentLayoutCallCount = 0;
        control.Move += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(locationChangedCallCount, moveCallCount);
            moveCallCount++;
        };
        control.LocationChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(moveCallCount - 1, locationChangedCallCount);
            locationChangedCallCount++;
        };
        control.Layout += (sender, e) => layoutCallCount++;
        control.Resize += (sender, e) => resizeCallCount++;
        control.SizeChanged += (sender, e) => sizeChangedCallCount++;
        control.ClientSizeChanged += (sender, e) => clientSizeChangedCallCount++;
        void parentHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(parent, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("Bounds", e.AffectedProperty);
            parentLayoutCallCount++;
        }

        parent.Layout += parentHandler;

        try
        {
            control.Left = value;
            Assert.Equal(new Size(0, 0), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, 0, 0), control.ClientRectangle);
            Assert.Equal(new Rectangle(0, 0, 0, 0), control.DisplayRectangle);
            Assert.Equal(new Size(0, 0), control.Size);
            Assert.Equal(value, control.Left);
            Assert.Equal(new Point(value, 0), control.Location);
            Assert.Equal(0, control.Top);
            Assert.Equal(0, control.Bottom);
            Assert.Equal(0, control.Width);
            Assert.Equal(0, control.Height);
            Assert.Equal(new Rectangle(value, 0, 0, 0), control.Bounds);
            Assert.Equal(expectedLocationChangedCallCount, moveCallCount);
            Assert.Equal(expectedLocationChangedCallCount, locationChangedCallCount);
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(expectedLocationChangedCallCount, parentLayoutCallCount);
            Assert.Equal(0, resizeCallCount);
            Assert.Equal(0, sizeChangedCallCount);
            Assert.Equal(0, clientSizeChangedCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.True(parent.IsHandleCreated);
            Assert.Equal(0, parentInvalidatedCallCount);
            Assert.Equal(0, parentStyleChangedCallCount);
            Assert.Equal(0, parentCreatedCallCount);

            // Call again.
            control.Left = value;
            Assert.Equal(new Size(0, 0), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, 0, 0), control.ClientRectangle);
            Assert.Equal(new Rectangle(0, 0, 0, 0), control.DisplayRectangle);
            Assert.Equal(new Size(0, 0), control.Size);
            Assert.Equal(value, control.Left);
            Assert.Equal(new Point(value, 0), control.Location);
            Assert.Equal(0, control.Top);
            Assert.Equal(0, control.Bottom);
            Assert.Equal(0, control.Width);
            Assert.Equal(0, control.Height);
            Assert.Equal(new Rectangle(value, 0, 0, 0), control.Bounds);
            Assert.Equal(expectedLocationChangedCallCount, moveCallCount);
            Assert.Equal(expectedLocationChangedCallCount, locationChangedCallCount);
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(expectedLocationChangedCallCount, parentLayoutCallCount);
            Assert.Equal(0, resizeCallCount);
            Assert.Equal(0, sizeChangedCallCount);
            Assert.Equal(0, clientSizeChangedCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.True(parent.IsHandleCreated);
            Assert.Equal(0, parentInvalidatedCallCount);
            Assert.Equal(0, parentStyleChangedCallCount);
            Assert.Equal(0, parentCreatedCallCount);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    [WinFormsTheory]
    [InlineData(true, 1)]
    [InlineData(false, 0)]
    public void Control_Left_SetWithHandleWithTransparentBackColor_DoesNotCallInvalidate(bool supportsTransparentBackgroundColor, int expectedInvalidatedCallCount)
    {
        using SubControl control = new();
        control.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
        control.BackColor = Color.FromArgb(254, 255, 255, 255);
        control.SetStyle(ControlStyles.SupportsTransparentBackColor, supportsTransparentBackgroundColor);

        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;

        // Set different.
        control.Left = 1;
        Assert.Equal(new Point(1, 0), control.Location);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);

        // Set same.
        control.Left = 1;
        Assert.Equal(new Point(1, 0), control.Location);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);

        // Set different.
        control.Left = 2;
        Assert.Equal(new Point(2, 0), control.Location);
        Assert.Equal(expectedInvalidatedCallCount * 2, invalidatedCallCount);
    }

    [WinFormsFact]
    public void Control_Left_SetWithHandler_CallsLocationChanged()
    {
        using Control control = new();
        int locationChangedCallCount = 0;
        EventHandler locationChangedHandler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            locationChangedCallCount++;
        };
        control.LocationChanged += locationChangedHandler;
        int moveCallCount = 0;
        EventHandler moveHandler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            moveCallCount++;
        };
        control.Move += moveHandler;

        // Set different.
        control.Left = 1;
        Assert.Equal(new Point(1, 0), control.Location);
        Assert.Equal(1, locationChangedCallCount);
        Assert.Equal(1, moveCallCount);

        // Set same.
        control.Left = 1;
        Assert.Equal(new Point(1, 0), control.Location);
        Assert.Equal(1, locationChangedCallCount);
        Assert.Equal(1, moveCallCount);

        // Set different.
        control.Left = 2;
        Assert.Equal(new Point(2, 0), control.Location);
        Assert.Equal(2, locationChangedCallCount);
        Assert.Equal(2, moveCallCount);

        // Remove handler.
        control.LocationChanged -= locationChangedHandler;
        control.Move -= moveHandler;
        control.Left = 1;
        Assert.Equal(new Point(1, 0), control.Location);
        Assert.Equal(2, locationChangedCallCount);
        Assert.Equal(2, moveCallCount);
    }

    public static IEnumerable<object[]> Location_Set_TestData()
    {
        yield return new object[] { new Point(0, 0), 0 };
        yield return new object[] { new Point(-1, -2), 1 };
        yield return new object[] { new Point(1, 0), 1 };
        yield return new object[] { new Point(0, 2), 1 };
        yield return new object[] { new Point(1, 2), 1 };
    }

    [WinFormsTheory]
    [MemberData(nameof(Location_Set_TestData))]
    public void Control_Location_Set_GetReturnsExpected(Point value, int expectedLocationChangedCallCount)
    {
        using Control control = new();
        int moveCallCount = 0;
        int locationChangedCallCount = 0;
        int layoutCallCount = 0;
        int resizeCallCount = 0;
        int sizeChangedCallCount = 0;
        int clientSizeChangedCallCount = 0;
        control.Move += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(locationChangedCallCount, moveCallCount);
            moveCallCount++;
        };
        control.LocationChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(moveCallCount - 1, locationChangedCallCount);
            locationChangedCallCount++;
        };
        control.Layout += (sender, e) => layoutCallCount++;
        control.Resize += (sender, e) => resizeCallCount++;
        control.SizeChanged += (sender, e) => sizeChangedCallCount++;
        control.ClientSizeChanged += (sender, e) => clientSizeChangedCallCount++;

        control.Location = value;
        Assert.Equal(new Size(0, 0), control.ClientSize);
        Assert.Equal(new Rectangle(0, 0, 0, 0), control.ClientRectangle);
        Assert.Equal(new Rectangle(0, 0, 0, 0), control.DisplayRectangle);
        Assert.Equal(new Size(0, 0), control.Size);
        Assert.Equal(value.X, control.Left);
        Assert.Equal(value.X, control.Right);
        Assert.Equal(value, control.Location);
        Assert.Equal(value.Y, control.Top);
        Assert.Equal(value.Y, control.Bottom);
        Assert.Equal(0, control.Width);
        Assert.Equal(0, control.Height);
        Assert.Equal(new Rectangle(value.X, value.Y, 0, 0), control.Bounds);
        Assert.Equal(expectedLocationChangedCallCount, moveCallCount);
        Assert.Equal(expectedLocationChangedCallCount, locationChangedCallCount);
        Assert.Equal(0, layoutCallCount);
        Assert.Equal(0, resizeCallCount);
        Assert.Equal(0, sizeChangedCallCount);
        Assert.Equal(0, clientSizeChangedCallCount);
        Assert.False(control.IsHandleCreated);

        // Call again.
        control.Location = value;
        Assert.Equal(new Size(0, 0), control.ClientSize);
        Assert.Equal(new Rectangle(0, 0, 0, 0), control.ClientRectangle);
        Assert.Equal(new Rectangle(0, 0, 0, 0), control.DisplayRectangle);
        Assert.Equal(new Size(0, 0), control.Size);
        Assert.Equal(value.X, control.Left);
        Assert.Equal(value.X, control.Right);
        Assert.Equal(value, control.Location);
        Assert.Equal(value.Y, control.Top);
        Assert.Equal(value.Y, control.Bottom);
        Assert.Equal(0, control.Width);
        Assert.Equal(0, control.Height);
        Assert.Equal(new Rectangle(value.X, value.Y, 0, 0), control.Bounds);
        Assert.Equal(expectedLocationChangedCallCount, moveCallCount);
        Assert.Equal(expectedLocationChangedCallCount, locationChangedCallCount);
        Assert.Equal(0, layoutCallCount);
        Assert.Equal(0, resizeCallCount);
        Assert.Equal(0, sizeChangedCallCount);
        Assert.Equal(0, clientSizeChangedCallCount);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(Location_Set_TestData))]
    public void Control_Location_SetWithParent_GetReturnsExpected(Point value, int expectedLocationChangedCallCount)
    {
        using Control parent = new();
        using SubControl control = new()
        {
            Parent = parent
        };
        int layoutCallCount = 0;
        int moveCallCount = 0;
        int locationChangedCallCount = 0;
        int resizeCallCount = 0;
        int sizeChangedCallCount = 0;
        int clientSizeChangedCallCount = 0;
        int parentLayoutCallCount = 0;
        control.Move += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(locationChangedCallCount, moveCallCount);
            moveCallCount++;
        };
        control.LocationChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(moveCallCount - 1, locationChangedCallCount);
            locationChangedCallCount++;
        };
        control.Layout += (sender, e) => layoutCallCount++;
        control.Resize += (sender, e) => resizeCallCount++;
        control.SizeChanged += (sender, e) => sizeChangedCallCount++;
        control.ClientSizeChanged += (sender, e) => clientSizeChangedCallCount++;
        void parentHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(parent, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("Bounds", e.AffectedProperty);
            parentLayoutCallCount++;
        }

        parent.Layout += parentHandler;

        try
        {
            control.Location = value;
            Assert.Equal(new Size(0, 0), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, 0, 0), control.ClientRectangle);
            Assert.Equal(new Rectangle(0, 0, 0, 0), control.DisplayRectangle);
            Assert.Equal(new Size(0, 0), control.Size);
            Assert.Equal(value.X, control.Left);
            Assert.Equal(value.X, control.Right);
            Assert.Equal(value, control.Location);
            Assert.Equal(value.Y, control.Top);
            Assert.Equal(value.Y, control.Bottom);
            Assert.Equal(0, control.Width);
            Assert.Equal(0, control.Height);
            Assert.Equal(new Rectangle(value.X, value.Y, 0, 0), control.Bounds);
            Assert.Equal(expectedLocationChangedCallCount, moveCallCount);
            Assert.Equal(expectedLocationChangedCallCount, locationChangedCallCount);
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(0, resizeCallCount);
            Assert.Equal(0, sizeChangedCallCount);
            Assert.Equal(0, clientSizeChangedCallCount);
            Assert.Equal(expectedLocationChangedCallCount, parentLayoutCallCount);
            Assert.False(control.IsHandleCreated);

            // Call again.
            control.Location = value;
            Assert.Equal(new Size(0, 0), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, 0, 0), control.ClientRectangle);
            Assert.Equal(new Rectangle(0, 0, 0, 0), control.DisplayRectangle);
            Assert.Equal(new Size(0, 0), control.Size);
            Assert.Equal(value.X, control.Left);
            Assert.Equal(value.X, control.Right);
            Assert.Equal(value, control.Location);
            Assert.Equal(value.Y, control.Top);
            Assert.Equal(value.Y, control.Bottom);
            Assert.Equal(0, control.Width);
            Assert.Equal(0, control.Height);
            Assert.Equal(new Rectangle(value.X, value.Y, 0, 0), control.Bounds);
            Assert.Equal(expectedLocationChangedCallCount, moveCallCount);
            Assert.Equal(expectedLocationChangedCallCount, locationChangedCallCount);
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(0, resizeCallCount);
            Assert.Equal(0, sizeChangedCallCount);
            Assert.Equal(0, clientSizeChangedCallCount);
            Assert.Equal(expectedLocationChangedCallCount, parentLayoutCallCount);
            Assert.False(control.IsHandleCreated);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    public static IEnumerable<object[]> Location_SetWithHandle_TestData()
    {
        foreach (bool resizeRedraw in new bool[] { true, false })
        {
            yield return new object[] { resizeRedraw, new Point(0, 0), 0 };
            yield return new object[] { resizeRedraw, new Point(-1, -2), 1 };
            yield return new object[] { resizeRedraw, new Point(1, 0), 1 };
            yield return new object[] { resizeRedraw, new Point(0, 2), 1 };
            yield return new object[] { resizeRedraw, new Point(1, 2), 1 };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(Location_SetWithHandle_TestData))]
    public void Control_Location_SetWithHandle_GetReturnsExpected(bool resizeRedraw, Point value, int expectedLocationChangedCallCount)
    {
        using SubControl control = new();
        control.SetStyle(ControlStyles.ResizeRedraw, resizeRedraw);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        int moveCallCount = 0;
        int locationChangedCallCount = 0;
        int layoutCallCount = 0;
        int resizeCallCount = 0;
        int sizeChangedCallCount = 0;
        int clientSizeChangedCallCount = 0;
        control.Move += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(locationChangedCallCount, moveCallCount);
            moveCallCount++;
        };
        control.LocationChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(moveCallCount - 1, locationChangedCallCount);
            locationChangedCallCount++;
        };
        control.Layout += (sender, e) => layoutCallCount++;
        control.Resize += (sender, e) => resizeCallCount++;
        control.SizeChanged += (sender, e) => sizeChangedCallCount++;
        control.ClientSizeChanged += (sender, e) => clientSizeChangedCallCount++;

        control.Location = value;
        Assert.Equal(new Size(0, 0), control.ClientSize);
        Assert.Equal(new Rectangle(0, 0, 0, 0), control.ClientRectangle);
        Assert.Equal(new Rectangle(0, 0, 0, 0), control.DisplayRectangle);
        Assert.Equal(new Size(0, 0), control.Size);
        Assert.Equal(value.X, control.Left);
        Assert.Equal(value.X, control.Right);
        Assert.Equal(value, control.Location);
        Assert.Equal(value.Y, control.Top);
        Assert.Equal(value.Y, control.Bottom);
        Assert.Equal(0, control.Width);
        Assert.Equal(0, control.Height);
        Assert.Equal(new Rectangle(value.X, value.Y, 0, 0), control.Bounds);
        Assert.Equal(expectedLocationChangedCallCount, moveCallCount);
        Assert.Equal(expectedLocationChangedCallCount, locationChangedCallCount);
        Assert.Equal(0, layoutCallCount);
        Assert.Equal(0, resizeCallCount);
        Assert.Equal(0, sizeChangedCallCount);
        Assert.Equal(0, clientSizeChangedCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Call again.
        control.Location = value;
        Assert.Equal(new Size(0, 0), control.ClientSize);
        Assert.Equal(new Rectangle(0, 0, 0, 0), control.ClientRectangle);
        Assert.Equal(new Rectangle(0, 0, 0, 0), control.DisplayRectangle);
        Assert.Equal(new Size(0, 0), control.Size);
        Assert.Equal(value.X, control.Left);
        Assert.Equal(value.X, control.Right);
        Assert.Equal(value, control.Location);
        Assert.Equal(value.Y, control.Top);
        Assert.Equal(value.Y, control.Bottom);
        Assert.Equal(0, control.Width);
        Assert.Equal(0, control.Height);
        Assert.Equal(new Rectangle(value.X, value.Y, 0, 0), control.Bounds);
        Assert.Equal(expectedLocationChangedCallCount, moveCallCount);
        Assert.Equal(expectedLocationChangedCallCount, locationChangedCallCount);
        Assert.Equal(0, layoutCallCount);
        Assert.Equal(0, resizeCallCount);
        Assert.Equal(0, sizeChangedCallCount);
        Assert.Equal(0, clientSizeChangedCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [MemberData(nameof(Location_SetWithHandle_TestData))]
    public void Control_Location_SetWithParentWithHandle_GetReturnsExpected(bool resizeRedraw, Point value, int expectedLocationChangedCallCount)
    {
        using Control parent = new();
        using SubControl control = new()
        {
            Parent = parent
        };
        control.SetStyle(ControlStyles.ResizeRedraw, resizeRedraw);
        Assert.NotEqual(IntPtr.Zero, parent.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
        int parentInvalidatedCallCount = 0;
        parent.Invalidated += (sender, e) => parentInvalidatedCallCount++;
        int parentStyleChangedCallCount = 0;
        parent.StyleChanged += (sender, e) => parentStyleChangedCallCount++;
        int parentCreatedCallCount = 0;
        parent.HandleCreated += (sender, e) => parentCreatedCallCount++;

        int layoutCallCount = 0;
        int moveCallCount = 0;
        int locationChangedCallCount = 0;
        int resizeCallCount = 0;
        int sizeChangedCallCount = 0;
        int clientSizeChangedCallCount = 0;
        int parentLayoutCallCount = 0;
        control.Move += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(locationChangedCallCount, moveCallCount);
            moveCallCount++;
        };
        control.LocationChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(moveCallCount - 1, locationChangedCallCount);
            locationChangedCallCount++;
        };
        control.Layout += (sender, e) => layoutCallCount++;
        control.Resize += (sender, e) => resizeCallCount++;
        control.SizeChanged += (sender, e) => sizeChangedCallCount++;
        control.ClientSizeChanged += (sender, e) => clientSizeChangedCallCount++;
        void parentHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(parent, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("Bounds", e.AffectedProperty);
            parentLayoutCallCount++;
        }

        parent.Layout += parentHandler;

        try
        {
            control.Location = value;
            Assert.Equal(new Size(0, 0), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, 0, 0), control.ClientRectangle);
            Assert.Equal(new Rectangle(0, 0, 0, 0), control.DisplayRectangle);
            Assert.Equal(new Size(0, 0), control.Size);
            Assert.Equal(value.X, control.Left);
            Assert.Equal(value.X, control.Right);
            Assert.Equal(value, control.Location);
            Assert.Equal(value.Y, control.Top);
            Assert.Equal(value.Y, control.Bottom);
            Assert.Equal(0, control.Width);
            Assert.Equal(0, control.Height);
            Assert.Equal(new Rectangle(value.X, value.Y, 0, 0), control.Bounds);
            Assert.Equal(expectedLocationChangedCallCount, moveCallCount);
            Assert.Equal(expectedLocationChangedCallCount, locationChangedCallCount);
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(expectedLocationChangedCallCount, parentLayoutCallCount);
            Assert.Equal(0, resizeCallCount);
            Assert.Equal(0, sizeChangedCallCount);
            Assert.Equal(0, clientSizeChangedCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.True(parent.IsHandleCreated);
            Assert.Equal(0, parentInvalidatedCallCount);
            Assert.Equal(0, parentStyleChangedCallCount);
            Assert.Equal(0, parentCreatedCallCount);

            // Call again.
            control.Location = value;
            Assert.Equal(new Size(0, 0), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, 0, 0), control.ClientRectangle);
            Assert.Equal(new Rectangle(0, 0, 0, 0), control.DisplayRectangle);
            Assert.Equal(new Size(0, 0), control.Size);
            Assert.Equal(value.X, control.Left);
            Assert.Equal(value.X, control.Right);
            Assert.Equal(value, control.Location);
            Assert.Equal(value.Y, control.Top);
            Assert.Equal(value.Y, control.Bottom);
            Assert.Equal(0, control.Width);
            Assert.Equal(0, control.Height);
            Assert.Equal(new Rectangle(value.X, value.Y, 0, 0), control.Bounds);
            Assert.Equal(expectedLocationChangedCallCount, moveCallCount);
            Assert.Equal(expectedLocationChangedCallCount, locationChangedCallCount);
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(expectedLocationChangedCallCount, parentLayoutCallCount);
            Assert.Equal(0, resizeCallCount);
            Assert.Equal(0, sizeChangedCallCount);
            Assert.Equal(0, clientSizeChangedCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.True(parent.IsHandleCreated);
            Assert.Equal(0, parentInvalidatedCallCount);
            Assert.Equal(0, parentStyleChangedCallCount);
            Assert.Equal(0, parentCreatedCallCount);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    [WinFormsTheory]
    [InlineData(true, 1)]
    [InlineData(false, 0)]
    public void Control_Location_SetWithHandleWithTransparentBackColor_DoesNotCallInvalidate(bool supportsTransparentBackgroundColor, int expectedInvalidatedCallCount)
    {
        using SubControl control = new();
        control.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
        control.BackColor = Color.FromArgb(254, 255, 255, 255);
        control.SetStyle(ControlStyles.SupportsTransparentBackColor, supportsTransparentBackgroundColor);

        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;

        // Set different.
        control.Location = new Point(1, 2);
        Assert.Equal(new Point(1, 2), control.Location);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);

        // Set same.
        control.Location = new Point(1, 2);
        Assert.Equal(new Point(1, 2), control.Location);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);

        // Set different.
        control.Location = new Point(2, 3);
        Assert.Equal(new Point(2, 3), control.Location);
        Assert.Equal(expectedInvalidatedCallCount * 2, invalidatedCallCount);
    }

    [WinFormsFact]
    public void Control_Location_ResetValue_Success()
    {
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(Control))[nameof(Control.Location)];
        using Control control = new();
        Assert.True(property.CanResetValue(control));

        control.Location = new Point(1, 0);
        Assert.Equal(new Point(1, 0), control.Location);
        Assert.True(property.CanResetValue(control));

        control.Location = new Point(0, 1);
        Assert.Equal(new Point(0, 1), control.Location);
        Assert.True(property.CanResetValue(control));

        control.Location = new Point(1, 2);
        Assert.Equal(new Point(1, 2), control.Location);
        Assert.True(property.CanResetValue(control));

        property.ResetValue(control);
        Assert.Equal(Point.Empty, control.Location);
        Assert.True(property.CanResetValue(control));
    }

    [WinFormsFact]
    public void Control_Location_ShouldSerializeValue_Success()
    {
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(Control))[nameof(Control.Location)];
        using Control control = new();
        Assert.True(property.ShouldSerializeValue(control));

        control.Location = new Point(1, 0);
        Assert.Equal(new Point(1, 0), control.Location);
        Assert.True(property.ShouldSerializeValue(control));

        control.Location = new Point(0, 1);
        Assert.Equal(new Point(0, 1), control.Location);
        Assert.True(property.ShouldSerializeValue(control));

        control.Location = new Point(1, 2);
        Assert.Equal(new Point(1, 2), control.Location);
        Assert.True(property.ShouldSerializeValue(control));

        property.ResetValue(control);
        Assert.Equal(Point.Empty, control.Location);
        Assert.True(property.ShouldSerializeValue(control));
    }

    [WinFormsFact]
    public void Control_Location_SetWithHandler_CallsLocationChanged()
    {
        using Control control = new();
        int locationChangedCallCount = 0;
        EventHandler locationChangedHandler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            locationChangedCallCount++;
        };
        control.LocationChanged += locationChangedHandler;
        int moveCallCount = 0;
        EventHandler moveHandler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            moveCallCount++;
        };
        control.Move += moveHandler;

        // Set different.
        control.Location = new Point(1, 2);
        Assert.Equal(new Point(1, 2), control.Location);
        Assert.Equal(1, locationChangedCallCount);
        Assert.Equal(1, moveCallCount);

        // Set same.
        control.Location = new Point(1, 2);
        Assert.Equal(new Point(1, 2), control.Location);
        Assert.Equal(1, locationChangedCallCount);
        Assert.Equal(1, moveCallCount);

        // Set different x.
        control.Location = new Point(2, 2);
        Assert.Equal(new Point(2, 2), control.Location);
        Assert.Equal(2, locationChangedCallCount);
        Assert.Equal(2, moveCallCount);

        // Set different y.
        control.Location = new Point(2, 3);
        Assert.Equal(new Point(2, 3), control.Location);
        Assert.Equal(3, locationChangedCallCount);
        Assert.Equal(3, moveCallCount);

        // Remove handler.
        control.LocationChanged -= locationChangedHandler;
        control.Move -= moveHandler;
        control.Location = new Point(1, 2);
        Assert.Equal(new Point(1, 2), control.Location);
        Assert.Equal(3, locationChangedCallCount);
        Assert.Equal(3, moveCallCount);
    }

    public static IEnumerable<object[]> Margin_Get_TestData()
    {
        yield return new object[] { new Control(), new Padding(3) };
        yield return new object[] { new NonZeroDefaultMarginControl(), new Padding(1, 2, 3, 4) };
        yield return new object[] { new ZeroDefaultMarginControl(), Padding.Empty };
    }

    [WinFormsTheory]
    [MemberData(nameof(Margin_Get_TestData))]
    public void Control_Margin_GetWithCustomDefaultMargin_ReturnsExpected(Control control, Padding expected)
    {
        Assert.Equal(expected, control.Margin);
    }

    private class NonZeroDefaultMarginControl : Control
    {
        protected override Padding DefaultMargin => new(1, 2, 3, 4);
    }

    private class ZeroDefaultMarginControl : Control
    {
        protected override Padding DefaultMargin => Padding.Empty;
    }

    public static IEnumerable<object[]> Margin_Set_TestData()
    {
        yield return new object[] { default(Padding), default(Padding) };
        yield return new object[] { new Padding(1, 2, 3, 4), new Padding(1, 2, 3, 4) };
        yield return new object[] { new Padding(1), new Padding(1) };
        yield return new object[] { new Padding(-1, -2, -3, -4), Padding.Empty };
    }

    [WinFormsTheory]
    [MemberData(nameof(Margin_Set_TestData))]
    public void Control_Margin_Set_GetReturnsExpected(Padding value, Padding expected)
    {
        using Control control = new();
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;

        control.Margin = value;
        Assert.Equal(expected, control.Margin);
        Assert.Equal(0, layoutCallCount);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.Margin = value;
        Assert.Equal(expected, control.Margin);
        Assert.Equal(0, layoutCallCount);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(Margin_Set_TestData))]
    public void Control_Margin_SetWithParent_GetReturnsExpected(Padding value, Padding expected)
    {
        using Control parent = new();
        using Control control = new()
        {
            Parent = parent
        };
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;
        int parentLayoutCallCount = 0;
        parent.Layout += (sender, e) => parentLayoutCallCount++;

        control.Margin = value;
        Assert.Equal(expected, control.Margin);
        Assert.Equal(0, layoutCallCount);
        Assert.Equal(1, parentLayoutCallCount);
        Assert.False(control.IsHandleCreated);
        Assert.False(parent.IsHandleCreated);

        // Set same.
        control.Margin = value;
        Assert.Equal(expected, control.Margin);
        Assert.Equal(0, layoutCallCount);
        Assert.Equal(1, parentLayoutCallCount);
        Assert.False(control.IsHandleCreated);
        Assert.False(parent.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(Margin_Set_TestData))]
    public void Control_Margin_SetWithHandle_GetReturnsExpected(Padding value, Padding expected)
    {
        using Control control = new();
        int layoutCallCount = 0;
        control.Layout += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("Padding", e.AffectedProperty);
            layoutCallCount++;
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.Margin = value;
        Assert.Equal(expected, control.Margin);
        Assert.Equal(0, layoutCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.Margin = value;
        Assert.Equal(expected, control.Margin);
        Assert.Equal(0, layoutCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [MemberData(nameof(Margin_Set_TestData))]
    public void Control_Margin_SetWithParentWithHandle_GetReturnsExpected(Padding value, Padding expected)
    {
        using Control parent = new();
        using Control control = new()
        {
            Parent = parent
        };
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;
        int parentLayoutCallCount = 0;
        parent.Layout += (sender, e) => parentLayoutCallCount++;
        Assert.NotEqual(IntPtr.Zero, parent.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
        int parentInvalidatedCallCount = 0;
        parent.Invalidated += (sender, e) => parentInvalidatedCallCount++;
        int parentStyleChangedCallCount = 0;
        parent.StyleChanged += (sender, e) => parentStyleChangedCallCount++;
        int parentCreatedCallCount = 0;
        parent.HandleCreated += (sender, e) => parentCreatedCallCount++;

        control.Margin = value;
        Assert.Equal(expected, control.Margin);
        Assert.Equal(0, layoutCallCount);
        Assert.Equal(1, parentLayoutCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.True(parent.IsHandleCreated);
        Assert.Equal(0, parentInvalidatedCallCount);
        Assert.Equal(0, parentStyleChangedCallCount);
        Assert.Equal(0, parentCreatedCallCount);

        // Set same.
        control.Margin = value;
        Assert.Equal(expected, control.Margin);
        Assert.Equal(0, layoutCallCount);
        Assert.Equal(1, parentLayoutCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.True(parent.IsHandleCreated);
        Assert.Equal(0, parentInvalidatedCallCount);
        Assert.Equal(0, parentStyleChangedCallCount);
        Assert.Equal(0, parentCreatedCallCount);
    }

    [WinFormsFact]
    public void Control_Margin_ResetValue_Success()
    {
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(Control))[nameof(Control.Margin)];
        using Control control = new();
        Assert.False(property.CanResetValue(control));

        control.Margin = new Padding(1, 2, 3, 4);
        Assert.Equal(new Padding(1, 2, 3, 4), control.Margin);
        Assert.True(property.CanResetValue(control));

        property.ResetValue(control);
        Assert.Equal(new Padding(3), control.Margin);
        Assert.False(property.CanResetValue(control));
    }

    [WinFormsFact]
    public void Control_Margin_ShouldSerializeValue_Success()
    {
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(Control))[nameof(Control.Margin)];
        using Control control = new();
        Assert.False(property.ShouldSerializeValue(control));

        control.Margin = new Padding(1, 2, 3, 4);
        Assert.Equal(new Padding(1, 2, 3, 4), control.Margin);
        Assert.True(property.ShouldSerializeValue(control));

        property.ResetValue(control);
        Assert.Equal(new Padding(3), control.Margin);
        Assert.False(property.ShouldSerializeValue(control));
    }

    public static IEnumerable<object[]> MaximumSize_Get_TestData()
    {
        yield return new object[] { new Control(), Size.Empty };
        yield return new object[] { new NonZeroWidthDefaultMaximumSizeControl(), new Size(10, 0) };
        yield return new object[] { new NonZeroHeightDefaultMaximumSizeControl(), new Size(0, 20) };
        yield return new object[] { new NonZeroWidthNonZeroHeightDefaultMaximumSizeControl(), new Size(10, 20) };
    }

    [WinFormsTheory]
    [MemberData(nameof(MaximumSize_Get_TestData))]
    public void Control_MaximumSize_GetWithCustomDefaultMaximumSize_ReturnsExpected(Control control, Size expected)
    {
        Assert.Equal(expected, control.MaximumSize);
    }

    private class NonZeroWidthDefaultMaximumSizeControl : Control
    {
        protected override Size DefaultMaximumSize => new(10, 0);
    }

    private class NonZeroHeightDefaultMaximumSizeControl : Control
    {
        protected override Size DefaultMaximumSize => new(0, 20);
    }

    private class NonZeroWidthNonZeroHeightDefaultMaximumSizeControl : Control
    {
        protected override Size DefaultMaximumSize => new(10, 20);
    }

    public static IEnumerable<object[]> MaximumSize_Set_TestData()
    {
        yield return new object[] { Size.Empty };
        yield return new object[] { new Size(-1, -2) };
        yield return new object[] { new Size(0, 1) };
        yield return new object[] { new Size(0, 10) };
        yield return new object[] { new Size(1, 0) };
        yield return new object[] { new Size(10, 0) };
        yield return new object[] { new Size(1, 2) };
        yield return new object[] { new Size(3, 4) };
        yield return new object[] { new Size(ushort.MaxValue - 1, ushort.MaxValue - 1) };
        yield return new object[] { new Size(ushort.MaxValue, ushort.MaxValue) };
        yield return new object[] { new Size(ushort.MaxValue + 1, ushort.MaxValue + 1) };
        yield return new object[] { new Size(int.MaxValue, int.MaxValue) };
    }

    [WinFormsTheory]
    [MemberData(nameof(MaximumSize_Set_TestData))]
    public void Control_MaximumSize_Set_GetReturnsExpected(Size value)
    {
        using Control control = new();
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;

        control.MaximumSize = value;
        Assert.Equal(value, control.MaximumSize);
        Assert.Equal(Size.Empty, control.Size);
        Assert.Equal(0, layoutCallCount);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.MaximumSize = value;
        Assert.Equal(value, control.MaximumSize);
        Assert.Equal(Size.Empty, control.Size);
        Assert.Equal(0, layoutCallCount);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(MaximumSize_Set_TestData))]
    public void Control_MaximumSize_SetWithCustomOldValue_GetReturnsExpected(Size value)
    {
        using Control control = new()
        {
            MaximumSize = new Size(1, 2)
        };
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;

        control.MaximumSize = value;
        Assert.Equal(value, control.MaximumSize);
        Assert.Equal(Size.Empty, control.Size);
        Assert.Equal(0, layoutCallCount);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.MaximumSize = value;
        Assert.Equal(value, control.MaximumSize);
        Assert.Equal(Size.Empty, control.Size);
        Assert.Equal(0, layoutCallCount);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> MaximumSize_SetWithSize_TestData()
    {
        yield return new object[] { new Size(2, 3), Size.Empty, new Size(2, 3), 0 };
        yield return new object[] { new Size(2, 3), new Size(0, 1), new Size(0, 1), 1 };
        yield return new object[] { new Size(2, 3), new Size(0, 10), new Size(0, 3), 1 };
        yield return new object[] { new Size(2, 3), new Size(1, 0), new Size(1, 0), 1 };
        yield return new object[] { new Size(2, 3), new Size(10, 0), new Size(2, 0), 1 };
        yield return new object[] { new Size(2, 3), new Size(1, 2), new Size(1, 2), 1 };
        yield return new object[] { new Size(2, 3), new Size(2, 2), new Size(2, 2), 1 };
        yield return new object[] { new Size(2, 3), new Size(1, 3), new Size(1, 3), 1 };
        yield return new object[] { new Size(2, 3), new Size(2, 3), new Size(2, 3), 0 };
        yield return new object[] { new Size(2, 3), new Size(3, 3), new Size(2, 3), 0 };
        yield return new object[] { new Size(2, 3), new Size(2, 4), new Size(2, 3), 0 };
        yield return new object[] { new Size(2, 3), new Size(3, 4), new Size(2, 3), 0 };
        yield return new object[] { new Size(2, 3), new Size(5, 6), new Size(2, 3), 0 };
        yield return new object[] { new Size(2, 3), new Size(ushort.MaxValue - 1, ushort.MaxValue - 1), new Size(2, 3), 0 };
        yield return new object[] { new Size(2, 3), new Size(ushort.MaxValue, ushort.MaxValue), new Size(2, 3), 0 };
        yield return new object[] { new Size(2, 3), new Size(ushort.MaxValue + 1, ushort.MaxValue + 1), new Size(2, 3), 0 };
        yield return new object[] { new Size(2, 3), new Size(int.MaxValue, int.MaxValue), new Size(2, 3), 0 };
    }

    [WinFormsTheory]
    [MemberData(nameof(MaximumSize_SetWithSize_TestData))]
    public void Control_MaximumSize_SetWithSize_GetReturnsExpected(Size size, Size value, Size expectedSize, int expectedLayoutCallCount)
    {
        using Control control = new()
        {
            Size = size
        };
        int layoutCallCount = 0;
        control.Layout += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("Bounds", e.AffectedProperty);
            layoutCallCount++;
        };

        control.MaximumSize = value;
        Assert.Equal(value, control.MaximumSize);
        Assert.Equal(expectedSize, control.Size);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.MaximumSize = value;
        Assert.Equal(value, control.MaximumSize);
        Assert.Equal(expectedSize, control.Size);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(MaximumSize_Set_TestData))]
    public void Control_MaximumSize_SetWithMinimumSize_GetReturnsExpected(Size value)
    {
        using Control control = new()
        {
            MinimumSize = new Size(100, 100)
        };
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;

        control.MaximumSize = value;
        Assert.Equal(value, control.MaximumSize);
        Assert.Equal(new Size(100, 100), control.Size);
        Assert.Equal(0, layoutCallCount);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.MaximumSize = value;
        Assert.Equal(value, control.MaximumSize);
        Assert.Equal(new Size(100, 100), control.Size);
        Assert.Equal(0, layoutCallCount);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(MaximumSize_SetWithSize_TestData))]
    public void Control_MaximumSize_SetWithCustomOldValueWithSize_GetReturnsExpected(Size size, Size value, Size expectedSize, int expectedLayoutCallCount)
    {
        using Control control = new()
        {
            MaximumSize = new Size(4, 5),
            Size = size
        };
        int layoutCallCount = 0;
        control.Layout += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("Bounds", e.AffectedProperty);
            layoutCallCount++;
        };

        control.MaximumSize = value;
        Assert.Equal(value, control.MaximumSize);
        Assert.Equal(expectedSize, control.Size);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.MaximumSize = value;
        Assert.Equal(value, control.MaximumSize);
        Assert.Equal(expectedSize, control.Size);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> MaximumSize_SetWithParent_TestData()
    {
        yield return new object[] { Size.Empty, 0 };
        yield return new object[] { new Size(0, 1), 1 };
        yield return new object[] { new Size(0, 10), 1 };
        yield return new object[] { new Size(1, 0), 1 };
        yield return new object[] { new Size(10, 0), 1 };
        yield return new object[] { new Size(-1, -2), 1 };
        yield return new object[] { new Size(1, 2), 1 };
        yield return new object[] { new Size(ushort.MaxValue - 1, ushort.MaxValue - 1), 1 };
        yield return new object[] { new Size(ushort.MaxValue, ushort.MaxValue), 1 };
        yield return new object[] { new Size(ushort.MaxValue + 1, ushort.MaxValue + 1), 1 };
        yield return new object[] { new Size(int.MaxValue, int.MaxValue), 1 };
    }

    [WinFormsTheory]
    [MemberData(nameof(MaximumSize_SetWithParent_TestData))]
    public void Control_MaximumSize_SetWithParent_GetReturnsExpected(Size value, int expectedParentLayoutCallCount)
    {
        using Control parent = new();
        using Control control = new()
        {
            Parent = parent
        };
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(parent, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("MaximumSize", e.AffectedProperty);
            parentLayoutCallCount++;
        }

        parent.Layout += parentHandler;

        control.MaximumSize = value;
        Assert.Equal(value, control.MaximumSize);
        Assert.Equal(Size.Empty, control.Size);
        Assert.Equal(0, layoutCallCount);
        Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.MaximumSize = value;
        Assert.Equal(value, control.MaximumSize);
        Assert.Equal(Size.Empty, control.Size);
        Assert.Equal(0, layoutCallCount);
        Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
        Assert.False(control.IsHandleCreated);

        parent.Layout -= parentHandler;
    }

    public static IEnumerable<object[]> MaximumSize_SetCustomOldValueWithParent_TestData()
    {
        yield return new object[] { Size.Empty, 0 };
        yield return new object[] { new Size(0, 1), 1 };
        yield return new object[] { new Size(0, 10), 1 };
        yield return new object[] { new Size(1, 0), 1 };
        yield return new object[] { new Size(10, 0), 1 };
        yield return new object[] { new Size(-1, -2), 1 };
        yield return new object[] { new Size(1, 2), 0 };
        yield return new object[] { new Size(ushort.MaxValue - 1, ushort.MaxValue - 1), 1 };
        yield return new object[] { new Size(ushort.MaxValue, ushort.MaxValue), 1 };
        yield return new object[] { new Size(ushort.MaxValue + 1, ushort.MaxValue + 1), 1 };
        yield return new object[] { new Size(int.MaxValue, int.MaxValue), 1 };
    }

    [WinFormsTheory]
    [MemberData(nameof(MaximumSize_SetCustomOldValueWithParent_TestData))]
    public void Control_MaximumSize_SetWithCustomOldValueWithParent_GetReturnsExpected(Size value, int expectedParentLayoutCallCount)
    {
        using Control parent = new();
        using Control control = new()
        {
            MaximumSize = new Size(1, 2),
            Parent = parent
        };
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(parent, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("MaximumSize", e.AffectedProperty);
            parentLayoutCallCount++;
        }

        parent.Layout += parentHandler;

        control.MaximumSize = value;
        Assert.Equal(value, control.MaximumSize);
        Assert.Equal(Size.Empty, control.Size);
        Assert.Equal(0, layoutCallCount);
        Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
        Assert.False(control.IsHandleCreated);
        Assert.False(parent.IsHandleCreated);

        // Set same.
        control.MaximumSize = value;
        Assert.Equal(value, control.MaximumSize);
        Assert.Equal(Size.Empty, control.Size);
        Assert.Equal(0, layoutCallCount);
        Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
        Assert.False(control.IsHandleCreated);
        Assert.False(parent.IsHandleCreated);

        parent.Layout -= parentHandler;
    }

    [WinFormsTheory]
    [MemberData(nameof(MaximumSize_Set_TestData))]
    public void Control_MaximumSize_SetWithHandle_GetReturnsExpected(Size value)
    {
        using Control control = new();
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.MaximumSize = value;
        Assert.Equal(value, control.MaximumSize);
        Assert.Equal(Size.Empty, control.Size);
        Assert.Equal(0, layoutCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.MaximumSize = value;
        Assert.Equal(value, control.MaximumSize);
        Assert.Equal(Size.Empty, control.Size);
        Assert.Equal(0, layoutCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [MemberData(nameof(MaximumSize_Set_TestData))]
    public void Control_MaximumSize_SetWithCustomOldValueWithHandle_GetReturnsExpected(Size value)
    {
        using Control control = new()
        {
            MaximumSize = new Size(1, 2)
        };
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.MaximumSize = value;
        Assert.Equal(value, control.MaximumSize);
        Assert.Equal(Size.Empty, control.Size);
        Assert.Equal(0, layoutCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.MaximumSize = value;
        Assert.Equal(value, control.MaximumSize);
        Assert.Equal(Size.Empty, control.Size);
        Assert.Equal(0, layoutCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [MemberData(nameof(MaximumSize_SetWithSize_TestData))]
    public void Control_MaximumSize_SetWithSizeWithHandle_GetReturnsExpected(Size size, Size value, Size expectedSize, int expectedLayoutCallCount)
    {
        using Control control = new()
        {
            Size = size
        };
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.MaximumSize = value;
        Assert.Equal(value, control.MaximumSize);
        Assert.Equal(expectedSize, control.Size);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.MaximumSize = value;
        Assert.Equal(value, control.MaximumSize);
        Assert.Equal(expectedSize, control.Size);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [MemberData(nameof(MaximumSize_SetWithParent_TestData))]
    public void Control_MaximumSize_SetWithParentWithHandle_GetReturnsExpected(Size value, int expectedParentLayoutCallCount)
    {
        using Control parent = new();
        using Control control = new()
        {
            Parent = parent
        };
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(parent, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("MaximumSize", e.AffectedProperty);
            parentLayoutCallCount++;
        }

        parent.Layout += parentHandler;
        Assert.NotEqual(IntPtr.Zero, parent.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
        int parentInvalidatedCallCount = 0;
        parent.Invalidated += (sender, e) => parentInvalidatedCallCount++;
        int parentStyleChangedCallCount = 0;
        parent.StyleChanged += (sender, e) => parentStyleChangedCallCount++;
        int parentCreatedCallCount = 0;
        parent.HandleCreated += (sender, e) => parentCreatedCallCount++;

        control.MaximumSize = value;
        Assert.Equal(value, control.MaximumSize);
        Assert.Equal(Size.Empty, control.Size);
        Assert.Equal(0, layoutCallCount);
        Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.True(parent.IsHandleCreated);
        Assert.Equal(0, parentInvalidatedCallCount);
        Assert.Equal(0, parentStyleChangedCallCount);
        Assert.Equal(0, parentCreatedCallCount);

        // Set same.
        control.MaximumSize = value;
        Assert.Equal(value, control.MaximumSize);
        Assert.Equal(Size.Empty, control.Size);
        Assert.Equal(0, layoutCallCount);
        Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.True(parent.IsHandleCreated);
        Assert.Equal(0, parentInvalidatedCallCount);
        Assert.Equal(0, parentStyleChangedCallCount);
        Assert.Equal(0, parentCreatedCallCount);

        parent.Layout -= parentHandler;
    }

    [WinFormsTheory]
    [MemberData(nameof(MaximumSize_SetCustomOldValueWithParent_TestData))]
    public void Control_MaximumSize_SetWithCustomOldValueWithParentWithHandle_GetReturnsExpected(Size value, int expectedParentLayoutCallCount)
    {
        using Control parent = new();
        using Control control = new()
        {
            MaximumSize = new Size(1, 2),
            Parent = parent
        };
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(parent, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("MaximumSize", e.AffectedProperty);
            parentLayoutCallCount++;
        }

        parent.Layout += parentHandler;
        Assert.NotEqual(IntPtr.Zero, parent.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
        int parentInvalidatedCallCount = 0;
        parent.Invalidated += (sender, e) => parentInvalidatedCallCount++;
        int parentStyleChangedCallCount = 0;
        parent.StyleChanged += (sender, e) => parentStyleChangedCallCount++;
        int parentCreatedCallCount = 0;
        parent.HandleCreated += (sender, e) => parentCreatedCallCount++;

        control.MaximumSize = value;
        Assert.Equal(value, control.MaximumSize);
        Assert.Equal(Size.Empty, control.Size);
        Assert.Equal(0, layoutCallCount);
        Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.True(parent.IsHandleCreated);
        Assert.Equal(0, parentInvalidatedCallCount);
        Assert.Equal(0, parentStyleChangedCallCount);
        Assert.Equal(0, parentCreatedCallCount);

        // Set same.
        control.MaximumSize = value;
        Assert.Equal(value, control.MaximumSize);
        Assert.Equal(Size.Empty, control.Size);
        Assert.Equal(0, layoutCallCount);
        Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.True(parent.IsHandleCreated);
        Assert.Equal(0, parentInvalidatedCallCount);
        Assert.Equal(0, parentStyleChangedCallCount);
        Assert.Equal(0, parentCreatedCallCount);

        parent.Layout -= parentHandler;
    }

    [WinFormsFact]
    public void Control_MaximumSize_ResetValue_Success()
    {
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(Control))[nameof(Control.MaximumSize)];
        using Control control = new();
        Assert.False(property.CanResetValue(control));

        control.MaximumSize = new Size(1, 0);
        Assert.Equal(new Size(1, 0), control.MaximumSize);
        Assert.True(property.CanResetValue(control));

        control.MaximumSize = new Size(0, 1);
        Assert.Equal(new Size(0, 1), control.MaximumSize);
        Assert.True(property.CanResetValue(control));

        control.MaximumSize = new Size(1, 2);
        Assert.Equal(new Size(1, 2), control.MaximumSize);
        Assert.True(property.CanResetValue(control));

        property.ResetValue(control);
        Assert.Equal(Size.Empty, control.MaximumSize);
        Assert.False(property.CanResetValue(control));
    }

    [WinFormsFact]
    public void Control_MaximumSize_ShouldSerializeValue_Success()
    {
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(Control))[nameof(Control.MaximumSize)];
        using Control control = new();
        Assert.False(property.ShouldSerializeValue(control));

        control.MaximumSize = new Size(1, 0);
        Assert.Equal(new Size(1, 0), control.MaximumSize);
        Assert.True(property.ShouldSerializeValue(control));

        control.MaximumSize = new Size(0, 1);
        Assert.Equal(new Size(0, 1), control.MaximumSize);
        Assert.True(property.ShouldSerializeValue(control));

        control.MaximumSize = new Size(1, 2);
        Assert.Equal(new Size(1, 2), control.MaximumSize);
        Assert.True(property.ShouldSerializeValue(control));

        property.ResetValue(control);
        Assert.Equal(Size.Empty, control.MaximumSize);
        Assert.False(property.ShouldSerializeValue(control));
    }

    public static IEnumerable<object[]> MinimumSize_Get_TestData()
    {
        yield return new object[] { new Control(), Size.Empty };
        yield return new object[] { new NonZeroWidthDefaultMinimumSizeControl(), new Size(10, 0) };
        yield return new object[] { new NonZeroHeightDefaultMinimumSizeControl(), new Size(0, 20) };
        yield return new object[] { new NonZeroWidthNonZeroHeightDefaultMinimumSizeControl(), new Size(10, 20) };
    }

    [WinFormsTheory]
    [MemberData(nameof(MinimumSize_Get_TestData))]
    public void Control_MinimumSize_GetWithCustomDefaultMinimumSize_ReturnsExpected(Control control, Size expected)
    {
        Assert.Equal(expected, control.MinimumSize);
    }

    private class NonZeroWidthDefaultMinimumSizeControl : Control
    {
        protected override Size DefaultMinimumSize => new(10, 0);
    }

    private class NonZeroHeightDefaultMinimumSizeControl : Control
    {
        protected override Size DefaultMinimumSize => new(0, 20);
    }

    private class NonZeroWidthNonZeroHeightDefaultMinimumSizeControl : Control
    {
        protected override Size DefaultMinimumSize => new(10, 20);
    }

    public static IEnumerable<object[]> MinimumSize_Set_TestData()
    {
        yield return new object[] { Size.Empty, Size.Empty, 0 };
        yield return new object[] { new Size(0, 1), new Size(0, 1), 1 };
        yield return new object[] { new Size(0, 10), new Size(0, 10), 1 };
        yield return new object[] { new Size(1, 0), new Size(1, 0), 1 };
        yield return new object[] { new Size(10, 0), new Size(10, 0), 1 };
        yield return new object[] { new Size(-1, -2), Size.Empty, 0 };
        yield return new object[] { new Size(1, 2), new Size(1, 2), 1 };
        yield return new object[] { new Size(3, 4), new Size(3, 4), 1 };
        yield return new object[] { new Size(ushort.MaxValue - 1, ushort.MaxValue - 1), new Size(ushort.MaxValue - 1, ushort.MaxValue - 1), 1 };
        yield return new object[] { new Size(ushort.MaxValue, ushort.MaxValue), new Size(ushort.MaxValue, ushort.MaxValue), 1 };
        yield return new object[] { new Size(ushort.MaxValue + 1, ushort.MaxValue + 1), new Size(ushort.MaxValue + 1, ushort.MaxValue + 1), 1 };
        yield return new object[] { new Size(int.MaxValue, int.MaxValue), new Size(int.MaxValue, int.MaxValue), 1 };
    }

    [WinFormsTheory]
    [MemberData(nameof(MinimumSize_Set_TestData))]
    public void Control_MinimumSize_Set_GetReturnsExpected(Size value, Size expectedSize, int expectedLayoutCallCount)
    {
        using Control control = new();
        int layoutCallCount = 0;
        control.Layout += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("Bounds", e.AffectedProperty);
            layoutCallCount++;
        };

        control.MinimumSize = value;
        Assert.Equal(value, control.MinimumSize);
        Assert.Equal(expectedSize, control.Size);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.MinimumSize = value;
        Assert.Equal(value, control.MinimumSize);
        Assert.Equal(expectedSize, control.Size);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> MinimumSize_SetWithCustomOldValue_TestData()
    {
        yield return new object[] { Size.Empty, new Size(1, 2), 0 };
        yield return new object[] { new Size(0, 1), new Size(1, 2), 0 };
        yield return new object[] { new Size(0, 10), new Size(1, 10), 1 };
        yield return new object[] { new Size(1, 0), new Size(1, 2), 0 };
        yield return new object[] { new Size(10, 0), new Size(10, 2), 1 };
        yield return new object[] { new Size(-1, -2), new Size(1, 2), 0 };
        yield return new object[] { new Size(1, 2), new Size(1, 2), 0 };
        yield return new object[] { new Size(3, 4), new Size(3, 4), 1 };
        yield return new object[] { new Size(ushort.MaxValue - 1, ushort.MaxValue - 1), new Size(ushort.MaxValue - 1, ushort.MaxValue - 1), 1 };
        yield return new object[] { new Size(ushort.MaxValue, ushort.MaxValue), new Size(ushort.MaxValue, ushort.MaxValue), 1 };
        yield return new object[] { new Size(ushort.MaxValue + 1, ushort.MaxValue + 1), new Size(ushort.MaxValue + 1, ushort.MaxValue + 1), 1 };
        yield return new object[] { new Size(int.MaxValue, int.MaxValue), new Size(int.MaxValue, int.MaxValue), 1 };
    }

    [WinFormsTheory]
    [MemberData(nameof(MinimumSize_SetWithCustomOldValue_TestData))]
    public void Control_MinimumSize_SetWithCustomOldValue_GetReturnsExpected(Size value, Size expectedSize, int expectedLayoutCallCount)
    {
        using Control control = new()
        {
            MinimumSize = new Size(1, 2)
        };
        int layoutCallCount = 0;
        control.Layout += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("Bounds", e.AffectedProperty);
            layoutCallCount++;
        };

        control.MinimumSize = value;
        Assert.Equal(value, control.MinimumSize);
        Assert.Equal(expectedSize, control.Size);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.MinimumSize = value;
        Assert.Equal(value, control.MinimumSize);
        Assert.Equal(expectedSize, control.Size);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> MinimumSize_SetWithSize_TestData()
    {
        yield return new object[] { new Size(2, 3), Size.Empty, new Size(2, 3), 0 };
        yield return new object[] { new Size(2, 3), new Size(0, 1), new Size(2, 3), 0 };
        yield return new object[] { new Size(2, 3), new Size(0, 10), new Size(2, 10), 1 };
        yield return new object[] { new Size(2, 3), new Size(1, 0), new Size(2, 3), 0 };
        yield return new object[] { new Size(2, 3), new Size(10, 0), new Size(10, 3), 1 };
        yield return new object[] { new Size(2, 3), new Size(-1, -2), new Size(2, 3), 0 };
        yield return new object[] { new Size(2, 3), new Size(1, 2), new Size(2, 3), 0 };
        yield return new object[] { new Size(2, 3), new Size(2, 2), new Size(2, 3), 0 };
        yield return new object[] { new Size(2, 3), new Size(1, 3), new Size(2, 3), 0 };
        yield return new object[] { new Size(2, 3), new Size(2, 3), new Size(2, 3), 0 };
        yield return new object[] { new Size(2, 3), new Size(3, 3), new Size(3, 3), 1 };
        yield return new object[] { new Size(2, 3), new Size(2, 4), new Size(2, 4), 1 };
        yield return new object[] { new Size(2, 3), new Size(3, 4), new Size(3, 4), 1 };
        yield return new object[] { new Size(2, 3), new Size(5, 6), new Size(5, 6), 1 };
        yield return new object[] { new Size(2, 3), new Size(ushort.MaxValue - 1, ushort.MaxValue - 1), new Size(ushort.MaxValue - 1, ushort.MaxValue - 1), 1 };
        yield return new object[] { new Size(2, 3), new Size(ushort.MaxValue, ushort.MaxValue), new Size(ushort.MaxValue, ushort.MaxValue), 1 };
        yield return new object[] { new Size(2, 3), new Size(ushort.MaxValue + 1, ushort.MaxValue + 1), new Size(ushort.MaxValue + 1, ushort.MaxValue + 1), 1 };
        yield return new object[] { new Size(2, 3), new Size(int.MaxValue, int.MaxValue), new Size(int.MaxValue, int.MaxValue), 1 };
    }

    [WinFormsTheory]
    [MemberData(nameof(MinimumSize_SetWithSize_TestData))]
    public void Control_MinimumSize_SetWithSize_GetReturnsExpected(Size size, Size value, Size expectedSize, int expectedLayoutCallCount)
    {
        using Control control = new()
        {
            Size = size
        };
        int layoutCallCount = 0;
        control.Layout += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("Bounds", e.AffectedProperty);
            layoutCallCount++;
        };

        control.MinimumSize = value;
        Assert.Equal(value, control.MinimumSize);
        Assert.Equal(expectedSize, control.Size);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.MinimumSize = value;
        Assert.Equal(value, control.MinimumSize);
        Assert.Equal(expectedSize, control.Size);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(MinimumSize_Set_TestData))]
    public void Control_MinimumSize_SetWithMaximumSize_GetReturnsExpected(Size value, Size expectedSize, int expectedLayoutCallCount)
    {
        using Control control = new()
        {
            MaximumSize = new Size(-1, -2)
        };
        int layoutCallCount = 0;
        control.Layout += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("Bounds", e.AffectedProperty);
            layoutCallCount++;
        };

        control.MinimumSize = value;
        Assert.Equal(value, control.MinimumSize);
        Assert.Equal(expectedSize, control.Size);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.MinimumSize = value;
        Assert.Equal(value, control.MinimumSize);
        Assert.Equal(expectedSize, control.Size);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(MinimumSize_SetWithSize_TestData))]
    public void Control_MinimumSize_SetWithCustomOldValueWithSize_GetReturnsExpected(Size size, Size value, Size expectedSize, int expectedLayoutCallCount)
    {
        using Control control = new()
        {
            MinimumSize = new Size(1, 2),
            Size = size
        };
        int layoutCallCount = 0;
        control.Layout += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("Bounds", e.AffectedProperty);
            layoutCallCount++;
        };

        control.MinimumSize = value;
        Assert.Equal(value, control.MinimumSize);
        Assert.Equal(expectedSize, control.Size);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.MinimumSize = value;
        Assert.Equal(value, control.MinimumSize);
        Assert.Equal(expectedSize, control.Size);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> MinimumSize_SetWithParent_TestData()
    {
        yield return new object[] { Size.Empty, Size.Empty, 0, 0 };
        yield return new object[] { new Size(0, 1), new Size(0, 1), 1, 1 };
        yield return new object[] { new Size(0, 10), new Size(0, 10), 1, 1 };
        yield return new object[] { new Size(1, 0), new Size(1, 0), 1, 1 };
        yield return new object[] { new Size(10, 0), new Size(10, 0), 1, 1 };
        yield return new object[] { new Size(-1, -2), Size.Empty, 0, 1 };
        yield return new object[] { new Size(1, 2), new Size(1, 2), 1, 1 };
        yield return new object[] { new Size(ushort.MaxValue - 1, ushort.MaxValue - 1), new Size(ushort.MaxValue - 1, ushort.MaxValue - 1), 1, 1 };
        yield return new object[] { new Size(ushort.MaxValue, ushort.MaxValue), new Size(ushort.MaxValue, ushort.MaxValue), 1, 1 };
        yield return new object[] { new Size(ushort.MaxValue + 1, ushort.MaxValue + 1), new Size(ushort.MaxValue + 1, ushort.MaxValue + 1), 1, 1 };
        yield return new object[] { new Size(int.MaxValue, int.MaxValue), new Size(int.MaxValue, int.MaxValue), 1, 1 };
    }

    [WinFormsTheory]
    [MemberData(nameof(MinimumSize_SetWithParent_TestData))]
    public void Control_MinimumSize_SetWithParent_GetReturnsExpected(Size value, Size expectedSize, int expectedLayoutCallCount, int expectedParentLayoutCallCount)
    {
        using Control parent = new();
        using Control control = new()
        {
            Parent = parent
        };
        int layoutCallCount = 0;
        control.Layout += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("Bounds", e.AffectedProperty);
            layoutCallCount++;
        };
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(parent, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("MinimumSize", e.AffectedProperty);
            parentLayoutCallCount++;
        }

        parent.Layout += parentHandler;

        control.MinimumSize = value;
        Assert.Equal(value, control.MinimumSize);
        Assert.Equal(expectedSize, control.Size);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.MinimumSize = value;
        Assert.Equal(value, control.MinimumSize);
        Assert.Equal(expectedSize, control.Size);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
        Assert.False(control.IsHandleCreated);

        parent.Layout -= parentHandler;
    }

    public static IEnumerable<object[]> MinimumSize_SetCustomOldValueWithParent_TestData()
    {
        yield return new object[] { Size.Empty, new Size(1, 2), 0, 1 };
        yield return new object[] { new Size(0, 1), new Size(1, 2), 0, 1 };
        yield return new object[] { new Size(0, 10), new Size(1, 10), 1, 1 };
        yield return new object[] { new Size(1, 0), new Size(1, 2), 0, 1 };
        yield return new object[] { new Size(10, 0), new Size(10, 2), 1, 1 };
        yield return new object[] { new Size(-1, -2), new Size(1, 2), 0, 1 };
        yield return new object[] { new Size(1, 2), new Size(1, 2), 0, 0 };
        yield return new object[] { new Size(ushort.MaxValue - 1, ushort.MaxValue - 1), new Size(ushort.MaxValue - 1, ushort.MaxValue - 1), 1, 1 };
        yield return new object[] { new Size(ushort.MaxValue, ushort.MaxValue), new Size(ushort.MaxValue, ushort.MaxValue), 1, 1 };
        yield return new object[] { new Size(ushort.MaxValue + 1, ushort.MaxValue + 1), new Size(ushort.MaxValue + 1, ushort.MaxValue + 1), 1, 1 };
        yield return new object[] { new Size(int.MaxValue, int.MaxValue), new Size(int.MaxValue, int.MaxValue), 1, 1 };
    }

    [WinFormsTheory]
    [MemberData(nameof(MinimumSize_SetCustomOldValueWithParent_TestData))]
    public void Control_MinimumSize_SetWithCustomOldValueWithParent_GetReturnsExpected(Size value, Size expectedSize, int expectedLayoutCallCount, int expectedParentLayoutCallCount)
    {
        using Control parent = new();
        using Control control = new()
        {
            MinimumSize = new Size(1, 2),
            Parent = parent
        };
        int layoutCallCount = 0;
        control.Layout += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("Bounds", e.AffectedProperty);
            layoutCallCount++;
        };
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(parent, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("MinimumSize", e.AffectedProperty);
            parentLayoutCallCount++;
        }

        parent.Layout += parentHandler;

        control.MinimumSize = value;
        Assert.Equal(value, control.MinimumSize);
        Assert.Equal(expectedSize, control.Size);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
        Assert.False(control.IsHandleCreated);
        Assert.False(parent.IsHandleCreated);

        // Set same.
        control.MinimumSize = value;
        Assert.Equal(value, control.MinimumSize);
        Assert.Equal(expectedSize, control.Size);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
        Assert.False(control.IsHandleCreated);
        Assert.False(parent.IsHandleCreated);

        parent.Layout -= parentHandler;
    }

    public static IEnumerable<object[]> MinimumSize_SetWithHandle_TestData()
    {
        yield return new object[] { Size.Empty, Size.Empty, 0 };
        yield return new object[] { new Size(0, 1), new Size(0, 1), 1 };
        yield return new object[] { new Size(0, 10), new Size(0, 10), 1 };
        yield return new object[] { new Size(1, 0), new Size(1, 0), 1 };
        yield return new object[] { new Size(10, 0), new Size(10, 0), 1 };
        yield return new object[] { new Size(-1, -2), Size.Empty, 0 };
        yield return new object[] { new Size(1, 2), new Size(1, 2), 1 };
        yield return new object[] { new Size(3, 4), new Size(3, 4), 1 };
        yield return new object[] { new Size(ushort.MaxValue - 1, ushort.MaxValue - 1), new Size(ushort.MaxValue - 1, ushort.MaxValue - 1), 1 };
        yield return new object[] { new Size(ushort.MaxValue, ushort.MaxValue), new Size(ushort.MaxValue, ushort.MaxValue), 1 };
        yield return new object[] { new Size(ushort.MaxValue + 1, ushort.MaxValue + 1), new Size(ushort.MaxValue, ushort.MaxValue), 1 };
        yield return new object[] { new Size(int.MaxValue, int.MaxValue), new Size(ushort.MaxValue, ushort.MaxValue), 1 };
    }

    [WinFormsTheory]
    [MemberData(nameof(MinimumSize_SetWithHandle_TestData))]
    public void Control_MinimumSize_SetWithHandle_GetReturnsExpected(Size value, Size expectedSize, int expectedLayoutCallCount)
    {
        using Control control = new();
        int layoutCallCount = 0;
        control.Layout += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("Bounds", e.AffectedProperty);
            layoutCallCount++;
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.MinimumSize = value;
        Assert.Equal(value, control.MinimumSize);
        Assert.Equal(expectedSize, control.Size);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.MinimumSize = value;
        Assert.Equal(value, control.MinimumSize);
        Assert.Equal(expectedSize, control.Size);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> MinimumSize_SetWithCustomOldValueWithHandle_TestData()
    {
        yield return new object[] { Size.Empty, new Size(1, 2), 0 };
        yield return new object[] { new Size(0, 1), new Size(1, 2), 0 };
        yield return new object[] { new Size(0, 10), new Size(1, 10), 1 };
        yield return new object[] { new Size(1, 0), new Size(1, 2), 0 };
        yield return new object[] { new Size(10, 0), new Size(10, 2), 1 };
        yield return new object[] { new Size(-1, -2), new Size(1, 2), 0 };
        yield return new object[] { new Size(1, 2), new Size(1, 2), 0 };
        yield return new object[] { new Size(3, 4), new Size(3, 4), 1 };
        yield return new object[] { new Size(ushort.MaxValue - 1, ushort.MaxValue - 1), new Size(ushort.MaxValue - 1, ushort.MaxValue - 1), 1 };
        yield return new object[] { new Size(ushort.MaxValue, ushort.MaxValue), new Size(ushort.MaxValue, ushort.MaxValue), 1 };
        yield return new object[] { new Size(ushort.MaxValue + 1, ushort.MaxValue + 1), new Size(ushort.MaxValue, ushort.MaxValue), 1 };
        yield return new object[] { new Size(int.MaxValue, int.MaxValue), new Size(ushort.MaxValue, ushort.MaxValue), 1 };
    }

    [WinFormsTheory]
    [MemberData(nameof(MinimumSize_SetWithCustomOldValueWithHandle_TestData))]
    public void Control_MinimumSize_SetWithCustomOldValueWithHandle_GetReturnsExpected(Size value, Size expectedSize, int expectedLayoutCallCount)
    {
        using Control control = new()
        {
            MinimumSize = new Size(1, 2)
        };
        int layoutCallCount = 0;
        control.Layout += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("Bounds", e.AffectedProperty);
            layoutCallCount++;
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.MinimumSize = value;
        Assert.Equal(value, control.MinimumSize);
        Assert.Equal(expectedSize, control.Size);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.MinimumSize = value;
        Assert.Equal(value, control.MinimumSize);
        Assert.Equal(expectedSize, control.Size);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> MinimumSize_SetWithSizeWithHandle_TestData()
    {
        yield return new object[] { new Size(2, 3), Size.Empty, new Size(2, 3), 0 };
        yield return new object[] { new Size(2, 3), new Size(0, 1), new Size(2, 3), 0 };
        yield return new object[] { new Size(2, 3), new Size(0, 10), new Size(2, 10), 1 };
        yield return new object[] { new Size(2, 3), new Size(1, 0), new Size(2, 3), 0 };
        yield return new object[] { new Size(2, 3), new Size(10, 0), new Size(10, 3), 1 };
        yield return new object[] { new Size(2, 3), new Size(-1, -2), new Size(2, 3), 0 };
        yield return new object[] { new Size(2, 3), new Size(1, 2), new Size(2, 3), 0 };
        yield return new object[] { new Size(2, 3), new Size(2, 2), new Size(2, 3), 0 };
        yield return new object[] { new Size(2, 3), new Size(1, 3), new Size(2, 3), 0 };
        yield return new object[] { new Size(2, 3), new Size(2, 3), new Size(2, 3), 0 };
        yield return new object[] { new Size(2, 3), new Size(3, 3), new Size(3, 3), 1 };
        yield return new object[] { new Size(2, 3), new Size(2, 4), new Size(2, 4), 1 };
        yield return new object[] { new Size(2, 3), new Size(3, 4), new Size(3, 4), 1 };
        yield return new object[] { new Size(2, 3), new Size(5, 6), new Size(5, 6), 1 };
        yield return new object[] { new Size(2, 3), new Size(ushort.MaxValue - 1, ushort.MaxValue - 1), new Size(ushort.MaxValue - 1, ushort.MaxValue - 1), 1 };
        yield return new object[] { new Size(2, 3), new Size(ushort.MaxValue, ushort.MaxValue), new Size(ushort.MaxValue, ushort.MaxValue), 1 };
        yield return new object[] { new Size(2, 3), new Size(ushort.MaxValue + 1, ushort.MaxValue + 1), new Size(ushort.MaxValue, ushort.MaxValue), 1 };
        yield return new object[] { new Size(2, 3), new Size(int.MaxValue, int.MaxValue), new Size(ushort.MaxValue, ushort.MaxValue), 1 };
    }

    [WinFormsTheory]
    [MemberData(nameof(MinimumSize_SetWithSizeWithHandle_TestData))]
    public void Control_MinimumSize_SetWithSizeWithHandle_GetReturnsExpected(Size size, Size value, Size expectedSize, int expectedLayoutCallCount)
    {
        using Control control = new()
        {
            Size = size
        };
        int layoutCallCount = 0;
        control.Layout += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("Bounds", e.AffectedProperty);
            layoutCallCount++;
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.MinimumSize = value;
        Assert.Equal(value, control.MinimumSize);
        Assert.Equal(expectedSize, control.Size);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.MinimumSize = value;
        Assert.Equal(value, control.MinimumSize);
        Assert.Equal(expectedSize, control.Size);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> MinimumSize_SetWithParentWithHandle_TestData()
    {
        yield return new object[] { Size.Empty, Size.Empty, 0, 0 };
        yield return new object[] { new Size(0, 1), new Size(0, 1), 1, 1 };
        yield return new object[] { new Size(0, 10), new Size(0, 10), 1, 1 };
        yield return new object[] { new Size(1, 0), new Size(1, 0), 1, 1 };
        yield return new object[] { new Size(10, 0), new Size(10, 0), 1, 1 };
        yield return new object[] { new Size(-1, -2), Size.Empty, 0, 1 };
        yield return new object[] { new Size(1, 2), new Size(1, 2), 1, 1 };
        yield return new object[] { new Size(ushort.MaxValue - 1, ushort.MaxValue - 1), new Size(ushort.MaxValue - 1, ushort.MaxValue - 1), 1, 1 };
        yield return new object[] { new Size(ushort.MaxValue, ushort.MaxValue), new Size(ushort.MaxValue, ushort.MaxValue), 1, 1 };
        yield return new object[] { new Size(ushort.MaxValue + 1, ushort.MaxValue + 1), new Size(ushort.MaxValue, ushort.MaxValue), 1, 1 };
        yield return new object[] { new Size(int.MaxValue, int.MaxValue), new Size(ushort.MaxValue, ushort.MaxValue), 1, 1 };
    }

    [WinFormsTheory]
    [MemberData(nameof(MinimumSize_SetWithParentWithHandle_TestData))]
    public void Control_MinimumSize_SetWithParentWithHandle_GetReturnsExpected(Size value, Size expectedSize, int expectedLayoutCallCount, int expectedParentLayoutCallCount)
    {
        using Control parent = new();
        using Control control = new()
        {
            Parent = parent
        };
        int layoutCallCount = 0;
        control.Layout += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("Bounds", e.AffectedProperty);
            layoutCallCount++;
        };
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(parent, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("MinimumSize", e.AffectedProperty);
            parentLayoutCallCount++;
        }

        parent.Layout += parentHandler;
        Assert.NotEqual(IntPtr.Zero, parent.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
        int parentInvalidatedCallCount = 0;
        parent.Invalidated += (sender, e) => parentInvalidatedCallCount++;
        int parentStyleChangedCallCount = 0;
        parent.StyleChanged += (sender, e) => parentStyleChangedCallCount++;
        int parentCreatedCallCount = 0;
        parent.HandleCreated += (sender, e) => parentCreatedCallCount++;

        control.MinimumSize = value;
        Assert.Equal(value, control.MinimumSize);
        Assert.Equal(expectedSize, control.Size);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.True(parent.IsHandleCreated);
        Assert.Equal(0, parentInvalidatedCallCount);
        Assert.Equal(0, parentStyleChangedCallCount);
        Assert.Equal(0, parentCreatedCallCount);

        // Set same.
        control.MinimumSize = value;
        Assert.Equal(value, control.MinimumSize);
        Assert.Equal(expectedSize, control.Size);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.True(parent.IsHandleCreated);
        Assert.Equal(0, parentInvalidatedCallCount);
        Assert.Equal(0, parentStyleChangedCallCount);
        Assert.Equal(0, parentCreatedCallCount);

        parent.Layout -= parentHandler;
    }

    public static IEnumerable<object[]> MinimumSize_SetCustomOldValueWithParentWithHandle_TestData()
    {
        yield return new object[] { Size.Empty, new Size(1, 2), 0, 1 };
        yield return new object[] { new Size(0, 1), new Size(1, 2), 0, 1 };
        yield return new object[] { new Size(0, 10), new Size(1, 10), 1, 1 };
        yield return new object[] { new Size(1, 0), new Size(1, 2), 0, 1 };
        yield return new object[] { new Size(10, 0), new Size(10, 2), 1, 1 };
        yield return new object[] { new Size(-1, -2), new Size(1, 2), 0, 1 };
        yield return new object[] { new Size(1, 2), new Size(1, 2), 0, 0 };
        yield return new object[] { new Size(ushort.MaxValue - 1, ushort.MaxValue - 1), new Size(ushort.MaxValue - 1, ushort.MaxValue - 1), 1, 1 };
        yield return new object[] { new Size(ushort.MaxValue, ushort.MaxValue), new Size(ushort.MaxValue, ushort.MaxValue), 1, 1 };
        yield return new object[] { new Size(ushort.MaxValue + 1, ushort.MaxValue + 1), new Size(ushort.MaxValue, ushort.MaxValue), 1, 1 };
        yield return new object[] { new Size(int.MaxValue, int.MaxValue), new Size(ushort.MaxValue, ushort.MaxValue), 1, 1 };
    }

    [WinFormsTheory]
    [MemberData(nameof(MinimumSize_SetCustomOldValueWithParentWithHandle_TestData))]
    public void Control_MinimumSize_SetWithCustomOldValueWithParentWithHandle_GetReturnsExpected(Size value, Size expectedSize, int expectedLayoutCallCount, int expectedParentLayoutCallCount)
    {
        using Control parent = new();
        using Control control = new()
        {
            MinimumSize = new Size(1, 2),
            Parent = parent
        };
        int layoutCallCount = 0;
        control.Layout += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("Bounds", e.AffectedProperty);
            layoutCallCount++;
        };
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(parent, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("MinimumSize", e.AffectedProperty);
            parentLayoutCallCount++;
        }

        parent.Layout += parentHandler;
        Assert.NotEqual(IntPtr.Zero, parent.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
        int parentInvalidatedCallCount = 0;
        parent.Invalidated += (sender, e) => parentInvalidatedCallCount++;
        int parentStyleChangedCallCount = 0;
        parent.StyleChanged += (sender, e) => parentStyleChangedCallCount++;
        int parentCreatedCallCount = 0;
        parent.HandleCreated += (sender, e) => parentCreatedCallCount++;

        control.MinimumSize = value;
        Assert.Equal(value, control.MinimumSize);
        Assert.Equal(expectedSize, control.Size);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.True(parent.IsHandleCreated);
        Assert.Equal(0, parentInvalidatedCallCount);
        Assert.Equal(0, parentStyleChangedCallCount);
        Assert.Equal(0, parentCreatedCallCount);

        // Set same.
        control.MinimumSize = value;
        Assert.Equal(value, control.MinimumSize);
        Assert.Equal(expectedSize, control.Size);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.True(parent.IsHandleCreated);
        Assert.Equal(0, parentInvalidatedCallCount);
        Assert.Equal(0, parentStyleChangedCallCount);
        Assert.Equal(0, parentCreatedCallCount);

        parent.Layout -= parentHandler;
    }

    [WinFormsFact]
    public void Control_MinimumSize_ResetValue_Success()
    {
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(Control))[nameof(Control.MinimumSize)];
        using Control control = new();
        Assert.False(property.CanResetValue(control));

        control.MinimumSize = new Size(1, 0);
        Assert.Equal(new Size(1, 0), control.MinimumSize);
        Assert.True(property.CanResetValue(control));

        control.MinimumSize = new Size(0, 1);
        Assert.Equal(new Size(0, 1), control.MinimumSize);
        Assert.True(property.CanResetValue(control));

        control.MinimumSize = new Size(1, 2);
        Assert.Equal(new Size(1, 2), control.MinimumSize);
        Assert.True(property.CanResetValue(control));

        property.ResetValue(control);
        Assert.Equal(Size.Empty, control.MinimumSize);
        Assert.False(property.CanResetValue(control));
    }

    [WinFormsFact]
    public void Control_MinimumSize_ShouldSerializeValue_Success()
    {
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(Control))[nameof(Control.MinimumSize)];
        using Control control = new();
        Assert.False(property.ShouldSerializeValue(control));

        control.MinimumSize = new Size(1, 0);
        Assert.Equal(new Size(1, 0), control.MinimumSize);
        Assert.True(property.ShouldSerializeValue(control));

        control.MinimumSize = new Size(0, 1);
        Assert.Equal(new Size(0, 1), control.MinimumSize);
        Assert.True(property.ShouldSerializeValue(control));

        control.MinimumSize = new Size(1, 2);
        Assert.Equal(new Size(1, 2), control.MinimumSize);
        Assert.True(property.ShouldSerializeValue(control));

        property.ResetValue(control);
        Assert.Equal(Size.Empty, control.MinimumSize);
        Assert.False(property.ShouldSerializeValue(control));
    }

    [WinFormsTheory]
    [InlineData(0, 0, 0, 0, 0, MouseButtons.None)]
    [InlineData(1, 2, 3, 4, 5, MouseButtons.None)]
    [InlineData(byte.MaxValue, 0, 0, 0, 0, MouseButtons.Left)]
    [InlineData(0, byte.MaxValue, 0, 0, 0, MouseButtons.Middle)]
    [InlineData(0, 0, byte.MaxValue, 0, 0, MouseButtons.Right)]
    [InlineData(0, 0, 0, byte.MaxValue, 0, MouseButtons.XButton1)]
    [InlineData(0, 0, 0, 0, byte.MaxValue, MouseButtons.XButton2)]
    [InlineData(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, MouseButtons.Left | MouseButtons.Middle | MouseButtons.Right | MouseButtons.XButton1 | MouseButtons.XButton2)]
    public unsafe void MouseButtons_Get_ReturnsExpected(byte lState, byte mState, byte rState, byte xState1, byte xState2, MouseButtons expected)
    {
        using SubControl control = new();

        byte[] keyState = new byte[256];
        fixed (byte* b = keyState)
        {
            Assert.True(PInvoke.GetKeyboardState(b));
            keyState[(int)Keys.LButton] = lState;
            keyState[(int)Keys.MButton] = mState;
            keyState[(int)Keys.RButton] = rState;
            keyState[(int)Keys.XButton1] = xState1;
            keyState[(int)Keys.XButton2] = xState2;
            PInvoke.SetKeyboardState(b);
        }

        Assert.Equal(expected, Control.MouseButtons);
    }

    [WinFormsTheory]
    [NormalizedStringData]
    public void Control_Name_GetWithSite_ReturnsExpected(string siteName, string expected)
    {
        Mock<ISite> mockSite = new(MockBehavior.Strict);
        mockSite
            .Setup(s => s.Container)
            .Returns((IContainer)null);
        mockSite
            .Setup(s => s.GetService(typeof(AmbientProperties)))
            .Returns(null);
        mockSite
            .Setup(s => s.Name)
            .Returns(siteName);
        using Control control = new()
        {
            Site = mockSite.Object
        };
        Assert.Equal(expected, control.Name);
        mockSite.Verify(s => s.Name, Times.Once());

        // Get again.
        Assert.Equal(expected, control.Name);
        mockSite.Verify(s => s.Name, Times.Exactly(2));
    }

    [WinFormsTheory]
    [NormalizedStringData]
    public void Control_Name_Set_GetReturnsExpected(string value, string expected)
    {
        using Control control = new()
        {
            Name = value
        };
        Assert.Equal(expected, control.Name);
        Assert.False(control.IsHandleCreated);

        // Get again.
        control.Name = value;
        Assert.Equal(expected, control.Name);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [NormalizedStringData]
    public void Control_Name_SetWithCustomOldValue_GetReturnsExpected(string value, string expected)
    {
        using Control control = new()
        {
            Name = "oldName"
        };

        control.Name = value;
        Assert.Equal(expected, control.Name);
        Assert.False(control.IsHandleCreated);

        // Get again.
        control.Name = value;
        Assert.Equal(expected, control.Name);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData("name", null, "name", 0)]
    [InlineData("", null, "", 1)]
    [InlineData("", "", "", 1)]
    [InlineData("", "siteName", "siteName", 1)]
    [InlineData(null, null, "", 1)]
    [InlineData(null, "", "", 1)]
    [InlineData(null, "siteName", "siteName", 1)]
    public void Control_Name_SetWithSite_GetReturnsExpected(string value, string siteName, string expected, int expectedSiteCallCount)
    {
        Mock<ISite> mockSite = new(MockBehavior.Strict);
        mockSite
            .Setup(s => s.Container)
            .Returns((IContainer)null);
        mockSite
            .Setup(s => s.GetService(typeof(AmbientProperties)))
            .Returns(null);
        mockSite
            .Setup(s => s.Name)
            .Returns(siteName);
        using Control control = new()
        {
            Site = mockSite.Object,
            Name = value
        };
        Assert.Equal(expected, control.Name);
        Assert.False(control.IsHandleCreated);
        mockSite.Verify(s => s.Name, Times.Exactly(expectedSiteCallCount));

        // Get again.
        Assert.Equal(expected, control.Name);
        Assert.False(control.IsHandleCreated);
        mockSite.Verify(s => s.Name, Times.Exactly(expectedSiteCallCount * 2));
    }

    public static IEnumerable<object[]> Padding_Get_TestData()
    {
        yield return new object[] { new Control(), Padding.Empty };
        yield return new object[] { new NonZeroDefaultPaddingControl(), new Padding(1, 2, 3, 4) };
    }

    [WinFormsTheory]
    [MemberData(nameof(Padding_Get_TestData))]
    public void Control_Padding_GetWithCustomDefaultPadding_ReturnsExpected(Control control, Padding expected)
    {
        Assert.Equal(expected, control.Padding);
    }

    private class NonZeroDefaultPaddingControl : Control
    {
        protected override Padding DefaultPadding => new(1, 2, 3, 4);
    }

    public static IEnumerable<object[]> Padding_Set_TestData()
    {
        yield return new object[] { default(Padding), default(Padding), 0, 0 };
        yield return new object[] { new Padding(1, 2, 3, 4), new Padding(1, 2, 3, 4), 1, 1 };
        yield return new object[] { new Padding(1), new Padding(1), 1, 1 };
        yield return new object[] { new Padding(-1, -2, -3, -4), Padding.Empty, 1, 2 };
    }

    [WinFormsTheory]
    [MemberData(nameof(Padding_Set_TestData))]
    public void Control_Padding_Set_GetReturnsExpected(Padding value, Padding expected, int expectedLayoutCallCount1, int expectedLayoutCallCount2)
    {
        using Control control = new();
        int layoutCallCount = 0;
        control.Layout += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("Padding", e.AffectedProperty);
            layoutCallCount++;
        };

        control.Padding = value;
        Assert.Equal(expected, control.Padding);
        Assert.Equal(expectedLayoutCallCount1, layoutCallCount);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.Padding = value;
        Assert.Equal(expected, control.Padding);
        Assert.Equal(expectedLayoutCallCount2, layoutCallCount);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(Padding_Set_TestData))]
    public void Control_Padding_SetWithParent_GetReturnsExpected(Padding value, Padding expected, int expectedLayoutCallCount1, int expectedLayoutCallCount2)
    {
        using Control parent = new();
        using Control control = new()
        {
            Parent = parent
        };
        int layoutCallCount = 0;
        control.Layout += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("Padding", e.AffectedProperty);
            layoutCallCount++;
        };
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(parent, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("Padding", e.AffectedProperty);
            parentLayoutCallCount++;
        }

        parent.Layout += parentHandler;

        control.Padding = value;
        Assert.Equal(expected, control.Padding);
        Assert.Equal(expectedLayoutCallCount1, layoutCallCount);
        Assert.Equal(expectedLayoutCallCount1, parentLayoutCallCount);
        Assert.False(control.IsHandleCreated);
        Assert.False(parent.IsHandleCreated);

        // Set same.
        control.Padding = value;
        Assert.Equal(expected, control.Padding);
        Assert.Equal(expectedLayoutCallCount2, layoutCallCount);
        Assert.Equal(expectedLayoutCallCount2, parentLayoutCallCount);
        Assert.False(control.IsHandleCreated);
        Assert.False(parent.IsHandleCreated);

        parent.Layout -= parentHandler;
    }

    public static IEnumerable<object[]> Padding_SetWithHandle_TestData()
    {
        yield return new object[] { false, default(Padding), default(Padding), 0, 0, 0, 0 };
        yield return new object[] { false, new Padding(1, 2, 3, 4), new Padding(1, 2, 3, 4), 1, 0, 1, 0 };
        yield return new object[] { false, new Padding(1), new Padding(1), 1, 0, 1, 0 };
        yield return new object[] { false, new Padding(-1, -2, -3, -4), Padding.Empty, 1, 0, 2, 0 };
        yield return new object[] { true, default(Padding), default(Padding), 0, 0, 0, 0 };
        yield return new object[] { true, new Padding(1, 2, 3, 4), new Padding(1, 2, 3, 4), 1, 1, 1, 1 };
        yield return new object[] { true, new Padding(1), new Padding(1), 1, 1, 1, 1 };
        yield return new object[] { true, new Padding(-1, -2, -3, -4), Padding.Empty, 1, 1, 2, 2 };
    }

    [WinFormsTheory]
    [MemberData(nameof(Padding_SetWithHandle_TestData))]
    public void Control_Padding_SetWithHandle_GetReturnsExpected(bool resizeRedraw, Padding value, Padding expected, int expectedLayoutCallCount1, int expectedInvalidatedCallCount1, int expectedLayoutCallCount2, int expectedInvalidatedCallCount2)
    {
        using SubControl control = new();
        control.SetStyle(ControlStyles.ResizeRedraw, resizeRedraw);
        int layoutCallCount = 0;
        control.Layout += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("Padding", e.AffectedProperty);
            layoutCallCount++;
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.Padding = value;
        Assert.Equal(expected, control.Padding);
        Assert.Equal(expectedLayoutCallCount1, layoutCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount1, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.Padding = value;
        Assert.Equal(expected, control.Padding);
        Assert.Equal(expectedLayoutCallCount2, layoutCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount2, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [MemberData(nameof(Padding_SetWithHandle_TestData))]
    public void Control_Padding_SetWithParentWithHandle_GetReturnsExpected(bool resizeRedraw, Padding value, Padding expected, int expectedLayoutCallCount1, int expectedInvalidatedCallCount1, int expectedLayoutCallCount2, int expectedInvalidatedCallCount2)
    {
        using Control parent = new();
        using SubControl control = new()
        {
            Parent = parent
        };
        control.SetStyle(ControlStyles.ResizeRedraw, resizeRedraw);
        int layoutCallCount = 0;
        control.Layout += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("Padding", e.AffectedProperty);
            layoutCallCount++;
        };
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(parent, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("Padding", e.AffectedProperty);
            parentLayoutCallCount++;
        }

        parent.Layout += parentHandler;
        Assert.NotEqual(IntPtr.Zero, parent.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
        int parentInvalidatedCallCount = 0;
        parent.Invalidated += (sender, e) => parentInvalidatedCallCount++;
        int parentStyleChangedCallCount = 0;
        parent.StyleChanged += (sender, e) => parentStyleChangedCallCount++;
        int parentCreatedCallCount = 0;
        parent.HandleCreated += (sender, e) => parentCreatedCallCount++;

        control.Padding = value;
        Assert.Equal(expected, control.Padding);
        Assert.Equal(expectedLayoutCallCount1, layoutCallCount);
        Assert.Equal(expectedLayoutCallCount1, parentLayoutCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount1, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.True(parent.IsHandleCreated);
        Assert.Equal(0, parentInvalidatedCallCount);
        Assert.Equal(0, parentStyleChangedCallCount);
        Assert.Equal(0, parentCreatedCallCount);

        // Set same.
        control.Padding = value;
        Assert.Equal(expected, control.Padding);
        Assert.Equal(expectedLayoutCallCount2, layoutCallCount);
        Assert.Equal(expectedLayoutCallCount2, parentLayoutCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount2, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.True(parent.IsHandleCreated);
        Assert.Equal(0, parentInvalidatedCallCount);
        Assert.Equal(0, parentStyleChangedCallCount);
        Assert.Equal(0, parentCreatedCallCount);

        parent.Layout -= parentHandler;
    }

    [WinFormsFact]
    public void Control_Padding_SetWithHandler_CallsPaddingChanged()
    {
        using Control control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Equal(control, sender);
            Assert.Equal(EventArgs.Empty, e);
            callCount++;
        };
        control.PaddingChanged += handler;

        // Set different.
        Padding padding1 = new(1);
        control.Padding = padding1;
        Assert.Equal(padding1, control.Padding);
        Assert.Equal(1, callCount);

        // Set same.
        control.Padding = padding1;
        Assert.Equal(padding1, control.Padding);
        Assert.Equal(1, callCount);

        // Set different.
        Padding padding2 = new(2);
        control.Padding = padding2;
        Assert.Equal(padding2, control.Padding);
        Assert.Equal(2, callCount);

        // Remove handler.
        control.PaddingChanged -= handler;
        control.Padding = padding1;
        Assert.Equal(padding1, control.Padding);
        Assert.Equal(2, callCount);
    }

    [WinFormsFact]
    public void Control_Padding_ResetValue_Success()
    {
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(Control))[nameof(Control.Padding)];
        using Control control = new();
        Assert.False(property.CanResetValue(control));

        control.Padding = new Padding(1, 2, 3, 4);
        Assert.Equal(new Padding(1, 2, 3, 4), control.Padding);
        Assert.True(property.CanResetValue(control));

        property.ResetValue(control);
        Assert.Equal(Padding.Empty, control.Padding);
        Assert.False(property.CanResetValue(control));
    }

    [WinFormsFact]
    public void Control_Padding_ShouldSerializeValue_Success()
    {
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(Control))[nameof(Control.Padding)];
        using Control control = new();
        Assert.False(property.ShouldSerializeValue(control));

        control.Padding = new Padding(1, 2, 3, 4);
        Assert.Equal(new Padding(1, 2, 3, 4), control.Padding);
        Assert.True(property.ShouldSerializeValue(control));

        property.ResetValue(control);
        Assert.Equal(Padding.Empty, control.Padding);
        Assert.False(property.ShouldSerializeValue(control));
    }

    public static IEnumerable<object[]> Parent_Set_TestData()
    {
        yield return new object[] { null };
        yield return new object[] { new Control() };
        yield return new object[] { new Form() };
    }

    [WinFormsTheory]
    [MemberData(nameof(Parent_Set_TestData))]
    public void Control_Parent_Set_GetReturnsExpected(Control value)
    {
        using Control control = new()
        {
            Parent = value
        };
        Assert.Same(value, control.Parent);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.Parent = value;
        Assert.Same(value, control.Parent);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(Parent_Set_TestData))]
    public void Control_Parent_SetWithNonNullOldParent_GetReturnsExpected(Control value)
    {
        using Control oldParent = new();
        using Control control = new()
        {
            Parent = oldParent
        };

        control.Parent = value;
        Assert.Same(value, control.Parent);
        Assert.Empty(oldParent.Controls);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.Parent = value;
        Assert.Same(value, control.Parent);
        Assert.Empty(oldParent.Controls);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void Control_Parent_SetNonNull_AddsToControls()
    {
        using Control parent = new();
        using Control control = new()
        {
            Parent = parent
        };
        Assert.Same(parent, control.Parent);
        Assert.Same(control, Assert.Single(parent.Controls));
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.Parent = parent;
        Assert.Same(parent, control.Parent);
        Assert.Same(control, Assert.Single(parent.Controls));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(Parent_Set_TestData))]
    public void Control_Parent_SetWithHandle_GetReturnsExpected(Control value)
    {
        using Control control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.Parent = value;
        Assert.Same(value, control.Parent);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.Parent = value;
        Assert.Same(value, control.Parent);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void Control_Parent_SetWithHandler_CallsParentChanged()
    {
        using Control parent = new();
        using Control control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        control.ParentChanged += handler;

        // Set different.
        control.Parent = parent;
        Assert.Same(parent, control.Parent);
        Assert.Equal(1, callCount);

        // Set same.
        control.Parent = parent;
        Assert.Same(parent, control.Parent);
        Assert.Equal(1, callCount);

        // Set null.
        control.Parent = null;
        Assert.Null(control.Parent);
        Assert.Equal(2, callCount);

        // Remove handler.
        control.ParentChanged -= handler;
        control.Parent = parent;
        Assert.Same(parent, control.Parent);
        Assert.Equal(2, callCount);
    }

    [WinFormsFact]
    public void Control_Parent_SetSame_ThrowsArgumentException()
    {
        using Control control = new();
        Assert.Throws<ArgumentException>(() => control.Parent = control);
        Assert.Null(control.Parent);
    }

    [WinFormsFact]
    public void Control_Parent_SetChild_ThrowsArgumentException()
    {
        using Control child = new();
        using Control control = new();
        control.Controls.Add(child);

        Assert.Throws<ArgumentException>(() => control.Parent = child);
        Assert.Null(control.Parent);
    }

    [WinFormsFact]
    public void Control_Parent_SetTopLevel_ThrowsArgumentException()
    {
        using Control parent = new();
        using SubControl control = new();
        control.SetTopLevel(true);

        Assert.Throws<ArgumentException>(() => control.Parent = parent);
        Assert.Null(control.Parent);
    }

    [WinFormsFact]
    public void Control_PreferredSize_GetWithChildrenSimple_ReturnsExpected()
    {
        using Control control = new();
        using Control child = new()
        {
            Size = new Size(16, 20)
        };
        control.Controls.Add(child);
        Assert.Equal(new Size(0, 0), control.PreferredSize);

        // Call again.
        Assert.Equal(new Size(0, 0), control.PreferredSize);
    }

    [WinFormsFact]
    public void Control_PreferredSize_GetWithChildrenAdvanced_ReturnsExpected()
    {
        using BorderedControl control = new()
        {
            Padding = new Padding(1, 2, 3, 4)
        };
        using Control child = new()
        {
            Size = new Size(16, 20)
        };
        control.Controls.Add(child);
        Assert.Equal(new Size(0, 0), control.PreferredSize);

        // Call again.
        Assert.Equal(new Size(0, 0), control.PreferredSize);
    }

    public static IEnumerable<object[]> Region_Set_TestData()
    {
        yield return new object[] { null };
        yield return new object[] { new Region() };
        yield return new object[] { new Region(new Rectangle(1, 2, 3, 4)) };
    }

    [WinFormsTheory]
    [MemberData(nameof(Region_Set_TestData))]
    public void Control_Region_Set_GetReturnsExpected(Region value)
    {
        using Control control = new()
        {
            Region = value
        };
        Assert.Same(value, control.Region);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.Region = value;
        Assert.Same(value, control.Region);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(Region_Set_TestData))]
    public void Control_Region_SetWithNonNullOldValue_GetReturnsExpected(Region value)
    {
        using Region oldValue = new();
        using Control control = new()
        {
            Region = oldValue
        };
        oldValue.MakeEmpty();

        control.Region = value;
        Assert.Same(value, control.Region);
        Assert.Throws<ArgumentException>(oldValue.MakeEmpty);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.Region = value;
        Assert.Same(value, control.Region);
        Assert.Throws<ArgumentException>(oldValue.MakeEmpty);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(Region_Set_TestData))]
    public void Control_Region_SetWithHandle_GetReturnsExpected(Region value)
    {
        using Control control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.Region = value;
        Assert.Same(value, control.Region);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.Region = value;
        Assert.Same(value, control.Region);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [MemberData(nameof(Region_Set_TestData))]
    public void Control_Region_SetWithNonNullOldValueWithHandle_GetReturnsExpected(Region value)
    {
        using Region oldValue = new();
        using Control control = new()
        {
            Region = oldValue
        };
        oldValue.MakeEmpty();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.Region = value;
        Assert.Same(value, control.Region);
        Assert.Throws<ArgumentException>(oldValue.MakeEmpty);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.Region = value;
        Assert.Same(value, control.Region);
        Assert.Throws<ArgumentException>(oldValue.MakeEmpty);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void Control_Region_SetWithHandler_CallsRegionChanged()
    {
        using Control control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        control.RegionChanged += handler;

        // Set different.
        using Region region1 = new();
        control.Region = region1;
        Assert.Same(region1, control.Region);
        Assert.Equal(1, callCount);

        // Set same.
        control.Region = region1;
        Assert.Same(region1, control.Region);
        Assert.Equal(1, callCount);

        // Set different.
        using Region region2 = new();
        control.Region = region2;
        Assert.Same(region2, control.Region);
        Assert.Equal(2, callCount);

        // Set null.
        control.Region = null;
        Assert.False(control.RecreatingHandle);
        Assert.Null(control.Region);
        Assert.Equal(3, callCount);

        // Remove handler.
        control.RegionChanged -= handler;
        control.Region = region1;
        Assert.Same(region1, control.Region);
        Assert.Equal(3, callCount);
    }

    [WinFormsTheory]
    [BoolData]
    public void Control_ResizeRedraw_Get_ReturnsExpected(bool value)
    {
        SubControl control = new();
        control.SetStyle(ControlStyles.ResizeRedraw, value);
        Assert.Equal(value, control.ResizeRedraw);
    }

    [WinFormsTheory]
    [BoolData]
    public void Control_ResizeRedraw_Set_GetReturnsExpected(bool value)
    {
        SubControl control = new()
        {
            ResizeRedraw = value
        };
        Assert.Equal(value, control.ResizeRedraw);
        Assert.Equal(value, control.GetStyle(ControlStyles.ResizeRedraw));

        // Set same.
        control.ResizeRedraw = value;
        Assert.Equal(value, control.ResizeRedraw);
        Assert.Equal(value, control.GetStyle(ControlStyles.ResizeRedraw));

        // Set different.
        control.ResizeRedraw = !value;
        Assert.Equal(!value, control.ResizeRedraw);
        Assert.Equal(!value, control.GetStyle(ControlStyles.ResizeRedraw));
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetRightToLeftTheoryData))]
    public void Control_RightToLeft_Set_GetReturnsExpected(RightToLeft value, RightToLeft expected)
    {
        using Control control = new()
        {
            RightToLeft = value
        };
        Assert.Equal(expected, control.RightToLeft);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.RightToLeft = value;
        Assert.Equal(expected, control.RightToLeft);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetRightToLeftTheoryData))]
    public void Control_RightToLeft_SetWithOldValue_GetReturnsExpected(RightToLeft value, RightToLeft expected)
    {
        using Control control = new()
        {
            RightToLeft = RightToLeft.Yes
        };

        control.RightToLeft = value;
        Assert.Equal(expected, control.RightToLeft);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.RightToLeft = value;
        Assert.Equal(expected, control.RightToLeft);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetRightToLeftTheoryData))]
    public void Control_RightToLeft_SetWithChildren_GetReturnsExpected(RightToLeft value, RightToLeft expected)
    {
        using Control child1 = new();
        using Control child2 = new();
        using Control control = new();
        control.Controls.Add(child1);
        control.Controls.Add(child2);

        control.RightToLeft = value;
        Assert.Equal(expected, control.RightToLeft);
        Assert.Equal(expected, child1.RightToLeft);
        Assert.Equal(expected, child2.RightToLeft);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.RightToLeft = value;
        Assert.Equal(expected, control.RightToLeft);
        Assert.Equal(expected, child1.RightToLeft);
        Assert.Equal(expected, child2.RightToLeft);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetRightToLeftTheoryData))]
    public void Control_RightToLeft_SetWithChildrenWithRightToLeft_GetReturnsExpected(RightToLeft value, RightToLeft expected)
    {
        using Control child1 = new()
        {
            RightToLeft = RightToLeft.Yes
        };
        using Control child2 = new()
        {
            RightToLeft = RightToLeft.No
        };
        using Control control = new();
        control.Controls.Add(child1);
        control.Controls.Add(child2);

        control.RightToLeft = value;
        Assert.Equal(expected, control.RightToLeft);
        Assert.Equal(RightToLeft.Yes, child1.RightToLeft);
        Assert.Equal(RightToLeft.No, child2.RightToLeft);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.RightToLeft = value;
        Assert.Equal(expected, control.RightToLeft);
        Assert.Equal(RightToLeft.Yes, child1.RightToLeft);
        Assert.Equal(RightToLeft.No, child2.RightToLeft);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(RightToLeft.Yes, RightToLeft.Yes, 1)]
    [InlineData(RightToLeft.No, RightToLeft.No, 0)]
    [InlineData(RightToLeft.Inherit, RightToLeft.No, 0)]
    public void Control_RightToLeft_SetWithHandle_GetReturnsExpected(RightToLeft value, RightToLeft expected, int expectedCreatedCallCount)
    {
        using Control control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.RightToLeft = value;
        Assert.Equal(expected, control.RightToLeft);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(expectedCreatedCallCount, createdCallCount);

        // Set same.
        control.RightToLeft = value;
        Assert.Equal(expected, control.RightToLeft);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(expectedCreatedCallCount, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(RightToLeft.Yes, RightToLeft.Yes, 0)]
    [InlineData(RightToLeft.No, RightToLeft.No, 1)]
    [InlineData(RightToLeft.Inherit, RightToLeft.No, 1)]
    public void Control_RightToLeft_SetWithOldValueWithHandle_GetReturnsExpected(RightToLeft value, RightToLeft expected, int expectedCreatedCallCount)
    {
        using Control control = new()
        {
            RightToLeft = RightToLeft.Yes
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.RightToLeft = value;
        Assert.Equal(expected, control.RightToLeft);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(expectedCreatedCallCount, createdCallCount);

        // Set same.
        control.RightToLeft = value;
        Assert.Equal(expected, control.RightToLeft);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(expectedCreatedCallCount, createdCallCount);
    }

    [WinFormsFact]
    public void Control_RightToLeft_SetWithHandler_CallsRightToLeftChanged()
    {
        using Control control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        control.RightToLeftChanged += handler;

        // Set different.
        control.RightToLeft = RightToLeft.Yes;
        Assert.Equal(RightToLeft.Yes, control.RightToLeft);
        Assert.Equal(1, callCount);

        // Set same.
        control.RightToLeft = RightToLeft.Yes;
        Assert.Equal(RightToLeft.Yes, control.RightToLeft);
        Assert.Equal(1, callCount);

        // Set different.
        control.RightToLeft = RightToLeft.Inherit;
        Assert.Equal(RightToLeft.No, control.RightToLeft);
        Assert.Equal(2, callCount);

        // Remove handler.
        control.RightToLeftChanged -= handler;
        control.RightToLeft = RightToLeft.Yes;
        Assert.Equal(RightToLeft.Yes, control.RightToLeft);
        Assert.Equal(2, callCount);
    }

    [WinFormsFact]
    public void Control_RightToLeft_SetWithHandlerInDisposing_DoesNotCallRightToLeftChanged()
    {
        using Control control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);

        int callCount = 0;
        control.RightToLeftChanged += (sender, e) => callCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        int disposedCallCount = 0;
        control.Disposed += (sender, e) =>
        {
            control.RightToLeft = RightToLeft.Yes;
            Assert.Equal(RightToLeft.Yes, control.RightToLeft);
            Assert.Equal(0, callCount);
            Assert.Equal(0, createdCallCount);
            disposedCallCount++;
        };

        control.Dispose();
        Assert.Equal(1, disposedCallCount);
    }

    [WinFormsFact]
    public void Control_RightToLeft_SetWithChildrenWithHandler_CallsRightToLeftChanged()
    {
        using Control child1 = new();
        using Control child2 = new()
        {
            RightToLeft = RightToLeft.Inherit
        };
        using Control control = new();
        control.Controls.Add(child1);
        control.Controls.Add(child2);

        int callCount = 0;
        int childCallCount1 = 0;
        int childCallCount2 = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(callCount, childCallCount1);
            Assert.Equal(childCallCount1, childCallCount2);
            callCount++;
        };
        EventHandler childHandler1 = (sender, e) =>
        {
            Assert.Same(child1, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(callCount - 1, childCallCount1);
            Assert.Equal(childCallCount1, childCallCount2);
            childCallCount1++;
        };
        EventHandler childHandler2 = (sender, e) =>
        {
            Assert.Same(child2, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(callCount, childCallCount1);
            Assert.Equal(childCallCount1 - 1, childCallCount2);
            childCallCount2++;
        };
        control.RightToLeftChanged += handler;
        child1.RightToLeftChanged += childHandler1;
        child2.RightToLeftChanged += childHandler2;

        // Set different.
        control.RightToLeft = RightToLeft.Yes;
        Assert.Equal(RightToLeft.Yes, control.RightToLeft);
        Assert.Equal(RightToLeft.Yes, child1.RightToLeft);
        Assert.Equal(RightToLeft.Yes, child2.RightToLeft);
        Assert.Equal(1, callCount);
        Assert.Equal(1, childCallCount1);
        Assert.Equal(1, childCallCount2);

        // Set same.
        control.RightToLeft = RightToLeft.Yes;
        Assert.Equal(RightToLeft.Yes, control.RightToLeft);
        Assert.Equal(RightToLeft.Yes, child1.RightToLeft);
        Assert.Equal(RightToLeft.Yes, child2.RightToLeft);
        Assert.Equal(1, callCount);
        Assert.Equal(1, childCallCount1);
        Assert.Equal(1, childCallCount2);

        // Set different.
        control.RightToLeft = RightToLeft.Inherit;
        Assert.Equal(RightToLeft.No, control.RightToLeft);
        Assert.Equal(RightToLeft.No, child1.RightToLeft);
        Assert.Equal(RightToLeft.No, child2.RightToLeft);
        Assert.Equal(2, callCount);
        Assert.Equal(2, childCallCount1);
        Assert.Equal(2, childCallCount2);

        // Remove handler.
        control.RightToLeftChanged -= handler;
        child1.RightToLeftChanged -= childHandler1;
        child2.RightToLeftChanged -= childHandler2;
        control.RightToLeft = RightToLeft.Yes;
        Assert.Equal(RightToLeft.Yes, control.RightToLeft);
        Assert.Equal(RightToLeft.Yes, child1.RightToLeft);
        Assert.Equal(RightToLeft.Yes, child2.RightToLeft);
        Assert.Equal(2, callCount);
        Assert.Equal(2, childCallCount1);
        Assert.Equal(2, childCallCount2);
    }

    [WinFormsFact]
    public void Control_RightToLeft_SetWithChildrenWithRightToLeftWithHandler_CallsRightToLeftChanged()
    {
        using Control child1 = new()
        {
            RightToLeft = RightToLeft.Yes
        };
        using Control child2 = new()
        {
            RightToLeft = RightToLeft.No
        };
        using Control control = new();
        control.Controls.Add(child1);
        control.Controls.Add(child2);

        int callCount = 0;
        int childCallCount1 = 0;
        int childCallCount2 = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        EventHandler childHandler1 = (sender, e) =>
        {
            Assert.Same(child1, sender);
            Assert.Same(EventArgs.Empty, e);
            childCallCount1++;
        };
        EventHandler childHandler2 = (sender, e) =>
        {
            Assert.Same(child2, sender);
            Assert.Same(EventArgs.Empty, e);
            childCallCount2++;
        };
        control.RightToLeftChanged += handler;
        child1.RightToLeftChanged += childHandler1;
        child2.RightToLeftChanged += childHandler2;

        // Set different.
        control.RightToLeft = RightToLeft.Yes;
        Assert.Equal(RightToLeft.Yes, control.RightToLeft);
        Assert.Equal(RightToLeft.Yes, child1.RightToLeft);
        Assert.Equal(RightToLeft.No, child2.RightToLeft);
        Assert.Equal(1, callCount);
        Assert.Equal(0, childCallCount1);
        Assert.Equal(0, childCallCount2);

        // Set same.
        control.RightToLeft = RightToLeft.Yes;
        Assert.Equal(RightToLeft.Yes, control.RightToLeft);
        Assert.Equal(RightToLeft.Yes, child1.RightToLeft);
        Assert.Equal(RightToLeft.No, child2.RightToLeft);
        Assert.Equal(1, callCount);
        Assert.Equal(0, childCallCount1);
        Assert.Equal(0, childCallCount2);

        // Set different.
        control.RightToLeft = RightToLeft.Inherit;
        Assert.Equal(RightToLeft.No, control.RightToLeft);
        Assert.Equal(RightToLeft.Yes, child1.RightToLeft);
        Assert.Equal(RightToLeft.No, child2.RightToLeft);
        Assert.Equal(2, callCount);
        Assert.Equal(0, childCallCount1);
        Assert.Equal(0, childCallCount2);

        // Remove handler.
        control.RightToLeftChanged -= handler;
        child1.RightToLeftChanged -= childHandler1;
        child2.RightToLeftChanged -= childHandler2;
        control.RightToLeft = RightToLeft.Yes;
        Assert.Equal(RightToLeft.Yes, control.RightToLeft);
        Assert.Equal(RightToLeft.Yes, child1.RightToLeft);
        Assert.Equal(RightToLeft.No, child2.RightToLeft);
        Assert.Equal(2, callCount);
        Assert.Equal(0, childCallCount1);
        Assert.Equal(0, childCallCount2);
    }

    [WinFormsTheory]
    [InvalidEnumData<RightToLeft>]
    public void Control_RightToLeft_SetInvalid_ThrowsInvalidEnumArgumentException(RightToLeft value)
    {
        using Control control = new();
        Assert.Throws<InvalidEnumArgumentException>("value", () => control.RightToLeft = value);
    }

    [WinFormsFact]
    public void Control_RightToLeft_ResetValue_Success()
    {
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(Control))[nameof(Control.RightToLeft)];
        using Control control = new();
        Assert.False(property.CanResetValue(control));

        control.RightToLeft = RightToLeft.Yes;
        Assert.Equal(RightToLeft.Yes, control.RightToLeft);
        Assert.True(property.CanResetValue(control));

        control.RightToLeft = RightToLeft.No;
        Assert.Equal(RightToLeft.No, control.RightToLeft);
        Assert.True(property.CanResetValue(control));

        control.RightToLeft = RightToLeft.Inherit;
        Assert.Equal(RightToLeft.No, control.RightToLeft);
        Assert.False(property.CanResetValue(control));

        control.RightToLeft = RightToLeft.No;
        Assert.Equal(RightToLeft.No, control.RightToLeft);
        Assert.True(property.CanResetValue(control));

        property.ResetValue(control);
        Assert.Equal(RightToLeft.No, control.RightToLeft);
        Assert.False(property.CanResetValue(control));
    }

    [WinFormsFact]
    public void Control_RightToLeft_ShouldSerializeValue_Success()
    {
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(Control))[nameof(Control.RightToLeft)];
        using Control control = new();
        Assert.False(property.ShouldSerializeValue(control));

        control.RightToLeft = RightToLeft.Yes;
        Assert.Equal(RightToLeft.Yes, control.RightToLeft);
        Assert.True(property.ShouldSerializeValue(control));

        control.RightToLeft = RightToLeft.No;
        Assert.Equal(RightToLeft.No, control.RightToLeft);
        Assert.True(property.ShouldSerializeValue(control));

        control.RightToLeft = RightToLeft.Inherit;
        Assert.Equal(RightToLeft.No, control.RightToLeft);
        Assert.False(property.ShouldSerializeValue(control));

        control.RightToLeft = RightToLeft.No;
        Assert.Equal(RightToLeft.No, control.RightToLeft);
        Assert.True(property.ShouldSerializeValue(control));

        property.ResetValue(control);
        Assert.Equal(RightToLeft.No, control.RightToLeft);
        Assert.False(property.ShouldSerializeValue(control));
    }

    [WinFormsFact]
    public void Control_ShowFocusCues_GetWithHandle_ReturnsExpected()
    {
        using SubControl control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        Assert.Equal(SystemInformation.MenuAccessKeysUnderlined, control.ShowFocusCues);
    }

    public static TheoryData<int, bool> Get_Control_ShowFocusCues_GetWithHandleMessageSent_ReturnsExpected()
    {
        return new TheoryData<int, bool>()
        {
            { (int)PInvoke.UIS_CLEAR | ((int)PInvoke.UISF_ACTIVE << 16), SystemInformation.MenuAccessKeysUnderlined },
            { (int)PInvoke.UIS_CLEAR | ((int)PInvoke.UISF_HIDEACCEL << 16), SystemInformation.MenuAccessKeysUnderlined },
            { (int)PInvoke.UIS_CLEAR | ((int)PInvoke.UISF_HIDEFOCUS << 16), true },
            { (int)PInvoke.UIS_CLEAR | ((int)(PInvoke.UISF_HIDEACCEL | PInvoke.UISF_HIDEFOCUS) << 16), true },
            { (int)PInvoke.UIS_SET | ((int)PInvoke.UISF_ACTIVE << 16), SystemInformation.MenuAccessKeysUnderlined },
            { (int)PInvoke.UIS_SET | ((int)PInvoke.UISF_HIDEACCEL << 16), SystemInformation.MenuAccessKeysUnderlined },
            { (int)PInvoke.UIS_SET | ((int)PInvoke.UISF_HIDEFOCUS << 16), false },
            { (int)PInvoke.UIS_SET | ((int)(PInvoke.UISF_HIDEACCEL | PInvoke.UISF_HIDEFOCUS) << 16), false },
            { (int)PInvoke.UIS_INITIALIZE | ((int)PInvoke.UISF_ACTIVE << 16), SystemInformation.MenuAccessKeysUnderlined },
            { (int)PInvoke.UIS_INITIALIZE | ((int)PInvoke.UISF_HIDEACCEL << 16), SystemInformation.MenuAccessKeysUnderlined },
            { (int)PInvoke.UIS_INITIALIZE | ((int)PInvoke.UISF_HIDEFOCUS << 16), SystemInformation.MenuAccessKeysUnderlined },
            { (int)PInvoke.UIS_INITIALIZE | ((int)(PInvoke.UISF_HIDEACCEL | PInvoke.UISF_HIDEFOCUS) << 16), SystemInformation.MenuAccessKeysUnderlined }
        };
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(ControlTests), nameof(Get_Control_ShowFocusCues_GetWithHandleMessageSent_ReturnsExpected))]
    public void Control_ShowFocusCues_GetWithHandleMessageSent_ReturnsExpected(int wParam, bool expected)
    {
        using SubControl control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        PInvokeCore.SendMessage(control, PInvokeCore.WM_UPDATEUISTATE, (WPARAM)wParam);
        Assert.Equal(expected, control.ShowFocusCues);
    }

    [WinFormsTheory]
    [BoolData]
    public void Control_ShowFocusCues_GetWithSiteWithHandle_ReturnsExpected(bool designMode)
    {
        Mock<ISite> mockSite = new(MockBehavior.Strict);
        mockSite
            .Setup(s => s.Container)
            .Returns((IContainer)null);
        mockSite
            .Setup(s => s.GetService(typeof(AmbientProperties)))
            .Returns(null);
        mockSite
            .Setup(s => s.DesignMode)
            .Returns(designMode);
        using SubControl control = new()
        {
            Site = mockSite.Object
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        Assert.Equal(SystemInformation.MenuAccessKeysUnderlined, control.ShowFocusCues);
    }

    [WinFormsFact]
    public void Control_ShowKeyboardCues_GetWithHandle_ReturnsExpected()
    {
        using SubControl control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        Assert.Equal(SystemInformation.MenuAccessKeysUnderlined, control.ShowKeyboardCues);
    }

    public static TheoryData<int, bool> Get_Control_ShowKeyboardCues_GetWithHandleMessageSent_ReturnsExpected()
    {
        return new TheoryData<int, bool>()
        {
            { (int)PInvoke.UIS_CLEAR | ((int)PInvoke.UISF_ACTIVE << 16), SystemInformation.MenuAccessKeysUnderlined },
            { (int)PInvoke.UIS_CLEAR | ((int)PInvoke.UISF_HIDEACCEL << 16), true },
            { (int)PInvoke.UIS_CLEAR | ((int)PInvoke.UISF_HIDEFOCUS << 16), SystemInformation.MenuAccessKeysUnderlined },
            { (int)PInvoke.UIS_CLEAR | ((int)(PInvoke.UISF_HIDEACCEL | PInvoke.UISF_HIDEFOCUS) << 16), true },
            { (int)PInvoke.UIS_SET | ((int)PInvoke.UISF_ACTIVE << 16), SystemInformation.MenuAccessKeysUnderlined },
            { (int)PInvoke.UIS_SET | ((int)PInvoke.UISF_HIDEACCEL << 16), false },
            { (int)PInvoke.UIS_SET | ((int)PInvoke.UISF_HIDEFOCUS << 16), SystemInformation.MenuAccessKeysUnderlined },
            { (int)PInvoke.UIS_SET | ((int)(PInvoke.UISF_HIDEACCEL | PInvoke.UISF_HIDEFOCUS) << 16), false },
            { (int)PInvoke.UIS_INITIALIZE | ((int)PInvoke.UISF_ACTIVE << 16), SystemInformation.MenuAccessKeysUnderlined },
            { (int)PInvoke.UIS_INITIALIZE | ((int)PInvoke.UISF_HIDEACCEL << 16), SystemInformation.MenuAccessKeysUnderlined },
            { (int)PInvoke.UIS_INITIALIZE | ((int)PInvoke.UISF_HIDEFOCUS << 16), SystemInformation.MenuAccessKeysUnderlined },
            { (int)PInvoke.UIS_INITIALIZE | ((int)(PInvoke.UISF_HIDEACCEL | PInvoke.UISF_HIDEFOCUS) << 16), SystemInformation.MenuAccessKeysUnderlined }
        };
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(ControlTests), nameof(Get_Control_ShowKeyboardCues_GetWithHandleMessageSent_ReturnsExpected))]
    public void Control_ShowKeyboardCues_GetWithHandleMessageSent_ReturnsExpected(int wParam, bool expected)
    {
        using SubControl control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        PInvokeCore.SendMessage(control, PInvokeCore.WM_UPDATEUISTATE, (WPARAM)wParam);
        Assert.Equal(expected, control.ShowKeyboardCues);
    }

    [WinFormsTheory]
    [BoolData]
    public void Control_ShowKeyboardCues_GetWithSiteWithHandle_ReturnsExpected(bool designMode)
    {
        Mock<ISite> mockSite = new(MockBehavior.Strict);
        mockSite
            .Setup(s => s.Container)
            .Returns((IContainer)null);
        mockSite
            .Setup(s => s.GetService(typeof(AmbientProperties)))
            .Returns(null);
        mockSite
            .Setup(s => s.DesignMode)
            .Returns(designMode);
        using SubControl control = new()
        {
            Site = mockSite.Object
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        Assert.Equal(designMode | SystemInformation.MenuAccessKeysUnderlined, control.ShowKeyboardCues);
    }

    public static IEnumerable<object[]> Site_Set_TestData()
    {
        yield return new object[] { null };

        Mock<ISite> mockNullSite = new(MockBehavior.Strict);
        mockNullSite
            .Setup(s => s.Container)
            .Returns((IContainer)null);
        mockNullSite
            .Setup(s => s.GetService(typeof(AmbientProperties)))
            .Returns(null);
        yield return new object[] { mockNullSite.Object };

        Mock<ISite> mockInvalidSite = new(MockBehavior.Strict);
        mockInvalidSite
            .Setup(s => s.Container)
            .Returns((IContainer)null);
        mockInvalidSite
            .Setup(s => s.GetService(typeof(AmbientProperties)))
            .Returns(new object());
        yield return new object[] { mockInvalidSite.Object };

        Mock<ISite> mockSite = new(MockBehavior.Strict);
        mockSite
            .Setup(s => s.Container)
            .Returns((IContainer)null);
        mockSite
            .Setup(s => s.GetService(typeof(AmbientProperties)))
            .Returns(new AmbientProperties());
        yield return new object[] { mockSite.Object };
    }

    [WinFormsTheory]
    [MemberData(nameof(Site_Set_TestData))]
    public void Control_Site_Set_GetReturnsExpected(ISite value)
    {
        using Control control = new()
        {
            Site = value
        };
        Assert.Same(value, control.Site);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.Site = value;
        Assert.Same(value, control.Site);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(Site_Set_TestData))]
    public void Control_Site_SetWithHandle_GetReturnsExpected(ISite value)
    {
        using Control control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.Site = value;
        Assert.Same(value, control.Site);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.Site = value;
        Assert.Same(value, control.Site);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [MemberData(nameof(Site_Set_TestData))]
    public void Control_Site_SetWithNonNullOldValue_GetReturnsExpected(ISite value)
    {
        Mock<ISite> mockSite = new(MockBehavior.Strict);
        mockSite
            .Setup(s => s.Container)
            .Returns((IContainer)null);
        mockSite
            .Setup(s => s.GetService(typeof(AmbientProperties)))
            .Returns(new AmbientProperties());
        using Control control = new()
        {
            Site = mockSite.Object
        };

        control.Site = value;
        Assert.Same(value, control.Site);

        // Set same.
        control.Site = value;
        Assert.Same(value, control.Site);
    }

    [WinFormsFact]
    public void Control_Site_SetWithoutAmbientPropertiesSet_UpdatesProperties()
    {
        Font font1 = SystemFonts.CaptionFont;
        Font font2 = SystemFonts.DialogFont;
        Cursor cursor1 = Cursors.AppStarting;
        Cursor cursor2 = Cursors.Arrow;
        AmbientProperties properties = new()
        {
            BackColor = Color.Blue,
            Cursor = cursor1,
            Font = font1,
            ForeColor = Color.Red
        };
        Mock<ISite> mockSite1 = new(MockBehavior.Strict);
        mockSite1
            .Setup(s => s.Container)
            .Returns((IContainer)null);
        mockSite1
            .Setup(s => s.GetService(typeof(AmbientProperties)))
            .Returns(properties)
            .Verifiable();

        AmbientProperties sameProperties = new()
        {
            BackColor = Color.Blue,
            Cursor = cursor1,
            Font = font1,
            ForeColor = Color.Red
        };
        Mock<ISite> mockSite2 = new(MockBehavior.Strict);
        mockSite2
            .Setup(s => s.Container)
            .Returns((IContainer)null);
        mockSite2
            .Setup(s => s.GetService(typeof(AmbientProperties)))
            .Returns(sameProperties)
            .Verifiable();

        AmbientProperties differentProperties = new()
        {
            BackColor = Color.Red,
            Cursor = cursor2,
            Font = font2,
            ForeColor = Color.Blue
        };
        Mock<ISite> mockSite3 = new(MockBehavior.Strict);
        mockSite3
            .Setup(s => s.Container)
            .Returns((IContainer)null);
        mockSite3
            .Setup(s => s.GetService(typeof(AmbientProperties)))
            .Returns(differentProperties)
            .Verifiable();

        Mock<ISite> mockSite4 = new(MockBehavior.Strict);
        mockSite4
            .Setup(s => s.Container)
            .Returns((IContainer)null);
        mockSite4
            .Setup(s => s.GetService(typeof(AmbientProperties)))
            .Returns(null)
            .Verifiable();

        using Control control = new();
        int backColorChangedCallCount = 0;
        control.BackColorChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            backColorChangedCallCount++;
        };
        int cursorChangedCallCount = 0;
        control.CursorChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            cursorChangedCallCount++;
        };
        int foreColorChangedCallCount = 0;
        control.ForeColorChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            foreColorChangedCallCount++;
        };
        int fontChangedCallCount = 0;
        control.FontChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            fontChangedCallCount++;
        };

        control.Site = mockSite1.Object;
        Assert.Same(mockSite1.Object, control.Site);
        Assert.Equal(Color.Blue, control.BackColor);
        Assert.Same(cursor1, control.Cursor);
        Assert.Equal(Color.Red, control.ForeColor);
        Assert.Same(font1, control.Font);
        mockSite1.Verify(s => s.GetService(typeof(AmbientProperties)), Times.Once());
        mockSite2.Verify(s => s.GetService(typeof(AmbientProperties)), Times.Never());
        mockSite3.Verify(s => s.GetService(typeof(AmbientProperties)), Times.Never());
        mockSite4.Verify(s => s.GetService(typeof(AmbientProperties)), Times.Never());
        Assert.Equal(1, backColorChangedCallCount);
        Assert.Equal(0, cursorChangedCallCount);
        Assert.Equal(1, foreColorChangedCallCount);
        Assert.Equal(0, fontChangedCallCount);

        // Set same.
        control.Site = mockSite1.Object;
        Assert.Same(mockSite1.Object, control.Site);
        Assert.Equal(Color.Blue, control.BackColor);
        Assert.Same(cursor1, control.Cursor);
        Assert.Equal(Color.Red, control.ForeColor);
        Assert.Same(font1, control.Font);
        mockSite1.Verify(s => s.GetService(typeof(AmbientProperties)), Times.Exactly(2));
        mockSite2.Verify(s => s.GetService(typeof(AmbientProperties)), Times.Never());
        mockSite3.Verify(s => s.GetService(typeof(AmbientProperties)), Times.Never());
        mockSite4.Verify(s => s.GetService(typeof(AmbientProperties)), Times.Never());
        Assert.Equal(1, backColorChangedCallCount);
        Assert.Equal(0, cursorChangedCallCount);
        Assert.Equal(1, foreColorChangedCallCount);
        Assert.Equal(0, fontChangedCallCount);

        // Set equal.
        control.Site = mockSite2.Object;
        Assert.Same(mockSite2.Object, control.Site);
        Assert.Equal(Color.Blue, control.BackColor);
        Assert.Same(cursor1, control.Cursor);
        Assert.Equal(Color.Red, control.ForeColor);
        Assert.Same(font1, control.Font);
        mockSite1.Verify(s => s.GetService(typeof(AmbientProperties)), Times.Exactly(2));
        mockSite2.Verify(s => s.GetService(typeof(AmbientProperties)), Times.Exactly(1));
        mockSite3.Verify(s => s.GetService(typeof(AmbientProperties)), Times.Never());
        mockSite4.Verify(s => s.GetService(typeof(AmbientProperties)), Times.Never());
        Assert.Equal(1, backColorChangedCallCount);
        Assert.Equal(1, cursorChangedCallCount);
        Assert.Equal(1, foreColorChangedCallCount);
        Assert.Equal(0, fontChangedCallCount);

        // Set different.
        control.Site = mockSite3.Object;
        Assert.Same(mockSite3.Object, control.Site);
        Assert.Equal(Color.Red, control.BackColor);
        Assert.Same(cursor2, control.Cursor);
        Assert.Equal(Color.Blue, control.ForeColor);
        Assert.Same(font2, control.Font);
        mockSite1.Verify(s => s.GetService(typeof(AmbientProperties)), Times.Exactly(2));
        mockSite2.Verify(s => s.GetService(typeof(AmbientProperties)), Times.Exactly(1));
        mockSite3.Verify(s => s.GetService(typeof(AmbientProperties)), Times.Exactly(1));
        mockSite4.Verify(s => s.GetService(typeof(AmbientProperties)), Times.Never());
        Assert.Equal(2, backColorChangedCallCount);
        Assert.Equal(1, cursorChangedCallCount);
        Assert.Equal(2, foreColorChangedCallCount);
        Assert.Equal(1, fontChangedCallCount);

        // Set null.
        control.Site = mockSite4.Object;
        Assert.Same(mockSite4.Object, control.Site);
        Assert.Equal(Control.DefaultBackColor, control.BackColor);
        Assert.Same(Cursors.Default, control.Cursor);
        Assert.Equal(Control.DefaultForeColor, control.ForeColor);
        Assert.Equal(Control.DefaultFont, control.Font);
        mockSite1.Verify(s => s.GetService(typeof(AmbientProperties)), Times.Exactly(2));
        mockSite2.Verify(s => s.GetService(typeof(AmbientProperties)), Times.Exactly(1));
        mockSite3.Verify(s => s.GetService(typeof(AmbientProperties)), Times.Exactly(1));
        mockSite4.Verify(s => s.GetService(typeof(AmbientProperties)), Times.Exactly(1));
        Assert.Equal(3, backColorChangedCallCount);
        Assert.Equal(2, cursorChangedCallCount);
        Assert.Equal(3, foreColorChangedCallCount);
        Assert.Equal(2, fontChangedCallCount);
    }

    [WinFormsFact]
    public void Control_Site_SetWithAmbientPropertiesSet_DoesNotUpdate()
    {
        Font font1 = SystemFonts.MenuFont;
        Font font2 = SystemFonts.DialogFont;
        Cursor cursor1 = Cursors.AppStarting;
        Cursor cursor2 = Cursors.Arrow;
        AmbientProperties properties = new()
        {
            BackColor = Color.Blue,
            Cursor = cursor1,
            Font = font1,
            ForeColor = Color.Red
        };
        Mock<ISite> mockSite1 = new(MockBehavior.Strict);
        mockSite1
            .Setup(s => s.Container)
            .Returns((IContainer)null);
        mockSite1
            .Setup(s => s.GetService(typeof(AmbientProperties)))
            .Returns(properties)
            .Verifiable();

        AmbientProperties sameProperties = new()
        {
            BackColor = Color.Blue,
            Cursor = cursor1,
            Font = font1,
            ForeColor = Color.Red
        };
        Mock<ISite> mockSite2 = new(MockBehavior.Strict);
        mockSite2
            .Setup(s => s.Container)
            .Returns((IContainer)null);
        mockSite2
            .Setup(s => s.GetService(typeof(AmbientProperties)))
            .Returns(sameProperties)
            .Verifiable();

        AmbientProperties differentProperties = new()
        {
            BackColor = Color.Red,
            Cursor = cursor2,
            Font = font2,
            ForeColor = Color.Blue
        };
        Mock<ISite> mockSite3 = new(MockBehavior.Strict);
        mockSite3
            .Setup(s => s.Container)
            .Returns((IContainer)null);
        mockSite3
            .Setup(s => s.GetService(typeof(AmbientProperties)))
            .Returns(differentProperties)
            .Verifiable();

        Mock<ISite> mockSite4 = new(MockBehavior.Strict);
        mockSite4
            .Setup(s => s.Container)
            .Returns((IContainer)null);
        mockSite4
            .Setup(s => s.GetService(typeof(AmbientProperties)))
            .Returns(null)
            .Verifiable();

        using Cursor controlCursor = new(3);
        Font controlFont = SystemFonts.StatusFont;
        using Control control = new()
        {
            BackColor = Color.Green,
            Cursor = controlCursor,
            ForeColor = Color.Yellow,
            Font = controlFont
        };
        int backColorChangedCallCount = 0;
        control.BackColorChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            backColorChangedCallCount++;
        };
        int cursorChangedCallCount = 0;
        control.CursorChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            cursorChangedCallCount++;
        };
        int foreColorChangedCallCount = 0;
        control.ForeColorChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            foreColorChangedCallCount++;
        };
        int fontChangedCallCount = 0;
        control.FontChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            fontChangedCallCount++;
        };

        control.Site = mockSite1.Object;
        Assert.Same(mockSite1.Object, control.Site);
        Assert.Equal(Color.Green, control.BackColor);
        Assert.Same(controlCursor, control.Cursor);
        Assert.Equal(Color.Yellow, control.ForeColor);
        Assert.Same(controlFont, control.Font);
        mockSite1.Verify(s => s.GetService(typeof(AmbientProperties)), Times.Once());
        mockSite2.Verify(s => s.GetService(typeof(AmbientProperties)), Times.Never());
        mockSite3.Verify(s => s.GetService(typeof(AmbientProperties)), Times.Never());
        mockSite4.Verify(s => s.GetService(typeof(AmbientProperties)), Times.Never());
        Assert.Equal(0, backColorChangedCallCount);
        Assert.Equal(0, cursorChangedCallCount);
        Assert.Equal(0, foreColorChangedCallCount);
        Assert.Equal(0, fontChangedCallCount);

        // Set same.
        control.Site = mockSite1.Object;
        Assert.Same(mockSite1.Object, control.Site);
        Assert.Equal(Color.Green, control.BackColor);
        Assert.Same(controlCursor, control.Cursor);
        Assert.Equal(Color.Yellow, control.ForeColor);
        Assert.Same(controlFont, control.Font);
        mockSite1.Verify(s => s.GetService(typeof(AmbientProperties)), Times.Exactly(2));
        mockSite2.Verify(s => s.GetService(typeof(AmbientProperties)), Times.Never());
        mockSite3.Verify(s => s.GetService(typeof(AmbientProperties)), Times.Never());
        mockSite4.Verify(s => s.GetService(typeof(AmbientProperties)), Times.Never());
        Assert.Equal(0, backColorChangedCallCount);
        Assert.Equal(0, cursorChangedCallCount);
        Assert.Equal(0, foreColorChangedCallCount);
        Assert.Equal(0, fontChangedCallCount);

        // Set equal.
        control.Site = mockSite2.Object;
        Assert.Same(mockSite2.Object, control.Site);
        Assert.Equal(Color.Green, control.BackColor);
        Assert.Same(controlCursor, control.Cursor);
        Assert.Equal(Color.Yellow, control.ForeColor);
        Assert.Same(controlFont, control.Font);
        mockSite1.Verify(s => s.GetService(typeof(AmbientProperties)), Times.Exactly(2));
        mockSite2.Verify(s => s.GetService(typeof(AmbientProperties)), Times.Exactly(1));
        mockSite3.Verify(s => s.GetService(typeof(AmbientProperties)), Times.Never());
        mockSite4.Verify(s => s.GetService(typeof(AmbientProperties)), Times.Never());
        Assert.Equal(0, backColorChangedCallCount);
        Assert.Equal(0, cursorChangedCallCount);
        Assert.Equal(0, foreColorChangedCallCount);
        Assert.Equal(0, fontChangedCallCount);

        // Set different.
        control.Site = mockSite3.Object;
        Assert.Same(mockSite3.Object, control.Site);
        Assert.Equal(Color.Green, control.BackColor);
        Assert.Same(controlCursor, control.Cursor);
        Assert.Equal(Color.Yellow, control.ForeColor);
        Assert.Same(controlFont, control.Font);
        mockSite1.Verify(s => s.GetService(typeof(AmbientProperties)), Times.Exactly(2));
        mockSite2.Verify(s => s.GetService(typeof(AmbientProperties)), Times.Exactly(1));
        mockSite3.Verify(s => s.GetService(typeof(AmbientProperties)), Times.Exactly(1));
        mockSite4.Verify(s => s.GetService(typeof(AmbientProperties)), Times.Never());
        Assert.Equal(0, backColorChangedCallCount);
        Assert.Equal(0, cursorChangedCallCount);
        Assert.Equal(0, foreColorChangedCallCount);
        Assert.Equal(0, fontChangedCallCount);
    }

    public static IEnumerable<object[]> Size_Set_TestData()
    {
        yield return new object[] { new Size(-3, -4), 1 };
        yield return new object[] { new Size(0, 0), 0 };
        yield return new object[] { new Size(1, 0), 1 };
        yield return new object[] { new Size(0, 2), 1 };
        yield return new object[] { new Size(30, 40), 1 };
    }

    [WinFormsTheory]
    [MemberData(nameof(Size_Set_TestData))]
    public void Control_Size_Set_GetReturnsExpected(Size value, int expectedLayoutCallCount)
    {
        using SubControl control = new();
        int layoutCallCount = 0;
        int resizeCallCount = 0;
        int sizeChangedCallCount = 0;
        int clientSizeChangedCallCount = 0;
        control.Layout += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("Bounds", e.AffectedProperty);
            Assert.Equal(resizeCallCount, layoutCallCount);
            Assert.Equal(sizeChangedCallCount, layoutCallCount);
            Assert.Equal(clientSizeChangedCallCount, layoutCallCount);
            layoutCallCount++;
        };
        control.Resize += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(layoutCallCount - 1, resizeCallCount);
            Assert.Equal(sizeChangedCallCount, resizeCallCount);
            Assert.Equal(clientSizeChangedCallCount, resizeCallCount);
            resizeCallCount++;
        };
        control.SizeChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(resizeCallCount - 1, sizeChangedCallCount);
            Assert.Equal(layoutCallCount - 1, sizeChangedCallCount);
            Assert.Equal(clientSizeChangedCallCount, sizeChangedCallCount);
            sizeChangedCallCount++;
        };
        control.ClientSizeChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(resizeCallCount - 1, clientSizeChangedCallCount);
            Assert.Equal(layoutCallCount - 1, clientSizeChangedCallCount);
            Assert.Equal(sizeChangedCallCount - 1, clientSizeChangedCallCount);
            clientSizeChangedCallCount++;
        };

        control.Size = value;
        Assert.Equal(value, control.ClientSize);
        Assert.Equal(new Rectangle(0, 0, value.Width, value.Height), control.ClientRectangle);
        Assert.Equal(new Rectangle(0, 0, value.Width, value.Height), control.DisplayRectangle);
        Assert.Equal(value, control.Size);
        Assert.Equal(0, control.Left);
        Assert.Equal(value.Width, control.Right);
        Assert.Equal(0, control.Top);
        Assert.Equal(value.Height, control.Bottom);
        Assert.Equal(value.Width, control.Width);
        Assert.Equal(value.Height, control.Height);
        Assert.Equal(new Rectangle(0, 0, value.Width, value.Height), control.Bounds);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.Equal(expectedLayoutCallCount, resizeCallCount);
        Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
        Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
        Assert.False(control.IsHandleCreated);

        // Call again.
        control.Size = value;
        Assert.Equal(value, control.ClientSize);
        Assert.Equal(new Rectangle(0, 0, value.Width, value.Height), control.ClientRectangle);
        Assert.Equal(new Rectangle(0, 0, value.Width, value.Height), control.DisplayRectangle);
        Assert.Equal(value, control.Size);
        Assert.Equal(0, control.Left);
        Assert.Equal(value.Width, control.Right);
        Assert.Equal(0, control.Top);
        Assert.Equal(value.Height, control.Bottom);
        Assert.Equal(value.Width, control.Width);
        Assert.Equal(value.Height, control.Height);
        Assert.Equal(new Rectangle(0, 0, value.Width, value.Height), control.Bounds);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.Equal(expectedLayoutCallCount, resizeCallCount);
        Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
        Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> Size_Set_WithConstrainedSize_TestData()
    {
        yield return new object[] { Size.Empty, Size.Empty, new Size(30, 40), 30, 40, 1 };
        yield return new object[] { new Size(10, 20), Size.Empty, new Size(30, 40), 30, 40, 1 };
        yield return new object[] { new Size(30, 40), Size.Empty, new Size(30, 40), 30, 40, 0 };
        yield return new object[] { new Size(31, 40), Size.Empty, new Size(30, 40), 31, 40, 0 };
        yield return new object[] { new Size(30, 41), Size.Empty, new Size(30, 40), 30, 41, 0 };
        yield return new object[] { new Size(40, 50), Size.Empty, new Size(30, 40), 40, 50, 0 };
        yield return new object[] { Size.Empty, new Size(20, 10), new Size(30, 40), 20, 10, 1 };
        yield return new object[] { Size.Empty, new Size(30, 40), new Size(30, 40), 30, 40, 1 };
        yield return new object[] { Size.Empty, new Size(31, 40), new Size(30, 40), 30, 40, 1 };
        yield return new object[] { Size.Empty, new Size(30, 41), new Size(30, 40), 30, 40, 1 };
        yield return new object[] { Size.Empty, new Size(40, 50), new Size(30, 40), 30, 40, 1 };
        yield return new object[] { new Size(10, 20), new Size(40, 50), new Size(30, 40), 30, 40, 1 };
        yield return new object[] { new Size(10, 20), new Size(20, 30), new Size(30, 40), 20, 30, 1 };
        yield return new object[] { new Size(10, 20), new Size(20, 30), new Size(30, 40), 20, 30, 1 };
        yield return new object[] { new Size(30, 40), new Size(20, 30), new Size(30, 40), 30, 40, 0 };
        yield return new object[] { new Size(30, 40), new Size(40, 50), new Size(30, 40), 30, 40, 0 };
        yield return new object[] { new Size(40, 50), new Size(20, 30), new Size(30, 40), 40, 50, 0 };
        yield return new object[] { new Size(40, 50), new Size(40, 50), new Size(30, 40), 40, 50, 0 };
    }

    [WinFormsTheory]
    [MemberData(nameof(Size_Set_WithConstrainedSize_TestData))]
    public void Control_Size_SetWithConstrainedSize_GetReturnsExpected(Size minimumSize, Size maximumSize, Size value, int expectedWidth, int expectedHeight, int expectedLayoutCallCount)
    {
        using SubControl control = new()
        {
            MinimumSize = minimumSize,
            MaximumSize = maximumSize
        };
        int layoutCallCount = 0;
        int resizeCallCount = 0;
        int sizeChangedCallCount = 0;
        int clientSizeChangedCallCount = 0;
        control.Layout += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("Bounds", e.AffectedProperty);
            Assert.Equal(resizeCallCount, layoutCallCount);
            Assert.Equal(sizeChangedCallCount, layoutCallCount);
            Assert.Equal(clientSizeChangedCallCount, layoutCallCount);
            layoutCallCount++;
        };
        control.Resize += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(layoutCallCount - 1, resizeCallCount);
            Assert.Equal(sizeChangedCallCount, resizeCallCount);
            Assert.Equal(clientSizeChangedCallCount, resizeCallCount);
            resizeCallCount++;
        };
        control.SizeChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(resizeCallCount - 1, sizeChangedCallCount);
            Assert.Equal(layoutCallCount - 1, sizeChangedCallCount);
            Assert.Equal(clientSizeChangedCallCount, sizeChangedCallCount);
            sizeChangedCallCount++;
        };
        control.ClientSizeChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(resizeCallCount - 1, clientSizeChangedCallCount);
            Assert.Equal(layoutCallCount - 1, clientSizeChangedCallCount);
            Assert.Equal(sizeChangedCallCount - 1, clientSizeChangedCallCount);
            clientSizeChangedCallCount++;
        };

        control.Size = value;
        Assert.Equal(new Size(expectedWidth, expectedHeight), control.ClientSize);
        Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.ClientRectangle);
        Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.DisplayRectangle);
        Assert.Equal(new Size(expectedWidth, expectedHeight), control.Size);
        Assert.Equal(0, control.Left);
        Assert.Equal(expectedWidth, control.Right);
        Assert.Equal(0, control.Top);
        Assert.Equal(expectedHeight, control.Bottom);
        Assert.Equal(expectedWidth, control.Width);
        Assert.Equal(expectedHeight, control.Height);
        Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.Bounds);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.Equal(expectedLayoutCallCount, resizeCallCount);
        Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
        Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
        Assert.False(control.IsHandleCreated);

        // Call again.
        control.Size = value;
        Assert.Equal(new Size(expectedWidth, expectedHeight), control.ClientSize);
        Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.ClientRectangle);
        Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.DisplayRectangle);
        Assert.Equal(new Size(expectedWidth, expectedHeight), control.Size);
        Assert.Equal(0, control.Left);
        Assert.Equal(expectedWidth, control.Right);
        Assert.Equal(0, control.Top);
        Assert.Equal(expectedHeight, control.Bottom);
        Assert.Equal(expectedWidth, control.Width);
        Assert.Equal(expectedHeight, control.Height);
        Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.Bounds);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.Equal(expectedLayoutCallCount, resizeCallCount);
        Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
        Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> Size_SetWithCustomStyle_TestData()
    {
        yield return new object[] { new Size(0, 0), 0, 0, 0 };
        yield return new object[] { new Size(-3, -4), -7, -8, 1 };
        yield return new object[] { new Size(1, 0), -3, -4, 1 };
        yield return new object[] { new Size(0, 2), -4, -2, 1 };
        yield return new object[] { new Size(1, 2), -3, -2, 1 };
        yield return new object[] { new Size(30, 40), 26, 36, 1 };
    }

    [WinFormsTheory]
    [MemberData(nameof(Size_SetWithCustomStyle_TestData))]
    public void Control_Size_SetWithCustomStyle_GetReturnsExpected(Size value, int expectedClientWidth, int expectedClientHeight, int expectedLayoutCallCount)
    {
        using BorderedControl control = new();
        int layoutCallCount = 0;
        int resizeCallCount = 0;
        int sizeChangedCallCount = 0;
        int clientSizeChangedCallCount = 0;
        control.Layout += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("Bounds", e.AffectedProperty);
            Assert.Equal(resizeCallCount, layoutCallCount);
            Assert.Equal(sizeChangedCallCount, layoutCallCount);
            Assert.Equal(clientSizeChangedCallCount, layoutCallCount);
            layoutCallCount++;
        };
        control.Resize += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(layoutCallCount - 1, resizeCallCount);
            Assert.Equal(sizeChangedCallCount, resizeCallCount);
            Assert.Equal(clientSizeChangedCallCount, resizeCallCount);
            resizeCallCount++;
        };
        control.SizeChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(resizeCallCount - 1, sizeChangedCallCount);
            Assert.Equal(layoutCallCount - 1, sizeChangedCallCount);
            Assert.Equal(clientSizeChangedCallCount, sizeChangedCallCount);
            sizeChangedCallCount++;
        };
        control.ClientSizeChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(resizeCallCount - 1, clientSizeChangedCallCount);
            Assert.Equal(layoutCallCount - 1, clientSizeChangedCallCount);
            Assert.Equal(sizeChangedCallCount - 1, clientSizeChangedCallCount);
            clientSizeChangedCallCount++;
        };

        control.Size = value;
        Assert.Equal(new Size(expectedClientWidth, expectedClientHeight), control.ClientSize);
        Assert.Equal(new Rectangle(0, 0, expectedClientWidth, expectedClientHeight), control.ClientRectangle);
        Assert.Equal(new Rectangle(0, 0, expectedClientWidth, expectedClientHeight), control.DisplayRectangle);
        Assert.Equal(value, control.Size);
        Assert.Equal(0, control.Left);
        Assert.Equal(value.Width, control.Right);
        Assert.Equal(0, control.Top);
        Assert.Equal(value.Height, control.Bottom);
        Assert.Equal(value.Width, control.Width);
        Assert.Equal(value.Height, control.Height);
        Assert.Equal(new Rectangle(0, 0, value.Width, value.Height), control.Bounds);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.Equal(expectedLayoutCallCount, resizeCallCount);
        Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
        Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
        Assert.False(control.IsHandleCreated);

        // Call again.
        control.Size = value;
        Assert.Equal(new Size(expectedClientWidth, expectedClientHeight), control.ClientSize);
        Assert.Equal(new Rectangle(0, 0, expectedClientWidth, expectedClientHeight), control.ClientRectangle);
        Assert.Equal(new Rectangle(0, 0, expectedClientWidth, expectedClientHeight), control.DisplayRectangle);
        Assert.Equal(value, control.Size);
        Assert.Equal(0, control.Left);
        Assert.Equal(value.Width, control.Right);
        Assert.Equal(0, control.Top);
        Assert.Equal(value.Height, control.Bottom);
        Assert.Equal(value.Width, control.Width);
        Assert.Equal(value.Height, control.Height);
        Assert.Equal(new Rectangle(0, 0, value.Width, value.Height), control.Bounds);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.Equal(expectedLayoutCallCount, resizeCallCount);
        Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
        Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> Size_SetWithParent_TestData()
    {
        yield return new object[] { new Size(0, 0), 0, 0 };
        yield return new object[] { new Size(-3, -4), 1, 2 };
        yield return new object[] { new Size(1, 0), 1, 2 };
        yield return new object[] { new Size(0, 2), 1, 2 };
        yield return new object[] { new Size(1, 2), 1, 2 };
        yield return new object[] { new Size(30, 40), 1, 2 };
    }

    [WinFormsTheory]
    [MemberData(nameof(Size_SetWithParent_TestData))]
    public void Control_Size_SetWithParent_GetReturnsExpected(Size value, int expectedLayoutCallCount, int expectedParentLayoutCallCount)
    {
        using Control parent = new();
        using SubControl control = new()
        {
            Parent = parent
        };
        int layoutCallCount = 0;
        int resizeCallCount = 0;
        int sizeChangedCallCount = 0;
        int clientSizeChangedCallCount = 0;
        int parentLayoutCallCount = 0;
        control.Layout += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("Bounds", e.AffectedProperty);
            Assert.Equal(resizeCallCount, layoutCallCount);
            Assert.Equal(sizeChangedCallCount, layoutCallCount);
            Assert.Equal(clientSizeChangedCallCount, layoutCallCount);
            Assert.Equal(parentLayoutCallCount, layoutCallCount);
            layoutCallCount++;
        };
        control.Resize += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(layoutCallCount - 1, resizeCallCount);
            Assert.Equal(sizeChangedCallCount, resizeCallCount);
            Assert.Equal(clientSizeChangedCallCount, resizeCallCount);
            Assert.Equal(parentLayoutCallCount, resizeCallCount);
            resizeCallCount++;
        };
        control.SizeChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(resizeCallCount - 1, sizeChangedCallCount);
            Assert.Equal(layoutCallCount - 1, sizeChangedCallCount);
            Assert.Equal(clientSizeChangedCallCount, sizeChangedCallCount);
            Assert.Equal(parentLayoutCallCount, sizeChangedCallCount);
            sizeChangedCallCount++;
        };
        control.ClientSizeChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(resizeCallCount - 1, clientSizeChangedCallCount);
            Assert.Equal(layoutCallCount - 1, clientSizeChangedCallCount);
            Assert.Equal(sizeChangedCallCount - 1, clientSizeChangedCallCount);
            Assert.Equal(parentLayoutCallCount, clientSizeChangedCallCount);
            clientSizeChangedCallCount++;
        };
        void parentHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(parent, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("Bounds", e.AffectedProperty);
            parentLayoutCallCount++;
        }

        parent.Layout += parentHandler;

        try
        {
            control.Size = value;
            Assert.Equal(value, control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, value.Width, value.Height), control.ClientRectangle);
            Assert.Equal(new Rectangle(0, 0, value.Width, value.Height), control.DisplayRectangle);
            Assert.Equal(value, control.Size);
            Assert.Equal(0, control.Left);
            Assert.Equal(value.Width, control.Right);
            Assert.Equal(0, control.Top);
            Assert.Equal(value.Height, control.Bottom);
            Assert.Equal(value.Width, control.Width);
            Assert.Equal(value.Height, control.Height);
            Assert.Equal(new Rectangle(0, 0, value.Width, value.Height), control.Bounds);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.Equal(expectedLayoutCallCount, resizeCallCount);
            Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
            Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
            Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
            Assert.False(control.IsHandleCreated);

            // Call again.
            control.Size = value;
            Assert.Equal(value, control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, value.Width, value.Height), control.ClientRectangle);
            Assert.Equal(new Rectangle(0, 0, value.Width, value.Height), control.DisplayRectangle);
            Assert.Equal(value, control.Size);
            Assert.Equal(0, control.Left);
            Assert.Equal(value.Width, control.Right);
            Assert.Equal(0, control.Top);
            Assert.Equal(value.Height, control.Bottom);
            Assert.Equal(value.Width, control.Width);
            Assert.Equal(value.Height, control.Height);
            Assert.Equal(new Rectangle(0, 0, value.Width, value.Height), control.Bounds);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.Equal(expectedLayoutCallCount, resizeCallCount);
            Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
            Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
            Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
            Assert.False(control.IsHandleCreated);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    public static IEnumerable<object[]> Size_SetWithHandle_TestData()
    {
        yield return new object[] { true, new Size(0, 0), 0, 0, 0, 0 };
        yield return new object[] { true, new Size(-3, -4), 0, 0, 0, 0 };
        yield return new object[] { true, new Size(1, 0), 1, 0, 1, 1 };
        yield return new object[] { true, new Size(0, 2), 0, 2, 1, 1 };
        yield return new object[] { true, new Size(1, 2), 1, 2, 1, 1 };
        yield return new object[] { true, new Size(30, 40), 30, 40, 1, 1 };

        yield return new object[] { false, new Size(0, 0), 0, 0, 0, 0 };
        yield return new object[] { false, new Size(-3, -4), 0, 0, 0, 0 };
        yield return new object[] { false, new Size(1, 0), 1, 0, 1, 0 };
        yield return new object[] { false, new Size(0, 2), 0, 2, 1, 0 };
        yield return new object[] { false, new Size(1, 2), 1, 2, 1, 0 };
        yield return new object[] { false, new Size(30, 40), 30, 40, 1, 0 };
    }

    [WinFormsTheory]
    [MemberData(nameof(Size_SetWithHandle_TestData))]
    public void Control_Size_SetWithHandle_GetReturnsExpected(bool resizeRedraw, Size value, int expectedWidth, int expectedHeight, int expectedLayoutCallCount, int expectedInvalidatedCallCount)
    {
        using SubControl control = new();
        control.SetStyle(ControlStyles.ResizeRedraw, resizeRedraw);
        int layoutCallCount = 0;
        int resizeCallCount = 0;
        int sizeChangedCallCount = 0;
        int clientSizeChangedCallCount = 0;
        control.Layout += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("Bounds", e.AffectedProperty);
            Assert.Equal(resizeCallCount, layoutCallCount);
            Assert.Equal(sizeChangedCallCount, layoutCallCount);
            Assert.Equal(clientSizeChangedCallCount, layoutCallCount);
            layoutCallCount++;
        };
        control.Resize += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(layoutCallCount - 1, resizeCallCount);
            Assert.Equal(sizeChangedCallCount, resizeCallCount);
            Assert.Equal(clientSizeChangedCallCount, resizeCallCount);
            resizeCallCount++;
        };
        control.SizeChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(resizeCallCount - 1, sizeChangedCallCount);
            Assert.Equal(layoutCallCount - 1, sizeChangedCallCount);
            Assert.Equal(clientSizeChangedCallCount, sizeChangedCallCount);
            sizeChangedCallCount++;
        };
        control.ClientSizeChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(resizeCallCount - 1, clientSizeChangedCallCount);
            Assert.Equal(layoutCallCount - 1, clientSizeChangedCallCount);
            Assert.Equal(sizeChangedCallCount - 1, clientSizeChangedCallCount);
            clientSizeChangedCallCount++;
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.Size = value;
        Assert.Equal(new Size(expectedWidth, expectedHeight), control.ClientSize);
        Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.ClientRectangle);
        Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.DisplayRectangle);
        Assert.Equal(new Size(expectedWidth, expectedHeight), control.Size);
        Assert.Equal(0, control.Left);
        Assert.Equal(expectedWidth, control.Right);
        Assert.Equal(0, control.Top);
        Assert.Equal(expectedHeight, control.Bottom);
        Assert.Equal(expectedWidth, control.Width);
        Assert.Equal(expectedHeight, control.Height);
        Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.Bounds);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.Equal(expectedLayoutCallCount, resizeCallCount);
        Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
        Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Call again.
        control.Size = value;
        Assert.Equal(new Size(expectedWidth, expectedHeight), control.ClientSize);
        Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.ClientRectangle);
        Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.DisplayRectangle);
        Assert.Equal(new Size(expectedWidth, expectedHeight), control.Size);
        Assert.Equal(0, control.Left);
        Assert.Equal(expectedWidth, control.Right);
        Assert.Equal(0, control.Top);
        Assert.Equal(expectedHeight, control.Bottom);
        Assert.Equal(expectedWidth, control.Width);
        Assert.Equal(expectedHeight, control.Height);
        Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.Bounds);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.Equal(expectedLayoutCallCount, resizeCallCount);
        Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
        Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> Size_SetWithParentHandle_TestData()
    {
        yield return new object[] { true, new Size(0, 0), 0, 0, 0, 0, 0, 0 };
        yield return new object[] { true, new Size(-3, -4), 0, 0, 0, 0, 1, 2 };
        yield return new object[] { true, new Size(1, 0), 1, 0, 1, 1, 2, 2 };
        yield return new object[] { true, new Size(0, 2), 0, 2, 1, 1, 2, 2 };
        yield return new object[] { true, new Size(1, 2), 1, 2, 1, 1, 2, 2 };
        yield return new object[] { true, new Size(30, 40), 30, 40, 1, 1, 2, 2 };

        yield return new object[] { false, new Size(0, 0), 0, 0, 0, 0, 0, 0 };
        yield return new object[] { false, new Size(-3, -4), 0, 0, 0, 0, 1, 2 };
        yield return new object[] { false, new Size(1, 0), 1, 0, 1, 0, 2, 2 };
        yield return new object[] { false, new Size(0, 2), 0, 2, 1, 0, 2, 2 };
        yield return new object[] { false, new Size(1, 2), 1, 2, 1, 0, 2, 2 };
        yield return new object[] { false, new Size(30, 40), 30, 40, 1, 0, 2, 2 };
    }

    [WinFormsTheory]
    [MemberData(nameof(Size_SetWithParentHandle_TestData))]
    public void Control_Size_SetWithParentWithHandle_GetReturnsExpected(bool resizeRedraw, Size value, int expectedWidth, int expectedHeight, int expectedLayoutCallCount, int expectedInvalidatedCallCount, int expectedParentLayoutCallCount1, int expectedParentLayoutCallCount2)
    {
        using Control parent = new();
        using SubControl control = new()
        {
            Parent = parent
        };
        control.SetStyle(ControlStyles.ResizeRedraw, resizeRedraw);
        int layoutCallCount = 0;
        int resizeCallCount = 0;
        int sizeChangedCallCount = 0;
        int clientSizeChangedCallCount = 0;
        int parentLayoutCallCount = 0;
        control.Layout += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("Bounds", e.AffectedProperty);
            Assert.Equal(resizeCallCount, layoutCallCount);
            Assert.Equal(sizeChangedCallCount, layoutCallCount);
            Assert.Equal(clientSizeChangedCallCount, layoutCallCount);
            Assert.Equal(parentLayoutCallCount, layoutCallCount);
            layoutCallCount++;
        };
        control.Resize += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(layoutCallCount - 1, resizeCallCount);
            Assert.Equal(sizeChangedCallCount, resizeCallCount);
            Assert.Equal(clientSizeChangedCallCount, resizeCallCount);
            Assert.Equal(parentLayoutCallCount, resizeCallCount);
            resizeCallCount++;
        };
        control.SizeChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(resizeCallCount - 1, sizeChangedCallCount);
            Assert.Equal(layoutCallCount - 1, sizeChangedCallCount);
            Assert.Equal(clientSizeChangedCallCount, sizeChangedCallCount);
            Assert.Equal(parentLayoutCallCount, sizeChangedCallCount);
            sizeChangedCallCount++;
        };
        control.ClientSizeChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(resizeCallCount - 1, clientSizeChangedCallCount);
            Assert.Equal(layoutCallCount - 1, clientSizeChangedCallCount);
            Assert.Equal(sizeChangedCallCount - 1, clientSizeChangedCallCount);
            Assert.Equal(parentLayoutCallCount, clientSizeChangedCallCount);
            clientSizeChangedCallCount++;
        };
        void parentHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(parent, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("Bounds", e.AffectedProperty);
            parentLayoutCallCount++;
        }

        parent.Layout += parentHandler;
        Assert.NotEqual(IntPtr.Zero, parent.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
        int parentInvalidatedCallCount = 0;
        parent.Invalidated += (sender, e) => parentInvalidatedCallCount++;
        int parentStyleChangedCallCount = 0;
        parent.StyleChanged += (sender, e) => parentStyleChangedCallCount++;
        int parentCreatedCallCount = 0;
        parent.HandleCreated += (sender, e) => parentCreatedCallCount++;

        try
        {
            control.Size = value;
            Assert.Equal(new Size(expectedWidth, expectedHeight), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.ClientRectangle);
            Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.DisplayRectangle);
            Assert.Equal(new Size(expectedWidth, expectedHeight), control.Size);
            Assert.Equal(0, control.Left);
            Assert.Equal(expectedWidth, control.Right);
            Assert.Equal(0, control.Top);
            Assert.Equal(expectedHeight, control.Bottom);
            Assert.Equal(expectedWidth, control.Width);
            Assert.Equal(expectedHeight, control.Height);
            Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.Bounds);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.Equal(expectedParentLayoutCallCount1, parentLayoutCallCount);
            Assert.Equal(expectedLayoutCallCount, resizeCallCount);
            Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
            Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.True(parent.IsHandleCreated);
            Assert.Equal(0, parentInvalidatedCallCount);
            Assert.Equal(0, parentStyleChangedCallCount);
            Assert.Equal(0, parentCreatedCallCount);

            // Call again.
            control.Size = value;
            Assert.Equal(new Size(expectedWidth, expectedHeight), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.ClientRectangle);
            Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.DisplayRectangle);
            Assert.Equal(new Size(expectedWidth, expectedHeight), control.Size);
            Assert.Equal(0, control.Left);
            Assert.Equal(expectedWidth, control.Right);
            Assert.Equal(0, control.Top);
            Assert.Equal(expectedHeight, control.Bottom);
            Assert.Equal(expectedWidth, control.Width);
            Assert.Equal(expectedHeight, control.Height);
            Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.Bounds);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.Equal(expectedParentLayoutCallCount2, parentLayoutCallCount);
            Assert.Equal(expectedLayoutCallCount, resizeCallCount);
            Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
            Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.True(parent.IsHandleCreated);
            Assert.Equal(0, parentInvalidatedCallCount);
            Assert.Equal(0, parentStyleChangedCallCount);
            Assert.Equal(0, parentCreatedCallCount);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    [WinFormsFact]
    public void Control_Size_ResetValue_Success()
    {
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(Control))[nameof(Control.Size)];
        using Control control = new();
        Assert.False(property.CanResetValue(control));

        control.Size = new Size(1, 0);
        Assert.Equal(new Size(1, 0), control.Size);
        Assert.True(property.CanResetValue(control));

        control.Size = new Size(0, 1);
        Assert.Equal(new Size(0, 1), control.Size);
        Assert.True(property.CanResetValue(control));

        control.Size = new Size(1, 2);
        Assert.Equal(new Size(1, 2), control.Size);
        Assert.True(property.CanResetValue(control));

        property.ResetValue(control);
        Assert.Equal(Size.Empty, control.Size);
        Assert.False(property.CanResetValue(control));
    }

    [WinFormsFact]
    public void Control_Size_ShouldSerializeValue_Success()
    {
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(Control))[nameof(Control.Size)];
        using Control control = new();
        Assert.False(property.ShouldSerializeValue(control));

        control.Size = new Size(1, 0);
        Assert.Equal(new Size(1, 0), control.Size);
        Assert.True(property.ShouldSerializeValue(control));

        control.Size = new Size(0, 1);
        Assert.Equal(new Size(0, 1), control.Size);
        Assert.True(property.ShouldSerializeValue(control));

        control.Size = new Size(1, 2);
        Assert.Equal(new Size(1, 2), control.Size);
        Assert.True(property.ShouldSerializeValue(control));

        property.ResetValue(control);
        Assert.Equal(Size.Empty, control.Size);
        Assert.False(property.ShouldSerializeValue(control));
    }

    [WinFormsTheory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    public void Control_TabIndex_Set_GetReturnsExpected(int value)
    {
        using Control control = new()
        {
            TabIndex = value
        };
        Assert.Equal(value, control.TabIndex);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.TabIndex = value;
        Assert.Equal(value, control.TabIndex);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void Control_TabIndex_SetWithHandler_CallsTabIndexChanged()
    {
        using Control control = new()
        {
            TabIndex = 0
        };
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        control.TabIndexChanged += handler;

        // Set different.
        control.TabIndex = 1;
        Assert.Equal(1, control.TabIndex);
        Assert.Equal(1, callCount);

        // Set same.
        control.TabIndex = 1;
        Assert.Equal(1, control.TabIndex);
        Assert.Equal(1, callCount);

        // Set different.
        control.TabIndex = 2;
        Assert.Equal(2, control.TabIndex);
        Assert.Equal(2, callCount);

        // Remove handler.
        control.TabIndexChanged -= handler;
        control.TabIndex = 1;
        Assert.Equal(1, control.TabIndex);
        Assert.Equal(2, callCount);
    }

    [WinFormsFact]
    public void Control_TabIndex_SetNegative_CallsArgumentOutOfRangeException()
    {
        using Control control = new();
        Assert.Throws<ArgumentOutOfRangeException>("value", () => control.TabIndex = -1);
    }

    [WinFormsTheory]
    [BoolData]
    public void Control_TabStop_Set_GetReturnsExpected(bool value)
    {
        using Control control = new()
        {
            TabStop = value
        };
        Assert.Equal(value, control.TabStop);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.TabStop = value;
        Assert.Equal(value, control.TabStop);
        Assert.False(control.IsHandleCreated);

        // Set different.
        control.TabStop = value;
        Assert.Equal(value, control.TabStop);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [BoolData]
    public void Control_TabStop_SetWithHandle_GetReturnsExpected(bool value)
    {
        using Control control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.TabStop = value;
        Assert.Equal(value, control.TabStop);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.TabStop = value;
        Assert.Equal(value, control.TabStop);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set different.
        control.TabStop = value;
        Assert.Equal(value, control.TabStop);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void Control_TabStop_SetWithHandler_CallsTabStopChanged()
    {
        using Control control = new()
        {
            TabStop = true
        };
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        control.TabStopChanged += handler;

        // Set different.
        control.TabStop = false;
        Assert.False(control.TabStop);
        Assert.Equal(1, callCount);

        // Set same.
        control.TabStop = false;
        Assert.False(control.TabStop);
        Assert.Equal(1, callCount);

        // Set different.
        control.TabStop = true;
        Assert.True(control.TabStop);
        Assert.Equal(2, callCount);

        // Remove handler.
        control.TabStopChanged -= handler;
        control.TabStop = false;
        Assert.False(control.TabStop);
        Assert.Equal(2, callCount);
    }

    [WinFormsTheory]
    [NormalizedStringData]
    public void Control_Text_Set_GetReturnsExpected(string value, string expected)
    {
        using Control control = new()
        {
            Text = value
        };
        Assert.Equal(expected, control.Text);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.Text = value;
        Assert.Equal(expected, control.Text);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [NormalizedStringData]
    public void Control_Text_SetWithHandle_GetReturnsExpected(string value, string expected)
    {
        using Control control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.Text = value;
        Assert.Equal(expected, control.Text);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.Text = value;
        Assert.Equal(expected, control.Text);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void Control_Text_SetWithHandler_CallsTextChanged()
    {
        using Control control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Equal(EventArgs.Empty, e);
            callCount++;
        };
        control.TextChanged += handler;

        // Set different.
        control.Text = "text";
        Assert.Equal("text", control.Text);
        Assert.Equal(1, callCount);

        // Set same.
        control.Text = "text";
        Assert.Equal("text", control.Text);
        Assert.Equal(1, callCount);

        // Set different.
        control.Text = null;
        Assert.Empty(control.Text);
        Assert.Equal(2, callCount);

        // Remove handler.
        control.TextChanged -= handler;
        control.Text = "text";
        Assert.Equal("text", control.Text);
        Assert.Equal(2, callCount);
    }

    [WinFormsFact]
    public void Control_Text_ResetValue_Success()
    {
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(Control))[nameof(Control.Text)];
        using Control control = new();
        Assert.False(property.CanResetValue(control));

        // Set null.
        control.Text = null;
        Assert.Empty(control.Text);
        Assert.False(property.CanResetValue(control));

        // Set empty.
        control.Text = string.Empty;
        Assert.Empty(control.Text);
        Assert.False(property.CanResetValue(control));

        // Set custom
        control.Text = "text";
        Assert.Equal("text", control.Text);
        Assert.True(property.CanResetValue(control));

        property.ResetValue(control);
        Assert.Empty(control.Text);
        Assert.False(property.CanResetValue(control));
    }

    [WinFormsFact]
    public void Control_Text_ShouldSerializeValue_Success()
    {
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(Control))[nameof(Control.Text)];
        using Control control = new();
        Assert.False(property.ShouldSerializeValue(control));

        // Set null.
        control.Text = null;
        Assert.Empty(control.Text);
        Assert.False(property.ShouldSerializeValue(control));

        // Set empty.
        control.Text = string.Empty;
        Assert.Empty(control.Text);
        Assert.False(property.ShouldSerializeValue(control));

        // Set custom
        control.Text = "text";
        Assert.Equal("text", control.Text);
        Assert.True(property.ShouldSerializeValue(control));

        property.ResetValue(control);
        Assert.Empty(control.Text);
        Assert.False(property.ShouldSerializeValue(control));
    }

    [WinFormsTheory]
    [InlineData(0, 0)]
    [InlineData(-1, 1)]
    [InlineData(1, 1)]
    public void Control_Top_Set_GetReturnsExpected(int value, int expectedLocationChangedCallCount)
    {
        using SubControl control = new();
        int moveCallCount = 0;
        int locationChangedCallCount = 0;
        int layoutCallCount = 0;
        int resizeCallCount = 0;
        int sizeChangedCallCount = 0;
        int clientSizeChangedCallCount = 0;
        control.Move += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(locationChangedCallCount, moveCallCount);
            moveCallCount++;
        };
        control.LocationChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(moveCallCount - 1, locationChangedCallCount);
            locationChangedCallCount++;
        };
        control.Layout += (sender, e) => layoutCallCount++;
        control.Resize += (sender, e) => resizeCallCount++;
        control.SizeChanged += (sender, e) => sizeChangedCallCount++;
        control.ClientSizeChanged += (sender, e) => clientSizeChangedCallCount++;

        control.Top = value;
        Assert.Equal(new Size(0, 0), control.ClientSize);
        Assert.Equal(new Rectangle(0, 0, 0, 0), control.ClientRectangle);
        Assert.Equal(new Rectangle(0, 0, 0, 0), control.DisplayRectangle);
        Assert.Equal(new Size(0, 0), control.Size);
        Assert.Equal(0, control.Left);
        Assert.Equal(new Point(0, value), control.Location);
        Assert.Equal(value, control.Top);
        Assert.Equal(value, control.Bottom);
        Assert.Equal(0, control.Width);
        Assert.Equal(0, control.Height);
        Assert.Equal(new Rectangle(0, value, 0, 0), control.Bounds);
        Assert.Equal(expectedLocationChangedCallCount, moveCallCount);
        Assert.Equal(expectedLocationChangedCallCount, locationChangedCallCount);
        Assert.Equal(0, layoutCallCount);
        Assert.Equal(0, resizeCallCount);
        Assert.Equal(0, sizeChangedCallCount);
        Assert.Equal(0, clientSizeChangedCallCount);
        Assert.False(control.IsHandleCreated);

        // Call again.
        control.Top = value;
        Assert.Equal(new Size(0, 0), control.ClientSize);
        Assert.Equal(new Rectangle(0, 0, 0, 0), control.ClientRectangle);
        Assert.Equal(new Rectangle(0, 0, 0, 0), control.DisplayRectangle);
        Assert.Equal(new Size(0, 0), control.Size);
        Assert.Equal(0, control.Left);
        Assert.Equal(new Point(0, value), control.Location);
        Assert.Equal(value, control.Top);
        Assert.Equal(value, control.Bottom);
        Assert.Equal(0, control.Width);
        Assert.Equal(0, control.Height);
        Assert.Equal(new Rectangle(0, value, 0, 0), control.Bounds);
        Assert.Equal(expectedLocationChangedCallCount, moveCallCount);
        Assert.Equal(expectedLocationChangedCallCount, locationChangedCallCount);
        Assert.Equal(0, layoutCallCount);
        Assert.Equal(0, resizeCallCount);
        Assert.Equal(0, sizeChangedCallCount);
        Assert.Equal(0, clientSizeChangedCallCount);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(0, 0)]
    [InlineData(-1, 1)]
    [InlineData(1, 1)]
    public void Control_Top_SetWithParent_GetReturnsExpected(int value, int expectedLocationChangedCallCount)
    {
        using Control parent = new();
        using SubControl control = new()
        {
            Parent = parent
        };
        int moveCallCount = 0;
        int locationChangedCallCount = 0;
        int layoutCallCount = 0;
        int resizeCallCount = 0;
        int sizeChangedCallCount = 0;
        int clientSizeChangedCallCount = 0;
        int parentLayoutCallCount = 0;
        control.Move += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(locationChangedCallCount, moveCallCount);
            moveCallCount++;
        };
        control.LocationChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(moveCallCount - 1, locationChangedCallCount);
            locationChangedCallCount++;
        };
        control.Layout += (sender, e) => layoutCallCount++;
        control.Resize += (sender, e) => resizeCallCount++;
        control.SizeChanged += (sender, e) => sizeChangedCallCount++;
        control.ClientSizeChanged += (sender, e) => clientSizeChangedCallCount++;
        void parentHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(parent, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("Bounds", e.AffectedProperty);
            parentLayoutCallCount++;
        }

        parent.Layout += parentHandler;

        try
        {
            control.Top = value;
            Assert.Equal(new Size(0, 0), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, 0, 0), control.ClientRectangle);
            Assert.Equal(new Rectangle(0, 0, 0, 0), control.DisplayRectangle);
            Assert.Equal(new Size(0, 0), control.Size);
            Assert.Equal(0, control.Left);
            Assert.Equal(new Point(0, value), control.Location);
            Assert.Equal(value, control.Top);
            Assert.Equal(value, control.Bottom);
            Assert.Equal(0, control.Width);
            Assert.Equal(0, control.Height);
            Assert.Equal(new Rectangle(0, value, 0, 0), control.Bounds);
            Assert.Equal(expectedLocationChangedCallCount, moveCallCount);
            Assert.Equal(expectedLocationChangedCallCount, locationChangedCallCount);
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(0, resizeCallCount);
            Assert.Equal(0, sizeChangedCallCount);
            Assert.Equal(0, clientSizeChangedCallCount);
            Assert.Equal(expectedLocationChangedCallCount, parentLayoutCallCount);
            Assert.False(control.IsHandleCreated);

            // Call again.
            control.Top = value;
            Assert.Equal(new Size(0, 0), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, 0, 0), control.ClientRectangle);
            Assert.Equal(new Rectangle(0, 0, 0, 0), control.DisplayRectangle);
            Assert.Equal(new Size(0, 0), control.Size);
            Assert.Equal(0, control.Left);
            Assert.Equal(new Point(0, value), control.Location);
            Assert.Equal(value, control.Top);
            Assert.Equal(value, control.Bottom);
            Assert.Equal(0, control.Width);
            Assert.Equal(0, control.Height);
            Assert.Equal(new Rectangle(0, value, 0, 0), control.Bounds);
            Assert.Equal(expectedLocationChangedCallCount, moveCallCount);
            Assert.Equal(expectedLocationChangedCallCount, locationChangedCallCount);
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(0, resizeCallCount);
            Assert.Equal(0, sizeChangedCallCount);
            Assert.Equal(0, clientSizeChangedCallCount);
            Assert.Equal(expectedLocationChangedCallCount, parentLayoutCallCount);
            Assert.False(control.IsHandleCreated);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    public static IEnumerable<object[]> Top_SetWithHandle_TestData()
    {
        foreach (bool resizeRedraw in new bool[] { true, false })
        {
            yield return new object[] { resizeRedraw, 0, 0 };
            yield return new object[] { resizeRedraw, -1, 1 };
            yield return new object[] { resizeRedraw, 1, 1 };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(Top_SetWithHandle_TestData))]
    public void Control_Top_SetWithHandle_GetReturnsExpected(bool resizeRedraw, int value, int expectedLocationChangedCallCount)
    {
        using SubControl control = new();
        control.SetStyle(ControlStyles.ResizeRedraw, resizeRedraw);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        int moveCallCount = 0;
        int locationChangedCallCount = 0;
        int layoutCallCount = 0;
        int resizeCallCount = 0;
        int sizeChangedCallCount = 0;
        int clientSizeChangedCallCount = 0;
        control.Move += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(locationChangedCallCount, moveCallCount);
            moveCallCount++;
        };
        control.LocationChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(moveCallCount - 1, locationChangedCallCount);
            locationChangedCallCount++;
        };
        control.Layout += (sender, e) => layoutCallCount++;
        control.Resize += (sender, e) => resizeCallCount++;
        control.SizeChanged += (sender, e) => sizeChangedCallCount++;
        control.ClientSizeChanged += (sender, e) => clientSizeChangedCallCount++;

        control.Top = value;
        Assert.Equal(new Size(0, 0), control.ClientSize);
        Assert.Equal(new Rectangle(0, 0, 0, 0), control.ClientRectangle);
        Assert.Equal(new Rectangle(0, 0, 0, 0), control.DisplayRectangle);
        Assert.Equal(new Size(0, 0), control.Size);
        Assert.Equal(0, control.Left);
        Assert.Equal(new Point(0, value), control.Location);
        Assert.Equal(value, control.Top);
        Assert.Equal(value, control.Bottom);
        Assert.Equal(0, control.Width);
        Assert.Equal(0, control.Height);
        Assert.Equal(new Rectangle(0, value, 0, 0), control.Bounds);
        Assert.Equal(expectedLocationChangedCallCount, moveCallCount);
        Assert.Equal(expectedLocationChangedCallCount, locationChangedCallCount);
        Assert.Equal(0, layoutCallCount);
        Assert.Equal(0, resizeCallCount);
        Assert.Equal(0, sizeChangedCallCount);
        Assert.Equal(0, clientSizeChangedCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Call again.
        control.Top = value;
        Assert.Equal(new Size(0, 0), control.ClientSize);
        Assert.Equal(new Rectangle(0, 0, 0, 0), control.ClientRectangle);
        Assert.Equal(new Rectangle(0, 0, 0, 0), control.DisplayRectangle);
        Assert.Equal(new Size(0, 0), control.Size);
        Assert.Equal(0, control.Left);
        Assert.Equal(new Point(0, value), control.Location);
        Assert.Equal(value, control.Top);
        Assert.Equal(value, control.Bottom);
        Assert.Equal(0, control.Width);
        Assert.Equal(0, control.Height);
        Assert.Equal(new Rectangle(0, value, 0, 0), control.Bounds);
        Assert.Equal(expectedLocationChangedCallCount, moveCallCount);
        Assert.Equal(expectedLocationChangedCallCount, locationChangedCallCount);
        Assert.Equal(0, layoutCallCount);
        Assert.Equal(0, resizeCallCount);
        Assert.Equal(0, sizeChangedCallCount);
        Assert.Equal(0, clientSizeChangedCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [MemberData(nameof(Top_SetWithHandle_TestData))]
    public void Control_Top_SetWithParentWithHandle_GetReturnsExpected(bool resizeRedraw, int value, int expectedLocationChangedCallCount)
    {
        using Control parent = new();
        using SubControl control = new()
        {
            Parent = parent
        };
        control.SetStyle(ControlStyles.ResizeRedraw, resizeRedraw);
        Assert.NotEqual(IntPtr.Zero, parent.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
        int parentInvalidatedCallCount = 0;
        parent.Invalidated += (sender, e) => parentInvalidatedCallCount++;
        int parentStyleChangedCallCount = 0;
        parent.StyleChanged += (sender, e) => parentStyleChangedCallCount++;
        int parentCreatedCallCount = 0;
        parent.HandleCreated += (sender, e) => parentCreatedCallCount++;

        int moveCallCount = 0;
        int locationChangedCallCount = 0;
        int layoutCallCount = 0;
        int resizeCallCount = 0;
        int sizeChangedCallCount = 0;
        int clientSizeChangedCallCount = 0;
        int parentLayoutCallCount = 0;
        control.Move += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(locationChangedCallCount, moveCallCount);
            moveCallCount++;
        };
        control.LocationChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(moveCallCount - 1, locationChangedCallCount);
            locationChangedCallCount++;
        };
        control.Layout += (sender, e) => layoutCallCount++;
        control.Resize += (sender, e) => resizeCallCount++;
        control.SizeChanged += (sender, e) => sizeChangedCallCount++;
        control.ClientSizeChanged += (sender, e) => clientSizeChangedCallCount++;
        void parentHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(parent, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("Bounds", e.AffectedProperty);
            parentLayoutCallCount++;
        }

        parent.Layout += parentHandler;

        try
        {
            control.Top = value;
            Assert.Equal(new Size(0, 0), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, 0, 0), control.ClientRectangle);
            Assert.Equal(new Rectangle(0, 0, 0, 0), control.DisplayRectangle);
            Assert.Equal(new Size(0, 0), control.Size);
            Assert.Equal(0, control.Left);
            Assert.Equal(new Point(0, value), control.Location);
            Assert.Equal(value, control.Top);
            Assert.Equal(value, control.Bottom);
            Assert.Equal(0, control.Width);
            Assert.Equal(0, control.Height);
            Assert.Equal(new Rectangle(0, value, 0, 0), control.Bounds);
            Assert.Equal(expectedLocationChangedCallCount, moveCallCount);
            Assert.Equal(expectedLocationChangedCallCount, locationChangedCallCount);
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(expectedLocationChangedCallCount, parentLayoutCallCount);
            Assert.Equal(0, resizeCallCount);
            Assert.Equal(0, sizeChangedCallCount);
            Assert.Equal(0, clientSizeChangedCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.True(parent.IsHandleCreated);
            Assert.Equal(0, parentInvalidatedCallCount);
            Assert.Equal(0, parentStyleChangedCallCount);
            Assert.Equal(0, parentCreatedCallCount);

            // Call again.
            control.Top = value;
            Assert.Equal(new Size(0, 0), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, 0, 0), control.ClientRectangle);
            Assert.Equal(new Rectangle(0, 0, 0, 0), control.DisplayRectangle);
            Assert.Equal(new Size(0, 0), control.Size);
            Assert.Equal(0, control.Left);
            Assert.Equal(new Point(0, value), control.Location);
            Assert.Equal(value, control.Top);
            Assert.Equal(value, control.Bottom);
            Assert.Equal(0, control.Width);
            Assert.Equal(0, control.Height);
            Assert.Equal(new Rectangle(0, value, 0, 0), control.Bounds);
            Assert.Equal(expectedLocationChangedCallCount, moveCallCount);
            Assert.Equal(expectedLocationChangedCallCount, locationChangedCallCount);
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(expectedLocationChangedCallCount, parentLayoutCallCount);
            Assert.Equal(0, resizeCallCount);
            Assert.Equal(0, sizeChangedCallCount);
            Assert.Equal(0, clientSizeChangedCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.True(parent.IsHandleCreated);
            Assert.Equal(0, parentInvalidatedCallCount);
            Assert.Equal(0, parentStyleChangedCallCount);
            Assert.Equal(0, parentCreatedCallCount);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    [WinFormsTheory]
    [InlineData(true, 1)]
    [InlineData(false, 0)]
    public void Control_Top_SetWithHandleWithTransparentBackColor_DoesNotCallInvalidate(bool supportsTransparentBackgroundColor, int expectedInvalidatedCallCount)
    {
        using SubControl control = new();
        control.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
        control.BackColor = Color.FromArgb(254, 255, 255, 255);
        control.SetStyle(ControlStyles.SupportsTransparentBackColor, supportsTransparentBackgroundColor);

        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;

        // Set different.
        control.Top = 1;
        Assert.Equal(new Point(0, 1), control.Location);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);

        // Set same.
        control.Top = 1;
        Assert.Equal(new Point(0, 1), control.Location);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);

        // Set different.
        control.Top = 2;
        Assert.Equal(new Point(0, 2), control.Location);
        Assert.Equal(expectedInvalidatedCallCount * 2, invalidatedCallCount);
    }

    [WinFormsFact]
    public void Control_Top_SetWithHandler_CallsLocationChanged()
    {
        using Control control = new();
        int locationChangedCallCount = 0;
        EventHandler locationChangedHandler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            locationChangedCallCount++;
        };
        control.LocationChanged += locationChangedHandler;
        int moveCallCount = 0;
        EventHandler moveHandler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            moveCallCount++;
        };
        control.Move += moveHandler;

        // Set different.
        control.Top = 1;
        Assert.Equal(new Point(0, 1), control.Location);
        Assert.Equal(1, locationChangedCallCount);
        Assert.Equal(1, moveCallCount);

        // Set same.
        control.Top = 1;
        Assert.Equal(new Point(0, 1), control.Location);
        Assert.Equal(1, locationChangedCallCount);
        Assert.Equal(1, moveCallCount);

        // Set different.
        control.Top = 2;
        Assert.Equal(new Point(0, 2), control.Location);
        Assert.Equal(2, locationChangedCallCount);
        Assert.Equal(2, moveCallCount);

        // Remove handler.
        control.LocationChanged -= locationChangedHandler;
        control.Move -= moveHandler;
        control.Top = 1;
        Assert.Equal(new Point(0, 1), control.Location);
        Assert.Equal(2, locationChangedCallCount);
        Assert.Equal(2, moveCallCount);
    }

    [WinFormsFact]
    public void Control_TopLevelControl_GetWithParent_ReturnsNull()
    {
        using Control grandparent = new();
        using Control parent = new()
        {
            Parent = grandparent
        };
        using Control control = new()
        {
            Parent = parent
        };
        Assert.Null(control.TopLevelControl);
        Assert.Null(parent.TopLevelControl);
        Assert.Null(grandparent.TopLevelControl);
    }

    [WinFormsFact]
    public void Control_TopLevelControl_GetWithTopLevelParent_ReturnsExpected()
    {
        using SubControl grandparent = new();
        grandparent.SetTopLevel(true);
        using Control parent = new()
        {
            Parent = grandparent
        };
        using Control control = new()
        {
            Parent = parent
        };
        Assert.Same(grandparent, control.TopLevelControl);
        Assert.Same(grandparent, parent.TopLevelControl);
        Assert.Same(grandparent, grandparent.TopLevelControl);
    }

    [WinFormsTheory]
    [BoolData]
    public void Control_UseWaitCursor_Set_GetReturnsExpected(bool value)
    {
        using Control control = new()
        {
            UseWaitCursor = value
        };
        Assert.Equal(value, control.UseWaitCursor);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.UseWaitCursor = value;
        Assert.Equal(value, control.UseWaitCursor);
        Assert.False(control.IsHandleCreated);

        // Set different.
        control.UseWaitCursor = !value;
        Assert.Equal(!value, control.UseWaitCursor);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [BoolData]
    public void Control_UseWaitCursor_SetWithChildren_GetReturnsExpected(bool value)
    {
        using Control control = new();
        using Control child1 = new();
        using Control child2 = new();
        control.Controls.Add(child1);
        control.Controls.Add(child2);

        control.UseWaitCursor = value;
        Assert.Equal(value, control.UseWaitCursor);
        Assert.Equal(value, child1.UseWaitCursor);
        Assert.Equal(value, child2.UseWaitCursor);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.UseWaitCursor = value;
        Assert.Equal(value, control.UseWaitCursor);
        Assert.Equal(value, child1.UseWaitCursor);
        Assert.Equal(value, child2.UseWaitCursor);
        Assert.False(control.IsHandleCreated);

        // Set different.
        control.UseWaitCursor = !value;
        Assert.Equal(!value, control.UseWaitCursor);
        Assert.Equal(!value, child1.UseWaitCursor);
        Assert.Equal(!value, child2.UseWaitCursor);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [BoolData]
    public void Control_UseWaitCursor_SetWithHandle_GetReturnsExpected(bool value)
    {
        using Control control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.UseWaitCursor = value;
        Assert.Equal(value, control.UseWaitCursor);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.UseWaitCursor = value;
        Assert.Equal(value, control.UseWaitCursor);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set different.
        control.UseWaitCursor = !value;
        Assert.Equal(!value, control.UseWaitCursor);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(true, true, true)]
    [InlineData(true, false, false)]
    [InlineData(false, true, false)]
    [InlineData(false, false, false)]
    public void Control_Visible_GetWithParent_ReturnsExpected(bool parentVisible, bool visible, bool expected)
    {
        using Control parent = new()
        {
            Visible = parentVisible
        };
        using Control item = new()
        {
            Parent = parent
        };
        Assert.Equal(parentVisible, item.Visible);

        // Set custom.
        item.Visible = visible;
        Assert.Equal(expected, item.Visible);
    }

    [WinFormsTheory]
    [BoolData]
    public void Control_Visible_Set_GetReturnsExpected(bool value)
    {
        using Control control = new()
        {
            Visible = value
        };
        Assert.Equal(value, control.Visible);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.Visible = value;
        Assert.Equal(value, control.Visible);
        Assert.False(control.IsHandleCreated);

        // Set different.
        control.Visible = !value;
        Assert.Equal(!value, control.Visible);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [BoolData]
    public void Control_Visible_SetWithParent_GetReturnsExpected(bool value)
    {
        using Control parent = new();
        using Control control = new()
        {
            Parent = parent,
            Visible = value
        };
        Assert.Equal(value, control.Visible);
        Assert.False(control.IsHandleCreated);
        Assert.False(parent.IsHandleCreated);

        // Set same.
        control.Visible = value;
        Assert.Equal(value, control.Visible);
        Assert.False(control.IsHandleCreated);
        Assert.False(parent.IsHandleCreated);

        // Set different.
        control.Visible = !value;
        Assert.Equal(!value, control.Visible);
        Assert.Equal(!value, control.IsHandleCreated);
        Assert.False(parent.IsHandleCreated);
    }

    public static IEnumerable<object[]> Visible_SetWithHandle_TestData()
    {
        foreach (bool userPaint in new bool[] { true, false })
        {
            yield return new object[] { userPaint, true };
            yield return new object[] { userPaint, false };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(Visible_SetWithHandle_TestData))]
    public void Control_Visible_SetWithHandle_GetReturnsExpected(bool userPaint, bool value)
    {
        using SubControl control = new();
        control.SetStyle(ControlStyles.UserPaint, userPaint);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.Visible = value;
        Assert.Equal(value, control.Visible);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.Visible = value;
        Assert.Equal(value, control.Visible);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set different.
        control.Visible = !value;
        Assert.Equal(!value, control.Visible);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [BoolData]
    public void Control_Visible_SetWithParentWithHandle_GetReturnsExpected(bool value)
    {
        using Control parent = new();
        using Control control = new()
        {
            Parent = parent
        };
        Assert.NotEqual(IntPtr.Zero, parent.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
        int parentInvalidatedCallCount = 0;
        parent.Invalidated += (sender, e) => parentInvalidatedCallCount++;
        int parentStyleChangedCallCount = 0;
        parent.StyleChanged += (sender, e) => parentStyleChangedCallCount++;
        int parentCreatedCallCount = 0;
        parent.HandleCreated += (sender, e) => parentCreatedCallCount++;

        control.Visible = value;
        Assert.Equal(value, control.Visible);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.True(parent.IsHandleCreated);
        Assert.Equal(0, parentInvalidatedCallCount);
        Assert.Equal(0, parentStyleChangedCallCount);
        Assert.Equal(0, parentCreatedCallCount);

        // Set same.
        control.Visible = value;
        Assert.Equal(value, control.Visible);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.True(parent.IsHandleCreated);
        Assert.Equal(0, parentInvalidatedCallCount);
        Assert.Equal(0, parentStyleChangedCallCount);
        Assert.Equal(0, parentCreatedCallCount);

        // Set different.
        control.Visible = !value;
        Assert.Equal(!value, control.Visible);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
        Assert.True(parent.IsHandleCreated);
        Assert.Equal(0, parentInvalidatedCallCount);
        Assert.Equal(0, parentStyleChangedCallCount);
        Assert.Equal(0, parentCreatedCallCount);
    }

    [WinFormsFact]
    public void Control_Visible_SetWithHandler_CallsVisibleChanged()
    {
        using Control control = new()
        {
            Visible = true
        };
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        control.VisibleChanged += handler;

        // Set different.
        control.Visible = false;
        Assert.False(control.Visible);
        Assert.Equal(1, callCount);

        // Set same.
        control.Visible = false;
        Assert.False(control.Visible);
        Assert.Equal(1, callCount);

        // Set different.
        control.Visible = true;
        Assert.True(control.Visible);
        Assert.Equal(2, callCount);

        // Remove handler.
        control.VisibleChanged -= handler;
        control.Visible = false;
        Assert.False(control.Visible);
        Assert.Equal(2, callCount);
    }

    [WinFormsFact]
    public void Control_Visible_SetWithChildrenWithHandler_CallsVisibleChanged()
    {
        using Control child1 = new();
        using Control child2 = new();
        using Control control = new()
        {
            Visible = true
        };
        control.Controls.Add(child1);
        control.Controls.Add(child2);

        int callCount = 0;
        int childCallCount1 = 0;
        int childCallCount2 = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        EventHandler childHandler1 = (sender, e) =>
        {
            Assert.Same(child1, sender);
            Assert.Same(EventArgs.Empty, e);
            childCallCount1++;
        };
        EventHandler childHandler2 = (sender, e) =>
        {
            Assert.Same(child2, sender);
            Assert.Same(EventArgs.Empty, e);
            childCallCount2++;
        };
        control.VisibleChanged += handler;
        child1.VisibleChanged += childHandler1;
        child2.VisibleChanged += childHandler2;

        // Set different.
        control.Visible = false;
        Assert.False(control.Visible);
        Assert.False(child1.Visible);
        Assert.False(child2.Visible);
        Assert.Equal(1, callCount);
        Assert.Equal(0, childCallCount1);
        Assert.Equal(0, childCallCount2);

        // Set same.
        control.Visible = false;
        Assert.False(control.Visible);
        Assert.False(child1.Visible);
        Assert.False(child2.Visible);
        Assert.Equal(1, callCount);
        Assert.Equal(0, childCallCount1);
        Assert.Equal(0, childCallCount2);

        // Set different.
        control.Visible = true;
        Assert.True(control.Visible);
        Assert.True(child1.Visible);
        Assert.True(child2.Visible);
        Assert.Equal(2, callCount);
        Assert.Equal(1, childCallCount1);
        Assert.Equal(1, childCallCount2);

        // Remove handler.
        control.VisibleChanged -= handler;
        child1.VisibleChanged -= childHandler1;
        child2.VisibleChanged -= childHandler2;
        control.Visible = false;
        Assert.False(control.Visible);
        Assert.False(child1.Visible);
        Assert.False(child2.Visible);
        Assert.Equal(2, callCount);
        Assert.Equal(1, childCallCount1);
        Assert.Equal(1, childCallCount2);
    }

    [WinFormsFact]
    public void Control_Visible_SetWithChildrenDisabledWithHandler_CallsVisibleChanged()
    {
        using Control child1 = new()
        {
            Visible = false
        };
        using Control child2 = new()
        {
            Visible = false
        };
        using Control control = new()
        {
            Visible = true
        };
        control.Controls.Add(child1);
        control.Controls.Add(child2);

        int callCount = 0;
        int childCallCount1 = 0;
        int childCallCount2 = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        EventHandler childHandler1 = (sender, e) =>
        {
            Assert.Same(child1, sender);
            Assert.Same(EventArgs.Empty, e);
            childCallCount1++;
        };
        EventHandler childHandler2 = (sender, e) =>
        {
            Assert.Same(child2, sender);
            Assert.Same(EventArgs.Empty, e);
            childCallCount2++;
        };
        control.VisibleChanged += handler;
        child1.VisibleChanged += childHandler1;
        child2.VisibleChanged += childHandler2;

        // Set different.
        control.Visible = false;
        Assert.False(control.Visible);
        Assert.False(child1.Visible);
        Assert.False(child2.Visible);
        Assert.Equal(1, callCount);
        Assert.Equal(0, childCallCount1);
        Assert.Equal(0, childCallCount2);

        // Set same.
        control.Visible = false;
        Assert.False(control.Visible);
        Assert.False(child1.Visible);
        Assert.False(child2.Visible);
        Assert.Equal(1, callCount);
        Assert.Equal(0, childCallCount1);
        Assert.Equal(0, childCallCount2);

        // Set different.
        control.Visible = true;
        Assert.True(control.Visible);
        Assert.False(child1.Visible);
        Assert.False(child2.Visible);
        Assert.Equal(2, callCount);
        Assert.Equal(0, childCallCount1);
        Assert.Equal(0, childCallCount2);

        // Remove handler.
        control.VisibleChanged -= handler;
        child1.VisibleChanged -= childHandler1;
        child2.VisibleChanged -= childHandler2;
        control.Visible = false;
        Assert.False(control.Visible);
        Assert.False(child1.Visible);
        Assert.False(child2.Visible);
        Assert.Equal(2, callCount);
        Assert.Equal(0, childCallCount1);
        Assert.Equal(0, childCallCount2);
    }

    [WinFormsFact]
    public void Control_Visible_ResetValue_Success()
    {
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(Control))[nameof(Control.Visible)];
        using Control control = new();
        Assert.False(property.CanResetValue(control));

        control.Visible = false;
        Assert.False(control.Visible);
        Assert.True(property.CanResetValue(control));

        property.ResetValue(control);
        Assert.True(control.Visible);
        Assert.False(property.CanResetValue(control));
    }

    [WinFormsFact]
    public void Control_Visible_ShouldSerializeValue_Success()
    {
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(Control))[nameof(Control.Visible)];
        using Control control = new();
        Assert.False(property.ShouldSerializeValue(control));

        control.Visible = false;
        Assert.False(control.Visible);
        Assert.True(property.ShouldSerializeValue(control));

        property.ResetValue(control);
        Assert.True(control.Visible);
        Assert.False(property.ShouldSerializeValue(control));
    }

    [WinFormsTheory]
    [InlineData(-4, 1)]
    [InlineData(0, 0)]
    [InlineData(2, 1)]
    [InlineData(40, 1)]
    public void Control_Width_Set_GetReturnsExpected(int value, int expectedLayoutCallCount)
    {
        using SubControl control = new();
        int layoutCallCount = 0;
        int resizeCallCount = 0;
        int sizeChangedCallCount = 0;
        int clientSizeChangedCallCount = 0;
        control.Layout += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("Bounds", e.AffectedProperty);
            Assert.Equal(resizeCallCount, layoutCallCount);
            Assert.Equal(sizeChangedCallCount, layoutCallCount);
            Assert.Equal(clientSizeChangedCallCount, layoutCallCount);
            layoutCallCount++;
        };
        control.Resize += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(layoutCallCount - 1, resizeCallCount);
            Assert.Equal(sizeChangedCallCount, resizeCallCount);
            Assert.Equal(clientSizeChangedCallCount, resizeCallCount);
            resizeCallCount++;
        };
        control.SizeChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(resizeCallCount - 1, sizeChangedCallCount);
            Assert.Equal(layoutCallCount - 1, sizeChangedCallCount);
            Assert.Equal(clientSizeChangedCallCount, sizeChangedCallCount);
            sizeChangedCallCount++;
        };
        control.ClientSizeChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(resizeCallCount - 1, clientSizeChangedCallCount);
            Assert.Equal(layoutCallCount - 1, clientSizeChangedCallCount);
            Assert.Equal(sizeChangedCallCount - 1, clientSizeChangedCallCount);
            clientSizeChangedCallCount++;
        };

        control.Width = value;
        Assert.Equal(new Size(value, 0), control.ClientSize);
        Assert.Equal(new Rectangle(0, 0, value, 0), control.ClientRectangle);
        Assert.Equal(new Rectangle(0, 0, value, 0), control.DisplayRectangle);
        Assert.Equal(new Size(value, 0), control.Size);
        Assert.Equal(0, control.Left);
        Assert.Equal(value, control.Right);
        Assert.Equal(0, control.Top);
        Assert.Equal(0, control.Bottom);
        Assert.Equal(value, control.Width);
        Assert.Equal(0, control.Height);
        Assert.Equal(new Rectangle(0, 0, value, 0), control.Bounds);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.Equal(expectedLayoutCallCount, resizeCallCount);
        Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
        Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
        Assert.False(control.IsHandleCreated);

        // Call again.
        control.Width = value;
        Assert.Equal(new Size(value, 0), control.ClientSize);
        Assert.Equal(new Rectangle(0, 0, value, 0), control.ClientRectangle);
        Assert.Equal(new Rectangle(0, 0, value, 0), control.DisplayRectangle);
        Assert.Equal(new Size(value, 0), control.Size);
        Assert.Equal(0, control.Left);
        Assert.Equal(value, control.Right);
        Assert.Equal(0, control.Top);
        Assert.Equal(0, control.Bottom);
        Assert.Equal(value, control.Width);
        Assert.Equal(0, control.Height);
        Assert.Equal(new Rectangle(0, 0, value, 0), control.Bounds);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.Equal(expectedLayoutCallCount, resizeCallCount);
        Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
        Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> Width_Set_WithConstrainedSize_TestData()
    {
        yield return new object[] { Size.Empty, Size.Empty, 30, 30, 0, 1 };
        yield return new object[] { new Size(10, 20), Size.Empty, 30, 30, 20, 1 };
        yield return new object[] { new Size(30, 40), Size.Empty, 30, 30, 40, 0 };
        yield return new object[] { new Size(31, 40), Size.Empty, 30, 31, 40, 0 };
        yield return new object[] { new Size(30, 41), Size.Empty, 30, 30, 41, 0 };
        yield return new object[] { new Size(40, 50), Size.Empty, 30, 40, 50, 0 };
        yield return new object[] { Size.Empty, new Size(20, 10), 30, 20, 0, 1 };
        yield return new object[] { Size.Empty, new Size(30, 40), 30, 30, 0, 1 };
        yield return new object[] { Size.Empty, new Size(31, 40), 30, 30, 0, 1 };
        yield return new object[] { Size.Empty, new Size(30, 41), 30, 30, 0, 1 };
        yield return new object[] { Size.Empty, new Size(40, 50), 30, 30, 0, 1 };
        yield return new object[] { new Size(10, 20), new Size(40, 50), 30, 30, 20, 1 };
        yield return new object[] { new Size(10, 20), new Size(20, 30), 30, 20, 20, 1 };
        yield return new object[] { new Size(30, 40), new Size(20, 30), 30, 30, 40, 0 };
        yield return new object[] { new Size(30, 40), new Size(40, 50), 30, 30, 40, 0 };
        yield return new object[] { new Size(40, 50), new Size(20, 30), 30, 40, 50, 0 };
        yield return new object[] { new Size(40, 50), new Size(40, 50), 30, 40, 50, 0 };
    }

    [WinFormsTheory]
    [MemberData(nameof(Width_Set_WithConstrainedSize_TestData))]
    public void Control_Width_SetWithConstrainedSize_GetReturnsExpected(Size minimumSize, Size maximumSize, int value, int expectedWidth, int expectedHeight, int expectedLayoutCallCount)
    {
        using SubControl control = new()
        {
            MinimumSize = minimumSize,
            MaximumSize = maximumSize
        };
        int layoutCallCount = 0;
        int resizeCallCount = 0;
        int sizeChangedCallCount = 0;
        int clientSizeChangedCallCount = 0;
        control.Layout += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("Bounds", e.AffectedProperty);
            Assert.Equal(resizeCallCount, layoutCallCount);
            Assert.Equal(sizeChangedCallCount, layoutCallCount);
            Assert.Equal(clientSizeChangedCallCount, layoutCallCount);
            layoutCallCount++;
        };
        control.Resize += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(layoutCallCount - 1, resizeCallCount);
            Assert.Equal(sizeChangedCallCount, resizeCallCount);
            Assert.Equal(clientSizeChangedCallCount, resizeCallCount);
            resizeCallCount++;
        };
        control.SizeChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(resizeCallCount - 1, sizeChangedCallCount);
            Assert.Equal(layoutCallCount - 1, sizeChangedCallCount);
            Assert.Equal(clientSizeChangedCallCount, sizeChangedCallCount);
            sizeChangedCallCount++;
        };
        control.ClientSizeChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(resizeCallCount - 1, clientSizeChangedCallCount);
            Assert.Equal(layoutCallCount - 1, clientSizeChangedCallCount);
            Assert.Equal(sizeChangedCallCount - 1, clientSizeChangedCallCount);
            clientSizeChangedCallCount++;
        };

        control.Width = value;
        Assert.Equal(new Size(expectedWidth, expectedHeight), control.ClientSize);
        Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.ClientRectangle);
        Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.DisplayRectangle);
        Assert.Equal(new Size(expectedWidth, expectedHeight), control.Size);
        Assert.Equal(0, control.Left);
        Assert.Equal(expectedWidth, control.Right);
        Assert.Equal(0, control.Top);
        Assert.Equal(expectedHeight, control.Bottom);
        Assert.Equal(expectedWidth, control.Width);
        Assert.Equal(expectedHeight, control.Height);
        Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.Bounds);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.Equal(expectedLayoutCallCount, resizeCallCount);
        Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
        Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
        Assert.False(control.IsHandleCreated);

        // Call again.
        control.Width = value;
        Assert.Equal(new Size(expectedWidth, expectedHeight), control.ClientSize);
        Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.ClientRectangle);
        Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.DisplayRectangle);
        Assert.Equal(new Size(expectedWidth, expectedHeight), control.Size);
        Assert.Equal(0, control.Left);
        Assert.Equal(expectedWidth, control.Right);
        Assert.Equal(0, control.Top);
        Assert.Equal(expectedHeight, control.Bottom);
        Assert.Equal(expectedWidth, control.Width);
        Assert.Equal(expectedHeight, control.Height);
        Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.Bounds);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.Equal(expectedLayoutCallCount, resizeCallCount);
        Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
        Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(-4, -8, -4, 1)]
    [InlineData(0, 0, 0, 0)]
    [InlineData(2, -2, -4, 1)]
    [InlineData(30, 26, -4, 1)]
    public void Control_Width_SetWithCustomStyle_GetReturnsExpected(int value, int expectedClientWidth, int expectedClientHeight, int expectedLayoutCallCount)
    {
        using BorderedControl control = new();
        int layoutCallCount = 0;
        int resizeCallCount = 0;
        int sizeChangedCallCount = 0;
        int clientSizeChangedCallCount = 0;
        control.Layout += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("Bounds", e.AffectedProperty);
            Assert.Equal(resizeCallCount, layoutCallCount);
            Assert.Equal(sizeChangedCallCount, layoutCallCount);
            Assert.Equal(clientSizeChangedCallCount, layoutCallCount);
            layoutCallCount++;
        };
        control.Resize += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(layoutCallCount - 1, resizeCallCount);
            Assert.Equal(sizeChangedCallCount, resizeCallCount);
            Assert.Equal(clientSizeChangedCallCount, resizeCallCount);
            resizeCallCount++;
        };
        control.SizeChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(resizeCallCount - 1, sizeChangedCallCount);
            Assert.Equal(layoutCallCount - 1, sizeChangedCallCount);
            Assert.Equal(clientSizeChangedCallCount, sizeChangedCallCount);
            sizeChangedCallCount++;
        };
        control.ClientSizeChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(resizeCallCount - 1, clientSizeChangedCallCount);
            Assert.Equal(layoutCallCount - 1, clientSizeChangedCallCount);
            Assert.Equal(sizeChangedCallCount - 1, clientSizeChangedCallCount);
            clientSizeChangedCallCount++;
        };

        control.Width = value;
        Assert.Equal(new Size(expectedClientWidth, expectedClientHeight), control.ClientSize);
        Assert.Equal(new Rectangle(0, 0, expectedClientWidth, expectedClientHeight), control.ClientRectangle);
        Assert.Equal(new Rectangle(0, 0, expectedClientWidth, expectedClientHeight), control.DisplayRectangle);
        Assert.Equal(new Size(value, 0), control.Size);
        Assert.Equal(0, control.Left);
        Assert.Equal(value, control.Right);
        Assert.Equal(0, control.Top);
        Assert.Equal(0, control.Bottom);
        Assert.Equal(value, control.Width);
        Assert.Equal(0, control.Height);
        Assert.Equal(new Rectangle(0, 0, value, 0), control.Bounds);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.Equal(expectedLayoutCallCount, resizeCallCount);
        Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
        Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
        Assert.False(control.IsHandleCreated);

        // Call again.
        control.Width = value;
        Assert.Equal(new Size(expectedClientWidth, expectedClientHeight), control.ClientSize);
        Assert.Equal(new Rectangle(0, 0, expectedClientWidth, expectedClientHeight), control.ClientRectangle);
        Assert.Equal(new Rectangle(0, 0, expectedClientWidth, expectedClientHeight), control.DisplayRectangle);
        Assert.Equal(new Size(value, 0), control.Size);
        Assert.Equal(0, control.Left);
        Assert.Equal(value, control.Right);
        Assert.Equal(0, control.Top);
        Assert.Equal(0, control.Bottom);
        Assert.Equal(value, control.Width);
        Assert.Equal(0, control.Height);
        Assert.Equal(new Rectangle(0, 0, value, 0), control.Bounds);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.Equal(expectedLayoutCallCount, resizeCallCount);
        Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
        Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(-4, 1, 2)]
    [InlineData(0, 0, 0)]
    [InlineData(2, 1, 2)]
    [InlineData(40, 1, 2)]
    public void Control_Width_SetWithParent_GetReturnsExpected(int value, int expectedLayoutCallCount, int expectedParentLayoutCallCount)
    {
        using Control parent = new();
        using SubControl control = new()
        {
            Parent = parent
        };
        int layoutCallCount = 0;
        int resizeCallCount = 0;
        int sizeChangedCallCount = 0;
        int clientSizeChangedCallCount = 0;
        int parentLayoutCallCount = 0;
        control.Layout += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("Bounds", e.AffectedProperty);
            Assert.Equal(resizeCallCount, layoutCallCount);
            Assert.Equal(sizeChangedCallCount, layoutCallCount);
            Assert.Equal(clientSizeChangedCallCount, layoutCallCount);
            Assert.Equal(parentLayoutCallCount, layoutCallCount);
            layoutCallCount++;
        };
        control.Resize += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(layoutCallCount - 1, resizeCallCount);
            Assert.Equal(sizeChangedCallCount, resizeCallCount);
            Assert.Equal(clientSizeChangedCallCount, resizeCallCount);
            Assert.Equal(parentLayoutCallCount, resizeCallCount);
            resizeCallCount++;
        };
        control.SizeChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(resizeCallCount - 1, sizeChangedCallCount);
            Assert.Equal(layoutCallCount - 1, sizeChangedCallCount);
            Assert.Equal(clientSizeChangedCallCount, sizeChangedCallCount);
            Assert.Equal(parentLayoutCallCount, sizeChangedCallCount);
            sizeChangedCallCount++;
        };
        control.ClientSizeChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(resizeCallCount - 1, clientSizeChangedCallCount);
            Assert.Equal(layoutCallCount - 1, clientSizeChangedCallCount);
            Assert.Equal(sizeChangedCallCount - 1, clientSizeChangedCallCount);
            Assert.Equal(parentLayoutCallCount, clientSizeChangedCallCount);
            clientSizeChangedCallCount++;
        };
        void parentHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(parent, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("Bounds", e.AffectedProperty);
            parentLayoutCallCount++;
        }

        parent.Layout += parentHandler;

        try
        {
            control.Width = value;
            Assert.Equal(new Size(value, 0), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, value, 0), control.ClientRectangle);
            Assert.Equal(new Rectangle(0, 0, value, 0), control.DisplayRectangle);
            Assert.Equal(new Size(value, 0), control.Size);
            Assert.Equal(0, control.Left);
            Assert.Equal(value, control.Right);
            Assert.Equal(0, control.Top);
            Assert.Equal(0, control.Bottom);
            Assert.Equal(value, control.Width);
            Assert.Equal(0, control.Height);
            Assert.Equal(new Rectangle(0, 0, value, 0), control.Bounds);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.Equal(expectedLayoutCallCount, resizeCallCount);
            Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
            Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
            Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
            Assert.False(control.IsHandleCreated);

            // Call again.
            control.Width = value;
            Assert.Equal(new Size(value, 0), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, value, 0), control.ClientRectangle);
            Assert.Equal(new Rectangle(0, 0, value, 0), control.DisplayRectangle);
            Assert.Equal(new Size(value, 0), control.Size);
            Assert.Equal(0, control.Left);
            Assert.Equal(value, control.Right);
            Assert.Equal(0, control.Top);
            Assert.Equal(0, control.Bottom);
            Assert.Equal(value, control.Width);
            Assert.Equal(0, control.Height);
            Assert.Equal(new Rectangle(0, 0, value, 0), control.Bounds);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.Equal(expectedLayoutCallCount, resizeCallCount);
            Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
            Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
            Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
            Assert.False(control.IsHandleCreated);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    public static IEnumerable<object[]> Width_SetWithHandle_TestData()
    {
        yield return new object[] { true, -4, 0, 0, 0 };
        yield return new object[] { true, 0, 0, 0, 0 };
        yield return new object[] { true, 2, 2, 1, 1 };
        yield return new object[] { true, 30, 30, 1, 1 };
        yield return new object[] { false, -4, 0, 0, 0 };
        yield return new object[] { false, 0, 0, 0, 0 };
        yield return new object[] { false, 2, 2, 1, 0 };
        yield return new object[] { false, 30, 30, 1, 0 };
    }

    [WinFormsTheory]
    [MemberData(nameof(Width_SetWithHandle_TestData))]
    public void Control_Width_SetWithHandle_GetReturnsExpected(bool resizeRedraw, int value, int expectedWidth, int expectedLayoutCallCount, int expectedInvalidatedCallCount)
    {
        using SubControl control = new();
        control.SetStyle(ControlStyles.ResizeRedraw, resizeRedraw);
        int layoutCallCount = 0;
        int resizeCallCount = 0;
        int sizeChangedCallCount = 0;
        int clientSizeChangedCallCount = 0;
        control.Layout += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("Bounds", e.AffectedProperty);
            Assert.Equal(resizeCallCount, layoutCallCount);
            Assert.Equal(sizeChangedCallCount, layoutCallCount);
            Assert.Equal(clientSizeChangedCallCount, layoutCallCount);
            layoutCallCount++;
        };
        control.Resize += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(layoutCallCount - 1, resizeCallCount);
            Assert.Equal(sizeChangedCallCount, resizeCallCount);
            Assert.Equal(clientSizeChangedCallCount, resizeCallCount);
            resizeCallCount++;
        };
        control.SizeChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(resizeCallCount - 1, sizeChangedCallCount);
            Assert.Equal(layoutCallCount - 1, sizeChangedCallCount);
            Assert.Equal(clientSizeChangedCallCount, sizeChangedCallCount);
            sizeChangedCallCount++;
        };
        control.ClientSizeChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(resizeCallCount - 1, clientSizeChangedCallCount);
            Assert.Equal(layoutCallCount - 1, clientSizeChangedCallCount);
            Assert.Equal(sizeChangedCallCount - 1, clientSizeChangedCallCount);
            clientSizeChangedCallCount++;
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.Width = value;
        Assert.Equal(new Size(expectedWidth, 0), control.ClientSize);
        Assert.Equal(new Rectangle(0, 0, expectedWidth, 0), control.ClientRectangle);
        Assert.Equal(new Rectangle(0, 0, expectedWidth, 0), control.DisplayRectangle);
        Assert.Equal(new Size(expectedWidth, 0), control.Size);
        Assert.Equal(0, control.Left);
        Assert.Equal(expectedWidth, control.Right);
        Assert.Equal(0, control.Top);
        Assert.Equal(0, control.Bottom);
        Assert.Equal(expectedWidth, control.Width);
        Assert.Equal(0, control.Height);
        Assert.Equal(new Rectangle(0, 0, expectedWidth, 0), control.Bounds);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.Equal(expectedLayoutCallCount, resizeCallCount);
        Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
        Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Call again.
        control.Width = value;
        Assert.Equal(new Size(expectedWidth, 0), control.ClientSize);
        Assert.Equal(new Rectangle(0, 0, expectedWidth, 0), control.ClientRectangle);
        Assert.Equal(new Rectangle(0, 0, expectedWidth, 0), control.DisplayRectangle);
        Assert.Equal(new Size(expectedWidth, 0), control.Size);
        Assert.Equal(0, control.Left);
        Assert.Equal(expectedWidth, control.Right);
        Assert.Equal(0, control.Top);
        Assert.Equal(0, control.Bottom);
        Assert.Equal(expectedWidth, control.Width);
        Assert.Equal(0, control.Height);
        Assert.Equal(new Rectangle(0, 0, expectedWidth, 0), control.Bounds);
        Assert.Equal(expectedLayoutCallCount, layoutCallCount);
        Assert.Equal(expectedLayoutCallCount, resizeCallCount);
        Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
        Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> Width_SetWithParentWithHandle_TestData()
    {
        yield return new object[] { true, -4, 0, 0, 0, 1, 2 };
        yield return new object[] { true, 0, 0, 0, 0, 0, 0 };
        yield return new object[] { true, 2, 2, 1, 1, 2, 2 };
        yield return new object[] { true, 40, 40, 1, 1, 2, 2 };
        yield return new object[] { false, -4, 0, 0, 0, 1, 2 };
        yield return new object[] { false, 0, 0, 0, 0, 0, 0 };
        yield return new object[] { false, 2, 2, 1, 0, 2, 2 };
        yield return new object[] { false, 40, 40, 1, 0, 2, 2 };
    }

    [WinFormsTheory]
    [MemberData(nameof(Width_SetWithParentWithHandle_TestData))]
    public void Control_Width_SetWithParentWithHandle_GetReturnsExpected(bool resizeRedraw, int value, int expectedWidth, int expectedLayoutCallCount, int expectedInvalidatedCallCount, int expectedParentLayoutCallCount1, int expectedParentLayoutCallCount2)
    {
        using Control parent = new();
        using SubControl control = new()
        {
            Parent = parent
        };
        control.SetStyle(ControlStyles.ResizeRedraw, resizeRedraw);
        int layoutCallCount = 0;
        int resizeCallCount = 0;
        int sizeChangedCallCount = 0;
        int clientSizeChangedCallCount = 0;
        int parentLayoutCallCount = 0;
        control.Layout += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("Bounds", e.AffectedProperty);
            Assert.Equal(resizeCallCount, layoutCallCount);
            Assert.Equal(sizeChangedCallCount, layoutCallCount);
            Assert.Equal(clientSizeChangedCallCount, layoutCallCount);
            Assert.Equal(parentLayoutCallCount, layoutCallCount);
            layoutCallCount++;
        };
        control.Resize += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(layoutCallCount - 1, resizeCallCount);
            Assert.Equal(sizeChangedCallCount, resizeCallCount);
            Assert.Equal(clientSizeChangedCallCount, resizeCallCount);
            Assert.Equal(parentLayoutCallCount, resizeCallCount);
            resizeCallCount++;
        };
        control.SizeChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(resizeCallCount - 1, sizeChangedCallCount);
            Assert.Equal(layoutCallCount - 1, sizeChangedCallCount);
            Assert.Equal(clientSizeChangedCallCount, sizeChangedCallCount);
            Assert.Equal(parentLayoutCallCount, sizeChangedCallCount);
            sizeChangedCallCount++;
        };
        control.ClientSizeChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.Equal(resizeCallCount - 1, clientSizeChangedCallCount);
            Assert.Equal(layoutCallCount - 1, clientSizeChangedCallCount);
            Assert.Equal(sizeChangedCallCount - 1, clientSizeChangedCallCount);
            Assert.Equal(parentLayoutCallCount, clientSizeChangedCallCount);
            clientSizeChangedCallCount++;
        };
        void parentHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(parent, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("Bounds", e.AffectedProperty);
            parentLayoutCallCount++;
        }

        parent.Layout += parentHandler;
        Assert.NotEqual(IntPtr.Zero, parent.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
        int parentInvalidatedCallCount = 0;
        parent.Invalidated += (sender, e) => parentInvalidatedCallCount++;
        int parentStyleChangedCallCount = 0;
        parent.StyleChanged += (sender, e) => parentStyleChangedCallCount++;
        int parentCreatedCallCount = 0;
        parent.HandleCreated += (sender, e) => parentCreatedCallCount++;

        try
        {
            control.Width = value;
            Assert.Equal(new Size(expectedWidth, 0), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, expectedWidth, 0), control.ClientRectangle);
            Assert.Equal(new Rectangle(0, 0, expectedWidth, 0), control.DisplayRectangle);
            Assert.Equal(new Size(expectedWidth, 0), control.Size);
            Assert.Equal(0, control.Left);
            Assert.Equal(expectedWidth, control.Right);
            Assert.Equal(0, control.Top);
            Assert.Equal(0, control.Bottom);
            Assert.Equal(expectedWidth, control.Width);
            Assert.Equal(0, control.Height);
            Assert.Equal(new Rectangle(0, 0, expectedWidth, 0), control.Bounds);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.Equal(expectedParentLayoutCallCount1, parentLayoutCallCount);
            Assert.Equal(expectedLayoutCallCount, resizeCallCount);
            Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
            Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.True(parent.IsHandleCreated);
            Assert.Equal(0, parentInvalidatedCallCount);
            Assert.Equal(0, parentStyleChangedCallCount);
            Assert.Equal(0, parentCreatedCallCount);

            // Call again.
            control.Width = value;
            Assert.Equal(new Size(expectedWidth, 0), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, expectedWidth, 0), control.ClientRectangle);
            Assert.Equal(new Rectangle(0, 0, expectedWidth, 0), control.DisplayRectangle);
            Assert.Equal(new Size(expectedWidth, 0), control.Size);
            Assert.Equal(0, control.Left);
            Assert.Equal(expectedWidth, control.Right);
            Assert.Equal(0, control.Top);
            Assert.Equal(0, control.Bottom);
            Assert.Equal(expectedWidth, control.Width);
            Assert.Equal(0, control.Height);
            Assert.Equal(new Rectangle(0, 0, expectedWidth, 0), control.Bounds);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.Equal(expectedParentLayoutCallCount2, parentLayoutCallCount);
            Assert.Equal(expectedLayoutCallCount, resizeCallCount);
            Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
            Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.True(parent.IsHandleCreated);
            Assert.Equal(0, parentInvalidatedCallCount);
            Assert.Equal(0, parentStyleChangedCallCount);
            Assert.Equal(0, parentCreatedCallCount);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    [WinFormsFact]
    public void Control_WindowTargetGetSet()
    {
        using Control cont = new();
        Mock<IWindowTarget> mock = new(MockBehavior.Strict);

        cont.WindowTarget = mock.Object;

        Assert.Equal(mock.Object, cont.WindowTarget);
    }

    [WinFormsTheory]
    [StringWithNullData]
    public void Control_WindowText_Set_GetReturnsExpected(string value)
    {
        using Control control = new()
        {
            WindowText = value
        };
        Assert.Equal(value ?? string.Empty, control.WindowText);

        // Set same.
        control.WindowText = value;
        Assert.Equal(value ?? string.Empty, control.WindowText);
    }

    [WinFormsTheory]
    [StringWithNullData]
    public void Control_WindowText_SetWithHandle_GetReturnsExpected(string value)
    {
        using Control control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);

        control.WindowText = value;
        Assert.Equal(value ?? string.Empty, control.WindowText);

        // Set same.
        control.WindowText = value;
        Assert.Equal(value ?? string.Empty, control.WindowText);
    }

    private class SubAxHost : AxHost
    {
        public SubAxHost(string clsid) : base(clsid)
        {
        }
    }
}
