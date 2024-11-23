// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using Moq;

namespace System.Windows.Forms.Tests;

// NB: doesn't require thread affinity
public class PropertyManagerTests
{
    [Fact]
    public void PropertyManager_Ctor_Default()
    {
        PropertyManager manager = new();
        Assert.Equal(1, manager.Count);
        Assert.Equal(0, manager.Position);
        Assert.Null(manager.Current);
        Assert.Empty(manager.GetListName(null));
    }

    [Theory]
    [IntegerData<int>]
    public void PropertyManager_Position_Set_Nop(int value)
    {
        BindingContext context = [];
        PropertyManager manager = Assert.IsType<PropertyManager>(context[new DataSource()]);
        manager.Position = value;
        Assert.Equal(0, manager.Position);
    }

    [Fact]
    public void PropertyManager_GetListName_Invoke_ReturnsEmpty()
    {
        BindingContext context = [];
        PropertyManager manager = Assert.IsType<PropertyManager>(context[new DataSource()]);
        Assert.Equal("System.Windows.Forms.Tests.PropertyManagerTests+DataSource.", manager.GetListName());
        Assert.Empty(manager.GetListName(null));
    }

    [Fact]
    public void PropertyManager_GetListName_NoDataSource_ThrowsArgumentException()
    {
        PropertyManager manager = new();
        Assert.Throws<ArgumentException>(manager.GetListName);
    }

    /*
    public static IEnumerable<object[]> GetItemProperties_Parameterless_TestData()
    {
        yield return new object[] { new PropertyManager(), Array.Empty<string>() };

        BindingContext singleContext = new();
        yield return new object[] { singleContext[new DataSource()], new string[] { "Property" } };

        BindingContext listContext = new();
        yield return new object[] { listContext[new ListDataSource()], new string[] { "ListProperty" } };
    }

    [Theory]
    [MemberData(nameof(GetItemProperties_Parameterless_TestData))]
    public void PropertyManager_GetItemProperties_InvokeParameterless_ReturnsExpected(PropertyManager manager, string[] expected)
    {
        IEnumerable<PropertyDescriptor> properties = manager.GetItemProperties()?.Cast<PropertyDescriptor>();
        Assert.Equal(expected, properties?.Select(p => p.Name));
    }

    public static IEnumerable<object[]> GetItemProperties_DataSourcesAndListAccessors_TestData()
    {
        yield return new object[] { new PropertyManager(), null, null, Array.Empty<string>() };
        yield return new object[] { new PropertyManager(), new ArrayList(), new ArrayList(), Array.Empty<string>() };

        BindingContext singleContext = new();
        yield return new object[] { singleContext[new DataSource()], null, null, new string[] { "Property" } };
        yield return new object[] { singleContext[new DataSource()], null, new ArrayList(), new string[] { "Property" } };

        BindingContext listContext = new();
        yield return new object[] { listContext[new ListDataSource()], null, new ArrayList(), new string[] { "ListProperty" } };
        yield return new object[] { listContext[new ListDataSource()], null, new ArrayList(TypeDescriptor.GetProperties(typeof(ListDataSource))), new string[] { "Property" } };
    }

    [Theory]
    [MemberData(nameof(GetItemProperties_DataSourcesAndListAccessors_TestData))]
    public void PropertyManager_GetItemProperties_InvokeWithDataSourcesAndListAccessors_ReturnsExpected(PropertyManager manager, ArrayList dataSources, ArrayList listAccessors, string[] expected)
    {
        IEnumerable<PropertyDescriptor> properties = manager.GetItemProperties(dataSources, listAccessors)?.Cast<PropertyDescriptor>();
        Assert.Equal(expected, properties?.Select(p => p.Name));
    }*/

    [Fact]
    public void PropertyManager_CancelCurrentEdit_NullCurrent_Nop()
    {
        PropertyManager manager = new();
        manager.CancelCurrentEdit();
        manager.CancelCurrentEdit();
    }

