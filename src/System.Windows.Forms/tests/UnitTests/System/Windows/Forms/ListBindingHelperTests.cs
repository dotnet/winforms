// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using Moq;

namespace System.Windows.Forms.Tests;

// NB: doesn't require thread affinity
public class ListBindingHelperTests
{
    public static IEnumerable<object[]> GetList_Object_TestData()
    {
        yield return new object[] { null, null };
        yield return new object[] { 1, 1 };
        yield return new object[] { typeof(int), typeof(int) };
        yield return new object[] { new int[] { 1 }, new int[] { 1 } };
        yield return new object[] { typeof(int[]), typeof(int[]) };

        Mock<IListSource> mockSource = new(MockBehavior.Strict);
        mockSource
            .Setup(s => s.GetList())
            .Returns(new int[] { 1 });
        yield return new object[] { mockSource.Object, new int[] { 1 } };
    }

    [Theory]
    [MemberData(nameof(GetList_Object_TestData))]
    public void ListBindingHelper_GetList_InvokeObject_ReturnsExpected(object list, object expected)
    {
        Assert.Equal(expected, ListBindingHelper.GetList(list));
    }

    public static IEnumerable<object[]> GetList_Object_String_TestData()
    {
        yield return new object[] { null, "dataMember", null };

        yield return new object[] { new DataClass { Property = 1 }, "Property", 1 };
        yield return new object[] { new DataClass { Property = 1 }, "property", 1 };
        yield return new object[] { typeof(DataClass), "NoSuchProperty", typeof(DataClass) };
        yield return new object[] { typeof(DataClass), "Property", typeof(DataClass) };

        yield return new object[] { new ListDataClass(), "ListProperty", null };
        yield return new object[] { new ListDataClass { ListProperty = [] }, "ListProperty", new List<DataClass>() };
        yield return new object[] { new IEnumerableWrapper(new object[] { new DataClass { Property = 1 } }), "Property", 1 };

        Mock<ICurrencyManagerProvider> nullCurrencyManagerProvider = new(MockBehavior.Strict);
        nullCurrencyManagerProvider
            .Setup(c => c.CurrencyManager)
            .Returns((CurrencyManager)null);
        yield return new object[] { nullCurrencyManagerProvider.Object, "CurrencyManager", null };

        BindingSource emptySource = [];
        Mock<ICurrencyManagerProvider> invalidCurrencyManagerProvider = new(MockBehavior.Strict);
        invalidCurrencyManagerProvider
            .Setup(c => c.CurrencyManager)
            .Returns(emptySource.CurrencyManager);
        yield return new object[] { invalidCurrencyManagerProvider.Object, "CurrencyManager", null };

        BindingSource validSource = new(new List<CustomCurrencyManagerProvider> { new() { Property = 1 } }, null);
        yield return new object[] { new CustomCurrencyManagerProvider { CurrencyManagerResult = validSource.CurrencyManager }, "Property", 1 };
    }

    [Theory]
    [MemberData(nameof(GetList_Object_String_TestData))]
    public void ListBindingHelper_GetList_InvokeObjectString_ReturnsExpected(object dataSource, string dataMember, object expected)
    {
        Assert.Equal(expected, ListBindingHelper.GetList(dataSource, dataMember));
    }

    public static IEnumerable<object[]> GetList_NoSuchDataMember_TestData()
    {
        yield return new object[] { new DataClass(), "NoSuchProperty" };
        yield return new object[] { new IEnumerableWrapper(Array.Empty<object>()), "NoSuchProperty" };
        yield return new object[] { new IEnumerableWrapper(Array.Empty<object>()), "Property" };
    }

    [Theory]
    [MemberData(nameof(GetList_NoSuchDataMember_TestData))]
    public void ListBindingHelper_GetList_NoSuchDataMember_ThrowsArgumentException(object dataSource, string dataMember)
    {
        Assert.Throws<ArgumentException>(() => ListBindingHelper.GetList(dataSource, dataMember));
    }

    public static IEnumerable<object[]> GetList_DataMember_TestData()
    {
        yield return new object[] { null, null, null };
        yield return new object[] { null, string.Empty, null };
        yield return new object[] { null, "reasonable", null };
        yield return new object[] { 1, null, 1 };
        yield return new object[] { 1, string.Empty, 1 };
        yield return new object[] { typeof(int), null, typeof(int) };
        yield return new object[] { typeof(int), string.Empty, typeof(int) };
        yield return new object[] { typeof(int), "reasonable", typeof(int) };

        Mock<IListSource> mockSource = new(MockBehavior.Strict);
        mockSource
            .Setup(s => s.GetList())
            .Returns(new int[] { 1 });
        yield return new object[] { mockSource.Object, null, new int[] { 1 } };
        yield return new object[] { mockSource.Object, string.Empty, new int[] { 1 } };
    }

    [Theory]
    [MemberData(nameof(GetList_DataMember_TestData))]
    public void ListBindingHelper_GetList_InvokeDataMember_ReturnsExpected(object dataSource, string dataMember, object expected)
    {
        Assert.Equal(expected, ListBindingHelper.GetList(dataSource, dataMember));
    }

