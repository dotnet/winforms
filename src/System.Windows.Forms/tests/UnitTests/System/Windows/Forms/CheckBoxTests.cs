// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Drawing;
using WinForms.Common.Tests;
using Xunit;

namespace System.Windows.Forms.Tests
{
    using static Interop.UiaCore;
    using Point = System.Drawing.Point;
    using Size = System.Drawing.Size;

    public class CheckBoxTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void CheckBox_Ctor_Default()
        {
            using var control = new SubCheckBox();
            Assert.Null(control.AccessibleDefaultActionDescription);
            Assert.Null(control.AccessibleDescription);
            Assert.Null(control.AccessibleName);
            Assert.Equal(AccessibleRole.Default, control.AccessibleRole);
            Assert.False(control.AllowDrop);
            Assert.Equal(Appearance.Normal, control.Appearance);
            Assert.Equal(AnchorStyles.Top | AnchorStyles.Left, control.Anchor);
            Assert.True(control.AutoCheck);
            Assert.False(control.AutoEllipsis);
            Assert.False(control.AutoSize);
            Assert.Equal(Control.DefaultBackColor, control.BackColor);
            Assert.Null(control.BackgroundImage);
            Assert.Equal(ImageLayout.Tile, control.BackgroundImageLayout);
            Assert.Null(control.BindingContext);
            Assert.Equal(24, control.Bottom);
            Assert.Equal(new Rectangle(0, 0, 104, 24), control.Bounds);
            Assert.False(control.CanEnableIme);
            Assert.False(control.CanFocus);
            Assert.True(control.CanRaiseEvents);
            Assert.True(control.CanSelect);
            Assert.False(control.Capture);
            Assert.True(control.CausesValidation);
            Assert.Equal(ContentAlignment.MiddleLeft, control.CheckAlign);
            Assert.False(control.Checked);
            Assert.Equal(CheckState.Unchecked, control.CheckState);
            Assert.Equal(new Size(104, 24), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, 104, 24), control.ClientRectangle);
            Assert.Null(control.Container);
            Assert.False(control.ContainsFocus);
            Assert.Null(control.ContextMenuStrip);
            Assert.Empty(control.Controls);
            Assert.Same(control.Controls, control.Controls);
            Assert.False(control.Created);
            Assert.Same(Cursors.Default, control.Cursor);
            Assert.Same(Cursors.Default, control.DefaultCursor);
            Assert.Equal(ImeMode.Disable, control.DefaultImeMode);
            Assert.Equal(new Padding(3), control.DefaultMargin);
            Assert.Equal(Size.Empty, control.DefaultMaximumSize);
            Assert.Equal(Size.Empty, control.DefaultMinimumSize);
            Assert.Equal(Padding.Empty, control.DefaultPadding);
            Assert.Equal(new Size(104, 24), control.DefaultSize);
            Assert.False(control.DesignMode);
            Assert.Equal(new Rectangle(0, 0, 104, 24), control.DisplayRectangle);
            Assert.Equal(DockStyle.None, control.Dock);
            Assert.True(control.DoubleBuffered);
            Assert.True(control.Enabled);
            Assert.NotNull(control.Events);
            Assert.Same(control.Events, control.Events);
            Assert.NotNull(control.FlatAppearance);
            Assert.Same(control.FlatAppearance, control.FlatAppearance);
            Assert.Equal(FlatStyle.Standard, control.FlatStyle);
            Assert.False(control.Focused);
            Assert.Equal(Control.DefaultFont, control.Font);
            Assert.Equal(control.Font.Height, control.FontHeight);
            Assert.Equal(Control.DefaultForeColor, control.ForeColor);
            Assert.False(control.HasChildren);
            Assert.Equal(24, control.Height);
            Assert.Null(control.Image);
            Assert.Equal(ContentAlignment.MiddleCenter, control.ImageAlign);
            Assert.Equal(-1, control.ImageIndex);
            Assert.Empty(control.ImageKey);
            Assert.Null(control.ImageList);
            Assert.Equal(ImeMode.Disable, control.ImeMode);
            Assert.Equal(ImeMode.Disable, control.ImeModeBase);
            Assert.False(control.IsAccessible);
            Assert.False(control.IsDefault);
            Assert.False(control.IsMirrored);
            Assert.NotNull(control.LayoutEngine);
            Assert.Same(control.LayoutEngine, control.LayoutEngine);
            Assert.Equal(0, control.Left);
            Assert.Equal(Point.Empty, control.Location);
            Assert.Equal(new Padding(3), control.Margin);
            Assert.Equal(Size.Empty, control.MaximumSize);
            Assert.Equal(Size.Empty, control.MinimumSize);
            Assert.Equal(Padding.Empty, control.Padding);
            Assert.Null(control.Parent);
            Assert.Equal("Microsoft\u00AE .NET", control.ProductName);
            Assert.True(control.PreferredSize.Width > 0);
            Assert.True(control.PreferredSize.Height > 0);
            Assert.False(control.RecreatingHandle);
            Assert.Null(control.Region);
            Assert.True(control.ResizeRedraw);
            Assert.Equal(104, control.Right);
            Assert.Equal(RightToLeft.No, control.RightToLeft);
            Assert.True(control.ShowFocusCues);
            Assert.True(control.ShowKeyboardCues);
            Assert.Null(control.Site);
            Assert.Equal(new Size(104, 24), control.Size);
            Assert.Equal(0, control.TabIndex);
            Assert.True(control.TabStop);
            Assert.Empty(control.Text);
            Assert.Equal(ContentAlignment.MiddleLeft, control.TextAlign);
            Assert.Equal(TextImageRelation.Overlay, control.TextImageRelation);
            Assert.False(control.ThreeState);
            Assert.Equal(0, control.Top);
            Assert.Null(control.TopLevelControl);
            Assert.True(control.UseMnemonic);
            Assert.True(control.UseCompatibleTextRendering);
            Assert.True(control.UseVisualStyleBackColor);
            Assert.False(control.UseWaitCursor);
            Assert.True(control.Visible);
            Assert.Equal(104, control.Width);

            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void CheckBox_CreateParams_GetDefault_ReturnsExpected()
        {
            using var control = new SubCheckBox();
            CreateParams createParams = control.CreateParams;
            Assert.Null(createParams.Caption);
            Assert.Equal("Button", createParams.ClassName);
            Assert.Equal(0x8, createParams.ClassStyle);
            Assert.Equal(0, createParams.ExStyle);
            Assert.Equal(24, createParams.Height);
            Assert.Equal(IntPtr.Zero, createParams.Parent);
            Assert.Null(createParams.Param);
            Assert.Equal(0x5601000B, createParams.Style);
            Assert.Equal(104, createParams.Width);
            Assert.Equal(0, createParams.X);
            Assert.Equal(0, createParams.Y);
            Assert.Same(createParams, control.CreateParams);
            Assert.False(control.IsHandleCreated);
        }