    [Fact]
    public void PropertyManager_CancelCurrentEdit_IEditableObjectCurrent_CallsCancelEdit()
    {
        Mock<IEditableObject> mockDataSource = new(MockBehavior.Strict);
        mockDataSource
            .Setup(o => o.CancelEdit())
            .Verifiable();

        PropertyManager manager = new(mockDataSource.Object);
        manager.CancelCurrentEdit();
        mockDataSource.Verify(o => o.CancelEdit(), Times.Exactly(1));

        manager.CancelCurrentEdit();
        mockDataSource.Verify(o => o.CancelEdit(), Times.Exactly(2));
    }

    [Fact]
    public void PropertyManager_CancelCurrentEdit_NonNullCurrent_Nop()
    {
        PropertyManager manager = new(new DataSource());
        manager.CancelCurrentEdit();
        manager.CancelCurrentEdit();
    }

    [Fact]
    public void PropertyManager_EndCurrentEdit_NullCurrent_Nop()
    {
        PropertyManager manager = new();
        manager.EndCurrentEdit();
        manager.EndCurrentEdit();
    }

    [Fact]
    public void PropertyManager_EndCurrentEdit_IEditableObjectCurrent_CallsEndEdit()
    {
        Mock<IEditableObject> mockDataSource = new(MockBehavior.Strict);
        mockDataSource
            .Setup(o => o.EndEdit())
            .Verifiable();

        PropertyManager manager = new(mockDataSource.Object);
        manager.EndCurrentEdit();
        mockDataSource.Verify(o => o.EndEdit(), Times.Exactly(1));

        manager.EndCurrentEdit();
        mockDataSource.Verify(o => o.EndEdit(), Times.Exactly(2));
    }

    [WinFormsTheory(Skip = "Flaky test, see: https://github.com/dotnet/winforms/issues/1030")]
    [InlineData(true, 0)]
    [InlineData(false, 1)]
    public void PropertyManager_EndCurrentEdit_IEditableObjectCurrentNotSuccess_DoesNotCallEndEdit(bool cancel, int expectedCallCount)
    {
        int callCount = 0;
        EditableDataSource dataSource = new()
        {
            EndEditHandler = () =>
            {
                callCount++;
            }
        };

        PropertyManager manager = new(dataSource);
        using SubControl control = new() { Visible = true };
        control.CreateControl();
        ControlBindingsCollection controlBindings = new(control);
        Binding cancelBinding = new("Value", dataSource, "Property", true);
        BindingCompleteEventHandler bindingCompleteHandler = (sender, e) =>
        {
            e.Cancel = cancel;
        };

        cancelBinding.BindingComplete += bindingCompleteHandler;
        controlBindings.Add(cancelBinding);
        manager.Bindings.Add(cancelBinding);
        manager.EndCurrentEdit();
        Assert.Equal(expectedCallCount, callCount);

        manager.EndCurrentEdit();
        Assert.Equal(expectedCallCount * 2, callCount);
    }

    [Fact]
    public void PropertyManager_EndCurrentEdit_NonNullCurrent_Nop()
    {
        PropertyManager manager = new(new DataSource());
        manager.EndCurrentEdit();
        manager.EndCurrentEdit();
    }

    [Fact]
    public void PropertyManager_ResumeBinding_SuspendBinding_Success()
    {
        PropertyManager manager = new(new DataSource());
        manager.ResumeBinding();
        manager.SuspendBinding();
        manager.ResumeBinding();
        manager.SuspendBinding();
    }