    public static IEnumerable<object[]> GetListItemType_Object_TestData()
    {
        yield return new object[] { null, null };

        yield return new object[] { 1, typeof(int) };
        yield return new object[] { typeof(int), typeof(int) };

        yield return new object[] { new int[] { 1 }, typeof(int) };
        yield return new object[] { typeof(int[]), typeof(int) };

        yield return new object[] { new List<int> { 1 }, typeof(int) };
        yield return new object[] { typeof(List<int>), typeof(int) };

        yield return new object[] { typeof(IList<int>), typeof(int) };
        yield return new object[] { typeof(IList), typeof(IList) };
        yield return new object[] { typeof(ICollection<int>), typeof(ICollection<int>) };
        yield return new object[] { typeof(IEnumerable), typeof(IEnumerable) };

        yield return new object[] { new Dictionary<int, string> { { 1, "string" } }, typeof(KeyValuePair<int, string>) };
        yield return new object[] { typeof(Dictionary<int, string>), typeof(Dictionary<int, string>) };

        yield return new object[] { new ClassWithItem(), typeof(ClassWithItem) };
        yield return new object[] { typeof(ClassWithItem), typeof(ClassWithItem) };

        Mock<IListSource> genericMockSource = new(MockBehavior.Strict);
        genericMockSource.Setup(s => s.GetList()).Returns(new GenericIListSourceClassWithItem());
        yield return new object[] { genericMockSource.Object, typeof(int) };

        Mock<IListSource> nonGenericMockSource = new(MockBehavior.Strict);
        nonGenericMockSource.Setup(s => s.GetList()).Returns(new GenericIListSourceClassWithItem());
        yield return new object[] { nonGenericMockSource.Object, typeof(int) };

        yield return new object[] { new ITypedListClassWithItem(), typeof(string) };
        yield return new object[] { new ITypedListClassWithCustomNamedItem(), typeof(string) };
        yield return new object[] { new OnlyGenericIListClassWithItem(), typeof(string) };

        yield return new object[] { new ClassWithIListSource(), typeof(int) };
        yield return new object[] { typeof(ClassWithIListSource), typeof(int) };

        yield return new object[] { new NullIListSource(), null };
        yield return new object[] { typeof(NullIListSource), null };

        // Type that wraps an enumerable but ONLY implements non-generic IEnumerable.
        yield return new object[] { new IEnumerableWrapper(null), typeof(object) };
        yield return new object[] { new IEnumerableWrapper(Array.Empty<object>()), typeof(object) };
        yield return new object[] { new IEnumerableWrapper(new int[] { 1 }), typeof(int) };
        yield return new object[] { new IEnumerableWrapper(new object[] { 1 }), typeof(int) };
        yield return new object[] { new IEnumerableWrapper(new object[] { 1, string.Empty }), typeof(int) };
        yield return new object[] { new IEnumerableWrapper(new object[] { null }), typeof(object) };
        yield return new object[] { new NotSupportedIEnumerable(), typeof(object) };

        // Type that only implements IList.
        yield return new object[] { new ArrayList(), typeof(object) };
        yield return new object[] { new ArrayList { 1 }, typeof(int) };
        yield return new object[] { new ArrayList { 1, string.Empty }, typeof(int) };
        yield return new object[] { new ArrayList { null }, typeof(object) };
    }

    [Theory]
    [MemberData(nameof(GetListItemType_Object_TestData))]
    public void ListBindingHelper_GetListItemType_InvokeObject_ReturnsExpected(object list, Type expected)
    {
        Assert.Equal(expected, ListBindingHelper.GetListItemType(list));
        Assert.Equal(list is null ? typeof(object) : expected, ListBindingHelper.GetListItemType(list, null));
        Assert.Equal(list is null ? typeof(object) : expected, ListBindingHelper.GetListItemType(list, string.Empty));
    }

    public static IEnumerable<object[]> GetListItemType_Object_String_TestData()
    {
        yield return new object[] { null, "dataMember", typeof(object) };
        yield return new object[] { new NullIListSource(), "dataMember", typeof(object) };
        yield return new object[] { new DataClass(), "noSuchMember", typeof(object) };
        yield return new object[] { new DataClass { Property = 1 }, "Property", typeof(int) };
        yield return new object[] { new DataClass { Property = 1 }, "property", typeof(int) };
        yield return new object[] { new ICustomTypeDescriptorPropertyClass(), "Property", typeof(ICustomTypeDescriptor) };

        Mock<ITypedList> nullMockTypedList = new(MockBehavior.Strict);
        nullMockTypedList.Setup(t => t.GetItemProperties(null)).Returns((PropertyDescriptorCollection)null);
        yield return new object[] { nullMockTypedList.Object, "dataMember", typeof(object) };

        ICustomTypeDescriptorPropertyDescriptorClass instance = new();
        Mock<ICustomTypeDescriptor> customTypeDescriptor = new(MockBehavior.Strict);
        customTypeDescriptor
            .Setup(t => t.GetProperties())
            .Returns(new PropertyDescriptorCollection([new CustomPropertyDescriptor("Property", null)]));
        Mock<TypeDescriptionProvider> customPropertyDescriptorProvider = new(MockBehavior.Strict);
        customPropertyDescriptorProvider
            .Setup(p => p.GetTypeDescriptor(instance.GetType(), instance))
            .Returns(customTypeDescriptor.Object);
        customPropertyDescriptorProvider
            .Setup(p => p.GetCache(instance))
            .Returns((IDictionary)null);
        customPropertyDescriptorProvider
            .Setup(p => p.GetExtendedTypeDescriptor(instance))
            .Returns(customTypeDescriptor.Object);
        TypeDescriptor.AddProvider(customPropertyDescriptorProvider.Object, instance);
        yield return new object[] { instance, "Property", typeof(object) };
    }

    [Theory]
    [MemberData(nameof(GetListItemType_Object_String_TestData))]
    public void ListBindingHelper_GetListItemType_InvokeObjectString_ReturnsExpected(object list, string dataMember, Type expected)
    {
        Assert.Equal(expected, ListBindingHelper.GetListItemType(list, dataMember));
    }

    [Theory]
    [InlineData(typeof(PrivateDefaultConstructor))]
    [InlineData(typeof(NoDefaultConstructor))]
    [InlineData(typeof(ThrowingDefaultConstructor))]
    public void ListBindingHelper_GetListItemType_InvalidIListSourceType_ThrowsNotSupportedException(Type list)
    {
        Assert.Throws<NotSupportedException>(() => ListBindingHelper.GetListItemType(list));
        Assert.Throws<NotSupportedException>(() => ListBindingHelper.GetListItemType(list, null));
        Assert.Throws<NotSupportedException>(() => ListBindingHelper.GetListItemType(list, string.Empty));
        Assert.Throws<NotSupportedException>(() => ListBindingHelper.GetListItemType(list, "dataMember"));
    }

