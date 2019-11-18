﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using Moq;
using WinForms.Common.Tests;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public partial class ControlTests
    {
        [Fact]
        public void Control_AccessibleNameGetSet()
        {
            var cont = new Control
            {
                AccessibleName = "Foo"
            };

            Assert.Equal("Foo", cont.AccessibleName);
        }

        /// <summary>
        ///  Data for the AccessibleRole test
        /// </summary>
        public static TheoryData<AccessibleRole> AccessibleRoleData =>
            CommonTestHelper.GetEnumTheoryData<AccessibleRole>();

        [Theory]
        [MemberData(nameof(AccessibleRoleData))]
        public void Control_SetItemCheckState(AccessibleRole expected)
        {
            var cont = new Control
            {
                AccessibleRole = expected
            };

            Assert.Equal(expected, cont.AccessibleRole);
        }

        /// <summary>
        ///  Data for the AccessibleRoleInvalid test
        /// </summary>
        public static TheoryData<CheckState> AccessibleRoleInvalidData =>
            CommonTestHelper.GetEnumTheoryDataInvalid<CheckState>();

        [Theory]
        [MemberData(nameof(AccessibleRoleInvalidData))]
        public void Control_AccessibleRoleInvalid(AccessibleRole expected)
        {
            var cont = new Control();

            InvalidEnumArgumentException ex = Assert.Throws<InvalidEnumArgumentException>(() => cont.AccessibleRole = expected);
            Assert.Equal("value", ex.ParamName);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void Control_AllowDrop_Set_GetReturnsExpected(bool value)
        {
            var control = new Control
            {
                AllowDrop = value
            };
            Assert.Equal(value, control.AllowDrop);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.AllowDrop = value;
            Assert.Equal(value, control.AllowDrop);
            Assert.False(control.IsHandleCreated);

            // Set different.
            control.AllowDrop = value;
            Assert.Equal(value, control.AllowDrop);
            Assert.False(control.IsHandleCreated);
        }

        [Fact]
        public void Control_AllowDrop_SetWithHandleNonSTAThread_ThrowsInvalidOperationException()
        {
            using var control = new Control();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            Assert.Throws<InvalidOperationException>(() => control.AllowDrop = true);
            Assert.False(control.AllowDrop);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Can set to false.
            control.AllowDrop = false;
            Assert.False(control.AllowDrop);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void Control_AllowDrop_SetWithHandleSTA_GetReturnsExpected(bool value)
        {
            using var control = new Control();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.AllowDrop = value;
            Assert.Equal(value, control.AllowDrop);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.AllowDrop = value;
            Assert.Equal(value, control.AllowDrop);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set different.
            control.AllowDrop = value;
            Assert.Equal(value, control.AllowDrop);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        public static IEnumerable<object[]> Anchor_Set_TestData()
        {
            yield return new object[] { AnchorStyles.Top, AnchorStyles.Top };
            yield return new object[] { AnchorStyles.Top | AnchorStyles.Bottom, AnchorStyles.Top | AnchorStyles.Bottom };
            yield return new object[] { AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left, AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left };
            yield return new object[] { AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right, AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right };
            yield return new object[] { AnchorStyles.Top | AnchorStyles.Left, AnchorStyles.Top | AnchorStyles.Left };
            yield return new object[] { AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right, AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
            yield return new object[] { AnchorStyles.Top | AnchorStyles.Right, AnchorStyles.Top | AnchorStyles.Right };
            yield return new object[] { AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right, AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right };

            yield return new object[] { AnchorStyles.Bottom, AnchorStyles.Bottom };
            yield return new object[] { AnchorStyles.Bottom | AnchorStyles.Left, AnchorStyles.Bottom | AnchorStyles.Left };
            yield return new object[] { AnchorStyles.Bottom | AnchorStyles.Right, AnchorStyles.Bottom | AnchorStyles.Right };
            yield return new object[] { AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right, AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right };

            yield return new object[] { AnchorStyles.Left, AnchorStyles.Left };
            yield return new object[] { AnchorStyles.Left | AnchorStyles.Right, AnchorStyles.Left | AnchorStyles.Right };

            yield return new object[] { AnchorStyles.Right, AnchorStyles.Right };

            yield return new object[] { AnchorStyles.None, AnchorStyles.None };
            yield return new object[] { (AnchorStyles)(-1), AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right };
            yield return new object[] { (AnchorStyles)int.MaxValue, AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right };
        }

        [Theory]
        [MemberData(nameof(Anchor_Set_TestData))]
        public void Control_Anchor_Set_GetReturnsExpected(AnchorStyles value, AnchorStyles expected)
        {
            var control = new Control
            {
                Anchor = value
            };
            Assert.Equal(expected, control.Anchor);
            Assert.Equal(DockStyle.None, control.Dock);

            // Set same.
            control.Anchor = value;
            Assert.Equal(expected, control.Anchor);
            Assert.Equal(DockStyle.None, control.Dock);
        }

        [Theory]
        [MemberData(nameof(Anchor_Set_TestData))]
        public void Control_Anchor_SetWithOldValue_GetReturnsExpected(AnchorStyles value, AnchorStyles expected)
        {
            var control = new Control
            {
                Anchor = AnchorStyles.Left
            };

            control.Anchor = value;
            Assert.Equal(expected, control.Anchor);
            Assert.Equal(DockStyle.None, control.Dock);

            // Set same.
            control.Anchor = value;
            Assert.Equal(expected, control.Anchor);
            Assert.Equal(DockStyle.None, control.Dock);
        }

        public static IEnumerable<object[]> Anchor_SetWithDock_TestData()
        {
            foreach (DockStyle dock in Enum.GetValues(typeof(DockStyle)))
            {
                yield return new object[] { dock, AnchorStyles.Top, AnchorStyles.Top, DockStyle.None };
                yield return new object[] { dock, AnchorStyles.Top | AnchorStyles.Bottom, AnchorStyles.Top | AnchorStyles.Bottom, DockStyle.None };
                yield return new object[] { dock, AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left, AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left, DockStyle.None };
                yield return new object[] { dock, AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right, AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right, DockStyle.None };
                yield return new object[] { dock, AnchorStyles.Top | AnchorStyles.Left, AnchorStyles.Top | AnchorStyles.Left, dock };
                yield return new object[] { dock, AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right, AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right, DockStyle.None };
                yield return new object[] { dock, AnchorStyles.Top | AnchorStyles.Right, AnchorStyles.Top | AnchorStyles.Right, DockStyle.None };
                yield return new object[] { dock, AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right, AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right, DockStyle.None };

                yield return new object[] { dock, AnchorStyles.Bottom, AnchorStyles.Bottom, DockStyle.None };
                yield return new object[] { dock, AnchorStyles.Bottom | AnchorStyles.Left, AnchorStyles.Bottom | AnchorStyles.Left, DockStyle.None };
                yield return new object[] { dock, AnchorStyles.Bottom | AnchorStyles.Right, AnchorStyles.Bottom | AnchorStyles.Right, DockStyle.None };
                yield return new object[] { dock, AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right, AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right, DockStyle.None };

                yield return new object[] { dock, AnchorStyles.Left, AnchorStyles.Left, DockStyle.None };
                yield return new object[] { dock, AnchorStyles.Left | AnchorStyles.Right, AnchorStyles.Left | AnchorStyles.Right, DockStyle.None };

                yield return new object[] { dock, AnchorStyles.Right, AnchorStyles.Right, DockStyle.None };

                yield return new object[] { dock, AnchorStyles.None, AnchorStyles.None, DockStyle.None };
                yield return new object[] { dock, (AnchorStyles)(-1), AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right, DockStyle.None };
                yield return new object[] { dock, (AnchorStyles)int.MaxValue, AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right, DockStyle.None };
            }
        }

        [Theory]
        [MemberData(nameof(Anchor_SetWithDock_TestData))]
        public void Control_Anchor_SetWithDock_GetReturnsExpected(DockStyle dock, AnchorStyles value, AnchorStyles expectedAnchor, DockStyle expectedDock)
        {
            var control = new Control
            {
                Dock = dock
            };

            control.Anchor = value;
            Assert.Equal(expectedAnchor, control.Anchor);
            Assert.Equal(expectedDock, control.Dock);

            // Set same.
            control.Anchor = value;
            Assert.Equal(expectedAnchor, control.Anchor);
            Assert.Equal(expectedDock, control.Dock);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetPointTheoryData))]
        public void Control_AutoScrollOffset_Set_GetReturnsExpected(Point value)
        {
            var control = new Control
            {
                AutoScrollOffset = value
            };
            Assert.Equal(value, control.AutoScrollOffset);

            // Set same.
            control.AutoScrollOffset = value;
            Assert.Equal(value, control.AutoScrollOffset);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void Control_AutoSize_Set_GetReturnsExpected(bool value)
        {
            using var control = new Control();
            int layoutCallCount = 0;
            control.Layout += (sender, e) => layoutCallCount++;

            control.AutoSize = value;
            Assert.Equal(value, control.AutoSize);
            Assert.False(control.IsHandleCreated);
            Assert.Equal(0, layoutCallCount);

            // Set same.
            control.AutoSize = value;
            Assert.Equal(value, control.AutoSize);
            Assert.False(control.IsHandleCreated);
            Assert.Equal(0, layoutCallCount);

            // Set different.
            control.AutoSize = !value;
            Assert.Equal(!value, control.AutoSize);
            Assert.False(control.IsHandleCreated);
            Assert.Equal(0, layoutCallCount);
        }

        [WinFormsTheory]
        [InlineData(true, 1)]
        [InlineData(false, 0)]
        public void Control_AutoSize_SetWithParent_GetReturnsExpected(bool value, int expectedLayoutCallCount)
        {
            using var parent = new Control();
            using var control = new Control
            {
                Parent = parent
            };
            int layoutCallCount = 0;
            parent.Layout += (sender, e) =>
            {
                if (e.AffectedProperty == "Parent")
                {
                    return;
                }

                Assert.Same(parent, sender);
                Assert.Same(control, e.AffectedControl);
                Assert.Equal("AutoSize", e.AffectedProperty);
                layoutCallCount++;
            };

            control.AutoSize = value;
            Assert.Equal(value, control.AutoSize);
            Assert.False(control.IsHandleCreated);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);

            // Set same.
            control.AutoSize = value;
            Assert.Equal(value, control.AutoSize);
            Assert.False(control.IsHandleCreated);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);

            // Set different.
            control.AutoSize = !value;
            Assert.Equal(!value, control.AutoSize);
            Assert.False(control.IsHandleCreated);
            Assert.Equal(expectedLayoutCallCount + 1, layoutCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void Control_AutoSize_SetWithHandle_GetReturnsExpected(bool value)
        {
            using var control = new Control();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;
            int layoutCallCount = 0;
            control.Layout += (sender, e) => layoutCallCount++;

            control.AutoSize = value;
            Assert.Equal(value, control.AutoSize);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.Equal(0, layoutCallCount);

            // Set same.
            control.AutoSize = value;
            Assert.Equal(value, control.AutoSize);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.Equal(0, layoutCallCount);

            // Set different.
            control.AutoSize = !value;
            Assert.Equal(!value, control.AutoSize);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.Equal(0, layoutCallCount);
        }

        [WinFormsFact]
        public void Control_AutoSize_SetWithHandler_CallsAutoSizeChanged()
        {
            using var control = new Control
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
        [CommonMemberData(nameof(CommonTestHelper.GetBackColorTheoryData))]
        public void Control_BackColor_Set_GetReturnsExpected(Color value, Color expected)
        {
            using var control = new Control
            {
                BackColor = value
            };
            Assert.Equal(expected, control.BackColor);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.BackColor = value;
            Assert.Equal(expected, control.BackColor);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBackColorTheoryData))]
        public void Control_BackColor_SetWithCustomOldValue_GetReturnsExpected(Color value, Color expected)
        {
            using var control = new Control
            {
                BackColor = Color.YellowGreen
            };

            control.BackColor = value;
            Assert.Equal(expected, control.BackColor);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.BackColor = value;
            Assert.Equal(expected, control.BackColor);
            Assert.False(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> BackColor_SetTransparent_TestData()
        {
            yield return new object[] { Color.Red, Color.Red };
            yield return new object[] { Color.FromArgb(254, 1, 2, 3), Color.FromArgb(254, 1, 2, 3) };
            yield return new object[] { Color.Empty, Control.DefaultBackColor };
        }

        [WinFormsTheory]
        [MemberData(nameof(BackColor_SetTransparent_TestData))]
        public void Control_BackColor_SetTransparent_GetReturnsExpected(Color value, Color expected)
        {
            using var control = new SubControl();
            control.SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            control.BackColor = value;
            Assert.Equal(expected, control.BackColor);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.BackColor = value;
            Assert.Equal(expected, control.BackColor);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBackColorTheoryData))]
        public void Control_BackColor_SetWithChildren_GetReturnsExpected(Color value, Color expected)
        {
            using var child1 = new Control();
            using var child2 = new Control();
            using var control = new Control();
            control.Controls.Add(child1);
            control.Controls.Add(child2);

            control.BackColor = value;
            Assert.Equal(expected, control.BackColor);
            Assert.Equal(expected, child1.BackColor);
            Assert.Equal(expected, child2.BackColor);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.BackColor = value;
            Assert.Equal(expected, control.BackColor);
            Assert.Equal(expected, child1.BackColor);
            Assert.Equal(expected, child2.BackColor);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBackColorTheoryData))]
        public void Control_BackColor_SetWithChildrenWithColor_GetReturnsExpected(Color value, Color expected)
        {
            using var child1 = new Control
            {
                BackColor = Color.Yellow
            };
            using var child2 = new Control
            {
                BackColor = Color.YellowGreen
            };
            using var control = new Control();
            control.Controls.Add(child1);
            control.Controls.Add(child2);

            control.BackColor = value;
            Assert.Equal(expected, control.BackColor);
            Assert.Equal(Color.Yellow, child1.BackColor);
            Assert.Equal(Color.YellowGreen, child2.BackColor);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.BackColor = value;
            Assert.Equal(expected, control.BackColor);
            Assert.Equal(Color.Yellow, child1.BackColor);
            Assert.Equal(Color.YellowGreen, child2.BackColor);
            Assert.False(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> BackColor_SetWithHandle_TestData()
        {
            yield return new object[] { Color.Red, Color.Red, 1 };
            yield return new object[] { Color.Empty, Control.DefaultBackColor, 0 };
        }

        [WinFormsTheory]
        [MemberData(nameof(BackColor_SetWithHandle_TestData))]
        public void Control_BackColor_SetWithHandle_GetReturnsExpected(Color value, Color expected, int expectedInvalidatedCallCount)
        {
            using var control = new Control();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.BackColor = value;
            Assert.Equal(expected, control.BackColor);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.BackColor = value;
            Assert.Equal(expected, control.BackColor);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void Control_BackColor_SetWithHandler_CallsBackColorChanged()
        {
            using var control = new Control();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            control.BackColorChanged += handler;

            // Set different.
            control.BackColor = Color.Red;
            Assert.Equal(Color.Red, control.BackColor);
            Assert.Equal(1, callCount);

            // Set same.
            control.BackColor = Color.Red;
            Assert.Equal(Color.Red, control.BackColor);
            Assert.Equal(1, callCount);

            // Set different.
            control.BackColor = Color.Empty;
            Assert.Equal(Control.DefaultBackColor, control.BackColor);
            Assert.Equal(2, callCount);

            // Remove handler.
            control.BackColorChanged -= handler;
            control.BackColor = Color.Red;
            Assert.Equal(Color.Red, control.BackColor);
            Assert.Equal(2, callCount);
        }

        [WinFormsFact]
        public void Control_BackColor_SetWithHandlerInDisposing_DoesNotCallBackColorChanged()
        {
            using var control = new Control();
            Assert.NotEqual(IntPtr.Zero, control.Handle);

            int callCount = 0;
            control.BackColorChanged += (sender, e) => callCount++;
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;

            int disposedCallCount = 0;
            control.Disposed += (sender, e) =>
            {
                control.BackColor = Color.Red;
                Assert.Equal(Color.Red, control.BackColor);
                Assert.Equal(0, callCount);
                Assert.Equal(0, invalidatedCallCount);
                disposedCallCount++;
            };

            control.Dispose();
            Assert.Equal(1, disposedCallCount);
        }

        [WinFormsFact]
        public void Control_BackColor_SetWithChildrenWithHandler_CallsBackColorChanged()
        {
            using var child1 = new Control();
            using var child2 = new Control();
            using var control = new Control();
            control.Controls.Add(child1);
            control.Controls.Add(child2);

            int callCount = 0;
            int childCallCount1 = 0;
            int childCallCount2 = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                Assert.Equal(callCount, childCallCount1);
                Assert.Equal(childCallCount1, childCallCount2);
                callCount++;
            };
            EventHandler childHandler1 = (sender, e) =>
            {
                Assert.Same(child1, sender);
                Assert.Same(EventArgs.Empty, e);
                Assert.Equal(callCount - 1, childCallCount1);
                Assert.Equal(childCallCount1, childCallCount2);
                childCallCount1++;
            };
            EventHandler childHandler2 = (sender, e) =>
            {
                Assert.Same(child2, sender);
                Assert.Same(EventArgs.Empty, e);
                Assert.Equal(callCount, childCallCount1);
                Assert.Equal(childCallCount1 - 1, childCallCount2);
                childCallCount2++;
            };
            control.BackColorChanged += handler;
            child1.BackColorChanged += childHandler1;
            child2.BackColorChanged += childHandler2;

            // Set different.
            control.BackColor = Color.Red;
            Assert.Equal(Color.Red, control.BackColor);
            Assert.Equal(Color.Red, child1.BackColor);
            Assert.Equal(Color.Red, child2.BackColor);
            Assert.Equal(1, callCount);
            Assert.Equal(1, childCallCount1);
            Assert.Equal(1, childCallCount2);

            // Set same.
            control.BackColor = Color.Red;
            Assert.Equal(Color.Red, control.BackColor);
            Assert.Equal(Color.Red, child1.BackColor);
            Assert.Equal(Color.Red, child2.BackColor);
            Assert.Equal(1, callCount);
            Assert.Equal(1, childCallCount1);
            Assert.Equal(1, childCallCount2);

            // Set different.
            control.BackColor = Color.Empty;
            Assert.Equal(Control.DefaultBackColor, control.BackColor);
            Assert.Equal(Control.DefaultBackColor, child1.BackColor);
            Assert.Equal(Control.DefaultBackColor, child2.BackColor);
            Assert.Equal(2, callCount);
            Assert.Equal(2, childCallCount1);
            Assert.Equal(2, childCallCount2);

            // Remove handler.
            control.BackColorChanged -= handler;
            child1.BackColorChanged -= childHandler1;
            child2.BackColorChanged -= childHandler2;
            control.BackColor = Color.Red;
            Assert.Equal(Color.Red, control.BackColor);
            Assert.Equal(Color.Red, child1.BackColor);
            Assert.Equal(Color.Red, child2.BackColor);
            Assert.Equal(2, callCount);
            Assert.Equal(2, childCallCount1);
            Assert.Equal(2, childCallCount2);
        }

        [WinFormsFact]
        public void Control_BackColor_SetWithChildrenWithBackColorWithHandler_CallsBackColorChanged()
        {
            using var child1 = new Control
            {
                BackColor = Color.Yellow
            };
            using var child2 = new Control
            {
                BackColor = Color.YellowGreen
            };
            using var control = new Control();
            control.Controls.Add(child1);
            control.Controls.Add(child2);

            int callCount = 0;
            int childCallCount1 = 0;
            int childCallCount2 = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            EventHandler childHandler1 = (sender, e) =>
            {
                Assert.Same(child1, sender);
                Assert.Same(EventArgs.Empty, e);
                childCallCount1++;
            };
            EventHandler childHandler2 = (sender, e) =>
            {
                Assert.Same(child2, sender);
                Assert.Same(EventArgs.Empty, e);
                childCallCount2++;
            };
            control.BackColorChanged += handler;
            child1.BackColorChanged += childHandler1;
            child2.BackColorChanged += childHandler2;

            // Set different.
            control.BackColor = Color.Red;
            Assert.Equal(Color.Red, control.BackColor);
            Assert.Equal(Color.Yellow, child1.BackColor);
            Assert.Equal(Color.YellowGreen, child2.BackColor);
            Assert.Equal(1, callCount);
            Assert.Equal(0, childCallCount1);
            Assert.Equal(0, childCallCount2);

            // Set same.
            control.BackColor = Color.Red;
            Assert.Equal(Color.Red, control.BackColor);
            Assert.Equal(Color.Yellow, child1.BackColor);
            Assert.Equal(Color.YellowGreen, child2.BackColor);
            Assert.Equal(1, callCount);
            Assert.Equal(0, childCallCount1);
            Assert.Equal(0, childCallCount2);

            // Set different.
            control.BackColor = Color.Empty;
            Assert.Equal(Control.DefaultBackColor, control.BackColor);
            Assert.Equal(Color.Yellow, child1.BackColor);
            Assert.Equal(Color.YellowGreen, child2.BackColor);
            Assert.Equal(2, callCount);
            Assert.Equal(0, childCallCount1);
            Assert.Equal(0, childCallCount2);

            // Remove handler.
            control.BackColorChanged -= handler;
            child1.BackColorChanged -= childHandler1;
            child2.BackColorChanged -= childHandler2;
            control.BackColor = Color.Red;
            Assert.Equal(Color.Red, control.BackColor);
            Assert.Equal(Color.Yellow, child1.BackColor);
            Assert.Equal(Color.YellowGreen, child2.BackColor);
            Assert.Equal(2, callCount);
            Assert.Equal(0, childCallCount1);
            Assert.Equal(0, childCallCount2);
        }

        [WinFormsFact]
        public void Control_BackColor_SetTransparent_ThrowsArgmentException()
        {
            using var control = new Control();
            Assert.Throws<ArgumentException>(null, () => control.BackColor = Color.FromArgb(254, 1, 2, 3));
        }

        [WinFormsFact]
        public void Control_BackColor_ResetValue_Success()
        {
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(Control))[nameof(Control.BackColor)];
            using var control = new Control();
            Assert.False(property.CanResetValue(control));

            control.BackColor = Color.Red;
            Assert.Equal(Color.Red, control.BackColor);
            Assert.True(property.CanResetValue(control));

            property.ResetValue(control);
            Assert.Equal(Control.DefaultBackColor, control.BackColor);
            Assert.False(property.CanResetValue(control));
        }

        [WinFormsFact]
        public void Control_BackColor_ShouldSerializeValue_Success()
        {
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(Control))[nameof(Control.BackColor)];
            using var control = new Control();
            Assert.False(property.ShouldSerializeValue(control));

            control.BackColor = Color.Red;
            Assert.Equal(Color.Red, control.BackColor);
            Assert.True(property.ShouldSerializeValue(control));

            property.ResetValue(control);
            Assert.Equal(Control.DefaultBackColor, control.BackColor);
            Assert.False(property.ShouldSerializeValue(control));
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetImageTheoryData))]
        public void Control_BackgroundImage_Set_GetReturnsExpected(Image value)
        {
            var control = new Control
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

        public static IEnumerable<object[]> BackgroundImage_SetWithHandle_TestData()
        {
            yield return new object[] { new Bitmap(10, 10), 1 };
            yield return new object[] { null, 0 };
        }

        [Theory]
        [MemberData(nameof(BackgroundImage_SetWithHandle_TestData))]
        public void Control_BackgroundImage_SetWithHandle_GetReturnsExpected(Image value, int expectedInvalidatedCallCount)
        {
            var control = new Control();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.BackgroundImage = value;
            Assert.Same(value, control.BackgroundImage);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.BackgroundImage = value;
            Assert.Same(value, control.BackgroundImage);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetImageTheoryData))]
        public void Control_BackgroundImage_SetWithChildren_GetReturnsExpected(Image value)
        {
            var child1 = new Control();
            var child2 = new Control();
            var control = new Control();
            control.Controls.Add(child1);
            control.Controls.Add(child2);

            control.BackgroundImage = value;
            Assert.Same(value, control.BackgroundImage);
            Assert.Null(child1.BackgroundImage);
            Assert.Null(child2.BackgroundImage);

            // Set same.
            control.BackgroundImage = value;
            Assert.Same(value, control.BackgroundImage);
            Assert.Null(child1.BackgroundImage);
            Assert.Null(child2.BackgroundImage);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetImageTheoryData))]
        public void Control_BackgroundImage_SetWithChildrenWithBackgroundImage_GetReturnsExpected(Image value)
        {
            var image1 = new Bitmap(10, 10);
            var image2 = new Bitmap(10, 10);
            var child1 = new Control
            {
                BackgroundImage = image1
            };
            var child2 = new Control
            {
                BackgroundImage = image2
            };
            var control = new Control();
            control.Controls.Add(child1);
            control.Controls.Add(child2);

            control.BackgroundImage = value;
            Assert.Same(value, control.BackgroundImage);
            Assert.Same(image1, child1.BackgroundImage);
            Assert.Same(image2, child2.BackgroundImage);

            // Set same.
            control.BackgroundImage = value;
            Assert.Same(value, control.BackgroundImage);
            Assert.Same(image1, child1.BackgroundImage);
            Assert.Same(image2, child2.BackgroundImage);
        }

        [Fact]
        public void Control_BackgroundImage_SetWithHandler_CallsBackgroundImageChanged()
        {
            var control = new Control();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            control.BackgroundImageChanged += handler;

            // Set different.
            var image1 = new Bitmap(10, 10);
            control.BackgroundImage = image1;
            Assert.Same(image1, control.BackgroundImage);
            Assert.Equal(1, callCount);

            // Set same.
            control.BackgroundImage = image1;
            Assert.Same(image1, control.BackgroundImage);
            Assert.Equal(1, callCount);

            // Set different.
            var image2 = new Bitmap(10, 10);
            control.BackgroundImage = image2;
            Assert.Same(image2, control.BackgroundImage);
            Assert.Equal(2, callCount);

            // Set null.
            control.BackgroundImage = null;
            Assert.Null(control.BackgroundImage);
            Assert.Equal(3, callCount);

            // Remove handler.
            control.BackgroundImageChanged -= handler;
            control.BackgroundImage = image1;
            Assert.Same(image1, control.BackgroundImage);
            Assert.Equal(3, callCount);
        }

        [Fact]
        public void Control_BackgroundImage_SetWithHandlerInDisposing_DoesNotCallBackgroundImageChanged()
        {
            var control = new Control();
            Assert.NotEqual(IntPtr.Zero, control.Handle);

            int callCount = 0;
            control.BackgroundImageChanged += (sender, e) => callCount++;
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;

            int disposedCallCount = 0;
            control.Disposed += (sender, e) =>
            {
                var value = new Bitmap(10, 10);
                control.BackgroundImage = value;
                Assert.Same(value, control.BackgroundImage);
                Assert.Equal(0, callCount);
                Assert.Equal(0, invalidatedCallCount);
                disposedCallCount++;
            };

            control.Dispose();
            Assert.Equal(1, disposedCallCount);
        }

        [Fact]
        public void Control_BackgroundImage_SetWithChildrenWithHandler_CallsBackgroundImageChanged()
        {
            var child1 = new Control();
            var child2 = new Control();
            var control = new Control();
            control.Controls.Add(child1);
            control.Controls.Add(child2);

            int callCount = 0;
            int child1CallCount = 0;
            int child2CallCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            EventHandler childHandler1 = (sender, e) =>
            {
                Assert.Same(child1, sender);
                Assert.Same(EventArgs.Empty, e);
                child1CallCount++;
            };
            EventHandler childHandler2 = (sender, e) =>
            {
                Assert.Same(child2, sender);
                Assert.Same(EventArgs.Empty, e);
                child2CallCount++;
            };
            control.BackgroundImageChanged += handler;
            child1.BackgroundImageChanged += childHandler1;
            child2.BackgroundImageChanged += childHandler2;

            // Set different.
            var image1 = new Bitmap(10, 10);
            control.BackgroundImage = image1;
            Assert.Same(image1, control.BackgroundImage);
            Assert.Null(child1.BackgroundImage);
            Assert.Null(child2.BackgroundImage);
            Assert.Equal(1, callCount);
            Assert.Equal(1, child1CallCount);
            Assert.Equal(1, child2CallCount);

            // Set same.
            control.BackgroundImage = image1;
            Assert.Same(image1, control.BackgroundImage);
            Assert.Null(child1.BackgroundImage);
            Assert.Null(child2.BackgroundImage);
            Assert.Equal(1, callCount);
            Assert.Equal(1, child1CallCount);
            Assert.Equal(1, child2CallCount);

            // Set different.
            var image2 = new Bitmap(10, 10);
            control.BackgroundImage = image2;
            Assert.Same(image2, control.BackgroundImage);
            Assert.Null(child1.BackgroundImage);
            Assert.Null(child2.BackgroundImage);
            Assert.Equal(2, callCount);
            Assert.Equal(2, child1CallCount);
            Assert.Equal(2, child2CallCount);

            // Set null.
            control.BackgroundImage = null;
            Assert.Null(control.BackgroundImage);
            Assert.Null(child1.BackgroundImage);
            Assert.Null(child2.BackgroundImage);
            Assert.Equal(3, callCount);
            Assert.Equal(3, child1CallCount);
            Assert.Equal(3, child2CallCount);

            // Remove handler.
            control.BackgroundImageChanged -= handler;
            child1.BackgroundImageChanged -= childHandler1;
            child2.BackgroundImageChanged -= childHandler2;
            control.BackgroundImage = image1;
            Assert.Same(image1, control.BackgroundImage);
            Assert.Null(child1.BackgroundImage);
            Assert.Null(child2.BackgroundImage);
            Assert.Equal(3, callCount);
            Assert.Equal(3, child1CallCount);
            Assert.Equal(3, child2CallCount);
        }

        [Fact]
        public void Control_BackgroundImage_SetWithChildrenWithBackgroundImageWithHandler_CallsBackgroundImageChanged()
        {
            var childBackgroundImage1 = new Bitmap(10, 10);
            var childBackgroundImage2 = new Bitmap(10, 10);
            var child1 = new Control
            {
                BackgroundImage = childBackgroundImage1
            };
            var child2 = new Control
            {
                BackgroundImage = childBackgroundImage2
            };
            var control = new Control();
            control.Controls.Add(child1);
            control.Controls.Add(child2);

            int callCount = 0;
            int child1CallCount = 0;
            int child2CallCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            EventHandler childHandler1 = (sender, e) =>
            {
                Assert.Same(child1, sender);
                Assert.Same(EventArgs.Empty, e);
                child1CallCount++;
            };
            EventHandler childHandler2 = (sender, e) =>
            {
                Assert.Same(child2, sender);
                Assert.Same(EventArgs.Empty, e);
                child2CallCount++;
            };
            control.BackgroundImageChanged += handler;
            child1.BackgroundImageChanged += childHandler1;
            child2.BackgroundImageChanged += childHandler2;

            // Set different.
            var image1 = new Bitmap(10, 10);
            control.BackgroundImage = image1;
            Assert.Same(image1, control.BackgroundImage);
            Assert.Same(childBackgroundImage1, child1.BackgroundImage);
            Assert.Same(childBackgroundImage2, child2.BackgroundImage);
            Assert.Equal(1, callCount);
            Assert.Equal(1, child1CallCount);
            Assert.Equal(1, child2CallCount);

            // Set same.
            control.BackgroundImage = image1;
            Assert.Same(image1, control.BackgroundImage);
            Assert.Same(childBackgroundImage1, child1.BackgroundImage);
            Assert.Same(childBackgroundImage2, child2.BackgroundImage);
            Assert.Equal(1, callCount);
            Assert.Equal(1, child1CallCount);
            Assert.Equal(1, child2CallCount);

            // Set different.
            var image2 = new Bitmap(10, 10);
            control.BackgroundImage = image2;
            Assert.Same(image2, control.BackgroundImage);
            Assert.Same(childBackgroundImage1, child1.BackgroundImage);
            Assert.Same(childBackgroundImage2, child2.BackgroundImage);
            Assert.Equal(2, callCount);
            Assert.Equal(2, child1CallCount);
            Assert.Equal(2, child2CallCount);

            // Set null.
            control.BackgroundImage = null;
            Assert.Null(control.BackgroundImage);
            Assert.Same(childBackgroundImage1, child1.BackgroundImage);
            Assert.Same(childBackgroundImage2, child2.BackgroundImage);
            Assert.Equal(3, callCount);
            Assert.Equal(3, child1CallCount);
            Assert.Equal(3, child2CallCount);

            // Remove handler.
            control.BackgroundImageChanged -= handler;
            child1.BackgroundImageChanged -= childHandler1;
            child2.BackgroundImageChanged -= childHandler2;
            control.BackgroundImage = image1;
            Assert.Same(image1, control.BackgroundImage);
            Assert.Same(childBackgroundImage1, child1.BackgroundImage);
            Assert.Same(childBackgroundImage2, child2.BackgroundImage);
            Assert.Equal(3, callCount);
            Assert.Equal(3, child1CallCount);
            Assert.Equal(3, child2CallCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(ImageLayout))]
        public void Control_BackgroundImageLayout_Set_GetReturnsExpected(ImageLayout value)
        {
            var control = new SubControl
            {
                BackgroundImageLayout = value
            };
            Assert.Equal(value, control.BackgroundImageLayout);
            Assert.False(control.DoubleBuffered);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.BackgroundImageLayout = value;
            Assert.Equal(value, control.BackgroundImageLayout);
            Assert.False(control.DoubleBuffered);
            Assert.False(control.IsHandleCreated);
        }

        [Theory]
        [InlineData(ImageLayout.Center, 1)]
        [InlineData(ImageLayout.None, 1)]
        [InlineData(ImageLayout.Stretch, 1)]
        [InlineData(ImageLayout.Tile, 0)]
        [InlineData(ImageLayout.Zoom, 1)]
        public void Control_BackgroundImageLayout_SetWithHandle_GetReturnsExpected(ImageLayout value, int expectedInvalidatedCallCount)
        {
            var control = new SubControl();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.BackgroundImageLayout = value;
            Assert.Equal(value, control.BackgroundImageLayout);
            Assert.False(control.DoubleBuffered);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.BackgroundImageLayout = value;
            Assert.Equal(value, control.BackgroundImageLayout);
            Assert.False(control.DoubleBuffered);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        public static IEnumerable<object[]> BackgroundImageLayout_SetWithBackgroundImage_TestData()
        {
            yield return new object[] { new Bitmap(10, 10, PixelFormat.Format32bppArgb), ImageLayout.None, false };
            yield return new object[] { new Bitmap(10, 10, PixelFormat.Format32bppArgb), ImageLayout.Tile, false };
            yield return new object[] { new Bitmap(10, 10, PixelFormat.Format32bppArgb), ImageLayout.Center, true };
            yield return new object[] { new Bitmap(10, 10, PixelFormat.Format32bppArgb), ImageLayout.Stretch, true };
            yield return new object[] { new Bitmap(10, 10, PixelFormat.Format32bppArgb), ImageLayout.Zoom, true };

            yield return new object[] { new Bitmap(10, 10, PixelFormat.Format32bppRgb), ImageLayout.None, false };
            yield return new object[] { new Bitmap(10, 10, PixelFormat.Format32bppRgb), ImageLayout.Tile, false };
            yield return new object[] { new Bitmap(10, 10, PixelFormat.Format32bppRgb), ImageLayout.Center, false };
            yield return new object[] { new Bitmap(10, 10, PixelFormat.Format32bppRgb), ImageLayout.Stretch, false };
            yield return new object[] { new Bitmap(10, 10, PixelFormat.Format32bppRgb), ImageLayout.Zoom, false };
        }

        [Theory]
        [MemberData(nameof(BackgroundImageLayout_SetWithBackgroundImage_TestData))]
        public void Control_BackgroundImageLayout_SetWithBackgroundImage_GetReturnsExpected(Image backgroundImage, ImageLayout value, bool expectedDoubleBuffered)
        {
            var control = new SubControl
            {
                BackgroundImage = backgroundImage,
                BackgroundImageLayout = value
            };
            Assert.Equal(value, control.BackgroundImageLayout);
            Assert.Equal(expectedDoubleBuffered, control.DoubleBuffered);

            // Set same.
            control.BackgroundImageLayout = value;
            Assert.Equal(value, control.BackgroundImageLayout);
            Assert.Equal(expectedDoubleBuffered, control.DoubleBuffered);
        }

        [Fact]
        public void Control_BackgroundImageLayout_SetWithHandler_CallsBackgroundImageLayoutChanged()
        {
            var control = new Control();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            control.BackgroundImageLayoutChanged += handler;

            // Set different.
            control.BackgroundImageLayout = ImageLayout.Center;
            Assert.Equal(ImageLayout.Center, control.BackgroundImageLayout);
            Assert.Equal(1, callCount);

            // Set same.
            control.BackgroundImageLayout = ImageLayout.Center;
            Assert.Equal(ImageLayout.Center, control.BackgroundImageLayout);
            Assert.Equal(1, callCount);

            // Set different.
            control.BackgroundImageLayout = ImageLayout.Stretch;
            Assert.Equal(ImageLayout.Stretch, control.BackgroundImageLayout);
            Assert.Equal(2, callCount);

            // Remove handler.
            control.BackgroundImageLayoutChanged -= handler;
            control.BackgroundImageLayout = ImageLayout.Center;
            Assert.Equal(ImageLayout.Center, control.BackgroundImageLayout);
            Assert.Equal(2, callCount);
        }

        [Fact]
        public void Control_BackgroundImageLayout_SetWithHandlerInDisposing_DoesNotCallBackgroundImageLayoutChanged()
        {
            var control = new Control();
            Assert.NotEqual(IntPtr.Zero, control.Handle);

            int callCount = 0;
            control.BackgroundImageLayoutChanged += (sender, e) => callCount++;
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;

            int disposedCallCount = 0;
            control.Disposed += (sender, e) =>
            {
                control.BackgroundImageLayout = ImageLayout.Center;
                Assert.Equal(ImageLayout.Center, control.BackgroundImageLayout);
                Assert.Equal(0, callCount);
                Assert.Equal(0, invalidatedCallCount);
                disposedCallCount++;
            };

            control.Dispose();
            Assert.Equal(1, disposedCallCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(ImageLayout))]
        public void Control_BackgroundImageLayout_SetInvalid_ThrowsInvalidEnumArgumentException(ImageLayout value)
        {
            var control = new Control();
            Assert.Throws<InvalidEnumArgumentException>("value", () => control.BackgroundImageLayout = value);
        }

        public static IEnumerable<object[]> BindingContext_Set_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new BindingContext() };
        }

        [Theory]
        [MemberData(nameof(BindingContext_Set_TestData))]
        public void Control_BindingContext_Set_GetReturnsExpected(BindingContext value)
        {
            var control = new Control
            {
                BindingContext = value
            };
            Assert.Same(value, control.BindingContext);

            // Set same.
            control.BindingContext = value;
            Assert.Same(value, control.BindingContext);
        }

        [Theory]
        [MemberData(nameof(BindingContext_Set_TestData))]
        public void Control_BindingContext_SetWithNonNullBindingContext_GetReturnsExpected(BindingContext value)
        {
            var control = new Control
            {
                BindingContext = new BindingContext()
            };

            control.BindingContext = value;
            Assert.Same(value, control.BindingContext);

            // Set same.
            control.BindingContext = value;
            Assert.Same(value, control.BindingContext);
        }

        [Fact]
        public void Control_BindingContext_SetWithHandler_CallsBindingContextChanged()
        {
            var control = new Control();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            control.BindingContextChanged += handler;

            // Set different.
            var context1 = new BindingContext();
            control.BindingContext = context1;
            Assert.Same(context1, control.BindingContext);
            Assert.Equal(1, callCount);

            // Set same.
            control.BindingContext = context1;
            Assert.Same(context1, control.BindingContext);
            Assert.Equal(1, callCount);

            // Set different.
            var context2 = new BindingContext();
            control.BindingContext = context2;
            Assert.Same(context2, control.BindingContext);
            Assert.Equal(2, callCount);

            // Set null.
            control.BindingContext = null;
            Assert.Null(control.BindingContext);
            Assert.Equal(3, callCount);

            // Remove handler.
            control.BindingContextChanged -= handler;
            control.BindingContext = context1;
            Assert.Same(context1, control.BindingContext);
            Assert.Equal(3, callCount);
        }

        [Fact]
        public void Control_BindingContext_SetWithChildrenWithHandler_CallsBindingContextChanged()
        {
            var child1 = new Control();
            var child2 = new Control();
            var control = new Control();
            control.Controls.Add(child1);
            control.Controls.Add(child2);

            int callCount = 0;
            int childCallCount1 = 0;
            int childCallCount2 = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            EventHandler childHandler1 = (sender, e) =>
            {
                Assert.Same(child1, sender);
                Assert.Same(EventArgs.Empty, e);
                childCallCount1++;
            };
            EventHandler childHandler2 = (sender, e) =>
            {
                Assert.Same(child2, sender);
                Assert.Same(EventArgs.Empty, e);
                childCallCount2++;
            };
            control.BindingContextChanged += handler;
            child1.BindingContextChanged += childHandler1;
            child2.BindingContextChanged += childHandler2;

            // Set different.
            var context1 = new BindingContext();
            control.BindingContext = context1;
            Assert.Same(context1, control.BindingContext);
            Assert.Same(context1, child1.BindingContext);
            Assert.Same(context1, child2.BindingContext);
            Assert.Equal(1, callCount);
            Assert.Equal(1, childCallCount1);
            Assert.Equal(1, childCallCount2);

            // Set same.
            control.BindingContext = context1;
            Assert.Same(context1, control.BindingContext);
            Assert.Same(context1, child1.BindingContext);
            Assert.Same(context1, child2.BindingContext);
            Assert.Equal(1, callCount);
            Assert.Equal(1, childCallCount1);
            Assert.Equal(1, childCallCount2);

            // Set different.
            var context2 = new BindingContext();
            control.BindingContext = context2;
            Assert.Same(context2, control.BindingContext);
            Assert.Same(context2, child1.BindingContext);
            Assert.Same(context2, child2.BindingContext);
            Assert.Equal(2, callCount);
            Assert.Equal(2, childCallCount1);
            Assert.Equal(2, childCallCount2);

            // Set null.
            control.BindingContext = null;
            Assert.Null(control.BindingContext);
            Assert.Null(child1.BindingContext);
            Assert.Null(child2.BindingContext);
            Assert.Equal(3, callCount);
            Assert.Equal(3, childCallCount1);
            Assert.Equal(3, childCallCount2);

            // Remove handler.
            control.BindingContextChanged -= handler;
            child1.BindingContextChanged -= childHandler1;
            child2.BindingContextChanged -= childHandler2;
            control.BindingContext = context1;
            Assert.Same(context1, control.BindingContext);
            Assert.Same(context1, child1.BindingContext);
            Assert.Same(context1, child2.BindingContext);
            Assert.Equal(3, callCount);
            Assert.Equal(3, childCallCount1);
            Assert.Equal(3, childCallCount2);
        }

        [Fact]
        public void Control_BindingContext_SetWithChildrenWithBindingContextWithHandler_CallsBindingContextChanged()
        {
            var childContext1 = new BindingContext();
            var childContext2 = new BindingContext();
            var child1 = new Control
            {
                BindingContext = childContext1
            };
            var child2 = new Control
            {
                BindingContext = childContext2
            };
            var control = new Control();
            control.Controls.Add(child1);
            control.Controls.Add(child2);

            int callCount = 0;
            int childCallCount1 = 0;
            int childCallCount2 = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            EventHandler childHandler1 = (sender, e) =>
            {
                Assert.Same(child1, sender);
                Assert.Same(EventArgs.Empty, e);
                childCallCount1++;
            };
            EventHandler childHandler2 = (sender, e) =>
            {
                Assert.Same(child2, sender);
                Assert.Same(EventArgs.Empty, e);
                childCallCount2++;
            };
            control.BindingContextChanged += handler;
            child1.BindingContextChanged += childHandler1;
            child2.BindingContextChanged += childHandler2;

            // Set different.
            var context1 = new BindingContext();
            control.BindingContext = context1;
            Assert.Same(context1, control.BindingContext);
            Assert.Same(childContext1, child1.BindingContext);
            Assert.Same(childContext2, child2.BindingContext);
            Assert.Equal(1, callCount);
            Assert.Equal(0, childCallCount1);
            Assert.Equal(0, childCallCount2);

            // Set same.
            control.BindingContext = context1;
            Assert.Same(context1, control.BindingContext);
            Assert.Same(childContext1, child1.BindingContext);
            Assert.Same(childContext2, child2.BindingContext);
            Assert.Equal(1, callCount);
            Assert.Equal(0, childCallCount1);
            Assert.Equal(0, childCallCount2);

            // Set different.
            var context2 = new BindingContext();
            control.BindingContext = context2;
            Assert.Same(context2, control.BindingContext);
            Assert.Same(childContext1, child1.BindingContext);
            Assert.Same(childContext2, child2.BindingContext);
            Assert.Equal(2, callCount);
            Assert.Equal(0, childCallCount1);
            Assert.Equal(0, childCallCount2);

            // Set null.
            control.BindingContext = null;
            Assert.Null(control.BindingContext);
            Assert.Same(childContext1, child1.BindingContext);
            Assert.Same(childContext2, child2.BindingContext);
            Assert.Equal(3, callCount);
            Assert.Equal(0, childCallCount1);
            Assert.Equal(0, childCallCount2);

            // Remove handler.
            control.BindingContextChanged -= handler;
            child1.BindingContextChanged -= childHandler1;
            child2.BindingContextChanged -= childHandler2;
            control.BindingContext = context1;
            Assert.Same(context1, control.BindingContext);
            Assert.Same(childContext1, child1.BindingContext);
            Assert.Same(childContext2, child2.BindingContext);
            Assert.Equal(3, callCount);
            Assert.Equal(0, childCallCount1);
            Assert.Equal(0, childCallCount2);
        }

        [WinFormsTheory]
        [MemberData(nameof(SetBounds_Int_Int_Int_Int_TestData))]
        public void Control_Bounds_Set_GetReturnsExpected(int x, int y, int width, int height, int expectedLayoutCallCount)
        {
            using var control = new SubControl();
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

            control.Bounds = new Rectangle(x, y, width, height);
            Assert.Equal(new Size(width, height), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, width, height), control.ClientRectangle);
            Assert.Equal(new Rectangle(0, 0, width, height), control.DisplayRectangle);
            Assert.Equal(new Size(width, height), control.Size);
            Assert.Equal(x, control.Left);
            Assert.Equal(x + width, control.Right);
            Assert.Equal(y, control.Top);
            Assert.Equal(y + height, control.Bottom);
            Assert.Equal(width, control.Width);
            Assert.Equal(height, control.Height);
            Assert.Equal(new Rectangle(x, y, width, height), control.Bounds);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.Equal(expectedLayoutCallCount, resizeCallCount);
            Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
            Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
            Assert.False(control.IsHandleCreated);

            // Call again.
            control.Bounds = new Rectangle(x, y, width, height);
            Assert.Equal(new Size(width, height), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, width, height), control.ClientRectangle);
            Assert.Equal(new Rectangle(0, 0, width, height), control.DisplayRectangle);
            Assert.Equal(new Size(width, height), control.Size);
            Assert.Equal(x, control.Left);
            Assert.Equal(x + width, control.Right);
            Assert.Equal(y, control.Top);
            Assert.Equal(y + height, control.Bottom);
            Assert.Equal(width, control.Width);
            Assert.Equal(height, control.Height);
            Assert.Equal(new Rectangle(x, y, width, height), control.Bounds);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.Equal(expectedLayoutCallCount, resizeCallCount);
            Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
            Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(SetBounds_Int_Int_Int_Int_WithConstrainedSize_TestData))]
        public void Control_Bounds_SetWithConstrainedSize_GetReturnsExpected(Size minimumSize, Size maximumSize, int x, int y, int width, int height, int expectedWidth, int expectedHeight, int expectedLayoutCallCount)
        {
            using var control = new SubControl
            {
                MinimumSize = minimumSize,
                MaximumSize = maximumSize
            };
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

            control.Bounds = new Rectangle(x, y, width, height);
            Assert.Equal(new Size(expectedWidth, expectedHeight), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.ClientRectangle);
            Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.DisplayRectangle);
            Assert.Equal(new Size(expectedWidth, expectedHeight), control.Size);
            Assert.Equal(x, control.Left);
            Assert.Equal(x + expectedWidth, control.Right);
            Assert.Equal(y, control.Top);
            Assert.Equal(y + expectedHeight, control.Bottom);
            Assert.Equal(expectedWidth, control.Width);
            Assert.Equal(expectedHeight, control.Height);
            Assert.Equal(new Rectangle(x, y, expectedWidth, expectedHeight), control.Bounds);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.Equal(expectedLayoutCallCount, resizeCallCount);
            Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
            Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
            Assert.False(control.IsHandleCreated);

            // Call again.
            control.Bounds = new Rectangle(x, y, width, height);
            Assert.Equal(new Size(expectedWidth, expectedHeight), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.ClientRectangle);
            Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.DisplayRectangle);
            Assert.Equal(new Size(expectedWidth, expectedHeight), control.Size);
            Assert.Equal(x, control.Left);
            Assert.Equal(x + expectedWidth, control.Right);
            Assert.Equal(y, control.Top);
            Assert.Equal(y + expectedHeight, control.Bottom);
            Assert.Equal(expectedWidth, control.Width);
            Assert.Equal(expectedHeight, control.Height);
            Assert.Equal(new Rectangle(x, y, expectedWidth, expectedHeight), control.Bounds);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.Equal(expectedLayoutCallCount, resizeCallCount);
            Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
            Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(SetBounds_Int_Int_Int_Int_WithCustomStyle_TestData))]
        public void Control_Bounds_SetWithCustomStyle_GetReturnsExpected(int x, int y, int width, int height, int expectedClientWidth, int expectedClientHeight, int expectedLayoutCallCount)
        {
            using var control = new BorderedControl();
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

            control.Bounds = new Rectangle(x, y, width, height);
            Assert.Equal(new Size(expectedClientWidth, expectedClientHeight), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, expectedClientWidth, expectedClientHeight), control.ClientRectangle);
            Assert.Equal(new Rectangle(0, 0, expectedClientWidth, expectedClientHeight), control.DisplayRectangle);
            Assert.Equal(new Size(width, height), control.Size);
            Assert.Equal(x, control.Left);
            Assert.Equal(x + width, control.Right);
            Assert.Equal(y, control.Top);
            Assert.Equal(y + height, control.Bottom);
            Assert.Equal(width, control.Width);
            Assert.Equal(height, control.Height);
            Assert.Equal(new Rectangle(x, y, width, height), control.Bounds);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.Equal(expectedLayoutCallCount, resizeCallCount);
            Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
            Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
            Assert.False(control.IsHandleCreated);

            // Call again.
            control.Bounds = new Rectangle(x, y, width, height);
            Assert.Equal(new Size(expectedClientWidth, expectedClientHeight), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, expectedClientWidth, expectedClientHeight), control.ClientRectangle);
            Assert.Equal(new Rectangle(0, 0, expectedClientWidth, expectedClientHeight), control.DisplayRectangle);
            Assert.Equal(new Size(width, height), control.Size);
            Assert.Equal(x, control.Left);
            Assert.Equal(x + width, control.Right);
            Assert.Equal(y, control.Top);
            Assert.Equal(y + height, control.Bottom);
            Assert.Equal(width, control.Width);
            Assert.Equal(height, control.Height);
            Assert.Equal(new Rectangle(x, y, width, height), control.Bounds);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.Equal(expectedLayoutCallCount, resizeCallCount);
            Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
            Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(SetBounds_Int_Int_Int_Int_WithParent_TestData))]
        public void Control_Bounds_SetWithParent_GetReturnsExpected(int x, int y, int width, int height, int expectedLayoutCallCount, int expectedParentLayoutCallCount)
        {
            using var parent = new Control();
            using var control = new SubControl
            {
                Parent = parent
            };
            int layoutCallCount = 0;
            int resizeCallCount = 0;
            int sizeChangedCallCount = 0;
            int clientSizeChangedCallCount = 0;
            int parentLayoutCallCount = 0;
            control.Layout += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(control, e.AffectedControl);
                Assert.Equal("Bounds", e.AffectedProperty);
                Assert.Equal(resizeCallCount, layoutCallCount);
                Assert.Equal(sizeChangedCallCount, layoutCallCount);
                Assert.Equal(clientSizeChangedCallCount, layoutCallCount);
                Assert.Equal(parentLayoutCallCount, layoutCallCount);
                layoutCallCount++;
            };
            control.Resize += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                Assert.Equal(layoutCallCount - 1, resizeCallCount);
                Assert.Equal(sizeChangedCallCount, resizeCallCount);
                Assert.Equal(clientSizeChangedCallCount, resizeCallCount);
                Assert.Equal(parentLayoutCallCount, resizeCallCount);
                resizeCallCount++;
            };
            control.SizeChanged += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                Assert.Equal(resizeCallCount - 1, sizeChangedCallCount);
                Assert.Equal(layoutCallCount - 1, sizeChangedCallCount);
                Assert.Equal(clientSizeChangedCallCount, sizeChangedCallCount);
                Assert.Equal(parentLayoutCallCount, sizeChangedCallCount);
                sizeChangedCallCount++;
            };
            control.ClientSizeChanged += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                Assert.Equal(resizeCallCount - 1, clientSizeChangedCallCount);
                Assert.Equal(layoutCallCount - 1, clientSizeChangedCallCount);
                Assert.Equal(sizeChangedCallCount - 1, clientSizeChangedCallCount);
                Assert.Equal(parentLayoutCallCount, clientSizeChangedCallCount);
                clientSizeChangedCallCount++;
            };
            void parentHandler(object sender, LayoutEventArgs e)
            {
                Assert.Same(parent, sender);
                Assert.Same(control, e.AffectedControl);
                Assert.Equal("Bounds", e.AffectedProperty);
                parentLayoutCallCount++;
            };
            parent.Layout += parentHandler;

            try
            {
                control.Bounds = new Rectangle(x, y, width, height);
                Assert.Equal(new Size(width, height), control.ClientSize);
                Assert.Equal(new Rectangle(0, 0, width, height), control.ClientRectangle);
                Assert.Equal(new Rectangle(0, 0, width, height), control.DisplayRectangle);
                Assert.Equal(new Size(width, height), control.Size);
                Assert.Equal(x, control.Left);
                Assert.Equal(x + width, control.Right);
                Assert.Equal(y, control.Top);
                Assert.Equal(y + height, control.Bottom);
                Assert.Equal(width, control.Width);
                Assert.Equal(height, control.Height);
                Assert.Equal(new Rectangle(x, y, width, height), control.Bounds);
                Assert.Equal(expectedLayoutCallCount, layoutCallCount);
                Assert.Equal(expectedLayoutCallCount, resizeCallCount);
                Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
                Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
                Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
                Assert.False(control.IsHandleCreated);

                // Call again.
                control.Bounds = new Rectangle(x, y, width, height);
                Assert.Equal(new Size(width, height), control.ClientSize);
                Assert.Equal(new Rectangle(0, 0, width, height), control.ClientRectangle);
                Assert.Equal(new Rectangle(0, 0, width, height), control.DisplayRectangle);
                Assert.Equal(new Size(width, height), control.Size);
                Assert.Equal(x, control.Left);
                Assert.Equal(x + width, control.Right);
                Assert.Equal(y, control.Top);
                Assert.Equal(y + height, control.Bottom);
                Assert.Equal(width, control.Width);
                Assert.Equal(height, control.Height);
                Assert.Equal(new Rectangle(x, y, width, height), control.Bounds);
                Assert.Equal(expectedLayoutCallCount, layoutCallCount);
                Assert.Equal(expectedLayoutCallCount, resizeCallCount);
                Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
                Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
                Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
                Assert.False(control.IsHandleCreated);
            }
            finally
            {
                parent.Layout -= parentHandler;
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(SetBounds_Int_Int_Int_Int_WithHandle_TestData))]
        public void Control_Bounds_SetWithHandle_GetReturnsExpected(bool resizeRedraw, int x, int y, int width, int height, int expectedWidth, int expectedHeight, int expectedLayoutCallCount, int expectedInvalidatedCallCount)
        {
            using var control = new SubControl();
            control.SetStyle(ControlStyles.ResizeRedraw, resizeRedraw);
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
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.Bounds = new Rectangle(x, y, width, height);
            Assert.Equal(new Size(expectedWidth, expectedHeight), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.ClientRectangle);
            Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.DisplayRectangle);
            Assert.Equal(new Size(expectedWidth, expectedHeight), control.Size);
            Assert.Equal(x, control.Left);
            Assert.Equal(x + expectedWidth, control.Right);
            Assert.Equal(y, control.Top);
            Assert.Equal(y + expectedHeight, control.Bottom);
            Assert.Equal(expectedWidth, control.Width);
            Assert.Equal(expectedHeight, control.Height);
            Assert.Equal(new Rectangle(x, y, expectedWidth, expectedHeight), control.Bounds);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.Equal(expectedLayoutCallCount, resizeCallCount);
            Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
            Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Call again.
            control.Bounds = new Rectangle(x, y, width, height);
            Assert.Equal(new Size(expectedWidth, expectedHeight), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.ClientRectangle);
            Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.DisplayRectangle);
            Assert.Equal(new Size(expectedWidth, expectedHeight), control.Size);
            Assert.Equal(x, control.Left);
            Assert.Equal(x + expectedWidth, control.Right);
            Assert.Equal(y, control.Top);
            Assert.Equal(y + expectedHeight, control.Bottom);
            Assert.Equal(expectedWidth, control.Width);
            Assert.Equal(expectedHeight, control.Height);
            Assert.Equal(new Rectangle(x, y, expectedWidth, expectedHeight), control.Bounds);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.Equal(expectedLayoutCallCount, resizeCallCount);
            Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
            Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [MemberData(nameof(SetBounds_Int_Int_Int_Int_WithParentWithHandle_TestData))]
        public void Control_Bounds_SetWithParentWithHandle_GetReturnsExpected(bool resizeRedraw, int x, int y, int width, int height, int expectedWidth, int expectedHeight, int expectedLayoutCallCount, int expectedInvalidatedCallCount, int expectedParentLayoutCallCount1, int expectedParentLayoutCallCount2)
        {
            using var parent = new Control();
            using var control = new SubControl
            {
                Parent = parent
            };
            control.SetStyle(ControlStyles.ResizeRedraw, resizeRedraw);
            int layoutCallCount = 0;
            int resizeCallCount = 0;
            int sizeChangedCallCount = 0;
            int clientSizeChangedCallCount = 0;
            int parentLayoutCallCount = 0;
            control.Layout += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(control, e.AffectedControl);
                Assert.Equal("Bounds", e.AffectedProperty);
                Assert.Equal(resizeCallCount, layoutCallCount);
                Assert.Equal(sizeChangedCallCount, layoutCallCount);
                Assert.Equal(clientSizeChangedCallCount, layoutCallCount);
                Assert.Equal(parentLayoutCallCount, layoutCallCount);
                layoutCallCount++;
            };
            control.Resize += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                Assert.Equal(layoutCallCount - 1, resizeCallCount);
                Assert.Equal(sizeChangedCallCount, resizeCallCount);
                Assert.Equal(clientSizeChangedCallCount, resizeCallCount);
                Assert.Equal(parentLayoutCallCount, resizeCallCount);
                resizeCallCount++;
            };
            control.SizeChanged += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                Assert.Equal(resizeCallCount - 1, sizeChangedCallCount);
                Assert.Equal(layoutCallCount - 1, sizeChangedCallCount);
                Assert.Equal(clientSizeChangedCallCount, sizeChangedCallCount);
                Assert.Equal(parentLayoutCallCount, sizeChangedCallCount);
                sizeChangedCallCount++;
            };
            control.ClientSizeChanged += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                Assert.Equal(resizeCallCount - 1, clientSizeChangedCallCount);
                Assert.Equal(layoutCallCount - 1, clientSizeChangedCallCount);
                Assert.Equal(sizeChangedCallCount - 1, clientSizeChangedCallCount);
                Assert.Equal(parentLayoutCallCount, clientSizeChangedCallCount);
                clientSizeChangedCallCount++;
            };
            void parentHandler(object sender, LayoutEventArgs e)
            {
                Assert.Same(parent, sender);
                Assert.Same(control, e.AffectedControl);
                Assert.Equal("Bounds", e.AffectedProperty);
                parentLayoutCallCount++;
            };
            parent.Layout += parentHandler;
            Assert.NotEqual(IntPtr.Zero, parent.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;
            int parentInvalidatedCallCount = 0;
            parent.Invalidated += (sender, e) => parentInvalidatedCallCount++;
            int parentStyleChangedCallCount = 0;
            parent.StyleChanged += (sender, e) => parentStyleChangedCallCount++;
            int parentCreatedCallCount = 0;
            parent.HandleCreated += (sender, e) => parentCreatedCallCount++;

            try
            {
                control.Bounds = new Rectangle(x, y, width, height);
                Assert.Equal(new Size(expectedWidth, expectedHeight), control.ClientSize);
                Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.ClientRectangle);
                Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.DisplayRectangle);
                Assert.Equal(new Size(expectedWidth, expectedHeight), control.Size);
                Assert.Equal(x, control.Left);
                Assert.Equal(x + expectedWidth, control.Right);
                Assert.Equal(y, control.Top);
                Assert.Equal(y + expectedHeight, control.Bottom);
                Assert.Equal(expectedWidth, control.Width);
                Assert.Equal(expectedHeight, control.Height);
                Assert.Equal(new Rectangle(x, y, expectedWidth, expectedHeight), control.Bounds);
                Assert.Equal(expectedLayoutCallCount, layoutCallCount);
                Assert.Equal(expectedParentLayoutCallCount1, parentLayoutCallCount);
                Assert.Equal(expectedLayoutCallCount, resizeCallCount);
                Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
                Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
                Assert.True(control.IsHandleCreated);
                Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
                Assert.Equal(0, styleChangedCallCount);
                Assert.Equal(0, createdCallCount);
                Assert.True(parent.IsHandleCreated);
                Assert.Equal(0, parentInvalidatedCallCount);
                Assert.Equal(0, parentStyleChangedCallCount);
                Assert.Equal(0, parentCreatedCallCount);

                // Call again.
                control.Bounds = new Rectangle(x, y, width, height);
                Assert.Equal(new Size(expectedWidth, expectedHeight), control.ClientSize);
                Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.ClientRectangle);
                Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.DisplayRectangle);
                Assert.Equal(new Size(expectedWidth, expectedHeight), control.Size);
                Assert.Equal(x, control.Left);
                Assert.Equal(x + expectedWidth, control.Right);
                Assert.Equal(y, control.Top);
                Assert.Equal(y + expectedHeight, control.Bottom);
                Assert.Equal(expectedWidth, control.Width);
                Assert.Equal(expectedHeight, control.Height);
                Assert.Equal(new Rectangle(x, y, expectedWidth, expectedHeight), control.Bounds);
                Assert.Equal(expectedLayoutCallCount, layoutCallCount);
                Assert.Equal(expectedParentLayoutCallCount2, parentLayoutCallCount);
                Assert.Equal(expectedLayoutCallCount, resizeCallCount);
                Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
                Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
                Assert.True(control.IsHandleCreated);
                Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
                Assert.Equal(0, styleChangedCallCount);
                Assert.Equal(0, createdCallCount);
                Assert.True(parent.IsHandleCreated);
                Assert.Equal(0, parentInvalidatedCallCount);
                Assert.Equal(0, parentStyleChangedCallCount);
                Assert.Equal(0, parentCreatedCallCount);
            }
            finally
            {
                parent.Layout -= parentHandler;
            }
        }

        /// <summary>
        ///  Data for the CaptureGetSet test
        /// </summary>
        public static TheoryData<bool> CaptureGetSetData =>
            CommonTestHelper.GetBoolTheoryData();

        [Theory]
        [MemberData(nameof(CaptureGetSetData))]
        public void Control_CaptureGetSet(bool expected)
        {
            var cont = new Control
            {
                Capture = expected
            };

            Assert.Equal(expected, cont.Capture);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void Control_CausesValidation_Set_GetReturnsExpected(bool value)
        {
            using var control = new Control
            {
                CausesValidation = value
            };
            Assert.Equal(value, control.CausesValidation);
            Assert.False(control.IsHandleCreated);

            // Set same
            control.CausesValidation = value;
            Assert.Equal(value, control.CausesValidation);
            Assert.False(control.IsHandleCreated);

            // Set different
            control.CausesValidation = !value;
            Assert.Equal(!value, control.CausesValidation);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void Control_CausesValidation_SetWithHandle_GetReturnsExpected(bool value)
        {
            using var control = new Control();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.CausesValidation = value;
            Assert.Equal(value, control.CausesValidation);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same
            control.CausesValidation = value;
            Assert.Equal(value, control.CausesValidation);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set different
            control.CausesValidation = !value;
            Assert.Equal(!value, control.CausesValidation);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [Fact]
        public void Control_CausesValidation_SetWithHandler_CallsCausesValidationChanged()
        {
            using var control = new Control
            {
                CausesValidation = true
            };
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            control.CausesValidationChanged += handler;

            // Set different.
            control.CausesValidation = false;
            Assert.False(control.CausesValidation);
            Assert.Equal(1, callCount);

            // Set same.
            control.CausesValidation = false;
            Assert.False(control.CausesValidation);
            Assert.Equal(1, callCount);

            // Set different.
            control.CausesValidation = true;
            Assert.True(control.CausesValidation);
            Assert.Equal(2, callCount);

            // Remove handler.
            control.CausesValidationChanged -= handler;
            control.CausesValidation = false;
            Assert.False(control.CausesValidation);
            Assert.Equal(2, callCount);
        }

        [Theory]
        [InlineData(false, false)]
        [InlineData(true, false)] // giving true cannot set to true
        public void Control_CacheTextInternalGetSet(bool given, bool expected)
        {
            var cont = new Control
            {
                CacheTextInternal = given
            };

            Assert.Equal(expected, cont.CacheTextInternal);
        }

        public static IEnumerable<object[]> ClientSize_Get_TestData()
        {
            yield return new object[] { new Control(), Size.Empty };
            yield return new object[] { new NonZeroWidthBorderedControl(), Size.Empty };
            yield return new object[] { new NonZeroHeightBorderedControl(), Size.Empty };
            yield return new object[] { new NonZeroWidthNonZeroHeightBorderedControl(), new Size(6, 16) };
        }

        [WinFormsTheory]
        [MemberData(nameof(ClientSize_Get_TestData))]
        public void Control_ClientSize_Get_ReturnsExpected(Control control, Size expected)
        {
            Assert.Equal(expected, control.ClientSize);
        }

        private class NonZeroWidthBorderedControl : BorderedControl
        {
            protected override Size DefaultSize => new Size(10, 0);
        }

        private class NonZeroHeightBorderedControl : BorderedControl
        {
            protected override Size DefaultSize => new Size(0, 10);
        }

        private class NonZeroWidthNonZeroHeightBorderedControl : BorderedControl
        {
            protected override Size DefaultSize => new Size(10, 20);
        }

        public static IEnumerable<object[]> ClientSize_Set_TestData()
        {
            yield return new object[] { new Size(10, 20), 1 };
            yield return new object[] { new Size(1, 2), 1 };
            yield return new object[] { new Size(0, 0), 0 };
            yield return new object[] { new Size(-1, -2), 1 };
            yield return new object[] { new Size(-1, 2), 1 };
            yield return new object[] { new Size(1, -2), 1 };
        }

        [WinFormsTheory]
        [MemberData(nameof(ClientSize_Set_TestData))]
        public void Control_ClientSize_Set_GetReturnsExpected(Size value, int expectedLayoutCallCount)
        {
            using var control = new Control();
            int layoutCallCount = 0;
            control.Layout += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(control, e.AffectedControl);
                Assert.Equal("Bounds", e.AffectedProperty);
                layoutCallCount++;
            };

            control.ClientSize = value;
            Assert.Equal(value, control.ClientSize);
            Assert.Equal(new Rectangle(Point.Empty, value), control.ClientRectangle);
            Assert.Equal(new Rectangle(Point.Empty, value), control.DisplayRectangle);
            Assert.Equal(value, control.Size);
            Assert.Equal(new Rectangle(Point.Empty, value), control.Bounds);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.ClientSize = value;
            Assert.Equal(value, control.ClientSize);
            Assert.Equal(new Rectangle(Point.Empty, value), control.ClientRectangle);
            Assert.Equal(new Rectangle(Point.Empty, value), control.DisplayRectangle);
            Assert.Equal(value, control.Size);
            Assert.Equal(new Rectangle(Point.Empty, value), control.Bounds);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.False(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> ClientSize_SetWithCustomStyle_TestData()
        {
            yield return new object[] { new Size(10, 20), new Size(14, 24) };
            yield return new object[] { new Size(1, 2), new Size(5, 6) };
            yield return new object[] { new Size(0, 0), new Size(4, 4) };
            yield return new object[] { new Size(-1, -2), new Size(3, 2) };
            yield return new object[] { new Size(-1, 2), new Size(3, 6) };
            yield return new object[] { new Size(1, -2), new Size(5, 2) };
        }

        [WinFormsTheory]
        [MemberData(nameof(ClientSize_SetWithCustomStyle_TestData))]
        public void Control_ClientSize_SetWithCustomStyle_GetReturnsExpected(Size value, Size expectedSize)
        {
            using var control = new BorderedControl();
            int layoutCallCount = 0;
            control.Layout += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(control, e.AffectedControl);
                Assert.Equal("Bounds", e.AffectedProperty);
                layoutCallCount++;
            };

            control.ClientSize = value;
            Assert.Equal(value, control.ClientSize);
            Assert.Equal(new Rectangle(Point.Empty, value), control.ClientRectangle);
            Assert.Equal(new Rectangle(Point.Empty, value), control.DisplayRectangle);
            Assert.Equal(expectedSize, control.Size);
            Assert.Equal(0, control.Left);
            Assert.Equal(expectedSize.Width, control.Right);
            Assert.Equal(0, control.Top);
            Assert.Equal(expectedSize.Height, control.Bottom);
            Assert.Equal(expectedSize.Width, control.Width);
            Assert.Equal(expectedSize.Height, control.Height);
            Assert.Equal(new Rectangle(Point.Empty, expectedSize), control.Bounds);
            Assert.Equal(1, layoutCallCount);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.ClientSize = value;
            Assert.Equal(value, control.ClientSize);
            Assert.Equal(new Rectangle(Point.Empty, value), control.ClientRectangle);
            Assert.Equal(new Rectangle(Point.Empty, value), control.DisplayRectangle);
            Assert.Equal(expectedSize, control.Size);
            Assert.Equal(0, control.Left);
            Assert.Equal(expectedSize.Width, control.Right);
            Assert.Equal(0, control.Top);
            Assert.Equal(expectedSize.Height, control.Bottom);
            Assert.Equal(expectedSize.Width, control.Width);
            Assert.Equal(expectedSize.Height, control.Height);
            Assert.Equal(new Rectangle(Point.Empty, expectedSize), control.Bounds);
            Assert.Equal(1, layoutCallCount);
            Assert.False(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> ClientSize_SetWithHandle_TestData()
        {
            yield return new object[] { true, new Size(1, 2), new Size(1, 2), 1, 1 };
            yield return new object[] { true, new Size(0, 0), new Size(0, 0), 0, 0 };
            yield return new object[] { true, new Size(-1, -2), new Size(0, 0), 0, 0 };
            yield return new object[] { true, new Size(-1, 2), new Size(0, 2), 1, 1 };
            yield return new object[] { true, new Size(1, -2), new Size(1, 0), 1, 1 };

            yield return new object[] { false, new Size(1, 2), new Size(1, 2), 1, 0 };
            yield return new object[] { false, new Size(0, 0), new Size(0, 0), 0, 0 };
            yield return new object[] { false, new Size(-1, -2), new Size(0, 0), 0, 0 };
            yield return new object[] { false, new Size(-1, 2), new Size(0, 2), 1, 0 };
            yield return new object[] { false, new Size(1, -2), new Size(1, 0), 1, 0 };
        }

        [WinFormsTheory]
        [MemberData(nameof(ClientSize_SetWithHandle_TestData))]
        public void Control_ClientSize_SetWithHandle_GetReturnsExpected(bool resizeRedraw, Size value, Size expectedSize, int expectedLayoutCallCount, int expectedInvalidatedCallCount)
        {
            using var control = new SubControl();
            control.SetStyle(ControlStyles.ResizeRedraw, resizeRedraw);
            int layoutCallCount = 0;
            control.Layout += (sender, e) => layoutCallCount++;
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.ClientSize = value;
            Assert.Equal(value, control.ClientSize);
            Assert.Equal(new Rectangle(Point.Empty, value), control.ClientRectangle);
            Assert.Equal(new Rectangle(Point.Empty, value), control.DisplayRectangle);
            Assert.Equal(expectedSize, control.Size);
            Assert.Equal(0, control.Left);
            Assert.Equal(expectedSize.Width, control.Right);
            Assert.Equal(0, control.Top);
            Assert.Equal(expectedSize.Height, control.Bottom);
            Assert.Equal(expectedSize.Width, control.Width);
            Assert.Equal(expectedSize.Height, control.Height);
            Assert.Equal(new Rectangle(Point.Empty, expectedSize), control.Bounds);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.ClientSize = value;
            Assert.Equal(value, control.ClientSize);
            Assert.Equal(new Rectangle(Point.Empty, value), control.ClientRectangle);
            Assert.Equal(new Rectangle(Point.Empty, value), control.DisplayRectangle);
            Assert.Equal(expectedSize, control.Size);
            Assert.Equal(0, control.Left);
            Assert.Equal(expectedSize.Width, control.Right);
            Assert.Equal(0, control.Top);
            Assert.Equal(expectedSize.Height, control.Bottom);
            Assert.Equal(expectedSize.Width, control.Width);
            Assert.Equal(expectedSize.Height, control.Height);
            Assert.Equal(new Rectangle(Point.Empty, expectedSize), control.Bounds);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void Control_ClientSize_SetWithHandler_CallsClientSizeChanged()
        {
            using var control = new Control();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            int sizeChangedCallCount = 0;
            EventHandler sizeChangedHandler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                sizeChangedCallCount++;
            };
            control.ClientSizeChanged += handler;
            control.SizeChanged += sizeChangedHandler;

            control.ClientSize = new Size(10, 10);
            Assert.Equal(new Size(10, 10), control.ClientSize);
            Assert.Equal(2, callCount);
            Assert.Equal(1, sizeChangedCallCount);

            // Set same.
            control.ClientSize = new Size(10, 10);
            Assert.Equal(new Size(10, 10), control.ClientSize);
            Assert.Equal(3, callCount);
            Assert.Equal(1, sizeChangedCallCount);

            // Set different.
            control.ClientSize = new Size(11, 11);
            Assert.Equal(new Size(11, 11), control.ClientSize);
            Assert.Equal(5, callCount);
            Assert.Equal(2, sizeChangedCallCount);

            // Remove handler.
            control.ClientSizeChanged -= handler;
            control.SizeChanged -= sizeChangedHandler;
            control.ClientSize = new Size(10, 10);
            Assert.Equal(new Size(10, 10), control.ClientSize);
            Assert.Equal(5, callCount);
            Assert.Equal(2, sizeChangedCallCount);
        }

        public static IEnumerable<object[]> ContextMenuStrip_Set_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new ContextMenuStrip() };
        }

        [Theory]
        [MemberData(nameof(ContextMenuStrip_Set_TestData))]
        public void Control_ContextMenuStrip_Set_GetReturnsExpected(ContextMenuStrip value)
        {
            var control = new Control
            {
                ContextMenuStrip = value
            };
            Assert.Same(value, control.ContextMenuStrip);

            // Set same.
            control.ContextMenuStrip = value;
            Assert.Same(value, control.ContextMenuStrip);
        }

        [Theory]
        [MemberData(nameof(ContextMenuStrip_Set_TestData))]
        public void Control_ContextMenuStrip_SetWithNonNullOldValue_GetReturnsExpected(ContextMenuStrip value)
        {
            var control = new Control
            {
                ContextMenuStrip = new ContextMenuStrip()
            };
            control.ContextMenuStrip = value;
            Assert.Same(value, control.ContextMenuStrip);

            // Set same.
            control.ContextMenuStrip = value;
            Assert.Same(value, control.ContextMenuStrip);
        }

        [Fact]
        public void Control_ContextMenuStrip_SetDisposeNew_RemovesContextMenuStrip()
        {
            var menu = new ContextMenuStrip();
            var control = new Control
            {
                ContextMenuStrip = menu
            };
            Assert.Same(menu, control.ContextMenuStrip);

            menu.Dispose();
            Assert.Null(control.ContextMenuStrip);
        }

        [Fact]
        public void Control_ContextMenuStrip_SetDisposeOld_RemovesContextMenuStrip()
        {
            var menu1 = new ContextMenuStrip();
            var menu2 = new ContextMenuStrip();
            var control = new Control
            {
                ContextMenuStrip = menu1
            };
            Assert.Same(menu1, control.ContextMenuStrip);

            control.ContextMenuStrip = menu2;
            Assert.Same(menu2, control.ContextMenuStrip);

            menu1.Dispose();
            Assert.Same(menu2, control.ContextMenuStrip);
        }

        [Fact]
        public void Control_ContextMenuStrip_SetWithHandler_CallsContextMenuStripChanged()
        {
            var control = new Control();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            control.ContextMenuStripChanged += handler;

            // Set different.
            var menu1 = new ContextMenuStrip();
            control.ContextMenuStrip = menu1;
            Assert.Same(menu1, control.ContextMenuStrip);
            Assert.Equal(1, callCount);

            // Set same.
            control.ContextMenuStrip = menu1;
            Assert.Same(menu1, control.ContextMenuStrip);
            Assert.Equal(1, callCount);

            // Set different.
            var menu2 = new ContextMenuStrip();
            control.ContextMenuStrip = menu2;
            Assert.Same(menu2, control.ContextMenuStrip);
            Assert.Equal(2, callCount);

            // Set null.
            control.ContextMenuStrip = null;
            Assert.Null(control.ContextMenuStrip);
            Assert.Equal(3, callCount);

            // Remove handler.
            control.ContextMenuStripChanged -= handler;
            control.ContextMenuStrip = menu1;
            Assert.Same(menu1, control.ContextMenuStrip);
            Assert.Equal(3, callCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetCursorTheoryData))]
        public void Control_Cursor_Set_GetReturnsExpected(Cursor value)
        {
            var control = new Control
            {
                Cursor = value
            };
            Assert.Same(value ?? Cursors.Default, control.Cursor);

            // Set same.
            control.Cursor = value;
            Assert.Same(value ?? Cursors.Default, control.Cursor);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetCursorTheoryData))]
        public void Control_Cursor_SetWithHandle_GetReturnsExpected(Cursor value)
        {
            var control = new Control();
            Assert.NotEqual(IntPtr.Zero, control.Handle);

            control.Cursor = value;
            Assert.Same(value ?? Cursors.Default, control.Cursor);

            // Set same.
            control.Cursor = value;
            Assert.Same(value ?? Cursors.Default, control.Cursor);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetCursorTheoryData))]
        public void Control_Cursor_SetWithChildren_GetReturnsExpected(Cursor value)
        {
            var child1 = new Control();
            var child2 = new Control();
            var control = new Control();
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

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetCursorTheoryData))]
        public void Control_Cursor_SetWithChildrenWithCursor_GetReturnsExpected(Cursor value)
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
            var control = new Control();
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

        [Fact]
        public void Control_Cursor_SetWithHandler_CallsCursorChanged()
        {
            var control = new Control();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            control.CursorChanged += handler;

            // Set different.
            var cursor1 = new Cursor((IntPtr)1);
            control.Cursor = cursor1;
            Assert.Same(cursor1, control.Cursor);
            Assert.Equal(1, callCount);

            // Set same.
            control.Cursor = cursor1;
            Assert.Same(cursor1, control.Cursor);
            Assert.Equal(1, callCount);

            // Set different.
            var cursor2 = new Cursor((IntPtr)2);
            control.Cursor = cursor2;
            Assert.Same(cursor2, control.Cursor);
            Assert.Equal(2, callCount);

            // Set null.
            control.Cursor = null;
            Assert.Same(Cursors.Default, control.Cursor);
            Assert.Equal(3, callCount);

            // Remove handler.
            control.CursorChanged -= handler;
            control.Cursor = cursor1;
            Assert.Same(cursor1, control.Cursor);
            Assert.Equal(3, callCount);
        }

        [Fact]
        public void Control_Cursor_SetWithChildrenWithHandler_CallsCursorChanged()
        {
            var child1 = new Control();
            var child2 = new Control();
            var control = new Control();
            control.Controls.Add(child1);
            control.Controls.Add(child2);

            int callCount = 0;
            int child1CallCount = 0;
            int child2CallCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            EventHandler childHandler1 = (sender, e) =>
            {
                Assert.Same(child1, sender);
                Assert.Same(EventArgs.Empty, e);
                child1CallCount++;
            };
            EventHandler childHandler2 = (sender, e) =>
            {
                Assert.Same(child2, sender);
                Assert.Same(EventArgs.Empty, e);
                child2CallCount++;
            };
            control.CursorChanged += handler;
            child1.CursorChanged += childHandler1;
            child2.CursorChanged += childHandler2;

            // Set different.
            var cursor1 = new Cursor((IntPtr)1);
            control.Cursor = cursor1;
            Assert.Same(cursor1, control.Cursor);
            Assert.Same(cursor1, child1.Cursor);
            Assert.Same(cursor1, child2.Cursor);
            Assert.Equal(1, callCount);
            Assert.Equal(1, child1CallCount);
            Assert.Equal(1, child2CallCount);

            // Set same.
            control.Cursor = cursor1;
            Assert.Same(cursor1, control.Cursor);
            Assert.Same(cursor1, child1.Cursor);
            Assert.Same(cursor1, child2.Cursor);
            Assert.Equal(1, callCount);
            Assert.Equal(1, child1CallCount);
            Assert.Equal(1, child2CallCount);

            // Set different.
            var cursor2 = new Cursor((IntPtr)2);
            control.Cursor = cursor2;
            Assert.Same(cursor2, control.Cursor);
            Assert.Same(cursor2, child1.Cursor);
            Assert.Same(cursor2, child2.Cursor);
            Assert.Equal(2, callCount);
            Assert.Equal(2, child1CallCount);
            Assert.Equal(2, child2CallCount);

            // Set null.
            control.Cursor = null;
            Assert.Same(Cursors.Default, control.Cursor);
            Assert.Same(Cursors.Default, child1.Cursor);
            Assert.Same(Cursors.Default, child2.Cursor);
            Assert.Equal(3, callCount);
            Assert.Equal(3, child1CallCount);
            Assert.Equal(3, child2CallCount);

            // Remove handler.
            control.CursorChanged -= handler;
            child1.CursorChanged -= childHandler1;
            child2.CursorChanged -= childHandler2;
            control.Cursor = cursor1;
            Assert.Same(cursor1, control.Cursor);
            Assert.Same(cursor1, child1.Cursor);
            Assert.Same(cursor1, child2.Cursor);
            Assert.Equal(3, callCount);
            Assert.Equal(3, child1CallCount);
            Assert.Equal(3, child2CallCount);
        }

        [Fact]
        public void Control_Cursor_SetWithChildrenWithCursorWithHandler_CallsCursorChanged()
        {
            var childCursor1 = new Cursor((IntPtr)1);
            var childCursor2 = new Cursor((IntPtr)2);
            var child1 = new Control
            {
                Cursor = childCursor1
            };
            var child2 = new Control
            {
                Cursor = childCursor2
            };
            var control = new Control();
            control.Controls.Add(child1);
            control.Controls.Add(child2);

            int callCount = 0;
            int child1CallCount = 0;
            int child2CallCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            EventHandler childHandler1 = (sender, e) =>
            {
                Assert.Same(child1, sender);
                Assert.Same(EventArgs.Empty, e);
                child1CallCount++;
            };
            EventHandler childHandler2 = (sender, e) =>
            {
                Assert.Same(child2, sender);
                Assert.Same(EventArgs.Empty, e);
                child2CallCount++;
            };
            control.CursorChanged += handler;
            child1.CursorChanged += childHandler1;
            child2.CursorChanged += childHandler2;

            // Set different.
            var cursor1 = new Cursor((IntPtr)3);
            control.Cursor = cursor1;
            Assert.Same(cursor1, control.Cursor);
            Assert.Same(childCursor1, child1.Cursor);
            Assert.Same(childCursor2, child2.Cursor);
            Assert.Equal(1, callCount);
            Assert.Equal(0, child1CallCount);
            Assert.Equal(0, child2CallCount);

            // Set same.
            control.Cursor = cursor1;
            Assert.Same(cursor1, control.Cursor);
            Assert.Same(childCursor1, child1.Cursor);
            Assert.Same(childCursor2, child2.Cursor);
            Assert.Equal(1, callCount);
            Assert.Equal(0, child1CallCount);
            Assert.Equal(0, child2CallCount);

            // Set different.
            var cursor2 = new Cursor((IntPtr)4);
            control.Cursor = cursor2;
            Assert.Same(cursor2, control.Cursor);
            Assert.Same(childCursor1, child1.Cursor);
            Assert.Same(childCursor2, child2.Cursor);
            Assert.Equal(2, callCount);
            Assert.Equal(0, child1CallCount);
            Assert.Equal(0, child2CallCount);

            // Set null.
            control.Cursor = null;
            Assert.Same(Cursors.Default, control.Cursor);
            Assert.Same(childCursor1, child1.Cursor);
            Assert.Same(childCursor2, child2.Cursor);
            Assert.Equal(3, callCount);
            Assert.Equal(0, child1CallCount);
            Assert.Equal(0, child2CallCount);

            // Remove handler.
            control.CursorChanged -= handler;
            child1.CursorChanged -= childHandler1;
            child2.CursorChanged -= childHandler2;
            control.Cursor = cursor1;
            Assert.Same(cursor1, control.Cursor);
            Assert.Same(childCursor1, child1.Cursor);
            Assert.Same(childCursor2, child2.Cursor);
            Assert.Equal(3, callCount);
            Assert.Equal(0, child1CallCount);
            Assert.Equal(0, child2CallCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(DockStyle))]
        public void Control_Dock_Set_GetReturnsExpected(DockStyle value)
        {
            var control = new Control
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

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(DockStyle))]
        public void Control_Dock_SetWithOldValue_GetReturnsExpected(DockStyle value)
        {
            var control = new Control
            {
                Dock = DockStyle.Top
            };

            control.Dock = value;
            Assert.Equal(value, control.Dock);
            Assert.Equal(AnchorStyles.Top | AnchorStyles.Left, control.Anchor);

            // Set same.
            control.Dock = value;
            Assert.Equal(value, control.Dock);
            Assert.Equal(AnchorStyles.Top | AnchorStyles.Left, control.Anchor);
        }

        public static IEnumerable<object[]> Dock_SetWithAnchor_TestData()
        {
            foreach (AnchorStyles anchor in Enum.GetValues(typeof(AnchorStyles)))
            {
                foreach (DockStyle value in Enum.GetValues(typeof(DockStyle)))
                {
                    yield return new object[] { anchor, value, value == DockStyle.None ? anchor : AnchorStyles.Top | AnchorStyles.Left };
                }
            }
        }

        [Theory]
        [MemberData(nameof(Dock_SetWithAnchor_TestData))]
        public void Control_Dock_SetWithAnchor_GetReturnsExpected(AnchorStyles anchor, DockStyle value, AnchorStyles expectedAnchor)
        {
            var control = new Control
            {
                Anchor = anchor
            };

            control.Dock = value;
            Assert.Equal(value, control.Dock);
            Assert.Equal(expectedAnchor, control.Anchor);

            // Set same.
            control.Dock = value;
            Assert.Equal(value, control.Dock);
            Assert.Equal(expectedAnchor, control.Anchor);
        }

        [Fact]
        public void Control_Dock_SetWithHandler_CallsDockChanged()
        {
            var control = new Control();
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

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(DockStyle))]
        public void Control_Dock_SetInvalid_ThrowsInvalidEnumArgumentException(DockStyle value)
        {
            var control = new Control();
            Assert.Throws<InvalidEnumArgumentException>("value", () => control.Dock = value);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void Control_DoubleBuffered_Get_ReturnsExpected(bool value)
        {
            var control = new SubControl();
            control.SetStyle(ControlStyles.OptimizedDoubleBuffer, value);
            Assert.Equal(value, control.DoubleBuffered);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void Control_DoubleBuffered_Set_GetReturnsExpected(bool value)
        {
            var control = new SubControl
            {
                DoubleBuffered = value
            };
            Assert.Equal(value, control.DoubleBuffered);
            Assert.Equal(value, control.GetStyle(ControlStyles.OptimizedDoubleBuffer));
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.DoubleBuffered = value;
            Assert.Equal(value, control.DoubleBuffered);
            Assert.Equal(value, control.GetStyle(ControlStyles.OptimizedDoubleBuffer));
            Assert.False(control.IsHandleCreated);

            // Set different.
            control.DoubleBuffered = !value;
            Assert.Equal(!value, control.DoubleBuffered);
            Assert.Equal(!value, control.GetStyle(ControlStyles.OptimizedDoubleBuffer));
            Assert.False(control.IsHandleCreated);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void Control_DoubleBuffered_SetWithHandle_GetReturnsExpected(bool value)
        {
            var control = new SubControl();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.DoubleBuffered = value;
            Assert.Equal(value, control.DoubleBuffered);
            Assert.Equal(value, control.GetStyle(ControlStyles.OptimizedDoubleBuffer));
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.DoubleBuffered = value;
            Assert.Equal(value, control.DoubleBuffered);
            Assert.Equal(value, control.GetStyle(ControlStyles.OptimizedDoubleBuffer));
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set different.
            control.DoubleBuffered = !value;
            Assert.Equal(!value, control.DoubleBuffered);
            Assert.Equal(!value, control.GetStyle(ControlStyles.OptimizedDoubleBuffer));
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }
        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void Control_Enabled_Set_GetReturnsExpected(bool value)
        {
            using var control = new Control
            {
                Enabled = value
            };
            Assert.Equal(value, control.Enabled);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.Enabled = value;
            Assert.Equal(value, control.Enabled);
            Assert.False(control.IsHandleCreated);

            // Set different.
            control.Enabled = !value;
            Assert.Equal(!value, control.Enabled);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, false, 1, 2)]
        [InlineData(true, true, 0, 1)]
        [InlineData(false, false, 0, 0)]
        [InlineData(false, true, 0, 0)]
        public void Control_Enabled_SetWithHandle_GetReturnsExpected(bool userPaint, bool value, int expectedInvalidateCallCount1, int expectedInvalidateCallCount2)
        {
            using var control = new SubControl();
            control.SetStyle(ControlStyles.UserPaint, userPaint);
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;
            Assert.Equal(userPaint, control.GetStyle(ControlStyles.UserPaint));

            control.Enabled = value;
            Assert.Equal(value, control.Enabled);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidateCallCount1, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.Enabled = value;
            Assert.Equal(value, control.Enabled);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidateCallCount1, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set different.
            control.Enabled = !value;
            Assert.Equal(!value, control.Enabled);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidateCallCount2, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void Control_Enabled_SetWithHandler_CallsEnabledChanged()
        {
            using var control = new Control
            {
                Enabled = true
            };
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            control.EnabledChanged += handler;

            // Set different.
            control.Enabled = false;
            Assert.False(control.Enabled);
            Assert.Equal(1, callCount);

            // Set same.
            control.Enabled = false;
            Assert.False(control.Enabled);
            Assert.Equal(1, callCount);

            // Set different.
            control.Enabled = true;
            Assert.True(control.Enabled);
            Assert.Equal(2, callCount);

            // Remove handler.
            control.EnabledChanged -= handler;
            control.Enabled = false;
            Assert.False(control.Enabled);
            Assert.Equal(2, callCount);
        }

        [WinFormsFact]
        public void Control_Enabled_SetWithChildrenWithHandler_CallsEnabledChanged()
        {
            using var child1 = new Control();
            using var child2 = new Control();
            using var control = new Control
            {
                Enabled = true
            };
            control.Controls.Add(child1);
            control.Controls.Add(child2);

            int callCount = 0;
            int childCallCount1 = 0;
            int childCallCount2 = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            EventHandler childHandler1 = (sender, e) =>
            {
                Assert.Same(child1, sender);
                Assert.Same(EventArgs.Empty, e);
                childCallCount1++;
            };
            EventHandler childHandler2 = (sender, e) =>
            {
                Assert.Same(child2, sender);
                Assert.Same(EventArgs.Empty, e);
                childCallCount2++;
            };
            control.EnabledChanged += handler;
            child1.EnabledChanged += childHandler1;
            child2.EnabledChanged += childHandler2;

            // Set different.
            control.Enabled = false;
            Assert.False(control.Enabled);
            Assert.False(child1.Enabled);
            Assert.False(child2.Enabled);
            Assert.Equal(1, callCount);
            Assert.Equal(1, childCallCount1);
            Assert.Equal(1, childCallCount2);

            // Set same.
            control.Enabled = false;
            Assert.False(control.Enabled);
            Assert.False(child1.Enabled);
            Assert.False(child2.Enabled);
            Assert.Equal(1, callCount);
            Assert.Equal(1, childCallCount1);
            Assert.Equal(1, childCallCount2);

            // Set different.
            control.Enabled = true;
            Assert.True(control.Enabled);
            Assert.True(child1.Enabled);
            Assert.True(child2.Enabled);
            Assert.Equal(2, callCount);
            Assert.Equal(2, childCallCount1);
            Assert.Equal(2, childCallCount2);

            // Remove handler.
            control.EnabledChanged -= handler;
            child1.EnabledChanged -= childHandler1;
            child2.EnabledChanged -= childHandler2;
            control.Enabled = false;
            Assert.False(control.Enabled);
            Assert.False(child1.Enabled);
            Assert.False(child2.Enabled);
            Assert.Equal(2, callCount);
            Assert.Equal(2, childCallCount1);
            Assert.Equal(2, childCallCount2);
        }

        [WinFormsFact]
        public void Control_Enabled_SetWithChildrenDisabledWithHandler_CallsEnabledChanged()
        {
            using var child1 = new Control
            {
                Enabled = false
            };
            using var child2 = new Control
            {
                Enabled = false
            };
            using var control = new Control
            {
                Enabled = true
            };
            control.Controls.Add(child1);
            control.Controls.Add(child2);

            int callCount = 0;
            int childCallCount1 = 0;
            int childCallCount2 = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            EventHandler childHandler1 = (sender, e) =>
            {
                Assert.Same(child1, sender);
                Assert.Same(EventArgs.Empty, e);
                childCallCount1++;
            };
            EventHandler childHandler2 = (sender, e) =>
            {
                Assert.Same(child2, sender);
                Assert.Same(EventArgs.Empty, e);
                childCallCount2++;
            };
            control.EnabledChanged += handler;
            child1.EnabledChanged += childHandler1;
            child2.EnabledChanged += childHandler2;

            // Set different.
            control.Enabled = false;
            Assert.False(control.Enabled);
            Assert.False(child1.Enabled);
            Assert.False(child2.Enabled);
            Assert.Equal(1, callCount);
            Assert.Equal(0, childCallCount1);
            Assert.Equal(0, childCallCount2);

            // Set same.
            control.Enabled = false;
            Assert.False(control.Enabled);
            Assert.False(child1.Enabled);
            Assert.False(child2.Enabled);
            Assert.Equal(1, callCount);
            Assert.Equal(0, childCallCount1);
            Assert.Equal(0, childCallCount2);

            // Set different.
            control.Enabled = true;
            Assert.True(control.Enabled);
            Assert.False(child1.Enabled);
            Assert.False(child2.Enabled);
            Assert.Equal(2, callCount);
            Assert.Equal(0, childCallCount1);
            Assert.Equal(0, childCallCount2);

            // Remove handler.
            control.EnabledChanged -= handler;
            child1.EnabledChanged -= childHandler1;
            child2.EnabledChanged -= childHandler2;
            control.Enabled = false;
            Assert.False(control.Enabled);
            Assert.False(child1.Enabled);
            Assert.False(child2.Enabled);
            Assert.Equal(2, callCount);
            Assert.Equal(0, childCallCount1);
            Assert.Equal(0, childCallCount2);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetFontTheoryData))]
        public void Control_Font_Set_GetReturnsExpected(Font value)
        {
            var control = new SubControl
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

        public static IEnumerable<object[]> Font_SetWithFontHeight_TestData()
        {
            var font = new Font("Arial", 8.25f);
            yield return new object[] { null, 10 };
            yield return new object[] { font, font.Height };
        }

        [WinFormsTheory]
        [MemberData(nameof(Font_SetWithFontHeight_TestData))]
        public void Control_Font_SetWithFontHeight_GetReturnsExpected(Font value, int expectedFontHeight)
        {
            var control = new SubControl
            {
                FontHeight = 10,
                Font = value
            };
            Assert.Equal(value ?? Control.DefaultFont, control.Font);
            Assert.Equal(expectedFontHeight, control.FontHeight);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.Font = value;
            Assert.Equal(value ?? Control.DefaultFont, control.Font);
            Assert.Equal(expectedFontHeight, control.FontHeight);
            Assert.False(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> Font_SetNonNullOldValueWithFontHeight_TestData()
        {
            var font = new Font("Arial", 8.25f);
            yield return new object[] { null, Control.DefaultFont.Height };
            yield return new object[] { font, font.Height };
        }

        [WinFormsTheory]
        [MemberData(nameof(Font_SetNonNullOldValueWithFontHeight_TestData))]
        public void Control_Font_SetNonNullOldValueWithFontHeight_GetReturnsExpected(Font value, int expectedFontHeight)
        {
            var control = new SubControl
            {
                FontHeight = 10,
                Font = new Font("Arial", 1)
            };

            control.Font = value;
            Assert.Equal(value ?? Control.DefaultFont, control.Font);
            Assert.Equal(expectedFontHeight, control.FontHeight);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.Font = value;
            Assert.Equal(value ?? Control.DefaultFont, control.Font);
            Assert.Equal(expectedFontHeight, control.FontHeight);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetFontTheoryData))]
        public void Control_Font_SetWithNonNullOldValue_GetReturnsExpected(Font value)
        {
            var control = new SubControl
            {
                Font = new Font("Arial", 1)
            };

            control.Font = value;
            Assert.Equal(value ?? Control.DefaultFont, control.Font);
            Assert.Equal(control.Font.Height, control.FontHeight);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.Font = value;
            Assert.Equal(value ?? Control.DefaultFont, control.Font);
            Assert.Equal(control.Font.Height, control.FontHeight);
            Assert.False(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> Font_SetWithHandle_TestData()
        {
            foreach (bool userPaint in new bool[] { true, false })
            {
                yield return new object[] { userPaint, new Font("Arial", 8.25f), 1 };
                yield return new object[] { userPaint, null, 0 };
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(Font_SetWithHandle_TestData))]
        public void Control_Font_SetWithHandle_GetReturnsExpected(bool userPaint, Font value, int expectedInvalidatedCallCount)
        {
            var control = new SubControl();
            control.SetStyle(ControlStyles.UserPaint, userPaint);
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            Assert.Equal(userPaint, control.GetStyle(ControlStyles.UserPaint));
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            // Set different.
            control.Font = value;
            Assert.Equal(value ?? Control.DefaultFont, control.Font);
            Assert.Equal(control.Font.Height, control.FontHeight);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.Font = value;
            Assert.Equal(value ?? Control.DefaultFont, control.Font);
            Assert.Equal(control.Font.Height, control.FontHeight);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        public static IEnumerable<object[]> Font_SetWithNonNullOldValueWithHandle_TestData()
        {
            foreach (bool userPaint in new bool[] { true, false })
            {
                yield return new object[] { userPaint, new Font("Arial", 8.25f) };
                yield return new object[] { userPaint, null };
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(Font_SetWithNonNullOldValueWithHandle_TestData))]
        public void Control_Font_SetWithNonNullOldValueWithHandle_GetReturnsExpected(bool userPaint, Font value)
        {
            var control = new SubControl
            {
                Font = new Font("Arial", 1)
            };
            control.SetStyle(ControlStyles.UserPaint, userPaint);
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            Assert.Equal(userPaint, control.GetStyle(ControlStyles.UserPaint));
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            // Set different.
            control.Font = value;
            Assert.Equal(value ?? Control.DefaultFont, control.Font);
            Assert.Equal(control.Font.Height, control.FontHeight);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(1, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.Font = value;
            Assert.Equal(value ?? Control.DefaultFont, control.Font);
            Assert.Equal(control.Font.Height, control.FontHeight);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(1, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void Control_Font_SetWithHandler_CallsFontChanged()
        {
            var control = new Control();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            control.FontChanged += handler;

            // Set different.
            Font font1 = new Font("Arial", 8.25f);
            control.Font = font1;
            Assert.Same(font1, control.Font);
            Assert.Equal(1, callCount);

            // Set same.
            control.Font = font1;
            Assert.Same(font1, control.Font);
            Assert.Equal(1, callCount);

            // Set different.
            Font font2 = SystemFonts.DialogFont;
            control.Font = font2;
            Assert.Same(font2, control.Font);
            Assert.Equal(2, callCount);

            // Set null.
            control.Font = null;
            Assert.Equal(Control.DefaultFont, control.Font);
            Assert.Equal(3, callCount);

            // Remove handler.
            control.FontChanged -= handler;
            control.Font = font1;
            Assert.Same(font1, control.Font);
            Assert.Equal(3, callCount);
        }

        [WinFormsFact]
        public void Control_Font_SetWithChildrenWithHandler_CallsFontChanged()
        {
            var child1 = new Control();
            var child2 = new Control();
            var control = new Control();
            control.Controls.Add(child1);
            control.Controls.Add(child2);

            int callCount = 0;
            int childCallCount1 = 0;
            int childCallCount2 = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            EventHandler childHandler1 = (sender, e) =>
            {
                Assert.Same(child1, sender);
                Assert.Same(EventArgs.Empty, e);
                childCallCount1++;
            };
            EventHandler childHandler2 = (sender, e) =>
            {
                Assert.Same(child2, sender);
                Assert.Same(EventArgs.Empty, e);
                childCallCount2++;
            };
            control.FontChanged += handler;
            child1.FontChanged += childHandler1;
            child2.FontChanged += childHandler2;

            // Set different.
            Font font1 = new Font("Arial", 8.25f);
            control.Font = font1;
            Assert.Same(font1, control.Font);
            Assert.Same(font1, child1.Font);
            Assert.Same(font1, child2.Font);
            Assert.Equal(1, callCount);
            Assert.Equal(1, childCallCount1);
            Assert.Equal(1, childCallCount2);

            // Set same.
            control.Font = font1;
            Assert.Same(font1, control.Font);
            Assert.Same(font1, child1.Font);
            Assert.Same(font1, child2.Font);
            Assert.Equal(1, callCount);
            Assert.Equal(1, childCallCount1);
            Assert.Equal(1, childCallCount2);

            // Set different.
            Font font2 = SystemFonts.DialogFont;
            control.Font = font2;
            Assert.Same(font2, control.Font);
            Assert.Same(font2, child1.Font);
            Assert.Same(font2, child2.Font);
            Assert.Equal(2, callCount);
            Assert.Equal(2, childCallCount1);
            Assert.Equal(2, childCallCount2);

            // Set null.
            control.Font = null;
            Assert.Equal(Control.DefaultFont, control.Font);
            Assert.Equal(Control.DefaultFont, child1.Font);
            Assert.Equal(Control.DefaultFont, child2.Font);
            Assert.Equal(3, callCount);
            Assert.Equal(3, childCallCount1);
            Assert.Equal(3, childCallCount2);

            // Remove handler.
            control.FontChanged -= handler;
            child1.FontChanged -= childHandler1;
            child2.FontChanged -= childHandler2;
            control.Font = font1;
            Assert.Same(font1, control.Font);
            Assert.Same(font1, child1.Font);
            Assert.Same(font1, child2.Font);
            Assert.Equal(3, callCount);
            Assert.Equal(3, childCallCount1);
            Assert.Equal(3, childCallCount2);
        }

        [WinFormsFact]
        public void Control_Font_SetWithChildrenWithFontWithHandler_CallsFontChanged()
        {
            var childFont1 = new Font("Arial", 1);
            var childFont2 = new Font("Arial", 1);
            var child1 = new Control
            {
                Font = childFont1
            };
            var child2 = new Control
            {
                Font = childFont2
            };
            var control = new Control();
            control.Controls.Add(child1);
            control.Controls.Add(child2);

            int callCount = 0;
            int childCallCount1 = 0;
            int childCallCount2 = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            EventHandler childHandler1 = (sender, e) =>
            {
                Assert.Same(child1, sender);
                Assert.Same(EventArgs.Empty, e);
                childCallCount1++;
            };
            EventHandler childHandler2 = (sender, e) =>
            {
                Assert.Same(child2, sender);
                Assert.Same(EventArgs.Empty, e);
                childCallCount2++;
            };
            control.FontChanged += handler;
            child1.FontChanged += childHandler1;
            child2.FontChanged += childHandler2;

            // Set different.
            Font font1 = new Font("Arial", 8.25f);
            control.Font = font1;
            Assert.Same(font1, control.Font);
            Assert.Same(childFont1, child1.Font);
            Assert.Same(childFont2, child2.Font);
            Assert.Equal(1, callCount);
            Assert.Equal(0, childCallCount1);
            Assert.Equal(0, childCallCount2);

            // Set same.
            control.Font = font1;
            Assert.Same(font1, control.Font);
            Assert.Same(childFont1, child1.Font);
            Assert.Same(childFont2, child2.Font);
            Assert.Equal(1, callCount);
            Assert.Equal(0, childCallCount1);
            Assert.Equal(0, childCallCount2);

            // Set different.
            Font font2 = SystemFonts.DialogFont;
            control.Font = font2;
            Assert.Same(font2, control.Font);
            Assert.Same(childFont1, child1.Font);
            Assert.Same(childFont2, child2.Font);
            Assert.Equal(2, callCount);
            Assert.Equal(0, childCallCount1);
            Assert.Equal(0, childCallCount2);

            // Set null.
            control.Font = null;
            Assert.Equal(Control.DefaultFont, control.Font);
            Assert.Same(childFont1, child1.Font);
            Assert.Same(childFont2, child2.Font);
            Assert.Equal(3, callCount);
            Assert.Equal(0, childCallCount1);
            Assert.Equal(0, childCallCount2);

            // Remove handler.
            control.FontChanged -= handler;
            child1.FontChanged -= childHandler1;
            child2.FontChanged -= childHandler2;
            control.Font = font1;
            Assert.Same(font1, control.Font);
            Assert.Same(childFont1, child1.Font);
            Assert.Same(childFont2, child2.Font);
            Assert.Equal(3, callCount);
            Assert.Equal(0, childCallCount1);
            Assert.Equal(0, childCallCount2);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetForeColorTheoryData))]
        public void Control_ForeColor_Set_GetReturnsExpected(Color value, Color expected)
        {
            using var control = new Control
            {
                ForeColor = value
            };
            Assert.Equal(expected, control.ForeColor);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.ForeColor = value;
            Assert.Equal(expected, control.ForeColor);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetForeColorTheoryData))]
        public void Control_ForeColor_SetWithCustomOldValue_GetReturnsExpected(Color value, Color expected)
        {
            using var control = new Control
            {
                ForeColor = Color.YellowGreen
            };

            control.ForeColor = value;
            Assert.Equal(expected, control.ForeColor);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.ForeColor = value;
            Assert.Equal(expected, control.ForeColor);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetForeColorTheoryData))]
        public void Control_ForeColor_SetWithChildren_GetReturnsExpected(Color value, Color expected)
        {
            using var child1 = new Control();
            using var child2 = new Control();
            using var control = new Control();
            control.Controls.Add(child1);
            control.Controls.Add(child2);

            control.ForeColor = value;
            Assert.Equal(expected, control.ForeColor);
            Assert.Equal(expected, child1.ForeColor);
            Assert.Equal(expected, child2.ForeColor);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.ForeColor = value;
            Assert.Equal(expected, control.ForeColor);
            Assert.Equal(expected, child1.ForeColor);
            Assert.Equal(expected, child2.ForeColor);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetForeColorTheoryData))]
        public void Control_ForeColor_SetWithChildrenWithColor_GetReturnsExpected(Color value, Color expected)
        {
            using var child1 = new Control
            {
                ForeColor = Color.Yellow
            };
            using var child2 = new Control
            {
                ForeColor = Color.YellowGreen
            };
            using var control = new Control();
            control.Controls.Add(child1);
            control.Controls.Add(child2);

            control.ForeColor = value;
            Assert.Equal(expected, control.ForeColor);
            Assert.Equal(Color.Yellow, child1.ForeColor);
            Assert.Equal(Color.YellowGreen, child2.ForeColor);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.ForeColor = value;
            Assert.Equal(expected, control.ForeColor);
            Assert.Equal(Color.Yellow, child1.ForeColor);
            Assert.Equal(Color.YellowGreen, child2.ForeColor);
            Assert.False(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> ForeColor_SetWithHandle_TestData()
        {
            yield return new object[] { Color.Red, Color.Red, 1 };
            yield return new object[] { Color.FromArgb(254, 1, 2, 3), Color.FromArgb(254, 1, 2, 3), 1 };
            yield return new object[] { Color.Empty, Control.DefaultForeColor, 0 };
        }

        [WinFormsTheory]
        [MemberData(nameof(ForeColor_SetWithHandle_TestData))]
        public void Control_ForeColor_SetWithHandle_GetReturnsExpected(Color value, Color expected, int expectedInvalidatedCallCount)
        {
            using var control = new Control();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.ForeColor = value;
            Assert.Equal(expected, control.ForeColor);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.ForeColor = value;
            Assert.Equal(expected, control.ForeColor);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void Control_ForeColor_SetWithHandler_CallsForeColorChanged()
        {
            using var control = new Control();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            control.ForeColorChanged += handler;

            // Set different.
            control.ForeColor = Color.Red;
            Assert.Equal(Color.Red, control.ForeColor);
            Assert.Equal(1, callCount);

            // Set same.
            control.ForeColor = Color.Red;
            Assert.Equal(Color.Red, control.ForeColor);
            Assert.Equal(1, callCount);

            // Set different.
            control.ForeColor = Color.Empty;
            Assert.Equal(Control.DefaultForeColor, control.ForeColor);
            Assert.Equal(2, callCount);

            // Remove handler.
            control.ForeColorChanged -= handler;
            control.ForeColor = Color.Red;
            Assert.Equal(Color.Red, control.ForeColor);
            Assert.Equal(2, callCount);
        }

        [WinFormsFact]
        public void Control_ForeColor_SetWithHandlerInDisposing_DoesNotCallForeColorChanged()
        {
            using var control = new Control();
            Assert.NotEqual(IntPtr.Zero, control.Handle);

            int callCount = 0;
            control.ForeColorChanged += (sender, e) => callCount++;
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;

            int disposedCallCount = 0;
            control.Disposed += (sender, e) =>
            {
                control.ForeColor = Color.Red;
                Assert.Equal(Color.Red, control.ForeColor);
                Assert.Equal(0, callCount);
                Assert.Equal(0, invalidatedCallCount);
                disposedCallCount++;
            };

            control.Dispose();
            Assert.Equal(1, disposedCallCount);
        }

        [WinFormsFact]
        public void Control_ForeColor_SetWithChildrenWithHandler_CallsForeColorChanged()
        {
            using var child1 = new Control();
            using var child2 = new Control();
            using var control = new Control();
            control.Controls.Add(child1);
            control.Controls.Add(child2);

            int callCount = 0;
            int childCallCount1 = 0;
            int childCallCount2 = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                Assert.Equal(callCount, childCallCount1);
                Assert.Equal(childCallCount1, childCallCount2);
                callCount++;
            };
            EventHandler childHandler1 = (sender, e) =>
            {
                Assert.Same(child1, sender);
                Assert.Same(EventArgs.Empty, e);
                Assert.Equal(callCount - 1, childCallCount1);
                Assert.Equal(childCallCount1, childCallCount2);
                childCallCount1++;
            };
            EventHandler childHandler2 = (sender, e) =>
            {
                Assert.Same(child2, sender);
                Assert.Same(EventArgs.Empty, e);
                Assert.Equal(callCount, childCallCount1);
                Assert.Equal(childCallCount1 - 1, childCallCount2);
                childCallCount2++;
            };
            control.ForeColorChanged += handler;
            child1.ForeColorChanged += childHandler1;
            child2.ForeColorChanged += childHandler2;

            // Set different.
            control.ForeColor = Color.Red;
            Assert.Equal(Color.Red, control.ForeColor);
            Assert.Equal(Color.Red, child1.ForeColor);
            Assert.Equal(Color.Red, child2.ForeColor);
            Assert.Equal(1, callCount);
            Assert.Equal(1, childCallCount1);
            Assert.Equal(1, childCallCount2);

            // Set same.
            control.ForeColor = Color.Red;
            Assert.Equal(Color.Red, control.ForeColor);
            Assert.Equal(Color.Red, child1.ForeColor);
            Assert.Equal(Color.Red, child2.ForeColor);
            Assert.Equal(1, callCount);
            Assert.Equal(1, childCallCount1);
            Assert.Equal(1, childCallCount2);

            // Set different.
            control.ForeColor = Color.Empty;
            Assert.Equal(Control.DefaultForeColor, control.ForeColor);
            Assert.Equal(Control.DefaultForeColor, child1.ForeColor);
            Assert.Equal(Control.DefaultForeColor, child2.ForeColor);
            Assert.Equal(2, callCount);
            Assert.Equal(2, childCallCount1);
            Assert.Equal(2, childCallCount2);

            // Remove handler.
            control.ForeColorChanged -= handler;
            child1.ForeColorChanged -= childHandler1;
            child2.ForeColorChanged -= childHandler2;
            control.ForeColor = Color.Red;
            Assert.Equal(Color.Red, control.ForeColor);
            Assert.Equal(Color.Red, child1.ForeColor);
            Assert.Equal(Color.Red, child2.ForeColor);
            Assert.Equal(2, callCount);
            Assert.Equal(2, childCallCount1);
            Assert.Equal(2, childCallCount2);
        }

        [WinFormsFact]
        public void Control_ForeColor_SetWithChildrenWithForeColorWithHandler_CallsForeColorChanged()
        {
            using var child1 = new Control
            {
                ForeColor = Color.Yellow
            };
            using var child2 = new Control
            {
                ForeColor = Color.YellowGreen
            };
            using var control = new Control();
            control.Controls.Add(child1);
            control.Controls.Add(child2);

            int callCount = 0;
            int childCallCount1 = 0;
            int childCallCount2 = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            EventHandler childHandler1 = (sender, e) =>
            {
                Assert.Same(child1, sender);
                Assert.Same(EventArgs.Empty, e);
                childCallCount1++;
            };
            EventHandler childHandler2 = (sender, e) =>
            {
                Assert.Same(child2, sender);
                Assert.Same(EventArgs.Empty, e);
                childCallCount2++;
            };
            control.ForeColorChanged += handler;
            child1.ForeColorChanged += childHandler1;
            child2.ForeColorChanged += childHandler2;

            // Set different.
            control.ForeColor = Color.Red;
            Assert.Equal(Color.Red, control.ForeColor);
            Assert.Equal(Color.Yellow, child1.ForeColor);
            Assert.Equal(Color.YellowGreen, child2.ForeColor);
            Assert.Equal(1, callCount);
            Assert.Equal(0, childCallCount1);
            Assert.Equal(0, childCallCount2);

            // Set same.
            control.ForeColor = Color.Red;
            Assert.Equal(Color.Red, control.ForeColor);
            Assert.Equal(Color.Yellow, child1.ForeColor);
            Assert.Equal(Color.YellowGreen, child2.ForeColor);
            Assert.Equal(1, callCount);
            Assert.Equal(0, childCallCount1);
            Assert.Equal(0, childCallCount2);

            // Set different.
            control.ForeColor = Color.Empty;
            Assert.Equal(Control.DefaultForeColor, control.ForeColor);
            Assert.Equal(Color.Yellow, child1.ForeColor);
            Assert.Equal(Color.YellowGreen, child2.ForeColor);
            Assert.Equal(2, callCount);
            Assert.Equal(0, childCallCount1);
            Assert.Equal(0, childCallCount2);

            // Remove handler.
            control.ForeColorChanged -= handler;
            child1.ForeColorChanged -= childHandler1;
            child2.ForeColorChanged -= childHandler2;
            control.ForeColor = Color.Red;
            Assert.Equal(Color.Red, control.ForeColor);
            Assert.Equal(Color.Yellow, child1.ForeColor);
            Assert.Equal(Color.YellowGreen, child2.ForeColor);
            Assert.Equal(2, callCount);
            Assert.Equal(0, childCallCount1);
            Assert.Equal(0, childCallCount2);
        }

        [WinFormsFact]
        public void Control_ForeColor_ResetValue_Success()
        {
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(Control))[nameof(Control.ForeColor)];
            using var control = new Control();
            Assert.False(property.CanResetValue(control));

            control.ForeColor = Color.Red;
            Assert.Equal(Color.Red, control.ForeColor);
            Assert.True(property.CanResetValue(control));

            property.ResetValue(control);
            Assert.Equal(Control.DefaultForeColor, control.ForeColor);
            Assert.False(property.CanResetValue(control));
        }

        [WinFormsFact]
        public void Control_ForeColor_ShouldSerializeValue_Success()
        {
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(Control))[nameof(Control.ForeColor)];
            using var control = new Control();
            Assert.False(property.ShouldSerializeValue(control));

            control.ForeColor = Color.Red;
            Assert.Equal(Color.Red, control.ForeColor);
            Assert.True(property.ShouldSerializeValue(control));

            property.ResetValue(control);
            Assert.Equal(Control.DefaultForeColor, control.ForeColor);
            Assert.False(property.ShouldSerializeValue(control));
        }

        [Fact]
        public void Control_GetHandle()
        {
            var cont = new Control();

            IntPtr intptr = cont.Handle;

            Assert.NotEqual(IntPtr.Zero, intptr);
        }
        [WinFormsTheory]
        [InlineData(-4, 1)]
        [InlineData(0, 0)]
        [InlineData(2, 1)]
        [InlineData(40, 1)]
        public void Control_Height_Set_GetReturnsExpected(int value, int expectedLayoutCallCount)
        {
            using var control = new SubControl();
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

            control.Height = value;
            Assert.Equal(new Size(0, value), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, 0, value), control.ClientRectangle);
            Assert.Equal(new Rectangle(0, 0, 0, value), control.DisplayRectangle);
            Assert.Equal(new Size(0, value), control.Size);
            Assert.Equal(0, control.Left);
            Assert.Equal(0, control.Right);
            Assert.Equal(0, control.Top);
            Assert.Equal(value, control.Bottom);
            Assert.Equal(0, control.Width);
            Assert.Equal(value, control.Height);
            Assert.Equal(new Rectangle(0, 0, 0, value), control.Bounds);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.Equal(expectedLayoutCallCount, resizeCallCount);
            Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
            Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
            Assert.False(control.IsHandleCreated);

            // Call again.
            control.Height = value;
            Assert.Equal(new Size(0, value), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, 0, value), control.ClientRectangle);
            Assert.Equal(new Rectangle(0, 0, 0, value), control.DisplayRectangle);
            Assert.Equal(new Size(0, value), control.Size);
            Assert.Equal(0, control.Left);
            Assert.Equal(0, control.Right);
            Assert.Equal(0, control.Top);
            Assert.Equal(value, control.Bottom);
            Assert.Equal(0, control.Width);
            Assert.Equal(value, control.Height);
            Assert.Equal(new Rectangle(0, 0, 0, value), control.Bounds);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.Equal(expectedLayoutCallCount, resizeCallCount);
            Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
            Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
            Assert.False(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> Height_Set_WithConstrainedSize_TestData()
        {
            yield return new object[] { Size.Empty, Size.Empty, 40, 0, 40, 1 };
            yield return new object[] { new Size(10, 20), Size.Empty, 40, 10, 40, 1 };
            yield return new object[] { new Size(30, 40), Size.Empty, 40, 30, 40, 0 };
            yield return new object[] { new Size(31, 40), Size.Empty, 40, 31, 40, 0 };
            yield return new object[] { new Size(30, 41), Size.Empty, 40, 30, 41, 0 };
            yield return new object[] { new Size(40, 50), Size.Empty, 40, 40, 50, 0 };
            yield return new object[] { Size.Empty, new Size(20, 10), 40, 0, 10, 1 };
            yield return new object[] { Size.Empty, new Size(30, 40), 40, 0, 40, 1 };
            yield return new object[] { Size.Empty, new Size(31, 40), 40, 0, 40, 1 };
            yield return new object[] { Size.Empty, new Size(30, 41), 40, 0, 40, 1 };
            yield return new object[] { Size.Empty, new Size(40, 50), 40, 0, 40, 1 };
            yield return new object[] { new Size(10, 20), new Size(40, 50), 40, 10, 40, 1 };
            yield return new object[] { new Size(10, 20), new Size(20, 30), 40, 10, 30, 1 };
            yield return new object[] { new Size(30, 40), new Size(20, 30), 40, 30, 40, 0 };
            yield return new object[] { new Size(30, 40), new Size(40, 50), 40, 30, 40, 0 };
            yield return new object[] { new Size(40, 50), new Size(20, 30), 40, 40, 50, 0 };
            yield return new object[] { new Size(40, 50), new Size(40, 50), 40, 40, 50, 0 };
        }

        [WinFormsTheory]
        [MemberData(nameof(Height_Set_WithConstrainedSize_TestData))]
        public void Control_Height_SetWithConstrainedSize_GetReturnsExpected(Size minimumSize, Size maximumSize, int value, int expectedWidth, int expectedHeight, int expectedLayoutCallCount)
        {
            using var control = new SubControl
            {
                MinimumSize = minimumSize,
                MaximumSize = maximumSize
            };
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

            control.Height = value;
            Assert.Equal(new Size(expectedWidth, expectedHeight), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.ClientRectangle);
            Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.DisplayRectangle);
            Assert.Equal(new Size(expectedWidth, expectedHeight), control.Size);
            Assert.Equal(0, control.Left);
            Assert.Equal(expectedWidth, control.Right);
            Assert.Equal(0, control.Top);
            Assert.Equal(expectedHeight, control.Bottom);
            Assert.Equal(expectedWidth, control.Width);
            Assert.Equal(expectedHeight, control.Height);
            Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.Bounds);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.Equal(expectedLayoutCallCount, resizeCallCount);
            Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
            Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
            Assert.False(control.IsHandleCreated);

            // Call again.
            control.Height = value;
            Assert.Equal(new Size(expectedWidth, expectedHeight), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.ClientRectangle);
            Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.DisplayRectangle);
            Assert.Equal(new Size(expectedWidth, expectedHeight), control.Size);
            Assert.Equal(0, control.Left);
            Assert.Equal(expectedWidth, control.Right);
            Assert.Equal(0, control.Top);
            Assert.Equal(expectedHeight, control.Bottom);
            Assert.Equal(expectedWidth, control.Width);
            Assert.Equal(expectedHeight, control.Height);
            Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.Bounds);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.Equal(expectedLayoutCallCount, resizeCallCount);
            Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
            Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(-4, -4, -8, 1)]
        [InlineData(0, 0, 0, 0)]
        [InlineData(2, -4, -2, 1)]
        [InlineData(40, -4, 36, 1)]
        public void Control_Height_SetWithCustomStyle_GetReturnsExpected(int value, int expectedClientWidth, int expectedClientHeight, int expectedLayoutCallCount)
        {
            using var control = new BorderedControl();
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

            control.Height = value;
            Assert.Equal(new Size(expectedClientWidth, expectedClientHeight), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, expectedClientWidth, expectedClientHeight), control.ClientRectangle);
            Assert.Equal(new Rectangle(0, 0, expectedClientWidth, expectedClientHeight), control.DisplayRectangle);
            Assert.Equal(new Size(0, value), control.Size);
            Assert.Equal(0, control.Left);
            Assert.Equal(0, control.Right);
            Assert.Equal(0, control.Top);
            Assert.Equal(value, control.Bottom);
            Assert.Equal(0, control.Width);
            Assert.Equal(value, control.Height);
            Assert.Equal(new Rectangle(0, 0, 0, value), control.Bounds);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.Equal(expectedLayoutCallCount, resizeCallCount);
            Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
            Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
            Assert.False(control.IsHandleCreated);

            // Call again.
            control.Height = value;
            Assert.Equal(new Size(expectedClientWidth, expectedClientHeight), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, expectedClientWidth, expectedClientHeight), control.ClientRectangle);
            Assert.Equal(new Rectangle(0, 0, expectedClientWidth, expectedClientHeight), control.DisplayRectangle);
            Assert.Equal(new Size(0, value), control.Size);
            Assert.Equal(0, control.Left);
            Assert.Equal(0, control.Right);
            Assert.Equal(0, control.Top);
            Assert.Equal(value, control.Bottom);
            Assert.Equal(0, control.Width);
            Assert.Equal(value, control.Height);
            Assert.Equal(new Rectangle(0, 0, 0, value), control.Bounds);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.Equal(expectedLayoutCallCount, resizeCallCount);
            Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
            Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(-4, 1, 2)]
        [InlineData(0, 0, 0)]
        [InlineData(2, 1, 2)]
        [InlineData(40, 1, 2)]
        public void Control_Height_SetWithParent_GetReturnsExpected(int value, int expectedLayoutCallCount, int expectedParentLayoutCallCount)
        {
            using var parent = new Control();
            using var control = new SubControl
            {
                Parent = parent
            };
            int layoutCallCount = 0;
            int resizeCallCount = 0;
            int sizeChangedCallCount = 0;
            int clientSizeChangedCallCount = 0;
            int parentLayoutCallCount = 0;
            control.Layout += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(control, e.AffectedControl);
                Assert.Equal("Bounds", e.AffectedProperty);
                Assert.Equal(resizeCallCount, layoutCallCount);
                Assert.Equal(sizeChangedCallCount, layoutCallCount);
                Assert.Equal(clientSizeChangedCallCount, layoutCallCount);
                Assert.Equal(parentLayoutCallCount, layoutCallCount);
                layoutCallCount++;
            };
            control.Resize += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                Assert.Equal(layoutCallCount - 1, resizeCallCount);
                Assert.Equal(sizeChangedCallCount, resizeCallCount);
                Assert.Equal(clientSizeChangedCallCount, resizeCallCount);
                Assert.Equal(parentLayoutCallCount, resizeCallCount);
                resizeCallCount++;
            };
            control.SizeChanged += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                Assert.Equal(resizeCallCount - 1, sizeChangedCallCount);
                Assert.Equal(layoutCallCount - 1, sizeChangedCallCount);
                Assert.Equal(clientSizeChangedCallCount, sizeChangedCallCount);
                Assert.Equal(parentLayoutCallCount, sizeChangedCallCount);
                sizeChangedCallCount++;
            };
            control.ClientSizeChanged += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                Assert.Equal(resizeCallCount - 1, clientSizeChangedCallCount);
                Assert.Equal(layoutCallCount - 1, clientSizeChangedCallCount);
                Assert.Equal(sizeChangedCallCount - 1, clientSizeChangedCallCount);
                Assert.Equal(parentLayoutCallCount, clientSizeChangedCallCount);
                clientSizeChangedCallCount++;
            };
            void parentHandler(object sender, LayoutEventArgs e)
            {
                Assert.Same(parent, sender);
                Assert.Same(control, e.AffectedControl);
                Assert.Equal("Bounds", e.AffectedProperty);
                parentLayoutCallCount++;
            };
            parent.Layout += parentHandler;

            try
            {
                control.Height = value;
                Assert.Equal(new Size(0, value), control.ClientSize);
                Assert.Equal(new Rectangle(0, 0, 0, value), control.ClientRectangle);
                Assert.Equal(new Rectangle(0, 0, 0, value), control.DisplayRectangle);
                Assert.Equal(new Size(0, value), control.Size);
                Assert.Equal(0, control.Left);
                Assert.Equal(0, control.Right);
                Assert.Equal(0, control.Top);
                Assert.Equal(value, control.Bottom);
                Assert.Equal(0, control.Width);
                Assert.Equal(value, control.Height);
                Assert.Equal(new Rectangle(0, 0, 0, value), control.Bounds);
                Assert.Equal(expectedLayoutCallCount, layoutCallCount);
                Assert.Equal(expectedLayoutCallCount, resizeCallCount);
                Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
                Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
                Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
                Assert.False(control.IsHandleCreated);

                // Call again.
                control.Height = value;
                Assert.Equal(new Size(0, value), control.ClientSize);
                Assert.Equal(new Rectangle(0, 0, 0, value), control.ClientRectangle);
                Assert.Equal(new Rectangle(0, 0, 0, value), control.DisplayRectangle);
                Assert.Equal(new Size(0, value), control.Size);
                Assert.Equal(0, control.Left);
                Assert.Equal(0, control.Right);
                Assert.Equal(0, control.Top);
                Assert.Equal(value, control.Bottom);
                Assert.Equal(0, control.Width);
                Assert.Equal(value, control.Height);
                Assert.Equal(new Rectangle(0, 0, 0, value), control.Bounds);
                Assert.Equal(expectedLayoutCallCount, layoutCallCount);
                Assert.Equal(expectedLayoutCallCount, resizeCallCount);
                Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
                Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
                Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
                Assert.False(control.IsHandleCreated);
            }
            finally
            {
                parent.Layout -= parentHandler;
            }
        }

        public static IEnumerable<object[]> Height_SetWithHandle_TestData()
        {
            yield return new object[] { true, -4, 0, 0, 0 };
            yield return new object[] { true, 0, 0, 0, 0 };
            yield return new object[] { true, 2, 2, 1, 1 };
            yield return new object[] { true, 40, 40, 1, 1 };
            yield return new object[] { false, -4, 0, 0, 0 };
            yield return new object[] { false, 0, 0, 0, 0 };
            yield return new object[] { false, 2, 2, 1, 0 };
            yield return new object[] { false, 40, 40, 1, 0 };
        }

        [WinFormsTheory]
        [MemberData(nameof(Height_SetWithHandle_TestData))]
        public void Control_Height_SetWithHandle_GetReturnsExpected(bool resizeRedraw, int value, int expectedHeight, int expectedLayoutCallCount, int expectedInvalidatedCallCount)
        {
            using var control = new SubControl();
            control.SetStyle(ControlStyles.ResizeRedraw, resizeRedraw);
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
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.Height = value;
            Assert.Equal(new Size(0, expectedHeight), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, 0, expectedHeight), control.ClientRectangle);
            Assert.Equal(new Rectangle(0, 0, 0, expectedHeight), control.DisplayRectangle);
            Assert.Equal(new Size(0, expectedHeight), control.Size);
            Assert.Equal(0, control.Left);
            Assert.Equal(0, control.Right);
            Assert.Equal(0, control.Top);
            Assert.Equal(expectedHeight, control.Bottom);
            Assert.Equal(0, control.Width);
            Assert.Equal(expectedHeight, control.Height);
            Assert.Equal(new Rectangle(0, 0, 0, expectedHeight), control.Bounds);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.Equal(expectedLayoutCallCount, resizeCallCount);
            Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
            Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Call again.
            control.Height = value;
            Assert.Equal(new Size(0, expectedHeight), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, 0, expectedHeight), control.ClientRectangle);
            Assert.Equal(new Rectangle(0, 0, 0, expectedHeight), control.DisplayRectangle);
            Assert.Equal(new Size(0, expectedHeight), control.Size);
            Assert.Equal(0, control.Left);
            Assert.Equal(0, control.Right);
            Assert.Equal(0, control.Top);
            Assert.Equal(expectedHeight, control.Bottom);
            Assert.Equal(0, control.Width);
            Assert.Equal(expectedHeight, control.Height);
            Assert.Equal(new Rectangle(0, 0, 0, expectedHeight), control.Bounds);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.Equal(expectedLayoutCallCount, resizeCallCount);
            Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
            Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        public static IEnumerable<object[]> Height_SetWithParentWithHandle_TestData()
        {
            yield return new object[] { true, -4, 0, 0, 0, 1, 2 };
            yield return new object[] { true, 0, 0, 0, 0, 0, 0 };
            yield return new object[] { true, 2, 2, 1, 1, 2, 2 };
            yield return new object[] { true, 40, 40, 1, 1, 2, 2 };
            yield return new object[] { false, -4, 0, 0, 0, 1, 2 };
            yield return new object[] { false, 0, 0, 0, 0, 0, 0 };
            yield return new object[] { false, 2, 2, 1, 0, 2, 2 };
            yield return new object[] { false, 40, 40, 1, 0, 2, 2 };
        }

        [WinFormsTheory]
        [MemberData(nameof(Height_SetWithParentWithHandle_TestData))]
        public void Control_Height_SetWithParentWithHandle_GetReturnsExpected(bool resizeRedraw, int value, int expectedHeight, int expectedLayoutCallCount, int expectedInvalidatedCallCount, int expectedParentLayoutCallCount1, int expectedParentLayoutCallCount2)
        {
            using var parent = new Control();
            using var control = new SubControl
            {
                Parent = parent
            };
            control.SetStyle(ControlStyles.ResizeRedraw, resizeRedraw);
            int layoutCallCount = 0;
            int resizeCallCount = 0;
            int sizeChangedCallCount = 0;
            int clientSizeChangedCallCount = 0;
            int parentLayoutCallCount = 0;
            control.Layout += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(control, e.AffectedControl);
                Assert.Equal("Bounds", e.AffectedProperty);
                Assert.Equal(resizeCallCount, layoutCallCount);
                Assert.Equal(sizeChangedCallCount, layoutCallCount);
                Assert.Equal(clientSizeChangedCallCount, layoutCallCount);
                Assert.Equal(parentLayoutCallCount, layoutCallCount);
                layoutCallCount++;
            };
            control.Resize += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                Assert.Equal(layoutCallCount - 1, resizeCallCount);
                Assert.Equal(sizeChangedCallCount, resizeCallCount);
                Assert.Equal(clientSizeChangedCallCount, resizeCallCount);
                Assert.Equal(parentLayoutCallCount, resizeCallCount);
                resizeCallCount++;
            };
            control.SizeChanged += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                Assert.Equal(resizeCallCount - 1, sizeChangedCallCount);
                Assert.Equal(layoutCallCount - 1, sizeChangedCallCount);
                Assert.Equal(clientSizeChangedCallCount, sizeChangedCallCount);
                Assert.Equal(parentLayoutCallCount, sizeChangedCallCount);
                sizeChangedCallCount++;
            };
            control.ClientSizeChanged += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                Assert.Equal(resizeCallCount - 1, clientSizeChangedCallCount);
                Assert.Equal(layoutCallCount - 1, clientSizeChangedCallCount);
                Assert.Equal(sizeChangedCallCount - 1, clientSizeChangedCallCount);
                Assert.Equal(parentLayoutCallCount, clientSizeChangedCallCount);
                clientSizeChangedCallCount++;
            };
            void parentHandler(object sender, LayoutEventArgs e)
            {
                Assert.Same(parent, sender);
                Assert.Same(control, e.AffectedControl);
                Assert.Equal("Bounds", e.AffectedProperty);
                parentLayoutCallCount++;
            };
            parent.Layout += parentHandler;
            Assert.NotEqual(IntPtr.Zero, parent.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;
            int parentInvalidatedCallCount = 0;
            parent.Invalidated += (sender, e) => parentInvalidatedCallCount++;
            int parentStyleChangedCallCount = 0;
            parent.StyleChanged += (sender, e) => parentStyleChangedCallCount++;
            int parentCreatedCallCount = 0;
            parent.HandleCreated += (sender, e) => parentCreatedCallCount++;

            try
            {
                control.Height = value;
                Assert.Equal(new Size(0, expectedHeight), control.ClientSize);
                Assert.Equal(new Rectangle(0, 0, 0, expectedHeight), control.ClientRectangle);
                Assert.Equal(new Rectangle(0, 0, 0, expectedHeight), control.DisplayRectangle);
                Assert.Equal(new Size(0, expectedHeight), control.Size);
                Assert.Equal(0, control.Left);
                Assert.Equal(0, control.Right);
                Assert.Equal(0, control.Top);
                Assert.Equal(expectedHeight, control.Bottom);
                Assert.Equal(0, control.Width);
                Assert.Equal(expectedHeight, control.Height);
                Assert.Equal(new Rectangle(0, 0, 0, expectedHeight), control.Bounds);
                Assert.Equal(expectedLayoutCallCount, layoutCallCount);
                Assert.Equal(expectedParentLayoutCallCount1, parentLayoutCallCount);
                Assert.Equal(expectedLayoutCallCount, resizeCallCount);
                Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
                Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
                Assert.True(control.IsHandleCreated);
                Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
                Assert.Equal(0, styleChangedCallCount);
                Assert.Equal(0, createdCallCount);
                Assert.True(parent.IsHandleCreated);
                Assert.Equal(0, parentInvalidatedCallCount);
                Assert.Equal(0, parentStyleChangedCallCount);
                Assert.Equal(0, parentCreatedCallCount);

                // Call again.
                control.Height = value;
                Assert.Equal(new Size(0, expectedHeight), control.ClientSize);
                Assert.Equal(new Rectangle(0, 0, 0, expectedHeight), control.ClientRectangle);
                Assert.Equal(new Rectangle(0, 0, 0, expectedHeight), control.DisplayRectangle);
                Assert.Equal(new Size(0, expectedHeight), control.Size);
                Assert.Equal(0, control.Left);
                Assert.Equal(0, control.Right);
                Assert.Equal(0, control.Top);
                Assert.Equal(expectedHeight, control.Bottom);
                Assert.Equal(0, control.Width);
                Assert.Equal(expectedHeight, control.Height);
                Assert.Equal(new Rectangle(0, 0, 0, expectedHeight), control.Bounds);
                Assert.Equal(expectedLayoutCallCount, layoutCallCount);
                Assert.Equal(expectedParentLayoutCallCount2, parentLayoutCallCount);
                Assert.Equal(expectedLayoutCallCount, resizeCallCount);
                Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
                Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
                Assert.True(control.IsHandleCreated);
                Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
                Assert.Equal(0, styleChangedCallCount);
                Assert.Equal(0, createdCallCount);
                Assert.True(parent.IsHandleCreated);
                Assert.Equal(0, parentInvalidatedCallCount);
                Assert.Equal(0, parentStyleChangedCallCount);
                Assert.Equal(0, parentCreatedCallCount);
            }
            finally
            {
                parent.Layout -= parentHandler;
            }
        }

        public static IEnumerable<object[]> ImeMode_Set_TestData()
        {
            yield return new object[] { ImeMode.Inherit, ImeMode.NoControl };
            yield return new object[] { ImeMode.NoControl, ImeMode.NoControl };
            yield return new object[] { ImeMode.On, ImeMode.On };
            yield return new object[] { ImeMode.Off, ImeMode.Off };
            yield return new object[] { ImeMode.Disable, ImeMode.Disable };
            yield return new object[] { ImeMode.Hiragana, ImeMode.Hiragana };
            yield return new object[] { ImeMode.Katakana, ImeMode.Katakana };
            yield return new object[] { ImeMode.KatakanaHalf, ImeMode.KatakanaHalf };
            yield return new object[] { ImeMode.AlphaFull, ImeMode.AlphaFull };
            yield return new object[] { ImeMode.Alpha, ImeMode.Alpha };
            yield return new object[] { ImeMode.HangulFull, ImeMode.HangulFull };
            yield return new object[] { ImeMode.Hangul, ImeMode.Hangul };
            yield return new object[] { ImeMode.Close, ImeMode.Close };
            yield return new object[] { ImeMode.OnHalf, ImeMode.On };
        }

        [Theory]
        [MemberData(nameof(ImeMode_Set_TestData))]
        public void Control_ImeMode_Set_GetReturnsExpected(ImeMode value, ImeMode expected)
        {
            using var control = new Control
            {
                ImeMode = value
            };
            Assert.Equal(expected, control.ImeMode);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.ImeMode = value;
            Assert.Equal(expected, control.ImeMode);
            Assert.False(control.IsHandleCreated);
        }

        [Theory]
        [MemberData(nameof(ImeMode_Set_TestData))]
        public void Control_ImeMode_SetWithHandle_GetReturnsExpected(ImeMode value, ImeMode expected)
        {
            using var control = new Control();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.ImeMode = value;
            Assert.Equal(expected, control.ImeMode);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.ImeMode = value;
            Assert.Equal(expected, control.ImeMode);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [Fact]
        public void Control_ImeMode_SetWithHandler_CallsImeModeChanged()
        {
            using var control = new Control();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            control.ImeModeChanged += handler;

            // Set different.
            control.ImeMode = ImeMode.On;
            Assert.Equal(ImeMode.On, control.ImeMode);
            Assert.Equal(1, callCount);

            // Set same.
            control.ImeMode = ImeMode.On;
            Assert.Equal(ImeMode.On, control.ImeMode);
            Assert.Equal(1, callCount);

            // Set different.
            control.ImeMode = ImeMode.Off;
            Assert.Equal(ImeMode.Off, control.ImeMode);
            Assert.Equal(2, callCount);

            // Remove handler.
            control.ImeModeChanged -= handler;
            control.ImeMode = ImeMode.Off;
            Assert.Equal(ImeMode.Off, control.ImeMode);
            Assert.Equal(2, callCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(ImeMode))]
        public void Control_ImeMode_SetInvalid_ThrowsInvalidEnumArgumentException(ImeMode value)
        {
            using var control = new Control();
            Assert.Throws<InvalidEnumArgumentException>("value", () => control.ImeMode = value);
        }

        public static IEnumerable<object[]> ImeModeBase_Set_TestData()
        {
            yield return new object[] { ImeMode.Inherit, ImeMode.NoControl };
            yield return new object[] { ImeMode.NoControl, ImeMode.NoControl };
            yield return new object[] { ImeMode.On, ImeMode.On };
            yield return new object[] { ImeMode.Off, ImeMode.Off };
            yield return new object[] { ImeMode.Disable, ImeMode.Disable };
            yield return new object[] { ImeMode.Hiragana, ImeMode.Hiragana };
            yield return new object[] { ImeMode.Katakana, ImeMode.Katakana };
            yield return new object[] { ImeMode.KatakanaHalf, ImeMode.KatakanaHalf };
            yield return new object[] { ImeMode.AlphaFull, ImeMode.AlphaFull };
            yield return new object[] { ImeMode.Alpha, ImeMode.Alpha };
            yield return new object[] { ImeMode.HangulFull, ImeMode.HangulFull };
            yield return new object[] { ImeMode.Hangul, ImeMode.Hangul };
            yield return new object[] { ImeMode.Close, ImeMode.Close };
            yield return new object[] { ImeMode.OnHalf, ImeMode.OnHalf };
        }

        [Theory]
        [MemberData(nameof(ImeModeBase_Set_TestData))]
        public void Control_ImeModeBase_Set_GetReturnsExpected(ImeMode value, ImeMode expected)
        {
            using var control = new SubControl
            {
                ImeModeBase = value
            };
            Assert.Equal(expected, control.ImeModeBase);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.ImeModeBase = value;
            Assert.Equal(expected, control.ImeModeBase);
            Assert.False(control.IsHandleCreated);
        }

        [Theory]
        [MemberData(nameof(ImeModeBase_Set_TestData))]
        public void Control_ImeModeBase_SetWithHandle_GetReturnsExpected(ImeMode value, ImeMode expected)
        {
            using var control = new SubControl();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.ImeModeBase = value;
            Assert.Equal(expected, control.ImeModeBase);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.ImeModeBase = value;
            Assert.Equal(expected, control.ImeModeBase);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [Fact]
        public void Control_ImeModeBase_SetWithHandler_CallsImeModeChanged()
        {
            using var control = new SubControl();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            control.ImeModeChanged += handler;

            // Set different.
            control.ImeModeBase = ImeMode.On;
            Assert.Equal(ImeMode.On, control.ImeModeBase);
            Assert.Equal(1, callCount);

            // Set same.
            control.ImeModeBase = ImeMode.On;
            Assert.Equal(ImeMode.On, control.ImeModeBase);
            Assert.Equal(1, callCount);

            // Set different.
            control.ImeModeBase = ImeMode.Off;
            Assert.Equal(ImeMode.Off, control.ImeModeBase);
            Assert.Equal(2, callCount);

            // Remove handler.
            control.ImeModeChanged -= handler;
            control.ImeModeBase = ImeMode.Off;
            Assert.Equal(ImeMode.Off, control.ImeModeBase);
            Assert.Equal(2, callCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(ImeMode))]
        public void Control_ImeModeBase_SetInvalid_ThrowsInvalidEnumArgumentException(ImeMode value)
        {
            using var control = new SubControl();
            Assert.Throws<InvalidEnumArgumentException>("value", () => control.ImeModeBase = value);
        }

        /// <summary>
        ///  Data for the IsAccessibleGetSet test
        /// </summary>
        public static TheoryData<bool> IsAccessibleGetSetData =>
            CommonTestHelper.GetBoolTheoryData();

        [Theory]
        [MemberData(nameof(IsAccessibleGetSetData))]
        public void Control_IsAccessibleGetSet(bool expected)
        {
            var cont = new Control
            {
                IsAccessible = expected
            };

            Assert.Equal(expected, cont.IsAccessible);
        }

        [WinFormsTheory]
        [InlineData(0, 0)]
        [InlineData(-1, 1)]
        [InlineData(1, 1)]
        public void Control_Left_Set_GetReturnsExpected(int value, int expectedLocationChangedCallCount)
        {
            using var control = new SubControl();
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

            control.Left = value;
            Assert.Equal(new Size(0, 0), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, 0, 0), control.ClientRectangle);
            Assert.Equal(new Rectangle(0, 0, 0, 0), control.DisplayRectangle);
            Assert.Equal(new Size(0, 0), control.Size);
            Assert.Equal(value, control.Left);
            Assert.Equal(value, control.Right);
            Assert.Equal(new Point(value, 0), control.Location);
            Assert.Equal(0, control.Top);
            Assert.Equal(0, control.Bottom);
            Assert.Equal(0, control.Width);
            Assert.Equal(0, control.Height);
            Assert.Equal(new Rectangle(value, 0, 0, 0), control.Bounds);
            Assert.Equal(expectedLocationChangedCallCount, moveCallCount);
            Assert.Equal(expectedLocationChangedCallCount, locationChangedCallCount);
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(0, resizeCallCount);
            Assert.Equal(0, sizeChangedCallCount);
            Assert.Equal(0, clientSizeChangedCallCount);
            Assert.False(control.IsHandleCreated);

            // Call again.
            control.Left = value;
            Assert.Equal(new Size(0, 0), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, 0, 0), control.ClientRectangle);
            Assert.Equal(new Rectangle(0, 0, 0, 0), control.DisplayRectangle);
            Assert.Equal(new Size(0, 0), control.Size);
            Assert.Equal(value, control.Left);
            Assert.Equal(value, control.Right);
            Assert.Equal(new Point(value, 0), control.Location);
            Assert.Equal(0, control.Top);
            Assert.Equal(0, control.Bottom);
            Assert.Equal(0, control.Width);
            Assert.Equal(0, control.Height);
            Assert.Equal(new Rectangle(value, 0, 0, 0), control.Bounds);
            Assert.Equal(expectedLocationChangedCallCount, moveCallCount);
            Assert.Equal(expectedLocationChangedCallCount, locationChangedCallCount);
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(0, resizeCallCount);
            Assert.Equal(0, sizeChangedCallCount);
            Assert.Equal(0, clientSizeChangedCallCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(0, 0)]
        [InlineData(-1, 1)]
        [InlineData(1, 1)]
        public void Control_Left_SetWithParent_GetReturnsExpected(int value, int expectedLocationChangedCallCount)
        {
            using var parent = new Control();
            using var control = new SubControl
            {
                Parent = parent
            };
            int moveCallCount = 0;
            int locationChangedCallCount = 0;
            int layoutCallCount = 0;
            int resizeCallCount = 0;
            int sizeChangedCallCount = 0;
            int clientSizeChangedCallCount = 0;
            int parentLayoutCallCount = 0;
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
            void parentHandler(object sender, LayoutEventArgs e)
            {
                Assert.Same(parent, sender);
                Assert.Same(control, e.AffectedControl);
                Assert.Equal("Bounds", e.AffectedProperty);
                parentLayoutCallCount++;
            };
            parent.Layout += parentHandler;

            try
            {
                control.Left = value;
                Assert.Equal(new Size(0, 0), control.ClientSize);
                Assert.Equal(new Rectangle(0, 0, 0, 0), control.ClientRectangle);
                Assert.Equal(new Rectangle(0, 0, 0, 0), control.DisplayRectangle);
                Assert.Equal(new Size(0, 0), control.Size);
                Assert.Equal(value, control.Left);
                Assert.Equal(new Point(value, 0), control.Location);
                Assert.Equal(0, control.Top);
                Assert.Equal(0, control.Bottom);
                Assert.Equal(0, control.Width);
                Assert.Equal(0, control.Height);
                Assert.Equal(new Rectangle(value, 0, 0, 0), control.Bounds);
                Assert.Equal(expectedLocationChangedCallCount, moveCallCount);
                Assert.Equal(expectedLocationChangedCallCount, locationChangedCallCount);
                Assert.Equal(0, layoutCallCount);
                Assert.Equal(0, resizeCallCount);
                Assert.Equal(0, sizeChangedCallCount);
                Assert.Equal(0, clientSizeChangedCallCount);
                Assert.Equal(expectedLocationChangedCallCount, parentLayoutCallCount);
                Assert.False(control.IsHandleCreated);

                // Call again.
                control.Left = value;
                Assert.Equal(new Size(0, 0), control.ClientSize);
                Assert.Equal(new Rectangle(0, 0, 0, 0), control.ClientRectangle);
                Assert.Equal(new Rectangle(0, 0, 0, 0), control.DisplayRectangle);
                Assert.Equal(new Size(0, 0), control.Size);
                Assert.Equal(value, control.Left);
                Assert.Equal(new Point(value, 0), control.Location);
                Assert.Equal(0, control.Top);
                Assert.Equal(0, control.Bottom);
                Assert.Equal(0, control.Width);
                Assert.Equal(0, control.Height);
                Assert.Equal(new Rectangle(value, 0, 0, 0), control.Bounds);
                Assert.Equal(expectedLocationChangedCallCount, moveCallCount);
                Assert.Equal(expectedLocationChangedCallCount, locationChangedCallCount);
                Assert.Equal(0, layoutCallCount);
                Assert.Equal(0, resizeCallCount);
                Assert.Equal(0, sizeChangedCallCount);
                Assert.Equal(0, clientSizeChangedCallCount);
                Assert.Equal(expectedLocationChangedCallCount, parentLayoutCallCount);
                Assert.False(control.IsHandleCreated);
            }
            finally
            {
                parent.Layout -= parentHandler;
            }
        }

        public static IEnumerable<object[]> Left_SetWithHandle_TestData()
        {
            foreach (bool resizeRedraw in new bool[] { true, false })
            {
                yield return new object[] { resizeRedraw, 0, 0 };
                yield return new object[] { resizeRedraw, -1, 1 };
                yield return new object[] { resizeRedraw, 1, 1 };
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(Left_SetWithHandle_TestData))]
        public void Control_Left_SetWithHandle_GetReturnsExpected(bool resizeRedraw, int value,  int expectedLocationChangedCallCount)
        {
            using var control = new SubControl();
            control.SetStyle(ControlStyles.ResizeRedraw, resizeRedraw);
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

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

            control.Left = value;
            Assert.Equal(new Size(0, 0), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, 0, 0), control.ClientRectangle);
            Assert.Equal(new Rectangle(0, 0, 0, 0), control.DisplayRectangle);
            Assert.Equal(new Size(0, 0), control.Size);
            Assert.Equal(value, control.Left);
            Assert.Equal(value, control.Right);
            Assert.Equal(new Point(value, 0), control.Location);
            Assert.Equal(0, control.Top);
            Assert.Equal(0, control.Bottom);
            Assert.Equal(0, control.Width);
            Assert.Equal(0, control.Height);
            Assert.Equal(new Rectangle(value, 0, 0, 0), control.Bounds);
            Assert.Equal(expectedLocationChangedCallCount, moveCallCount);
            Assert.Equal(expectedLocationChangedCallCount, locationChangedCallCount);
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(0, resizeCallCount);
            Assert.Equal(0, sizeChangedCallCount);
            Assert.Equal(0, clientSizeChangedCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Call again.
            control.Left = value;
            Assert.Equal(new Size(0, 0), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, 0, 0), control.ClientRectangle);
            Assert.Equal(new Rectangle(0, 0, 0, 0), control.DisplayRectangle);
            Assert.Equal(new Size(0, 0), control.Size);
            Assert.Equal(value, control.Left);
            Assert.Equal(value, control.Right);
            Assert.Equal(new Point(value, 0), control.Location);
            Assert.Equal(0, control.Top);
            Assert.Equal(0, control.Bottom);
            Assert.Equal(0, control.Width);
            Assert.Equal(0, control.Height);
            Assert.Equal(new Rectangle(value, 0, 0, 0), control.Bounds);
            Assert.Equal(expectedLocationChangedCallCount, moveCallCount);
            Assert.Equal(expectedLocationChangedCallCount, locationChangedCallCount);
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(0, resizeCallCount);
            Assert.Equal(0, sizeChangedCallCount);
            Assert.Equal(0, clientSizeChangedCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [MemberData(nameof(Left_SetWithHandle_TestData))]
        public void Control_Left_SetWithParentWithHandle_GetReturnsExpected(bool resizeRedraw, int value, int expectedLocationChangedCallCount)
        {
            using var parent = new Control();
            using var control = new SubControl
            {
                Parent = parent
            };
            control.SetStyle(ControlStyles.ResizeRedraw, resizeRedraw);
            Assert.NotEqual(IntPtr.Zero, parent.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;
            int parentInvalidatedCallCount = 0;
            parent.Invalidated += (sender, e) => parentInvalidatedCallCount++;
            int parentStyleChangedCallCount = 0;
            parent.StyleChanged += (sender, e) => parentStyleChangedCallCount++;
            int parentCreatedCallCount = 0;
            parent.HandleCreated += (sender, e) => parentCreatedCallCount++;

            int moveCallCount = 0;
            int locationChangedCallCount = 0;
            int layoutCallCount = 0;
            int resizeCallCount = 0;
            int sizeChangedCallCount = 0;
            int clientSizeChangedCallCount = 0;
            int parentLayoutCallCount = 0;
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
            void parentHandler(object sender, LayoutEventArgs e)
            {
                Assert.Same(parent, sender);
                Assert.Same(control, e.AffectedControl);
                Assert.Equal("Bounds", e.AffectedProperty);
                parentLayoutCallCount++;
            };
            parent.Layout += parentHandler;

            try
            {
                control.Left = value;
                Assert.Equal(new Size(0, 0), control.ClientSize);
                Assert.Equal(new Rectangle(0, 0, 0, 0), control.ClientRectangle);
                Assert.Equal(new Rectangle(0, 0, 0, 0), control.DisplayRectangle);
                Assert.Equal(new Size(0, 0), control.Size);
                Assert.Equal(value, control.Left);
                Assert.Equal(new Point(value, 0), control.Location);
                Assert.Equal(0, control.Top);
                Assert.Equal(0, control.Bottom);
                Assert.Equal(0, control.Width);
                Assert.Equal(0, control.Height);
                Assert.Equal(new Rectangle(value, 0, 0, 0), control.Bounds);
                Assert.Equal(expectedLocationChangedCallCount, moveCallCount);
                Assert.Equal(expectedLocationChangedCallCount, locationChangedCallCount);
                Assert.Equal(0, layoutCallCount);
                Assert.Equal(expectedLocationChangedCallCount, parentLayoutCallCount);
                Assert.Equal(0, resizeCallCount);
                Assert.Equal(0, sizeChangedCallCount);
                Assert.Equal(0, clientSizeChangedCallCount);
                Assert.True(control.IsHandleCreated);
                Assert.Equal(0, invalidatedCallCount);
                Assert.Equal(0, styleChangedCallCount);
                Assert.Equal(0, createdCallCount);
                Assert.True(parent.IsHandleCreated);
                Assert.Equal(0, parentInvalidatedCallCount);
                Assert.Equal(0, parentStyleChangedCallCount);
                Assert.Equal(0, parentCreatedCallCount);

                // Call again.
                control.Left = value;
                Assert.Equal(new Size(0, 0), control.ClientSize);
                Assert.Equal(new Rectangle(0, 0, 0, 0), control.ClientRectangle);
                Assert.Equal(new Rectangle(0, 0, 0, 0), control.DisplayRectangle);
                Assert.Equal(new Size(0, 0), control.Size);
                Assert.Equal(value, control.Left);
                Assert.Equal(new Point(value, 0), control.Location);
                Assert.Equal(0, control.Top);
                Assert.Equal(0, control.Bottom);
                Assert.Equal(0, control.Width);
                Assert.Equal(0, control.Height);
                Assert.Equal(new Rectangle(value, 0, 0, 0), control.Bounds);
                Assert.Equal(expectedLocationChangedCallCount, moveCallCount);
                Assert.Equal(expectedLocationChangedCallCount, locationChangedCallCount);
                Assert.Equal(0, layoutCallCount);
                Assert.Equal(expectedLocationChangedCallCount, parentLayoutCallCount);
                Assert.Equal(0, resizeCallCount);
                Assert.Equal(0, sizeChangedCallCount);
                Assert.Equal(0, clientSizeChangedCallCount);
                Assert.True(control.IsHandleCreated);
                Assert.Equal(0, invalidatedCallCount);
                Assert.Equal(0, styleChangedCallCount);
                Assert.Equal(0, createdCallCount);
                Assert.True(parent.IsHandleCreated);
                Assert.Equal(0, parentInvalidatedCallCount);
                Assert.Equal(0, parentStyleChangedCallCount);
                Assert.Equal(0, parentCreatedCallCount);
            }
            finally
            {
                parent.Layout -= parentHandler;
            }
        }

        [WinFormsTheory]
        [InlineData(true, 1)]
        [InlineData(false, 0)]
        public void Control_Left_SetWithHandleWithTransparentBackColor_DoesNotCallInvalidate(bool supportsTransparentBackgroundColor, int expectedInvalidatedCallCount)
        {
            using var control = new SubControl();
            control.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            control.BackColor = Color.FromArgb(254, 255, 255, 255);
            control.SetStyle(ControlStyles.SupportsTransparentBackColor, supportsTransparentBackgroundColor);

            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;

            // Set different.
            control.Left = 1;
            Assert.Equal(new Point(1, 0), control.Location);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);

            // Set same.
            control.Left = 1;
            Assert.Equal(new Point(1, 0), control.Location);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);

            // Set different.
            control.Left = 2;
            Assert.Equal(new Point(2, 0), control.Location);
            Assert.Equal(expectedInvalidatedCallCount * 2, invalidatedCallCount);
        }

        [WinFormsFact]
        public void Control_Left_SetWithHandler_CallsLocationChanged()
        {
            using var control = new Control();
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
            control.Left = 1;
            Assert.Equal(new Point(1, 0), control.Location);
            Assert.Equal(1, locationChangedCallCount);
            Assert.Equal(1, moveCallCount);

            // Set same.
            control.Left = 1;
            Assert.Equal(new Point(1, 0), control.Location);
            Assert.Equal(1, locationChangedCallCount);
            Assert.Equal(1, moveCallCount);

            // Set different.
            control.Left = 2;
            Assert.Equal(new Point(2, 0), control.Location);
            Assert.Equal(2, locationChangedCallCount);
            Assert.Equal(2, moveCallCount);

            // Remove handler.
            control.LocationChanged -= locationChangedHandler;
            control.Move -= moveHandler;
            control.Left = 1;
            Assert.Equal(new Point(1, 0), control.Location);
            Assert.Equal(2, locationChangedCallCount);
            Assert.Equal(2, moveCallCount);
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
        public void Control_Location_Set_GetReturnsExpected(Point value, int expectedLocationChangedCallCount)
        {
            using var control = new Control();
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
            Assert.Equal(new Size(0, 0), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, 0, 0), control.ClientRectangle);
            Assert.Equal(new Rectangle(0, 0, 0, 0), control.DisplayRectangle);
            Assert.Equal(new Size(0, 0), control.Size);
            Assert.Equal(value.X, control.Left);
            Assert.Equal(value.X, control.Right);
            Assert.Equal(value, control.Location);
            Assert.Equal(value.Y, control.Top);
            Assert.Equal(value.Y, control.Bottom);
            Assert.Equal(0, control.Width);
            Assert.Equal(0, control.Height);
            Assert.Equal(new Rectangle(value.X, value.Y, 0, 0), control.Bounds);
            Assert.Equal(expectedLocationChangedCallCount, moveCallCount);
            Assert.Equal(expectedLocationChangedCallCount, locationChangedCallCount);
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(0, resizeCallCount);
            Assert.Equal(0, sizeChangedCallCount);
            Assert.Equal(0, clientSizeChangedCallCount);
            Assert.False(control.IsHandleCreated);

            // Call again.
            control.Location = value;
            Assert.Equal(new Size(0, 0), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, 0, 0), control.ClientRectangle);
            Assert.Equal(new Rectangle(0, 0, 0, 0), control.DisplayRectangle);
            Assert.Equal(new Size(0, 0), control.Size);
            Assert.Equal(value.X, control.Left);
            Assert.Equal(value.X, control.Right);
            Assert.Equal(value, control.Location);
            Assert.Equal(value.Y, control.Top);
            Assert.Equal(value.Y, control.Bottom);
            Assert.Equal(0, control.Width);
            Assert.Equal(0, control.Height);
            Assert.Equal(new Rectangle(value.X, value.Y, 0, 0), control.Bounds);
            Assert.Equal(expectedLocationChangedCallCount, moveCallCount);
            Assert.Equal(expectedLocationChangedCallCount, locationChangedCallCount);
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(0, resizeCallCount);
            Assert.Equal(0, sizeChangedCallCount);
            Assert.Equal(0, clientSizeChangedCallCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(Location_Set_TestData))]
        public void Control_Location_SetWithParent_GetReturnsExpected(Point value, int expectedLocationChangedCallCount)
        {
            using var parent = new Control();
            using var control = new SubControl
            {
                Parent = parent
            };
            int layoutCallCount = 0;
            int moveCallCount = 0;
            int locationChangedCallCount = 0;
            int resizeCallCount = 0;
            int sizeChangedCallCount = 0;
            int clientSizeChangedCallCount = 0;
            int parentLayoutCallCount = 0;
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
            void parentHandler(object sender, LayoutEventArgs e)
            {
                Assert.Same(parent, sender);
                Assert.Same(control, e.AffectedControl);
                Assert.Equal("Bounds", e.AffectedProperty);
                parentLayoutCallCount++;
            };
            parent.Layout += parentHandler;

            try
            {
                control.Location = value;
                Assert.Equal(new Size(0, 0), control.ClientSize);
                Assert.Equal(new Rectangle(0, 0, 0, 0), control.ClientRectangle);
                Assert.Equal(new Rectangle(0, 0, 0, 0), control.DisplayRectangle);
                Assert.Equal(new Size(0, 0), control.Size);
                Assert.Equal(value.X, control.Left);
                Assert.Equal(value.X, control.Right);
                Assert.Equal(value, control.Location);
                Assert.Equal(value.Y, control.Top);
                Assert.Equal(value.Y, control.Bottom);
                Assert.Equal(0, control.Width);
                Assert.Equal(0, control.Height);
                Assert.Equal(new Rectangle(value.X, value.Y, 0, 0), control.Bounds);
                Assert.Equal(expectedLocationChangedCallCount, moveCallCount);
                Assert.Equal(expectedLocationChangedCallCount, locationChangedCallCount);
                Assert.Equal(0, layoutCallCount);
                Assert.Equal(0, resizeCallCount);
                Assert.Equal(0, sizeChangedCallCount);
                Assert.Equal(0, clientSizeChangedCallCount);
                Assert.Equal(expectedLocationChangedCallCount, parentLayoutCallCount);
                Assert.False(control.IsHandleCreated);

                // Call again.
                control.Location = value;
                Assert.Equal(new Size(0, 0), control.ClientSize);
                Assert.Equal(new Rectangle(0, 0, 0, 0), control.ClientRectangle);
                Assert.Equal(new Rectangle(0, 0, 0, 0), control.DisplayRectangle);
                Assert.Equal(new Size(0, 0), control.Size);
                Assert.Equal(value.X, control.Left);
                Assert.Equal(value.X, control.Right);
                Assert.Equal(value, control.Location);
                Assert.Equal(value.Y, control.Top);
                Assert.Equal(value.Y, control.Bottom);
                Assert.Equal(0, control.Width);
                Assert.Equal(0, control.Height);
                Assert.Equal(new Rectangle(value.X, value.Y, 0, 0), control.Bounds);
                Assert.Equal(expectedLocationChangedCallCount, moveCallCount);
                Assert.Equal(expectedLocationChangedCallCount, locationChangedCallCount);
                Assert.Equal(0, layoutCallCount);
                Assert.Equal(0, resizeCallCount);
                Assert.Equal(0, sizeChangedCallCount);
                Assert.Equal(0, clientSizeChangedCallCount);
                Assert.Equal(expectedLocationChangedCallCount, parentLayoutCallCount);
                Assert.False(control.IsHandleCreated);
            }
            finally
            {
                parent.Layout -= parentHandler;
            }
        }

        public static IEnumerable<object[]> Location_SetWithHandle_TestData()
        {
            foreach (bool resizeRedraw in new bool[] { true, false })
            {
                yield return new object[] { resizeRedraw, new Point(0, 0), 0 };
                yield return new object[] { resizeRedraw, new Point(-1, -2), 1 };
                yield return new object[] { resizeRedraw, new Point(1, 0), 1 };
                yield return new object[] { resizeRedraw, new Point(0, 2), 1 };
                yield return new object[] { resizeRedraw, new Point(1, 2), 1 };
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(Location_SetWithHandle_TestData))]
        public void Control_Location_SetWithHandle_GetReturnsExpected(bool resizeRedraw, Point value, int expectedLocationChangedCallCount)
        {
            using var control = new SubControl();
            control.SetStyle(ControlStyles.ResizeRedraw, resizeRedraw);
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

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
            Assert.Equal(new Size(0, 0), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, 0, 0), control.ClientRectangle);
            Assert.Equal(new Rectangle(0, 0, 0, 0), control.DisplayRectangle);
            Assert.Equal(new Size(0, 0), control.Size);
            Assert.Equal(value.X, control.Left);
            Assert.Equal(value.X, control.Right);
            Assert.Equal(value, control.Location);
            Assert.Equal(value.Y, control.Top);
            Assert.Equal(value.Y, control.Bottom);
            Assert.Equal(0, control.Width);
            Assert.Equal(0, control.Height);
            Assert.Equal(new Rectangle(value.X, value.Y, 0, 0), control.Bounds);
            Assert.Equal(expectedLocationChangedCallCount, moveCallCount);
            Assert.Equal(expectedLocationChangedCallCount, locationChangedCallCount);
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(0, resizeCallCount);
            Assert.Equal(0, sizeChangedCallCount);
            Assert.Equal(0, clientSizeChangedCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Call again.
            control.Location = value;
            Assert.Equal(new Size(0, 0), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, 0, 0), control.ClientRectangle);
            Assert.Equal(new Rectangle(0, 0, 0, 0), control.DisplayRectangle);
            Assert.Equal(new Size(0, 0), control.Size);
            Assert.Equal(value.X, control.Left);
            Assert.Equal(value.X, control.Right);
            Assert.Equal(value, control.Location);
            Assert.Equal(value.Y, control.Top);
            Assert.Equal(value.Y, control.Bottom);
            Assert.Equal(0, control.Width);
            Assert.Equal(0, control.Height);
            Assert.Equal(new Rectangle(value.X, value.Y, 0, 0), control.Bounds);
            Assert.Equal(expectedLocationChangedCallCount, moveCallCount);
            Assert.Equal(expectedLocationChangedCallCount, locationChangedCallCount);
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(0, resizeCallCount);
            Assert.Equal(0, sizeChangedCallCount);
            Assert.Equal(0, clientSizeChangedCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [MemberData(nameof(Location_SetWithHandle_TestData))]
        public void Control_Location_SetWithParentWithHandle_GetReturnsExpected(bool resizeRedraw, Point value, int expectedLocationChangedCallCount)
        {
            using var parent = new Control();
            using var control = new SubControl
            {
                Parent = parent
            };
            control.SetStyle(ControlStyles.ResizeRedraw, resizeRedraw);
            Assert.NotEqual(IntPtr.Zero, parent.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;
            int parentInvalidatedCallCount = 0;
            parent.Invalidated += (sender, e) => parentInvalidatedCallCount++;
            int parentStyleChangedCallCount = 0;
            parent.StyleChanged += (sender, e) => parentStyleChangedCallCount++;
            int parentCreatedCallCount = 0;
            parent.HandleCreated += (sender, e) => parentCreatedCallCount++;

            int layoutCallCount = 0;
            int moveCallCount = 0;
            int locationChangedCallCount = 0;
            int resizeCallCount = 0;
            int sizeChangedCallCount = 0;
            int clientSizeChangedCallCount = 0;
            int parentLayoutCallCount = 0;
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
            void parentHandler(object sender, LayoutEventArgs e)
            {
                Assert.Same(parent, sender);
                Assert.Same(control, e.AffectedControl);
                Assert.Equal("Bounds", e.AffectedProperty);
                parentLayoutCallCount++;
            };
            parent.Layout += parentHandler;

            try
            {
                control.Location = value;
                Assert.Equal(new Size(0, 0), control.ClientSize);
                Assert.Equal(new Rectangle(0, 0, 0, 0), control.ClientRectangle);
                Assert.Equal(new Rectangle(0, 0, 0, 0), control.DisplayRectangle);
                Assert.Equal(new Size(0, 0), control.Size);
                Assert.Equal(value.X, control.Left);
                Assert.Equal(value.X, control.Right);
                Assert.Equal(value, control.Location);
                Assert.Equal(value.Y, control.Top);
                Assert.Equal(value.Y, control.Bottom);
                Assert.Equal(0, control.Width);
                Assert.Equal(0, control.Height);
                Assert.Equal(new Rectangle(value.X, value.Y, 0, 0), control.Bounds);
                Assert.Equal(expectedLocationChangedCallCount, moveCallCount);
                Assert.Equal(expectedLocationChangedCallCount, locationChangedCallCount);
                Assert.Equal(0, layoutCallCount);
                Assert.Equal(expectedLocationChangedCallCount, parentLayoutCallCount);
                Assert.Equal(0, resizeCallCount);
                Assert.Equal(0, sizeChangedCallCount);
                Assert.Equal(0, clientSizeChangedCallCount);
                Assert.True(control.IsHandleCreated);
                Assert.Equal(0, invalidatedCallCount);
                Assert.Equal(0, styleChangedCallCount);
                Assert.Equal(0, createdCallCount);
                Assert.True(parent.IsHandleCreated);
                Assert.Equal(0, parentInvalidatedCallCount);
                Assert.Equal(0, parentStyleChangedCallCount);
                Assert.Equal(0, parentCreatedCallCount);

                // Call again.
                control.Location = value;
                Assert.Equal(new Size(0, 0), control.ClientSize);
                Assert.Equal(new Rectangle(0, 0, 0, 0), control.ClientRectangle);
                Assert.Equal(new Rectangle(0, 0, 0, 0), control.DisplayRectangle);
                Assert.Equal(new Size(0, 0), control.Size);
                Assert.Equal(value.X, control.Left);
                Assert.Equal(value.X, control.Right);
                Assert.Equal(value, control.Location);
                Assert.Equal(value.Y, control.Top);
                Assert.Equal(value.Y, control.Bottom);
                Assert.Equal(0, control.Width);
                Assert.Equal(0, control.Height);
                Assert.Equal(new Rectangle(value.X, value.Y, 0, 0), control.Bounds);
                Assert.Equal(expectedLocationChangedCallCount, moveCallCount);
                Assert.Equal(expectedLocationChangedCallCount, locationChangedCallCount);
                Assert.Equal(0, layoutCallCount);
                Assert.Equal(expectedLocationChangedCallCount, parentLayoutCallCount);
                Assert.Equal(0, resizeCallCount);
                Assert.Equal(0, sizeChangedCallCount);
                Assert.Equal(0, clientSizeChangedCallCount);
                Assert.True(control.IsHandleCreated);
                Assert.Equal(0, invalidatedCallCount);
                Assert.Equal(0, styleChangedCallCount);
                Assert.Equal(0, createdCallCount);
                Assert.True(parent.IsHandleCreated);
                Assert.Equal(0, parentInvalidatedCallCount);
                Assert.Equal(0, parentStyleChangedCallCount);
                Assert.Equal(0, parentCreatedCallCount);
            }
            finally
            {
                parent.Layout -= parentHandler;
            }
        }

        [WinFormsTheory]
        [InlineData(true, 1)]
        [InlineData(false, 0)]
        public void Control_Location_SetWithHandleWithTransparentBackColor_DoesNotCallInvalidate(bool supportsTransparentBackgroundColor, int expectedInvalidatedCallCount)
        {
            using var control = new SubControl();
            control.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            control.BackColor = Color.FromArgb(254, 255, 255, 255);
            control.SetStyle(ControlStyles.SupportsTransparentBackColor, supportsTransparentBackgroundColor);

            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;

            // Set different.
            control.Location = new Point(1, 2);
            Assert.Equal(new Point(1, 2), control.Location);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);

            // Set same.
            control.Location = new Point(1, 2);
            Assert.Equal(new Point(1, 2), control.Location);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);

            // Set different.
            control.Location = new Point(2, 3);
            Assert.Equal(new Point(2, 3), control.Location);
            Assert.Equal(expectedInvalidatedCallCount * 2, invalidatedCallCount);
        }

        [WinFormsFact]
        public void Control_Location_SetWithHandler_CallsLocationChanged()
        {
            using var control = new Control();
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

        public static IEnumerable<object[]> Margin_Get_TestData()
        {
            yield return new object[] { new Control(), new Padding(3) };
            yield return new object[] { new NonZeroDefaultMarginControl(), new Padding(1, 2, 3, 4) };
            yield return new object[] { new ZeroDefaultMarginControl(), Padding.Empty };
        }

        [WinFormsTheory]
        [MemberData(nameof(Margin_Get_TestData))]
        public void Control_Margin_GetWithCustomDefaultMargin_ReturnsExpected(Control control, Padding expected)
        {
            Assert.Equal(expected, control.Margin);
        }

        private class NonZeroDefaultMarginControl : Control
        {
            protected override Padding DefaultMargin => new Padding(1, 2, 3, 4);
        }

        private class ZeroDefaultMarginControl : Control
        {
            protected override Padding DefaultMargin => Padding.Empty;
        }

        public static IEnumerable<object[]> Margin_Set_TestData()
        {
            yield return new object[] { new Padding(), new Padding() };
            yield return new object[] { new Padding(1, 2, 3, 4), new Padding(1, 2, 3, 4) };
            yield return new object[] { new Padding(1), new Padding(1) };
            yield return new object[] { new Padding(-1, -2, -3, -4), Padding.Empty };
        }

        [WinFormsTheory]
        [MemberData(nameof(Margin_Set_TestData))]
        public void Control_Margin_Set_GetReturnsExpected(Padding value, Padding expected)
        {
            using var control = new Control();
            int layoutCallCount = 0;
            control.Layout += (sender, e) => layoutCallCount++;

            control.Margin = value;
            Assert.Equal(expected, control.Margin);
            Assert.Equal(0, layoutCallCount);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.Margin = value;
            Assert.Equal(expected, control.Margin);
            Assert.Equal(0, layoutCallCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(Margin_Set_TestData))]
        public void Control_Margin_SetWithParent_GetReturnsExpected(Padding value, Padding expected)
        {
            using var parent = new Control();
            using var control = new Control
            {
                Parent = parent
            };
            int layoutCallCount = 0;
            control.Layout += (sender, e) => layoutCallCount++;
            int parentLayoutCallCount = 0;
            parent.Layout += (sender, e) => parentLayoutCallCount++;

            control.Margin = value;
            Assert.Equal(expected, control.Margin);
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(1, parentLayoutCallCount);
            Assert.False(control.IsHandleCreated);
            Assert.False(parent.IsHandleCreated);

            // Set same.
            control.Margin = value;
            Assert.Equal(expected, control.Margin);
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(1, parentLayoutCallCount);
            Assert.False(control.IsHandleCreated);
            Assert.False(parent.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(Margin_Set_TestData))]
        public void Control_Margin_SetWithHandle_GetReturnsExpected(Padding value, Padding expected)
        {
            using var control = new Control();
            int layoutCallCount = 0;
            control.Layout += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(control, e.AffectedControl);
                Assert.Equal("Padding", e.AffectedProperty);
                layoutCallCount++;
            };
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.Margin = value;
            Assert.Equal(expected, control.Margin);
            Assert.Equal(0, layoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.Margin = value;
            Assert.Equal(expected, control.Margin);
            Assert.Equal(0, layoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [MemberData(nameof(Margin_Set_TestData))]
        public void Control_Margin_SetWithParentWithHandle_GetReturnsExpected(Padding value, Padding expected)
        {
            using var parent = new Control();
            using var control = new Control
            {
                Parent = parent
            };
            int layoutCallCount = 0;
            control.Layout += (sender, e) => layoutCallCount++;
            int parentLayoutCallCount = 0;
            parent.Layout += (sender, e) => parentLayoutCallCount++;
            Assert.NotEqual(IntPtr.Zero, parent.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;
            int parentInvalidatedCallCount = 0;
            parent.Invalidated += (sender, e) => parentInvalidatedCallCount++;
            int parentStyleChangedCallCount = 0;
            parent.StyleChanged += (sender, e) => parentStyleChangedCallCount++;
            int parentCreatedCallCount = 0;
            parent.HandleCreated += (sender, e) => parentCreatedCallCount++;

            control.Margin = value;
            Assert.Equal(expected, control.Margin);
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(1, parentLayoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.True(parent.IsHandleCreated);
            Assert.Equal(0, parentInvalidatedCallCount);
            Assert.Equal(0, parentStyleChangedCallCount);
            Assert.Equal(0, parentCreatedCallCount);

            // Set same.
            control.Margin = value;
            Assert.Equal(expected, control.Margin);
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(1, parentLayoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.True(parent.IsHandleCreated);
            Assert.Equal(0, parentInvalidatedCallCount);
            Assert.Equal(0, parentStyleChangedCallCount);
            Assert.Equal(0, parentCreatedCallCount);
        }

        [WinFormsFact]
        public void Control_Margin_ResetValue_Success()
        {
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(Control))[nameof(Control.Margin)];
            using var control = new Control();
            Assert.False(property.CanResetValue(control));

            control.Margin = new Padding(1, 2, 3, 4);
            Assert.Equal(new Padding(1, 2, 3, 4), control.Margin);
            Assert.True(property.CanResetValue(control));

            property.ResetValue(control);
            Assert.Equal(new Padding(3), control.Margin);
            Assert.False(property.CanResetValue(control));
        }

        [WinFormsFact]
        public void Control_Margin_ShouldSerializeValue_Success()
        {
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(Control))[nameof(Control.Margin)];
            using var control = new Control();
            Assert.False(property.ShouldSerializeValue(control));

            control.Margin = new Padding(1, 2, 3, 4);
            Assert.Equal(new Padding(1, 2, 3, 4), control.Margin);
            Assert.True(property.ShouldSerializeValue(control));

            property.ResetValue(control);
            Assert.Equal(new Padding(3), control.Margin);
            Assert.False(property.ShouldSerializeValue(control));
        }

        public static IEnumerable<object[]> MaximumSize_Get_TestData()
        {
            yield return new object[] { new Control(), Size.Empty };
            yield return new object[] { new NonZeroWidthDefaultMaximumSizeControl(), new Size(10, 0) };
            yield return new object[] { new NonZeroHeightDefaultMaximumSizeControl(), new Size(0, 20) };
            yield return new object[] { new NonZeroWidthNonZeroHeightDefaultMaximumSizeControl(), new Size(10, 20) };
        }

        [WinFormsTheory]
        [MemberData(nameof(MaximumSize_Get_TestData))]
        public void Control_MaximumSize_GetWithCustomDefaultMaximumSize_ReturnsExpected(Control control, Size expected)
        {
            Assert.Equal(expected, control.MaximumSize);
        }

        private class NonZeroWidthDefaultMaximumSizeControl : Control
        {
            protected override Size DefaultMaximumSize => new Size(10, 0);
        }

        private class NonZeroHeightDefaultMaximumSizeControl : Control
        {
            protected override Size DefaultMaximumSize => new Size(0, 20);
        }

        private class NonZeroWidthNonZeroHeightDefaultMaximumSizeControl : Control
        {
            protected override Size DefaultMaximumSize => new Size(10, 20);
        }

        public static IEnumerable<object[]> MaximumSize_Set_TestData()
        {
            yield return new object[] { Size.Empty };
            yield return new object[] { new Size(-1, -2) };
            yield return new object[] { new Size(0, 1) };
            yield return new object[] { new Size(0, 10) };
            yield return new object[] { new Size(1, 0) };
            yield return new object[] { new Size(10, 0) };
            yield return new object[] { new Size(1, 2) };
            yield return new object[] { new Size(3, 4) };
            yield return new object[] { new Size(ushort.MaxValue - 1, ushort.MaxValue - 1) };
            yield return new object[] { new Size(ushort.MaxValue, ushort.MaxValue) };
            yield return new object[] { new Size(ushort.MaxValue + 1, ushort.MaxValue + 1) };
            yield return new object[] { new Size(int.MaxValue, int.MaxValue) };
        }

        [WinFormsTheory]
        [MemberData(nameof(MaximumSize_Set_TestData))]
        public void Control_MaximumSize_Set_GetReturnsExpected(Size value)
        {
            using var control = new Control();
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
        [MemberData(nameof(MaximumSize_Set_TestData))]
        public void Control_MaximumSize_SetWithCustomOldValue_GetReturnsExpected(Size value)
        {
            using var control = new Control
            {
                MaximumSize = new Size(1, 2)
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

        public static IEnumerable<object[]> MaximumSize_SetWithSize_TestData()
        {
            yield return new object[] { new Size(2, 3), Size.Empty, new Size(2, 3), 0 };
            yield return new object[] { new Size(2, 3), new Size(0, 1), new Size(0, 1), 1 };
            yield return new object[] { new Size(2, 3), new Size(0, 10), new Size(0, 3), 1 };
            yield return new object[] { new Size(2, 3), new Size(1, 0), new Size(1, 0), 1 };
            yield return new object[] { new Size(2, 3), new Size(10, 0), new Size(2, 0), 1 };
            yield return new object[] { new Size(2, 3), new Size(1, 2), new Size(1, 2), 1 };
            yield return new object[] { new Size(2, 3), new Size(2, 2), new Size(2, 2), 1 };
            yield return new object[] { new Size(2, 3), new Size(1, 3), new Size(1, 3), 1 };
            yield return new object[] { new Size(2, 3), new Size(2, 3), new Size(2, 3), 0 };
            yield return new object[] { new Size(2, 3), new Size(3, 3), new Size(2, 3), 0 };
            yield return new object[] { new Size(2, 3), new Size(2, 4), new Size(2, 3), 0 };
            yield return new object[] { new Size(2, 3), new Size(3, 4), new Size(2, 3), 0 };
            yield return new object[] { new Size(2, 3), new Size(5, 6), new Size(2, 3), 0 };
            yield return new object[] { new Size(2, 3), new Size(ushort.MaxValue - 1, ushort.MaxValue - 1), new Size(2, 3), 0 };
            yield return new object[] { new Size(2, 3), new Size(ushort.MaxValue, ushort.MaxValue), new Size(2, 3), 0 };
            yield return new object[] { new Size(2, 3), new Size(ushort.MaxValue + 1, ushort.MaxValue + 1), new Size(2, 3), 0 };
            yield return new object[] { new Size(2, 3), new Size(int.MaxValue, int.MaxValue), new Size(2, 3), 0 };
        }

        [WinFormsTheory]
        [MemberData(nameof(MaximumSize_SetWithSize_TestData))]
        public void Control_MaximumSize_SetWithSize_GetReturnsExpected(Size size, Size value, Size expectedSize, int expectedLayoutCallCount)
        {
            using var control = new Control
            {
                Size = size
            };
            int layoutCallCount = 0;
            control.Layout += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(control, e.AffectedControl);
                Assert.Same("Bounds", e.AffectedProperty);
                layoutCallCount++;
            };

            control.MaximumSize = value;
            Assert.Equal(value, control.MaximumSize);
            Assert.Equal(expectedSize, control.Size);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.MaximumSize = value;
            Assert.Equal(value, control.MaximumSize);
            Assert.Equal(expectedSize, control.Size);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(MaximumSize_Set_TestData))]
        public void Control_MaximumSize_SetWithMinimumSize_GetReturnsExpected(Size value)
        {
            using var control = new Control
            {
                MinimumSize = new Size(100, 100)
            };
            int layoutCallCount = 0;
            control.Layout += (sender, e) => layoutCallCount++;

            control.MaximumSize = value;
            Assert.Equal(value, control.MaximumSize);
            Assert.Equal(new Size(100, 100), control.Size);
            Assert.Equal(0, layoutCallCount);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.MaximumSize = value;
            Assert.Equal(value, control.MaximumSize);
            Assert.Equal(new Size(100, 100), control.Size);
            Assert.Equal(0, layoutCallCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(MaximumSize_SetWithSize_TestData))]
        public void Control_MaximumSize_SetWithCustomOldValueWithSize_GetReturnsExpected(Size size, Size value, Size expectedSize, int expectedLayoutCallCount)
        {
            using var control = new Control
            {
                MaximumSize = new Size(4, 5),
                Size = size
            };
            int layoutCallCount = 0;
            control.Layout += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(control, e.AffectedControl);
                Assert.Same("Bounds", e.AffectedProperty);
                layoutCallCount++;
            };

            control.MaximumSize = value;
            Assert.Equal(value, control.MaximumSize);
            Assert.Equal(expectedSize, control.Size);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.MaximumSize = value;
            Assert.Equal(value, control.MaximumSize);
            Assert.Equal(expectedSize, control.Size);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.False(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> MaximumSize_SetWithParent_TestData()
        {
            yield return new object[] { Size.Empty, 0 };
            yield return new object[] { new Size(0, 1), 1 };
            yield return new object[] { new Size(0, 10), 1 };
            yield return new object[] { new Size(1, 0), 1 };
            yield return new object[] { new Size(10, 0), 1 };
            yield return new object[] { new Size(-1, -2), 1 };
            yield return new object[] { new Size(1, 2), 1 };
            yield return new object[] { new Size(ushort.MaxValue - 1, ushort.MaxValue - 1), 1 };
            yield return new object[] { new Size(ushort.MaxValue, ushort.MaxValue), 1 };
            yield return new object[] { new Size(ushort.MaxValue + 1, ushort.MaxValue + 1), 1 };
            yield return new object[] { new Size(int.MaxValue, int.MaxValue), 1 };
        }

        [WinFormsTheory]
        [MemberData(nameof(MaximumSize_SetWithParent_TestData))]
        public void Control_MaximumSize_SetWithParent_GetReturnsExpected(Size value, int expectedParentLayoutCallCount)
        {
            using var parent = new Control();
            using var control = new Control
            {
                Parent = parent
            };
            int layoutCallCount = 0;
            control.Layout += (sender, e) => layoutCallCount++;
            int parentLayoutCallCount = 0;
            void parentHandler(object sender, LayoutEventArgs e)
            {
                Assert.Same(parent, sender);
                Assert.Same(control, e.AffectedControl);
                Assert.Same("MaximumSize", e.AffectedProperty);
                parentLayoutCallCount++;
            };
            parent.Layout += parentHandler;

            control.MaximumSize = value;
            Assert.Equal(value, control.MaximumSize);
            Assert.Equal(Size.Empty, control.Size);
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.MaximumSize = value;
            Assert.Equal(value, control.MaximumSize);
            Assert.Equal(Size.Empty, control.Size);
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
            Assert.False(control.IsHandleCreated);

            parent.Layout -= parentHandler;
        }

        public static IEnumerable<object[]> MaximumSize_SetCustomOldValueWithParent_TestData()
        {
            yield return new object[] { Size.Empty, 0 };
            yield return new object[] { new Size(0, 1), 1 };
            yield return new object[] { new Size(0, 10), 1 };
            yield return new object[] { new Size(1, 0), 1 };
            yield return new object[] { new Size(10, 0), 1 };
            yield return new object[] { new Size(-1, -2), 1 };
            yield return new object[] { new Size(1, 2), 0 };
            yield return new object[] { new Size(ushort.MaxValue - 1, ushort.MaxValue - 1), 1 };
            yield return new object[] { new Size(ushort.MaxValue, ushort.MaxValue), 1 };
            yield return new object[] { new Size(ushort.MaxValue + 1, ushort.MaxValue + 1), 1 };
            yield return new object[] { new Size(int.MaxValue, int.MaxValue), 1 };
        }

        [WinFormsTheory]
        [MemberData(nameof(MaximumSize_SetCustomOldValueWithParent_TestData))]
        public void Control_MaximumSize_SetWithCustomOldValueWithParent_GetReturnsExpected(Size value, int expectedParentLayoutCallCount)
        {
            using var parent = new Control();
            using var control = new Control
            {
                MaximumSize = new Size(1, 2),
                Parent = parent
            };
            int layoutCallCount = 0;
            control.Layout += (sender, e) => layoutCallCount++;
            int parentLayoutCallCount = 0;
            void parentHandler(object sender, LayoutEventArgs e)
            {
                Assert.Same(parent, sender);
                Assert.Same(control, e.AffectedControl);
                Assert.Same("MaximumSize", e.AffectedProperty);
                parentLayoutCallCount++;
            };
            parent.Layout += parentHandler;

            control.MaximumSize = value;
            Assert.Equal(value, control.MaximumSize);
            Assert.Equal(Size.Empty, control.Size);
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
            Assert.False(control.IsHandleCreated);
            Assert.False(parent.IsHandleCreated);

            // Set same.
            control.MaximumSize = value;
            Assert.Equal(value, control.MaximumSize);
            Assert.Equal(Size.Empty, control.Size);
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
            Assert.False(control.IsHandleCreated);
            Assert.False(parent.IsHandleCreated);

            parent.Layout -= parentHandler;
        }

        [WinFormsTheory]
        [MemberData(nameof(MaximumSize_Set_TestData))]
        public void Control_MaximumSize_SetWithHandle_GetReturnsExpected(Size value)
        {
            using var control = new Control();
            int layoutCallCount = 0;
            control.Layout += (sender, e) => layoutCallCount++;
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.MaximumSize = value;
            Assert.Equal(value, control.MaximumSize);
            Assert.Equal(Size.Empty, control.Size);
            Assert.Equal(0, layoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.MaximumSize = value;
            Assert.Equal(value, control.MaximumSize);
            Assert.Equal(Size.Empty, control.Size);
            Assert.Equal(0, layoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [MemberData(nameof(MaximumSize_Set_TestData))]
        public void Control_MaximumSize_SetWithCustomOldValueWithHandle_GetReturnsExpected(Size value)
        {
            using var control = new Control
            {
                MaximumSize = new Size(1, 2)
            };
            int layoutCallCount = 0;
            control.Layout += (sender, e) => layoutCallCount++;
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.MaximumSize = value;
            Assert.Equal(value, control.MaximumSize);
            Assert.Equal(Size.Empty, control.Size);
            Assert.Equal(0, layoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.MaximumSize = value;
            Assert.Equal(value, control.MaximumSize);
            Assert.Equal(Size.Empty, control.Size);
            Assert.Equal(0, layoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [MemberData(nameof(MaximumSize_SetWithSize_TestData))]
        public void Control_MaximumSize_SetWithSizeWithHandle_GetReturnsExpected(Size size, Size value, Size expectedSize, int expectedLayoutCallCount)
        {
            using var control = new Control
            {
                Size = size
            };
            int layoutCallCount = 0;
            control.Layout += (sender, e) => layoutCallCount++;
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.MaximumSize = value;
            Assert.Equal(value, control.MaximumSize);
            Assert.Equal(expectedSize, control.Size);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.MaximumSize = value;
            Assert.Equal(value, control.MaximumSize);
            Assert.Equal(expectedSize, control.Size);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [MemberData(nameof(MaximumSize_SetWithParent_TestData))]
        public void Control_MaximumSize_SetWithParentWithHandle_GetReturnsExpected(Size value, int expectedParentLayoutCallCount)
        {
            using var parent = new Control();
            using var control = new Control
            {
                Parent = parent
            };
            int layoutCallCount = 0;
            control.Layout += (sender, e) => layoutCallCount++;
            int parentLayoutCallCount = 0;
            void parentHandler(object sender, LayoutEventArgs e)
            {
                Assert.Same(parent, sender);
                Assert.Same(control, e.AffectedControl);
                Assert.Same("MaximumSize", e.AffectedProperty);
                parentLayoutCallCount++;
            };
            parent.Layout += parentHandler;
            Assert.NotEqual(IntPtr.Zero, parent.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;
            int parentInvalidatedCallCount = 0;
            parent.Invalidated += (sender, e) => parentInvalidatedCallCount++;
            int parentStyleChangedCallCount = 0;
            parent.StyleChanged += (sender, e) => parentStyleChangedCallCount++;
            int parentCreatedCallCount = 0;
            parent.HandleCreated += (sender, e) => parentCreatedCallCount++;

            control.MaximumSize = value;
            Assert.Equal(value, control.MaximumSize);
            Assert.Equal(Size.Empty, control.Size);
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.True(parent.IsHandleCreated);
            Assert.Equal(0, parentInvalidatedCallCount);
            Assert.Equal(0, parentStyleChangedCallCount);
            Assert.Equal(0, parentCreatedCallCount);

            // Set same.
            control.MaximumSize = value;
            Assert.Equal(value, control.MaximumSize);
            Assert.Equal(Size.Empty, control.Size);
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.True(parent.IsHandleCreated);
            Assert.Equal(0, parentInvalidatedCallCount);
            Assert.Equal(0, parentStyleChangedCallCount);
            Assert.Equal(0, parentCreatedCallCount);

            parent.Layout -= parentHandler;
        }

        [WinFormsTheory]
        [MemberData(nameof(MaximumSize_SetCustomOldValueWithParent_TestData))]
        public void Control_MaximumSize_SetWithCustomOldValueWithParentWithHandle_GetReturnsExpected(Size value, int expectedParentLayoutCallCount)
        {
            using var parent = new Control();
            using var control = new Control
            {
                MaximumSize = new Size(1, 2),
                Parent = parent
            };
            int layoutCallCount = 0;
            control.Layout += (sender, e) => layoutCallCount++;
            int parentLayoutCallCount = 0;
            void parentHandler(object sender, LayoutEventArgs e)
            {
                Assert.Same(parent, sender);
                Assert.Same(control, e.AffectedControl);
                Assert.Same("MaximumSize", e.AffectedProperty);
                parentLayoutCallCount++;
            };
            parent.Layout += parentHandler;
            Assert.NotEqual(IntPtr.Zero, parent.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;
            int parentInvalidatedCallCount = 0;
            parent.Invalidated += (sender, e) => parentInvalidatedCallCount++;
            int parentStyleChangedCallCount = 0;
            parent.StyleChanged += (sender, e) => parentStyleChangedCallCount++;
            int parentCreatedCallCount = 0;
            parent.HandleCreated += (sender, e) => parentCreatedCallCount++;

            control.MaximumSize = value;
            Assert.Equal(value, control.MaximumSize);
            Assert.Equal(Size.Empty, control.Size);
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.True(parent.IsHandleCreated);
            Assert.Equal(0, parentInvalidatedCallCount);
            Assert.Equal(0, parentStyleChangedCallCount);
            Assert.Equal(0, parentCreatedCallCount);

            // Set same.
            control.MaximumSize = value;
            Assert.Equal(value, control.MaximumSize);
            Assert.Equal(Size.Empty, control.Size);
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.True(parent.IsHandleCreated);
            Assert.Equal(0, parentInvalidatedCallCount);
            Assert.Equal(0, parentStyleChangedCallCount);
            Assert.Equal(0, parentCreatedCallCount);

            parent.Layout -= parentHandler;
        }

        [WinFormsFact]
        public void Control_MaximumSize_ResetValue_Success()
        {
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(Control))[nameof(Control.MaximumSize)];
            using var control = new Control();
            Assert.False(property.CanResetValue(control));

            control.MaximumSize = new Size(1, 2);
            Assert.Equal(new Size(1, 2), control.MaximumSize);
            Assert.True(property.CanResetValue(control));

            property.ResetValue(control);
            Assert.Equal(Size.Empty, control.MaximumSize);
            Assert.False(property.CanResetValue(control));
        }

        [WinFormsFact]
        public void Control_MaximumSize_ShouldSerializeValue_Success()
        {
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(Control))[nameof(Control.MaximumSize)];
            using var control = new Control();
            Assert.False(property.ShouldSerializeValue(control));

            control.MaximumSize = new Size(1, 2);
            Assert.Equal(new Size(1, 2), control.MaximumSize);
            Assert.True(property.ShouldSerializeValue(control));

            property.ResetValue(control);
            Assert.Equal(Size.Empty, control.MaximumSize);
            Assert.False(property.ShouldSerializeValue(control));
        }

        public static IEnumerable<object[]> MinimumSize_Get_TestData()
        {
            yield return new object[] { new Control(), Size.Empty };
            yield return new object[] { new NonZeroWidthDefaultMinimumSizeControl(), new Size(10, 0) };
            yield return new object[] { new NonZeroHeightDefaultMinimumSizeControl(), new Size(0, 20) };
            yield return new object[] { new NonZeroWidthNonZeroHeightDefaultMinimumSizeControl(), new Size(10, 20) };
        }

        [WinFormsTheory]
        [MemberData(nameof(MinimumSize_Get_TestData))]
        public void Control_MinimumSize_GetWithCustomDefaultMinimumSize_ReturnsExpected(Control control, Size expected)
        {
            Assert.Equal(expected, control.MinimumSize);
        }

        private class NonZeroWidthDefaultMinimumSizeControl : Control
        {
            protected override Size DefaultMinimumSize => new Size(10, 0);
        }

        private class NonZeroHeightDefaultMinimumSizeControl : Control
        {
            protected override Size DefaultMinimumSize => new Size(0, 20);
        }

        private class NonZeroWidthNonZeroHeightDefaultMinimumSizeControl : Control
        {
            protected override Size DefaultMinimumSize => new Size(10, 20);
        }

        public static IEnumerable<object[]> MinimumSize_Set_TestData()
        {
            yield return new object[] { Size.Empty, Size.Empty, 0 };
            yield return new object[] { new Size(0, 1), new Size(0, 1), 1 };
            yield return new object[] { new Size(0, 10), new Size(0, 10), 1 };
            yield return new object[] { new Size(1, 0), new Size(1, 0), 1 };
            yield return new object[] { new Size(10, 0), new Size(10, 0), 1 };
            yield return new object[] { new Size(-1, -2), Size.Empty, 0 };
            yield return new object[] { new Size(1, 2), new Size(1, 2), 1 };
            yield return new object[] { new Size(3, 4), new Size(3, 4), 1 };
            yield return new object[] { new Size(ushort.MaxValue - 1, ushort.MaxValue - 1), new Size(ushort.MaxValue - 1, ushort.MaxValue - 1), 1 };
            yield return new object[] { new Size(ushort.MaxValue, ushort.MaxValue), new Size(ushort.MaxValue, ushort.MaxValue), 1 };
            yield return new object[] { new Size(ushort.MaxValue + 1, ushort.MaxValue + 1), new Size(ushort.MaxValue + 1, ushort.MaxValue + 1), 1 };
            yield return new object[] { new Size(int.MaxValue, int.MaxValue), new Size(int.MaxValue, int.MaxValue), 1 };
        }

        [WinFormsTheory]
        [MemberData(nameof(MinimumSize_Set_TestData))]
        public void Control_MinimumSize_Set_GetReturnsExpected(Size value, Size expectedSize, int expectedLayoutCallCount)
        {
            using var control = new Control();
            int layoutCallCount = 0;
            control.Layout += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(control, e.AffectedControl);
                Assert.Same("Bounds", e.AffectedProperty);
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

        public static IEnumerable<object[]> MinimumSize_SetWithCustomOldValue_TestData()
        {
            yield return new object[] { Size.Empty, new Size(1, 2), 0 };
            yield return new object[] { new Size(0, 1), new Size(1, 2), 0 };
            yield return new object[] { new Size(0, 10), new Size(1, 10), 1 };
            yield return new object[] { new Size(1, 0), new Size(1, 2), 0 };
            yield return new object[] { new Size(10, 0), new Size(10, 2), 1 };
            yield return new object[] { new Size(-1, -2), new Size(1, 2), 0 };
            yield return new object[] { new Size(1, 2), new Size(1, 2), 0 };
            yield return new object[] { new Size(3, 4), new Size(3, 4), 1 };
            yield return new object[] { new Size(ushort.MaxValue - 1, ushort.MaxValue - 1), new Size(ushort.MaxValue - 1, ushort.MaxValue - 1), 1 };
            yield return new object[] { new Size(ushort.MaxValue, ushort.MaxValue), new Size(ushort.MaxValue, ushort.MaxValue), 1 };
            yield return new object[] { new Size(ushort.MaxValue + 1, ushort.MaxValue + 1), new Size(ushort.MaxValue + 1, ushort.MaxValue + 1), 1 };
            yield return new object[] { new Size(int.MaxValue, int.MaxValue), new Size(int.MaxValue, int.MaxValue), 1 };
        }

        [WinFormsTheory]
        [MemberData(nameof(MinimumSize_SetWithCustomOldValue_TestData))]
        public void Control_MinimumSize_SetWithCustomOldValue_GetReturnsExpected(Size value, Size expectedSize, int expectedLayoutCallCount)
        {
            using var control = new Control
            {
                MinimumSize = new Size(1, 2)
            };
            int layoutCallCount = 0;
            control.Layout += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(control, e.AffectedControl);
                Assert.Same("Bounds", e.AffectedProperty);
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

        public static IEnumerable<object[]> MinimumSize_SetWithSize_TestData()
        {
            yield return new object[] { new Size(2, 3), Size.Empty, new Size(2, 3), 0 };
            yield return new object[] { new Size(2, 3), new Size(0, 1), new Size(2, 3), 0 };
            yield return new object[] { new Size(2, 3), new Size(0, 10), new Size(2, 10), 1 };
            yield return new object[] { new Size(2, 3), new Size(1, 0), new Size(2, 3), 0 };
            yield return new object[] { new Size(2, 3), new Size(10, 0), new Size(10, 3), 1 };
            yield return new object[] { new Size(2, 3), new Size(-1, -2), new Size(2, 3), 0 };
            yield return new object[] { new Size(2, 3), new Size(1, 2), new Size(2, 3), 0 };
            yield return new object[] { new Size(2, 3), new Size(2, 2), new Size(2, 3), 0 };
            yield return new object[] { new Size(2, 3), new Size(1, 3), new Size(2, 3), 0 };
            yield return new object[] { new Size(2, 3), new Size(2, 3), new Size(2, 3), 0 };
            yield return new object[] { new Size(2, 3), new Size(3, 3), new Size(3, 3), 1 };
            yield return new object[] { new Size(2, 3), new Size(2, 4), new Size(2, 4), 1 };
            yield return new object[] { new Size(2, 3), new Size(3, 4), new Size(3, 4), 1 };
            yield return new object[] { new Size(2, 3), new Size(5, 6), new Size(5, 6), 1 };
            yield return new object[] { new Size(2, 3), new Size(ushort.MaxValue - 1, ushort.MaxValue - 1), new Size(ushort.MaxValue - 1, ushort.MaxValue - 1), 1 };
            yield return new object[] { new Size(2, 3), new Size(ushort.MaxValue, ushort.MaxValue), new Size(ushort.MaxValue, ushort.MaxValue), 1 };
            yield return new object[] { new Size(2, 3), new Size(ushort.MaxValue + 1, ushort.MaxValue + 1), new Size(ushort.MaxValue + 1, ushort.MaxValue + 1), 1 };
            yield return new object[] { new Size(2, 3), new Size(int.MaxValue, int.MaxValue), new Size(int.MaxValue, int.MaxValue), 1 };
        }

        [WinFormsTheory]
        [MemberData(nameof(MinimumSize_SetWithSize_TestData))]
        public void Control_MinimumSize_SetWithSize_GetReturnsExpected(Size size, Size value, Size expectedSize, int expectedLayoutCallCount)
        {
            using var control = new Control
            {
                Size = size
            };
            int layoutCallCount = 0;
            control.Layout += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(control, e.AffectedControl);
                Assert.Same("Bounds", e.AffectedProperty);
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
        [MemberData(nameof(MinimumSize_Set_TestData))]
        public void Control_MinimumSize_SetWithMaximumSize_GetReturnsExpected(Size value, Size expectedSize, int expectedLayoutCallCount)
        {
            using var control = new Control
            {
                MaximumSize = new Size(-1, -2)
            };
            int layoutCallCount = 0;
            control.Layout += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(control, e.AffectedControl);
                Assert.Same("Bounds", e.AffectedProperty);
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
        [MemberData(nameof(MinimumSize_SetWithSize_TestData))]
        public void Control_MinimumSize_SetWithCustomOldValueWithSize_GetReturnsExpected(Size size, Size value, Size expectedSize, int expectedLayoutCallCount)
        {
            using var control = new Control
            {
                MinimumSize = new Size(1, 2),
                Size = size
            };
            int layoutCallCount = 0;
            control.Layout += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(control, e.AffectedControl);
                Assert.Same("Bounds", e.AffectedProperty);
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

        public static IEnumerable<object[]> MinimumSize_SetWithParent_TestData()
        {
            yield return new object[] { Size.Empty, Size.Empty, 0, 0 };
            yield return new object[] { new Size(0, 1), new Size(0, 1), 1, 1 };
            yield return new object[] { new Size(0, 10), new Size(0, 10), 1, 1 };
            yield return new object[] { new Size(1, 0), new Size(1, 0), 1, 1 };
            yield return new object[] { new Size(10, 0), new Size(10, 0), 1, 1 };
            yield return new object[] { new Size(-1, -2), Size.Empty, 0, 1 };
            yield return new object[] { new Size(1, 2), new Size(1, 2), 1, 1 };
            yield return new object[] { new Size(ushort.MaxValue - 1, ushort.MaxValue - 1), new Size(ushort.MaxValue - 1, ushort.MaxValue - 1), 1, 1 };
            yield return new object[] { new Size(ushort.MaxValue, ushort.MaxValue), new Size(ushort.MaxValue, ushort.MaxValue), 1, 1 };
            yield return new object[] { new Size(ushort.MaxValue + 1, ushort.MaxValue + 1), new Size(ushort.MaxValue + 1, ushort.MaxValue + 1), 1, 1 };
            yield return new object[] { new Size(int.MaxValue, int.MaxValue), new Size(int.MaxValue, int.MaxValue), 1, 1 };
        }

        [WinFormsTheory]
        [MemberData(nameof(MinimumSize_SetWithParent_TestData))]
        public void Control_MinimumSize_SetWithParent_GetReturnsExpected(Size value, Size expectedSize, int expectedLayoutCallCount, int expectedParentLayoutCallCount)
        {
            using var parent = new Control();
            using var control = new Control
            {
                Parent = parent
            };
            int layoutCallCount = 0;
            control.Layout += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(control, e.AffectedControl);
                Assert.Same("Bounds", e.AffectedProperty);
                layoutCallCount++;
            };
            int parentLayoutCallCount = 0;
            void parentHandler(object sender, LayoutEventArgs e)
            {
                Assert.Same(parent, sender);
                Assert.Same(control, e.AffectedControl);
                Assert.Same("MinimumSize", e.AffectedProperty);
                parentLayoutCallCount++;
            };
            parent.Layout += parentHandler;

            control.MinimumSize = value;
            Assert.Equal(value, control.MinimumSize);
            Assert.Equal(expectedSize, control.Size);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.MinimumSize = value;
            Assert.Equal(value, control.MinimumSize);
            Assert.Equal(expectedSize, control.Size);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
            Assert.False(control.IsHandleCreated);

            parent.Layout -= parentHandler;
        }

        public static IEnumerable<object[]> MinimumSize_SetCustomOldValueWithParent_TestData()
        {
            yield return new object[] { Size.Empty, new Size(1, 2), 0, 1 };
            yield return new object[] { new Size(0, 1), new Size(1, 2), 0, 1 };
            yield return new object[] { new Size(0, 10), new Size(1, 10), 1, 1 };
            yield return new object[] { new Size(1, 0), new Size(1, 2), 0, 1 };
            yield return new object[] { new Size(10, 0), new Size(10, 2), 1, 1 };
            yield return new object[] { new Size(-1, -2), new Size(1, 2), 0, 1 };
            yield return new object[] { new Size(1, 2), new Size(1, 2), 0, 0 };
            yield return new object[] { new Size(ushort.MaxValue - 1, ushort.MaxValue - 1), new Size(ushort.MaxValue - 1, ushort.MaxValue - 1), 1, 1 };
            yield return new object[] { new Size(ushort.MaxValue, ushort.MaxValue), new Size(ushort.MaxValue, ushort.MaxValue), 1, 1 };
            yield return new object[] { new Size(ushort.MaxValue + 1, ushort.MaxValue + 1), new Size(ushort.MaxValue + 1, ushort.MaxValue + 1), 1, 1 };
            yield return new object[] { new Size(int.MaxValue, int.MaxValue), new Size(int.MaxValue, int.MaxValue), 1, 1 };
        }

        [WinFormsTheory]
        [MemberData(nameof(MinimumSize_SetCustomOldValueWithParent_TestData))]
        public void Control_MinimumSize_SetWithCustomOldValueWithParent_GetReturnsExpected(Size value, Size expectedSize, int expectedLayoutCallCount, int expectedParentLayoutCallCount)
        {
            using var parent = new Control();
            using var control = new Control
            {
                MinimumSize = new Size(1, 2),
                Parent = parent
            };
            int layoutCallCount = 0;
            control.Layout += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(control, e.AffectedControl);
                Assert.Same("Bounds", e.AffectedProperty);
                layoutCallCount++;
            };
            int parentLayoutCallCount = 0;
            void parentHandler(object sender, LayoutEventArgs e)
            {
                Assert.Same(parent, sender);
                Assert.Same(control, e.AffectedControl);
                Assert.Same("MinimumSize", e.AffectedProperty);
                parentLayoutCallCount++;
            };
            parent.Layout += parentHandler;

            control.MinimumSize = value;
            Assert.Equal(value, control.MinimumSize);
            Assert.Equal(expectedSize, control.Size);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
            Assert.False(control.IsHandleCreated);
            Assert.False(parent.IsHandleCreated);

            // Set same.
            control.MinimumSize = value;
            Assert.Equal(value, control.MinimumSize);
            Assert.Equal(expectedSize, control.Size);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
            Assert.False(control.IsHandleCreated);
            Assert.False(parent.IsHandleCreated);

            parent.Layout -= parentHandler;
        }

        public static IEnumerable<object[]> MinimumSize_SetWithHandle_TestData()
        {
            yield return new object[] { Size.Empty, Size.Empty, 0 };
            yield return new object[] { new Size(0, 1), new Size(0, 1), 1 };
            yield return new object[] { new Size(0, 10), new Size(0, 10), 1 };
            yield return new object[] { new Size(1, 0), new Size(1, 0), 1 };
            yield return new object[] { new Size(10, 0), new Size(10, 0), 1 };
            yield return new object[] { new Size(-1, -2), Size.Empty, 0 };
            yield return new object[] { new Size(1, 2), new Size(1, 2), 1 };
            yield return new object[] { new Size(3, 4), new Size(3, 4), 1 };
            yield return new object[] { new Size(ushort.MaxValue - 1, ushort.MaxValue - 1), new Size(ushort.MaxValue - 1, ushort.MaxValue - 1), 1 };
            yield return new object[] { new Size(ushort.MaxValue, ushort.MaxValue), new Size(ushort.MaxValue, ushort.MaxValue), 1 };
            yield return new object[] { new Size(ushort.MaxValue + 1, ushort.MaxValue + 1), new Size(ushort.MaxValue, ushort.MaxValue), 1 };
            yield return new object[] { new Size(int.MaxValue, int.MaxValue), new Size(ushort.MaxValue, ushort.MaxValue), 1 };
        }

        [WinFormsTheory]
        [MemberData(nameof(MinimumSize_SetWithHandle_TestData))]
        public void Control_MinimumSize_SetWithHandle_GetReturnsExpected(Size value, Size expectedSize, int expectedLayoutCallCount)
        {
            using var control = new Control();
            int layoutCallCount = 0;
            control.Layout += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(control, e.AffectedControl);
                Assert.Same("Bounds", e.AffectedProperty);
                layoutCallCount++;
            };
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.MinimumSize = value;
            Assert.Equal(value, control.MinimumSize);
            Assert.Equal(expectedSize, control.Size);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.MinimumSize = value;
            Assert.Equal(value, control.MinimumSize);
            Assert.Equal(expectedSize, control.Size);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        public static IEnumerable<object[]> MinimumSize_SetWithCustomOldValueWithHandle_TestData()
        {
            yield return new object[] { Size.Empty, new Size(1, 2), 0 };
            yield return new object[] { new Size(0, 1), new Size(1, 2), 0 };
            yield return new object[] { new Size(0, 10), new Size(1, 10), 1 };
            yield return new object[] { new Size(1, 0), new Size(1, 2), 0 };
            yield return new object[] { new Size(10, 0), new Size(10, 2), 1 };
            yield return new object[] { new Size(-1, -2), new Size(1, 2), 0 };
            yield return new object[] { new Size(1, 2), new Size(1, 2), 0 };
            yield return new object[] { new Size(3, 4), new Size(3, 4), 1 };
            yield return new object[] { new Size(ushort.MaxValue - 1, ushort.MaxValue - 1), new Size(ushort.MaxValue - 1, ushort.MaxValue - 1), 1 };
            yield return new object[] { new Size(ushort.MaxValue, ushort.MaxValue), new Size(ushort.MaxValue, ushort.MaxValue), 1 };
            yield return new object[] { new Size(ushort.MaxValue + 1, ushort.MaxValue + 1), new Size(ushort.MaxValue, ushort.MaxValue), 1 };
            yield return new object[] { new Size(int.MaxValue, int.MaxValue), new Size(ushort.MaxValue, ushort.MaxValue), 1 };
        }

        [WinFormsTheory]
        [MemberData(nameof(MinimumSize_SetWithCustomOldValueWithHandle_TestData))]
        public void Control_MinimumSize_SetWithCustomOldValueWithHandle_GetReturnsExpected(Size value, Size expectedSize, int expectedLayoutCallCount)
        {
            using var control = new Control
            {
                MinimumSize = new Size(1, 2)
            };
            int layoutCallCount = 0;
            control.Layout += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(control, e.AffectedControl);
                Assert.Same("Bounds", e.AffectedProperty);
                layoutCallCount++;
            };
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.MinimumSize = value;
            Assert.Equal(value, control.MinimumSize);
            Assert.Equal(expectedSize, control.Size);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.MinimumSize = value;
            Assert.Equal(value, control.MinimumSize);
            Assert.Equal(expectedSize, control.Size);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        public static IEnumerable<object[]> MinimumSize_SetWithSizeWithHandle_TestData()
        {
            yield return new object[] { new Size(2, 3), Size.Empty, new Size(2, 3), 0 };
            yield return new object[] { new Size(2, 3), new Size(0, 1), new Size(2, 3), 0 };
            yield return new object[] { new Size(2, 3), new Size(0, 10), new Size(2, 10), 1 };
            yield return new object[] { new Size(2, 3), new Size(1, 0), new Size(2, 3), 0 };
            yield return new object[] { new Size(2, 3), new Size(10, 0), new Size(10, 3), 1 };
            yield return new object[] { new Size(2, 3), new Size(-1, -2), new Size(2, 3), 0 };
            yield return new object[] { new Size(2, 3), new Size(1, 2), new Size(2, 3), 0 };
            yield return new object[] { new Size(2, 3), new Size(2, 2), new Size(2, 3), 0 };
            yield return new object[] { new Size(2, 3), new Size(1, 3), new Size(2, 3), 0 };
            yield return new object[] { new Size(2, 3), new Size(2, 3), new Size(2, 3), 0 };
            yield return new object[] { new Size(2, 3), new Size(3, 3), new Size(3, 3), 1 };
            yield return new object[] { new Size(2, 3), new Size(2, 4), new Size(2, 4), 1 };
            yield return new object[] { new Size(2, 3), new Size(3, 4), new Size(3, 4), 1 };
            yield return new object[] { new Size(2, 3), new Size(5, 6), new Size(5, 6), 1 };
            yield return new object[] { new Size(2, 3), new Size(ushort.MaxValue - 1, ushort.MaxValue - 1), new Size(ushort.MaxValue - 1, ushort.MaxValue - 1), 1 };
            yield return new object[] { new Size(2, 3), new Size(ushort.MaxValue, ushort.MaxValue), new Size(ushort.MaxValue, ushort.MaxValue), 1 };
            yield return new object[] { new Size(2, 3), new Size(ushort.MaxValue + 1, ushort.MaxValue + 1), new Size(ushort.MaxValue, ushort.MaxValue), 1 };
            yield return new object[] { new Size(2, 3), new Size(int.MaxValue, int.MaxValue), new Size(ushort.MaxValue, ushort.MaxValue), 1 };
        }

        [WinFormsTheory]
        [MemberData(nameof(MinimumSize_SetWithSizeWithHandle_TestData))]
        public void Control_MinimumSize_SetWithSizeWithHandle_GetReturnsExpected(Size size, Size value, Size expectedSize, int expectedLayoutCallCount)
        {
            using var control = new Control
            {
                Size = size
            };
            int layoutCallCount = 0;
            control.Layout += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(control, e.AffectedControl);
                Assert.Same("Bounds", e.AffectedProperty);
                layoutCallCount++;
            };
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.MinimumSize = value;
            Assert.Equal(value, control.MinimumSize);
            Assert.Equal(expectedSize, control.Size);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.MinimumSize = value;
            Assert.Equal(value, control.MinimumSize);
            Assert.Equal(expectedSize, control.Size);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        public static IEnumerable<object[]> MinimumSize_SetWithParentWithHandle_TestData()
        {
            yield return new object[] { Size.Empty, Size.Empty, 0, 0 };
            yield return new object[] { new Size(0, 1), new Size(0, 1), 1, 1 };
            yield return new object[] { new Size(0, 10), new Size(0, 10), 1, 1 };
            yield return new object[] { new Size(1, 0), new Size(1, 0), 1, 1 };
            yield return new object[] { new Size(10, 0), new Size(10, 0), 1, 1 };
            yield return new object[] { new Size(-1, -2), Size.Empty, 0, 1 };
            yield return new object[] { new Size(1, 2), new Size(1, 2), 1, 1 };
            yield return new object[] { new Size(ushort.MaxValue - 1, ushort.MaxValue - 1), new Size(ushort.MaxValue - 1, ushort.MaxValue - 1), 1, 1 };
            yield return new object[] { new Size(ushort.MaxValue, ushort.MaxValue), new Size(ushort.MaxValue, ushort.MaxValue), 1, 1 };
            yield return new object[] { new Size(ushort.MaxValue + 1, ushort.MaxValue + 1), new Size(ushort.MaxValue, ushort.MaxValue), 1, 1 };
            yield return new object[] { new Size(int.MaxValue, int.MaxValue), new Size(ushort.MaxValue, ushort.MaxValue), 1, 1 };
        }

        [WinFormsTheory]
        [MemberData(nameof(MinimumSize_SetWithParentWithHandle_TestData))]
        public void Control_MinimumSize_SetWithParentWithHandle_GetReturnsExpected(Size value, Size expectedSize, int expectedLayoutCallCount, int expectedParentLayoutCallCount)
        {
            using var parent = new Control();
            using var control = new Control
            {
                Parent = parent
            };
            int layoutCallCount = 0;
            control.Layout += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(control, e.AffectedControl);
                Assert.Same("Bounds", e.AffectedProperty);
                layoutCallCount++;
            };
            int parentLayoutCallCount = 0;
            void parentHandler(object sender, LayoutEventArgs e)
            {
                Assert.Same(parent, sender);
                Assert.Same(control, e.AffectedControl);
                Assert.Same("MinimumSize", e.AffectedProperty);
                parentLayoutCallCount++;
            };
            parent.Layout += parentHandler;
            Assert.NotEqual(IntPtr.Zero, parent.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;
            int parentInvalidatedCallCount = 0;
            parent.Invalidated += (sender, e) => parentInvalidatedCallCount++;
            int parentStyleChangedCallCount = 0;
            parent.StyleChanged += (sender, e) => parentStyleChangedCallCount++;
            int parentCreatedCallCount = 0;
            parent.HandleCreated += (sender, e) => parentCreatedCallCount++;

            control.MinimumSize = value;
            Assert.Equal(value, control.MinimumSize);
            Assert.Equal(expectedSize, control.Size);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.True(parent.IsHandleCreated);
            Assert.Equal(0, parentInvalidatedCallCount);
            Assert.Equal(0, parentStyleChangedCallCount);
            Assert.Equal(0, parentCreatedCallCount);

            // Set same.
            control.MinimumSize = value;
            Assert.Equal(value, control.MinimumSize);
            Assert.Equal(expectedSize, control.Size);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.True(parent.IsHandleCreated);
            Assert.Equal(0, parentInvalidatedCallCount);
            Assert.Equal(0, parentStyleChangedCallCount);
            Assert.Equal(0, parentCreatedCallCount);

            parent.Layout -= parentHandler;
        }

        public static IEnumerable<object[]> MinimumSize_SetCustomOldValueWithParentWithHandle_TestData()
        {
            yield return new object[] { Size.Empty, new Size(1, 2), 0, 1 };
            yield return new object[] { new Size(0, 1), new Size(1, 2), 0, 1 };
            yield return new object[] { new Size(0, 10), new Size(1, 10), 1, 1 };
            yield return new object[] { new Size(1, 0), new Size(1, 2), 0, 1 };
            yield return new object[] { new Size(10, 0), new Size(10, 2), 1, 1 };
            yield return new object[] { new Size(-1, -2), new Size(1, 2), 0, 1 };
            yield return new object[] { new Size(1, 2), new Size(1, 2), 0, 0 };
            yield return new object[] { new Size(ushort.MaxValue - 1, ushort.MaxValue - 1), new Size(ushort.MaxValue - 1, ushort.MaxValue - 1), 1, 1 };
            yield return new object[] { new Size(ushort.MaxValue, ushort.MaxValue), new Size(ushort.MaxValue, ushort.MaxValue), 1, 1 };
            yield return new object[] { new Size(ushort.MaxValue + 1, ushort.MaxValue + 1), new Size(ushort.MaxValue, ushort.MaxValue), 1, 1 };
            yield return new object[] { new Size(int.MaxValue, int.MaxValue), new Size(ushort.MaxValue, ushort.MaxValue), 1, 1 };
        }

        [WinFormsTheory]
        [MemberData(nameof(MinimumSize_SetCustomOldValueWithParentWithHandle_TestData))]
        public void Control_MinimumSize_SetWithCustomOldValueWithParentWithHandle_GetReturnsExpected(Size value, Size expectedSize, int expectedLayoutCallCount, int expectedParentLayoutCallCount)
        {
            using var parent = new Control();
            using var control = new Control
            {
                MinimumSize = new Size(1, 2),
                Parent = parent
            };
            int layoutCallCount = 0;
            control.Layout += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(control, e.AffectedControl);
                Assert.Same("Bounds", e.AffectedProperty);
                layoutCallCount++;
            };
            int parentLayoutCallCount = 0;
            void parentHandler(object sender, LayoutEventArgs e)
            {
                Assert.Same(parent, sender);
                Assert.Same(control, e.AffectedControl);
                Assert.Same("MinimumSize", e.AffectedProperty);
                parentLayoutCallCount++;
            };
            parent.Layout += parentHandler;
            Assert.NotEqual(IntPtr.Zero, parent.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;
            int parentInvalidatedCallCount = 0;
            parent.Invalidated += (sender, e) => parentInvalidatedCallCount++;
            int parentStyleChangedCallCount = 0;
            parent.StyleChanged += (sender, e) => parentStyleChangedCallCount++;
            int parentCreatedCallCount = 0;
            parent.HandleCreated += (sender, e) => parentCreatedCallCount++;

            control.MinimumSize = value;
            Assert.Equal(value, control.MinimumSize);
            Assert.Equal(expectedSize, control.Size);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.True(parent.IsHandleCreated);
            Assert.Equal(0, parentInvalidatedCallCount);
            Assert.Equal(0, parentStyleChangedCallCount);
            Assert.Equal(0, parentCreatedCallCount);

            // Set same.
            control.MinimumSize = value;
            Assert.Equal(value, control.MinimumSize);
            Assert.Equal(expectedSize, control.Size);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.True(parent.IsHandleCreated);
            Assert.Equal(0, parentInvalidatedCallCount);
            Assert.Equal(0, parentStyleChangedCallCount);
            Assert.Equal(0, parentCreatedCallCount);

            parent.Layout -= parentHandler;
        }

        [WinFormsFact]
        public void Control_MinimumSize_ResetValue_Success()
        {
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(Control))[nameof(Control.MinimumSize)];
            using var control = new Control();
            Assert.False(property.CanResetValue(control));

            control.MinimumSize = new Size(1, 2);
            Assert.Equal(new Size(1, 2), control.MinimumSize);
            Assert.True(property.CanResetValue(control));

            property.ResetValue(control);
            Assert.Equal(Size.Empty, control.MinimumSize);
            Assert.False(property.CanResetValue(control));
        }

        [WinFormsFact]
        public void Control_MinimumSize_ShouldSerializeValue_Success()
        {
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(Control))[nameof(Control.MinimumSize)];
            using var control = new Control();
            Assert.False(property.ShouldSerializeValue(control));

            control.MinimumSize = new Size(1, 2);
            Assert.Equal(new Size(1, 2), control.MinimumSize);
            Assert.True(property.ShouldSerializeValue(control));

            property.ResetValue(control);
            Assert.Equal(Size.Empty, control.MinimumSize);
            Assert.False(property.ShouldSerializeValue(control));
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void Control_Name_GetWithSite_ReturnsExpected(string siteName, string expected)
        {
            var mockSite = new Mock<ISite>(MockBehavior.Strict);
            mockSite
                .Setup(s => s.Container)
                .Returns((IContainer)null);
            mockSite
                .Setup(s => s.GetService(typeof(AmbientProperties)))
                .Returns(null);
            mockSite
                .Setup(s => s.Name)
                .Returns(siteName);
            using var control = new Control
            {
                Site = mockSite.Object
            };
            Assert.Equal(expected, control.Name);
            mockSite.Verify(s => s.Name, Times.Once());

            // Get again.
            Assert.Equal(expected, control.Name);
            mockSite.Verify(s => s.Name, Times.Exactly(2));
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void Control_Name_Set_GetReturnsExpected(string value, string expected)
        {
            using var control = new Control
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

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void Control_Name_SetWithCustomOldValue_GetReturnsExpected(string value, string expected)
        {
            using var control = new Control
            {
                Name = "oldName"
            };

            control.Name = value;
            Assert.Equal(expected, control.Name);
            Assert.False(control.IsHandleCreated);

            // Get again.
            control.Name = value;
            Assert.Equal(expected, control.Name);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData("name", null, "name", 0)]
        [InlineData("", null, "", 1)]
        [InlineData("", "", "", 1)]
        [InlineData("", "siteName", "siteName", 1)]
        [InlineData(null, null, "", 1)]
        [InlineData(null, "", "", 1)]
        [InlineData(null, "siteName", "siteName", 1)]
        public void Control_Name_SetWithSite_GetReturnsExpected(string value, string siteName, string expected, int expectedSiteCallCount)
        {
            var mockSite = new Mock<ISite>(MockBehavior.Strict);
            mockSite
                .Setup(s => s.Container)
                .Returns((IContainer)null);
            mockSite
                .Setup(s => s.GetService(typeof(AmbientProperties)))
                .Returns(null);
            mockSite
                .Setup(s => s.Name)
                .Returns(siteName);
            using var control = new Control
            {
                Site = mockSite.Object,
                Name = value
            };
            Assert.Equal(expected, control.Name);
            Assert.False(control.IsHandleCreated);
            mockSite.Verify(s => s.Name, Times.Exactly(expectedSiteCallCount));

            // Get again.
            Assert.Equal(expected, control.Name);
            Assert.False(control.IsHandleCreated);
            mockSite.Verify(s => s.Name, Times.Exactly(expectedSiteCallCount * 2));
        }

        public static IEnumerable<object[]> Padding_Get_TestData()
        {
            yield return new object[] { new Control(), Padding.Empty };
            yield return new object[] { new NonZeroDefaultPaddingControl(), new Padding(1, 2, 3, 4) };
        }

        [WinFormsTheory]
        [MemberData(nameof(Padding_Get_TestData))]
        public void Control_Padding_GetWithCustomDefaultPadding_ReturnsExpected(Control control, Padding expected)
        {
            Assert.Equal(expected, control.Padding);
        }

        private class NonZeroDefaultPaddingControl : Control
        {
            protected override Padding DefaultPadding => new Padding(1, 2, 3, 4);
        }

        public static IEnumerable<object[]> Padding_Set_TestData()
        {
            yield return new object[] { new Padding(), new Padding(), 0, 0 };
            yield return new object[] { new Padding(1, 2, 3, 4), new Padding(1, 2, 3, 4), 1, 1 };
            yield return new object[] { new Padding(1), new Padding(1), 1, 1 };
            yield return new object[] { new Padding(-1, -2, -3, -4), Padding.Empty, 1, 2 };
        }

        [WinFormsTheory]
        [MemberData(nameof(Padding_Set_TestData))]
        public void Control_Padding_Set_GetReturnsExpected(Padding value, Padding expected, int expectedLayoutCallCount1, int expectedLayoutCallCount2)
        {
            using var control = new Control();
            int layoutCallCount = 0;
            control.Layout += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(control, e.AffectedControl);
                Assert.Equal("Padding", e.AffectedProperty);
                layoutCallCount++;
            };

            control.Padding = value;
            Assert.Equal(expected, control.Padding);
            Assert.Equal(expectedLayoutCallCount1, layoutCallCount);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.Padding = value;
            Assert.Equal(expected, control.Padding);
            Assert.Equal(expectedLayoutCallCount2, layoutCallCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(Padding_Set_TestData))]
        public void Control_Padding_SetWithParent_GetReturnsExpected(Padding value, Padding expected, int expectedLayoutCallCount1, int expectedLayoutCallCount2)
        {
            using var parent = new Control();
            using var control = new Control
            {
                Parent = parent
            };
            int layoutCallCount = 0;
            control.Layout += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(control, e.AffectedControl);
                Assert.Equal("Padding", e.AffectedProperty);
                layoutCallCount++;
            };
            int parentLayoutCallCount = 0;
            void parentHandler(object sender, LayoutEventArgs e)
            {
                Assert.Same(parent, sender);
                Assert.Same(control, e.AffectedControl);
                Assert.Same("Padding", e.AffectedProperty);
                parentLayoutCallCount++;
            };
            parent.Layout += parentHandler;

            control.Padding = value;
            Assert.Equal(expected, control.Padding);
            Assert.Equal(expectedLayoutCallCount1, layoutCallCount);
            Assert.Equal(expectedLayoutCallCount1, parentLayoutCallCount);
            Assert.False(control.IsHandleCreated);
            Assert.False(parent.IsHandleCreated);

            // Set same.
            control.Padding = value;
            Assert.Equal(expected, control.Padding);
            Assert.Equal(expectedLayoutCallCount2, layoutCallCount);
            Assert.Equal(expectedLayoutCallCount2, parentLayoutCallCount);
            Assert.False(control.IsHandleCreated);
            Assert.False(parent.IsHandleCreated);

            parent.Layout -= parentHandler;
        }

        public static IEnumerable<object[]> Padding_SetWithHandle_TestData()
        {
            yield return new object[] { false, new Padding(), new Padding(), 0, 0, 0, 0 };
            yield return new object[] { false, new Padding(1, 2, 3, 4), new Padding(1, 2, 3, 4), 1, 0, 1, 0 };
            yield return new object[] { false, new Padding(1), new Padding(1), 1, 0, 1, 0 };
            yield return new object[] { false, new Padding(-1, -2, -3, -4), Padding.Empty, 1, 0, 2, 0 };
            yield return new object[] { true, new Padding(), new Padding(), 0, 0, 0, 0 };
            yield return new object[] { true, new Padding(1, 2, 3, 4), new Padding(1, 2, 3, 4), 1, 1, 1, 1 };
            yield return new object[] { true, new Padding(1), new Padding(1), 1, 1, 1, 1 };
            yield return new object[] { true, new Padding(-1, -2, -3, -4), Padding.Empty, 1, 1, 2, 2 };
        }

        [WinFormsTheory]
        [MemberData(nameof(Padding_SetWithHandle_TestData))]
        public void Control_Padding_SetWithHandle_GetReturnsExpected(bool resizeRedraw, Padding value, Padding expected, int expectedLayoutCallCount1, int expectedInvalidatedCallCount1, int expectedLayoutCallCount2, int expectedInvalidatedCallCount2)
        {
            using var control = new SubControl();
            control.SetStyle(ControlStyles.ResizeRedraw, resizeRedraw);
            int layoutCallCount = 0;
            control.Layout += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(control, e.AffectedControl);
                Assert.Equal("Padding", e.AffectedProperty);
                layoutCallCount++;
            };
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.Padding = value;
            Assert.Equal(expected, control.Padding);
            Assert.Equal(expectedLayoutCallCount1, layoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount1, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.Padding = value;
            Assert.Equal(expected, control.Padding);
            Assert.Equal(expectedLayoutCallCount2, layoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount2, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [MemberData(nameof(Padding_SetWithHandle_TestData))]
        public void Control_Padding_SetWithParentWithHandle_GetReturnsExpected(bool resizeRedraw, Padding value, Padding expected, int expectedLayoutCallCount1, int expectedInvalidatedCallCount1, int expectedLayoutCallCount2, int expectedInvalidatedCallCount2)
        {
            using var parent = new Control();
            using var control = new SubControl
            {
                Parent = parent
            };
            control.SetStyle(ControlStyles.ResizeRedraw, resizeRedraw);
            int layoutCallCount = 0;
            control.Layout += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(control, e.AffectedControl);
                Assert.Equal("Padding", e.AffectedProperty);
                layoutCallCount++;
            };
            int parentLayoutCallCount = 0;
            void parentHandler(object sender, LayoutEventArgs e)
            {
                Assert.Same(parent, sender);
                Assert.Same(control, e.AffectedControl);
                Assert.Same("Padding", e.AffectedProperty);
                parentLayoutCallCount++;
            };
            parent.Layout += parentHandler;
            Assert.NotEqual(IntPtr.Zero, parent.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;
            int parentInvalidatedCallCount = 0;
            parent.Invalidated += (sender, e) => parentInvalidatedCallCount++;
            int parentStyleChangedCallCount = 0;
            parent.StyleChanged += (sender, e) => parentStyleChangedCallCount++;
            int parentCreatedCallCount = 0;
            parent.HandleCreated += (sender, e) => parentCreatedCallCount++;

            control.Padding = value;
            Assert.Equal(expected, control.Padding);
            Assert.Equal(expectedLayoutCallCount1, layoutCallCount);
            Assert.Equal(expectedLayoutCallCount1, parentLayoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount1, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.True(parent.IsHandleCreated);
            Assert.Equal(0, parentInvalidatedCallCount);
            Assert.Equal(0, parentStyleChangedCallCount);
            Assert.Equal(0, parentCreatedCallCount);

            // Set same.
            control.Padding = value;
            Assert.Equal(expected, control.Padding);
            Assert.Equal(expectedLayoutCallCount2, layoutCallCount);
            Assert.Equal(expectedLayoutCallCount2, parentLayoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount2, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.True(parent.IsHandleCreated);
            Assert.Equal(0, parentInvalidatedCallCount);
            Assert.Equal(0, parentStyleChangedCallCount);
            Assert.Equal(0, parentCreatedCallCount);

            parent.Layout -= parentHandler;
        }

        [WinFormsFact]
        public void Control_Padding_SetWithHandler_CallsPaddingChanged()
        {
            using var control = new Control();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Equal(control, sender);
                Assert.Equal(EventArgs.Empty, e);
                callCount++;
            };
            control.PaddingChanged += handler;

            // Set different.
            var padding1 = new Padding(1);
            control.Padding = padding1;
            Assert.Equal(padding1, control.Padding);
            Assert.Equal(1, callCount);

            // Set same.
            control.Padding = padding1;
            Assert.Equal(padding1, control.Padding);
            Assert.Equal(1, callCount);

            // Set different.
            var padding2 = new Padding(2);
            control.Padding = padding2;
            Assert.Equal(padding2, control.Padding);
            Assert.Equal(2, callCount);

            // Remove handler.
            control.PaddingChanged -= handler;
            control.Padding = padding1;
            Assert.Equal(padding1, control.Padding);
            Assert.Equal(2, callCount);
        }

        [WinFormsFact]
        public void Control_Padding_ResetValue_Success()
        {
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(Control))[nameof(Control.Padding)];
            using var control = new Control();
            Assert.False(property.CanResetValue(control));

            control.Padding = new Padding(1, 2, 3, 4);
            Assert.Equal(new Padding(1, 2, 3, 4), control.Padding);
            Assert.True(property.CanResetValue(control));

            property.ResetValue(control);
            Assert.Equal(Padding.Empty, control.Padding);
            Assert.False(property.CanResetValue(control));
        }

        [WinFormsFact]
        public void Control_Padding_ShouldSerializeValue_Success()
        {
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(Control))[nameof(Control.Padding)];
            using var control = new Control();
            Assert.False(property.ShouldSerializeValue(control));

            control.Padding = new Padding(1, 2, 3, 4);
            Assert.Equal(new Padding(1, 2, 3, 4), control.Padding);
            Assert.True(property.ShouldSerializeValue(control));

            property.ResetValue(control);
            Assert.Equal(Padding.Empty, control.Padding);
            Assert.False(property.ShouldSerializeValue(control));
        }

        public static IEnumerable<object[]> Parent_Set_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new Control() };
        }

        [WinFormsTheory]
        [MemberData(nameof(Parent_Set_TestData))]
        public void Control_Parent_Set_GetReturnsExpected(Control value)
        {
            using var control = new Control
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

        [WinFormsTheory]
        [MemberData(nameof(Parent_Set_TestData))]
        public void Control_Parent_SetWithNonNullOldParent_GetReturnsExpected(Control value)
        {
            using var oldParent = new Control();
            using var control = new Control
            {
                Parent = oldParent
            };

            control.Parent = value;
            Assert.Same(value, control.Parent);
            Assert.Empty(oldParent.Controls);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.Parent = value;
            Assert.Same(value, control.Parent);
            Assert.Empty(oldParent.Controls);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void Control_Parent_SetNonNull_AddsToControls()
        {
            using var parent = new Control();
            using var control = new Control
            {
                Parent = parent
            };
            Assert.Same(parent, control.Parent);
            Assert.Same(control, Assert.Single(parent.Controls));
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.Parent = parent;
            Assert.Same(parent, control.Parent);
            Assert.Same(control, Assert.Single(parent.Controls));
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(Parent_Set_TestData))]
        public void Control_Parent_SetWithHandle_GetReturnsExpected(Control value)
        {
            using var control = new Control();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.Parent = value;
            Assert.Same(value, control.Parent);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.Parent = value;
            Assert.Same(value, control.Parent);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void Control_Parent_SetWithHandler_CallsParentChanged()
        {
            using var parent = new Control();
            using var control = new Control();
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

        [WinFormsFact]
        public void Control_Parent_SetSame_ThrowsArgumentException()
        {
            using var control = new Control();
            Assert.Throws<ArgumentException>(null, () => control.Parent = control);
            Assert.Null(control.Parent);
        }

        [WinFormsFact]
        public void Control_Parent_SetChild_ThrowsArgumentException()
        {
            using var child = new Control();
            using var control = new Control();
            control.Controls.Add(child);

            Assert.Throws<ArgumentException>(null, () => control.Parent = child);
            Assert.Null(control.Parent);
        }

        [WinFormsFact]
        public void Control_Parent_SetTopLevel_ThrowsArgumentException()
        {
            using var parent = new Control();
            using var control = new SubControl();
            control.SetTopLevel(true);

            Assert.Throws<ArgumentException>(null, () => control.Parent = parent);
            Assert.Null(control.Parent);
        }

        [WinFormsFact]
        public void Control_PreferredSize_GetWithChildrenSimple_ReturnsExpected()
        {
            using var control = new Control();
            using var child = new Control
            {
                Size = new Size(16, 20)
            };
            control.Controls.Add(child);
            Assert.Equal(new Size(0, 0), control.PreferredSize);

            // Call again.
            Assert.Equal(new Size(0, 0), control.PreferredSize);
        }

        [WinFormsFact]
        public void Control_PreferredSize_GetWithChildrenAdvanced_ReturnsExpected()
        {
            using var control = new BorderedControl
            {
                Padding = new Padding(1, 2, 3, 4)
            };
            using var child = new Control
            {
                Size = new Size(16, 20)
            };
            control.Controls.Add(child);
            Assert.Equal(new Size(0, 0), control.PreferredSize);

            // Call again.
            Assert.Equal(new Size(0, 0), control.PreferredSize);
        }

        public static IEnumerable<object[]> Region_Set_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new Region() };
            yield return new object[] { new Region(new Rectangle(1, 2, 3, 4)) };
        }

        [Theory]
        [MemberData(nameof(Region_Set_TestData))]
        public void Control_Region_Set_GetReturnsExpected(Region value)
        {
            var control = new Control
            {
                Region = value
            };
            Assert.Same(value, control.Region);

            // Set same.
            control.Region = value;
            Assert.Same(value, control.Region);
        }

        [Theory]
        [MemberData(nameof(Region_Set_TestData))]
        public void Control_Region_SetWithNonNullOldValue_GetReturnsExpected(Region value)
        {
            var oldValue = new Region();
            var control = new Control
            {
                Region = oldValue
            };
            oldValue.MakeEmpty();

            control.Region = value;
            Assert.Same(value, control.Region);
            Assert.Throws<ArgumentException>(null, () => oldValue.MakeEmpty());

            // Set same.
            control.Region = value;
            Assert.Same(value, control.Region);
            Assert.Throws<ArgumentException>(null, () => oldValue.MakeEmpty());
        }

        [Theory]
        [MemberData(nameof(Region_Set_TestData))]
        public void Control_Region_SetWithHandle_GetReturnsExpected(Region value)
        {
            var control = new Control();
            Assert.NotEqual(IntPtr.Zero, control.Handle);

            control.Region = value;
            Assert.Same(value, control.Region);

            // Set same.
            control.Region = value;
            Assert.Same(value, control.Region);
        }

        [Theory]
        [MemberData(nameof(Region_Set_TestData))]
        public void Control_Region_SetWithNonNullOldValueWithHandle_GetReturnsExpected(Region value)
        {
            var oldValue = new Region();
            var control = new Control
            {
                Region = oldValue
            };
            oldValue.MakeEmpty();
            Assert.NotEqual(IntPtr.Zero, control.Handle);

            control.Region = value;
            Assert.Same(value, control.Region);
            Assert.Throws<ArgumentException>(null, () => oldValue.MakeEmpty());

            // Set same.
            control.Region = value;
            Assert.Same(value, control.Region);
            Assert.Throws<ArgumentException>(null, () => oldValue.MakeEmpty());
        }

        [Fact]
        public void Control_Region_SetWithHandler_CallsRegionChanged()
        {
            var control = new Control();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            control.RegionChanged += handler;

            // Set different.
            var context1 = new Region();
            control.Region = context1;
            Assert.Same(context1, control.Region);
            Assert.Equal(1, callCount);

            // Set same.
            control.Region = context1;
            Assert.Same(context1, control.Region);
            Assert.Equal(1, callCount);

            // Set different.
            var context2 = new Region();
            control.Region = context2;
            Assert.Same(context2, control.Region);
            Assert.Equal(2, callCount);

            // Set null.
            control.Region = null;
            Assert.False(control.RecreatingHandle);
            Assert.Null(control.Region);
            Assert.Equal(3, callCount);

            // Remove handler.
            control.RegionChanged -= handler;
            control.Region = context1;
            Assert.Same(context1, control.Region);
            Assert.Equal(3, callCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void Control_ResizeRedraw_Get_ReturnsExpected(bool value)
        {
            var control = new SubControl();
            control.SetStyle(ControlStyles.ResizeRedraw, value);
            Assert.Equal(value, control.ResizeRedraw);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void Control_ResizeRedraw_Set_GetReturnsExpected(bool value)
        {
            var control = new SubControl
            {
                ResizeRedraw = value
            };
            Assert.Equal(value, control.ResizeRedraw);
            Assert.Equal(value, control.GetStyle(ControlStyles.ResizeRedraw));

            // Set same.
            control.ResizeRedraw = value;
            Assert.Equal(value, control.ResizeRedraw);
            Assert.Equal(value, control.GetStyle(ControlStyles.ResizeRedraw));

            // Set different.
            control.ResizeRedraw = !value;
            Assert.Equal(!value, control.ResizeRedraw);
            Assert.Equal(!value, control.GetStyle(ControlStyles.ResizeRedraw));
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetRightToLeftTheoryData))]
        public void Control_RightToLeft_Set_GetReturnsExpected(RightToLeft value, RightToLeft expected)
        {
            var control = new Control
            {
                RightToLeft = value
            };
            Assert.Equal(expected, control.RightToLeft);

            // Set same.
            control.RightToLeft = value;
            Assert.Equal(expected, control.RightToLeft);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetRightToLeftTheoryData))]
        public void Control_RightToLeft_SetAsParent_GetReturnsExpected(RightToLeft value, RightToLeft expected)
        {
            var parent = new Control
            {
                RightToLeft = value
            };
            var control = new Control
            {
                Parent = parent
            };
            Assert.Equal(expected, control.RightToLeft);

            // Set same.
            control.RightToLeft = value;
            Assert.Equal(expected, control.RightToLeft);
        }

        [Fact]
        public void Control_RightToLeft_SetWithHandler_CallsRightToLeftChanged()
        {
            var control = new Control();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            control.RightToLeftChanged += handler;

            // Set different.
            control.RightToLeft = RightToLeft.Yes;
            Assert.Equal(RightToLeft.Yes, control.RightToLeft);
            Assert.Equal(1, callCount);

            // Set same.
            control.RightToLeft = RightToLeft.Yes;
            Assert.Equal(RightToLeft.Yes, control.RightToLeft);
            Assert.Equal(1, callCount);

            // Set different.
            control.RightToLeft = RightToLeft.Inherit;
            Assert.Equal(RightToLeft.No, control.RightToLeft);
            Assert.Equal(2, callCount);

            // Remove handler.
            control.RightToLeftChanged -= handler;
            control.RightToLeft = RightToLeft.Yes;
            Assert.Equal(RightToLeft.Yes, control.RightToLeft);
            Assert.Equal(2, callCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(RightToLeft))]
        public void Control_RightToLeft_SetInvalid_ThrowsInvalidEnumArgumentException(RightToLeft value)
        {
            var control = new Control();
            Assert.Throws<InvalidEnumArgumentException>("value", () => control.RightToLeft = value);
        }

        public static IEnumerable<object[]> Site_Set_TestData()
        {
            yield return new object[] { null };

            var mockNullSite = new Mock<ISite>(MockBehavior.Strict);
            mockNullSite
                .Setup(s => s.Container)
                .Returns((IContainer)null);
            mockNullSite
                .Setup(s => s.GetService(typeof(AmbientProperties)))
                .Returns(null);
            yield return new object[] { mockNullSite.Object };

            var mockInvalidSite = new Mock<ISite>(MockBehavior.Strict);
            mockInvalidSite
                .Setup(s => s.Container)
                .Returns((IContainer)null);
            mockInvalidSite
                .Setup(s => s.GetService(typeof(AmbientProperties)))
                .Returns(new object());
            yield return new object[] { mockInvalidSite.Object };

            var mockSite = new Mock<ISite>(MockBehavior.Strict);
            mockSite
                .Setup(s => s.Container)
                .Returns((IContainer)null);
            mockSite
                .Setup(s => s.GetService(typeof(AmbientProperties)))
                .Returns(new AmbientProperties());
            yield return new object[] { mockSite.Object };
        }

        [WinFormsTheory]
        [MemberData(nameof(Site_Set_TestData))]
        public void Control_Site_Set_GetReturnsExpected(ISite value)
        {
            using var control = new Control
            {
                Site = value
            };
            Assert.Same(value, control.Site);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.Site = value;
            Assert.Same(value, control.Site);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(Site_Set_TestData))]
        public void Control_Site_SetWithHandle_GetReturnsExpected(ISite value)
        {
            using var control = new Control();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.Site = value;
            Assert.Same(value, control.Site);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.Site = value;
            Assert.Same(value, control.Site);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [MemberData(nameof(Site_Set_TestData))]
        public void Control_Site_SetWithNonNullOldValue_GetReturnsExpected(ISite value)
        {
            var mockSite = new Mock<ISite>(MockBehavior.Strict);
            mockSite
                .Setup(s => s.Container)
                .Returns((IContainer)null);
            mockSite
                .Setup(s => s.GetService(typeof(AmbientProperties)))
                .Returns(new AmbientProperties());
            using var control = new Control
            {
                Site = mockSite.Object
            };

            control.Site = value;
            Assert.Same(value, control.Site);

            // Set same.
            control.Site = value;
            Assert.Same(value, control.Site);
        }

        [WinFormsFact]
        public void Control_Site_SetWithoutAmbientPropertiesSet_UpdatesProperties()
        {
            Font font1 = SystemFonts.CaptionFont;
            Font font2 = SystemFonts.DialogFont;
            Cursor cursor1 = Cursors.AppStarting;
            Cursor cursor2 = Cursors.Arrow;
            var properties = new AmbientProperties
            {
                BackColor = Color.Blue,
                Cursor = cursor1,
                Font = font1,
                ForeColor = Color.Red
            };
            var mockSite1 = new Mock<ISite>(MockBehavior.Strict);
            mockSite1
                .Setup(s => s.Container)
                .Returns((IContainer)null);
            mockSite1
                .Setup(s => s.GetService(typeof(AmbientProperties)))
                .Returns(properties)
                .Verifiable();

            var sameProperties = new AmbientProperties
            {
                BackColor = Color.Blue,
                Cursor = cursor1,
                Font = font1,
                ForeColor = Color.Red
            };
            var mockSite2 = new Mock<ISite>(MockBehavior.Strict);
            mockSite2
                .Setup(s => s.Container)
                .Returns((IContainer)null);
            mockSite2
                .Setup(s => s.GetService(typeof(AmbientProperties)))
                .Returns(sameProperties)
                .Verifiable();

            var differentProperties = new AmbientProperties
            {
                BackColor = Color.Red,
                Cursor = cursor2,
                Font = font2,
                ForeColor = Color.Blue
            };
            var mockSite3 = new Mock<ISite>(MockBehavior.Strict);
            mockSite3
                .Setup(s => s.Container)
                .Returns((IContainer)null);
            mockSite3
                .Setup(s => s.GetService(typeof(AmbientProperties)))
                .Returns(differentProperties)
                .Verifiable();

            var mockSite4 = new Mock<ISite>(MockBehavior.Strict);
            mockSite4
                .Setup(s => s.Container)
                .Returns((IContainer)null);
            mockSite4
                .Setup(s => s.GetService(typeof(AmbientProperties)))
                .Returns(null)
                .Verifiable();

            using var control = new Control();
            int backColorChangedCallCount = 0;
            control.BackColorChanged += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                backColorChangedCallCount++;
            };
            int cursorChangedCallCount = 0;
            control.CursorChanged += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                cursorChangedCallCount++;
            };
            int foreColorChangedCallCount = 0;
            control.ForeColorChanged += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                foreColorChangedCallCount++;
            };
            int fontChangedCallCount = 0;
            control.FontChanged += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                fontChangedCallCount++;
            };

            control.Site = mockSite1.Object;
            Assert.Same(mockSite1.Object, control.Site);
            Assert.Equal(Color.Blue, control.BackColor);
            Assert.Same(cursor1, control.Cursor);
            Assert.Equal(Color.Red, control.ForeColor);
            Assert.Same(font1, control.Font);
            mockSite1.Verify(s => s.GetService(typeof(AmbientProperties)), Times.Once());
            mockSite2.Verify(s => s.GetService(typeof(AmbientProperties)), Times.Never());
            mockSite3.Verify(s => s.GetService(typeof(AmbientProperties)), Times.Never());
            mockSite4.Verify(s => s.GetService(typeof(AmbientProperties)), Times.Never());
            Assert.Equal(1, backColorChangedCallCount);
            Assert.Equal(0, cursorChangedCallCount);
            Assert.Equal(1, foreColorChangedCallCount);
            Assert.Equal(0, fontChangedCallCount);

            // Set same.
            control.Site = mockSite1.Object;
            Assert.Same(mockSite1.Object, control.Site);
            Assert.Equal(Color.Blue, control.BackColor);
            Assert.Same(cursor1, control.Cursor);
            Assert.Equal(Color.Red, control.ForeColor);
            Assert.Same(font1, control.Font);
            mockSite1.Verify(s => s.GetService(typeof(AmbientProperties)), Times.Exactly(2));
            mockSite2.Verify(s => s.GetService(typeof(AmbientProperties)), Times.Never());
            mockSite3.Verify(s => s.GetService(typeof(AmbientProperties)), Times.Never());
            mockSite4.Verify(s => s.GetService(typeof(AmbientProperties)), Times.Never());
            Assert.Equal(1, backColorChangedCallCount);
            Assert.Equal(0, cursorChangedCallCount);
            Assert.Equal(1, foreColorChangedCallCount);
            Assert.Equal(0, fontChangedCallCount);

            // Set equal.
            control.Site = mockSite2.Object;
            Assert.Same(mockSite2.Object, control.Site);
            Assert.Equal(Color.Blue, control.BackColor);
            Assert.Same(cursor1, control.Cursor);
            Assert.Equal(Color.Red, control.ForeColor);
            Assert.Same(font1, control.Font);
            mockSite1.Verify(s => s.GetService(typeof(AmbientProperties)), Times.Exactly(2));
            mockSite2.Verify(s => s.GetService(typeof(AmbientProperties)), Times.Exactly(1));
            mockSite3.Verify(s => s.GetService(typeof(AmbientProperties)), Times.Never());
            mockSite4.Verify(s => s.GetService(typeof(AmbientProperties)), Times.Never());
            Assert.Equal(1, backColorChangedCallCount);
            Assert.Equal(1, cursorChangedCallCount);
            Assert.Equal(1, foreColorChangedCallCount);
            Assert.Equal(0, fontChangedCallCount);

            // Set different.
            control.Site = mockSite3.Object;
            Assert.Same(mockSite3.Object, control.Site);
            Assert.Equal(Color.Red, control.BackColor);
            Assert.Same(cursor2, control.Cursor);
            Assert.Equal(Color.Blue, control.ForeColor);
            Assert.Same(font2, control.Font);
            mockSite1.Verify(s => s.GetService(typeof(AmbientProperties)), Times.Exactly(2));
            mockSite2.Verify(s => s.GetService(typeof(AmbientProperties)), Times.Exactly(1));
            mockSite3.Verify(s => s.GetService(typeof(AmbientProperties)), Times.Exactly(1));
            mockSite4.Verify(s => s.GetService(typeof(AmbientProperties)), Times.Never());
            Assert.Equal(2, backColorChangedCallCount);
            Assert.Equal(1, cursorChangedCallCount);
            Assert.Equal(2, foreColorChangedCallCount);
            Assert.Equal(1, fontChangedCallCount);

            // Set null.
            control.Site = mockSite4.Object;
            Assert.Same(mockSite4.Object, control.Site);
            Assert.Equal(Control.DefaultBackColor, control.BackColor);
            Assert.Same(Cursors.Default, control.Cursor);
            Assert.Equal(Control.DefaultForeColor, control.ForeColor);
            Assert.Equal(Control.DefaultFont, control.Font);
            mockSite1.Verify(s => s.GetService(typeof(AmbientProperties)), Times.Exactly(2));
            mockSite2.Verify(s => s.GetService(typeof(AmbientProperties)), Times.Exactly(1));
            mockSite3.Verify(s => s.GetService(typeof(AmbientProperties)), Times.Exactly(1));
            mockSite4.Verify(s => s.GetService(typeof(AmbientProperties)), Times.Exactly(1));
            Assert.Equal(3, backColorChangedCallCount);
            Assert.Equal(2, cursorChangedCallCount);
            Assert.Equal(3, foreColorChangedCallCount);
            Assert.Equal(2, fontChangedCallCount);
        }

        [WinFormsFact]
        public void Control_Site_SetWithAmbientPropertiesSet_DoesNotUpdate()
        {
            Font font1 = SystemFonts.MenuFont;
            Font font2 = SystemFonts.DialogFont;
            Cursor cursor1 = Cursors.AppStarting;
            Cursor cursor2 = Cursors.Arrow;
            var properties = new AmbientProperties
            {
                BackColor = Color.Blue,
                Cursor = cursor1,
                Font = font1,
                ForeColor = Color.Red
            };
            var mockSite1 = new Mock<ISite>(MockBehavior.Strict);
            mockSite1
                .Setup(s => s.Container)
                .Returns((IContainer)null);
            mockSite1
                .Setup(s => s.GetService(typeof(AmbientProperties)))
                .Returns(properties)
                .Verifiable();

            var sameProperties = new AmbientProperties
            {
                BackColor = Color.Blue,
                Cursor = cursor1,
                Font = font1,
                ForeColor = Color.Red
            };
            var mockSite2 = new Mock<ISite>(MockBehavior.Strict);
            mockSite2
                .Setup(s => s.Container)
                .Returns((IContainer)null);
            mockSite2
                .Setup(s => s.GetService(typeof(AmbientProperties)))
                .Returns(sameProperties)
                .Verifiable();

            var differentProperties = new AmbientProperties
            {
                BackColor = Color.Red,
                Cursor = cursor2,
                Font = font2,
                ForeColor = Color.Blue
            };
            var mockSite3 = new Mock<ISite>(MockBehavior.Strict);
            mockSite3
                .Setup(s => s.Container)
                .Returns((IContainer)null);
            mockSite3
                .Setup(s => s.GetService(typeof(AmbientProperties)))
                .Returns(differentProperties)
                .Verifiable();

            var mockSite4 = new Mock<ISite>(MockBehavior.Strict);
            mockSite4
                .Setup(s => s.Container)
                .Returns((IContainer)null);
            mockSite4
                .Setup(s => s.GetService(typeof(AmbientProperties)))
                .Returns(null)
                .Verifiable();

            using var controlCursor = new Cursor((IntPtr)3);
            Font controlFont = SystemFonts.StatusFont;
            using var control = new Control
            {
                BackColor = Color.Green,
                Cursor = controlCursor,
                ForeColor = Color.Yellow,
                Font = controlFont
            };
            int backColorChangedCallCount = 0;
            control.BackColorChanged += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                backColorChangedCallCount++;
            };
            int cursorChangedCallCount = 0;
            control.CursorChanged += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                cursorChangedCallCount++;
            };
            int foreColorChangedCallCount = 0;
            control.ForeColorChanged += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                foreColorChangedCallCount++;
            };
            int fontChangedCallCount = 0;
            control.FontChanged += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                fontChangedCallCount++;
            };

            control.Site = mockSite1.Object;
            Assert.Same(mockSite1.Object, control.Site);
            Assert.Equal(Color.Green, control.BackColor);
            Assert.Same(controlCursor, control.Cursor);
            Assert.Equal(Color.Yellow, control.ForeColor);
            Assert.Same(controlFont, control.Font);
            mockSite1.Verify(s => s.GetService(typeof(AmbientProperties)), Times.Once());
            mockSite2.Verify(s => s.GetService(typeof(AmbientProperties)), Times.Never());
            mockSite3.Verify(s => s.GetService(typeof(AmbientProperties)), Times.Never());
            mockSite4.Verify(s => s.GetService(typeof(AmbientProperties)), Times.Never());
            Assert.Equal(0, backColorChangedCallCount);
            Assert.Equal(0, cursorChangedCallCount);
            Assert.Equal(0, foreColorChangedCallCount);
            Assert.Equal(0, fontChangedCallCount);

            // Set same.
            control.Site = mockSite1.Object;
            Assert.Same(mockSite1.Object, control.Site);
            Assert.Equal(Color.Green, control.BackColor);
            Assert.Same(controlCursor, control.Cursor);
            Assert.Equal(Color.Yellow, control.ForeColor);
            Assert.Same(controlFont, control.Font);
            mockSite1.Verify(s => s.GetService(typeof(AmbientProperties)), Times.Exactly(2));
            mockSite2.Verify(s => s.GetService(typeof(AmbientProperties)), Times.Never());
            mockSite3.Verify(s => s.GetService(typeof(AmbientProperties)), Times.Never());
            mockSite4.Verify(s => s.GetService(typeof(AmbientProperties)), Times.Never());
            Assert.Equal(0, backColorChangedCallCount);
            Assert.Equal(0, cursorChangedCallCount);
            Assert.Equal(0, foreColorChangedCallCount);
            Assert.Equal(0, fontChangedCallCount);

            // Set equal.
            control.Site = mockSite2.Object;
            Assert.Same(mockSite2.Object, control.Site);
            Assert.Equal(Color.Green, control.BackColor);
            Assert.Same(controlCursor, control.Cursor);
            Assert.Equal(Color.Yellow, control.ForeColor);
            Assert.Same(controlFont, control.Font);
            mockSite1.Verify(s => s.GetService(typeof(AmbientProperties)), Times.Exactly(2));
            mockSite2.Verify(s => s.GetService(typeof(AmbientProperties)), Times.Exactly(1));
            mockSite3.Verify(s => s.GetService(typeof(AmbientProperties)), Times.Never());
            mockSite4.Verify(s => s.GetService(typeof(AmbientProperties)), Times.Never());
            Assert.Equal(0, backColorChangedCallCount);
            Assert.Equal(0, cursorChangedCallCount);
            Assert.Equal(0, foreColorChangedCallCount);
            Assert.Equal(0, fontChangedCallCount);

            // Set different.
            control.Site = mockSite3.Object;
            Assert.Same(mockSite3.Object, control.Site);
            Assert.Equal(Color.Green, control.BackColor);
            Assert.Same(controlCursor, control.Cursor);
            Assert.Equal(Color.Yellow, control.ForeColor);
            Assert.Same(controlFont, control.Font);
            mockSite1.Verify(s => s.GetService(typeof(AmbientProperties)), Times.Exactly(2));
            mockSite2.Verify(s => s.GetService(typeof(AmbientProperties)), Times.Exactly(1));
            mockSite3.Verify(s => s.GetService(typeof(AmbientProperties)), Times.Exactly(1));
            mockSite4.Verify(s => s.GetService(typeof(AmbientProperties)), Times.Never());
            Assert.Equal(0, backColorChangedCallCount);
            Assert.Equal(0, cursorChangedCallCount);
            Assert.Equal(0, foreColorChangedCallCount);
            Assert.Equal(0, fontChangedCallCount);
        }

        public static IEnumerable<object[]> Size_Set_TestData()
        {
            yield return new object[] { new Size(-3, -4), 1 };
            yield return new object[] { new Size(0, 0), 0 };
            yield return new object[] { new Size(1, 0), 1 };
            yield return new object[] { new Size(0, 2), 1 };
            yield return new object[] { new Size(30, 40), 1 };
        }

        [WinFormsTheory]
        [MemberData(nameof(Size_Set_TestData))]
        public void Control_Size_Set_GetReturnsExpected(Size value, int expectedLayoutCallCount)
        {
            using var control = new SubControl();
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

        public static IEnumerable<object[]> Size_Set_WithConstrainedSize_TestData()
        {
            yield return new object[] { Size.Empty, Size.Empty, new Size(30, 40), 30, 40, 1 };
            yield return new object[] { new Size(10, 20), Size.Empty, new Size(30, 40), 30, 40, 1 };
            yield return new object[] { new Size(30, 40), Size.Empty, new Size(30, 40), 30, 40, 0 };
            yield return new object[] { new Size(31, 40), Size.Empty, new Size(30, 40), 31, 40, 0 };
            yield return new object[] { new Size(30, 41), Size.Empty, new Size(30, 40), 30, 41, 0 };
            yield return new object[] { new Size(40, 50), Size.Empty, new Size(30, 40), 40, 50, 0 };
            yield return new object[] { Size.Empty, new Size(20, 10), new Size(30, 40), 20, 10, 1 };
            yield return new object[] { Size.Empty, new Size(30, 40), new Size(30, 40), 30, 40, 1 };
            yield return new object[] { Size.Empty, new Size(31, 40), new Size(30, 40), 30, 40, 1 };
            yield return new object[] { Size.Empty, new Size(30, 41), new Size(30, 40), 30, 40, 1 };
            yield return new object[] { Size.Empty, new Size(40, 50), new Size(30, 40), 30, 40, 1 };
            yield return new object[] { new Size(10, 20), new Size(40, 50), new Size(30, 40), 30, 40, 1 };
            yield return new object[] { new Size(10, 20), new Size(20, 30), new Size(30, 40), 20, 30, 1 };
            yield return new object[] { new Size(10, 20), new Size(20, 30), new Size(30, 40), 20, 30, 1 };
            yield return new object[] { new Size(30, 40), new Size(20, 30), new Size(30, 40), 30, 40, 0 };
            yield return new object[] { new Size(30, 40), new Size(40, 50), new Size(30, 40), 30, 40, 0 };
            yield return new object[] { new Size(40, 50), new Size(20, 30), new Size(30, 40), 40, 50, 0 };
            yield return new object[] { new Size(40, 50), new Size(40, 50), new Size(30, 40), 40, 50, 0 };
        }

        [WinFormsTheory]
        [MemberData(nameof(Size_Set_WithConstrainedSize_TestData))]
        public void Control_Size_SetWithConstrainedSize_GetReturnsExpected(Size minimumSize, Size maximumSize, Size value, int expectedWidth, int expectedHeight, int expectedLayoutCallCount)
        {
            using var control = new SubControl
            {
                MinimumSize = minimumSize,
                MaximumSize = maximumSize
            };
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
            Assert.Equal(new Size(expectedWidth, expectedHeight), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.ClientRectangle);
            Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.DisplayRectangle);
            Assert.Equal(new Size(expectedWidth, expectedHeight), control.Size);
            Assert.Equal(0, control.Left);
            Assert.Equal(expectedWidth, control.Right);
            Assert.Equal(0, control.Top);
            Assert.Equal(expectedHeight, control.Bottom);
            Assert.Equal(expectedWidth, control.Width);
            Assert.Equal(expectedHeight, control.Height);
            Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.Bounds);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.Equal(expectedLayoutCallCount, resizeCallCount);
            Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
            Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
            Assert.False(control.IsHandleCreated);

            // Call again.
            control.Size = value;
            Assert.Equal(new Size(expectedWidth, expectedHeight), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.ClientRectangle);
            Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.DisplayRectangle);
            Assert.Equal(new Size(expectedWidth, expectedHeight), control.Size);
            Assert.Equal(0, control.Left);
            Assert.Equal(expectedWidth, control.Right);
            Assert.Equal(0, control.Top);
            Assert.Equal(expectedHeight, control.Bottom);
            Assert.Equal(expectedWidth, control.Width);
            Assert.Equal(expectedHeight, control.Height);
            Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.Bounds);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.Equal(expectedLayoutCallCount, resizeCallCount);
            Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
            Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
            Assert.False(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> Size_SetWithCustomStyle_TestData()
        {
            yield return new object[] { new Size(0, 0), 0, 0, 0 };
            yield return new object[] { new Size(-3, -4), -7, -8, 1 };
            yield return new object[] { new Size(1, 0), -3, -4, 1 };
            yield return new object[] { new Size(0, 2), -4, -2, 1 };
            yield return new object[] { new Size(1, 2), -3, -2, 1 };
            yield return new object[] { new Size(30, 40), 26, 36, 1 };
        }

        [WinFormsTheory]
        [MemberData(nameof(Size_SetWithCustomStyle_TestData))]
        public void Control_Size_SetWithCustomStyle_GetReturnsExpected(Size value, int expectedClientWidth, int expectedClientHeight, int expectedLayoutCallCount)
        {
            using var control = new BorderedControl();
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
            Assert.Equal(new Size(expectedClientWidth, expectedClientHeight), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, expectedClientWidth, expectedClientHeight), control.ClientRectangle);
            Assert.Equal(new Rectangle(0, 0, expectedClientWidth, expectedClientHeight), control.DisplayRectangle);
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
            Assert.Equal(new Size(expectedClientWidth, expectedClientHeight), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, expectedClientWidth, expectedClientHeight), control.ClientRectangle);
            Assert.Equal(new Rectangle(0, 0, expectedClientWidth, expectedClientHeight), control.DisplayRectangle);
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

        public static IEnumerable<object[]> Size_SetWithParent_TestData()
        {
            yield return new object[] { new Size(0, 0), 0, 0 };
            yield return new object[] { new Size(-3, -4), 1, 2 };
            yield return new object[] { new Size(1, 0), 1, 2 };
            yield return new object[] { new Size(0, 2), 1, 2 };
            yield return new object[] { new Size(1, 2), 1, 2 };
            yield return new object[] { new Size(30, 40), 1, 2 };
        }

        [WinFormsTheory]
        [MemberData(nameof(Size_SetWithParent_TestData))]
        public void Control_Size_SetWithParent_GetReturnsExpected(Size value, int expectedLayoutCallCount, int expectedParentLayoutCallCount)
        {
            using var parent = new Control();
            using var control = new SubControl
            {
                Parent = parent
            };
            int layoutCallCount = 0;
            int resizeCallCount = 0;
            int sizeChangedCallCount = 0;
            int clientSizeChangedCallCount = 0;
            int parentLayoutCallCount = 0;
            control.Layout += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(control, e.AffectedControl);
                Assert.Equal("Bounds", e.AffectedProperty);
                Assert.Equal(resizeCallCount, layoutCallCount);
                Assert.Equal(sizeChangedCallCount, layoutCallCount);
                Assert.Equal(clientSizeChangedCallCount, layoutCallCount);
                Assert.Equal(parentLayoutCallCount, layoutCallCount);
                layoutCallCount++;
            };
            control.Resize += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                Assert.Equal(layoutCallCount - 1, resizeCallCount);
                Assert.Equal(sizeChangedCallCount, resizeCallCount);
                Assert.Equal(clientSizeChangedCallCount, resizeCallCount);
                Assert.Equal(parentLayoutCallCount, resizeCallCount);
                resizeCallCount++;
            };
            control.SizeChanged += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                Assert.Equal(resizeCallCount - 1, sizeChangedCallCount);
                Assert.Equal(layoutCallCount - 1, sizeChangedCallCount);
                Assert.Equal(clientSizeChangedCallCount, sizeChangedCallCount);
                Assert.Equal(parentLayoutCallCount, sizeChangedCallCount);
                sizeChangedCallCount++;
            };
            control.ClientSizeChanged += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                Assert.Equal(resizeCallCount - 1, clientSizeChangedCallCount);
                Assert.Equal(layoutCallCount - 1, clientSizeChangedCallCount);
                Assert.Equal(sizeChangedCallCount - 1, clientSizeChangedCallCount);
                Assert.Equal(parentLayoutCallCount, clientSizeChangedCallCount);
                clientSizeChangedCallCount++;
            };
            void parentHandler(object sender, LayoutEventArgs e)
            {
                Assert.Same(parent, sender);
                Assert.Same(control, e.AffectedControl);
                Assert.Equal("Bounds", e.AffectedProperty);
                parentLayoutCallCount++;
            };
            parent.Layout += parentHandler;

            try
            {
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
                Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
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
                Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
                Assert.False(control.IsHandleCreated);
            }
            finally
            {
                parent.Layout -= parentHandler;
            }
        }

        public static IEnumerable<object[]> Size_SetWithHandle_TestData()
        {
            yield return new object[] { true, new Size(0, 0), 0, 0, 0, 0 };
            yield return new object[] { true, new Size(-3, -4), 0, 0, 0, 0 };
            yield return new object[] { true, new Size(1, 0), 1, 0, 1, 1 };
            yield return new object[] { true, new Size(0, 2), 0, 2, 1, 1 };
            yield return new object[] { true, new Size(1, 2), 1, 2, 1, 1 };
            yield return new object[] { true, new Size(30, 40), 30, 40, 1, 1 };

            yield return new object[] { false, new Size(0, 0), 0, 0, 0, 0 };
            yield return new object[] { false, new Size(-3, -4), 0, 0, 0, 0 };
            yield return new object[] { false, new Size(1, 0), 1, 0, 1, 0 };
            yield return new object[] { false, new Size(0, 2), 0, 2, 1, 0 };
            yield return new object[] { false, new Size(1, 2), 1, 2, 1, 0 };
            yield return new object[] { false, new Size(30, 40), 30, 40, 1, 0 };
        }

        [WinFormsTheory]
        [MemberData(nameof(Size_SetWithHandle_TestData))]
        public void Control_Size_SetWithHandle_GetReturnsExpected(bool resizeRedraw, Size value, int expectedWidth, int expectedHeight, int expectedLayoutCallCount, int expectedInvalidatedCallCount)
        {
            using var control = new SubControl();
            control.SetStyle(ControlStyles.ResizeRedraw, resizeRedraw);
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
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.Size = value;
            Assert.Equal(new Size(expectedWidth, expectedHeight), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.ClientRectangle);
            Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.DisplayRectangle);
            Assert.Equal(new Size(expectedWidth, expectedHeight), control.Size);
            Assert.Equal(0, control.Left);
            Assert.Equal(expectedWidth, control.Right);
            Assert.Equal(0, control.Top);
            Assert.Equal(expectedHeight, control.Bottom);
            Assert.Equal(expectedWidth, control.Width);
            Assert.Equal(expectedHeight, control.Height);
            Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.Bounds);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.Equal(expectedLayoutCallCount, resizeCallCount);
            Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
            Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Call again.
            control.Size = value;
            Assert.Equal(new Size(expectedWidth, expectedHeight), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.ClientRectangle);
            Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.DisplayRectangle);
            Assert.Equal(new Size(expectedWidth, expectedHeight), control.Size);
            Assert.Equal(0, control.Left);
            Assert.Equal(expectedWidth, control.Right);
            Assert.Equal(0, control.Top);
            Assert.Equal(expectedHeight, control.Bottom);
            Assert.Equal(expectedWidth, control.Width);
            Assert.Equal(expectedHeight, control.Height);
            Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.Bounds);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.Equal(expectedLayoutCallCount, resizeCallCount);
            Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
            Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        public static IEnumerable<object[]> Size_SetWithParentHandle_TestData()
        {
            yield return new object[] { true, new Size(0, 0), 0, 0, 0, 0, 0, 0 };
            yield return new object[] { true, new Size(-3, -4), 0, 0, 0, 0, 1, 2 };
            yield return new object[] { true, new Size(1, 0), 1, 0, 1, 1, 2, 2 };
            yield return new object[] { true, new Size(0, 2), 0, 2, 1, 1, 2, 2 };
            yield return new object[] { true, new Size(1, 2), 1, 2, 1, 1, 2, 2 };
            yield return new object[] { true, new Size(30, 40), 30, 40, 1, 1, 2, 2 };

            yield return new object[] { false, new Size(0, 0), 0, 0, 0, 0, 0, 0 };
            yield return new object[] { false, new Size(-3, -4), 0, 0, 0, 0, 1, 2 };
            yield return new object[] { false, new Size(1, 0), 1, 0, 1, 0, 2, 2 };
            yield return new object[] { false, new Size(0, 2), 0, 2, 1, 0, 2, 2 };
            yield return new object[] { false, new Size(1, 2), 1, 2, 1, 0, 2, 2 };
            yield return new object[] { false, new Size(30, 40), 30, 40, 1, 0, 2, 2 };
        }

        [WinFormsTheory]
        [MemberData(nameof(Size_SetWithParentHandle_TestData))]
        public void Control_Size_SetWithParentWithHandle_GetReturnsExpected(bool resizeRedraw, Size value, int expectedWidth, int expectedHeight, int expectedLayoutCallCount, int expectedInvalidatedCallCount, int expectedParentLayoutCallCount1, int expectedParentLayoutCallCount2)
        {
            using var parent = new Control();
            using var control = new SubControl
            {
                Parent = parent
            };
            control.SetStyle(ControlStyles.ResizeRedraw, resizeRedraw);
            int layoutCallCount = 0;
            int resizeCallCount = 0;
            int sizeChangedCallCount = 0;
            int clientSizeChangedCallCount = 0;
            int parentLayoutCallCount = 0;
            control.Layout += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(control, e.AffectedControl);
                Assert.Equal("Bounds", e.AffectedProperty);
                Assert.Equal(resizeCallCount, layoutCallCount);
                Assert.Equal(sizeChangedCallCount, layoutCallCount);
                Assert.Equal(clientSizeChangedCallCount, layoutCallCount);
                Assert.Equal(parentLayoutCallCount, layoutCallCount);
                layoutCallCount++;
            };
            control.Resize += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                Assert.Equal(layoutCallCount - 1, resizeCallCount);
                Assert.Equal(sizeChangedCallCount, resizeCallCount);
                Assert.Equal(clientSizeChangedCallCount, resizeCallCount);
                Assert.Equal(parentLayoutCallCount, resizeCallCount);
                resizeCallCount++;
            };
            control.SizeChanged += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                Assert.Equal(resizeCallCount - 1, sizeChangedCallCount);
                Assert.Equal(layoutCallCount - 1, sizeChangedCallCount);
                Assert.Equal(clientSizeChangedCallCount, sizeChangedCallCount);
                Assert.Equal(parentLayoutCallCount, sizeChangedCallCount);
                sizeChangedCallCount++;
            };
            control.ClientSizeChanged += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                Assert.Equal(resizeCallCount - 1, clientSizeChangedCallCount);
                Assert.Equal(layoutCallCount - 1, clientSizeChangedCallCount);
                Assert.Equal(sizeChangedCallCount - 1, clientSizeChangedCallCount);
                Assert.Equal(parentLayoutCallCount, clientSizeChangedCallCount);
                clientSizeChangedCallCount++;
            };
            void parentHandler(object sender, LayoutEventArgs e)
            {
                Assert.Same(parent, sender);
                Assert.Same(control, e.AffectedControl);
                Assert.Equal("Bounds", e.AffectedProperty);
                parentLayoutCallCount++;
            };
            parent.Layout += parentHandler;
            Assert.NotEqual(IntPtr.Zero, parent.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;
            int parentInvalidatedCallCount = 0;
            parent.Invalidated += (sender, e) => parentInvalidatedCallCount++;
            int parentStyleChangedCallCount = 0;
            parent.StyleChanged += (sender, e) => parentStyleChangedCallCount++;
            int parentCreatedCallCount = 0;
            parent.HandleCreated += (sender, e) => parentCreatedCallCount++;

            try
            {
                control.Size = value;
                Assert.Equal(new Size(expectedWidth, expectedHeight), control.ClientSize);
                Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.ClientRectangle);
                Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.DisplayRectangle);
                Assert.Equal(new Size(expectedWidth, expectedHeight), control.Size);
                Assert.Equal(0, control.Left);
                Assert.Equal(expectedWidth, control.Right);
                Assert.Equal(0, control.Top);
                Assert.Equal(expectedHeight, control.Bottom);
                Assert.Equal(expectedWidth, control.Width);
                Assert.Equal(expectedHeight, control.Height);
                Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.Bounds);
                Assert.Equal(expectedLayoutCallCount, layoutCallCount);
                Assert.Equal(expectedParentLayoutCallCount1, parentLayoutCallCount);
                Assert.Equal(expectedLayoutCallCount, resizeCallCount);
                Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
                Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
                Assert.True(control.IsHandleCreated);
                Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
                Assert.Equal(0, styleChangedCallCount);
                Assert.Equal(0, createdCallCount);
                Assert.True(parent.IsHandleCreated);
                Assert.Equal(0, parentInvalidatedCallCount);
                Assert.Equal(0, parentStyleChangedCallCount);
                Assert.Equal(0, parentCreatedCallCount);

                // Call again.
                control.Size = value;
                Assert.Equal(new Size(expectedWidth, expectedHeight), control.ClientSize);
                Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.ClientRectangle);
                Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.DisplayRectangle);
                Assert.Equal(new Size(expectedWidth, expectedHeight), control.Size);
                Assert.Equal(0, control.Left);
                Assert.Equal(expectedWidth, control.Right);
                Assert.Equal(0, control.Top);
                Assert.Equal(expectedHeight, control.Bottom);
                Assert.Equal(expectedWidth, control.Width);
                Assert.Equal(expectedHeight, control.Height);
                Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.Bounds);
                Assert.Equal(expectedLayoutCallCount, layoutCallCount);
                Assert.Equal(expectedParentLayoutCallCount2, parentLayoutCallCount);
                Assert.Equal(expectedLayoutCallCount, resizeCallCount);
                Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
                Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
                Assert.True(control.IsHandleCreated);
                Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
                Assert.Equal(0, styleChangedCallCount);
                Assert.Equal(0, createdCallCount);
                Assert.True(parent.IsHandleCreated);
                Assert.Equal(0, parentInvalidatedCallCount);
                Assert.Equal(0, parentStyleChangedCallCount);
                Assert.Equal(0, parentCreatedCallCount);
            }
            finally
            {
                parent.Layout -= parentHandler;
            }
        }

        [WinFormsTheory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        public void Control_TabIndex_Set_GetReturnsExpected(int value)
        {
            using var control = new Control
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
        public void Control_TabIndex_SetWithHandler_CallsTabIndexChanged()
        {
            using var control = new Control
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
        public void Control_TabIndex_SetNegative_CallsArgumentOutOfRangeException()
        {
            using var control = new Control();
            Assert.Throws<ArgumentOutOfRangeException>("value", () => control.TabIndex = -1);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void Control_TabStop_Set_GetReturnsExpected(bool value)
        {
            using var control = new Control
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
        public void Control_TabStop_SetWithHandle_GetReturnsExpected(bool value)
        {
            using var control = new Control();
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
        public void Control_TabStop_SetWithHandler_CallsTabStopChanged()
        {
            using var control = new Control
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
        public void Control_Text_Set_GetReturnsExpected(string value, string expected)
        {
            using var control = new Control
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
        public void Control_Text_SetWithHandle_GetReturnsExpected(string value, string expected)
        {
            using var control = new Control();
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
        public void Control_Text_SetWithHandler_CallsTextChanged()
        {
            using var control = new Control();
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
            Assert.Same("text", control.Text);
            Assert.Equal(1, callCount);

            // Set same.
            control.Text = "text";
            Assert.Same("text", control.Text);
            Assert.Equal(1, callCount);

            // Set different.
            control.Text = null;
            Assert.Empty(control.Text);
            Assert.Equal(2, callCount);

            // Remove handler.
            control.TextChanged -= handler;
            control.Text = "text";
            Assert.Same("text", control.Text);
            Assert.Equal(2, callCount);
        }

        [WinFormsTheory]
        [InlineData(0, 0)]
        [InlineData(-1, 1)]
        [InlineData(1, 1)]
        public void Control_Top_Set_GetReturnsExpected(int value, int expectedLocationChangedCallCount)
        {
            using var control = new SubControl();
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

            control.Top = value;
            Assert.Equal(new Size(0, 0), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, 0, 0), control.ClientRectangle);
            Assert.Equal(new Rectangle(0, 0, 0, 0), control.DisplayRectangle);
            Assert.Equal(new Size(0, 0), control.Size);
            Assert.Equal(0, control.Left);
            Assert.Equal(new Point(0, value), control.Location);
            Assert.Equal(value, control.Top);
            Assert.Equal(value, control.Bottom);
            Assert.Equal(0, control.Width);
            Assert.Equal(0, control.Height);
            Assert.Equal(new Rectangle(0, value, 0, 0), control.Bounds);
            Assert.Equal(expectedLocationChangedCallCount, moveCallCount);
            Assert.Equal(expectedLocationChangedCallCount, locationChangedCallCount);
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(0, resizeCallCount);
            Assert.Equal(0, sizeChangedCallCount);
            Assert.Equal(0, clientSizeChangedCallCount);
            Assert.False(control.IsHandleCreated);

            // Call again.
            control.Top = value;
            Assert.Equal(new Size(0, 0), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, 0, 0), control.ClientRectangle);
            Assert.Equal(new Rectangle(0, 0, 0, 0), control.DisplayRectangle);
            Assert.Equal(new Size(0, 0), control.Size);
            Assert.Equal(0, control.Left);
            Assert.Equal(new Point(0, value), control.Location);
            Assert.Equal(value, control.Top);
            Assert.Equal(value, control.Bottom);
            Assert.Equal(0, control.Width);
            Assert.Equal(0, control.Height);
            Assert.Equal(new Rectangle(0, value, 0, 0), control.Bounds);
            Assert.Equal(expectedLocationChangedCallCount, moveCallCount);
            Assert.Equal(expectedLocationChangedCallCount, locationChangedCallCount);
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(0, resizeCallCount);
            Assert.Equal(0, sizeChangedCallCount);
            Assert.Equal(0, clientSizeChangedCallCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(0, 0)]
        [InlineData(-1, 1)]
        [InlineData(1, 1)]
        public void Control_Top_SetWithParent_GetReturnsExpected(int value, int expectedLocationChangedCallCount)
        {
            using var parent = new Control();
            using var control = new SubControl
            {
                Parent = parent
            };
            int moveCallCount = 0;
            int locationChangedCallCount = 0;
            int layoutCallCount = 0;
            int resizeCallCount = 0;
            int sizeChangedCallCount = 0;
            int clientSizeChangedCallCount = 0;
            int parentLayoutCallCount = 0;
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
            void parentHandler(object sender, LayoutEventArgs e)
            {
                Assert.Same(parent, sender);
                Assert.Same(control, e.AffectedControl);
                Assert.Equal("Bounds", e.AffectedProperty);
                parentLayoutCallCount++;
            };
            parent.Layout += parentHandler;

            try
            {
                control.Top = value;
                Assert.Equal(new Size(0, 0), control.ClientSize);
                Assert.Equal(new Rectangle(0, 0, 0, 0), control.ClientRectangle);
                Assert.Equal(new Rectangle(0, 0, 0, 0), control.DisplayRectangle);
                Assert.Equal(new Size(0, 0), control.Size);
                Assert.Equal(0, control.Left);
                Assert.Equal(new Point(0, value), control.Location);
                Assert.Equal(value, control.Top);
                Assert.Equal(value, control.Bottom);
                Assert.Equal(0, control.Width);
                Assert.Equal(0, control.Height);
                Assert.Equal(new Rectangle(0, value, 0, 0), control.Bounds);
                Assert.Equal(expectedLocationChangedCallCount, moveCallCount);
                Assert.Equal(expectedLocationChangedCallCount, locationChangedCallCount);
                Assert.Equal(0, layoutCallCount);
                Assert.Equal(0, resizeCallCount);
                Assert.Equal(0, sizeChangedCallCount);
                Assert.Equal(0, clientSizeChangedCallCount);
                Assert.Equal(expectedLocationChangedCallCount, parentLayoutCallCount);
                Assert.False(control.IsHandleCreated);

                // Call again.
                control.Top = value;
                Assert.Equal(new Size(0, 0), control.ClientSize);
                Assert.Equal(new Rectangle(0, 0, 0, 0), control.ClientRectangle);
                Assert.Equal(new Rectangle(0, 0, 0, 0), control.DisplayRectangle);
                Assert.Equal(new Size(0, 0), control.Size);
                Assert.Equal(0, control.Left);
                Assert.Equal(new Point(0, value), control.Location);
                Assert.Equal(value, control.Top);
                Assert.Equal(value, control.Bottom);
                Assert.Equal(0, control.Width);
                Assert.Equal(0, control.Height);
                Assert.Equal(new Rectangle(0, value, 0, 0), control.Bounds);
                Assert.Equal(expectedLocationChangedCallCount, moveCallCount);
                Assert.Equal(expectedLocationChangedCallCount, locationChangedCallCount);
                Assert.Equal(0, layoutCallCount);
                Assert.Equal(0, resizeCallCount);
                Assert.Equal(0, sizeChangedCallCount);
                Assert.Equal(0, clientSizeChangedCallCount);
                Assert.Equal(expectedLocationChangedCallCount, parentLayoutCallCount);
                Assert.False(control.IsHandleCreated);
            }
            finally
            {
                parent.Layout -= parentHandler;
            }
        }

        public static IEnumerable<object[]> Top_SetWithHandle_TestData()
        {
            foreach (bool resizeRedraw in new bool[] { true, false })
            {
                yield return new object[] { resizeRedraw, 0, 0 };
                yield return new object[] { resizeRedraw, -1, 1 };
                yield return new object[] { resizeRedraw, 1, 1 };
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(Top_SetWithHandle_TestData))]
        public void Control_Top_SetWithHandle_GetReturnsExpected(bool resizeRedraw, int value, int expectedLocationChangedCallCount)
        {
            using var control = new SubControl();
            control.SetStyle(ControlStyles.ResizeRedraw, resizeRedraw);
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

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

            control.Top = value;
            Assert.Equal(new Size(0, 0), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, 0, 0), control.ClientRectangle);
            Assert.Equal(new Rectangle(0, 0, 0, 0), control.DisplayRectangle);
            Assert.Equal(new Size(0, 0), control.Size);
            Assert.Equal(0, control.Left);
            Assert.Equal(new Point(0, value), control.Location);
            Assert.Equal(value, control.Top);
            Assert.Equal(value, control.Bottom);
            Assert.Equal(0, control.Width);
            Assert.Equal(0, control.Height);
            Assert.Equal(new Rectangle(0, value, 0, 0), control.Bounds);
            Assert.Equal(expectedLocationChangedCallCount, moveCallCount);
            Assert.Equal(expectedLocationChangedCallCount, locationChangedCallCount);
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(0, resizeCallCount);
            Assert.Equal(0, sizeChangedCallCount);
            Assert.Equal(0, clientSizeChangedCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Call again.
            control.Top = value;
            Assert.Equal(new Size(0, 0), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, 0, 0), control.ClientRectangle);
            Assert.Equal(new Rectangle(0, 0, 0, 0), control.DisplayRectangle);
            Assert.Equal(new Size(0, 0), control.Size);
            Assert.Equal(0, control.Left);
            Assert.Equal(new Point(0, value), control.Location);
            Assert.Equal(value, control.Top);
            Assert.Equal(value, control.Bottom);
            Assert.Equal(0, control.Width);
            Assert.Equal(0, control.Height);
            Assert.Equal(new Rectangle(0, value, 0, 0), control.Bounds);
            Assert.Equal(expectedLocationChangedCallCount, moveCallCount);
            Assert.Equal(expectedLocationChangedCallCount, locationChangedCallCount);
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(0, resizeCallCount);
            Assert.Equal(0, sizeChangedCallCount);
            Assert.Equal(0, clientSizeChangedCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [MemberData(nameof(Top_SetWithHandle_TestData))]
        public void Control_Top_SetWithParentWithHandle_GetReturnsExpected(bool resizeRedraw, int value, int expectedLocationChangedCallCount)
        {
            using var parent = new Control();
            using var control = new SubControl
            {
                Parent = parent
            };
            control.SetStyle(ControlStyles.ResizeRedraw, resizeRedraw);
            Assert.NotEqual(IntPtr.Zero, parent.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;
            int parentInvalidatedCallCount = 0;
            parent.Invalidated += (sender, e) => parentInvalidatedCallCount++;
            int parentStyleChangedCallCount = 0;
            parent.StyleChanged += (sender, e) => parentStyleChangedCallCount++;
            int parentCreatedCallCount = 0;
            parent.HandleCreated += (sender, e) => parentCreatedCallCount++;

            int moveCallCount = 0;
            int locationChangedCallCount = 0;
            int layoutCallCount = 0;
            int resizeCallCount = 0;
            int sizeChangedCallCount = 0;
            int clientSizeChangedCallCount = 0;
            int parentLayoutCallCount = 0;
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
            void parentHandler(object sender, LayoutEventArgs e)
            {
                Assert.Same(parent, sender);
                Assert.Same(control, e.AffectedControl);
                Assert.Equal("Bounds", e.AffectedProperty);
                parentLayoutCallCount++;
            };
            parent.Layout += parentHandler;

            try
            {
                control.Top = value;
                Assert.Equal(new Size(0, 0), control.ClientSize);
                Assert.Equal(new Rectangle(0, 0, 0, 0), control.ClientRectangle);
                Assert.Equal(new Rectangle(0, 0, 0, 0), control.DisplayRectangle);
                Assert.Equal(new Size(0, 0), control.Size);
                Assert.Equal(0, control.Left);
                Assert.Equal(new Point(0, value), control.Location);
                Assert.Equal(value, control.Top);
                Assert.Equal(value, control.Bottom);
                Assert.Equal(0, control.Width);
                Assert.Equal(0, control.Height);
                Assert.Equal(new Rectangle(0, value, 0, 0), control.Bounds);
                Assert.Equal(expectedLocationChangedCallCount, moveCallCount);
                Assert.Equal(expectedLocationChangedCallCount, locationChangedCallCount);
                Assert.Equal(0, layoutCallCount);
                Assert.Equal(expectedLocationChangedCallCount, parentLayoutCallCount);
                Assert.Equal(0, resizeCallCount);
                Assert.Equal(0, sizeChangedCallCount);
                Assert.Equal(0, clientSizeChangedCallCount);
                Assert.True(control.IsHandleCreated);
                Assert.Equal(0, invalidatedCallCount);
                Assert.Equal(0, styleChangedCallCount);
                Assert.Equal(0, createdCallCount);
                Assert.True(parent.IsHandleCreated);
                Assert.Equal(0, parentInvalidatedCallCount);
                Assert.Equal(0, parentStyleChangedCallCount);
                Assert.Equal(0, parentCreatedCallCount);

                // Call again.
                control.Top = value;
                Assert.Equal(new Size(0, 0), control.ClientSize);
                Assert.Equal(new Rectangle(0, 0, 0, 0), control.ClientRectangle);
                Assert.Equal(new Rectangle(0, 0, 0, 0), control.DisplayRectangle);
                Assert.Equal(new Size(0, 0), control.Size);
                Assert.Equal(0, control.Left);
                Assert.Equal(new Point(0, value), control.Location);
                Assert.Equal(value, control.Top);
                Assert.Equal(value, control.Bottom);
                Assert.Equal(0, control.Width);
                Assert.Equal(0, control.Height);
                Assert.Equal(new Rectangle(0, value, 0, 0), control.Bounds);
                Assert.Equal(expectedLocationChangedCallCount, moveCallCount);
                Assert.Equal(expectedLocationChangedCallCount, locationChangedCallCount);
                Assert.Equal(0, layoutCallCount);
                Assert.Equal(expectedLocationChangedCallCount, parentLayoutCallCount);
                Assert.Equal(0, resizeCallCount);
                Assert.Equal(0, sizeChangedCallCount);
                Assert.Equal(0, clientSizeChangedCallCount);
                Assert.True(control.IsHandleCreated);
                Assert.Equal(0, invalidatedCallCount);
                Assert.Equal(0, styleChangedCallCount);
                Assert.Equal(0, createdCallCount);
                Assert.True(parent.IsHandleCreated);
                Assert.Equal(0, parentInvalidatedCallCount);
                Assert.Equal(0, parentStyleChangedCallCount);
                Assert.Equal(0, parentCreatedCallCount);
            }
            finally
            {
                parent.Layout -= parentHandler;
            }
        }

        [WinFormsTheory]
        [InlineData(true, 1)]
        [InlineData(false, 0)]
        public void Control_Top_SetWithHandleWithTransparentBackColor_DoesNotCallInvalidate(bool supportsTransparentBackgroundColor, int expectedInvalidatedCallCount)
        {
            using var control = new SubControl();
            control.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            control.BackColor = Color.FromArgb(254, 255, 255, 255);
            control.SetStyle(ControlStyles.SupportsTransparentBackColor, supportsTransparentBackgroundColor);

            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;

            // Set different.
            control.Top = 1;
            Assert.Equal(new Point(0, 1), control.Location);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);

            // Set same.
            control.Top = 1;
            Assert.Equal(new Point(0, 1), control.Location);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);

            // Set different.
            control.Top = 2;
            Assert.Equal(new Point(0, 2), control.Location);
            Assert.Equal(expectedInvalidatedCallCount * 2, invalidatedCallCount);
        }

        [WinFormsFact]
        public void Control_Top_SetWithHandler_CallsLocationChanged()
        {
            using var control = new Control();
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
            control.Top = 1;
            Assert.Equal(new Point(0, 1), control.Location);
            Assert.Equal(1, locationChangedCallCount);
            Assert.Equal(1, moveCallCount);

            // Set same.
            control.Top = 1;
            Assert.Equal(new Point(0, 1), control.Location);
            Assert.Equal(1, locationChangedCallCount);
            Assert.Equal(1, moveCallCount);

            // Set different.
            control.Top = 2;
            Assert.Equal(new Point(0, 2), control.Location);
            Assert.Equal(2, locationChangedCallCount);
            Assert.Equal(2, moveCallCount);

            // Remove handler.
            control.LocationChanged -= locationChangedHandler;
            control.Move -= moveHandler;
            control.Top = 1;
            Assert.Equal(new Point(0, 1), control.Location);
            Assert.Equal(2, locationChangedCallCount);
            Assert.Equal(2, moveCallCount);
        }

        [WinFormsFact]
        public void Control_TopLevelControl_GetWithParent_ReturnsNull()
        {
            using var grandparent = new Control();
            using var parent = new Control
            {
                Parent = grandparent
            };
            using var control = new Control
            {
                Parent = parent
            };
            Assert.Null(control.TopLevelControl);
            Assert.Null(parent.TopLevelControl);
            Assert.Null(grandparent.TopLevelControl);
        }

        [WinFormsFact]
        public void Control_TopLevelControl_GetWithTopLevelParent_ReturnsExpected()
        {
            using var grandparent = new SubControl();
            grandparent.SetTopLevel(true);
            using var parent = new Control
            {
                Parent = grandparent
            };
            using var control = new Control
            {
                Parent = parent
            };
            Assert.Same(grandparent, control.TopLevelControl);
            Assert.Same(grandparent, parent.TopLevelControl);
            Assert.Same(grandparent, grandparent.TopLevelControl);
        }

        /// <summary>
        ///  Data for the UseWaitCursorGetSet test
        /// </summary>
        public static TheoryData<bool> UseWaitCursorGetSetData =>
            CommonTestHelper.GetBoolTheoryData();

        [Theory]
        [MemberData(nameof(UseWaitCursorGetSetData))]
        public void Control_UseWaitCursorGetSet(bool expected)
        {
            var cont = new Control
            {
                UseWaitCursor = expected
            };

            Assert.Equal(expected, cont.UseWaitCursor);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void Control_Visible_Set_GetReturnsExpected(bool value)
        {
            using var control = new Control
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

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void Control_Visible_SetWithParent_GetReturnsExpected(bool value)
        {
            using var parent = new Control();
            using var control = new Control
            {
                Parent = parent,
                Visible = value
            };
            Assert.Equal(value, control.Visible);
            Assert.False(control.IsHandleCreated);
            Assert.False(parent.IsHandleCreated);

            // Set same.
            control.Visible = value;
            Assert.Equal(value, control.Visible);
            Assert.False(control.IsHandleCreated);
            Assert.False(parent.IsHandleCreated);

            // Set different.
            control.Visible = !value;
            Assert.Equal(!value, control.Visible);
            Assert.Equal(!value, control.IsHandleCreated);
            Assert.False(parent.IsHandleCreated);
        }

        public static IEnumerable<object[]> Visible_SetWithHandle_TestData()
        {
            foreach (bool userPaint in new bool[] { true, false })
            {
                yield return new object[] { userPaint, true };
                yield return new object[] { userPaint, false };
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(Visible_SetWithHandle_TestData))]
        public void Control_Visible_SetWithHandle_GetReturnsExpected(bool userPaint, bool value)
        {
            using var control = new SubControl();
            control.SetStyle(ControlStyles.UserPaint, userPaint);
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.Visible = value;
            Assert.Equal(value, control.Visible);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.Visible = value;
            Assert.Equal(value, control.Visible);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set different.
            control.Visible = !value;
            Assert.Equal(!value, control.Visible);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void Control_Visible_SetWithParentWithHandle_GetReturnsExpected(bool value)
        {
            using var parent = new Control();
            using var control = new Control
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
            int parentInvalidatedCallCount = 0;
            parent.Invalidated += (sender, e) => parentInvalidatedCallCount++;
            int parentStyleChangedCallCount = 0;
            parent.StyleChanged += (sender, e) => parentStyleChangedCallCount++;
            int parentCreatedCallCount = 0;
            parent.HandleCreated += (sender, e) => parentCreatedCallCount++;

            control.Visible = value;
            Assert.Equal(value, control.Visible);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.True(parent.IsHandleCreated);
            Assert.Equal(0, parentInvalidatedCallCount);
            Assert.Equal(0, parentStyleChangedCallCount);
            Assert.Equal(0, parentCreatedCallCount);

            // Set same.
            control.Visible = value;
            Assert.Equal(value, control.Visible);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.True(parent.IsHandleCreated);
            Assert.Equal(0, parentInvalidatedCallCount);
            Assert.Equal(0, parentStyleChangedCallCount);
            Assert.Equal(0, parentCreatedCallCount);

            // Set different.
            control.Visible = !value;
            Assert.Equal(!value, control.Visible);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.True(parent.IsHandleCreated);
            Assert.Equal(0, parentInvalidatedCallCount);
            Assert.Equal(0, parentStyleChangedCallCount);
            Assert.Equal(0, parentCreatedCallCount);
        }

        [WinFormsFact]
        public void Control_Visible_SetWithHandler_CallsVisibleChanged()
        {
            using var control = new Control
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

        [WinFormsFact]
        public void Control_Visible_SetWithChildrenWithHandler_CallsVisibleChanged()
        {
            using var child1 = new Control();
            using var child2 = new Control();
            using var control = new Control
            {
                Visible = true
            };
            control.Controls.Add(child1);
            control.Controls.Add(child2);

            int callCount = 0;
            int childCallCount1 = 0;
            int childCallCount2 = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            EventHandler childHandler1 = (sender, e) =>
            {
                Assert.Same(child1, sender);
                Assert.Same(EventArgs.Empty, e);
                childCallCount1++;
            };
            EventHandler childHandler2 = (sender, e) =>
            {
                Assert.Same(child2, sender);
                Assert.Same(EventArgs.Empty, e);
                childCallCount2++;
            };
            control.VisibleChanged += handler;
            child1.VisibleChanged += childHandler1;
            child2.VisibleChanged += childHandler2;

            // Set different.
            control.Visible = false;
            Assert.False(control.Visible);
            Assert.False(child1.Visible);
            Assert.False(child2.Visible);
            Assert.Equal(1, callCount);
            Assert.Equal(0, childCallCount1);
            Assert.Equal(0, childCallCount2);

            // Set same.
            control.Visible = false;
            Assert.False(control.Visible);
            Assert.False(child1.Visible);
            Assert.False(child2.Visible);
            Assert.Equal(1, callCount);
            Assert.Equal(0, childCallCount1);
            Assert.Equal(0, childCallCount2);

            // Set different.
            control.Visible = true;
            Assert.True(control.Visible);
            Assert.True(child1.Visible);
            Assert.True(child2.Visible);
            Assert.Equal(2, callCount);
            Assert.Equal(1, childCallCount1);
            Assert.Equal(1, childCallCount2);

            // Remove handler.
            control.VisibleChanged -= handler;
            child1.VisibleChanged -= childHandler1;
            child2.VisibleChanged -= childHandler2;
            control.Visible = false;
            Assert.False(control.Visible);
            Assert.False(child1.Visible);
            Assert.False(child2.Visible);
            Assert.Equal(2, callCount);
            Assert.Equal(1, childCallCount1);
            Assert.Equal(1, childCallCount2);
        }

        [WinFormsFact]
        public void Control_Visible_SetWithChildrenDisabledWithHandler_CallsVisibleChanged()
        {
            using var child1 = new Control
            {
                Visible = false
            };
            using var child2 = new Control
            {
                Visible = false
            };
            using var control = new Control
            {
                Visible = true
            };
            control.Controls.Add(child1);
            control.Controls.Add(child2);

            int callCount = 0;
            int childCallCount1 = 0;
            int childCallCount2 = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            EventHandler childHandler1 = (sender, e) =>
            {
                Assert.Same(child1, sender);
                Assert.Same(EventArgs.Empty, e);
                childCallCount1++;
            };
            EventHandler childHandler2 = (sender, e) =>
            {
                Assert.Same(child2, sender);
                Assert.Same(EventArgs.Empty, e);
                childCallCount2++;
            };
            control.VisibleChanged += handler;
            child1.VisibleChanged += childHandler1;
            child2.VisibleChanged += childHandler2;

            // Set different.
            control.Visible = false;
            Assert.False(control.Visible);
            Assert.False(child1.Visible);
            Assert.False(child2.Visible);
            Assert.Equal(1, callCount);
            Assert.Equal(0, childCallCount1);
            Assert.Equal(0, childCallCount2);

            // Set same.
            control.Visible = false;
            Assert.False(control.Visible);
            Assert.False(child1.Visible);
            Assert.False(child2.Visible);
            Assert.Equal(1, callCount);
            Assert.Equal(0, childCallCount1);
            Assert.Equal(0, childCallCount2);

            // Set different.
            control.Visible = true;
            Assert.True(control.Visible);
            Assert.False(child1.Visible);
            Assert.False(child2.Visible);
            Assert.Equal(2, callCount);
            Assert.Equal(0, childCallCount1);
            Assert.Equal(0, childCallCount2);

            // Remove handler.
            control.VisibleChanged -= handler;
            child1.VisibleChanged -= childHandler1;
            child2.VisibleChanged -= childHandler2;
            control.Visible = false;
            Assert.False(control.Visible);
            Assert.False(child1.Visible);
            Assert.False(child2.Visible);
            Assert.Equal(2, callCount);
            Assert.Equal(0, childCallCount1);
            Assert.Equal(0, childCallCount2);
        }

        [WinFormsTheory]
        [InlineData(-4, 1)]
        [InlineData(0, 0)]
        [InlineData(2, 1)]
        [InlineData(40, 1)]
        public void Control_Width_Set_GetReturnsExpected(int value, int expectedLayoutCallCount)
        {
            using var control = new SubControl();
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

            control.Width = value;
            Assert.Equal(new Size(value, 0), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, value, 0), control.ClientRectangle);
            Assert.Equal(new Rectangle(0, 0, value, 0), control.DisplayRectangle);
            Assert.Equal(new Size(value, 0), control.Size);
            Assert.Equal(0, control.Left);
            Assert.Equal(value, control.Right);
            Assert.Equal(0, control.Top);
            Assert.Equal(0, control.Bottom);
            Assert.Equal(value, control.Width);
            Assert.Equal(0, control.Height);
            Assert.Equal(new Rectangle(0, 0, value, 0), control.Bounds);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.Equal(expectedLayoutCallCount, resizeCallCount);
            Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
            Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
            Assert.False(control.IsHandleCreated);

            // Call again.
            control.Width = value;
            Assert.Equal(new Size(value, 0), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, value, 0), control.ClientRectangle);
            Assert.Equal(new Rectangle(0, 0, value, 0), control.DisplayRectangle);
            Assert.Equal(new Size(value, 0), control.Size);
            Assert.Equal(0, control.Left);
            Assert.Equal(value, control.Right);
            Assert.Equal(0, control.Top);
            Assert.Equal(0, control.Bottom);
            Assert.Equal(value, control.Width);
            Assert.Equal(0, control.Height);
            Assert.Equal(new Rectangle(0, 0, value, 0), control.Bounds);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.Equal(expectedLayoutCallCount, resizeCallCount);
            Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
            Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
            Assert.False(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> Width_Set_WithConstrainedSize_TestData()
        {
            yield return new object[] { Size.Empty, Size.Empty, 30, 30, 0, 1 };
            yield return new object[] { new Size(10, 20), Size.Empty, 30, 30, 20, 1 };
            yield return new object[] { new Size(30, 40), Size.Empty, 30, 30, 40, 0 };
            yield return new object[] { new Size(31, 40), Size.Empty, 30, 31, 40, 0 };
            yield return new object[] { new Size(30, 41), Size.Empty, 30, 30, 41, 0 };
            yield return new object[] { new Size(40, 50), Size.Empty, 30, 40, 50, 0 };
            yield return new object[] { Size.Empty, new Size(20, 10), 30, 20, 0, 1 };
            yield return new object[] { Size.Empty, new Size(30, 40), 30, 30, 0, 1 };
            yield return new object[] { Size.Empty, new Size(31, 40), 30, 30, 0, 1 };
            yield return new object[] { Size.Empty, new Size(30, 41), 30, 30, 0, 1 };
            yield return new object[] { Size.Empty, new Size(40, 50), 30, 30, 0, 1 };
            yield return new object[] { new Size(10, 20), new Size(40, 50), 30, 30, 20, 1 };
            yield return new object[] { new Size(10, 20), new Size(20, 30), 30, 20, 20, 1 };
            yield return new object[] { new Size(30, 40), new Size(20, 30), 30, 30, 40, 0 };
            yield return new object[] { new Size(30, 40), new Size(40, 50), 30, 30, 40, 0 };
            yield return new object[] { new Size(40, 50), new Size(20, 30), 30, 40, 50, 0 };
            yield return new object[] { new Size(40, 50), new Size(40, 50), 30, 40, 50, 0 };
        }

        [WinFormsTheory]
        [MemberData(nameof(Width_Set_WithConstrainedSize_TestData))]
        public void Control_Width_SetWithConstrainedSize_GetReturnsExpected(Size minimumSize, Size maximumSize, int value, int expectedWidth, int expectedHeight, int expectedLayoutCallCount)
        {
            using var control = new SubControl
            {
                MinimumSize = minimumSize,
                MaximumSize = maximumSize
            };
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

            control.Width = value;
            Assert.Equal(new Size(expectedWidth, expectedHeight), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.ClientRectangle);
            Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.DisplayRectangle);
            Assert.Equal(new Size(expectedWidth, expectedHeight), control.Size);
            Assert.Equal(0, control.Left);
            Assert.Equal(expectedWidth, control.Right);
            Assert.Equal(0, control.Top);
            Assert.Equal(expectedHeight, control.Bottom);
            Assert.Equal(expectedWidth, control.Width);
            Assert.Equal(expectedHeight, control.Height);
            Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.Bounds);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.Equal(expectedLayoutCallCount, resizeCallCount);
            Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
            Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
            Assert.False(control.IsHandleCreated);

            // Call again.
            control.Width = value;
            Assert.Equal(new Size(expectedWidth, expectedHeight), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.ClientRectangle);
            Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.DisplayRectangle);
            Assert.Equal(new Size(expectedWidth, expectedHeight), control.Size);
            Assert.Equal(0, control.Left);
            Assert.Equal(expectedWidth, control.Right);
            Assert.Equal(0, control.Top);
            Assert.Equal(expectedHeight, control.Bottom);
            Assert.Equal(expectedWidth, control.Width);
            Assert.Equal(expectedHeight, control.Height);
            Assert.Equal(new Rectangle(0, 0, expectedWidth, expectedHeight), control.Bounds);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.Equal(expectedLayoutCallCount, resizeCallCount);
            Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
            Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(-4, -8, -4, 1)]
        [InlineData(0, 0, 0, 0)]
        [InlineData(2, -2, -4, 1)]
        [InlineData(30, 26, -4, 1)]
        public void Control_Width_SetWithCustomStyle_GetReturnsExpected(int value, int expectedClientWidth, int expectedClientHeight, int expectedLayoutCallCount)
        {
            using var control = new BorderedControl();
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

            control.Width = value;
            Assert.Equal(new Size(expectedClientWidth, expectedClientHeight), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, expectedClientWidth, expectedClientHeight), control.ClientRectangle);
            Assert.Equal(new Rectangle(0, 0, expectedClientWidth, expectedClientHeight), control.DisplayRectangle);
            Assert.Equal(new Size(value, 0), control.Size);
            Assert.Equal(0, control.Left);
            Assert.Equal(value, control.Right);
            Assert.Equal(0, control.Top);
            Assert.Equal(0, control.Bottom);
            Assert.Equal(value, control.Width);
            Assert.Equal(0, control.Height);
            Assert.Equal(new Rectangle(0, 0, value, 0), control.Bounds);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.Equal(expectedLayoutCallCount, resizeCallCount);
            Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
            Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
            Assert.False(control.IsHandleCreated);

            // Call again.
            control.Width = value;
            Assert.Equal(new Size(expectedClientWidth, expectedClientHeight), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, expectedClientWidth, expectedClientHeight), control.ClientRectangle);
            Assert.Equal(new Rectangle(0, 0, expectedClientWidth, expectedClientHeight), control.DisplayRectangle);
            Assert.Equal(new Size(value, 0), control.Size);
            Assert.Equal(0, control.Left);
            Assert.Equal(value, control.Right);
            Assert.Equal(0, control.Top);
            Assert.Equal(0, control.Bottom);
            Assert.Equal(value, control.Width);
            Assert.Equal(0, control.Height);
            Assert.Equal(new Rectangle(0, 0, value, 0), control.Bounds);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.Equal(expectedLayoutCallCount, resizeCallCount);
            Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
            Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(-4, 1, 2)]
        [InlineData(0, 0, 0)]
        [InlineData(2, 1, 2)]
        [InlineData(40, 1, 2)]
        public void Control_Width_SetWithParent_GetReturnsExpected(int value, int expectedLayoutCallCount, int expectedParentLayoutCallCount)
        {
            using var parent = new Control();
            using var control = new SubControl
            {
                Parent = parent
            };
            int layoutCallCount = 0;
            int resizeCallCount = 0;
            int sizeChangedCallCount = 0;
            int clientSizeChangedCallCount = 0;
            int parentLayoutCallCount = 0;
            control.Layout += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(control, e.AffectedControl);
                Assert.Equal("Bounds", e.AffectedProperty);
                Assert.Equal(resizeCallCount, layoutCallCount);
                Assert.Equal(sizeChangedCallCount, layoutCallCount);
                Assert.Equal(clientSizeChangedCallCount, layoutCallCount);
                Assert.Equal(parentLayoutCallCount, layoutCallCount);
                layoutCallCount++;
            };
            control.Resize += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                Assert.Equal(layoutCallCount - 1, resizeCallCount);
                Assert.Equal(sizeChangedCallCount, resizeCallCount);
                Assert.Equal(clientSizeChangedCallCount, resizeCallCount);
                Assert.Equal(parentLayoutCallCount, resizeCallCount);
                resizeCallCount++;
            };
            control.SizeChanged += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                Assert.Equal(resizeCallCount - 1, sizeChangedCallCount);
                Assert.Equal(layoutCallCount - 1, sizeChangedCallCount);
                Assert.Equal(clientSizeChangedCallCount, sizeChangedCallCount);
                Assert.Equal(parentLayoutCallCount, sizeChangedCallCount);
                sizeChangedCallCount++;
            };
            control.ClientSizeChanged += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                Assert.Equal(resizeCallCount - 1, clientSizeChangedCallCount);
                Assert.Equal(layoutCallCount - 1, clientSizeChangedCallCount);
                Assert.Equal(sizeChangedCallCount - 1, clientSizeChangedCallCount);
                Assert.Equal(parentLayoutCallCount, clientSizeChangedCallCount);
                clientSizeChangedCallCount++;
            };
            void parentHandler(object sender, LayoutEventArgs e)
            {
                Assert.Same(parent, sender);
                Assert.Same(control, e.AffectedControl);
                Assert.Equal("Bounds", e.AffectedProperty);
                parentLayoutCallCount++;
            };
            parent.Layout += parentHandler;

            try
            {
                control.Width = value;
                Assert.Equal(new Size(value, 0), control.ClientSize);
                Assert.Equal(new Rectangle(0, 0, value, 0), control.ClientRectangle);
                Assert.Equal(new Rectangle(0, 0, value, 0), control.DisplayRectangle);
                Assert.Equal(new Size(value, 0), control.Size);
                Assert.Equal(0, control.Left);
                Assert.Equal(value, control.Right);
                Assert.Equal(0, control.Top);
                Assert.Equal(0, control.Bottom);
                Assert.Equal(value, control.Width);
                Assert.Equal(0, control.Height);
                Assert.Equal(new Rectangle(0, 0, value, 0), control.Bounds);
                Assert.Equal(expectedLayoutCallCount, layoutCallCount);
                Assert.Equal(expectedLayoutCallCount, resizeCallCount);
                Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
                Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
                Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
                Assert.False(control.IsHandleCreated);

                // Call again.
                control.Width = value;
                Assert.Equal(new Size(value, 0), control.ClientSize);
                Assert.Equal(new Rectangle(0, 0, value, 0), control.ClientRectangle);
                Assert.Equal(new Rectangle(0, 0, value, 0), control.DisplayRectangle);
                Assert.Equal(new Size(value, 0), control.Size);
                Assert.Equal(0, control.Left);
                Assert.Equal(value, control.Right);
                Assert.Equal(0, control.Top);
                Assert.Equal(0, control.Bottom);
                Assert.Equal(value, control.Width);
                Assert.Equal(0, control.Height);
                Assert.Equal(new Rectangle(0, 0, value, 0), control.Bounds);
                Assert.Equal(expectedLayoutCallCount, layoutCallCount);
                Assert.Equal(expectedLayoutCallCount, resizeCallCount);
                Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
                Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
                Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
                Assert.False(control.IsHandleCreated);
            }
            finally
            {
                parent.Layout -= parentHandler;
            }
        }

        public static IEnumerable<object[]> Width_SetWithHandle_TestData()
        {
            yield return new object[] { true, -4, 0, 0, 0 };
            yield return new object[] { true, 0, 0, 0, 0 };
            yield return new object[] { true, 2, 2, 1, 1 };
            yield return new object[] { true, 30, 30, 1, 1 };
            yield return new object[] { false, -4, 0, 0, 0 };
            yield return new object[] { false, 0, 0, 0, 0 };
            yield return new object[] { false, 2, 2, 1, 0 };
            yield return new object[] { false, 30, 30, 1, 0 };
        }

        [WinFormsTheory]
        [MemberData(nameof(Width_SetWithHandle_TestData))]
        public void Control_Width_SetWithHandle_GetReturnsExpected(bool resizeRedraw, int value, int expectedWidth, int expectedLayoutCallCount, int expectedInvalidatedCallCount)
        {
            using var control = new SubControl();
            control.SetStyle(ControlStyles.ResizeRedraw, resizeRedraw);
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
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.Width = value;
            Assert.Equal(new Size(expectedWidth, 0), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, expectedWidth, 0), control.ClientRectangle);
            Assert.Equal(new Rectangle(0, 0, expectedWidth, 0), control.DisplayRectangle);
            Assert.Equal(new Size(expectedWidth, 0), control.Size);
            Assert.Equal(0, control.Left);
            Assert.Equal(expectedWidth, control.Right);
            Assert.Equal(0, control.Top);
            Assert.Equal(0, control.Bottom);
            Assert.Equal(expectedWidth, control.Width);
            Assert.Equal(0, control.Height);
            Assert.Equal(new Rectangle(0, 0, expectedWidth, 0), control.Bounds);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.Equal(expectedLayoutCallCount, resizeCallCount);
            Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
            Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Call again.
            control.Width = value;
            Assert.Equal(new Size(expectedWidth, 0), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, expectedWidth, 0), control.ClientRectangle);
            Assert.Equal(new Rectangle(0, 0, expectedWidth, 0), control.DisplayRectangle);
            Assert.Equal(new Size(expectedWidth, 0), control.Size);
            Assert.Equal(0, control.Left);
            Assert.Equal(expectedWidth, control.Right);
            Assert.Equal(0, control.Top);
            Assert.Equal(0, control.Bottom);
            Assert.Equal(expectedWidth, control.Width);
            Assert.Equal(0, control.Height);
            Assert.Equal(new Rectangle(0, 0, expectedWidth, 0), control.Bounds);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.Equal(expectedLayoutCallCount, resizeCallCount);
            Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
            Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        public static IEnumerable<object[]> Width_SetWithParentWithHandle_TestData()
        {
            yield return new object[] { true, -4, 0, 0, 0, 1, 2 };
            yield return new object[] { true, 0, 0, 0, 0, 0, 0 };
            yield return new object[] { true, 2, 2, 1, 1, 2, 2 };
            yield return new object[] { true, 40, 40, 1, 1, 2, 2 };
            yield return new object[] { false, -4, 0, 0, 0, 1, 2 };
            yield return new object[] { false, 0, 0, 0, 0, 0, 0 };
            yield return new object[] { false, 2, 2, 1, 0, 2, 2 };
            yield return new object[] { false, 40, 40, 1, 0, 2, 2 };
        }

        [WinFormsTheory]
        [MemberData(nameof(Width_SetWithParentWithHandle_TestData))]
        public void Control_Width_SetWithParentWithHandle_GetReturnsExpected(bool resizeRedraw, int value, int expectedWidth, int expectedLayoutCallCount, int expectedInvalidatedCallCount, int expectedParentLayoutCallCount1, int expectedParentLayoutCallCount2)
        {
            using var parent = new Control();
            using var control = new SubControl
            {
                Parent = parent
            };
            control.SetStyle(ControlStyles.ResizeRedraw, resizeRedraw);
            int layoutCallCount = 0;
            int resizeCallCount = 0;
            int sizeChangedCallCount = 0;
            int clientSizeChangedCallCount = 0;
            int parentLayoutCallCount = 0;
            control.Layout += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(control, e.AffectedControl);
                Assert.Equal("Bounds", e.AffectedProperty);
                Assert.Equal(resizeCallCount, layoutCallCount);
                Assert.Equal(sizeChangedCallCount, layoutCallCount);
                Assert.Equal(clientSizeChangedCallCount, layoutCallCount);
                Assert.Equal(parentLayoutCallCount, layoutCallCount);
                layoutCallCount++;
            };
            control.Resize += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                Assert.Equal(layoutCallCount - 1, resizeCallCount);
                Assert.Equal(sizeChangedCallCount, resizeCallCount);
                Assert.Equal(clientSizeChangedCallCount, resizeCallCount);
                Assert.Equal(parentLayoutCallCount, resizeCallCount);
                resizeCallCount++;
            };
            control.SizeChanged += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                Assert.Equal(resizeCallCount - 1, sizeChangedCallCount);
                Assert.Equal(layoutCallCount - 1, sizeChangedCallCount);
                Assert.Equal(clientSizeChangedCallCount, sizeChangedCallCount);
                Assert.Equal(parentLayoutCallCount, sizeChangedCallCount);
                sizeChangedCallCount++;
            };
            control.ClientSizeChanged += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                Assert.Equal(resizeCallCount - 1, clientSizeChangedCallCount);
                Assert.Equal(layoutCallCount - 1, clientSizeChangedCallCount);
                Assert.Equal(sizeChangedCallCount - 1, clientSizeChangedCallCount);
                Assert.Equal(parentLayoutCallCount, clientSizeChangedCallCount);
                clientSizeChangedCallCount++;
            };
            void parentHandler(object sender, LayoutEventArgs e)
            {
                Assert.Same(parent, sender);
                Assert.Same(control, e.AffectedControl);
                Assert.Equal("Bounds", e.AffectedProperty);
                parentLayoutCallCount++;
            };
            parent.Layout += parentHandler;
            Assert.NotEqual(IntPtr.Zero, parent.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;
            int parentInvalidatedCallCount = 0;
            parent.Invalidated += (sender, e) => parentInvalidatedCallCount++;
            int parentStyleChangedCallCount = 0;
            parent.StyleChanged += (sender, e) => parentStyleChangedCallCount++;
            int parentCreatedCallCount = 0;
            parent.HandleCreated += (sender, e) => parentCreatedCallCount++;

            try
            {
                control.Width = value;
                Assert.Equal(new Size(expectedWidth, 0), control.ClientSize);
                Assert.Equal(new Rectangle(0, 0, expectedWidth, 0), control.ClientRectangle);
                Assert.Equal(new Rectangle(0, 0, expectedWidth, 0), control.DisplayRectangle);
                Assert.Equal(new Size(expectedWidth, 0), control.Size);
                Assert.Equal(0, control.Left);
                Assert.Equal(expectedWidth, control.Right);
                Assert.Equal(0, control.Top);
                Assert.Equal(0, control.Bottom);
                Assert.Equal(expectedWidth, control.Width);
                Assert.Equal(0, control.Height);
                Assert.Equal(new Rectangle(0, 0, expectedWidth, 0), control.Bounds);
                Assert.Equal(expectedLayoutCallCount, layoutCallCount);
                Assert.Equal(expectedParentLayoutCallCount1, parentLayoutCallCount);
                Assert.Equal(expectedLayoutCallCount, resizeCallCount);
                Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
                Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
                Assert.True(control.IsHandleCreated);
                Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
                Assert.Equal(0, styleChangedCallCount);
                Assert.Equal(0, createdCallCount);
                Assert.True(parent.IsHandleCreated);
                Assert.Equal(0, parentInvalidatedCallCount);
                Assert.Equal(0, parentStyleChangedCallCount);
                Assert.Equal(0, parentCreatedCallCount);

                // Call again.
                control.Width = value;
                Assert.Equal(new Size(expectedWidth, 0), control.ClientSize);
                Assert.Equal(new Rectangle(0, 0, expectedWidth, 0), control.ClientRectangle);
                Assert.Equal(new Rectangle(0, 0, expectedWidth, 0), control.DisplayRectangle);
                Assert.Equal(new Size(expectedWidth, 0), control.Size);
                Assert.Equal(0, control.Left);
                Assert.Equal(expectedWidth, control.Right);
                Assert.Equal(0, control.Top);
                Assert.Equal(0, control.Bottom);
                Assert.Equal(expectedWidth, control.Width);
                Assert.Equal(0, control.Height);
                Assert.Equal(new Rectangle(0, 0, expectedWidth, 0), control.Bounds);
                Assert.Equal(expectedLayoutCallCount, layoutCallCount);
                Assert.Equal(expectedParentLayoutCallCount2, parentLayoutCallCount);
                Assert.Equal(expectedLayoutCallCount, resizeCallCount);
                Assert.Equal(expectedLayoutCallCount, sizeChangedCallCount);
                Assert.Equal(expectedLayoutCallCount, clientSizeChangedCallCount);
                Assert.True(control.IsHandleCreated);
                Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
                Assert.Equal(0, styleChangedCallCount);
                Assert.Equal(0, createdCallCount);
                Assert.True(parent.IsHandleCreated);
                Assert.Equal(0, parentInvalidatedCallCount);
                Assert.Equal(0, parentStyleChangedCallCount);
                Assert.Equal(0, parentCreatedCallCount);
            }
            finally
            {
                parent.Layout -= parentHandler;
            }
        }

        [Fact]
        public void Control_WindowTargetGetSet()
        {
            var cont = new Control();
            var mock = new Mock<IWindowTarget>(MockBehavior.Strict);

            cont.WindowTarget = mock.Object;

            Assert.Equal(mock.Object, cont.WindowTarget);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringWithNullTheoryData))]
        public void Control_WindowText_Set_GetReturnsExpected(string value)
        {
            var control = new Control
            {
                WindowText = value
            };
            Assert.Equal(value ?? string.Empty, control.WindowText);

            // Set same.
            control.WindowText = value;
            Assert.Equal(value ?? string.Empty, control.WindowText);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringWithNullTheoryData))]
        public void Control_WindowText_SetWithHandle_GetReturnsExpected(string value)
        {
            var control = new Control();
            Assert.NotEqual(IntPtr.Zero, control.Handle);

            control.WindowText = value;
            Assert.Equal(value ?? string.Empty, control.WindowText);

            // Set same.
            control.WindowText = value;
            Assert.Equal(value ?? string.Empty, control.WindowText);
        }
    }
}
