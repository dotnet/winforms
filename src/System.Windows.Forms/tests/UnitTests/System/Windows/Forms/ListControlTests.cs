// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using Moq;
using Point = System.Drawing.Point;
using Size = System.Drawing.Size;

namespace System.Windows.Forms.Tests;

public class ListControlTests
{
    [WinFormsFact]
    public void ListControl_Ctor_Default()
    {
        using SubListControl control = new();
        Assert.Null(control.AccessibleDefaultActionDescription);
        Assert.Null(control.AccessibleDescription);
        Assert.Null(control.AccessibleName);
        Assert.Equal(AccessibleRole.Default, control.AccessibleRole);
        Assert.False(control.AllowDrop);
        Assert.True(control.AllowSelectionEntry);
        Assert.Equal(AnchorStyles.Top | AnchorStyles.Left, control.Anchor);
        Assert.False(control.AutoSize);
        Assert.Equal(Control.DefaultBackColor, control.BackColor);
        Assert.Null(control.BackgroundImage);
        Assert.Equal(ImageLayout.Tile, control.BackgroundImageLayout);
        Assert.Null(control.BindingContext);
        Assert.Equal(0, control.Bottom);
        Assert.Equal(Rectangle.Empty, control.Bounds);
        Assert.True(control.CanEnableIme);
        Assert.False(control.CanFocus);
        Assert.True(control.CanRaiseEvents);
        Assert.True(control.CanSelect);
        Assert.False(control.Capture);
        Assert.True(control.CausesValidation);
        Assert.Equal(Size.Empty, control.ClientSize);
        Assert.Equal(Rectangle.Empty, control.ClientRectangle);
        Assert.Null(control.Container);
        Assert.False(control.ContainsFocus);
        Assert.Null(control.ContextMenuStrip);
        Assert.Empty(control.Controls);
        Assert.Same(control.Controls, control.Controls);
        Assert.False(control.Created);
        Assert.Same(Cursors.Default, control.Cursor);
        Assert.Null(control.DataManager);
        Assert.Null(control.DataSource);
        Assert.Same(Cursors.Default, control.DefaultCursor);
        Assert.Equal(ImeMode.Inherit, control.DefaultImeMode);
        Assert.Equal(new Padding(3), control.DefaultMargin);
        Assert.Equal(Size.Empty, control.DefaultMaximumSize);
        Assert.Equal(Size.Empty, control.DefaultMinimumSize);
        Assert.Equal(Padding.Empty, control.DefaultPadding);
        Assert.Equal(Size.Empty, control.DefaultSize);
        Assert.False(control.DesignMode);
        Assert.Empty(control.DisplayMember);
        Assert.Equal(Rectangle.Empty, control.DisplayRectangle);
        Assert.Equal(DockStyle.None, control.Dock);
        Assert.False(control.DoubleBuffered);
        Assert.True(control.Enabled);
        Assert.NotNull(control.Events);
        Assert.Same(control.Events, control.Events);
        Assert.False(control.Focused);
        Assert.Equal(Control.DefaultFont, control.Font);
        Assert.Equal(control.Font.Height, control.FontHeight);
        Assert.Equal(Control.DefaultForeColor, control.ForeColor);
        Assert.Null(control.FormatInfo);
        Assert.Empty(control.FormatString);
        Assert.False(control.FormattingEnabled);
        Assert.False(control.HasChildren);
        Assert.Equal(0, control.Height);
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
        Assert.Equal(Padding.Empty, control.Padding);
        Assert.Null(control.Parent);
        Assert.Equal("Microsoft\u00AE .NET", control.ProductName);
        Assert.Equal(Size.Empty, control.PreferredSize);
        Assert.False(control.RecreatingHandle);
        Assert.Null(control.Region);
        Assert.False(control.ResizeRedraw);
        Assert.Equal(0, control.Right);
        Assert.Equal(RightToLeft.No, control.RightToLeft);
        Assert.Null(control.SelectedValue);
        Assert.Equal(0, control.SelectedIndex);
        Assert.True(control.ShowFocusCues);
        Assert.True(control.ShowKeyboardCues);
        Assert.Null(control.Site);
        Assert.Equal(Size.Empty, control.Size);
        Assert.Equal(0, control.TabIndex);
        Assert.True(control.TabStop);
        Assert.Empty(control.Text);
        Assert.Equal(0, control.Top);
        Assert.Null(control.TopLevelControl);
        Assert.False(control.UseWaitCursor);
        Assert.Empty(control.ValueMember);
        Assert.True(control.Visible);
        Assert.Equal(0, control.Width);

        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListControl_CreateParams_GetDefault_ReturnsExpected()
    {
        using SubListControl control = new();
        CreateParams createParams = control.CreateParams;
        Assert.Null(createParams.Caption);
        Assert.Null(createParams.ClassName);
        Assert.Equal(0x8, createParams.ClassStyle);
        Assert.Equal(0, createParams.ExStyle);
        Assert.Equal(0, createParams.Height);
        Assert.Equal(IntPtr.Zero, createParams.Parent);
        Assert.Null(createParams.Param);
        Assert.Equal(0x56010000, createParams.Style);
        Assert.Equal(0, createParams.Width);
        Assert.Equal(0, createParams.X);
        Assert.Equal(0, createParams.Y);
        Assert.Same(createParams, control.CreateParams);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> BindingContext_Set_TestData()
    {
        yield return new object[] { null };
        yield return new object[] { new BindingContext() };
    }

    [WinFormsTheory]
    [MemberData(nameof(BindingContext_Set_TestData))]
    public void ListControl_BindingContext_Set_GetReturnsExpected(BindingContext value)
    {
        using SubListControl control = new()
        {
            BindingContext = value
        };
        Assert.Same(value, control.BindingContext);
        Assert.Null(control.DataSource);
        Assert.Empty(control.DisplayMember);
        Assert.Null(control.DataManager);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.BindingContext = value;
        Assert.Same(value, control.BindingContext);
        Assert.Null(control.DataSource);
        Assert.Empty(control.DisplayMember);
        Assert.Null(control.DataManager);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(BindingContext_Set_TestData))]
    public void ListControl_BindingContext_SetWithNonNullBindingContext_GetReturnsExpected(BindingContext value)
    {
        using SubListControl control = new()
        {
            BindingContext = []
        };

        control.BindingContext = value;
        Assert.Same(value, control.BindingContext);
        Assert.Null(control.DataSource);
        Assert.Empty(control.DisplayMember);
        Assert.Null(control.DataManager);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.BindingContext = value;
        Assert.Same(value, control.BindingContext);
        Assert.Null(control.DataSource);
        Assert.Empty(control.DisplayMember);
        Assert.Null(control.DataManager);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListControl_BindingContext_SetWithDataSource_GetReturnsExpected()
    {
        BindingContext value = [];
        List<DataClass> dataSource = [];
        using SubListControl control = new()
        {
            DataSource = dataSource
        };

        control.BindingContext = value;
        Assert.Same(value, control.BindingContext);
        Assert.Same(dataSource, control.DataSource);
        Assert.Empty(control.DisplayMember);
        Assert.Same(value[dataSource], control.DataManager);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.BindingContext = value;
        Assert.Same(value, control.BindingContext);
        Assert.Same(dataSource, control.DataSource);
        Assert.Empty(control.DisplayMember);
        Assert.Same(value[dataSource], control.DataManager);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListControl_BindingContext_SetWithDataSourceAndDisplayMember_GetReturnsExpected()
    {
        BindingContext value = [];
        List<DataClass> dataSource = [];
        using SubListControl control = new()
        {
            DataSource = dataSource,
            DisplayMember = "Value"
        };

        control.BindingContext = value;
        Assert.Same(value, control.BindingContext);
        Assert.Same(dataSource, control.DataSource);
        Assert.Equal("Value", control.DisplayMember);
        Assert.Same(value[dataSource], control.DataManager);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.BindingContext = value;
        Assert.Same(value, control.BindingContext);
        Assert.Same(dataSource, control.DataSource);
        Assert.Equal("Value", control.DisplayMember);
        Assert.Same(value[dataSource], control.DataManager);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListControl_BindingContext_SetWithDataSourceWithBindingContext_GetReturnsExpected()
    {
        BindingContext originalValue = [];
        BindingContext value = [];
        List<DataClass> dataSource = [];
        using SubListControl control = new()
        {
            BindingContext = originalValue,
            DataSource = dataSource
        };

        control.BindingContext = value;
        Assert.Same(value, control.BindingContext);
        Assert.Same(dataSource, control.DataSource);
        Assert.Empty(control.DisplayMember);
        Assert.Same(value[dataSource], control.DataManager);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.BindingContext = value;
        Assert.Same(value, control.BindingContext);
        Assert.Same(dataSource, control.DataSource);
        Assert.Empty(control.DisplayMember);
        Assert.Same(value[dataSource], control.DataManager);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListControl_BindingContext_SetWithDataSourceAndDisplayMemberWithBindingContext_GetReturnsExpected()
    {
        BindingContext originalValue = [];
        BindingContext value = [];
        List<DataClass> dataSource = [];
        using SubListControl control = new()
        {
            BindingContext = originalValue,
            DataSource = dataSource,
            DisplayMember = "Value"
        };

        control.BindingContext = value;
        Assert.Same(value, control.BindingContext);
        Assert.Same(dataSource, control.DataSource);
        Assert.Equal("Value", control.DisplayMember);
        Assert.Same(value[dataSource], control.DataManager);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.BindingContext = value;
        Assert.Same(dataSource, control.DataSource);
        Assert.Equal("Value", control.DisplayMember);
        Assert.Same(value[dataSource], control.DataManager);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListControl_BindingContext_SetWithHandler_CallsBindingContextChanged()
    {
        using SubListControl control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        control.BindingContextChanged += handler;

        // Set different.
        BindingContext context1 = [];
        control.BindingContext = context1;
        Assert.Same(context1, control.BindingContext);
        Assert.Equal(1, callCount);

        // Set same.
        control.BindingContext = context1;
        Assert.Same(context1, control.BindingContext);
        Assert.Equal(1, callCount);

        // Set different.
        BindingContext context2 = [];
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

    [WinFormsFact]
    public void ListControl_DataManager_ChangePosition_UpdatesSelectionIndex()
    {
        BindingContext context = [];
        List<DataClass> dataSource = [new DataClass(), new DataClass()];
        using SubListControl control = new()
        {
            BindingContext = context,
            DataSource = dataSource
        };

        control.DataManager.Position = 1;
        Assert.Equal(1, control.SelectedIndex);
    }

    [WinFormsFact]
    public void ListControl_DataManager_ChangePositionDoesNotAllowSelection_DoesNotUpdateSelectionIndex()
    {
        BindingContext context = [];
        List<DataClass> dataSource = [new DataClass(), new DataClass()];
        using AllowSelectionFalseListControl control = new()
        {
            SelectedIndex = -2,
            BindingContext = context,
            DataSource = dataSource,
        };

        control.DataManager.Position = 1;
        Assert.Equal(-2, control.SelectedIndex);
    }

    [WinFormsFact]
    public void ListControl_DataManager_SuspendResumeBinding_CallsSetItemsCore()
    {
        BindingContext context = [];
        List<DataClass> dataSource = [new DataClass(), new DataClass()];
        using SubListControl control = new()
        {
            SelectedIndex = -1,
            BindingContext = context,
            DataSource = dataSource
        };
        int callCount = 0;
        control.SetItemsCoreHandler += (items) =>
        {
            Assert.Same(dataSource, items);
            callCount++;
        };

        Assert.Equal(-1, control.SelectedIndex);
        Assert.Equal(0, control.DataManager.Position);

        // Suspending should call.
        control.DataManager.SuspendBinding();
        Assert.Equal(-1, control.SelectedIndex);
        Assert.Equal(1, callCount);

        // Suspending again should be a nop.
        control.DataManager.SuspendBinding();
        Assert.Equal(-1, control.SelectedIndex);
        Assert.Equal(1, callCount);

        // Resuming should call.
        control.DataManager.ResumeBinding();
        Assert.Equal(0, control.SelectedIndex);
        Assert.Equal(2, callCount);

        // Resuming again should be a nop.
        control.DataManager.ResumeBinding();
        Assert.Equal(0, control.SelectedIndex);
        Assert.Equal(2, callCount);

        // Should not call if the DataSource is changed.
        control.DataSource = new List<DataClass>();
    }

    [WinFormsFact]
    public void ListControl_DataManager_SuspendResumeBindingAfterDataManagerChanged_DoesNotCallSetItemsCore()
    {
        BindingContext context = [];
        List<DataClass> dataSource = [new DataClass(), new DataClass()];
        using SubListControl control = new()
        {
            SelectedIndex = -1,
            BindingContext = context,
            DataSource = dataSource
        };
        int callCount = 0;
        control.SetItemsCoreHandler += (items) =>
        {
            Assert.Same(dataSource, items);
            callCount++;
        };

        CurrencyManager dataManger = control.DataManager;
        dataManger.SuspendBinding();
        Assert.Equal(-1, control.SelectedIndex);
        Assert.Equal(1, callCount);

        // Should not call if the DataSource is changed.
        control.DataSource = new List<DataClass>();
        Assert.NotSame(dataManger, control.DataManager);
        dataManger.ResumeBinding();
        Assert.Equal(-1, control.SelectedIndex);
        Assert.Equal(1, callCount);
    }

    [WinFormsFact]
    public void ListControl_DataManager_SuspendResumeBindingDoesNotAllowSelection_CallsSetItemsCore()
    {
        BindingContext context = [];
        List<DataClass> dataSource = [new DataClass(), new DataClass()];
        using AllowSelectionFalseListControl control = new()
        {
            SelectedIndex = -1,
            BindingContext = context,
            DataSource = dataSource
        };
        int callCount = 0;
        control.SetItemsCoreHandler += (items) =>
        {
            Assert.Same(dataSource, items);
            callCount++;
        };

        Assert.Equal(-1, control.SelectedIndex);
        Assert.Equal(0, control.DataManager.Position);

        // Suspending should call.
        control.DataManager.SuspendBinding();
        Assert.Equal(-1, control.SelectedIndex);
        Assert.Equal(1, callCount);

        // Suspending again should be a nop.
        control.DataManager.SuspendBinding();
        Assert.Equal(-1, control.SelectedIndex);
        Assert.Equal(1, callCount);

        // Resuming should call.
        control.DataManager.ResumeBinding();
        Assert.Equal(-1, control.SelectedIndex);
        Assert.Equal(2, callCount);

        // Resuming again should be a nop.
        control.DataManager.ResumeBinding();
        Assert.Equal(-1, control.SelectedIndex);
        Assert.Equal(2, callCount);
    }

    [WinFormsFact]
    public void ListControl_DataManager_CancelCurrentEdit_CallsSetItemCore()
    {
        BindingContext context = [];
        List<DataClass> dataSource = [new DataClass(), new DataClass()];
        using SubListControl control = new()
        {
            SelectedIndex = -1,
            BindingContext = context,
            DataSource = dataSource
        };
        int callCount = 0;
        control.SetItemCoreHandler += (index, value) =>
        {
            Assert.Equal(0, index);
            Assert.Same(dataSource[0], value);
            callCount++;
        };

        Assert.Equal(-1, control.SelectedIndex);
        Assert.Equal(0, control.DataManager.Position);

        control.DataManager.CancelCurrentEdit();
        Assert.Equal(-1, control.SelectedIndex);
        Assert.Equal(1, callCount);
    }

    [WinFormsFact]
    public void ListControl_DataManager_CancelCurrentEditAfterDataManagerChanged_DoesNotCallSetItemCore()
    {
        BindingContext context = [];
        List<DataClass> dataSource = [new DataClass(), new DataClass()];
        using SubListControl control = new()
        {
            SelectedIndex = -1,
            BindingContext = context,
            DataSource = dataSource
        };
        int callCount = 0;
        control.SetItemCoreHandler += (index, value) =>
        {
            Assert.Equal(0, index);
            Assert.Same(dataSource[0], value);
            callCount++;
        };

        CurrencyManager dataManger = control.DataManager;
        dataManger.CancelCurrentEdit();
        Assert.Equal(-1, control.SelectedIndex);
        Assert.Equal(1, callCount);

        // Should not call if the DataSource is changed.
        control.DataSource = new List<DataClass>();
        Assert.NotSame(dataManger, control.DataManager);
        dataManger.CancelCurrentEdit();
        Assert.Equal(-1, control.SelectedIndex);
        Assert.Equal(1, callCount);
    }

    public static IEnumerable<object[]> DataSource_Set_TestData()
    {
        yield return new object[] { null };
        yield return new object[] { new List<int>() };
        yield return new object[] { Array.Empty<int>() };

        Mock<IListSource> mockSource = new(MockBehavior.Strict);
        mockSource
            .Setup(s => s.GetList())
            .Returns(new int[] { 1 });
        yield return new object[] { mockSource.Object };
    }

    [WinFormsTheory]
    [MemberData(nameof(DataSource_Set_TestData))]
    public void ListControl_DataSource_Set_GetReturnsExpected(object value)
    {
        using SubListControl control = new()
        {
            DataSource = value
        };
        Assert.Same(value, control.DataSource);
        Assert.Empty(control.DisplayMember);
        Assert.Null(control.DataManager);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.DataSource = value;
        Assert.Same(value, control.DataSource);
        Assert.Empty(control.DisplayMember);
        Assert.Null(control.DataManager);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(DataSource_Set_TestData))]
    public void ListControl_DataSource_SetWithDataSourceNoDisplayMember_GetReturnsExpected(object value)
    {
        using SubListControl control = new()
        {
            DataSource = new List<int>()
        };

        control.DataSource = value;
        Assert.Same(value, control.DataSource);
        Assert.Empty(control.DisplayMember);
        Assert.Null(control.DataManager);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.DataSource = value;
        Assert.Same(value, control.DataSource);
        Assert.Empty(control.DisplayMember);
        Assert.Null(control.DataManager);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(DataSource_Set_TestData))]
    public void ListControl_DataSource_SetWithDisplayMember_GetReturnsExpected(object value)
    {
        using SubListControl control = new()
        {
            DisplayMember = "Capacity"
        };

        control.DataSource = value;
        Assert.Same(value, control.DataSource);
        Assert.Equal("Capacity", control.DisplayMember);
        Assert.Null(control.DataManager);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.DataSource = value;
        Assert.Same(value, control.DataSource);
        Assert.Equal("Capacity", control.DisplayMember);
        Assert.Null(control.DataManager);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(DataSource_Set_TestData))]
    public void ListControl_DataSource_SetWithDataSourceNoSuchDisplayMemberAnymore_GetReturnsExpected(object value)
    {
        List<int> originalValue = [];
        using SubListControl control = new()
        {
            DataSource = originalValue,
            DisplayMember = "Capacity"
        };

        control.DataSource = value;
        Assert.Same(value, control.DataSource);
        Assert.Equal(value is null ? string.Empty : "Capacity", control.DisplayMember);
        Assert.Null(control.DataManager);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.DataSource = value;
        Assert.Same(value, control.DataSource);
        Assert.Equal(value is null ? string.Empty : "Capacity", control.DisplayMember);
        Assert.Null(control.DataManager);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListControl_DataSource_SetComponent_DisposeValue_Removes()
    {
        ComponentList value = new();
        using SubListControl control = new()
        {
            DataSource = value,
            DisplayMember = "Count"
        };
        Assert.Same(value, control.DataSource);
        Assert.Equal("Count", control.DisplayMember);
        Assert.Null(control.DataManager);

        value.Dispose();
        Assert.Null(control.DataSource);
        Assert.Empty(control.DisplayMember);
        Assert.Null(control.DataManager);
    }

    [WinFormsFact]
    public void ListControl_DataSource_SetComponent_DisposeValueDifferentSender_Removes()
    {
        ComponentList value = new();
        using SubListControl control = new()
        {
            DataSource = value,
            DisplayMember = "Count"
        };
        Assert.Same(value, control.DataSource);
        Assert.Equal("Count", control.DisplayMember);
        Assert.Null(control.DataManager);

        value.OnDisposed(new object(), EventArgs.Empty);
        Assert.Null(control.DataSource);
        Assert.Empty(control.DisplayMember);
        Assert.Null(control.DataManager);
    }

    [WinFormsFact]
    public void ListControl_DataSource_SetComponent_DisposeValueNullSender_Removes()
    {
        ComponentList value = new();
        using SubListControl control = new()
        {
            DataSource = value,
            DisplayMember = "Count"
        };
        Assert.Same(value, control.DataSource);
        Assert.Equal("Count", control.DisplayMember);
        Assert.Null(control.DataManager);

        value.OnDisposed(null, null);
        Assert.Null(control.DataSource);
        Assert.Empty(control.DisplayMember);
        Assert.Null(control.DataManager);
    }

    [WinFormsFact]
    public void ListControl_DataSource_SetOverriddenComponent_DisposeValue_DoesNotRemove()
    {
        ComponentList originalValue = new();
        List<int> value = [];
        using SubListControl control = new()
        {
            DataSource = originalValue,
            DisplayMember = "Count"
        };
        Assert.Same(originalValue, control.DataSource);
        Assert.Equal("Count", control.DisplayMember);
        Assert.Null(control.DataManager);

        control.DataSource = value;
        originalValue.Dispose();
        Assert.Same(value, control.DataSource);
        Assert.Equal("Count", control.DisplayMember);
        Assert.Null(control.DataManager);
    }

    [WinFormsFact]
    public void ListControl_DataSource_SetSupportInitializeNotification_InitializeValueNotInitializedSetsInitialized_Success()
    {
        SupportInitializeNotificationList value = new();
        value.Initialized += (sender, e) =>
        {
            value.IsInitialized = true;
        };

        using SubListControl control = new()
        {
            DataSource = value,
            DisplayMember = "Count"
        };
        Assert.Same(value, control.DataSource);
        Assert.Equal("Count", control.DisplayMember);
        Assert.Null(control.DataManager);

        value.OnInitialized(value, EventArgs.Empty);
        Assert.Same(value, control.DataSource);
        Assert.Equal("Count", control.DisplayMember);
        Assert.Null(control.DataManager);
    }

    [WinFormsFact]
    public void ListControl_DataSource_SetSupportInitializeNotification_InitializeValueNotInitialized_Success()
    {
        SupportInitializeNotificationList value = new();
        using SubListControl control = new()
        {
            DataSource = value,
            DisplayMember = "Count"
        };
        Assert.Same(value, control.DataSource);
        Assert.Equal("Count", control.DisplayMember);
        Assert.Null(control.DataManager);

        value.OnInitialized(value, EventArgs.Empty);
        Assert.Same(value, control.DataSource);
        Assert.Equal("Count", control.DisplayMember);
        Assert.Null(control.DataManager);
    }

    [WinFormsFact]
    public void ListControl_DataSource_SetSupportInitializeNotification_InitializeValueInitialized_Success()
    {
        SupportInitializeNotificationList value = new();
        using SubListControl control = new()
        {
            DataSource = value,
            DisplayMember = "Count"
        };
        Assert.Same(value, control.DataSource);
        Assert.Equal("Count", control.DisplayMember);
        Assert.Null(control.DataManager);

        value.OnInitialized(value, EventArgs.Empty);
        Assert.Same(value, control.DataSource);
        Assert.Equal("Count", control.DisplayMember);
        Assert.Null(control.DataManager);
    }

    [WinFormsFact]
    public void ListControl_DataSource_SetSupportInitializeNotification_InitializeValueDifferentSender_Success()
    {
        SupportInitializeNotificationList value = new();
        using SubListControl control = new()
        {
            DataSource = value,
            DisplayMember = "Count"
        };
        Assert.Same(value, control.DataSource);
        Assert.Equal("Count", control.DisplayMember);
        Assert.Null(control.DataManager);

        value.OnInitialized(new object(), EventArgs.Empty);
        Assert.Same(value, control.DataSource);
        Assert.Equal("Count", control.DisplayMember);
        Assert.Null(control.DataManager);
    }

    [WinFormsFact]
    public void ListControl_DataSource_SetSupportInitializeNotification_InitializeValueNullSender_Succes()
    {
        SupportInitializeNotificationList value = new();
        using SubListControl control = new()
        {
            DataSource = value,
            DisplayMember = "Count"
        };
        Assert.Same(value, control.DataSource);
        Assert.Equal("Count", control.DisplayMember);
        Assert.Null(control.DataManager);

        value.OnInitialized(null, null);
        Assert.Same(value, control.DataSource);
        Assert.Equal("Count", control.DisplayMember);
        Assert.Null(control.DataManager);
    }

    [WinFormsTheory]
    [BoolData]
    public void ListControl_DataSource_SetOverriddenSupportInitializeNotification_InitializeValue_Success(bool isInitialized)
    {
        SupportInitializeNotificationList originalValue = new();
        List<int> value = [];
        using SubListControl control = new()
        {
            DataSource = originalValue,
            DisplayMember = "Count"
        };
        Assert.Same(originalValue, control.DataSource);
        Assert.Equal("Count", control.DisplayMember);
        Assert.Null(control.DataManager);

        originalValue.IsInitialized = isInitialized;
        control.DataSource = value;
        originalValue.OnInitialized(originalValue, EventArgs.Empty);
        Assert.Same(value, control.DataSource);
        Assert.Equal("Count", control.DisplayMember);
        Assert.Null(control.DataManager);
    }

    [WinFormsTheory]
    [MemberData(nameof(DataSource_Set_TestData))]
    public void ListControl_DataSource_SetWithBindingContext_GetReturnsExpected(object value)
    {
        BindingContext context = [];
        using SubListControl control = new()
        {
            BindingContext = context
        };

        control.DataSource = value;
        Assert.Same(value, control.DataSource);
        Assert.Empty(control.DisplayMember);
        if (value is null)
        {
            Assert.Null(control.DataManager);
        }
        else
        {
            Assert.Same(context[value], control.DataManager);
        }

        // Set same.
        control.DataSource = value;
        Assert.Same(value, control.DataSource);
        Assert.Empty(control.DisplayMember);
        if (value is null)
        {
            Assert.Null(control.DataManager);
        }
        else
        {
            Assert.Same(context[value], control.DataManager);
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(DataSource_Set_TestData))]
    public void ListControl_DataSource_SetWithBindingContextWithDataSource_GetReturnsExpected(object value)
    {
        BindingContext context = [];
        using SubListControl control = new()
        {
            BindingContext = context,
            DataSource = new List<int>()
        };

        control.DataSource = value;
        Assert.Same(value, control.DataSource);
        Assert.Empty(control.DisplayMember);
        if (value is null)
        {
            Assert.Null(control.DataManager);
        }
        else
        {
            Assert.Same(context[value], control.DataManager);
        }

        // Set same.
        control.DataSource = value;
        Assert.Same(value, control.DataSource);
        Assert.Empty(control.DisplayMember);
        if (value is null)
        {
            Assert.Null(control.DataManager);
        }
        else
        {
            Assert.Same(context[value], control.DataManager);
        }
    }

    [WinFormsFact]
    public void ListControl_DataSource_SetWithBindingContextWithDisplayMemberCanCreate_GetReturnsExpected()
    {
        BindingContext context = [];
        List<DataClass> value = [];
        using SubListControl control = new()
        {
            BindingContext = context,
            DisplayMember = "Value"
        };

        control.DataSource = value;
        Assert.Same(value, control.DataSource);
        Assert.Equal("Value", control.DisplayMember);
        Assert.Same(context[value], control.DataManager);

        // Set same.
        control.DataSource = value;
        Assert.Same(value, control.DataSource);
        Assert.Equal("Value", control.DisplayMember);
        Assert.Same(context[value], control.DataManager);
    }

    [WinFormsTheory]
    [MemberData(nameof(DataSource_Set_TestData))]
    public void ListControl_DataSource_SetWithBindingContextWithDisplayMemberCantCreate_GetReturnsExpected(object value)
    {
        BindingContext context = [];
        using SubListControl control = new()
        {
            BindingContext = context,
            DisplayMember = "NoSuchDisplayMember"
        };

        control.DataSource = value;
        Assert.Same(value, control.DataSource);
        Assert.Equal(value is null ? "NoSuchDisplayMember" : string.Empty, control.DisplayMember);
        if (value is null)
        {
            Assert.Null(control.DataManager);
        }
        else
        {
            Assert.Same(context[value], control.DataManager);
        }

        // Set same.
        control.DataSource = value;
        Assert.Same(value, control.DataSource);
        Assert.Equal(value is null ? "NoSuchDisplayMember" : string.Empty, control.DisplayMember);
        if (value is null)
        {
            Assert.Null(control.DataManager);
        }
        else
        {
            Assert.Same(context[value], control.DataManager);
        }
    }

    [WinFormsFact]
    public void ListControl_DataSource_SetWithHandler_CallsDataSourceChanged()
    {
        using SubListControl control = new();
        int dataSourceCallCount = 0;
        int displayMemberCallCount = 0;
        EventHandler dataSourceHandler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            dataSourceCallCount++;
        };
        EventHandler displayMemberHandler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            displayMemberCallCount++;
        };
        control.DataSourceChanged += dataSourceHandler;
        control.DisplayMemberChanged += displayMemberHandler;

        // Set different.
        List<int> dataSource1 = [];
        control.DataSource = dataSource1;
        Assert.Same(dataSource1, control.DataSource);
        Assert.Equal(1, dataSourceCallCount);
        Assert.Equal(0, displayMemberCallCount);

        // Set same.
        control.DataSource = dataSource1;
        Assert.Same(dataSource1, control.DataSource);
        Assert.Equal(1, dataSourceCallCount);
        Assert.Equal(0, displayMemberCallCount);

        // Set different.
        List<int> dataSource2 = [];
        control.DataSource = dataSource2;
        Assert.Same(dataSource2, control.DataSource);
        Assert.Equal(2, dataSourceCallCount);
        Assert.Equal(0, displayMemberCallCount);

        // Set null.
        control.DataSource = null;
        Assert.Null(control.DataSource);
        Assert.Equal(3, dataSourceCallCount);
        Assert.Equal(0, displayMemberCallCount);

        // Remove handler.
        control.DataSourceChanged -= dataSourceHandler;
        control.DisplayMemberChanged -= displayMemberHandler;
        control.DataSource = dataSource1;
        Assert.Same(dataSource1, control.DataSource);
        Assert.Equal(3, dataSourceCallCount);
        Assert.Equal(0, displayMemberCallCount);
    }

    [WinFormsFact]
    public void ListControl_DataSource_SetInsideDataSourceChanged_Nop()
    {
        using SubListControl control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            control.DataSource = Array.Empty<int>();
            callCount++;
        };
        control.DataSourceChanged += handler;

        List<int> value = [];
        control.DataSource = value;
        Assert.Same(value, control.DataSource);
        Assert.Empty(control.DisplayMember);
        Assert.Equal(1, callCount);
    }

    [WinFormsFact]
    public void ListControl_DataSource_SetInvalid_ThrowsArgumentException()
    {
        using SubListControl control = new();
        Assert.Throws<ArgumentException>("value", () => control.DataSource = new object());
    }

    [WinFormsTheory]
    [NormalizedStringData]
    public void ListControl_DisplayMember_Set_GetReturnsExpected(string value, string expected)
    {
        using SubListControl control = new()
        {
            DisplayMember = value
        };
        Assert.Null(control.DataSource);
        Assert.Equal(expected, control.DisplayMember);
        Assert.Null(control.DataManager);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.DisplayMember = value;
        Assert.Null(control.DataSource);
        Assert.Equal(expected, control.DisplayMember);
        Assert.Null(control.DataManager);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [NormalizedStringData]
    public void ListControl_DataMember_SetWithDisplayMember_GetReturnsExpected(string value, string expected)
    {
        using SubListControl control = new()
        {
            DisplayMember = "DataMember"
        };

        control.DisplayMember = value;
        Assert.Equal(expected, control.DisplayMember);
        Assert.Null(control.DataSource);
        Assert.Null(control.DataManager);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.DisplayMember = value;
        Assert.Equal(expected, control.DisplayMember);
        Assert.Null(control.DataSource);
        Assert.Null(control.DataManager);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [StringData]
    public void ListControl_DisplayMember_SetWithDataSource_GetReturnsExpected(string value)
    {
        List<int> dataSource = [];
        using SubListControl control = new()
        {
            DataSource = dataSource
        };

        control.DisplayMember = value;
        Assert.Equal(value, control.DisplayMember);
        Assert.Same(dataSource, control.DataSource);
        Assert.Null(control.DataManager);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.DisplayMember = value;
        Assert.Equal(value, control.DisplayMember);
        Assert.Same(dataSource, control.DataSource);
        Assert.Null(control.DataManager);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [NormalizedStringData]
    public void ListControl_DisplayMember_SetWithBindingContext_GetReturnsExpected(string value, string expected)
    {
        BindingContext context = [];
        using SubListControl control = new()
        {
            BindingContext = context
        };

        control.DisplayMember = value;
        Assert.Equal(expected, control.DisplayMember);
        Assert.Null(control.DataSource);
        Assert.Null(control.DataManager);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.DisplayMember = value;
        Assert.Equal(expected, control.DisplayMember);
        Assert.Null(control.DataSource);
        Assert.Null(control.DataManager);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [StringData]
    public void ListControl_DisplayMember_SetWithBindingContextWithDataSource_GetReturnsExpected(string value)
    {
        BindingContext context = [];
        List<int> dataSource = [];
        using SubListControl control = new()
        {
            BindingContext = context,
            DataSource = dataSource
        };

        control.DisplayMember = value;
        Assert.Empty(control.DisplayMember);
        Assert.Same(dataSource, control.DataSource);
        Assert.Same(context[dataSource], control.DataManager);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.DisplayMember = value;
        Assert.Empty(control.DisplayMember);
        Assert.Same(dataSource, control.DataSource);
        Assert.Same(context[dataSource], control.DataManager);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(null, "")]
    [InlineData("", "")]
    [InlineData("Value", "Value")]
    [InlineData("value", "value")]
    public void ListControl_DisplayMember_SetWithBindingContextWithDataSourceCanCreate_GetReturnsExpected(string value, string expected)
    {
        BindingContext context = [];
        List<DataClass> dataSource = [];
        using SubListControl control = new()
        {
            BindingContext = context,
            DataSource = dataSource
        };

        control.DisplayMember = value;
        Assert.Equal(expected, control.DisplayMember);
        Assert.Equal(dataSource, control.DataSource);
        Assert.Same(context[dataSource], control.DataManager);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.DisplayMember = value;
        Assert.Equal(expected, control.DisplayMember);
        Assert.Equal(dataSource, control.DataSource);
        Assert.Same(context[dataSource], control.DataManager);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [StringData]
    [InlineData("ListValue")]
    public void ListControl_DisplayMember_SetWithBindingContextWithDataSourceCantCreate_GetReturnsExpected(string value)
    {
        BindingContext context = [];
        List<DataClass> dataSource = [];
        using SubListControl control = new()
        {
            BindingContext = context,
            DataSource = dataSource
        };

        control.DisplayMember = value;
        Assert.Same(dataSource, control.DataSource);
        Assert.Empty(control.DisplayMember);
        if (value is null)
        {
            Assert.Null(control.DataManager);
        }
        else
        {
            Assert.Same(context[dataSource], control.DataManager);
        }

        Assert.False(control.IsHandleCreated);

        // Set same.
        control.DisplayMember = value;
        Assert.Same(dataSource, control.DataSource);
        Assert.Empty(control.DisplayMember);
        if (value is null)
        {
            Assert.Null(control.DataManager);
        }
        else
        {
            Assert.Same(context[dataSource], control.DataManager);
        }

        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListControl_DisplayMember_SetWithHandler_CallsDisplayMemberChanged()
    {
        using SubListControl control = new();
        int displayMemberCallCount = 0;
        int dataSourceCallCount = 0;
        EventHandler displayMemberHandler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            displayMemberCallCount++;
        };
        EventHandler dataSourceHandler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            dataSourceCallCount++;
        };
        control.DisplayMemberChanged += displayMemberHandler;
        control.DataSourceChanged += dataSourceHandler;

        // Set different.
        control.DisplayMember = "Value1";
        Assert.Equal("Value1", control.DisplayMember);
        Assert.Equal(1, displayMemberCallCount);
        Assert.Equal(0, dataSourceCallCount);

        // Set same.
        control.DisplayMember = "Value1";
        Assert.Equal("Value1", control.DisplayMember);
        Assert.Equal(1, displayMemberCallCount);
        Assert.Equal(0, dataSourceCallCount);

        // Set different.
        control.DisplayMember = "Value2";
        Assert.Equal("Value2", control.DisplayMember);
        Assert.Equal(2, displayMemberCallCount);
        Assert.Equal(0, dataSourceCallCount);

        // Set null.
        control.DisplayMember = null;
        Assert.Empty(control.DisplayMember);
        Assert.Equal(3, displayMemberCallCount);
        Assert.Equal(0, dataSourceCallCount);

        // Set empty.
        control.DisplayMember = string.Empty;
        Assert.Empty(control.DisplayMember);
        Assert.Equal(3, displayMemberCallCount);
        Assert.Equal(0, dataSourceCallCount);

        // Remove handler.
        control.DisplayMemberChanged -= displayMemberHandler;
        control.DataSourceChanged -= dataSourceHandler;
        control.DisplayMember = "Value1";
        Assert.Equal("Value1", control.DisplayMember);
        Assert.Equal(3, displayMemberCallCount);
        Assert.Equal(0, dataSourceCallCount);
    }

    [WinFormsFact]
    public void ListControl_DisplayMember_SetInsideDisplayMemberChanged_Nop()
    {
        using SubListControl control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            control.DisplayMember = "Value2";
            callCount++;
        };
        control.DisplayMemberChanged += handler;

        control.DisplayMember = "Value1";
        Assert.Equal("Value1", control.DisplayMember);
        Assert.Equal(1, callCount);
    }

    [WinFormsFact]
    public void ListControl_Format_AddRemove_CallsRefreshItems()
    {
        using SubListControl control = new();
        int callCount = 0;
        control.RefreshItemsHandler = () => callCount++;
        ListControlConvertEventHandler handler = (sender, e) => { };

        // Add.
        control.Format += handler;
        Assert.Equal(1, callCount);

        // Remove.
        control.Format -= handler;
        Assert.Equal(2, callCount);
    }

    public static IEnumerable<object[]> FormatInfo_Set_TestData()
    {
        yield return new object[] { null };
        yield return new object[] { CultureInfo.CurrentCulture };
    }

    [WinFormsTheory]
    [MemberData(nameof(FormatInfo_Set_TestData))]
    public void ListControl_FormatInfo_Set_GetReturnsExpected(IFormatProvider value)
    {
        using SubListControl control = new()
        {
            FormatInfo = value
        };
        Assert.Same(value, control.FormatInfo);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.FormatInfo = value;
        Assert.Same(value, control.FormatInfo);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(FormatInfo_Set_TestData))]
    public void ListControl_FormatInfo_SetWithCustomOldValue_GetReturnsExpected(IFormatProvider value)
    {
        using SubListControl control = new()
        {
            FormatInfo = CultureInfo.InvariantCulture
        };

        control.FormatInfo = value;
        Assert.Same(value, control.FormatInfo);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.FormatInfo = value;
        Assert.Same(value, control.FormatInfo);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListControl_FormatInfo_Set_CallsRefreshItems()
    {
        using SubListControl control = new();
        int callCount = 0;
        control.RefreshItemsHandler = () => callCount++;

        // Set different.
        control.FormatInfo = CultureInfo.CurrentCulture;
        Assert.Equal(CultureInfo.CurrentCulture, control.FormatInfo);
        Assert.Equal(1, callCount);

        // Set same.
        control.FormatInfo = CultureInfo.CurrentCulture;
        Assert.Equal(CultureInfo.CurrentCulture, control.FormatInfo);
        Assert.Equal(1, callCount);

        // Set different.
        control.FormatInfo = CultureInfo.InvariantCulture;
        Assert.Equal(CultureInfo.InvariantCulture, control.FormatInfo);
        Assert.Equal(2, callCount);

        // Set null.
        control.FormatInfo = null;
        Assert.Null(control.FormatInfo);
        Assert.Equal(3, callCount);
    }

    [WinFormsFact]
    public void ListControl_FormatInfo_SetWithHandler_CallsOnFormatInfoChanged()
    {
        using SubListControl control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        control.FormatInfoChanged += handler;

        // Set different.
        control.FormatInfo = CultureInfo.CurrentCulture;
        Assert.Equal(CultureInfo.CurrentCulture, control.FormatInfo);
        Assert.Equal(1, callCount);

        // Set same.
        control.FormatInfo = CultureInfo.CurrentCulture;
        Assert.Equal(CultureInfo.CurrentCulture, control.FormatInfo);
        Assert.Equal(1, callCount);

        // Set different.
        control.FormatInfo = CultureInfo.InvariantCulture;
        Assert.Equal(CultureInfo.InvariantCulture, control.FormatInfo);
        Assert.Equal(2, callCount);

        // Set null.
        control.FormatInfo = null;
        Assert.Null(control.FormatInfo);
        Assert.Equal(3, callCount);

        // Remove handler.
        control.FormatInfoChanged -= handler;
        control.FormatInfo = CultureInfo.CurrentCulture;
        Assert.Equal(CultureInfo.CurrentCulture, control.FormatInfo);
        Assert.Equal(3, callCount);
    }

    [WinFormsTheory]
    [NormalizedStringData]
    public void ListControl_FormatString_Set_GetReturnsExpected(string value, string expected)
    {
        using SubListControl control = new()
        {
            FormatString = value
        };
        Assert.Equal(expected, control.FormatString);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.FormatString = value;
        Assert.Equal(expected, control.FormatString);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [NormalizedStringData]
    public void ListControl_FormatString_SetWithCustomOldValue_GetReturnsExpected(string value, string expected)
    {
        using SubListControl control = new()
        {
            FormatString = "FormatString"
        };

        control.FormatString = value;
        Assert.Equal(expected, control.FormatString);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.FormatString = value;
        Assert.Equal(expected, control.FormatString);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListControl_FormatString_Set_CallsRefreshItems()
    {
        using SubListControl control = new();
        int callCount = 0;
        control.RefreshItemsHandler = () => callCount++;

        // Set different.
        control.FormatString = "Value1";
        Assert.Equal("Value1", control.FormatString);
        Assert.Equal(1, callCount);

        // Set same.
        control.FormatString = "Value1";
        Assert.Equal("Value1", control.FormatString);
        Assert.Equal(1, callCount);

        // Set different.
        control.FormatString = "Value2";
        Assert.Equal("Value2", control.FormatString);
        Assert.Equal(2, callCount);

        // Set null.
        control.FormatString = null;
        Assert.Empty(control.FormatString);
        Assert.Equal(3, callCount);

        // Set empty.
        control.FormatString = string.Empty;
        Assert.Empty(control.FormatString);
        Assert.Equal(3, callCount);
    }

    [WinFormsFact]
    public void ListControl_FormatString_SetWithHandler_CallsOnFormatStringChanged()
    {
        using SubListControl control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        control.FormatStringChanged += handler;

        // Set different.
        control.FormatString = "Value1";
        Assert.Equal("Value1", control.FormatString);
        Assert.Equal(1, callCount);

        // Set same.
        control.FormatString = "Value1";
        Assert.Equal("Value1", control.FormatString);
        Assert.Equal(1, callCount);

        // Set different.
        control.FormatString = "Value2";
        Assert.Equal("Value2", control.FormatString);
        Assert.Equal(2, callCount);

        // Set null.
        control.FormatString = null;
        Assert.Empty(control.FormatString);
        Assert.Equal(3, callCount);

        // Set empty.
        control.FormatString = string.Empty;
        Assert.Empty(control.FormatString);
        Assert.Equal(3, callCount);

        // Remove handler.
        control.FormatStringChanged -= handler;
        control.FormatString = "Value1";
        Assert.Equal("Value1", control.FormatString);
        Assert.Equal(3, callCount);
    }

    [WinFormsTheory]
    [BoolData]
    public void ListControl_FormattingEnabled_Set_GetReturnsExpected(bool value)
    {
        using SubListControl control = new()
        {
            FormattingEnabled = value
        };
        Assert.Equal(value, control.FormattingEnabled);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.FormattingEnabled = value;
        Assert.Equal(value, control.FormattingEnabled);
        Assert.False(control.IsHandleCreated);

        // Set different.
        control.FormattingEnabled = !value;
        Assert.Equal(!value, control.FormattingEnabled);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [BoolData]
    public void ListControl_FormattingEnabled_SetWithFormattingEnabled_GetReturnsExpected(bool value)
    {
        using SubListControl control = new()
        {
            FormattingEnabled = true
        };

        control.FormattingEnabled = value;
        Assert.Equal(value, control.FormattingEnabled);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.FormattingEnabled = value;
        Assert.Equal(value, control.FormattingEnabled);
        Assert.False(control.IsHandleCreated);

        // Set different.
        control.FormattingEnabled = !value;
        Assert.Equal(!value, control.FormattingEnabled);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListControl_FormattingEnabled_Set_CallsRefreshItems()
    {
        using SubListControl control = new();
        int callCount = 0;
        control.RefreshItemsHandler = () => callCount++;

        // Set different.
        control.FormattingEnabled = true;
        Assert.True(control.FormattingEnabled);
        Assert.Equal(1, callCount);

        // Set same.
        control.FormattingEnabled = true;
        Assert.True(control.FormattingEnabled);
        Assert.Equal(1, callCount);

        // Set different.
        control.FormattingEnabled = false;
        Assert.False(control.FormattingEnabled);
        Assert.Equal(2, callCount);
    }

    [WinFormsFact]
    public void ListControl_FormattingEnabled_SetWithHandler_CallsOnFormattingEnabledChanged()
    {
        using SubListControl control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        control.FormattingEnabledChanged += handler;

        // Set different.
        control.FormattingEnabled = true;
        Assert.True(control.FormattingEnabled);
        Assert.Equal(1, callCount);

        // Set same.
        control.FormattingEnabled = true;
        Assert.True(control.FormattingEnabled);
        Assert.Equal(1, callCount);

        // Set different.
        control.FormattingEnabled = false;
        Assert.False(control.FormattingEnabled);
        Assert.Equal(2, callCount);

        // Remove handler.
        control.FormattingEnabledChanged -= handler;
        control.FormattingEnabled = true;
        Assert.True(control.FormattingEnabled);
        Assert.Equal(2, callCount);
    }

    [WinFormsTheory]
    [NormalizedStringData]
    public void ListControl_ValueMember_Set_GetReturnsExpected(string value, string expected)
    {
        using SubListControl control = new()
        {
            ValueMember = value
        };
        Assert.Null(control.DataSource);
        Assert.Equal(expected, control.DisplayMember);
        Assert.Equal(expected, control.ValueMember);
        Assert.Null(control.DataManager);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.ValueMember = value;
        Assert.Null(control.DataSource);
        Assert.Equal(expected, control.DisplayMember);
        Assert.Equal(expected, control.ValueMember);
        Assert.Null(control.DataManager);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [NormalizedStringData]
    public void ListControl_ValueMember_SetWithDisplayMember_GetReturnsExpected(string value, string expected)
    {
        using SubListControl control = new()
        {
            DisplayMember = "DisplayMember"
        };

        control.ValueMember = value;
        Assert.Null(control.DataSource);
        Assert.Equal("DisplayMember", control.DisplayMember);
        Assert.Equal(expected, control.ValueMember);
        Assert.Null(control.DataManager);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.ValueMember = value;
        Assert.Null(control.DataSource);
        Assert.Equal("DisplayMember", control.DisplayMember);
        Assert.Equal(expected, control.ValueMember);
        Assert.Null(control.DataManager);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [NormalizedStringData]
    public void ListControl_ValueMember_SetWithBindingContext_GetReturnsExpected(string value, string expected)
    {
        BindingContext context = [];
        using SubListControl control = new()
        {
            BindingContext = context
        };

        control.ValueMember = value;
        Assert.Null(control.DataSource);
        Assert.Equal(expected, control.DisplayMember);
        Assert.Equal(expected, control.ValueMember);
        Assert.Null(control.DataManager);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.ValueMember = value;
        Assert.Null(control.DataSource);
        Assert.Equal(expected, control.DisplayMember);
        Assert.Equal(expected, control.ValueMember);
        Assert.Null(control.DataManager);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(null, "")]
    [InlineData("", "")]
    [InlineData("Value", "Value")]
    [InlineData("value", "value")]
    public void ListControl_ValueMember_SetWithBindingContextWithDataSourceCanCreate_GetReturnsExpected(string value, string expected)
    {
        BindingContext context = [];
        List<DataClass> dataSource = [];
        using SubListControl control = new()
        {
            BindingContext = context,
            DataSource = dataSource
        };

        control.ValueMember = value;
        Assert.Same(dataSource, control.DataSource);
        Assert.Equal(expected, control.DisplayMember);
        Assert.Equal(expected, control.ValueMember);
        Assert.Same(context[dataSource], control.DataManager);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.ValueMember = value;
        Assert.Same(dataSource, control.DataSource);
        Assert.Equal(expected, control.DisplayMember);
        Assert.Equal(expected, control.ValueMember);
        Assert.Same(context[dataSource], control.DataManager);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListControl_ValueMember_SetWithBindingContextWithDataSourceCantCreate_ThrowsArgumentException()
    {
        BindingContext context = [];
        List<DataClass> dataSource = [];
        using SubListControl control = new()
        {
            BindingContext = context,
            DataSource = dataSource
        };

        Assert.Throws<ArgumentException>("newDisplayMember", (() => control.ValueMember = "NoSuchValue"));
        Assert.Equal("NoSuchValue", control.DisplayMember);
        Assert.Empty(control.ValueMember);
    }

    [WinFormsTheory]
    [InlineData(null, "")]
    [InlineData("", "")]
    [InlineData("Value", "Value")]
    [InlineData("value", "value")]
    public void ListControl_ValueMember_SetWithBindingContextWithDataSourceWithDisplayMemberCanCreate_GetReturnsExpected(string value, string expected)
    {
        BindingContext context = [];
        List<DataClass> dataSource = [];
        using SubListControl control = new()
        {
            BindingContext = context,
            DataSource = dataSource,
            DisplayMember = "OtherValue"
        };

        control.ValueMember = value;
        Assert.Same(dataSource, control.DataSource);
        Assert.Equal("OtherValue", control.DisplayMember);
        Assert.Equal(expected, control.ValueMember);
        Assert.Same(context[dataSource], control.DataManager);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.ValueMember = value;
        Assert.Same(dataSource, control.DataSource);
        Assert.Equal("OtherValue", control.DisplayMember);
        Assert.Equal(expected, control.ValueMember);
        Assert.Same(context[dataSource], control.DataManager);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListControl_ValueMember_SetWithBindingContextWithDataSourceWithDisplayMemberCantCreate_ThrowsArgumentException()
    {
        BindingContext context = [];
        List<DataClass> dataSource = [];
        using SubListControl control = new()
        {
            BindingContext = context,
            DataSource = dataSource,
            DisplayMember = "Value"
        };

        Assert.Throws<ArgumentException>("value", (() => control.ValueMember = "NoSuchValue"));
        Assert.Equal("Value", control.DisplayMember);
        Assert.Empty(control.ValueMember);
    }

    [WinFormsFact]
    public void ListControl_ValueMember_SetWithHandler_CallsValueMemberChanged()
    {
        using SubListControl control = new();
        int valueMemberCallCount = 0;
        int selectedValueCallCount = 0;
        EventHandler valueMemberHandler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            valueMemberCallCount++;
        };
        EventHandler selectedValueHandler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.True(valueMemberCallCount > selectedValueCallCount);
            selectedValueCallCount++;
        };
        control.ValueMemberChanged += valueMemberHandler;
        control.SelectedValueChanged += selectedValueHandler;

        // Set different.
        control.ValueMember = "Value1";
        Assert.Equal("Value1", control.ValueMember);
        Assert.Equal(1, valueMemberCallCount);
        Assert.Equal(1, selectedValueCallCount);

        // Set same.
        control.ValueMember = "Value1";
        Assert.Equal("Value1", control.ValueMember);
        Assert.Equal(1, valueMemberCallCount);
        Assert.Equal(1, selectedValueCallCount);

        // Set different.
        control.ValueMember = "Value2";
        Assert.Equal("Value2", control.ValueMember);
        Assert.Equal(2, valueMemberCallCount);
        Assert.Equal(2, selectedValueCallCount);

        // Set null.
        control.ValueMember = null;
        Assert.Empty(control.ValueMember);
        Assert.Equal(3, valueMemberCallCount);
        Assert.Equal(3, selectedValueCallCount);

        // Set empty.
        control.ValueMember = string.Empty;
        Assert.Empty(control.ValueMember);
        Assert.Equal(3, valueMemberCallCount);
        Assert.Equal(3, selectedValueCallCount);

        // Remove handler.
        control.ValueMemberChanged -= valueMemberHandler;
        control.SelectedValueChanged -= selectedValueHandler;
        control.ValueMember = "Value1";
        Assert.Equal("Value1", control.ValueMember);
        Assert.Equal(3, valueMemberCallCount);
        Assert.Equal(3, selectedValueCallCount);
    }

    [WinFormsTheory]
    [InlineData("Value")]
    [InlineData("value")]
    public void ListControl_SelectedValue_SetWithMatchingValue_Success(string valueMember)
    {
        BindingContext context = [];
        List<DataClass> dataSource = [new DataClass { Value = "StringValue" }];
        using SubListControl control = new()
        {
            SelectedIndex = 0,
            ValueMember = valueMember,
            BindingContext = context,
            DataSource = dataSource
        };

        control.SelectedValue = "StringValue";
        Assert.Equal(0, control.SelectedIndex);
        Assert.Equal("StringValue", control.SelectedValue);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.SelectedValue = "StringValue";
        Assert.Equal(0, control.SelectedIndex);
        Assert.Equal("StringValue", control.SelectedValue);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> SelectedValue_NoMatchingValue_TestData()
    {
        yield return new object[] { new List<DataClass>(), "selected" };
        yield return new object[] { new List<DataClass> { new() { Value = "NoSuchValue" } }, string.Empty };
        yield return new object[] { new List<DataClass> { new() { Value = "NoSuchValue" } }, "selected" };
        yield return new object[] { new List<DataClass> { new() { Value = "NoSuchValue" } }, "nosuchvalue" };
    }

    [WinFormsTheory]
    [MemberData(nameof(SelectedValue_NoMatchingValue_TestData))]
    public void ListControl_SelectedValue_SetWithNoMatchingValue_Success(object dataSource, string value)
    {
        BindingContext context = [];
        using SubListControl control = new()
        {
            SelectedIndex = 0,
            ValueMember = "Value",
            BindingContext = context,
            DataSource = dataSource
        };

        control.SelectedValue = value;
        Assert.Equal(-1, control.SelectedIndex);
        Assert.Null(control.SelectedValue);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.SelectedValue = value;
        Assert.Equal(-1, control.SelectedIndex);
        Assert.Null(control.SelectedValue);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListControl_SelectedValue_SetWithChangedDataManager_Success()
    {
        BindingContext context = [];
        List<DataClass> dataSource = [];
        using SubListControl control = new()
        {
            SelectedIndex = 0,
            ValueMember = "ValueMember",
            BindingContext = context,
            DataSource = dataSource
        };

        control.SelectedValue = "selected";
        Assert.Equal(-1, control.SelectedIndex);
        Assert.Null(control.SelectedValue);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.SelectedValue = "selected";
        Assert.Equal(-1, control.SelectedIndex);
        Assert.Null(control.SelectedValue);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(null)]
    [InlineData(1)]
    public void ListControl_SelectedValue_SetWithoutDataManager_GetReturnsNull(object value)
    {
        using SubListControl control = new()
        {
            SelectedIndex = 0,
            SelectedValue = value
        };
        Assert.Equal(0, control.SelectedIndex);
        Assert.Null(control.SelectedValue);

        // Not set even when given a DataManager.
        control.BindingContext = [];
        control.DataSource = new List<int>();
        Assert.Equal(0, control.SelectedIndex);
        Assert.Throws<IndexOutOfRangeException>(() => control.SelectedValue);
    }

    [WinFormsFact]
    public void ListControl_SelectedValue_SetWithHandler_DoesNotCallSelectedValueChanged()
    {
        BindingContext context = [];
        List<DataClass> dataSource = [new DataClass { Value = "StringValue" }];
        using SubListControl control = new()
        {
            SelectedIndex = 0,
            ValueMember = "Value",
            BindingContext = context,
            DataSource = dataSource
        };

        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        control.SelectedValueChanged += handler;

        // Set different.
        control.SelectedValue = "StringValue";
        Assert.Equal("StringValue", control.SelectedValue);
        Assert.Equal(0, callCount);

        // Set same.
        control.SelectedValue = "StringValue";
        Assert.Equal("StringValue", control.SelectedValue);
        Assert.Equal(0, callCount);

        // Remove handler.
        control.SelectedValueChanged -= handler;
        control.SelectedValue = "StringValue";
        Assert.Equal("StringValue", control.SelectedValue);
        Assert.Equal(0, callCount);
    }

    [WinFormsFact]
    public void ListControl_SelectedValue_SetNull_ThrowsArgumentNullException()
    {
        BindingContext context = [];
        List<DataClass> dataSource = [];
        using SubListControl control = new()
        {
            SelectedIndex = 0,
            BindingContext = context,
            DataSource = dataSource,
            ValueMember = "Value"
        };
        Assert.Throws<ArgumentNullException>("key", () => control.SelectedValue = null);
        Assert.Equal(-1, control.SelectedIndex);
        Assert.Null(control.SelectedValue);
    }

    [WinFormsTheory]
    [InlineData(null)]
    [InlineData(1)]
    public void ListControl_SelectedValue_SetWithoutValueMember_ThrowsInvalidOperationException(object value)
    {
        BindingContext context = [];
        List<DataClass> dataSource = [];
        using SubListControl control = new()
        {
            SelectedIndex = 0,
            BindingContext = context,
            DataSource = dataSource
        };
        Assert.Throws<InvalidOperationException>(() => control.SelectedValue = value);
        Assert.Equal(0, control.SelectedIndex);
        Assert.Throws<IndexOutOfRangeException>(() => control.SelectedValue);
    }

    [WinFormsTheory]
    [InlineData(Keys.Alt, false)]
    [InlineData(Keys.Alt | Keys.PageUp, false)]
    [InlineData(Keys.PageUp, true)]
    [InlineData(Keys.PageDown, true)]
    [InlineData(Keys.Home, true)]
    [InlineData(Keys.End, true)]
    [InlineData(Keys.A, false)]
    public void IsInputKey_Invoke_ReturnsExpected(Keys keyData, bool expected)
    {
        using SubListControl control = new();
        Assert.Equal(expected, control.IsInputKey(keyData));
    }

    public static IEnumerable<object[]> FilterItemOnProperty_TestData()
    {
        yield return new object[]
        {
            new SubListControl(),
            null,
            null
        };

        object item = new();
        yield return new object[]
        {
            new SubListControl(),
            item,
            item
        };

        yield return new object[]
        {
            new SubListControl { DisplayMember = "Length" },
            "abc",
            3
        };
        yield return new object[]
        {
            new SubListControl { DisplayMember = "length" },
            "abc",
            3
        };
        yield return new object[]
        {
            new SubListControl { DisplayMember = "Path.Length" },
            "abc",
            3
        };
        yield return new object[]
        {
            new SubListControl { DisplayMember = "NoSuchProperty" },
            "abc",
            "abc"
        };

        DataClass dataClass = new() { Value = 10 };
        List<DataClass> list = [dataClass];
        yield return new object[]
        {
            new SubListControl
            {
                DataSource = list
            },
            list,
            list
        };

        yield return new object[]
        {
            new SubListControl
            {
                BindingContext = [],
                DataSource = list
            },
            list,
            list
        };
        yield return new object[]
        {
            new SubListControl
            {
                BindingContext = [],
                DataSource = list,
                DisplayMember = "Value"
            },
            list,
            list
        };
        yield return new object[]
        {
            new SubListControl
            {
                BindingContext = [],
                DataSource = list,
                DisplayMember = "Value"
            },
            dataClass,
            10
        };
        yield return new object[]
        {
            new SubListControl
            {
                BindingContext = [],
                DataSource = list,
                DisplayMember = "value"
            },
            dataClass,
            10
        };
        yield return new object[]
        {
            new SubListControl
            {
                BindingContext = [],
                DataSource = list,
                DisplayMember = "NoSuchProperty"
            },
            dataClass,
            dataClass
        };
    }

    [WinFormsTheory]
    [MemberData(nameof(FilterItemOnProperty_TestData))]
    public void ListControl_FilterItemOnProperty_Invoke_ReturnsExpected(SubListControl control, object item, object expected)
    {
        Assert.Equal(expected, control.FilterItemOnProperty(item));
    }

    public static IEnumerable<object[]> FilterItemOnProperty_String_TestData()
    {
        yield return new object[]
        {
            new SubListControl(),
            null,
            "Field",
            null
        };

        object item = new();
        yield return new object[]
        {
            new SubListControl(),
            item,
            null,
            item
        };
        yield return new object[]
        {
            new SubListControl(),
            item,
            string.Empty,
            item
        };

        yield return new object[]
        {
            new SubListControl(),
            "abc",
            "Length",
            3
        };
        yield return new object[]
        {
            new SubListControl(),
            "abc",
            "length",
            3
        };
        yield return new object[]
        {
            new SubListControl(),
            "abc",
            "NoSuchProperty",
            "abc"
        };

        DataClass dataClass = new() { Value = 10 };
        List<DataClass> list = [dataClass];
        yield return new object[]
        {
            new SubListControl
            {
                BindingContext = [],
                DataSource = list
            },
            list,
            "NoSuchField",
            list
        };
        yield return new object[]
        {
            new SubListControl
            {
                BindingContext = [],
                DataSource = list
            },
            list,
            "Value",
            list
        };
        yield return new object[]
        {
            new SubListControl
            {
                BindingContext = [],
                DataSource = list
            },
            dataClass,
            "Value",
            10
        };
        yield return new object[]
        {
            new SubListControl
            {
                BindingContext = [],
                DataSource = list
            },
            dataClass,
            "value",
            10
        };
        yield return new object[]
        {
            new SubListControl
            {
                BindingContext = [],
                DataSource = list
            },
            dataClass,
            "NoSuchProperty",
            dataClass
        };
    }

    [WinFormsTheory]
    [MemberData(nameof(FilterItemOnProperty_String_TestData))]
    public void ListControl_FilterItemOnProperty_InvokeString_ReturnsExpected(SubListControl control, object item, string field, object expected)
    {
        Assert.Equal(expected, control.FilterItemOnProperty(item, field));
    }

    public static IEnumerable<object[]> GetItemText_TestData()
    {
        yield return new object[]
        {
            new SubListControl(), null, string.Empty
        };
        yield return new object[]
        {
            new SubListControl(),
            "abc",
            "abc"
        };
        yield return new object[]
        {
            new SubListControl
            {
                DisplayMember = "Length"
            },
            "abc",
            3
        };
        yield return new object[]
        {
            new SubListControl
            {
                DisplayMember = "Value"
            },
            new DataClass { Value = 1 },
            "1"
        };
        yield return new object[]
        {
            new SubListControl
            {
                DisplayMember = "Value"
            },
            new DataClass { Value = null },
            string.Empty
        };
    }

    [WinFormsTheory]
    [MemberData(nameof(GetItemText_TestData))]
    public void ListControl_GetItemText_Invoke_ReturnsExpected(ListControl control, object item, string expected)
    {
        Assert.Equal(expected, control.GetItemText(item));

        // Test caching behavior.
        Assert.Equal(expected, control.GetItemText(item));
    }

    public static IEnumerable<object[]> GetItemText_HasHandler_TestData()
    {
        DataClass item = new() { Value = 3 };
        yield return new object[] { item, null, "3" };
        yield return new object[] { item, new(), "3" };
        yield return new object[] { item, item, "3" };
        yield return new object[] { item, "custom", "custom" };
    }

    [WinFormsTheory]
    [MemberData(nameof(GetItemText_HasHandler_TestData))]
    public void ListControl_GetItemText_HasHandler_CallsFormat(object item, object value, object expected)
    {
        using SubListControl control = new()
        {
            FormattingEnabled = true,
            DisplayMember = "Value"
        };

        // Handler.
        int callCount = 0;
        ListControlConvertEventHandler handler = (sender, e) =>
        {
            Assert.Equal(control, sender);
            Assert.Equal(3, e.Value);
            Assert.Equal(typeof(string), e.DesiredType);
            Assert.Same(item, e.ListItem);

            e.Value = value;
            callCount++;
        };

        control.Format += handler;
        Assert.Equal(expected, control.GetItemText(item));
        Assert.Equal(1, callCount);

        // Should not call if the handler is removed.
        control.Format -= handler;
        Assert.Equal("3", control.GetItemText(item));
        Assert.Equal(1, callCount);
    }

    public static IEnumerable<object[]> GetItemText_HasHandlerFormattingDisabled_TestData()
    {
        DataClass item = new() { Value = 3 };
        yield return new object[] { item, null };
        yield return new object[] { item, new() };
        yield return new object[] { item, item };
        yield return new object[] { item, "custom" };
    }

    [WinFormsTheory]
    [MemberData(nameof(GetItemText_HasHandlerFormattingDisabled_TestData))]
    public void ListControl_GetItemText_HasHandlerFormattingDisabled_DoesNotCallFormat(object item, object value)
    {
        using SubListControl control = new()
        {
            FormattingEnabled = false,
            DisplayMember = "Value"
        };

        // Handler.
        int callCount = 0;
        ListControlConvertEventHandler handler = (sender, e) =>
        {
            Assert.Equal(control, sender);
            Assert.Equal(3, e.Value);
            Assert.Equal(typeof(string), e.DesiredType);
            Assert.Same(item, e.ListItem);

            e.Value = value;
            callCount++;
        };

        control.Format += handler;
        Assert.Equal("3", control.GetItemText(item));
        Assert.Equal(0, callCount);

        // Should not call if the handler is removed.
        control.Format -= handler;
        Assert.Equal("3", control.GetItemText(item));
        Assert.Equal(0, callCount);
    }

    [WinFormsFact]
    public void ListControl_GetItemText_CustomConverter_ReturnsExpected()
    {
        CustomTypeConverterDataClass item = new() { Value = 10 };
        using SubListControl control = new()
        {
            FormattingEnabled = true,
            BindingContext = [],
            DataSource = new List<CustomTypeConverterDataClass> { item },
            DisplayMember = "Value"
        };
        Assert.Equal("custom", control.GetItemText(item));

        // Test caching behavior.
        Assert.Equal("custom", control.GetItemText(item));
    }

    [WinFormsFact]
    public void ListControl_GetItemText_CustomConverterFormattingDisabled_ReturnsExpected()
    {
        CustomTypeConverterDataClass item = new() { Value = 10 };
        using SubListControl control = new()
        {
            FormattingEnabled = false,
            BindingContext = [],
            DataSource = new List<CustomTypeConverterDataClass> { item },
            DisplayMember = "Value"
        };
        Assert.Equal("10", control.GetItemText(item));

        // Test caching behavior.
        Assert.Equal("10", control.GetItemText(item));
    }

    [WinFormsFact]
    public void ListControl_GetItemText_CustomConverterNoContext_ReturnsExpected()
    {
        CustomTypeConverterDataClass item = new() { Value = 10 };
        using SubListControl control = new()
        {
            FormattingEnabled = true,
            DataSource = new List<CustomTypeConverterDataClass> { item },
            DisplayMember = "Value"
        };
        Assert.Equal("10", control.GetItemText(item));

        // Test caching behavior.
        Assert.Equal("10", control.GetItemText(item));
    }

    [WinFormsFact]
    public void ListControl_GetItemText_NonCriticalThrowingExceptionType_ReturnsExpected()
    {
        NonCriticalThrowingTypeConverterDataClass item = new();
        using SubListControl control = new()
        {
            FormattingEnabled = true,
            BindingContext = [],
            DataSource = new List<NonCriticalThrowingTypeConverterDataClass> { item },
            DisplayMember = "Value"
        };
        Assert.Equal("NonCriticalThrowingTypeConverterDataClassToString", control.GetItemText(item));

        // Test caching behavior.
        Assert.Equal("NonCriticalThrowingTypeConverterDataClassToString", control.GetItemText(item));
    }

    [WinFormsFact]
    public void ListControl_GetItemText_CriticalThrowingExceptionType_RethrowsException()
    {
        CriticalThrowingTypeConverterDataClass item = new();
        using SubListControl control = new()
        {
            FormattingEnabled = true,
            BindingContext = [],
            DataSource = new List<CriticalThrowingTypeConverterDataClass> { item },
            DisplayMember = "Value"
        };
        Assert.Throws<StackOverflowException>(() => control.GetItemText(item));

        // Test caching behavior.
        Assert.Throws<StackOverflowException>(() => control.GetItemText(item));
    }

    [WinFormsFact]
    public void ListControl_GetAutoSizeMode_Invoke_ReturnsExpected()
    {
        using SubListControl control = new();
        Assert.Equal(AutoSizeMode.GrowOnly, control.GetAutoSizeMode());
    }

    [WinFormsTheory]
    [InlineData(ControlStyles.ContainerControl, false)]
    [InlineData(ControlStyles.UserPaint, true)]
    [InlineData(ControlStyles.Opaque, false)]
    [InlineData(ControlStyles.ResizeRedraw, false)]
    [InlineData(ControlStyles.FixedWidth, false)]
    [InlineData(ControlStyles.FixedHeight, false)]
    [InlineData(ControlStyles.StandardClick, true)]
    [InlineData(ControlStyles.Selectable, true)]
    [InlineData(ControlStyles.UserMouse, false)]
    [InlineData(ControlStyles.SupportsTransparentBackColor, false)]
    [InlineData(ControlStyles.StandardDoubleClick, true)]
    [InlineData(ControlStyles.AllPaintingInWmPaint, true)]
    [InlineData(ControlStyles.CacheText, false)]
    [InlineData(ControlStyles.EnableNotifyMessage, false)]
    [InlineData(ControlStyles.DoubleBuffer, false)]
    [InlineData(ControlStyles.OptimizedDoubleBuffer, false)]
    [InlineData(ControlStyles.UseTextForAccessibility, true)]
    [InlineData((ControlStyles)0, true)]
    [InlineData((ControlStyles)int.MaxValue, false)]
    [InlineData((ControlStyles)(-1), false)]
    public void ListControl_GetStyle_Invoke_ReturnsExpected(ControlStyles flag, bool expected)
    {
        using SubListControl control = new();
        Assert.Equal(expected, control.GetStyle(flag));

        // Call again to test caching.
        Assert.Equal(expected, control.GetStyle(flag));
    }

    [WinFormsFact]
    public void ListControl_GetTopLevel_Invoke_ReturnsExpected()
    {
        using SubListControl control = new();
        Assert.False(control.GetTopLevel());
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void ListControl_OnBindingContextChanged_Invoke_CallsBindingContextChanged(EventArgs eventArgs)
    {
        using SubListControl control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.BindingContextChanged += handler;
        control.OnBindingContextChanged(eventArgs);
        Assert.Equal(1, callCount);

        // Remove handler.
        control.BindingContextChanged -= handler;
        control.OnBindingContextChanged(eventArgs);
        Assert.Equal(1, callCount);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void ListControl_OnDataSourceChanged_Invoke_CallsDataSourceChanged(EventArgs eventArgs)
    {
        using SubListControl control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.DataSourceChanged += handler;
        control.OnDataSourceChanged(eventArgs);
        Assert.Equal(1, callCount);

        // Remove handler.
        control.DataSourceChanged -= handler;
        control.OnDataSourceChanged(eventArgs);
        Assert.Equal(1, callCount);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void ListControl_OnDisplayMemberChanged_Invoke_CallsDisplayMemberChanged(EventArgs eventArgs)
    {
        using SubListControl control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.DisplayMemberChanged += handler;
        control.OnDisplayMemberChanged(eventArgs);
        Assert.Equal(1, callCount);

        // Remove handler.
        control.DisplayMemberChanged -= handler;
        control.OnDisplayMemberChanged(eventArgs);
        Assert.Equal(1, callCount);
    }

    public static IEnumerable<object[]> OnFormat_TestData()
    {
        yield return new object[] { null };
        yield return new object[] { new ListControlConvertEventArgs(null, null, null) };
        yield return new object[] { new ListControlConvertEventArgs(new object(), typeof(int), new object()) };
    }

    [WinFormsTheory]
    [MemberData(nameof(OnFormat_TestData))]
    public void ListControl_OnFormat_Invoke_CallsFormat(ListControlConvertEventArgs eventArgs)
    {
        using SubListControl control = new();
        int callCount = 0;
        ListControlConvertEventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.Format += handler;
        control.OnFormat(eventArgs);
        Assert.Equal(1, callCount);

        // Remove handler.
        control.Format -= handler;
        control.OnFormat(eventArgs);
        Assert.Equal(1, callCount);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void ListControl_OnFormatInfoChanged_Invoke_CallsFormatInfoChanged(EventArgs eventArgs)
    {
        using SubListControl control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.FormatInfoChanged += handler;
        control.OnFormatInfoChanged(eventArgs);
        Assert.Equal(1, callCount);

        // Remove handler.
        control.FormatInfoChanged -= handler;
        control.OnFormatInfoChanged(eventArgs);
        Assert.Equal(1, callCount);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void ListControl_OnFormatStringChanged_Invoke_CallsFormatStringChanged(EventArgs eventArgs)
    {
        using SubListControl control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.FormatStringChanged += handler;
        control.OnFormatStringChanged(eventArgs);
        Assert.Equal(1, callCount);

        // Remove handler.
        control.FormatStringChanged -= handler;
        control.OnFormatStringChanged(eventArgs);
        Assert.Equal(1, callCount);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void ListControl_OnFormattingEnabledChanged_Invoke_CallsFormattingEnabledChanged(EventArgs eventArgs)
    {
        using SubListControl control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.FormattingEnabledChanged += handler;
        control.OnFormattingEnabledChanged(eventArgs);
        Assert.Equal(1, callCount);

        // Remove handler.
        control.FormattingEnabledChanged -= handler;
        control.OnFormattingEnabledChanged(eventArgs);
        Assert.Equal(1, callCount);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void ListControl_OnSelectedIndexChanged_Invoke_CallsSelectedValueChanged(EventArgs eventArgs)
    {
        using SubListControl control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.SelectedValueChanged += handler;
        control.OnSelectedIndexChanged(eventArgs);
        Assert.Equal(1, callCount);

        // Remove handler.
        control.SelectedValueChanged -= handler;
        control.OnSelectedIndexChanged(eventArgs);
        Assert.Equal(1, callCount);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void ListControl_OnSelectedValueChanged_Invoke_CallsSelectedValueChanged(EventArgs eventArgs)
    {
        using SubListControl control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.SelectedValueChanged += handler;
        control.OnSelectedValueChanged(eventArgs);
        Assert.Equal(1, callCount);

        // Remove handler.
        control.SelectedValueChanged -= handler;
        control.OnSelectedValueChanged(eventArgs);
        Assert.Equal(1, callCount);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void ListControl_OnValueMemberChanged_Invoke_CallsValueMemberChanged(EventArgs eventArgs)
    {
        using SubListControl control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.ValueMemberChanged += handler;
        control.OnValueMemberChanged(eventArgs);
        Assert.Equal(1, callCount);

        // Remove handler.
        control.ValueMemberChanged -= handler;
        control.OnValueMemberChanged(eventArgs);
        Assert.Equal(1, callCount);
    }

    [WinFormsFact]
    public void ListControl_RefreshItems_Invoke_Nop()
    {
        using SubListControl control = new();
        control.RefreshItemsEntry();
    }

    [WinFormsTheory]
    [InlineData(0, null)]
    [InlineData(-1, 1)]
    public void ListControl_SetItemCore_Invoke_Nop(int index, object value)
    {
        using SubListControl control = new();
        control.SetItemCoreEntry(index, value);
    }

    private class DataClass
    {
        public object Value { get; set; }
        public object OtherValue { get; set; }
        public IList ListValue { get; set; }

        public override string ToString() => "DataClassToString";
    }

    private class ComponentList : List<int>, IComponent
    {
        public ISite Site { get; set; }

        public event EventHandler Disposed;

        public void Dispose() => OnDisposed(this, EventArgs.Empty);

        public void OnDisposed(object sender, EventArgs e)
        {
            Disposed?.Invoke(sender, e);
        }
    }

    private class SupportInitializeNotificationList : List<int>, ISupportInitializeNotification
    {
        public bool IsInitialized { get; set; }

        public event EventHandler Initialized;

        public void BeginInit()
        {
        }

        public void EndInit()
        {
        }

        public void OnInitialized(object sender, EventArgs e)
        {
            Initialized?.Invoke(sender, e);
        }
    }

    public class SubListControl : ListControl
    {
        public override int SelectedIndex { get; set; }

        public bool AllowSelectionEntry => base.AllowSelection;

        public new bool CanEnableIme => base.CanEnableIme;

        public new bool CanRaiseEvents => base.CanRaiseEvents;

        public new CreateParams CreateParams => base.CreateParams;

        public new CurrencyManager DataManager => base.DataManager;

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

        public new AutoSizeMode GetAutoSizeMode() => base.GetAutoSizeMode();

        public new bool GetStyle(ControlStyles flag) => base.GetStyle(flag);

        public new bool GetTopLevel() => base.GetTopLevel();

        public new bool IsInputKey(Keys keyData) => base.IsInputKey(keyData);

        public new object FilterItemOnProperty(object item) => base.FilterItemOnProperty(item);

        public new object FilterItemOnProperty(object item, string field) => base.FilterItemOnProperty(item, field);

        public new void OnBindingContextChanged(EventArgs e) => base.OnBindingContextChanged(e);

        public new void OnDataSourceChanged(EventArgs e) => base.OnDataSourceChanged(e);

        public new void OnDisplayMemberChanged(EventArgs e) => base.OnDisplayMemberChanged(e);

        public new void OnFormat(ListControlConvertEventArgs e) => base.OnFormat(e);

        public new void OnFormatInfoChanged(EventArgs e) => base.OnFormatInfoChanged(e);

        public new void OnFormatStringChanged(EventArgs e) => base.OnFormatStringChanged(e);

        public new void OnFormattingEnabledChanged(EventArgs e) => base.OnFormattingEnabledChanged(e);

        public new void OnSelectedIndexChanged(EventArgs e) => base.OnSelectedIndexChanged(e);

        public new void OnSelectedValueChanged(EventArgs e) => base.OnSelectedValueChanged(e);

        public new void OnValueMemberChanged(EventArgs e) => base.OnValueMemberChanged(e);

        public Action RefreshItemsHandler { get; set; }

        public void RefreshItemsEntry() => RefreshItems();

        protected override void RefreshItem(int index)
        {
        }

        protected override void RefreshItems()
        {
            RefreshItemsHandler?.Invoke();
            base.RefreshItems();
        }

        public Action<int, object> SetItemCoreHandler { get; set; }

        public void SetItemCoreEntry(int index, object value) => base.SetItemCore(index, value);

        protected override void SetItemCore(int index, object value)
        {
            SetItemCoreHandler?.Invoke(index, value);
        }

        public Action<IList> SetItemsCoreHandler { get; set; }

        protected override void SetItemsCore(IList items)
        {
            SetItemsCoreHandler?.Invoke(items);
        }
    }

    public class AllowSelectionFalseListControl : SubListControl
    {
        protected override bool AllowSelection => false;
    }

    public class CustomTypeConverterDataClass
    {
        [TypeConverter(typeof(CustomTypeConverter))]
        public int Value { get; set; }

        public override string ToString() => "CustomTypeConverterDataClassToString";
    }

    private class CustomTypeConverter : TypeConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            Assert.Equal(10, value);
            Assert.Equal(typeof(string), destinationType);
            return "custom";
        }
    }

    public class NonCriticalThrowingTypeConverterDataClass
    {
        [TypeConverter(typeof(NonCriticalThrowingTypeConverter))]
        public int Value { get; set; }

        public override string ToString() => "NonCriticalThrowingTypeConverterDataClassToString";
    }

    private class NonCriticalThrowingTypeConverter : TypeConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            throw new InvalidOperationException();
        }
    }

    public class CriticalThrowingTypeConverterDataClass
    {
        [TypeConverter(typeof(CriticalThrowingTypeConverter))]
        public int Value { get; set; }
    }

    private class CriticalThrowingTypeConverter : TypeConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
#pragma warning disable CA2201 // Do not raise reserved exception types
            throw new StackOverflowException();
#pragma warning restore CA2201
        }
    }
}
