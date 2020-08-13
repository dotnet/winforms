// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Moq;
using WinForms.Common.Tests;
using Xunit;

namespace System.Windows.Forms.Design.Tests
{
    public class DockEditorTests : IClassFixture<ThreadExceptionFixture>
    {
        [Fact]
        public void DockEditor_Ctor_Default()
        {
            var editor = new DockEditor();
            Assert.False(editor.IsDropDownResizable);
        }

        public static IEnumerable<object[]> EditValue_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { "value" };
            yield return new object[] { DockStyle.Top };
            yield return new object[] { new object() };
        }

        [Theory]
        [MemberData(nameof(EditValue_TestData))]
        public void DockEditor_EditValue_ValidProvider_ReturnsValue(object value)
        {
            var editor = new DockEditor();
            var mockEditorService = new Mock<IWindowsFormsEditorService>(MockBehavior.Strict);
            var mockServiceProvider = new Mock<IServiceProvider>(MockBehavior.Strict);
            mockServiceProvider
                .Setup(p => p.GetService(typeof(IWindowsFormsEditorService)))
                .Returns(mockEditorService.Object)
                .Verifiable();
            mockEditorService
                .Setup(e => e.DropDownControl(It.IsAny<Control>()))
                .Verifiable();
            Assert.Equal(value, editor.EditValue(null, mockServiceProvider.Object, value));
            mockServiceProvider.Verify(p => p.GetService(typeof(IWindowsFormsEditorService)), Times.Once());
            mockEditorService.Verify(e => e.DropDownControl(It.IsAny<Control>()), Times.Once());

            // Edit again.
            Assert.Equal(value, editor.EditValue(null, mockServiceProvider.Object, value));
            mockServiceProvider.Verify(p => p.GetService(typeof(IWindowsFormsEditorService)), Times.Exactly(2));
            mockServiceProvider.Verify(p => p.GetService(typeof(IWindowsFormsEditorService)), Times.Exactly(2));
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEditValueInvalidProviderTestData))]
        public void DockEditor_EditValue_InvalidProvider_ReturnsValue(IServiceProvider provider, object value)
        {
            var editor = new DockEditor();
            Assert.Same(value, editor.EditValue(null, provider, value));
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetITypeDescriptorContextTestData))]
        public void DockEditor_GetEditStyle_Invoke_ReturnsModal(ITypeDescriptorContext context)
        {
            var editor = new DockEditor();
            Assert.Equal(UITypeEditorEditStyle.DropDown, editor.GetEditStyle(context));
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetITypeDescriptorContextTestData))]
        public void DockEditor_GetPaintValueSupported_Invoke_ReturnsFalse(ITypeDescriptorContext context)
        {
            var editor = new DockEditor();
            Assert.False(editor.GetPaintValueSupported(context));
        }
    }
}
