// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using System.ComponentModel;
using System.Drawing;
using Moq;

namespace System.Windows.Forms.Tests
{
    public class ControlTests
    {

        #region Constructor

        [Fact]
        public void ControlTests_Constructor()
        {
            var cont = new Control();

            Assert.NotNull(cont);
            Assert.Equal(DpiHelper.DeviceDpi, cont.deviceDpi);
            Assert.True(cont.RequiredScalingEnabled);
            Assert.Equal(BoundsSpecified.All, cont.RequiredScaling);
            Assert.Equal(0, cont.TabIndex);
            Assert.Equal(Size.Empty, cont.Size);
            Assert.Null(cont.Parent);
            Assert.True(cont.TabStop);
            Assert.Equal(cont.Location, new Point());
            Assert.True(cont.Enabled);
            Assert.Equal(Control.DefaultFont, cont.Font);
            Assert.Equal(Control.DefaultForeColor, cont.ForeColor);
            Assert.Equal(Control.DefaultBackColor, cont.BackColor);
            Assert.Equal("", cont.Text);
            Assert.True(cont.Visible);
            Assert.False(cont.Created);
        }

        [Fact]
        public void ControlTests_ConstructorText()
        {
            var cont = new Control("Foo");

            Assert.Equal("Foo", cont.Text);
            Assert.Null(cont.Parent);
            Assert.True(cont.TabStop);
            Assert.Equal(cont.Location, new Point());
            Assert.True(cont.Enabled);
            Assert.Equal(Control.DefaultFont, cont.Font);
            Assert.Equal(Control.DefaultForeColor, cont.ForeColor);
            Assert.Equal(Control.DefaultBackColor, cont.BackColor);
            Assert.True(cont.Visible);
            Assert.False(cont.Created);
        }

        [Fact]
        public void ControlTests_ConstructorSize()
        {
            var cont = new Control("Foo", 1, 2, 3, 4);

            Assert.Equal("Foo", cont.Text);
            Assert.Equal(1, cont.Left);
            Assert.Equal(2, cont.Top);
            Assert.Equal(3, cont.Width);
            Assert.Equal(4, cont.Height);
            Assert.True(cont.TabStop);
            Assert.True(cont.Enabled);
            Assert.Equal(Control.DefaultFont, cont.Font);
            Assert.Equal(Control.DefaultForeColor, cont.ForeColor);
            Assert.Equal(Control.DefaultBackColor, cont.BackColor);
            Assert.True(cont.Visible);
            Assert.False(cont.Created);
        }

        [Fact]
        public void ControlTests_ConstructorParent()
        {
            var parent = new Control();

            var cont = new Control(parent, "Foo");

            Assert.NotNull(cont.Parent);
            Assert.Equal(parent, cont.Parent);
            Assert.Equal("Foo", cont.Text);
            Assert.Equal(Size.Empty, cont.Size);
            Assert.True(cont.TabStop);
            Assert.Equal(cont.Location, new Point());
            Assert.True(cont.Enabled);
            Assert.Equal(Control.DefaultFont, cont.Font);
            Assert.Equal(Control.DefaultForeColor, cont.ForeColor);
            Assert.Equal(Control.DefaultBackColor, cont.BackColor);
            Assert.True(cont.Visible);
            Assert.False(cont.Created);
        }

        [Fact]
        public void ControlTests_ConstructorAll()
        {
            var parent = new Control();

            var cont = new Control(parent, "Foo", 1, 2, 3, 4);

            Assert.Equal(parent, cont.Parent);
            Assert.Equal("Foo", cont.Text);
            Assert.Equal(1, cont.Left);
            Assert.Equal(2, cont.Top);
            Assert.Equal(3, cont.Width);
            Assert.Equal(4, cont.Height);
            Assert.True(cont.TabStop);
            Assert.True(cont.Enabled);
            Assert.Equal(Control.DefaultFont, cont.Font);
            Assert.Equal(Control.DefaultForeColor, cont.ForeColor);
            Assert.Equal(Control.DefaultBackColor, cont.BackColor);
            Assert.True(cont.Visible);
            Assert.False(cont.Created);
        }

        #endregion

        #region Control Creation

        [Fact]
        public void ControlTests_CreateControl()
        {
            var cont = new Control();

            cont.CreateControl();

            Assert.True(cont.Created);
        }


        /// <summary>
        /// Data for the CreateControlInternal test
        /// </summary>
        public static TheoryData<bool> CreateControlInternalData =>
            TestHelper.GetBoolTheoryData();

        [Theory]
        [MemberData(nameof(CreateControlInternalData))]
        public void ControlTests_CreateControlInternal(bool fIgnoreVisible)
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
            TestHelper.GetUIntTheoryData();

        [Theory]
        [MemberData(nameof(TabIndexGetSetData))]
        public void ControlTests_TabIndexGetSet(uint expected)
        {
            var cont = new Control();

            cont.TabIndex = (int)expected;

            Assert.Equal(expected, (uint)cont.TabIndex);
        }

