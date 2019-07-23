// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using Moq;
using WinForms.Common.Tests;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class ToolStripItemTests
    {
        [Fact]
        public void ToolStripItem_Ctor_Default()
        {
            var item = new SubToolStripItem();
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
            Assert.Equal(new Rectangle(2, 2, 19, 19), item.ContentRectangle);
            Assert.False(item.DefaultAutoToolTip);
            Assert.Equal(ToolStripItemDisplayStyle.ImageAndText, item.DefaultDisplayStyle);
            Assert.Equal(Padding.Empty, item.DefaultPadding);
            Assert.Equal(new Size(23, 23), item.DefaultSize);
            Assert.True(item.DismissWhenClicked);
            Assert.Equal(ToolStripItemDisplayStyle.ImageAndText, item.DisplayStyle);
            Assert.Equal(DockStyle.None, item.Dock);
            Assert.False(item.DoubleClickEnabled);
            Assert.True(item.Enabled);
            Assert.Equal(23, item.Height);
            Assert.Null(item.Image);
            Assert.Equal(ContentAlignment.MiddleCenter, item.ImageAlign);
            Assert.Equal(-1, item.ImageIndex);
            Assert.Empty(item.ImageKey);
            Assert.Equal(ToolStripItemImageScaling.SizeToFit, item.ImageScaling);
            Assert.Equal(Color.Empty, item.ImageTransparentColor);
            Assert.False(item.IsDisposed);
            Assert.False(item.IsOnOverflow);
            Assert.Equal(new Padding(0, 1, 0, 2), item.Margin);
            Assert.Equal(MergeAction.Append, item.MergeAction);
            Assert.Equal(-1, item.MergeIndex);
            Assert.Empty(item.Name);
            Assert.Equal(ToolStripItemOverflow.AsNeeded, item.Overflow);
            Assert.Equal(Padding.Empty, item.Padding);
            Assert.Null(item.Parent);
            Assert.Equal(ToolStripItemPlacement.None, item.Placement);
            Assert.False(item.Pressed);
            Assert.False(item.RightToLeftAutoMirrorImage);
            Assert.Equal(new Size(23, 23), item.Size);
            Assert.Null(item.Tag);
            Assert.Empty(item.Text);
            Assert.Equal(ContentAlignment.MiddleCenter, item.TextAlign);
            Assert.Equal(TextImageRelation.ImageBeforeText, item.TextImageRelation);
            Assert.Equal(23, item.Width);
        }

        public static IEnumerable<object[]> Ctor_String_Image_EventHandler_TestData()
        {
            EventHandler onClick = (sender, e) => { };

            yield return new object[] { null, null, null };
            yield return new object[] { string.Empty, new Bitmap(10, 10), onClick };
            yield return new object[] { "text", new Bitmap(10, 10), onClick };
        }

        [Theory]
        [MemberData(nameof(Ctor_String_Image_EventHandler_TestData))]
        public void ToolStripItem_Ctor_String_Image_EventHandler(string text, Image image, EventHandler onClick)
        {
            var item = new SubToolStripItem(text, image, onClick);
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
            Assert.Equal(new Rectangle(2, 2, 19, 19), item.ContentRectangle);
            Assert.False(item.DefaultAutoToolTip);
            Assert.Equal(ToolStripItemDisplayStyle.ImageAndText, item.DefaultDisplayStyle);
            Assert.Equal(Padding.Empty, item.DefaultPadding);
            Assert.Equal(new Size(23, 23), item.DefaultSize);
            Assert.True(item.DismissWhenClicked);
            Assert.Equal(ToolStripItemDisplayStyle.ImageAndText, item.DisplayStyle);
            Assert.Equal(DockStyle.None, item.Dock);
            Assert.False(item.DoubleClickEnabled);
            Assert.True(item.Enabled);
            Assert.Equal(23, item.Height);
            Assert.Equal(image, item.Image);
            Assert.Equal(ContentAlignment.MiddleCenter, item.ImageAlign);
            Assert.Equal(-1, item.ImageIndex);
            Assert.Empty(item.ImageKey);
            Assert.Equal(ToolStripItemImageScaling.SizeToFit, item.ImageScaling);
            Assert.Equal(Color.Empty, item.ImageTransparentColor);
            Assert.False(item.IsDisposed);
            Assert.False(item.IsOnOverflow);
            Assert.Equal(new Padding(0, 1, 0, 2), item.Margin);
            Assert.Equal(MergeAction.Append, item.MergeAction);
            Assert.Equal(-1, item.MergeIndex);
            Assert.Empty(item.Name);
            Assert.Equal(ToolStripItemOverflow.AsNeeded, item.Overflow);
            Assert.Equal(Padding.Empty, item.Padding);
            Assert.Null(item.Parent);
            Assert.Equal(ToolStripItemPlacement.None, item.Placement);
            Assert.False(item.Pressed);
            Assert.False(item.RightToLeftAutoMirrorImage);
            Assert.Equal(new Size(23, 23), item.Size);
            Assert.Null(item.Tag);
            Assert.Equal(text, item.Text);
            Assert.Equal(ContentAlignment.MiddleCenter, item.TextAlign);
            Assert.Equal(TextImageRelation.ImageBeforeText, item.TextImageRelation);
            Assert.Equal(23, item.Width);
        }

        public static IEnumerable<object[]> Ctor_String_Image_EventHandler_String_TestData()
        {
            EventHandler onClick = (sender, e) => { };

            yield return new object[] { null, null, null, null, string.Empty };
            yield return new object[] { string.Empty, new Bitmap(10, 10), onClick, string.Empty, string.Empty };
            yield return new object[] { "text", new Bitmap(10, 10), onClick, "name", "name" };
        }

        [Theory]
        [MemberData(nameof(Ctor_String_Image_EventHandler_String_TestData))]
        public void ToolStripItem_Ctor_String_Image_EventHandler_String(string text, Image image, EventHandler onClick, string name, string expectedName)
        {
            var item = new SubToolStripItem(text, image, onClick, name);
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
            Assert.Equal(new Rectangle(2, 2, 19, 19), item.ContentRectangle);
            Assert.False(item.DefaultAutoToolTip);
            Assert.Equal(ToolStripItemDisplayStyle.ImageAndText, item.DefaultDisplayStyle);
            Assert.Equal(Padding.Empty, item.DefaultPadding);
            Assert.Equal(new Size(23, 23), item.DefaultSize);
            Assert.True(item.DismissWhenClicked);
            Assert.Equal(ToolStripItemDisplayStyle.ImageAndText, item.DisplayStyle);
            Assert.Equal(DockStyle.None, item.Dock);
            Assert.False(item.DoubleClickEnabled);
            Assert.True(item.Enabled);
            Assert.Equal(23, item.Height);
            Assert.Equal(image, item.Image);
            Assert.Equal(ContentAlignment.MiddleCenter, item.ImageAlign);
            Assert.Equal(-1, item.ImageIndex);
            Assert.Empty(item.ImageKey);
            Assert.Equal(ToolStripItemImageScaling.SizeToFit, item.ImageScaling);
            Assert.Equal(Color.Empty, item.ImageTransparentColor);
            Assert.False(item.IsDisposed);
            Assert.False(item.IsOnOverflow);
            Assert.Equal(new Padding(0, 1, 0, 2), item.Margin);
            Assert.Equal(MergeAction.Append, item.MergeAction);
            Assert.Equal(-1, item.MergeIndex);
            Assert.Equal(expectedName, item.Name);
            Assert.Equal(ToolStripItemOverflow.AsNeeded, item.Overflow);
            Assert.Equal(Padding.Empty, item.Padding);
            Assert.Null(item.Parent);
            Assert.Equal(ToolStripItemPlacement.None, item.Placement);
            Assert.False(item.Pressed);
            Assert.False(item.RightToLeftAutoMirrorImage);
            Assert.Equal(new Size(23, 23), item.Size);
            Assert.Null(item.Tag);
            Assert.Equal(text, item.Text);
            Assert.Equal(ContentAlignment.MiddleCenter, item.TextAlign);
            Assert.Equal(TextImageRelation.ImageBeforeText, item.TextImageRelation);
            Assert.Equal(23, item.Width);
        }

        [Fact]
        public void ToolStripItem_AccessibilityObject_GetNullCreatedInstance_ReturnsNull()
        {
            var item = new NullCreateAccessibilityInstanceToolStripItem();
            Assert.Null(item.AccessibilityObject);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringTheoryData))]
        public void ToolStripItem_AccessibleDefaultActionDescription_Set_GetReturnsExpected(string value)
        {
            var item = new SubToolStripItem
            {
                AccessibleDefaultActionDescription = value
            };
            Assert.Equal(value, item.AccessibleDefaultActionDescription);

            // Set same.
            item.AccessibleDefaultActionDescription = value;
            Assert.Equal(value, item.AccessibleDefaultActionDescription);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringTheoryData))]
        public void ToolStripItem_AccessibleDescription_Set_GetReturnsExpected(string value)
        {
            var item = new SubToolStripItem
            {
                AccessibleDescription = value
            };
            Assert.Equal(value, item.AccessibleDescription);

            // Set same.
            item.AccessibleDescription = value;
            Assert.Equal(value, item.AccessibleDescription);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringTheoryData))]
        public void ToolStripItem_AccessibleName_Set_GetReturnsExpected(string value)
        {
            var item = new SubToolStripItem
            {
                AccessibleName = value
            };
            Assert.Equal(value, item.AccessibleName);

            // Set same.
            item.AccessibleName = value;
            Assert.Equal(value, item.AccessibleName);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(AccessibleRole))]
        public void ToolStripItem_AccessibleRole_Set_GetReturnsExpected(AccessibleRole value)
        {
            var item = new SubToolStripItem
            {
                AccessibleRole = value
            };
            Assert.Equal(value, item.AccessibleRole);

            // Set same.
            item.AccessibleRole = value;
            Assert.Equal(value, item.AccessibleRole);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(AccessibleRole))]
        public void ToolStripItem_AccessibleRole_SetInvalid_ThrowsInvalidEnumArgumentException(AccessibleRole value)
        {
            var item = new SubToolStripItem();
            Assert.Throws<InvalidEnumArgumentException>("value", () => item.AccessibleRole = value);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(ToolStripItemAlignment))]
        public void ToolStripItem_Alignment_Set_GetReturnsExpected(ToolStripItemAlignment value)
        {
            var item = new SubToolStripItem
            {
                Alignment = value
            };
            Assert.Equal(value, item.Alignment);

            // Set same.
            item.Alignment = value;
            Assert.Equal(value, item.Alignment);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(ToolStripItemAlignment))]
        public void ToolStripItem_Alignment_SetWithParent_GetReturnsExpected(ToolStripItemAlignment value)
        {
            var parent = new ToolStrip();
            var item = new SubToolStripItem
            {
                Parent = parent,

                Alignment = value
            };
            Assert.Equal(value, item.Alignment);

            // Set same.
            item.Alignment = value;
            Assert.Equal(value, item.Alignment);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(ToolStripItemAlignment))]
        public void ToolStripItem_Alignment_SetWithCreatedParent_GetReturnsExpected(ToolStripItemAlignment value)
        {
            var parent = new ToolStrip();
            var item = new SubToolStripItem
            {
                Parent = parent
            };
            Assert.NotEqual(IntPtr.Zero, parent.Handle);

            item.Alignment = value;
            Assert.Equal(value, item.Alignment);

            // Set same.
            item.Alignment = value;
            Assert.Equal(value, item.Alignment);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(ToolStripItemAlignment))]
        public void ToolStripItem_Alignment_SetInvalid_ThrowsInvalidEnumArgumentException(ToolStripItemAlignment value)
        {
            var item = new SubToolStripItem();
            Assert.Throws<InvalidEnumArgumentException>("value", () => item.Alignment = value);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ToolStripItem_AllowDrop_Set_GetReturnsExpected(bool value)
        {
            var item = new SubToolStripItem
            {
                AllowDrop = value
            };
            Assert.Equal(value, item.AllowDrop);

            // Set same.
            item.AllowDrop = value;
            Assert.Equal(value, item.AllowDrop);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ToolStripItem_AllowDrop_SetWithParent_GetReturnsExpected(bool value)
        {
            var parent = new ToolStrip();
            var item = new SubToolStripItem
            {
                Parent = parent,

                AllowDrop = value
            };
            Assert.Equal(value, item.AllowDrop);

            // Set same.
            item.AllowDrop = value;
            Assert.Equal(value, item.AllowDrop);
        }

        public static IEnumerable<object[]> Anchor_TestData()
        {
            yield return new object[] { AnchorStyles.Top | AnchorStyles.Left, AnchorStyles.Top | AnchorStyles.Left };
            yield return new object[] { AnchorStyles.Top, AnchorStyles.Top };
            yield return new object[] { AnchorStyles.None, AnchorStyles.None };
            yield return new object[] { AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right, AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right };
            yield return new object[] { (AnchorStyles)(-1), AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right };
            yield return new object[] { (AnchorStyles)0x10, AnchorStyles.Top | AnchorStyles.Left };
        }

        [Theory]
        [MemberData(nameof(Anchor_TestData))]
        public void ToolStripItem_Anchor_Set_GetReturnsExpected(AnchorStyles value, AnchorStyles expected)
        {
            var item = new SubToolStripItem
            {
                Anchor = value
            };
            Assert.Equal(expected, item.Anchor);

            // Set same.
            item.Anchor = value;
            Assert.Equal(expected, item.Anchor);
        }

        [Theory]
        [MemberData(nameof(Anchor_TestData))]
        public void ToolStripItem_Anchor_SetWithParent_GetReturnsExpected(AnchorStyles value, AnchorStyles expected)
        {
            var parent = new ToolStrip();
            var item = new SubToolStripItem
            {
                Parent = parent,

                Anchor = value
            };
            Assert.Equal(expected, item.Anchor);

            // Set same.
            item.Anchor = value;
            Assert.Equal(expected, item.Anchor);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ToolStripItem_AutoSize_Set_GetReturnsExpected(bool value)
        {
            var item = new SubToolStripItem
            {
                AutoSize = value
            };
            Assert.Equal(value, item.AutoSize);

            // Set same.
            item.AutoSize = value;
            Assert.Equal(value, item.AutoSize);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ToolStripItem_AutoSize_SetWithOwner_GetReturnsExpected(bool value)
        {
            var owner = new ToolStrip();
            var item = new SubToolStripItem();
            owner.Items.Add(item);

            item.AutoSize = value;
            Assert.Equal(value, item.AutoSize);

            // Set same.
            item.AutoSize = value;
            Assert.Equal(value, item.AutoSize);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ToolStripItem_AutoToolTip_Set_GetReturnsExpected(bool value)
        {
            var item = new SubToolStripItem
            {
                AutoToolTip = value
            };
            Assert.Equal(value, item.AutoToolTip);

            // Set same.
            item.AutoToolTip = value;
            Assert.Equal(value, item.AutoToolTip);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ToolStripItem_Available_Set_GetReturnsExpected(bool value)
        {
            var item = new SubToolStripItem
            {
                Available = value
            };
            Assert.Equal(value, item.Available);

            // Set same.
            item.Available = value;
            Assert.Equal(value, item.Available);
        }

        [Fact]
        public void ToolStripItem_Available_SetWithHandler_CallsAvailableChanged()
        {
            var item = new SubToolStripItem();
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

        [Fact]
        public void ToolStripItem_Available_SetWithHandler_CallsVisibleChanged()
        {
            var item = new SubToolStripItem();
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

        public static IEnumerable<object[]> BackColor_Get_TestData()
        {
            yield return new object[] { new SubToolStripItem(), Control.DefaultBackColor };
            yield return new object[] { new SubToolStripItem("text", null, null), Control.DefaultBackColor };
            yield return new object[] { new SubToolStripItem("text", null, null, "name"), Control.DefaultBackColor };

            var toolStrip = new ToolStrip
            {
                BackColor = Color.Red
            };
            var toolStripItem = new SubToolStripItem();
            toolStrip.Items.Add(toolStripItem);
            yield return new object[] { toolStripItem, Control.DefaultBackColor };

            var toolStripParent = new ToolStrip
            {
                BackColor = Color.Red
            };
            var toolStripParentItem = new SubToolStripItem
            {
                Parent = toolStripParent
            };
            yield return new object[] { toolStripParentItem, Color.Red };
        }

        [Theory]
        [MemberData(nameof(BackColor_Get_TestData))]
        public void ToolStripItem_BackColor_Get_ReturnsExpected(ToolStripItem item, Color expected)
        {
            Assert.Equal(expected, item.BackColor);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBackColorTheoryData))]
        public void ToolStripItem_BackColor_Set_GetReturnsExpected(Color value, Color expected)
        {
            var item = new SubToolStripItem
            {
                BackColor = value
            };
            Assert.Equal(expected, item.BackColor);

            // Set same.
            item.BackColor = value;
            Assert.Equal(expected, item.BackColor);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBackColorTheoryData))]
        public void ToolStripItem_BackColor_SetWithParent_GetReturnsExpected(Color value, Color expected)
        {
            var parent = new ToolStrip();
            var item = new SubToolStripItem
            {
                Parent = parent
            };

            item.BackColor = value;
            Assert.Equal(expected, item.BackColor);

            // Set same.
            item.BackColor = value;
            Assert.Equal(expected, item.BackColor);
        }

        [Fact]
        public void ToolStripItem_BackColor_SetWithHandler_CallsBackColorChanged()
        {
            var item = new SubToolStripItem();
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

        public static IEnumerable<object[]> BackgroundImage_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new Bitmap(10, 10) };
            yield return new object[] { Image.FromFile(Path.Combine("bitmaps", "nature24bits.gif")) };
            yield return new object[] { Image.FromFile(Path.Combine("bitmaps", "10x16_one_entry_32bit.ico")) };
        }

        [Theory]
        [MemberData(nameof(BackgroundImage_TestData))]
        public void ToolStripItem_BackgroundImage_Set_GetReturnsExpected(Image value)
        {
            var item = new SubToolStripItem
            {
                BackgroundImage = value
            };
            Assert.Equal(value, item.BackgroundImage);

            // Set same.
            item.BackgroundImage = value;
            Assert.Equal(value, item.BackgroundImage);
        }

        [Theory]
        [MemberData(nameof(BackgroundImage_TestData))]
        public void ToolStripItem_BackgroundImage_SetWithParent_GetReturnsExpected(Image value)
        {
            var parent = new ToolStrip();
            var item = new SubToolStripItem
            {
                Parent = parent
            };

            item.BackgroundImage = value;
            Assert.Equal(value, item.BackgroundImage);

            // Set same.
            item.BackgroundImage = value;
            Assert.Equal(value, item.BackgroundImage);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(ImageLayout))]
        public void ToolStripItem_BackgroundImageLayout_Set_GetReturnsExpected(ImageLayout value)
        {
            var item = new SubToolStripItem
            {
                BackgroundImageLayout = value
            };
            Assert.Equal(value, item.BackgroundImageLayout);

            // Set same.
            item.BackgroundImageLayout = value;
            Assert.Equal(value, item.BackgroundImageLayout);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(ImageLayout))]
        public void ToolStripItem_BackgroundImageLayout_SetWithParent_GetReturnsExpected(ImageLayout value)
        {
            var parent = new ToolStrip();
            var item = new SubToolStripItem
            {
                Parent = parent
            };

            item.BackgroundImageLayout = value;
            Assert.Equal(value, item.BackgroundImageLayout);

            // Set same.
            item.BackgroundImageLayout = value;
            Assert.Equal(value, item.BackgroundImageLayout);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(ImageLayout))]
        public void ToolStripItem_BackgroundImageLayout_SetInvalid_ThrowsInvalidEnumArgumentException(ImageLayout value)
        {
            var item = new SubToolStripItem();
            Assert.Throws<InvalidEnumArgumentException>("value", () => item.BackgroundImageLayout = value);
        }

        [Fact]
        public void ToolStripItem_DefaultAutoToolTip_Custom_AutoToolTipReturnsExpected()
        {
            var item = new CustomToolStripItem();
            Assert.True(item.AutoToolTip);
        }

        [Fact]
        public void ToolStripItem_DefaultDisplayStyle_Custom_DisplayStyleReturnsExpected()
        {
            var item = new CustomToolStripItem();
            Assert.Equal(ToolStripItemDisplayStyle.Text, item.DisplayStyle);
        }

        public static IEnumerable<object[]> DefaultMargin_TestData()
        {
            yield return new object[] { new SubToolStripItem(), new Padding(0, 1, 0, 2) };

            var strip = new ToolStrip();
            var stripItem = new SubToolStripItem();
            strip.Items.Add(stripItem);
            yield return new object[] { stripItem, new Padding(0, 1, 0, 2) };

            var statusStrip = new StatusStrip();
            var statusStripItem = new SubToolStripItem();
            statusStrip.Items.Add(statusStripItem);
            yield return new object[] { statusStripItem, new Padding(0, 2, 0, 0) };
        }

        [Theory]
        [MemberData(nameof(DefaultMargin_TestData))]
        public void ToolStripItem_DefaultMargin_Set_GetReturnsExpected(ToolStripItem item, Padding expected)
        {
            Assert.Equal(expected, item.DefaultMargin);
        }

        [Fact]
        public void ToolStripItem_DefaultMargin_Custom_MarginReturnsExpected()
        {
            var item = new CustomToolStripItem();
            Assert.Equal(new Padding(1, 2, 3, 4), item.Margin);
        }

        [Fact]
        public void ToolStripItem_DefaultPadding_Custom_PaddingReturnsExpected()
        {
            var item = new CustomToolStripItem();
            Assert.Equal(new Padding(2, 3, 4, 5), item.Padding);
        }

        [Fact]
        public void ToolStripItem_DefaultSize_Custom_SizeReturnsExpected()
        {
            var item = new CustomToolStripItem();
            Assert.Equal(new Size(10, 11), item.Size);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(ToolStripItemDisplayStyle))]
        public void ToolStripItem_DisplayStyle_Set_GetReturnsExpected(ToolStripItemDisplayStyle value)
        {
            var item = new SubToolStripItem
            {
                DisplayStyle = value
            };
            Assert.Equal(value, item.DisplayStyle);

            // Set same.
            item.DisplayStyle = value;
            Assert.Equal(value, item.DisplayStyle);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(ToolStripItemDisplayStyle))]
        public void ToolStripItem_DisplayStyle_SetWithOwner_GetReturnsExpected(ToolStripItemDisplayStyle value)
        {
            var owner = new ToolStrip();
            var item = new SubToolStripItem();
            owner.Items.Add(item);

            item.DisplayStyle = value;
            Assert.Equal(value, item.DisplayStyle);

            // Set same.
            item.DisplayStyle = value;
            Assert.Equal(value, item.DisplayStyle);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(ToolStripItemDisplayStyle))]
        public void ToolStripItem_DisplayStyle_SetInvalid_ThrowsInvalidEnumArgumentException(ToolStripItemDisplayStyle value)
        {
            var item = new SubToolStripItem();
            Assert.Throws<InvalidEnumArgumentException>("value", () => item.DisplayStyle = value);
        }

        [Fact]
        public void ToolStripItem_DisplayStyle_SetWithHandler_CallsDisplayStyleChanged()
        {
            var item = new SubToolStripItem();
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

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(DockStyle))]
        public void ToolStripItem_Dock_Set_GetReturnsExpected(DockStyle value)
        {
            var item = new SubToolStripItem
            {
                Dock = value
            };
            Assert.Equal(value, item.Dock);

            // Set same.
            item.Dock = value;
            Assert.Equal(value, item.Dock);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(DockStyle))]
        public void ToolStripItem_Dock_SetWithParent_GetReturnsExpected(DockStyle value)
        {
            var parent = new ToolStrip();
            var item = new SubToolStripItem
            {
                Parent = parent
            };

            item.Dock = value;
            Assert.Equal(value, item.Dock);

            // Set same.
            item.Dock = value;
            Assert.Equal(value, item.Dock);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(DockStyle))]
        public void ToolStripItem_Dock_SetInvalid_ThrowsInvalidEnumArgumentException(DockStyle value)
        {
            var item = new SubToolStripItem();
            Assert.Throws<InvalidEnumArgumentException>("value", () => item.Dock = value);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ToolStripItem_DoubleClickEnabled_Set_GetReturnsExpected(bool value)
        {
            var item = new SubToolStripItem
            {
                DoubleClickEnabled = value
            };
            Assert.Equal(value, item.DoubleClickEnabled);

            // Set same.
            item.DoubleClickEnabled = value;
            Assert.Equal(value, item.DoubleClickEnabled);
        }

        [Fact]
        public void ToolStripItem_Enabled_Set_GetReturnsExpected()
        {
            var item = new SubToolStripItem
            {
                Enabled = false
            };
            Assert.False(item.Enabled);

            item.Enabled = true;
            Assert.True(item.Enabled);

            // Set same.
            item.Enabled = true;
            Assert.True(item.Enabled);
        }

        [Fact]
        public void ToolStripItem_Enabled_SetWithParent_GetReturnsExpected()
        {
            var parent = new ToolStrip();
            var item = new SubToolStripItem
            {
                Parent = parent
            };

            item.Enabled = false;
            Assert.False(item.Enabled);

            item.Enabled = true;
            Assert.True(item.Enabled);

            // Set same.
            item.Enabled = true;
            Assert.True(item.Enabled);
        }

        [Fact]
        public void ToolStripItem_Enabled_SetSelected_GetReturnsExpected()
        {
            var item = new SubToolStripItem();
            item.Select();
            Assert.True(item.Selected);

            item.Enabled = false;
            Assert.False(item.Enabled);
            Assert.False(item.Selected);

            item.Enabled = true;
            Assert.True(item.Enabled);
            Assert.False(item.Selected);
        }

        [Fact]
        public void ToolStripItem_Enabled_SetPressed_GetReturnsExpected()
        {
            var item = new SubToolStripItem();

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

        [Fact]
        public void ToolStripItem_Enabled_SetWithHandler_CallsEnabledChanged()
        {
            var item = new SubToolStripItem();
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

        public static IEnumerable<object[]> Font_GetDefault_TestData()
        {
            yield return new object[] { new SubToolStripItem() };
            yield return new object[] { new SubToolStripItem("text", null, null) };
            yield return new object[] { new SubToolStripItem("text", null, null, "name") };

            var toolStripParent = new ToolStrip
            {
                Font = SystemFonts.MenuFont
            };
            var toolStripParentItem = new SubToolStripItem
            {
                Parent = toolStripParent
            };
            yield return new object[] { toolStripParentItem };
        }

        [Theory]
        [MemberData(nameof(Font_GetDefault_TestData))]
        public void ToolStripItem_Font_GetDefault_ReturnsExpected(ToolStripItem item)
        {
            Assert.Equal(ToolStripManager.DefaultFont, item.Font);
        }

        [Fact]
        public void ToolStripItem_Font_GetWithParent_ReturnsExpected()
        {
            var toolStrip = new ToolStrip
            {
                Font = SystemFonts.MenuFont
            };
            var toolStripItem = new SubToolStripItem();
            toolStrip.Items.Add(toolStripItem);
            Assert.Equal(SystemFonts.MenuFont, toolStripItem.Font);
        }

        public static IEnumerable<object[]> Font_Set_TestData()
        {
            foreach (Enum displayStyle in Enum.GetValues(typeof(ToolStripItemDisplayStyle)))
            {
                yield return new object[] { displayStyle, null };
                yield return new object[] { displayStyle, SystemFonts.MenuFont };
            }
        }

        [Theory]
        [MemberData(nameof(Font_Set_TestData))]
        public void ToolStripItem_Font_Set_GetReturnsExpected(ToolStripItemDisplayStyle displayStyle, Font value)
        {
            var item = new SubToolStripItem
            {
                DisplayStyle = displayStyle
            };

            item.Font = value;
            Assert.Equal(value ?? ToolStripManager.DefaultFont, item.Font);

            // Set same.
            item.Font = value;
            Assert.Equal(value ?? ToolStripManager.DefaultFont, item.Font);
        }

        public static IEnumerable<object[]> ForeColor_Get_TestData()
        {
            yield return new object[] { new SubToolStripItem(), Control.DefaultForeColor };
            yield return new object[] { new SubToolStripItem("text", null, null), Control.DefaultForeColor };
            yield return new object[] { new SubToolStripItem("text", null, null, "name"), Control.DefaultForeColor };

            var toolStrip = new ToolStrip
            {
                ForeColor = Color.Red
            };
            var toolStripItem = new SubToolStripItem();
            toolStrip.Items.Add(toolStripItem);
            yield return new object[] { toolStripItem, Control.DefaultForeColor };

            var toolStripParent = new ToolStrip
            {
                ForeColor = Color.Red
            };
            var toolStripParentItem = new SubToolStripItem
            {
                Parent = toolStripParent
            };
            yield return new object[] { toolStripParentItem, Color.Red };
        }

        [Theory]
        [MemberData(nameof(ForeColor_Get_TestData))]
        public void ToolStripItem_ForeColor_Get_ReturnsExpected(ToolStripItem item, Color expected)
        {
            Assert.Equal(expected, item.ForeColor);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetForeColorTheoryData))]
        public void ToolStripItem_ForeColor_Set_GetReturnsExpected(Color value, Color expected)
        {
            var item = new SubToolStripItem
            {
                ForeColor = value
            };
            Assert.Equal(expected, item.ForeColor);

            // Set same.
            item.ForeColor = value;
            Assert.Equal(expected, item.ForeColor);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetForeColorTheoryData))]
        public void ToolStripItem_ForeColor_SetWithParent_GetReturnsExpected(Color value, Color expected)
        {
            var parent = new ToolStrip();
            var item = new SubToolStripItem
            {
                Parent = parent
            };

            item.ForeColor = value;
            Assert.Equal(expected, item.ForeColor);

            // Set same.
            item.ForeColor = value;
            Assert.Equal(expected, item.ForeColor);
        }

        [Fact]
        public void ToolStripItem_ForeColor_SetWithHandler_CallsForeColorChanged()
        {
            var item = new SubToolStripItem();
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

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetIntTheoryData))]
        public void ToolStripItem_Height_Set_GetReturnsExpected(int value)
        {
            var item = new SubToolStripItem
            {
                Height = value
            };
            Assert.Equal(value, item.Height);

            // Set same.
            item.Height = value;
            Assert.Equal(value, item.Height);
        }

        public static IEnumerable<object[]> Image_TestData()
        {
            foreach (Color imageTransparentColor in new Color[] { Color.Empty, Color.Red })
            {
                yield return new object[] { imageTransparentColor, null };
                yield return new object[] { imageTransparentColor, new Bitmap(10, 10) };
                yield return new object[] { imageTransparentColor, Image.FromFile(Path.Combine("bitmaps", "nature24bits.gif")) };
                yield return new object[] { imageTransparentColor, Image.FromFile(Path.Combine("bitmaps", "10x16_one_entry_32bit.ico")) };
            }
        }

        [Theory]
        [MemberData(nameof(Image_TestData))]
        public void ToolStripItem_Image_Set_GetReturnsExpected(Color imageTransparentColor, Image value)
        {
            var item = new SubToolStripItem
            {
                ImageTransparentColor = imageTransparentColor
            };

            item.Image = value;
            Assert.Equal(value, item.Image);

            // Set same.
            item.Image = value;
            Assert.Equal(value, item.Image);
        }

        [Theory]
        [MemberData(nameof(Image_TestData))]
        public void ToolStripItem_Image_SetAlreadyWithImage_GetReturnsExpected(Color imageTransparentColor, Image value)
        {
            var item = new SubToolStripItem
            {
                Image = new Bitmap(10, 10),
                ImageTransparentColor = imageTransparentColor
            };

            item.Image = value;
            Assert.Equal(value, item.Image);

            // Set same.
            item.Image = value;
            Assert.Equal(value, item.Image);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(ContentAlignment))]
        public void ToolStripItem_ImageAlign_Set_GetReturnsExpected(ContentAlignment value)
        {
            var item = new SubToolStripItem
            {
                ImageAlign = value
            };
            Assert.Equal(value, item.ImageAlign);

            // Set same.
            item.ImageAlign = value;
            Assert.Equal(value, item.ImageAlign);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(ContentAlignment))]
        public void ToolStripItem_ImageAlign_SetWithOwner_GetReturnsExpected(ContentAlignment value)
        {
            var owner = new ToolStrip();
            var item = new SubToolStripItem();
            owner.Items.Add(item);

            item.ImageAlign = value;
            Assert.Equal(value, item.ImageAlign);

            // Set same.
            item.ImageAlign = value;
            Assert.Equal(value, item.ImageAlign);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(ContentAlignment))]
        public void ToolStripItem_ImageAlign_SetInvalid_ThrowsInvalidEnumArgumentException(ContentAlignment value)
        {
            var item = new SubToolStripItem();
            Assert.Throws<InvalidEnumArgumentException>("value", () => item.ImageAlign = value);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        public void ToolStripItem_ImageIndex_Set_GetReturnsExpected(int value)
        {
            var item = new SubToolStripItem
            {
                ImageIndex = value
            };
            Assert.Equal(value, item.ImageIndex);
            Assert.Null(item.Image);

            // Set same.
            item.ImageIndex = value;
            Assert.Equal(value, item.ImageIndex);
            Assert.Null(item.Image);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        public void ToolStripItem_ImageIndex_SetWithImage_GetReturnsExpected(int value)
        {
            using (var image = new Bitmap(10, 10))
            {
                var item = new SubToolStripItem
                {
                    Image = image
                };

                item.ImageIndex = value;
                Assert.Null(item.Image);
            }
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        public void ToolStripItem_ImageIndex_SetWithOwnerWithoutImageList_ReturnsExpected(int value)
        {
            var owner = new ToolStrip();
            var item = new SubToolStripItem();
            owner.Items.Add(item);

            item.ImageIndex = value;
            Assert.Equal(value, item.ImageIndex);
            Assert.Null(item.Image);
        }

        [Theory]
        [InlineData(-1, -1)]
        [InlineData(0, 0)]
        [InlineData(1, 0)]
        public void ToolStripItem_ImageIndex_SetWithOwnerWithImageList_ReturnsExpected(int value, int expected)
        {
            var owner = new ToolStrip
            {
                ImageList = new ImageList()
            };
            owner.ImageList.Images.Add(new Bitmap(10, 10));

            var item = new SubToolStripItem();
            owner.Items.Add(item);

            item.ImageIndex = value;
            Assert.Equal(expected, item.ImageIndex);
            Assert.Equal(value == 0, item.Image != null);
        }

        [Fact]
        public void ToolStripItem_ImageIndex_SetInvalid_ThrowsArgumentOutOfRangeException()
        {
            var item = new SubToolStripItem();
            Assert.Throws<ArgumentOutOfRangeException>("value", () => item.ImageIndex = -2);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringTheoryData))]
        public void ToolStripItem_ImageKey_Set_GetReturnsExpected(string value)
        {
            var item = new SubToolStripItem
            {
                ImageKey = value
            };
            Assert.Equal(value, item.ImageKey);

            // Set same.
            item.ImageKey = value;
            Assert.Equal(value, item.ImageKey);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringTheoryData))]
        public void ToolStripItem_ImageKey_SetWithImage_GetReturnsExpected(string value)
        {
            using (var image = new Bitmap(10, 10))
            {
                var item = new SubToolStripItem
                {
                    Image = image
                };

                item.ImageKey = value;
                Assert.Null(item.Image);
            }
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(ToolStripItemImageScaling))]
        public void ToolStripItem_ImageScaling_Set_GetReturnsExpected(ToolStripItemImageScaling value)
        {
            var item = new SubToolStripItem
            {
                ImageScaling = value
            };
            Assert.Equal(value, item.ImageScaling);

            // Set same.
            item.ImageScaling = value;
            Assert.Equal(value, item.ImageScaling);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(ToolStripItemImageScaling))]
        public void ToolStripItem_ImageScaling_SetWithOwner_GetReturnsExpected(ToolStripItemImageScaling value)
        {
            var owner = new ToolStrip();
            var item = new SubToolStripItem();
            owner.Items.Add(item);

            item.ImageScaling = value;
            Assert.Equal(value, item.ImageScaling);

            // Set same.
            item.ImageScaling = value;
            Assert.Equal(value, item.ImageScaling);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(ToolStripItemImageScaling))]
        public void ToolStripItem_ImageScaling_SetInvalid_ThrowsInvalidEnumArgumentException(ToolStripItemImageScaling value)
        {
            var item = new SubToolStripItem();
            Assert.Throws<InvalidEnumArgumentException>("value", () => item.ImageScaling = value);
        }

        public static IEnumerable<object[]> ImageTransparentColor_TestData()
        {
            foreach (Color color in new Color[] { Color.Empty, Color.Red })
            {
                yield return new object[] { null, color };
                yield return new object[] { new Bitmap(10, 10), color };
                yield return new object[] { Image.FromFile(Path.Combine("bitmaps", "nature24bits.gif")), color };
                yield return new object[] { Image.FromFile(Path.Combine("bitmaps", "10x16_one_entry_32bit.ico")), color };
            }
        }

        [Theory]
        [MemberData(nameof(ImageTransparentColor_TestData))]
        public void ToolStripItem_ImageTransparentColor_Set_GetReturnsExpected(Image image, Color value)
        {
            var item = new SubToolStripItem
            {
                Image = image
            };

            item.ImageTransparentColor = value;
            Assert.Equal(value, item.ImageTransparentColor);

            // Set same.
            item.ImageTransparentColor = value;
            Assert.Equal(value, item.ImageTransparentColor);
        }

        [Theory]
        [MemberData(nameof(ImageTransparentColor_TestData))]
        public void ToolStripItem_ImageTransparentColor_SetWithParent_GetReturnsExpected(Image image, Color value)
        {
            var parent = new ToolStrip();
            var item = new SubToolStripItem
            {
                Parent = parent,
                Image = image
            };

            item.ImageTransparentColor = value;
            Assert.Equal(value, item.ImageTransparentColor);

            // Set same.
            item.ImageTransparentColor = value;
            Assert.Equal(value, item.ImageTransparentColor);
        }

        public static IEnumerable<object[]> IsOnDropDown_TestData()
        {
            yield return new object[] { new SubToolStripItem(), false };
            yield return new object[] { new SubToolStripItem("text", null, null), false };
            yield return new object[] { new SubToolStripItem("text", null, null, "name"), false };

            var toolStrip = new ToolStrip();
            var toolStripItem = new SubToolStripItem();
            toolStrip.Items.Add(toolStripItem);
            yield return new object[] { toolStripItem, false };

            var toolStripDropDown = new ToolStripDropDown();
            var toolStripDropDownItem = new SubToolStripItem();
            toolStripDropDown.Items.Add(toolStripDropDownItem);
            yield return new object[] { toolStripDropDownItem, true };

            var toolStripParentItem = new SubToolStripItem
            {
                Parent = toolStrip
            };
            yield return new object[] { toolStripParentItem, false };

            var toolStripDropDownParentItem = new SubToolStripItem
            {
                Parent = toolStripDropDown
            };
            yield return new object[] { toolStripDropDownParentItem, true };
        }

        [Theory]
        [MemberData(nameof(IsOnDropDown_TestData))]
        public void ToolStripItem_IsOnDropDown_Get_ReturnsExpected(ToolStripItem item, bool expected)
        {
            Assert.Equal(expected, item.IsOnDropDown);
        }

        public static IEnumerable<object[]> IsOnOverflow_TestData()
        {
            yield return new object[] { new SubToolStripItem(), false };
            yield return new object[] { new SubToolStripItem("text", null, null), false };
            yield return new object[] { new SubToolStripItem("text", null, null, "name"), false };

            var toolStrip = new ToolStrip();
            var toolStripItem = new SubToolStripItem();
            toolStrip.Items.Add(toolStripItem);
            yield return new object[] { toolStripItem, false };

            var toolStripOverflowParent = new SubToolStripItem();
            var toolStripOverflow = new ToolStripOverflow(toolStripOverflowParent);
            yield return new object[] { toolStripOverflowParent, false };

            var overflowingToolStrip = new ToolStrip
            {
                Size = new Size(1, 2)
            };
            var overflowingToolStripItem = new SubToolStripItem();
            overflowingToolStrip.Items.Add(overflowingToolStripItem);
            overflowingToolStrip.LayoutEngine.Layout(overflowingToolStrip, null);
            yield return new object[] { overflowingToolStripItem, true };

            var noLayoutOverflowingToolStripItem = new SubToolStripItem();
            overflowingToolStrip.Items.Add(noLayoutOverflowingToolStripItem);
            yield return new object[] { noLayoutOverflowingToolStripItem, true };
        }

        [Theory]
        [MemberData(nameof(IsOnOverflow_TestData))]
        public void ToolStripItem_IsOnOverflow_Get_ReturnsExpected(ToolStripItem item, bool expected)
        {
            Assert.Equal(expected, item.IsOnOverflow);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetPaddingTheoryData))]
        public void ToolStripItem_Margin_Set_GetReturnsExpected(Padding value)
        {
            var item = new SubToolStripItem
            {
                Margin = value
            };
            Assert.Equal(value, item.Margin);

            // Set same.
            item.Margin = value;
            Assert.Equal(value, item.Margin);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(MergeAction))]
        public void ToolStripItem_MergeAction_Set_GetReturnsExpected(MergeAction value)
        {
            var item = new SubToolStripItem
            {
                MergeAction = value
            };
            Assert.Equal(value, item.MergeAction);

            // Set same.
            item.MergeAction = value;
            Assert.Equal(value, item.MergeAction);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(MergeAction))]
        public void ToolStripItem_MergeAction_SetInvalid_ThrowsInvalidEnumArgumentException(MergeAction value)
        {
            var item = new SubToolStripItem();
            Assert.Throws<InvalidEnumArgumentException>("value", () => item.MergeAction = value);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetIntTheoryData))]
        public void ToolStripItem_MergeIndex_Set_GetReturnsExpected(int value)
        {
            var item = new SubToolStripItem
            {
                MergeIndex = value
            };
            Assert.Equal(value, item.MergeIndex);

            // Set same.
            item.MergeIndex = value;
            Assert.Equal(value, item.MergeIndex);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringTheoryData))]
        public void ToolStripItem_Name_Set_GetReturnsExpected(string value)
        {
            var item = new SubToolStripItem
            {
                Name = value
            };
            Assert.Equal(value, item.Name);

            // Set same.
            item.Name = value;
            Assert.Equal(value, item.Name);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringTheoryData))]
        public void ToolStripItem_Name_SetInDesignMode_Nop(string value)
        {
            var mockSite = new Mock<ISite>();
            mockSite.Setup(s => s.DesignMode).Returns(true);
            var item = new SubToolStripItem
            {
                Site = mockSite.Object
            };

            item.Name = value;
            Assert.Empty(item.Name);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(ToolStripItemOverflow))]
        public void ToolStripItem_Overflow_Set_GetReturnsExpected(ToolStripItemOverflow value)
        {
            var item = new SubToolStripItem
            {
                Overflow = value
            };
            Assert.Equal(value, item.Overflow);

            // Set same.
            item.Overflow = value;
            Assert.Equal(value, item.Overflow);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(ToolStripItemOverflow))]
        public void ToolStripItem_Overflow_SetWithOwner_GetReturnsExpected(ToolStripItemOverflow value)
        {
            var owner = new ToolStrip();
            var item = new SubToolStripItem();
            owner.Items.Add(item);

            item.Overflow = value;
            Assert.Equal(value, item.Overflow);

            // Set same.
            item.Overflow = value;
            Assert.Equal(value, item.Overflow);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(ToolStripItemOverflow))]
        public void ToolStripItem_Overflow_SetInvalid_ThrowsInvalidEnumArgumentException(ToolStripItemOverflow value)
        {
            var item = new SubToolStripItem();
            Assert.Throws<InvalidEnumArgumentException>("value", () => item.Overflow = value);
        }

        [Fact]
        public void ToolStripItem_Owner_Set_GetReturnsExpected()
        {
            var owner = new ToolStrip();
            var otherOwner = new ToolStrip();
            var item = new SubToolStripItem
            {
                Owner = owner
            };
            Assert.Same(owner, item.Owner);
            Assert.Same(item, Assert.Single(owner.Items));

            // Set same.
            item.Owner = owner;
            Assert.Same(owner, item.Owner);
            Assert.Same(item, Assert.Single(owner.Items));
            Assert.Empty(otherOwner.Items);

            // Set different.
            item.Owner = otherOwner;
            Assert.Same(otherOwner, item.Owner);
            Assert.Empty(owner.Items);
            Assert.Same(item, Assert.Single(otherOwner.Items));

            // Set null.
            item.Owner = null;
            Assert.Null(item.Owner);
            Assert.Empty(owner.Items);
            Assert.Empty(otherOwner.Items);
        }

        public static IEnumerable<object[]> OwnerItem_TestData()
        {
            yield return new object[] { new SubToolStripItem(), null };
            yield return new object[] { new SubToolStripItem("text", null, null), null };
            yield return new object[] { new SubToolStripItem("text", null, null, "name"), null };

            var toolStrip = new ToolStrip();
            var toolStripItem = new SubToolStripItem();
            toolStrip.Items.Add(toolStripItem);
            yield return new object[] { toolStripItem, null };

            var toolStripDropDown = new ToolStripDropDown();
            var toolStripDropDownItem = new SubToolStripItem();
            toolStripDropDown.Items.Add(toolStripDropDownItem);
            yield return new object[] { toolStripDropDownItem, null };

            var toolStripDropDownWithOwnerItemOwnerItem = new SubToolStripItem();
            var toolStripDropDownWithOwnerItem = new ToolStripDropDown
            {
                OwnerItem = toolStripDropDownWithOwnerItemOwnerItem
            };
            var toolStripDropDownWithOwnerItemItem = new SubToolStripItem();
            toolStripDropDownWithOwnerItem.Items.Add(toolStripDropDownWithOwnerItemItem);
            yield return new object[] { toolStripDropDownWithOwnerItemItem, toolStripDropDownWithOwnerItemOwnerItem };

            var toolStripParent = new ToolStrip();
            var toolStripParentItem = new SubToolStripItem
            {
                Parent = toolStripParent
            };
            yield return new object[] { toolStripParentItem, null };

            var toolStripDropDownParent = new ToolStripDropDown();
            var toolStripDropDownParentItem = new SubToolStripItem
            {
                Parent = toolStripDropDownParent
            };
            yield return new object[] { toolStripDropDownParentItem, null };

            var toolStripDropDownWithOwnerItemParentOwnerItem = new SubToolStripItem();
            var toolStripDropDownWithOwnerItemParent = new ToolStripDropDown
            {
                OwnerItem = toolStripDropDownWithOwnerItemParentOwnerItem
            };
            var toolStripDropDownWithOwnerItemParentItem = new SubToolStripItem
            {
                Parent = toolStripDropDownWithOwnerItemParent
            };
            yield return new object[] { toolStripDropDownWithOwnerItemParentItem, toolStripDropDownWithOwnerItemParentOwnerItem };
        }

        [Theory]
        [MemberData(nameof(OwnerItem_TestData))]
        public void ToolStripItem_OwnerItem_Get_ReturnsExpected(ToolStripItem item, ToolStripItem expected)
        {
            Assert.Equal(expected, item.OwnerItem);
        }

        public static IEnumerable<object[]> Padding_TestData()
        {
            yield return new object[] { new Padding(), new Padding() };
            yield return new object[] { new Padding(1, 2, 3, 4), new Padding(1, 2, 3, 4) };
            yield return new object[] { new Padding(1), new Padding(1) };
            yield return new object[] { new Padding(-1, -2, -3, -4), new Padding(0, 0, 0, 0) };
        }

        [Theory]
        [MemberData(nameof(Padding_TestData))]
        public void ToolStripItem_Padding_Set_GetReturnsExpected(Padding value, Padding expected)
        {
            var item = new SubToolStripItem
            {
                Padding = value
            };
            Assert.Equal(expected, item.Padding);

            // Set same.
            item.Padding = value;
            Assert.Equal(expected, item.Padding);
        }

        public static IEnumerable<object[]> Parent_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new ToolStrip() };
        }

        [Theory]
        [MemberData(nameof(Parent_TestData))]
        public void ToolStripItem_Parent_Set_GetReturnsExpected(ToolStrip value)
        {
            var item = new SubToolStripItem
            {
                Parent = value
            };
            Assert.Same(value, item.Parent);
            if (value != null)
            {
                Assert.Empty(value.Items);
            }

            // Set same.
            item.Parent = value;
            Assert.Same(value, item.Parent);
            if (value != null)
            {
                Assert.Empty(value.Items);
            }
        }

        public static IEnumerable<object[]> RightToLeft_TestData()
        {
            yield return new object[] { new SubToolStripItem(), RightToLeft.Inherit };
            yield return new object[] { new SubToolStripItem("text", null, null), RightToLeft.Inherit };
            yield return new object[] { new SubToolStripItem("text", null, null, "name"), RightToLeft.Inherit };

            foreach (RightToLeft rightToLeft in Enum.GetValues(typeof(RightToLeft)))
            {
                RightToLeft expected = rightToLeft == RightToLeft.Inherit ? RightToLeft.No : rightToLeft;

                var toolStrip = new ToolStrip
                {
                    RightToLeft = rightToLeft
                };
                var toolStripItem = new SubToolStripItem();
                toolStrip.Items.Add(toolStripItem);
                yield return new object[] { toolStripItem, expected };

                var toolStripParent = new ToolStrip
                {
                    RightToLeft = rightToLeft
                };
                var toolStripParentItem = new SubToolStripItem
                {
                    Parent = toolStripParent
                };
                yield return new object[] { toolStripParentItem, expected };
            }

            var toolStripRightToLeft = new ToolStrip
            {
                RightToLeft = RightToLeft.Yes
            };
            var toolStripLeftToRight = new ToolStrip
            {
                RightToLeft = RightToLeft.No
            };
            var item = new SubToolStripItem
            {
                Owner = toolStripRightToLeft
            };
            toolStripLeftToRight.Items.Add(item);
            yield return new object[] { item, RightToLeft.No };
        }

        [Theory]
        [MemberData(nameof(RightToLeft_TestData))]
        public void ToolStripItem_RightToLeft_Get_ReturnsExpected(ToolStripItem item, RightToLeft expected)
        {
            Assert.Equal(expected, item.RightToLeft);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(RightToLeft))]
        public void ToolStripItem_RightToLeft_Set_GetReturnsExpected(RightToLeft value)
        {
            var item = new SubToolStripItem
            {
                RightToLeft = value
            };
            Assert.Equal(value, item.RightToLeft);

            // Set same.
            item.RightToLeft = value;
            Assert.Equal(value, item.RightToLeft);
        }

        [Fact]
        public void ToolStripItem_RightToLeft_SetWithHandler_CallsRightToLeftChanged()
        {
            var item = new SubToolStripItem();
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

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(RightToLeft))]
        public void ToolStripItem_RightToLeft_SetInvalid_ThrowsInvalidEnumArgumentException(RightToLeft value)
        {
            var item = new SubToolStripItem();
            Assert.Throws<InvalidEnumArgumentException>("value", () => item.RightToLeft = value);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ToolStripItem_RightToLeftAutoMirrorImage_Set_GetReturnsExpected(bool value)
        {
            var item = new SubToolStripItem
            {
                RightToLeftAutoMirrorImage = value
            };
            Assert.Equal(value, item.RightToLeftAutoMirrorImage);

            // Set same.
            item.RightToLeftAutoMirrorImage = value;
            Assert.Equal(value, item.RightToLeftAutoMirrorImage);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ToolStripItem_RightToLeftAutoMirrorImage_SetWithParent_GetReturnsExpected(bool value)
        {
            var parent = new ToolStrip();
            var item = new SubToolStripItem
            {
                Parent = parent
            };

            item.RightToLeftAutoMirrorImage = value;
            Assert.Equal(value, item.RightToLeftAutoMirrorImage);

            // Set same.
            item.RightToLeftAutoMirrorImage = value;
            Assert.Equal(value, item.RightToLeftAutoMirrorImage);
        }

        public static IEnumerable<object[]> Selected_Get_TestData()
        {
            yield return new object[] { new SubToolStripItem(), false };
            yield return new object[] { new SubToolStripItem("text", null, null), false };
            yield return new object[] { new SubToolStripItem("text", null, null, "name"), false };

            yield return new object[] { new CannotSelectToolStripItem(), false };

            var mockSite = new Mock<ISite>();
            mockSite.Setup(s => s.DesignMode).Returns(true);
            yield return new object[] { new SubToolStripItem { Site = mockSite.Object }, false };

            var toolStripParent = new ToolStrip();
            var toolStripParentItem = new SubToolStripItem
            {
                Parent = toolStripParent
            };
            yield return new object[] { toolStripParentItem, false };
        }

        [Theory]
        [MemberData(nameof(Selected_Get_TestData))]
        public void ToolStripItem_Selected_Get_ReturnsExpected(ToolStripItem item, bool expected)
        {
            Assert.Equal(expected, item.Selected);
        }

        public static IEnumerable<object[]> ShowKeyboardCues_TestData()
        {
            yield return new object[] { new SubToolStripItem(), false };
            yield return new object[] { new SubToolStripItem("text", null, null), false };
            yield return new object[] { new SubToolStripItem("text", null, null, "name"), false };

            var mockSite = new Mock<ISite>();
            mockSite.Setup(s => s.DesignMode).Returns(true);
            yield return new object[] { new SubToolStripItem { Site = mockSite.Object }, true };
        }

        [Theory]
        [MemberData(nameof(ShowKeyboardCues_TestData))]
        public void ToolStripItem_ShowKeyboardCues_Get_ReturnsExpected(ToolStripItem item, bool expected)
        {
            Assert.Equal(expected, item.ShowKeyboardCues);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetSizeTheoryData))]
        public void ToolStripItem_Size_Set_GetReturnsExpected(Size value)
        {
            var item = new SubToolStripItem
            {
                Size = value
            };
            Assert.Equal(value, item.Size);

            // Set same.
            item.Size = value;
            Assert.Equal(value, item.Size);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringWithNullTheoryData))]
        public void ToolStripItem_Tag_Set_GetReturnsExpected(string value)
        {
            var item = new SubToolStripItem
            {
                Tag = value
            };
            Assert.Same(value, item.Tag);

            // Set same.
            item.Tag = value;
            Assert.Same(value, item.Tag);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringWithNullTheoryData))]
        public void ToolStripItem_Text_Set_GetReturnsExpected(string value)
        {
            var item = new SubToolStripItem
            {
                Text = value
            };
            Assert.Same(value, item.Text);

            // Set same.
            item.Text = value;
            Assert.Same(value, item.Text);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringWithNullTheoryData))]
        public void ToolStripItem_Text_SetWithOwner_GetReturnsExpected(string value)
        {
            var owner = new ToolStrip();
            var item = new SubToolStripItem();
            owner.Items.Add(item);

            item.Text = value;
            Assert.Same(value, item.Text);

            // Set same.
            item.Text = value;
            Assert.Same(value, item.Text);
        }

        [Fact]
        public void ToolStripItem_Text_SetWithHandler_CallsForeColorChanged()
        {
            var item = new SubToolStripItem();
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

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(ContentAlignment))]
        public void ToolStripItem_TextAlign_Set_GetReturnsExpected(ContentAlignment value)
        {
            var item = new SubToolStripItem
            {
                TextAlign = value
            };
            Assert.Equal(value, item.TextAlign);

            // Set same.
            item.TextAlign = value;
            Assert.Equal(value, item.TextAlign);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(ContentAlignment))]
        public void ToolStripItem_TextAlign_SetWithOwner_GetReturnsExpected(ContentAlignment value)
        {
            var owner = new ToolStrip();
            var item = new SubToolStripItem();
            owner.Items.Add(item);

            item.TextAlign = value;
            Assert.Equal(value, item.TextAlign);

            // Set same.
            item.TextAlign = value;
            Assert.Equal(value, item.TextAlign);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(ContentAlignment))]
        public void ToolStripItem_TextAlign_SetInvalid_ThrowsInvalidEnumArgumentException(ContentAlignment value)
        {
            var item = new SubToolStripItem();
            Assert.Throws<InvalidEnumArgumentException>("value", () => item.TextAlign = value);
        }

        public static IEnumerable<object[]> TextDirection_Get_TestData()
        {
            yield return new object[] { new SubToolStripItem(), ToolStripTextDirection.Horizontal };
            yield return new object[] { new SubToolStripItem("text", null, null), ToolStripTextDirection.Horizontal };
            yield return new object[] { new SubToolStripItem("text", null, null, "name"), ToolStripTextDirection.Horizontal };

            var toolStrip = new ToolStrip
            {
                TextDirection = ToolStripTextDirection.Vertical270
            };
            var toolStripItem = new SubToolStripItem();
            toolStrip.Items.Add(toolStripItem);
            yield return new object[] { toolStripItem, ToolStripTextDirection.Vertical270 };

            var toolStripParent = new ToolStrip
            {
                TextDirection = ToolStripTextDirection.Vertical90
            };
            var toolStripParentItem = new SubToolStripItem
            {
                Parent = toolStripParent
            };
            yield return new object[] { toolStripParentItem, ToolStripTextDirection.Vertical90 };

            var toolStripOwnerParentItem = new SubToolStripItem();
            toolStrip.Items.Add(toolStripOwnerParentItem);
            toolStripOwnerParentItem.Parent = toolStripParent;
            yield return new object[] { toolStripOwnerParentItem, ToolStripTextDirection.Vertical90 };
        }

        [Theory]
        [MemberData(nameof(TextDirection_Get_TestData))]
        public void ToolStripItem_TextDirection_Get_ReturnsExpected(ToolStripItem item, ToolStripTextDirection expected)
        {
            Assert.Equal(expected, item.TextDirection);
        }

        public static IEnumerable<object[]> TextDirection_Set_TestData()
        {
            yield return new object[] { ToolStripTextDirection.Inherit, ToolStripTextDirection.Horizontal };
            yield return new object[] { ToolStripTextDirection.Horizontal, ToolStripTextDirection.Horizontal };
            yield return new object[] { ToolStripTextDirection.Vertical90, ToolStripTextDirection.Vertical90 };
            yield return new object[] { ToolStripTextDirection.Vertical270, ToolStripTextDirection.Vertical270 };
        }

        [Theory]
        [MemberData(nameof(TextDirection_Set_TestData))]
        public void ToolStripItem_TextDirection_Set_GetReturnsExpected(ToolStripTextDirection value, ToolStripTextDirection expected)
        {
            var item = new SubToolStripItem
            {
                TextDirection = value
            };
            Assert.Equal(expected, item.TextDirection);

            // Set same.
            item.TextDirection = value;
            Assert.Equal(expected, item.TextDirection);
        }

        [Theory]
        [MemberData(nameof(TextDirection_Set_TestData))]
        public void ToolStripItem_TextDirection_SetWithOwner_GetReturnsExpected(ToolStripTextDirection value, ToolStripTextDirection expected)
        {
            var owner = new ToolStrip();
            var item = new SubToolStripItem();
            owner.Items.Add(item);

            item.TextDirection = value;
            Assert.Equal(expected, item.TextDirection);

            // Set same.
            item.TextDirection = value;
            Assert.Equal(expected, item.TextDirection);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(ToolStripTextDirection))]
        public void ToolStripItem_TextDirection_SetInvalid_ThrowsInvalidEnumArgumentException(ToolStripTextDirection value)
        {
            var item = new SubToolStripItem();
            Assert.Throws<InvalidEnumArgumentException>("value", () => item.TextDirection = value);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(TextImageRelation))]
        public void ToolStripItem_TextImageRelation_Set_GetReturnsExpected(TextImageRelation value)
        {
            var item = new SubToolStripItem
            {
                TextImageRelation = value
            };
            Assert.Equal(value, item.TextImageRelation);

            // Set same.
            item.TextImageRelation = value;
            Assert.Equal(value, item.TextImageRelation);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(TextImageRelation))]
        public void ToolStripItem_TextImageRelation_SetWithOwner_GetReturnsExpected(TextImageRelation value)
        {
            var owner = new ToolStrip();
            var item = new SubToolStripItem();
            owner.Items.Add(item);

            item.TextImageRelation = value;
            Assert.Equal(value, item.TextImageRelation);

            // Set same.
            item.TextImageRelation = value;
            Assert.Equal(value, item.TextImageRelation);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(TextImageRelation))]
        [InlineData((TextImageRelation)3)]
        [InlineData((TextImageRelation)5)]
        [InlineData((TextImageRelation)6)]
        [InlineData((TextImageRelation)7)]
        public void ToolStripItem_TextImageRelation_SetInvalid_ThrowsInvalidEnumArgumentException(TextImageRelation value)
        {
            var item = new SubToolStripItem();
            Assert.Throws<InvalidEnumArgumentException>("value", () => item.TextImageRelation = value);
        }

        public static IEnumerable<object[]> ToolTipText_Get_TestData()
        {
            yield return new object[] { new SubToolStripItem(), null };
            yield return new object[] { new SubToolStripItem("text", null, null), null };
            yield return new object[] { new SubToolStripItem("text", null, null, "name"), null };
            yield return new object[] { new SubToolStripItem { AutoToolTip = false, Text = "reasonable" }, null };

            yield return new object[] { new SubToolStripItem { AutoToolTip = true, Text = null }, null };
            yield return new object[] { new SubToolStripItem { AutoToolTip = true, Text = string.Empty }, string.Empty };
            yield return new object[] { new SubToolStripItem { AutoToolTip = true, Text = "reasonable" }, "reasonable" };
            yield return new object[] { new SubToolStripItem { AutoToolTip = true, Text = "&" }, "&" };
            yield return new object[] { new SubToolStripItem { AutoToolTip = true, Text = "&&" }, "&&" };

            yield return new object[] { new SubToolStripItem { AutoToolTip = true, Text = "&" }, "&" };
            yield return new object[] { new SubToolStripItem { AutoToolTip = true, Text = "&T" }, "T" };
            yield return new object[] { new SubToolStripItem { AutoToolTip = true, Text = "&Text" }, "Text" };
            yield return new object[] { new SubToolStripItem { AutoToolTip = true, Text = "&Text&Text2" }, "&Text&Text2" };
        }

        [Theory]
        [MemberData(nameof(ToolTipText_Get_TestData))]
        public void ToolStripItem_ToolTipText_Get_ReturnsExpected(ToolStripItem item, string expected)
        {
            Assert.Equal(expected, item.ToolTipText);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringTheoryData))]
        public void ToolStripItem_ToolTipText_Set_GetReturnsExpected(string value)
        {
            var item = new SubToolStripItem
            {
                ToolTipText = value
            };
            Assert.Equal(value, item.ToolTipText);

            // Set same.
            item.ToolTipText = value;
            Assert.Equal(value, item.ToolTipText);
        }

        public static IEnumerable<object[]> Visible_Get_TestData()
        {
            yield return new object[] { new SubToolStripItem(), false };
            yield return new object[] { new SubToolStripItem("text", null, null), false };
            yield return new object[] { new SubToolStripItem("text", null, null, "name"), false };

            yield return new object[] { new SubToolStripItem { Available = true }, false };

            foreach (bool visible in new bool[] { true, false })
            {
                var toolStripParentVisible = new ToolStrip
                {
                    Visible = visible
                };
                var toolStripParentVisibleItem = new SubToolStripItem
                {
                    Available = true,
                    Parent = toolStripParentVisible
                };
                yield return new object[] { toolStripParentVisibleItem, visible };

                var toolStripParentNotVisibleItem = new SubToolStripItem
                {
                    Available = false,
                    Parent = toolStripParentVisible
                };
                yield return new object[] { toolStripParentNotVisibleItem, false };
            }
        }

        [Theory]
        [MemberData(nameof(Visible_Get_TestData))]
        public void ToolStripItem_Visible_Get_ReturnsExpected(ToolStripItem item, bool expected)
        {
            Assert.Equal(expected, item.Visible);
        }

        public static IEnumerable<object[]> Visible_Set_TestData()
        {
            yield return new object[] { true, true };
            yield return new object[] { true, false };
            yield return new object[] { false, true };
            yield return new object[] { false, false };
        }

        [Theory]
        [MemberData(nameof(Visible_Set_TestData))]
        public void ToolStripItem_Visible_Set_GetReturnsExpected(bool available, bool value)
        {
            var item = new SubToolStripItem
            {
                Available = available,
                Visible = value
            };
            Assert.False(item.Visible);

            // Set same.
            item.Visible = value;
            Assert.False(item.Visible);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ToolStripItem_Visible_SetWithParent_GetReturnsExpected(bool value)
        {
            var parent = new ToolStrip();
            var item = new SubToolStripItem
            {
                Parent = parent,

                Visible = value
            };
            Assert.Equal(value, item.Visible);

            // Set same.
            item.Visible = value;
            Assert.Equal(value, item.Visible);
        }

        [Fact]
        public void ToolStripItem_Visible_SetWithHandler_CallsAvailableChanged()
        {
            var item = new SubToolStripItem();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(item, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            item.AvailableChanged += handler;

            // Set different.
            item.Visible = false;
            Assert.False(item.Visible);
            Assert.Equal(1, callCount);

            // Set same.
            item.Visible = false;
            Assert.False(item.Visible);
            Assert.Equal(1, callCount);

            // Set different.
            item.Visible = true;
            Assert.False(item.Visible);
            Assert.Equal(2, callCount);

            // Remove handler.
            item.AvailableChanged -= handler;
            item.Visible = false;
            Assert.False(item.Visible);
            Assert.Equal(2, callCount);
        }

        [Fact]
        public void ToolStripItem_Visible_SetWithHandler_CallsVisibleChanged()
        {
            var item = new SubToolStripItem();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(item, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            item.VisibleChanged += handler;

            // Set different.
            item.Visible = false;
            Assert.False(item.Visible);
            Assert.Equal(1, callCount);

            // Set same.
            item.Visible = false;
            Assert.False(item.Visible);
            Assert.Equal(1, callCount);

            // Set different.
            item.Visible = true;
            Assert.False(item.Visible);
            Assert.Equal(2, callCount);

            // Remove handler.
            item.VisibleChanged -= handler;
            item.Visible = false;
            Assert.False(item.Visible);
            Assert.Equal(2, callCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetIntTheoryData))]
        public void ToolStripItem_Width_Set_GetReturnsExpected(int value)
        {
            var item = new SubToolStripItem
            {
                Width = value
            };
            Assert.Equal(value, item.Width);

            // Set same.
            item.Width = value;
            Assert.Equal(value, item.Width);
        }

        [Fact]
        public void ToolStripItem_Dispose_Invoke_Success()
        {
            using (var image = new Bitmap(10, 10))
            {
                var item = new SubToolStripItem
                {
                    Image = image
                };
                item.Dispose();
                Assert.False(item.IsDisposed);
                Assert.Null(item.Image);

                // Dispose multiple times.
                item.Dispose();
                Assert.False(item.IsDisposed);
                Assert.Null(item.Image);
            }
        }

        [Fact]
        public void ToolStripItem_Dispose_InvokeWithOwner_Success()
        {
            using (var image = new Bitmap(10, 10))
            {
                var owner = new ToolStrip();
                var item = new SubToolStripItem
                {
                    Image = image
                };
                owner.Items.Add(item);

                item.Dispose();
                Assert.True(item.IsDisposed);
                Assert.Null(item.Image);
                Assert.Null(item.Owner);
                Assert.Empty(owner.Items);

                // Dispose multiple times.
                item.Dispose();
                Assert.True(item.IsDisposed);
                Assert.Null(item.Image);
                Assert.Null(item.Owner);
                Assert.Empty(owner.Items);
            }
        }

        public static IEnumerable<object[]> GetCurrentParent_TestData()
        {
            yield return new object[] { new SubToolStripItem(), null };

            var parent = new ToolStrip();
            yield return new object[] { new SubToolStripItem { Parent = parent }, parent };
        }

        [Theory]
        [MemberData(nameof(GetCurrentParent_TestData))]
        public void ToolStripItem_GetCurrentParent_Invoke_ReturnsExpected(ToolStripItem item, ToolStrip expected)
        {
            Assert.Equal(expected, item.GetCurrentParent());
        }

        [Theory]
        [InlineData('a')]
        [InlineData('\0')]
        public void ToolStripItem_IsInputChar_Invoke_ReturnsFalse(char charCode)
        {
            var item = new SubToolStripItem();
            Assert.False(item.IsInputChar(charCode));
        }

        [Theory]
        [InlineData(Keys.None)]
        [InlineData((Keys)(Keys.None - 1))]
        public void ToolStripItem_IsInputKey_Invoke_ReturnsFalse(Keys keyData)
        {
            var item = new SubToolStripItem();
            Assert.False(item.IsInputKey(keyData));
        }

        public static IEnumerable<object[]> Invalidate_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new ToolStrip() };
        }

        [Theory]
        [MemberData(nameof(Invalidate_TestData))]
        public void ToolStripItem_Invalidate_Invoke_Success(ToolStrip parent)
        {
            var item = new SubToolStripItem
            {
                Parent = parent
            };
            item.Invalidate();
        }

        [Fact]
        public void ToolStripItem_OnAvailableChanged_Invoke_CallsHandler()
        {
            var item = new SubToolStripItem();
            var eventArgs = new EventArgs();
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

        public static IEnumerable<object[]> OnBackColorChanged_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new ToolStrip() };
        }

        [Theory]
        [MemberData(nameof(OnBackColorChanged_TestData))]
        public void ToolStripItem_OnBackColorChanged_Invoke_CallsHandler(ToolStrip parent)
        {
            var item = new SubToolStripItem
            {
                Parent = parent
            };
            var eventArgs = new EventArgs();
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

        [Fact]
        public void ToolStripItem_OnBoundsChanged_Invoke_Success()
        {
            var item = new SubToolStripItem();
            item.OnBoundsChanged();
        }

        [Fact]
        public void ToolStripItem_OnClick_Invoke_CallsHandler()
        {
            var item = new SubToolStripItem();
            var eventArgs = new EventArgs();
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

            // Remove handler.
            item.Click -= handler;
            item.OnClick(eventArgs);
            Assert.Equal(1, callCount);
        }

        [Fact]
        public void ToolStripItem_OnDisplayStyleChanged_Invoke_CallsHandler()
        {
            var item = new SubToolStripItem();
            var eventArgs = new EventArgs();
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

        [Fact]
        public void ToolStripItem_OnDoubleClick_Invoke_CallsHandler()
        {
            var item = new SubToolStripItem();
            var eventArgs = new EventArgs();
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

        [Fact]
        public void ToolStripItem_OnDragDrop_Invoke_CallsHandler()
        {
            var item = new SubToolStripItem();
            var eventArgs = new DragEventArgs(null, 0, 0, 0, DragDropEffects.None, DragDropEffects.None);
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

        [Fact]
        public void ToolStripItem_OnDragDrop_InvokeOnInterface_CallsHandler()
        {
            var item = new SubToolStripItem();
            var eventArgs = new DragEventArgs(null, 0, 0, 0, DragDropEffects.None, DragDropEffects.None);
            int callCount = 0;
            DragEventHandler handler = (sender, e) =>
            {
                Assert.Same(item, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            item.DragDrop += handler;
            ((IDropTarget)item).OnDragDrop(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            item.DragDrop -= handler;
            ((IDropTarget)item).OnDragDrop(eventArgs);
            Assert.Equal(1, callCount);
        }

        [Fact]
        public void ToolStripItem_OnDragEnter_Invoke_CallsHandler()
        {
            var item = new SubToolStripItem();
            var eventArgs = new DragEventArgs(null, 0, 0, 0, DragDropEffects.None, DragDropEffects.None);
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

        [Fact]
        public void ToolStripItem_OnDragEnter_InvokeOnInterface_CallsHandler()
        {
            var item = new SubToolStripItem();
            var eventArgs = new DragEventArgs(null, 0, 0, 0, DragDropEffects.None, DragDropEffects.None);
            int callCount = 0;
            DragEventHandler handler = (sender, e) =>
            {
                Assert.Same(item, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            item.DragEnter += handler;
            ((IDropTarget)item).OnDragEnter(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            item.DragEnter -= handler;
            ((IDropTarget)item).OnDragEnter(eventArgs);
            Assert.Equal(1, callCount);
        }

        [Fact]
        public void ToolStripItem_OnDragLeave_Invoke_CallsHandler()
        {
            var item = new SubToolStripItem();
            var eventArgs = new EventArgs();
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

        [Fact]
        public void ToolStripItem_OnDragLeave_InvokeOnInterface_CallsHandler()
        {
            var item = new SubToolStripItem();
            var eventArgs = new EventArgs();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(item, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            item.DragLeave += handler;
            ((IDropTarget)item).OnDragLeave(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            item.DragLeave -= handler;
            ((IDropTarget)item).OnDragLeave(eventArgs);
            Assert.Equal(1, callCount);
        }

        [Fact]
        public void ToolStripItem_OnDragOver_Invoke_CallsHandler()
        {
            var item = new SubToolStripItem();
            var eventArgs = new DragEventArgs(null, 0, 0, 0, DragDropEffects.None, DragDropEffects.None);
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

        [Fact]
        public void ToolStripItem_OnDragOver_InvokeOnInterface_CallsHandler()
        {
            var item = new SubToolStripItem();
            var eventArgs = new DragEventArgs(null, 0, 0, 0, DragDropEffects.None, DragDropEffects.None);
            int callCount = 0;
            DragEventHandler handler = (sender, e) =>
            {
                Assert.Same(item, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            item.DragOver += handler;
            ((IDropTarget)item).OnDragOver(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            item.DragOver -= handler;
            ((IDropTarget)item).OnDragOver(eventArgs);
            Assert.Equal(1, callCount);
        }

        [Fact]
        public void ToolStripItem_OnEnabledChanged_Invoke_CallsHandler()
        {
            var item = new SubToolStripItem();
            var eventArgs = new EventArgs();
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

        [Fact]
        public void ToolStripItem_OnFontChanged_Invoke_Success()
        {
            var item = new SubToolStripItem();
            var eventArgs = new EventArgs();
            item.OnFontChanged(eventArgs);
        }

        public static IEnumerable<object[]> OnForeColorChanged_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new ToolStrip() };
        }

        [Theory]
        [MemberData(nameof(OnForeColorChanged_TestData))]
        public void ToolStripItem_OnForeColorChanged_Invoke_CallsHandler(ToolStrip parent)
        {
            var item = new SubToolStripItem
            {
                Parent = parent
            };
            var eventArgs = new EventArgs();
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

        [Fact]
        public void ToolStripItem_OnGiveFeedback_Invoke_CallsHandler()
        {
            var item = new SubToolStripItem();
            var eventArgs = new GiveFeedbackEventArgs(DragDropEffects.None, useDefaultCursors: true);
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

        [Fact]
        public void ToolStripItem_OnLayout_Invoke_Success()
        {
            var item = new SubToolStripItem();
            var eventArgs = new LayoutEventArgs(item, "property");
            item.OnLayout(eventArgs);
        }

        [Fact]
        public void ToolStripItem_OnLocationChanged_Invoke_CallsHandler()
        {
            var item = new SubToolStripItem();
            var eventArgs = new EventArgs();
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

        [Fact]
        public void ToolStripItem_OnMouseDown_Invoke_Nop()
        {
            var item = new SubToolStripItem();
            var eventArgs = new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0);
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

        [Fact]
        public void ToolStripItem_OnMouseEnter_Invoke_Nop()
        {
            var item = new SubToolStripItem();
            var eventArgs = new EventArgs();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(item, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            item.MouseEnter += handler;
            item.OnMouseEnter(eventArgs);
            Assert.Equal(0, callCount);

            // Remove handler.
            item.MouseEnter -= handler;
            item.OnMouseEnter(eventArgs);
            Assert.Equal(0, callCount);
        }

        [Fact]
        public void ToolStripItem_OnMouseHover_Invoke_Nop()
        {
            var item = new SubToolStripItem();
            var eventArgs = new EventArgs();
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

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringTheoryData))]
        public void ToolStripItem_OnMouseHover_InvokeWithParent_Nop(string toolTipText)
        {
            var parent = new ToolStrip();
            var item = new SubToolStripItem
            {
                ToolTipText = toolTipText,
                Parent = parent
            };
            var eventArgs = new EventArgs();
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

        [Fact]
        public void ToolStripItem_OnMouseLeave_Invoke_Nop()
        {
            var item = new SubToolStripItem();
            var eventArgs = new EventArgs();
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

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringTheoryData))]
        public void ToolStripItem_OnMouseLeave_InvokeWithParent_Nop(string toolTipText)
        {
            var parent = new ToolStrip();
            var item = new SubToolStripItem
            {
                ToolTipText = toolTipText,
                Parent = parent
            };
            var eventArgs = new EventArgs();
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

        [Fact]
        public void ToolStripItem_OnMouseMove_Invoke_Nop()
        {
            var item = new SubToolStripItem();
            var eventArgs = new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0);
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

        [Fact]
        public void ToolStripItem_OnMouseUp_Invoke_Nop()
        {
            var item = new SubToolStripItem();
            var eventArgs = new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0);
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

        [Theory]
        [InlineData(RightToLeft.Inherit, RightToLeft.Inherit, 1)]
        [InlineData(RightToLeft.Inherit, RightToLeft.Yes, 0)]
        [InlineData(RightToLeft.Inherit, RightToLeft.No, 0)]
        [InlineData(RightToLeft.Yes, RightToLeft.Inherit, 1)]
        [InlineData(RightToLeft.Yes, RightToLeft.Yes, 0)]
        [InlineData(RightToLeft.Yes, RightToLeft.No, 0)]
        [InlineData(RightToLeft.No, RightToLeft.Inherit, 1)]
        [InlineData(RightToLeft.No, RightToLeft.Yes, 0)]
        [InlineData(RightToLeft.No, RightToLeft.No, 0)]
        public void ToolStripItem_OnOwnerChanged_InvokeWithOwner_CallsHandler(RightToLeft ownerRightToLeft, RightToLeft itemRightToLeft, int expectedRightToLeftChangedCallCount)
        {
            var owner = new ToolStrip
            {
                RightToLeft = ownerRightToLeft
            };
            var item = new SubToolStripItem
            {
                RightToLeft = itemRightToLeft
            };
            owner.Items.Add(item);

            var eventArgs = new EventArgs();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(item, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };
            int rightToLeftChangedCallCount = 0;
            EventHandler rightToLeftChangedHandler = (sender, e) =>
            {
                Assert.Same(item, sender);
                Assert.NotSame(eventArgs, e);
                rightToLeftChangedCallCount++;
            };
            item.RightToLeftChanged += rightToLeftChangedHandler;

            // Call with handler.
            item.OwnerChanged += handler;
            item.OnOwnerChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(expectedRightToLeftChangedCallCount, rightToLeftChangedCallCount);

            // Remove handler.
            item.OwnerChanged -= handler;
            item.RightToLeftChanged -= rightToLeftChangedHandler;
            item.OnOwnerChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(expectedRightToLeftChangedCallCount, rightToLeftChangedCallCount);
        }

        [Fact]
        public void ToolStripItem_OnOwnerFontChanged_Invoke_Success()
        {
            var item = new SubToolStripItem();
            var eventArgs = new EventArgs();

            // Call without font.
            item.OnOwnerFontChanged(eventArgs);

            // Call with font.
            item.Font = SystemFonts.MenuFont;
            item.OnOwnerFontChanged(eventArgs);
        }

        [Fact]
        public void ToolStripItem_OnPaint_Invoke_Success()
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                var item = new SubToolStripItem();
                var eventArgs = new PaintEventArgs(graphics, Rectangle.Empty);

                item.OnPaint(eventArgs);
            }
        }

        [Fact]
        public void ToolStripItem_OnParentBackColorChanged_Invoke_CallsHandler()
        {
            var item = new SubToolStripItem();
            var eventArgs = new EventArgs();
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
            foreach (bool allowDrop in new bool[] { true, false })
            {
                yield return new object[] { allowDrop, null, null };
                yield return new object[] { allowDrop, null, new ToolStrip() };
                yield return new object[] { allowDrop, new ToolStrip(), null };
                yield return new object[] { allowDrop, new ToolStrip(), new ToolStrip() };
            }
        }

        [Theory]
        [MemberData(nameof(OnParentChanged_TestData))]
        public void ToolStripItem_OnParentChanged_Invoke_Success(bool allowDrop, ToolStrip oldParent, ToolStrip newParent)
        {
            var item = new SubToolStripItem
            {
                AllowDrop = allowDrop
            };
            item.OnParentChanged(oldParent, newParent);
        }

        [Fact]
        public void ToolStripItem_OnParentEnabledChanged_Invoke_CallsHandler()
        {
            var item = new SubToolStripItem();
            var eventArgs = new EventArgs();
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

        [Fact]
        public void ToolStripItem_OnParentForeColorChanged_Invoke_CallsHandler()
        {
            var item = new SubToolStripItem();
            var eventArgs = new EventArgs();
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

        [Fact]
        public void ToolStripItem_OnRightToLeftChanged_Invoke_CallsHandler()
        {
            var item = new SubToolStripItem();
            var eventArgs = new EventArgs();
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

        [Theory]
        [InlineData(RightToLeft.Inherit, 1)]
        [InlineData(RightToLeft.Yes, 0)]
        [InlineData(RightToLeft.No, 0)]
        public void ToolStripItem_OnParentRightToLeftChanged_InvokeWithRightToLeft_CallsHandler(RightToLeft rightToLeft, int expectedCallCount)
        {
            var item = new SubToolStripItem
            {
                RightToLeft = rightToLeft
            };
            var eventArgs = new EventArgs();
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

        [Fact]
        public void ToolStripItem_OnParentRightToLeftChanged_InvokeWithoutRightToLeft_CallsHandler()
        {
            var item = new SubToolStripItem();
            var eventArgs = new EventArgs();
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

        [Fact]
        public void ToolStripItem_OnQueryContinueDrag_InvokeWithOwner_CallsHandler()
        {
            var owner = new ToolStrip();
            var item = new SubToolStripItem();
            owner.Items.Add(item);

            var eventArgs = new QueryContinueDragEventArgs(0, true, DragAction.Cancel);
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

        [Fact]
        public void ToolStripItem_OnRightToLeftChanged_InvokeWithOwner_CallsHandler()
        {
            var owner = new ToolStrip();
            var item = new SubToolStripItem();
            owner.Items.Add(item);

            var eventArgs = new EventArgs();
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

        [Fact]
        public void ToolStripItem_OnTextChanged_Invoke_CallsHandler()
        {
            var item = new SubToolStripItem();
            var eventArgs = new EventArgs();
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

        [Fact]
        public void ToolStripItem_OnTextChanged_InvokeWithOwner_CallsHandler()
        {
            var owner = new ToolStrip();
            var item = new SubToolStripItem();
            owner.Items.Add(item);
            var eventArgs = new EventArgs();
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

        [Fact]
        public void ToolStripItem_OnVisibleChanged_Invoke_CallsHandler()
        {
            var item = new SubToolStripItem();
            var eventArgs = new EventArgs();
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

        [Fact]
        public void ToolStripItem_OnVisibleChanged_InvokeWithOwner_CallsHandler()
        {
            var owner = new ToolStrip();
            var item = new SubToolStripItem();
            owner.Items.Add(item);

            var eventArgs = new EventArgs();
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

        [Fact]
        public void ToolStripItem_OnVisibleChanged_InvokeWithDisposedOwner_CallsHandler()
        {
            var owner = new ToolStrip();
            var item = new SubToolStripItem();
            owner.Items.Add(item);
            owner.Dispose();

            var eventArgs = new EventArgs();
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

        public static IEnumerable<object[]> PerformClick_TestData()
        {
            yield return new object[] { true, true, 1 };
            yield return new object[] { true, false, 0 };
            yield return new object[] { false, true, 0 };
            yield return new object[] { false, false, 0 };
        }

        [Theory]
        [MemberData(nameof(PerformClick_TestData))]
        public void ToolStripItem_PerformClick_Invoke_Success(bool enabled, bool available, int expectedClickCount)
        {
            var item = new SubToolStripItem
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
        }

        [Fact]
        public void ToolStripItem_PerformClick_InvokeDesignMode_Success()
        {
            var mockSite = new Mock<ISite>();
            mockSite.Setup(s => s.DesignMode).Returns(true);
            var item = new SubToolStripItem
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
        }

        [Fact]
        public void ToolStripItem_PerformClick_InvokeWithOwner_Success()
        {
            var owner = new ToolStrip();
            var item = new SubToolStripItem();
            owner.Items.Add(item);

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

            // Remove handler.
            item.Click -= handler;
            item.PerformClick();
            Assert.Equal(1, callCount);
            Assert.Equal(2, itemClickedCallCount);

            // Remove other handler.
            owner.ItemClicked -= itemClickedHandler;
            item.PerformClick();
            Assert.Equal(1, callCount);
            Assert.Equal(2, itemClickedCallCount);
        }

        [Fact]
        public void ToolStripItem_PerformClick_InvokeWithParent_Success()
        {
            var parent = new ToolStrip();
            var item = new SubToolStripItem
            {
                Parent = parent
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
            Assert.Equal(1, callCount);

            // Remove handler.
            item.Click -= handler;
            item.PerformClick();
            Assert.Equal(1, callCount);
        }

        [Fact]
        public void ToolStripItem_PerformClick_EnabledChangesInMiddle_Success()
        {
            var item = new FlippingEnabledToolStripItem();

            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(item, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };

            // Call with handler.
            item.Click += handler;
            item.PerformClick();
            Assert.Equal(1, callCount);

            // Remove handler.
            item.Click -= handler;
            item.PerformClick();
            Assert.Equal(1, callCount);
        }

        [Fact]
        public void ToolStripItem_ProcessCmdKey_Invoke_ReturnsFalse()
        {
            var item = new SubToolStripItem();
            var message = new Message();
            Assert.False(item.ProcessCmdKey(ref message, Keys.Enter));
        }

        [Theory]
        [InlineData(true, 1)]
        [InlineData(false, 0)]
        public void ToolStripItem_ProcessDialogKey_EnterKey_PerformsClick(bool enabled, int expectedCallCount)
        {
            var item = new SubToolStripItem
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

        [Theory]
        [InlineData(true, 1)]
        [InlineData(false, 0)]
        public void ToolStripItem_ProcessDialogKey_SpaceKey_PerformsClick(bool enabled, int expectedCallCount)
        {
            var item = new ToolStripButton
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

        [Theory]
        [InlineData(true, 1)]
        [InlineData(false, 0)]
        public void ToolStripItem_ProcessDialogKey_EnterKeyWithParent_PerformsClick(bool enabled, int expectedCallCount)
        {
            var parent = new ToolStrip();
            var item = new SubToolStripItem
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

        [Theory]
        [InlineData(true, 1)]
        [InlineData(false, 0)]
        public void ToolStripItem_ProcessDialogKey_EnterKeyWithDropDownParent_PerformsClick(bool enabled, int expectedCallCount)
        {
            var parent = new ToolStripDropDown();
            var item = new SubToolStripItem
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

        [Theory]
        [InlineData(Keys.Space)]
        [InlineData(Keys.A)]
        [InlineData(Keys.None)]
        [InlineData((Keys)(Keys.None - 1))]
        public void ToolStripItem_ProcessDialogKey_UnknownKey_ReturnsFalse(Keys keyData)
        {
            var item = new SubToolStripItem();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(item, sender);
                Assert.Same(EventArgs.Empty, e);
                Assert.True(item.Pressed);
                callCount++;
            };

            item.Click += handler;
            Assert.False(item.ProcessDialogKey(keyData));
            Assert.Equal(0, callCount);
            Assert.False(item.Pressed);
        }

        [Theory]
        [InlineData(Keys.A)]
        [InlineData(Keys.None)]
        [InlineData((Keys)(Keys.None - 1))]
        public void ToolStripItem_ProcessDialogKey_UnknownKeySpace_ReturnsFalse(Keys keyData)
        {
            var item = new ToolStripButton();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(item, sender);
                Assert.Same(EventArgs.Empty, e);
                Assert.True(item.Pressed);
                callCount++;
            };

            item.Click += handler;
            Assert.False(item.ProcessDialogKey(keyData));
            Assert.Equal(0, callCount);
            Assert.False(item.Pressed);
        }

        [Theory]
        [InlineData(true, 1)]
        [InlineData(false, 0)]
        public void ToolStripItem_ProcessMnemonic_EnterKey_PerformsClick(bool enabled, int expectedCallCount)
        {
            var item = new SubToolStripItem
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

        [Fact]
        public void ToolStripItem_ResetBackColor_Invoke_Success()
        {
            var item = new SubToolStripItem
            {
                BackColor = Color.Red
            };
            item.ResetBackColor();
            Assert.Equal(Control.DefaultBackColor, item.BackColor);
        }

        [Fact]
        public void ToolStripItem_ResetDisplay_Invoke_Success()
        {
            var item = new SubToolStripItem
            {
                DisplayStyle = ToolStripItemDisplayStyle.Text
            };
            item.ResetDisplayStyle();
            Assert.Equal(ToolStripItemDisplayStyle.ImageAndText, item.DisplayStyle);
        }

        [Fact]
        public void ToolStripItem_ResetFont_Invoke_Success()
        {
            var item = new SubToolStripItem
            {
                Font = SystemFonts.MenuFont
            };
            item.ResetFont();
            Assert.Equal(ToolStripManager.DefaultFont, item.Font);
        }

        [Fact]
        public void ToolStripItem_ResetForeColor_Invoke_Success()
        {
            var item = new SubToolStripItem
            {
                ForeColor = Color.Red
            };
            item.ResetForeColor();
            Assert.Equal(Control.DefaultForeColor, item.ForeColor);
        }

        [Fact]
        public void ToolStripItem_ResetImage_Invoke_Success()
        {
            var item = new SubToolStripItem
            {
                Image = new Bitmap(10, 10)
            };
            item.ResetImage();
            Assert.Null(item.Image);
        }

        [Fact]
        public void ToolStripItem_ResetMargin_Invoke_Success()
        {
            var item = new SubToolStripItem
            {
                Margin = new Padding(1, 2, 3, 4)
            };
            item.ResetMargin();
            Assert.Equal(new Padding(0, 1, 0, 2), item.Margin);
        }

        [Fact]
        public void ToolStripItem_ResetPadding_Invoke_Success()
        {
            var item = new SubToolStripItem
            {
                Padding = new Padding(1, 2, 3, 4)
            };
            item.ResetPadding();
            Assert.Equal(Padding.Empty, item.Padding);
        }

        [Fact]
        public void ToolStripItem_ResetRightToLeft_Invoke_Success()
        {
            var item = new SubToolStripItem
            {
                RightToLeft = RightToLeft.Yes
            };
            item.ResetRightToLeft();
            Assert.Equal(RightToLeft.Inherit, item.RightToLeft);
        }

        [Fact]
        public void ToolStripItem_ResetTextDirection_Invoke_Success()
        {
            var item = new SubToolStripItem
            {
                TextDirection = ToolStripTextDirection.Vertical90
            };
            item.ResetTextDirection();
            Assert.Equal(ToolStripTextDirection.Horizontal, item.TextDirection);
        }

        public static IEnumerable<object[]> Select_TestData()
        {
            yield return new object[] { new SubToolStripItem(), true };
            yield return new object[] { new CannotSelectToolStripItem(), false };

            var toolStrip = new ToolStrip();
            var toolStripItem = new SubToolStripItem();
            toolStrip.Items.Add(toolStripItem);
            yield return new object[] { toolStripItem, true };

            var toolStripDropDown = new ToolStripDropDown();
            var toolStripDropDownItem = new SubToolStripItem();
            toolStripDropDown.Items.Add(toolStripDropDownItem);
            yield return new object[] { toolStripDropDownItem, true };

            var toolStripParent = new ToolStrip();
            var toolStripParentItem = new SubToolStripItem
            {
                Parent = toolStripParent
            };
            yield return new object[] { toolStripParentItem, true };

            var toolStripDropDownParentItem = new SubToolStripItem
            {
                Parent = toolStripDropDown
            };
            yield return new object[] { toolStripDropDownParentItem, true };

            var toolStripDropDownWithOwnerItemOwnerItem = new SubToolStripItem();
            var toolStripDropDownWithOwnerItem = new ToolStripDropDown
            {
                OwnerItem = toolStripDropDownWithOwnerItemOwnerItem
            };
            var toolStripDropDownWithOwnerItemItem = new SubToolStripItem();
            toolStripDropDownWithOwnerItem.Items.Add(toolStripDropDownWithOwnerItemItem);
            yield return new object[] { toolStripDropDownWithOwnerItemItem, true };

        }

        [Theory]
        [MemberData(nameof(Select_TestData))]
        public void ToolStripItem_Select_Invoke_Success(ToolStripItem item, bool expected)
        {
            item.Select();
            Assert.Equal(expected, item.Selected);

            // Select again.
            item.Select();
            Assert.Equal(expected, item.Selected);
        }

        [Fact]
        public void ToolStripItem_Select_CantSelect_Success()
        {
            var item = new CannotSelectToolStripItem();
            item.Select();
            Assert.False(item.Selected);
        }

        [Fact]
        public void ToolStripItem_Select_InvokeWithParent_Success()
        {
            var parent = new ToolStrip();
            var item = new SubToolStripItem();
            item.Select();
            Assert.True(item.Selected);

            // Select again.
            item.Select();
            Assert.True(item.Selected);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetRectangleTheoryData))]
        public void ToolStripItem_SetBounds_Invoke_Success(Rectangle bounds)
        {
            var item = new SubToolStripItem();
            item.SetBounds(bounds);
            Assert.Equal(bounds, item.Bounds);
            Assert.Equal(bounds.Width, item.Width);
            Assert.Equal(bounds.Height, item.Height);
            Assert.Equal(bounds.Size, item.Bounds.Size);

            // Set same.
            item.SetBounds(bounds);
            Assert.Equal(bounds, item.Bounds);
            Assert.Equal(bounds.Width, item.Width);
            Assert.Equal(bounds.Height, item.Height);
            Assert.Equal(bounds.Size, item.Bounds.Size);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetRectangleTheoryData))]
        public void ToolStripItem_SetBounds_InvokeWithParent_Success(Rectangle bounds)
        {
            var parent = new ToolStrip();
            var item = new SubToolStripItem
            {
                Parent = parent
            };

            item.SetBounds(bounds);
            Assert.Equal(bounds, item.Bounds);
            Assert.Equal(bounds.Width, item.Width);
            Assert.Equal(bounds.Height, item.Height);
            Assert.Equal(bounds.Size, item.Bounds.Size);

            // Set same.
            item.SetBounds(bounds);
            Assert.Equal(bounds, item.Bounds);
            Assert.Equal(bounds.Width, item.Width);
            Assert.Equal(bounds.Height, item.Height);
            Assert.Equal(bounds.Size, item.Bounds.Size);
        }

        [Fact]
        public void ToolStripItem_SetBounds_SetWithHandler_CallsAvailableChanged()
        {
            var item = new SubToolStripItem();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(item, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            item.LocationChanged += handler;

            // Set different.
            item.SetBounds(new Rectangle(1, 2, 3, 4));
            Assert.Equal(new Rectangle(1, 2, 3, 4), item.Bounds);
            Assert.Equal(1, callCount);

            // Set same.
            item.SetBounds(new Rectangle(1, 2, 3, 4));
            Assert.Equal(new Rectangle(1, 2, 3, 4), item.Bounds);
            Assert.Equal(1, callCount);

            // Set different x.
            item.SetBounds(new Rectangle(2, 2, 3, 4));
            Assert.Equal(new Rectangle(2, 2, 3, 4), item.Bounds);
            Assert.Equal(2, callCount);

            // Set different y.
            item.SetBounds(new Rectangle(2, 3, 3, 4));
            Assert.Equal(new Rectangle(2, 3, 3, 4), item.Bounds);
            Assert.Equal(3, callCount);

            // Set different width.
            item.SetBounds(new Rectangle(2, 3, 4, 4));
            Assert.Equal(new Rectangle(2, 3, 4, 4), item.Bounds);
            Assert.Equal(3, callCount);

            // Set different height.
            item.SetBounds(new Rectangle(2, 3, 4, 5));
            Assert.Equal(new Rectangle(2, 3, 4, 5), item.Bounds);
            Assert.Equal(3, callCount);

            // Remove handler.
            item.LocationChanged -= handler;
            item.SetBounds(new Rectangle(1, 2, 3, 4));
            Assert.Equal(new Rectangle(1, 2, 3, 4), item.Bounds);
            Assert.Equal(3, callCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ToolStripItem_SetVisibleCore_Set_GetReturnsExpected(bool visible)
        {
            var item = new SubToolStripItem();
            item.SetVisibleCore(visible);
            Assert.Equal(visible, item.Available);

            // Set same.
            item.SetVisibleCore(visible);
            Assert.Equal(visible, item.Available);
        }

        [Fact]
        public void ToolStripItem_SetVisibleCore_SetWithHandler_CallsAvailableChanged()
        {
            var item = new SubToolStripItem();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(item, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            item.AvailableChanged += handler;

            // Set different.
            item.SetVisibleCore(false);
            Assert.False(item.Available);
            Assert.Equal(1, callCount);

            // Set same.
            item.SetVisibleCore(false);
            Assert.False(item.Available);
            Assert.Equal(1, callCount);

            // Set different.
            item.SetVisibleCore(true);
            Assert.True(item.Available);
            Assert.Equal(2, callCount);

            // Remove handler.
            item.AvailableChanged -= handler;
            item.SetVisibleCore(false);
            Assert.False(item.Available);
            Assert.Equal(2, callCount);
        }

        [Fact]
        public void ToolStripItem_SetVisibleCore_SetWithHandler_CallsVisibleChanged()
        {
            var item = new SubToolStripItem();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(item, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            item.VisibleChanged += handler;

            // Set different.
            item.SetVisibleCore(false);
            Assert.False(item.Available);
            Assert.Equal(1, callCount);

            // Set same.
            item.SetVisibleCore(false);
            Assert.False(item.Available);
            Assert.Equal(1, callCount);

            // Set different.
            item.SetVisibleCore(true);
            Assert.True(item.Available);
            Assert.Equal(2, callCount);

            // Remove handler.
            item.VisibleChanged -= handler;
            item.SetVisibleCore(false);
            Assert.False(item.Available);
            Assert.Equal(2, callCount);
        }

        public static IEnumerable<object[]> ToString_TestData()
        {
            yield return new object[] { new SubToolStripItem(), "System.Windows.Forms.Tests.ToolStripItemTests+SubToolStripItem" };
            yield return new object[] { new SubToolStripItem { Text = "text" }, "text" };
        }

        [Theory]
        [MemberData(nameof(ToString_TestData))]
        public void ToolStripItem_ToString_Invoke_ReturnsExpected(ToolStripItem item, string expected)
        {
            Assert.Equal(expected, item.ToString());
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

        private class CustomToolStripItem : ToolStripItem
        {
            protected override bool DefaultAutoToolTip => true;

            protected override ToolStripItemDisplayStyle DefaultDisplayStyle => ToolStripItemDisplayStyle.Text;

            protected internal override Padding DefaultMargin => new Padding(1, 2, 3, 4);

            protected override Padding DefaultPadding => new Padding(2, 3, 4, 5);

            protected override Size DefaultSize => new Size(10, 11);
        }

        private class CannotSelectToolStripItem : ToolStripItem
        {
            public override bool CanSelect => false;
        }

        private class NullCreateAccessibilityInstanceToolStripItem : ToolStripItem
        {
            public NullCreateAccessibilityInstanceToolStripItem() : base()
            {
            }

            protected override AccessibleObject CreateAccessibilityInstance() => null;
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

            public new bool DefaultAutoToolTip => base.DefaultAutoToolTip;

            public new ToolStripItemDisplayStyle DefaultDisplayStyle => base.DefaultDisplayStyle;

            public new Padding DefaultPadding => base.DefaultPadding;

            public new Size DefaultSize => base.DefaultSize;

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

            public new void OnLocationChanged(EventArgs e) => base.OnLocationChanged(e);

            public new void OnMouseDown(MouseEventArgs e) => base.OnMouseDown(e);

            public new void OnMouseEnter(EventArgs e) => base.OnMouseEnter(e);

            public new void OnMouseHover(EventArgs e) => base.OnMouseHover(e);

            public new void OnMouseLeave(EventArgs e) => base.OnMouseLeave(e);

            public new void OnMouseMove(MouseEventArgs e) => base.OnMouseMove(e);

            public new void OnMouseUp(MouseEventArgs e) => base.OnMouseUp(e);

            public new void OnOwnerChanged(EventArgs e) => base.OnOwnerChanged(e);

            public new void OnPaint(PaintEventArgs e) => base.OnPaint(e);

            public new void OnParentBackColorChanged(EventArgs e) => base.OnParentBackColorChanged(e);

            public new void OnParentChanged(ToolStrip oldParent, ToolStrip newParent) => base.OnParentChanged(oldParent, newParent);

            public new void OnParentEnabledChanged(EventArgs e) => base.OnParentEnabledChanged(e);

            public new void OnParentForeColorChanged(EventArgs e) => base.OnParentForeColorChanged(e);

            public new void OnQueryContinueDrag(QueryContinueDragEventArgs e) => base.OnQueryContinueDrag(e);

            public new void OnRightToLeftChanged(EventArgs e) => base.OnRightToLeftChanged(e);

            public new void OnTextChanged(EventArgs e) => base.OnTextChanged(e);

            public new void OnVisibleChanged(EventArgs e) => base.OnVisibleChanged(e);

            public new void SetVisibleCore(bool visible) => base.SetVisibleCore(visible);
        }
    }
}
