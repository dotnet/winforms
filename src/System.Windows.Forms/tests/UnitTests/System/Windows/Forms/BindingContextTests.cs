// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel;
using System.Reflection;
using Moq;

namespace System.Windows.Forms.Tests;

// NB: doesn't require thread affinity
public class BindingContextTests
{
    [Fact]
    public void BindingContext_Ctor_Default()
    {
        BindingContext context = [];
        Assert.False(context.IsReadOnly);
        Assert.Empty(context);
    }

    [Fact]
    public void BindingContext_ICollection_GetProperties_ReturnsExpected()
    {
        ICollection context = new BindingContext();
        Assert.False(context.IsSynchronized);
        Assert.Same(context, context.SyncRoot);
        Assert.Empty(context);
        Assert.Empty(context);
    }

    [Fact]
    public void BindingContext_Count_GetWithNullWeakReferenceTarget_ScrubsWeakRefs()
    {
        BindingContext context = [];
        DataSource dataSource1 = new() { Property = 1 };
        DataSource dataSource2 = new() { Property = 2 };
        PropertyManager manager1 = Assert.IsAssignableFrom<PropertyManager>(context[dataSource1, "Property"]);
        PropertyManager manager2 = Assert.IsAssignableFrom<PropertyManager>(context[dataSource2, "Property"]);
        var array = new DictionaryEntry[4];
        ((ICollection)context).CopyTo(array, 0);

        WeakReference reference1 = Assert.IsType<WeakReference>(array.Single(p => ((WeakReference)p.Value).Target == manager1).Value);
        WeakReference reference2 = Assert.IsType<WeakReference>(array.Single(p => ((WeakReference)p.Value).Target == manager2).Value);
        Assert.Same(manager1, reference1.Target);
        Assert.Same(manager2, reference2.Target);

        // Simulate a GC by assigning the weak reference to null.
        reference1.Target = null;
        reference2.Target = null;

        // Verify these weak references have been scrubbed.
        Assert.Equal(2, ((ICollection)context).Count);
    }

    [Fact]
    public void BindingContext_Add_NullDataSource_ThrowsArgumentNullException()
    {
        SubBindingContext context = new();
        using BindingSource source = [];
        Assert.Throws<ArgumentNullException>("dataSource", () => context.Add(null, source.CurrencyManager));
    }

    [Fact]
    public void BindingContext_Add_Invoke_GetReturnsExpected()
    {
        SubBindingContext context = new();
        using BindingSource source1 = [];
        using BindingSource source2 = [];
        DataSource dataSource = new();
        context.Add(dataSource, source1.CurrencyManager);
        Assert.Single(context);

        Assert.Same(source1.CurrencyManager, context[dataSource]);
        Assert.Same(source1.CurrencyManager, context[dataSource, null]);
        Assert.Same(source1.CurrencyManager, context[dataSource, string.Empty]);

        // Set new value.
        context.Add(dataSource, source2.CurrencyManager);
        Assert.Single(context);
        Assert.Same(source2.CurrencyManager, context[dataSource]);
        Assert.Same(source2.CurrencyManager, context[dataSource, null]);
        Assert.Same(source2.CurrencyManager, context[dataSource, string.Empty]);
    }

    [Fact]
    public void BindingContext_Add_InvokeMultiple_Success()
    {
        SubBindingContext context = new();
        using BindingSource source1 = [];
        using BindingSource source2 = [];
        DataSource dataSource1 = new();
        DataSource dataSource2 = new();
        context.Add(dataSource1, source1.CurrencyManager);
        context.Add(dataSource2, source2.CurrencyManager);
        Assert.Equal(2, ((ICollection)context).Count);

        Assert.Same(source1.CurrencyManager, context[dataSource1]);
        Assert.Same(source2.CurrencyManager, context[dataSource2]);
    }

    [Fact]
    public void BindingContext_Add_NullListManager_ThrowsArgumentNullException()
    {
        SubBindingContext context = new();
        Assert.Throws<ArgumentNullException>("listManager", () => context.Add(1, null));
    }

    [Fact]
    public void BindingContext_AddCore_Invoke_GetReturnsExpected()
    {
        SubBindingContext context = new();
        using BindingSource source1 = [];
        using BindingSource source2 = [];
        DataSource dataSource = new();
        context.AddCore(dataSource, source1.CurrencyManager);
        Assert.Single(context);

        Assert.Same(source1.CurrencyManager, context[dataSource]);
        Assert.Same(source1.CurrencyManager, context[dataSource, null]);
        Assert.Same(source1.CurrencyManager, context[dataSource, string.Empty]);

        // Set new value.
        context.AddCore(dataSource, source2.CurrencyManager);
        Assert.Single(context);
        Assert.Same(source2.CurrencyManager, context[dataSource]);
        Assert.Same(source2.CurrencyManager, context[dataSource, null]);
        Assert.Same(source2.CurrencyManager, context[dataSource, string.Empty]);
    }

