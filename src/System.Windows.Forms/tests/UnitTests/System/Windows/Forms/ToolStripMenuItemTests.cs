// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.ComponentModel;
using System.Drawing;
using System.Globalization;

namespace System.Windows.Forms.Tests;

public class ToolStripMenuItemTests
{
    [WinFormsFact]
    public void ToolStripMenuItem_Ctor_Default()
    {
        using SubToolStripMenuItem item = new();
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
        Assert.Empty(item.Name ?? string.Empty);
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
        Assert.Empty(item.Text ?? string.Empty);
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
        using SubToolStripMenuItem item = new();
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
        using SubToolStripMenuItem item = new();
        Assert.Throws<InvalidEnumArgumentException>(() => item.ShortcutKeys = keys);
    }

    [WinFormsTheory]
    [MemberData(nameof(CultureInfo_Shortcut_TestData))]
    public void ToolStripMenuItem_SetShortcutKeys_ReturnExpectedShortcutText(CultureInfo threadCulture, CultureInfo threadUICulture, string expectedShortcutText)
    {
        CultureInfo uiCulture = Thread.CurrentThread.CurrentUICulture;
        CultureInfo curCulture = Thread.CurrentThread.CurrentCulture;

        Thread.CurrentThread.CurrentUICulture = threadUICulture;
        Thread.CurrentThread.CurrentCulture = threadCulture;
        using SubToolStripMenuItem item = new();
        item.ShortcutKeys = Keys.Control | Keys.Shift | Keys.K;
        Assert.Equal(expectedShortcutText, item.GetShortcutText());

        Thread.CurrentThread.CurrentUICulture = uiCulture;
        Thread.CurrentThread.CurrentCulture = curCulture;
    }

    public static IEnumerable<object[]> CultureInfo_Shortcut_TestData()
    {
        yield return new object[] { new CultureInfo("en-US"), new CultureInfo("en-US"), "Ctrl+Shift+K" };
        yield return new object[] { new CultureInfo("fr-FR"), new CultureInfo("en-US"), "Ctrl+Shift+K" };
        yield return new object[] { new CultureInfo("zh-CN"), new CultureInfo("en-US"), "Ctrl+Shift+K" };
        yield return new object[] { new CultureInfo("de-DE"), new CultureInfo("en-US"), "Ctrl+Shift+K" };

        yield return new object[] { new CultureInfo("en-US"), new CultureInfo("zh-CN"), "Ctrl+Shift+K" };
        yield return new object[] { new CultureInfo("fr-FR"), new CultureInfo("zh-CN"), "Ctrl+Shift+K" };
        yield return new object[] { new CultureInfo("zh-CN"), new CultureInfo("zh-CN"), "Ctrl+Shift+K" };
        yield return new object[] { new CultureInfo("de-DE"), new CultureInfo("zh-CN"), "Ctrl+Shift+K" };

        yield return new object[] { new CultureInfo("en-US"), new CultureInfo("fr-FR"), "Ctrl+Majuscule+K" };
        yield return new object[] { new CultureInfo("fr-FR"), new CultureInfo("fr-FR"), "Ctrl+Majuscule+K" };
        yield return new object[] { new CultureInfo("zh-CN"), new CultureInfo("fr-FR"), "Ctrl+Majuscule+K" };
        yield return new object[] { new CultureInfo("de-DE"), new CultureInfo("fr-FR"), "Ctrl+Majuscule+K" };

        yield return new object[] { new CultureInfo("en-US"), new CultureInfo("de-DE"), "Strg+Umschalttaste+K" };
        yield return new object[] { new CultureInfo("fr-FR"), new CultureInfo("de-DE"), "Strg+Umschalttaste+K" };
        yield return new object[] { new CultureInfo("zh-CN"), new CultureInfo("de-DE"), "Strg+Umschalttaste+K" };
        yield return new object[] { new CultureInfo("de-DE"), new CultureInfo("de-DE"), "Strg+Umschalttaste+K" };
    }

    [WinFormsFact]
    public void ToolStripMenuItem_Ctor_ShouldInitializeCorrectly()
    {
        using Bitmap image = new(10, 10);
        string text = "Test Item";

        using ToolStripMenuItem itemWithImage = new(image);
        itemWithImage.Image.Should().Be(image);
        itemWithImage.Text.Should().BeNull();

        using ToolStripMenuItem itemWithTextAndImage = new(text, image);
        itemWithTextAndImage.Text.Should().Be(text);
        itemWithTextAndImage.Image.Should().Be(image);
    }

