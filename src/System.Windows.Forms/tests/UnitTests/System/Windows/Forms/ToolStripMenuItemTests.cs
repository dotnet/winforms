// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms.Tests;

public class ToolStripMenuItemTests
{
    [WinFormsFact]
    public void ToolStripMenuItem_Ctor_Default()
    {
        using var item = new SubToolStripMenuItem();
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
        Assert.Equal(new Rectangle(0, 0, 32, 19), item.Bounds);
        Assert.True(item.CanSelect);
        Assert.True(item.CanRaiseEvents);
        Assert.Null(item.Container);
        Assert.Equal(new Rectangle(-2, 2, 36, 15), item.ContentRectangle);
        Assert.False(item.DefaultAutoToolTip);
        Assert.Equal(ToolStripItemDisplayStyle.ImageAndText, item.DefaultDisplayStyle);
        Assert.Equal(new Padding(0, 0, 0, 0), item.DefaultMargin);
        Assert.Equal(new Padding(4, 0, 4, 0), item.DefaultPadding);
        Assert.Equal(new Size(32, 19), item.DefaultSize);
        Assert.False(item.DesignMode);
        Assert.True(item.DismissWhenClicked);
        Assert.Equal(ToolStripItemDisplayStyle.ImageAndText, item.DisplayStyle);
        Assert.Equal(DockStyle.None, item.Dock);
        Assert.False(item.DoubleClickEnabled);
        Assert.Equal(ToolStripDropDownDirection.Default, item.DropDownDirection);
        Assert.Equal(Point.Empty, item.DropDownLocation);
        Assert.False(item.Checked);
        Assert.False(item.CheckOnClick);
        Assert.Equal(CheckState.Unchecked, item.CheckState);
        Assert.True(item.Enabled);
        Assert.NotNull(item.Events);
        Assert.Same(item.Events, item.Events);
        Assert.NotNull(item.Font);
        Assert.NotSame(Control.DefaultFont, item.Font);
        Assert.Same(item.Font, item.Font);
        Assert.Equal(Control.DefaultForeColor, item.ForeColor);
        Assert.False(item.HasDropDownItems);
        Assert.False(item.HasDropDown);
        Assert.Equal(19, item.Height);
        Assert.Null(item.Image);
        Assert.Equal(ContentAlignment.MiddleCenter, item.ImageAlign);
        Assert.Equal(-1, item.ImageIndex);
        Assert.Empty(item.ImageKey);
        Assert.Equal(ToolStripItemImageScaling.SizeToFit, item.ImageScaling);
        Assert.Equal(Color.Empty, item.ImageTransparentColor);
        Assert.False(item.IsDisposed);
        Assert.False(item.IsMdiWindowListEntry);
        Assert.False(item.IsOnDropDown);
        Assert.False(item.IsOnOverflow);
        Assert.True(item.IsTopLevel);
        Assert.Equal(Padding.Empty, item.Margin);
        Assert.Null(item.MdiForm);
        Assert.Equal(MergeAction.Append, item.MergeAction);
        Assert.Equal(-1, item.MergeIndex);
        Assert.Empty(item.Name);
        Assert.Equal(ToolStripItemOverflow.Never, item.Overflow);
        Assert.Null(item.OwnerItem);
        Assert.Equal(new Padding(4, 0, 4, 0), item.Padding);
        Assert.Null(item.Parent);
        Assert.Equal(ToolStripItemPlacement.None, item.Placement);
        Assert.False(item.Pressed);
        Assert.Equal(RightToLeft.Inherit, item.RightToLeft);
        Assert.False(item.RightToLeftAutoMirrorImage);
        Assert.False(item.Selected);
        Assert.Equal(SystemInformation.MenuAccessKeysUnderlined, item.ShowKeyboardCues);
        Assert.Equal(Keys.None, item.ShortcutKeys);
        Assert.True(item.ShowShortcutKeys);
        Assert.Null(item.Site);
        Assert.Equal(new Size(32, 19), item.Size);
        Assert.Null(item.Tag);
        Assert.Empty(item.Text);
        Assert.Equal(ContentAlignment.MiddleCenter, item.TextAlign);
        Assert.Equal(ToolStripTextDirection.Horizontal, item.TextDirection);
        Assert.Equal(TextImageRelation.ImageBeforeText, item.TextImageRelation);
        Assert.Null(item.ToolTipText);
        Assert.False(item.Visible);
        Assert.Equal(32, item.Width);
    }

    public static IEnumerable<object[]> GetNativeMenuItemImage_TestData()
    {
        yield return new object[] { PInvoke.SC_MINIMIZE };
        yield return new object[] { PInvoke.SC_CLOSE };
        yield return new object[] { PInvoke.SC_RESTORE };
    }

    [WinFormsTheory]
    [MemberData(nameof(GetNativeMenuItemImage_TestData))]
    public void ToolStripMenuItem_GetNativeMenuItemImage_ReturnsExpected(int nativeMenuCommandID)
    {
        using Form form = new();
        form.CreateControl();
        HMENU hmenu = PInvoke.GetSystemMenu(form, bRevert: false);
        using SubToolStripMenuItem menuItem = new(hmenu, nativeMenuCommandID, form);

        using Bitmap bitmap = menuItem.TestAccessor().Dynamic.GetNativeMenuItemImage();
        Assert.NotNull(bitmap);
    }

    [WinFormsTheory]
    [InlineData(Keys.F1)]
    [InlineData(Keys.None)]
    [InlineData(Keys.Control | Keys.Add)]
    [InlineData(Keys.Control | Keys.Alt | Keys.D)]
    [InlineData(Keys.Control | Keys.Alt | Keys.Shift | Keys.A)]
    [InlineData(Keys.Control | Keys.Alt | Keys.Shift | Keys.F1)]
    public void ToolStripMenuItem_SetShortcutKeys(Keys keys)
    {
        using var item = new SubToolStripMenuItem();
        item.ShortcutKeys = keys;
        Assert.Equal(keys, item.ShortcutKeys);
    }

    [WinFormsTheory]
    [InlineData(Keys.A)]
    [InlineData(Keys.Control)]
    [InlineData(Keys.Control | Keys.Alt)]
    [InlineData(Keys.Control | Keys.Alt | Keys.Shift)]
    public void ToolStripMenuItem_SetShortcutKeys_ThrowsInvalidEnumArgumentException(Keys keys)
    {
        using var item = new SubToolStripMenuItem();
        Assert.Throws<InvalidEnumArgumentException>(() => item.ShortcutKeys = keys);
    }

    private class SubToolStripMenuItem : ToolStripMenuItem
    {
        public SubToolStripMenuItem() : base()
        {
        }

        public SubToolStripMenuItem(string text, Image image, EventHandler onClick) : base(text, image, onClick)
        {
        }

        public SubToolStripMenuItem(string text, Image image, EventHandler onClick, string name) : base(text, image, onClick, name)
        {
        }

        public SubToolStripMenuItem(string text, Image image, params ToolStripItem[] dropDownItems) : base(text, image, dropDownItems)
        {
        }

        internal SubToolStripMenuItem(HMENU hmenu, int nativeMenuCommandId, IWin32Window targetWindow) : base(hmenu, nativeMenuCommandId, targetWindow)
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

        public new ToolStripDropDown CreateDefaultDropDown() => base.CreateDefaultDropDown();
    }
}