        /// <summary>
        ///  Data for the AppearanceGetSet test
        /// </summary>
        public static TheoryData<Appearance> AppearanceGetSetData =>
            CommonTestHelper.GetEnumTheoryData<Appearance>();

        [WinFormsTheory]
        [MemberData(nameof(AppearanceGetSetData))]
        public void CheckBox_AutoSizeModeGetSet(Appearance expected)
        {
            using var box = new CheckBox
            {
                Appearance = expected
            };

            Assert.Equal(expected, box.Appearance);
        }

        /// <summary>
        ///  Data for the AppearanceGetSetInvalid test
        /// </summary>
        public static TheoryData<Appearance> AppearanceGetSetInvalidData =>
            CommonTestHelper.GetEnumTheoryDataInvalid<Appearance>();

        [WinFormsTheory]
        [MemberData(nameof(AppearanceGetSetInvalidData))]
        public void CheckBox_AppearanceGetSetInvalid(Appearance expected)
        {
            using var box = new CheckBox();

            InvalidEnumArgumentException ex = Assert.Throws<InvalidEnumArgumentException>(() => box.Appearance = expected);
            Assert.Equal("value", ex.ParamName);
        }

        /// <summary>
        ///  Data for the AutoCheck test
        /// </summary>
        public static TheoryData<bool> AutoCheckData =>
            CommonTestHelper.GetBoolTheoryData();

