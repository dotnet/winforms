﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using static Interop.UiaCore;

namespace System.Windows.Forms.Tests;

public class ToolStripScrollButtonAccessibleObject_ToolStripScrollButtonAccessibleObjectTests
{
    public static IEnumerable<object[]> ToolStripScrollButtonAccessibleObject_FragmentNavigate_TestData()
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
    [MemberData(nameof(ToolStripScrollButtonAccessibleObject_FragmentNavigate_TestData))]
    public void ToolStripScrollButtonAccessibleObject_FragmentNavigate_ReturnsExpected(Type itemType, bool createControl)
    {
        using ToolStrip toolStrip = new();

        if (createControl)
        {
            toolStrip.CreateControl();
        }

        using ToolStripDropDownItem ownerItem = ReflectionHelper.InvokePublicConstructor<ToolStripDropDownItem>(itemType);
        SubToolStripDropDownMenu dropDownMenu = new SubToolStripDropDownMenu(ownerItem, true, true);

        toolStrip.Items.Add(ownerItem);
        ownerItem.TestAccessor().Dynamic._dropDown = dropDownMenu;
        ownerItem.DropDownItems.Add(new ToolStripDropDownButton("Item 1"));
        ownerItem.DropDownItems.Add(new ToolStripDropDownButton("Item 2"));
        ownerItem.DropDownItems.Add(new ToolStripDropDownButton("Item 3"));
        dropDownMenu.UpdateDisplayedItems();

        AccessibleObject accessibleObject = dropDownMenu.AccessibilityObject;

        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.FirstChild));
        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.LastChild));

        if (!createControl)
        {
            return;
        }

        dropDownMenu.CreateControl(ignoreVisible: true);

        AccessibleObject upScrollButtonAccessibleObject = dropDownMenu.UpScrollButton.AccessibilityObject;
        AccessibleObject itemAccessibleObject1 = dropDownMenu.Items[0].AccessibilityObject;
        AccessibleObject itemAccessibleObject2 = dropDownMenu.Items[1].AccessibilityObject;
        AccessibleObject itemAccessibleObject3 = dropDownMenu.Items[2].AccessibilityObject;
        AccessibleObject downScrollButtonAccessibleObject = dropDownMenu.DownScrollButton.AccessibilityObject;

        Assert.Equal(upScrollButtonAccessibleObject, accessibleObject.FragmentNavigate(NavigateDirection.FirstChild));
        Assert.Equal(downScrollButtonAccessibleObject, accessibleObject.FragmentNavigate(NavigateDirection.LastChild));

        Assert.Equal(upScrollButtonAccessibleObject, itemAccessibleObject1.FragmentNavigate(NavigateDirection.PreviousSibling));
        Assert.Equal(itemAccessibleObject2, itemAccessibleObject1.FragmentNavigate(NavigateDirection.NextSibling));
        Assert.Equal(itemAccessibleObject2, itemAccessibleObject3.FragmentNavigate(NavigateDirection.PreviousSibling));
        Assert.Equal(downScrollButtonAccessibleObject, itemAccessibleObject3.FragmentNavigate(NavigateDirection.NextSibling));

        Assert.Equal(accessibleObject, upScrollButtonAccessibleObject.FragmentNavigate(NavigateDirection.Parent));
        Assert.Equal(itemAccessibleObject1, upScrollButtonAccessibleObject.FragmentNavigate(NavigateDirection.NextSibling));
        Assert.Null(upScrollButtonAccessibleObject.FragmentNavigate(NavigateDirection.PreviousSibling));
        Assert.Null(upScrollButtonAccessibleObject.FragmentNavigate(NavigateDirection.FirstChild));
        Assert.Null(upScrollButtonAccessibleObject.FragmentNavigate(NavigateDirection.LastChild));

        Assert.Equal(accessibleObject, downScrollButtonAccessibleObject.FragmentNavigate(NavigateDirection.Parent));
        Assert.Null(downScrollButtonAccessibleObject.FragmentNavigate(NavigateDirection.NextSibling));
        Assert.Equal(itemAccessibleObject3, downScrollButtonAccessibleObject.FragmentNavigate(NavigateDirection.PreviousSibling));
        Assert.Null(downScrollButtonAccessibleObject.FragmentNavigate(NavigateDirection.FirstChild));
        Assert.Null(downScrollButtonAccessibleObject.FragmentNavigate(NavigateDirection.LastChild));

        Assert.Equal(createControl, toolStrip.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(ToolStripScrollButtonAccessibleObject_FragmentNavigate_TestData))]
    public void ToolStripScrollButtonAccessibleObject_FragmentNavigate_ReturnsNull_If_ScrollButtonsHidden(Type itemType, bool createControl)
    {
        using ToolStrip toolStrip = new();

        if (createControl)
        {
            toolStrip.CreateControl();
        }

        using ToolStripDropDownItem ownerItem = ReflectionHelper.InvokePublicConstructor<ToolStripDropDownItem>(itemType);
        SubToolStripDropDownMenu dropDownMenu = new SubToolStripDropDownMenu(ownerItem, true, false);

        toolStrip.Items.Add(ownerItem);
        ownerItem.TestAccessor().Dynamic._dropDown = dropDownMenu;
        ownerItem.DropDownItems.Add(new ToolStripDropDownButton("Item 1"));
        ownerItem.DropDownItems.Add(new ToolStripDropDownButton("Item 2"));

        dropDownMenu.UpdateDisplayedItems();

        AccessibleObject accessibleObject = dropDownMenu.AccessibilityObject;

        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.FirstChild));
        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.LastChild));

        if (!createControl)
        {
            return;
        }

        dropDownMenu.CreateControl(ignoreVisible: true);

        AccessibleObject itemAccessibleObject1 = dropDownMenu.Items[0].AccessibilityObject;
        AccessibleObject itemAccessibleObject2 = dropDownMenu.Items[1].AccessibilityObject;

        Assert.Equal(itemAccessibleObject1, accessibleObject.FragmentNavigate(NavigateDirection.FirstChild));
        Assert.Equal(itemAccessibleObject2, accessibleObject.FragmentNavigate(NavigateDirection.LastChild));

        Assert.Null(itemAccessibleObject1.FragmentNavigate(NavigateDirection.PreviousSibling));
        Assert.Equal(itemAccessibleObject2, itemAccessibleObject1.FragmentNavigate(NavigateDirection.NextSibling));
        Assert.Equal(itemAccessibleObject1, itemAccessibleObject2.FragmentNavigate(NavigateDirection.PreviousSibling));
        Assert.Null(itemAccessibleObject2.FragmentNavigate(NavigateDirection.NextSibling));
    }

    [WinFormsFact]
    public void ToolStripScrollButtonAccessibleObject_Properties_ReturnExpected()
    {
        using ToolStrip toolStrip = new();
        using ToolStripDropDownItem ownerItem = new ToolStripDropDownButton();
        SubToolStripDropDownMenu dropDownMenu = new SubToolStripDropDownMenu(ownerItem, true, true);

        toolStrip.Items.Add(ownerItem);
        ownerItem.TestAccessor().Dynamic._dropDown = dropDownMenu;
        ownerItem.DropDownItems.Add(new ToolStripDropDownButton("Item 1"));
        ownerItem.DropDownItems.Add(new ToolStripDropDownButton("Item 2"));
        dropDownMenu.UpdateDisplayedItems();

        AccessibleObject upScrollButtonAccessibleObject = dropDownMenu.UpScrollButton.AccessibilityObject;
        AccessibleObject downScrollButtonAccessibleObject = dropDownMenu.DownScrollButton.AccessibilityObject;

        var expectedUpButtonName = SR.ToolStripScrollButtonUpAccessibleName;
        var expectedDownButtonName = SR.ToolStripScrollButtonDownAccessibleName;
        var expectedDefaultAction = SR.AccessibleActionPress;
        var expectedControlType = UIA.ButtonControlTypeId;

        Assert.Equal(expectedUpButtonName, upScrollButtonAccessibleObject.Name);
        Assert.Equal(expectedDefaultAction, upScrollButtonAccessibleObject.DefaultAction);
        Assert.Equal(expectedControlType, upScrollButtonAccessibleObject.GetPropertyValue(UIA.ControlTypePropertyId));

        Assert.Equal(expectedDownButtonName, downScrollButtonAccessibleObject.Name);
        Assert.Equal(expectedDefaultAction, downScrollButtonAccessibleObject.DefaultAction);
        Assert.Equal(expectedControlType, downScrollButtonAccessibleObject.GetPropertyValue(UIA.ControlTypePropertyId));
    }

    private class SubToolStripDropDownMenu : ToolStripDropDownMenu
    {
        private readonly bool _requiresScrollButtons;

        internal SubToolStripDropDownMenu(ToolStripItem ownerItem, bool isAutoGenerated, bool requiresScrollButtons) : base(ownerItem, isAutoGenerated)
        {
            _requiresScrollButtons = requiresScrollButtons;
        }

        internal override bool RequiresScrollButtons => _requiresScrollButtons;

        internal void UpdateDisplayedItems() => base.SetDisplayedItems();
    }
}
