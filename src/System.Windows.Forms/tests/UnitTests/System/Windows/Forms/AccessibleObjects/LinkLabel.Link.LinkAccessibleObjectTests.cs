// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;
using static System.Windows.Forms.LinkLabel;
using static System.Windows.Forms.LinkLabel.Link;

namespace System.Windows.Forms.Tests.AccessibleObjects;

public class LinkLabel_Link_LinkAccessibleObjectTests
{
    [WinFormsFact]
    public void LinkAccessibleObject_Ctor_OwnerLinkCannotBeNull()
    {
        using LinkLabel linkLabel = new();

        Assert.Throws<ArgumentNullException>(() => new LinkAccessibleObject(link: null, linkLabel));
    }

    [WinFormsFact]
    public void LinkAccessibleObject_Ctor_OwnerLinkLabelCannotBeNull()
    {
        Assert.Throws<ArgumentNullException>(() => new LinkAccessibleObject(new Link(), owner: null));
    }

    [WinFormsFact]
    public void LinkAccessibleObject_CurrentIndex_IsExpected()
    {
        using LinkLabel linkLabel = new();

        for (int index = 0; index < 4; index++)
        {
            linkLabel.Links.Add(new());
        }

        for (int index = 0; index < 4; index++)
        {
            LinkAccessibleObject linkAccessibleObject = linkLabel.Links[index].AccessibleObject;
            int actual = linkAccessibleObject.TestAccessor().Dynamic.CurrentIndex;

            Assert.Equal(index, actual);
        }

        Assert.False(linkLabel.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData("TestDescription")]
    [InlineData(null)]
    public void LinkAccessibleObject_Description_IsExpected(string description)
    {
        using LinkLabel linkLabel = new();

        Link link = linkLabel.Links[0];
        link.Description = description;

        Assert.Equal(description, link.AccessibleObject.Description);
        Assert.False(linkLabel.IsHandleCreated);
    }

    [WinFormsFact]
    public void LinkAccessibleObject_FragmentNavigate_NextSibling_IsExpected()
    {
        using LinkLabel linkLabel = new();

        for (int index = 0; index < 4; index++)
        {
            linkLabel.Links.Add(new());
        }

        AccessibleObject linkLabelAccessibleObject1 = linkLabel.Links[0].AccessibleObject;
        AccessibleObject linkLabelAccessibleObject2 = linkLabel.Links[1].AccessibleObject;
        AccessibleObject linkLabelAccessibleObject3 = linkLabel.Links[2].AccessibleObject;
        AccessibleObject linkLabelAccessibleObject4 = linkLabel.Links[3].AccessibleObject;

        Assert.Equal(linkLabelAccessibleObject2, linkLabelAccessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Equal(linkLabelAccessibleObject3, linkLabelAccessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Equal(linkLabelAccessibleObject4, linkLabelAccessibleObject3.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(linkLabelAccessibleObject4.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.False(linkLabel.IsHandleCreated);
    }

    [WinFormsFact]
    public void LinkAccessibleObject_FragmentNavigate_PreviousSibling_IsExpected()
    {
        using LinkLabel linkLabel = new();

        for (int index = 0; index < 4; index++)
        {
            linkLabel.Links.Add(new());
        }

        AccessibleObject linkLabelAccessibleObject1 = linkLabel.Links[0].AccessibleObject;
        AccessibleObject linkLabelAccessibleObject2 = linkLabel.Links[1].AccessibleObject;
        AccessibleObject linkLabelAccessibleObject3 = linkLabel.Links[2].AccessibleObject;
        AccessibleObject linkLabelAccessibleObject4 = linkLabel.Links[3].AccessibleObject;

        Assert.Null(linkLabelAccessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Equal(linkLabelAccessibleObject1, linkLabelAccessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Equal(linkLabelAccessibleObject2, linkLabelAccessibleObject3.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Equal(linkLabelAccessibleObject3, linkLabelAccessibleObject4.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.False(linkLabel.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void LinkAccessibleObject_FragmentNavigate_Parent_IsExpected(int linkIndex)
    {
        using LinkLabel linkLabel = new();

        AccessibleObject expected = linkLabel.AccessibilityObject;

        for (int index = 0; index < 4; index++)
        {
            linkLabel.Links.Add(new());
        }

        LinkAccessibleObject linkAccessibleObject = linkLabel.Links[linkIndex].AccessibleObject;
        IRawElementProviderFragment.Interface actual = linkAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_Parent);

        Assert.Equal(expected, actual);
        Assert.False(linkLabel.IsHandleCreated);
    }

    [WinFormsFact]
    public void LinkAccessibleObject_Parent_IsExpected_IfNoOneLinkWasAdded()
    {
        using LinkLabel linkLabel = new();
        LinkAccessibleObject linkAccessibleObject = linkLabel.Links[0].AccessibleObject;

        AccessibleObject expected = linkLabel.AccessibilityObject;
        AccessibleObject actual = linkAccessibleObject.Parent;

        Assert.Equal(linkLabel.AccessibilityObject, linkAccessibleObject.Parent);
        Assert.False(linkLabel.IsHandleCreated);
    }

    [WinFormsFact]
    public void LinkAccessibleObject_Role_IsLink()
    {
        using LinkLabel linkLabel = new();
        LinkAccessibleObject accessibleObject = linkLabel.Links[0].AccessibleObject;

        AccessibleRole actual = accessibleObject.Role;

        Assert.Equal(AccessibleRole.Link, actual);
        Assert.False(linkLabel.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(((int)UIA_PROPERTY_ID.UIA_IsLegacyIAccessiblePatternAvailablePropertyId))]
    [InlineData(((int)UIA_PROPERTY_ID.UIA_IsInvokePatternAvailablePropertyId))]
    public void LinkAccessibleObject_GetPropertyValue_IsPatternSupported(int propertyId)
    {
        using LinkLabel linkLabel = new();
        LinkAccessibleObject accessibleObject = linkLabel.Links[0].AccessibleObject;

        bool actual = (bool)accessibleObject.GetPropertyValue((UIA_PROPERTY_ID)propertyId);

        Assert.True(actual);
        Assert.False(linkLabel.IsHandleCreated);
    }

    [WinFormsFact]
    public void LinkAccessibleObject_GetPropertyValue_RuntimeIdNotNull()
    {
        using LinkLabel linkLabel = new();
        LinkAccessibleObject accessibleObject = linkLabel.Links[0].AccessibleObject;

        object actual = accessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_RuntimeIdPropertyId);

        Assert.NotNull(actual);
        Assert.False(linkLabel.IsHandleCreated);
    }

    [WinFormsFact]
    public unsafe void LinkAccessibleObject_GetPropertyValue_RuntimeId_ReturnsExpected()
    {
        using LinkLabel linkLabel = new();
        LinkAccessibleObject accessibleObject = linkLabel.Links[0].AccessibleObject;

        using VARIANT actual = accessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_RuntimeIdPropertyId);

        Assert.Equal(accessibleObject.RuntimeId, actual.ToObject());
        Assert.False(linkLabel.IsHandleCreated);
    }

    [WinFormsFact]
    public void LinkAccessibleObject_GetPropertyValue_Name_IsExpected_ForOneLink()
    {
        using LinkLabel linkLabel = new();
        string testName = "TestNameLink";
        linkLabel.Text = testName;

        LinkAccessibleObject linkAccessibleObject = linkLabel.Links[0].AccessibleObject;
        string actual = ((BSTR)linkAccessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_NamePropertyId)).ToStringAndFree();

        Assert.Equal(testName, actual);
        Assert.False(linkLabel.IsHandleCreated);
    }

    [WinFormsFact]
    public void LinkAccessibleObject_Name_IsExpected_ForSeveralLinks()
    {
        using LinkLabel linkLabel = new();
        string[] names = ["Home", "About", "Help", "Details"];
        linkLabel.Text = string.Join(' ', names);
        int start = 0;

        foreach (string name in names)
        {
            linkLabel.Links.Add(new(start, name.Length));
            start += name.Length + 1;
        }

        for (int index = 0; index < linkLabel.Links.Count; index++)
        {
            string actual = linkLabel.Links[index].AccessibleObject.Name;

            Assert.Equal(names[index], actual);
        }

        Assert.False(linkLabel.IsHandleCreated);
    }

    [WinFormsFact]
    public void LinkAccessibleObject_GetPropertyValue_HelpText_ReturnsExpected()
    {
        using LinkLabel linkLabel = new();
        LinkAccessibleObject accessibleObject = linkLabel.Links[0].AccessibleObject;

        string actual = ((BSTR)accessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_HelpTextPropertyId)).ToStringAndFree();

        Assert.Equal(accessibleObject.Help ?? string.Empty, actual);
        Assert.False(linkLabel.IsHandleCreated);
    }

    [WinFormsFact]
    public void LinkAccessibleObject_GetPropertyValue_IsOffscreen_ReturnsFalse()
    {
        using LinkLabel linkLabel = new();
        LinkAccessibleObject accessibleObject = linkLabel.Links[0].AccessibleObject;

        bool actual = (bool)accessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_IsOffscreenPropertyId);

        Assert.False(actual);
        Assert.False(linkLabel.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(false, ((int)UIA_PROPERTY_ID.UIA_IsExpandCollapsePatternAvailablePropertyId))]
    [InlineData(false, ((int)UIA_PROPERTY_ID.UIA_IsGridItemPatternAvailablePropertyId))]
    [InlineData(false, ((int)UIA_PROPERTY_ID.UIA_IsGridPatternAvailablePropertyId))]
    [InlineData(true, ((int)UIA_PROPERTY_ID.UIA_IsLegacyIAccessiblePatternAvailablePropertyId))]
    [InlineData(false, ((int)UIA_PROPERTY_ID.UIA_IsMultipleViewPatternAvailablePropertyId))]
    [InlineData(false, ((int)UIA_PROPERTY_ID.UIA_IsScrollItemPatternAvailablePropertyId))]
    [InlineData(false, ((int)UIA_PROPERTY_ID.UIA_IsScrollPatternAvailablePropertyId))]
    [InlineData(false, ((int)UIA_PROPERTY_ID.UIA_IsSelectionItemPatternAvailablePropertyId))]
    [InlineData(false, ((int)UIA_PROPERTY_ID.UIA_IsSelectionPatternAvailablePropertyId))]
    [InlineData(false, ((int)UIA_PROPERTY_ID.UIA_IsTableItemPatternAvailablePropertyId))]
    [InlineData(false, ((int)UIA_PROPERTY_ID.UIA_IsTablePatternAvailablePropertyId))]
    [InlineData(false, ((int)UIA_PROPERTY_ID.UIA_IsTextPattern2AvailablePropertyId))]
    [InlineData(false, ((int)UIA_PROPERTY_ID.UIA_IsTextPatternAvailablePropertyId))]
    [InlineData(false, ((int)UIA_PROPERTY_ID.UIA_IsTogglePatternAvailablePropertyId))]
    [InlineData(false, ((int)UIA_PROPERTY_ID.UIA_IsValuePatternAvailablePropertyId))]
    public void LinkAccessibleObject_GetPropertyValue_Pattern_ReturnsExpected(bool expected, int propertyId)
    {
        using LinkLabel linkLabel = new();
        LinkAccessibleObject accessibleObject = linkLabel.Links[0].AccessibleObject;
        var result = accessibleObject.GetPropertyValue((UIA_PROPERTY_ID)propertyId);

        Assert.Equal(expected, !result.IsEmpty && (bool)result);
        Assert.False(linkLabel.IsHandleCreated);
    }
}
