// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.IO;
using System.Reflection;
using Moq;
using WinForms.Common.Tests;
using Xunit;

namespace System.Windows.Forms.Tests
{
    using Size = System.Drawing.Size;

    public class ToolStripControlHostTests
    {
        public static IEnumerable<object[]> Ctor_Control_TestData()
        {
            yield return new object[] { new Control() };
            yield return new object[]
            {
                new Control
                {
                    AccessibleDefaultActionDescription = "AccessibleDefaultActionDescription",
                    AccessibleDescription = "AccessibleDescription",
                    AccessibleName = "AccessibleName",
                    AccessibleRole = AccessibleRole.HelpBalloon,
                    BackColor = Color.Red,
                    BackgroundImage = new Bitmap(10, 10),
                    BackgroundImageLayout = ImageLayout.Zoom,
                    Bounds = new Rectangle(1, 2, 3, 4),
                    CausesValidation = false,
                    ForeColor = Color.Blue,
                    Font = SystemFonts.MenuFont,
                    RightToLeft = RightToLeft.Yes,
                    Tag = new object(),
                    Text = "text"
                }
            };
            yield return new object[] { new Control { Enabled = false } };
            yield return new object[] { new Control { Visible = false } };
        }

        [WinFormsTheory]
        [MemberData(nameof(Ctor_Control_TestData))]
        public void ToolStripControlHost_Ctor_Control(Control c)
        {
            using var item = new SubToolStripControlHost(c);
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
            Assert.Equal(c.BackColor, item.BackColor);
            Assert.Equal(c.BackgroundImage, item.BackgroundImage);
            Assert.Equal(c.BackgroundImageLayout, item.BackgroundImageLayout);
            Assert.Equal(c.Bounds, item.Bounds);
            Assert.Equal(c.CanSelect, item.CanSelect);
            Assert.True(item.CanRaiseEvents);
            Assert.Equal(c.CausesValidation, item.CausesValidation);
            Assert.Null(item.Container);
            Assert.Equal(new Rectangle(2, 2, 0, 0), item.ContentRectangle);
            Assert.Same(c, item.Control);
            Assert.Equal(ContentAlignment.MiddleCenter, item.ControlAlign);
            Assert.False(item.DefaultAutoToolTip);
            Assert.Equal(ToolStripItemDisplayStyle.ImageAndText, item.DefaultDisplayStyle);
            Assert.Equal(new Padding(0, 1, 0, 2), item.DefaultMargin);
            Assert.Equal(Padding.Empty, item.DefaultPadding);
            Assert.Equal(c.Size, item.DefaultSize);
            Assert.False(item.DesignMode);
            Assert.True(item.DismissWhenClicked);
            Assert.Equal(ToolStripItemDisplayStyle.ImageAndText, item.DisplayStyle);
            Assert.Equal(DockStyle.None, item.Dock);
            Assert.False(item.DoubleClickEnabled);
            Assert.Equal(c.Enabled, item.Enabled);
            Assert.NotNull(item.Events);
            Assert.Same(item.Events, item.Events);
            Assert.Equal(c.Focused, item.Focused);
            Assert.Same(c.Font, item.Font);
            Assert.Equal(c.ForeColor, item.ForeColor);
            Assert.Equal(c.Height, item.Height);
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
            Assert.Equal(c.RightToLeft, item.RightToLeft);
            Assert.False(item.RightToLeftAutoMirrorImage);
            Assert.Equal(c.Focused, item.Selected);
            Assert.False(item.ShowKeyboardCues);
            Assert.Null(item.Site);
            Assert.Equal(c.Size, item.Size);
            Assert.Null(item.Tag);
            Assert.Equal(c.Text, item.Text);
            Assert.Equal(ContentAlignment.MiddleCenter, item.TextAlign);
            Assert.Equal(ToolStripTextDirection.Horizontal, item.TextDirection);
            Assert.Equal(TextImageRelation.ImageBeforeText, item.TextImageRelation);
            Assert.Null(item.ToolTipText);
            Assert.False(item.Visible);
            Assert.Equal(c.Width, item.Width);

            Assert.True(c.Visible);
            Assert.False(c.IsHandleCreated);
        }