        /// <summary>
        /// Data for the TabStopGetSet test
        /// </summary>
        public static TheoryData<bool> TabStopGetSetData =>
            TestHelper.GetBoolTheoryData();

        [Theory]
        [MemberData(nameof(TabStopGetSetData))]
        public void ControlTests_TabStopGetSet(bool expected)
        {
            var cont = new Control();

            cont.TabStop = expected;

            Assert.Equal(expected, cont.TabStop);
        }

        /// <summary>
        /// Data for the TabStopInternalGetSet test
        /// </summary>
        public static TheoryData<bool> TabStopInternalGetSetData =>
            TestHelper.GetBoolTheoryData();

        [Theory]
        [MemberData(nameof(TabStopInternalGetSetData))]
        public void ControlTests_TabStopInternalGetSet(bool expected)
        {
            var cont = new Control();

            cont.TabStopInternal = expected;

            Assert.Equal(expected, cont.TabStopInternal);
        }

        [Fact]
        public void ControlTests_GetChildControlsInTabOrder()
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
        public void ControlTests_GetChildControlsInTabOrderHandlesOnly()
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
        public void ControlTests_GetFirstChildControlInTabOrder()
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
        public void ControlTests_GetFirstChildControlInTabOrderReverse()
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
        public void ControlTests_GetNextControl()
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
        public void ControlTests_GetNextControlReverse()
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
        public void ControlTests_GetNextControlNoNext()
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
        public void ControlTests_GetNextControlNoNextReverse()
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
        public void ControlTests_AssignParent()
        {
            var cont = new Control();
            var parent = new Control();

            cont.AssignParent(parent);

            Assert.Equal(parent, cont.Parent);
        }

        [Fact]
        public void ControlTests_ParentChangedFromAssign()
        {
            bool wasChanged = false;
            var cont = new Control();
            cont.ParentChanged += (sender, args) => wasChanged = true;
            var parent = new Control();

            cont.AssignParent(parent);

            Assert.True(wasChanged);
        }

        [Fact]
        public void ControlTests_ParentChangedFromSet()
        {
            bool wasChanged = false;
            var cont = new Control();
            cont.ParentChanged += (sender, args) => wasChanged = true;
            var parent = new Control();

            cont.Parent = parent;

            Assert.True(wasChanged);
        }

        [Fact]
        public void ControlTests_ParentGetSet()
        {
            var parent = new Control();
            var cont = new Control();

            cont.Parent = parent;

            Assert.NotNull(cont.Parent);
            Assert.Equal(parent, cont.Parent);
        }

        [Fact]
        public void ControlTests_ParentInternalGetSet()
        {
            var parent = new Control();
            var cont = new Control();

            cont.ParentInternal = parent;

            Assert.NotNull(cont.Parent);
            Assert.Equal(parent, cont.ParentInternal);
            Assert.True(parent.Controls.Contains(cont));
        }

        [Fact]
        public void ControlTests_GetContainerControl()
        {
            var cont = new Control();

            var ret = cont.GetContainerControl();

            Assert.Null(ret);
        }

        #region Contains

        [Fact]
        public void ControlTests_Contains()
        {
            var cont = new Control();
            var child = new Control();
            cont.Controls.Add(child);

            // act and assert
            Assert.True(cont.Contains(child));
        }

        [Fact]
        public void ControlTests_ContainsGrandchild()
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
        public void ControlTests_ContainsNot()
        {
            var cont = new Control();
            var child = new Control();

            // act and assert
            Assert.False(cont.Contains(child));
        }

        #endregion

        [Fact]
        public void ControlTests_ParentingExcpetion()
        {
            var bootstrap = new Control();
            var paradox = new Control();
            bootstrap.Parent = paradox;

            // act and assert
            Assert.Throws<ArgumentException>(() => paradox.Parent = bootstrap);
        }

        [Fact]
        public void ControlTests_ChildingExcpetion()
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
        public void ControlTests_AccessibleNameGetSet()
        {
            var cont = new Control();

            cont.AccessibleName = "Foo";

            Assert.Equal("Foo", cont.AccessibleName);
        }

        /// <summary>
        /// Data for the AccessibleRole test
        /// </summary>
        public static TheoryData<AccessibleRole> AccessibleRoleData =>
            TestHelper.GetEnumTheoryData<AccessibleRole>();

        [Theory]
        [MemberData(nameof(AccessibleRoleData))]
        public void ControlTests_SetItemCheckState(AccessibleRole expected)
        {
            var cont = new Control();

            cont.AccessibleRole = expected;

            Assert.Equal(expected, cont.AccessibleRole);
        }

        /// <summary>
        /// Data for the AccessibleRoleInvalid test
        /// </summary>
        public static TheoryData<CheckState> AccessibleRoleInvalidData =>
            TestHelper.GetEnumTheoryDataInvalid<CheckState>();

        [Theory]
        [MemberData(nameof(AccessibleRoleInvalidData))]
        public void ControlTests_AccessibleRoleInvalid(AccessibleRole expected)
        {
            var cont = new Control();

            var ex = Assert.Throws<InvalidEnumArgumentException>(() => cont.AccessibleRole = expected);
            Assert.Equal("value", ex.ParamName);
        }

