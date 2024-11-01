// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Runtime.CompilerServices;

namespace System.Windows.Forms.Tests;

public partial class ControlTests
{
    [WinFormsFact]
    public void Control_GetHandleInternalShouldBeZero()
    {
        using Control control = new();

        IntPtr intptr = control.HandleInternal;

        Assert.Equal(IntPtr.Zero, intptr);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [BoolData]
    public void Control_IsTopMdiWindowClosingGetSet(bool expected)
    {
        using Control control = new()
        {
            IsTopMdiWindowClosing = expected
        };

        Assert.Equal(expected, control.IsTopMdiWindowClosing);
    }

    [WinFormsTheory]
    [EnumData<BoundsSpecified>]
    public void Control_RequiredScaling_Set_GetReturnsExpected(BoundsSpecified value)
    {
        using Control control = new()
        {
            RequiredScaling = value
        };
        Assert.Equal(value, control.RequiredScaling);

        // Set same.
        control.RequiredScaling = value;
        Assert.Equal(value, control.RequiredScaling);
    }

    [WinFormsTheory]
    [BoolData]
    public void Control_RequiredScalingEnabled_Get_ReturnsExpected(bool value)
    {
        using Control control = new()
        {
            RequiredScalingEnabled = value
        };
        Assert.Equal(value, control.RequiredScalingEnabled);

        // Set same.
        control.RequiredScalingEnabled = value;
        Assert.Equal(value, control.RequiredScalingEnabled);

        // Set different.
        control.RequiredScalingEnabled = !value;
        Assert.Equal(!value, control.RequiredScalingEnabled);
    }

    [WinFormsTheory]
    [BoolData]
    public void Control_TabStopInternal_Set_GetReturnsExpected(bool value)
    {
        using Control control = new()
        {
            TabStopInternal = value
        };
        Assert.Equal(value, control.TabStopInternal);

        // Set same.
        control.TabStopInternal = value;
        Assert.Equal(value, control.TabStopInternal);

        // Set different.
        control.TabStopInternal = value;
        Assert.Equal(value, control.TabStopInternal);
    }

    [WinFormsTheory]
    [BoolData]
    public void Control_TabStopInternal_SetWithHandle_GetReturnsExpected(bool value)
    {
        using Control control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);

        control.TabStopInternal = value;
        Assert.Equal(value, control.TabStopInternal);

        // Set same.
        control.TabStopInternal = value;
        Assert.Equal(value, control.TabStopInternal);

        // Set different.
        control.TabStopInternal = value;
        Assert.Equal(value, control.TabStopInternal);
    }

    [WinFormsFact]
    public void Control_TabStopInternal_SetWithHandler_DoesNotCallTabStopChanged()
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
        control.TabStopInternal = false;
        Assert.False(control.TabStopInternal);
        Assert.Equal(0, callCount);

        // Set same.
        control.TabStopInternal = false;
        Assert.False(control.TabStopInternal);
        Assert.Equal(0, callCount);

        // Set different.
        control.TabStopInternal = true;
        Assert.True(control.TabStopInternal);
        Assert.Equal(0, callCount);

