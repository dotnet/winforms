// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms.Tests;

public class DataGridViewImageColumnTests : IDisposable
{
    private readonly DataGridViewImageColumn _column = new();

    public void Dispose() => _column.Dispose();

    [Fact]
    public void Description_GetSet_RoundTrips()
    {
        _column.Description.Should().Be(string.Empty);

        _column.Description = "Test description";
        _column.Description.Should().Be("Test description");

        _column.Description = "";
        _column.Description.Should().Be(string.Empty);
    }

    [Fact]
    public void Description_Get_ThrowsIfCellTemplateIsNull()
    {
        _column.CellTemplate = null;
        Action action = () => { var _ = _column.Description; };
        action.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Description_Set_ThrowsIfCellTemplateIsNull()
    {
        _column.CellTemplate = null;
        Action action = () => _column.Description = "desc";
        action.Should().Throw<InvalidOperationException>();
    }

    [WinFormsFact]
    public void Description_Set_PropagatesToAllCells()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add(_column);
        dataGridView.Rows.Add(3);

        _column.Description = "desc1";
        foreach (DataGridViewRow row in dataGridView.Rows)
        {
            if (row.IsNewRow)
                continue;
            var cell = row.Cells[0] as DataGridViewImageCell;
            cell.Should().NotBeNull();
            cell.Description.Should().Be("desc1");
        }

        _column.Description = "desc2";
        foreach (DataGridViewRow row in dataGridView.Rows)
        {
            if (row.IsNewRow)
                continue;
            var cell = row.Cells[0] as DataGridViewImageCell;
            cell.Should().NotBeNull();
            cell.Description.Should().Be("desc2");
        }
    }

    [WinFormsFact]
    public void Description_Set_SameValue_DoesNotPropagate()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add(_column);
        dataGridView.Rows.Add(2);

        _column.Description = "foo";

        List<DataGridViewImageCell> cells = [];
        foreach (DataGridViewRow row in dataGridView.Rows)
        {
            if (row.IsNewRow)
                continue;
            var cell = row.Cells[0] as DataGridViewImageCell;
            cell.Should().NotBeNull();
            cell.Description = "foo";
            cells.Add(cell!);
        }

        _column.Description = "foo";