    [WinFormsFact]
    public void ToolStripMenuItem_Ctor_WithTextImageOnClickName_ShouldInitializeCorrectly()
    {
        using Bitmap image = new(10, 10);
        string text = "Test Item";
        string name = "TestName";
        bool wasClicked = false;
        EventHandler onClick = (sender, e) => wasClicked = true;

        using ToolStripMenuItem item = new(text, image, onClick, name);

        item.Text.Should().Be(text);
        item.Image.Should().Be(image);
        item.Name.Should().Be(name);
        item.TestAccessor().Dynamic.OnClick(null);
        wasClicked.Should().BeTrue();
    }

    [WinFormsFact]
    public void ToolStripMenuItem_Ctor_WithTextImageOnClickShortcutKeys_ShouldInitializeCorrectly()
    {
        using Bitmap image = new(10, 10);
        string text = "Test Item";
        bool wasClicked = false;
        EventHandler onClick = (sender, e) => wasClicked = true;
        Keys shortcutKeys = Keys.Control | Keys.A;

        using ToolStripMenuItem item = new(text, image, onClick, shortcutKeys);

        item.Text.Should().Be(text);
        item.Image.Should().Be(image);
        item.TestAccessor().Dynamic.OnClick(null);
        wasClicked.Should().BeTrue();
        item.ShortcutKeys.Should().Be(shortcutKeys);
    }

    [WinFormsFact]
    public void ToolStripMenuItem_Ctor_TextImageDropDownItems_ShouldInitializeCorrectly()
    {
        using Bitmap image = new(10, 10);
        string text = "Test Item";
        ToolStripItem[] dropDownItems = [new ToolStripMenuItem("SubItem1"), new ToolStripMenuItem("SubItem2")];

        using ToolStripMenuItem item = new(text, image, dropDownItems);

        item.Text.Should().Be(text);
        item.Image.Should().Be(image);
        item.DropDownItems.Count.Should().Be(2);
        item.DropDownItems[0].Text.Should().Be("SubItem1");
        item.DropDownItems[1].Text.Should().Be("SubItem2");
    }

    [WinFormsFact]
    public void ToolStripMenuItem_MdiForm_ShouldReturnExpected()
    {
        Form? mdiForm1 = null;
        using ToolStripMenuItem itemWithoutMdiForm = new(mdiForm1!);

        itemWithoutMdiForm.MdiForm.Should().BeNull();
    }

