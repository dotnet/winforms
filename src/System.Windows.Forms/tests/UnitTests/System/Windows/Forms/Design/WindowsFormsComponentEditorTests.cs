// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using Moq;
using WinForms.Common.Tests;
using Xunit;

namespace System.Windows.Forms.Design.Tests
{
    // NB: doesn't require thread affinity
    public class WindowsFormsComponentEditorTests : IClassFixture<ThreadExceptionFixture>
    {
        public static IEnumerable<object[]> EditComponent_ObjectIWin32Window_TestData()
        {
            yield return new object[] { null, null, null };
            yield return new object[] { Array.Empty<Type>(), null, null };

            var mockWindow = new Mock<IWin32Window>(MockBehavior.Strict);
            yield return new object[] { null, new object(), mockWindow.Object };
            yield return new object[] { Array.Empty<Type>(), new object(), mockWindow.Object };
        }

        [Theory]
        [MemberData(nameof(EditComponent_ObjectIWin32Window_TestData))]
        public void WindowsFormsComponentEditor_EditComponent_InvokeObjectIWin32Window_ReturnsFalse(Type[] pages, object component, IWin32Window owner)
        {
            var editor = new CustomWindowsFormsComponentEditor
            {
                GetComponentEditorPagesResult = pages
            };
            Assert.False(editor.EditComponent(component, owner));
        }

        public static IEnumerable<object[]> EditComponent_ITypeDescriptorContextObject_TestData()
        {
            yield return new object[] { null, null, null };
            yield return new object[] { Array.Empty<Type>(), null, null };

            var mockContext = new Mock<ITypeDescriptorContext>(MockBehavior.Strict);
            yield return new object[] { null, mockContext.Object, new object() };
            yield return new object[] { Array.Empty<Type>(), mockContext.Object, new object() };
        }

        [Theory]
        [MemberData(nameof(EditComponent_ITypeDescriptorContextObject_TestData))]
        public void WindowsFormsComponentEditor_EditComponent_InvokeITypeDescriptorContextObject_ReturnsFalse(Type[] pages, ITypeDescriptorContext context, object component)
        {
            var editor = new CustomWindowsFormsComponentEditor
            {
                GetComponentEditorPagesResult = pages
            };
            Assert.False(editor.EditComponent(context, component));
        }

        public static IEnumerable<object[]> EditComponent_ITypeDescriptorContextObjectIWin32Window_TestData()
        {
            yield return new object[] { null, null, null, null };
            yield return new object[] { Array.Empty<Type>(), null, null, null };

            var mockContext = new Mock<ITypeDescriptorContext>(MockBehavior.Strict);
            var mockWindow = new Mock<IWin32Window>(MockBehavior.Strict);
            yield return new object[] { null, mockContext.Object, new object(), mockWindow.Object };
            yield return new object[] { Array.Empty<Type>(), mockContext.Object, new object(), mockWindow.Object };
        }

        [Theory]
        [MemberData(nameof(EditComponent_ITypeDescriptorContextObjectIWin32Window_TestData))]
        public void WindowsFormsComponentEditor_EditComponent_InvokeITypeDescriptorContextObjectIWin32Window_ReturnsFalse(Type[] pages, ITypeDescriptorContext context, object component, IWin32Window owner)
        {
            var editor = new CustomWindowsFormsComponentEditor
            {
                GetComponentEditorPagesResult = pages
            };
            Assert.False(editor.EditComponent(context, component, owner));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("component")]
        public void EditComponent_NonComponentWithPages_ThrowsArgumentException(object component)
        {
            var editor = new CustomWindowsFormsComponentEditor
            {
                GetComponentEditorPagesResult = new Type[] { typeof(int) }
            };
            Assert.Throws<ArgumentException>("component", () => editor.EditComponent(component, null));
            Assert.Throws<ArgumentException>("component", () => editor.EditComponent(null, component));
            Assert.Throws<ArgumentException>("component", () => editor.EditComponent(null, component, null));
        }

        [Fact]
        public void WindowsFormsComponentEditor_GetComponentEditorPages_Invoke_ReturnsNull()
        {
            var editor = new SubWindowsFormsComponentEditor();
            Assert.Null(editor.GetComponentEditorPages());
        }

        [Fact]
        public void WindowsFormsComponentEditor_GetInitialComponentEditorPageIndex_Invoke_ReturnsZero()
        {
            var editor = new SubWindowsFormsComponentEditor();
            Assert.Equal(0, editor.GetInitialComponentEditorPageIndex());
        }

        private class SubWindowsFormsComponentEditor : WindowsFormsComponentEditor
        {
            public new Type[] GetComponentEditorPages() => base.GetComponentEditorPages();

            public new int GetInitialComponentEditorPageIndex() => base.GetInitialComponentEditorPageIndex();
        }

        private class CustomWindowsFormsComponentEditor : WindowsFormsComponentEditor
        {
            public Type[] GetComponentEditorPagesResult { get; set; }

            protected override Type[] GetComponentEditorPages() => GetComponentEditorPagesResult;
        }
    }
}