    [Fact]
    public void BindingContext_AddCore_InvokeMultiple_Success()
    {
        SubBindingContext context = new();
        using BindingSource source1 = [];
        using BindingSource source2 = [];
        DataSource dataSource1 = new();
        DataSource dataSource2 = new();
        context.AddCore(dataSource1, source1.CurrencyManager);
        context.AddCore(dataSource2, source2.CurrencyManager);
        Assert.Equal(2, ((ICollection)context).Count);

        Assert.Same(source1.CurrencyManager, context[dataSource1]);
        Assert.Same(source2.CurrencyManager, context[dataSource2]);
    }

    [Fact]
    public void BindingContext_AddCore_NullDataSource_ThrowsArgumentNullException()
    {
        SubBindingContext context = new();
        using BindingSource source = [];
        Assert.Throws<ArgumentNullException>("dataSource", () => context.AddCore(null, source.CurrencyManager));
    }

    [Fact]
    public void BindingContext_AddCore_NullListManager_ThrowsArgumentNullException()
    {
        SubBindingContext context = new();
        Assert.Throws<ArgumentNullException>("listManager", () => context.AddCore(1, null));
    }

    [Fact]
    public void BindingContext_CopyTo_Invoke_Success()
    {
        SubBindingContext context = new();
        using BindingSource source = [];
        DataSource dataSource = new();
        context.Add(dataSource, source.CurrencyManager);

        object[] array = [1, 2, 3];
        ((ICollection)context).CopyTo(array, 1);
        Assert.Equal(1, array[0]);
        Assert.NotNull(Assert.IsType<DictionaryEntry>(array[1]).Key);
        Assert.Equal(source.CurrencyManager, Assert.IsType<WeakReference>(Assert.IsType<DictionaryEntry>(array[1]).Value).Target);
        Assert.Equal(3, array[2]);
    }

    [Fact]
    public void BindingContext_CopyTo_WithNullWeakReferenceTarget_ScrubsWeakRefs()
    {
        BindingContext context = [];
        DataSource dataSource1 = new() { Property = 1 };
        DataSource dataSource2 = new() { Property = 2 };
        PropertyManager manager1 = Assert.IsAssignableFrom<PropertyManager>(context[dataSource1, "Property"]);
        PropertyManager manager2 = Assert.IsAssignableFrom<PropertyManager>(context[dataSource2, "Property"]);
        var array = new DictionaryEntry[4];
        ((ICollection)context).CopyTo(array, 0);

        WeakReference reference1 = Assert.IsType<WeakReference>(array.Single(p => ((WeakReference)p.Value).Target == manager1).Value);
        WeakReference reference2 = Assert.IsType<WeakReference>(array.Single(p => ((WeakReference)p.Value).Target == manager2).Value);
        Assert.Same(manager1, reference1.Target);
        Assert.Same(manager2, reference2.Target);

        // Simulate a GC by assigning the weak reference to null.
        reference1.Target = null;
        reference2.Target = null;

        // Verify these weak references have been scrubbed.
        object[] destArray = [1, 2, 3, 4];
        ((ICollection)context).CopyTo(destArray, 1);
        Assert.Equal(1, destArray[0]);
        Assert.IsType<DictionaryEntry>(destArray[1]);
        Assert.IsType<DictionaryEntry>(destArray[2]);
        Assert.Equal(4, destArray[3]);
    }

    [Fact]
    public void BindingContext_GetEnumerator_WithNullWeakReferenceTarget_ScrubsWeakRefs()
    {
        BindingContext context = [];
        DataSource dataSource1 = new() { Property = 1 };
        DataSource dataSource2 = new() { Property = 2 };
        PropertyManager manager1 = Assert.IsAssignableFrom<PropertyManager>(context[dataSource1, "Property"]);
        PropertyManager manager2 = Assert.IsAssignableFrom<PropertyManager>(context[dataSource2, "Property"]);
        var array = new DictionaryEntry[4];
        ((ICollection)context).CopyTo(array, 0);

        WeakReference reference1 = Assert.IsType<WeakReference>(array.Single(p => ((WeakReference)p.Value).Target == manager1).Value);
        WeakReference reference2 = Assert.IsType<WeakReference>(array.Single(p => ((WeakReference)p.Value).Target == manager2).Value);
        Assert.Same(manager1, reference1.Target);
        Assert.Same(manager2, reference2.Target);

        // Simulate a GC by assigning the weak reference to null.
        reference1.Target = null;
        reference2.Target = null;

        // Verify these weak references have been scrubbed.
        IEnumerator enumerator = ((ICollection)context).GetEnumerator();
        Assert.True(enumerator.MoveNext());
        Assert.IsType<DictionaryEntry>(enumerator.Current);
        Assert.True(enumerator.MoveNext());
        Assert.IsType<DictionaryEntry>(enumerator.Current);
        Assert.False(enumerator.MoveNext());
    }

    [Fact]
    public void BindingContext_Remove_Invoke_Success()
    {
        SubBindingContext context = new();
        using BindingSource source1 = [];
        DataSource dataSource1 = new();
        using BindingSource source2 = [];
        DataSource dataSource2 = new();
        context.Add(dataSource1, source1.CurrencyManager);
        context.Add(dataSource2, source2.CurrencyManager);

        context.Remove(dataSource1);
        Assert.Single(context);

        // Remove again.
        context.Remove(dataSource1);
        Assert.Single(context);

        context.Remove(dataSource2);
        Assert.Empty(context);

        // Remove again.
        context.Remove(dataSource2);
        Assert.Empty(context);
    }

