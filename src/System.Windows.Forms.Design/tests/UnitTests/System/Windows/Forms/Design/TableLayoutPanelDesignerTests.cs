// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using Moq;
using Moq.Protected;

namespace System.Windows.Forms.Design.Tests;

public class TableLayoutPanelDesignerTests : IDisposable
{
    private readonly TableLayoutPanelDesigner _designer;
    private readonly TableLayoutPanel _tableLayoutPanel;
    private readonly TableLayoutPanelDesigner.DesignerTableLayoutControlCollection _collection;

    public TableLayoutPanelDesignerTests()
    {
        _designer = new();
        _tableLayoutPanel = new();
        _designer.Initialize(_tableLayoutPanel);
        _collection = new(_tableLayoutPanel);
    }

    public void Dispose()
    {
        _designer.Dispose();
        _tableLayoutPanel.Dispose();
    }

    [Fact]
    public void RowCount_Property_Tests()
    {
        _tableLayoutPanel.RowCount = 5;
        _designer.RowCount.Should().Be(5);

        _designer.RowCount = 3;
        _tableLayoutPanel.RowCount.Should().Be(3);

        Action rowCountAct = () => _designer.RowCount = 0;
        rowCountAct.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void ColumnCount_Property_Tests()
    {
        _tableLayoutPanel.ColumnCount = 5;
        _designer.ColumnCount.Should().Be(5);

        _designer.ColumnCount = 3;
        _tableLayoutPanel.ColumnCount.Should().Be(3);

        Action columnCountAct = () => _designer.ColumnCount = 0;
        columnCountAct.Should().Throw<ArgumentException>();

        _designer.TestAccessor().Dynamic.Undoing = true;
        _designer.ColumnCount = 0;
        _tableLayoutPanel.ColumnCount.Should().Be(0);
    }

    [Fact]
    public void Verbs_Property_Tests()
    {
        List<DesignerVerb> verbs = _designer.Verbs.Cast<DesignerVerb>().ToList();

        verbs.Should().NotBeNull();
        verbs.Should().HaveCount(5);

        verbs.Select(v => v.Text).Should()
            .Contain(SR.TableLayoutPanelDesignerAddColumn)
            .And.Contain(SR.TableLayoutPanelDesignerAddRow)
            .And.Contain(SR.TableLayoutPanelDesignerRemoveColumn)
            .And.Contain(SR.TableLayoutPanelDesignerRemoveRow)
            .And.Contain(SR.TableLayoutPanelDesignerEditRowAndCol);
    }

    [Fact]
    public void Verbs_Property_CheckVerbStatus()
    {
        _tableLayoutPanel.ColumnCount = 1;
        _tableLayoutPanel.RowCount = 1;

        _designer.TestAccessor().Dynamic.CheckVerbStatus();
        List<DesignerVerb> verbs = _designer.Verbs.Cast<DesignerVerb>().ToList();

        verbs.Should().ContainSingle(v => v.Text == SR.TableLayoutPanelDesignerRemoveColumn && !v.Enabled);
        verbs.Should().ContainSingle(v => v.Text == SR.TableLayoutPanelDesignerRemoveRow && !v.Enabled);

        _tableLayoutPanel.ColumnCount = 2;
        _tableLayoutPanel.RowCount = 2;

        _designer.TestAccessor().Dynamic.CheckVerbStatus();
        verbs = _designer.Verbs.Cast<DesignerVerb>().ToList();

        verbs.Should().ContainSingle(v => v.Text == SR.TableLayoutPanelDesignerRemoveColumn && v.Enabled);
        verbs.Should().ContainSingle(v => v.Text == SR.TableLayoutPanelDesignerRemoveRow && v.Enabled);
    }

    [Fact]
    public void ActionLists_Should_Be_Initialized_Correctly()
    {
        _designer.TestAccessor().Dynamic._actionLists = null;
        _designer.TestAccessor().Dynamic.BuildActionLists();

        DesignerActionListCollection actionLists = _designer.ActionLists;

        actionLists.Should().NotBeNull();
        actionLists.Count.Should().BeGreaterThan(0);
        actionLists.Should().BeOfType<DesignerActionListCollection>();
    }

    [Fact]
    public void Initialize_Should_Setup_Correctly()
    {
        Mock<IDesignerHost> hostMock = new();
        Mock<IComponentChangeService> compChangeServiceMock = new();

        hostMock.Setup(h => h.GetService(typeof(IComponentChangeService)))
                .Returns(compChangeServiceMock.Object);
        hostMock.SetupAdd(h => h.TransactionClosing += It.IsAny<DesignerTransactionCloseEventHandler>());

        Mock<IServiceProvider> serviceProviderMock = new();
        serviceProviderMock.Setup(sp => sp.GetService(typeof(IDesignerHost)))
                           .Returns(hostMock.Object);

        Mock<TableLayoutPanelDesigner> designerMock = new() { CallBase = true };
        designerMock.Protected()
            .Setup<object>("GetService", ItExpr.IsAny<Type>())
            .Returns((Type serviceType) =>
            {
                if (serviceType == typeof(IDesignerHost))
                    return hostMock.Object;
                return null!;
            });

        TableLayoutPanelDesigner designer = designerMock.Object;
        designer.Initialize(_tableLayoutPanel);

        hostMock.VerifyAdd(h => h.TransactionClosing += It.IsAny<DesignerTransactionCloseEventHandler>(), Times.Once);
        compChangeServiceMock.VerifyAdd(c => c.ComponentChanging += It.IsAny<ComponentChangingEventHandler>(), Times.Once);
        compChangeServiceMock.VerifyAdd(c => c.ComponentChanged += It.IsAny<ComponentChangedEventHandler>(), Times.Once);

        _tableLayoutPanel.ControlAdded += It.IsAny<ControlEventHandler>();
        _tableLayoutPanel.ControlRemoved += It.IsAny<ControlEventHandler>();

        PropertyDescriptor rowStyleProp = (PropertyDescriptor)designer.TestAccessor().Dynamic._rowStyleProp;
        PropertyDescriptor colStyleProp = (PropertyDescriptor)designer.TestAccessor().Dynamic._colStyleProp;

        rowStyleProp.Should().NotBeNull();
        colStyleProp.Should().NotBeNull();

        InheritanceAttribute inheritanceAttribute = _designer.TestAccessor().Dynamic.InheritanceAttribute;

        if (inheritanceAttribute == InheritanceAttribute.InheritedReadOnly)
        {
            foreach (Control control in _tableLayoutPanel.Controls)
            {
                TypeDescriptor.GetAttributes(control).Contains(InheritanceAttribute.InheritedReadOnly).Should().BeTrue();
            }
        }
    }

    [Fact]
    public void InitializeNewComponent_Should_CreateEmptyTable()
    {
        Dictionary<string, object> defaultValues = new();

        _designer.InitializeNewComponent(defaultValues);

        _tableLayoutPanel.RowCount.Should().Be(2);
        _tableLayoutPanel.ColumnCount.Should().Be(2);
        _tableLayoutPanel.RowStyles.Count.Should().Be(2);
        _tableLayoutPanel.ColumnStyles.Count.Should().Be(2);
    }

    [Fact]
    public void DesignerTableLayoutControlCollection_Constructor_Should_Initialize_RealCollection()
    {
        _collection.Should().NotBeNull();
        _collection.Count.Should().Be(0);
        _collection.Should().BeAssignableTo<IList>();
    }

    [Fact]
    public void DesignerTableLayoutControlCollection_Count_Should_Return_RealCollection_Count()
    {
        using Button button = new();
        _tableLayoutPanel.Controls.Add(button);
        _collection.Count.Should().Be(1);
    }

    [Fact]
    public void DesignerTableLayoutControlCollection_IsReadOnly_Should_Return_RealCollection_IsReadOnly()
    {
        _collection.IsReadOnly.Should().Be(_tableLayoutPanel.Controls.IsReadOnly);
    }

    [Fact]
    public void DesignerTableLayoutControlCollection_Add_Should_Add_Control_To_RealCollection()
    {
        using Button button = new();
        _collection.Add(button);
        _tableLayoutPanel.Controls.Contains(button).Should().BeTrue();
    }

    [Fact]
    public void DesignerTableLayoutControlCollection_AddRange_Should_Add_Controls_To_RealCollection()
    {
        using Button button1 = new();
        using Button button2 = new();

        Control[] controls = [button1, button2];

        _collection.AddRange(controls);

        _tableLayoutPanel.Controls.Contains(button1).Should().BeTrue();
        _tableLayoutPanel.Controls.Contains(button2).Should().BeTrue();
    }

    [Fact]
    public void DesignerTableLayoutControlCollection_CopyTo_Should_Copy_Controls_To_Array()
    {
        using Button button1 = new();
        using Button button2 = new();
        _collection.Add(button1);
        _collection.Add(button2);

        Control[] array = new Control[2];
        _collection.CopyTo(array, 0);

        array.Should().Contain(button1);
        array.Should().Contain(button2);
    }

    [Fact]
    public void DesignerTableLayoutControlCollection_Equals_Tests()
    {
        TableLayoutPanelDesigner.DesignerTableLayoutControlCollection collection1 = new(_tableLayoutPanel);
        TableLayoutPanelDesigner.DesignerTableLayoutControlCollection collection2 = new(_tableLayoutPanel);
        TableLayoutPanel anotherTableLayoutPanel = new();
        TableLayoutPanelDesigner.DesignerTableLayoutControlCollection collection3 = new(anotherTableLayoutPanel);
        object nonCollectionObject = new();

        collection1.Equals(collection1).Should().BeTrue();

        collection1.Equals(collection2).Should().BeTrue();

        collection1.Equals(collection3).Should().BeTrue();

        collection1.Equals(null).Should().BeFalse();

        collection1?.Equals(nonCollectionObject).Should().BeFalse();
    }

    [Fact]
    public void DesignerTableLayoutControlCollection_GetEnumerator_Tests()
    {
        using Button button1 = new();
        using Button button2 = new();
        using Button button3 = new();

        _collection.Add(button1);
        _collection.Add(button2);
        _collection.Add(button3);

        IEnumerator enumerator = _collection.GetEnumerator();
        List<Control> controls = new();

        while (enumerator.MoveNext())
        {
            controls.Add((Control)enumerator.Current);
        }

        enumerator.Should().NotBeNull();

        enumerator.Should().BeAssignableTo<IEnumerator>();

        controls.Should().Contain(button1);
        controls.Should().Contain(button2);
        controls.Should().Contain(button3);
    }

    [Fact]
    public void DesignerTableLayoutControlCollection_Add_Control_To_Specific_Cell()
    {
        using Button button = new();
        _collection.Add(button, 1, 1);
        TableLayoutPanelCellPosition position = _tableLayoutPanel.GetPositionFromControl(button);
        position.Column.Should().Be(1);
        position.Row.Should().Be(1);
    }

    [Fact]
    public void DesignerTableLayoutControlCollection_GetChildIndex_Should_Return_Correct_Index()
    {
        using Button button1 = new();
        using Button button2 = new();
        _collection.Add(button1);
        _collection.Add(button2);
        _collection.GetChildIndex(button2).Should().Be(1);
    }

    [Fact]
    public void DesignerTableLayoutControlCollection_SetChildIndex_Should_Update_Index()
    {
        using Button button1 = new();
        using Button button2 = new();
        _collection.Add(button1);
        _collection.Add(button2);
        _collection.SetChildIndex(button1, 1);
        _collection.GetChildIndex(button1).Should().Be(1);
    }
}
