// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Reflection;
using System.Windows.Forms.PropertyGridInternal;
using Windows.Win32.UI.Accessibility;
using static System.Windows.Forms.PropertyGridInternal.PropertyGridView;

namespace System.Windows.Forms.Tests.AccessibleObjects;

public class PropertyGridView_GridViewListBoxItemAccessibleObjectTests
{
    [WinFormsFact]
    public void GridViewListBoxItemAccessibleObject_Ctor_OwnerGridViewListBoxCannotBeNull()
    {
        Type type = typeof(PropertyGridView)
            .GetNestedType("GridViewListBoxItemAccessibleObject", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.Throws<TargetInvocationException>(() => (AccessibleObject)Activator.CreateInstance(type, [null, new ItemArray.Entry("A")]));
    }

    [WinFormsFact]
    public void GridViewListBoxItemAccessibleObject_Ctor_OwnerItemCannotBeNull()
    {
        using GridViewListBox control = new(new PropertyGridView(null, null));
        Type type = typeof(PropertyGridView)
            .GetNestedType("GridViewListBoxItemAccessibleObject", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.Throws<TargetInvocationException>(() => (AccessibleObject)Activator.CreateInstance(type, [control, null]));
    }

    [WinFormsFact]
    public void GridViewListBoxItemAccessibleObject_FragmentRoot_ReturnsExpected()
    {
        using GridViewListBox control = new(new PropertyGridView(null, null));
        Type type = typeof(PropertyGridView)
            .GetNestedType("GridViewListBoxItemAccessibleObject", BindingFlags.NonPublic | BindingFlags.Instance);
        var accessibleObject = (AccessibleObject)Activator.CreateInstance(type, [control, new ItemArray.Entry("A")]);

        Assert.Equal(control.AccessibilityObject, accessibleObject.FragmentRoot);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData("Some string for test")]
    [InlineData("")]
    [InlineData(null)]
    public void GridViewListBoxItemAccessibleObject_Name_ReturnsExpected(string testName)
    {
        using GridViewListBox control = new(new PropertyGridView(null, null));
        Type type = typeof(PropertyGridView)
            .GetNestedType("GridViewListBoxItemAccessibleObject", BindingFlags.NonPublic | BindingFlags.Instance);
        var itemEntry = new ItemArray.Entry(testName);
        var accessibleObject = (AccessibleObject)Activator.CreateInstance(type, [control, itemEntry]);

        Assert.Equal(itemEntry.ToString(), accessibleObject.Name);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData((int)UIA_PATTERN_ID.UIA_InvokePatternId)]
    public void GridViewListBoxItemAccessibleObject_IsPatternSupported_ReturnsExpected(int patternId)
    {
        using GridViewListBox control = new(new PropertyGridView(null, null));
        Type type = typeof(PropertyGridView)
            .GetNestedType("GridViewListBoxItemAccessibleObject", BindingFlags.NonPublic | BindingFlags.Instance);
        var accessibleObject = (AccessibleObject)Activator.CreateInstance(type, [control, new ItemArray.Entry("A")]);

        Assert.True(accessibleObject.IsPatternSupported((UIA_PATTERN_ID)patternId));
        Assert.False(control.IsHandleCreated);
    }
}