        [WinFormsTheory]
        [MemberData(nameof(AutoCheckData))]
        public void CheckBox_AutoCheck(bool expected)
        {
            using var box = new CheckBox
            {
                AutoCheck = expected
            };

            Assert.Equal(expected, box.AutoCheck);
        }

        /// <summary>
        ///  Data for the ContentAlignmentGetSet test
        /// </summary>
        public static TheoryData<ContentAlignment> ContentAlignmentGetSetData =>
            CommonTestHelper.GetEnumTheoryData<ContentAlignment>();

        [WinFormsTheory]
        [MemberData(nameof(ContentAlignmentGetSetData))]
        public void CheckBox_ContentAlignmentGetSet(ContentAlignment expected)
        {
            using var box = new CheckBox
            {
                CheckAlign = expected,
                TextAlign = expected
            };

            Assert.Equal(expected, box.CheckAlign);
            Assert.Equal(expected, box.TextAlign);
        }

        /// <summary>
        ///  Data for the ContentAlignmentGetSetInvalid test
        /// </summary>
        public static TheoryData<ContentAlignment> ContentAlignmentGetSetInvalidData =>
            CommonTestHelper.GetEnumTheoryDataInvalid<ContentAlignment>();

        [WinFormsTheory]
        [MemberData(nameof(ContentAlignmentGetSetInvalidData))]
        public void CheckBox_ContentAlignmentGetSetInvalid(ContentAlignment expected)
        {
            using var box = new CheckBox();

            InvalidEnumArgumentException ex = Assert.Throws<InvalidEnumArgumentException>(() => box.CheckAlign = expected);
            Assert.Equal("value", ex.ParamName);
        }

        [WinFormsTheory]
        [InlineData(true, CheckState.Checked)]
        [InlineData(false, CheckState.Unchecked)]
        public void CheckBox_CheckedGetSet(bool sent, CheckState expected)
        {
            using var box = new CheckBox
            {
                Checked = sent
            };

            Assert.Equal(expected, box.CheckState);
        }

        [WinFormsTheory]
        [InlineData(true, CheckState.Checked, true, CheckState.Indeterminate)]
        [InlineData(true, CheckState.Unchecked, true, CheckState.Checked)]
        [InlineData(true, CheckState.Indeterminate, false, CheckState.Unchecked)]
        [InlineData(false, CheckState.Checked, false, CheckState.Unchecked)]
        [InlineData(false, CheckState.Unchecked, true, CheckState.Checked)]
        [InlineData(false, CheckState.Indeterminate, false, CheckState.Unchecked)]
        public void CheckBox_OnClick_AutoCheck_SetCorrectCheckState(bool threeState, CheckState checkState, bool expectedChecked, CheckState expectedCheckState)
        {
            using var box = new SubCheckBox
            {
                AutoCheck = true,
                ThreeState = threeState,
                CheckState = checkState
            };

            box.OnClick(EventArgs.Empty);

            Assert.Equal(expectedChecked, box.Checked);
            Assert.Equal(expectedCheckState, box.CheckState);
        }

        [WinFormsTheory]
        [InlineData(true, CheckState.Checked, true)]
        [InlineData(true, CheckState.Unchecked, false)]
        [InlineData(true, CheckState.Indeterminate, true)]
        [InlineData(false, CheckState.Checked, true)]
        [InlineData(false, CheckState.Unchecked, false)]
        [InlineData(false, CheckState.Indeterminate, true)]
        public void CheckBox_OnClick_AutoCheckFalse_DoesNotChangeCheckState(bool threeState, CheckState expectedCheckState, bool expectedChecked)
        {
            using var box = new SubCheckBox
            {
                AutoCheck = false,
                ThreeState = threeState,
                CheckState = expectedCheckState
            };

            box.OnClick(EventArgs.Empty);

            Assert.Equal(expectedChecked, box.Checked);
            Assert.Equal(expectedCheckState, box.CheckState);
        }

