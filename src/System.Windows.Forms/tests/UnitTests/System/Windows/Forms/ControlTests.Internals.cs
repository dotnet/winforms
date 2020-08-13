// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using Moq;
using WinForms.Common.Tests;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public partial class ControlTests
    {
        [WinFormsFact]
        public void Control_GetHandleInternalShouldBeZero()
        {
            using var control = new Control();

            IntPtr intptr = control.HandleInternal;

            Assert.Equal(IntPtr.Zero, intptr);
            Assert.False(control.IsHandleCreated);
        }

        /// <summary>
        ///  Data for the IsTopMdiWindowClosingGetSet test
        /// </summary>
        public static TheoryData<bool> IsTopMdiWindowClosingGetSetData =>
            CommonTestHelper.GetBoolTheoryData();

        [WinFormsTheory]
        [MemberData(nameof(IsTopMdiWindowClosingGetSetData))]
        public void Control_IsTopMdiWindowClosingGetSet(bool expected)
        {
            using var control = new Control
            {
                IsTopMdiWindowClosing = expected
            };

            Assert.Equal(expected, control.IsTopMdiWindowClosing);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(BoundsSpecified))]
        public void Control_RequiredScaling_Set_GetReturnsExpected(BoundsSpecified value)
        {
            using var control = new Control
            {
                RequiredScaling = value
            };
            Assert.Equal(value, control.RequiredScaling);

            // Set same.
            control.RequiredScaling = value;
            Assert.Equal(value, control.RequiredScaling);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void Control_RequiredScalingEnabled_Get_ReturnsExpected(bool value)
        {
            using var control = new Control
            {
                RequiredScalingEnabled = value
            };
            Assert.Equal(value, control.RequiredScalingEnabled);

            // Set same.
            control.RequiredScalingEnabled = value;
            Assert.Equal(value, control.RequiredScalingEnabled);

            // Set different.
            control.RequiredScalingEnabled = !value;
            Assert.Equal(!value, control.RequiredScalingEnabled);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void Control_TabStopInternal_Set_GetReturnsExpected(bool value)
        {
            using var control = new Control
            {
                TabStopInternal = value
            };
            Assert.Equal(value, control.TabStopInternal);

            // Set same.
            control.TabStopInternal = value;
            Assert.Equal(value, control.TabStopInternal);

            // Set different.
            control.TabStopInternal = value;
            Assert.Equal(value, control.TabStopInternal);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void Control_TabStopInternal_SetWithHandle_GetReturnsExpected(bool value)
        {
            using var control = new Control();
            Assert.NotEqual(IntPtr.Zero, control.Handle);

            control.TabStopInternal = value;
            Assert.Equal(value, control.TabStopInternal);

            // Set same.
            control.TabStopInternal = value;
            Assert.Equal(value, control.TabStopInternal);

            // Set different.
            control.TabStopInternal = value;
            Assert.Equal(value, control.TabStopInternal);
        }

        [WinFormsFact]
        public void Control_TabStopInternal_SetWithHandler_DoesNotCallTabStopChanged()
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
            control.TabStopInternal = false;
            Assert.False(control.TabStopInternal);
            Assert.Equal(0, callCount);

            // Set same.
            control.TabStopInternal = false;
            Assert.False(control.TabStopInternal);
            Assert.Equal(0, callCount);

            // Set different.
            control.TabStopInternal = true;
            Assert.True(control.TabStopInternal);
            Assert.Equal(0, callCount);

            // Remove handler.
            control.TabStopChanged -= handler;
            control.TabStopInternal = false;
            Assert.False(control.TabStopInternal);
            Assert.Equal(0, callCount);
        }

        [WinFormsTheory]
        [InlineData(true, true)]
        [InlineData(false, true)] // setting is impossible; default is false
        // SupportsUseCompatibleTextRendering is always false
        public void Control_UseCompatibleTextRenderingIntGetSet(bool given, bool expected)
        {
            using var control = new Control
            {
                UseCompatibleTextRenderingInt = given
            };

            Assert.Equal(expected, control.UseCompatibleTextRenderingInt);
        }

        /// <summary>
        ///  Data for the ValidationCancelledGetSet test
        /// </summary>
        public static TheoryData<bool> ValidationCancelledGetSetData =>
            CommonTestHelper.GetBoolTheoryData();

        [WinFormsTheory]
        [MemberData(nameof(ValidationCancelledGetSetData))]
        public void Control_ValidationCancelledGetSet(bool expected)
        {
            using var control = new Control
            {
                ValidationCancelled = expected
            };

            Assert.Equal(expected, control.ValidationCancelled);
        }

        /// <summary>
        ///  Data for the ApplyBoundsConstraints test
        /// </summary>
        public static TheoryData<int> ApplyBoundsConstraintsData =>
            CommonTestHelper.GetIntTheoryData();

        [WinFormsTheory]
        [MemberData(nameof(ApplySizeConstraintsData))]
        public void Control_ApplyBoundsConstraints(int expected)
        {
            using var control = new Control();
            var expectedBounds = new Rectangle(expected, expected, expected, expected);

            Rectangle actualBounds = control.ApplyBoundsConstraints(expected, expected, expected, expected);

            Assert.Equal(expectedBounds, actualBounds);
        }

        /// <summary>
        ///  Data for the ApplySizeConstraints test
        /// </summary>
        public static TheoryData<int> ApplySizeConstraintsData =>
            CommonTestHelper.GetIntTheoryData();

        [WinFormsTheory]
        [MemberData(nameof(ApplySizeConstraintsData))]
        public void Control_ApplySizeConstraints(int expected)
        {
            using var control = new Control();
            var expectedSize = new Size(expected, expected);

            Size actualSize = control.ApplySizeConstraints(expected, expected);

            Assert.Equal(expectedSize, actualSize);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetSizeTheoryData))]
        public void Control_ApplySizeConstraintsSize_Invoke_ReturnsExpected(Size expectedSize)
        {
            using var control = new Control();
            Size actualSize = control.ApplySizeConstraints(expectedSize);
            Assert.Equal(expectedSize, actualSize);
        }

        [WinFormsFact]
        public void Control_AssignParent()
        {
            using var control = new Control();
            var parent = new Control();

            control.AssignParent(parent);

            Assert.Equal(parent, control.Parent);
        }

        [WinFormsFact]
        public void Control_ParentChangedFromAssign()
        {
            bool wasChanged = false;
            using var control = new Control();
            control.ParentChanged += (sender, args) => wasChanged = true;
            var parent = new Control();

            control.AssignParent(parent);

            Assert.True(wasChanged);
        }

        [WinFormsFact]
        public void Control_CanProcessMnemonic()
        {
            using var control = new Control();

            // act and assert
            Assert.True(control.CanProcessMnemonic());
        }

        [WinFormsFact]
        public void Control_CanProcessMnemonicNotEnabled()
        {
            using var control = new Control
            {
                Enabled = false
            };

            // act and assert
            Assert.False(control.CanProcessMnemonic());
        }

        [WinFormsFact]
        public void Control_CanProcessMnemonicNotVisible()
        {
            using var control = new Control
            {
                Visible = false
            };

            // act and assert
            Assert.False(control.CanProcessMnemonic());
        }

        [WinFormsFact]
        public void Control_CanProcessMnemonicParent()
        {
            using var control = new Control();
            var parent = new Control();
            control.AssignParent(parent);

            // act and assert
            Assert.True(control.CanProcessMnemonic());
        }

        /// <summary>
        ///  Data for the CreateControlInternal test
        /// </summary>
        public static TheoryData<bool> CreateControlInternalData =>
            CommonTestHelper.GetBoolTheoryData();

        [WinFormsTheory]
        [MemberData(nameof(CreateControlInternalData))]
        public void Control_CreateControlInternal(bool fIgnoreVisible)
        {
            using var control = new Control();

            control.CreateControl(fIgnoreVisible);

            Assert.True(control.Created);
        }

        [WinFormsFact]
        public void Control_GetChildControlsInTabOrder()
        {
            using var control = new Control();
            var first = new Control
            {
                TabIndex = 0
            };
            var second = new Control
            {
                TabIndex = 1
            };
            var third = new Control
            {
                TabIndex = 2
            };
            var ordered = new Control[]
            {
                first,
                second,
                third
            };
            var unordered = new Control[]
            {
                second,
                first,
                third
            };
            control.Controls.AddRange(unordered);

            Control[] tabOrderedChildren = control.GetChildControlsInTabOrder(false);

            Assert.Equal(ordered, tabOrderedChildren);
        }

        [WinFormsFact]
        public void Control_GetChildControlsInTabOrderHandlesOnly()
        {
            using var control = new Control();
            var first = new Control
            {
                TabIndex = 0
            };
            var second = new Control
            {
                TabIndex = 1
            };
            var third = new Control
            {
                TabIndex = 2
            };
            var unordered = new Control[]
            {
                second,
                first,
                third
            };
            control.Controls.AddRange(unordered);

            Control[] tabOrderedChildrenWithhandlesOnly = control.GetChildControlsInTabOrder(true);

            Assert.Empty(tabOrderedChildrenWithhandlesOnly);
        }

        [WinFormsFact]
        public void Control_GetFirstChildControlInTabOrder()
        {
            using var control = new Control();
            var first = new Control
            {
                TabIndex = 0
            };
            var second = new Control
            {
                TabIndex = 1
            };
            var third = new Control
            {
                TabIndex = 2
            };
            var tabOrder = new Control[]
            {
                second,
                first,
                third
            };
            control.Controls.AddRange(tabOrder);

            // act and assert
            Assert.Equal(first, control.GetFirstChildControlInTabOrder(true));
        }

        [WinFormsFact]
        public void Control_GetFirstChildControlInTabOrderReverse()
        {
            using var control = new Control();
            var first = new Control
            {
                TabIndex = 0
            };
            var second = new Control
            {
                TabIndex = 1
            };
            var third = new Control
            {
                TabIndex = 2
            };
            var tabOrder = new Control[]
            {
                second,
                first,
                third
            };
            control.Controls.AddRange(tabOrder);

            // act and assert
            Assert.Equal(third, control.GetFirstChildControlInTabOrder(false));
        }

        [WinFormsFact]
        public void Control_NotifyEnter()
        {
            bool wasChanged = false;
            using var control = new Control();
            control.Enter += (sender, args) => wasChanged = true;

            control.NotifyEnter();

            Assert.True(wasChanged);
        }

        [WinFormsFact]
        public void Control_NotifyLeave()
        {
            bool wasChanged = false;
            using var control = new Control();
            control.Leave += (sender, args) => wasChanged = true;

            control.NotifyLeave();

            Assert.True(wasChanged);
        }

        [WinFormsTheory]
        [InlineData(10.0f)]
        [InlineData(0.1f)]
        [InlineData(float.Epsilon)]
        public void Control_ScaleFont(float expected)
        {
            using var control = new Control
            {
                Font = new Font(new FontFamily(Drawing.Text.GenericFontFamilies.Serif), 1.0f)
            };

            control.ScaleFont(expected);

            Assert.Equal(expected, control.Font.Size);
        }
    }
}
