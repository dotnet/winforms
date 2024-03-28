// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using Moq;
using System.Windows.Forms.TestUtilities;

namespace System.Windows.Forms.Tests;

public class DataGridViewElementTests
{
    [WinFormsFact]
    public void DataGridViewElement_Ctor_Default()
    {
        DataGridViewElement element = new();
        Assert.Null(element.DataGridView);
        Assert.Equal(DataGridViewElementStates.Visible, element.State);
    }

    [WinFormsFact]
    public void DataGridViewElement_OnDataGridViewChanged_Invoke_Nop()
    {
        SubDataGridViewElement element = new();
        element.OnDataGridViewChanged();
    }

    public static IEnumerable<object[]> DataGridViewCellEventArgs_TestData()
    {
        yield return new object[] { null };
        yield return new object[] { new DataGridViewCellEventArgs(1, 2) };
    }

    public static IEnumerable<object[]> DataGridViewElement_Subclasses_SuppressFinalizeCall_TestData()
    {
        foreach (var type in typeof(DataGridViewElement).Assembly.GetTypes().Where(type =>
            type == typeof(DataGridViewBand) || type == typeof(DataGridViewCell) ||
            type.IsSubclassOf(typeof(DataGridViewBand)) || type.IsSubclassOf(typeof(DataGridViewCell))))
        {
            yield return new object[] { type };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(DataGridViewCellEventArgs_TestData))]
    public void DataGridViewElement_RaiseCellClick_Invoke_Nop(DataGridViewCellEventArgs eventArgs)
    {
        SubDataGridViewElement element = new();
        element.RaiseCellClick(eventArgs);
    }

    [WinFormsFact]
    public void DataGridViewElement_RaiseCellClick_InvokeWithDataGridView_Success()
    {
        DataGridViewCellEventArgs eventArgs = new(0, 0);
        using DataGridView control = new()
        {
            RowCount = 1,
            ColumnCount = 1
        };
        SubDataGridViewCell element = new();
        control.Rows[0].Cells[0] = element;

        int callCount = 0;
        void handler(object sender, EventArgs e)
        {
            Assert.Same(sender, control);
            Assert.Same(eventArgs, e);
            callCount++;
        }

        // Call with handler.
        control.CellClick += handler;
        element.RaiseCellClick(eventArgs);
        Assert.Equal(1, callCount);

        // Remove handler.
        control.CellClick -= handler;
        element.RaiseCellClick(eventArgs);
        Assert.Equal(1, callCount);
    }

    [WinFormsFact]
    public void DataGridViewElement_RaiseCellClick_NullE_ThrowsNullReferenceException()
    {
        using DataGridView control = new()
        {
            RowCount = 1,
            ColumnCount = 1
        };
        SubDataGridViewCell element = new();
        control.Rows[0].Cells[0] = element;
        Assert.Throws<NullReferenceException>(() => element.RaiseCellClick(null));
    }

    [WinFormsTheory]
    [MemberData(nameof(DataGridViewCellEventArgs_TestData))]
    public void DataGridViewElement_RaiseCellContentClick_Invoke_Nop(DataGridViewCellEventArgs eventArgs)
    {
        SubDataGridViewElement element = new();
        element.RaiseCellContentClick(eventArgs);
    }

    [WinFormsFact]
    public void DataGridViewElement_RaiseCellContentClick_InvokeWithDataGridView_Success()
    {
        DataGridViewCellEventArgs eventArgs = new(0, 0);
        using DataGridView control = new()
        {
            RowCount = 1,
            ColumnCount = 1
        };
        SubDataGridViewCell element = new();
        control.Rows[0].Cells[0] = element;

        int callCount = 0;
        void handler(object sender, EventArgs e)
        {
            Assert.Same(sender, control);
            Assert.Same(eventArgs, e);
            callCount++;
        }

        // Call with handler.
        control.CellContentClick += handler;
        element.RaiseCellContentClick(eventArgs);
        Assert.Equal(1, callCount);

        // Remove handler.
        control.CellContentClick -= handler;
        element.RaiseCellContentClick(eventArgs);
        Assert.Equal(1, callCount);
    }

    [WinFormsFact]
    public void DataGridViewElement_RaiseCellContentClick_NullE_ThrowsNullReferenceException()
    {
        using DataGridView control = new()
        {
            RowCount = 1,
            ColumnCount = 1
        };
        SubDataGridViewCell element = new();
        control.Rows[0].Cells[0] = element;
        Assert.Throws<NullReferenceException>(() => element.RaiseCellContentClick(null));
    }

    [WinFormsTheory]
    [MemberData(nameof(DataGridViewCellEventArgs_TestData))]
    public void DataGridViewElement_RaiseCellContentDoubleClick_Invoke_Nop(DataGridViewCellEventArgs eventArgs)
    {
        SubDataGridViewElement element = new();
        element.RaiseCellContentDoubleClick(eventArgs);
    }