    public static IEnumerable<object[]> GetListItemProperties_Object_TestData()
    {
        // Simple objects.
        yield return new object[] { null, Array.Empty<string>() };
        yield return new object[] { 1, Array.Empty<string>() };
        yield return new object[] { typeof(int), Array.Empty<string>() };

        yield return new object[] { string.Empty, new string[] { "Length" } };
        yield return new object[] { typeof(string), new string[] { "Length" } };

        yield return new object[] { new DataClass(), new string[] { "Property" } };
        yield return new object[] { typeof(DataClass), new string[] { "Property" } };

        // Lists.
        yield return new object[] { new List<DataClass>(), new string[] { "Property" } };
        yield return new object[] { typeof(IList<DataClass>), new string[] { "Property" } };

        yield return new object[] { new List<ICustomTypeDescriptor>(), Array.Empty<string>() };
        yield return new object[] { typeof(IList<ICustomTypeDescriptor>), new string[] { nameof(ICustomTypeDescriptor.RequireRegisteredTypes) } };

        // Array.
        yield return new object[] { Array.Empty<DataClass>(), new string[] { "Property" } };
        yield return new object[] { typeof(DataClass[]), new string[] { "Property" } };
        yield return new object[] { Array.Empty<object>(), Array.Empty<string>() };
        yield return new object[] { new object[] { new DataClass() }, Array.Empty<string>() };

        // Only implements IEnumerable.
        yield return new object[] { new IEnumerableWrapper(Array.Empty<object>()), Array.Empty<string>() };
        yield return new object[] { new IEnumerableWrapper(new object[] { new DataClass() }), new string[] { "Property" } };
        yield return new object[] { new IEnumerableWrapper(new object[] { 1 }), new string[] { "Property" } };
        yield return new object[] { new IEnumerableWrapper(new object[] { null }), Array.Empty<string>() };

        // Only implements IList.
        yield return new object[] { new ArrayList(), Array.Empty<string>() };
        yield return new object[] { new ArrayList { new DataClass() }, new string[] { "Property" } };
        yield return new object[] { new ArrayList { 1 }, Array.Empty<string>() };
        yield return new object[] { new ArrayList { null }, Array.Empty<string>() };

        // ITypedList.
        Mock<ITypedList> mockTypedList = new(MockBehavior.Strict);
        mockTypedList.Setup(t => t.GetItemProperties(null)).Returns(TypeDescriptor.GetProperties(typeof(DataClass)));
        yield return new object[] { mockTypedList.Object, new string[] { "Property" } };

        Mock<ITypedList> nullMockTypedList = new(MockBehavior.Strict);
        nullMockTypedList.Setup(t => t.GetItemProperties(null)).Returns((PropertyDescriptorCollection)null);
        yield return new object[] { nullMockTypedList.Object, null };

        yield return new object[] { new EnumerableITypedListImplementor(), new string[] { "Property" } };
        yield return new object[] { typeof(EnumerableITypedListImplementor), new string[] { "OtherProperty" } };

        yield return new object[] { new NonEnumerableITypedListImplementor(), new string[] { "Property" } };
        yield return new object[] { typeof(NonEnumerableITypedListImplementor), new string[] { "OtherProperty" } };
    }

    [Theory]
    [MemberData(nameof(GetListItemProperties_Object_TestData))]
    public void ListBindingHelper_GetListItemProperties_InvokeObject_ReturnsExpected(object list, string[] expected)
    {
        IEnumerable<PropertyDescriptor> properties = ListBindingHelper.GetListItemProperties(list)?.Cast<PropertyDescriptor>();
        Assert.Equal(expected, properties?.Select(p => p.Name));

        // Null list accessors.
        properties = ListBindingHelper.GetListItemProperties(list, null)?.Cast<PropertyDescriptor>();
        Assert.Equal(expected, properties?.Select(p => p.Name));

        // Empty list accessors.
        properties = ListBindingHelper.GetListItemProperties(list, Array.Empty<PropertyDescriptor>())?.Cast<PropertyDescriptor>();
        Assert.Equal(expected, properties?.Select(p => p.Name));

        // Null data member.
        properties = ListBindingHelper.GetListItemProperties(list, null, Array.Empty<PropertyDescriptor>())?.Cast<PropertyDescriptor>();
        Assert.Equal(expected, properties?.Select(p => p.Name));

        // Empty data member.
        properties = ListBindingHelper.GetListItemProperties(list, string.Empty, Array.Empty<PropertyDescriptor>())?.Cast<PropertyDescriptor>();
        Assert.Equal(expected, properties?.Select(p => p.Name));
    }

