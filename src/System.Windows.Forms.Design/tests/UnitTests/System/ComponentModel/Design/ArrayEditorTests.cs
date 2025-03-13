// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using System.Collections;
using System.Drawing.Design;
using System.Reflection;
using System.Windows.Forms.TestUtilities;

namespace System.ComponentModel.Design.Tests;

public class ArrayEditorTests
{
    [Theory]
    [InlineData(typeof(string[]), typeof(string))]
    [InlineData(typeof(int[]), typeof(int))]
    public void ArrayEditor_Ctor_Type(Type type, Type expectedItemType)
    {
        SubArrayEditor editor = new(type);
        Assert.Equal(expectedItemType, editor.CollectionItemType);
        Assert.Same(editor.CollectionItemType, editor.CollectionItemType);
        Assert.Equal(type, editor.CollectionType);
        Assert.Null(editor.Context);
        Assert.Equal("net.ComponentModel.CollectionEditor", editor.HelpTopic);
        Assert.False(editor.IsDropDownResizable);
        Assert.Equal([expectedItemType], editor.NewItemTypes);
    }

    [Theory]
    [InlineData(typeof(object))]
    [InlineData(typeof(string))]
    [InlineData(typeof(IList<int>))]
    [InlineData(typeof(IList))]
    [InlineData(typeof(ClassWithItem))]
    [InlineData(typeof(ClassWithPrivateItem))]
    [InlineData(typeof(ClassWithStaticItem))]
    [InlineData(typeof(ClassWithItems))]
    [InlineData(typeof(ClassWithPrivateItems))]
    [InlineData(typeof(ClassWithStaticItems))]
    public void ArrayEditor_Ctor_Invalid_Type(Type type)
    {
        SubArrayEditor editor = new(type);
        Assert.Throws<InvalidOperationException>(() => editor.CollectionItemType);
    }

