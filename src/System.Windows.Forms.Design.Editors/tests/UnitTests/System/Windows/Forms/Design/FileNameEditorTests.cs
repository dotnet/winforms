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
    public class FileNameEditorTests : IClassFixture<ThreadExceptionFixture>
    {
        [Fact]
        public void FileNameEditor_Ctor_Default()
        {
            var editor = new FileNameEditor();
            Assert.False(editor.IsDropDownResizable);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEditValueInvalidProviderTestData))]
        public void FileNameEditor_EditValue_InvalidProvider_ReturnsValue(IServiceProvider provider, object value)
        {
            var editor = new FileNameEditor();
            Assert.Same(value, editor.EditValue(null, provider, value));
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetITypeDescriptorContextTestData))]
        public void FileNameEditor_GetEditStyle_Invoke_ReturnsModal(ITypeDescriptorContext context)
        {
            var editor = new FileNameEditor();
            Assert.Equal(UITypeEditorEditStyle.Modal, editor.GetEditStyle(context));
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetITypeDescriptorContextTestData))]
        public void FileNameEditor_GetPaintValueSupported_Invoke_ReturnsFalse(ITypeDescriptorContext context)
        {
            var editor = new FileNameEditor();
            Assert.False(editor.GetPaintValueSupported(context));
        }

        [Fact]
        public void FileNameEditor_InitializeDialog_Invoke_Success()
        {
            var editor = new SubFileNameEditor();
            using (var openFileDialog = new OpenFileDialog())
            {
                editor.InitializeDialog(openFileDialog);
                Assert.Equal("All Files(*.*)|*.*", openFileDialog.Filter);
                Assert.Equal("Open File", openFileDialog.Title);
            }
        }

        [Fact]
        public void FileNameEditor_InitializeDialog_NullOpenFileDialog_ThrowsArgumentNullException()
        {
            var editor = new SubFileNameEditor();
            Assert.Throws<ArgumentNullException>("openFileDialog", () => editor.InitializeDialog(null));
        }

        private class SubFileNameEditor : FileNameEditor
        {
            public new void InitializeDialog(OpenFileDialog openFileDialog) => base.InitializeDialog(openFileDialog);
        }
    }
}
