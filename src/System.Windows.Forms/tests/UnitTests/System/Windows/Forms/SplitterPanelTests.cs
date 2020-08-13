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
    using Point = System.Drawing.Point;
    using Size = System.Drawing.Size;

    public class SplitterPanelTests : IClassFixture<ThreadExceptionFixture>
    {
        public static IEnumerable<object[]> Ctor_SplitContainer_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new SplitContainer() };
        }

        [WinFormsTheory]
        [MemberData(nameof(Ctor_SplitContainer_TestData))]
        public void SplitterPanel_Ctor_SplitContainer(SplitContainer owner)
        {
            using var control = new SplitterPanel(owner);
            Assert.Null(control.AccessibleDefaultActionDescription);
            Assert.Null(control.AccessibleDescription);
            Assert.Null(control.AccessibleName);
            Assert.Equal(AccessibleRole.Default, control.AccessibleRole);
            Assert.False(control.AllowDrop);
            Assert.Equal(AnchorStyles.Top | AnchorStyles.Left, control.Anchor);
            Assert.False(control.AutoScroll);
            Assert.Equal(Size.Empty, control.AutoScrollMargin);
            Assert.Equal(Size.Empty, control.AutoScrollMinSize);
            Assert.Equal(Point.Empty, control.AutoScrollPosition);
            Assert.False(control.AutoSize);
            Assert.Equal(AutoSizeMode.GrowOnly, control.AutoSizeMode);
            Assert.Equal(Control.DefaultBackColor, control.BackColor);
            Assert.Null(control.BackgroundImage);
            Assert.Equal(ImageLayout.Tile, control.BackgroundImageLayout);
            Assert.Null(control.BindingContext);
            Assert.Equal(BorderStyle.None, control.BorderStyle);
            Assert.Equal(100, control.Bottom);
            Assert.Equal(new Rectangle(0, 0, 200, 100), control.Bounds);
            Assert.False(control.CanFocus);
            Assert.False(control.CanSelect);
            Assert.False(control.Capture);
            Assert.True(control.CausesValidation);
            Assert.Equal(new Rectangle(0, 0, 200, 100), control.ClientRectangle);
            Assert.Equal(new Size(200, 100), control.ClientSize);
            Assert.Null(control.Container);
            Assert.False(control.ContainsFocus);
            Assert.Null(control.ContextMenuStrip);
            Assert.Empty(control.Controls);
            Assert.Same(control.Controls, control.Controls);
            Assert.False(control.Created);
            Assert.Equal(Cursors.Default, control.Cursor);
            Assert.Equal(new Rectangle(0, 0, 200, 100), control.DisplayRectangle);
            Assert.Equal(DockStyle.None, control.Dock);
            Assert.NotNull(control.DockPadding);
            Assert.Same(control.DockPadding, control.DockPadding);
            Assert.Equal(0, control.DockPadding.Top);
            Assert.Equal(0, control.DockPadding.Bottom);
            Assert.Equal(0, control.DockPadding.Left);
            Assert.Equal(0, control.DockPadding.Right);
            Assert.True(control.Enabled);
            Assert.False(control.Focused);
            Assert.Equal(Control.DefaultFont, control.Font);
            Assert.Equal(Control.DefaultForeColor, control.ForeColor);
            Assert.False(control.HasChildren);
            Assert.Equal(100, control.Height);
            Assert.NotNull(control.HorizontalScroll);
            Assert.Same(control.HorizontalScroll, control.HorizontalScroll);
            Assert.Equal(ImeMode.NoControl, control.ImeMode);
            Assert.False(control.IsAccessible);
            Assert.False(control.IsMirrored);
            Assert.NotNull(control.LayoutEngine);
            Assert.Same(control.LayoutEngine, control.LayoutEngine);
            Assert.Equal(0, control.Left);
            Assert.Equal(Point.Empty, control.Location);
            Assert.Equal(Padding.Empty, control.Margin);
            Assert.Equal(Size.Empty, control.MaximumSize);
            Assert.Equal(Size.Empty, control.MinimumSize);
            Assert.Equal(Padding.Empty, control.Padding);
            Assert.Null(control.Parent);
            Assert.Equal(Size.Empty, control.PreferredSize);
            Assert.Equal("Microsoft\u00AE .NET", control.ProductName);
            Assert.False(control.RecreatingHandle);
            Assert.Null(control.Region);
            Assert.Equal(200, control.Right);
            Assert.Equal(RightToLeft.No, control.RightToLeft);
            Assert.Equal(new Size(200, 100), control.Size);
            Assert.Equal(0, control.TabIndex);
            Assert.False(control.TabStop);
            Assert.Empty(control.Text);
            Assert.Equal(0, control.Top);
            Assert.Null(control.TopLevelControl);
            Assert.False(control.UseWaitCursor);
            Assert.True(control.Visible);
            Assert.NotNull(control.VerticalScroll);
            Assert.Same(control.VerticalScroll, control.VerticalScroll);
            Assert.Equal(200, control.Width);

            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(ControlTests.Anchor_Set_TestData), MemberType = typeof(ControlTests))]
        public void SplitterPanel_Anchor_Set_GetReturnsExpected(AnchorStyles value, AnchorStyles expected)
        {
            using var control = new SplitterPanel(null)
            {
                Anchor = value
            };
            Assert.Equal(expected, control.Anchor);
            Assert.Equal(DockStyle.None, control.Dock);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.Anchor = value;
            Assert.Equal(expected, control.Anchor);
            Assert.Equal(DockStyle.None, control.Dock);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void SplitterPanel_AutoSize_Set_GetReturnsExpected(bool value)
        {
            using var control = new SplitterPanel(null);
            int layoutCallCount = 0;
            control.Layout += (sender, e) => layoutCallCount++;

            control.AutoSize = value;
            Assert.Equal(value, control.AutoSize);
            Assert.Equal(0, layoutCallCount);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.AutoSize = value;
            Assert.Equal(value, control.AutoSize);
            Assert.Equal(0, layoutCallCount);
            Assert.False(control.IsHandleCreated);

            // Set different.
            control.AutoSize = !value;
            Assert.Equal(!value, control.AutoSize);
            Assert.Equal(0, layoutCallCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void SplitterPanel_AutoSize_SetWithHandler_CallsAutoSizeChanged()
        {
            using var control = new SplitterPanel(null)
            {
                AutoSize = true
            };
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            control.AutoSizeChanged += handler;

            // Set different.
            control.AutoSize = false;
            Assert.False(control.AutoSize);
            Assert.Equal(1, callCount);

            // Set same.
            control.AutoSize = false;
            Assert.False(control.AutoSize);
            Assert.Equal(1, callCount);

            // Set different.
            control.AutoSize = true;
            Assert.True(control.AutoSize);
            Assert.Equal(2, callCount);

            // Remove handler.
            control.AutoSizeChanged -= handler;
            control.AutoSize = false;
            Assert.False(control.AutoSize);
            Assert.Equal(2, callCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(AutoSizeMode))]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(AutoSizeMode))]
        public void SplitterPanel_AutoSizeMode_Set_GetReturnsExpected(AutoSizeMode value)
        {
            using var control = new SplitterPanel(null);
            int layoutCallCount = 0;
            control.Layout += (sender, e) => layoutCallCount++;

            control.AutoSizeMode = value;
            Assert.Equal(AutoSizeMode.GrowOnly, control.AutoSizeMode);
            Assert.False(control.IsHandleCreated);
            Assert.Equal(0, layoutCallCount);

            // Set same.
            control.AutoSizeMode = value;
            Assert.Equal(AutoSizeMode.GrowOnly, control.AutoSizeMode);
            Assert.False(control.IsHandleCreated);
            Assert.Equal(0, layoutCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(AutoSizeMode))]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(AutoSizeMode))]
        public void SplitterPanel_AutoSizeMode_SetWithParent_GetReturnsExpected(AutoSizeMode value)
        {
            using var parent = new Control();
            using var control = new SplitterPanel(null)
            {
                Parent = parent
            };
            int layoutCallCount = 0;
            control.Layout += (sender, e) => layoutCallCount++;
            int parentLayoutCallCount = 0;
            parent.Layout += (sender, e) => parentLayoutCallCount++;

            control.AutoSizeMode = value;
            Assert.Equal(AutoSizeMode.GrowOnly, control.AutoSizeMode);
            Assert.False(control.IsHandleCreated);
            Assert.Equal(0, layoutCallCount);
            Assert.False(parent.IsHandleCreated);
            Assert.Equal(0, parentLayoutCallCount);

            // Set same.
            control.AutoSizeMode = value;
            Assert.Equal(AutoSizeMode.GrowOnly, control.AutoSizeMode);
            Assert.False(control.IsHandleCreated);
            Assert.Equal(0, layoutCallCount);
            Assert.False(parent.IsHandleCreated);
            Assert.Equal(0, parentLayoutCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(AutoSizeMode))]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(AutoSizeMode))]
        public void SplitterPanel_AutoSizeMode_SetWithHandle_GetReturnsExpected(AutoSizeMode value)
        {
            using var control = new SplitterPanel(null);
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.AutoSizeMode = value;
            Assert.Equal(AutoSizeMode.GrowOnly, control.AutoSizeMode);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.AutoSizeMode = value;
            Assert.Equal(AutoSizeMode.GrowOnly, control.AutoSizeMode);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(AutoSizeMode))]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(AutoSizeMode))]
        public void SplitterPanel_AutoSizeMode_SetWithHandleWithParent_GetReturnsExpected(AutoSizeMode value)
        {
            using var parent = new Control();
            using var control = new SplitterPanel(null)
            {
                Parent = parent
            };
            Assert.NotEqual(IntPtr.Zero, parent.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;
            int layoutCallCount = 0;
            control.Layout += (sender, e) => layoutCallCount++;
            int parentInvalidatedCallCount = 0;
            parent.Invalidated += (sender, e) => parentInvalidatedCallCount++;
            int parentStyleChangedCallCount = 0;
            parent.StyleChanged += (sender, e) => parentStyleChangedCallCount++;
            int parentCreatedCallCount = 0;
            parent.HandleCreated += (sender, e) => parentCreatedCallCount++;
            int parentLayoutCallCount = 0;
            parent.Layout += (sender, e) => parentLayoutCallCount++;

            control.AutoSizeMode = value;
            Assert.Equal(AutoSizeMode.GrowOnly, control.AutoSizeMode);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.Equal(0, layoutCallCount);
            Assert.True(parent.IsHandleCreated);
            Assert.Equal(0, parentInvalidatedCallCount);
            Assert.Equal(0, parentStyleChangedCallCount);
            Assert.Equal(0, parentCreatedCallCount);
            Assert.Equal(0, parentLayoutCallCount);

            // Set same.
            control.AutoSizeMode = value;
            Assert.Equal(AutoSizeMode.GrowOnly, control.AutoSizeMode);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.Equal(0, layoutCallCount);
            Assert.True(parent.IsHandleCreated);
            Assert.Equal(0, parentInvalidatedCallCount);
            Assert.Equal(0, parentStyleChangedCallCount);
            Assert.Equal(0, parentCreatedCallCount);
            Assert.Equal(0, parentLayoutCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(BorderStyle))]
        public void SplitterPanel_BorderStyle_Set_GetReturnsExpected(BorderStyle value)
        {
            using var control = new SplitterPanel(null)
            {
                BorderStyle = value
            };
            Assert.Equal(value, control.BorderStyle);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.BorderStyle = value;
            Assert.Equal(value, control.BorderStyle);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(BorderStyle.Fixed3D, 1)]
        [InlineData(BorderStyle.FixedSingle, 1)]
        [InlineData(BorderStyle.None, 0)]
        public void SplitterPanel_BorderStyle_SetWithHandle_GetReturnsExpected(BorderStyle value, int expectedInvalidatedCallCount)
        {
            using var control = new SplitterPanel(null);
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.BorderStyle = value;
            Assert.Equal(value, control.BorderStyle);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount * 2, invalidatedCallCount);
            Assert.Equal(expectedInvalidatedCallCount, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.BorderStyle = value;
            Assert.Equal(value, control.BorderStyle);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount * 2, invalidatedCallCount);
            Assert.Equal(expectedInvalidatedCallCount, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(BorderStyle))]
        public void SplitterPanel_BorderStyle_SetInvalid_ThrowsInvalidEnumArgumentException(BorderStyle value)
        {
            using var control = new SplitterPanel(null);
            Assert.Throws<InvalidEnumArgumentException>("value", () => control.BorderStyle = value);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(DockStyle))]
        public void SplitterPanel_Dock_Set_GetReturnsExpected(DockStyle value)
        {
            using var control = new SplitterPanel(null)
            {
                Dock = value
            };
            Assert.Equal(value, control.Dock);
            Assert.Equal(AnchorStyles.Top | AnchorStyles.Left, control.Anchor);

            // Set same.
            control.Dock = value;
            Assert.Equal(value, control.Dock);
            Assert.Equal(AnchorStyles.Top | AnchorStyles.Left, control.Anchor);
        }

        [WinFormsFact]
        public void SplitterPanel_Dock_SetWithHandler_CallsDockChanged()
        {
            using var control = new SplitterPanel(null)
            {
                Dock = DockStyle.None
            };
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            control.DockChanged += handler;

            // Set different.
            control.Dock = DockStyle.Top;
            Assert.Equal(DockStyle.Top, control.Dock);
            Assert.Equal(1, callCount);

            // Set same.
            control.Dock = DockStyle.Top;
            Assert.Equal(DockStyle.Top, control.Dock);
            Assert.Equal(1, callCount);

            // Set different.
            control.Dock = DockStyle.Left;
            Assert.Equal(DockStyle.Left, control.Dock);
            Assert.Equal(2, callCount);

            // Remove handler.
            control.DockChanged -= handler;
            control.Dock = DockStyle.Top;
            Assert.Equal(DockStyle.Top, control.Dock);
            Assert.Equal(2, callCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(DockStyle))]
        public void SplitterPanel_Dock_SetInvalid_ThrowsInvalidEnumArgumentException(DockStyle value)
        {
            using var control = new SplitterPanel(null);
            Assert.Throws<InvalidEnumArgumentException>("value", () => control.Dock = value);
        }

        [WinFormsTheory]
        [InlineData(-4)]
        [InlineData(0)]
        [InlineData(2)]
        [InlineData(40)]
        public void SplitterPanel_Height_Set_ThrowsNotSupportedException(int value)
        {
            using var control = new SplitterPanel(null);
            int layoutCallCount = 0;
            int resizeCallCount = 0;
            int sizeChangedCallCount = 0;
            int clientSizeChangedCallCount = 0;
            control.Layout += (sender, e) => layoutCallCount++;
            control.Resize += (sender, e) => resizeCallCount++;
            control.SizeChanged += (sender, e) => sizeChangedCallCount++;
            control.ClientSizeChanged += (sender, e) => clientSizeChangedCallCount++;

            Assert.Throws<NotSupportedException>(() => control.Height = value);
            Assert.Equal(new Size(200, 100), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, 200, 100), control.ClientRectangle);
            Assert.Equal(new Rectangle(0, 0, 200, 100), control.DisplayRectangle);
            Assert.Equal(new Size(200, 100), control.Size);
            Assert.Equal(0, control.Left);
            Assert.Equal(200, control.Right);
            Assert.Equal(0, control.Top);
            Assert.Equal(100, control.Bottom);
            Assert.Equal(200, control.Width);
            Assert.Equal(100, control.Height);
            Assert.Equal(new Rectangle(0, 0, 200, 100), control.Bounds);
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(0, resizeCallCount);
            Assert.Equal(0, sizeChangedCallCount);
            Assert.Equal(0, clientSizeChangedCallCount);
            Assert.False(control.IsHandleCreated);

            // Call again.
            Assert.Throws<NotSupportedException>(() => control.Height = value);
            Assert.Equal(new Size(200, 100), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, 200, 100), control.ClientRectangle);
            Assert.Equal(new Rectangle(0, 0, 200, 100), control.DisplayRectangle);
            Assert.Equal(new Size(200, 100), control.Size);
            Assert.Equal(0, control.Left);
            Assert.Equal(200, control.Right);
            Assert.Equal(0, control.Top);
            Assert.Equal(100, control.Bottom);
            Assert.Equal(200, control.Width);
            Assert.Equal(100, control.Height);
            Assert.Equal(new Rectangle(0, 0, 200, 100), control.Bounds);
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(0, resizeCallCount);
            Assert.Equal(0, sizeChangedCallCount);
            Assert.Equal(0, clientSizeChangedCallCount);
            Assert.False(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> Location_Set_TestData()
        {
            yield return new object[] { new Point(0, 0), 0 };
            yield return new object[] { new Point(-1, -2), 1 };
            yield return new object[] { new Point(1, 0), 1 };
            yield return new object[] { new Point(0, 2), 1 };
            yield return new object[] { new Point(1, 2), 1 };
        }

        [WinFormsTheory]
        [MemberData(nameof(Location_Set_TestData))]
        public void SplitterPanel_Location_Set_GetReturnsExpected(Point value, int expectedLocationChangedCallCount)
        {
            using var control = new SplitterPanel(null);
            int moveCallCount = 0;
            int locationChangedCallCount = 0;
            int layoutCallCount = 0;
            int resizeCallCount = 0;
            int sizeChangedCallCount = 0;
            int clientSizeChangedCallCount = 0;
            control.Move += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                Assert.Equal(locationChangedCallCount, moveCallCount);
                moveCallCount++;
            };
            control.LocationChanged += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                Assert.Equal(moveCallCount - 1, locationChangedCallCount);
                locationChangedCallCount++;
            };
            control.Layout += (sender, e) => layoutCallCount++;
            control.Resize += (sender, e) => resizeCallCount++;
            control.SizeChanged += (sender, e) => sizeChangedCallCount++;
            control.ClientSizeChanged += (sender, e) => clientSizeChangedCallCount++;

            control.Location = value;
            Assert.Equal(new Size(200, 100), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, 200, 100), control.ClientRectangle);
            Assert.Equal(new Rectangle(0, 0, 200, 100), control.DisplayRectangle);
            Assert.Equal(new Size(200, 100), control.Size);
            Assert.Equal(value.X, control.Left);
            Assert.Equal(value.X + 200, control.Right);
            Assert.Equal(value, control.Location);
            Assert.Equal(value.Y, control.Top);
            Assert.Equal(value.Y + 100, control.Bottom);
            Assert.Equal(200, control.Width);
            Assert.Equal(100, control.Height);
            Assert.Equal(new Rectangle(value.X, value.Y, 200, 100), control.Bounds);
            Assert.Equal(expectedLocationChangedCallCount, moveCallCount);
            Assert.Equal(expectedLocationChangedCallCount, locationChangedCallCount);
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(0, resizeCallCount);
            Assert.Equal(0, sizeChangedCallCount);
            Assert.Equal(0, clientSizeChangedCallCount);
            Assert.False(control.IsHandleCreated);

            // Call again.
            control.Location = value;
            Assert.Equal(new Size(200, 100), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, 200, 100), control.ClientRectangle);
            Assert.Equal(new Rectangle(0, 0, 200, 100), control.DisplayRectangle);
            Assert.Equal(new Size(200, 100), control.Size);
            Assert.Equal(value.X, control.Left);
            Assert.Equal(value.X + 200, control.Right);
            Assert.Equal(value, control.Location);
            Assert.Equal(value.Y, control.Top);
            Assert.Equal(value.Y + 100, control.Bottom);
            Assert.Equal(200, control.Width);
            Assert.Equal(100, control.Height);
            Assert.Equal(new Rectangle(value.X, value.Y, 200, 100), control.Bounds);
            Assert.Equal(expectedLocationChangedCallCount, moveCallCount);
            Assert.Equal(expectedLocationChangedCallCount, locationChangedCallCount);
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(0, resizeCallCount);
            Assert.Equal(0, sizeChangedCallCount);
            Assert.Equal(0, clientSizeChangedCallCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void SplitterPanel_Location_SetWithHandler_CallsLocationChanged()
        {
            using var control = new SplitterPanel(null);
            int locationChangedCallCount = 0;
            EventHandler locationChangedHandler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                locationChangedCallCount++;
            };
            control.LocationChanged += locationChangedHandler;
            int moveCallCount = 0;
            EventHandler moveHandler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                moveCallCount++;
            };
            control.Move += moveHandler;

            // Set different.
            control.Location = new Point(1, 2);
            Assert.Equal(new Point(1, 2), control.Location);
            Assert.Equal(1, locationChangedCallCount);
            Assert.Equal(1, moveCallCount);

            // Set same.
            control.Location = new Point(1, 2);
            Assert.Equal(new Point(1, 2), control.Location);
            Assert.Equal(1, locationChangedCallCount);
            Assert.Equal(1, moveCallCount);

            // Set different x.
            control.Location = new Point(2, 2);
            Assert.Equal(new Point(2, 2), control.Location);
            Assert.Equal(2, locationChangedCallCount);
            Assert.Equal(2, moveCallCount);

            // Set different y.
            control.Location = new Point(2, 3);
            Assert.Equal(new Point(2, 3), control.Location);
            Assert.Equal(3, locationChangedCallCount);
            Assert.Equal(3, moveCallCount);

            // Remove handler.
            control.LocationChanged -= locationChangedHandler;
            control.Move -= moveHandler;
            control.Location = new Point(1, 2);
            Assert.Equal(new Point(1, 2), control.Location);
            Assert.Equal(3, locationChangedCallCount);
            Assert.Equal(3, moveCallCount);
        }

        [WinFormsTheory]
        [MemberData(nameof(ControlTests.MaximumSize_Set_TestData), MemberType = typeof(ControlTests))]
        public void SplitterPanel_MaximumSize_Set_GetReturnsExpected(Size value)
        {
            using var control = new SplitterPanel(null)
            {
                Size = Size.Empty
            };
            int layoutCallCount = 0;
            control.Layout += (sender, e) => layoutCallCount++;

            control.MaximumSize = value;
            Assert.Equal(value, control.MaximumSize);
            Assert.Equal(Size.Empty, control.Size);
            Assert.Equal(0, layoutCallCount);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.MaximumSize = value;
            Assert.Equal(value, control.MaximumSize);
            Assert.Equal(Size.Empty, control.Size);
            Assert.Equal(0, layoutCallCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(ControlTests.MinimumSize_Set_TestData), MemberType = typeof(ControlTests))]
        public void SplitterPanel_MinimumSize_Set_GetReturnsExpected(Size value, Size expectedSize, int expectedLayoutCallCount)
        {
            using var control = new SplitterPanel(null)
            {
                Size = Size.Empty
            };
            int layoutCallCount = 0;
            control.Layout += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(control, e.AffectedControl);
                Assert.Equal("Bounds", e.AffectedProperty);
                layoutCallCount++;
            };

            control.MinimumSize = value;
            Assert.Equal(value, control.MinimumSize);
            Assert.Equal(expectedSize, control.Size);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.MinimumSize = value;
            Assert.Equal(value, control.MinimumSize);
            Assert.Equal(expectedSize, control.Size);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void SplitterPanel_Name_Set_GetReturnsExpected(string value, string expected)
        {
            using var control = new SplitterPanel(null)
            {
                Name = value
            };
            Assert.Equal(expected, control.Name);
            Assert.False(control.IsHandleCreated);

            // Get again.
            control.Name = value;
            Assert.Equal(expected, control.Name);
            Assert.False(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> Parent_Set_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new Control() };
            yield return new object[] { new Form() };
        }

        [WinFormsTheory]
        [MemberData(nameof(Parent_Set_TestData))]
        public void SplitterPanel_Parent_Set_GetReturnsExpected(Control value)
        {
            using var control = new SplitterPanel(null)
            {
                Parent = value
            };
            Assert.Same(value, control.Parent);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.Parent = value;
            Assert.Same(value, control.Parent);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void SplitterPanel_Parent_SetWithHandler_CallsParentChanged()
        {
            using var parent = new Control();
            using var control = new SplitterPanel(null);
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            control.ParentChanged += handler;

            // Set different.
            control.Parent = parent;
            Assert.Same(parent, control.Parent);
            Assert.Equal(1, callCount);

            // Set same.
            control.Parent = parent;
            Assert.Same(parent, control.Parent);
            Assert.Equal(1, callCount);

            // Set null.
            control.Parent = null;
            Assert.Null(control.Parent);
            Assert.Equal(2, callCount);

            // Remove handler.
            control.ParentChanged -= handler;
            control.Parent = parent;
            Assert.Same(parent, control.Parent);
            Assert.Equal(2, callCount);
        }

        public static IEnumerable<object[]> Size_Set_TestData()
        {
            yield return new object[] { new Size(-3, -4), 1 };
            yield return new object[] { new Size(0, 0), 1 };
            yield return new object[] { new Size(1, 0), 1 };
            yield return new object[] { new Size(0, 2), 1 };
            yield return new object[] { new Size(30, 40), 1 };
            yield return new object[] { new Size(200, 100), 0 };
        }

        [WinFormsTheory]
        [MemberData(nameof(Size_Set_TestData))]
        public void SplitterPanel_Size_Set_GetReturnsExpected(Size value, int expectedLayoutCallCount)
        {
            using var control = new SplitterPanel(null);
            int layoutCallCount = 0;
            int resizeCallCount = 0;
            int sizeChangedCallCount = 0;
            int clientSizeChangedCallCount = 0;
            control.Layout += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(control, e.AffectedControl);
                Assert.Equal("Bounds", e.AffectedProperty);
                Assert.Equal(resizeCallCount, layoutCallCount);
                Assert.Equal(sizeChangedCallCount, layoutCallCount);
                Assert.Equal(clientSizeChangedCallCount, layoutCallCount);
                layoutCallCount++;
            };
            control.Resize += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                Assert.Equal(layoutCallCount - 1, resizeCallCount);
                Assert.Equal(sizeChangedCallCount, resizeCallCount);
                Assert.Equal(clientSizeChangedCallCount, resizeCallCount);
                resizeCallCount++;
            };
            control.SizeChanged += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                Assert.Equal(resizeCallCount - 1, sizeChangedCallCount);
                Assert.Equal(layoutCallCount - 1, sizeChangedCallCount);
                Assert.Equal(clientSizeChangedCallCount, sizeChangedCallCount);
                sizeChangedCallCount++;
            };
            control.ClientSizeChanged += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                Assert.Equal(resizeCallCount - 1, clientSizeChangedCallCount);
                Assert.Equal(layoutCallCount - 1, clientSizeChangedCallCount);
                Assert.Equal(sizeChangedCallCount - 1, clientSizeChangedCallCount);
                clientSizeChangedCallCount++;
            };

            control.Size = value;
            Assert.Equal(value, control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, value.Width, value.Height), control.ClientRectangle);
            Assert.Equal(new Rectangle(0, 0, value.Width, value.Height), control.DisplayRectangle);
            Assert.Equal(value, control.Size);
            Assert.Equal(0, control.Left);
            Assert.Equal(value.Width, control.Right);
            Assert.Equal(0, control.Top);
            Assert.Equal(value.Height, control.Bottom);
            Assert.Equal(value.Width, control.Width);
            Assert.Equal(value.Height, control.Height);
            Assert.Equal(new Rectangle(0, 0, value.Width, value.Height), control.Bounds);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.Equal(expectedLayoutCallCount, resizeCallCount);
            Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
            Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
            Assert.False(control.IsHandleCreated);

            // Call again.
            control.Size = value;
            Assert.Equal(value, control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, value.Width, value.Height), control.ClientRectangle);
            Assert.Equal(new Rectangle(0, 0, value.Width, value.Height), control.DisplayRectangle);
            Assert.Equal(value, control.Size);
            Assert.Equal(0, control.Left);
            Assert.Equal(value.Width, control.Right);
            Assert.Equal(0, control.Top);
            Assert.Equal(value.Height, control.Bottom);
            Assert.Equal(value.Width, control.Width);
            Assert.Equal(value.Height, control.Height);
            Assert.Equal(new Rectangle(0, 0, value.Width, value.Height), control.Bounds);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.Equal(expectedLayoutCallCount, resizeCallCount);
            Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
            Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        public void SplitterPanel_TabIndex_Set_GetReturnsExpected(int value)
        {
            using var control = new SplitterPanel(null)
            {
                TabIndex = value
            };
            Assert.Equal(value, control.TabIndex);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.TabIndex = value;
            Assert.Equal(value, control.TabIndex);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void SplitterPanel_TabIndex_SetWithHandler_CallsTabIndexChanged()
        {
            using var control = new SplitterPanel(null)
            {
                TabIndex = 0
            };
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            control.TabIndexChanged += handler;

            // Set different.
            control.TabIndex = 1;
            Assert.Equal(1, control.TabIndex);
            Assert.Equal(1, callCount);

            // Set same.
            control.TabIndex = 1;
            Assert.Equal(1, control.TabIndex);
            Assert.Equal(1, callCount);

            // Set different.
            control.TabIndex = 2;
            Assert.Equal(2, control.TabIndex);
            Assert.Equal(2, callCount);

            // Remove handler.
            control.TabIndexChanged -= handler;
            control.TabIndex = 1;
            Assert.Equal(1, control.TabIndex);
            Assert.Equal(2, callCount);
        }

        [WinFormsFact]
        public void SplitterPanel_TabIndex_SetNegative_CallsArgumentOutOfRangeException()
        {
            using var control = new SplitterPanel(null);
            Assert.Throws<ArgumentOutOfRangeException>("value", () => control.TabIndex = -1);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void SplitterPanel_TabStop_Set_GetReturnsExpected(bool value)
        {
            using var control = new SplitterPanel(null)
            {
                TabStop = value
            };
            Assert.Equal(value, control.TabStop);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.TabStop = value;
            Assert.Equal(value, control.TabStop);
            Assert.False(control.IsHandleCreated);

            // Set different.
            control.TabStop = value;
            Assert.Equal(value, control.TabStop);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void SplitterPanel_TabStop_SetWithHandle_GetReturnsExpected(bool value)
        {
            using var control = new SplitterPanel(null);
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.TabStop = value;
            Assert.Equal(value, control.TabStop);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.TabStop = value;
            Assert.Equal(value, control.TabStop);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set different.
            control.TabStop = value;
            Assert.Equal(value, control.TabStop);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void SplitterPanel_TabStop_SetWithHandler_CallsTabStopChanged()
        {
            using var control = new SplitterPanel(null)
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
            control.TabStop = false;
            Assert.False(control.TabStop);
            Assert.Equal(1, callCount);

            // Set same.
            control.TabStop = false;
            Assert.False(control.TabStop);
            Assert.Equal(1, callCount);

            // Set different.
            control.TabStop = true;
            Assert.True(control.TabStop);
            Assert.Equal(2, callCount);

            // Remove handler.
            control.TabStopChanged -= handler;
            control.TabStop = false;
            Assert.False(control.TabStop);
            Assert.Equal(2, callCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void SplitterPanel_Text_Set_GetReturnsExpected(string value, string expected)
        {
            using var control = new SplitterPanel(null)
            {
                Text = value
            };
            Assert.Equal(expected, control.Text);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.Text = value;
            Assert.Equal(expected, control.Text);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void SplitterPanel_Text_SetWithHandle_GetReturnsExpected(string value, string expected)
        {
            using var control = new SplitterPanel(null);
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.Text = value;
            Assert.Equal(expected, control.Text);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.Text = value;
            Assert.Equal(expected, control.Text);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void SplitterPanel_Text_SetWithHandler_CallsTextChanged()
        {
            using var control = new SplitterPanel(null);
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Equal(EventArgs.Empty, e);
                callCount++;
            };
            control.TextChanged += handler;

            // Set different.
            control.Text = "text";
            Assert.Equal("text", control.Text);
            Assert.Equal(1, callCount);

            // Set same.
            control.Text = "text";
            Assert.Equal("text", control.Text);
            Assert.Equal(1, callCount);

            // Set different.
            control.Text = null;
            Assert.Empty(control.Text);
            Assert.Equal(2, callCount);

            // Remove handler.
            control.TextChanged -= handler;
            control.Text = "text";
            Assert.Equal("text", control.Text);
            Assert.Equal(2, callCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void SplitterPanel_Visible_Set_GetReturnsExpected(bool value)
        {
            using var control = new SplitterPanel(null)
            {
                Visible = value
            };
            Assert.Equal(value, control.Visible);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.Visible = value;
            Assert.Equal(value, control.Visible);
            Assert.False(control.IsHandleCreated);

            // Set different.
            control.Visible = !value;
            Assert.Equal(!value, control.Visible);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void SplitterPanel_Visible_SetWithHandler_CallsVisibleChanged()
        {
            using var control = new SplitterPanel(null)
            {
                Visible = true
            };
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            control.VisibleChanged += handler;

            // Set different.
            control.Visible = false;
            Assert.False(control.Visible);
            Assert.Equal(1, callCount);

            // Set same.
            control.Visible = false;
            Assert.False(control.Visible);
            Assert.Equal(1, callCount);

            // Set different.
            control.Visible = true;
            Assert.True(control.Visible);
            Assert.Equal(2, callCount);

            // Remove handler.
            control.VisibleChanged -= handler;
            control.Visible = false;
            Assert.False(control.Visible);
            Assert.Equal(2, callCount);
        }

        [WinFormsTheory]
        [InlineData(-4)]
        [InlineData(0)]
        [InlineData(2)]
        [InlineData(40)]
        public void SplitterPanel_Width_Set_ThrowsNotSupportedException(int value)
        {
            using var control = new SplitterPanel(null);
            int layoutCallCount = 0;
            int resizeCallCount = 0;
            int sizeChangedCallCount = 0;
            int clientSizeChangedCallCount = 0;
            control.Layout += (sender, e) => layoutCallCount++;
            control.Resize += (sender, e) => resizeCallCount++;
            control.SizeChanged += (sender, e) => sizeChangedCallCount++;
            control.ClientSizeChanged += (sender, e) => clientSizeChangedCallCount++;

            Assert.Throws<NotSupportedException>(() => control.Width = value);
            Assert.Equal(new Size(200, 100), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, 200, 100), control.ClientRectangle);
            Assert.Equal(new Rectangle(0, 0, 200, 100), control.DisplayRectangle);
            Assert.Equal(new Size(200, 100), control.Size);
            Assert.Equal(0, control.Left);
            Assert.Equal(200, control.Right);
            Assert.Equal(0, control.Top);
            Assert.Equal(100, control.Bottom);
            Assert.Equal(200, control.Width);
            Assert.Equal(100, control.Height);
            Assert.Equal(new Rectangle(0, 0, 200, 100), control.Bounds);
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(0, resizeCallCount);
            Assert.Equal(0, sizeChangedCallCount);
            Assert.Equal(0, clientSizeChangedCallCount);
            Assert.False(control.IsHandleCreated);

            // Call again.
            Assert.Throws<NotSupportedException>(() => control.Width = value);
            Assert.Equal(new Size(200, 100), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, 200, 100), control.ClientRectangle);
            Assert.Equal(new Rectangle(0, 0, 200, 100), control.DisplayRectangle);
            Assert.Equal(new Size(200, 100), control.Size);
            Assert.Equal(0, control.Left);
            Assert.Equal(200, control.Right);
            Assert.Equal(0, control.Top);
            Assert.Equal(100, control.Bottom);
            Assert.Equal(200, control.Width);
            Assert.Equal(100, control.Height);
            Assert.Equal(new Rectangle(0, 0, 200, 100), control.Bounds);
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(0, resizeCallCount);
            Assert.Equal(0, sizeChangedCallCount);
            Assert.Equal(0, clientSizeChangedCallCount);
            Assert.False(control.IsHandleCreated);
        }
    }
}