    public static IEnumerable<object[]> GetListItemProperties_Object_PropertyDescriptorArray_TestData()
    {
        yield return new object[] { null, TypeDescriptor.GetProperties(typeof(DataClass)).Cast<PropertyDescriptor>().ToArray(), Array.Empty<string>() };

        yield return new object[] { new DataClass(), TypeDescriptor.GetProperties(typeof(DataClass)).Cast<PropertyDescriptor>().ToArray(), Array.Empty<string>() };
        yield return new object[] { typeof(DataClass), TypeDescriptor.GetProperties(typeof(DataClass)).Cast<PropertyDescriptor>().ToArray(), Array.Empty<string>() };

        yield return new object[] { new ListDataClass(), TypeDescriptor.GetProperties(typeof(ListDataClass)).Cast<PropertyDescriptor>().ToArray(), new string[] { "Property" } };
        yield return new object[] { new ListDataClass() { ListProperty = [new()] }, TypeDescriptor.GetProperties(typeof(ListDataClass)).Cast<PropertyDescriptor>().ToArray(), new string[] { "Property" } };
        yield return new object[] { typeof(ListDataClass), TypeDescriptor.GetProperties(typeof(ListDataClass)).Cast<PropertyDescriptor>().ToArray(), new string[] { "Property" } };

        yield return new object[] { new MultiListDataClass(), TypeDescriptor.GetProperties(typeof(MultiListDataClass)).Cast<PropertyDescriptor>().ToArray(), new string[] { "ListProperty" } };
        yield return new object[] { new MultiListDataClass { ParentListProperty = [new() { ListProperty = [new()] }] }, TypeDescriptor.GetProperties(typeof(MultiListDataClass)).Cast<PropertyDescriptor>().Take(0).ToArray(), new string[] { "ParentListProperty" } };

        var inner = new PropertyDescriptor[] { TypeDescriptor.GetProperties(typeof(MultiListDataClass))[0], TypeDescriptor.GetProperties(typeof(ListDataClass))[0], TypeDescriptor.GetProperties(typeof(DataClass))[0] };
        yield return new object[] { new MultiListDataClass { ParentListProperty = [new() { ListProperty = [new()] }] }, inner.Take(2).ToArray(), new string[] { "Property" } };
        yield return new object[] { typeof(MultiListDataClass), TypeDescriptor.GetProperties(typeof(MultiListDataClass)).Cast<PropertyDescriptor>().ToArray(), new string[] { "ListProperty" } };

        yield return new object[] { typeof(DataClass), TypeDescriptor.GetProperties(typeof(ListDataClass)).Cast<PropertyDescriptor>().ToArray(), new string[] { "Property" } };
        yield return new object[] { new DataClass(), new PropertyDescriptor[] { null }, Array.Empty<string>() };
        yield return new object[] { typeof(DataClass), new PropertyDescriptor[] { null }, Array.Empty<string>() };

        // Only implements IEnumerable.
        PropertyDescriptor[] descriptors = TypeDescriptor.GetProperties(typeof(ListDataClass)).Cast<PropertyDescriptor>().ToArray();
        yield return new object[] { new IEnumerableWrapper(Array.Empty<object>()), descriptors, new string[] { "Property" } };
        yield return new object[] { new IEnumerableWrapper(new object[] { new ListDataClass() }), descriptors, new string[] { "Property" } };
        yield return new object[] { new IEnumerableWrapper(new object[] { new MultiListDataClass() }), inner.Take(2).ToArray(), new string[] { "Property" } };
        yield return new object[] { new IEnumerableWrapper(new object[] { null }), descriptors, new string[] { "Property" } };
        yield return new object[] { new IEnumerableWrapper(new ListDataClass[] { null }), descriptors, new string[] { "Property" } };

        // Only implements IList.
        yield return new object[] { new ArrayList(), descriptors, new string[] { "Property" } };
        yield return new object[] { new ArrayList { new ListDataClass() }, descriptors, new string[] { "Property" } };
        yield return new object[] { new ArrayList { new MultiListDataClass() }, inner.Take(0).ToArray(), new string[] { "ParentListProperty" } };
        yield return new object[] { new ArrayList { new MultiListDataClass() }, inner.Take(2).ToArray(), new string[] { "Property" } };
        yield return new object[] { new ArrayList { new MultiListDataClass { ParentListProperty = [] } }, inner.Take(0).ToArray(), new string[] { "ParentListProperty" } };
        yield return new object[] { new ArrayList { new MultiListDataClass { ParentListProperty = [] } }, inner.Take(2).ToArray(), new string[] { "Property" } };
        yield return new object[] { new ArrayList { new MultiListDataClass { ParentListProperty = [new() { ListProperty = [new()] }] } }, inner.Take(0).ToArray(), new string[] { "ParentListProperty" } };
        yield return new object[] { new ArrayList { new MultiListDataClass { ParentListProperty = [new() { ListProperty = [new()] }] } }, inner.Take(2).ToArray(), new string[] { "Property" } };
        yield return new object[] { new ArrayList { new MultiListDataClass { ParentListProperty = [new() { ListProperty = [new()] }] } }, inner.Take(3).ToArray(), Array.Empty<string>() };
        yield return new object[] { new ArrayList { null }, descriptors, new string[] { "Property" } };

        // ITypedList.
        Mock<ITypedList> mockTypedList = new(MockBehavior.Strict);
        mockTypedList.Setup(t => t.GetItemProperties(descriptors)).Returns(TypeDescriptor.GetProperties(typeof(DataClass)));
        yield return new object[] { mockTypedList.Object, descriptors, new string[] { "Property" } };

        Mock<ITypedList> nullMockTypedList = new(MockBehavior.Strict);
        nullMockTypedList.Setup(t => t.GetItemProperties(descriptors)).Returns((PropertyDescriptorCollection)null);
        yield return new object[] { nullMockTypedList.Object, descriptors, null };

        yield return new object[] { new EnumerableITypedListImplementor(), descriptors, new string[] { "Property" } };
        yield return new object[] { typeof(EnumerableITypedListImplementor), descriptors, new string[] { "Property" } };
        yield return new object[] { new EnumerableITypedListImplementor[] { new() }, TypeDescriptor.GetProperties(typeof(EnumerableITypedListImplementor)).Cast<PropertyDescriptor>().ToArray(), Array.Empty<string>() };
        yield return new object[] { new List<EnumerableITypedListImplementor> { new() }, TypeDescriptor.GetProperties(typeof(EnumerableITypedListImplementor)).Cast<PropertyDescriptor>().ToArray(), Array.Empty<string>() };
        yield return new object[] { new ArrayList { new EnumerableITypedListImplementor() }, TypeDescriptor.GetProperties(typeof(EnumerableITypedListImplementor)).Cast<PropertyDescriptor>().ToArray(), Array.Empty<string>() };
        yield return new object[] { new IEnumerableWrapper(new object[] { new EnumerableITypedListImplementor() }), TypeDescriptor.GetProperties(typeof(EnumerableITypedListImplementor)).Cast<PropertyDescriptor>().ToArray(), Array.Empty<string>() };
        yield return new object[] { typeof(EnumerableITypedListImplementor[]), descriptors, new string[] { "Property" } };

        yield return new object[] { new NonEnumerableITypedListImplementor(), descriptors, new string[] { "Property" } };
        yield return new object[] { typeof(NonEnumerableITypedListImplementor), descriptors, new string[] { "Property" } };
        yield return new object[] { new NonEnumerableITypedListImplementor[] { new() }, TypeDescriptor.GetProperties(typeof(NonEnumerableITypedListImplementor)).Cast<PropertyDescriptor>().ToArray(), Array.Empty<string>() };
        yield return new object[] { new List<NonEnumerableITypedListImplementor> { new() }, TypeDescriptor.GetProperties(typeof(NonEnumerableITypedListImplementor)).Cast<PropertyDescriptor>().ToArray(), Array.Empty<string>() };
        yield return new object[] { new ArrayList { new NonEnumerableITypedListImplementor() }, TypeDescriptor.GetProperties(typeof(NonEnumerableITypedListImplementor)).Cast<PropertyDescriptor>().ToArray(), Array.Empty<string>() };
        yield return new object[] { new IEnumerableWrapper(new object[] { new NonEnumerableITypedListImplementor() }), TypeDescriptor.GetProperties(typeof(NonEnumerableITypedListImplementor)).Cast<PropertyDescriptor>().ToArray(), Array.Empty<string>() };
        yield return new object[] { typeof(NonEnumerableITypedListImplementor[]), descriptors, new string[] { "Property" } };

        ITypedListDataClass typedListDataClass = new() { ListProperty = [new()] };
        yield return new object[] { new ITypedListDataClass(), TypeDescriptor.GetProperties(typeof(ITypedListDataClass)).Cast<PropertyDescriptor>().ToArray(), new string[] { "OtherProperty" } };
        yield return new object[] { typedListDataClass, TypeDescriptor.GetProperties(typeof(ITypedListDataClass)).Cast<PropertyDescriptor>().ToArray(), new string[] { "OtherProperty" } };
        yield return new object[] { typeof(ITypedListDataClass), TypeDescriptor.GetProperties(typeof(ITypedListDataClass)).Cast<PropertyDescriptor>().ToArray(), new string[] { "OtherProperty" } };
        yield return new object[] { new ITypedListDataClass[] { new() }, TypeDescriptor.GetProperties(typeof(ITypedListDataClass)).Cast<PropertyDescriptor>().ToArray(), new string[] { "OtherProperty" } };
        yield return new object[] { new ITypedListDataClass[] { typedListDataClass }, TypeDescriptor.GetProperties(typeof(ITypedListDataClass)).Cast<PropertyDescriptor>().ToArray(), new string[] { "OtherProperty" } };
        yield return new object[] { new List<ITypedListDataClass> { new() }, TypeDescriptor.GetProperties(typeof(ITypedListDataClass)).Cast<PropertyDescriptor>().ToArray(), new string[] { "OtherProperty" } };
        yield return new object[] { new List<ITypedListDataClass> { typedListDataClass }, TypeDescriptor.GetProperties(typeof(ITypedListDataClass)).Cast<PropertyDescriptor>().ToArray(), new string[] { "OtherProperty" } };
        yield return new object[] { new ArrayList { new ITypedListDataClass() }, TypeDescriptor.GetProperties(typeof(ITypedListDataClass)).Cast<PropertyDescriptor>().ToArray(), new string[] { "OtherProperty" } };
        yield return new object[] { new ArrayList { typedListDataClass }, TypeDescriptor.GetProperties(typeof(ITypedListDataClass)).Cast<PropertyDescriptor>().ToArray(), new string[] { "OtherProperty" } };
        yield return new object[] { new IEnumerableWrapper(new object[] { new ITypedListDataClass() }), TypeDescriptor.GetProperties(typeof(ITypedListDataClass)).Cast<PropertyDescriptor>().ToArray(), new string[] { "OtherProperty" } };
        yield return new object[] { new IEnumerableWrapper(new object[] { typedListDataClass }), TypeDescriptor.GetProperties(typeof(ITypedListDataClass)).Cast<PropertyDescriptor>().ToArray(), new string[] { "OtherProperty" } };
        yield return new object[] { typeof(ITypedListDataClass[]), TypeDescriptor.GetProperties(typeof(ITypedListDataClass)).Cast<PropertyDescriptor>().ToArray(), new string[] { "OtherProperty" } };

        ITypedListParent typedListParent = new() { ListProperty = new EnumerableITypedListImplementor() };
        yield return new object[] { new ITypedListParent(), TypeDescriptor.GetProperties(typeof(ITypedListParent)).Cast<PropertyDescriptor>().ToArray(), new string[] { "OtherProperty" } };
        yield return new object[] { typedListParent, TypeDescriptor.GetProperties(typeof(ITypedListParent)).Cast<PropertyDescriptor>().ToArray(), new string[] { "Property" } };
        yield return new object[] { new ITypedListParent[] { new() }, TypeDescriptor.GetProperties(typeof(ITypedListParent)).Cast<PropertyDescriptor>().ToArray(), new string[] { "OtherProperty" } };
        yield return new object[] { new ITypedListParent[] { typedListParent }, TypeDescriptor.GetProperties(typeof(ITypedListParent)).Cast<PropertyDescriptor>().ToArray(), new string[] { "Property" } };
        yield return new object[] { new List<ITypedListParent> { new() }, TypeDescriptor.GetProperties(typeof(ITypedListParent)).Cast<PropertyDescriptor>().ToArray(), new string[] { "OtherProperty" } };
        yield return new object[] { new List<ITypedListParent> { typedListParent }, TypeDescriptor.GetProperties(typeof(ITypedListParent)).Cast<PropertyDescriptor>().ToArray(), new string[] { "Property" } };
        yield return new object[] { new ArrayList { new ITypedListParent() }, TypeDescriptor.GetProperties(typeof(ITypedListParent)).Cast<PropertyDescriptor>().ToArray(), new string[] { "OtherProperty" } };
        yield return new object[] { new ArrayList { typedListParent }, TypeDescriptor.GetProperties(typeof(ITypedListParent)).Cast<PropertyDescriptor>().ToArray(), new string[] { "Property" } };
        yield return new object[] { new IEnumerableWrapper(new object[] { new ITypedListParent() }), TypeDescriptor.GetProperties(typeof(ITypedListParent)).Cast<PropertyDescriptor>().ToArray(), new string[] { "OtherProperty" } };
        yield return new object[] { new IEnumerableWrapper(new object[] { typedListParent }), TypeDescriptor.GetProperties(typeof(ITypedListParent)).Cast<PropertyDescriptor>().ToArray(), new string[] { "Property" } };
    }

