
using Xunit;
using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms.Tests
{
    public class ControlTests
    {
        [Fact]
        public void Constructor()
        {
            // act
            var cont = new Control();

            // assert
            Assert.NotNull(cont);
            Assert.Equal(DpiHelper.DeviceDpi, cont.deviceDpi);
            Assert.True(cont.RequiredScalingEnabled);
            Assert.Equal(BoundsSpecified.All, cont.RequiredScaling);
            Assert.Equal(0, cont.TabIndex);
            Assert.Equal(Size.Empty, cont.Size);
            Assert.Null(cont.Parent);
        }

        [Fact]
        public void ConstructorText()
        {
            // act
            var cont = new Control("Foo");

            // assert
            Assert.Equal("Foo", cont.Text);
            Assert.Null(cont.Parent);
        }

        [Fact]
        public void ConstructorSize()
        {
            // act
            var cont = new Control("Foo", 1, 2, 3, 4);

            // assert
            Assert.Equal("Foo", cont.Text);
            Assert.Equal(1, cont.Left);
            Assert.Equal(2, cont.Top);
            Assert.Equal(3, cont.Width);
            Assert.Equal(4, cont.Height);
        }

        [Fact]
        public void ConstructorParent()
        {
            // arrange
            var parent = new Control();

            // act
            var cont = new Control(parent, "Foo");

            // assert
            Assert.NotNull(cont.Parent);
            Assert.Equal(parent, cont.Parent);
            Assert.Equal("Foo", cont.Text);
            Assert.Equal(Size.Empty, cont.Size);
        }

        [Fact]
        public void ConstructorAll()
        {
            // arrange
            var parent = new Control();

            // act
            var cont = new Control(parent, "Foo", 1, 2, 3, 4);

            // assert
            Assert.Equal(parent, cont.Parent);
            Assert.Equal("Foo", cont.Text);
            Assert.Equal(1, cont.Left);
            Assert.Equal(2, cont.Top);
            Assert.Equal(3, cont.Width);
            Assert.Equal(4, cont.Height);
        }

        [Fact]
        public void AccessibleNameGetSet()
        {
            // arrange
            var cont = new Control();

            // act
            cont.AccessibleName = "Foo";

            // assert
            Assert.Equal("Foo", cont.AccessibleName);
        }

        /// <summary>
        /// Data for the AccessibleRole test
        /// </summary>
        public static TheoryData<AccessibleRole> AccessibleRoleData =>
            TestHelper.GetEnumTheoryData<AccessibleRole>();

        [Theory]
        [MemberData(nameof(AccessibleRoleData))]
        public void SetItemCheckState(AccessibleRole expected)
        {
            // arrange
            var cont = new Control();

            // act
            cont.AccessibleRole = expected;

            // assert
            Assert.Equal(expected, cont.AccessibleRole);
        }

        /// <summary>
        /// Data for the AccessibleRoleInvalid test
        /// </summary>
        public static TheoryData<CheckState> AccessibleRoleInvalidData =>
            TestHelper.GetEnumTheoryDataInvalid<CheckState>();

        [Theory]
        [MemberData(nameof(AccessibleRoleInvalidData))]
        public void AccessibleRoleInvalid(AccessibleRole expected)
        {
            // arrange
            var cont = new Control();

            var ex = Assert.Throws<InvalidEnumArgumentException>(() => cont.AccessibleRole = expected);
            Assert.Equal("value", ex.ParamName);
        }

        /// <summary>
        /// Data for the AllowDropGetSet test
        /// </summary>
        public static TheoryData<bool> AllowDropData =>
            TestHelper.GetBoolTheoryData();

        [Theory]
        [MemberData(nameof(AllowDropData))]
        public void AllowDropGetSet(bool expected)
        {
            // arrange
            var cont = new Control();

            // act
            cont.AllowDrop = expected;

            // assert
            Assert.Equal(expected, cont.AllowDrop);
        }

        /// <summary>
        /// Data for the AnchorGetSet test
        /// </summary>
        public static TheoryData<AnchorStyles> AnchorData =>
            TestHelper.GetEnumTheoryData<AnchorStyles>();

        [Theory]
        [MemberData(nameof(AnchorData))]
        public void AnchorGetSet(AnchorStyles expected)
        {
            // arrange
            var cont = new Control();

            // act
            cont.Anchor = expected;

            // assert
            Assert.Equal(expected, cont.Anchor);
        }

        /// <summary>
        /// Data for the AutoSizeGetSet test
        /// </summary>
        public static TheoryData<bool> AutoSizeGetSetData =>
            TestHelper.GetBoolTheoryData();

        [Theory]
        [MemberData(nameof(AutoSizeGetSetData))]
        public void AutoSizeGetSet(bool expected)
        {
            // arrange
            var cont = new Control();

            // act
            cont.AutoSize = expected;

            // assert
            Assert.Equal(expected, cont.AutoSize);
        }

        /// <summary>
        /// Data for the AutoScrollOffsetGetSet test
        /// </summary>
        public static TheoryData<Point> AutoScrollOffsetGetSetData =>
            TestHelper.GetPointTheoryData();

        [Theory]
        [MemberData(nameof(AutoScrollOffsetGetSetData))]
        public void AutoScrollOffsetGetSet(Point expected)
        {
            // arrange
            var cont = new Control();

            // act
            cont.AutoScrollOffset = expected;

            // assert
            Assert.Equal(expected, cont.AutoScrollOffset);
        }

        /// <summary>
        /// Data for the BackColorGetSet test
        /// </summary>
        public static TheoryData<Color> BackColorGetSetData =>
            TestHelper.GetColorTheoryData();

        [Theory]
        [MemberData(nameof(BackColorGetSetData))]
        public void BackColorGetSet(Color expected)
        {
            // arrange
            var cont = new Control();

            // act
            cont.BackColor = expected;

            // assert
            Assert.Equal(expected, cont.BackColor);
        }

        /// <summary>
        /// Data for the BackgroundImageLayoutGetSet test
        /// </summary>
        public static TheoryData<ImageLayout> BackgroundImageLayoutData =>
            TestHelper.GetEnumTheoryData<ImageLayout>();

        [Theory]
        [MemberData(nameof(BackgroundImageLayoutData))]
        public void BackgroundImageLayoutGetSet(ImageLayout expected)
        {
            // arrange
            var cont = new Control();

            // act
            cont.BackgroundImageLayout = expected;

            // assert
            Assert.Equal(expected, cont.BackgroundImageLayout);
        }

        /// <summary>
        /// Data for the BackgroundImageLayoutGetSetInvalid test
        /// </summary>
        public static TheoryData<ImageLayout> BackgroundImageLayoutGetSetInvalidData =>
            TestHelper.GetEnumTheoryDataInvalid<ImageLayout>();

        [Theory]
        [MemberData(nameof(BackgroundImageLayoutGetSetInvalidData))]
        public void BackgroundImageLayoutGetSetInvalid(ImageLayout expected)
        {
            // arrange
            var cont = new Control();

            var ex = Assert.Throws<InvalidEnumArgumentException>(() => cont.BackgroundImageLayout = expected);
            Assert.Equal("value", ex.ParamName);
        }

        [Fact]
        public void BindingContextGetSet()
        {
            // arrange
            var cont = new Control();
            var expected = new BindingContext();

            // act
            cont.BindingContext = expected;

            // assert
            Assert.NotNull(cont.BindingContext);
            Assert.Equal(expected, cont.BindingContext);
        }

        [Fact]
        public void BoundsGetSet()
        {
            // arrange
            var cont = new Control();
            var expected = new Rectangle(1,1,20,20);

            // act
            cont.Bounds = expected;

            // assert
            Assert.Equal(expected, cont.Bounds);
        }

        [Fact]
        public void HeightGetSet()
        {
            // arrange
            var cont = new Control();
            var expected = 40;

            // act
            cont.Height = expected;

            // assert
            Assert.Equal(expected, cont.Height);
        }

        [Fact]
        public void LeftGetSet()
        {
            // arrange
            var cont = new Control();
            var expected = 2;

            // act
            cont.Left = expected;

            // assert
            Assert.Equal(expected, cont.Left);
        }

        /// <summary>
        /// Data for the IsAccessibleGetSet test
        /// </summary>
        public static TheoryData<Point> LocationGetSetData =>
            TestHelper.GetPointTheoryData();

        [Theory]
        [MemberData(nameof(LocationGetSetData))]
        public void LocationGetSet(Point expected)
        {
            // arrange
            var cont = new Control();

            // act
            cont.Location = expected;

            // assert
            Assert.Equal(expected, cont.Location);
        }

        [Fact]
        public void LocationChanged()
        {
            // arrange
            bool wasChanged = false;
            var cont = new Control();
            cont.LocationChanged += (sender, args) => wasChanged = true;

            // act
            cont.Location = new Point(1,1);

            // assert
            Assert.True(wasChanged);
        }

        /// <summary>
        /// Data for the CaptureGetSet test
        /// </summary>
        public static TheoryData<bool> CaptureGetSetData =>
            TestHelper.GetBoolTheoryData();

        [Theory]
        [MemberData(nameof(CaptureGetSetData))]
        public void CaptureGetSet(bool expected)
        {
            // arrange
            var cont = new Control();

            // act
            cont.Capture = expected;

            // assert
            Assert.Equal(expected, cont.Capture);
        }

        /// <summary>
        /// Data for the CaptureInternalGetSet test
        /// </summary>
        public static TheoryData<bool> CaptureInternalGetSetData =>
            TestHelper.GetBoolTheoryData();

        [Theory]
        [MemberData(nameof(CaptureInternalGetSetData))]
        public void CaptureInternalGetSet(bool expected)
        {
            // arrange
            var cont = new Control();

            // act
            cont.CaptureInternal = expected;

            // assert
            Assert.Equal(expected, cont.CaptureInternal);
        }

        /// <summary>
        /// Data for the CausesValidationGetSet test
        /// </summary>
        public static TheoryData<bool> CausesValidationData =>
            TestHelper.GetBoolTheoryData();

        [Theory]
        [MemberData(nameof(CausesValidationData))]
        public void CausesValidationGetSet(bool expected)
        {
            // arrange
            var cont = new Control();

            // act
            cont.CausesValidation = expected;

            // assert
            Assert.Equal(expected, cont.CausesValidation);
        }

        [Theory]
        [InlineData(false, false)]
        [InlineData(true, false)] // giving true cannot set to true
        public void CacheTextInternalGetSet(bool given, bool expected)
        {
            // arrange
            var cont = new Control();

            // act
            cont.CacheTextInternal = given;

            // assert
            Assert.Equal(expected, cont.CacheTextInternal);
        }

        /// <summary>
        /// Data for the ClientSizeGetSet test
        /// </summary>
        public static TheoryData<Size> ClientSizeGetSetData =>
            TestHelper.GetSizeTheoryData();

        [Theory]
        [MemberData(nameof(ClientSizeGetSetData))]
        public void ClientSizeGetSet(Size expected)
        {
            // arrange
            var cont = new Control();

            // act
            cont.ClientSize = expected;

            // assert
            Assert.Equal(expected, cont.ClientSize);
        }

        [Fact]
        public void ContextMenuGetSet()
        {
            // arrange
            var cont = new Control();
            var expected = new ContextMenu();

            // act
            cont.ContextMenu = expected;

            // assert
            Assert.Equal(expected, cont.ContextMenu);
        }

        [Fact]
        public void ContextMenuStripGetSet()
        {
            // arrange
            var cont = new Control();
            var expected = new ContextMenuStrip();

            // act
            cont.ContextMenuStrip = expected;

            // assert
            Assert.Equal(expected, cont.ContextMenuStrip);
        }

        /// <summary>
        /// Data for the ValidationCancelledGetSet test
        /// </summary>
        public static TheoryData<bool> ValidationCancelledGetSetData =>
            TestHelper.GetBoolTheoryData();

        [Theory]
        [MemberData(nameof(ValidationCancelledGetSetData))]
        public void ValidationCancelledGetSet(bool expected)
        {
            // arrange
            var cont = new Control();

            // act
            cont.ValidationCancelled = expected;

            // assert
            Assert.Equal(expected, cont.ValidationCancelled);
        }

        /// <summary>
        /// Data for the IsTopMdiWindowClosingGetSet test
        /// </summary>
        public static TheoryData<bool> IsTopMdiWindowClosingGetSetData =>
            TestHelper.GetBoolTheoryData();

        [Theory]
        [MemberData(nameof(IsTopMdiWindowClosingGetSetData))]
        public void IsTopMdiWindowClosingGetSet(bool expected)
        {
            // arrange
            var cont = new Control();

            // act
            cont.IsTopMdiWindowClosing = expected;

            // assert
            Assert.Equal(expected, cont.IsTopMdiWindowClosing);
        }

        /// <summary>
        /// Data for the CursorGetSet test
        /// </summary>
        public static TheoryData<Cursor> CursorGetSetData =>
            TestHelper.GetCursorTheoryData();

        [Theory]
        [MemberData(nameof(CursorGetSetData))]
        public void CursorGetSet(Cursor expected)
        {
            // arrange
            var cont = new Control();

            // act
            cont.Cursor = expected;

            // assert
            Assert.Equal(expected, cont.Cursor);
        }

        /// <summary>
        /// Data for the DockGetSet test
        /// </summary>
        public static TheoryData<DockStyle> DockGetSetData =>
            TestHelper.GetEnumTheoryData<DockStyle>();

        [Theory]
        [MemberData(nameof(DockGetSetData))]
        public void DockGetSet(DockStyle expected)
        {
            // arrange
            var cont = new Control();

            // act
            cont.Dock = expected;

            // assert
            Assert.Equal(expected, cont.Dock);
        }

        /// <summary>
        /// Data for the EnabledGetSet test
        /// </summary>
        public static TheoryData<bool> EnabledGetSetData =>
            TestHelper.GetBoolTheoryData();

        [Theory]
        [MemberData(nameof(EnabledGetSetData))]
        public void EnabledGetSet(bool expected)
        {
            // arrange
            var cont = new Control();

            // act
            cont.Enabled = expected;

            // assert
            Assert.Equal(expected, cont.Enabled);
        }

        [Fact]
        public void EnabledChanged()
        {
            // arrange
            bool wasEnabled = false;
            var cont = new Control();
            cont.EnabledChanged += (sender, args) => wasEnabled = true;

            // act
            cont.Enabled = false;

            //assert
            Assert.True(wasEnabled);
        }

        /// <summary>
        /// Data for the FontGetSet test
        /// </summary>
        public static TheoryData<Font> FontGetSetData =>
            TestHelper.GetFontTheoryData();

        [Theory]
        [MemberData(nameof(FontGetSetData))]
        public void FontGetSet(Font expected)
        {
            // arrange
            var cont = new Control();

            // act
            cont.Font = expected;

            //assert
            Assert.Equal(expected, cont.Font);
        }

        [Theory]
        [InlineData(10.0f)]
        [InlineData(0.1f)]
        [InlineData(System.Single.Epsilon)]
        public void ScaleFont(float expected)
        {
            // arrange
            var cont = new Control();
            cont.Font = new Font(new FontFamily(Drawing.Text.GenericFontFamilies.Serif), 1.0f);

            // act
            cont.ScaleFont(expected);

            //assert
            Assert.Equal(expected, cont.Font.Size);
        }

        [Fact]
        public void FontChanged()
        {
            // arrange
            bool wasChanged = false;
            var cont = new Control();
            cont.FontChanged += (sender, args) => wasChanged = true;

            // act
            cont.Font = new Font(new FontFamily(Drawing.Text.GenericFontFamilies.Serif), 1.0f);

            //assert
            Assert.True(wasChanged);
        }

        /// <summary>
        /// Data for the ForeColorGetSet test
        /// </summary>
        public static TheoryData<Color> ForeColorGetSetData =>
            TestHelper.GetColorTheoryData();

        [Theory]
        [MemberData(nameof(ForeColorGetSetData))]
        public void ForeColorGetSet(Color expected)
        {
            // arrange
            var cont = new Control();

            // act
            cont.ForeColor = expected;

            //assert
            Assert.Equal(expected, cont.ForeColor);
        }

        [Fact]
        public void ForeColorChanged()
        {
            // arrange
            bool wasChanged = false;
            var cont = new Control();
            cont.ForeColorChanged += (sender, args) => wasChanged = true;

            // act
            cont.ForeColor = Color.Black;

            //assert
            Assert.True(wasChanged);
        }

        [Fact]
        public void GetHandle()
        {
            // arrange
            var cont = new Control();

            // act
            var intptr = cont.Handle;

            // assert
            Assert.NotEqual(IntPtr.Zero, intptr);
        }

        [Fact]
        public void GetHandleInternalShouldBeZero()
        {
            // arrange
            var cont = new Control();

            // act
            var intptr = cont.HandleInternal;

            // assert
            Assert.Equal(IntPtr.Zero, intptr);
            Assert.False(cont.IsHandleCreated);
        }

        /// <summary>
        /// Data for the IsAccessibleGetSet test
        /// </summary>
        public static TheoryData<bool> IsAccessibleGetSetData =>
            TestHelper.GetBoolTheoryData();

        [Theory]
        [MemberData(nameof(IsAccessibleGetSetData))]
        public void IsAccessibleGetSet(bool expected)
        {
            // arrange
            var cont = new Control();

            // act
            cont.IsAccessible = expected;

            // assert
            Assert.Equal(expected, cont.IsAccessible);
        }

        /// <summary>
        /// Data for the MarginGetSet test
        /// </summary>
        public static TheoryData<Padding> MarginGetSetData =>
            TestHelper.GetPaddingTheoryData();

        [Theory]
        [MemberData(nameof(MarginGetSetData))]
        public void MarginGetSet(Padding expected)
        {
            // arrange
            var cont = new Control();

            // act
            cont.Margin = expected;

            // assert
            Assert.Equal(expected, cont.Margin);
        }

        [Fact]
        public void MarginChanged()
        {
            bool wasChanged = false;
            var cont = new Control();
            cont.MarginChanged += (sender, args) => wasChanged = true;

            // act
            cont.Margin = new Padding(1);

            // assert
            Assert.True(wasChanged);
        }

        /// <summary>
        /// Data for the MaximumSizeGetSet test
        /// </summary>
        public static TheoryData<Size> MaximumSizeGetSetData =>
            TestHelper.GetSizeTheoryData();

        [Theory]
        [MemberData(nameof(MaximumSizeGetSetData))]
        public void MaximumSizeGetSet(Size expected)
        {
            // arrange
            var cont = new Control();

            // act
            cont.MaximumSize = expected;

            // assert
            Assert.Equal(expected, cont.MaximumSize);
        }

        /// <summary>
        /// Data for the MinimumSizeGetSet test
        /// </summary>
        public static TheoryData<Size> MinimumSizeGetSetData =>
            TestHelper.GetSizeTheoryData();

        [Theory]
        [MemberData(nameof(MinimumSizeGetSetData))]
        public void MinimumSizeGetSet(Size expected)
        {
            // arrange
            var cont = new Control();

            // act
            cont.MinimumSize = expected;

            // assert
            Assert.Equal(expected, cont.MinimumSize);
        }

        /// <summary>
        /// Data for the NameGetSet test
        /// </summary>
        public static TheoryData<string> NameGetSetData =>
            TestHelper.GetStringTheoryData();

        [Theory]
        [MemberData(nameof(NameGetSetData))]
        public void NameGetSet(string expected)
        {
            // arrange
            var cont = new Control();

            // act
            cont.Name = expected;

            // assert
            Assert.Equal(expected, cont.Name);
        }

        [Fact]
        public void ParentGetSet()
        {
            // arrange
            var parent = new Control();
            var cont = new Control();

            // act
            cont.Parent = parent;

            // assert
            Assert.NotNull(cont.Parent);
            Assert.Equal(parent, cont.Parent);
        }

        [Fact]
        public void ParentInternalGetSet()
        {
            // arrange
            var parent = new Control();
            var cont = new Control();

            // act
            cont.ParentInternal = parent;

            // assert
            Assert.NotNull(cont.Parent);
            Assert.Equal(parent, cont.ParentInternal);
            Assert.True(parent.Controls.Contains(cont));
        }

        [Fact]
        public void RegionGetSet()
        {
            var cont = new Control();
            var expected = new Region(new Rectangle(1,1,20,20));

            // act
            cont.Region = expected;

            // assert
            Assert.Equal(expected, cont.Region);
        }

        [Fact]
        public void RegionChanged()
        {
            bool wasChanged = false;
            var cont = new Control();
            cont.RegionChanged += (sender, args) => wasChanged = true;

            // act
            cont.Region = new Region(new Rectangle(1, 1, 20, 20));

            // assert
            Assert.True(wasChanged);
        }

        /// <summary>
        /// Data for the RequiredScaling test
        /// </summary>
        public static TheoryData<BoundsSpecified> RequiredScalingData =>
            TestHelper.GetEnumTheoryData<BoundsSpecified>();

        [Theory]
        [MemberData(nameof(RequiredScalingData))]
        public void RequiredScaling(BoundsSpecified expected)
        {
            // arrange
            var cont = new Control();

            // act
            cont.RequiredScaling = expected;

            // assert
            Assert.Equal(expected, cont.RequiredScaling);
        }

        /// <summary>
        /// Data for the RequiredScalingEnabled test
        /// </summary>
        public static TheoryData<bool> RequiredScalingEnabledData =>
            TestHelper.GetBoolTheoryData();

        [Theory]
        [MemberData(nameof(RequiredScalingEnabledData))]
        public void RequiredScalingEnabled(bool expected)
        {
            // arrange
            var cont = new Control();

            // act
            cont.RequiredScalingEnabled = expected;

            // assert
            Assert.Equal(expected, cont.RequiredScalingEnabled);
        }

        /// <summary>
        /// Data for the RightToLeft test
        /// </summary>
        public static TheoryData<RightToLeft> RightToLeftData =>
            TestHelper.GetEnumTheoryData<RightToLeft>();

        [Theory]
        [InlineData(RightToLeft.Yes)]
        [InlineData(RightToLeft.No)]
        public void RightToLeftTest(RightToLeft expected)
        {
            // arrange
            var cont = new Control();

            // act
            cont.RightToLeft = expected;

            // assert
            Assert.Equal(expected, cont.RightToLeft);
        }

        [Fact]
        public void RightToLeftInherit()
        {
            // arrange
            var parent = new Control();
            var cont = new Control();
            cont.Parent = parent;

            // act
            parent.RightToLeft = RightToLeft.Yes;
            cont.RightToLeft = RightToLeft.Inherit;

            // assert
            Assert.Equal(RightToLeft.Yes, cont.RightToLeft);
        }

        /// <summary>
        /// Data for the RightToLeftInvalid test
        /// </summary>
        public static TheoryData<RightToLeft> RightToLeftInvalidData =>
            TestHelper.GetEnumTheoryDataInvalid<RightToLeft>();

        [Theory]
        [MemberData(nameof(RightToLeftInvalidData))]
        public void RightToLeftInvalid(RightToLeft expected)
        {
            // arrange
            var cont = new Control();

            // act & assert
            var ex = Assert.Throws<InvalidEnumArgumentException>(() => cont.RightToLeft = expected);
            Assert.Equal("RightToLeft", ex.ParamName);
        }
    }
}
