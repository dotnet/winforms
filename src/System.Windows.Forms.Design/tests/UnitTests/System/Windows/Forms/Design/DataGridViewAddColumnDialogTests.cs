// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Reflection;
using Moq;

namespace System.Windows.Forms.Design.Tests;

public sealed class DataGridViewAddColumnDialogTests : IDisposable
{
    private readonly DataGridView _dataGridView = new();
    private readonly Container _container = new();

    public void Dispose()
    {
        _dataGridView.Dispose();
        _container.Dispose();
    }

    [Fact]
    public void Constructor_ShouldInitializeFields()
    {
        DataGridViewColumnCollection columns = _dataGridView.Columns;

        Mock<ISite> mockSite = new();
        Mock<IUIService> mockUIService = new();
        using Font expectedFont = new("Arial", 12);

        mockUIService.Setup(ui => ui.Styles["DialogFont"]).Returns(expectedFont);
        mockSite.Setup(site => site.GetService(typeof(IUIService))).Returns(mockUIService.Object);
        _dataGridView.Site = mockSite.Object;

        using DataGridViewAddColumnDialog dialog = new(columns, _dataGridView);

        DataGridViewColumnCollection dataGridViewColumns = (DataGridViewColumnCollection)dialog.TestAccessor().Dynamic._dataGridViewColumns;
        using DataGridView liveDataGridView = (DataGridView)dialog.TestAccessor().Dynamic._liveDataGridView;

        dialog.Should().NotBeNull();
        dataGridViewColumns.Should().BeSameAs(columns);
        liveDataGridView.Should().BeSameAs(_dataGridView);
        dialog.Font.Should().Be(expectedFont);
    }

    [WinFormsTheory]
    [InlineData(typeof(NoDesignerAttributeClass), null, null)]
    [InlineData(typeof(WithDesignerAttributeClass), typeof(MockComponentDesigner), true)]
    [InlineData(typeof(WithDesignerAttributeClass), typeof(MockComponentDesigner), false)]
    public void GetComponentDesignerForType_ShouldReturnExpectedResult(Type type, Type? expectedDesignerType, bool? useTypeResolutionService)
    {
        ITypeResolutionService? typeResolutionService = useTypeResolutionService.HasValue && useTypeResolutionService.Value ? new MockTypeResolutionService() : null;

        using ComponentDesigner? result = DataGridViewAddColumnDialog.GetComponentDesignerForType(typeResolutionService, type);

        if (expectedDesignerType is null)
        {
            result.Should().BeNull();
        }
        else
        {
            result.Should().NotBeNull();
            result.Should().BeOfType(expectedDesignerType);
        }
    }

    private class NoDesignerAttributeClass
    {
    }

    [Designer(typeof(MockComponentDesigner))]
    private class WithDesignerAttributeClass
    {
    }

    private class MockComponentDesigner : ComponentDesigner
    {
    }

    private class MockTypeResolutionService : ITypeResolutionService
    {
        public Assembly GetAssembly(AssemblyName name) => throw new NotImplementedException();
        public Assembly GetAssembly(AssemblyName name, bool throwOnError) => throw new NotImplementedException();
        public string GetPathOfAssembly(AssemblyName name) => throw new NotImplementedException();
        public Type? GetType(string name) => Type.GetType(name);
        public Type? GetType(string name, bool throwOnError) => Type.GetType(name, throwOnError);
        public Type? GetType(string name, bool throwOnError, bool ignoreCase) => Type.GetType(name, throwOnError, ignoreCase);
        public void ReferenceAssembly(AssemblyName name) => throw new NotImplementedException();
    }

    [WinFormsTheory]
    [InlineData("ValidName", false, false, false, true, true)]
    [InlineData("DuplicateColumnName", true, false, false, true, false)]
    [InlineData("DuplicateComponentName", false, true, false, true, false)]
    [InlineData("InvalidIdentifier", false, false, true, true, false)]
    [InlineData("DuplicateLiveColumnName", false, true, false, false, false)]
    public void ValidName_ShouldReturnExpectedResult(string name, bool columnContains, bool containerContains, bool invalidIdentifier, bool allowDuplicateNameInLiveColumnCollection, bool expectedResult)
    {
        DataGridViewColumnCollection columns = _dataGridView.Columns;
        if (columnContains)
        {
            columns.Add(new DataGridViewTextBoxColumn { Name = name });
        }

        if (containerContains)
        {
            _container.Add(new Component(), name);
        }

        NameCreationService nameCreationService = new(invalidIdentifier);
        DataGridViewColumnCollection liveColumns = new(new DataGridView());
        if (!allowDuplicateNameInLiveColumnCollection)
        {
            liveColumns.Add(new DataGridViewTextBoxColumn { Name = name });
        }

        bool result = DataGridViewAddColumnDialog.ValidName(name, columns, _container, nameCreationService, liveColumns, allowDuplicateNameInLiveColumnCollection);

        result.Should().Be(expectedResult);
    }

    private class NameCreationService : INameCreationService
    {
        private readonly bool _invalidIdentifier;

        public NameCreationService(bool invalidIdentifier)
        {
            _invalidIdentifier = invalidIdentifier;
        }

        public string CreateName(IContainer? container, Type dataType) => throw new NotImplementedException();
        public bool IsValidName(string name) => !_invalidIdentifier;
        public void ValidateName(string name) => throw new NotImplementedException();
    }
}
