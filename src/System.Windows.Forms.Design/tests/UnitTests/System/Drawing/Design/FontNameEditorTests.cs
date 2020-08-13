// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Moq;
using WinForms.Common.Tests;
using Xunit;

namespace System.Drawing.Design.Tests
{
    public class FontNameEditorTests : IClassFixture<ThreadExceptionFixture>
    {
        private readonly ITypeDescriptorContext _typeDescriptorContext;

        public FontNameEditorTests()
        {
            _typeDescriptorContext = new Mock<ITypeDescriptorContext>(MockBehavior.Strict).Object;
        }

        [Fact]
        public void FontNameEditor_Ctor_Default()
        {
            var editor = new FontNameEditor();
            Assert.False(editor.IsDropDownResizable);
        }

        public static IEnumerable<object[]> EditValue_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { "value" };
            yield return new object[] { new object() };
        }

        [Theory]
        [MemberData(nameof(EditValue_TestData))]
        public void FontNameEditor_EditValue_ValidProvider_ReturnsValue(object value)
        {
            var editor = new FontNameEditor();
            var mockEditorService = new Mock<IWindowsFormsEditorService>(MockBehavior.Strict);
            var mockServiceProvider = new Mock<IServiceProvider>(MockBehavior.Strict);
            mockServiceProvider
                .Setup(p => p.GetService(typeof(IWindowsFormsEditorService)))
                .Returns(mockEditorService.Object)
                .Verifiable();
            Assert.Same(value, editor.EditValue(null, mockServiceProvider.Object, value));
            mockServiceProvider.Verify(p => p.GetService(typeof(IWindowsFormsEditorService)), Times.Never());

            // Edit again.
            Assert.Same(value, editor.EditValue(null, mockServiceProvider.Object, value));
            mockServiceProvider.Verify(p => p.GetService(typeof(IWindowsFormsEditorService)), Times.Never());
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEditValueInvalidProviderTestData))]
        public void FontNameEditor_EditValue_InvalidProvider_ReturnsValue(IServiceProvider provider, object value)
        {
            var editor = new FontNameEditor();
            Assert.Same(value, editor.EditValue(null, provider, value));
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetITypeDescriptorContextTestData))]
        public void FontNameEditor_GetEditStyle_Invoke_ReturnsNone(ITypeDescriptorContext context)
        {
            var editor = new FontNameEditor();
            Assert.Equal(UITypeEditorEditStyle.None, editor.GetEditStyle(context));
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetITypeDescriptorContextTestData))]
        public void FontNameEditor_GetPaintValueSupported_Invoke_ReturnsTrue(ITypeDescriptorContext context)
        {
            var editor = new FontNameEditor();
            Assert.True(editor.GetPaintValueSupported(context));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("\t")]
        public void FontNameEditor_PaintValue_ReturnsEarly_InvalidPaintValueEventArgsValue(string fontName)
        {
            PaintValueEventArgs e;
            using (var bitmap = new Bitmap(1, 1))
            {
                using (var g = Graphics.FromImage(bitmap))
                {
                    e = new PaintValueEventArgs(_typeDescriptorContext, fontName, g, Rectangle.Empty);
                }
            }

            // assert by the virtue of calling the method
            // if the impementation is incorrect, having disposed of the Graphics object
            // we would received an AE attempting to call e.Graphics.FillRectangle()
            var editor = new FontNameEditor();
            editor.PaintValue(e);
        }
    }
}