    [WinFormsFact]
    public void DataGridViewElement_RaiseCellContentDoubleClick_InvokeWithDataGridView_Success()
    {
        DataGridViewCellEventArgs eventArgs = new(0, 0);
        using DataGridView control = new()
        {
            RowCount = 1,
            ColumnCount = 1
        };
        SubDataGridViewCell element = new();
        control.Rows[0].Cells[0] = element;

        int callCount = 0;
        void handler(object sender, EventArgs e)
        {
            Assert.Same(sender, control);
            Assert.Same(eventArgs, e);
            callCount++;
        }

        // Call with handler.
        control.CellContentDoubleClick += handler;
        element.RaiseCellContentDoubleClick(eventArgs);
        Assert.Equal(1, callCount);

        // Remove handler.
        control.CellContentDoubleClick -= handler;
        element.RaiseCellContentDoubleClick(eventArgs);
        Assert.Equal(1, callCount);
    }

    [WinFormsFact]
    public void DataGridViewElement_RaiseCellContentDoubleClick_NullE_ThrowsNullReferenceException()
    {
        using DataGridView control = new()
        {
            RowCount = 1,
            ColumnCount = 1
        };
        SubDataGridViewCell element = new();
        control.Rows[0].Cells[0] = element;
        Assert.Throws<NullReferenceException>(() => element.RaiseCellContentDoubleClick(null));
    }

    [WinFormsTheory]
    [MemberData(nameof(DataGridViewCellEventArgs_TestData))]
    public void DataGridViewElement_RaiseCellValueChanged_Invoke_Nop(DataGridViewCellEventArgs eventArgs)
    {
        SubDataGridViewElement element = new();
        element.RaiseCellValueChanged(eventArgs);
    }

    [WinFormsFact]
    public void DataGridViewElement_RaiseCellValueChanged_InvokeWithDataGridView_Success()
    {
        DataGridViewCellEventArgs eventArgs = new(0, 0);
        using DataGridView control = new()
        {
            RowCount = 1,
            ColumnCount = 1
        };
        SubDataGridViewCell element = new();
        control.Rows[0].Cells[0] = element;

        int callCount = 0;
        void handler(object sender, EventArgs e)
        {
            Assert.Same(sender, control);
            Assert.Same(eventArgs, e);
            callCount++;
        }

        // Call with handler.
        control.CellValueChanged += handler;
        element.RaiseCellValueChanged(eventArgs);
        Assert.Equal(1, callCount);

        // Remove handler.
        control.CellValueChanged -= handler;
        element.RaiseCellValueChanged(eventArgs);
        Assert.Equal(1, callCount);
    }

    [WinFormsFact]
    public void DataGridViewElement_RaiseCellValueChanged_NullE_ThrowsNullReferenceException()
    {
        using DataGridView control = new()
        {
            RowCount = 1,
            ColumnCount = 1
        };
        SubDataGridViewCell element = new();
        control.Rows[0].Cells[0] = element;
        Assert.Throws<NullReferenceException>(() => element.RaiseCellValueChanged(null));
    }

    public static IEnumerable<object[]> DataGridViewDataErrorEventArgs_TestData()
    {
        yield return new object[] { null };
        yield return new object[] { new DataGridViewDataErrorEventArgs(new InvalidOperationException(), 1, 2, DataGridViewDataErrorContexts.Formatting) };
    }

    [WinFormsTheory]
    [MemberData(nameof(DataGridViewDataErrorEventArgs_TestData))]
    public void DataGridViewElement_RaiseDataError_Invoke_Nop(DataGridViewDataErrorEventArgs eventArgs)
    {
        SubDataGridViewElement element = new();
        element.RaiseDataError(eventArgs);
    }

    [WinFormsFact]
    public void DataGridViewElement_RaiseDataError_InvokeWithDataGridView_Success()
    {
        DataGridViewDataErrorEventArgs eventArgs = new(new InvalidOperationException(), 1, 2, DataGridViewDataErrorContexts.Formatting);
        Mock<ISite> mockSite = new(MockBehavior.Strict);
        mockSite
            .Setup(s => s.Container)
            .Returns((IContainer)null);
        mockSite
            .Setup(s => s.DesignMode)
            .Returns(true);
        mockSite
            .Setup(s => s.GetService(typeof(AmbientProperties)))
            .Returns(new AmbientProperties());
        mockSite
            .Setup(s => s.Name)
            .Returns((string)null);
        using DataGridView control = new()
        {
            RowCount = 1,
            ColumnCount = 1,
            Site = mockSite.Object
        };
        SubDataGridViewCell element = new();
        control.Rows[0].Cells[0] = element;

        int callCount = 0;
        void handler(object sender, EventArgs e)
        {
            Assert.Same(sender, control);
            Assert.Same(eventArgs, e);
            callCount++;
        }

        // Call with handler.
        control.DataError += handler;
        element.RaiseDataError(eventArgs);
        Assert.Equal(1, callCount);

        // Remove handler.
        control.DataError -= handler;
        element.RaiseDataError(eventArgs);
        Assert.Equal(1, callCount);
    }