    [Fact]
    public void BindingContext_Remove_NullDataSource_ThrowsArgumentNullException()
    {
        SubBindingContext context = new();
        Assert.Throws<ArgumentNullException>("dataSource", () => context.Remove(null));
    }

    [Fact]
    public void BindingContext_RemoveCore_Invoke_Success()
    {
        SubBindingContext context = new();
        using BindingSource source1 = [];
        DataSource dataSource1 = new();
        using BindingSource source2 = [];
        DataSource dataSource2 = new();
        context.Add(dataSource1, source1.CurrencyManager);
        context.Add(dataSource2, source2.CurrencyManager);

        context.RemoveCore(dataSource1);
        Assert.Single(context);

        // Remove again.
        context.RemoveCore(dataSource1);
        Assert.Single(context);

        context.RemoveCore(dataSource2);
        Assert.Empty(context);

        // Remove again.
        context.RemoveCore(dataSource2);
        Assert.Empty(context);
    }

    [Fact]
    public void BindingContext_RemoveCore_NullDataSource_ThrowsArgumentNullException()
    {
        SubBindingContext context = new();
        Assert.Throws<ArgumentNullException>("dataSource", () => context.RemoveCore(null));
    }

    [Fact]
    public void BindingContext_Clear_Empty_Success()
    {
        SubBindingContext context = new();
        context.Clear();
        Assert.Empty(context);

        // Clear again.
        context.Clear();
        Assert.Empty(context);
    }

    [Fact]
    public void BindingContext_Clear_NotEmpty_Success()
    {
        SubBindingContext context = new();
        using BindingSource source = [];
        context.Add(new DataSource(), source.CurrencyManager);

        // Clear again.
        context.Clear();
        Assert.Empty(context);

        context.Clear();
        Assert.Empty(context);
    }

    [Fact]
    public void BindingContext_ClearCore_Empty_Success()
    {
        SubBindingContext context = new();
        context.ClearCore();
        Assert.Empty(context);

        // Clear again.
        context.ClearCore();
        Assert.Empty(context);
    }

    [Fact]
    public void BindingContext_ClearCore_NotEmpty_Success()
    {
        SubBindingContext context = new();
        using BindingSource source = [];
        context.Add(new DataSource(), source.CurrencyManager);

        // Clear again.
        context.ClearCore();
        Assert.Empty(context);

        context.ClearCore();
        Assert.Empty(context);
    }

    [Fact]
    public void BindingContext_Contains_DataSource_ReturnsExpected()
    {
        SubBindingContext context = new();
        using BindingSource source = [];
        DataSource dataSource = new();
        context.Add(dataSource, source.CurrencyManager);
        Assert.True(context.Contains(dataSource));
        Assert.True(context.Contains(dataSource, null));
        Assert.True(context.Contains(dataSource, string.Empty));
        Assert.False(context.Contains(1));
        Assert.False(context.Contains(1, null));
        Assert.False(context.Contains(1, string.Empty));
    }

    [Fact]
    public void BindingContext_Contains_DataSourceDataMember_ReturnsExpected()
    {
        SubBindingContext context = new();
        using BindingSource source = [];
        DataSource dataSource1 = new();
        DataSource dataSource2 = new();
        context.Add(dataSource1, source.CurrencyManager);
        Assert.NotNull(context[dataSource2, "Property"]);

        Assert.True(context.Contains(dataSource1, null));
        Assert.True(context.Contains(dataSource1, string.Empty));
        Assert.False(context.Contains(dataSource1, "Property"));
        Assert.True(context.Contains(dataSource2, null));
        Assert.True(context.Contains(dataSource2, string.Empty));
        Assert.True(context.Contains(dataSource2, "Property"));
        Assert.True(context.Contains(dataSource2, "property"));
        Assert.False(context.Contains(dataSource2, "NoSuchProperty"));
        Assert.False(context.Contains(1, "Property"));
    }

    [Fact]
    public void BindingContext_Contains_NullDataSource_ThrowsArgumentNullException()
    {
        BindingContext context = [];
        Assert.Throws<ArgumentNullException>("dataSource", () => context.Contains(null));
        Assert.Throws<ArgumentNullException>("dataSource", () => context.Contains(null, null));
        Assert.Throws<ArgumentNullException>("dataSource", () => context.Contains(null, string.Empty));
    }

    [Fact]
    public void BindingContext_Item_GetNoSuchDataSource_AddsToCollection()
    {
        BindingContext context = [];
        DataSource dataSource = new();
        PropertyManager manager = Assert.IsType<PropertyManager>(context[dataSource]);
        Assert.Same(dataSource, manager.Current);
        Assert.Equal(1, manager.Count);
        Assert.Equal(0, manager.Position);
        Assert.Single(context);
        Assert.Same(manager, context[dataSource]);
    }