    [Theory]
    [MemberData(nameof(GetListItemProperties_Object_PropertyDescriptorArray_TestData))]
    public void ListBindingHelper_GetListItemProperties_InvokeObjectPropertyDescriptorArray_ReturnsExpected(object list, PropertyDescriptor[] listAccessors, string[] expected)
    {
        IEnumerable<PropertyDescriptor> properties = ListBindingHelper.GetListItemProperties(list, listAccessors)?.Cast<PropertyDescriptor>();
        Assert.Equal(expected, properties?.Select(p => p.Name));

        // Null data member.
        properties = ListBindingHelper.GetListItemProperties(list, null, listAccessors)?.Cast<PropertyDescriptor>();
        Assert.Equal(expected, properties?.Select(p => p.Name));

        // Empty data member.
        properties = ListBindingHelper.GetListItemProperties(list, string.Empty, listAccessors)?.Cast<PropertyDescriptor>();
        Assert.Equal(expected, properties?.Select(p => p.Name));
    }

    public static IEnumerable<object[]> GetListItemProperties_Object_String_PropertyDescriptorArray_TestData()
    {
        yield return new object[] { new ListDataClass(), "ListProperty", null, new string[] { "Property" } };
        yield return new object[] { new ListDataClass(), "listproperty", Array.Empty<PropertyDescriptor>(), new string[] { "Property" } };
        yield return new object[] { new MultiListDataClass(), "ParentListProperty", TypeDescriptor.GetProperties(typeof(ListDataClass)).Cast<PropertyDescriptor>().ToArray(), new string[] { "Property" } };
        yield return new object[] { typeof(ListDataClass), "ListProperty", null, new string[] { "Property" } };
        yield return new object[] { typeof(ListDataClass), "listproperty", Array.Empty<PropertyDescriptor>(), new string[] { "Property" } };
        yield return new object[] { typeof(MultiListDataClass), "ParentListProperty", TypeDescriptor.GetProperties(typeof(ListDataClass)).Cast<PropertyDescriptor>().ToArray(), new string[] { "Property" } };
    }

