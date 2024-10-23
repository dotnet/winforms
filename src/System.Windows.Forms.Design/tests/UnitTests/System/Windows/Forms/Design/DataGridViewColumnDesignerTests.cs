// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using Moq;
using System.ComponentModel.Design;
using System.ComponentModel;

namespace System.Windows.Forms.Design.Tests;

public class DataGridViewColumnDesignerTests
{
    [Fact]
    public void Initialize_SetsUpServices()
    {
        using DataGridViewColumn dataGridViewColumn = new();
        Mock<ISite> siteMock = new();
        Mock<ISelectionService> selectionServiceMock = new();
        Mock<IDesignerHost> designerHostMock = new();

        siteMock
            .Setup(s => s.GetService(typeof(ISelectionService)))
            .Returns(selectionServiceMock.Object);
        siteMock
            .Setup(s => s.GetService(typeof(IDesignerHost)))
            .Returns(designerHostMock.Object);

        dataGridViewColumn.Site = siteMock.Object;

        using DataGridViewColumnDesigner designer = new();

        designer.Initialize(dataGridViewColumn);

        designer.Should().NotBeNull().And.BeOfType<DataGridViewColumnDesigner>();
    }

    [Fact]
    public void NameProperty_GetAndSet()
    {
        using DataGridViewColumn column = new();
        Mock<ISite> siteMock = new();
        using DataGridViewColumnDesigner designer = new();
        designer.Initialize(column);
        column.Site = siteMock.Object;

        designer.TestAccessor().Dynamic.Name = "NewColumnName";

        column.Name.Should().Be("NewColumnName");
    }

    [Fact]
    public void WidthProperty_GetAndSet()
    {
        using DataGridViewColumn column = new();
        using DataGridViewColumnDesigner designer = new();
        designer.Initialize(column);

        designer.TestAccessor().Dynamic.Width = 150;

        column.Width.Should().Be(150);
    }

    [Fact]
    public void LiveDataGridView_Set()
    {
        using DataGridView dataGridView = new();
        using DataGridViewColumnDesigner designer = new();

        designer.LiveDataGridView = dataGridView;

        ((DataGridView)designer.TestAccessor().Dynamic._liveDataGridView).Should().Be(dataGridView);
    }
}