    [WinFormsFact]
    public void ToolStripMenuItem_Clone_ShouldReturnExpected()
    {
        using ToolStripMenuItem original = new()
        {
            AccessibleName = "AccessibleName",
            AccessibleRole = AccessibleRole.MenuItem,
            Alignment = ToolStripItemAlignment.Right,
            AllowDrop = true,
            Anchor = AnchorStyles.Bottom,
            AutoSize = false,
            AutoToolTip = true,
            BackColor = Color.Red,
            BackgroundImage = new Bitmap(10, 10),
            BackgroundImageLayout = ImageLayout.Center,
            Checked = true,
            CheckOnClick = true,
            CheckState = CheckState.Checked,
            DisplayStyle = ToolStripItemDisplayStyle.Image,
            Dock = DockStyle.Bottom,
            DoubleClickEnabled = true,
            Enabled = false,
            Font = new Font("Arial", 12),
            ForeColor = Color.Blue,
            Image = new Bitmap(10, 10),
            ImageAlign = ContentAlignment.BottomCenter,
            ImageScaling = ToolStripItemImageScaling.None,
            ImageTransparentColor = Color.Green,
            Margin = new Padding(1),
            MergeAction = MergeAction.Append,
            MergeIndex = 1,
            Name = "Name",
            Overflow = ToolStripItemOverflow.Always,
            Padding = new Padding(2),
            RightToLeft = RightToLeft.Yes,
            ShortcutKeys = Keys.Control | Keys.A,
            ShowShortcutKeys = false,
            Tag = "Tag",
            Text = "Text",
            TextAlign = ContentAlignment.BottomRight,
            TextDirection = ToolStripTextDirection.Vertical90,
            TextImageRelation = TextImageRelation.ImageAboveText,
            ToolTipText = "ToolTipText",
            Visible = true,
            Size = new Size(100, 50)
        };

        using ToolStripMenuItem clone = original.Clone();

        clone.Should().NotBeSameAs(original);
        clone.AccessibleName.Should().Be(original.AccessibleName);
        clone.AccessibleRole.Should().Be(original.AccessibleRole);
        clone.Alignment.Should().Be(original.Alignment);
        clone.AllowDrop.Should().Be(original.AllowDrop);
        clone.Anchor.Should().Be(original.Anchor);
        clone.AutoSize.Should().Be(original.AutoSize);
        clone.AutoToolTip.Should().Be(original.AutoToolTip);
        clone.BackColor.Should().Be(original.BackColor);
        clone.BackgroundImage.Should().Be(original.BackgroundImage);
        clone.BackgroundImageLayout.Should().Be(original.BackgroundImageLayout);
        clone.Checked.Should().Be(original.Checked);
        clone.CheckOnClick.Should().Be(original.CheckOnClick);
        clone.CheckState.Should().Be(original.CheckState);
        clone.DisplayStyle.Should().Be(original.DisplayStyle);
        clone.Dock.Should().Be(original.Dock);
        clone.DoubleClickEnabled.Should().Be(original.DoubleClickEnabled);
        clone.Enabled.Should().Be(original.Enabled);
        clone.Font.Should().Be(original.Font);
        clone.ForeColor.Should().Be(original.ForeColor);
        clone.Image.Should().Be(original.Image);
        clone.ImageAlign.Should().Be(original.ImageAlign);
        clone.ImageScaling.Should().Be(original.ImageScaling);
        clone.ImageTransparentColor.Should().Be(original.ImageTransparentColor);
        clone.Margin.Should().Be(original.Margin);
        clone.MergeAction.Should().Be(original.MergeAction);
        clone.MergeIndex.Should().Be(original.MergeIndex);
        clone.Name.Should().Be(original.Name);
        clone.Overflow.Should().Be(original.Overflow);
        clone.Padding.Should().Be(original.Padding);
        clone.RightToLeft.Should().Be(original.RightToLeft);
        clone.ShortcutKeys.Should().Be(original.ShortcutKeys);
        clone.ShowShortcutKeys.Should().Be(original.ShowShortcutKeys);
        clone.Tag.Should().Be(original.Tag);
        clone.Text.Should().Be(original.Text);
        clone.TextAlign.Should().Be(original.TextAlign);
        clone.TextDirection.Should().Be(original.TextDirection);
        clone.TextImageRelation.Should().Be(original.TextImageRelation);
        clone.ToolTipText.Should().Be(original.ToolTipText);
        clone.Visible.Should().Be(original.Visible);
        clone.Size.Should().Be(original.Size);
    }

    [WinFormsFact]
    public void ToolStripMenuItem_SetDeviceDpi_ShouldUpdateDpiAndDisposeImages()
    {
        using ToolStripMenuItem item = new();
        dynamic accessor = item.TestAccessor().Dynamic;

        accessor.DeviceDpi = 96;

        accessor.t_indeterminateCheckedImage = new Bitmap(10, 10);
        accessor.t_checkedImage = new Bitmap(10, 10);

        accessor.DeviceDpi = 120;

        ((object)accessor.t_indeterminateCheckedImage).Should().BeNull();
        ((object)accessor.t_checkedImage).Should().BeNull();
    }

    [WinFormsFact]
    public void ToolStripMenuItem_CheckedChanged_InvokeHandler()
    {
        using ToolStripMenuItem item = new();
        bool eventCalled = false;
        EventHandler handler = (sender, e) => { eventCalled = true; };

        item.CheckedChanged += handler;

        item.Checked = true;
        eventCalled.Should().BeTrue();

        eventCalled = false;
        item.Checked = false;
        eventCalled.Should().BeTrue();
    }

    [WinFormsFact]
    public void ToolStripMenuItem_CheckStateChanged_InvokeHandler()
    {
        using ToolStripMenuItem item = new();
        bool eventInvoked = false;
        EventHandler handler = (sender, e) => { eventInvoked = true; };

        item.CheckStateChanged += handler;

        item.CheckState = CheckState.Checked;
        eventInvoked.Should().BeTrue();

        eventInvoked = false;
        item.CheckState = CheckState.Unchecked;
        eventInvoked.Should().BeTrue();
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
            get => base.Parent!;
            set => base.Parent = value;
        }

        public new bool ShowKeyboardCues => base.ShowKeyboardCues;

        public new AccessibleObject CreateAccessibilityInstance() => base.CreateAccessibilityInstance();

        public new ToolStripDropDown CreateDefaultDropDown() => base.CreateDefaultDropDown();
    }
}