    [Theory]
    [MemberData(nameof(GetListItemProperties_Object_String_PropertyDescriptorArray_TestData))]
    public void ListBindingHelper_GetListItemProperties_InvokeObjectStringPropertyDescriptorArray_ReturnsExpected(object list, string dataMember, PropertyDescriptor[] listAccessors, string[] expected)
    {
        IEnumerable<PropertyDescriptor> properties = ListBindingHelper.GetListItemProperties(list, dataMember, listAccessors)?.Cast<PropertyDescriptor>();
        Assert.Equal(expected, properties?.Select(p => p.Name));
    }

    public static IEnumerable<object[]> GetListItemProperties_NoSuchDataMember_TestData()
    {
        yield return new object[] { null, "name" };
        yield return new object[] { typeof(DataClass), "NoSuchProperty" };
        yield return new object[] { new DataClass(), "NoSuchProperty" };
    }

    [Theory]
    [MemberData(nameof(GetListItemProperties_NoSuchDataMember_TestData))]
    public void ListBindingHelper_GetListItemProperties_NoSuchDataMember_ThrowsArgumentException(object list, string dataMember)
    {
        Assert.Throws<ArgumentException>(() => ListBindingHelper.GetListItemProperties(list, dataMember, null));
    }

    [Fact]
    public void ListBindingHelper_GetListItemProperties_InvalidListAccessors_ThrowsTargetException()
    {
        Assert.Throws<TargetInvocationException>(() => ListBindingHelper.GetListItemProperties(new DataClass(), TypeDescriptor.GetProperties(typeof(ListDataClass)).Cast<PropertyDescriptor>().ToArray()));
        Assert.Throws<TargetInvocationException>(() => ListBindingHelper.GetListItemProperties(new DataClass(), null, TypeDescriptor.GetProperties(typeof(ListDataClass)).Cast<PropertyDescriptor>().ToArray()));
        Assert.Throws<TargetInvocationException>(() => ListBindingHelper.GetListItemProperties(new DataClass(), string.Empty, TypeDescriptor.GetProperties(typeof(ListDataClass)).Cast<PropertyDescriptor>().ToArray()));
    }

    public static IEnumerable<object[]> GetListName_TestData()
    {
        yield return new object[] { null, null, string.Empty };

        Mock<ITypedList> mockTypedList = new(MockBehavior.Strict);
        mockTypedList.Setup(t => t.GetListName(null)).Returns("Name");
        yield return new object[] { mockTypedList.Object, null, "Name" };

        yield return new object[] { 1, null, "Int32" };
        yield return new object[] { typeof(int), null, "Int32" };
        yield return new object[] { 1, Array.Empty<PropertyDescriptor>(), "Int32" };
        yield return new object[] { typeof(int), Array.Empty<PropertyDescriptor>(), "Int32" };
        yield return new object[] { 1, new PropertyDescriptor[] { null }, "Int32" };
        yield return new object[] { typeof(int), new PropertyDescriptor[] { null }, "Int32" };

        yield return new object[] { Array.Empty<int>(), null, "Int32" };
        yield return new object[] { typeof(int[]), null, "Int32" };
        yield return new object[] { Array.Empty<object>(), null, "Object" };
        yield return new object[] { typeof(object[]), null, "Object" };

        yield return new object[] { new List<int>(), null, "Int32" };
        yield return new object[] { typeof(List<int>), null, "Int32" };
        yield return new object[] { new List<object>(), null, "List`1" };
        yield return new object[] { typeof(List<object>), null, "List`1" };

        yield return new object[] { typeof(Array), null, "Array" };
        yield return new object[] { typeof(IList), null, "IList" };
        yield return new object[] { typeof(IList<int>), null, "IList`1" };
        yield return new object[] { typeof(IEnumerable), null, "IEnumerable" };
        yield return new object[] { typeof(IEnumerable), new PropertyDescriptor[] { TypeDescriptor.GetProperties(typeof(DataClass))[0] }, "Int32" };
    }

