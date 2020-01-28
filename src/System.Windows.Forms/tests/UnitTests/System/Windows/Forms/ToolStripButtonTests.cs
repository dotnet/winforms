// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using WinForms.Common.Tests;
using Xunit;

namespace System.Windows.Forms.Tests
{
    using Size = System.Drawing.Size;

    public class ToolStripButtonTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void ToolStripButton_Ctor_Default()
        {
            using var item = new SubToolStripButton();
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
            Assert.True(item.AutoToolTip);
            Assert.True(item.Available);
            Assert.Equal(Control.DefaultBackColor, item.BackColor);
            Assert.Null(item.BackgroundImage);
            Assert.Equal(ImageLayout.Tile, item.BackgroundImageLayout);
            Assert.Equal(new Rectangle(0, 0, 23, 23), item.Bounds);
            Assert.True(item.CanSelect);
            Assert.True(item.CanRaiseEvents);
            Assert.False(item.Checked);
            Assert.False(item.CheckOnClick);
            Assert.Equal(CheckState.Unchecked, item.CheckState);
            Assert.Null(item.Container);
            Assert.Equal(new Rectangle(2, 2, 19, 19), item.ContentRectangle);
            Assert.True(item.DefaultAutoToolTip);
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
            Assert.False(item.ShowKeyboardCues);
            Assert.Null(item.Site);
            Assert.Equal(new Size(23, 23), item.Size);
            Assert.Null(item.Tag);
            Assert.Empty(item.Text);
            Assert.Equal(ContentAlignment.MiddleCenter, item.TextAlign);
            Assert.Equal(ToolStripTextDirection.Horizontal, item.TextDirection);
            Assert.Equal(TextImageRelation.ImageBeforeText, item.TextImageRelation);
            Assert.Empty(item.ToolTipText);
            Assert.False(item.Visible);
            Assert.Equal(23, item.Width);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringWithNullTheoryData))]
        public void ToolStripButton_Ctor_String(string text)
        {
            using var item = new SubToolStripButton(text);
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
            Assert.True(item.AutoToolTip);
            Assert.True(item.Available);
            Assert.Equal(Control.DefaultBackColor, item.BackColor);
            Assert.Null(item.BackgroundImage);
            Assert.Equal(ImageLayout.Tile, item.BackgroundImageLayout);
            Assert.Equal(new Rectangle(0, 0, 23, 23), item.Bounds);
            Assert.True(item.CanSelect);
            Assert.True(item.CanRaiseEvents);
            Assert.False(item.Checked);
            Assert.False(item.CheckOnClick);
            Assert.Equal(CheckState.Unchecked, item.CheckState);
            Assert.Null(item.Container);
            Assert.Equal(new Rectangle(2, 2, 19, 19), item.ContentRectangle);
            Assert.True(item.DefaultAutoToolTip);
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
            Assert.False(item.ShowKeyboardCues);
            Assert.Null(item.Site);
            Assert.Equal(new Size(23, 23), item.Size);
            Assert.Null(item.Tag);
            Assert.Equal(text, item.Text);
            Assert.Equal(ContentAlignment.MiddleCenter, item.TextAlign);
            Assert.Equal(ToolStripTextDirection.Horizontal, item.TextDirection);
            Assert.Equal(TextImageRelation.ImageBeforeText, item.TextImageRelation);
            Assert.Equal(text, item.ToolTipText);
            Assert.False(item.Visible);
            Assert.Equal(23, item.Width);
        }

        public static IEnumerable<object[]> Ctor_Image_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new Bitmap(10, 10) };
        }

        [WinFormsTheory]
        [MemberData(nameof(Ctor_Image_TestData))]
        public void ToolStripButton_Ctor_Image(Image image)
        {
            using var item = new SubToolStripButton(image);
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
            Assert.True(item.AutoToolTip);
            Assert.True(item.Available);
            Assert.Equal(Control.DefaultBackColor, item.BackColor);
            Assert.Null(item.BackgroundImage);
            Assert.Equal(ImageLayout.Tile, item.BackgroundImageLayout);
            Assert.Equal(new Rectangle(0, 0, 23, 23), item.Bounds);
            Assert.True(item.CanSelect);
            Assert.True(item.CanRaiseEvents);
            Assert.False(item.Checked);
            Assert.False(item.CheckOnClick);
            Assert.Equal(CheckState.Unchecked, item.CheckState);
            Assert.Null(item.Container);
            Assert.Equal(new Rectangle(2, 2, 19, 19), item.ContentRectangle);
            Assert.True(item.DefaultAutoToolTip);
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
            Assert.Same(image, item.Image);
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
            Assert.Null(item.Text);
            Assert.Equal(ContentAlignment.MiddleCenter, item.TextAlign);
            Assert.Equal(ToolStripTextDirection.Horizontal, item.TextDirection);
            Assert.Equal(TextImageRelation.ImageBeforeText, item.TextImageRelation);
            Assert.Null(item.ToolTipText);
            Assert.False(item.Visible);
            Assert.Equal(23, item.Width);
        }

        public static IEnumerable<object[]> Ctor_String_Image_TestData()
        {
            foreach (string text in new string[] { null, string.Empty, "text" })
            {
                yield return new object[] { text, null };
                yield return new object[] { text, new Bitmap(10, 10) };
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(Ctor_String_Image_TestData))]
        public void ToolStripButton_Ctor_String_Image(string text, Image image)
        {
            using var item = new SubToolStripButton(text, image);
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
            Assert.True(item.AutoToolTip);
            Assert.True(item.Available);
            Assert.Equal(Control.DefaultBackColor, item.BackColor);
            Assert.Null(item.BackgroundImage);
            Assert.Equal(ImageLayout.Tile, item.BackgroundImageLayout);
            Assert.Equal(new Rectangle(0, 0, 23, 23), item.Bounds);
            Assert.True(item.CanSelect);
            Assert.True(item.CanRaiseEvents);
            Assert.False(item.Checked);
            Assert.False(item.CheckOnClick);
            Assert.Equal(CheckState.Unchecked, item.CheckState);
            Assert.Null(item.Container);
            Assert.Equal(new Rectangle(2, 2, 19, 19), item.ContentRectangle);
            Assert.True(item.DefaultAutoToolTip);
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
            Assert.Same(image, item.Image);
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
            Assert.Equal(text, item.ToolTipText);
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
        public void ToolStripButton_Ctor_String_Image_EventHandler(string text, Image image, EventHandler onClick)
        {
            using var item = new SubToolStripButton(text, image, onClick);
            Assert.NotNull(item.AccessibilityObject);
            Assert.Null(item.AccessibleDefaultActionDescription);
            Assert.Null(item.AccessibleDescription);
            Assert.Null(item.AccessibleName);
            Assert.Equal(AccessibleRole.Default, item.AccessibleRole);
            Assert.Equal(ToolStripItemAlignment.Left, item.Alignment);
            Assert.False(item.AllowDrop);
            Assert.Equal(AnchorStyles.Top | AnchorStyles.Left, item.Anchor);
            Assert.True(item.AutoSize);
            Assert.True(item.AutoToolTip);
            Assert.True(item.Available);
            Assert.Null(item.BackgroundImage);
            Assert.Equal(ImageLayout.Tile, item.BackgroundImageLayout);
            Assert.Equal(new Rectangle(0, 0, 23, 23), item.Bounds);
            Assert.True(item.CanSelect);
            Assert.True(item.CanRaiseEvents);
            Assert.False(item.Checked);
            Assert.False(item.CheckOnClick);
            Assert.Equal(CheckState.Unchecked, item.CheckState);
            Assert.Null(item.Container);
            Assert.Equal(new Rectangle(2, 2, 19, 19), item.ContentRectangle);
            Assert.True(item.DefaultAutoToolTip);
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
            Assert.False(item.ShowKeyboardCues);
            Assert.Null(item.Site);
            Assert.Equal(new Size(23, 23), item.Size);
            Assert.Null(item.Tag);
            Assert.Equal(text, item.Text);
            Assert.Equal(ContentAlignment.MiddleCenter, item.TextAlign);
            Assert.Equal(ToolStripTextDirection.Horizontal, item.TextDirection);
            Assert.Equal(TextImageRelation.ImageBeforeText, item.TextImageRelation);
            Assert.Equal(text, item.ToolTipText);
            Assert.False(item.Visible);
            Assert.Equal(23, item.Width);
        }

        [WinFormsFact]
        public void ToolStripButton_Ctor_String_Image_EventHandler_InvokeClick_CallsOnClick()
        {
            int callCount = 0;
            EventHandler onClick = (sender, e) => callCount++;
            using var item = new ToolStripButton("text", null, onClick);
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
        public void ToolStripButton_Ctor_String_Image_EventHandler_String(string text, Image image, EventHandler onClick, string name, string expectedName)
        {
            using var item = new SubToolStripButton(text, image, onClick, name);
            Assert.NotNull(item.AccessibilityObject);
            Assert.Null(item.AccessibleDefaultActionDescription);
            Assert.Null(item.AccessibleDescription);
            Assert.Null(item.AccessibleName);
            Assert.Equal(AccessibleRole.Default, item.AccessibleRole);
            Assert.Equal(ToolStripItemAlignment.Left, item.Alignment);
            Assert.False(item.AllowDrop);
            Assert.Equal(AnchorStyles.Top | AnchorStyles.Left, item.Anchor);
            Assert.True(item.AutoSize);
            Assert.True(item.AutoToolTip);
            Assert.True(item.Available);
            Assert.Null(item.BackgroundImage);
            Assert.Equal(ImageLayout.Tile, item.BackgroundImageLayout);
            Assert.Equal(new Rectangle(0, 0, 23, 23), item.Bounds);
            Assert.True(item.CanSelect);
            Assert.True(item.CanRaiseEvents);
            Assert.False(item.Checked);
            Assert.False(item.CheckOnClick);
            Assert.Equal(CheckState.Unchecked, item.CheckState);
            Assert.Null(item.Container);
            Assert.Equal(new Rectangle(2, 2, 19, 19), item.ContentRectangle);
            Assert.True(item.DefaultAutoToolTip);
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
            Assert.False(item.ShowKeyboardCues);
            Assert.Null(item.Site);
            Assert.Equal(new Size(23, 23), item.Size);
            Assert.Null(item.Tag);
            Assert.Equal(text, item.Text);
            Assert.Equal(ContentAlignment.MiddleCenter, item.TextAlign);
            Assert.Equal(ToolStripTextDirection.Horizontal, item.TextDirection);
            Assert.Equal(TextImageRelation.ImageBeforeText, item.TextImageRelation);
            Assert.Equal(text, item.ToolTipText);
            Assert.False(item.Visible);
            Assert.Equal(23, item.Width);
        }

        [WinFormsFact]
        public void ToolStripButton_Ctor_String_Image_EventHandler_String_InvokeClick_CallsOnClick()
        {
            int callCount = 0;
            EventHandler onClick = (sender, e) => callCount++;
            using var item = new ToolStripButton("text", null, onClick, "name");
            item.PerformClick();
            Assert.Equal(1, callCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ToolStripButton_AutoToolTip_Set_GetReturnsExpected(bool value)
        {
            using var item = new ToolStripButton
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
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ToolStripButton_CheckOnClick_Set_GetReturnsExpected(bool value)
        {
            using var item = new ToolStripButton
            {
                CheckOnClick = value
            };
            Assert.Equal(value, item.CheckOnClick);

            // Set same.
            item.CheckOnClick = value;
            Assert.Equal(value, item.CheckOnClick);

            // Set different.
            item.CheckOnClick = !value;
            Assert.Equal(!value, item.CheckOnClick);
        }

        [WinFormsTheory]
        [InlineData(true, CheckState.Checked, CheckState.Unchecked)]
        [InlineData(false, CheckState.Unchecked, CheckState.Checked)]
        public void ToolStripButton_Checked_Set_GetReturnsExpected(bool value, CheckState expectedCheckState1, CheckState expectedCheckState2)
        {
            using var item = new ToolStripButton
            {
                Checked = value
            };
            Assert.Equal(value, item.Checked);
            Assert.Equal(expectedCheckState1, item.CheckState);

            // Set same.
            item.Checked = value;
            Assert.Equal(value, item.Checked);
            Assert.Equal(expectedCheckState1, item.CheckState);

            // Set different.
            item.Checked = !value;
            Assert.Equal(!value, item.Checked);
            Assert.Equal(expectedCheckState2, item.CheckState);
        }

        [WinFormsTheory]
        [InlineData(true, CheckState.Checked, CheckState.Unchecked)]
        [InlineData(false, CheckState.Unchecked, CheckState.Checked)]
        public void ToolStripButton_Checked_SetWithOwner_GetReturnsExpected(bool value, CheckState expectedCheckState1, CheckState expectedCheckState2)
        {
            using var owner = new ToolStrip();
            using var item = new ToolStripButton
            {
                Owner = owner,
                Checked = value
            };
            Assert.Equal(value, item.Checked);
            Assert.Equal(expectedCheckState1, item.CheckState);
            Assert.False(owner.IsHandleCreated);

            // Set same.
            item.Checked = value;
            Assert.Equal(value, item.Checked);
            Assert.Equal(expectedCheckState1, item.CheckState);
            Assert.False(owner.IsHandleCreated);

            // Set different.
            item.Checked = !value;
            Assert.Equal(!value, item.Checked);
            Assert.Equal(expectedCheckState2, item.CheckState);
            Assert.False(owner.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, CheckState.Checked, CheckState.Unchecked)]
        [InlineData(false, CheckState.Unchecked, CheckState.Checked)]
        public void ToolStripButton_Checked_SetWithOwnerWithHandle_GetReturnsExpected(bool value, CheckState expectedCheckState1, CheckState expectedCheckState2)
        {
            using var owner = new ToolStrip();
            using var item = new ToolStripButton
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

            item.Checked = value;
            Assert.Equal(value, item.Checked);
            Assert.Equal(expectedCheckState1, item.CheckState);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            item.Checked = value;
            Assert.Equal(value, item.Checked);
            Assert.Equal(expectedCheckState1, item.CheckState);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set different.
            item.Checked = !value;
            Assert.Equal(!value, item.Checked);
            Assert.Equal(expectedCheckState2, item.CheckState);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [InlineData(true, CheckState.Checked, CheckState.Unchecked)]
        [InlineData(false, CheckState.Unchecked, CheckState.Checked)]
        public void ToolStripButton_Checked_SetWithParent_GetReturnsExpected(bool value, CheckState expectedCheckState1, CheckState expectedCheckState2)
        {
            using var parent = new ToolStrip();
            using var item = new SubToolStripButton
            {
                Parent = parent,
                Checked = value
            };
            Assert.Equal(value, item.Checked);
            Assert.Equal(expectedCheckState1, item.CheckState);
            Assert.False(parent.IsHandleCreated);

            // Set same.
            item.Checked = value;
            Assert.Equal(value, item.Checked);
            Assert.Equal(expectedCheckState1, item.CheckState);
            Assert.False(parent.IsHandleCreated);

            // Set different.
            item.Checked = !value;
            Assert.Equal(!value, item.Checked);
            Assert.Equal(expectedCheckState2, item.CheckState);
            Assert.False(parent.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, 2, CheckState.Checked, CheckState.Unchecked)]
        [InlineData(false, 0, CheckState.Unchecked, CheckState.Checked)]
        public void ToolStripButton_Checked_SetWithParentWithHandle_GetReturnsExpected(bool value, int expectedInvalidatedCallCount, CheckState expectedCheckState1, CheckState expectedCheckState2)
        {
            using var parent = new ToolStrip();
            using var item = new SubToolStripButton
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

            item.Checked = value;
            Assert.Equal(value, item.Checked);
            Assert.Equal(expectedCheckState1, item.CheckState);
            Assert.True(parent.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            item.Checked = value;
            Assert.Equal(value, item.Checked);
            Assert.Equal(expectedCheckState1, item.CheckState);
            Assert.True(parent.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set different.
            item.Checked = !value;
            Assert.Equal(!value, item.Checked);
            Assert.Equal(expectedCheckState2, item.CheckState);
            Assert.True(parent.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount + 2, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void ToolStripButton_Checked_SetWithHandler_CallsCheckedChanged()
        {
            using var item = new ToolStripButton
            {
                Checked = true
            };
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(item, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            item.CheckedChanged += handler;
            int checkStateCallCount = 0;
            EventHandler checkStateHandler = (sender, e) =>
            {
                Assert.Same(item, sender);
                Assert.Same(EventArgs.Empty, e);
                checkStateCallCount++;
            };
            item.CheckStateChanged += checkStateHandler;

            // Set different.
            item.Checked = false;
            Assert.False(item.Checked);
            Assert.Equal(1, callCount);
            Assert.Equal(1, checkStateCallCount);

            // Set same.
            item.Checked = false;
            Assert.False(item.Checked);
            Assert.Equal(1, callCount);
            Assert.Equal(1, checkStateCallCount);

            // Set different.
            item.Checked = true;
            Assert.True(item.Checked);
            Assert.Equal(2, callCount);
            Assert.Equal(2, checkStateCallCount);

            // Remove handler.
            item.CheckedChanged -= handler;
            item.Checked = false;
            Assert.False(item.Checked);
            Assert.Equal(2, callCount);
            Assert.Equal(3, checkStateCallCount);

            // Remove other handler.
            item.CheckStateChanged -= checkStateHandler;
            item.Checked = true;
            Assert.True(item.Checked);
            Assert.Equal(2, callCount);
            Assert.Equal(3, checkStateCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(CheckState))]
        public void ToolStripButton_CheckState_Set_GetReturnsExpected(CheckState value)
        {
            using var item = new ToolStripButton
            {
                CheckState = value
            };
            Assert.Equal(value, item.CheckState);
            Assert.Equal(value != CheckState.Unchecked, item.Checked);

            // Set same.
            item.CheckState = value;
            Assert.Equal(value, item.CheckState);
            Assert.Equal(value != CheckState.Unchecked, item.Checked);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(CheckState))]
        public void ToolStripButton_CheckState_SetWithOwner_GetReturnsExpected(CheckState value)
        {
            using var owner = new ToolStrip();
            using var item = new ToolStripButton
            {
                Owner = owner,
                CheckState = value
            };
            Assert.Equal(value, item.CheckState);
            Assert.Equal(value != CheckState.Unchecked, item.Checked);
            Assert.False(owner.IsHandleCreated);

            // Set same.
            item.CheckState = value;
            Assert.Equal(value, item.CheckState);
            Assert.Equal(value != CheckState.Unchecked, item.Checked);
            Assert.False(owner.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(CheckState))]
        public void ToolStripButton_CheckState_SetWithOwnerWithHandle_GetReturnsExpected(CheckState value)
        {
            using var owner = new ToolStrip();
            using var item = new ToolStripButton
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

            item.CheckState = value;
            Assert.Equal(value, item.CheckState);
            Assert.Equal(value != CheckState.Unchecked, item.Checked);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            item.CheckState = value;
            Assert.Equal(value, item.CheckState);
            Assert.Equal(value != CheckState.Unchecked, item.Checked);
            Assert.True(owner.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void ToolStripButton_CheckState_SetWithHandler_CallsCheckedChanged()
        {
            using var item = new ToolStripButton
            {
                CheckState = CheckState.Checked
            };
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(item, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            item.CheckedChanged += handler;
            int checkStateCallCount = 0;
            EventHandler checkStateHandler = (sender, e) =>
            {
                Assert.Same(item, sender);
                Assert.Same(EventArgs.Empty, e);
                checkStateCallCount++;
            };
            item.CheckStateChanged += checkStateHandler;

            // Set different.
            item.CheckState = CheckState.Unchecked;
            Assert.Equal(CheckState.Unchecked, item.CheckState);
            Assert.Equal(1, callCount);
            Assert.Equal(1, checkStateCallCount);

            // Set same.
            item.CheckState = CheckState.Unchecked;
            Assert.Equal(CheckState.Unchecked, item.CheckState);
            Assert.Equal(1, callCount);
            Assert.Equal(1, checkStateCallCount);

            // Set different.
            item.CheckState = CheckState.Indeterminate;
            Assert.Equal(CheckState.Indeterminate, item.CheckState);
            Assert.Equal(2, callCount);
            Assert.Equal(2, checkStateCallCount);

            // Remove handler.
            item.CheckedChanged -= handler;
            item.CheckState = CheckState.Unchecked;
            Assert.Equal(CheckState.Unchecked, item.CheckState);
            Assert.Equal(2, callCount);
            Assert.Equal(3, checkStateCallCount);

            // Remove other handler.
            item.CheckStateChanged -= checkStateHandler;
            item.CheckState = CheckState.Indeterminate;
            Assert.Equal(CheckState.Indeterminate, item.CheckState);
            Assert.Equal(2, callCount);
            Assert.Equal(3, checkStateCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(CheckState))]
        public void ToolStripButton_CheckState_SetInvalidValue_ThrowsInvalidEnumArgumentException(CheckState value)
        {
            using var item = new ToolStripButton();
            Assert.Throws<InvalidEnumArgumentException>("value", () => item.CheckState = value);
        }

        [WinFormsFact]
        public void ToolStripButton_CreateAccessibilityInstance_Invoke_ReturnsExpected()
        {
            using var item = new SubToolStripButton();
            ToolStripItem.ToolStripItemAccessibleObject accessibleObject = Assert.IsAssignableFrom<ToolStripItem.ToolStripItemAccessibleObject>(item.CreateAccessibilityInstance());
            Assert.Equal(AccessibleRole.PushButton, accessibleObject.Role);
            Assert.Equal(AccessibleStates.Focusable, accessibleObject.State);
            Assert.NotSame(accessibleObject, item.CreateAccessibilityInstance());
            Assert.NotSame(accessibleObject, item.AccessibilityObject);
        }

        [WinFormsFact]
        public void ToolStripButton_CreateAccessibilityInstance_InvokeWithCustomRole_ReturnsExpected()
        {
            using var item = new SubToolStripButton
            {
                AccessibleRole = AccessibleRole.HelpBalloon
            };
            ToolStripItem.ToolStripItemAccessibleObject accessibleObject = Assert.IsAssignableFrom<ToolStripItem.ToolStripItemAccessibleObject>(item.CreateAccessibilityInstance());
            Assert.Equal(AccessibleRole.HelpBalloon, accessibleObject.Role);
            Assert.Equal(AccessibleStates.Focusable, accessibleObject.State);
            Assert.NotSame(accessibleObject, item.CreateAccessibilityInstance());
            Assert.NotSame(accessibleObject, item.AccessibilityObject);
        }

        [WinFormsFact]
        public void ToolStripButton_CreateAccessibilityInstance_InvokeCheckOnClick_ReturnsExpected()
        {
            using var item = new SubToolStripButton
            {
                CheckOnClick = true
            };
            ToolStripItem.ToolStripItemAccessibleObject accessibleObject = Assert.IsAssignableFrom<ToolStripItem.ToolStripItemAccessibleObject>(item.CreateAccessibilityInstance());
            Assert.Equal(AccessibleRole.CheckButton, accessibleObject.Role);
            Assert.Equal(AccessibleStates.Focusable, accessibleObject.State);
            Assert.NotSame(accessibleObject, item.CreateAccessibilityInstance());
            Assert.NotSame(accessibleObject, item.AccessibilityObject);
        }

        [WinFormsFact]
        public void ToolStripButton_CreateAccessibilityInstance_InvokeCheckOnClickWithCustomRole_ReturnsExpected()
        {
            using var item = new SubToolStripButton
            {
                CheckOnClick = true,
                AccessibleRole = AccessibleRole.HelpBalloon
            };
            ToolStripItem.ToolStripItemAccessibleObject accessibleObject = Assert.IsAssignableFrom<ToolStripItem.ToolStripItemAccessibleObject>(item.CreateAccessibilityInstance());
            Assert.Equal(AccessibleRole.CheckButton, accessibleObject.Role);
            Assert.Equal(AccessibleStates.Focusable, accessibleObject.State);
            Assert.NotSame(accessibleObject, item.CreateAccessibilityInstance());
            Assert.NotSame(accessibleObject, item.AccessibilityObject);
        }

        [WinFormsTheory]
        [InlineData(true, CheckState.Checked, AccessibleStates.Focusable | AccessibleStates.Checked)]
        [InlineData(true, CheckState.Indeterminate, AccessibleStates.Focusable | AccessibleStates.Checked)]
        [InlineData(true, CheckState.Unchecked, AccessibleStates.Focusable)]
        [InlineData(false, CheckState.Checked, AccessibleStates.Unavailable)]
        [InlineData(false, CheckState.Indeterminate, AccessibleStates.Unavailable)]
        [InlineData(false, CheckState.Unchecked, AccessibleStates.Unavailable)]
        public void ToolStripButton_CreateAccessibilityInstance_InvokeChecked_ReturnsExpected(bool enabled, CheckState checkState, AccessibleStates expectedState)
        {
            using var item = new SubToolStripButton
            {
                Enabled = enabled,
                CheckState = checkState
            };
            ToolStripItem.ToolStripItemAccessibleObject accessibleObject = Assert.IsAssignableFrom<ToolStripItem.ToolStripItemAccessibleObject>(item.CreateAccessibilityInstance());
            Assert.Equal(AccessibleRole.PushButton, accessibleObject.Role);
            Assert.Equal(expectedState, accessibleObject.State);
            Assert.NotSame(accessibleObject, item.CreateAccessibilityInstance());
            Assert.NotSame(accessibleObject, item.AccessibilityObject);
        }

        [WinFormsTheory]
        [InlineData(true, AccessibleStates.Focused | AccessibleStates.HotTracked | AccessibleStates.Focusable)]
        [InlineData(false, AccessibleStates.Unavailable | AccessibleStates.Focused)]
        public void ToolStripButton_CreateAccessibilityInstance_InvokeSelected_ReturnsExpected(bool enabled, AccessibleStates expectedState)
        {
            using var item = new SubToolStripButton
            {
                Enabled = enabled
            };
            item.Select();
            Assert.True(item.Selected);

            ToolStripItem.ToolStripItemAccessibleObject accessibleObject = Assert.IsAssignableFrom<ToolStripItem.ToolStripItemAccessibleObject>(item.CreateAccessibilityInstance());
            Assert.Equal(AccessibleRole.PushButton, accessibleObject.Role);
            Assert.Equal(expectedState, accessibleObject.State);
            Assert.NotSame(accessibleObject, item.CreateAccessibilityInstance());
            Assert.NotSame(accessibleObject, item.AccessibilityObject);
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
        public void ToolStripButton_GetPreferredSize_Invoke_ReturnsExpected(Size proposedSize)
        {
            using var item = new ToolStripButton();
            Assert.Equal(new Size(23, 4), item.GetPreferredSize(proposedSize));

            // Call again.
            Assert.Equal(new Size(23, 4), item.GetPreferredSize(proposedSize));
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void ToolStripButton_OnCheckedChanged_Invoke_CallsCheckedChanged(EventArgs eventArgs)
        {
            using var item = new SubToolStripButton();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(item, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            item.CheckedChanged += handler;
            item.OnCheckedChanged(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            item.CheckedChanged -= handler;
            item.OnCheckedChanged(eventArgs);
            Assert.Equal(1, callCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void ToolStripButton_OnCheckStateChanged_Invoke_CallsCheckStateChanged(EventArgs eventArgs)
        {
            using var item = new SubToolStripButton();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(item, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            item.CheckStateChanged += handler;
            item.OnCheckStateChanged(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            item.CheckStateChanged -= handler;
            item.OnCheckStateChanged(eventArgs);
            Assert.Equal(1, callCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void ToolStripButton_OnClick_Invoke_CallsClick(EventArgs eventArgs)
        {
            using var item = new SubToolStripButton();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(item, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            item.Click += handler;
            item.OnClick(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(item.Checked);

            // Remove handler.
            item.Click -= handler;
            item.OnClick(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(item.Checked);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void ToolStripButton_OnClick_InvokeCheckOnClick_CallsClick(EventArgs eventArgs)
        {
            using var item = new SubToolStripButton
            {
                CheckOnClick = true
            };
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(item, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            item.Click += handler;
            item.OnClick(eventArgs);
            Assert.Equal(1, callCount);
            Assert.True(item.Checked);

            // Remove handler.
            item.Click -= handler;
            item.OnClick(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(item.Checked);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetPaintEventArgsTheoryData))]
        public void ToolStripButton_OnPaint_Invoke_DoesNotCallPaint(PaintEventArgs eventArgs)
        {
            using var item = new SubToolStripButton();
            int callCount = 0;
            PaintEventHandler handler = (sender, e) =>
            {
                Assert.Same(item, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            item.Paint += handler;
            item.OnPaint(eventArgs);
            Assert.Equal(0, callCount);

            // Remove handler.
            item.Paint -= handler;
            item.OnPaint(eventArgs);
            Assert.Equal(0, callCount);
        }

        public static IEnumerable<object[]> OnPaint_TestData()
        {
            yield return new object[] { ContentAlignment.MiddleCenter, RightToLeft.No, ToolStripItemDisplayStyle.None, 0, 0, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.HidePrefix };
            yield return new object[] { ContentAlignment.MiddleCenter, RightToLeft.No, ToolStripItemDisplayStyle.Image, 1, 0, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.HidePrefix };
            yield return new object[] { ContentAlignment.MiddleCenter, RightToLeft.No, ToolStripItemDisplayStyle.ImageAndText, 1, 1, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.HidePrefix };

            yield return new object[] { ContentAlignment.TopLeft, RightToLeft.Yes, ToolStripItemDisplayStyle.Text, 0, 1, TextFormatFlags.Left | TextFormatFlags.Top | TextFormatFlags.HidePrefix | TextFormatFlags.RightToLeft };
            yield return new object[] { ContentAlignment.TopLeft, RightToLeft.Inherit, ToolStripItemDisplayStyle.Text, 0, 1, TextFormatFlags.Left | TextFormatFlags.Top | TextFormatFlags.HidePrefix };
            yield return new object[] { ContentAlignment.TopLeft, RightToLeft.No, ToolStripItemDisplayStyle.Text, 0, 1, TextFormatFlags.Left | TextFormatFlags.Top | TextFormatFlags.HidePrefix };
            yield return new object[] { ContentAlignment.TopCenter, RightToLeft.Yes, ToolStripItemDisplayStyle.Text, 0, 1, TextFormatFlags.HorizontalCenter | TextFormatFlags.Top | TextFormatFlags.HidePrefix | TextFormatFlags.RightToLeft };
            yield return new object[] { ContentAlignment.TopCenter, RightToLeft.Inherit, ToolStripItemDisplayStyle.Text, 0, 1, TextFormatFlags.HorizontalCenter | TextFormatFlags.Top | TextFormatFlags.HidePrefix };
            yield return new object[] { ContentAlignment.TopCenter, RightToLeft.No, ToolStripItemDisplayStyle.Text, 0, 1, TextFormatFlags.HorizontalCenter | TextFormatFlags.Top | TextFormatFlags.HidePrefix };
            yield return new object[] { ContentAlignment.TopRight, RightToLeft.Yes, ToolStripItemDisplayStyle.Text, 0, 1, TextFormatFlags.Right | TextFormatFlags.Top | TextFormatFlags.HidePrefix | TextFormatFlags.RightToLeft };
            yield return new object[] { ContentAlignment.TopRight, RightToLeft.Inherit, ToolStripItemDisplayStyle.Text, 0, 1, TextFormatFlags.Right | TextFormatFlags.Top | TextFormatFlags.HidePrefix };
            yield return new object[] { ContentAlignment.TopRight, RightToLeft.No, ToolStripItemDisplayStyle.Text, 0, 1, TextFormatFlags.Right | TextFormatFlags.Top | TextFormatFlags.HidePrefix };

            yield return new object[] { ContentAlignment.MiddleLeft, RightToLeft.Yes, ToolStripItemDisplayStyle.Text, 0, 1, TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.HidePrefix | TextFormatFlags.RightToLeft };
            yield return new object[] { ContentAlignment.MiddleLeft, RightToLeft.Inherit, ToolStripItemDisplayStyle.Text, 0, 1, TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.HidePrefix };
            yield return new object[] { ContentAlignment.MiddleLeft, RightToLeft.No, ToolStripItemDisplayStyle.Text, 0, 1, TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.HidePrefix };
            yield return new object[] { ContentAlignment.MiddleCenter, RightToLeft.Yes, ToolStripItemDisplayStyle.Text, 0, 1, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.HidePrefix | TextFormatFlags.RightToLeft };
            yield return new object[] { ContentAlignment.MiddleCenter, RightToLeft.Inherit, ToolStripItemDisplayStyle.Text, 0, 1, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.HidePrefix };
            yield return new object[] { ContentAlignment.MiddleCenter, RightToLeft.No, ToolStripItemDisplayStyle.Text, 0, 1, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.HidePrefix };
            yield return new object[] { ContentAlignment.MiddleRight, RightToLeft.Yes, ToolStripItemDisplayStyle.Text, 0, 1, TextFormatFlags.Right | TextFormatFlags.VerticalCenter | TextFormatFlags.HidePrefix | TextFormatFlags.RightToLeft };
            yield return new object[] { ContentAlignment.MiddleRight, RightToLeft.Inherit, ToolStripItemDisplayStyle.Text, 0, 1, TextFormatFlags.Right | TextFormatFlags.VerticalCenter | TextFormatFlags.HidePrefix };
            yield return new object[] { ContentAlignment.MiddleRight, RightToLeft.No, ToolStripItemDisplayStyle.Text, 0, 1, TextFormatFlags.Right | TextFormatFlags.VerticalCenter | TextFormatFlags.HidePrefix };

            yield return new object[] { ContentAlignment.BottomLeft, RightToLeft.Yes, ToolStripItemDisplayStyle.Text, 0, 1, TextFormatFlags.Left | TextFormatFlags.Bottom | TextFormatFlags.HidePrefix | TextFormatFlags.RightToLeft };
            yield return new object[] { ContentAlignment.BottomLeft, RightToLeft.Inherit, ToolStripItemDisplayStyle.Text, 0, 1, TextFormatFlags.Left | TextFormatFlags.Bottom | TextFormatFlags.HidePrefix };
            yield return new object[] { ContentAlignment.BottomLeft, RightToLeft.No, ToolStripItemDisplayStyle.Text, 0, 1, TextFormatFlags.Left | TextFormatFlags.Bottom | TextFormatFlags.HidePrefix };
            yield return new object[] { ContentAlignment.BottomCenter, RightToLeft.Yes, ToolStripItemDisplayStyle.Text, 0, 1, TextFormatFlags.HorizontalCenter | TextFormatFlags.Bottom | TextFormatFlags.HidePrefix | TextFormatFlags.RightToLeft };
            yield return new object[] { ContentAlignment.BottomCenter, RightToLeft.Inherit, ToolStripItemDisplayStyle.Text, 0, 1, TextFormatFlags.HorizontalCenter | TextFormatFlags.Bottom | TextFormatFlags.HidePrefix };
            yield return new object[] { ContentAlignment.BottomCenter, RightToLeft.No, ToolStripItemDisplayStyle.Text, 0, 1, TextFormatFlags.HorizontalCenter | TextFormatFlags.Bottom | TextFormatFlags.HidePrefix };
            yield return new object[] { ContentAlignment.BottomRight, RightToLeft.Yes, ToolStripItemDisplayStyle.Text, 0, 1, TextFormatFlags.Right | TextFormatFlags.Bottom | TextFormatFlags.HidePrefix | TextFormatFlags.RightToLeft };
            yield return new object[] { ContentAlignment.BottomRight, RightToLeft.Inherit, ToolStripItemDisplayStyle.Text, 0, 1, TextFormatFlags.Right | TextFormatFlags.Bottom | TextFormatFlags.HidePrefix };
            yield return new object[] { ContentAlignment.BottomRight, RightToLeft.No, ToolStripItemDisplayStyle.Text, 0, 1, TextFormatFlags.Right | TextFormatFlags.Bottom | TextFormatFlags.HidePrefix };
        }

        [WinFormsTheory]
        [MemberData(nameof(OnPaint_TestData))]
        public void ToolStripButton_OnPaint_InvokeWithOwner_DoesNotCallPaint(ContentAlignment textAlign, RightToLeft rightToLeft, ToolStripItemDisplayStyle displayStyle, int expectedRenderItemImageCallCount, int expectedRenderItemTextCallCount, TextFormatFlags expectedTextFormat)
        {
            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);
            var eventArgs = new PaintEventArgs(graphics, new Rectangle(1, 2, 3, 4));

            var renderer = new SubToolStripRenderer();
            using var owner = new ToolStrip
            {
                Renderer = renderer
            };
            using var font = new Font("Arial", 8.25f);
            using var item = new SubToolStripButton
            {
                DisplayStyle = displayStyle,
                Font = font,
                ForeColor = Color.Red,
                RightToLeft = rightToLeft,
                Text = "Text",
                TextAlign = textAlign,
                TextDirection = ToolStripTextDirection.Vertical270,
                Owner = owner
            };

            int renderButtonBackgroundCallCount = 0;
            renderer.RenderButtonBackground += (sender, e) =>
            {
                Assert.Same(renderer, sender);
                Assert.Same(graphics, e.Graphics);
                Assert.Same(item, e.Item);
                Assert.Null(e.ToolStrip);
                renderButtonBackgroundCallCount++;
            };
            int renderItemImageCallCount = 0;
            renderer.RenderItemImage += (sender, e) =>
            {
                Assert.Same(renderer, sender);
                Assert.Same(graphics, e.Graphics);
                Assert.Same(item, e.Item);
                Assert.Null(e.ToolStrip);
                Assert.True(e.ImageRectangle.X >= 0);
                Assert.True(e.ImageRectangle.Y >= 0);
                Assert.Equal(0, e.ImageRectangle.Width);
                Assert.Equal(0, e.ImageRectangle.Height);
                renderItemImageCallCount++;
            };
            int renderItemTextCallCount = 0;
            renderer.RenderItemText += (sender, e) =>
            {
                Assert.Same(renderer, sender);
                Assert.Same(graphics, e.Graphics);
                Assert.Same(item, e.Item);
                Assert.Null(e.ToolStrip);
                Assert.Equal("Text", e.Text);
                Assert.Equal(Color.Red, e.TextColor);
                Assert.Equal(ToolStripTextDirection.Vertical270, e.TextDirection);
                Assert.Same(font, e.TextFont);
                Assert.Equal(expectedTextFormat, e.TextFormat);
                Assert.True(e.TextRectangle.X >= 0);
                Assert.True(e.TextRectangle.Y >= 0);
                Assert.True(e.TextRectangle.Width >= 0);
                Assert.True(e.TextRectangle.Height >= 0);
                renderItemTextCallCount++;
            };
            int callCount = 0;
            PaintEventHandler handler = (sender, e) =>
            {
                Assert.Same(item, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            item.Paint += handler;
            item.OnPaint(eventArgs);
            Assert.Equal(0, callCount);
            Assert.Equal(1, renderButtonBackgroundCallCount);
            Assert.Equal(expectedRenderItemImageCallCount, renderItemImageCallCount);
            Assert.Equal(expectedRenderItemTextCallCount, renderItemTextCallCount);

            // Remove handler.
            item.Paint -= handler;
            item.OnPaint(eventArgs);
            Assert.Equal(0, callCount);
            Assert.Equal(2, renderButtonBackgroundCallCount);
            Assert.Equal(expectedRenderItemImageCallCount * 2, renderItemImageCallCount);
            Assert.Equal(expectedRenderItemTextCallCount * 2, renderItemTextCallCount);
        }

        [WinFormsTheory]
        [MemberData(nameof(OnPaint_TestData))]
        public void ToolStripButton_OnPaint_InvokeWithOwnerCantShowKeyboardCues_DoesNotCallPaint(ContentAlignment textAlign, RightToLeft rightToLeft, ToolStripItemDisplayStyle displayStyle, int expectedRenderItemImageCallCount, int expectedRenderItemTextCallCount, TextFormatFlags expectedTextFormat)
        {
            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);
            var eventArgs = new PaintEventArgs(graphics, new Rectangle(1, 2, 3, 4));

            var renderer = new SubToolStripRenderer();
            using var owner = new ToolStrip
            {
                Renderer = renderer
            };
            using var font = new Font("Arial", 8.25f);
            using var item = new CantShowKeyboardCuesToolStripButton
            {
                DisplayStyle = displayStyle,
                Font = font,
                ForeColor = Color.Red,
                RightToLeft = rightToLeft,
                Text = "Text",
                TextAlign = textAlign,
                TextDirection = ToolStripTextDirection.Vertical270,
                Owner = owner
            };

            int renderButtonBackgroundCallCount = 0;
            renderer.RenderButtonBackground += (sender, e) =>
            {
                Assert.Same(renderer, sender);
                Assert.Same(graphics, e.Graphics);
                Assert.Same(item, e.Item);
                Assert.Null(e.ToolStrip);
                renderButtonBackgroundCallCount++;
            };
            int renderItemImageCallCount = 0;
            renderer.RenderItemImage += (sender, e) =>
            {
                Assert.Same(renderer, sender);
                Assert.Same(graphics, e.Graphics);
                Assert.Same(item, e.Item);
                Assert.Null(e.ToolStrip);
                Assert.True(e.ImageRectangle.X >= 0);
                Assert.True(e.ImageRectangle.Y >= 0);
                Assert.Equal(0, e.ImageRectangle.Width);
                Assert.Equal(0, e.ImageRectangle.Height);
                renderItemImageCallCount++;
            };
            int renderItemTextCallCount = 0;
            renderer.RenderItemText += (sender, e) =>
            {
                Assert.Same(renderer, sender);
                Assert.Same(graphics, e.Graphics);
                Assert.Same(item, e.Item);
                Assert.Null(e.ToolStrip);
                Assert.Equal("Text", e.Text);
                Assert.Equal(Color.Red, e.TextColor);
                Assert.Equal(ToolStripTextDirection.Vertical270, e.TextDirection);
                Assert.Same(font, e.TextFont);
                Assert.Equal(expectedTextFormat, e.TextFormat);
                Assert.True(e.TextRectangle.X >= 0);
                Assert.True(e.TextRectangle.Y >= 0);
                Assert.True(e.TextRectangle.Width >= 0);
                Assert.True(e.TextRectangle.Height >= 0);
                renderItemTextCallCount++;
            };
            int callCount = 0;
            PaintEventHandler handler = (sender, e) =>
            {
                Assert.Same(item, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            item.Paint += handler;
            item.OnPaint(eventArgs);
            Assert.Equal(0, callCount);
            Assert.Equal(1, renderButtonBackgroundCallCount);
            Assert.Equal(expectedRenderItemImageCallCount, renderItemImageCallCount);
            Assert.Equal(expectedRenderItemTextCallCount, renderItemTextCallCount);

            // Remove handler.
            item.Paint -= handler;
            item.OnPaint(eventArgs);
            Assert.Equal(0, callCount);
            Assert.Equal(2, renderButtonBackgroundCallCount);
            Assert.Equal(expectedRenderItemImageCallCount * 2, renderItemImageCallCount);
            Assert.Equal(expectedRenderItemTextCallCount * 2, renderItemTextCallCount);
        }

        public static IEnumerable<object[]> OnPaint_WithParent_TestData()
        {
            foreach (ToolStripItemDisplayStyle displayStyle in Enum.GetValues(typeof(ToolStripItemDisplayStyle)))
            {
                yield return new object[] { displayStyle, null };

                var image = new Bitmap(10, 10);
                Graphics graphics = Graphics.FromImage(image);
                yield return new object[] { displayStyle, new PaintEventArgs(graphics, new Rectangle(1, 2, 3, 4)) };
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(OnPaint_WithParent_TestData))]
        public void ToolStripButton_OnPaint_InvokeWithParent_DoesNotCallPaint(ToolStripItemDisplayStyle displayStyle, PaintEventArgs eventArgs)
        {
            var renderer = new SubToolStripRenderer();
            using var parent = new ToolStrip
            {
                Renderer = renderer
            };
            using var font = new Font("Arial", 8.25f);
            using var item = new SubToolStripButton
            {
                DisplayStyle = displayStyle,
                Font = font,
                ForeColor = Color.Red,
                Text = "Text",
                TextDirection = ToolStripTextDirection.Vertical270,
                Parent = parent
            };

            int renderButtonBackgroundCallCount = 0;
            renderer.RenderButtonBackground += (sender, e) => renderButtonBackgroundCallCount++;
            int renderItemImageCallCount = 0;
            renderer.RenderItemImage += (sender, e) => renderItemImageCallCount++;
            int renderItemTextCallCount = 0;
            renderer.RenderItemText += (sender, e) => renderItemTextCallCount++;
            int callCount = 0;
            PaintEventHandler handler = (sender, e) => callCount++;

            // Call with handler.
            item.Paint += handler;
            item.OnPaint(eventArgs);
            Assert.Equal(0, callCount);
            Assert.Equal(0, renderButtonBackgroundCallCount);
            Assert.Equal(0, renderItemImageCallCount);
            Assert.Equal(0, renderItemTextCallCount);

            // Remove handler.
            item.Paint -= handler;
            item.OnPaint(eventArgs);
            Assert.Equal(0, callCount);
            Assert.Equal(0, renderButtonBackgroundCallCount);
            Assert.Equal(0, renderItemImageCallCount);
            Assert.Equal(0, renderItemTextCallCount);
        }

        [WinFormsFact]
        public void ToolStripButton_OnPaint_NullE_ThrowsNullReferenceException()
        {
            using var owner = new ToolStrip();
            using var item = new SubToolStripButton
            {
                Owner = owner
            };
            Assert.Throws<NullReferenceException>(() => item.OnPaint(null));
        }

        [WinFormsTheory]
        [InlineData(true, 1)]
        [InlineData(false, 0)]
        public void ToolStripButton_ProcessDialogKey_EnterKey_PerformsClick(bool enabled, int expectedCallCount)
        {
            using var item = new SubToolStripButton
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
        public void ToolStripButton_ProcessDialogKey_EnterKeyWithParent_PerformsClick(bool enabled, int expectedCallCount)
        {
            using var parent = new ToolStrip();
            using var item = new SubToolStripButton
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
        public void ToolStripButton_ProcessDialogKey_EnterKeyWithDropDownParent_PerformsClick(bool enabled, int expectedCallCount)
        {
            var parent = new ToolStripDropDown();
            using var item = new SubToolStripButton
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
        public void ToolStripButton_ProcessDialogKey_SpaceKey_PerformsClick(bool enabled, int expectedCallCount)
        {
            using var item = new SubToolStripButton
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
        public void ToolStripButton_ProcessDialogKey_SpaceKeyWithParent_PerformsClick(bool enabled, int expectedCallCount)
        {
            var parent = new ToolStripDropDown();
            using var item = new SubToolStripButton
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
        public void ToolStripButton_ProcessDialogKey_SpaceKeyWithDropDownParent_PerformsClick(bool enabled, int expectedCallCount)
        {
            var parent = new ToolStripDropDown();
            using var item = new SubToolStripButton
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
        [InlineData(Keys.A)]
        [InlineData(Keys.None)]
        [InlineData((Keys)(Keys.None - 1))]
        public void ToolStripButton_ProcessDialogKey_UnknownKey_ReturnsFalse(Keys keyData)
        {
            using var item = new SubToolStripButton();
            int callCount = 0;
            item.Click += (sender, e) => callCount++;
            Assert.False(item.ProcessDialogKey(keyData));
            Assert.Equal(0, callCount);
            Assert.False(item.Pressed);
        }

        private class SubToolStripRenderer : ToolStripRenderer
        {
        }

        private class CantShowKeyboardCuesToolStripButton : ToolStripButton
        {
            protected internal override bool ShowKeyboardCues => false;

            public new void OnPaint(PaintEventArgs e) => base.OnPaint(e);
        }

        private class SubToolStripButton : ToolStripButton
        {
            public SubToolStripButton() : base()
            {
            }

            public SubToolStripButton(string text) : base(text)
            {
            }

            public SubToolStripButton(Image image) : base(image)
            {
            }

            public SubToolStripButton(string text, Image image) : base(text, image)
            {
            }

            public SubToolStripButton(string text, Image image, EventHandler onClick) : base(text, image, onClick)
            {
            }

            public SubToolStripButton(string text, Image image, EventHandler onClick, string name) : base(text, image, onClick, name)
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

            public new void OnCheckedChanged(EventArgs e) => base.OnCheckedChanged(e);

            public new void OnCheckStateChanged(EventArgs e) => base.OnCheckStateChanged(e);

            public new void OnClick(EventArgs e) => base.OnClick(e);

            public new void OnPaint(PaintEventArgs e) => base.OnPaint(e);

            public new bool ProcessDialogKey(Keys keyData) => base.ProcessDialogKey(keyData);
        }
    }
}