    [Fact]
    public void ArrayEditor_Ctor_NullType_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new SubArrayEditor(null));
    }

    public static IEnumerable<object[]> CanRemoveInstance_TestData()
    {
        yield return new object[] { "some string" };
        yield return new object[] { 123 };
        yield return new object[] { null };
        yield return new object[] { new Component() };
    }

    [Theory]
    [MemberData(nameof(CanRemoveInstance_TestData))]
    public void ArrayEditor_CanRemoveInstance_Invoke_ReturnsExpected(object value)
    {
        SubArrayEditor editor = new(typeof(string[]));
        Assert.True(editor.CanRemoveInstance(value));
    }

    public static IEnumerable<object[]> CanRemoveInstance_InheritanceAttribute_TestData()
    {
        yield return new object[] { new InheritanceAttribute(InheritanceLevel.Inherited - 1), false };
        yield return new object[] { new InheritanceAttribute(InheritanceLevel.Inherited), false };
        yield return new object[] { new InheritanceAttribute(InheritanceLevel.InheritedReadOnly), false };
        yield return new object[] { new InheritanceAttribute(InheritanceLevel.NotInherited), true };
        yield return new object[] { new InheritanceAttribute(InheritanceLevel.NotInherited + 1), false };
    }

    [Theory]
    [MemberData(nameof(CanRemoveInstance_InheritanceAttribute_TestData))]
    public void ArrayEditor_CanRemoveInstance_InheritanceAttribute_ReturnsExpected(InheritanceAttribute attribute, bool expected)
    {
        using Component component = new();
        TypeDescriptor.AddAttributes(component, attribute);
        SubArrayEditor editor = new(typeof(string[]));
        Assert.Equal(expected, editor.CanRemoveInstance(component));
    }

    [Fact]
    public void ArrayEditor_CanSelectMultipleInstances_Invoke_ReturnsFalse()
    {
        SubArrayEditor editor = new(typeof(string[]));
        Assert.True(editor.CanSelectMultipleInstances());
    }

    public static IEnumerable<object[]> GetDisplayText_TestData()
    {
        yield return new object[] { typeof(int), null, string.Empty };
        yield return new object[] { typeof(int), "", "String" };
        yield return new object[] { typeof(int), "value", "value" };
        yield return new object[] { typeof(int), 1, "1" };
        yield return new object[] { typeof(int), new ClassWithStringDefaultProperty { DefaultProperty = "CustomName" }, "ClassWithStringDefaultProperty" };

        yield return new object[] { typeof(ClassWithStringDefaultProperty), null, string.Empty };
        yield return new object[] { typeof(ClassWithStringDefaultProperty), new ClassWithStringDefaultProperty { DefaultProperty = "CustomName" }, "CustomName" };
        yield return new object[] { typeof(ClassWithStringDefaultProperty), new ClassWithStringDefaultProperty { DefaultProperty = string.Empty }, "ClassWithStringDefaultProperty" };
        yield return new object[] { typeof(ClassWithStringDefaultProperty), new ClassWithStringDefaultProperty { DefaultProperty = null }, "ClassWithStringDefaultProperty" };
        yield return new object[] { typeof(ClassWithNonStringDefaultProperty), new ClassWithNonStringDefaultProperty { DefaultProperty = 1 }, "ClassWithNonStringDefaultProperty" };
        yield return new object[] { typeof(ClassWithNoSuchDefaultProperty), new ClassWithNoSuchDefaultProperty { DefaultProperty = "CustomName" }, "ClassWithNoSuchDefaultProperty" };
        yield return new object[] { typeof(List<ClassWithStringDefaultProperty>), new ClassWithStringDefaultProperty { DefaultProperty = "CustomName" }, "ClassWithStringDefaultProperty" };
    }

    [Theory]
    [MemberData(nameof(GetDisplayText_TestData))]
    public void ArrayEditor_GetDisplayText_Invoke_ReturnsExpected(Type type, object value, string expected)
    {
        SubArrayEditor editor = new(type);
        Assert.Equal(expected, editor.GetDisplayText(value));
    }

    [Fact]
    public void ArrayEditor_GetDisplayText_ValueDoesNotMatchCollectionType_ThrowsTargetException()
    {
        SubArrayEditor editor = new(typeof(ClassWithStringDefaultProperty));
        TargetInvocationException ex = Assert.Throws<TargetInvocationException>(() => editor.GetDisplayText(new ClassWithNonStringDefaultProperty()));
        Assert.IsType<TargetException>(ex.InnerException);
    }

    [Theory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetITypeDescriptorContextTestData))]
    public void ArrayEditor_GetEditStyle_Invoke_ReturnsModal(ITypeDescriptorContext context)
    {
        ArrayEditor editor = new(typeof(string[]));
        Assert.Equal(UITypeEditorEditStyle.Modal, editor.GetEditStyle(context));
    }

    public static IEnumerable<object[]> GetItems_TestData()
    {
        yield return new object[] { null, Array.Empty<object>() };
        yield return new object[] { new(), Array.Empty<object>() };
        yield return new object[] { new int[] { 1, 2, 3 }, new object[] { 1, 2, 3, } };
        yield return new object[] { new ArrayList { 1, 2, 3 }, Array.Empty<object>() };
    }

    [Theory]
    [MemberData(nameof(GetItems_TestData))]
    public void ArrayEditor_GetItems_Invoke_ReturnsExpected(object editValue, object[] expected)
    {
        SubArrayEditor editor = new(typeof(string[]));
        object[] items = editor.GetItems(editValue);
        Assert.Equal(expected, items);
        Assert.IsType(expected.GetType(), items);
        Assert.NotSame(editValue, items);
    }

    [Theory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetITypeDescriptorContextTestData))]
    public void ArrayEditor_GetPaintValueSupported_Invoke_ReturnsFalse(ITypeDescriptorContext context)
    {
        ArrayEditor editor = new(typeof(string[]));
        Assert.False(editor.GetPaintValueSupported(context));
    }

    public static IEnumerable<object[]> SetItems_Array_TestData()
    {
        yield return new object[] { null, Array.Empty<object>(), Array.Empty<object>() };
        yield return new object[] { null, new object[] { 1, 2, 3 }, new object[] { 1, 2, 3 } };
        yield return new object[] { new object[] { 4, 5 }, new object[] { 1, 2, 3 }, new object[] { 1, 2, 3 } };
        yield return new object[] { new object[] { 4, 5, 6 }, new object[] { 1, 2, 3 }, new object[] { 1, 2, 3 } };
        yield return new object[] { new object[] { 4, 5, 6, 7 }, new object[] { 1, 2, 3 }, new object[] { 1, 2, 3 } };
    }

    [Theory]
    [MemberData(nameof(SetItems_Array_TestData))]
    public void ArrayEditor_SetItems_InvokeArray_ReturnsCopy(object editValue, object[] value, int[] expected)
    {
        SubArrayEditor editor = new(typeof(int[]));
        int[] result = Assert.IsType<int[]>(editor.SetItems(editValue, value));
        Assert.NotSame(value, expected);
        Assert.Equal(expected, result);
    }

    public static IEnumerable<object[]> SetItems_NonArray_TestData()
    {
        object editValue = new();
        yield return new object[] { editValue, null, editValue };
        yield return new object[] { editValue, Array.Empty<object>(), editValue };
        yield return new object[] { Array.Empty<object>(), null, null };
        yield return new object[] { null, null, null };
    }

    [Theory]
    [MemberData(nameof(SetItems_NonArray_TestData))]
    public void ArrayEditor_SetItems_InvokeNonArrayValue_ReturnsExpected(object editValue, object[] value, object expected)
    {
        SubArrayEditor editor = new(typeof(int[]));
        Assert.Same(expected, editor.SetItems(editValue, value));
    }

    private class SubArrayEditor : ArrayEditor
    {
        public SubArrayEditor(Type type) : base(type)
        {
        }

        public new Type CollectionItemType => base.CollectionItemType;

        public new Type CollectionType => base.CollectionType;

        public new ITypeDescriptorContext Context => base.Context;

        public new string HelpTopic => base.HelpTopic;

        public new Type[] NewItemTypes => base.NewItemTypes;

        public new bool CanRemoveInstance(object value) => base.CanRemoveInstance(value);

        public new bool CanSelectMultipleInstances() => base.CanSelectMultipleInstances();

        public new string GetDisplayText(object value) => base.GetDisplayText(value);

        public new object[] GetItems(object editValue) => base.GetItems(editValue);

        public new object SetItems(object editValue, object[] value) => base.SetItems(editValue, value);
    }

    private class ClassWithItem
    {
        public int Item { get; set; }
    }

