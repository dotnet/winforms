// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Reflection;
using System.Windows.Forms.PropertyGridInternal;
using Windows.Win32.UI.Accessibility;
using static System.Windows.Forms.Control;
using static System.Windows.Forms.PropertyGridInternal.PropertyGridView;

namespace System.Windows.Forms.Tests.AccessibleObjects;

public class PropertyGridView_PropertyGridViewAccessibleObjectTests
{
    [WinFormsFact]
    public void PropertyGridViewAccessibleObject_Ctor_Default()
    {
        using PropertyGrid propertyGrid = new();
        AccessibleObject accessibleObject = propertyGrid.GridViewAccessibleObject;
        Assert.NotNull(accessibleObject);

        using PropertyGridView propertyGridView = new(null, propertyGrid);
        accessibleObject = new PropertyGridViewAccessibleObject(propertyGridView, propertyGrid);
        Assert.NotNull(accessibleObject);
    }

    [WinFormsFact]
    public void PropertyGridViewAccessibleObject_Ctor_NullOwner_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new PropertyGridViewAccessibleObject(null, null));
    }

    [WinFormsTheory]
    [InlineData((int)NavigateDirection.NavigateDirection_Parent)]
    [InlineData((int)NavigateDirection.NavigateDirection_NextSibling)]
    [InlineData((int)NavigateDirection.NavigateDirection_PreviousSibling)]
    [InlineData((int)NavigateDirection.NavigateDirection_FirstChild)]
    [InlineData((int)NavigateDirection.NavigateDirection_LastChild)]
    public void PropertyGridViewAccessibleObject_FragmentNavigate_DoesNotThrowExpection_WithoutOwnerGrid(int direction)
    {
        using PropertyGrid propertyGrid = new();
        using PropertyGridView propertyGridView = new(null, null);
        AccessibleObject accessibleObject = new PropertyGridViewAccessibleObject(propertyGridView, propertyGrid);
        Assert.Null(accessibleObject.FragmentNavigate((NavigateDirection)direction));
    }

    [WinFormsFact]
    public void PropertyGridViewAccessibleObject_GetFocused_ReturnsCorrectValue()
    {
        using PropertyGrid propertyGrid = new();
        using ComboBox comboBox = new();
        propertyGrid.SelectedObject = comboBox;
        GridEntry entry = propertyGrid.GetCurrentEntries()[0].Children[2];
        entry.HasFocus = true;
        entry.Select();
        Assert.Equal(entry, propertyGrid.SelectedGridItem);

        AccessibleObject focusedEntry = propertyGrid.GridViewAccessibleObject.GetFocused();
        Assert.Equal(entry.AccessibilityObject, focusedEntry);
    }

    [WinFormsFact]
    public void PropertyGridViewAccessibleObject_GetSelected_ReturnsCorrectValue()
    {
        using PropertyGrid propertyGrid = new();
        using ComboBox comboBox = new();
        propertyGrid.SelectedObject = comboBox;
        GridEntry entry = propertyGrid.GetCurrentEntries()[0].Children[2];
        entry.HasFocus = true;
        entry.Select();
        Assert.Equal(entry, propertyGrid.SelectedGridItem);

        AccessibleObject focusedEntry = propertyGrid.GridViewAccessibleObject.GetSelected();
        Assert.Equal(entry.AccessibilityObject, focusedEntry);
    }

    [WinFormsFact]
    public void PropertyGridViewAccessibleObject_GetChildCount_ReturnsCorrectValue()
    {
        using PropertyGrid propertyGrid = new();
        AccessibleObject accessibleObject = propertyGrid.GridViewAccessibleObject;

        Assert.Equal(0, accessibleObject.GetChildCount()); // propertyGrid doesn't have an item

        using ComboBox comboBox = new();
        propertyGrid.SelectedObject = comboBox;

        int count = 0;

        foreach (GridEntry category in propertyGrid.GetCurrentEntries())
        {
            count++;

            foreach (GridEntry entry in category.Children)
            {
                count++;
            }
        }

        Assert.Equal(count, accessibleObject.GetChildCount()); // Properties
    }

    [WinFormsFact]
    public void PropertyGridViewAccessibleObject_GetChild_ReturnsCorrectValue()
    {
        using PropertyGrid propertyGrid = new();
        AccessibleObject accessibleObject = propertyGrid.GridViewAccessibleObject;

        Assert.Equal(0, accessibleObject.GetChildCount()); // propertyGrid doesn't have items
        Assert.Null(accessibleObject.GetChild(0)); // GetChild method should not throw an exception

        using ComboBox comboBox = new();
        propertyGrid.SelectedObject = comboBox;

        Assert.True(accessibleObject.GetChild(0) is GridEntry.GridEntryAccessibleObject); // "Accessibility" category entry
        Assert.True(accessibleObject.GetChild(1) is GridEntry.GridEntryAccessibleObject); // "AccessibleDescriptor" entry
        Assert.True(accessibleObject.GetChild(2) is GridEntry.GridEntryAccessibleObject); // "AccessibleName" entry
        Assert.True(accessibleObject.GetChild(3) is GridEntry.GridEntryAccessibleObject); // "AccessibleRole" entry
        Assert.True(accessibleObject.GetChild(4) is GridEntry.GridEntryAccessibleObject); // "Appearance" category entry
    }

    [WinFormsFact]
    public void PropertyGridViewAccessibleObject_Bounds_InsidePropertyGridBounds()
    {
        using PropertyGrid propertyGrid = new();
        propertyGrid.CreateControl();
        propertyGrid.Size = new Size(300, 500);
        using ComboBox comboBox = new();
        propertyGrid.SelectedObject = comboBox;
        AccessibleObject accessibleObject = propertyGrid.GridViewAccessibleObject;

        Rectangle gridViewBounds = propertyGrid.RectangleToClient(accessibleObject.Bounds);
        Rectangle propertyGridBounds = propertyGrid.Bounds;
        Assert.True(propertyGridBounds.Contains(gridViewBounds));
    }

    [WinFormsFact]
    public void PropertyGridViewAccessibleObject_ControlType_IsTable_IfAccessibleRoleIsDefault()
    {
        using PropertyGrid propertyGrid = new();
        // AccessibleRole is not set = Default

        var actual = (UIA_CONTROLTYPE_ID)(int)propertyGrid.GridViewAccessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId);

        Assert.Equal(UIA_CONTROLTYPE_ID.UIA_TableControlTypeId, actual);
        Assert.False(propertyGrid.IsHandleCreated);
    }

    [WinFormsFact]
    public void PropertyGridViewAccessibleObject_Role_IsTable_ByDefault()
    {
        using PropertyGrid propertyGrid = new();
        // AccessibleRole is not set = Default

        AccessibleRole actual = propertyGrid.GridViewAccessibleObject.Role;

        Assert.Equal(AccessibleRole.Table, actual);
        Assert.False(propertyGrid.IsHandleCreated);
    }

    [WinFormsFact]
    public void PropertyGridViewAccessibleObject_GetItem_ReturnsCorrectValue()
    {
        using PropertyGrid propertyGrid = new();
        propertyGrid.CreateControl();
        using ComboBox comboBox = new();
        propertyGrid.SelectedObject = comboBox;
        AccessibleObject accessibleObject = propertyGrid.GridViewAccessibleObject;

        int i = 0;

        foreach (GridEntry category in propertyGrid.GetCurrentEntries())
        {
            AccessibleObject categoryAccessibilityObject = category.AccessibilityObject;
            AccessibleObject categoryItem = (AccessibleObject)accessibleObject.GetItem(i, 1);
            Assert.Equal(categoryAccessibilityObject, categoryItem);
            i++;

            foreach (GridEntry entry in category.Children)
            {
                AccessibleObject entryAccessibilityObject = entry.AccessibilityObject;
                AccessibleObject entryItem = (AccessibleObject)accessibleObject.GetItem(i, 1);
                Assert.Equal(categoryAccessibilityObject, categoryItem);
                i++;
            }
        }
    }

    [WinFormsFact]
    public void PropertyGridViewAccessibleObject_State_IsFocusable()
    {
        using PropertyGrid propertyGrid = new();
        propertyGrid.CreateControl();
        using ComboBox comboBox = new();
        propertyGrid.SelectedObject = comboBox;
        AccessibleObject accessibleObject = propertyGrid.GridViewAccessibleObject;
        Assert.Equal(AccessibleStates.Focusable, accessibleObject.State & AccessibleStates.Focusable);
    }

    [WinFormsFact]
    public void PropertyGridViewAccessibleObject_Ctor_NullOwnerParameter_ThrowsArgumentNullException()
    {
        using PropertyGrid propertyGrid = new();
        AccessibleObject accessibleObject = propertyGrid.GridViewAccessibleObject;
        Type type = accessibleObject.GetType();
        ConstructorInfo ctor = type.GetConstructor([typeof(PropertyGridView), typeof(PropertyGrid)]);
        Assert.NotNull(ctor);
        Assert.Throws<TargetInvocationException>(() => ctor.Invoke([null, null]));
    }

    [WinFormsTheory]
    [InlineData((int)UIA_PROPERTY_ID.UIA_IsGridPatternAvailablePropertyId)]
    [InlineData((int)UIA_PROPERTY_ID.UIA_IsTablePatternAvailablePropertyId)]
    public void PropertyGridViewAccessibleObject_Pattern_IsAvailable(int propertyId)
    {
        using PropertyGrid propertyGrid = new();
        AccessibleObject accessibleObject = propertyGrid.GridViewAccessibleObject;
        Assert.True((bool)accessibleObject.GetPropertyValue((UIA_PROPERTY_ID)propertyId));
    }

    [WinFormsTheory]
    [InlineData((int)UIA_PATTERN_ID.UIA_TablePatternId)]
    [InlineData((int)UIA_PATTERN_ID.UIA_GridPatternId)]
    public void PropertyGridViewAccessibleObject_IsPatternSupported(int patternId)
    {
        using PropertyGrid propertyGrid = new();
        AccessibleObject accessibleObject = propertyGrid.GridViewAccessibleObject;
        Assert.True(accessibleObject.IsPatternSupported((UIA_PATTERN_ID)patternId));
    }

    [WinFormsFact]
    public void PropertyGridViewAccessibleObject_Entry_IsOffscreen_ReturnsCorrectValue()
    {
        using PropertyGrid propertyGrid = new();
        propertyGrid.Size = new Size(300, 500);
        propertyGrid.Location = new Point(0, 0);
        propertyGrid.CreateControl();
        using ComboBox comboBox = new();
        propertyGrid.SelectedObject = comboBox;
        ControlAccessibleObject accessibleObject = (ControlAccessibleObject)propertyGrid.GridViewAccessibleObject;
        PropertyGridView propertyGridView = (PropertyGridView)accessibleObject.Owner;

        Rectangle gridViewRectangle = accessibleObject.Bounds;

        int ROWLABEL = 1;
        int ROWVALUE = 2;

        foreach (GridEntry category in propertyGrid.GetCurrentEntries())
        {
            AccessibleObject categoryAccessibilityObject = category.AccessibilityObject;
            int row = propertyGridView.GetRowFromGridEntry(category);
            Rectangle rect = propertyGridView.GetRectangle(row, ROWVALUE | ROWLABEL);
            rect = propertyGridView.RectangleToScreen(rect);

            bool visible = rect.Height > 0 &&
                         ((rect.Y > gridViewRectangle.Y + 1 && rect.Y < gridViewRectangle.Bottom - 1) ||
                          (rect.Y < gridViewRectangle.Bottom - 1 && rect.Bottom > gridViewRectangle.Y + 1)); // +-1 are borders

            Assert.Equal(!visible, (bool)categoryAccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_IsOffscreenPropertyId));

            foreach (GridEntry entry in category.Children)
            {
                AccessibleObject entryAccessibilityObject = entry.AccessibilityObject;
                row = propertyGridView.GetRowFromGridEntry(entry);
                rect = propertyGridView.GetRectangle(row, ROWVALUE | ROWLABEL);
                rect = propertyGridView.RectangleToScreen(rect);

                visible = rect.Height > 0 &&
                         ((rect.Y > gridViewRectangle.Y + 1 && rect.Y < gridViewRectangle.Bottom - 1) ||
                          (rect.Y < gridViewRectangle.Bottom - 1 && rect.Bottom > gridViewRectangle.Y + 1)); // +-1 are borders

                Assert.Equal(!visible, (bool)entryAccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_IsOffscreenPropertyId));
            }
        }
    }

    [WinFormsFact]
    public void PropertyGridViewAccessibleObject_Owner_IsNotNull()
    {
        using PropertyGrid propertyGrid = new();
        ControlAccessibleObject accessibleObject = (ControlAccessibleObject)propertyGrid.GridViewAccessibleObject;
        Assert.NotNull(accessibleObject.Owner);
    }

    [WinFormsFact]
    public void PropertyGridViewAccessibleObject_Parent_IsNotNull_IfHandleIsCreated()
    {
        using PropertyGrid propertyGrid = new();
        propertyGrid.CreateControl();
        ControlAccessibleObject accessibleObject = (ControlAccessibleObject)propertyGrid.GridViewAccessibleObject;
        Assert.NotNull(accessibleObject.Parent);
    }

    [WinFormsFact]
    public void PropertyGridViewAccessibleObject_Parent_IsNull_IfHandleIsNotCreated()
    {
        using PropertyGrid propertyGrid = new();
        ControlAccessibleObject accessibleObject = (ControlAccessibleObject)propertyGrid.GridViewAccessibleObject;
        Assert.Null(accessibleObject.Parent);
    }

    [WinFormsTheory]
    [InlineData("Some test text")]
    [InlineData("")]
    public void PropertyGridView_GridViewListBoxAccessibleObject_Name_ReturnsDeterminedName(string name)
    {
        using PropertyGrid propertyGrid = new();
        ControlAccessibleObject gridViewAccessibleObject = (ControlAccessibleObject)propertyGrid.GridViewAccessibleObject;
        PropertyGridView propertyGridView = (PropertyGridView)gridViewAccessibleObject.Owner;

        propertyGridView.DropDownListBoxAccessibleObject.Name = name;
        string listAccessibleName = propertyGridView.DropDownListBoxAccessibleObject.Name;
        Assert.Equal(name, listAccessibleName);
    }

    [WinFormsFact]
    public void PropertyGridView_GridViewListBoxAccessibleObject_ReturnsDefaultName()
    {
        using PropertyGrid propertyGrid = new();
        ControlAccessibleObject gridViewAccessibleObject = (ControlAccessibleObject)propertyGrid.GridViewAccessibleObject;
        PropertyGridView propertyGridView = (PropertyGridView)gridViewAccessibleObject.Owner;

        string listAccessibleName = propertyGridView.DropDownListBoxAccessibleObject.Name;
        Assert.Equal(SR.PropertyGridEntryValuesListDefaultAccessibleName, listAccessibleName);
    }

    [WinFormsFact]
    public void PropertyGridView_GridViewListBoxAccessibleObject_ReturnsDefaultName_IfBaseNameIsSetAsNull()
    {
        using PropertyGrid propertyGrid = new();
        ControlAccessibleObject gridViewAccessibleObject = (ControlAccessibleObject)propertyGrid.GridViewAccessibleObject;
        PropertyGridView propertyGridView = (PropertyGridView)gridViewAccessibleObject.Owner;

        propertyGridView.DropDownListBoxAccessibleObject.Name = null;
        string listAccessibleName = propertyGridView.DropDownListBoxAccessibleObject.Name;
        Assert.Equal(SR.PropertyGridEntryValuesListDefaultAccessibleName, listAccessibleName);
    }
}
