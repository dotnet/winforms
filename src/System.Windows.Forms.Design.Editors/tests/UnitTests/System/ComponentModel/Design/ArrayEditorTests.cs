// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using System.Drawing.Design;
using System.Globalization;
using System.Reflection;
using WinForms.Common.Tests;
using Xunit;

namespace System.ComponentModel.Design.Tests
{
    public class ArrayEditorTests : IClassFixture<ThreadExceptionFixture>
    {
        [Theory]
        [InlineData(typeof(object), null)]
        [InlineData(typeof(string), null)]
        [InlineData(typeof(int[]), typeof(int))]
        [InlineData(typeof(IList<int>), null)]
        [InlineData(typeof(IList), null)]
        [InlineData(typeof(ClassWithItem), null)]
        [InlineData(typeof(ClassWithPrivateItem), null)]
        [InlineData(typeof(ClassWithStaticItem), null)]
        [InlineData(typeof(ClassWithItems), null)]
        [InlineData(typeof(ClassWithPrivateItems), null)]
        [InlineData(typeof(ClassWithStaticItems), null)]
        public void ArrayEditor_Ctor_Type(Type type, Type expectedItemType)
        {
            var editor = new SubArrayEditor(type);
            Assert.Equal(expectedItemType, editor.CollectionItemType);
            Assert.Same(editor.CollectionItemType, editor.CollectionItemType);
            Assert.Equal(type, editor.CollectionType);
            Assert.Null(editor.Context);
            Assert.Equal("net.ComponentModel.CollectionEditor", editor.HelpTopic);
            Assert.False(editor.IsDropDownResizable);
            Assert.Equal(new Type[] { expectedItemType }, editor.NewItemTypes);
        }

        [Fact]
        public void ArrayEditor_Ctor_NullType()
        {
            var editor = new SubArrayEditor(null);
            Assert.Null(editor.CollectionItemType);
            Assert.Null(editor.CollectionType);
            Assert.Null(editor.Context);
            Assert.Equal("net.ComponentModel.CollectionEditor", editor.HelpTopic);
            Assert.False(editor.IsDropDownResizable);
            Assert.Equal(new Type[] { null }, editor.NewItemTypes);
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
            var editor = new SubArrayEditor(null);
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
            var component = new Component();
            TypeDescriptor.AddAttributes(component, attribute);
            var editor = new SubArrayEditor(null);
            Assert.Equal(expected, editor.CanRemoveInstance(component));
        }

        [Fact]
        public void ArrayEditor_CanSelectMultipleInstances_Invoke_ReturnsFalse()
        {
            var editor = new SubArrayEditor(null);
            Assert.True(editor.CanSelectMultipleInstances());
        }

        public static IEnumerable<Object[]> GetDisplayText_TestData()
        {
            yield return new object[] { null, null, string.Empty };
            yield return new object[] { null, string.Empty, "String" };
            yield return new object[] { null, "string", "string" };

            yield return new object[] { null, new ClassWithStringName { Name = "CustomName" }, "CustomName" };
            yield return new object[] { null, new ClassWithStringName { Name = string.Empty }, "ClassWithStringName" };
            yield return new object[] { null, new ClassWithStringName { Name = null }, "ClassWithStringName" };
            yield return new object[] { null, new ClassWithNonStringName { Name = 1 }, "ClassWithNonStringName" };
            yield return new object[] { null, new ClassWithNullToString(), "ClassWithNullToString" };

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
            var editor = new SubArrayEditor(type);
            Assert.Equal(expected, editor.GetDisplayText(value));
        }

        [Fact]
        public void ArrayEditor_GetDisplayText_ValueDoesntMatchCollectionType_ThrowsTargetException()
        {
            var editor = new SubArrayEditor(typeof(ClassWithStringDefaultProperty));
            TargetInvocationException ex = Assert.Throws<TargetInvocationException>(() => editor.GetDisplayText(new ClassWithNonStringDefaultProperty()));
            Assert.IsType<TargetException>(ex.InnerException);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetITypeDescriptorContextTestData))]
        public void ArrayEditor_GetEditStyle_Invoke_ReturnsModal(ITypeDescriptorContext context)
        {
            var editor = new ArrayEditor(null);
            Assert.Equal(UITypeEditorEditStyle.Modal, editor.GetEditStyle(context));
        }

        public static IEnumerable<object[]> GetItems_TestData()
        {
            yield return new object[] { null, Array.Empty<object>() };
            yield return new object[] { new object(), Array.Empty<object>() };
            yield return new object[] { new int[] { 1, 2, 3 }, new object[] { 1, 2, 3, } };
            yield return new object[] { new ArrayList { 1, 2, 3 }, Array.Empty<object>() };
        }

        [Theory]
        [MemberData(nameof(GetItems_TestData))]
        public void ArrayEditor_GetItems_Invoke_ReturnsExpected(object editValue, object[] expected)
        {
            var editor = new SubArrayEditor(null);
            object[] items = editor.GetItems(editValue);
            Assert.Equal(expected, items);
            Assert.IsType(expected.GetType(), items);
            Assert.NotSame(editValue, items);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetITypeDescriptorContextTestData))]
        public void ArrayEditor_GetPaintValueSupported_Invoke_ReturnsFalse(ITypeDescriptorContext context)
        {
            var editor = new ArrayEditor(null);
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
            var editor = new SubArrayEditor(typeof(int[]));
            int[] result = Assert.IsType<int[]>(editor.SetItems(editValue, value));
            Assert.NotSame(value, expected);
            Assert.Equal(expected, result);
        }

        public static IEnumerable<object[]> SetItems_NonArray_TestData()
        {
            var editValue = new object();
            yield return new object[] { editValue, null, editValue };
            yield return new object[] { editValue, Array.Empty<object>(), editValue };
            yield return new object[] { Array.Empty<object>(), null, null };
            yield return new object[] { null, null, null };
        }

        [Theory]
        [MemberData(nameof(SetItems_NonArray_TestData))]
        public void ArrayEditor_SetItems_InvokeNonArrayValue_ReturnsExpected(object editValue, object[] value, object expected)
        {
            var editor = new SubArrayEditor(typeof(int[]));
            Assert.Same(expected, editor.SetItems(editValue, value));
        }

        [Theory]
        [InlineData(typeof(object))]
        [InlineData(null)]
        public void ArrayEditor_SetItems_NullCollectionItemType_ThrowsArgumentNullException(Type type)
        {
            var editor = new SubArrayEditor(type);
            Assert.Null(editor.CollectionItemType);
            Assert.Throws<ArgumentNullException>("elementType", () => editor.SetItems(null, Array.Empty<object>()));
            Assert.Throws<ArgumentNullException>("elementType", () => editor.SetItems(Array.Empty<object>(), Array.Empty<object>()));
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

        [DefaultProperty(nameof(ClassWithStringDefaultProperty.DefaultProperty))]
        private class ClassWithStringDefaultProperty
        {
            public string DefaultProperty { get; set; }

            public override string ToString() => nameof(ClassWithStringDefaultProperty);
        }

        [DefaultProperty(nameof(ClassWithNonStringDefaultProperty.DefaultProperty))]
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
    }
}
