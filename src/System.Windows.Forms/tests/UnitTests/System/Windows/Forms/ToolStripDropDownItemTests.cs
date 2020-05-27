// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using WinForms.Common.Tests;
using Xunit;

namespace System.Windows.Forms.Tests
{
    using Size = System.Drawing.Size;
    using Point = System.Drawing.Point;

    public class ToolStripItemDropDownTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void ToolStripDropDownItem_Ctor_Default()
        {
            using var item = new SubToolStripDropDownItem();
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
            Assert.Equal(ToolStripDropDownDirection.Default, item.DropDownDirection);
            Assert.Equal(Point.Empty, item.DropDownLocation);
            Assert.True(item.Enabled);
            Assert.NotNull(item.Events);
            Assert.Same(item.Events, item.Events);
            Assert.NotNull(item.Font);
            Assert.NotSame(Control.DefaultFont, item.Font);
            Assert.Same(item.Font, item.Font);
            Assert.Equal(Control.DefaultForeColor, item.ForeColor);
            Assert.False(item.HasDropDownItems);
            Assert.False(item.HasDropDown);
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
            Assert.False(item.ShowKeyboardCues);
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
        public void ToolStripDropDownItem_Ctor_String_Image_EventHandler(string text, Image image, EventHandler onClick)
        {
            using var item = new SubToolStripDropDownItem(text, image, onClick);
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
            Assert.Equal(ToolStripDropDownDirection.Default, item.DropDownDirection);
            Assert.Equal(Point.Empty, item.DropDownLocation);
            Assert.True(item.Enabled);
            Assert.NotNull(item.Events);
            Assert.Same(item.Events, item.Events);
            Assert.NotNull(item.Font);
            Assert.NotSame(Control.DefaultFont, item.Font);
            Assert.Same(item.Font, item.Font);
            Assert.Equal(Control.DefaultForeColor, item.ForeColor);
            Assert.False(item.HasDropDownItems);
            Assert.False(item.HasDropDown);
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
            Assert.False(item.ShowKeyboardCues);
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
        public void ToolStripDropDownItem_Ctor_String_Image_EventHandler_InvokeClick_CallsOnClick()
        {
            int callCount = 0;
            EventHandler onClick = (sender, e) => callCount++;
            using var item = new SubToolStripDropDownItem("text", null, onClick);
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
        public void ToolStripDropDownItem_Ctor_String_Image_EventHandler_String(string text, Image image, EventHandler onClick, string name, string expectedName)
        {
            using var item = new SubToolStripDropDownItem(text, image, onClick, name);
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
            Assert.Equal(ToolStripDropDownDirection.Default, item.DropDownDirection);
            Assert.Equal(Point.Empty, item.DropDownLocation);
            Assert.True(item.Enabled);
            Assert.NotNull(item.Events);
            Assert.Same(item.Events, item.Events);
            Assert.NotNull(item.Font);
            Assert.NotSame(Control.DefaultFont, item.Font);
            Assert.Same(item.Font, item.Font);
            Assert.Equal(Control.DefaultForeColor, item.ForeColor);
            Assert.False(item.HasDropDownItems);
            Assert.False(item.HasDropDown);
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
            Assert.False(item.ShowKeyboardCues);
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
        public void ToolStripDropDownItem_Ctor_String_Image_EventHandler_String_InvokeClick_CallsOnClick()
        {
            int callCount = 0;
            EventHandler onClick = (sender, e) => callCount++;
            using var item = new SubToolStripDropDownItem("text", null, onClick, "name");
            item.PerformClick();
            Assert.Equal(1, callCount);
        }

        public static IEnumerable<object[]> Ctor_String_Image_ToolStripItemArray_TestData()
        {
            yield return new object[] { null, null, null, Array.Empty<ToolStripItem>() };
            yield return new object[] { string.Empty, new Bitmap(10, 10), Array.Empty<ToolStripItem>(), Array.Empty<ToolStripItem>() };

            var items = new ToolStripItem[] { new SubToolStripItem() };
            yield return new object[] { "text", new Bitmap(10, 10), items, items };
        }

        [WinFormsTheory]
        [MemberData(nameof(Ctor_String_Image_ToolStripItemArray_TestData))]
        public void ToolStripDropDownItem_Ctor_String_Image_ToolStripItemArray(string text, Image image, ToolStripItem[] dropDownItems, ToolStripItem[] expectedDropDownItems)
        {
            using var item = new SubToolStripDropDownItem(text, image, dropDownItems);
            Assert.Equal(dropDownItems != null, item.HasDropDown);
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
            Assert.NotNull(item.DropDown);
            Assert.Same(item.DropDown, item.DropDown);
            Assert.True(item.DropDown.IsAutoGenerated);
            Assert.True(item.DropDown.ShowItemToolTips);
            Assert.Same(item, item.DropDown.OwnerItem);
            Assert.Equal(ToolStripDropDownDirection.Default, item.DropDownDirection);
            Assert.Equal(expectedDropDownItems, item.DropDownItems.Cast<ToolStripItem>());
            Assert.Same(item.DropDownItems, item.DropDownItems);
            Assert.True(item.HasDropDown);
            Assert.Equal(Point.Empty, item.DropDownLocation);
            Assert.True(item.Enabled);
            Assert.NotNull(item.Events);
            Assert.Same(item.Events, item.Events);
            Assert.NotNull(item.Font);
            Assert.NotSame(Control.DefaultFont, item.Font);
            Assert.Same(item.Font, item.Font);
            Assert.Equal(Control.DefaultForeColor, item.ForeColor);
            Assert.Equal(expectedDropDownItems.Length > 0, item.HasDropDownItems);
            Assert.True(item.HasDropDown);
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
            Assert.False(item.ShowKeyboardCues);
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
        public void ToolStripDropDownItem_DropDown_Get_ReturnsExpected()
        {
            using var item = new SubToolStripDropDownItem();
            Assert.NotNull(item.DropDown);
            Assert.Same(item.DropDown, item.DropDown);
            Assert.True(item.DropDown.IsAutoGenerated);
            Assert.True(item.DropDown.ShowItemToolTips);
            Assert.Same(item, item.DropDown.OwnerItem);
            Assert.True(item.HasDropDown);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ToolStripDropDownItem_DropDown_GetWithOwner_ReturnsExpected(bool showItemToolTips)
        {
            using var owner = new ToolStrip
            {
                ShowItemToolTips = showItemToolTips
            };
            using var item = new SubToolStripDropDownItem
            {
                Owner = owner
            };
            Assert.NotNull(item.DropDown);
            Assert.Same(item.DropDown, item.DropDown);
            Assert.True(item.DropDown.IsAutoGenerated);
            Assert.True(item.DropDown.ShowItemToolTips);
            Assert.Same(item, item.DropDown.OwnerItem);
            Assert.True(item.HasDropDown);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ToolStripDropDownItem_DropDown_GetWithParent_ReturnsExpected(bool showItemToolTips)
        {
            using var parent = new ToolStrip
            {
                ShowItemToolTips = showItemToolTips
            };
            using var item = new SubToolStripDropDownItem
            {
                Parent = parent
            };
            Assert.NotNull(item.DropDown);
            Assert.Same(item.DropDown, item.DropDown);
            Assert.True(item.DropDown.IsAutoGenerated);
            Assert.Equal(showItemToolTips, item.DropDown.ShowItemToolTips);
            Assert.Same(item, item.DropDown.OwnerItem);
            Assert.True(item.HasDropDown);
        }

        [WinFormsFact]
        public void ToolStripDropDownItem_DropDown_GetCustomCreateDefaultDropDown_ReturnsExpected()
        {
            using var result = new ToolStripDropDown();
            using var item = new CustomCreateDefaultDropDownToolStripDropDownItem
            {
                CreateDefaultDropDownResult = result
            };
            Assert.NotNull(item.DropDown);
            Assert.Same(item.DropDown, item.DropDown);
            Assert.Same(result, item.DropDown);
            Assert.True(result.IsAutoGenerated);
            Assert.True(result.ShowItemToolTips);
            Assert.Null(item.DropDown.OwnerItem);
            Assert.True(item.HasDropDown);
        }

        [WinFormsFact]
        public void ToolStripDropDownItem_DropDown_GetNullCreateDefaultDropDown_ReturnsExpected()
        {
            using var item = new CustomCreateDefaultDropDownToolStripDropDownItem
            {
                CreateDefaultDropDownResult = null
            };
            Assert.Throws<NullReferenceException>(() => item.DropDown);
            Assert.Throws<NullReferenceException>(() => item.DropDown);
            Assert.False(item.HasDropDown);
        }

        private class CustomCreateDefaultDropDownToolStripDropDownItem : ToolStripDropDownItem
        {
            public ToolStripDropDown CreateDefaultDropDownResult { get; set; }

            protected override ToolStripDropDown CreateDefaultDropDown() => CreateDefaultDropDownResult;
        }

        [WinFormsFact]
        public void ToolStripDropDownItem_DropDownItems_Get_ReturnsExpected()
        {
            using var item = new SubToolStripDropDownItem();
            Assert.Empty(item.DropDownItems);
            Assert.Same(item.DropDownItems, item.DropDownItems);
            Assert.True(item.HasDropDown);
        }

        private class SubToolStripItem : ToolStripItem
        {
        }

        private class SubToolStripDropDownItem : ToolStripDropDownItem
        {
            public SubToolStripDropDownItem() : base()
            {
            }

            public SubToolStripDropDownItem(string text, Image image, EventHandler onClick) : base(text, image, onClick)
            {
            }

            public SubToolStripDropDownItem(string text, Image image, EventHandler onClick, string name) : base(text, image, onClick, name)
            {
            }

            public SubToolStripDropDownItem(string text, Image image, params ToolStripItem[] dropDownItems) : base(text, image, dropDownItems)
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
}
