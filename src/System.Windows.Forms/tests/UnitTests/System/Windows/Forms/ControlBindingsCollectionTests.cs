// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Globalization;
using Moq;
using WinForms.Common.Tests;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class ControlBindingsCollectionTests : IClassFixture<ThreadExceptionFixture>
    {
        public static IEnumerable<object[]> Ctor_IBindableComponent_TestData()
        {
            var mockBindableComponent = new Mock<IBindableComponent>(MockBehavior.Strict);
            mockBindableComponent.Setup(c => c.Dispose());
            yield return new object[] { mockBindableComponent.Object, null };

            var control = new Control();
            yield return new object[] { control, control };
            yield return new object[] { null, null };
        }

        [WinFormsTheory]
        [MemberData(nameof(Ctor_IBindableComponent_TestData))]
        public void Ctor_IBindableComponent(IBindableComponent control, Control expectedControl)
        {
            var collection = new ControlBindingsCollection(control);
            Assert.Same(control, collection.BindableComponent);
            Assert.Same(expectedControl, collection.Control);
            Assert.Equal(DataSourceUpdateMode.OnValidation, collection.DefaultDataSourceUpdateMode);
            Assert.Empty(collection);
        }

        [WinFormsTheory]
        [MemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(DataSourceUpdateMode), MemberType = typeof(CommonTestHelper))]
        [MemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(DataSourceUpdateMode), MemberType = typeof(CommonTestHelper))]
        public void DefaultDataSourceUpdateMode_Set_GetReturnsExpected(DataSourceUpdateMode value)
        {
            using var control = new Control();
            var collection = new ControlBindingsCollection(control)
            {
                DefaultDataSourceUpdateMode = value
            };
            Assert.Equal(value, collection.DefaultDataSourceUpdateMode);
        }

        public static IEnumerable<object[]> Add_Binding_TestData()
        {
            yield return new object[] { new Binding(null, new object(), "dataMember") };
            yield return new object[] { new Binding("", new object(), "dataMember") };
            yield return new object[] { new Binding(nameof(Control.Text), new object(), "dataMember") };
        }

        [WinFormsTheory]
        [MemberData(nameof(Add_Binding_TestData))]
        public void Add_Binding_Success(Binding binding)
        {
            using var control = new Control();
            var collection = new ControlBindingsCollection(control)
            {
                binding
            };
            Assert.Same(binding, Assert.Single(collection));
            Assert.Same(control, binding.BindableComponent);
        }

        [WinFormsFact]
        public void Add_DuplicateBinding_Success()
        {
            using var control = new Control();
            var collection = new ControlBindingsCollection(control);
            var binding1 = new Binding("", 1, "dataMember");
            var binding2 = new Binding(null, 1, "dataMember");
            var binding3 = new Binding(nameof(Control.Text), 1, "dataMember");
            collection.Add(binding1);
            collection.Add(binding2);
            collection.Add(binding3);

            var binding4 = new Binding(nameof(Control.Text), 1, "dataMember");
            collection.Add(binding4);
            Assert.Equal(4, collection.Count);
            Assert.Same(binding1, collection[0]);
            Assert.Same(binding2, collection[1]);
            Assert.Same(binding3, collection[2]);
            Assert.Same(binding4, collection[3]);
        }

        [WinFormsFact]
        public void Add_FormControl_Success()
        {
            using var control = new Form();
            var collection = new ControlBindingsCollection(control);
            var binding = new Binding(nameof(Control.Text), 1, "dataMember");
            collection.Add(binding);
            Assert.Same(binding, Assert.Single(collection));
        }

        [WinFormsFact]
        public void Add_ControlBindings_Success()
        {
            using var control = new SubControl();
            ControlBindingsCollection collection = control.DataBindings;
            var binding1 = new Binding(nameof(SubControl.Text), 1, "dataMember");
            var binding2 = new Binding(nameof(SubControl.AccessibleRole), 1, "dataMember");
            var binding3 = new Binding(null, 1, "dataMember");
            var binding4 = new Binding("", 1, "dataMember");
            var binding5 = new Binding(nameof(SubControl.text), 1, "dataMember");

            collection.Add(binding1);
            collection.Add(binding2);
            collection.Add(binding3);
            collection.Add(binding4);
            collection.Add(binding5);
            Assert.Equal(5, collection.Count);
            Assert.Same(binding1, collection[0]);
            Assert.Same(binding2, collection[1]);
            Assert.Same(binding3, collection[2]);
            Assert.Same(binding4, collection[3]);
            Assert.Same(binding5, collection[4]);
        }

        [WinFormsFact]
        public void Add_StringObjectString_Success()
        {
            using var control = new Control();
            var collection = new ControlBindingsCollection(control);
            Binding binding = collection.Add(nameof(Control.Text), 1, "dataMember");
            Assert.Same(binding, Assert.Single(collection));
            Assert.Same(control, binding.BindableComponent);
            Assert.Equal(nameof(Control.Text), binding.PropertyName);
            Assert.Equal(1, binding.DataSource);
            Assert.False(binding.FormattingEnabled);
            Assert.Equal(DataSourceUpdateMode.OnValidation, binding.DataSourceUpdateMode);
            Assert.Null(binding.NullValue);
            Assert.Empty(binding.FormatString);
            Assert.Null(binding.FormatInfo);
        }

        [WinFormsFact]
        public void Add_StringObjectStringBool_Success()
        {
            using var control = new Control();
            var collection = new ControlBindingsCollection(control);
            Binding binding = collection.Add(nameof(Control.Text), 1, "dataMember", true);
            Assert.Same(binding, Assert.Single(collection));
            Assert.Same(control, binding.BindableComponent);
            Assert.Equal(nameof(Control.Text), binding.PropertyName);
            Assert.Equal(1, binding.DataSource);
            Assert.True(binding.FormattingEnabled);
            Assert.Equal(DataSourceUpdateMode.OnValidation, binding.DataSourceUpdateMode);
            Assert.Null(binding.NullValue);
            Assert.Empty(binding.FormatString);
            Assert.Null(binding.FormatInfo);
        }

        [WinFormsFact]
        public void Add_StringObjectStringBoolDataSourceUpdateMode_Success()
        {
            using var control = new Control();
            var collection = new ControlBindingsCollection(control);
            Binding binding = collection.Add(nameof(Control.Text), 1, "dataMember", true, DataSourceUpdateMode.OnPropertyChanged);
            Assert.Same(binding, Assert.Single(collection));
            Assert.Same(control, binding.BindableComponent);
            Assert.Equal(nameof(Control.Text), binding.PropertyName);
            Assert.Equal(1, binding.DataSource);
            Assert.True(binding.FormattingEnabled);
            Assert.Equal(DataSourceUpdateMode.OnPropertyChanged, binding.DataSourceUpdateMode);
            Assert.Null(binding.NullValue);
            Assert.Empty(binding.FormatString);
            Assert.Null(binding.FormatInfo);
        }

        [WinFormsFact]
        public void Add_StringObjectStringBoolDataSourceUpdateModeObject_Success()
        {
            using var control = new Control();
            var collection = new ControlBindingsCollection(control);
            Binding binding = collection.Add(nameof(Control.Text), 1, "dataMember", true, DataSourceUpdateMode.OnPropertyChanged, "null");
            Assert.Same(binding, Assert.Single(collection));
            Assert.Same(control, binding.BindableComponent);
            Assert.Equal(nameof(Control.Text), binding.PropertyName);
            Assert.Equal(1, binding.DataSource);
            Assert.True(binding.FormattingEnabled);
            Assert.Equal(DataSourceUpdateMode.OnPropertyChanged, binding.DataSourceUpdateMode);
            Assert.Equal("null", binding.NullValue);
            Assert.Empty(binding.FormatString);
            Assert.Null(binding.FormatInfo);
        }

        [WinFormsFact]
        public void Add_StringObjectStringBoolDataSourceUpdateModeObjectString_Success()
        {
            using var control = new Control();
            var collection = new ControlBindingsCollection(control);
            Binding binding = collection.Add(nameof(Control.Text), 1, "dataMember", true, DataSourceUpdateMode.OnPropertyChanged, "null", "formatString");
            Assert.Same(binding, Assert.Single(collection));
            Assert.Same(control, binding.BindableComponent);
            Assert.Equal(nameof(Control.Text), binding.PropertyName);
            Assert.Equal(1, binding.DataSource);
            Assert.True(binding.FormattingEnabled);
            Assert.Equal(DataSourceUpdateMode.OnPropertyChanged, binding.DataSourceUpdateMode);
            Assert.Equal("null", binding.NullValue);
            Assert.Equal("formatString", binding.FormatString);
            Assert.Null(binding.FormatInfo);
        }

        [WinFormsFact]
        public void Add_StringObjectStringBoolDataSourceUpdateModeObjectStringIFormatProvider_Success()
        {
            using var control = new Control();
            var collection = new ControlBindingsCollection(control);
            Binding binding = collection.Add(nameof(Control.Text), 1, "dataMember", true, DataSourceUpdateMode.OnPropertyChanged, "null", "formatString", CultureInfo.CurrentCulture);
            Assert.Same(binding, Assert.Single(collection));
            Assert.Same(control, binding.BindableComponent);
            Assert.Equal(nameof(Control.Text), binding.PropertyName);
            Assert.Equal(1, binding.DataSource);
            Assert.True(binding.FormattingEnabled);
            Assert.Equal(DataSourceUpdateMode.OnPropertyChanged, binding.DataSourceUpdateMode);
            Assert.Equal("null", binding.NullValue);
            Assert.Equal("formatString", binding.FormatString);
            Assert.Equal(CultureInfo.CurrentCulture, binding.FormatInfo);
        }

        [WinFormsFact]
        public void Add_NullDataSource_ThrowsArgumentNullException()
        {
            using var control = new Control();
            var collection = new ControlBindingsCollection(control);
            Assert.Throws<ArgumentNullException>("dataSource", () => collection.Add("propertyName", null, "dataMember"));
            Assert.Throws<ArgumentNullException>("dataSource", () => collection.Add("propertyName", null, "dataMember", true));
            Assert.Throws<ArgumentNullException>("dataSource", () => collection.Add("propertyName", null, "dataMember", true, DataSourceUpdateMode.OnPropertyChanged));
            Assert.Throws<ArgumentNullException>("dataSource", () => collection.Add("propertyName", null, "dataMember", true, DataSourceUpdateMode.OnPropertyChanged, "null"));
            Assert.Throws<ArgumentNullException>("dataSource", () => collection.Add("propertyName", null, "dataMember", true, DataSourceUpdateMode.OnPropertyChanged, "null", "formatString"));
            Assert.Throws<ArgumentNullException>("dataSource", () => collection.Add("propertyName", null, "dataMember", true, DataSourceUpdateMode.OnPropertyChanged, "null", "formatString", CultureInfo.CurrentCulture));
        }

        [WinFormsFact]
        public void Add_InvalidBinding_ThrowsArgumentException()
        {
            using var control = new Control();
            var collection = new ControlBindingsCollection(control);
            Assert.Throws<ArgumentException>("PropertyName", () => collection.Add(new Binding("NoSuchProperty", new object(), "dataMember")));
            Assert.Throws<ArgumentException>("PropertyName", () => collection.Add("NoSuchProperty", new object(), "dataMember"));
            Assert.Throws<ArgumentException>("PropertyName", () => collection.Add("NoSuchProperty", new object(), "dataMember", true));
            Assert.Throws<ArgumentException>("PropertyName", () => collection.Add("NoSuchProperty", new object(), "dataMember", true, DataSourceUpdateMode.OnPropertyChanged));
            Assert.Throws<ArgumentException>("PropertyName", () => collection.Add("NoSuchProperty", new object(), "dataMember", true, DataSourceUpdateMode.OnPropertyChanged, "null"));
            Assert.Throws<ArgumentException>("PropertyName", () => collection.Add("NoSuchProperty", new object(), "dataMember", true, DataSourceUpdateMode.OnPropertyChanged, "null", "formatString"));
            Assert.Throws<ArgumentException>("PropertyName", () => collection.Add("NoSuchProperty", new object(), "dataMember", true, DataSourceUpdateMode.OnPropertyChanged, "null", "formatString", CultureInfo.CurrentCulture));
        }

        [WinFormsFact]
        public void Add_DuplicateBinding_ThrowsArgumentException()
        {
            using var control = new Control();
            ControlBindingsCollection collection = control.DataBindings;
            var binding = new Binding(nameof(Control.Text), new object(), "dataMember");
            collection.Add(binding);

            Assert.Throws<ArgumentException>("binding", () => collection.Add(new Binding(nameof(Control.Text), new object(), "dataMember")));
            Assert.Throws<ArgumentException>("binding", () => collection.Add(nameof(Control.Text), new object(), "dataMember"));
            Assert.Throws<ArgumentException>("binding", () => collection.Add(nameof(Control.Text), new object(), "dataMember", true));
            Assert.Throws<ArgumentException>("binding", () => collection.Add(nameof(Control.Text), new object(), "dataMember", true, DataSourceUpdateMode.OnPropertyChanged));
            Assert.Throws<ArgumentException>("binding", () => collection.Add(nameof(Control.Text), new object(), "dataMember", true, DataSourceUpdateMode.OnPropertyChanged, "null"));
            Assert.Throws<ArgumentException>("binding", () => collection.Add(nameof(Control.Text), new object(), "dataMember", true, DataSourceUpdateMode.OnPropertyChanged, "null", "formatString"));
            Assert.Throws<ArgumentException>("binding", () => collection.Add(nameof(Control.Text), new object(), "dataMember", true, DataSourceUpdateMode.OnPropertyChanged, "null", "formatString", CultureInfo.CurrentCulture));
        }

        [WinFormsFact]
        public void Add_NullDataBinding_ThrowsArgumentNullException()
        {
            using var control = new Control();
            var collection = new ControlBindingsCollection(control);
            Assert.Throws<ArgumentNullException>("dataBinding", () => collection.Add(null));
        }

        [WinFormsFact]
        public void Add_AddAlreadyInSameManager_ThrowsArgumentException()
        {
            using var control = new Control();
            var collection = new ControlBindingsCollection(control);
            var binding = new Binding(null, new object(), "member");

            collection.Add(binding);
            Assert.Throws<ArgumentException>("dataBinding", () => collection.Add(binding));
        }

        [WinFormsFact]
        public void Add_AlreadyInDifferentManager_ThrowsArgumentException()
        {
            using var control1 = new Control();
            using var control2 = new Control();
            var collection1 = new ControlBindingsCollection(control1);
            var collection2 = new ControlBindingsCollection(control2);
            var binding = new Binding(null, new object(), "member");

            collection1.Add(binding);
            Assert.Throws<ArgumentException>("dataBinding", () => collection2.Add(binding));
        }

        [WinFormsFact]
        public void Clear_Invoke_Success()
        {
            using var control = new Control();
            var collection = new ControlBindingsCollection(control);
            var binding = new Binding(null, new object(), "member");

            collection.Add(binding);
            Assert.Same(binding, Assert.Single(collection));
            Assert.Same(control, binding.BindableComponent);

            collection.Clear();
            Assert.Empty(collection);
            Assert.Null(binding.BindableComponent);

            // Clear again.
            collection.Clear();
            Assert.Empty(collection);
        }

        [WinFormsFact]
        public void Remove_Invoke_Success()
        {
            using var control = new Control();
            var collection = new ControlBindingsCollection(control);
            var binding = new Binding(null, new object(), "member");

            collection.Add(binding);
            Assert.Same(binding, Assert.Single(collection));
            Assert.Same(control, binding.BindableComponent);

            collection.Remove(binding);
            Assert.Empty(collection);
            Assert.Null(binding.BindableComponent);
        }

        [WinFormsFact]
        public void Remove_NullDataBinding_ThrowsArgumentNullException()
        {
            using var control = new Control();
            var collection = new ControlBindingsCollection(control);
            var binding = new Binding(null, new object(), "member");
            collection.Add(binding);

            Assert.Throws<ArgumentNullException>("dataBinding", () => collection.Remove(null));
            Assert.Same(binding, Assert.Single(collection));
        }

        [WinFormsFact]
        public void Remove_NoSuchDataBinding_ThrowsArgumentException()
        {
            using var control = new Control();
            var collection = new ControlBindingsCollection(control);
            var binding1 = new Binding(null, new object(), "member");
            var binding2 = new Binding(null, new object(), "member");
            collection.Add(binding1);

            Assert.Throws<ArgumentException>("dataBinding", () => collection.Remove(binding2));
            Assert.Same(binding1, Assert.Single(collection));
        }

        [WinFormsFact]
        public void Remove_DataBindingFromOtherCollection_ThrowsArgumentException()
        {
            using var control1 = new Control();
            using var control2 = new Control();
            var collection1 = new ControlBindingsCollection(control1);
            var collection2 = new ControlBindingsCollection(control2);
            var binding1 = new Binding(null, new object(), "member");
            var binding2 = new Binding(null, new object(), "member");
            collection1.Add(binding1);
            collection2.Add(binding2);

            Assert.Throws<ArgumentException>("dataBinding", () => collection2.Remove(binding1));
            Assert.Same(binding1, Assert.Single(collection1));
            Assert.Same(binding2, Assert.Single(collection2));
        }

        [WinFormsFact]
        public void RemoveAt_Invoke_Success()
        {
            using var control = new Control();
            var collection = new ControlBindingsCollection(control);
            var binding = new Binding(null, new object(), "member");

            collection.Add(binding);
            Assert.Same(binding, Assert.Single(collection));
            Assert.Same(control, binding.BindableComponent);

            collection.RemoveAt(0);
            Assert.Empty(collection);
            Assert.Null(binding.BindableComponent);
        }

        [WinFormsTheory]
        [InlineData("text")]
        [InlineData("TEXT")]
        public void Item_PropertyNameExists_ReturnsExpected(string propertyName)
        {
            using var control = new Control();
            var collection = new ControlBindingsCollection(control);
            var binding = new Binding(nameof(Control.Text), new object(), "member");
            collection.Add(binding);
            Assert.Same(binding, collection[propertyName]);
        }

        [WinFormsTheory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("NoSuchProperty")]
        public void Item_NoSuchPropertyName_ReturnsNull(string propertyName)
        {
            using var control = new Control();
            var collection = new ControlBindingsCollection(control);
            var binding = new Binding(nameof(Control.Text), new object(), "member");
            collection.Add(binding);
            Assert.Null(collection[propertyName]);
        }

        private class SubControl : Control
        {
            public string text { get; set; }
        }
    }
}