        /// <summary>
        ///  Data for the CheckStateGetSet test
        /// </summary>
        public static TheoryData<CheckState> CheckStateGetSetData =>
            CommonTestHelper.GetEnumTheoryData<CheckState>();

        [WinFormsTheory]
        [MemberData(nameof(CheckStateGetSetData))]
        public void CheckBox_CheckStateGetSet(CheckState expected)
        {
            using var box = new CheckBox
            {
                CheckState = expected
            };

            Assert.Equal(expected, box.CheckState);
        }

        /// <summary>
        ///  Data for the CheckStateGetSetInvalid test
        /// </summary>
        public static TheoryData<CheckState> CheckStateGetSetInvalidData =>
            CommonTestHelper.GetEnumTheoryDataInvalid<CheckState>();

        [WinFormsTheory]
        [MemberData(nameof(CheckStateGetSetInvalidData))]
        public void CheckBox_CheckStateGetSetInvalid(CheckState expected)
        {
            using var box = new CheckBox();

            InvalidEnumArgumentException ex = Assert.Throws<InvalidEnumArgumentException>(() => box.CheckState = expected);
            Assert.Equal("value", ex.ParamName);
        }

        /// <summary>
        ///  Data for the ThreeState test
        /// </summary>
        public static TheoryData<bool> ThreeStateData =>
            CommonTestHelper.GetBoolTheoryData();

        [WinFormsTheory]
        [MemberData(nameof(ThreeStateData))]
        public void CheckBox_ThreeState(bool expected)
        {
            using var box = new CheckBox
            {
                ThreeState = expected
            };

            Assert.Equal(expected, box.ThreeState);
        }

        [WinFormsFact]
        public void CheckBox_CreateFlatAdapter()
        {
            using var box = new CheckBox();

            ButtonInternal.ButtonBaseAdapter buttonBaseAdptr = box.CreateFlatAdapter();

            Assert.NotNull(buttonBaseAdptr);
        }

        [WinFormsFact]
        public void CheckBox_CreatePopupAdapter()
        {
            using var box = new CheckBox();

            ButtonInternal.ButtonBaseAdapter checkBoxPopupAdptr = box.CreatePopupAdapter();

            Assert.NotNull(checkBoxPopupAdptr);
        }

        [WinFormsFact]
        public void CheckBox_CreateStandardAdapter()
        {
            using var box = new CheckBox();

            ButtonInternal.ButtonBaseAdapter checkBoxSndAdptr = box.CreateStandardAdapter();

            Assert.NotNull(checkBoxSndAdptr);
        }

        [WinFormsFact]
        public void CheckBox_GetAutoSizeMode_Invoke_ReturnsExpected()
        {
            using var control = new SubCheckBox();
            Assert.Equal(AutoSizeMode.GrowAndShrink, control.GetAutoSizeMode());
        }

        [WinFormsTheory]
        [InlineData(ControlStyles.ContainerControl, false)]
        [InlineData(ControlStyles.UserPaint, true)]
        [InlineData(ControlStyles.Opaque, true)]
        [InlineData(ControlStyles.ResizeRedraw, true)]
        [InlineData(ControlStyles.FixedWidth, false)]
        [InlineData(ControlStyles.FixedHeight, false)]
        [InlineData(ControlStyles.StandardClick, false)]
        [InlineData(ControlStyles.Selectable, true)]
        [InlineData(ControlStyles.UserMouse, true)]
        [InlineData(ControlStyles.SupportsTransparentBackColor, true)]
        [InlineData(ControlStyles.StandardDoubleClick, false)]
        [InlineData(ControlStyles.AllPaintingInWmPaint, true)]
        [InlineData(ControlStyles.CacheText, true)]
        [InlineData(ControlStyles.EnableNotifyMessage, false)]
        [InlineData(ControlStyles.DoubleBuffer, false)]
        [InlineData(ControlStyles.OptimizedDoubleBuffer, true)]
        [InlineData(ControlStyles.UseTextForAccessibility, true)]
        [InlineData((ControlStyles)0, true)]
        [InlineData((ControlStyles)int.MaxValue, false)]
        [InlineData((ControlStyles)(-1), false)]
        public void CheckBox_GetStyle_Invoke_ReturnsExpected(ControlStyles flag, bool expected)
        {
            using var control = new SubCheckBox();
            Assert.Equal(expected, control.GetStyle(flag));

            // Call again to test caching.
            Assert.Equal(expected, control.GetStyle(flag));
        }