#pragma warning disable IDE0051 // Remove unused private members
#pragma warning disable CA1052 // Static holder types should be Static or NotInheritable
    private class ClassWithPrivateItem
    {
        private int Item { get; set; }
    }

    private class ClassWithStaticItem
    {
        public static int Item { get; set; }
    }

    private class ClassWithItems
    {
        public int Items { get; set; }
    }

    private class ClassWithPrivateItems
    {
        private int Items { get; set; }
    }

    private class ClassWithStaticItems
    {
        public static int Items { get; set; }
    }

    private class ClassWithStringName
    {
        public string Name { get; set; }

        public override string ToString() => nameof(ClassWithStringName);
    }

    private class ClassWithNonStringName
    {
        public int Name { get; set; }

        public override string ToString() => nameof(ClassWithNonStringName);
    }

    private class ClassWithNullToString
    {
        public int Name { get; set; }

        public override string ToString() => null;
    }

    [DefaultProperty(nameof(DefaultProperty))]
    private class ClassWithStringDefaultProperty
    {
        public string DefaultProperty { get; set; }

        public override string ToString() => nameof(ClassWithStringDefaultProperty);
    }

    [DefaultProperty(nameof(DefaultProperty))]
    private class ClassWithNonStringDefaultProperty
    {
        public int DefaultProperty { get; set; }

        public override string ToString() => nameof(ClassWithNonStringDefaultProperty);
    }

    [DefaultProperty("NoSuchProperty")]
    private class ClassWithNoSuchDefaultProperty
    {
        public string DefaultProperty { get; set; }

        public override string ToString() => nameof(ClassWithNoSuchDefaultProperty);
    }

#pragma warning restore IDE0051
#pragma warning restore CA1052
}