        foreach (var cell in cells)
        {
            cell.Description.Should().Be("foo");
        }
    }

    [Fact]
    public void Icon_GetSet_RoundTrips()
    {
        _column.Icon.Should().BeNull();

        using Icon icon = SystemIcons.Information;
        _column.Icon = icon;
        _column.Icon.Should().Be(icon);

        _column.Icon = null;
        _column.Icon.Should().BeNull();
    }

    [Fact]
    public void Image_GetSet_RoundTrips()
    {
        _column.Image.Should().BeNull();

        using Bitmap bmp = new(10, 10);
        _column.Image = bmp;
        _column.Image.Should().Be(bmp);

        _column.Image = null;
        _column.Image.Should().BeNull();
    }

    [Fact]
    public void ImageLayout_Get_DefaultsToNormal() =>
        _column.ImageLayout.Should().Be(DataGridViewImageCellLayout.Normal);

    [Fact]
    public void ImageLayout_Get_ReturnsCellTemplateValue()
    {
        using DataGridViewImageCell cellTemplate = (DataGridViewImageCell)_column.CellTemplate!;
        cellTemplate.ImageLayout = DataGridViewImageCellLayout.Stretch;

        _column.ImageLayout.Should().Be(DataGridViewImageCellLayout.Stretch);
    }

    [WinFormsFact]
    public void ImageLayout_Set_UpdatesCellTemplateAndCells()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add(_column);
        dataGridView.Rows.Add(2);

        _column.ImageLayout = DataGridViewImageCellLayout.Zoom;
        _column.CellTemplate.Should().NotBeNull();

        foreach (DataGridViewRow row in dataGridView.Rows)
        {
            if (row.IsNewRow)
                continue;
            var cell = row.Cells[0] as DataGridViewImageCell;
            cell.Should().NotBeNull();
            cell.ImageLayout.Should().Be(DataGridViewImageCellLayout.Zoom);
        }
    }

    [Fact]
    public void ValuesAreIcons_Get_DefaultsToFalse() =>
        _column.ValuesAreIcons.Should().BeFalse();

    [Theory]
    [BoolData]
    public void ValuesAreIcons_Get_ReturnsCellTemplateValue(bool valueIsIcon)
    {
        using DataGridViewImageCell cellTemplate = (DataGridViewImageCell)_column.CellTemplate!;
        cellTemplate.ValueIsIcon = valueIsIcon;

        _column.ValuesAreIcons.Should().Be(valueIsIcon);
    }

    [WinFormsFact]
    public void ValuesAreIcons_Set_UpdatesCellTemplateAndCells()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add(_column);
        dataGridView.Rows.Add(2);

        _column.ValuesAreIcons = true;
        _column.CellTemplate.Should().NotBeNull();
        ((DataGridViewImageCell)_column.CellTemplate).ValueIsIcon.Should().BeTrue();

        foreach (DataGridViewRow row in dataGridView.Rows)
        {
            if (row.IsNewRow)
                continue;
            var cell = row.Cells[0] as DataGridViewImageCell;
            cell.Should().NotBeNull();
            cell.ValueIsIcon.Should().BeTrue();
        }

        _column.ValuesAreIcons = false;
        _column.CellTemplate.Should().NotBeNull();

        foreach (DataGridViewRow row in dataGridView.Rows)
        {
            if (row.IsNewRow)
                continue;
            var cell = row.Cells[0] as DataGridViewImageCell;

            cell.Should().NotBeNull();
            cell.ValueIsIcon.Should().BeFalse();
        }
    }

    [WinFormsFact]
    public void ImageLayout_Set_SameValue_DoesNothing()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add(_column);
        dataGridView.Rows.Add(2);

        _column.ImageLayout = DataGridViewImageCellLayout.Normal;

        _column.ImageLayout = DataGridViewImageCellLayout.Stretch;

        List<DataGridViewImageCell> cells = [];
        foreach (DataGridViewRow row in dataGridView.Rows)
        {
            if (row.IsNewRow)
                continue;
            var cell = row.Cells[0] as DataGridViewImageCell;
            cell.Should().NotBeNull();
            cell.ImageLayout = DataGridViewImageCellLayout.Stretch;
            cells.Add(cell);
        }

        _column.ImageLayout = DataGridViewImageCellLayout.Stretch;

        _column.CellTemplate.Should().NotBeNull();
        ((DataGridViewImageCell)_column.CellTemplate).ImageLayout.Should().Be(DataGridViewImageCellLayout.Stretch);
        foreach (var cell in cells)
        {
            cell.ImageLayout.Should().Be(DataGridViewImageCellLayout.Stretch);
        }
    }

    [Fact]
    public void Clone_CreatesDistinctInstanceWithSameProperties()
    {
        using Icon icon = SystemIcons.Question;
        using Bitmap bmp = new(4, 4);
        _column.Icon = icon;
        _column.Image = bmp;
        _column.Description = "desc";
        _column.ValuesAreIcons = true;
        _column.ImageLayout = DataGridViewImageCellLayout.Stretch;

        DataGridViewImageColumn dataGridViewImageColumnclone = (DataGridViewImageColumn)_column.Clone();

        dataGridViewImageColumnclone.Should().NotBeSameAs(_column);
        dataGridViewImageColumnclone.Icon.Should().Be(_column.Icon);
        dataGridViewImageColumnclone.Image.Should().Be(_column.Image);
        dataGridViewImageColumnclone.Description.Should().Be(_column.Description);
        dataGridViewImageColumnclone.ValuesAreIcons.Should().Be(_column.ValuesAreIcons);
        dataGridViewImageColumnclone.ImageLayout.Should().Be(_column.ImageLayout);
        dataGridViewImageColumnclone.DefaultCellStyle.Alignment.Should().Be(_column.DefaultCellStyle.Alignment);
    }

    [Fact]
    public void Clone_TypeDerived_CreatesInstanceOfSameType()
    {
        using MyDataGridViewImageColumn derived = new();
        DataGridViewImageColumn dataGridViewImageColumnclone = (DataGridViewImageColumn)derived.Clone();

        dataGridViewImageColumnclone.Should().BeOfType<MyDataGridViewImageColumn>();
    }

    private class MyDataGridViewImageColumn : DataGridViewImageColumn { }

    [WinFormsFact]
    public void ToString_ReturnsExpectedFormat()
    {
        _column.Name = "MyImageColumn";

        _column.ToString().Should().Be("DataGridViewImageColumn { Name=MyImageColumn, Index=-1 }");

        using DataGridView dataGridView = new();
        dataGridView.Columns.Add(_column);

        _column.ToString().Should().Be($"DataGridViewImageColumn {{ Name=MyImageColumn, Index={_column.Index} }}");
    }
}