    [Fact]
    public void PropertyManager_ResumeBinding_Invoke_CallsHandlers()
    {
        PropertyManager manager = new(new DataSource());

        // No handlers.
        manager.ResumeBinding();

        // Only current changed handler.
        int currentChangedCallCount = 0;
        EventHandler currentChangedHandler = (sender, e) =>
        {
            currentChangedCallCount++;
            Assert.Same(manager, sender);
            Assert.Same(EventArgs.Empty, e);
        };
        manager.CurrentChanged += currentChangedHandler;
        manager.ResumeBinding();
        Assert.Equal(1, currentChangedCallCount);

        // Current changed and current item changed handler.
        int currentItemChangedCallCount = 0;
        EventHandler currentItemChangedHandler = (sender, e) =>
        {
            currentItemChangedCallCount++;
            Assert.Same(manager, sender);
            Assert.Same(EventArgs.Empty, e);
        };
        manager.CurrentItemChanged += currentItemChangedHandler;
        manager.ResumeBinding();
        Assert.Equal(2, currentChangedCallCount);
        Assert.Equal(1, currentItemChangedCallCount);

        // Only current item changed handler.
        manager.CurrentChanged -= currentChangedHandler;
        manager.ResumeBinding();
        Assert.Equal(2, currentChangedCallCount);
        Assert.Equal(2, currentItemChangedCallCount);

        // No handlers.
        manager.CurrentItemChanged -= currentItemChangedHandler;
        manager.ResumeBinding();
        Assert.Equal(2, currentChangedCallCount);
        Assert.Equal(2, currentItemChangedCallCount);
    }

    [Fact]
    public void PropertyManager_SuspendBinding_Invoke_CallsHandlers()
    {
        Mock<IEditableObject> mockDataSource = new(MockBehavior.Strict);
        mockDataSource
            .Setup(o => o.EndEdit())
            .Verifiable();

        PropertyManager manager = new(mockDataSource.Object);

        // No handlers.
        manager.SuspendBinding();
        mockDataSource.Verify(o => o.EndEdit(), Times.Exactly(1));

        // Only current changed handler.
        int currentChangedCallCount = 0;
        EventHandler currentChangedHandler = (sender, e) =>
        {
            currentChangedCallCount++;
            Assert.Same(manager, sender);
            Assert.Same(EventArgs.Empty, e);
        };
        manager.CurrentChanged += currentChangedHandler;
        manager.ResumeBinding();
        Assert.Equal(1, currentChangedCallCount);

        // Current changed and current item changed handler.
        int currentItemChangedCallCount = 0;
        EventHandler currentItemChangedHandler = (sender, e) =>
        {
            currentItemChangedCallCount++;
            Assert.Same(manager, sender);
            Assert.Same(EventArgs.Empty, e);
        };
        manager.CurrentItemChanged += currentItemChangedHandler;
        manager.ResumeBinding();
        Assert.Equal(2, currentChangedCallCount);
        Assert.Equal(1, currentItemChangedCallCount);

        // Only current item changed handler.
        manager.CurrentChanged -= currentChangedHandler;
        manager.ResumeBinding();
        Assert.Equal(2, currentChangedCallCount);
        Assert.Equal(2, currentItemChangedCallCount);

        // No handlers.
        manager.CurrentItemChanged -= currentItemChangedHandler;
        manager.ResumeBinding();
        Assert.Equal(2, currentChangedCallCount);
        Assert.Equal(2, currentItemChangedCallCount);
    }

