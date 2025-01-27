// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices.ComTypes;
using System.Windows.Forms.TestUtilities;
using Moq;
using IComDataObject = System.Runtime.InteropServices.ComTypes.IDataObject;
using Size = System.Drawing.Size;

namespace System.Windows.Forms.Tests;

public class ToolStripItemTests
{
    [WinFormsFact]
    public void ToolStripItem_Ctor_Default()
    {
        using SubToolStripItem item = new();
        Assert.NotNull(item.AccessibilityObject);
        Assert.Same(item.AccessibilityObject, item.AccessibilityObject);
        Assert.Null(item.AccessibleDefaultActionDescription);
        Assert.Null(item.AccessibleDescription);
        Assert.Null(item.AccessibleName);
        Assert.Equal(AccessibleRole.Default, item.AccessibleRole);
        Assert.Equal(ToolStripItemAlignment.Left, item.Alignment);
        Assert.False(item.AllowDrop);
        Assert.Equal(AnchorStyles.Top | AnchorStyles.Left, item.Anchor);
        Assert.True(item.AutoSize);
        Assert.False(item.AutoToolTip);
        Assert.True(item.Available);
        Assert.Equal(Control.DefaultBackColor, item.BackColor);
        Assert.Null(item.BackgroundImage);
        Assert.Equal(ImageLayout.Tile, item.BackgroundImageLayout);
        Assert.Equal(new Rectangle(0, 0, 23, 23), item.Bounds);
        Assert.True(item.CanSelect);
        Assert.True(item.CanRaiseEvents);
        Assert.Null(item.Container);
        Assert.Equal(new Rectangle(2, 2, 19, 19), item.ContentRectangle);
        Assert.False(item.DefaultAutoToolTip);
        Assert.Equal(ToolStripItemDisplayStyle.ImageAndText, item.DefaultDisplayStyle);
        Assert.Equal(new Padding(0, 1, 0, 2), item.DefaultMargin);
        Assert.Equal(Padding.Empty, item.DefaultPadding);
        Assert.Equal(new Size(23, 23), item.DefaultSize);
        Assert.False(item.DesignMode);
        Assert.True(item.DismissWhenClicked);
        Assert.Equal(ToolStripItemDisplayStyle.ImageAndText, item.DisplayStyle);
        Assert.Equal(DockStyle.None, item.Dock);
        Assert.False(item.DoubleClickEnabled);
        Assert.True(item.Enabled);
        Assert.NotNull(item.Events);
        Assert.Same(item.Events, item.Events);
        Assert.NotNull(item.Font);
        Assert.NotSame(Control.DefaultFont, item.Font);
        Assert.Same(item.Font, item.Font);
        Assert.Equal(Control.DefaultForeColor, item.ForeColor);
        Assert.Equal(23, item.Height);
        Assert.Null(item.Image);
        Assert.Equal(ContentAlignment.MiddleCenter, item.ImageAlign);
        Assert.Equal(-1, item.ImageIndex);
        Assert.Empty(item.ImageKey);
        Assert.Equal(ToolStripItemImageScaling.SizeToFit, item.ImageScaling);
        Assert.Equal(Color.Empty, item.ImageTransparentColor);
        Assert.False(item.IsDisposed);
        Assert.False(item.IsOnDropDown);
        Assert.False(item.IsOnOverflow);
        Assert.Equal(new Padding(0, 1, 0, 2), item.Margin);
        Assert.Equal(MergeAction.Append, item.MergeAction);
        Assert.Equal(-1, item.MergeIndex);
        Assert.Empty(item.Name);
        Assert.Equal(ToolStripItemOverflow.AsNeeded, item.Overflow);
        Assert.Null(item.OwnerItem);
        Assert.Equal(Padding.Empty, item.Padding);
        Assert.Null(item.Parent);
        Assert.Equal(ToolStripItemPlacement.None, item.Placement);
        Assert.False(item.Pressed);
        Assert.Equal(RightToLeft.Inherit, item.RightToLeft);
        Assert.False(item.RightToLeftAutoMirrorImage);
        Assert.False(item.Selected);
        Assert.Equal(SystemInformation.MenuAccessKeysUnderlined, item.ShowKeyboardCues);
        Assert.Null(item.Site);
        Assert.Equal(new Size(23, 23), item.Size);
        Assert.Null(item.Tag);
        Assert.Empty(item.Text);
        Assert.Equal(ContentAlignment.MiddleCenter, item.TextAlign);
        Assert.Equal(ToolStripTextDirection.Horizontal, item.TextDirection);
        Assert.Equal(TextImageRelation.ImageBeforeText, item.TextImageRelation);
        Assert.Null(item.ToolTipText);
        Assert.False(item.Visible);
        Assert.Equal(23, item.Width);
    }

    public static IEnumerable<object[]> Ctor_String_Image_EventHandler_TestData()
    {
        EventHandler onClick = (sender, e) => { };

        yield return new object[] { null, null, null };
        yield return new object[] { string.Empty, new Bitmap(10, 10), onClick };
        yield return new object[] { "text", new Bitmap(10, 10), onClick };
    }

    [WinFormsTheory]
    [MemberData(nameof(Ctor_String_Image_EventHandler_TestData))]
    public void ToolStripItem_Ctor_String_Image_EventHandler(string text, Image image, EventHandler onClick)
    {
        using SubToolStripItem item = new(text, image, onClick);
        Assert.NotNull(item.AccessibilityObject);
        Assert.Null(item.AccessibleDefaultActionDescription);
        Assert.Null(item.AccessibleDescription);
        Assert.Null(item.AccessibleName);
        Assert.Equal(AccessibleRole.Default, item.AccessibleRole);
        Assert.Equal(ToolStripItemAlignment.Left, item.Alignment);
        Assert.False(item.AllowDrop);
        Assert.Equal(AnchorStyles.Top | AnchorStyles.Left, item.Anchor);
        Assert.True(item.AutoSize);
        Assert.False(item.AutoToolTip);
        Assert.True(item.Available);
        Assert.Null(item.BackgroundImage);
        Assert.Equal(ImageLayout.Tile, item.BackgroundImageLayout);
        Assert.Equal(new Rectangle(0, 0, 23, 23), item.Bounds);
        Assert.True(item.CanSelect);
        Assert.True(item.CanRaiseEvents);
        Assert.Null(item.Container);
        Assert.Equal(new Rectangle(2, 2, 19, 19), item.ContentRectangle);
        Assert.False(item.DefaultAutoToolTip);
        Assert.Equal(ToolStripItemDisplayStyle.ImageAndText, item.DefaultDisplayStyle);
        Assert.Equal(new Padding(0, 1, 0, 2), item.DefaultMargin);
        Assert.Equal(Padding.Empty, item.DefaultPadding);
        Assert.Equal(new Size(23, 23), item.DefaultSize);
        Assert.False(item.DesignMode);
        Assert.True(item.DismissWhenClicked);
        Assert.Equal(ToolStripItemDisplayStyle.ImageAndText, item.DisplayStyle);
        Assert.Equal(DockStyle.None, item.Dock);
        Assert.False(item.DoubleClickEnabled);
        Assert.True(item.Enabled);
        Assert.NotNull(item.Events);
        Assert.Same(item.Events, item.Events);
        Assert.NotNull(item.Font);
        Assert.NotSame(Control.DefaultFont, item.Font);
        Assert.Same(item.Font, item.Font);
        Assert.Equal(Control.DefaultForeColor, item.ForeColor);
        Assert.Equal(23, item.Height);
        Assert.Equal(image, item.Image);
        Assert.Equal(ContentAlignment.MiddleCenter, item.ImageAlign);
        Assert.Equal(-1, item.ImageIndex);
        Assert.Empty(item.ImageKey);
        Assert.Equal(ToolStripItemImageScaling.SizeToFit, item.ImageScaling);
        Assert.Equal(Color.Empty, item.ImageTransparentColor);
        Assert.False(item.IsDisposed);
        Assert.False(item.IsOnDropDown);
        Assert.False(item.IsOnOverflow);
        Assert.Equal(new Padding(0, 1, 0, 2), item.Margin);
        Assert.Equal(MergeAction.Append, item.MergeAction);
        Assert.Equal(-1, item.MergeIndex);
        Assert.Empty(item.Name);
        Assert.Equal(ToolStripItemOverflow.AsNeeded, item.Overflow);
        Assert.Null(item.OwnerItem);
        Assert.Equal(Padding.Empty, item.Padding);
        Assert.Null(item.Parent);
        Assert.Equal(ToolStripItemPlacement.None, item.Placement);
        Assert.False(item.Pressed);
        Assert.Equal(RightToLeft.Inherit, item.RightToLeft);
        Assert.False(item.RightToLeftAutoMirrorImage);
        Assert.False(item.Selected);
        Assert.Equal(SystemInformation.MenuAccessKeysUnderlined, item.ShowKeyboardCues);
        Assert.Null(item.Site);
        Assert.Equal(new Size(23, 23), item.Size);
        Assert.Null(item.Tag);
        Assert.Equal(text, item.Text);
        Assert.Equal(ContentAlignment.MiddleCenter, item.TextAlign);
        Assert.Equal(ToolStripTextDirection.Horizontal, item.TextDirection);
        Assert.Equal(TextImageRelation.ImageBeforeText, item.TextImageRelation);
        Assert.Null(item.ToolTipText);
        Assert.False(item.Visible);
        Assert.Equal(23, item.Width);
    }

    [WinFormsFact]
    public void ToolStripItem_Ctor_String_Image_EventHandler_InvokeClick_CallsOnClick()
    {
        int callCount = 0;
        EventHandler onClick = (sender, e) => callCount++;
        using SubToolStripItem item = new("text", null, onClick);
        item.PerformClick();
        Assert.Equal(1, callCount);
    }

    public static IEnumerable<object[]> Ctor_String_Image_EventHandler_String_TestData()
    {
        EventHandler onClick = (sender, e) => { };

        yield return new object[] { null, null, null, null, string.Empty };
        yield return new object[] { string.Empty, new Bitmap(10, 10), onClick, string.Empty, string.Empty };
        yield return new object[] { "text", new Bitmap(10, 10), onClick, "name", "name" };
    }

    [WinFormsTheory]
    [MemberData(nameof(Ctor_String_Image_EventHandler_String_TestData))]
    public void ToolStripItem_Ctor_String_Image_EventHandler_String(string text, Image image, EventHandler onClick, string name, string expectedName)
    {
        using SubToolStripItem item = new(text, image, onClick, name);
        Assert.NotNull(item.AccessibilityObject);
        Assert.Null(item.AccessibleDefaultActionDescription);
        Assert.Null(item.AccessibleDescription);
        Assert.Null(item.AccessibleName);
        Assert.Equal(AccessibleRole.Default, item.AccessibleRole);
        Assert.Equal(ToolStripItemAlignment.Left, item.Alignment);
        Assert.False(item.AllowDrop);
        Assert.Equal(AnchorStyles.Top | AnchorStyles.Left, item.Anchor);
        Assert.True(item.AutoSize);
        Assert.False(item.AutoToolTip);
        Assert.True(item.Available);
        Assert.Null(item.BackgroundImage);
        Assert.Equal(ImageLayout.Tile, item.BackgroundImageLayout);
        Assert.Equal(new Rectangle(0, 0, 23, 23), item.Bounds);
        Assert.True(item.CanSelect);
        Assert.True(item.CanRaiseEvents);
        Assert.Null(item.Container);
        Assert.Equal(new Rectangle(2, 2, 19, 19), item.ContentRectangle);
        Assert.False(item.DefaultAutoToolTip);
        Assert.Equal(ToolStripItemDisplayStyle.ImageAndText, item.DefaultDisplayStyle);
        Assert.Equal(new Padding(0, 1, 0, 2), item.DefaultMargin);
        Assert.Equal(Padding.Empty, item.DefaultPadding);
        Assert.Equal(new Size(23, 23), item.DefaultSize);
        Assert.False(item.DesignMode);
        Assert.True(item.DismissWhenClicked);
        Assert.Equal(ToolStripItemDisplayStyle.ImageAndText, item.DisplayStyle);
        Assert.Equal(DockStyle.None, item.Dock);
        Assert.False(item.DoubleClickEnabled);
        Assert.True(item.Enabled);
        Assert.NotNull(item.Events);
        Assert.Same(item.Events, item.Events);
        Assert.NotNull(item.Font);
        Assert.NotSame(Control.DefaultFont, item.Font);
        Assert.Same(item.Font, item.Font);
        Assert.Equal(Control.DefaultForeColor, item.ForeColor);
        Assert.Equal(23, item.Height);
        Assert.Equal(image, item.Image);
        Assert.Equal(ContentAlignment.MiddleCenter, item.ImageAlign);
        Assert.Equal(-1, item.ImageIndex);
        Assert.Empty(item.ImageKey);
        Assert.Equal(ToolStripItemImageScaling.SizeToFit, item.ImageScaling);
        Assert.Equal(Color.Empty, item.ImageTransparentColor);
        Assert.False(item.IsDisposed);
        Assert.False(item.IsOnDropDown);
        Assert.False(item.IsOnOverflow);
        Assert.Equal(new Padding(0, 1, 0, 2), item.Margin);
        Assert.Equal(MergeAction.Append, item.MergeAction);
        Assert.Equal(-1, item.MergeIndex);
        Assert.Equal(expectedName, item.Name);
        Assert.Equal(ToolStripItemOverflow.AsNeeded, item.Overflow);
        Assert.Null(item.OwnerItem);
        Assert.Equal(Padding.Empty, item.Padding);
        Assert.Null(item.Parent);
        Assert.Equal(ToolStripItemPlacement.None, item.Placement);
        Assert.False(item.Pressed);
        Assert.Equal(RightToLeft.Inherit, item.RightToLeft);
        Assert.False(item.RightToLeftAutoMirrorImage);
        Assert.False(item.Selected);
        Assert.Equal(SystemInformation.MenuAccessKeysUnderlined, item.ShowKeyboardCues);
        Assert.Null(item.Site);
        Assert.Equal(new Size(23, 23), item.Size);
        Assert.Null(item.Tag);
        Assert.Equal(text, item.Text);
        Assert.Equal(ContentAlignment.MiddleCenter, item.TextAlign);
        Assert.Equal(ToolStripTextDirection.Horizontal, item.TextDirection);
        Assert.Equal(TextImageRelation.ImageBeforeText, item.TextImageRelation);
        Assert.Null(item.ToolTipText);
        Assert.False(item.Visible);
        Assert.Equal(23, item.Width);
    }

    [WinFormsFact]
    public void ToolStripItem_Ctor_String_Image_EventHandler_String_InvokeClick_CallsOnClick()
    {
        int callCount = 0;
        EventHandler onClick = (sender, e) => callCount++;
        using SubToolStripItem item = new("text", null, onClick, "name");
        item.PerformClick();
        Assert.Equal(1, callCount);
    }

    public static IEnumerable<object[]> AccessibilityObject_Get_TestData()
    {
        yield return new object[] { new AccessibleObject() };
        yield return new object[] { new ToolStripItem.ToolStripItemAccessibleObject(new SubToolStripItem()) };
    }

    [WinFormsTheory]
    [MemberData(nameof(AccessibilityObject_Get_TestData))]
    public void ToolStripItem_AccessibilityObject_GetCustomCreateAccessibilityInstance_ReturnsExpected(AccessibleObject result)
    {
        using CustomCreateAccessibilityInstanceToolStripItem item = new()
        {
            CreateAccessibilityInstanceResult = result
        };
        Assert.Same(result, item.AccessibilityObject);
        Assert.Same(item.AccessibilityObject, item.AccessibilityObject);
    }

    private class CustomCreateAccessibilityInstanceToolStripItem : ToolStripItem
    {
        public AccessibleObject CreateAccessibilityInstanceResult { get; set; }

        protected override AccessibleObject CreateAccessibilityInstance() => CreateAccessibilityInstanceResult;
    }

    [WinFormsTheory]
    [StringData]
    public void ToolStripItem_AccessibleDefaultActionDescription_Set_GetReturnsExpected(string value)
    {
        using SubToolStripItem item = new()
        {
            AccessibleDefaultActionDescription = value
        };
        Assert.Equal(value, item.AccessibleDefaultActionDescription);

        // Set same.
        item.AccessibleDefaultActionDescription = value;
        Assert.Equal(value, item.AccessibleDefaultActionDescription);
    }

    [WinFormsTheory]
    [StringData]
    public void ToolStripItem_AccessibleDescription_Set_GetReturnsExpected(string value)
    {
        using SubToolStripItem item = new()
        {
            AccessibleDescription = value
        };
        Assert.Equal(value, item.AccessibleDescription);

        // Set same.
        item.AccessibleDescription = value;
        Assert.Equal(value, item.AccessibleDescription);
    }

    [WinFormsTheory]
    [StringData]
    public void ToolStripItem_AccessibleName_Set_GetReturnsExpected(string value)
    {
        using SubToolStripItem item = new()
        {
            AccessibleName = value
        };
        Assert.Equal(value, item.AccessibleName);

        // Set same.
        item.AccessibleName = value;
        Assert.Equal(value, item.AccessibleName);
    }

    [WinFormsTheory]
    [EnumData<AccessibleRole>]
    public void ToolStripItem_AccessibleRole_Set_GetReturnsExpected(AccessibleRole value)
    {
        using SubToolStripItem item = new()
        {
            AccessibleRole = value
        };
        Assert.Equal(value, item.AccessibleRole);

        // Set same.
        item.AccessibleRole = value;
        Assert.Equal(value, item.AccessibleRole);
    }

    [WinFormsTheory]
    [InvalidEnumData<AccessibleRole>]
    public void ToolStripItem_AccessibleRole_SetInvalid_ThrowsInvalidEnumArgumentException(AccessibleRole value)
    {
        using SubToolStripItem item = new();
        Assert.Throws<InvalidEnumArgumentException>("value", () => item.AccessibleRole = value);
    }

    [WinFormsTheory]
    [EnumData<ToolStripItemAlignment>]
    public void ToolStripItem_Alignment_Set_GetReturnsExpected(ToolStripItemAlignment value)
    {
        using SubToolStripItem item = new();

        item.Alignment = value;
        Assert.Equal(value, item.Alignment);

        // Set same.
        item.Alignment = value;
        Assert.Equal(value, item.Alignment);
    }

    [WinFormsFact]
    public void ToolStripItem_Renderer_GetReturnsExpected()
    {
        using ToolStrip toolStrip = new();
        using SubToolStripItem item = new();
        toolStrip.Items.Add(item);

        Assert.Same(toolStrip.Renderer, item.Renderer);
    }

    [WinFormsTheory]
    [EnumData<ToolStripItemAlignment>]
    public void ToolStripItem_Alignment_SetWithParent_GetReturnsExpected(ToolStripItemAlignment value)
    {
        using ToolStrip parent = new();
        using SubToolStripItem item = new()
        {
            Parent = parent
        };
        int parentLayoutCallCount = 0;
        parent.Layout += (sender, e) => parentLayoutCallCount++;

        item.Alignment = value;
        Assert.Equal(value, item.Alignment);
        Assert.Equal(0, parentLayoutCallCount);
        Assert.False(parent.IsHandleCreated);

        // Set same.
        item.Alignment = value;
        Assert.Equal(value, item.Alignment);
        Assert.Equal(0, parentLayoutCallCount);
        Assert.False(parent.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(ToolStripItemAlignment.Left, 0)]
    [InlineData(ToolStripItemAlignment.Right, 1)]
    public void ToolStripItem_Alignment_SetWithParentWithHandle_GetReturnsExpected(ToolStripItemAlignment value, int expectedParentLayoutCallCount)
    {
        using ToolStrip parent = new();
        using SubToolStripItem item = new()
        {
            Parent = parent
        };
        Assert.NotEqual(IntPtr.Zero, parent.Handle);
        int invalidatedCallCount = 0;
        parent.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        parent.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        parent.HandleCreated += (sender, e) => createdCallCount++;
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(parent, sender);
            Assert.Null(e.AffectedComponent);
            Assert.Null(e.AffectedProperty);
            parentLayoutCallCount++;
        }

        parent.Layout += parentHandler;

        try
        {
            item.Alignment = value;
            Assert.Equal(value, item.Alignment);
            Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
            Assert.True(parent.IsHandleCreated);
            Assert.Equal(expectedParentLayoutCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            item.Alignment = value;
            Assert.Equal(value, item.Alignment);
            Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
            Assert.True(parent.IsHandleCreated);
            Assert.Equal(expectedParentLayoutCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    [WinFormsTheory]
    [EnumData<ToolStripItemAlignment>]
    public void ToolStripItem_Alignment_SetWithOwner_GetReturnsExpected(ToolStripItemAlignment value)
    {
        using ToolStrip owner = new();
        using SubToolStripItem item = new()
        {
            Owner = owner
        };
        int ownerLayoutCallCount = 0;
        void ownerHandler(object sender, LayoutEventArgs e) => ownerLayoutCallCount++;
        owner.Layout += ownerHandler;

        try
        {
            item.Alignment = value;
            Assert.Equal(value, item.Alignment);
            Assert.Equal(0, ownerLayoutCallCount);
            Assert.False(owner.IsHandleCreated);

            // Set same.
            item.Alignment = value;
            Assert.Equal(value, item.Alignment);
            Assert.Equal(0, ownerLayoutCallCount);
            Assert.False(owner.IsHandleCreated);
        }
        finally
        {
            owner.Layout -= ownerHandler;
        }
    }

    [WinFormsTheory]
    [EnumData<ToolStripItemAlignment>]
    public void ToolStripItem_Alignment_SetWithOwnerWithHandle_GetReturnsExpected(ToolStripItemAlignment value)
    {
        using ToolStrip owner = new();
        using SubToolStripItem item = new()
        {
            Owner = owner
        };
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;
        int ownerLayoutCallCount = 0;
        void ownerHandler(object sender, LayoutEventArgs e) => ownerLayoutCallCount++;
        owner.Layout += ownerHandler;

        try
        {
            item.Alignment = value;
            Assert.Equal(value, item.Alignment);
            Assert.Equal(0, ownerLayoutCallCount);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            item.Alignment = value;
            Assert.Equal(value, item.Alignment);
            Assert.Equal(0, ownerLayoutCallCount);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }
        finally
        {
            owner.Layout -= ownerHandler;
        }
    }

    [WinFormsTheory]
    [InvalidEnumData<ToolStripItemAlignment>]
    public void ToolStripItem_Alignment_SetInvalid_ThrowsInvalidEnumArgumentException(ToolStripItemAlignment value)
    {
        using SubToolStripItem item = new();
        Assert.Throws<InvalidEnumArgumentException>("value", () => item.Alignment = value);
    }

    [WinFormsTheory]
    [BoolData]
    public void ToolStripItem_AllowDrop_Set_GetReturnsExpected(bool value)
    {
        using SubToolStripItem item = new()
        {
            AllowDrop = value
        };
        Assert.Equal(value, item.AllowDrop);

        // Set same.
        item.AllowDrop = value;
        Assert.Equal(value, item.AllowDrop);

        // Set different.
        item.AllowDrop = !value;
        Assert.Equal(!value, item.AllowDrop);
    }

    [WinFormsTheory]
    [BoolData]
    public void ToolStripItem_AllowDrop_SetWithOwner_GetReturnsExpected(bool value)
    {
        using ToolStrip owner = new();
        using SubToolStripItem item = new()
        {
            Owner = owner,
            AllowDrop = value
        };
        Assert.Equal(value, item.AllowDrop);
        Assert.False(owner.AllowDrop);
        Assert.False(owner.IsHandleCreated);

        // Set same.
        item.AllowDrop = value;
        Assert.Equal(value, item.AllowDrop);
        Assert.False(owner.AllowDrop);
        Assert.False(owner.IsHandleCreated);

        // Set different.
        item.AllowDrop = !value;
        Assert.Equal(!value, item.AllowDrop);
        Assert.False(owner.AllowDrop);
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsTheory]
    [BoolData]
    public void ToolStripItem_AllowDrop_SetWithOwnerWithHandle_GetReturnsExpected(bool value)
    {
        using ToolStrip owner = new();
        using SubToolStripItem item = new()
        {
            Owner = owner
        };
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;

        item.AllowDrop = value;
        Assert.Equal(value, item.AllowDrop);
        Assert.False(owner.AllowDrop);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        item.AllowDrop = value;
        Assert.Equal(value, item.AllowDrop);
        Assert.False(owner.AllowDrop);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set different.
        item.AllowDrop = !value;
        Assert.Equal(!value, item.AllowDrop);
        Assert.False(owner.AllowDrop);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [BoolData]
    public void ToolStripItem_AllowDrop_SetWithParent_GetReturnsExpected(bool value)
    {
        using ToolStrip parent = new();
        using SubToolStripItem item = new()
        {
            Parent = parent,
            AllowDrop = value
        };
        Assert.Equal(value, item.AllowDrop);
        Assert.False(parent.AllowDrop);
        Assert.False(parent.IsHandleCreated);

        // Set same.
        item.AllowDrop = value;
        Assert.Equal(value, item.AllowDrop);
        Assert.False(parent.AllowDrop);
        Assert.False(parent.IsHandleCreated);

        // Set different.
        item.AllowDrop = !value;
        Assert.Equal(!value, item.AllowDrop);
        Assert.False(parent.AllowDrop);
        Assert.False(parent.IsHandleCreated);
    }

    [WinFormsTheory]
    [BoolData]
    public void ToolStripItem_AllowDrop_SetWithParentWithHandle_GetReturnsExpected(bool value)
    {
        using ToolStrip parent = new();
        using SubToolStripItem item = new()
        {
            Parent = parent
        };
        Assert.NotEqual(IntPtr.Zero, parent.Handle);
        int invalidatedCallCount = 0;
        parent.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        parent.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        parent.HandleCreated += (sender, e) => createdCallCount++;

        item.AllowDrop = value;
        Assert.Equal(value, item.AllowDrop);
        Assert.False(parent.AllowDrop);
        Assert.True(parent.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        item.AllowDrop = value;
        Assert.Equal(value, item.AllowDrop);
        Assert.False(parent.AllowDrop);
        Assert.True(parent.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set different.
        item.AllowDrop = !value;
        Assert.Equal(!value, item.AllowDrop);
        Assert.False(parent.AllowDrop);
        Assert.True(parent.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [BoolData]
    public void ToolStripItem_AllowDrop_SetWithParentWithHandleAlreadyRegistered_GetReturnsExpected(bool value)
    {
        using ToolStrip parent = new()
        {
            AllowDrop = true
        };
        using SubToolStripItem item = new()
        {
            Parent = parent,
            AllowDrop = value
        };
        Assert.Equal(value, item.AllowDrop);

        // Set same.
        item.AllowDrop = value;
        Assert.Equal(value, item.AllowDrop);

        // Set different.
        item.AllowDrop = !value;
        Assert.Equal(!value, item.AllowDrop);
    }

    [Fact] // x-thread
    public void Control_AllowDrop_SetWithParentWithHandleNonSTAThread_ThrowsInvalidOperationException()
    {
        using ToolStrip parent = new();
        using SubToolStripItem item = new()
        {
            Parent = parent
        };
        Assert.NotEqual(IntPtr.Zero, parent.Handle);
        int invalidatedCallCount = 0;
        parent.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        parent.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        parent.HandleCreated += (sender, e) => createdCallCount++;

        Assert.Throws<InvalidOperationException>(() => item.AllowDrop = true);
        Assert.False(item.AllowDrop);
        Assert.True(parent.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Can set to false.
        item.AllowDrop = false;
        Assert.False(item.AllowDrop);
        Assert.True(parent.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> Anchor_Set_TestData()
    {
        yield return new object[] { AnchorStyles.Top | AnchorStyles.Left, AnchorStyles.Top | AnchorStyles.Left };
        yield return new object[] { AnchorStyles.Top, AnchorStyles.Top };
        yield return new object[] { AnchorStyles.None, AnchorStyles.None };
        yield return new object[] { AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right, AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right };
        yield return new object[] { (AnchorStyles)(-1), AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right };
        yield return new object[] { (AnchorStyles)0x10, AnchorStyles.Top | AnchorStyles.Left };
    }

    [WinFormsTheory]
    [MemberData(nameof(Anchor_Set_TestData))]
    public void ToolStripItem_Anchor_Set_GetReturnsExpected(AnchorStyles value, AnchorStyles expected)
    {
        using SubToolStripItem item = new()
        {
            Anchor = value
        };
        Assert.Equal(expected, item.Anchor);

        // Set same.
        item.Anchor = value;
        Assert.Equal(expected, item.Anchor);
    }

    [WinFormsTheory]
    [MemberData(nameof(Anchor_Set_TestData))]
    public void ToolStripItem_Anchor_SetWithOwner_GetReturnsExpected(AnchorStyles value, AnchorStyles expected)
    {
        using ToolStrip owner = new();
        using SubToolStripItem item = new()
        {
            Owner = owner
        };
        int ownerLayoutCallCount = 0;
        void ownerHandler(object sender, LayoutEventArgs e) => ownerLayoutCallCount++;
        owner.Layout += ownerHandler;

        try
        {
            item.Anchor = value;
            Assert.Equal(expected, item.Anchor);
            Assert.Equal(0, ownerLayoutCallCount);
            Assert.False(owner.IsHandleCreated);

            // Set same.
            item.Anchor = value;
            Assert.Equal(expected, item.Anchor);
            Assert.Equal(0, ownerLayoutCallCount);
            Assert.False(owner.IsHandleCreated);
        }
        finally
        {
            owner.Layout -= ownerHandler;
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(Anchor_Set_TestData))]
    public void ToolStripItem_Anchor_SetWithParent_GetReturnsExpected(AnchorStyles value, AnchorStyles expected)
    {
        using ToolStrip parent = new();
        using SubToolStripItem item = new()
        {
            Parent = parent
        };
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e) => parentLayoutCallCount++;
        parent.Layout += parentHandler;

        try
        {
            item.Anchor = value;
            Assert.Equal(expected, item.Anchor);
            Assert.Equal(0, parentLayoutCallCount);
            Assert.False(parent.IsHandleCreated);

            // Set same.
            item.Anchor = value;
            Assert.Equal(expected, item.Anchor);
            Assert.Equal(0, parentLayoutCallCount);
            Assert.False(parent.IsHandleCreated);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    [WinFormsTheory]
    [BoolData]
    public void ToolStripItem_AutoSize_Set_GetReturnsExpected(bool value)
    {
        using SubToolStripItem item = new()
        {
            AutoSize = value
        };
        Assert.Equal(value, item.AutoSize);

        // Set same.
        item.AutoSize = value;
        Assert.Equal(value, item.AutoSize);

        // Set different.
        item.AutoSize = !value;
        Assert.Equal(!value, item.AutoSize);
    }

    [WinFormsTheory]
    [InlineData(true, 0)]
    [InlineData(false, 1)]
    public void ToolStripItem_AutoSize_SetWithOwner_GetReturnsExpected(bool value, int expectedParentLayoutCallCount)
    {
        using ToolStrip owner = new();
        using SubToolStripItem item = new()
        {
            Owner = owner
        };
        int ownerLayoutCallCount = 0;
        void ownerHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(owner, sender);
            Assert.Same(item, e.AffectedComponent);
            Assert.Equal("AutoSize", e.AffectedProperty);
            ownerLayoutCallCount++;
        }

        owner.Layout += ownerHandler;

        try
        {
            item.AutoSize = value;
            Assert.Equal(value, item.AutoSize);
            Assert.Equal(expectedParentLayoutCallCount, ownerLayoutCallCount);
            Assert.False(owner.IsHandleCreated);

            // Set same.
            item.AutoSize = value;
            Assert.Equal(value, item.AutoSize);
            Assert.Equal(expectedParentLayoutCallCount, ownerLayoutCallCount);
            Assert.False(owner.IsHandleCreated);

            // Set different.
            item.AutoSize = !value;
            Assert.Equal(!value, item.AutoSize);
            Assert.Equal(expectedParentLayoutCallCount + 1, ownerLayoutCallCount);
            Assert.False(owner.IsHandleCreated);
        }
        finally
        {
            owner.Layout -= ownerHandler;
        }
    }

    [WinFormsTheory]
    [InlineData(true, 0)]
    [InlineData(false, 1)]
    public void ToolStripItem_AutoSize_SetWithOwnerWithHandle_GetReturnsExpected(bool value, int expectedParentLayoutCallCount)
    {
        using ToolStrip owner = new();
        using SubToolStripItem item = new()
        {
            Owner = owner
        };
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;
        int ownerLayoutCallCount = 0;
        void ownerHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(owner, sender);
            Assert.Same(item, e.AffectedComponent);
            Assert.Equal("AutoSize", e.AffectedProperty);
            ownerLayoutCallCount++;
        }

        owner.Layout += ownerHandler;

        try
        {
            item.AutoSize = value;
            Assert.Equal(value, item.AutoSize);
            Assert.Equal(expectedParentLayoutCallCount, ownerLayoutCallCount);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(expectedParentLayoutCallCount * 2, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            item.AutoSize = value;
            Assert.Equal(value, item.AutoSize);
            Assert.Equal(expectedParentLayoutCallCount, ownerLayoutCallCount);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(expectedParentLayoutCallCount * 2, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set different.
            item.AutoSize = !value;
            Assert.Equal(!value, item.AutoSize);
            Assert.Equal(expectedParentLayoutCallCount + 1, ownerLayoutCallCount);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(expectedParentLayoutCallCount * 2 + 2, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }
        finally
        {
            owner.Layout -= ownerHandler;
        }
    }

    [WinFormsTheory]
    [BoolData]
    public void ToolStripItem_AutoSize_SetWithParent_GetReturnsExpected(bool value)
    {
        using ToolStrip parent = new();
        using SubToolStripItem item = new()
        {
            Parent = parent
        };
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e) => parentLayoutCallCount++;
        parent.Layout += parentHandler;

        try
        {
            item.AutoSize = value;
            Assert.Equal(value, item.AutoSize);
            Assert.Equal(0, parentLayoutCallCount);
            Assert.False(parent.IsHandleCreated);

            // Set same.
            item.AutoSize = value;
            Assert.Equal(value, item.AutoSize);
            Assert.Equal(0, parentLayoutCallCount);
            Assert.False(parent.IsHandleCreated);

            // Set different.
            item.AutoSize = !value;
            Assert.Equal(!value, item.AutoSize);
            Assert.Equal(0, parentLayoutCallCount);
            Assert.False(parent.IsHandleCreated);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    [WinFormsTheory]
    [BoolData]
    public void ToolStripItem_AutoSize_SetWithParentWithHandle_GetReturnsExpected(bool value)
    {
        using ToolStrip parent = new();
        using SubToolStripItem item = new()
        {
            Parent = parent
        };
        Assert.NotEqual(IntPtr.Zero, parent.Handle);
        int invalidatedCallCount = 0;
        parent.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        parent.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        parent.HandleCreated += (sender, e) => createdCallCount++;
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e) => parentLayoutCallCount++;
        parent.Layout += parentHandler;

        try
        {
            item.AutoSize = value;
            Assert.Equal(value, item.AutoSize);
            Assert.Equal(0, parentLayoutCallCount);
            Assert.True(parent.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            item.AutoSize = value;
            Assert.Equal(value, item.AutoSize);
            Assert.Equal(0, parentLayoutCallCount);
            Assert.True(parent.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set different.
            item.AutoSize = !value;
            Assert.Equal(!value, item.AutoSize);
            Assert.Equal(0, parentLayoutCallCount);
            Assert.True(parent.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    [WinFormsFact]
    public void ToolStripItem_AutoToolTip_GetWithDefaultAutoToolTip_ReturnsExpected()
    {
        using CustomDefaultAutoToolTipToolStripItem item = new();
        Assert.True(item.AutoToolTip);
    }

    private class CustomDefaultAutoToolTipToolStripItem : ToolStripItem
    {
        protected override bool DefaultAutoToolTip => true;
    }

    [WinFormsTheory]
    [BoolData]
    public void ToolStripItem_AutoToolTip_Set_GetReturnsExpected(bool value)
    {
        using SubToolStripItem item = new()
        {
            AutoToolTip = value
        };
        Assert.Equal(value, item.AutoToolTip);

        // Set same.
        item.AutoToolTip = value;
        Assert.Equal(value, item.AutoToolTip);

        // Set different.
        item.AutoToolTip = !value;
        Assert.Equal(!value, item.AutoToolTip);
    }

    [WinFormsTheory]
    [MemberData(nameof(SetVisibleCore_TestData))]
    public void ToolStripItem_Available_Set_GetReturnsExpected(bool enabled, Image image, bool value)
    {
        using SubToolStripItem item = new()
        {
            Enabled = enabled,
            Image = image,
            Available = value
        };
        Assert.Equal(value, item.Available);
        Assert.False(item.Visible);
        Assert.False(item.Selected);

        // Set same.
        item.Available = value;
        Assert.Equal(value, item.Available);
        Assert.False(item.Visible);
        Assert.False(item.Selected);

        // Set different.
        item.Available = !value;
        Assert.Equal(!value, item.Available);
        Assert.False(item.Visible);
        Assert.False(item.Selected);
    }

    [WinFormsTheory]
    [MemberData(nameof(SetVisibleCore_TestData))]
    public void ToolStripItem_Available_SetDesignMode_GetReturnsExpected(bool enabled, Image image, bool value)
    {
        Mock<ISite> mockSite = new(MockBehavior.Strict);
        mockSite
            .Setup(s => s.Name)
            .Returns("Name");
        mockSite
            .Setup(s => s.DesignMode)
            .Returns(true);
        mockSite
            .Setup(s => s.Container)
            .Returns((IContainer)null);
        using SubToolStripItem item = new()
        {
            Enabled = enabled,
            Image = image,
            Site = mockSite.Object,
            Available = value
        };
        Assert.Equal(value, item.Available);
        Assert.False(item.Visible);
        Assert.False(item.Selected);

        // Set same.
        item.Available = value;
        Assert.Equal(value, item.Available);
        Assert.False(item.Visible);
        Assert.False(item.Selected);

        // Set different.
        item.Available = !value;
        Assert.Equal(!value, item.Available);
        Assert.False(item.Visible);
        Assert.False(item.Selected);
    }

    [WinFormsTheory]
    [BoolData]
    public void ToolStripItem_Available_SetSelected_GetReturnsExpected(bool value)
    {
        using SubToolStripItem item = new();
        item.Select();
        Assert.True(item.Selected);

        item.Available = value;
        Assert.Equal(value, item.Available);
        Assert.False(item.Visible);
        Assert.Equal(value, item.Selected);

        // Set same.
        item.Available = value;
        Assert.Equal(value, item.Available);
        Assert.False(item.Visible);
        Assert.Equal(value, item.Selected);

        // Set different.
        item.Available = !value;
        Assert.Equal(!value, item.Available);
        Assert.False(item.Visible);
        Assert.False(item.Selected);
    }

    [WinFormsTheory]
    [MemberData(nameof(SetVisibleCore_TestData))]
    public void ToolStripItem_Available_SetWithOwner_GetReturnsExpected(bool enabled, Image image, bool value)
    {
        using ToolStrip owner = new();
        using SubToolStripItem item = new()
        {
            Enabled = enabled,
            Image = image,
            Owner = owner
        };

        item.Available = value;
        Assert.Equal(value, item.Available);
        Assert.False(item.Visible);
        Assert.False(item.Selected);
        Assert.False(owner.IsHandleCreated);

        // Set same.
        item.Available = value;
        Assert.Equal(value, item.Available);
        Assert.False(item.Visible);
        Assert.False(item.Selected);
        Assert.False(owner.IsHandleCreated);

        // Set different.
        item.Available = !value;
        Assert.Equal(!value, item.Available);
        Assert.False(item.Visible);
        Assert.False(item.Selected);
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(SetVisibleCore_TestData))]
    public void ToolStripItem_Available_SetDesignModeWithOwner_GetReturnsExpected(bool enabled, Image image, bool value)
    {
        Mock<ISite> mockSite = new(MockBehavior.Strict);
        mockSite
            .Setup(s => s.Name)
            .Returns("Name");
        mockSite
            .Setup(s => s.DesignMode)
            .Returns(true);
        mockSite
            .Setup(s => s.Container)
            .Returns((IContainer)null);
        using ToolStrip owner = new();
        using SubToolStripItem item = new()
        {
            Enabled = enabled,
            Image = image,
            Site = mockSite.Object,
            Owner = owner,
        };

        item.Available = value;
        Assert.Equal(value, item.Available);
        Assert.False(item.Visible);
        Assert.False(item.Selected);
        Assert.False(owner.IsHandleCreated);

        // Set same.
        item.Available = value;
        Assert.Equal(value, item.Available);
        Assert.False(item.Visible);
        Assert.False(item.Selected);
        Assert.False(owner.IsHandleCreated);

        // Set different.
        item.Available = !value;
        Assert.Equal(!value, item.Available);
        Assert.False(item.Visible);
        Assert.False(item.Selected);
        Assert.False(owner.IsHandleCreated);
    }

    public static IEnumerable<object[]> Available_SetWithOwnerWithHandle_TestData()
    {
        foreach (bool enabled in new bool[] { true, false })
        {
            foreach (Image image in new Image[] { null, new Bitmap(10, 10) })
            {
                yield return new object[] { enabled, image, true, 0 };
                yield return new object[] { enabled, image, false, 2 };
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(Available_SetWithOwnerWithHandle_TestData))]
    public void ToolStripItem_Available_SetWithOwnerWithHandle_GetReturnsExpected(bool enabled, Image image, bool value, int expectedInvalidatedCallCount)
    {
        using ToolStrip owner = new();
        using SubToolStripItem item = new()
        {
            Enabled = enabled,
            Image = image,
            Owner = owner
        };
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;

        item.Available = value;
        Assert.Equal(value, item.Available);
        Assert.False(item.Visible);
        Assert.False(item.Selected);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        item.Available = value;
        Assert.Equal(value, item.Available);
        Assert.False(item.Visible);
        Assert.False(item.Selected);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set different.
        item.Available = !value;
        Assert.Equal(!value, item.Available);
        Assert.Equal(!value, item.Visible);
        Assert.False(item.Selected);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount + 2, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [MemberData(nameof(SetVisibleCore_TestData))]
    public void ToolStripItem_Available_SetWithParent_GetReturnsExpected(bool enabled, Image image, bool value)
    {
        using ToolStrip parent = new();
        using SubToolStripItem item = new()
        {
            Enabled = enabled,
            Image = image,
            Parent = parent
        };

        item.Available = value;
        Assert.Equal(value, item.Available);
        Assert.Equal(value, item.Visible);
        Assert.False(item.Selected);
        Assert.False(parent.IsHandleCreated);

        // Set same.
        item.Available = value;
        Assert.Equal(value, item.Available);
        Assert.Equal(value, item.Visible);
        Assert.False(item.Selected);
        Assert.False(parent.IsHandleCreated);

        // Set different.
        item.Available = !value;
        Assert.Equal(!value, item.Available);
        Assert.Equal(!value, item.Visible);
        Assert.False(item.Selected);
        Assert.False(parent.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(SetVisibleCore_TestData))]
    public void ToolStripItem_Available_SetDesignModeWithParent_GetReturnsExpected(bool enabled, Image image, bool value)
    {
        Mock<ISite> mockSite = new(MockBehavior.Strict);
        mockSite
            .Setup(s => s.Name)
            .Returns("Name");
        mockSite
            .Setup(s => s.DesignMode)
            .Returns(true);
        mockSite
            .Setup(s => s.Container)
            .Returns((IContainer)null);
        using ToolStrip parent = new();
        using SubToolStripItem item = new()
        {
            Enabled = enabled,
            Image = image,
            Site = mockSite.Object,
            Parent = parent
        };

        item.Available = value;
        Assert.Equal(value, item.Available);
        Assert.Equal(value, item.Visible);
        Assert.False(item.Selected);
        Assert.False(parent.IsHandleCreated);

        // Set same.
        item.Available = value;
        Assert.Equal(value, item.Available);
        Assert.Equal(value, item.Visible);
        Assert.False(item.Selected);
        Assert.False(parent.IsHandleCreated);

        // Set different.
        item.Available = !value;
        Assert.Equal(!value, item.Available);
        Assert.Equal(!value, item.Visible);
        Assert.False(item.Selected);
        Assert.False(parent.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(SetVisibleCore_TestData))]
    public void ToolStripItem_Available_SetWithParentWithHandle_GetReturnsExpected(bool enabled, Image image, bool value)
    {
        using ToolStrip parent = new();
        using SubToolStripItem item = new()
        {
            Enabled = enabled,
            Image = image,
            Parent = parent
        };
        Assert.NotEqual(IntPtr.Zero, parent.Handle);
        int invalidatedCallCount = 0;
        parent.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        parent.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        parent.HandleCreated += (sender, e) => createdCallCount++;

        item.Available = value;
        Assert.Equal(value, item.Available);
        Assert.Equal(value, item.Visible);
        Assert.False(item.Selected);
        Assert.True(parent.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        item.Available = value;
        Assert.Equal(value, item.Available);
        Assert.Equal(value, item.Visible);
        Assert.False(item.Selected);
        Assert.True(parent.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set different.
        item.Available = !value;
        Assert.Equal(!value, item.Available);
        Assert.Equal(!value, item.Visible);
        Assert.False(item.Selected);
        Assert.True(parent.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void ToolStripItem_Available_SetWithHandler_CallsAvailableChanged()
    {
        using SubToolStripItem item = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(item, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        item.AvailableChanged += handler;

        // Set different.
        item.Available = false;
        Assert.False(item.Available);
        Assert.Equal(1, callCount);

        // Set same.
        item.Available = false;
        Assert.False(item.Available);
        Assert.Equal(1, callCount);

        // Set different.
        item.Available = true;
        Assert.True(item.Available);
        Assert.Equal(2, callCount);

        // Remove handler.
        item.AvailableChanged -= handler;
        item.Available = false;
        Assert.False(item.Available);
        Assert.Equal(2, callCount);
    }

    [WinFormsFact]
    public void ToolStripItem_Available_SetWithHandler_CallsVisibleChanged()
    {
        using SubToolStripItem item = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(item, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        item.VisibleChanged += handler;

        // Set different.
        item.Available = false;
        Assert.False(item.Available);
        Assert.Equal(1, callCount);

        // Set same.
        item.Available = false;
        Assert.False(item.Available);
        Assert.Equal(1, callCount);

        // Set different.
        item.Available = true;
        Assert.True(item.Available);
        Assert.Equal(2, callCount);

        // Remove handler.
        item.VisibleChanged -= handler;
        item.Available = false;
        Assert.False(item.Available);
        Assert.Equal(2, callCount);
    }

    [WinFormsFact]
    public void ToolStripItem_BackColor_GetWithOwner_ReturnsExpected()
    {
        using ToolStrip owner = new()
        {
            BackColor = Color.Red
        };
        using SubToolStripItem item = new()
        {
            Owner = owner
        };
        Assert.Equal(Control.DefaultBackColor, item.BackColor);
    }

    [WinFormsFact]
    public void ToolStripItem_BackColor_GetWithParent_ReturnsExpected()
    {
        using ToolStrip parent = new()
        {
            BackColor = Color.Red
        };
        using SubToolStripItem item = new()
        {
            Parent = parent
        };
        Assert.Equal(Color.Red, item.BackColor);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetBackColorTheoryData))]
    public void ToolStripItem_BackColor_Set_GetReturnsExpected(Color value, Color expected)
    {
        using SubToolStripItem item = new()
        {
            BackColor = value
        };
        Assert.Equal(expected, item.BackColor);

        // Set same.
        item.BackColor = value;
        Assert.Equal(expected, item.BackColor);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetBackColorTheoryData))]
    public void ToolStripItem_BackColor_SetWithOwner_GetReturnsExpected(Color value, Color expected)
    {
        using ToolStrip owner = new();
        using SubToolStripItem item = new()
        {
            Owner = owner
        };

        item.BackColor = value;
        Assert.Equal(expected, item.BackColor);
        Assert.False(owner.IsHandleCreated);

        // Set same.
        item.BackColor = value;
        Assert.Equal(expected, item.BackColor);
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetBackColorTheoryData))]
    public void ToolStripItem_BackColor_SetWithOwnerWithHandle_GetReturnsExpected(Color value, Color expected)
    {
        using ToolStrip owner = new();
        using SubToolStripItem item = new()
        {
            Owner = owner
        };
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;

        item.BackColor = value;
        Assert.Equal(expected, item.BackColor);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        item.BackColor = value;
        Assert.Equal(expected, item.BackColor);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetBackColorTheoryData))]
    public void ToolStripItem_BackColor_SetWithParent_GetReturnsExpected(Color value, Color expected)
    {
        using ToolStrip parent = new();
        using SubToolStripItem item = new()
        {
            Parent = parent
        };

        item.BackColor = value;
        Assert.Equal(expected, item.BackColor);
        Assert.False(parent.IsHandleCreated);

        // Set same.
        item.BackColor = value;
        Assert.Equal(expected, item.BackColor);
        Assert.False(parent.IsHandleCreated);
    }

    public static IEnumerable<object[]> BackColor_SetWithParentWithHandle_TestData()
    {
        yield return new object[] { Color.Empty, Control.DefaultBackColor, 0 };
        yield return new object[] { Color.Red, Color.Red, 1 };
    }

    [WinFormsTheory]
    [MemberData(nameof(BackColor_SetWithParentWithHandle_TestData))]
    public void ToolStripItem_BackColor_SetWithParentWithHandle_GetReturnsExpected(Color value, Color expected, int expectedInvalidatedCallCount)
    {
        using ToolStrip parent = new();
        using SubToolStripItem item = new()
        {
            Parent = parent
        };
        Assert.NotEqual(IntPtr.Zero, parent.Handle);
        int invalidatedCallCount = 0;
        parent.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        parent.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        parent.HandleCreated += (sender, e) => createdCallCount++;

        item.BackColor = value;
        Assert.Equal(expected, item.BackColor);
        Assert.True(parent.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        item.BackColor = value;
        Assert.Equal(expected, item.BackColor);
        Assert.True(parent.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void ToolStripItem_BackColor_SetWithHandler_CallsBackColorChanged()
    {
        using SubToolStripItem item = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(item, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        item.BackColorChanged += handler;

        // Set different.
        item.BackColor = Color.Red;
        Assert.Equal(Color.Red, item.BackColor);
        Assert.Equal(1, callCount);

        // Set same.
        item.BackColor = Color.Red;
        Assert.Equal(Color.Red, item.BackColor);
        Assert.Equal(1, callCount);

        // Set different.
        item.BackColor = Color.Empty;
        Assert.Equal(Control.DefaultBackColor, item.BackColor);
        Assert.Equal(2, callCount);

        // Remove handler.
        item.BackColorChanged -= handler;
        item.BackColor = Color.Red;
        Assert.Equal(Color.Red, item.BackColor);
        Assert.Equal(2, callCount);
    }

    [WinFormsFact]
    public void ToolStripItem_BackColor_ResetValue_Success()
    {
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ToolStripItem))[nameof(ToolStripItem.BackColor)];
        using SubToolStripItem item = new();
        Assert.False(property.CanResetValue(item));

        item.BackColor = Color.Red;
        Assert.Equal(Color.Red, item.BackColor);
        Assert.True(property.CanResetValue(item));

        property.ResetValue(item);
        Assert.Equal(Control.DefaultBackColor, item.BackColor);
        Assert.False(property.CanResetValue(item));
    }

    [WinFormsFact]
    public void ToolStripItem_BackColor_ShouldSerializeValue_Success()
    {
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ToolStripItem))[nameof(ToolStripItem.BackColor)];
        using SubToolStripItem item = new();
        Assert.False(property.ShouldSerializeValue(item));

        item.BackColor = Color.Red;
        Assert.Equal(Color.Red, item.BackColor);
        Assert.True(property.ShouldSerializeValue(item));

        property.ResetValue(item);
        Assert.Equal(Control.DefaultBackColor, item.BackColor);
        Assert.False(property.ShouldSerializeValue(item));
    }

    public static IEnumerable<object[]> BackgroundImage_Set_TestData()
    {
        yield return new object[] { null };
        yield return new object[] { new Bitmap(10, 10) };
        yield return new object[] { Image.FromFile(Path.Combine("bitmaps", "nature24bits.gif")) };
        yield return new object[] { Image.FromFile(Path.Combine("bitmaps", "10x16_one_entry_32bit.ico")) };
    }

    [WinFormsTheory]
    [MemberData(nameof(BackgroundImage_Set_TestData))]
    public void ToolStripItem_BackgroundImage_Set_GetReturnsExpected(Image value)
    {
        using SubToolStripItem item = new()
        {
            BackgroundImage = value
        };
        Assert.Equal(value, item.BackgroundImage);

        // Set same.
        item.BackgroundImage = value;
        Assert.Equal(value, item.BackgroundImage);
    }

    [WinFormsTheory]
    [MemberData(nameof(BackgroundImage_Set_TestData))]
    public void ToolStripItem_BackgroundImage_SetWithOwner_GetReturnsExpected(Image value)
    {
        using ToolStrip owner = new();
        using SubToolStripItem item = new()
        {
            Owner = owner
        };

        item.BackgroundImage = value;
        Assert.Equal(value, item.BackgroundImage);
        Assert.False(owner.IsHandleCreated);

        // Set same.
        item.BackgroundImage = value;
        Assert.Equal(value, item.BackgroundImage);
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(BackgroundImage_Set_TestData))]
    public void ToolStripItem_BackgroundImage_SetWithOwnerWithHandle_GetReturnsExpected(Image value)
    {
        using ToolStrip owner = new();
        using SubToolStripItem item = new()
        {
            Owner = owner
        };
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;

        item.BackgroundImage = value;
        Assert.Equal(value, item.BackgroundImage);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        item.BackgroundImage = value;
        Assert.Equal(value, item.BackgroundImage);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [MemberData(nameof(BackgroundImage_Set_TestData))]
    public void ToolStripItem_BackgroundImage_SetWithParent_GetReturnsExpected(Image value)
    {
        using ToolStrip parent = new();
        using SubToolStripItem item = new()
        {
            Parent = parent
        };

        item.BackgroundImage = value;
        Assert.Equal(value, item.BackgroundImage);
        Assert.False(parent.IsHandleCreated);

        // Set same.
        item.BackgroundImage = value;
        Assert.Equal(value, item.BackgroundImage);
        Assert.False(parent.IsHandleCreated);
    }

    public static IEnumerable<object[]> BackgroundImage_SetWithParentWithHandle_TestData()
    {
        yield return new object[] { null, 0 };
        yield return new object[] { new Bitmap(10, 10), 1 };
        yield return new object[] { Image.FromFile(Path.Combine("bitmaps", "nature24bits.gif")), 1 };
        yield return new object[] { Image.FromFile(Path.Combine("bitmaps", "10x16_one_entry_32bit.ico")), 1 };
    }

    [WinFormsTheory]
    [MemberData(nameof(BackgroundImage_SetWithParentWithHandle_TestData))]
    public void ToolStripItem_BackgroundImage_SetWithParentWithHandle_GetReturnsExpected(Image value, int expectedInvalidatedCallCount)
    {
        using ToolStrip parent = new();
        using SubToolStripItem item = new()
        {
            Parent = parent
        };
        Assert.NotEqual(IntPtr.Zero, parent.Handle);
        int invalidatedCallCount = 0;
        parent.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        parent.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        parent.HandleCreated += (sender, e) => createdCallCount++;

        item.BackgroundImage = value;
        Assert.Equal(value, item.BackgroundImage);
        Assert.True(parent.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        item.BackgroundImage = value;
        Assert.Equal(value, item.BackgroundImage);
        Assert.True(parent.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [EnumData<ImageLayout>]
    public void ToolStripItem_BackgroundImageLayout_Set_GetReturnsExpected(ImageLayout value)
    {
        using SubToolStripItem item = new()
        {
            BackgroundImageLayout = value
        };
        Assert.Equal(value, item.BackgroundImageLayout);

        // Set same.
        item.BackgroundImageLayout = value;
        Assert.Equal(value, item.BackgroundImageLayout);
    }

    [WinFormsTheory]
    [EnumData<ImageLayout>]
    public void ToolStripItem_BackgroundImageLayout_SetWithOwner_GetReturnsExpected(ImageLayout value)
    {
        using ToolStrip owner = new();
        using SubToolStripItem item = new()
        {
            Owner = owner
        };

        item.BackgroundImageLayout = value;
        Assert.Equal(value, item.BackgroundImageLayout);
        Assert.False(owner.IsHandleCreated);

        // Set same.
        item.BackgroundImageLayout = value;
        Assert.Equal(value, item.BackgroundImageLayout);
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsTheory]
    [EnumData<ImageLayout>]
    public void ToolStripItem_BackgroundImageLayout_SetWithOwnerWithHandle_GetReturnsExpected(ImageLayout value)
    {
        using ToolStrip owner = new();
        using SubToolStripItem item = new()
        {
            Owner = owner
        };
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;

        item.BackgroundImageLayout = value;
        Assert.Equal(value, item.BackgroundImageLayout);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        item.BackgroundImageLayout = value;
        Assert.Equal(value, item.BackgroundImageLayout);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [EnumData<ImageLayout>]
    public void ToolStripItem_BackgroundImageLayout_SetWithParent_GetReturnsExpected(ImageLayout value)
    {
        using ToolStrip parent = new();
        using SubToolStripItem item = new()
        {
            Parent = parent
        };

        item.BackgroundImageLayout = value;
        Assert.Equal(value, item.BackgroundImageLayout);
        Assert.False(parent.IsHandleCreated);

        // Set same.
        item.BackgroundImageLayout = value;
        Assert.Equal(value, item.BackgroundImageLayout);
        Assert.False(parent.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(ImageLayout.None, 1)]
    [InlineData(ImageLayout.Tile, 0)]
    [InlineData(ImageLayout.Center, 1)]
    [InlineData(ImageLayout.Stretch, 1)]
    [InlineData(ImageLayout.Zoom, 1)]
    public void ToolStripItem_BackgroundImageLayout_SetWithParentWithHandle_GetReturnsExpected(ImageLayout value, int expectedInvalidatedCallCount)
    {
        using ToolStrip parent = new();
        using SubToolStripItem item = new()
        {
            Parent = parent
        };
        Assert.NotEqual(IntPtr.Zero, parent.Handle);
        int invalidatedCallCount = 0;
        parent.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        parent.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        parent.HandleCreated += (sender, e) => createdCallCount++;

        item.BackgroundImageLayout = value;
        Assert.Equal(value, item.BackgroundImageLayout);
        Assert.True(parent.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        item.BackgroundImageLayout = value;
        Assert.Equal(value, item.BackgroundImageLayout);
        Assert.True(parent.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InvalidEnumData<ImageLayout>]
    public void ToolStripItem_BackgroundImageLayout_SetInvalid_ThrowsInvalidEnumArgumentException(ImageLayout value)
    {
        using SubToolStripItem item = new();
        Assert.Throws<InvalidEnumArgumentException>("value", () => item.BackgroundImageLayout = value);
    }

    public static IEnumerable<object[]> ContentRectangle_GetWithPadding_ReturnsExpected()
    {
        yield return new object[] { new Padding(-23), new Rectangle(2, 2, 19, 19) };
        yield return new object[] { new Padding(-24), new Rectangle(2, 2, 19, 19) };
        yield return new object[] { new Padding(2), new Rectangle(0, 0, 23, 23) };
        yield return new object[] { new Padding(23), new Rectangle(-21, -21, 65, 65) };
        yield return new object[] { new Padding(24), new Rectangle(-22, -22, 67, 67) };
    }

    [WinFormsTheory]
    [MemberData(nameof(ContentRectangle_GetWithPadding_ReturnsExpected))]
    public void ToolStripItem_ContentRectangle_GetWithLargePadding_ReturnsExpected(Padding padding, Rectangle expected)
    {
        using SubToolStripItem item = new()
        {
            Padding = padding
        };
        Assert.Equal(expected, item.ContentRectangle);
    }

    [WinFormsFact]
    public void ToolStripItem_DefaultMargin_GetWithToolStripOwner_ReturnsExpected()
    {
        using ToolStrip owner = new();
        using SubToolStripItem item = new()
        {
            Owner = owner
        };
        Assert.Equal(new Padding(0, 1, 0, 2), item.DefaultMargin);
    }

    [WinFormsFact]
    public void ToolStripItem_DefaultMargin_GetWithToolStripParent_ReturnsExpected()
    {
        using ToolStrip parent = new();
        using SubToolStripItem item = new()
        {
            Parent = parent
        };
        Assert.Equal(new Padding(0, 1, 0, 2), item.DefaultMargin);
    }

    [WinFormsFact]
    public void ToolStripItem_DefaultMargin_GetWithStatusStripOwner_ReturnsExpected()
    {
        using StatusStrip owner = new();
        using SubToolStripItem item = new()
        {
            Owner = owner
        };
        Assert.Equal(new Padding(0, 2, 0, 0), item.DefaultMargin);
    }

    [WinFormsFact]
    public void ToolStripItem_DefaultMargin_GetWithStatusStripParent_ReturnsExpected()
    {
        using StatusStrip parent = new();
        using SubToolStripItem item = new()
        {
            Parent = parent
        };
        Assert.Equal(new Padding(0, 1, 0, 2), item.DefaultMargin);
    }

    [WinFormsFact]
    public void ToolStripItem_DisplayStyle_GetWithCustomDisplayStyle_ReturnsExpected()
    {
        using CustomDefaultDisplayStyleToolStripItem item = new();
        Assert.Equal(ToolStripItemDisplayStyle.Text, item.DisplayStyle);
    }

    private class CustomDefaultDisplayStyleToolStripItem : ToolStripItem
    {
        protected override ToolStripItemDisplayStyle DefaultDisplayStyle => ToolStripItemDisplayStyle.Text;
    }

    [WinFormsTheory]
    [EnumData<ToolStripItemDisplayStyle>]
    public void ToolStripItem_DisplayStyle_Set_GetReturnsExpected(ToolStripItemDisplayStyle value)
    {
        using SubToolStripItem item = new()
        {
            DisplayStyle = value
        };
        Assert.Equal(value, item.DisplayStyle);

        // Set same.
        item.DisplayStyle = value;
        Assert.Equal(value, item.DisplayStyle);
    }

    [WinFormsTheory]
    [InlineData(ToolStripItemDisplayStyle.None, 1)]
    [InlineData(ToolStripItemDisplayStyle.Text, 1)]
    [InlineData(ToolStripItemDisplayStyle.Image, 1)]
    [InlineData(ToolStripItemDisplayStyle.ImageAndText, 0)]
    public void ToolStripItem_DisplayStyle_SetWithOwner_GetReturnsExpected(ToolStripItemDisplayStyle value, int expectedOwnerLayoutCallCount)
    {
        using ToolStrip owner = new();
        using SubToolStripItem item = new()
        {
            Owner = owner
        };
        int ownerLayoutCallCount = 0;
        void ownerHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(owner, sender);
            Assert.Same(item, e.AffectedComponent);
            Assert.Equal("DisplayStyle", e.AffectedProperty);
            ownerLayoutCallCount++;
        }

        owner.Layout += ownerHandler;

        try
        {
            item.DisplayStyle = value;
            Assert.Equal(value, item.DisplayStyle);
            Assert.Equal(expectedOwnerLayoutCallCount, ownerLayoutCallCount);
            Assert.False(owner.IsHandleCreated);

            // Set same.
            item.DisplayStyle = value;
            Assert.Equal(value, item.DisplayStyle);
            Assert.False(owner.IsHandleCreated);
        }
        finally
        {
            owner.Layout -= ownerHandler;
        }
    }

    [WinFormsTheory]
    [InlineData(ToolStripItemDisplayStyle.None, 1)]
    [InlineData(ToolStripItemDisplayStyle.Text, 1)]
    [InlineData(ToolStripItemDisplayStyle.Image, 1)]
    [InlineData(ToolStripItemDisplayStyle.ImageAndText, 0)]
    public void ToolStripItem_DisplayStyle_SetWithOwnerWithHandle_GetReturnsExpected(ToolStripItemDisplayStyle value, int expectedOwnerLayoutCallCount)
    {
        using ToolStrip owner = new();
        using SubToolStripItem item = new()
        {
            Owner = owner
        };
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;
        int ownerLayoutCallCount = 0;
        void ownerHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(owner, sender);
            Assert.Same(item, e.AffectedComponent);
            Assert.Equal("DisplayStyle", e.AffectedProperty);
            ownerLayoutCallCount++;
        }

        owner.Layout += ownerHandler;

        try
        {
            item.DisplayStyle = value;
            Assert.Equal(value, item.DisplayStyle);
            Assert.Equal(expectedOwnerLayoutCallCount, ownerLayoutCallCount);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(expectedOwnerLayoutCallCount * 2, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            item.DisplayStyle = value;
            Assert.Equal(value, item.DisplayStyle);
            Assert.Equal(expectedOwnerLayoutCallCount, ownerLayoutCallCount);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(expectedOwnerLayoutCallCount * 2, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }
        finally
        {
            owner.Layout -= ownerHandler;
        }
    }

    [WinFormsTheory]
    [EnumData<ToolStripItemDisplayStyle>]
    public void ToolStripItem_DisplayStyle_SetWithParent_GetReturnsExpected(ToolStripItemDisplayStyle value)
    {
        using ToolStrip parent = new();
        using SubToolStripItem item = new()
        {
            Parent = parent
        };
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e) => parentLayoutCallCount++;
        parent.Layout += parentHandler;

        item.DisplayStyle = value;
        Assert.Equal(value, item.DisplayStyle);
        Assert.Equal(0, parentLayoutCallCount);
        Assert.False(parent.IsHandleCreated);

        // Set same.
        item.DisplayStyle = value;
        Assert.Equal(value, item.DisplayStyle);
        Assert.False(parent.IsHandleCreated);
    }

    [WinFormsTheory]
    [EnumData<ToolStripItemDisplayStyle>]
    public void ToolStripItem_DisplayStyle_SetWithParentWithHandle_GetReturnsExpected(ToolStripItemDisplayStyle value)
    {
        using ToolStrip parent = new();
        using SubToolStripItem item = new()
        {
            Parent = parent
        };
        Assert.NotEqual(IntPtr.Zero, parent.Handle);
        int invalidatedCallCount = 0;
        parent.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        parent.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        parent.HandleCreated += (sender, e) => createdCallCount++;
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e) => parentLayoutCallCount++;
        parent.Layout += parentHandler;

        item.DisplayStyle = value;
        Assert.Equal(value, item.DisplayStyle);
        Assert.Equal(0, parentLayoutCallCount);
        Assert.True(parent.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        item.DisplayStyle = value;
        Assert.Equal(value, item.DisplayStyle);
        Assert.Equal(0, parentLayoutCallCount);
        Assert.True(parent.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void ToolStripItem_DisplayStyle_SetWithHandler_CallsDisplayStyleChanged()
    {
        using SubToolStripItem item = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(item, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        item.DisplayStyleChanged += handler;

        // Set different.
        item.DisplayStyle = ToolStripItemDisplayStyle.Text;
        Assert.Equal(ToolStripItemDisplayStyle.Text, item.DisplayStyle);
        Assert.Equal(1, callCount);

        // Set same.
        item.DisplayStyle = ToolStripItemDisplayStyle.Text;
        Assert.Equal(ToolStripItemDisplayStyle.Text, item.DisplayStyle);
        Assert.Equal(1, callCount);

        // Set different.
        item.DisplayStyle = ToolStripItemDisplayStyle.None;
        Assert.Equal(ToolStripItemDisplayStyle.None, item.DisplayStyle);
        Assert.Equal(2, callCount);

        // Remove handler.
        item.DisplayStyleChanged -= handler;
        item.DisplayStyle = ToolStripItemDisplayStyle.Text;
        Assert.Equal(2, callCount);
    }

    [WinFormsTheory]
    [InvalidEnumData<ToolStripItemDisplayStyle>]
    public void ToolStripItem_DisplayStyle_SetInvalid_ThrowsInvalidEnumArgumentException(ToolStripItemDisplayStyle value)
    {
        using SubToolStripItem item = new();
        Assert.Throws<InvalidEnumArgumentException>("value", () => item.DisplayStyle = value);
    }

    [WinFormsFact]
    public void ToolStripItem_DisplayStyle_ResetValue_Success()
    {
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ToolStripItem))[nameof(ToolStripItem.DisplayStyle)];
        using SubToolStripItem item = new();
        Assert.False(property.CanResetValue(item));

        item.DisplayStyle = ToolStripItemDisplayStyle.Text;
        Assert.Equal(ToolStripItemDisplayStyle.Text, item.DisplayStyle);
        Assert.True(property.CanResetValue(item));

        property.ResetValue(item);
        Assert.Equal(ToolStripItemDisplayStyle.ImageAndText, item.DisplayStyle);
        Assert.False(property.CanResetValue(item));
    }

    [WinFormsFact]
    public void ToolStripItem_DisplayStyle_ShouldSerializeValue_Success()
    {
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ToolStripItem))[nameof(ToolStripItem.DisplayStyle)];
        using SubToolStripItem item = new();
        Assert.False(property.ShouldSerializeValue(item));

        item.DisplayStyle = ToolStripItemDisplayStyle.Text;
        Assert.Equal(ToolStripItemDisplayStyle.Text, item.DisplayStyle);
        Assert.True(property.ShouldSerializeValue(item));

        property.ResetValue(item);
        Assert.Equal(ToolStripItemDisplayStyle.ImageAndText, item.DisplayStyle);
        Assert.False(property.ShouldSerializeValue(item));
    }

    [WinFormsTheory]
    [EnumData<DockStyle>]
    public void ToolStripItem_Dock_Set_GetReturnsExpected(DockStyle value)
    {
        using SubToolStripItem item = new()
        {
            Dock = value
        };
        Assert.Equal(value, item.Dock);

        // Set same.
        item.Dock = value;
        Assert.Equal(value, item.Dock);
    }

    [WinFormsTheory]
    [EnumData<DockStyle>]
    public void ToolStripItem_Dock_SetWithOwner_GetReturnsExpected(DockStyle value)
    {
        using ToolStrip owner = new();
        using SubToolStripItem item = new()
        {
            Owner = owner
        };

        int ownerLayoutCallCount = 0;
        void ownerHandler(object sender, LayoutEventArgs e) => ownerLayoutCallCount++;
        owner.Layout += ownerHandler;

        try
        {
            item.Dock = value;
            Assert.Equal(value, item.Dock);
            Assert.Equal(0, ownerLayoutCallCount);
            Assert.False(owner.IsHandleCreated);

            // Set same.
            item.Dock = value;
            Assert.Equal(value, item.Dock);
            Assert.Equal(0, ownerLayoutCallCount);
            Assert.False(owner.IsHandleCreated);
        }
        finally
        {
            owner.Layout -= ownerHandler;
        }
    }

    [WinFormsTheory]
    [EnumData<DockStyle>]
    public void ToolStripItem_Dock_SetWithParent_GetReturnsExpected(DockStyle value)
    {
        using ToolStrip parent = new();
        using SubToolStripItem item = new()
        {
            Parent = parent
        };

        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e) => parentLayoutCallCount++;
        parent.Layout += parentHandler;

        try
        {
            item.Dock = value;
            Assert.Equal(value, item.Dock);
            Assert.Equal(0, parentLayoutCallCount);
            Assert.False(parent.IsHandleCreated);

            // Set same.
            item.Dock = value;
            Assert.Equal(value, item.Dock);
            Assert.Equal(0, parentLayoutCallCount);
            Assert.False(parent.IsHandleCreated);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    [WinFormsTheory]
    [InvalidEnumData<DockStyle>]
    public void ToolStripItem_Dock_SetInvalid_ThrowsInvalidEnumArgumentException(DockStyle value)
    {
        using SubToolStripItem item = new();
        Assert.Throws<InvalidEnumArgumentException>("value", () => item.Dock = value);
    }

    [WinFormsTheory]
    [BoolData]
    public void ToolStripItem_DoubleClickEnabled_Set_GetReturnsExpected(bool value)
    {
        using SubToolStripItem item = new()
        {
            DoubleClickEnabled = value
        };
        Assert.Equal(value, item.DoubleClickEnabled);

        // Set same.
        item.DoubleClickEnabled = value;
        Assert.Equal(value, item.DoubleClickEnabled);
    }

    [WinFormsTheory]
    [InlineData(true, true, true)]
    [InlineData(true, false, false)]
    [InlineData(false, true, false)]
    [InlineData(false, false, false)]
    public void ToolStripItem_Enabled_GetWithOwner_ReturnsExpected(bool ownerEnabled, bool enabled, bool expected)
    {
        using ToolStrip owner = new()
        {
            Enabled = ownerEnabled
        };
        using SubToolStripItem item = new()
        {
            Owner = owner
        };
        Assert.Equal(ownerEnabled, item.Enabled);

        // Set custom.
        item.Enabled = enabled;
        Assert.Equal(expected, item.Enabled);
    }

    [WinFormsTheory]
    [InlineData(true, true, true)]
    [InlineData(true, false, false)]
    [InlineData(false, true, true)]
    [InlineData(false, false, false)]
    public void ToolStripItem_Enabled_GetWithParent_ReturnsExpected(bool parentEnabled, bool enabled, bool expected)
    {
        using ToolStrip parent = new()
        {
            Enabled = parentEnabled
        };
        using SubToolStripItem item = new()
        {
            Parent = parent
        };
        Assert.True(item.Enabled);

        // Set custom.
        item.Enabled = enabled;
        Assert.Equal(expected, item.Enabled);
    }

    public static IEnumerable<object[]> Enabled_Set_TestData()
    {
        foreach (bool visible in new bool[] { true, false })
        {
            foreach (Image image in new Image[] { null, new Bitmap(10, 10) })
            {
                yield return new object[] { visible, image, true };
                yield return new object[] { visible, image, false };
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(Enabled_Set_TestData))]
    public void ToolStripItem_Enabled_Set_GetReturnsExpected(bool visible, Image image, bool value)
    {
        using SubToolStripItem item = new()
        {
            Visible = visible,
            Image = image,
            Enabled = value
        };
        Assert.Equal(value, item.Enabled);

        // Set same.
        item.Enabled = value;
        Assert.Equal(value, item.Enabled);

        // Set different.
        item.Enabled = !value;
        Assert.Equal(!value, item.Enabled);
    }

    [WinFormsTheory]
    [BoolData]
    public void ToolStripItem_Enabled_SetSelected_GetReturnsExpected(bool value)
    {
        using SubToolStripItem item = new();
        item.Select();
        Assert.True(item.Selected);

        item.Enabled = value;
        Assert.Equal(value, item.Enabled);
        Assert.Equal(value, item.Selected);

        // Set same.
        item.Enabled = value;
        Assert.Equal(value, item.Enabled);
        Assert.Equal(value, item.Selected);

        // Set different.
        item.Enabled = !value;
        Assert.Equal(!value, item.Enabled);
        Assert.False(item.Selected);
    }

    [WinFormsFact]
    public void ToolStripItem_Enabled_SetPressed_GetReturnsExpected()
    {
        using SubToolStripItem item = new();

        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            item.Enabled = false;
            Assert.False(item.Pressed);
            callCount++;
        };

        // Call with handler.
        item.Click += handler;
        item.PerformClick();
        Assert.Equal(1, callCount);
    }

    [WinFormsTheory]
    [MemberData(nameof(Enabled_Set_TestData))]
    public void ToolStripItem_Enabled_SetDesignMode_GetReturnsExpected(bool visible, Image image, bool value)
    {
        Mock<ISite> mockSite = new(MockBehavior.Strict);
        mockSite
            .Setup(s => s.Name)
            .Returns("Name");
        mockSite
            .Setup(s => s.DesignMode)
            .Returns(true);
        mockSite
            .Setup(s => s.Container)
            .Returns((IContainer)null);
        using SubToolStripItem item = new()
        {
            Visible = visible,
            Image = image,
            Site = mockSite.Object,
            Enabled = value
        };
        Assert.Equal(value, item.Enabled);

        // Set same.
        item.Enabled = value;
        Assert.Equal(value, item.Enabled);

        // Set different.
        item.Enabled = !value;
        Assert.Equal(!value, item.Enabled);
    }

    [WinFormsTheory]
    [MemberData(nameof(Enabled_Set_TestData))]
    public void ToolStripItem_Enabled_SetWithOwner_GetReturnsExpected(bool visible, Image image, bool value)
    {
        using ToolStrip owner = new();
        using SubToolStripItem item = new()
        {
            Visible = visible,
            Image = image,
            Owner = owner
        };

        item.Enabled = value;
        Assert.Equal(value, item.Enabled);
        Assert.False(owner.IsHandleCreated);

        // Set same.
        item.Enabled = value;
        Assert.Equal(value, item.Enabled);
        Assert.False(owner.IsHandleCreated);

        // Set different.
        item.Enabled = !value;
        Assert.Equal(!value, item.Enabled);
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(Enabled_Set_TestData))]
    public void ToolStripItem_Enabled_SetWithImageWithOwner_GetReturnsExpected(bool visible, Image image, bool value)
    {
        using ToolStrip owner = new();
        using SubToolStripItem item = new()
        {
            Visible = visible,
            Image = image,
            Owner = owner
        };

        item.Enabled = value;
        Assert.Equal(value, item.Enabled);
        Assert.False(owner.IsHandleCreated);

        // Set same.
        item.Enabled = value;
        Assert.Equal(value, item.Enabled);
        Assert.False(owner.IsHandleCreated);

        // Set different.
        item.Enabled = !value;
        Assert.Equal(!value, item.Enabled);
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(Enabled_Set_TestData))]
    public void ToolStripItem_Enabled_SetDesignModeWithOwner_GetReturnsExpected(bool visible, Image image, bool value)
    {
        Mock<ISite> mockSite = new(MockBehavior.Strict);
        mockSite
            .Setup(s => s.Name)
            .Returns("Name");
        mockSite
            .Setup(s => s.DesignMode)
            .Returns(true);
        mockSite
            .Setup(s => s.Container)
            .Returns((IContainer)null);
        using ToolStrip owner = new();
        using SubToolStripItem item = new()
        {
            Visible = visible,
            Image = image,
            Site = mockSite.Object,
            Owner = owner
        };

        item.Enabled = value;
        Assert.Equal(value, item.Enabled);
        Assert.False(owner.IsHandleCreated);

        // Set same.
        item.Enabled = value;
        Assert.Equal(value, item.Enabled);
        Assert.False(owner.IsHandleCreated);

        // Set different.
        item.Enabled = !value;
        Assert.Equal(!value, item.Enabled);
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(Enabled_Set_TestData))]
    public void ToolStripItem_Enabled_SetWithOwnerWithHandle_GetReturnsExpected(bool visible, Image image, bool value)
    {
        using ToolStrip owner = new();
        using SubToolStripItem item = new()
        {
            Visible = visible,
            Image = image,
            Owner = owner
        };
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;

        item.Enabled = value;
        Assert.Equal(value, item.Enabled);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        item.Enabled = value;
        Assert.Equal(value, item.Enabled);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set different.
        item.Enabled = !value;
        Assert.Equal(!value, item.Enabled);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [MemberData(nameof(Enabled_Set_TestData))]
    public void ToolStripItem_Enabled_SetWithParent_GetReturnsExpected(bool visible, Image image, bool value)
    {
        using ToolStrip parent = new();
        using SubToolStripItem item = new()
        {
            Visible = visible,
            Image = image,
            Parent = parent
        };

        item.Enabled = value;
        Assert.Equal(value, item.Enabled);
        Assert.False(parent.IsHandleCreated);

        // Set same.
        item.Enabled = value;
        Assert.Equal(value, item.Enabled);
        Assert.False(parent.IsHandleCreated);

        // Set different.
        item.Enabled = !value;
        Assert.Equal(!value, item.Enabled);
        Assert.False(parent.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(Enabled_Set_TestData))]
    public void ToolStripItem_Enabled_SetWithImageWithParent_GetReturnsExpected(bool visible, Image image, bool value)
    {
        using ToolStrip parent = new();
        using SubToolStripItem item = new()
        {
            Visible = visible,
            Image = image,
            Parent = parent
        };

        item.Enabled = value;
        Assert.Equal(value, item.Enabled);
        Assert.False(parent.IsHandleCreated);

        // Set same.
        item.Enabled = value;
        Assert.Equal(value, item.Enabled);
        Assert.False(parent.IsHandleCreated);

        // Set different.
        item.Enabled = !value;
        Assert.Equal(!value, item.Enabled);
        Assert.False(parent.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(Enabled_Set_TestData))]
    public void ToolStripItem_Enabled_SetDesignModeWithParent_GetReturnsExpected(bool visible, Image image, bool value)
    {
        Mock<ISite> mockSite = new(MockBehavior.Strict);
        mockSite
            .Setup(s => s.Name)
            .Returns("Name");
        mockSite
            .Setup(s => s.DesignMode)
            .Returns(true);
        mockSite
            .Setup(s => s.Container)
            .Returns((IContainer)null);
        using ToolStrip parent = new();
        using SubToolStripItem item = new()
        {
            Visible = visible,
            Image = image,
            Site = mockSite.Object,
            Parent = parent
        };

        item.Enabled = value;
        Assert.Equal(value, item.Enabled);
        Assert.False(parent.IsHandleCreated);

        // Set same.
        item.Enabled = value;
        Assert.Equal(value, item.Enabled);
        Assert.False(parent.IsHandleCreated);

        // Set different.
        item.Enabled = !value;
        Assert.Equal(!value, item.Enabled);
        Assert.False(parent.IsHandleCreated);
    }

    public static IEnumerable<object[]> Enabled_SetWithParentWithHandle_TestData()
    {
        foreach (bool visible in new bool[] { true, false })
        {
            foreach (Image image in new Image[] { null, new Bitmap(10, 10) })
            {
                yield return new object[] { visible, image, true, 0 };
                yield return new object[] { visible, image, false, 1 };
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(Enabled_SetWithParentWithHandle_TestData))]
    public void ToolStripItem_Enabled_SetWithParentWithHandle_GetReturnsExpected(bool visible, Image image, bool value, int expectedInvalidatedCallCount)
    {
        using ToolStrip parent = new();
        using SubToolStripItem item = new()
        {
            Visible = visible,
            Image = image,
            Parent = parent
        };
        Assert.NotEqual(IntPtr.Zero, parent.Handle);
        int invalidatedCallCount = 0;
        parent.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        parent.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        parent.HandleCreated += (sender, e) => createdCallCount++;

        item.Enabled = value;
        Assert.Equal(value, item.Enabled);
        Assert.True(parent.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        item.Enabled = value;
        Assert.Equal(value, item.Enabled);
        Assert.True(parent.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set different.
        item.Enabled = !value;
        Assert.Equal(!value, item.Enabled);
        Assert.True(parent.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount + 1, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void ToolStripItem_Enabled_SetWithHandler_CallsEnabledChanged()
    {
        using SubToolStripItem item = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(item, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        item.EnabledChanged += handler;

        // Set different.
        item.Enabled = false;
        Assert.False(item.Enabled);
        Assert.Equal(1, callCount);

        // Set same.
        item.Enabled = false;
        Assert.False(item.Enabled);
        Assert.Equal(1, callCount);

        // Set different.
        item.Enabled = true;
        Assert.True(item.Enabled);
        Assert.Equal(2, callCount);

        // Remove handler.
        item.EnabledChanged -= handler;
        item.Enabled = false;
        Assert.Equal(2, callCount);
    }

    [WinFormsFact]
    public void ToolStripItem_Font_GetWithParent_ReturnsExpected()
    {
        using Font font = new("Arial", 8.25f);
        using ToolStrip parent = new()
        {
            Font = font
        };
        using SubToolStripItem item = new()
        {
            Parent = parent
        };
        Assert.NotNull(item.Font);
        Assert.NotSame(font, item.Font);
        Assert.NotSame(Control.DefaultFont, item.Font);
        Assert.Same(item.Font, item.Font);
    }

    [WinFormsFact]
    public void ToolStripItem_Font_GetWithOwner_ReturnsExpected()
    {
        using Font font = new("Arial", 8.25f);
        using ToolStrip owner = new()
        {
            Font = font
        };
        using SubToolStripItem item = new()
        {
            Owner = owner
        };
        Assert.Same(font, item.Font);
    }

    public static IEnumerable<object[]> Font_Set_TestData()
    {
        foreach (Enum displayStyle in Enum.GetValues(typeof(ToolStripItemDisplayStyle)))
        {
            yield return new object[] { displayStyle, null };
            yield return new object[] { displayStyle, new Font("Arial", 8.25f) };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(Font_Set_TestData))]
    public void ToolStripItem_Font_Set_GetReturnsExpected(ToolStripItemDisplayStyle displayStyle, Font value)
    {
        using SubToolStripItem item = new()
        {
            DisplayStyle = displayStyle
        };

        item.Font = value;
        Assert.Equal(value ?? new SubToolStripItem().Font, item.Font);

        // Set same.
        item.Font = value;
        Assert.Equal(value ?? new SubToolStripItem().Font, item.Font);
    }

    public static IEnumerable<object[]> Font_SetWithOwner_TestData()
    {
        foreach (Enum displayStyle in Enum.GetValues(typeof(ToolStripItemDisplayStyle)))
        {
            yield return new object[] { displayStyle, null, 0 };
        }

        yield return new object[] { ToolStripItemDisplayStyle.None, new Font("Arial", 8.25f), 0 };
        yield return new object[] { ToolStripItemDisplayStyle.Text, new Font("Arial", 8.25f), 1 };
        yield return new object[] { ToolStripItemDisplayStyle.Image, new Font("Arial", 8.25f), 0 };
        yield return new object[] { ToolStripItemDisplayStyle.ImageAndText, new Font("Arial", 8.25f), 1 };
    }

    [WinFormsTheory]
    [MemberData(nameof(Font_SetWithOwner_TestData))]
    public void ToolStripItem_Font_SetWithOwner_GetReturnsExpected(ToolStripItemDisplayStyle displayStyle, Font value, int expectedOwnerLayoutCallCount)
    {
        using ToolStrip owner = new();
        using SubToolStripItem item = new()
        {
            Owner = owner,
            DisplayStyle = displayStyle
        };

        int ownerLayoutCallCount = 0;
        void ownerHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(owner, sender);
            Assert.Same(item, e.AffectedComponent);
            Assert.Equal("Font", e.AffectedProperty);
            ownerLayoutCallCount++;
        }

        owner.Layout += ownerHandler;

        try
        {
            item.Font = value;
            Assert.Equal(value ?? new SubToolStripItem().Font, item.Font);
            Assert.Equal(expectedOwnerLayoutCallCount, ownerLayoutCallCount);
            Assert.False(owner.IsHandleCreated);

            // Set same.
            item.Font = value;
            Assert.Equal(value ?? new SubToolStripItem().Font, item.Font);
            Assert.Equal(expectedOwnerLayoutCallCount, ownerLayoutCallCount);
            Assert.False(owner.IsHandleCreated);
        }
        finally
        {
            owner.Layout -= ownerHandler;
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(Font_SetWithOwner_TestData))]
    public void ToolStripItem_Font_SetWithOwnerWithHandle_GetReturnsExpected(ToolStripItemDisplayStyle displayStyle, Font value, int expectedOwnerLayoutCallCount)
    {
        using ToolStrip owner = new();
        using SubToolStripItem item = new()
        {
            Owner = owner,
            DisplayStyle = displayStyle
        };
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;
        int ownerLayoutCallCount = 0;
        void ownerHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(owner, sender);
            Assert.Same(item, e.AffectedComponent);
            Assert.Equal("Font", e.AffectedProperty);
            ownerLayoutCallCount++;
        }

        owner.Layout += ownerHandler;

        try
        {
            item.Font = value;
            Assert.Equal(value ?? new SubToolStripItem().Font, item.Font);
            Assert.Equal(expectedOwnerLayoutCallCount, ownerLayoutCallCount);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(expectedOwnerLayoutCallCount * 2, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            item.Font = value;
            Assert.Equal(value ?? new SubToolStripItem().Font, item.Font);
            Assert.Equal(expectedOwnerLayoutCallCount, ownerLayoutCallCount);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(expectedOwnerLayoutCallCount * 2, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }
        finally
        {
            owner.Layout -= ownerHandler;
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(Font_Set_TestData))]
    public void ToolStripItem_Font_SetWithParent_GetReturnsExpected(ToolStripItemDisplayStyle displayStyle, Font value)
    {
        using ToolStrip parent = new();
        using SubToolStripItem item = new()
        {
            Parent = parent,
            DisplayStyle = displayStyle
        };

        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e) => parentLayoutCallCount++;
        parent.Layout += parentHandler;

        try
        {
            item.Font = value;
            Assert.Equal(value ?? new SubToolStripItem().Font, item.Font);
            Assert.Equal(0, parentLayoutCallCount);
            Assert.False(parent.IsHandleCreated);

            // Set same.
            item.Font = value;
            Assert.Equal(value ?? new SubToolStripItem().Font, item.Font);
            Assert.Equal(0, parentLayoutCallCount);
            Assert.False(parent.IsHandleCreated);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(Font_Set_TestData))]
    public void ToolStripItem_Font_SetWithParentWithHandle_GetReturnsExpected(ToolStripItemDisplayStyle displayStyle, Font value)
    {
        using ToolStrip parent = new();
        using SubToolStripItem item = new()
        {
            Parent = parent,
            DisplayStyle = displayStyle
        };
        Assert.NotEqual(IntPtr.Zero, parent.Handle);
        int invalidatedCallCount = 0;
        parent.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        parent.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        parent.HandleCreated += (sender, e) => createdCallCount++;
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e) => parentLayoutCallCount++;
        parent.Layout += parentHandler;

        try
        {
            item.Font = value;
            Assert.Equal(value ?? new SubToolStripItem().Font, item.Font);
            Assert.Equal(0, parentLayoutCallCount);
            Assert.True(parent.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            item.Font = value;
            Assert.Equal(value ?? new SubToolStripItem().Font, item.Font);
            Assert.Equal(0, parentLayoutCallCount);
            Assert.True(parent.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    [WinFormsFact]
    public void ToolStripItem_Font_ResetValue_Success()
    {
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ToolStripItem))[nameof(ToolStripItem.Font)];
        using SubToolStripItem item = new();
        Assert.False(property.CanResetValue(item));

        using Font font = new("Arial", 8.25f);
        item.Font = font;
        Assert.Same(font, item.Font);
        Assert.True(property.CanResetValue(item));

        item.Font = null;
        Assert.NotNull(item.Font);
        Assert.NotSame(Control.DefaultFont, item.Font);
        Assert.Same(item.Font, item.Font);
        Assert.False(property.CanResetValue(item));

        item.Font = font;
        Assert.Same(font, item.Font);
        Assert.True(property.CanResetValue(item));

        property.ResetValue(item);
        Assert.NotNull(item.Font);
        Assert.NotSame(Control.DefaultFont, item.Font);
        Assert.Same(item.Font, item.Font);
        Assert.False(property.CanResetValue(item));
    }

    [WinFormsFact]
    public void ToolStripItem_Font_ShouldSerializeValue_Success()
    {
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ToolStripItem))[nameof(ToolStripItem.Font)];
        using SubToolStripItem item = new();
        Assert.False(property.ShouldSerializeValue(item));

        using Font font = new("Arial", 8.25f);
        item.Font = font;
        Assert.Same(font, item.Font);
        Assert.True(property.ShouldSerializeValue(item));

        item.Font = null;
        Assert.NotNull(item.Font);
        Assert.NotSame(Control.DefaultFont, item.Font);
        Assert.Same(item.Font, item.Font);
        Assert.False(property.ShouldSerializeValue(item));

        item.Font = font;
        Assert.Same(font, item.Font);
        Assert.True(property.ShouldSerializeValue(item));

        property.ResetValue(item);
        Assert.NotNull(item.Font);
        Assert.NotSame(Control.DefaultFont, item.Font);
        Assert.Same(item.Font, item.Font);
        Assert.False(property.ShouldSerializeValue(item));
    }

    [WinFormsFact]
    public void ToolStripItem_ForeColor_GetWithOwner_ReturnsExpected()
    {
        using ToolStrip owner = new()
        {
            ForeColor = Color.Red
        };
        using SubToolStripItem item = new()
        {
            Owner = owner
        };
        Assert.Equal(Control.DefaultForeColor, item.ForeColor);
    }

    [WinFormsFact]
    public void ToolStripItem_ForeColor_GetWithParent_ReturnsExpected()
    {
        using ToolStrip parent = new()
        {
            ForeColor = Color.Red
        };
        using SubToolStripItem item = new()
        {
            Parent = parent
        };
        Assert.Equal(Color.Red, item.ForeColor);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetForeColorTheoryData))]
    public void ToolStripItem_ForeColor_Set_GetReturnsExpected(Color value, Color expected)
    {
        using SubToolStripItem item = new()
        {
            ForeColor = value
        };
        Assert.Equal(expected, item.ForeColor);

        // Set same.
        item.ForeColor = value;
        Assert.Equal(expected, item.ForeColor);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetForeColorTheoryData))]
    public void ToolStripItem_ForeColor_SetWithOwner_GetReturnsExpected(Color value, Color expected)
    {
        using ToolStrip owner = new();
        using SubToolStripItem item = new()
        {
            Owner = owner
        };

        item.ForeColor = value;
        Assert.Equal(expected, item.ForeColor);
        Assert.False(owner.IsHandleCreated);

        // Set same.
        item.ForeColor = value;
        Assert.Equal(expected, item.ForeColor);
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetForeColorTheoryData))]
    public void ToolStripItem_ForeColor_SetWithOwnerWithHandle_GetReturnsExpected(Color value, Color expected)
    {
        using ToolStrip owner = new();
        using SubToolStripItem item = new()
        {
            Owner = owner
        };
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;

        item.ForeColor = value;
        Assert.Equal(expected, item.ForeColor);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        item.ForeColor = value;
        Assert.Equal(expected, item.ForeColor);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetForeColorTheoryData))]
    public void ToolStripItem_ForeColor_SetWithParent_GetReturnsExpected(Color value, Color expected)
    {
        using ToolStrip parent = new();
        using SubToolStripItem item = new()
        {
            Parent = parent
        };

        item.ForeColor = value;
        Assert.Equal(expected, item.ForeColor);
        Assert.False(parent.IsHandleCreated);

        // Set same.
        item.ForeColor = value;
        Assert.Equal(expected, item.ForeColor);
        Assert.False(parent.IsHandleCreated);
    }

    public static IEnumerable<object[]> ForeColor_SetWithParentWithHandle_TestData()
    {
        yield return new object[] { Color.Empty, Control.DefaultForeColor, 0 };
        yield return new object[] { Color.FromArgb(254, 1, 2, 3), Color.FromArgb(254, 1, 2, 3), 1 };
        yield return new object[] { Color.White, Color.White, 1 };
        yield return new object[] { Color.Black, Color.Black, 1 };
        yield return new object[] { Color.Red, Color.Red, 1 };
    }

    [WinFormsTheory]
    [MemberData(nameof(ForeColor_SetWithParentWithHandle_TestData))]
    public void ToolStripItem_ForeColor_SetWithParentWithHandle_GetReturnsExpected(Color value, Color expected, int expectedInvalidatedCallCount)
    {
        using ToolStrip parent = new();
        using SubToolStripItem item = new()
        {
            Parent = parent
        };
        Assert.NotEqual(IntPtr.Zero, parent.Handle);
        int invalidatedCallCount = 0;
        parent.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        parent.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        parent.HandleCreated += (sender, e) => createdCallCount++;

        item.ForeColor = value;
        Assert.Equal(expected, item.ForeColor);
        Assert.True(parent.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        item.ForeColor = value;
        Assert.Equal(expected, item.ForeColor);
        Assert.True(parent.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void ToolStripItem_ForeColor_SetWithHandler_CallsForeColorChanged()
    {
        using SubToolStripItem item = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(item, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        item.ForeColorChanged += handler;

        // Set different.
        item.ForeColor = Color.Red;
        Assert.Equal(Color.Red, item.ForeColor);
        Assert.Equal(1, callCount);

        // Set same.
        item.ForeColor = Color.Red;
        Assert.Equal(Color.Red, item.ForeColor);
        Assert.Equal(1, callCount);

        // Set different.
        item.ForeColor = Color.Empty;
        Assert.Equal(Control.DefaultForeColor, item.ForeColor);
        Assert.Equal(2, callCount);

        // Remove handler.
        item.ForeColorChanged -= handler;
        item.ForeColor = Color.Red;
        Assert.Equal(Color.Red, item.ForeColor);
        Assert.Equal(2, callCount);
    }

    [WinFormsFact]
    public void ToolStripItem_ForeColor_ResetValue_Success()
    {
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ToolStripItem))[nameof(ToolStripItem.ForeColor)];
        using SubToolStripItem item = new();
        Assert.False(property.CanResetValue(item));

        item.ForeColor = Color.Red;
        Assert.Equal(Color.Red, item.ForeColor);
        Assert.True(property.CanResetValue(item));

        property.ResetValue(item);
        Assert.Equal(Control.DefaultForeColor, item.ForeColor);
        Assert.False(property.CanResetValue(item));
    }

    [WinFormsFact]
    public void ToolStripItem_ForeColor_ShouldSerializeValue_Success()
    {
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ToolStripItem))[nameof(ToolStripItem.ForeColor)];
        using SubToolStripItem item = new();
        Assert.False(property.ShouldSerializeValue(item));

        item.ForeColor = Color.Red;
        Assert.Equal(Color.Red, item.ForeColor);
        Assert.True(property.ShouldSerializeValue(item));

        property.ResetValue(item);
        Assert.Equal(Control.DefaultForeColor, item.ForeColor);
        Assert.False(property.ShouldSerializeValue(item));
    }

    [WinFormsTheory]
    [IntegerData<int>]
    public void ToolStripItem_Height_Set_GetReturnsExpected(int value)
    {
        using SubToolStripItem item = new();
        int locationChangedCallCount = 0;
        item.LocationChanged += (sender, e) => locationChangedCallCount++;

        item.Height = value;
        Assert.Equal(value, item.Height);
        Assert.Equal(0, locationChangedCallCount);

        // Set same.
        item.Height = value;
        Assert.Equal(value, item.Height);
        Assert.Equal(0, locationChangedCallCount);
    }

    [WinFormsTheory]
    [IntegerData<int>]
    public void ToolStripItem_Height_SetWithOwner_GetReturnsExpected(int value)
    {
        using ToolStrip owner = new();
        using SubToolStripItem item = new()
        {
            Owner = owner
        };
        int ownerLayoutCallCount = 0;
        void ownerHandler(object sender, LayoutEventArgs e) => ownerLayoutCallCount++;
        owner.Layout += ownerHandler;
        int locationChangedCallCount = 0;
        item.LocationChanged += (sender, e) => locationChangedCallCount++;

        try
        {
            item.Height = value;
            Assert.Equal(value, item.Height);
            Assert.Equal(0, locationChangedCallCount);
            Assert.Equal(0, ownerLayoutCallCount);
            Assert.False(owner.IsHandleCreated);

            // Set same.
            item.Height = value;
            Assert.Equal(value, item.Height);
            Assert.Equal(0, locationChangedCallCount);
            Assert.Equal(0, ownerLayoutCallCount);
            Assert.False(owner.IsHandleCreated);
        }
        finally
        {
            owner.Layout -= ownerHandler;
        }
    }

    [WinFormsTheory]
    [IntegerData<int>]
    public void ToolStripItem_Height_SetWithOwnerWithHandle_GetReturnsExpected(int value)
    {
        using ToolStrip owner = new();
        using SubToolStripItem item = new()
        {
            Owner = owner
        };
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;
        int ownerLayoutCallCount = 0;
        void ownerHandler(object sender, LayoutEventArgs e) => ownerLayoutCallCount++;
        owner.Layout += ownerHandler;
        int locationChangedCallCount = 0;
        item.LocationChanged += (sender, e) => locationChangedCallCount++;

        try
        {
            item.Height = value;
            Assert.Equal(value, item.Height);
            Assert.Equal(0, locationChangedCallCount);
            Assert.Equal(0, ownerLayoutCallCount);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            item.Height = value;
            Assert.Equal(value, item.Height);
            Assert.Equal(0, locationChangedCallCount);
            Assert.Equal(0, ownerLayoutCallCount);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }
        finally
        {
            owner.Layout -= ownerHandler;
        }
    }

    [WinFormsTheory]
    [InlineData(-1, 1)]
    [InlineData(0, 1)]
    [InlineData(1, 1)]
    [InlineData(23, 0)]
    public void ToolStripItem_Height_SetWithParent_GetReturnsExpected(int value, int expectedParentLayoutCallCount)
    {
        using ToolStrip parent = new();
        using SubToolStripItem item = new()
        {
            Parent = parent
        };
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(parent, sender);
            Assert.Same(item, e.AffectedComponent);
            Assert.Equal("Bounds", e.AffectedProperty);
            parentLayoutCallCount++;
        }

        parent.Layout += parentHandler;
        int locationChangedCallCount = 0;
        item.LocationChanged += (sender, e) => locationChangedCallCount++;

        try
        {
            item.Height = value;
            Assert.Equal(value, item.Height);
            Assert.Equal(0, locationChangedCallCount);
            Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
            Assert.False(parent.IsHandleCreated);

            // Set same.
            item.Height = value;
            Assert.Equal(value, item.Height);
            Assert.Equal(0, locationChangedCallCount);
            Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
            Assert.False(parent.IsHandleCreated);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    [WinFormsTheory]
    [InlineData(-1, 1)]
    [InlineData(0, 1)]
    [InlineData(1, 1)]
    [InlineData(23, 0)]
    public void ToolStripItem_Height_SetWithParentWithHandle_GetReturnsExpected(int value, int expectedParentLayoutCallCount)
    {
        using ToolStrip parent = new();
        using SubToolStripItem item = new()
        {
            Parent = parent
        };
        Assert.NotEqual(IntPtr.Zero, parent.Handle);
        int invalidatedCallCount = 0;
        parent.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        parent.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        parent.HandleCreated += (sender, e) => createdCallCount++;
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(parent, sender);
            Assert.Same(item, e.AffectedComponent);
            Assert.Equal("Bounds", e.AffectedProperty);
            parentLayoutCallCount++;
        }

        parent.Layout += parentHandler;
        int locationChangedCallCount = 0;
        item.LocationChanged += (sender, e) => locationChangedCallCount++;

        try
        {
            item.Height = value;
            Assert.Equal(value, item.Height);
            Assert.Equal(0, locationChangedCallCount);
            Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
            Assert.True(parent.IsHandleCreated);
            Assert.Equal(expectedParentLayoutCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            item.Height = value;
            Assert.Equal(value, item.Height);
            Assert.Equal(0, locationChangedCallCount);
            Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
            Assert.True(parent.IsHandleCreated);
            Assert.Equal(expectedParentLayoutCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    public static IEnumerable<object[]> Image_GetWithOwner_TestData()
    {
        yield return new object[] { null };
        yield return new object[] { new ImageList() };
    }

    [WinFormsTheory]
    [MemberData(nameof(Image_GetWithOwner_TestData))]
    public void ToolStripItem_Image_GetWithOwner_ReturnsNull(ImageList imageList)
    {
        using ToolStrip owner = new()
        {
            ImageList = imageList
        };
        using SubToolStripItem item = new()
        {
            Owner = owner
        };
        Assert.Null(item.Image);

        // Get again to test caching behavior.
        Assert.Null(item.Image);
    }

    [WinFormsFact]
    public void ToolStripItem_Image_GetWithValidImageIndexWithOwner_ReturnsExpected()
    {
        using Bitmap image = new(10, 10);
        using ImageList imageList = new();
        imageList.Images.Add(image);
        using ToolStrip owner = new()
        {
            ImageList = imageList
        };
        using SubToolStripItem item = new()
        {
            ImageIndex = 0,
            Owner = owner
        };
        Assert.NotNull(item.Image);

        // Get again to test caching behavior.
        Assert.Same(item.Image, item.Image);
    }

    [WinFormsFact]
    public void ToolStripItem_Image_GetWithInvalidImageIndexWithOwner_ReturnsExpected()
    {
        using Bitmap image = new(10, 10);
        using ImageList imageList = new();
        imageList.Images.Add(image);
        using ToolStrip owner = new()
        {
            ImageList = imageList
        };
        using SubToolStripItem item = new()
        {
            ImageIndex = 1,
            Owner = owner
        };
        Assert.Null(item.Image);

        // Get again to test caching behavior.
        Assert.Null(item.Image);
    }

    public static IEnumerable<object[]> Image_GetWithParent_TestData()
    {
        yield return new object[] { null };
        yield return new object[] { new ImageList() };
    }

    [WinFormsTheory]
    [MemberData(nameof(Image_GetWithParent_TestData))]
    public void ToolStripItem_Image_GetWithParent_ReturnsNull(ImageList imageList)
    {
        using ToolStrip parent = new()
        {
            ImageList = imageList
        };
        using SubToolStripItem item = new()
        {
            Parent = parent
        };
        Assert.Null(item.Image);

        // Get again to test caching behavior.
        Assert.Null(item.Image);
    }

    [WinFormsTheory]
    [InlineData(0)]
    [InlineData(1)]
    public void ToolStripItem_Image_GetWithImageIndexWithParent_ReturnsExpected(int imageIndex)
    {
        using Bitmap image = new(10, 10);
        using ImageList imageList = new();
        imageList.Images.Add(image);
        using ToolStrip parent = new()
        {
            ImageList = imageList
        };
        using SubToolStripItem item = new()
        {
            ImageIndex = imageIndex,
            Parent = parent
        };
        Assert.Null(item.Image);

        // Get again to test caching behavior.
        Assert.Null(item.Image);
    }

    public static IEnumerable<object[]> Image_Set_TestData()
    {
        foreach (Color imageTransparentColor in new Color[] { Color.Empty, Color.Red })
        {
            yield return new object[] { imageTransparentColor, null };
            yield return new object[] { imageTransparentColor, new Bitmap(10, 10) };
            yield return new object[] { imageTransparentColor, Image.FromFile(Path.Combine("bitmaps", "nature24bits.gif")) };
            yield return new object[] { imageTransparentColor, Image.FromFile(Path.Combine("bitmaps", "10x16_one_entry_32bit.ico")) };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(Image_Set_TestData))]
    public void ToolStripItem_Image_Set_GetReturnsExpected(Color imageTransparentColor, Image value)
    {
        using SubToolStripItem item = new()
        {
            ImageTransparentColor = imageTransparentColor
        };

        item.Image = value;
        Assert.Equal(value, item.Image);
        Assert.Equal(-1, item.ImageIndex);

        // Set same.
        item.Image = value;
        Assert.Equal(value, item.Image);
        Assert.Equal(-1, item.ImageIndex);
    }

    public static IEnumerable<object[]> Image_SetWithImageIndex_TestData()
    {
        foreach (Color imageTransparentColor in new Color[] { Color.Empty, Color.Red })
        {
            yield return new object[] { imageTransparentColor, null, 1 };
            yield return new object[] { imageTransparentColor, new Bitmap(10, 10), -1 };
            yield return new object[] { imageTransparentColor, Image.FromFile(Path.Combine("bitmaps", "nature24bits.gif")), -1 };
            yield return new object[] { imageTransparentColor, Image.FromFile(Path.Combine("bitmaps", "10x16_one_entry_32bit.ico")), -1 };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(Image_SetWithImageIndex_TestData))]
    public void ToolStripItem_Image_SetWithImageIndex_GetReturnsExpected(Color imageTransparentColor, Image value, int expectedImageIndex)
    {
        using SubToolStripItem item = new()
        {
            ImageTransparentColor = imageTransparentColor,
            ImageIndex = 1
        };

        item.Image = value;
        Assert.Equal(value, item.Image);
        Assert.Equal(expectedImageIndex, item.ImageIndex);

        // Set same.
        item.Image = value;
        Assert.Equal(value, item.Image);
        Assert.Equal(expectedImageIndex, item.ImageIndex);
    }

    [WinFormsTheory]
    [MemberData(nameof(Image_Set_TestData))]
    public void ToolStripItem_Image_SetWithNonNullOldValue_GetReturnsExpected(Color imageTransparentColor, Image value)
    {
        using Bitmap oldValue = new(10, 10);
        using SubToolStripItem item = new()
        {
            Image = oldValue,
            ImageTransparentColor = imageTransparentColor
        };

        item.Image = value;
        Assert.Equal(value, item.Image);
        Assert.Equal(-1, item.ImageIndex);

        // Set same.
        item.Image = value;
        Assert.Equal(value, item.Image);
        Assert.Equal(-1, item.ImageIndex);
    }

    [WinFormsTheory]
    [MemberData(nameof(Image_SetWithImageIndex_TestData))]
    public void ToolStripItem_Image_SetWithNonNullOldValueWithImageIndex_GetReturnsExpected(Color imageTransparentColor, Image value, int expectedImageIndex)
    {
        using Bitmap oldValue = new(10, 10);
        using SubToolStripItem item = new()
        {
            Image = oldValue,
            ImageTransparentColor = imageTransparentColor,
            ImageIndex = 1
        };

        item.Image = value;
        Assert.Equal(value, item.Image);
        Assert.Equal(expectedImageIndex, item.ImageIndex);

        // Set same.
        item.Image = value;
        Assert.Equal(value, item.Image);
        Assert.Equal(expectedImageIndex, item.ImageIndex);
    }

    public static IEnumerable<object[]> Image_SetWithOwner_TestData()
    {
        foreach (Color imageTransparentColor in new Color[] { Color.Empty, Color.Red })
        {
            yield return new object[] { imageTransparentColor, null, 0 };
            yield return new object[] { imageTransparentColor, new Bitmap(10, 10), 1 };
            yield return new object[] { imageTransparentColor, Image.FromFile(Path.Combine("bitmaps", "nature24bits.gif")), 1 };
            yield return new object[] { imageTransparentColor, Image.FromFile(Path.Combine("bitmaps", "10x16_one_entry_32bit.ico")), 1 };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(Image_SetWithOwner_TestData))]
    public void ToolStripItem_Image_SetWithOwner_GetReturnsExpected(Color imageTransparentColor, Image value, int expectedOwnerLayoutCallCount)
    {
        using ToolStrip owner = new();
        using SubToolStripItem item = new()
        {
            ImageTransparentColor = imageTransparentColor,
            Owner = owner
        };
        int ownerLayoutCallCount = 0;
        void ownerHandler(object sender, LayoutEventArgs e)
        {
            if (e.AffectedProperty != "ImageIndex")
            {
                Assert.Same(owner, sender);
                Assert.Same(item, e.AffectedComponent);
                Assert.Equal("Image", e.AffectedProperty);
                ownerLayoutCallCount++;
            }
        }

        owner.Layout += ownerHandler;

        try
        {
            item.Image = value;
            Assert.Equal(value, item.Image);
            Assert.Equal(-1, item.ImageIndex);
            Assert.Equal(expectedOwnerLayoutCallCount, ownerLayoutCallCount);
            Assert.False(owner.IsHandleCreated);

            // Set same.
            item.Image = value;
            Assert.Equal(value, item.Image);
            Assert.Equal(-1, item.ImageIndex);
            Assert.Equal(expectedOwnerLayoutCallCount, ownerLayoutCallCount);
            Assert.False(owner.IsHandleCreated);
        }
        finally
        {
            owner.Layout -= ownerHandler;
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(Image_SetWithOwner_TestData))]
    public void ToolStripItem_Image_SetWithOwnerWithHandle_GetReturnsExpected(Color imageTransparentColor, Image value, int expectedOwnerLayoutCallCount)
    {
        using ToolStrip owner = new();
        using SubToolStripItem item = new()
        {
            ImageTransparentColor = imageTransparentColor,
            Owner = owner
        };
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;
        int ownerLayoutCallCount = 0;
        void ownerHandler(object sender, LayoutEventArgs e)
        {
            if (e.AffectedProperty != "ImageIndex")
            {
                Assert.Same(owner, sender);
                Assert.Same(item, e.AffectedComponent);
                Assert.Equal("Image", e.AffectedProperty);
                ownerLayoutCallCount++;
            }
        }

        owner.Layout += ownerHandler;

        try
        {
            item.Image = value;
            Assert.Equal(value, item.Image);
            Assert.Equal(-1, item.ImageIndex);
            Assert.Equal(expectedOwnerLayoutCallCount, ownerLayoutCallCount);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(expectedOwnerLayoutCallCount * 4, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            item.Image = value;
            Assert.Equal(value, item.Image);
            Assert.Equal(-1, item.ImageIndex);
            Assert.Equal(expectedOwnerLayoutCallCount, ownerLayoutCallCount);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(expectedOwnerLayoutCallCount * 4, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }
        finally
        {
            owner.Layout -= ownerHandler;
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(Image_Set_TestData))]
    public void ToolStripItem_Image_SetWithParent_GetReturnsExpected(Color imageTransparentColor, Image value)
    {
        using ToolStrip parent = new();
        using SubToolStripItem item = new()
        {
            ImageTransparentColor = imageTransparentColor,
            Parent = parent
        };
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e) => parentLayoutCallCount++;
        parent.Layout += parentHandler;

        try
        {
            item.Image = value;
            Assert.Equal(value, item.Image);
            Assert.Equal(-1, item.ImageIndex);
            Assert.Equal(0, parentLayoutCallCount);
            Assert.False(parent.IsHandleCreated);

            // Set same.
            item.Image = value;
            Assert.Equal(value, item.Image);
            Assert.Equal(-1, item.ImageIndex);
            Assert.Equal(0, parentLayoutCallCount);
            Assert.False(parent.IsHandleCreated);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(Image_Set_TestData))]
    public void ToolStripItem_Image_SetWithParentWithHandle_GetReturnsExpected(Color imageTransparentColor, Image value)
    {
        using ToolStrip parent = new();
        using SubToolStripItem item = new()
        {
            ImageTransparentColor = imageTransparentColor,
            Parent = parent
        };
        Assert.NotEqual(IntPtr.Zero, parent.Handle);
        int invalidatedCallCount = 0;
        parent.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        parent.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        parent.HandleCreated += (sender, e) => createdCallCount++;
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e) => parentLayoutCallCount++;
        parent.Layout += parentHandler;

        try
        {
            item.Image = value;
            Assert.Equal(value, item.Image);
            Assert.Equal(-1, item.ImageIndex);
            Assert.Equal(0, parentLayoutCallCount);
            Assert.True(parent.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            item.Image = value;
            Assert.Equal(value, item.Image);
            Assert.Equal(-1, item.ImageIndex);
            Assert.Equal(0, parentLayoutCallCount);
            Assert.True(parent.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    [WinFormsFact]
    public void ToolStripItem_Image_ResetValue_Success()
    {
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ToolStripItem))[nameof(ToolStripItem.Image)];
        using SubToolStripItem item = new();
        Assert.False(property.CanResetValue(item));

        using Bitmap image = new(10, 10);
        item.Image = image;
        Assert.Same(image, item.Image);
        Assert.True(property.CanResetValue(item));

        property.ResetValue(item);
        Assert.Null(item.Image);
        Assert.False(property.CanResetValue(item));
    }

    [WinFormsFact]
    public void ToolStripItem_Image_ResetValueWithOwnerWithImageList_Success()
    {
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ToolStripItem))[nameof(ToolStripItem.Image)];
        using Bitmap image = new(10, 10);
        using ImageList imageList = new();
        imageList.Images.Add(image);
        using ToolStrip owner = new()
        {
            ImageList = imageList
        };
        using SubToolStripItem item = new()
        {
            ImageIndex = 0,
            Owner = owner
        };
        Assert.False(property.CanResetValue(item));

        using Bitmap otherImage = new(10, 10);
        item.Image = otherImage;
        Assert.Same(otherImage, item.Image);
        Assert.True(property.CanResetValue(item));

        property.ResetValue(item);
        Assert.Null(item.Image);
        Assert.False(property.CanResetValue(item));
    }

    [WinFormsFact]
    public void ToolStripItem_Image_ShouldSerializeValue_Success()
    {
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ToolStripItem))[nameof(ToolStripItem.Image)];
        using SubToolStripItem item = new();
        Assert.False(property.ShouldSerializeValue(item));

        using Bitmap image = new(10, 10);
        item.Image = image;
        Assert.Same(image, item.Image);
        Assert.True(property.ShouldSerializeValue(item));

        property.ResetValue(item);
        Assert.Null(item.Image);
        Assert.False(property.ShouldSerializeValue(item));
    }

    [WinFormsFact]
    public void ToolStripItem_Image_ShouldSerializeValueWithOwnerWithImageList_Success()
    {
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ToolStripItem))[nameof(ToolStripItem.Image)];
        using Bitmap image = new(10, 10);
        using ImageList imageList = new();
        imageList.Images.Add(image);
        using ToolStrip owner = new()
        {
            ImageList = imageList
        };
        using SubToolStripItem item = new()
        {
            ImageIndex = 0,
            Owner = owner
        };
        Assert.False(property.ShouldSerializeValue(item));

        using Bitmap otherImage = new(10, 10);
        item.Image = otherImage;
        Assert.Same(otherImage, item.Image);
        Assert.True(property.ShouldSerializeValue(item));

        property.ResetValue(item);
        Assert.Null(item.Image);
        Assert.False(property.ShouldSerializeValue(item));
    }

    [WinFormsTheory]
    [EnumData<ContentAlignment>]
    public void ToolStripItem_ImageAlign_Set_GetReturnsExpected(ContentAlignment value)
    {
        using SubToolStripItem item = new()
        {
            ImageAlign = value
        };
        Assert.Equal(value, item.ImageAlign);

        // Set same.
        item.ImageAlign = value;
        Assert.Equal(value, item.ImageAlign);
    }

    [WinFormsTheory]
    [InlineData(ContentAlignment.TopLeft, 1)]
    [InlineData(ContentAlignment.TopCenter, 1)]
    [InlineData(ContentAlignment.TopRight, 1)]
    [InlineData(ContentAlignment.MiddleLeft, 1)]
    [InlineData(ContentAlignment.MiddleCenter, 0)]
    [InlineData(ContentAlignment.MiddleRight, 1)]
    [InlineData(ContentAlignment.BottomLeft, 1)]
    [InlineData(ContentAlignment.BottomCenter, 1)]
    [InlineData(ContentAlignment.BottomRight, 1)]
    public void ToolStripItem_ImageAlign_SetWithOwner_GetReturnsExpected(ContentAlignment value, int expectedParentLayoutCallCount)
    {
        using ToolStrip owner = new();
        using SubToolStripItem item = new()
        {
            Owner = owner
        };
        int ownerLayoutCallCount = 0;
        void ownerHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(owner, sender);
            Assert.Same(item, e.AffectedComponent);
            Assert.Equal("ImageAlign", e.AffectedProperty);
            ownerLayoutCallCount++;
        }

        owner.Layout += ownerHandler;

        try
        {
            item.ImageAlign = value;
            Assert.Equal(value, item.ImageAlign);
            Assert.Equal(expectedParentLayoutCallCount, ownerLayoutCallCount);
            Assert.False(owner.IsHandleCreated);

            // Set same.
            item.ImageAlign = value;
            Assert.Equal(value, item.ImageAlign);
            Assert.Equal(expectedParentLayoutCallCount, ownerLayoutCallCount);
            Assert.False(owner.IsHandleCreated);
        }
        finally
        {
            owner.Layout -= ownerHandler;
        }
    }

    [WinFormsTheory]
    [InlineData(ContentAlignment.TopLeft, 1)]
    [InlineData(ContentAlignment.TopCenter, 1)]
    [InlineData(ContentAlignment.TopRight, 1)]
    [InlineData(ContentAlignment.MiddleLeft, 1)]
    [InlineData(ContentAlignment.MiddleCenter, 0)]
    [InlineData(ContentAlignment.MiddleRight, 1)]
    [InlineData(ContentAlignment.BottomLeft, 1)]
    [InlineData(ContentAlignment.BottomCenter, 1)]
    [InlineData(ContentAlignment.BottomRight, 1)]
    public void ToolStripItem_ImageAlign_SetWithOwnerWithHandle_GetReturnsExpected(ContentAlignment value, int expectedParentLayoutCallCount)
    {
        using ToolStrip owner = new();
        using SubToolStripItem item = new()
        {
            Owner = owner
        };
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;
        int ownerLayoutCallCount = 0;
        void ownerHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(owner, sender);
            Assert.Same(item, e.AffectedComponent);
            Assert.Equal("ImageAlign", e.AffectedProperty);
            ownerLayoutCallCount++;
        }

        owner.Layout += ownerHandler;

        try
        {
            item.ImageAlign = value;
            Assert.Equal(value, item.ImageAlign);
            Assert.Equal(expectedParentLayoutCallCount, ownerLayoutCallCount);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(expectedParentLayoutCallCount * 2, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            item.ImageAlign = value;
            Assert.Equal(value, item.ImageAlign);
            Assert.Equal(expectedParentLayoutCallCount, ownerLayoutCallCount);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(expectedParentLayoutCallCount * 2, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }
        finally
        {
            owner.Layout -= ownerHandler;
        }
    }

    [WinFormsTheory]
    [EnumData<ContentAlignment>]
    public void ToolStripItem_ImageAlign_SetWithParent_GetReturnsExpected(ContentAlignment value)
    {
        using ToolStrip parent = new();
        using SubToolStripItem item = new()
        {
            Parent = parent
        };
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e) => parentLayoutCallCount++;
        parent.Layout += parentHandler;

        try
        {
            item.ImageAlign = value;
            Assert.Equal(value, item.ImageAlign);
            Assert.Equal(0, parentLayoutCallCount);
            Assert.False(parent.IsHandleCreated);

            // Set same.
            item.ImageAlign = value;
            Assert.Equal(value, item.ImageAlign);
            Assert.Equal(0, parentLayoutCallCount);
            Assert.False(parent.IsHandleCreated);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    [WinFormsTheory]
    [EnumData<ContentAlignment>]
    public void ToolStripItem_ImageAlign_SetWithParentWithHandle_GetReturnsExpected(ContentAlignment value)
    {
        using ToolStrip parent = new();
        using SubToolStripItem item = new()
        {
            Parent = parent
        };
        Assert.NotEqual(IntPtr.Zero, parent.Handle);
        int invalidatedCallCount = 0;
        parent.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        parent.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        parent.HandleCreated += (sender, e) => createdCallCount++;
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e) => parentLayoutCallCount++;
        parent.Layout += parentHandler;

        try
        {
            item.ImageAlign = value;
            Assert.Equal(value, item.ImageAlign);
            Assert.Equal(0, parentLayoutCallCount);
            Assert.True(parent.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            item.ImageAlign = value;
            Assert.Equal(value, item.ImageAlign);
            Assert.Equal(0, parentLayoutCallCount);
            Assert.True(parent.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    [WinFormsTheory]
    [InvalidEnumData<ContentAlignment>]
    [InlineData((ContentAlignment)int.MaxValue)]
    [InlineData((ContentAlignment)int.MinValue)]
    public void ToolStripItem_ImageAlign_SetInvalid_ThrowsInvalidEnumArgumentException(ContentAlignment value)
    {
        using SubToolStripItem item = new();
        Assert.Throws<InvalidEnumArgumentException>("value", () => item.ImageAlign = value);
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    public void ToolStripItem_ImageIndex_Set_GetReturnsExpected(int value)
    {
        using SubToolStripItem item = new()
        {
            ImageIndex = value
        };
        Assert.Equal(value, item.ImageIndex);
        Assert.Empty(item.ImageKey);
        Assert.Null(item.Image);

        // Set same.
        item.ImageIndex = value;
        Assert.Equal(value, item.ImageIndex);
        Assert.Empty(item.ImageKey);
        Assert.Null(item.Image);
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    public void ToolStripItem_ImageIndex_SetWithImage_GetReturnsExpected(int value)
    {
        using Bitmap image = new(10, 10);
        using SubToolStripItem item = new()
        {
            Image = image,
            ImageIndex = value
        };
        Assert.Equal(value, item.ImageIndex);
        Assert.Empty(item.ImageKey);
        Assert.Empty(item.ImageKey);
        Assert.Null(item.Image);

        // Set same.
        item.ImageIndex = value;
        Assert.Equal(value, item.ImageIndex);
        Assert.Empty(item.ImageKey);
        Assert.Empty(item.ImageKey);
        Assert.Null(item.Image);
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    public void ToolStripItem_ImageIndex_SetWithImageKey_GetReturnsExpected(int value)
    {
        using SubToolStripItem item = new()
        {
            ImageKey = "ImageKey",
            ImageIndex = value
        };
        Assert.Equal(value, item.ImageIndex);
        Assert.Empty(item.ImageKey);
        Assert.Empty(item.ImageKey);
        Assert.Null(item.Image);

        // Set same.
        item.ImageIndex = value;
        Assert.Equal(value, item.ImageIndex);
        Assert.Empty(item.ImageKey);
        Assert.Empty(item.ImageKey);
        Assert.Null(item.Image);
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    public void ToolStripItem_ImageIndex_SetWithOwner_GetReturnsExpected(int value)
    {
        using ToolStrip owner = new();
        using SubToolStripItem item = new()
        {
            Owner = owner
        };
        int ownerLayoutCallCount = 0;
        void ownerHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(owner, sender);
            Assert.Same(item, e.AffectedComponent);
            Assert.Equal("ImageIndex", e.AffectedProperty);
            ownerLayoutCallCount++;
        }

        owner.Layout += ownerHandler;

        try
        {
            item.ImageIndex = value;
            Assert.Equal(value, item.ImageIndex);
            Assert.Null(item.Image);
            Assert.Equal(1, ownerLayoutCallCount);
            Assert.False(owner.IsHandleCreated);

            // Set same.
            item.ImageIndex = value;
            Assert.Equal(value, item.ImageIndex);
            Assert.Null(item.Image);
            Assert.Equal(2, ownerLayoutCallCount);
            Assert.False(owner.IsHandleCreated);
        }
        finally
        {
            owner.Layout -= ownerHandler;
        }
    }

    [WinFormsTheory]
    [InlineData(-1, -1, false)]
    [InlineData(0, 0, true)]
    [InlineData(1, 0, false)]
    public void ToolStripItem_ImageIndex_SetWithOwnerWithImageList_GetReturnsExpected(int value, int expected, bool expectedHasImage)
    {
        using Bitmap image = new(10, 10);
        using ImageList imageList = new();
        imageList.Images.Add(image);
        using ToolStrip owner = new()
        {
            ImageList = imageList
        };
        using SubToolStripItem item = new()
        {
            Owner = owner
        };
        int ownerLayoutCallCount = 0;
        void ownerHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(owner, sender);
            Assert.Same(item, e.AffectedComponent);
            Assert.Equal("ImageIndex", e.AffectedProperty);
            ownerLayoutCallCount++;
        }

        owner.Layout += ownerHandler;

        try
        {
            item.ImageIndex = value;
            Assert.Equal(expected, item.ImageIndex);
            Assert.Empty(item.ImageKey);
            Assert.Equal(expectedHasImage, item.Image is not null);
            Assert.Equal(1, ownerLayoutCallCount);
            Assert.False(owner.IsHandleCreated);

            // Set same.
            item.ImageIndex = value;
            Assert.Equal(expected, item.ImageIndex);
            Assert.Empty(item.ImageKey);
            Assert.Equal(expectedHasImage, item.Image is not null);
            Assert.Equal(2, ownerLayoutCallCount);
            Assert.False(owner.IsHandleCreated);
        }
        finally
        {
            owner.Layout -= ownerHandler;
        }
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    public void ToolStripItem_ImageIndex_SetWithOwnerWithHandle_GetReturnsExpected(int value)
    {
        using ToolStrip owner = new();
        using SubToolStripItem item = new()
        {
            Owner = owner
        };
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;
        int ownerLayoutCallCount = 0;
        void ownerHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(owner, sender);
            Assert.Same(item, e.AffectedComponent);
            Assert.Equal("ImageIndex", e.AffectedProperty);
            ownerLayoutCallCount++;
        }

        owner.Layout += ownerHandler;

        try
        {
            item.ImageIndex = value;
            Assert.Equal(value, item.ImageIndex);
            Assert.Null(item.Image);
            Assert.Equal(1, ownerLayoutCallCount);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(2, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            item.ImageIndex = value;
            Assert.Equal(value, item.ImageIndex);
            Assert.Null(item.Image);
            Assert.Equal(2, ownerLayoutCallCount);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(4, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }
        finally
        {
            owner.Layout -= ownerHandler;
        }
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    public void ToolStripItem_ImageIndex_SetWithParent_GetReturnsExpected(int value)
    {
        using ToolStrip parent = new();
        using SubToolStripItem item = new()
        {
            Parent = parent
        };
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e) => parentLayoutCallCount++;
        parent.Layout += parentHandler;

        try
        {
            item.ImageIndex = value;
            Assert.Equal(value, item.ImageIndex);
            Assert.Null(item.Image);
            Assert.Equal(0, parentLayoutCallCount);
            Assert.False(parent.IsHandleCreated);

            // Set same.
            item.ImageIndex = value;
            Assert.Equal(value, item.ImageIndex);
            Assert.Null(item.Image);
            Assert.Equal(0, parentLayoutCallCount);
            Assert.False(parent.IsHandleCreated);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    public void ToolStripItem_ImageIndex_SetWithParentWithImageList_GetReturnsExpected(int value)
    {
        using Bitmap image = new(10, 10);
        using ImageList imageList = new();
        imageList.Images.Add(image);
        using ToolStrip parent = new()
        {
            ImageList = imageList
        };
        using SubToolStripItem item = new()
        {
            Parent = parent
        };
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e) => parentLayoutCallCount++;
        parent.Layout += parentHandler;

        try
        {
            item.ImageIndex = value;
            Assert.Equal(value, item.ImageIndex);
            Assert.Null(item.Image);
            Assert.Equal(0, parentLayoutCallCount);
            Assert.False(parent.IsHandleCreated);

            // Set same.
            item.ImageIndex = value;
            Assert.Equal(value, item.ImageIndex);
            Assert.Null(item.Image);
            Assert.Equal(0, parentLayoutCallCount);
            Assert.False(parent.IsHandleCreated);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    public void ToolStripItem_ImageIndex_SetWithParentWithHandle_GetReturnsExpected(int value)
    {
        using ToolStrip parent = new();
        using SubToolStripItem item = new()
        {
            Parent = parent
        };
        Assert.NotEqual(IntPtr.Zero, parent.Handle);
        int invalidatedCallCount = 0;
        parent.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        parent.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        parent.HandleCreated += (sender, e) => createdCallCount++;
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e) => parentLayoutCallCount++;
        parent.Layout += parentHandler;

        try
        {
            item.ImageIndex = value;
            Assert.Equal(value, item.ImageIndex);
            Assert.Null(item.Image);
            Assert.Equal(0, parentLayoutCallCount);
            Assert.True(parent.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            item.ImageIndex = value;
            Assert.Equal(value, item.ImageIndex);
            Assert.Null(item.Image);
            Assert.Equal(0, parentLayoutCallCount);
            Assert.True(parent.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    [WinFormsFact]
    public void ToolStripItem_ImageIndex_SetInvalid_ThrowsArgumentOutOfRangeException()
    {
        using SubToolStripItem item = new();
        Assert.Throws<ArgumentOutOfRangeException>("value", () => item.ImageIndex = -2);
    }

    [WinFormsFact]
    public void ToolStripItem_ImageIndex_ResetValue_Success()
    {
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ToolStripItem))[nameof(ToolStripItem.ImageIndex)];
        using SubToolStripItem item = new();
        Assert.False(property.CanResetValue(item));

        item.ImageIndex = -1;
        Assert.Equal(-1, item.ImageIndex);
        Assert.False(property.CanResetValue(item));

        // Set custom
        item.ImageIndex = 0;
        Assert.Equal(0, item.ImageIndex);
        Assert.False(property.CanResetValue(item));

        property.ResetValue(item);
        Assert.Equal(0, item.ImageIndex);
        Assert.False(property.CanResetValue(item));
    }

    [WinFormsFact]
    public void ToolStripItem_ImageIndex_ResetValueWithImage_Success()
    {
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ToolStripItem))[nameof(ToolStripItem.ImageIndex)];
        using Bitmap image = new(10, 10);
        using SubToolStripItem item = new()
        {
            Image = image
        };
        Assert.False(property.CanResetValue(item));

        item.ImageIndex = -1;
        Assert.Equal(-1, item.ImageIndex);
        Assert.False(property.CanResetValue(item));

        // Set custom
        item.ImageIndex = 0;
        Assert.Equal(0, item.ImageIndex);
        Assert.False(property.CanResetValue(item));

        property.ResetValue(item);
        Assert.Equal(0, item.ImageIndex);
        Assert.False(property.CanResetValue(item));
    }

    [WinFormsFact]
    public void ToolStripItem_ImageIndex_ResetValueWithOwnerWithImageList_Success()
    {
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ToolStripItem))[nameof(ToolStripItem.ImageIndex)];
        using Bitmap image = new(10, 10);
        using ImageList imageList = new();
        imageList.Images.Add(image);
        using ToolStrip owner = new()
        {
            ImageList = imageList
        };
        using SubToolStripItem item = new()
        {
            Owner = owner
        };
        Assert.False(property.CanResetValue(item));

        item.ImageIndex = 0;
        Assert.Equal(0, item.ImageIndex);
        Assert.False(property.CanResetValue(item));

        item.ImageIndex = 0;
        Assert.Equal(0, item.ImageIndex);
        Assert.False(property.CanResetValue(item));

        item.ImageIndex = -1;
        Assert.Equal(-1, item.ImageIndex);
        Assert.False(property.CanResetValue(item));

        property.ResetValue(item);
        Assert.Equal(-1, item.ImageIndex);
        Assert.False(property.CanResetValue(item));
    }

    [WinFormsFact]
    public void ToolStripItem_ImageIndex_ShouldSerializeValue_Success()
    {
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ToolStripItem))[nameof(ToolStripItem.ImageIndex)];
        using SubToolStripItem item = new();
        Assert.False(property.ShouldSerializeValue(item));
        item.ImageIndex = -1;
        Assert.Equal(-1, item.ImageIndex);
        Assert.False(property.ShouldSerializeValue(item));

        // Set custom
        item.ImageIndex = 0;
        Assert.Equal(0, item.ImageIndex);
        Assert.False(property.ShouldSerializeValue(item));

        property.ResetValue(item);
        Assert.Equal(0, item.ImageIndex);
        Assert.False(property.ShouldSerializeValue(item));
    }

    [WinFormsFact]
    public void ToolStripItem_ImageIndex_ShouldSerializeValueWithImage_Success()
    {
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ToolStripItem))[nameof(ToolStripItem.ImageIndex)];
        using Bitmap image = new(10, 10);
        using SubToolStripItem item = new()
        {
            Image = image
        };
        Assert.False(property.ShouldSerializeValue(item));
        item.ImageIndex = -1;
        Assert.Equal(-1, item.ImageIndex);
        Assert.False(property.ShouldSerializeValue(item));

        // Set custom
        item.ImageIndex = 0;
        Assert.Equal(0, item.ImageIndex);
        Assert.False(property.ShouldSerializeValue(item));

        property.ResetValue(item);
        Assert.Equal(0, item.ImageIndex);
        Assert.False(property.ShouldSerializeValue(item));
    }

    [WinFormsFact]
    public void ToolStripItem_ImageIndex_ShouldSerializeValueWithOwnerWithImageList_Success()
    {
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ToolStripItem))[nameof(ToolStripItem.ImageIndex)];
        using Bitmap image = new(10, 10);
        using ImageList imageList = new();
        imageList.Images.Add(image);
        using ToolStrip owner = new()
        {
            ImageList = imageList
        };
        using SubToolStripItem item = new()
        {
            Owner = owner
        };
        Assert.False(property.ShouldSerializeValue(item));

        item.ImageIndex = 0;
        Assert.Equal(0, item.ImageIndex);
        Assert.True(property.ShouldSerializeValue(item));

        item.ImageIndex = 0;
        Assert.Equal(0, item.ImageIndex);
        Assert.True(property.ShouldSerializeValue(item));

        item.ImageIndex = -1;
        Assert.Equal(-1, item.ImageIndex);
        Assert.False(property.ShouldSerializeValue(item));

        property.ResetValue(item);
        Assert.Equal(-1, item.ImageIndex);
        Assert.False(property.ShouldSerializeValue(item));
    }

    [WinFormsTheory]
    [NormalizedStringData]
    public void ToolStripItem_ImageKey_Set_GetReturnsExpected(string value, string expected)
    {
        using SubToolStripItem item = new()
        {
            ImageKey = value
        };
        Assert.Equal(expected, item.ImageKey);
        Assert.Equal(-1, item.ImageIndex);
        Assert.Null(item.Image);

        // Set same.
        item.ImageKey = value;
        Assert.Equal(expected, item.ImageKey);
        Assert.Equal(-1, item.ImageIndex);
        Assert.Null(item.Image);
    }

    [WinFormsTheory]
    [NormalizedStringData]
    public void ToolStripItem_ImageKey_SetWithImage_GetReturnsExpected(string value, string expected)
    {
        using Bitmap image = new(10, 10);
        using SubToolStripItem item = new()
        {
            Image = image,
            ImageKey = value
        };
        Assert.Equal(expected, item.ImageKey);
        Assert.Equal(-1, item.ImageIndex);
        Assert.Null(item.Image);

        // Set same.
        item.ImageKey = value;
        Assert.Equal(expected, item.ImageKey);
        Assert.Equal(-1, item.ImageIndex);
        Assert.Null(item.Image);
    }

    [WinFormsTheory]
    [NormalizedStringData]
    public void ToolStripItem_ImageKey_SetWithImageIndex_GetReturnsExpected(string value, string expected)
    {
        using SubToolStripItem item = new()
        {
            ImageIndex = 1,
            ImageKey = value
        };
        Assert.Equal(expected, item.ImageKey);
        Assert.Equal(-1, item.ImageIndex);
        Assert.Null(item.Image);

        // Set same.
        item.ImageKey = value;
        Assert.Equal(expected, item.ImageKey);
        Assert.Equal(-1, item.ImageIndex);
        Assert.Null(item.Image);
    }

    [WinFormsTheory]
    [NormalizedStringData]
    public void ToolStripItem_ImageKey_SetWithOwner_GetReturnsExpected(string value, string expected)
    {
        using ToolStrip owner = new();
        using SubToolStripItem item = new()
        {
            Owner = owner
        };
        int ownerLayoutCallCount = 0;
        void ownerHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(owner, sender);
            Assert.Same(item, e.AffectedComponent);
            Assert.Equal("ImageKey", e.AffectedProperty);
            ownerLayoutCallCount++;
        }

        owner.Layout += ownerHandler;

        try
        {
            item.ImageKey = value;
            Assert.Equal(expected, item.ImageKey);
            Assert.Null(item.Image);
            Assert.Equal(1, ownerLayoutCallCount);
            Assert.False(owner.IsHandleCreated);

            // Set same.
            item.ImageKey = value;
            Assert.Equal(expected, item.ImageKey);
            Assert.Null(item.Image);
            Assert.Equal(2, ownerLayoutCallCount);
            Assert.False(owner.IsHandleCreated);
        }
        finally
        {
            owner.Layout -= ownerHandler;
        }
    }

    [WinFormsTheory]
    [InlineData(null, "", false)]
    [InlineData("", "", false)]
    [InlineData("Image", "Image", true)]
    [InlineData("image", "image", true)]
    [InlineData("OtherImage", "OtherImage", false)]
    public void ToolStripItem_ImageKey_SetWithOwnerWithImageList_GetReturnsExpected(string value, string expected, bool expectedHasImage)
    {
        using Bitmap image = new(10, 10);
        using ImageList imageList = new();
        imageList.Images.Add("Image", image);
        using ToolStrip owner = new()
        {
            ImageList = imageList
        };
        using SubToolStripItem item = new()
        {
            Owner = owner
        };
        int ownerLayoutCallCount = 0;
        void ownerHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(owner, sender);
            Assert.Same(item, e.AffectedComponent);
            Assert.Equal("ImageKey", e.AffectedProperty);
            ownerLayoutCallCount++;
        }

        owner.Layout += ownerHandler;

        try
        {
            item.ImageKey = value;
            Assert.Equal(expected, item.ImageKey);
            Assert.Equal(-1, item.ImageIndex);
            Assert.Equal(expectedHasImage, item.Image is not null);
            Assert.Equal(1, ownerLayoutCallCount);
            Assert.False(owner.IsHandleCreated);

            // Set same.
            item.ImageKey = value;
            Assert.Equal(expected, item.ImageKey);
            Assert.Equal(-1, item.ImageIndex);
            Assert.Equal(expectedHasImage, item.Image is not null);
            Assert.Equal(2, ownerLayoutCallCount);
            Assert.False(owner.IsHandleCreated);
        }
        finally
        {
            owner.Layout -= ownerHandler;
        }
    }

    [WinFormsTheory]
    [NormalizedStringData]
    public void ToolStripItem_ImageKey_SetWithOwnerWithHandle_GetReturnsExpected(string value, string expected)
    {
        using ToolStrip owner = new();
        using SubToolStripItem item = new()
        {
            Owner = owner
        };
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;
        int ownerLayoutCallCount = 0;
        void ownerHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(owner, sender);
            Assert.Same(item, e.AffectedComponent);
            Assert.Equal("ImageKey", e.AffectedProperty);
            ownerLayoutCallCount++;
        }

        owner.Layout += ownerHandler;

        try
        {
            item.ImageKey = value;
            Assert.Equal(expected, item.ImageKey);
            Assert.Null(item.Image);
            Assert.Equal(1, ownerLayoutCallCount);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(2, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            item.ImageKey = value;
            Assert.Equal(expected, item.ImageKey);
            Assert.Null(item.Image);
            Assert.Equal(2, ownerLayoutCallCount);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(4, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }
        finally
        {
            owner.Layout -= ownerHandler;
        }
    }

    [WinFormsTheory]
    [NormalizedStringData]
    public void ToolStripItem_ImageKey_SetWithParent_GetReturnsExpected(string value, string expected)
    {
        using ToolStrip parent = new();
        using SubToolStripItem item = new()
        {
            Parent = parent
        };
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e) => parentLayoutCallCount++;
        parent.Layout += parentHandler;

        try
        {
            item.ImageKey = value;
            Assert.Equal(expected, item.ImageKey);
            Assert.Null(item.Image);
            Assert.Equal(0, parentLayoutCallCount);
            Assert.False(parent.IsHandleCreated);

            // Set same.
            item.ImageKey = value;
            Assert.Equal(expected, item.ImageKey);
            Assert.Null(item.Image);
            Assert.Equal(0, parentLayoutCallCount);
            Assert.False(parent.IsHandleCreated);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    [WinFormsTheory]
    [NormalizedStringData]
    public void ToolStripItem_ImageKey_SetWithParentWithImageList_GetReturnsExpected(string value, string expected)
    {
        using Bitmap image = new(10, 10);
        using ImageList imageList = new();
        imageList.Images.Add(image);
        using ToolStrip parent = new()
        {
            ImageList = imageList
        };
        using SubToolStripItem item = new()
        {
            Parent = parent
        };
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e) => parentLayoutCallCount++;
        parent.Layout += parentHandler;

        try
        {
            item.ImageKey = value;
            Assert.Equal(expected, item.ImageKey);
            Assert.Null(item.Image);
            Assert.Equal(0, parentLayoutCallCount);
            Assert.False(parent.IsHandleCreated);

            // Set same.
            item.ImageKey = value;
            Assert.Equal(expected, item.ImageKey);
            Assert.Null(item.Image);
            Assert.Equal(0, parentLayoutCallCount);
            Assert.False(parent.IsHandleCreated);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    [WinFormsTheory]
    [NormalizedStringData]
    public void ToolStripItem_ImageKey_SetWithParentWithHandle_GetReturnsExpected(string value, string expected)
    {
        using ToolStrip parent = new();
        using SubToolStripItem item = new()
        {
            Parent = parent
        };
        Assert.NotEqual(IntPtr.Zero, parent.Handle);
        int invalidatedCallCount = 0;
        parent.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        parent.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        parent.HandleCreated += (sender, e) => createdCallCount++;
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e) => parentLayoutCallCount++;
        parent.Layout += parentHandler;

        try
        {
            item.ImageKey = value;
            Assert.Equal(expected, item.ImageKey);
            Assert.Null(item.Image);
            Assert.Equal(0, parentLayoutCallCount);
            Assert.True(parent.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            item.ImageKey = value;
            Assert.Equal(expected, item.ImageKey);
            Assert.Null(item.Image);
            Assert.Equal(0, parentLayoutCallCount);
            Assert.True(parent.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    [WinFormsFact]
    public void ToolStripItem_ImageKey_ResetValue_Success()
    {
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ToolStripItem))[nameof(ToolStripItem.ImageKey)];
        using SubToolStripItem item = new();
        Assert.False(property.CanResetValue(item));

        // Set null.
        item.ImageKey = null;
        Assert.Empty(item.ImageKey);
        Assert.False(property.CanResetValue(item));

        // Set empty.
        item.ImageKey = string.Empty;
        Assert.Empty(item.ImageKey);
        Assert.False(property.CanResetValue(item));

        // Set custom
        item.ImageKey = "text";
        Assert.Equal("text", item.ImageKey);
        Assert.False(property.CanResetValue(item));

        property.ResetValue(item);
        Assert.Equal("text", item.ImageKey);
        Assert.False(property.CanResetValue(item));
    }

    [WinFormsFact]
    public void ToolStripItem_ImageKey_ResetValueWithImage_Success()
    {
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ToolStripItem))[nameof(ToolStripItem.ImageKey)];
        using Bitmap image = new(10, 10);
        using SubToolStripItem item = new()
        {
            Image = image
        };
        Assert.False(property.CanResetValue(item));

        // Set null.
        item.ImageKey = null;
        Assert.Empty(item.ImageKey);
        Assert.False(property.CanResetValue(item));

        // Set empty.
        item.ImageKey = string.Empty;
        Assert.Empty(item.ImageKey);
        Assert.False(property.CanResetValue(item));

        // Set custom
        item.ImageKey = "text";
        Assert.Equal("text", item.ImageKey);
        Assert.False(property.CanResetValue(item));

        property.ResetValue(item);
        Assert.Equal("text", item.ImageKey);
        Assert.False(property.CanResetValue(item));
    }

    [WinFormsFact]
    public void ToolStripItem_ImageKey_ResetValueWithOwnerWithImageList_Success()
    {
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ToolStripItem))[nameof(ToolStripItem.ImageKey)];
        using Bitmap image = new(10, 10);
        using ImageList imageList = new();
        imageList.Images.Add("Image", image);
        using ToolStrip owner = new()
        {
            ImageList = imageList
        };
        using SubToolStripItem item = new()
        {
            Owner = owner
        };
        Assert.False(property.CanResetValue(item));

        item.ImageKey = "Image";
        Assert.Equal("Image", item.ImageKey);
        Assert.False(property.CanResetValue(item));

        item.ImageKey = "NoSuchImage";
        Assert.Equal("NoSuchImage", item.ImageKey);
        Assert.False(property.CanResetValue(item));

        item.ImageKey = string.Empty;
        Assert.Empty(item.ImageKey);
        Assert.False(property.CanResetValue(item));

        property.ResetValue(item);
        Assert.Empty(item.ImageKey);
        Assert.False(property.CanResetValue(item));
    }

    [WinFormsFact]
    public void ToolStripItem_ImageKey_ShouldSerializeValue_Success()
    {
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ToolStripItem))[nameof(ToolStripItem.ImageKey)];
        using SubToolStripItem item = new();
        Assert.False(property.ShouldSerializeValue(item));

        // Set null.
        item.ImageKey = null;
        Assert.Empty(item.ImageKey);
        Assert.False(property.ShouldSerializeValue(item));

        // Set empty.
        item.ImageKey = string.Empty;
        Assert.Empty(item.ImageKey);
        Assert.False(property.ShouldSerializeValue(item));

        // Set custom
        item.ImageKey = "text";
        Assert.Equal("text", item.ImageKey);
        Assert.False(property.ShouldSerializeValue(item));

        property.ResetValue(item);
        Assert.Equal("text", item.ImageKey);
        Assert.False(property.ShouldSerializeValue(item));
    }

    [WinFormsFact]
    public void ToolStripItem_ImageKey_ShouldSerializeValueWithImage_Success()
    {
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ToolStripItem))[nameof(ToolStripItem.ImageKey)];
        using Bitmap image = new(10, 10);
        using SubToolStripItem item = new()
        {
            Image = image
        };
        Assert.False(property.ShouldSerializeValue(item));

        // Set null.
        item.ImageKey = null;
        Assert.Empty(item.ImageKey);
        Assert.False(property.ShouldSerializeValue(item));

        // Set empty.
        item.ImageKey = string.Empty;
        Assert.Empty(item.ImageKey);
        Assert.False(property.ShouldSerializeValue(item));

        // Set custom
        item.ImageKey = "text";
        Assert.Equal("text", item.ImageKey);
        Assert.False(property.ShouldSerializeValue(item));

        property.ResetValue(item);
        Assert.Equal("text", item.ImageKey);
        Assert.False(property.ShouldSerializeValue(item));
    }

    [WinFormsFact(Skip = "https://github.com/dotnet/winforms/issues/8636")]
    public void ToolStripItem_ImageKey_ShouldSerializeValueWithOwnerWithImageList_Success()
    {
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ToolStripItem))[nameof(ToolStripItem.ImageKey)];
        using Bitmap image = new(10, 10);
        using ImageList imageList = new();
        imageList.Images.Add("Image", image);
        using ToolStrip owner = new()
        {
            ImageList = imageList
        };
        using SubToolStripItem item = new()
        {
            Owner = owner
        };
        Assert.False(property.ShouldSerializeValue(item));

        item.ImageKey = "Image";
        Assert.Equal("Image", item.ImageKey);
        Assert.True(property.ShouldSerializeValue(item));

        item.ImageKey = "NoSuchImage";
        Assert.Equal("NoSuchImage", item.ImageKey);
        Assert.False(property.ShouldSerializeValue(item));

        item.ImageKey = string.Empty;
        Assert.Empty(item.ImageKey);
        Assert.False(property.ShouldSerializeValue(item));

        property.ResetValue(item);
        Assert.Empty(item.ImageKey);
        Assert.False(property.ShouldSerializeValue(item));
    }

    [WinFormsTheory]
    [EnumData<ToolStripItemImageScaling>]
    public void ToolStripItem_ImageScaling_Set_GetReturnsExpected(ToolStripItemImageScaling value)
    {
        using SubToolStripItem item = new()
        {
            ImageScaling = value
        };
        Assert.Equal(value, item.ImageScaling);

        // Set same.
        item.ImageScaling = value;
        Assert.Equal(value, item.ImageScaling);
    }

    [WinFormsTheory]
    [InlineData(ToolStripItemImageScaling.None, 1)]
    [InlineData(ToolStripItemImageScaling.SizeToFit, 0)]
    public void ToolStripItem_ImageScaling_SetWithOwner_GetReturnsExpected(ToolStripItemImageScaling value, int expectedParentLayoutCallCount)
    {
        using ToolStrip owner = new();
        using SubToolStripItem item = new()
        {
            Owner = owner
        };
        int ownerLayoutCallCount = 0;
        void ownerHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(owner, sender);
            Assert.Same(item, e.AffectedComponent);
            Assert.Equal("ImageScaling", e.AffectedProperty);
            ownerLayoutCallCount++;
        }

        owner.Layout += ownerHandler;

        try
        {
            item.ImageScaling = value;
            Assert.Equal(value, item.ImageScaling);
            Assert.Equal(expectedParentLayoutCallCount, ownerLayoutCallCount);
            Assert.False(owner.IsHandleCreated);

            // Set same.
            item.ImageScaling = value;
            Assert.Equal(value, item.ImageScaling);
            Assert.Equal(expectedParentLayoutCallCount, ownerLayoutCallCount);
            Assert.False(owner.IsHandleCreated);
        }
        finally
        {
            owner.Layout -= ownerHandler;
        }
    }

    [WinFormsTheory]
    [InlineData(ToolStripItemImageScaling.None, 1)]
    [InlineData(ToolStripItemImageScaling.SizeToFit, 0)]
    public void ToolStripItem_ImageScaling_SetWithOwnerWithHandle_GetReturnsExpected(ToolStripItemImageScaling value, int expectedParentLayoutCallCount)
    {
        using ToolStrip owner = new();
        using SubToolStripItem item = new()
        {
            Owner = owner
        };
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;
        int ownerLayoutCallCount = 0;
        void ownerHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(owner, sender);
            Assert.Same(item, e.AffectedComponent);
            Assert.Equal("ImageScaling", e.AffectedProperty);
            ownerLayoutCallCount++;
        }

        owner.Layout += ownerHandler;

        try
        {
            item.ImageScaling = value;
            Assert.Equal(value, item.ImageScaling);
            Assert.Equal(expectedParentLayoutCallCount, ownerLayoutCallCount);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(expectedParentLayoutCallCount * 2, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            item.ImageScaling = value;
            Assert.Equal(value, item.ImageScaling);
            Assert.Equal(expectedParentLayoutCallCount, ownerLayoutCallCount);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(expectedParentLayoutCallCount * 2, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }
        finally
        {
            owner.Layout -= ownerHandler;
        }
    }

    [WinFormsTheory]
    [EnumData<ToolStripItemImageScaling>]
    public void ToolStripItem_ImageScaling_SetWithParent_GetReturnsExpected(ToolStripItemImageScaling value)
    {
        using ToolStrip parent = new();
        using SubToolStripItem item = new()
        {
            Parent = parent
        };
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e) => parentLayoutCallCount++;
        parent.Layout += parentHandler;

        try
        {
            item.ImageScaling = value;
            Assert.Equal(value, item.ImageScaling);
            Assert.Equal(0, parentLayoutCallCount);
            Assert.False(parent.IsHandleCreated);

            // Set same.
            item.ImageScaling = value;
            Assert.Equal(value, item.ImageScaling);
            Assert.Equal(0, parentLayoutCallCount);
            Assert.False(parent.IsHandleCreated);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    [WinFormsTheory]
    [EnumData<ToolStripItemImageScaling>]
    public void ToolStripItem_ImageScaling_SetWithParentWithHandle_GetReturnsExpected(ToolStripItemImageScaling value)
    {
        using ToolStrip parent = new();
        using SubToolStripItem item = new()
        {
            Parent = parent
        };
        Assert.NotEqual(IntPtr.Zero, parent.Handle);
        int invalidatedCallCount = 0;
        parent.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        parent.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        parent.HandleCreated += (sender, e) => createdCallCount++;
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e) => parentLayoutCallCount++;
        parent.Layout += parentHandler;

        try
        {
            item.ImageScaling = value;
            Assert.Equal(value, item.ImageScaling);
            Assert.Equal(0, parentLayoutCallCount);
            Assert.True(parent.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            item.ImageScaling = value;
            Assert.Equal(value, item.ImageScaling);
            Assert.Equal(0, parentLayoutCallCount);
            Assert.True(parent.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    [WinFormsTheory]
    [InvalidEnumData<ToolStripItemImageScaling>]
    public void ToolStripItem_ImageScaling_SetInvalid_ThrowsInvalidEnumArgumentException(ToolStripItemImageScaling value)
    {
        using SubToolStripItem item = new();
        Assert.Throws<InvalidEnumArgumentException>("value", () => item.ImageScaling = value);
    }

    public static IEnumerable<object[]> ImageTransparentColor_Set_TestData()
    {
        foreach (Color color in new Color[] { Color.Empty, Color.Red })
        {
            yield return new object[] { null, color };
            yield return new object[] { new Bitmap(10, 10), color };
            yield return new object[] { Image.FromFile(Path.Combine("bitmaps", "nature24bits.gif")), color };
            yield return new object[] { Image.FromFile(Path.Combine("bitmaps", "10x16_one_entry_32bit.ico")), color };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(ImageTransparentColor_Set_TestData))]
    public void ToolStripItem_ImageTransparentColor_Set_GetReturnsExpected(Image image, Color value)
    {
        using SubToolStripItem item = new()
        {
            Image = image
        };

        item.ImageTransparentColor = value;
        Assert.Equal(value, item.ImageTransparentColor);

        // Set same.
        item.ImageTransparentColor = value;
        Assert.Equal(value, item.ImageTransparentColor);
    }

    [WinFormsTheory]
    [MemberData(nameof(ImageTransparentColor_Set_TestData))]
    public void ToolStripItem_ImageTransparentColor_SetWithCustomOldValue_GetReturnsExpected(Image image, Color value)
    {
        using SubToolStripItem item = new()
        {
            Image = image,
            ImageTransparentColor = Color.Red
        };

        item.ImageTransparentColor = value;
        Assert.Equal(value, item.ImageTransparentColor);

        // Set same.
        item.ImageTransparentColor = value;
        Assert.Equal(value, item.ImageTransparentColor);
    }

    [WinFormsTheory]
    [MemberData(nameof(ImageTransparentColor_Set_TestData))]
    public void ToolStripItem_ImageTransparentColor_SetWithOwner_GetReturnsExpected(Image image, Color value)
    {
        using ToolStrip owner = new();
        using SubToolStripItem item = new()
        {
            Owner = owner,
            Image = image
        };

        item.ImageTransparentColor = value;
        Assert.Equal(value, item.ImageTransparentColor);
        Assert.False(owner.IsHandleCreated);

        // Set same.
        item.ImageTransparentColor = value;
        Assert.Equal(value, item.ImageTransparentColor);
        Assert.False(owner.IsHandleCreated);
    }

    public static IEnumerable<object[]> ImageTransparentColor_SetWithOwnerWithHandle_TestData()
    {
        yield return new object[] { null, Color.Empty, 0 };
        yield return new object[] { null, Color.Red, 0 };
        yield return new object[] { new Bitmap(10, 10), Color.Empty, 0 };
        yield return new object[] { new Bitmap(10, 10), Color.Red, 1 };
        yield return new object[] { Image.FromFile(Path.Combine("bitmaps", "nature24bits.gif")), Color.Empty, 0 };
        yield return new object[] { Image.FromFile(Path.Combine("bitmaps", "nature24bits.gif")), Color.Red, 1 };
        yield return new object[] { Image.FromFile(Path.Combine("bitmaps", "10x16_one_entry_32bit.ico")), Color.Empty, 0 };
        yield return new object[] { Image.FromFile(Path.Combine("bitmaps", "10x16_one_entry_32bit.ico")), Color.Red, 1 };
    }

    [WinFormsTheory]
    [MemberData(nameof(ImageTransparentColor_SetWithOwnerWithHandle_TestData))]
    public void ToolStripItem_ImageTransparentColor_SetWithOwnerWithHandle_GetReturnsExpected(Image image, Color value, int expectedInvalidatedCallCount)
    {
        using ToolStrip owner = new();
        using SubToolStripItem item = new()
        {
            Owner = owner,
            Image = image
        };
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;

        item.ImageTransparentColor = value;
        Assert.Equal(value, item.ImageTransparentColor);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        item.ImageTransparentColor = value;
        Assert.Equal(value, item.ImageTransparentColor);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [MemberData(nameof(ImageTransparentColor_Set_TestData))]
    public void ToolStripItem_ImageTransparentColor_SetWithParent_GetReturnsExpected(Image image, Color value)
    {
        using ToolStrip parent = new();
        using SubToolStripItem item = new()
        {
            Parent = parent,
            Image = image
        };

        item.ImageTransparentColor = value;
        Assert.Equal(value, item.ImageTransparentColor);
        Assert.False(parent.IsHandleCreated);

        // Set same.
        item.ImageTransparentColor = value;
        Assert.Equal(value, item.ImageTransparentColor);
        Assert.False(parent.IsHandleCreated);
    }

    public static IEnumerable<object[]> ImageTransparentColor_SetWithParentWithHandle_TestData()
    {
        yield return new object[] { null, Color.Empty, 0 };
        yield return new object[] { null, Color.Red, 1 };
        yield return new object[] { new Bitmap(10, 10), Color.Empty, 0 };
        yield return new object[] { new Bitmap(10, 10), Color.Red, 1 };
        yield return new object[] { Image.FromFile(Path.Combine("bitmaps", "nature24bits.gif")), Color.Empty, 0 };
        yield return new object[] { Image.FromFile(Path.Combine("bitmaps", "nature24bits.gif")), Color.Red, 1 };
        yield return new object[] { Image.FromFile(Path.Combine("bitmaps", "10x16_one_entry_32bit.ico")), Color.Empty, 0 };
        yield return new object[] { Image.FromFile(Path.Combine("bitmaps", "10x16_one_entry_32bit.ico")), Color.Red, 1 };
    }

    [WinFormsTheory]
    [MemberData(nameof(ImageTransparentColor_SetWithParentWithHandle_TestData))]
    public void ToolStripItem_ImageTransparentColor_SetWithParentWithHandle_GetReturnsExpected(Image image, Color value, int expectedParentInvalidatedCallCount)
    {
        using ToolStrip parent = new();
        using SubToolStripItem item = new()
        {
            Parent = parent,
            Image = image
        };
        Assert.NotEqual(IntPtr.Zero, parent.Handle);
        int invalidatedCallCount = 0;
        parent.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        parent.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        parent.HandleCreated += (sender, e) => createdCallCount++;

        item.ImageTransparentColor = value;
        Assert.Equal(value, item.ImageTransparentColor);
        Assert.True(parent.IsHandleCreated);
        Assert.Equal(expectedParentInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        item.ImageTransparentColor = value;
        Assert.Equal(value, item.ImageTransparentColor);
        Assert.True(parent.IsHandleCreated);
        Assert.Equal(expectedParentInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void ToolStripItem_ImageTransparentColor_ResetValue_Success()
    {
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ToolStripItem))[nameof(ToolStripItem.ImageTransparentColor)];
        using SubToolStripItem item = new();
        Assert.False(property.CanResetValue(item));

        item.ImageTransparentColor = Color.Red;
        Assert.Equal(Color.Red, item.ImageTransparentColor);
        Assert.True(property.CanResetValue(item));

        property.ResetValue(item);
        Assert.Equal(Color.Empty, item.ImageTransparentColor);
        Assert.False(property.CanResetValue(item));
    }

    [WinFormsFact]
    public void ToolStripItem_ImageTransparentColor_ShouldSerializeValue_Success()
    {
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ToolStripItem))[nameof(ToolStripItem.ImageTransparentColor)];
        using SubToolStripItem item = new();
        Assert.False(property.ShouldSerializeValue(item));

        item.ImageTransparentColor = Color.Red;
        Assert.Equal(Color.Red, item.ImageTransparentColor);
        Assert.True(property.ShouldSerializeValue(item));

        property.ResetValue(item);
        Assert.Equal(Color.Empty, item.ImageTransparentColor);
        Assert.False(property.ShouldSerializeValue(item));
    }

    [WinFormsFact]
    public void ToolStripItem_IsOnDropDown_GetWithOwner_ReturnsFalse()
    {
        using ToolStrip owner = new();
        using SubToolStripItem item = new()
        {
            Owner = owner
        };
        Assert.False(item.IsOnDropDown);
    }

    [WinFormsFact]
    public void ToolStripItem_IsOnDropDown_GetWithDropDownOwner_ReturnsTrue()
    {
        using ToolStripDropDown owner = new();
        using SubToolStripItem item = new()
        {
            Owner = owner
        };
        Assert.True(item.IsOnDropDown);
    }

    [WinFormsFact]
    public void ToolStripItem_IsOnDropDown_GetWithOverflowingOwner_ReturnsTrue()
    {
        using ToolStrip owner = new()
        {
            Size = new Size(1, 2)
        };
        using SubToolStripItem item = new()
        {
            Owner = owner
        };
        Assert.False(item.IsOnDropDown);
        owner.LayoutEngine.Layout(owner, null);
        Assert.True(item.IsOnDropDown);
    }

    [WinFormsFact]
    public void ToolStripItem_IsOnDropDown_GetWithParent_ReturnsFalse()
    {
        using ToolStrip parent = new();
        using SubToolStripItem item = new()
        {
            Parent = parent
        };
        Assert.False(item.IsOnDropDown);
    }

    [WinFormsFact]
    public void ToolStripItem_IsOnDropDown_GetWithDropDownParent_ReturnsTrue()
    {
        using ToolStripDropDown parent = new();
        using SubToolStripItem item = new()
        {
            Parent = parent
        };
        Assert.True(item.IsOnDropDown);
    }

    [WinFormsFact]
    public void ToolStripItem_IsOnDropDown_GetWithOverflowingParent_ReturnsFalse()
    {
        using ToolStrip parent = new()
        {
            Size = new Size(1, 2)
        };
        using SubToolStripItem item = new()
        {
            Parent = parent
        };
        Assert.False(item.IsOnDropDown);
        parent.LayoutEngine.Layout(parent, null);
        Assert.False(item.IsOnDropDown);
    }

    [WinFormsFact]
    public void ToolStripItem_Margin_GetWithDefaultMargin_ReturnsExpected()
    {
        using CustomDefaultMarginToolStripItem item = new();
        Assert.Equal(new Padding(1, 2, 3, 4), item.Margin);
    }

    private class CustomDefaultMarginToolStripItem : ToolStripItem
    {
        protected internal override Padding DefaultMargin => new(1, 2, 3, 4);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetPaddingTheoryData))]
    public void ToolStripItem_Margin_Set_GetReturnsExpected(Padding value)
    {
        using SubToolStripItem item = new()
        {
            Margin = value
        };
        Assert.Equal(value, item.Margin);

        // Set same.
        item.Margin = value;
        Assert.Equal(value, item.Margin);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetPaddingTheoryData))]
    public void ToolStripItem_Margin_SetWithOwner_GetReturnsExpected(Padding value)
    {
        using ToolStrip owner = new();
        using SubToolStripItem item = new()
        {
            Owner = owner
        };
        int ownerLayoutCallCount = 0;
        void ownerHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(owner, sender);
            Assert.Same(item, e.AffectedComponent);
            Assert.Equal("Margin", e.AffectedProperty);
            ownerLayoutCallCount++;
        }

        owner.Layout += ownerHandler;

        try
        {
            item.Margin = value;
            Assert.Equal(value, item.Margin);
            Assert.Equal(1, ownerLayoutCallCount);
            Assert.False(owner.IsHandleCreated);

            // Set same.
            item.Margin = value;
            Assert.Equal(value, item.Margin);
            Assert.Equal(1, ownerLayoutCallCount);
            Assert.False(owner.IsHandleCreated);
        }
        finally
        {
            owner.Layout -= ownerHandler;
        }
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetPaddingTheoryData))]
    public void ToolStripItem_Margin_SetWithOwnerWithHandle_GetReturnsExpected(Padding value)
    {
        using ToolStrip owner = new();
        using SubToolStripItem item = new()
        {
            Owner = owner
        };
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;
        int ownerLayoutCallCount = 0;
        void ownerHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(owner, sender);
            Assert.Same(item, e.AffectedComponent);
            Assert.Equal("Margin", e.AffectedProperty);
            ownerLayoutCallCount++;
        }

        owner.Layout += ownerHandler;

        try
        {
            item.Margin = value;
            Assert.Equal(value, item.Margin);
            Assert.Equal(1, ownerLayoutCallCount);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(1, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            item.Margin = value;
            Assert.Equal(value, item.Margin);
            Assert.Equal(1, ownerLayoutCallCount);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(1, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }
        finally
        {
            owner.Layout -= ownerHandler;
        }
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetPaddingTheoryData))]
    public void ToolStripItem_Margin_SetWithParent_GetReturnsExpected(Padding value)
    {
        using ToolStrip parent = new();
        using SubToolStripItem item = new()
        {
            Parent = parent
        };
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(parent, sender);
            Assert.Same(item, e.AffectedComponent);
            Assert.Equal("Margin", e.AffectedProperty);
            parentLayoutCallCount++;
        }

        parent.Layout += parentHandler;

        try
        {
            item.Margin = value;
            Assert.Equal(value, item.Margin);
            Assert.Equal(1, parentLayoutCallCount);
            Assert.False(parent.IsHandleCreated);

            // Set same.
            item.Margin = value;
            Assert.Equal(value, item.Margin);
            Assert.Equal(1, parentLayoutCallCount);
            Assert.False(parent.IsHandleCreated);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetPaddingTheoryData))]
    public void ToolStripItem_Margin_SetWithParentWithHandle_GetReturnsExpected(Padding value)
    {
        using ToolStrip parent = new();
        using SubToolStripItem item = new()
        {
            Parent = parent
        };
        Assert.NotEqual(IntPtr.Zero, parent.Handle);
        int invalidatedCallCount = 0;
        parent.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        parent.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        parent.HandleCreated += (sender, e) => createdCallCount++;
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(parent, sender);
            Assert.Same(item, e.AffectedComponent);
            Assert.Equal("Margin", e.AffectedProperty);
            parentLayoutCallCount++;
        }

        parent.Layout += parentHandler;

        try
        {
            item.Margin = value;
            Assert.Equal(value, item.Margin);
            Assert.Equal(1, parentLayoutCallCount);
            Assert.True(parent.IsHandleCreated);
            Assert.Equal(1, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            item.Margin = value;
            Assert.Equal(value, item.Margin);
            Assert.Equal(1, parentLayoutCallCount);
            Assert.True(parent.IsHandleCreated);
            Assert.Equal(1, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    [WinFormsFact]
    public void ToolStripItem_Margin_ResetValue_Success()
    {
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ToolStripItem))[nameof(ToolStripItem.Margin)];
        using SubToolStripItem item = new();
        Assert.False(property.CanResetValue(item));

        item.Margin = new Padding(1, 2, 3, 4);
        Assert.Equal(new Padding(1, 2, 3, 4), item.Margin);
        Assert.True(property.CanResetValue(item));

        property.ResetValue(item);
        Assert.Equal(new Padding(0, 1, 0, 2), item.Margin);
        Assert.False(property.CanResetValue(item));
    }

    [WinFormsFact]
    public void ToolStripItem_Margin_ShouldSerializeValue_Success()
    {
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ToolStripItem))[nameof(ToolStripItem.Margin)];
        using SubToolStripItem item = new();
        Assert.False(property.ShouldSerializeValue(item));

        item.Margin = new Padding(1, 2, 3, 4);
        Assert.Equal(new Padding(1, 2, 3, 4), item.Margin);
        Assert.True(property.ShouldSerializeValue(item));

        property.ResetValue(item);
        Assert.Equal(new Padding(0, 1, 0, 2), item.Margin);
        Assert.False(property.ShouldSerializeValue(item));
    }

    [WinFormsTheory]
    [EnumData<MergeAction>]
    public void ToolStripItem_MergeAction_Set_GetReturnsExpected(MergeAction value)
    {
        using SubToolStripItem item = new()
        {
            MergeAction = value
        };
        Assert.Equal(value, item.MergeAction);

        // Set same.
        item.MergeAction = value;
        Assert.Equal(value, item.MergeAction);
    }

    [WinFormsTheory]
    [InvalidEnumData<MergeAction>]
    public void ToolStripItem_MergeAction_SetInvalid_ThrowsInvalidEnumArgumentException(MergeAction value)
    {
        using SubToolStripItem item = new();
        Assert.Throws<InvalidEnumArgumentException>("value", () => item.MergeAction = value);
    }

    [WinFormsTheory]
    [IntegerData<int>]
    public void ToolStripItem_MergeIndex_Set_GetReturnsExpected(int value)
    {
        using SubToolStripItem item = new()
        {
            MergeIndex = value
        };
        Assert.Equal(value, item.MergeIndex);

        // Set same.
        item.MergeIndex = value;
        Assert.Equal(value, item.MergeIndex);
    }

    [WinFormsTheory]
    [NormalizedStringData]
    public void ToolStripItem_Name_GetWithSite_ReturnsExpected(string siteName, string expected)
    {
        Mock<ISite> mockSite = new(MockBehavior.Strict);
        mockSite
            .Setup(s => s.Name)
            .Returns(siteName);
        mockSite
            .Setup(s => s.Container)
            .Returns((IContainer)null);
        using SubToolStripItem item = new()
        {
            Site = mockSite.Object
        };
        Assert.Equal(expected, item.Name);
        mockSite.Verify(s => s.Name, Times.Once());

        // Get again.
        Assert.Equal(expected, item.Name);
        mockSite.Verify(s => s.Name, Times.Exactly(2));
    }

    [WinFormsTheory]
    [StringData]
    public void ToolStripItem_Name_Set_GetReturnsExpected(string value)
    {
        using SubToolStripItem item = new()
        {
            Name = value
        };
        Assert.Equal(value, item.Name);

        // Set same.
        item.Name = value;
        Assert.Equal(value, item.Name);
    }

    [WinFormsTheory]
    [StringData]
    public void ToolStripItem_Name_SetDesignMode_Nop(string value)
    {
        Mock<ISite> mockSite = new(MockBehavior.Strict);
        mockSite
            .Setup(s => s.Name)
            .Returns("name");
        mockSite
            .Setup(s => s.DesignMode)
            .Returns(true);
        mockSite
            .Setup(s => s.Container)
            .Returns((IContainer)null);
        using SubToolStripItem item = new()
        {
            Site = mockSite.Object
        };

        item.Name = value;
        Assert.Equal("name", item.Name);
    }

    [WinFormsTheory]
    [EnumData<ToolStripItemOverflow>]
    public void ToolStripItem_Overflow_Set_GetReturnsExpected(ToolStripItemOverflow value)
    {
        using SubToolStripItem item = new()
        {
            Overflow = value
        };
        Assert.Equal(value, item.Overflow);
        Assert.False(item.IsOnOverflow);

        // Set same.
        item.Overflow = value;
        Assert.Equal(value, item.Overflow);
        Assert.False(item.IsOnOverflow);
    }

    [WinFormsTheory]
    [InlineData(ToolStripItemOverflow.Never, ToolStripItemPlacement.Main, 1)]
    [InlineData(ToolStripItemOverflow.Always, ToolStripItemPlacement.Overflow, 1)]
    [InlineData(ToolStripItemOverflow.AsNeeded, ToolStripItemPlacement.None, 0)]
    public void ToolStripItem_Overflow_SetWithOwner_GetReturnsExpected(ToolStripItemOverflow value, ToolStripItemPlacement expectedPlacement, int expectedOwnerLayoutCallCount)
    {
        using ToolStrip owner = new();
        using SubToolStripItem item = new()
        {
            Owner = owner
        };
        int ownerLayoutCallCount = 0;
        void ownerHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(owner, sender);
            Assert.Same(owner, e.AffectedComponent);
            Assert.Equal("Overflow", e.AffectedProperty);
            ownerLayoutCallCount++;
        }

        owner.Layout += ownerHandler;

        try
        {
            item.Overflow = value;
            Assert.Equal(value, item.Overflow);
            Assert.Equal(value == ToolStripItemOverflow.Always, item.IsOnOverflow);
            Assert.Equal(expectedPlacement, item.Placement);
            Assert.Equal(expectedOwnerLayoutCallCount, ownerLayoutCallCount);
            Assert.False(owner.IsHandleCreated);

            // Set same.
            item.Overflow = value;
            Assert.Equal(value, item.Overflow);
            Assert.Equal(value == ToolStripItemOverflow.Always, item.IsOnOverflow);
            Assert.Equal(expectedPlacement, item.Placement);
            Assert.Equal(expectedOwnerLayoutCallCount, ownerLayoutCallCount);
            Assert.False(owner.IsHandleCreated);
        }
        finally
        {
            owner.Layout -= ownerHandler;
        }
    }

    [WinFormsTheory]
    [InlineData(ToolStripItemOverflow.Never, ToolStripItemPlacement.Main, 1)]
    [InlineData(ToolStripItemOverflow.Always, ToolStripItemPlacement.Overflow, 1)]
    [InlineData(ToolStripItemOverflow.AsNeeded, ToolStripItemPlacement.None, 0)]
    public void ToolStripItem_Overflow_SetWithOwnerWithHandle_GetReturnsExpected(ToolStripItemOverflow value, ToolStripItemPlacement expectedPlacement, int expectedOwnerLayoutCallCount)
    {
        using ToolStrip owner = new();
        using SubToolStripItem item = new()
        {
            Owner = owner
        };
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;
        int ownerLayoutCallCount = 0;
        void ownerHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(owner, sender);
            Assert.Same(owner, e.AffectedComponent);
            Assert.Equal("Overflow", e.AffectedProperty);
            ownerLayoutCallCount++;
        }

        owner.Layout += ownerHandler;

        try
        {
            item.Overflow = value;
            Assert.Equal(value, item.Overflow);
            Assert.Equal(value == ToolStripItemOverflow.Always, item.IsOnOverflow);
            Assert.Equal(expectedPlacement, item.Placement);
            Assert.Equal(expectedOwnerLayoutCallCount, ownerLayoutCallCount);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(expectedOwnerLayoutCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            item.Overflow = value;
            Assert.Equal(value, item.Overflow);
            Assert.Equal(value == ToolStripItemOverflow.Always, item.IsOnOverflow);
            Assert.Equal(expectedPlacement, item.Placement);
            Assert.Equal(expectedOwnerLayoutCallCount, ownerLayoutCallCount);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(expectedOwnerLayoutCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }
        finally
        {
            owner.Layout -= ownerHandler;
        }
    }

    [WinFormsTheory]
    [EnumData<ToolStripItemOverflow>]
    public void ToolStripItem_Overflow_SetWithParent_GetReturnsExpected(ToolStripItemOverflow value)
    {
        using ToolStrip parent = new();
        using SubToolStripItem item = new()
        {
            Parent = parent
        };
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e) => parentLayoutCallCount++;
        parent.Layout += parentHandler;

        try
        {
            item.Overflow = value;
            Assert.Equal(value, item.Overflow);
            Assert.False(item.IsOnOverflow);
            Assert.Equal(ToolStripItemPlacement.None, item.Placement);
            Assert.Equal(0, parentLayoutCallCount);
            Assert.False(parent.IsHandleCreated);

            // Set same.
            item.Overflow = value;
            Assert.Equal(value, item.Overflow);
            Assert.False(item.IsOnOverflow);
            Assert.Equal(ToolStripItemPlacement.None, item.Placement);
            Assert.Equal(0, parentLayoutCallCount);
            Assert.False(parent.IsHandleCreated);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    [WinFormsTheory]
    [EnumData<ToolStripItemOverflow>]
    public void ToolStripItem_Overflow_SetWithParentWithHandle_GetReturnsExpected(ToolStripItemOverflow value)
    {
        using ToolStrip parent = new();
        using SubToolStripItem item = new()
        {
            Parent = parent
        };
        Assert.NotEqual(IntPtr.Zero, parent.Handle);
        int invalidatedCallCount = 0;
        parent.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        parent.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        parent.HandleCreated += (sender, e) => createdCallCount++;
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e) => parentLayoutCallCount++;
        parent.Layout += parentHandler;

        try
        {
            item.Overflow = value;
            Assert.Equal(value, item.Overflow);
            Assert.False(item.IsOnOverflow);
            Assert.Equal(ToolStripItemPlacement.None, item.Placement);
            Assert.Equal(0, parentLayoutCallCount);
            Assert.True(parent.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            item.Overflow = value;
            Assert.Equal(value, item.Overflow);
            Assert.False(item.IsOnOverflow);
            Assert.Equal(ToolStripItemPlacement.None, item.Placement);
            Assert.Equal(0, parentLayoutCallCount);
            Assert.True(parent.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    [WinFormsTheory]
    [InvalidEnumData<ToolStripItemOverflow>]
    public void ToolStripItem_Overflow_SetInvalid_ThrowsInvalidEnumArgumentException(ToolStripItemOverflow value)
    {
        using SubToolStripItem item = new();
        Assert.Throws<InvalidEnumArgumentException>("value", () => item.Overflow = value);
    }

    [WinFormsFact]
    public void ToolStripItem_Owner_Set_GetReturnsExpected()
    {
        using ToolStrip owner = new();
        using ToolStrip otherOwner = new();
        using StatusStrip statusOwner = new();
        using SubToolStripItem item = new()
        {
            Owner = owner
        };
        Assert.Same(owner, item.Owner);
        Assert.Null(item.Parent);
        Assert.Same(item, Assert.Single(owner.Items));
        Assert.Equal(new Padding(0, 1, 0, 2), item.Margin);

        // Set same.
        item.Owner = owner;
        Assert.Same(owner, item.Owner);
        Assert.Null(item.Parent);
        Assert.Same(item, Assert.Single(owner.Items));
        Assert.Empty(otherOwner.Items);
        Assert.Equal(new Padding(0, 1, 0, 2), item.Margin);

        // Set different.
        item.Owner = otherOwner;
        Assert.Same(otherOwner, item.Owner);
        Assert.Null(item.Parent);
        Assert.Empty(owner.Items);
        Assert.Same(item, Assert.Single(otherOwner.Items));
        Assert.Equal(new Padding(0, 1, 0, 2), item.Margin);

        // Set null.
        item.Owner = null;
        Assert.Null(item.Owner);
        Assert.Null(item.Parent);
        Assert.Empty(owner.Items);
        Assert.Empty(otherOwner.Items);
        Assert.Equal(new Padding(0, 1, 0, 2), item.Margin);

        // Set status strip.
        item.Owner = statusOwner;
        Assert.Same(statusOwner, item.Owner);
        Assert.Null(item.Parent);
        Assert.Empty(owner.Items);
        Assert.Empty(otherOwner.Items);
        Assert.Same(item, Assert.Single(statusOwner.Items));
        Assert.Equal(new Padding(0, 2, 0, 0), item.Margin);
    }

    [WinFormsFact]
    public void ToolStripItem_Owner_SetWithMargin_GetReturnsExpected()
    {
        using ToolStrip owner = new();
        using ToolStrip otherOwner = new();
        using StatusStrip statusOwner = new();
        using SubToolStripItem item = new()
        {
            Margin = new Padding(1, 2, 3, 4),
            Owner = owner
        };
        Assert.Same(owner, item.Owner);
        Assert.Null(item.Parent);
        Assert.Same(item, Assert.Single(owner.Items));
        Assert.Equal(new Padding(1, 2, 3, 4), item.Margin);

        // Set same.
        item.Owner = owner;
        Assert.Same(owner, item.Owner);
        Assert.Null(item.Parent);
        Assert.Same(item, Assert.Single(owner.Items));
        Assert.Empty(otherOwner.Items);
        Assert.Equal(new Padding(1, 2, 3, 4), item.Margin);

        // Set different.
        item.Owner = otherOwner;
        Assert.Same(otherOwner, item.Owner);
        Assert.Null(item.Parent);
        Assert.Empty(owner.Items);
        Assert.Same(item, Assert.Single(otherOwner.Items));
        Assert.Equal(new Padding(1, 2, 3, 4), item.Margin);

        // Set null.
        item.Owner = null;
        Assert.Null(item.Owner);
        Assert.Null(item.Parent);
        Assert.Empty(owner.Items);
        Assert.Empty(otherOwner.Items);
        Assert.Equal(new Padding(1, 2, 3, 4), item.Margin);

        // Set status strip.
        item.Owner = statusOwner;
        Assert.Same(statusOwner, item.Owner);
        Assert.Null(item.Parent);
        Assert.Empty(owner.Items);
        Assert.Empty(otherOwner.Items);
        Assert.Same(item, Assert.Single(statusOwner.Items));
        Assert.Equal(new Padding(1, 2, 3, 4), item.Margin);
    }

    [WinFormsTheory]
    [InlineData(RightToLeft.Yes, RightToLeft.Yes, RightToLeft.Yes, 0)]
    [InlineData(RightToLeft.Yes, RightToLeft.No, RightToLeft.No, 0)]
    [InlineData(RightToLeft.Yes, RightToLeft.Inherit, RightToLeft.Yes, 1)]
    [InlineData(RightToLeft.No, RightToLeft.Yes, RightToLeft.Yes, 0)]
    [InlineData(RightToLeft.No, RightToLeft.No, RightToLeft.No, 0)]
    [InlineData(RightToLeft.No, RightToLeft.Inherit, RightToLeft.No, 1)]
    [InlineData(RightToLeft.Inherit, RightToLeft.Yes, RightToLeft.Yes, 0)]
    [InlineData(RightToLeft.Inherit, RightToLeft.No, RightToLeft.No, 0)]
    [InlineData(RightToLeft.Inherit, RightToLeft.Inherit, RightToLeft.No, 1)]
    public void ToolStripItem_Owner_SetWithRightToLeft_CallsRightToLeftChanged(RightToLeft ownerRightToLeft, RightToLeft rightToLeft, RightToLeft expectedRightToLeft, int expectedRightToLeftChangedCallCount)
    {
        using ToolStrip owner = new()
        {
            RightToLeft = ownerRightToLeft
        };
        using SubToolStripItem item = new()
        {
            RightToLeft = rightToLeft
        };
        int rightToLeftChangedCallCount = 0;
        item.RightToLeftChanged += (sender, e) =>
        {
            Assert.Same(item, sender);
            Assert.Same(EventArgs.Empty, e);
            rightToLeftChangedCallCount++;
        };

        item.Owner = owner;
        Assert.Same(owner, item.Owner);
        Assert.Equal(expectedRightToLeft, item.RightToLeft);
        Assert.Equal(expectedRightToLeftChangedCallCount, rightToLeftChangedCallCount);

        // Set same.
        item.Owner = owner;
        Assert.Same(owner, item.Owner);
        Assert.Equal(expectedRightToLeft, item.RightToLeft);
        Assert.Equal(expectedRightToLeftChangedCallCount, rightToLeftChangedCallCount);
    }

    [WinFormsFact]
    public void ToolStripItem_Owner_SetWithHandler_CallsOwnerChanged()
    {
        using ToolStrip owner = new();
        using ToolStrip otherOwner = new();
        using SubToolStripItem item = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(item, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };

        item.OwnerChanged += handler;
        item.Owner = owner;
        Assert.Same(owner, item.Owner);
        Assert.Same(item, Assert.Single(owner.Items));
        Assert.Equal(1, callCount);

        // Set same.
        item.Owner = owner;
        Assert.Same(owner, item.Owner);
        Assert.Same(item, Assert.Single(owner.Items));
        Assert.Empty(otherOwner.Items);
        Assert.Equal(1, callCount);

        // Set different.
        item.Owner = otherOwner;
        Assert.Same(otherOwner, item.Owner);
        Assert.Empty(owner.Items);
        Assert.Same(item, Assert.Single(otherOwner.Items));
        Assert.Equal(3, callCount);

        // Set null.
        item.Owner = null;
        Assert.Null(item.Owner);
        Assert.Empty(owner.Items);
        Assert.Empty(otherOwner.Items);
        Assert.Equal(4, callCount);

        // Remove handler.
        item.OwnerChanged -= handler;
        item.Owner = owner;
        Assert.Same(owner, item.Owner);
        Assert.Same(item, Assert.Single(owner.Items));
        Assert.Empty(otherOwner.Items);
        Assert.Equal(4, callCount);
    }

    [WinFormsFact]
    public void ToolStripItem_OwnerItem_GetWithOwner_ReturnsNull()
    {
        using ToolStrip owner = new();
        using SubToolStripItem item = new()
        {
            Owner = owner
        };
        Assert.Null(item.OwnerItem);
    }

    public static IEnumerable<object[]> OwnerItem_GetWithDropDown_TestData()
    {
        yield return new object[] { null };
        yield return new object[] { new SubToolStripItem() };
    }

    [WinFormsTheory]
    [MemberData(nameof(OwnerItem_GetWithDropDown_TestData))]
    public void ToolStripItem_OwnerItem_GetWithDropDownOwner_ReturnsExpected(ToolStripItem result)
    {
        using ToolStripDropDown owner = new()
        {
            OwnerItem = result
        };
        using SubToolStripItem item = new()
        {
            Owner = owner
        };
        Assert.Same(result, item.OwnerItem);
    }

    [WinFormsFact]
    public void ToolStripItem_OwnerItem_GetWithParent_ReturnsNull()
    {
        using ToolStrip parent = new();
        using SubToolStripItem item = new()
        {
            Parent = parent
        };
        Assert.Null(item.OwnerItem);
    }

    [WinFormsTheory]
    [MemberData(nameof(OwnerItem_GetWithDropDown_TestData))]
    public void ToolStripItem_OwnerItem_GetWithDropDownParent_ReturnsExpected(ToolStripItem result)
    {
        using ToolStripDropDown parent = new()
        {
            OwnerItem = result
        };
        using SubToolStripItem item = new()
        {
            Parent = parent
        };
        Assert.Same(result, item.OwnerItem);
    }

    [WinFormsFact]
    public void ToolStripItem_OwnerItem_GetWithOwnerAndParent_ReturnsExpected()
    {
        using SubToolStripItem ownerItem = new();
        using ToolStripDropDown owner = new()
        {
            OwnerItem = ownerItem
        };
        using SubToolStripItem parentItem = new();
        using ToolStripDropDown parent = new()
        {
            OwnerItem = parentItem
        };
        using SubToolStripItem item = new()
        {
            Owner = owner,
            Parent = parent
        };
        Assert.Same(parentItem, item.OwnerItem);
    }

    [WinFormsFact]
    public void ToolStripItem_Padding_GetWithDefaultPadding_ReturnsExpected()
    {
        using CustomDefaultPaddingToolStripItem item = new();
        Assert.Equal(new Padding(2, 3, 4, 5), item.Padding);
    }

    private class CustomDefaultPaddingToolStripItem : ToolStripItem
    {
        protected override Padding DefaultPadding => new(2, 3, 4, 5);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetPaddingNormalizedTheoryData))]
    public void ToolStripItem_Padding_Set_GetReturnsExpected(Padding value, Padding expected)
    {
        using SubToolStripItem item = new()
        {
            Padding = value
        };
        Assert.Equal(expected, item.Padding);

        // Set same.
        item.Padding = value;
        Assert.Equal(expected, item.Padding);
    }

    public static IEnumerable<object[]> Padding_SetWithOwner_TestData()
    {
        yield return new object[] { default(Padding), default(Padding), 0, 0 };
        yield return new object[] { new Padding(1, 2, 3, 4), new Padding(1, 2, 3, 4), 1, 1 };
        yield return new object[] { new Padding(1), new Padding(1), 1, 1 };
        yield return new object[] { new Padding(-1, -2, -3, -4), Padding.Empty, 1, 2 };
    }

    [WinFormsTheory]
    [MemberData(nameof(Padding_SetWithOwner_TestData))]
    public void ToolStripItem_Padding_SetWithOwner_GetReturnsExpected(Padding value, Padding expected, int expectedOwnerLayoutCallCount1, int expectedOwnerLayoutCallCount2)
    {
        using ToolStrip owner = new();
        using SubToolStripItem item = new()
        {
            Owner = owner
        };
        int ownerLayoutCallCount = 0;
        void ownerHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(owner, sender);
            Assert.Same(item, e.AffectedComponent);
            Assert.Equal("Padding", e.AffectedProperty);
            ownerLayoutCallCount++;
        }

        owner.Layout += ownerHandler;

        try
        {
            item.Padding = value;
            Assert.Equal(expected, item.Padding);
            Assert.Equal(expectedOwnerLayoutCallCount1, ownerLayoutCallCount);
            Assert.False(owner.IsHandleCreated);

            // Set same.
            item.Padding = value;
            Assert.Equal(expected, item.Padding);
            Assert.Equal(expectedOwnerLayoutCallCount2, ownerLayoutCallCount);
            Assert.False(owner.IsHandleCreated);
        }
        finally
        {
            owner.Layout -= ownerHandler;
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(Padding_SetWithOwner_TestData))]
    public void ToolStripItem_Padding_SetWithOwnerWithHandle_GetReturnsExpected(Padding value, Padding expected, int expectedOwnerLayoutCallCount1, int expectedOwnerLayoutCallCount2)
    {
        using ToolStrip owner = new();
        using SubToolStripItem item = new()
        {
            Owner = owner
        };
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;
        int ownerLayoutCallCount = 0;
        void ownerHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(owner, sender);
            Assert.Same(item, e.AffectedComponent);
            Assert.Equal("Padding", e.AffectedProperty);
            ownerLayoutCallCount++;
        }

        owner.Layout += ownerHandler;

        try
        {
            item.Padding = value;
            Assert.Equal(expected, item.Padding);
            Assert.Equal(expectedOwnerLayoutCallCount1, ownerLayoutCallCount);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(expectedOwnerLayoutCallCount1 * 2, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            item.Padding = value;
            Assert.Equal(expected, item.Padding);
            Assert.Equal(expectedOwnerLayoutCallCount2, ownerLayoutCallCount);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(expectedOwnerLayoutCallCount2 * 2, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }
        finally
        {
            owner.Layout -= ownerHandler;
        }
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetPaddingNormalizedTheoryData))]
    public void ToolStripItem_Padding_SetWithParent_GetReturnsExpected(Padding value, Padding expected)
    {
        using ToolStrip parent = new();
        using SubToolStripItem item = new()
        {
            Parent = parent
        };
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e) => parentLayoutCallCount++;
        parent.Layout += parentHandler;

        try
        {
            item.Padding = value;
            Assert.Equal(expected, item.Padding);
            Assert.Equal(0, parentLayoutCallCount);
            Assert.False(parent.IsHandleCreated);

            // Set same.
            item.Padding = value;
            Assert.Equal(expected, item.Padding);
            Assert.Equal(0, parentLayoutCallCount);
            Assert.False(parent.IsHandleCreated);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetPaddingNormalizedTheoryData))]
    public void ToolStripItem_Padding_SetWithParentWithHandle_GetReturnsExpected(Padding value, Padding expected)
    {
        using ToolStrip parent = new();
        using SubToolStripItem item = new()
        {
            Parent = parent
        };
        Assert.NotEqual(IntPtr.Zero, parent.Handle);
        int invalidatedCallCount = 0;
        parent.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        parent.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        parent.HandleCreated += (sender, e) => createdCallCount++;
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e) => parentLayoutCallCount++;
        parent.Layout += parentHandler;

        try
        {
            item.Padding = value;
            Assert.Equal(expected, item.Padding);
            Assert.Equal(0, parentLayoutCallCount);
            Assert.True(parent.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            item.Padding = value;
            Assert.Equal(expected, item.Padding);
            Assert.Equal(0, parentLayoutCallCount);
            Assert.True(parent.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    [WinFormsFact]
    public void ToolStripItem_Padding_ResetValue_Success()
    {
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ToolStripItem))[nameof(ToolStripItem.Padding)];
        using SubToolStripItem item = new();
        Assert.False(property.CanResetValue(item));

        item.Padding = new Padding(1, 2, 3, 4);
        Assert.Equal(new Padding(1, 2, 3, 4), item.Padding);
        Assert.True(property.CanResetValue(item));

        property.ResetValue(item);
        Assert.Equal(Padding.Empty, item.Padding);
        Assert.False(property.CanResetValue(item));
    }

    [WinFormsFact]
    public void ToolStripItem_Padding_ShouldSerializeValue_Success()
    {
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ToolStripItem))[nameof(ToolStripItem.Padding)];
        using SubToolStripItem item = new();
        Assert.False(property.ShouldSerializeValue(item));

        item.Padding = new Padding(1, 2, 3, 4);
        Assert.Equal(new Padding(1, 2, 3, 4), item.Padding);
        Assert.True(property.ShouldSerializeValue(item));

        property.ResetValue(item);
        Assert.Equal(Padding.Empty, item.Padding);
        Assert.False(property.ShouldSerializeValue(item));
    }

    public static IEnumerable<object[]> Parent_Set_TestData()
    {
        foreach (bool enabled in new bool[] { true, false })
        {
            foreach (bool visible in new bool[] { true, false })
            {
                foreach (Image image in new Image[] { null, new Bitmap(10, 10) })
                {
                    foreach (bool allowDrop in new bool[] { true, false })
                    {
                        yield return new object[] { enabled, visible, image, allowDrop };
                        yield return new object[] { enabled, visible, image, allowDrop };
                    }
                }
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(Parent_Set_TestData))]
    public void ToolStripItem_Parent_Set_GetReturnsExpected(bool enabled, bool visible, Image image, bool allowDrop)
    {
        using ToolStrip parent = new();
        using ToolStrip otherParent = new();
        using StatusStrip statusParent = new();
        using SubToolStripItem item = new()
        {
            Enabled = enabled,
            Visible = visible,
            Image = image,
            AllowDrop = allowDrop,
            Parent = parent
        };
        Assert.Same(parent, item.Parent);
        Assert.Same(parent, item.GetCurrentParent());
        Assert.Null(item.Owner);
        Assert.Empty(parent.Items);
        Assert.Equal(new Padding(0, 1, 0, 2), item.Margin);

        // Set same.
        item.Parent = parent;
        Assert.Same(parent, item.Parent);
        Assert.Same(parent, item.GetCurrentParent());
        Assert.Null(item.Owner);
        Assert.Empty(parent.Items);
        Assert.Empty(otherParent.Items);
        Assert.Equal(new Padding(0, 1, 0, 2), item.Margin);

        // Set different.
        item.Parent = otherParent;
        Assert.Same(otherParent, item.Parent);
        Assert.Null(item.Owner);
        Assert.Empty(parent.Items);
        Assert.Empty(otherParent.Items);
        Assert.Equal(new Padding(0, 1, 0, 2), item.Margin);

        // Set null.
        item.Parent = null;
        Assert.Null(item.Parent);
        Assert.Null(item.Owner);
        Assert.Empty(parent.Items);
        Assert.Empty(otherParent.Items);
        Assert.Equal(new Padding(0, 1, 0, 2), item.Margin);

        // Set status strip.
        item.Parent = statusParent;
        Assert.Same(statusParent, item.Parent);
        Assert.Null(item.Owner);
        Assert.Empty(parent.Items);
        Assert.Empty(otherParent.Items);
        Assert.Empty(statusParent.Items);
        Assert.Equal(new Padding(0, 1, 0, 2), item.Margin);
    }

    [WinFormsTheory]
    [MemberData(nameof(Parent_Set_TestData))]
    public void ToolStripItem_Parent_SetWithMargin_GetReturnsExpected(bool enabled, bool visible, Image image, bool allowDrop)
    {
        using ToolStrip parent = new();
        using ToolStrip otherParent = new();
        using StatusStrip statusParent = new();
        using SubToolStripItem item = new()
        {
            Enabled = enabled,
            Visible = visible,
            Image = image,
            AllowDrop = allowDrop,
            Margin = new Padding(1, 2, 3, 4),
            Parent = parent
        };
        Assert.Same(parent, item.Parent);
        Assert.Same(parent, item.GetCurrentParent());
        Assert.Null(item.Owner);
        Assert.Empty(parent.Items);
        Assert.Equal(new Padding(1, 2, 3, 4), item.Margin);

        // Set same.
        item.Parent = parent;
        Assert.Same(parent, item.Parent);
        Assert.Same(parent, item.GetCurrentParent());
        Assert.Null(item.Owner);
        Assert.Empty(parent.Items);
        Assert.Empty(otherParent.Items);
        Assert.Equal(new Padding(1, 2, 3, 4), item.Margin);

        // Set different.
        item.Parent = otherParent;
        Assert.Same(otherParent, item.Parent);
        Assert.Null(item.Owner);
        Assert.Empty(parent.Items);
        Assert.Empty(otherParent.Items);
        Assert.Equal(new Padding(1, 2, 3, 4), item.Margin);

        // Set null.
        item.Parent = null;
        Assert.Null(item.Parent);
        Assert.Null(item.Owner);
        Assert.Empty(parent.Items);
        Assert.Empty(otherParent.Items);
        Assert.Equal(new Padding(1, 2, 3, 4), item.Margin);

        // Set status strip.
        item.Parent = statusParent;
        Assert.Same(statusParent, item.Parent);
        Assert.Null(item.Owner);
        Assert.Empty(parent.Items);
        Assert.Empty(otherParent.Items);
        Assert.Empty(statusParent.Items);
        Assert.Equal(new Padding(1, 2, 3, 4), item.Margin);
    }

    [WinFormsFact]
    public void ToolStripItem_Releases_UiaProvider()
    {
        using ToolStripWithDisconnectCount toolStrip = new();

        using ToolStripDropDownItemWithAccessibleObjectFieldAccessor toolStripDropDownItem1 = new();
        toolStrip.Items.Add(toolStripDropDownItem1);

        using ToolStripDropDownItemWithAccessibleObjectFieldAccessor toolStripDropDownItem2 = new();
        toolStripDropDownItem1.DropDownItems.Add(toolStripDropDownItem2);

        _ = toolStrip.AccessibilityObject;
        Assert.True(toolStrip.IsAccessibilityObjectCreated);

        toolStrip.ReleaseUiaProvider(toolStrip.HWND);

        Assert.Equal(1, toolStrip.Disconnects);
        Assert.True(toolStripDropDownItem1.IsAccessibleObjectCleared());
        Assert.True(toolStripDropDownItem2.IsAccessibleObjectCleared());
        Assert.True(toolStrip.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(RightToLeft.Inherit, RightToLeft.No)]
    [InlineData(RightToLeft.Yes, RightToLeft.Yes)]
    [InlineData(RightToLeft.No, RightToLeft.No)]
    public void ToolStripItem_RightToLeft_GetWithOwner_ReturnsExpected(RightToLeft ownerRightToLeft, RightToLeft expected)
    {
        using ToolStrip owner = new()
        {
            RightToLeft = ownerRightToLeft
        };
        using SubToolStripItem item = new()
        {
            Owner = owner
        };
        Assert.Equal(expected, item.RightToLeft);
    }

    [WinFormsTheory]
    [InlineData(RightToLeft.Inherit, RightToLeft.No)]
    [InlineData(RightToLeft.Yes, RightToLeft.Yes)]
    [InlineData(RightToLeft.No, RightToLeft.No)]
    public void ToolStripItem_RightToLeft_GetWithParent_ReturnsExpected(RightToLeft parentRightToLeft, RightToLeft expected)
    {
        using ToolStrip parent = new()
        {
            RightToLeft = parentRightToLeft
        };
        using SubToolStripItem item = new()
        {
            Parent = parent
        };
        Assert.Equal(expected, item.RightToLeft);
    }

    [WinFormsFact]
    public void ToolStripItem_RightToLeft_GetWithOwnerAndParent_ReturnsExpected()
    {
        using ToolStrip owner = new()
        {
            RightToLeft = RightToLeft.Yes
        };
        using ToolStrip parent = new()
        {
            RightToLeft = RightToLeft.No
        };
        using SubToolStripItem item = new()
        {
            Owner = owner,
            Parent = parent
        };
        Assert.Equal(RightToLeft.Yes, item.RightToLeft);
    }

    [WinFormsTheory]
    [EnumData<RightToLeft>]
    public void ToolStripItem_RightToLeft_Set_GetReturnsExpected(RightToLeft value)
    {
        using SubToolStripItem item = new()
        {
            RightToLeft = value
        };
        Assert.Equal(value, item.RightToLeft);

        // Set same.
        item.RightToLeft = value;
        Assert.Equal(value, item.RightToLeft);
    }

    public static IEnumerable<object[]> RightToLeft_SetWithOwner_TestData()
    {
        yield return new object[] { RightToLeft.Yes, RightToLeft.Yes, RightToLeft.Yes, 0 };
        yield return new object[] { RightToLeft.Yes, RightToLeft.No, RightToLeft.No, 1 };
        yield return new object[] { RightToLeft.Yes, RightToLeft.Inherit, RightToLeft.Yes, 0 };
        yield return new object[] { RightToLeft.No, RightToLeft.Yes, RightToLeft.Yes, 1 };
        yield return new object[] { RightToLeft.No, RightToLeft.No, RightToLeft.No, 0 };
        yield return new object[] { RightToLeft.No, RightToLeft.Inherit, RightToLeft.No, 0 };
        yield return new object[] { RightToLeft.Inherit, RightToLeft.Yes, RightToLeft.Yes, 1 };
        yield return new object[] { RightToLeft.Inherit, RightToLeft.No, RightToLeft.No, 0 };
        yield return new object[] { RightToLeft.Inherit, RightToLeft.Inherit, RightToLeft.No, 0 };
    }

    [WinFormsTheory]
    [MemberData(nameof(RightToLeft_SetWithOwner_TestData))]
    public void ToolStripItem_RightToLeft_SetWithOwner_GetReturnsExpected(RightToLeft ownerRightToLeft, RightToLeft value, RightToLeft expected, int expectedOwnerLayoutCallCount)
    {
        using ToolStrip owner = new()
        {
            RightToLeft = ownerRightToLeft
        };
        using SubToolStripItem item = new()
        {
            Owner = owner
        };
        int ownerLayoutCallCount = 0;
        void ownerHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(owner, sender);
            Assert.Same(item, e.AffectedComponent);
            Assert.Equal("RightToLeft", e.AffectedProperty);
            ownerLayoutCallCount++;
        }

        owner.Layout += ownerHandler;

        try
        {
            item.RightToLeft = value;
            Assert.Equal(expected, item.RightToLeft);
            Assert.Equal(expectedOwnerLayoutCallCount, ownerLayoutCallCount);
            Assert.False(owner.IsHandleCreated);

            // Set same.
            item.RightToLeft = value;
            Assert.Equal(expected, item.RightToLeft);
            Assert.Equal(expectedOwnerLayoutCallCount, ownerLayoutCallCount);
            Assert.False(owner.IsHandleCreated);
        }
        finally
        {
            owner.Layout -= ownerHandler;
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(RightToLeft_SetWithOwner_TestData))]
    public void ToolStripItem_RightToLeft_SetWithOwnerWithHandle_GetReturnsExpected(RightToLeft ownerRightToLeft, RightToLeft value, RightToLeft expected, int expectedOwnerLayoutCallCount)
    {
        using ToolStrip owner = new()
        {
            RightToLeft = ownerRightToLeft
        };
        using SubToolStripItem item = new()
        {
            Owner = owner
        };
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;
        int ownerLayoutCallCount = 0;
        void ownerHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(owner, sender);
            Assert.Same(item, e.AffectedComponent);
            Assert.Equal("RightToLeft", e.AffectedProperty);
            ownerLayoutCallCount++;
        }

        owner.Layout += ownerHandler;

        try
        {
            item.RightToLeft = value;
            Assert.Equal(expected, item.RightToLeft);
            Assert.Equal(expectedOwnerLayoutCallCount, ownerLayoutCallCount);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(expectedOwnerLayoutCallCount * 2, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            item.RightToLeft = value;
            Assert.Equal(expected, item.RightToLeft);
            Assert.Equal(expectedOwnerLayoutCallCount, ownerLayoutCallCount);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(expectedOwnerLayoutCallCount * 2, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }
        finally
        {
            owner.Layout -= ownerHandler;
        }
    }

    public static IEnumerable<object[]> RightToLeft_SetWithParent_TestData()
    {
        yield return new object[] { RightToLeft.Yes, RightToLeft.Yes, RightToLeft.Yes };
        yield return new object[] { RightToLeft.Yes, RightToLeft.No, RightToLeft.No };
        yield return new object[] { RightToLeft.Yes, RightToLeft.Inherit, RightToLeft.Yes };
        yield return new object[] { RightToLeft.No, RightToLeft.Yes, RightToLeft.Yes };
        yield return new object[] { RightToLeft.No, RightToLeft.No, RightToLeft.No };
        yield return new object[] { RightToLeft.No, RightToLeft.Inherit, RightToLeft.No };
        yield return new object[] { RightToLeft.Inherit, RightToLeft.Yes, RightToLeft.Yes };
        yield return new object[] { RightToLeft.Inherit, RightToLeft.No, RightToLeft.No };
        yield return new object[] { RightToLeft.Inherit, RightToLeft.Inherit, RightToLeft.No };
    }

    [WinFormsTheory]
    [MemberData(nameof(RightToLeft_SetWithParent_TestData))]
    public void ToolStripItem_RightToLeft_SetWithParent_GetReturnsExpected(RightToLeft parentRightToLeft, RightToLeft value, RightToLeft expected)
    {
        using ToolStrip parent = new()
        {
            RightToLeft = parentRightToLeft
        };
        using SubToolStripItem item = new()
        {
            Parent = parent
        };
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e) => parentLayoutCallCount++;
        parent.Layout += parentHandler;

        try
        {
            item.RightToLeft = value;
            Assert.Equal(expected, item.RightToLeft);
            Assert.Equal(0, parentLayoutCallCount);
            Assert.False(parent.IsHandleCreated);

            // Set same.
            item.RightToLeft = value;
            Assert.Equal(expected, item.RightToLeft);
            Assert.Equal(0, parentLayoutCallCount);
            Assert.False(parent.IsHandleCreated);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(RightToLeft_SetWithParent_TestData))]
    public void ToolStripItem_RightToLeft_SetWithParentWithHandle_GetReturnsExpected(RightToLeft parentRightToLeft, RightToLeft value, RightToLeft expected)
    {
        using ToolStrip parent = new()
        {
            RightToLeft = parentRightToLeft
        };
        using SubToolStripItem item = new()
        {
            Parent = parent
        };
        Assert.NotEqual(IntPtr.Zero, parent.Handle);
        int invalidatedCallCount = 0;
        parent.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        parent.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        parent.HandleCreated += (sender, e) => createdCallCount++;
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e) => parentLayoutCallCount++;
        parent.Layout += parentHandler;

        try
        {
            item.RightToLeft = value;
            Assert.Equal(expected, item.RightToLeft);
            Assert.Equal(0, parentLayoutCallCount);
            Assert.True(parent.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            item.RightToLeft = value;
            Assert.Equal(expected, item.RightToLeft);
            Assert.Equal(0, parentLayoutCallCount);
            Assert.True(parent.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    [WinFormsFact]
    public void ToolStripItem_RightToLeft_SetWithHandler_CallsRightToLeftChanged()
    {
        using SubToolStripItem item = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(item, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        item.RightToLeftChanged += handler;

        // Set different.
        item.RightToLeft = RightToLeft.Yes;
        Assert.Equal(RightToLeft.Yes, item.RightToLeft);
        Assert.Equal(1, callCount);

        // Set same.
        item.RightToLeft = RightToLeft.Yes;
        Assert.Equal(RightToLeft.Yes, item.RightToLeft);
        Assert.Equal(1, callCount);

        // Set different.
        item.RightToLeft = RightToLeft.No;
        Assert.Equal(RightToLeft.No, item.RightToLeft);
        Assert.Equal(2, callCount);

        // Remove handler.
        item.RightToLeftChanged -= handler;
        item.RightToLeft = RightToLeft.Yes;
        Assert.Equal(RightToLeft.Yes, item.RightToLeft);
        Assert.Equal(2, callCount);
    }

    [WinFormsTheory]
    [InvalidEnumData<RightToLeft>]
    public void ToolStripItem_RightToLeft_SetInvalid_ThrowsInvalidEnumArgumentException(RightToLeft value)
    {
        using SubToolStripItem item = new();
        Assert.Throws<InvalidEnumArgumentException>("value", () => item.RightToLeft = value);
    }

    [WinFormsFact]
    public void ToolStripItem_RightToLeft_ResetValue_Success()
    {
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ToolStripItem))[nameof(ToolStripItem.RightToLeft)];
        using SubToolStripItem item = new();
        Assert.False(property.CanResetValue(item));

        item.RightToLeft = RightToLeft.Yes;
        Assert.Equal(RightToLeft.Yes, item.RightToLeft);
        Assert.True(property.CanResetValue(item));

        item.RightToLeft = RightToLeft.No;
        Assert.Equal(RightToLeft.No, item.RightToLeft);
        Assert.True(property.CanResetValue(item));

        item.RightToLeft = RightToLeft.Inherit;
        Assert.Equal(RightToLeft.Inherit, item.RightToLeft);
        Assert.False(property.CanResetValue(item));

        item.RightToLeft = RightToLeft.No;
        Assert.Equal(RightToLeft.No, item.RightToLeft);
        Assert.True(property.CanResetValue(item));

        property.ResetValue(item);
        Assert.Equal(RightToLeft.Inherit, item.RightToLeft);
        Assert.False(property.CanResetValue(item));
    }

    [WinFormsFact]
    public void ToolStripItem_RightToLeft_ShouldSerializeValue_Success()
    {
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ToolStripItem))[nameof(ToolStripItem.RightToLeft)];
        using SubToolStripItem item = new();
        Assert.False(property.ShouldSerializeValue(item));

        item.RightToLeft = RightToLeft.Yes;
        Assert.Equal(RightToLeft.Yes, item.RightToLeft);
        Assert.True(property.ShouldSerializeValue(item));

        item.RightToLeft = RightToLeft.No;
        Assert.Equal(RightToLeft.No, item.RightToLeft);
        Assert.True(property.ShouldSerializeValue(item));

        item.RightToLeft = RightToLeft.Inherit;
        Assert.Equal(RightToLeft.Inherit, item.RightToLeft);
        Assert.False(property.ShouldSerializeValue(item));

        item.RightToLeft = RightToLeft.No;
        Assert.Equal(RightToLeft.No, item.RightToLeft);
        Assert.True(property.ShouldSerializeValue(item));

        property.ResetValue(item);
        Assert.Equal(RightToLeft.Inherit, item.RightToLeft);
        Assert.False(property.ShouldSerializeValue(item));
    }

    [WinFormsTheory]
    [BoolData]
    public void ToolStripItem_RightToLeftAutoMirrorImage_Set_GetReturnsExpected(bool value)
    {
        using SubToolStripItem item = new()
        {
            RightToLeftAutoMirrorImage = value
        };
        Assert.Equal(value, item.RightToLeftAutoMirrorImage);

        // Set same.
        item.RightToLeftAutoMirrorImage = value;
        Assert.Equal(value, item.RightToLeftAutoMirrorImage);

        // Set different.
        item.RightToLeftAutoMirrorImage = !value;
        Assert.Equal(!value, item.RightToLeftAutoMirrorImage);
    }

    [WinFormsTheory]
    [BoolData]
    public void ToolStripItem_RightToLeftAutoMirrorImage_SetWithOwner_GetReturnsExpected(bool value)
    {
        using ToolStrip owner = new();
        using SubToolStripItem item = new()
        {
            Owner = owner
        };
        int ownerLayoutCallCount = 0;
        void ownerHandler(object sender, LayoutEventArgs e) => ownerLayoutCallCount++;
        owner.Layout += ownerHandler;

        try
        {
            item.RightToLeftAutoMirrorImage = value;
            Assert.Equal(value, item.RightToLeftAutoMirrorImage);
            Assert.Equal(0, ownerLayoutCallCount);
            Assert.False(owner.IsHandleCreated);

            // Set same.
            item.RightToLeftAutoMirrorImage = value;
            Assert.Equal(value, item.RightToLeftAutoMirrorImage);
            Assert.Equal(0, ownerLayoutCallCount);
            Assert.False(owner.IsHandleCreated);

            // Set different.
            item.RightToLeftAutoMirrorImage = !value;
            Assert.Equal(!value, item.RightToLeftAutoMirrorImage);
            Assert.Equal(0, ownerLayoutCallCount);
            Assert.False(owner.IsHandleCreated);
        }
        finally
        {
            owner.Layout -= ownerHandler;
        }
    }

    [WinFormsTheory]
    [BoolData]
    public void ToolStripItem_RightToLeftAutoMirrorImage_SetWithOwnerWithHandle_GetReturnsExpected(bool value)
    {
        using ToolStrip owner = new();
        using SubToolStripItem item = new()
        {
            Owner = owner
        };
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;
        int ownerLayoutCallCount = 0;
        void ownerHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(owner, sender);
            Assert.Same(item, e.AffectedComponent);
            Assert.Equal("RightToLeftAutoMirrorImage", e.AffectedProperty);
            ownerLayoutCallCount++;
        }

        owner.Layout += ownerHandler;

        try
        {
            item.RightToLeftAutoMirrorImage = value;
            Assert.Equal(value, item.RightToLeftAutoMirrorImage);
            Assert.Equal(0, ownerLayoutCallCount);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            item.RightToLeftAutoMirrorImage = value;
            Assert.Equal(value, item.RightToLeftAutoMirrorImage);
            Assert.Equal(0, ownerLayoutCallCount);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set different.
            item.RightToLeftAutoMirrorImage = !value;
            Assert.Equal(!value, item.RightToLeftAutoMirrorImage);
            Assert.Equal(0, ownerLayoutCallCount);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }
        finally
        {
            owner.Layout -= ownerHandler;
        }
    }

    [WinFormsTheory]
    [BoolData]
    public void ToolStripItem_RightToLeftAutoMirrorImage_SetWithParent_GetReturnsExpected(bool value)
    {
        using ToolStrip parent = new();
        using SubToolStripItem item = new()
        {
            Parent = parent
        };
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e) => parentLayoutCallCount++;
        parent.Layout += parentHandler;

        try
        {
            item.RightToLeftAutoMirrorImage = value;
            Assert.Equal(value, item.RightToLeftAutoMirrorImage);
            Assert.Equal(0, parentLayoutCallCount);
            Assert.False(parent.IsHandleCreated);

            // Set same.
            item.RightToLeftAutoMirrorImage = value;
            Assert.Equal(value, item.RightToLeftAutoMirrorImage);
            Assert.Equal(0, parentLayoutCallCount);
            Assert.False(parent.IsHandleCreated);

            // Set different.
            item.RightToLeftAutoMirrorImage = !value;
            Assert.Equal(!value, item.RightToLeftAutoMirrorImage);
            Assert.Equal(0, parentLayoutCallCount);
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
    public void ToolStripItem_RightToLeftAutoMirrorImage_SetWithParentWithHandle_GetReturnsExpected(bool value, int expectedParentInvalidatedCallCount)
    {
        using ToolStrip parent = new();
        using SubToolStripItem item = new()
        {
            Parent = parent
        };
        Assert.NotEqual(IntPtr.Zero, parent.Handle);
        int invalidatedCallCount = 0;
        parent.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        parent.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        parent.HandleCreated += (sender, e) => createdCallCount++;
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e) => parentLayoutCallCount++;
        parent.Layout += parentHandler;

        try
        {
            item.RightToLeftAutoMirrorImage = value;
            Assert.Equal(value, item.RightToLeftAutoMirrorImage);
            Assert.Equal(0, parentLayoutCallCount);
            Assert.True(parent.IsHandleCreated);
            Assert.Equal(expectedParentInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            item.RightToLeftAutoMirrorImage = value;
            Assert.Equal(value, item.RightToLeftAutoMirrorImage);
            Assert.Equal(0, parentLayoutCallCount);
            Assert.True(parent.IsHandleCreated);
            Assert.Equal(expectedParentInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set different.
            item.RightToLeftAutoMirrorImage = !value;
            Assert.Equal(!value, item.RightToLeftAutoMirrorImage);
            Assert.Equal(0, parentLayoutCallCount);
            Assert.True(parent.IsHandleCreated);
            Assert.Equal(expectedParentInvalidatedCallCount + 1, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    [WinFormsFact]
    public void ToolStripItem_Selected_GetCantSelect_ReturnsFalse()
    {
        using CannotSelectToolStripItem item = new();
        Assert.False(item.Selected);
    }

    [WinFormsFact]
    public void ToolStripItem_Selected_GetDesignMode_ReturnsFalse()
    {
        Mock<ISite> mockSite = new(MockBehavior.Strict);
        mockSite
            .Setup(s => s.Name)
            .Returns("Name");
        mockSite
            .Setup(s => s.DesignMode)
            .Returns(true);
        mockSite
            .Setup(s => s.Container)
            .Returns((IContainer)null);
        using SubToolStripItem item = new()
        {
            Site = mockSite.Object
        };
        Assert.False(item.Selected);
    }

    [WinFormsFact]
    public void ToolStripItem_Selected_GetWithOwner_ReturnsFalse()
    {
        using ToolStrip owner = new();
        using SubToolStripItem item = new()
        {
            Owner = owner
        };
        Assert.False(item.Selected);
    }

    [WinFormsFact]
    public void ToolStripItem_Selected_GetWithDraggingOwner_ReturnsFalse()
    {
        using SubToolStrip owner = new();
        owner.OnBeginDrag(EventArgs.Empty);
        using SubToolStripItem item = new()
        {
            Owner = owner
        };
        Assert.False(item.Selected);
    }

    [WinFormsFact]
    public void ToolStripItem_Selected_GetWithSelectionSuspendedOwner_ReturnsFalse()
    {
        using SubToolStrip owner = new();
        owner.OnBeginDrag(EventArgs.Empty);
        using SubToolStripItem item = new()
        {
            Owner = owner
        };
        Assert.NotNull(owner.GetItemAt(item.Bounds.X, item.Bounds.Y));
        owner.OnMouseDown(new MouseEventArgs(MouseButtons.Left, 1, item.Bounds.X, item.Bounds.Y, 0));

        Assert.False(item.Selected);
    }

    [WinFormsFact]
    public void ToolStripItem_Selected_GetWithParent_ReturnsFalse()
    {
        using ToolStrip parent = new();
        using SubToolStripItem item = new()
        {
            Parent = parent
        };
        Assert.False(item.Selected);
    }

    [WinFormsFact]
    public void ToolStripItem_Selected_GetWithDraggingParent_ReturnsFalse()
    {
        using SubToolStrip parent = new();
        parent.OnBeginDrag(EventArgs.Empty);
        using SubToolStripItem item = new()
        {
            Parent = parent
        };
        Assert.False(item.Selected);
    }

    [WinFormsFact]
    public void ToolStripItem_Selected_GetWithSelectionSuspendedParentNotCurrentItem_ReturnsFalse()
    {
        using SubToolStrip parent = new();
        parent.OnBeginDrag(EventArgs.Empty);
        using SubToolStripItem item = new()
        {
            Parent = parent
        };
        Assert.NotNull(parent.GetItemAt(item.Bounds.X, item.Bounds.Y));
        parent.OnMouseDown(new MouseEventArgs(MouseButtons.Left, 1, item.Bounds.X, item.Bounds.Y, 0));

        Assert.False(item.Selected);
    }

    [WinFormsFact]
    public void ToolStripItem_Selected_GetWithSelectionSuspendedParentCurrentItem_ReturnsTrue()
    {
        using SubToolStrip parent = new();
        parent.OnBeginDrag(EventArgs.Empty);
        using SubToolStripItem item = new()
        {
            Parent = parent,
            Owner = parent,
        };
        Assert.True(item.Visible);
        parent.PerformLayout();
        Assert.Same(item, parent.GetItemAt(item.Bounds.X, item.Bounds.Y));
        parent.OnMouseDown(new MouseEventArgs(MouseButtons.Left, 1, item.Bounds.X, item.Bounds.Y, 0));

        Assert.True(item.Selected);
    }

    [WinFormsFact]
    public void ToolStripItem_ShowKeyboardCues_GetDesignMode_ReturnsTrue()
    {
        Mock<ISite> mockSite = new();
        mockSite
        .Setup(s => s.DesignMode)
        .Returns(true);
        using SubToolStripItem item = new()
        {
            Site = mockSite.Object
        };
        Assert.True(item.ShowKeyboardCues);
    }

    [WinFormsFact]
    public void ToolStripItem_Size_GetWithDefaultSize_ReturnsExpected()
    {
        using CustomDefaultSizeToolStripItem item = new();
        Assert.Equal(new Size(10, 11), item.Size);
    }

    private class CustomDefaultSizeToolStripItem : ToolStripItem
    {
        protected override Size DefaultSize => new(10, 11);
    }

    public static IEnumerable<object[]> Size_Set_TestData()
    {
        yield return new object[] { new Size(-1, -2) };
        yield return new object[] { new Size(0, 0) };
        yield return new object[] { new Size(1, 2) };
        yield return new object[] { new Size(22, 23) };
        yield return new object[] { new Size(23, 22) };
        yield return new object[] { new Size(23, 23) };
    }

    [WinFormsTheory]
    [MemberData(nameof(Size_Set_TestData))]
    public void ToolStripItem_Size_Set_GetReturnsExpected(Size value)
    {
        using SubToolStripItem item = new();
        int locationChangedCallCount = 0;
        item.LocationChanged += (sender, e) => locationChangedCallCount++;

        item.Size = value;
        Assert.Equal(value, item.Size);
        Assert.Equal(0, locationChangedCallCount);

        // Set same.
        item.Size = value;
        Assert.Equal(value, item.Size);
        Assert.Equal(0, locationChangedCallCount);
    }

    [WinFormsTheory]
    [MemberData(nameof(Size_Set_TestData))]
    public void ToolStripItem_Size_SetWithOwner_GetReturnsExpected(Size value)
    {
        using ToolStrip owner = new();
        using SubToolStripItem item = new()
        {
            Owner = owner
        };
        int ownerLayoutCallCount = 0;
        void ownerHandler(object sender, LayoutEventArgs e) => ownerLayoutCallCount++;
        owner.Layout += ownerHandler;
        int locationChangedCallCount = 0;
        item.LocationChanged += (sender, e) => locationChangedCallCount++;

        try
        {
            item.Size = value;
            Assert.Equal(value, item.Size);
            Assert.Equal(0, locationChangedCallCount);
            Assert.Equal(0, ownerLayoutCallCount);
            Assert.False(owner.IsHandleCreated);

            // Set same.
            item.Size = value;
            Assert.Equal(value, item.Size);
            Assert.Equal(0, locationChangedCallCount);
            Assert.Equal(0, ownerLayoutCallCount);
            Assert.False(owner.IsHandleCreated);
        }
        finally
        {
            owner.Layout -= ownerHandler;
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(Size_Set_TestData))]
    public void ToolStripItem_Size_SetWithOwnerWithHandle_GetReturnsExpected(Size value)
    {
        using ToolStrip owner = new();
        using SubToolStripItem item = new()
        {
            Owner = owner
        };
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;
        int ownerLayoutCallCount = 0;
        void ownerHandler(object sender, LayoutEventArgs e) => ownerLayoutCallCount++;
        owner.Layout += ownerHandler;
        int locationChangedCallCount = 0;
        item.LocationChanged += (sender, e) => locationChangedCallCount++;

        try
        {
            item.Size = value;
            Assert.Equal(value, item.Size);
            Assert.Equal(0, locationChangedCallCount);
            Assert.Equal(0, ownerLayoutCallCount);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            item.Size = value;
            Assert.Equal(value, item.Size);
            Assert.Equal(0, locationChangedCallCount);
            Assert.Equal(0, ownerLayoutCallCount);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }
        finally
        {
            owner.Layout -= ownerHandler;
        }
    }

    public static IEnumerable<object[]> Size_SetWithParent_TestData()
    {
        yield return new object[] { new Size(-1, -2), 1 };
        yield return new object[] { new Size(0, 0), 1 };
        yield return new object[] { new Size(1, 2), 1 };
        yield return new object[] { new Size(22, 23), 1 };
        yield return new object[] { new Size(23, 22), 1 };
        yield return new object[] { new Size(23, 23), 0 };
    }

    [WinFormsTheory]
    [MemberData(nameof(Size_SetWithParent_TestData))]
    public void ToolStripItem_Size_SetWithParent_GetReturnsExpected(Size value, int expectedParentLayoutCallCount)
    {
        using ToolStrip parent = new();
        using SubToolStripItem item = new()
        {
            Parent = parent
        };
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(parent, sender);
            Assert.Same(item, e.AffectedComponent);
            Assert.Equal("Bounds", e.AffectedProperty);
            parentLayoutCallCount++;
        }

        parent.Layout += parentHandler;
        int locationChangedCallCount = 0;
        item.LocationChanged += (sender, e) => locationChangedCallCount++;

        try
        {
            item.Size = value;
            Assert.Equal(value, item.Size);
            Assert.Equal(0, locationChangedCallCount);
            Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
            Assert.False(parent.IsHandleCreated);

            // Set same.
            item.Size = value;
            Assert.Equal(value, item.Size);
            Assert.Equal(0, locationChangedCallCount);
            Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
            Assert.False(parent.IsHandleCreated);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(Size_SetWithParent_TestData))]
    public void ToolStripItem_Size_SetWithParentWithHandle_GetReturnsExpected(Size value, int expectedParentLayoutCallCount)
    {
        using ToolStrip parent = new();
        using SubToolStripItem item = new()
        {
            Parent = parent
        };
        Assert.NotEqual(IntPtr.Zero, parent.Handle);
        int invalidatedCallCount = 0;
        parent.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        parent.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        parent.HandleCreated += (sender, e) => createdCallCount++;
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(parent, sender);
            Assert.Same(item, e.AffectedComponent);
            Assert.Equal("Bounds", e.AffectedProperty);
            parentLayoutCallCount++;
        }

        parent.Layout += parentHandler;
        int locationChangedCallCount = 0;
        item.LocationChanged += (sender, e) => locationChangedCallCount++;

        try
        {
            item.Size = value;
            Assert.Equal(value, item.Size);
            Assert.Equal(0, locationChangedCallCount);
            Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
            Assert.True(parent.IsHandleCreated);
            Assert.Equal(expectedParentLayoutCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            item.Size = value;
            Assert.Equal(value, item.Size);
            Assert.Equal(0, locationChangedCallCount);
            Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
            Assert.True(parent.IsHandleCreated);
            Assert.Equal(expectedParentLayoutCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    [WinFormsTheory]
    [StringWithNullData]
    public void ToolStripItem_Tag_Set_GetReturnsExpected(string value)
    {
        using SubToolStripItem item = new()
        {
            Tag = value
        };
        Assert.Same(value, item.Tag);

        // Set same.
        item.Tag = value;
        Assert.Same(value, item.Tag);
    }

    [WinFormsTheory]
    [StringWithNullData]
    public void ToolStripItem_Text_Set_GetReturnsExpected(string value)
    {
        using SubToolStripItem item = new()
        {
            Text = value
        };
        Assert.Equal(value, item.Text);

        // Set same.
        item.Text = value;
        Assert.Equal(value, item.Text);
    }

    [WinFormsTheory]
    [InlineData(null, 1)]
    [InlineData("", 0)]
    [InlineData("text", 1)]
    public void ToolStripItem_Text_SetWithOwner_GetReturnsExpected(string value, int expectedOwnerLayoutCallCount)
    {
        using ToolStrip owner = new();
        using SubToolStripItem item = new()
        {
            Owner = owner
        };
        int ownerLayoutCallCount = 0;
        void ownerHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(owner, sender);
            Assert.Same(item, e.AffectedComponent);
            Assert.Equal("Text", e.AffectedProperty);
            ownerLayoutCallCount++;
        }

        owner.Layout += ownerHandler;

        try
        {
            item.Text = value;
            Assert.Equal(value, item.Text);
            Assert.Equal(expectedOwnerLayoutCallCount, ownerLayoutCallCount);
            Assert.False(owner.IsHandleCreated);

            // Set same.
            item.Text = value;
            Assert.Equal(value, item.Text);
            Assert.False(owner.IsHandleCreated);
            Assert.Equal(expectedOwnerLayoutCallCount, ownerLayoutCallCount);
            Assert.False(owner.IsHandleCreated);
        }
        finally
        {
            owner.Layout -= ownerHandler;
        }
    }

    [WinFormsTheory]
    [InlineData(null, 1)]
    [InlineData("", 0)]
    [InlineData("text", 1)]
    public void ToolStripItem_Text_SetWithOwnerWithHandle_GetReturnsExpected(string value, int expectedOwnerLayoutCallCount)
    {
        using ToolStrip owner = new();
        using SubToolStripItem item = new()
        {
            Owner = owner
        };
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;
        int ownerLayoutCallCount = 0;
        void ownerHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(owner, sender);
            Assert.Same(item, e.AffectedComponent);
            Assert.Equal("Text", e.AffectedProperty);
            ownerLayoutCallCount++;
        }

        owner.Layout += ownerHandler;

        try
        {
            item.Text = value;
            Assert.Equal(value, item.Text);
            Assert.Equal(expectedOwnerLayoutCallCount, ownerLayoutCallCount);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(expectedOwnerLayoutCallCount * 2, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            item.Text = value;
            Assert.Equal(value, item.Text);
            Assert.Equal(expectedOwnerLayoutCallCount, ownerLayoutCallCount);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(expectedOwnerLayoutCallCount * 2, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }
        finally
        {
            owner.Layout -= ownerHandler;
        }
    }

    [WinFormsTheory]
    [StringWithNullData]
    public void ToolStripItem_Text_SetWithParent_GetReturnsExpected(string value)
    {
        using ToolStrip parent = new();
        using SubToolStripItem item = new()
        {
            Parent = parent
        };
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e) => parentLayoutCallCount++;
        parent.Layout += parentHandler;

        try
        {
            item.Text = value;
            Assert.Equal(value, item.Text);
            Assert.Equal(0, parentLayoutCallCount);
            Assert.False(parent.IsHandleCreated);

            // Set same.
            item.Text = value;
            Assert.Equal(value, item.Text);
            Assert.False(parent.IsHandleCreated);
            Assert.Equal(0, parentLayoutCallCount);
            Assert.False(parent.IsHandleCreated);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    [WinFormsTheory]
    [StringWithNullData]
    public void ToolStripItem_Text_SetWithParentWithHandle_GetReturnsExpected(string value)
    {
        using ToolStrip parent = new();
        using SubToolStripItem item = new()
        {
            Parent = parent
        };
        Assert.NotEqual(IntPtr.Zero, parent.Handle);
        int invalidatedCallCount = 0;
        parent.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        parent.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        parent.HandleCreated += (sender, e) => createdCallCount++;
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e) => parentLayoutCallCount++;
        parent.Layout += parentHandler;

        try
        {
            item.Text = value;
            Assert.Equal(value, item.Text);
            Assert.Equal(0, parentLayoutCallCount);
            Assert.True(parent.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            item.Text = value;
            Assert.Equal(value, item.Text);
            Assert.Equal(0, parentLayoutCallCount);
            Assert.True(parent.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    [WinFormsFact]
    public void ToolStripItem_Text_SetWithHandler_CallsTextChanged()
    {
        using SubToolStripItem item = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(item, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        item.TextChanged += handler;

        // Set different.
        item.Text = "value";
        Assert.Equal("value", item.Text);
        Assert.Equal(1, callCount);

        // Set same.
        item.Text = "value";
        Assert.Equal("value", item.Text);
        Assert.Equal(1, callCount);

        // Set different.
        item.Text = string.Empty;
        Assert.Empty(item.Text);
        Assert.Equal(2, callCount);

        // Remove handler.
        item.TextChanged -= handler;
        item.Text = "value";
        Assert.Equal("value", item.Text);
        Assert.Equal(2, callCount);
    }

    [WinFormsTheory]
    [EnumData<ContentAlignment>]
    public void ToolStripItem_TextAlign_Set_GetReturnsExpected(ContentAlignment value)
    {
        using SubToolStripItem item = new()
        {
            TextAlign = value
        };
        Assert.Equal(value, item.TextAlign);

        // Set same.
        item.TextAlign = value;
        Assert.Equal(value, item.TextAlign);
    }

    [WinFormsTheory]
    [InlineData(ContentAlignment.TopLeft, 1)]
    [InlineData(ContentAlignment.TopCenter, 1)]
    [InlineData(ContentAlignment.TopRight, 1)]
    [InlineData(ContentAlignment.MiddleLeft, 1)]
    [InlineData(ContentAlignment.MiddleCenter, 0)]
    [InlineData(ContentAlignment.MiddleRight, 1)]
    [InlineData(ContentAlignment.BottomLeft, 1)]
    [InlineData(ContentAlignment.BottomCenter, 1)]
    [InlineData(ContentAlignment.BottomRight, 1)]
    public void ToolStripItem_TextAlign_SetWithOwner_GetReturnsExpected(ContentAlignment value, int expectedParentLayoutCallCount)
    {
        using ToolStrip owner = new();
        using SubToolStripItem item = new()
        {
            Owner = owner
        };
        int ownerLayoutCallCount = 0;
        void ownerHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(owner, sender);
            Assert.Same(item, e.AffectedComponent);
            Assert.Equal("TextAlign", e.AffectedProperty);
            ownerLayoutCallCount++;
        }

        owner.Layout += ownerHandler;

        try
        {
            item.TextAlign = value;
            Assert.Equal(value, item.TextAlign);
            Assert.Equal(expectedParentLayoutCallCount, ownerLayoutCallCount);
            Assert.False(owner.IsHandleCreated);

            // Set same.
            item.TextAlign = value;
            Assert.Equal(value, item.TextAlign);
            Assert.Equal(expectedParentLayoutCallCount, ownerLayoutCallCount);
            Assert.False(owner.IsHandleCreated);
        }
        finally
        {
            owner.Layout -= ownerHandler;
        }
    }

    [WinFormsTheory]
    [InlineData(ContentAlignment.TopLeft, 1)]
    [InlineData(ContentAlignment.TopCenter, 1)]
    [InlineData(ContentAlignment.TopRight, 1)]
    [InlineData(ContentAlignment.MiddleLeft, 1)]
    [InlineData(ContentAlignment.MiddleCenter, 0)]
    [InlineData(ContentAlignment.MiddleRight, 1)]
    [InlineData(ContentAlignment.BottomLeft, 1)]
    [InlineData(ContentAlignment.BottomCenter, 1)]
    [InlineData(ContentAlignment.BottomRight, 1)]
    public void ToolStripItem_TextAlign_SetWithOwnerWithHandle_GetReturnsExpected(ContentAlignment value, int expectedParentLayoutCallCount)
    {
        using ToolStrip owner = new();
        using SubToolStripItem item = new()
        {
            Owner = owner
        };
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;
        int ownerLayoutCallCount = 0;
        void ownerHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(owner, sender);
            Assert.Same(item, e.AffectedComponent);
            Assert.Equal("TextAlign", e.AffectedProperty);
            ownerLayoutCallCount++;
        }

        owner.Layout += ownerHandler;

        try
        {
            item.TextAlign = value;
            Assert.Equal(value, item.TextAlign);
            Assert.Equal(expectedParentLayoutCallCount, ownerLayoutCallCount);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(expectedParentLayoutCallCount * 2, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            item.TextAlign = value;
            Assert.Equal(value, item.TextAlign);
            Assert.Equal(expectedParentLayoutCallCount, ownerLayoutCallCount);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(expectedParentLayoutCallCount * 2, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }
        finally
        {
            owner.Layout -= ownerHandler;
        }
    }

    [WinFormsTheory]
    [EnumData<ContentAlignment>]
    public void ToolStripItem_TextAlign_SetWithParent_GetReturnsExpected(ContentAlignment value)
    {
        using ToolStrip parent = new();
        using SubToolStripItem item = new()
        {
            Parent = parent
        };
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e) => parentLayoutCallCount++;
        parent.Layout += parentHandler;

        try
        {
            item.TextAlign = value;
            Assert.Equal(value, item.TextAlign);
            Assert.Equal(0, parentLayoutCallCount);
            Assert.False(parent.IsHandleCreated);

            // Set same.
            item.TextAlign = value;
            Assert.Equal(value, item.TextAlign);
            Assert.Equal(0, parentLayoutCallCount);
            Assert.False(parent.IsHandleCreated);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    [WinFormsTheory]
    [EnumData<ContentAlignment>]
    public void ToolStripItem_TextAlign_SetWithParentWithHandle_GetReturnsExpected(ContentAlignment value)
    {
        using ToolStrip parent = new();
        using SubToolStripItem item = new()
        {
            Parent = parent
        };
        Assert.NotEqual(IntPtr.Zero, parent.Handle);
        int invalidatedCallCount = 0;
        parent.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        parent.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        parent.HandleCreated += (sender, e) => createdCallCount++;
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e) => parentLayoutCallCount++;
        parent.Layout += parentHandler;

        try
        {
            item.TextAlign = value;
            Assert.Equal(value, item.TextAlign);
            Assert.Equal(0, parentLayoutCallCount);
            Assert.True(parent.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            item.TextAlign = value;
            Assert.Equal(value, item.TextAlign);
            Assert.Equal(0, parentLayoutCallCount);
            Assert.True(parent.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    [WinFormsTheory]
    [InvalidEnumData<ContentAlignment>]
    [InlineData((ContentAlignment)int.MaxValue)]
    [InlineData((ContentAlignment)int.MinValue)]
    public void ToolStripItem_TextAlign_SetInvalid_ThrowsInvalidEnumArgumentException(ContentAlignment value)
    {
        using SubToolStripItem item = new();
        Assert.Throws<InvalidEnumArgumentException>("value", () => item.TextAlign = value);
    }

    [WinFormsTheory]
    [InlineData(ToolStripTextDirection.Inherit, ToolStripTextDirection.Horizontal)]
    [InlineData(ToolStripTextDirection.Horizontal, ToolStripTextDirection.Horizontal)]
    [InlineData(ToolStripTextDirection.Vertical90, ToolStripTextDirection.Vertical90)]
    [InlineData(ToolStripTextDirection.Vertical270, ToolStripTextDirection.Vertical270)]
    public void ToolStripItem_TextDirection_GetWithOwner_ReturnsExpected(ToolStripTextDirection ownerTextDirection, ToolStripTextDirection expected)
    {
        using ToolStrip owner = new()
        {
            TextDirection = ownerTextDirection
        };
        using SubToolStripItem item = new()
        {
            Owner = owner
        };
        Assert.Equal(expected, item.TextDirection);
    }

    [WinFormsTheory]
    [InlineData(ToolStripTextDirection.Inherit, ToolStripTextDirection.Horizontal)]
    [InlineData(ToolStripTextDirection.Horizontal, ToolStripTextDirection.Horizontal)]
    [InlineData(ToolStripTextDirection.Vertical90, ToolStripTextDirection.Vertical90)]
    [InlineData(ToolStripTextDirection.Vertical270, ToolStripTextDirection.Vertical270)]
    public void ToolStripItem_TextDirection_GetWithParent_ReturnsExpected(ToolStripTextDirection parentTextDirection, ToolStripTextDirection expected)
    {
        using ToolStrip parent = new()
        {
            TextDirection = parentTextDirection
        };
        using SubToolStripItem item = new()
        {
            Parent = parent
        };
        Assert.Equal(expected, item.TextDirection);
    }

    [WinFormsFact]
    public void ToolStripItem_TextDirection_GetWithOwnerAndParent_ReturnsExpected()
    {
        using ToolStrip owner = new()
        {
            TextDirection = ToolStripTextDirection.Vertical90
        };
        using ToolStrip parent = new()
        {
            TextDirection = ToolStripTextDirection.Vertical270
        };
        using SubToolStripItem item = new()
        {
            Owner = owner,
            Parent = parent
        };
        Assert.Equal(ToolStripTextDirection.Vertical270, item.TextDirection);
    }

    public static IEnumerable<object[]> TextDirection_Set_TestData()
    {
        yield return new object[] { ToolStripTextDirection.Inherit, ToolStripTextDirection.Horizontal };
        yield return new object[] { ToolStripTextDirection.Horizontal, ToolStripTextDirection.Horizontal };
        yield return new object[] { ToolStripTextDirection.Vertical90, ToolStripTextDirection.Vertical90 };
        yield return new object[] { ToolStripTextDirection.Vertical270, ToolStripTextDirection.Vertical270 };
    }

    [WinFormsTheory]
    [MemberData(nameof(TextDirection_Set_TestData))]
    public void ToolStripItem_TextDirection_Set_GetReturnsExpected(ToolStripTextDirection value, ToolStripTextDirection expected)
    {
        using SubToolStripItem item = new()
        {
            TextDirection = value
        };
        Assert.Equal(expected, item.TextDirection);

        // Set same.
        item.TextDirection = value;
        Assert.Equal(expected, item.TextDirection);
    }

    public static IEnumerable<object[]> TextDirection_SetWithOwner_TestData()
    {
        yield return new object[] { ToolStripTextDirection.Horizontal, ToolStripTextDirection.Inherit, ToolStripTextDirection.Horizontal };
        yield return new object[] { ToolStripTextDirection.Horizontal, ToolStripTextDirection.Horizontal, ToolStripTextDirection.Horizontal };
        yield return new object[] { ToolStripTextDirection.Horizontal, ToolStripTextDirection.Vertical90, ToolStripTextDirection.Vertical90 };
        yield return new object[] { ToolStripTextDirection.Horizontal, ToolStripTextDirection.Vertical270, ToolStripTextDirection.Vertical270 };
    }

    [WinFormsTheory]
    [MemberData(nameof(TextDirection_SetWithOwner_TestData))]
    public void ToolStripItem_TextDirection_SetWithOwner_GetReturnsExpected(ToolStripTextDirection ownerTextDirection, ToolStripTextDirection value, ToolStripTextDirection expected)
    {
        using ToolStrip owner = new()
        {
            TextDirection = ownerTextDirection
        };
        using SubToolStripItem item = new()
        {
            Owner = owner
        };
        int ownerLayoutCallCount = 0;
        void ownerHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(owner, sender);
            Assert.Same(item, e.AffectedComponent);
            Assert.Equal("TextDirection", e.AffectedProperty);
            ownerLayoutCallCount++;
        }

        owner.Layout += ownerHandler;

        try
        {
            item.TextDirection = value;
            Assert.Equal(expected, item.TextDirection);
            Assert.Equal(1, ownerLayoutCallCount);
            Assert.False(owner.IsHandleCreated);

            // Set same.
            item.TextDirection = value;
            Assert.Equal(expected, item.TextDirection);
            Assert.Equal(2, ownerLayoutCallCount);
            Assert.False(owner.IsHandleCreated);
        }
        finally
        {
            owner.Layout -= ownerHandler;
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(TextDirection_SetWithOwner_TestData))]
    public void ToolStripItem_TextDirection_SetWithOwnerWithHandle_GetReturnsExpected(ToolStripTextDirection ownerTextDirection, ToolStripTextDirection value, ToolStripTextDirection expected)
    {
        using ToolStrip owner = new()
        {
            TextDirection = ownerTextDirection
        };
        using SubToolStripItem item = new()
        {
            Owner = owner
        };
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;
        int ownerLayoutCallCount = 0;
        void ownerHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(owner, sender);
            Assert.Same(item, e.AffectedComponent);
            Assert.Equal("TextDirection", e.AffectedProperty);
            ownerLayoutCallCount++;
        }

        owner.Layout += ownerHandler;

        try
        {
            item.TextDirection = value;
            Assert.Equal(expected, item.TextDirection);
            Assert.Equal(1, ownerLayoutCallCount);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(2, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            item.TextDirection = value;
            Assert.Equal(expected, item.TextDirection);
            Assert.Equal(2, ownerLayoutCallCount);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(4, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }
        finally
        {
            owner.Layout -= ownerHandler;
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(TextDirection_SetWithOwner_TestData))]
    public void ToolStripItem_TextDirection_SetWithParent_GetReturnsExpected(ToolStripTextDirection parentTextDirection, ToolStripTextDirection value, ToolStripTextDirection expected)
    {
        using ToolStrip parent = new()
        {
            TextDirection = parentTextDirection
        };
        using SubToolStripItem item = new()
        {
            Parent = parent
        };
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e) => parentLayoutCallCount++;
        parent.Layout += parentHandler;

        try
        {
            item.TextDirection = value;
            Assert.Equal(expected, item.TextDirection);
            Assert.Equal(0, parentLayoutCallCount);
            Assert.False(parent.IsHandleCreated);

            // Set same.
            item.TextDirection = value;
            Assert.Equal(expected, item.TextDirection);
            Assert.Equal(0, parentLayoutCallCount);
            Assert.False(parent.IsHandleCreated);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(TextDirection_SetWithOwner_TestData))]
    public void ToolStripItem_TextDirection_SetWithParentWithHandle_GetReturnsExpected(ToolStripTextDirection parentTextDirection, ToolStripTextDirection value, ToolStripTextDirection expected)
    {
        using ToolStrip parent = new()
        {
            TextDirection = parentTextDirection
        };
        using SubToolStripItem item = new()
        {
            Parent = parent
        };
        Assert.NotEqual(IntPtr.Zero, parent.Handle);
        int invalidatedCallCount = 0;
        parent.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        parent.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        parent.HandleCreated += (sender, e) => createdCallCount++;
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e) => parentLayoutCallCount++;
        parent.Layout += parentHandler;

        try
        {
            item.TextDirection = value;
            Assert.Equal(expected, item.TextDirection);
            Assert.Equal(0, parentLayoutCallCount);
            Assert.True(parent.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            item.TextDirection = value;
            Assert.Equal(expected, item.TextDirection);
            Assert.Equal(0, parentLayoutCallCount);
            Assert.True(parent.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    [WinFormsTheory]
    [InvalidEnumData<ToolStripTextDirection>]
    public void ToolStripItem_TextDirection_SetInvalid_ThrowsInvalidEnumArgumentException(ToolStripTextDirection value)
    {
        using SubToolStripItem item = new();
        Assert.Throws<InvalidEnumArgumentException>("value", () => item.TextDirection = value);
    }

    [WinFormsFact]
    public void ToolStripItem_TextDirection_ResetValue_Success()
    {
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ToolStripItem))[nameof(ToolStripItem.TextDirection)];
        using SubToolStripItem item = new();
        Assert.False(property.CanResetValue(item));

        item.TextDirection = ToolStripTextDirection.Vertical270;
        Assert.Equal(ToolStripTextDirection.Vertical270, item.TextDirection);
        Assert.True(property.CanResetValue(item));

        item.TextDirection = ToolStripTextDirection.Vertical90;
        Assert.Equal(ToolStripTextDirection.Vertical90, item.TextDirection);
        Assert.True(property.CanResetValue(item));

        item.TextDirection = ToolStripTextDirection.Horizontal;
        Assert.Equal(ToolStripTextDirection.Horizontal, item.TextDirection);
        Assert.True(property.CanResetValue(item));

        property.ResetValue(item);
        Assert.Equal(ToolStripTextDirection.Horizontal, item.TextDirection);
        Assert.False(property.CanResetValue(item));
    }

    [WinFormsFact]
    public void ToolStripItem_TextDirection_ShouldSerializeValue_Success()
    {
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ToolStripItem))[nameof(ToolStripItem.TextDirection)];
        using SubToolStripItem item = new();
        Assert.False(property.ShouldSerializeValue(item));

        item.TextDirection = ToolStripTextDirection.Vertical270;
        Assert.Equal(ToolStripTextDirection.Vertical270, item.TextDirection);
        Assert.True(property.ShouldSerializeValue(item));

        item.TextDirection = ToolStripTextDirection.Vertical90;
        Assert.Equal(ToolStripTextDirection.Vertical90, item.TextDirection);
        Assert.True(property.ShouldSerializeValue(item));

        item.TextDirection = ToolStripTextDirection.Horizontal;
        Assert.Equal(ToolStripTextDirection.Horizontal, item.TextDirection);
        Assert.True(property.ShouldSerializeValue(item));

        property.ResetValue(item);
        Assert.Equal(ToolStripTextDirection.Horizontal, item.TextDirection);
        Assert.False(property.ShouldSerializeValue(item));
    }

    [WinFormsTheory]
    [EnumData<TextImageRelation>]
    public void ToolStripItem_TextImageRelation_Set_GetReturnsExpected(TextImageRelation value)
    {
        using SubToolStripItem item = new()
        {
            TextImageRelation = value
        };
        Assert.Equal(value, item.TextImageRelation);

        // Set same.
        item.TextImageRelation = value;
        Assert.Equal(value, item.TextImageRelation);
    }

    [WinFormsTheory]
    [InlineData(TextImageRelation.Overlay, 1)]
    [InlineData(TextImageRelation.ImageBeforeText, 0)]
    [InlineData(TextImageRelation.TextBeforeImage, 1)]
    [InlineData(TextImageRelation.ImageAboveText, 1)]
    [InlineData(TextImageRelation.TextAboveImage, 1)]
    public void ToolStripItem_TextImageRelation_SetWithOwner_GetReturnsExpected(TextImageRelation value, int expectedParentLayoutCallCount)
    {
        using ToolStrip owner = new();
        using SubToolStripItem item = new()
        {
            Owner = owner
        };
        int ownerLayoutCallCount = 0;
        void ownerHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(owner, sender);
            Assert.Same(item, e.AffectedComponent);
            Assert.Equal("TextImageRelation", e.AffectedProperty);
            ownerLayoutCallCount++;
        }

        owner.Layout += ownerHandler;

        try
        {
            item.TextImageRelation = value;
            Assert.Equal(value, item.TextImageRelation);
            Assert.Equal(expectedParentLayoutCallCount, ownerLayoutCallCount);
            Assert.False(owner.IsHandleCreated);

            // Set same.
            item.TextImageRelation = value;
            Assert.Equal(value, item.TextImageRelation);
            Assert.Equal(expectedParentLayoutCallCount, ownerLayoutCallCount);
            Assert.False(owner.IsHandleCreated);
        }
        finally
        {
            owner.Layout -= ownerHandler;
        }
    }

    [WinFormsTheory]
    [InlineData(TextImageRelation.Overlay, 1)]
    [InlineData(TextImageRelation.ImageBeforeText, 0)]
    [InlineData(TextImageRelation.TextBeforeImage, 1)]
    [InlineData(TextImageRelation.ImageAboveText, 1)]
    [InlineData(TextImageRelation.TextAboveImage, 1)]
    public void ToolStripItem_TextImageRelation_SetWithOwnerWithHandle_GetReturnsExpected(TextImageRelation value, int expectedParentLayoutCallCount)
    {
        using ToolStrip owner = new();
        using SubToolStripItem item = new()
        {
            Owner = owner
        };
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;
        int ownerLayoutCallCount = 0;
        void ownerHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(owner, sender);
            Assert.Same(item, e.AffectedComponent);
            Assert.Equal("TextImageRelation", e.AffectedProperty);
            ownerLayoutCallCount++;
        }

        owner.Layout += ownerHandler;

        try
        {
            item.TextImageRelation = value;
            Assert.Equal(value, item.TextImageRelation);
            Assert.Equal(expectedParentLayoutCallCount, ownerLayoutCallCount);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(expectedParentLayoutCallCount * 2, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            item.TextImageRelation = value;
            Assert.Equal(value, item.TextImageRelation);
            Assert.Equal(expectedParentLayoutCallCount, ownerLayoutCallCount);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(expectedParentLayoutCallCount * 2, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }
        finally
        {
            owner.Layout -= ownerHandler;
        }
    }

    [WinFormsTheory]
    [EnumData<TextImageRelation>]
    public void ToolStripItem_TextImageRelation_SetWithParent_GetReturnsExpected(TextImageRelation value)
    {
        using ToolStrip parent = new();
        using SubToolStripItem item = new()
        {
            Parent = parent
        };
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e) => parentLayoutCallCount++;
        parent.Layout += parentHandler;

        try
        {
            item.TextImageRelation = value;
            Assert.Equal(value, item.TextImageRelation);
            Assert.Equal(0, parentLayoutCallCount);
            Assert.False(parent.IsHandleCreated);

            // Set same.
            item.TextImageRelation = value;
            Assert.Equal(value, item.TextImageRelation);
            Assert.Equal(0, parentLayoutCallCount);
            Assert.False(parent.IsHandleCreated);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    [WinFormsTheory]
    [EnumData<TextImageRelation>]
    public void ToolStripItem_TextImageRelation_SetWithParentWithHandle_GetReturnsExpected(TextImageRelation value)
    {
        using ToolStrip parent = new();
        using SubToolStripItem item = new()
        {
            Parent = parent
        };
        Assert.NotEqual(IntPtr.Zero, parent.Handle);
        int invalidatedCallCount = 0;
        parent.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        parent.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        parent.HandleCreated += (sender, e) => createdCallCount++;
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e) => parentLayoutCallCount++;
        parent.Layout += parentHandler;

        try
        {
            item.TextImageRelation = value;
            Assert.Equal(value, item.TextImageRelation);
            Assert.Equal(0, parentLayoutCallCount);
            Assert.True(parent.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            item.TextImageRelation = value;
            Assert.Equal(value, item.TextImageRelation);
            Assert.Equal(0, parentLayoutCallCount);
            Assert.True(parent.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    [WinFormsTheory]
    [InvalidEnumData<TextImageRelation>]
    [InlineData((TextImageRelation)3)]
    [InlineData((TextImageRelation)5)]
    [InlineData((TextImageRelation)6)]
    [InlineData((TextImageRelation)7)]
    public void ToolStripItem_TextImageRelation_SetInvalid_ThrowsInvalidEnumArgumentException(TextImageRelation value)
    {
        using SubToolStripItem item = new();
        Assert.Throws<InvalidEnumArgumentException>("value", () => item.TextImageRelation = value);
    }

    public static IEnumerable<object[]> ToolTipText_Get_TestData()
    {
        yield return new object[] { true, null, null };
        yield return new object[] { true, "", "" };
        yield return new object[] { true, "text", "text" };
        yield return new object[] { true, "&", "&" };
        yield return new object[] { true, "&&", "&&" };
        yield return new object[] { true, "&T", "T" };
        yield return new object[] { true, "&Text", "Text" };
        yield return new object[] { true, "&Text1&Text2", "&Text1&Text2" };
        yield return new object[] { true, "Text1&Text2", "Text1Text2" };

        yield return new object[] { false, null, null };
        yield return new object[] { false, "", null };
        yield return new object[] { false, "text", null };
        yield return new object[] { false, "&", null };
        yield return new object[] { false, "&&", null };
        yield return new object[] { false, "&T", null };
        yield return new object[] { false, "&Text", null };
        yield return new object[] { false, "&Text1&Text2", null };
        yield return new object[] { false, "Text1&Text2", null };
    }

    [WinFormsTheory]
    [MemberData(nameof(ToolTipText_Get_TestData))]
    public void ToolStripItem_ToolTipText_Get_ReturnsExpected(bool autoToolTip, string text, string expected)
    {
        using SubToolStripItem item = new()
        {
            AutoToolTip = autoToolTip,
            Text = text
        };
        Assert.Equal(expected, item.ToolTipText);
    }

    [WinFormsTheory]
    [StringData]
    public void ToolStripItem_ToolTipText_Set_GetReturnsExpected(string value)
    {
        using SubToolStripItem item = new()
        {
            ToolTipText = value
        };
        Assert.Equal(value, item.ToolTipText);

        // Set same.
        item.ToolTipText = value;
        Assert.Equal(value, item.ToolTipText);
    }

    [WinFormsFact]
    public void ToolStripItem_ToolTipText_ResetValue_Success()
    {
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ToolStripItem))[nameof(ToolStripItem.ToolTipText)];
        using SubToolStripItem item = new();
        Assert.False(property.CanResetValue(item));

        // Set null.
        item.ToolTipText = null;
        Assert.Null(item.ToolTipText);
        Assert.False(property.CanResetValue(item));

        // Set empty.
        item.ToolTipText = string.Empty;
        Assert.Empty(item.ToolTipText);
        Assert.False(property.CanResetValue(item));

        // Set custom
        item.ToolTipText = "text";
        Assert.Equal("text", item.ToolTipText);
        Assert.True(property.CanResetValue(item));

        property.ResetValue(item);
        Assert.Null(item.ToolTipText);
        Assert.False(property.CanResetValue(item));
    }

    [WinFormsFact]
    public void ToolStripItem_ToolTipText_ShouldSerializeValue_Success()
    {
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ToolStripItem))[nameof(ToolStripItem.ToolTipText)];
        using SubToolStripItem item = new();
        Assert.False(property.ShouldSerializeValue(item));

        // Set null.
        item.ToolTipText = null;
        Assert.Null(item.ToolTipText);
        Assert.False(property.ShouldSerializeValue(item));

        // Set empty.
        item.ToolTipText = string.Empty;
        Assert.Empty(item.ToolTipText);
        Assert.False(property.ShouldSerializeValue(item));

        // Set custom
        item.ToolTipText = "text";
        Assert.Equal("text", item.ToolTipText);
        Assert.True(property.ShouldSerializeValue(item));

        property.ResetValue(item);
        Assert.Null(item.ToolTipText);
        Assert.False(property.ShouldSerializeValue(item));
    }

    [WinFormsTheory]
    [InlineData(true, true)]
    [InlineData(true, false)]
    [InlineData(false, true)]
    [InlineData(false, false)]
    public void ToolStripItem_Visible_GetWithOwner_ReturnsExpected(bool ownerVisible, bool visible)
    {
        using ToolStrip owner = new()
        {
            Visible = ownerVisible
        };
        using SubToolStripItem item = new()
        {
            Owner = owner
        };
        Assert.False(item.Visible);

        // Set custom.
        item.Visible = visible;
        Assert.False(item.Visible);
    }

    [WinFormsTheory]
    [InlineData(true, true, true)]
    [InlineData(true, false, false)]
    [InlineData(false, true, false)]
    [InlineData(false, false, false)]
    public void ToolStripItem_Visible_GetWithParent_ReturnsExpected(bool parentVisible, bool visible, bool expected)
    {
        using ToolStrip parent = new()
        {
            Visible = parentVisible
        };
        using SubToolStripItem item = new()
        {
            Parent = parent
        };
        Assert.Equal(parentVisible, item.Visible);

        // Set custom.
        item.Visible = visible;
        Assert.Equal(expected, item.Visible);
    }

    [WinFormsTheory]
    [MemberData(nameof(SetVisibleCore_TestData))]
    public void ToolStripItem_Visible_Set_GetReturnsExpected(bool enabled, Image image, bool value)
    {
        using SubToolStripItem item = new()
        {
            Enabled = enabled,
            Image = image,
            Visible = value
        };
        Assert.False(item.Visible);
        Assert.Equal(value, item.Available);
        Assert.False(item.Selected);

        // Set same.
        item.Visible = value;
        Assert.False(item.Visible);
        Assert.Equal(value, item.Available);
        Assert.False(item.Selected);

        // Set different.
        item.Available = !value;
        Assert.False(item.Visible);
        Assert.Equal(!value, item.Available);
        Assert.False(item.Selected);
    }

    [WinFormsTheory]
    [MemberData(nameof(SetVisibleCore_TestData))]
    public void ToolStripItem_Visible_SetDesignMode_GetReturnsExpected(bool enabled, Image image, bool value)
    {
        Mock<ISite> mockSite = new(MockBehavior.Strict);
        mockSite
            .Setup(s => s.Name)
            .Returns("Name");
        mockSite
            .Setup(s => s.DesignMode)
            .Returns(true);
        mockSite
            .Setup(s => s.Container)
            .Returns((IContainer)null);
        using SubToolStripItem item = new()
        {
            Enabled = enabled,
            Image = image,
            Site = mockSite.Object,
            Visible = value
        };
        Assert.False(item.Visible);
        Assert.Equal(value, item.Available);
        Assert.False(item.Selected);

        // Set same.
        item.Visible = value;
        Assert.False(item.Visible);
        Assert.Equal(value, item.Available);
        Assert.False(item.Selected);

        // Set different.
        item.Available = !value;
        Assert.False(item.Visible);
        Assert.Equal(!value, item.Available);
        Assert.False(item.Selected);
    }

    [WinFormsTheory]
    [BoolData]
    public void ToolStripItem_Visible_SetSelected_GetReturnsExpected(bool value)
    {
        using SubToolStripItem item = new();
        item.Select();
        Assert.True(item.Selected);

        item.Visible = value;
        Assert.False(item.Visible);
        Assert.Equal(value, item.Available);
        Assert.Equal(value, item.Selected);

        // Set same.
        item.Visible = value;
        Assert.False(item.Visible);
        Assert.Equal(value, item.Available);
        Assert.Equal(value, item.Selected);

        // Set different.
        item.Available = !value;
        Assert.False(item.Visible);
        Assert.Equal(!value, item.Available);
        Assert.False(item.Selected);
    }

    [WinFormsTheory]
    [MemberData(nameof(SetVisibleCore_TestData))]
    public void ToolStripItem_Visible_SetWithOwner_GetReturnsExpected(bool enabled, Image image, bool value)
    {
        using ToolStrip owner = new();
        using SubToolStripItem item = new()
        {
            Enabled = enabled,
            Image = image,
            Owner = owner
        };

        item.Visible = value;
        Assert.False(item.Visible);
        Assert.Equal(value, item.Available);
        Assert.False(item.Selected);
        Assert.False(owner.IsHandleCreated);

        // Set same.
        item.Visible = value;
        Assert.False(item.Visible);
        Assert.Equal(value, item.Available);
        Assert.False(item.Selected);
        Assert.False(owner.IsHandleCreated);

        // Set different.
        item.Available = !value;
        Assert.False(item.Visible);
        Assert.Equal(!value, item.Available);
        Assert.False(item.Selected);
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(SetVisibleCore_TestData))]
    public void ToolStripItem_Visible_SetDesignModeWithOwner_GetReturnsExpected(bool enabled, Image image, bool value)
    {
        Mock<ISite> mockSite = new(MockBehavior.Strict);
        mockSite
            .Setup(s => s.Name)
            .Returns("Name");
        mockSite
            .Setup(s => s.DesignMode)
            .Returns(true);
        mockSite
            .Setup(s => s.Container)
            .Returns((IContainer)null);
        using ToolStrip owner = new();
        using SubToolStripItem item = new()
        {
            Enabled = enabled,
            Image = image,
            Site = mockSite.Object,
            Owner = owner,
        };

        item.Visible = value;
        Assert.False(item.Visible);
        Assert.Equal(value, item.Available);
        Assert.False(item.Selected);
        Assert.False(owner.IsHandleCreated);

        // Set same.
        item.Visible = value;
        Assert.False(item.Visible);
        Assert.Equal(value, item.Available);
        Assert.False(item.Selected);
        Assert.False(owner.IsHandleCreated);

        // Set different.
        item.Available = !value;
        Assert.False(item.Visible);
        Assert.Equal(!value, item.Available);
        Assert.False(item.Selected);
        Assert.False(owner.IsHandleCreated);
    }

    public static IEnumerable<object[]> Visible_SetWithOwnerWithHandle_TestData()
    {
        foreach (bool enabled in new bool[] { true, false })
        {
            foreach (Image image in new Image[] { null, new Bitmap(10, 10) })
            {
                yield return new object[] { enabled, image, true, 0 };
                yield return new object[] { enabled, image, false, 2 };
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(Visible_SetWithOwnerWithHandle_TestData))]
    public void ToolStripItem_Visible_SetWithOwnerWithHandle_GetReturnsExpected(bool enabled, Image image, bool value, int expectedInvalidatedCallCount)
    {
        using ToolStrip owner = new();
        using SubToolStripItem item = new()
        {
            Enabled = enabled,
            Image = image,
            Owner = owner
        };
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;

        item.Visible = value;
        Assert.False(item.Visible);
        Assert.Equal(value, item.Available);
        Assert.False(item.Selected);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        item.Visible = value;
        Assert.False(item.Visible);
        Assert.Equal(value, item.Available);
        Assert.False(item.Selected);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set different.
        item.Available = !value;
        Assert.Equal(!value, item.Visible);
        Assert.Equal(!value, item.Available);
        Assert.False(item.Selected);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount + 2, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [MemberData(nameof(SetVisibleCore_TestData))]
    public void ToolStripItem_Visible_SetWithParent_GetReturnsExpected(bool enabled, Image image, bool value)
    {
        using ToolStrip parent = new();
        using SubToolStripItem item = new()
        {
            Enabled = enabled,
            Image = image,
            Parent = parent
        };

        item.Visible = value;
        Assert.Equal(value, item.Visible);
        Assert.Equal(value, item.Available);
        Assert.False(item.Selected);
        Assert.False(parent.IsHandleCreated);

        // Set same.
        item.Visible = value;
        Assert.Equal(value, item.Visible);
        Assert.Equal(value, item.Available);
        Assert.False(item.Selected);
        Assert.False(parent.IsHandleCreated);

        // Set different.
        item.Available = !value;
        Assert.Equal(!value, item.Visible);
        Assert.Equal(!value, item.Available);
        Assert.False(item.Selected);
        Assert.False(parent.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(SetVisibleCore_TestData))]
    public void ToolStripItem_Visible_SetDesignModeWithParent_GetReturnsExpected(bool enabled, Image image, bool value)
    {
        Mock<ISite> mockSite = new(MockBehavior.Strict);
        mockSite
            .Setup(s => s.Name)
            .Returns("Name");
        mockSite
            .Setup(s => s.DesignMode)
            .Returns(true);
        mockSite
            .Setup(s => s.Container)
            .Returns((IContainer)null);
        using ToolStrip parent = new();
        using SubToolStripItem item = new()
        {
            Enabled = enabled,
            Image = image,
            Site = mockSite.Object,
            Parent = parent
        };

        item.Visible = value;
        Assert.Equal(value, item.Visible);
        Assert.Equal(value, item.Available);
        Assert.False(item.Selected);
        Assert.False(parent.IsHandleCreated);

        // Set same.
        item.Visible = value;
        Assert.Equal(value, item.Visible);
        Assert.Equal(value, item.Available);
        Assert.False(item.Selected);
        Assert.False(parent.IsHandleCreated);

        // Set different.
        item.Available = !value;
        Assert.Equal(!value, item.Visible);
        Assert.Equal(!value, item.Available);
        Assert.False(item.Selected);
        Assert.False(parent.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(SetVisibleCore_TestData))]
    public void ToolStripItem_Visible_SetWithParentWithHandle_GetReturnsExpected(bool enabled, Image image, bool value)
    {
        using ToolStrip parent = new();
        using SubToolStripItem item = new()
        {
            Enabled = enabled,
            Image = image,
            Parent = parent
        };
        Assert.NotEqual(IntPtr.Zero, parent.Handle);
        int invalidatedCallCount = 0;
        parent.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        parent.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        parent.HandleCreated += (sender, e) => createdCallCount++;

        item.Visible = value;
        Assert.Equal(value, item.Visible);
        Assert.Equal(value, item.Available);
        Assert.False(item.Selected);
        Assert.True(parent.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        item.Visible = value;
        Assert.Equal(value, item.Visible);
        Assert.Equal(value, item.Available);
        Assert.False(item.Selected);
        Assert.True(parent.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set different.
        item.Available = !value;
        Assert.Equal(!value, item.Visible);
        Assert.Equal(!value, item.Available);
        Assert.False(item.Selected);
        Assert.True(parent.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void ToolStripItem_Visible_SetWithHandler_CallsAvailableChanged()
    {
        using SubToolStripItem item = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(item, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        item.AvailableChanged += handler;

        // Set different.
        item.Available = false;
        Assert.False(item.Available);
        Assert.Equal(1, callCount);

        // Set same.
        item.Available = false;
        Assert.False(item.Available);
        Assert.Equal(1, callCount);

        // Set different.
        item.Available = true;
        Assert.True(item.Available);
        Assert.Equal(2, callCount);

        // Remove handler.
        item.AvailableChanged -= handler;
        item.Available = false;
        Assert.False(item.Available);
        Assert.Equal(2, callCount);
    }

    [WinFormsFact]
    public void ToolStripItem_Visible_SetWithHandler_CallsVisibleChanged()
    {
        using SubToolStripItem item = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(item, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        item.VisibleChanged += handler;

        // Set different.
        item.Available = false;
        Assert.False(item.Available);
        Assert.Equal(1, callCount);

        // Set same.
        item.Available = false;
        Assert.False(item.Available);
        Assert.Equal(1, callCount);

        // Set different.
        item.Available = true;
        Assert.True(item.Available);
        Assert.Equal(2, callCount);

        // Remove handler.
        item.VisibleChanged -= handler;
        item.Available = false;
        Assert.False(item.Available);
        Assert.Equal(2, callCount);
    }

    [WinFormsFact]
    public void ToolStripItem_Visible_ResetValue_Success()
    {
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ToolStripItem))[nameof(ToolStripItem.Visible)];
        using SubToolStripItem item = new();
        Assert.False(property.CanResetValue(item));

        item.Visible = false;
        Assert.False(item.Visible);
        Assert.False(property.CanResetValue(item));

        property.ResetValue(item);
        Assert.False(item.Visible);
        Assert.False(property.CanResetValue(item));
    }

    [WinFormsFact]
    public void ToolStripItem_Visible_ShouldSerializeValue_Success()
    {
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ToolStripItem))[nameof(ToolStripItem.Visible)];
        using SubToolStripItem item = new();
        Assert.False(property.ShouldSerializeValue(item));

        item.Visible = false;
        Assert.False(item.Visible);
        Assert.True(property.ShouldSerializeValue(item));

        property.ResetValue(item);
        Assert.False(item.Visible);
        Assert.True(property.ShouldSerializeValue(item));
    }

    [WinFormsTheory]
    [IntegerData<int>]
    public void ToolStripItem_Width_Set_GetReturnsExpected(int value)
    {
        using SubToolStripItem item = new();
        int locationChangedCallCount = 0;
        item.LocationChanged += (sender, e) => locationChangedCallCount++;

        item.Width = value;
        Assert.Equal(value, item.Width);
        Assert.Equal(0, locationChangedCallCount);

        // Set same.
        item.Width = value;
        Assert.Equal(value, item.Width);
        Assert.Equal(0, locationChangedCallCount);
    }

    [WinFormsTheory]
    [IntegerData<int>]
    public void ToolStripItem_Width_SetWithOwner_GetReturnsExpected(int value)
    {
        using ToolStrip owner = new();
        using SubToolStripItem item = new()
        {
            Owner = owner
        };
        int ownerLayoutCallCount = 0;
        void ownerHandler(object sender, LayoutEventArgs e) => ownerLayoutCallCount++;
        owner.Layout += ownerHandler;
        int locationChangedCallCount = 0;
        item.LocationChanged += (sender, e) => locationChangedCallCount++;

        try
        {
            item.Width = value;
            Assert.Equal(value, item.Width);
            Assert.Equal(0, locationChangedCallCount);
            Assert.Equal(0, ownerLayoutCallCount);
            Assert.False(owner.IsHandleCreated);

            // Set same.
            item.Width = value;
            Assert.Equal(value, item.Width);
            Assert.Equal(0, locationChangedCallCount);
            Assert.Equal(0, ownerLayoutCallCount);
            Assert.False(owner.IsHandleCreated);
        }
        finally
        {
            owner.Layout -= ownerHandler;
        }
    }

    [WinFormsTheory]
    [IntegerData<int>]
    public void ToolStripItem_Width_SetWithOwnerWithHandle_GetReturnsExpected(int value)
    {
        using ToolStrip owner = new();
        using SubToolStripItem item = new()
        {
            Owner = owner
        };
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;
        int ownerLayoutCallCount = 0;
        void ownerHandler(object sender, LayoutEventArgs e) => ownerLayoutCallCount++;
        owner.Layout += ownerHandler;
        int locationChangedCallCount = 0;
        item.LocationChanged += (sender, e) => locationChangedCallCount++;

        try
        {
            item.Width = value;
            Assert.Equal(value, item.Width);
            Assert.Equal(0, locationChangedCallCount);
            Assert.Equal(0, ownerLayoutCallCount);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            item.Width = value;
            Assert.Equal(value, item.Width);
            Assert.Equal(0, locationChangedCallCount);
            Assert.Equal(0, ownerLayoutCallCount);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }
        finally
        {
            owner.Layout -= ownerHandler;
        }
    }

    [WinFormsTheory]
    [InlineData(-1, 1)]
    [InlineData(0, 1)]
    [InlineData(1, 1)]
    [InlineData(23, 0)]
    public void ToolStripItem_Width_SetWithParent_GetReturnsExpected(int value, int expectedParentLayoutCallCount)
    {
        using ToolStrip parent = new();
        using SubToolStripItem item = new()
        {
            Parent = parent
        };
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(parent, sender);
            Assert.Same(item, e.AffectedComponent);
            Assert.Equal("Bounds", e.AffectedProperty);
            parentLayoutCallCount++;
        }

        parent.Layout += parentHandler;
        int locationChangedCallCount = 0;
        item.LocationChanged += (sender, e) => locationChangedCallCount++;

        try
        {
            item.Width = value;
            Assert.Equal(value, item.Width);
            Assert.Equal(0, locationChangedCallCount);
            Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
            Assert.False(parent.IsHandleCreated);

            // Set same.
            item.Width = value;
            Assert.Equal(value, item.Width);
            Assert.Equal(0, locationChangedCallCount);
            Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
            Assert.False(parent.IsHandleCreated);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    [WinFormsTheory]
    [InlineData(-1, 1)]
    [InlineData(0, 1)]
    [InlineData(1, 1)]
    [InlineData(23, 0)]
    public void ToolStripItem_Width_SetWithParentWithHandle_GetReturnsExpected(int value, int expectedParentLayoutCallCount)
    {
        using ToolStrip parent = new();
        using SubToolStripItem item = new()
        {
            Parent = parent
        };
        Assert.NotEqual(IntPtr.Zero, parent.Handle);
        int invalidatedCallCount = 0;
        parent.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        parent.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        parent.HandleCreated += (sender, e) => createdCallCount++;
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(parent, sender);
            Assert.Same(item, e.AffectedComponent);
            Assert.Equal("Bounds", e.AffectedProperty);
            parentLayoutCallCount++;
        }

        parent.Layout += parentHandler;
        int locationChangedCallCount = 0;
        item.LocationChanged += (sender, e) => locationChangedCallCount++;

        try
        {
            item.Width = value;
            Assert.Equal(value, item.Width);
            Assert.Equal(0, locationChangedCallCount);
            Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
            Assert.True(parent.IsHandleCreated);
            Assert.Equal(expectedParentLayoutCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            item.Width = value;
            Assert.Equal(value, item.Width);
            Assert.Equal(0, locationChangedCallCount);
            Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
            Assert.True(parent.IsHandleCreated);
            Assert.Equal(expectedParentLayoutCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    [WinFormsFact]
    public void ToolStripItem_CreateAccessibilityInstance_Invoke_ReturnsExpected()
    {
        using SubToolStripItem item = new();
        ToolStripItem.ToolStripItemAccessibleObject accessibleObject = Assert.IsAssignableFrom<ToolStripItem.ToolStripItemAccessibleObject>(item.CreateAccessibilityInstance());
        Assert.Equal(AccessibleRole.PushButton, accessibleObject.Role);
        Assert.Equal(AccessibleStates.Focusable, accessibleObject.State);
        Assert.NotSame(accessibleObject, item.CreateAccessibilityInstance());
        Assert.NotSame(accessibleObject, item.AccessibilityObject);
    }

    [WinFormsFact]
    public void ToolStripItem_Dispose_Invoke_Success()
    {
        using SubToolStripItem item = new();
        int callCount = 0;
        void handler(object sender, EventArgs e) => callCount++;
        item.Disposed += handler;

        try
        {
            item.Dispose();
            Assert.False(item.IsDisposed);
            Assert.Null(item.Image);
            Assert.Equal(1, callCount);

            // Dispose multiple times.
            item.Dispose();
            Assert.False(item.IsDisposed);
            Assert.Null(item.Image);
            Assert.Equal(2, callCount);
        }
        finally
        {
            item.Disposed -= handler;
        }
    }

    [WinFormsFact]
    public void ToolStripItem_Dispose_InvokeWithImage_Success()
    {
        using Bitmap image = new(10, 10);
        using SubToolStripItem item = new()
        {
            Image = image
        };
        int callCount = 0;
        void handler(object sender, EventArgs e)
        {
            Assert.Equal(callCount > 0, item.Image is null);
            callCount++;
        }

        item.Disposed += handler;

        try
        {
            item.Dispose();
            Assert.False(item.IsDisposed);
            Assert.Null(item.Image);
            Assert.Equal(1, callCount);

            // Dispose multiple times.
            item.Dispose();
            Assert.False(item.IsDisposed);
            Assert.Null(item.Image);
            Assert.Equal(2, callCount);
        }
        finally
        {
            item.Disposed -= handler;
        }
    }

    [WinFormsFact]
    public void ToolStripItem_Dispose_InvokeWithOwner_Success()
    {
        using Bitmap image = new(10, 10);
        using ToolStrip owner = new();
        using SubToolStripItem item = new()
        {
            Image = image,
            Owner = owner
        };
        int callCount = 0;
        void handler(object sender, EventArgs e)
        {
            Assert.Equal(callCount > 0, item.Image is null);
            callCount++;
        }

        item.Disposed += handler;

        try
        {
            item.Dispose();
            Assert.True(item.IsDisposed);
            Assert.Null(item.Image);
            Assert.Null(item.Owner);
            Assert.Empty(owner.Items);
            Assert.Equal(1, callCount);

            // Dispose multiple times.
            item.Dispose();
            Assert.True(item.IsDisposed);
            Assert.Null(item.Image);
            Assert.Null(item.Owner);
            Assert.Empty(owner.Items);
            Assert.Equal(2, callCount);
        }
        finally
        {
            item.Disposed -= handler;
        }
    }

    [WinFormsFact]
    public void ToolStripItem_Dispose_InvokeWithParent_Success()
    {
        using Bitmap image = new(10, 10);
        using ToolStrip parent = new();
        using SubToolStripItem item = new()
        {
            Image = image,
            Parent = parent
        };
        int callCount = 0;
        void handler(object sender, EventArgs e)
        {
            Assert.Equal(callCount > 0, item.Image is null);
            callCount++;
        }

        item.Disposed += handler;

        try
        {
            item.Dispose();
            Assert.False(item.IsDisposed);
            Assert.Null(item.Image);
            Assert.Same(parent, item.Parent);
            Assert.Empty(parent.Items);
            Assert.Equal(1, callCount);

            // Dispose multiple times.
            item.Dispose();
            Assert.False(item.IsDisposed);
            Assert.Null(item.Image);
            Assert.Same(parent, item.Parent);
            Assert.Empty(parent.Items);
            Assert.Equal(2, callCount);
        }
        finally
        {
            item.Disposed -= handler;
        }
    }

    [WinFormsTheory]
    [InlineData(true, 1)]
    [InlineData(false, 0)]
    public void ToolStripItem_Dispose_InvokeBool_Success(bool disposing, int expectedCallCount)
    {
        using SubToolStripItem item = new();
        int callCount = 0;
        void handler(object sender, EventArgs e) => callCount++;
        item.Disposed += handler;

        try
        {
            item.Dispose(disposing);
            Assert.False(item.IsDisposed);
            Assert.Null(item.Image);
            Assert.Equal(expectedCallCount, callCount);

            // Dispose multiple times.
            item.Dispose(disposing);
            Assert.False(item.IsDisposed);
            Assert.Null(item.Image);
            Assert.Equal(expectedCallCount * 2, callCount);
        }
        finally
        {
            item.Disposed -= handler;
        }
    }

    [WinFormsTheory]
    [InlineData(true, 1)]
    [InlineData(false, 0)]
    public void ToolStripItem_Dispose_InvokeBoolWithImage_Success(bool disposing, int expectedCallCount)
    {
        using Bitmap image = new(10, 10);
        using SubToolStripItem item = new()
        {
            Image = image
        };
        int callCount = 0;
        void handler(object sender, EventArgs e)
        {
            Assert.Equal(callCount > 0, item.Image is null);
            callCount++;
        }

        item.Disposed += handler;

        try
        {
            item.Dispose(disposing);
            Assert.False(item.IsDisposed);
            Assert.Equal(disposing, item.Image is null);
            Assert.Equal(expectedCallCount, callCount);

            // Dispose multiple times.
            item.Dispose(disposing);
            Assert.False(item.IsDisposed);
            Assert.Equal(disposing, item.Image is null);
            Assert.Equal(expectedCallCount * 2, callCount);
        }
        finally
        {
            item.Disposed -= handler;
        }
    }

    [WinFormsTheory]
    [InlineData(true, 1)]
    [InlineData(false, 0)]
    public void ToolStripItem_Dispose_InvokeBoolWithOwner_Success(bool disposing, int expectedCallCount)
    {
        using Bitmap image = new(10, 10);
        using ToolStrip owner = new();
        using SubToolStripItem item = new()
        {
            Image = image,
            Owner = owner
        };
        int callCount = 0;
        void handler(object sender, EventArgs e)
        {
            Assert.Equal(callCount > 0, item.Image is null);
            callCount++;
        }

        item.Disposed += handler;

        try
        {
            item.Dispose(disposing);
            Assert.Equal(disposing, item.IsDisposed);
            Assert.Equal(disposing, item.Image is null);
            Assert.Equal(disposing, item.Owner is null);
            Assert.Equal(!disposing, owner.Items.Contains(item));
            Assert.Equal(expectedCallCount, callCount);

            // Dispose multiple times.
            item.Dispose(disposing);
            Assert.Equal(disposing, item.IsDisposed);
            Assert.Equal(disposing, item.Image is null);
            Assert.Equal(disposing, item.Owner is null);
            Assert.Equal(!disposing, owner.Items.Contains(item));
            Assert.Equal(expectedCallCount * 2, callCount);
        }
        finally
        {
            item.Disposed -= handler;
        }
    }

    [WinFormsTheory]
    [InlineData(true, 1)]
    [InlineData(false, 0)]
    public void ToolStripItem_Dispose_InvokeBoolWithParent_Success(bool disposing, int expectedCallCount)
    {
        using Bitmap image = new(10, 10);
        using ToolStrip parent = new();
        using SubToolStripItem item = new()
        {
            Image = image,
            Parent = parent
        };
        int callCount = 0;
        void handler(object sender, EventArgs e)
        {
            Assert.Equal(callCount > 0, item.Image is null);
            callCount++;
        }

        item.Disposed += handler;

        try
        {
            item.Dispose(disposing);
            Assert.False(item.IsDisposed);
            Assert.Equal(disposing, item.Image is null);
            Assert.Same(parent, item.Parent);
            Assert.Empty(parent.Items);
            Assert.Equal(expectedCallCount, callCount);

            // Dispose multiple times.
            item.Dispose(disposing);
            Assert.False(item.IsDisposed);
            Assert.Equal(disposing, item.Image is null);
            Assert.Same(parent, item.Parent);
            Assert.Empty(parent.Items);
            Assert.Equal(expectedCallCount * 2, callCount);
        }
        finally
        {
            item.Disposed -= handler;
        }
    }

    public static IEnumerable<object[]> DoDragDrop_TestData()
    {
        foreach (DragDropEffects allowedEffects in Enum.GetValues(typeof(DragDropEffects)))
        {
            yield return new object[] { "text", allowedEffects };
            yield return new object[] { new SubToolStripItem(), allowedEffects };
            yield return new object[] { new DataObject(), allowedEffects };
            yield return new object[] { new DataObject("data"), allowedEffects };
            yield return new object[] { new Mock<IDataObject>(MockBehavior.Strict).Object, allowedEffects };
            yield return new object[] { new Mock<IComDataObject>(MockBehavior.Strict).Object, allowedEffects };
        }
    }

    [WinFormsTheory(Skip = "hangs CI, see https://github.com/dotnet/winforms/issues/3336")]
    [ActiveIssue("https://github.com/dotnet/winforms/issues/3336")]
    [MemberData(nameof(DoDragDrop_TestData))]
    public void ToolStripItem_DoDragDrop_Invoke_ReturnsNone(object data, DragDropEffects allowedEffects)
    {
        using SubToolStripItem item = new();
        Assert.Equal(DragDropEffects.None, item.DoDragDrop(data, allowedEffects));
    }

    [WinFormsTheory(Skip = "hangs CI, see https://github.com/dotnet/winforms/issues/3336")]
    [ActiveIssue("https://github.com/dotnet/winforms/issues/3336")]
    [MemberData(nameof(DoDragDrop_TestData))]
    public void ToolStripItem_DoDragDrop_InvokeWithParent_ReturnsNone(object data, DragDropEffects allowedEffects)
    {
        using ToolStrip parent = new();
        using SubToolStripItem item = new()
        {
            Parent = parent
        };
        Assert.Equal(DragDropEffects.None, item.DoDragDrop(data, allowedEffects));
        Assert.False(parent.IsHandleCreated);
    }

    [WinFormsTheory(Skip = "hangs CI, see https://github.com/dotnet/winforms/issues/3336")]
    [ActiveIssue("https://github.com/dotnet/winforms/issues/3336")]
    [MemberData(nameof(DoDragDrop_TestData))]
    public void ToolStripItem_DoDragDrop_InvokeWithParentAllowItemReorder_ReturnsNone(object data, DragDropEffects allowedEffects)
    {
        using ToolStrip parent = new()
        {
            AllowItemReorder = true
        };
        using SubToolStripItem item = new()
        {
            Parent = parent
        };
        Assert.Equal(DragDropEffects.None, item.DoDragDrop(data, allowedEffects));
        Assert.False(parent.IsHandleCreated);
    }

    [WinFormsTheory(Skip = "hangs CI, see https://github.com/dotnet/winforms/issues/3336")]
    [ActiveIssue("https://github.com/dotnet/winforms/issues/3336")]
    [MemberData(nameof(DoDragDrop_TestData))]
    public void ToolStripItem_DoDragDrop_InvokeWithOwner_ReturnsNone(object data, DragDropEffects allowedEffects)
    {
        using ToolStrip owner = new();
        using SubToolStripItem item = new()
        {
            Owner = owner
        };
        Assert.Equal(DragDropEffects.None, item.DoDragDrop(data, allowedEffects));
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsTheory(Skip = "hangs CI, see https://github.com/dotnet/winforms/issues/3336")]
    [ActiveIssue("https://github.com/dotnet/winforms/issues/3336")]
    [MemberData(nameof(DoDragDrop_TestData))]
    public void ToolStripItem_DoDragDrop_InvokeWithOwnerAllowItemReorder_ReturnsNone(object data, DragDropEffects allowedEffects)
    {
        using ToolStrip owner = new()
        {
            AllowItemReorder = true
        };
        using SubToolStripItem item = new()
        {
            Owner = owner
        };
        Assert.Equal(DragDropEffects.None, item.DoDragDrop(data, allowedEffects));
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsFact(Skip = "hangs CI, see https://github.com/dotnet/winforms/issues/3336")]
    [ActiveIssue("https://github.com/dotnet/winforms/issues/3336")]
    public void ToolStripItem_DoDragDrop_NullData_ThrowsArgumentNullException()
    {
        using SubToolStripItem item = new();
        Assert.Throws<ArgumentNullException>("data", () => item.DoDragDrop(null, DragDropEffects.All));
    }

    private class CustomDataObject : IDataObject
    {
        public object GetData(string format, bool autoConvert) => throw new NotImplementedException();
        public object GetData(string format) => throw new NotImplementedException();
        public object GetData(Type format) => throw new NotImplementedException();
        public bool GetDataPresent(string format, bool autoConvert) => throw new NotImplementedException();
        public bool GetDataPresent(string format) => throw new NotImplementedException();
        public bool GetDataPresent(Type format) => throw new NotImplementedException();
        public string[] GetFormats(bool autoConvert) => throw new NotImplementedException();
        public string[] GetFormats() => throw new NotImplementedException();
        public void SetData(string format, bool autoConvert, object data) => throw new NotImplementedException();
        public void SetData(string format, object data) => throw new NotImplementedException();
        public void SetData(Type format, object data) => throw new NotImplementedException();
        public void SetData(object data) => throw new NotImplementedException();
    }

    private class CustomComDataObject : IComDataObject
    {
        public void GetData(ref FORMATETC format, out STGMEDIUM medium) => throw new NotImplementedException();
        public void GetDataHere(ref FORMATETC format, ref STGMEDIUM medium) => throw new NotImplementedException();
        public int QueryGetData(ref FORMATETC format) => throw new NotImplementedException();
        public int GetCanonicalFormatEtc(ref FORMATETC formatIn, out FORMATETC formatOut) => throw new NotImplementedException();
        public void SetData(ref FORMATETC formatIn, ref STGMEDIUM medium, bool release) => throw new NotImplementedException();
        public IEnumFORMATETC EnumFormatEtc(DATADIR direction) => throw new NotImplementedException();
        public int DAdvise(ref FORMATETC pFormatetc, ADVF advf, IAdviseSink adviseSink, out int connection) => throw new NotImplementedException();
        public void DUnadvise(int connection) => throw new NotImplementedException();
        public int EnumDAdvise(out IEnumSTATDATA enumAdvise) => throw new NotImplementedException();
    }

    [WinFormsFact]
    public void ToolStripItem_GetCurrentParent_InvokeWithoutParent_ReturnsNull()
    {
        using SubToolStripItem item = new();
        Assert.Null(item.GetCurrentParent());
    }

    [WinFormsFact]
    public void ToolStripItem_GetCurrentParent_InvokeWithOwner_ReturnsNull()
    {
        using ToolStrip parent = new();
        using SubToolStripItem item = new()
        {
            Parent = parent
        };
        Assert.Same(parent, item.GetCurrentParent());
    }

    [WinFormsFact]
    public void ToolStripItem_GetCurrentParent_InvokeWithParent_ReturnsExpected()
    {
        using ToolStrip owner = new();
        using SubToolStripItem item = new()
        {
            Owner = owner
        };
        Assert.Null(item.GetCurrentParent());
    }

    public static IEnumerable<object[]> GetPreferredSize_TestData()
    {
        yield return new object[] { Size.Empty };
        yield return new object[] { new Size(-1, -2) };
        yield return new object[] { new Size(10, 20) };
        yield return new object[] { new Size(30, 40) };
        yield return new object[] { new Size(int.MaxValue, int.MaxValue) };
    }

    [WinFormsTheory]
    [MemberData(nameof(GetPreferredSize_TestData))]
    public void ToolStripItem_GetPreferredSize_Invoke_ReturnsExpected(Size proposedSize)
    {
        using SubToolStripItem item = new();
        Assert.Equal(new Size(4, 4), item.GetPreferredSize(proposedSize));

        // Call again.
        Assert.Equal(new Size(4, 4), item.GetPreferredSize(proposedSize));
    }

    [WinFormsTheory]
    [MemberData(nameof(GetPreferredSize_TestData))]
    public void ToolStripItem_GetPreferredSize_InvokeWithOwner_ReturnsExpected(Size proposedSize)
    {
        using ToolStrip owner = new();
        using SubToolStripItem item = new()
        {
            Owner = owner
        };
        Assert.Equal(new Size(4, 4), item.GetPreferredSize(proposedSize));

        // Call again.
        Assert.Equal(new Size(4, 4), item.GetPreferredSize(proposedSize));
    }

    [WinFormsTheory]
    [MemberData(nameof(GetPreferredSize_TestData))]
    public void ToolStripItem_GetPreferredSize_InvokeWithParent_ReturnsExpected(Size proposedSize)
    {
        using ToolStrip parent = new();
        using SubToolStripItem item = new()
        {
            Parent = parent
        };
        Assert.Equal(new Size(4, 4), item.GetPreferredSize(proposedSize));

        // Call again.
        Assert.Equal(new Size(4, 4), item.GetPreferredSize(proposedSize));
    }

    [WinFormsTheory]
    [InlineData('a')]
    [InlineData('\0')]
    public void ToolStripItem_IsInputChar_Invoke_ReturnsFalse(char charCode)
    {
        using SubToolStripItem item = new();
        Assert.False(item.IsInputChar(charCode));
    }

    [WinFormsTheory]
    [InlineData(Keys.None)]
    [InlineData(Keys.A)]
    [InlineData(Keys.Enter)]
    [InlineData(Keys.Space)]
    [InlineData((Keys.None - 1))]
    public void ToolStripItem_IsInputKey_Invoke_ReturnsFalse(Keys keyData)
    {
        using SubToolStripItem item = new();
        Assert.False(item.IsInputKey(keyData));
    }

    [WinFormsFact]
    public void ToolStripItem_Invalidate_Invoke_Nop()
    {
        using SubToolStripItem item = new();
        item.Invalidate();

        // Call again.
        item.Invalidate();
    }

    [WinFormsFact]
    public void ToolStripItem_Invalidate_InvokeWithOwner_Nop()
    {
        using ToolStrip owner = new();
        using SubToolStripItem item = new()
        {
            Owner = owner
        };
        item.Invalidate();
        Assert.False(owner.IsHandleCreated);

        // Call again.
        item.Invalidate();
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsFact]
    public void ToolStripItem_Invalidate_InvokeWithOwnerWithHandler_Nop()
    {
        using ToolStrip owner = new();
        using SubToolStripItem item = new()
        {
            Owner = owner
        };
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;

        item.Invalidate();
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Call again.
        item.Invalidate();
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void ToolStripItem_Invalidate_InvokeWithParent_Nop()
    {
        using ToolStrip parent = new();
        using SubToolStripItem item = new()
        {
            Parent = parent
        };
        item.Invalidate();
        Assert.False(parent.IsHandleCreated);

        // Call again.
        item.Invalidate();
        Assert.False(parent.IsHandleCreated);
    }

    [WinFormsFact]
    public void ToolStripItem_Invalidate_InvokeWithParentWithHandler_CallsInvalidate()
    {
        using ToolStrip parent = new();
        using SubToolStripItem item = new()
        {
            Parent = parent
        };
        Assert.NotEqual(IntPtr.Zero, parent.Handle);
        int invalidatedCallCount = 0;
        parent.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        parent.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        parent.HandleCreated += (sender, e) => createdCallCount++;

        item.Invalidate();
        Assert.True(parent.IsHandleCreated);
        Assert.Equal(1, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Call again.
        item.Invalidate();
        Assert.True(parent.IsHandleCreated);
        Assert.Equal(2, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> Invalidate_Rectangle_TestData()
    {
        yield return new object[] { Rectangle.Empty };
        yield return new object[] { new Rectangle(1, 2, 3, 4) };
    }

    [WinFormsTheory]
    [MemberData(nameof(Invalidate_Rectangle_TestData))]
    public void ToolStripItem_Invalidate_InvokeRectangle_Nop(Rectangle r)
    {
        using SubToolStripItem item = new();
        item.Invalidate(r);

        // Call again.
        item.Invalidate(r);
    }

    [WinFormsTheory]
    [MemberData(nameof(Invalidate_Rectangle_TestData))]
    public void ToolStripItem_Invalidate_InvokeRectangleWithOwner_Nop(Rectangle r)
    {
        using ToolStrip owner = new();
        using SubToolStripItem item = new()
        {
            Owner = owner
        };
        item.Invalidate(r);
        Assert.False(owner.IsHandleCreated);

        // Call again.
        item.Invalidate(r);
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(Invalidate_Rectangle_TestData))]
    public void ToolStripItem_Invalidate_InvokeRectangleWithOwnerWithHandler_Nop(Rectangle r)
    {
        using ToolStrip owner = new();
        using SubToolStripItem item = new()
        {
            Owner = owner
        };
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;

        item.Invalidate(r);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Call again.
        item.Invalidate(r);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [MemberData(nameof(Invalidate_Rectangle_TestData))]
    public void ToolStripItem_Invalidate_InvokeRectangleWithParent_Nop(Rectangle r)
    {
        using ToolStrip parent = new();
        using SubToolStripItem item = new()
        {
            Parent = parent
        };
        item.Invalidate(r);
        Assert.False(parent.IsHandleCreated);

        // Call again.
        item.Invalidate(r);
        Assert.False(parent.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(Invalidate_Rectangle_TestData))]
    public void ToolStripItem_Invalidate_InvokeRectangleWithParentWithHandler_CallsInvalidate(Rectangle r)
    {
        using ToolStrip parent = new();
        using SubToolStripItem item = new()
        {
            Parent = parent
        };
        Assert.NotEqual(IntPtr.Zero, parent.Handle);
        int invalidatedCallCount = 0;
        parent.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        parent.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        parent.HandleCreated += (sender, e) => createdCallCount++;

        item.Invalidate(r);
        Assert.True(parent.IsHandleCreated);
        Assert.Equal(1, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Call again.
        item.Invalidate(r);
        Assert.True(parent.IsHandleCreated);
        Assert.Equal(2, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void ToolStripItem_OnAvailableChanged_Invoke_CallsAvailableChanged(EventArgs eventArgs)
    {
        using SubToolStripItem item = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(item, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        item.AvailableChanged += handler;
        item.OnAvailableChanged(eventArgs);
        Assert.Equal(1, callCount);

        // Remove handler.
        item.AvailableChanged -= handler;
        item.OnAvailableChanged(eventArgs);
        Assert.Equal(1, callCount);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void ToolStripItem_OnBackColorChanged_Invoke_CallsBackColorChanged(EventArgs eventArgs)
    {
        using SubToolStripItem item = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(item, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        item.BackColorChanged += handler;
        item.OnBackColorChanged(eventArgs);
        Assert.Equal(1, callCount);

        // Remove handler.
        item.BackColorChanged -= handler;
        item.OnBackColorChanged(eventArgs);
        Assert.Equal(1, callCount);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void ToolStripItem_OnBackColorChanged_InvokeWithOwner_CallsBackColorChanged(EventArgs eventArgs)
    {
        using ToolStrip owner = new();
        using SubToolStripItem item = new()
        {
            Owner = owner
        };
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(item, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        item.BackColorChanged += handler;
        item.OnBackColorChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.False(owner.IsHandleCreated);

        // Remove handler.
        item.BackColorChanged -= handler;
        item.OnBackColorChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void ToolStripItem_OnBackColorChanged_InvokeWithOwnerWithHandle_CallsBackColorChanged(EventArgs eventArgs)
    {
        using ToolStrip owner = new();
        using SubToolStripItem item = new()
        {
            Owner = owner
        };
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;

        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(item, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        item.BackColorChanged += handler;
        item.OnBackColorChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Remove handler.
        item.BackColorChanged -= handler;
        item.OnBackColorChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void ToolStripItem_OnBackColorChanged_InvokeWithParent_CallsBackColorChanged(EventArgs eventArgs)
    {
        using ToolStrip parent = new();
        using SubToolStripItem item = new()
        {
            Parent = parent
        };
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(item, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        item.BackColorChanged += handler;
        item.OnBackColorChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.False(parent.IsHandleCreated);

        // Remove handler.
        item.BackColorChanged -= handler;
        item.OnBackColorChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.False(parent.IsHandleCreated);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void ToolStripItem_OnBackColorChanged_InvokeWithParentWithHandle_CallsBackColorChanged(EventArgs eventArgs)
    {
        using ToolStrip parent = new();
        using SubToolStripItem item = new()
        {
            Parent = parent
        };
        Assert.NotEqual(IntPtr.Zero, parent.Handle);
        int invalidatedCallCount = 0;
        parent.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        parent.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        parent.HandleCreated += (sender, e) => createdCallCount++;

        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(item, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        item.BackColorChanged += handler;
        item.OnBackColorChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.True(parent.IsHandleCreated);
        Assert.Equal(1, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Remove handler.
        item.BackColorChanged -= handler;
        item.OnBackColorChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.True(parent.IsHandleCreated);
        Assert.Equal(2, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void ToolStripItem_OnBoundsChanged_Invoke_Success()
    {
        using SubToolStripItem item = new();
        item.OnBoundsChanged();
    }

    [WinFormsFact]
    public void ToolStripItem_OnBoundsChanged_InvokeWithOwner_Success()
    {
        using ToolStrip owner = new();
        using SubToolStripItem item = new()
        {
            Owner = owner
        };
        int ownerLayoutCallCount = 0;
        void ownerHandler(object sender, LayoutEventArgs e) => ownerLayoutCallCount++;
        owner.Layout += ownerHandler;

        try
        {
            item.OnBoundsChanged();
            Assert.Equal(0, ownerLayoutCallCount);
            Assert.False(owner.IsHandleCreated);
        }
        finally
        {
            owner.Layout -= ownerHandler;
        }
    }

    [WinFormsFact]
    public void ToolStripItem_OnBoundsChanged_InvokeWithOwnerWithHandle_Success()
    {
        using ToolStrip owner = new();
        using SubToolStripItem item = new()
        {
            Owner = owner
        };
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;
        int ownerLayoutCallCount = 0;
        void ownerHandler(object sender, LayoutEventArgs e) => ownerLayoutCallCount++;
        owner.Layout += ownerHandler;

        try
        {
            item.OnBoundsChanged();
            Assert.Equal(0, ownerLayoutCallCount);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }
        finally
        {
            owner.Layout -= ownerHandler;
        }
    }

    [WinFormsFact]
    public void ToolStripItem_OnBoundsChanged_InvokeWithParent_Success()
    {
        using ToolStrip parent = new();
        using SubToolStripItem item = new()
        {
            Parent = parent
        };
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(parent, sender);
            Assert.Same(item, e.AffectedComponent);
            Assert.Equal("Bounds", e.AffectedProperty);
            parentLayoutCallCount++;
        }

        parent.Layout += parentHandler;

        try
        {
            item.OnBoundsChanged();
            Assert.Equal(1, parentLayoutCallCount);
            Assert.False(parent.IsHandleCreated);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    [WinFormsFact]
    public void ToolStripItem_OnBoundsChanged_InvokeWithParentWithHandle_Success()
    {
        using ToolStrip parent = new();
        using SubToolStripItem item = new()
        {
            Parent = parent
        };
        Assert.NotEqual(IntPtr.Zero, parent.Handle);
        int invalidatedCallCount = 0;
        parent.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        parent.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        parent.HandleCreated += (sender, e) => createdCallCount++;
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(parent, sender);
            Assert.Same(item, e.AffectedComponent);
            Assert.Equal("Bounds", e.AffectedProperty);
            parentLayoutCallCount++;
        }

        parent.Layout += parentHandler;

        try
        {
            item.OnBoundsChanged();
            Assert.Equal(1, parentLayoutCallCount);
            Assert.True(parent.IsHandleCreated);
            Assert.Equal(1, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void ToolStripItem_OnClick_Invoke_CallsClick(EventArgs eventArgs)
    {
        using SubToolStripItem item = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(item, sender);
            Assert.Same(eventArgs, e);
            Assert.False(item.Pressed);
            callCount++;
        };

        // Call with handler.
        item.Click += handler;
        item.OnClick(eventArgs);
        Assert.Equal(1, callCount);

        // Remove handler.
        item.Click -= handler;
        item.OnClick(eventArgs);
        Assert.Equal(1, callCount);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void ToolStripItem_OnDisplayStyleChanged_Invoke_CallsDisplayStyleChanged(EventArgs eventArgs)
    {
        using SubToolStripItem item = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(item, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        item.DisplayStyleChanged += handler;
        item.OnDisplayStyleChanged(eventArgs);
        Assert.Equal(1, callCount);

        // Remove handler.
        item.DisplayStyleChanged -= handler;
        item.OnDisplayStyleChanged(eventArgs);
        Assert.Equal(1, callCount);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void ToolStripItem_OnDoubleClick_Invoke_CallsDoubleClick(EventArgs eventArgs)
    {
        using SubToolStripItem item = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(item, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        item.DoubleClick += handler;
        item.OnDoubleClick(eventArgs);
        Assert.Equal(1, callCount);

        // Remove handler.
        item.DoubleClick -= handler;
        item.OnDoubleClick(eventArgs);
        Assert.Equal(1, callCount);
    }

    public static IEnumerable<object[]> DragEventArgs_TestData()
    {
        yield return new object[] { null };
        yield return new object[] { new DragEventArgs(null, 0, 0, 0, DragDropEffects.None, DragDropEffects.None) };
    }

    [WinFormsTheory]
    [MemberData(nameof(DragEventArgs_TestData))]
    public void ToolStripItem_OnDragDrop_Invoke_CallsDragDrop(DragEventArgs eventArgs)
    {
        using SubToolStripItem item = new();
        int callCount = 0;
        DragEventHandler handler = (sender, e) =>
        {
            Assert.Same(item, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        item.DragDrop += handler;
        item.OnDragDrop(eventArgs);
        Assert.Equal(1, callCount);

        // Remove handler.
        item.DragDrop -= handler;
        item.OnDragDrop(eventArgs);
        Assert.Equal(1, callCount);
    }

    [WinFormsTheory]
    [MemberData(nameof(DragEventArgs_TestData))]
    public void ToolStripItem_OnDragEnter_Invoke_CallsDragEnter(DragEventArgs eventArgs)
    {
        using SubToolStripItem item = new();
        int callCount = 0;
        DragEventHandler handler = (sender, e) =>
        {
            Assert.Same(item, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        item.DragEnter += handler;
        item.OnDragEnter(eventArgs);
        Assert.Equal(1, callCount);

        // Remove handler.
        item.DragEnter -= handler;
        item.OnDragEnter(eventArgs);
        Assert.Equal(1, callCount);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void ToolStripItem_OnDragLeave_Invoke_CallsDragLeave(EventArgs eventArgs)
    {
        using SubToolStripItem item = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(item, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        item.DragLeave += handler;
        item.OnDragLeave(eventArgs);
        Assert.Equal(1, callCount);

        // Remove handler.
        item.DragLeave -= handler;
        item.OnDragLeave(eventArgs);
        Assert.Equal(1, callCount);
    }

    [WinFormsTheory]
    [MemberData(nameof(DragEventArgs_TestData))]
    public void ToolStripItem_OnDragOver_Invoke_CallsDragOver(DragEventArgs eventArgs)
    {
        using SubToolStripItem item = new();
        int callCount = 0;
        DragEventHandler handler = (sender, e) =>
        {
            Assert.Same(item, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        item.DragOver += handler;
        item.OnDragOver(eventArgs);
        Assert.Equal(1, callCount);

        // Remove handler.
        item.DragOver -= handler;
        item.OnDragOver(eventArgs);
        Assert.Equal(1, callCount);
    }

    public static IEnumerable<object[]> OnEnabledChanged_TestData()
    {
        foreach (bool enabled in new bool[] { true, false })
        {
            foreach (bool visible in new bool[] { true, false })
            {
                foreach (Image image in new Image[] { null, new Bitmap(10, 10) })
                {
                    yield return new object[] { enabled, visible, image, null };
                    yield return new object[] { enabled, visible, image, new EventArgs() };
                }
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(OnEnabledChanged_TestData))]
    public void ToolStripItem_OnEnabledChanged_Invoke_CallsEnabledChanged(bool enabled, bool visible, Image image, EventArgs eventArgs)
    {
        using SubToolStripItem item = new()
        {
            Enabled = enabled,
            Visible = visible,
            Image = image
        };
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(item, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        item.EnabledChanged += handler;
        item.OnEnabledChanged(eventArgs);
        Assert.Equal(1, callCount);

        // Remove handler.
        item.EnabledChanged -= handler;
        item.OnEnabledChanged(eventArgs);
        Assert.Equal(1, callCount);
    }

    [WinFormsTheory]
    [MemberData(nameof(OnEnabledChanged_TestData))]
    public void ToolStripItem_OnEnabledChanged_InvokeDesignMode_CallsEnabledChanged(bool enabled, bool visible, Image image, EventArgs eventArgs)
    {
        using SubToolStripItem item = new()
        {
            Enabled = enabled,
            Visible = visible,
            Image = image
        };
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(item, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        item.EnabledChanged += handler;
        item.OnEnabledChanged(eventArgs);
        Assert.Equal(1, callCount);

        // Remove handler.
        item.EnabledChanged -= handler;
        item.OnEnabledChanged(eventArgs);
        Assert.Equal(1, callCount);
    }

    [WinFormsTheory]
    [MemberData(nameof(OnEnabledChanged_TestData))]
    public void ToolStripItem_OnEnabledChanged_InvokeWithOwner_CallsEnabledChanged(bool enabled, bool visible, Image image, EventArgs eventArgs)
    {
        using ToolStrip owner = new();
        using SubToolStripItem item = new()
        {
            Enabled = enabled,
            Visible = visible,
            Image = image,
            Owner = owner
        };
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(item, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        item.EnabledChanged += handler;
        item.OnEnabledChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.False(owner.IsHandleCreated);

        // Remove handler.
        item.EnabledChanged -= handler;
        item.OnEnabledChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(OnEnabledChanged_TestData))]
    public void ToolStripItem_OnEnabledChanged_InvokeWithOwnerWithHandle_CallsEnabledChanged(bool enabled, bool visible, Image image, EventArgs eventArgs)
    {
        using ToolStrip owner = new();
        using SubToolStripItem item = new()
        {
            Enabled = enabled,
            Visible = visible,
            Image = image,
            Owner = owner
        };
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(item, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        item.EnabledChanged += handler;
        item.OnEnabledChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Remove handler.
        item.EnabledChanged -= handler;
        item.OnEnabledChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [MemberData(nameof(OnEnabledChanged_TestData))]
    public void ToolStripItem_OnEnabledChanged_InvokeWithParent_CallsEnabledChanged(bool enabled, bool visible, Image image, EventArgs eventArgs)
    {
        using ToolStrip parent = new();
        using SubToolStripItem item = new()
        {
            Enabled = enabled,
            Visible = visible,
            Image = image,
            Parent = parent
        };
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(item, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        item.EnabledChanged += handler;
        item.OnEnabledChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.False(parent.IsHandleCreated);

        // Remove handler.
        item.EnabledChanged -= handler;
        item.OnEnabledChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.False(parent.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(OnEnabledChanged_TestData))]
    public void ToolStripItem_OnEnabledChanged_InvokeWithParentWithHandle_CallsEnabledChanged(bool enabled, bool visible, Image image, EventArgs eventArgs)
    {
        using ToolStrip parent = new();
        using SubToolStripItem item = new()
        {
            Enabled = enabled,
            Visible = visible,
            Image = image,
            Parent = parent
        };
        Assert.NotEqual(IntPtr.Zero, parent.Handle);
        int invalidatedCallCount = 0;
        parent.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        parent.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        parent.HandleCreated += (sender, e) => createdCallCount++;
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(item, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        item.EnabledChanged += handler;
        item.OnEnabledChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.True(parent.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Remove handler.
        item.EnabledChanged -= handler;
        item.OnEnabledChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.True(parent.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> OnFontChanged_TestData()
    {
        foreach (ToolStripItemDisplayStyle displayStyle in Enum.GetValues(typeof(ToolStripItemDisplayStyle)))
        {
            yield return new object[] { displayStyle, null };
            yield return new object[] { displayStyle, new EventArgs() };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(OnFontChanged_TestData))]
    public void ToolStripItem_OnFontChanged_Invoke_Success(ToolStripItemDisplayStyle displayStyle, EventArgs eventArgs)
    {
        using SubToolStripItem item = new()
        {
            DisplayStyle = displayStyle
        };

        item.OnFontChanged(eventArgs);

        // Call again.
        item.OnFontChanged(eventArgs);
    }

    public static IEnumerable<object[]> OnFontChanged_WithOwner_TestData()
    {
        yield return new object[] { ToolStripItemDisplayStyle.None, null, 0 };
        yield return new object[] { ToolStripItemDisplayStyle.None, new EventArgs(), 0 };
        yield return new object[] { ToolStripItemDisplayStyle.Text, null, 1 };
        yield return new object[] { ToolStripItemDisplayStyle.Text, new EventArgs(), 1 };
        yield return new object[] { ToolStripItemDisplayStyle.Image, null, 0 };
        yield return new object[] { ToolStripItemDisplayStyle.Image, new EventArgs(), 0 };
        yield return new object[] { ToolStripItemDisplayStyle.ImageAndText, null, 1 };
        yield return new object[] { ToolStripItemDisplayStyle.ImageAndText, new EventArgs(), 1 };
    }

    [WinFormsTheory]
    [MemberData(nameof(OnFontChanged_WithOwner_TestData))]
    public void ToolStripItem_OnFontChanged_InvokeWithOwner_Success(ToolStripItemDisplayStyle displayStyle, EventArgs eventArgs, int expectedOwnerLayoutCallCount)
    {
        using ToolStrip owner = new();
        using SubToolStripItem item = new()
        {
            Owner = owner,
            DisplayStyle = displayStyle
        };
        int ownerLayoutCallCount = 0;
        void ownerHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(owner, sender);
            Assert.Same(item, e.AffectedComponent);
            Assert.Equal("Font", e.AffectedProperty);
            ownerLayoutCallCount++;
        }

        owner.Layout += ownerHandler;

        try
        {
            item.OnFontChanged(eventArgs);
            Assert.Equal(expectedOwnerLayoutCallCount, ownerLayoutCallCount);
            Assert.False(owner.IsHandleCreated);

            // Call again.
            item.OnFontChanged(eventArgs);
            Assert.Equal(expectedOwnerLayoutCallCount * 2, ownerLayoutCallCount);
            Assert.False(owner.IsHandleCreated);
        }
        finally
        {
            owner.Layout -= ownerHandler;
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(OnFontChanged_WithOwner_TestData))]
    public void ToolStripItem_OnFontChanged_InvokeWithOwnerWithHandle_Success(ToolStripItemDisplayStyle displayStyle, EventArgs eventArgs, int expectedParentLayoutCallCount)
    {
        using ToolStrip owner = new();
        using SubToolStripItem item = new()
        {
            Owner = owner,
            DisplayStyle = displayStyle
        };
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;
        int ownerLayoutCallCount = 0;
        void ownerHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(owner, sender);
            Assert.Same(item, e.AffectedComponent);
            Assert.Equal("Font", e.AffectedProperty);
            ownerLayoutCallCount++;
        }

        owner.Layout += ownerHandler;

        try
        {
            item.OnFontChanged(eventArgs);
            Assert.Equal(expectedParentLayoutCallCount, ownerLayoutCallCount);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(expectedParentLayoutCallCount * 2, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Call again.
            item.OnFontChanged(eventArgs);
            Assert.Equal(expectedParentLayoutCallCount * 2, ownerLayoutCallCount);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(expectedParentLayoutCallCount * 4, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }
        finally
        {
            owner.Layout -= ownerHandler;
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(OnFontChanged_TestData))]
    public void ToolStripItem_OnFontChanged_InvokeWithParent_Success(ToolStripItemDisplayStyle displayStyle, EventArgs eventArgs)
    {
        using ToolStrip parent = new();
        using SubToolStripItem item = new()
        {
            Parent = parent,
            DisplayStyle = displayStyle
        };
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e) => parentLayoutCallCount++;
        parent.Layout += parentHandler;

        try
        {
            item.OnFontChanged(eventArgs);
            Assert.Equal(0, parentLayoutCallCount);
            Assert.False(parent.IsHandleCreated);

            // Call again.
            item.OnFontChanged(eventArgs);
            Assert.Equal(0, parentLayoutCallCount);
            Assert.False(parent.IsHandleCreated);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(OnFontChanged_TestData))]
    public void ToolStripItem_OnFontChanged_InvokeWithParentWithHandle_Success(ToolStripItemDisplayStyle displayStyle, EventArgs eventArgs)
    {
        using ToolStrip parent = new();
        using SubToolStripItem item = new()
        {
            Parent = parent,
            DisplayStyle = displayStyle
        };
        Assert.NotEqual(IntPtr.Zero, parent.Handle);
        int invalidatedCallCount = 0;
        parent.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        parent.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        parent.HandleCreated += (sender, e) => createdCallCount++;
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e) => parentLayoutCallCount++;
        parent.Layout += parentHandler;

        try
        {
            item.OnFontChanged(eventArgs);
            Assert.Equal(0, parentLayoutCallCount);
            Assert.True(parent.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Call again.
            item.OnFontChanged(eventArgs);
            Assert.Equal(0, parentLayoutCallCount);
            Assert.True(parent.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void ToolStripItem_OnForeColorChanged_Invoke_CallsForeColorChanged(EventArgs eventArgs)
    {
        using SubToolStripItem item = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(item, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        item.ForeColorChanged += handler;
        item.OnForeColorChanged(eventArgs);
        Assert.Equal(1, callCount);

        // Remove handler.
        item.ForeColorChanged -= handler;
        item.OnForeColorChanged(eventArgs);
        Assert.Equal(1, callCount);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void ToolStripItem_OnForeColorChanged_InvokeWithOwner_CallsForeColorChanged(EventArgs eventArgs)
    {
        using ToolStrip owner = new();
        using SubToolStripItem item = new()
        {
            Owner = owner
        };
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(item, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        item.ForeColorChanged += handler;
        item.OnForeColorChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.False(owner.IsHandleCreated);

        // Remove handler.
        item.ForeColorChanged -= handler;
        item.OnForeColorChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void ToolStripItem_OnForeColorChanged_InvokeWithOwnerWithHandle_CallsForeColorChanged(EventArgs eventArgs)
    {
        using ToolStrip owner = new();
        using SubToolStripItem item = new()
        {
            Owner = owner
        };
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;

        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(item, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        item.ForeColorChanged += handler;
        item.OnForeColorChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Remove handler.
        item.ForeColorChanged -= handler;
        item.OnForeColorChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void ToolStripItem_OnForeColorChanged_InvokeWithParent_CallsForeColorChanged(EventArgs eventArgs)
    {
        using ToolStrip parent = new();
        using SubToolStripItem item = new()
        {
            Parent = parent
        };
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(item, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        item.ForeColorChanged += handler;
        item.OnForeColorChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.False(parent.IsHandleCreated);

        // Remove handler.
        item.ForeColorChanged -= handler;
        item.OnForeColorChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.False(parent.IsHandleCreated);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void ToolStripItem_OnForeColorChanged_InvokeWithParentWithHandle_CallsForeColorChanged(EventArgs eventArgs)
    {
        using ToolStrip parent = new();
        using SubToolStripItem item = new()
        {
            Parent = parent
        };
        Assert.NotEqual(IntPtr.Zero, parent.Handle);
        int invalidatedCallCount = 0;
        parent.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        parent.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        parent.HandleCreated += (sender, e) => createdCallCount++;

        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(item, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        item.ForeColorChanged += handler;
        item.OnForeColorChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.True(parent.IsHandleCreated);
        Assert.Equal(1, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Remove handler.
        item.ForeColorChanged -= handler;
        item.OnForeColorChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.True(parent.IsHandleCreated);
        Assert.Equal(2, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> OnGiveFeedback_TestData()
    {
        yield return new object[] { null };
        yield return new object[] { new GiveFeedbackEventArgs(DragDropEffects.None, true) };
    }

    [WinFormsTheory]
    [MemberData(nameof(OnGiveFeedback_TestData))]
    public void ToolStripItem_OnGiveFeedback_Invoke_CallsGiveFeedback(GiveFeedbackEventArgs eventArgs)
    {
        using SubToolStripItem item = new();
        int callCount = 0;
        GiveFeedbackEventHandler handler = (sender, e) =>
        {
            Assert.Same(item, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        item.GiveFeedback += handler;
        item.OnGiveFeedback(eventArgs);
        Assert.Equal(1, callCount);

        // Remove handler.
        item.GiveFeedback -= handler;
        item.OnGiveFeedback(eventArgs);
        Assert.Equal(1, callCount);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetLayoutEventArgsTheoryData))]
    public void ToolStripItem_OnLayout_Invoke_Nop(LayoutEventArgs eventArgs)
    {
        using SubToolStripItem item = new();

        item.OnLayout(eventArgs);

        // Call again.
        item.OnLayout(eventArgs);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void ToolStripItem_OnLocationChanged_Invoke_CallsLocationChanged(EventArgs eventArgs)
    {
        using SubToolStripItem item = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(item, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        item.LocationChanged += handler;
        item.OnLocationChanged(eventArgs);
        Assert.Equal(1, callCount);

        // Remove handler.
        item.LocationChanged -= handler;
        item.OnLocationChanged(eventArgs);
        Assert.Equal(1, callCount);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetMouseEventArgsTheoryData))]
    public void ToolStripItem_OnMouseDown_Invoke_Nop(MouseEventArgs eventArgs)
    {
        using SubToolStripItem item = new();
        int callCount = 0;
        MouseEventHandler handler = (sender, e) =>
        {
            Assert.Same(item, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        item.MouseDown += handler;
        item.OnMouseDown(eventArgs);
        Assert.Equal(0, callCount);

        // Remove handler.
        item.MouseDown -= handler;
        item.OnMouseDown(eventArgs);
        Assert.Equal(0, callCount);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void ToolStripItem_OnMouseEnter_Invoke_DoesNotCallMouseEnter(EventArgs eventArgs)
    {
        using SubToolStripItem item = new();
        int callCount = 0;
        EventHandler handler = (sender, e) => callCount++;

        // Call with handler.
        item.MouseEnter += handler;
        item.OnMouseEnter(eventArgs);
        Assert.Equal(0, callCount);

        // Remove handler.
        item.MouseEnter -= handler;
        item.OnMouseEnter(eventArgs);
        Assert.Equal(0, callCount);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void ToolStripItem_OnMouseHover_Invoke_DoesNotCallMouseHover(EventArgs eventArgs)
    {
        using SubToolStripItem item = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(item, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        item.MouseHover += handler;
        item.OnMouseHover(eventArgs);
        Assert.Equal(0, callCount);

        // Remove handler.
        item.MouseHover -= handler;
        item.OnMouseHover(eventArgs);
        Assert.Equal(0, callCount);
    }

    public static IEnumerable<object[]> OnMouseHover_WithOwner_TestData()
    {
        foreach (string toolTipText in new string[] { null, string.Empty, "ToolTipText" })
        {
            foreach (bool showItemToolTips in new bool[] { true, false })
            {
                yield return new object[] { toolTipText, showItemToolTips, null };
                yield return new object[] { toolTipText, showItemToolTips, new EventArgs() };
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(OnMouseHover_WithOwner_TestData))]
    public void ToolStripItem_OnMouseHover_InvokeWithOwner_Nop(string toolTipText, bool showItemToolTips, EventArgs eventArgs)
    {
        using ToolStrip owner = new()
        {
            ShowItemToolTips = showItemToolTips
        };
        using SubToolStripItem item = new()
        {
            ToolTipText = toolTipText,
            Owner = owner
        };
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(item, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        item.MouseHover += handler;
        item.OnMouseHover(eventArgs);
        Assert.Equal(0, callCount);

        // Remove handler.
        item.MouseHover -= handler;
        item.OnMouseHover(eventArgs);
        Assert.Equal(0, callCount);
    }

    [WinFormsTheory]
    [MemberData(nameof(OnMouseHover_WithOwner_TestData))]
    public void ToolStripItem_OnMouseHover_InvokeWithParent_Nop(string toolTipText, bool showItemToolTips, EventArgs eventArgs)
    {
        using ToolStrip parent = new()
        {
            ShowItemToolTips = showItemToolTips
        };
        using SubToolStripItem item = new()
        {
            ToolTipText = toolTipText,
            Parent = parent
        };
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(item, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        item.MouseHover += handler;
        item.OnMouseHover(eventArgs);
        Assert.Equal(0, callCount);

        // Remove handler.
        item.MouseHover -= handler;
        item.OnMouseHover(eventArgs);
        Assert.Equal(0, callCount);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void ToolStripItem_OnMouseLeave_Invoke_Nop(EventArgs eventArgs)
    {
        using SubToolStripItem item = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(item, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        item.MouseLeave += handler;
        item.OnMouseLeave(eventArgs);
        Assert.Equal(0, callCount);

        // Remove handler.
        item.MouseLeave -= handler;
        item.OnMouseLeave(eventArgs);
        Assert.Equal(0, callCount);
    }

    public static IEnumerable<object[]> OnMouseLeave_WithOwner_TestData()
    {
        foreach (string toolTipText in new string[] { null, string.Empty, "ToolTipText" })
        {
            foreach (bool showItemToolTips in new bool[] { true, false })
            {
                yield return new object[] { toolTipText, showItemToolTips, null };
                yield return new object[] { toolTipText, showItemToolTips, new EventArgs() };
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(OnMouseLeave_WithOwner_TestData))]
    public void ToolStripItem_OnMouseLeave_InvokeWithOwner_Nop(string toolTipText, bool showItemToolTips, EventArgs eventArgs)
    {
        using ToolStrip owner = new()
        {
            ShowItemToolTips = showItemToolTips
        };
        using SubToolStripItem item = new()
        {
            ToolTipText = toolTipText,
            Owner = owner
        };
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(item, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        item.MouseLeave += handler;
        item.OnMouseLeave(eventArgs);
        Assert.Equal(0, callCount);

        // Remove handler.
        item.MouseLeave -= handler;
        item.OnMouseLeave(eventArgs);
        Assert.Equal(0, callCount);
    }

    [WinFormsTheory]
    [MemberData(nameof(OnMouseLeave_WithOwner_TestData))]
    public void ToolStripItem_OnMouseLeave_InvokeWithOwnerAfterHover_Nop(string toolTipText, bool showItemToolTips, EventArgs eventArgs)
    {
        using ToolStrip owner = new()
        {
            ShowItemToolTips = showItemToolTips
        };
        using SubToolStripItem item = new()
        {
            ToolTipText = toolTipText,
            Owner = owner
        };
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(item, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };
        item.OnMouseHover(eventArgs);

        // Call with handler.
        item.MouseLeave += handler;
        item.OnMouseLeave(eventArgs);
        Assert.Equal(0, callCount);

        // Remove handler.
        item.MouseLeave -= handler;
        item.OnMouseLeave(eventArgs);
        Assert.Equal(0, callCount);
    }

    [WinFormsTheory]
    [MemberData(nameof(OnMouseLeave_WithOwner_TestData))]
    public void ToolStripItem_OnMouseLeave_InvokeWithParent_Nop(string toolTipText, bool showItemToolTips, EventArgs eventArgs)
    {
        using ToolStrip parent = new()
        {
            ShowItemToolTips = showItemToolTips
        };
        using SubToolStripItem item = new()
        {
            ToolTipText = toolTipText,
            Parent = parent
        };
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(item, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        item.MouseLeave += handler;
        item.OnMouseLeave(eventArgs);
        Assert.Equal(0, callCount);

        // Remove handler.
        item.MouseLeave -= handler;
        item.OnMouseLeave(eventArgs);
        Assert.Equal(0, callCount);
    }

    [WinFormsTheory]
    [MemberData(nameof(OnMouseLeave_WithOwner_TestData))]
    public void ToolStripItem_OnMouseLeave_InvokeWithParentAfterHover_Nop(string toolTipText, bool showItemToolTips, EventArgs eventArgs)
    {
        using ToolStrip parent = new()
        {
            ShowItemToolTips = showItemToolTips
        };
        using SubToolStripItem item = new()
        {
            ToolTipText = toolTipText,
            Parent = parent
        };
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(item, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };
        item.OnMouseHover(eventArgs);

        // Call with handler.
        item.MouseLeave += handler;
        item.OnMouseLeave(eventArgs);
        Assert.Equal(0, callCount);

        // Remove handler.
        item.MouseLeave -= handler;
        item.OnMouseLeave(eventArgs);
        Assert.Equal(0, callCount);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetMouseEventArgsTheoryData))]
    public void ToolStripItem_OnMouseMove_Invoke_Nop(MouseEventArgs eventArgs)
    {
        using SubToolStripItem item = new();
        int callCount = 0;
        MouseEventHandler handler = (sender, e) =>
        {
            Assert.Same(item, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        item.MouseMove += handler;
        item.OnMouseMove(eventArgs);
        Assert.Equal(0, callCount);

        // Remove handler.
        item.MouseMove -= handler;
        item.OnMouseMove(eventArgs);
        Assert.Equal(0, callCount);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetMouseEventArgsTheoryData))]
    public void ToolStripItem_OnMouseUp_Invoke_Nop(MouseEventArgs eventArgs)
    {
        using SubToolStripItem item = new();
        int callCount = 0;
        MouseEventHandler handler = (sender, e) =>
        {
            Assert.Same(item, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        item.MouseUp += handler;
        item.OnMouseUp(eventArgs);
        Assert.Equal(0, callCount);

        // Remove handler.
        item.MouseUp -= handler;
        item.OnMouseUp(eventArgs);
        Assert.Equal(0, callCount);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void ToolStripItem_OnOwnerChanged_Invoke_CallsOwnerChanged(EventArgs eventArgs)
    {
        using SubToolStripItem item = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(item, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        item.OwnerChanged += handler;
        item.OnOwnerChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Equal(new Padding(0, 1, 0, 2), item.Margin);

        // Remove handler.
        item.OwnerChanged -= handler;
        item.OnOwnerChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Equal(new Padding(0, 1, 0, 2), item.Margin);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void ToolStripItem_OnOwnerChanged_InvokeWithMargin_CallsOwnerChanged(EventArgs eventArgs)
    {
        using SubToolStripItem item = new()
        {
            Margin = new Padding(1, 2, 3, 4)
        };

        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(item, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        item.OwnerChanged += handler;
        item.OnOwnerChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Equal(new Padding(1, 2, 3, 4), item.Margin);

        // Remove handler.
        item.OwnerChanged -= handler;
        item.OnOwnerChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Equal(new Padding(1, 2, 3, 4), item.Margin);
    }

    public static IEnumerable<object[]> OnOwnerChanged_TestData()
    {
        yield return new object[] { RightToLeft.Yes, RightToLeft.Yes, null, RightToLeft.Yes, 0 };
        yield return new object[] { RightToLeft.Yes, RightToLeft.Yes, new EventArgs(), RightToLeft.Yes, 0 };

        yield return new object[] { RightToLeft.Yes, RightToLeft.No, null, RightToLeft.No, 0 };
        yield return new object[] { RightToLeft.Yes, RightToLeft.No, new EventArgs(), RightToLeft.No, 0 };

        yield return new object[] { RightToLeft.Yes, RightToLeft.Inherit, null, RightToLeft.Yes, 1 };
        yield return new object[] { RightToLeft.Yes, RightToLeft.Inherit, new EventArgs(), RightToLeft.Yes, 1 };

        yield return new object[] { RightToLeft.No, RightToLeft.Yes, null, RightToLeft.Yes, 0 };
        yield return new object[] { RightToLeft.No, RightToLeft.Yes, new EventArgs(), RightToLeft.Yes, 0 };

        yield return new object[] { RightToLeft.No, RightToLeft.No, null, RightToLeft.No, 0 };
        yield return new object[] { RightToLeft.No, RightToLeft.No, new EventArgs(), RightToLeft.No, 0 };

        yield return new object[] { RightToLeft.No, RightToLeft.Inherit, null, RightToLeft.No, 1 };
        yield return new object[] { RightToLeft.No, RightToLeft.Inherit, new EventArgs(), RightToLeft.No, 1 };

        yield return new object[] { RightToLeft.Inherit, RightToLeft.Yes, null, RightToLeft.Yes, 0 };
        yield return new object[] { RightToLeft.Inherit, RightToLeft.Yes, new EventArgs(), RightToLeft.Yes, 0 };

        yield return new object[] { RightToLeft.Inherit, RightToLeft.No, null, RightToLeft.No, 0 };
        yield return new object[] { RightToLeft.Inherit, RightToLeft.No, new EventArgs(), RightToLeft.No, 0 };

        yield return new object[] { RightToLeft.Inherit, RightToLeft.Inherit, null, RightToLeft.No, 1 };
        yield return new object[] { RightToLeft.Inherit, RightToLeft.Inherit, new EventArgs(), RightToLeft.No, 1 };
    }

    [WinFormsTheory]
    [MemberData(nameof(OnOwnerChanged_TestData))]
    public void ToolStripItem_OnOwnerChanged_InvokeWithRightToLeft_CallsHandler(RightToLeft ownerRightToLeft, RightToLeft rightToLeft, EventArgs eventArgs, RightToLeft expectedRightToLeft, int expectedRightToLeftChangedCallCount)
    {
        using ToolStrip owner = new()
        {
            RightToLeft = ownerRightToLeft
        };
        using SubToolStripItem item = new()
        {
            Owner = owner,
            RightToLeft = rightToLeft
        };

        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(item, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };
        int rightToLeftChangedCallCount = 0;
        item.RightToLeftChanged += (sender, e) =>
        {
            Assert.Same(item, sender);
            Assert.Same(EventArgs.Empty, e);
            rightToLeftChangedCallCount++;
        };

        // Call with handler.
        item.OwnerChanged += handler;
        item.OnOwnerChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Equal(expectedRightToLeft, item.RightToLeft);
        Assert.Equal(expectedRightToLeftChangedCallCount, rightToLeftChangedCallCount);
        Assert.Equal(new Padding(0, 1, 0, 2), item.Margin);

        // Remove handler.
        item.OwnerChanged -= handler;
        item.OnOwnerChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Equal(expectedRightToLeft, item.RightToLeft);
        Assert.Equal(expectedRightToLeftChangedCallCount * 2, rightToLeftChangedCallCount);
        Assert.Equal(new Padding(0, 1, 0, 2), item.Margin);
    }

    [WinFormsTheory]
    [MemberData(nameof(OnFontChanged_TestData))]
    public void ToolStripItem_OnOwnerFontChanged_Invoke_Success(ToolStripItemDisplayStyle displayStyle, EventArgs eventArgs)
    {
        using SubToolStripItem item = new()
        {
            DisplayStyle = displayStyle
        };

        item.OnOwnerFontChanged(eventArgs);

        // Call again.
        item.OnOwnerFontChanged(eventArgs);

        // Call with font.
        item.Font = new Font("Arial", 8.25f);
        item.OnOwnerFontChanged(eventArgs);
    }

    [WinFormsTheory]
    [MemberData(nameof(OnFontChanged_WithOwner_TestData))]
    public void ToolStripItem_OnOwnerFontChanged_InvokeWithOwner_Success(ToolStripItemDisplayStyle displayStyle, EventArgs eventArgs, int expectedOwnerLayoutCallCount)
    {
        using ToolStrip owner = new();
        using SubToolStripItem item = new()
        {
            Owner = owner,
            DisplayStyle = displayStyle
        };
        int ownerLayoutCallCount = 0;
        void ownerHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(owner, sender);
            Assert.Same(item, e.AffectedComponent);
            Assert.Equal("Font", e.AffectedProperty);
            ownerLayoutCallCount++;
        }

        owner.Layout += ownerHandler;

        try
        {
            item.OnOwnerFontChanged(eventArgs);
            Assert.Equal(expectedOwnerLayoutCallCount, ownerLayoutCallCount);
            Assert.False(owner.IsHandleCreated);

            // Call again.
            item.OnOwnerFontChanged(eventArgs);
            Assert.Equal(expectedOwnerLayoutCallCount * 2, ownerLayoutCallCount);
            Assert.False(owner.IsHandleCreated);

            // Call with font.
            owner.Layout -= ownerHandler;
            item.Font = new Font("Arial", 8.25f);
            owner.Layout += ownerHandler;
            item.OnOwnerFontChanged(eventArgs);
            Assert.Equal(expectedOwnerLayoutCallCount * 2, ownerLayoutCallCount);
            Assert.False(owner.IsHandleCreated);
        }
        finally
        {
            owner.Layout -= ownerHandler;
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(OnFontChanged_WithOwner_TestData))]
    public void ToolStripItem_OnOwnerFontChanged_InvokeWithOwnerWithHandle_Success(ToolStripItemDisplayStyle displayStyle, EventArgs eventArgs, int expectedParentLayoutCallCount)
    {
        using ToolStrip owner = new();
        using SubToolStripItem item = new()
        {
            Owner = owner,
            DisplayStyle = displayStyle
        };
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        void invalidatedHandler(object sender, EventArgs e) => invalidatedCallCount++;
        owner.Invalidated += invalidatedHandler;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;
        int ownerLayoutCallCount = 0;
        void ownerHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(owner, sender);
            Assert.Same(item, e.AffectedComponent);
            Assert.Equal("Font", e.AffectedProperty);
            ownerLayoutCallCount++;
        }

        owner.Layout += ownerHandler;

        try
        {
            item.OnOwnerFontChanged(eventArgs);
            Assert.Equal(expectedParentLayoutCallCount, ownerLayoutCallCount);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(expectedParentLayoutCallCount * 2, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Call again.
            item.OnOwnerFontChanged(eventArgs);
            Assert.Equal(expectedParentLayoutCallCount * 2, ownerLayoutCallCount);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(expectedParentLayoutCallCount * 4, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Call with font.
            owner.Layout -= ownerHandler;
            owner.Invalidated -= invalidatedHandler;
            item.Font = new Font("Arial", 8.25f);
            owner.Layout += ownerHandler;
            owner.Invalidated += invalidatedHandler;
            item.OnOwnerFontChanged(eventArgs);
            Assert.Equal(expectedParentLayoutCallCount * 2, ownerLayoutCallCount);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(expectedParentLayoutCallCount * 4, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }
        finally
        {
            owner.Layout -= ownerHandler;
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(OnFontChanged_TestData))]
    public void ToolStripItem_OnOwnerFontChanged_InvokeWithParent_Success(ToolStripItemDisplayStyle displayStyle, EventArgs eventArgs)
    {
        using ToolStrip parent = new();
        using SubToolStripItem item = new()
        {
            Parent = parent,
            DisplayStyle = displayStyle
        };
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e) => parentLayoutCallCount++;
        parent.Layout += parentHandler;

        try
        {
            item.OnOwnerFontChanged(eventArgs);
            Assert.Equal(0, parentLayoutCallCount);
            Assert.False(parent.IsHandleCreated);

            // Call again.
            item.OnOwnerFontChanged(eventArgs);
            Assert.Equal(0, parentLayoutCallCount);
            Assert.False(parent.IsHandleCreated);

            // Call with font.
            item.Font = new Font("Arial", 8.25f);
            item.OnOwnerFontChanged(eventArgs);
            Assert.Equal(0, parentLayoutCallCount);
            Assert.False(parent.IsHandleCreated);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(OnFontChanged_TestData))]
    public void ToolStripItem_OnOwnerFontChanged_InvokeWithParentWithHandle_Success(ToolStripItemDisplayStyle displayStyle, EventArgs eventArgs)
    {
        using ToolStrip parent = new();
        using SubToolStripItem item = new()
        {
            Parent = parent,
            DisplayStyle = displayStyle
        };
        Assert.NotEqual(IntPtr.Zero, parent.Handle);
        int invalidatedCallCount = 0;
        parent.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        parent.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        parent.HandleCreated += (sender, e) => createdCallCount++;
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e) => parentLayoutCallCount++;
        parent.Layout += parentHandler;

        try
        {
            item.OnOwnerFontChanged(eventArgs);
            Assert.Equal(0, parentLayoutCallCount);
            Assert.True(parent.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Call again.
            item.OnOwnerFontChanged(eventArgs);
            Assert.Equal(0, parentLayoutCallCount);
            Assert.True(parent.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Call with font.
            item.Font = new Font("Arial", 8.25f);
            item.OnOwnerFontChanged(eventArgs);
            Assert.Equal(0, parentLayoutCallCount);
            Assert.True(parent.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetPaintEventArgsTheoryData))]
    public void ToolStripItem_OnPaint_Invoke_DoesNotCallPaint(PaintEventArgs eventArgs)
    {
        using SubToolStripItem item = new();
        int callCount = 0;
        PaintEventHandler handler = (sender, e) => callCount++;

        // Call with handler.
        item.Paint += handler;
        item.OnPaint(eventArgs);
        Assert.Equal(0, callCount);

        // Remove handler.
        item.Paint -= handler;
        item.OnPaint(eventArgs);
        Assert.Equal(0, callCount);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void ToolStripItem_OnParentBackColorChanged_Invoke_CallsBackColorChanged(EventArgs eventArgs)
    {
        using SubToolStripItem item = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(item, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        item.BackColorChanged += handler;
        item.OnParentBackColorChanged(eventArgs);
        Assert.Equal(1, callCount);

        item.BackColorChanged -= handler;
        item.BackColor = Color.Red;
        item.BackColorChanged += handler;
        item.OnParentBackColorChanged(eventArgs);
        Assert.Equal(1, callCount);

        // Remove handler.
        item.BackColorChanged -= handler;
        item.OnParentBackColorChanged(eventArgs);
        Assert.Equal(1, callCount);
    }

    public static IEnumerable<object[]> OnParentChanged_TestData()
    {
        foreach (bool enabled in new bool[] { true, false })
        {
            foreach (bool visible in new bool[] { true, false })
            {
                foreach (Image image in new Image[] { null, new Bitmap(10, 10) })
                {
                    foreach (bool allowDrop in new bool[] { true, false })
                    {
                        yield return new object[] { enabled, visible, image, allowDrop, null, null };
                        yield return new object[] { enabled, visible, image, allowDrop, null, new ToolStrip() };
                        yield return new object[] { enabled, visible, image, allowDrop, new ToolStrip(), null };
                        yield return new object[] { enabled, visible, image, allowDrop, new ToolStrip(), new ToolStrip() };
                    }
                }
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(OnParentChanged_TestData))]
    public void ToolStripItem_OnParentChanged_Invoke_Success(bool enabled, bool visible, Image image, bool allowDrop, ToolStrip oldParent, ToolStrip newParent)
    {
        using SubToolStripItem item = new()
        {
            Enabled = enabled,
            Visible = visible,
            Image = image,
            AllowDrop = allowDrop
        };
        item.OnParentChanged(oldParent, newParent);
    }

    [WinFormsTheory]
    [MemberData(nameof(OnEnabledChanged_TestData))]
    public void ToolStripItem_OnParentEnabledChanged_Invoke_CallsEnabledChanged(bool enabled, bool visible, Image image, EventArgs eventArgs)
    {
        using SubToolStripItem item = new()
        {
            Enabled = enabled,
            Visible = visible,
            Image = image
        };
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(item, sender);
            Assert.NotSame(eventArgs, e);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };

        // Call with handler.
        item.EnabledChanged += handler;
        item.OnParentEnabledChanged(eventArgs);
        Assert.Equal(1, callCount);

        // Remove handler.
        item.EnabledChanged -= handler;
        item.OnParentEnabledChanged(eventArgs);
        Assert.Equal(1, callCount);
    }

    [WinFormsFact]
    public void ToolStripItem_OnParentForeColorChanged_Invoke_CallsHandler()
    {
        using SubToolStripItem item = new();
        EventArgs eventArgs = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(item, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        item.ForeColorChanged += handler;
        item.OnParentForeColorChanged(eventArgs);
        Assert.Equal(1, callCount);

        item.ForeColorChanged -= handler;
        item.ForeColor = Color.Red;
        item.ForeColorChanged += handler;
        item.OnParentForeColorChanged(eventArgs);
        Assert.Equal(1, callCount);

        // Remove handler.
        item.ForeColorChanged -= handler;
        item.OnParentForeColorChanged(eventArgs);
        Assert.Equal(1, callCount);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void ToolStripItem_OnParentRightToLeftChanged_Invoke_CallsParentRightToLeftChanged(EventArgs eventArgs)
    {
        using SubToolStripItem item = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(item, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        item.RightToLeftChanged += handler;
        item.OnParentRightToLeftChanged(eventArgs);
        Assert.Equal(1, callCount);

        // Remove handler.
        item.RightToLeftChanged -= handler;
        item.OnParentRightToLeftChanged(eventArgs);
        Assert.Equal(1, callCount);
    }

    public static IEnumerable<object[]> OnParentRightToLeftChanged_TestData()
    {
        yield return new object[] { RightToLeft.Inherit, null, 1 };
        yield return new object[] { RightToLeft.Inherit, new EventArgs(), 1 };
        yield return new object[] { RightToLeft.Yes, null, 0 };
        yield return new object[] { RightToLeft.Yes, new EventArgs(), 0 };
        yield return new object[] { RightToLeft.No, null, 0 };
        yield return new object[] { RightToLeft.No, new EventArgs(), 0 };
    }

    [WinFormsTheory]
    [MemberData(nameof(OnParentRightToLeftChanged_TestData))]
    public void ToolStripItem_OnParentRightToLeftChanged_InvokeWithRightToLeft_CallsRightToLeftChange(RightToLeft rightToLeft, EventArgs eventArgs, int expectedCallCount)
    {
        using SubToolStripItem item = new()
        {
            RightToLeft = rightToLeft
        };
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(item, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        item.RightToLeftChanged += handler;
        item.OnParentRightToLeftChanged(eventArgs);
        Assert.Equal(expectedCallCount, callCount);

        // Remove handler.
        item.RightToLeftChanged -= handler;
        item.OnParentRightToLeftChanged(eventArgs);
        Assert.Equal(expectedCallCount, callCount);
    }

    public static IEnumerable<object[]> OnQueryContinueDrag_TestData()
    {
        yield return new object[] { null };
        yield return new object[] { new QueryContinueDragEventArgs(0, true, DragAction.Drop) };
    }

    [WinFormsTheory]
    [MemberData(nameof(OnQueryContinueDrag_TestData))]
    public void ToolStripItem_OnQueryContinueDrag_Invoke_CallsQueryContinueDrag(QueryContinueDragEventArgs eventArgs)
    {
        using SubToolStripItem item = new();
        int callCount = 0;
        QueryContinueDragEventHandler handler = (sender, e) =>
        {
            Assert.Same(item, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        item.QueryContinueDrag += handler;
        item.OnQueryContinueDrag(eventArgs);
        Assert.Equal(1, callCount);

        // Remove handler.
        item.QueryContinueDrag -= handler;
        item.OnQueryContinueDrag(eventArgs);
        Assert.Equal(1, callCount);
    }

    [WinFormsFact]
    public void ToolStripItem_QueryAccessibilityHelp_AddRemove_Success()
    {
        using SubToolStripItem item = new();
        QueryAccessibilityHelpEventHandler handler = (sender, e) => { };
        item.QueryAccessibilityHelp += handler;
        item.QueryAccessibilityHelp -= handler;
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void ToolStripItem_OnRightToLeftChanged_Invoke_CallsRightToLeftChanged(EventArgs eventArgs)
    {
        using SubToolStripItem item = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(item, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        item.RightToLeftChanged += handler;
        item.OnRightToLeftChanged(eventArgs);
        Assert.Equal(1, callCount);

        // Remove handler.
        item.RightToLeftChanged -= handler;
        item.OnRightToLeftChanged(eventArgs);
        Assert.Equal(1, callCount);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void ToolStripItem_OnRightToLeftChanged_InvokeWithOwner_CallsRightToLeftChanged(EventArgs eventArgs)
    {
        using ToolStrip owner = new();
        using SubToolStripItem item = new()
        {
            Owner = owner
        };
        int ownerLayoutCallCount = 0;
        void ownerHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(owner, sender);
            Assert.Same(item, e.AffectedComponent);
            Assert.Equal("RightToLeft", e.AffectedProperty);
            ownerLayoutCallCount++;
        }

        owner.Layout += ownerHandler;

        try
        {
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(item, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            item.RightToLeftChanged += handler;
            item.OnRightToLeftChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(1, ownerLayoutCallCount);
            Assert.False(owner.IsHandleCreated);

            // Remove handler.
            item.RightToLeftChanged -= handler;
            item.OnRightToLeftChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(2, ownerLayoutCallCount);
            Assert.False(owner.IsHandleCreated);
        }
        finally
        {
            owner.Layout -= ownerHandler;
        }
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void ToolStripItem_OnRightToLeftChanged_InvokeWithOwnerWithHandle_CallsRightToLeftChanged(EventArgs eventArgs)
    {
        using ToolStrip owner = new();
        using SubToolStripItem item = new()
        {
            Owner = owner
        };
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;
        int ownerLayoutCallCount = 0;
        void ownerHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(owner, sender);
            Assert.Same(item, e.AffectedComponent);
            Assert.Equal("RightToLeft", e.AffectedProperty);
            ownerLayoutCallCount++;
        }

        owner.Layout += ownerHandler;

        try
        {
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(item, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            item.RightToLeftChanged += handler;
            item.OnRightToLeftChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(1, ownerLayoutCallCount);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(2, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Remove handler.
            item.RightToLeftChanged -= handler;
            item.OnRightToLeftChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(2, ownerLayoutCallCount);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(4, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }
        finally
        {
            owner.Layout -= ownerHandler;
        }
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void ToolStripItem_OnRightToLeftChanged_InvokeWithParent_CallsRightToLeftChanged(EventArgs eventArgs)
    {
        using ToolStrip parent = new();
        using SubToolStripItem item = new()
        {
            Parent = parent
        };
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e) => parentLayoutCallCount++;
        parent.Layout += parentHandler;

        try
        {
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(item, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            item.RightToLeftChanged += handler;
            item.OnRightToLeftChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(0, parentLayoutCallCount);
            Assert.False(parent.IsHandleCreated);

            // Remove handler.
            item.RightToLeftChanged -= handler;
            item.OnRightToLeftChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(0, parentLayoutCallCount);
            Assert.False(parent.IsHandleCreated);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void ToolStripItem_OnRightToLeftChanged_InvokeWithParentWithHandle_CallsRightToLeftChanged(EventArgs eventArgs)
    {
        using ToolStrip parent = new();
        using SubToolStripItem item = new()
        {
            Parent = parent
        };
        Assert.NotEqual(IntPtr.Zero, parent.Handle);
        int invalidatedCallCount = 0;
        parent.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        parent.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        parent.HandleCreated += (sender, e) => createdCallCount++;
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e) => parentLayoutCallCount++;
        parent.Layout += parentHandler;

        try
        {
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(item, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            item.RightToLeftChanged += handler;
            item.OnRightToLeftChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(0, parentLayoutCallCount);
            Assert.True(parent.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Remove handler.
            item.RightToLeftChanged -= handler;
            item.OnRightToLeftChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(0, parentLayoutCallCount);
            Assert.True(parent.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void ToolStripItem_OnTextChanged_Invoke_CallsTextChanged(EventArgs eventArgs)
    {
        using SubToolStripItem item = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(item, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        item.TextChanged += handler;
        item.OnTextChanged(eventArgs);
        Assert.Equal(1, callCount);

        // Remove handler.
        item.TextChanged -= handler;
        item.OnTextChanged(eventArgs);
        Assert.Equal(1, callCount);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void ToolStripItem_OnTextChanged_InvokeWithOwner_CallsTextChanged(EventArgs eventArgs)
    {
        using ToolStrip owner = new();
        using SubToolStripItem item = new()
        {
            Owner = owner
        };
        int ownerLayoutCallCount = 0;
        void ownerHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(owner, sender);
            Assert.Same(item, e.AffectedComponent);
            Assert.Equal("Text", e.AffectedProperty);
            ownerLayoutCallCount++;
        }

        owner.Layout += ownerHandler;

        try
        {
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(item, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            item.TextChanged += handler;
            item.OnTextChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(1, ownerLayoutCallCount);
            Assert.False(owner.IsHandleCreated);

            // Remove handler.
            item.TextChanged -= handler;
            item.OnTextChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(2, ownerLayoutCallCount);
            Assert.False(owner.IsHandleCreated);
        }
        finally
        {
            owner.Layout -= ownerHandler;
        }
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void ToolStripItem_OnTextChanged_InvokeWithOwnerWithHandle_CallsTextChanged(EventArgs eventArgs)
    {
        using ToolStrip owner = new();
        using SubToolStripItem item = new()
        {
            Owner = owner
        };
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;
        int ownerLayoutCallCount = 0;
        void ownerHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(owner, sender);
            Assert.Same(item, e.AffectedComponent);
            Assert.Equal("Text", e.AffectedProperty);
            ownerLayoutCallCount++;
        }

        owner.Layout += ownerHandler;

        try
        {
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(item, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            item.TextChanged += handler;
            item.OnTextChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(1, ownerLayoutCallCount);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(2, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Remove handler.
            item.TextChanged -= handler;
            item.OnTextChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(2, ownerLayoutCallCount);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(4, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }
        finally
        {
            owner.Layout -= ownerHandler;
        }
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void ToolStripItem_OnTextChanged_InvokeWithParent_CallsTextChanged(EventArgs eventArgs)
    {
        using ToolStrip parent = new();
        using SubToolStripItem item = new()
        {
            Parent = parent
        };
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e) => parentLayoutCallCount++;
        parent.Layout += parentHandler;

        try
        {
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(item, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            item.TextChanged += handler;
            item.OnTextChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(0, parentLayoutCallCount);
            Assert.False(parent.IsHandleCreated);

            // Remove handler.
            item.TextChanged -= handler;
            item.OnTextChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(0, parentLayoutCallCount);
            Assert.False(parent.IsHandleCreated);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void ToolStripItem_OnTextChanged_InvokeWithParentWithHandle_CallsTextChanged(EventArgs eventArgs)
    {
        using ToolStrip parent = new();
        using SubToolStripItem item = new()
        {
            Parent = parent
        };
        Assert.NotEqual(IntPtr.Zero, parent.Handle);
        int invalidatedCallCount = 0;
        parent.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        parent.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        parent.HandleCreated += (sender, e) => createdCallCount++;
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e) => parentLayoutCallCount++;
        parent.Layout += parentHandler;

        try
        {
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(item, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            item.TextChanged += handler;
            item.OnTextChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(0, parentLayoutCallCount);
            Assert.True(parent.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Remove handler.
            item.TextChanged -= handler;
            item.OnTextChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(0, parentLayoutCallCount);
            Assert.True(parent.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    public static IEnumerable<object[]> OnVisibleChanged_TestData()
    {
        foreach (bool enabled in new bool[] { true, false })
        {
            foreach (bool visible in new bool[] { true, false })
            {
                foreach (Image image in new Image[] { null, new Bitmap(10, 10) })
                {
                    yield return new object[] { enabled, visible, image, null };
                    yield return new object[] { enabled, visible, image, new EventArgs() };
                }
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(OnVisibleChanged_TestData))]
    public void ToolStripItem_OnVisibleChanged_Invoke_CallsVisibleChanged(bool enabled, bool visible, Image image, EventArgs eventArgs)
    {
        using SubToolStripItem item = new()
        {
            Enabled = enabled,
            Visible = visible,
            Image = image
        };
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(item, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        item.VisibleChanged += handler;
        item.OnVisibleChanged(eventArgs);
        Assert.Equal(1, callCount);

        // Remove handler.
        item.VisibleChanged -= handler;
        item.OnVisibleChanged(eventArgs);
        Assert.Equal(1, callCount);
    }

    [WinFormsTheory]
    [MemberData(nameof(OnVisibleChanged_TestData))]
    public void ToolStripItem_OnVisibleChanged_InvokeDesignMode_CallsVisibleChanged(bool enabled, bool visible, Image image, EventArgs eventArgs)
    {
        using SubToolStripItem item = new()
        {
            Enabled = enabled,
            Visible = visible,
            Image = image
        };
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(item, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        item.VisibleChanged += handler;
        item.OnVisibleChanged(eventArgs);
        Assert.Equal(1, callCount);

        // Remove handler.
        item.VisibleChanged -= handler;
        item.OnVisibleChanged(eventArgs);
        Assert.Equal(1, callCount);
    }

    [WinFormsTheory]
    [MemberData(nameof(OnVisibleChanged_TestData))]
    public void ToolStripItem_OnVisibleChanged_InvokeWithOwner_CallsVisibleChanged(bool enabled, bool visible, Image image, EventArgs eventArgs)
    {
        using ToolStrip owner = new();
        using SubToolStripItem item = new()
        {
            Enabled = enabled,
            Visible = visible,
            Image = image,
            Owner = owner
        };
        int ownerLayoutCallCount = 0;
        void ownerHandler(object sender, LayoutEventArgs e) => ownerLayoutCallCount++;
        owner.Layout += ownerHandler;

        try
        {
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(item, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            item.VisibleChanged += handler;
            item.OnVisibleChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(owner.IsHandleCreated);

            // Remove handler.
            item.VisibleChanged -= handler;
            item.OnVisibleChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(owner.IsHandleCreated);
        }
        finally
        {
            owner.Layout -= ownerHandler;
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(OnVisibleChanged_TestData))]
    public void ToolStripItem_OnVisibleChanged_InvokeWithOwnerWithHandle_CallsVisibleChanged(bool enabled, bool visible, Image image, EventArgs eventArgs)
    {
        using ToolStrip owner = new();
        using SubToolStripItem item = new()
        {
            Enabled = enabled,
            Visible = visible,
            Image = image,
            Owner = owner
        };
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;
        int ownerLayoutCallCount = 0;
        void ownerHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(owner, sender);
            Assert.Same(item, e.AffectedComponent);
            Assert.Equal("Items", e.AffectedProperty);
            ownerLayoutCallCount++;
        }

        owner.Layout += ownerHandler;

        try
        {
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(item, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            item.VisibleChanged += handler;
            item.OnVisibleChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(1, ownerLayoutCallCount);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(2, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Remove handler.
            item.VisibleChanged -= handler;
            item.OnVisibleChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(2, ownerLayoutCallCount);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(4, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }
        finally
        {
            owner.Layout -= ownerHandler;
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(OnVisibleChanged_TestData))]
    public void ToolStripItem_OnVisibleChanged_InvokeWithDisposingOwner_CallsVisibleChanged(bool enabled, bool visible, Image image, EventArgs eventArgs)
    {
        using ToolStrip owner = new();
        using SubToolStripItem item = new()
        {
            Enabled = enabled,
            Visible = visible,
            Image = image,
            Owner = owner
        };
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;
        int ownerLayoutCallCount = 0;
        void ownerHandler(object sender, LayoutEventArgs e) => ownerLayoutCallCount++;
        owner.Layout += ownerHandler;
        int disposedCallCount = 0;
        owner.Disposed += (sender, e) =>
        {
            try
            {
                int callCount = 0;
                EventHandler handler = (sender, e) =>
                {
                    Assert.Same(item, sender);
                    Assert.Same(eventArgs, e);
                    callCount++;
                };

                // Call with handler.
                item.VisibleChanged += handler;
                item.OnVisibleChanged(eventArgs);
                Assert.Equal(1, callCount);
                Assert.Equal(0, ownerLayoutCallCount);
                Assert.False(owner.IsHandleCreated);
                Assert.Equal(0, invalidatedCallCount);
                Assert.Equal(0, styleChangedCallCount);
                Assert.Equal(0, createdCallCount);

                // Remove handler.
                item.VisibleChanged -= handler;
                item.OnVisibleChanged(eventArgs);
                Assert.Equal(1, callCount);
                Assert.Equal(0, ownerLayoutCallCount);
                Assert.False(owner.IsHandleCreated);
                Assert.Equal(0, invalidatedCallCount);
                Assert.Equal(0, styleChangedCallCount);
                Assert.Equal(0, createdCallCount);
            }
            finally
            {
                owner.Layout -= ownerHandler;
            }

            disposedCallCount++;
        };
        owner.Dispose();
        Assert.Equal(1, disposedCallCount);
    }

    [WinFormsTheory]
    [MemberData(nameof(OnVisibleChanged_TestData))]
    public void ToolStripItem_OnVisibleChanged_InvokeWithDisposedOwner_CallsVisibleChanged(bool enabled, bool visible, Image image, EventArgs eventArgs)
    {
        using ToolStrip owner = new();
        using SubToolStripItem item = new()
        {
            Enabled = enabled,
            Visible = visible,
            Image = image,
            Owner = owner
        };
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;
        int ownerLayoutCallCount = 0;
        void ownerHandler(object sender, LayoutEventArgs e) => ownerLayoutCallCount++;
        owner.Layout += ownerHandler;
        owner.Dispose();

        try
        {
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(item, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            item.VisibleChanged += handler;
            item.OnVisibleChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(0, ownerLayoutCallCount);
            Assert.False(owner.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Remove handler.
            item.VisibleChanged -= handler;
            item.OnVisibleChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(0, ownerLayoutCallCount);
            Assert.False(owner.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }
        finally
        {
            owner.Layout -= ownerHandler;
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(OnVisibleChanged_TestData))]
    public void ToolStripItem_OnVisibleChanged_InvokeWithParent_CallsVisibleChanged(bool enabled, bool visible, Image image, EventArgs eventArgs)
    {
        using ToolStrip parent = new();
        using SubToolStripItem item = new()
        {
            Enabled = enabled,
            Visible = visible,
            Image = image,
            Parent = parent
        };
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e) => parentLayoutCallCount++;
        parent.Layout += parentHandler;

        try
        {
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(item, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            item.VisibleChanged += handler;
            item.OnVisibleChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(parent.IsHandleCreated);

            // Remove handler.
            item.VisibleChanged -= handler;
            item.OnVisibleChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(parent.IsHandleCreated);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(OnVisibleChanged_TestData))]
    public void ToolStripItem_OnVisibleChanged_InvokeWithParentWithHandle_CallsVisibleChanged(bool enabled, bool visible, Image image, EventArgs eventArgs)
    {
        using ToolStrip parent = new();
        using SubToolStripItem item = new()
        {
            Enabled = enabled,
            Visible = visible,
            Image = image,
            Parent = parent
        };
        Assert.NotEqual(IntPtr.Zero, parent.Handle);
        int invalidatedCallCount = 0;
        parent.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        parent.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        parent.HandleCreated += (sender, e) => createdCallCount++;
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e) => parentLayoutCallCount++;
        parent.Layout += parentHandler;

        try
        {
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(item, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            item.VisibleChanged += handler;
            item.OnVisibleChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.True(parent.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Remove handler.
            item.VisibleChanged -= handler;
            item.OnVisibleChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.True(parent.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    public static IEnumerable<object[]> PerformClick_TestData()
    {
        yield return new object[] { true, true, 1 };
        yield return new object[] { true, false, 0 };
        yield return new object[] { false, true, 0 };
        yield return new object[] { false, false, 0 };
    }

    [WinFormsTheory]
    [MemberData(nameof(PerformClick_TestData))]
    public void ToolStripItem_PerformClick_Invoke_Success(bool enabled, bool available, int expectedClickCount)
    {
        using SubToolStripItem item = new()
        {
            Enabled = enabled,
            Available = available
        };

        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(item, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.True(item.Pressed);
            callCount++;
        };

        // Call with handler.
        item.Click += handler;
        item.PerformClick();
        Assert.Equal(expectedClickCount, callCount);
        Assert.False(item.Pressed);

        // Remove handler.
        item.Click -= handler;
        item.PerformClick();
        Assert.Equal(expectedClickCount, callCount);
        Assert.False(item.Pressed);
    }

    [WinFormsFact]
    public void ToolStripItem_PerformClick_InvokeDesignMode_Success()
    {
        Mock<ISite> mockSite = new();
        mockSite
            .Setup(s => s.DesignMode)
            .Returns(true);
        using SubToolStripItem item = new()
        {
            Site = mockSite.Object
        };

        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(item, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.False(item.Pressed);
            callCount++;
        };

        // Call with handler.
        item.Click += handler;
        item.PerformClick();
        Assert.Equal(1, callCount);
        Assert.False(item.Pressed);

        // Remove handler.
        item.Click -= handler;
        item.PerformClick();
        Assert.Equal(1, callCount);
        Assert.False(item.Pressed);
    }

    [WinFormsTheory]
    [MemberData(nameof(PerformClick_TestData))]
    public void ToolStripItem_PerformClick_InvokeDoesNotSupportItemClick_Success(bool enabled, bool available, int expectedClickCount)
    {
        using ToolStrip control = new();
        ToolStripItem grip = Assert.IsAssignableFrom<ToolStripItem>(Assert.Single(control.DisplayedItems));
        grip.Enabled = enabled;
        grip.Available = available;

        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(grip, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.False(grip.Pressed);
            callCount++;
        };
        int itemClickedCallCount = 0;
        ToolStripItemClickedEventHandler itemClickedHandler = (sender, e) => itemClickedCallCount++;
        control.ItemClicked += itemClickedHandler;

        // Call with handler.
        grip.Click += handler;
        grip.PerformClick();
        Assert.Equal(expectedClickCount, callCount);
        Assert.Equal(0, itemClickedCallCount);
        Assert.False(grip.Pressed);
        Assert.False(control.IsHandleCreated);

        // Remove handler.
        grip.Click -= handler;
        grip.PerformClick();
        Assert.Equal(expectedClickCount, callCount);
        Assert.Equal(0, itemClickedCallCount);
        Assert.False(grip.Pressed);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(PerformClick_TestData))]
    public void ToolStripItem_PerformClick_InvokeCantSelect_Success(bool enabled, bool available, int expectedClickCount)
    {
        using CannotSelectToolStripItem item = new()
        {
            Enabled = enabled,
            Available = available
        };

        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(item, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.False(item.Pressed);
            callCount++;
        };

        // Call with handler.
        item.Click += handler;
        item.PerformClick();
        Assert.Equal(expectedClickCount, callCount);
        Assert.False(item.Pressed);

        // Remove handler.
        item.Click -= handler;
        item.PerformClick();
        Assert.Equal(expectedClickCount, callCount);
        Assert.False(item.Pressed);
    }

    [WinFormsFact]
    public void ToolStripItem_PerformClick_InvokeWithOwner_Success()
    {
        using ToolStrip owner = new();
        using SubToolStripItem item = new()
        {
            Owner = owner
        };

        int callCount = 0;
        int itemClickedCallCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(item, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.True(item.Pressed);
            callCount++;
        };
        ToolStripItemClickedEventHandler itemClickedHandler = (sender, e) =>
        {
            Assert.Same(owner, sender);
            Assert.Same(item, e.ClickedItem);
            itemClickedCallCount++;
            Assert.True(itemClickedCallCount > callCount);
        };
        owner.ItemClicked += itemClickedHandler;

        // Call with handler.
        item.Click += handler;
        item.PerformClick();
        Assert.Equal(1, callCount);
        Assert.Equal(1, itemClickedCallCount);
        Assert.False(item.Pressed);
        Assert.False(owner.IsHandleCreated);

        // Remove handler.
        item.Click -= handler;
        item.PerformClick();
        Assert.Equal(1, callCount);
        Assert.Equal(2, itemClickedCallCount);
        Assert.False(item.Pressed);
        Assert.False(owner.IsHandleCreated);

        // Remove other handler.
        owner.ItemClicked -= itemClickedHandler;
        item.PerformClick();
        Assert.Equal(1, callCount);
        Assert.Equal(2, itemClickedCallCount);
        Assert.False(item.Pressed);
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsFact]
    public void ToolStripItem_PerformClick_InvokeWithOwnerWithHandle_Success()
    {
        using ToolStrip owner = new();
        using SubToolStripItem item = new()
        {
            Owner = owner
        };
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;

        int callCount = 0;
        int itemClickedCallCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(item, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.True(item.Pressed);
            callCount++;
        };
        ToolStripItemClickedEventHandler itemClickedHandler = (sender, e) =>
        {
            Assert.Same(owner, sender);
            Assert.Same(item, e.ClickedItem);
            itemClickedCallCount++;
            Assert.True(itemClickedCallCount > callCount);
        };
        owner.ItemClicked += itemClickedHandler;

        // Call with handler.
        item.Click += handler;
        item.PerformClick();
        Assert.Equal(1, callCount);
        Assert.Equal(1, itemClickedCallCount);
        Assert.False(item.Pressed);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Remove handler.
        item.Click -= handler;
        item.PerformClick();
        Assert.Equal(1, callCount);
        Assert.Equal(2, itemClickedCallCount);
        Assert.False(item.Pressed);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Remove other handler.
        owner.ItemClicked -= itemClickedHandler;
        item.PerformClick();
        Assert.Equal(1, callCount);
        Assert.Equal(2, itemClickedCallCount);
        Assert.False(item.Pressed);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void ToolStripItem_PerformClick_InvokeWithParent_Success()
    {
        using ToolStrip parent = new();
        using SubToolStripItem item = new()
        {
            Parent = parent
        };
        int itemClickedCallCount = 0;
        parent.ItemClicked += (sender, e) => itemClickedCallCount++;

        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(item, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.True(item.Pressed);
            callCount++;
        };

        // Call with handler.
        item.Click += handler;
        item.PerformClick();
        Assert.Equal(1, callCount);
        Assert.Equal(0, itemClickedCallCount);
        Assert.False(item.Pressed);
        Assert.False(parent.IsHandleCreated);

        // Remove handler.
        item.Click -= handler;
        item.PerformClick();
        Assert.Equal(1, callCount);
        Assert.Equal(0, itemClickedCallCount);
        Assert.False(item.Pressed);
        Assert.False(parent.IsHandleCreated);
    }

    [WinFormsFact]
    public void ToolStripItem_PerformClick_InvokeWithParentWithHandle_Success()
    {
        using ToolStrip parent = new();
        using SubToolStripItem item = new()
        {
            Parent = parent
        };
        Assert.NotEqual(IntPtr.Zero, parent.Handle);
        int invalidatedCallCount = 0;
        parent.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        parent.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        parent.HandleCreated += (sender, e) => createdCallCount++;
        int itemClickedCallCount = 0;
        parent.ItemClicked += (sender, e) => itemClickedCallCount++;

        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(item, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.True(item.Pressed);
            callCount++;
        };

        // Call with handler.
        item.Click += handler;
        item.PerformClick();
        Assert.Equal(1, callCount);
        Assert.Equal(0, itemClickedCallCount);
        Assert.False(item.Pressed);
        Assert.True(parent.IsHandleCreated);
        Assert.Equal(2, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Remove handler.
        item.Click -= handler;
        item.PerformClick();
        Assert.Equal(1, callCount);
        Assert.Equal(0, itemClickedCallCount);
        Assert.False(item.Pressed);
        Assert.True(parent.IsHandleCreated);
        Assert.Equal(4, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(Keys.None)]
    [InlineData(Keys.A)]
    [InlineData(Keys.Enter)]
    [InlineData(Keys.Space)]
    [InlineData((Keys.None - 1))]
    public void ToolStripItem_ProcessCmdKey_Invoke_ReturnsFalse(Keys keyData)
    {
        using SubToolStripItem item = new();
        Message message = default;
        Assert.False(item.ProcessCmdKey(ref message, keyData));
    }

    [WinFormsTheory]
    [InlineData(true, 1)]
    [InlineData(false, 0)]
    public void ToolStripItem_ProcessDialogKey_EnterKey_PerformsClick(bool enabled, int expectedCallCount)
    {
        using SubToolStripItem item = new()
        {
            Enabled = enabled
        };

        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(item, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.True(item.Pressed);
            callCount++;
        };

        item.Click += handler;
        Assert.True(item.ProcessDialogKey(Keys.Enter));
        Assert.Equal(expectedCallCount, callCount);
        Assert.False(item.Pressed);
    }

    [WinFormsTheory]
    [InlineData(true, 1)]
    [InlineData(false, 0)]
    public void ToolStripItem_ProcessDialogKey_EnterKeyWithParent_PerformsClick(bool enabled, int expectedCallCount)
    {
        using ToolStrip parent = new();
        using SubToolStripItem item = new()
        {
            Parent = parent,
            Enabled = enabled
        };

        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(item, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.True(item.Pressed);
            callCount++;
        };

        item.Click += handler;
        Assert.True(item.ProcessDialogKey(Keys.Enter));
        Assert.Equal(expectedCallCount, callCount);
        Assert.False(item.Pressed);
    }

    [WinFormsTheory]
    [InlineData(true, 1)]
    [InlineData(false, 0)]
    public void ToolStripItem_ProcessDialogKey_EnterKeyWithDropDownParent_PerformsClick(bool enabled, int expectedCallCount)
    {
        ToolStripDropDown parent = new();
        using SubToolStripItem item = new()
        {
            Parent = parent,
            Enabled = enabled
        };

        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(item, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.True(item.Pressed);
            callCount++;
        };

        item.Click += handler;
        Assert.True(item.ProcessDialogKey(Keys.Enter));
        Assert.Equal(expectedCallCount, callCount);
        Assert.False(item.Pressed);
    }

    [WinFormsTheory]
    [InlineData(Keys.Space)]
    [InlineData(Keys.A)]
    [InlineData(Keys.None)]
    [InlineData((Keys.None - 1))]
    public void ToolStripItem_ProcessDialogKey_UnknownKey_ReturnsFalse(Keys keyData)
    {
        using SubToolStripItem item = new();
        int callCount = 0;
        item.Click += (sender, e) => callCount++;
        Assert.False(item.ProcessDialogKey(keyData));
        Assert.Equal(0, callCount);
        Assert.False(item.Pressed);
    }

    [WinFormsTheory]
    [InlineData(true, 1)]
    [InlineData(false, 0)]
    public void ToolStripItem_ProcessMnemonic_Invoke_PerformsClick(bool enabled, int expectedCallCount)
    {
        using SubToolStripItem item = new()
        {
            Enabled = enabled
        };

        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(item, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.True(item.Pressed);
            callCount++;
        };

        item.Click += handler;
        Assert.True(item.ProcessMnemonic('a'));
        Assert.Equal(expectedCallCount, callCount);
        Assert.False(item.Pressed);
    }

    [WinFormsFact]
    public void ToolStripItem_ResetBackColor_Invoke_Success()
    {
        using SubToolStripItem item = new();

        // Reset without value.
        item.ResetBackColor();
        Assert.Equal(Control.DefaultBackColor, item.BackColor);

        // Reset with value.
        item.BackColor = Color.Black;
        item.ResetBackColor();
        Assert.Equal(Control.DefaultBackColor, item.BackColor);

        // Reset again.
        item.ResetBackColor();
        Assert.Equal(Control.DefaultBackColor, item.BackColor);
    }

    [WinFormsFact]
    public void ToolStripItem_ResetDisplayStyle_Invoke_Success()
    {
        using SubToolStripItem item = new();

        // Reset without value.
        item.ResetDisplayStyle();
        Assert.Equal(ToolStripItemDisplayStyle.ImageAndText, item.DisplayStyle);

        // Reset with value.
        item.DisplayStyle = ToolStripItemDisplayStyle.Text;
        item.ResetDisplayStyle();
        Assert.Equal(ToolStripItemDisplayStyle.ImageAndText, item.DisplayStyle);

        // Reset again.
        item.ResetDisplayStyle();
        Assert.Equal(ToolStripItemDisplayStyle.ImageAndText, item.DisplayStyle);
    }

    [WinFormsFact]
    public void ToolStripItem_ResetFont_Invoke_Success()
    {
        using Font font = new("Arial", 8.25f);
        using SubToolStripItem item = new();

        // Reset without value.
        Assert.NotSame(font, item.Font);
        Assert.NotSame(Control.DefaultFont, item.Font);
        Assert.Same(item.Font, item.Font);

        // Reset with value.
        item.Font = font;
        item.ResetFont();
        Assert.NotSame(font, item.Font);
        Assert.NotSame(Control.DefaultFont, item.Font);
        Assert.Same(item.Font, item.Font);

        // Reset again.
        item.ResetFont();
        Assert.NotSame(font, item.Font);
        Assert.NotSame(Control.DefaultFont, item.Font);
        Assert.Same(item.Font, item.Font);
    }

    [WinFormsFact]
    public void ToolStripItem_ResetForeColor_Invoke_Success()
    {
        using SubToolStripItem item = new();

        // Reset without value.
        item.ResetForeColor();
        Assert.Equal(Control.DefaultForeColor, item.ForeColor);

        // Reset with value.
        item.ForeColor = Color.Black;
        item.ResetForeColor();
        Assert.Equal(Control.DefaultForeColor, item.ForeColor);

        // Reset again.
        item.ResetForeColor();
        Assert.Equal(Control.DefaultForeColor, item.ForeColor);
    }

    [WinFormsFact]
    public void ToolStripItem_ResetImage_Invoke_Success()
    {
        using SubToolStripItem item = new();

        // Reset without value.
        item.ResetImage();
        Assert.Null(item.Image);

        // Reset with value.
        item.Image = new Bitmap(10, 10);
        item.ResetImage();
        Assert.Null(item.Image);

        // Reset again.
        item.ResetImage();
        Assert.Null(item.Image);
    }

    [WinFormsFact]
    public void ToolStripItem_ResetMargin_Invoke_Success()
    {
        using SubToolStripItem item = new();

        // Reset without value.
        item.ResetMargin();
        Assert.Equal(new Padding(0, 1, 0, 2), item.Margin);

        // Reset with value.
        item.Margin = new Padding(1, 2, 3, 4);
        item.ResetMargin();
        Assert.Equal(new Padding(0, 1, 0, 2), item.Margin);

        // Reset again.
        item.ResetMargin();
        Assert.Equal(new Padding(0, 1, 0, 2), item.Margin);
    }

    [WinFormsFact]
    public void ToolStripItem_ResetPadding_Invoke_Success()
    {
        using SubToolStripItem item = new();

        // Reset without value.
        item.ResetPadding();
        Assert.Equal(Padding.Empty, item.Padding);

        // Reset with value.
        item.Padding = new Padding(1, 2, 3, 4);
        item.ResetPadding();
        Assert.Equal(Padding.Empty, item.Padding);

        // Reset again.
        item.ResetPadding();
        Assert.Equal(Padding.Empty, item.Padding);
    }

    [WinFormsFact]
    public void ToolStripItem_ResetRightToLeft_Invoke_Success()
    {
        using SubToolStripItem item = new();

        // Reset without value.
        item.ResetRightToLeft();
        Assert.Equal(RightToLeft.Inherit, item.RightToLeft);

        // Reset with value.
        item.RightToLeft = RightToLeft.Yes;
        item.ResetRightToLeft();
        Assert.Equal(RightToLeft.Inherit, item.RightToLeft);

        // Reset again.
        item.ResetRightToLeft();
        Assert.Equal(RightToLeft.Inherit, item.RightToLeft);
    }

    [WinFormsFact]
    public void ToolStripItem_ResetTextDirection_Invoke_Success()
    {
        using SubToolStripItem item = new();

        // Reset without value.
        item.ResetTextDirection();
        Assert.Equal(ToolStripTextDirection.Horizontal, item.TextDirection);

        // Reset with value.
        item.TextDirection = ToolStripTextDirection.Vertical90;
        item.ResetTextDirection();
        Assert.Equal(ToolStripTextDirection.Horizontal, item.TextDirection);

        // Reset again.
        item.ResetTextDirection();
        Assert.Equal(ToolStripTextDirection.Horizontal, item.TextDirection);
    }

    [WinFormsTheory]
    [BoolData]
    public void ToolStripItem_Select_Invoke_Success(bool enabled)
    {
        using SubToolStripItem item = new()
        {
            Enabled = enabled
        };

        item.Select();
        Assert.Equal(item.CanSelect, item.Selected);

        // Select again.
        item.Select();
        Assert.Equal(item.CanSelect, item.Selected);
    }

    public static IEnumerable<object[]> Select_WithoutToolStripItemAccessibleObject_TestData()
    {
        yield return new object[] { null };
        yield return new object[] { new AccessibleObject() };
    }

    [WinFormsTheory]
    [MemberData(nameof(Select_WithoutToolStripItemAccessibleObject_TestData))]
    public void ToolStripItem_Select_InvokeWithoutToolStripItemAccessibleObject_Success(AccessibleObject result)
    {
        using CustomCreateAccessibilityInstanceToolStripItem item = new()
        {
            CreateAccessibilityInstanceResult = result
        };

        item.Select();
        Assert.True(item.Selected);

        // Select again.
        item.Select();
        Assert.True(item.Selected);
    }

    [WinFormsFact]
    public void ToolStripItem_Select_CantSelect_Success()
    {
        using CannotSelectToolStripItem item = new();
        item.Select();
        Assert.False(item.Selected);

        // Select again.
        item.Select();
        Assert.False(item.Selected);
    }

    [WinFormsFact]
    public void ToolStripItem_Select_InvokeWithOwnerItemOnDropdown_Success()
    {
        using ToolStripDropDown ownerItemOwner = new();
        using SubToolStripItem ownerItem = new()
        {
            Owner = ownerItemOwner
        };
        using ToolStripDropDown owner = new()
        {
            OwnerItem = ownerItem
        };
        using SubToolStripItem item = new()
        {
            Owner = owner
        };
        item.Select();
        Assert.True(item.Selected);
        Assert.True(ownerItem.Selected);

        // Select again.
        item.Select();
        Assert.True(item.Selected);
        Assert.True(ownerItem.Selected);
    }

    [WinFormsFact]
    public void ToolStripItem_Select_InvokeWithOwnerItemNotOnDropDown_Success()
    {
        using SubToolStripItem ownerItem = new();
        using ToolStripDropDown owner = new()
        {
            OwnerItem = ownerItem
        };
        using SubToolStripItem item = new()
        {
            Owner = owner
        };
        item.Select();
        Assert.True(item.Selected);
        Assert.False(ownerItem.Selected);

        // Select again.
        item.Select();
        Assert.True(item.Selected);
        Assert.False(ownerItem.Selected);
    }

    [WinFormsFact]
    public void ToolStripItem_Select_InvokeWithOwner_Success()
    {
        using ToolStrip owner = new();
        using SubToolStripItem item = new()
        {
            Owner = owner
        };

        item.Select();
        Assert.True(item.Selected);
        Assert.False(owner.IsHandleCreated);

        // Select again.
        item.Select();
        Assert.True(item.Selected);
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsFact]
    public void ToolStripItem_Select_InvokeWithOwnerWithHandle_Success()
    {
        using ToolStrip owner = new();
        using SubToolStripItem item = new()
        {
            Owner = owner
        };
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;

        item.Select();
        Assert.True(item.Selected);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Select again.
        item.Select();
        Assert.True(item.Selected);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void ToolStripItem_Select_InvokeWithDraggingOwner_Success()
    {
        using SubToolStrip owner = new();
        owner.OnBeginDrag(EventArgs.Empty);
        using SubToolStripItem item = new()
        {
            Owner = owner
        };

        item.Select();
        Assert.False(item.Selected);

        // Select again.
        item.Select();
        Assert.False(item.Selected);
    }

    [WinFormsFact]
    public void ToolStripItem_Select_InvokeWithSelectionSuspendedOwner_Success()
    {
        using SubToolStrip owner = new();
        owner.OnBeginDrag(EventArgs.Empty);
        using SubToolStripItem item = new()
        {
            Owner = owner
        };
        Assert.NotNull(owner.GetItemAt(item.Bounds.X, item.Bounds.Y));
        owner.OnMouseDown(new MouseEventArgs(MouseButtons.Left, 1, item.Bounds.X, item.Bounds.Y, 0));

        item.Select();
        Assert.False(item.Selected);

        // Select again.
        item.Select();
        Assert.False(item.Selected);
    }

    [WinFormsFact]
    public void ToolStripItem_Select_InvokeWithParent_Success()
    {
        using ToolStrip parent = new();
        using SubToolStripItem item = new()
        {
            Parent = parent
        };

        item.Select();
        Assert.True(item.Selected);
        Assert.False(parent.IsHandleCreated);

        // Select again.
        item.Select();
        Assert.True(item.Selected);
        Assert.False(parent.IsHandleCreated);
    }

    [WinFormsFact]
    public void ToolStripItem_Select_InvokeWithParentWithHandle_Success()
    {
        using ToolStrip parent = new();
        using SubToolStripItem item = new()
        {
            Parent = parent
        };
        Assert.NotEqual(IntPtr.Zero, parent.Handle);
        int invalidatedCallCount = 0;
        parent.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        parent.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        parent.HandleCreated += (sender, e) => createdCallCount++;

        item.Select();
        Assert.True(item.Selected);
        Assert.True(parent.IsHandleCreated);
        Assert.Equal(1, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Select again.
        item.Select();
        Assert.True(item.Selected);
        Assert.True(parent.IsHandleCreated);
        Assert.Equal(1, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void ToolStripItem_Select_InvokeWithDraggingParent_Success()
    {
        using SubToolStrip parent = new();
        parent.OnBeginDrag(EventArgs.Empty);
        using SubToolStripItem item = new()
        {
            Parent = parent
        };

        item.Select();
        Assert.True(item.Selected);

        // Select again.
        item.Select();
        Assert.True(item.Selected);
    }

    [WinFormsFact]
    public void ToolStripItem_Select_InvokeWithSelectionSuspendedParent_Success()
    {
        using SubToolStrip parent = new();
        parent.OnBeginDrag(EventArgs.Empty);
        using SubToolStripItem item = new()
        {
            Parent = parent
        };
        Assert.NotNull(parent.GetItemAt(item.Bounds.X, item.Bounds.Y));
        parent.OnMouseDown(new MouseEventArgs(MouseButtons.Left, 1, item.Bounds.X, item.Bounds.Y, 0));

        item.Select();
        Assert.False(item.Selected);

        // Select again.
        item.Select();
        Assert.False(item.Selected);
    }

    public static IEnumerable<object[]> SetBounds_TestData()
    {
        yield return new object[] { new Rectangle(1, 0, 23, 23), 1 };
        yield return new object[] { new Rectangle(0, 2, 23, 23), 1 };
        yield return new object[] { new Rectangle(1, 2, 23, 23), 1 };
        yield return new object[] { new Rectangle(0, 0, -1, -2), 0 };
        yield return new object[] { new Rectangle(0, 0, 0, 0), 0 };
        yield return new object[] { new Rectangle(0, 0, 1, 2), 0 };
        yield return new object[] { new Rectangle(0, 0, 22, 23), 0 };
        yield return new object[] { new Rectangle(0, 0, 23, 22), 0 };
        yield return new object[] { new Rectangle(0, 0, 23, 23), 0 };
        yield return new object[] { new Rectangle(1, 2, 3, 4), 1 };
    }

    [WinFormsTheory]
    [MemberData(nameof(SetBounds_TestData))]
    public void ToolStripItem_SetBounds_Invoke_GetReturnsExpected(Rectangle bounds, int expectedLocationChangedCallCount)
    {
        using SubToolStripItem item = new();
        int locationChangedCallCount = 0;
        item.LocationChanged += (sender, e) =>
        {
            Assert.Same(item, sender);
            Assert.Same(EventArgs.Empty, e);
            locationChangedCallCount++;
        };

        item.SetBounds(bounds);
        Assert.Equal(bounds, item.Bounds);
        Assert.Equal(bounds.Size, item.Size);
        Assert.Equal(bounds.Width, item.Width);
        Assert.Equal(bounds.Height, item.Height);
        Assert.Equal(expectedLocationChangedCallCount, locationChangedCallCount);

        // Set same.
        item.SetBounds(bounds);
        Assert.Equal(bounds, item.Bounds);
        Assert.Equal(bounds.Size, item.Size);
        Assert.Equal(bounds.Width, item.Width);
        Assert.Equal(bounds.Height, item.Height);
        Assert.Equal(expectedLocationChangedCallCount, locationChangedCallCount);
    }

    [WinFormsTheory]
    [MemberData(nameof(SetBounds_TestData))]
    public void ToolStripItem_SetBounds_InvokeWithOwner_GetReturnsExpected(Rectangle bounds, int expectedLocationChangedCallCount)
    {
        using ToolStrip owner = new();
        using SubToolStripItem item = new()
        {
            Owner = owner
        };
        int ownerLayoutCallCount = 0;
        void ownerHandler(object sender, LayoutEventArgs e) => ownerLayoutCallCount++;
        owner.Layout += ownerHandler;
        int locationChangedCallCount = 0;
        item.LocationChanged += (sender, e) =>
        {
            Assert.Same(item, sender);
            Assert.Same(EventArgs.Empty, e);
            locationChangedCallCount++;
        };

        try
        {
            item.SetBounds(bounds);
            Assert.Equal(bounds, item.Bounds);
            Assert.Equal(bounds.Size, item.Size);
            Assert.Equal(bounds.Width, item.Width);
            Assert.Equal(bounds.Height, item.Height);
            Assert.Equal(expectedLocationChangedCallCount, locationChangedCallCount);
            Assert.Equal(0, ownerLayoutCallCount);
            Assert.False(owner.IsHandleCreated);

            // Set same.
            item.SetBounds(bounds);
            Assert.Equal(bounds, item.Bounds);
            Assert.Equal(bounds.Size, item.Size);
            Assert.Equal(bounds.Width, item.Width);
            Assert.Equal(bounds.Height, item.Height);
            Assert.Equal(expectedLocationChangedCallCount, locationChangedCallCount);
            Assert.Equal(0, ownerLayoutCallCount);
            Assert.False(owner.IsHandleCreated);
        }
        finally
        {
            owner.Layout -= ownerHandler;
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(SetBounds_TestData))]
    public void ToolStripItem_SetBounds_InvokeWithOwnerWithHandle_GetReturnsExpected(Rectangle bounds, int expectedLocationChangedCallCount)
    {
        using ToolStrip owner = new();
        using SubToolStripItem item = new()
        {
            Owner = owner
        };
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;
        int ownerLayoutCallCount = 0;
        void ownerHandler(object sender, LayoutEventArgs e) => ownerLayoutCallCount++;
        owner.Layout += ownerHandler;
        int locationChangedCallCount = 0;
        item.LocationChanged += (sender, e) =>
        {
            Assert.Same(item, sender);
            Assert.Same(EventArgs.Empty, e);
            locationChangedCallCount++;
        };

        try
        {
            item.SetBounds(bounds);
            Assert.Equal(bounds, item.Bounds);
            Assert.Equal(bounds.Size, item.Size);
            Assert.Equal(bounds.Width, item.Width);
            Assert.Equal(bounds.Height, item.Height);
            Assert.Equal(expectedLocationChangedCallCount, locationChangedCallCount);
            Assert.Equal(0, ownerLayoutCallCount);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            item.SetBounds(bounds);
            Assert.Equal(bounds, item.Bounds);
            Assert.Equal(bounds.Size, item.Size);
            Assert.Equal(bounds.Width, item.Width);
            Assert.Equal(bounds.Height, item.Height);
            Assert.Equal(expectedLocationChangedCallCount, locationChangedCallCount);
            Assert.Equal(0, ownerLayoutCallCount);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }
        finally
        {
            owner.Layout -= ownerHandler;
        }
    }

    public static IEnumerable<object[]> SetBounds_WithParent_TestData()
    {
        yield return new object[] { new Rectangle(1, 0, 23, 23), 1, 1 };
        yield return new object[] { new Rectangle(0, 2, 23, 23), 1, 1 };
        yield return new object[] { new Rectangle(1, 2, 23, 23), 1, 1 };
        yield return new object[] { new Rectangle(0, 0, -1, -2), 0, 1 };
        yield return new object[] { new Rectangle(0, 0, 0, 0), 0, 1 };
        yield return new object[] { new Rectangle(0, 0, 1, 2), 0, 1 };
        yield return new object[] { new Rectangle(0, 0, 22, 23), 0, 1 };
        yield return new object[] { new Rectangle(0, 0, 23, 22), 0, 1 };
        yield return new object[] { new Rectangle(0, 0, 23, 23), 0, 0 };
        yield return new object[] { new Rectangle(1, 2, 3, 4), 1, 1 };
    }

    [WinFormsTheory]
    [MemberData(nameof(SetBounds_WithParent_TestData))]
    public void ToolStripItem_SetBounds_InvokeWithParent_GetReturnsExpected(Rectangle bounds, int expectedLocationChangedCallCount, int expectedParentLayoutCallCount)
    {
        using ToolStrip parent = new();
        using SubToolStripItem item = new()
        {
            Parent = parent
        };
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(parent, sender);
            Assert.Same(item, e.AffectedComponent);
            Assert.Equal("Bounds", e.AffectedProperty);
            parentLayoutCallCount++;
        }

        parent.Layout += parentHandler;
        int locationChangedCallCount = 0;
        item.LocationChanged += (sender, e) =>
        {
            Assert.Same(item, sender);
            Assert.Same(EventArgs.Empty, e);
            locationChangedCallCount++;
        };

        try
        {
            item.SetBounds(bounds);
            Assert.Equal(bounds, item.Bounds);
            Assert.Equal(bounds.Size, item.Size);
            Assert.Equal(bounds.Width, item.Width);
            Assert.Equal(bounds.Height, item.Height);
            Assert.Equal(expectedLocationChangedCallCount, locationChangedCallCount);
            Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
            Assert.False(parent.IsHandleCreated);

            // Set same.
            item.SetBounds(bounds);
            Assert.Equal(bounds, item.Bounds);
            Assert.Equal(bounds.Size, item.Size);
            Assert.Equal(bounds.Width, item.Width);
            Assert.Equal(bounds.Height, item.Height);
            Assert.Equal(expectedLocationChangedCallCount, locationChangedCallCount);
            Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
            Assert.False(parent.IsHandleCreated);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(SetBounds_WithParent_TestData))]
    public void ToolStripItem_SetBounds_InvokeWithParentWithHandle_GetReturnsExpected(Rectangle bounds, int expectedLocationChangedCallCount, int expectedParentLayoutCallCount)
    {
        using ToolStrip parent = new();
        using SubToolStripItem item = new()
        {
            Parent = parent
        };
        Assert.NotEqual(IntPtr.Zero, parent.Handle);
        int invalidatedCallCount = 0;
        parent.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        parent.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        parent.HandleCreated += (sender, e) => createdCallCount++;
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(parent, sender);
            Assert.Same(item, e.AffectedComponent);
            Assert.Equal("Bounds", e.AffectedProperty);
            parentLayoutCallCount++;
        }

        parent.Layout += parentHandler;
        int locationChangedCallCount = 0;
        item.LocationChanged += (sender, e) =>
        {
            Assert.Same(item, sender);
            Assert.Same(EventArgs.Empty, e);
            locationChangedCallCount++;
        };

        try
        {
            item.SetBounds(bounds);
            Assert.Equal(bounds, item.Bounds);
            Assert.Equal(bounds.Size, item.Size);
            Assert.Equal(bounds.Width, item.Width);
            Assert.Equal(bounds.Height, item.Height);
            Assert.Equal(expectedLocationChangedCallCount, locationChangedCallCount);
            Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
            Assert.True(parent.IsHandleCreated);
            Assert.Equal(expectedParentLayoutCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            item.SetBounds(bounds);
            Assert.Equal(bounds, item.Bounds);
            Assert.Equal(bounds.Size, item.Size);
            Assert.Equal(bounds.Width, item.Width);
            Assert.Equal(bounds.Height, item.Height);
            Assert.Equal(expectedLocationChangedCallCount, locationChangedCallCount);
            Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
            Assert.True(parent.IsHandleCreated);
            Assert.Equal(expectedParentLayoutCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    public static IEnumerable<object[]> SetVisibleCore_TestData()
    {
        foreach (bool enabled in new bool[] { true, false })
        {
            foreach (Image image in new Image[] { null, new Bitmap(10, 10) })
            {
                yield return new object[] { enabled, image, true };
                yield return new object[] { enabled, image, false };
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(SetVisibleCore_TestData))]
    public void ToolStripItem_SetVisibleCore_Invoke_GetReturnsExpected(bool enabled, Image image, bool value)
    {
        using SubToolStripItem item = new()
        {
            Enabled = enabled,
            Image = image
        };

        item.SetVisibleCore(value);
        Assert.False(item.Visible);
        Assert.Equal(value, item.Available);
        Assert.False(item.Selected);

        // Set same.
        item.SetVisibleCore(value);
        Assert.False(item.Visible);
        Assert.Equal(value, item.Available);
        Assert.False(item.Selected);

        // Set different.
        item.Available = !value;
        Assert.False(item.Visible);
        Assert.Equal(!value, item.Available);
        Assert.False(item.Selected);
    }

    [WinFormsTheory]
    [MemberData(nameof(SetVisibleCore_TestData))]
    public void ToolStripItem_SetVisibleCore_InvokeDesignMode_GetReturnsExpected(bool enabled, Image image, bool value)
    {
        Mock<ISite> mockSite = new(MockBehavior.Strict);
        mockSite
            .Setup(s => s.Name)
            .Returns("Name");
        mockSite
            .Setup(s => s.DesignMode)
            .Returns(true);
        mockSite
            .Setup(s => s.Container)
            .Returns((IContainer)null);
        using SubToolStripItem item = new()
        {
            Enabled = enabled,
            Image = image,
            Site = mockSite.Object
        };

        item.SetVisibleCore(value);
        Assert.False(item.Visible);
        Assert.Equal(value, item.Available);
        Assert.False(item.Selected);

        // Set same.
        item.SetVisibleCore(value);
        Assert.False(item.Visible);
        Assert.Equal(value, item.Available);
        Assert.False(item.Selected);

        // Set different.
        item.Available = !value;
        Assert.False(item.Visible);
        Assert.Equal(!value, item.Available);
        Assert.False(item.Selected);
    }

    [WinFormsTheory]
    [BoolData]
    public void ToolStripItem_SetVisibleCore_InvokeSelected_GetReturnsExpected(bool value)
    {
        using SubToolStripItem item = new();
        item.Select();
        Assert.True(item.Selected);

        item.SetVisibleCore(value);
        Assert.False(item.Visible);
        Assert.Equal(value, item.Available);
        Assert.Equal(value, item.Selected);

        // Set same.
        item.SetVisibleCore(value);
        Assert.False(item.Visible);
        Assert.Equal(value, item.Available);
        Assert.Equal(value, item.Selected);

        // Set different.
        item.Available = !value;
        Assert.False(item.Visible);
        Assert.Equal(!value, item.Available);
        Assert.False(item.Selected);
    }

    [WinFormsTheory]
    [MemberData(nameof(SetVisibleCore_TestData))]
    public void ToolStripItem_SetVisibleCore_InvokeWithOwner_GetReturnsExpected(bool enabled, Image image, bool value)
    {
        using ToolStrip owner = new();
        using SubToolStripItem item = new()
        {
            Enabled = enabled,
            Image = image,
            Owner = owner
        };

        item.SetVisibleCore(value);
        Assert.False(item.Visible);
        Assert.Equal(value, item.Available);
        Assert.False(item.Selected);
        Assert.False(owner.IsHandleCreated);

        // Set same.
        item.SetVisibleCore(value);
        Assert.False(item.Visible);
        Assert.Equal(value, item.Available);
        Assert.False(item.Selected);
        Assert.False(owner.IsHandleCreated);

        // Set different.
        item.Available = !value;
        Assert.False(item.Visible);
        Assert.Equal(!value, item.Available);
        Assert.False(item.Selected);
        Assert.False(owner.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(SetVisibleCore_TestData))]
    public void ToolStripItem_SetVisibleCore_InvokeDesignModeWithOwner_GetReturnsExpected(bool enabled, Image image, bool value)
    {
        Mock<ISite> mockSite = new(MockBehavior.Strict);
        mockSite
            .Setup(s => s.Name)
            .Returns("Name");
        mockSite
            .Setup(s => s.DesignMode)
            .Returns(true);
        mockSite
            .Setup(s => s.Container)
            .Returns((IContainer)null);
        using ToolStrip owner = new();
        using SubToolStripItem item = new()
        {
            Enabled = enabled,
            Image = image,
            Site = mockSite.Object,
            Owner = owner,
        };

        item.SetVisibleCore(value);
        Assert.False(item.Visible);
        Assert.Equal(value, item.Available);
        Assert.False(item.Selected);
        Assert.False(owner.IsHandleCreated);

        // Set same.
        item.SetVisibleCore(value);
        Assert.False(item.Visible);
        Assert.Equal(value, item.Available);
        Assert.False(item.Selected);
        Assert.False(owner.IsHandleCreated);

        // Set different.
        item.Available = !value;
        Assert.False(item.Visible);
        Assert.Equal(!value, item.Available);
        Assert.False(item.Selected);
        Assert.False(owner.IsHandleCreated);
    }

    public static IEnumerable<object[]> SetVisibleCore_InvokeWithOwnerWithHandle_TestData()
    {
        foreach (bool enabled in new bool[] { true, false })
        {
            foreach (Image image in new Image[] { null, new Bitmap(10, 10) })
            {
                yield return new object[] { enabled, image, true, 0 };
                yield return new object[] { enabled, image, false, 2 };
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(SetVisibleCore_InvokeWithOwnerWithHandle_TestData))]
    public void ToolStripItem_SetVisibleCore_InvokeWithOwnerWithHandle_GetReturnsExpected(bool enabled, Image image, bool value, int expectedInvalidatedCallCount)
    {
        using ToolStrip owner = new();
        using SubToolStripItem item = new()
        {
            Enabled = enabled,
            Image = image,
            Owner = owner
        };
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        int invalidatedCallCount = 0;
        owner.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        owner.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        owner.HandleCreated += (sender, e) => createdCallCount++;

        item.SetVisibleCore(value);
        Assert.False(item.Visible);
        Assert.Equal(value, item.Available);
        Assert.False(item.Selected);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        item.SetVisibleCore(value);
        Assert.False(item.Visible);
        Assert.Equal(value, item.Available);
        Assert.False(item.Selected);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set different.
        item.Available = !value;
        Assert.Equal(!value, item.Visible);
        Assert.Equal(!value, item.Available);
        Assert.False(item.Selected);
        Assert.True(owner.IsHandleCreated);
        Assert.Equal(expectedInvalidatedCallCount + 2, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [MemberData(nameof(SetVisibleCore_TestData))]
    public void ToolStripItem_SetVisibleCore_InvokeWithParent_GetReturnsExpected(bool enabled, Image image, bool value)
    {
        using ToolStrip parent = new();
        using SubToolStripItem item = new()
        {
            Enabled = enabled,
            Image = image,
            Parent = parent
        };

        item.SetVisibleCore(value);
        Assert.Equal(value, item.Visible);
        Assert.Equal(value, item.Available);
        Assert.False(item.Selected);
        Assert.False(parent.IsHandleCreated);

        // Set same.
        item.SetVisibleCore(value);
        Assert.Equal(value, item.Visible);
        Assert.Equal(value, item.Available);
        Assert.False(item.Selected);
        Assert.False(parent.IsHandleCreated);

        // Set different.
        item.Available = !value;
        Assert.Equal(!value, item.Visible);
        Assert.Equal(!value, item.Available);
        Assert.False(item.Selected);
        Assert.False(parent.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(SetVisibleCore_TestData))]
    public void ToolStripItem_SetVisibleCore_InvokeDesignModeWithParent_GetReturnsExpected(bool enabled, Image image, bool value)
    {
        Mock<ISite> mockSite = new(MockBehavior.Strict);
        mockSite
            .Setup(s => s.Name)
            .Returns("Name");
        mockSite
            .Setup(s => s.DesignMode)
            .Returns(true);
        mockSite
            .Setup(s => s.Container)
            .Returns((IContainer)null);
        using ToolStrip parent = new();
        using SubToolStripItem item = new()
        {
            Enabled = enabled,
            Image = image,
            Site = mockSite.Object,
            Parent = parent
        };

        item.SetVisibleCore(value);
        Assert.Equal(value, item.Visible);
        Assert.Equal(value, item.Available);
        Assert.False(item.Selected);
        Assert.False(parent.IsHandleCreated);

        // Set same.
        item.SetVisibleCore(value);
        Assert.Equal(value, item.Visible);
        Assert.Equal(value, item.Available);
        Assert.False(item.Selected);
        Assert.False(parent.IsHandleCreated);

        // Set different.
        item.Available = !value;
        Assert.Equal(!value, item.Visible);
        Assert.Equal(!value, item.Available);
        Assert.False(item.Selected);
        Assert.False(parent.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(SetVisibleCore_TestData))]
    public void ToolStripItem_SetVisibleCore_InvokeWithParentWithHandle_GetReturnsExpected(bool enabled, Image image, bool value)
    {
        using ToolStrip parent = new();
        using SubToolStripItem item = new()
        {
            Enabled = enabled,
            Image = image,
            Parent = parent
        };
        Assert.NotEqual(IntPtr.Zero, parent.Handle);
        int invalidatedCallCount = 0;
        parent.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        parent.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        parent.HandleCreated += (sender, e) => createdCallCount++;

        item.SetVisibleCore(value);
        Assert.Equal(value, item.Visible);
        Assert.Equal(value, item.Available);
        Assert.False(item.Selected);
        Assert.True(parent.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        item.SetVisibleCore(value);
        Assert.Equal(value, item.Visible);
        Assert.Equal(value, item.Available);
        Assert.False(item.Selected);
        Assert.True(parent.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set different.
        item.Available = !value;
        Assert.Equal(!value, item.Visible);
        Assert.Equal(!value, item.Available);
        Assert.False(item.Selected);
        Assert.True(parent.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void ToolStripItem_SetVisibleCore_InvokeWithHandler_CallsAvailableChanged()
    {
        using SubToolStripItem item = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(item, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        item.AvailableChanged += handler;

        // Set different.
        item.Available = false;
        Assert.False(item.Available);
        Assert.Equal(1, callCount);

        // Set same.
        item.Available = false;
        Assert.False(item.Available);
        Assert.Equal(1, callCount);

        // Set different.
        item.Available = true;
        Assert.True(item.Available);
        Assert.Equal(2, callCount);

        // Remove handler.
        item.AvailableChanged -= handler;
        item.Available = false;
        Assert.False(item.Available);
        Assert.Equal(2, callCount);
    }

    [WinFormsFact]
    public void ToolStripItem_SetVisibleCore_InvokeWithHandler_CallsVisibleChanged()
    {
        using SubToolStripItem item = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(item, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        item.VisibleChanged += handler;

        // Set different.
        item.Available = false;
        Assert.False(item.Available);
        Assert.Equal(1, callCount);

        // Set same.
        item.Available = false;
        Assert.False(item.Available);
        Assert.Equal(1, callCount);

        // Set different.
        item.Available = true;
        Assert.True(item.Available);
        Assert.Equal(2, callCount);

        // Remove handler.
        item.VisibleChanged -= handler;
        item.Available = false;
        Assert.False(item.Available);
        Assert.Equal(2, callCount);
    }

    [WinFormsFact]
    public void ToolStripItem_ToString_InvokeWithoutText_ReturnsExpected()
    {
        using SubToolStripItem item = new();
        Assert.Equal("System.Windows.Forms.Tests.ToolStripItemTests+SubToolStripItem", item.ToString());
    }

    [WinFormsTheory]
    [InlineData(null, "System.Windows.Forms.Tests.ToolStripItemTests+SubToolStripItem")]
    [InlineData("", "System.Windows.Forms.Tests.ToolStripItemTests+SubToolStripItem")]
    [InlineData("text", "text")]
    public void ToolStripItem_ToString_InvokeWithText_ReturnsExpected(string text, string expected)
    {
        using SubToolStripItem item = new()
        {
            Text = text
        };
        Assert.Equal(expected, item.ToString());
    }

    [WinFormsFact]
    public void ToolStripItem_ToString_InvokeWithNullText_ReturnsExpected()
    {
        using NullTextToolStripItem item = new();
        Assert.Equal("System.Windows.Forms.Tests.ToolStripItemTests+NullTextToolStripItem", item.ToString());
    }

    private class NullTextToolStripItem : ToolStripItem
    {
        public override string Text
        {
            get => null;
            set { }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(DragEventArgs_TestData))]
    public void ToolStripItem_IDropTargetOnDragDrop_Invoke_CallsDragDrop(DragEventArgs eventArgs)
    {
        using SubToolStripItem item = new();
        IDropTarget dropTarget = item;
        int callCount = 0;
        DragEventHandler handler = (sender, e) =>
        {
            Assert.Same(item, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        item.DragDrop += handler;
        dropTarget.OnDragDrop(eventArgs);
        Assert.Equal(1, callCount);

        // Remove handler.
        item.DragDrop -= handler;
        dropTarget.OnDragDrop(eventArgs);
        Assert.Equal(1, callCount);
    }

    [WinFormsTheory]
    [MemberData(nameof(DragEventArgs_TestData))]
    public void ToolStripItem_IDropTargetOnDragEnter_Invoke_CallsDragEnter(DragEventArgs eventArgs)
    {
        using SubToolStripItem item = new();
        IDropTarget dropTarget = item;
        int callCount = 0;
        DragEventHandler handler = (sender, e) =>
        {
            Assert.Same(item, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        item.DragEnter += handler;
        dropTarget.OnDragEnter(eventArgs);
        Assert.Equal(1, callCount);

        // Remove handler.
        item.DragEnter -= handler;
        dropTarget.OnDragEnter(eventArgs);
        Assert.Equal(1, callCount);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void ToolStripItem_IDropTargetOnDragLeave_Invoke_CallsDragLeave(EventArgs eventArgs)
    {
        using SubToolStripItem item = new();
        IDropTarget dropTarget = item;
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(item, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        item.DragLeave += handler;
        dropTarget.OnDragLeave(eventArgs);
        Assert.Equal(1, callCount);

        // Remove handler.
        item.DragLeave -= handler;
        dropTarget.OnDragLeave(eventArgs);
        Assert.Equal(1, callCount);
    }

    [WinFormsTheory]
    [MemberData(nameof(DragEventArgs_TestData))]
    public void ToolStripItem_IDropTargetOnDragOver_Invoke_CallsDragOver(DragEventArgs eventArgs)
    {
        using SubToolStripItem item = new();
        IDropTarget dropTarget = item;
        int callCount = 0;
        DragEventHandler handler = (sender, e) =>
        {
            Assert.Same(item, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        item.DragOver += handler;
        dropTarget.OnDragOver(eventArgs);
        Assert.Equal(1, callCount);

        // Remove handler.
        item.DragOver -= handler;
        dropTarget.OnDragOver(eventArgs);
        Assert.Equal(1, callCount);
    }

    // Unit test for https://github.com/dotnet/winforms/issues/8548
    [WinFormsFact]
    public void ToolStripItem_OnItemSelectedChanged()
    {
        using MyMenuStrip menuStrip1 = new();
        using ToolStripMenuItem toolStripMenuItem1 = new();
        using ToolStripMenuItem toolStripMenuItem2 = new();
        using ToolStripMenuItem toolStripMenuItem3 = new();

        menuStrip1.Size = new Size(100, 50);
        toolStripMenuItem1.Size = new Size(10, 30);
        toolStripMenuItem2.Size = new Size(15, 30);
        toolStripMenuItem3.Size = new Size(15, 30);

        int callBackInvokedCount = 0;

        toolStripMenuItem2.SelectedChanged += (e, s) =>
        {
            callBackInvokedCount++;
        };

        menuStrip1.Items.AddRange(new ToolStripMenuItem[] { toolStripMenuItem1, toolStripMenuItem2, toolStripMenuItem3 });

        menuStrip1.CreateControl();

        // try to emulate mouse move event.
        for (int i = 0; i < 10; i++)
        {
            menuStrip1.MoveMouse(new MouseEventArgs(MouseButtons.None, 0, new Point(i, 5)));
        }

        Assert.Equal(0, callBackInvokedCount);

        for (int i = 10; i < 100; i++)
        {
            menuStrip1.MoveMouse(new MouseEventArgs(MouseButtons.None, 0, new Point(i, 5)));
        }

        // SelectedChanged event should be fired once in one round.

        Assert.Equal(1, callBackInvokedCount);
    }

    private class MyMenuStrip : MenuStrip
    {
        public void MoveMouse(MouseEventArgs mea)
        {
            OnMouseMove(mea);
        }
    }

    private class SubToolStrip : ToolStrip
    {
        public new void OnBeginDrag(EventArgs e) => base.OnBeginDrag(e);

        public new void OnMouseDown(MouseEventArgs mea) => base.OnMouseDown(mea);
    }

    private class FlippingEnabledToolStripItem : ToolStripItem
    {
        public FlippingEnabledToolStripItem() : base()
        {
        }

        private int _callCount;

        public override bool Enabled
        {
            get
            {
                if (_callCount <= 1)
                {
                    _callCount++;
                    return true;
                }

                return false;
            }
            set { }
        }
    }

    private class CannotSelectToolStripItem : ToolStripItem
    {
        public override bool CanSelect => false;
    }

    private class SubToolStripButton : ToolStripButton
    {
        public new bool ProcessDialogKey(Keys keyData) => base.ProcessDialogKey(keyData);
    }

    private class SubToolStripItem : ToolStripItem
    {
        public SubToolStripItem() : base()
        {
        }

        public SubToolStripItem(string text, Image image, EventHandler onClick) : base(text, image, onClick)
        {
        }

        public SubToolStripItem(string text, Image image, EventHandler onClick, string name) : base(text, image, onClick, name)
        {
        }

        public new bool CanRaiseEvents => base.CanRaiseEvents;

        public new bool DefaultAutoToolTip => base.DefaultAutoToolTip;

        public new ToolStripItemDisplayStyle DefaultDisplayStyle => base.DefaultDisplayStyle;

        public new Padding DefaultMargin => base.DefaultMargin;

        public new Padding DefaultPadding => base.DefaultPadding;

        public new Size DefaultSize => base.DefaultSize;

        public new bool DesignMode => base.DesignMode;

        public new bool DismissWhenClicked => base.DismissWhenClicked;

        public new EventHandlerList Events => base.Events;

        public new ToolStrip Parent
        {
            get => base.Parent;
            set => base.Parent = value;
        }

        public new bool ShowKeyboardCues => base.ShowKeyboardCues;

        public new AccessibleObject CreateAccessibilityInstance() => base.CreateAccessibilityInstance();

        public new void Dispose(bool disposing) => base.Dispose(disposing);

        public new bool IsInputChar(char charCode) => base.IsInputChar(charCode);

        public new bool IsInputKey(Keys keyData) => base.IsInputKey(keyData);

        public new void OnAvailableChanged(EventArgs e) => base.OnAvailableChanged(e);

        public new void OnBackColorChanged(EventArgs e) => base.OnBackColorChanged(e);

        public new void OnBoundsChanged() => base.OnBoundsChanged();

        public new void OnClick(EventArgs e) => base.OnClick(e);

        public new void OnDisplayStyleChanged(EventArgs e) => base.OnDisplayStyleChanged(e);

        public new void OnDoubleClick(EventArgs e) => base.OnDoubleClick(e);

        public new void OnDragDrop(DragEventArgs e) => base.OnDragDrop(e);

        public new void OnDragEnter(DragEventArgs e) => base.OnDragEnter(e);

        public new void OnDragLeave(EventArgs e) => base.OnDragLeave(e);

        public new void OnDragOver(DragEventArgs e) => base.OnDragOver(e);

        public new void OnEnabledChanged(EventArgs e) => base.OnEnabledChanged(e);

        public new void OnFontChanged(EventArgs e) => base.OnFontChanged(e);

        public new void OnForeColorChanged(EventArgs e) => base.OnForeColorChanged(e);

        public new void OnGiveFeedback(GiveFeedbackEventArgs giveFeedbackEvent) => base.OnGiveFeedback(giveFeedbackEvent);

        public new void OnLayout(LayoutEventArgs e) => base.OnLayout(e);

        public new void OnLocationChanged(EventArgs e) => base.OnLocationChanged(e);

        public new void OnMouseDown(MouseEventArgs e) => base.OnMouseDown(e);

        public new void OnMouseEnter(EventArgs e) => base.OnMouseEnter(e);

        public new void OnMouseHover(EventArgs e) => base.OnMouseHover(e);

        public new void OnMouseLeave(EventArgs e) => base.OnMouseLeave(e);

        public new void OnMouseMove(MouseEventArgs e) => base.OnMouseMove(e);

        public new void OnMouseUp(MouseEventArgs e) => base.OnMouseUp(e);

        public new void OnOwnerChanged(EventArgs e) => base.OnOwnerChanged(e);

        public new void OnOwnerFontChanged(EventArgs e) => base.OnOwnerFontChanged(e);

        public new void OnPaint(PaintEventArgs e) => base.OnPaint(e);

        public new void OnParentBackColorChanged(EventArgs e) => base.OnParentBackColorChanged(e);

        public new void OnParentChanged(ToolStrip oldParent, ToolStrip newParent) => base.OnParentChanged(oldParent, newParent);

        public new void OnParentEnabledChanged(EventArgs e) => base.OnParentEnabledChanged(e);

        public new void OnParentForeColorChanged(EventArgs e) => base.OnParentForeColorChanged(e);

        public new void OnParentRightToLeftChanged(EventArgs e) => base.OnParentRightToLeftChanged(e);

        public new void OnQueryContinueDrag(QueryContinueDragEventArgs e) => base.OnQueryContinueDrag(e);

        public new void OnRightToLeftChanged(EventArgs e) => base.OnRightToLeftChanged(e);

        public new void OnTextChanged(EventArgs e) => base.OnTextChanged(e);

        public new void OnVisibleChanged(EventArgs e) => base.OnVisibleChanged(e);

        public new bool ProcessCmdKey(ref Message m, Keys keyData) => base.ProcessCmdKey(ref m, keyData);

        public new bool ProcessDialogKey(Keys keyData) => base.ProcessDialogKey(keyData);

        public new bool ProcessMnemonic(char charCode) => base.ProcessMnemonic(charCode);

        public new void SetBounds(Rectangle bounds) => base.SetBounds(bounds);

        public new void SetVisibleCore(bool visible) => base.SetVisibleCore(visible);
    }

    private class ToolStripWithDisconnectCount : ToolStrip
    {
        public ToolStripWithDisconnectCount() : base() { }

        public int Disconnects { get; private set; }

        internal new void ReleaseUiaProvider(HWND handle)
        {
            base.ReleaseUiaProvider(handle);

            Disconnects++;
        }
    }

    private class ToolStripDropDownItemWithAccessibleObjectFieldAccessor : ToolStripDropDownItem
    {
        public ToolStripDropDownItemWithAccessibleObjectFieldAccessor() : base() { }

        public bool IsAccessibleObjectCleared()
        {
            var key = this.TestAccessor().Dynamic.s_accessibilityProperty;
            return !Properties.ContainsKey(key);
        }
    }
}