    [Fact]
    public void BindingContext_Item_GetIListDataSource_AddsToCollection()
    {
        BindingContext context = [];
        List<int> dataSource = [1, 2, 3];
        CurrencyManager manager = Assert.IsType<CurrencyManager>(context[dataSource]);
        Assert.Same(dataSource, manager.List);
        Assert.Equal(1, manager.Current);
        Assert.Equal(3, manager.Count);
        Assert.Equal(0, manager.Position);
        Assert.Single(context);
        Assert.Same(manager, context[dataSource]);
    }

    [Fact]
    public void BindingContext_Item_GetArrayDataSource_AddsToCollection()
    {
        BindingContext context = [];
        int[] dataSource = [1, 2, 3];
        CurrencyManager manager = Assert.IsType<CurrencyManager>(context[dataSource]);
        Assert.Same(dataSource, manager.List);
        Assert.Equal(1, manager.Current);
        Assert.Equal(3, manager.Count);
        Assert.Equal(0, manager.Position);
        Assert.Single(context);
        Assert.Same(manager, context[dataSource]);
    }

    [Fact]
    public void BindingContext_Item_GetIListSourceDataSource_AddsToCollection()
    {
        BindingContext context = [];
        List<int> dataSource = [1, 2, 3];
        Mock<IListSource> mockIListSource = new(MockBehavior.Strict);
        mockIListSource
            .Setup(s => s.GetList())
            .Returns(dataSource);

        CurrencyManager manager = Assert.IsType<CurrencyManager>(context[mockIListSource.Object]);
        Assert.Same(dataSource, manager.List);
        Assert.Equal(1, manager.Current);
        Assert.Equal(3, manager.Count);
        Assert.Equal(0, manager.Position);
        Assert.Single(context);
        Assert.Same(manager, context[mockIListSource.Object]);
    }

    [Fact]
    public void BindingContext_Item_GetIListSourceDataSourceReturningNull_ThrowsArgumentNullException()
    {
        BindingContext context = [];
        Mock<IListSource> mockIListSource = new(MockBehavior.Strict);
        mockIListSource
            .Setup(s => s.GetList())
            .Returns((IList)null);

        Assert.Throws<ArgumentNullException>("dataSource", () => context[mockIListSource.Object]);
        Assert.Empty(context);
    }

    [Fact]
    public void BindingContext_Item_GetWithICurrencyManagerProvider_DoesNotAddToCollection()
    {
        BindingContext context = [];
        CurrencyManager manager = new BindingSource().CurrencyManager;
        Mock<ICurrencyManagerProvider> mockCurrencyManagerProvider = new();
        mockCurrencyManagerProvider
            .Setup(p => p.GetRelatedCurrencyManager("dataMember"))
            .Returns(manager);

        Assert.Same(manager, context[mockCurrencyManagerProvider.Object, "dataMember"]);
        Assert.Empty(context);
    }

    [Fact]
    public void BindingContext_Item_GetWithNullICurrencyManagerProvider_AddsToCollection()
    {
        BindingContext context = [];
        Mock<ICurrencyManagerProvider> mockCurrencyManagerProvider = new();
        mockCurrencyManagerProvider
            .Setup(p => p.GetRelatedCurrencyManager("dataMember"))
            .Returns((CurrencyManager)null);

        PropertyManager manager = Assert.IsType<PropertyManager>(context[mockCurrencyManagerProvider.Object]);
        Assert.Single(context);
        Assert.Same(manager, context[mockCurrencyManagerProvider.Object]);
    }

    public static IEnumerable<object[]> Item_DataSourceWithDataMember_TestData()
    {
        ParentDataSource dataSource = new()
        {
            ParentProperty = new DataSource
            {
                Property = 1
            }
        };
        yield return new object[] { dataSource, "ParentProperty", dataSource.ParentProperty, 2 };
        yield return new object[] { dataSource, "ParentProperty.Property", dataSource.ParentProperty.Property, 3 };
        yield return new object[] { dataSource, "parentproperty", dataSource.ParentProperty, 2 };
        yield return new object[] { dataSource, "parentproperty.property", dataSource.ParentProperty.Property, 3 };
    }

    [Theory]
    [MemberData(nameof(Item_DataSourceWithDataMember_TestData))]
    public void BindingContext_Item_GetNoSuchDataSourceWithDataMember_AddsToCollection(object dataSource, string dataMember, object expectedCurrent, int expectedCount)
    {
        BindingContext context = [];
        PropertyManager manager = Assert.IsAssignableFrom<PropertyManager>(context[dataSource, dataMember]);
        Assert.Equal(expectedCurrent, manager.Current);
        Assert.Equal(1, manager.Count);
        Assert.Equal(0, manager.Position);
        Assert.Equal(expectedCount, ((ICollection)context).Count);
        Assert.Same(manager, context[dataSource, dataMember]);
        Assert.Same(manager, context[dataSource, dataMember.ToLowerInvariant()]);
    }

