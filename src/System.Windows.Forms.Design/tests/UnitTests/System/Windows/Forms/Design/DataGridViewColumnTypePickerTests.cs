// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel.Design;
using System.Drawing;
using Moq;

namespace System.Windows.Forms.Design.Tests;

public class DataGridViewColumnTypePickerTests : IDisposable
{
    private readonly DataGridViewColumnTypePicker _picker;

    public DataGridViewColumnTypePickerTests()
    {
        _picker = new();
    }

    public void Dispose() => _picker.Dispose();

    [Fact]
    public void Constructor_ShouldInitializeListBoxCorrectly()
    {
        _picker.Controls.Count.Should().Be(1);
        _picker.Controls[0].Should().BeOfType<ListBox>().Which.Should().Match<ListBox>(listBox =>
            listBox.Dock == DockStyle.Fill
            && listBox.Sorted
            && listBox.HorizontalScrollbar);
    }

    [Fact]
    public void Constructor_ShouldSetBackColorAndActiveControl()
    {
        _picker.BackColor.Should().Be(SystemColors.Control);
        _picker.ActiveControl.Should().Be(_picker.Controls[0]);
    }

    [Fact]
    public void SelectedType_ShouldReturnNull_WhenNotSet() =>
        _picker.SelectedType.Should().BeNull();

    [Fact]
    public void Start_ShouldPopulateListBoxItems()
    {
        Mock<IWindowsFormsEditorService> editorServiceMock = new();
        Mock<ITypeDiscoveryService> discoveryServiceMock = new();
        List<Type> types = [typeof(DataGridViewTextBoxColumn), typeof(DataGridViewButtonColumn)];
        discoveryServiceMock.Setup(ds => ds.GetTypes(It.IsAny<Type>(), It.IsAny<bool>())).Returns(types);

        _picker.Start(editorServiceMock.Object, discoveryServiceMock.Object, typeof(DataGridViewTextBoxColumn));

        var listBox = _picker.TestAccessor().Dynamic._typesListBox;
        object count = listBox.Items.Count;
        count.Should().Be(2);
    }

    [Fact]
    public void SetBoundsCore_ShouldSetMinimumWidthAndHeight()
    {
        var accessor = _picker.TestAccessor().Dynamic;
        accessor.SetBoundsCore(0, 0, 50, 50, BoundsSpecified.All);

        _picker.Width.Should().BeGreaterThanOrEqualTo(100);
        _picker.Height.Should().BeGreaterThanOrEqualTo(90);
    }

    [Fact]
    public void typesListBox_SelectedIndexChanged_ShouldSetSelectedType()
    {
        Mock<IWindowsFormsEditorService> editorServiceMock = new();
        Mock<ITypeDiscoveryService> discoveryServiceMock = new();
        List<Type> types = [typeof(DataGridViewTextBoxColumn), typeof(DataGridViewButtonColumn)];
        discoveryServiceMock.Setup(ds => ds.GetTypes(It.IsAny<Type>(), It.IsAny<bool>())).Returns(types);

        _picker.Start(editorServiceMock.Object, discoveryServiceMock.Object, typeof(DataGridViewTextBoxColumn));
        var listBox = _picker.TestAccessor().Dynamic._typesListBox;
        listBox.SelectedIndex = 1;

        _picker.TestAccessor().Dynamic.typesListBox_SelectedIndexChanged(listBox, EventArgs.Empty);

        _picker.SelectedType.Should().Be(typeof(DataGridViewTextBoxColumn));
    }
}
