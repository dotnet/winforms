// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Windows.Forms.TestUtilities;
using Xunit;

namespace System.Drawing.Design.Tests
{
    public class FontEditorTests : IClassFixture<ThreadExceptionFixture>
    {
        [Fact]
        public void FontEditor_Ctor_Default()
        {
            var editor = new FontEditor();
            Assert.False(editor.IsDropDownResizable);
        }

        [Theory]
        [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetEditValueInvalidProviderTestData))]
        public void FontEditor_EditValue_InvalidProvider_ReturnsValue(IServiceProvider provider, object value)
        {
            var editor = new FontEditor();
            Assert.Same(value, editor.EditValue(null, provider, value));
        }

        [Theory]
        [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetITypeDescriptorContextTestData))]
        public void FontEditor_GetEditStyle_Invoke_ReturnsModal(ITypeDescriptorContext context)
        {
            var editor = new FontEditor();
            Assert.Equal(UITypeEditorEditStyle.Modal, editor.GetEditStyle(context));
        }

        [Theory]
        [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetITypeDescriptorContextTestData))]
        public void FontEditor_GetPaintValueSupported_Invoke_ReturnsFalse(ITypeDescriptorContext context)
        {
            var editor = new FontEditor();
            Assert.False(editor.GetPaintValueSupported(context));
        }
    }
}
