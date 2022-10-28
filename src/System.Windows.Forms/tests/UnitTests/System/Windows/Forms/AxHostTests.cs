﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms.TestUtilities;
using Moq;
using Windows.Win32.System.Com;
using Windows.Win32.System.Ole;
using Xunit;
using static Interop;
using Point = System.Drawing.Point;
using Size = System.Drawing.Size;

namespace System.Windows.Forms.Tests
{
    [Collection("Sequential")] // workaround for WebBrowser control corrupting memory when run on multiple UI threads (instantiated via GUID)
    public class AxHostTests : IClassFixture<ThreadExceptionFixture>
    {
        private const string EmptyClsidString = "00000000-0000-0000-0000-000000000000";
        private const string WebBrowserClsidString = "8856f961-340a-11d0-a96b-00c04fd705a2";

        [WinFormsTheory]
        [InlineData(EmptyClsidString)]
        public void AxHost_Ctor_String(string clsid)
        {
            using var control = new SubAxHost(clsid);
            Assert.Null(control.AccessibleDefaultActionDescription);
            Assert.Null(control.AccessibleDescription);
            Assert.Null(control.AccessibleName);
            Assert.Equal(AccessibleRole.Default, control.AccessibleRole);
            Assert.False(control.AllowDrop);
            Assert.Equal(AnchorStyles.Top | AnchorStyles.Left, control.Anchor);
            Assert.False(control.AutoSize);
            Assert.Equal(Control.DefaultBackColor, control.BackColor);
            Assert.Null(control.BackgroundImage);
            Assert.Equal(ImageLayout.Tile, control.BackgroundImageLayout);
            Assert.Null(control.BindingContext);
            Assert.Equal(23, control.Bottom);
            Assert.Equal(new Rectangle(0, 0, 75, 23), control.Bounds);
            Assert.True(control.CanEnableIme);
            Assert.False(control.CanFocus);
            Assert.True(control.CanRaiseEvents);
            Assert.False(control.CanSelect);
            Assert.False(control.Capture);
            Assert.True(control.CausesValidation);
            Assert.Equal(new Size(75, 23), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, 75, 23), control.ClientRectangle);
            Assert.Null(control.Container);
            Assert.Null(control.ContainingControl);
            Assert.False(control.ContainsFocus);
            Assert.Null(control.ContextMenuStrip);
            Assert.Empty(control.Controls);
            Assert.Same(control.Controls, control.Controls);
            Assert.False(control.Created);
            Assert.Same(Cursors.Default, control.Cursor);
            Assert.Same(Cursors.Default, control.DefaultCursor);
            Assert.Equal(ImeMode.Inherit, control.DefaultImeMode);
            Assert.Equal(new Padding(3), control.DefaultMargin);
            Assert.Equal(Size.Empty, control.DefaultMaximumSize);
            Assert.Equal(Size.Empty, control.DefaultMinimumSize);
            Assert.Equal(Padding.Empty, control.DefaultPadding);
            Assert.Equal(new Size(75, 23), control.DefaultSize);
            Assert.False(control.DesignMode);
            Assert.Equal(new Rectangle(0, 0, 75, 23), control.DisplayRectangle);
            Assert.Equal(DockStyle.None, control.Dock);
            Assert.False(control.DoubleBuffered);
            Assert.False(control.EditMode);
            Assert.True(control.Enabled);
            Assert.NotNull(control.Events);
            Assert.Same(control.Events, control.Events);
            Assert.False(control.Focused);
            Assert.Equal(Control.DefaultFont, control.Font);
            Assert.Equal(control.Font.Height, control.FontHeight);
            Assert.Equal(Control.DefaultForeColor, control.ForeColor);
            Assert.False(control.HasAboutBox);
            Assert.False(control.HasChildren);
            Assert.Equal(23, control.Height);
            Assert.Equal(ImeMode.NoControl, control.ImeMode);
            Assert.Equal(ImeMode.NoControl, control.ImeModeBase);
            Assert.False(control.IsAccessible);
            Assert.False(control.IsMirrored);
            Assert.NotNull(control.LayoutEngine);
            Assert.Same(control.LayoutEngine, control.LayoutEngine);
            Assert.Equal(0, control.Left);
            Assert.Equal(Point.Empty, control.Location);
            Assert.Equal(new Padding(3), control.Margin);
            Assert.Equal(Size.Empty, control.MaximumSize);
            Assert.Equal(Size.Empty, control.MinimumSize);
            Assert.Null(control.OcxState);
            Assert.Equal(Padding.Empty, control.Padding);
            Assert.Null(control.Parent);
            Assert.Equal(new Size(75, 23), control.PreferredSize);
            Assert.Equal("Microsoft\u00AE .NET", control.ProductName);
            Assert.False(control.RecreatingHandle);
            Assert.Null(control.Region);
            Assert.False(control.ResizeRedraw);
            Assert.Equal(75, control.Right);
            Assert.False(control.RightToLeft);
            Assert.Equal(RightToLeft.No, ((Control)control).RightToLeft);
            Assert.True(control.ShowFocusCues);
            Assert.True(control.ShowKeyboardCues);
            Assert.Null(control.Site);
            Assert.Equal(new Size(75, 23), control.Size);
            Assert.Equal(0, control.TabIndex);
            Assert.True(control.TabStop);
            Assert.Empty(control.Text);
            Assert.Equal(0, control.Top);
            Assert.Null(control.TopLevelControl);
            Assert.False(control.UseWaitCursor);
            Assert.True(control.Visible);
            Assert.Equal(75, control.Width);

            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(EmptyClsidString, 0)]
        public void AxHost_Ctor_String_Int(string clsid, int flags)
        {
            using var control = new SubAxHost(clsid, flags);
            Assert.Null(control.AccessibleDefaultActionDescription);
            Assert.Null(control.AccessibleDescription);
            Assert.Null(control.AccessibleName);
            Assert.Equal(AccessibleRole.Default, control.AccessibleRole);
            Assert.False(control.AllowDrop);
            Assert.Equal(AnchorStyles.Top | AnchorStyles.Left, control.Anchor);
            Assert.False(control.AutoSize);
            Assert.Equal(Control.DefaultBackColor, control.BackColor);
            Assert.Null(control.BackgroundImage);
            Assert.Equal(ImageLayout.Tile, control.BackgroundImageLayout);
            Assert.Null(control.BindingContext);
            Assert.Equal(23, control.Bottom);
            Assert.Equal(new Rectangle(0, 0, 75, 23), control.Bounds);
            Assert.True(control.CanEnableIme);
            Assert.False(control.CanFocus);
            Assert.True(control.CanRaiseEvents);
            Assert.False(control.CanSelect);
            Assert.False(control.Capture);
            Assert.True(control.CausesValidation);
            Assert.Equal(new Size(75, 23), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, 75, 23), control.ClientRectangle);
            Assert.Null(control.Container);
            Assert.Null(control.ContainingControl);
            Assert.False(control.ContainsFocus);
            Assert.Null(control.ContextMenuStrip);
            Assert.Empty(control.Controls);
            Assert.Same(control.Controls, control.Controls);
            Assert.False(control.Created);
            Assert.Same(Cursors.Default, control.Cursor);
            Assert.Same(Cursors.Default, control.DefaultCursor);
            Assert.Equal(ImeMode.Inherit, control.DefaultImeMode);
            Assert.Equal(new Padding(3), control.DefaultMargin);
            Assert.Equal(Size.Empty, control.DefaultMaximumSize);
            Assert.Equal(Size.Empty, control.DefaultMinimumSize);
            Assert.Equal(Padding.Empty, control.DefaultPadding);
            Assert.Equal(new Size(75, 23), control.DefaultSize);
            Assert.False(control.DesignMode);
            Assert.Equal(new Rectangle(0, 0, 75, 23), control.DisplayRectangle);
            Assert.Equal(DockStyle.None, control.Dock);
            Assert.False(control.DoubleBuffered);
            Assert.False(control.EditMode);
            Assert.True(control.Enabled);
            Assert.NotNull(control.Events);
            Assert.Same(control.Events, control.Events);
            Assert.False(control.Focused);
            Assert.Equal(Control.DefaultFont, control.Font);
            Assert.Equal(control.Font.Height, control.FontHeight);
            Assert.Equal(Control.DefaultForeColor, control.ForeColor);
            Assert.False(control.HasAboutBox);
            Assert.False(control.HasChildren);
            Assert.Equal(23, control.Height);
            Assert.Equal(ImeMode.NoControl, control.ImeMode);
            Assert.Equal(ImeMode.NoControl, control.ImeModeBase);
            Assert.False(control.IsAccessible);
            Assert.False(control.IsMirrored);
            Assert.NotNull(control.LayoutEngine);
            Assert.Same(control.LayoutEngine, control.LayoutEngine);
            Assert.Equal(0, control.Left);
            Assert.Equal(Point.Empty, control.Location);
            Assert.Equal(new Padding(3), control.Margin);
            Assert.Equal(Size.Empty, control.MaximumSize);
            Assert.Equal(Size.Empty, control.MinimumSize);
            Assert.Null(control.OcxState);
            Assert.Equal(Padding.Empty, control.Padding);
            Assert.Null(control.Parent);
            Assert.Equal(new Size(75, 23), control.PreferredSize);
            Assert.Equal("Microsoft\u00AE .NET", control.ProductName);
            Assert.False(control.RecreatingHandle);
            Assert.Null(control.Region);
            Assert.False(control.ResizeRedraw);
            Assert.Equal(75, control.Right);
            Assert.False(control.RightToLeft);
            Assert.Equal(RightToLeft.No, ((Control)control).RightToLeft);
            Assert.True(control.ShowFocusCues);
            Assert.True(control.ShowKeyboardCues);
            Assert.Null(control.Site);
            Assert.Equal(new Size(75, 23), control.Size);
            Assert.Equal(0, control.TabIndex);
            Assert.True(control.TabStop);
            Assert.Empty(control.Text);
            Assert.Equal(0, control.Top);
            Assert.Null(control.TopLevelControl);
            Assert.False(control.UseWaitCursor);
            Assert.True(control.Visible);
            Assert.Equal(75, control.Width);

            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void AxHost_Ctor_NullClsid_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>("g", () => new SubAxHost(null));
            Assert.Throws<ArgumentNullException>("g", () => new SubAxHost(null, 0));
        }

        [WinFormsTheory]
        [InlineData("")]
        [InlineData("clsid")]
        public void AxHost_Ctor_EmptyClsid_ThrowsFormatException(string clsid)
        {
            Assert.Throws<FormatException>(() => new SubAxHost(clsid));
        }

        [WinFormsFact]
        public void AxHost_CreateParams_GetDefault_ReturnsExpected()
        {
            using var control = new SubAxHost(EmptyClsidString);
            CreateParams createParams = control.CreateParams;
            Assert.Null(createParams.Caption);
            Assert.Null(createParams.ClassName);
            Assert.Equal(0x8, createParams.ClassStyle);
            Assert.Equal(0, createParams.ExStyle);
            Assert.Equal(23, createParams.Height);
            Assert.Equal(IntPtr.Zero, createParams.Parent);
            Assert.Null(createParams.Param);
            Assert.Equal(0x56010000, createParams.Style);
            Assert.Equal(75, createParams.Width);
            Assert.Equal(0, createParams.X);
            Assert.Equal(0, createParams.Y);
            Assert.Same(createParams, control.CreateParams);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void AxHost_CreateParams_GetWithSite_ReturnsExpected()
        {
            var mockSite = new Mock<ISite>(MockBehavior.Strict);
            mockSite
                .Setup(s => s.Container)
                .Returns((IContainer)null);
            mockSite
                .Setup(s => s.GetService(typeof(AmbientProperties)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IComponentChangeService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IDesignerHost)))
                .Returns(null);
            mockSite
                .Setup(s => s.DesignMode)
                .Returns(true);
            using var control = new SubAxHost(WebBrowserClsidString)
            {
                Site = mockSite.Object
            };

            CreateParams createParams = control.CreateParams;
            Assert.Null(createParams.Caption);
            Assert.Null(createParams.ClassName);
            Assert.Equal(0x8, createParams.ClassStyle);
            Assert.Equal(0, createParams.ExStyle);
            Assert.Equal(23, createParams.Height);
            Assert.Equal(IntPtr.Zero, createParams.Parent);
            Assert.Null(createParams.Param);
            Assert.Equal(0x56010000, createParams.Style);
            Assert.Equal(75, createParams.Width);
            Assert.Equal(0, createParams.X);
            Assert.Equal(0, createParams.Y);
            Assert.Same(createParams, control.CreateParams);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetBackColorTheoryData))]
        public void AxHost_BackColor_Set_GetReturnsExpected(Color value, Color expected)
        {
            using var control = new SubAxHost(EmptyClsidString)
            {
                BackColor = value
            };
            Assert.Equal(expected, control.BackColor);

            // Set same.
            control.BackColor = value;
            Assert.Equal(expected, control.BackColor);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetImageTheoryData))]
        public void AxHost_BackgroundImage_Set_GetReturnsExpected(Image value)
        {
            using var control = new SubAxHost(EmptyClsidString)
            {
                BackgroundImage = value
            };
            Assert.Same(value, control.BackgroundImage);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.BackgroundImage = value;
            Assert.Same(value, control.BackgroundImage);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(CommonTestHelper), nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(ImageLayout))]
        public void AxHost_BackgroundImageLayout_Set_GetReturnsExpected(ImageLayout value)
        {
            using var control = new SubAxHost(EmptyClsidString)
            {
                BackgroundImageLayout = value
            };
            Assert.Equal(value, control.BackgroundImageLayout);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.BackgroundImageLayout = value;
            Assert.Equal(value, control.BackgroundImageLayout);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(CommonTestHelper), nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(ImageLayout))]
        public void AxHost_BackgroundImageLayout_SetInvalid_ThrowsInvalidEnumArgumentException(ImageLayout value)
        {
            using var control = new SubAxHost(EmptyClsidString);
            Assert.Throws<InvalidEnumArgumentException>("value", () => control.BackgroundImageLayout = value);
        }

        [WinFormsFact]
        public static void AxHost_ContainingControl_GetWithContainerControlGrandparent_ReturnsExpected()
        {
            var grandparent = new ContainerControl();
            var parent = new Control
            {
                Parent = grandparent
            };
            using var control = new SubAxHost(EmptyClsidString)
            {
                Parent = parent
            };
            Assert.Same(grandparent, control.ContainingControl);

            // Call again to check caching.
            Assert.Same(control.ContainingControl, control.ContainingControl);

            // Remove from chain.
            parent.Parent = null;
            Assert.Same(grandparent, control.ContainingControl);
        }

        [WinFormsFact]
        public static void AxHost_ContainingControl_GetWithContainerControlParent_ReturnsExpected()
        {
            var parent = new ContainerControl();
            using var control = new SubAxHost(EmptyClsidString)
            {
                Parent = parent
            };
            Assert.Same(parent, control.ContainingControl);

            // Call again to check caching.
            Assert.Same(parent, control.ContainingControl);

            // Remove from chain.
            control.Parent = null;
            Assert.Same(parent, control.ContainingControl);
        }

        [WinFormsFact]
        public static void AxHost_ContainingControl_GetWithNonContainerControlParent_ReturnsExpected()
        {
            var parent = new Control();
            using var control = new SubAxHost(EmptyClsidString)
            {
                Parent = parent
            };
            Assert.Null(control.ContainingControl);

            // Call again to check caching.
            Assert.Null(control.ContainingControl);

            // Modify.
            var grandparent = new ContainerControl();
            parent.Parent = grandparent;
            Assert.Same(grandparent, control.ContainingControl);

            // Call again to check caching.
            Assert.Same(grandparent, control.ContainingControl);

            // Remove from chain.
            parent.Parent = null;
            Assert.Same(grandparent, control.ContainingControl);
        }

        public static IEnumerable<object[]> ContainingControl_Set_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new ContainerControl() };
        }

        [WinFormsTheory]
        [MemberData(nameof(ContainingControl_Set_TestData))]
        public void AxHost_ContainingControl_Set_GetReturnsExpected(ContainerControl value)
        {
            using var control = new SubAxHost(EmptyClsidString)
            {
                ContainingControl = value
            };
            Assert.Same(value, control.ContainingControl);

            // Set same.
            control.ContainingControl = value;
            Assert.Same(value, control.ContainingControl);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetCursorTheoryData))]
        public void AxHost_Cursor_Set_GetReturnsExpected(Cursor value)
        {
            using var control = new SubAxHost(EmptyClsidString)
            {
                Cursor = value
            };
            Assert.Same(value ?? Cursors.Default, control.Cursor);

            // Set same.
            control.Cursor = value;
            Assert.Same(value ?? Cursors.Default, control.Cursor);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetCursorTheoryData))]
        public void AxHost_Cursor_SetWithHandle_GetReturnsExpected(Cursor value)
        {
            using var control = new SubAxHost(WebBrowserClsidString);
            Assert.NotEqual(IntPtr.Zero, control.Handle);

            control.Cursor = value;
            Assert.Same(value ?? Cursors.Default, control.Cursor);

            // Set same.
            control.Cursor = value;
            Assert.Same(value ?? Cursors.Default, control.Cursor);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetCursorTheoryData))]
        public void AxHost_Cursor_SetWithChildren_GetReturnsExpected(Cursor value)
        {
            var child1 = new Control();
            var child2 = new Control();
            using var control = new SubAxHost(EmptyClsidString);
            control.Controls.Add(child1);
            control.Controls.Add(child2);

            control.Cursor = value;
            Assert.Same(value ?? Cursors.Default, control.Cursor);
            Assert.Same(value ?? Cursors.Default, child1.Cursor);
            Assert.Same(value ?? Cursors.Default, child2.Cursor);

            // Set same.
            control.Cursor = value;
            Assert.Same(value ?? Cursors.Default, control.Cursor);
            Assert.Same(value ?? Cursors.Default, child1.Cursor);
            Assert.Same(value ?? Cursors.Default, child2.Cursor);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetCursorTheoryData))]
        public void AxHost_Cursor_SetWithChildrenWithCursor_GetReturnsExpected(Cursor value)
        {
            var cursor1 = new Cursor((IntPtr)1);
            var cursor2 = new Cursor((IntPtr)1);
            var child1 = new Control
            {
                Cursor = cursor1
            };
            var child2 = new Control
            {
                Cursor = cursor2
            };
            using var control = new SubAxHost(EmptyClsidString);
            control.Controls.Add(child1);
            control.Controls.Add(child2);

            control.Cursor = value;
            Assert.Same(value ?? Cursors.Default, control.Cursor);
            Assert.Same(cursor1, child1.Cursor);
            Assert.Same(cursor2, child2.Cursor);

            // Set same.
            control.Cursor = value;
            Assert.Same(value ?? Cursors.Default, control.Cursor);
            Assert.Same(cursor1, child1.Cursor);
            Assert.Same(cursor2, child2.Cursor);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetFontTheoryData))]
        public void AxHost_Font_Set_GetReturnsExpected(Font value)
        {
            using var control = new SubAxHost(EmptyClsidString)
            {
                Font = value
            };
            Assert.Equal(value ?? Control.DefaultFont, control.Font);
            Assert.Equal(control.Font.Height, control.FontHeight);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.Font = value;
            Assert.Equal(value ?? Control.DefaultFont, control.Font);
            Assert.Equal(control.Font.Height, control.FontHeight);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetForeColorTheoryData))]
        public void AxHost_ForeColor_Set_GetReturnsExpected(Color value, Color expected)
        {
            using var control = new SubAxHost(EmptyClsidString)
            {
                ForeColor = value
            };
            Assert.Equal(expected, control.ForeColor);

            // Set same.
            control.ForeColor = value;
            Assert.Equal(expected, control.ForeColor);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(CommonTestHelper), nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(ImageLayout))]
        public void AxHost_ImeMode_Set_GetReturnsExpected(ImeMode value)
        {
            using var control = new SubAxHost(EmptyClsidString)
            {
                ImeMode = value
            };
            Assert.Equal(value, control.ImeMode);

            // Set same.
            control.ImeMode = value;
            Assert.Equal(value, control.ImeMode);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(CommonTestHelper), nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(ImeMode))]
        public void AxHost_ImeMode_SetInvalid_ThrowsInvalidEnumArgumentException(ImeMode value)
        {
            using var control = new SubAxHost(EmptyClsidString);
            Assert.Throws<InvalidEnumArgumentException>("value", () => control.ImeMode = value);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(CommonTestHelper), nameof(CommonTestHelper.GetBoolTheoryData))]
        public void AxHost_Enabled_Set_GetReturnsExpected(bool value)
        {
            using var control = new SubAxHost(EmptyClsidString)
            {
                Enabled = value
            };
            Assert.Equal(value, control.Enabled);

            // Set same.
            control.Enabled = value;
            Assert.Equal(value, control.Enabled);

            // Set different.
            control.Enabled = !value;
            Assert.Equal(!value, control.Enabled);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(CommonTestHelper), nameof(CommonTestHelper.GetBoolTheoryData))]
        public void AxHost_Site_SetDesignMode_CreatesOcx(bool otherDesignMode)
        {
            var mockSite1 = new Mock<ISite>(MockBehavior.Strict);
            mockSite1
                .Setup(s => s.Container)
                .Returns((IContainer)null);
            mockSite1
                .Setup(s => s.GetService(typeof(AmbientProperties)))
                .Returns(null);
            mockSite1
                .Setup(s => s.GetService(typeof(IDesignerHost)))
                .Returns(null);
            mockSite1
                .Setup(s => s.GetService(typeof(IComponentChangeService)))
                .Returns(null);
            mockSite1
                .Setup(s => s.DesignMode)
                .Returns(true);
            using var control = new SubAxHost(WebBrowserClsidString)
            {
                Site = mockSite1.Object
            };
            Assert.Same(mockSite1.Object, control.Site);
            Assert.False(control.IsHandleCreated);
            object ocx = control.GetOcx();
            Assert.NotNull(ocx);
            Assert.Same(ocx, control.GetOcx());

            // Set same.
            control.Site = mockSite1.Object;
            Assert.Same(mockSite1.Object, control.Site);
            Assert.False(control.IsHandleCreated);
            Assert.Same(ocx, control.GetOcx());

            // Set another.
            var mockSite2 = new Mock<ISite>(MockBehavior.Strict);
            mockSite2
                .Setup(s => s.Container)
                .Returns((IContainer)null);
            mockSite2
                .Setup(s => s.GetService(typeof(AmbientProperties)))
                .Returns(null);
            mockSite2
                .Setup(s => s.GetService(typeof(IDesignerHost)))
                .Returns(null);
            mockSite2
                .Setup(s => s.GetService(typeof(IComponentChangeService)))
                .Returns(null);
            mockSite2
                .Setup(s => s.DesignMode)
                .Returns(otherDesignMode);
            control.Site = mockSite2.Object;
            Assert.Same(mockSite2.Object, control.Site);
            Assert.False(control.IsHandleCreated);
            Assert.Same(ocx, control.GetOcx());

            // Set null.
            control.Site = null;
            Assert.Null(control.Site);
            Assert.False(control.IsHandleCreated);
            Assert.Same(ocx, control.GetOcx());
        }

        [WinFormsFact]
        public void AxHost_Site_SetNonDesignModeThenDesignMode_CreatesOcx()
        {
            var mockSite1 = new Mock<ISite>(MockBehavior.Strict);
            mockSite1
                .Setup(s => s.Container)
                .Returns((IContainer)null);
            mockSite1
                .Setup(s => s.GetService(typeof(AmbientProperties)))
                .Returns(null);
            mockSite1
                .Setup(s => s.GetService(typeof(IDesignerHost)))
                .Returns(null);
            mockSite1
                .Setup(s => s.GetService(typeof(IComponentChangeService)))
                .Returns(null);
            mockSite1
                .Setup(s => s.DesignMode)
                .Returns(false);
            using var control = new SubAxHost(WebBrowserClsidString)
            {
                Site = mockSite1.Object
            };
            Assert.Same(mockSite1.Object, control.Site);
            Assert.False(control.IsHandleCreated);
            Assert.Null(control.GetOcx());

            // Set same.
            control.Site = mockSite1.Object;
            Assert.Same(mockSite1.Object, control.Site);
            Assert.False(control.IsHandleCreated);
            Assert.Null(control.GetOcx());

            // Set another.
            var mockSite2 = new Mock<ISite>(MockBehavior.Strict);
            mockSite2
                .Setup(s => s.Container)
                .Returns((IContainer)null);
            mockSite2
                .Setup(s => s.GetService(typeof(AmbientProperties)))
                .Returns(null);
            mockSite2
                .Setup(s => s.GetService(typeof(IDesignerHost)))
                .Returns(null);
            mockSite2
                .Setup(s => s.GetService(typeof(IComponentChangeService)))
                .Returns(null);
            mockSite2
                .Setup(s => s.DesignMode)
                .Returns(true);
            control.Site = mockSite2.Object;
            Assert.Same(mockSite2.Object, control.Site);
            Assert.False(control.IsHandleCreated);
            object ocx = control.GetOcx();
            Assert.NotNull(ocx);
            Assert.Same(ocx, control.GetOcx());

            // Set null.
            control.Site = null;
            Assert.Null(control.Site);
            Assert.False(control.IsHandleCreated);
            Assert.Same(ocx, control.GetOcx());
        }

        [WinFormsFact]
        public void AxHost_Site_SetNonDesignModeThenNonDesignMode_DoesNotCreateOcx()
        {
            var mockSite1 = new Mock<ISite>(MockBehavior.Strict);
            mockSite1
                .Setup(s => s.Container)
                .Returns((IContainer)null);
            mockSite1
                .Setup(s => s.GetService(typeof(AmbientProperties)))
                .Returns(null);
            mockSite1
                .Setup(s => s.GetService(typeof(IDesignerHost)))
                .Returns(null);
            mockSite1
                .Setup(s => s.GetService(typeof(IComponentChangeService)))
                .Returns(null);
            mockSite1
                .Setup(s => s.DesignMode)
                .Returns(false);
            using var control = new SubAxHost(WebBrowserClsidString)
            {
                Site = mockSite1.Object
            };
            Assert.Same(mockSite1.Object, control.Site);
            Assert.False(control.IsHandleCreated);
            Assert.Null(control.GetOcx());

            // Set same.
            control.Site = mockSite1.Object;
            Assert.Same(mockSite1.Object, control.Site);
            Assert.False(control.IsHandleCreated);
            Assert.Null(control.GetOcx());

            // Set another.
            var mockSite2 = new Mock<ISite>(MockBehavior.Strict);
            mockSite2
                .Setup(s => s.Container)
                .Returns((IContainer)null);
            mockSite2
                .Setup(s => s.GetService(typeof(AmbientProperties)))
                .Returns(null);
            mockSite2
                .Setup(s => s.GetService(typeof(IDesignerHost)))
                .Returns(null);
            mockSite2
                .Setup(s => s.GetService(typeof(IComponentChangeService)))
                .Returns(null);
            mockSite2
                .Setup(s => s.DesignMode)
                .Returns(false);
            control.Site = mockSite2.Object;
            Assert.Same(mockSite2.Object, control.Site);
            Assert.False(control.IsHandleCreated);
            Assert.Null(control.GetOcx());

            // Set null.
            control.Site = null;
            Assert.Null(control.Site);
            Assert.False(control.IsHandleCreated);
            Assert.Null(control.GetOcx());
        }

        [WinFormsTheory]
        [InlineData(true, true, 1, 1)]
        [InlineData(true, false, 1, 1)]
        [InlineData(false, true, 0, 1)]
        [InlineData(false, false, 0, 0)]
        public void AxHost_Site_SetWithHandle_CreatesOcx(bool designMode1, bool designMode2, int expectedCreatedCallCount1, int expectedCreatedCallCount2)
        {
            var mockSite1 = new Mock<ISite>(MockBehavior.Strict);
            mockSite1
                .Setup(s => s.Container)
                .Returns((IContainer)null);
            mockSite1
                .Setup(s => s.GetService(typeof(AmbientProperties)))
                .Returns(null);
            mockSite1
                .Setup(s => s.GetService(typeof(IDesignerHost)))
                .Returns(null);
            mockSite1
                .Setup(s => s.GetService(typeof(IComponentChangeService)))
                .Returns(null);
            mockSite1
                .Setup(s => s.DesignMode)
                .Returns(designMode1);
            mockSite1
                .Setup(s => s.Name)
                .Returns("Name");
            using var control = new SubAxHost(WebBrowserClsidString);
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            ((Control)control).StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;
            object ocx = control.GetOcx();
            Assert.NotNull(ocx);

            control.Site = mockSite1.Object;
            Assert.Same(mockSite1.Object, control.Site);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(expectedCreatedCallCount1, createdCallCount);
            Assert.Same(ocx, control.GetOcx());

            // Set same.
            control.Site = mockSite1.Object;
            Assert.Same(mockSite1.Object, control.Site);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(expectedCreatedCallCount1, createdCallCount);
            Assert.Same(ocx, control.GetOcx());

            // Set another.
            var mockSite2 = new Mock<ISite>(MockBehavior.Strict);
            mockSite2
                .Setup(s => s.Container)
                .Returns((IContainer)null);
            mockSite2
                .Setup(s => s.GetService(typeof(AmbientProperties)))
                .Returns(null);
            mockSite2
                .Setup(s => s.GetService(typeof(IDesignerHost)))
                .Returns(null);
            mockSite2
                .Setup(s => s.GetService(typeof(IComponentChangeService)))
                .Returns(null);
            mockSite2
                .Setup(s => s.DesignMode)
                .Returns(designMode2);
            mockSite2
                .Setup(s => s.Name)
                .Returns("Name");
            control.Site = mockSite2.Object;
            Assert.Same(mockSite2.Object, control.Site);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(expectedCreatedCallCount2, createdCallCount);
            Assert.Same(ocx, control.GetOcx());

            // Set null.
            control.Site = null;
            Assert.Null(control.Site);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(expectedCreatedCallCount2, createdCallCount);
            Assert.Same(ocx, control.GetOcx());
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(CommonTestHelper), nameof(CommonTestHelper.GetStringWithNullTheoryData))]
        public void AxHost_Text_Set_GetReturnsExpected(string value)
        {
            using var control = new SubAxHost(EmptyClsidString)
            {
                Text = value
            };
            Assert.Equal(value, control.Text);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.Text = value;
            Assert.Equal(value, control.Text);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(CommonTestHelper), nameof(CommonTestHelper.GetStringWithNullTheoryData))]
        public void AxHost_Text_SetWithHandle_GetReturnsExpected(string value)
        {
            using var control = new SubAxHost(WebBrowserClsidString);
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            ((Control)control).StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.Text = value;
            Assert.Equal(value, control.Text);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.Text = value;
            Assert.Equal(value, control.Text);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void AxHost_BackColorChanged_AddRemove_ThrowsNotSupportedException()
        {
            using var control = new SubAxHost(EmptyClsidString);
            EventHandler handler = (sender, e) => { };
            Assert.Throws<NotSupportedException>(() => control.BackColorChanged += handler);
            control.BackColorChanged -= handler;
        }

        [WinFormsFact]
        public void AxHost_BackgroundImageChanged_AddRemove_ThrowsNotSupportedException()
        {
            using var control = new SubAxHost(EmptyClsidString);
            EventHandler handler = (sender, e) => { };
            Assert.Throws<NotSupportedException>(() => control.BackgroundImageChanged += handler);
            control.BackgroundImageChanged -= handler;
        }

        [WinFormsFact]
        public void AxHost_BackgroundImageLayoutChanged_AddRemove_ThrowsNotSupportedException()
        {
            using var control = new SubAxHost(EmptyClsidString);
            EventHandler handler = (sender, e) => { };
            Assert.Throws<NotSupportedException>(() => control.BackgroundImageLayoutChanged += handler);
            control.BackgroundImageLayoutChanged -= handler;
        }

        [WinFormsFact]
        public void AxHost_BindingContextChanged_AddRemove_ThrowsNotSupportedException()
        {
            using var control = new SubAxHost(EmptyClsidString);
            EventHandler handler = (sender, e) => { };
            Assert.Throws<NotSupportedException>(() => control.BindingContextChanged += handler);
            control.BindingContextChanged -= handler;
        }

        [WinFormsFact]
        public void AxHost_ChangeUICues_AddRemove_ThrowsNotSupportedException()
        {
            using var control = new SubAxHost(EmptyClsidString);
            UICuesEventHandler handler = (sender, e) => { };
            Assert.Throws<NotSupportedException>(() => control.ChangeUICues += handler);
            control.ChangeUICues -= handler;
        }

        [WinFormsFact]
        public void AxHost_Click_AddRemove_ThrowsNotSupportedException()
        {
            using var control = new SubAxHost(EmptyClsidString);
            EventHandler handler = (sender, e) => { };
            Assert.Throws<NotSupportedException>(() => control.Click += handler);
            control.Click -= handler;
        }

        [WinFormsFact]
        public void AxHost_CursorChanged_AddRemove_ThrowsNotSupportedException()
        {
            using var control = new SubAxHost(EmptyClsidString);
            EventHandler handler = (sender, e) => { };
            Assert.Throws<NotSupportedException>(() => control.CursorChanged += handler);
            control.CursorChanged -= handler;
        }

        [WinFormsFact]
        public void AxHost_DoubleClick_AddRemove_ThrowsNotSupportedException()
        {
            using var control = new SubAxHost(EmptyClsidString);
            EventHandler handler = (sender, e) => { };
            Assert.Throws<NotSupportedException>(() => control.DoubleClick += handler);
            control.DoubleClick -= handler;
        }

        [WinFormsFact]
        public void AxHost_DragDrop_AddRemove_ThrowsNotSupportedException()
        {
            using var control = new SubAxHost(EmptyClsidString);
            DragEventHandler handler = (sender, e) => { };
            Assert.Throws<NotSupportedException>(() => control.DragDrop += handler);
            control.DragDrop -= handler;
        }

        [WinFormsFact]
        public void AxHost_DragEnter_AddRemove_ThrowsNotSupportedException()
        {
            using var control = new SubAxHost(EmptyClsidString);
            DragEventHandler handler = (sender, e) => { };
            Assert.Throws<NotSupportedException>(() => control.DragEnter += handler);
            control.DragEnter -= handler;
        }

        [WinFormsFact]
        public void AxHost_DragLeave_AddRemove_ThrowsNotSupportedException()
        {
            using var control = new SubAxHost(EmptyClsidString);
            EventHandler handler = (sender, e) => { };
            Assert.Throws<NotSupportedException>(() => control.DragLeave += handler);
            control.DragLeave -= handler;
        }

        [WinFormsFact]
        public void AxHost_DragOver_AddRemove_ThrowsNotSupportedException()
        {
            using var control = new SubAxHost(EmptyClsidString);
            DragEventHandler handler = (sender, e) => { };
            Assert.Throws<NotSupportedException>(() => control.DragOver += handler);
            control.DragOver -= handler;
        }

        [WinFormsFact]
        public void AxHost_EnabledChanged_AddRemove_ThrowsNotSupportedException()
        {
            using var control = new SubAxHost(EmptyClsidString);
            EventHandler handler = (sender, e) => { };
            Assert.Throws<NotSupportedException>(() => control.EnabledChanged += handler);
            control.EnabledChanged -= handler;
        }

        [WinFormsFact]
        public void AxHost_FontChanged_AddRemove_ThrowsNotSupportedException()
        {
            using var control = new SubAxHost(EmptyClsidString);
            EventHandler handler = (sender, e) => { };
            Assert.Throws<NotSupportedException>(() => control.FontChanged += handler);
            control.FontChanged -= handler;
        }

        [WinFormsFact]
        public void AxHost_ForeColorChanged_AddRemove_ThrowsNotSupportedException()
        {
            using var control = new SubAxHost(EmptyClsidString);
            EventHandler handler = (sender, e) => { };
            Assert.Throws<NotSupportedException>(() => control.ForeColorChanged += handler);
            control.ForeColorChanged -= handler;
        }

        [WinFormsFact]
        public void AxHost_GiveFeedback_AddRemove_ThrowsNotSupportedException()
        {
            using var control = new SubAxHost(EmptyClsidString);
            GiveFeedbackEventHandler handler = (sender, e) => { };
            Assert.Throws<NotSupportedException>(() => control.GiveFeedback += handler);
            control.GiveFeedback -= handler;
        }

        [WinFormsFact]
        public void AxHost_HelpRequested_AddRemove_ThrowsNotSupportedException()
        {
            using var control = new SubAxHost(EmptyClsidString);
            HelpEventHandler handler = (sender, e) => { };
            Assert.Throws<NotSupportedException>(() => control.HelpRequested += handler);
            control.HelpRequested -= handler;
        }

        [WinFormsFact]
        public void AxHost_ImeModeChanged_AddRemove_ThrowsNotSupportedException()
        {
            using var control = new SubAxHost(EmptyClsidString);
            EventHandler handler = (sender, e) => { };
            Assert.Throws<NotSupportedException>(() => control.ImeModeChanged += handler);
            control.ImeModeChanged -= handler;
        }

        [WinFormsFact]
        public void AxHost_KeyDown_AddRemove_ThrowsNotSupportedException()
        {
            using var control = new SubAxHost(EmptyClsidString);
            KeyEventHandler handler = (sender, e) => { };
            Assert.Throws<NotSupportedException>(() => control.KeyDown += handler);
            control.KeyDown -= handler;
        }

        [WinFormsFact]
        public void AxHost_KeyPress_AddRemove_ThrowsNotSupportedException()
        {
            using var control = new SubAxHost(EmptyClsidString);
            KeyPressEventHandler handler = (sender, e) => { };
            Assert.Throws<NotSupportedException>(() => control.KeyPress += handler);
            control.KeyPress -= handler;
        }

        [WinFormsFact]
        public void AxHost_KeyUp_AddRemove_ThrowsNotSupportedException()
        {
            using var control = new SubAxHost(EmptyClsidString);
            KeyEventHandler handler = (sender, e) => { };
            Assert.Throws<NotSupportedException>(() => control.KeyUp += handler);
            control.KeyUp -= handler;
        }

        [WinFormsFact]
        public void AxHost_Layout_AddRemove_ThrowsNotSupportedException()
        {
            using var control = new SubAxHost(EmptyClsidString);
            LayoutEventHandler handler = (sender, e) => { };
            Assert.Throws<NotSupportedException>(() => control.Layout += handler);
            control.Layout -= handler;
        }

        [WinFormsFact]
        public void AxHost_MouseClick_AddRemove_ThrowsNotSupportedException()
        {
            using var control = new SubAxHost(EmptyClsidString);
            EventHandler handler = (sender, e) => { };
            Assert.Throws<NotSupportedException>(() => control.MouseClick += handler);
            control.MouseClick -= handler;
        }

        [WinFormsFact]
        public void AxHost_MouseDoubleClick_AddRemove_ThrowsNotSupportedException()
        {
            using var control = new SubAxHost(EmptyClsidString);
            EventHandler handler = (sender, e) => { };
            Assert.Throws<NotSupportedException>(() => control.MouseDoubleClick += handler);
            control.MouseDoubleClick -= handler;
        }

        [WinFormsFact]
        public void AxHost_MouseDown_AddRemove_ThrowsNotSupportedException()
        {
            using var control = new SubAxHost(EmptyClsidString);
            MouseEventHandler handler = (sender, e) => { };
            Assert.Throws<NotSupportedException>(() => control.MouseDown += handler);
            control.MouseDown -= handler;
        }

        [WinFormsFact]
        public void AxHost_MouseEnter_AddRemove_ThrowsNotSupportedException()
        {
            using var control = new SubAxHost(EmptyClsidString);
            EventHandler handler = (sender, e) => { };
            Assert.Throws<NotSupportedException>(() => control.MouseEnter += handler);
            control.MouseEnter -= handler;
        }

        [WinFormsFact]
        public void AxHost_MouseHover_AddRemove_ThrowsNotSupportedException()
        {
            using var control = new SubAxHost(EmptyClsidString);
            EventHandler handler = (sender, e) => { };
            Assert.Throws<NotSupportedException>(() => control.MouseHover += handler);
            control.MouseHover -= handler;
        }

        [WinFormsFact]
        public void AxHost_MouseLeave_AddRemove_ThrowsNotSupportedException()
        {
            using var control = new SubAxHost(EmptyClsidString);
            EventHandler handler = (sender, e) => { };
            Assert.Throws<NotSupportedException>(() => control.MouseLeave += handler);
            control.MouseLeave -= handler;
        }

        [WinFormsFact]
        public void AxHost_MouseMove_AddRemove_ThrowsNotSupportedException()
        {
            using var control = new SubAxHost(EmptyClsidString);
            MouseEventHandler handler = (sender, e) => { };
            Assert.Throws<NotSupportedException>(() => control.MouseMove += handler);
            control.MouseMove -= handler;
        }

        [WinFormsFact]
        public void AxHost_MouseUp_AddRemove_ThrowsNotSupportedException()
        {
            using var control = new SubAxHost(EmptyClsidString);
            MouseEventHandler handler = (sender, e) => { };
            Assert.Throws<NotSupportedException>(() => control.MouseUp += handler);
            control.MouseUp -= handler;
        }

        [WinFormsFact]
        public void AxHost_MouseWheel_AddRemove_ThrowsNotSupportedException()
        {
            using var control = new SubAxHost(EmptyClsidString);
            MouseEventHandler handler = (sender, e) => { };
            Assert.Throws<NotSupportedException>(() => control.MouseWheel += handler);
            control.MouseWheel -= handler;
        }

        [WinFormsFact]
        public void AxHost_Paint_AddRemove_ThrowsNotSupportedException()
        {
            using var control = new SubAxHost(EmptyClsidString);
            PaintEventHandler handler = (sender, e) => { };
            Assert.Throws<NotSupportedException>(() => control.Paint += handler);
            control.Paint -= handler;
        }

        [WinFormsFact]
        public void AxHost_QueryAccessibilityHelp__AddRemove_ThrowsNotSupportedException()
        {
            using var control = new SubAxHost(EmptyClsidString);
            QueryAccessibilityHelpEventHandler handler = (sender, e) => { };
            Assert.Throws<NotSupportedException>(() => control.QueryAccessibilityHelp += handler);
            control.QueryAccessibilityHelp -= handler;
        }

        [WinFormsFact]
        public void AxHost_QueryContinueDrag__AddRemove_ThrowsNotSupportedException()
        {
            using var control = new SubAxHost(EmptyClsidString);
            QueryContinueDragEventHandler handler = (sender, e) => { };
            Assert.Throws<NotSupportedException>(() => control.QueryContinueDrag += handler);
            control.QueryContinueDrag -= handler;
        }

        [WinFormsFact]
        public void AxHost_RightToLeftChanged_AddRemove_ThrowsNotSupportedException()
        {
            using var control = new SubAxHost(EmptyClsidString);
            EventHandler handler = (sender, e) => { };
            Assert.Throws<NotSupportedException>(() => control.RightToLeftChanged += handler);
            control.RightToLeftChanged -= handler;
        }

        [WinFormsFact]
        public void AxHost_StyleChanged_AddRemove_ThrowsNotSupportedException()
        {
            using var control = new SubAxHost(EmptyClsidString);
            EventHandler handler = (sender, e) => { };
            Assert.Throws<NotSupportedException>(() => control.StyleChanged += handler);
            control.StyleChanged -= handler;
        }

        [WinFormsFact]
        public void AxHost_TextChanged_AddRemove_ThrowsNotSupportedException()
        {
            using var control = new SubAxHost(EmptyClsidString);
            EventHandler handler = (sender, e) => { };
            Assert.Throws<NotSupportedException>(() => control.TextChanged += handler);
            control.TextChanged -= handler;
        }

        [WinFormsFact]
        public void AxHost_AttachInterfaces_Invoke_Nop()
        {
            using var control = new SubAxHost(EmptyClsidString);
            control.AttachInterfaces();
            control.AttachInterfaces();
        }

        [WinFormsFact]
        public void AxHost_BeginInit_Invoke_Nop()
        {
            using var control = new SubAxHost(EmptyClsidString);
            control.BeginInit();
            control.BeginInit();
            Assert.False(control.Created);
        }

        public static IEnumerable<object[]> DoVerb_TestData()
        {
            yield return new object[] { 1 };
            yield return new object[] { 0 };
            yield return new object[] { -1 };
            yield return new object[] { -2 };
            yield return new object[] { -3 };
            yield return new object[] { -4 };
            yield return new object[] { -5 };
            yield return new object[] { -6 };
            yield return new object[] { -7 };
        }

        [WinFormsTheory]
        [MemberData(nameof(DoVerb_TestData))]
        public void AxHost_DoVerb_InvokeWithHandle_Success(int verb)
        {
            using var control = new SubAxHost(WebBrowserClsidString);
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            control.DoVerb(verb);
        }

        [WinFormsTheory]
        [MemberData(nameof(DoVerb_TestData))]
        public void AxHost_DoVerb_InvokeWithHandleWithParent_Success(int verb)
        {
            using var parent = new Control();
            using var control = new SubAxHost(WebBrowserClsidString)
            {
                Parent = parent
            };
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            Assert.True(parent.IsHandleCreated);
            control.DoVerb(verb);
            Assert.True(parent.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(DoVerb_TestData))]
        public void AxHost_DoVerb_InvokeWithHandleWithParentWithoutHandle_Success(int verb)
        {
            using var parent = new Control();
            using var control = new SubAxHost(WebBrowserClsidString);
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            control.Parent = parent;
            Assert.False(parent.IsHandleCreated);
            control.DoVerb(verb);
            Assert.True(parent.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(DoVerb_TestData))]
        public void AxHost_DoVerb_InvokeWithoutHandle_ThrowsNullReferenceException(int verb)
        {
            using var control = new SubAxHost(EmptyClsidString);
            Assert.Throws<NullReferenceException>(() => control.DoVerb(verb));
        }

        [WinFormsFact]
        public void AxHost_EndInit_Invoke_Nop()
        {
            using var control = new SubAxHost(EmptyClsidString);
            control.EndInit();
            control.EndInit();
            Assert.False(control.Created);
        }

        [WinFormsFact]
        public void AxHost_EndInit_InvokeWithParent_CreatesControl()
        {
            var parent = new Control();
            using var control = new SubAxHost(WebBrowserClsidString)
            {
                Parent = parent
            };
            control.EndInit();
            control.EndInit();
            Assert.True(parent.Created);
            Assert.True(control.Created);
        }

        [ComImport]
        [Guid("D30C1661-CDAF-11d0-8A3E-00C04FC9E26E")]
        public interface IWebBrowser2
        {
        }

        [WinFormsFact]
        public void AxHost_CreateControl_ValidClsid_Success()
        {
            using var control = new SubAxHost(WebBrowserClsidString);
            control.CreateControl();
            Assert.True(control.Created);
            Assert.NotEqual(IntPtr.Zero, control.Handle);

            object ocx = control.GetOcx();
            Assert.True(ocx is IWebBrowser2);
        }

        [WinFormsFact]
        public void AxHost_CreateControl_InvalidClsid_ThrowsCOMException()
        {
            using var control = new SubAxHost(EmptyClsidString);
            Assert.Throws<COMException>(() => control.CreateControl());
        }

        [WinFormsFact]
        public void AxHost_CreateSink_Invoke_Nop()
        {
            using var control = new SubAxHost(EmptyClsidString);
            control.CreateSink();
            control.CreateSink();
            Assert.False(control.Created);
        }

        [WinFormsFact]
        public void AxHost_DetachSink_Invoke_Nop()
        {
            using var control = new SubAxHost(EmptyClsidString);
            control.DetachSink();
            control.DetachSink();
            Assert.False(control.Created);
        }

        [WinFormsFact]
        public void AxHost_GetOcx_NotCreated_ReturnsNull()
        {
            using var control = new SubAxHost(EmptyClsidString);
            Assert.Null(control.GetOcx());
        }

        [WinFormsFact]
        public void AxHost_GetOcx_InvokeWithHandle_ReturnsExpected()
        {
            using var control = new CustomAxHost(WebBrowserClsidString);
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            ((Control)control).StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            object ocx = control.GetOcx();
            Assert.NotNull(ocx);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void AxHost_GetFontFromIFont_NullFont_ReturnsNull()
        {
            Assert.Null(SubAxHost.GetFontFromIFont(null));
        }

        [WinFormsFact]
        public void AxHost_GetFontFromIFont_InvalidFont_ThrowsInvalidCastException()
        {
            Assert.Throws<InvalidCastException>(() => SubAxHost.GetFontFromIFont(new object()));
        }

        [WinFormsFact]
        public void AxHost_GetIFontDispFromFont_InvokeSimpleStyle_Roundtrips()
        {
            using var font = new Font("Arial", 10);
            object disp = SubAxHost.GetIFontDispFromFont(font);
            Ole32.IFont iFont = (Ole32.IFont)disp;
            Assert.Equal(font.Name, iFont.Name);
            Assert.Equal(97500, iFont.Size);
            Assert.False(iFont.Bold);
            Assert.False(iFont.Italic);
            Assert.False(iFont.Underline);
            Assert.False(iFont.Strikethrough);
            // Charset is locale specific
            // Assert.Equal(0, iFont.Charset);
            Assert.NotEqual(IntPtr.Zero, iFont.hFont);

            Ole32.IFontDisp iFontDisp = (Ole32.IFontDisp)disp;
            Assert.Equal(font.Name, iFontDisp.Name);
            Assert.Equal(10, iFontDisp.Size);
            Assert.False(iFontDisp.Bold);
            Assert.False(iFontDisp.Italic);
            Assert.False(iFontDisp.Underline);
            Assert.False(iFontDisp.Strikethrough);
            // Charset is locale specific
            // Assert.Equal(0, iFontDisp.Charset);

            Font result = SubAxHost.GetFontFromIFont(iFont);
            Assert.Equal(font.Name, result.Name);
            Assert.Equal(9.75, result.Size);
            Assert.Equal(font.Style, result.Style);
            Assert.Equal(1, result.GdiCharSet);

            result = SubAxHost.GetFontFromIFont(iFontDisp);
            Assert.Equal(font.Name, result.Name);
            Assert.Equal(9.75, result.Size);
            Assert.Equal(font.Style, result.Style);
            Assert.Equal(1, result.GdiCharSet);
        }

        [WinFormsFact]
        public void AxHost_GetIFontDispFromFont_InvokeComplexStyle_Roundtrips()
        {
            using var font = new Font("Arial", 10, FontStyle.Bold | FontStyle.Underline | FontStyle.Italic | FontStyle.Strikeout, GraphicsUnit.Point, 10);
            object disp = SubAxHost.GetIFontDispFromFont(font);

            Ole32.IFont iFont = (Ole32.IFont)disp;
            Assert.Equal(font.Name, iFont.Name);
            Assert.Equal(97500, iFont.Size);
            Assert.True(iFont.Bold);
            Assert.True(iFont.Italic);
            Assert.True(iFont.Underline);
            Assert.True(iFont.Strikethrough);
            Assert.Equal(0, iFont.Charset);
            Assert.NotEqual(IntPtr.Zero, iFont.hFont);

            Ole32.IFontDisp iFontDisp = (Ole32.IFontDisp)disp;
            Assert.Equal(font.Name, iFontDisp.Name);
            Assert.Equal(10, iFontDisp.Size);
            Assert.True(iFontDisp.Bold);
            Assert.True(iFontDisp.Italic);
            Assert.True(iFontDisp.Underline);
            Assert.True(iFontDisp.Strikethrough);
            Assert.Equal(0, iFontDisp.Charset);

            Font result = SubAxHost.GetFontFromIFont(iFont);
            Assert.Equal(font.Name, result.Name);
            Assert.Equal(9.75, result.Size);
            Assert.Equal(font.Style, result.Style);
            Assert.Equal(10, result.GdiCharSet);

            result = SubAxHost.GetFontFromIFont(iFontDisp);
            Assert.Equal(font.Name, result.Name);
            Assert.Equal(9.75, result.Size);
            Assert.Equal(font.Style, result.Style);
            Assert.Equal(10, result.GdiCharSet);
        }

        [WinFormsTheory]
        [InlineData(GraphicsUnit.Document)]
        [InlineData(GraphicsUnit.Inch)]
        [InlineData(GraphicsUnit.Millimeter)]
        [InlineData(GraphicsUnit.Pixel)]
        [InlineData(GraphicsUnit.World)]
        public void AxHost_GetIFontDispFromFont_InvalidFontUnit_ThrowsArgumentException(GraphicsUnit unit)
        {
            using var font = new Font("Arial", 10, unit);
            Assert.Throws<ArgumentException>("font", () => SubAxHost.GetIFontDispFromFont(font));
        }

        [WinFormsFact]
        public void AxHost_GetIFontDispFromFont_NullFont_ReturnsNull()
        {
            Assert.Null(SubAxHost.GetIFontDispFromFont(null));
        }

        [WinFormsFact]
        public void AxHost_GetIFontFromFont_InvokeSimpleStyle_Roundtrips()
        {
            using var font = new Font("Arial", 10);
            Ole32.IFont iFont = (Ole32.IFont)SubAxHost.GetIFontFromFont(font);
            Assert.Equal(font.Name, iFont.Name);
            Assert.Equal(97500, iFont.Size);
            Assert.False(iFont.Bold);
            Assert.False(iFont.Italic);
            Assert.False(iFont.Underline);
            Assert.False(iFont.Strikethrough);
            // Charset is locale specific
            // Assert.Equal(0, iFont.Charset);
            Assert.NotEqual(IntPtr.Zero, iFont.hFont);

            Font result = SubAxHost.GetFontFromIFont(iFont);
            Assert.Equal(font.Name, result.Name);
            Assert.Equal(9.75, result.Size);
            Assert.Equal(font.Style, result.Style);
            Assert.Equal(1, result.GdiCharSet);
        }

        [WinFormsFact]
        public void AxHost_GetIFontFromFont_InvokeComplexStyle_Roundtrips()
        {
            using var font = new Font("Arial", 10, FontStyle.Bold | FontStyle.Underline | FontStyle.Italic | FontStyle.Strikeout, GraphicsUnit.Point, 10);
            Ole32.IFont iFont = (Ole32.IFont)SubAxHost.GetIFontFromFont(font);
            Assert.Equal(font.Name, iFont.Name);
            Assert.Equal(97500, iFont.Size);
            Assert.True(iFont.Bold);
            Assert.True(iFont.Italic);
            Assert.True(iFont.Underline);
            Assert.True(iFont.Strikethrough);
            Assert.Equal(0, iFont.Charset);
            Assert.NotEqual(IntPtr.Zero, iFont.hFont);

            Font result = SubAxHost.GetFontFromIFont(iFont);
            Assert.Equal(font.Name, result.Name);
            Assert.Equal(9.75, result.Size);
            Assert.Equal(font.Style, result.Style);
            Assert.Equal(10, result.GdiCharSet);
        }

        [WinFormsTheory]
        [InlineData(GraphicsUnit.Document)]
        [InlineData(GraphicsUnit.Inch)]
        [InlineData(GraphicsUnit.Millimeter)]
        [InlineData(GraphicsUnit.Pixel)]
        [InlineData(GraphicsUnit.World)]
        public void AxHost_GetIFontFromFont_InvalidFontUnit_ThrowsArgumentException(GraphicsUnit unit)
        {
            using var font = new Font("Arial", 10, unit);
            Assert.Throws<ArgumentException>("font", () => SubAxHost.GetIFontFromFont(font));
        }

        [WinFormsFact]
        public void AxHost_GetIFontFromFont_NullFont_ReturnsNull()
        {
            Assert.Null(SubAxHost.GetIFontFromFont(null));
        }

        [WinFormsFact]
        public unsafe void AxHost_GetIPictureFromCursor_Invoke_Roundtrips()
        {
            var original = new Cursor("bitmaps/cursor.cur");
            IPicture.Interface iPicture = (IPicture.Interface)SubAxHost.GetIPictureFromCursor(original);
            Assert.NotNull(iPicture);

            iPicture.get_Handle(out uint handle).ThrowOnFailure();
            iPicture.get_Type(out short type).ThrowOnFailure();
            iPicture.get_Width(out int width).ThrowOnFailure();
            iPicture.get_Height(out int height).ThrowOnFailure();
            iPicture.get_Attributes(out uint attributes).ThrowOnFailure();

            Assert.NotEqual(0u, handle);
            Assert.True(iPicture.get_hPal(out _).Failed);
            Assert.Equal(3, type);
            Assert.Equal(847, width);
            Assert.Equal(847, height);
            HDC curDc;
            Assert.True(iPicture.get_CurDC(&curDc).Failed);
            Assert.Equal(2u, attributes);

            Assert.Throws<InvalidCastException>(() => SubAxHost.GetPictureFromIPicture(iPicture));
        }

        [WinFormsFact]
        public unsafe void AxHost_GetIPictureDispFromPicture_InvokeBitmap_Roundtrips()
        {
            var original = new Bitmap(10, 11);
            original.SetPixel(1, 2, Color.FromArgb(unchecked((int)0xFF010203)));
            object disp = SubAxHost.GetIPictureDispFromPicture(original);
            using var iPictureDisp = ComHelpers.GetComScope<IDispatch>(disp, out bool succeeded);

            Assert.True(succeeded);
            using VARIANT variant = default;
            ComHelpers.GetDispatchProperty(iPictureDisp, PInvoke.DISPID_PICT_HANDLE, &variant).ThrowOnFailure();
            Assert.NotEqual(0u, variant.data.uintVal);
            ComHelpers.GetDispatchProperty(iPictureDisp, PInvoke.DISPID_PICT_HPAL, &variant).ThrowOnFailure();
            Assert.Equal(0u, variant.data.uintVal);
            ComHelpers.GetDispatchProperty(iPictureDisp, PInvoke.DISPID_PICT_TYPE, &variant).ThrowOnFailure();
            Assert.Equal(1, variant.data.iVal);
            ComHelpers.GetDispatchProperty(iPictureDisp, PInvoke.DISPID_PICT_WIDTH, &variant).ThrowOnFailure();
            Assert.Equal(265u, variant.data.uintVal);
            ComHelpers.GetDispatchProperty(iPictureDisp, PInvoke.DISPID_PICT_HEIGHT, &variant).ThrowOnFailure();
            Assert.Equal(291u, variant.data.uintVal);

            var result = Assert.IsType<Bitmap>(SubAxHost.GetPictureFromIPictureDisp(disp));
            Assert.Equal(original.Size, result.Size);
            Assert.Equal(PixelFormat.Format32bppRgb, result.PixelFormat);
            Assert.Equal(Color.FromArgb(unchecked((int)0xFF010203)), original.GetPixel(1, 2));
        }

        [WinFormsFact]
        public unsafe void AxHost_GetIPictureDispFromPicture_InvokeEnhancedMetafile_Roundtrips()
        {
            var original = new Metafile("bitmaps/milkmateya01.emf");
            object disp = SubAxHost.GetIPictureDispFromPicture(original);

            using var iPictureDisp = ComHelpers.GetComScope<IDispatch>(disp, out bool succeeded);

            Assert.True(succeeded);
            using VARIANT variant = default;
            ComHelpers.GetDispatchProperty(iPictureDisp, PInvoke.DISPID_PICT_HANDLE, &variant).ThrowOnFailure();
            Assert.NotEqual(0u, variant.data.uintVal);
            Assert.True(ComHelpers.GetDispatchProperty(iPictureDisp, PInvoke.DISPID_PICT_HPAL, &variant).Failed);
            ComHelpers.GetDispatchProperty(iPictureDisp, PInvoke.DISPID_PICT_TYPE, &variant).ThrowOnFailure();
            Assert.Equal(4, variant.data.iVal);
            ComHelpers.GetDispatchProperty(iPictureDisp, PInvoke.DISPID_PICT_WIDTH, &variant).ThrowOnFailure();
            Assert.Equal(19972u, variant.data.uintVal);
            ComHelpers.GetDispatchProperty(iPictureDisp, PInvoke.DISPID_PICT_HEIGHT, &variant).ThrowOnFailure();
            Assert.Equal(28332u, variant.data.uintVal);

            var result = Assert.IsType<Metafile>(SubAxHost.GetPictureFromIPictureDisp(disp));
            Assert.Equal(new Size(759, 1073), result.Size);
        }

        [WinFormsFact]
        public void AxHost_GetIPictureDispFromPicture_InvokeMetafile_ThrowsCOMException()
        {
            var original = new Metafile("bitmaps/telescope_01.wmf");
            Assert.Throws<COMException>(() => SubAxHost.GetIPictureDispFromPicture(original));
        }

        [WinFormsFact]
        public void AxHost_GetIPictureDispFromPicture_NullImage_ReturnsNull()
        {
            Assert.Null(SubAxHost.GetIPictureDispFromPicture(null));
        }

        [WinFormsFact]
        public unsafe void AxHost_GetIPictureFromPicture_InvokeBitmap_Roundtrips()
        {
            var original = new Bitmap(10, 11);
            original.SetPixel(1, 2, Color.FromArgb(unchecked((int)0xFF010203)));
            IPicture.Interface iPicture = (IPicture.Interface)SubAxHost.GetIPictureFromPicture(original);
            Assert.NotNull(iPicture);

            iPicture.get_Handle(out uint handle).ThrowOnFailure();
            iPicture.get_hPal(out uint hPal).ThrowOnFailure();
            iPicture.get_Type(out short type).ThrowOnFailure();
            iPicture.get_Width(out int width).ThrowOnFailure();
            iPicture.get_Height(out int height).ThrowOnFailure();
            iPicture.get_Attributes(out uint attributes).ThrowOnFailure();
            HDC curDc;
            iPicture.get_CurDC(&curDc).ThrowOnFailure();

            Assert.NotEqual(0u, handle);
            Assert.Equal(0u, hPal);
            Assert.Equal(1, type);
            Assert.Equal(265, width);
            Assert.Equal(291, height);
            Assert.Equal(HDC.Null, curDc);
            Assert.Equal(0u, attributes);

            var result = Assert.IsType<Bitmap>(SubAxHost.GetPictureFromIPicture(iPicture));
            Assert.Equal(original.Size, result.Size);
            Assert.Equal(PixelFormat.Format32bppRgb, result.PixelFormat);
            Assert.Equal(Color.FromArgb(unchecked((int)0xFF010203)), original.GetPixel(1, 2));
        }

        [WinFormsFact]
        public unsafe void AxHost_GetIPictureFromPicture_InvokeEnhancedMetafile_Roundtrips()
        {
            var original = new Metafile("bitmaps/milkmateya01.emf");
            IPicture.Interface iPicture = (IPicture.Interface)SubAxHost.GetIPictureFromPicture(original);
            Assert.NotNull(iPicture);

            iPicture.get_Handle(out uint handle).ThrowOnFailure();
            iPicture.get_Type(out short type).ThrowOnFailure();
            iPicture.get_Width(out int width).ThrowOnFailure();
            iPicture.get_Height(out int height).ThrowOnFailure();
            iPicture.get_Attributes(out uint attributes).ThrowOnFailure();

            Assert.NotEqual(0u, handle);
            Assert.True(iPicture.get_hPal(out _).Failed);
            Assert.Equal(4, type);
            Assert.Equal(19972, width);
            Assert.Equal(28332, height);
            HDC curDc;
            Assert.True(iPicture.get_CurDC(&curDc).Failed);
            Assert.Equal(3u, attributes);

            var result = Assert.IsType<Metafile>(SubAxHost.GetPictureFromIPicture(iPicture));
            Assert.Equal(new Size(759, 1073), result.Size);
        }

        [WinFormsFact]
        public void AxHost_GetIPictureFromPicture_InvokeMetafile_ThrowsCOMException()
        {
            var original = new Metafile("bitmaps/telescope_01.wmf");
            Assert.Throws<COMException>(() => SubAxHost.GetIPictureFromPicture(original));
        }

        [WinFormsFact]
        public void AxHost_GetIPictureFromPicture_NullImage_ReturnsNull()
        {
            Assert.Null(SubAxHost.GetIPictureFromPicture(null));
        }

        [WinFormsFact]
        public void AxHost_GetPictureFromIPicture_InvokeInvalid_ThrowsInvalidCastException()
        {
            Assert.Throws<InvalidCastException>(() => SubAxHost.GetPictureFromIPicture(new object()));
        }

        [WinFormsFact]
        public void AxHost_GetPictureFromIPicture_NullPicture_ReturnsNull()
        {
            Assert.Null(SubAxHost.GetPictureFromIPicture(null));
        }

        [WinFormsFact]
        public void AxHost_InvokeEditMode_Invoke_Success()
        {
            using var control = new SubAxHost(EmptyClsidString);
            control.InvokeEditMode();
            Assert.Null(control.GetOcx());

            // Call again.
            control.InvokeEditMode();
            Assert.Null(control.GetOcx());
        }

        public static IEnumerable<object[]> InvokeEditMode_Site_TestData()
        {
            yield return new object[] { true, null, 2 };
            yield return new object[] { true, new object(), 2 };

            yield return new object[] { false, null, 1 };
            yield return new object[] { false, new object(), 1 };
            yield return new object[] { false, new Mock<ISelectionService>(MockBehavior.Strict).Object, 1 };
        }

        [WinFormsTheory]
        [MemberData(nameof(InvokeEditMode_Site_TestData))]
        public void AxHost_InvokeEditMode_InvokeWithSite_Success(bool designMode, object selectionService, int expectedCallCount)
        {
            var mockSite = new Mock<ISite>(MockBehavior.Strict);
            mockSite
                .Setup(s => s.Container)
                .Returns((IContainer)null);
            mockSite
                .Setup(s => s.GetService(typeof(AmbientProperties)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IComponentChangeService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IDesignerHost)))
                .Returns(null);
            mockSite
                .Setup(s => s.DesignMode)
                .Returns(designMode);
            mockSite
                .Setup(s => s.GetService(typeof(ISelectionService)))
                .Returns(selectionService)
                .Verifiable();
            mockSite
                .Setup(s => s.Name)
                .Returns("Name");
            using var control = new SubAxHost(WebBrowserClsidString)
            {
                Site = mockSite.Object
            };
            control.InvokeEditMode();
            Assert.False(control.IsHandleCreated);
            mockSite.Verify(s => s.GetService(typeof(ISelectionService)), Times.Exactly(expectedCallCount));

            // Call again.
            control.InvokeEditMode();
            Assert.False(control.IsHandleCreated);
            mockSite.Verify(s => s.GetService(typeof(ISelectionService)), Times.Exactly(expectedCallCount));
        }

        public static IEnumerable<object[]> InvokeEditMode_SiteWithParent_TestData()
        {
            yield return new object[] { true, null, 3 };
            yield return new object[] { true, new object(), 3 };

            yield return new object[] { false, null, 1 };
            yield return new object[] { false, new object(), 1 };
            yield return new object[] { false, new Mock<ISelectionService>(MockBehavior.Strict).Object, 1 };
        }

        [WinFormsTheory]
        [MemberData(nameof(InvokeEditMode_SiteWithParent_TestData))]
        public void AxHost_InvokeEditMode_InvokeWithSiteWithParent_Success(bool designMode, object selectionService, int expectedCallCount)
        {
            using var parent = new Control();
            var mockSite = new Mock<ISite>(MockBehavior.Strict);
            mockSite
                .Setup(s => s.Container)
                .Returns((IContainer)null);
            mockSite
                .Setup(s => s.GetService(typeof(AmbientProperties)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IComponentChangeService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IDesignerHost)))
                .Returns(null);
            mockSite
                .Setup(s => s.DesignMode)
                .Returns(designMode);
            mockSite
                .Setup(s => s.GetService(typeof(ISelectionService)))
                .Returns(selectionService)
                .Verifiable();
            mockSite
                .Setup(s => s.GetService(typeof(IExtenderListService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IDictionaryService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(ITypeDescriptorFilterService)))
                .Returns(null);
            mockSite
                .Setup(s => s.Name)
                .Returns("Name");
            using var control = new SubAxHost(WebBrowserClsidString)
            {
                Parent = parent,
                Site = mockSite.Object
            };
            control.InvokeEditMode();
            Assert.True(control.IsHandleCreated);
            Assert.True(parent.IsHandleCreated);
            mockSite.Verify(s => s.GetService(typeof(ISelectionService)), Times.Exactly(expectedCallCount));

            // Call again.
            control.InvokeEditMode();
            Assert.True(control.IsHandleCreated);
            Assert.True(parent.IsHandleCreated);
            mockSite.Verify(s => s.GetService(typeof(ISelectionService)), Times.Exactly(expectedCallCount));
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(CommonTestHelper), nameof(CommonTestHelper.GetBoolTheoryData))]
        public void AxHost_InvokeEditMode_InvokeWithSiteDesignModeWithComponentSelectedNoSelectionStyleProperty_Success(bool componentSelected)
        {
            using var control = new SubAxHost(WebBrowserClsidString);
            var mockSelectionService = new Mock<ISelectionService>(MockBehavior.Strict);
            mockSelectionService
                .Setup(s => s.GetComponentSelected(control))
                .Returns(componentSelected)
                .Verifiable();
            var mockSite = new Mock<ISite>(MockBehavior.Strict);
            mockSite
                .Setup(s => s.Container)
                .Returns((IContainer)null);
            mockSite
                .Setup(s => s.GetService(typeof(AmbientProperties)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IComponentChangeService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IDesignerHost)))
                .Returns(null);
            mockSite
                .Setup(s => s.DesignMode)
                .Returns(true);
            mockSite
                .Setup(s => s.GetService(typeof(ISelectionService)))
                .Returns(mockSelectionService.Object)
                .Verifiable();
            mockSite
                .Setup(s => s.GetService(typeof(IExtenderListService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IDictionaryService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(ITypeDescriptorFilterService)))
                .Returns(null);
            mockSite
                .Setup(s => s.Name)
                .Returns("Name");
            control.Site = mockSite.Object;
            control.InvokeEditMode();
            Assert.False(control.IsHandleCreated);
            mockSite.Verify(s => s.GetService(typeof(ISelectionService)), Times.Exactly(2));
            mockSelectionService.Verify(s => s.GetComponentSelected(control), Times.Once());

            // Call again.
            control.InvokeEditMode();
            Assert.False(control.IsHandleCreated);
            mockSite.Verify(s => s.GetService(typeof(ISelectionService)), Times.Exactly(2));
            mockSelectionService.Verify(s => s.GetComponentSelected(control), Times.Once());
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(CommonTestHelper), nameof(CommonTestHelper.GetBoolTheoryData))]
        public void AxHost_InvokeEditMode_InvokeWithSiteDesignModeWithComponentSelectedInvalidSelectionStyleProperty_Success(bool componentSelected)
        {
            using var control = new InvalidSelectionStyleAxHost(WebBrowserClsidString);
            var mockSelectionService = new Mock<ISelectionService>(MockBehavior.Strict);
            mockSelectionService
                .Setup(s => s.GetComponentSelected(control))
                .Returns(componentSelected)
                .Verifiable();
            var mockSite = new Mock<ISite>(MockBehavior.Strict);
            mockSite
                .Setup(s => s.Container)
                .Returns((IContainer)null);
            mockSite
                .Setup(s => s.GetService(typeof(AmbientProperties)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IComponentChangeService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IDesignerHost)))
                .Returns(null);
            mockSite
                .Setup(s => s.DesignMode)
                .Returns(true);
            mockSite
                .Setup(s => s.GetService(typeof(ISelectionService)))
                .Returns(mockSelectionService.Object)
                .Verifiable();
            mockSite
                .Setup(s => s.GetService(typeof(IExtenderListService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IDictionaryService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(ITypeDescriptorFilterService)))
                .Returns(null);
            mockSite
                .Setup(s => s.Name)
                .Returns("Name");
            control.Site = mockSite.Object;
            control.InvokeEditMode();
            Assert.Null(control.SelectionStyle);
            Assert.False(control.IsHandleCreated);
            mockSite.Verify(s => s.GetService(typeof(ISelectionService)), Times.Exactly(2));
            mockSelectionService.Verify(s => s.GetComponentSelected(control), Times.Once());

            // Call again.
            control.InvokeEditMode();
            Assert.Null(control.SelectionStyle);
            Assert.False(control.IsHandleCreated);
            mockSite.Verify(s => s.GetService(typeof(ISelectionService)), Times.Exactly(2));
            mockSelectionService.Verify(s => s.GetComponentSelected(control), Times.Once());
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(CommonTestHelper), nameof(CommonTestHelper.GetBoolTheoryData))]
        public void AxHost_InvokeEditMode_InvokeWithSiteDesignModeWithComponentSelectedValidSelectionStyleProperty_Success(bool componentSelected)
        {
            using var control = new ValidSelectionStyleAxHost(WebBrowserClsidString);
            var mockSelectionService = new Mock<ISelectionService>(MockBehavior.Strict);
            mockSelectionService
                .Setup(s => s.GetComponentSelected(control))
                .Returns(componentSelected)
                .Verifiable();
            var mockSite = new Mock<ISite>(MockBehavior.Strict);
            mockSite
                .Setup(s => s.Container)
                .Returns((IContainer)null);
            mockSite
                .Setup(s => s.GetService(typeof(AmbientProperties)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IComponentChangeService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IDesignerHost)))
                .Returns(null);
            mockSite
                .Setup(s => s.DesignMode)
                .Returns(true);
            mockSite
                .Setup(s => s.GetService(typeof(ISelectionService)))
                .Returns(mockSelectionService.Object)
                .Verifiable();
            mockSite
                .Setup(s => s.GetService(typeof(IExtenderListService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IDictionaryService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(ITypeDescriptorFilterService)))
                .Returns(null);
            mockSite
                .Setup(s => s.Name)
                .Returns("Name");
            control.Site = mockSite.Object;

            control.InvokeEditMode();
            Assert.Equal(0, control.SelectionStyle);
            Assert.False(control.IsHandleCreated);
            mockSite.Verify(s => s.GetService(typeof(ISelectionService)), Times.Exactly(2));
            mockSelectionService.Verify(s => s.GetComponentSelected(control), Times.Once());

            // Call again.
            control.InvokeEditMode();
            Assert.Equal(0, control.SelectionStyle);
            Assert.False(control.IsHandleCreated);
            mockSite.Verify(s => s.GetService(typeof(ISelectionService)), Times.Exactly(2));
            mockSelectionService.Verify(s => s.GetComponentSelected(control), Times.Once());
        }

        public static IEnumerable<object[]> InvokeEditMode_SiteWithHandle_TestData()
        {
            yield return new object[] { true, null, 3 };
            yield return new object[] { true, new object(), 3 };

            yield return new object[] { false, null, 1 };
            yield return new object[] { false, new object(), 1 };
            yield return new object[] { false, new Mock<ISelectionService>(MockBehavior.Strict).Object, 1 };
        }

        [WinFormsTheory]
        [MemberData(nameof(InvokeEditMode_SiteWithHandle_TestData))]
        public void AxHost_InvokeEditMode_InvokeWithSiteWithHandle_Success(bool designMode, object selectionService, int expectedCallCount)
        {
            var mockSite = new Mock<ISite>(MockBehavior.Strict);
            mockSite
                .Setup(s => s.Container)
                .Returns((IContainer)null);
            mockSite
                .Setup(s => s.GetService(typeof(AmbientProperties)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IComponentChangeService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IDesignerHost)))
                .Returns(null);
            mockSite
                .Setup(s => s.DesignMode)
                .Returns(designMode);
            mockSite
                .Setup(s => s.GetService(typeof(ISelectionService)))
                .Returns(selectionService)
                .Verifiable();
            mockSite
                .Setup(s => s.GetService(typeof(IExtenderListService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IDictionaryService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(ITypeDescriptorFilterService)))
                .Returns(null);
            mockSite
                .Setup(s => s.Name)
                .Returns("Name");
            using var control = new SubAxHost(WebBrowserClsidString)
            {
                Site = mockSite.Object
            };
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            ((Control)control).StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.InvokeEditMode();
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            mockSite.Verify(s => s.GetService(typeof(ISelectionService)), Times.Exactly(expectedCallCount));

            // Call again.
            control.InvokeEditMode();
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            mockSite.Verify(s => s.GetService(typeof(ISelectionService)), Times.Exactly(expectedCallCount));
        }

        [WinFormsTheory]
        [MemberData(nameof(InvokeEditMode_SiteWithParent_TestData))]
        public void AxHost_InvokeEditMode_InvokeWithSiteWithParentWithHandle_Success(bool designMode, object selectionService, int expectedCallCount)
        {
            using var parent = new Control();
            var mockSite = new Mock<ISite>(MockBehavior.Strict);
            mockSite
                .Setup(s => s.Container)
                .Returns((IContainer)null);
            mockSite
                .Setup(s => s.GetService(typeof(AmbientProperties)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IComponentChangeService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IDesignerHost)))
                .Returns(null);
            mockSite
                .Setup(s => s.DesignMode)
                .Returns(designMode);
            mockSite
                .Setup(s => s.GetService(typeof(ISelectionService)))
                .Returns(selectionService)
                .Verifiable();
            mockSite
                .Setup(s => s.GetService(typeof(IExtenderListService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IDictionaryService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(ITypeDescriptorFilterService)))
                .Returns(null);
            mockSite
                .Setup(s => s.Name)
                .Returns("Name");
            using var control = new SubAxHost(WebBrowserClsidString)
            {
                Parent = parent,
                Site = mockSite.Object
            };
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            ((Control)control).StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.InvokeEditMode();
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.True(parent.IsHandleCreated);
            mockSite.Verify(s => s.GetService(typeof(ISelectionService)), Times.Exactly(expectedCallCount));

            // Call again.
            control.InvokeEditMode();
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.True(parent.IsHandleCreated);
            mockSite.Verify(s => s.GetService(typeof(ISelectionService)), Times.Exactly(expectedCallCount));
        }

        [WinFormsTheory]
        [InlineData(true, 2)]
        [InlineData(false, 0)]
        public void AxHost_InvokeEditMode_InvokeWithSiteDesignModeWithComponentSelectedValidSelectionStylePropertyWithHandle_Success(bool componentSelected, int expectedSelectionStyle)
        {
            using var control = new ValidSelectionStyleAxHost(WebBrowserClsidString);
            var mockSelectionService = new Mock<ISelectionService>(MockBehavior.Strict);
            mockSelectionService
                .Setup(s => s.GetComponentSelected(control))
                .Returns(componentSelected)
                .Verifiable();
            var mockSite = new Mock<ISite>(MockBehavior.Strict);
            mockSite
                .Setup(s => s.Container)
                .Returns((IContainer)null);
            mockSite
                .Setup(s => s.GetService(typeof(AmbientProperties)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IComponentChangeService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IDesignerHost)))
                .Returns(null);
            mockSite
                .Setup(s => s.DesignMode)
                .Returns(true);
            mockSite
                .Setup(s => s.GetService(typeof(ISelectionService)))
                .Returns(mockSelectionService.Object)
                .Verifiable();
            mockSite
                .Setup(s => s.GetService(typeof(IExtenderListService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IDictionaryService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(ITypeDescriptorFilterService)))
                .Returns(null);
            mockSite
                .Setup(s => s.Name)
                .Returns("Name");
            control.Site = mockSite.Object;
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            ((Control)control).StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.InvokeEditMode();
            Assert.Equal(expectedSelectionStyle, control.SelectionStyle);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            mockSite.Verify(s => s.GetService(typeof(ISelectionService)), Times.Exactly(3));
            mockSelectionService.Verify(s => s.GetComponentSelected(control), Times.Exactly(2));

            // Call again.
            control.InvokeEditMode();
            Assert.Equal(expectedSelectionStyle, control.SelectionStyle);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            mockSite.Verify(s => s.GetService(typeof(ISelectionService)), Times.Exactly(3));
            mockSelectionService.Verify(s => s.GetComponentSelected(control), Times.Exactly(2));
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(CommonTestHelper), nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void AxHost_OnEnter_Invoke_CallsEnter(EventArgs eventArgs)
        {
            using var control = new SubAxHost(EmptyClsidString);
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.Enter += handler;
            control.OnEnter(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.Enter -= handler;
            control.OnEnter(eventArgs);
            Assert.Equal(1, callCount);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetKeyEventArgsTheoryData))]
        public void AxHost_OnLeave_Invoke_CallsLeave(KeyEventArgs eventArgs)
        {
            using var control = new SubAxHost(EmptyClsidString);
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.Leave += handler;
            control.OnLeave(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.Leave -= handler;
            control.OnLeave(eventArgs);
            Assert.Equal(1, callCount);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(CommonTestHelper), nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void AxHost_OnMouseCaptureChanged_Invoke_CallsMouseCaptureChanged(EventArgs eventArgs)
        {
            using var control = new SubAxHost(EmptyClsidString);
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.MouseCaptureChanged += handler;
            control.OnMouseCaptureChanged(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.MouseCaptureChanged -= handler;
            control.OnMouseCaptureChanged(eventArgs);
            Assert.Equal(1, callCount);
        }

        [WinFormsFact]
        public void AxHost_ICustomTypeDescriptorGetAttributes_Invoke_ReturnsExpected()
        {
            using var control = new CustomAxHost(EmptyClsidString);
            ICustomTypeDescriptor customTypeDescriptor = control;
            AttributeCollection attributes = customTypeDescriptor.GetAttributes();
            Assert.NotNull(attributes[typeof(CustomAttribute)]);
            Assert.False(control.IsHandleCreated);

            // Call again.
            attributes = customTypeDescriptor.GetAttributes();
            Assert.NotNull(attributes[typeof(CustomAttribute)]);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void AxHost_ICustomTypeDescriptorGetAttributes_InvokeWithHandle_ReturnsExpected()
        {
            using var control = new CustomAxHost(WebBrowserClsidString);
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            ((Control)control).StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            ICustomTypeDescriptor customTypeDescriptor = control;
            AttributeCollection attributes = customTypeDescriptor.GetAttributes();
            Assert.NotNull(attributes[typeof(CustomAttribute)]);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Call again.
            attributes = customTypeDescriptor.GetAttributes();
            Assert.NotNull(attributes[typeof(CustomAttribute)]);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void AxHost_ICustomTypeDescriptorGetClassName_Invoke_ReturnsExpected()
        {
            using var control = new CustomAxHost(EmptyClsidString);
            ICustomTypeDescriptor customTypeDescriptor = control;
            Assert.Null(customTypeDescriptor.GetClassName());
            Assert.False(control.IsHandleCreated);

            // Call again.
            Assert.Null(customTypeDescriptor.GetClassName());
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void AxHost_ICustomTypeDescriptorGetClassName_InvokeWithHandle_ReturnsExpected()
        {
            using var control = new CustomAxHost(WebBrowserClsidString);
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            ((Control)control).StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            ICustomTypeDescriptor customTypeDescriptor = control;
            Assert.Null(customTypeDescriptor.GetClassName());
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Call again.
            Assert.Null(customTypeDescriptor.GetClassName());
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void AxHost_ICustomTypeDescriptorGetComponentName_Invoke_ReturnsExpected()
        {
            using var control = new CustomAxHost(EmptyClsidString);
            ICustomTypeDescriptor customTypeDescriptor = control;
            Assert.Null(customTypeDescriptor.GetComponentName());
            Assert.False(control.IsHandleCreated);

            // Call again.
            Assert.Null(customTypeDescriptor.GetComponentName());
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void AxHost_ICustomTypeDescriptorGetComponentName_InvokeWithHandle_ReturnsExpected()
        {
            using var control = new CustomAxHost(WebBrowserClsidString);
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            ((Control)control).StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            ICustomTypeDescriptor customTypeDescriptor = control;
            Assert.Null(customTypeDescriptor.GetComponentName());
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Call again.
            Assert.Null(customTypeDescriptor.GetComponentName());
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void AxHost_ICustomTypeDescriptorGetConverter_Invoke_ReturnsExpected()
        {
            using var control = new CustomAxHost(EmptyClsidString);
            ICustomTypeDescriptor customTypeDescriptor = control;
            Assert.Null(customTypeDescriptor.GetConverter());
            Assert.False(control.IsHandleCreated);

            // Call again.
            Assert.Null(customTypeDescriptor.GetConverter());
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void AxHost_ICustomTypeDescriptorGetConverter_InvokeWithHandle_ReturnsExpected()
        {
            using var control = new CustomAxHost(WebBrowserClsidString);
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            ((Control)control).StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            ICustomTypeDescriptor customTypeDescriptor = control;
            Assert.Null(customTypeDescriptor.GetConverter());
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Call again.
            Assert.Null(customTypeDescriptor.GetConverter());
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void AxHost_ICustomTypeDescriptorGetDefaultEvent_Invoke_ReturnsExpected()
        {
            using var control = new CustomAxHost(EmptyClsidString);
            ICustomTypeDescriptor customTypeDescriptor = control;
            EventDescriptor eventDescriptor = customTypeDescriptor.GetDefaultEvent();
            Assert.Equal(nameof(AxHost.Enter), eventDescriptor.Name);
            Assert.False(control.IsHandleCreated);

            // Call again.
            eventDescriptor = customTypeDescriptor.GetDefaultEvent();
            Assert.Equal(nameof(AxHost.Enter), eventDescriptor.Name);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void AxHost_ICustomTypeDescriptorGetDefaultEvent_InvokeWithHandle_ReturnsExpected()
        {
            using var control = new CustomAxHost(WebBrowserClsidString);
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            ((Control)control).StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            ICustomTypeDescriptor customTypeDescriptor = control;
            EventDescriptor eventDescriptor = customTypeDescriptor.GetDefaultEvent();
            Assert.Equal(nameof(AxHost.Enter), eventDescriptor.Name);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Call again.
            eventDescriptor = customTypeDescriptor.GetDefaultEvent();
            Assert.Equal(nameof(AxHost.Enter), eventDescriptor.Name);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void AxHost_ICustomTypeDescriptorGetDefaultProperty_Invoke_ReturnsExpected()
        {
            using var control = new CustomAxHost(EmptyClsidString);
            ICustomTypeDescriptor customTypeDescriptor = control;
            PropertyDescriptor propertyDescriptor = customTypeDescriptor.GetDefaultProperty();
            Assert.Equal(nameof(AxHost.Text), propertyDescriptor.Name);
            Assert.False(control.IsHandleCreated);

            // Call again.
            propertyDescriptor = customTypeDescriptor.GetDefaultProperty();
            Assert.Equal(nameof(AxHost.Text), propertyDescriptor.Name);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void AxHost_ICustomTypeDescriptorGetDefaultProperty_InvokeWithHandle_ReturnsExpected()
        {
            using var control = new CustomAxHost(WebBrowserClsidString);
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            ((Control)control).StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            ICustomTypeDescriptor customTypeDescriptor = control;
            PropertyDescriptor propertyDescriptor = customTypeDescriptor.GetDefaultProperty();
            Assert.Equal(nameof(AxHost.Text), propertyDescriptor.Name);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Call again.
            propertyDescriptor = customTypeDescriptor.GetDefaultProperty();
            Assert.Equal(nameof(AxHost.Text), propertyDescriptor.Name);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [InlineData(null)]
        [InlineData(typeof(int))]
        [InlineData(typeof(ComponentEditor))]
        [InlineData(typeof(SubComponentEditor))]
        public void AxHost_ICustomTypeDescriptorGetEditor_typeInvoke_ReturnsExpected(Type editorBaseType)
        {
            using var control = new CustomAxHost(EmptyClsidString);
            ICustomTypeDescriptor customTypeDescriptor = control;
            Assert.Null(customTypeDescriptor.GetEditor(editorBaseType));
            Assert.False(control.IsHandleCreated);

            // Call again.
            Assert.Null(customTypeDescriptor.GetEditor(editorBaseType));
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(null)]
        [InlineData(typeof(int))]
        [InlineData(typeof(ComponentEditor))]
        [InlineData(typeof(SubComponentEditor))]
        public void AxHost_ICustomTypeDescriptorGetEditor_typeInvokeWithHandle_ReturnsExpected(Type editorBaseType)
        {
            using var control = new CustomAxHost(WebBrowserClsidString);
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            ((Control)control).StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            ICustomTypeDescriptor customTypeDescriptor = control;
            Assert.Null(customTypeDescriptor.GetEditor(editorBaseType));
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Call again.
            Assert.Null(customTypeDescriptor.GetEditor(editorBaseType));
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void AxHost_ICustomTypeDescriptorGetEvents_Invoke_ReturnsExpected()
        {
            using var control = new CustomAxHost(EmptyClsidString);
            ICustomTypeDescriptor customTypeDescriptor = control;
            EventDescriptorCollection events = customTypeDescriptor.GetEvents();
            Assert.True(events.Count > 1);
            Assert.NotNull(events[nameof(CustomAxHost.CustomEvent)]);
            Assert.False(control.IsHandleCreated);

            // Call again.
            events = customTypeDescriptor.GetEvents();
            Assert.True(events.Count > 1);
            Assert.NotNull(events[nameof(CustomAxHost.CustomEvent)]);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void AxHost_ICustomTypeDescriptorGetEvents_InvokeWithHandle_ReturnsExpected()
        {
            using var control = new CustomAxHost(WebBrowserClsidString);
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            ((Control)control).StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            ICustomTypeDescriptor customTypeDescriptor = control;
            EventDescriptorCollection events = customTypeDescriptor.GetEvents();
            Assert.True(events.Count > 1);
            Assert.NotNull(events[nameof(CustomAxHost.CustomEvent)]);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Call again.
            events = customTypeDescriptor.GetEvents();
            Assert.True(events.Count > 1);
            Assert.NotNull(events[nameof(CustomAxHost.CustomEvent)]);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        public static IEnumerable<object[]> GetEvents_AttributeArray_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { Array.Empty<Attribute>() };
            yield return new object[] { new Attribute[] { null } };
        }

        [WinFormsTheory]
        [MemberData(nameof(GetEvents_AttributeArray_TestData))]
        public void AxHost_ICustomTypeDescriptorGetEvents_InvokeAttributeArray_ReturnsExpected(Attribute[] attributes)
        {
            using var control = new CustomAxHost(EmptyClsidString);
            ICustomTypeDescriptor customTypeDescriptor = control;
            EventDescriptorCollection events = customTypeDescriptor.GetEvents(attributes);
            Assert.True(events.Count > 1);
            Assert.NotNull(events[nameof(CustomAxHost.CustomEvent)]);
            Assert.False(control.IsHandleCreated);

            // Call again.
            events = customTypeDescriptor.GetEvents(attributes);
            Assert.True(events.Count > 1);
            Assert.NotNull(events[nameof(CustomAxHost.CustomEvent)]);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(GetEvents_AttributeArray_TestData))]
        public void AxHost_ICustomTypeDescriptorGetEvents_InvokeAttributeArrayWithHandle_ReturnsExpected(Attribute[] attributes)
        {
            using var control = new CustomAxHost(WebBrowserClsidString);
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            ((Control)control).StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            ICustomTypeDescriptor customTypeDescriptor = control;
            EventDescriptorCollection events = customTypeDescriptor.GetEvents(attributes);
            Assert.True(events.Count > 1);
            Assert.NotNull(events[nameof(CustomAxHost.CustomEvent)]);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Call again.
            events = customTypeDescriptor.GetEvents(attributes);
            Assert.True(events.Count > 1);
            Assert.NotNull(events[nameof(CustomAxHost.CustomEvent)]);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void AxHost_ICustomTypeDescriptorGetEvents_InvokeAttributeArrayCustom_ReturnsExpected()
        {
            using var control = new CustomAxHost(EmptyClsidString);
            ICustomTypeDescriptor customTypeDescriptor = control;
            EventDescriptorCollection events = customTypeDescriptor.GetEvents(new Attribute[] { new CustomAttribute() });
            Assert.True(events.Count > 1);
            Assert.NotNull(events[nameof(CustomAxHost.CustomEvent)]);
            Assert.False(control.IsHandleCreated);

            // Call again.
            events = customTypeDescriptor.GetEvents(new Attribute[] { new CustomAttribute() });
            Assert.True(events.Count > 1);
            Assert.NotNull(events[nameof(CustomAxHost.CustomEvent)]);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void AxHost_ICustomTypeDescriptorGetEvents_InvokeAttributeArrayCustomWithHandle_ReturnsExpected()
        {
            using var control = new CustomAxHost(WebBrowserClsidString);
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            ((Control)control).StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            ICustomTypeDescriptor customTypeDescriptor = control;
            EventDescriptorCollection events = customTypeDescriptor.GetEvents(new Attribute[] { new CustomAttribute() });
            Assert.True(events.Count > 1);
            Assert.NotNull(events[nameof(CustomAxHost.CustomEvent)]);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Call again.
            events = customTypeDescriptor.GetEvents(new Attribute[] { new CustomAttribute() });
            Assert.True(events.Count > 1);
            Assert.NotNull(events[nameof(CustomAxHost.CustomEvent)]);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void AxHost_ICustomTypeDescriptorGetEvents_InvokeAttributeArrayNoSuch_ReturnsExpected()
        {
            using var control = new CustomAxHost(EmptyClsidString);
            ICustomTypeDescriptor customTypeDescriptor = control;
            EventDescriptorCollection events = customTypeDescriptor.GetEvents(new Attribute[] { new NoSuchAttribute() });
            Assert.True(events.Count > 1);
            Assert.NotNull(events[nameof(CustomAxHost.CustomEvent)]);
            Assert.False(control.IsHandleCreated);

            // Call again.
            events = customTypeDescriptor.GetEvents(new Attribute[] { new NoSuchAttribute() });
            Assert.True(events.Count > 1);
            Assert.NotNull(events[nameof(CustomAxHost.CustomEvent)]);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void AxHost_ICustomTypeDescriptorGetEvents_InvokeAttributeArrayNoSuchWithHandle_ReturnsExpected()
        {
            using var control = new CustomAxHost(WebBrowserClsidString);
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            ((Control)control).StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            ICustomTypeDescriptor customTypeDescriptor = control;
            EventDescriptorCollection events = customTypeDescriptor.GetEvents(new Attribute[] { new NoSuchAttribute() });
            Assert.True(events.Count > 1);
            Assert.NotNull(events[nameof(CustomAxHost.CustomEvent)]);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Call again.
            events = customTypeDescriptor.GetEvents(new Attribute[] { new NoSuchAttribute() });
            Assert.True(events.Count > 1);
            Assert.NotNull(events[nameof(CustomAxHost.CustomEvent)]);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void AxHost_ICustomTypeDescriptorGetProperties_Invoke_ReturnsExpected()
        {
            using var control = new CustomAxHost(EmptyClsidString);
            ICustomTypeDescriptor customTypeDescriptor = control;
            PropertyDescriptorCollection events = customTypeDescriptor.GetProperties();
            Assert.NotNull(events[nameof(CustomAxHost.CustomProperty)]);
            Assert.False(control.IsHandleCreated);

            // Call again.
            events = customTypeDescriptor.GetProperties();
            Assert.NotNull(events[nameof(CustomAxHost.CustomProperty)]);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void AxHost_ICustomTypeDescriptorGetProperties_InvokeWithHandle_ReturnsExpected()
        {
            using var control = new CustomAxHost(WebBrowserClsidString);
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            ((Control)control).StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            ICustomTypeDescriptor customTypeDescriptor = control;
            PropertyDescriptorCollection events = customTypeDescriptor.GetProperties();
            Assert.NotNull(events[nameof(CustomAxHost.CustomProperty)]);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Call again.
            events = customTypeDescriptor.GetProperties();
            Assert.NotNull(events[nameof(CustomAxHost.CustomProperty)]);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        public static IEnumerable<object[]> GetProperties_AttributeArray_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { Array.Empty<Attribute>() };
#if false
            yield return new object[] { new Attribute[] { null } };
#endif
        }

        [WinFormsTheory]
        [MemberData(nameof(GetProperties_AttributeArray_TestData))]
        public void AxHost_ICustomTypeDescriptorGetProperties_InvokeAttributeArray_ReturnsExpected(Attribute[] attributes)
        {
            using var control = new CustomAxHost(EmptyClsidString);
            ICustomTypeDescriptor customTypeDescriptor = control;
            PropertyDescriptorCollection properties = customTypeDescriptor.GetProperties(attributes);
            Assert.True(properties.Count > 1);
            Assert.NotNull(properties[nameof(CustomAxHost.CustomProperty)]);
            Assert.False(control.IsHandleCreated);

            // Call again.
            properties = customTypeDescriptor.GetProperties(attributes);
            Assert.True(properties.Count > 1);
            Assert.NotNull(properties[nameof(CustomAxHost.CustomProperty)]);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(GetProperties_AttributeArray_TestData))]
        public void AxHost_ICustomTypeDescriptorGetProperties_InvokeAttributeArrayWithHandle_ReturnsExpected(Attribute[] attributes)
        {
            using var control = new CustomAxHost(WebBrowserClsidString);
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            ((Control)control).StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            ICustomTypeDescriptor customTypeDescriptor = control;
            PropertyDescriptorCollection properties = customTypeDescriptor.GetProperties(attributes);
            Assert.True(properties.Count > 1);
            Assert.NotNull(properties[nameof(CustomAxHost.CustomProperty)]);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Call again.
            properties = customTypeDescriptor.GetProperties(attributes);
            Assert.True(properties.Count > 1);
            Assert.NotNull(properties[nameof(CustomAxHost.CustomProperty)]);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void AxHost_ICustomTypeDescriptorGetProperties_InvokeAttributeArrayCustom_ReturnsExpected()
        {
            using var control = new CustomAxHost(EmptyClsidString);
            ICustomTypeDescriptor customTypeDescriptor = control;
            PropertyDescriptorCollection properties = customTypeDescriptor.GetProperties(new Attribute[] { new CustomAttribute() });
            Assert.True(properties.Count > 1);
            Assert.NotNull(properties[nameof(CustomAxHost.CustomProperty)]);
            Assert.False(control.IsHandleCreated);

            // Call again.
            properties = customTypeDescriptor.GetProperties(new Attribute[] { new CustomAttribute() });
            Assert.True(properties.Count > 1);
            Assert.NotNull(properties[nameof(CustomAxHost.CustomProperty)]);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void AxHost_ICustomTypeDescriptorGetProperties_InvokeAttributeArrayCustomWithHandle_ReturnsExpected()
        {
            using var control = new CustomAxHost(WebBrowserClsidString);
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            ((Control)control).StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            ICustomTypeDescriptor customTypeDescriptor = control;
            PropertyDescriptorCollection properties = customTypeDescriptor.GetProperties(new Attribute[] { new CustomAttribute() });
            Assert.True(properties.Count > 1);
            Assert.NotNull(properties[nameof(CustomAxHost.CustomProperty)]);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Call again.
            properties = customTypeDescriptor.GetProperties(new Attribute[] { new CustomAttribute() });
            Assert.True(properties.Count > 1);
            Assert.NotNull(properties[nameof(CustomAxHost.CustomProperty)]);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void AxHost_ICustomTypeDescriptorGetProperties_InvokeAttributeArrayNoSuch_ReturnsExpected()
        {
            using var control = new CustomAxHost(EmptyClsidString);
            ICustomTypeDescriptor customTypeDescriptor = control;
            PropertyDescriptorCollection properties = customTypeDescriptor.GetProperties(new Attribute[] { new NoSuchAttribute() });
            Assert.True(properties.Count > 1);
            Assert.NotNull(properties[nameof(CustomAxHost.CustomProperty)]);
            Assert.False(control.IsHandleCreated);

            // Call again.
            properties = customTypeDescriptor.GetProperties(new Attribute[] { new NoSuchAttribute() });
            Assert.True(properties.Count > 1);
            Assert.NotNull(properties[nameof(CustomAxHost.CustomProperty)]);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void AxHost_ICustomTypeDescriptorGetProperties_InvokeAttributeArrayNoSuchWithHandle_ReturnsExpected()
        {
            using var control = new CustomAxHost(WebBrowserClsidString);
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            ((Control)control).StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            ICustomTypeDescriptor customTypeDescriptor = control;
            PropertyDescriptorCollection properties = customTypeDescriptor.GetProperties(new Attribute[] { new NoSuchAttribute() });
            Assert.True(properties.Count > 1);
            Assert.NotNull(properties[nameof(CustomAxHost.CustomProperty)]);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Call again.
            properties = customTypeDescriptor.GetProperties(new Attribute[] { new NoSuchAttribute() });
            Assert.True(properties.Count > 1);
            Assert.NotNull(properties[nameof(CustomAxHost.CustomProperty)]);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        public static IEnumerable<object[]> GetPropertyOwner_TestData()
        {
            yield return new object[] { null };

            PropertyDescriptor descriptor = TypeDescriptor.GetProperties(typeof(string))[nameof(string.Length)];
            yield return new object[] { descriptor };
        }

        [WinFormsTheory]
        [MemberData(nameof(GetPropertyOwner_TestData))]
        public void AxHost_ICustomTypeDescriptorGetPropertyOwner_Invoke_ReturnsExpected(PropertyDescriptor pd)
        {
            using var control = new CustomAxHost(EmptyClsidString);
            ICustomTypeDescriptor customTypeDescriptor = control;
            Assert.Same(control, customTypeDescriptor.GetPropertyOwner(pd));
            Assert.False(control.IsHandleCreated);

            // Call again.
            Assert.Same(control, customTypeDescriptor.GetPropertyOwner(pd));
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(GetPropertyOwner_TestData))]
        public void AxHost_ICustomTypeDescriptorGetPropertyOwner_InvokeWithHandle_ReturnsExpected(PropertyDescriptor pd)
        {
            using var control = new CustomAxHost(WebBrowserClsidString);
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            ((Control)control).StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            ICustomTypeDescriptor customTypeDescriptor = control;
            Assert.Same(control, customTypeDescriptor.GetPropertyOwner(pd));
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Call again.
            Assert.Same(control, customTypeDescriptor.GetPropertyOwner(pd));
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        private class SubComponentEditor : ComponentEditor
        {
            public override bool EditComponent(ITypeDescriptorContext context, object component)
            {
                throw new NotImplementedException();
            }
        }

        [AttributeUsage(AttributeTargets.All)]
        private class CustomAttribute : Attribute
        {
        }

        [AttributeUsage(AttributeTargets.All)]
        private class NoSuchAttribute : Attribute
        {
        }

        [CustomAttribute]
        private class CustomAxHost : AxHost
        {
            public CustomAxHost(string clsid) : base(clsid)
            {
            }

            [CustomAttribute]
            public event EventHandler CustomEvent;

            [CustomAttribute]
            public string CustomProperty { get; set; }
        }

        private class InvalidSelectionStyleAxHost : AxHost
        {
            public InvalidSelectionStyleAxHost(string clsid) : base(clsid)
            {
            }

            public InvalidSelectionStyleAxHost(string clsid, int flags) : base(clsid, flags)
            {
            }

            public string SelectionStyle { get; set; }
        }

        private class ValidSelectionStyleAxHost : AxHost
        {
            public ValidSelectionStyleAxHost(string clsid) : base(clsid)
            {
            }

            public ValidSelectionStyleAxHost(string clsid, int flags) : base(clsid, flags)
            {
            }

            public int SelectionStyle { get; set; }
        }

        private class SubAxHost : AxHost
        {
            public SubAxHost(string clsid) : base(clsid)
            {
            }

            public SubAxHost(string clsid, int flags) : base(clsid, flags)
            {
            }

            public new bool CanEnableIme => base.CanEnableIme;

            public new bool CanRaiseEvents => base.CanRaiseEvents;

            public new CreateParams CreateParams => base.CreateParams;

            public new Cursor DefaultCursor => base.DefaultCursor;

            public new ImeMode DefaultImeMode => base.DefaultImeMode;

            public new Padding DefaultMargin => base.DefaultMargin;

            public new Size DefaultMaximumSize => base.DefaultMaximumSize;

            public new Size DefaultMinimumSize => base.DefaultMinimumSize;

            public new Padding DefaultPadding => base.DefaultPadding;

            public new Size DefaultSize => base.DefaultSize;

            public new bool DesignMode => base.DesignMode;

            public new bool DoubleBuffered
            {
                get => base.DoubleBuffered;
                set => base.DoubleBuffered = value;
            }

            public new EventHandlerList Events => base.Events;

            public new int FontHeight
            {
                get => base.FontHeight;
                set => base.FontHeight = value;
            }

            public new ImeMode ImeModeBase
            {
                get => base.ImeModeBase;
                set => base.ImeModeBase = value;
            }

            public new bool ResizeRedraw
            {
                get => base.ResizeRedraw;
                set => base.ResizeRedraw = value;
            }

            public new bool ShowFocusCues => base.ShowFocusCues;

            public new bool ShowKeyboardCues => base.ShowKeyboardCues;

            public new void AttachInterfaces() => base.AttachInterfaces();

            public new void CreateSink() => base.CreateSink();

            public new void DetachSink() => base.DetachSink();

            public static new Font GetFontFromIFont(object font) => AxHost.GetFontFromIFont(font);

            public static new object GetIFontDispFromFont(Font font) => AxHost.GetIFontDispFromFont(font);

            public static new object GetIFontFromFont(Font font) => AxHost.GetIFontFromFont(font);

            public static new object GetIPictureFromCursor(Cursor cursor) => AxHost.GetIPictureFromCursor(cursor);

            public static new object GetIPictureDispFromPicture(Image image) => AxHost.GetIPictureDispFromPicture(image);

            public static new object GetIPictureFromPicture(Image image) => AxHost.GetIPictureFromPicture(image);

            public static new Image GetPictureFromIPicture(object picture) => AxHost.GetPictureFromIPicture(picture);

            public static new Image GetPictureFromIPictureDisp(object picture) => AxHost.GetPictureFromIPictureDisp(picture);

            public new void OnEnter(EventArgs e) => base.OnEnter(e);

            public new void OnLeave(EventArgs e) => base.OnLeave(e);

            public new void OnMouseCaptureChanged(EventArgs e) => base.OnMouseCaptureChanged(e);
        }
    }
}
