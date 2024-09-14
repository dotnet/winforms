// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms.Tests.AccessibleObjects;

public class ToolStripDropDownMenu_ToolStripDropDownMenuAccessibleObjectTests
{
    public static IEnumerable<object[]> ToolStripDropDownMenuAccessible_FragmentNavigate_WithoutItem_TestData()
    {
        IEnumerable<Type> types = ReflectionHelper.GetPublicNotAbstractClasses<ToolStripDropDownItem>().Select(type => type);
        foreach (Type itemType in types)
        {
            foreach (bool createControl in new[] { true, false })
            {
                yield return new object[] { itemType, createControl };
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(ToolStripDropDownMenuAccessible_FragmentNavigate_WithoutItem_TestData))]
    public void ToolStripDropDownMenuAccessible_FragmentNavigate_ReturnExpected_WithoutItem(Type itemType, bool createControl)
    {
        using ToolStrip toolStrip = new();

        if (createControl)
        {
            toolStrip.CreateControl();
        }

        using ToolStripDropDownItem item = ReflectionHelper.InvokePublicConstructor<ToolStripDropDownItem>(itemType);
        toolStrip.Items.Add(item);

        AccessibleObject accessibleObject = item.DropDown.AccessibilityObject;

        Assert.Equal(item.AccessibilityObject, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));

        Assert.Null(item.AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Null(item.AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));

        Assert.Equal(createControl, toolStrip.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void ToolStripDropDownMenuAccessible_FragmentNavigate_ReturnExpected_WithoutItem_ToolStripOverflowButton(bool createControl)
    {
        using ToolStrip toolStrip = new();

        if (createControl)
        {
            toolStrip.CreateControl();
        }

        ToolStripOverflowButton item = toolStrip.OverflowButton;

        AccessibleObject accessibleObject = item.DropDown.AccessibilityObject;

        Assert.Equal(item.AccessibilityObject, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));

        Assert.Null(item.AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Null(item.AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
        Assert.Equal(createControl, toolStrip.IsHandleCreated);
    }

    public static IEnumerable<object[]> ToolStripDropDownMenuAccessible_FragmentNavigate_TestData()
    {
        IEnumerable<Type> types = ReflectionHelper.GetPublicNotAbstractClasses<ToolStripDropDownItem>().Select(type => type);
#pragma warning disable CA1851 // Possible multiple enumerations of 'IEnumerable' collection
        foreach (Type ownerType in types)
        {
            foreach (Type parentType in types)
            {
                foreach (Type childType in types)
                {
                    foreach (bool createControl in new[] { true, false })
                    {
                        yield return new object[] { ownerType, parentType, childType, createControl };
                    }
                }
            }
        }
#pragma warning restore CA1851
    }

    [WinFormsTheory]
    [MemberData(nameof(ToolStripDropDownMenuAccessible_FragmentNavigate_TestData))]
    public void ToolStripDropDownMenuAccessible_FragmentNavigate_ReturnExpected(Type ownerType, Type parentType, Type childType, bool createControl)
    {
        using ToolStrip toolStrip = new();

        if (createControl)
        {
            toolStrip.CreateControl();
        }

        using ToolStripDropDownItem ownerItem = ReflectionHelper.InvokePublicConstructor<ToolStripDropDownItem>(ownerType);
        using ToolStripDropDownItem parentItem1 = ReflectionHelper.InvokePublicConstructor<ToolStripDropDownItem>(parentType);
        using ToolStripDropDownItem parentItem2 = ReflectionHelper.InvokePublicConstructor<ToolStripDropDownItem>(parentType);
        using ToolStripDropDownItem childItem1 = ReflectionHelper.InvokePublicConstructor<ToolStripDropDownItem>(childType);
        using ToolStripDropDownItem childItem2 = ReflectionHelper.InvokePublicConstructor<ToolStripDropDownItem>(childType);
        using ToolStripDropDownItem childItem3 = ReflectionHelper.InvokePublicConstructor<ToolStripDropDownItem>(childType);
        using ToolStripDropDownItem childItem4 = ReflectionHelper.InvokePublicConstructor<ToolStripDropDownItem>(childType);

        toolStrip.Items.Add(ownerItem);
        ownerItem.DropDownItems.Add(parentItem1);
        ownerItem.DropDownItems.Add(parentItem2);
        parentItem1.DropDownItems.Add(childItem1);
        parentItem1.DropDownItems.Add(childItem2);
        parentItem2.DropDownItems.Add(childItem3);
        parentItem2.DropDownItems.Add(childItem4);

        ownerItem.DropDown.Show();
        AccessibleObject accessibleObject = ownerItem.DropDown.AccessibilityObject;

        Assert.Equal(ownerItem.AccessibilityObject, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Equal(parentItem1.AccessibilityObject, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Equal(parentItem2.AccessibilityObject, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));

        Assert.Equal(accessibleObject, ownerItem.AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Equal(accessibleObject, ownerItem.AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
        Assert.Equal(accessibleObject, parentItem1.AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
        Assert.Equal(accessibleObject, parentItem2.AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));

        parentItem1.DropDown.Show();
        AccessibleObject accessibleObject1 = parentItem1.DropDown.AccessibilityObject;

        Assert.Equal(parentItem1.AccessibilityObject, accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
        Assert.Null(accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Equal(childItem1.AccessibilityObject, accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Equal(childItem2.AccessibilityObject, accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));

        Assert.Equal(accessibleObject1, parentItem1.AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Equal(accessibleObject1, parentItem1.AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
        Assert.Equal(accessibleObject1, childItem1.AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
        Assert.Equal(accessibleObject1, childItem2.AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));

        parentItem2.DropDown.Show();
        AccessibleObject accessibleObject2 = parentItem2.DropDown.AccessibilityObject;

        Assert.Equal(parentItem2.AccessibilityObject, accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
        Assert.Null(accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Equal(childItem3.AccessibilityObject, accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Equal(childItem4.AccessibilityObject, accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));

        Assert.Equal(accessibleObject2, parentItem2.AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Equal(accessibleObject2, parentItem2.AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
        Assert.Equal(accessibleObject2, childItem3.AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
        Assert.Equal(accessibleObject2, childItem4.AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));

        Assert.Equal(createControl, toolStrip.IsHandleCreated);
    }

    public static IEnumerable<object[]> ToolStripDropDownMenuAccessible_FragmentNavigate_ToolStripOverflowButton_TestData()
    {
        IEnumerable<Type> types = ReflectionHelper.GetPublicNotAbstractClasses<ToolStripDropDownItem>().Select(type => type);
#pragma warning disable CA1851 // Possible multiple enumerations of 'IEnumerable' collection
        foreach (Type parentType in types)
        {
            foreach (Type childType in types)
            {
                foreach (bool createControl in new[] { true, false })
                {
                    yield return new object[] { parentType, childType };
                }
            }
        }
#pragma warning restore CA1851
    }

    [WinFormsTheory]
    [MemberData(nameof(ToolStripDropDownMenuAccessible_FragmentNavigate_ToolStripOverflowButton_TestData))]
    public void ToolStripDropDownMenuAccessible_FragmentNavigate_ReturnExpected_ToolStripOverflowButton(Type parentType, Type childType)
    {
        using ToolStrip toolStrip = new();
        toolStrip.CreateControl();

        using ToolStripDropDownItem parentItem1 = ReflectionHelper.InvokePublicConstructor<ToolStripDropDownItem>(parentType);
        using ToolStripDropDownItem parentItem2 = ReflectionHelper.InvokePublicConstructor<ToolStripDropDownItem>(parentType);
        using ToolStripDropDownItem childItem1 = ReflectionHelper.InvokePublicConstructor<ToolStripDropDownItem>(childType);
        using ToolStripDropDownItem childItem2 = ReflectionHelper.InvokePublicConstructor<ToolStripDropDownItem>(childType);
        using ToolStripDropDownItem childItem3 = ReflectionHelper.InvokePublicConstructor<ToolStripDropDownItem>(childType);
        using ToolStripDropDownItem childItem4 = ReflectionHelper.InvokePublicConstructor<ToolStripDropDownItem>(childType);

        parentItem1.DropDownItems.Add(childItem1);
        parentItem1.DropDownItems.Add(childItem2);
        parentItem2.DropDownItems.Add(childItem3);
        parentItem2.DropDownItems.Add(childItem4);

        using ToolStripOverflowButton ownerItem = toolStrip.OverflowButton;
        toolStrip.OverflowItems.Add(parentItem1);
        toolStrip.OverflowItems.Add(parentItem2);

        // ToolStripSplitStackLayout does it in runtime when Layout method is working.
        // Use this way as a workaround in tests.
        // It should be reset in the end of the test to avoid Dispose method errors.
        parentItem1.ParentInternal = ownerItem.DropDown;
        parentItem2.ParentInternal = ownerItem.DropDown;

        ownerItem.DropDown.Show();
        AccessibleObject accessibleObject = ownerItem.DropDown.AccessibilityObject;

        Assert.Equal(ownerItem.AccessibilityObject, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Equal(parentItem1.AccessibilityObject, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Equal(parentItem2.AccessibilityObject, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));

        Assert.Equal(accessibleObject, ownerItem.AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Equal(accessibleObject, ownerItem.AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
        Assert.Equal(accessibleObject, parentItem1.AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
        Assert.Equal(accessibleObject, parentItem2.AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));

        parentItem1.DropDown.Show();
        AccessibleObject accessibleObject1 = parentItem1.DropDown.AccessibilityObject;

        Assert.Equal(parentItem1.AccessibilityObject, accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
        Assert.Null(accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Equal(childItem1.AccessibilityObject, accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Equal(childItem2.AccessibilityObject, accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));

        Assert.Equal(accessibleObject1, parentItem1.AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Equal(accessibleObject1, parentItem1.AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
        Assert.Equal(accessibleObject1, childItem1.AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
        Assert.Equal(accessibleObject1, childItem2.AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));

        parentItem2.DropDown.Show();
        AccessibleObject accessibleObject2 = parentItem2.DropDown.AccessibilityObject;

        Assert.Equal(parentItem2.AccessibilityObject, accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
        Assert.Null(accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Equal(childItem3.AccessibilityObject, accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Equal(childItem4.AccessibilityObject, accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));

        Assert.Equal(accessibleObject2, parentItem2.AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Equal(accessibleObject2, parentItem2.AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
        Assert.Equal(accessibleObject2, childItem3.AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
        Assert.Equal(accessibleObject2, childItem4.AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));

        // Reset the parent to avoid errors when disposing.
        parentItem1.ParentInternal = null;
        parentItem2.ParentInternal = null;
    }

    [WinFormsFact]
    public void ToolStripDropDownMenuAccessible_GetPropertyValue_IsControlElement_ReturnsExpected_InToolStripMenuItem()
    {
        using ToolStripMenuItem menuItem = new();

        ToolStripDropDown dropDown = menuItem.DropDown;

        ToolStripDropDownMenu.ToolStripDropDownMenuAccessibleObject accessibleObject = (ToolStripDropDownMenu.ToolStripDropDownMenuAccessibleObject)dropDown.AccessibilityObject;

        Assert.True((bool)accessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_IsControlElementPropertyId));
    }

    [WinFormsFact]
    public void ToolStripDropDownMenuAccessible_GetPropertyValue_IsControlElement_ReturnsExpected_InToolStripDropDownButton()
    {
        using ToolStripDropDownButton dropDownButton = new();

        ToolStripDropDown dropDown = dropDownButton.DropDown;

        ToolStripDropDownMenu.ToolStripDropDownMenuAccessibleObject accessibleObject = (ToolStripDropDownMenu.ToolStripDropDownMenuAccessibleObject)dropDown.AccessibilityObject;

        Assert.True((bool)accessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_IsControlElementPropertyId));
    }

    [WinFormsFact]
    public void ToolStripDropDownMenuAccessible_GetPropertyValue_IsControlElement_ReturnsExpected_InContextMenuStrip()
    {
        using ContextMenuStrip contextMenu = new();

        ToolStripDropDownMenu.ToolStripDropDownMenuAccessibleObject accessibleObject = (ToolStripDropDownMenu.ToolStripDropDownMenuAccessibleObject)contextMenu.AccessibilityObject;

        Assert.True((bool)accessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_IsControlElementPropertyId));
    }

    [WinFormsFact]
    public void ToolStripDropDownMenuAccessible_GetPropertyValue_IsContentElement_ReturnsExpected_InToolStripMenuItem()
    {
        using ToolStripMenuItem menuItem = new();

        ToolStripDropDown dropDown = menuItem.DropDown;

        ToolStripDropDownMenu.ToolStripDropDownMenuAccessibleObject accessibleObject = (ToolStripDropDownMenu.ToolStripDropDownMenuAccessibleObject)dropDown.AccessibilityObject;

        Assert.False((bool)accessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_IsContentElementPropertyId));
    }

    [WinFormsFact]
    public void ToolStripDropDownMenuAccessible_GetPropertyValue_IsContentElement_ReturnsExpected_InToolStripDropDownButton()
    {
        using ToolStripDropDownButton dropDownButton = new();

        ToolStripDropDown dropDown = dropDownButton.DropDown;

        ToolStripDropDownMenu.ToolStripDropDownMenuAccessibleObject accessibleObject = (ToolStripDropDownMenu.ToolStripDropDownMenuAccessibleObject)dropDown.AccessibilityObject;

        Assert.False((bool)accessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_IsContentElementPropertyId));
    }

    [WinFormsFact]
    public void ToolStripDropDownMenuAccessible_GetPropertyValue_IsContentElement_ReturnsExpected_InContextMenuStrip()
    {
        using ContextMenuStrip contextMenu = new();

        ToolStripDropDownMenu.ToolStripDropDownMenuAccessibleObject accessibleObject = (ToolStripDropDownMenu.ToolStripDropDownMenuAccessibleObject)contextMenu.AccessibilityObject;

        Assert.True((bool)accessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_IsContentElementPropertyId));
    }
}
