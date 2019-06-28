// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Moq;
using WinForms.Common.Tests;
using Xunit;

namespace System.ComponentModel.Design.Tests
{
    public class CollectionFormTests : CollectionEditor
    {
        public CollectionFormTests() : base(typeof(List<int>))
        {
        }

        [Fact]
        public void CollectionForm_Ctor_CollectionEditor()
        {
            var editor = new CollectionEditor(typeof(List<int>));
            var form = new SubCollectionForm(editor);
            Assert.Equal(typeof(int), form.CollectionItemType);
            Assert.Same(form.CollectionItemType, form.CollectionItemType);
            Assert.Equal(typeof(List<int>), form.CollectionType);
            Assert.Null(form.Context);
            Assert.Null(form.EditValue);
            Assert.Empty(form.Items);
            Assert.Equal(new Type[] { typeof(int) }, form.NewItemTypes);
        }

        [Fact]
        public void CollectionForm_Ctor_NullEditor_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>("editor", () => new SubCollectionForm(null));
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringWithNullTheoryData))]
        public void CollectionForm_EditValue_Set_GetReturnsExpected(object value)
        {
            var editor = new CollectionEditor(typeof(int[]));
            var form = new SubCollectionForm(editor)
            {
                EditValue = value
            };
            Assert.Same(value, form.EditValue);
            Assert.Equal(1, form.OnEditValueChangedCallCount);

            // Set same.
            form.EditValue = value;
            Assert.Same(value, form.EditValue);
            Assert.Equal(2, form.OnEditValueChangedCallCount);
        }

        public static IEnumerable<object[]> Items_Set_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { Array.Empty<object>() };
            yield return new object[] { new object[] { 1, 2, 3 } };
        }

        [Theory]
        [MemberData(nameof(Items_Set_TestData))]
        public void CollectionForm_Items_SetWithoutContext_GetReturnsExpected(object[] value)
        {
            var editor = new CollectionEditor(typeof(int[]));
            var form = new SubCollectionForm(editor)
            {
                Items = value
            };
            Assert.Null(form.EditValue);
            Assert.Empty(form.Items);
            Assert.Equal(0, form.OnEditValueChangedCallCount);

            // Set same.
            form.Items = value;
            Assert.Null(form.EditValue);
            Assert.Empty(form.Items);
            Assert.Equal(0, form.OnEditValueChangedCallCount);
        }

        [Theory]
        [MemberData(nameof(Items_Set_TestData))]
        public void CollectionForm_Items_SetWithEditValue_GetReturnsExpected(object[] value)
        {
            var editor = new SubCollectionEditor(typeof(List<int>));
            var form = new SubCollectionForm(editor)
            {
                EditValue = new List<int> { 1, 2 }
            };
            form.OnEditValueChangedCallCount = 0;

            form.Items = value;
            Assert.Equal(new object[] { 1, 2 }, form.EditValue);
            Assert.Equal(new object[] { 1, 2 }, form.Items);
            Assert.Equal(0, form.OnEditValueChangedCallCount);

            // Set same.
            form.Items = value;
            Assert.Equal(new object[] { 1, 2 }, form.EditValue);
            Assert.Equal(new object[] { 1, 2 }, form.Items);
            Assert.Equal(0, form.OnEditValueChangedCallCount);
        }

        [Theory]
        [MemberData(nameof(Items_Set_TestData))]
        public void CollectionForm_Items_SetWithContext_GetReturnsExpected(object[] value)
        {
            var mockEditorService = new Mock<IWindowsFormsEditorService>(MockBehavior.Strict);
            mockEditorService
                .Setup(s => s.ShowDialog(It.IsAny<Form>()))
                .Returns(DialogResult.OK);

            var mockServiceProvider = new Mock<IServiceProvider>(MockBehavior.Strict);
            mockServiceProvider
                .Setup(p => p.GetService(typeof(IWindowsFormsEditorService)))
                .Returns(mockEditorService.Object);

            var mockContext = new Mock<ITypeDescriptorContext>(MockBehavior.Strict);
            mockContext
                .Setup(c => c.GetService(typeof(IDesignerHost)))
                .Returns(null);
            mockContext
                .Setup(c => c.GetService(typeof(AmbientProperties)))
                .Returns(null);
            mockContext
                .Setup(c => c.GetService(typeof(IComponentChangeService)))
                .Returns(null);
            mockContext
                .Setup(c => c.OnComponentChanging())
                .Returns(true)
                .Verifiable();
            mockContext
                .Setup(c => c.OnComponentChanged())
                .Verifiable();

            var editor = new SubCollectionEditor(typeof(List<int>));
            var editValue = new object();
            Assert.Same(editValue, editor.EditValue(mockContext.Object, mockServiceProvider.Object, editValue));
            Assert.Same(mockContext.Object, editor.Context);

            var form = new SubCollectionForm(editor)
            {
                Items = value
            };
            Assert.Null(form.EditValue);
            Assert.Empty(form.Items);
            Assert.Equal(0, form.OnEditValueChangedCallCount);
            mockContext.Verify(c => c.OnComponentChanging(), Times.Once());
            mockContext.Verify(c => c.OnComponentChanged(), Times.Once());

            // Set same.
            form.Items = value;
            Assert.Null(form.EditValue);
            Assert.Empty(form.Items);
            Assert.Equal(0, form.OnEditValueChangedCallCount);
            mockContext.Verify(c => c.OnComponentChanging(), Times.Exactly(2));
            mockContext.Verify(c => c.OnComponentChanged(), Times.Exactly(2));
        }

        [Theory]
        [MemberData(nameof(Items_Set_TestData))]
        public void CollectionForm_Items_SetWithContextWithEditValue_GetReturnsExpected(object[] value)
        {
            var mockEditorService = new Mock<IWindowsFormsEditorService>(MockBehavior.Strict);
            mockEditorService
                .Setup(s => s.ShowDialog(It.IsAny<Form>()))
                .Returns(DialogResult.OK);

            var mockServiceProvider = new Mock<IServiceProvider>(MockBehavior.Strict);
            mockServiceProvider
                .Setup(p => p.GetService(typeof(IWindowsFormsEditorService)))
                .Returns(mockEditorService.Object);

            var mockContext = new Mock<ITypeDescriptorContext>(MockBehavior.Strict);
            mockContext
                .Setup(c => c.GetService(typeof(IDesignerHost)))
                .Returns(null);
            mockContext
                .Setup(c => c.GetService(typeof(AmbientProperties)))
                .Returns(null);
            mockContext
                .Setup(c => c.GetService(typeof(IComponentChangeService)))
                .Returns(null);
            mockContext
                .Setup(c => c.OnComponentChanging())
                .Returns(true)
                .Verifiable();
            mockContext
                .Setup(c => c.OnComponentChanged())
                .Verifiable();

            var editor = new SubCollectionEditor(typeof(List<int>));
            var editValue = new object();
            Assert.Same(editValue, editor.EditValue(mockContext.Object, mockServiceProvider.Object, editValue));
            Assert.Same(mockContext.Object, editor.Context);

            var form = new SubCollectionForm(editor)
            {
                EditValue = new List<int> { 1, 2 }
            };
            form.OnEditValueChangedCallCount = 0;

            form.Items = value;
            Assert.Equal(value ?? Array.Empty<object>(), form.EditValue);
            Assert.Equal(value ?? Array.Empty<object>(), form.Items);
            Assert.Equal(0, form.OnEditValueChangedCallCount);
            mockContext.Verify(c => c.OnComponentChanging(), Times.Once());
            mockContext.Verify(c => c.OnComponentChanged(), Times.Once());

            // Set same.
            form.Items = value;
            Assert.Equal(value ?? Array.Empty<object>(), form.EditValue);
            Assert.Equal(value ?? Array.Empty<object>(), form.Items);
            Assert.Equal(0, form.OnEditValueChangedCallCount);
            mockContext.Verify(c => c.OnComponentChanging(), Times.Exactly(2));
            mockContext.Verify(c => c.OnComponentChanged(), Times.Exactly(2));
        }

        [Theory]
        [MemberData(nameof(Items_Set_TestData))]
        public void CollectionForm_Items_SetWithContextWithEditValueCustomSetItems_GetReturnsExpected(object[] value)
        {
            var mockEditorService = new Mock<IWindowsFormsEditorService>(MockBehavior.Strict);
            mockEditorService
                .Setup(s => s.ShowDialog(It.IsAny<Form>()))
                .Returns(DialogResult.OK);

            var mockServiceProvider = new Mock<IServiceProvider>(MockBehavior.Strict);
            mockServiceProvider
                .Setup(p => p.GetService(typeof(IWindowsFormsEditorService)))
                .Returns(mockEditorService.Object);

            var mockContext = new Mock<ITypeDescriptorContext>(MockBehavior.Strict);
            mockContext
                .Setup(c => c.GetService(typeof(IDesignerHost)))
                .Returns(null);
            mockContext
                .Setup(c => c.GetService(typeof(AmbientProperties)))
                .Returns(null);
            mockContext
                .Setup(c => c.GetService(typeof(IComponentChangeService)))
                .Returns(null);
            mockContext
                .Setup(c => c.OnComponentChanging())
                .Returns(true)
                .Verifiable();
            mockContext
                .Setup(c => c.OnComponentChanged())
                .Verifiable();

            var editor = new CustomSetItemsCollectionEditor(typeof(List<int>));
            var editValue = new object();
            Assert.Same(editValue, editor.EditValue(mockContext.Object, mockServiceProvider.Object, editValue));
            Assert.Same(mockContext.Object, editor.Context);

            var form = new SubCollectionForm(editor)
            {
                EditValue = new List<int> { 1, 2 }
            };
            form.OnEditValueChangedCallCount = 0;

            form.Items = value;
            Assert.Equal("CustomSetItems", form.EditValue);
            Assert.Empty(form.Items);
            Assert.Equal(1, form.OnEditValueChangedCallCount);
            mockContext.Verify(c => c.OnComponentChanging(), Times.Once());
            mockContext.Verify(c => c.OnComponentChanged(), Times.Once());

            // Set same.
            form.Items = value;
            Assert.Equal("CustomSetItems", form.EditValue);
            Assert.Empty(form.Items);
            Assert.Equal(1, form.OnEditValueChangedCallCount);
            mockContext.Verify(c => c.OnComponentChanging(), Times.Exactly(2));
            mockContext.Verify(c => c.OnComponentChanged(), Times.Exactly(2));
        }

        [Theory]
        [MemberData(nameof(Items_Set_TestData))]
        public void CollectionForm_Items_SetWithContextWithEditValueFalseOnChanging_GetReturnsExpected(object[] value)
        {
            var mockEditorService = new Mock<IWindowsFormsEditorService>(MockBehavior.Strict);
            mockEditorService
                .Setup(s => s.ShowDialog(It.IsAny<Form>()))
                .Returns(DialogResult.OK);

            var mockServiceProvider = new Mock<IServiceProvider>(MockBehavior.Strict);
            mockServiceProvider
                .Setup(p => p.GetService(typeof(IWindowsFormsEditorService)))
                .Returns(mockEditorService.Object);

            var mockContext = new Mock<ITypeDescriptorContext>(MockBehavior.Strict);
            mockContext
                .Setup(c => c.GetService(typeof(IDesignerHost)))
                .Returns(null);
            mockContext
                .Setup(c => c.GetService(typeof(AmbientProperties)))
                .Returns(null);
            mockContext
                .Setup(c => c.GetService(typeof(IComponentChangeService)))
                .Returns(null);
            mockContext
                .Setup(c => c.OnComponentChanging())
                .Returns(false)
                .Verifiable();
            mockContext
                .Setup(c => c.OnComponentChanged())
                .Verifiable();

            var editor = new SubCollectionEditor(typeof(List<int>));
            var editValue = new object();
            Assert.Same(editValue, editor.EditValue(mockContext.Object, mockServiceProvider.Object, editValue));
            Assert.Same(mockContext.Object, editor.Context);

            var form = new SubCollectionForm(editor)
            {
                EditValue = new List<int> { 1, 2 }
            };
            form.OnEditValueChangedCallCount = 0;

            form.Items = value;
            Assert.Equal(new object[] { 1, 2 }, form.EditValue);
            Assert.Equal(new object[] { 1, 2 }, form.Items);
            Assert.Equal(0, form.OnEditValueChangedCallCount);
            mockContext.Verify(c => c.OnComponentChanging(), Times.Once());
            mockContext.Verify(c => c.OnComponentChanged(), Times.Never());

            // Set same.
            form.Items = value;
            Assert.Equal(new object[] { 1, 2 }, form.EditValue);
            Assert.Equal(new object[] { 1, 2 }, form.Items);
            Assert.Equal(0, form.OnEditValueChangedCallCount);
            mockContext.Verify(c => c.OnComponentChanging(), Times.Exactly(2));
            mockContext.Verify(c => c.OnComponentChanged(), Times.Never());
        }

        [Theory]
        [MemberData(nameof(Items_Set_TestData))]
        public void CollectionForm_Items_SetWithContextWithEditValueThrowsCriticalExceptionOnChanging_Rethrows(object[] value)
        {
            var mockEditorService = new Mock<IWindowsFormsEditorService>(MockBehavior.Strict);
            mockEditorService
                .Setup(s => s.ShowDialog(It.IsAny<Form>()))
                .Returns(DialogResult.OK);

            var mockServiceProvider = new Mock<IServiceProvider>(MockBehavior.Strict);
            mockServiceProvider
                .Setup(p => p.GetService(typeof(IWindowsFormsEditorService)))
                .Returns(mockEditorService.Object);

            var mockContext = new Mock<ITypeDescriptorContext>(MockBehavior.Strict);
            mockContext
                .Setup(c => c.GetService(typeof(IDesignerHost)))
                .Returns(null);
            mockContext
                .Setup(c => c.GetService(typeof(AmbientProperties)))
                .Returns(null);
            mockContext
                .Setup(c => c.GetService(typeof(IComponentChangeService)))
                .Returns(null);
            mockContext
                .Setup(c => c.OnComponentChanging())
                .Returns(() => throw new StackOverflowException())
                .Verifiable();
            mockContext
                .Setup(c => c.OnComponentChanged())
                .Verifiable();

            var editor = new SubCollectionEditor(typeof(List<int>));
            var editValue = new object();
            Assert.Same(editValue, editor.EditValue(mockContext.Object, mockServiceProvider.Object, editValue));
            Assert.Same(mockContext.Object, editor.Context);

            var form = new SubCollectionForm(editor)
            {
                EditValue = new List<int> { 1, 2 }
            };
            form.OnEditValueChangedCallCount = 0;

            Assert.Throws<StackOverflowException>(() => form.Items = value);
        }

        [Theory]
        [MemberData(nameof(Items_Set_TestData))]
        public void CollectionForm_Items_SetWithContextWithEditValueThrowsCriticalExceptionOnChanging_CallsDisplayError(object[] value)
        {
            var mockEditorService = new Mock<IWindowsFormsEditorService>(MockBehavior.Strict);
            mockEditorService
                .Setup(s => s.ShowDialog(It.IsAny<Form>()))
                .Returns(DialogResult.OK);

            var mockServiceProvider = new Mock<IServiceProvider>(MockBehavior.Strict);
            mockServiceProvider
                .Setup(p => p.GetService(typeof(IWindowsFormsEditorService)))
                .Returns(mockEditorService.Object);

            var exception = new DivideByZeroException();
            var mockService = new Mock<IUIService>(MockBehavior.Strict);
            mockService
                .Setup(s => s.ShowError(exception))
                .Verifiable();

            var mockContext = new Mock<ITypeDescriptorContext>(MockBehavior.Strict);
            mockContext
                .Setup(c => c.GetService(typeof(IDesignerHost)))
                .Returns(null);
            mockContext
                .Setup(c => c.GetService(typeof(AmbientProperties)))
                .Returns(null);
            mockContext
                .Setup(c => c.GetService(typeof(IComponentChangeService)))
                .Returns(null);
            mockContext
                .Setup(c => c.OnComponentChanging())
                .Returns(() => throw exception)
                .Verifiable();
            mockContext
                .Setup(c => c.OnComponentChanged())
                .Verifiable();
            mockContext
                .Setup(c => c.GetService(typeof(IUIService)))
                .Returns(mockService.Object);

            var editor = new SubCollectionEditor(typeof(List<int>));
            var editValue = new object();
            Assert.Same(editValue, editor.EditValue(mockContext.Object, mockServiceProvider.Object, editValue));
            Assert.Same(mockContext.Object, editor.Context);

            var form = new SubCollectionForm(editor)
            {
                EditValue = new List<int> { 1, 2 }
            };
            form.OnEditValueChangedCallCount = 0;

            form.Items = value;
            Assert.Equal(new object[] { 1, 2 }, form.EditValue);
            Assert.Equal(new object[] { 1, 2 }, form.Items);
            Assert.Equal(0, form.OnEditValueChangedCallCount);
            mockContext.Verify(c => c.OnComponentChanging(), Times.Once());
            mockContext.Verify(c => c.OnComponentChanged(), Times.Never());
            mockService.Verify(s => s.ShowError(exception), Times.Once());
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
        public void CollectionForm_CanRemoveInstance_Invoke_ReturnsExpected(object value)
        {
            var editor = new SubCollectionEditor(null);
            var form = new SubCollectionForm(editor);
            Assert.True(form.CanRemoveInstance(value));
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
        public void CollectionForm_CanRemoveInstance_InheritanceAttribute_ReturnsExpected(InheritanceAttribute attribute, bool expected)
        {
            var component = new Component();
            TypeDescriptor.AddAttributes(component, attribute);
            var editor = new SubCollectionEditor(null);
            var form = new SubCollectionForm(editor);
            Assert.Equal(expected, form.CanRemoveInstance(component));
        }

        [Fact]
        public void CollectionForm_CanSelectMultipleInstances_Invoke_ReturnsFalse()
        {
            var editor = new SubCollectionEditor(null);
            var form = new SubCollectionForm(editor);
            Assert.True(form.CanSelectMultipleInstances());
        }
        public static IEnumerable<object[]> InvalidDesignerHost_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new object() };
        }

        [Theory]
        [MemberData(nameof(InvalidDesignerHost_TestData))]
        public void CollectionForm_CreateInstance_WithContextWithInvalidDesignerHost_ReturnsExpected(object host)
        {
            var mockEditorService = new Mock<IWindowsFormsEditorService>(MockBehavior.Strict);
            mockEditorService
                .Setup(s => s.ShowDialog(It.IsAny<Form>()))
                .Returns(DialogResult.OK);

            var mockServiceProvider = new Mock<IServiceProvider>(MockBehavior.Strict);
            mockServiceProvider
                .Setup(p => p.GetService(typeof(IWindowsFormsEditorService)))
                .Returns(mockEditorService.Object);

            var mockContext = new Mock<ITypeDescriptorContext>(MockBehavior.Strict);
            mockContext
                .Setup(c => c.GetService(typeof(IDesignerHost)))
                .Returns(host);
            mockContext
                .Setup(c => c.GetService(typeof(AmbientProperties)))
                .Returns(null);
            mockContext
                .Setup(c => c.GetService(typeof(IComponentChangeService)))
                .Returns(null);

            var editor = new SubCollectionEditor(typeof(List<int>));
            var value = new object();
            Assert.Same(value, editor.EditValue(mockContext.Object, mockServiceProvider.Object, value));
            Assert.Same(mockContext.Object, editor.Context);

            var form = new SubCollectionForm(editor);
            Assert.IsType<Component>(form.CreateInstance(typeof(Component)));
        }

        public static IEnumerable<object[]> CreateInstance_HostDesigner_TestData()
        {
            yield return new object[] { null };

            var mockDesigner = new Mock<IDesigner>(MockBehavior.Strict);
            mockDesigner
                .Setup(d => d.Dispose());
            yield return new object[] { mockDesigner.Object };
        }

        [Theory]
        [MemberData(nameof(CreateInstance_HostDesigner_TestData))]
        public void CollectionForm_CreateInstance_WithContextWithHostReturningComponent_CallsCreateComponent(IDesigner designer)
        {
            var mockEditorService = new Mock<IWindowsFormsEditorService>(MockBehavior.Strict);
            mockEditorService
                .Setup(s => s.ShowDialog(It.IsAny<Form>()))
                .Returns(DialogResult.OK);

            var mockServiceProvider = new Mock<IServiceProvider>(MockBehavior.Strict);
            mockServiceProvider
                .Setup(p => p.GetService(typeof(IWindowsFormsEditorService)))
                .Returns(mockEditorService.Object);

            var result = new Component();
            var mockHost = new Mock<IDesignerHost>(MockBehavior.Strict);
            mockHost
                .Setup(h => h.CreateTransaction("Add or remove Int32 objects"))
                .Returns((DesignerTransaction)null);
            mockHost
                .Setup(c => c.GetService(typeof(IComponentChangeService)))
                .Returns(null);
            mockHost
                .Setup(h => h.CreateComponent(typeof(Component), null))
                .Returns(result);
            mockHost
                .Setup(h => h.GetDesigner(result))
                .Returns(designer);

            var mockContext = new Mock<ITypeDescriptorContext>(MockBehavior.Strict);
            mockContext
                .Setup(c => c.GetService(typeof(IDesignerHost)))
                .Returns(mockHost.Object);
            mockContext
                .Setup(c => c.GetService(typeof(AmbientProperties)))
                .Returns(null);
            mockContext
                .Setup(c => c.GetService(typeof(IComponentChangeService)))
                .Returns(null);

            var editor = new SubCollectionEditor(typeof(List<int>));
            var value = new object();
            Assert.Same(value, editor.EditValue(mockContext.Object, mockServiceProvider.Object, value));
            Assert.Same(mockContext.Object, editor.Context);

            var form = new SubCollectionForm(editor);
            Assert.Same(result, form.CreateInstance(typeof(Component)));
        }

        [Theory]
        [MemberData(nameof(CreateInstance_HostDesigner_TestData))]
        public void CollectionForm_CreateInstance_WithContextWithHostReturningNullComponent_CallsCreateComponent(IDesigner designer)
        {
            var mockEditorService = new Mock<IWindowsFormsEditorService>(MockBehavior.Strict);
            mockEditorService
                .Setup(s => s.ShowDialog(It.IsAny<Form>()))
                .Returns(DialogResult.OK);

            var mockServiceProvider = new Mock<IServiceProvider>(MockBehavior.Strict);
            mockServiceProvider
                .Setup(p => p.GetService(typeof(IWindowsFormsEditorService)))
                .Returns(mockEditorService.Object);

            var mockHost = new Mock<IDesignerHost>(MockBehavior.Strict);
            mockHost
                .Setup(h => h.CreateTransaction("Add or remove Int32 objects"))
                .Returns((DesignerTransaction)null);
            mockHost
                .Setup(c => c.GetService(typeof(IComponentChangeService)))
                .Returns(null);
            mockHost
                .Setup(h => h.CreateComponent(typeof(Component), null))
                .Returns((IComponent)null);
            mockHost
                .Setup(h => h.GetDesigner(null))
                .Returns(designer);
            mockHost
                .Setup(c => c.GetService(typeof(TypeDescriptionProvider)))
                .Returns(null);

            var result = new object();
            var mockContext = new Mock<ITypeDescriptorContext>(MockBehavior.Strict);
            mockContext
                .Setup(c => c.GetService(typeof(IDesignerHost)))
                .Returns(mockHost.Object);
            mockContext
                .Setup(c => c.GetService(typeof(AmbientProperties)))
                .Returns(null);
            mockContext
                .Setup(c => c.GetService(typeof(IComponentChangeService)))
                .Returns(null);

            var editor = new SubCollectionEditor(typeof(List<int>));
            var value = new object();
            Assert.Same(value, editor.EditValue(mockContext.Object, mockServiceProvider.Object, value));
            Assert.Same(mockContext.Object, editor.Context);

            var form = new SubCollectionForm(editor);
            Assert.IsType<Component>(form.CreateInstance(typeof(Component)));
        }

        [Fact]
        public void CollectionForm_CreateInstance_WithContextWithHostReturningComponentWithIComponentInitializerDesigner_CallsInitializeNewComponent()
        {
            var mockEditorService = new Mock<IWindowsFormsEditorService>(MockBehavior.Strict);
            mockEditorService
                .Setup(s => s.ShowDialog(It.IsAny<Form>()))
                .Returns(DialogResult.OK);

            var mockServiceProvider = new Mock<IServiceProvider>(MockBehavior.Strict);
            mockServiceProvider
                .Setup(p => p.GetService(typeof(IWindowsFormsEditorService)))
                .Returns(mockEditorService.Object);

            var result = new Component();
            var mockDesigner = new Mock<IDesigner>(MockBehavior.Strict);
            Mock<IComponentInitializer> mockComponentInitializer = mockDesigner.As<IComponentInitializer>();
            mockComponentInitializer
                .Setup(d => d.InitializeNewComponent(null))
                .Verifiable();

            var mockHost = new Mock<IDesignerHost>(MockBehavior.Strict);
            mockHost
                .Setup(h => h.CreateTransaction("Add or remove Int32 objects"))
                .Returns((DesignerTransaction)null);
            mockHost
                .Setup(c => c.GetService(typeof(IComponentChangeService)))
                .Returns(null);
            mockHost
                .Setup(h => h.CreateComponent(typeof(Component), null))
                .Returns(result);
            mockHost
                .Setup(h => h.GetDesigner(result))
                .Returns(mockDesigner.Object);

            var mockContext = new Mock<ITypeDescriptorContext>(MockBehavior.Strict);
            mockContext
                .Setup(c => c.GetService(typeof(IDesignerHost)))
                .Returns(mockHost.Object);
            mockContext
                .Setup(c => c.GetService(typeof(AmbientProperties)))
                .Returns(null);
            mockContext
                .Setup(c => c.GetService(typeof(IComponentChangeService)))
                .Returns(null);

            var editor = new SubCollectionEditor(typeof(List<int>));
            var value = new object();
            Assert.Same(value, editor.EditValue(mockContext.Object, mockServiceProvider.Object, value));
            Assert.Same(mockContext.Object, editor.Context);

            var form = new SubCollectionForm(editor);
            Assert.Same(result, form.CreateInstance(typeof(Component)));
            mockComponentInitializer.Verify(d => d.InitializeNewComponent(null), Times.Once());
        }

        [Fact]
        public void CollectionForm_CreateInstance_InvokeWithoutContext_ReturnsExpected()
        {
            var editor = new SubCollectionEditor(null);
            var form = new SubCollectionForm(editor);
            Assert.Equal(0, form.CreateInstance(typeof(int)));
        }

        [Fact]
        public void CollectionForm_CreateInstance_NullItemType_ThrowsArgumentNullException()
        {
            var editor = new SubCollectionEditor(null);
            var form = new SubCollectionForm(editor);
            Assert.Throws<ArgumentNullException>("objectType", () => form.CreateInstance(null));
        }

        [Fact]
        public void CollectionForm_DisplayError_InvokeWithContextWithIUIService_CallsShowError()
        {
            var mockEditorService = new Mock<IWindowsFormsEditorService>(MockBehavior.Strict);
            mockEditorService
                .Setup(s => s.ShowDialog(It.IsAny<Form>()))
                .Returns(DialogResult.OK);

            var mockServiceProvider = new Mock<IServiceProvider>(MockBehavior.Strict);
            mockServiceProvider
                .Setup(p => p.GetService(typeof(IWindowsFormsEditorService)))
                .Returns(mockEditorService.Object);

            var exception = new Exception();
            var mockService = new Mock<IUIService>(MockBehavior.Strict);
            mockService
                .Setup(s => s.ShowError(exception))
                .Verifiable();

            var mockContext = new Mock<ITypeDescriptorContext>(MockBehavior.Strict);
            mockContext
                .Setup(c => c.GetService(typeof(IDesignerHost)))
                .Returns(null);
            mockContext
                .Setup(c => c.GetService(typeof(AmbientProperties)))
                .Returns(null);
            mockContext
                .Setup(c => c.GetService(typeof(IComponentChangeService)))
                .Returns(null);
            mockContext
                .Setup(c => c.GetService(typeof(IUIService)))
                .Returns(mockService.Object);

            var editor = new SubCollectionEditor(typeof(List<int>));
            var editValue = new object();
            Assert.Same(editValue, editor.EditValue(mockContext.Object, mockServiceProvider.Object, editValue));
            Assert.Same(mockContext.Object, editor.Context);

            var form = new SubCollectionForm(editor);
            form.DisplayError(exception);
            mockService.Verify(s => s.ShowError(exception), Times.Once());
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetTypeWithNullTheoryData))]
        public void CollectionForm_GetService_WithContext_CallsContextGetService(Type serviceType)
        {
            var mockEditorService = new Mock<IWindowsFormsEditorService>(MockBehavior.Strict);
            mockEditorService
                .Setup(s => s.ShowDialog(It.IsAny<Form>()))
                .Returns(DialogResult.OK);

            var mockServiceProvider = new Mock<IServiceProvider>(MockBehavior.Strict);
            mockServiceProvider
                .Setup(p => p.GetService(typeof(IWindowsFormsEditorService)))
                .Returns(mockEditorService.Object);

            var result = new object();
            var mockContext = new Mock<ITypeDescriptorContext>(MockBehavior.Strict);
            mockContext
                .Setup(c => c.GetService(typeof(IDesignerHost)))
                .Returns(null);
            mockContext
                .Setup(c => c.GetService(typeof(AmbientProperties)))
                .Returns(null);
            mockContext
                .Setup(c => c.GetService(typeof(IComponentChangeService)))
                .Returns(null);
            mockContext
                .Setup(c => c.GetService(serviceType))
                .Returns(result);

            var editor = new SubCollectionEditor(typeof(List<int>));
            var value = new object();
            Assert.Same(value, editor.EditValue(mockContext.Object, mockServiceProvider.Object, value));
            Assert.Same(mockContext.Object, editor.Context);

            var form = new SubCollectionForm(editor);
            Assert.Same(result, form.GetService(serviceType));
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetTypeWithNullTheoryData))]
        public void CollectionForm_GetService_InvokeWithoutContext_ReturnsNull(Type serviceType)
        {
            var editor = new SubCollectionEditor(serviceType);
            var form = new SubCollectionForm(editor);
            Assert.Null(form.GetService(serviceType));
        }

        [Fact]
        public void CollectionForm_ShowEditorDialog_Invoke_Success()
        {
            var editor = new SubCollectionEditor(typeof(List<int>));
            var form = new SubCollectionForm(editor);

            var mockEditorService = new Mock<IWindowsFormsEditorService>(MockBehavior.Strict);
            mockEditorService
                .Setup(s => s.ShowDialog(form))
                .Returns(DialogResult.OK);
            Assert.Equal(DialogResult.OK, form.ShowEditorDialog(mockEditorService.Object));
            mockEditorService.Verify(s => s.ShowDialog(form), Times.Once());
        }

        [Fact]
        public void CollectionForm_ShowEditorDialog_NullEdSvc_ThrowsArgumentNullException()
        {
            var editor = new SubCollectionEditor(typeof(List<int>));
            var form = new SubCollectionForm(editor);
            Assert.Throws<ArgumentNullException>("edSvc", () => form.ShowEditorDialog(null));
        }

        private class SubCollectionEditor : CollectionEditor
        {
            public SubCollectionEditor(Type type) : base(type)
            {
            }

            public new ITypeDescriptorContext Context => base.Context;
        }

        private class CustomSetItemsCollectionEditor : CollectionEditor
        {
            public CustomSetItemsCollectionEditor(Type type) : base(type)
            {
            }

            public new ITypeDescriptorContext Context => base.Context;

            protected override object SetItems(object editValue, object[] value) => "CustomSetItems";
        }

        private class SubCollectionForm : CollectionForm
        {
            public SubCollectionForm(CollectionEditor editor) : base(editor)
            {
            }

            public new Type CollectionItemType => base.CollectionItemType;

            public new Type CollectionType => base.CollectionType;

            public new ITypeDescriptorContext Context => base.Context;

            public new object[] Items
            {
                get => base.Items;
                set => base.Items = value;
            }

            public new Type[] NewItemTypes => base.NewItemTypes;

            public new bool CanRemoveInstance(object value) => base.CanRemoveInstance(value);

            public new bool CanSelectMultipleInstances() => base.CanSelectMultipleInstances();

            public new object CreateInstance(Type itemType) => base.CreateInstance(itemType);

            public new void DestroyInstance(object instance) => base.DestroyInstance(instance);

            public new void DisplayError(Exception e) => base.DisplayError(e);

            public new object GetService(Type serviceType) => base.GetService(serviceType);

            public new DialogResult ShowEditorDialog(IWindowsFormsEditorService edSvc) => base.ShowEditorDialog(edSvc);

            public int OnEditValueChangedCallCount { get; set; }

            protected override void OnEditValueChanged() => OnEditValueChangedCallCount++;
        }
    }
}
