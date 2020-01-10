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

namespace System.Drawing.Design.Tests
{
    public class ColorEditorTests : IClassFixture<ThreadExceptionFixture>
    {
        [Fact]
        public void ColorEditor_Ctor_Default()
        {
            var editor = new ColorEditor();
            Assert.False(editor.IsDropDownResizable);
        }

        public static IEnumerable<object[]> EditValue_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { "value" };
            yield return new object[] { Color.Empty };
            yield return new object[] { Color.Red };
            yield return new object[] { new object() };
        }

        [Theory]
        [MemberData(nameof(EditValue_TestData))]
        public void ColorEditor_EditValue_ValidProvider_ReturnsValue(object value)
        {
            var editor = new ColorEditor();
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
        public void ColorEditor_EditValue_InvalidProvider_ReturnsValue(IServiceProvider provider, object value)
        {
            var editor = new ColorEditor();
            Assert.Same(value, editor.EditValue(null, provider, value));
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetITypeDescriptorContextTestData))]
        public void ColorEditor_GetEditStyle_Invoke_ReturnsModal(ITypeDescriptorContext context)
        {
            var editor = new ColorEditor();
            Assert.Equal(UITypeEditorEditStyle.DropDown, editor.GetEditStyle(context));
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetITypeDescriptorContextTestData))]
        public void ColorEditor_GetPaintValueSupported_Invoke_ReturnsTrue(ITypeDescriptorContext context)
        {
            var editor = new ColorEditor();
            Assert.True(editor.GetPaintValueSupported(context));
        }
    }
}