    [Fact]
    public void BindingContext_Item_GetWithAddedDataSourceWithDataMember_ThrowsArgumentException()
    {
        SubBindingContext context = new();
        using BindingSource source = [];
        DataSource dataSource = new();
        context.Add(dataSource, source.CurrencyManager);
        Assert.Throws<ArgumentException>(() => context[dataSource, "Property"]);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void BindingContext_Item_GetNoSuchDataSourceNullOrEmptyMember_AddsToCollection(string dataMember)
    {
        SubBindingContext context = new();
        DataSource dataSource = new();
        PropertyManager manager = Assert.IsType<PropertyManager>(context[dataSource, dataMember]);
        Assert.Single(context);
        Assert.Same(manager, context[dataSource]);
        Assert.Same(manager, context[dataSource, string.Empty]);
        Assert.Same(manager, context[dataSource]);
        Assert.Same(manager, context[dataSource, null]);
    }

    [Theory]
    [InlineData("NoSuchProperty")]
    [InlineData(".")]
    [InlineData("..")]
    [InlineData("ParentProperty.")]
    [InlineData("ParentProperty.NoSuchProperty")]
    public void BindingContext_Item_GetNoSuchDataSourceNoSuchDataMember_ThrowsArgumentException(string dataMember)
    {
        SubBindingContext context = new();
        ParentDataSource dataSource = new();
        Assert.Throws<ArgumentException>(() => context[dataSource, dataMember]);
    }

    [Fact]
    public void BindingContext_Item_GetIListWithDataMemberReturningIList_AddsToCollection()
    {
        BindingContext context = [];
        List<int> list = [1, 2, 3];
        IListDataSource dataSource = new()
        {
            Property = list
        };

        CurrencyManager manager = Assert.IsAssignableFrom<CurrencyManager>(context[dataSource, "Property"]);
        Assert.Same(list, manager.List);
        Assert.Equal(1, manager.Current);
        Assert.Equal(3, manager.Count);
        Assert.Equal(0, manager.Position);
        Assert.Equal(2, ((ICollection)context).Count);
        Assert.Same(manager, context[dataSource, "Property"]);
    }

    [Fact]
    public void BindingContext_Item_GetIListWithDataMemberReturningNonIList_AddsToCollection()
    {
        BindingContext context = [];
        List<int> list = [1, 2, 3];
        ObjectDataSource dataSource = new()
        {
            Property = list
        };

        PropertyManager manager = Assert.IsAssignableFrom<PropertyManager>(context[dataSource, "Property"]);
        Assert.Same(list, manager.Current);
        Assert.Equal(1, manager.Count);
        Assert.Equal(0, manager.Position);
        Assert.Equal(2, ((ICollection)context).Count);
        Assert.Same(manager, context[dataSource, "Property"]);
    }

    [Fact]
    public void BindingContext_Item_GetArrayWithDataMember_AddsToCollection()
    {
        BindingContext context = [];
        int[] list = [1, 2, 3];
        IListDataSource dataSource = new()
        {
            Property = list
        };

        CurrencyManager manager = Assert.IsAssignableFrom<CurrencyManager>(context[dataSource, "Property"]);
        Assert.Same(list, manager.List);
        Assert.Equal(1, manager.Current);
        Assert.Equal(3, manager.Count);
        Assert.Equal(0, manager.Position);
        Assert.Equal(2, ((ICollection)context).Count);
        Assert.Same(manager, context[dataSource, "Property"]);
    }

    [Fact]
    public void BindingContext_Item_GetIListSourceDataSourceWithDataMemberReturningIList_AddsToCollection()
    {
        BindingContext context = [];
        List<int> list = [1, 2, 3];
        IListDataSource dataSource = new();
        Mock<IListSource> mockIListSource = new(MockBehavior.Strict);
        mockIListSource
            .Setup(s => s.GetList())
            .Returns(list);
        Mock<IList> mockIList = mockIListSource.As<IList>();
        dataSource.Property = mockIList.Object;

        CurrencyManager manager = Assert.IsAssignableFrom<CurrencyManager>(context[dataSource, "Property"]);
        Assert.Same(list, manager.List);
        Assert.Equal(1, manager.Current);
        Assert.Equal(3, manager.Count);
        Assert.Equal(0, manager.Position);
        Assert.Equal(2, ((ICollection)context).Count);
        Assert.Same(manager, context[dataSource, "Property"]);
    }

    [Fact]
    public void BindingContext_Item_GetIListSourceDataSourceWithDataMemberReturningNonIList_AddsToCollection()
    {
        BindingContext context = [];
        List<int> list = [1, 2, 3];
        IListSourceDataSource dataSource = new();
        Mock<IListSource> mockIListSource = new(MockBehavior.Strict);
        mockIListSource
            .Setup(s => s.GetList())
            .Returns(list);
        dataSource.Property = mockIListSource.Object;

        PropertyManager manager = Assert.IsAssignableFrom<PropertyManager>(context[dataSource, "Property"]);
        Assert.Same(mockIListSource.Object, manager.Current);
        Assert.Equal(1, manager.Count);
        Assert.Equal(0, manager.Position);
        Assert.Equal(2, ((ICollection)context).Count);
        Assert.Same(manager, context[dataSource, "Property"]);
    }

    [ActiveIssue("https://github.com/dotnet/winforms/issues/11795")]
    [Fact]
    [SkipOnArchitecture(TestArchitectures.X86,
        "Flaky tests, see: https://github.com/dotnet/winforms/issues/11795")]
    public void BindingContext_Item_GetIListSourceDataSourceWithDataMemberReturningIListNull_ThrowsArgumentNullException()
    {
        BindingContext context = [];
        IListDataSource dataSource = new();
        Mock<IListSource> mockIListSource = new(MockBehavior.Strict);
        mockIListSource
            .Setup(s => s.GetList())
            .Returns((IList)null);
        Mock<IList> mockIList = mockIListSource.As<IList>();
        dataSource.Property = mockIList.Object;

        Assert.Throws<ArgumentNullException>("dataSource", () => context[dataSource, "Property"]);

        // Does, however, add the parent.
        PropertyManager parentManager = Assert.IsType<PropertyManager>(Assert.IsType<WeakReference>(Assert.IsType<DictionaryEntry>(Assert.Single(context)).Value).Target);
        Assert.Same(dataSource, parentManager.Current);
    }

    [Fact]
    public void BindingContext_Item_GetIListSourceDataSourceWithDataMemberReturningNonIListNull_AddsToCollection()
    {
        BindingContext context = [];
        IListSourceDataSource dataSource = new();
        Mock<IListSource> mockIListSource = new(MockBehavior.Strict);
        mockIListSource
            .Setup(s => s.GetList())
            .Returns((IList)null);
        dataSource.Property = mockIListSource.Object;

        PropertyManager manager = Assert.IsAssignableFrom<PropertyManager>(context[dataSource, "Property"]);
        Assert.Same(mockIListSource.Object, manager.Current);
        Assert.Equal(1, manager.Count);
        Assert.Equal(0, manager.Position);
        Assert.Equal(2, ((ICollection)context).Count);
        Assert.Same(manager, context[dataSource, "Property"]);
    }

    [Fact]
    public void BindingContext_Item_GetWithNullWeakReferenceTarget_Success()
    {
        BindingContext context = [];
        DataSource dataSource = new();
        PropertyManager manager = Assert.IsType<PropertyManager>(context[dataSource]);
        WeakReference reference = Assert.IsType<WeakReference>(Assert.IsType<DictionaryEntry>(Assert.Single(context)).Value);
        Assert.Same(manager, reference.Target);

        // Simulate a GC by assigning the weak reference to null.
        reference.Target = null;

        // Now get the new manager and verify it.
        PropertyManager newManager = Assert.IsType<PropertyManager>(context[dataSource]);
        Assert.NotSame(manager, newManager);
        Assert.Same(dataSource, newManager.Current);
        Assert.Equal(1, newManager.Count);
        Assert.Equal(0, newManager.Position);
        Assert.Single(context);
        Assert.Same(newManager, context[dataSource]);
    }

    [Fact]
    public void BindingContext_Item_GetNullDataSource_ThrowsArgumentNullException()
    {
        SubBindingContext context = new();
        Assert.Throws<ArgumentNullException>("dataSource", () => context[null]);
        Assert.Throws<ArgumentNullException>("dataSource", () => context[null, null]);
        Assert.Throws<ArgumentNullException>("dataSource", () => context[null, string.Empty]);
    }

    [Fact]
    public void BindingContext_CollectionChanged_Add_ThrowsNotImplementedException()
    {
        BindingContext context = [];
        CollectionChangeEventHandler handler = (sender, e) => { };
        Assert.Throws<NotImplementedException>(() => context.CollectionChanged += handler);
    }

    [Fact]
    public void BindingContext_CollectionChanged_Remove_Nop()
    {
        BindingContext context = [];
        CollectionChangeEventHandler handler = (sender, e) => { };
        context.CollectionChanged -= handler;
    }

    [Fact]
    public void BindingContext_OnCollectionChanged_Invoke_Nop()
    {
        SubBindingContext context = new();
        context.OnCollectionChanged(null);
    }

    [Fact]
    public void BindingContext_KeyEquals_Invoke_ReturnsExpected()
    {
        SubBindingContext context1 = new();
        SubBindingContext context2 = new();
        SubBindingContext context3 = new();
        SubBindingContext context4 = new();
        using BindingSource source1 = [];
        using BindingSource source2 = [];
        DataSource dataSource1 = new();
        DataSource dataSource2 = new();

        context1.Add(dataSource1, source1.CurrencyManager);
        context2.Add(dataSource1, source1.CurrencyManager);
        context3.Add(dataSource2, source1.CurrencyManager);
        context4.Add(dataSource2, source2.CurrencyManager);

        DictionaryEntry entry1 = Assert.IsType<DictionaryEntry>(Assert.Single(context1));
        DictionaryEntry entry2 = Assert.IsType<DictionaryEntry>(Assert.Single(context2));
        DictionaryEntry entry3 = Assert.IsType<DictionaryEntry>(Assert.Single(context3));
        DictionaryEntry entry4 = Assert.IsType<DictionaryEntry>(Assert.Single(context3));

        Assert.True(entry1.Key.Equals(entry1.Key));
        Assert.True(entry1.Key.Equals(entry2.Key));
        Assert.False(entry1.Key.Equals(entry3.Key));
        Assert.False(entry1.Key.Equals(entry4.Key));

        Assert.False(entry1.Key.Equals(new object()));
        Assert.False(entry1.Key.Equals(null));
    }

    [Fact]
    public void BindingContext_UpdateBinding_NewBindingWithoutDataMember_Success()
    {
        BindingContext context = [];
        DataSource dataSource = new();
        Binding binding = new(null, dataSource, "dataMember");

        BindingContext.UpdateBinding(context, binding);
        Assert.Single(context);

        PropertyManager manager = Assert.IsType<PropertyManager>(context[dataSource]);
        Assert.Same(binding, Assert.Single(manager.Bindings));
        Assert.Same(dataSource, manager.Current);
        Assert.Equal(1, manager.Count);
        Assert.Equal(0, manager.Position);
        Assert.Same(manager, binding.BindingManagerBase);
    }

    [Fact]
    public void BindingContext_UpdateBinding_NewListBindingWithoutDataMember_Success()
    {
        BindingContext context = [];
        List<int> dataSource = [1, 2, 3];
        Binding binding = new(null, dataSource, "dataMember");

        BindingContext.UpdateBinding(context, binding);
        Assert.Single(context);

        CurrencyManager manager = Assert.IsType<CurrencyManager>(context[dataSource]);
        Assert.Same(binding, Assert.Single(manager.Bindings));
        Assert.Same(dataSource, manager.List);
        Assert.Equal(1, manager.Current);
        Assert.Equal(3, manager.Count);
        Assert.Equal(0, manager.Position);
        Assert.Same(manager, binding.BindingManagerBase);
    }

    [Fact]
    public void BindingContext_UpdateBinding_NewBindingWithDataMember_Success()
    {
        BindingContext context = [];
        DataSource dataSource = new() { Property = 1 };
        Binding binding = new(null, dataSource, "Property.ignored");

        BindingContext.UpdateBinding(context, binding);
        Assert.Equal(2, ((ICollection)context).Count);

        PropertyManager manager = Assert.IsAssignableFrom<PropertyManager>(context[dataSource, "Property"]);
        Assert.Same(binding, Assert.Single(manager.Bindings));
        Assert.Equal(1, manager.Current);
        Assert.Equal(1, manager.Count);
        Assert.Equal(0, manager.Position);
        Assert.Same(manager, binding.BindingManagerBase);
    }

    [Fact]
    public void BindingContext_UpdateBinding_NewListBindingWithDataMember_Success()
    {
        BindingContext context = [];
        List<int> list = [1, 2, 3];
        IListDataSource dataSource = new() { Property = list };
        Binding binding = new(null, dataSource, "Property.ignore");

        BindingContext.UpdateBinding(context, binding);
        Assert.Equal(2, ((ICollection)context).Count);

        CurrencyManager manager = Assert.IsAssignableFrom<CurrencyManager>(context[dataSource, "Property"]);
        Assert.Same(binding, Assert.Single(manager.Bindings));
        Assert.Same(list, manager.List);
        Assert.Equal(1, manager.Current);
        Assert.Equal(3, manager.Count);
        Assert.Equal(0, manager.Position);
        Assert.Same(manager, binding.BindingManagerBase);
    }

    [Fact]
    public void BindingContext_UpdateBinding_UpdateBindingWithoutDataMember_Success()
    {
        BindingContext context1 = [];
        BindingContext context2 = [];
        DataSource dataSource = new();
        Binding binding = new(null, dataSource, "dataMember");

        BindingContext.UpdateBinding(context1, binding);
        Assert.Single(context1);
        Assert.Empty(context2);

        // Update.
        BindingContext.UpdateBinding(context2, binding);
        Assert.Single(context1);
        Assert.Single(context2);

        PropertyManager manager1 = Assert.IsType<PropertyManager>(context1[dataSource]);
        Assert.Empty(manager1.Bindings);
        Assert.Same(dataSource, manager1.Current);
        Assert.Equal(1, manager1.Count);
        Assert.Equal(0, manager1.Position);

        PropertyManager manager2 = Assert.IsType<PropertyManager>(context2[dataSource]);
        Assert.Same(binding, Assert.Single(manager2.Bindings));
        Assert.Same(dataSource, manager2.Current);
        Assert.Equal(1, manager2.Count);
        Assert.Equal(0, manager2.Position);
        Assert.Same(manager2, binding.BindingManagerBase);
    }

    [Fact]
    public void BindingContext_UpdateBinding_UpdateListBindingWithoutDataMember_Success()
    {
        BindingContext context1 = [];
        BindingContext context2 = [];
        List<int> dataSource = [1, 2, 3];
        Binding binding = new(null, dataSource, "dataMember");

        BindingContext.UpdateBinding(context1, binding);
        Assert.Single(context1);
        Assert.Empty(context2);

        // Update.
        BindingContext.UpdateBinding(context2, binding);
        Assert.Single(context1);
        Assert.Single(context2);

        CurrencyManager manager1 = Assert.IsType<CurrencyManager>(context1[dataSource]);
        Assert.Empty(manager1.Bindings);
        Assert.Same(dataSource, manager1.List);
        Assert.Equal(1, manager1.Current);
        Assert.Equal(3, manager1.Count);
        Assert.Equal(0, manager1.Position);

        CurrencyManager manager2 = Assert.IsType<CurrencyManager>(context2[dataSource]);
        Assert.Same(binding, Assert.Single(manager2.Bindings));
        Assert.Same(dataSource, manager2.List);
        Assert.Equal(1, manager2.Current);
        Assert.Equal(3, manager2.Count);
        Assert.Equal(0, manager2.Position);
        Assert.Same(manager2, binding.BindingManagerBase);
    }

    [Fact]
    public void BindingContext_UpdateBinding_NullBindingContext_Success()
    {
        BindingContext context = [];
        DataSource dataSource = new();
        Binding binding = new(null, dataSource, "dataMember");

        // Without binding manager.
        BindingContext.UpdateBinding(null, binding);
        Assert.Null(binding.BindingManagerBase);

        // With binding manager.
        BindingContext.UpdateBinding(context, binding);
        Assert.Single(context);
        Assert.NotNull(binding.BindingManagerBase);

        BindingContext.UpdateBinding(null, binding);
        Assert.Single(context);

        PropertyManager manager = Assert.IsAssignableFrom<PropertyManager>(context[dataSource]);
        Assert.Empty(manager.Bindings);
        Assert.Equal(dataSource, manager.Current);
        Assert.Equal(1, manager.Count);
        Assert.Equal(0, manager.Position);
        Assert.Null(binding.BindingManagerBase);
    }

    [Fact]
    public void BindingContext_InvokeCircularWithoutComponent_ThrowsArgumentException()
    {
        BindingContext context = [];
        DataSource dataSource = new();
        Binding binding = new(null, dataSource, "dataMember");

        BindingContext.UpdateBinding(context, binding);
        BindingManagerBase oldManager = binding.BindingManagerBase;
        int callCount = 0;
        oldManager.Bindings.CollectionChanged += (sender, e) =>
        {
            if (e.Action == CollectionChangeAction.Remove)
            {
                Assert.Null(binding.BindingManagerBase);
                MethodInfo m = oldManager.Bindings.GetType().GetMethod("Add", BindingFlags.NonPublic | BindingFlags.Instance);
                m.Invoke(oldManager.Bindings, [binding]);
                Assert.NotNull(binding.BindingManagerBase);
                callCount++;
            }
        };
        Assert.Throws<ArgumentException>("dataBinding", () => BindingContext.UpdateBinding(context, binding));
        Assert.Equal(1, callCount);
    }

    [Fact]
    public void BindingContext_InvokeCircularWithComponent_ThrowsArgumentException()
    {
        BindingContext context = [];
        DataSource dataSource = new();
        Binding binding = new(null, dataSource, "dataMember");
        Control control = new();
        control.DataBindings.Add(binding);
        Assert.NotNull(binding.BindableComponent);

        BindingContext.UpdateBinding(context, binding);
        BindingManagerBase oldManager = binding.BindingManagerBase;
        int callCount = 0;
        oldManager.Bindings.CollectionChanged += (sender, e) =>
        {
            if (e.Action == CollectionChangeAction.Remove)
            {
                Assert.Null(binding.BindingManagerBase);
                MethodInfo m = oldManager.Bindings.GetType().GetMethod("Add", BindingFlags.NonPublic | BindingFlags.Instance);
                m.Invoke(oldManager.Bindings, [binding]);
                Assert.NotNull(binding.BindingManagerBase);
                callCount++;
            }
        };
        Assert.Throws<ArgumentException>("dataBinding", () => BindingContext.UpdateBinding(context, binding));
        Assert.Equal(1, callCount);
    }

    [Fact]
    public void BindingContext_UpdateBinding_NullBinding_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>("binding", () => BindingContext.UpdateBinding([], null));
    }

    private class ParentDataSource
    {
        public DataSource ParentProperty { get; set; }
    }

    private class DataSource
    {
        public int Property { get; set; }
    }

    private class IListDataSource
    {
        public IList Property { get; set; }
    }

    private class IListSourceDataSource
    {
        public IListSource Property { get; set; }
    }

    private class ObjectDataSource
    {
        public object Property { get; set; }
    }

    private class SubBindingContext : BindingContext
    {
        public new void Add(object dataSource, BindingManagerBase listManager) => base.Add(dataSource, listManager);

        public new void AddCore(object dataSource, BindingManagerBase listManager) => base.AddCore(dataSource, listManager);

        public new void Remove(object dataSource) => base.Remove(dataSource);

        public new void RemoveCore(object dataSource) => base.RemoveCore(dataSource);

        public new void Clear() => base.Clear();

        public new void ClearCore() => base.ClearCore();

        public new void OnCollectionChanged(CollectionChangeEventArgs ccevent) => base.OnCollectionChanged(ccevent);
    }
}