        /// <summary>
        /// Data for the IsAccessibleGetSet test
        /// </summary>
        public static TheoryData<bool> IsAccessibleGetSetData =>
            TestHelper.GetBoolTheoryData();

        [Theory]
        [MemberData(nameof(IsAccessibleGetSetData))]
        public void ControlTests_IsAccessibleGetSet(bool expected)
        {
            var cont = new Control();

            cont.IsAccessible = expected;

            Assert.Equal(expected, cont.IsAccessible);
        }

        #endregion

        #region Colors

        /// <summary>
        /// Data for the BackColorGetSet test
        /// </summary>
        public static TheoryData<Color> BackColorGetSetData =>
            TestHelper.GetColorTheoryData();

        [Theory]
        [MemberData(nameof(BackColorGetSetData))]
        public void ControlTests_BackColorGetSet(Color expected)
        {
            var cont = new Control();

            cont.BackColor = expected;

            Assert.Equal(expected, cont.BackColor);
        }

        [Fact]
        public void ControlTests_BackColorChanged()
        {
            bool wasChanged = false;
            var cont = new Control();
            cont.BackColorChanged += (sender, args) => wasChanged = true;

            cont.BackColor = Color.White;

            Assert.True(wasChanged);
        }


        /// <summary>
        /// Data for the ForeColorGetSet test
        /// </summary>
        public static TheoryData<Color> ForeColorGetSetData =>
            TestHelper.GetColorTheoryData();

        [Theory]
        [MemberData(nameof(ForeColorGetSetData))]
        public void ControlTests_ForeColorGetSet(Color expected)
        {
            var cont = new Control();

            cont.ForeColor = expected;

            Assert.Equal(expected, cont.ForeColor);
        }

        [Fact]
        public void ControlTests_ForeColorChanged()
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
            TestHelper.GetEnumTheoryData<ImageLayout>();

        [Theory]
        [MemberData(nameof(BackgroundImageLayoutData))]
        public void ControlTests_BackgroundImageLayoutGetSet(ImageLayout expected)
        {
            var cont = new Control();

            cont.BackgroundImageLayout = expected;

            Assert.Equal(expected, cont.BackgroundImageLayout);
        }

        /// <summary>
        /// Data for the BackgroundImageLayoutGetSetInvalid test
        /// </summary>
        public static TheoryData<ImageLayout> BackgroundImageLayoutGetSetInvalidData =>
            TestHelper.GetEnumTheoryDataInvalid<ImageLayout>();

        [Theory]
        [MemberData(nameof(BackgroundImageLayoutGetSetInvalidData))]
        public void ControlTests_BackgroundImageLayoutGetSetInvalid(ImageLayout expected)
        {
            var cont = new Control();

            var ex = Assert.Throws<InvalidEnumArgumentException>(() => cont.BackgroundImageLayout = expected);
            Assert.Equal("value", ex.ParamName);
        }

        #endregion

        #region Place and Shape

        [Fact]
        public void ControlTests_RegionGetSet()
        {
            var cont = new Control();
            var expected = new Region(new Rectangle(1, 1, 20, 20));

            cont.Region = expected;

            Assert.Equal(expected, cont.Region);
        }

        [Theory]
        [MemberData(nameof(AutoSizeGetSetData))]
        public void ControlTests_AutoSizeGetSet(bool expected)
        {
            var cont = new Control();

            cont.AutoSize = expected;

            Assert.Equal(expected, cont.AutoSize);
        }

        [Fact]
        public void ControlTests_PreferedSizeGet()
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
            TestHelper.GetIntTheoryData();

        [Theory]
        [MemberData(nameof(ApplySizeConstraintsData))]
        public void ControlTests_ApplySizeConstraints(int expected)
        {
            var cont = new Control();
            var expectedSize = new Size(expected, expected);

            var actualSize = cont.ApplySizeConstraints(expected, expected);

            Assert.Equal(expectedSize, actualSize);
        }

        /// <summary>
        /// Data for the ApplySizeConstraintsSize test
        /// </summary>
        public static TheoryData<Size> ApplySizeConstraintsSizeData =>
            TestHelper.GetSizeTheoryData();

        [Theory]
        [MemberData(nameof(ApplySizeConstraintsSizeData))]
        public void ControlTests_ApplySizeConstraintsSize(Size expectedSize)
        {
            var cont = new Control();

            var actualSize = cont.ApplySizeConstraints(expectedSize);

            Assert.Equal(expectedSize, actualSize);
        }

        #endregion

        #region ApplyBoundsConstraints

        /// <summary>
        /// Data for the ApplyBoundsConstraints test
        /// </summary>
        public static TheoryData<int> ApplyBoundsConstraintsData =>
            TestHelper.GetIntTheoryData();

        [Theory]
        [MemberData(nameof(ApplySizeConstraintsData))]
        public void ControlTests_ApplyBoundsConstraints(int expected)
        {
            var cont = new Control();
            var expectedBounds = new Rectangle(expected, expected, expected, expected);

            var actualBounds = cont.ApplyBoundsConstraints(expected, expected, expected, expected);

            Assert.Equal(expectedBounds, actualBounds);
        }

