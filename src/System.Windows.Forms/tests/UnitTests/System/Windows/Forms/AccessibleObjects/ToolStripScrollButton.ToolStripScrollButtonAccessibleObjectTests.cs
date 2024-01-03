// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms.Tests.AccessibleObjects;

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
        SubToolStripDropDownMenu dropDownMenu = new(ownerItem, true, true);

        toolStrip.Items.Add(ownerItem);
        ownerItem.TestAccessor().Dynamic._dropDown = dropDownMenu;
        ownerItem.DropDownItems.Add(new ToolStripDropDownButton("Item 1"));
        ownerItem.DropDownItems.Add(new ToolStripDropDownButton("Item 2"));
        ownerItem.DropDownItems.Add(new ToolStripDropDownButton("Item 3"));
        dropDownMenu.UpdateDisplayedItems();

        AccessibleObject accessibleObject = dropDownMenu.AccessibilityObject;

        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));

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

        Assert.Equal(upScrollButtonAccessibleObject, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Equal(downScrollButtonAccessibleObject, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));

        Assert.Equal(upScrollButtonAccessibleObject, itemAccessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Equal(itemAccessibleObject2, itemAccessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Equal(itemAccessibleObject2, itemAccessibleObject3.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Equal(downScrollButtonAccessibleObject, itemAccessibleObject3.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));

        Assert.Equal(accessibleObject, upScrollButtonAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
        Assert.Equal(itemAccessibleObject1, upScrollButtonAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(upScrollButtonAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Null(upScrollButtonAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Null(upScrollButtonAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));

        Assert.Equal(accessibleObject, downScrollButtonAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
        Assert.Null(downScrollButtonAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Equal(itemAccessibleObject3, downScrollButtonAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Null(downScrollButtonAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Null(downScrollButtonAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));

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
        SubToolStripDropDownMenu dropDownMenu = new(ownerItem, true, false);

        toolStrip.Items.Add(ownerItem);
        ownerItem.TestAccessor().Dynamic._dropDown = dropDownMenu;
        ownerItem.DropDownItems.Add(new ToolStripDropDownButton("Item 1"));
        ownerItem.DropDownItems.Add(new ToolStripDropDownButton("Item 2"));

        dropDownMenu.UpdateDisplayedItems();

        AccessibleObject accessibleObject = dropDownMenu.AccessibilityObject;

        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));

        if (!createControl)
        {
            return;
        }

        dropDownMenu.CreateControl(ignoreVisible: true);

        AccessibleObject itemAccessibleObject1 = dropDownMenu.Items[0].AccessibilityObject;
        AccessibleObject itemAccessibleObject2 = dropDownMenu.Items[1].AccessibilityObject;

        Assert.Equal(itemAccessibleObject1, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Equal(itemAccessibleObject2, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));

        Assert.Null(itemAccessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Equal(itemAccessibleObject2, itemAccessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Equal(itemAccessibleObject1, itemAccessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Null(itemAccessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
    }

    [WinFormsFact]
    public void ToolStripScrollButtonAccessibleObject_Properties_ReturnExpected()
    {
        using ToolStrip toolStrip = new();
        using ToolStripDropDownItem ownerItem = new ToolStripDropDownButton();
        SubToolStripDropDownMenu dropDownMenu = new(ownerItem, true, true);

        toolStrip.Items.Add(ownerItem);
        ownerItem.TestAccessor().Dynamic._dropDown = dropDownMenu;
        ownerItem.DropDownItems.Add(new ToolStripDropDownButton("Item 1"));
        ownerItem.DropDownItems.Add(new ToolStripDropDownButton("Item 2"));
        dropDownMenu.UpdateDisplayedItems();

        AccessibleObject upScrollButtonAccessibleObject = dropDownMenu.UpScrollButton.AccessibilityObject;
        AccessibleObject downScrollButtonAccessibleObject = dropDownMenu.DownScrollButton.AccessibilityObject;

        string expectedUpButtonName = SR.ToolStripScrollButtonUpAccessibleName;
        string expectedDownButtonName = SR.ToolStripScrollButtonDownAccessibleName;
        string expectedDefaultAction = SR.AccessibleActionPress;
        var expectedControlType = UIA_CONTROLTYPE_ID.UIA_ButtonControlTypeId;

        Assert.Equal(expectedUpButtonName, upScrollButtonAccessibleObject.Name);
        Assert.Equal(expectedDefaultAction, upScrollButtonAccessibleObject.DefaultAction);
        Assert.Equal(expectedControlType, (UIA_CONTROLTYPE_ID)(int)upScrollButtonAccessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId));

        Assert.Equal(expectedDownButtonName, downScrollButtonAccessibleObject.Name);
        Assert.Equal(expectedDefaultAction, downScrollButtonAccessibleObject.DefaultAction);
        Assert.Equal(expectedControlType, (UIA_CONTROLTYPE_ID)(int)downScrollButtonAccessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId));
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
