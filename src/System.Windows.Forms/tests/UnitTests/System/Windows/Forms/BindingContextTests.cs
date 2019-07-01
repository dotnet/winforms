// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Moq;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class BindingContextTests
    {
        [Fact]
        public void BindingContext_Ctor_Default()
        {
            var context = new BindingContext();
            Assert.False(context.IsReadOnly);
            Assert.Empty(context);
        }

        [Fact]
        public void BindingContext_ICollection_GetProperties_ReturnsExpected()
        {
            ICollection context = new BindingContext();
            Assert.False(context.IsSynchronized);
            Assert.Null(context.SyncRoot);
            Assert.Empty(context);
        }

        [Fact]
        public void BindingContext_Count_GetWithNullWeakReferenceTarget_ScrubsWeakRefs()
        {
            var context = new BindingContext();
            var dataSource1 = new DataSource { Property = 1 };
            var dataSource2 = new DataSource { Property = 2 };
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
            var context = new BindingContext();
            var source = new BindingSource();
            Assert.Throws<ArgumentNullException>("dataSource", () => context.Add(null, source.CurrencyManager));
        }

        [Fact]
        public void BindingContext_Add_Invoke_GetReturnsExpected()
        {
            var context = new BindingContext();
            var source1 = new BindingSource();
            var source2 = new BindingSource();
            var dataSource = new DataSource();
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
            var context = new BindingContext();
            var source1 = new BindingSource();
            var source2 = new BindingSource();
            var dataSource1 = new DataSource();
            var dataSource2 = new DataSource();
            context.Add(dataSource1, source1.CurrencyManager);
            context.Add(dataSource2, source2.CurrencyManager);
            Assert.Equal(2, ((ICollection)context).Count);

            Assert.Same(source1.CurrencyManager, context[dataSource1]);
            Assert.Same(source2.CurrencyManager, context[dataSource2]);
        }

        [Fact]
        public void BindingContext_Add_NullListManager_ThrowsArgumentNullException()
        {
            var context = new BindingContext();
            Assert.Throws<ArgumentNullException>("listManager", () => context.Add(1, null));
        }

        [Fact]
        public void BindingContext_AddCore_Invoke_GetReturnsExpected()
        {
            var context = new SubBindingContext();
            var source1 = new BindingSource();
            var source2 = new BindingSource();
            var dataSource = new DataSource();
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
            var context = new SubBindingContext();
            var source1 = new BindingSource();
            var source2 = new BindingSource();
            var dataSource1 = new DataSource();
            var dataSource2 = new DataSource();
            context.AddCore(dataSource1, source1.CurrencyManager);
            context.AddCore(dataSource2, source2.CurrencyManager);
            Assert.Equal(2, ((ICollection)context).Count);

            Assert.Same(source1.CurrencyManager, context[dataSource1]);
            Assert.Same(source2.CurrencyManager, context[dataSource2]);
        }

        [Fact]
        public void BindingContext_AddCore_NullDataSource_ThrowsArgumentNullException()
        {
            var context = new SubBindingContext();
            var source = new BindingSource();
            Assert.Throws<ArgumentNullException>("dataSource", () => context.AddCore(null, source.CurrencyManager));
        }

        [Fact]
        public void BindingContext_AddCore_NullListManager_ThrowsArgumentNullException()
        {
            var context = new SubBindingContext();
            Assert.Throws<ArgumentNullException>("listManager", () => context.AddCore(1, null));
        }

        [Fact]
        public void BindingContext_CopyTo_Invoke_Success()
        {
            var context = new BindingContext();
            var source = new BindingSource();
            var dataSource = new DataSource();
            context.Add(dataSource, source.CurrencyManager);

            var array = new object[] { 1, 2, 3 };
            ((ICollection)context).CopyTo(array, 1);
            Assert.Equal(1, array[0]);
            Assert.NotNull(Assert.IsType<DictionaryEntry>(array[1]).Key);
            Assert.Equal(source.CurrencyManager, Assert.IsType<WeakReference>(Assert.IsType<DictionaryEntry>(array[1]).Value).Target);
            Assert.Equal(3, array[2]);
        }

        [Fact]
        public void BindingContext_CopyTo_WithNullWeakReferenceTarget_ScrubsWeakRefs()
        {
            var context = new BindingContext();
            var dataSource1 = new DataSource { Property = 1 };
            var dataSource2 = new DataSource { Property = 2 };
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
            var destArray = new object[] { 1, 2, 3, 4 };
            ((ICollection)context).CopyTo(destArray, 1);
            Assert.Equal(1, destArray[0]);
            Assert.IsType<DictionaryEntry>(destArray[1]);
            Assert.IsType<DictionaryEntry>(destArray[2]);
            Assert.Equal(4, destArray[3]);
        }

        [Fact]
        public void BindingContext_GetEnumerator_WithNullWeakReferenceTarget_ScrubsWeakRefs()
        {
            var context = new BindingContext();
            var dataSource1 = new DataSource { Property = 1 };
            var dataSource2 = new DataSource { Property = 2 };
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
            var context = new BindingContext();
            var source1 = new BindingSource();
            var dataSource1 = new DataSource();
            var source2 = new BindingSource();
            var dataSource2 = new DataSource();
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
            var context = new SubBindingContext();
            Assert.Throws<ArgumentNullException>("dataSource", () => context.Remove(null));
        }

        [Fact]
        public void BindingContext_RemoveCore_Invoke_Success()
        {
            var context = new SubBindingContext();
            var source1 = new BindingSource();
            var dataSource1 = new DataSource();
            var source2 = new BindingSource();
            var dataSource2 = new DataSource();
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
            var context = new SubBindingContext();
            Assert.Throws<ArgumentNullException>("dataSource", () => context.RemoveCore(null));
        }

        [Fact]
        public void BindingContext_Clear_Empty_Success()
        {
            var context = new BindingContext();
            context.Clear();
            Assert.Empty(context);

            // Clear again.
            context.Clear();
            Assert.Empty(context);
        }

        [Fact]
        public void BindingContext_Clear_NotEmpty_Success()
        {
            var context = new BindingContext();
            var source = new BindingSource();
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
            var context = new SubBindingContext();
            context.ClearCore();
            Assert.Empty(context);

            // Clear again.
            context.ClearCore();
            Assert.Empty(context);
        }

        [Fact]
        public void BindingContext_ClearCore_NotEmpty_Success()
        {
            var context = new SubBindingContext();
            var source = new BindingSource();
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
            var context = new BindingContext();
            var source = new BindingSource();
            var dataSource = new DataSource();
            context.Add(dataSource, source.CurrencyManager);
            Assert.True(context.Contains(dataSource));
            Assert.True(context.Contains(dataSource, null));
            Assert.True(context.Contains(dataSource, string.Empty));
            Assert.False(context.Contains(1));
            Assert.False(context.Contains(1, null));
            Assert.False(context.Contains(1, string.Empty));
        }

        public static IEnumerable<object[]> Contains_DataSourceDataMember_TestData()
        {
            var context = new BindingContext();
            var source = new BindingSource();
            var dataSource1 = new DataSource();
            var dataSource2 = new DataSource();
            context.Add(dataSource1, source.CurrencyManager);
            Assert.NotNull(context[dataSource2, "Property"]);

            yield return new object[] { context, dataSource1, string.Empty, true };
            yield return new object[] { context, dataSource1, null, true };
            yield return new object[] { context, dataSource1, "Property", false };
            yield return new object[] { context, dataSource2, "Property", true };
            yield return new object[] { context, dataSource2, "property", true };
            yield return new object[] { context, dataSource2, "NoSuchProperty", false };
            yield return new object[] { context, dataSource2, string.Empty, true };
            yield return new object[] { context, dataSource2, null, true };
            yield return new object[] { context, 1, "Property", false };
        }

        // This is failing sporadically, breaking random builds. Disabling it to allow code to flow.
        // Issue is tracked at https://github.com/dotnet/winforms/issues/1031
        // [Theory]
        // [MemberData(nameof(Contains_DataSourceDataMember_TestData))]
        // public void BindingContext_Contains_DataSourceDataMember_ReturnsExpected(BindingContext context, object dataSource, string dataMember, bool expected)
        // {
        //     Assert.Equal(expected, context.Contains(dataSource, dataMember));
        // }

        [Fact]
        public void BindingContext_Contains_NullDataSource_ThrowsArgumentNullException()
        {
            var context = new BindingContext();
            Assert.Throws<ArgumentNullException>("dataSource", () => context.Contains(null));
            Assert.Throws<ArgumentNullException>("dataSource", () => context.Contains(null, null));
            Assert.Throws<ArgumentNullException>("dataSource", () => context.Contains(null, string.Empty));
        }

        [Fact]
        public void BindingContext_Item_GetNoSuchDataSource_AddsToCollection()
        {
            var context = new BindingContext();
            var dataSource = new DataSource();
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
            var context = new BindingContext();
            var dataSource = new List<int> { 1, 2, 3 };
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
            var context = new BindingContext();
            var dataSource = new int[] { 1, 2, 3 };
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
            var context = new BindingContext();
            var dataSource = new List<int> { 1, 2, 3 };
            var mockIListSource = new Mock<IListSource>(MockBehavior.Strict);
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
            var context = new BindingContext();
            var mockIListSource = new Mock<IListSource>(MockBehavior.Strict);
            mockIListSource
                .Setup(s => s.GetList())
                .Returns((IList)null);

            Assert.Throws<ArgumentNullException>("dataSource", () => context[mockIListSource.Object]);
            Assert.Empty(context);
        }

        [Fact]
        public void BindingContext_Item_GetWithICurrencyManagerProvider_DoesNotAddToCollection()
        {
            var context = new BindingContext();
            CurrencyManager manager = new BindingSource().CurrencyManager;
            var mockCurrencyManagerProvider = new Mock<ICurrencyManagerProvider>();
            mockCurrencyManagerProvider
                .Setup(p => p.GetRelatedCurrencyManager("dataMember"))
                .Returns(manager);

            Assert.Same(manager, context[mockCurrencyManagerProvider.Object, "dataMember"]);
            Assert.Empty(context);
        }

        [Fact]
        public void BindingContext_Item_GetWithNullICurrencyManagerProvider_AddsToCollection()
        {
            var context = new BindingContext();
            var mockCurrencyManagerProvider = new Mock<ICurrencyManagerProvider>();
            mockCurrencyManagerProvider
                .Setup(p => p.GetRelatedCurrencyManager("dataMember"))
                .Returns((CurrencyManager)null);

            PropertyManager manager = Assert.IsType<PropertyManager>(context[mockCurrencyManagerProvider.Object]);
            Assert.Single(context);
            Assert.Same(manager, context[mockCurrencyManagerProvider.Object]);
        }

        public static IEnumerable<object[]> Item_DataSourceWithDataMember_TestData()
        {
            var dataSource = new ParentDataSource
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
            var context = new BindingContext();
            PropertyManager manager = Assert.IsAssignableFrom<PropertyManager>(context[dataSource, dataMember]);
            Assert.Equal(expectedCurrent, manager.Current);
            Assert.Equal(1, manager.Count);
            Assert.Equal(0, manager.Position);
            Assert.Equal(expectedCount, ((ICollection)context).Count);
            Assert.Same(manager, context[dataSource, dataMember]);
            Assert.Same(manager, context[dataSource, dataMember.ToLower()]);
        }

        [Fact]
        public void BindingContext_Item_GetWithAddedDataSourceWithDataMember_ThrowsArgumentException()
        {
            var context = new BindingContext();
            var source = new BindingSource();
            var dataSource = new DataSource();
            context.Add(dataSource, source.CurrencyManager);
            Assert.Throws<ArgumentException>(null, () => context[dataSource, "Property"]);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void BindingContext_Item_GetNoSuchDataSourceNullOrEmptyMember_AddsToCollection(string dataMember)
        {
            var context = new SubBindingContext();
            var dataSource = new DataSource();
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
            var context = new SubBindingContext();
            var dataSource = new ParentDataSource();
            Assert.Throws<ArgumentException>(null, () => context[dataSource, dataMember]);
        }

        [Fact]
        public void BindingContext_Item_GetIListWithDataMemberReturningIList_AddsToCollection()
        {
            var context = new BindingContext();
            var list = new List<int> { 1, 2, 3 };
            var dataSource = new IListDataSource
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
            var context = new BindingContext();
            var list = new List<int> { 1, 2, 3 };
            var dataSource = new ObjectDataSource
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
            var context = new BindingContext();
            var list = new int[] { 1, 2, 3 };
            var dataSource = new IListDataSource
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
            var context = new BindingContext();
            var list = new List<int> { 1, 2, 3 };
            var dataSource = new IListDataSource();
            var mockIListSource = new Mock<IListSource>(MockBehavior.Strict);
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
            var context = new BindingContext();
            var list = new List<int> { 1, 2, 3 };
            var dataSource = new IListSourceDataSource();
            var mockIListSource = new Mock<IListSource>(MockBehavior.Strict);
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

        [Fact]
        public void BindingContext_Item_GetIListSourceDataSourceWithDataMemberReturningIListNull_ThrowsArgumentNullException()
        {
            var context = new BindingContext();
            var dataSource = new IListDataSource();
            var mockIListSource = new Mock<IListSource>(MockBehavior.Strict);
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
            var context = new BindingContext();
            var dataSource = new IListSourceDataSource();
            var mockIListSource = new Mock<IListSource>(MockBehavior.Strict);
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
            var context = new BindingContext();
            var dataSource = new DataSource();
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
            var context = new SubBindingContext();
            Assert.Throws<ArgumentNullException>("dataSource", () => context[null]);
            Assert.Throws<ArgumentNullException>("dataSource", () => context[null, null]);
            Assert.Throws<ArgumentNullException>("dataSource", () => context[null, string.Empty]);
        }

        [Fact]
        public void BindingContext_CollectionChanged_Add_ThrowsNotImplementedException()
        {
            var context = new BindingContext();
            CollectionChangeEventHandler handler = (sender, e) => { };
            Assert.Throws<NotImplementedException>(() => context.CollectionChanged += handler);
        }

        [Fact]
        public void BindingContext_CollectionChanged_Remove_Nop()
        {
            var context = new BindingContext();
            CollectionChangeEventHandler handler = (sender, e) => { };
            context.CollectionChanged -= handler;
        }

        [Fact]
        public void BindingContext_OnCollectionChanged_Invoke_Nop()
        {
            var context = new SubBindingContext();
            CollectionChangeEventHandler handler = (sender, e) => { };
            context.OnCollectionChanged(null);
        }

        [Fact]
        public void BindingContext_KeyEquals_Invoke_ReturnsExpected()
        {
            var context1 = new BindingContext();
            var context2 = new BindingContext();
            var context3 = new BindingContext();
            var context4 = new BindingContext();
            var source1 = new BindingSource();
            var source2 = new BindingSource();
            var dataSource1 = new DataSource();
            var dataSource2 = new DataSource();

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
            var context = new BindingContext();
            var dataSource = new DataSource();
            var binding = new Binding(null, dataSource, "dataMember");

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
            var context = new BindingContext();
            var dataSource = new List<int> { 1, 2, 3 };
            var binding = new Binding(null, dataSource, "dataMember");

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
            var context = new BindingContext();
            var dataSource = new DataSource { Property = 1 };
            var binding = new Binding(null, dataSource, "Property.ignored");

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
            var context = new BindingContext();
            var list = new List<int> { 1, 2, 3 };
            var dataSource = new IListDataSource { Property = list };
            var binding = new Binding(null, dataSource, "Property.ignore");

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
            var context1 = new BindingContext();
            var context2 = new BindingContext();
            var dataSource = new DataSource();
            var binding = new Binding(null, dataSource, "dataMember");

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
            var context1 = new BindingContext();
            var context2 = new BindingContext();
            var dataSource = new List<int> { 1, 2, 3 };
            var binding = new Binding(null, dataSource, "dataMember");

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
            var context = new BindingContext();
            var dataSource = new DataSource();
            var binding = new Binding(null, dataSource, "dataMember");

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
            var context = new BindingContext();
            var dataSource = new DataSource();
            var binding = new Binding(null, dataSource, "dataMember");

            BindingContext.UpdateBinding(context, binding);
            BindingManagerBase oldManager = binding.BindingManagerBase;
            int callCount = 0;
            oldManager.Bindings.CollectionChanged += (sender, e) =>
            {
                if (e.Action == CollectionChangeAction.Remove)
                {
                    Assert.Null(binding.BindingManagerBase);
                    oldManager.Bindings.Add(binding);
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
            var context = new BindingContext();
            var dataSource = new DataSource();
            var binding = new Binding(null, dataSource, "dataMember");
            var control = new Control();
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
                    oldManager.Bindings.Add(binding);
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
            Assert.Throws<ArgumentNullException>("binding", () => BindingContext.UpdateBinding(new BindingContext(), null));
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
            public new void AddCore(object dataSource, BindingManagerBase listManager)
            {
                base.AddCore(dataSource, listManager);
            }

            public new void RemoveCore(object dataSource)
            {
                base.RemoveCore(dataSource);
            }

            public new void ClearCore()
            {
                base.ClearCore();
            }

            public new void OnCollectionChanged(CollectionChangeEventArgs ccevent)
            {
                base.OnCollectionChanged(ccevent);
            }
        }
    }
}