        [WinFormsFact]
        public void ToolStripControlHost_Ctor_NullControl_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>("c", () => new SubToolStripControlHost(null));
            Assert.Throws<ArgumentNullException>("c", () => new SubToolStripControlHost(null, "name"));
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringTheoryData))]
        public void ToolStripControlHost_AccessibleDefaultActionDescription_Set_GetReturnsExpected(string value)
        {
            using var c = new Control();
            using var item = new ToolStripControlHost(c)
            {
                AccessibleDefaultActionDescription = value
            };
            Assert.Equal(value, item.AccessibleDefaultActionDescription);
            Assert.Equal(value, c.AccessibleDefaultActionDescription);
            Assert.False(c.IsHandleCreated);

            // Set same.
            item.AccessibleDefaultActionDescription = value;
            Assert.Equal(value, item.AccessibleDefaultActionDescription);
            Assert.Equal(value, c.AccessibleDefaultActionDescription);
            Assert.False(c.IsHandleCreated);
        }

        [WinFormsFact]
        public void ToolStripControlHost_AccessibleDefaultActionDescription_SetDisposed_ThrowsNullReferenceException()
        {
            using var c = new Control();
            using var item = new ToolStripControlHost(c);
            item.Dispose();

            Assert.Throws<NullReferenceException>(() => item.AccessibleDefaultActionDescription = "value");
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringTheoryData))]
        public void ToolStripControlHost_AccessibleDescription_Set_GetReturnsExpected(string value)
        {
            using var c = new Control();
            using var item = new ToolStripControlHost(c)
            {
                AccessibleDescription = value
            };
            Assert.Equal(value, item.AccessibleDescription);
            Assert.Equal(value, c.AccessibleDescription);
            Assert.False(c.IsHandleCreated);

            // Set same.
            item.AccessibleDescription = value;
            Assert.Equal(value, item.AccessibleDescription);
            Assert.Equal(value, c.AccessibleDescription);
            Assert.False(c.IsHandleCreated);
        }

        [WinFormsFact]
        public void ToolStripControlHost_AccessibleDescription_SetDisposed_ThrowsNullReferenceException()
        {
            using var c = new Control();
            using var item = new ToolStripControlHost(c);
            item.Dispose();

            Assert.Throws<NullReferenceException>(() => item.AccessibleDescription = "value");
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringTheoryData))]
        public void ToolStripControlHost_AccessibleName_Set_GetReturnsExpected(string value)
        {
            using var c = new Control();
            using var item = new ToolStripControlHost(c)
            {
                AccessibleName = value
            };
            Assert.Equal(value, item.AccessibleName);
            Assert.Equal(value, c.AccessibleName);
            Assert.False(c.IsHandleCreated);

            // Set same.
            item.AccessibleName = value;
            Assert.Equal(value, item.AccessibleName);
            Assert.Equal(value, c.AccessibleName);
            Assert.False(c.IsHandleCreated);
        }

        [WinFormsFact]
        public void ToolStripControlHost_AccessibleName_SetDisposed_ThrowsNullReferenceException()
        {
            using var c = new Control();
            using var item = new ToolStripControlHost(c);
            item.Dispose();

            Assert.Throws<NullReferenceException>(() => item.AccessibleName = "value");
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(AccessibleRole))]
        public void ToolStripControlHost_AccessibleRole_Set_GetReturnsExpected(AccessibleRole value)
        {
            using var c = new Control();
            using var item = new ToolStripControlHost(c)
            {
                AccessibleRole = value
            };
            Assert.Equal(value, item.AccessibleRole);
            Assert.Equal(value, c.AccessibleRole);
            Assert.False(c.IsHandleCreated);

            // Set same.
            item.AccessibleRole = value;
            Assert.Equal(value, item.AccessibleRole);
            Assert.Equal(value, c.AccessibleRole);
            Assert.False(c.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(AccessibleRole))]
        public void ToolStripControlHost_AccessibleRole_SetInvalid_ThrowsInvalidEnumArgumentException(AccessibleRole value)
        {
            using var c = new Control();
            using var item = new ToolStripControlHost(c);
            Assert.Throws<InvalidEnumArgumentException>("value", () => item.AccessibleRole = value);
        }

        [WinFormsFact]
        public void ToolStripControlHost_BackColor_GetDisposed_ThrowsNullReferenceException()
        {
            using var c = new Control();
            using var item = new ToolStripControlHost(c);
            item.Dispose();

            Assert.Throws<NullReferenceException>(() => item.BackColor);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBackColorTheoryData))]
        public void ToolStripControlHost_BackColor_Set_GetReturnsExpected(Color value, Color expected)
        {
            using var c = new Control();
            using var item = new SubToolStripControlHost(c)
            {
                BackColor = value
            };
            Assert.Equal(expected, item.BackColor);
            Assert.Equal(expected, c.BackColor);
            Assert.False(c.IsHandleCreated);

            // Set same.
            item.BackColor = value;
            Assert.Equal(expected, item.BackColor);
            Assert.Equal(expected, c.BackColor);
            Assert.False(c.IsHandleCreated);
        }

        [WinFormsFact]
        public void ToolStripControlHost_BackColor_SetWithHandler_CallsBackColorChanged()
        {
            using var c = new Control();
            using var item = new SubToolStripControlHost(c);
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
        public void ToolStripControlHost_BackColor_SetDisposed_ThrowsNullReferenceException()
        {
            using var c = new Control();
            using var item = new ToolStripControlHost(c);
            item.Dispose();

            Assert.Throws<NullReferenceException>(() => item.BackColor = Color.Red);
        }

        [WinFormsFact]
        public void ToolStripControlHost_BackColor_ResetValue_Success()
        {
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ToolStripControlHost))[nameof(ToolStripControlHost.BackColor)];
            using var c = new Control();
            using var item = new SubToolStripControlHost(c);
            Assert.False(property.CanResetValue(item));

            item.BackColor = Color.Red;
            Assert.Equal(Color.Red, item.BackColor);
            Assert.True(property.CanResetValue(item));

            property.ResetValue(item);
            Assert.Equal(Control.DefaultBackColor, item.BackColor);
            Assert.False(property.CanResetValue(item));
        }

        [WinFormsFact]
        public void ToolStripControlHost_BackColor_ResetValueDisposed_ThrowsNullReferenceException()
        {
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ToolStripControlHost))[nameof(ToolStripControlHost.BackColor)];
            using var c = new Control();
            using var item = new SubToolStripControlHost(c);
            item.Dispose();
            Assert.False(property.CanResetValue(item));

            TargetInvocationException ex = Assert.Throws<TargetInvocationException>(() => property.ResetValue(item));
            Assert.IsType<NullReferenceException>(ex.InnerException);
            Assert.False(property.CanResetValue(item));
        }

        [WinFormsFact]
        public void ToolStripControlHost_BackColor_ShouldSerializeValue_Success()
        {
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ToolStripControlHost))[nameof(ToolStripControlHost.BackColor)];
            using var c = new Control();
            using var item = new SubToolStripControlHost(c);
            Assert.False(property.ShouldSerializeValue(item));

            item.BackColor = Color.Red;
            Assert.Equal(Color.Red, item.BackColor);
            Assert.True(property.ShouldSerializeValue(item));

            property.ResetValue(item);
            Assert.Equal(Control.DefaultBackColor, item.BackColor);
            Assert.False(property.ShouldSerializeValue(item));
        }

        [WinFormsFact]
        public void ToolStripControlHost_BackColor_ShouldSerializeValueDisposed_Success()
        {
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ToolStripControlHost))[nameof(ToolStripControlHost.BackColor)];
            using var c = new Control();
            using var item = new SubToolStripControlHost(c);
            item.Dispose();
            Assert.False(property.ShouldSerializeValue(item));
        }

        [WinFormsFact]
        public void ToolStripControlHost_BackgroundImage_GetDisposed_ThrowsNullReferenceException()
        {
            using var c = new Control();
            using var item = new ToolStripControlHost(c);
            item.Dispose();

            Assert.Throws<NullReferenceException>(() => item.BackgroundImage);
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
        public void ToolStripControlHost_BackgroundImage_Set_GetReturnsExpected(Image value)
        {
            using var c = new Control();
            using var item = new ToolStripControlHost(c)
            {
                BackgroundImage = value
            };
            Assert.Equal(value, item.BackgroundImage);
            Assert.Equal(value, c.BackgroundImage);
            Assert.False(c.IsHandleCreated);

            // Set same.
            item.BackgroundImage = value;
            Assert.Equal(value, item.BackgroundImage);
            Assert.Equal(value, c.BackgroundImage);
            Assert.False(c.IsHandleCreated);
        }

        [WinFormsFact]
        public void ToolStripControlHost_BackgroundImage_SetDisposed_ThrowsNullReferenceException()
        {
            using var c = new Control();
            using var item = new ToolStripControlHost(c);
            item.Dispose();

            using var value = new Bitmap(10, 10);
            Assert.Throws<NullReferenceException>(() => item.BackgroundImage = value);
        }

        [WinFormsFact]
        public void ToolStripControlHost_BackgroundImageLayout_GetDisposed_ThrowsNullReferenceException()
        {
            using var c = new Control();
            using var item = new ToolStripControlHost(c);
            item.Dispose();

            Assert.Throws<NullReferenceException>(() => item.BackgroundImageLayout);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(ImageLayout))]
        public void ToolStripControlHost_BackgroundImageLayout_Set_GetReturnsExpected(ImageLayout value)
        {
            using var c = new Control();
            using var item = new ToolStripControlHost(c)
            {
                BackgroundImageLayout = value
            };
            Assert.Equal(value, item.BackgroundImageLayout);
            Assert.Equal(value, c.BackgroundImageLayout);
            Assert.False(c.IsHandleCreated);

            // Set same.
            item.BackgroundImageLayout = value;
            Assert.Equal(value, item.BackgroundImageLayout);
            Assert.Equal(value, c.BackgroundImageLayout);
            Assert.False(c.IsHandleCreated);
        }

        [WinFormsFact]
        public void ToolStripControlHost_BackgroundImageLayout_SetDisposed_ThrowsNullReferenceException()
        {
            using var c = new Control();
            using var item = new ToolStripControlHost(c);
            item.Dispose();

            Assert.Throws<NullReferenceException>(() => item.BackgroundImageLayout = ImageLayout.Zoom);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ToolStripControlHost_CanSelect_InvokeDesignMode_ReturnsExpected(bool enabled)
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
            mockSite
                .Setup(s => s.GetService(typeof(AmbientProperties)))
                .Returns(null);
            using var c = new Control
            {
                Enabled = enabled
            };
            using var item = new ToolStripControlHost(c)
            {
                Site = mockSite.Object
            };
            Assert.True(item.CanSelect);
        }

        [WinFormsFact]
        public void ToolStripControlHost_CanSelect_GetDisposed_ReturnsFalse()
        {
            using var c = new Control();
            using var item = new ToolStripControlHost(c);
            item.Dispose();
            Assert.False(item.CanSelect);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ToolStripControlHost_CausesValidation_Set_GetReturnsExpected(bool value)
        {
            using var c = new Control();
            using var item = new ToolStripControlHost(c)
            {
                CausesValidation = value
            };
            Assert.Equal(value, item.CausesValidation);
            Assert.Equal(value, c.CausesValidation);
            Assert.False(c.IsHandleCreated);

            // Set same
            item.CausesValidation = value;
            Assert.Equal(value, item.CausesValidation);
            Assert.Equal(value, c.CausesValidation);
            Assert.False(c.IsHandleCreated);

            // Set different
            item.CausesValidation = !value;
            Assert.Equal(!value, item.CausesValidation);
            Assert.Equal(!value, c.CausesValidation);
            Assert.False(c.IsHandleCreated);
        }

        [WinFormsFact]
        public void ToolStripControlHost_Control_ChangeBackColor_UpdatesHost()
        {
            using var c = new Control();
            using var item = new ToolStripControlHost(c);
            int callCount = 0;
            item.BackColorChanged += (sender, e) => callCount++;

            c.BackColor = Color.Red;
            Assert.Equal(Color.Red, c.BackColor);
            Assert.Equal(Color.Red, item.BackColor);
            Assert.Equal(1, callCount);
        }

        [WinFormsFact]
        public void ToolStripControlHost_Control_ChangeBackgroundImage_UpdatesHost()
        {
            using var c = new Control();
            using var item = new ToolStripControlHost(c);

            using var backgroundImage = new Bitmap(10, 10);
            c.BackgroundImage = backgroundImage;
            Assert.Same(backgroundImage, c.BackgroundImage);
            Assert.Same(backgroundImage, item.BackgroundImage);
        }

        [WinFormsFact]
        public void ToolStripControlHost_Control_ChangeBackgroundImageLayout_UpdatesHost()
        {
            using var c = new Control();
            using var item = new ToolStripControlHost(c);

            c.BackgroundImageLayout = ImageLayout.Zoom;
            Assert.Equal(ImageLayout.Zoom, c.BackgroundImageLayout);
            Assert.Equal(ImageLayout.Zoom, item.BackgroundImageLayout);
        }

        [WinFormsFact]
        public void ToolStripControlHost_Control_ChangeCausesValidation_UpdatesHost()
        {
            using var c = new Control();
            using var item = new ToolStripControlHost(c);

            c.CausesValidation = false;
            Assert.False(c.CausesValidation);
            Assert.False(item.CausesValidation);

            c.CausesValidation = true;
            Assert.True(c.CausesValidation);
            Assert.True(item.CausesValidation);
        }

        [WinFormsFact]
        public void ToolStripControlHost_Control_ChangeEnabled_UpdatesHost()
        {
            using var c = new Control();
            using var item = new ToolStripControlHost(c);
            int callCount = 0;
            item.EnabledChanged += (sender, e) => callCount++;

            c.Enabled = false;
            Assert.False(c.Enabled);
            Assert.False(item.Enabled);
            Assert.Equal(1, callCount);

            c.Enabled = true;
            Assert.True(c.Enabled);
            Assert.True(item.Enabled);
            Assert.Equal(2, callCount);
        }

        [WinFormsFact]
        public void ToolStripControlHost_Control_ChangeFont_UpdatesHost()
        {
            using var c = new Control();
            using var item = new ToolStripControlHost(c);

            using var value = new Font("Arial", 8.25f);
            c.Font = value;
            Assert.Same(value, c.Font);
            Assert.Same(value, item.Font);
        }

        [WinFormsFact]
        public void ToolStripControlHost_Control_ChangeForeColor_UpdatesHost()
        {
            using var c = new Control();
            using var item = new ToolStripControlHost(c);
            int callCount = 0;
            item.ForeColorChanged += (sender, e) => callCount++;

            c.ForeColor = Color.Red;
            Assert.Equal(Color.Red, c.ForeColor);
            Assert.Equal(Color.Red, item.ForeColor);
            Assert.Equal(1, callCount);
        }

        [WinFormsFact]
        public void ToolStripControlHost_Control_ChangeRightToLeft_UpdatesHost()
        {
            using var c = new Control();
            using var item = new ToolStripControlHost(c);
            int callCount = 0;
            item.RightToLeftChanged += (sender, e) => callCount++;

            c.RightToLeft = RightToLeft.Yes;
            Assert.Equal(RightToLeft.Yes, c.RightToLeft);
            Assert.Equal(RightToLeft.Yes, item.RightToLeft);
            Assert.Equal(1, callCount);
        }

        [WinFormsFact]
        public void ToolStripControlHost_Control_ChangeSize_GetReturnsExpected()
        {
            using var c = new Control();
            using var item = new ToolStripControlHost(c);

            c.Size = new Size(10, 20);
            Assert.Equal(new Size(10, 20), c.Size);
        }

        [WinFormsFact]
        public void ToolStripControlHost_Control_ChangeText_UpdatesHost()
        {
            using var c = new Control();
            using var item = new ToolStripControlHost(c);
            int callCount = 0;
            item.TextChanged += (sender, e) => callCount++;

            c.Text = "Text";
            Assert.Equal("Text", c.Text);
            Assert.Equal("Text", item.Text);
            Assert.Equal(1, callCount);
        }

        [WinFormsFact]
        public void ToolStripControlHost_Control_ChangeVisible_UpdatesHost()
        {
            using var c = new Control();
            using var item = new ToolStripControlHost(c);
            int callCount = 0;
            item.VisibleChanged += (sender, e) => callCount++;

            c.Visible = false;
            Assert.False(c.Visible);
            Assert.False(item.Available);
            Assert.False(item.Visible);
            Assert.Equal(1, callCount);

            c.Visible = true;
            Assert.True(c.Visible);
            Assert.True(item.Available);
            Assert.False(item.Visible);
            Assert.Equal(2, callCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(ContentAlignment))]
        public void ToolStripControlHost_ControlAlign_Set_GetReturnsExpected(ContentAlignment value)
        {
            using var c = new Control
            {
                Bounds = new Rectangle(10, 20, 30, 40)
            };
            using var item = new ToolStripControlHost(c)
            {
                ControlAlign = value
            };
            Assert.Equal(value, item.ControlAlign);
            Assert.Equal(new Rectangle(10, 20, 30, 40), item.Bounds);
            Assert.Equal(new Rectangle(10, 20, 30, 40), c.Bounds);
            Assert.False(c.IsHandleCreated);

            // Set same.
            item.ControlAlign = value;
            Assert.Equal(value, item.ControlAlign);
            Assert.Equal(new Rectangle(10, 20, 30, 40), item.Bounds);
            Assert.Equal(new Rectangle(10, 20, 30, 40), c.Bounds);
            Assert.False(c.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(ContentAlignment))]
        public void ToolStripControlHost_ControlAlign_SetDisposed_GetReturnsExpected(ContentAlignment value)
        {
            using var c = new Control();
            using var item = new ToolStripControlHost(c);
            item.Dispose();

            item.ControlAlign = value;
            Assert.Equal(value, item.ControlAlign);
            Assert.False(c.IsHandleCreated);

            // Set same.
            item.ControlAlign = value;
            Assert.Equal(value, item.ControlAlign);
            Assert.False(c.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(ContentAlignment))]
        [InlineData((ContentAlignment)int.MaxValue)]
        [InlineData((ContentAlignment)int.MinValue)]
        public void ToolStripControlHost_ControlAlign_SetInvalid_ThrowsInvalidEnumArgumentException(ContentAlignment value)
        {
            using var c = new Control();
            using var item = new ToolStripControlHost(c);
            Assert.Throws<InvalidEnumArgumentException>("value", () => item.ControlAlign = value);
        }

        [WinFormsFact]
        public void ToolStripControlHost_DefaultSize_GetDisposed_ReturnsExpected()
        {
            using var c = new Control();
            using var item = new SubToolStripControlHost(c);
            item.Dispose();

            Assert.Equal(new Size(23, 23), item.DefaultSize);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(ToolStripItemDisplayStyle))]
        public void ToolStripControlHost_DisplayStyle_Set_GetReturnsExpected(ToolStripItemDisplayStyle value)
        {
            using var c = new Control();
            using var item = new ToolStripControlHost(c)
            {
                DisplayStyle = value
            };
            Assert.Equal(value, item.DisplayStyle);
            Assert.False(c.IsHandleCreated);

            // Set same.
            item.DisplayStyle = value;
            Assert.Equal(value, item.DisplayStyle);
            Assert.False(c.IsHandleCreated);
        }

        [WinFormsFact]
        public void ToolStripControlHost_DisplayStyle_SetWithHandler_CallsDisplayStyleChanged()
        {
            using var c = new Control();
            using var item = new ToolStripControlHost(c);
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
        public void ToolStripControlHost_DisplayStyle_SetInvalid_ThrowsInvalidEnumArgumentException(ToolStripItemDisplayStyle value)
        {
            using var c = new Control();
            using var item = new ToolStripControlHost(c);
            Assert.Throws<InvalidEnumArgumentException>("value", () => item.DisplayStyle = value);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ToolStripControlHost_DoubleClickEnabled_Set_GetReturnsExpected(bool value)
        {
            using var c = new Control();
            using var item = new ToolStripControlHost(c)
            {
                DoubleClickEnabled = value
            };
            Assert.Equal(value, item.DoubleClickEnabled);
            Assert.False(c.IsHandleCreated);

            // Set same.
            item.DoubleClickEnabled = value;
            Assert.Equal(value, item.DoubleClickEnabled);
            Assert.False(c.IsHandleCreated);
        }

        [WinFormsFact]
        public void ToolStripControlHost_Enabled_GetDisposed_ThrowsNullReferenceException()
        {
            using var c = new Control();
            using var item = new ToolStripControlHost(c);
            item.Dispose();

            Assert.Throws<NullReferenceException>(() => item.Enabled);
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
        public void ToolStripControlHost_Enabled_Set_GetReturnsExpected(bool visible, Image image, bool value)
        {
            using var c = new Control();
            using var item = new ToolStripControlHost(c)
            {
                Visible = visible,
                Image = image,
                Enabled = value
            };
            Assert.Equal(value, item.Enabled);
            Assert.Equal(value, c.Enabled);
            Assert.False(c.IsHandleCreated);

            // Set same.
            item.Enabled = value;
            Assert.Equal(value, item.Enabled);
            Assert.Equal(value, c.Enabled);
            Assert.False(c.IsHandleCreated);

            // Set different.
            item.Enabled = !value;
            Assert.Equal(!value, item.Enabled);
            Assert.Equal(!value, c.Enabled);
            Assert.False(c.IsHandleCreated);
        }

        [WinFormsFact]
        public void ToolStripControlHost_Enabled_SetWithHandler_CallsEnabledChanged()
        {
            using var c = new Control();
            using var item = new ToolStripControlHost(c);
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
        public void ToolStripControlHost_Enabled_SetDisposed_ThrowsNullReferenceException()
        {
            using var c = new Control();
            using var item = new ToolStripControlHost(c);
            item.Dispose();

            Assert.Throws<NullReferenceException>(() => item.Enabled = false);
        }

        [WinFormsFact]
        public void ToolStripControlHost_Focused_GetDisposed_ThrowsNullReferenceException()
        {
            using var c = new Control();
            using var item = new ToolStripControlHost(c);
            item.Dispose();

            Assert.Throws<NullReferenceException>(() => item.Focused);
        }

        [WinFormsFact]
        public void ToolStripControlHost_Font_GetDisposed_ThrowsNullReferenceException()
        {
            using var c = new Control();
            using var item = new ToolStripControlHost(c);
            item.Dispose();

            Assert.Throws<NullReferenceException>(() => item.Font);
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
        public void ToolStripControlHost_Font_Set_GetReturnsExpected(ToolStripItemDisplayStyle displayStyle, Font value)
        {
            using var c = new Control();
            using var item = new ToolStripControlHost(c)
            {
                DisplayStyle = displayStyle
            };

            item.Font = value;
            Assert.Equal(value ?? Control.DefaultFont, item.Font);
            Assert.Equal(value ?? Control.DefaultFont, c.Font);
            Assert.False(c.IsHandleCreated);

            // Set same.
            item.Font = value;
            Assert.Equal(value ?? Control.DefaultFont, item.Font);
            Assert.Equal(value ?? Control.DefaultFont, c.Font);
            Assert.False(c.IsHandleCreated);
        }

        [WinFormsFact]
        public void ToolStripControlHost_Font_SetDisposed_ThrowsNullReferenceException()
        {
            using var c = new Control();
            using var item = new ToolStripControlHost(c);
            item.Dispose();

            using var font = new Font("Arial", 8.25f);
            Assert.Throws<NullReferenceException>(() => item.Font = font);
        }

        [WinFormsFact]
        public void ToolStripControlHost_Font_ResetValue_Success()
        {
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ToolStripControlHost))[nameof(ToolStripControlHost.Font)];
            using var c = new Control();
            using var item = new ToolStripControlHost(c);
            Assert.False(property.CanResetValue(item));

            using var font = new Font("Arial", 8.25f);
            item.Font = font;
            Assert.Same(font, item.Font);
            Assert.True(property.CanResetValue(item));

            item.Font = null;
            Assert.Equal(Control.DefaultFont, item.Font);
            Assert.False(property.CanResetValue(item));

            item.Font = font;
            Assert.Same(font, item.Font);
            Assert.True(property.CanResetValue(item));

            property.ResetValue(item);
            Assert.Equal(Control.DefaultFont, item.Font);
            Assert.False(property.CanResetValue(item));
        }

        [WinFormsFact]
        public void ToolStripControlHost_Font_ResetValueDisposed_ThrowsNullReferenceException()
        {
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ToolStripControlHost))[nameof(ToolStripControlHost.Font)];
            using var c = new Control();
            using var item = new ToolStripControlHost(c);
            item.Dispose();
            Assert.False(property.CanResetValue(item));

            TargetInvocationException ex = Assert.Throws<TargetInvocationException>(() => property.ResetValue(item));
            Assert.IsType<NullReferenceException>(ex.InnerException);
            Assert.False(property.CanResetValue(item));
        }

        [WinFormsFact]
        public void ToolStripControlHost_Font_ShouldSerializeValue_Success()
        {
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ToolStripControlHost))[nameof(ToolStripControlHost.Font)];
            using var c = new Control();
            using var item = new ToolStripControlHost(c);
            Assert.False(property.ShouldSerializeValue(item));

            using var font = new Font("Arial", 8.25f);
            item.Font = font;
            Assert.Same(font, item.Font);
            Assert.True(property.ShouldSerializeValue(item));

            item.Font = null;
            Assert.Equal(Control.DefaultFont, item.Font);
            Assert.False(property.ShouldSerializeValue(item));

            item.Font = font;
            Assert.Same(font, item.Font);
            Assert.True(property.ShouldSerializeValue(item));

            property.ResetValue(item);
            Assert.Equal(Control.DefaultFont, item.Font);
            Assert.False(property.ShouldSerializeValue(item));
        }

        [WinFormsFact]
        public void ToolStripControlHost_Font_ShouldSerializeValueDisposed_Success()
        {
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ToolStripControlHost))[nameof(ToolStripControlHost.Font)];
            using var c = new Control();
            using var item = new ToolStripControlHost(c);
            item.Dispose();
            Assert.False(property.ShouldSerializeValue(item));
        }

        [WinFormsFact]
        public void ToolStripControlHost_ForeColor_GetDisposed_ThrowsNullReferenceException()
        {
            using var c = new Control();
            using var item = new ToolStripControlHost(c);
            item.Dispose();

            Assert.Throws<NullReferenceException>(() => item.ForeColor);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetForeColorTheoryData))]
        public void ToolStripControlHost_ForeColor_Set_GetReturnsExpected(Color value, Color expected)
        {
            using var c = new Control();
            using var item = new SubToolStripControlHost(c)
            {
                ForeColor = value
            };
            Assert.Equal(expected, item.ForeColor);
            Assert.Equal(expected, c.ForeColor);
            Assert.False(c.IsHandleCreated);

            // Set same.
            item.ForeColor = value;
            Assert.Equal(expected, item.ForeColor);
            Assert.Equal(expected, c.ForeColor);
            Assert.False(c.IsHandleCreated);
        }

        [WinFormsFact]
        public void ToolStripControlHost_ForeColor_SetWithHandler_CallsForeColorChanged()
        {
            using var c = new Control();
            using var item = new SubToolStripControlHost(c);
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
        public void ToolStripControlHost_ForeColor_SetDisposed_ThrowsNullReferenceException()
        {
            using var c = new Control();
            using var item = new ToolStripControlHost(c);
            item.Dispose();

            Assert.Throws<NullReferenceException>(() => item.ForeColor = Color.Red);
        }

        [WinFormsFact]
        public void ToolStripControlHost_ForeColor_ResetValue_Success()
        {
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ToolStripControlHost))[nameof(ToolStripControlHost.ForeColor)];
            using var c = new Control();
            using var item = new ToolStripControlHost(c);
            Assert.False(property.CanResetValue(item));

            item.ForeColor = Color.Red;
            Assert.Equal(Color.Red, item.ForeColor);
            Assert.True(property.CanResetValue(item));

            property.ResetValue(item);
            Assert.Equal(Control.DefaultForeColor, item.ForeColor);
            Assert.False(property.CanResetValue(item));
        }

        [WinFormsFact]
        public void ToolStripControlHost_ForeColor_ResetValueDisposed_ThrowsNullReferenceException()
        {
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ToolStripControlHost))[nameof(ToolStripControlHost.ForeColor)];
            using var c = new Control();
            using var item = new ToolStripControlHost(c);
            item.Dispose();
            Assert.False(property.CanResetValue(item));

            TargetInvocationException ex = Assert.Throws<TargetInvocationException>(() => property.ResetValue(item));
            Assert.IsType<NullReferenceException>(ex.InnerException);
            Assert.False(property.CanResetValue(item));
        }

        [WinFormsFact]
        public void ToolStripControlHost_ForeColor_ShouldSerializeValue_Success()
        {
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ToolStripControlHost))[nameof(ToolStripControlHost.ForeColor)];
            using var c = new Control();
            using var item = new ToolStripControlHost(c);
            Assert.False(property.ShouldSerializeValue(item));

            item.ForeColor = Color.Red;
            Assert.Equal(Color.Red, item.ForeColor);
            Assert.True(property.ShouldSerializeValue(item));

            property.ResetValue(item);
            Assert.Equal(Control.DefaultForeColor, item.ForeColor);
            Assert.False(property.ShouldSerializeValue(item));
        }

        [WinFormsFact]
        public void ToolStripControlHost_ForeColor_ShouldSerializeValueDisposed_Success()
        {
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ToolStripControlHost))[nameof(ToolStripControlHost.ForeColor)];
            using var c = new Control();
            using var item = new ToolStripControlHost(c);
            item.Dispose();
            Assert.False(property.ShouldSerializeValue(item));
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
        public void ToolStripControlHost_Image_Set_GetReturnsExpected(Color imageTransparentColor, Image value)
        {
            using var c = new Control();
            using var item = new ToolStripControlHost(c)
            {
                ImageTransparentColor = imageTransparentColor
            };

            item.Image = value;
            Assert.Equal(value, item.Image);
            Assert.Equal(-1, item.ImageIndex);
            Assert.False(c.IsHandleCreated);

            // Set same.
            item.Image = value;
            Assert.Equal(value, item.Image);
            Assert.Equal(-1, item.ImageIndex);
            Assert.False(c.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(ContentAlignment))]
        public void ToolStripControlHost_ImageAlign_Set_GetReturnsExpected(ContentAlignment value)
        {
            using var c = new Control();
            using var item = new ToolStripControlHost(c)
            {
                ImageAlign = value
            };
            Assert.Equal(value, item.ImageAlign);
            Assert.False(c.IsHandleCreated);

            // Set same.
            item.ImageAlign = value;
            Assert.Equal(value, item.ImageAlign);
            Assert.False(c.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(ContentAlignment))]
        [InlineData((ContentAlignment)int.MaxValue)]
        [InlineData((ContentAlignment)int.MinValue)]
        public void ToolStripControlHost_ImageAlign_SetInvalid_ThrowsInvalidEnumArgumentException(ContentAlignment value)
        {
            using var c = new Control();
            using var item = new ToolStripControlHost(c);
            Assert.Throws<InvalidEnumArgumentException>("value", () => item.ImageAlign = value);
        }

        [WinFormsTheory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        public void ToolStripControlHost_ImageIndex_Set_GetReturnsExpected(int value)
        {
            using var c = new Control();
            using var item = new ToolStripControlHost(c)
            {
                ImageIndex = value
            };
            Assert.Equal(value, item.ImageIndex);
            Assert.Empty(item.ImageKey);
            Assert.Null(item.Image);
            Assert.False(c.IsHandleCreated);

            // Set same.
            item.ImageIndex = value;
            Assert.Equal(value, item.ImageIndex);
            Assert.Empty(item.ImageKey);
            Assert.Null(item.Image);
            Assert.False(c.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void ToolStripControlHost_ImageKey_Set_GetReturnsExpected(string value, string expected)
        {
            using var c = new Control();
            using var item = new ToolStripControlHost(c)
            {
                ImageKey = value
            };
            Assert.Equal(expected, item.ImageKey);
            Assert.Equal(-1, item.ImageIndex);
            Assert.Null(item.Image);
            Assert.False(c.IsHandleCreated);

            // Set same.
            item.ImageKey = value;
            Assert.Equal(expected, item.ImageKey);
            Assert.Equal(-1, item.ImageIndex);
            Assert.Null(item.Image);
            Assert.False(c.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(ToolStripItemImageScaling))]
        public void ToolStripControlHost_ImageScaling_Set_GetReturnsExpected(ToolStripItemImageScaling value)
        {
            using var c = new Control();
            using var item = new ToolStripControlHost(c)
            {
                ImageScaling = value
            };
            Assert.Equal(value, item.ImageScaling);
            Assert.False(c.IsHandleCreated);

            // Set same.
            item.ImageScaling = value;
            Assert.Equal(value, item.ImageScaling);
            Assert.False(c.IsHandleCreated);
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
        public void ToolStripControlHost_ImageTransparentColor_Set_GetReturnsExpected(Image image, Color value)
        {
            using var c = new Control();
            using var item = new ToolStripControlHost(c)
            {
                Image = image
            };

            item.ImageTransparentColor = value;
            Assert.Equal(value, item.ImageTransparentColor);
            Assert.False(c.IsHandleCreated);

            // Set same.
            item.ImageTransparentColor = value;
            Assert.Equal(value, item.ImageTransparentColor);
            Assert.False(c.IsHandleCreated);
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
        public void ToolStripControlHost_Parent_Set_GetReturnsExpected(bool enabled, bool visible, Image image, bool allowDrop)
        {
            using var parent = new ToolStrip();
            using var otherParent = new ToolStrip();
            using var statusParent = new StatusStrip();
            using var c = new Control();
            using var item = new SubToolStripControlHost(c)
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
            Assert.Same(c, Assert.Single(parent.Controls));
            Assert.Equal(new Padding(0, 1, 0, 2), item.Margin);

            // Set same.
            item.Parent = parent;
            Assert.Same(parent, item.Parent);
            Assert.Same(parent, item.GetCurrentParent());
            Assert.Null(item.Owner);
            Assert.Empty(parent.Items);
            Assert.Same(c, Assert.Single(parent.Controls));
            Assert.Empty(otherParent.Items);
            Assert.Empty(otherParent.Controls);
            Assert.Equal(new Padding(0, 1, 0, 2), item.Margin);

            // Set different.
            item.Parent = otherParent;
            Assert.Same(otherParent, item.Parent);
            Assert.Null(item.Owner);
            Assert.Empty(parent.Items);
            Assert.Empty(parent.Controls);
            Assert.Empty(otherParent.Items);
            Assert.Same(c, Assert.Single(otherParent.Controls));
            Assert.Equal(new Padding(0, 1, 0, 2), item.Margin);

            // Set null.
            item.Parent = null;
            Assert.Null(item.Parent);
            Assert.Null(item.Owner);
            Assert.Empty(parent.Controls);
            Assert.Empty(parent.Items);
            Assert.Empty(otherParent.Items);
            Assert.Empty(otherParent.Controls);
            Assert.Equal(new Padding(0, 1, 0, 2), item.Margin);

            // Set status strip.
            item.Parent = statusParent;
            Assert.Same(statusParent, item.Parent);
            Assert.Null(item.Owner);
            Assert.Empty(parent.Items);
            Assert.Empty(parent.Controls);
            Assert.Empty(otherParent.Items);
            Assert.Empty(otherParent.Controls);
            Assert.Empty(statusParent.Items);
            Assert.Same(c, Assert.Single(statusParent.Controls));
            Assert.Equal(new Padding(0, 1, 0, 2), item.Margin);
        }

        [WinFormsTheory]
        [MemberData(nameof(Parent_Set_TestData))]
        public void ToolStripControlHost_Parent_SetWithOwner_GetReturnsExpected(bool enabled, bool visible, Image image, bool allowDrop)
        {
            using var owner = new ToolStrip();
            using var parent = new ToolStrip();
            using var otherParent = new ToolStrip();
            using var statusParent = new StatusStrip();
            using var c = new Control();
            using var item = new SubToolStripControlHost(c)
            {
                Enabled = enabled,
                Visible = visible,
                Image = image,
                AllowDrop = allowDrop,
                Owner = owner,
                Parent = parent
            };
            Assert.Same(parent, item.Parent);
            Assert.Same(parent, item.GetCurrentParent());
            Assert.Same(owner, item.Owner);
            Assert.Same(item, Assert.Single(owner.Items));
            Assert.Empty(owner.Controls);
            Assert.Empty(parent.Items);
            Assert.Same(c, Assert.Single(parent.Controls));
            Assert.Equal(new Padding(0, 1, 0, 2), item.Margin);

            // Set same.
            item.Parent = parent;
            Assert.Same(parent, item.Parent);
            Assert.Same(parent, item.GetCurrentParent());
            Assert.Same(owner, item.Owner);
            Assert.Same(item, Assert.Single(owner.Items));
            Assert.Empty(owner.Controls);
            Assert.Empty(parent.Items);
            Assert.Same(c, Assert.Single(parent.Controls));
            Assert.Empty(otherParent.Items);
            Assert.Empty(otherParent.Controls);
            Assert.Equal(new Padding(0, 1, 0, 2), item.Margin);

            // Set different.
            item.Parent = otherParent;
            Assert.Same(otherParent, item.Parent);
            Assert.Same(owner, item.Owner);
            Assert.Same(item, Assert.Single(owner.Items));
            Assert.Empty(owner.Controls);
            Assert.Empty(parent.Items);
            Assert.Empty(parent.Controls);
            Assert.Empty(otherParent.Items);
            Assert.Same(c, Assert.Single(otherParent.Controls));
            Assert.Equal(new Padding(0, 1, 0, 2), item.Margin);

            // Set null.
            item.Parent = null;
            Assert.Null(item.Parent);
            Assert.Same(owner, item.Owner);
            Assert.Same(item, Assert.Single(owner.Items));
            Assert.Empty(owner.Controls);
            Assert.Empty(parent.Controls);
            Assert.Empty(parent.Items);
            Assert.Empty(otherParent.Items);
            Assert.Same(c, Assert.Single(otherParent.Controls));
            Assert.Equal(new Padding(0, 1, 0, 2), item.Margin);

            // Set status strip.
            item.Parent = statusParent;
            Assert.Same(statusParent, item.Parent);
            Assert.Same(owner, item.Owner);
            Assert.Same(item, Assert.Single(owner.Items));
            Assert.Empty(owner.Controls);
            Assert.Empty(parent.Items);
            Assert.Empty(parent.Controls);
            Assert.Empty(otherParent.Items);
            Assert.Empty(otherParent.Controls);
            Assert.Empty(statusParent.Items);
            Assert.Same(c, Assert.Single(statusParent.Controls));
            Assert.Equal(new Padding(0, 1, 0, 2), item.Margin);
        }

        [WinFormsFact]
        public void ToolStripControlHost_Parent_SetDisposed_GetReturnsExpected()
        {
            using var parent = new ToolStrip();
            using var c = new Control();
            using var item = new SubToolStripControlHost(c);
            item.Dispose();

            Assert.Throws<NullReferenceException>(() => item.Parent = parent);
            Assert.Same(parent, item.Parent);
            Assert.Same(parent, item.GetCurrentParent());
            Assert.Null(item.Owner);
            Assert.Empty(parent.Items);
            Assert.Empty(parent.Controls);
            Assert.Equal(new Padding(0, 1, 0, 2), item.Margin);
        }

        [WinFormsFact]
        public void ToolStripControlHost_RightToLeft_GetDisposed_ReturnsExpected()
        {
            using var c = new Control();
            using var item = new ToolStripControlHost(c);
            item.Dispose();

            Assert.Equal(RightToLeft.Inherit, item.RightToLeft);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(RightToLeft))]
        public void ToolStripControlHost_RightToLeft_SetDisposed_Nop(RightToLeft value)
        {
            using var c = new Control();
            using var item = new ToolStripControlHost(c);
            item.Dispose();

            item.RightToLeft = value;
            Assert.Equal(RightToLeft.Inherit, item.RightToLeft);
            Assert.Equal(RightToLeft.No, c.RightToLeft);
            Assert.False(c.IsHandleCreated);

            // Set same.
            item.RightToLeft = value;
            Assert.Equal(RightToLeft.Inherit, item.RightToLeft);
            Assert.Equal(RightToLeft.No, c.RightToLeft);
            Assert.False(c.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetRightToLeftTheoryData))]
        public void ToolStripControlHost_RightToLeft_Set_GetReturnsExpected(RightToLeft value, RightToLeft expected)
        {
            using var c = new Control();
            using var item = new ToolStripControlHost(c)
            {
                RightToLeft = value
            };
            Assert.Equal(expected, item.RightToLeft);
            Assert.Equal(expected, c.RightToLeft);
            Assert.False(c.IsHandleCreated);

            // Set same.
            item.RightToLeft = value;
            Assert.Equal(expected, item.RightToLeft);
            Assert.Equal(expected, c.RightToLeft);
            Assert.False(c.IsHandleCreated);
        }

        [WinFormsFact]
        public void ToolStripControlHost_RightToLeft_SetWithHandler_CallsRightToLeftChanged()
        {
            using var c = new Control();
            using var item = new ToolStripControlHost(c);
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
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(RightToLeft))]
        public void ToolStripControlHost_RightToLeft_SetInvalid_ThrowsInvalidEnumArgumentException(RightToLeft value)
        {
            using var c = new Control();
            using var item = new ToolStripControlHost(c);
            Assert.Throws<InvalidEnumArgumentException>("value", () => item.RightToLeft = value);
        }

        [WinFormsFact]
        public void ToolStripControlHost_RightToLeft_ResetValue_Success()
        {
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(SubToolStripControlHost))[nameof(ToolStripControlHost.RightToLeft)];
            using var c = new Control();
            using var item = new ToolStripControlHost(c);
            Assert.False(property.CanResetValue(item));

            item.RightToLeft = RightToLeft.Yes;
            Assert.Equal(RightToLeft.Yes, item.RightToLeft);
            Assert.True(property.CanResetValue(item));

            item.RightToLeft = RightToLeft.No;
            Assert.Equal(RightToLeft.No, item.RightToLeft);
            Assert.True(property.CanResetValue(item));

            item.RightToLeft = RightToLeft.Inherit;
            Assert.Equal(RightToLeft.No, item.RightToLeft);
            Assert.False(property.CanResetValue(item));

            item.RightToLeft = RightToLeft.No;
            Assert.Equal(RightToLeft.No, item.RightToLeft);
            Assert.True(property.CanResetValue(item));

            property.ResetValue(item);
            Assert.Equal(RightToLeft.No, item.RightToLeft);
            Assert.False(property.CanResetValue(item));
        }

        [WinFormsFact]
        public void ToolStripControlHost_RightToLeft_ResetValueDisposed_Success()
        {
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(SubToolStripControlHost))[nameof(ToolStripControlHost.RightToLeft)];
            using var c = new Control();
            using var item = new ToolStripControlHost(c);

            item.Dispose();
            Assert.False(property.CanResetValue(item));

            property.ResetValue(item);
            Assert.False(property.CanResetValue(item));
        }

        [WinFormsFact]
        public void ToolStripControlHost_RightToLeft_ShouldSerializeValue_Success()
        {
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(SubToolStripControlHost))[nameof(ToolStripControlHost.RightToLeft)];
            using var c = new Control();
            using var item = new ToolStripControlHost(c);
            Assert.False(property.ShouldSerializeValue(item));

            item.RightToLeft = RightToLeft.Yes;
            Assert.Equal(RightToLeft.Yes, item.RightToLeft);
            Assert.True(property.ShouldSerializeValue(item));

            item.RightToLeft = RightToLeft.No;
            Assert.Equal(RightToLeft.No, item.RightToLeft);
            Assert.True(property.ShouldSerializeValue(item));

            item.RightToLeft = RightToLeft.Inherit;
            Assert.Equal(RightToLeft.No, item.RightToLeft);
            Assert.False(property.ShouldSerializeValue(item));

            item.RightToLeft = RightToLeft.No;
            Assert.Equal(RightToLeft.No, item.RightToLeft);
            Assert.True(property.ShouldSerializeValue(item));

            property.ResetValue(item);
            Assert.Equal(RightToLeft.No, item.RightToLeft);
            Assert.False(property.ShouldSerializeValue(item));
        }

        [WinFormsFact]
        public void ToolStripControlHost_RightToLeft_ShouldSerializeValueDisposed_Success()
        {
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(SubToolStripControlHost))[nameof(ToolStripControlHost.RightToLeft)];
            using var c = new Control();
            using var item = new ToolStripControlHost(c);

            item.Dispose();
            Assert.False(property.ShouldSerializeValue(item));
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ToolStripControlHost_RightToLeftAutoMirrorImage_Set_GetReturnsExpected(bool value)
        {
            using var c = new Control();
            using var item = new ToolStripControlHost(c)
            {
                RightToLeftAutoMirrorImage = value
            };
            Assert.Equal(value, item.RightToLeftAutoMirrorImage);
            Assert.False(c.IsHandleCreated);

            // Set same.
            item.RightToLeftAutoMirrorImage = value;
            Assert.Equal(value, item.RightToLeftAutoMirrorImage);
            Assert.False(c.IsHandleCreated);

            // Set different.
            item.RightToLeftAutoMirrorImage = !value;
            Assert.Equal(!value, item.RightToLeftAutoMirrorImage);
            Assert.False(c.IsHandleCreated);
        }

        [WinFormsFact]
        public void ToolStripControlHost_Selected_GetDisposed_ReturnsFalse()
        {
            using var c = new Control();
            using var item = new ToolStripControlHost(c);
            item.Dispose();

            Assert.False(item.Selected);
        }

        public static IEnumerable<object[]> Site_Set_TestData()
        {
            foreach (string name in new string[] { null, string.Empty, "name" })
            {
                foreach (bool designMode in new bool[] { true, false })
                {
                    yield return new object[] { null, name, designMode };
                    yield return new object[] { new Container(), name, designMode };
                }
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(Site_Set_TestData))]
        public void ToolStripControlHost_Site_Set_GetReturnsExpected(IContainer container, string name, bool designMode)
        {
            using var c = new Control();
            using var item = new ToolStripControlHost(c);

            var mockSite = new Mock<ISite>(MockBehavior.Strict);
            mockSite
                .Setup(s => s.Container)
                .Returns(container);
            mockSite
                .Setup(s => s.DesignMode)
                .Returns(designMode);
            mockSite
                .Setup(s => s.Name)
                .Returns(name);
            mockSite
                .Setup(s => s.GetService(typeof(AmbientProperties)))
                .Returns(null);

            item.Site = mockSite.Object;
            Assert.Same(mockSite.Object, item.Site);
            Assert.NotSame(mockSite.Object, c.Site);
            Assert.Same(c, c.Site.Component);
            Assert.Same(container, c.Site.Container);
            Assert.Equal(designMode, c.Site.DesignMode);
            Assert.Equal(name, c.Site.Name);
            Assert.False(c.IsHandleCreated);

            // Set same.
            item.Site = mockSite.Object;
            Assert.Same(mockSite.Object, item.Site);
            Assert.NotSame(mockSite.Object, c.Site);
            Assert.Same(c, c.Site.Component);
            Assert.Same(container, c.Site.Container);
            Assert.Equal(designMode, c.Site.DesignMode);
            Assert.Equal(name, c.Site.Name);
            Assert.False(c.IsHandleCreated);

            // Set null.
            item.Site = null;
            Assert.Null(item.Site);
            Assert.Null(c.Site);
            Assert.False(c.IsHandleCreated);
        }

        [WinFormsFact]
        public void ToolStripControlHost_Site_GetControlSiteContainerAfterSettingNull_ThrowsNullReferenceException()
        {
            using var c = new Control();
            using var item = new ToolStripControlHost(c);

            using var container = new Container();
            var mockSite = new Mock<ISite>(MockBehavior.Strict);
            mockSite
                .Setup(s => s.Container)
                .Returns(container);
            mockSite
                .Setup(s => s.GetService(typeof(AmbientProperties)))
                .Returns(null);
            item.Site = mockSite.Object;
            ISite oldSite = c.Site;

            item.Site = null;
            Assert.Throws<NullReferenceException>(() => oldSite.Container);
        }

        [WinFormsFact]
        public void ToolStripControlHost_Site_GetControlSiteDesignModeAfterSettingNull_ThrowsNullReferenceException()
        {
            using var c = new Control();
            using var item = new ToolStripControlHost(c);

            using var container = new Container();
            var mockSite = new Mock<ISite>(MockBehavior.Strict);
            mockSite
                .Setup(s => s.Container)
                .Returns(container);
            mockSite
                .Setup(s => s.GetService(typeof(AmbientProperties)))
                .Returns(null);
            item.Site = mockSite.Object;
            ISite oldSite = c.Site;

            item.Site = null;
            Assert.Throws<NullReferenceException>(() => oldSite.DesignMode);
        }

        [WinFormsFact]
        public void ToolStripControlHost_Site_GetControlSiteNameAfterSettingNull_ThrowsNullReferenceException()
        {
            using var c = new Control();
            using var item = new ToolStripControlHost(c);

            using var container = new Container();
            var mockSite = new Mock<ISite>(MockBehavior.Strict);
            mockSite
                .Setup(s => s.Container)
                .Returns(container);
            mockSite
                .Setup(s => s.GetService(typeof(AmbientProperties)))
                .Returns(null);
            item.Site = mockSite.Object;
            ISite oldSite = c.Site;

            item.Site = null;
            Assert.Throws<NullReferenceException>(() => oldSite.Name);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void ToolStripControlHost_Site_SetControlSiteName_GetReturnsExpected(string value, string expected)
        {
            using var c = new Control();
            using var item = new ToolStripControlHost(c);

            using var container = new Container();
            var mockSite = new Mock<ISite>(MockBehavior.Strict);
            mockSite
                .Setup(s => s.Container)
                .Returns(container);
            mockSite
                .Setup(s => s.Name)
                .Returns(expected);
            mockSite
                .SetupSet(s => s.Name = value)
                .Verifiable();
            mockSite
                .Setup(s => s.GetService(typeof(AmbientProperties)))
                .Returns(null);
            item.Site = mockSite.Object;

            c.Site.Name = value;
            Assert.Equal(expected, item.Site.Name);
            Assert.Equal(expected, c.Site.Name);
            mockSite.VerifySet(s => s.Name = value, Times.Once());
            Assert.False(c.IsHandleCreated);

            // Set same.
            c.Site.Name = value;
            Assert.Equal(expected, item.Site.Name);
            Assert.Equal(expected, c.Site.Name);
            mockSite.VerifySet(s => s.Name = value, Times.Exactly(2));
            Assert.False(c.IsHandleCreated);
        }

        [WinFormsFact]
        public void ToolStripControlHost_Site_SetControlSiteNameAfterSettingNull_ThrowsNullReferenceException()
        {
            using var c = new Control();
            using var item = new ToolStripControlHost(c);

            using var container = new Container();
            var mockSite = new Mock<ISite>(MockBehavior.Strict);
            mockSite
                .Setup(s => s.Container)
                .Returns(container);
            mockSite
                .Setup(s => s.GetService(typeof(AmbientProperties)))
                .Returns(null);
            item.Site = mockSite.Object;
            ISite oldSite = c.Site;

            item.Site = null;
            Assert.Throws<NullReferenceException>(() => oldSite.Name = "name");
        }

        public static IEnumerable<object[]> Site_GetService_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new object() };
        }

        [WinFormsTheory]
        [MemberData(nameof(Site_GetService_TestData))]
        public void ToolStripControlHost_Site_ControlSiteGetService_ReturnsExpected(object result)
        {
            using var c = new Control();
            using var item = new ToolStripControlHost(c);

            var mockSite = new Mock<ISite>(MockBehavior.Strict);
            mockSite
                .Setup(s => s.Container)
                .Returns((IContainer)null);
            mockSite
                .Setup(s => s.GetService(typeof(AmbientProperties)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(int)))
                .Returns(result)
                .Verifiable();
            item.Site = mockSite.Object;
            Assert.Same(result, c.Site.GetService(typeof(int)));
            mockSite.Verify(s => s.GetService(typeof(int)), Times.Once());
            Assert.False(c.IsHandleCreated);
        }

        [WinFormsFact]
        public void ToolStripControlHost_Site_ControlSiteGetServiceIDictionaryServices_ReturnsExpected()
        {
            using var c = new Control();
            using var item = new ToolStripControlHost(c);

            var mockSite = new Mock<ISite>(MockBehavior.Strict);
            mockSite
                .Setup(s => s.Container)
                .Returns((IContainer)null);
            mockSite
                .Setup(s => s.GetService(typeof(AmbientProperties)))
                .Returns(null);
            item.Site = mockSite.Object;
            Assert.Same(c.Site, c.Site.GetService(typeof(IDictionaryService)));
            Assert.False(c.IsHandleCreated);
        }

        [WinFormsFact]
        public void ToolStripControlHost_Site_ControlSiteGetServiceAfterSettingNull_ReturnsExpected()
        {
            using var c = new Control();
            using var item = new ToolStripControlHost(c);

            var mockSite = new Mock<ISite>(MockBehavior.Strict);
            mockSite
                .Setup(s => s.Container)
                .Returns((IContainer)null);
            mockSite
                .Setup(s => s.GetService(typeof(AmbientProperties)))
                .Returns(null);
            item.Site = mockSite.Object;
            ISite oldSite = c.Site;

            item.Site = null;
            Assert.Null(oldSite.GetService(typeof(int)));
            Assert.False(c.IsHandleCreated);
        }

        [WinFormsFact]
        public void ToolStripControlHost_Site_ControlSiteGetServiceNullServiceType_ThrowsArgumentNullException()
        {
            using var c = new Control();
            using var item = new ToolStripControlHost(c);

            var mockSite = new Mock<ISite>(MockBehavior.Strict);
            mockSite
                .Setup(s => s.Container)
                .Returns((IContainer)null);
            mockSite
                .Setup(s => s.GetService(typeof(AmbientProperties)))
                .Returns(null);
            item.Site = mockSite.Object;
            Assert.Throws<ArgumentNullException>("service", () => c.Site.GetService(null));
        }

        [WinFormsTheory]
        [InlineData(null)]
        [InlineData("value")]
        public void ToolStripControlHost_Site_ControlSiteGetKey_NoSuchValueEmpty_ReturnsExpected(object value)
        {
            using var c = new Control();
            using var item = new ToolStripControlHost(c);

            var mockSite = new Mock<ISite>(MockBehavior.Strict);
            mockSite
                .Setup(s => s.Container)
                .Returns((IContainer)null);
            mockSite
                .Setup(s => s.GetService(typeof(AmbientProperties)))
                .Returns(null);
            item.Site = mockSite.Object;
            IDictionaryService iDictionaryService = Assert.IsAssignableFrom<IDictionaryService>(c.Site);
            Assert.Null(iDictionaryService.GetKey(value));
        }

        [WinFormsTheory]
        [InlineData(null)]
        [InlineData("value")]
        public void ToolStripControlHost_Site_ControlSiteGetKey_NoSuchValueNotEmpty_ReturnsExpected(object value)
        {
            using var c = new Control();
            using var item = new ToolStripControlHost(c);

            var mockSite = new Mock<ISite>(MockBehavior.Strict);
            mockSite
                .Setup(s => s.Container)
                .Returns((IContainer)null);
            mockSite
                .Setup(s => s.GetService(typeof(AmbientProperties)))
                .Returns(null);
            item.Site = mockSite.Object;
            IDictionaryService iDictionaryService = Assert.IsAssignableFrom<IDictionaryService>(c.Site);

            iDictionaryService.SetValue("key1", "value1");
            Assert.Null(iDictionaryService.GetKey(value));
        }

        [WinFormsTheory]
        [InlineData(null)]
        [InlineData("key")]
        public void ToolStripControlHost_Site_ControlSiteGetValue_NoSuchKeyEmpty_ReturnsExpected(object key)
        {
            using var c = new Control();
            using var item = new ToolStripControlHost(c);

            var mockSite = new Mock<ISite>(MockBehavior.Strict);
            mockSite
                .Setup(s => s.Container)
                .Returns((IContainer)null);
            mockSite
                .Setup(s => s.GetService(typeof(AmbientProperties)))
                .Returns(null);
            item.Site = mockSite.Object;
            IDictionaryService iDictionaryService = Assert.IsAssignableFrom<IDictionaryService>(c.Site);
            Assert.Null(iDictionaryService.GetValue(key));
        }

        [WinFormsTheory]
        [InlineData("key")]
        public void ToolStripControlHost_Site_ControlSiteGetValue_NoSuchKeyNotEmpty_ReturnsExpected(object key)
        {
            using var c = new Control();
            using var item = new ToolStripControlHost(c);

            var mockSite = new Mock<ISite>(MockBehavior.Strict);
            mockSite
                .Setup(s => s.Container)
                .Returns((IContainer)null);
            mockSite
                .Setup(s => s.GetService(typeof(AmbientProperties)))
                .Returns(null);
            item.Site = mockSite.Object;
            IDictionaryService iDictionaryService = Assert.IsAssignableFrom<IDictionaryService>(c.Site);

            iDictionaryService.SetValue("key1", "value1");
            Assert.Null(iDictionaryService.GetValue(key));
        }

        [WinFormsFact]
        public void ToolStripControlHost_Site_ControlSiteGetValue_NullKey_ThrowsArgumentNullException()
        {
            using var c = new Control();
            using var item = new ToolStripControlHost(c);

            var mockSite = new Mock<ISite>(MockBehavior.Strict);
            mockSite
                .Setup(s => s.Container)
                .Returns((IContainer)null);
            mockSite
                .Setup(s => s.GetService(typeof(AmbientProperties)))
                .Returns(null);
            item.Site = mockSite.Object;
            IDictionaryService iDictionaryService = Assert.IsAssignableFrom<IDictionaryService>(c.Site);

            iDictionaryService.SetValue("key1", "value1");
            Assert.Throws<ArgumentNullException>("key", () => iDictionaryService.GetValue(null));
        }

        [WinFormsFact]
        public void ToolStripControlHost_Site_ControlSiteSetValue_Invoke_Success()
        {
            using var c = new Control();
            using var item = new ToolStripControlHost(c);

            var mockSite = new Mock<ISite>(MockBehavior.Strict);
            mockSite
                .Setup(s => s.Container)
                .Returns((IContainer)null);
            mockSite
                .Setup(s => s.GetService(typeof(AmbientProperties)))
                .Returns(null);
            item.Site = mockSite.Object;
            IDictionaryService iDictionaryService = Assert.IsAssignableFrom<IDictionaryService>(c.Site);

            // Set custom.
            iDictionaryService.SetValue("key1", "value1");
            Assert.Equal("value1", iDictionaryService.GetValue("key1"));
            Assert.Equal("key1", iDictionaryService.GetKey("value1"));

            // Set same key.
            iDictionaryService.SetValue("key1", "otherValue");
            Assert.Equal("otherValue", iDictionaryService.GetValue("key1"));
            Assert.Equal("key1", iDictionaryService.GetKey("otherValue"));

            // Set null value.
            iDictionaryService.SetValue("key2", null);
            Assert.Null(iDictionaryService.GetValue("key2"));
            Assert.Null(iDictionaryService.GetKey(null));
        }

        [WinFormsFact]
        public void ToolStripControlHost_Site_ControlSiteSetValue_NullKey_ThrowsArgumentNullException()
        {
            using var c = new Control();
            using var item = new ToolStripControlHost(c);

            var mockSite = new Mock<ISite>(MockBehavior.Strict);
            mockSite
                .Setup(s => s.Container)
                .Returns((IContainer)null);
            mockSite
                .Setup(s => s.GetService(typeof(AmbientProperties)))
                .Returns(null);
            item.Site = mockSite.Object;
            IDictionaryService iDictionaryService = Assert.IsAssignableFrom<IDictionaryService>(c.Site);

            Assert.Throws<ArgumentNullException>("key", () => iDictionaryService.SetValue(null, "value"));
        }

        [WinFormsFact]
        public void ToolStripControlHost_Site_SetDisposed_ThrowsNullReferenceException()
        {
            using var c = new Control();
            using var item = new ToolStripControlHost(c);
            item.Dispose();

            var mockSite = new Mock<ISite>(MockBehavior.Strict);
            mockSite
                .Setup(s => s.Container)
                .Returns((IContainer)null);
            Assert.Throws<NullReferenceException>(() => item.Site = mockSite.Object);
            Assert.Equal(mockSite.Object, item.Site);
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
        public void ToolStripControlHost_Size_Set_GetReturnsExpected(Size value)
        {
            using var c = new Control
            {
                Bounds = new Rectangle(1, 2, 3, 4)
            };
            using var item = new ToolStripControlHost(c);
            int locationChangedCallCount = 0;
            item.LocationChanged += (sender, e) => locationChangedCallCount++;

            item.Size = value;
            Assert.Equal(value, item.Size);
            Assert.Equal(new Rectangle(1, 2, value.Width, value.Height), c.Bounds);
            Assert.Equal(0, locationChangedCallCount);

            // Set same.
            item.Size = value;
            Assert.Equal(value, item.Size);
            Assert.Equal(new Rectangle(1, 2, value.Width, value.Height), c.Bounds);
            Assert.Equal(0, locationChangedCallCount);
        }

        [WinFormsTheory]
        [MemberData(nameof(Size_Set_TestData))]
        public void ToolStripControlHost_Size_SetDisposed_GetReturnsExpected(Size value)
        {
            using var c = new Control();
            using var item = new ToolStripControlHost(c);
            item.Dispose();

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
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void ToolStripControlHost_Text_Set_GetReturnsExpected(string value, string expected)
        {
            using var c = new Control();
            using var item = new SubToolStripControlHost(c)
            {
                Text = value
            };
            Assert.Equal(expected, item.Text);
            Assert.Equal(expected, c.Text);
            Assert.False(c.IsHandleCreated);

            // Set same.
            item.Text = value;
            Assert.Equal(expected, item.Text);
            Assert.Equal(expected, c.Text);
            Assert.False(c.IsHandleCreated);
        }

        [WinFormsFact]
        public void ToolStripControlHost_Text_SetWithHandler_CallsTextChanged()
        {
            using var c = new Control();
            using var item = new SubToolStripControlHost(c);
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
        public void ToolStripControlHost_TextAlign_Set_GetReturnsExpected(ContentAlignment value)
        {
            using var c = new Control();
            using var item = new SubToolStripControlHost(c)
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
        public void ToolStripControlHost_TextAlign_SetInvalid_ThrowsInvalidEnumArgumentException(ContentAlignment value)
        {
            using var c = new Control();
            using var item = new SubToolStripControlHost(c);
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
        public void ToolStripControlHost_TextDirection_Set_GetReturnsExpected(ToolStripTextDirection value, ToolStripTextDirection expected)
        {
            using var c = new Control();
            using var item = new SubToolStripControlHost(c)
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
        public void ToolStripControlHost_TextDirection_SetInvalid_ThrowsInvalidEnumArgumentException(ToolStripTextDirection value)
        {
            using var c = new Control();
            using var item = new SubToolStripControlHost(c);
            Assert.Throws<InvalidEnumArgumentException>("value", () => item.TextDirection = value);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(TextImageRelation))]
        public void ToolStripControlHost_TextImageRelation_Set_GetReturnsExpected(TextImageRelation value)
        {
            using var c = new Control();
            using var item = new SubToolStripControlHost(c)
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
        public void ToolStripControlHost_TextImageRelation_SetInvalid_ThrowsInvalidEnumArgumentException(TextImageRelation value)
        {
            using var c = new Control();
            using var item = new SubToolStripControlHost(c);
            Assert.Throws<InvalidEnumArgumentException>("value", () => item.TextImageRelation = value);
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
        public void ToolStripControlHost_Visible_Set_GetReturnsExpected(bool enabled, Image image, bool value)
        {
            using var c = new Control();
            using var item = new SubToolStripControlHost(c)
            {
                Enabled = enabled,
                Image = image,
                Visible = value
            };
            Assert.False(item.Visible);
            Assert.Equal(value, item.Available);
            Assert.Equal(value, c.Visible);
            Assert.False(item.Selected);

            // Set same.
            item.Visible = value;
            Assert.False(item.Visible);
            Assert.Equal(value, item.Available);
            Assert.Equal(value, c.Visible);
            Assert.False(item.Selected);

            // Set different.
            item.Available = !value;
            Assert.False(item.Visible);
            Assert.Equal(!value, item.Available);
            Assert.Equal(!value, c.Visible);
            Assert.False(item.Selected);
        }

        [WinFormsFact]
        public void ToolStripControlHost_Visible_SetWithHandler_CallsAvailableChanged()
        {
            using var c = new Control();
            using var item = new SubToolStripControlHost(c);
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
        public void ToolStripControlHost_Visible_SetWithHandler_CallsVisibleChanged()
        {
            using var c = new Control();
            using var item = new SubToolStripControlHost(c);
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

        [WinFormsTheory]
        [InlineData(true, AccessibleRole.Client)]
        [InlineData(false, AccessibleRole.None)]
        public void ToolStripControlHost_CreateAccessibilityInstance_Invoke_ReturnsExpected(bool createControl, AccessibleRole expectedAccessibleRole)
        {
            using var c = new Control();
            if (createControl)
            {
                c.CreateControl();
            }

            Assert.Equal(createControl, c.IsHandleCreated);
            using var item = new SubToolStripControlHost(c);
            ToolStripItem.ToolStripItemAccessibleObject accessibleObject = Assert.IsAssignableFrom<ToolStripItem.ToolStripItemAccessibleObject>(item.CreateAccessibilityInstance());
            Assert.Equal(createControl, c.IsHandleCreated);
            Assert.Empty(accessibleObject.DefaultAction);
            Assert.Equal(expectedAccessibleRole, accessibleObject.Role);
            Assert.Equal(AccessibleStates.Focusable, accessibleObject.State);
            Assert.NotSame(accessibleObject, item.CreateAccessibilityInstance());
            Assert.NotSame(accessibleObject, item.AccessibilityObject);
        }

        [WinFormsFact]
        public void ToolStripControlHost_CreateAccessibilityInstance_InvokeWithCustomRole_ReturnsExpected()
        {
            using var c = new Control();
            using var item = new SubToolStripControlHost(c)
            {
                AccessibleDefaultActionDescription = "AccessibleDefaultActionDescription",
                AccessibleRole = AccessibleRole.HelpBalloon
            };
            c.AccessibleRole = AccessibleRole.Alert;
            ToolStripItem.ToolStripItemAccessibleObject accessibleObject = Assert.IsAssignableFrom<ToolStripItem.ToolStripItemAccessibleObject>(item.CreateAccessibilityInstance());
            Assert.Empty(accessibleObject.DefaultAction);
            Assert.Equal(AccessibleRole.Alert, accessibleObject.Role);
            Assert.Equal(AccessibleStates.Focusable, accessibleObject.State);
            Assert.NotSame(accessibleObject, item.CreateAccessibilityInstance());
            Assert.NotSame(accessibleObject, item.AccessibilityObject);
        }

        [WinFormsFact]
        public void ToolStripControlHost_CreateAccessibilityInstance_InvokeDisposed_ReturnsExpected()
        {
            using var c = new Control();
            using var item = new SubToolStripControlHost(c);
            item.Dispose();

            ToolStripItem.ToolStripItemAccessibleObject accessibleObject = Assert.IsAssignableFrom<ToolStripItem.ToolStripItemAccessibleObject>(item.CreateAccessibilityInstance());
            Assert.Empty(accessibleObject.DefaultAction);
            Assert.Equal(AccessibleRole.Default, accessibleObject.Role);
            Assert.Equal(AccessibleStates.None, accessibleObject.State);
            Assert.NotSame(accessibleObject, item.CreateAccessibilityInstance());
            Assert.NotSame(accessibleObject, item.AccessibilityObject);
        }

        [WinFormsFact]
        public void ToolStripControlHost_CreateAccessibilityInstanceDoDefaultAction_Nop()
        {
            using var c = new Control();
            using var item = new SubToolStripControlHost(c);
            ToolStripItem.ToolStripItemAccessibleObject accessibleObject = Assert.IsAssignableFrom<ToolStripItem.ToolStripItemAccessibleObject>(item.CreateAccessibilityInstance());
            accessibleObject.DoDefaultAction();

            // Call again.
            accessibleObject.DoDefaultAction();
        }

        [WinFormsFact]
        public void ToolStripControlHost_Dispose_Invoke_Success()
        {
            using var c = new Control();
            using var item = new SubToolStripControlHost(c);
            int callCount = 0;
            void handler(object sender, EventArgs e)
            {
                if (callCount == 0)
                {
                    Assert.Same(c, item.Control);
                    Assert.False(c.IsDisposed);
                }
                callCount++;
            };
            item.Disposed += handler;

            try
            {
                item.Dispose();
                Assert.False(item.IsDisposed);
                Assert.True(c.IsDisposed);
                Assert.Null(item.Control);
                Assert.Null(item.Image);
                Assert.Equal(1, callCount);

                // Dispose multiple times.
                item.Dispose();
                Assert.False(item.IsDisposed);
                Assert.True(c.IsDisposed);
                Assert.Null(item.Control);
                Assert.Null(item.Image);
                Assert.Equal(2, callCount);
            }
            finally
            {
                item.Disposed -= handler;
            }
        }

        [WinFormsFact]
        public void ToolStripControlHost_Dispose_InvokeDisposing_Success()
        {
            using var c = new Control();
            using var item = new SubToolStripControlHost(c);
            int callCount = 0;
            void handler(object sender, EventArgs e)
            {
                if (callCount == 0)
                {
                    Assert.Equal(c, item.Control);
                    Assert.False(c.IsDisposed);
                }
                callCount++;
            };
            item.Disposed += handler;

            try
            {
                item.Dispose(true);
                Assert.False(item.IsDisposed);
                Assert.True(c.IsDisposed);
                Assert.Null(item.Control);
                Assert.Null(item.Image);
                Assert.Equal(1, callCount);

                // Dispose multiple times.
                item.Dispose(true);
                Assert.False(item.IsDisposed);
                Assert.True(c.IsDisposed);
                Assert.Null(item.Control);
                Assert.Null(item.Image);
                Assert.Equal(2, callCount);
            }
            finally
            {
                item.Disposed -= handler;
            }
        }

        [WinFormsFact]
        public void ToolStripControlHost_Dispose_InvokeNotDisposing_Success()
        {
            using var c = new Control();
            using var item = new SubToolStripControlHost(c);
            int callCount = 0;
            void handler(object sender, EventArgs e) => callCount++;
            item.Disposed += handler;

            try
            {
                item.Dispose(false);
                Assert.False(item.IsDisposed);
                Assert.False(c.IsDisposed);
                Assert.Same(c, item.Control);
                Assert.Null(item.Image);
                Assert.Equal(0, callCount);

                // Dispose multiple times.
                item.Dispose(false);
                Assert.False(item.IsDisposed);
                Assert.False(c.IsDisposed);
                Assert.Same(c, item.Control);
                Assert.Null(item.Image);
                Assert.Equal(0, callCount);
            }
            finally
            {
                item.Disposed -= handler;
            }
        }

        [WinFormsFact]
        public void ToolStripControlHost_Focus_InvokeWithoutHandle_Nop()
        {
            using var c = new Control();
            using var item = new ToolStripControlHost(c);
            item.Focus();
            Assert.False(c.Focused);
            Assert.False(c.IsHandleCreated);
        }

        [WinFormsFact]
        public void ToolStripControlHost_Focus_InvokeDisposed_ThrowsNullReferenceException()
        {
            using var c = new Control();
            using var item = new ToolStripControlHost(c);
            item.Dispose();

            Assert.Throws<NullReferenceException>(() => item.Focus());
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
        public void ToolStripControlHost_GetPreferredSize_Invoke_ReturnsExpected(Size proposedSize)
        {
            using var c = new Control
            {
                MinimumSize = new Size(10, 20)
            };
            using var item = new ToolStripControlHost(c);
            Assert.Equal(new Size(10, 20), item.GetPreferredSize(proposedSize));

            // Call again.
            Assert.Equal(new Size(10, 20), item.GetPreferredSize(proposedSize));
        }

        [WinFormsTheory]
        [MemberData(nameof(GetPreferredSize_TestData))]
        public void ToolStripControlHost_GetPreferredSize_InvokeWithPadding_ReturnsExpected(Size proposedSize)
        {
            using var c = new Control
            {
                MinimumSize = new Size(10, 20)
            };
            using var item = new ToolStripControlHost(c)
            {
                Padding = new Padding(1, 2, 3, 4)
            };
            Assert.Equal(new Size(14, 26), item.GetPreferredSize(proposedSize));

            // Call again.
            Assert.Equal(new Size(14, 26), item.GetPreferredSize(proposedSize));
        }

        [WinFormsTheory]
        [MemberData(nameof(GetPreferredSize_TestData))]
        public void ToolStripControlHost_GetPreferredSize_InvokeDisposed_ThrowsNullReferenceException(Size proposedSize)
        {
            using var c = new Control();
            using var item = new ToolStripControlHost(c);
            item.Dispose();

            Assert.Throws<NullReferenceException>(() => item.GetPreferredSize(proposedSize));

            // Call again.
            Assert.Throws<NullReferenceException>(() => item.GetPreferredSize(proposedSize));
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(ContentAlignment))]
        public void ToolStripControlHost_OnBoundsChanged_Invoke_Success(ContentAlignment controlAlign)
        {
            using var c = new Control
            {
                Bounds = new Rectangle(10, 20, 30, 40)
            };
            using var item = new SubToolStripControlHost(c)
            {
                ControlAlign = controlAlign
            };

            item.OnBoundsChanged();
            Assert.Equal(new Rectangle(10, 20, 30, 40), item.Bounds);
            Assert.Equal(new Rectangle(10, 20, 30, 40), c.Bounds);
            Assert.False(c.IsHandleCreated);

            // Call again.
            item.OnBoundsChanged();
            Assert.Equal(new Rectangle(10, 20, 30, 40), item.Bounds);
            Assert.Equal(new Rectangle(10, 20, 30, 40), c.Bounds);
            Assert.False(c.IsHandleCreated);
        }

        [WinFormsFact]
        public void ToolStripControlHost_OnBoundsChanged_InvokeDisposed_Nop()
        {
            using var c = new Control();
            using var item = new SubToolStripControlHost(c);
            item.Dispose();

            item.OnBoundsChanged();

            // Call again.
            item.OnBoundsChanged();
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void ToolStripControlHost_OnEnter_Invoke_CallsEnter(EventArgs eventArgs)
        {
            using var c = new Control();
            using var item = new SubToolStripControlHost(c);
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(item, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            item.Enter += handler;
            item.OnEnter(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            item.Enter -= handler;
            item.OnEnter(eventArgs);
            Assert.Equal(1, callCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void ToolStripControlHost_OnGotFocus_Invoke_CallsGotFocus(EventArgs eventArgs)
        {
            using var c = new Control();
            using var item = new SubToolStripControlHost(c);
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(item, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            item.GotFocus += handler;
            item.OnGotFocus(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            item.GotFocus -= handler;
            item.OnGotFocus(eventArgs);
            Assert.Equal(1, callCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void ToolStripControlHost_OnHostedControlResize_Invoke_CallsHostedControlResize(EventArgs eventArgs)
        {
            using var c = new Control
            {
                Bounds = new Rectangle(1, 2, 3, 4)
            };
            using var item = new SubToolStripControlHost(c);

            item.OnHostedControlResize(eventArgs);
            Assert.Equal(new Rectangle(1, 2, 3, 4), item.Bounds);

            // Call again.
            item.OnHostedControlResize(eventArgs);
            Assert.Equal(new Rectangle(1, 2, 3, 4), item.Bounds);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetKeyEventArgsTheoryData))]
        public void ToolStripControlHost_OnKeyDown_Invoke_CallsKeyDown(KeyEventArgs eventArgs)
        {
            using var c = new Control();
            using var item = new SubToolStripControlHost(c);
            int callCount = 0;
            KeyEventHandler handler = (sender, e) =>
            {
                Assert.Same(item, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            item.KeyDown += handler;
            item.OnKeyDown(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            item.KeyDown -= handler;
            item.OnKeyDown(eventArgs);
            Assert.Equal(1, callCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetKeyPressEventArgsTheoryData))]
        public void ToolStripControlHost_OnKeyPress_Invoke_CallsKeyPress(KeyPressEventArgs eventArgs)
        {
            using var c = new Control();
            using var item = new SubToolStripControlHost(c);
            int callCount = 0;
            KeyPressEventHandler handler = (sender, e) =>
            {
                Assert.Same(item, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            item.KeyPress += handler;
            item.OnKeyPress(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            item.KeyPress -= handler;
            item.OnKeyPress(eventArgs);
            Assert.Equal(1, callCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetKeyEventArgsTheoryData))]
        public void ToolStripControlHost_OnKeyUp_Invoke_CallsKeyUp(KeyEventArgs eventArgs)
        {
            using var c = new Control();
            using var item = new SubToolStripControlHost(c);
            int callCount = 0;
            KeyEventHandler handler = (sender, e) =>
            {
                Assert.Same(item, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            item.KeyUp += handler;
            item.OnKeyUp(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            item.KeyUp -= handler;
            item.OnKeyUp(eventArgs);
            Assert.Equal(1, callCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetLayoutEventArgsTheoryData))]
        public void ToolStripControlHost_OnLayout_Invoke_Nop(LayoutEventArgs eventArgs)
        {
            using var c = new Control();
            using var item = new SubToolStripControlHost(c);

            item.OnLayout(eventArgs);

            // Call again.
            item.OnLayout(eventArgs);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void ToolStripControlHost_OnLeave_Invoke_CallsLeave(EventArgs eventArgs)
        {
            using var c = new Control();
            using var item = new SubToolStripControlHost(c);
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(item, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            item.Leave += handler;
            item.OnLeave(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            item.Leave -= handler;
            item.OnLeave(eventArgs);
            Assert.Equal(1, callCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void ToolStripControlHost_OnLostFocus_Invoke_CallsLostFocus(EventArgs eventArgs)
        {
            using var c = new Control();
            using var item = new SubToolStripControlHost(c);
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(item, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            item.LostFocus += handler;
            item.OnLostFocus(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            item.LostFocus -= handler;
            item.OnLostFocus(eventArgs);
            Assert.Equal(1, callCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetPaintEventArgsTheoryData))]
        public void ToolStripControlHost_OnPaint_Invoke_DoesNotCallPaint(PaintEventArgs eventArgs)
        {
            using var c = new Control();
            using var item = new SubToolStripControlHost(c);
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
        public void ToolStripControlHost_OnParentChanged_Invoke_Success(bool enabled, bool visible, Image image, bool allowDrop, ToolStrip oldParent, ToolStrip newParent)
        {
            using var c = new Control();
            using var item = new SubToolStripControlHost(c)
            {
                Enabled = enabled,
                Visible = visible,
                Image = image,
                AllowDrop = allowDrop
            };
            item.OnParentChanged(oldParent, newParent);
            Assert.Null(item.Owner);
            Assert.Empty(c.Controls);
            Assert.False(c.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(OnParentChanged_TestData))]
        public void ToolStripControlHost_OnParentChanged_InvokeWithOwner_Success(bool enabled, bool visible, Image image, bool allowDrop, ToolStrip oldParent, ToolStrip newParent)
        {
            using var owner = new ToolStrip();
            using var c = new Control();
            using var item = new SubToolStripControlHost(c)
            {
                Enabled = enabled,
                Visible = visible,
                Image = image,
                AllowDrop = allowDrop,
                Owner = owner
            };
            item.OnParentChanged(oldParent, newParent);
            Assert.Same(owner, item.Owner);
            Assert.Same(item, Assert.Single(owner.Items));
            Assert.Empty(owner.Controls);
            Assert.Empty(c.Controls);
            Assert.False(c.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(Parent_Set_TestData))]
        public void ToolStripControlHost_OnParentChanged_InvokeControlWithoutParent_Success(bool enabled, bool visible, Image image, bool allowDrop)
        {
            using var parent = new ToolStrip();
            using var otherParent = new ToolStrip();
            using var statusParent = new StatusStrip();
            using var c = new Control();
            using var item = new SubToolStripControlHost(c)
            {
                Enabled = enabled,
                Visible = visible,
                Image = image,
                AllowDrop = allowDrop
            };

            item.OnParentChanged(null, parent);
            Assert.Null(item.Parent);
            Assert.Null(item.GetCurrentParent());
            Assert.Null(item.Owner);
            Assert.Empty(parent.Items);
            Assert.Empty(parent.Controls);
            Assert.Equal(new Padding(0, 1, 0, 2), item.Margin);

            // Set same.
            item.OnParentChanged(parent, parent);
            Assert.Null(item.Parent);
            Assert.Null(item.GetCurrentParent());
            Assert.Null(item.Owner);
            Assert.Empty(parent.Items);
            Assert.Empty(parent.Controls);
            Assert.Empty(otherParent.Items);
            Assert.Empty(otherParent.Controls);
            Assert.Equal(new Padding(0, 1, 0, 2), item.Margin);

            // Set different.
            item.OnParentChanged(parent, otherParent);
            Assert.Null(item.Parent);
            Assert.Null(item.GetCurrentParent());
            Assert.Null(item.Owner);
            Assert.Empty(parent.Items);
            Assert.Empty(parent.Controls);
            Assert.Empty(otherParent.Items);
            Assert.Empty(otherParent.Controls);
            Assert.Equal(new Padding(0, 1, 0, 2), item.Margin);

            // Set null.
            item.OnParentChanged(otherParent, null);
            Assert.Null(item.Parent);
            Assert.Null(item.GetCurrentParent());
            Assert.Null(item.Owner);
            Assert.Empty(parent.Items);
            Assert.Empty(parent.Controls);
            Assert.Empty(otherParent.Items);
            Assert.Empty(otherParent.Controls);
            Assert.Equal(new Padding(0, 1, 0, 2), item.Margin);

            // Set status strip.
            item.OnParentChanged(null, statusParent);
            Assert.Null(item.Parent);
            Assert.Null(item.GetCurrentParent());
            Assert.Null(item.Owner);
            Assert.Empty(parent.Items);
            Assert.Empty(parent.Controls);
            Assert.Empty(otherParent.Items);
            Assert.Empty(otherParent.Controls);
            Assert.Equal(new Padding(0, 1, 0, 2), item.Margin);
        }

        [WinFormsTheory]
        [MemberData(nameof(Parent_Set_TestData))]
        public void ToolStripControlHost_OnParentChanged_InvokeControlWithParent_Success(bool enabled, bool visible, Image image, bool allowDrop)
        {
            using var parent = new ToolStrip();
            using var otherParent = new ToolStrip();
            using var statusParent = new StatusStrip();
            using var controlParent = new Control();
            using var c = new Control
            {
                Parent = controlParent
            };
            using var item = new SubToolStripControlHost(c)
            {
                Enabled = enabled,
                Visible = visible,
                Image = image,
                AllowDrop = allowDrop
            };

            item.OnParentChanged(null, parent);
            Assert.Null(item.Parent);
            Assert.Null(item.GetCurrentParent());
            Assert.Null(item.Owner);
            Assert.Empty(parent.Items);
            Assert.Empty(parent.Controls);
            Assert.Same(controlParent, c.Parent);
            Assert.Equal(new Padding(0, 1, 0, 2), item.Margin);

            // Set same.
            item.OnParentChanged(parent, parent);
            Assert.Null(item.Parent);
            Assert.Null(item.GetCurrentParent());
            Assert.Null(item.Owner);
            Assert.Empty(parent.Items);
            Assert.Empty(parent.Controls);
            Assert.Empty(otherParent.Items);
            Assert.Empty(otherParent.Controls);
            Assert.Same(controlParent, c.Parent);
            Assert.Equal(new Padding(0, 1, 0, 2), item.Margin);

            // Set different.
            item.OnParentChanged(parent, otherParent);
            Assert.Null(item.Parent);
            Assert.Null(item.GetCurrentParent());
            Assert.Null(item.Owner);
            Assert.Empty(parent.Items);
            Assert.Empty(parent.Controls);
            Assert.Empty(otherParent.Items);
            Assert.Empty(otherParent.Controls);
            Assert.Same(controlParent, c.Parent);
            Assert.Equal(new Padding(0, 1, 0, 2), item.Margin);

            // Set null.
            item.OnParentChanged(otherParent, null);
            Assert.Null(item.Parent);
            Assert.Null(item.GetCurrentParent());
            Assert.Null(item.Owner);
            Assert.Empty(parent.Items);
            Assert.Empty(parent.Controls);
            Assert.Empty(otherParent.Items);
            Assert.Empty(otherParent.Controls);
            Assert.Same(controlParent, c.Parent);
            Assert.Equal(new Padding(0, 1, 0, 2), item.Margin);

            // Set status strip.
            item.OnParentChanged(null, statusParent);
            Assert.Null(item.Parent);
            Assert.Null(item.GetCurrentParent());
            Assert.Null(item.Owner);
            Assert.Empty(parent.Items);
            Assert.Empty(parent.Controls);
            Assert.Empty(otherParent.Items);
            Assert.Empty(otherParent.Controls);
            Assert.Same(controlParent, c.Parent);
            Assert.Equal(new Padding(0, 1, 0, 2), item.Margin);
        }

        [WinFormsTheory]
        [MemberData(nameof(OnParentChanged_TestData))]
        public void ToolStripControlHost_OnParentChanged_InvokeDisposed_Success(bool enabled, bool visible, Image image, bool allowDrop, ToolStrip oldParent, ToolStrip newParent)
        {
            using var c = new Control();
            using var item = new SubToolStripControlHost(c)
            {
                Enabled = enabled,
                Visible = visible,
                Image = image,
                AllowDrop = allowDrop
            };
            item.Dispose();

            item.OnParentChanged(oldParent, newParent);
            Assert.Null(item.Owner);
            Assert.Empty(c.Controls);
            Assert.False(c.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(OnParentChanged_TestData))]
        public void ToolStripControlHost_OnParentChanged_InvokeDisposedWithOwner_Success(bool enabled, bool visible, Image image, bool allowDrop, ToolStrip oldParent, ToolStrip newParent)
        {
            using var owner = new ToolStrip();
            using var c = new Control();
            using var item = new SubToolStripControlHost(c)
            {
                Enabled = enabled,
                Visible = visible,
                Image = image,
                AllowDrop = allowDrop,
                Owner = owner
            };
            item.Dispose();

            item.OnParentChanged(oldParent, newParent);
            Assert.Null(item.Owner);
            Assert.Empty(c.Controls);
            Assert.False(c.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void ToolStripControlHost_OnSubscribeControlEvents_InvokeBackColorChanged_Success(EventArgs eventArgs)
        {
            using var c = new SubControl();
            using var item = new SubToolStripControlHost(c);
            item.OnSubscribeControlEvents(c);

            int callCount = 0;
            item.BackColorChanged += (sender, e) =>
            {
                Assert.Same(item, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            c.OnBackColorChanged(eventArgs);
            Assert.Equal(2, callCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void ToolStripControlHost_OnSubscribeControlEvents_InvokeClick_Success(EventArgs eventArgs)
        {
            using var c = new SubControl();
            using var item = new SubToolStripControlHost(c);
            item.OnSubscribeControlEvents(c);

            int callCount = 0;
            item.Click += (sender, e) =>
            {
                Assert.Same(item, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            c.OnClick(eventArgs);
            Assert.Equal(2, callCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void ToolStripControlHost_OnSubscribeControlEvents_InvokeDoubleClick_Success(EventArgs eventArgs)
        {
            using var c = new SubControl();
            using var item = new SubToolStripControlHost(c);
            item.OnSubscribeControlEvents(c);

            int callCount = 0;
            item.DoubleClick += (sender, e) =>
            {
                Assert.Same(item, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            c.OnDoubleClick(eventArgs);
            Assert.Equal(2, callCount);
        }

        public static IEnumerable<object[]> DragEventArgs_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new DragEventArgs(null, 1, 2, 3, DragDropEffects.Copy, DragDropEffects.Move) };
        }

        [WinFormsTheory]
        [MemberData(nameof(DragEventArgs_TestData))]
        public void ToolStripControlHost_OnSubscribeControlEvents_InvokeDragDrop_Success(DragEventArgs eventArgs)
        {
            using var c = new SubControl();
            using var item = new SubToolStripControlHost(c);
            item.OnSubscribeControlEvents(c);

            int callCount = 0;
            item.DragDrop += (sender, e) =>
            {
                Assert.Same(item, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            c.OnDragDrop(eventArgs);
            Assert.Equal(2, callCount);
        }

        [WinFormsTheory]
        [MemberData(nameof(DragEventArgs_TestData))]
        public void ToolStripControlHost_OnSubscribeControlEvents_InvokeDragEnter_Success(DragEventArgs eventArgs)
        {
            using var c = new SubControl();
            using var item = new SubToolStripControlHost(c);
            item.OnSubscribeControlEvents(c);

            int callCount = 0;
            item.DragEnter += (sender, e) =>
            {
                Assert.Same(item, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            c.OnDragEnter(eventArgs);
            Assert.Equal(2, callCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void ToolStripControlHost_OnSubscribeControlEvents_InvokeDragLeave_Success(EventArgs eventArgs)
        {
            using var c = new SubControl();
            using var item = new SubToolStripControlHost(c);
            item.OnSubscribeControlEvents(c);

            int callCount = 0;
            item.DragLeave += (sender, e) =>
            {
                Assert.Same(item, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            c.OnDragLeave(eventArgs);
            Assert.Equal(2, callCount);
        }

        [WinFormsTheory]
        [MemberData(nameof(DragEventArgs_TestData))]
        public void ToolStripControlHost_OnSubscribeControlEvents_InvokeDragOver_Success(DragEventArgs eventArgs)
        {
            using var c = new SubControl();
            using var item = new SubToolStripControlHost(c);
            item.OnSubscribeControlEvents(c);

            int callCount = 0;
            item.DragOver += (sender, e) =>
            {
                Assert.Same(item, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            c.OnDragOver(eventArgs);
            Assert.Equal(2, callCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void ToolStripControlHost_OnSubscribeControlEvents_InvokeEnabledChanged_Success(EventArgs eventArgs)
        {
            using var c = new SubControl();
            using var item = new SubToolStripControlHost(c);
            item.OnSubscribeControlEvents(c);

            int callCount = 0;
            item.EnabledChanged += (sender, e) =>
            {
                Assert.Same(item, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            c.OnEnabledChanged(eventArgs);
            Assert.Equal(2, callCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void ToolStripControlHost_OnSubscribeControlEvents_InvokeEnter_Success(EventArgs eventArgs)
        {
            using var c = new SubControl();
            using var item = new SubToolStripControlHost(c);
            item.OnSubscribeControlEvents(c);

            int callCount = 0;
            item.Enter += (sender, e) =>
            {
                Assert.Same(item, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            c.OnEnter(eventArgs);
            Assert.Equal(2, callCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void ToolStripControlHost_OnSubscribeControlEvents_InvokeForeColorChanged_Success(EventArgs eventArgs)
        {
            using var c = new SubControl();
            using var item = new SubToolStripControlHost(c);
            item.OnSubscribeControlEvents(c);

            int callCount = 0;
            item.ForeColorChanged += (sender, e) =>
            {
                Assert.Same(item, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            c.OnForeColorChanged(eventArgs);
            Assert.Equal(2, callCount);
        }

        public static IEnumerable<object[]> GiveFeedbackEventArgs_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new GiveFeedbackEventArgs(DragDropEffects.None, true) };
        }

        [WinFormsTheory]
        [MemberData(nameof(GiveFeedbackEventArgs_TestData))]
        public void ToolStripControlHost_OnSubscribeControlEvents_InvokeGiveFeedback_Success(GiveFeedbackEventArgs eventArgs)
        {
            using var c = new SubControl();
            using var item = new SubToolStripControlHost(c);
            item.OnSubscribeControlEvents(c);

            int callCount = 0;
            item.GiveFeedback += (sender, e) =>
            {
                Assert.Same(item, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            c.OnGiveFeedback(eventArgs);
            Assert.Equal(2, callCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void ToolStripControlHost_OnSubscribeControlEvents_InvokeGotFocus_Success(EventArgs eventArgs)
        {
            using var c = new SubControl();
            using var item = new SubToolStripControlHost(c);
            item.OnSubscribeControlEvents(c);

            int callCount = 0;
            item.GotFocus += (sender, e) =>
            {
                Assert.Same(item, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            c.OnGotFocus(eventArgs);
            Assert.Equal(2, callCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetKeyEventArgsTheoryData))]
        public void ToolStripControlHost_OnSubscribeControlEvents_InvokeKeyDown_Success(KeyEventArgs eventArgs)
        {
            using var c = new SubControl();
            using var item = new SubToolStripControlHost(c);
            item.OnSubscribeControlEvents(c);

            int callCount = 0;
            item.KeyDown += (sender, e) =>
            {
                Assert.Same(item, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            c.OnKeyDown(eventArgs);
            Assert.Equal(2, callCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetKeyPressEventArgsTheoryData))]
        public void ToolStripControlHost_OnSubscribeControlEvents_InvokeKeyPress_Success(KeyPressEventArgs eventArgs)
        {
            using var c = new SubControl();
            using var item = new SubToolStripControlHost(c);
            item.OnSubscribeControlEvents(c);

            int callCount = 0;
            item.KeyPress += (sender, e) =>
            {
                Assert.Same(item, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            c.OnKeyPress(eventArgs);
            Assert.Equal(2, callCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetKeyEventArgsTheoryData))]
        public void ToolStripControlHost_OnSubscribeControlEvents_InvokeKeyUp_Success(KeyEventArgs eventArgs)
        {
            using var c = new SubControl();
            using var item = new SubToolStripControlHost(c);
            item.OnSubscribeControlEvents(c);

            int callCount = 0;
            item.KeyUp += (sender, e) =>
            {
                Assert.Same(item, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            c.OnKeyUp(eventArgs);
            Assert.Equal(2, callCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void ToolStripControlHost_OnSubscribeControlEvents_InvokeLeave_Success(EventArgs eventArgs)
        {
            using var c = new SubControl();
            using var item = new SubToolStripControlHost(c);
            item.OnSubscribeControlEvents(c);

            int callCount = 0;
            item.Leave += (sender, e) =>
            {
                Assert.Same(item, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            c.OnLeave(eventArgs);
            Assert.Equal(2, callCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void ToolStripControlHost_OnSubscribeControlEvents_InvokeLocationChanged_Success(EventArgs eventArgs)
        {
            using var c = new SubControl();
            using var item = new SubToolStripControlHost(c);
            item.OnSubscribeControlEvents(c);

            int callCount = 0;
            item.LocationChanged += (sender, e) =>
            {
                Assert.Same(item, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            c.OnLocationChanged(eventArgs);
            Assert.Equal(2, callCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void ToolStripControlHost_OnSubscribeControlEvents_InvokeLostFocus_Success(EventArgs eventArgs)
        {
            using var c = new SubControl();
            using var item = new SubToolStripControlHost(c);
            item.OnSubscribeControlEvents(c);

            int callCount = 0;
            item.LostFocus += (sender, e) =>
            {
                Assert.Same(item, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            c.OnLostFocus(eventArgs);
            Assert.Equal(2, callCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetMouseEventArgsTheoryData))]
        public void ToolStripControlHost_OnSubscribeControlEvents_InvokeMouseDown_Success(MouseEventArgs eventArgs)
        {
            using var c = new SubControl();
            using var item = new SubToolStripControlHost(c);
            item.OnSubscribeControlEvents(c);

            int callCount = 0;
            item.MouseDown += (sender, e) =>
            {
                Assert.Same(item, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            c.OnMouseDown(eventArgs);
            Assert.Equal(2, callCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void ToolStripControlHost_OnSubscribeControlEvents_InvokeMouseEnter_Success(EventArgs eventArgs)
        {
            using var c = new SubControl();
            using var item = new SubToolStripControlHost(c);
            item.OnSubscribeControlEvents(c);

            int callCount = 0;
            item.MouseEnter += (sender, e) =>
            {
                Assert.Same(item, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            c.OnMouseEnter(eventArgs);
            Assert.Equal(2, callCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void ToolStripControlHost_OnSubscribeControlEvents_InvokeMouseHover_Success(EventArgs eventArgs)
        {
            using var c = new SubControl();
            using var item = new SubToolStripControlHost(c);
            item.OnSubscribeControlEvents(c);

            int callCount = 0;
            item.MouseHover += (sender, e) =>
            {
                Assert.Same(item, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            c.OnMouseHover(eventArgs);
            Assert.Equal(2, callCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void ToolStripControlHost_OnSubscribeControlEvents_InvokeMouseLeave_Success(EventArgs eventArgs)
        {
            using var c = new SubControl();
            using var item = new SubToolStripControlHost(c);
            item.OnSubscribeControlEvents(c);

            int callCount = 0;
            item.MouseLeave += (sender, e) =>
            {
                Assert.Same(item, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            c.OnMouseLeave(eventArgs);
            Assert.Equal(2, callCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetMouseEventArgsTheoryData))]
        public void ToolStripControlHost_OnSubscribeControlEvents_InvokeMouseMove_Success(MouseEventArgs eventArgs)
        {
            using var c = new SubControl();
            using var item = new SubToolStripControlHost(c);
            item.OnSubscribeControlEvents(c);

            int callCount = 0;
            item.MouseMove += (sender, e) =>
            {
                Assert.Same(item, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            c.OnMouseMove(eventArgs);
            Assert.Equal(2, callCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetMouseEventArgsTheoryData))]
        public void ToolStripControlHost_OnSubscribeControlEvents_InvokeMouseUp_Success(MouseEventArgs eventArgs)
        {
            using var c = new SubControl();
            using var item = new SubToolStripControlHost(c);
            item.OnSubscribeControlEvents(c);

            int callCount = 0;
            item.MouseUp += (sender, e) =>
            {
                Assert.Same(item, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            c.OnMouseUp(eventArgs);
            Assert.Equal(2, callCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetPaintEventArgsTheoryData))]
        public void ToolStripControlHost_OnSubscribeControlEvents_InvokePaint_Success(PaintEventArgs eventArgs)
        {
            using var c = new SubControl();
            using var item = new SubToolStripControlHost(c);
            item.OnSubscribeControlEvents(c);

            int callCount = 0;
            item.Paint += (sender, e) =>
            {
                Assert.Same(item, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            c.OnPaint(eventArgs);
            Assert.Equal(2, callCount);
        }

        public static IEnumerable<object[]> QueryContinueDragEventArgs_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new QueryContinueDragEventArgs(0, true, DragAction.Drop) };
        }

        [WinFormsTheory]
        [MemberData(nameof(QueryContinueDragEventArgs_TestData))]
        public void ToolStripControlHost_OnSubscribeControlEvents_InvokeQueryContinueDrag_Success(QueryContinueDragEventArgs eventArgs)
        {
            using var c = new SubControl();
            using var item = new SubToolStripControlHost(c);
            item.OnSubscribeControlEvents(c);

            int callCount = 0;
            item.QueryContinueDrag += (sender, e) =>
            {
                Assert.Same(item, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            c.OnQueryContinueDrag(eventArgs);
            Assert.Equal(2, callCount);
        }

        [WinFormsFact]
        public void ToolStripControlHost_OnSubscribeControlEvents_InvokeQueryAccessibilityHelp_Success()
        {
            using var c = new SubControl();
            using var item = new SubToolStripControlHost(c);
            item.OnSubscribeControlEvents(c);

            int callCount = 0;
            item.QueryAccessibilityHelp += (sender, e) =>
            {
                Assert.Same(item, sender);
                e.HelpKeyword = "1";
                e.HelpNamespace = "FileName";
                callCount++;
            };

            Assert.Equal(1, c.AccessibilityObject.GetHelpTopic(out string fileName));
            Assert.Equal("FileName", fileName);
            Assert.Equal(2, callCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void ToolStripControlHost_OnSubscribeControlEvents_InvokeResize_Success(EventArgs eventArgs)
        {
            using var c = new SubControl
            {
                Bounds = new Rectangle(1, 2, 3, 4)
            };
            using var item = new SubToolStripControlHost(c);
            item.OnSubscribeControlEvents(c);

            c.OnResize(eventArgs);
            Assert.Equal(new Rectangle(1, 2, 3, 4), item.Bounds);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void ToolStripControlHost_OnSubscribeControlEvents_InvokeRightToLeftChanged_Success(EventArgs eventArgs)
        {
            using var c = new SubControl();
            using var item = new SubToolStripControlHost(c);
            item.OnSubscribeControlEvents(c);

            int callCount = 0;
            item.RightToLeftChanged += (sender, e) =>
            {
                Assert.Same(item, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            c.OnRightToLeftChanged(eventArgs);
            Assert.Equal(2, callCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void ToolStripControlHost_OnSubscribeControlEvents_InvokeTextChanged_Success(EventArgs eventArgs)
        {
            using var c = new SubControl();
            using var item = new SubToolStripControlHost(c);
            item.OnSubscribeControlEvents(c);

            int callCount = 0;
            item.TextChanged += (sender, e) =>
            {
                Assert.Same(item, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            c.OnTextChanged(eventArgs);
            Assert.Equal(2, callCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void ToolStripControlHost_OnSubscribeControlEvents_InvokeValidated_Success(EventArgs eventArgs)
        {
            using var c = new SubControl();
            using var item = new SubToolStripControlHost(c);
            item.OnSubscribeControlEvents(c);

            int callCount = 0;
            item.Validated += (sender, e) =>
            {
                Assert.Same(item, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            c.OnValidated(eventArgs);
            Assert.Equal(2, callCount);
        }

        public static IEnumerable<object[]> CancelEventArgs_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new CancelEventArgs() };
            yield return new object[] { new CancelEventArgs(true) };
        }

        [WinFormsTheory]
        [MemberData(nameof(CancelEventArgs_TestData))]
        public void ToolStripControlHost_OnSubscribeControlEvents_InvokeValidating_Success(CancelEventArgs eventArgs)
        {
            using var c = new SubControl();
            using var item = new SubToolStripControlHost(c);
            item.OnSubscribeControlEvents(c);

            int callCount = 0;
            item.Validating += (sender, e) =>
            {
                Assert.Same(item, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            c.OnValidating(eventArgs);
            Assert.Equal(2, callCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void ToolStripControlHost_OnSubscribeControlEvents_InvokeVisibleChanged_Success(EventArgs eventArgs)
        {
            using var c = new SubControl();
            using var item = new SubToolStripControlHost(c);
            item.OnSubscribeControlEvents(c);

            int callCount = 0;
            item.VisibleChanged += (sender, e) =>
            {
                Assert.Same(item, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            c.OnVisibleChanged(eventArgs);
            Assert.Equal(0, callCount);
        }

        [WinFormsTheory]
        [InlineData(true, 0)]
        [InlineData(false, -1)]
        public void ToolStripControlHost_OnUnsubscribeControlEvents_Invoke_Success(bool createControl, int expectedResult)
        {
            using var c = new SubControl();

            if (createControl)
            {
                c.CreateControl();
            }

            using var item = new SubToolStripControlHost(c);
            item.OnUnsubscribeControlEvents(c);

            int backColorChangedCallCount = 0;
            item.BackColorChanged += (sender, e) => backColorChangedCallCount++;
            int clickCallCount = 0;
            item.Click += (sender, e) => clickCallCount++;
            int doubleClickCallCount = 0;
            item.Click += (sender, e) => doubleClickCallCount++;
            int dragDropCallCount = 0;
            item.DragDrop += (sender, e) => dragDropCallCount++;
            int dragEnterCallCount = 0;
            item.DragEnter += (sender, e) => dragEnterCallCount++;
            int dragLeaveCallCount = 0;
            item.DragLeave += (sender, e) => dragLeaveCallCount++;
            int dragOverCallCount = 0;
            item.DragOver += (sender, e) => dragOverCallCount++;
            int enabledChangedCallCount = 0;
            item.EnabledChanged += (sender, e) => enabledChangedCallCount++;
            int enterCallCount = 0;
            item.Enter += (sender, e) => enterCallCount++;
            int foreColorChangedCallCount = 0;
            item.Enter += (sender, e) => foreColorChangedCallCount++;
            int giveFeedbackCallCount = 0;
            item.GiveFeedback += (sender, e) => giveFeedbackCallCount++;
            int gotFocusCallCount = 0;
            item.GotFocus += (sender, e) => gotFocusCallCount++;
            int keyDownCallCount = 0;
            item.KeyDown += (sender, e) => keyDownCallCount++;
            int keyPressCallCount = 0;
            item.KeyPress += (sender, e) => keyPressCallCount++;
            int keyUpCallCount = 0;
            item.KeyUp += (sender, e) => keyUpCallCount++;
            int leaveCallCount = 0;
            item.Leave += (sender, e) => leaveCallCount++;
            int locationChangedCallCount = 0;
            item.LocationChanged += (sender, e) => locationChangedCallCount++;
            int lostFocusCallCount = 0;
            item.LostFocus += (sender, e) => lostFocusCallCount++;
            int mouseDownCallCount = 0;
            item.MouseDown += (sender, e) => mouseDownCallCount++;
            int mouseEnterCallCount = 0;
            item.MouseEnter += (sender, e) => mouseEnterCallCount++;
            int mouseHoverCallCount = 0;
            item.MouseHover += (sender, e) => mouseHoverCallCount++;
            int mouseLeaveCallCount = 0;
            item.MouseLeave += (sender, e) => mouseLeaveCallCount++;
            int mouseMoveCallCount = 0;
            item.MouseMove += (sender, e) => mouseMoveCallCount++;
            int mouseUpCallCount = 0;
            item.MouseUp += (sender, e) => mouseUpCallCount++;
            int paintCallCount = 0;
            item.Paint += (sender, e) => paintCallCount++;
            int queryAccessibilityHelpCallCount = 0;
            item.QueryAccessibilityHelp += (sender, e) => queryAccessibilityHelpCallCount++;
            int queryContinueDragCallCount = 0;
            item.QueryContinueDrag += (sender, e) => queryContinueDragCallCount++;
            int rightToLeftChangedCallCount = 0;
            item.RightToLeftChanged += (sender, e) => rightToLeftChangedCallCount++;
            int textChangedCallCount = 0;
            item.TextChanged += (sender, e) => textChangedCallCount++;
            int validatedCallCount = 0;
            item.Validated += (sender, e) => validatedCallCount++;
            int validatingCallCount = 0;
            item.Validating += (sender, e) => validatingCallCount++;

            c.OnBackColorChanged(EventArgs.Empty);
            Assert.Equal(0, backColorChangedCallCount);

            c.OnClick(EventArgs.Empty);
            Assert.Equal(0, clickCallCount);

            c.OnDoubleClick(EventArgs.Empty);
            Assert.Equal(0, doubleClickCallCount);

            c.OnEnabledChanged(EventArgs.Empty);
            Assert.Equal(0, enabledChangedCallCount);

            c.OnEnter(EventArgs.Empty);
            Assert.Equal(0, enterCallCount);

            c.OnForeColorChanged(EventArgs.Empty);
            Assert.Equal(0, foreColorChangedCallCount);

            c.OnGiveFeedback(new GiveFeedbackEventArgs(DragDropEffects.All, false));
            Assert.Equal(0, giveFeedbackCallCount);

            c.OnGotFocus(EventArgs.Empty);
            Assert.Equal(0, gotFocusCallCount);

            c.OnKeyDown(new KeyEventArgs(Keys.A));
            Assert.Equal(0, keyDownCallCount);

            c.OnKeyPress(new KeyPressEventArgs('a'));
            Assert.Equal(0, keyPressCallCount);

            c.OnKeyUp(new KeyEventArgs(Keys.A));
            Assert.Equal(0, keyUpCallCount);

            c.OnLeave(EventArgs.Empty);
            Assert.Equal(0, leaveCallCount);

            c.OnLocationChanged(EventArgs.Empty);
            Assert.Equal(0, locationChangedCallCount);

            c.OnLostFocus(EventArgs.Empty);
            Assert.Equal(0, lostFocusCallCount);

            c.OnMouseDown(new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
            Assert.Equal(0, mouseDownCallCount);

            c.OnMouseEnter(EventArgs.Empty);
            Assert.Equal(0, mouseEnterCallCount);

            c.OnMouseHover(EventArgs.Empty);
            Assert.Equal(0, mouseHoverCallCount);

            c.OnMouseLeave(EventArgs.Empty);
            Assert.Equal(0, mouseLeaveCallCount);

            c.OnMouseMove(new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
            Assert.Equal(0, mouseMoveCallCount);

            c.OnMouseUp(new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
            Assert.Equal(0, mouseUpCallCount);

            Assert.Equal(expectedResult, c.AccessibilityObject.GetHelpTopic(out string fileName));
            Assert.Null(fileName);
            Assert.Equal(0, queryAccessibilityHelpCallCount);

            c.OnQueryContinueDrag(new QueryContinueDragEventArgs(0, false, DragAction.Cancel));
            Assert.Equal(0, queryContinueDragCallCount);

            using var image = new Bitmap(10, 10);
            var graphics = Graphics.FromImage(image);
            c.OnPaint(new PaintEventArgs(graphics, new Rectangle(1, 2, 3, 4)));
            Assert.Equal(0, paintCallCount);

            c.OnResize(EventArgs.Empty);
            Assert.Equal(Size.Empty, c.Size);

            c.Size = new Size(10, 20);
            Assert.Equal(new Size(10, 20), c.Size);

            c.OnRightToLeftChanged(EventArgs.Empty);
            Assert.Equal(0, rightToLeftChangedCallCount);

            c.OnTextChanged(EventArgs.Empty);
            Assert.Equal(0, textChangedCallCount);

            c.OnValidated(EventArgs.Empty);
            Assert.Equal(0, validatedCallCount);

            c.OnValidating(new CancelEventArgs());
            Assert.Equal(0, validatingCallCount);
        }

        [WinFormsFact]
        public void ToolStripControlHost_OnUnsubscribeControlEvents_InvokeNullControl_Nop()
        {
            using var c = new Control();
            using var item = new SubToolStripControlHost(c);
            item.OnUnsubscribeControlEvents(null);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void ToolStripControlHost_OnValidated_Invoke_CallsValidated(EventArgs eventArgs)
        {
            using var c = new Control();
            using var item = new SubToolStripControlHost(c);
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(item, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            item.Validated += handler;
            item.OnValidated(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            item.Validated -= handler;
            item.OnValidated(eventArgs);
            Assert.Equal(1, callCount);
        }

        [WinFormsTheory]
        [MemberData(nameof(CancelEventArgs_TestData))]
        public void ToolStripControlHost_OnValidating_Invoke_CallsValidating(CancelEventArgs eventArgs)
        {
            using var c = new Control();
            using var item = new SubToolStripControlHost(c);
            int callCount = 0;
            CancelEventHandler handler = (sender, e) =>
            {
                Assert.Same(item, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            item.Validating += handler;
            item.OnValidating(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            item.Validating -= handler;
            item.OnValidating(eventArgs);
            Assert.Equal(1, callCount);
        }

        [WinFormsTheory]
        [InlineData(Keys.None)]
        [InlineData(Keys.A)]
        [InlineData(Keys.Enter)]
        [InlineData(Keys.Space)]
        [InlineData((Keys)(Keys.None - 1))]
        public void ToolStripControlHost_ProcessCmdKey_Invoke_ReturnsFalse(Keys keyData)
        {
            using var c = new Control();
            using var item = new SubToolStripControlHost(c);
            var m = new Message();
            Assert.False(item.ProcessCmdKey(ref m, keyData));
            Assert.False(c.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(Keys.A, true)]
        [InlineData(Keys.A, false)]
        public void ToolStripControlHost_ProcessCmdKey_InvokeWithCustomControl_ReturnsFalse(Keys keyData, bool result)
        {
            int callCount = 0;
            bool action(Message actualMsg, Keys actualKeyData)
            {
                callCount++;
                return result;
            }
            using var c = new CustomProcessControl
            {
                ProcessCmdKeyAction = action
            };
            using var item = new SubToolStripControlHost(c);
            var m = new Message
            {
                Msg = 1
            };
            Assert.False(item.ProcessCmdKey(ref m, keyData));
            Assert.Equal(0, callCount);
            Assert.False(c.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(Keys.None)]
        [InlineData(Keys.A)]
        [InlineData(Keys.Enter)]
        [InlineData(Keys.Space)]
        [InlineData((Keys)(Keys.None - 1))]
        public void ToolStripControlHost_ProcessCmdKey_InvokeDisposed_ReturnsFalse(Keys keyData)
        {
            using var c = new Control();
            using var item = new SubToolStripControlHost(c);
            item.Dispose();

            var m = new Message();
            Assert.False(item.ProcessCmdKey(ref m, keyData));
        }

        [WinFormsTheory]
        [InlineData(Keys.None)]
        [InlineData(Keys.A)]
        [InlineData(Keys.Enter)]
        [InlineData(Keys.Space)]
        [InlineData((Keys)(Keys.None - 1))]
        public void ToolStripControlHost_ProcessDialogKey_Invoke_ReturnsFalse(Keys keyData)
        {
            using var c = new Control();
            using var item = new SubToolStripControlHost(c);
            int clickCallCount = 0;
            item.Click += (sender, e) => clickCallCount++;

            Assert.False(item.ProcessDialogKey(keyData));
            Assert.Equal(0, clickCallCount);
            Assert.False(c.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(Keys.A, true)]
        [InlineData(Keys.A, false)]
        public void ToolStripControlHost_ProcessDialogKey_InvokeWithCustomControl_ReturnsFalse(Keys keyData, bool result)
        {
            int callCount = 0;
            bool action(Keys actualKeyData)
            {
                callCount++;
                return result;
            }
            using var c = new CustomProcessControl
            {
                ProcessDialogKeyAction = action
            };
            using var item = new SubToolStripControlHost(c);
            int clickCallCount = 0;
            item.Click += (sender, e) => clickCallCount++;

            Assert.False(item.ProcessDialogKey(keyData));
            Assert.Equal(0, clickCallCount);
            Assert.Equal(0, callCount);
            Assert.False(c.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(Keys.None)]
        [InlineData(Keys.A)]
        [InlineData(Keys.Enter)]
        [InlineData(Keys.Space)]
        [InlineData((Keys)(Keys.None - 1))]
        public void ToolStripControlHost_ProcessDialogKey_InvokeDisposed_ReturnsFalse(Keys keyData)
        {
            using var c = new Control();
            using var item = new SubToolStripControlHost(c);
            item.Dispose();
            int clickCallCount = 0;
            item.Click += (sender, e) => clickCallCount++;

            Assert.False(item.ProcessDialogKey(keyData));
            Assert.Equal(0, clickCallCount);
        }

        [WinFormsTheory]
        [InlineData('a')]
        [InlineData(char.MinValue)]
        public void ToolStripControlHost_ProcessMnemonic_Invoke_ReturnsFalse(char charCode)
        {
            using var c = new Control();
            using var item = new SubToolStripControlHost(c);
            int clickCallCount = 0;
            item.Click += (sender, e) => clickCallCount++;

            Assert.False(item.ProcessMnemonic(charCode));
            Assert.Equal(0, clickCallCount);
            Assert.False(c.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData('a', true)]
        [InlineData('a', false)]
        public void ToolStripControlHost_ProcessMnemonic_InvokeWithCustomControl_ReturnsExpected(char charCode, bool result)
        {
            int callCount = 0;
            bool action(char actualCharCode)
            {
                Assert.Equal(charCode, actualCharCode);
                callCount++;
                return result;
            }
            using var c = new CustomProcessControl
            {
                ProcessMnemonicAction = action
            };
            using var item = new SubToolStripControlHost(c);
            int clickCallCount = 0;
            item.Click += (sender, e) => clickCallCount++;

            Assert.Equal(result, item.ProcessMnemonic(charCode));
            Assert.Equal(0, clickCallCount);
            Assert.Equal(1, callCount);
            Assert.False(c.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ToolStripControlHost_ProcessMnemonic_InvokeDisposed_ThrowsNullReferenceException(bool enabled)
        {
            using var c = new Control();
            using var item = new SubToolStripControlHost(c)
            {
                Enabled = enabled
            };
            item.Dispose();

            int clickCallCount = 0;
            item.Click += (sender, e) => clickCallCount++;
            Assert.Throws<NullReferenceException>(() => item.ProcessMnemonic('a'));
            Assert.Equal(0, clickCallCount);
            Assert.False(item.Pressed);
        }

        private class CustomProcessControl : Control
        {
            public Func<Message, Keys, bool> ProcessCmdKeyAction { get; set; }

            protected override bool ProcessCmdKey(ref Message msg, Keys keyData) => ProcessCmdKeyAction(msg, keyData);

            public Func<Keys, bool> ProcessDialogKeyAction { get; set; }

            protected override bool ProcessDialogKey(Keys keyData) => ProcessDialogKeyAction(keyData);

            public Func<char, bool> ProcessMnemonicAction { get; set; }

            protected internal override bool ProcessMnemonic(char charCode) => ProcessMnemonicAction(charCode);
        }

        [WinFormsFact]
        public void ToolStripControlHost_ResetBackColor_Invoke_Success()
        {
            using var c = new Control();
            using var item = new SubToolStripControlHost(c);

            // Reset without value.
            item.ResetBackColor();
            Assert.Equal(Control.DefaultBackColor, item.BackColor);
            Assert.Equal(Control.DefaultBackColor, c.BackColor);

            // Reset with value.
            item.BackColor = Color.Black;
            item.ResetBackColor();
            Assert.Equal(Control.DefaultBackColor, item.BackColor);
            Assert.Equal(Control.DefaultBackColor, c.BackColor);

            // Reset again.
            item.ResetBackColor();
            Assert.Equal(Control.DefaultBackColor, item.BackColor);
            Assert.Equal(Control.DefaultBackColor, c.BackColor);
        }

        [WinFormsFact]
        public void ToolStripControlHost_ResetBackColor_InvokeDisposed_ThrowsNullReferenceException()
        {
            using var c = new Control();
            using var item = new SubToolStripControlHost(c);
            item.Dispose();

            Assert.Throws<NullReferenceException>(() => item.ResetBackColor());
        }

        [WinFormsFact]
        public void ToolStripControlHost_ResetForeColor_Invoke_Success()
        {
            using var c = new Control();
            using var item = new SubToolStripControlHost(c);

            // Reset without value.
            item.ResetForeColor();
            Assert.Equal(Control.DefaultForeColor, item.ForeColor);
            Assert.Equal(Control.DefaultForeColor, c.ForeColor);

            // Reset with value.
            item.ForeColor = Color.Black;
            item.ResetForeColor();
            Assert.Equal(Control.DefaultForeColor, item.ForeColor);
            Assert.Equal(Control.DefaultForeColor, c.ForeColor);

            // Reset again.
            item.ResetForeColor();
            Assert.Equal(Control.DefaultForeColor, item.ForeColor);
            Assert.Equal(Control.DefaultForeColor, c.ForeColor);
        }

        [WinFormsFact]
        public void ToolStripControlHost_ResetForeColor_InvokeDisposed_ThrowsNullReferenceException()
        {
            using var c = new Control();
            using var item = new SubToolStripControlHost(c);
            item.Dispose();

            Assert.Throws<NullReferenceException>(() => item.ResetForeColor());
        }

        [WinFormsTheory]
        [MemberData(nameof(SetVisibleCore_TestData))]
        public void ToolStripControlHost_SetVisibleCore_Invoke_GetReturnsExpected(bool enabled, Image image, bool value)
        {
            using var c = new Control();
            using var item = new SubToolStripControlHost(c)
            {
                Enabled = enabled,
                Image = image
            };

            item.SetVisibleCore(value);
            Assert.Equal(value, c.Visible);
            Assert.False(item.Visible);
            Assert.Equal(value, item.Available);
            Assert.False(item.Selected);
            Assert.False(c.IsHandleCreated);

            // Set same.
            item.SetVisibleCore(value);
            Assert.Equal(value, c.Visible);
            Assert.False(item.Visible);
            Assert.Equal(value, item.Available);
            Assert.False(item.Selected);
            Assert.False(c.IsHandleCreated);

            // Set different.
            item.Available = !value;
            Assert.Equal(!value, c.Visible);
            Assert.False(item.Visible);
            Assert.Equal(!value, item.Available);
            Assert.False(item.Selected);
            Assert.False(c.IsHandleCreated);
        }

        [WinFormsFact]
        public void ToolStripControlHost_SetVisibleCore_InvokeWithHandler_CallsAvailableChanged()
        {
            using var c = new Control();
            using var item = new SubToolStripControlHost(c);
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
        public void ToolStripControlHost_SetVisibleCore_InvokeWithHandler_CallsVisibleChanged()
        {
            using var c = new Control();
            using var item = new SubToolStripControlHost(c);
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

        private class SubControl : Control
        {
            public new void CreateControl() => base.CreateControl();

            public new void OnBackColorChanged(EventArgs e) => base.OnBackColorChanged(e);

            public new void OnClick(EventArgs e) => base.OnClick(e);

            public new void OnDragDrop(DragEventArgs e) => base.OnDragDrop(e);

            public new void OnDragEnter(DragEventArgs e) => base.OnDragEnter(e);

            public new void OnDragLeave(EventArgs e) => base.OnDragLeave(e);

            public new void OnDragOver(DragEventArgs e) => base.OnDragOver(e);

            public new void OnDoubleClick(EventArgs e) => base.OnDoubleClick(e);

            public new void OnEnabledChanged(EventArgs e) => base.OnEnabledChanged(e);

            public new void OnEnter(EventArgs e) => base.OnEnter(e);

            public new void OnFontChanged(EventArgs e) => base.OnFontChanged(e);

            public new void OnForeColorChanged(EventArgs e) => base.OnForeColorChanged(e);

            public new void OnGiveFeedback(GiveFeedbackEventArgs e) => base.OnGiveFeedback(e);

            public new void OnGotFocus(EventArgs e) => base.OnGotFocus(e);

            public new void OnKeyDown(KeyEventArgs e) => base.OnKeyDown(e);

            public new void OnKeyPress(KeyPressEventArgs e) => base.OnKeyPress(e);

            public new void OnKeyUp(KeyEventArgs e) => base.OnKeyUp(e);

            public new void OnLeave(EventArgs e) => base.OnLeave(e);

            public new void OnLocationChanged(EventArgs e) => base.OnLocationChanged(e);

            public new void OnLostFocus(EventArgs e) => base.OnLostFocus(e);

            public new void OnMouseDown(MouseEventArgs e) => base.OnMouseDown(e);

            public new void OnMouseEnter(EventArgs e) => base.OnMouseEnter(e);

            public new void OnMouseHover(EventArgs e) => base.OnMouseHover(e);

            public new void OnMouseLeave(EventArgs e) => base.OnMouseLeave(e);

            public new void OnMouseMove(MouseEventArgs e) => base.OnMouseMove(e);

            public new void OnMouseUp(MouseEventArgs e) => base.OnMouseUp(e);

            public new void OnPaint(PaintEventArgs e) => base.OnPaint(e);

            public new void OnQueryContinueDrag(QueryContinueDragEventArgs e) => base.OnQueryContinueDrag(e);

            public new void OnResize(EventArgs e) => base.OnResize(e);

            public new void OnRightToLeftChanged(EventArgs e) => base.OnRightToLeftChanged(e);

            public new void OnTextChanged(EventArgs e) => base.OnTextChanged(e);

            public new void OnValidated(EventArgs e) => base.OnValidated(e);

            public new void OnValidating(CancelEventArgs e) => base.OnValidating(e);

            public new void OnVisibleChanged(EventArgs e) => base.OnVisibleChanged(e);
        }

        private class SubToolStripControlHost : ToolStripControlHost
        {
            public SubToolStripControlHost(Control c) : base(c)
            {
            }

            public SubToolStripControlHost(Control c, string name) : base(c, name)
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

            public new void OnBoundsChanged() => base.OnBoundsChanged();

            public new void OnEnter(EventArgs e) => base.OnEnter(e);

            public new void OnGotFocus(EventArgs e) => base.OnGotFocus(e);

            public new void OnHostedControlResize(EventArgs e) => base.OnHostedControlResize(e);

            public new void OnKeyDown(KeyEventArgs e) => base.OnKeyDown(e);

            public new void OnKeyPress(KeyPressEventArgs e) => base.OnKeyPress(e);

            public new void OnKeyUp(KeyEventArgs e) => base.OnKeyUp(e);

            public new void OnLayout(LayoutEventArgs e) => base.OnLayout(e);

            public new void OnLeave(EventArgs e) => base.OnLeave(e);

            public new void OnLostFocus(EventArgs e) => base.OnLostFocus(e);

            public new void OnPaint(PaintEventArgs e) => base.OnPaint(e);

            public new void OnParentChanged(ToolStrip oldParent, ToolStrip newParent) => base.OnParentChanged(oldParent, newParent);

            public new void OnSubscribeControlEvents(Control control) => base.OnSubscribeControlEvents(control);

            public new void OnUnsubscribeControlEvents(Control control) => base.OnUnsubscribeControlEvents(control);

            public new void OnValidated(EventArgs e) => base.OnValidated(e);

            public new void OnValidating(CancelEventArgs e) => base.OnValidating(e);

            public new bool ProcessCmdKey(ref Message m, Keys keyData) => base.ProcessCmdKey(ref m, keyData);

            public new bool ProcessDialogKey(Keys keyData) => base.ProcessDialogKey(keyData);

            public new bool ProcessMnemonic(char charCode) => base.ProcessMnemonic(charCode);

            public new void SetVisibleCore(bool visible) => base.SetVisibleCore(visible);
        }
    }
}