        #endregion

        /// <summary>
        /// Data for the PaddingGetSet test
        /// </summary>
        public static TheoryData<Padding> PaddingGetSetData =>
            TestHelper.GetPaddingTheoryData();

        [Theory]
        [MemberData(nameof(PaddingGetSetData))]
        public void ControlTests_PaddingGetSet(Padding expected)
        {
            var cont = new Control();

            cont.Padding = expected;

            Assert.Equal(expected, cont.Padding);
        }

        /// <summary>
        /// Data for the AnchorGetSet test
        /// </summary>
        public static TheoryData<AnchorStyles> AnchorData =>
            TestHelper.GetEnumTheoryData<AnchorStyles>();

        [Theory]
        [MemberData(nameof(AnchorData))]
        public void ControlTests_AnchorGetSet(AnchorStyles expected)
        {
            var cont = new Control();

            cont.Anchor = expected;

            Assert.Equal(expected, cont.Anchor);
        }

        [Fact]
        public void ControlTests_BoundsGetSet()
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
            TestHelper.GetIntTheoryData();

        [Theory]
        [MemberData(nameof(HeightGetSetData))]
        public void ControlTests_HeightGetSet(int expected)
        {
            var cont = new Control();

            cont.Height = expected;

            Assert.Equal(expected, cont.Height);
        }

        /// <summary>
        /// Data for the LeftGetSet test
        /// </summary>
        public static TheoryData<int> LeftGetSetData =>
            TestHelper.GetIntTheoryData();

        [Theory]
        [MemberData(nameof(LeftGetSetData))]
        public void ControlTests_LeftGetSet(int expected)
        {
            var cont = new Control();

            cont.Left = expected;

            Assert.Equal(expected, cont.Left);
        }

        /// <summary>
        /// Data for the TopGetSet test
        /// </summary>
        public static TheoryData<int> TopGetSetData =>
            TestHelper.GetIntTheoryData();

        [Theory]
        [MemberData(nameof(TopGetSetData))]
        public void ControlTests_TopGetSet(int expected)
        {
            var cont = new Control();

            cont.Top = expected;

            Assert.Equal(expected, cont.Top);
        }

        /// <summary>
        /// Data for the IsAccessibleGetSet test
        /// </summary>
        public static TheoryData<Point> LocationGetSetData =>
            TestHelper.GetPointTheoryData();

        [Theory]
        [MemberData(nameof(LocationGetSetData))]
        public void ControlTests_LocationGetSet(Point expected)
        {
            var cont = new Control();

            cont.Location = expected;

            Assert.Equal(expected, cont.Location);
        }

        /// <summary>
        /// Data for the MarginGetSet test
        /// </summary>
        public static TheoryData<Padding> MarginGetSetData =>
            TestHelper.GetPaddingTheoryData();

        [Theory]
        [MemberData(nameof(MarginGetSetData))]
        public void ControlTests_MarginGetSet(Padding expected)
        {
            var cont = new Control();

            cont.Margin = expected;

            Assert.Equal(expected, cont.Margin);
        }

        /// <summary>
        /// Data for the MaximumSizeGetSet test
        /// </summary>
        public static TheoryData<Size> MaximumSizeGetSetData =>
            TestHelper.GetSizeTheoryData();

        [Theory]
        [MemberData(nameof(MaximumSizeGetSetData))]
        public void ControlTests_MaximumSizeGetSet(Size expected)
        {
            var cont = new Control();

            cont.MaximumSize = expected;

            Assert.Equal(expected, cont.MaximumSize);
        }

        /// <summary>
        /// Data for the MinimumSizeGetSet test
        /// </summary>
        public static TheoryData<Size> MinimumSizeGetSetData =>
            TestHelper.GetSizeTheoryData();

        [Theory]
        [MemberData(nameof(MinimumSizeGetSetData))]
        public void ControlTests_MinimumSizeGetSet(Size expected)
        {
            var cont = new Control();

            cont.MinimumSize = expected;

            Assert.Equal(expected, cont.MinimumSize);
        }

        /// <summary>
        /// Data for the RequiredScaling test
        /// </summary>
        public static TheoryData<BoundsSpecified> RequiredScalingData =>
            TestHelper.GetEnumTheoryData<BoundsSpecified>();

        [Theory]
        [MemberData(nameof(RequiredScalingData))]
        public void ControlTests_RequiredScaling(BoundsSpecified expected)
        {
            var cont = new Control();

            cont.RequiredScaling = expected;

            Assert.Equal(expected, cont.RequiredScaling);
        }

        /// <summary>
        /// Data for the RequiredScalingEnabled test
        /// </summary>
        public static TheoryData<bool> RequiredScalingEnabledData =>
            TestHelper.GetBoolTheoryData();

