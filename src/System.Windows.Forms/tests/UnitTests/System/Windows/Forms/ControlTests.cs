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
    public class ControlTests
    {
        [Fact]
        public void Control_Ctor_Default()
        {
            var control = new Control();
            Assert.Equal(Control.DefaultBackColor, control.BackColor);
            Assert.Null(control.BindingContext);
            Assert.Equal(0, control.Bottom);
            Assert.False(control.Created);
            Assert.Equal(DpiHelper.DeviceDpi, control.deviceDpi);
            Assert.True(control.Enabled);
            Assert.Equal(Control.DefaultFont, control.Font);
            Assert.Equal(Control.DefaultForeColor, control.ForeColor);
            Assert.Equal(0, control.Left);
            Assert.Equal(Point.Empty, control.Location);
            Assert.Null(control.Parent);
            Assert.Equal(BoundsSpecified.All, control.RequiredScaling);
            Assert.True(control.RequiredScalingEnabled);
            Assert.Equal(0, control.Right);
            Assert.Equal(Size.Empty, control.Size);
            Assert.Equal(0, control.TabIndex);
            Assert.True(control.TabStop);
            Assert.Empty(control.Text);
            Assert.Equal(0, control.Top);
            Assert.True(control.Visible);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void Control_Ctor_String(string text, string expectedText)
        {
            var control = new Control(text);
            Assert.Equal(Control.DefaultBackColor, control.BackColor);
            Assert.Null(control.BindingContext);
            Assert.Equal(0, control.Bottom);
            Assert.False(control.Created);
            Assert.Equal(DpiHelper.DeviceDpi, control.deviceDpi);
            Assert.True(control.Enabled);
            Assert.Equal(Control.DefaultFont, control.Font);
            Assert.Equal(Control.DefaultForeColor, control.ForeColor);
            Assert.Equal(0, control.Left);
            Assert.Equal(Point.Empty, control.Location);
            Assert.Null(control.Parent);
            Assert.Equal(BoundsSpecified.All, control.RequiredScaling);
            Assert.True(control.RequiredScalingEnabled);
            Assert.Equal(0, control.Right);
            Assert.Equal(Size.Empty, control.Size);
            Assert.Equal(0, control.TabIndex);
            Assert.True(control.TabStop);
            Assert.Same(expectedText, control.Text);
            Assert.Equal(0, control.Top);
            Assert.True(control.Visible);
        }

        public static IEnumerable<object[]> Ctor_String_Int_Int_Int_Int()
        {
            yield return new object[] { null, -1, -2, -3, -4, string.Empty };
            yield return new object[] { string.Empty, 0, 0, 0, 0, string.Empty };
            yield return new object[] { "Text", 1, 2, 3, 4, "Text" };
        }

        [Theory]
        [MemberData(nameof(Ctor_String_Int_Int_Int_Int))]
        public void Control_ConstructorSize(string text, int left, int top, int width, int height, string expectedText)
        {
            var control = new Control(text, left, top, width, height);
            Assert.Equal(Control.DefaultBackColor, control.BackColor);
            Assert.Null(control.BindingContext);
            Assert.Equal(top + height, control.Bottom);
            Assert.False(control.Created);
            Assert.Equal(DpiHelper.DeviceDpi, control.deviceDpi);
            Assert.True(control.Enabled);
            Assert.Equal(Control.DefaultFont, control.Font);
            Assert.Equal(Control.DefaultForeColor, control.ForeColor);
            Assert.Equal(left, control.Left);
            Assert.Equal(new Point(left, top), control.Location);
            Assert.Null(control.Parent);
            Assert.Equal(BoundsSpecified.All, control.RequiredScaling);
            Assert.True(control.RequiredScalingEnabled);
            Assert.Equal(left + width, control.Right);
            Assert.Equal(new Size(width, height), control.Size);
            Assert.Equal(0, control.TabIndex);
            Assert.True(control.TabStop);
            Assert.Same(expectedText, control.Text);
            Assert.Equal(top, control.Top);
            Assert.True(control.Visible);
        }

        public static IEnumerable<object[]> Ctor_Control_String_TestData()
        {
            yield return new object[] { null, null, string.Empty };
            yield return new object[] { new Control(), string.Empty, string.Empty };
            yield return new object[] { new Control(), "text", "text" };
        }

        [Theory]
        [MemberData(nameof(Ctor_Control_String_TestData))]
        public void Control_Ctor_Control_String(Control parent, string text, string expectedText)
        {
            var control = new Control(parent, text);
            Assert.Equal(Control.DefaultBackColor, control.BackColor);
            Assert.Null(control.BindingContext);
            Assert.Equal(0, control.Bottom);
            Assert.False(control.Created);
            Assert.Equal(DpiHelper.DeviceDpi, control.deviceDpi);
            Assert.True(control.Enabled);
            Assert.Equal(Control.DefaultFont, control.Font);
            Assert.Equal(Control.DefaultForeColor, control.ForeColor);
            Assert.Equal(0, control.Left);
            Assert.Equal(Point.Empty, control.Location);
            Assert.Same(parent, control.Parent);
            Assert.Equal(BoundsSpecified.All, control.RequiredScaling);
            Assert.True(control.RequiredScalingEnabled);
            Assert.Equal(0, control.Right);
            Assert.Equal(Size.Empty, control.Size);
            Assert.Equal(0, control.TabIndex);
            Assert.True(control.TabStop);
            Assert.Same(expectedText, control.Text);
            Assert.Equal(0, control.Top);
            Assert.True(control.Visible);
        }

        public static IEnumerable<object[]> Ctor_Control_String_Int_Int_Int_Int_TestData()
        {
            yield return new object[] { null, null, -1, -2, -3, -4, string.Empty };
            yield return new object[] { new Control(), string.Empty, 0, 0, 0, 0, string.Empty };
            yield return new object[] { new Control(), "text", 1, 2, 3, 4, "text" };
        }

        [Theory]
        [MemberData(nameof(Ctor_Control_String_Int_Int_Int_Int_TestData))]
        public void Control_Ctor_Control_String_Int_Int_Int_Int(Control parent, string text, int left, int top, int width, int height, string expectedText)
        {
            var control = new Control(parent, text, left, top, width, height);
            Assert.Equal(Control.DefaultBackColor, control.BackColor);
            Assert.Null(control.BindingContext);
            Assert.Equal(top + height, control.Bottom);
            Assert.False(control.Created);
            Assert.Equal(DpiHelper.DeviceDpi, control.deviceDpi);
            Assert.True(control.Enabled);
            Assert.Equal(Control.DefaultFont, control.Font);
            Assert.Equal(Control.DefaultForeColor, control.ForeColor);
            Assert.Equal(left, control.Left);
            Assert.Equal(new Point(left, top), control.Location);
            Assert.Same(parent, control.Parent);
            Assert.Equal(BoundsSpecified.All, control.RequiredScaling);
            Assert.True(control.RequiredScalingEnabled);
            Assert.Equal(left + width, control.Right);
            Assert.Equal(new Size(width, height), control.Size);
            Assert.Equal(0, control.TabIndex);
            Assert.True(control.TabStop);
            Assert.Same(expectedText, control.Text);
            Assert.Equal(top, control.Top);
            Assert.True(control.Visible);
        }

        #region Control Creation

        [Fact]
        public void Control_CreateControl()
        {
            var cont = new Control();

            cont.CreateControl();

            Assert.True(cont.Created);
        }


        /// <summary>
        /// Data for the CreateControlInternal test
        /// </summary>
        public static TheoryData<bool> CreateControlInternalData =>
            CommonTestHelper.GetBoolTheoryData();

        [Theory]
        [MemberData(nameof(CreateControlInternalData))]
        public void Control_CreateControlInternal(bool fIgnoreVisible)
        {
            var cont = new Control();

            cont.CreateControl(fIgnoreVisible);

            Assert.True(cont.Created);
        }

        #endregion

        #region Parenting

        #region Tabbing

        /// <summary>
        /// Data for the TabIndexGetSet test
        /// </summary>
        public static TheoryData<uint> TabIndexGetSetData =>
            CommonTestHelper.GetUIntTheoryData();

        [Theory]
        [MemberData(nameof(TabIndexGetSetData))]
        public void Control_TabIndexGetSet(uint expected)
        {
            var cont = new Control();

            cont.TabIndex = (int)expected;

            Assert.Equal(expected, (uint)cont.TabIndex);
        }

        /// <summary>
        /// Data for the TabStopGetSet test
        /// </summary>
        public static TheoryData<bool> TabStopGetSetData =>
            CommonTestHelper.GetBoolTheoryData();

        [Theory]
        [MemberData(nameof(TabStopGetSetData))]
        public void Control_TabStopGetSet(bool expected)
        {
            var cont = new Control();

            cont.TabStop = expected;

            Assert.Equal(expected, cont.TabStop);
        }

        /// <summary>
        /// Data for the TabStopInternalGetSet test
        /// </summary>
        public static TheoryData<bool> TabStopInternalGetSetData =>
            CommonTestHelper.GetBoolTheoryData();

        [Theory]
        [MemberData(nameof(TabStopInternalGetSetData))]
        public void Control_TabStopInternalGetSet(bool expected)
        {
            var cont = new Control();

            cont.TabStopInternal = expected;

            Assert.Equal(expected, cont.TabStopInternal);
        }

        [Fact]
        public void Control_GetChildControlsInTabOrder()
        {
            var cont = new Control();
            var first = new Control();
            first.TabIndex = 0;
            var second = new Control();
            second.TabIndex = 1;
            var third = new Control();
            third.TabIndex = 2;
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
            cont.Controls.AddRange(unordered);

            var tabOrderedChildren = cont.GetChildControlsInTabOrder(false);

            Assert.Equal(ordered, tabOrderedChildren);
        }

        [Fact]
        public void Control_GetChildControlsInTabOrderHandlesOnly()
        {
            var cont = new Control();
            var first = new Control();
            first.TabIndex = 0;
            var second = new Control();
            second.TabIndex = 1;
            var third = new Control();
            third.TabIndex = 2;
            var unordered = new Control[]
            {
                second,
                first,
                third
            };
            cont.Controls.AddRange(unordered);

            var tabOrderedChildrenWithhandlesOnly = cont.GetChildControlsInTabOrder(true);

            Assert.Empty(tabOrderedChildrenWithhandlesOnly);
        }

        [Fact]
        public void Control_GetFirstChildControlInTabOrder()
        {
            var cont = new Control();
            var first = new Control();
            first.TabIndex = 0;
            var second = new Control();
            second.TabIndex = 1;
            var third = new Control();
            third.TabIndex = 2;
            var tabOrder = new Control[]
            {
                second,
                first,
                third
            };
            cont.Controls.AddRange(tabOrder);

            // act and assert
            Assert.Equal(first, cont.GetFirstChildControlInTabOrder(true));
        }

        [Fact]
        public void Control_GetFirstChildControlInTabOrderReverse()
        {
            var cont = new Control();
            var first = new Control();
            first.TabIndex = 0;
            var second = new Control();
            second.TabIndex = 1;
            var third = new Control();
            third.TabIndex = 2;
            var tabOrder = new Control[]
            {
                second,
                first,
                third
            };
            cont.Controls.AddRange(tabOrder);

            // act and assert
            Assert.Equal(third, cont.GetFirstChildControlInTabOrder(false));
        }

        [Fact]
        public void Control_GetNextControl()
        {
            var cont = new Control();
            var first = new Control();
            first.TabIndex = 0;
            var second = new Control();
            second.TabIndex = 1;
            var third = new Control();
            third.TabIndex = 2;
            var tabOrder = new Control[]
            {
                second,
                first,
                third
            };
            cont.Controls.AddRange(tabOrder);

            // act and assert
            Assert.Equal(second, cont.GetNextControl(first, true));
        }

        [Fact]
        public void Control_GetNextControlReverse()
        {
            var cont = new Control();
            var first = new Control();
            first.TabIndex = 0;
            var second = new Control();
            second.TabIndex = 1;
            var third = new Control();
            third.TabIndex = 2;
            var tabOrder = new Control[]
            {
                second,
                first,
                third
            };
            cont.Controls.AddRange(tabOrder);

            // act and assert
            Assert.Equal(first, cont.GetNextControl(second, false));
        }

        [Fact]
        public void Control_GetNextControlNoNext()
        {
            var cont = new Control();
            var first = new Control();
            first.TabIndex = 0;
            var second = new Control();
            second.TabIndex = 1;
            var third = new Control();
            third.TabIndex = 2;
            var tabOrder = new Control[]
            {
                second,
                first,
                third
            };
            cont.Controls.AddRange(tabOrder);

            // act and assert
            Assert.Null(cont.GetNextControl(third, true));
        }

        [Fact]
        public void Control_GetNextControlNoNextReverse()
        {
            var cont = new Control();
            var first = new Control();
            first.TabIndex = 0;
            var second = new Control();
            second.TabIndex = 1;
            var third = new Control();
            third.TabIndex = 2;
            var tabOrder = new Control[]
            {
                second,
                first,
                third
            };
            cont.Controls.AddRange(tabOrder);

            // act and assert
            Assert.Null(cont.GetNextControl(first, false));
        }

        #endregion

        [Fact]
        public void Control_AssignParent()
        {
            var cont = new Control();
            var parent = new Control();

            cont.AssignParent(parent);

            Assert.Equal(parent, cont.Parent);
        }

        [Fact]
        public void Control_ParentChangedFromAssign()
        {
            bool wasChanged = false;
            var cont = new Control();
            cont.ParentChanged += (sender, args) => wasChanged = true;
            var parent = new Control();

            cont.AssignParent(parent);

            Assert.True(wasChanged);
        }

        [Fact]
        public void Control_ParentChangedFromSet()
        {
            bool wasChanged = false;
            var cont = new Control();
            cont.ParentChanged += (sender, args) => wasChanged = true;
            var parent = new Control();

            cont.Parent = parent;

            Assert.True(wasChanged);
        }

        [Fact]
        public void Control_ParentGetSet()
        {
            var parent = new Control();
            var cont = new Control();

            cont.Parent = parent;

            Assert.NotNull(cont.Parent);
            Assert.Equal(parent, cont.Parent);
        }

        [Fact]
        public void Control_ParentInternalGetSet()
        {
            var parent = new Control();
            var cont = new Control();

            cont.ParentInternal = parent;

            Assert.NotNull(cont.Parent);
            Assert.Equal(parent, cont.ParentInternal);
            Assert.True(parent.Controls.Contains(cont));
        }

        [Fact]
        public void Control_GetContainerControl()
        {
            var cont = new Control();

            var ret = cont.GetContainerControl();

            Assert.Null(ret);
        }

        #region Contains

        [Fact]
        public void Control_Contains()
        {
            var cont = new Control();
            var child = new Control();
            cont.Controls.Add(child);

            // act and assert
            Assert.True(cont.Contains(child));
        }

        [Fact]
        public void Control_ContainsGrandchild()
        {
            var cont = new Control();
            var child = new Control();
            var grandchild = new Control();
            cont.Controls.Add(child);
            child.Controls.Add(grandchild);

            // act and assert
            Assert.True(cont.Contains(grandchild));
        }

        [Fact]
        public void Control_ContainsNot()
        {
            var cont = new Control();
            var child = new Control();

            // act and assert
            Assert.False(cont.Contains(child));
        }

        #endregion

        [Fact]
        public void Control_ParentingExcpetion()
        {
            var bootstrap = new Control();
            var paradox = new Control();
            bootstrap.Parent = paradox;

            // act and assert
            Assert.Throws<ArgumentException>(() => paradox.Parent = bootstrap);
        }

        [Fact]
        public void Control_ChildingExcpetion()
        {
            var bootstrap = new Control();
            var paradox = new Control();
            bootstrap.Controls.Add(paradox);

            // act and assert
            Assert.Throws<ArgumentException>(() => paradox.Controls.Add(bootstrap));
        }

        #endregion

        #region Accesability

        [Fact]
        public void Control_AccessibleNameGetSet()
        {
            var cont = new Control();

            cont.AccessibleName = "Foo";

            Assert.Equal("Foo", cont.AccessibleName);
        }

        /// <summary>
        /// Data for the AccessibleRole test
        /// </summary>
        public static TheoryData<AccessibleRole> AccessibleRoleData =>
            CommonTestHelper.GetEnumTheoryData<AccessibleRole>();

        [Theory]
        [MemberData(nameof(AccessibleRoleData))]
        public void Control_SetItemCheckState(AccessibleRole expected)
        {
            var cont = new Control();

            cont.AccessibleRole = expected;

            Assert.Equal(expected, cont.AccessibleRole);
        }

        /// <summary>
        /// Data for the AccessibleRoleInvalid test
        /// </summary>
        public static TheoryData<CheckState> AccessibleRoleInvalidData =>
            CommonTestHelper.GetEnumTheoryDataInvalid<CheckState>();

        [Theory]
        [MemberData(nameof(AccessibleRoleInvalidData))]
        public void Control_AccessibleRoleInvalid(AccessibleRole expected)
        {
            var cont = new Control();

            var ex = Assert.Throws<InvalidEnumArgumentException>(() => cont.AccessibleRole = expected);
            Assert.Equal("value", ex.ParamName);
        }

        /// <summary>
        /// Data for the IsAccessibleGetSet test
        /// </summary>
        public static TheoryData<bool> IsAccessibleGetSetData =>
            CommonTestHelper.GetBoolTheoryData();

        [Theory]
        [MemberData(nameof(IsAccessibleGetSetData))]
        public void Control_IsAccessibleGetSet(bool expected)
        {
            var cont = new Control();

            cont.IsAccessible = expected;

            Assert.Equal(expected, cont.IsAccessible);
        }

        #endregion

        #region Colors

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetColorTheoryData))]
        public void Control_BackColorGetSet(Color expected)
        {
            var cont = new Control();

            cont.BackColor = expected;

            Assert.Equal(expected, cont.BackColor);
        }

        [Fact]
        public void Control_BackColorChanged()
        {
            bool wasChanged = false;
            var cont = new Control();
            cont.BackColorChanged += (sender, args) => wasChanged = true;

            cont.BackColor = Color.White;

            Assert.True(wasChanged);
        }


        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetColorTheoryData))]
        public void Control_ForeColorGetSet(Color expected)
        {
            var cont = new Control();

            cont.ForeColor = expected;

            Assert.Equal(expected, cont.ForeColor);
        }

        [Fact]
        public void Control_ForeColorChanged()
        {
            bool wasChanged = false;
            var cont = new Control();
            cont.ForeColorChanged += (sender, args) => wasChanged = true;

            cont.ForeColor = Color.Black;

            Assert.True(wasChanged);
        }

        #endregion

        #region ImageLayout

        /// <summary>
        /// Data for the BackgroundImageLayoutGetSet test
        /// </summary>
        public static TheoryData<ImageLayout> BackgroundImageLayoutData =>
            CommonTestHelper.GetEnumTheoryData<ImageLayout>();

        [Theory]
        [MemberData(nameof(BackgroundImageLayoutData))]
        public void Control_BackgroundImageLayoutGetSet(ImageLayout expected)
        {
            var cont = new Control();

            cont.BackgroundImageLayout = expected;

            Assert.Equal(expected, cont.BackgroundImageLayout);
        }

        /// <summary>
        /// Data for the BackgroundImageLayoutGetSetInvalid test
        /// </summary>
        public static TheoryData<ImageLayout> BackgroundImageLayoutGetSetInvalidData =>
            CommonTestHelper.GetEnumTheoryDataInvalid<ImageLayout>();

        [Theory]
        [MemberData(nameof(BackgroundImageLayoutGetSetInvalidData))]
        public void Control_BackgroundImageLayoutGetSetInvalid(ImageLayout expected)
        {
            var cont = new Control();

            var ex = Assert.Throws<InvalidEnumArgumentException>(() => cont.BackgroundImageLayout = expected);
            Assert.Equal("value", ex.ParamName);
        }

        #endregion

        #region Place and Shape

        [Fact]
        public void Control_RegionGetSet()
        {
            var cont = new Control();
            var expected = new Region(new Rectangle(1, 1, 20, 20));

            cont.Region = expected;

            Assert.Equal(expected, cont.Region);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void Control_AutoSize_Set_GetReturnsExpected(bool value)
        {
            var control = new Control
            {
                AutoSize = value
            };
            Assert.Equal(value, control.AutoSize);

            // Set same.
            control.AutoSize = value;
            Assert.Equal(value, control.AutoSize);

            // Set different.
            control.AutoSize = value;
            Assert.Equal(value, control.AutoSize);
        }

        [Fact]
        public void Control_PreferedSizeGet()
        {
            var cont = new Control();

            // act and assert
            Assert.Equal(Size.Empty, cont.PreferredSize);
        }

        #region ApplySizeConstraints

        /// <summary>
        /// Data for the ApplySizeConstraints test
        /// </summary>
        public static TheoryData<int> ApplySizeConstraintsData =>
            CommonTestHelper.GetIntTheoryData();

        [Theory]
        [MemberData(nameof(ApplySizeConstraintsData))]
        public void Control_ApplySizeConstraints(int expected)
        {
            var cont = new Control();
            var expectedSize = new Size(expected, expected);

            var actualSize = cont.ApplySizeConstraints(expected, expected);

            Assert.Equal(expectedSize, actualSize);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetSizeTheoryData))]
        public void Control_ApplySizeConstraintsSize_Invoke_ReturnsExpected(Size expectedSize)
        {
            var control = new Control();
            var actualSize = control.ApplySizeConstraints(expectedSize);
            Assert.Equal(expectedSize, actualSize);
        }

        #endregion

        #region ApplyBoundsConstraints

        /// <summary>
        /// Data for the ApplyBoundsConstraints test
        /// </summary>
        public static TheoryData<int> ApplyBoundsConstraintsData =>
            CommonTestHelper.GetIntTheoryData();

        [Theory]
        [MemberData(nameof(ApplySizeConstraintsData))]
        public void Control_ApplyBoundsConstraints(int expected)
        {
            var cont = new Control();
            var expectedBounds = new Rectangle(expected, expected, expected, expected);

            var actualBounds = cont.ApplyBoundsConstraints(expected, expected, expected, expected);

            Assert.Equal(expectedBounds, actualBounds);
        }

        #endregion

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetPaddingNormalizedTheoryData))]
        public void Control_Padding_Set_GetReturnsExpected(Padding value, Padding expected)
        {
            var control = new Control
            {
                Padding = value
            };
            Assert.Equal(expected, control.Padding);

            // Set same.
            control.Padding = value;
            Assert.Equal(expected, control.Padding);
        }

        /// <summary>
        /// Data for the AnchorGetSet test
        /// </summary>
        public static TheoryData<AnchorStyles> AnchorData =>
            CommonTestHelper.GetEnumTheoryData<AnchorStyles>();

        [Theory]
        [MemberData(nameof(AnchorData))]
        public void Control_AnchorGetSet(AnchorStyles expected)
        {
            var cont = new Control();

            cont.Anchor = expected;

            Assert.Equal(expected, cont.Anchor);
        }

        [Fact]
        public void Control_BoundsGetSet()
        {
            var cont = new Control();
            var expected = new Rectangle(1, 1, 20, 20);

            cont.Bounds = expected;

            Assert.Equal(expected, cont.Bounds);
        }

        /// <summary>
        /// Data for the HeightGetSet test
        /// </summary>
        public static TheoryData<int> HeightGetSetData =>
            CommonTestHelper.GetIntTheoryData();

        [Theory]
        [MemberData(nameof(HeightGetSetData))]
        public void Control_HeightGetSet(int expected)
        {
            var cont = new Control();

            cont.Height = expected;

            Assert.Equal(expected, cont.Height);
        }

        /// <summary>
        /// Data for the LeftGetSet test
        /// </summary>
        public static TheoryData<int> LeftGetSetData =>
            CommonTestHelper.GetIntTheoryData();

        [Theory]
        [MemberData(nameof(LeftGetSetData))]
        public void Control_LeftGetSet(int expected)
        {
            var cont = new Control();

            cont.Left = expected;

            Assert.Equal(expected, cont.Left);
        }

        /// <summary>
        /// Data for the TopGetSet test
        /// </summary>
        public static TheoryData<int> TopGetSetData =>
            CommonTestHelper.GetIntTheoryData();

        [Theory]
        [MemberData(nameof(TopGetSetData))]
        public void Control_TopGetSet(int expected)
        {
            var cont = new Control();

            cont.Top = expected;

            Assert.Equal(expected, cont.Top);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetPointTheoryData))]
        public void Control_LocationGetSet(Point value)
        {
            var control = new Control
            {
                Location = value
            };
            Assert.Equal(value, control.Location);

            // Set same.
            control.Location = value;
            Assert.Equal(value, control.Location);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetPaddingNormalizedTheoryData))]
        public void Control_Margin_Set_GetReturnsExpected(Padding value, Padding expected)
        {
            var control = new Control
            {
                Margin = value
            };
            Assert.Equal(expected, control.Margin);

            // Set same.
            control.Margin = value;
            Assert.Equal(expected, control.Margin);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetSizeTheoryData))]
        public void Control_MaximumSize_Set_GetReturnsExpected(Size value)
        {
            var control = new Control
            {
                MaximumSize = value
            };
            Assert.Equal(value, control.MaximumSize);

            // Set same.
            control.MaximumSize = value;
            Assert.Equal(value, control.MaximumSize);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetSizeTheoryData))]
        public void Control_MinimumSize_Set_GetReturnsExpected(Size value)
        {
            var control = new Control
            {
                MinimumSize = value
            };
            Assert.Equal(value, control.MinimumSize);

            // Set same.
            control.MinimumSize = value;
            Assert.Equal(value, control.MinimumSize);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(BoundsSpecified))]
        public void Control_RequiredScaling_Set_GetReturnsExpected(BoundsSpecified value)
        {
            var control = new Control
            {
                RequiredScaling = value
            };
            Assert.Equal(value, control.RequiredScaling);

            // Set same.
            control.RequiredScaling = value;
            Assert.Equal(value, control.RequiredScaling);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void Control_RequiredScalingEnabled_Get_ReturnsExpected(bool value)
        {
            var control = new Control
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

        [Theory]
        [InlineData(RightToLeft.Yes)]
        [InlineData(RightToLeft.No)]
        public void Control_RightToLeftTest(RightToLeft expected)
        {
            var cont = new Control();

            cont.RightToLeft = expected;

            Assert.Equal(expected, cont.RightToLeft);
        }

        [Fact]
        public void Control_RightToLeftInherit()
        {
            var parent = new Control();
            var cont = new Control();
            cont.Parent = parent;

            parent.RightToLeft = RightToLeft.Yes;
            cont.RightToLeft = RightToLeft.Inherit;

            Assert.Equal(RightToLeft.Yes, cont.RightToLeft);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(RightToLeft))]
        public void Control_RightToLeft_SetInvalid_ThrowsInvalidEnumArgumentException(RightToLeft value)
        {
            var control = new Control();
            Assert.Throws<InvalidEnumArgumentException>("value", () => control.RightToLeft = value);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetSizeTheoryData))]
        public void Control_SizeGetSet(Size value)
        {
            var control = new Control
            {
                Size = value
            };
            Assert.Equal(value, control.Size);

            // Set same.
            control.Size = value;
            Assert.Equal(value, control.Size);
        }

        /// <summary>
        /// Data for the WidthGetSet test
        /// </summary>
        public static TheoryData<int> WidthGetSetData =>
            CommonTestHelper.GetIntTheoryData();

        [Theory]
        [MemberData(nameof(WidthGetSetData))]
        public void Control_WidthGetSet(int expected)
        {
            var cont = new Control();

            cont.Width = expected;

            Assert.Equal(expected, cont.Width);
        }



        #endregion

        #region Events

        [Fact]
        public void Control_Enter()
        {
            bool wasChanged = false;
            var cont = new Control();
            cont.Enter += (sender, args) => wasChanged = true;

            cont.NotifyEnter();

            Assert.True(wasChanged);
        }

        [Fact]
        public void Control_Leave()
        {
            bool wasChanged = false;
            var cont = new Control();
            cont.Leave += (sender, args) => wasChanged = true;

            cont.NotifyLeave();

            Assert.True(wasChanged);
        }

        [Fact]
        public void Control_PaddingChanged()
        {
            bool wasChanged = false;
            var cont = new Control();
            cont.PaddingChanged += (sender, args) => wasChanged = true;

            cont.Padding = new Padding(1);

            Assert.True(wasChanged);
        }

        [Fact]
        public void Control_ControlAddedAndRemoved()
        {
            bool wasAdded = false;
            bool wasRemoved = false;
            var cont = new Control();
            cont.ControlAdded += (sender, args) => wasAdded = true;
            cont.ControlRemoved += (sender, args) => wasRemoved = true;
            var child = new Control();

            cont.Controls.Add(child);
            cont.Controls.Remove(child);

            Assert.True(wasAdded);
            Assert.True(wasRemoved);
        }

        [Fact]
        public void Control_TextChanged()
        {
            bool wasChanged = false;
            var cont = new Control();
            cont.TextChanged += (sender, args) => wasChanged = true;

            cont.Text = "foo";

            Assert.True(wasChanged);
        }

        [Fact]
        public void Control_LocationChanged()
        {
            bool wasChanged = false;
            var cont = new Control();
            cont.LocationChanged += (sender, args) => wasChanged = true;

            cont.Location = new Point(1, 1);

            Assert.True(wasChanged);
        }

        [Fact]
        public void Control_EnabledChanged()
        {
            bool wasEnabled = false;
            var cont = new Control();
            cont.EnabledChanged += (sender, args) => wasEnabled = true;

            cont.Enabled = false;

            Assert.True(wasEnabled);
        }

        [Fact]
        public void Control_FontChanged()
        {
            bool wasChanged = false;
            var cont = new Control();
            cont.FontChanged += (sender, args) => wasChanged = true;

            cont.Font = new Font(new FontFamily(Drawing.Text.GenericFontFamilies.Serif), 1.0f);

            Assert.True(wasChanged);
        }

        [Fact]
        public void Control_MarginChanged()
        {
            bool wasChanged = false;
            var cont = new Control();
            cont.MarginChanged += (sender, args) => wasChanged = true;

            cont.Margin = new Padding(1);

            Assert.True(wasChanged);
        }

        [Fact]
        public void Control_RegionChanged()
        {
            bool wasChanged = false;
            var cont = new Control();
            cont.RegionChanged += (sender, args) => wasChanged = true;

            cont.Region = new Region(new Rectangle(1, 1, 20, 20));

            Assert.True(wasChanged);
        }

        [Fact]
        public void Control_RightToLeftChanged()
        {
            bool wasChanged = false;
            var cont = new Control();
            cont.RightToLeftChanged += (sender, args) => wasChanged = true;

            cont.RightToLeft = RightToLeft.Yes;

            Assert.True(wasChanged);
        }

        [Fact]
        public void Control_VisibleChanged()
        {
            bool wasChanged = false;
            var cont = new Control();
            cont.VisibleChanged += (sender, args) => wasChanged = true;

            cont.Visible = false;

            Assert.True(wasChanged);
        }

        [Fact]
        public void Control_SizeChanged()
        {
            bool wasChanged = false;
            var cont = new Control();
            cont.SizeChanged += (sender, args) => wasChanged = true;

            cont.Size = new Size(50, 50);

            Assert.True(wasChanged);
        }

        [Fact]
        public void Control_TabIndexChanged()
        {
            bool wasChanged = false;
            var cont = new Control();
            cont.TabIndexChanged += (sender, args) => wasChanged = true;

            cont.TabIndex = 1;

            Assert.True(wasChanged);
        }

        [Fact]
        public void Control_TabStopChanged()
        {
            bool wasChanged = false;
            var cont = new Control();
            cont.TabStopChanged += (sender, args) => wasChanged = true;

            cont.TabStop = false;

            Assert.True(wasChanged);
        }

        #endregion

        #region Enabled and Visible

        /// <summary>
        /// Data for the EnabledGetSet test
        /// </summary>
        public static TheoryData<bool> EnabledGetSetData =>
            CommonTestHelper.GetBoolTheoryData();

        [Theory]
        [MemberData(nameof(EnabledGetSetData))]
        public void Control_EnabledGetSet(bool expected)
        {
            var cont = new Control();

            cont.Enabled = expected;

            Assert.Equal(expected, cont.Enabled);
        }

        /// <summary>
        /// Data for the VisibleGetSet test
        /// </summary>
        public static TheoryData<bool> VisibleGetSetData =>
            CommonTestHelper.GetBoolTheoryData();

        [Theory]
        [MemberData(nameof(VisibleGetSetData))]
        public void Control_VisibleGetSet(bool expected)
        {
            var cont = new Control();

            cont.Visible = expected;

            Assert.Equal(expected, cont.Visible);
        }

        [Fact]
        public void Control_GetVisibleCoreNoParent()
        {
            var cont = new Control();

            // act & assert
            Assert.Null(cont.Parent);
            Assert.True(cont.GetVisibleCore());
        }

        [Fact]
        public void Control_GetVisibleCoreVisibleParent()
        {
            var cont = new Control();
            var parent = new Control();
            cont.Parent = parent;

            // act & assert
            Assert.True(cont.GetVisibleCore());
        }

        [Fact]
        public void Control_Hide()
        {
            var cont = new Control();
            cont.Visible = true;

            cont.Hide();

            Assert.False(cont.Visible);
        }

        #endregion

        #region Font

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetFontTheoryData))]
        public void Control_FontGetSet(Font value)
        {
            var control = new Control
            {
                Font = value
            };
            Assert.Equal(value ?? Control.DefaultFont, control.Font);

            // Set same.
            control.Font = value;
            Assert.Equal(value ?? Control.DefaultFont, control.Font);
        }

        [Theory]
        [InlineData(10.0f)]
        [InlineData(0.1f)]
        [InlineData(float.Epsilon)]
        public void Control_ScaleFont(float expected)
        {
            var cont = new Control();
            cont.Font = new Font(new FontFamily(Drawing.Text.GenericFontFamilies.Serif), 1.0f);

            cont.ScaleFont(expected);

            Assert.Equal(expected, cont.Font.Size);
        }

        #endregion

        #region Name and Text

        /// <summary>
        /// Data for the WindowTextGetSet test
        /// </summary>
        public static TheoryData<string> WindowTextGetSetData =>
            CommonTestHelper.GetStringTheoryData();

        [Theory]
        [MemberData(nameof(WindowTextGetSetData))]
        public void Control_WindowTextGetSet(string expected)
        {
            var cont = new Control();

            cont.WindowText = expected;

            Assert.Equal(expected, cont.WindowText);
        }

        /// <summary>
        /// Data for the NameGetSet test
        /// </summary>
        public static TheoryData<string> NameGetSetData =>
            CommonTestHelper.GetStringTheoryData();

        [Theory]
        [MemberData(nameof(NameGetSetData))]
        public void Control_NameGetSet(string expected)
        {
            var cont = new Control();

            cont.Name = expected;

            Assert.Equal(expected, cont.Name);
        }

        /// <summary>
        /// Data for the TextGetSet test
        /// </summary>
        public static TheoryData<string> TextGetSetData =>
            CommonTestHelper.GetStringTheoryData();

        [Theory]
        [MemberData(nameof(TextGetSetData))]
        public void Control_TextGetSet(string expected)
        {
            var cont = new Control();

            cont.Text = expected;

            Assert.Equal(expected, cont.Text);
        }

        #endregion

        #region Capture

        /// <summary>
        /// Data for the CaptureGetSet test
        /// </summary>
        public static TheoryData<bool> CaptureGetSetData =>
            CommonTestHelper.GetBoolTheoryData();

        [Theory]
        [MemberData(nameof(CaptureGetSetData))]
        public void Control_CaptureGetSet(bool expected)
        {
            var cont = new Control();

            cont.Capture = expected;

            Assert.Equal(expected, cont.Capture);
        }

        /// <summary>
        /// Data for the CaptureInternalGetSet test
        /// </summary>
        public static TheoryData<bool> CaptureInternalGetSetData =>
            CommonTestHelper.GetBoolTheoryData();

        [Theory]
        [MemberData(nameof(CaptureInternalGetSetData))]
        public void Control_CaptureInternalGetSet(bool expected)
        {
            var cont = new Control();

            cont.CaptureInternal = expected;

            Assert.Equal(expected, cont.CaptureInternal);
        }

        #endregion

        #region CanProcessMnemonic

        [Fact]
        public void Control_CanProcessMnemonic()
        {
            var cont = new Control();

            // act and assert
            Assert.True(cont.CanProcessMnemonic());
        }

        [Fact]
        public void Control_CanProcessMnemonicNotEnabled()
        {
            var cont = new Control();
            cont.Enabled = false;

            // act and assert
            Assert.False(cont.CanProcessMnemonic());
        }

        [Fact]
        public void Control_CanProcessMnemonicNotVisible()
        {
            var cont = new Control();
            cont.Visible = false;

            // act and assert
            Assert.False(cont.CanProcessMnemonic());
        }

        [Fact]
        public void Control_CanProcessMnemonicParent()
        {
            var cont = new Control();
            var parent = new Control();
            cont.AssignParent(parent);

            // act and assert
            Assert.True(cont.CanProcessMnemonic());
        }

        #endregion

        #region CanSelectCore

        [Fact]
        public void Control_CanSelectCore()
        {
            var cont = new Control();

            // act and assert
            Assert.True(cont.CanSelectCore());
        }

        [Fact]
        public void Control_CanSelectCoreNotEnabled()
        {
            var cont = new Control();
            cont.Enabled = false;

            // act and assert
            Assert.False(cont.CanSelectCore());
        }

        [Fact]
        public void Control_CanSelectCoreParentNotEnabled()
        {
            var cont = new Control();
            cont.Enabled = true;
            var parent = new Control();
            parent.Enabled = false;
            cont.AssignParent(parent);

            // act and assert
            Assert.False(cont.CanSelectCore());
        }

        #endregion

        [Fact]
        public void Control_FindFormWithParent_ReturnsForm()
        {
            var control = new Control();
            var form = new Form();
            control.Parent = form;
            Assert.Equal(form, control.FindForm());
        }

        [Fact]
        public void Control_FindFormWithoutParent_ReturnsNull()
        {
            var control = new Control();
            Assert.Null(control.FindForm());
        }

        #region GetChildAtPoint

        /// <summary>
        /// Data for the GetChildAtPointNull test
        /// </summary>
        public static TheoryData<GetChildAtPointSkip> GetChildAtPointNullData =>
            CommonTestHelper.GetEnumTheoryData<GetChildAtPointSkip>();

        [Theory]
        [MemberData(nameof(GetChildAtPointNullData))]
        public void Control_GetChildAtPointNull(GetChildAtPointSkip skip)
        {
            var cont = new Control();

            var ret = cont.GetChildAtPoint(new Point(5, 5), skip);

            Assert.Null(ret);
        }

        /// <summary>
        /// Data for the GetChildAtPointInvalid test
        /// </summary>
        public static TheoryData<GetChildAtPointSkip> GetChildAtPointInvalidData =>
            CommonTestHelper.GetEnumTheoryDataInvalid<GetChildAtPointSkip>();

        [Theory]
        [MemberData(nameof(GetChildAtPointInvalidData))]
        public void Control_GetChildAtPointInvalid(GetChildAtPointSkip skip)
        {
            var cont = new Control();

            // act & assert
            var ex = Assert.Throws<InvalidEnumArgumentException>(() => cont.GetChildAtPoint(new Point(5, 5), skip));
            Assert.Equal("skipValue", ex.ParamName);
        }

        #endregion

        [Fact]
        public void Control_GetHandle()
        {
            var cont = new Control();

            var intptr = cont.Handle;

            Assert.NotEqual(IntPtr.Zero, intptr);
        }

        [Fact]
        public void Control_GetHandleInternalShouldBeZero()
        {
            var cont = new Control();

            var intptr = cont.HandleInternal;

            Assert.Equal(IntPtr.Zero, intptr);
            Assert.False(cont.IsHandleCreated);
        }

        /// <summary>
        /// Data for the DoDragDrop test
        /// </summary>
        public static TheoryData<DragDropEffects> DoDragDropData =>
            CommonTestHelper.GetEnumTheoryData<DragDropEffects>();

        [Theory]
        [MemberData(nameof(DoDragDropData))]
        public void Control_DoDragDrop(DragDropEffects expected)
        {
            var cont = new Control();
            var mock = new Mock<IDataObject>(MockBehavior.Strict);

            var ret = cont.DoDragDrop(mock.Object, expected);

            Assert.Equal(DragDropEffects.None, ret);
        }

        // TODO: create a focus test that returns true when a handle has been created
        [Fact]
        public void Control_FocusHandleNotCreated()
        {
            var cont = new Control();

            var ret = cont.Focus();

            Assert.False(ret);
        }

        #region Misc. GetSet

        [Fact]
        public void Control_SiteGetSet()
        {
            var cont = new Control();
            var mock = new Mock<ISite>(MockBehavior.Strict);
            mock.Setup(x => x.GetService(typeof(AmbientProperties))).Returns(new AmbientProperties());

            cont.Site = mock.Object;

            Assert.Equal(mock.Object, cont.Site);
        }

        /// <summary>
        /// Data for the UseWaitCursorGetSet test
        /// </summary>
        public static TheoryData<bool> UseWaitCursorGetSetData =>
            CommonTestHelper.GetBoolTheoryData();

        [Theory]
        [MemberData(nameof(UseWaitCursorGetSetData))]
        public void Control_UseWaitCursorGetSet(bool expected)
        {
            var cont = new Control();

            cont.UseWaitCursor = expected;

            Assert.Equal(expected, cont.UseWaitCursor);
        }


        [Theory]
        [InlineData(true, true)]
        [InlineData(false, true)] // setting is impossible; default is false
        // SupportsUseCompatibleTextRendering is always false
        public void Control_UseCompatibleTextRenderingIntGetSet(bool given, bool expected)
        {
            var cont = new Control();

            cont.UseCompatibleTextRenderingInt = given;

            Assert.Equal(expected, cont.UseCompatibleTextRenderingInt);
        }

        [Fact]
        public void Control_WindowTargetGetSet()
        {
            var cont = new Control();
            var mock = new Mock<IWindowTarget>(MockBehavior.Strict);

            cont.WindowTarget = mock.Object;

            Assert.Equal(mock.Object, cont.WindowTarget);
        }

        /// <summary>
        /// Data for the AllowDropGetSet test
        /// </summary>
        public static TheoryData<bool> AllowDropData =>
            CommonTestHelper.GetBoolTheoryData();

        [Theory]
        [MemberData(nameof(AllowDropData))]
        public void Control_AllowDropGetSet(bool expected)
        {
            var cont = new Control();

            cont.AllowDrop = expected;

            Assert.Equal(expected, cont.AllowDrop);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetPointTheoryData))]
        public void Control_AutoScrollOffsetGetSet(Point value)
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

        /// <summary>
        /// Data for the CausesValidationGetSet test
        /// </summary>
        public static TheoryData<bool> CausesValidationData =>
            CommonTestHelper.GetBoolTheoryData();

        [Theory]
        [MemberData(nameof(CausesValidationData))]
        public void Control_CausesValidationGetSet(bool expected)
        {
            var cont = new Control();

            cont.CausesValidation = expected;

            Assert.Equal(expected, cont.CausesValidation);
        }

        [Theory]
        [InlineData(false, false)]
        [InlineData(true, false)] // giving true cannot set to true
        public void Control_CacheTextInternalGetSet(bool given, bool expected)
        {
            var cont = new Control();

            cont.CacheTextInternal = given;

            Assert.Equal(expected, cont.CacheTextInternal);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetSizeTheoryData))]
        public void Control_ClientSize_Set_GetReturnsExpected(Size value)
        {
            var control = new Control
            {
                ClientSize = value
            };
            Assert.Equal(value, control.ClientSize);

            // Set same.
            control.ClientSize = value;
            Assert.Equal(value, control.ClientSize);
        }

        [Fact]
        public void Control_ContextMenuGetSet()
        {
            var cont = new Control();
            var expected = new ContextMenu();

            cont.ContextMenu = expected;

            Assert.Equal(expected, cont.ContextMenu);
        }

        [Fact]
        public void Control_ContextMenuStripGetSet()
        {
            var cont = new Control();
            var expected = new ContextMenuStrip();

            cont.ContextMenuStrip = expected;

            Assert.Equal(expected, cont.ContextMenuStrip);
        }

        /// <summary>
        /// Data for the ValidationCancelledGetSet test
        /// </summary>
        public static TheoryData<bool> ValidationCancelledGetSetData =>
            CommonTestHelper.GetBoolTheoryData();

        [Theory]
        [MemberData(nameof(ValidationCancelledGetSetData))]
        public void Control_ValidationCancelledGetSet(bool expected)
        {
            var cont = new Control();

            cont.ValidationCancelled = expected;

            Assert.Equal(expected, cont.ValidationCancelled);
        }

        /// <summary>
        /// Data for the IsTopMdiWindowClosingGetSet test
        /// </summary>
        public static TheoryData<bool> IsTopMdiWindowClosingGetSetData =>
            CommonTestHelper.GetBoolTheoryData();

        [Theory]
        [MemberData(nameof(IsTopMdiWindowClosingGetSetData))]
        public void Control_IsTopMdiWindowClosingGetSet(bool expected)
        {
            var cont = new Control();

            cont.IsTopMdiWindowClosing = expected;

            Assert.Equal(expected, cont.IsTopMdiWindowClosing);
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

        /// <summary>
        /// Data for the DockGetSet test
        /// </summary>
        public static TheoryData<DockStyle> DockGetSetData =>
            CommonTestHelper.GetEnumTheoryData<DockStyle>();

        [Theory]
        [MemberData(nameof(DockGetSetData))]
        public void Control_DockGetSet(DockStyle expected)
        {
            var cont = new Control();

            cont.Dock = expected;

            Assert.Equal(expected, cont.Dock);
        }

        #endregion

    }
}
