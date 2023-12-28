// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Tests.AccessibleObjects;

public class ComboBox_ChildAccessibleObjectTests
{
    [WinFormsFact]
    public void ChildAccessibleObject_Ctor_NullOwner_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new ComboBox.ChildAccessibleObject(null, IntPtr.Zero));
    }

    [WinFormsFact]
    public void ChildAccessibleObject_Ctor_Default()
    {
        using ComboBox control = new();
        control.CreateControl();

        var accessibleObject = new ComboBox.ChildAccessibleObject(control, IntPtr.Zero);

        Assert.NotNull(accessibleObject.TestAccessor().Dynamic._owner);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData("Some string for test")]
    [InlineData("")]
    [InlineData(null)]
    public void ChildAccessibleObject_Name_Default(string testName)
    {
        using ComboBox control = new();
        control.AccessibilityObject.Name = testName;
        control.CreateControl();

        var accessibleObject = new ComboBox.ChildAccessibleObject(control, IntPtr.Zero);

        Assert.Equal(testName, accessibleObject.Name);
        Assert.True(control.IsHandleCreated);
    }
}