        [Theory]
        [MemberData(nameof(RequiredScalingEnabledData))]
        public void ControlTests_RequiredScalingEnabled(bool expected)
        {
            var cont = new Control();

            cont.RequiredScalingEnabled = expected;

            Assert.Equal(expected, cont.RequiredScalingEnabled);
        }

        [Theory]
        [InlineData(RightToLeft.Yes)]
        [InlineData(RightToLeft.No)]
        public void ControlTests_RightToLeftTest(RightToLeft expected)
        {
            var cont = new Control();

            cont.RightToLeft = expected;

            Assert.Equal(expected, cont.RightToLeft);
        }

        [Fact]
        public void ControlTests_RightToLeftInherit()
        {
            var parent = new Control();
            var cont = new Control();
            cont.Parent = parent;

            parent.RightToLeft = RightToLeft.Yes;
            cont.RightToLeft = RightToLeft.Inherit;

            Assert.Equal(RightToLeft.Yes, cont.RightToLeft);
        }

        /// <summary>
        /// Data for the RightToLeftInvalid test
        /// </summary>
        public static TheoryData<RightToLeft> RightToLeftInvalidData =>
            TestHelper.GetEnumTheoryDataInvalid<RightToLeft>();

        [Theory]
        [MemberData(nameof(RightToLeftInvalidData))]
        public void ControlTests_RightToLeftInvalid(RightToLeft expected)
        {
            var cont = new Control();

            // act & assert
            var ex = Assert.Throws<InvalidEnumArgumentException>(() => cont.RightToLeft = expected);
            Assert.Equal("RightToLeft", ex.ParamName);
        }

        /// <summary>
        /// Data for the SizeGetSet test
        /// </summary>
        public static TheoryData<Size> SizeGetSetData =>
            TestHelper.GetSizeTheoryData();

        [Theory]
        [MemberData(nameof(SizeGetSetData))]
        public void ControlTests_SizeGetSet(Size expected)
        {
            var cont = new Control();

            cont.Size = expected;

            Assert.Equal(expected, cont.Size);
        }

        /// <summary>
        /// Data for the WidthGetSet test
        /// </summary>
        public static TheoryData<int> WidthGetSetData =>
            TestHelper.GetIntTheoryData();

        [Theory]
        [MemberData(nameof(WidthGetSetData))]
        public void ControlTests_WidthGetSet(int expected)
        {
            var cont = new Control();

            cont.Width = expected;

            Assert.Equal(expected, cont.Width);
        }



        #endregion

        #region Events

        [Fact]
        public void ControlTests_Enter()
        {
            bool wasChanged = false;
            var cont = new Control();
            cont.Enter += (sender, args) => wasChanged = true;

            cont.NotifyEnter();

            Assert.True(wasChanged);
        }

        [Fact]
        public void ControlTests_Leave()
        {
            bool wasChanged = false;
            var cont = new Control();
            cont.Leave += (sender, args) => wasChanged = true;

            cont.NotifyLeave();

            Assert.True(wasChanged);
        }

        [Fact]
        public void ControlTests_PaddingChanged()
        {
            bool wasChanged = false;
            var cont = new Control();
            cont.PaddingChanged += (sender, args) => wasChanged = true;

            cont.Padding = new Padding(1);

            Assert.True(wasChanged);
        }

        [Fact]
        public void ControlTests_ControlAddedAndRemoved()
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
        public void ControlTests_TextChanged()
        {
            bool wasChanged = false;
            var cont = new Control();
            cont.TextChanged += (sender, args) => wasChanged = true;

            cont.Text = "foo";

            Assert.True(wasChanged);
        }

        [Fact]
        public void ControlTests_LocationChanged()
        {
            bool wasChanged = false;
            var cont = new Control();
            cont.LocationChanged += (sender, args) => wasChanged = true;

            cont.Location = new Point(1, 1);

            Assert.True(wasChanged);
        }

        [Fact]
        public void ControlTests_EnabledChanged()
        {
            bool wasEnabled = false;
            var cont = new Control();
            cont.EnabledChanged += (sender, args) => wasEnabled = true;

            cont.Enabled = false;

            Assert.True(wasEnabled);
        }

        [Fact]
        public void ControlTests_FontChanged()
        {
            bool wasChanged = false;
            var cont = new Control();
            cont.FontChanged += (sender, args) => wasChanged = true;

            cont.Font = new Font(new FontFamily(Drawing.Text.GenericFontFamilies.Serif), 1.0f);

            Assert.True(wasChanged);
        }

        [Fact]
        public void ControlTests_MarginChanged()
        {
            bool wasChanged = false;
            var cont = new Control();
            cont.MarginChanged += (sender, args) => wasChanged = true;

            cont.Margin = new Padding(1);

            Assert.True(wasChanged);
        }

        [Fact]
        public void ControlTests_RegionChanged()
        {
            bool wasChanged = false;
            var cont = new Control();
            cont.RegionChanged += (sender, args) => wasChanged = true;

            cont.Region = new Region(new Rectangle(1, 1, 20, 20));

            Assert.True(wasChanged);
        }