    [Theory]
    [MemberData(nameof(GetListName_TestData))]
    public void ListBindingHelper_GetListName_Invoke_ReturnsExpected(object list, PropertyDescriptor[] listAccessors, string expected)
    {
        Assert.Equal(expected, ListBindingHelper.GetListName(list, listAccessors));
    }

    private class ClassWithItem
    {
        public string this[int i]
        {
            get => null;
        }
    }

    private class CustomCurrencyManagerProvider : ICurrencyManagerProvider
    {
        public CurrencyManager CurrencyManagerResult { get; set; }

        public CurrencyManager CurrencyManager => CurrencyManagerResult;

        public CurrencyManager GetRelatedCurrencyManager(string dataMember)
        {
            throw new NotImplementedException();
        }

        public int Property { get; set; }
    }

    private class GenericIListSourceClassWithItem : List<int>, IListSource
    {
        public bool ContainsListCollection => throw new NotImplementedException();

        public IList GetList() => throw new NotImplementedException();
    }

    private class NonGenericIListSourceClassWithItem : ArrayList, IListSource
    {
        public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors)
        {
            throw new NotSupportedException();
        }

        public bool ContainsListCollection => throw new NotImplementedException();

        public IList GetList() => throw new NotImplementedException();
    }

    private class ITypedListClassWithItem : ClassWithItem, ITypedList
    {
        public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors)
        {
            throw new NotSupportedException();
        }

        public string GetListName(PropertyDescriptor[] listAccessors)
        {
            throw new NotSupportedException();
        }
    }

    private class ITypedListClassWithCustomNamedItem : ITypedList
    {
        [IndexerName("CustomName")]
        public string this[int i]
        {
            get => null;
        }

        public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors)
        {
            throw new NotSupportedException();
        }

        public string GetListName(PropertyDescriptor[] listAccessors)
        {
            throw new NotSupportedException();
        }
    }

    private class OnlyGenericIListClassWithItem : ClassWithItem, IList<int>
    {
        int IList<int>.this[int index] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public int Count => throw new NotImplementedException();

        public bool IsReadOnly => throw new NotImplementedException();

        public void Add(int item) => throw new NotImplementedException();

        public void Clear() => throw new NotImplementedException();

        public bool Contains(int item) => throw new NotImplementedException();

        public void CopyTo(int[] array, int arrayIndex) => throw new NotImplementedException();

        public IEnumerator<int> GetEnumerator() => throw new NotImplementedException();

        public int IndexOf(int item) => throw new NotImplementedException();

        public void Insert(int index, int item) => throw new NotImplementedException();

        public bool Remove(int item) => throw new NotImplementedException();

        public void RemoveAt(int index) => throw new NotImplementedException();

        IEnumerator IEnumerable.GetEnumerator() => throw new NotImplementedException();
    }

    private class PrivateDefaultConstructor : IListSource
    {
        private PrivateDefaultConstructor() { }

        public bool ContainsListCollection => throw new NotImplementedException();

        public IList GetList() => new int[] { 1 };
    }

    private class NoDefaultConstructor : IListSource
    {
#pragma warning disable IDE0060 // Remove unused parameter
        public NoDefaultConstructor(int i) { }
#pragma warning restore IDE0060

        public bool ContainsListCollection => throw new NotImplementedException();

        public IList GetList() => new int[] { 1 };
    }

    private class ThrowingDefaultConstructor : IListSource
    {
        public ThrowingDefaultConstructor()
        {
            throw new DivideByZeroException();
        }

        public bool ContainsListCollection => throw new NotImplementedException();

        public IList GetList() => new int[] { 1 };
    }

    private class ClassWithIListSource : IListSource
    {
        public bool ContainsListCollection => throw new NotImplementedException();

        public IList GetList() => new int[] { 1 };
    }

    private class NullIListSource : IListSource
    {
        public bool ContainsListCollection => throw new NotImplementedException();

        public IList GetList() => null;
    }

    // Type that wraps an enumerable but ONLY implements non-generic IEnumerable.
    private class IEnumerableWrapper : IEnumerable
    {
        public IEnumerableWrapper(IList list)
        {
            List = list;
        }

        private IList List { get; }

        public IEnumerator GetEnumerator() => List?.GetEnumerator();

        public int Property { get; set; }
    }

    private class NotSupportedIEnumerable : IEnumerable
    {
        public IEnumerator GetEnumerator() => throw new NotSupportedException();
    }

    private class DataClass
    {
        public int Property { get; set; }
    }

    private class ListDataClass
    {
        public List<DataClass> ListProperty { get; set; }
    }

    private class MultiListDataClass
    {
        public List<ListDataClass> ParentListProperty { get; set; }
    }

    private class ITypedListDataClass
    {
        public List<EnumerableITypedListImplementor> ListProperty { get; set; }
    }

    private class ITypedListParent
    {
        public EnumerableITypedListImplementor ListProperty { get; set; }
    }

    private class NonEnumerableITypedListImplementor : ITypedList
    {
        public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors)
        {
            return TypeDescriptor.GetProperties(typeof(DataClass));
        }

        public string GetListName(PropertyDescriptor[] listAccessors)
        {
            throw new NotImplementedException();
        }

        public int OtherProperty { get; set; }
    }

    private class EnumerableITypedListImplementor : ITypedList, IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            return null;
        }

        public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors)
        {
            return TypeDescriptor.GetProperties(typeof(DataClass));
        }

        public string GetListName(PropertyDescriptor[] listAccessors)
        {
            throw new NotImplementedException();
        }

        public int OtherProperty { get; set; }
    }

    private class ICustomTypeDescriptorPropertyClass
    {
        public ICustomTypeDescriptor Property { get; set; }
    }

    private class ICustomTypeDescriptorPropertyDescriptorClass
    {
        public int Property { get; set; }
    }

    private class CustomPropertyDescriptor : PropertyDescriptor
    {
        public CustomPropertyDescriptor(string name, Attribute[] attrs) : base(name, attrs)
        {
        }

        public override Type PropertyType => new CustomTypeDescriptorType();

        public override Type ComponentType => throw new NotImplementedException();

        public override bool IsReadOnly => throw new NotImplementedException();

        public override bool CanResetValue(object component) => throw new NotImplementedException();

        public override object GetValue(object component) => throw new NotImplementedException();

        public override void ResetValue(object component) => throw new NotImplementedException();

        public override void SetValue(object component, object value)
        {
            throw new NotImplementedException();
        }

        public override bool ShouldSerializeValue(object component)
        {
            throw new NotImplementedException();
        }
    }

    public class CustomTypeDescriptorType : Type, ICustomTypeDescriptor
    {
        public override Guid GUID => throw new NotImplementedException();

        public override Module Module => throw new NotImplementedException();

        public override Assembly Assembly => throw new NotImplementedException();

        public override string FullName => throw new NotImplementedException();

        public override string Namespace => throw new NotImplementedException();

        public override string AssemblyQualifiedName => throw new NotImplementedException();

        public override Type BaseType => throw new NotImplementedException();

        public override Type UnderlyingSystemType => throw new NotImplementedException();

        public override string Name => throw new NotImplementedException();

        public AttributeCollection GetAttributes()
        {
            throw new NotImplementedException();
        }

        public string GetClassName()
        {
            throw new NotImplementedException();
        }

        public string GetComponentName()
        {
            throw new NotImplementedException();
        }

        public override ConstructorInfo[] GetConstructors(BindingFlags bindingAttr)
        {
            throw new NotImplementedException();
        }

        public TypeConverter GetConverter()
        {
            throw new NotImplementedException();
        }

        public override object[] GetCustomAttributes(bool inherit)
        {
            throw new NotImplementedException();
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            throw new NotImplementedException();
        }

        public EventDescriptor GetDefaultEvent()
        {
            throw new NotImplementedException();
        }

        public PropertyDescriptor GetDefaultProperty()
        {
            throw new NotImplementedException();
        }

        public object GetEditor(Type editorBaseType)
        {
            throw new NotImplementedException();
        }

        public override Type GetElementType() => throw new NotImplementedException();

        public override EventInfo GetEvent(string name, BindingFlags bindingAttr)
        {
            throw new NotImplementedException();
        }

        public override EventInfo[] GetEvents(BindingFlags bindingAttr)
        {
            throw new NotImplementedException();
        }

        public EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            throw new NotImplementedException();
        }

        public override FieldInfo GetField(string name, BindingFlags bindingAttr)
        {
            throw new NotImplementedException();
        }

        public override FieldInfo[] GetFields(BindingFlags bindingAttr)
        {
            throw new NotImplementedException();
        }

        public override Type GetInterface(string name, bool ignoreCase)
        {
            throw new NotImplementedException();
        }

        public override Type[] GetInterfaces() => throw new NotImplementedException();

        public override MemberInfo[] GetMembers(BindingFlags bindingAttr)
        {
            throw new NotImplementedException();
        }

        public override MethodInfo[] GetMethods(BindingFlags bindingAttr)
        {
            throw new NotImplementedException();
        }

        public override Type GetNestedType(string name, BindingFlags bindingAttr)
        {
            throw new NotImplementedException();
        }

        public override Type[] GetNestedTypes(BindingFlags bindingAttr)
        {
            throw new NotImplementedException();
        }

        public override PropertyInfo[] GetProperties(BindingFlags bindingAttr)
        {
            throw new NotImplementedException();
        }

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            throw new NotImplementedException();
        }

        public object GetPropertyOwner(PropertyDescriptor pd)
        {
            throw new NotImplementedException();
        }

        public override object InvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target, object[] args, ParameterModifier[] modifiers, CultureInfo culture, string[] namedParameters)
        {
            throw new NotImplementedException();
        }

        public override bool IsDefined(Type attributeType, bool inherit) => throw new NotImplementedException();

        protected override TypeAttributes GetAttributeFlagsImpl() => throw new NotImplementedException();

        protected override ConstructorInfo GetConstructorImpl(BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
        {
            throw new NotImplementedException();
        }

        protected override MethodInfo GetMethodImpl(string name, BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
        {
            throw new NotImplementedException();
        }

        protected override PropertyInfo GetPropertyImpl(string name, BindingFlags bindingAttr, Binder binder, Type returnType, Type[] types, ParameterModifier[] modifiers)
        {
            throw new NotImplementedException();
        }

        protected override bool HasElementTypeImpl() => throw new NotImplementedException();

        protected override bool IsArrayImpl() => throw new NotImplementedException();

        protected override bool IsByRefImpl() => throw new NotImplementedException();

        protected override bool IsCOMObjectImpl() => throw new NotImplementedException();

        protected override bool IsPointerImpl() => throw new NotImplementedException();

        protected override bool IsPrimitiveImpl() => throw new NotImplementedException();

        EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
        {
            throw new NotImplementedException();
        }

        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
        {
            throw new NotImplementedException();
        }
    }
}