        [WinFormsFact]
        public void CheckBox_GetTopLevel_Invoke_ReturnsExpected()
        {
            using var control = new SubCheckBox();
            Assert.False(control.GetTopLevel());
        }

        [WinFormsFact]
        public void CheckBox_RaiseAutomationEvent_Invoke_Success()
        {
            using var checkBox = new TestCheckBox();
            Assert.False(checkBox.IsHandleCreated);

            var accessibleObject = (SubCheckBoxAccessibleObject)checkBox.AccessibilityObject;
            Assert.Equal(0, accessibleObject.RaiseAutomationEventCallsCount);
            Assert.Equal(0, accessibleObject.RaiseAutomationPropertyChangedEventCallsCount);

            checkBox.Checked = true;

            Assert.Equal(1, accessibleObject.RaiseAutomationEventCallsCount);
            Assert.Equal(1, accessibleObject.RaiseAutomationPropertyChangedEventCallsCount);
            Assert.False(checkBox.IsHandleCreated);
        }

        // the zero here may be an issue with cultural variance
        [WinFormsFact]
        public void CheckBox_ToStringTest()
        {
            using var box = new CheckBox();
            var expected = "System.Windows.Forms.CheckBox, CheckState: 0";

            var actual = box.ToString();

            Assert.Equal(expected, actual);
        }

        public class SubCheckBox : CheckBox
        {
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

            public new bool IsDefault
            {
                get => base.IsDefault;
                set => base.IsDefault = value;
            }

            public new bool ResizeRedraw
            {
                get => base.ResizeRedraw;
                set => base.ResizeRedraw = value;
            }

            public new bool ShowFocusCues => base.ShowFocusCues;

            public new bool ShowKeyboardCues => base.ShowKeyboardCues;

            public new AutoSizeMode GetAutoSizeMode() => base.GetAutoSizeMode();

            public new bool GetStyle(ControlStyles flag) => base.GetStyle(flag);

            public new bool GetTopLevel() => base.GetTopLevel();

            public new void OnClick(EventArgs e) => base.OnClick(e);
        }

        private class TestCheckBox : CheckBox
        {
            protected override AccessibleObject CreateAccessibilityInstance()
            {
                return new SubCheckBoxAccessibleObject(this);
            }
        }

        private class SubCheckBoxAccessibleObject : CheckBox.CheckBoxAccessibleObject
        {
            public SubCheckBoxAccessibleObject(CheckBox owner) : base(owner)
            {
                RaiseAutomationEventCallsCount = 0;
                RaiseAutomationPropertyChangedEventCallsCount = 0;
            }

            public int RaiseAutomationEventCallsCount { get; private set; }

            public int RaiseAutomationPropertyChangedEventCallsCount { get; private set; }

            internal override bool RaiseAutomationEvent(UIA eventId)
            {
                RaiseAutomationEventCallsCount++;
                return base.RaiseAutomationEvent(eventId);
            }

            internal override bool RaiseAutomationPropertyChangedEvent(UIA propertyId, object oldValue, object newValue)
            {
                RaiseAutomationPropertyChangedEventCallsCount++;
                return base.RaiseAutomationPropertyChangedEvent(propertyId, oldValue, newValue);
            }
        }
    }
}
