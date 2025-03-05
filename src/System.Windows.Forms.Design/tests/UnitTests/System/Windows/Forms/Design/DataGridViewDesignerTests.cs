// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.ComponentModel.Design;
using Moq;

namespace System.Windows.Forms.Design.Tests;

public class DataGridViewDesignerTests : IDisposable
{
    private readonly DataGridViewDesigner _designer;
    private readonly DataGridView _dataGridView;

    public DataGridViewDesignerTests()
    {
        _designer = new();
        _dataGridView = new();
        _designer.Initialize(_dataGridView);
    }

    public void Dispose()
    {
        _designer.Dispose();
        _dataGridView.Dispose();
    }

    [Fact]
    public void Control_ReturnsDataGridView()
    {
        _designer.Control.Should().NotBeNull();
        _designer.Control.Should().BeOfType<DataGridView>();
    }

    [Fact]
    public void Constructor_SetsAutoResizeHandlesToTrue() =>
        _designer.AutoResizeHandles.Should().BeTrue();

    [Fact]
    public void AssociatedComponents_ReturnsDataGridViewColumns() =>
        _designer.AssociatedComponents.Should().BeSameAs(_dataGridView.Columns);

    [Fact]
    public void AutoSizeColumnsMode_Get_ReturnsExpected()
    {
        _dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        _designer.AutoSizeColumnsMode.Should().Be(DataGridViewAutoSizeColumnsMode.Fill);
    }

    [Fact]
    public void AutoSizeColumnsMode_Set_CallsComponentChangeService()
    {
        Mock<IComponentChangeService> mockChangeService = new();
        Mock<ISite> site = new();
        site.Setup(s => s.GetService(typeof(IComponentChangeService))).Returns(mockChangeService.Object);
        _dataGridView.Site = site.Object;

        _designer.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

        mockChangeService.Verify(
            s => s.OnComponentChanging(
                It.IsAny<object>(),
                It.IsAny<PropertyDescriptor>()
            ),
            Times.Exactly(_dataGridView.Columns.Count)
        );

        mockChangeService.Verify(
            s => s.OnComponentChanged(
                It.IsAny<object>(),
                It.IsAny<PropertyDescriptor>(),
                It.IsAny<object>(),
                It.IsAny<object>()
            ),
            Times.Exactly(_dataGridView.Columns.Count)
        );

        _dataGridView.AutoSizeColumnsMode.Should().Be(DataGridViewAutoSizeColumnsMode.Fill);
    }

    [Fact]
    public void DataSource_Get_ReturnsExpected()
    {
        object dataSource = new();
        _dataGridView.DataSource = dataSource;

        _designer.DataSource.Should().BeSameAs(dataSource);
    }

    [Fact]
    public void DataSource_Set_SetsExpected()
    {
        object dataSource = new();
        _designer.DataSource = dataSource;

        _dataGridView.DataSource.Should().BeSameAs(dataSource);
        _dataGridView.AutoGenerateColumns.Should().BeFalse();
    }

    [Fact]
    public void DataSource_Set_Null_SetsExpected()
    {
        _dataGridView.AutoGenerateColumns = false;
        _designer.DataSource = null;

        _dataGridView.DataSource.Should().BeNull();
        _dataGridView.AutoGenerateColumns.Should().BeFalse();
    }

    [Fact]
    public void Initialize_SetsAutoGenerateColumnsCorrectly() =>
        _dataGridView.AutoGenerateColumns.Should().BeTrue();

    [Fact]
    public void InitializeNewComponent_SetsColumnHeadersHeightSizeModeToAutoSize()
    {
        _designer.InitializeNewComponent(null);

        _dataGridView.ColumnHeadersHeightSizeMode.Should().Be(DataGridViewColumnHeadersHeightSizeMode.AutoSize);
    }

    [Fact]
    public void Verbs_ReturnsExpected()
    {
        DesignerVerbCollection verbs = _designer.Verbs;

        verbs.Should().NotBeNull();
        verbs.Count.Should().Be(2);
        verbs[0]!.Text.Should().Be(SR.DataGridViewEditColumnsVerb);
        verbs[1]!.Text.Should().Be(SR.DataGridViewAddColumnVerb);
    }

    [Fact]
    public void ActionLists_CachesActionLists()
    {
        DesignerActionListCollection actionLists1 = _designer.ActionLists;
        DesignerActionListCollection actionLists2 = _designer.ActionLists;

        actionLists1.Should().BeSameAs(actionLists2);
    }

    [Fact]
    public void PreFilterProperties_ShadowsPropertiesCorrectly()
    {
        Dictionary<string, PropertyDescriptor> properties = new()
        {
            { "AutoSizeColumnsMode", TypeDescriptor.GetProperties(typeof(DataGridView))["AutoSizeColumnsMode"]!},
            { "DataSource", TypeDescriptor.GetProperties(typeof(DataGridView))["DataSource"]!}
        };

        _designer.TestAccessor().Dynamic.PreFilterProperties(properties);

        properties["AutoSizeColumnsMode"].ComponentType.Should().Be(typeof(DataGridViewDesigner));
        properties["DataSource"].ComponentType.Should().Be(typeof(DataGridViewDesigner));
    }
}