    [WinFormsFact]
    public void DataGridViewElement_RaiseDataError_NullE_ThrowsNullReferenceException()
    {
        using DataGridView control = new()
        {
            RowCount = 1,
            ColumnCount = 1
        };
        SubDataGridViewCell element = new();
        control.Rows[0].Cells[0] = element;
        Assert.Throws<NullReferenceException>(() => element.RaiseDataError(null));
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetMouseEventArgsTheoryData))]
    public void DataGridViewElement_RaiseMouseWheel_Invoke_Nop(MouseEventArgs eventArgs)
    {
        SubDataGridViewElement element = new();
        element.RaiseMouseWheel(eventArgs);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetMouseEventArgsTheoryData))]
    public void DataGridViewElement_RaiseMouseWheel_InvokeWithDataGridView_Success(MouseEventArgs eventArgs)
    {
        using DataGridView control = new()
        {
            RowCount = 1,
            ColumnCount = 1
        };
        SubDataGridViewCell element = new();
        control.Rows[0].Cells[0] = element;

        int callCount = 0;
        void handler(object sender, EventArgs e)
        {
            Assert.Same(sender, control);
            Assert.Same(eventArgs, e);
            callCount++;
        }

        // Call with handler.
        control.MouseWheel += handler;
        element.RaiseMouseWheel(eventArgs);
        Assert.Equal(1, callCount);

        // Remove handler.
        control.MouseWheel -= handler;
        element.RaiseMouseWheel(eventArgs);
        Assert.Equal(1, callCount);
    }

    [WinFormsTheory]
    [MemberData(nameof(DataGridViewElement_Subclasses_SuppressFinalizeCall_TestData))]
    public void DataGridViewElement_Subclasses_SuppressFinalizeCall(Type type)
    {
        Assert.True(type == typeof(DataGridViewBand) || type == typeof(DataGridViewColumn) ||
            type == typeof(DataGridViewButtonColumn) || type == typeof(DataGridViewCheckBoxColumn) ||
            type == typeof(DataGridViewComboBoxColumn) || type == typeof(DataGridViewImageColumn) ||
            type == typeof(DataGridViewLinkColumn) || type == typeof(DataGridViewTextBoxColumn) ||
            type == typeof(DataGridViewRow) || type == typeof(DataGridViewCell) || type == typeof(DataGridViewButtonCell) ||
            type == typeof(DataGridViewCheckBoxCell) || type == typeof(DataGridViewComboBoxCell) ||
            type == typeof(DataGridViewHeaderCell) || type == typeof(DataGridViewColumnHeaderCell) ||
            type == typeof(DataGridViewTopLeftHeaderCell) || type == typeof(DataGridViewRowHeaderCell) ||
            type == typeof(DataGridViewImageCell) || type == typeof(DataGridViewLinkCell) || type == typeof(DataGridViewTextBoxCell),
            $"Type {type} is not one of known {nameof(DataGridViewElement)} subclauses with empty finalizers. " +
            $"Consider adding it here and to the {nameof(DataGridViewElement)}() constructor. " +
            $"Or add exclusion to this test (if a new class really needs a finalizer).");
    }

    private class SubDataGridViewCell : DataGridViewCell
    {
        public new void RaiseCellClick(DataGridViewCellEventArgs e) => base.RaiseCellClick(e);

        public new void RaiseCellContentClick(DataGridViewCellEventArgs e) => base.RaiseCellContentClick(e);

        public new void RaiseCellContentDoubleClick(DataGridViewCellEventArgs e) => base.RaiseCellContentDoubleClick(e);

        public new void RaiseCellValueChanged(DataGridViewCellEventArgs e) => base.RaiseCellValueChanged(e);

        public new void RaiseDataError(DataGridViewDataErrorEventArgs e) => base.RaiseDataError(e);

        public new void RaiseMouseWheel(MouseEventArgs e) => base.RaiseMouseWheel(e);
    }

    private class SubDataGridViewElement : DataGridViewElement
    {
        public new void OnDataGridViewChanged() => base.OnDataGridViewChanged();

        public new void RaiseCellClick(DataGridViewCellEventArgs e) => base.RaiseCellClick(e);

        public new void RaiseCellContentClick(DataGridViewCellEventArgs e) => base.RaiseCellContentClick(e);

        public new void RaiseCellContentDoubleClick(DataGridViewCellEventArgs e) => base.RaiseCellContentDoubleClick(e);

        public new void RaiseCellValueChanged(DataGridViewCellEventArgs e) => base.RaiseCellValueChanged(e);

        public new void RaiseDataError(DataGridViewDataErrorEventArgs e) => base.RaiseDataError(e);

        public new void RaiseMouseWheel(MouseEventArgs e) => base.RaiseMouseWheel(e);
    }
}
