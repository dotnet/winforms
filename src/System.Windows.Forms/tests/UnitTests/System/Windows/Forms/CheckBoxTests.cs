// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms.TestUtilities;
using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;
using Point = System.Drawing.Point;
using Size = System.Drawing.Size;

namespace System.Windows.Forms.Tests;

public class CheckBoxTests : AbstractButtonBaseTests
{
    [WinFormsFact]
    public void CheckBox_Ctor_Default()
    {
        using SubCheckBox control = new();
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
        using SubCheckBox control = new();
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

    [WinFormsTheory]
    [EnumData<Appearance>]
    public void CheckBox_Appearance_Set_GetReturnsExpected(Appearance value)
    {
        using CheckBox control = new()
        {
            Appearance = value
        };
        Assert.Equal(value, control.Appearance);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.Appearance = value;
        Assert.Equal(value, control.Appearance);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InvalidEnumData<Appearance>]
    public void CheckBox_Appearance_SetInvalidValue_ThrowsInvalidEnumArgumentException(Appearance value)
    {
        using CheckBox control = new();
        Assert.Throws<InvalidEnumArgumentException>("value", () => control.Appearance = value);
    }

    [WinFormsTheory]
    [BoolData]
    public void CheckBox_AutoCheck(bool expected)
    {
        using CheckBox box = new()
        {
            AutoCheck = expected
        };

        Assert.Equal(expected, box.AutoCheck);
    }

    [WinFormsTheory]
    [EnumData<ContentAlignment>]
    public void CheckBox_CheckAlign_Set_GetReturnsExpected(ContentAlignment value)
    {
        using CheckBox control = new()
        {
            CheckAlign = value
        };
        Assert.Equal(value, control.CheckAlign);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.CheckAlign = value;
        Assert.Equal(value, control.CheckAlign);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InvalidEnumData<ContentAlignment>]
    public void CheckBox_CheckAlign_SetInvalidValue_ThrowsInvalidEnumArgumentException(ContentAlignment value)
    {
        using CheckBox control = new();
        Assert.Throws<InvalidEnumArgumentException>("value", () => control.CheckAlign = value);
    }

    [WinFormsTheory]
    [InlineData(true, CheckState.Checked)]
    [InlineData(false, CheckState.Unchecked)]
    public void CheckBox_CheckedGetSet(bool sent, CheckState expected)
    {
        using CheckBox box = new()
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
        using SubCheckBox box = new()
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
        using SubCheckBox box = new()
        {
            AutoCheck = false,
            ThreeState = threeState,
            CheckState = expectedCheckState
        };

        box.OnClick(EventArgs.Empty);

        Assert.Equal(expectedChecked, box.Checked);
        Assert.Equal(expectedCheckState, box.CheckState);
    }

    [WinFormsTheory]
    [EnumData<CheckState>]
    public void CheckBox_CheckState_Set_GetReturnsExpected(CheckState value)
    {
        using CheckBox control = new()
        {
            CheckState = value
        };
        Assert.Equal(value, control.CheckState);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.CheckState = value;
        Assert.Equal(value, control.CheckState);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InvalidEnumData<CheckState>]
    public void CheckBox_CheckState_SetInvalidValue_ThrowsInvalidEnumArgumentException(CheckState value)
    {
        using CheckBox control = new();
        Assert.Throws<InvalidEnumArgumentException>("value", () => control.CheckState = value);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void CheckBox_OnCheckedChanged_Invoke_CallsCheckedChanged(EventArgs eventArgs)
    {
        using SubCheckBox control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            sender.Should().BeSameAs(control);
            eventArgs.Should().BeSameAs(e);
            callCount++;
        };

        // Call with handler.
        control.CheckedChanged += handler;
        control.OnCheckedChanged(eventArgs);
        callCount.Should().Be(1);

        // Remove handler.
        control.CheckedChanged -= handler;
        control.OnCheckedChanged(eventArgs);
        callCount.Should().Be(1);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void CheckBox_OnCheckedChanged_InvokeWithHandle_CallsCheckedChanged(EventArgs eventArgs)
    {
        using SubCheckBox control = new();
        control.Handle.Should().NotBe(IntPtr.Zero);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            sender.Should().BeSameAs(control);
            eventArgs.Should().BeSameAs(e);
            callCount++;
        };

        // Call with handler.
        control.CheckedChanged += handler;
        control.OnCheckedChanged(eventArgs);
        callCount.Should().Be(1);
        control.Handle.Should().NotBe(IntPtr.Zero);
        control.IsHandleCreated.Should().BeTrue();
        invalidatedCallCount.Should().Be(0);
        styleChangedCallCount.Should().Be(0);
        createdCallCount.Should().Be(0);

        // Remove handler.
        control.CheckedChanged -= handler;
        control.OnCheckedChanged(eventArgs);
        callCount.Should().Be(1);
        control.Handle.Should().NotBe(IntPtr.Zero);
        control.IsHandleCreated.Should().BeTrue();
        invalidatedCallCount.Should().Be(0);
        styleChangedCallCount.Should().Be(0);
        createdCallCount.Should().Be(0);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void CheckBox_OnCheckStateChanged_Invoke_CallsCheckStateChanged(EventArgs eventArgs)
    {
        using SubCheckBox control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            sender.Should().BeSameAs(control);
            eventArgs.Should().BeSameAs(e);
            callCount++;
        };

        // Call with handler.
        control.CheckStateChanged += handler;
        control.OnCheckStateChanged(eventArgs);
        callCount.Should().Be(1);

        // Remove handler.
        control.CheckStateChanged -= handler;
        control.OnCheckStateChanged(eventArgs);
        callCount.Should().Be(1);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void CheckBox_OnCheckStateChanged_InvokeWithHandle_CallsCheckStateChanged(EventArgs eventArgs)
    {
        using SubCheckBox control = new();
        control.Handle.Should().NotBe(IntPtr.Zero);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            sender.Should().BeSameAs(control);
            eventArgs.Should().BeSameAs(e);
            callCount++;
        };

        // Call with handler.
        control.CheckStateChanged += handler;
        control.OnCheckStateChanged(eventArgs);
        callCount.Should().Be(1);
        control.Handle.Should().NotBe(IntPtr.Zero);
        control.IsHandleCreated.Should().BeTrue();
        invalidatedCallCount.Should().Be(1);
        styleChangedCallCount.Should().Be(0);
        createdCallCount.Should().Be(0);

        // Remove handler.
        control.CheckStateChanged -= handler;
        control.OnCheckStateChanged(eventArgs);
        callCount.Should().Be(1);
        control.Handle.Should().NotBe(IntPtr.Zero);
        control.IsHandleCreated.Should().BeTrue();
        invalidatedCallCount.Should().Be(2);
        styleChangedCallCount.Should().Be(0);
        createdCallCount.Should().Be(0);
    }

    [WinFormsTheory]
    [EnumData<ContentAlignment>]
    public void CheckBox_TextAlign_Set_GetReturnsExpected(ContentAlignment value)
    {
        using CheckBox control = new()
        {
            TextAlign = value
        };
        Assert.Equal(value, control.TextAlign);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.TextAlign = value;
        Assert.Equal(value, control.TextAlign);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InvalidEnumData<ContentAlignment>]
    public void CheckBox_TextAlign_SetInvalidValue_ThrowsInvalidEnumArgumentException(ContentAlignment value)
    {
        using CheckBox control = new();
        Assert.Throws<InvalidEnumArgumentException>("value", () => control.TextAlign = value);
    }

    [WinFormsTheory]
    [BoolData]
    public void CheckBox_ThreeState(bool expected)
    {
        using CheckBox box = new()
        {
            ThreeState = expected
        };

        Assert.Equal(expected, box.ThreeState);
    }

    [WinFormsFact]
    public void CheckBox_CreateFlatAdapter()
    {
        using CheckBox box = new();

        ButtonInternal.ButtonBaseAdapter buttonBaseAdptr = box.CreateFlatAdapter();

        Assert.NotNull(buttonBaseAdptr);
    }

    [WinFormsFact]
    public void CheckBox_CreatePopupAdapter()
    {
        using CheckBox box = new();

        ButtonInternal.ButtonBaseAdapter checkBoxPopupAdptr = box.CreatePopupAdapter();

        Assert.NotNull(checkBoxPopupAdptr);
    }

    [WinFormsFact]
    public void CheckBox_CreateStandardAdapter()
    {
        using CheckBox box = new();

        ButtonInternal.ButtonBaseAdapter checkBoxSndAdptr = box.CreateStandardAdapter();

        Assert.NotNull(checkBoxSndAdptr);
    }

    [WinFormsFact]
    public void CheckBox_GetAutoSizeMode_Invoke_ReturnsExpected()
    {
        using SubCheckBox control = new();
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
        using SubCheckBox control = new();
        Assert.Equal(expected, control.GetStyle(flag));

        // Call again to test caching.
        Assert.Equal(expected, control.GetStyle(flag));
    }

    [WinFormsFact]
    public void CheckBox_GetTopLevel_Invoke_ReturnsExpected()
    {
        using SubCheckBox control = new();
        Assert.False(control.GetTopLevel());
    }

    [WinFormsFact]
    public void CheckBox_RaiseAutomationEvent_Invoke_Success()
    {
        using TestCheckBox checkBox = new();
        Assert.False(checkBox.IsHandleCreated);

        var accessibleObject = (SubCheckBoxAccessibleObject)checkBox.AccessibilityObject;
        Assert.Equal(0, accessibleObject.RaiseAutomationEventCallsCount);
        Assert.Equal(0, accessibleObject.RaiseAutomationPropertyChangedEventCallsCount);

        checkBox.Checked = true;

        Assert.Equal(1, accessibleObject.RaiseAutomationEventCallsCount);
        Assert.Equal(1, accessibleObject.RaiseAutomationPropertyChangedEventCallsCount);
        Assert.False(checkBox.IsHandleCreated);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void CheckBox_OnAppearanceChanged_Invoke_CallsDoubleClick(EventArgs eventArgs)
    {
        using SubCheckBox control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            sender.Should().BeSameAs(control);
            eventArgs.Should().BeSameAs(e);
            callCount++;
        };

        // Call with handler.
        control.AppearanceChanged += handler;
        control.OnAppearanceChanged(eventArgs);
        callCount.Should().Be(1);

        // Remove handler.
        control.AppearanceChanged -= handler;
        control.OnAppearanceChanged(eventArgs);
        callCount.Should().Be(1);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void CheckBox_OnDoubleClick_Invoke_CallsDoubleClick(EventArgs eventArgs)
    {
        using SubCheckBox control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            sender.Should().BeSameAs(control);
            eventArgs.Should().BeSameAs(e);
            callCount++;
        };

        // Call with handler.
        control.DoubleClick += handler;
        control.OnDoubleClick(eventArgs);
        callCount.Should().Be(1);

        // Remove handler.
        control.DoubleClick -= handler;
        control.OnDoubleClick(eventArgs);
        callCount.Should().Be(1);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetMouseEventArgsTheoryData))]
    public void CheckBox_OnMouseDoubleClick_Invoke_CallsMouseDoubleClick(MouseEventArgs eventArgs)
    {
        using SubCheckBox control = new();
        int callCount = 0;
        MouseEventHandler handler = (sender, e) =>
        {
            sender.Should().BeSameAs(control);
            eventArgs.Should().BeSameAs(e);
            callCount++;
        };

        // Call with handler.
        control.MouseDoubleClick += handler;
        control.OnMouseDoubleClick(eventArgs);
        callCount.Should().Be(1);

        // Remove handler.
        control.MouseDoubleClick -= handler;
        control.OnMouseDoubleClick(eventArgs);
        callCount.Should().Be(1);
    }

    // the zero here may be an issue with cultural variance
    [WinFormsFact]
    public void CheckBox_ToStringTest()
    {
        using CheckBox box = new();
        string expected = "System.Windows.Forms.CheckBox, CheckState: 0";

        string actual = box.ToString();

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

        public new void OnAppearanceChanged(EventArgs e) => base.OnAppearanceChanged(e);

        public new void OnClick(EventArgs e) => base.OnClick(e);

        public new void OnDoubleClick(EventArgs e) => base.OnDoubleClick(e);

        public new void OnMouseDoubleClick(MouseEventArgs e) => base.OnMouseDoubleClick(e);

        public new void OnMouseUp(MouseEventArgs e) => base.OnMouseUp(e);

        public new void OnCheckedChanged(EventArgs e) => base.OnCheckedChanged(e);

        public new void OnCheckStateChanged(EventArgs e) => base.OnCheckStateChanged(e);

        internal new void OnMouseClick(MouseEventArgs e) => base.OnMouseClick(e);

        internal new void OnMouseDown(MouseEventArgs e) => base.OnMouseDown(e);
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

        internal override bool RaiseAutomationEvent(UIA_EVENT_ID eventId)
        {
            RaiseAutomationEventCallsCount++;
            return base.RaiseAutomationEvent(eventId);
        }

        internal override bool RaiseAutomationPropertyChangedEvent(UIA_PROPERTY_ID propertyId, VARIANT oldValue, VARIANT newValue)
        {
            RaiseAutomationPropertyChangedEventCallsCount++;
            return base.RaiseAutomationPropertyChangedEvent(propertyId, oldValue, newValue);
        }
    }

    [WinFormsFact]
    public void CheckBox_CheckedChangedEvent_Raised()
    {
        using CheckBox checkBox = (CheckBox)CreateButton();
        bool eventFired = false;

        checkBox.CheckedChanged += (sender, args) => eventFired = true;
        checkBox.Checked = !checkBox.Checked;

        eventFired.Should().BeTrue();
    }

    [WinFormsFact]
    public void CheckBox_CheckStateChangedEvent_Raised()
    {
        using CheckBox checkBox = (CheckBox)CreateButton();
        bool eventFired = false;

        checkBox.CheckStateChanged += (sender, args) => eventFired = true;
        checkBox.CheckState = checkBox.CheckState == CheckState.Checked ? CheckState.Unchecked : CheckState.Checked;

        eventFired.Should().BeTrue();
    }

    public static IEnumerable<object[]> Appearance_FlatStyle_TestData()
    {
        yield return new object[] { Appearance.Button, FlatStyle.Standard };
        yield return new object[] { Appearance.Button, FlatStyle.Flat };
        yield return new object[] { Appearance.Button, FlatStyle.Popup };
        yield return new object[] { Appearance.Button, FlatStyle.System };
        yield return new object[] { Appearance.Normal, FlatStyle.Standard };
        yield return new object[] { Appearance.Normal, FlatStyle.Flat };
        yield return new object[] { Appearance.Normal, FlatStyle.Popup };
        yield return new object[] { Appearance.Normal, FlatStyle.System };
    }

    [WinFormsTheory]
    [MemberData(nameof(Appearance_FlatStyle_TestData))]
    public void CheckBox_OverChangeRectangle_Get(Appearance appearance, FlatStyle flatStyle) => base.ButtonBase_OverChangeRectangle_Get(appearance, flatStyle);

    [WinFormsTheory]
    [MemberData(nameof(Appearance_FlatStyle_TestData))]
    public void CheckBox_DownChangeRectangle_ReturnsExpectedRectangle(Appearance appearance, FlatStyle flatStyle)
    {
        CheckBox checkBox = (CheckBox)CreateButton();
        checkBox.Appearance = appearance;
        checkBox.FlatStyle = flatStyle;

        Rectangle downChangeRectangle = checkBox.DownChangeRectangle;

        if (appearance == Appearance.Button || flatStyle == FlatStyle.System)
        {
            downChangeRectangle.Should().Be(checkBox.ClientRectangle);
        }
        else
        {
            downChangeRectangle.Should().Be(checkBox.Adapter.CommonLayout().Layout().CheckBounds);
        }
    }

    [WinFormsTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void CheckBox_LeftClick_MouseUpCounts(bool capture)
    {
        using Form form = new();
        using SubCheckBox control = (SubCheckBox)CreateButton();
        control.Capture = capture;
        form.Controls.Add(control);
        control.TabIndex = 9999;
        form.Show();

        MouseEventArgs eventArgs = new(MouseButtons.Left, 1, new Point(0, 0), 0);

        int callCountOnMouseUp = 0;

        control.MouseUp += (sender, e) =>
        {
            sender.Should().Be(control);
            e.Should().Be(eventArgs);
            callCountOnMouseUp++;
        };

        control.OnMouseUp(eventArgs);
        callCountOnMouseUp.Should().Be(1);
    }

    [WinFormsTheory]
    [InlineData(true, '&', "&MnemonicText")]
    [InlineData(true, 'N', "NonMnemonicText")]
    [InlineData(true, 'M', "&MnemonicText")]
    [InlineData(false, 'M', "&MnemonicText")]
    public void CheckBox_ProcessMnemonic_ValidCases(bool useMnemonic, char charCode, string buttonText)
    {
        // Arrange
        using Form form = new();
        using SubCheckBox checkBox = new()
        {
            UseMnemonic = useMnemonic,
            Text = buttonText,
        };
        form.Controls.Add(checkBox);
        form.Show();

        // Act
        bool result = checkBox.ProcessMnemonic(charCode);

        // Assert
        // Requirements for SUT to process mnemonic
        bool requirements =
            useMnemonic
                && charCode != '&'
                && buttonText.Contains($"&{charCode}", StringComparison.OrdinalIgnoreCase);

        if (!requirements)
        {
            return;
        }

        result.Should().BeTrue();
        checkBox.Focused.Should().BeTrue();
        checkBox.CheckState.Should().Be(CheckState.Checked);
    }

    protected override ButtonBase CreateButton() => new SubCheckBox();
}
