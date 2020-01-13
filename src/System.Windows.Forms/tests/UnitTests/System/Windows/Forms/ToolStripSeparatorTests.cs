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
    using Size = System.Drawing.Size;

    public class ToolStripSeparatorTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void ToolStripSeparator_Ctor_Default()
        {
            using var item = new SubToolStripSeparator();
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
            Assert.Equal(new Rectangle(0, 0, 6, 6), item.Bounds);
            Assert.False(item.CanSelect);
            Assert.True(item.CanRaiseEvents);
            Assert.Null(item.Container);
            Assert.Equal(new Rectangle(2, 2, 2, 2), item.ContentRectangle);
            Assert.False(item.DefaultAutoToolTip);
            Assert.Equal(ToolStripItemDisplayStyle.ImageAndText, item.DefaultDisplayStyle);
            Assert.Equal(Padding.Empty, item.DefaultMargin);
            Assert.Equal(Padding.Empty, item.DefaultPadding);
            Assert.Equal(new Size(6, 6), item.DefaultSize);
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
            Assert.Equal(SystemColors.ControlDark, item.ForeColor);
            Assert.Equal(6, item.Height);
            Assert.Null(item.Image);
            Assert.Equal(ContentAlignment.MiddleCenter, item.ImageAlign);
            Assert.Equal(-1, item.ImageIndex);
            Assert.Empty(item.ImageKey);
            Assert.Equal(ToolStripItemImageScaling.SizeToFit, item.ImageScaling);
            Assert.Equal(Color.Empty, item.ImageTransparentColor);
            Assert.False(item.IsDisposed);
            Assert.False(item.IsOnDropDown);
            Assert.False(item.IsOnOverflow);
            Assert.Equal(Padding.Empty, item.Margin);
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
            Assert.Equal(new Size(6, 6), item.Size);
            Assert.Null(item.Tag);
            Assert.Empty(item.Text);
            Assert.Equal(ContentAlignment.MiddleCenter, item.TextAlign);
            Assert.Equal(ToolStripTextDirection.Horizontal, item.TextDirection);
            Assert.Equal(TextImageRelation.ImageBeforeText, item.TextImageRelation);
            Assert.Null(item.ToolTipText);
            Assert.False(item.Visible);
            Assert.Equal(6, item.Width);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ToolStripSeparator_AutoToolTip_Set_GetReturnsExpected(bool value)
        {
            using var item = new ToolStripSeparator
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

        public static IEnumerable<object[]> BackgroundImage_Set_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new Bitmap(10, 10) };
            yield return new object[] { Image.FromFile(Path.Combine("bitmaps", "nature24bits.gif")) };
            yield return new object[] { Image.FromFile(Path.Combine("bitmaps", "10x16_one_entry_32bit.ico")) };
        }

        [WinFormsTheory]
        [MemberData(nameof(BackgroundImage_Set_TestData))]
        public void ToolStripSeparator_BackgroundImage_Set_GetReturnsExpected(Image value)
        {
            using var item = new ToolStripSeparator
            {
                BackgroundImage = value
            };
            Assert.Equal(value, item.BackgroundImage);

            // Set same.
            item.BackgroundImage = value;
            Assert.Equal(value, item.BackgroundImage);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(ImageLayout))]
        public void ToolStripSeparator_BackgroundImageLayout_Set_GetReturnsExpected(ImageLayout value)
        {
            using var item = new ToolStripSeparator
            {
                BackgroundImageLayout = value
            };
            Assert.Equal(value, item.BackgroundImageLayout);

            // Set same.
            item.BackgroundImageLayout = value;
            Assert.Equal(value, item.BackgroundImageLayout);
        }

        [WinFormsFact]
        public void ToolStripSeparator_CanSelect_InvokeDesignMode_ReturnsTrue()
        {
            var mockSite = new Mock<ISite>(MockBehavior.Strict);
            mockSite
                .Setup(s => s.Name)
                .Returns("Name");
            mockSite
                .Setup(s => s.DesignMode)
                .Returns(true);
            mockSite
                .Setup(s => s.Container)
                .Returns((IContainer)null);
            using var item = new ToolStripSeparator
            {
                Site = mockSite.Object
            };
            Assert.True(item.CanSelect);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(ToolStripItemDisplayStyle))]
        public void ToolStripSeparator_DisplayStyle_Set_GetReturnsExpected(ToolStripItemDisplayStyle value)
        {
            using var item = new ToolStripSeparator
            {
                DisplayStyle = value
            };
            Assert.Equal(value, item.DisplayStyle);

            // Set same.
            item.DisplayStyle = value;
            Assert.Equal(value, item.DisplayStyle);
        }

        [WinFormsFact]
        public void ToolStripSeparator_DisplayStyle_SetWithHandler_CallsDisplayStyleChanged()
        {
            using var item = new ToolStripSeparator();
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
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(ToolStripItemDisplayStyle))]
        public void ToolStripSeparator_DisplayStyle_SetInvalid_ThrowsInvalidEnumArgumentException(ToolStripItemDisplayStyle value)
        {
            using var item = new ToolStripSeparator();
            Assert.Throws<InvalidEnumArgumentException>("value", () => item.DisplayStyle = value);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ToolStripSeparator_DoubleClickEnabled_Set_GetReturnsExpected(bool value)
        {
            using var item = new ToolStripSeparator
            {
                DoubleClickEnabled = value
            };
            Assert.Equal(value, item.DoubleClickEnabled);

            // Set same.
            item.DoubleClickEnabled = value;
            Assert.Equal(value, item.DoubleClickEnabled);
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
        public void ToolStripSeparator_Enabled_Set_GetReturnsExpected(bool visible, Image image, bool value)
        {
            using var item = new ToolStripSeparator
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

        [WinFormsFact]
        public void ToolStripSeparator_Enabled_SetWithHandler_CallsEnabledChanged()
        {
            using var item = new ToolStripSeparator();
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
        public void ToolStripSeparator_Font_Set_GetReturnsExpected(ToolStripItemDisplayStyle displayStyle, Font value)
        {
            using var item = new ToolStripSeparator
            {
                DisplayStyle = displayStyle
            };

            item.Font = value;
            Assert.Equal(value ?? new ToolStripSeparator().Font, item.Font);

            // Set same.
            item.Font = value;
            Assert.Equal(value ?? new ToolStripSeparator().Font, item.Font);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetForeColorTheoryData))]
        public void ToolStripSeparator_ForeColor_Set_GetReturnsExpected(Color value, Color expected)
        {
            using var item = new ToolStripSeparator
            {
                ForeColor = value
            };
            Assert.Equal(expected, item.ForeColor);

            // Set same.
            item.ForeColor = value;
            Assert.Equal(expected, item.ForeColor);
        }

        [WinFormsFact]
        public void ToolStripSeparator_ForeColor_SetWithHandler_CallsForeColorChanged()
        {
            using var item = new ToolStripSeparator();
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
        public void ToolStripSeparator_ForeColor_ResetValue_Success()
        {
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ToolStripSeparator))[nameof(ToolStripSeparator.ForeColor)];
            using var item = new ToolStripSeparator();
            Assert.False(property.CanResetValue(item));

            item.ForeColor = Color.Red;
            Assert.Equal(Color.Red, item.ForeColor);
            Assert.True(property.CanResetValue(item));

            property.ResetValue(item);
            Assert.Equal(Control.DefaultForeColor, item.ForeColor);
            Assert.True(property.CanResetValue(item));
        }

        [WinFormsFact]
        public void ToolStripSeparator_ForeColor_ShouldSerializeValue_Success()
        {
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ToolStripSeparator))[nameof(ToolStripSeparator.ForeColor)];
            using var item = new ToolStripSeparator();
            Assert.False(property.ShouldSerializeValue(item));

            item.ForeColor = Color.Red;
            Assert.Equal(Color.Red, item.ForeColor);
            Assert.True(property.ShouldSerializeValue(item));

            property.ResetValue(item);
            Assert.Equal(Control.DefaultForeColor, item.ForeColor);
            Assert.True(property.ShouldSerializeValue(item));
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
        public void ToolStripSeparator_Image_Set_GetReturnsExpected(Color imageTransparentColor, Image value)
        {
            using var item = new ToolStripSeparator
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

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(ContentAlignment))]
        public void ToolStripSeparator_ImageAlign_Set_GetReturnsExpected(ContentAlignment value)
        {
            using var item = new ToolStripSeparator
            {
                ImageAlign = value
            };
            Assert.Equal(value, item.ImageAlign);

            // Set same.
            item.ImageAlign = value;
            Assert.Equal(value, item.ImageAlign);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(ContentAlignment))]
        [InlineData((ContentAlignment)int.MaxValue)]
        [InlineData((ContentAlignment)int.MinValue)]
        public void ToolStripSeparator_ImageAlign_SetInvalid_ThrowsInvalidEnumArgumentException(ContentAlignment value)
        {
            using var item = new ToolStripSeparator();
            Assert.Throws<InvalidEnumArgumentException>("value", () => item.ImageAlign = value);
        }

        [WinFormsTheory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        public void ToolStripSeparator_ImageIndex_Set_GetReturnsExpected(int value)
        {
            using var item = new ToolStripSeparator
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
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void ToolStripSeparator_ImageKey_Set_GetReturnsExpected(string value, string expected)
        {
            using var item = new ToolStripSeparator
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
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(ToolStripItemImageScaling))]
        public void ToolStripSeparator_ImageScaling_Set_GetReturnsExpected(ToolStripItemImageScaling value)
        {
            using var item = new ToolStripSeparator
            {
                ImageScaling = value
            };
            Assert.Equal(value, item.ImageScaling);

            // Set same.
            item.ImageScaling = value;
            Assert.Equal(value, item.ImageScaling);
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
        public void ToolStripSeparator_ImageTransparentColor_Set_GetReturnsExpected(Image image, Color value)
        {
            using var item = new ToolStripSeparator
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
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ToolStripSeparator_RightToLeftAutoMirrorImage_Set_GetReturnsExpected(bool value)
        {
            using var item = new ToolStripSeparator
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
        [CommonMemberData(nameof(CommonTestHelper.GetStringWithNullTheoryData))]
        public void ToolStripSeparator_Text_Set_GetReturnsExpected(string value)
        {
            using var item = new ToolStripSeparator
            {
                Text = value
            };
            Assert.Equal(value, item.Text);

            // Set same.
            item.Text = value;
            Assert.Equal(value, item.Text);
        }

        [WinFormsFact]
        public void ToolStripSeparator_Text_SetWithHandler_CallsTextChanged()
        {
            using var item = new ToolStripSeparator();
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
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(ContentAlignment))]
        public void ToolStripSeparator_TextAlign_Set_GetReturnsExpected(ContentAlignment value)
        {
            using var item = new ToolStripSeparator
            {
                TextAlign = value
            };
            Assert.Equal(value, item.TextAlign);

            // Set same.
            item.TextAlign = value;
            Assert.Equal(value, item.TextAlign);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(ContentAlignment))]
        [InlineData((ContentAlignment)int.MaxValue)]
        [InlineData((ContentAlignment)int.MinValue)]
        public void ToolStripSeparator_TextAlign_SetInvalid_ThrowsInvalidEnumArgumentException(ContentAlignment value)
        {
            using var item = new ToolStripSeparator();
            Assert.Throws<InvalidEnumArgumentException>("value", () => item.TextAlign = value);
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
        public void ToolStripSeparator_TextDirection_Set_GetReturnsExpected(ToolStripTextDirection value, ToolStripTextDirection expected)
        {
            using var item = new ToolStripSeparator
            {
                TextDirection = value
            };
            Assert.Equal(expected, item.TextDirection);

            // Set same.
            item.TextDirection = value;
            Assert.Equal(expected, item.TextDirection);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(ToolStripTextDirection))]
        public void ToolStripSeparator_TextDirection_SetInvalid_ThrowsInvalidEnumArgumentException(ToolStripTextDirection value)
        {
            using var item = new ToolStripSeparator();
            Assert.Throws<InvalidEnumArgumentException>("value", () => item.TextDirection = value);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(TextImageRelation))]
        public void ToolStripSeparator_TextImageRelation_Set_GetReturnsExpected(TextImageRelation value)
        {
            using var item = new ToolStripSeparator
            {
                TextImageRelation = value
            };
            Assert.Equal(value, item.TextImageRelation);

            // Set same.
            item.TextImageRelation = value;
            Assert.Equal(value, item.TextImageRelation);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(TextImageRelation))]
        [InlineData((TextImageRelation)3)]
        [InlineData((TextImageRelation)5)]
        [InlineData((TextImageRelation)6)]
        [InlineData((TextImageRelation)7)]
        public void ToolStripSeparator_TextImageRelation_SetInvalid_ThrowsInvalidEnumArgumentException(TextImageRelation value)
        {
            using var item = new ToolStripSeparator();
            Assert.Throws<InvalidEnumArgumentException>("value", () => item.TextImageRelation = value);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringTheoryData))]
        public void ToolStripSeparator_ToolTipText_Set_GetReturnsExpected(string value)
        {
            using var item = new ToolStripSeparator
            {
                ToolTipText = value
            };
            Assert.Equal(value, item.ToolTipText);

            // Set same.
            item.ToolTipText = value;
            Assert.Equal(value, item.ToolTipText);
        }

        [WinFormsFact]
        public void ToolStripSeparator_CreateAccessibilityInstance_Invoke_ReturnsExpected()
        {
            using var item = new SubToolStripSeparator();
            ToolStripItem.ToolStripItemAccessibleObject accessibleObject = Assert.IsAssignableFrom<ToolStripItem.ToolStripItemAccessibleObject>(item.CreateAccessibilityInstance());
            Assert.Equal(AccessibleRole.Separator, accessibleObject.Role);
            Assert.NotSame(accessibleObject, item.CreateAccessibilityInstance());
            Assert.NotSame(accessibleObject, item.AccessibilityObject);
        }

        [WinFormsFact]
        public void ToolStripSeparator_CreateAccessibilityInstance_InvokeWithCustomRole_ReturnsExpected()
        {
            using var item = new SubToolStripSeparator
            {
                AccessibleRole = AccessibleRole.HelpBalloon
            };
            ToolStripItem.ToolStripItemAccessibleObject accessibleObject = Assert.IsAssignableFrom<ToolStripItem.ToolStripItemAccessibleObject>(item.CreateAccessibilityInstance());
            Assert.Equal(AccessibleRole.HelpBalloon, accessibleObject.Role);
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
        public void ToolStripSeparator_GetPreferredSize_Invoke_ReturnsExpected(Size proposedSize)
        {
            using var item = new ToolStripSeparator();
            Assert.Equal(new Size(6, 6), item.GetPreferredSize(proposedSize));

            // Call again.
            Assert.Equal(new Size(6, 6), item.GetPreferredSize(proposedSize));
        }

        public static IEnumerable<object[]> GetPreferredSize_WithOwner_TestData()
        {
            yield return new object[] { ToolStripLayoutStyle.Flow, Size.Empty, new Size(6, 23) };
            yield return new object[] { ToolStripLayoutStyle.Flow, new Size(-1, -2), new Size(6, 23) };
            yield return new object[] { ToolStripLayoutStyle.Flow, new Size(10, 20), new Size(6, 23) };
            yield return new object[] { ToolStripLayoutStyle.Flow, new Size(30, 40), new Size(6, 23) };
            yield return new object[] { ToolStripLayoutStyle.Flow, new Size(int.MaxValue, int.MaxValue), new Size(6, 23) };

            yield return new object[] { ToolStripLayoutStyle.HorizontalStackWithOverflow, Size.Empty, new Size(6, 23) };
            yield return new object[] { ToolStripLayoutStyle.HorizontalStackWithOverflow, new Size(-1, -2), new Size(6, 23) };
            yield return new object[] { ToolStripLayoutStyle.HorizontalStackWithOverflow, new Size(10, 20), new Size(6, 23) };
            yield return new object[] { ToolStripLayoutStyle.HorizontalStackWithOverflow, new Size(30, 40), new Size(6, 23) };
            yield return new object[] { ToolStripLayoutStyle.HorizontalStackWithOverflow, new Size(int.MaxValue, int.MaxValue), new Size(6, 23) };

            yield return new object[] { ToolStripLayoutStyle.StackWithOverflow, Size.Empty, new Size(6, 23) };
            yield return new object[] { ToolStripLayoutStyle.StackWithOverflow, new Size(-1, -2), new Size(6, 23) };
            yield return new object[] { ToolStripLayoutStyle.StackWithOverflow, new Size(10, 20), new Size(6, 23) };
            yield return new object[] { ToolStripLayoutStyle.StackWithOverflow, new Size(30, 40), new Size(6, 23) };
            yield return new object[] { ToolStripLayoutStyle.StackWithOverflow, new Size(int.MaxValue, int.MaxValue), new Size(6, 23) };

            yield return new object[] { ToolStripLayoutStyle.Table, Size.Empty, new Size(6, 23) };
            yield return new object[] { ToolStripLayoutStyle.Table, new Size(-1, -2), new Size(6, 23) };
            yield return new object[] { ToolStripLayoutStyle.Table, new Size(10, 20), new Size(6, 23) };
            yield return new object[] { ToolStripLayoutStyle.Table, new Size(30, 40), new Size(6, 23) };
            yield return new object[] { ToolStripLayoutStyle.Table, new Size(int.MaxValue, int.MaxValue), new Size(6, 23) };

            yield return new object[] { ToolStripLayoutStyle.VerticalStackWithOverflow, Size.Empty, new Size(23, 6) };
            yield return new object[] { ToolStripLayoutStyle.VerticalStackWithOverflow, new Size(-1, -2), new Size(23, 6) };
            yield return new object[] { ToolStripLayoutStyle.VerticalStackWithOverflow, new Size(10, 20), new Size(23, 6) };
            yield return new object[] { ToolStripLayoutStyle.VerticalStackWithOverflow, new Size(30, 40), new Size(23, 6) };
            yield return new object[] { ToolStripLayoutStyle.VerticalStackWithOverflow, new Size(int.MaxValue, int.MaxValue), new Size(23, 6) };
        }

        [WinFormsTheory]
        [MemberData(nameof(GetPreferredSize_WithOwner_TestData))]
        public void ToolStripSeparator_GetPreferredSize_InvokeWithOwner_ReturnsExpected(ToolStripLayoutStyle ownerLayoutStyle, Size proposedSize, Size expected)
        {
            using var owner = new ToolStrip
            {
                LayoutStyle = ownerLayoutStyle
            };
            using var item = new ToolStripSeparator
            {
                Owner = owner
            };
            Assert.Equal(expected, item.GetPreferredSize(proposedSize));

            // Call again.
            Assert.Equal(expected, item.GetPreferredSize(proposedSize));
        }

        public static IEnumerable<object[]> GetPreferredSize_WithToolStripDropDownMenuOwner_TestData()
        {
            foreach (ToolStripLayoutStyle layoutStyle in Enum.GetValues(typeof(ToolStripLayoutStyle)))
            {
                yield return new object[] { layoutStyle, Size.Empty };
                yield return new object[] { layoutStyle, new Size(-1, -2) };
                yield return new object[] { layoutStyle, new Size(10, 20) };
                yield return new object[] { layoutStyle, new Size(30, 40) };
                yield return new object[] { layoutStyle, new Size(int.MaxValue, int.MaxValue) };
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(GetPreferredSize_WithToolStripDropDownMenuOwner_TestData))]
        public void ToolStripSeparator_GetPreferredSize_InvokeWithToolStripDropDownMenuOwner_ReturnsExpected(ToolStripLayoutStyle ownerLayoutStyle, Size proposedSize)
        {
            using var owner = new ToolStripDropDownMenu
            {
                LayoutStyle = ownerLayoutStyle
            };
            using var item = new ToolStripSeparator
            {
                Owner = owner
            };
            Assert.Equal(new Size(52, 6), item.GetPreferredSize(proposedSize));

            // Call again.
            Assert.Equal(new Size(52, 6), item.GetPreferredSize(proposedSize));
        }

        [WinFormsTheory]
        [MemberData(nameof(GetPreferredSize_WithOwner_TestData))]
        public void ToolStripSeparator_GetPreferredSize_InvokeWithParent_ReturnsExpected(ToolStripLayoutStyle parentLayoutStyle, Size proposedSize, Size expected)
        {
            using var parent = new ToolStrip
            {
                LayoutStyle = parentLayoutStyle
            };
            using var item = new SubToolStripSeparator
            {
                Parent = parent
            };
            Assert.Equal(expected, item.GetPreferredSize(proposedSize));

            // Call again.
            Assert.Equal(expected, item.GetPreferredSize(proposedSize));
        }

        public static IEnumerable<object[]> GetPreferredSize_WithToolStripDropDownMenuParent_TestData()
        {
            yield return new object[] { ToolStripLayoutStyle.Flow, Size.Empty, new Size(91, 6) };
            yield return new object[] { ToolStripLayoutStyle.Flow, new Size(-1, -2), new Size(91, 6) };
            yield return new object[] { ToolStripLayoutStyle.Flow, new Size(10, 20), new Size(91, 6) };
            yield return new object[] { ToolStripLayoutStyle.Flow, new Size(30, 40), new Size(91, 6) };
            yield return new object[] { ToolStripLayoutStyle.Flow, new Size(int.MaxValue, int.MaxValue), new Size(91, 6) };

            yield return new object[] { ToolStripLayoutStyle.HorizontalStackWithOverflow, Size.Empty, new Size(91, 6) };
            yield return new object[] { ToolStripLayoutStyle.HorizontalStackWithOverflow, new Size(-1, -2), new Size(91, 6) };
            yield return new object[] { ToolStripLayoutStyle.HorizontalStackWithOverflow, new Size(10, 20), new Size(91, 6) };
            yield return new object[] { ToolStripLayoutStyle.HorizontalStackWithOverflow, new Size(30, 40), new Size(91, 6) };
            yield return new object[] { ToolStripLayoutStyle.HorizontalStackWithOverflow, new Size(int.MaxValue, int.MaxValue), new Size(91, 6) };

            yield return new object[] { ToolStripLayoutStyle.StackWithOverflow, Size.Empty, new Size(52, 6) };
            yield return new object[] { ToolStripLayoutStyle.StackWithOverflow, new Size(-1, -2), new Size(52, 6) };
            yield return new object[] { ToolStripLayoutStyle.StackWithOverflow, new Size(10, 20), new Size(52, 6) };
            yield return new object[] { ToolStripLayoutStyle.StackWithOverflow, new Size(30, 40), new Size(52, 6) };
            yield return new object[] { ToolStripLayoutStyle.StackWithOverflow, new Size(int.MaxValue, int.MaxValue), new Size(52, 6) };

            yield return new object[] { ToolStripLayoutStyle.Table, Size.Empty, new Size(91, 6) };
            yield return new object[] { ToolStripLayoutStyle.Table, new Size(-1, -2), new Size(91, 6) };
            yield return new object[] { ToolStripLayoutStyle.Table, new Size(10, 20), new Size(91, 6) };
            yield return new object[] { ToolStripLayoutStyle.Table, new Size(30, 40), new Size(91, 6) };
            yield return new object[] { ToolStripLayoutStyle.Table, new Size(int.MaxValue, int.MaxValue), new Size(91, 6) };

            yield return new object[] { ToolStripLayoutStyle.VerticalStackWithOverflow, Size.Empty, new Size(52, 6) };
            yield return new object[] { ToolStripLayoutStyle.VerticalStackWithOverflow, new Size(-1, -2), new Size(52, 6) };
            yield return new object[] { ToolStripLayoutStyle.VerticalStackWithOverflow, new Size(10, 20), new Size(52, 6) };
            yield return new object[] { ToolStripLayoutStyle.VerticalStackWithOverflow, new Size(30, 40), new Size(52, 6) };
            yield return new object[] { ToolStripLayoutStyle.VerticalStackWithOverflow, new Size(int.MaxValue, int.MaxValue), new Size(52, 6) };
        }

        [WinFormsTheory]
        [MemberData(nameof(GetPreferredSize_WithToolStripDropDownMenuParent_TestData))]
        public void ToolStripSeparator_GetPreferredSize_InvokeWithToolStripDropDownMenuParent_ReturnsExpected(ToolStripLayoutStyle ownerLayoutStyle, Size proposedSize, Size expected)
        {
            using var parent = new ToolStripDropDownMenu
            {
                LayoutStyle = ownerLayoutStyle
            };
            using var item = new SubToolStripSeparator
            {
                Parent = parent
            };
            Assert.Equal(expected, item.GetPreferredSize(proposedSize));

            // Call again.
            Assert.Equal(expected, item.GetPreferredSize(proposedSize));
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
        public void ToolStripSeparator_OnFontChanged_Invoke_Success(ToolStripItemDisplayStyle displayStyle, EventArgs eventArgs)
        {
            using var item = new SubToolStripSeparator
            {
                DisplayStyle = displayStyle
            };

            item.OnFontChanged(eventArgs);

            // Call again.
            item.OnFontChanged(eventArgs);
        }

        [WinFormsTheory]
        [MemberData(nameof(OnFontChanged_TestData))]
        public void ToolStripSeparator_OnFontChanged_InvokeWithOwner_Success(ToolStripItemDisplayStyle displayStyle, EventArgs eventArgs)
        {
            using var owner = new ToolStrip();
            using var item = new SubToolStripSeparator
            {
                Owner = owner,
                DisplayStyle = displayStyle
            };
            int ownerLayoutCallCount = 0;
            void ownerHandler(object sender, LayoutEventArgs e) => ownerLayoutCallCount++;
            owner.Layout += ownerHandler;

            try
            {
                item.OnFontChanged(eventArgs);
                Assert.Equal(0, ownerLayoutCallCount);
                Assert.False(owner.IsHandleCreated);

                // Call again.
                item.OnFontChanged(eventArgs);
                Assert.Equal(0, ownerLayoutCallCount);
                Assert.False(owner.IsHandleCreated);
            }
            finally
            {
                owner.Layout -= ownerHandler;
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(OnFontChanged_TestData))]
        public void ToolStripSeparator_OnFontChanged_InvokeWithOwnerWithHandle_Success(ToolStripItemDisplayStyle displayStyle, EventArgs eventArgs)
        {
            using var owner = new ToolStrip();
            using var item = new SubToolStripSeparator
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
            void ownerHandler(object sender, LayoutEventArgs e) => ownerLayoutCallCount++;
            owner.Layout += ownerHandler;

            try
            {
                item.OnFontChanged(eventArgs);
                Assert.Equal(0, ownerLayoutCallCount);
                Assert.True(owner.IsHandleCreated);
                Assert.Equal(0, invalidatedCallCount);
                Assert.Equal(0, styleChangedCallCount);
                Assert.Equal(0, createdCallCount);

                // Call again.
                item.OnFontChanged(eventArgs);
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
        [MemberData(nameof(OnFontChanged_TestData))]
        public void ToolStripSeparator_OnFontChanged_InvokeWithParent_Success(ToolStripItemDisplayStyle displayStyle, EventArgs eventArgs)
        {
            using var parent = new ToolStrip();
            using var item = new SubToolStripSeparator
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
        public void ToolStripSeparator_OnFontChanged_InvokeWithParentWithHandle_Success(ToolStripItemDisplayStyle displayStyle, EventArgs eventArgs)
        {
            using var parent = new ToolStrip();
            using var item = new SubToolStripSeparator
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
        [CommonMemberData(nameof(CommonTestHelper.GetPaintEventArgsTheoryData))]
        public void ToolStripSeparator_OnPaint_Invoke_DoesNotCallPaint(PaintEventArgs eventArgs)
        {
            using var item = new SubToolStripSeparator();
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

        public static IEnumerable<object[]> OnPaint_WithOwner_TestData()
        {
            foreach (ToolStripLayoutStyle layoutStyle in Enum.GetValues(typeof(ToolStripLayoutStyle)))
            {
                yield return new object[] { layoutStyle, null };

                var image = new Bitmap(10, 10);
                Graphics graphics = Graphics.FromImage(image);
                yield return new object[] { layoutStyle, new PaintEventArgs(graphics, new Rectangle(1, 2, 3, 4)) };
            }
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetPaintEventArgsTheoryData))]
        public void ToolStripSeparator_OnPaint_InvokeWithOwner_DoesNotCallPaint(PaintEventArgs eventArgs)
        {
            var renderer = new SubToolStripRenderer();
            int renderSeparatorCallCount = 0;
            renderer.RenderSeparator += (sender, e) => renderSeparatorCallCount++;
            using var owner = new ToolStrip
            {
                Renderer = renderer
            };
            using var item = new SubToolStripSeparator
            {
                Owner = owner
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
            Assert.Equal(0, renderSeparatorCallCount);

            // Remove handler.
            item.Paint -= handler;
            item.OnPaint(eventArgs);
            Assert.Equal(0, callCount);
            Assert.Equal(0, renderSeparatorCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetPaintEventArgsTheoryData))]
        public void ToolStripSeparator_OnPaint_InvokeWithParent_DoesNotCallPaint(PaintEventArgs eventArgs)
        {
            var renderer = new SubToolStripRenderer();
            int renderSeparatorCallCount = 0;
            renderer.RenderSeparator += (sender, e) => renderSeparatorCallCount++;
            using var parent = new ToolStrip
            {
                Renderer = renderer
            };
            using var item = new SubToolStripSeparator
            {
                Parent = parent
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
            Assert.Equal(0, renderSeparatorCallCount);

            // Remove handler.
            item.Paint -= handler;
            item.OnPaint(eventArgs);
            Assert.Equal(0, callCount);
            Assert.Equal(0, renderSeparatorCallCount);
        }

        public static IEnumerable<object[]> OnPaint_WithOwnerAndParent_TestData()
        {
            foreach (ToolStripLayoutStyle ownerLayoutStyle in Enum.GetValues(typeof(ToolStripLayoutStyle)))
            {
                yield return new object[] { ownerLayoutStyle, ToolStripLayoutStyle.Flow, true };
                yield return new object[] { ownerLayoutStyle, ToolStripLayoutStyle.HorizontalStackWithOverflow, true };
                yield return new object[] { ownerLayoutStyle, ToolStripLayoutStyle.StackWithOverflow, true };
                yield return new object[] { ownerLayoutStyle, ToolStripLayoutStyle.Table, true };
                yield return new object[] { ownerLayoutStyle, ToolStripLayoutStyle.VerticalStackWithOverflow, false };
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(OnPaint_WithOwnerAndParent_TestData))]
        public void ToolStripSeparator_OnPaint_InvokeWithOwnerAndParent_DoesNotCallPaint(ToolStripLayoutStyle ownerLayoutStyle, ToolStripLayoutStyle parentLayoutStyle, bool expectedVertical)
        {
            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);
            var eventArgs = new PaintEventArgs(graphics, new Rectangle(1, 2, 3, 4));

            var renderer1 = new SubToolStripRenderer();
            var renderer2 = new SubToolStripRenderer();
            using var owner = new ToolStrip
            {
                LayoutStyle = ownerLayoutStyle,
                Renderer = renderer1
            };
            using var parent = new ToolStrip
            {
                LayoutStyle = parentLayoutStyle,
                Renderer = renderer2
            };
            using var item = new SubToolStripSeparator
            {
                Owner = owner,
                Parent = parent
            };
            int renderSeparatorCallCount1 = 0;
            renderer1.RenderSeparator += (sender, e) =>
            {
                Assert.Same(renderer1, sender);
                Assert.Same(graphics, e.Graphics);
                Assert.Same(item, e.Item);
                Assert.Same(parent, e.ToolStrip);
                Assert.Equal(expectedVertical, e.Vertical);
                renderSeparatorCallCount1++;
            };
            int renderSeparatorCallCount2 = 0;
            renderer2.RenderSeparator += (sender, e) => renderSeparatorCallCount2++;
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
            Assert.Equal(1, renderSeparatorCallCount1);
            Assert.Equal(0, renderSeparatorCallCount2);

            // Remove handler.
            item.Paint -= handler;
            item.OnPaint(eventArgs);
            Assert.Equal(0, callCount);
            Assert.Equal(2, renderSeparatorCallCount1);
            Assert.Equal(0, renderSeparatorCallCount2);
        }

        public static IEnumerable<object[]> OnPaint_WithOwnerAndToolStripDropDownMenuParent_TestData()
        {
            foreach (ToolStripLayoutStyle ownerLayoutStyle in Enum.GetValues(typeof(ToolStripLayoutStyle)))
            {
                foreach (ToolStripLayoutStyle paremtLayoutStyle in Enum.GetValues(typeof(ToolStripLayoutStyle)))
                {
                    yield return new object[] { ownerLayoutStyle, paremtLayoutStyle };
                }
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(OnPaint_WithOwnerAndToolStripDropDownMenuParent_TestData))]
        public void ToolStripSeparator_OnPaint_InvokeWithOwnerAndToolStripDropDownMenuParent_DoesNotCallPaint(ToolStripLayoutStyle ownerLayoutStyle, ToolStripLayoutStyle parentLayoutStyle)
        {
            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);
            var eventArgs = new PaintEventArgs(graphics, new Rectangle(1, 2, 3, 4));

            var renderer1 = new SubToolStripRenderer();
            var renderer2 = new SubToolStripRenderer();
            using var owner = new ToolStrip
            {
                LayoutStyle = ownerLayoutStyle,
                Renderer = renderer1
            };
            using var parent = new ToolStripDropDownMenu
            {
                LayoutStyle = parentLayoutStyle,
                Renderer = renderer2
            };
            using var item = new SubToolStripSeparator
            {
                Owner = owner,
                Parent = parent
            };
            int renderSeparatorCallCount1 = 0;
            renderer1.RenderSeparator += (sender, e) =>
            {
                Assert.Same(renderer1, sender);
                Assert.Same(graphics, e.Graphics);
                Assert.Same(item, e.Item);
                Assert.Same(parent, e.ToolStrip);
                Assert.False(e.Vertical);
                renderSeparatorCallCount1++;
            };
            int renderSeparatorCallCount2 = 0;
            renderer2.RenderSeparator += (sender, e) => renderSeparatorCallCount2++;
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
            Assert.Equal(1, renderSeparatorCallCount1);
            Assert.Equal(0, renderSeparatorCallCount2);

            // Remove handler.
            item.Paint -= handler;
            item.OnPaint(eventArgs);
            Assert.Equal(0, callCount);
            Assert.Equal(2, renderSeparatorCallCount1);
            Assert.Equal(0, renderSeparatorCallCount2);
        }

        [WinFormsFact]
        public void ToolStripSeparator_OnPaint_NullE_ThrowsNullReferenceException()
        {
            using var owner = new ToolStrip();
            using var parent = new ToolStrip();
            using var item = new SubToolStripSeparator
            {
                Owner = owner,
                Parent = parent
            };
            Assert.Throws<NullReferenceException>(() => item.OnPaint(null));
        }

        public static IEnumerable<object[]> SetBounds_TestData()
        {
            yield return new object[] { new Rectangle(1, 0, 23, 23), 1 };
            yield return new object[] { new Rectangle(0, 2, 23, 23), 1 };
            yield return new object[] { new Rectangle(1, 2, 23, 23), 1 };
            yield return new object[] { new Rectangle(0, 0, -1, -2), 0 };
            yield return new object[] { new Rectangle(0, 0, 0, 0), 0 };
            yield return new object[] { new Rectangle(0, 0, 1, 2), 0 };
            yield return new object[] { new Rectangle(0, 0, 5, 6), 0 };
            yield return new object[] { new Rectangle(0, 0, 6, 5), 0 };
            yield return new object[] { new Rectangle(0, 0, 6, 6), 0 };
            yield return new object[] { new Rectangle(1, 2, 3, 4), 1 };
        }

        [WinFormsTheory]
        [MemberData(nameof(SetBounds_TestData))]
        public void ToolStripSeparator_SetBounds_Invoke_GetReturnsExpected(Rectangle bounds, int expectedLocationChangedCallCount)
        {
            using var item = new SubToolStripSeparator();
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
        public void ToolStripSeparator_SetBounds_InvokeWithOwner_GetReturnsExpected(Rectangle bounds, int expectedLocationChangedCallCount)
        {
            using var owner = new ToolStrip();
            using var item = new SubToolStripSeparator
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

        public static IEnumerable<object[]> SetBounds_ToolStripDropDownMenuOwner_TestData()
        {
            yield return new object[] { new Rectangle(1, 0, 23, 23), 1 };
            yield return new object[] { new Rectangle(0, 2, 23, 23), 1 };
            yield return new object[] { new Rectangle(1, 2, 23, 23), 1 };
            yield return new object[] { new Rectangle(0, 0, -1, -2), 1 };
            yield return new object[] { new Rectangle(0, 0, 0, 0), 1 };
            yield return new object[] { new Rectangle(0, 0, 1, 2), 1 };
            yield return new object[] { new Rectangle(0, 0, 5, 6), 1 };
            yield return new object[] { new Rectangle(0, 0, 6, 5), 1 };
            yield return new object[] { new Rectangle(0, 0, 6, 6), 1 };
            yield return new object[] { new Rectangle(1, 2, 3, 4), 1 };
            yield return new object[] { new Rectangle(2, 0, 3, 4), 1 };
        }

        [WinFormsTheory]
        [MemberData(nameof(SetBounds_ToolStripDropDownMenuOwner_TestData))]
        public void ToolStripSeparator_SetBounds_InvokeWithToolStripDropDownMenuOwner_GetReturnsExpected(Rectangle bounds, int expectedLocationChangedCallCount)
        {
            using var owner = new ToolStripDropDownMenu();
            using var item = new SubToolStripSeparator
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
                Assert.Equal(new Rectangle(2, bounds.Y, 57, bounds.Height), item.Bounds);
                Assert.Equal(new Size(57, bounds.Height), item.Size);
                Assert.Equal(57, item.Width);
                Assert.Equal(bounds.Height, item.Height);
                Assert.Equal(expectedLocationChangedCallCount, locationChangedCallCount);
                Assert.Equal(0, ownerLayoutCallCount);
                Assert.False(owner.IsHandleCreated);

                // Set same.
                item.SetBounds(bounds);
                Assert.Equal(new Rectangle(2, bounds.Y, 57, bounds.Height), item.Bounds);
                Assert.Equal(new Size(57, bounds.Height), item.Size);
                Assert.Equal(57, item.Width);
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

        public static IEnumerable<object[]> SetBounds_WithParent_TestData()
        {
            yield return new object[] { new Rectangle(1, 0, 23, 23), 1, 1 };
            yield return new object[] { new Rectangle(0, 2, 23, 23), 1, 1 };
            yield return new object[] { new Rectangle(1, 2, 23, 23), 1, 1 };
            yield return new object[] { new Rectangle(0, 0, -1, -2), 0, 1 };
            yield return new object[] { new Rectangle(0, 0, 0, 0), 0, 1 };
            yield return new object[] { new Rectangle(0, 0, 1, 2), 0, 1 };
            yield return new object[] { new Rectangle(0, 0, 5, 6), 0, 1 };
            yield return new object[] { new Rectangle(0, 0, 6, 5), 0, 1 };
            yield return new object[] { new Rectangle(0, 0, 6, 6), 0, 0 };
            yield return new object[] { new Rectangle(1, 2, 3, 4), 1, 1 };
        }

        [WinFormsTheory]
        [MemberData(nameof(SetBounds_WithParent_TestData))]
        public void ToolStripSeparator_SetBounds_InvokeWithParent_GetReturnsExpected(Rectangle bounds, int expectedLocationChangedCallCount, int expectedParentLayoutCallCount)
        {
            using var parent = new ToolStrip();
            using var item = new SubToolStripSeparator
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
            };
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
        public void ToolStripSeparator_SetBounds_InvokeWithDropDownMenuParent_GetReturnsExpected(Rectangle bounds, int expectedLocationChangedCallCount, int expectedParentLayoutCallCount)
        {
            using var parent = new ToolStripDropDownMenu();
            using var item = new SubToolStripSeparator
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
            };
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

        private class SubToolStripRenderer : ToolStripRenderer
        {
        }

        private class SubToolStripSeparator : ToolStripSeparator
        {
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

            public new void OnFontChanged(EventArgs e) => base.OnFontChanged(e);

            public new void OnPaint(PaintEventArgs e) => base.OnPaint(e);

            public new void SetBounds(Rectangle bounds) => base.SetBounds(bounds);
        }
    }
}