        [Fact]
        public void ControlTests_RightToLeftChanged()
        {
            bool wasChanged = false;
            var cont = new Control();
            cont.RightToLeftChanged += (sender, args) => wasChanged = true;

            cont.RightToLeft = RightToLeft.Yes;

            Assert.True(wasChanged);
        }

        [Fact]
        public void ControlTests_VisibleChanged()
        {
            bool wasChanged = false;
            var cont = new Control();
            cont.VisibleChanged += (sender, args) => wasChanged = true;

            cont.Visible = false;

            Assert.True(wasChanged);
        }

        [Fact]
        public void ControlTests_SizeChanged()
        {
            bool wasChanged = false;
            var cont = new Control();
            cont.SizeChanged += (sender, args) => wasChanged = true;

            cont.Size = new Size(50, 50);

            Assert.True(wasChanged);
        }

        [Fact]
        public void ControlTests_TabIndexChanged()
        {
            bool wasChanged = false;
            var cont = new Control();
            cont.TabIndexChanged += (sender, args) => wasChanged = true;

            cont.TabIndex = 1;

            Assert.True(wasChanged);
        }

        [Fact]
        public void ControlTests_TabStopChanged()
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
            TestHelper.GetBoolTheoryData();

        [Theory]
        [MemberData(nameof(EnabledGetSetData))]
        public void ControlTests_EnabledGetSet(bool expected)
        {
            var cont = new Control();

            cont.Enabled = expected;

            Assert.Equal(expected, cont.Enabled);
        }

        /// <summary>
        /// Data for the VisibleGetSet test
        /// </summary>
        public static TheoryData<bool> VisibleGetSetData =>
            TestHelper.GetBoolTheoryData();

        [Theory]
        [MemberData(nameof(VisibleGetSetData))]
        public void ControlTests_VisibleGetSet(bool expected)
        {
            var cont = new Control();

            cont.Visible = expected;

            Assert.Equal(expected, cont.Visible);
        }

        [Fact]
        public void ControlTests_GetVisibleCoreNoParent()
        {
            var cont = new Control();

            // act & assert
            Assert.Null(cont.Parent);
            Assert.True(cont.GetVisibleCore());
        }

        [Fact]
        public void ControlTests_GetVisibleCoreVisibleParent()
        {
            var cont = new Control();
            var parent = new Control();
            cont.Parent = parent;

            // act & assert
            Assert.True(cont.GetVisibleCore());
        }

        [Fact]
        public void ControlTests_Hide()
        {
            var cont = new Control();
            cont.Visible = true;

            cont.Hide();

            Assert.False(cont.Visible);
        }

        #endregion

        #region Font

        /// <summary>
        /// Data for the FontGetSet test
        /// </summary>
        public static TheoryData<Font> FontGetSetData =>
            TestHelper.GetFontTheoryData();

        [Theory]
        [MemberData(nameof(FontGetSetData))]
        public void ControlTests_FontGetSet(Font expected)
        {
            var cont = new Control();

            cont.Font = expected;

            Assert.Equal(expected, cont.Font);
        }

        [Theory]
        [InlineData(10.0f)]
        [InlineData(0.1f)]
        [InlineData(System.Single.Epsilon)]
        public void ControlTests_ScaleFont(float expected)
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
            TestHelper.GetStringTheoryData();

        [Theory]
        [MemberData(nameof(WindowTextGetSetData))]
        public void ControlTests_WindowTextGetSet(string expected)
        {
            var cont = new Control();

            cont.WindowText = expected;

            Assert.Equal(expected, cont.WindowText);
        }

        /// <summary>
        /// Data for the NameGetSet test
        /// </summary>
        public static TheoryData<string> NameGetSetData =>
            TestHelper.GetStringTheoryData();

        [Theory]
        [MemberData(nameof(NameGetSetData))]
        public void ControlTests_NameGetSet(string expected)
        {
            var cont = new Control();

            cont.Name = expected;

            Assert.Equal(expected, cont.Name);
        }

        /// <summary>
        /// Data for the TextGetSet test
        /// </summary>
        public static TheoryData<string> TextGetSetData =>
            TestHelper.GetStringTheoryData();

        [Theory]
        [MemberData(nameof(TextGetSetData))]
        public void ControlTests_TextGetSet(string expected)
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
            TestHelper.GetBoolTheoryData();

        [Theory]
        [MemberData(nameof(CaptureGetSetData))]
        public void ControlTests_CaptureGetSet(bool expected)
        {
            var cont = new Control();

            cont.Capture = expected;

            Assert.Equal(expected, cont.Capture);
        }

        /// <summary>
        /// Data for the CaptureInternalGetSet test
        /// </summary>
        public static TheoryData<bool> CaptureInternalGetSetData =>
            TestHelper.GetBoolTheoryData();

        [Theory]
        [MemberData(nameof(CaptureInternalGetSetData))]
        public void ControlTests_CaptureInternalGetSet(bool expected)
        {
            var cont = new Control();

            cont.CaptureInternal = expected;

            Assert.Equal(expected, cont.CaptureInternal);
        }

        #endregion

        #region CanProcessMnemonic