    [Fact]
    public void PropertyManager_OnCurrentChanged_Invoke_CallsHandlers()
    {
        PropertyManager manager = new(new DataSource());

        // No handlers.
        manager.OnCurrentChanged(null);

        // Only current changed handler.
        int currentChangedCallCount = 0;
        EventHandler currentChangedHandler = (sender, e) =>
        {
            currentChangedCallCount++;
            Assert.Same(manager, sender);
            Assert.Null(e);
        };
        manager.CurrentChanged += currentChangedHandler;
        manager.OnCurrentChanged(null);
        Assert.Equal(1, currentChangedCallCount);

        // Current changed and current item changed handler.
        int currentItemChangedCallCount = 0;
        EventHandler currentItemChangedHandler = (sender, e) =>
        {
            currentItemChangedCallCount++;
            Assert.Same(manager, sender);
            Assert.Null(e);
        };
        manager.CurrentItemChanged += currentItemChangedHandler;
        manager.OnCurrentChanged(null);
        Assert.Equal(2, currentChangedCallCount);
        Assert.Equal(1, currentItemChangedCallCount);

        // Only current item changed handler.
        manager.CurrentChanged -= currentChangedHandler;
        manager.OnCurrentChanged(null);
        Assert.Equal(2, currentChangedCallCount);
        Assert.Equal(2, currentItemChangedCallCount);

        // No handlers.
        manager.CurrentItemChanged -= currentItemChangedHandler;
        manager.OnCurrentChanged(null);
        Assert.Equal(2, currentChangedCallCount);
        Assert.Equal(2, currentItemChangedCallCount);
    }

    [Fact]
    public void PropertyManager_OnCurrentItemChanged_Invoke_CallsHandlers()
    {
        PropertyManager manager = new(new DataSource());

        // No handlers.
        manager.OnCurrentChanged(null);

        // Only current changed handler.
        int currentChangedCallCount = 0;
        EventHandler currentChangedHandler = (sender, e) =>
        {
            currentChangedCallCount++;
            Assert.Same(manager, sender);
            Assert.Null(e);
        };
        manager.CurrentChanged += currentChangedHandler;
        manager.OnCurrentItemChanged(null);
        Assert.Equal(0, currentChangedCallCount);

        // Current changed and current item changed handler.
        int currentItemChangedCallCount = 0;
        EventHandler currentItemChangedHandler = (sender, e) =>
        {
            currentItemChangedCallCount++;
            Assert.Same(manager, sender);
            Assert.Null(e);
        };
        manager.CurrentItemChanged += currentItemChangedHandler;
        manager.OnCurrentItemChanged(null);
        Assert.Equal(0, currentChangedCallCount);
        Assert.Equal(1, currentItemChangedCallCount);

        // Only current item changed handler.
        manager.CurrentChanged -= currentChangedHandler;
        manager.OnCurrentItemChanged(null);
        Assert.Equal(0, currentChangedCallCount);
        Assert.Equal(2, currentItemChangedCallCount);

        // No handlers.
        manager.CurrentItemChanged -= currentItemChangedHandler;
        manager.OnCurrentItemChanged(null);
        Assert.Equal(0, currentChangedCallCount);
        Assert.Equal(2, currentItemChangedCallCount);
    }

    [Fact]
    public void PropertyManager_AddNew_Invoke_ThrowsNotSupportedException()
    {
        BindingContext context = [];
        BindingSource source = [];
        PropertyManager manager = Assert.IsType<PropertyManager>(context[new DataSource()]);
        Assert.Throws<NotSupportedException>(manager.AddNew);
    }

    [Theory]
    [IntegerData<int>]
    public void PropertyManager_RemoveAt_Invoke_ThrowsNotSupportedException(int index)
    {
        BindingContext context = [];
        BindingSource source = [];
        PropertyManager manager = Assert.IsType<PropertyManager>(context[new DataSource()]);
        Assert.Throws<NotSupportedException>(() => manager.RemoveAt(index));
    }

    private class ListDataSource
    {
        public List<DataSource> ListProperty { get; set; }
    }

    private class DataSource
    {
        public int Property { get; set; }
    }

    private class SubControl : Control, INotifyPropertyChanged
    {
        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add { }
            remove { }
        }

        public string Value { get; set; }
    }

    private class EditableDataSource : IEditableObject
    {
        public int Property { get; set; }

        public void BeginEdit() => throw new NotImplementedException();

        public void CancelEdit() => throw new NotImplementedException();

        public Action EndEditHandler { get; set; }

        public void EndEdit() => EndEditHandler();
    }
}
