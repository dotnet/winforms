// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;
using Moq;
using Point = System.Drawing.Point;
using Size = System.Drawing.Size;

namespace System.Windows.Forms.Design.Tests;

public class ComponentEditorPageTests
{
    [WinFormsFact]
    public void ComponentEditorPagePanel_Ctor_Default()
    {
        using SubComponentEditorPage control = new();
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
        Assert.True(control.CanEnableIme);
        Assert.False(control.CanFocus);
        Assert.True(control.CanRaiseEvents);
        Assert.False(control.CanSelect);
        Assert.False(control.Capture);
        Assert.True(control.CausesValidation);
        Assert.Equal(new Rectangle(0, 0, 200, 100), control.ClientRectangle);
        Assert.Equal(new Size(200, 100), control.ClientSize);
        Assert.False(control.CommitOnDeactivate);
        Assert.Null(control.Component);
        Assert.Null(control.Container);
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
        Assert.Equal(new Size(200, 100), control.DefaultSize);
        Assert.False(control.DesignMode);
        Assert.Equal(new Rectangle(0, 0, 200, 100), control.DisplayRectangle);
        Assert.Equal(DockStyle.None, control.Dock);
        Assert.NotNull(control.DockPadding);
        Assert.Same(control.DockPadding, control.DockPadding);
        Assert.Equal(0, control.DockPadding.Top);
        Assert.Equal(0, control.DockPadding.Bottom);
        Assert.Equal(0, control.DockPadding.Left);
        Assert.Equal(0, control.DockPadding.Right);
        Assert.False(control.DoubleBuffered);
        Assert.True(control.Enabled);
        Assert.NotNull(control.Events);
        Assert.Same(control.Events, control.Events);
        Assert.True(control.FirstActivate);
        Assert.False(control.Focused);
        Assert.Equal(Control.DefaultFont, control.Font);
        Assert.Equal(control.Font.Height, control.FontHeight);
        Assert.Equal(Control.DefaultForeColor, control.ForeColor);
        Assert.False(control.HasChildren);
        Assert.Equal(100, control.Height);
        Assert.NotNull(control.HorizontalScroll);
        Assert.Same(control.HorizontalScroll, control.HorizontalScroll);
        Assert.False(control.HScroll);
        Assert.NotNull(control.Icon);
        Assert.Same(control.Icon, control.Icon);
        Assert.Equal(ImeMode.NoControl, control.ImeMode);
        Assert.Equal(ImeMode.NoControl, control.ImeModeBase);
        Assert.False(control.IsAccessible);
        Assert.False(control.IsMirrored);
        Assert.NotNull(control.LayoutEngine);
        Assert.Same(control.LayoutEngine, control.LayoutEngine);
        Assert.Equal(0, control.Left);
        Assert.Equal(0, control.Loading);
        Assert.False(control.LoadRequired);
        Assert.Equal(Point.Empty, control.Location);
        Assert.Equal(new Padding(3), control.Margin);
        Assert.Equal(Size.Empty, control.MaximumSize);
        Assert.Equal(Size.Empty, control.MinimumSize);
        Assert.Equal(Padding.Empty, control.Padding);
        Assert.Null(control.PageSite);
        Assert.Null(control.Parent);
        Assert.Equal(Size.Empty, control.PreferredSize);
        Assert.Equal("Microsoft\u00AE .NET", control.ProductName);
        Assert.False(control.RecreatingHandle);
        Assert.Null(control.Region);
        Assert.False(control.ResizeRedraw);
        Assert.Equal(200, control.Right);
        Assert.Equal(RightToLeft.No, control.RightToLeft);
        Assert.True(control.ShowFocusCues);
        Assert.True(control.ShowKeyboardCues);
        Assert.Equal(new Size(200, 100), control.Size);
        Assert.Null(control.Site);
        Assert.Equal(0, control.TabIndex);
        Assert.False(control.TabStop);
        Assert.Empty(control.Text);
        Assert.Empty(control.Title);
        Assert.Equal(0, control.Top);
        Assert.Null(control.TopLevelControl);
        Assert.False(control.UseWaitCursor);
        Assert.False(control.Visible);
        Assert.NotNull(control.VerticalScroll);
        Assert.Same(control.VerticalScroll, control.VerticalScroll);
        Assert.False(control.VScroll);
        Assert.Equal(200, control.Width);

        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void ComponentEditorPage_CreateParams_GetDefault_ReturnsExpected()
    {
        using SubComponentEditorPage control = new();
        CreateParams createParams = control.CreateParams;
        Assert.Null(createParams.Caption);
        Assert.Null(createParams.ClassName);
        Assert.Equal(0x8, createParams.ClassStyle);
        Assert.Equal(0x10000, createParams.ExStyle);
        Assert.Equal(100, createParams.Height);
        Assert.Equal(IntPtr.Zero, createParams.Parent);
        Assert.Null(createParams.Param);
        Assert.Equal(0x46000000, createParams.Style);
        Assert.Equal(200, createParams.Width);
        Assert.Equal(0, createParams.X);
        Assert.Equal(0, createParams.Y);
        Assert.Same(createParams, control.CreateParams);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [BoolData]
    public void ComponentEditorPage_AutoSize_Set_GetReturnsExpected(bool value)
    {
        using SubComponentEditorPage control = new();
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
    public void ComponentEditorPage_AutoSize_SetWithHandler_CallsAutoSizeChanged()
    {
        using SubComponentEditorPage control = new()
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
    [BoolData]
    public void ComponentEditorPage_CommitOnDeactivate_Set_GetReturnsExpected(bool value)
    {
        using SubComponentEditorPage control = new()
        {
            CommitOnDeactivate = value
        };
        Assert.Equal(value, control.CommitOnDeactivate);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.CommitOnDeactivate = value;
        Assert.Equal(value, control.CommitOnDeactivate);
        Assert.False(control.IsHandleCreated);

        // Set different.
        control.CommitOnDeactivate = value;
        Assert.Equal(value, control.CommitOnDeactivate);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> Component_Set_TestData()
    {
        yield return new object[] { null };
        Mock<IComponent> mockComponent = new(MockBehavior.Strict);
        mockComponent
            .Setup(c => c.Dispose());
        yield return new object[] { mockComponent.Object };
    }

    [WinFormsTheory]
    [MemberData(nameof(Component_Set_TestData))]
    public void ComponentEditorPage_Component_Set_GetReturnsExpected(IComponent value)
    {
        using SubComponentEditorPage control = new()
        {
            Component = value
        };
        Assert.Same(value, control.Component);
        Assert.Same(value, control.GetSelectedComponent());
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.Component = value;
        Assert.Same(value, control.Component);
        Assert.Same(value, control.GetSelectedComponent());
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [BoolData]
    public void ComponentEditorPage_FirstActivate_Set_GetReturnsExpected(bool value)
    {
        using SubComponentEditorPage control = new()
        {
            FirstActivate = value
        };
        Assert.Equal(value, control.FirstActivate);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.FirstActivate = value;
        Assert.Equal(value, control.FirstActivate);
        Assert.False(control.IsHandleCreated);

        // Set different.
        control.FirstActivate = value;
        Assert.Equal(value, control.FirstActivate);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void ComponentEditorPage_Icon_Set_GetReturnsExpected()
    {
        using var value = Icon.FromHandle(new Bitmap(10, 10).GetHicon());
        using SubComponentEditorPage control = new()
        {
            Icon = value
        };
        Assert.Same(value, control.Icon);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.Icon = value;
        Assert.Same(value, control.Icon);
        Assert.False(control.IsHandleCreated);

        // Set null.
        control.Icon = null;
        Assert.NotSame(value, control.Icon);
        Assert.NotNull(control.Icon);
        Assert.Same(control.Icon, control.Icon);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(-1, true)]
    [InlineData(0, false)]
    [InlineData(1, true)]
    public void ComponentEditorPage_Loading_Set_GetReturnsExpected(int value, bool expectedIsLoading)
    {
        using SubComponentEditorPage control = new()
        {
            Loading = value
        };
        Assert.Equal(value, control.Loading);
        Assert.Equal(expectedIsLoading, control.IsLoading());
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.Loading = value;
        Assert.Equal(value, control.Loading);
        Assert.Equal(expectedIsLoading, control.IsLoading());
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [BoolData]
    public void ComponentEditorPage_LoadRequired_Set_GetReturnsExpected(bool value)
    {
        using SubComponentEditorPage control = new()
        {
            LoadRequired = value
        };
        Assert.Equal(value, control.LoadRequired);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.LoadRequired = value;
        Assert.Equal(value, control.LoadRequired);
        Assert.False(control.IsHandleCreated);

        // Set different.
        control.LoadRequired = value;
        Assert.Equal(value, control.LoadRequired);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> PageSite_Set_TestData()
    {
        yield return new object[] { null };
        Mock<IComponentEditorPageSite> mockComponentEditorPageSite = new(MockBehavior.Strict);
        yield return new object[] { mockComponentEditorPageSite.Object };
    }

    [WinFormsTheory]
    [MemberData(nameof(PageSite_Set_TestData))]
    public void ComponentEditorPage_PageSite_Set_GetReturnsExpected(IComponentEditorPageSite value)
    {
        using SubComponentEditorPage control = new()
        {
            PageSite = value
        };
        Assert.Same(value, control.PageSite);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.PageSite = value;
        Assert.Same(value, control.PageSite);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [NormalizedStringData]
    public void ComponentEditorPage_Text_Set_GetReturnsExpected(string value, string expected)
    {
        using SubComponentEditorPage control = new()
        {
            Text = value
        };
        Assert.Equal(expected, control.Text);
        Assert.Equal(expected, control.Title);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.Text = value;
        Assert.Equal(expected, control.Text);
        Assert.Equal(expected, control.Title);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [NormalizedStringData]
    public void ComponentEditorPage_Text_SetWithHandle_GetReturnsExpected(string value, string expected)
    {
        using SubComponentEditorPage control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.Text = value;
        Assert.Equal(expected, control.Text);
        Assert.Equal(expected, control.Title);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.Text = value;
        Assert.Equal(expected, control.Text);
        Assert.Equal(expected, control.Title);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void ComponentEditorPage_Text_SetWithHandler_CallsTextChanged()
    {
        using SubComponentEditorPage control = new();
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
        Assert.Equal("text", control.Title);
        Assert.Equal(1, callCount);

        // Set same.
        control.Text = "text";
        Assert.Equal("text", control.Text);
        Assert.Equal("text", control.Title);
        Assert.Equal(1, callCount);

        // Set different.
        control.Text = null;
        Assert.Empty(control.Text);
        Assert.Empty(control.Title);
        Assert.Equal(2, callCount);

        // Remove handler.
        control.TextChanged -= handler;
        control.Text = "text";
        Assert.Equal("text", control.Text);
        Assert.Equal("text", control.Title);
        Assert.Equal(2, callCount);
    }

    [WinFormsTheory]
    [InlineData(true, 1)]
    [InlineData(false, 0)]
    public void ComponentEditorPage_Activate_Invoke_SetsVisible(bool loadRequired, int expectedLoadComponentCallCount)
    {
        using SubComponentEditorPage control = new()
        {
            LoadRequired = loadRequired
        };
        control.Activate();
        Assert.Equal(expectedLoadComponentCallCount, control.LoadComponentCallCount);
        Assert.Equal(0, control.Loading);
        Assert.False(control.LoadRequired);
        Assert.True(control.Visible);
        Assert.False(control.IsHandleCreated);

        // Call again.
        control.Activate();
        Assert.Equal(expectedLoadComponentCallCount, control.LoadComponentCallCount);
        Assert.Equal(0, control.Loading);
        Assert.False(control.LoadRequired);
        Assert.True(control.Visible);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [BoolData]
    public void ComponentEditorPage_ApplyChanges_Invoke_CallsSaveComponent(bool loadRequired)
    {
        using SubComponentEditorPage control = new()
        {
            LoadRequired = loadRequired
        };
        control.ApplyChanges();
        Assert.Equal(1, control.SaveComponentCallCount);
        Assert.Equal(0, control.Loading);
        Assert.Equal(loadRequired, control.LoadRequired);
        Assert.False(control.IsHandleCreated);

        // Call again.
        control.ApplyChanges();
        Assert.Equal(2, control.SaveComponentCallCount);
        Assert.Equal(0, control.Loading);
        Assert.Equal(loadRequired, control.LoadRequired);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true, 1)]
    [InlineData(false, 0)]
    public void ComponentEditorPage_Deactivate_InvokeActivated_SetsInvisible(bool loadRequired, int expectedLoadComponentCallCount)
    {
        using SubComponentEditorPage control = new()
        {
            LoadRequired = loadRequired
        };
        control.Activate();
        Assert.Equal(expectedLoadComponentCallCount, control.LoadComponentCallCount);
        Assert.Equal(0, control.Loading);
        Assert.False(control.LoadRequired);
        Assert.True(control.Visible);
        Assert.False(control.IsHandleCreated);

        control.Deactivate();
        Assert.Equal(expectedLoadComponentCallCount, control.LoadComponentCallCount);
        Assert.Equal(0, control.Loading);
        Assert.False(control.LoadRequired);
        Assert.False(control.Visible);
        Assert.False(control.IsHandleCreated);

        // Call again.
        control.Deactivate();
        Assert.Equal(expectedLoadComponentCallCount, control.LoadComponentCallCount);
        Assert.Equal(0, control.Loading);
        Assert.False(control.LoadRequired);
        Assert.False(control.Visible);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [BoolData]
    public void ComponentEditorPage_Deactivate_InvokeNotActivated_SetsInvisible(bool loadRequired)
    {
        using SubComponentEditorPage control = new()
        {
            LoadRequired = loadRequired
        };
        control.Deactivate();
        Assert.Equal(0, control.LoadComponentCallCount);
        Assert.Equal(0, control.Loading);
        Assert.Equal(loadRequired, control.LoadRequired);
        Assert.False(control.Visible);
        Assert.False(control.IsHandleCreated);

        // Call again.
        control.Deactivate();
        Assert.Equal(0, control.LoadComponentCallCount);
        Assert.Equal(0, control.Loading);
        Assert.Equal(loadRequired, control.LoadRequired);
        Assert.False(control.Visible);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void ComponentEditorPage_EnterLoadingMode_Invoke_IncrementsLoading()
    {
        using SubComponentEditorPage control = new();
        control.EnterLoadingMode();
        Assert.Equal(1, control.Loading);

        // Call again.
        control.EnterLoadingMode();
        Assert.Equal(2, control.Loading);
    }

    [WinFormsFact]
    public void ComponentEditorPage_EnterLoadingMode_ExitLoadingMode_Resets()
    {
        using SubComponentEditorPage control = new();
        control.EnterLoadingMode();
        Assert.Equal(1, control.Loading);

        control.ExitLoadingMode();
        Assert.Equal(0, control.Loading);
    }

    [WinFormsFact]
    public void ComponentEditorPage_ExitLoadingMode_Invoke_DoesNotDecrementLoading()
    {
        using SubComponentEditorPage control = new();
        control.ExitLoadingMode();
        Assert.Equal(0, control.Loading);

        // Call again.
        control.ExitLoadingMode();
        Assert.Equal(0, control.Loading);
    }

    [WinFormsFact]
    public void ComponentEditorPage_ExitLoadingMode_EnterLoadingMode_IncrementsLoading()
    {
        using SubComponentEditorPage control = new();
        control.ExitLoadingMode();
        Assert.Equal(0, control.Loading);

        control.EnterLoadingMode();
        Assert.Equal(1, control.Loading);
    }

    [WinFormsFact]
    public void ComponentEditorPage_IsFirstActivate_Invoke_ReturnsTrue()
    {
        using SubComponentEditorPage control = new();
        Assert.True(control.IsFirstActivate());
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void ComponentEditorPage_IsLoading_Invoke_ReturnsFalse()
    {
        using SubComponentEditorPage control = new();
        Assert.False(control.IsLoading());
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void ComponentEditorPage_IsPageMessage_Invoke_ReturnsExpected()
    {
        using SubComponentEditorPage control = new();
        Message message = default;
        Assert.True(control.IsPageMessage(ref message));
        Assert.Equal(1, control.PreProcessMessageCallCount);
    }

    [WinFormsFact]
    public void ComponentEditorPage_GetAutoSizeMode_Invoke_ReturnsExpected()
    {
        using SubComponentEditorPage control = new();
        Assert.Equal(AutoSizeMode.GrowOnly, control.GetAutoSizeMode());
    }

    [WinFormsFact]
    public void ComponentEditorPage_GetControl_InvokeDefault_ReturnsSame()
    {
        using SubComponentEditorPage control = new();
        Assert.Same(control, control.GetControl());
    }

    [WinFormsFact]
    public void ComponentEditorPage_GetSelectedComponent_InvokeDefault_ReturnsNull()
    {
        using SubComponentEditorPage control = new();
        Assert.Null(control.GetSelectedComponent());
    }

    [WinFormsTheory]
    [InlineData(0, true)]
    [InlineData(SubComponentEditorPage.ScrollStateAutoScrolling, false)]
    [InlineData(SubComponentEditorPage.ScrollStateFullDrag, false)]
    [InlineData(SubComponentEditorPage.ScrollStateHScrollVisible, false)]
    [InlineData(SubComponentEditorPage.ScrollStateUserHasScrolled, false)]
    [InlineData(SubComponentEditorPage.ScrollStateVScrollVisible, false)]
    [InlineData(int.MaxValue, false)]
    [InlineData((-1), false)]
    public void ComponentEditorPage_GetScrollState_Invoke_ReturnsExpected(int bit, bool expected)
    {
        using SubComponentEditorPage control = new();
        Assert.Equal(expected, control.GetScrollState(bit));
    }

    [WinFormsTheory]
    [InlineData(ControlStyles.ContainerControl, true)]
    [InlineData(ControlStyles.UserPaint, true)]
    [InlineData(ControlStyles.Opaque, false)]
    [InlineData(ControlStyles.ResizeRedraw, false)]
    [InlineData(ControlStyles.FixedWidth, false)]
    [InlineData(ControlStyles.FixedHeight, false)]
    [InlineData(ControlStyles.StandardClick, true)]
    [InlineData(ControlStyles.Selectable, false)]
    [InlineData(ControlStyles.UserMouse, false)]
    [InlineData(ControlStyles.SupportsTransparentBackColor, true)]
    [InlineData(ControlStyles.StandardDoubleClick, true)]
    [InlineData(ControlStyles.AllPaintingInWmPaint, false)]
    [InlineData(ControlStyles.CacheText, false)]
    [InlineData(ControlStyles.EnableNotifyMessage, false)]
    [InlineData(ControlStyles.DoubleBuffer, false)]
    [InlineData(ControlStyles.OptimizedDoubleBuffer, false)]
    [InlineData(ControlStyles.UseTextForAccessibility, true)]
    [InlineData((ControlStyles)0, true)]
    [InlineData((ControlStyles)int.MaxValue, false)]
    [InlineData((ControlStyles)(-1), false)]
    public void ComponentEditorPage_GetStyle_Invoke_ReturnsExpected(ControlStyles flag, bool expected)
    {
        using SubComponentEditorPage control = new();
        Assert.Equal(expected, control.GetStyle(flag));

        // Call again to test caching.
        Assert.Equal(expected, control.GetStyle(flag));
    }

    [WinFormsFact]
    public void ComponentEditorPage_GetTopLevel_Invoke_ReturnsExpected()
    {
        using SubComponentEditorPage control = new();
        Assert.False(control.GetTopLevel());
    }

    [WinFormsTheory]
    [InlineData(true, false)]
    [InlineData(false, true)]
    public void ComponentEditorPage_OnApplyComplete_Invoke_SetsLoadRequired(bool visible, bool expectedLoadRequired)
    {
        using SubComponentEditorPage control = new()
        {
            Visible = visible
        };
        Assert.Equal(visible, control.Visible);
        control.OnApplyComplete();
        Assert.Equal(0, control.LoadComponentCallCount);
        Assert.Equal(0, control.Loading);
        Assert.Equal(expectedLoadRequired, control.LoadRequired);
    }

    [WinFormsTheory]
    [InlineData(true, false)]
    [InlineData(false, true)]
    public void ComponentEditorPage_ReloadComponent_Invoke_SetsLoadRequired(bool visible, bool expectedLoadRequired)
    {
        using SubComponentEditorPage control = new()
        {
            Visible = visible
        };
        Assert.Equal(visible, control.Visible);
        control.ReloadComponent();
        Assert.Equal(0, control.LoadComponentCallCount);
        Assert.Equal(0, control.Loading);
        Assert.Equal(expectedLoadRequired, control.LoadRequired);
    }

    [WinFormsTheory]
    [MemberData(nameof(Component_Set_TestData))]
    public void ComponentEditorPage_SetComponent_Invoke_SetsComponent(IComponent component)
    {
        using SubComponentEditorPage control = new();
        control.SetComponent(component);
        Assert.Same(component, control.Component);
        Assert.Equal(0, control.LoadComponentCallCount);
        Assert.Equal(0, control.Loading);
        Assert.True(control.LoadRequired);

        // Set same.
        control.SetComponent(component);
        Assert.Same(component, control.Component);
        Assert.Equal(0, control.LoadComponentCallCount);
        Assert.Equal(0, control.Loading);
        Assert.True(control.LoadRequired);
    }

    [WinFormsTheory]
    [InlineData(-1, 0)]
    [InlineData(0, 1)]
    [InlineData(1, 0)]
    public void ComponentEditorPage_SetDirty_InvokeWithPageSites_CallsSetDirty(int loading, int expectedSetDirtyCallCount)
    {
        Mock<IComponentEditorPageSite> mockComponentEditorPageSite = new(MockBehavior.Strict);
        mockComponentEditorPageSite
            .Setup(s => s.SetDirty())
            .Verifiable();
        using SubComponentEditorPage control = new()
        {
            PageSite = mockComponentEditorPageSite.Object,
            Loading = loading
        };
        control.SetDirty();
        mockComponentEditorPageSite.Verify(s => s.SetDirty(), Times.Exactly(expectedSetDirtyCallCount));

        // Call again.
        control.SetDirty();
        mockComponentEditorPageSite.Verify(s => s.SetDirty(), Times.Exactly(expectedSetDirtyCallCount * 2));
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    public void ComponentEditorPage_SetDirty_InvokeWithoutPageSite_Nop(int loading)
    {
        using SubComponentEditorPage control = new()
        {
            Loading = loading
        };
        control.SetDirty();
        control.SetDirty();
    }

    [WinFormsFact]
    public void ComponentEditorPage_SetSite_Invoke_SetsPageSite()
    {
        using Control result = new();
        Mock<IComponentEditorPageSite> controlSite = new(MockBehavior.Strict);
        controlSite
            .Setup(s => s.GetControl())
            .Returns(result)
            .Verifiable();
        Mock<IComponentEditorPageSite> noControlSite = new(MockBehavior.Strict);
        noControlSite
            .Setup(s => s.GetControl())
            .Returns<Control>(null)
            .Verifiable();
        using SubComponentEditorPage control = new();
        control.SetSite(controlSite.Object);
        Assert.Same(controlSite.Object, control.PageSite);
        Assert.Same(control, Assert.Single(result.Controls));
        Assert.Equal(0, control.LoadComponentCallCount);
        Assert.Equal(0, control.Loading);
        Assert.False(control.LoadRequired);

        // Set same.
        control.SetSite(controlSite.Object);
        Assert.Same(controlSite.Object, control.PageSite);
        Assert.Same(control, Assert.Single(result.Controls));
        Assert.Equal(0, control.LoadComponentCallCount);
        Assert.Equal(0, control.Loading);
        Assert.False(control.LoadRequired);

        // Set different.
        control.SetSite(noControlSite.Object);
        Assert.Same(noControlSite.Object, control.PageSite);
        Assert.Same(control, Assert.Single(result.Controls));
        Assert.Equal(0, control.LoadComponentCallCount);
        Assert.Equal(0, control.Loading);
        Assert.False(control.LoadRequired);

        // Set null.
        control.SetSite(null);
        Assert.Null(control.PageSite);
        Assert.Same(control, Assert.Single(result.Controls));
        Assert.Equal(0, control.LoadComponentCallCount);
        Assert.Equal(0, control.Loading);
        Assert.False(control.LoadRequired);
    }

    [WinFormsFact]
    public void ComponentEditorPage_ShowHelp_Invoke_Nop()
    {
        using SubComponentEditorPage control = new();
        control.ShowHelp();

        // Call again.
        control.ShowHelp();
    }

    [WinFormsFact]
    public void ComponentEditorPage_SupportsHelp_Invoke_ReturnsFalse()
    {
        using SubComponentEditorPage control = new();
        Assert.False(control.SupportsHelp());

        // Call again.
        Assert.False(control.SupportsHelp());
    }

    private class SubComponentEditorPage : ComponentEditorPage
    {
        public new const int ScrollStateAutoScrolling = ScrollableControl.ScrollStateAutoScrolling;

        public new const int ScrollStateHScrollVisible = ScrollableControl.ScrollStateHScrollVisible;

        public new const int ScrollStateVScrollVisible = ScrollableControl.ScrollStateVScrollVisible;

        public new const int ScrollStateUserHasScrolled = ScrollableControl.ScrollStateUserHasScrolled;

        public new const int ScrollStateFullDrag = ScrollableControl.ScrollStateFullDrag;

        public new bool CanEnableIme => base.CanEnableIme;

        public new bool CanRaiseEvents => base.CanRaiseEvents;

        public new IComponent Component
        {
            get => base.Component;
            set => base.Component = value;
        }

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

        public new bool FirstActivate
        {
            get => base.FirstActivate;
            set => base.FirstActivate = value;
        }

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

        public new bool HScroll
        {
            get => base.HScroll;
            set => base.HScroll = value;
        }

        public new int Loading
        {
            get => base.Loading;
            set => base.Loading = value;
        }

        public new bool LoadRequired
        {
            get => base.LoadRequired;
            set => base.LoadRequired = value;
        }

        public new IComponentEditorPageSite PageSite
        {
            get => base.PageSite;
            set => base.PageSite = value;
        }

        public new bool VScroll
        {
            get => base.VScroll;
            set => base.VScroll = value;
        }

        public new void EnterLoadingMode() => base.EnterLoadingMode();

        public new void ExitLoadingMode() => base.ExitLoadingMode();

        public new AutoSizeMode GetAutoSizeMode() => base.GetAutoSizeMode();

        public new Control GetControl() => base.GetControl();

        public new bool GetScrollState(int bit) => base.GetScrollState(bit);

        public new IComponent GetSelectedComponent() => base.GetSelectedComponent();

        public new bool GetStyle(ControlStyles flag) => base.GetStyle(flag);

        public new bool GetTopLevel() => base.GetTopLevel();

        public new bool IsFirstActivate() => base.IsFirstActivate();

        public new bool IsLoading() => base.IsLoading();

        public int LoadComponentCallCount { get; set; }

        protected override void LoadComponent() => LoadComponentCallCount++;

        public int PreProcessMessageCallCount { get; set; }

        public override bool PreProcessMessage(ref Message msg)
        {
            PreProcessMessageCallCount++;
            return true;
        }

        public new void ReloadComponent() => base.ReloadComponent();

        public int SaveComponentCallCount { get; set; }

        protected override void SaveComponent() => SaveComponentCallCount++;

        public new void SetDirty() => base.SetDirty();
    }
}