        [Fact]
        public void ControlTests_CanProcessMnemonic()
        {
            var cont = new Control();

            // act and assert
            Assert.True(cont.CanProcessMnemonic());
        }

        [Fact]
        public void ControlTests_CanProcessMnemonicNotEnabled()
        {
            var cont = new Control();
            cont.Enabled = false;

            // act and assert
            Assert.False(cont.CanProcessMnemonic());
        }

        [Fact]
        public void ControlTests_CanProcessMnemonicNotVisible()
        {
            var cont = new Control();
            cont.Visible = false;

            // act and assert
            Assert.False(cont.CanProcessMnemonic());
        }

        [Fact]
        public void ControlTests_CanProcessMnemonicParent()
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
        public void ControlTests_CanSelectCore()
        {
            var cont = new Control();

            // act and assert
            Assert.True(cont.CanSelectCore());
        }

        [Fact]
        public void ControlTests_CanSelectCoreNotEnabled()
        {
            var cont = new Control();
            cont.Enabled = false;

            // act and assert
            Assert.False(cont.CanSelectCore());
        }

        [Fact]
        public void ControlTests_CanSelectCoreParentNotEnabled()
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

        #region FindForm

        [Fact]
        public void ControlTests_FindForm()
        {
            var cont = new Control();
            var form = new Form();
            cont.Parent = form;

            // act and assert
            Assert.Equal(form, cont.FindForm());
        }

        [Fact]
        public void ControlTests_FindFormNot()
        {
            var cont = new Control();

            // act and assert
            Assert.NotEqual(cont, cont.FindForm());
            // returns the control itself if not found
        }

        [Fact]
        public void ControlTests_FindFormInternal()
        {
            var cont = new Control();
            var form = new Form();
            cont.Parent = form;

            // act and assert
            Assert.Equal(form, cont.FindFormInternal());
        }

        [Fact]
        public void ControlTests_FindFormInternalNot()
        {
            var cont = new Control();

            // act and assert
            Assert.NotEqual(cont, cont.FindFormInternal());
            // returns the control itself if not found
        }

        #endregion

        #region GetChildAtPoint

        /// <summary>
        /// Data for the GetChildAtPointNull test
        /// </summary>
        public static TheoryData<GetChildAtPointSkip> GetChildAtPointNullData =>
            TestHelper.GetEnumTheoryData<GetChildAtPointSkip>();

        [Theory]
        [MemberData(nameof(GetChildAtPointNullData))]
        public void ControlTests_GetChildAtPointNull(GetChildAtPointSkip skip)
        {
            var cont = new Control();

            var ret = cont.GetChildAtPoint(new Point(5, 5), skip);

            Assert.Null(ret);
        }

        /// <summary>
        /// Data for the GetChildAtPointInvalid test
        /// </summary>
        public static TheoryData<GetChildAtPointSkip> GetChildAtPointInvalidData =>
            TestHelper.GetEnumTheoryDataInvalid<GetChildAtPointSkip>();

        [Theory]
        [MemberData(nameof(GetChildAtPointInvalidData))]
        public void ControlTests_GetChildAtPointInvalid(GetChildAtPointSkip skip)
        {
            var cont = new Control();

            // act & assert
            var ex = Assert.Throws<InvalidEnumArgumentException>(() => cont.GetChildAtPoint(new Point(5, 5), skip));
            Assert.Equal("skipValue", ex.ParamName);
        }

        #endregion

        [Fact]
        public void ControlTests_GetHandle()
        {
            var cont = new Control();

            var intptr = cont.Handle;

            Assert.NotEqual(IntPtr.Zero, intptr);
        }

        [Fact]
        public void ControlTests_GetHandleInternalShouldBeZero()
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
            TestHelper.GetEnumTheoryData<DragDropEffects>();

        [Theory]
        [MemberData(nameof(DoDragDropData))]
        public void ControlTests_DoDragDrop(DragDropEffects expected)
        {
            var cont = new Control();
            var mock = new Mock<IDataObject>(MockBehavior.Strict);

            var ret = cont.DoDragDrop(mock.Object, expected);

            Assert.Equal(DragDropEffects.None, ret);
        }

        // TODO: create a focus test that returns true when a handle has been created
        [Fact]
        public void ControlTests_FocusHandleNotCreated()
        {
            var cont = new Control();

            var ret = cont.Focus();

            Assert.False(ret);
        }

        #region Misc. GetSet

        [Fact]
        public void ControlTests_SiteGetSet()
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
            TestHelper.GetBoolTheoryData();

        [Theory]
        [MemberData(nameof(UseWaitCursorGetSetData))]
        public void ControlTests_UseWaitCursorGetSet(bool expected)
        {
            var cont = new Control();

            cont.UseWaitCursor = expected;

            Assert.Equal(expected, cont.UseWaitCursor);
        }


        [Theory]
        [InlineData(true, true)]
        [InlineData(false, true)] // setting is impossible; default is false
        // SupportsUseCompatibleTextRendering is always false
        public void ControlTests_UseCompatibleTextRenderingIntGetSet(bool given, bool expected)
        {
            var cont = new Control();

            cont.UseCompatibleTextRenderingInt = given;

            Assert.Equal(expected, cont.UseCompatibleTextRenderingInt);
        }

