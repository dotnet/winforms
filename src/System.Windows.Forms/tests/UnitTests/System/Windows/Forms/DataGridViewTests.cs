// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Data;
using System.Drawing;

namespace System.Windows.Forms.Tests;

public partial class DataGridViewTests
{
    public DataGridViewTests()
    {
        // Some controls have behavior that changes when the mouse is over them. Make sure we start with the cursor
        // at a known position.
        //
        // See https://github.com/dotnet/winforms/pull/7031#issuecomment-1101339968 for an example of this.
        Cursor.Position = default;
    }

    [WinFormsFact]
    public void DataGridView_Ctor_Default()
    {
        using DataGridView control = new();
        Assert.Equal(23, control.ColumnHeadersHeight);
        Assert.Equal(DataGridViewColumnHeadersHeightSizeMode.EnableResizing, control.ColumnHeadersHeightSizeMode);
        Assert.Equal(41, control.RowHeadersWidth);
        Assert.Equal(DataGridViewRowHeadersWidthSizeMode.EnableResizing, control.RowHeadersWidthSizeMode);
        Assert.NotNull(control.RowTemplate);
        Assert.Same(control.RowTemplate, control.RowTemplate);
    }

    private const int DefaultColumnHeadersHeight = 23;

    public static IEnumerable<object[]> ColumnHeadersHeight_Set_TestData()
    {
        foreach (bool columnHeadersVisible in new bool[] { true, false })
        {
            foreach (bool autoSize in new bool[] { true, false })
            {
                yield return new object[] { DataGridViewColumnHeadersHeightSizeMode.AutoSize, columnHeadersVisible, autoSize, 4, DefaultColumnHeadersHeight };
                yield return new object[] { DataGridViewColumnHeadersHeightSizeMode.AutoSize, columnHeadersVisible, autoSize, DefaultColumnHeadersHeight, DefaultColumnHeadersHeight };
                yield return new object[] { DataGridViewColumnHeadersHeightSizeMode.AutoSize, columnHeadersVisible, autoSize, 32768, DefaultColumnHeadersHeight };

                foreach (DataGridViewColumnHeadersHeightSizeMode columnHeadersWidthSizeMode in new DataGridViewColumnHeadersHeightSizeMode[] { DataGridViewColumnHeadersHeightSizeMode.EnableResizing, DataGridViewColumnHeadersHeightSizeMode.DisableResizing })
                {
                    yield return new object[] { columnHeadersWidthSizeMode, columnHeadersVisible, autoSize, 4, 4 };
                    yield return new object[] { columnHeadersWidthSizeMode, columnHeadersVisible, autoSize, DefaultColumnHeadersHeight, DefaultColumnHeadersHeight };
                    yield return new object[] { columnHeadersWidthSizeMode, columnHeadersVisible, autoSize, 32768, 32768 };
                }
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(ColumnHeadersHeight_Set_TestData))]
    public void DataGridView_ColumnHeadersHeight_Set_GetReturnsExpected(DataGridViewColumnHeadersHeightSizeMode columnHeadersWidthSizeMode, bool columnHeadersVisible, bool autoSize, int value, int expectedValue)
    {
        using DataGridView control = new()
        {
            ColumnHeadersHeightSizeMode = columnHeadersWidthSizeMode,
            ColumnHeadersVisible = columnHeadersVisible,
            AutoSize = autoSize
        };
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;

        control.ColumnHeadersHeight = value;
        Assert.Equal(expectedValue, control.ColumnHeadersHeight);
        Assert.Equal(0, layoutCallCount);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.ColumnHeadersHeight = value;
        Assert.Equal(expectedValue, control.ColumnHeadersHeight);
        Assert.Equal(0, layoutCallCount);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> ColumnHeadersHeight_SetWithParent_TestData()
    {
        foreach (bool columnHeadersVisible in new bool[] { true, false })
        {
            yield return new object[] { DataGridViewColumnHeadersHeightSizeMode.AutoSize, columnHeadersVisible, true, 4, DefaultColumnHeadersHeight, 0, 0 };
            yield return new object[] { DataGridViewColumnHeadersHeightSizeMode.AutoSize, columnHeadersVisible, true, DefaultColumnHeadersHeight, DefaultColumnHeadersHeight, 0, 0 };
            yield return new object[] { DataGridViewColumnHeadersHeightSizeMode.AutoSize, columnHeadersVisible, true, 32768, DefaultColumnHeadersHeight, 0, 0 };
            yield return new object[] { DataGridViewColumnHeadersHeightSizeMode.AutoSize, columnHeadersVisible, false, 4, DefaultColumnHeadersHeight, 0, 0 };
            yield return new object[] { DataGridViewColumnHeadersHeightSizeMode.AutoSize, columnHeadersVisible, false, DefaultColumnHeadersHeight, DefaultColumnHeadersHeight, 0, 0 };
            yield return new object[] { DataGridViewColumnHeadersHeightSizeMode.AutoSize, columnHeadersVisible, false, 32768, DefaultColumnHeadersHeight, 0, 0 };

            foreach (DataGridViewColumnHeadersHeightSizeMode columnHeadersWidthSizeMode in new DataGridViewColumnHeadersHeightSizeMode[] { DataGridViewColumnHeadersHeightSizeMode.EnableResizing, DataGridViewColumnHeadersHeightSizeMode.DisableResizing })
            {
                yield return new object[] { columnHeadersWidthSizeMode, columnHeadersVisible, true, 4, 4, 0, 1 };
                yield return new object[] { columnHeadersWidthSizeMode, columnHeadersVisible, true, DefaultColumnHeadersHeight, DefaultColumnHeadersHeight, 0, 0 };
                yield return new object[] { columnHeadersWidthSizeMode, columnHeadersVisible, true, 32768, 32768, columnHeadersVisible ? 1 : 0, 1 };
                yield return new object[] { columnHeadersWidthSizeMode, columnHeadersVisible, false, 4, 4, 0, 0 };
                yield return new object[] { columnHeadersWidthSizeMode, columnHeadersVisible, false, DefaultColumnHeadersHeight, DefaultColumnHeadersHeight, 0, 0 };
                yield return new object[] { columnHeadersWidthSizeMode, columnHeadersVisible, false, 32768, 32768, 0, 0 };
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(ColumnHeadersHeight_SetWithParent_TestData))]
    public void DataGridView_ColumnHeadersHeight_SetWithParent_GetReturnsExpected(DataGridViewColumnHeadersHeightSizeMode columnHeadersWidthSizeMode, bool columnHeadersVisible, bool autoSize, int value, int expectedValue, int expectedLayoutCallCount, int expectedParentLayoutCallCount)
    {
        using Control parent = new();
        using DataGridView control = new()
        {
            ColumnHeadersHeightSizeMode = columnHeadersWidthSizeMode,
            ColumnHeadersVisible = columnHeadersVisible,
            AutoSize = autoSize,
            Parent = parent
        };
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(parent, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("ColumnHeadersHeight", e.AffectedProperty);
            parentLayoutCallCount++;
        }

        parent.Layout += parentHandler;

        try
        {
            control.ColumnHeadersHeight = value;
            Assert.Equal(expectedValue, control.ColumnHeadersHeight);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.ColumnHeadersHeight = value;
            Assert.Equal(expectedValue, control.ColumnHeadersHeight);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
            Assert.False(control.IsHandleCreated);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    public static IEnumerable<object[]> ColumnHeadersHeight_SetWithHandle_TestData()
    {
        yield return new object[] { DataGridViewColumnHeadersHeightSizeMode.AutoSize, true, true, 4, 18 };
        yield return new object[] { DataGridViewColumnHeadersHeightSizeMode.AutoSize, true, true, DefaultColumnHeadersHeight, 18 };
        yield return new object[] { DataGridViewColumnHeadersHeightSizeMode.AutoSize, true, true, 32768, 18 };
        yield return new object[] { DataGridViewColumnHeadersHeightSizeMode.AutoSize, true, false, 4, 18 };
        yield return new object[] { DataGridViewColumnHeadersHeightSizeMode.AutoSize, true, false, DefaultColumnHeadersHeight, 18 };
        yield return new object[] { DataGridViewColumnHeadersHeightSizeMode.AutoSize, true, false, 32768, 18 };
        yield return new object[] { DataGridViewColumnHeadersHeightSizeMode.AutoSize, false, true, 4, DefaultColumnHeadersHeight };
        yield return new object[] { DataGridViewColumnHeadersHeightSizeMode.AutoSize, false, true, DefaultColumnHeadersHeight, DefaultColumnHeadersHeight };
        yield return new object[] { DataGridViewColumnHeadersHeightSizeMode.AutoSize, false, true, 32768, DefaultColumnHeadersHeight };
        yield return new object[] { DataGridViewColumnHeadersHeightSizeMode.AutoSize, false, false, 4, DefaultColumnHeadersHeight };
        yield return new object[] { DataGridViewColumnHeadersHeightSizeMode.AutoSize, false, false, DefaultColumnHeadersHeight, DefaultColumnHeadersHeight };
        yield return new object[] { DataGridViewColumnHeadersHeightSizeMode.AutoSize, false, false, 32768, DefaultColumnHeadersHeight };

        foreach (bool columnHeadersVisible in new bool[] { true, false })
        {
            foreach (DataGridViewColumnHeadersHeightSizeMode columnHeadersWidthSizeMode in new DataGridViewColumnHeadersHeightSizeMode[] { DataGridViewColumnHeadersHeightSizeMode.EnableResizing, DataGridViewColumnHeadersHeightSizeMode.DisableResizing })
            {
                yield return new object[] { columnHeadersWidthSizeMode, columnHeadersVisible, true, 4, 4 };
                yield return new object[] { columnHeadersWidthSizeMode, columnHeadersVisible, false, 4, 4 };
                yield return new object[] { columnHeadersWidthSizeMode, columnHeadersVisible, true, DefaultColumnHeadersHeight, DefaultColumnHeadersHeight };
                yield return new object[] { columnHeadersWidthSizeMode, columnHeadersVisible, false, DefaultColumnHeadersHeight, DefaultColumnHeadersHeight };
                yield return new object[] { columnHeadersWidthSizeMode, columnHeadersVisible, true, 32768, 32768 };
                yield return new object[] { columnHeadersWidthSizeMode, columnHeadersVisible, false, 32768, 32768 };
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(ColumnHeadersHeight_SetWithHandle_TestData))]
    public void DataGridView_ColumnHeadersHeight_SetWithHandle_GetReturnsExpected(DataGridViewColumnHeadersHeightSizeMode columnHeadersWidthSizeMode, bool columnHeadersVisible, bool autoSize, int value, int expectedValue)
    {
        // Invalidation checks are omitted due to https://github.com/dotnet/winforms/issues/7799

        using DataGridView control = new()
        {
            ColumnHeadersHeightSizeMode = columnHeadersWidthSizeMode,
            ColumnHeadersVisible = columnHeadersVisible,
            AutoSize = autoSize
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;

        control.ColumnHeadersHeight = value;
        Assert.Equal(expectedValue, control.ColumnHeadersHeight);
        Assert.Equal(0, layoutCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.ColumnHeadersHeight = value;
        Assert.Equal(expectedValue, control.ColumnHeadersHeight);
        Assert.Equal(0, layoutCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> ColumnHeadersHeight_SetWithParentWithHandle_TestData()
    {
        yield return new object[] { DataGridViewColumnHeadersHeightSizeMode.AutoSize, true, true, 4, 18, 0, 0 };
        yield return new object[] { DataGridViewColumnHeadersHeightSizeMode.AutoSize, true, true, DefaultColumnHeadersHeight, 18, 0, 0 };
        yield return new object[] { DataGridViewColumnHeadersHeightSizeMode.AutoSize, true, true, 32768, 18, 0, 0 };
        yield return new object[] { DataGridViewColumnHeadersHeightSizeMode.AutoSize, true, false, 4, 18, 0, 0 };
        yield return new object[] { DataGridViewColumnHeadersHeightSizeMode.AutoSize, true, false, DefaultColumnHeadersHeight, 18, 0, 0 };
        yield return new object[] { DataGridViewColumnHeadersHeightSizeMode.AutoSize, true, false, 32768, 18, 0, 0 };
        yield return new object[] { DataGridViewColumnHeadersHeightSizeMode.AutoSize, false, true, 4, DefaultColumnHeadersHeight, 0, 0 };
        yield return new object[] { DataGridViewColumnHeadersHeightSizeMode.AutoSize, false, true, DefaultColumnHeadersHeight, DefaultColumnHeadersHeight, 0, 0 };
        yield return new object[] { DataGridViewColumnHeadersHeightSizeMode.AutoSize, false, true, 32768, DefaultColumnHeadersHeight, 0, 0 };
        yield return new object[] { DataGridViewColumnHeadersHeightSizeMode.AutoSize, false, false, 4, DefaultColumnHeadersHeight, 0, 0 };
        yield return new object[] { DataGridViewColumnHeadersHeightSizeMode.AutoSize, false, false, DefaultColumnHeadersHeight, DefaultColumnHeadersHeight, 0, 0 };
        yield return new object[] { DataGridViewColumnHeadersHeightSizeMode.AutoSize, false, false, 32768, DefaultColumnHeadersHeight, 0, 0 };

        foreach (bool columnHeadersVisible in new bool[] { true, false })
        {
            foreach (DataGridViewColumnHeadersHeightSizeMode columnHeadersWidthSizeMode in new DataGridViewColumnHeadersHeightSizeMode[] { DataGridViewColumnHeadersHeightSizeMode.EnableResizing, DataGridViewColumnHeadersHeightSizeMode.DisableResizing })
            {
                yield return new object[] { columnHeadersWidthSizeMode, columnHeadersVisible, true, 4, 4, 0, 1 };
                yield return new object[] { columnHeadersWidthSizeMode, columnHeadersVisible, true, DefaultColumnHeadersHeight, DefaultColumnHeadersHeight, 0, 0 };
                yield return new object[] { columnHeadersWidthSizeMode, columnHeadersVisible, true, 32768, 32768, columnHeadersVisible ? 1 : 0, 1 };
                yield return new object[] { columnHeadersWidthSizeMode, columnHeadersVisible, false, 4, 4, 0, 0 };
                yield return new object[] { columnHeadersWidthSizeMode, columnHeadersVisible, false, DefaultColumnHeadersHeight, DefaultColumnHeadersHeight, 0, 0 };
                yield return new object[] { columnHeadersWidthSizeMode, columnHeadersVisible, false, 32768, 32768, 0, 0 };
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(ColumnHeadersHeight_SetWithParentWithHandle_TestData))]
    public void DataGridView_ColumnHeadersHeight_SetWithParentWithHandle_GetReturnsExpected(DataGridViewColumnHeadersHeightSizeMode columnHeadersWidthSizeMode, bool columnHeadersVisible, bool autoSize, int value, int expectedValue, int expectedLayoutCallCount, int expectedParentLayoutCallCount)
    {
        // Invalidation checks are omitted due to https://github.com/dotnet/winforms/issues/7799

        using Control parent = new();
        using DataGridView control = new()
        {
            ColumnHeadersHeightSizeMode = columnHeadersWidthSizeMode,
            ColumnHeadersVisible = columnHeadersVisible,
            AutoSize = autoSize,
            Parent = parent
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(parent, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("ColumnHeadersHeight", e.AffectedProperty);
            parentLayoutCallCount++;
        }

        parent.Layout += parentHandler;

        try
        {
            control.ColumnHeadersHeight = value;
            Assert.Equal(expectedValue, control.ColumnHeadersHeight);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
            Assert.True(control.IsHandleCreated);;
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.ColumnHeadersHeight = value;
            Assert.Equal(expectedValue, control.ColumnHeadersHeight);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    [WinFormsTheory]
    [InlineData(DataGridViewColumnHeadersHeightSizeMode.EnableResizing, 1)]
    [InlineData(DataGridViewColumnHeadersHeightSizeMode.DisableResizing, 1)]
    [InlineData(DataGridViewColumnHeadersHeightSizeMode.AutoSize, 0)]
    public void DataGridView_ColumnHeadersHeight_SetWithHandler_CallsColumnHeadersHeightChanged(DataGridViewColumnHeadersHeightSizeMode columnHeadersWidthSizeMode, int expectedCallCount)
    {
        using DataGridView control = new()
        {
            ColumnHeadersHeightSizeMode = columnHeadersWidthSizeMode
        };
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        control.ColumnHeadersHeightChanged += handler;

        // Set different.
        control.ColumnHeadersHeight = 20;
        Assert.True(control.ColumnHeadersHeight > 0);
        Assert.Equal(expectedCallCount, callCount);

        // Set same.
        control.ColumnHeadersHeight = 20;
        Assert.True(control.ColumnHeadersHeight > 0);
        Assert.Equal(expectedCallCount, callCount);

        // Set different.
        control.ColumnHeadersHeight = 18;
        Assert.True(control.ColumnHeadersHeight > 0);
        Assert.Equal(expectedCallCount * 2, callCount);

        // Remove handler.
        control.ColumnHeadersHeightChanged -= handler;
        control.ColumnHeadersHeight = 20;
        Assert.True(control.ColumnHeadersHeight > 0);
        Assert.Equal(expectedCallCount * 2, callCount);
    }

    [WinFormsTheory]
    [EnumData<DataGridViewColumnHeadersHeightSizeMode>]
    public void DataGridView_ColumnHeadersHeight_SetWithHandlerDisposed_DoesNotCallColumnHeadersHeightChanged(DataGridViewColumnHeadersHeightSizeMode columnHeadersWidthSizeMode)
    {
        using DataGridView control = new()
        {
            ColumnHeadersHeightSizeMode = columnHeadersWidthSizeMode
        };
        control.Dispose();

        int callCount = 0;
        EventHandler handler = (sender, e) => callCount++;
        control.ColumnHeadersHeightChanged += handler;

        // Set different.
        control.ColumnHeadersHeight = 20;
        Assert.True(control.ColumnHeadersHeight > 0);
        Assert.Equal(0, callCount);

        // Set same.
        control.ColumnHeadersHeight = 20;
        Assert.True(control.ColumnHeadersHeight > 0);
        Assert.Equal(0, callCount);

        // Set different.
        control.ColumnHeadersHeight = 18;
        Assert.True(control.ColumnHeadersHeight > 0);
        Assert.Equal(0, callCount);

        // Remove handler.
        control.ColumnHeadersHeightChanged -= handler;
        control.ColumnHeadersHeight = 20;
        Assert.True(control.ColumnHeadersHeight > 0);
        Assert.Equal(0, callCount);
    }

    [WinFormsTheory]
    [InlineData(DataGridViewColumnHeadersHeightSizeMode.EnableResizing, 1)]
    [InlineData(DataGridViewColumnHeadersHeightSizeMode.DisableResizing, 1)]
    [InlineData(DataGridViewColumnHeadersHeightSizeMode.AutoSize, 0)]
    public void DataGridView_ColumnHeadersHeight_SetWithHandlerInDisposing_CallsColumnHeadersHeightChanged(DataGridViewColumnHeadersHeightSizeMode columnHeadersWidthSizeMode, int expectedCallCount)
    {
        using DataGridView control = new()
        {
            ColumnHeadersHeightSizeMode = columnHeadersWidthSizeMode
        };
        int disposingCallCount = 0;
        EventHandler handler = (sender, e) =>
        {
            int callCount = 0;
            EventHandler handler = (sender, e) => callCount++;
            control.ColumnHeadersHeightChanged += handler;

            // Set different.
            control.ColumnHeadersHeight = 20;
            Assert.True(control.ColumnHeadersHeight > 0);
            Assert.Equal(expectedCallCount, callCount);

            // Set same.
            control.ColumnHeadersHeight = 20;
            Assert.True(control.ColumnHeadersHeight > 0);
            Assert.Equal(expectedCallCount, callCount);

            // Set different.
            control.ColumnHeadersHeight = 18;
            Assert.True(control.ColumnHeadersHeight > 0);
            Assert.Equal(expectedCallCount * 2, callCount);

            // Remove handler.
            control.ColumnHeadersHeightChanged -= handler;
            control.ColumnHeadersHeight = 20;
            Assert.True(control.ColumnHeadersHeight > 0);
            Assert.Equal(expectedCallCount * 2, callCount);

            disposingCallCount++;
        };
        control.Disposed += handler;
        try
        {
            control.Dispose();
            Assert.Equal(1, disposingCallCount);
        }
        finally
        {
            control.Disposed -= handler;
        }
    }

    [WinFormsTheory]
    [EnumData<DataGridViewColumnHeadersHeightSizeMode>]
    public void DataGridView_ColumnHeadersHeight_SetWithHandlerInColumnDisposing_DoesNotCallColumnHeadersHeightChanged(DataGridViewColumnHeadersHeightSizeMode columnHeadersWidthSizeMode)
    {
        using DataGridView control = new()
        {
            ColumnHeadersHeightSizeMode = columnHeadersWidthSizeMode
        };
        using SubDataGridViewCell cellTemplate = new();
        using DataGridViewColumn column = new()
        {
            CellTemplate = cellTemplate
        };
        control.Columns.Add(column);
        int disposingCallCount = 0;
        column.Disposed += (sender, e) =>
        {
            int callCount = 0;
            EventHandler handler = (sender, e) => callCount++;
            control.ColumnHeadersHeightChanged += handler;

            // Set different.
            control.ColumnHeadersHeight = 20;
            Assert.True(control.ColumnHeadersHeight > 0);
            Assert.Equal(0, callCount);

            // Set same.
            control.ColumnHeadersHeight = 20;
            Assert.True(control.ColumnHeadersHeight > 0);
            Assert.Equal(0, callCount);

            // Set different.
            control.ColumnHeadersHeight = 18;
            Assert.True(control.ColumnHeadersHeight > 0);
            Assert.Equal(0, callCount);

            // Remove handler.
            control.ColumnHeadersHeightChanged -= handler;
            control.ColumnHeadersHeight = 20;
            Assert.True(control.ColumnHeadersHeight > 0);
            Assert.Equal(0, callCount);

            disposingCallCount++;
        };
        control.Dispose();
        Assert.Equal(1, disposingCallCount);
    }

    [WinFormsTheory]
    [InlineData(3)]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(32769)]
    public void DataGridView_ColumnHeadersHeight_SetInvalidValue_ThrowsArgumentOutOfRangeException(int value)
    {
        using DataGridView control = new();
        Assert.Throws<ArgumentOutOfRangeException>("value", () => control.ColumnHeadersHeight = value);
    }

    public static IEnumerable<object[]> ColumnHeadersHeightSizeMode_Set_TestData()
    {
        foreach (bool columnHeadersVisible in new bool[] { true, false })
        {
            foreach (DataGridViewColumnHeadersHeightSizeMode value in Enum.GetValues(typeof(DataGridViewColumnHeadersHeightSizeMode)))
            {
                yield return new object[] { columnHeadersVisible, value };
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(ColumnHeadersHeightSizeMode_Set_TestData))]
    public void DataGridView_ColumnHeadersHeightSizeMode_Set_GetReturnsExpected(bool columnHeadersVisible, DataGridViewColumnHeadersHeightSizeMode value)
    {
        using DataGridView control = new()
        {
            ColumnHeadersVisible = columnHeadersVisible,
            ColumnHeadersHeightSizeMode = value
        };
        Assert.Equal(value, control.ColumnHeadersHeightSizeMode);
        Assert.Equal(DefaultColumnHeadersHeight, control.ColumnHeadersHeight);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.ColumnHeadersHeightSizeMode = value;
        Assert.Equal(value, control.ColumnHeadersHeightSizeMode);
        Assert.Equal(DefaultColumnHeadersHeight, control.ColumnHeadersHeight);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> ColumnHeadersHeightSizeMode_SetWithHandle_TestData()
    {
        yield return new object[] { true, DataGridViewColumnHeadersHeightSizeMode.AutoSize, 18};
        yield return new object[] { true, DataGridViewColumnHeadersHeightSizeMode.DisableResizing, DefaultColumnHeadersHeight };
        yield return new object[] { true, DataGridViewColumnHeadersHeightSizeMode.EnableResizing, DefaultColumnHeadersHeight };
        yield return new object[] { false, DataGridViewColumnHeadersHeightSizeMode.AutoSize, DefaultColumnHeadersHeight };
        yield return new object[] { false, DataGridViewColumnHeadersHeightSizeMode.DisableResizing, DefaultColumnHeadersHeight };
        yield return new object[] { false, DataGridViewColumnHeadersHeightSizeMode.EnableResizing, DefaultColumnHeadersHeight };
    }

    [WinFormsTheory]
    [MemberData(nameof(ColumnHeadersHeightSizeMode_SetWithHandle_TestData))]
    public void DataGridView_ColumnHeadersHeightSizeMode_SetWithHandle_GetReturnsExpected(bool columnHeadersVisible, DataGridViewColumnHeadersHeightSizeMode value, int expectedColumnHeadersHeight)
    {
        // Invalidation checks are omitted due to https://github.com/dotnet/winforms/issues/7799

        using DataGridView control = new()
        {
            ColumnHeadersVisible = columnHeadersVisible
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.ColumnHeadersHeightSizeMode = value;
        Assert.Equal(value, control.ColumnHeadersHeightSizeMode);
        Assert.Equal(expectedColumnHeadersHeight, control.ColumnHeadersHeight);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.ColumnHeadersHeightSizeMode = value;
        Assert.Equal(value, control.ColumnHeadersHeightSizeMode);
        Assert.Equal(expectedColumnHeadersHeight, control.ColumnHeadersHeight);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(DataGridViewColumnHeadersHeightSizeMode.DisableResizing, DataGridViewColumnHeadersHeightSizeMode.AutoSize)]
    [InlineData(DataGridViewColumnHeadersHeightSizeMode.EnableResizing, DataGridViewColumnHeadersHeightSizeMode.AutoSize)]
    public void DataGridView_ColumnHeadersHeightSizeMode_SetNonResizeThenResize_RestoresOldValue(DataGridViewColumnHeadersHeightSizeMode originalColumnHeadersHeightSizeMode, DataGridViewColumnHeadersHeightSizeMode value)
    {
        // Invalidation checks are omitted due to https://github.com/dotnet/winforms/issues/7799

        using DataGridView control = new()
        {
            ColumnHeadersHeightSizeMode = originalColumnHeadersHeightSizeMode,
            ColumnHeadersHeight = 30
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
        int columnHeadersWidthChangedCallCount = 0;
        control.ColumnHeadersHeightChanged += (sender, e) => columnHeadersWidthChangedCallCount++;

        control.ColumnHeadersHeightSizeMode = value;
        Assert.Equal(value, control.ColumnHeadersHeightSizeMode);
        Assert.Equal(18, control.ColumnHeadersHeight);
        Assert.Equal(1, columnHeadersWidthChangedCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Restore.
        control.ColumnHeadersHeightSizeMode = originalColumnHeadersHeightSizeMode;
        Assert.Equal(originalColumnHeadersHeightSizeMode, control.ColumnHeadersHeightSizeMode);
        Assert.Equal(30, control.ColumnHeadersHeight);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(DataGridViewColumnHeadersHeightSizeMode.DisableResizing, false)]
    [InlineData(DataGridViewColumnHeadersHeightSizeMode.AutoSize, true)]
    public void DataGridView_ColumnHeadersHeightSizeMode_SetWithHandler_CallsColumnHeadersHeightSizeModeChanged(DataGridViewColumnHeadersHeightSizeMode value, bool expectedPreviousModeAutoSized)
    {
        using DataGridView control = new();
        int callCount = 0;
        object expected = false;
        DataGridViewAutoSizeModeEventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Equal(expected, e.PreviousModeAutoSized);
            callCount++;
        };
        control.ColumnHeadersHeightSizeModeChanged += handler;

        // Set different.
        control.ColumnHeadersHeightSizeMode = value;
        Assert.Equal(value, control.ColumnHeadersHeightSizeMode);
        Assert.Equal(1, callCount);

        // Set same.
        control.ColumnHeadersHeightSizeMode = value;
        Assert.Equal(value, control.ColumnHeadersHeightSizeMode);
        Assert.Equal(1, callCount);

        // Set different.
        expected = expectedPreviousModeAutoSized;
        control.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
        Assert.Equal(DataGridViewColumnHeadersHeightSizeMode.EnableResizing, control.ColumnHeadersHeightSizeMode);
        Assert.Equal(2, callCount);

        // Remove handler.
        control.ColumnHeadersHeightSizeModeChanged -= handler;
        control.ColumnHeadersHeightSizeMode = value;
        Assert.Equal(value, control.ColumnHeadersHeightSizeMode);
        Assert.Equal(2, callCount);
    }

    [WinFormsTheory]
    [InlineData(DataGridViewColumnHeadersHeightSizeMode.DisableResizing)]
    [InlineData(DataGridViewColumnHeadersHeightSizeMode.AutoSize)]
    public void DataGridView_ColumnHeadersHeightSizeMode_SetWithHandlerDisposed_DoesNotCallColumnHeadersHeightSizeModeChanged(DataGridViewColumnHeadersHeightSizeMode value)
    {
        using DataGridView control = new();
        control.Dispose();
        int callCount = 0;
        DataGridViewAutoSizeModeEventHandler handler = (sender, e) => callCount++;
        control.ColumnHeadersHeightSizeModeChanged += handler;

        // Set different.
        control.ColumnHeadersHeightSizeMode = value;
        Assert.Equal(value, control.ColumnHeadersHeightSizeMode);
        Assert.Equal(0, callCount);

        // Set same.
        control.ColumnHeadersHeightSizeMode = value;
        Assert.Equal(value, control.ColumnHeadersHeightSizeMode);
        Assert.Equal(0, callCount);

        // Set different.
        control.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
        Assert.Equal(DataGridViewColumnHeadersHeightSizeMode.EnableResizing, control.ColumnHeadersHeightSizeMode);
        Assert.Equal(0, callCount);

        // Remove handler.
        control.ColumnHeadersHeightSizeModeChanged -= handler;
        control.ColumnHeadersHeightSizeMode = value;
        Assert.Equal(value, control.ColumnHeadersHeightSizeMode);
        Assert.Equal(0, callCount);
    }

    [WinFormsTheory]
    [InlineData(DataGridViewColumnHeadersHeightSizeMode.DisableResizing)]
    [InlineData(DataGridViewColumnHeadersHeightSizeMode.AutoSize)]
    public void DataGridView_ColumnHeadersHeightSizeMode_SetWithHandlerInDisposing_DoesNotCallColumnHeadersHeightSizeModeChanged(DataGridViewColumnHeadersHeightSizeMode value)
    {
        using DataGridView control = new();
        using SubDataGridViewCell cellTemplate = new();
        using DataGridViewColumn column = new()
        {
            CellTemplate = cellTemplate
        };
        control.Columns.Add(column);
        int disposingCallCount = 0;
        EventHandler handler = (sender, e) =>
        {
            int callCount = 0;
            DataGridViewAutoSizeModeEventHandler handler = (sender, e) => callCount++;
            control.ColumnHeadersHeightSizeModeChanged += handler;

            // Set different.
            control.ColumnHeadersHeightSizeMode = value;
            Assert.Equal(value, control.ColumnHeadersHeightSizeMode);
            Assert.Equal(1, callCount);

            // Set same.
            control.ColumnHeadersHeightSizeMode = value;
            Assert.Equal(value, control.ColumnHeadersHeightSizeMode);
            Assert.Equal(1, callCount);

            // Set different.
            control.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            Assert.Equal(DataGridViewColumnHeadersHeightSizeMode.EnableResizing, control.ColumnHeadersHeightSizeMode);
            Assert.Equal(2, callCount);

            // Remove handler.
            control.ColumnHeadersHeightSizeModeChanged -= handler;
            control.ColumnHeadersHeightSizeMode = value;
            Assert.Equal(value, control.ColumnHeadersHeightSizeMode);
            Assert.Equal(2, callCount);

            disposingCallCount++;
        };
        control.Disposed += handler;
        try
        {
            control.Dispose();
            Assert.Equal(1, disposingCallCount);
        }
        finally
        {
            control.Disposed -= handler;
        }
    }

    [WinFormsTheory]
    [InlineData(DataGridViewColumnHeadersHeightSizeMode.DisableResizing)]
    [InlineData(DataGridViewColumnHeadersHeightSizeMode.AutoSize)]
    public void DataGridView_ColumnHeadersHeightSizeMode_SetWithHandlerInColumnDisposing_DoesNotCallColumnHeadersHeightSizeModeChanged(DataGridViewColumnHeadersHeightSizeMode value)
    {
        using DataGridView control = new();
        using SubDataGridViewCell cellTemplate = new();
        using DataGridViewColumn column = new()
        {
            CellTemplate = cellTemplate
        };
        control.Columns.Add(column);
        int disposingCallCount = 0;
        column.Disposed += (sender, e) =>
        {
            int callCount = 0;
            DataGridViewAutoSizeModeEventHandler handler = (sender, e) => callCount++;
            control.ColumnHeadersHeightSizeModeChanged += handler;

            // Set different.
            control.ColumnHeadersHeightSizeMode = value;
            Assert.Equal(value, control.ColumnHeadersHeightSizeMode);
            Assert.Equal(0, callCount);

            // Set same.
            control.ColumnHeadersHeightSizeMode = value;
            Assert.Equal(value, control.ColumnHeadersHeightSizeMode);
            Assert.Equal(0, callCount);

            // Set different.
            control.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            Assert.Equal(DataGridViewColumnHeadersHeightSizeMode.EnableResizing, control.ColumnHeadersHeightSizeMode);
            Assert.Equal(0, callCount);

            // Remove handler.
            control.ColumnHeadersHeightSizeModeChanged -= handler;
            control.ColumnHeadersHeightSizeMode = value;
            Assert.Equal(value, control.ColumnHeadersHeightSizeMode);
            Assert.Equal(0, callCount);

            disposingCallCount++;
        };
        control.Dispose();
        Assert.Equal(1, disposingCallCount);
    }

    [WinFormsTheory]
    [InvalidEnumData<DataGridViewColumnHeadersHeightSizeMode>]
    public void DataGridView_ColumnHeadersHeightSizeMode_SetInvalidValue_ThrowsInvalidEnumArgumentException(DataGridViewColumnHeadersHeightSizeMode value)
    {
        using DataGridView control = new();
        Assert.Throws<InvalidEnumArgumentException>("value", () => control.ColumnHeadersHeightSizeMode = value);
    }

    public static IEnumerable<object[]> DefaultCellStyle_TestData()
    {
        // If any of the following properties are not initialised or set to the following values
        // accessing DefaultCellStyle property will return a copy of cell styles, instead of the existing object

        yield return new object[] { new DataGridViewCellStyle() };
        yield return new object[] { new DataGridViewCellStyle { BackColor = Color.Empty } };
        yield return new object[] { new DataGridViewCellStyle { ForeColor = Color.Empty } };
        yield return new object[] { new DataGridViewCellStyle { SelectionBackColor = Color.Empty } };
        yield return new object[] { new DataGridViewCellStyle { SelectionForeColor = Color.Empty } };
        yield return new object[] { new DataGridViewCellStyle { Font = null } };
        yield return new object[] { new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.NotSet } };
        yield return new object[] { new DataGridViewCellStyle { WrapMode = DataGridViewTriState.NotSet } };
    }

    [WinFormsTheory]
    [MemberData(nameof(DefaultCellStyle_TestData))]
    public void DataGridView_DefaultCellStyle_returns_copy_if_not_all_fields_initialised(DataGridViewCellStyle cellStyle)
    {
        using DataGridView dataGridView = new DataGridView
        {
            DefaultCellStyle = cellStyle,
        };

        Assert.NotSame(cellStyle, dataGridView.DefaultCellStyle);
    }

    public static IEnumerable<object[]> Parent_Set_TestData()
    {
        yield return new object[] { null };
        yield return new object[] { new Control() };
        yield return new object[] { new DataGridView() };
    }

    [WinFormsTheory]
    [MemberData(nameof(Parent_Set_TestData))]
    public void DataGridView_Parent_Set_GetReturnsExpected(Control value)
    {
        using DataGridView control = new()
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
    public void DataGridView_Parent_SetWithNonNullOldParent_GetReturnsExpected(Control value)
    {
        using Control oldParent = new();
        using DataGridView control = new()
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
    public void DataGridView_Parent_SetNonNull_AddsToControls()
    {
        using Control parent = new();
        using DataGridView control = new()
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
    public void DataGridView_Parent_SetWithHandle_GetReturnsExpected(Control value)
    {
        // Invalidation checks are omitted due to https://github.com/dotnet/winforms/issues/7799

        using DataGridView control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.Parent = value;
        Assert.Same(value, control.Parent);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.Parent = value;
        Assert.Same(value, control.Parent);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void DataGridView_Parent_SetWithHandler_CallsParentChanged()
    {
        using Control parent = new();
        using DataGridView control = new();
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
    public void DataGridView_Parent_SetSame_ThrowsArgumentException()
    {
        using DataGridView control = new();
        Assert.Throws<ArgumentException>(() => control.Parent = control);
        Assert.Null(control.Parent);
    }

    private const int DefaultRowHeadersWidth = 41;

    public static IEnumerable<object[]> RowHeadersWidth_Set_TestData()
    {
        foreach (bool rowHeadersVisible in new bool[] { true, false })
        {
            foreach (bool autoSize in new bool[] { true, false })
            {
                foreach (DataGridViewRowHeadersWidthSizeMode rowHeadersWidthSizeMode in new DataGridViewRowHeadersWidthSizeMode[] { DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders, DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders, DataGridViewRowHeadersWidthSizeMode.AutoSizeToFirstHeader })
                {
                    yield return new object[] { rowHeadersWidthSizeMode, rowHeadersVisible, autoSize, 4, DefaultRowHeadersWidth };
                    yield return new object[] { rowHeadersWidthSizeMode, rowHeadersVisible, autoSize, DefaultRowHeadersWidth, DefaultRowHeadersWidth };
                    yield return new object[] { rowHeadersWidthSizeMode, rowHeadersVisible, autoSize, 32768, DefaultRowHeadersWidth };
                }

                foreach (DataGridViewRowHeadersWidthSizeMode rowHeadersWidthSizeMode in new DataGridViewRowHeadersWidthSizeMode[] { DataGridViewRowHeadersWidthSizeMode.EnableResizing, DataGridViewRowHeadersWidthSizeMode.DisableResizing })
                {
                    yield return new object[] { rowHeadersWidthSizeMode, rowHeadersVisible, autoSize, 4, 4 };
                    yield return new object[] { rowHeadersWidthSizeMode, rowHeadersVisible, autoSize, DefaultRowHeadersWidth, DefaultRowHeadersWidth };
                    yield return new object[] { rowHeadersWidthSizeMode, rowHeadersVisible, autoSize, 32768, 32768 };
                }
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(RowHeadersWidth_Set_TestData))]
    public void DataGridView_RowHeadersWidth_Set_GetReturnsExpected(DataGridViewRowHeadersWidthSizeMode rowHeadersWidthSizeMode, bool rowHeadersVisible, bool autoSize, int value, int expectedValue)
    {
        using DataGridView control = new()
        {
            RowHeadersWidthSizeMode = rowHeadersWidthSizeMode,
            RowHeadersVisible = rowHeadersVisible,
            AutoSize = autoSize
        };
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;

        control.RowHeadersWidth = value;
        Assert.Equal(expectedValue, control.RowHeadersWidth);
        Assert.Equal(0, layoutCallCount);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.RowHeadersWidth = value;
        Assert.Equal(expectedValue, control.RowHeadersWidth);
        Assert.Equal(0, layoutCallCount);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> RowHeadersWidth_SetWithParent_TestData()
    {
        foreach (bool rowHeadersVisible in new bool[] { true, false })
        {
            foreach (DataGridViewRowHeadersWidthSizeMode rowHeadersWidthSizeMode in new DataGridViewRowHeadersWidthSizeMode[] { DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders, DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders, DataGridViewRowHeadersWidthSizeMode.AutoSizeToFirstHeader })
            {
                yield return new object[] { rowHeadersWidthSizeMode, rowHeadersVisible, true, 4, DefaultRowHeadersWidth, 0, 0 };
                yield return new object[] { rowHeadersWidthSizeMode, rowHeadersVisible, true, DefaultRowHeadersWidth, DefaultRowHeadersWidth, 0, 0 };
                yield return new object[] { rowHeadersWidthSizeMode, rowHeadersVisible, true, 32768, DefaultRowHeadersWidth, 0, 0 };
                yield return new object[] { rowHeadersWidthSizeMode, rowHeadersVisible, false, 4, DefaultRowHeadersWidth, 0, 0 };
                yield return new object[] { rowHeadersWidthSizeMode, rowHeadersVisible, false, DefaultRowHeadersWidth, DefaultRowHeadersWidth, 0, 0 };
                yield return new object[] { rowHeadersWidthSizeMode, rowHeadersVisible, false, 32768, DefaultRowHeadersWidth, 0, 0 };
            }

            foreach (DataGridViewRowHeadersWidthSizeMode rowHeadersWidthSizeMode in new DataGridViewRowHeadersWidthSizeMode[] { DataGridViewRowHeadersWidthSizeMode.EnableResizing, DataGridViewRowHeadersWidthSizeMode.DisableResizing })
            {
                yield return new object[] { rowHeadersWidthSizeMode, rowHeadersVisible, true, 4, 4, 0, 1 };
                yield return new object[] { rowHeadersWidthSizeMode, rowHeadersVisible, true, DefaultRowHeadersWidth, DefaultRowHeadersWidth, 0, 0 };
                yield return new object[] { rowHeadersWidthSizeMode, rowHeadersVisible, true, 32768, 32768, rowHeadersVisible ? 1 : 0, 1 };
                yield return new object[] { rowHeadersWidthSizeMode, rowHeadersVisible, false, 4, 4, 0, 0 };
                yield return new object[] { rowHeadersWidthSizeMode, rowHeadersVisible, false, DefaultRowHeadersWidth, DefaultRowHeadersWidth, 0, 0 };
                yield return new object[] { rowHeadersWidthSizeMode, rowHeadersVisible, false, 32768, 32768, 0, 0 };
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(RowHeadersWidth_SetWithParent_TestData))]
    public void DataGridView_RowHeadersWidth_SetWithParent_GetReturnsExpected(DataGridViewRowHeadersWidthSizeMode rowHeadersWidthSizeMode, bool rowHeadersVisible, bool autoSize, int value, int expectedValue, int expectedLayoutCallCount, int expectedParentLayoutCallCount)
    {
        using Control parent = new();
        using DataGridView control = new()
        {
            RowHeadersWidthSizeMode = rowHeadersWidthSizeMode,
            RowHeadersVisible = rowHeadersVisible,
            AutoSize = autoSize,
            Parent = parent
        };
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(parent, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("RowHeadersWidth", e.AffectedProperty);
            parentLayoutCallCount++;
        }

        parent.Layout += parentHandler;

        try
        {
            control.RowHeadersWidth = value;
            Assert.Equal(expectedValue, control.RowHeadersWidth);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.RowHeadersWidth = value;
            Assert.Equal(expectedValue, control.RowHeadersWidth);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
            Assert.False(control.IsHandleCreated);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    public static IEnumerable<object[]> RowHeadersWidth_SetWithHandle_TestData()
    {
        foreach (DataGridViewRowHeadersWidthSizeMode rowHeadersWidthSizeMode in new DataGridViewRowHeadersWidthSizeMode[] { DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders, DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders, DataGridViewRowHeadersWidthSizeMode.AutoSizeToFirstHeader })
        {
            yield return new object[] { rowHeadersWidthSizeMode, true, true, 4, 21 };
            yield return new object[] { rowHeadersWidthSizeMode, true, true, DefaultRowHeadersWidth, 21 };
            yield return new object[] { rowHeadersWidthSizeMode, true, true, 32768, 21 };
            yield return new object[] { rowHeadersWidthSizeMode, true, false, 4, 21 };
            yield return new object[] { rowHeadersWidthSizeMode, true, false, DefaultRowHeadersWidth, 21 };
            yield return new object[] { rowHeadersWidthSizeMode, true, false, 32768, 21 };
            yield return new object[] { rowHeadersWidthSizeMode, false, true, 4, DefaultRowHeadersWidth };
            yield return new object[] { rowHeadersWidthSizeMode, false, true, DefaultRowHeadersWidth, DefaultRowHeadersWidth };
            yield return new object[] { rowHeadersWidthSizeMode, false, true, 32768, DefaultRowHeadersWidth };
            yield return new object[] { rowHeadersWidthSizeMode, false, false, 4, DefaultRowHeadersWidth };
            yield return new object[] { rowHeadersWidthSizeMode, false, false, DefaultRowHeadersWidth, DefaultRowHeadersWidth };
            yield return new object[] { rowHeadersWidthSizeMode, false, false, 32768, DefaultRowHeadersWidth };
        }

        foreach (bool rowHeadersVisible in new bool[] { true, false })
        {
            foreach (DataGridViewRowHeadersWidthSizeMode rowHeadersWidthSizeMode in new DataGridViewRowHeadersWidthSizeMode[] { DataGridViewRowHeadersWidthSizeMode.EnableResizing, DataGridViewRowHeadersWidthSizeMode.DisableResizing })
            {
                yield return new object[] { rowHeadersWidthSizeMode, rowHeadersVisible, true, 4, 4 };
                yield return new object[] { rowHeadersWidthSizeMode, rowHeadersVisible, false, 4, 4 };
                yield return new object[] { rowHeadersWidthSizeMode, rowHeadersVisible, true, DefaultRowHeadersWidth, DefaultRowHeadersWidth };
                yield return new object[] { rowHeadersWidthSizeMode, rowHeadersVisible, false, DefaultRowHeadersWidth, DefaultRowHeadersWidth };
                yield return new object[] { rowHeadersWidthSizeMode, rowHeadersVisible, true, 32768, 32768 };
                yield return new object[] { rowHeadersWidthSizeMode, rowHeadersVisible, false, 32768, 32768 };
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(RowHeadersWidth_SetWithHandle_TestData))]
    public void DataGridView_RowHeadersWidth_SetWithHandle_GetReturnsExpected(DataGridViewRowHeadersWidthSizeMode rowHeadersWidthSizeMode, bool rowHeadersVisible, bool autoSize, int value, int expectedValue)
    {
        // Invalidation checks are omitted due to https://github.com/dotnet/winforms/issues/7799

        using DataGridView control = new()
        {
            RowHeadersWidthSizeMode = rowHeadersWidthSizeMode,
            RowHeadersVisible = rowHeadersVisible,
            AutoSize = autoSize
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);

        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;

        control.RowHeadersWidth = value;
        Assert.Equal(expectedValue, control.RowHeadersWidth);
        Assert.Equal(0, layoutCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.RowHeadersWidth = value;
        Assert.Equal(expectedValue, control.RowHeadersWidth);
        Assert.Equal(0, layoutCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> RowHeadersWidth_SetWithParentWithHandle_TestData()
    {
        foreach (DataGridViewRowHeadersWidthSizeMode rowHeadersWidthSizeMode in new DataGridViewRowHeadersWidthSizeMode[] { DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders, DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders, DataGridViewRowHeadersWidthSizeMode.AutoSizeToFirstHeader })
        {
            yield return new object[] { rowHeadersWidthSizeMode, true, true, 4, 21, 0, 0 };
            yield return new object[] { rowHeadersWidthSizeMode, true, true, DefaultRowHeadersWidth, 21, 0, 0 };
            yield return new object[] { rowHeadersWidthSizeMode, true, true, 32768, 21, 0, 0 };
            yield return new object[] { rowHeadersWidthSizeMode, true, false, 4, 21, 0, 0 };
            yield return new object[] { rowHeadersWidthSizeMode, true, false, DefaultRowHeadersWidth, 21, 0, 0 };
            yield return new object[] { rowHeadersWidthSizeMode, true, false, 32768, 21, 0, 0 };
            yield return new object[] { rowHeadersWidthSizeMode, false, true, 4, DefaultRowHeadersWidth, 0, 0 };
            yield return new object[] { rowHeadersWidthSizeMode, false, true, DefaultRowHeadersWidth, DefaultRowHeadersWidth, 0, 0 };
            yield return new object[] { rowHeadersWidthSizeMode, false, true, 32768, DefaultRowHeadersWidth, 0, 0 };
            yield return new object[] { rowHeadersWidthSizeMode, false, false, 4, DefaultRowHeadersWidth, 0, 0 };
            yield return new object[] { rowHeadersWidthSizeMode, false, false, DefaultRowHeadersWidth, DefaultRowHeadersWidth, 0, 0 };
            yield return new object[] { rowHeadersWidthSizeMode, false, false, 32768, DefaultRowHeadersWidth, 0, 0 };
        }

        foreach (bool rowHeadersVisible in new bool[] { true, false })
        {
            foreach (DataGridViewRowHeadersWidthSizeMode rowHeadersWidthSizeMode in new DataGridViewRowHeadersWidthSizeMode[] { DataGridViewRowHeadersWidthSizeMode.EnableResizing, DataGridViewRowHeadersWidthSizeMode.DisableResizing })
            {
                yield return new object[] { rowHeadersWidthSizeMode, rowHeadersVisible, true, 4, 4, 0, 1 };
                yield return new object[] { rowHeadersWidthSizeMode, rowHeadersVisible, true, DefaultRowHeadersWidth, DefaultRowHeadersWidth, 0, 0 };
                yield return new object[] { rowHeadersWidthSizeMode, rowHeadersVisible, true, 32768, 32768, rowHeadersVisible ? 1 : 0, 1 };
                yield return new object[] { rowHeadersWidthSizeMode, rowHeadersVisible, false, 4, 4, 0, 0 };
                yield return new object[] { rowHeadersWidthSizeMode, rowHeadersVisible, false, DefaultRowHeadersWidth, DefaultRowHeadersWidth, 0, 0 };
                yield return new object[] { rowHeadersWidthSizeMode, rowHeadersVisible, false, 32768, 32768, 0, 0 };
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(RowHeadersWidth_SetWithParentWithHandle_TestData))]
    public void DataGridView_RowHeadersWidth_SetWithParentWithHandle_GetReturnsExpected(DataGridViewRowHeadersWidthSizeMode rowHeadersWidthSizeMode, bool rowHeadersVisible, bool autoSize, int value, int expectedValue, int expectedLayoutCallCount, int expectedParentLayoutCallCount)
    {
        // Invalidation checks are omitted due to https://github.com/dotnet/winforms/issues/7799

        using Control parent = new();
        using DataGridView control = new()
        {
            RowHeadersWidthSizeMode = rowHeadersWidthSizeMode,
            RowHeadersVisible = rowHeadersVisible,
            AutoSize = autoSize,
            Parent = parent
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
        int layoutCallCount = 0;
        control.Layout += (sender, e) => layoutCallCount++;
        int parentLayoutCallCount = 0;
        void parentHandler(object sender, LayoutEventArgs e)
        {
            Assert.Same(parent, sender);
            Assert.Same(control, e.AffectedControl);
            Assert.Equal("RowHeadersWidth", e.AffectedProperty);
            parentLayoutCallCount++;
        }

        parent.Layout += parentHandler;

        try
        {
            control.RowHeadersWidth = value;
            Assert.Equal(expectedValue, control.RowHeadersWidth);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.RowHeadersWidth = value;
            Assert.Equal(expectedValue, control.RowHeadersWidth);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }
        finally
        {
            parent.Layout -= parentHandler;
        }
    }

    [WinFormsTheory]
    [InlineData(DataGridViewRowHeadersWidthSizeMode.EnableResizing, 1)]
    [InlineData(DataGridViewRowHeadersWidthSizeMode.DisableResizing, 1)]
    [InlineData(DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders, 0)]
    [InlineData(DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders, 0)]
    [InlineData(DataGridViewRowHeadersWidthSizeMode.AutoSizeToFirstHeader, 0)]
    public void DataGridView_RowHeadersWidth_SetWithHandler_CallsRowHeadersWidthChanged(DataGridViewRowHeadersWidthSizeMode rowHeadersWidthSizeMode, int expectedCallCount)
    {
        using DataGridView control = new()
        {
            RowHeadersWidthSizeMode = rowHeadersWidthSizeMode
        };
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        control.RowHeadersWidthChanged += handler;

        // Set different.
        control.RowHeadersWidth = 20;
        Assert.True(control.RowHeadersWidth > 0);
        Assert.Equal(expectedCallCount, callCount);

        // Set same.
        control.RowHeadersWidth = 20;
        Assert.True(control.RowHeadersWidth > 0);
        Assert.Equal(expectedCallCount, callCount);

        // Set different.
        control.RowHeadersWidth = 21;
        Assert.True(control.RowHeadersWidth > 0);
        Assert.Equal(expectedCallCount * 2, callCount);

        // Remove handler.
        control.RowHeadersWidthChanged -= handler;
        control.RowHeadersWidth = 20;
        Assert.True(control.RowHeadersWidth > 0);
        Assert.Equal(expectedCallCount * 2, callCount);
    }

    [WinFormsTheory]
    [EnumData<DataGridViewRowHeadersWidthSizeMode>]
    public void DataGridView_RowHeadersWidth_SetWithHandlerDisposed_DoesNotCallRowHeadersWidthChanged(DataGridViewRowHeadersWidthSizeMode rowHeadersWidthSizeMode)
    {
        using DataGridView control = new()
        {
            RowHeadersWidthSizeMode = rowHeadersWidthSizeMode
        };
        control.Dispose();

        int callCount = 0;
        EventHandler handler = (sender, e) => callCount++;
        control.RowHeadersWidthChanged += handler;

        // Set different.
        control.RowHeadersWidth = 20;
        Assert.True(control.RowHeadersWidth > 0);
        Assert.Equal(0, callCount);

        // Set same.
        control.RowHeadersWidth = 20;
        Assert.True(control.RowHeadersWidth > 0);
        Assert.Equal(0, callCount);

        // Set different.
        control.RowHeadersWidth = 21;
        Assert.True(control.RowHeadersWidth > 0);
        Assert.Equal(0, callCount);

        // Remove handler.
        control.RowHeadersWidthChanged -= handler;
        control.RowHeadersWidth = 20;
        Assert.True(control.RowHeadersWidth > 0);
        Assert.Equal(0, callCount);
    }

    [WinFormsTheory]
    [InlineData(DataGridViewRowHeadersWidthSizeMode.EnableResizing, 1)]
    [InlineData(DataGridViewRowHeadersWidthSizeMode.DisableResizing, 1)]
    [InlineData(DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders, 0)]
    [InlineData(DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders, 0)]
    [InlineData(DataGridViewRowHeadersWidthSizeMode.AutoSizeToFirstHeader, 0)]
    public void DataGridView_RowHeadersWidth_SetWithHandlerInDisposing_CallsRowHeadersWidthChanged(DataGridViewRowHeadersWidthSizeMode rowHeadersWidthSizeMode, int expectedCallCount)
    {
        using DataGridView control = new()
        {
            RowHeadersWidthSizeMode = rowHeadersWidthSizeMode
        };
        int disposingCallCount = 0;
        EventHandler handler = (sender, e) =>
        {
            int callCount = 0;
            EventHandler handler = (sender, e) => callCount++;
            control.RowHeadersWidthChanged += handler;

            // Set different.
            control.RowHeadersWidth = 20;
            Assert.True(control.RowHeadersWidth > 0);
            Assert.Equal(expectedCallCount, callCount);

            // Set same.
            control.RowHeadersWidth = 20;
            Assert.True(control.RowHeadersWidth > 0);
            Assert.Equal(expectedCallCount, callCount);

            // Set different.
            control.RowHeadersWidth = 21;
            Assert.True(control.RowHeadersWidth > 0);
            Assert.Equal(expectedCallCount * 2, callCount);

            // Remove handler.
            control.RowHeadersWidthChanged -= handler;
            control.RowHeadersWidth = 20;
            Assert.True(control.RowHeadersWidth > 0);
            Assert.Equal(expectedCallCount * 2, callCount);

            disposingCallCount++;
        };
        control.Disposed += handler;
        try
        {
            control.Dispose();
            Assert.Equal(1, disposingCallCount);
        }
        finally
        {
            control.Disposed -= handler;
        }
    }

    [WinFormsTheory]
    [EnumData<DataGridViewRowHeadersWidthSizeMode>]
    public void DataGridView_RowHeadersWidth_SetWithHandlerInColumnDisposing_DoesNotCallRowHeadersWidthChanged(DataGridViewRowHeadersWidthSizeMode rowHeadersWidthSizeMode)
    {
        using DataGridView control = new()
        {
            RowHeadersWidthSizeMode = rowHeadersWidthSizeMode
        };
        using SubDataGridViewCell cellTemplate = new();
        using DataGridViewColumn column = new()
        {
            CellTemplate = cellTemplate
        };
        control.Columns.Add(column);
        int disposingCallCount = 0;
        column.Disposed += (sender, e) =>
        {
            int callCount = 0;
            EventHandler handler = (sender, e) => callCount++;
            control.RowHeadersWidthChanged += handler;

            // Set different.
            control.RowHeadersWidth = 20;
            Assert.True(control.RowHeadersWidth > 0);
            Assert.Equal(0, callCount);

            // Set same.
            control.RowHeadersWidth = 20;
            Assert.True(control.RowHeadersWidth > 0);
            Assert.Equal(0, callCount);

            // Set different.
            control.RowHeadersWidth = 21;
            Assert.True(control.RowHeadersWidth > 0);
            Assert.Equal(0, callCount);

            // Remove handler.
            control.RowHeadersWidthChanged -= handler;
            control.RowHeadersWidth = 20;
            Assert.True(control.RowHeadersWidth > 0);
            Assert.Equal(0, callCount);

            disposingCallCount++;
        };
        control.Dispose();
        Assert.Equal(1, disposingCallCount);
    }

    [WinFormsTheory]
    [InlineData(3)]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(32769)]
    public void DataGridView_RowHeadersWidth_SetInvalidValue_ThrowsArgumentOutOfRangeException(int value)
    {
        using DataGridView control = new();
        Assert.Throws<ArgumentOutOfRangeException>("value", () => control.RowHeadersWidth = value);
    }

    public static IEnumerable<object[]> RowHeadersWidthSizeMode_Set_TestData()
    {
        foreach (bool rowHeadersVisible in new bool[] { true, false })
        {
            foreach (DataGridViewRowHeadersWidthSizeMode value in Enum.GetValues(typeof(DataGridViewRowHeadersWidthSizeMode)))
            {
                yield return new object[] { rowHeadersVisible, value };
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(RowHeadersWidthSizeMode_Set_TestData))]
    public void DataGridView_RowHeadersWidthSizeMode_Set_GetReturnsExpected(bool rowHeadersVisible, DataGridViewRowHeadersWidthSizeMode value)
    {
        using DataGridView control = new()
        {
            RowHeadersVisible = rowHeadersVisible,
            RowHeadersWidthSizeMode = value
        };
        Assert.Equal(value, control.RowHeadersWidthSizeMode);
        Assert.Equal(DefaultRowHeadersWidth, control.RowHeadersWidth);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.RowHeadersWidthSizeMode = value;
        Assert.Equal(value, control.RowHeadersWidthSizeMode);
        Assert.Equal(DefaultRowHeadersWidth, control.RowHeadersWidth);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> RowHeadersWidthSizeMode_SetWithHandle_TestData()
    {
        yield return new object[] { true, DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders, 21 };
        yield return new object[] { true, DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders, 21 };
        yield return new object[] { true, DataGridViewRowHeadersWidthSizeMode.AutoSizeToFirstHeader, 21 };
        yield return new object[] { true, DataGridViewRowHeadersWidthSizeMode.DisableResizing, DefaultRowHeadersWidth };
        yield return new object[] { true, DataGridViewRowHeadersWidthSizeMode.EnableResizing, DefaultRowHeadersWidth };
        yield return new object[] { false, DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders, DefaultRowHeadersWidth };
        yield return new object[] { false, DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders, DefaultRowHeadersWidth };
        yield return new object[] { false, DataGridViewRowHeadersWidthSizeMode.AutoSizeToFirstHeader, DefaultRowHeadersWidth };
        yield return new object[] { false, DataGridViewRowHeadersWidthSizeMode.DisableResizing, DefaultRowHeadersWidth };
        yield return new object[] { false, DataGridViewRowHeadersWidthSizeMode.EnableResizing, DefaultRowHeadersWidth };
    }

    [WinFormsTheory]
    [MemberData(nameof(RowHeadersWidthSizeMode_SetWithHandle_TestData))]
    public void DataGridView_RowHeadersWidthSizeMode_SetWithHandle_GetReturnsExpected(bool rowHeadersVisible, DataGridViewRowHeadersWidthSizeMode value, int expectedRowHeadersWidth)
    {
        // Invalidation checks are omitted due to https://github.com/dotnet/winforms/issues/7799

        using DataGridView control = new()
        {
            RowHeadersVisible = rowHeadersVisible
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.RowHeadersWidthSizeMode = value;
        Assert.Equal(value, control.RowHeadersWidthSizeMode);
        Assert.Equal(expectedRowHeadersWidth, control.RowHeadersWidth);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.RowHeadersWidthSizeMode = value;
        Assert.Equal(value, control.RowHeadersWidthSizeMode);
        Assert.Equal(expectedRowHeadersWidth, control.RowHeadersWidth);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(DataGridViewRowHeadersWidthSizeMode.DisableResizing, DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders)]
    [InlineData(DataGridViewRowHeadersWidthSizeMode.DisableResizing, DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders)]
    [InlineData(DataGridViewRowHeadersWidthSizeMode.DisableResizing, DataGridViewRowHeadersWidthSizeMode.AutoSizeToFirstHeader)]
    [InlineData(DataGridViewRowHeadersWidthSizeMode.EnableResizing, DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders)]
    [InlineData(DataGridViewRowHeadersWidthSizeMode.EnableResizing, DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders)]
    [InlineData(DataGridViewRowHeadersWidthSizeMode.EnableResizing, DataGridViewRowHeadersWidthSizeMode.AutoSizeToFirstHeader)]
    public void DataGridView_RowHeadersWidthSizeMode_SetNonResizeThenResize_RestoresOldValue(DataGridViewRowHeadersWidthSizeMode originalRowHeadersWidthSizeMode, DataGridViewRowHeadersWidthSizeMode value)
    {
        // Invalidation checks are omitted due to https://github.com/dotnet/winforms/issues/7799

        using DataGridView control = new()
        {
            RowHeadersWidthSizeMode = originalRowHeadersWidthSizeMode,
            RowHeadersWidth = 30
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
        int rowHeadersWidthChangedCallCount = 0;
        control.RowHeadersWidthChanged += (sender, e) => rowHeadersWidthChangedCallCount++;

        control.RowHeadersWidthSizeMode = value;
        Assert.Equal(value, control.RowHeadersWidthSizeMode);
        Assert.Equal(21, control.RowHeadersWidth);
        Assert.Equal(1, rowHeadersWidthChangedCallCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Restore.
        control.RowHeadersWidthSizeMode = originalRowHeadersWidthSizeMode;
        Assert.Equal(originalRowHeadersWidthSizeMode, control.RowHeadersWidthSizeMode);
        Assert.Equal(30, control.RowHeadersWidth);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(DataGridViewRowHeadersWidthSizeMode.DisableResizing, false)]
    [InlineData(DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders, true)]
    [InlineData(DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders, true)]
    [InlineData(DataGridViewRowHeadersWidthSizeMode.AutoSizeToFirstHeader, true)]
    public void DataGridView_RowHeadersWidthSizeMode_SetWithHandler_CallsRowHeadersWidthSizeModeChanged(DataGridViewRowHeadersWidthSizeMode value, object expectedPreviousModeAutoSized)
    {
        using DataGridView control = new();
        int callCount = 0;
        object expected = false;
        DataGridViewAutoSizeModeEventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Equal(expected, e.PreviousModeAutoSized);
            callCount++;
        };
        control.RowHeadersWidthSizeModeChanged += handler;

        // Set different.
        control.RowHeadersWidthSizeMode = value;
        Assert.Equal(value, control.RowHeadersWidthSizeMode);
        Assert.Equal(1, callCount);

        // Set same.
        control.RowHeadersWidthSizeMode = value;
        Assert.Equal(value, control.RowHeadersWidthSizeMode);
        Assert.Equal(1, callCount);

        // Set different.
        expected = expectedPreviousModeAutoSized;
        control.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.EnableResizing;
        Assert.Equal(DataGridViewRowHeadersWidthSizeMode.EnableResizing, control.RowHeadersWidthSizeMode);
        Assert.Equal(2, callCount);

        // Remove handler.
        control.RowHeadersWidthSizeModeChanged -= handler;
        control.RowHeadersWidthSizeMode = value;
        Assert.Equal(value, control.RowHeadersWidthSizeMode);
        Assert.Equal(2, callCount);
    }

    [WinFormsTheory]
    [InlineData(DataGridViewRowHeadersWidthSizeMode.DisableResizing)]
    [InlineData(DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders)]
    [InlineData(DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders)]
    [InlineData(DataGridViewRowHeadersWidthSizeMode.AutoSizeToFirstHeader)]
    public void DataGridView_RowHeadersWidthSizeMode_SetWithHandlerDisposed_DoesNotCallRowHeadersWidthSizeModeChanged(DataGridViewRowHeadersWidthSizeMode value)
    {
        using DataGridView control = new();
        control.Dispose();
        int callCount = 0;
        DataGridViewAutoSizeModeEventHandler handler = (sender, e) => callCount++;
        control.RowHeadersWidthSizeModeChanged += handler;

        // Set different.
        control.RowHeadersWidthSizeMode = value;
        Assert.Equal(value, control.RowHeadersWidthSizeMode);
        Assert.Equal(0, callCount);

        // Set same.
        control.RowHeadersWidthSizeMode = value;
        Assert.Equal(value, control.RowHeadersWidthSizeMode);
        Assert.Equal(0, callCount);

        // Set different.
        control.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.EnableResizing;
        Assert.Equal(DataGridViewRowHeadersWidthSizeMode.EnableResizing, control.RowHeadersWidthSizeMode);
        Assert.Equal(0, callCount);

        // Remove handler.
        control.RowHeadersWidthSizeModeChanged -= handler;
        control.RowHeadersWidthSizeMode = value;
        Assert.Equal(value, control.RowHeadersWidthSizeMode);
        Assert.Equal(0, callCount);
    }

    [WinFormsTheory]
    [InlineData(DataGridViewRowHeadersWidthSizeMode.DisableResizing)]
    [InlineData(DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders)]
    [InlineData(DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders)]
    [InlineData(DataGridViewRowHeadersWidthSizeMode.AutoSizeToFirstHeader)]
    public void DataGridView_RowHeadersWidthSizeMode_SetWithHandlerInDisposing_DoesNotCallRowHeadersWidthSizeModeChanged(DataGridViewRowHeadersWidthSizeMode value)
    {
        using DataGridView control = new();
        using SubDataGridViewCell cellTemplate = new();
        using DataGridViewColumn column = new()
        {
            CellTemplate = cellTemplate
        };
        control.Columns.Add(column);
        int disposingCallCount = 0;
        EventHandler handler = (sender, e) =>
        {
            int callCount = 0;
            DataGridViewAutoSizeModeEventHandler handler = (sender, e) => callCount++;
            control.RowHeadersWidthSizeModeChanged += handler;

            // Set different.
            control.RowHeadersWidthSizeMode = value;
            Assert.Equal(value, control.RowHeadersWidthSizeMode);
            Assert.Equal(1, callCount);

            // Set same.
            control.RowHeadersWidthSizeMode = value;
            Assert.Equal(value, control.RowHeadersWidthSizeMode);
            Assert.Equal(1, callCount);

            // Set different.
            control.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.EnableResizing;
            Assert.Equal(DataGridViewRowHeadersWidthSizeMode.EnableResizing, control.RowHeadersWidthSizeMode);
            Assert.Equal(2, callCount);

            // Remove handler.
            control.RowHeadersWidthSizeModeChanged -= handler;
            control.RowHeadersWidthSizeMode = value;
            Assert.Equal(value, control.RowHeadersWidthSizeMode);
            Assert.Equal(2, callCount);

            disposingCallCount++;
        };
        control.Disposed += handler;
        try
        {
            control.Dispose();
            Assert.Equal(1, disposingCallCount);
        }
        finally
        {
            control.Disposed -= handler;
        }
    }

    [WinFormsTheory]
    [InlineData(DataGridViewRowHeadersWidthSizeMode.DisableResizing)]
    [InlineData(DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders)]
    [InlineData(DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders)]
    [InlineData(DataGridViewRowHeadersWidthSizeMode.AutoSizeToFirstHeader)]
    public void DataGridView_RowHeadersWidthSizeMode_SetWithHandlerInColumnDisposing_DoesNotCallRowHeadersWidthSizeModeChanged(DataGridViewRowHeadersWidthSizeMode value)
    {
        using DataGridView control = new();
        using SubDataGridViewCell cellTemplate = new();
        using DataGridViewColumn column = new()
        {
            CellTemplate = cellTemplate
        };
        control.Columns.Add(column);
        int disposingCallCount = 0;
        column.Disposed += (sender, e) =>
        {
            int callCount = 0;
            DataGridViewAutoSizeModeEventHandler handler = (sender, e) => callCount++;
            control.RowHeadersWidthSizeModeChanged += handler;

            // Set different.
            control.RowHeadersWidthSizeMode = value;
            Assert.Equal(value, control.RowHeadersWidthSizeMode);
            Assert.Equal(0, callCount);

            // Set same.
            control.RowHeadersWidthSizeMode = value;
            Assert.Equal(value, control.RowHeadersWidthSizeMode);
            Assert.Equal(0, callCount);

            // Set different.
            control.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.EnableResizing;
            Assert.Equal(DataGridViewRowHeadersWidthSizeMode.EnableResizing, control.RowHeadersWidthSizeMode);
            Assert.Equal(0, callCount);

            // Remove handler.
            control.RowHeadersWidthSizeModeChanged -= handler;
            control.RowHeadersWidthSizeMode = value;
            Assert.Equal(value, control.RowHeadersWidthSizeMode);
            Assert.Equal(0, callCount);

            disposingCallCount++;
        };
        control.Dispose();
        Assert.Equal(1, disposingCallCount);
    }

    [WinFormsTheory]
    [InvalidEnumData<DataGridViewRowHeadersWidthSizeMode>]
    public void DataGridView_RowHeadersWidthSizeMode_SetInvalidValue_ThrowsInvalidEnumArgumentException(DataGridViewRowHeadersWidthSizeMode value)
    {
        using DataGridView control = new();
        Assert.Throws<InvalidEnumArgumentException>("value", () => control.RowHeadersWidthSizeMode = value);
    }

    [WinFormsTheory]
    [InlineData(true, true)]
    [InlineData(true, false)]
    [InlineData(false, true)]
    [InlineData(false, false)]
    public void DataGridView_TopLeftHeaderCell_Set_GetReturnsExpected(bool rowHeadersVisible, bool columnHeadersVisible)
    {
        using DataGridViewHeaderCell cell1 = new();
        using DataGridView control = new()
        {
            RowHeadersVisible = rowHeadersVisible,
            ColumnHeadersVisible = columnHeadersVisible,
            TopLeftHeaderCell = cell1
        };
        Assert.Same(cell1, control.TopLeftHeaderCell);
        Assert.Same(control, cell1.DataGridView);
        Assert.Null(cell1.OwningColumn);
        Assert.Null(cell1.OwningRow);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.TopLeftHeaderCell = cell1;
        Assert.Same(cell1, control.TopLeftHeaderCell);
        Assert.Same(control, cell1.DataGridView);
        Assert.Null(cell1.OwningColumn);
        Assert.Null(cell1.OwningRow);
        Assert.False(control.IsHandleCreated);

        // Set different.
        using DataGridViewHeaderCell cell2 = new();
        control.TopLeftHeaderCell = cell2;
        Assert.Same(cell2, control.TopLeftHeaderCell);
        Assert.Null(cell1.DataGridView);
        Assert.Null(cell1.OwningColumn);
        Assert.Null(cell1.OwningRow);
        Assert.Same(control, cell2.DataGridView);
        Assert.Null(cell2.OwningColumn);
        Assert.Null(cell2.OwningRow);
        Assert.False(control.IsHandleCreated);

        // Set null.
        control.TopLeftHeaderCell = null;
        DataGridViewHeaderCell cell = Assert.IsType<DataGridViewTopLeftHeaderCell>(control.TopLeftHeaderCell);
        Assert.Same(control, cell.DataGridView);
        Assert.Null(cell.OwningColumn);
        Assert.Null(cell.OwningRow);
        Assert.Same(control.TopLeftHeaderCell, control.TopLeftHeaderCell);
        Assert.Null(cell1.DataGridView);
        Assert.Null(cell1.OwningColumn);
        Assert.Null(cell1.OwningRow);
        Assert.Null(cell2.DataGridView);
        Assert.Null(cell2.OwningColumn);
        Assert.Null(cell2.OwningRow);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true, true)]
    [InlineData(true, false)]
    [InlineData(false, true)]
    [InlineData(false, false)]
    public void DataGridView_TopLeftHeaderCell_SetWithHandle_GetReturnsExpected(bool rowHeadersVisible, bool columnHeadersVisible)
    {
        // Invalidation checks are omitted due to https://github.com/dotnet/winforms/issues/7799

        using DataGridViewHeaderCell cell1 = new();
        using DataGridView control = new()
        {
            RowHeadersVisible = rowHeadersVisible,
            ColumnHeadersVisible = columnHeadersVisible
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.TopLeftHeaderCell = cell1;
        Assert.Same(cell1, control.TopLeftHeaderCell);
        Assert.Same(control, cell1.DataGridView);
        Assert.Null(cell1.OwningColumn);
        Assert.Null(cell1.OwningRow);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.TopLeftHeaderCell = cell1;
        Assert.Same(cell1, control.TopLeftHeaderCell);
        Assert.Same(control, cell1.DataGridView);
        Assert.Null(cell1.OwningColumn);
        Assert.Null(cell1.OwningRow);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set different.
        using DataGridViewHeaderCell cell2 = new();
        control.TopLeftHeaderCell = cell2;
        Assert.Same(cell2, control.TopLeftHeaderCell);
        Assert.Null(cell1.DataGridView);
        Assert.Null(cell1.OwningColumn);
        Assert.Null(cell1.OwningRow);
        Assert.Same(control, cell2.DataGridView);
        Assert.Null(cell2.OwningColumn);
        Assert.Null(cell2.OwningRow);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set null.
        control.TopLeftHeaderCell = null;
        DataGridViewHeaderCell cell = Assert.IsType<DataGridViewTopLeftHeaderCell>(control.TopLeftHeaderCell);
        Assert.Same(control, cell.DataGridView);
        Assert.Null(cell.OwningColumn);
        Assert.Null(cell.OwningRow);
        Assert.Same(control.TopLeftHeaderCell, control.TopLeftHeaderCell);
        Assert.Null(cell1.DataGridView);
        Assert.Null(cell1.OwningColumn);
        Assert.Null(cell1.OwningRow);
        Assert.Null(cell2.DataGridView);
        Assert.Null(cell2.OwningColumn);
        Assert.Null(cell2.OwningRow);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(true, true)]
    [InlineData(true, false)]
    [InlineData(false, true)]
    [InlineData(false, false)]
    public void DataGridView_TopLeftHeaderCell_Get_ReturnsExpected(bool rowHeadersVisible, bool columnHeadersVisible)
    {
        using DataGridView control = new()
        {
            RowHeadersVisible = rowHeadersVisible,
            ColumnHeadersVisible = columnHeadersVisible
        };
        DataGridViewHeaderCell cell = Assert.IsType<DataGridViewTopLeftHeaderCell>(control.TopLeftHeaderCell);
        Assert.Same(control, cell.DataGridView);
        Assert.Null(cell.OwningColumn);
        Assert.Null(cell.OwningRow);
        Assert.Same(control.TopLeftHeaderCell, control.TopLeftHeaderCell);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true, true)]
    [InlineData(true, false)]
    [InlineData(false, true)]
    [InlineData(false, false)]
    public void DataGridView_TopLeftHeaderCell_GetWithHandle_ReturnsExpected(bool rowHeadersVisible, bool columnHeadersVisible)
    {
        // Invalidation checks are omitted due to https://github.com/dotnet/winforms/issues/7799

        using DataGridView control = new()
        {
            RowHeadersVisible = rowHeadersVisible,
            ColumnHeadersVisible = columnHeadersVisible
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        DataGridViewHeaderCell cell = Assert.IsType<DataGridViewTopLeftHeaderCell>(control.TopLeftHeaderCell);
        Assert.Same(control, cell.DataGridView);
        Assert.Null(cell.OwningColumn);
        Assert.Null(cell.OwningRow);
        Assert.Same(control.TopLeftHeaderCell, control.TopLeftHeaderCell);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> OnColumnHeadersHeightChanged_TestData()
    {
        foreach (DataGridViewColumnHeadersHeightSizeMode columnHeadersWidthSizeMode in Enum.GetValues(typeof(DataGridViewColumnHeadersHeightSizeMode)))
        {
            foreach (bool columnHeadersVisible in new bool[] { true, false })
            {
                yield return new object[] { columnHeadersWidthSizeMode, columnHeadersVisible, null };
                yield return new object[] { columnHeadersWidthSizeMode, columnHeadersVisible, new EventArgs() };
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(OnColumnHeadersHeightChanged_TestData))]
    public void DataGridView_OnColumnHeadersHeightChanged_Invoke_CallsColumnHeadersHeightChanged(DataGridViewColumnHeadersHeightSizeMode columnHeadersWidthSizeMode, bool columnHeadersVisible, EventArgs eventArgs)
    {
        using SubDataGridView control = new()
        {
            ColumnHeadersHeightSizeMode = columnHeadersWidthSizeMode,
            ColumnHeadersVisible = columnHeadersVisible
        };
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.ColumnHeadersHeightChanged += handler;
        control.OnColumnHeadersHeightChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.False(control.IsHandleCreated);

        // Remove handler.
        control.ColumnHeadersHeightChanged -= handler;
        control.OnColumnHeadersHeightChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(OnColumnHeadersHeightChanged_TestData))]
    public void DataGridView_OnColumnHeadersHeightChanged_InvokeWithHandle_CallsColumnHeadersHeightChanged(DataGridViewColumnHeadersHeightSizeMode columnHeadersWidthSizeMode, bool columnHeadersVisible, EventArgs eventArgs)
    {
        // Invalidation checks are omitted due to https://github.com/dotnet/winforms/issues/7799

        using SubDataGridView control = new()
        {
            ColumnHeadersHeightSizeMode = columnHeadersWidthSizeMode,
            ColumnHeadersVisible = columnHeadersVisible
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);

        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.ColumnHeadersHeightChanged += handler;
        control.OnColumnHeadersHeightChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Remove handler.
        control.ColumnHeadersHeightChanged -= handler;
        control.OnColumnHeadersHeightChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [MemberData(nameof(OnColumnHeadersHeightChanged_TestData))]
    public void DataGridView_OnColumnHeadersHeightChanged_InvokeDisposed_DoesNotCallColumnHeadersHeightChanged(DataGridViewColumnHeadersHeightSizeMode columnHeadersWidthSizeMode, bool columnHeadersVisible, EventArgs eventArgs)
    {
        using SubDataGridView control = new()
        {
            ColumnHeadersHeightSizeMode = columnHeadersWidthSizeMode,
            ColumnHeadersVisible = columnHeadersVisible
        };
        control.Dispose();
        int callCount = 0;
        EventHandler handler = (sender, e) => callCount++;

        // Call with handler.
        control.ColumnHeadersHeightChanged += handler;
        control.OnColumnHeadersHeightChanged(eventArgs);
        Assert.Equal(0, callCount);
        Assert.False(control.IsHandleCreated);

        // Remove handler.
        control.ColumnHeadersHeightChanged -= handler;
        control.OnColumnHeadersHeightChanged(eventArgs);
        Assert.Equal(0, callCount);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(OnColumnHeadersHeightChanged_TestData))]
    public void DataGridView_OnColumnHeadersHeightChanged_InvokeInDisposing_CallsColumnHeadersHeightChanged(DataGridViewColumnHeadersHeightSizeMode columnHeadersWidthSizeMode, bool columnHeadersVisible, EventArgs eventArgs)
    {
        using SubDataGridView control = new()
        {
            ColumnHeadersHeightSizeMode = columnHeadersWidthSizeMode,
            ColumnHeadersVisible = columnHeadersVisible
        };
        int disposingCallCount = 0;
        EventHandler handler = (sender, e) =>
        {
            int callCount = 0;
            EventHandler handler = (sender, e) => callCount++;

            // Call with handler.
            control.ColumnHeadersHeightChanged += handler;
            control.OnColumnHeadersHeightChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);

            // Remove handler.
            control.ColumnHeadersHeightChanged -= handler;
            control.OnColumnHeadersHeightChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);
            disposingCallCount++;
        };
        control.Disposed += handler;
        try
        {
            control.Dispose();
            Assert.Equal(1, disposingCallCount);
        }
        finally
        {
            control.Disposed -= handler;
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(OnColumnHeadersHeightChanged_TestData))]
    public void DataGridView_OnColumnHeadersHeightChanged_InvokeInColumnDisposing_DoesNotCallColumnHeadersHeightChanged(DataGridViewColumnHeadersHeightSizeMode columnHeadersWidthSizeMode, bool columnHeadersVisible, EventArgs eventArgs)
    {
        using SubDataGridView control = new()
        {
            ColumnHeadersHeightSizeMode = columnHeadersWidthSizeMode,
            ColumnHeadersVisible = columnHeadersVisible
        };
        using SubDataGridViewCell cellTemplate = new();
        using DataGridViewColumn column = new()
        {
            CellTemplate = cellTemplate
        };
        control.Columns.Add(column);
        int disposingCallCount = 0;
        column.Disposed += (sender, e) =>
        {
            int callCount = 0;
            EventHandler handler = (sender, e) => callCount++;

            // Call with handler.
            control.ColumnHeadersHeightChanged += handler;
            control.OnColumnHeadersHeightChanged(eventArgs);
            Assert.Equal(0, callCount);
            Assert.False(control.IsHandleCreated);

            // Remove handler.
            control.ColumnHeadersHeightChanged -= handler;
            control.OnColumnHeadersHeightChanged(eventArgs);
            Assert.Equal(0, callCount);
            Assert.False(control.IsHandleCreated);
            disposingCallCount++;
        };
        control.Dispose();
        Assert.Equal(1, disposingCallCount);
    }

    public static IEnumerable<object[]> OnColumnHeadersHeightSizeModeChanged_TestData()
    {
        yield return new object[] { DataGridViewColumnHeadersHeightSizeMode.AutoSize, new DataGridViewAutoSizeModeEventArgs(true) };
        yield return new object[] { DataGridViewColumnHeadersHeightSizeMode.AutoSize, new DataGridViewAutoSizeModeEventArgs(false) };
        yield return new object[] { DataGridViewColumnHeadersHeightSizeMode.DisableResizing, new DataGridViewAutoSizeModeEventArgs(false) };
        yield return new object[] { DataGridViewColumnHeadersHeightSizeMode.EnableResizing, new DataGridViewAutoSizeModeEventArgs(false) };
    }

    [WinFormsTheory]
    [MemberData(nameof(OnColumnHeadersHeightSizeModeChanged_TestData))]
    public void DataGridView_OnColumnHeadersHeightSizeModeChanged_Invoke_CallsColumnHeadersHeightSizeModeChanged(DataGridViewColumnHeadersHeightSizeMode columnHeadersWidthSizeMode, DataGridViewAutoSizeModeEventArgs eventArgs)
    {
        using SubDataGridView control = new()
        {
            ColumnHeadersHeightSizeMode = columnHeadersWidthSizeMode
        };
        int callCount = 0;
        DataGridViewAutoSizeModeEventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.ColumnHeadersHeightSizeModeChanged += handler;
        control.OnColumnHeadersHeightSizeModeChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Equal(DefaultColumnHeadersHeight, control.ColumnHeadersHeight);
        Assert.False(control.IsHandleCreated);

        // Remove handler.
        control.ColumnHeadersHeightSizeModeChanged -= handler;
        control.OnColumnHeadersHeightSizeModeChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Equal(DefaultColumnHeadersHeight, control.ColumnHeadersHeight);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> OnColumnHeadersHeightSizeModeChanged_WithHandle_TestData()
    {
        yield return new object[] { DataGridViewColumnHeadersHeightSizeMode.AutoSize, new DataGridViewAutoSizeModeEventArgs(true), 18 };
        yield return new object[] { DataGridViewColumnHeadersHeightSizeMode.AutoSize, new DataGridViewAutoSizeModeEventArgs(false), 18 };
        yield return new object[] { DataGridViewColumnHeadersHeightSizeMode.DisableResizing, new DataGridViewAutoSizeModeEventArgs(false), DefaultColumnHeadersHeight };
        yield return new object[] { DataGridViewColumnHeadersHeightSizeMode.EnableResizing, new DataGridViewAutoSizeModeEventArgs(false), DefaultColumnHeadersHeight };
    }

    [WinFormsTheory]
    [MemberData(nameof(OnColumnHeadersHeightSizeModeChanged_WithHandle_TestData))]
    public void DataGridView_OnColumnHeadersHeightSizeModeChanged_InvokeWithHandle_CallsColumnHeadersHeightSizeModeChanged(DataGridViewColumnHeadersHeightSizeMode columnHeadersWidthSizeMode, DataGridViewAutoSizeModeEventArgs eventArgs, int expectedColumnHeadersHeight)
    {
        // Invalidation checks are omitted due to https://github.com/dotnet/winforms/issues/7799

        using SubDataGridView control = new()
        {
            ColumnHeadersHeightSizeMode = columnHeadersWidthSizeMode
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
        int callCount = 0;
        DataGridViewAutoSizeModeEventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.ColumnHeadersHeightSizeModeChanged += handler;
        control.OnColumnHeadersHeightSizeModeChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Equal(expectedColumnHeadersHeight, control.ColumnHeadersHeight);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Remove handler.
        control.ColumnHeadersHeightSizeModeChanged -= handler;
        control.OnColumnHeadersHeightSizeModeChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Equal(expectedColumnHeadersHeight, control.ColumnHeadersHeight);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(DataGridViewColumnHeadersHeightSizeMode.EnableResizing)]
    [InlineData(DataGridViewColumnHeadersHeightSizeMode.DisableResizing)]
    public void DataGridView_OnColumnHeadersHeightSizeModeChanged_InvalidEnableDisableResizingPreviousAutoModeSize_ThrowsArgumentOutOfRangeException(DataGridViewColumnHeadersHeightSizeMode columnHeadersWidthSizeMode)
    {
        using SubDataGridView control = new()
        {
            ColumnHeadersHeightSizeMode = columnHeadersWidthSizeMode
        };
        DataGridViewAutoSizeModeEventArgs eventArgs = new(true);
        Assert.Throws<ArgumentOutOfRangeException>("value", () => control.OnColumnHeadersHeightSizeModeChanged(eventArgs));
    }

    [WinFormsTheory]
    [MemberData(nameof(OnColumnHeadersHeightSizeModeChanged_TestData))]
    public void DataGridView_OnColumnHeadersHeightSizeModeChanged_InvokeDisposed_DoesNotCallColumnHeadersHeightSizeModeChanged(DataGridViewColumnHeadersHeightSizeMode columnHeadersWidthSizeMode, DataGridViewAutoSizeModeEventArgs eventArgs)
    {
        using SubDataGridView control = new()
        {
            ColumnHeadersHeightSizeMode = columnHeadersWidthSizeMode
        };
        control.Dispose();
        int callCount = 0;
        DataGridViewAutoSizeModeEventHandler handler = (sender, e) => callCount++;

        // Call with handler.
        control.ColumnHeadersHeightSizeModeChanged += handler;
        control.OnColumnHeadersHeightSizeModeChanged(eventArgs);
        Assert.Equal(0, callCount);
        Assert.Equal(DefaultColumnHeadersHeight, control.ColumnHeadersHeight);
        Assert.False(control.IsHandleCreated);

        // Remove handler.
        control.ColumnHeadersHeightSizeModeChanged -= handler;
        control.OnColumnHeadersHeightSizeModeChanged(eventArgs);
        Assert.Equal(0, callCount);
        Assert.Equal(DefaultColumnHeadersHeight, control.ColumnHeadersHeight);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(OnColumnHeadersHeightSizeModeChanged_TestData))]
    public void DataGridView_OnColumnHeadersHeightSizeModeChanged_InvokeInDisposing_CallsColumnHeadersHeightSizeModeChanged(DataGridViewColumnHeadersHeightSizeMode columnHeadersWidthSizeMode, DataGridViewAutoSizeModeEventArgs eventArgs)
    {
        using SubDataGridView control = new()
        {
            ColumnHeadersHeightSizeMode = columnHeadersWidthSizeMode
        };
        int disposingCallCount = 0;
        EventHandler handler = (sender, e) =>
        {
            int callCount = 0;
            DataGridViewAutoSizeModeEventHandler handler = (sender, e) => callCount++;

            // Call with handler.
            control.ColumnHeadersHeightSizeModeChanged += handler;
            control.OnColumnHeadersHeightSizeModeChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);

            // Remove handler.
            control.ColumnHeadersHeightSizeModeChanged -= handler;
            control.OnColumnHeadersHeightSizeModeChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);
            disposingCallCount++;
        };
        control.Disposed += handler;
        try
        {
            control.Dispose();
            Assert.Equal(1, disposingCallCount);
        }
        finally
        {
            control.Disposed -= handler;
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(OnColumnHeadersHeightSizeModeChanged_TestData))]
    public void DataGridView_OnColumnHeadersHeightSizeModeChanged_InvokeInColumnDisposing_DoesNotCallColumnHeadersHeightSizeModeChanged(DataGridViewColumnHeadersHeightSizeMode columnHeadersWidthSizeMode, DataGridViewAutoSizeModeEventArgs eventArgs)
    {
        using SubDataGridView control = new()
        {
            ColumnHeadersHeightSizeMode = columnHeadersWidthSizeMode
        };
        using SubDataGridViewCell cellTemplate = new();
        using DataGridViewColumn column = new()
        {
            CellTemplate = cellTemplate
        };
        control.Columns.Add(column);
        int disposingCallCount = 0;
        column.Disposed += (sender, e) =>
        {
            int callCount = 0;
            DataGridViewAutoSizeModeEventHandler handler = (sender, e) => callCount++;

            // Call with handler.
            control.ColumnHeadersHeightSizeModeChanged += handler;
            control.OnColumnHeadersHeightSizeModeChanged(eventArgs);
            Assert.Equal(0, callCount);
            Assert.Equal(DefaultColumnHeadersHeight, control.ColumnHeadersHeight);
            Assert.False(control.IsHandleCreated);

            // Remove handler.
            control.ColumnHeadersHeightSizeModeChanged -= handler;
            control.OnColumnHeadersHeightSizeModeChanged(eventArgs);
            Assert.Equal(0, callCount);
            Assert.Equal(DefaultColumnHeadersHeight, control.ColumnHeadersHeight);
            Assert.False(control.IsHandleCreated);

            disposingCallCount++;
        };
        control.Dispose();
        Assert.Equal(1, disposingCallCount);
    }

    [WinFormsFact]
    public void DataGridView_OnColumnHeadersHeightSizeModeChanged_NullE_ThrowsNullReferenceException()
    {
        using SubDataGridView control = new();
        Assert.Throws<NullReferenceException>(() => control.OnColumnHeadersHeightSizeModeChanged(null));
    }

    [WinFormsFact]
    [Trait("Issue", "https://github.com/dotnet/winforms/issues/3033")]
    public void DataGridView_OnFontChanged_does_not_change_user_fonts()
    {
        using Font formFont1 = new("Times New Roman", 12F, FontStyle.Regular);
        using Form form = new Form
        {
            Font = formFont1
        };

        using Font customFont1 = new("Tahoma", 8.25F, FontStyle.Regular);
        using Font customFont2 = new("Consolas", 14F, FontStyle.Italic);
        using Font customFont3 = new("Arial", 9F, FontStyle.Bold);

        DataGridViewCellStyle defaultCellStyle = new()
        {
            Font = customFont1,

            // We must supply a completely initialised instance, else we'd be receiving a copy
            // refer to DefaultCellStyle implementation

            Alignment = DataGridViewContentAlignment.MiddleLeft,
            BackColor = SystemColors.Info,
            ForeColor = Color.Maroon,
            SelectionBackColor = SystemColors.Highlight,
            SelectionForeColor = SystemColors.HighlightText,
            WrapMode = DataGridViewTriState.False
        };

        using DataGridView dataGridView = new DataGridView
        {
            DefaultCellStyle = defaultCellStyle,
            ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle { Font = customFont2 },
            RowHeadersDefaultCellStyle = new DataGridViewCellStyle { Font = customFont3 }
        };
        dataGridView.Columns.AddRange(new[] { new DataGridViewTextBoxColumn(), new DataGridViewTextBoxColumn() });
        dataGridView.Rows.Add("DefaultCellStyle", customFont1.ToString());
        dataGridView.Rows.Add("ColumnHeadersDefaultCellStyle", customFont2.ToString());
        dataGridView.Rows.Add("RowHeadersDefaultCellStyle", customFont3.ToString());

        Assert.Same(customFont1, dataGridView.DefaultCellStyle.Font);
        Assert.Same(customFont2, dataGridView.ColumnHeadersDefaultCellStyle.Font);
        Assert.Same(customFont3, dataGridView.RowHeadersDefaultCellStyle.Font);

        // Add the datagridview to the form, this will trigger Font change via OnFontChanged
        form.Controls.Add(dataGridView);

        // Ensure custom fonts are preserved
        Assert.Same(customFont1, dataGridView.DefaultCellStyle.Font);
        Assert.Same(customFont2, dataGridView.ColumnHeadersDefaultCellStyle.Font);
        Assert.Same(customFont3, dataGridView.RowHeadersDefaultCellStyle.Font);

        // Force another global font change
        using Font formFont2 = new("Arial Black", 10F, FontStyle.Italic);
        form.Font = formFont2;

        // Ensure custom fonts are preserved
        Assert.Same(customFont1, dataGridView.DefaultCellStyle.Font);
        Assert.Same(customFont2, dataGridView.ColumnHeadersDefaultCellStyle.Font);
        Assert.Same(customFont3, dataGridView.RowHeadersDefaultCellStyle.Font);

        // Ensure a user is still able to change datagridview fonts
        dataGridView.DefaultCellStyle.Font = customFont2;
        dataGridView.ColumnHeadersDefaultCellStyle.Font = customFont3;
        dataGridView.RowHeadersDefaultCellStyle.Font = customFont1;

        Assert.Same(customFont2, dataGridView.DefaultCellStyle.Font);
        Assert.Same(customFont3, dataGridView.ColumnHeadersDefaultCellStyle.Font);
        Assert.Same(customFont1, dataGridView.RowHeadersDefaultCellStyle.Font);
    }

    public static IEnumerable<object[]> OnRowHeadersWidthChanged_TestData()
    {
        foreach (DataGridViewRowHeadersWidthSizeMode rowHeadersWidthSizeMode in Enum.GetValues(typeof(DataGridViewRowHeadersWidthSizeMode)))
        {
            foreach (bool rowHeadersVisible in new bool[] { true, false })
            {
                yield return new object[] { rowHeadersWidthSizeMode, rowHeadersVisible, null };
                yield return new object[] { rowHeadersWidthSizeMode, rowHeadersVisible, new EventArgs() };
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(OnRowHeadersWidthChanged_TestData))]
    public void DataGridView_OnRowHeadersWidthChanged_Invoke_CallsRowHeadersWidthChanged(DataGridViewRowHeadersWidthSizeMode rowHeadersWidthSizeMode, bool rowHeadersVisible, EventArgs eventArgs)
    {
        using SubDataGridView control = new()
        {
            RowHeadersWidthSizeMode = rowHeadersWidthSizeMode,
            RowHeadersVisible = rowHeadersVisible
        };
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.RowHeadersWidthChanged += handler;
        control.OnRowHeadersWidthChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.False(control.IsHandleCreated);

        // Remove handler.
        control.RowHeadersWidthChanged -= handler;
        control.OnRowHeadersWidthChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(OnRowHeadersWidthChanged_TestData))]
    public void DataGridView_OnRowHeadersWidthChanged_InvokeWithHandle_CallsRowHeadersWidthChanged(DataGridViewRowHeadersWidthSizeMode rowHeadersWidthSizeMode, bool rowHeadersVisible, EventArgs eventArgs)
    {
        // Invalidation checks are omitted due to https://github.com/dotnet/winforms/issues/7799

        using SubDataGridView control = new()
        {
            RowHeadersWidthSizeMode = rowHeadersWidthSizeMode,
            RowHeadersVisible = rowHeadersVisible
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.RowHeadersWidthChanged += handler;
        control.OnRowHeadersWidthChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Remove handler.
        control.RowHeadersWidthChanged -= handler;
        control.OnRowHeadersWidthChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [MemberData(nameof(OnRowHeadersWidthChanged_TestData))]
    public void DataGridView_OnRowHeadersWidthChanged_InvokeDisposed_DoesNotCallRowHeadersWidthChanged(DataGridViewRowHeadersWidthSizeMode rowHeadersWidthSizeMode, bool rowHeadersVisible, EventArgs eventArgs)
    {
        using SubDataGridView control = new()
        {
            RowHeadersWidthSizeMode = rowHeadersWidthSizeMode,
            RowHeadersVisible = rowHeadersVisible
        };
        control.Dispose();
        int callCount = 0;
        EventHandler handler = (sender, e) => callCount++;

        // Call with handler.
        control.RowHeadersWidthChanged += handler;
        control.OnRowHeadersWidthChanged(eventArgs);
        Assert.Equal(0, callCount);
        Assert.False(control.IsHandleCreated);

        // Remove handler.
        control.RowHeadersWidthChanged -= handler;
        control.OnRowHeadersWidthChanged(eventArgs);
        Assert.Equal(0, callCount);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(OnRowHeadersWidthChanged_TestData))]
    public void DataGridView_OnRowHeadersWidthChanged_InvokeInDisposing_CallsRowHeadersWidthChanged(DataGridViewRowHeadersWidthSizeMode rowHeadersWidthSizeMode, bool rowHeadersVisible, EventArgs eventArgs)
    {
        using SubDataGridView control = new()
        {
            RowHeadersWidthSizeMode = rowHeadersWidthSizeMode,
            RowHeadersVisible = rowHeadersVisible
        };
        int disposingCallCount = 0;
        EventHandler handler = (sender, e) =>
        {
            int callCount = 0;
            EventHandler handler = (sender, e) => callCount++;

            // Call with handler.
            control.RowHeadersWidthChanged += handler;
            control.OnRowHeadersWidthChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);

            // Remove handler.
            control.RowHeadersWidthChanged -= handler;
            control.OnRowHeadersWidthChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);
            disposingCallCount++;
        };
        control.Disposed += handler;
        try
        {
            control.Dispose();
            Assert.Equal(1, disposingCallCount);
        }
        finally
        {
            control.Disposed -= handler;
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(OnRowHeadersWidthChanged_TestData))]
    public void DataGridView_OnRowHeadersWidthChanged_InvokeInColumnDisposing_DoesNotCallRowHeadersWidthChanged(DataGridViewRowHeadersWidthSizeMode rowHeadersWidthSizeMode, bool rowHeadersVisible, EventArgs eventArgs)
    {
        using SubDataGridView control = new()
        {
            RowHeadersWidthSizeMode = rowHeadersWidthSizeMode,
            RowHeadersVisible = rowHeadersVisible
        };
        using SubDataGridViewCell cellTemplate = new();
        using DataGridViewColumn column = new()
        {
            CellTemplate = cellTemplate
        };
        control.Columns.Add(column);
        int disposingCallCount = 0;
        column.Disposed += (sender, e) =>
        {
            int callCount = 0;
            EventHandler handler = (sender, e) => callCount++;

            // Call with handler.
            control.RowHeadersWidthChanged += handler;
            control.OnRowHeadersWidthChanged(eventArgs);
            Assert.Equal(0, callCount);
            Assert.False(control.IsHandleCreated);

            // Remove handler.
            control.RowHeadersWidthChanged -= handler;
            control.OnRowHeadersWidthChanged(eventArgs);
            Assert.Equal(0, callCount);
            Assert.False(control.IsHandleCreated);
            disposingCallCount++;
        };
        control.Dispose();
        Assert.Equal(1, disposingCallCount);
    }

    public static IEnumerable<object[]> OnRowHeadersWidthSizeModeChanged_TestData()
    {
        yield return new object[] { DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders, new DataGridViewAutoSizeModeEventArgs(true) };
        yield return new object[] { DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders, new DataGridViewAutoSizeModeEventArgs(false) };
        yield return new object[] { DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders, new DataGridViewAutoSizeModeEventArgs(true) };
        yield return new object[] { DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders, new DataGridViewAutoSizeModeEventArgs(false) };
        yield return new object[] { DataGridViewRowHeadersWidthSizeMode.AutoSizeToFirstHeader, new DataGridViewAutoSizeModeEventArgs(true) };
        yield return new object[] { DataGridViewRowHeadersWidthSizeMode.AutoSizeToFirstHeader, new DataGridViewAutoSizeModeEventArgs(false) };
        yield return new object[] { DataGridViewRowHeadersWidthSizeMode.DisableResizing, new DataGridViewAutoSizeModeEventArgs(false) };
        yield return new object[] { DataGridViewRowHeadersWidthSizeMode.EnableResizing, new DataGridViewAutoSizeModeEventArgs(false) };
    }

    [WinFormsTheory]
    [MemberData(nameof(OnRowHeadersWidthSizeModeChanged_TestData))]
    public void DataGridView_OnRowHeadersWidthSizeModeChanged_Invoke_CallsRowHeadersWidthSizeModeChanged(DataGridViewRowHeadersWidthSizeMode rowHeadersWidthSizeMode, DataGridViewAutoSizeModeEventArgs eventArgs)
    {
        using SubDataGridView control = new()
        {
            RowHeadersWidthSizeMode = rowHeadersWidthSizeMode
        };
        int callCount = 0;
        DataGridViewAutoSizeModeEventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.RowHeadersWidthSizeModeChanged += handler;
        control.OnRowHeadersWidthSizeModeChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Equal(DefaultRowHeadersWidth, control.RowHeadersWidth);
        Assert.False(control.IsHandleCreated);

        // Remove handler.
        control.RowHeadersWidthSizeModeChanged -= handler;
        control.OnRowHeadersWidthSizeModeChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Equal(DefaultRowHeadersWidth, control.RowHeadersWidth);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> OnRowHeadersWidthSizeModeChanged_WithHandle_TestData()
    {
        yield return new object[] { DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders, new DataGridViewAutoSizeModeEventArgs(true), 21 };
        yield return new object[] { DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders, new DataGridViewAutoSizeModeEventArgs(false), 21 };
        yield return new object[] { DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders, new DataGridViewAutoSizeModeEventArgs(true), 21 };
        yield return new object[] { DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders, new DataGridViewAutoSizeModeEventArgs(false), 21 };
        yield return new object[] { DataGridViewRowHeadersWidthSizeMode.AutoSizeToFirstHeader, new DataGridViewAutoSizeModeEventArgs(true), 21 };
        yield return new object[] { DataGridViewRowHeadersWidthSizeMode.AutoSizeToFirstHeader, new DataGridViewAutoSizeModeEventArgs(false), 21 };
        yield return new object[] { DataGridViewRowHeadersWidthSizeMode.DisableResizing, new DataGridViewAutoSizeModeEventArgs(false), DefaultRowHeadersWidth };
        yield return new object[] { DataGridViewRowHeadersWidthSizeMode.EnableResizing, new DataGridViewAutoSizeModeEventArgs(false), DefaultRowHeadersWidth };
    }

    [WinFormsTheory]
    [MemberData(nameof(OnRowHeadersWidthSizeModeChanged_WithHandle_TestData))]
    public void DataGridView_OnRowHeadersWidthSizeModeChanged_InvokeWithHandle_CallsRowHeadersWidthSizeModeChanged(DataGridViewRowHeadersWidthSizeMode rowHeadersWidthSizeMode, DataGridViewAutoSizeModeEventArgs eventArgs, int expectedRowHeadersWidth)
    {
        // Invalidation checks are omitted due to https://github.com/dotnet/winforms/issues/7799

        using SubDataGridView control = new()
        {
            RowHeadersWidthSizeMode = rowHeadersWidthSizeMode
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int styleChangedCallCount = 0;
        control.StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
        int callCount = 0;
        DataGridViewAutoSizeModeEventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.RowHeadersWidthSizeModeChanged += handler;
        control.OnRowHeadersWidthSizeModeChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Equal(expectedRowHeadersWidth, control.RowHeadersWidth);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Remove handler.
        control.RowHeadersWidthSizeModeChanged -= handler;
        control.OnRowHeadersWidthSizeModeChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Equal(expectedRowHeadersWidth, control.RowHeadersWidth);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(DataGridViewRowHeadersWidthSizeMode.EnableResizing)]
    [InlineData(DataGridViewRowHeadersWidthSizeMode.DisableResizing)]
    public void DataGridView_OnRowHeadersWidthSizeModeChanged_InvalidEnableDisableResizingPreviousAutoModeSize_ThrowsArgumentOutOfRangeException(DataGridViewRowHeadersWidthSizeMode rowHeadersWidthSizeMode)
    {
        using SubDataGridView control = new()
        {
            RowHeadersWidthSizeMode = rowHeadersWidthSizeMode
        };
        DataGridViewAutoSizeModeEventArgs eventArgs = new(true);
        Assert.Throws<ArgumentOutOfRangeException>("value", () => control.OnRowHeadersWidthSizeModeChanged(eventArgs));
    }

    [WinFormsTheory]
    [MemberData(nameof(OnRowHeadersWidthSizeModeChanged_TestData))]
    public void DataGridView_OnRowHeadersWidthSizeModeChanged_InvokeDisposed_DoesNotCallRowHeadersWidthSizeModeChanged(DataGridViewRowHeadersWidthSizeMode rowHeadersWidthSizeMode, DataGridViewAutoSizeModeEventArgs eventArgs)
    {
        using SubDataGridView control = new()
        {
            RowHeadersWidthSizeMode = rowHeadersWidthSizeMode
        };
        control.Dispose();
        int callCount = 0;
        DataGridViewAutoSizeModeEventHandler handler = (sender, e) => callCount++;

        // Call with handler.
        control.RowHeadersWidthSizeModeChanged += handler;
        control.OnRowHeadersWidthSizeModeChanged(eventArgs);
        Assert.Equal(0, callCount);
        Assert.Equal(DefaultRowHeadersWidth, control.RowHeadersWidth);
        Assert.False(control.IsHandleCreated);

        // Remove handler.
        control.RowHeadersWidthSizeModeChanged -= handler;
        control.OnRowHeadersWidthSizeModeChanged(eventArgs);
        Assert.Equal(0, callCount);
        Assert.Equal(DefaultRowHeadersWidth, control.RowHeadersWidth);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(OnRowHeadersWidthSizeModeChanged_TestData))]
    public void DataGridView_OnRowHeadersWidthSizeModeChanged_InvokeInDisposing_CallsRowHeadersWidthSizeModeChanged(DataGridViewRowHeadersWidthSizeMode rowHeadersWidthSizeMode, DataGridViewAutoSizeModeEventArgs eventArgs)
    {
        using SubDataGridView control = new()
        {
            RowHeadersWidthSizeMode = rowHeadersWidthSizeMode
        };
        int disposingCallCount = 0;
        EventHandler handler = (sender, e) =>
        {
            int callCount = 0;
            DataGridViewAutoSizeModeEventHandler handler = (sender, e) => callCount++;

            // Call with handler.
            control.RowHeadersWidthSizeModeChanged += handler;
            control.OnRowHeadersWidthSizeModeChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);

            // Remove handler.
            control.RowHeadersWidthSizeModeChanged -= handler;
            control.OnRowHeadersWidthSizeModeChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);
            disposingCallCount++;
        };
        control.Disposed += handler;
        try
        {
            control.Dispose();
            Assert.Equal(1, disposingCallCount);
        }
        finally
        {
            control.Disposed -= handler;
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(OnRowHeadersWidthSizeModeChanged_TestData))]
    public void DataGridView_OnRowHeadersWidthSizeModeChanged_InvokeInColumnDisposing_DoesNotCallRowHeadersWidthSizeModeChanged(DataGridViewRowHeadersWidthSizeMode rowHeadersWidthSizeMode, DataGridViewAutoSizeModeEventArgs eventArgs)
    {
        using SubDataGridView control = new()
        {
            RowHeadersWidthSizeMode = rowHeadersWidthSizeMode
        };
        using SubDataGridViewCell cellTemplate = new();
        using DataGridViewColumn column = new()
        {
            CellTemplate = cellTemplate
        };
        control.Columns.Add(column);
        int disposingCallCount = 0;
        column.Disposed += (sender, e) =>
        {
            int callCount = 0;
            DataGridViewAutoSizeModeEventHandler handler = (sender, e) => callCount++;

            // Call with handler.
            control.RowHeadersWidthSizeModeChanged += handler;
            control.OnRowHeadersWidthSizeModeChanged(eventArgs);
            Assert.Equal(0, callCount);
            Assert.Equal(DefaultRowHeadersWidth, control.RowHeadersWidth);
            Assert.False(control.IsHandleCreated);

            // Remove handler.
            control.RowHeadersWidthSizeModeChanged -= handler;
            control.OnRowHeadersWidthSizeModeChanged(eventArgs);
            Assert.Equal(0, callCount);
            Assert.Equal(DefaultRowHeadersWidth, control.RowHeadersWidth);
            Assert.False(control.IsHandleCreated);

            disposingCallCount++;
        };
        control.Dispose();
        Assert.Equal(1, disposingCallCount);
    }

    [WinFormsFact]
    public void DataGridView_OnRowHeadersWidthSizeModeChanged_NullE_ThrowsNullReferenceException()
    {
        using SubDataGridView control = new();
        Assert.Throws<NullReferenceException>(() => control.OnRowHeadersWidthSizeModeChanged(null));
    }

    [WinFormsFact]
    public void DataGridView_UpdatesItsItems_AfterDataSourceDisposing()
    {
        using DataGridView control = new();
        int rowsCount = 5;
        BindingSource bindingSource = GetTestBindingSource(rowsCount);
        BindingContext context = new();
        context.Add(bindingSource, bindingSource.CurrencyManager);
        control.BindingContext = context;
        control.DataSource = bindingSource;

        // The TestBindingSource table contains 2 columns
        Assert.Equal(2, control.Columns.Count);
        // The TestBindingSource table contains some rows + 1 new DGV row (because AllowUserToAddRows is true)
        Assert.Equal(rowsCount + 1, control.Rows.Count);

        bindingSource.Dispose();

        // The DataGridView updates its Rows and Columns collections after its DataSource is disposed
        Assert.Empty(control.Columns);
        Assert.Empty(control.Rows);
    }

    [WinFormsFact]
    public void DataGridView_DataSource_IsNull_AfterDisposing()
    {
        using DataGridView control = new();
        BindingSource bindingSource = GetTestBindingSource(5);
        control.DataSource = bindingSource;

        Assert.Equal(bindingSource, control.DataSource);

        bindingSource.Dispose();

        Assert.Null(control.DataSource);
    }

    [WinFormsFact]
    public void DataGridView_DataSource_IsActual_AfterOldOneIsDisposed()
    {
        using DataGridView control = new();
        int rowsCount1 = 3;
        BindingSource bindingSource1 = GetTestBindingSource(rowsCount1);
        int rowsCount2 = 5;
        BindingSource bindingSource2 = GetTestBindingSource(rowsCount2);
        BindingContext context = new();
        context.Add(bindingSource1, bindingSource1.CurrencyManager);
        control.BindingContext = context;
        control.DataSource = bindingSource1;

        Assert.Equal(bindingSource1, control.DataSource);
        Assert.Equal(2, control.Columns.Count);
        Assert.Equal(rowsCount1 + 1, control.Rows.Count); // + 1 is the new DGV row

        control.DataSource = bindingSource2;

        Assert.Equal(bindingSource2, control.DataSource);
        Assert.Equal(2, control.Columns.Count);
        Assert.Equal(rowsCount2 + 1, control.Rows.Count); // + 1 is the new DGV row

        bindingSource1.Dispose();

        // bindingSource2 is actual for the DataGridView so it will contain correct Rows and Columns counts
        // even after bindingSource1 is disposed. This test checks that Disposed events unsubscribed correctly
        Assert.Equal(bindingSource2, control.DataSource);
        Assert.Equal(2, control.Columns.Count);
        Assert.Equal(rowsCount2 + 1, control.Rows.Count); // + 1 is the new DGV row
    }

    private BindingSource GetTestBindingSource(int rowsCount)
    {
        DataTable dt = new();
        dt.Columns.Add("Name");
        dt.Columns.Add("Age");

        for (int i = 0; i < rowsCount; i++)
        {
            DataRow dr = dt.NewRow();
            dr[0] = $"User{i}";
            dr[1] = i * 3;
            dt.Rows.Add(dr);
        }

        return new() { DataSource = dt };
    }

    private class SubDataGridViewCell : DataGridViewCell
    {
    }

    private class SubDataGridView : DataGridView
    {
        public new void OnColumnHeadersHeightChanged(EventArgs e) => base.OnColumnHeadersHeightChanged(e);

        public new void OnColumnHeadersHeightSizeModeChanged(DataGridViewAutoSizeModeEventArgs e) => base.OnColumnHeadersHeightSizeModeChanged(e);

        public new void OnRowHeadersWidthChanged(EventArgs e) => base.OnRowHeadersWidthChanged(e);

        public new void OnRowHeadersWidthSizeModeChanged(DataGridViewAutoSizeModeEventArgs e) => base.OnRowHeadersWidthSizeModeChanged(e);
    }

    [WinFormsFact]
    public void DataGridView_GridColor()
    {
        using DataGridView dataGrid = new();

        int changedCount = 0;
        dataGrid.GridColorChanged += (object sender, EventArgs e) =>
        {
            changedCount++;
        };

        dataGrid.GridColor = Color.Red;

        Assert.Equal(1, changedCount);
        Assert.Equal(Color.Red, dataGrid.GridColor);
    }
}