        // Remove handler.
        control.TabStopChanged -= handler;
        control.TabStopInternal = false;
        Assert.False(control.TabStopInternal);
        Assert.Equal(0, callCount);
    }

    [WinFormsTheory]
    [InlineData(true, true)]
    [InlineData(false, true)] // setting is impossible; default is false
    // SupportsUseCompatibleTextRendering is always false
    public void Control_UseCompatibleTextRenderingIntGetSet(bool given, bool expected)
    {
        using Control control = new()
        {
            UseCompatibleTextRenderingInternal = given
        };

        Assert.Equal(expected, control.UseCompatibleTextRenderingInternal);
    }

    [WinFormsTheory]
    [BoolData]
    public void Control_ValidationCancelledGetSet(bool expected)
    {
        using Control control = new()
        {
            ValidationCancelled = expected
        };

        Assert.Equal(expected, control.ValidationCancelled);
    }

    [WinFormsTheory]
    [IntegerData<int>]
    public void Control_ApplyBoundsConstraints(int expected)
    {
        using Control control = new();
        Rectangle expectedBounds = new(expected, expected, expected, expected);

        Rectangle actualBounds = control.ApplyBoundsConstraints(expected, expected, expected, expected);

        Assert.Equal(expectedBounds, actualBounds);
    }

    [WinFormsTheory]
    [IntegerData<int>]
    public void Control_ApplySizeConstraints(int expected)
    {
        using Control control = new();
        Size expectedSize = new(expected, expected);

        Size actualSize = control.ApplySizeConstraints(expected, expected);

        Assert.Equal(expectedSize, actualSize);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelper), nameof(CommonTestHelper.GetSizeTheoryData))]
    public void Control_ApplySizeConstraintsSize_Invoke_ReturnsExpected(Size expectedSize)
    {
        using Control control = new();
        Size actualSize = control.ApplySizeConstraints(expectedSize);
        Assert.Equal(expectedSize, actualSize);
    }

    [WinFormsFact]
    public void Control_AssignParent()
    {
        using Control control = new();
        Control parent = new();

        control.AssignParent(parent);

        Assert.Equal(parent, control.Parent);
    }

    [WinFormsFact]
    public void Control_ParentChangedFromAssign()
    {
        bool wasChanged = false;
        using Control control = new();
        control.ParentChanged += (sender, args) => wasChanged = true;
        Control parent = new();

        control.AssignParent(parent);

        Assert.True(wasChanged);
    }

    [WinFormsFact]
    public void Control_CanProcessMnemonic()
    {
        using Control control = new();

        // act and assert
        Assert.True(control.CanProcessMnemonic());
    }

    [WinFormsFact]
    public void Control_CanProcessMnemonicNotEnabled()
    {
        using Control control = new()
        {
            Enabled = false
        };

        // act and assert
        Assert.False(control.CanProcessMnemonic());
    }

    [WinFormsFact]
    public void Control_CanProcessMnemonicNotVisible()
    {
        using Control control = new()
        {
            Visible = false
        };

        // act and assert
        Assert.False(control.CanProcessMnemonic());
    }

    [WinFormsFact]
    public void Control_CanProcessMnemonicParent()
    {
        using Control control = new();
        Control parent = new();
        control.AssignParent(parent);

        // act and assert
        Assert.True(control.CanProcessMnemonic());
    }

    [WinFormsTheory]
    [BoolData]
    public void Control_CreateControlInternal(bool fIgnoreVisible)
    {
        using Control control = new();

        control.CreateControl(fIgnoreVisible);

        Assert.True(control.Created);
    }

    [WinFormsFact]
    public void Control_GetChildControlsInTabOrder()
    {
        using Control control = new();
        Control first = new()
        {
            TabIndex = 0
        };
        Control second = new()
        {
            TabIndex = 1
        };
        Control third = new()
        {
            TabIndex = 2
        };
        var ordered = new Control[]
        {
            first,
            second,
            third
        };
        var unordered = new Control[]
        {
            second,
            first,
            third
        };
        control.Controls.AddRange(unordered);

        Control[] tabOrderedChildren = control.GetChildControlsInTabOrder(false);

        Assert.Equal(ordered, tabOrderedChildren);
    }

    [WinFormsFact]
    public void Control_GetChildControlsInTabOrderHandlesOnly()
    {
        using Control control = new();
        Control first = new()
        {
            TabIndex = 0
        };
        Control second = new()
        {
            TabIndex = 1
        };
        Control third = new()
        {
            TabIndex = 2
        };
        var unordered = new Control[]
        {
            second,
            first,
            third
        };
        control.Controls.AddRange(unordered);

        Control[] tabOrderedChildrenWithhandlesOnly = control.GetChildControlsInTabOrder(true);

        Assert.Empty(tabOrderedChildrenWithhandlesOnly);
    }

    [WinFormsFact]
    public void Control_GetFirstChildControlInTabOrder()
    {
        using Control control = new();
        Control first = new()
        {
            TabIndex = 0
        };
        Control second = new()
        {
            TabIndex = 1
        };
        Control third = new()
        {
            TabIndex = 2
        };
        var tabOrder = new Control[]
        {
            second,
            first,
            third
        };
        control.Controls.AddRange(tabOrder);

        // act and assert
        Assert.Equal(first, control.GetFirstChildControlInTabOrder(true));
    }

    [WinFormsFact]
    public void Control_GetFirstChildControlInTabOrderReverse()
    {
        using Control control = new();
        Control first = new()
        {
            TabIndex = 0
        };
        Control second = new()
        {
            TabIndex = 1
        };
        Control third = new()
        {
            TabIndex = 2
        };
        var tabOrder = new Control[]
        {
            second,
            first,
            third
        };
        control.Controls.AddRange(tabOrder);

        // act and assert
        Assert.Equal(third, control.GetFirstChildControlInTabOrder(false));
    }

    [WinFormsFact]
    public void Control_ReflectParentDoesNotRootParent()
    {
        using Control control = new();
        CreateAndDispose(control);
        GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, blocking: true);
        Assert.Null(control.TestAccessor().Dynamic.ReflectParent);

        [MethodImpl(MethodImplOptions.NoInlining)]
        static void CreateAndDispose(Control control)
        {
            // Note that rooting of locals changes between release and debug. Having a separate method here
            // ensures that we'll always get it collected when doing GC.Collect out in the caller.
            using Form form = new();
            form.Controls.Add(control);
            form.Show();
            form.Controls.Remove(control);
            Assert.NotNull(control.TestAccessor().Dynamic.ReflectParent);
        }
    }
}