        [Fact]
        public void ControlTests_WindowTargetGetSet()
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
            TestHelper.GetBoolTheoryData();

        [Theory]
        [MemberData(nameof(AllowDropData))]
        public void ControlTests_AllowDropGetSet(bool expected)
        {
            var cont = new Control();

            cont.AllowDrop = expected;

            Assert.Equal(expected, cont.AllowDrop);
        }

        /// <summary>
        /// Data for the AutoSizeGetSet test
        /// </summary>
        public static TheoryData<bool> AutoSizeGetSetData =>
            TestHelper.GetBoolTheoryData();

        /// <summary>
        /// Data for the AutoScrollOffsetGetSet test
        /// </summary>
        public static TheoryData<Point> AutoScrollOffsetGetSetData =>
            TestHelper.GetPointTheoryData();

        [Theory]
        [MemberData(nameof(AutoScrollOffsetGetSetData))]
        public void ControlTests_AutoScrollOffsetGetSet(Point expected)
        {
            var cont = new Control();

            cont.AutoScrollOffset = expected;

            Assert.Equal(expected, cont.AutoScrollOffset);
        }

        [Fact]
        public void ControlTests_BindingContextGetSet()
        {
            var cont = new Control();
            var expected = new BindingContext();

            cont.BindingContext = expected;

            Assert.NotNull(cont.BindingContext);
            Assert.Equal(expected, cont.BindingContext);
        }

        /// <summary>
        /// Data for the CausesValidationGetSet test
        /// </summary>
        public static TheoryData<bool> CausesValidationData =>
            TestHelper.GetBoolTheoryData();

        [Theory]
        [MemberData(nameof(CausesValidationData))]
        public void ControlTests_CausesValidationGetSet(bool expected)
        {
            var cont = new Control();

            cont.CausesValidation = expected;

            Assert.Equal(expected, cont.CausesValidation);
        }

        [Theory]
        [InlineData(false, false)]
        [InlineData(true, false)] // giving true cannot set to true
        public void ControlTests_CacheTextInternalGetSet(bool given, bool expected)
        {
            var cont = new Control();

            cont.CacheTextInternal = given;

            Assert.Equal(expected, cont.CacheTextInternal);
        }

        /// <summary>
        /// Data for the ClientSizeGetSet test
        /// </summary>
        public static TheoryData<Size> ClientSizeGetSetData =>
            TestHelper.GetSizeTheoryData();

        [Theory]
        [MemberData(nameof(ClientSizeGetSetData))]
        public void ControlTests_ClientSizeGetSet(Size expected)
        {
            var cont = new Control();

            cont.ClientSize = expected;

            Assert.Equal(expected, cont.ClientSize);
        }

        [Fact]
        public void ControlTests_ContextMenuGetSet()
        {
            var cont = new Control();
            var expected = new ContextMenu();

            cont.ContextMenu = expected;

            Assert.Equal(expected, cont.ContextMenu);
        }

        [Fact]
        public void ControlTests_ContextMenuStripGetSet()
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
            TestHelper.GetBoolTheoryData();

        [Theory]
        [MemberData(nameof(ValidationCancelledGetSetData))]
        public void ControlTests_ValidationCancelledGetSet(bool expected)
        {
            var cont = new Control();

            cont.ValidationCancelled = expected;

            Assert.Equal(expected, cont.ValidationCancelled);
        }

        /// <summary>
        /// Data for the IsTopMdiWindowClosingGetSet test
        /// </summary>
        public static TheoryData<bool> IsTopMdiWindowClosingGetSetData =>
            TestHelper.GetBoolTheoryData();

        [Theory]
        [MemberData(nameof(IsTopMdiWindowClosingGetSetData))]
        public void ControlTests_IsTopMdiWindowClosingGetSet(bool expected)
        {
            var cont = new Control();

            cont.IsTopMdiWindowClosing = expected;

            Assert.Equal(expected, cont.IsTopMdiWindowClosing);
        }

        /// <summary>
        /// Data for the CursorGetSet test
        /// </summary>
        public static TheoryData<Cursor> CursorGetSetData =>
            TestHelper.GetCursorTheoryData();

        [Theory]
        [MemberData(nameof(CursorGetSetData))]
        public void ControlTests_CursorGetSet(Cursor expected)
        {
            var cont = new Control();

            cont.Cursor = expected;

            Assert.Equal(expected, cont.Cursor);
        }

        /// <summary>
        /// Data for the DockGetSet test
        /// </summary>
        public static TheoryData<DockStyle> DockGetSetData =>
            TestHelper.GetEnumTheoryData<DockStyle>();

        [Theory]
        [MemberData(nameof(DockGetSetData))]
        public void ControlTests_DockGetSet(DockStyle expected)
        {
            var cont = new Control();

            cont.Dock = expected;

            Assert.Equal(expected, cont.Dock);
        }

        #endregion

    }
}
